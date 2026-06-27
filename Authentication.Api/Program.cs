using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("jwt");

// add services
builder.Services.AddAuthorization();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.IncludeErrorDetails = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["key"] ?? ""))

    };
});

var app = builder.Build();



app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", (HttpRequest request) => Results.Text($$"""
<html>
<body>
Hello, World! Authentication Scheme: {{JwtBearerDefaults.AuthenticationScheme}}
JWT:<br/>
<div id="jwt_content"></div>
<br/><br/>
Response from <a href="/secret">/secret</a>
<div id="message"></div>
<br/><br/>

<button id="jwt">Get Secret</button>

<script>
 let btn = document.getElementById('jwt');
 btn.addEventListener('click', async function() {
    const url = window.location.protocol + '//' + window.location.host  + "/jwt";
    const response = await fetch(url,  {
        headers: { 'Accept': 'application/json' }
    });

    const json = await response.json();
    document.getElementById('jwt_content').textContent = json.token;

    const url2 = window.location.protocol + '//' + window.location.host  + "/secret";
    const response2 = await fetch(url2,  {
        headers: { 'Accept': 'text/plain', 'Authorization': 'Bearer ' + json.token, }
    });

    const text =  await response2.text();

    document.getElementById('message').textContent = text;
 });
</script>

</body>
</html>
""", "text/html"));


// secret
app.MapGet("/secret", (ClaimsPrincipal user) => $"Hello {user.Identity?.Name}. This is secret !").RequireAuthorization();

//jwt
app.MapGet("/jwt", () => Results.Json(new { token = GenerateJsonWebToken() }));


string GenerateJsonWebToken()
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["key"] ?? ""));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: jwtSettings["Issuer"],
     audience: jwtSettings["Audience"],
      claims: new[]
    {
            new Claim(ClaimTypes.Name, "Eshan")
    },
    notBefore: null,
    expires: DateTime.Now.AddMinutes(int.TryParse(jwtSettings["ExpireMinutes"], out var expireMinutes) ? expireMinutes : 60),
    signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);

}


app.Run();
