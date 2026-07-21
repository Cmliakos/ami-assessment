import { useState } from 'react';
import './App.css'

function App() {
const [message, setMessage] = useState('');

async function fetchMessage(event) {
  event.preventDefault();

  const state = document.getElementById('state').value.trim().toUpperCase();
  const city = document.getElementById('city').value.trim();
  const zip = document.getElementById('zip').value.trim();

  try {
    const response = await fetch('/api/weather', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ city, state, zip })
    });
    const data = await response.json();
    setMessage(`City: ${data.city}, State: ${data.state}, ZIP: ${data.zip}, Token: ${data.token}`);
  } catch (error) {
    console.error('Error fetching message:', error);
  }
}

function clearResults() {
  const stateInput = document.getElementById('state');
  const cityInput = document.getElementById('city');
  const zipInput = document.getElementById('zip');
  stateInput.value = '';
  cityInput.value = '';
  zipInput.value = '';
  setMessage('');
}

  return (
    <>
    <center>
    <h1 className="title">AMI Assessment</h1>
    <p id="subtitle">Enter a location to get current and historical weather data.</p>

    <form onSubmit={fetchMessage}>
    <label>City</label>
    <input 
      type="text"
      id="city"
      name="city"
      required
      pattern="[A-Za-z ]+"
      placeholder="Enter a city name"
    />
    <br />
    <label>State Code</label>
    <input 
      type="text" 
      id="state" 
      name="state" 
      maxLength="2"
      required
      pattern="[A-Za-z]{2}"
      placeholder="Enter a 2-letter state code" 
    />
    <br />
    <label>ZIP Code</label>
    <input 
      type="text" 
      id="zip" 
      name="zip" 
      maxLength="5"
      required
      pattern="[0-9]{5}"
      placeholder="Enter a 5-digit ZIP code" 
    />
    <br />

    <div>
    <button type="submit">Submit</button>
    
    <button type="button" id="clearButton" onClick={clearResults}>
      Clear
    </button>
    </div>
    </form>

    <h2 id="resultsTitle" className="title">Results</h2>

    <div className="results">
      <h3 id="noData">No data yet.</h3>
       <p>{message}</p>
      <p>Weather data will be displayed here after you submit a location.</p>
    </div>
    </center>
    </>
  );
}

export default App;