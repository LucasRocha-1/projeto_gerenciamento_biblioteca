using BibliotecaApi.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDataContext>();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
    options.AddPolicy("Acesso Total",
        configs => configs
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod())
);

var app = builder.Build();

// Seeding de dados iniciais
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDataContext>();
        // Antes de aplicar migrations, marque manualmente migrations problemáticas
        try
        {
            // Garante que a tabela de histórico exista
            db.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (\"MigrationId\" TEXT NOT NULL CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY, \"ProductVersion\" TEXT NOT NULL);");
            // Marca a migration que recria tabelas (evita erro 'table already exists') como aplicada
            db.Database.ExecuteSqlRaw("INSERT OR IGNORE INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ('20251119001616_AdicionandoCapas', '9.0.10');");
        }
        catch
        {
            // Ignorar falhas aqui — iremos tentar aplicar migrations normalmente em seguida
        }

        // Aplica migrations pendentes automaticamente (garante que colunas como IsAdmin existam)
        try
        {
            db.Database.Migrate();
        }
        catch (Microsoft.Data.Sqlite.SqliteException ex)
        {
            if (!ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                throw;
        }
    
    // Adicionar autores se não existirem
    if (!db.Autores.Any())
    {
        var autores = new List<Autor>
        {
            new Autor { Nome = "Machado de Assis" },
            new Autor { Nome = "Jorge Amado" },
            new Autor { Nome = "Aluísio Azevedo" },
            new Autor { Nome = "Paulo Coelho" },
            new Autor { Nome = "Clarice Lispector" },
            new Autor { Nome = "Carlos Drummond de Andrade" },
            new Autor { Nome = "Cecília Meireles" },
            new Autor { Nome = "Cruz e Sousa" }
        };
        db.Autores.AddRange(autores);
        db.SaveChanges();
        Console.WriteLine("Autores adicionados com sucesso!");
    }

    // Adicionar livros se não existirem
    if (!db.Livros.Any())
    {
        // Garantir que a coluna CapaUrl exista (pode não existir se uma migration que recriava tabelas foi ignorada)
        try
        {
            var connCheck = db.Database.GetDbConnection();
            connCheck.Open();
            using (var cmdCheck = connCheck.CreateCommand())
            {
                cmdCheck.CommandText = "PRAGMA table_info('Livros');";
                using var rdr = cmdCheck.ExecuteReader();
                var hasCapa = false;
                while (rdr.Read())
                {
                    var col = rdr.GetString(1);
                    if (string.Equals(col, "CapaUrl", StringComparison.OrdinalIgnoreCase))
                    {
                        hasCapa = true;
                        break;
                    }
                }
                rdr.Close();
                if (!hasCapa)
                {
                    db.Database.ExecuteSqlRaw("ALTER TABLE Livros ADD COLUMN CapaUrl TEXT NULL;");
                }
            }
            connCheck.Close();
        }
        catch
        {
            // Não bloquear o seed caso não seja possível alterar o schema aqui
        }
        var autores = db.Autores.ToList();
        var livros = new List<Livro>
        {
            new Livro { Titulo = "Dom Casmurro", ISBN = "978-8535902778", AnoPublicacao = 1899, AutorId = autores.First(a => a.Nome == "Machado de Assis").Id },
            new Livro { Titulo = "Quincas Borba", ISBN = "978-8535928556", AnoPublicacao = 1891, AutorId = autores.First(a => a.Nome == "Machado de Assis").Id },
            new Livro { Titulo = "Memórias Póstumas de Brás Cubas", ISBN = "978-8525404010", AnoPublicacao = 1881, AutorId = autores.First(a => a.Nome == "Machado de Assis").Id },
            new Livro { Titulo = "Capitães da Areia", ISBN = "978-8525404492", AnoPublicacao = 1937, AutorId = autores.First(a => a.Nome == "Jorge Amado").Id },
            new Livro { Titulo = "Gabriela, Cravo e Canela", ISBN = "978-8525404676", AnoPublicacao = 1958, AutorId = autores.First(a => a.Nome == "Jorge Amado").Id },
            new Livro { Titulo = "O Cortiço", ISBN = "978-8525404775", AnoPublicacao = 1890, AutorId = autores.First(a => a.Nome == "Aluísio Azevedo").Id },
            new Livro { Titulo = "O Alquimista", ISBN = "978-8532604165", AnoPublicacao = 1988, AutorId = autores.First(a => a.Nome == "Paulo Coelho").Id },
            new Livro { Titulo = "Zahir", ISBN = "978-8532614170", AnoPublicacao = 2005, AutorId = autores.First(a => a.Nome == "Paulo Coelho").Id },
            new Livro { Titulo = "A Hora da Estrela", ISBN = "978-8525406526", AnoPublicacao = 1977, AutorId = autores.First(a => a.Nome == "Clarice Lispector").Id },
            new Livro { Titulo = "Sentimento do Mundo", ISBN = "978-8525404850", AnoPublicacao = 1940, AutorId = autores.First(a => a.Nome == "Carlos Drummond de Andrade").Id },
            new Livro { Titulo = "Romanceiro da Inconfidência", ISBN = "978-8525404935", AnoPublicacao = 1953, AutorId = autores.First(a => a.Nome == "Cecília Meireles").Id },
            new Livro { Titulo = "Missal", ISBN = "978-8525405017", AnoPublicacao = 1893, AutorId = autores.First(a => a.Nome == "Cruz e Sousa").Id }
        };
        db.Livros.AddRange(livros);
        db.SaveChanges();
        Console.WriteLine("Livros adicionados com sucesso!");
    }
}

