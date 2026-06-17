using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/authorize", async (AuthorizeRequest request) =>
{
    int lastDigit = Math.Abs(request.Amount) % 10;

    if (lastDigit == 0)
    {
        return Results.Ok(new AuthorizeResponse
        {
            AuthCode = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant(),
            Status = "APPROVED"
        });
    }

    if (lastDigit == 1)
    {
        return Results.BadRequest(new ErrorResponse { Error = "INVALID_CARD_OR_FUNDS" });
    }

    if (lastDigit == 2)
    {
        await Task.Delay(TimeSpan.FromSeconds(30));
        return Results.StatusCode(504);
    }

    if (lastDigit == 3)
    {
        return Results.StatusCode(503);
    }

    return Results.Ok(new AuthorizeResponse
    {
        AuthCode = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant(),
        Status = "APPROVED"
    });
});

app.Run("http://localhost:5001");

public class AuthorizeRequest
{
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
}

public class AuthorizeResponse
{
    [JsonPropertyName("auth_code")]
    public required string AuthCode { get; set; }

    [JsonPropertyName("status")]
    public required string Status { get; set; }
}

public class ErrorResponse
{
    [JsonPropertyName("error")]
    public required string Error { get; set; }
}
