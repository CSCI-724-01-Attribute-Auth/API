using API.Models;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace API.Data
{

    public class DBContext: DbContext
    {
        public DBContext() { }

        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public virtual DbSet<Movie> Movies { get; set; }

        public virtual DbSet<Person> Persons { get; set; }

        public virtual DbSet<CrewMember> CrewMembers { get; set; }

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
        }
    }
}
