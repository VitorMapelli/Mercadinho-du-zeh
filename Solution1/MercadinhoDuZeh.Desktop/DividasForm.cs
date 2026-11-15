using MercadinhoDuZeh.Classes.Models;
using MercadinhoDuZeh.Classes.Services;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace MercadinhoDuZeh.Desktop
{
    public partial class DividasForm : Form
    {
        private readonly DividasService dividasService;
        private readonly Cliente cliente;

        private readonly DataGridView gridDividas = new DataGridView();
        private readonly NumericUpDown nudValor = new NumericUpDown();
        private readonly Button btnNova = new Button();
        private readonly Button btnPagar = new Button();

        public DividasForm(DividasService dividasService, Cliente cliente)
        {
            this.dividasService = dividasService;
            this.cliente = cliente;

            Text = "Dívidas do cliente";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(700, 450);

            var lblCliente = new Label
            {
                Text = $"Cliente: {cliente.nome}",
                Location = new Point(20, 15),
                AutoSize = true
            };

            gridDividas.Location = new Point(20, 50);
            gridDividas.Size = new Size(660, 280);
            gridDividas.ReadOnly = true;
            gridDividas.AllowUserToAddRows = false;
            gridDividas.MultiSelect = false;
            gridDividas.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            gridDividas.RowHeadersVisible = false;
            gridDividas.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            gridDividas.Columns.Add("Id", "Id");
            gridDividas.Columns["Id"].Visible = false;

            gridDividas.Columns.Add("Valor", "Valor");
            gridDividas.Columns["Valor"].MinimumWidth = 100;
            gridDividas.Columns["Valor"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;

            gridDividas.Columns.Add("Situacao", "Situação");
            gridDividas.Columns.Add("Criacao", "Data de criação");
            gridDividas.Columns.Add("Pagamento", "Data de pagamento");

            gridDividas.SelectionChanged += (s, e) => AtualizarBotaoPagar();

            var lblValor = new Label
            {
                Text = "Valor da nova dívida:",
                Location = new Point(20, 350),
                AutoSize = true
            };

            nudValor.Location = new Point(160, 347);
            nudValor.Width = 120;
            nudValor.DecimalPlaces = 2;
            nudValor.Minimum = 0.01m;
            nudValor.Maximum = 9999999;
            nudValor.Increment = 1;

            btnNova.Text = "Criar dívida";
            btnNova.Location = new Point(300, 345);
            btnNova.Size = new Size(110, 30);
            btnNova.Click += (s, e) => CriarDivida();

            btnPagar.Text = "Marcar como paga";
            btnPagar.Location = new Point(420, 345);
            btnPagar.Size = new Size(130, 30);
            btnPagar.Click += (s, e) => PagarDivida();

            var btnFechar = new Button
            {
                Text = "Fechar",
                Location = new Point(570, 345),
                Size = new Size(90, 30),
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(lblCliente);
            Controls.Add(gridDividas);
            Controls.Add(lblValor);
            Controls.Add(nudValor);
            Controls.Add(btnNova);
            Controls.Add(btnPagar);
            Controls.Add(btnFechar);

            CancelButton = btnFechar;

            CarregarDividas();
        }

        private void CarregarDividas()
        {
            var dividas = dividasService.Consultar(cliente.id);
            var cultura = CultureInfo.GetCultureInfo("pt-BR");

            gridDividas.Rows.Clear();

            foreach (var divida in dividas)
            {
                gridDividas.Rows.Add(
                    divida.Id,
                    divida.Valor.ToString("C2", cultura),
                    divida.Paga ? "Paga" : "Em aberto",
                    divida.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                    divida.DataPagamento?.ToString("dd/MM/yyyy HH:mm") ?? "-");
            }

            btnNova.Enabled = !dividas.Any(divida => !divida.Paga);

            AtualizarBotaoPagar();
        }

        private void AtualizarBotaoPagar()
        {
            btnPagar.Enabled =
                gridDividas.SelectedRows.Count > 0 &&
                gridDividas.SelectedRows[0].Cells["Situacao"].Value?.ToString()
                    == "Em aberto";
        }

        private void CriarDivida()
        {
            var divida = new Dividas
            {
                ClienteId = cliente.id,
                Valor = nudValor.Value
            };

            try
            {
                if (!dividasService.Criar(divida, out var erros))
                {
                    var mensagem = string.Join(
                        Environment.NewLine,
                        erros.Select(erro => erro.ErrorMessage));

                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(mensagem)
                            ? "Não foi possível criar a dívida."
                            : mensagem);

                    return;
                }

                nudValor.Value = nudValor.Minimum;
                CarregarDividas();
                MessageBox.Show("Dívida criada.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao criar a dívida: {ex.GetBaseException().Message}");
            }
        }

        private void PagarDivida()
        {
            if (gridDividas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione uma dívida em aberto.");
                return;
            }

            var confirmacao = MessageBox.Show(
                "Confirma o pagamento desta dívida?",
                "Confirmar pagamento",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacao != DialogResult.Yes)
            {
                return;
            }

            int id = Convert.ToInt32(
                gridDividas.SelectedRows[0].Cells["Id"].Value);

            try
            {
                if (!dividasService.Pagar(id, out var erros))
                {
                    var mensagem = string.Join(
                        Environment.NewLine,
                        erros.Select(erro => erro.ErrorMessage));

                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(mensagem)
                            ? "Não foi possível pagar a dívida."
                            : mensagem);

                    return;
                }

                CarregarDividas();
                MessageBox.Show("Pagamento registrado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao pagar a dívida: {ex.GetBaseException().Message}");
            }
        }
    }
}