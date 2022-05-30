using System;
using System.Collections.Generic;
using AshLake.Services.YandeStore.Domain.Posts;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AshLake.Services.YandeStore.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:post_rating", "safe,questionable,explicit")
                .Annotation("Npgsql:Enum:post_status", "active,pending,flagged,deleted");

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Author = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FileExt = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: true),
                    HasChildren = table.Column<bool>(type: "boolean", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Md5 = table.Column<string>(type: "character(32)", fixedLength: true, maxLength: 32, nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Rating = table.Column<PostRating>(type: "post_rating", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<PostStatus>(type: "post_status", nullable: false),
                    Tags = table.Column<List<string>>(type: "character varying[]", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_FileExt",
                table: "Post",
                column: "FileExt");

            migrationBuilder.CreateIndex(
                name: "IX_Post_FileSize",
                table: "Post",
                column: "FileSize");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Height",
                table: "Post",
                column: "Height");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ParentId",
                table: "Post",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Score",
                table: "Post",
                column: "Score");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Tags",
                table: "Post",
                column: "Tags")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Width",
                table: "Post",
                column: "Width");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Post");
        }
    }
}
