using MercadinhoDuZeh.Classes.Models;
using MercadinhoDuZeh.Classes.Services;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MercadinhoDuZeh.Desktop
{
    public partial class ClienteForm : Form
    {
        private readonly ClienteService clienteService;

        private readonly TextBox txtNome = new TextBox();
        private readonly TextBox txtCpf = new TextBox();
        private readonly TextBox txtEmail = new TextBox();
        private readonly DateTimePicker dtpNascimento = new DateTimePicker();

        public ClienteForm(ClienteService clienteService)
        {
            this.clienteService = clienteService;

            Text = "Novo cliente";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(400, 260);

            Controls.Add(new Label
            {
                Text = "Nome:",
                Location = new Point(20, 25),
                AutoSize = true
            });
            txtNome.Location = new Point(140, 22);
            txtNome.Width = 220;

            Controls.Add(new Label
            {
                Text = "CPF:",
                Location = new Point(20, 65),
                AutoSize = true
            });
            txtCpf.Location = new Point(140, 62);
            txtCpf.Width = 220;

            Controls.Add(new Label
            {
                Text = "Nascimento:",
                Location = new Point(20, 105),
                AutoSize = true
            });
            dtpNascimento.Location = new Point(140, 102);
            dtpNascimento.Width = 220;
            dtpNascimento.MaxDate = DateTime.Today;
            dtpNascimento.ShowCheckBox = true;
            dtpNascimento.Checked = false;

            Controls.Add(new Label
            {
                Text = "E-mail:",
                Location = new Point(20, 145),
                AutoSize = true
            });
            txtEmail.Location = new Point(140, 142);
            txtEmail.Width = 220;

            var btnSalvar = new Button
            {
                Text = "Salvar",
                Location = new Point(140, 195),
                Size = new Size(100, 30)
            };
            btnSalvar.Click += SalvarCliente;

            var btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(250, 195),
                Size = new Size(100, 30),
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(txtNome);
            Controls.Add(txtCpf);
            Controls.Add(txtEmail);
            Controls.Add(dtpNascimento);
            Controls.Add(btnSalvar);
            Controls.Add(btnCancelar);

            AcceptButton = btnSalvar;
            CancelButton = btnCancelar;
        }

        private void SalvarCliente(object sender, EventArgs e)
        {
            if (!dtpNascimento.Checked)
            {
                MessageBox.Show("Informe a data de nascimento.");
                return;
            }

            var cliente = new Cliente
            {
                nome = txtNome.Text.Trim(),
                cpf = txtCpf.Text.Trim(),
                email = txtEmail.Text.Trim(),
                DataNascimento = dtpNascimento.Value.Date
            };

            try
            {
                if (!clienteService.Criar(cliente, out var erros))
                {
                    var mensagem = string.Join(
                        Environment.NewLine,
                        erros.Select(erro => erro.ErrorMessage));

                    MessageBox.Show(mensagem);
                    return;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Não foi possível salvar o cliente: {ex.GetBaseException().Message}");
            }
        }
    }
}