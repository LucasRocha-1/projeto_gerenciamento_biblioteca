using System;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Modelos;

public class AppDataContext : DbContext
{
    public DbSet<Livro> Livros { get; set; }
    public DbSet<Autor> Autores { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        optionsBuilder.UseSqlite("Data Source=Biblioteca.db");
    }
     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Autor>()
            .Property(a => a.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Livro>()
            .Property(a => a.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Usuario>()
            .Property(a => a.Id)
            .ValueGeneratedOnAdd();
    }
}