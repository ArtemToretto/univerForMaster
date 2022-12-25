using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace aspNETuniversity.Models
{
    public partial class univerContext : DbContext
    {
        public univerContext()
        {
        }

        public univerContext(DbContextOptions<univerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AverageSalarySmaller> AverageSalarySmallers { get; set; } = null!;
        public virtual DbSet<Faculty> Facultys { get; set; } = null!;
        public virtual DbSet<Specialization> Specializations { get; set; } = null!;
        public virtual DbSet<StudGroup> StudGroups { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<StudentsAndAverageSalary> StudentsAndAverageSalaries { get; set; } = null!;
        public virtual DbSet<StudentsAndAverageSalaryWithFunction> StudentsAndAverageSalaryWithFunctions { get; set; } = null!;
        public virtual DbSet<UniversityAudit> UniversityAudits { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-2359S0L; Database=univer; Trusted_connection=true; TrustServerCertificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AverageSalarySmaller>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("average_salary_smaller");

                entity.Property(e => e.AverageSalaryForOnePeople).HasColumnName("average_salary_for_one_people");

                entity.Property(e => e.GroupCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("group_code");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Faculty>(entity =>
            {
                entity.HasKey(e => e.FacultyCode);

                entity.ToTable("facultys");

                entity.HasIndex(e => e.FacultyName, "UC_faculty_name")
                    .IsUnique();

                entity.HasIndex(e => e.FacultyName, "indx_name");

                entity.Property(e => e.FacultyCode)
                .HasColumnName("faculty_code")
                .HasColumnType("int");

                entity.Property(e => e.DeanName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("dean_name");

                entity.Property(e => e.FacultyName)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("faculty_name");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("roles");

                entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("int");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Specialization>(entity =>
            {
                entity.HasKey(e => e.SpecCode);

                entity.ToTable("specializations");

                entity.HasIndex(e => e.Name, "UC_spec_name")
                    .IsUnique();

                entity.HasIndex(e => e.FacultyCode, "indx_faculty_code");

                entity.Property(e => e.SpecCode)
                    .ValueGeneratedNever()
                    .HasColumnName("spec_code");

                entity.Property(e => e.Cvalification)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("cvalification");

                entity.Property(e => e.FacultyCode).HasColumnName("faculty_code");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.HasOne(d => d.FacultyCodeNavigation)
                    .WithMany(p => p.Specializations)
                    .HasForeignKey(d => d.FacultyCode)
                    .HasConstraintName("FK_specializations_facultys");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("users");

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.Login)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("login");

                entity.Property(e => e.RoleId).HasColumnName("roleId");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.HasOne(d => d.RoleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_users_roles");
            });

            modelBuilder.Entity<StudGroup>(entity =>
            {
                entity.HasKey(e => e.StudGroupCode);

                entity.ToTable("stud_groups");

                entity.HasIndex(e => e.SpecializationCode, "indx_spec");

                entity.Property(e => e.StudGroupCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("stud_group_code");

                entity.Property(e => e.SpecializationCode).HasColumnName("specialization_code");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.HasOne(d => d.SpecializationCodeNavigation)
                    .WithMany(p => p.StudGroups)
                    .HasForeignKey(d => d.SpecializationCode)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_stud_groups_specializations");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Zachetka);

                entity.ToTable("students");

                entity.HasIndex(e => e.StudGroupCode, "indx_group");

                entity.Property(e => e.Zachetka)
                    .ValueGeneratedNever()
                    .HasColumnName("zachetka");

                entity.Property(e => e.FamilyKol).HasColumnName("family_kol");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.SalaryFather)
                    .HasColumnName("salary_father")
                    .HasDefaultValueSql("((13000))");

                entity.Property(e => e.SalaryMother)
                    .HasColumnName("salary_mother")
                    .HasDefaultValueSql("((13000))");

                entity.Property(e => e.StudGroupCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("stud_group_code");

                entity.HasOne(d => d.StudGroupCodeNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.StudGroupCode)
                    .HasConstraintName("FK_students_stud_groups");
            });

            modelBuilder.Entity<StudentsAndAverageSalary>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("students_and_average_salary");

                entity.Property(e => e.Asname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("asname");

                entity.Property(e => e.AverageGroupSalary).HasColumnName("average_group_salary");

                entity.Property(e => e.FacultyName)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("faculty_name");

                entity.Property(e => e.GroupCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("group_code");

                entity.Property(e => e.SpecializationName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("specialization_name");
            });

            modelBuilder.Entity<StudentsAndAverageSalaryWithFunction>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("students_and_average_salary_with_function");

                entity.Property(e => e.AverageGroupSalary).HasColumnName("average_group_salary");

                entity.Property(e => e.FacultyName)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("faculty_name");

                entity.Property(e => e.GroupCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("group_code");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.SpecializationName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("specialization_name");

                entity.Property(e => e.Zachetka).HasColumnName("zachetka");
            });

            modelBuilder.Entity<UniversityAudit>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("universityAudit");

                entity.Property(e => e.EditBy)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(original_login())");

                entity.Property(e => e.EventDateTime)
                    .HasColumnName("eventDateTime")
                    .HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.EventName)
                    .HasMaxLength(1000)
                    .IsUnicode(false)
                    .HasColumnName("eventName");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
