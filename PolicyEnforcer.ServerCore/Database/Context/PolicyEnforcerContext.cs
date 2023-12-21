using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PolicyEnforcer.ServerCore.Database.Models;

namespace PolicyEnforcer.ServerCore.Database.Context;

public partial class PolicyEnforcerContext : DbContext
{
    public PolicyEnforcerContext()
    {
    }

    public PolicyEnforcerContext(DbContextOptions<PolicyEnforcerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BrowserHistory> BrowserHistories { get; set; }

    public virtual DbSet<HardwareInfo> HardwareInfos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\HOASERVER_DEV;Database=PolicyEnforcer;User Id=sa;Password=chuchikmuchik;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BrowserHistory>(entity =>
        {
            entity.ToTable("BrowserHistory");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.BrowserName).HasMaxLength(30);
            entity.Property(e => e.DateVisited).HasColumnType("datetime");
            entity.Property(e => e.Url)
                .HasColumnType("text")
                .HasColumnName("URL");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.BrowserHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_BrowserHistory_Users");
        });

        modelBuilder.Entity<HardwareInfo>(entity =>
        {
            entity.HasKey(e => e.MeasurementId);

            entity.ToTable("HardwareInfo");

            entity.Property(e => e.MeasurementId)
                .ValueGeneratedNever()
                .HasColumnName("MeasurementID");
            entity.Property(e => e.DateMeasured).HasColumnType("datetime");
            entity.Property(e => e.InstanceName).HasMaxLength(70);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.HardwareInfos)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_HardwareInfo_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.Login).HasMaxLength(40);
            entity.Property(e => e.Password)
                .HasMaxLength(16)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
