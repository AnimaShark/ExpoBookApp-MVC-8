Write-Host "🛠️ Building React app..."
npm run build

if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ React build failed."
    exit $LASTEXITCODE
}

Write-Host "📁 Copying build output to wwwroot/react..."

$distPath = "$PSScriptRoot\dist"
$targetPath = "..\wwwroot\react"

# Remove existing react build in wwwroot/react (optional)
if (Test-Path $targetPath) {
    Remove-Item -Recurse -Force $targetPath
}

# Copy new build
Copy-Item -Path $distPath -Destination $targetPath -Recurse -Force

Write-Host "✅ Done! React build copied to ASP.NET Core wwwroot."
