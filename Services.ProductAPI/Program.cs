using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Services.ProductAPI;
using Services.ProductAPI.Data;
using Services.ProductAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ApplicationDBContext, ApplicationDBContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Description = "Enter the bearer authorization string as following : `Bearer Generated-JWT-Token`",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});
	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference=new OpenApiReference
				{
					Type= ReferenceType.SecurityScheme,
					Id = JwtBearerDefaults.AuthenticationScheme
				}
			},new string[]{}
		}
	});
});
builder.AddAuthentication();
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
applyMigration();
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();

void applyMigration()
{
	using (var scope = app.Services.CreateScope())
	{
		var _db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
		if (_db.Database.GetAppliedMigrations().Count() > 0)
		{
			_db.Database.Migrate();
		}

	}
}
