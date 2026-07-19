import { useState } from 'react';
import './App.css'

function App() {
const [message, setMessage] = useState('');

async function fetchMessage() {
  try {
    const response = await fetch('/api/test');
    const data = await response.json();
    setMessage(data.message);
  } catch (error) {
    console.error('Error fetching message:', error);
  }
}

  return (
    <>
    <h1>AMI Assessment</h1>

    <button onClick={fetchMessage}>Fetch Message</button>
    <p>{message}</p>
    </>
  );
}

export default App;