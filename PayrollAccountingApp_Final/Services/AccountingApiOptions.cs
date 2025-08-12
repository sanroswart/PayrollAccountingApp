namespace PayrollAccountingApp.Services;

public class AccountingApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DebitAccount { get; set; } = "6101";
    public string CreditAccount { get; set; } = "2101";
}

public record CatalogoCuenta(string Codigo, string Nombre);
