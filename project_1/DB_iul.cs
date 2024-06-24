using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace project_1
{
    public class DB_iul:DbContext
    {

        private const string TABLE_NAME = "iulHistory";
        private const string CONNECTION_DATA = "Host=LocalHost;Port=5432;Database=all;Username=artemchik;Password=123";
               
        private NpgsqlConnection connection = null!;
        public DbSet<iulData> iul { get; set; } = null!;


        public DB_iul ()
        {
            connection = new NpgsqlConnection(CONNECTION_DATA);
            connection.Open();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(CONNECTION_DATA);
        }


        public void Add(iulData iul) // Добавляем все
        {
            this.iul.Add(iul);
            this.SaveChanges();
        }

        public void DeleteID(int id) // Поиск по id
        {
            foreach (var iul in iul)
            {
                if (iul.id == id)
                {
                    this.iul.Remove(iul);
                    this.SaveChanges();
                }
            }
        }

        public void Clear() // Удаляем все
        {
        
            foreach (var iul in iul)
            {
                this.iul.Remove(iul);
                this.SaveChanges();
               
            }
        }


        public iulData GetID(int id) // Поиск по id
        {
            foreach (var iul in iul)
            {
                if (iul.id == id)
                {
                    return iul;
                }
            }
            return null;
        }

    }
}

