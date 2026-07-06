using LightNovelTranslator.Core;
using LightNovelTranslator.Core.Interfaces;
using LightNovelTranslator.Docx;
using LightNovelTranslator.Ollama;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddScoped<ITranslator, OllamaTranslator>();
builder.Services.AddScoped<ITranslationProgressStore, TranslationProgressStore>();
builder.Services.AddScoped<DocxDocumentReader>();
builder.Services.AddScoped<DocxDocumentWriter>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();



app.MapControllers();

app.Run();
