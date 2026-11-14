using MercadinhoDuZeh.Classes.Context;
using MercadinhoDuZeh.Classes.Models;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MercadinhoDuZeh.Classes.Services
{
    public class ClienteService
    {
        private readonly MercadinhoDbContext context;

        public ClienteService(MercadinhoDbContext context)
        {
            this.context = context;
        }

        public List<Cliente> Consultar()
        {
            return context.Clientes
                .OrderBy(cliente => cliente.nome)
                .ToList();
        }

        public bool Criar(Cliente cliente, out List<ValidationResult> erros)
        {
            if (!Validar(cliente, out erros))
            {
                return false;
            }

            context.Add(cliente);
            context.SaveChanges();
            return true;
        }

        public bool Validar(Cliente cliente, out List<ValidationResult> erros)
        {
            var validationContext = new ValidationContext(cliente);
            erros = new List<ValidationResult>();

            return Validator.TryValidateObject(
                cliente, validationContext, erros, true);
        }
    }
}
