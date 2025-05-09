﻿using API.Models;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace API.Data
{

    public class DBContext : DbContext
    {
        public DBContext() { }

        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public virtual DbSet<Movie> Movies { get; set; }

        public virtual DbSet<Person> Persons { get; set; }

        public virtual DbSet<CrewMember> CrewMembers { get; set; }

        public virtual DbSet<AuthorizedAttributes> AuthorizedAttributes { get; set; }

        public virtual DbSet<AuthorizedAttributesByRole> AuthorizedAttributesByRole { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ArgumentNullException.ThrowIfNull(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=csci724project;Initial Catalog=db-local;User Id=admin;Password=password;TrustServerCertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            modelBuilder.Entity<TableHashResult>().HasNoKey();

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.ToTable(nameof(Movie));

                entity.HasKey(e => e.Id)
                    .HasName("PK_Movie");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .IsRequired();

                entity.Property(e => e.Title)
                    .HasColumnType("varchar(255)")
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(1000)")
                    .IsRequired();

                entity.Property(e => e.TotalBudget)
                    .HasColumnType("decimal(25,2)")
                    .IsRequired();

                entity.Property(e => e.TotalCost)
                    .HasColumnType("decimal(25,2)")
                    .IsRequired();

                entity.Property(e => e.ReleaseDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.HasMany(e => e.Crew)
                    .WithMany(e => e.WorkedOn)
                    .UsingEntity<CrewMember>(
                        l => l.HasOne<Person>().WithMany().HasForeignKey(e => e.PersonId),
                        r => r.HasOne<Movie>().WithMany().HasForeignKey(e => e.MovieId)
                    );
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable(nameof(User));

                entity.HasKey(e => e.UserId)
                    .HasName("PK_User");

                entity.Property(e => e.RoleId)
                    .HasColumnType("varchar(50)")
                    .IsRequired();

                entity.HasOne(e => e.Role)
                    .WithMany()
                    .HasForeignKey(e => e.RoleId)
                    .HasConstraintName("FK_User_RoleId")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable(nameof(Role));

                entity.HasKey(e => e.RoleId)
                    .HasName("PK_Role");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable(nameof(Person));

                entity.HasKey(e => e.Id)
                    .HasName("PK_Person");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .IsRequired();

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(255)")
                    .IsRequired();

                entity.Property(e => e.BirthDate)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(e => e.MostFamousMovieId)
                    .HasColumnType("int")
                    .IsRequired();

                entity.HasOne(e => e.MostFamousMovie)
                    .WithMany()
                    .HasForeignKey(e => e.MostFamousMovieId)
                    .HasConstraintName("FK_MostFamousMovie_Movie")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CrewMember>(entity =>
            {
                entity.ToTable(nameof(CrewMember));

                entity.HasKey(e => new { e.PersonId, e.MovieId })
                    .HasName("PK_CrewMember");

                entity.Property(e => e.PersonId)
                    .HasColumnType("int")
                    .IsRequired();

                entity.Property(e => e.MovieId)
                    .HasColumnType("int")
                    .IsRequired();
            });

            modelBuilder.Entity<AuthorizedAttributes>(entity =>
            {
                entity.ToTable(nameof(AuthorizedAttributes));

                entity.HasKey(e => new { e.ClientId, e.Method, e.Path });

                entity.Property(e => e.ClientId)
                    .HasColumnType("varchar(50)")
                    .IsRequired();

                entity.Property(e => e.Method)
                    .HasColumnType("varchar(10)")
                    .IsRequired();

                entity.Property(e => e.Path)
                    .HasColumnType("varchar(255)")
                    .IsRequired();

                entity.Property(e => e.AttributeList)
                    .HasColumnType("json")
                    .IsRequired();
            });

            modelBuilder.Entity<AuthorizedAttributesByRole>(entity =>
            {
                entity.ToTable(nameof(AuthorizedAttributesByRole));

                entity.HasKey(e => new { e.RoleId, e.Method, e.Path })
                    .HasName("PK_AuthorizedAttributesByRole");

                entity.Property(e => e.RoleId)
                    .HasColumnType("varchar(50)")
                    .IsRequired();

                entity.Property(e => e.Method)
                    .HasColumnType("varchar(10)")
                    .IsRequired();

                entity.Property(e => e.Path)
                    .HasColumnType("varchar(255)")
                    .IsRequired();

                entity.Property(e => e.AttributeList)
                    .HasColumnType("json")
                    .IsRequired();

                entity.HasOne(e => e.Role)
                    .WithMany()
                    .HasForeignKey(e => e.RoleId)
                    .HasConstraintName("FK_AuthorizedAttributesByRole_RoleId")
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
