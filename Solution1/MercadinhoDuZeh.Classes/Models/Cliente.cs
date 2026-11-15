using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MercadinhoDuZeh.Classes.Models
{
    public class Cliente
    {
        [Required]
        public int id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        [RegularExpression(@"^[\p{L}\p{M}]+([ '\-][\p{L}\p{M}]+)*$",ErrorMessage = "Informe um nome válido.")]
        public string nome { get; set; }

        private string _cpf = string.Empty;

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter apenas números.")]
        public string cpf
        {
            get => _cpf;
            set => _cpf = new string(
                (value ?? string.Empty)
                    .Where(char.IsDigit)
                    .ToArray());
        }

        [EmailAddress]
        public string? email { get; set; }

        public DateTime DataNascimento { get; set; }


        public int Idade
        {
            get
            {
                var hoje = DateTime.Today;
                var idade = hoje.Year - DataNascimento.Year;
                var hojeAnoNascimento = hoje.AddYears(-idade);
                if (DataNascimento > hojeAnoNascimento)
                {
                    idade--;
                }
                return idade;

            }
        }


    }
}
