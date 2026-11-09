using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MercadinhoDuZeh.Classes.Models
{
    internal class Cliente
    {
        [Required, StringLength(10)]
        public string id { get; set; }

        [Required]
        [StringLength(80, MinimumLength = 6)]
        [RegularExpression("^[a-zA-Z ]+$")]
        public string nome { get; set; }

        private string _cpf = string.Empty;

        private string cpf
        {
            get => _cpf;
            set => _cpf = value.Replace(".", "").Replace("-", "");
        }

        [EmailAddress]
        private string email { get; set; }

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
