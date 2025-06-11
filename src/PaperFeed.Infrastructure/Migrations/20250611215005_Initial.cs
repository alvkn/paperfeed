using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaperFeed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostedImages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    OrderedId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostedImages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostedImages_OrderedId",
                table: "PostedImages",
                column: "OrderedId",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostedImages");
        }
    }
}
