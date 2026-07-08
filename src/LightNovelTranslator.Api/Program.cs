using LightNovelTranslator.Api.Hubs;
using LightNovelTranslator.Api.Services;
using LightNovelTranslator.Core;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Docx;
using LightNovelTranslator.Ollama;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024L * 1024L * 1024L; // 1 GB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});
builder.Services.AddScoped<ITranslator, OllamaTranslator>();
builder.Services.AddScoped<ITranslationProgressStore, TranslationProgressStore>();
builder.Services.AddScoped<IDocumentReader, DocxDocumentReader>();
builder.Services.AddScoped<IDocumentWriter, DocxDocumentWriter>();
builder.Services.AddSingleton<ITranslationQueue, TranslationQueue>();
builder.Services.AddHostedService<TranslationWorker>();
builder.Services.AddScoped<TranslationJobProcessor>();
builder.Services.AddScoped<ITranslationProgressReporter, SignalRProgressReporter>();
builder.Services.AddHttpClient<OllamaService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434");
});
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173",
                "https://ashy-bush-04ce5a910.7.azurestaticapps.net")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddSignalR();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("Frontend");

app.UseAuthorization();

app.MapControllers();
app.MapHub<TranslateHub>("/hubs/translation");

app.Run();
