using Gestion_Quirurgica.Dominio;
using Microsoft.EntityFrameworkCore;
using System;

namespace Gestion_Quirurgica.Data
{
    public class GestionQuirurgicaContext : DbContext
    {
        public GestionQuirurgicaContext(DbContextOptions<GestionQuirurgicaContext> options)
            : base(options) { }

        public DbSet<Paciente>                 Pacientes                  { get; set; }
        public DbSet<SalaQuirurgica>           SalasQuirurgicas           { get; set; }
        public DbSet<Cirujano>                 Cirujanos                  { get; set; }
        public DbSet<EquipoMedico>             EquiposMedicos             { get; set; }
        public DbSet<ProcedimientoQuirurgico>  ProcedimientosQuirurgicos  { get; set; }
        public DbSet<RolQuirurgico>            RolesQuirurgicos           { get; set; }
        public DbSet<ProcedimientoEquipo>      ProcedimientosEquipo       { get; set; }
        public DbSet<TimelineIncidencia>       TimelineIncidencias        { get; set; }

        // ── Nuevas tablas: integración con Logística y Gestión Legal ──────────
        public DbSet<InsumoQuirurgico>         InsumosQuirurgicos         { get; set; }
        public DbSet<RegistroFallecimiento>    RegistrosFallecimiento     { get; set; }
        public DbSet<ProgramacionCirugia>      ProgramacionesCirugia      { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paciente>(e =>
            {
                e.ToTable("pacientes");
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).HasColumnName("id");
                e.Property(p => p.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                e.HasIndex(p => p.Codigo).IsUnique();
                e.Property(p => p.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                e.Property(p => p.Edad).HasColumnName("edad");
                e.Property(p => p.DiagnosticoPre).HasColumnName("diagnostico_pre").HasColumnType("text");
                e.Property(p => p.Alergias).HasColumnName("alergias").HasColumnType("text");
                e.Property(p => p.GrupoSanguineo).HasColumnName("grupo_sanguineo").HasMaxLength(5);
                e.Property(p => p.Estado).HasColumnName("estado").HasMaxLength(10).HasDefaultValue("Activo");
            });

            modelBuilder.Entity<SalaQuirurgica>(e =>
            {
                e.ToTable("salas_quirurgicas");
                e.HasKey(s => s.Id);
                e.Property(s => s.Id).HasColumnName("id");
                e.Property(s => s.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                e.HasIndex(s => s.Codigo).IsUnique();
                e.Property(s => s.NumeroSala).HasColumnName("numero_sala").HasMaxLength(10).IsRequired();
                e.HasIndex(s => s.NumeroSala).IsUnique();
                e.Property(s => s.NivelEsterilidad).HasColumnName("nivel_esterilidad").HasMaxLength(20);
                e.Property(s => s.Disponible).HasColumnName("disponible");
                e.Property(s => s.UltimaLimpieza).HasColumnName("ultima_limpieza");
                e.Property(s => s.Estado).HasColumnName("estado").HasMaxLength(10).HasDefaultValue("Activo");
            });

            modelBuilder.Entity<Cirujano>(e =>
            {
                e.ToTable("cirujanos");
                e.HasKey(c => c.Id);
                e.Property(c => c.Id).HasColumnName("id");
                e.Property(c => c.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                e.HasIndex(c => c.Codigo).IsUnique();
                e.Property(c => c.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                e.Property(c => c.Especialidad).HasColumnName("especialidad").HasMaxLength(50);
                e.Property(c => c.AnosExperiencia).HasColumnName("anos_experiencia");
                e.Property(c => c.Estado).HasColumnName("estado").HasMaxLength(10).HasDefaultValue("Activo");
            });

            modelBuilder.Entity<EquipoMedico>(e =>
            {
                e.ToTable("equipos_medicos");
                e.HasKey(em => em.Id);
                e.Property(em => em.Id).HasColumnName("id");
                e.Property(em => em.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                e.HasIndex(em => em.Codigo).IsUnique();
                e.Property(em => em.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                e.Property(em => em.Fabricante).HasColumnName("fabricante").HasMaxLength(50);
                e.Property(em => em.UltimoMantenimiento).HasColumnName("ultimo_mantenimiento");
                e.Property(em => em.EstadoOperativo).HasColumnName("estado_operativo");
                e.Property(em => em.Estado).HasColumnName("estado").HasMaxLength(10).HasDefaultValue("Activo");
            });

            modelBuilder.Entity<ProcedimientoQuirurgico>(e =>
            {
                e.ToTable("procedimientos_quirurgicos");
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).HasColumnName("id");
                e.Property(p => p.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                e.HasIndex(p => p.Codigo).IsUnique();
                e.Property(p => p.PacienteId).HasColumnName("paciente_id");
                e.Property(p => p.SalaId).HasColumnName("sala_id");
                e.Property(p => p.HoraInicio).HasColumnName("hora_inicio");
                e.Property(p => p.HoraFin).HasColumnName("hora_fin");
                e.Property(p => p.EstadoProcedimiento).HasColumnName("estado_procedimiento").HasMaxLength(20);
                e.Property(p => p.Prioridad).HasColumnName("prioridad").HasMaxLength(15);
                e.Property(p => p.Estado).HasColumnName("estado").HasMaxLength(10).HasDefaultValue("Activo");
                e.HasOne(p => p.Paciente).WithMany(pa => pa.Procedimientos)
                 .HasForeignKey(p => p.PacienteId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(p => p.Sala).WithMany(s => s.Procedimientos)
                 .HasForeignKey(p => p.SalaId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RolQuirurgico>(e =>
            {
                e.ToTable("roles_quirurgicos");
                e.HasKey(r => r.Id);
                e.Property(r => r.Id).HasColumnName("id");
                e.Property(r => r.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                e.HasIndex(r => r.Codigo).IsUnique();
                e.Property(r => r.ProcedimientoId).HasColumnName("procedimiento_id");
                e.Property(r => r.CirujanoId).HasColumnName("cirujano_id");
                e.Property(r => r.Rol).HasColumnName("rol").HasMaxLength(30).IsRequired();
                e.Property(r => r.Estado).HasColumnName("estado").HasMaxLength(10).HasDefaultValue("Activo");
                e.HasOne(r => r.Procedimiento).WithMany(p => p.Roles)
                 .HasForeignKey(r => r.ProcedimientoId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(r => r.Cirujano).WithMany(c => c.Roles)
                 .HasForeignKey(r => r.CirujanoId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProcedimientoEquipo>(e =>
            {
                e.ToTable("procedimientos_equipo");
                e.HasKey(pe => pe.Id);
                e.Property(pe => pe.Id).HasColumnName("id");
                e.Property(pe => pe.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                e.HasIndex(pe => pe.Codigo).IsUnique();
                e.Property(pe => pe.ProcedimientoId).HasColumnName("procedimiento_id");
                e.Property(pe => pe.EquipoId).HasColumnName("equipo_id");
                e.Property(pe => pe.EstadoUso).HasColumnName("estado_uso").HasMaxLength(20);
                e.Property(pe => pe.Observaciones).HasColumnName("observaciones").HasColumnType("text");
                e.Property(pe => pe.Estado).HasColumnName("estado").HasMaxLength(10).HasDefaultValue("Activo");
                e.HasOne(pe => pe.Procedimiento).WithMany(p => p.Equipos)
                 .HasForeignKey(pe => pe.ProcedimientoId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(pe => pe.Equipo).WithMany(em => em.ProcedimientosEquipo)
                 .HasForeignKey(pe => pe.EquipoId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TimelineIncidencia>(e =>
            {
                e.ToTable("timeline_incidencias");
                e.HasKey(t => t.Id);
                e.Property(t => t.Id).HasColumnName("id");
                e.Property(t => t.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                e.HasIndex(t => t.Codigo).IsUnique();
                e.Property(t => t.ProcedimientoId).HasColumnName("procedimiento_id");
                e.Property(t => t.Timestamp).HasColumnName("timestamp");
                e.Property(t => t.Tipo).HasColumnName("tipo").HasMaxLength(30);
                e.Property(t => t.Descripcion).HasColumnName("descripcion").HasColumnType("text");
                e.Property(t => t.UsuarioRegistra).HasColumnName("usuario_registra").HasMaxLength(50);
                e.Property(t => t.Estado).HasColumnName("estado").HasMaxLength(10).HasDefaultValue("Activo");
                e.HasOne(t => t.Procedimiento).WithMany(p => p.Timeline)
                 .HasForeignKey(t => t.ProcedimientoId).OnDelete(DeleteBehavior.Restrict);
            });

            // ── InsumoQuirurgico ──────────────────────────────────────────────
            modelBuilder.Entity<InsumoQuirurgico>(e =>
            {
                e.ToTable("insumos_quirurgicos");
                e.HasKey(i => i.Id);
                e.Property(i => i.Id).HasColumnName("id");
                e.Property(i => i.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                e.HasIndex(i => i.Codigo).IsUnique();
                e.Property(i => i.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
                e.Property(i => i.UnidadMedida).HasColumnName("unidad_medida").HasMaxLength(20);
                e.Property(i => i.CantidadMinima).HasColumnName("cantidad_minima");
                e.Property(i => i.CantidadActual).HasColumnName("cantidad_actual");
                e.Property(i => i.Estado).HasColumnName("estado").HasMaxLength(10).HasDefaultValue("Activo");
                // CantidadNecesaria es calculada, no se persiste en BD
                e.Ignore(i => i.CantidadNecesaria);
            });

            // ── RegistroFallecimiento ─────────────────────────────────────────
            modelBuilder.Entity<RegistroFallecimiento>(e =>
            {
                e.ToTable("registros_fallecimiento");
                e.HasKey(r => r.Id);
                e.Property(r => r.Id).HasColumnName("id");
                e.Property(r => r.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                e.HasIndex(r => r.Codigo).IsUnique();
                e.Property(r => r.ProcedimientoId).HasColumnName("procedimiento_id");
                e.Property(r => r.FechaHoraFallecimiento).HasColumnName("fecha_hora_fallecimiento");
                e.Property(r => r.CausaFallecimiento).HasColumnName("causa_fallecimiento").HasColumnType("text");
                e.Property(r => r.UsuarioRegistra).HasColumnName("usuario_registra").HasMaxLength(50);
                e.Property(r => r.NumeroActaLegal).HasColumnName("numero_acta_legal").HasMaxLength(50);
                e.Property(r => r.EstadoLegal).HasColumnName("estado_legal").HasMaxLength(20).HasDefaultValue("Pendiente");
                e.Property(r => r.Estado).HasColumnName("estado").HasMaxLength(10).HasDefaultValue("Activo");
                e.HasOne(r => r.Procedimiento).WithMany()
                 .HasForeignKey(r => r.ProcedimientoId).OnDelete(DeleteBehavior.Restrict);
            });

            // ── ProgramacionCirugia ───────────────────────────────────────────
            modelBuilder.Entity<ProgramacionCirugia>(e =>
            {
                e.ToTable("programaciones_cirugia");
                e.HasKey(p => p.Id);
                e.Property(p => p.Id).HasColumnName("id");
                e.Property(p => p.Codigo).HasColumnName("codigo").HasMaxLength(20).IsRequired();
                e.HasIndex(p => p.Codigo).IsUnique();
                e.Property(p => p.PacienteId).HasColumnName("paciente_id");
                e.Property(p => p.SalaId).HasColumnName("sala_id");
                e.Property(p => p.CirujanoSolicitanteId).HasColumnName("cirujano_solicitante_id");
                e.Property(p => p.FechaSolicitada).HasColumnName("fecha_solicitada");
                e.Property(p => p.HoraInicioEstimada).HasColumnName("hora_inicio_estimada");
                e.Property(p => p.HoraFinEstimada).HasColumnName("hora_fin_estimada");
                e.Property(p => p.TipoCirugia).HasColumnName("tipo_cirugia").HasMaxLength(100);
                e.Property(p => p.Prioridad).HasColumnName("prioridad").HasMaxLength(15);
                e.Property(p => p.Observaciones).HasColumnName("observaciones").HasColumnType("text");
                e.Property(p => p.EstadoProgramacion).HasColumnName("estado_programacion").HasMaxLength(20).HasDefaultValue("Solicitada");
                e.Property(p => p.Estado).HasColumnName("estado").HasMaxLength(10).HasDefaultValue("Activo");
                e.HasOne(p => p.Paciente).WithMany()
                 .HasForeignKey(p => p.PacienteId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(p => p.Sala).WithMany()
                 .HasForeignKey(p => p.SalaId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(p => p.CirujanoSolicitante).WithMany()
                 .HasForeignKey(p => p.CirujanoSolicitanteId).OnDelete(DeleteBehavior.Restrict);
            });

            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder mb)
        {
            var now = new DateTime(2025, 1, 15, 8, 0, 0, DateTimeKind.Utc);

            mb.Entity<Paciente>().HasData(
                new Paciente { Id=1, Codigo="PAC-001", Nombre="Juan Pérez Mamani",    Edad=45, DiagnosticoPre="Apendicitis aguda",        Alergias="Penicilina",  GrupoSanguineo="O+",  Estado="Activo" },
                new Paciente { Id=2, Codigo="PAC-002", Nombre="María López Flores",   Edad=32, DiagnosticoPre="Colecistitis crónica",     Alergias="Ninguna",     GrupoSanguineo="A+",  Estado="Activo" },
                new Paciente { Id=3, Codigo="PAC-003", Nombre="Carlos Ruiz Torrez",   Edad=58, DiagnosticoPre="Hernia inguinal derecha",  Alergias="Ibuprofeno",  GrupoSanguineo="B-",  Estado="Activo" },
                new Paciente { Id=4, Codigo="PAC-004", Nombre="Ana Condori Huanca",   Edad=27, DiagnosticoPre="Fractura de fémur",        Alergias="Ninguna",     GrupoSanguineo="AB+", Estado="Activo" },
                new Paciente { Id=5, Codigo="PAC-005", Nombre="Luis Quispe Mamani",   Edad=61, DiagnosticoPre="Tumor gástrico",           Alergias="Aspirina",    GrupoSanguineo="O-",  Estado="Activo" }
            );

            mb.Entity<SalaQuirurgica>().HasData(
                new SalaQuirurgica { Id=1, Codigo="SALA-001", NumeroSala="Q-01", NivelEsterilidad="Alto",  Disponible=false, UltimaLimpieza=now.AddHours(-2),  Estado="Activo" },
                new SalaQuirurgica { Id=2, Codigo="SALA-002", NumeroSala="Q-02", NivelEsterilidad="Medio", Disponible=true,  UltimaLimpieza=now.AddHours(-5),  Estado="Activo" },
                new SalaQuirurgica { Id=3, Codigo="SALA-003", NumeroSala="Q-03", NivelEsterilidad="Alto",  Disponible=true,  UltimaLimpieza=now.AddHours(-1),  Estado="Activo" },
                new SalaQuirurgica { Id=4, Codigo="SALA-004", NumeroSala="Q-04", NivelEsterilidad="Bajo",  Disponible=false, UltimaLimpieza=now.AddHours(-8),  Estado="Activo" }
            );

            mb.Entity<Cirujano>().HasData(
                new Cirujano { Id=1, Codigo="CIR-001", Nombre="Dr. Roberto Vega",      Especialidad="Cirugía General",        AnosExperiencia=15, Estado="Activo" },
                new Cirujano { Id=2, Codigo="CIR-002", Nombre="Dra. Ana Condori",       Especialidad="Anestesiología",         AnosExperiencia=10, Estado="Activo" },
                new Cirujano { Id=3, Codigo="CIR-003", Nombre="Dr. Luis Quispe",        Especialidad="Cirugía Laparoscópica",  AnosExperiencia=8,  Estado="Activo" },
                new Cirujano { Id=4, Codigo="CIR-004", Nombre="Dra. Patricia Mamani",   Especialidad="Traumatología",          AnosExperiencia=12, Estado="Activo" },
                new Cirujano { Id=5, Codigo="CIR-005", Nombre="Dr. Fernando Soria",     Especialidad="Oncología Quirúrgica",   AnosExperiencia=20, Estado="Activo" },
                new Cirujano { Id=6, Codigo="CIR-006", Nombre="Dra. Carmen Flores",     Especialidad="Anestesiología",         AnosExperiencia=6,  Estado="Activo" }
            );

            mb.Entity<EquipoMedico>().HasData(
                new EquipoMedico { Id=1, Codigo="EQ-001", Nombre="Bisturí Eléctrico",    Fabricante="Bovie Medical",  UltimoMantenimiento=now.AddDays(-30), EstadoOperativo=true,  Estado="Activo" },
                new EquipoMedico { Id=2, Codigo="EQ-002", Nombre="Monitor de Signos",    Fabricante="Mindray",        UltimoMantenimiento=now.AddDays(-15), EstadoOperativo=true,  Estado="Activo" },
                new EquipoMedico { Id=3, Codigo="EQ-003", Nombre="Laparoscopio HD",      Fabricante="Stryker",        UltimoMantenimiento=now.AddDays(-7),  EstadoOperativo=true,  Estado="Activo" },
                new EquipoMedico { Id=4, Codigo="EQ-004", Nombre="Respirador UCI",       Fabricante="Dräger",         UltimoMantenimiento=now.AddDays(-45), EstadoOperativo=false, Estado="Activo" },
                new EquipoMedico { Id=5, Codigo="EQ-005", Nombre="Mesa de Operaciones",  Fabricante="Maquet",         UltimoMantenimiento=now.AddDays(-60), EstadoOperativo=true,  Estado="Activo" }
            );

            mb.Entity<ProcedimientoQuirurgico>().HasData(
                new ProcedimientoQuirurgico { Id=1, Codigo="PROC-001", PacienteId=1, SalaId=1, HoraInicio=now,             HoraFin=now.AddHours(2),  EstadoProcedimiento="En curso",   Prioridad="Alta",   Estado="Activo" },
                new ProcedimientoQuirurgico { Id=2, Codigo="PROC-002", PacienteId=2, SalaId=2, HoraInicio=now.AddHours(3), HoraFin=now.AddHours(5),  EstadoProcedimiento="Programado", Prioridad="Media",  Estado="Activo" },
                new ProcedimientoQuirurgico { Id=3, Codigo="PROC-003", PacienteId=3, SalaId=3, HoraInicio=now.AddHours(-3),HoraFin=now.AddHours(-1), EstadoProcedimiento="Finalizado", Prioridad="Baja",   Estado="Activo" },
                new ProcedimientoQuirurgico { Id=4, Codigo="PROC-004", PacienteId=4, SalaId=4, HoraInicio=now.AddHours(1), HoraFin=now.AddHours(4),  EstadoProcedimiento="Programado", Prioridad="Alta",   Estado="Activo" },
                new ProcedimientoQuirurgico { Id=5, Codigo="PROC-005", PacienteId=5, SalaId=1, HoraInicio=now.AddDays(-1), HoraFin=now.AddDays(-1).AddHours(3), EstadoProcedimiento="Finalizado", Prioridad="Media", Estado="Activo" }
            );

            mb.Entity<RolQuirurgico>().HasData(
                new RolQuirurgico { Id=1, Codigo="ROL-001", ProcedimientoId=1, CirujanoId=1, Rol="Cirujano Principal", Estado="Activo" },
                new RolQuirurgico { Id=2, Codigo="ROL-002", ProcedimientoId=1, CirujanoId=2, Rol="Anestesiólogo",      Estado="Activo" },
                new RolQuirurgico { Id=3, Codigo="ROL-003", ProcedimientoId=2, CirujanoId=3, Rol="Cirujano Principal", Estado="Activo" },
                new RolQuirurgico { Id=4, Codigo="ROL-004", ProcedimientoId=2, CirujanoId=6, Rol="Anestesiólogo",      Estado="Activo" },
                new RolQuirurgico { Id=5, Codigo="ROL-005", ProcedimientoId=3, CirujanoId=1, Rol="Cirujano Principal", Estado="Activo" },
                new RolQuirurgico { Id=6, Codigo="ROL-006", ProcedimientoId=4, CirujanoId=4, Rol="Cirujano Principal", Estado="Activo" },
                new RolQuirurgico { Id=7, Codigo="ROL-007", ProcedimientoId=5, CirujanoId=5, Rol="Cirujano Principal", Estado="Activo" },
                new RolQuirurgico { Id=8, Codigo="ROL-008", ProcedimientoId=5, CirujanoId=2, Rol="Anestesiólogo",      Estado="Activo" }
            );

            mb.Entity<ProcedimientoEquipo>().HasData(
                new ProcedimientoEquipo { Id=1, Codigo="PE-001", ProcedimientoId=1, EquipoId=1, EstadoUso="En uso",    Observaciones="Modo corte activo",          Estado="Activo" },
                new ProcedimientoEquipo { Id=2, Codigo="PE-002", ProcedimientoId=1, EquipoId=2, EstadoUso="En uso",    Observaciones="Monitoreo continuo",          Estado="Activo" },
                new ProcedimientoEquipo { Id=3, Codigo="PE-003", ProcedimientoId=2, EquipoId=3, EstadoUso="Reservado", Observaciones="Reservado para laparoscopia", Estado="Activo" },
                new ProcedimientoEquipo { Id=4, Codigo="PE-004", ProcedimientoId=3, EquipoId=1, EstadoUso="Liberado",  Observaciones="Proceso completado",          Estado="Activo" },
                new ProcedimientoEquipo { Id=5, Codigo="PE-005", ProcedimientoId=4, EquipoId=5, EstadoUso="Reservado", Observaciones="Mesa reservada para trauma",  Estado="Activo" }
            );

            mb.Entity<TimelineIncidencia>().HasData(
                new TimelineIncidencia { Id=1, Codigo="TL-001", ProcedimientoId=1, Timestamp=now.AddMinutes(-30), Tipo="Inicio",      Descripcion="Procedimiento iniciado sin novedades",         UsuarioRegistra="enf.garcia",   Estado="Activo" },
                new TimelineIncidencia { Id=2, Codigo="TL-002", ProcedimientoId=1, Timestamp=now.AddMinutes(-15), Tipo="Observación", Descripcion="Presión arterial estable 120/80",              UsuarioRegistra="enf.garcia",   Estado="Activo" },
                new TimelineIncidencia { Id=3, Codigo="TL-003", ProcedimientoId=1, Timestamp=now.AddMinutes(-5),  Tipo="Incidencia",  Descripcion="Sangrado moderado controlado",                 UsuarioRegistra="dr.vega",      Estado="Activo" },
                new TimelineIncidencia { Id=4, Codigo="TL-004", ProcedimientoId=3, Timestamp=now.AddHours(-3),    Tipo="Inicio",      Descripcion="Cirugía de hernia iniciada",                   UsuarioRegistra="enf.morales",  Estado="Activo" },
                new TimelineIncidencia { Id=5, Codigo="TL-005", ProcedimientoId=3, Timestamp=now.AddHours(-1),    Tipo="Fin",         Descripcion="Cirugía finalizada exitosamente",              UsuarioRegistra="enf.morales",  Estado="Activo" },
                new TimelineIncidencia { Id=6, Codigo="TL-006", ProcedimientoId=5, Timestamp=now.AddDays(-1),     Tipo="Inicio",      Descripcion="Resección gástrica parcial iniciada",          UsuarioRegistra="enf.silva",    Estado="Activo" },
                new TimelineIncidencia { Id=7, Codigo="TL-007", ProcedimientoId=5, Timestamp=now.AddDays(-1).AddHours(3), Tipo="Fin", Descripcion="Resección completada sin complicaciones",    UsuarioRegistra="enf.silva",    Estado="Activo" }
            );

            // ── Seed: Insumos Quirúrgicos (inventario inicial del área quirúrgica)
            mb.Entity<InsumoQuirurgico>().HasData(
                new InsumoQuirurgico { Id=1, Codigo="INS-001", Nombre="Vendas estériles",       UnidadMedida="unidad",   CantidadMinima=10, CantidadActual=5,  Estado="Activo" },
                new InsumoQuirurgico { Id=2, Codigo="INS-002", Nombre="Paquetes de sangre O+",  UnidadMedida="paquete",  CantidadMinima=2,  CantidadActual=2,  Estado="Activo" },
                new InsumoQuirurgico { Id=3, Codigo="INS-003", Nombre="Escalpelos",             UnidadMedida="unidad",   CantidadMinima=5,  CantidadActual=3,  Estado="Activo" },
                new InsumoQuirurgico { Id=4, Codigo="INS-004", Nombre="Guantes quirúrgicos",    UnidadMedida="par",      CantidadMinima=20, CantidadActual=20, Estado="Activo" },
                new InsumoQuirurgico { Id=5, Codigo="INS-005", Nombre="Suturas absorbibles",    UnidadMedida="caja",     CantidadMinima=4,  CantidadActual=1,  Estado="Activo" },
                new InsumoQuirurgico { Id=6, Codigo="INS-006", Nombre="Solución salina 500ml",  UnidadMedida="frasco",   CantidadMinima=8,  CantidadActual=8,  Estado="Activo" },
                new InsumoQuirurgico { Id=7, Codigo="INS-007", Nombre="Gasas estériles 10x10",  UnidadMedida="paquete",  CantidadMinima=15, CantidadActual=6,  Estado="Activo" }
            );
        }
    }
}
