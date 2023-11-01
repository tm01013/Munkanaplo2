using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Munkanaplo2.Data.Migrations
{
    /// <inheritdoc />
    public partial class initialsetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JobTitle = table.Column<string>(type: "TEXT", nullable: false),
                    JobDescription = table.Column<string>(type: "TEXT", nullable: false),
                    JobOwner = table.Column<string>(type: "TEXT", nullable: false),
                    JobCreator = table.Column<string>(type: "TEXT", nullable: false),
                    CreationDate = table.Column<string>(type: "TEXT", nullable: false),
                    JobStatus = table.Column<string>(type: "TEXT", nullable: false),
                    FinishDate = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobModel", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobModel");
        }
    }
}
