using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class EditarProduto : ContentPage
{
    Produto produtoAtual;

    public EditarProduto(Produto produto)
    {
        InitializeComponent();

        produtoAtual = produto;

        txt_descricao.Text = produtoAtual.Descricao;
        txt_quantidade.Text = produtoAtual.Quantidade.ToString();
        txt_preco.Text = produtoAtual.Preco.ToString();
    }

    private async void ToolBarItem_Clicked(object sender, EventArgs e)
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

            produtoAtual.Descricao = txt_descricao.Text;
            produtoAtual.Quantidade = quantidade;
            produtoAtual.Preco = preco;

            await App.Db.Update(produtoAtual);

            await DisplayAlert("Sucesso", "Produto atualizado com sucesso!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }
}