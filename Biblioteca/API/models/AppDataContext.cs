using System;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Modelos;

public class AppDataContext : DbContext
{
    public DbSet<Livro> Livros { get; set; }
    public DbSet<Autor> Autores { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Emprestimo> Emprestimos { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        optionsBuilder.UseSqlite("Data Source=Biblioteca.db");
    }
}