using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    // NOTE: IssuerSigningKeys *plural* will contain key from `dotnet user-jwts` so 
    //       do not overwrite IssuerSigningKeys.
    //       It is ok to either assign a new key to IssuerSigningKey (singular) or
    //       concatenate IssuerSigningKeys (plural)
    options.TokenValidationParameters.IssuerSigningKey = mySecurityKey;
});

var app = builder.Build();
app.UseAuthorization();

app.MapGet("/", (ClaimsPrincipal user) => $"Hello User: {user?.Identity?.Name}");
app.MapGet("/secret", (ClaimsPrincipal user) => $"Hello User: {user?.Identity?.Name}").RequireAuthorization();
app.MapGet("/jwt", (string userId) => GenerateToken(userId));

app.Run();


// https://dotnetcoretutorials.com/2020/01/15/creating-and-validating-jwt-tokens-in-asp-net-core/
string GenerateToken(string userId)
{
    // var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
    // var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

    var myIssuer = "http://mysite.com";
    var myAudience = "http://myaudience.com";

    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, userId),
            new Claim(ClaimTypes.NameIdentifier, userId),
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        Issuer = myIssuer,
        Audience = myAudience,
        SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}
