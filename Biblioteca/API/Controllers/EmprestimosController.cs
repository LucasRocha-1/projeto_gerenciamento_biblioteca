using Microsoft.AspNetCore.Mvc;
using BibliotecaApi.Modelos;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmprestimosController : ControllerBase
{
    private readonly AppDataContext _context;

    public EmprestimosController(AppDataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Emprestimo>>> GetEmprestimos()
    {
        var emprestimos = await _context.Emprestimos.OrderByDescending(e => e.DataEmprestimo).ToListAsync();
        return Ok(emprestimos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Emprestimo>> GetEmprestimo(int id)
    {
        var emprestimo = await _context.Emprestimos.FindAsync(id);
        if (emprestimo == null)
            return NotFound();
        return Ok(emprestimo);
    }

    [HttpPost]
    public async Task<ActionResult<Emprestimo>> CreateEmprestimo([FromBody] Emprestimo emprestimo)
    {
        emprestimo.DataEmprestimo = DateTime.UtcNow;
        _context.Emprestimos.Add(emprestimo);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEmprestimo), new { id = emprestimo.Id }, emprestimo);
    }

    [HttpPost("registrar/{livroId}/{usuarioId}")]
    public async Task<ActionResult<Emprestimo>> RegistrarEmprestimo(int livroId, int usuarioId)
    {
        var livro = await _context.Livros.FindAsync(livroId);
        if (livro == null)
            return NotFound("Livro não encontrado.");

        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
            return NotFound("Usuário não encontrado.");

        var emprestimo = new Emprestimo
        {
            LivroId = livroId,
            LivroTitulo = livro.Titulo,
            UsuarioId = usuarioId,
            UsuarioNome = usuario.Nome,
            DataEmprestimo = DateTime.UtcNow,
            Status = "ativo"
        };

        _context.Emprestimos.Add(emprestimo);
        // Atualiza status do livro como emprestado
        livro.EmprestadoParaUsuarioId = usuarioId;
        livro.DataEmprestimo = DateTime.UtcNow;
        _context.Livros.Update(livro);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEmprestimo), new { id = emprestimo.Id }, emprestimo);
    }

    [HttpPut("{id}/devolver")]
    public async Task<IActionResult> DevolverEmprestimo(int id)
    {
        var emprestimo = await _context.Emprestimos.FindAsync(id);
        if (emprestimo == null)
            return NotFound();

        emprestimo.DataDevolucao = DateTime.UtcNow;
        emprestimo.Status = "devolvido";

        // Atualiza também o livro relacionado: marca como disponível
        var livro = await _context.Livros.FindAsync(emprestimo.LivroId);
        if (livro != null)
        {
            livro.EmprestadoParaUsuarioId = null;
            livro.DataEmprestimo = null;
            _context.Livros.Update(livro);
        }

        _context.Emprestimos.Update(emprestimo);
        await _context.SaveChangesAsync();

        return Ok(emprestimo);
    }

    [HttpPost("devolver-por-livro/{livroId}")]
    public async Task<IActionResult> DevolverPorLivro(int livroId)
    {
        // Encontra o empréstimo ativo mais recente para esse livro
        var emprestimo = await _context.Emprestimos
            .Where(e => e.LivroId == livroId && e.Status == "ativo")
            .OrderByDescending(e => e.DataEmprestimo)
            .FirstOrDefaultAsync();

        if (emprestimo == null)
            return NotFound("Empréstimo ativo não encontrado para esse livro.");

        // Reaproveita a lógica de devolver
        emprestimo.DataDevolucao = DateTime.UtcNow;
        emprestimo.Status = "devolvido";

        var livro = await _context.Livros.FindAsync(emprestimo.LivroId);
        if (livro != null)
        {
            livro.EmprestadoParaUsuarioId = null;
            livro.DataEmprestimo = null;
            _context.Livros.Update(livro);
        }

        _context.Emprestimos.Update(emprestimo);
        await _context.SaveChangesAsync();

        return Ok(emprestimo);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmprestimo(int id)
    {
        var emprestimo = await _context.Emprestimos.FindAsync(id);
        if (emprestimo == null)
            return NotFound();

        _context.Emprestimos.Remove(emprestimo);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
