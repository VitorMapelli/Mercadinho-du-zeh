using MercadinhoDuZeh.Classes.Services;
using System;
using System.Drawing;
using System.Globalization;
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
        private readonly Button btnAnterior = new Button();
        private readonly Button btnProxima = new Button();
        private readonly Label lblPagina = new Label();

        private int paginaAtual = 1;
        private const int TamanhoPagina = 10;

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
            txtBusca.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    PesquisarClientes();
                }
            };

            var btnPesquisar = new Button
            {
                Text = "Pesquisar",
                Location = new Point(260, 53),
                Size = new Size(90, 28)
            };
            btnPesquisar.Click += (s, e) => PesquisarClientes();

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
                    CarregarClientes();
                }
            };

            btnEditar.Click += (s, e) => EditarCliente();
            btnExcluir.Click += (s, e) => ExcluirCliente();
            btnDividas.Click += (s, e) => AbrirDividas();

            gridClientes.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            gridClientes.MultiSelect = false;
            gridClientes.Location = new Point(20, 100);
            gridClientes.Size = new Size(860, 320);
            gridClientes.Anchor =
                AnchorStyles.Top | AnchorStyles.Bottom |
                AnchorStyles.Left | AnchorStyles.Right;
            gridClientes.ReadOnly = true;
            gridClientes.AllowUserToAddRows = false;
            gridClientes.RowHeadersVisible = false;
            gridClientes.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            gridClientes.Columns.Add("Id", "Id");
            gridClientes.Columns["Id"].Visible = false;

            gridClientes.Columns.Add("Nome", "Nome");
            gridClientes.Columns.Add("CPF", "CPF");
            gridClientes.Columns.Add("Idade", "Idade");
            gridClientes.Columns.Add("Total", "Total em aberto");
            gridClientes.Columns["Total"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;

            btnAnterior.Text = "Anterior";
            btnAnterior.Location = new Point(330, 435);
            btnAnterior.Size = new Size(85, 28);
            btnAnterior.Anchor = AnchorStyles.Bottom;
            btnAnterior.Click += (s, e) =>
            {
                if (paginaAtual > 1)
                {
                    paginaAtual--;
                    CarregarClientes();
                }
            };

            lblPagina.Location = new Point(430, 441);
            lblPagina.Size = new Size(100, 20);
            lblPagina.TextAlign = ContentAlignment.MiddleCenter;
            lblPagina.Anchor = AnchorStyles.Bottom;

            btnProxima.Text = "Próxima";
            btnProxima.Location = new Point(545, 435);
            btnProxima.Size = new Size(85, 28);
            btnProxima.Anchor = AnchorStyles.Bottom;
            btnProxima.Click += (s, e) =>
            {
                paginaAtual++;
                CarregarClientes();
            };

            Controls.Add(lblTitulo);
            Controls.Add(txtBusca);
            Controls.Add(btnPesquisar);
            Controls.Add(btnNovo);
            Controls.Add(btnEditar);
            Controls.Add(btnExcluir);
            Controls.Add(btnDividas);
            Controls.Add(gridClientes);
            Controls.Add(btnAnterior);
            Controls.Add(lblPagina);
            Controls.Add(btnProxima);

            CarregarClientes();
        }

        private void PesquisarClientes()
        {
            paginaAtual = 1;
            CarregarClientes();
        }

        private void CarregarClientes()
        {
            gridClientes.Rows.Clear();

            var pesquisa = txtBusca.Text.Trim();
            var clientes = clienteService.Consultar();

            var clientesOrdenados = clientes
                .Where(cliente =>
                    string.IsNullOrWhiteSpace(pesquisa) ||
                    cliente.nome.Contains(
                        pesquisa,
                        StringComparison.OrdinalIgnoreCase))
                .Select(cliente => new
                {
                    Cliente = cliente,
                    TotalEmAberto = dividasService
                        .Consultar(cliente.id)
                        .Where(divida => !divida.Paga)
                        .Sum(divida => divida.Valor)
                })
                .OrderByDescending(item => item.TotalEmAberto)
                .ThenBy(
                    item => item.Cliente.nome,
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

            int totalPaginas = Math.Max(
                1,
                (int)Math.Ceiling(
                    clientesOrdenados.Count / (double)TamanhoPagina));

            if (paginaAtual > totalPaginas)
            {
                paginaAtual = totalPaginas;
            }

            var clientesDaPagina = clientesOrdenados
                .Skip((paginaAtual - 1) * TamanhoPagina)
                .Take(TamanhoPagina);

            var cultura = CultureInfo.GetCultureInfo("pt-BR");

            foreach (var item in clientesDaPagina)
            {
                gridClientes.Rows.Add(
                    item.Cliente.id,
                    item.Cliente.nome,
                    item.Cliente.cpf,
                    item.Cliente.Idade,
                    item.TotalEmAberto.ToString("C2", cultura));
            }

            lblPagina.Text = $"Página {paginaAtual} de {totalPaginas}";
            btnAnterior.Enabled = paginaAtual > 1;
            btnProxima.Enabled = paginaAtual < totalPaginas;
        }

        private int? ObterIdSelecionado()
        {
            if (gridClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um cliente na tabela.");
                return null;
            }

            return Convert.ToInt32(
                gridClientes.SelectedRows[0].Cells["Id"].Value);
        }

        private void EditarCliente()
        {
            var id = ObterIdSelecionado();

            if (id == null)
            {
                return;
            }

            var cliente = clienteService
                .Consultar()
                .FirstOrDefault(c => c.id == id.Value);

            if (cliente == null)
            {
                MessageBox.Show("Cliente não encontrado.");
                return;
            }

            using var formulario = new ClienteForm(clienteService, cliente);

            if (formulario.ShowDialog(this) == DialogResult.OK)
            {
                CarregarClientes();
            }
        }

        private void ExcluirCliente()
        {
            var id = ObterIdSelecionado();

            if (id == null)
            {
                return;
            }

            var confirmacao = MessageBox.Show(
                "Deseja excluir este cliente?",
                "Confirmar exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacao != DialogResult.Yes)
            {
                return;
            }

            try
            {
                if (clienteService.Excluir(id.Value))
                {
                    CarregarClientes();
                }
                else
                {
                    MessageBox.Show("Cliente não encontrado.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Não foi possível excluir. O cliente pode ter dívidas vinculadas.\n\n" +
                    ex.GetBaseException().Message);
            }
        }

        private void AbrirDividas()
        {
            var id = ObterIdSelecionado();

            if (id == null)
            {
                return;
            }

            var cliente = clienteService
                .Consultar()
                .FirstOrDefault(c => c.id == id.Value);

            if (cliente == null)
            {
                MessageBox.Show("Cliente não encontrado.");
                return;
            }

            using var formulario = new DividasForm(dividasService, cliente);
            formulario.ShowDialog(this);

            CarregarClientes();
        }
    }
}