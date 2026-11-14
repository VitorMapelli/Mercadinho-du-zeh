using MercadinhoDuZeh.Classes.Context;
using MercadinhoDuZeh.Classes.Models;
using MercadinhoDuZeh.Classes.Services;
using Microsoft.EntityFrameworkCore;
namespace MercadinhoDuZeh.Desktop
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
            var connectionString =
            "(connection string do db)";

                    var options = new DbContextOptionsBuilder<MercadinhoDbContext>()
                        .UseNpgsql(connectionString)
                        .Options;

                    try
                    {
                        using var context = new MercadinhoDbContext(options);

                        var clienteService = new ClienteService(context);
                        var clientes = clienteService.Consultar();

                        var resultado = string.Join(
                            Environment.NewLine,
                            clientes.Select(cliente =>
                                $"{cliente.id} | {cliente.nome} | {cliente.cpf} | {cliente.email}")
                        );

                        MessageBox.Show(
                            string.IsNullOrEmpty(resultado) ? "Nenhum cliente cadastrado." : resultado
                        );

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao salvar: {ex.GetBaseException().Message}");
                        return;
                    }

                    // Application.Run(new Form1());
        }
    }
}