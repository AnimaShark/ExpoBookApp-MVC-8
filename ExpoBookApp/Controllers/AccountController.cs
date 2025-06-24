using ExpoBookApp.Models;
using ExpoBookApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpoBookApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly PasswordService _passwordService;
        private readonly EmailService _emailService;

        public AccountController(AppDbContext context, PasswordService passwordService, EmailService emailService)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _passwordService = passwordService;
            _emailService = emailService;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string role)
        {
            if (_context.Users.Where(u => u.Email == email).Any())
            {
                ModelState.AddModelError("email", "Email already registered.");
                return View();
            }

            var user = new User
            {
                Email = email
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            user.Role = role;

           // Generate token
            var token = Guid.NewGuid().ToString();
            user.EmailConfirmationToken = token;
            _context.Users.Add(user);
            user.IsEmailConfirmed = false; // Ensure email confirmation is required
            await _context.SaveChangesAsync();

            // Send activation link
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { token }, Request.Scheme);
            await _emailService.SendActivationEmail(email, token);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }


        // Account/ConfirmEmail
        public IActionResult ConfirmEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Invalid confirmation token.");

            var user = _context.Users.FirstOrDefault(u => u.EmailConfirmationToken == token);

            if (user == null)
                return NotFound("User not found or token invalid.");

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null;
            _context.SaveChanges();

            return View("ConfirmEmail"); 
        }


        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("Login", "Invalid login attempt.");
                return View();
            }
            if (!user.IsEmailConfirmed)
            {
                ModelState.AddModelError("", "Please confirm your email before logging in.");
                return View();
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("Login", "Invalid login attempt.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // keeps cookie between browser sessions
            };

            await HttpContext.SignInAsync("MyCookieAuth", principal, authProperties);

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ForgetPassword
        [HttpGet]
        public IActionResult ForgetPassword() => View();

        // POST: /Account/ForgetPassword
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return RedirectToAction("ForgotPasswordConfirmation"); // don't reveal user existence

            // Create a token (GUID or secure random)
            var token = Guid.NewGuid().ToString();

            // Save it to DB with expiry (or use a Token table)
            user.PasswordResetToken = token;
            user.TokenExpiry = DateTime.UtcNow.AddHours(1);
            _context.SaveChanges();

            // Send email with reset link (e.g., https://yourdomain.com/Account/ResetPassword?token=xxx)
            _emailService.SendResetEmail(user.Email, token);

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid password reset token.");
            }

            var user = _context.Users
                .FirstOrDefault(u => u.PasswordResetToken == token && u.TokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                return BadRequest("Token is invalid or expired.");
            }

            return View(new ResetPwd { PasswordResetToken = token });
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPwd model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users
                .FirstOrDefault(u => u.PasswordResetToken == model.PasswordResetToken && u.TokenExpiry > DateTime.UtcNow);

            if (user == null)
            {
                return BadRequest("Token is invalid or expired.");
            }

            var hashed = _passwordService.HashPassword(user, model.NewPassword);

            // Check if new password was previously used
            if (_passwordService.IsInPasswordHistory(user, model.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "You cannot reuse a recent password.");
                return View(model);
            }

            // Save new password and update password history
            user.PasswordHash = hashed;
            user.PasswordResetToken = null;
            user.TokenExpiry = null;
            _context.SaveChanges();

            _passwordService.SavePasswordHistory(user.Id, hashed);

            //Check if user already changed the password 5 times


            return RedirectToAction("Login");
        }

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }


    }
}
