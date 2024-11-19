using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Models.ViewModel;
using LibraryApp.Data;
using Microsoft.AspNetCore.Identity;

namespace LibraryApp.Models;

public partial class S22024Group4ProjectContext : DbContext
{
    public S22024Group4ProjectContext()
    {
    }

    public S22024Group4ProjectContext(DbContextOptions<S22024Group4ProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookAuthor> BookAuthors { get; set; }

    public virtual DbSet<BookCopy> BookCopies { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<BookInventory> BookInventories { get; set; }
    public virtual DbSet<Borrow_Record> Borrow_Records { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("Author");

            entity.Property(e => e.FirstName).HasMaxLength(20);
            entity.Property(e => e.LastName).HasMaxLength(20);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Book");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Publisher).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.Books)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Book_Category");
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity
                .HasKey(e => new { e.BookId, e.AuthorId }); // Set composite primary key

            entity
                .ToTable("BookAuthor");

            entity.HasOne(d => d.Author).WithMany()
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookAuthor_Author");

            entity.HasOne(d => d.Book).WithMany()
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookAuthor_Book");
        });


        modelBuilder.Entity<BookCopy>(entity =>
        {
            entity.HasKey(e => e.ISBN);

            entity.ToTable("BookCopy");

            entity.Property(e => e.ISBN)
                .HasMaxLength(100)
                .HasColumnName("ISBN");
            entity.Property(e => e.Edition).HasMaxLength(50);

            entity.HasOne(d => d.Book).WithMany(p => p.BookCopies)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookCopy_Book");
        });
        modelBuilder.Entity<Borrow_Record>(entity =>
        {
            entity.HasKey(e => e.RecordNumber);

            entity.ToTable("Borrow_Record");

            entity.Property(e => e.AmountPaid).HasColumnType("money");

            entity.Property(e => e.DateBorrowed).HasColumnType("datetime");

            entity.Property(e => e.DueDate).HasColumnType("datetime");

            entity.HasOne(d => d.Copy)
                .WithMany(p => p.Borrow_Records)
                .HasForeignKey(d => d.Isbn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Borrow_Record_BookCopy");

            entity.Property(e => e.LateFee).HasColumnType("money");

            entity.Property(e => e.OutstandingFee).HasColumnType("money");

            entity.Property(e => e.ReturnedDate).HasColumnType("datetime");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Borrow_Records)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Borrow_Record_StudentUsers");

        });


        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).ValueGeneratedNever();
            entity.Property(e => e.CategoryName).HasMaxLength(50);
        });

  


        // Seed roles

        modelBuilder.Entity<IdentityRole>().HasData(

            new IdentityRole

            {

                Id = "1",

                Name = "Administrator",

                NormalizedName = "ADMINISTRATOR",

                ConcurrencyStamp = Guid.NewGuid().ToString()

            },

            new IdentityRole

            {

                Id = "2",

                Name = "Staff",

                NormalizedName = "STAFF",

                ConcurrencyStamp = Guid.NewGuid().ToString()

            },

            new IdentityRole

            {

                Id = "3",

                Name = "Student",

                NormalizedName = "STUDENT",

                ConcurrencyStamp = Guid.NewGuid().ToString()

            }



        );
        OnModelCreatingPartial(modelBuilder);
        //End
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<LibraryApp.Models.ViewModel.BookInventory> BookInventory { get; set; } = default!;
}
