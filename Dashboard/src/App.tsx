import { useEffect, useState } from 'react'
import './App.css'
import type { Host, Metric } from './api/types';
import { getHosts, getHostMetrics } from './api/client';
import { LineChart, Line, XAxis, YAxis, Tooltip, CartesianGrid, ResponsiveContainer } from "recharts";

function App() {
  const [hosts, setHosts] = useState<Host[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [selectedHostId, setSelectedHostId] = useState<number | null>(null);
  const [metrics, setMetrics] = useState<Metric[]>([]); // You can replace 'any' with a more specific type if you have one

  useEffect(() => {
    getHosts()
      .then(setHosts)
      .catch(e => setError(e.message));
  }, []);

  useEffect(() => {
    if (selectedHostId !== null) {
      getHostMetrics(selectedHostId)
        .then(setMetrics)
        .catch(e => setError(e.message));
    }

  }, [selectedHostId]);

  if (error) return <div>Error: {error}</div>;

  const chartData = metrics.map(m => ({
    time: new Date(m.timestamp).toLocaleTimeString(),
    cpu: m.cpu.details.reduce((sum, c) => sum + c.usage, 0) / m.cpu.details.length,
  }));

  return (
    <div>
      <ul>
        {hosts.map(h => <li key={h.id} onClick={() => setSelectedHostId(h.id)}>{h.hostname}</li>)}
      </ul>

      <ResponsiveContainer width="100%" height={300}>
        <LineChart data={chartData}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="time" />
          <YAxis domain={[0, 100]} />
          <Tooltip />
          <Line type="monotone" dataKey="cpu" stroke="#1FB6A6" dot={false} />
        </LineChart>
      </ResponsiveContainer>
    </div>
    );
}

export default App