using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interlex.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public virtual DbSet<Case> Cases { get; set; }
        public virtual DbSet<CaseLog> CasesLog { get; set; }
        public virtual DbSet<Codes> Codes { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Relationship> Relationships { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<Texts> Texts { get; set; }
        public virtual DbSet<Court> Court { get; set; }
        public virtual DbSet<CourtEng> CourtEng { get; set; }

        public virtual DbSet<Keyword> Keywords { get; set; }

        public virtual DbSet<Jurisdiction> Jurisdictions { get; set; }
        public virtual DbSet<Source> Source { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Codes>(entity =>
            {
                entity.ToTable("Codes", "classifier");

                entity.HasIndex(e => e.Code)
                    .HasName("idx_classifier_Codes_Code")
                    .IsUnique();

                entity.HasIndex(e => e.Level)
                    .HasName("idx_classifier_Codes_Level");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            builder.Entity<Court>(entity =>
            {
                entity.ToTable("Court", "suggest");

                entity.HasKey(e => new { e.Name, e.JurId });

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.Jurisdiction)
                    .WithMany(p => p.Courts)
                    .HasForeignKey(d => d.JurId)
                    .HasConstraintName("FK_Court_Jurisdiction");
            });

            builder.Entity<Keyword>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.ToTable("Keywords", "suggest");

                entity.Property(e => e.Name)
                    .HasMaxLength(256)
                    .ValueGeneratedNever();
            });

            builder.Entity<CourtEng>(entity =>
            {
                entity.ToTable("CourtEng", "suggest");

                entity.HasKey(e => new { e.Name, e.JurId });

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.Jurisdiction)
                    .WithMany(p => p.CourtsEng)
                    .HasForeignKey(d => d.JurId)
                    .HasConstraintName("FK_CourtEng_Jurisdiction");
            });

            builder.Entity<Jurisdiction>(entity =>
            {
                entity.ToTable("Jurisdictions", "suggest");

                entity.Property(e => e.JurCode)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            builder.Entity<Source>(entity =>
            {
                entity.ToTable("Source", "suggest");

                entity.HasKey(e => new { e.Name, e.JurId });

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.Jurisdiction)
                    .WithMany(p => p.Sources)
                    .HasForeignKey(d => d.JurId)
                    .HasConstraintName("FK_Source_Jurisdiction");
            });

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.OrganizationId).HasMaxLength(450);
                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK_Organizations_Users");
                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
                entity.HasMany(e => e.Claims)
                .WithOne().HasForeignKey(uc => uc.UserId)
                .IsRequired();
            });

            builder.Entity<Case>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("idx_userId_cases");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Caption)
                    .IsRequired()
                    .HasColumnName("caption")
                    .HasMaxLength(4000);

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content");

                entity.Property(e => e.LastChange).HasColumnName("lastChange");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Cases)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Cases__userId__4BAC3F29");
            });

            builder.Entity<CaseLog>(entity =>
            {
                entity.HasIndex(e => e.CaseId)
                    .HasName("idx_caseid_caseslog");

                entity.HasIndex(e => e.UserId)
                    .HasName("idx_userId_caseslog");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CaseId).HasColumnName("caseId");

                entity.Property(e => e.ChangeDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("userId");

                entity.HasOne(d => d.Case)
                    .WithMany(p => p.CasesLog)
                    .HasForeignKey(d => d.CaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CasesLog__caseId__59063A47");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CasesLog)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CasesLog__userId__5812160E");
            });

            builder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.FullName).HasMaxLength(256);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            builder.Entity<Language>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ThreeLetter)
                    .IsRequired()
                    .HasMaxLength(3);

                entity.Property(e => e.TwoLetter)
                    .IsRequired()
                    .HasMaxLength(2);
            });

            builder.Entity<Relationship>(entity =>
            {
                entity.HasKey(e => new { e.ParentId, e.ChildId });

                entity.ToTable("Relationships", "classifier");

                entity.HasIndex(e => e.ChildId)
                    .HasName("idx_classifier_Relationships_ChildId");

                entity.HasIndex(e => e.ParentId)
                    .HasName("idx_classifier_Relationships_ParentId");

                entity.HasOne(d => d.Child)
                    .WithMany(p => p.Parents)
                    .HasForeignKey(d => d.ChildId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_classifier_Relationships_ChildId");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.Children)
                    .HasForeignKey(d => d.ParentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_classifier_Relationships_ParentId");
            });

            builder.Entity<Texts>(entity =>
            {
                entity.ToTable("Texts", "classifier");

                entity.Property(e => e.Text).IsRequired();

                entity.HasOne(d => d.Classifier)
                    .WithMany(p => p.Texts)
                    .HasForeignKey(d => d.ClassifierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_classifier_Texts_Codes");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.Texts)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Texts_Languages");
            });

        }
    }
}