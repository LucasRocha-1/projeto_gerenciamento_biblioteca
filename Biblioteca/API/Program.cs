using BibliotecaApi.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using API.Models;

var builder = WebApplication.CreateBuilder(args);
// Adiciona o DbContext com SQLite 
builder.Services.AddDbContext<AppDataContext>();
var app = builder.Build();



// --- Dados em Memória ---
var autores = new List<Autor>
{
    new() { Id = 1, Nome = "Machado de Assis" },
    new() { Id = 2, Nome = "J.K. Rowling" }
};

var usuarios = new List<Usuario>
{
    new() { Id = 1, Nome = "Alice", Email = "alice@email.com" },
    new() { Id = 2, Nome = "Bob", Email = "bob@email.com" }
};

var livros = new List<Livro>
{
    new() { Id = 1, Titulo = "Dom Casmurro", AutorId = 1, AnoPublicacao = 1899 },
    new() { Id = 2, Titulo = "Memórias Póstumas de Brás Cubas", AutorId = 1, AnoPublicacao = 1881 },
    new() { Id = 3, Titulo = "Harry Potter e a Pedra Filosofal", AutorId = 2, AnoPublicacao = 1997 }
};
// -------------------------

app.MapGet("/", () => "API da Biblioteca").WithTags("Info");

#region Endpoints de Usuários (usuarios)

// app.MapGet("/api/usuarios", () => Results.Ok(usuarios)).WithTags("Usuarios");
app.MapGet("/api/usuarios", async (AppDataContext db) =>
{
    return Results.Ok(await db.Usuarios.ToListAsync());
}).WithTags("Usuarios");

// app.MapGet("/api/usuarios/{id:int}", (int id) =>
// {
//     var usuario = usuarios.FirstOrDefault(u => u.Id == id);
//     return usuario is not null ? Results.Ok(usuario) : Results.NotFound();
// })
// .WithName("GetUsuarioPorId") // Damos um nome à rota para referência
// .WithTags("Usuarios");
app.MapGet("/api/usuarios/{id:int}", async (AppDataContext db, int id) =>
{
    var usuario = await db.Usuarios.FindAsync(id);
    return usuario is not null ? Results.Ok(usuario) : Results.NotFound();
})
.WithName("GetUsuarioPorId")
.WithTags("Usuarios");

// app.MapPost("/api/usuarios", ([FromBody] Usuario novoUsuario) =>
// {
//     // Simula a criação de um novo ID
//     novoUsuario.Id = usuarios.Any() ? usuarios.Max(u => u.Id) + 1 : 1;
//     usuarios.Add(novoUsuario);
//     return Results.CreatedAtRoute("GetUsuarioPorId", new { id = novoUsuario.Id }, novoUsuario);
// }).WithTags("Usuarios");
app.MapPost("/api/usuarios", async (AppDataContext db, [FromBody] Usuario novoUsuario) =>
{
    db.Usuarios.Add(novoUsuario);
    await db.SaveChangesAsync();
    return Results.CreatedAtRoute("GetUsuarioPorId", new { id = novoUsuario.Id }, novoUsuario);
}).WithTags("Usuarios");

// app.MapPut("/api/usuarios/{id:int}", (int id, [FromBody] Usuario usuarioAtualizado) =>
// {
//     var usuario = usuarios.FirstOrDefault(u => u.Id == id);
//     if (usuario is null) return Results.NotFound();

//     usuario.Nome = usuarioAtualizado.Nome;
//     usuario.Email = usuarioAtualizado.Email;

//     return Results.Ok(usuario);
// }).WithTags("Usuarios");
app.MapPut("/api/usuarios/{id:int}", async (AppDataContext db, int id, [FromBody] Usuario usuarioAtualizado) =>
{
    var usuario = await db.Usuarios.FindAsync(id);
    if (usuario is null) return Results.NotFound();

    usuario.Nome = usuarioAtualizado.Nome;
    usuario.Email = usuarioAtualizado.Email;

    await db.SaveChangesAsync();
    return Results.Ok(usuario);
}).WithTags("Usuarios");

// app.MapDelete("/api/usuarios/{id:int}", (int id) =>
// {
//     var usuario = usuarios.FirstOrDefault(u => u.Id == id);
//     if (usuario is null) return Results.NotFound();

