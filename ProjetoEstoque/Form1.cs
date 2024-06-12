using MongoDB.Driver;
using Newtonsoft.Json;
using ProjetoEstoque.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoEstoque
{
    public partial class Form1 : Form
    {
        private MongoClient client;
        private IMongoCollection<produto> collection;

        public Form1()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadData();
        }

        private void InitializeDatabase()
        {
            client = new MongoClient("mongodb://localhost:27017/");
            var database = client.GetDatabase("DBProdutos");
            collection = database.GetCollection<produto>("Produto");
        }

        private void LoadData()
        {
            var listaProdutos = collection.Find(p => true).ToList();
            dataGridView1.DataSource = listaProdutos;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string product_name = maskedTextBox1.Text;
            string product_qnt = maskedTextBox2.Text;


            //int.TryParse serve para converter o string em inteiro (ideal lockar o campo somente para inteiro)
            if (!string.IsNullOrWhiteSpace(product_name) && int.TryParse(product_qnt, out int quantidade))
            {
                try
                {
                    // Criar um objeto Produto com os valores fornecidos
                    var produto = new produto
                    {
                        nome = product_name,
                        quantidade = quantidade
                    };

                    // Insira o objeto Produto na coleção
                    collection.InsertOne(produto);

                    MessageBox.Show("Produto adicionado com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    maskedTextBox1.Clear();
                    maskedTextBox2.Clear();

                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ocorreu um erro na inserção: {ex.Message}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor, inclua um nome de produto válido e uma quantidade numérica.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}