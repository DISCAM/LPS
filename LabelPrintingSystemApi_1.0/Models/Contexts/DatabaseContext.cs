using System;
using System.Collections.Generic;
using LabelPrintingSystemApi_1._0.Models;
using Microsoft.EntityFrameworkCore;

namespace LabelPrintingSystemApi_1._0.Models.Contexts;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Label> Labels { get; set; }

    public virtual DbSet<LabelTemplate> LabelTemplates { get; set; }

    public virtual DbSet<LogisticUnit> LogisticUnits { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PrintJob> PrintJobs { get; set; }

    public virtual DbSet<PrintJobHistory> PrintJobHistories { get; set; }

    public virtual DbSet<Printer> Printers { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductionUnit> ProductionUnits { get; set; }

    public virtual DbSet<ReprintRequest> ReprintRequests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<ScanEvent> ScanEvents { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AuditLog__EB5F6CBDC35FF467");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_AuditLogs_CreatedAt");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs).HasConstraintName("FK_AuditLogs_Users");
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasKey(e => e.LabelId).HasName("PK__Labels__397E2BC305D38AFE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Labels_CreatedAt");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Labels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Labels_Users");

            entity.HasOne(d => d.LabelTemplate).WithMany(p => p.Labels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Labels_LabelTemplates");

            entity.HasOne(d => d.LogisticUnit).WithMany(p => p.Labels).HasConstraintName("FK_Labels_LogisticUnits");

            entity.HasOne(d => d.Product).WithMany(p => p.Labels).HasConstraintName("FK_Labels_Products");

            entity.HasOne(d => d.ProductionLot).WithMany(p => p.Labels).HasConstraintName("FK_Labels_ProductionLots");
        });

        modelBuilder.Entity<LabelTemplate>(entity =>
        {
            entity.HasKey(e => e.LabelTemplateId).HasName("PK__LabelTem__242CE1EA7E912CD2");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_LabelTemplates_CreatedAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_LabelTemplates_IsActive");
            entity.Property(e => e.TemplateEngine).HasDefaultValue("AEP", "DF_LabelTemplates_TemplateEngine");
            entity.Property(e => e.VersionNo).HasDefaultValue(1, "DF_LabelTemplates_VersionNo");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.LabelTemplateCreatedByUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LabelTemplates_CreatedByUser");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.LabelTemplateModifiedByUsers).HasConstraintName("FK_LabelTemplates_ModifiedByUser");
        });

        modelBuilder.Entity<LogisticUnit>(entity =>
        {
            entity.HasKey(e => e.LogisticUnitId).HasName("PK__Logistic__ED95663F3032FFB3");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_LogisticUnits_CreatedAt");
            entity.Property(e => e.Status).HasDefaultValue("CREATED", "DF_LogisticUnits_Status");
            entity.Property(e => e.UnitType).HasDefaultValue("PALLET", "DF_LogisticUnits_UnitType");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.LogisticUnits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogisticUnits_Users");

            entity.HasOne(d => d.ProductionLot).WithMany(p => p.LogisticUnits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogisticUnits_ProductionLots");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__EFA6FB2F1B277453");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Permision_CreatedAt");
        });

        modelBuilder.Entity<PrintJob>(entity =>
        {
            entity.HasKey(e => e.PrintJobId).HasName("PK__PrintJob__180135201CE3BDA2");

            entity.Property(e => e.Copies).HasDefaultValue(1, "DF_PrintJobs_Copies");
            entity.Property(e => e.RequestedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_PrintJobs_RequestedAt");
            entity.Property(e => e.Status).HasDefaultValue("NEW", "DF_PrintJobs_Status");

            entity.HasOne(d => d.Label).WithMany(p => p.PrintJobs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrintJobs_Labels");

            entity.HasOne(d => d.Printer).WithMany(p => p.PrintJobs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrintJobs_Printers");

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.PrintJobs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrintJobs_Users");

            entity.HasOne(d => d.ScanEvent).WithMany(p => p.PrintJobs).HasConstraintName("FK_PrintJobs_ScanEvents");
        });

        modelBuilder.Entity<PrintJobHistory>(entity =>
        {
            entity.HasKey(e => e.PrintJobHistoryId).HasName("PK__PrintJob__1719F094DE020A2A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_PrintJobStatusHistory_ChangedAt");

            entity.HasOne(d => d.PrintJob).WithMany(p => p.PrintJobHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrintJobStatusHistory_PrintJobs");
        });

        modelBuilder.Entity<Printer>(entity =>
        {
            entity.HasKey(e => e.PrinterId).HasName("PK__Printers__D452AAC1B0167CCE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Printers_CreatedAt");
            entity.Property(e => e.IntegrationType).HasDefaultValue("SATO_AEP", "DF_Printers_IntegrationType");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Printers_IsActive");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CD7AEA8C89");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Products_CreatedAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Products_IsActive");
        });

        modelBuilder.Entity<ProductionUnit>(entity =>
        {
            entity.HasKey(e => e.ProductionUnitId).HasName("PK__Producti__21F484AC78DAEB0F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_ProductionLots_CreatedAt");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ProductionUnits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionLots_Users");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductionUnits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionLots_Products");
        });

        modelBuilder.Entity<ReprintRequest>(entity =>
        {
            entity.HasKey(e => e.ReprintRequestId).HasName("PK__ReprintR__7C80A049BA93861D");

            entity.Property(e => e.RequestedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_ReprintRequests_RequestedAt");

            entity.HasOne(d => d.PrintJob).WithMany(p => p.ReprintRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReprintRequests_SourcePrintJob");

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.ReprintRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReprintRequests_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1ADC8ACDEA");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Roles_CreatedAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Roles_IsActive");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_RolePermissions_CreatedAt");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermissions_Permissions");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolePermissions_Roles");
        });

        modelBuilder.Entity<ScanEvent>(entity =>
        {
            entity.HasKey(e => e.ScanEventId).HasName("PK__ScanEven__316CC94AFFAADC92");

            entity.Property(e => e.ScannedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_ScanEvents_ScannedAt");

            entity.HasOne(d => d.ProductionUnit).WithMany(p => p.ScanEvents).HasConstraintName("FK_ScanEvents_ProductionLots");

            entity.HasOne(d => d.SourceLabel).WithMany(p => p.ScanEvents).HasConstraintName("FK_ScanEvents_Labels");

            entity.HasOne(d => d.User).WithMany(p => p.ScanEvents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScanEvents_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C312E146A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Users_CreatedAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Users_IsActive");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Uers_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
