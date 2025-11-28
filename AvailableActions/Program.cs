using AvailableActions.Business.Services;
using AvailableActions.Business.Services.Abstraction;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var rulesFilePath = builder.Configuration["Rules:Path"] ?? Path.Combine(AppContext.BaseDirectory, "rules.json");

// Services are registered as singletons since they hold in-memory data
builder.Services.AddSingleton<ICardService, CardService>();
builder.Services.AddSingleton<IRulesService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var env = sp.GetRequiredService<IHostEnvironment>(); // ContentRootPath
    var logger = sp.GetRequiredService<ILogger<RulesService>>();

    var configured = config["Rules:Path"]; // z appsettings.json, np "Config/rules.json"
    string rulesPath;

    if (!string.IsNullOrWhiteSpace(configured))
    {
        // jeœli u¿ytkownik poda³ œcie¿kê bezwzglêdn¹ - u¿yj jej, jeœli wzglêdn¹ - traktuj wzglêdem ContentRootPath
        rulesPath = Path.IsPathRooted(configured)
            ? configured
            : Path.Combine(env.ContentRootPath, configured.Replace('/', Path.DirectorySeparatorChar));
    }
    else
    {
        // domyœlnie spodziewamy siê Config/rules.json w katalogu root projektu (ContentRootPath)
        rulesPath = Path.Combine(env.ContentRootPath, "Config", "rules.json");
    }

    logger.LogInformation("Using rules file path: {Path}", rulesPath);
    return new RulesService(rulesPath, logger);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
