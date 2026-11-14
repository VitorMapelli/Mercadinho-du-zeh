using MercadinhoDuZeh.Classes.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MercadinhoDuZeh.Classes.Context
{
    public class MercadinhoDbContext : DbContext
    {
        private string connectionString = string.Empty;

        public MercadinhoDbContext(string connString)
        {
            this.connectionString = connString;
        }

        public MercadinhoDbContext(DbContextOptions opt) : base(opt)
        {
        }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Dividas> Dividas => Set<Dividas>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entityCliente = modelBuilder.Entity<Cliente>();

            entityCliente.ToTable("clientes");
            entityCliente.HasKey(c => c.id);
            entityCliente.Property(c => c.id).HasColumnName("id");
            entityCliente.Property(c => c.nome).HasColumnName("nome");
            entityCliente.Property(c => c.cpf).HasColumnName("cpf");
            entityCliente.Property(c => c.email).HasColumnName("email");
            entityCliente.Property(c => c.DataNascimento).HasColumnName("data_nascimento").HasColumnType("date");

            var entityDivida = modelBuilder.Entity<Dividas>();

            entityDivida.ToTable("dividas");
            entityDivida.HasKey(d => d.Id);
            entityDivida.Property(d => d.Id).HasColumnName("id");
            entityDivida.Property(d => d.ClienteId).HasColumnName("cliente_id");
            entityDivida.Property(d => d.Valor).HasColumnName("valor");
            entityDivida.Property(d => d.Paga).HasColumnName("paga");
            entityDivida.Property(d => d.DataCriacao).HasColumnName("data_criacao");
            entityDivida.Property(d => d.DataPagamento).HasColumnName("data_pagamento");

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseNpgsql(connectionString)
                    .LogTo(message => Debug.WriteLine(message))
                    .EnableDetailedErrors();
            }

            base.OnConfiguring(optionsBuilder);
        }
    }
}