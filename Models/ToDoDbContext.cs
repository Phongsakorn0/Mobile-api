using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ToDo.Models
{
    public partial class ToDoDbContext : DbContext
    {
        public ToDoDbContext()
        {
        }

        public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Activity> Activity { get; set; } = null!;
        public virtual DbSet<User> User { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;port=3306;user=username;password=password;database=ToDo", Microsoft.EntityFrameworkCore.ServerVersion.Parse("11.7.2-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_uca1400_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.HasIndex(e => e.Userid, "FK_Activity_Userid");

                entity.Property(e => e.Id).HasColumnType("int(10)");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Userid).HasColumnType("int(10)");

                entity.Property(e => e.When).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Activity)
                    .HasForeignKey(d => d.Userid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Activity_Userid");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(10)");

                entity.Property(e => e.Firstname).HasMaxLength(100);

                entity.Property(e => e.HashedPassword)
                    .HasMaxLength(44)
                    .IsFixedLength();

                entity.Property(e => e.Lastname).HasMaxLength(100);

                entity.Property(e => e.NationalId)
                    .HasMaxLength(13)
                    .IsFixedLength();

                entity.Property(e => e.Salt)
                    .HasMaxLength(24)
                    .IsFixedLength();

                entity.Property(e => e.Tittle).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
