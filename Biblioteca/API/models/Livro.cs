namespace BibliotecaApi.Modelos;

public class Livro
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int AnoPublicacao { get; set; }

    // Relacionamento com Autor
    public int AutorId { get; set; }
    public Autor? Autor { get; set; }

    // Rastreamento de Empréstimo
    public int? EmprestadoParaUsuarioId { get; set; } // Nullable, pois o livro pode não estar emprestado
    public Usuario? EmprestadoParaUsuario { get; set; }
    public DateTime? DataEmprestimo { get; set; }
    public string? CapaUrl { get; set; }
}