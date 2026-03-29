using MauiAppMinhasCompras.Models;
using SQLite;
using System.Linq;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;

        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
            _conn.CreateTableAsync<Produto>().Wait();
            AddCategoriaColumnIfNeeded();
        }

        private void AddCategoriaColumnIfNeeded()
        {
            var columns = _conn.GetTableInfoAsync("Produto").Result;
            bool categoriaExiste = columns.Any(c => c.Name == "Categoria");

            if (!categoriaExiste)
            {
                _conn.ExecuteAsync("ALTER TABLE Produto ADD COLUMN Categoria TEXT").Wait();
            }
        }

        public Task<int> Insert(Produto p)
        {
            return _conn.InsertAsync(p);
        }

        public Task<List<Produto>> Update(Produto p)
        {
            string sql = "UPDATE Produto SET Descricao = ?, Categoria = ?, Quantidade = ?, Preco = ? WHERE Id = ?";

            return _conn.QueryAsync<Produto>(sql, p.Descricao, p.Categoria, p.Quantidade, p.Preco, p.Id);
        }

        public Task<int> Delete(int id)
        {
            return _conn.Table<Produto>().DeleteAsync(i => i.Id == id);
        }

        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>().ToListAsync();
        }

        public Task<List<Produto>> Search(string q)
        {
            string sql = "SELECT * FROM Produto WHERE Descricao LIKE ?";

            return _conn.QueryAsync<Produto>(sql, "%" + q + "%");
        }

        public Task<List<Produto>> FilterByCategoria(string categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria))
                return GetAll();

            string sql = "SELECT * FROM Produto WHERE Categoria = ?";

            return _conn.QueryAsync<Produto>(sql, categoria);
        }
    }
}