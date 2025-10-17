using System.Text.Json.Serialization;

namespace BibliotecaApi.Modelos;

public class Autor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    [JsonIgnore] // Evita loops de referência ao serializar para JSON
    public ICollection<Livro> Livros { get; set; } = new List<Livro>();
}