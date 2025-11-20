using BibliotecaApi.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutoresController : ControllerBase
{
    private readonly AppDataContext _db;
    private readonly ILogger<AutoresController> _logger;

    public AutoresController(AppDataContext db, ILogger<AutoresController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Autor>>> GetAutores()
    {
        _logger.LogInformation("GET /api/autores - Buscando todos os autores");
        var autores = await _db.Autores
            .Include(a => a.Livros)
            .ToListAsync();
        
        _logger.LogInformation($"Encontrados {autores.Count} autores");
        return Ok(autores);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Autor>> GetAutor(int id)
    {
        var autor = await _db.Autores
            .Include(a => a.Livros)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (autor is null)
            return NotFound();

        return Ok(autor);
    }

    [HttpPost]
    public async Task<ActionResult<Autor>> CreateAutor([FromBody] Autor novoAutor)
    {
        _db.Autores.Add(novoAutor);
        await _db.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetAutor), new { id = novoAutor.Id }, novoAutor);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Autor>> UpdateAutor(int id, [FromBody] Autor autorAtualizado)
    {
        var autor = await _db.Autores.FindAsync(id);
        if (autor is null)
            return NotFound();

        autor.Nome = autorAtualizado.Nome;
        await _db.SaveChangesAsync();

        return Ok(autor);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Autor>> DeleteAutor(int id)
    {
        var autor = await _db.Autores.FindAsync(id);
        if (autor is null)
            return NotFound();

        _db.Autores.Remove(autor);
        await _db.SaveChangesAsync();

        return Ok(autor);
    }
}
