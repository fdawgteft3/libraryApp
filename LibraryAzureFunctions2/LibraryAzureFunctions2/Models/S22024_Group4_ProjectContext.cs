﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LibraryAzureFunctions2.Models
{
    public partial class S22024_Group4_ProjectContext : DbContext
    {
        public S22024_Group4_ProjectContext()
        {
        }

        public S22024_Group4_ProjectContext(DbContextOptions<S22024_Group4_ProjectContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BookCopy> BookCopies { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Borrow_Record> Borrow_Records { get; set; }
        public virtual DbSet<StudentUsers> StudentUserss { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=citizen.manukautech.info,6304;Database=S22024_Group4_Project;UID=S22024_Group4;PWD=fBit$96986;encrypt=true;trustservercertificate=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspNetUserRole",
                        l => l.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                        r => r.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");

                            j.ToTable("AspNetUserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("Author");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(20);
            });
            modelBuilder.Entity<StudentUsers>(entity =>
            {
                entity.ToTable("StudentUsers");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(256);
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.Password).HasMaxLength(256);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Book");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Publisher)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Book_Category");

                entity.HasMany(d => d.Authors)
                    .WithMany(p => p.Books)
                    .UsingEntity<Dictionary<string, object>>(
                        "BookAuthor",
                        l => l.HasOne<Author>().WithMany().HasForeignKey("AuthorId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_BookAuthor_Author"),
                        r => r.HasOne<Book>().WithMany().HasForeignKey("BookId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_BookAuthor_Book"),
                        j =>
                        {
                            j.HasKey("BookId", "AuthorId");

                            j.ToTable("BookAuthor");
                        });
            });

            modelBuilder.Entity<BookCopy>(entity =>
            {
                entity.HasKey(e => e.Isbn);

                entity.ToTable("BookCopy");

                entity.Property(e => e.Isbn)
                    .HasMaxLength(100)
                    .HasColumnName("ISBN");

                entity.Property(e => e.Edition)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BookCopies)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookCopy_Book");
            });

           

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId).ValueGeneratedNever();

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(50);
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
