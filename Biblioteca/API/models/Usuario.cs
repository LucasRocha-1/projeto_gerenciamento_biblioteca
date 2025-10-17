using System.Text.Json.Serialization;

namespace BibliotecaApi.Modelos;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ICollection<Livro> Livros { get; set; }
}