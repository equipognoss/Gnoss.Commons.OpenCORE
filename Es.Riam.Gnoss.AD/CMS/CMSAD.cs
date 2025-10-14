using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.CMS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Es.Riam.Gnoss.AD.CMS
{
    public class JoinCMSPropiedadComponenteCMSComponente
    {
        public CMSPropiedadComponente CMSPropiedadComponente { get; set; }
        public CMSComponente CMSComponente { get; set; }
    }

    public class JoinCMSComponenteRolGrupoIdentidadesCMSComponente
    {
        public CMSComponenteRolGrupoIdentidades CMSComponenteRolGrupoIdentidades { get; set; }
        public CMSComponente CMSComponente { get; set; }
    }

    public class JoinCMSComponenteRolIdentidadCMSComponente
    {
        public CMSComponenteRolIdentidad CMSComponenteRolIdentidad { get; set; }
        public CMSComponente CMSComponente { get; set; }
    }

    public class JoinCMSBloqueComponenteCMSBloque
    {
        public CMSBloqueComponente CMSBloqueComponente { get; set; }
        public CMSBloque CMSBloque { get; set; }
    }

    public class JoinCMSBloqueComponenteCMSBloqueCMSPagina
    {
        public CMSBloqueComponente CMSBloqueComponente { get; set; }
        public CMSBloque CMSBloque { get; set; }
        public CMSPagina CMSPagina { get; set; }
    }

    public class JoinCMSBloqueComponenteCMSBloqueCMSPaginaProyectoPestanyaCMS
    {
        public CMSBloqueComponente CMSBloqueComponente { get; set; }
        public CMSBloque CMSBloque { get; set; }
        public CMSPagina CMSPagina { get; set; }
        public ProyectoPestanyaCMS ProyectoPestanyaCMS { get; set; }
    }

    public class JoinCMSBloqueComponenteCMSBloqueCMSPaginaProyectoPestanyaCMSProyectoPestanyaMenu
    {
        public CMSBloqueComponente CMSBloqueComponente { get; set; }
        public CMSBloque CMSBloque { get; set; }
        public CMSPagina CMSPagina { get; set; }
        public ProyectoPestanyaCMS ProyectoPestanyaCMS { get; set; }
        public ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }
    }

    public class JoinCMSBloqueComponenteCMSBloqueCMSComponente
    {
        public CMSBloqueComponente CMSBloqueComponente { get; set; }
        public CMSBloque CMSBloque { get; set; }
        public CMSComponente CMSComponente { get; set; }
    }

    public class JoinCMSBloqueComponenteCMSBloqueCMSComponenteCMSPropiedadComponente
    {
        public CMSBloqueComponente CMSBloqueComponente { get; set; }
        public CMSBloque CMSBloque { get; set; }
        public CMSComponente CMSComponente { get; set; }

        public CMSPropiedadComponente CMSPropiedadComponente { get; set; }
    }

    public class JoinCMSBloqueComponenteCMSBloqueCMSComponenteCMSComponenteRolGrupoIdentidades
    {
        public CMSBloqueComponente CMSBloqueComponente { get; set; }
        public CMSBloque CMSBloque { get; set; }
        public CMSComponente CMSComponente { get; set; }
        public CMSComponenteRolGrupoIdentidades CMSComponenteRolGrupoIdentidades { get; set; }
    }

    public class JoinCMSBloqueComponenteCMSBloqueCMSComponenteCMSComponenteRolIdentidad
    {
        public CMSBloqueComponente CMSBloqueComponente { get; set; }
        public CMSBloque CMSBloque { get; set; }
        public CMSComponente CMSComponente { get; set; }
        public CMSComponenteRolIdentidad CMSComponenteRolIdentidad { get; set; }
    }

    public class JoinCMSBloqueComponentePropiedadComponenteCMSBloque
    {
        public CMSBloque CMSBloque { get; set; }
        public CMSBloqueComponentePropiedadComponente CMSBloqueComponentePropiedadComponente { get; set; }
    }

    public class JoinCMSComponenteCMSPropiedadComponente
    {
        public CMSComponente CMSComponente { get; set; }
        public CMSPropiedadComponente CMSPropiedadComponente { get; set; }
    }

    public class JoinCMSPaginaCMSBloque
    {
        public CMSPagina CMSPagina { get; set; }
        public CMSBloque CMSBloque { get; set; }
    }

    public class JoinCMSPaginaCMSBloqueCMSBloqueComponente
    {
        public CMSPagina CMSPagina { get; set; }
        public CMSBloque CMSBloque { get; set; }
        public CMSBloqueComponente CMSBloqueComponente { get; set; }
    }

    //CMSBloqueComponentePropiedadComponente INNER JOIN CMSBloque on CMSBloque.BloqueID=CMSBloqueComponentePropiedadComponente.BloqueID

    public static class JoinsCMS
    {
        public static IQueryable<JoinCMSPaginaCMSBloqueCMSBloqueComponente> JoinCMSBloqueComponente(this IQueryable<JoinCMSPaginaCMSBloque> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CMSBloqueComponente, item => item.CMSBloque.BloqueID, cmsBloqueComponente => cmsBloqueComponente.BloqueID, (item, cmsBloqueComponente) => new JoinCMSPaginaCMSBloqueCMSBloqueComponente
            {
                CMSBloque = item.CMSBloque,
                CMSBloqueComponente = cmsBloqueComponente,
                CMSPagina = item.CMSPagina
            });
        }

        public static IQueryable<JoinCMSPaginaCMSBloque> JoinCMSBloque(this IQueryable<CMSPagina> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CMSBloque, cmsPagina => cmsPagina.Ubicacion, cmsBloque => cmsBloque.Ubicacion, (cmsPagina, cmsBloque) => new JoinCMSPaginaCMSBloque
            {
                CMSBloque = cmsBloque,
                CMSPagina = cmsPagina
            });
        }

        public static IQueryable<JoinCMSComponenteCMSPropiedadComponente> JoinCMSPropiedadComponente(this IQueryable<CMSComponente> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CMSPropiedadComponente, cmsComponente => cmsComponente.ComponenteID, cmsPropiedadComponente => cmsPropiedadComponente.ComponenteID, (cmsComponente, cmsPropiedadComponente) => new JoinCMSComponenteCMSPropiedadComponente
            {
                CMSComponente = cmsComponente,
                CMSPropiedadComponente = cmsPropiedadComponente
            });
        }

        public static IQueryable<JoinCMSBloqueComponentePropiedadComponenteCMSBloque> JoinCMSBloque(this IQueryable<CMSBloqueComponentePropiedadComponente> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CMSBloque, cmsBloqueComponentePropiedadComponente => cmsBloqueComponentePropiedadComponente.BloqueID, cmsBloque => cmsBloque.BloqueID, (cmsBloqueComponentePropiedadComponente, cmsBloque) => new JoinCMSBloqueComponentePropiedadComponenteCMSBloque
            {
                CMSBloqueComponentePropiedadComponente = cmsBloqueComponentePropiedadComponente,
                CMSBloque = cmsBloque
            });
        }

        public static IQueryable<JoinCMSBloqueComponenteCMSBloqueCMSComponenteCMSComponenteRolIdentidad> JoinCMSComponenteRolIdentidad(this IQueryable<JoinCMSBloqueComponenteCMSBloqueCMSComponente> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CMSComponenteRolIdentidad, item => item.CMSComponente.ComponenteID, cmsComponenteRolIdentidad => cmsComponenteRolIdentidad.ComponenteID, (item, cmsComponenteRolIdentidad) => new JoinCMSBloqueComponenteCMSBloqueCMSComponenteCMSComponenteRolIdentidad
            {
                CMSComponenteRolIdentidad = cmsComponenteRolIdentidad,
                CMSBloque = item.CMSBloque,
                CMSBloqueComponente = item.CMSBloqueComponente,
                CMSComponente = item.CMSComponente
            });
        }

        public static IQueryable<JoinCMSBloqueComponenteCMSBloqueCMSComponenteCMSComponenteRolGrupoIdentidades> JoinCMSComponenteRolGrupoIdentidades(this IQueryable<JoinCMSBloqueComponenteCMSBloqueCMSComponente> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CMSComponenteRolGrupoIdentidades, item => item.CMSComponente.ComponenteID, cmsComponenteRolGrupoIdentidades => cmsComponenteRolGrupoIdentidades.ComponenteID, (item, cmsComponenteRolGrupoIdentidades) => new JoinCMSBloqueComponenteCMSBloqueCMSComponenteCMSComponenteRolGrupoIdentidades
            {
                CMSComponenteRolGrupoIdentidades = cmsComponenteRolGrupoIdentidades,
                CMSBloque = item.CMSBloque,
                CMSBloqueComponente = item.CMSBloqueComponente,
                CMSComponente = item.CMSComponente
            });
        }

        public static IQueryable<JoinCMSBloqueComponenteCMSBloqueCMSComponenteCMSPropiedadComponente> JoinCMSPropiedadComponente(this IQueryable<JoinCMSBloqueComponenteCMSBloqueCMSComponente> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CMSPropiedadComponente, item => item.CMSComponente.ComponenteID, cmsPropiedadComponente => cmsPropiedadComponente.ComponenteID, (item, cmsPropiedadComponente) => new JoinCMSBloqueComponenteCMSBloqueCMSComponenteCMSPropiedadComponente
            {
                CMSPropiedadComponente = cmsPropiedadComponente,
                CMSBloque = item.CMSBloque,
                CMSBloqueComponente = item.CMSBloqueComponente,
                CMSComponente = item.CMSComponente
            });
        }

        public static IQueryable<JoinCMSBloqueComponenteCMSBloqueCMSComponente> JoinCMSComponente(this IQueryable<JoinCMSBloqueComponenteCMSBloque> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CMSComponente, item => item.CMSBloqueComponente.ComponenteID, cmsComponente => cmsComponente.ComponenteID, (item, cmsComponente) => new JoinCMSBloqueComponenteCMSBloqueCMSComponente
            {
                CMSBloqueComponente = item.CMSBloqueComponente,
                CMSBloque = item.CMSBloque,
                CMSComponente = cmsComponente
            });
        }

        public static IQueryable<JoinCMSBloqueComponenteCMSBloque> JoinCMSBloque(this IQueryable<CMSBloqueComponente> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CMSBloque, cmsBloqueComponente => cmsBloqueComponente.BloqueID, cmsBloque => cmsBloque.BloqueID, (cmsBloqueComponente, cmsBloque) => new JoinCMSBloqueComponenteCMSBloque
            {
                CMSBloqueComponente = cmsBloqueComponente,
                CMSBloque = cmsBloque
            });
        }

        public static IQueryable<JoinCMSBloqueComponenteCMSBloqueCMSPagina> JoinCMSPagina(this IQueryable<JoinCMSBloqueComponenteCMSBloque> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CMSPagina, item => item.CMSBloque.Ubicacion, cmsPagina => cmsPagina.Ubicacion, (item, cmsPagina) => new JoinCMSBloqueComponenteCMSBloqueCMSPagina
            {
                CMSBloqueComponente = item.CMSBloqueComponente,
                CMSBloque = item.CMSBloque,
                CMSPagina = cmsPagina
            });
        }

        public static IQueryable<JoinCMSBloqueComponenteCMSBloqueCMSPaginaProyectoPestanyaCMS> JoinProyectoPestanyaCMS(this IQueryable<JoinCMSBloqueComponenteCMSBloqueCMSPagina> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ProyectoPestanyaCMS, item => item.CMSPagina.Ubicacion, proyectoPestanyaCMS => proyectoPestanyaCMS.Ubicacion, (item, proyectoPestanyaCMS) => new JoinCMSBloqueComponenteCMSBloqueCMSPaginaProyectoPestanyaCMS
            {
                CMSBloqueComponente = item.CMSBloqueComponente,
                CMSBloque = item.CMSBloque,
                CMSPagina = item.CMSPagina,
                ProyectoPestanyaCMS = proyectoPestanyaCMS
            });
        }

        public static IQueryable<JoinCMSBloqueComponenteCMSBloqueCMSPaginaProyectoPestanyaCMSProyectoPestanyaMenu> JoinProyectoPestanyaMenu(this IQueryable<JoinCMSBloqueComponenteCMSBloqueCMSPaginaProyectoPestanyaCMS> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ProyectoPestanyaMenu, item => item.ProyectoPestanyaCMS.PestanyaID, proyectoPestanyaMenu => proyectoPestanyaMenu.PestanyaID, (item, proyectoPestanyaMenu) => new JoinCMSBloqueComponenteCMSBloqueCMSPaginaProyectoPestanyaCMSProyectoPestanyaMenu
            {
                CMSBloqueComponente = item.CMSBloqueComponente,
                CMSBloque = item.CMSBloque,
                CMSPagina = item.CMSPagina,
                ProyectoPestanyaCMS = item.ProyectoPestanyaCMS,
                ProyectoPestanyaMenu = proyectoPestanyaMenu
            });
        }

        public static IQueryable<JoinCMSPropiedadComponenteCMSComponente> JoinCMSComponente(this IQueryable<CMSPropiedadComponente> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CMSComponente, cmsPropiedadComponente => cmsPropiedadComponente.ComponenteID, cmsComponente => cmsComponente.ComponenteID, (cmsPropiedadComponente, cmsComponente) => new JoinCMSPropiedadComponenteCMSComponente
            {
                CMSComponente = cmsComponente,
                CMSPropiedadComponente = cmsPropiedadComponente
            });
        }

        public static IQueryable<JoinCMSComponenteRolGrupoIdentidadesCMSComponente> JoinCMSComponente(this IQueryable<CMSComponenteRolGrupoIdentidades> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CMSComponente, cmsComponenteRolGrupoIdentidades => cmsComponenteRolGrupoIdentidades.ComponenteID, cmsComponente => cmsComponente.ComponenteID, (cmsComponenteRolGrupoIdentidades, cmsComponente) => new JoinCMSComponenteRolGrupoIdentidadesCMSComponente
            {
                CMSComponente = cmsComponente,
                CMSComponenteRolGrupoIdentidades = cmsComponenteRolGrupoIdentidades
            });
        }

        public static IQueryable<JoinCMSComponenteRolIdentidadCMSComponente> JoinCMSComponente(this IQueryable<CMSComponenteRolIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.CMSComponente, cmsComponenteRolIdentidad => cmsComponenteRolIdentidad.ComponenteID, cmsComponente => cmsComponente.ComponenteID, (cmsComponenteRolIdentidad, cmsComponente) => new JoinCMSComponenteRolIdentidadCMSComponente
            {
                CMSComponenteRolIdentidad = cmsComponenteRolIdentidad,
                CMSComponente = cmsComponente
            });
        }
    }

    /// <summary>
    /// DataAdapter de proyecto
    /// </summary>
    public class CMSAD : BaseAD
    {
        private EntityContext mEntityContext;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// Constructor por defecto, sin par�metros, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public CMSAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CMSAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mEntityContext = entityContext;
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Constructor a partir del fichero de configuraci�n de conexi�n a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuraci�n de la conexi�n a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se est�n usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public CMSAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CMSAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        //Consultas parte select
        private string sqlSelectCMSPagina;
        private string sqlSelectCMSBloque;
        private string sqlSelectCMSBloqueComponente;
        private string sqlSelectCMSComponente;
        private string sqlSelectCMSPropiedadComponente;
        private string sqlSelectCMSComponentePrivadoProyecto;
        private string sqlSelectCMSBloqueComponentePropiedadComponente;
        private string sqlSelectCMSComponenteRolIdentidad;
        private string sqlSelectCMSComponenteRolGrupoIdentidades;

        //Consultas mas complejas
        private string sqlSelectCMSPaginaPorProyectoID;
        private string sqlSelectCMSBloquePorProyectoID;
        private string sqlSelectCMSBloqueComponentePorProyectoID;
        private string sqlSelectCMSComponentePorProyectoID;
        private string sqlSelectCMSPropiedadComponentePorProyectoID;
        private string sqlSelectCMSComponentePrivadoProyectoPorProyectoID;
        private string sqlSelectCMSBloqueComponentePropiedadComponentePorProyectoID;
        private string sqlSelectCMSComponenteRolGrupoIdentidadesPorProyectoID;
        private string sqlSelectCMSComponenteRolIdentidadPorProyectoID;

        private string sqlSelectCMSComponentePorProyectoIDYTipoComponente;
        private string sqlSelectCMSPropiedadComponentePorProyectoIDYTipoComponente;

        private string sqlSelectCMSPaginaPorUbicacionYProyectoID;
        private string sqlSelectCMSBloquePorUbicacionYProyectoID;
        private string sqlSelectCMSBloqueComponentePorUbicacionYProyectoID;
        private string sqlSelectCMSBloqueComponentePropiedadComponentePorUbicacionYProyectoID;
        private string sqlSelectCMSComponentePorUbicacionYProyectoID;
        private string sqlSelectCMSPropiedadComponentePorUbicacionYProyectoID;
        private string sqlSelectCMSComponenteRolGrupoIdentidadesPorUbicacionYProyectoID;
        private string sqlSelectCMSComponenteRolIdentidadPorUbicacionYProyectoID;
        private string sqlSelectCMSBloqueComponentePropiedadComponentePorProyectoIDYComponenteID;

        #endregion

        #region DataAdapters

        #region CMSPagina

        private string sqlCMSPaginaInsert;
        private string sqlCMSPaginaDelete;
        private string sqlCMSPaginaModify;

        #endregion

        #region CMSBloque

        private string sqlCMSBloqueInsert;
        private string sqlCMSBloqueDelete;
        private string sqlCMSBloqueModify;

        #endregion

        #region CMSBloqueComponente

        private string sqlCMSBloqueComponenteInsert;
        private string sqlCMSBloqueComponenteDelete;
        private string sqlCMSBloqueComponenteModify;

        #endregion

        #region CMSComponente

        private string sqlCMSComponenteInsert;
        private string sqlCMSComponenteDelete;
        private string sqlCMSComponenteModify;

        #endregion

        #region CMSPropiedadComponente

        private string sqlCMSPropiedadComponenteInsert;
        private string sqlCMSPropiedadComponenteDelete;
        private string sqlCMSPropiedadComponenteModify;

        #endregion

        #region CMSComponentePrivadoProyecto

        private string sqlCMSComponentePrivadoProyectoInsert;
        private string sqlCMSComponentePrivadoProyectoDelete;
        private string sqlCMSComponentePrivadoProyectoModify;

        #endregion

        #region CMSBloqueComponentePropiedadComponente

        private string sqlCMSBloqueComponentePropiedadComponenteInsert;
        private string sqlCMSBloqueComponentePropiedadComponenteDelete;
        private string sqlCMSBloqueComponentePropiedadComponenteModify;

        #endregion

        #region CMSComponenteRolIdentidad

        private string sqlCMSComponenteRolIdentidadInsert;
        private string sqlCMSComponenteRolIdentidadDelete;
        private string sqlCMSComponenteRolIdentidadModify;

        #endregion

        #region CMSComponenteRolGrupoIdentidades

        private string sqlCMSComponenteRolGrupoIdentidadesInsert;
        private string sqlCMSComponenteRolGrupoIdentidadesDelete;
        private string sqlCMSComponenteRolGrupoIdentidadesModify;

        #endregion

        #endregion

        #region M�todos generales

        #region P�blicos

        /// <summary>
        /// Obtiene si un proyecto tiene algun componente con caducidad de tipo recurso
        /// </summary>       
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>TRUE si tiene algun componente con caducidad de tipo recurso</returns>
        public bool ObtenerSiTieneComponenteConCaducidadTipoRecurso(Guid pProyectoID)
        {
            return mEntityContext.CMSComponente.Any(item => item.ProyectoID.Equals(pProyectoID));
        }


        /// <summary>
        /// Obtiene las tablas CMSPagina, CMSBLoque, CMSBLoqueComponente, CMSComponente, CMSPropiedadComponente de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerCMSDeProyecto(Guid pProyectoID)
        {
            DataWrapperCMS dataWrapperCMS = new DataWrapperCMS();

            dataWrapperCMS.ListaCMSPagina = mEntityContext.CMSPagina.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperCMS.ListaCMSBloque = mEntityContext.CMSBloque.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperCMS.ListaCMSBloqueComponente = mEntityContext.CMSBloqueComponente.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperCMS.ListaCMSComponente = mEntityContext.CMSComponente.Where(item => item.ProyectoID.Equals(pProyectoID)).OrderBy(item => item.Nombre).ToList();
            dataWrapperCMS.ListaCMSPropiedadComponente = mEntityContext.CMSPropiedadComponente.JoinCMSComponente().Where(item => item.CMSComponente.ProyectoID.Equals(pProyectoID)).Select(item => item.CMSPropiedadComponente).ToList();
            dataWrapperCMS.ListaCMSComponentePrivadoProyecto = mEntityContext.CMSComponentePrivadoProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperCMS.ListaCMSBloqueComponentePropiedadComponente = mEntityContext.CMSBloqueComponentePropiedadComponente.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperCMS.ListaCMSComponenteRolGrupoIdentidades = mEntityContext.CMSComponenteRolGrupoIdentidades.JoinCMSComponente().Where(item => item.CMSComponente.ProyectoID.Equals(pProyectoID)).Select(item => item.CMSComponenteRolGrupoIdentidades).ToList();
            dataWrapperCMS.ListaCMSComponenteRolIdentidad = mEntityContext.CMSComponenteRolIdentidad.JoinCMSComponente().Where(item => item.CMSComponente.ProyectoID.Equals(pProyectoID)).Select(item => item.CMSComponenteRolIdentidad).ToList();

            return dataWrapperCMS;
        }

        /// <summary>
        /// Obtiene el Dataset CMSDS de la comunidad, las tablas CMSPagina, CMSBloque, CMSBloqueComponente
        /// </summary>
        /// <param name="pProyecto">Guid del proyecto</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerConfiguracionCMSPorProyecto(Guid pProyectoID)
        {
            DataWrapperCMS dataWrapperCMS = new DataWrapperCMS();

            dataWrapperCMS.ListaCMSPagina = mEntityContext.CMSPagina.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperCMS.ListaCMSBloque = mEntityContext.CMSBloque.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperCMS.ListaCMSBloqueComponente = mEntityContext.CMSBloqueComponente.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperCMS.ListaCMSComponentePrivadoProyecto = mEntityContext.CMSComponentePrivadoProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperCMS.ListaCMSBloqueComponentePropiedadComponente = mEntityContext.CMSBloqueComponentePropiedadComponente.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            return dataWrapperCMS;
        }

        public void BorrarPaginaCMS(Guid pMyGnoss, Guid pClave, short pTipoUbicacion)
        {
            //Revisar
            CMSPagina paginaCMS = mEntityContext.CMSPagina.Where(item => item.OrganizacionID.Equals(pMyGnoss) && item.ProyectoID.Equals(pClave) && item.Ubicacion.Equals(pTipoUbicacion)).FirstOrDefault();

            mEntityContext.EliminarElemento(paginaCMS);
            ActualizarBaseDeDatos();
        }

        /// <summary>
        /// Comprueba si un componete CMS pertenece a un grupo de componentes (TipoPropiedadComponente = 6)
        /// </summary>
        /// <param name="componenteID">Id del componete a verificar</param>
        /// <returns>true si pertenece a un grupo de componentes, falso en caso contrario</returns>
        public bool ComponenteCMSPerteneceGrupo(Guid componenteID)
        {
            List<string> valoresPropiedades= mEntityContext.CMSPropiedadComponente.Where(item => item.TipoPropiedadComponente == 6).Select(item=>item.ValorPropiedad).ToList();
            List<string> pertenecenAunGrupo = new List<string>();
            foreach(string valor in valoresPropiedades)
            {
                string[] listadoGrupo= valor.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string id in listadoGrupo)
                {
                    pertenecenAunGrupo.Add(id);
                }
            }
            return pertenecenAunGrupo.Contains(componenteID.ToString());

        }
        public void ActualizarBaseDeDatos()
        {
            mEntityContext.SaveChanges();
        }


        /// <summary>
        /// Obtiene las tablas CMSPagina, CMSBLoque,CMSBLoqueComponente, CMSComponente, CMSPropiedadComponente de una ubicacion en un proyecto
        /// </summary>
        /// <param name="pUbicacion">Ubicacion de la p�gina</param>
        /// <param name="pProyectoID">ID del proyecto</param>        
        /// <param name="pBorradorOPublicado">0-Todo,1-Borrador,2-Publicado</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerCMSDeUbicacionDeProyecto(short pUbicacion, Guid pProyectoID, short pBorradorOPublicado, bool pTraerSoloActivos)
        {
            DataWrapperCMS dataWrapperCMS = new DataWrapperCMS();

            //CMSPagina
            dataWrapperCMS.ListaCMSPagina = mEntityContext.CMSPagina.Where(item => item.ProyectoID.Equals(pProyectoID) && item.Ubicacion.Equals(pUbicacion)).ToList().Distinct().ToList();

            //CMSBloque
            var queryListaCMSBloque = mEntityContext.CMSBloque.Where(item => item.ProyectoID.Equals(pProyectoID) && item.Ubicacion.Equals(pUbicacion));
            if (pBorradorOPublicado == 1)
            {
                queryListaCMSBloque = queryListaCMSBloque.Where(item => item.Borrador.Equals(true));
            }
            else if (pBorradorOPublicado == 2)
            {
                queryListaCMSBloque = queryListaCMSBloque.Where(item => item.Borrador.Equals(false));
            }

            dataWrapperCMS.ListaCMSBloque = queryListaCMSBloque.ToList().Distinct().ToList();

            //CMSBloqueComponente
            var queryListaCMSBloqueComponente = mEntityContext.CMSBloqueComponente.JoinCMSBloque().JoinCMSComponente().Where(item => item.CMSBloqueComponente.ProyectoID.Equals(pProyectoID) && item.CMSBloque.Ubicacion.Equals(pUbicacion));

            if (pBorradorOPublicado == 1)
            {
                queryListaCMSBloqueComponente = queryListaCMSBloqueComponente.Where(item => item.CMSBloque.Borrador.Equals(true));
            }
            else if (pBorradorOPublicado == 2)
            {
                queryListaCMSBloqueComponente = queryListaCMSBloqueComponente.Where(item => item.CMSBloque.Borrador.Equals(false));
            }

            if (pTraerSoloActivos)
            {
                queryListaCMSBloqueComponente = queryListaCMSBloqueComponente.Where(item => item.CMSComponente.Activo.Equals(true));
            }
            dataWrapperCMS.ListaCMSBloqueComponente = queryListaCMSBloqueComponente.Select(item => item.CMSBloqueComponente).ToList().Distinct().ToList();

            //CMSComponente
            var queryCMSComponente = mEntityContext.CMSBloqueComponente.JoinCMSBloque().JoinCMSComponente().Where(item => item.CMSComponente.ProyectoID.Equals(pProyectoID) && item.CMSBloque.Ubicacion.Equals(pUbicacion));

            if (pBorradorOPublicado == 1)
            {
                queryCMSComponente = queryListaCMSBloqueComponente.Where(item => item.CMSBloque.Borrador);
            }
            else if (pBorradorOPublicado == 2)
            {
                queryCMSComponente = queryListaCMSBloqueComponente.Where(item => item.CMSBloque.Borrador.Equals(false));
            }

            if (pTraerSoloActivos)
            {
                queryCMSComponente = queryListaCMSBloqueComponente.Where(item => item.CMSComponente.Activo);
            }
            dataWrapperCMS.ListaCMSComponente = queryCMSComponente.Select(item => item.CMSComponente).ToList().Distinct().ToList();

            //CMSPropiedadComponente
            var queryCMSPropiedadComponente = mEntityContext.CMSBloqueComponente.JoinCMSBloque().JoinCMSComponente().JoinCMSPropiedadComponente().Where(item => item.CMSBloqueComponente.ProyectoID.Equals(pProyectoID) && item.CMSBloque.Ubicacion.Equals(pUbicacion));

            if (pBorradorOPublicado == 1)
            {
                queryCMSPropiedadComponente = queryCMSPropiedadComponente.Where(item => item.CMSBloque.Borrador.Equals(true));
            }
            else if (pBorradorOPublicado == 2)
            {
                queryCMSPropiedadComponente = queryCMSPropiedadComponente.Where(item => item.CMSBloque.Borrador.Equals(false));
            }
            dataWrapperCMS.ListaCMSPropiedadComponente = queryCMSPropiedadComponente.Select(item => item.CMSPropiedadComponente).ToList().Distinct().ToList();

            //CMSComponenteRolGrupoIdentidades
            var queryCMSComponenteRolGrupoIdentidades = mEntityContext.CMSBloqueComponente.JoinCMSBloque().JoinCMSComponente().JoinCMSComponenteRolGrupoIdentidades().Where(item => item.CMSBloqueComponente.ProyectoID.Equals(pProyectoID) && item.CMSBloque.Ubicacion.Equals(pUbicacion));

            if (pBorradorOPublicado == 1)
            {
                queryCMSComponenteRolGrupoIdentidades = queryCMSComponenteRolGrupoIdentidades.Where(item => item.CMSBloque.Borrador.Equals(true));
            }
            else if (pBorradorOPublicado == 2)
            {
                queryCMSComponenteRolGrupoIdentidades = queryCMSComponenteRolGrupoIdentidades.Where(item => item.CMSBloque.Borrador.Equals(false));
            }
            dataWrapperCMS.ListaCMSComponenteRolGrupoIdentidades = queryCMSComponenteRolGrupoIdentidades.Select(item => item.CMSComponenteRolGrupoIdentidades).ToList().Distinct().ToList();

            //CMSComponenteRolIdentidad
            var queryCMSComponenteRolIdentidad = mEntityContext.CMSBloqueComponente.JoinCMSBloque().JoinCMSComponente().JoinCMSComponenteRolIdentidad().Where(item => item.CMSBloqueComponente.ProyectoID.Equals(pProyectoID) && item.CMSBloque.Ubicacion.Equals(pUbicacion));

            if (pBorradorOPublicado == 1)
            {
                queryCMSComponenteRolIdentidad = queryCMSComponenteRolIdentidad.Where(item => item.CMSBloque.Borrador.Equals(true));
            }
            else if (pBorradorOPublicado == 2)
            {
                queryCMSComponenteRolIdentidad = queryCMSComponenteRolIdentidad.Where(item => item.CMSBloque.Borrador.Equals(false));
            }
            dataWrapperCMS.ListaCMSComponenteRolIdentidad = queryCMSComponenteRolIdentidad.Select(item => item.CMSComponenteRolIdentidad).ToList().Distinct().ToList();

            //CMSBloqueComponentePropiedadComponente
            var queyCMSBloqueComponentePropiedadComponente = mEntityContext.CMSBloqueComponentePropiedadComponente.JoinCMSBloque().Where(item => item.CMSBloqueComponentePropiedadComponente.ProyectoID.Equals(pProyectoID) && item.CMSBloque.Ubicacion.Equals(pUbicacion));

            if (pBorradorOPublicado == 1)
            {
                queyCMSBloqueComponentePropiedadComponente = queyCMSBloqueComponentePropiedadComponente.Where(item => item.CMSBloque.Borrador.Equals(true));
            }
            else if (pBorradorOPublicado == 2)
            {
                queyCMSBloqueComponentePropiedadComponente = queyCMSBloqueComponentePropiedadComponente.Where(item => item.CMSBloque.Borrador.Equals(false));
            }
            dataWrapperCMS.ListaCMSBloqueComponentePropiedadComponente = queyCMSBloqueComponentePropiedadComponente.Select(item => item.CMSBloqueComponentePropiedadComponente).ToList().Distinct().ToList();

            dataWrapperCMS = RecargarComponentesDeGruposDeComponentes(dataWrapperCMS, pProyectoID, pTraerSoloActivos);

            return dataWrapperCMS;

        }

        public DataWrapperCMS RecargarComponentesDeGruposDeComponentes(DataWrapperCMS pCMSDW, Guid pProyectoID, bool pTraerSoloActivos)
        {
            List<Guid> listaComponentesCargados = pCMSDW.ListaCMSComponente.Select(item => item.ComponenteID).ToList().Distinct().ToList();

            List<Guid> listaComponentesRecargar = new List<Guid>();
            foreach (CMSComponente filaC in pCMSDW.ListaCMSComponente)
            {
                if (filaC.TipoComponente == (short)TipoComponenteCMS.GrupoComponentes)
                {
                    foreach (CMSPropiedadComponente filaP in pCMSDW.ListaCMSPropiedadComponente)
                    {
                        if (filaP.TipoPropiedadComponente == (short)TipoPropiedadCMS.ListaIDs)
                        {
                            string[] listadoComponentesGrupo = filaP.ValorPropiedad.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string idComponente in listadoComponentesGrupo)
                            {
                                try
                                {
                                    Guid idNuevoComponente = new Guid(idComponente);
                                    if (!listaComponentesRecargar.Contains(idNuevoComponente) && !listaComponentesCargados.Contains(idNuevoComponente))
                                    {
                                        listaComponentesRecargar.Add(idNuevoComponente);
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
            }

            DataWrapperCMS nuevosCMSDR = ObtenerComponentePorListaID(listaComponentesRecargar, pProyectoID, pTraerSoloActivos);

            if (nuevosCMSDR.ListaCMSComponente.Count > 0)
            {
                pCMSDW.Merge(nuevosCMSDR);
                RecargarComponentesDeGruposDeComponentes(pCMSDW, pProyectoID, pTraerSoloActivos);
            }
            return pCMSDW;
        }

        /// <summary>
        /// Obtiene las tablas CMSComponente y CMSPropiedadComponente de una lista de componentes
        /// </summary>
        /// <param name="pComponenteID">lista de IDs de los componentes</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerComponentePorListaID(List<Guid> pListaComponenteID, Guid pProyectoID, bool pCargarSoloActivos)
        {
            DataWrapperCMS CMSDW = new DataWrapperCMS();

            if (pListaComponenteID.Count > 0)
            {
                var consultaCMSComponente = mEntityContext.CMSComponente.Where(item => pListaComponenteID.Contains(item.ComponenteID) && item.ProyectoID.Equals(pProyectoID));

                if (pCargarSoloActivos)
                {
                    consultaCMSComponente = consultaCMSComponente.Where(item => item.Activo);
                }

                CMSDW.ListaCMSComponente = consultaCMSComponente.ToList();
                CMSDW.ListaCMSPropiedadComponente = mEntityContext.CMSPropiedadComponente.Where(item => pListaComponenteID.Contains(item.ComponenteID)).ToList();
                CMSDW.ListaCMSComponenteRolGrupoIdentidades = mEntityContext.CMSComponenteRolGrupoIdentidades.Where(item => pListaComponenteID.Contains(item.ComponenteID)).ToList();
                CMSDW.ListaCMSComponenteRolIdentidad = mEntityContext.CMSComponenteRolIdentidad.Where(item => pListaComponenteID.Contains(item.ComponenteID)).ToList();
            }

            return CMSDW;
        }

        /// <summary>
        /// Obtiene la tabla CMSBloque
        /// </summary>
        /// <param name="pBloqueID">ID del bloque</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerBloquePorID(Guid pBloqueID)
        {
            DataWrapperCMS dataWrapperCMS = new DataWrapperCMS();

            dataWrapperCMS.ListaCMSBloque = mEntityContext.CMSBloque.Where(item => item.BloqueID.Equals(pBloqueID)).ToList();

            return dataWrapperCMS;
        }

        /// <summary>
        /// Nos indica las p�ginas a las que esta vinculado el componente
        /// </summary>
        /// <param name="pComponenteID"></param>
        /// <returns></returns>
        public List<string> PaginasVinculadasComponente(Guid pComponenteID, Guid pProyectoID)
        {
            return mEntityContext.CMSBloqueComponente.JoinCMSBloque().JoinCMSPagina().JoinProyectoPestanyaCMS().JoinProyectoPestanyaMenu().Where(item => item.CMSBloque.ProyectoID.Equals(pProyectoID) && item.CMSBloqueComponente.ComponenteID.Equals(pComponenteID)).Select(item => item.ProyectoPestanyaMenu.Nombre).ToList();
        }

        /// <summary>
        /// Obtiene las filas CMSComponente de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pTipoCaducidad">Tipo de caducidad</param>
        /// <returns>Lista de componentes</returns>
        public List<CMSComponente> ObtenerFilasComponentesCMSDeProyectoPorTipoCaducidad(Guid pProyectoID, TipoCaducidadComponenteCMS pTipoCaducidad)
        {
            DataWrapperCMS dataWrapperCMS = new DataWrapperCMS();

            dataWrapperCMS.ListaCMSComponente = mEntityContext.CMSComponente.Where(item => item.ProyectoID.Equals(pProyectoID) && item.TipoCaducidadComponente.Equals((short)pTipoCaducidad)).ToList();

            return dataWrapperCMS.ListaCMSComponente;
        }

        /// <summary>
        /// Obtiene las filas CMSComponente caducadas
        /// </summary>
        /// <returns>Lista de componentes</returns>
        public List<CMSComponente> ObtenerFilasComponentesCMSCaducados()
        {
            DataWrapperCMS dataWrapperCMS = new DataWrapperCMS();

            dataWrapperCMS.ListaCMSComponente = mEntityContext.CMSComponente.Where(item => (item.TipoCaducidadComponente.Equals((short)TipoCaducidadComponenteCMS.Hora) || item.TipoCaducidadComponente.Equals((short)TipoCaducidadComponenteCMS.Dia) || item.TipoCaducidadComponente.Equals((short)TipoCaducidadComponenteCMS.Semana)) && item.FechaUltimaActualizacion < DateTime.Now).ToList();

            return dataWrapperCMS.ListaCMSComponente;
        }

        /// <summary>
        /// Obtiene las tablas CMSComponente y CMSPropiedadComponente de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerComponentesCMSDeProyecto(Guid pProyectoID)
        {
            DataWrapperCMS CMSDW = new DataWrapperCMS();

            CMSDW.ListaCMSComponente = mEntityContext.CMSComponente.Where(item => item.ProyectoID.Equals(pProyectoID)).OrderBy(item => item.Nombre).ToList();
            CMSDW.ListaCMSPropiedadComponente = mEntityContext.CMSPropiedadComponente.JoinCMSComponente().Where(item => item.CMSComponente.ProyectoID.Equals(pProyectoID)).Select(item => item.CMSPropiedadComponente).ToList();
            CMSDW.ListaCMSComponenteRolGrupoIdentidades = mEntityContext.CMSComponenteRolGrupoIdentidades.JoinCMSComponente().Where(item => item.CMSComponente.ProyectoID.Equals(pProyectoID)).Select(item => item.CMSComponenteRolGrupoIdentidades).ToList();
            CMSDW.ListaCMSComponenteRolIdentidad = mEntityContext.CMSComponenteRolIdentidad.JoinCMSComponente().Where(item => item.CMSComponente.ProyectoID.Equals(pProyectoID)).Select(item => item.CMSComponenteRolIdentidad).ToList();

            return CMSDW;
        }

        /// <summary>
        /// Obtiene la tabla CMSComponente del proyecto indicado por par�metro
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pLimite">N�mero de resultados a devolver</param>
        /// <param name="pBusqueda">T�tulo del componente a buscar</param>
        /// <returns>Lista de CMSComponente</returns>
        public List<CMSComponente> ObtenerCMSComponentePorProyecto(Guid pProyectoID, int pLimite, string pBusqueda)
        {
            CorregirFechaActualizacionComponentesAntiguos();

            if (pLimite != -1)
            {
                return mEntityContext.CMSComponente.Where(item => item.ProyectoID.Equals(pProyectoID) && item.Nombre.ToLower().Contains(pBusqueda.ToLower())).OrderByDescending(item => item.FechaUltimaActualizacion.Value).Take(pLimite).ToList();
            }
            else
            {
                return mEntityContext.CMSComponente.Where(item => item.ProyectoID.Equals(pProyectoID) && item.Nombre.ToLower().Contains(pBusqueda.ToLower())).OrderByDescending(item => item.FechaUltimaActualizacion.Value).ToList();
            }
        }

        /// <summary>
        /// Obtiene las tablas CMSComponente y CMSPropiedadComponente de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pTexto">Texto que debe contener el nombre</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerComponentesCMSDeProyecto(Guid pProyectoID, string pTexto)
        {
            DataWrapperCMS dataWrapperCMS = new DataWrapperCMS();

            //CMSComponente
            var queryCMSComponente = mEntityContext.CMSComponente.Where(item => item.ProyectoID.Equals(pProyectoID));
            if (!string.IsNullOrEmpty(pTexto))
            {
                queryCMSComponente = queryCMSComponente.Where(item => item.Nombre.ToLower().Contains(pTexto) || item.NombreCortoComponente.ToLower().Contains(pTexto));
            }
            dataWrapperCMS.ListaCMSComponente = queryCMSComponente.OrderBy(item => item.Nombre).ToList();

            //CMSPropiedadComponente
            var queryCMSPropiedadComponente = mEntityContext.CMSComponente.JoinCMSPropiedadComponente().Where(item => item.CMSComponente.ProyectoID.Equals(pProyectoID));
            if (!string.IsNullOrEmpty(pTexto))
            {
                queryCMSPropiedadComponente = queryCMSPropiedadComponente.Where(item => item.CMSComponente.Nombre.ToLower().Contains(pTexto));
            }
            dataWrapperCMS.ListaCMSPropiedadComponente = queryCMSPropiedadComponente.Select(item => item.CMSPropiedadComponente).ToList();

            return dataWrapperCMS;
        }

        /// <summary>
        /// Obtiene las tablas CMSComponente y CMSPropiedadComponente... de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pTipoComponente">Tipo de componente</param>
        /// <param name="pTexto">Texto que debe contener el nombre</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerComponentesCMSDeProyectoDelTipoEspecificado(Guid pProyectoID, TipoComponenteCMS pTipoComponente, string pTexto)
        {
            DataWrapperCMS dataWrapperCMS = new DataWrapperCMS();

            //CMSComponente
            var queryCMSComponente = mEntityContext.CMSComponente.Where(item => item.ProyectoID.Equals(pProyectoID) && item.TipoComponente.Equals((short)pTipoComponente));
            if (!string.IsNullOrEmpty(pTexto))
            {
                queryCMSComponente = queryCMSComponente.Where(item => item.Nombre.ToLower().Contains(pTexto.ToLower()) || item.NombreCortoComponente.Equals(pTexto.ToLower()));
            }
            dataWrapperCMS.ListaCMSComponente = queryCMSComponente.OrderBy(item => item.Nombre).ToList();

            //CMSPropiedadComponente
            var queryCMSPropiedadComponente = mEntityContext.CMSComponente.JoinCMSPropiedadComponente().Where(item => item.CMSComponente
            .ProyectoID.Equals(pProyectoID) && item.CMSComponente.TipoComponente.Equals((short)pTipoComponente));
            if (!string.IsNullOrEmpty(pTexto))
            {
                queryCMSPropiedadComponente = queryCMSPropiedadComponente.Where(item => item.CMSComponente.Nombre.ToLower().Contains(pTexto.ToLower()) || item.CMSComponente.NombreCortoComponente.ToLower().Contains(pTexto.ToLower()));
            }
            dataWrapperCMS.ListaCMSPropiedadComponente = queryCMSPropiedadComponente.Select(item => item.CMSPropiedadComponente).ToList();

            return dataWrapperCMS;
        }

        public Guid? ObtenerIDComponentePorNombreEnProyecto(string pNombreComponente, Guid pProyectoID)
        {
            return mEntityContext.CMSComponente.Where(item => item.NombreCortoComponente.Equals(pNombreComponente) && item.ProyectoID.Equals(pProyectoID)).Select(item => item.ComponenteID).FirstOrDefault();
        }

        public string ObtenerNombreCortoComponentePorIDComponenteEnProyecto(Guid pComponenteID, Guid pProyectoID)
        {
            var query = mEntityContext.CMSComponente.Where(item => item.ComponenteID.Equals(pComponenteID) && item.ProyectoID.Equals(pProyectoID)).Select(item => item.NombreCortoComponente);
            return query.FirstOrDefault();
        }
        public Dictionary<Guid, string> ObtenerNombreComponentesPorIDComponente(List<Guid> pIdsComponentes)
        {
            var query = mEntityContext.CMSComponente.Where(item => pIdsComponentes.Contains(item.ComponenteID));
            return query.ToDictionary(k => k.ComponenteID, v => v.Nombre);
        }

        /// <summary>
        /// Elimina los bloques de una p�gina de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pTipoUbicacionCMS">Tipo de ubicacion</param>
        /// <param name="pSoloLosBorradores">Eliminar solo los borradores</param>
        public void EliminarBloquesDePaginaDeProyecto(Guid pProyectoID, short pTipoUbicacionCMS, bool pSoloLosBorradores)
        {
            //Revisar
            var querySubconsulta = mEntityContext.CMSBloque.Where(item => item.ProyectoID.Equals(pProyectoID) && item.Ubicacion.Equals(pTipoUbicacionCMS));

            if (pSoloLosBorradores)
            {
                querySubconsulta = querySubconsulta.Where(item => item.Borrador.Equals(true));
            }

            List<Guid> listaBloqueID = querySubconsulta.Select(item => item.BloqueID).ToList();
            List<CMSBloqueComponentePropiedadComponente> listaComponentesAEliminar = mEntityContext.CMSBloqueComponentePropiedadComponente.Where(item => listaBloqueID.Contains(item.BloqueID)).ToList();

            foreach (CMSBloqueComponentePropiedadComponente item in listaComponentesAEliminar)
            {
                mEntityContext.EliminarElemento(item);
            }

            List<CMSBloqueComponente> listaBloqueComponenteEliminar = mEntityContext.CMSBloqueComponente.Where(item => listaBloqueID.Contains(item.BloqueID)).ToList();

            foreach (CMSBloqueComponente item in listaBloqueComponenteEliminar)
            {
                mEntityContext.EliminarElemento(item);
            }

            List<CMSBloque> listaBloqueAEliminar = querySubconsulta.ToList();
            foreach (CMSBloque item in listaBloqueAEliminar)
            {
                mEntityContext.EliminarElemento(item);
            }

            ActualizarBaseDeDatos();
        }

        /// <summary>
        /// Actualiza la fecha de actualizaci�n de un componente
        /// </summary>
        /// <param name="pComponenteID">ID del componente</param>
        /// <param name="pFechaActualizacion">Fecha de la  �ltima actualizaci�n</param>
        public void ActualizarCaducidadComponente(Guid pComponenteID, DateTime pFechaActualizacion)
        {
            //Revisar
            CMSComponente cmsComponente = mEntityContext.CMSComponente.Where(item => item.ComponenteID.Equals(pComponenteID)).FirstOrDefault();

            if (cmsComponente != null)
            {
                cmsComponente.FechaUltimaActualizacion = pFechaActualizacion;
            }

            ActualizarBaseDeDatos();
        }

        public DataWrapperCMS ObtenerPaginasPerteneceComponentePorComponenteID(Guid pComponenteID, Guid pProyectoID)
        {
            DataWrapperCMS dataWrapperCMS = new DataWrapperCMS();

            dataWrapperCMS.ListaCMSPagina = mEntityContext.CMSPagina.JoinCMSBloque().JoinCMSBloqueComponente().Where(item => item.CMSBloqueComponente.ComponenteID.Equals(pComponenteID) && item.CMSPagina.ProyectoID.Equals(pProyectoID)).Select(item => item.CMSPagina).ToList().Distinct().ToList();

            return dataWrapperCMS;
        }

        public DataSet ObtenerConsultaComponenteSQLSERVER(string pQuery)
        {
            DataSet dataSet = new DataSet();
            DbCommand commandsqlSelectSQLSERVER = ObtenerComando(pQuery);
            CargarDataSet(commandsqlSelectSQLSERVER, dataSet, "Resultado", pEjecutarSiEsOracle: true, pEjecutarSiEsPostgres: true);
            return dataSet;
        }

        public DataWrapperCMS ObtenerCMSBloqueComponentePropiedadComponente(Guid pProyectoID, Guid pComponenteID)
        {
            //Revisar
            DataWrapperCMS dataWrapperCMS = new DataWrapperCMS();

            dataWrapperCMS.ListaCMSBloqueComponentePropiedadComponente = mEntityContext.CMSBloqueComponentePropiedadComponente.JoinCMSBloque().Where(item => item.CMSBloqueComponentePropiedadComponente.ProyectoID.Equals(pProyectoID) && item.CMSBloqueComponentePropiedadComponente.ComponenteID.Equals(pComponenteID)).Select(item => item.CMSBloqueComponentePropiedadComponente).ToList();

            return dataWrapperCMS;
        }

        public List<CMSComponenteVersion> ObtenerVersionesComponenteCMS(Guid pComponenteID)
        {
            return mEntityContext.CMSComponenteVersion.Where(item => item.ComponenteID.Equals(pComponenteID)).ToList();
        }

        public List<CMSComponenteVersion> ObtenerVersionesComponenteCMSSinModelo(Guid pComponenteID)
        {
            return mEntityContext.CMSComponenteVersion.Where(item => item.ComponenteID.Equals(pComponenteID)).Select(item => new CMSComponenteVersion
            {
                VersionID = item.VersionID,
                ComponenteID = item.ComponenteID,
                IdentidadID = item.IdentidadID,
                VersionAnterior = item.VersionAnterior,
                Fecha = item.Fecha,
                Comentario = item.Comentario
            }).ToList();
        }

        public CMSComponenteVersion ObtenerVersionComponenteCMS(Guid pComponenteID, Guid pVersionID)
        {
            return mEntityContext.CMSComponenteVersion.Where(item => item.ComponenteID.Equals(pComponenteID) && item.VersionID.Equals(pVersionID)).FirstOrDefault();
        }

        public List<ProyectoPestanyaVersionCMS> ObtenerVersionesEstructuraPaginaCMS(Guid pPestanyaID)
        {
            return mEntityContext.ProyectoPestanyaVersionCMS.Where(item => item.PestanyaID.Equals(pPestanyaID)).ToList();
        }

        /// <summary>
        /// Obtiene las versiones de la estructura de una pagina del CMS pero evitando cargar todo el modelo de la estructura.
        /// </summary>
        /// <param name="pPestanyaID">Id de la pagina a consultar</param>
        /// <returns></returns>
        public List<ProyectoPestanyaVersionCMS> ObtenerVersionesEstructuraPaginaCMSSinEstructura(Guid pPestanyaID)
        {
            return mEntityContext.ProyectoPestanyaVersionCMS.Where(item => item.PestanyaID.Equals(pPestanyaID)).Select(item => new ProyectoPestanyaVersionCMS()
            {
                PestanyaID = item.PestanyaID,
                VersionID = item.VersionID,
                Fecha = item.Fecha,
                IdentidadID = item.IdentidadID,
                Comentario = item.Comentario
            }).ToList();
        }

        public List<Guid> ObtenerIdVersionesEstructuraPaginaCMS(Guid pPestanyaID)
        {
            return mEntityContext.ProyectoPestanyaVersionCMS.Where(item => item.PestanyaID.Equals(pPestanyaID)).OrderBy(item => item.Fecha).Select(item => item.VersionID).ToList();
        }

        public ProyectoPestanyaVersionCMS ObtenerVersionEstructuraPaginaCMS(Guid pVersionID)
        {
            return mEntityContext.ProyectoPestanyaVersionCMS.Where(item => item.VersionID.Equals(pVersionID)).FirstOrDefault();
        }

        #endregion

        #region Privados

        /// <summary>
        /// En caso de que se utilice el GnossConfig.xml por defecto se sigue utilizando el IBD est�tico
        /// </summary>
        private void CargarConsultasYDataAdapters()
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como par�metro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {
            #region Consultas

            this.sqlSelectCMSPagina = "SELECT " + IBD.CargarGuid("CMSPagina.OrganizacionID") + ", " + IBD.CargarGuid("CMSPagina.ProyectoID") + ", CMSPagina.Activa, CMSPagina.Ubicacion, CMSPAgina.MostrarSoloCuerpo FROM CMSPagina";

            this.sqlSelectCMSBloque = "SELECT " + IBD.CargarGuid("CMSBloque.OrganizacionID") + ", " + IBD.CargarGuid("CMSBloque.ProyectoID") + ", CMSBloque.Ubicacion," + IBD.CargarGuid("CMSBloque.BloqueID") + ", " + IBD.CargarGuid("CMSBloque.BloquePadreID") + ", CMSBloque.Orden, CMSBloque.Estilos, CMSBloque.Borrador FROM CMSBloque";

            this.sqlSelectCMSBloqueComponente = "SELECT " + IBD.CargarGuid("CMSBloqueComponente.OrganizacionID") + ", " + IBD.CargarGuid("CMSBloqueComponente.ProyectoID") + ", " + IBD.CargarGuid("CMSBloqueComponente.BloqueID") + ", " + IBD.CargarGuid("CMSBloqueComponente.ComponenteID") + ", CMSBloqueComponente.Orden FROM CMSBloqueComponente";

            this.sqlSelectCMSComponente = "SELECT " + IBD.CargarGuid("CMSComponente.OrganizacionID") + ", " + IBD.CargarGuid("CMSComponente.ProyectoID") + ", " + IBD.CargarGuid("CMSComponente.ComponenteID") + ", CMSComponente.Nombre, CMSComponente.TipoComponente, CMSComponente.TipoCaducidadComponente, CMSComponente.FechaUltimaActualizacion, CMSComponente.Estilos, CMSComponente.Activo, CMSComponente.IdiomasDisponibles, CMSComponente.NombreCortoComponente, CMSComponente.AccesoPublico FROM CMSComponente";

            this.sqlSelectCMSPropiedadComponente = "SELECT " + IBD.CargarGuid("CMSPropiedadComponente.ComponenteID") + ", CMSPropiedadComponente.TipoPropiedadComponente, CMSPropiedadComponente.ValorPropiedad FROM CMSPropiedadComponente";

            this.sqlSelectCMSComponentePrivadoProyecto = "SELECT " + IBD.CargarGuid("CMSComponentePrivadoProyecto.OrganizacionID") + ", " + IBD.CargarGuid("CMSComponentePrivadoProyecto.ProyectoID") + ", CMSComponentePrivadoProyecto.TipoComponente FROM CMSComponentePrivadoProyecto";

            this.sqlSelectCMSBloqueComponentePropiedadComponente = "SELECT " + IBD.CargarGuid("CMSBloqueComponentePropiedadComponente.OrganizacionID") + ", " + IBD.CargarGuid("CMSBloqueComponentePropiedadComponente.ProyectoID") + "," + IBD.CargarGuid("CMSBloqueComponentePropiedadComponente.BloqueID") + ", " + IBD.CargarGuid("CMSBloqueComponentePropiedadComponente.ComponenteID") + ", CMSBloqueComponentePropiedadComponente.TipoPropiedadComponente, CMSBloqueComponentePropiedadComponente.ValorPropiedad FROM CMSBloqueComponentePropiedadComponente";

            this.sqlSelectCMSComponentePrivadoProyecto = "SELECT " + IBD.CargarGuid("CMSComponentePrivadoProyecto.OrganizacionID") + ", " + IBD.CargarGuid("CMSComponentePrivadoProyecto.ProyectoID") + ", CMSComponentePrivadoProyecto.TipoComponente FROM CMSComponentePrivadoProyecto";

            this.sqlSelectCMSComponenteRolGrupoIdentidades = "SELECT " + IBD.CargarGuid("CMSComponenteRolGrupoIdentidades.ComponenteID") + ", " + IBD.CargarGuid("CMSComponenteRolGrupoIdentidades.GrupoID") + " FROM CMSComponenteRolGrupoIdentidades";

            this.sqlSelectCMSComponenteRolIdentidad = "SELECT " + IBD.CargarGuid("CMSComponenteRolIdentidad.ComponenteID") + ", " + IBD.CargarGuid("CMSComponenteRolIdentidad.PerfilID") + " FROM CMSComponenteRolIdentidad";

            #endregion

            #region Consultas mas complejas

            this.sqlSelectCMSPaginaPorProyectoID = sqlSelectCMSPagina + " WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") ";
            this.sqlSelectCMSBloquePorProyectoID = sqlSelectCMSBloque + " WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") ";
            this.sqlSelectCMSBloqueComponentePorProyectoID = sqlSelectCMSBloqueComponente + " WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") ";
            this.sqlSelectCMSComponentePorProyectoID = sqlSelectCMSComponente + " WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") Order by Nombre ASC  ";
            this.sqlSelectCMSPropiedadComponentePorProyectoID = sqlSelectCMSPropiedadComponente + " INNER JOIN CMSComponente ON CMSComponente.ComponenteID=CMSPropiedadComponente.ComponenteID WHERE (CMSComponente.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") ";
            this.sqlSelectCMSComponentePrivadoProyectoPorProyectoID = sqlSelectCMSComponentePrivadoProyecto + " WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") ";
            this.sqlSelectCMSBloqueComponentePropiedadComponentePorProyectoID = sqlSelectCMSBloqueComponentePropiedadComponente + " WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") ";

            this.sqlSelectCMSComponenteRolGrupoIdentidadesPorProyectoID = sqlSelectCMSComponenteRolGrupoIdentidades + " INNER JOIN CMSComponente ON CMSComponente.ComponenteID=CMSComponenteRolGrupoIdentidades.ComponenteID WHERE (CMSComponente.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") ";

            this.sqlSelectCMSComponenteRolIdentidadPorProyectoID = sqlSelectCMSComponenteRolIdentidad + " INNER JOIN CMSComponente ON CMSComponente.ComponenteID=CMSComponenteRolIdentidad.ComponenteID WHERE (CMSComponente.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") ";

            this.sqlSelectCMSComponentePorProyectoIDYTipoComponente = sqlSelectCMSComponente + " WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND TipoComponente=@TipoComponente) Order by Nombre ASC ";
            this.sqlSelectCMSPropiedadComponentePorProyectoIDYTipoComponente = sqlSelectCMSPropiedadComponente.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN CMSComponente ON CMSComponente.ComponenteID=CMSPropiedadComponente.ComponenteID WHERE (CMSComponente.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND CMSComponente.TipoComponente=@TipoComponente) ";

            this.sqlSelectCMSPaginaPorUbicacionYProyectoID = sqlSelectCMSPagina.Replace("SELECT", "SELECT DISTINCT") + " WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") and Ubicacion=@Ubicacion ";
            this.sqlSelectCMSBloquePorUbicacionYProyectoID = sqlSelectCMSBloque.Replace("SELECT", "SELECT DISTINCT") + " WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") and Ubicacion=@Ubicacion Order by BloquePadreID, Orden ASC ";
            this.sqlSelectCMSBloqueComponentePorUbicacionYProyectoID = sqlSelectCMSBloqueComponente.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN CMSBloque on CMSBloque.BloqueID=CMSBloqueComponente.BloqueID INNER JOIN CMSComponente on CMSComponente.ComponenteID = CMSBloqueComponente.ComponenteID WHERE (CMSBloqueComponente.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") and CMSBloque.Ubicacion=@Ubicacion ";
            this.sqlSelectCMSBloqueComponentePropiedadComponentePorUbicacionYProyectoID = sqlSelectCMSBloqueComponentePropiedadComponente.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN CMSBloque on CMSBloque.BloqueID=CMSBloqueComponentePropiedadComponente.BloqueID WHERE (CMSBloqueComponentePropiedadComponente.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") and CMSBloque.Ubicacion=@Ubicacion ";
            this.sqlSelectCMSComponentePorUbicacionYProyectoID = sqlSelectCMSComponente.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN CMSBloqueComponente on CMSBloqueComponente.ComponenteID=CMSComponente.ComponenteID INNER JOIN CMSBloque on CMSBloque.BloqueID=CMSBloqueComponente.BloqueID  WHERE (CMSComponente.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") and CMSBloque.Ubicacion=@Ubicacion ";
            this.sqlSelectCMSPropiedadComponentePorUbicacionYProyectoID = sqlSelectCMSPropiedadComponente.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN CMSComponente ON CMSComponente.ComponenteID=CMSPropiedadComponente.ComponenteID INNER JOIN CMSBloqueComponente on CMSBloqueComponente.ComponenteID=CMSComponente.ComponenteID INNER JOIN CMSBloque on CMSBloque.BloqueID=CMSBloqueComponente.BloqueID  WHERE (CMSComponente.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") and CMSBloque.Ubicacion=@Ubicacion ";

            this.sqlSelectCMSComponenteRolGrupoIdentidadesPorUbicacionYProyectoID = sqlSelectCMSComponenteRolGrupoIdentidades.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN CMSComponente ON CMSComponente.ComponenteID=CMSComponenteRolGrupoIdentidades.ComponenteID INNER JOIN CMSBloqueComponente on CMSBloqueComponente.ComponenteID=CMSComponente.ComponenteID INNER JOIN CMSBloque on CMSBloque.BloqueID=CMSBloqueComponente.BloqueID  WHERE (CMSComponente.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") and CMSBloque.Ubicacion=@Ubicacion ";

            this.sqlSelectCMSComponenteRolIdentidadPorUbicacionYProyectoID = sqlSelectCMSComponenteRolIdentidad.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN CMSComponente ON CMSComponente.ComponenteID=CMSComponenteRolIdentidad.ComponenteID INNER JOIN CMSBloqueComponente on CMSBloqueComponente.ComponenteID=CMSComponente.ComponenteID INNER JOIN CMSBloque on CMSBloque.BloqueID=CMSBloqueComponente.BloqueID  WHERE (CMSComponente.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") and CMSBloque.Ubicacion=@Ubicacion ";

            this.sqlSelectCMSBloqueComponentePropiedadComponentePorProyectoIDYComponenteID = sqlSelectCMSBloqueComponentePropiedadComponente + " WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") AND (ComponenteID = " + IBD.GuidParamValor("componenteID") + ") ";

            #endregion

            #region DataAdapters

            #region CMSPagina

            this.sqlCMSPaginaInsert = IBD.ReplaceParam("INSERT INTO CMSPagina (OrganizacionID, ProyectoID, Activa, Ubicacion, MostrarSoloCuerpo) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @Activa, @Ubicacion, @MostrarSoloCuerpo)");

            this.sqlCMSPaginaDelete = IBD.ReplaceParam("DELETE FROM CMSPagina WHERE  (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " AND ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + " AND Ubicacion = @O_Ubicacion)");

            this.sqlCMSPaginaModify = IBD.ReplaceParam("UPDATE CMSPagina SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Activa = @Activa, Ubicacion = @Ubicacion, MostrarSoloCuerpo=@MostrarSoloCuerpo WHERE(OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " AND ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + " AND Ubicacion = @O_Ubicacion)");

            #endregion

            #region CMSBloque

            this.sqlCMSBloqueInsert = IBD.ReplaceParam("INSERT INTO CMSBloque (OrganizacionID, ProyectoID,Ubicacion, BloqueID, BloquePadreID, Orden, Estilos, Borrador) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ",@Ubicacion, " + IBD.GuidParamColumnaTabla("BloqueID") + ", " + IBD.GuidParamColumnaTabla("BloquePadreID") + ", @Orden, @Estilos, @Borrador)");

            this.sqlCMSBloqueDelete = IBD.ReplaceParam("DELETE FROM CMSBloque WHERE  (BloqueID = " + IBD.GuidParamColumnaTabla("O_BloqueID") + ")");

            this.sqlCMSBloqueModify = IBD.ReplaceParam("UPDATE CMSBloque SET  OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Ubicacion = @Ubicacion, BloqueID = " + IBD.GuidParamColumnaTabla("BloqueID") + ", BloquePadreID = " + IBD.GuidParamColumnaTabla("BloquePadreID") + ", Orden = @Orden, Estilos=@Estilos, Borrador=@Borrador WHERE (BloqueID = " + IBD.GuidParamColumnaTabla("O_BloqueID") + ")");

            #endregion

            #region CMSBloqueComponente

            this.sqlCMSBloqueComponenteInsert = IBD.ReplaceParam("INSERT INTO CMSBloqueComponente (OrganizacionID, ProyectoID, BloqueID, ComponenteID, Orden) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("BloqueID") + ", " + IBD.GuidParamColumnaTabla("ComponenteID") + ", @Orden)");

            this.sqlCMSBloqueComponenteDelete = IBD.ReplaceParam("DELETE FROM CMSBloqueComponente WHERE  (BloqueID = " + IBD.GuidParamColumnaTabla("O_BloqueID") + " AND ComponenteID = " + IBD.GuidParamColumnaTabla("O_ComponenteID") + ")");

            this.sqlCMSBloqueComponenteModify = IBD.ReplaceParam("UPDATE CMSBloqueComponente SET  OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", BloqueID = " + IBD.GuidParamColumnaTabla("BloqueID") + ", ComponenteID = " + IBD.GuidParamColumnaTabla("ComponenteID") + ", Orden = @Orden WHERE  (BloqueID = " + IBD.GuidParamColumnaTabla("O_BloqueID") + " AND ComponenteID = " + IBD.GuidParamColumnaTabla("O_ComponenteID") + ")");

            #endregion

            #region CMSComponente

            this.sqlCMSComponenteInsert = IBD.ReplaceParam("INSERT INTO CMSComponente (OrganizacionID, ProyectoID,ComponenteID,Nombre,TipoCaducidadComponente, TipoComponente,FechaUltimaActualizacion,Estilos,Activo,IdiomasDisponibles, NombreCortoComponente,AccesoPublico) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + "," + IBD.GuidParamColumnaTabla("ComponenteID") + ", @Nombre,@TipoCaducidadComponente, @TipoComponente, @FechaUltimaActualizacion, @Estilos, @Activo, @IdiomasDisponibles, @NombreCortoComponente,@AccesoPublico)");

            this.sqlCMSComponenteDelete = IBD.ReplaceParam("DELETE FROM CMSComponente WHERE  (ComponenteID = " + IBD.GuidParamColumnaTabla("O_ComponenteID") + ")");

            this.sqlCMSComponenteModify = IBD.ReplaceParam("UPDATE CMSComponente SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", ComponenteID = " + IBD.GuidParamColumnaTabla("ComponenteID") + ",Nombre=@Nombre, TipoCaducidadComponente = @TipoCaducidadComponente, TipoComponente = @TipoComponente,FechaUltimaActualizacion=@FechaUltimaActualizacion,Estilos=@Estilos,Activo=@Activo,IdiomasDisponibles=@IdiomasDisponibles, NombreCortoComponente=@NombreCortoComponente,AccesoPublico=@AccesoPublico WHERE (ComponenteID = " + IBD.GuidParamColumnaTabla("O_ComponenteID") + ")");

            #endregion

            #region CMSComponentePropiedad

            this.sqlCMSPropiedadComponenteInsert = IBD.ReplaceParam("INSERT INTO CMSPropiedadComponente (ComponenteID,TipopropiedadComponente, ValorPropiedad) VALUES (" + IBD.GuidParamColumnaTabla("ComponenteID") + ",@TipopropiedadComponente, @ValorPropiedad)");

            this.sqlCMSPropiedadComponenteDelete = IBD.ReplaceParam("DELETE FROM CMSPropiedadComponente WHERE  (ComponenteID = " + IBD.GuidParamColumnaTabla("O_ComponenteID") + " AND TipoPropiedadComponente=@O_TipoPropiedadComponente)");

            this.sqlCMSPropiedadComponenteModify = IBD.ReplaceParam("UPDATE CMSPropiedadComponente SET ComponenteID = " + IBD.GuidParamColumnaTabla("ComponenteID") + ", TipopropiedadComponente = @TipopropiedadComponente, ValorPropiedad = @ValorPropiedad WHERE (ComponenteID = " + IBD.GuidParamColumnaTabla("O_ComponenteID") + " AND TipoPropiedadComponente=@O_TipoPropiedadComponente)");

            #endregion

            #region CMSComponentePrivadoProyecto

            this.sqlCMSComponentePrivadoProyectoInsert = IBD.ReplaceParam("INSERT INTO CMSComponentePrivadoProyecto (OrganizacionID, ProyectoID, TipoComponente) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @TipoComponente)");

            this.sqlCMSComponentePrivadoProyectoDelete = IBD.ReplaceParam("DELETE FROM CMSComponentePrivadoProyecto WHERE  (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " AND ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + " AND TipoComponente = @O_TipoComponente )");

            this.sqlCMSComponentePrivadoProyectoModify = IBD.ReplaceParam("UPDATE CMSComponentePrivadoProyecto SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", TipoComponente = @TipoComponente WHERE(OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " AND ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + " AND TipoComponente = @O_TipoComponente )");

            #endregion

            #region CMSBloqueComponentePropiedadComponente

            this.sqlCMSBloqueComponentePropiedadComponenteInsert = IBD.ReplaceParam("INSERT INTO CMSBloqueComponentePropiedadComponente (OrganizacionID, ProyectoID, BloqueID, ComponenteID, TipoPropiedadComponente, ValorPropiedad) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("BloqueID") + ", " + IBD.GuidParamColumnaTabla("ComponenteID") + ", @TipoPropiedadComponente, @ValorPropiedad)");

            this.sqlCMSBloqueComponentePropiedadComponenteDelete = IBD.ReplaceParam("DELETE FROM CMSBloqueComponentePropiedadComponente WHERE  (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " AND ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + " AND BloqueID = " + IBD.GuidParamColumnaTabla("O_BloqueID") + " AND ComponenteID = " + IBD.GuidParamColumnaTabla("O_ComponenteID") + " AND TipoPropiedadComponente = @O_TipoPropiedadComponente AND ValorPropiedad = @O_ValorPropiedad )");

            this.sqlCMSBloqueComponentePropiedadComponenteModify = IBD.ReplaceParam("UPDATE CMSBloqueComponentePropiedadComponente SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ",BloqueID = " + IBD.GuidParamColumnaTabla("BloqueID") + ", ComponenteID = " + IBD.GuidParamColumnaTabla("ComponenteID") + ", TipoPropiedadComponente = @TipoPropiedadComponente, ValorPropiedad = @ValorPropiedad WHERE(OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " AND ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + " AND BloqueID = " + IBD.GuidParamColumnaTabla("O_BloqueID") + " AND ComponenteID = " + IBD.GuidParamColumnaTabla("O_ComponenteID") + " AND TipoPropiedadComponente = @O_TipoPropiedadComponente AND ValorPropiedad = @O_ValorPropiedad )");

            #endregion

            #region CMSComponenteRolIdentidad

            this.sqlCMSComponenteRolIdentidadInsert = IBD.ReplaceParam("INSERT INTO CMSComponenteRolIdentidad (ComponenteID, PerfilID) VALUES (" + IBD.GuidParamColumnaTabla("ComponenteID") + ", " + IBD.GuidParamColumnaTabla("PerfilID") + ")");

            this.sqlCMSComponenteRolIdentidadDelete = IBD.ReplaceParam("DELETE FROM CMSComponenteRolIdentidad WHERE  (ComponenteID = " + IBD.GuidParamColumnaTabla("O_ComponenteID") + " AND PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + ")");

            this.sqlCMSComponenteRolIdentidadModify = IBD.ReplaceParam("UPDATE CMSComponenteRolIdentidad SET ComponenteID = " + IBD.GuidParamColumnaTabla("ComponenteID") + ", PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + " WHERE(ComponenteID = " + IBD.GuidParamColumnaTabla("O_ComponenteID") + " AND PerfilID = " + IBD.GuidParamColumnaTabla("O_PerfilID") + " )");

            #endregion

            #region CMSComponenteRolGrupoIdentidades

            this.sqlCMSComponenteRolGrupoIdentidadesInsert = IBD.ReplaceParam("INSERT INTO CMSComponenteRolGrupoIdentidades (ComponenteID, GrupoID) VALUES (" + IBD.GuidParamColumnaTabla("ComponenteID") + ", " + IBD.GuidParamColumnaTabla("GrupoID") + ")");

            this.sqlCMSComponenteRolGrupoIdentidadesDelete = IBD.ReplaceParam("DELETE FROM CMSComponenteRolGrupoIdentidades WHERE  (ComponenteID = " + IBD.GuidParamColumnaTabla("O_ComponenteID") + " AND GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ")");

            this.sqlCMSComponenteRolGrupoIdentidadesModify = IBD.ReplaceParam("UPDATE CMSComponenteRolGrupoIdentidades SET ComponenteID = " + IBD.GuidParamColumnaTabla("ComponenteID") + ", GrupoID = " + IBD.GuidParamColumnaTabla("GrupoID") + " WHERE(ComponenteID = " + IBD.GuidParamColumnaTabla("O_ComponenteID") + " AND GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + " )");

            #endregion

            #endregion
        }

        /// <summary>
        /// A partir de la versi�n 5.6.0 es necesario que los CMSComponentes tenga FechaActualizacion, este campo antes no era necesario as� que actualizamos los componentes antiguos
        /// para que tambi�n tengan, si no fallar�n las instrucciones siguientes.
        /// </summary>
        private void CorregirFechaActualizacionComponentesAntiguos()
        {
            if (mEntityContext.CMSComponente.Any(item => !item.FechaUltimaActualizacion.HasValue))
            {
                List<CMSComponente> listaComponentesSinFecha = mEntityContext.CMSComponente.Where(item => !item.FechaUltimaActualizacion.HasValue).ToList();
                foreach (CMSComponente cmsComponenteSinFecha in listaComponentesSinFecha)
                {
                    cmsComponenteSinFecha.FechaUltimaActualizacion = DateTime.Now;
                }

                mEntityContext.SaveChanges();
            }
        }

        #endregion

        #endregion
    }
}
