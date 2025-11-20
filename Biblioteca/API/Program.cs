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
app.Run();
