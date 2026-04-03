using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TaskManagementApi.Api.Data;

#nullable disable

namespace TaskManagementApi.Api.Migrations;

[DbContext(typeof(ApplicationDbContext))]
public partial class ApplicationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.23");

        modelBuilder.Entity("TaskManagementApi.Api.Entities.TaskItem", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("TEXT");

            b.Property<DateTime>("CreatedAt")
                .HasColumnType("TEXT");

            b.Property<string>("Description")
                .HasMaxLength(1000)
                .HasColumnType("TEXT");

            b.Property<DateTime>("DueDate")
                .HasColumnType("TEXT");

            b.Property<int>("Priority")
                .HasColumnType("INTEGER");

            b.Property<int>("Status")
                .HasColumnType("INTEGER");

            b.Property<string>("Title")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("TEXT");

            b.Property<DateTime>("UpdatedAt")
                .HasColumnType("TEXT");

            b.HasKey("Id");

            b.ToTable("Tasks");
        });
    }
}
