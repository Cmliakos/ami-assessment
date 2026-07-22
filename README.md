# AMI Weather Assessment

This project is a React frontend with an ASP.NET Core backend that displays current weather information and a 12-month average high temperature for an user-specified location.

## Requirements

- .NET SDK
- Node.js

## Running the Application

### Backend

From the server directory:

```bash
dotnet run
```

Leave the backend running.

### Frontend

From the client directory:

```bash
npm install
npm run dev
```

Open the URL displayed by Vite in your browser.

## Notes

- Start the backend before launching the frontend.
- Keep the backend running while using the application.
- Open the URL displayed by Vite after starting the frontend.

## Features

- Weather search by city, state, and ZIP code
- Current temperature
- Cloud coverage
- Wind speed and direction
- 12-month average high temperature
- Client-side input validation
- Loading spinner
- Error handling
- Dynamic weather background