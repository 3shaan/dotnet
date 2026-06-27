using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

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
        ValidIssuer = "Authentication.Api",
        ValidAudience = "http://localhost:5144",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Eshan Ahamed Amar Name. Tomar name ki??"))

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
app.MapGet("/secret", (ClaimsPrincipal user) => $"Hello {user.Identity.Name}. This is secret !").RequireAuthorization();

//jwt
app.MapGet("/jwt", () => Results.Json(new { token = GenerateJsonWebToken() }));


string GenerateJsonWebToken()
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Eshan Ahamed Amar Name. Tomar name ki??"));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: "Authentication.Api",
     audience: "http://localhost:5144",
      claims: new List<Claim>
    {
            new Claim(ClaimTypes.Name, "Eshan")
    },
    notBefore: null,
    expires: DateTime.Now.AddMinutes(120),
    signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);

}



app.Run();
