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

// GET: listar todos os livros
app.MapGet("/api/livros", async (AppDataContext db) =>
{
    var livros = await db.Livros
        .Include(l => l.Autor)
        .Include(l => l.EmprestadoParaUsuario)
        .ToListAsync();
    return Results.Ok(livros);
}).WithTags("Livros");

// GET: buscar livro por ID
app.MapGet("/api/livros/{id:int}", async (int id, AppDataContext db) =>
{
    var livro = await db.Livros
        .Include(l => l.Autor)
        .Include(l => l.EmprestadoParaUsuario)
        .FirstOrDefaultAsync(l => l.Id == id);

    return livro is not null ? Results.Ok(livro) : Results.NotFound();
}).WithName("GetLivroPorId").WithTags("Livros");

// POST: adicionar livro
app.MapPost("/api/livros", async (AppDataContext db, [FromBody] Livro novoLivro) =>
{
    var autorExiste = await db.Autores.AnyAsync(a => a.Id == novoLivro.AutorId);
    if (!autorExiste)
        return Results.BadRequest("Autor não encontrado.");

    db.Livros.Add(novoLivro);
    await db.SaveChangesAsync();
    return Results.CreatedAtRoute("GetLivroPorId", new { id = novoLivro.Id }, novoLivro);
}).WithTags("Livros");

// DELETE: remover livro
app.MapDelete("/api/livros/{id:int}", async (int id, AppDataContext db) =>
{
    var livro = await db.Livros.FindAsync(id);
    if (livro is null)
        return Results.NotFound();

    db.Livros.Remove(livro);
    await db.SaveChangesAsync();
    return Results.Ok(livro);
}).WithTags("Livros");

#endregion

#region Endpoints de Empréstimos (Loans)

// POST: Emprestar um livro para um usuário
app.MapPost("/api/livros/{livroId:int}/emprestar/{usuarioId:int}", async (int livroId, int usuarioId, AppDataContext db) =>
{
    var livro = await db.Livros.FindAsync(livroId);
    if (livro is null)
        return Results.NotFound("Livro não encontrado.");

    var usuario = await db.Usuarios.FindAsync(usuarioId);
    if (usuario is null)
        return Results.NotFound("Usuário não encontrado.");

    if (livro.EmprestadoParaUsuarioId is not null)
        return Results.Conflict($"Este livro já está emprestado para o usuário com ID {livro.EmprestadoParaUsuarioId}.");

    livro.EmprestadoParaUsuarioId = usuarioId;
    livro.DataEmprestimo = DateTime.UtcNow;

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        mensagem = $"Livro '{livro.Titulo}' emprestado para {usuario.Nome}.",
        livro
    });
}).WithTags("Emprestimos");

// POST: Devolver um livro
app.MapPost("/api/livros/{livroId:int}/devolver", async (int livroId, AppDataContext db) =>
{
    var livro = await db.Livros.FindAsync(livroId);
    if (livro is null)
        return Results.NotFound("Livro não encontrado.");

    if (livro.EmprestadoParaUsuarioId is null)
        return Results.BadRequest("Este livro não está emprestado.");

    livro.EmprestadoParaUsuarioId = null;
    livro.DataEmprestimo = null;

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        mensagem = $"Livro '{livro.Titulo}' foi devolvido com sucesso.",
        livro
    });
}).WithTags("Emprestimos");

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
