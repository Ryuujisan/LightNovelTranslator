using LightNovelTranslator.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LightNovelTranslator.Api.Controllers;

[Route("api/[controller]")]
public class OllamaController : BaseController
{
    private readonly OllamaService _ollamaService;

    public OllamaController(OllamaService ollamaService)
    {
        _ollamaService = ollamaService;
    }

    [HttpGet("status")]
    public async Task<IActionResult> Get()
    {
        var available = await _ollamaService.IsAvailableAsync();

        return Ok(new
        {
            Installed = available
        });
    }
    
    [HttpGet("models")]
    public async Task<IActionResult> Models()
    {
        var models = await _ollamaService.ModelListAsync();

        return Ok(models);
    }
}