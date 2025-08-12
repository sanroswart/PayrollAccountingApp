using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PayrollAccountingApp.Models;

namespace PayrollAccountingApp.Services;

public class AccountingApiClient
{
    private readonly HttpClient _http;
    private readonly AccountingApiOptions _opts;

    public AccountingApiClient(HttpClient http, IOptions<AccountingApiOptions> opts)
    {
        _http = http;
        _opts = opts.Value;
        _http.BaseAddress = new Uri(_opts.BaseUrl);
        _http.DefaultRequestHeaders.Add("x-api-key", _opts.ApiKey);
    }

    public async Task<IReadOnlyList<CatalogoCuenta>> GetCatalogoCuentasAsync(CancellationToken ct=default)
    {
        var res = await _http.GetAsync("public/catalogo-cuentas", ct);
        res.EnsureSuccessStatusCode();
        using var s = await res.Content.ReadAsStreamAsync(ct);
        var json = await JsonSerializer.DeserializeAsync<List<CatalogoCuenta>>(s, new JsonSerializerOptions{PropertyNameCaseInsensitive=true}, ct);
        return json ?? new List<CatalogoCuenta>();
    }

    public async Task<string?> CrearEntradaContableAsync(JournalEntry entry, CancellationToken ct=default)
    {
        var payload = new
        {
            fecha = entry.FechaAsiento.ToString("yyyy-MM-dd"),
            descripcion = entry.Descripcion,
            movimientos = entry.Movimientos.Select(m => new { cuenta = m.Cuenta, debe = m.Debe, haber = m.Haber })
        };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var res = await _http.PostAsync("public/entradas-contables", content, ct);
        res.EnsureSuccessStatusCode();
        var txt = await res.Content.ReadAsStringAsync(ct);
        return txt; // devolver el cuerpo; en real, parsear id externo
    }
}
