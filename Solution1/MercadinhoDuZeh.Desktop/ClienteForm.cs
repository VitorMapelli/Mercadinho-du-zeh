using MercadinhoDuZeh.Classes.Models;
using MercadinhoDuZeh.Classes.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MercadinhoDuZeh.Desktop
{
    public partial class ClienteForm : Form
    {
        private readonly ClienteService clienteService;
        private readonly Cliente? clienteOriginal;

        private readonly TextBox txtNome = new TextBox();
        private readonly TextBox txtCpf = new TextBox();
        private readonly TextBox txtEmail = new TextBox();
        private readonly DateTimePicker dtpNascimento = new DateTimePicker();

        public ClienteForm(
            ClienteService clienteService,
            Cliente? cliente = null)
        {
            this.clienteService = clienteService;
            clienteOriginal = cliente;

            Text = cliente == null ? "Novo cliente" : "Editar cliente";
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
            txtNome.MaxLength = 50;

            Controls.Add(new Label
            {
                Text = "CPF:",
                Location = new Point(20, 65),
                AutoSize = true
            });

            txtCpf.Location = new Point(140, 62);
            txtCpf.Width = 220;
            txtCpf.MaxLength = 14;

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
            txtEmail.MaxLength = 50;

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

            if (cliente != null)
            {
                txtNome.Text = cliente.nome;
                txtCpf.Text = cliente.cpf;
                txtEmail.Text = cliente.email ?? string.Empty;
                dtpNascimento.Value = cliente.DataNascimento.Date;
                dtpNascimento.Checked = true;
            }
        }

        private void SalvarCliente(object? sender, EventArgs e)
        {
            if (!dtpNascimento.Checked)
            {
                MessageBox.Show("Informe a data de nascimento.");
                return;
            }

            var cliente = clienteOriginal ?? new Cliente();

            cliente.nome = txtNome.Text.Trim();
            cliente.cpf = txtCpf.Text.Trim();
            cliente.email = string.IsNullOrWhiteSpace(txtEmail.Text)
                ? null
                : txtEmail.Text.Trim();
            cliente.DataNascimento = dtpNascimento.Value.Date;

            try
            {
                bool sucesso;
                List<ValidationResult> erros;

                if (clienteOriginal == null)
                {
                    sucesso = clienteService.Criar(cliente, out erros);
                }
                else
                {
                    sucesso = clienteService.Atualizar(cliente, out erros);
                }

                if (!sucesso)
                {
                    MessageBox.Show(string.Join(
                        Environment.NewLine,
                        erros.Select(erro => erro.ErrorMessage)));

                    return;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Não foi possível salvar: {ex.GetBaseException().Message}");
            }
        }
    }
}