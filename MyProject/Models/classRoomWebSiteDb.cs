using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace MyProject.Models
{
    public class classRoomWebSiteDBContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<calendar_detail>().ToTable("calendar_detail");
            modelBuilder.Entity<cat1>().ToTable("cat1");
            modelBuilder.Entity<cat2>().ToTable("cat2");
            modelBuilder.Entity<courseIntro>().ToTable("courseIntro");
            modelBuilder.Entity<QuestionAnser>().ToTable("QuestionAnser");
            modelBuilder.Entity<student>().ToTable("student");
            modelBuilder.Entity<SystemUser>().ToTable("SystemUser");
            modelBuilder.Entity<TaiwanZipCode>().ToTable("TaiwanZipCode");
            modelBuilder.Entity<teacher>().ToTable("teacher");
        }

        public DbSet<calendar_detail> calendar_detail { get; set; }

        public DbSet<cat1> cat1 { get; set; }

        public DbSet<cat2> cat2 { get; set; }

        public DbSet<courseIntro> courseIntro { get; set; }

        public DbSet<QuestionAnser> QuestionAnser { get; set; }

        public DbSet<student> student { get; set; }

        public DbSet<SystemUser> SystemUser { get; set; }

        public DbSet<TaiwanZipCode> TaiwanZipCode { get; set; }

        public DbSet<teacher> teacher { get; set; }

    }
}
 
