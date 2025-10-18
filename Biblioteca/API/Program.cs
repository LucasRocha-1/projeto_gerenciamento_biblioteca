using BibliotecaApi.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDataContext>();

var app = builder.Build();
// --- Dados em Memória (Exemplo) ---
var autores = new List<Autor>{ };

var usuarios = new List<Usuario>{ };

var livros = new List<Livro>{ };

app.MapGet("/", () => "API da Biblioteca").WithTags("Info");

#region Endpoints de Usuários (Users)

// GET: Listar todos os usuários
app.MapGet("/api/usuarios", async (AppDataContext db) =>
{
    return Results.Ok(await db.Usuarios.ToListAsync());
})
.WithTags("Usuarios");

// GET: Buscar usuário por ID
app.MapGet("/api/usuarios/{id:int}", async (int id, AppDataContext db) =>
{
    var usuario = await db.Usuarios.FindAsync(id);
    return usuario is not null ? Results.Ok(usuario) : Results.NotFound();
})
.WithName("GetUsuarioPorId")
.WithTags("Usuarios");

// POST: Criar um novo usuário
app.MapPost("/api/usuarios", async ([FromBody] Usuario novoUsuario, AppDataContext db) =>
{
    db.Usuarios.Add(novoUsuario);
    await db.SaveChangesAsync();
    return Results.CreatedAtRoute("GetUsuarioPorId", new { id = novoUsuario.Id }, novoUsuario);
}).WithTags("Usuarios");

// PUT: Atualizar um usuário existente
app.MapPut("/api/usuarios/{id:int}", async (int id, [FromBody] Usuario usuarioAtualizado, AppDataContext db) =>
{
    var usuario = await db.Usuarios.FindAsync(id);
    if (usuario is null) return Results.NotFound();

    usuario.Nome = usuarioAtualizado.Nome;
    usuario.Email = usuarioAtualizado.Email;

    await db.SaveChangesAsync();
    return Results.Ok(usuario);
}).WithTags("Usuarios");

// DELETE: Remover um usuário
app.MapDelete("/api/usuarios/{id:int}", async (int id, AppDataContext db) =>
{
    var usuario = await db.Usuarios.FindAsync(id);
    if (usuario is null) return Results.NotFound();

    db.Usuarios.Remove(usuario);
    await db.SaveChangesAsync();

    return Results.Ok(usuario);
}).WithTags("Usuarios");

#endregion

#region Endpoints de Livros (Books)

// Retorna todos os livros, sem paginação
app.MapGet("/api/livros", (AppDataContext db) =>
{
    foreach (var livro in livros)
    {
        livro.Autor = autores.FirstOrDefault(a => a.Id == livro.AutorId);
    }
    return Results.Ok(livros);
}).WithTags("Livros");

app.MapGet("/api/livros/{id:int}", (int id, AppDataContext db) =>
{
    var livro = livros.FirstOrDefault(b => b.Id == id);
    if (livro is null) return Results.NotFound();

    livro.Autor = autores.FirstOrDefault(a => a.Id == livro.AutorId);
    livro.EmprestadoParaUsuario = usuarios.FirstOrDefault(u => u.Id == livro.EmprestadoParaUsuarioId);

    return Results.Ok(livro);
})
.WithName("GetLivroPorId")
.WithTags("Livros");

app.MapPost("/api/livros", ([FromBody] Livro novoLivro, AppDataContext db) =>
{
    var autorExiste = autores.Any(a => a.Id == novoLivro.AutorId);
    if (!autorExiste) return Results.BadRequest("Autor não encontrado.");

    novoLivro.Id = livros.Any() ? livros.Max(b => b.Id) + 1 : 1;
    livros.Add(novoLivro);
    return Results.CreatedAtRoute("GetLivroPorId", new { id = novoLivro.Id }, novoLivro);
}).WithTags("Livros");

app.MapPost("/api/livros", async (AppDataContext db, [FromBody] Livro novoLivro) =>
{
    // A verificação acontece AQUI, usando o banco de dados (db.Autores)
    var autorExiste = await db.Autores.AnyAsync(a => a.Id == novoLivro.AutorId);
    
    if (!autorExiste)
    {

        return Results.BadRequest("Autor não encontrado.");
    }

    db.Livros.Add(novoLivro);
    await db.SaveChangesAsync();
    return Results.CreatedAtRoute("GetLivroPorId", new { id = novoLivro.Id }, novoLivro);
});

app.MapDelete("/api/livros/{id:int}", (int id, AppDataContext db) =>
{
    var livro = livros.FirstOrDefault(b => b.Id == id);
    if (livro is null) return Results.NotFound();

    livros.Remove(livro);
    return Results.Ok(livro);
}).WithTags("Livros");

#endregion

#region Endpoints de Empréstimos (Loans)

app.MapPost("/api/livros/{livroId:int}/emprestar/{usuarioId:int}", (int livroId, int usuarioId, AppDataContext db) =>
{
    var livro = livros.FirstOrDefault(l => l.Id == livroId);
    if (livro is null)
    {
        return Results.NotFound("Livro não encontrado.");
    }

    var usuario = usuarios.FirstOrDefault(u => u.Id == usuarioId);
    if (usuario is null)
    {
        return Results.NotFound("Usuário não encontrado.");
    }

    if (livro.EmprestadoParaUsuarioId is not null)
    {
        return Results.Conflict($"Este livro já está emprestado para o usuário com ID {livro.EmprestadoParaUsuarioId}.");
    }

    livro.EmprestadoParaUsuarioId = usuarioId;
    livro.DataEmprestimo = DateTime.UtcNow;

    return Results.Ok(livro);
})
.WithTags("Emprestimos");

#endregion

#region Endpoints de Autores (Authors)


// POST: Cadastrar um novo autor
app.MapPost("/api/autores", async (AppDataContext db, [FromBody] Autor novoAutor) =>
{
    db.Autores.Add(novoAutor);
    await db.SaveChangesAsync();
    // Retorna o autor criado com o ID gerado pelo banco
    return Results.Created($"/api/autores/{novoAutor.Id}", novoAutor);
}).WithTags("Autores");

// GET: Listar todos os autores
app.MapGet("/api/autores/listar", async (AppDataContext db) =>
{
    var autores = await db.Autores
        .Include(a => a.Livros) // Inclui a lista de livros de cada autor
        .ToListAsync();
    return Results.Ok(autores);
}).WithTags("Autores");



// PUT: Atualizar um autor existente
app.MapPut("/api/autores/{id:int}", async (AppDataContext db, int id, [FromBody] Autor autorAtualizado) =>
{
    var autor = await db.Autores.FindAsync(id);
    if (autor is null) return Results.NotFound();

    autor.Nome = autorAtualizado.Nome;
    await db.SaveChangesAsync();

    return Results.Ok(autor);
}).WithTags("Autores");


// DELETE: Deletar um autor
app.MapDelete("/api/autores/{id:int}", async (AppDataContext db, int id) =>
{
    var autor = await db.Autores.FindAsync(id);
    if (autor is null) return Results.NotFound();

    db.Autores.Remove(autor);
    await db.SaveChangesAsync();

    return Results.Ok(autor);
}).WithTags("Autores");

#endregion
app.Run();
