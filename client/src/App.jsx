import { useState } from 'react';
import './App.css'
import cloudy from './assets/cloudy.jpg';
import sunny from './assets/sunny.jpg';
import partlyCloudy from './assets/partlyCloudy.jpg';

function App() {
  const [weather, setWeather] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  let background = null;

  if (weather) {
    if (weather.cloudCoverage < 0.3) {
      background = sunny;
    } else if (weather.cloudCoverage < 0.7) {
      background = partlyCloudy;
    } else {
      background = cloudy;
    }
  }

  async function fetchMessage(event) {
    event.preventDefault();

    const state = document.getElementById('state').value.trim().toUpperCase();
    const city = document.getElementById('city').value.trim();
    const zip = document.getElementById('zip').value.trim();

    try {
      setLoading(true);
      setWeather(null);
      setError('');
      const response = await fetch('/api/weather', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ city, state, zip })
      });
      if (!response.ok) {
        throw new Error(`Unable to retrieve weather data.`);
      }
      const data = await response.json();
      if (!data.current || !data.current[0]) {
        throw new Error('No weather data was returned');
      }

      const returnedWeather = data.current[0];
      returnedWeather.averageHigh = data.averageHigh;

      setWeather(returnedWeather);
    } catch (error) {
      console.error('Error fetching weather data:', error);
      setWeather(null);
      setError(error.message);
    } finally {
      setLoading(false);
    }
  }

  function clearResults() {
    const stateInput = document.getElementById('state');
    const cityInput = document.getElementById('city');
    const zipInput = document.getElementById('zip');
    stateInput.value = '';
    cityInput.value = '';
    zipInput.value = '';
    setWeather(null);
    setError('');
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
            <button type="submit" disabled={loading} id="submitButton">
              {loading ? 'Loading...' : 'Submit'}
            </button>

            <button type="button" id="clearButton" onClick={clearResults} disabled={loading}>
              Clear
            </button>
          </div>
        </form>

        <h2 id="resultsTitle" className="title">Results</h2>

        <div className="results"
          style={{ backgroundImage: `url(${background})` }}
        >

          {loading ? (
            <div className="loading">
              <div className="spinner">☀️</div>
              <p>Loading weather data...</p>
            </div>
          ) :
            weather ? (
              <>
                <h3>{weather.city}, {weather.state}, {weather.zip}</h3>

                <p>Temperature: {weather.temperature}°F</p>

                <p>Cloud Coverage: {Math.round(weather.cloudCoverage * 100)}%</p>

                <p>Wind: {weather.windSpeed} mph at {weather.windDirection}°</p>

                <p>12-Month Average High: {weather.averageHigh}°F</p>

              </>
            ) : error ? (
              <p className="error">Error: {error}</p>
            ) : (
              <>
                <h3 id="noData">No data yet.</h3>
                <p>Weather data will be displayed here after you submit a location.</p>
              </>
            )}
        </div>
      </center>
    </>
  );
}

export default App;