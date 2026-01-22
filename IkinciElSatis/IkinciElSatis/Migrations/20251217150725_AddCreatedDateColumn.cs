using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IkinciElSatis.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedDateColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteItems_FavoriteLists_FavoriteListId",
                table: "FavoriteItems");

            migrationBuilder.DropTable(
                name: "FavoriteLists");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteItems_FavoriteListId",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "FavoriteListId",
                table: "FavoriteItems");

            migrationBuilder.RenameColumn(
                name: "AddedDate",
                table: "FavoriteItems",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FavoriteItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteItems_UserId",
                table: "FavoriteItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteItems_AspNetUsers_UserId",
                table: "FavoriteItems",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteItems_AspNetUsers_UserId",
                table: "FavoriteItems");

            migrationBuilder.DropIndex(
                name: "IX_FavoriteItems_UserId",
                table: "FavoriteItems");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FavoriteItems");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "FavoriteItems",
                newName: "AddedDate");

            migrationBuilder.AddColumn<int>(
                name: "FavoriteListId",
                table: "FavoriteItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FavoriteLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteLists_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteItems_FavoriteListId",
                table: "FavoriteItems",
                column: "FavoriteListId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteLists_UserId",
                table: "FavoriteLists",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteItems_FavoriteLists_FavoriteListId",
                table: "FavoriteItems",
                column: "FavoriteListId",
                principalTable: "FavoriteLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
