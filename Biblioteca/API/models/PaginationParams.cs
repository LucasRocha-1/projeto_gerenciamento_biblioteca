namespace BibliotecaApi.Helpers;

public class PaginationParams
{
    private const int TamanhoMaximoPagina = 50;
    public int NumeroPagina { get; set; } = 1;
    private int _tamanhopagina = 10;
    public int PageSize
    {
        get => _tamanhopagina;
        set => _tamanhopagina = (value > TamanhoMaximoPagina) ? TamanhoMaximoPagina : value;
    }
}