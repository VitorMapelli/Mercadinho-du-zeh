using MercadinhoDuZeh.Classes.Context;
using MercadinhoDuZeh.Classes.Models;
using System.ComponentModel.DataAnnotations;

namespace MercadinhoDuZeh.Classes.Services
{
    public class DividasService
    {
        private readonly MercadinhoDbContext context;

        public DividasService(MercadinhoDbContext context)
        {
            this.context = context;
        }

        public List<Dividas> Consultar(int clienteId)
        {
            return context.Dividas
                .Where(divida => divida.ClienteId == clienteId)
                .OrderByDescending(divida => divida.DataCriacao)
                .ToList();
        }

        public bool Criar(Dividas divida, out List<ValidationResult> erros)
        {
            erros = new List<ValidationResult>();

            if (divida.Valor <= 0)
            {
                erros.Add(new ValidationResult("O valor da dívida deve ser maior que zero."));
                return false;
            }

            bool clienteExiste = context.Clientes
                .Any(cliente => cliente.id == divida.ClienteId);

            if (!clienteExiste)
            {
                erros.Add(new ValidationResult("Cliente não encontrado."));
                return false;
            }

            bool possuiDividaAberta = context.Dividas
                .Any(atual =>
                    atual.ClienteId == divida.ClienteId &&
                    !atual.Paga);

            if (possuiDividaAberta)
            {
                erros.Add(new ValidationResult(
                    "Este cliente já possui uma dívida em aberto."));
                return false;
            }

            divida.Paga = false;
            divida.DataCriacao =
                DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            divida.DataPagamento = null;

            if (!Validar(divida, out erros))
            {
                return false;
            }

            context.Add(divida);
            context.SaveChanges();
            return true;
        }

        public bool Pagar(int id, out List<ValidationResult> erros)
        {
            erros = new List<ValidationResult>();

            var divida = context.Dividas.Find(id);

            if (divida == null)
            {
                erros.Add(new ValidationResult("Dívida não encontrada."));
                return false;
            }

            if (divida.Paga)
            {
                erros.Add(new ValidationResult("Esta dívida já está paga."));
                return false;
            }

            divida.Paga = true;
            divida.DataPagamento =
                DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            context.SaveChanges();
            return true;
        }

        public bool Validar(
            Dividas divida,
            out List<ValidationResult> erros)
        {
            var validationContext = new ValidationContext(divida);
            erros = new List<ValidationResult>();

            return Validator.TryValidateObject(
                divida, validationContext, erros, true);
        }
    }
}