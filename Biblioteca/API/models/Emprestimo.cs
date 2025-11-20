namespace BibliotecaApi.Modelos;

public class Emprestimo
{
    public int Id { get; set; }
    public int LivroId { get; set; }
    public string LivroTitulo { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public DateTime DataEmprestimo { get; set; } = DateTime.UtcNow;
    public DateTime? DataDevolucao { get; set; }
    public string Status { get; set; } = "ativo"; // ativo ou devolvido
}
