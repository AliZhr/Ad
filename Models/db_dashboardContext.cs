using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Adient_DashBoard.Models
{
    public partial class db_dashboardContext : DbContext
    {
        public db_dashboardContext()
        {
        }

        public db_dashboardContext(DbContextOptions<db_dashboardContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TConfig> TConfigs { get; set; }
        public virtual DbSet<TDataplant> TDataplants { get; set; }
        public virtual DbSet<TDatarecord> TDatarecords { get; set; }
        public virtual DbSet<TRecordtype> TRecordtypes { get; set; }
        public virtual DbSet<TShift> TShifts { get; set; }
        public virtual DbSet<TTranslation> TTranslations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=a-690mv72;Database=db_dashboard;User Id=u_dashboard_admin;Password=Adm4Rastatt_db;MultipleActiveResultSets=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            modelBuilder.Entity<TConfig>(entity =>
            {
                entity.HasKey(e => e.FConfigid)
                    .HasName("f_configid");

                entity.ToTable("t_config");

                entity.Property(e => e.FConfigid).HasColumnName("f_configid");

                entity.Property(e => e.FField)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("f_field");

                entity.Property(e => e.FValue).HasColumnName("f_value");
            });

            modelBuilder.Entity<TDataplant>(entity =>
            {
                entity.HasKey(e => e.FId)
                    .HasName("f_id");

                entity.ToTable("t_dataplant");

                entity.Property(e => e.FId).HasColumnName("f_id");

                entity.Property(e => e.FDate)
                    .HasColumnType("datetime")
                    .HasColumnName("f_date");

                entity.Property(e => e.FTypeid).HasColumnName("f_typeid");

                entity.Property(e => e.FValue).HasColumnName("f_value");

                entity.HasOne(d => d.FType)
                    .WithMany(p => p.TDataplants)
                    .HasForeignKey(d => d.FTypeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("t_recordtypes_t_dataplant_fk");
            });

            modelBuilder.Entity<TDatarecord>(entity =>
            {
                entity.HasKey(e => e.FRecordid)
                    .HasName("f_recordid");

                entity.ToTable("t_datarecords");

                entity.Property(e => e.FRecordid).HasColumnName("f_recordid");

                entity.Property(e => e.FRecordtypeid).HasColumnName("f_recordtypeid");

                entity.Property(e => e.FRecordvalue).HasColumnName("f_recordvalue");

                entity.Property(e => e.FShiftid).HasColumnName("f_shiftid");

                entity.Property(e => e.FTypeid).HasColumnName("f_typeid");

                entity.HasOne(d => d.FShift)
                    .WithMany(p => p.TDatarecords)
                    .HasForeignKey(d => d.FShiftid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("t_shift_t_datarecords_fk");

                entity.HasOne(d => d.FType)
                    .WithMany(p => p.TDatarecords)
                    .HasForeignKey(d => d.FTypeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("t_recordtypes_t_datarecords_fk");
            });

            modelBuilder.Entity<TRecordtype>(entity =>
            {
                entity.HasKey(e => e.FTypeid)
                    .HasName("f_typeid");

                entity.ToTable("t_recordtypes");

                entity.Property(e => e.FTypeid).HasColumnName("f_typeid");

                entity.Property(e => e.FBand).HasColumnName("f_band");

                entity.Property(e => e.FTypedescription)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("f_typedescription");

                entity.Property(e => e.FTypename)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("f_typename");
            });

            modelBuilder.Entity<TShift>(entity =>
            {
                entity.HasKey(e => e.FShiftid)
                    .HasName("f_shiftid");

                entity.ToTable("t_shift");

                entity.Property(e => e.FShiftid).HasColumnName("f_shiftid");

                entity.Property(e => e.FConfigid).HasColumnName("f_configid");

                entity.Property(e => e.FCoordinator)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("f_coordinator");

                entity.Property(e => e.FDate)
                    .HasColumnType("datetime")
                    .HasColumnName("f_date");

                entity.Property(e => e.FHall)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("f_hall");

                entity.Property(e => e.FSection)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("f_section");

                entity.Property(e => e.FShift)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("f_shift");

                entity.Property(e => e.FTimestamp)
                    .HasColumnType("datetime")
                    .HasColumnName("f_timestamp");

                entity.HasOne(d => d.FConfig)
                    .WithMany(p => p.TShifts)
                    .HasForeignKey(d => d.FConfigid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("t_config_t_shift_fk");
            });

            modelBuilder.Entity<TTranslation>(entity =>
            {
                entity.HasKey(e => e.FTranslationid)
                    .HasName("t_translationid");

                entity.ToTable("t_translation");

                entity.Property(e => e.FTranslationid).HasColumnName("f_translationid");

                entity.Property(e => e.FLabelid).HasColumnName("f_labelid");

                entity.Property(e => e.FLanguage)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("f_language");

                entity.Property(e => e.FTranslation)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .HasColumnName("f_translation");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
