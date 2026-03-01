using System;
using System.Collections.Generic;
using Bintainer.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Repository;

public partial class BintainerDbContext : DbContext
{
    public BintainerDbContext()
    {
    }

    public BintainerDbContext(DbContextOptions<BintainerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Bin> Bins { get; set; }

    public virtual DbSet<BinSubspace> BinSubspaces { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventorySection> InventorySections { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderPartAssociation> OrderPartAssociations { get; set; }

    public virtual DbSet<Part> Parts { get; set; }

    public virtual DbSet<PartAttribute> PartAttributes { get; set; }

    public virtual DbSet<PartAttributeDefinition> PartAttributeDefinitions { get; set; }

    public virtual DbSet<PartAttributeTemplate> PartAttributeTemplates { get; set; }

    public virtual DbSet<PartBinAssociation> PartBinAssociations { get; set; }

    public virtual DbSet<PartCategory> PartCategories { get; set; }

    public virtual DbSet<PartGroup> PartGroups { get; set; }

    public virtual DbSet<PartLabel> PartLabels { get; set; }

    public virtual DbSet<PartPackage> PartPackages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.Property(e => e.RoleId).HasMaxLength(450);

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Bin>(entity =>
        {
            entity.ToTable("Bin");

            entity.HasOne(d => d.Section).WithMany(p => p.Bins)
                .HasForeignKey(d => d.SectionId);
        });

        modelBuilder.Entity<BinSubspace>(entity =>
        {
            entity.ToTable("BinSubspace");

            entity.Property(e => e.Label)
                .HasMaxLength(100);

            entity.HasOne(d => d.Bin).WithMany(p => p.BinSubspaces)
                .HasForeignKey(d => d.BinId);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.ToTable("Inventory");

            entity.Property(e => e.Admin).HasMaxLength(256);
            entity.Property(e => e.Name)
                .HasMaxLength(150);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InventorySection>(entity =>
        {
            entity.ToTable("InventorySection");

            entity.Property(e => e.SectionName)
                .HasMaxLength(150);

            entity.HasOne(d => d.Inventory).WithMany(p => p.InventorySections)
                .HasForeignKey(d => d.InventoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.HandOverDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.OrderDate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(100)
                .HasDefaultValueSql("'default'");
            entity.Property(e => e.Supplier)
                .HasMaxLength(100)
                .HasDefaultValueSql("'default'");
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrderPartAssociation>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.PartId });

            entity.ToTable("OrderPartAssociation");

            entity.Property(e => e.Quantity).HasDefaultValueSql("0");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderPartAssociations)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Part).WithMany(p => p.OrderPartAssociations)
                .HasForeignKey(d => d.PartId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Part>(entity =>
        {
            entity.ToTable("Part");

            entity.HasIndex(e => e.GuidId, "IDX_Part_GuidId");

            entity.Property(e => e.DatasheetUri).HasMaxLength(150);
            entity.Property(e => e.Description)
                .HasMaxLength(150);
            entity.Property(e => e.GuidId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ImageUri).HasMaxLength(150);
            entity.Property(e => e.Number)
                .HasMaxLength(100);
            entity.Property(e => e.Supplier)
                .HasMaxLength(100)
                .HasDefaultValueSql("'default'");
            entity.Property(e => e.SupplierUri).HasMaxLength(150);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.Category).WithMany(p => p.Parts)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Package).WithMany(p => p.Parts)
                .HasForeignKey(d => d.PackageId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Template).WithMany(p => p.Parts)
                .HasForeignKey(d => d.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.User).WithMany(p => p.Parts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasMany(d => d.Groups).WithMany(p => p.Parts)
                .UsingEntity<Dictionary<string, object>>(
                    "PartGroupAssociation",
                    r => r.HasOne<PartGroup>().WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Part>().WithMany()
                        .HasForeignKey("PartId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("PartId", "GroupId");
                        j.ToTable("PartGroupAssociation");
                    });
        });

        modelBuilder.Entity<PartAttribute>(entity =>
        {
            entity.ToTable("PartAttribute");

            entity.HasIndex(e => e.GuidId, "IDX_PartAttribute_GuidId");

            entity.Property(e => e.GuidId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Value).HasMaxLength(150);

            entity.HasOne(d => d.Part).WithMany(p => p.PartAttributes)
                .HasForeignKey(d => d.PartId);
        });

        modelBuilder.Entity<PartAttributeDefinition>(entity =>
        {
            entity.ToTable("PartAttributeDefinition");

            entity.HasIndex(e => e.GuidId, "IDX_PartAttributeDefinition_GuidId");

            entity.Property(e => e.GuidId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Value).HasMaxLength(150);

            entity.HasOne(d => d.Template).WithMany(p => p.PartAttributeDefinitions)
                .HasForeignKey(d => d.TemplateId);
        });

        modelBuilder.Entity<PartAttributeTemplate>(entity =>
        {
            entity.ToTable("PartAttributeTemplate");

            entity.HasIndex(e => e.GuidId, "IDX_PartAttributeTemplate_GuidId");

            entity.Property(e => e.GuidId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.TemplateName)
                .HasMaxLength(50);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.PartAttributeTemplates)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PartBinAssociation>(entity =>
        {
            entity.HasKey(e => new { e.PartId, e.BinId, e.SubspaceId });

            entity.ToTable("PartBinAssociation");

            entity.HasOne(d => d.Bin).WithMany(p => p.PartBinAssociations)
                .HasForeignKey(d => d.BinId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Part).WithMany(p => p.PartBinAssociations)
                .HasForeignKey(d => d.PartId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Subspace).WithMany(p => p.PartBinAssociations)
                .HasForeignKey(d => d.SubspaceId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PartCategory>(entity =>
        {
            entity.ToTable("PartCategory");

            entity.Property(e => e.GuidId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasMaxLength(75);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId);

            entity.HasOne(d => d.User).WithMany(p => p.PartCategories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PartGroup>(entity =>
        {
            entity.ToTable("PartGroup");

            entity.Property(e => e.GuidId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.PartGroups)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PartLabel>(entity =>
        {
            entity.ToTable("PartLabel");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Value)
                .HasMaxLength(50);

            entity.HasOne(d => d.Part).WithMany(p => p.PartLabels)
                .HasForeignKey(d => d.PartId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PartPackage>(entity =>
        {
            entity.ToTable("PartPackage");

            entity.Property(e => e.FullFileName).HasMaxLength(250);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasDefaultValueSql("'undefined'");
            entity.Property(e => e.Url).HasMaxLength(250);
            entity.Property(e => e.UserId).HasMaxLength(450);

            entity.HasOne(d => d.User).WithMany(p => p.PartPackages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
