using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    public partial class CSIDTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CSID",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    branchId = table.Column<int>(type: "int", nullable: false),
                    cSR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cSIDKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    privateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    publicKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    secretNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    error_ar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    error_en = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    otp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sucess = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CSID", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CSID_GLBranch_branchId",
                        column: x => x.branchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CSID_branchId",
                table: "CSID",
                column: "branchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CSID");
        }
    }
}
