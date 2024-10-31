using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.Elementos.MapeoTesauroComunidad;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Es.Riam.Gnoss.CL.Documentacion
{
    public class DocumentacionCL : BaseCL, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Clase de negocio
        /// </summary>
        private DocumentacionCN mDocumentacionCN = null;

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.DOCUMENTACION };

        /// <summary>
        /// Duración de la caché para los controles de recursos (30 días en segundos).
        /// </summary>
        private const double DuracionCacheControlRecursos = 2592000;//30 días en segundos

        /// <summary>
        /// Duración de la caché para los contextos de recursos (1 día en segundos).
        /// </summary>
        private const double DuracionCacheContextosRecursos = 86400;//1 día,  antes => 432000;//5 días en segundos

        /// <summary>
        /// Duración de la caché para los contextos de recursos (5 días en segundos).
        /// </summary>
        private const double DuracionCacheFichaRecursos = 172800;//2 días en segundos

        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private RedisCacheWrapper mRedisCacheWrapper;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor para DocumentacionCL
        /// </summary>
        public DocumentacionCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        /// <summary>
        /// Constructor para DocumentacionCL
        /// </summary>
        /// <param name="pPoolName">Nombre del pool de conexión</param>
        public DocumentacionCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        /// <summary>
        /// Constructor para DocumentacionCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pPoolName">Nombre del pool de conexión</param>
        public DocumentacionCL(string pFicheroConfiguracionBD, string pPoolName, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, pPoolName, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        #endregion

        #region Metodos

        public FacetadoDS ObtenerFichaRecursoPropiedadesSemanticasMVC(Guid pDocumentoID, Guid pProyectoID, string pIdioma)
        {
            List<Guid> listaDoc = new List<Guid>();
            listaDoc.Add(pDocumentoID);
            return ObtenerFichasRecursosPropiedadesSemanticasMVC(listaDoc, pProyectoID, pIdioma)[pDocumentoID];
        }

        public Dictionary<Guid, FacetadoDS> ObtenerFichasRecursosPropiedadesSemanticasMVC(List<Guid> pListaDocumentosID, Guid pProyectoID, string pIdioma)
        {
            Dictionary<Guid, FacetadoDS> listaFichas = new Dictionary<Guid, FacetadoDS>();
            if (pListaDocumentosID.Count > 0)
            {
                Dictionary<Guid, string> keysRecursos = new Dictionary<Guid, string>();
                string[] listaClaves = new string[pListaDocumentosID.Count];
                int i = 0;
                foreach (Guid idRecurso in pListaDocumentosID)
                {
                    string clave = ObtenerClaveCache(NombresCL.FICHARECURSOMVC + "_" + idRecurso + "_" + pProyectoID + "_propsem");
                    keysRecursos.Add(idRecurso, clave.ToLower());
                    listaClaves[i] = clave.ToLower();
                    i++;
                }
                Dictionary<string, object> objetosCache = ObtenerListaObjetosCache(listaClaves, typeof(FacetadoDS));
                foreach (Guid idRecurso in keysRecursos.Keys)
                {
                    string clave = keysRecursos[idRecurso];
                    Dictionary<string, FacetadoDS> configVistaFicha = (Dictionary<string, FacetadoDS>)(objetosCache[clave]);
                    if (configVistaFicha != null && configVistaFicha.ContainsKey(pIdioma))
                    {
                        listaFichas.Add(idRecurso, configVistaFicha[pIdioma]);
                    }
                }
            }
            return listaFichas;
        }

        public Dictionary<Guid, FacetadoDS> ObtenerFichasRecursosPropiedadesPersonalizadasSemanticasMVC(List<Guid> pListaDocumentosID, Guid pProyectoID, string pIdioma)
        {
            Dictionary<Guid, FacetadoDS> listaFichas = new Dictionary<Guid, FacetadoDS>();
            if (pListaDocumentosID.Count > 0)
            {
                Dictionary<Guid, string> keysRecursos = new Dictionary<Guid, string>();
                string[] listaClaves = new string[pListaDocumentosID.Count];
                int i = 0;
                foreach (Guid idRecurso in pListaDocumentosID)
                {
                    string clave = ObtenerClaveCache(NombresCL.FICHARECURSOMVC + "_" + idRecurso + "_" + pProyectoID + "_propsempersonalizadas");
                    keysRecursos.Add(idRecurso, clave.ToLower());
                    listaClaves[i] = clave.ToLower();
                    i++;
                }
                Dictionary<string, object> objetosCache = ObtenerListaObjetosCache(listaClaves, typeof(FacetadoDS));
                foreach (Guid idRecurso in keysRecursos.Keys)
                {
                    string clave = keysRecursos[idRecurso];
                    Dictionary<string, FacetadoDS> configVistaFicha = (Dictionary<string, FacetadoDS>)(objetosCache[clave]);
                    if (configVistaFicha != null && configVistaFicha.ContainsKey(pIdioma))
                    {
                        listaFichas.Add(idRecurso, configVistaFicha[pIdioma]);
                    }
                }
            }
            return listaFichas;
        }

        public void AgregarFichaRecursoPropiedadesSemanticasMVC(Guid pDocumentoID, Guid pProyectoID, FacetadoDS pFichaRecursoMVC, string pIdioma)
        {
            Dictionary<Guid, FacetadoDS> listaRecursos = new Dictionary<Guid, FacetadoDS>();
            listaRecursos.Add(pDocumentoID, pFichaRecursoMVC);
            AgregarFichasRecursosPropiedadesSemanticasMVC(listaRecursos, pProyectoID, pIdioma);
        }

        public void AgregarFichasRecursosPropiedadesSemanticasMVC(Dictionary<Guid, FacetadoDS> pListaRecursos, Guid pProyectoID, string pIdioma)
        {
            Dictionary<Guid, string> keysRecursos = new Dictionary<Guid, string>();
            string[] listaClaves = new string[pListaRecursos.Count];
            object[] listaObjetosAgregar = new object[pListaRecursos.Count];
            int i = 0;
            foreach (Guid idRecurso in pListaRecursos.Keys)
            {
                string clave = ObtenerClaveCache(NombresCL.FICHARECURSOMVC + "_" + idRecurso + "_" + pProyectoID + "_propsem");
                keysRecursos.Add(idRecurso, clave.ToLower());
                listaClaves[i] = clave.ToLower();
                i++;
            }
            Dictionary<string, object> objetosCache = ObtenerListaObjetosCache(listaClaves, typeof(FacetadoDS));
            i = 0;
            foreach (Guid idRecurso in keysRecursos.Keys)
            {
                string clave = keysRecursos[idRecurso];
                Dictionary<string, FacetadoDS> configVistaFicha = (Dictionary<string, FacetadoDS>)(objetosCache[clave]);
                if (configVistaFicha == null)
                {
                    configVistaFicha = new Dictionary<string, FacetadoDS>();
                }
                if (configVistaFicha.ContainsKey(pIdioma))
                {
                    configVistaFicha[pIdioma] = pListaRecursos[idRecurso];
                }
                else
                {
                    configVistaFicha.Add(pIdioma, pListaRecursos[idRecurso]);
                }
                listaObjetosAgregar[i] = configVistaFicha;
                i++;
            }
            AgregarListaObjetosCache(listaClaves, listaObjetosAgregar, false, DuracionCacheFichaRecursos);
        }

        public void AgregarFichasRecursosPropiedadesPersonalizadasSemanticasMVC(Dictionary<Guid, FacetadoDS> pListaRecursos, Guid pProyectoID, string pIdioma)
        {
            Dictionary<Guid, string> keysRecursos = new Dictionary<Guid, string>();
            string[] listaClaves = new string[pListaRecursos.Count];
            object[] listaObjetosAgregar = new object[pListaRecursos.Count];
            int i = 0;
            foreach (Guid idRecurso in pListaRecursos.Keys)
            {
                string clave = ObtenerClaveCache(NombresCL.FICHARECURSOMVC + "_" + idRecurso + "_" + pProyectoID + "_propsempersonalizadas");
                keysRecursos.Add(idRecurso, clave.ToLower());
                listaClaves[i] = clave.ToLower();
                i++;
            }
            Dictionary<string, object> objetosCache = ObtenerListaObjetosCache(listaClaves, typeof(FacetadoDS));
            i = 0;
            foreach (Guid idRecurso in keysRecursos.Keys)
            {
                string clave = keysRecursos[idRecurso];
                Dictionary<string, FacetadoDS> configVistaFicha = (Dictionary<string, FacetadoDS>)(objetosCache[clave]);
                if (configVistaFicha == null)
                {
                    configVistaFicha = new Dictionary<string, FacetadoDS>();
                }
                if (configVistaFicha.ContainsKey(pIdioma))
                {
                    configVistaFicha[pIdioma] = pListaRecursos[idRecurso];
                }
                else
                {
                    configVistaFicha.Add(pIdioma, pListaRecursos[idRecurso]);
                }
                listaObjetosAgregar[i] = configVistaFicha;
                i++;
            }
            AgregarListaObjetosCache(listaClaves, listaObjetosAgregar, false, DuracionCacheFichaRecursos);
        }

        public ResourceModel ObtenerFichaRecursoMVC(Guid pDocumentoID, Guid pProyectoID)
        {
            List<Guid> listaDoc = new List<Guid>();
            listaDoc.Add(pDocumentoID);
            return ObtenerFichasRecursoMVC(listaDoc, pProyectoID)[pDocumentoID];
        }

        public Dictionary<Guid, ResourceModel> ObtenerFichasRecursoMVC(List<Guid> pListaDocumentosID, Guid pProyectoID)
        {
            Dictionary<Guid, ResourceModel> listaFichas = new Dictionary<Guid, ResourceModel>();
            if (pListaDocumentosID.Count > 0)
            {
                Dictionary<Guid, string> keysRecursos = new Dictionary<Guid, string>();
                string[] listaClaves = new string[pListaDocumentosID.Count];
                int i = 0;
                foreach (Guid idRecurso in pListaDocumentosID)
                {
                    string clave = ObtenerClaveCache(NombresCL.FICHARECURSOMVC + "_" + idRecurso + "_" + pProyectoID);
                    if (!keysRecursos.ContainsKey(idRecurso))
                    {
                        keysRecursos.Add(idRecurso, clave.ToLower());
                    }
                    listaClaves[i] = clave.ToLower();
                    i++;
                }
                Dictionary<string, object> objetosCache = ObtenerListaObjetosCache(listaClaves, typeof(ResourceModel));
                foreach (Guid idRecurso in keysRecursos.Keys)
                {
                    string clave = keysRecursos[idRecurso];

                    if (!objetosCache.ContainsKey(clave))
                    {
                        objetosCache.Add(clave, null);
                    }
                    else if (objetosCache[clave] != null && ResourceModel.LastCacheVersion > ((ResourceModel)objetosCache[clave]).CacheVersion)
                    {
                        objetosCache[clave] = null;
                    }
                    else if (objetosCache[clave] != null && !((ResourceModel)objetosCache[clave]).Key.Equals(idRecurso))
                    {
                        objetosCache[clave] = null;
                    }

                    listaFichas.Add(idRecurso, (ResourceModel)(objetosCache[clave]));
                }
            }
            return listaFichas;
        }

        public void AgregarFichaRecursoMVC(Guid pDocumentoID, Guid pProyectoID, ResourceModel pFichaRecursoMVC)
        {
            Dictionary<Guid, ResourceModel> listaRecursos = new Dictionary<Guid, ResourceModel>();
            listaRecursos.Add(pDocumentoID, pFichaRecursoMVC);
            AgregarFichasRecursoMVC(listaRecursos, pProyectoID);
        }

        public void AgregarFichasRecursoMVC(Dictionary<Guid, ResourceModel> pListaRecursos, Guid pProyectoID)
        {
            string[] listaClavesAgregar = new string[pListaRecursos.Count];
            object[] listaObjetosAgregar = new object[pListaRecursos.Count];
            int j = 0;
            foreach (Guid idRecurso in pListaRecursos.Keys)
            {
                pListaRecursos[idRecurso].CacheVersion = ResourceModel.LastCacheVersion;

                listaClavesAgregar[j] = NombresCL.FICHARECURSOMVC + "_" + idRecurso + "_" + pProyectoID;
                listaObjetosAgregar[j] = pListaRecursos[idRecurso];
                j++;
            }
            AgregarListaObjetosCache(listaClavesAgregar, listaObjetosAgregar, true, DuracionCacheFichaRecursos);
        }

        public void InvalidarFichaRecursoMVC(Guid pDocumentoID, Guid pProyectoID)
        {
            List<Guid> listaDoc = new List<Guid>();
            listaDoc.Add(pDocumentoID);
            InvalidarFichasRecursoMVC(listaDoc, pProyectoID);
        }

        public void InvalidarFichasRecursoMVC(List<Guid> pListaRecursosID, Guid pProyectoID)
        {
            List<string> listaClavesEliminar = new List<string>();
            foreach (Guid idRecurso in pListaRecursosID)
            {
                listaClavesEliminar.Add(ObtenerClaveCache(NombresCL.FICHARECURSOMVC + "_" + idRecurso + "_" + pProyectoID).ToLower());
                listaClavesEliminar.Add(ObtenerClaveCache(NombresCL.FICHARECURSOMVC + "_" + idRecurso + "_" + pProyectoID + "_propsem").ToLower());
                listaClavesEliminar.Add(ObtenerClaveCache(NombresCL.FICHARECURSOMVC + "_" + idRecurso + "_" + pProyectoID + "_propsempersonalizadas").ToLower());
            }
            InvalidarCachesMultiples(listaClavesEliminar);
        }

        public Dictionary<Guid, List<ResourceEventModel>> ObtenerEventosRecursoMVC(List<Guid> pListaDocumentosID, Guid pProyectoID)
        {
            Dictionary<Guid, List<ResourceEventModel>> listaEventos = new Dictionary<Guid, List<ResourceEventModel>>();
            if (pListaDocumentosID.Count > 0)
            {
                Dictionary<Guid, string> keysRecursos = new Dictionary<Guid, string>();
                string[] listaClaves = new string[pListaDocumentosID.Count];
                int i = 0;
                foreach (Guid idRecurso in pListaDocumentosID)
                {
                    string clave = ObtenerClaveCache("FichaEventosRecursoMVC_" + idRecurso + "_" + pProyectoID);
                    keysRecursos.Add(idRecurso, clave.ToLower());
                    listaClaves[i] = clave.ToLower();
                    i++;
                }
                Dictionary<string, object> objetosCache = ObtenerListaObjetosCache(listaClaves, typeof(ResourceEventModel));
                foreach (Guid idRecurso in keysRecursos.Keys)
                {
                    string clave = keysRecursos[idRecurso];
                    listaEventos.Add(idRecurso, (List<ResourceEventModel>)(objetosCache[clave]));
                }
            }
            return listaEventos;
        }

        public object ObtenerComentariosRecursoMVC(Guid pDocumentoID, Guid pProyectoID)
        {
            string rawKey = "ComentariosRecursoMVC_" + pDocumentoID + "_" + pProyectoID;

            object comentarios = ObtenerObjetoDeCache(rawKey);

            return comentarios;
        }

        public object ObtenerVinculadosRecursoMVC(Guid pDocumentoID, Guid pProyectoID)
        {
            string rawKey = "VinculadosRecursoMVC_" + pDocumentoID + "_" + pProyectoID;

            object vinculados = ObtenerObjetoDeCache(rawKey);

            return vinculados;
        }

        public List<Guid> ObtenerContextoRecursoMVC(Guid pDocumentoID, Guid pProyectoID, Guid pGadgetID)
        {
            string rawKey = "ContextoRecursoMVC_" + pDocumentoID + "_" + pProyectoID + "_" + pGadgetID;

            List<Guid> contexto = ObtenerObjetoDeCache(rawKey) as List<Guid>;

            return contexto;
        }

        public object ObtenerRelacionadosRecursoMVC(Guid pDocumentoID, Guid pProyectoID)
        {
            string rawKey = "RelacionadosRecursoMVC_" + pDocumentoID + "_" + pProyectoID;

            object relaccionados = ObtenerObjetoDeCache(rawKey);

            return relaccionados;
        }

        public void AgregarEventosRecursoMVC(Dictionary<Guid, List<ResourceEventModel>> pListaEventosRecursos, Guid pProyectoID)
        {
            string[] listaClavesAgregar = new string[pListaEventosRecursos.Count];
            object[] listaObjetosAgregar = new object[pListaEventosRecursos.Count];
            int j = 0;
            foreach (Guid idRecurso in pListaEventosRecursos.Keys)
            {
                listaClavesAgregar[j] = "FichaEventosRecursoMVC_" + idRecurso + "_" + pProyectoID;
                listaObjetosAgregar[j] = pListaEventosRecursos[idRecurso];
                j++;
            }
            AgregarListaObjetosCache(listaClavesAgregar, listaObjetosAgregar, true, DuracionCacheFichaRecursos);
        }

        public void AgregarComentariosRecursoMVC(Guid pDocumentoID, Guid pProyectoID, object pComentariosRecursoMVC)
        {
            string rawKey = "ComentariosRecursoMVC_" + pDocumentoID + "_" + pProyectoID;

            AgregarObjetoCache(rawKey, pComentariosRecursoMVC, DuracionCacheFichaRecursos);
        }

        public void AgregarVinculadosRecursoMVC(Guid pDocumentoID, Guid pProyectoID, object pComentariosRecursoMVC)
        {
            string rawKey = "VinculadosRecursoMVC_" + pDocumentoID + "_" + pProyectoID;

            AgregarObjetoCache(rawKey, pComentariosRecursoMVC, DuracionCacheContextosRecursos);
        }

        public void AgregarContextoRecursoMVC(Guid pDocumentoID, Guid pProyectoID, Guid pGadgetID, List<Guid> pListaRecursosContextoRecursoMVC)
        {
            string rawKey = "ContextoRecursoMVC_" + pDocumentoID + "_" + pProyectoID + "_" + pGadgetID;

            AgregarObjetoCache(rawKey, pListaRecursosContextoRecursoMVC, DuracionCacheContextosRecursos);
        }

        public void AgregarRelacionadosRecursoMVC(Guid pDocumentoID, Guid pProyectoID, object pRelacionadosRecursoMVC)
        {
            string rawKey = "RelacionadosRecursoMVC_" + pDocumentoID + "_" + pProyectoID;

            AgregarObjetoCache(rawKey, pRelacionadosRecursoMVC, DuracionCacheContextosRecursos);
        }

        public void InvalidarEventosRecursoMVC(Guid pDocumentoID, Guid pProyectoID)
        {
            string rawKey = "FichaEventosRecursoMVC_" + pDocumentoID + "_" + pProyectoID;

            InvalidarCache(rawKey);
        }

        public void InvalidarComentariosRecursoMVC(Guid pDocumentoID, Guid pProyectoID)
        {
            string rawKey = "ComentariosRecursoMVC_" + pDocumentoID + "_" + pProyectoID;

            InvalidarCache(rawKey);
        }

        public void InvalidarVinculadosRecursoMVC(Guid pDocumentoID, Guid pProyectoID)
        {
            string rawKey = "VinculadosRecursoMVC_" + pDocumentoID + "_" + pProyectoID;

            InvalidarCache(rawKey);
        }

        public void InvalidarContextoRecursoMVC(Guid pDocumentoID, Guid pProyectoID, Guid pGadgetID)
        {
            string rawKey = "ContextoRecursoMVC_" + pDocumentoID + "_" + pProyectoID + "_" + pGadgetID;

            InvalidarCache(rawKey);
        }

		public void InvalidarContextoRecursoMVC(Guid pDocumentoID, Guid pProyectoID)
		{
			string rawKey = "ContextoRecursoMVC_" + pDocumentoID + "_" + pProyectoID;

			InvalidarCacheQueContengaCadena(rawKey);
		}

		public void InvalidarRelacionadosRecursoMVC(Guid pDocumentoID, Guid pProyectoID)
        {
            string rawKey = "RelacionadosRecursoMVC_" + pDocumentoID + "_" + pProyectoID;

            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Obtiene si un perfil tiene acceso a recursos privados en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <param name="pTipoDocumento">Integer que indica el tipo de documento que se carga de la cache (-1 para recursos)</param>
        /// <returns>TRUE si tiene acceso a recursos privados, FALSE en caso contrario</returns>
        public bool TienePrivados(Guid pProyectoID, Guid pPerfilID, int pTipoDocumento)
        {
            switch (pTipoDocumento)
            {
                case -1:
                case (int)TiposDocumentacion.Debate:
                    //return PerfilesConRecursosPrivados(pProyectoID).Contains(pPerfilID);
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Obtiene los perfiles que tienen recursos privados en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista con los perfiles</returns>
        public List<Guid> PerfilesConRecursosPrivados(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.PERFILESCONRECURSOSPRIVADOS, "_", pProyectoID.ToString());

            List<Guid> perfilesConRecursosPrivados = ObtenerObjetoDeCache(rawKey) as List<Guid>;

            if (perfilesConRecursosPrivados == null)
            {
                perfilesConRecursosPrivados = DocumentacionCN.ObtenerPerfilesConRecursosPrivados(pProyectoID);
                AgregarObjetoCache(rawKey, perfilesConRecursosPrivados);
            }
            return perfilesConRecursosPrivados;
        }

        /// <summary>
        /// Obtiene los perfiles que tienen recursos privados en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista con los perfiles</returns>
        public void InvalidarPerfilesConRecursosPrivados(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.PERFILESCONRECURSOSPRIVADOS, "_", pProyectoID.ToString());
            InvalidarCache(rawKey);
            VersionarCacheLocal(pProyectoID);
        }


        /// <summary>
        /// Obtiene el Enlace de un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></returns>
        public string ObtenerEnlaceDocumentoPorDocumentoID(Guid pDocumentoID)
        {
            string rawKey = string.Concat("Enlace_", pDocumentoID.ToString());
            string enlace = (string)ObtenerObjetoDeCache(rawKey);
            if (enlace == null)
            {
                enlace = DocumentacionCN.ObtenerEnlaceDocumentoPorDocumentoID(pDocumentoID);
                AgregarObjetoCache(rawKey, enlace);
            }
            return enlace;
        }

        public DataWrapperDocumentacion ObtenerOntologiasProyecto(Guid pProyectoID, bool pTraerOntosEntorno)
        {
            string rawKey = string.Concat("DocumentosOntologiasProyecto_", pProyectoID.ToString());
            mEntityContext.UsarEntityCache = true;
            DataWrapperDocumentacion docDW = ObtenerObjetoDeCache(rawKey) as DataWrapperDocumentacion;

            if (docDW == null)
            {
                docDW = new DataWrapperDocumentacion();

                if (TieneComunidadPadreConfigurada(pProyectoID))
                {
                    DocumentacionCN.ObtenerOntologiasProyecto(ProyectoIDPadreEcosistema.Value, docDW, false, true, false);
                    ModificarDataWrapperComunidadHijaNivelesCertificacion(pProyectoID, docDW);
                }
                else
                {
                    DocumentacionCN.ObtenerOntologiasProyecto(pProyectoID, docDW, false, true, false);
                }

                if (docDW != null)
                {
                    docDW.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, docDW);
            }

            if (pTraerOntosEntorno)
            {
                DataWrapperDocumentacion docDWEntorno = ObtenerObjetoDeCache("DocumentosOntologiasEntorno") as DataWrapperDocumentacion;

                if (docDWEntorno == null)
                {
                    docDWEntorno = DocumentacionCN.ObtenerOntologiasEntorno();
                    if (docDWEntorno != null)
                    {
                        docDWEntorno.CargaRelacionesPerezosasCache();
                    }
                    AgregarObjetoCache("DocumentosOntologiasEntorno", docDWEntorno);
                }

                docDW.Merge(docDWEntorno);
            }
            mEntityContext.UsarEntityCache = false;
            return docDW;
        }


        public void InvalidarOntologiasProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat("DocumentosOntologiasProyecto_", pProyectoID.ToString());
            InvalidarCache(rawKey);

            InvalidarCache("DocumentosOntologiasEntorno");
        }

        #region Primeros recursos de una comunidad

        /// <summary>
        /// Obtiene los primeros recursos de una comunidad
        /// </summary>
        /// <param name="pDocumentacionDW">DataSet de documentación donde se insertaran los resultados</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pPerfilID">Identificador del perfil del usuario en el proyecto</param>
        /// <param name="pCuantos">Número total de resultados</param>
        /// <param name="pTipoDocumento">Integer que indica el tipo de documento que se carga de la cache (-1 para recursos)</param>
        /// <returns>TRUE si lo encuentra en cache, FALSE en caso contrario</returns>
        public bool ObtenerPrimerosRecursos(DataWrapperDocumentacion pDocumentacionDW, Guid pProyectoID, Guid pPerfilID, out int pCuantos, int pTipoDocumento)
        {
            int? cuantos;
            DataWrapperDocumentacion documentacionDS;
            string rawKey = "";
            string rawKeyCuantos = "";

            switch (pTipoDocumento)
            {
                case -1:
                    rawKey = string.Concat(NombresCL.PRIMEROSRECURSOS, "_", pProyectoID.ToString());
                    rawKeyCuantos = string.Concat(NombresCL.TOTALRESULTADOSRECURSOS, "_", pProyectoID.ToString());

                    if (TienePrivados(pProyectoID, pPerfilID, pTipoDocumento))
                    {
                        rawKey = string.Concat(rawKey, "_", pPerfilID.ToString());
                        rawKeyCuantos = string.Concat(rawKeyCuantos, "_", pPerfilID.ToString());
                    }
                    break;
                case (int)TiposDocumentacion.Debate:
                    rawKey = string.Concat(NombresCL.PRIMEROSDEBATES, "_", pProyectoID.ToString());
                    rawKeyCuantos = string.Concat(NombresCL.TOTALRESULTADOSDEBATES, "_", pProyectoID.ToString());

                    if (TienePrivados(pProyectoID, pPerfilID, pTipoDocumento))
                    {
                        rawKey = string.Concat(rawKey, "_", pPerfilID.ToString());
                        rawKeyCuantos = string.Concat(rawKeyCuantos, "_", pPerfilID.ToString());
                    }
                    break;
                case (int)TiposDocumentacion.Pregunta:
                    rawKey = string.Concat(NombresCL.PRIMERASPREGUNTAS, "_", pProyectoID.ToString());
                    rawKeyCuantos = string.Concat(NombresCL.TOTALRESULTADOSPREGUNTAS, "_", pProyectoID.ToString());
                    break;
            }

            documentacionDS = (DataWrapperDocumentacion)ObtenerObjetoDeCache(rawKey);
            cuantos = (int?)ObtenerObjetoDeCache(string.Concat(rawKeyCuantos));
            if (cuantos.HasValue)
            {
                pCuantos = cuantos.Value;
            }
            else
            {
                pCuantos = 0;
            }

            if (documentacionDS != null)
            {
                pDocumentacionDW.Merge(documentacionDS);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Obtiene los primeros recursos de una comunidad
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación donde se insertaran los resultados</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pPerfilID">Identificador del perfil del usuario en el proyecto</param>
        /// <param name="pCuantos">Número total de resultados</param>
        /// <param name="pTipoDocumento">Integer que indica el tipo de documento que se carga de la cache (-1 para recursos)</param>
        public void AgregarPrimerosRecursos(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoID, Guid pPerfilID, int pCuantos, int pTipoDocumento)
        {
            string rawKey = "";
            string rawKeyCuantos = "";

            switch (pTipoDocumento)
            {
                case -1:
                    rawKey = string.Concat(NombresCL.PRIMEROSRECURSOS, "_", pProyectoID.ToString());
                    rawKeyCuantos = string.Concat(NombresCL.TOTALRESULTADOSRECURSOS, "_", pProyectoID.ToString());

                    if (TienePrivados(pProyectoID, pPerfilID, pTipoDocumento))
                    {
                        rawKey = string.Concat(rawKey, "_", pPerfilID.ToString());
                        rawKeyCuantos = string.Concat(rawKeyCuantos, "_", pPerfilID.ToString());
                    }
                    break;
                case (int)TiposDocumentacion.Debate:
                    rawKey = string.Concat(NombresCL.PRIMEROSDEBATES, "_", pProyectoID.ToString());
                    rawKeyCuantos = string.Concat(NombresCL.TOTALRESULTADOSDEBATES, "_", pProyectoID.ToString());

                    if (TienePrivados(pProyectoID, pPerfilID, pTipoDocumento))
                    {
                        rawKey = string.Concat(rawKey, "_", pPerfilID.ToString());
                        rawKeyCuantos = string.Concat(rawKeyCuantos, "_", pPerfilID.ToString());
                    }
                    break;
                case (int)TiposDocumentacion.Pregunta:
                    rawKey = string.Concat(NombresCL.PRIMERASPREGUNTAS, "_", pProyectoID.ToString());
                    rawKeyCuantos = string.Concat(NombresCL.TOTALRESULTADOSPREGUNTAS, "_", pProyectoID.ToString());
                    break;
            }

            AgregarObjetoCache(rawKey, pDataWrapperDocumentacion);
            AgregarObjetoCache(rawKeyCuantos, pCuantos);
            if (pDataWrapperDocumentacion != null)
            {
                pDataWrapperDocumentacion.CargaRelacionesPerezosasCache();
            }
        }

        /// <summary>
        /// Borra los primeros recursos de la cache para un proyecto dado
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipoDocumento">Integer que indica el tipo de documento que se carga de la cache (-1 para recursos)</param>
        public void BorrarPrimerosRecursos(Guid pProyectoID, int pTipoDocumento)
        {
            string rawKey = "";
            string rawKeyCuantos = "";
            string rawKeyPerfilesConPrivados = "";

            switch (pTipoDocumento)
            {
                case -1:
                    rawKey = ObtenerClaveCache(string.Concat(NombresCL.PRIMEROSRECURSOS, "_", pProyectoID.ToString()));
                    rawKeyCuantos = ObtenerClaveCache(string.Concat(NombresCL.TOTALRESULTADOSRECURSOS, "_", pProyectoID.ToString()));
                    rawKeyPerfilesConPrivados = ObtenerClaveCache(string.Concat(NombresCL.PERFILESCONRECURSOSPRIVADOS, "_", pProyectoID.ToString()));
                    break;
                case (int)TiposDocumentacion.Debate:
                    rawKey = ObtenerClaveCache(string.Concat(NombresCL.PRIMEROSDEBATES, "_", pProyectoID.ToString()));
                    rawKeyCuantos = ObtenerClaveCache(string.Concat(NombresCL.TOTALRESULTADOSDEBATES, "_", pProyectoID.ToString()));
                    rawKeyPerfilesConPrivados = ObtenerClaveCache(string.Concat(NombresCL.PERFILESCONDEBATESPRIVADOS, "_", pProyectoID.ToString()));
                    break;
                case (int)TiposDocumentacion.Pregunta:
                    rawKey = ObtenerClaveCache(string.Concat(NombresCL.PRIMERASPREGUNTAS, "_", pProyectoID.ToString()));
                    rawKeyCuantos = ObtenerClaveCache(string.Concat(NombresCL.TOTALRESULTADOSPREGUNTAS, "_", pProyectoID.ToString()));
                    break;
            }

            List<string> claves = new List<string>();
            if (ClienteRedisLectura != null)
            {
                claves = ClienteRedisLectura.Keys(rawKey.ToLower() + "*").Result.ToList();
            }

            if (ClienteRedisLectura != null)
            {
                claves.AddRange(ClienteRedisLectura.Keys(rawKeyCuantos.ToLower() + "*").Result.ToList());
            }

            if (pTipoDocumento == -1 || pTipoDocumento == (int)TiposDocumentacion.Debate)
            {
                if (ClienteRedisLectura != null)
                {
                    claves.AddRange(ClienteRedisLectura.Keys(rawKeyPerfilesConPrivados.ToLower() + "*").Result.ToList());
                }
            }

            InvalidarCachesMultiples(claves);
        }

        /// <summary>
        /// Borra la cache de los primeros recursos si el recurso esta entre ellos
        /// </summary>
        /// <param name="pRecursoID">Identificador del recurso</param>
        /// <param name="pListaProyectoID">Lista de identificadores de los proyectos</param>
        /// <param name="pTipoDocumento">Integer que indica el tipo de documento que se carga de la cache (-1 para recursos)</param>
        public void BorrarPrimerosRecursosSiEstaRecurso(Guid pRecursoID, List<Guid> pListaProyectoID, int pTipoDocumento)
        {
            DataWrapperDocumentacion docDW = new DataWrapperDocumentacion();


            //Recorremos los proyectos
            foreach (Guid proyectoID in pListaProyectoID)
            {
                string rawKey = "";
                switch (pTipoDocumento)
                {
                    case -1:
                        rawKey = ObtenerClaveCache(string.Concat(NombresCL.PRIMEROSRECURSOS, "_", proyectoID.ToString()));
                        break;
                    case (int)TiposDocumentacion.Debate:
                        rawKey = ObtenerClaveCache(string.Concat(NombresCL.PRIMEROSDEBATES, "_", proyectoID.ToString()));
                        break;
                    case (int)TiposDocumentacion.Pregunta:
                        rawKey = ObtenerClaveCache(string.Concat(NombresCL.PRIMERASPREGUNTAS, "_", proyectoID.ToString()));
                        break;
                }

                List<string> claves = new List<string>();
                if (ClienteRedisLectura != null)
                {
                    claves = ClienteRedisLectura.Keys(rawKey.ToLower() + "*").Result.ToList();
                }
                List<string> clavesEliminar = new List<string>(claves);

                foreach (string claveCache in claves)
                {
                    //Para cada cache obtenida, comprobamos si el documento esta en ella
                    docDW = (DataWrapperDocumentacion)ObtenerObjetoDeCache(claveCache, false);
                    if (docDW != null)
                    {
                        bool contieneElRecurso = false;
                        foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in docDW.ListaDocumento)
                        {
                            if (filaDoc.DocumentoID == pRecursoID)
                            {
                                contieneElRecurso = true;
                                break;
                            }
                        }

                        //Si el documento esta en ella, borramos esa cache, asi como la que lleva el nº de recursos y la lista de gente que tiene recursos privados
                        if (contieneElRecurso)
                        {
                            switch (pTipoDocumento)
                            {
                                case -1:
                                    //InvalidarCache(claveCache, false);                                    
                                    //InvalidarCache(claveCache.Replace(NombresCL.PRIMEROSRECURSOS, NombresCL.TOTALRESULTADOSRECURSOS), false);
                                    //InvalidarCache(claveCache.Replace(NombresCL.PRIMEROSRECURSOS, NombresCL.PERFILESCONRECURSOSPRIVADOS), false);
                                    clavesEliminar.Add(claveCache);
                                    clavesEliminar.Add(claveCache.Replace(NombresCL.PRIMEROSRECURSOS.ToLower(), NombresCL.TOTALRESULTADOSRECURSOS.ToLower()));
                                    clavesEliminar.Add(claveCache.Replace(NombresCL.PRIMEROSRECURSOS.ToLower(), NombresCL.PERFILESCONRECURSOSPRIVADOS.ToLower()));
                                    break;
                                case (int)TiposDocumentacion.Debate:
                                    //InvalidarCache(claveCache, false);
                                    //InvalidarCache(claveCache.Replace(NombresCL.PRIMEROSRECURSOS, NombresCL.TOTALRESULTADOSDEBATES), false);
                                    //InvalidarCache(claveCache.Replace(NombresCL.PRIMEROSRECURSOS, NombresCL.PERFILESCONDEBATESPRIVADOS), false);
                                    clavesEliminar.Add(claveCache);
                                    clavesEliminar.Add(claveCache.Replace(NombresCL.PRIMEROSRECURSOS.ToLower(), NombresCL.TOTALRESULTADOSDEBATES.ToLower()));
                                    clavesEliminar.Add(claveCache.Replace(NombresCL.PRIMEROSRECURSOS.ToLower(), NombresCL.PERFILESCONDEBATESPRIVADOS.ToLower()));
                                    break;
                                case (int)TiposDocumentacion.Pregunta:
                                    //InvalidarCache(claveCache, false);
                                    //InvalidarCache(claveCache.Replace(NombresCL.PRIMEROSRECURSOS, NombresCL.TOTALRESULTADOSPREGUNTAS), false);
                                    clavesEliminar.Add(claveCache);
                                    clavesEliminar.Add(claveCache.Replace(NombresCL.PRIMEROSRECURSOS.ToLower(), NombresCL.TOTALRESULTADOSPREGUNTAS.ToLower()));
                                    break;
                            }
                        }
                    }
                }

                InvalidarCachesMultiples(clavesEliminar);
            }

        }

        /// <summary>
        /// Borra la cache de los primeros recursos de los perfiles indicados
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto donde se borrara la cache</param>
        /// <param name="pListaPerfiles">Lista de perfiles a los que se les borrara la cache</param>
        /// <param name="pTipoDocumento">Integer que indica el tipo de documento que se borra de la cache (-1 para recursos)</param>
        public void BorrarPrimerosRecursosDePerfiles(Guid pProyectoID, List<Guid> pListaPerfiles, int pTipoDocumento)
        {
            // Eliminamos la entrada de la cache en la base de datos

            string primeros = NombresCL.PRIMEROSRECURSOS;
            if (pTipoDocumento == (int)TiposDocumentacion.Debate)
            {
                primeros = NombresCL.PRIMEROSDEBATES;
            }
            else if (pTipoDocumento == (int)TiposDocumentacion.Pregunta)
            {
                primeros = NombresCL.PRIMERASPREGUNTAS;
            }

            foreach (Guid perfil in pListaPerfiles)
            {
                string clave = ObtenerClaveCache(string.Concat(primeros, "_", pProyectoID, "_", perfil));
                InvalidarCache(clave, false);
                switch (pTipoDocumento)
                {
                    case -1:
                        InvalidarCache(clave.Replace(NombresCL.PRIMEROSRECURSOS, NombresCL.TOTALRESULTADOSRECURSOS), false);
                        InvalidarCache(clave.Replace(NombresCL.PRIMEROSRECURSOS, NombresCL.PERFILESCONRECURSOSPRIVADOS), false);
                        break;
                    case (int)TiposDocumentacion.Debate:
                        InvalidarCache(clave.Replace(NombresCL.PRIMEROSRECURSOS, NombresCL.TOTALRESULTADOSDEBATES), false);
                        InvalidarCache(clave.Replace(NombresCL.PRIMEROSRECURSOS, NombresCL.PERFILESCONDEBATESPRIVADOS), false);
                        break;
                    case (int)TiposDocumentacion.Pregunta:
                        InvalidarCache(clave.Replace(NombresCL.PRIMEROSRECURSOS, NombresCL.TOTALRESULTADOSPREGUNTAS), false);
                        break;
                }
            }
        }

        #endregion

        #region Lecciones didactalia
        public void AgregarLeccionesDidactalia(string pLeccionesDidactalia)
        {
            string rawKey = "LeccionesDidactalia";

            AgregarObjetoCache(rawKey, pLeccionesDidactalia);
        }

        public string ObtenerLecciones()
        {
            string rawKey = "LeccionesDidactalia";

            return (string)ObtenerObjetoDeCache(rawKey);
        }
        #endregion

        #region MetaDatos
        /// <summary>
        /// Invalida las caches de los metadatos de un recurso
        /// </summary>
        /// <param name="pListaProyectos">Listado de identificadores de los proyectos</param>
        public void BorrarMetasDocumento(Guid pDocumentoID)
        {
            string rawKey = string.Concat("DocumentoMetaDatos_DocumentoID_", pDocumentoID);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Obtiene las caches de los metadatos de un recurso
        /// </summary>
        /// <param name="pListaProyectos">Listado de identificadores de los proyectos</param>
        public AD.EntityModel.Models.Documentacion.DocumentoMetaDatos ObtenerMetaDatos(Guid pDocumentoID)
        {
            string rawKey = string.Concat("DocumentoMetaDatos_DocumentoID_", pDocumentoID);
            AD.EntityModel.Models.Documentacion.DocumentoMetaDatos documentoMetaDatos = ObtenerObjetoDeCache(rawKey) as AD.EntityModel.Models.Documentacion.DocumentoMetaDatos;

            if (documentoMetaDatos == null)
            {
                DocumentacionAD documentacionAD = new DocumentacionAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                documentoMetaDatos = documentacionAD.ObtenerEtiquetasMeta(pDocumentoID);
                if (documentoMetaDatos != null)
                {
                    AgregarMetaDatos(pDocumentoID, documentoMetaDatos);
                }
                documentacionAD.Dispose();
            }

            return documentoMetaDatos;
        }

        /// <summary>
        /// Obtiene las caches de los metadatos de un recurso
        /// </summary>
        /// <param name="pListaProyectos">Listado de identificadores de los proyectos</param>
        public void AgregarMetaDatos(Guid pDocumentoID, AD.EntityModel.Models.Documentacion.DocumentoMetaDatos pDocumentoMetaDatos)
        {
            string rawKey = string.Concat("DocumentoMetaDatos_DocumentoID_", pDocumentoID);
            AgregarObjetoCache(rawKey, pDocumentoMetaDatos);
        }

        #endregion

        #region Recursos relacionados

        /// <summary>
        /// Agrega el HTML de los recursos relacionados de un contexto
        /// </summary>
        /// <param name="pTablaBaseProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pNombreCortoGadget">Identificador del Gadget</param>
        /// <param name="pRecursosRelacionados">HTML de los recursos relacionados</param>
        /// <returns>HTML de los recursos relacionados de un documento</returns>
        public void AgregarRecursosRelacionadosContexto(string pRecursosRelacionados, int pTablaBaseProyectoID, Guid pDocumentoID, string pNombreCortoGadget, string pIdioma)
        {
            string rawKey = string.Concat(NombresCL.RECURSOSRELACIONADOSCONTEXTOS, "_", pTablaBaseProyectoID.ToString(), "_", pDocumentoID.ToString(), "_", pNombreCortoGadget, "_", pIdioma, "_");

            AgregarObjetoCache(rawKey, pRecursosRelacionados);
        }

        /// <summary>
        /// Obtiene el HTML de los recursos relacionados de un contexto
        /// </summary>
        /// <param name="pTablaBaseProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pNombreCortoGadget">Identificador del Gadget</param>
        /// <returns>HTML de los recursos relacionados de un documento</returns>
        public string ObtenerRecursosRelacionadosContexto(int pTablaBaseProyectoID, Guid pDocumentoID, string pNombreCortoGadget, string pIdioma)
        {

            string rawKey = string.Concat(NombresCL.RECURSOSRELACIONADOSCONTEXTOS, "_", pTablaBaseProyectoID.ToString(), "_", pDocumentoID.ToString(), "_", pNombreCortoGadget, "_", pIdioma, "_");

            return (string)ObtenerObjetoDeCache(rawKey);
        }

        /// <summary>
        /// Obtiene los recursos más populares de un proyecto. La clave expira en 1 día.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet donde se cargarán los recursos más populares.</param>
        /// <param name="pProyectoID">Proyecto al que pertenecen los recursos.</param>
        /// <param name="pNumeroResultadosPagina">Numero de recursos que se quieren cargar</param>
        public void ObtenerRecursosPopularesProyecto(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoID, int pNumeroResultadosPagina)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("RecursosPopularesProyecto_", pProyectoID.ToString());

            DataWrapperDocumentacion docDW = (DataWrapperDocumentacion)ObtenerObjetoDeCache(rawKey);
            if (docDW == null)
            {
                docDW = new DataWrapperDocumentacion();
                docDW = DocumentacionCN.ObtenerRecursosPopularesProyecto(pProyectoID, pNumeroResultadosPagina);
                //Duración de la cache: 1 día.
                if (docDW != null)
                {
                    docDW.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, docDW, 86400);
            }
            pDataWrapperDocumentacion.Merge(docDW);
            mEntityContext.UsarEntityCache = false;
        }

        /// <summary>
        /// Agrega el HTML de los recursos relacionados del 1 al 5 de un documento a la cache
        /// </summary>
        /// <param name="pRecursosRelacionados">HTML con los recursos relacionados</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pNumRecursos">Número de recursos relacionados</param>
        public void AgregarRecursosRelacionados(string pRecursosRelacionados, Guid pProyectoID, Guid pDocumentoID, int pNumRecursos, string pIdioma)
        {
            string rawKey = string.Concat(NombresCL.RECURSOSRELACIONADOS, "_", pProyectoID.ToString(), "_", pDocumentoID.ToString(), "_", pIdioma);
            string rawKeyNum = string.Concat(NombresCL.NUMRECURSOSRELACIONADOS, "_", pProyectoID.ToString(), "_", pDocumentoID.ToString(), "_", pIdioma);

            byte[] comprimido = UtilZip.Zip(pRecursosRelacionados);

            AgregarObjetoCache(rawKey, comprimido);
            AgregarObjetoCache(rawKeyNum, pNumRecursos);
        }

        /// <summary>
        /// Obtiene el HTML de los recursos relacionados del 1 al 5 de un documento a la cache
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pNumRecursos">Número de recursos relacionados</param>
        /// <returns>HTML de los recursos relacionados de un documento</returns>
        public string ObtenerRecursosRelacionados(Guid pProyectoID, Guid pDocumentoID, out int pNumRecursos, string pIdioma)
        {
            string rawKey = string.Concat(NombresCL.RECURSOSRELACIONADOS, "_", pProyectoID.ToString(), "_", pDocumentoID.ToString(), "_", pIdioma);
            string rawKeyNum = string.Concat(NombresCL.NUMRECURSOSRELACIONADOS, "_", pProyectoID.ToString(), "_", pDocumentoID.ToString(), "_", pIdioma);

            int? numRecursos = (int?)ObtenerObjetoDeCache(rawKeyNum);
            if (numRecursos.HasValue)
            {
                pNumRecursos = numRecursos.Value;
            }
            else
            {
                pNumRecursos = 0;
            }

            string relacionados = null;
            byte[] comprimido = (byte[])ObtenerObjetoDeCache(rawKey);

            if (comprimido != null)
            {
                relacionados = (string)UtilZip.UnZip(comprimido);
            }

            return relacionados;
        }

        /// <summary>
        /// Borra los recursos relacionados de un recurso en las comunidades que se indica
        /// </summary>
        /// <param name="pListaProyectos"></param>
        /// <param name="pDocumento"></param>
        public void BorrarRecursosRelacionadosYContextos(Guid pDocumentoID)
        {
            #region Recursos relacionados
            string rawKey = ObtenerClaveCache(string.Concat(NombresCL.RECURSOSRELACIONADOS, "_*_", pDocumentoID.ToString(), "*"));
            string rawKeyNum = ObtenerClaveCache(string.Concat(NombresCL.NUMRECURSOSRELACIONADOS, "_*_", pDocumentoID.ToString(), "*"));
            List<string> claves = new List<string>();
            if (ClienteRedisLectura != null)
            {
                claves = ClienteRedisLectura.Keys(rawKey.ToLower()).Result.ToList();
            }
            //InvalidarCachesMultiples(claves);

            if (ClienteRedisLectura != null)
            {
                claves.AddRange(ClienteRedisLectura.Keys(rawKeyNum.ToLower()).Result.ToList());
            }
            //InvalidarCachesMultiples(claves);
            #endregion

            #region Contextos

            string rawKeyContexto = ObtenerClaveCache(string.Concat(NombresCL.RECURSOSRELACIONADOSCONTEXTOS, "_*_", pDocumentoID.ToString(), "_*"));

            if (ClienteRedisLectura != null)
            {
                claves.AddRange(ClienteRedisLectura.Get<List<string>>(rawKeyContexto.ToLower()).Result);
            }

            InvalidarCachesMultiples(claves);
            #endregion
        }

        /// <summary>
        /// Borra los recursos relacionados de un recurso en las comunidades que se indica
        /// </summary>
        /// <param name="pListaProyectos"></param>
        /// <param name="pDocumento"></param>
        public void BorrarContextosRelacionados(Guid pGadgetID)
        {
            #region Contextos

            string rawKeyContexto = ObtenerClaveCache(string.Concat(NombresCL.RECURSOSRELACIONADOSCONTEXTOS, "_*_", pGadgetID.ToString(), "_*"));
            List<string> claves = new List<string>();
            if (ClienteRedisLectura != null)
            {
                claves = ClienteRedisLectura.Keys(rawKeyContexto.ToLower()).Result.ToList();
            }

            InvalidarCachesMultiples(claves);
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        public void BorrarRecursosRelacionadosYContextosPorProyecto(Guid pProyectoID, int pIDBase)
        {
            #region Recursos relacionados
            string rawKey = ObtenerClaveCache(string.Concat(NombresCL.RECURSOSRELACIONADOS, "_", pProyectoID.ToString(), "_", "*"));
            string rawKeyNum = ObtenerClaveCache(string.Concat(NombresCL.NUMRECURSOSRELACIONADOS, "_", pProyectoID.ToString(), "_", "*"));
            List<string> claves = new List<string>();
            if (ClienteRedisLectura != null)
            {
                claves = ClienteRedisLectura.Keys(rawKey.ToLower()).Result.ToList();
            }
            //InvalidarCachesMultiples(claves);

            if (ClienteRedisLectura != null)
            {
                claves.AddRange(ClienteRedisLectura.Keys(rawKeyNum.ToLower()).Result.ToList());
            }

            //InvalidarCachesMultiples(claves);
            #endregion

            #region Contextos

            string rawKeyContexto = ObtenerClaveCache(string.Concat(NombresCL.RECURSOSRELACIONADOSCONTEXTOS, "_", pIDBase.ToString(), "_", "*"));

            if (ClienteRedisLectura != null)
            {
                claves.AddRange(ClienteRedisLectura.Keys(rawKeyContexto.ToLower()).Result.ToList());
            }

            InvalidarCachesMultiples(claves);
            #endregion
        }

        /// <summary>
        /// Obtiene la base de recursos de un determinado proyecto.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pOrganizacionID">Identificador de organziación</param>
        public void ObtenerBaseRecursosProyecto(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoID, Guid pOrganizacionID, Guid pUsuarioID)
        {
            string rawKey = string.Concat("BaseRecursosProyecto_", pProyectoID.ToString());
            mEntityContext.UsarEntityCache = true;
            if (!pOrganizacionID.Equals(Guid.Empty))
            {
                rawKey += "_" + pOrganizacionID;
            }

            DataWrapperDocumentacion dataWrapperDocumentacion = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperDocumentacion;
            if (dataWrapperDocumentacion == null)
            {
                dataWrapperDocumentacion = ObtenerObjetoDeCache(rawKey) as DataWrapperDocumentacion;
                if (dataWrapperDocumentacion != null)
                {
                    dataWrapperDocumentacion.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperDocumentacion);


            }

            if (dataWrapperDocumentacion == null)
            {
                dataWrapperDocumentacion = new DataWrapperDocumentacion();
                if (!pOrganizacionID.Equals(Guid.Empty) && !pUsuarioID.Equals(Guid.Empty))
                {
                    DocumentacionCN.ObtenerBaseRecursosProyecto(dataWrapperDocumentacion, pProyectoID, pOrganizacionID, pUsuarioID);
                }
                else
                {
                    DocumentacionCN.ObtenerBaseRecursosProyecto(dataWrapperDocumentacion, pProyectoID);
                }
                if (dataWrapperDocumentacion != null)
                {
                    dataWrapperDocumentacion.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, dataWrapperDocumentacion);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperDocumentacion);
            }
            pDataWrapperDocumentacion.Merge(dataWrapperDocumentacion);
            mEntityContext.UsarEntityCache = false;
        }


        /// <summary>
        /// Borra los recursos relacionados de los proyectos pasados como parametro
        /// </summary>
        /// <param name="pListaProyectos">Listado de identificadores de los proyectos</param>
        public void BorrarRecursosRelacionados(List<Guid> pListaProyectos)
        {
            //       PARA QUIEN TENGA ALGUNA INTENCIÓN DE DESCOMENTAR ESTO:
            //       En didactalia, el coste de esto es superior a 1 minuto guardando un recurso
            //       (hay más de 20000 recursos, imagínate lo que le puede costar 
            //       borrar todas las claves de caché...tu verás)



            //Para cada proyecto, obtenemos las caches de recursos relacionados que tiene y las invalidamos
            //foreach (Guid proyectoID in pListaProyectos)
            //{
            //    string rawKey = ObtenerClaveCache(string.Concat(NombresCL.RECURSOSRELACIONADOS, "_", proyectoID.ToString()));
            //    string rawKeyNum = ObtenerClaveCache(string.Concat(NombresCL.NUMRECURSOSRELACIONADOS, "_", proyectoID.ToString()));

            //    List<string> claves = ClienteRedisLectura.SearchKeys(rawKey.ToLower() + "*");

            //    foreach (string claveCache in claves)
            //    {
            //        InvalidarCache(claveCache, false);
            //    }

            //    claves = ClienteRedisLectura.SearchKeys(rawKeyNum.ToLower() + "*");

            //    foreach (string claveCache in claves)
            //    {
            //        InvalidarCache(claveCache, false);
            //    }
            //}
        }


        ///// <summary>
        ///// Obtiene la base de recursos de un determinado proyecto.
        ///// </summary>
        ///// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        ///// <param name="pProyectoID">Identificador de proyecto</param>
        //public void ObtenerBaseRecursosProyecto(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pProyectoID)
        //{
        //    ObtenerBaseRecursosProyecto(pDataWrapperDocumentacion, pProyectoID, Guid.Empty);
        //}

        /// <summary>
        /// Obtiene la base de recursos de una organización.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        public void ObtenerBaseRecursosOrganizacion(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pOrganizacionID, Guid pProyectoID)
        {
            string rawKey = string.Concat("BaseRecursosOrganizacion_", pOrganizacionID.ToString());
            mEntityContext.UsarEntityCache = true;
            DataWrapperDocumentacion dataWrapperDocumentacion = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperDocumentacion;
            if (dataWrapperDocumentacion == null)
            {
                dataWrapperDocumentacion = (DataWrapperDocumentacion)ObtenerObjetoDeCache(rawKey);
                if (dataWrapperDocumentacion != null)
                {
                    dataWrapperDocumentacion.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperDocumentacion);
            }

            if (dataWrapperDocumentacion == null)
            {
                dataWrapperDocumentacion = new DataWrapperDocumentacion();
                DocumentacionCN.ObtenerBaseRecursosOrganizacion(dataWrapperDocumentacion, pOrganizacionID);
                if (dataWrapperDocumentacion != null)
                {
                    dataWrapperDocumentacion.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, dataWrapperDocumentacion);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperDocumentacion);
            }
            mEntityContext.UsarEntityCache = false;
            pDataWrapperDocumentacion.Merge(dataWrapperDocumentacion);
        }

        /// <summary>
        /// Obtiene la base de recursos de un usuario.
        /// </summary>
        /// <param name="pDataWrapperDocumentacion">DataSet de documentación</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        public void ObtenerBaseRecursosUsuario(DataWrapperDocumentacion pDataWrapperDocumentacion, Guid pUsuarioID)
        {
            string rawKey = string.Concat("BaseRecursosUsuario_", pUsuarioID.ToString());
            mEntityContext.UsarEntityCache = true;
            DataWrapperDocumentacion dataWrapperDocumentacion = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperDocumentacion;
            if (dataWrapperDocumentacion == null)
            {
                dataWrapperDocumentacion = (DataWrapperDocumentacion)ObtenerObjetoDeCache(rawKey);
                if (dataWrapperDocumentacion != null)
                {
                    dataWrapperDocumentacion.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCacheLocal(Guid.Empty, rawKey, dataWrapperDocumentacion);
            }

            if (dataWrapperDocumentacion == null)
            {
                dataWrapperDocumentacion = new DataWrapperDocumentacion();
                DocumentacionCN.ObtenerBaseRecursosUsuario(dataWrapperDocumentacion, pUsuarioID);
                if (dataWrapperDocumentacion != null)
                {
                    dataWrapperDocumentacion.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, dataWrapperDocumentacion);
                AgregarObjetoCacheLocal(Guid.Empty, rawKey, dataWrapperDocumentacion);
            }
            mEntityContext.UsarEntityCache = false;
            pDataWrapperDocumentacion.Merge(dataWrapperDocumentacion);
        }

        #endregion

        #region Que se esta leyendo

        /// <summary>
        /// Agrega el HTML de los recursos más populares de la comunidad a la cache
        /// </summary>
        /// <param name="pContenidoQueEstaPasando">HTML con los recursos más populares</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void AgregarQueSeEstaLeyendo(string pContenidoQueSeEstaLeyendo, Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.QUESEESTALEYENDO, "_", pProyectoID.ToString());

            AgregarObjetoCache(rawKey, pContenidoQueSeEstaLeyendo);
        }

        /// <summary>
        /// Obtiene el HTML de los recursos más populares de una comunidad desde la cache
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>HTML de los recursos más populares de una comunidad</returns>
        public string ObtenerQueSeEstaLeyendo(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.QUESEESTALEYENDO, "_", pProyectoID.ToString());
            return (string)ObtenerObjetoDeCache(rawKey);
        }

        #endregion

        #region Form Sem

        /// <summary>
        /// Carga las propiedades y el mapeo de las categorias del tesauro semántico desde el xml de configuración
        /// </summary>
        public void CargarPropiedadesYMappingCategoriasTesauroSemantico(Guid pProyectoID, Dictionary<string, List<Guid>> pDiccionarioMapeoCategorias, List<List<string>> pListaMapeoPropiedadesEntidades, byte[] pArrayMapeo)
        {
            //Dictionary<string, List<Guid>> diccionarioProvisional = ObtenerDiccionarioMapeoCategoriasTesauroSemantico(pProyectoID);
            //List<List<string>> listaProvisional = ObtenerListaMapeoPropiedadesEntidades(pProyectoID);

            //if (diccionarioProvisional != null)
            //{
            //    foreach (string clave in diccionarioProvisional.Keys)
            //    {
            //        if (!pDiccionarioMapeoCategorias.ContainsKey(clave))
            //        {
            //            pDiccionarioMapeoCategorias.Add(clave, diccionarioProvisional[clave]);
            //        }
            //    }
            //}

            //if (listaProvisional != null)
            //{
            //    pListaMapeoPropiedadesEntidades.AddRange(listaProvisional);
            //}

            //if (pDiccionarioMapeoCategorias == null || pListaMapeoPropiedadesEntidades == null)
            //{
            try
            {
                //pListaMapeoPropiedadesEntidades = new List<List<string>>();
                //pDiccionarioMapeoCategorias = new Dictionary<string, List<Guid>>();

                //leer el xml con el mapeo de las categorías
                XmlDocument configXML = new XmlDocument();
                //antes se cargaba desde una ruta y ahora se lee el byte[]
                //configXML.Load(pUrlMappingCategorias);

                MemoryStream ms = new MemoryStream(pArrayMapeo);
                configXML.Load(ms);

                //carga de propiedades
                XmlNodeList bloquesPropiedades = configXML.SelectNodes("MappingsProyecto/Propiedades");
                foreach (XmlNode nodoPropiedades in bloquesPropiedades)
                {
                    XmlNodeList nodosInternoPropiedad = nodoPropiedades.SelectNodes("Propiedad");
                    List<string> listaPropEntidad = new List<string>();

                    foreach (XmlNode nodoPropiedad in nodosInternoPropiedad)
                    {
                        //obtener el nombre de la propiedad
                        XmlNode nodoNombrePropiedad = nodoPropiedad.SelectSingleNode("NombrePropiedad");
                        string nombrePropiedad = nodoNombrePropiedad.InnerText;

                        //obtener el tipo de entidad
                        XmlNode nodoTipoEntidad = nodoPropiedad.SelectSingleNode("TipoEntidad");
                        string tipoEntidad = nodoTipoEntidad.InnerText;

                        listaPropEntidad.Add(nombrePropiedad);
                        listaPropEntidad.Add(tipoEntidad);
                    }

                    pListaMapeoPropiedadesEntidades.Add(listaPropEntidad);
                }

                //carga de categorías
                XmlNodeList mapeos = configXML.SelectNodes("/MappingsProyecto/Mapping");
                foreach (XmlNode mapeo in mapeos)
                {
                    //obtener del xml la categoría origen
                    XmlNode nodoCatOrigen = mapeo.SelectSingleNode("CategoriaOrigen");
                    string catOrigen = nodoCatOrigen.InnerText;

                    //obtener del xml la lista de categorías destino
                    XmlNodeList nodosCatsDestino = mapeo.SelectNodes("CategoriaDestino");
                    List<Guid> categoriasDestino = new List<Guid>();
                    foreach (XmlNode nodoCatDestino in nodosCatsDestino)
                    {
                        Guid catDestinoID = new Guid(nodoCatDestino.InnerText);

                        if (!categoriasDestino.Contains(catDestinoID))
                        {
                            categoriasDestino.Add(catDestinoID);
                        }
                    }
                    if (!pDiccionarioMapeoCategorias.ContainsKey(catOrigen))
                    {
                        pDiccionarioMapeoCategorias.Add(catOrigen, categoriasDestino);
                    }
                }

                if (pDiccionarioMapeoCategorias.Count > 0)
                {
                    GuardarDiccionarioMapeoCategoriasTesauroSemantico(pProyectoID, pDiccionarioMapeoCategorias);
                    GuardarListaMapeoPropiedadesEntidades(pProyectoID, pListaMapeoPropiedadesEntidades);
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //GuardarLogError("Error al cargar el mapeo del tesauro semántico: " + ex.StackTrace);
            }
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pListaPropiedadesEntidades"></param>
        /// <param name="pListaEntidades"></param>
        /// <param name="pListaCategoriasTesauroSemantico"></param>
        /// <param name="pDiccionarioMapeoCategorias"></param>
        private void CargarCategoriasDesdePropiedadesSemanticas(List<string> pListaPropiedadesEntidades, List<ElementoOntologia> pListaEntidades, List<Guid> pListaCategoriasTesauroSemantico, Dictionary<string, List<Guid>> pDiccionarioMapeoCategorias)
        {
            string nombrePropiedad = pListaPropiedadesEntidades[0];
            string tipoEntidad = pListaPropiedadesEntidades[1];

            Propiedad propiedadPadre = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(nombrePropiedad, tipoEntidad, pListaEntidades);
            List<string> listaParaBorrar = new List<string>();

            if (pListaPropiedadesEntidades.Count > 2)
            {
                //para hacer el remove hacemos una compia de la lista para no perder las originales
                listaParaBorrar.AddRange(pListaPropiedadesEntidades);

                //se elimina la primera pareja, como Remove recalcula la lista borramos de nuevo la posicion 0
                listaParaBorrar.RemoveAt(0);
                listaParaBorrar.RemoveAt(0);

                foreach (ElementoOntologia entidad in propiedadPadre.ListaValores.Values)
                {
                    List<ElementoOntologia> listaEntidades = new List<ElementoOntologia>();
                    listaEntidades.Add(entidad);
                    CargarCategoriasDesdePropiedadesSemanticas(listaParaBorrar, listaEntidades, pListaCategoriasTesauroSemantico, pDiccionarioMapeoCategorias);
                }
            }
            else if (pListaPropiedadesEntidades.Count == 2)
            {
                List<string> lista = new List<string>(propiedadPadre.ListaValores.Keys);
                lista.Reverse();
                foreach (string valor in lista)
                {
                    if (pDiccionarioMapeoCategorias.ContainsKey(valor))
                    {
                        foreach (Guid catDestinoID in pDiccionarioMapeoCategorias[valor])
                        {
                            if (!pListaCategoriasTesauroSemantico.Contains(catDestinoID))
                            {
                                pListaCategoriasTesauroSemantico.Add(catDestinoID);
                            }
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Carga las propiedades y el mapeo de las categorias del tesauro semántico desde el xml de configuración
        /// </summary>
        public MapeoTesauroComunidad CargarPropiedadesYMappingCategoriasTesauroComunidad(Guid pProyectoID, string pUrlMappingCategorias, byte[] pArrayMapeo)
        {
            MapeoTesauroComunidad mapeoTesauroComunidad = ObtenerDeCacheMapeoCategoriasTesauroComunidad(pProyectoID);

            if (mapeoTesauroComunidad == null || mapeoTesauroComunidad.Bloques.Count == 0)
            {
                try
                {
                    //leer el xml con el mapeo de las categorías
                    XmlDocument configXML = new XmlDocument();
                    //antes se cargaba desde una ruta y ahora se lee el byte[]
                    //configXML.Load(pUrlMappingCategorias);

                    MemoryStream ms = new MemoryStream(pArrayMapeo);
                    configXML.Load(ms);

                    mapeoTesauroComunidad = new MapeoTesauroComunidad();
                    //carga de los bloques
                    List<BloqueMapeo> listaBloques = new List<BloqueMapeo>();
                    XmlNodeList listaNodosBloque = configXML.SelectNodes("MappingsProyecto/Bloque");

                    foreach (XmlNode nodoBloque in listaNodosBloque)
                    {
                        BloqueMapeo bloque = new BloqueMapeo();
                        //carga de propiedades
                        List<PropiedadMapeo> listaPropiedades = new List<PropiedadMapeo>();
                        XmlNodeList nodosPropiedad = nodoBloque.SelectNodes("Propiedades/Propiedad");

                        foreach (XmlNode nodoPropiedad in nodosPropiedad)
                        {
                            PropiedadMapeo propiedad = new PropiedadMapeo();
                            //obtener el nombre de la propiedad
                            XmlNode nodoNombrePropiedad = nodoPropiedad.SelectSingleNode("NombrePropiedad");
                            propiedad.NombrePropiedad = nodoNombrePropiedad.InnerText;

                            //obtener el tipo de entidad
                            XmlNode nodoTipoEntidad = nodoPropiedad.SelectSingleNode("TipoEntidad");
                            propiedad.TipoEntidad = nodoTipoEntidad.InnerText;

                            listaPropiedades.Add(propiedad);
                        }

                        List<Mapping> listaMappingsBloque = new List<Mapping>();
                        //carga de categorías
                        XmlNodeList nodosMapping = nodoBloque.SelectNodes("Mapping");

                        foreach (XmlNode nodoMapping in nodosMapping)
                        {
                            Mapping mapeo = new Mapping();
                            List<Guid> listaCategoriasMapping = new List<Guid>();
                            List<Rama> listaRamas = new List<Rama>();

                            //obtener del xml la categoría origen
                            XmlNodeList nodosCatOrigen = nodoMapping.SelectNodes("CategoriaOrigen");
                            foreach (XmlNode nodoCatOrigen in nodosCatOrigen)
                            {
                                Guid categoriaID = new Guid(nodoCatOrigen.InnerText);

                                if (!listaCategoriasMapping.Contains(categoriaID))
                                {
                                    listaCategoriasMapping.Add(categoriaID);
                                }
                            }

                            mapeo.CategoriasOrigen.AddRange(listaCategoriasMapping);

                            //obtener las categorias destino de las ramas de ese nomo mapeo
                            XmlNodeList nodosRamas = nodoMapping.SelectNodes("Rama");
                            foreach (XmlNode nodoRama in nodosRamas)
                            {
                                Rama rama = new Rama();
                                List<string> listaCategoriasDestino = new List<string>();

                                //obtener del xml la lista de categorías destino
                                XmlNodeList nodosCatsDestino = nodoRama.SelectNodes("CategoriaDestino");

                                foreach (XmlNode nodoCatDestino in nodosCatsDestino)
                                {
                                    if (!listaCategoriasDestino.Contains(nodoCatDestino.InnerText))
                                    {
                                        listaCategoriasDestino.Add(nodoCatDestino.InnerText);
                                    }
                                }

                                rama.CategoriasDestino.AddRange(listaCategoriasDestino);
                                listaRamas.Add(rama);
                            }

                            mapeo.Ramas.AddRange(listaRamas);
                            listaMappingsBloque.Add(mapeo);
                        }

                        bloque.ListaMapping.AddRange(listaMappingsBloque);
                        bloque.ListaPropiedades.AddRange(listaPropiedades);
                        bloque.MontarDiccionarioBloqueMapeo();
                        listaBloques.Add(bloque);
                    }

                    mapeoTesauroComunidad.Bloques.AddRange(listaBloques);

                    if (mapeoTesauroComunidad.Bloques.Count > 0)
                    {
                        GuardarEnCacheMapeoCategoriasTesauroComunidad(pProyectoID, mapeoTesauroComunidad);
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                    //GuardarLogError("Error al cargar el mapeo del tesauro semántico: " + ex.StackTrace);
                }
            }

            return mapeoTesauroComunidad;
        }

        /// <summary>
        /// Mapea las categorías del tesauro de la comunidad con las categorías del tesauro semántico
        /// </summary>
        public List<string> MapearCategoriasTesauroComunidad(List<Guid> pListaCategoriasRecurso, Guid pProyectoID, string pUrlMappingCategorias, string pSujeto, byte[] pArrayMapeo)
        {
            List<string> listaTriples = new List<string>();

            MapeoTesauroComunidad mapeoTesauroComunidad = CargarPropiedadesYMappingCategoriasTesauroComunidad(pProyectoID, pUrlMappingCategorias, pArrayMapeo);


            if (mapeoTesauroComunidad != null && mapeoTesauroComunidad.Bloques.Count > 0)
            {
                //calcular los triples del tesauro semántico
                listaTriples.AddRange(CalcularTriplesCategoriasTesauroComunidad(pListaCategoriasRecurso, pSujeto, mapeoTesauroComunidad));
            }

            return listaTriples;
        }

        private List<string> CalcularTriplesCategoriasTesauroComunidad(List<Guid> pListaCategoriasRecurso, string pSujeto, MapeoTesauroComunidad pMapeoTesauroComunidad)
        {
            List<string> listaTriples = new List<string>();

            //odenamos la lista de categorias del recurso
            SortedList<string, string> listaOrdCategoriasRecurso = new SortedList<string, string>();
            foreach (Guid categoriaID in pListaCategoriasRecurso)
            {
                listaOrdCategoriasRecurso.Add(categoriaID.ToString(), null);
            }
            foreach (BloqueMapeo bloque in pMapeoTesauroComunidad.Bloques)
            {
                List<Mapping> listaMapeos = BuscarCategoriasID(listaOrdCategoriasRecurso, bloque);

                foreach (Mapping mapeo in listaMapeos)
                {
                    foreach (Rama rama in mapeo.Ramas)
                    {
                        string siguienteSujeto = pSujeto;

                        for (int i = 0; i < bloque.ListaPropiedades.Count; i++)
                        {
                            string predicado = "<" + bloque.ListaPropiedades[i].NombrePropiedad + ">";

                            if (i < bloque.ListaPropiedades.Count - 1)
                            {
                                string sujetoActual = siguienteSujeto;
                                //sujeto auxiliar
                                siguienteSujeto = (pSujeto.Substring(0, pSujeto.Length - 1) + "_aux_" + Guid.NewGuid() + ">").ToLower();
                                listaTriples.Add(FacetadoAD.GenerarTripleta(sujetoActual, predicado, siguienteSujeto));
                            }
                            else
                            {
                                foreach (string categoriaDestino in rama.CategoriasDestino)
                                {
                                    string objeto = ("<" + categoriaDestino + ">").ToLower();
                                    listaTriples.Add(FacetadoAD.GenerarTripleta(siguienteSujeto, predicado, objeto));
                                }
                            }
                        }
                    }
                }
            }

            return listaTriples;
        }

        private List<Mapping> BuscarCategoriasID(SortedList<string, string> pListaOrdCategoriasRecurso, BloqueMapeo pBloqueMapeo)
        {
            List<string> categoriasDescartadasBusqueda = new List<string>();
            List<Mapping> listaMapeos = new List<Mapping>();
            List<string> listaOrdCategoriaRecurso = new List<string>(pListaOrdCategoriasRecurso.Keys);

            //1º buscar parejas de categorias del recurso en el mapeo de la comunidad
            for (int i = 0; i < listaOrdCategoriaRecurso.Count - 1; i++)
            {
                for (int j = i + 1; j < listaOrdCategoriaRecurso.Count; j++)
                {
                    string cadenaBuscar = listaOrdCategoriaRecurso[i] + "," + listaOrdCategoriaRecurso[j];

                    if (pBloqueMapeo.DiccionarioMappingRamas.ContainsKey(cadenaBuscar))
                    {
                        if (!listaMapeos.Contains(pBloqueMapeo.DiccionarioMappingRamas[cadenaBuscar]))
                        {
                            listaMapeos.Add(pBloqueMapeo.DiccionarioMappingRamas[cadenaBuscar]);

                            if (!categoriasDescartadasBusqueda.Contains(listaOrdCategoriaRecurso[i]))
                            {
                                categoriasDescartadasBusqueda.Add(listaOrdCategoriaRecurso[i]);
                            }

                            if (!categoriasDescartadasBusqueda.Contains(listaOrdCategoriaRecurso[j]))
                            {
                                categoriasDescartadasBusqueda.Add(listaOrdCategoriaRecurso[j]);
                            }
                        }
                    }
                }
            }

            foreach (string categoriaID in listaOrdCategoriaRecurso)
            {
                if (!categoriasDescartadasBusqueda.Contains(categoriaID))
                {
                    if (pBloqueMapeo.DiccionarioMappingRamas.ContainsKey(categoriaID))
                    {
                        if (!listaMapeos.Contains(pBloqueMapeo.DiccionarioMappingRamas[categoriaID]))
                        {
                            listaMapeos.Add(pBloqueMapeo.DiccionarioMappingRamas[categoriaID]);
                        }
                    }
                }
            }

            return listaMapeos;
        }

        /// <summary>
        /// Mapea las categorías del tesauro semántico con las categorías del tesauro de la comunidad
        /// </summary>
        public List<Guid> MapearCategoriasTesauroSemantico(List<ElementoOntologia> pListaEntidades, Guid pProyectoID, Dictionary<string, List<Guid>> pDiccionarioMapeoCategorias, List<List<string>> pListaMapeoPropiedadesEntidades, byte[] pArrayMapeo)
        {
            List<Guid> listaCategoriasMapeadas = null;

            if (pDiccionarioMapeoCategorias == null || pListaMapeoPropiedadesEntidades == null)
            {
                DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                docCL.CargarPropiedadesYMappingCategoriasTesauroSemantico(pProyectoID, pDiccionarioMapeoCategorias, pListaMapeoPropiedadesEntidades, pArrayMapeo);
                docCL.Dispose();
            }

            if (pDiccionarioMapeoCategorias != null && pListaMapeoPropiedadesEntidades != null && pDiccionarioMapeoCategorias.Count > 0 && pListaMapeoPropiedadesEntidades.Count > 0)
            {
                listaCategoriasMapeadas = new List<Guid>();

                foreach (List<string> listaPropiedades in pListaMapeoPropiedadesEntidades)
                {
                    CargarCategoriasDesdePropiedadesSemanticas(listaPropiedades, pListaEntidades, listaCategoriasMapeadas, pDiccionarioMapeoCategorias);
                }
            }

            return listaCategoriasMapeadas;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public Dictionary<string, List<Guid>> ObtenerDiccionarioMapeoCategoriasTesauroSemantico(Guid pProyectoID)
        {
            string rawKey = string.Concat("MapeoCategoriasTesauroSemantico", "_", pProyectoID.ToString());
            Dictionary<string, List<Guid>> diccionarioMapeo = (Dictionary<string, List<Guid>>)ObtenerObjetoDeCache(rawKey);
            return diccionarioMapeo;
        }

        /// <summary>
        /// Guarda el array de una ontología.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public void GuardarDiccionarioMapeoCategoriasTesauroSemantico(Guid pProyectoID, Dictionary<string, List<Guid>> pDiccionarioMapeo)
        {
            string rawKey = string.Concat("MapeoCategoriasTesauroSemantico", "_", pProyectoID.ToString());

            if (pDiccionarioMapeo != null)
            {
                AgregarObjetoCache(rawKey, pDiccionarioMapeo);
            }
            else
            {
                InvalidarCache(rawKey, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public MapeoTesauroComunidad ObtenerDeCacheMapeoCategoriasTesauroComunidad(Guid pProyectoID)
        {
            string rawKey = string.Concat("MapeoCategoriasTesauroComunidad", "_", pProyectoID.ToString());
            MapeoTesauroComunidad mapeoTesauroComunidad = (MapeoTesauroComunidad)ObtenerObjetoDeCache(rawKey);
            return mapeoTesauroComunidad;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public void GuardarEnCacheMapeoCategoriasTesauroComunidad(Guid pProyectoID, MapeoTesauroComunidad pMapeoTesauroComunidad)
        {
            string rawKey = string.Concat("MapeoCategoriasTesauroComunidad", "_", pProyectoID.ToString());

            if (pMapeoTesauroComunidad != null)
            {
                AgregarObjetoCache(rawKey, pMapeoTesauroComunidad);
            }
            else
            {
                InvalidarCache(rawKey, true);
            }
        }

        public List<List<string>> ObtenerListaMapeoPropiedadesEntidades(Guid pProyectoID)
        {
            string rawKey = string.Concat("MapeoPropiedadesEntidades", "_", pProyectoID.ToString());
            List<List<string>> listaPropiedadesEntidades = (List<List<string>>)ObtenerObjetoDeCache(rawKey);
            return listaPropiedadesEntidades;
        }

        /// <summary>
        /// Guarda el array de una ontología.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public void GuardarListaMapeoPropiedadesEntidades(Guid pProyectoID, List<List<string>> pListaPropiedadesEntidades)
        {
            string rawKey = string.Concat("MapeoPropiedadesEntidades", "_", pProyectoID.ToString());

            if (pListaPropiedadesEntidades != null)
            {
                AgregarObjetoCache(rawKey, pListaPropiedadesEntidades);
            }
            else
            {
                InvalidarCache(rawKey, true);
            }
        }

        /// <summary>
        /// Array del mapeo del tesauro de la comunidad.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <param name="pNombreDocumento">Nombre del documento de mapeo se va a almacenar(semántico o comunidad)</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public byte[] ObtenerDocumentoMapeoTesauro(Guid pProyectoID, string pNombreDocumento)
        {
            string rawKey = string.Concat(pNombreDocumento, "_", pProyectoID.ToString());
            byte[] onto = (byte[])ObtenerObjetoDeCache(rawKey);
            return onto;
        }

        /// <summary>
        /// Guarda el array del mapeo de la comunidad o el semántico.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <param name="pNombreDocumento">Nombre del documento de mapeo se va a almacenar(semántico o comunidad)</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public void GuardarDocumentoMapeoTesauro(Guid pProyectoID, byte[] pMapping, string pNombreDocumento)
        {
            string rawKey = string.Concat(pNombreDocumento, "_", pProyectoID.ToString());

            if (pMapping != null)
            {
                AgregarObjetoCache(rawKey, pMapping);
            }
            else
            {
                InvalidarCache(rawKey, true);
            }
        }

        /// <summary>
        /// Array de una ontología.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public byte[] ObtenerOntologia(Guid pDocumentoID)
        {
            string rawKey = string.Concat("OntologiaDocSem", "_", pDocumentoID.ToString());
            byte[] onto = (byte[])ObtenerObjetoDeCache(rawKey);
            return onto;
        }

        /// <summary>
        /// Guarda el array de una ontología.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public void GuardarOntologia(Guid pDocumentoID, byte[] pOntologia)
        {
            string rawKey = string.Concat("OntologiaDocSem", "_", pDocumentoID.ToString());

            if (pOntologia != null)
            {
                AgregarObjetoCache(rawKey, pOntologia);
            }
            else
            {
                InvalidarCache(rawKey, true);
            }
        }

        /// <summary>
        /// ID del Xml de una ontología.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public Guid? ObtenerIDXmlOntologia(Guid pDocumentoID)
        {
            string rawKey = string.Concat("XmlOntologiaDocSem", "_", pDocumentoID.ToString());
            Guid? xmlID = (Guid?)ObtenerObjetoDeCache(rawKey);
            return xmlID;
        }

        /// <summary>
        /// Guarda el ID del Xml de una ontología.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento de la ontología</param>
        /// <param name="pXmlID">ID del xml</param>
        /// <returns>ruta del fichero de ontología descargado</returns>
        public void GuardarIDXmlOntologia(Guid pDocumentoID, Guid pXmlID)
        {
            string rawKey = string.Concat("XmlOntologiaDocSem", "_", pDocumentoID.ToString());
            AgregarObjetoCache(rawKey, pXmlID);
        }

        private void ModificarDataWrapperComunidadHijaNivelesCertificacion(Guid pProyectoID, DataWrapperDocumentacion dataWrapperDocumento)
        {
            foreach (AD.EntityModel.Models.Documentacion.Documento documento in dataWrapperDocumento.ListaDocumento)
            {
                documento.ProyectoID = pProyectoID;
            }
        }

        #endregion

        #region Cache Controles recursos

        public void ActualizarCaducidadControlesCache(Guid pProyectoID, Guid pDocumentoID)
        {
            string claveRec = ObtenerClaveCache(ClaveControlFichaRecursos(pProyectoID, pDocumentoID));
            string claveComen = ObtenerClaveCache(ClaveControlComentariosFichaRecursos(pProyectoID, pDocumentoID));
            string claveVin = ObtenerClaveCache(ClaveControlVinculadosFichaRecursos(pProyectoID, pDocumentoID));

            double duracionCache = 60 * 60 * 24 * 30;

            AgregarCaducidadAObjetoCache(claveRec, duracionCache);
            AgregarCaducidadAObjetoCache(claveComen, duracionCache);
            AgregarCaducidadAObjetoCache(claveVin, duracionCache);
        }

        /// <summary>
        /// Devuelve los HTML de todos los controles de la ficha de un recurso.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>HTML de todos los controles de la ficha de un recurso</returns>
        public string[] ObtenerControlesRecurso(Guid pProyectoID, Guid pDocumentoID)
        {
            string claveRec = ObtenerClaveCache(ClaveControlFichaRecursos(pProyectoID, pDocumentoID)).ToLower();
            string claveComen = ObtenerClaveCache(ClaveControlComentariosFichaRecursos(pProyectoID, pDocumentoID)).ToLower();
            string claveVin = ObtenerClaveCache(ClaveControlVinculadosFichaRecursos(pProyectoID, pDocumentoID)).ToLower();

            string[] array = new string[] { claveRec, claveComen, claveVin };
            Dictionary<string, object> objetos = ObtenerListaObjetosCache(array, typeof(object));
            if (objetos != null)
            {
                string[] controles = new string[3];

                foreach (string key in objetos.Keys)
                {
                    if (key == claveRec)
                    {
                        controles[0] = (string)objetos[key];
                    }
                    else if (key == claveComen)
                    {
                        controles[1] = (string)objetos[key];
                    }
                    else if (key == claveVin)
                    {
                        controles[2] = (string)objetos[key];
                    }
                }

                return controles;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Devuelve la calve del HTML de la ficha de un recurso.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Calve del HTML de la ficha de un recurso</returns>
        public string ClaveControlFichaRecursos(Guid pProyectoID, Guid pDocumentoID)
        {
            return string.Concat("ControlFichaRec", "_", pDocumentoID.ToString(), "_", pProyectoID.ToString());
        }

        /// <summary>
        /// Agrega el HTML de la ficha de un recurso.
        /// </summary>
        /// <param name="pHtml">Html de la ficha del recurso</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void AgregarControlFichaRecursos(string pHtml, Guid pProyectoID, Guid pDocumentoID)
        {
            string rawKey = ClaveControlFichaRecursos(pProyectoID, pDocumentoID);
            AgregarObjetoCache(rawKey, pHtml, DuracionCacheControlRecursos);
        }

        /// <summary>
        /// Obtiene el HTML de la ficha de un recurso.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Html de la ficha del recurso</returns>
        public string ObtenerControlFichaRecursos(Guid pProyectoID, Guid pDocumentoID)
        {
            string rawKey = ClaveControlFichaRecursos(pProyectoID, pDocumentoID);

            string html = (string)ObtenerObjetoDeCache(rawKey);

            return html;
        }

        /// <summary>
        /// Borra el HTML de la ficha de un recurso.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void BorrarControlFichaRecursos(Guid pDocumentoID)
        {
            string rawKey = ObtenerClaveCache(string.Concat(NombresCL.FICHARECURSOMVC, "_", pDocumentoID.ToString(), "_"));

            List<string> claves = new List<string>();
            if (ClienteRedisLectura != null)
            {
                claves = ClienteRedisLectura.Keys(rawKey.ToLower() + "*").Result.ToList();
            }

            InvalidarCachesMultiples(claves);
        }

        /// <summary>
        /// Borra un elemento de la cache de recursos.
        /// </summary>
        /// <param name="clave">Clave a borrar</param>
        public void BorrarRecurso(string clave)
        {
            if (ClienteRedisLectura != null)
            {
                ClienteRedisLectura.Del(clave);
            }
        }

        /// <summary>
        /// Obtiene todos los elemento de la cache de recursos.
        /// </summary>
        public string[] ObtenerRecursos(string filtro)
        {
            return ClienteRedisLectura.Keys(filtro).Result;
        }

        /// <summary>
        /// Devuelve la calve del HTML de los comentarios de la ficha de un recurso.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Calve del HTML de los comentarios de la ficha de un recurso</returns>
        public string ClaveControlComentariosFichaRecursos(Guid pProyectoID, Guid pDocumentoID)
        {
            return string.Concat("ControlComentFichaRec", "_", pDocumentoID.ToString(), "_", pProyectoID.ToString());
        }

        /// <summary>
        /// Agrega el HTML de los comentarios de la ficha de un recurso.
        /// </summary>
        /// <param name="pHtml">Html de la ficha del recurso</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void AgregarControlComentariosFichaRecursos(string pHtml, Guid pProyectoID, Guid pDocumentoID)
        {
            string rawKey = ClaveControlComentariosFichaRecursos(pProyectoID, pDocumentoID);
            AgregarObjetoCache(rawKey, pHtml, DuracionCacheControlRecursos);
        }

        /// <summary>
        /// Obtiene el HTML de los comentarios de la ficha de un recurso.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Html de los comentarios de la ficha de un recurso</returns>
        public string ObtenerControlComentariosFichaRecursos(Guid pProyectoID, Guid pDocumentoID)
        {
            string rawKey = ClaveControlComentariosFichaRecursos(pProyectoID, pDocumentoID);

            string html = (string)ObtenerObjetoDeCache(rawKey);

            return html;
        }

        /// <summary>
        /// Borra el HTML de los comentarios de la ficha de un recurso.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void BorrarControlComentariosFichaRecursos(Guid pDocumentoID)
        {
            string rawKey = ObtenerClaveCache(string.Concat("ControlComentFichaRec", "_", pDocumentoID.ToString(), "_"));

            List<string> claves = new List<string>();
            if (ClienteRedisLectura != null)
            {
                claves = ClienteRedisLectura.Keys(rawKey.ToLower() + "*").Result.ToList();
            }

            InvalidarCachesMultiples(claves);
        }

        /// <summary>
        /// Devuelve la calve del HTML de los vinculados de la ficha de un recurso.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Calve del HTML de los vinculados de la ficha de un recurso</returns>
        public string ClaveControlVinculadosFichaRecursos(Guid pProyectoID, Guid pDocumentoID)
        {
            return string.Concat("ControlVinculadosFichaRec", "_", pDocumentoID.ToString(), "_", pProyectoID.ToString());
        }

        /// <summary>
        /// Agrega el HTML de los vinculados de la ficha de un recurso.
        /// </summary>
        /// <param name="pHtml">Html de la ficha del recurso</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void AgregarControlVinculadosFichaRecursos(string pHtml, Guid pProyectoID, Guid pDocumentoID)
        {
            string rawKey = ClaveControlVinculadosFichaRecursos(pProyectoID, pDocumentoID);
            AgregarObjetoCache(rawKey, pHtml, DuracionCacheControlRecursos);
        }

        /// <summary>
        /// Obtiene el HTML de los vinculados de la ficha de un recurso.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>Html de los vinculados de la ficha de un recurso</returns>
        public string ObtenerControlVinculadosFichaRecursos(Guid pProyectoID, Guid pDocumentoID)
        {
            string rawKey = ClaveControlVinculadosFichaRecursos(pProyectoID, pDocumentoID);

            string html = (string)ObtenerObjetoDeCache(rawKey);

            return html;
        }

        /// <summary>
        /// Borra el HTML de los vinculados de la ficha de un recurso.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void BorrarControlVinculadosFichaRecursos(Guid pDocumentoID)
        {
            string rawKey = ObtenerClaveCache(string.Concat("ControlVinculadosFichaRec", "_", pDocumentoID.ToString(), "_"));

            List<string> claves = new List<string>();
            if (ClienteRedisLectura != null)
            {
                claves = ClienteRedisLectura.Keys(rawKey.ToLower() + "*").Result.ToList();
            }

            InvalidarCachesMultiples(claves);
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Clase de negocio
        /// </summary>
        protected DocumentacionCN DocumentacionCN
        {
            get
            {
                if (mDocumentacionCN == null)
                {
                    if (mFicheroConfiguracionBD != null && mFicheroConfiguracionBD != "")
                    {
                        mDocumentacionCN = new DocumentacionCN(mFicheroConfiguracionBD, true, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    }
                    else
                    {
                        mDocumentacionCN = new DocumentacionCN(true, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    }
                }

                return mDocumentacionCN;
            }
        }

        /// <summary>
        /// Clave para la caché
        /// </summary>
        public override string[] ClaveCache
        {
            get
            {
                return mMasterCacheKeyArray;
            }
        }

        /// <summary>
        /// Dominio sobre el que se genera la cache
        /// </summary>
        public override string Dominio
        {
            get
            {
                return mDominio;
            }
            set
            {
                mDominio = value;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~DocumentacionCL()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                try
                {
                    if (disposing)
                    {
                    }
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e);
                }
            }
        }

        #endregion
    }
}
