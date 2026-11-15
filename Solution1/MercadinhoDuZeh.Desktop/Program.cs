using MercadinhoDuZeh.Classes.Context;
using MercadinhoDuZeh.Classes.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Windows.Forms;

namespace MercadinhoDuZeh.Desktop
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();

            var connectionString =
                configuration.GetConnectionString("Postgres")
                ?? throw new InvalidOperationException(
                    "Connection string Postgres não encontrada.");

            var options = new DbContextOptionsBuilder<MercadinhoDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            using var context = new MercadinhoDbContext(options);

            var clienteService = new ClienteService(context);
            var dividasService = new DividasService(context);

            Application.Run(
                new Form1(clienteService, dividasService));
        }
    }
}