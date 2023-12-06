using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Munkanaplo2.Data.Migrations
{
    /// <inheritdoc />
    public partial class subtaskDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubTaskModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TaskTitle = table.Column<string>(type: "TEXT", nullable: false),
                    TaskDetails = table.Column<string>(type: "TEXT", nullable: false),
                    JobId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskCreator = table.Column<string>(type: "TEXT", nullable: false),
                    TaskCreationDate = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubTaskModel", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubTaskModel");
        }
    }
}
