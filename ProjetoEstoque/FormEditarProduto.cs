using MongoDB.Driver;
using ProjetoEstoque.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoEstoque
{
    public partial class FormEditarProduto : Form
    {
        private produto produtoEditado;
        private MongoClient client;
        private IMongoCollection<produto> collection;

        public produto ProdutoEditado
        {
            get { return produtoEditado; }
            private set { produtoEditado = value; }
        }

        public FormEditarProduto(produto produto)
        {
            InitializeComponent();
            InitializeDatabase();

            // Preenche os controles de entrada com os detalhes do produto
            textBoxID.Text = produto.id;
            textBoxNome.Text = produto.nome;
            maskedTextBoxQtd.Text = produto.quantidade.ToString();

            textBoxID.Enabled = false;

            ProdutoEditado = produto;
        }

        private void InitializeDatabase()
        {
            client = new MongoClient("mongodb://localhost:27017/");
            var database = client.GetDatabase("DBProdutos");
            collection = database.GetCollection<produto>("Produto");
        }

        private void buttonSalvar_Click(object sender, EventArgs e)
        {
            // Coleta os novos dados dos controles de entrada
            string mesmoID = textBoxID.Text;
            string novoNome = textBoxNome.Text;
            int novaQuantidade;

            if (!int.TryParse(maskedTextBoxQtd.Text, out novaQuantidade))
            {
                MessageBox.Show("Por favor, insira uma quantidade válida.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Atualiza os detalhes do produto com os novos dados
            ProdutoEditado.id = mesmoID;
            ProdutoEditado.nome = novoNome;
            ProdutoEditado.quantidade = novaQuantidade;

            var filter = Builders<produto>.Filter.Eq("id", ProdutoEditado.id);
            collection.ReplaceOne(filter, ProdutoEditado);

            this.Close();
        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
