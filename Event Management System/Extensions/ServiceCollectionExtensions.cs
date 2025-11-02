using Api.Mapping.Profiles;
using Core.Interfaces;
using Core.Options;
using Infrastructure.Interfaces;
using Infrastructure.Mapping.Profiles;
using Infrastructure.Persistence.Entity;
using Infrastructure.Persistence.MongoDb;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

namespace Api.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddOrganizaServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddHttpServices();
			services.AddApplicationServices(configuration);
			services.AddMongoDbServices(configuration);
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
			services.AddScoped<ISubscriptionService, SubscriptionService>();
			services.AddScoped<IJwtService, JwtService>();
			services.AddScoped<PasswordHasher<UserEntity>>();
			services.AddScoped<IEventService, EventService>();
			services.AddAutoMapper(cfg =>
			{
				cfg.AddProfile<UserProfile>();
				cfg.AddProfile<UserDtoProfile>();
				cfg.AddProfile<SubscriptionProfile>();
				cfg.AddProfile<SubscriptionDtoProfile>();
				cfg.AddProfile<EventProfile>();
				cfg.AddProfile<EventDtoProfile>();
			});

			return services;
		}

		private static IServiceCollection AddMongoDbServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<MongoDbOptions>(configuration.GetSection(nameof(MongoDbOptions)));

			services.AddScoped<IMongoClient>(sp =>
			{
				string connectionString = configuration.GetConnectionString("MongoDb")!;
				var settings = MongoClientSettings.FromConnectionString(connectionString);
				settings.ServerApi = new ServerApi(ServerApiVersion.V1);
				var client = new MongoClient(settings);

				return client;
			});

			services.AddScoped<IMongoDbContext, MongoDbContext>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
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
