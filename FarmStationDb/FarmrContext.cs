using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using FarmStation.Models;
using FarmStation.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace FarmStationDb.DataLayer;

public partial class FarmrContext : DbContext
{
	//public FarmrContext()
	//{
	//}

	public FarmrContext(DbContextOptions<FarmrContext> options)
		: base(options)
	{
	}

	public virtual DbSet<Balance> Balances { get; set; }

	public virtual DbSet<Config> Configs { get; set; }

	public virtual DbSet<Drive> Drives { get; set; }

	public virtual DbSet<Farm> Farms { get; set; }

	public virtual DbSet<Lastplot> Lastplots { get; set; }

	public virtual DbSet<Notification> Notifications { get; set; }

	public virtual DbSet<Offline> Offlines { get; set; }

	public virtual DbSet<Status> Statuses { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		////https://stackoverflow.com/questions/55033666/what-is-the-entity-framework-core-attribute-equivalent-to-modelbuilders-hasdefa/64803061#64803061
		//foreach (var entityType in modelBuilder.Model.GetEntityTypes())
		//{
		//	foreach (var property in entityType.GetProperties())
		//	{
		//		var memberInfo = property.PropertyInfo ?? (MemberInfo)property.FieldInfo!;
		//		if (memberInfo == null) continue;
		//		var defaultValue = Attribute.GetCustomAttribute(memberInfo, typeof(DefaultValueAttribute)) as DefaultValueAttribute;
		//		if (defaultValue == null) continue;
		//		//property.SqlServer().DefaultValue = defaultValue.Value;
		//		property.SetDefaultValueSql(defaultValue.Value.ToString());
		//	}
		//}

		modelBuilder.Entity<Farm>()
			.Property(f => f.FarmingStatus)
			.HasDefaultValueSql("'OFFLINE'");

		modelBuilder.Entity<Farm>()
			.Property(f => f.User)
			.HasDefaultValueSql("'(none)'");

		//modelBuilder.Entity<Farm>()
		//	.Property(f => f.Data)
		//	.HasDefaultValueSql(" ");

		modelBuilder.Entity<Farm>()
			.Property(f => f.NotifyWhenOffline)
			.HasDefaultValueSql("true");

		modelBuilder.Entity<Farm>()
			.Property(f => f.LastUpdated)
			.HasDefaultValue("2000-1-1");

		modelBuilder.Entity<Farm>()
			.Property(f => f.PublicApi)
			.HasDefaultValue(false); 

		base.OnModelCreating(modelBuilder);	
	}

	//protected override void OnModelCreating(ModelBuilder modelBuilder)
	//{
	//    modelBuilder
	//        .UseCollation("latin1_swedish_ci")
	//        .HasCharSet("latin1");

	//    modelBuilder.Entity<Balance>(entity =>
	//    {
	//        entity.HasKey(e => e.Id).HasName("PRIMARY");

	//        entity.ToTable("balances");

	//        entity.Property(e => e.Id)
	//            .HasMaxLength(200)
	//            .IsFixedLength()
	//            .HasColumnName("id");
	//        entity.Property(e => e.Balance1)
	//            .HasMaxLength(200)
	//            .HasColumnName("balance");
	//    });

	//    modelBuilder.Entity<Config>(entity =>
	//    {
	//        entity.HasKey(e => e.Id).HasName("PRIMARY");

	//        entity.ToTable("configs");

	//        entity.Property(e => e.Id)
	//            .HasMaxLength(200)
	//            .IsFixedLength()
	//            .HasColumnName("id");
	//        entity.Property(e => e.Config1)
	//            .HasColumnType("varchar(60000)")
	//            .HasColumnName("config");
	//    });

	//    modelBuilder.Entity<Drife>(entity =>
	//    {
	//        entity.HasKey(e => e.Id).HasName("PRIMARY");

	//        entity.ToTable("drives");

	//        entity.Property(e => e.Id)
	//            .HasMaxLength(200)
	//            .IsFixedLength()
	//            .HasColumnName("id");
	//        entity.Property(e => e.Drives).HasColumnName("drives");
	//    });

	//    modelBuilder.Entity<Farm>(entity =>
	//    {
	//        entity.HasKey(e => e.Id).HasName("PRIMARY");

	//        entity.ToTable("farms");

	//        entity.Property(e => e.Id)
	//            .HasMaxLength(200)
	//            .IsFixedLength()
	//            .HasColumnName("id");
	//        entity.Property(e => e.Data)
	//            .HasColumnType("mediumtext")
	//            .HasColumnName("data");
	//        entity.Property(e => e.LastUpdated)
	//            .ValueGeneratedOnAddOrUpdate()
	//            .HasDefaultValueSql("current_timestamp()")
	//            .HasColumnType("timestamp")
	//            .HasColumnName("lastUpdated");
	//        entity.Property(e => e.PublicApi).HasColumnName("publicAPI");
	//        entity.Property(e => e.User)
	//            .HasMaxLength(200)
	//            .IsFixedLength()
	//            .HasColumnName("user")
	//            .UseCollation("utf8mb3_general_ci")
	//            .HasCharSet("utf8mb3");
	//    });

	//    modelBuilder.Entity<Lastplot>(entity =>
	//    {
	//        entity.HasKey(e => e.Id).HasName("PRIMARY");

	//        entity.ToTable("lastplot");

	//        entity.Property(e => e.Id)
	//            .HasMaxLength(200)
	//            .IsFixedLength()
	//            .HasColumnName("id");
	//        entity.Property(e => e.Lastplot1)
	//            .HasMaxLength(200)
	//            .HasColumnName("lastplot");
	//    });

	//    modelBuilder.Entity<Notification>(entity =>
	//    {
	//        entity.HasKey(e => e.NotificationId).HasName("PRIMARY");

	//        entity.ToTable("notifications");

	//        entity.Property(e => e.NotificationId)
	//            .HasColumnType("int(11)")
	//            .HasColumnName("notificationID");
	//        entity.Property(e => e.Name)
	//            .HasMaxLength(200)
	//            .HasColumnName("name");
	//        entity.Property(e => e.Type)
	//            .HasMaxLength(200)
	//            .HasColumnName("type");
	//        entity.Property(e => e.User)
	//            .HasMaxLength(200)
	//            .HasColumnName("user");
	//    });

	//    modelBuilder.Entity<Offline>(entity =>
	//    {
	//        entity.HasKey(e => e.Id).HasName("PRIMARY");

	//        entity.ToTable("offline");

	//        entity.Property(e => e.Id)
	//            .HasMaxLength(200)
	//            .IsFixedLength()
	//            .HasColumnName("id");
	//        entity.Property(e => e.Name)
	//            .HasMaxLength(200)
	//            .HasColumnName("name");
	//        entity.Property(e => e.Notify)
	//            .HasColumnType("int(11)")
	//            .HasColumnName("notify");
	//    });

	//    modelBuilder.Entity<Status>(entity =>
	//    {
	//        entity.HasKey(e => e.Id).HasName("PRIMARY");

	//        entity.ToTable("statuses");

	//        entity.Property(e => e.Id)
	//            .HasMaxLength(200)
	//            .IsFixedLength()
	//            .HasColumnName("id");
	//        entity.Property(e => e.Isfarming)
	//            .HasColumnType("int(11)")
	//            .HasColumnName("isfarming");
	//    });

	//    OnModelCreatingPartial(modelBuilder);
	//}

	//partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
