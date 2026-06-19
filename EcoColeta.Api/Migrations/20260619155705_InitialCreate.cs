using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoColeta.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DestinacoesResiduos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoResiduo = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InstrucoesDescarte = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Reciclavel = table.Column<bool>(type: "bit", nullable: false),
                    RiscoAmbiental = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinacoesResiduos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PontosColeta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    TipoResiduoAceito = table.Column<int>(type: "int", nullable: false),
                    CapacidadeMaximaKg = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    OcupacaoAtualKg = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PontosColeta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlertasColeta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PontoColetaId = table.Column<int>(type: "int", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Resolvido = table.Column<bool>(type: "bit", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvidoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertasColeta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertasColeta_PontosColeta_PontoColetaId",
                        column: x => x.PontoColetaId,
                        principalTable: "PontosColeta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosResiduos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PontoColetaId = table.Column<int>(type: "int", nullable: false),
                    TipoResiduo = table.Column<int>(type: "int", nullable: false),
                    PesoKg = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Origem = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RegistradoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosResiduos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosResiduos_PontosColeta_PontoColetaId",
                        column: x => x.PontoColetaId,
                        principalTable: "PontosColeta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertasColeta_PontoColetaId",
                table: "AlertasColeta",
                column: "PontoColetaId");

            migrationBuilder.CreateIndex(
                name: "IX_DestinacoesResiduos_TipoResiduo",
                table: "DestinacoesResiduos",
                column: "TipoResiduo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PontosColeta_Cidade",
                table: "PontosColeta",
                column: "Cidade");

            migrationBuilder.CreateIndex(
                name: "IX_PontosColeta_TipoResiduoAceito",
                table: "PontosColeta",
                column: "TipoResiduoAceito");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosResiduos_PontoColetaId",
                table: "RegistrosResiduos",
                column: "PontoColetaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSistema_Email",
                table: "UsuariosSistema",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertasColeta");

            migrationBuilder.DropTable(
                name: "DestinacoesResiduos");

            migrationBuilder.DropTable(
                name: "RegistrosResiduos");

            migrationBuilder.DropTable(
                name: "UsuariosSistema");

            migrationBuilder.DropTable(
                name: "PontosColeta");
        }
    }
}
