using MercadinhoDuZeh.Classes.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MercadinhoDuZeh.Classes.Context
{
    public class MercadinhoDbContext : DbContext
    {
        public MercadinhoDbContext(DbContextOptions opt) : base(opt)
        {

        }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Dividas> Dividas => Set<Dividas>();
    }
}
