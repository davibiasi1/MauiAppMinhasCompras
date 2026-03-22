using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class NovoProduto : ContentPage
{
    public NovoProduto()
    {
        InitializeComponent();
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txt_descricao.Text))
            {
                await DisplayAlert("Atenção", "Informe a descrição do produto.", "OK");
                return;
            }

            if (!double.TryParse(txt_quantidade.Text, out double quantidade))
            {
                await DisplayAlert("Atenção", "Informe uma quantidade válida.", "OK");
                return;
            }

            if (!double.TryParse(txt_preco.Text, out double preco))
            {
                await DisplayAlert("Atenção", "Informe um preço válido.", "OK");
                return;
            }

            Produto p = new Produto
            {
                Descricao = txt_descricao.Text,
                Quantidade = quantidade,
                Preco = preco
            };

            await App.Db.Insert(p);

            await DisplayAlert("Sucesso", "Produto cadastrado com sucesso!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }
}