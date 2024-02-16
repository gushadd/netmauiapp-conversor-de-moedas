using Conversor_de_Moedas.Models;
using Conversor_de_Moedas.ViewModels;
using Conversor_de_Moedas.Services;

namespace Conversor_de_Moedas.Views;

public partial class Convert : ContentPage
{
    private CurrencyViewModel currencyViewModel = new();
	private CurrencyConversionService currencyConversionService = new();

    public Convert()
	{
		InitializeComponent();
		
		BindingContext = currencyViewModel;
		TopPicker.ItemsSource = currencyViewModel.TopPickerCurrencies;
		ValueEntry.IsEnabled = false;
	}

	private void TopPicker_SelectedIndexChanged(object sender, EventArgs e)
	{
        var selectedTopCurrency = (Currency)TopPicker.SelectedItem;		
		_ = currencyViewModel.UpdateBottomPickerCurrencies(selectedTopCurrency);
        BottomPicker.ItemsSource = currencyViewModel.BottomPickerCurrencies;

		TopCurrencyLabel.Text = selectedTopCurrency.Code;

		BottomPicker.SelectedItem = null;

		ValueEntry.IsEnabled = false;
		ValueEntry.Text = "  ";
		ValueEntry.Placeholder = "Digite um valor   ";

		ValueResult.Text = "  ";
    }

	private void BottomPicker_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (BottomPicker.SelectedItem != null)
		{
			var selectedBottomCurrency = (Currency)BottomPicker.SelectedItem;
			BottomCurrencyLabel.Text = selectedBottomCurrency.Code;
			ValueEntry.IsEnabled = true;
		}
	}

	private async void ConverterButton_Clicked(object sender, EventArgs e)
	{
        var selectedTopCurrency = (Currency)TopPicker.SelectedItem;
        var selectedBottomCurrency = (Currency)BottomPicker.SelectedItem;

        await currencyConversionService.ConvertCurrency(selectedTopCurrency.Code, selectedBottomCurrency.Code);

        double value;
        double.TryParse(ValueEntry.Text, out value);

        double result = value * currencyConversionService.conversionResult;
		 
        ValueResult.Text = FormatDecimal(result.ToString("0.000000"));        
    }

	private static string FormatDecimal(string result)
	{
		result = result.TrimEnd('0');

		if (result.EndsWith("."))
		{
            result = result.TrimEnd('.');
        }
		return result;
    }
}