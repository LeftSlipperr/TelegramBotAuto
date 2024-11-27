using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot.Data.Migrations
{
    /// <inheritdoc />
    public partial class AutoDataChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""Auto"" 
      ALTER COLUMN ""YearofIssue"" TYPE integer 
      USING ""YearofIssue""::integer");


            migrationBuilder.Sql(
                @"ALTER TABLE ""Auto"" 
      ALTER COLUMN ""SeatInTheCabin"" TYPE integer 
      USING ""SeatInTheCabin""::integer");


            migrationBuilder.Sql(
                @"ALTER TABLE ""Auto"" 
      ALTER COLUMN ""EngineSize"" TYPE integer 
      USING ""EngineSize""::integer");

            migrationBuilder.AddColumn<string>(
                name: "iImageUrl",
                table: "Auto",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "iImageUrl",
                table: "Auto");

            migrationBuilder.AlterColumn<string>(
                name: "YearofIssue",
                table: "Auto",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "SeatInTheCabin",
                table: "Auto",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "EngineSize",
                table: "Auto",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
