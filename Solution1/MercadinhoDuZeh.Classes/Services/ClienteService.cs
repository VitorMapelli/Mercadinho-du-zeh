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

        public bool Atualizar(Cliente cliente, out List<ValidationResult> erros)
        {
            if (!Validar(cliente, out erros))
            {
                return false;
            }

            var clienteExistente = context.Clientes.Find(cliente.id);

            if (clienteExistente == null)
            {
                erros.Add(new ValidationResult("Cliente não encontrado."));
                return false;
            }

            clienteExistente.nome = cliente.nome;
            clienteExistente.cpf = cliente.cpf;
            clienteExistente.email = cliente.email;
            clienteExistente.DataNascimento = cliente.DataNascimento;

            context.SaveChanges();
            return true;
        }

        public bool Excluir(int id)
        {
            var cliente = context.Clientes.Find(id);

            if (cliente == null)
            {
                return false;
            }

            context.Clientes.Remove(cliente);
            context.SaveChanges();
            return true;
        }
    }
}
