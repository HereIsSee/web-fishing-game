using Api.Hubs;
using Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
// Add SignalR 
builder.Services.AddSignalR();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors();

app.UseRouting();

app.MapHub<GameHub>("/gamehub");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// app.UseAuthorization();
// Auto-run flyweight test on startup
Console.WriteLine("🧪 Auto-running Flyweight Test...");
Api.Models.Flyweight.FishFlyweightFactory.TestPerformance();

app.MapControllers();



app.Run();
