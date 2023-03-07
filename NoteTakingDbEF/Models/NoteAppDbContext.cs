using Microsoft.EntityFrameworkCore;
using Utils;

namespace NoteTakingDbEF.Models;

public partial class NoteAppDbContext : DbContext
{
    public NoteAppDbContext()
    {
    }

    public NoteAppDbContext(DbContextOptions<NoteAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Note> Notes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = UtilityFunctions.GetConnectionString();
            optionsBuilder.UseSqlServer(connectionString);
        }
    }       

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Note__3214EC07CBF2D520");

            entity.ToTable("Note");

            entity.Property(e => e.Date).HasColumnType("date");
            entity.Property(e => e.Note1)
                .IsUnicode(false)
                .HasColumnName("Note");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
