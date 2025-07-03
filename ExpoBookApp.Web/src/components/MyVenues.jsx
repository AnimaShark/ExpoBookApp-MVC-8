// src/components/MyVenues.jsx
import { useEffect, useState } from 'react';
import axios from 'axios';

export default function MyVenues() {
  const [venues, setVenues] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    axios.get('https://localhost:5001/api/venuesapi/myvenues', { withCredentials: true })
      .then((res) => setVenues(res.data))
      .catch((err) => console.error("Error fetching venues:", err))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <div>Loading venues...</div>;

  return (
    <div className="container mt-4">
      <h2>My Venues</h2>
      {venues.length === 0 ? (
        <div className="alert alert-info">You haven't listed any venues yet.</div>
      ) : (
        <div className="table-responsive">
          <table className="table table-bordered table-hover">
            <thead className="thead-dark">
              <tr>
                <th>Name</th>
                <th>Area</th>
                <th>Seat Spacing</th>
                <th>Created</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {venues.map(v => (
                <tr key={v.id}>
                  <td>{v.name}</td>
                  <td>{v.area} m²</td>
                  <td>{v.seatSpacing} m</td>
                  <td>{v.createdAt}</td>
                  <td>{v.approvalStatus}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
