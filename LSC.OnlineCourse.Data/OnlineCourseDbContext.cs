using System;
using System.Collections.Generic;
using LSC.OnlineCourse.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LSC.OnlineCourse.Data.Entities;

// to use EF Core, we inherit from DbContext class
public partial class OnlineCourseDbContext : DbContext
{
    public OnlineCourseDbContext()
    {
    }

    //this option is used to pass sql connection string details from the program.cs DI
    public OnlineCourseDbContext(DbContextOptions<OnlineCourseDbContext> options)
        : base(options)
    {
    }

    //Every table in database needs to be configured as DbSet<T> where T denotes the table.
    //we did all 10 tables from databse so that we can use it in our application
    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseCategory> CourseCategories { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Instructor> Instructors { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SessionDetail> SessionDetails { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    //All the table definitions are configured here based on our DB
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK_Course_CourseId");

            entity.ToTable("Course");

            entity.Property(e => e.CourseType).HasMaxLength(10);
            entity.Property(e => e.Duration).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Course_CourseCategory");

            entity.HasOne(d => d.Instructor).WithMany(p => p.Courses)
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Course_Instructor");
        });

        //These helps EF Core to know where what we try to insert/update is valid or not. e.g. if you try to insert 
        //a record with CategoryName more than 50 characters, it will throw error instead of going to DB and come back with error
        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK_CourseCategory_CategoryId");

            entity.ToTable("CourseCategory");

            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(250);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK_Enrollment_EnrollmentId");

            entity.ToTable("Enrollment");

            entity.Property(e => e.EnrollmentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentStatus).HasMaxLength(20);

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollment_Course");

            entity.HasOne(d => d.User).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollment_UserProfile");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.HasKey(e => e.InstructorId).HasName("PK_Instructor_InstructorId");

            entity.ToTable("Instructor");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Instructors)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Instructor_UserProfile");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK_Payment_PaymentId");

            entity.ToTable("Payment");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(20);

            entity.HasOne(d => d.Enrollment).WithMany(p => p.Payments)
                .HasForeignKey(d => d.EnrollmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Enrollment");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK_Review_ReviewId");

            entity.ToTable("Review");

            entity.Property(e => e.ReviewDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Course).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_Course");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_UserProfile");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK_Roles_RoleId");

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<SessionDetail>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK_SessionDetails_SessionId");

            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.VideoUrl).HasMaxLength(500);

            entity.HasOne(d => d.Course).WithMany(p => p.SessionDetails)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SessionDetails_Course");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_UserProfile_UserId");

            entity.ToTable("UserProfile");

            entity.Property(e => e.AdObjId).HasMaxLength(128);
            entity.Property(e => e.DisplayName)
                .HasMaxLength(100)
                .HasDefaultValue("Guest");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK_UserRole_UserRoleId");

            entity.ToTable("UserRole");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_UserProfile");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
