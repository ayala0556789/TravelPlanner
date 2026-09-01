using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiAndWeatherFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChildrenAges",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PreferAccessiblePaths",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BudgetLevel",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IncludeBaby",
                table: "Trips",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "PredictedTemperature",
                table: "Trips",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresAccessibility",
                table: "Trips",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SelectedParticipants",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeatherCondition",
                table: "Trips",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChildrenAges",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PreferAccessiblePaths",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BudgetLevel",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IncludeBaby",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "PredictedTemperature",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "RequiresAccessibility",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "SelectedParticipants",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "WeatherCondition",
                table: "Trips");
        }
    }
}
