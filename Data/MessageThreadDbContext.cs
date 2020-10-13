using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SVV.MessagingApp.Data
{
    public class MessageThreadDbContext : DbContext
    {
        public MessageThreadDbContext()
        {
            connectionString = "Data Source=messaging.db";
        }

        public MessageThreadDbContext(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        private string connectionString;

        public DbSet<MessageThread> MessageThreads { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MessageThread>()
                .HasMany(t => t.Messages)
                .WithOne();
        }
    }
}