
using CameraAnalyzer.bl.APIs;
using CameraAnalyzer.bl.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// הוספת GeminiAPI ו־GeminiService
builder.Services.AddSingleton<GeminiAPI>();
builder.Services.AddScoped<IGeminiService, GeminiService>();
// builder.Services.AddSingleton<GoogleVisionAPI>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
