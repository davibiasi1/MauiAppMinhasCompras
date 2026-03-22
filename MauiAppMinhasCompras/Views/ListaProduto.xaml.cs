using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProdutos : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

    public ListaProdutos()
    {
        InitializeComponent();
        lst_produtos.ItemsSource = lista;
    }

    protected async override void OnAppearing()
    {
        try
        {
            base.OnAppearing();

            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();
            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", $"Erro ao carregar produtos: {ex.Message}", "OK");
        }
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new Views.NovoProduto());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string q = e.NewTextValue?.Trim() ?? "";

            lista.Clear();

            List<Produto> tmp;

            if (string.IsNullOrWhiteSpace(q))
                tmp = await App.Db.GetAll();
            else
                tmp = await App.Db.Search(q);

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", $"Erro na busca: {ex.Message}", "OK");
        }
    }

    private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        try
        {
            double soma = lista.Sum(i => i.Total);
            string msg = $"O total é {soma:C}";
            await DisplayAlert("Total dos Produtos", msg, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", $"Erro ao calcular total: {ex.Message}", "OK");
        }
    }

    private async void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            if (e.SelectedItem is Produto produtoSelecionado)
            {
                await Navigation.PushAsync(new EditarProduto(produtoSelecionado));
                lst_produtos.SelectedItem = null;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", $"Erro ao abrir produto: {ex.Message}", "OK");
        }
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem item = (MenuItem)sender;
            Produto produto = (Produto)item.CommandParameter;

            bool confirmar = await DisplayAlert(
                "Confirmar exclusão",
                $"Deseja excluir o produto '{produto.Descricao}'?",
                "Sim",
                "Não");

            if (confirmar)
            {
                await App.Db.Delete(produto.Id);
                lista.Remove(produto);

                await DisplayAlert("Sucesso", "Produto excluído com sucesso!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", $"Erro ao excluir produto: {ex.Message}", "OK");
        }
    }
}