using Api.Mapping.Profiles;
using Core.Interfaces;
using Core.Options;
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
				cfg.AddProfile<SubscriptionDtoProfile>();
				cfg.AddProfile<EventProfile>();
				cfg.AddProfile<EventDtoProfile>();
			});

			return services;
		}

		private static IServiceCollection AddDbServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseNpgsql(configuration.GetConnectionString("PostgreSql"));

				options.EnableSensitiveDataLogging();
				options.LogTo(Console.WriteLine);
			});

			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IParticipantRepository, ParticipantRepository>();
			services.AddScoped<IEventRepository, EventRepository>();

			return services;
		}

		private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			// Register JWT options
			services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
			var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();

			// Set JWT auth
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
			});

			return services;
		}
	}
}
