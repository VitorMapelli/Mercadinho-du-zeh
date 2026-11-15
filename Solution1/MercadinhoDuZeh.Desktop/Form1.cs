using MercadinhoDuZeh.Classes.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MercadinhoDuZeh.Desktop
{
    public partial class Form1 : Form
    {
        private readonly ClienteService clienteService;
        private readonly DividasService dividasService;

        private readonly TextBox txtBusca = new TextBox();
        private readonly DataGridView gridClientes = new DataGridView();

        public Form1(
            ClienteService clienteService,
            DividasService dividasService)
        {
            InitializeComponent();

            this.clienteService = clienteService;
            this.dividasService = dividasService;

            Text = "Mercadinho Du Zéh - Clientes";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(900, 500);

            var lblTitulo = new Label
            {
                Text = "Clientes",
                Location = new Point(20, 15),
                AutoSize = true
            };

            txtBusca.Location = new Point(20, 55);
            txtBusca.Size = new Size(230, 25);

            var btnPesquisar = new Button
            {
                Text = "Pesquisar",
                Location = new Point(260, 53),
                Size = new Size(90, 28)
            };
            btnPesquisar.Click += (s, e) => CarregarClientes(txtBusca.Text);

            var btnNovo = new Button
            {
                Text = "Novo",
                Location = new Point(380, 53),
                Size = new Size(80, 28)
            };

            var btnEditar = new Button
            {
                Text = "Editar",
                Location = new Point(470, 53),
                Size = new Size(80, 28)
            };

            var btnExcluir = new Button
            {
                Text = "Excluir",
                Location = new Point(560, 53),
                Size = new Size(80, 28)
            };

            var btnDividas = new Button
            {
                Text = "Dívidas",
                Location = new Point(650, 53),
                Size = new Size(80, 28)
            };

            btnNovo.Click += (s, e) =>
            {
                using var formulario = new ClienteForm(clienteService);

                if (formulario.ShowDialog(this) == DialogResult.OK)
                {
                    CarregarClientes(txtBusca.Text);
                }
            };

            gridClientes.Location = new Point(20, 100);
            gridClientes.Size = new Size(860, 370);
            gridClientes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                                  AnchorStyles.Left | AnchorStyles.Right;
            gridClientes.ReadOnly = true;
            gridClientes.AllowUserToAddRows = false;
            gridClientes.RowHeadersVisible = false;
            gridClientes.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            gridClientes.Columns.Add("Nome", "Nome");
            gridClientes.Columns.Add("CPF", "CPF");
            gridClientes.Columns.Add("Idade", "Idade");
            gridClientes.Columns.Add("Total", "Total em aberto");

            Controls.Add(lblTitulo);
            Controls.Add(txtBusca);
            Controls.Add(btnPesquisar);
            Controls.Add(btnNovo);
            Controls.Add(btnEditar);
            Controls.Add(btnExcluir);
            Controls.Add(btnDividas);
            Controls.Add(gridClientes);

            CarregarClientes();
        }

        private void CarregarClientes(string pesquisa = "")
        {
            gridClientes.Rows.Clear();

            var clientes = clienteService.Consultar();

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                clientes = clientes
                    .Where(cliente => cliente.nome.Contains(
                        pesquisa,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            foreach (var cliente in clientes)
            {
                var totalEmAberto = dividasService
                    .Consultar(cliente.id)
                    .Where(divida => !divida.Paga)
                    .Sum(divida => divida.Valor);

                gridClientes.Rows.Add(
                    cliente.nome,
                    cliente.cpf,
                    cliente.Idade,
                    totalEmAberto.ToString("C"));
            }
        }
    }
}