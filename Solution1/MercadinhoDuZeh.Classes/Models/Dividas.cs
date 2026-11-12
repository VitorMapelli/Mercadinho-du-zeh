using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MercadinhoDuZeh.Classes.Models
{
    public class Dividas
    {
        [Required, StringLength(10)]
        public string Id { get; set; }

        [Required, StringLength(10)]
        public string ClienteId { get; set; }

        public decimal Valor { get; set; }

        public bool Paga { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataPagamento { get; set; }
    }
}
