using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814

namespace Gestion_Quirurgica.Migrations
{
    /// <inheritdoc />
    public partial class IntegracionDepartamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Tabla: insumos_quirurgicos (Logística) ────────────────────────
            migrationBuilder.CreateTable(
                name: "insumos_quirurgicos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo          = table.Column<string>(type: "character varying(20)",  maxLength: 20,  nullable: false),
                    nombre          = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    unidad_medida   = table.Column<string>(type: "character varying(20)",  maxLength: 20,  nullable: false, defaultValue: ""),
                    cantidad_minima = table.Column<int>(type: "integer", nullable: false),
                    cantidad_actual = table.Column<int>(type: "integer", nullable: false),
                    estado          = table.Column<string>(type: "character varying(10)",  maxLength: 10,  nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_insumos_quirurgicos", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_insumos_quirurgicos_codigo",
                table: "insumos_quirurgicos",
                column: "codigo",
                unique: true);

            // ── Tabla: registros_fallecimiento (Gestión Legal) ────────────────
            migrationBuilder.CreateTable(
                name: "registros_fallecimiento",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo                   = table.Column<string>(type: "character varying(20)",  maxLength: 20, nullable: false),
                    procedimiento_id         = table.Column<int>(type: "integer",                   nullable: false),
                    fecha_hora_fallecimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    causa_fallecimiento      = table.Column<string>(type: "text",                   nullable: false, defaultValue: ""),
                    usuario_registra         = table.Column<string>(type: "character varying(50)",  maxLength: 50, nullable: false, defaultValue: ""),
                    numero_acta_legal        = table.Column<string>(type: "character varying(50)",  maxLength: 50, nullable: true),
                    estado_legal             = table.Column<string>(type: "character varying(20)",  maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    estado                   = table.Column<string>(type: "character varying(10)",  maxLength: 10, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registros_fallecimiento", x => x.id);
                    table.ForeignKey(
                        name: "FK_registros_fallecimiento_procedimientos_quirurgicos_procedimiento_id",
                        column: x => x.procedimiento_id,
                        principalTable: "procedimientos_quirurgicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_registros_fallecimiento_codigo",
                table: "registros_fallecimiento",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_registros_fallecimiento_procedimiento_id",
                table: "registros_fallecimiento",
                column: "procedimiento_id");

            // ── Tabla: programaciones_cirugia (POST médicos) ──────────────────
            migrationBuilder.CreateTable(
                name: "programaciones_cirugia",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo                    = table.Column<string>(type: "character varying(20)",  maxLength: 20,  nullable: false),
                    paciente_id               = table.Column<int>(type: "integer",                   nullable: false),
                    sala_id                   = table.Column<int>(type: "integer",                   nullable: false),
                    cirujano_solicitante_id   = table.Column<int>(type: "integer",                   nullable: false),
                    fecha_solicitada          = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hora_inicio_estimada      = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hora_fin_estimada         = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tipo_cirugia              = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: ""),
                    prioridad                 = table.Column<string>(type: "character varying(15)",  maxLength: 15,  nullable: false, defaultValue: ""),
                    observaciones             = table.Column<string>(type: "text",                   nullable: false, defaultValue: ""),
                    estado_programacion       = table.Column<string>(type: "character varying(20)",  maxLength: 20,  nullable: false, defaultValue: "Solicitada"),
                    estado                    = table.Column<string>(type: "character varying(10)",  maxLength: 10,  nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_programaciones_cirugia", x => x.id);
                    table.ForeignKey(
                        name: "FK_programaciones_cirugia_pacientes_paciente_id",
                        column: x => x.paciente_id,
                        principalTable: "pacientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_programaciones_cirugia_salas_quirurgicas_sala_id",
                        column: x => x.sala_id,
                        principalTable: "salas_quirurgicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_programaciones_cirugia_cirujanos_cirujano_solicitante_id",
                        column: x => x.cirujano_solicitante_id,
                        principalTable: "cirujanos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_programaciones_cirugia_codigo",
                table: "programaciones_cirugia",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_programaciones_cirugia_paciente_id",
                table: "programaciones_cirugia",
                column: "paciente_id");

            migrationBuilder.CreateIndex(
                name: "IX_programaciones_cirugia_sala_id",
                table: "programaciones_cirugia",
                column: "sala_id");

            migrationBuilder.CreateIndex(
                name: "IX_programaciones_cirugia_cirujano_solicitante_id",
                table: "programaciones_cirugia",
                column: "cirujano_solicitante_id");

            // ── Seed data para insumos_quirurgicos ────────────────────────────
            migrationBuilder.InsertData(
                table: "insumos_quirurgicos",
                columns: new[] { "id", "codigo", "nombre", "unidad_medida", "cantidad_minima", "cantidad_actual", "estado" },
                values: new object[,]
                {
                    { 1, "INS-001", "Vendas estériles",      "unidad",  10,  5, "Activo" },
                    { 2, "INS-002", "Paquetes de sangre O+", "paquete",  2,  2, "Activo" },
                    { 3, "INS-003", "Escalpelos",            "unidad",   5,  3, "Activo" },
                    { 4, "INS-004", "Guantes quirúrgicos",   "par",     20, 20, "Activo" },
                    { 5, "INS-005", "Suturas absorbibles",   "caja",     4,  1, "Activo" },
                    { 6, "INS-006", "Solución salina 500ml", "frasco",   8,  8, "Activo" },
                    { 7, "INS-007", "Gasas estériles 10x10", "paquete", 15,  6, "Activo" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "programaciones_cirugia");
            migrationBuilder.DropTable(name: "registros_fallecimiento");
            migrationBuilder.DropTable(name: "insumos_quirurgicos");
        }
    }
}
