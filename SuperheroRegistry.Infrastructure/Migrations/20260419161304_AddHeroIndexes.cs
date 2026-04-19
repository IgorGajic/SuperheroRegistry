using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuperheroRegistry.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHeroIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Powers");

            migrationBuilder.DropTable(
                name: "Heroes");

            migrationBuilder.CreateTable(
                name: "HeroeEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Codename = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OriginStory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Race = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alignment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroeEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroeEntities_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PowerEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeroId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerEntities_HeroeEntities_HeroId",
                        column: x => x.HeroId,
                        principalTable: "HeroeEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeroEntity_Codename",
                table: "HeroeEntities",
                column: "Codename");

            migrationBuilder.CreateIndex(
                name: "IX_HeroEntity_Status",
                table: "HeroeEntities",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_HeroEntity_UserId",
                table: "HeroeEntities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerEntities_HeroId",
                table: "PowerEntities",
                column: "HeroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PowerEntities");

            migrationBuilder.DropTable(
                name: "HeroeEntities");

            migrationBuilder.CreateTable(
                name: "Heroes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alignment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Codename = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OriginStory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Race = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heroes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Heroes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Powers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeroId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Powers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Powers_Heroes_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Heroes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Heroes_UserId",
                table: "Heroes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Powers_HeroId",
                table: "Powers",
                column: "HeroId");
        }
    }
}
