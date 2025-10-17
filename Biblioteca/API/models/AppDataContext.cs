namespace API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using BibliotecaApi.Modelos;

//Entity Framework: Code First
//1 - Implementou a herança da classe DbContext
//2 - Criou as propriedades que vão informar quais as classes
//vão virar tabelas no banco
//3 - Configurar qual o banco utilizado e a string de conexão
public class AppDataContext : DbContext
{
    public DbSet<Autor> Autores { get; set; }
    public DbSet<Livro> Livros { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Biblioteca.db");
    }

}
