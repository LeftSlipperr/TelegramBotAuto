using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Data.Migrations
{
    /// <inheritdoc />
    public partial class chatID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "iImageUrl",
                table: "Auto",
                newName: "ImageUrl");

            migrationBuilder.AddColumn<long>(
                name: "chatId",
                table: "Person",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "chatId",
                table: "Auto",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "chatId",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "chatId",
                table: "Auto");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Auto",
                newName: "iImageUrl");
        }
    }
}
