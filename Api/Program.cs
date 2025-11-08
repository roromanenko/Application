using Api.Extensions;
using Api.Middlewares;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Environment.IsDevelopment()
	? new[] { "http://localhost:50214", "http://localhost:4200" }
	: new[]
	{
		builder.Configuration["FrontendUrl"] ?? "https://organiza-frontend.onrender.com",
		"https://organiza-frontend.onrender.com"
    };

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend",
		policy =>
		{
			policy
				.WithOrigins(allowedOrigins)
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowCredentials();
		});
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Organiza API",
		Version = "v1",
		Description = "Event Management System API"
	});

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});

	c.MapType<TimeSpan>(() => new OpenApiSchema
	{
		Type = "string",
		Format = "time-span",
		Example = new OpenApiString("01:30:00")
	});
});

builder.Services.AddOrganizaServices(builder.Configuration);
builder.Services.AddOrganizaHealthChecks(builder.Configuration);

var app = builder.Build();

await app.UseDatabaseMigration();

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapOrganizaHealthChecks();

app.Run();