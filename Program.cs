using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();

var app = builder.Build();
app.UseAuthorization();

app.MapGet("/", (ClaimsPrincipal user) => $"Hello User: {user?.Identity?.Name}");

app.Run();
