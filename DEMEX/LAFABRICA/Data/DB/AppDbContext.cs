using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LAFABRICA.Data.DB;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<ClientPayment> ClientPayments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<MaterialSupplier> MaterialSuppliers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductOrder> ProductOrders { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ADMINIST__3214EC27261094B9");

            entity.ToTable("ADMINISTRATOR");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Identification)
                .HasMaxLength(12)
                .HasColumnName("IDENTIFICATION");
            entity.Property(e => e.Isactive)
                .HasDefaultValue((byte)1)
                .HasColumnName("ISACTIVE");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.Password).HasColumnName("PASSWORD");
            entity.Property(e => e.RolId).HasColumnName("ROL_ID");

            entity.HasOne(d => d.Rol).WithMany(p => p.Administrators)
                .HasForeignKey(d => d.RolId)
                .HasConstraintName("FK__ADMINISTR__ROL_I__412EB0B6");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CLIENT__3214EC2706FAB57E");

            entity.ToTable("CLIENT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("EMAIL");
            entity.Property(e => e.IsActive)
                .HasDefaultValue((byte)1)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsFrequent).HasColumnName("IS_FREQUENT");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("LOCATION");
            entity.Property(e => e.Manager)
                .HasMaxLength(255)
                .HasColumnName("MANAGER");
            entity.Property(e => e.ManagerPhoneNumber)
                .HasMaxLength(100)
                .HasColumnName("MANAGER_PHONE_NUMBER");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(100)
                .HasColumnName("PHONE_NUMBER");
            entity.Property(e => e.QuantityPurchase).HasColumnName("QUANTITY_PURCHASE");
            entity.Property(e => e.SpecificLocation)
                .HasMaxLength(100)
                .HasColumnName("SPECIFIC_LOCATION");
        });

        modelBuilder.Entity<ClientPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CLIENT_P__3214EC2724D43DD9");

            entity.ToTable("CLIENT_PAYMENT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("AMOUNT");
            entity.Property(e => e.OrderId).HasColumnName("ORDER_ID");
            entity.Property(e => e.PaymentDate).HasColumnName("PAYMENT_DATE");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(100)
                .HasColumnName("PAYMENT_METHOD");
            entity.Property(e => e.PhotoVoucherUrl)
                .HasMaxLength(255)
                .HasColumnName("PHOTO_VOUCHER_URL");

            entity.HasOne(d => d.Order).WithMany(p => p.ClientPayments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__CLIENT_PA__ORDER__6C190EBB");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EMPLOYEE__3214EC2784DED161");

            entity.ToTable("EMPLOYEE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Identification)
                .HasMaxLength(12)
                .HasColumnName("IDENTIFICATION");
            entity.Property(e => e.IsActive)
                .HasDefaultValue((byte)1)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.Password).HasColumnName("PASSWORD");
            entity.Property(e => e.RolId).HasColumnName("ROL_ID");
            entity.Property(e => e.Speciality)
                .HasMaxLength(100)
                .HasColumnName("SPECIALITY");

            entity.HasOne(d => d.Rol).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RolId)
                .HasConstraintName("FK__EMPLOYEE__ROL_ID__44FF419A");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__INVENTOR__3214EC2756497961");

            entity.ToTable("INVENTORY");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MaterialId).HasColumnName("MATERIAL_ID");
            entity.Property(e => e.MinimunQuantity).HasColumnName("MINIMUN_QUANTITY");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");
            entity.Property(e => e.State)
                .HasMaxLength(10)
                .HasColumnName("STATE");
            entity.Property(e => e.SupplierId).HasColumnName("SUPPLIER_ID");

            entity.HasOne(d => d.Material).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.MaterialId)
                .HasConstraintName("FK__INVENTORY__MATER__68487DD7");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__INVENTORY__SUPPL__693CA210");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MATERIAL__3214EC27EDDBBA52");

            entity.ToTable("MATERIAL");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("NAME");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(255)
                .HasColumnName("PHOTO_URL");
            entity.Property(e => e.PricePurchase)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("PRICE_PURCHASE");
            entity.Property(e => e.SupplierId).HasColumnName("SUPPLIER_ID");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Materials)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK__MATERIAL__SUPPLI__5FB337D6");
        });

        modelBuilder.Entity<MaterialSupplier>(entity =>
        {
            entity.HasKey(e => new { e.MaterialId, e.SupplierId }).HasName("PK__MATERIAL__6E61AFCB20AE3228");

            entity.ToTable("MATERIAL_SUPPLIER");

            entity.Property(e => e.MaterialId).HasColumnName("MATERIAL_ID");
            entity.Property(e => e.SupplierId).HasColumnName("SUPPLIER_ID");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");

            entity.HasOne(d => d.Material).WithMany(p => p.MaterialSuppliers)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MATERIAL___MATER__628FA481");

            entity.HasOne(d => d.Supplier).WithMany(p => p.MaterialSuppliers)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MATERIAL___SUPPL__6383C8BA");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ORDERS__3214EC270D9964BC");

            entity.ToTable("ORDERS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AdminId).HasColumnName("ADMIN_ID");
            entity.Property(e => e.Advancement)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("ADVANCEMENT");
            entity.Property(e => e.ClientId).HasColumnName("CLIENT_ID");
            entity.Property(e => e.CreationDate).HasColumnName("CREATION_DATE");
            entity.Property(e => e.DaliveryDate).HasColumnName("DALIVERY_DATE");
            entity.Property(e => e.Discount)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("DISCOUNT");
            entity.Property(e => e.IsActive)
                .HasDefaultValue((byte)1)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.Priority)
                .HasMaxLength(10)
                .HasColumnName("PRIORITY");
            entity.Property(e => e.ResumePath)
                .HasMaxLength(255)
                .HasColumnName("RESUME_PATH");
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .HasColumnName("STATE");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("TOTAL_AMOUNT");

            entity.HasOne(d => d.Admin).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK__ORDERS__ADMIN_ID__4D94879B");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK__ORDERS__CLIENT_I__4CA06362");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PRODUCTS__3214EC270CA74CC2");

            entity.ToTable("PRODUCTS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .HasColumnName("CATEGORY");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.IsActive)
                .HasDefaultValue((byte)1)
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.IsCustom)
                .HasDefaultValue((byte)1)
                .HasColumnName("IS_CUSTOM");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(255)
                .HasColumnName("PHOTO_URL");
            entity.Property(e => e.PriceBase)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("PRICE_BASE");
        });

        modelBuilder.Entity<ProductOrder>(entity =>
        {
            entity.HasKey(e => new { e.IdProduct, e.IdOrder }).HasName("PK__PRODUCT___2491A4768A57FE4F");

            entity.ToTable("PRODUCT_ORDER");

            entity.Property(e => e.IdProduct).HasColumnName("ID_PRODUCT");
            entity.Property(e => e.IdOrder).HasColumnName("ID_ORDER");
            entity.Property(e => e.Quantity).HasColumnName("QUANTITY");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.ProductOrders)
                .HasForeignKey(d => d.IdOrder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRODUCT_O__ID_OR__5AEE82B9");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.ProductOrders)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PRODUCT_O__ID_PR__59FA5E80");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ROL__3214EC27B04AB8DD");

            entity.ToTable("ROL");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.Module }).HasName("PK__ROLE_PER__93B89656E74098C7");

            entity.ToTable("ROLE_PERMISSIONS");

            entity.Property(e => e.RoleId).HasColumnName("ROLE_ID");
            entity.Property(e => e.Module)
                .HasMaxLength(100)
                .HasColumnName("MODULE");
            entity.Property(e => e.Cancreate).HasColumnName("CANCREATE");
            entity.Property(e => e.Candelete).HasColumnName("CANDELETE");
            entity.Property(e => e.Canedit).HasColumnName("CANEDIT");
            entity.Property(e => e.Canview).HasColumnName("CANVIEW");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ROLE_PERM__ROLE___3D5E1FD2");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SUPPLIER__3214EC27FB64233E");

            entity.ToTable("SUPPLIERS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.DateLastPurchase)
                .HasColumnType("datetime")
                .HasColumnName("DATE_LAST_PURCHASE");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("NAME");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("NOTES");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("PHONE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
