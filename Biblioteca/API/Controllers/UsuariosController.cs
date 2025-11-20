using BibliotecaApi.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDataContext _db;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(AppDataContext db, ILogger<UsuariosController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Usuario>>> GetUsuarios()
    {
        _logger.LogInformation("GET /api/usuarios - Buscando todos os usuários");
        var usuarios = await _db.Usuarios.ToListAsync();
        
        _logger.LogInformation($"Encontrados {usuarios.Count} usuários");
        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> GetUsuario(int id)
    {
        var usuario = await _db.Usuarios.FindAsync(id);

        if (usuario is null)
            return NotFound();

        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<Usuario>> CreateUsuario([FromBody] Usuario novoUsuario)
    {
        _db.Usuarios.Add(novoUsuario);
        await _db.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetUsuario), new { id = novoUsuario.Id }, novoUsuario);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Usuario>> UpdateUsuario(int id, [FromBody] Usuario usuarioAtualizado)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario is null)
            return NotFound();

        usuario.Nome = usuarioAtualizado.Nome;
        usuario.Email = usuarioAtualizado.Email;

        await _db.SaveChangesAsync();
        return Ok(usuario);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Usuario>> DeleteUsuario(int id)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario is null)
            return NotFound();

        _db.Usuarios.Remove(usuario);
        await _db.SaveChangesAsync();

        return Ok(usuario);
    }
}
