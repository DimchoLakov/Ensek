using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ensek.Meters.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueMeterReadAndDateTimeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_MeterRead_MeterReadingDateTime",
                table: "MeterReadings",
                columns: new[] { "MeterRead", "MeterReadingDateTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MeterReadings_MeterRead_MeterReadingDateTime",
                table: "MeterReadings");
        }
    }
}
