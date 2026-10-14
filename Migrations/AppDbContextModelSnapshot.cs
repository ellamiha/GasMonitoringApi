using GasMonitoringApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GasMonitoringApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "10.0.12");

            modelBuilder.Entity("GasMonitoringApi.Auth.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedLogin")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedLogin")
                        .IsUnique();

                    b.ToTable("Users", null, t =>
                        {
                            t.HasCheckConstraint("CK_Users_Role", "Role IN ('User', 'Admin')");
                        });
                });

            modelBuilder.Entity("GasMonitoringApi.Models.Measurement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("Flow")
                        .HasColumnType("REAL");

                    b.Property<double>("Pressure")
                        .HasColumnType("REAL");

                    b.Property<long>("RecordedAt")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WellId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("WellId", "RecordedAt");

                    b.ToTable("Measurements", null, t =>
                        {
                            t.HasCheckConstraint("CK_Measurements_Flow", "Flow BETWEEN 0 AND 1000000");

                            t.HasCheckConstraint("CK_Measurements_Pressure", "Pressure BETWEEN 0 AND 1000");
                        });
                });

            modelBuilder.Entity("GasMonitoringApi.Models.Well", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("MaxPressure")
                        .HasColumnType("REAL");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<double>("PlannedFlow")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Wells", null, t =>
                        {
                            t.HasCheckConstraint("CK_Wells_MaxPressure", "MaxPressure BETWEEN 0.01 AND 1000");

                            t.HasCheckConstraint("CK_Wells_Name", "length(trim(Name)) BETWEEN 1 AND 100");

                            t.HasCheckConstraint("CK_Wells_PlannedFlow", "PlannedFlow BETWEEN 0.01 AND 1000000");
                        });
                });

            modelBuilder.Entity("GasMonitoringApi.Models.Measurement", b =>
                {
                    b.HasOne("GasMonitoringApi.Models.Well", "Well")
                        .WithMany("Measurements")
                        .HasForeignKey("WellId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Well");
                });

            modelBuilder.Entity("GasMonitoringApi.Models.Well", b =>
                {
                    b.Navigation("Measurements");
                });
#pragma warning restore 612, 618
        }
    }
}
