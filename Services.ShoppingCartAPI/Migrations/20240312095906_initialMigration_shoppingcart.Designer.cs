﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Services.ShoppingCartAPI.Data;

#nullable disable

namespace Services.ShoppingCartAPI.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20240312095906_initialMigration_shoppingcart")]
    partial class initialMigration_shoppingcart
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Services.ShoppingCartAPI.Models.CartDetails", b =>
                {
                    b.Property<int>("CartDetailsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartDetailsId"));

                    b.Property<int>("CartHeaderId")
                        .HasColumnType("int");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("CartDetailsId");

                    b.HasIndex("CartHeaderId");

                    b.ToTable("CartDetails");
                });

            modelBuilder.Entity("Services.ShoppingCartAPI.Models.CartHeader", b =>
                {
                    b.Property<int>("CartHearderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartHearderId"));

                    b.Property<string>("CouponCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CartHearderId");

                    b.ToTable("CartHeaders");
                });

            modelBuilder.Entity("Services.ShoppingCartAPI.Models.CartDetails", b =>
                {
                    b.HasOne("Services.ShoppingCartAPI.Models.CartHeader", "CartHeader")
                        .WithMany()
                        .HasForeignKey("CartHeaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CartHeader");
                });
#pragma warning restore 612, 618
        }
    }
}
