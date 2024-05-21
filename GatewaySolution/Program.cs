using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;
using GatewaySolution.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddAuthentication();

if (builder.Environment.EnvironmentName.ToString().ToLower() == "production")
{
    builder.Configuration.AddJsonFile("Ocelot.Production.json", optional: false, reloadOnChange: true);
}
else
{
    builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);
}
builder.Services.AddOcelot(builder.Configuration);


var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseOcelot().GetAwaiter().GetResult();
app.Run();
