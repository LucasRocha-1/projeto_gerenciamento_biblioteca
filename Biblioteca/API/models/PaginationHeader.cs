namespace BibliotecaApi.Helpers;

public class PaginationHeader
{
    public int PaginaAtual { get; set; }
    public int ItensPorPagina { get;  set; }
    public int TodosItens { get;  set; }
    public int TodasPaginas { get;  set; }

    public PaginationHeader(int PaginaAtual, int ItensPorPagina, int TodosItens, int TodasPaginas)
    {
        this.PaginaAtual = PaginaAtual;
        this.ItensPorPagina = ItensPorPagina;
        this.TodosItens = TodosItens;
        this.TodasPaginas = TodasPaginas;
    }
}