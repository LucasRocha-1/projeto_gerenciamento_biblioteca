using Microsoft.AspNetCore.Mvc;
using BibliotecaApi.Modelos;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDataContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AppDataContext context, ILogger<AuthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private string HashSenha(string senha)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    [HttpPost("cadastro")]
    public async Task<ActionResult<object>> Cadastro([FromBody] CadastroDto dto)
    {
        _logger.LogInformation("[CADASTRO] Requisição recebida");
        _logger.LogInformation($"[CADASTRO] Dados recebidos: Nome={dto.Nome}, Email={dto.Email}, SenhaLen={dto.Senha?.Length ?? 0}");

        if (string.IsNullOrEmpty(dto.Nome) || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Senha))
        {
            _logger.LogWarning("[CADASTRO] Validação falhou: campos obrigatórios vazios");
            return BadRequest("Nome, email e senha são obrigatórios.");
        }

        var emailExists = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
        if (emailExists)
        {
            _logger.LogWarning($"[CADASTRO] Email já cadastrado: {dto.Email}");
            return BadRequest("Email já cadastrado.");
        }

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = HashSenha(dto.Senha)
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"[CADASTRO] Usuário cadastrado com sucesso! Id={usuario.Id}, Nome={usuario.Nome}");
        return Ok(new { message = "Usuário cadastrado com sucesso!", id = usuario.Id, nome = usuario.Nome });
    }

    [HttpPost("login")]
    public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
    {
        _logger.LogInformation("[LOGIN] Requisição recebida");
        _logger.LogInformation($"[LOGIN] Dados recebidos: Email={dto.Email}, SenhaLen={dto.Senha?.Length ?? 0}");

        if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Senha))
        {
            _logger.LogWarning("[LOGIN] Validação falhou: email ou senha vazios");
            return BadRequest("Email e senha são obrigatórios.");
        }

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (usuario == null)
        {
            _logger.LogWarning($"[LOGIN] Usuário não encontrado: {dto.Email}");
            return Unauthorized("Email ou senha incorretos.");
        }

        var senhaHash = HashSenha(dto.Senha);
        if (usuario.Senha != senhaHash)
        {
            _logger.LogWarning($"[LOGIN] Senha incorreta para: {dto.Email}");
            return Unauthorized("Email ou senha incorretos.");
        }

        _logger.LogInformation($"[LOGIN] Login realizado com sucesso! Id={usuario.Id}, Nome={usuario.Nome}");
        return Ok(new { message = "Login realizado com sucesso!", id = usuario.Id, nome = usuario.Nome, email = usuario.Email, isAdmin = usuario.IsAdmin });
    }

    [HttpPost("tornar-admin/{usuarioId:int}")]
    public async Task<ActionResult> TornarAdmin(int usuarioId)
    {
        _logger.LogInformation($"🔐 [ADMIN] Tentativa de tornar usuário {usuarioId} admin");

        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
        {
            _logger.LogWarning($"🔐 [ADMIN] Usuário não encontrado: {usuarioId}");
            return NotFound("Usuário não encontrado.");
        }

        usuario.IsAdmin = true;
        await _context.SaveChangesAsync();

        _logger.LogInformation($"[ADMIN] Usuário {usuarioId} ({usuario.Nome}) agora é admin!");
        return Ok(new { message = "Usuário agora é administrador!", id = usuario.Id, nome = usuario.Nome, isAdmin = usuario.IsAdmin });
    }
}

public class CadastroDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
