using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.AD.ParametrosProyecto
{

    #region Enumeraciones

    public enum DatosExtraVistas
    {
        Recursos = 0,
        DatosExtraRecursos = 1,
        Identidades = 2,
        DatosExtraIdentidades = 3
    }

    #endregion


    /// <summary>
    /// DataAdapter para categorías de documentación
    /// </summary>
    /// 
    public class JoinVistaVirtualPersonalizacionVistaVirtualProyecto
    {
        public VistaVirtualPersonalizacion VistaVirtualPersonalizacion { get; set; }
        public VistaVirtualProyecto VistaVirtualProyecto { get; set; }
    }

    public class JoinVistaVirtualRecursosVistaVirtualProyecto
    {
        public VistaVirtualRecursos VistaVirtualRecursos { get; set; }
        public VistaVirtualProyecto VistaVirtualProyecto { get; set; }
    }

    public class JoinVistaVirtualVistaVirtualProyecto
    {
        public VistaVirtual VistaVirtual { get; set; }
        public VistaVirtualProyecto VistaVirtualProyecto { get; set; }
    }

    public class JoinProyectoVistaVirtualProyecto
    {
        public Proyecto Proyecto { get; set; }
        public VistaVirtualProyecto VistaVirtualProyecto { get; set; }
    }

    public class JoinVistaVirtualCMSVistaVirtualProyecto
    {
        public VistaVirtualCMS VistaVirtualCMS { get; set; }
        public VistaVirtualProyecto VistaVirtualProyecto { get; set; }
    }

    public class JoinVistaVirtualGadgetRecursosVistaVirtualProyecto
    {
        public VistaVirtualGadgetRecursos VistaVirtualGadgetRecursos { get; set; }
        public VistaVirtualProyecto VistaVirtualProyecto { get; set; }
    }

    //INNER JOIN VistaVirtualProyecto ON  VistaVirtualGadgetRecursos.PersonalizacionID = VistaVirtualProyecto.PersonalizacionID 
    public static partial class Joins
    {
        public static IQueryable<JoinVistaVirtualGadgetRecursosVistaVirtualProyecto> JoinVistaVirtualProyecto(this IQueryable<VistaVirtualGadgetRecursos> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.VistaVirtualProyecto, vistaVirtualGadgetRecursos => vistaVirtualGadgetRecursos.PersonalizacionID, vistaVirtualProyecto => vistaVirtualProyecto.PersonalizacionID, (vistaVirtualGadgetRecursos, vistaVirtualProyecto) => new JoinVistaVirtualGadgetRecursosVistaVirtualProyecto
            {
                VistaVirtualProyecto = vistaVirtualProyecto,
                VistaVirtualGadgetRecursos = vistaVirtualGadgetRecursos
            });
        }

        public static IQueryable<JoinVistaVirtualCMSVistaVirtualProyecto> JoinVistaVirtualProyecto(this IQueryable<VistaVirtualCMS> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.VistaVirtualProyecto, vistaVirtualCMS => vistaVirtualCMS.PersonalizacionID, vistaVirtualProyecto => vistaVirtualProyecto.PersonalizacionID, (vistaVirtualCMS, vistaVirtualProyecto) => new JoinVistaVirtualCMSVistaVirtualProyecto()
            {
                VistaVirtualProyecto = vistaVirtualProyecto,
                VistaVirtualCMS = vistaVirtualCMS
            });
        }

        public static IQueryable<JoinVistaVirtualPersonalizacionVistaVirtualProyecto> JoinVistaVirtualProyecto(this IQueryable<VistaVirtualPersonalizacion> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.VistaVirtualProyecto, personalizacion => personalizacion.PersonalizacionID, proyecto => proyecto.PersonalizacionID, (personalizacion, proyecto) =>
            new JoinVistaVirtualPersonalizacionVistaVirtualProyecto
            {
                VistaVirtualPersonalizacion = personalizacion,
                VistaVirtualProyecto = proyecto
            });
        }

        public static IQueryable<JoinVistaVirtualRecursosVistaVirtualProyecto> JoinVistaVirtualProyecto(this IQueryable<VistaVirtualRecursos> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.VistaVirtualProyecto, recursos => recursos.PersonalizacionID, proyecto => proyecto.PersonalizacionID, (recursos, proyecto) =>
            new JoinVistaVirtualRecursosVistaVirtualProyecto
            {
                VistaVirtualRecursos = recursos,
                VistaVirtualProyecto = proyecto
            });
        }

        public static IQueryable<JoinVistaVirtualVistaVirtualProyecto> JoinVistaVirtualProyecto(this IQueryable<VistaVirtual> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.VistaVirtualProyecto, vista => vista.PersonalizacionID, proyecto => proyecto.PersonalizacionID, (vista, proyecto) =>
            new JoinVistaVirtualVistaVirtualProyecto
            {
                VistaVirtual = vista,
                VistaVirtualProyecto = proyecto
            });
        }
        public static IQueryable<JoinProyectoVistaVirtualProyecto> JoinVistaVirtualProyecto(this IQueryable<Proyecto> pIQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pIQuery);
            return pIQuery.Join(entityContext.VistaVirtualProyecto, proy => proy.ProyectoID, proyecto => proyecto.ProyectoID, (proy, proyecto) =>
            new JoinProyectoVistaVirtualProyecto
            {
                Proyecto = proy,
                VistaVirtualProyecto = proyecto
            });
        }

    }
    public class VistaVirtualAD : BaseAD
    {
        #region Variables Estaticas

        public const string TipoVistaVirtual = "VistaVirtual";
        public const string TipoVistaVirtualCMS = "VistaVirtualCMS";
        public const string TipoVistaVirtualRdfType = "VistaVirtualRdfType";
        public const string TipoVistaVirtualGadget = "VistaVirtualGadget";

        #endregion

        #region Consultas

        string selectVistaVirtual;
        string selectVistaVirtualProyecto;
        string selectVistaVirtualRecursos;
        string selectVistaVirtualCMS;
        string selectVistaVirtualGadgetRecursos;

        #endregion

        #region DataAdapter

        string sqlVistaVirtualPersonalizacionInsert;
        string sqlVistaVirtualPersonalizacionDelete;

        string sqlVistaVirtualInsert;
        string sqlVistaVirtualDelete;
        string sqlVistaVirtualModify;

        string sqlVistaVirtualProyectoInsert;
        string sqlVistaVirtualProyectoDelete;
        string sqlVistaVirtualProyectoModify;

        string sqlVistaVirtualRecursosInsert;
        string sqlVistaVirtualRecursosDelete;
        string sqlVistaVirtualRecursosModify;

        string sqlVistaVirtualCMSInsert;
        string sqlVistaVirtualCMSDelete;
        string sqlVistaVirtualCMSModify;

        string sqlVistaVirtualGadgetRecursosInsert;
        string sqlVistaVirtualGadgetRecursosDelete;
        string sqlVistaVirtualGadgetRecursosModify;

        public object ResourceManager { get; set; }

        #endregion

        #region Constructor

        private EntityContext mEntityContext;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        /// <summary>
        /// Constructor
        /// </summary>
        public VistaVirtualAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<VistaVirtualAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public VistaVirtualAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<VistaVirtualAD> logger, ILoggerFactory loggerFactory)
            : base("vistas", loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Métodos Generales

        #region Públicos


        /// <summary>
        /// Comprueba si existe una fila en la tabla VistaVirtualPersonalizacion con un id concreto
        /// </summary>
        /// <param name="pPersonalizacionID">Identificador de la personalización que se quiere comprobar</param>
        /// <returns>True si existe la personalización</returns>
        public bool ComprobarExistePersonalizacionID(Guid pPersonalizacionID)
        {
            return mEntityContext.VistaVirtualPersonalizacion.Any(item => item.PersonalizacionID.Equals(pPersonalizacionID));
        }

        /// <summary>
        /// Obtiene el Guid de personalizacion que le corresponde a un proyecto
        /// </summary>
        /// <param name="pProyecto">Guid del proyecto</param>
        /// <returns>Guid de la personalicacion</returns>
        public Guid ObtenerPersonalicacionIdDadoProyectoID(Guid pProyecto)
        {
            object resultado = mEntityContext.VistaVirtualProyecto.Where(item => item.ProyectoID.Equals(pProyecto)).Select(item => item.PersonalizacionID).FirstOrDefault();
            return (Guid)resultado;
        }

        public Guid ObtenerPersonalizacionDominio(string pDominio)
        {
			Guid personalizacionID = mEntityContext.VistaVirtualDominio.Where(item => item.Dominio.Equals(pDominio)).Select(item => item.PersonalizacionID).FirstOrDefault();
			return personalizacionID;

		}

        public bool ComprobarPersonalizacionCompartidaEnDominio(string pDominio, Guid pProyectoID)
        {
            Guid personalizacionProyecto = ObtenerPersonalicacionIdDadoProyectoID(pProyectoID);
            Guid personalizacionDominio = ObtenerPersonalizacionDominio(pDominio);

            return personalizacionDominio.Equals(personalizacionProyecto);
        }

        public void CompartirPersonalizacionEnDominio(string pDominio, Guid pProyectoID)
        {
            Guid personalizacionProyecto = ObtenerPersonalicacionIdDadoProyectoID(pProyectoID);
			Guid personalizacionDominio = ObtenerPersonalizacionDominio(pDominio);

			if (!personalizacionDominio.Equals(Guid.Empty))
			{
				DejarDeCompartirPersonalizacion(pDominio);
			}

			VistaVirtualDominio vistaVirtualDominio = new VistaVirtualDominio();
			vistaVirtualDominio.PersonalizacionID = personalizacionProyecto;
			vistaVirtualDominio.Dominio = pDominio;

			mEntityContext.VistaVirtualDominio.Add(vistaVirtualDominio);

			mEntityContext.SaveChanges();
		}

        public void DejarDeCompartirPersonalizacion(string pDominio)
        {
            Guid personalizacionDominio = ObtenerPersonalizacionDominio(pDominio);

			if (!personalizacionDominio.Equals(Guid.Empty))
			{
				VistaVirtualDominio vistaVirtualDominio = mEntityContext.VistaVirtualDominio.Where(item => item.Dominio.Equals(pDominio)).FirstOrDefault();

				mEntityContext.VistaVirtualDominio.Remove(vistaVirtualDominio);

				mEntityContext.SaveChanges();
			}
		}

        public void DejarDeCompartirPersonalizacionEnDominio(string pDominio, Guid pProyecto)
        {
            Guid personalizacion = ObtenerPersonalicacionIdDadoProyectoID(pProyecto);
            Guid personalizacionDominio = ObtenerPersonalizacionDominio(pDominio);

            if (!personalizacionDominio.Equals(Guid.Empty))
            {
                VistaVirtualDominio vistaVirtualDominio = mEntityContext.VistaVirtualDominio.Where(item => item.PersonalizacionID.Equals(personalizacion) && item.Dominio.Equals(pDominio)).FirstOrDefault();

                mEntityContext.VistaVirtualDominio.Remove(vistaVirtualDominio);
                
                mEntityContext.SaveChanges();
            }
        }

        public List<string> ObtenerDominiosEstaCompartidaPersonalizacion(Guid pProyecto)
        {
            Guid personalizacion = ObtenerPersonalicacionIdDadoProyectoID(pProyecto);

            return mEntityContext.VistaVirtualDominio.Where(item => item.PersonalizacionID.Equals(personalizacion)).Select(item => item.Dominio).ToList();
        }

		/// <summary>
		/// Obtiene la personalización.
		/// </summary>
		/// <param name="pPersonalizacionID">Id de la personalizacion</param>
		/// <param name="pNombreVista">Nombre de la vista</param>
		/// <returns>Guid de la personalizacion de la vista</returns>
		public Guid ObtenerPersonalizacionComponenteCMSdeProyecto(Guid? pPersonalizacionID, string pNombreVista, string pRutaTipoComponente)
        {
            object devuelta = mEntityContext.VistaVirtualCMS.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID.Value) && item.Nombre.Equals(pNombreVista) && item.TipoComponente.Equals(pRutaTipoComponente)).Select(x => x.PersonalizacionComponenteID).FirstOrDefault();

            if (devuelta != null)
            {
                return (Guid)devuelta;
            }
            else
            {
                return Guid.Empty;
            }
        }


        /// <summary>
        /// Obtiene una lista con los proyectos y su personalizacion
        /// </summary>
        /// <returns>Lista con los proyectos y su personalizacion</returns>
        public Dictionary<Guid, KeyValuePair<string, Guid>> ObtenerProyectosConVistas()
        {
            Dictionary<Guid, KeyValuePair<string, Guid>> listaProyectosConVistas = new Dictionary<Guid, KeyValuePair<string, Guid>>();
            var resultado = mEntityContext.Proyecto.JoinVistaVirtualProyecto().Select(item => new { item.Proyecto.ProyectoID, item.Proyecto.Nombre, item.VistaVirtualProyecto.PersonalizacionID });

            foreach (var fila in resultado)
            {
                if (!listaProyectosConVistas.ContainsKey(fila.ProyectoID))
                {
                    listaProyectosConVistas.Add(fila.ProyectoID, new KeyValuePair<string, Guid>(fila.Nombre, fila.PersonalizacionID));
                }
            }

            return listaProyectosConVistas;
        }

        /// <summary>
        /// Obtiene las tablas de VistaVirtual de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>DataSet de VistaVirtual de un proyecto</returns>
        public DataWrapperVistaVirtual ObtenerVistasVirtualPorProyectoID(Guid pProyectoID)
        {
            DataWrapperVistaVirtual dataWrapperVistaVirtual = new DataWrapperVistaVirtual();

            dataWrapperVistaVirtual.ListaVistaVirtualPersonalizacion = mEntityContext.VistaVirtualPersonalizacion.JoinVistaVirtualProyecto().Where(item => item.VistaVirtualProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.VistaVirtualPersonalizacion).ToList();

            dataWrapperVistaVirtual.ListaVistaVirtualProyecto = mEntityContext.VistaVirtualProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperVistaVirtual.ListaVistaVirtualRecursos = mEntityContext.VistaVirtualRecursos.JoinVistaVirtualProyecto().Where(item => item.VistaVirtualProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.VistaVirtualRecursos).ToList();

            dataWrapperVistaVirtual.ListaVistaVirtual = mEntityContext.VistaVirtual.JoinVistaVirtualProyecto().Where(item => item.VistaVirtualProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.VistaVirtual).ToList();

            dataWrapperVistaVirtual.ListaVistaVirtualCMS = mEntityContext.VistaVirtualCMS.JoinVistaVirtualProyecto().Where(item => item.VistaVirtualProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.VistaVirtualCMS).ToList();

            dataWrapperVistaVirtual.ListaVistaVirtualGadgetRecursos = mEntityContext.VistaVirtualGadgetRecursos.JoinVistaVirtualProyecto().Where(item => item.VistaVirtualProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.VistaVirtualGadgetRecursos).ToList();

            return dataWrapperVistaVirtual;
        }

        /// <summary>
        /// Obtiene las tablas de VistaVirtual de una personaalizacion
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>DataSet de VistaVirtual de un proyecto</returns>
        public DataWrapperVistaVirtual ObtenerVistasVirtualPorEcosistemaID(Guid pPersonalizacionEcosistemaID)
        {
            return ObtenerVistasVirtualPorPersonalizacionID(pPersonalizacionEcosistemaID);
        }

        /// <summary>
        /// Obtiene las tablas de VistaVirtual de una personaalizacion
        /// </summary>
        /// <param name="pPersonalizacionID">Identificador de la personalizacion</param>
        /// <returns>DataSet de VistaVirtual de una personalizacion</returns>
        public DataWrapperVistaVirtual ObtenerVistasVirtualPorPersonalizacionID(Guid pPersonalizacionID)
        {
            DataWrapperVistaVirtual dataWrapperVistaVirtual = new DataWrapperVistaVirtual();

            dataWrapperVistaVirtual.ListaVistaVirtualPersonalizacion = mEntityContext.VistaVirtualPersonalizacion.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID)).ToList();

            dataWrapperVistaVirtual.ListaVistaVirtualProyecto = mEntityContext.VistaVirtualProyecto.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID)).ToList();

            dataWrapperVistaVirtual.ListaVistaVirtual = mEntityContext.VistaVirtual.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID)).ToList();

            dataWrapperVistaVirtual.ListaVistaVirtualDominio = mEntityContext.VistaVirtualDominio.Where((item) => item.PersonalizacionID.Equals(pPersonalizacionID)).ToList();

            dataWrapperVistaVirtual.ListaVistaVirtualRecursos = mEntityContext.VistaVirtualRecursos.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID)).ToList();

            dataWrapperVistaVirtual.ListaVistaVirtualCMS = mEntityContext.VistaVirtualCMS.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID)).ToList();

            dataWrapperVistaVirtual.ListaVistaVirtualGadgetRecursos = mEntityContext.VistaVirtualGadgetRecursos.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID)).ToList();

            return dataWrapperVistaVirtual;
        }


        /// <summary>
        /// Obtiene el html para una vista de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto a actualizar su vista</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pVista">Vista que se va a personalizar</param>
        /// <param name="pEsVistaRdfType">Verdad si la vista es de un formulario semántico, falso si es una vista común</param>
        public string ObtenerHtmlParaVistaDeProyecto(Guid pOrganizacionID, Guid pProyectoID, string pVista, bool pEsVistaRdfType)
        {
            var resultado = mEntityContext.VistaVirtual.JoinVistaVirtualProyecto().Where(item => item.VistaVirtual.TipoPagina.Equals(pVista) && item.VistaVirtualProyecto.ProyectoID.Equals(pProyectoID) && item.VistaVirtualProyecto.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.VistaVirtual.HTML).FirstOrDefault();

            if (pEsVistaRdfType)
            {
                resultado = mEntityContext.VistaVirtualRecursos.JoinVistaVirtualProyecto().Where(item => item.VistaVirtualRecursos.RdfType.Equals(pVista) && item.VistaVirtualProyecto.ProyectoID.Equals(pProyectoID) && item.VistaVirtualProyecto.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.VistaVirtualRecursos.HTML).FirstOrDefault();
            }

            return resultado;
        }
        /// <summary>
        /// Obtiene el html para una vista de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto a actualizar su vista</param>
        /// <param name="PersonalizacionID">PersonalizacionID del proyecto</param>
        /// <param name="pVista">Vista que se va a personalizar</param>
        /// <param name="pEsVistaRdfType">Verdad si la vista es de un formulario semántico, falso si es una vista común</param>
        public string ObtenerHtmlParaVistaDeProyectoConpersonalizacion(Guid PersonalizacionID, string pVista, bool pEsVistaRdfType)
        {
            string resultado = mEntityContext.VistaVirtual.Where(item => item.TipoPagina.Equals(pVista) && item.PersonalizacionID.Equals(PersonalizacionID)).Select(item => item.HTML).FirstOrDefault();

            if (pEsVistaRdfType)
            {
                resultado = mEntityContext.VistaVirtualRecursos.Where(item => item.RdfType.Equals(pVista.Replace(".cshtml", "")) && item.PersonalizacionID.Equals(PersonalizacionID)).Select(item => item.HTML).FirstOrDefault();
            }

            return resultado;
        }


        public string ObtenerHtmlParaVistaDePersonalizacion(Guid pPersonalizacionID, string pVista)
        {
            return mEntityContext.VistaVirtual.Where(item => item.TipoPagina.Equals(pVista) && item.PersonalizacionID.Equals(pPersonalizacionID)).Select(item => item.HTML).FirstOrDefault();
        }

        public string ObtenerHtmlParaVistaRDFTypeDePersonalizacion(Guid pPersonalizacionID, string pVista)
        {
            return mEntityContext.VistaVirtualRecursos.Where(item => item.RdfType.Equals(pVista) && item.PersonalizacionID.Equals(pPersonalizacionID)).Select(item => item.HTML).FirstOrDefault();
        }

        public string ObtenerHtmlParaVistaCMSDePersonalizacion(Guid pPersonalizacionID, Guid pPersonalizacionComponenteID)
        {
            return mEntityContext.VistaVirtualCMS.Where(item => item.PersonalizacionComponenteID.Equals(pPersonalizacionComponenteID) && item.PersonalizacionID.Equals(pPersonalizacionID)).Select(item => item.HTML).FirstOrDefault();
        }

        public string ObtenerHtmlParaVistaCMSPorTipo(string pTipoComponente)
        {
            return mEntityContext.VistaVirtualCMS.Where(item => item.TipoComponente.Contains(pTipoComponente)).Select(item => item.HTML).FirstOrDefault();
        }

        public string ObtenerHtmlParaVistaCMSPorTipoDePersonalizacion(string pTipoComponente, Guid pPersonalizacionComponenteID)
        {
            return mEntityContext.VistaVirtualCMS.Where(item => item.TipoComponente.Contains(pTipoComponente) && item.PersonalizacionComponenteID.Equals(pPersonalizacionComponenteID)).Select(item => item.HTML).FirstOrDefault();
        }

        public string ObtenerHtmlParaVistaGadgetDePersonalizacion(Guid pPersonalizacionID, Guid pPersonalizacionComponenteID)
        {
            return mEntityContext.VistaVirtualGadgetRecursos.Where(item => item.PersonalizacionComponenteID.Equals(pPersonalizacionComponenteID) && item.PersonalizacionID.Equals(pPersonalizacionID)).Select(item => item.HTML).FirstOrDefault();
        }

        public void ComprobarEInsertarPersonalizacionEcosistema(Guid pPersonalizacionID)
        {
            bool comprobacionID = mEntityContext.VistaVirtualPersonalizacion.Any(item => item.PersonalizacionID.Equals(pPersonalizacionID));

            if (!comprobacionID)
            {
                VistaVirtualPersonalizacion vistaVirtualPersonalizacion = new VistaVirtualPersonalizacion();
                vistaVirtualPersonalizacion.PersonalizacionID = pPersonalizacionID;

                mEntityContext.VistaVirtualPersonalizacion.Add(vistaVirtualPersonalizacion);

                ActualizarBaseDeDatosEntityContext();
            }
        }

        public Guid ComprobarEInsertarPersonalizacionProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            Guid personalizacionID = mEntityContext.VistaVirtualProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).Select(item => item.PersonalizacionID).FirstOrDefault();


            if (personalizacionID.Equals(Guid.Empty))
            {

                personalizacionID = Guid.NewGuid();

                VistaVirtualPersonalizacion vistaVirtualPersonalizacion = new VistaVirtualPersonalizacion();
                vistaVirtualPersonalizacion.PersonalizacionID = personalizacionID;

                mEntityContext.VistaVirtualPersonalizacion.Add(vistaVirtualPersonalizacion);

                VistaVirtualProyecto vistaVirtualProyecto = new VistaVirtualProyecto();
                vistaVirtualProyecto.PersonalizacionID = personalizacionID;
                vistaVirtualProyecto.ProyectoID = pProyectoID;
                vistaVirtualProyecto.OrganizacionID = pOrganizacionID;

                mEntityContext.SaveChanges();
            }

            return personalizacionID;
        }

        public void GuardarDatosExtrapersonalicacionComponente(Guid pPersonalicacionComponenteID, Tuple<string, string> pTuplasNombreDatosExtra)
        {
            List<VistaVirtualCMS> listaVistaVirtualCMS = mEntityContext.VistaVirtualCMS.Where(item => item.PersonalizacionComponenteID.Equals(pPersonalicacionComponenteID)).ToList();

            //Lo queremos eliminar si viene a null
            if (pTuplasNombreDatosExtra == null)
            {
                foreach (VistaVirtualCMS vistaVirtualCMS in listaVistaVirtualCMS)
                {
                    EliminarHtmlParaVistaDeComponenteCMSdeProyecto(vistaVirtualCMS.PersonalizacionID, vistaVirtualCMS.PersonalizacionComponenteID, vistaVirtualCMS.TipoComponente);
                }
            }
            else
            {
                foreach (VistaVirtualCMS vistaVirtualCMS in listaVistaVirtualCMS)
                {
                    vistaVirtualCMS.Nombre = pTuplasNombreDatosExtra.Item1;
                    vistaVirtualCMS.DatosExtra = pTuplasNombreDatosExtra.Item2;
                }
            }
            ActualizarBaseDeDatosEntityContext();
        }

        public bool ComprobarExisteVistaPersonalizadaEnProyecto(Guid pPersonalizacionID, string pTipoPagina, bool pEsVistaRdfType)
        {

            if (pEsVistaRdfType)
            {
                return mEntityContext.VistaVirtualRecursos.Any(item => item.PersonalizacionID.Equals(pPersonalizacionID) && item.RdfType.Equals(pTipoPagina));
            }
            else
            {
                return mEntityContext.VistaVirtual.Any(item => item.PersonalizacionID.Equals(pPersonalizacionID) && item.TipoPagina.Equals(pTipoPagina));
            }
        }

        public bool ComprobarExisteVistaPersonalizadaDeComponenteCMSEnProyecto(Guid pPersonalizacionID, Guid pPersonalizacionComponenteID)
        {
            return mEntityContext.VistaVirtualCMS.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID) && item.PersonalizacionComponenteID.Equals(pPersonalizacionComponenteID)).Any();
        }

        public void ActualizarVistaPersonalizadaEnProyecto(Guid pPersonalizacionID, string pTipoPagina, string pHTML, bool pEsVistaRdfType)
        {
            DateTime fechaActual = DateTime.Now;
            if (pEsVistaRdfType)
            {
                List<VistaVirtualRecursos> listaVistaVirtualRecursos = mEntityContext.VistaVirtualRecursos.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID) && item.RdfType.Equals(pTipoPagina)).ToList();

                foreach (VistaVirtualRecursos vistaVirtualRecursos in listaVistaVirtualRecursos)
                {
                    vistaVirtualRecursos.HTML = pHTML;
                    vistaVirtualRecursos.FechaModificacion = fechaActual;
                }
            }
            else
            {
                List<VistaVirtual> listaVistaVirtual = mEntityContext.VistaVirtual.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID) && item.TipoPagina.Equals(pTipoPagina)).ToList();

                foreach (VistaVirtual vistaVirtual in listaVistaVirtual)
                {
                    vistaVirtual.HTML = pHTML;
                    vistaVirtual.FechaModificacion = fechaActual;
                }
            }

            ActualizarBaseDeDatosEntityContext();
        }

        public void ActualizarVistaVirtual()
        {
            ActualizarBaseDeDatosEntityContext();
        }

        public void ActualizarVistaPersonalizadaDeComponenteCMSEnProyecto(Guid pPersonalizacionID, string pTipoComponente, string pNombre, Guid pPersonalizacionComponenteID, string pHTML, string pDatosExtra)
        {

            List<VistaVirtualCMS> listaVistaVirtualCMS = mEntityContext.VistaVirtualCMS.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID) && item.TipoComponente.Equals(pTipoComponente) && item.PersonalizacionComponenteID.Equals(pPersonalizacionComponenteID)).ToList();

            DateTime fechaActual = DateTime.Now;
            if (listaVistaVirtualCMS.Count > 0)
            {
                foreach (VistaVirtualCMS vistaVirtualCMS in listaVistaVirtualCMS)
                {
                    vistaVirtualCMS.HTML = pHTML;
                    vistaVirtualCMS.Nombre = pNombre;
                    vistaVirtualCMS.DatosExtra = pDatosExtra;
                    vistaVirtualCMS.FechaModificacion = fechaActual;
                }

                ActualizarBaseDeDatosEntityContext();
            }
            else
            {
                InsertarVistaPersonalizadaDeComponenteCMSEnProyecto(pPersonalizacionID, pTipoComponente, pNombre, pHTML, pPersonalizacionComponenteID, pDatosExtra);
            }
            
        }


        public void EliminarVistaPersonalizadaEnProyecto(Guid pPersonalizacionID, string pTipoPagina, bool pEsVistaRdfType)
        {
            if (pEsVistaRdfType)
            {
                var resultado = mEntityContext.VistaVirtualRecursos.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID) && item.RdfType.Equals(pTipoPagina)).ToList();
                foreach (var item in resultado)
                {
                    mEntityContext.Entry(item).State = EntityState.Deleted;
                }
            }
            else
            {
                var resultado = mEntityContext.VistaVirtual.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID) && item.TipoPagina.Equals(pTipoPagina)).ToList();
                foreach (var item in resultado)
                {
                    mEntityContext.Entry(item).State = EntityState.Deleted;
                }
            }

            ActualizarBaseDeDatosEntityContext();
        }

        public void EliminarHtmlParaVistaDeComponenteCMSdeProyecto(Guid pPersonalizacionID, Guid pPersonalizacionComponenteID, string pTipoComponente)
        {
            var listaVistaVirtualCMSEliminar = mEntityContext.VistaVirtualCMS.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID) && item.TipoComponente.Equals(pTipoComponente) && item.PersonalizacionComponenteID.Equals(pPersonalizacionComponenteID)).ToList();

            foreach (var vistaVirtualCMSEliminar in listaVistaVirtualCMSEliminar)
            {
                mEntityContext.EliminarElemento(vistaVirtualCMSEliminar);
            }

            ActualizarBaseDeDatosEntityContext();
        }
        public void EliminarHtmlParaVistaDeComponenteCMSdeProyectoServicioFTP(Guid pPersonalizacionID, string pTipoComponente, string pNombreVista)
        {
            var listaVistaVirtualCMSEliminar = mEntityContext.VistaVirtualCMS.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID) && item.TipoComponente.Equals(pTipoComponente) && item.Nombre.Equals(pNombreVista)).ToList();

            foreach (var vistaVirtualCMSEliminar in listaVistaVirtualCMSEliminar)
            {
                mEntityContext.Entry(vistaVirtualCMSEliminar).State = EntityState.Deleted;
            }

            ActualizarBaseDeDatosEntityContext();
        }
        public void RenombrarArchivoParaVistaComponenteCMSdeProyecto(Guid pPersonalizacionID, string pTipoComponente, string pNuevoTipoComponente)
        {
            var listaVistaVirtualCMSRenombrar = mEntityContext.VistaVirtualCMS.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID) && item.TipoComponente.Equals(pTipoComponente)).ToList();
            foreach (var vistaVirtualCMSRenombrar in listaVistaVirtualCMSRenombrar)
            {
                Guid personalizacionComponenteID = vistaVirtualCMSRenombrar.PersonalizacionComponenteID;
                string HTML = vistaVirtualCMSRenombrar.HTML;
                string nombre = vistaVirtualCMSRenombrar.Nombre;
                string datosExtra = vistaVirtualCMSRenombrar.DatosExtra;

                mEntityContext.Entry(vistaVirtualCMSRenombrar).State = EntityState.Deleted;
                VistaVirtualCMS vistaVirtualCMSAAniadir = new VistaVirtualCMS();
                vistaVirtualCMSAAniadir.PersonalizacionID = pPersonalizacionID;
                vistaVirtualCMSAAniadir.PersonalizacionComponenteID = personalizacionComponenteID;
                vistaVirtualCMSAAniadir.TipoComponente = pNuevoTipoComponente;
                vistaVirtualCMSAAniadir.HTML = HTML;
                vistaVirtualCMSAAniadir.Nombre = nombre;
                vistaVirtualCMSAAniadir.DatosExtra = datosExtra;

                mEntityContext.VistaVirtualCMS.Add(vistaVirtualCMSAAniadir);
            }
            ActualizarBaseDeDatosEntityContext();
        }
        public void RenombrarVistaPersonalizadaEnProyecto(Guid pPersonalizacionID, string pTipoPagina, string pNuevoTipoPagina, bool pEsVistaRdfType)
        {
            if (pEsVistaRdfType)
            {
                var resultado = mEntityContext.VistaVirtualRecursos.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID) && item.RdfType.Equals(pTipoPagina)).ToList();
                foreach (var item in resultado)
                {
                    string HTML = item.HTML;
                    mEntityContext.Entry(item).State = EntityState.Deleted;
                    VistaVirtualRecursos vistaVirtualRecursos = new VistaVirtualRecursos();
                    vistaVirtualRecursos.PersonalizacionID = pPersonalizacionID;
                    vistaVirtualRecursos.RdfType = pNuevoTipoPagina;
                    vistaVirtualRecursos.HTML = HTML;

                    mEntityContext.VistaVirtualRecursos.Add(vistaVirtualRecursos);
                }
            }
            else
            {
                var resultado = mEntityContext.VistaVirtual.Where(item => item.PersonalizacionID.Equals(pPersonalizacionID) && item.TipoPagina.Equals(pTipoPagina)).ToList();
                foreach (var item in resultado)
                {
                    string HTML = item.HTML;
                    mEntityContext.Entry(item).State = EntityState.Deleted;
                    VistaVirtual vistaVirtual = new VistaVirtual();
                    vistaVirtual.PersonalizacionID = pPersonalizacionID;
                    vistaVirtual.TipoPagina = pNuevoTipoPagina;
                    vistaVirtual.HTML = HTML;

                    mEntityContext.VistaVirtual.Add(vistaVirtual);
                }
            }

            ActualizarBaseDeDatosEntityContext();
        }
        public void InsertarVistaPersonalizadaEnProyecto(Guid pPersonalizacionID, string pTipoPagina, string pHTML, bool pEsVistaRdfType)
        {
            DateTime fechaActual = DateTime.Now;
            //TODO: revisar
            if (pEsVistaRdfType)
            {
                VistaVirtualRecursos vistaVirtualRecursos = new VistaVirtualRecursos();
                vistaVirtualRecursos.PersonalizacionID = pPersonalizacionID;
                vistaVirtualRecursos.RdfType = pTipoPagina;
                vistaVirtualRecursos.HTML = pHTML;
                vistaVirtualRecursos.FechaCreacion = fechaActual;
                vistaVirtualRecursos.FechaModificacion = fechaActual;

                mEntityContext.VistaVirtualRecursos.Add(vistaVirtualRecursos);
            }
            else
            {
                VistaVirtual vistaVirtual = new VistaVirtual();
                vistaVirtual.PersonalizacionID = pPersonalizacionID;
                vistaVirtual.TipoPagina = pTipoPagina;
                vistaVirtual.HTML = pHTML;
                vistaVirtual.FechaCreacion = fechaActual;
                vistaVirtual.FechaModificacion= fechaActual;

                mEntityContext.VistaVirtual.Add(vistaVirtual);
            }
            ActualizarBaseDeDatosEntityContext();
        }

        public void InsertarVistaPersonalizadaDeComponenteCMSEnProyecto(Guid pPersonalizacionID, string pTipoComponente, string pNombre, string pHTML, Guid pPersonalizacionComponenteID, string pDatosExtra)
        {
            // IBD.ReplaceParam("INSERT INTO VistaVirtualCMS (PersonalizacionID, TipoComponente, PersonalizacionComponenteID, HTML, Nombre, DatosExtra) VALUES (" + IBD.GuidParamColumnaTabla("PersonalizacionID") + ", @TipoComponente," + IBD.GuidParamColumnaTabla("PersonalizacionComponenteID") + ", @HTML, @Nombre, @DatosExtra)");
            DateTime fechaActual = DateTime.Now;

            VistaVirtualCMS vistaVirtualCMSAAniadir = new VistaVirtualCMS();
            vistaVirtualCMSAAniadir.PersonalizacionID = pPersonalizacionID;
            vistaVirtualCMSAAniadir.PersonalizacionComponenteID = pPersonalizacionComponenteID;
            vistaVirtualCMSAAniadir.TipoComponente = pTipoComponente;
            vistaVirtualCMSAAniadir.HTML = pHTML;
            vistaVirtualCMSAAniadir.Nombre = pNombre;
            vistaVirtualCMSAAniadir.DatosExtra = pDatosExtra;
            vistaVirtualCMSAAniadir.FechaCreacion = fechaActual;
            vistaVirtualCMSAAniadir.FechaModificacion = fechaActual;

            mEntityContext.VistaVirtualCMS.Add(vistaVirtualCMSAAniadir);

            ActualizarBaseDeDatosEntityContext();
        }


        #endregion

        #region Privados

        /// <summary>
        /// Procedimiento para construir las consultas
        /// </summary>
        /// <param name="IBD"></param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {
            #region DataAdapter

            sqlVistaVirtualPersonalizacionInsert = IBD.ReplaceParam("INSERT INTO VistaVirtualPersonalizacion (PersonalizacionID) VALUES (" + IBD.GuidParamColumnaTabla("PersonalizacionID") + ")");
            sqlVistaVirtualPersonalizacionDelete = IBD.ReplaceParam("DELETE FROM VistaVirtualPersonalizacion WHERE (PersonalizacionID = " + IBD.GuidParamColumnaTabla("O_PersonalizacionID") + ")");

            sqlVistaVirtualInsert = IBD.ReplaceParam("INSERT INTO VistaVirtual (PersonalizacionID, TipoPagina, HTML) VALUES (" + IBD.GuidParamColumnaTabla("PersonalizacionID") + ", @TipoPagina, @HTML)");
            sqlVistaVirtualDelete = IBD.ReplaceParam("DELETE FROM VistaVirtual WHERE (PersonalizacionID = " + IBD.GuidParamColumnaTabla("O_PersonalizacionID") + ") AND (TipoPagina = @O_TipoPagina)");
            sqlVistaVirtualModify = IBD.ReplaceParam("UPDATE VistaVirtual SET HTML = @HTML WHERE (PersonalizacionID = " + IBD.GuidParamColumnaTabla("O_PersonalizacionID") + ") AND (TipoPagina = @O_TipoPagina)");

            sqlVistaVirtualProyectoInsert = IBD.ReplaceParam("INSERT INTO VistaVirtualProyecto (OrganizacionID, ProyectoID, PersonalizacionID) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("PersonalizacionID") + ")");
            sqlVistaVirtualProyectoDelete = IBD.ReplaceParam("DELETE FROM VistaVirtualProyecto WHERE (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");
            sqlVistaVirtualProyectoModify = IBD.ReplaceParam("UPDATE VistaVirtualProyecto SET PersonalizacionID = " + IBD.GuidParamColumnaTabla("PersonalizacionID") + " WHERE (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            sqlVistaVirtualRecursosInsert = IBD.ReplaceParam("INSERT INTO VistaVirtualRecursos (PersonalizacionID, RdfType, HTML) VALUES (" + IBD.GuidParamColumnaTabla("PersonalizacionID") + ", @RdfType, @HTML)");
            sqlVistaVirtualRecursosDelete = IBD.ReplaceParam("DELETE FROM VistaVirtualRecursos WHERE (PersonalizacionID = " + IBD.GuidParamColumnaTabla("O_PersonalizacionID") + ") AND (RdfType = @O_RdfType)");
            sqlVistaVirtualRecursosModify = IBD.ReplaceParam("UPDATE VistaVirtualRecursos SET HTML = @HTML WHERE (PersonalizacionID = " + IBD.GuidParamColumnaTabla("O_PersonalizacionID") + ") AND (RdfType = @O_RdfType)");

            sqlVistaVirtualCMSInsert = IBD.ReplaceParam("INSERT INTO VistaVirtualCMS (PersonalizacionID, TipoComponente, PersonalizacionComponenteID, HTML, Nombre, DatosExtra) VALUES (" + IBD.GuidParamColumnaTabla("PersonalizacionID") + ", @TipoComponente," + IBD.GuidParamColumnaTabla("PersonalizacionComponenteID") + ", @HTML, @Nombre, @DatosExtra)");
            sqlVistaVirtualCMSDelete = IBD.ReplaceParam("DELETE FROM VistaVirtualCMS WHERE (PersonalizacionID = " + IBD.GuidParamColumnaTabla("O_PersonalizacionID") + ") AND (TipoComponente = @O_TipoComponente) AND (PersonalizacionComponenteID = " + IBD.GuidParamColumnaTabla("O_PersonalizacionComponenteID") + ")");
            sqlVistaVirtualCMSModify = IBD.ReplaceParam("UPDATE VistaVirtualCMS SET HTML = @HTML, Nombre = @Nombre, DatosExtra=@DatosExtra WHERE (PersonalizacionID = " + IBD.GuidParamColumnaTabla("O_PersonalizacionID") + ") AND (TipoComponente = @O_TipoComponente) AND (PersonalizacionComponenteID = " + IBD.GuidParamColumnaTabla("O_PersonalizacionComponenteID") + ")");

            sqlVistaVirtualGadgetRecursosInsert = IBD.ReplaceParam("INSERT INTO VistaVirtualGadgetRecursos (PersonalizacionID, PersonalizacionComponenteID, HTML, Nombre) VALUES (" + IBD.GuidParamColumnaTabla("PersonalizacionID") + ", " + IBD.GuidParamColumnaTabla("PersonalizacionComponenteID") + ", @HTML, @Nombre)");
            sqlVistaVirtualGadgetRecursosDelete = IBD.ReplaceParam("DELETE FROM VistaVirtualGadgetRecursos WHERE (PersonalizacionID = " + IBD.GuidParamColumnaTabla("O_PersonalizacionID") + ") AND (PersonalizacionComponenteID = " + IBD.GuidParamColumnaTabla("O_PersonalizacionComponenteID") + ")");
            sqlVistaVirtualGadgetRecursosModify = IBD.ReplaceParam("UPDATE VistaVirtualGadgetRecursos SET HTML = @HTML WHERE (PersonalizacionID = " + IBD.GuidParamColumnaTabla("O_PersonalizacionID") + ") AND (PersonalizacionComponenteID = " + IBD.GuidParamColumnaTabla("O_PersonalizacionComponenteID") + ")");

            #endregion

            selectVistaVirtual = "SELECT " + IBD.CargarGuid("VistaVirtual.PersonalizacionID") + ", VistaVirtual.TipoPagina, VistaVirtual.HTML FROM VistaVirtual ";

            selectVistaVirtualProyecto = "SELECT " + IBD.CargarGuid("VistaVirtualProyecto.OrganizacionID") + "," + IBD.CargarGuid("VistaVirtualProyecto.ProyectoID") + ", " + IBD.CargarGuid("VistaVirtualProyecto.PersonalizacionID") + " FROM VistaVirtualProyecto ";

            selectVistaVirtualRecursos = "SELECT " + IBD.CargarGuid("VistaVirtualRecursos.PersonalizacionID") + ", VistaVirtualRecursos.RdfType, VistaVirtualRecursos.HTML FROM VistaVirtualRecursos ";

            selectVistaVirtualCMS = "SELECT " + IBD.CargarGuid("VistaVirtualCMS.PersonalizacionID") + ", VistaVirtualCMS.TipoComponente, " + IBD.CargarGuid("VistaVirtualCMS.PersonalizacionComponenteID") + " , VistaVirtualCMS.HTML , VistaVirtualCMS.Nombre, VistaVirtualCMS.DatosExtra FROM VistaVirtualCMS ";

            selectVistaVirtualGadgetRecursos = "SELECT " + IBD.CargarGuid("VistaVirtualGadgetRecursos.PersonalizacionID") + ", " + IBD.CargarGuid("VistaVirtualGadgetRecursos.PersonalizacionComponenteID") + " , VistaVirtualGadgetRecursos.HTML , VistaVirtualGadgetRecursos.Nombre FROM VistaVirtualGadgetRecursos ";
        }

        #endregion

        #endregion
    }
}
