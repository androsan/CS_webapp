using Microsoft.EntityFrameworkCore.Migrations;

namespace CS_webapp.Migrations
{
    public partial class addPodatkiToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
 
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastDate = table.Column<string>(type: "nvarchar(max)", nullable: true),

                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    /*
                    table.ForeignKey(
                        name: "FK_Podatki_Category_Id",
                        column: x => x.Id,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    */
                });

            migrationBuilder.CreateTable(
                name: "Podatki",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Secret = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Podatki", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Podatki_Category_Id",
                        column: x => x.Id,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropTable(
                name: "Podatki");
            */

         
        }
    }
}