//     usuarios.Remove(usuario);
//     return Results.Ok(usuario);
// }).WithTags("Usuarios");
app.MapDelete("/api/usuarios/{id:int}", async (AppDataContext db, int id) =>
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
// app.MapGet("/api/livros", () =>
// {
//     // Para cada livro, encontra o autor correspondente na lista de autores
//     foreach (var livro in livros)
//     {
//         livro.Autor = autores.FirstOrDefault(a => a.Id == livro.AutorId);
//     }
//     return Results.Ok(livros);
// }).WithTags("Livros");
app.MapGet("/api/livros", async (AppDataContext db) =>
{
    var livros = await db.Livros
        .Include(l => l.Autor)
        .Include(l => l.EmprestadoParaUsuario)
        .ToListAsync();
    return Results.Ok(livros);
}).WithTags("Livros");

// app.MapGet("/api/autores", () => Results.Ok(autores)).WithTags("Autores");
// app.MapGet("/api/autores", async (AppDataContext db) =>
// {
//     var autores = await db.Autores
//         .Include(a => a.Livros)
//         .ToListAsync();
//     return Results.Ok(autores);
// }).WithTags("Autores");
app.MapGet("/api/autores", async (AppDataContext db) =>
{
    var autores = await db.Autores
        .Include(a => a.Livros)
        .ToListAsync();
    return Results.Ok(autores);
}).WithTags("Autores");

// app.MapGet("/api/livros/{id:int}", (int id) =>
// {
//     var livro = livros.FirstOrDefault(b => b.Id == id);
//     if (livro is null) return Results.NotFound();

//     // Simula o "Include" buscando os dados relacionados nas outras listas
//     livro.Autor = autores.FirstOrDefault(a => a.Id == livro.AutorId);
//     livro.EmprestadoParaUsuario = usuarios.FirstOrDefault(u => u.Id == livro.EmprestadoParaUsuarioId);

//     return Results.Ok(livro);
// })
// .WithName("GetLivroPorId") // Damos um nome à rota para referência
// .WithTags("Livros");
app.MapGet("/api/livros/{id:int}", async (AppDataContext db, int id) =>
{
    var livro = await db.Livros
        .Include(l => l.Autor)
        .Include(l => l.EmprestadoParaUsuario)
        .FirstOrDefaultAsync(l => l.Id == id);
    return livro is not null ? Results.Ok(livro) : Results.NotFound();
})
.WithName("GetLivroPorId")
.WithTags("Livros");

// app.MapPost("/api/livros", ([FromBody] Livro novoLivro) =>
// {
//     var autorExiste = autores.Any(a => a.Id == novoLivro.AutorId);
//     if (!autorExiste) return Results.BadRequest("Autor não encontrado.");

//     // Simula a criação de um novo ID, tratando o caso da lista vazia
//     novoLivro.Id = livros.Any() ? livros.Max(b => b.Id) + 1 : 1;
//     livros.Add(novoLivro);
//     return Results.CreatedAtRoute("GetLivroPorId", new { id = novoLivro.Id }, novoLivro);
// }).WithTags("Livros");
app.MapPost("/api/livros", async (AppDataContext db, [FromBody] Livro novoLivro) =>
{
    var autorExiste = await db.Autores.AnyAsync(a => a.Id == novoLivro.AutorId);
    if (!autorExiste) return Results.BadRequest("Autor não encontrado.");

    db.Livros.Add(novoLivro);
    await db.SaveChangesAsync();
    return Results.CreatedAtRoute("GetLivroPorId", new { id = novoLivro.Id }, novoLivro);
}).WithTags("Livros");

// app.MapPut("/api/livros/{id:int}", (int id, [FromBody] Livro livroAtualizado) =>
// {
//     var livro = livros.FirstOrDefault(b => b.Id == id);
//     if (livro is null) return Results.NotFound();

//     var autorExiste = autores.Any(a => a.Id == livroAtualizado.AutorId);
//     if (!autorExiste) return Results.BadRequest("Autor não encontrado.");

//     livro.Titulo = livroAtualizado.Titulo;
//     livro.ISBN = livroAtualizado.ISBN;
//     livro.AnoPublicacao = livroAtualizado.AnoPublicacao;
//     livro.AutorId = livroAtualizado.AutorId;

//     return Results.Ok(livro);
// }).WithTags("Livros");
app.MapPost("/api/livros", async (AppDataContext db, [FromBody] Livro novoLivro) =>
{
    var autorExiste = await db.Autores.AnyAsync(a => a.Id == novoLivro.AutorId);
    if (!autorExiste) return Results.BadRequest("Autor não encontrado.");

    db.Livros.Add(novoLivro);
    await db.SaveChangesAsync();
    return Results.CreatedAtRoute("GetLivroPorId", new { id = novoLivro.Id }, novoLivro);
}).WithTags("Livros");

// app.MapDelete("/api/livros/{id:int}", (int id) =>
// {
//     var livro = livros.FirstOrDefault(b => b.Id == id);
//     if (livro is null) return Results.NotFound();

//     livros.Remove(livro);
//     return Results.Ok(livro);
// }).WithTags("Livros");
app.MapPut("/api/livros/{id:int}", async (AppDataContext db, int id, [FromBody] Livro livroAtualizado) =>
{
    var livro = await db.Livros.FindAsync(id);
    if (livro is null) return Results.NotFound();

    var autorExiste = await db.Autores.AnyAsync(a => a.Id == livroAtualizado.AutorId);
    if (!autorExiste) return Results.BadRequest("Autor não encontrado.");

    livro.Titulo = livroAtualizado.Titulo;
    livro.ISBN = livroAtualizado.ISBN;
    livro.AnoPublicacao = livroAtualizado.AnoPublicacao;
    livro.AutorId = livroAtualizado.AutorId;

    await db.SaveChangesAsync();
    return Results.Ok(livro);
}).WithTags("Livros");

app.MapDelete("/api/livros/{id:int}", async (AppDataContext db, int id) =>
{
    var livro = await db.Livros.FindAsync(id);
    if (livro is null) return Results.NotFound();

    db.Livros.Remove(livro);
    await db.SaveChangesAsync();
    return Results.Ok(livro);
}).WithTags("Livros");

#endregion

#region Endpoints de Empréstimos (Loans)

// app.MapPost("/api/livros/{livroId:int}/emprestar/{usuarioId:int}", (int livroId, int usuarioId) =>
// {
//     // 1. Encontrar o livro que será emprestado
//     var livro = livros.FirstOrDefault(l => l.Id == livroId);
//     if (livro is null)
//     {
//         return Results.NotFound("Livro não encontrado.");
//     }

//     // 2. Encontrar o usuário que está pegando o livro
//     var usuario = usuarios.FirstOrDefault(u => u.Id == usuarioId);
//     if (usuario is null)
//     {
//         return Results.NotFound("Usuário não encontrado.");
//     }

//     // 3. Verificar se o livro já está emprestado
//     if (livro.EmprestadoParaUsuarioId is not null)
//     {
//         return Results.Conflict($"Este livro já está emprestado para o usuário com ID {livro.EmprestadoParaUsuarioId}.");
//     }

//     // 4. Realizar o empréstimo
//     livro.EmprestadoParaUsuarioId = usuarioId;
//     livro.DataEmprestimo = DateTime.UtcNow;

//     // 5. Retornar o livro com os dados do empréstimo atualizados
//     return Results.Ok(livro);
// })
// .WithTags("Emprestimos");
app.MapPost("/api/livros/{livroId:int}/emprestar/{usuarioId:int}", async (AppDataContext db, int livroId, int usuarioId) =>
{
    var livro = await db.Livros.FindAsync(livroId);
    if (livro is null) return Results.NotFound("Livro não encontrado.");

    var usuario = await db.Usuarios.FindAsync(usuarioId);
    if (usuario is null) return Results.NotFound("Usuário não encontrado.");

    if (livro.EmprestadoParaUsuarioId is not null)
    {
        return Results.Conflict($"Este livro já está emprestado para o usuário com ID {livro.EmprestadoParaUsuarioId}.");
    }

    livro.EmprestadoParaUsuarioId = usuarioId;
    livro.DataEmprestimo = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(livro);
})
.WithTags("Emprestimos");

#endregion

app.Run();
