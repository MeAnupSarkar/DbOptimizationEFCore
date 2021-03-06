using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DBOptimizedDotNet.Models;
using System.Collections.Generic;
using DBOptimizedDotNet.Models.Entity;

#nullable disable

namespace DBOptimizedDotNet.Context
{
    public partial class AppDbContext : DbContext
    {
        private string query;
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<UserAccount> UserAccounts { get; set; }
        public virtual DbSet<Leader> Leaders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                // optionsBuilder.UseSqlServer("=DESKTOP-BN3GLN3;Database=TestDB;Trusted_Connection=True;MultipleActiveResultSets=true");
                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXP2019;Initial Catalog=DBOptimizations;Integrated Security=True;MultipleActiveResultSets=true");

               // optionsBuilder.UseSqlServer("Data Source=192.168.8.111;Initial Catalog=test;Persist Security Info=True;User ID=sa;Password=HPL123&*(;MultipleActiveResultSets=True");



            }
        }

        public AppDbContext Query(string q)
        {

            this.query = q;
            return this;
        }

        public List<List<RowMetaData>> ToListDynamic()
        {
            this.Database.GetDbConnection().Open();
            var cmd = this.Database.GetDbConnection().CreateCommand();   
            cmd.CommandText = query;

            var reader = cmd.ExecuteReader();
            var rows = new List<List<RowMetaData>>();

            while (reader.Read())
            {
                var row = new List<RowMetaData>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var key = reader.GetName(i);
                    //var type = reader.GetType();
                    var value = reader.GetValue(i);

                    row.Add(new RowMetaData(key, value)); ;
                }
                rows.Add(row);
                row = null;
            }
            return rows;
        }



        public List<Dictionary<string,object>> ToDynamicList()
        {

            this.Database.GetDbConnection().Open();
            var cmd = this.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = query;

            var reader = cmd.ExecuteReader();
            var rows = new List<Dictionary<string, object>>();

            while (reader.Read())
            {
                var row =new Dictionary<string, object>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var key = reader.GetName(i);
                    //var type = reader.GetType();
                    var value = reader.GetValue(i);
                    row.Add(key, value);

                }

                rows.Add(row);
                row = null;

            }

            return rows;

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.Isbn).HasColumnName("ISBN");
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.HasKey(e => e.Uid);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
