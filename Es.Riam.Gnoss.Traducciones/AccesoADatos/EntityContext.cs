namespace Traduccion.AccesoADatos
{
    using Microsoft.EntityFrameworkCore;

    public partial class EntityContext : DbContext
    {
        public EntityContext()
            : base()
        {
        }

        public virtual DbSet<TextosPersonalizadosPersonalizacion> TextosPersonalizadosPersonalizacion { get; set; }
        public virtual DbSet<TextosPersonalizadosPlataforma> TextosPersonalizadosPlataforma { get; set; }
        public virtual DbSet<TextosPersonalizadosProyecto> TextosPersonalizadosProyecto { get; set; }


        public virtual DbSet<BaseRecursosProyecto> BaseRecursosProyecto { get; set; }
        public virtual DbSet<CategoriaTesauro> CategoriaTesauro { get; set; }
        public virtual DbSet<ClausulaRegistro> ClausulaRegistro { get; set; }
        public virtual DbSet<CMSComponente> CMSComponente { get; set; }
        public virtual DbSet<CMSPropiedadComponente> CMSPropiedadComponente { get; set; }
        public virtual DbSet<Documento> Documento { get; set; }
        public virtual DbSet<DocumentoWebVinBaseRecursos> DocumentoWebVinBaseRecursos { get; set; }
        public virtual DbSet<FacetaObjetoConocimientoProyecto> FacetaObjetoConocimientoProyecto { get; set; }
        public virtual DbSet<OntologiaProyecto> OntologiaProyecto { get; set; }
        public virtual DbSet<ProyectoGadget> ProyectoGadget { get; set; }
        public virtual DbSet<ProyectoPestanyaMenu> ProyectoPestanyaMenu { get; set; }
        public virtual DbSet<TesauroProyecto> TesauroProyecto { get; set; }
        public virtual DbSet<ConfiguracionBBDD> ConfiguracionBBDD { get; set; }
        public virtual DbSet<ParametroAplicacion> ParametroAplicacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CMSComponente>()
                .Property(e => e.IdiomasDisponibles)
                .IsUnicode(false);

            modelBuilder.Entity<CMSComponente>()
                .Property(e => e.NombreCortoComponente)
                .IsUnicode(false);

            modelBuilder.Entity<CMSComponente>()
                .HasMany(e => e.CMSPropiedadComponente)
                .WithOne(e => e.CMSComponente).IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Documento>()
                .HasMany(e => e.DocumentoWebVinBaseRecursos)
                .WithOne(e => e.Documento).IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OntologiaProyecto>()
                .Property(e => e.NombreCortoOnt)
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .Property(e => e.NombreCortoPestanya)
                .IsUnicode(false);

            modelBuilder.Entity<ProyectoPestanyaMenu>()
                .HasMany(e => e.ProyectoPestanyaMenu1)
                .WithOne(e => e.ProyectoPestanyaMenu2)
                .HasForeignKey(e => e.PestanyaPadreID);
        }
    }
}