app.MapControllers();
app.UseCors("Acesso Total");



#region api de livros do google
app.MapPost("/api/seed", async (AppDataContext db) =>
{
    using var client = new HttpClient();

    // Lista de Assuntos Populares (Pode adicionar mais se quiser)
    var assuntos = new List<string> 
    { 
        "ficcao+fantasia", "harry+potter", "senhor+dos+aneis", "agatha+christie", 
        "stephen+king", "george+orwell", "jogos+vorazes", "sherlock+holmes", 
        "percy+jackson", "isaac+asimov", "julia+quinn", "game+of+thrones", "star+wars"
    };

    var random = new Random();
    var assuntoEscolhido = assuntos[random.Next(assuntos.Count)];

    // Busca 15 livros do assunto sorteado
    var url = $"https://www.googleapis.com/books/v1/volumes?q={assuntoEscolhido}&maxResults=15&langRestrict=pt";
    var googleData = await client.GetFromJsonAsync<GoogleBooksResponse>(url);

    if (googleData?.Items is null) return Results.Problem("Erro no Google ou nenhum livro encontrado.");

    int livrosAdicionados = 0;

    foreach (var item in googleData.Items)
    {
        var info = item.VolumeInfo;
        if (info is null || string.IsNullOrEmpty(info.Title)) continue;
        if (await db.Livros.AnyAsync(l => l.Titulo == info.Title)) continue;

        // 1. Cria ou Busca Autor
        var nomeAutor = info.Authors?.FirstOrDefault() ?? "Autor Desconhecido";
        var autor = await db.Autores.FirstOrDefaultAsync(a => a.Nome == nomeAutor);
        if (autor == null)
        {
            autor = new Autor { Nome = nomeAutor };
            db.Autores.Add(autor);
            await db.SaveChangesAsync();
        }

        // Cria o Livro
        var novoLivro = new Livro
        {
            Titulo = info.Title,
            AnoPublicacao = int.TryParse(info.PublishedDate?.Split('-')[0], out int ano) ? ano : 2020,
            ISBN = "GGL-" + Guid.NewGuid().ToString().Substring(0, 6),
            AutorId = autor.Id,
            CapaUrl = info.ImageLinks?.Thumbnail?.Replace("http://", "https://") // Garante HTTPS na imagem
        };

        db.Livros.Add(novoLivro);
        livrosAdicionados++;
    }

    await db.SaveChangesAsync();
    
    return Results.Ok(new { 
        mensagem = $"Importação finalizada! Assunto: '{assuntoEscolhido}'. {livrosAdicionados} novos livros adicionados." 
    });
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