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


            //int.TryParse serve para converter o string em inteiro (ideal lockar o campo somente para inteiro) <-- ver isso
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


        // Métodos de edição e exclusão não testados
        // Necessário adicionar 2 botões no formulário: buttonEditar & buttonExcluir
        private void buttonEditar_Click(object sender, EventArgs e)
        {
            // Verifica se alguma linha foi selecionada no dataGridView1
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtém o produto selecionado na linha selecionada
                produto produtoSelecionado = dataGridView1.SelectedRows[0].DataBoundItem as produto;

                // Abre um novo formulário de edição passando o produto selecionado
                FormEditarProduto formEditar = new FormEditarProduto(produtoSelecionado);
                formEditar.ShowDialog();

                // Atualiza a listagem após a edição (se necessário)
                if (formEditar.ProdutoEditado != null)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione um produto para editar.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonExcluir_Click(object sender, EventArgs e)
        {
            // Verifica se alguma linha foi selecionada no dataGridView1
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtém o produto selecionado na linha selecionada
                produto produtoSelecionado = dataGridView1.SelectedRows[0].DataBoundItem as produto;

                // Pergunta ao usuário se ele realmente deseja excluir o produto
                DialogResult result = MessageBox.Show($"Tem certeza que deseja excluir o produto '{produtoSelecionado.nome}'?",
                    "Confirmação de Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Se o usuário confirmar a exclusão, exclui o produto
                if (result == DialogResult.Yes)
                {
                    // Exclui o produto do banco de dados
                    collection.DeleteOne(p => p.Id == produtoSelecionado.Id);

                    // Atualiza a listagem após a exclusão
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione um produto para excluir.", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}