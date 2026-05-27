using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTracker.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Projetos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", 1),
                    Nome = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    MotorJogo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    PlataformasAlvo = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true),
                    VersaoAtual = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projetos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", 1),
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "longtext", nullable: false),
                    Perfil = table.Column<string>(type: "longtext", nullable: false),
                    AceitouTermos = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AceitouTermosEm = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Bugs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", 1),
                    Titulo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    Severidade = table.Column<string>(type: "longtext", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false),
                    TipoBug = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Plataforma = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    VersaoJogo = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    NumeroBuild = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    Milestone = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Cena = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    FrequenciaReproducao = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    BloqueiaLancamento = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ResultadoEsperado = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    ResultadoObtido = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    DetalhesAmbiente = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ProjetoId = table.Column<int>(type: "int", nullable: false),
                    ReportadoPorId = table.Column<int>(type: "int", nullable: false),
                    AtribuidoParaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bugs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bugs_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bugs_Usuarios_AtribuidoParaId",
                        column: x => x.AtribuidoParaId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Bugs_Usuarios_ReportadoPorId",
                        column: x => x.ReportadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", 1),
                    Texto = table.Column<string>(type: "longtext", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    BugId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comentarios_Bugs_BugId",
                        column: x => x.BugId,
                        principalTable: "Bugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comentarios_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(name: "IX_Bugs_AtribuidoParaId", table: "Bugs", column: "AtribuidoParaId");
            migrationBuilder.CreateIndex(name: "IX_Bugs_ProjetoId", table: "Bugs", column: "ProjetoId");
            migrationBuilder.CreateIndex(name: "IX_Bugs_ReportadoPorId", table: "Bugs", column: "ReportadoPorId");
            migrationBuilder.CreateIndex(name: "IX_Comentarios_BugId", table: "Comentarios", column: "BugId");
            migrationBuilder.CreateIndex(name: "IX_Comentarios_UsuarioId", table: "Comentarios", column: "UsuarioId");
            migrationBuilder.CreateIndex(name: "IX_Usuarios_Email", table: "Usuarios", column: "Email", unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Comentarios");
            migrationBuilder.DropTable(name: "Bugs");
            migrationBuilder.DropTable(name: "Usuarios");
            migrationBuilder.DropTable(name: "Projetos");
        }
    }
}
