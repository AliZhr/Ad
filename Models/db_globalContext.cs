using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Adient_DashBoard.Models
{
    public partial class db_globalContext : DbContext
    {
        public db_globalContext()
        {
        }

        public db_globalContext(DbContextOptions<db_globalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TApplication> TApplications { get; set; }
        public virtual DbSet<TCounter> TCounters { get; set; }
        public virtual DbSet<TRole> TRoles { get; set; }
        public virtual DbSet<TUser> TUsers { get; set; }
        public virtual DbSet<TUserAccess> TUserAccesses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=a-690mv87.autoexpr.com;database=db_global;user id=u_dashboard_usr;password=usrUsr_188;allowzerodatetime=True", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.16-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_0900_ai_ci");

            modelBuilder.Entity<TApplication>(entity =>
            {
                entity.HasKey(e => e.FApplicationId)
                    .HasName("PRIMARY");

                entity.ToTable("t_application");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.FApplicationId)
                    .HasColumnType("int(10)")
                    .HasColumnName("f_Application_Id");

                entity.Property(e => e.FApplication)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("f_Application")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<TCounter>(entity =>
            {
                entity.HasKey(e => e.FWebpage)
                    .HasName("PRIMARY");

                entity.ToTable("t_counter");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.FWebpage)
                    .HasColumnName("f_webpage")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.FCount)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("f_count");

                entity.Property(e => e.FLastAccessed)
                    .HasMaxLength(20)
                    .HasColumnName("f_last_accessed")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<TRole>(entity =>
            {
                entity.HasKey(e => e.FRoleId)
                    .HasName("PRIMARY");

                entity.ToTable("t_role");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.FRoleId)
                    .HasColumnType("int(10)")
                    .HasColumnName("f_role_id");

                entity.Property(e => e.FRole)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("f_role")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<TUser>(entity =>
            {
                entity.HasKey(e => e.FUserId)
                    .HasName("PRIMARY");

                entity.ToTable("t_user");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.FUserId)
                    .HasMaxLength(10)
                    .HasColumnName("f_UserId")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.FActive)
                    .IsRequired()
                    .HasColumnType("enum('yes','no')")
                    .HasColumnName("f_active")
                    .HasDefaultValueSql("'yes'");

                entity.Property(e => e.FChangePw)
                    .IsRequired()
                    .HasColumnType("enum('yes','no')")
                    .HasColumnName("f_Change_Pw")
                    .HasDefaultValueSql("'yes'");

                entity.Property(e => e.FEmail)
                    .HasMaxLength(50)
                    .HasColumnName("f_email");

                entity.Property(e => e.FFirstName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("f_FirstName")
                    .HasDefaultValueSql("''");

                /*entity.Property(e => e.FLastAccess)
                    .HasColumnType("datetime")
                    .HasColumnName("f_LastAccess")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");*/

                entity.Property(e => e.FLastName)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("f_LastName")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.FPassword)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("f_Password")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<TUserAccess>(entity =>
            {
                entity.HasKey(e => new { e.FUserId, e.FApplication })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("t_user_access");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.FUserId)
                    .HasMaxLength(10)
                    .HasColumnName("f_UserId")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.FApplication)
                    .HasMaxLength(20)
                    .HasColumnName("f_Application")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.FRole)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("f_Role")
                    .HasDefaultValueSql("''");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
