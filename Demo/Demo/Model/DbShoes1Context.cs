using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Demo.Model;

public partial class DbShoes1Context : DbContext
{
    public DbShoes1Context()
    {
    }

    public DbShoes1Context(DbContextOptions<DbShoes1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<PickUpPoint> PickUpPoints { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StructureOrder> StructureOrders { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Tovar> Tovars { get; set; }

    public virtual DbSet<TovarCategory> TovarCategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=1234;database=db_shoes1", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.44-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("PRIMARY");

            entity.ToTable("manufacturers");

            entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id");
            entity.Property(e => e.ManufacturerName)
                .HasMaxLength(30)
                .HasColumnName("manufacturer_name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.PickUpPointId, "fk_pick_up_point_idx");

            entity.HasIndex(e => e.UserId, "fk_user_id_idx");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(3)
                .HasColumnName("order_code");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.OrderDateDelivery).HasColumnName("order_date_delivery");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(8)
                .HasColumnName("order_status");
            entity.Property(e => e.PickUpPointId).HasColumnName("pick_up_point_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.PickUpPoint).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PickUpPointId)
                .HasConstraintName("fk_pick_up_point");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_id");
        });

        modelBuilder.Entity<PickUpPoint>(entity =>
        {
            entity.HasKey(e => e.PickUpPointId).HasName("PRIMARY");

            entity.ToTable("pick_up_points");

            entity.Property(e => e.PickUpPointId).HasColumnName("pick_up_point_id");
            entity.Property(e => e.PickUpPointCity)
                .HasMaxLength(35)
                .HasColumnName("pick_up_point_city");
            entity.Property(e => e.PickUpPointHome)
                .HasMaxLength(5)
                .HasColumnName("pick_up_point_home");
            entity.Property(e => e.PickUpPointIndex)
                .HasMaxLength(6)
                .HasColumnName("pick_up_point_index");
            entity.Property(e => e.PickUpPointStreet)
                .HasMaxLength(45)
                .HasColumnName("pick_up_point_street");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(25)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<StructureOrder>(entity =>
        {
            entity.HasKey(e => e.StructureOrderId).HasName("PRIMARY");

            entity.ToTable("structure_orders");

            entity.HasIndex(e => e.OrderId, "fk_order_idx");

            entity.HasIndex(e => e.TovarArticle, "fk_tovar_idx");

            entity.Property(e => e.StructureOrderId).HasColumnName("structure_order_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.StructureOrderTovarCount).HasColumnName("structure_order_tovar_count");
            entity.Property(e => e.TovarArticle)
                .HasMaxLength(6)
                .HasColumnName("tovar_article");

            entity.HasOne(d => d.Order).WithMany(p => p.StructureOrders)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("fk_order");

            entity.HasOne(d => d.TovarArticleNavigation).WithMany(p => p.StructureOrders)
                .HasForeignKey(d => d.TovarArticle)
                .HasConstraintName("fk_tovar");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PRIMARY");

            entity.ToTable("suppliers");

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(30)
                .HasColumnName("supplier_name");
        });

        modelBuilder.Entity<Tovar>(entity =>
        {
            entity.HasKey(e => e.TovarArticle).HasName("PRIMARY");

            entity.ToTable("tovars");

            entity.HasIndex(e => e.ManufacturerId, "fk_manufacturer_idx");

            entity.HasIndex(e => e.SupplierId, "fk_supplier_idx");

            entity.HasIndex(e => e.TovarCategoryId, "fk_tovar_category_idx");

            entity.Property(e => e.TovarArticle)
                .HasMaxLength(6)
                .HasColumnName("tovar_article");
            entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id");
            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.TovarCategoryId).HasColumnName("tovar_category_id");
            entity.Property(e => e.TovarCost)
                .HasPrecision(10, 2)
                .HasColumnName("tovar_cost");
            entity.Property(e => e.TovarCount).HasColumnName("tovar_count");
            entity.Property(e => e.TovarDescription)
                .HasColumnType("text")
                .HasColumnName("tovar_description");
            entity.Property(e => e.TovarDiscount).HasColumnName("tovar_discount");
            entity.Property(e => e.TovarImage)
                .HasColumnType("mediumblob")
                .HasColumnName("tovar_image");
            entity.Property(e => e.TovarName)
                .HasMaxLength(45)
                .HasColumnName("tovar_name");
            entity.Property(e => e.TovarUnit)
                .HasMaxLength(5)
                .HasColumnName("tovar_unit");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.ManufacturerId)
                .HasConstraintName("fk_manufacturer");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("fk_supplier");

            entity.HasOne(d => d.TovarCategory).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.TovarCategoryId)
                .HasConstraintName("fk_tovar_category");
        });

        modelBuilder.Entity<TovarCategory>(entity =>
        {
            entity.HasKey(e => e.TovarCategoryId).HasName("PRIMARY");

            entity.ToTable("tovar_categories");

            entity.Property(e => e.TovarCategoryId).HasColumnName("tovar_category_id");
            entity.Property(e => e.TovarCategoryName)
                .HasMaxLength(15)
                .HasColumnName("tovar_category_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.RoleId, "fk_role_idx");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserLastname)
                .HasMaxLength(25)
                .HasColumnName("user_lastname");
            entity.Property(e => e.UserLogin)
                .HasMaxLength(45)
                .HasColumnName("user_login");
            entity.Property(e => e.UserName)
                .HasMaxLength(20)
                .HasColumnName("user_name");
            entity.Property(e => e.UserPass)
                .HasMaxLength(255)
                .HasColumnName("user_pass");
            entity.Property(e => e.UserSurname)
                .HasMaxLength(25)
                .HasColumnName("user_surname");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("fk_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
