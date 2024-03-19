using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserAndBookLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BookAuthor",
                table: "BookAuthor");

            migrationBuilder.DropIndex(
                name: "IX_BookAuthor_BookId",
                table: "BookAuthor");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Publisher");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Publisher");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Author");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Author");

            migrationBuilder.RenameColumn(
                name: "UpdatedByName",
                table: "Publisher",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedByName",
                table: "Publisher",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByName",
                table: "Book",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedByName",
                table: "Book",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByName",
                table: "Author",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedByName",
                table: "Author",
                newName: "CreatedByUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookAuthor",
                table: "BookAuthor",
                columns: new[] { "BookId", "AuthorId" });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UserName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    HashedPassword = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    UpdatedByUserId = table.Column<string>(type: "text", nullable: true),
                    VersionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookLike",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LikedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookLike", x => new { x.BookId, x.UserId });
                    table.ForeignKey(
                        name: "FK_BookLike_Book_BookId",
                        column: x => x.BookId,
                        principalTable: "Book",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookLike_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthor_AuthorId",
                table: "BookAuthor",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_BookLike_UserId",
                table: "BookLike",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserId",
                table: "User",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookLike");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookAuthor",
                table: "BookAuthor");

            migrationBuilder.DropIndex(
                name: "IX_BookAuthor_AuthorId",
                table: "BookAuthor");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "Publisher",
                newName: "UpdatedByName");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Publisher",
                newName: "CreatedByName");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "Book",
                newName: "UpdatedByName");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Book",
                newName: "CreatedByName");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "Author",
                newName: "UpdatedByName");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Author",
                newName: "CreatedByName");

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Publisher",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "Publisher",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Book",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "Book",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Author",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "Author",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookAuthor",
                table: "BookAuthor",
                columns: new[] { "AuthorId", "BookId" });

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthor_BookId",
                table: "BookAuthor",
                column: "BookId");
        }
    }
}
