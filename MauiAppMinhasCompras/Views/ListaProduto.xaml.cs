using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProdutos : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
    string textoPesquisaAtual = string.Empty;

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

            await CarregarCategorias();
            await CarregarProdutos();
            await AtualizarRelatorio();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", $"Erro ao carregar produtos: {ex.Message}", "OK");
        }
    }

    private async Task CarregarProdutos()
    {
        lista.Clear();

        List<Produto> tmp;

        if (string.IsNullOrWhiteSpace(textoPesquisaAtual))
            tmp = await App.Db.GetAll();
        else
            tmp = await App.Db.Search(textoPesquisaAtual);

        string categoriaSelecionada = picker_categoria.SelectedItem?.ToString() ?? "Todas";

        if (!string.IsNullOrWhiteSpace(categoriaSelecionada) && categoriaSelecionada != "Todas")
        {
            tmp = tmp
                .Where(p => !string.IsNullOrWhiteSpace(p.Categoria) &&
                            p.Categoria.Equals(categoriaSelecionada, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        tmp.ForEach(i => lista.Add(i));
    }

    private async Task CarregarCategorias()
    {
        string categoriaAtual = picker_categoria.SelectedItem?.ToString() ?? "Todas";

        List<Produto> produtos = await App.Db.GetAll();

        List<string> categorias = produtos
            .Where(p => !string.IsNullOrWhiteSpace(p.Categoria))
            .Select(p => p.Categoria)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        picker_categoria.Items.Clear();
        picker_categoria.Items.Add("Todas");

        foreach (string categoria in categorias)
            picker_categoria.Items.Add(categoria);

        if (picker_categoria.Items.Contains(categoriaAtual))
            picker_categoria.SelectedItem = categoriaAtual;
        else
            picker_categoria.SelectedIndex = 0;
    }

    private async Task AtualizarRelatorio()
    {
        List<Produto> produtos = await App.Db.GetAll();

        if (produtos.Count == 0)
        {
            lbl_relatorio.Text = "Relatório por categoria:\nNenhum produto cadastrado.";
            return;
        }

        var grupos = produtos
            .Where(p => !string.IsNullOrWhiteSpace(p.Categoria))
            .GroupBy(p => p.Categoria)
            .OrderBy(g => g.Key)
            .Select(g => $"{g.Key}: {g.Sum(p => p.Total):C}")
            .ToList();

        if (grupos.Count == 0)
        {
            lbl_relatorio.Text = "Relatório por categoria:\nNenhuma categoria informada.";
            return;
        }

        lbl_relatorio.Text = "Relatório por categoria:\n" + string.Join("\n", grupos);
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
            textoPesquisaAtual = e.NewTextValue?.Trim() ?? string.Empty;

            await CarregarProdutos();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", $"Erro na busca: {ex.Message}", "OK");
        }
    }

    private async void picker_categoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            await CarregarProdutos();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", $"Erro ao filtrar por categoria: {ex.Message}", "OK");
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

                await CarregarCategorias();
                await CarregarProdutos();
                await AtualizarRelatorio();

                await DisplayAlert("Sucesso", "Produto excluído com sucesso!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", $"Erro ao excluir produto: {ex.Message}", "OK");
        }
    }
}