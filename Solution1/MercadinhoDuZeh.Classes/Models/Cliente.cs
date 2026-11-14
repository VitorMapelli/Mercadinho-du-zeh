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
        [RegularExpression("^[a-zA-Z ]+$")]
        public string nome { get; set; }

        private string _cpf = string.Empty;

        public string cpf
        {
            get => _cpf;
            set => _cpf = value.Replace(".", "").Replace("-", "");
        }

        [EmailAddress]
        public string email { get; set; }

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
