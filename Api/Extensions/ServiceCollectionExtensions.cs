using Api.Mapping.Profiles;
using Core.Interfaces;
using Core.Options;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Interfaces;
using Infrastructure.Mapping.Profiles;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Entity;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddOrganizaServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddHttpServices();
			services.AddApplicationServices(configuration);
			services.AddDbServices(configuration);
			services.AddJwtAuthentication(configuration);
			services.AddValidationServices();
			return services;
		}

		private static IServiceCollection AddHttpServices(this IServiceCollection services)
		{
			services.AddHttpClient();
			return services;
		}

		private static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IParticipantService, ParticipantService>();
			services.AddScoped<IJwtService, JwtService>();
			services.AddScoped<PasswordHasher<UserEntity>>();
			services.AddScoped<IEventService, EventService>();

			services.AddAutoMapper(cfg =>
			{
				cfg.AddProfile<UserProfile>();
				cfg.AddProfile<UserDtoProfile>();
				cfg.AddProfile<ParticipantProfile>();
				cfg.AddProfile<ParticipantDtoProfile>();
				cfg.AddProfile<EventProfile>();
				cfg.AddProfile<EventDtoProfile>();
			});

			return services;
		}

		private static IServiceCollection AddDbServices(this IServiceCollection services, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("PostgreSql");

			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseNpgsql(connectionString);

				if (configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Development")
				{
					options.EnableSensitiveDataLogging();
					options.LogTo(Console.WriteLine);
				}
			});

			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IParticipantRepository, ParticipantRepository>();
			services.AddScoped<IEventRepository, EventRepository>();

			return services;
		}

		private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

			var jwtSection = configuration.GetSection("Jwt");
			var jwtOptions = jwtSection.Get<JwtOptions>();

			if (jwtOptions == null || string.IsNullOrEmpty(jwtOptions.SecretKey))
			{
				throw new InvalidOperationException(
					"JWT configuration is missing or invalid. Please check appsettings.json");
			}

			if (jwtOptions.SecretKey.Length < 32)
			{
				throw new InvalidOperationException(
					"JWT SecretKey must be at least 32 characters long for security reasons.");
			}

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
					ValidateIssuer = true,
					ValidIssuer = jwtOptions.Issuer,
					ValidateAudience = true,
					ValidAudience = jwtOptions.Audience,
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero,
				};

				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						var accessToken = context.Request.Query["access_token"];
						var path = context.HttpContext.Request.Path;

						if (!string.IsNullOrEmpty(accessToken))
						{
							context.Token = accessToken;
						}

						return Task.CompletedTask;
					}
				};
			});

			return services;
		}

		private static IServiceCollection AddValidationServices(this IServiceCollection services)
		{
			services.AddFluentValidationAutoValidation();
			services.AddValidatorsFromAssemblyContaining<Program>();
			return services;
		}
	}
}