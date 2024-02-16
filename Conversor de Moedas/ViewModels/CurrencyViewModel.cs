using Conversor_de_Moedas.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Xml;

namespace Conversor_de_Moedas.ViewModels;

partial class CurrencyViewModel
{
    public ObservableCollection<Currency> TopPickerCurrencies { get; set; } = new();
    public ObservableCollection<Currency> BottomPickerCurrencies { get; set; } = new();

    public CurrencyViewModel()
    {
        _ = LoadCurrenciesToTopPicker();
        SortCurrencies();
    }

    async Task LoadCurrenciesToTopPicker()
    {             
        XmlNodeList currencyCombinationList = await ReadXmlFile("CurrencyCombinations.xml");
        List<string> currencyNames = new(); // Serve para conferir quais moedas já foram adicionadas. Usar a própria
                                            // TopPickerCurrencies para checar não funciona (tenho nem ideia do porquê)

        foreach (XmlNode currencyCombination in currencyCombinationList)
        {
            string xmlName = currencyCombination.Name;
            string[] xmlNameParts = xmlName.Split('-');

            string xmlInnerText = currencyCombination.InnerText;
            string[] xmlInnerTextParts = xmlInnerText.Split('/');

            if (xmlNameParts.Length == 2 && xmlInnerTextParts.Length == 2)
            {                       
                Currency currency = new Currency(xmlNameParts[0], xmlInnerTextParts[0]);    

                if (!currencyNames.Contains(currency.CurrencyName))
                {
                    currencyNames.Add(currency.CurrencyName);
                    TopPickerCurrencies.Add(currency);
                }               
            }         
        }        
    }

    public async Task UpdateBottomPickerCurrencies(Currency selectedTopCurrency)
    {
        BottomPickerCurrencies.Clear();

        XmlNodeList currencyCombinationList = await ReadXmlFile("CurrencyCombinations.xml");

        foreach (XmlNode currencyCombination in currencyCombinationList)
        {
            if (currencyCombination.Name.StartsWith(selectedTopCurrency.Code))
            {
                // Armazenar o código e o nome da moeda para a qual
                // será convertida a partir dos dados do arquivo XML
                string xmlName = currencyCombination.Name;
                string[] xmlNameParts = xmlName.Split('-'); 

                string xmlInnerText = currencyCombination.InnerText;
                string[] xmlInnerTextParts = xmlInnerText.Split('/');   

                if (xmlNameParts.Length == 2 && xmlInnerTextParts.Length == 2)
                {
                    Currency currency = new Currency(xmlNameParts[1], xmlInnerTextParts[1]);                            
                    BottomPickerCurrencies.Add(currency);                                       
                }
            }
        }
        SortCurrencies();
    }

    static async Task<XmlNodeList> ReadXmlFile (string xmlFilePath)
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(xmlFilePath);
            using var reader = new StreamReader(stream);

            string xmlContent = reader.ReadToEnd();

            XmlDocument xmlDoc = new();
            xmlDoc.LoadXml(xmlContent);
            XmlNodeList list = xmlDoc.DocumentElement!.ChildNodes;

            return list;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"1 - Erro ao ler arquivos de moedas: {ex.Message}");
            return new XmlDocument().CreateNode(XmlNodeType.Element, "EmptyList", "").ChildNodes;
        }
    }

    private void SortCurrencies()
    {
        TopPickerCurrencies = new ObservableCollection<Currency>(TopPickerCurrencies.OrderBy(c => c.CurrencyName));
        BottomPickerCurrencies = new ObservableCollection<Currency>(BottomPickerCurrencies.OrderBy(c => c.CurrencyName));        
    } 
}
