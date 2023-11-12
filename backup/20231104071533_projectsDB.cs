/*using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Munkanaplo2.Data.Migrations
{
    /// <inheritdoc />
    public partial class projectsDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembership_ProjectModel_ProjectId",
                table: "ProjectMembership");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembership",
                table: "ProjectMembership");

            migrationBuilder.RenameTable(
                name: "ProjectMembership",
                newName: "ProjectMemberships");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMembership_ProjectId",
                table: "ProjectMemberships",
                newName: "IX_ProjectMemberships_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMemberships",
                table: "ProjectMemberships",
                column: "ProjectMembershipId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMemberships_ProjectModel_ProjectId",
                table: "ProjectMemberships",
                column: "ProjectId",
                principalTable: "ProjectModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMemberships_ProjectModel_ProjectId",
                table: "ProjectMemberships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMemberships",
                table: "ProjectMemberships");

            migrationBuilder.RenameTable(
                name: "ProjectMemberships",
                newName: "ProjectMembership");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMemberships_ProjectId",
                table: "ProjectMembership",
                newName: "IX_ProjectMembership_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMembership",
                table: "ProjectMembership",
                column: "ProjectMembershipId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembership_ProjectModel_ProjectId",
                table: "ProjectMembership",
                column: "ProjectId",
                principalTable: "ProjectModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
*/