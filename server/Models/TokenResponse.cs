namespace server.Models;

// Represents the response containing an access token for authentication
public record TokenResponse(
    string AccessToken
);