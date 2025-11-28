using AvailableActions.Services;
using AvailableActions.Services.Abstraction;

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
builder.Services.AddSingleton<IRulesService, RulesService>();

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
