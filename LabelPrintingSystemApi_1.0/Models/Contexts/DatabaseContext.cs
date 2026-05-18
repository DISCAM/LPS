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

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Label> Labels { get; set; }

    public virtual DbSet<LabelTemplate> LabelTemplates { get; set; }

    public virtual DbSet<LogisticUnit> LogisticUnits { get; set; }

    public virtual DbSet<LogisticUnitItem> LogisticUnitItems { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PrintJob> PrintJobs { get; set; }

    public virtual DbSet<PrintJobHistory> PrintJobHistories { get; set; }

    public virtual DbSet<Printer> Printers { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductionLot> ProductionLots { get; set; }

    public virtual DbSet<ProductionOrder> ProductionOrders { get; set; }

    public virtual DbSet<ReprintRequest> ReprintRequests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<ScanEvent> ScanEvents { get; set; }

    public virtual DbSet<StockMovement> StockMovements { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WarehouseOrder> WarehouseOrders { get; set; }

    public virtual DbSet<WarehouseOrderItem> WarehouseOrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AuditLog__EB5F6CBDC35FF467");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_AuditLogs_CreatedAt");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.AuditLogs).HasConstraintName("FK_AuditLogs_CreatedByUser");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D824632F41");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Customers_CreatedAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Customers_IsActive");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.CustomerCreatedByUsers).HasConstraintName("FK_Customers_CreatedByUser");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.CustomerModifiedByUsers).HasConstraintName("FK_Customers_ModifiedByUser");
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasKey(e => e.LabelId).HasName("PK__Labels__397E2BC305D38AFE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Labels_CreatedAt");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Labels)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Labels_CreatedByUser");

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

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.LogisticUnitCreatedByUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogisticUnits_CreatedByUser");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.LogisticUnitModifiedByUsers).HasConstraintName("FK_LogisticUnits_ModifiedByUser");

            entity.HasOne(d => d.WarehouseOrder).WithMany(p => p.LogisticUnits).HasConstraintName("FK_LogisticUnits_WarehouseOrders");
        });

        modelBuilder.Entity<LogisticUnitItem>(entity =>
        {
            entity.HasKey(e => e.LogisticUnitItemId).HasName("PK__Logistic__9F739072C9CC73E1");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_LogisticUnitItems_CreatedAt");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.LogisticUnitItemCreatedByUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogisticUnitItems_CreatedByUser");

            entity.HasOne(d => d.LogisticUnit).WithMany(p => p.LogisticUnitItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogisticUnitItems_LogisticUnits");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.LogisticUnitItemModifiedByUsers).HasConstraintName("FK_LogisticUnitItems_ModifiedByUser");

            entity.HasOne(d => d.ProductionLot).WithMany(p => p.LogisticUnitItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogisticUnitItems_ProductionLots");

            entity.HasOne(d => d.WarehouseOrderItem).WithMany(p => p.LogisticUnitItems).HasConstraintName("FK_LogisticUnitItems_WarehouseOrderItems");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__EFA6FB2F1B277453");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Permissions_CreatedAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Permissions_IsActive");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.PermissionCreatedByUsers).HasConstraintName("FK_Permissions_CreatedByUser");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.PermissionModifiedByUsers).HasConstraintName("FK_Permissions_ModifiedByUser");
        });

        modelBuilder.Entity<PrintJob>(entity =>
        {
            entity.HasKey(e => e.PrintJobId).HasName("PK__PrintJob__180135201CE3BDA2");

            entity.Property(e => e.Copies).HasDefaultValue(1, "DF_PrintJobs_Copies");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_PrintJobs_CreatedAt");
            entity.Property(e => e.Status).HasDefaultValue("NEW", "DF_PrintJobs_Status");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.PrintJobCreatedByUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrintJobs_CreatedByUser");

            entity.HasOne(d => d.Label).WithMany(p => p.PrintJobs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrintJobs_Labels");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.PrintJobModifiedByUsers).HasConstraintName("FK_PrintJobs_ModifiedByUser");

            entity.HasOne(d => d.Printer).WithMany(p => p.PrintJobs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrintJobs_Printers");

            entity.HasOne(d => d.ScanEvent).WithMany(p => p.PrintJobs).HasConstraintName("FK_PrintJobs_ScanEvents");
        });

        modelBuilder.Entity<PrintJobHistory>(entity =>
        {
            entity.HasKey(e => e.PrintJobHistoryId).HasName("PK__PrintJob__1719F094DE020A2A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_PrintJobHistory_CreatedAt");
            entity.Property(e => e.Status).HasDefaultValue("NEW", "DF_PrintJobHistory_Status");

            entity.HasOne(d => d.PrintJob).WithMany(p => p.PrintJobHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrintJobHistory_PrintJobs");
        });

        modelBuilder.Entity<Printer>(entity =>
        {
            entity.HasKey(e => e.PrinterId).HasName("PK__Printers__D452AAC1B0167CCE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Printers_CreatedAt");
            entity.Property(e => e.IntegrationType).HasDefaultValue("SATO_AEP", "DF_Printers_IntegrationType");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Printers_IsActive");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.PrinterCreatedByUsers).HasConstraintName("FK_Printers_CreatedByUser");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.PrinterModifiedByUsers).HasConstraintName("FK_Printers_ModifiedByUser");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CD7AEA8C89");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Products_CreatedAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Products_IsActive");
        });

        modelBuilder.Entity<ProductionLot>(entity =>
        {
            entity.HasKey(e => e.ProductionLotId).HasName("PK__Producti__21F484AC78DAEB0F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_ProductionLots_CreatedAt");
            entity.Property(e => e.Status).HasDefaultValue("CREATED", "DF_ProductionLots_Status");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ProductionLotCreatedByUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionLots_Users");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.ProductionLotModifiedByUsers).HasConstraintName("FK_ProductionLots_ModifiedByUser");

            entity.HasOne(d => d.ProductionOrder).WithMany(p => p.ProductionLots)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionLots_ProductionOrders");
        });

        modelBuilder.Entity<ProductionOrder>(entity =>
        {
            entity.HasKey(e => e.ProductionOrderId).HasName("PK__Producti__E861541072DCC8A0");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_ProductionOrders_CreatedAt");
            entity.Property(e => e.ProductionOrderType).HasDefaultValue("STOCK", "DF_ProductionOrders_ProductionOrderType");
            entity.Property(e => e.Status).HasDefaultValue("NEW", "DF_ProductionOrders_Status");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ProductionOrderCreatedByUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionOrders_CreatedByUser");

            entity.HasOne(d => d.Customer).WithMany(p => p.ProductionOrders).HasConstraintName("FK_ProductionOrders_Customers");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.ProductionOrderModifiedByUsers).HasConstraintName("FK_ProductionOrders_ModifiedByUser");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductionOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionOrders_Products");
        });

        modelBuilder.Entity<ReprintRequest>(entity =>
        {
            entity.HasKey(e => e.ReprintRequestId).HasName("PK__ReprintR__7C80A049BA93861D");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_ReprintRequests_CreatedAt");
            entity.Property(e => e.Status).HasDefaultValue("NEW", "DF_ReprintRequests_Status");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ReprintRequestCreatedByUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReprintRequests_CreatedByUser");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.ReprintRequestModifiedByUsers).HasConstraintName("FK_ReprintRequests_ModifiedByUser");

            entity.HasOne(d => d.PrintJob).WithMany(p => p.ReprintRequests)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReprintRequests_PrintJobs");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1ADC8ACDEA");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Roles_CreatedAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Roles_IsActive");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.RoleCreatedByUsers).HasConstraintName("FK_Roles_CreatedByUser");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.RoleModifiedByUsers).HasConstraintName("FK_Roles_ModifiedByUser");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_RolePermissions_CreatedAt");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.RolePermissions).HasConstraintName("FK_RolePermissions_CreatedByUser");

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

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_ScanEvents_CreatedAt");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.ScanEvents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScanEvents_CreatedByUser");

            entity.HasOne(d => d.ProductionLot).WithMany(p => p.ScanEvents).HasConstraintName("FK_ScanEvents_ProductionLots");

            entity.HasOne(d => d.SourceLabel).WithMany(p => p.ScanEvents).HasConstraintName("FK_ScanEvents_Labels");
        });

        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(e => e.StockMovementId).HasName("PK__StockMov__E963E37C4EF95250");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_StockMovements_CreatedAt");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.StockMovements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockMovements_CreatedByUser");

            entity.HasOne(d => d.LogisticUnit).WithMany(p => p.StockMovements).HasConstraintName("FK_StockMovements_LogisticUnits");

            entity.HasOne(d => d.ProductionLot).WithMany(p => p.StockMovements)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockMovements_ProductionLots");

            entity.HasOne(d => d.WarehouseOrder).WithMany(p => p.StockMovements).HasConstraintName("FK_StockMovements_WarehouseOrders");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C312E146A");

            entity.HasIndex(e => e.IdentityUserId, "IX_Users_IdentityUserId")
                .IsUnique()
                .HasFilter("([IdentityUserId] IS NOT NULL)");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Users_CreatedAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Users_IsActive");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.InverseCreatedByUser).HasConstraintName("FK_Users_CreatedByUser");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.InverseModifiedByUser).HasConstraintName("FK_Users_ModifiedByUser");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<WarehouseOrder>(entity =>
        {
            entity.HasKey(e => e.WarehouseOrderId).HasName("PK__Warehous__1F0ABD6DAA2E4C47");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_WarehouseOrders_CreatedAt");
            entity.Property(e => e.OrderType).HasDefaultValue("SHIPMENT", "DF_WarehouseOrders_OrderType");
            entity.Property(e => e.Status).HasDefaultValue("NEW", "DF_WarehouseOrders_Status");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.WarehouseOrderCreatedByUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WarehouseOrders_CreatedByUser");

            entity.HasOne(d => d.Customer).WithMany(p => p.WarehouseOrders).HasConstraintName("FK_WarehouseOrders_Customers");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.WarehouseOrderModifiedByUsers).HasConstraintName("FK_WarehouseOrders_ModifiedByUser");
        });

        modelBuilder.Entity<WarehouseOrderItem>(entity =>
        {
            entity.HasKey(e => e.WarehouseOrderItemId).HasName("PK__Warehous__1FCF6C41636F3B8A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_WarehouseOrderItems_CreatedAt");
            entity.Property(e => e.Status).HasDefaultValue("NEW", "DF_WarehouseOrderItems_Status");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.WarehouseOrderItemCreatedByUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WarehouseOrderItems_CreatedByUser");

            entity.HasOne(d => d.ModifiedByUser).WithMany(p => p.WarehouseOrderItemModifiedByUsers).HasConstraintName("FK_WarehouseOrderItems_ModifiedByUser");

            entity.HasOne(d => d.Product).WithMany(p => p.WarehouseOrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WarehouseOrderItems_Products");

            entity.HasOne(d => d.WarehouseOrder).WithMany(p => p.WarehouseOrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WarehouseOrderItems_WarehouseOrders");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
