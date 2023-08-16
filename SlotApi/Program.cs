using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SlotApi.Database;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Extensions.Options;

var corsPolicyName = "corsPolicy";

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Slots");
builder.Services.AddDbContext<SlotsContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
      };
      //options.IncludeErrorDetails = true;
    });
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.AddServer(new OpenApiServer() { Description = "Local Instance", Url = "https://localhost:7109/" });
  c.AddServer(new OpenApiServer() { Description = "Azure Instance", Url = "https://slots-api.azurewebsites.net/" });

  c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
  {
    Description = @"JWT Authorization header using the Bearer scheme.",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer"
  });

  c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });
});
builder.Services.AddCors(options =>
{
  options.AddPolicy(name: corsPolicyName,
                    policy =>
                    {
                      policy.WithOrigins(builder.Configuration["AccessControlAllowOrigin"])
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                    });
});

var app = builder.Build();

// Configure the HTTP request pipeline. Order Matters!
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
