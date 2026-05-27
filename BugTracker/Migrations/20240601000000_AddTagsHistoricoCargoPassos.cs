using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTracker.Migrations
{
    public partial class AddTagsHistoricoCargoPassos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Cargo em Usuarios
            migrationBuilder.AddColumn<string>(
                name: "Cargo",
                table: "Usuarios",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Programador");

            // PassosReproducao em Bugs
            migrationBuilder.AddColumn<string>(
                name: "PassosReproducao",
                table: "Bugs",
                type: "longtext",
                nullable: true);

            // Tags
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", 1),
                    Nome = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Cor = table.Column<string>(type: "varchar(7)", maxLength: 7, nullable: false),
                    Departamento = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // BugTags (junction)
            migrationBuilder.CreateTable(
                name: "BugTags",
                columns: table => new
                {
                    BugId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugTags", x => new { x.BugId, x.TagId });
                    table.ForeignKey(
                        name: "FK_BugTags_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BugTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // BugHistoricos
            migrationBuilder.CreateTable(
                name: "BugHistoricos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", 1),
                    BugId = table.Column<int>(type: "int", nullable: false),
                    StatusAnterior = table.Column<string>(type: "longtext", nullable: false),
                    StatusNovo = table.Column<string>(type: "longtext", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugHistoricos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BugHistoricos_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BugHistoricos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(name: "IX_BugTags_TagId", table: "BugTags", column: "TagId");
            migrationBuilder.CreateIndex(name: "IX_BugHistoricos_BugId", table: "BugHistoricos", column: "BugId");
            migrationBuilder.CreateIndex(name: "IX_BugHistoricos_UsuarioId", table: "BugHistoricos", column: "UsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "BugHistoricos");
            migrationBuilder.DropTable(name: "BugTags");
            migrationBuilder.DropTable(name: "Tags");
            migrationBuilder.DropColumn(name: "PassosReproducao", table: "Bugs");
            migrationBuilder.DropColumn(name: "Cargo", table: "Usuarios");
        }
    }
}
