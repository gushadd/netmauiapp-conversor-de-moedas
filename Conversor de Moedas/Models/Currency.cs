namespace Conversor_de_Moedas.Models;

internal class Currency(string code, string name)
{
    public string Code { get; set; } = code;
    public string CurrencyName { get; set; } = name;
}

