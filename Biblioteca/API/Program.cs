using BibliotecaApi.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

    // Salva as alterações no banco.
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
app.MapGet("/api/livros", () =>
{

    foreach (var livro in livros)
    {
        livro.Autor = autores.FirstOrDefault(a => a.Id == livro.AutorId);
    }
    return Results.Ok(livros);
}).WithTags("Livros");

app.MapGet("/api/livros/{id:int}", (int id) =>
{
    var livro = livros.FirstOrDefault(b => b.Id == id);
    if (livro is null) return Results.NotFound();

    // Simula o "Include" buscando os dados relacionados nas outras listas
    livro.Autor = autores.FirstOrDefault(a => a.Id == livro.AutorId);
    livro.EmprestadoParaUsuario = usuarios.FirstOrDefault(u => u.Id == livro.EmprestadoParaUsuarioId);

    return Results.Ok(livro);
})
.WithName("GetLivroPorId") // Damos um nome à rota para referência
.WithTags("Livros");

app.MapPost("/api/livros", ([FromBody] Livro novoLivro) =>
{
    var autorExiste = autores.Any(a => a.Id == novoLivro.AutorId);
    if (!autorExiste) return Results.BadRequest("Autor não encontrado.");

    // Simula a criação de um novo ID, tratando o caso da lista vazia
    novoLivro.Id = livros.Any() ? livros.Max(b => b.Id) + 1 : 1;
    livros.Add(novoLivro);
    return Results.CreatedAtRoute("GetLivroPorId", new { id = novoLivro.Id }, novoLivro);
}).WithTags("Livros");

app.MapPut("/api/livros/{id:int}", (int id, [FromBody] Livro livroAtualizado) =>
{
    var livro = livros.FirstOrDefault(b => b.Id == id);
    if (livro is null) return Results.NotFound();

    var autorExiste = autores.Any(a => a.Id == livroAtualizado.AutorId);
    if (!autorExiste) return Results.BadRequest("Autor não encontrado.");

    livro.Titulo = livroAtualizado.Titulo;
    livro.ISBN = livroAtualizado.ISBN;
    livro.AnoPublicacao = livroAtualizado.AnoPublicacao;
    livro.AutorId = livroAtualizado.AutorId;

    return Results.Ok(livro);
}).WithTags("Livros");

app.MapDelete("/api/livros/{id:int}", (int id) =>
{
    var livro = livros.FirstOrDefault(b => b.Id == id);
    if (livro is null) return Results.NotFound();

    livros.Remove(livro);
    return Results.Ok(livro);
}).WithTags("Livros");

#endregion

#region Endpoints de Empréstimos (Loans)

app.MapPost("/api/livros/{livroId:int}/emprestar/{usuarioId:int}", (int livroId, int usuarioId) =>
{
    // 1. Encontrar o livro que será emprestado
    var livro = livros.FirstOrDefault(l => l.Id == livroId);
    if (livro is null)
    {
        return Results.NotFound("Livro não encontrado.");
    }

    // 2. Encontrar o usuário que está pegando o livro
    var usuario = usuarios.FirstOrDefault(u => u.Id == usuarioId);
    if (usuario is null)
    {
        return Results.NotFound("Usuário não encontrado.");
    }

    // 3. Verificar se o livro já está emprestado
    if (livro.EmprestadoParaUsuarioId is not null)
    {
        return Results.Conflict($"Este livro já está emprestado para o usuário com ID {livro.EmprestadoParaUsuarioId}.");
    }

    // 4. Realizar o empréstimo
    livro.EmprestadoParaUsuarioId = usuarioId;
    livro.DataEmprestimo = DateTime.UtcNow;

    // 5. Retornar o livro com os dados do empréstimo atualizados
    return Results.Ok(livro);
})
.WithTags("Emprestimos");

#endregion

app.Run();
