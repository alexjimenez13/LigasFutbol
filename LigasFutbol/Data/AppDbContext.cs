using Microsoft.EntityFrameworkCore;
using LigasFutbol.Models;

namespace LigasFutbol.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Liga> FUT_LIGAS { get; set; }
        public DbSet<Equipo> FUT_EQUIPOS { get; set; }
        public DbSet<Jugador> FUT_JUGADORES { get; set; }
        public DbSet<Partido> FUT_PARTIDOS { get; set; }
        public DbSet<Resultado> FUT_RESULTADOS { get; set; }
        public DbSet<Estadistica> FUT_ESTADISTICAS { get; set; }
        public DbSet<Clasificacion> FUT_CLASIFICACIONES { get; set; }
        public DbSet<Entrenamiento> FUT_ENTRENAMIENTOS { get; set; }
        public DbSet<Informe> FUT_INFORMES { get; set; }
        public DbSet<Posicion> FUT_POSICIONES { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Jugador>(entity =>
            {
                entity.ToTable("FUT_JUGADORES");
                entity.HasKey(e => e.JugadorId).HasName("PK_FUT_JUGADORES");
                entity.Property(e => e.JugadorId)
                      .HasColumnName("JUGADOR_ID");
                entity.Property(e => e.Nombre)
                      .HasColumnName("NOMBRE")
                      .IsRequired();
                entity.Property(e => e.Apellido)
                      .HasColumnName("APELLIDO")
                      .IsRequired();
                entity.Property(e => e.FechaNacimiento)
                      .HasColumnName("FECHA_NACIMIENTO");
                entity.Property(e => e.PosicionId)
                      .HasColumnName("POSICION_ID");
                entity.Property(e => e.EquipoId)           
                      .HasColumnName("EQUIPO_ID");
                entity.Property(e => e.Estado)
                      .HasColumnName("ESTADO");

                entity.HasOne(e => e.Posicion)
                      .WithMany()
                      .HasForeignKey(e => e.PosicionId);

                entity.HasOne(e => e.Equipo)
                      .WithMany()
                      .HasForeignKey(e => e.EquipoId);
            });

            modelBuilder.Entity<Posicion>(entity =>
            {
                entity.ToTable("FUT_POSICIONES");
                entity.HasKey(e => e.PosicionId).HasName("PK_FUT_POSICIONES");
                entity.Property(e => e.PosicionId).HasColumnName("POSICION_ID");
                entity.Property(e => e.Nombre).HasColumnName("NOMBRE").IsRequired();
                entity.Property(e => e.Estado).HasColumnName("ESTADO");
            });

            modelBuilder.Entity<Liga>(entity =>
            {
                entity.ToTable("FUT_LIGAS");
                entity.HasKey(e => e.LigaId).HasName("PK_FUT_LIGAS");
                entity.Property(e => e.LigaId)
                      .HasColumnName("LIGA_ID");
                entity.Property(e => e.Nombre)
                      .HasColumnName("NOMBRE")
                      .IsRequired();
                entity.Property(e => e.Descripcion)
                      .HasColumnName("DESCRIPCION");
                entity.Property(e => e.FechaInicio)
                      .HasColumnName("FECHA_INICIO");
                entity.Property(e => e.FechaFin)
                      .HasColumnName("FECHA_FIN");
                entity.Property(e => e.Estado)
                      .HasColumnName("ESTADO");
            });

            modelBuilder.Entity<Equipo>(entity =>
            {
                entity.ToTable("FUT_EQUIPOS");
                entity.HasKey(e => e.EquipoId).HasName("PK_FUT_EQUIPOS");
                entity.Property(e => e.EquipoId).HasColumnName("EQUIPO_ID");
                entity.Property(e => e.Nombre).HasColumnName("NOMBRE").IsRequired();
                entity.Property(e => e.LigaId).HasColumnName("LIGA_ID");
                entity.Property(e => e.Entrenador).HasColumnName("ENTRENADOR");
                entity.Property(e => e.Estado).HasColumnName("ESTADO");
            });

            modelBuilder.Entity<Partido>(entity =>
            {
                entity.ToTable("FUT_PARTIDOS");
                entity.HasKey(e => e.PartidoId).HasName("PK_FUT_PARTIDOS");

                entity.Property(e => e.PartidoId)
                      .HasColumnName("PARTIDO_ID");
                entity.Property(e => e.LigaId)
                      .HasColumnName("LIGA_ID");
                entity.Property(e => e.EquipoLocalId)
                      .HasColumnName("EQUIPO_LOCAL_ID");
                entity.Property(e => e.EquipoVisitanteId)
                      .HasColumnName("EQUIPO_VISITA_ID");   
                entity.Property(e => e.FechaHora)
                      .HasColumnName("FECHA_HORA");
                entity.Property(e => e.Estadio)
                      .HasColumnName("ESTADIO");            
                entity.Property(e => e.Estado)
                      .HasColumnName("ESTADO");

                entity.HasOne(e => e.Liga)
                      .WithMany()
                      .HasForeignKey(e => e.LigaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EquipoLocal)
                      .WithMany()
                      .HasForeignKey(e => e.EquipoLocalId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EquipoVisitante)
                      .WithMany()
                      .HasForeignKey(e => e.EquipoVisitanteId)
                      .OnDelete(DeleteBehavior.Restrict);
            });



            modelBuilder.Entity<Resultado>(entity =>
            {
                entity.ToTable("FUT_RESULTADOS");
                entity.HasKey(e => e.ResultadoId).HasName("PK_FUT_RESULTADOS");

                entity.Property(e => e.ResultadoId)
                      .HasColumnName("RESULTADO_ID");
                entity.Property(e => e.PartidoId)
                      .HasColumnName("PARTIDO_ID");
                entity.Property(e => e.GolesLocal)
                      .HasColumnName("GOLES_LOCAL");
                entity.Property(e => e.GolesVisita)
                      .HasColumnName("GOLES_VISITA");
                entity.HasOne(e => e.Partido)
                      .WithMany()
                      .HasForeignKey(e => e.PartidoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Estadistica>(entity =>
            {
                entity.ToTable("FUT_ESTADISTICAS");
                entity.HasKey(e => e.EstadisticaId).HasName("PK_FUT_ESTADISTICAS");

                entity.Property(e => e.EstadisticaId)
                      .HasColumnName("ESTADISTICA_ID");
                entity.Property(e => e.JugadorId)
                      .HasColumnName("JUGADOR_ID");
                entity.Property(e => e.PartidoId)
                      .HasColumnName("PARTIDO_ID");
                entity.Property(e => e.Goles)
                      .HasColumnName("GOLES");
                entity.Property(e => e.Asistencias)
                      .HasColumnName("ASISTENCIAS");
                entity.Property(e => e.TarjetasAmarillas)
                      .HasColumnName("TARJETAS_AMARILLAS");
                entity.Property(e => e.TarjetasRojas)
                      .HasColumnName("TARJETAS_ROJAS");

                entity.HasOne(e => e.Partido)
                      .WithMany()
                      .HasForeignKey(e => e.PartidoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Jugador)
                      .WithMany()
                      .HasForeignKey(e => e.JugadorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Clasificacion>().ToTable("FUT_CLASIFICACIONES");
            modelBuilder.Entity<Entrenamiento>().ToTable("FUT_ENTRENAMIENTOS");
            modelBuilder.Entity<Informe>().ToTable("FUT_INFORMES");

            modelBuilder
              .Entity<Clasificacion>(eb =>
              {
                  eb.HasNoKey();
                  eb.ToView(null);
              });
        }
    }
}
