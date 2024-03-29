﻿// <auto-generated />
using System;
using System.Collections.Generic;
using AshLake.Services.YandeStore.Domain.Posts;
using AshLake.Services.YandeStore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AshLake.Services.YandeStore.Infrastructure.Migrations
{
    [DbContext(typeof(YandeDbContext))]
    partial class YandeDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "post_rating", new[] { "safe", "questionable", "explicit" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "post_status", new[] { "active", "pending", "flagged", "deleted" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AshLake.Services.YandeStore.Domain.Posts.Post", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<string>("Author")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FileExt")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("character varying(4)");

                    b.Property<long>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("FileUrl")
                        .HasColumnType("text");

                    b.Property<bool>("HasChildren")
                        .HasColumnType("boolean");

                    b.Property<int>("Height")
                        .HasColumnType("integer");

                    b.Property<string>("Md5")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character(32)")
                        .IsFixedLength();

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<PostRating>("Rating")
                        .HasColumnType("post_rating");

                    b.Property<int>("Score")
                        .HasColumnType("integer");

                    b.Property<string>("Source")
                        .HasColumnType("text");

                    b.Property<PostStatus>("Status")
                        .HasColumnType("post_status");

                    b.Property<List<string>>("Tags")
                        .IsRequired()
                        .HasColumnType("character varying[]");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FileExt");

                    b.HasIndex("FileSize");

                    b.HasIndex("Height");

                    b.HasIndex("ParentId");

                    b.HasIndex("Rating");

                    b.HasIndex("Score");

                    b.HasIndex("Status");

                    b.HasIndex("Tags");

                    NpgsqlIndexBuilderExtensions.HasMethod(b.HasIndex("Tags"), "gin");

                    b.HasIndex("Width");

                    b.ToTable("Post", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
