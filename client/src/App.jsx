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

async function clearResults() {
  const stateInput = document.getElementById('state');
  const cityInput = document.getElementById('city');
  const zipInput = document.getElementById('zip');
  const resultsDiv = document.querySelector('.results');
  const noDataMessage = document.getElementById('noData');
  stateInput.value = '';
  cityInput.value = '';
  zipInput.value = '';
  setMessage('');
}

  return (
    <>
    <center>
    <h1 class="title">AMI Assessment</h1>
    <p id="subtitle">Enter a location to get current and historical weather data.</p>

    <label>City</label>
    <input type="text" id="city" name="city" placeholder="Enter city name" />
    <br />
    <label>State</label>
    <input type="text" id="state" name="state" placeholder="Enter state name" />
    <br />
    <label>ZIP Code</label>
    <input type="text" id="zip" name="zip" placeholder="Enter ZIP code" />
    <br />

    <div>
    <button onClick={fetchMessage}>Submit</button>
    
    <button id="clearButton" onClick={clearResults}>
      Clear
    </button>
    </div>

    <h2 id="resultsTitle" class="title">Results</h2>

    <div class="results">
      <h3 id="noData">No data yet.</h3>
       <p>{message}</p>
      <p>Weather data will be displayed here after you submit a location.</p>
    </div>
    </center>
    </>
  );
}

export default App;