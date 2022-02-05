using Microsoft.EntityFrameworkCore.Migrations;

namespace CS_webapp.Migrations
{
    public partial class addCategoryToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    
                    DisplayOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastDate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
