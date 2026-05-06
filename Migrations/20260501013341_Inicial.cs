using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Gestion_Quirurgica.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cirujanos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    especialidad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    anos_experiencia = table.Column<int>(type: "integer", nullable: false),
                    estado = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cirujanos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "equipos_medicos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fabricante = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ultimo_mantenimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    estado_operativo = table.Column<bool>(type: "boolean", nullable: false),
                    estado = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipos_medicos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pacientes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    edad = table.Column<int>(type: "integer", nullable: false),
                    diagnostico_pre = table.Column<string>(type: "text", nullable: false),
                    alergias = table.Column<string>(type: "text", nullable: false),
                    grupo_sanguineo = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    estado = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pacientes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "salas_quirurgicas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    numero_sala = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    nivel_esterilidad = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    disponible = table.Column<bool>(type: "boolean", nullable: false),
                    ultima_limpieza = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    estado = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salas_quirurgicas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "procedimientos_quirurgicos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    paciente_id = table.Column<int>(type: "integer", nullable: false),
                    sala_id = table.Column<int>(type: "integer", nullable: false),
                    hora_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hora_fin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    estado_procedimiento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    prioridad = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    estado = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_procedimientos_quirurgicos", x => x.id);
                    table.ForeignKey(
                        name: "FK_procedimientos_quirurgicos_pacientes_paciente_id",
                        column: x => x.paciente_id,
                        principalTable: "pacientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_procedimientos_quirurgicos_salas_quirurgicas_sala_id",
                        column: x => x.sala_id,
                        principalTable: "salas_quirurgicas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "procedimientos_equipo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    procedimiento_id = table.Column<int>(type: "integer", nullable: false),
                    equipo_id = table.Column<int>(type: "integer", nullable: false),
                    estado_uso = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    observaciones = table.Column<string>(type: "text", nullable: false),
                    estado = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_procedimientos_equipo", x => x.id);
                    table.ForeignKey(
                        name: "FK_procedimientos_equipo_equipos_medicos_equipo_id",
                        column: x => x.equipo_id,
                        principalTable: "equipos_medicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_procedimientos_equipo_procedimientos_quirurgicos_procedimie~",
                        column: x => x.procedimiento_id,
                        principalTable: "procedimientos_quirurgicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "roles_quirurgicos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    procedimiento_id = table.Column<int>(type: "integer", nullable: false),
                    cirujano_id = table.Column<int>(type: "integer", nullable: false),
                    rol = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    estado = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles_quirurgicos", x => x.id);
                    table.ForeignKey(
                        name: "FK_roles_quirurgicos_cirujanos_cirujano_id",
                        column: x => x.cirujano_id,
                        principalTable: "cirujanos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_roles_quirurgicos_procedimientos_quirurgicos_procedimiento_~",
                        column: x => x.procedimiento_id,
                        principalTable: "procedimientos_quirurgicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "timeline_incidencias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    procedimiento_id = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    usuario_registra = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    estado = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timeline_incidencias", x => x.id);
                    table.ForeignKey(
                        name: "FK_timeline_incidencias_procedimientos_quirurgicos_procedimien~",
                        column: x => x.procedimiento_id,
                        principalTable: "procedimientos_quirurgicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "cirujanos",
                columns: new[] { "id", "anos_experiencia", "codigo", "especialidad", "estado", "nombre" },
                values: new object[,]
                {
                    { 1, 15, "CIR-001", "Cirugía General", "Activo", "Dr. Roberto Vega" },
                    { 2, 10, "CIR-002", "Anestesiología", "Activo", "Dra. Ana Condori" },
                    { 3, 8, "CIR-003", "Cirugía Laparoscópica", "Activo", "Dr. Luis Quispe" },
                    { 4, 12, "CIR-004", "Traumatología", "Activo", "Dra. Patricia Mamani" },
                    { 5, 20, "CIR-005", "Oncología Quirúrgica", "Activo", "Dr. Fernando Soria" },
                    { 6, 6, "CIR-006", "Anestesiología", "Activo", "Dra. Carmen Flores" }
                });

            migrationBuilder.InsertData(
                table: "equipos_medicos",
                columns: new[] { "id", "codigo", "estado", "estado_operativo", "fabricante", "nombre", "ultimo_mantenimiento" },
                values: new object[,]
                {
                    { 1, "EQ-001", "Activo", true, "Bovie Medical", "Bisturí Eléctrico", new DateTime(2024, 12, 16, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "EQ-002", "Activo", true, "Mindray", "Monitor de Signos", new DateTime(2024, 12, 31, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "EQ-003", "Activo", true, "Stryker", "Laparoscopio HD", new DateTime(2025, 1, 8, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "EQ-004", "Activo", false, "Dräger", "Respirador UCI", new DateTime(2024, 12, 1, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, "EQ-005", "Activo", true, "Maquet", "Mesa de Operaciones", new DateTime(2024, 11, 16, 8, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "pacientes",
                columns: new[] { "id", "alergias", "codigo", "diagnostico_pre", "edad", "estado", "grupo_sanguineo", "nombre" },
                values: new object[,]
                {
                    { 1, "Penicilina", "PAC-001", "Apendicitis aguda", 45, "Activo", "O+", "Juan Pérez Mamani" },
                    { 2, "Ninguna", "PAC-002", "Colecistitis crónica", 32, "Activo", "A+", "María López Flores" },
                    { 3, "Ibuprofeno", "PAC-003", "Hernia inguinal derecha", 58, "Activo", "B-", "Carlos Ruiz Torrez" },
                    { 4, "Ninguna", "PAC-004", "Fractura de fémur", 27, "Activo", "AB+", "Ana Condori Huanca" },
                    { 5, "Aspirina", "PAC-005", "Tumor gástrico", 61, "Activo", "O-", "Luis Quispe Mamani" }
                });

            migrationBuilder.InsertData(
                table: "salas_quirurgicas",
                columns: new[] { "id", "codigo", "disponible", "estado", "nivel_esterilidad", "numero_sala", "ultima_limpieza" },
                values: new object[,]
                {
                    { 1, "SALA-001", false, "Activo", "Alto", "Q-01", new DateTime(2025, 1, 15, 6, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "SALA-002", true, "Activo", "Medio", "Q-02", new DateTime(2025, 1, 15, 3, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "SALA-003", true, "Activo", "Alto", "Q-03", new DateTime(2025, 1, 15, 7, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "SALA-004", false, "Activo", "Bajo", "Q-04", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "procedimientos_quirurgicos",
                columns: new[] { "id", "codigo", "estado", "estado_procedimiento", "hora_fin", "hora_inicio", "paciente_id", "prioridad", "sala_id" },
                values: new object[,]
                {
                    { 1, "PROC-001", "Activo", "En curso", new DateTime(2025, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), 1, "Alta", 1 },
                    { 2, "PROC-002", "Activo", "Programado", new DateTime(2025, 1, 15, 13, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 15, 11, 0, 0, 0, DateTimeKind.Utc), 2, "Media", 2 },
                    { 3, "PROC-003", "Activo", "Finalizado", new DateTime(2025, 1, 15, 7, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 15, 5, 0, 0, 0, DateTimeKind.Utc), 3, "Baja", 3 },
                    { 4, "PROC-004", "Activo", "Programado", new DateTime(2025, 1, 15, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), 4, "Alta", 4 },
                    { 5, "PROC-005", "Activo", "Finalizado", new DateTime(2025, 1, 14, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 14, 8, 0, 0, 0, DateTimeKind.Utc), 5, "Media", 1 }
                });

            migrationBuilder.InsertData(
                table: "procedimientos_equipo",
                columns: new[] { "id", "codigo", "equipo_id", "estado", "estado_uso", "observaciones", "procedimiento_id" },
                values: new object[,]
                {
                    { 1, "PE-001", 1, "Activo", "En uso", "Modo corte activo", 1 },
                    { 2, "PE-002", 2, "Activo", "En uso", "Monitoreo continuo", 1 },
                    { 3, "PE-003", 3, "Activo", "Reservado", "Reservado para laparoscopia", 2 },
                    { 4, "PE-004", 1, "Activo", "Liberado", "Proceso completado", 3 },
                    { 5, "PE-005", 5, "Activo", "Reservado", "Mesa reservada para trauma", 4 }
                });

            migrationBuilder.InsertData(
                table: "roles_quirurgicos",
                columns: new[] { "id", "cirujano_id", "codigo", "estado", "procedimiento_id", "rol" },
                values: new object[,]
                {
                    { 1, 1, "ROL-001", "Activo", 1, "Cirujano Principal" },
                    { 2, 2, "ROL-002", "Activo", 1, "Anestesiólogo" },
                    { 3, 3, "ROL-003", "Activo", 2, "Cirujano Principal" },
                    { 4, 6, "ROL-004", "Activo", 2, "Anestesiólogo" },
                    { 5, 1, "ROL-005", "Activo", 3, "Cirujano Principal" },
                    { 6, 4, "ROL-006", "Activo", 4, "Cirujano Principal" },
                    { 7, 5, "ROL-007", "Activo", 5, "Cirujano Principal" },
                    { 8, 2, "ROL-008", "Activo", 5, "Anestesiólogo" }
                });

            migrationBuilder.InsertData(
                table: "timeline_incidencias",
                columns: new[] { "id", "codigo", "descripcion", "estado", "procedimiento_id", "timestamp", "tipo", "usuario_registra" },
                values: new object[,]
                {
                    { 1, "TL-001", "Procedimiento iniciado sin novedades", "Activo", 1, new DateTime(2025, 1, 15, 7, 30, 0, 0, DateTimeKind.Utc), "Inicio", "enf.garcia" },
                    { 2, "TL-002", "Presión arterial estable 120/80", "Activo", 1, new DateTime(2025, 1, 15, 7, 45, 0, 0, DateTimeKind.Utc), "Observación", "enf.garcia" },
                    { 3, "TL-003", "Sangrado moderado controlado", "Activo", 1, new DateTime(2025, 1, 15, 7, 55, 0, 0, DateTimeKind.Utc), "Incidencia", "dr.vega" },
                    { 4, "TL-004", "Cirugía de hernia iniciada", "Activo", 3, new DateTime(2025, 1, 15, 5, 0, 0, 0, DateTimeKind.Utc), "Inicio", "enf.morales" },
                    { 5, "TL-005", "Cirugía finalizada exitosamente", "Activo", 3, new DateTime(2025, 1, 15, 7, 0, 0, 0, DateTimeKind.Utc), "Fin", "enf.morales" },
                    { 6, "TL-006", "Resección gástrica parcial iniciada", "Activo", 5, new DateTime(2025, 1, 14, 8, 0, 0, 0, DateTimeKind.Utc), "Inicio", "enf.silva" },
                    { 7, "TL-007", "Resección completada sin complicaciones", "Activo", 5, new DateTime(2025, 1, 14, 11, 0, 0, 0, DateTimeKind.Utc), "Fin", "enf.silva" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_cirujanos_codigo",
                table: "cirujanos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_equipos_medicos_codigo",
                table: "equipos_medicos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pacientes_codigo",
                table: "pacientes",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_procedimientos_equipo_codigo",
                table: "procedimientos_equipo",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_procedimientos_equipo_equipo_id",
                table: "procedimientos_equipo",
                column: "equipo_id");

            migrationBuilder.CreateIndex(
                name: "IX_procedimientos_equipo_procedimiento_id",
                table: "procedimientos_equipo",
                column: "procedimiento_id");

            migrationBuilder.CreateIndex(
                name: "IX_procedimientos_quirurgicos_codigo",
                table: "procedimientos_quirurgicos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_procedimientos_quirurgicos_paciente_id",
                table: "procedimientos_quirurgicos",
                column: "paciente_id");

            migrationBuilder.CreateIndex(
                name: "IX_procedimientos_quirurgicos_sala_id",
                table: "procedimientos_quirurgicos",
                column: "sala_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_quirurgicos_cirujano_id",
                table: "roles_quirurgicos",
                column: "cirujano_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_quirurgicos_codigo",
                table: "roles_quirurgicos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_quirurgicos_procedimiento_id",
                table: "roles_quirurgicos",
                column: "procedimiento_id");

            migrationBuilder.CreateIndex(
                name: "IX_salas_quirurgicas_codigo",
                table: "salas_quirurgicas",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_salas_quirurgicas_numero_sala",
                table: "salas_quirurgicas",
                column: "numero_sala",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_timeline_incidencias_codigo",
                table: "timeline_incidencias",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_timeline_incidencias_procedimiento_id",
                table: "timeline_incidencias",
                column: "procedimiento_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "procedimientos_equipo");

            migrationBuilder.DropTable(
                name: "roles_quirurgicos");

            migrationBuilder.DropTable(
                name: "timeline_incidencias");

            migrationBuilder.DropTable(
                name: "equipos_medicos");

            migrationBuilder.DropTable(
                name: "cirujanos");

            migrationBuilder.DropTable(
                name: "procedimientos_quirurgicos");

            migrationBuilder.DropTable(
                name: "pacientes");

            migrationBuilder.DropTable(
                name: "salas_quirurgicas");
        }
    }
}
