using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace project_1
{
    public class DB_users : DbContext
    {
        private const string TABLE_NAME = "users";
        private const string CONNECTION_DATA = "Host=LocalHost;Port=5432;Database=all;Username=artemchik;Password=123";

        private NpgsqlConnection connection = null!;
        public DbSet<User> users { get; set; } = null!;

        public DB_users()
        {
            connection = new NpgsqlConnection(CONNECTION_DATA);
            connection.Open();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(CONNECTION_DATA);
        


        }
  
        public void Add(User user) // Добавляем все
        {

            this.users.Add(user);
            this.SaveChanges();
        }

        public void DeleteID(int id) // Поиск по id
        {
            foreach (var user in users)
            {
                if (user.id == id)
                {
                    this.users.Remove(user); 
                }
            }
             this.SaveChanges();
        }


        public void Clear()  // Удаляем все
        {
            foreach (var user in users)
            {
                this.users.Remove(user);
            }
             this.SaveChanges();
        }


        public User GetID(int id) // Поиск по id
        {
            foreach (var user in users)
            {
                if (user.id == id)
                {
                    return user;
                }
            }
            return null;
        }
    }
}