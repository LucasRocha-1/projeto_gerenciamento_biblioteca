using BibliotecaApi.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LivrosController : ControllerBase
{
    private readonly AppDataContext _db;
    private readonly ILogger<LivrosController> _logger;

    public LivrosController(AppDataContext db, ILogger<LivrosController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Livro>>> GetLivros()
    {
        _logger.LogInformation("GET /api/livros - Buscando todos os livros");
        var livros = await _db.Livros
            .Include(l => l.Autor)
            .ToListAsync();
        
        _logger.LogInformation($"Encontrados {livros.Count} livros");
        return Ok(livros);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Livro>> GetLivro(int id)
    {
        var livro = await _db.Livros
            .Include(l => l.Autor)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (livro is null)
            return NotFound();

        return Ok(livro);
    }

    [HttpPost]
    public async Task<ActionResult<Livro>> CreateLivro([FromBody] Livro novoLivro)
    {
        var autorExiste = await _db.Autores.AnyAsync(a => a.Id == novoLivro.AutorId);
        if (!autorExiste)
            return BadRequest("Autor não encontrado.");

        _db.Livros.Add(novoLivro);
        await _db.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetLivro), new { id = novoLivro.Id }, novoLivro);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Livro>> DeleteLivro(int id)
    {
        var livro = await _db.Livros.FindAsync(id);
        if (livro is null)
            return NotFound();

        _db.Livros.Remove(livro);
        await _db.SaveChangesAsync();
        
        return Ok(livro);
    }
}
