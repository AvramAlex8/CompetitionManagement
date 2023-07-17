using System;
using System.Collections.Generic;
using CompetitionManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagement.Data;

public partial class CompetitionManagementContext : DbContext
{
    public CompetitionManagementContext()
    {
    }

    public CompetitionManagementContext(DbContextOptions<CompetitionManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Competition> Competitions { get; set; }

    public virtual DbSet<CompetitionType> CompetitionTypes { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-R5GD5V1; Initial catalog=CompetitionManagement; trusted_connection=yes; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Competition>(entity =>
        {
            entity.HasOne(d => d.Type).WithMany(p => p.Competitions).HasConstraintName("Competition_FK_Type");

            entity.HasMany(d => d.Teams).WithMany(p => p.Competitions)
                .UsingEntity<Dictionary<string, object>>(
                    "TeamCompetition",
                    r => r.HasOne<Team>().WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_TeamCompetition_TeamID"),
                    l => l.HasOne<Competition>().WithMany()
                        .HasForeignKey("CompetitionId")
                        .HasConstraintName("FK_TeamCompetition_CompetitionID"),
                    j =>
                    {
                        j.HasKey("CompetitionId", "TeamId");
                        j.ToTable("TeamCompetition");
                        j.IndexerProperty<int>("CompetitionId").HasColumnName("CompetitionID");
                        j.IndexerProperty<int>("TeamId").HasColumnName("TeamID");
                    });
        });

        modelBuilder.Entity<CompetitionType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Type");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasOne(d => d.Competition).WithMany(p => p.Games).HasConstraintName("FK_Game_Competition");

            entity.HasOne(d => d.Team1).WithMany(p => p.GameTeam1s)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Team1ID");

            entity.HasOne(d => d.Team2).WithMany(p => p.GameTeam2s)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Team2ID");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Member");

            entity.HasOne(d => d.Team).WithMany(p => p.Players)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Member_FK_Team");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
