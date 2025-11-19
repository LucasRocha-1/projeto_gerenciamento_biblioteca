using BibliotecaApi.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDataContext>();

builder.Services.AddCors(options =>
    options.AddPolicy("Acesso Total",
        configs => configs
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod())
);

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

app.UseCors("Acesso Total");

#endregion


#region api de livros do google
app.MapPost("/api/seed", async (AppDataContext db) =>
{
    // 1. Verifica se já tem muitos livros para não duplicar à toa
    if (await db.Livros.CountAsync() > 0) 
        return Results.Ok("O banco já possui livros cadastrados.");

    using var client = new HttpClient();
    
    // 2. Busca livros sobre "Programação" na API do Google
    var url = "https://www.googleapis.com/books/v1/volumes?q=programacao&maxResults=20";
    var googleData = await client.GetFromJsonAsync<GoogleBooksResponse>(url);

    if (googleData?.Items is null) return Results.Problem("Erro ao buscar no Google");

    int livrosAdicionados = 0;

    foreach (var item in googleData.Items)
    {
        var info = item.VolumeInfo;
        if (info is null || string.IsNullOrEmpty(info.Title)) continue;

        // Pega o primeiro autor ou define como "Desconhecido"
        var nomeAutor = info.Authors?.FirstOrDefault() ?? "Autor Desconhecido";

        // 3. Lógica de Autor: Verifica se o autor já existe no banco para não duplicar
        var autor = await db.Autores.FirstOrDefaultAsync(a => a.Nome == nomeAutor);
        if (autor == null)
        {
            autor = new Autor { Nome = nomeAutor };
            db.Autores.Add(autor);
            await db.SaveChangesAsync(); // Salva o autor para gerar o ID
        }

        // 4. Cria o Livro
        var novoLivro = new Livro
        {
            Titulo = info.Title,
            AnoPublicacao = int.TryParse(info.PublishedDate?.Split('-')[0], out int ano) ? ano : 2000,
            ISBN = "Google-" + Guid.NewGuid().ToString().Substring(0, 6), 
            AutorId = autor.Id,
            CapaUrl = info.ImageLinks?.Thumbnail
        };

        db.Livros.Add(novoLivro);
        livrosAdicionados++;
    }

    await db.SaveChangesAsync();
    return Results.Ok(new { mensagem = $"{livrosAdicionados} livros importados do Google com sucesso!" });
})
.WithTags("Admin");
#endregion


app.Run();

public class GoogleBooksResponse { public List<GoogleBookItem>? Items { get; set; } }
public class GoogleBookItem { public GoogleBookVolumeInfo? VolumeInfo { get; set; } }
public class GoogleBookVolumeInfo
{
    public string? Title { get; set; }
    public List<string>? Authors { get; set; }
    public string? PublishedDate { get; set; }
    public GoogleBookImageLinks? ImageLinks { get; set; }
}
public class GoogleBookImageLinks 
{
    public string? Thumbnail { get; set; }
    public string? SmallThumbnail { get; set; }
}