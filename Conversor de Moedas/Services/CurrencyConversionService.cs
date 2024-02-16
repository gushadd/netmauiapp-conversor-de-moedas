using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Conversor_de_Moedas.Services;

public class CurrencyConversionService
{
    public double conversionResult { get; set; }
    private HttpClient httpClient = new();

    public async Task ConvertCurrency(string topCurrencyCode, string bottomCurrencyCode)
    {
        string conversion = $"{topCurrencyCode}-{bottomCurrencyCode}";

        try
        {
            string apiUrl = $"https://economia.awesomeapi.com.br/last/{conversion}";

            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync();                
                JObject json = JObject.Parse(responseJson);
                
                foreach (var currency in json)
                {  
                    conversionResult = Convert.ToDouble(currency.Value["ask"].ToString());
                }
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine($"Erro ao conectar-se a API: ", ex.Message);
        }
    }
}
