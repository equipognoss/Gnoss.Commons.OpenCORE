using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.AD.ParametrosProyecto
{
    #region Enumeraciones
    #endregion



    public class JoinProyectoPestanyaBusquedaExportacionProyectoPestanyaMenu
    {
        public ProyectoPestanyaBusquedaExportacion ProyectoPestanyaBusquedaExportacion { get; set; }
        public ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }
    }

    public class JoinProyectoPestanyaBusquedaExportacionPropiedadProyectoPestanyaBusquedaExportacion
    {
        public ProyectoPestanyaBusquedaExportacionPropiedad ProyectoPestanyaBusquedaExportacionPropiedad { get; set; }
        public ProyectoPestanyaBusquedaExportacion ProyectoPestanyaBusquedaExportacion { get; set; }
    }

    public class JoinProyectoPestanyaBusquedaExportacionPropiedadProyectoPestanyaBusquedaExportacionProyectoPestanyaMenu
    {
        public ProyectoPestanyaBusquedaExportacionPropiedad ProyectoPestanyaBusquedaExportacionPropiedad { get; set; }
        public ProyectoPestanyaBusquedaExportacion ProyectoPestanyaBusquedaExportacion { get; set; }
        public ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }
    }

    public class JoinProyectoPestanyaBusquedaExportacionExternaProyectoPestanyaBusquedaExportacion
    {
        public ProyectoPestanyaBusquedaExportacion ProyectoPestanyaBusquedaExportacion { get; set; }
        public ProyectoPestanyaBusquedaExportacionExterna ProyectoPestanyaBusquedaExportacionExterna { get; set; }
    }

    public class JoinProyectoPestanyaBusquedaExportacionExternaProyectoPestanyaBusquedaExportacionProyectoPestanyaMenu
    {
        public ProyectoPestanyaBusquedaExportacion ProyectoPestanyaBusquedaExportacion { get; set; }
        public ProyectoPestanyaBusquedaExportacionExterna ProyectoPestanyaBusquedaExportacionExterna { get; set; }
        public ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }
    }

    //ProyectoPestanyaMenu ON ProyectoPestanyaBusquedaExportacion.PestanyaID = ProyectoPestanyaMenu.PestanyaID

    public static partial class Joins
    {
        public static IQueryable<JoinProyectoPestanyaBusquedaExportacionExternaProyectoPestanyaBusquedaExportacionProyectoPestanyaMenu> JoinProyectoPestanyaMenu(this IQueryable<JoinProyectoPestanyaBusquedaExportacionExternaProyectoPestanyaBusquedaExportacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ProyectoPestanyaMenu, item => item.ProyectoPestanyaBusquedaExportacion.PestanyaID, proyectoPestanyaMenu => proyectoPestanyaMenu.PestanyaID, (item, proyectoPestanyaMenu) => new JoinProyectoPestanyaBusquedaExportacionExternaProyectoPestanyaBusquedaExportacionProyectoPestanyaMenu
            {
                ProyectoPestanyaMenu = proyectoPestanyaMenu,
                ProyectoPestanyaBusquedaExportacion = item.ProyectoPestanyaBusquedaExportacion,
                ProyectoPestanyaBusquedaExportacionExterna = item.ProyectoPestanyaBusquedaExportacionExterna
            });
        }

        public static IQueryable<JoinProyectoPestanyaBusquedaExportacionExternaProyectoPestanyaBusquedaExportacion> JoinProyectoPestanyaBusquedaExportacion(this IQueryable<ProyectoPestanyaBusquedaExportacionExterna> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ProyectoPestanyaBusquedaExportacion, proyectoPestanyaBusquedaExportacionExterna => proyectoPestanyaBusquedaExportacionExterna.ExportacionID, proyectoPestanyaBusquedaExportacion => proyectoPestanyaBusquedaExportacion.ExportacionID, (proyectoPestanyaBusquedaExportacionExterna, proyectoPestanyaBusquedaExportacion) => new JoinProyectoPestanyaBusquedaExportacionExternaProyectoPestanyaBusquedaExportacion
            {
                ProyectoPestanyaBusquedaExportacion = proyectoPestanyaBusquedaExportacion,
                ProyectoPestanyaBusquedaExportacionExterna = proyectoPestanyaBusquedaExportacionExterna
            });
        }

        public static IQueryable<JoinProyectoPestanyaBusquedaExportacionPropiedadProyectoPestanyaBusquedaExportacionProyectoPestanyaMenu> JoinProyectoPestanyaMenu(this IQueryable<JoinProyectoPestanyaBusquedaExportacionPropiedadProyectoPestanyaBusquedaExportacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ProyectoPestanyaMenu, item => item.ProyectoPestanyaBusquedaExportacion.PestanyaID, proyectoPestanyaMenu => proyectoPestanyaMenu.PestanyaID, (item, proyectoPestanyaMenu) => new JoinProyectoPestanyaBusquedaExportacionPropiedadProyectoPestanyaBusquedaExportacionProyectoPestanyaMenu
            {
                ProyectoPestanyaMenu = proyectoPestanyaMenu,
                ProyectoPestanyaBusquedaExportacion = item.ProyectoPestanyaBusquedaExportacion,
                ProyectoPestanyaBusquedaExportacionPropiedad = item.ProyectoPestanyaBusquedaExportacionPropiedad
            });
        }

        public static IQueryable<JoinProyectoPestanyaBusquedaExportacionPropiedadProyectoPestanyaBusquedaExportacion> JoinProyectoPestanyaBusquedaExportacion(this IQueryable<ProyectoPestanyaBusquedaExportacionPropiedad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ProyectoPestanyaBusquedaExportacion, proyectoPestanyaBusquedaExportacionPropiedad => proyectoPestanyaBusquedaExportacionPropiedad.ExportacionID, proyectoPestanyaBusquedaExportacion => proyectoPestanyaBusquedaExportacion.ExportacionID, (proyectoPestanyaBusquedaExportacionPropiedad, proyectoPestanyaBusquedaExportacion) => new JoinProyectoPestanyaBusquedaExportacionPropiedadProyectoPestanyaBusquedaExportacion
            {
                ProyectoPestanyaBusquedaExportacion = proyectoPestanyaBusquedaExportacion,
                ProyectoPestanyaBusquedaExportacionPropiedad = proyectoPestanyaBusquedaExportacionPropiedad
            });
        }

        public static IQueryable<JoinProyectoPestanyaBusquedaExportacionProyectoPestanyaMenu> JoinProyectoPestanyaMenu(this IQueryable<ProyectoPestanyaBusquedaExportacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ProyectoPestanyaMenu, proyectoPestanyaBusquedaExportacion => proyectoPestanyaBusquedaExportacion.PestanyaID, proyectoPestanyaMenu => proyectoPestanyaMenu.PestanyaID, (proyectoPestanyaBusquedaExportacion, proyectoPestanyaMenu) => new JoinProyectoPestanyaBusquedaExportacionProyectoPestanyaMenu
            {
                ProyectoPestanyaMenu = proyectoPestanyaMenu,
                ProyectoPestanyaBusquedaExportacion = proyectoPestanyaBusquedaExportacion
            });
        }
    }

    public class ExportacionBusquedaAD : BaseAD
    {
        #region Consultas

        string sqlSelectProyectoPestanyaBusquedaExportacion;
        string sqlSelectProyectoPestanyaBusquedaExportacionPropiedad;
        string sqlSelectProyectoPestanyaBusquedaExportacionExterna;

        #endregion

        #region DataAdapter

        #region ProyectoPestanyaBusquedaExportacion

        string sqlProyectoPestanyaBusquedaExportacionInsert;
        string sqlProyectoPestanyaBusquedaExportacionDelete;
        string sqlProyectoPestanyaBusquedaExportacionModify;

        #endregion

        #region ProyectoPestanyaBusquedaExportacionPropiedad

        string sqlProyectoPestanyaBusquedaExportacionPropiedadInsert;
        string sqlProyectoPestanyaBusquedaExportacionPropiedadDelete;
        string sqlProyectoPestanyaBusquedaExportacionPropiedadModify;

        #endregion

        #region ProyectoPestanyaBusquedaExportacionExterna

        string sqlProyectoPestanyaBusquedaExportacionExternaInsert;
        string sqlProyectoPestanyaBusquedaExportacionExternaDelete;
        string sqlProyectoPestanyaBusquedaExportacionExternaModify;

        #endregion

        #endregion

        #region Constructor

        private EntityContext mEntityContext;

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ExportacionBusquedaAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ExportacionBusquedaAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Metodos Generales

        #region Privados

        /// <summary>
        /// Procedimiento para construir las consultas
        /// </summary>
        /// <param name="pIBD">Interfaz de base de datos</param>
        private void CargarConsultasYDataAdapters(IBaseDatos pIBD)
        {
            #region Consultas

            sqlSelectProyectoPestanyaBusquedaExportacion = "SELECT ProyectoPestanyaBusquedaExportacion." + pIBD.CargarGuid("ExportacionID") + ", ProyectoPestanyaBusquedaExportacion." + pIBD.CargarGuid("PestanyaID") + ", ProyectoPestanyaBusquedaExportacion.NombreExportacion, ProyectoPestanyaBusquedaExportacion.Orden, ProyectoPestanyaBusquedaExportacion.GruposExportadores, ProyectoPestanyaBusquedaExportacion.FormatosExportacion FROM ProyectoPestanyaBusquedaExportacion";
            sqlSelectProyectoPestanyaBusquedaExportacionPropiedad = "SELECT ProyectoPestanyaBusquedaExportacionPropiedad." + pIBD.CargarGuid("ExportacionID") + ", ProyectoPestanyaBusquedaExportacionPropiedad.Orden, ProyectoPestanyaBusquedaExportacionPropiedad." + pIBD.CargarGuid("OntologiaID") + ", ProyectoPestanyaBusquedaExportacionPropiedad.Ontologia, ProyectoPestanyaBusquedaExportacionPropiedad.Propiedad, ProyectoPestanyaBusquedaExportacionPropiedad.NombrePropiedad, ProyectoPestanyaBusquedaExportacionPropiedad.DatosExtraPropiedad FROM ProyectoPestanyaBusquedaExportacionPropiedad";
            sqlSelectProyectoPestanyaBusquedaExportacionExterna = "SELECT ProyectoPestanyaBusquedaExportacionExterna." + pIBD.CargarGuid("ExportacionID") + ", ProyectoPestanyaBusquedaExportacionExterna." + pIBD.CargarGuid("PestanyaID") + ", ProyectoPestanyaBusquedaExportacionExterna.UrlServicioExterno FROM ProyectoPestanyaBusquedaExportacionExterna";

            #endregion

            #region DataAdapter

            #region ProyectoPestanyaBusquedaExportacion

            sqlProyectoPestanyaBusquedaExportacionInsert = pIBD.ReplaceParam("INSERT INTO ProyectoPestanyaBusquedaExportacion (ExportacionID, PestanyaID, NombreExportacion, Orden, GruposExportadores, FormatosExportacion ) VALUES (" + pIBD.GuidParamColumnaTabla("ExportacionID") + ", " + pIBD.GuidParamColumnaTabla("PestanyaID") + ", @NombreExportacion, @Orden, @GruposExportadores, @FormatosExportacion)");

            sqlProyectoPestanyaBusquedaExportacionDelete = pIBD.ReplaceParam("DELETE FROM ProyectoPestanyaBusquedaExportacion WHERE (ExportacionID = " + pIBD.GuidParamColumnaTabla("Original_ExportacionID") + ")");

            sqlProyectoPestanyaBusquedaExportacionModify = pIBD.ReplaceParam("UPDATE ProyectoPestanyaBusquedaExportacion SET ExportacionID = " + pIBD.GuidParamColumnaTabla("ExportacionID") + ", PestanyaID = " + pIBD.GuidParamColumnaTabla("PestanyaID") + ", NombreExportacion=@NombreExportacion, Orden=@Orden, GruposExportadores=@GruposExportadores, FormatosExportacion=@FormatosExportacion WHERE (ExportacionID = " + pIBD.GuidParamColumnaTabla("Original_ExportacionID") + ")");

            #endregion

            #region ProyectoPestanyaBusquedaExportacionPropiedad

            sqlProyectoPestanyaBusquedaExportacionPropiedadInsert = pIBD.ReplaceParam("INSERT INTO ProyectoPestanyaBusquedaExportacionPropiedad (ExportacionID, Orden, OntologiaID, Ontologia, Propiedad, NombrePropiedad, DatosExtraPropiedad) VALUES (" + pIBD.GuidParamColumnaTabla("ExportacionID") + ", @Orden, " + pIBD.GuidParamColumnaTabla("OntologiaID") + ", @Ontologia, @Propiedad, @NombrePropiedad, @DatosExtraPropiedad)");

            sqlProyectoPestanyaBusquedaExportacionPropiedadDelete = pIBD.ReplaceParam("DELETE FROM ProyectoPestanyaBusquedaExportacionPropiedad WHERE (ExportacionID = " + pIBD.GuidParamColumnaTabla("Original_ExportacionID") + " AND Orden=@Original_Orden)");

            sqlProyectoPestanyaBusquedaExportacionPropiedadModify = pIBD.ReplaceParam("UPDATE ProyectoPestanyaBusquedaExportacionPropiedad SET ExportacionID = " + pIBD.GuidParamColumnaTabla("ExportacionID") + ", Orden=@Orden, OntologiaID=" + pIBD.GuidParamColumnaTabla("OntologiaID") + ", Ontologia=@Ontologia, Propiedad=@Propiedad, NombrePropiedad=@NombrePropiedad,  DatosExtraPropiedad=@DatosExtraPropiedad WHERE (ExportacionID = " + pIBD.GuidParamColumnaTabla("Original_ExportacionID") + " AND Orden=@Original_Orden)");

            #endregion

            #region ProyectoPestanyaBusquedaExportacionExterna

            sqlProyectoPestanyaBusquedaExportacionExternaInsert = pIBD.ReplaceParam("INSERT INTO ProyectoPestanyaBusquedaExportacionExterna (ExportacionID, PestanyaID, UrlServicioExterno) VALUES (" + pIBD.GuidParamColumnaTabla("ExportacionID") + ", " + pIBD.GuidParamColumnaTabla("PestanyaID") + ", @UrlServicioExterno)");

            sqlProyectoPestanyaBusquedaExportacionExternaDelete = pIBD.ReplaceParam("DELETE FROM ProyectoPestanyaBusquedaExportacionExterna WHERE (ExportacionID = " + pIBD.GuidParamColumnaTabla("Original_ExportacionID") + " AND PestanyaID = " + pIBD.GuidParamColumnaTabla("Original_PestanyaID") + ")");

            sqlProyectoPestanyaBusquedaExportacionExternaModify = pIBD.ReplaceParam("UPDATE ProyectoPestanyaBusquedaExportacionExterna SET ExportacionID = " + pIBD.GuidParamColumnaTabla("ExportacionID") + ", PestanyaID = " + pIBD.GuidParamColumnaTabla("PestanyaID") + ", UrlServicioExterno=@UrlServicioExterno WHERE (ExportacionID = " + pIBD.GuidParamColumnaTabla("Original_ExportacionID") + " AND PestanyaID = " + pIBD.GuidParamColumnaTabla("Original_PestanyaID") + ")");

            #endregion

            #endregion
        }

        #endregion

        #region Públicos

        /// <summary>
        /// Actualiza exportaciones de búsqueda de proyecto
        /// </summary>
        public void ActualizarExportacionBusqueda()
        {
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Obtiene las exportaciones de búsqueda de un proyecto 
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset con las exportaciones de búsqueda del proyecto</returns>
        public DataWrapperExportacionBusqueda ObtenerExportacionesProyecto(Guid pProyectoID)
        {
            DataWrapperExportacionBusqueda exportacionBusquedaDW = new DataWrapperExportacionBusqueda();

            //ProyectoPestanyaBusquedaExportacion
            exportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacion = mEntityContext.ProyectoPestanyaBusquedaExportacion.JoinProyectoPestanyaMenu().Where(item => item.ProyectoPestanyaMenu.ProyectoID.Equals(pProyectoID)).Select(item => item.ProyectoPestanyaBusquedaExportacion).ToList();

            //ProyectoPestanyaBusquedaExportacionPropiedad
            exportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacionPropiedad = mEntityContext.ProyectoPestanyaBusquedaExportacionPropiedad.JoinProyectoPestanyaBusquedaExportacion().JoinProyectoPestanyaMenu().Where(item => item.ProyectoPestanyaMenu.ProyectoID.Equals(pProyectoID)).Select(item => item.ProyectoPestanyaBusquedaExportacionPropiedad).ToList();

            //ProyectoPestanyaBusquedaExportacionExterna
            exportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacionExterna = mEntityContext.ProyectoPestanyaBusquedaExportacionExterna.JoinProyectoPestanyaBusquedaExportacion().JoinProyectoPestanyaMenu().Where(item => item.ProyectoPestanyaMenu.ProyectoID.Equals(pProyectoID)).Select(item => item.ProyectoPestanyaBusquedaExportacionExterna).ToList();

            return exportacionBusquedaDW;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Define parametros de exportación a excel.
    /// </summary>
    public class TipoPropExportacion
    {
        /// <summary>
        /// Constante para título.
        /// </summary>
        public const string Titulo = "titulo";

        /// <summary>
        /// Constante para Etiquetas.
        /// </summary>
        public const string Etiquetas = "etiquetas";

        /// <summary>
        /// Constante para Descripcion.
        /// </summary>
        public const string Descripcion = "descripcion";

        /// <summary>
        /// Constante para Categorias.
        /// </summary>
        public const string Categorias = "categorias";

        /// <summary>
        /// Constante para CategoriaFiltrada.
        /// </summary>
        public const string CategoriaFiltrada = "categoriafiltrada";

        /// <summary>
        /// Constante para Publicador.
        /// </summary>
        public const string Publicador = "publicador";

        /// <summary>
        /// Constante para Publicador.
        /// </summary>
        public const string Publicadores = "publicadores";

        /// <summary>
        /// Constante para Tipo.
        /// </summary>
        public const string Tipo = "tipo";

        /// <summary>
        /// Constante para FechaPublicacion.
        /// </summary>
        public const string FechaPublicacion = "fechaPublicacion";

        /// <summary>
        /// Constante para número de descargas.
        /// </summary>
        public const string NumeroDescargas = "numerodescargas";

        /// <summary>
        /// Constante para número de consultas.
        /// </summary>
        public const string NumeroConsultas = "numeroconsultas";

        /// <summary>
        /// Constante para número de votos.
        /// </summary>
        public const string NumeroVotos = "numerovotos";

        /// <summary>
        /// Constante para el país.
        /// </summary>
        public const string Pais = "pais";

        /// <summary>
        /// Constante para la provincia.
        /// </summary>
        public const string Provincia = "provincia";

        /// <summary>
        /// Constante para la localidad.
        /// </summary>
        public const string Localidad = "localidad";

        /// <summary>
        /// Constante para la url del documento.
        /// </summary>
        public const string Url = "url";

        /// <summary>
        /// Constante para los editores del documento.
        /// </summary>
        public const string Editores = "editores";

        /// <summary>
        /// Constante para los que han comentado el documento.
        /// </summary>
        public const string Comentadores = "comentadores";

        /// <summary>
        /// Constante para saber el numero de comentarios.
        /// </summary>
        public const string NumComentarios = "numcomentarios";

        /// <summary>
        /// Constante para saber el numero de comentarios.
        /// </summary>
        public const string NumVisitas = "numvisitas";

        /// <summary>
        /// Constante para la calificacion del documento.
        /// </summary>
        public const string Calificacion = "calificacion";
    }


    /// <summary>
    /// Define parametros de exportación a excel para Personas y Organizaciones.
    /// </summary>
    public class TipoPropExportacionPersonas
    {
        /// <summary>
        /// Constante para FechaAlta.
        /// </summary>
        public const string FechaAlta = "fechaalta";

        /// <summary>
        /// Constante para Nombre.
        /// </summary>
        public const string Nombre = "nombre";

        /// <summary>
        /// Constante para Apellidos.
        /// </summary>
        public const string Apellidos = "apellidos";

        /// <summary>
        /// Constante para Mail.
        /// </summary>
        public const string Email = "email";

        /// <summary>
        /// Constante para FechaNacimiento
        /// </summary>
        public const string FechaNacimiento = "fechanacimiento";

        /// <summary>
        /// Constante para NumeroAccesos.
        /// </summary>
        public const string NumeroAccesos = "numeroaccesos";

        /// <summary>
        /// Constante para NumeroVisitas.
        /// </summary>
        public const string NumeroVisitas = "numerovisitas";

        /// <summary>
        /// Constante para número de descargas.
        /// </summary>
        public const string NumeroDescargas = "numerodescargas";

        /// <summary>
        /// Constante para el país.
        /// </summary>
        public const string Pais = "pais";

        /// <summary>
        /// Constante para la provincia.
        /// </summary>
        public const string Provincia = "provincia";

        /// <summary>
        /// Constante para la localidad.
        /// </summary>
        public const string Localidad = "localidad";

        /// <summary>
        /// Constante para la localidad.
        /// </summary>
        public const string RedesSociales = "redessociales";

        /// <summary>
        /// Constante para los numeros de recursos publicados
        /// </summary>
        public const string NumRecursosPublicados = "numerorecursospublicados";

        /// <summary>
        /// Constante para los numeros de recursos publicados
        /// </summary>
        public const string NumRecursosComentados = "numerorecursoscomentados";

        /// <summary>
        /// Constante para los numeros de recursos publicados
        /// </summary>
        public const string FechaUltimoAcceso = "fechaultimoacceso";
    }
    public class TipoDatosExtraPropiedadExportacion
    {
        public const string SepararValoresPorFila = "SepararValoresPorFila";
        public const string PropiedadTransitiva = "PropiedadTransitiva";
        public const string UltimoValorRequerido = "UltimoValorRequerido";
        public const string FiltroCategoria = "FiltroCategoria";
    }

    public class FormatosExportancion
    {
        public const string CSV = "csv";
        public const string EXCEL = "excel";
    }

}
