using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByID = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedByID = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedByName = table.Column<string>(type: "TEXT", nullable: true),
                    VersionID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Publisher",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByID = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedByID = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedByName = table.Column<string>(type: "TEXT", nullable: true),
                    VersionID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publisher", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Book",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ISBN = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PublisherID = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByID = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedByID = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedByName = table.Column<string>(type: "TEXT", nullable: true),
                    VersionID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Book", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Book_Publisher_PublisherID",
                        column: x => x.PublisherID,
                        principalTable: "Publisher",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookAuthor",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookID = table.Column<int>(type: "INTEGER", nullable: false),
                    AuthorID = table.Column<int>(type: "INTEGER", nullable: false),
                    BookDataModelID = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookAuthor", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BookAuthor_Author_AuthorID",
                        column: x => x.AuthorID,
                        principalTable: "Author",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookAuthor_Book_BookDataModelID",
                        column: x => x.BookDataModelID,
                        principalTable: "Book",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_BookAuthor_Book_BookID",
                        column: x => x.BookID,
                        principalTable: "Book",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Author_Name",
                table: "Author",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Book_ISBN",
                table: "Book",
                column: "ISBN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Book_PublisherID",
                table: "Book",
                column: "PublisherID");

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthor_AuthorID",
                table: "BookAuthor",
                column: "AuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthor_BookDataModelID",
                table: "BookAuthor",
                column: "BookDataModelID");

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthor_BookID",
                table: "BookAuthor",
                column: "BookID");

            migrationBuilder.CreateIndex(
                name: "IX_Publisher_Name",
                table: "Publisher",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookAuthor");

            migrationBuilder.DropTable(
                name: "Author");

            migrationBuilder.DropTable(
                name: "Book");

            migrationBuilder.DropTable(
                name: "Publisher");
        }
    }
}
