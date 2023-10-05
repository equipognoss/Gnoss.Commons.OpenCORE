using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Suscripcion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.AD.Tesauro
{
    #region Enumeraciones

    /// <summary>
    /// Estado en el que puede estar una sugerencia de categoría de tesauro.
    /// </summary>
    public enum EstadoSugerenciaCatTesauro
    {
        /// <summary>
        /// Sugerencia de categoría de tesauro en espera de ser aceptada
        /// </summary>
        Espera = 0,
        /// <summary>
        /// Sugerencia de categoría de tesauro aceptada
        /// </summary>
        Aceptada = 1,
        /// <summary>
        /// Sugerencia de categoría de tesauro rechazada
        /// </summary>
        Rechazada = 2
    }

    #endregion

    public class JoinTesauroProyectoCategoriaTesuaro
    {
        public TesauroProyecto TesauroProyecto { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
    }

    public class JoinTesauroOrganizacionBaseRecursosOrganizacion
    {
        public TesauroOrganizacion TesauroOrganizacion { get; set; }
        public BaseRecursosOrganizacion BaseRecursosOrganizacion { get; set; }
    }

    public class JoinTesauroUsuarioBaseRecursosUsuario
    {
        public TesauroUsuario TesauroUsuario { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
    }

    public class JoinBaseRecursosProyectoTesauroProyecto
    {
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public TesauroProyecto TesauroProyecto { get; set; }
    }

    public class JoinCatTesauroAgCatTesauroCategoriaTesauro
    {
        public CatTesauroAgCatTesauro CatTesauroAgCatTesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
    }

    public class JoinCatTesauroAgCatTesauroCategoriaTesauroTesauro
    {
        public CatTesauroAgCatTesauro CatTesauroAgCatTesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public EntityModel.Models.Tesauro.Tesauro Tesauro { get; set; }
    }

    public class JoinCatTesauroAgCatTesauroCategoriaTesauroTesauroCategoriaTesauro
    {
        public CatTesauroAgCatTesauro CatTesauroAgCatTesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public EntityModel.Models.Tesauro.Tesauro Tesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro_1 { get; set; }
    }

    public class JoinDocumentoWebAgCatTesauroDocumento
    {
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public Documento Documento { get; set; }
    }

    public class JoinTesauroUsuarioPersona
    {
        public TesauroUsuario TesauroUsuario { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinTesauroProyectoCategoriaTesuaroCatTesauroPermiteTipoRec
    {
        public TesauroProyecto TesauroProyecto { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public CatTesauroPermiteTipoRec CatTesauroPermiteTipoRec { get; set; }
    }

    public class JoinCategoriaTesauroTesauroProyecto
    {
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public TesauroProyecto TesauroProyecto { get; set; }
    }

    public class JoinTesauroOrganizacionBaseRecursosOrganizacionTesauro
    {
        public TesauroOrganizacion TesauroOrganizacion { get; set; }
        public BaseRecursosOrganizacion BaseRecursosOrganizacion { get; set; }
        public AD.EntityModel.Models.Tesauro.Tesauro Tesauro { get; set; }
    }

    public class JoinTesauroUsuarioBaseRecursosUsuarioTesauro
    {
        public TesauroUsuario TesauroUsuario { get; set; }
        public BaseRecursosUsuario BaseRecursosUsuario { get; set; }
        public AD.EntityModel.Models.Tesauro.Tesauro Tesauro { get; set; }
    }

    public class JoinBaseRecursosProyectoTesauroProyectoTesauro
    {
        public BaseRecursosProyecto BaseRecursosProyecto { get; set; }
        public TesauroProyecto TesauroProyecto { get; set; }
        public AD.EntityModel.Models.Tesauro.Tesauro Tesauro { get; set; }
    }

    public class JoinCategoriaTesauroPropiedadesTesauro
    {
        public CategoriaTesauroPropiedades CategoriaTesauroPropiedades { get; set; }
        public AD.EntityModel.Models.Tesauro.Tesauro Tesauro { get; set; }
    }

    public class CategoriaTesauroDocumentoWebAgCatTesauro
    {
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
    }

    public class CategoriaTesauroDocumentoWebAgCatTesauroDocumento
    {
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public Documento Documento { get; set; }
    }

    public class CategoriaTesauroCategoriaTesVinSuscrip
    {
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
    }

    public class CategoriaTesauroCategoriaTesVinSuscripSuscripcion
    {
        public CategoriaTesauro CategoriaTesauro { get; set; }
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class DocumentoWebAgCatTesauroCategoriaTesauro
    {
        public DocumentoWebAgCatTesauro DocumentoWebAgCatTesauro { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
    }

    public class CategoriaTesVinSuscripCategoriaTesauro
    {
        public CategoriaTesVinSuscrip CategoriaTesVinSuscrip { get; set; }
        public CategoriaTesauro CategoriaTesauro { get; set; }
    }

    public static class JoinsTesauro
    {
        public static IQueryable<CategoriaTesVinSuscripCategoriaTesauro> JoinCategoriaTesauro(this IQueryable<CategoriaTesVinSuscrip> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesauro, categoriaTesVinSuscrip => categoriaTesVinSuscrip.CategoriaTesauroID, categoriaTesauro => categoriaTesauro.CategoriaTesauroID, (categoriaTesVinSuscrip, categoriaTesauro) => new CategoriaTesVinSuscripCategoriaTesauro
            {
                CategoriaTesauro = categoriaTesauro,
                CategoriaTesVinSuscrip = categoriaTesVinSuscrip
            });
        }

        public static IQueryable<DocumentoWebAgCatTesauroCategoriaTesauro> JoinCategoriaTesauro(this IQueryable<DocumentoWebAgCatTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesauro, documentoWebAgCatTesauro => documentoWebAgCatTesauro.CategoriaTesauroID, categoriaTesauro => categoriaTesauro.CategoriaTesauroID, (documentoWebAgCatTesauro, categoriaTesauro) => new DocumentoWebAgCatTesauroCategoriaTesauro
            {
                CategoriaTesauro = categoriaTesauro,
                DocumentoWebAgCatTesauro = documentoWebAgCatTesauro
            });
        }

        public static IQueryable<CategoriaTesauroCategoriaTesVinSuscripSuscripcion> JoinSuscripcion(this IQueryable<CategoriaTesauroCategoriaTesVinSuscrip> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Suscripcion, item => item.CategoriaTesVinSuscrip.SuscripcionID, suscripcion => suscripcion.SuscripcionID, (item, suscripcion) => new CategoriaTesauroCategoriaTesVinSuscripSuscripcion
            {
                CategoriaTesauro = item.CategoriaTesauro,
                CategoriaTesVinSuscrip = item.CategoriaTesVinSuscrip,
                Suscripcion = suscripcion
            });
        }

        public static IQueryable<CategoriaTesauroCategoriaTesVinSuscrip> JoinCategoriaTesVinSuscrip(this IQueryable<CategoriaTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesVinSuscrip, categoriaTesauro => new { CategoriaTesauroID = categoriaTesauro.CategoriaTesauroID, TesauroID = categoriaTesauro.TesauroID }, categoriaTesVinSuscrip => new { CategoriaTesauroID = categoriaTesVinSuscrip.CategoriaTesauroID, TesauroID = categoriaTesVinSuscrip.TesauroID }, (categoriaTesauro, categoriaTesVinSuscrip) => new CategoriaTesauroCategoriaTesVinSuscrip
            {
                CategoriaTesVinSuscrip = categoriaTesVinSuscrip,
                CategoriaTesauro = categoriaTesauro
            });
        }

        public static IQueryable<CategoriaTesauroDocumentoWebAgCatTesauroDocumento> JoinDocumento(this IQueryable<CategoriaTesauroDocumentoWebAgCatTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, item => item.DocumentoWebAgCatTesauro.DocumentoID, documento => documento.DocumentoID, (item, documento) => new CategoriaTesauroDocumentoWebAgCatTesauroDocumento
            {
                Documento = documento,
                CategoriaTesauro = item.CategoriaTesauro,
                DocumentoWebAgCatTesauro = item.DocumentoWebAgCatTesauro
            });
        }

        public static IQueryable<CategoriaTesauroDocumentoWebAgCatTesauro> JoinDocumentoWebAgCatTesauro(this IQueryable<CategoriaTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoWebAgCatTesauro, categoriaTesauro => new { categoriaTesauro.CategoriaTesauroID, categoriaTesauro.TesauroID }, documentoWebAgCatTesauro => new { documentoWebAgCatTesauro.CategoriaTesauroID, documentoWebAgCatTesauro.TesauroID }, (categoriaTesauro, documentoWebAgCatTesauro) => new CategoriaTesauroDocumentoWebAgCatTesauro
            {
                CategoriaTesauro = categoriaTesauro,
                DocumentoWebAgCatTesauro = documentoWebAgCatTesauro
            });
        }

        public static IQueryable<JoinCategoriaTesauroPropiedadesTesauro> JoinTesauro(this IQueryable<CategoriaTesauroPropiedades> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Tesauro, categoriaTesauroPropiedades => categoriaTesauroPropiedades.TesauroID, tesauro => tesauro.TesauroID, (categoriaTesauroPropiedades, tesauro) => new JoinCategoriaTesauroPropiedadesTesauro
            {
                Tesauro = tesauro,
                CategoriaTesauroPropiedades = categoriaTesauroPropiedades
            });
        }

        public static IQueryable<JoinBaseRecursosProyectoTesauroProyectoTesauro> JoinTesauro(this IQueryable<JoinBaseRecursosProyectoTesauroProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Tesauro, item => item.TesauroProyecto.TesauroID, tesauro => tesauro.TesauroID, (item, tesauro) => new JoinBaseRecursosProyectoTesauroProyectoTesauro
            {
                Tesauro = tesauro,
                BaseRecursosProyecto = item.BaseRecursosProyecto,
                TesauroProyecto = item.TesauroProyecto
            });
        }

        public static IQueryable<JoinTesauroUsuarioBaseRecursosUsuarioTesauro> JoinTesauro(this IQueryable<JoinTesauroUsuarioBaseRecursosUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Tesauro, item => item.TesauroUsuario.TesauroID, tesauro => tesauro.TesauroID, (item, tesauro) => new JoinTesauroUsuarioBaseRecursosUsuarioTesauro
            {
                Tesauro = tesauro,
                BaseRecursosUsuario = item.BaseRecursosUsuario,
                TesauroUsuario = item.TesauroUsuario
            });
        }

        public static IQueryable<JoinTesauroOrganizacionBaseRecursosOrganizacionTesauro> JoinTesauro(this IQueryable<JoinTesauroOrganizacionBaseRecursosOrganizacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Tesauro, item => item.TesauroOrganizacion.TesauroID, tesauro => tesauro.TesauroID, (item, tesauro) => new JoinTesauroOrganizacionBaseRecursosOrganizacionTesauro
            {
                BaseRecursosOrganizacion = item.BaseRecursosOrganizacion,
                TesauroOrganizacion = item.TesauroOrganizacion,
                Tesauro = tesauro
            });
        }

        public static IQueryable<JoinCategoriaTesauroTesauroProyecto> JoinTesauroProyecto(this IQueryable<CategoriaTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.TesauroProyecto, categoriaTesauro => categoriaTesauro.TesauroID, tesauroProyecto => tesauroProyecto.TesauroID, (categoriaTesauro, tesauroProyecto) => new JoinCategoriaTesauroTesauroProyecto
            {
                CategoriaTesauro = categoriaTesauro,
                TesauroProyecto = tesauroProyecto
            });
        }

        public static IQueryable<JoinTesauroProyectoCategoriaTesuaroCatTesauroPermiteTipoRec> JoinCatTesauroPermiteTipoRec(this IQueryable<JoinTesauroProyectoCategoriaTesuaro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CatTesauroPermiteTipoRec, item => item.CategoriaTesauro.CategoriaTesauroID, catTesauroPermiteTipoRec => catTesauroPermiteTipoRec.CategoriaTesauroID, (item, catTesauroPermiteTipoRec) => new JoinTesauroProyectoCategoriaTesuaroCatTesauroPermiteTipoRec
            {
                CategoriaTesauro = item.CategoriaTesauro,
                TesauroProyecto = item.TesauroProyecto,
                CatTesauroPermiteTipoRec = catTesauroPermiteTipoRec

            });
        }

        public static IQueryable<JoinTesauroUsuarioPersona> JoinPersona(this IQueryable<TesauroUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Persona, tesauroUsuario => tesauroUsuario.UsuarioID, persona => persona.UsuarioID, (tesauroUsusario, persona) => new JoinTesauroUsuarioPersona
            {
                TesauroUsuario = tesauroUsusario,
                Persona = persona
            });
        }


        public static IQueryable<JoinDocumentoWebAgCatTesauroDocumento> JoinDocumento(this IQueryable<DocumentoWebAgCatTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, documentoWebAgCatTesauro => documentoWebAgCatTesauro.DocumentoID, documento => documento.DocumentoID, (documentoWebAgCatTesauro, documento) => new JoinDocumentoWebAgCatTesauroDocumento
            {
                DocumentoWebAgCatTesauro = documentoWebAgCatTesauro,
                Documento = documento
            });
        }

        public static IQueryable<JoinCatTesauroAgCatTesauroCategoriaTesauroTesauroCategoriaTesauro> JoinCategoriaTesauro(this IQueryable<JoinCatTesauroAgCatTesauroCategoriaTesauroTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesauro, item => new { item.CatTesauroAgCatTesauro.TesauroID, item.CatTesauroAgCatTesauro.CategoriaSuperiorID, TesauroIDTesauro = item.Tesauro.TesauroID }, categoriaTesauro_1 => new { categoriaTesauro_1.TesauroID, CategoriaSuperiorID = categoriaTesauro_1.CategoriaTesauroID, TesauroIDTesauro = categoriaTesauro_1.TesauroID }, (item, categoriaTesauro_1) => new JoinCatTesauroAgCatTesauroCategoriaTesauroTesauroCategoriaTesauro
            {
                CatTesauroAgCatTesauro = item.CatTesauroAgCatTesauro,
                CategoriaTesauro = item.CategoriaTesauro,
                Tesauro = item.Tesauro,
                CategoriaTesauro_1 = categoriaTesauro_1
            });
        }

        public static IQueryable<JoinCatTesauroAgCatTesauroCategoriaTesauroTesauro> JoinTesauro(this IQueryable<JoinCatTesauroAgCatTesauroCategoriaTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Tesauro, item => item.CategoriaTesauro.TesauroID, tesauro => tesauro.TesauroID, (item, tesauro) => new JoinCatTesauroAgCatTesauroCategoriaTesauroTesauro
            {
                CatTesauroAgCatTesauro = item.CatTesauroAgCatTesauro,
                CategoriaTesauro = item.CategoriaTesauro,
                Tesauro = tesauro
            });
        }


        public static IQueryable<JoinCatTesauroAgCatTesauroCategoriaTesauro> JoinCategoriaTesauro(this IQueryable<CatTesauroAgCatTesauro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesauro, catTesauroAgCatTesauro => new { catTesauroAgCatTesauro.TesauroID, CategoriaTesauroID = catTesauroAgCatTesauro.CategoriaInferiorID }, categoriaTesauro => new { categoriaTesauro.TesauroID, CategoriaTesauroID = categoriaTesauro.CategoriaTesauroID }, (catTesauroAgCatTesauro, categoriaTesauro) => new JoinCatTesauroAgCatTesauroCategoriaTesauro
            {
                CategoriaTesauro = categoriaTesauro,
                CatTesauroAgCatTesauro = catTesauroAgCatTesauro
            });
        }


        public static IQueryable<JoinTesauroProyectoCategoriaTesuaro> JoinCategoriaTesauro(this IQueryable<TesauroProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CategoriaTesauro, tesauroProyecto => tesauroProyecto.TesauroID, categoriaTesauro => categoriaTesauro.TesauroID, (tesauroProyecto, categoriaTesauro) => new JoinTesauroProyectoCategoriaTesuaro
            {
                TesauroProyecto = tesauroProyecto,
                CategoriaTesauro = categoriaTesauro
            });
        }

        public static IQueryable<JoinTesauroOrganizacionBaseRecursosOrganizacion> JoinTesauroOrganizacion(this IQueryable<BaseRecursosOrganizacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.TesauroOrganizacion, baseRecursosOrganizacion => baseRecursosOrganizacion.OrganizacionID, tesauroOrganizacion => tesauroOrganizacion.OrganizacionID, (baseRecursosOrganizacion, tesauroOrganizacion) => new JoinTesauroOrganizacionBaseRecursosOrganizacion
            {
                BaseRecursosOrganizacion = baseRecursosOrganizacion,
                TesauroOrganizacion = tesauroOrganizacion
            });
        }

        public static IQueryable<JoinTesauroUsuarioBaseRecursosUsuario> JoinTesauroUsuario(this IQueryable<BaseRecursosUsuario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.TesauroUsuario, baseRecursosUsuario => baseRecursosUsuario.UsuarioID, tesauroUsuario => tesauroUsuario.UsuarioID, (baseRecursosUsuario, tesauroUsuario) => new JoinTesauroUsuarioBaseRecursosUsuario
            {
                BaseRecursosUsuario = baseRecursosUsuario,
                TesauroUsuario = tesauroUsuario
            });
        }

        public static IQueryable<JoinBaseRecursosProyectoTesauroProyecto> JoinTesauroProyecto(this IQueryable<BaseRecursosProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.TesauroProyecto, baseRecursosProyecto => baseRecursosProyecto.ProyectoID, tesauroProyecto => tesauroProyecto.ProyectoID, (baseRecursosProyecto, tesauroProyecto) => new JoinBaseRecursosProyectoTesauroProyecto
            {
                BaseRecursosProyecto = baseRecursosProyecto,
                TesauroProyecto = tesauroProyecto
            });
        }

    }

    /// <summary>
    /// DataAdapter de tesauro
    /// </summary>
    public class TesauroAD : BaseAD
    {

        private EntityContext mEntityContext;

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public TesauroAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public TesauroAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
        }

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Obtiene el TesauroID y la CategoriaTesauroID que corresponden al nombre del tesauro y al proyecto dados
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pNombre">Nombre del tesauro</param>
        /// <returns>Lista con el TesauroID en la 1ª posición de la lista y CategoriaTesauroID en la 2ª. Lista null si no existen</returns>
        public List<Guid> ObtenerTesauroYCategoria(Guid pProyectoID, string pNombre)
        {
            List<Guid> lista = null;

            var resultado = mEntityContext.TesauroProyecto.JoinCategoriaTesauro().Where(item => item.TesauroProyecto.ProyectoID.Equals(pProyectoID) && item.CategoriaTesauro.Nombre.Equals(pNombre)).Select(item => new { item.TesauroProyecto.TesauroID, item.CategoriaTesauro.CategoriaTesauroID }).FirstOrDefault();

            if (resultado != null)
            {
                lista = new List<Guid>();
                lista.Add(resultado.TesauroID); //tesauroid
                lista.Add(resultado.CategoriaTesauroID); //categoriatesauroid
            }
            return lista;
        }

        /// <summary>
        /// Obtiene el identificador del tesauro de un proyecto pasado por parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Identificador del tesauro</returns>
        public Guid ObtenerIDTesauroDeProyecto(Guid pProyectoID)
        {
            return mEntityContext.TesauroProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).Select(item => item.TesauroID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el identificador del tesauro de un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Identificador de tesauro de usuario</returns>
        public Guid ObtenerIDTesauroDeUsuario(Guid pUsuarioID)
        {
            return mEntityContext.TesauroUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).Select(item => item.TesauroID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el identificador del tesauro de una organización
        /// </summary>
        /// <param name="pUsuarioID">Identificador de la organización</param>
        /// <returns>Identificador de tesauro de la organización</returns>
        public Guid ObtenerIDTesauroDeOrganizacion(Guid pOrganizacionID)
        {
            return mEntityContext.TesauroOrganizacion.Where(item => item.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.TesauroID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el tesauro de un proyecto pasado por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de tesauro con el tesauro del proyecto dado</returns>
        public DataWrapperTesauro ObtenerTesauroDeProyecto(Guid pProyectoID)
        {
            DataWrapperTesauro dataWrapperTesauro = new DataWrapperTesauro();

            dataWrapperTesauro.ListaTesauroProyecto = mEntityContext.TesauroProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            if (dataWrapperTesauro.ListaTesauroProyecto.Count > 0)
            {
                dataWrapperTesauro.Merge(ObtenerTesauroPorID(dataWrapperTesauro.ListaTesauroProyecto.FirstOrDefault().TesauroID));
            }

            return dataWrapperTesauro;
        }

        /// <summary>
        /// Obtiene los tesauros de una lista de proyectos
        /// </summary>
        /// <param name="pListaProyectos">Lista de identificadores de proyecto</param>
        /// <returns>Dataset de tesauro</returns>
        public DataWrapperTesauro ObtenerTesauroDeListaProyectos(List<Guid> pListaProyectos)
        {
            DataWrapperTesauro dataWrapperTesauro = new DataWrapperTesauro();

            dataWrapperTesauro.ListaTesauroProyecto = mEntityContext.TesauroProyecto.Where(item => pListaProyectos.Contains(item.ProyectoID)).ToList();

            List<Guid> listaTesauroProyecto = dataWrapperTesauro.ListaTesauroProyecto.Select(item => item.TesauroID).ToList();

            if (pListaProyectos.Count > 0)
            {
                dataWrapperTesauro.Merge(ObtenerTesauroPorListaIDs(listaTesauroProyecto));
            }
            return dataWrapperTesauro;
        }

        /// <summary>
        /// Obtiene un conjunto de tesauros a partir de los identificadores de tesauro pasados por parámetro
        /// </summary>
        /// <param name="pListaTesauros">Lista de identificadores de tesauro</param>
        /// <returns>Dataset de tesauro</returns>
        public DataWrapperTesauro ObtenerTesauroPorListaIDs(List<Guid> pListaTesauros)
        {
            DataWrapperTesauro dataWrapperTesauro = new DataWrapperTesauro();

            if (pListaTesauros.Count > 0)
            {
                dataWrapperTesauro.ListaTesauro = mEntityContext.Tesauro.Where(item => pListaTesauros.Contains(item.TesauroID)).ToList();

                dataWrapperTesauro.ListaCategoriaTesauro = mEntityContext.CategoriaTesauro.Where(item => pListaTesauros.Contains(item.TesauroID)).ToList();

                dataWrapperTesauro.ListaCatTesauroAgCatTesauro = mEntityContext.CatTesauroAgCatTesauro.Where(item => pListaTesauros.Contains(item.TesauroID)).ToList();

                dataWrapperTesauro.ListaCatTesauroCompartida = mEntityContext.CatTesauroCompartida.Where(item => pListaTesauros.Contains(item.TesauroDestinoID)).ToList();

                dataWrapperTesauro.ListaCategoriaTesauroPropiedades = mEntityContext.CategoriaTesauroPropiedades.JoinTesauro().Where(item => pListaTesauros.Contains(item.CategoriaTesauroPropiedades.TesauroID)).Select(item => item.CategoriaTesauroPropiedades).ToList();
            }

            return dataWrapperTesauro;
        }

        /// <summary>
        /// Obtiene un tesauro a partir del identificador de una base de recursos pasado por parámetro
        /// </summary>
        /// <param name="pBaseRecursosID">Identificador de una base de recursos</param>
        /// <returns>Dataset de tesauro</returns>
        public DataWrapperTesauro ObtenerTesauroPorBaseRecursosID(Guid pBaseRecursosID)
        {
            var query = mEntityContext.BaseRecursosOrganizacion.JoinTesauroOrganizacion().JoinTesauro().Where(item => item.BaseRecursosOrganizacion.BaseRecursosID.Equals(pBaseRecursosID)).Select(item => item.Tesauro).Concat(mEntityContext.BaseRecursosUsuario.JoinTesauroUsuario().JoinTesauro().Where(item => item.BaseRecursosUsuario.BaseRecursosID.Equals(pBaseRecursosID)).Select(item => item.Tesauro)).Concat(mEntityContext.BaseRecursosProyecto.JoinTesauroProyecto().JoinTesauro().Where(item => item.BaseRecursosProyecto.BaseRecursosID.Equals(pBaseRecursosID)).Select(item => item.Tesauro));

            DataWrapperTesauro dataWrapperTesauro = new DataWrapperTesauro();

            dataWrapperTesauro.ListaTesauro = query.ToList();

            if (dataWrapperTesauro.ListaTesauro.Count > 0)
            {
                Guid tesauroID = query.FirstOrDefault().TesauroID;

                dataWrapperTesauro.ListaCategoriaTesauro = mEntityContext.CategoriaTesauro.Where(item => item.TesauroID.Equals(tesauroID)).ToList();

                dataWrapperTesauro.ListaCatTesauroAgCatTesauro = mEntityContext.CatTesauroAgCatTesauro.JoinCategoriaTesauro().JoinTesauro().JoinCategoriaTesauro().Where(item => item.Tesauro.TesauroID.Equals(tesauroID)).Select(item => item.CatTesauroAgCatTesauro).ToList();

                dataWrapperTesauro.ListaCatTesauroCompartida = mEntityContext.CatTesauroCompartida.Where(item => item.TesauroDestinoID.Equals(pBaseRecursosID)).ToList();

                dataWrapperTesauro.ListaCategoriaTesauroPropiedades = mEntityContext.CategoriaTesauroPropiedades.JoinTesauro().Where(item => item.CategoriaTesauroPropiedades.TesauroID.Equals(pBaseRecursosID)).Select(item => item.CategoriaTesauroPropiedades).ToList();

                return dataWrapperTesauro;
            }
            return null;
        }

        /// <summary>
        /// Obtiene un tesauro completo a partir del identificador de tesauro pasado por parámetro, obtiene las tablas: "Tesauro","CategoriaTesauro" y "CatTesauroAgCatTesauro"
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Dataset de tesauro</returns>
        public DataWrapperTesauro ObtenerTesauroCompletoPorID(Guid pTesauroID)
        {
            DataWrapperTesauro dataWrapperTesauro = new DataWrapperTesauro();

            dataWrapperTesauro.ListaTesauro = mEntityContext.Tesauro.Where(item => item.TesauroID.Equals(pTesauroID)).ToList();

            dataWrapperTesauro.ListaCategoriaTesauro = mEntityContext.CategoriaTesauro.Where(item => item.TesauroID.Equals(pTesauroID)).ToList();

            dataWrapperTesauro.ListaCatTesauroAgCatTesauro = mEntityContext.CatTesauroAgCatTesauro.JoinCategoriaTesauro().JoinTesauro().JoinCategoriaTesauro().Where(item => item.Tesauro.TesauroID.Equals(pTesauroID)).Select(item => item.CatTesauroAgCatTesauro).ToList();

            dataWrapperTesauro.ListaCategoriaTesauroPropiedades = mEntityContext.CategoriaTesauroPropiedades.Where(item => item.TesauroID.Equals(pTesauroID)).ToList();

            dataWrapperTesauro.ListaCatTesauroCompartida = mEntityContext.CatTesauroCompartida.Where(item => item.TesauroDestinoID.Equals(pTesauroID)).ToList();

            return dataWrapperTesauro;
        }

        /// <summary>
        /// Obtiene los tesauros de una lista de usuarios
        /// </summary>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuarios</param>
        /// <returns>Dataset de tesauro con los datos</returns>
        public DataWrapperTesauro ObtenerTesaurosDeListaUsuarios(List<Guid> pListaUsuariosID)
        {
            DataWrapperTesauro dataWrapperTesauro = new DataWrapperTesauro();

            if (pListaUsuariosID.Count > 0)
            {
                dataWrapperTesauro.ListaTesauroUsuario = mEntityContext.TesauroUsuario.Where(item => pListaUsuariosID.Contains(item.UsuarioID)).ToList();

                dataWrapperTesauro.Merge(ObtenerTesauroPorListaIDs(dataWrapperTesauro.ListaTesauroUsuario.Select(item => item.TesauroID).ToList()));
            }

            return dataWrapperTesauro;
        }

        /// <summary>
        /// Obtiene el tesauro de un usuario a través de su personaID.
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>DataSet Tesauro de un usuario a través de su personaID</returns>
        public DataWrapperTesauro ObtenerTesauroUsuarioPorPersonaID(Guid pPersonaID)
        {
            DataWrapperTesauro dataWrapperTesauro = new DataWrapperTesauro();

            dataWrapperTesauro.ListaTesauroUsuario = mEntityContext.TesauroUsuario.JoinPersona().Where(item => item.Persona.PersonaID.Equals(pPersonaID)).Select(item => item.TesauroUsuario).ToList();
            dataWrapperTesauro.Merge(ObtenerTesauroPorListaIDs(dataWrapperTesauro.ListaTesauroUsuario.Select(item => item.TesauroID).ToList()));

            return dataWrapperTesauro;
        }

        /// <summary>
        /// Obtiene los tesauros de un conjunto de organizacioens
        /// </summary>
        /// <param name="pListaOrganizaciones">Lista de identificadores de organizaciones</param>
        /// <returns>Dataset de tesauro</returns>
        public DataWrapperTesauro ObtenerTesaurosDeListaOrganizaciones(List<Guid> pListaOrganizaciones)
        {
            DataWrapperTesauro dataWrapperTesauro = new DataWrapperTesauro();

            if (pListaOrganizaciones.Count > 0)
            {
                dataWrapperTesauro.ListaTesauroOrganizacion = mEntityContext.TesauroOrganizacion.Where(item => pListaOrganizaciones.Contains(item.OrganizacionID)).ToList();

                dataWrapperTesauro.Merge(ObtenerTesauroPorListaIDs(dataWrapperTesauro.ListaTesauroOrganizacion.Select(item => item.TesauroID).ToList()));
            }

            return dataWrapperTesauro;
        }


        /// <summary>
        /// Obtiene el nombre de una categoria a partir de CategoriaTesauroID
        /// </summary>
        /// <param name="pCategoriaTesauroID">CategoriaTesauroID</param>
        /// <returns>string con el nombre de la categoria</returns>
        public string ObtenerNombreCategoriaPorID(Guid pCategoriaTesauroID, string pIdioma)
        {
            string nombre = mEntityContext.CategoriaTesauro.Where(item => item.CategoriaTesauroID.Equals(pCategoriaTesauroID)).Select(item => item.Nombre).FirstOrDefault();

            if (string.IsNullOrEmpty(pIdioma))
            {
                return nombre;
            }
            else
            {
                return UtilCadenas.ObtenerTextoDeIdioma(nombre, pIdioma, null);
            }
        }

        public string ObtenerNombreTesauroProyOnt(Guid pProyectoID, string pOntologiaID)
        {
            return mEntityContext.TesauroProyecto.JoinCategoriaTesauro().JoinCatTesauroPermiteTipoRec().Where(item => item.TesauroProyecto.ProyectoID.Equals(pProyectoID) && item.CatTesauroPermiteTipoRec.OntologiasID.Equals(pOntologiaID)).Select(item => item.CategoriaTesauro.Nombre).FirstOrDefault();
        }

        /// <summary>
        /// Comprueba si una categoría del tesauro está vinculada o no a algún elemento de Gnoss
        /// </summary>
        /// <param name="pTesauroID">Identificador de tesauro</param>
        /// <param name="pCategoriasID">Lista de identificadores de categoría</param>
        /// <returns>TRUE si está vinculado, FALSE en caso contrario</returns>
        public bool EstanVinculadasCategoriasTesauro(Guid pTesauroID, List<Guid> pCategoriasID)
        {
            bool vinculado = false;

            if (pCategoriasID.Count > 0)
            {
                var sqlSelectCategoriasRecursos = mEntityContext.CategoriaTesauro.JoinDocumentoWebAgCatTesauro().JoinDocumento().Where(item => item.CategoriaTesauro.TesauroID.Equals(pTesauroID) && pCategoriasID.Contains(item.CategoriaTesauro.CategoriaTesauroID)).Select(item => item.CategoriaTesauro.CategoriaTesauroID);

                var sqlSelectCategoriasSuscripciones = mEntityContext.CategoriaTesauro.JoinCategoriaTesVinSuscrip().JoinSuscripcion().Where(item => item.CategoriaTesauro.TesauroID.Equals(pTesauroID) && pCategoriasID.Contains(item.CategoriaTesauro.CategoriaTesauroID)).Select(item => item.CategoriaTesauro.CategoriaTesauroID);

                var consultaFinal = sqlSelectCategoriasRecursos.Union(sqlSelectCategoriasSuscripciones);
                return consultaFinal.Any();
            }
            return vinculado;
        }

        /// <summary>
        /// Obtiene si existen elementos vinculados con una categoría de forma no exclusiva (también están relacionados con otras categorías)
        /// </summary>
        /// <param name="pTesauroID">Identificador de tesauro</param>
        /// <param name="pCategoriasID">Lista de identificadores de categorías</param>
        /// <returns>Obtiene si existen elementos vinculados con una categoría de forma no exclusiva (también están relacionados con otras categorías)</returns>
        public bool ObtenerSiExistenElementosNoHuerfanos(Guid pTesauroID, List<Guid> pCategoriasID)
        {
            List<Guid> listaIDSubconsultaCategoriaRecursos = mEntityContext.CategoriaTesauro.JoinDocumentoWebAgCatTesauro().JoinDocumento().Where(item => item.Documento.UltimaVersion.Equals(true) && item.CategoriaTesauro.TesauroID.Equals(pTesauroID) && pCategoriasID.Contains(item.CategoriaTesauro.CategoriaTesauroID)).Select(item => item.DocumentoWebAgCatTesauro.DocumentoID).Distinct().ToList();

            var sqlSelectCategoriasRecursos = mEntityContext.DocumentoWebAgCatTesauro.JoinCategoriaTesauro().Where(item => listaIDSubconsultaCategoriaRecursos.Contains(item.DocumentoWebAgCatTesauro.DocumentoID) && !pCategoriasID.Contains(item.CategoriaTesauro.CategoriaTesauroID));


            List<Guid> listaIDSubconsultaCategoriaSuscripciones = mEntityContext.CategoriaTesauro.JoinCategoriaTesVinSuscrip().JoinSuscripcion().Where(item => item.Suscripcion.Bloqueada.Equals(false) && item.CategoriaTesauro.TesauroID.Equals(pTesauroID) && pCategoriasID.Contains(item.CategoriaTesauro.CategoriaTesauroID)).Select(item => item.CategoriaTesVinSuscrip.SuscripcionID).Distinct().ToList();

            var sqlSelectCategoriasSuscripciones = mEntityContext.CategoriaTesVinSuscrip.JoinCategoriaTesauro().Where(item => listaIDSubconsultaCategoriaSuscripciones.Contains(item.CategoriaTesVinSuscrip.SuscripcionID) && !pCategoriasID.Contains(item.CategoriaTesauro.CategoriaTesauroID));

            if (sqlSelectCategoriasRecursos.Any() || sqlSelectCategoriasSuscripciones.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Obtiene el identificador de una categoría a partir de su nombre y del proyecto al que pertenece
        /// </summary>
        /// <param name="pNombre">Nombre de categoría de tesauro</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Identificador de categoría</returns>
        public Guid ObtenerCategoriaIDPorNombreYProyecto(string pNombre, Guid pProyectoID, string pIdioma)
        {
            //REvisar
            Guid categoriaID = mEntityContext.CategoriaTesauro.JoinTesauroProyecto().Where(item => item.TesauroProyecto.ProyectoID.Equals(pProyectoID) && (item.CategoriaTesauro.Nombre.ToLower().Equals(pNombre.ToLower()) || item.CategoriaTesauro.Nombre.Contains(pIdioma))).Select(item => item.CategoriaTesauro.CategoriaTesauroID).FirstOrDefault();

            if (categoriaID != null)
                return categoriaID;
            else
                return Guid.Empty;

        }

        public List<Guid> ObtenerCategoriasHijas(List<Guid> pListaCategorias)
        {
            return mEntityContext.CatTesauroAgCatTesauro.Where(item => pListaCategorias.Contains(item.CategoriaSuperiorID)).Select(item => item.CategoriaInferiorID).ToList();
        }

        /// <summary>
        /// Obtiene todas las sugerencias pendientes de categorías para un tesauro
        /// </summary>
        /// <param name="pTesauroID">Identificador del tesauro</param>
        /// <returns>Dataset de tesauro</returns>
        public DataWrapperTesauro ObtenerSugerenciasCatDeUnTesauro(Guid pTesauroID)
        {
            DataWrapperTesauro tesauroDW = new DataWrapperTesauro();

            tesauroDW.ListaCategoriaTesauroSugerencia = mEntityContext.CategoriaTesauroSugerencia.Where(item => item.TesauroSugerenciaID.Equals(pTesauroID)).ToList();

            return tesauroDW;
        }

        /// <summary>
        /// Obtiene las categorías que permiten solo ciertos tipos de recurso.
        /// </summary>
        /// <param name="pTesauroID">ID de tesauro</param>
        /// <returns>DataSet con la tabla CatTesauroPermiteTipoRec</returns>
        public DataWrapperTesauro ObtenerCategoriasPermitidasPorTipoRecurso(Guid pTesauroID)
        {
            DataWrapperTesauro tesDW = new DataWrapperTesauro();

            tesDW.ListaCatTesauroPermiteTipoRec = mEntityContext.CatTesauroPermiteTipoRec.Where(item => item.TesauroID.Equals(pTesauroID)).ToList();

            return tesDW;
        }

        /// <summary>
        /// Actualiza un tesauro
        /// </summary>
        /// <param name="pTesauroDS">Dataset de tesauro</param>
        public void ActualizarTesauro()
        {
            try
            {
                mEntityContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Privados


        /// <summary>
        /// Obtiene un tesauro a partir del identificador pasado por parámetro
        /// </summary>
        /// <param name="pTesauroID">Identificador de tesauro</param>
        /// <returns>Dataset de tesauro</returns>
        private DataWrapperTesauro ObtenerTesauroPorID(Guid pTesauroID)
        {
            List<Guid> listaIDs = new List<Guid>();
            listaIDs.Add(pTesauroID);

            return ObtenerTesauroPorListaIDs(listaIDs);
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el identificador de categoría de tesauro para autoguardado
        /// </summary>
        public static Guid CategoriaTesauroAutoguardado
        {
            get
            {
                return new Guid("11111111-1111-1111-1111-111111111111");
            }
        }

        #endregion
    }
}
