using MercadinhoDuZeh.Classes.Context;
using MercadinhoDuZeh.Classes.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

            try
            {
                context.SaveChanges();
                return true;
            }
            catch
            {
                context.Entry(cliente).State = EntityState.Detached;
                throw;
            }
        }

        public bool Validar(Cliente cliente, out List<ValidationResult> erros)
        {
            erros = new List<ValidationResult>();

            // limpa tudo que não for numero do cpf antes de validar
            string cpf = new string(
                (cliente.cpf ?? string.Empty)
                    .Where(char.IsDigit)
                    .ToArray());

            cliente.cpf = cpf;

            var validationContext = new ValidationContext(cliente);

            bool valido = Validator.TryValidateObject(
                cliente,
                validationContext,
                erros,
                validateAllProperties: true);

            if (!CpfValido(cpf))
            {
                erros.Add(new ValidationResult("Informe um CPF válido."));
            }
            else
            {
                bool cpfJaCadastrado = context.Clientes.Any(c =>
                    c.cpf == cpf &&
                    c.id != cliente.id);

                if (cpfJaCadastrado)
                {
                    erros.Add(new ValidationResult(
                        "Já existe um cliente cadastrado com este CPF."));
                }
            }

            if (cliente.DataNascimento == default)
            {
                erros.Add(new ValidationResult(
                    "Informe a data de nascimento."));
            }
            else if (cliente.DataNascimento.Date > DateTime.Today)
            {
                erros.Add(new ValidationResult(
                    "A data de nascimento não pode ser futura."));
            }

            return valido && erros.Count == 0;
        }

        private static bool CpfValido(string cpf)
        {
            if (cpf.Length != 11)
            {
                return false;
            }

            // não pode por 11111111111 ou 22222222222 etc
            if (cpf.Distinct().Count() == 1)
            {
                return false;
            }

            int soma = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += (cpf[i] - '0') * (10 - i);
            }

            int resto = soma % 11;
            int primeiroDigito = resto < 2 ? 0 : 11 - resto;

            if (primeiroDigito != cpf[9] - '0')
            {
                return false;
            }

            soma = 0;

            for (int i = 0; i < 10; i++)
            {
                soma += (cpf[i] - '0') * (11 - i);
            }

            resto = soma % 11;
            int segundoDigito = resto < 2 ? 0 : 11 - resto;

            return segundoDigito == cpf[10] - '0';
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
