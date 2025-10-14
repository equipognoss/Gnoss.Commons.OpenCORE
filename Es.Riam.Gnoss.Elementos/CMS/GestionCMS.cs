using System;
using System.Data;
using System.Collections.Generic;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using System.Linq;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Microsoft.Extensions.Logging;

namespace Es.Riam.Gnoss.Elementos.CMS
{
    /// <summary>
    /// Gestor de suscripciones
    /// </summary>
    [Serializable]
    public class GestionCMS : GestionGnoss
    {
        #region Miembros

        /// <summary>
        /// Lista de paginas de los proyectos
        /// </summary>
        private Dictionary<Guid, Dictionary<short, CMSPagina>> mListaPaginas;

        /// <summary>
        /// Lista de bloques
        /// </summary>
        private Dictionary<Guid, CMSBloque> mListaBloques;

        /// <summary>
        /// Lista de componentes
        /// </summary>
        private Dictionary<Guid, CMSComponente> mListaComponentes;

        /// <summary>
        /// Lista de componentes privados disponibles
        /// </summary>
        public List<TipoComponenteCMS> mListaComponentesPrivadosProyecto;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;

        #endregion

        #region Constructores

        public GestionCMS() { }

        /// <summary>
        /// Crea el gestor de CMS
        /// </summary>
        /// <param name="pCMSDS">Dataset de CMS</param>
        public GestionCMS(DataWrapperCMS pCMSDS, LoggingService loggingService, EntityContext entityContext)
            : base(pCMSDS)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el dataset de CMS
        /// </summary>
        public DataWrapperCMS CMSDW
        {
            get
            {
                return (DataWrapperCMS)DataWrapper;
            }
        }

        /// <summary>
        /// Obtiene la lista de componentes
        /// </summary>
        public Dictionary<Guid, CMSComponente> ListaComponentes
        {
            get
            {
                if (mListaComponentes == null)
                {
                    CargarComponentes();
                }
                return mListaComponentes;
            }
        }

        /// <summary>
        /// Obtiene la lista de paginas de los proyectos
        /// </summary>
        public Dictionary<Guid, Dictionary<short, CMSPagina>> ListaPaginasProyectos
        {
            get
            {
                if (mListaPaginas == null)
                {
                    CargarPaginas();
                }
                return mListaPaginas;
            }
        }

        /// <summary>
        /// Obtiene la lista de bloques
        /// </summary>
        public Dictionary<Guid, CMSBloque> ListaBloques
        {
            get
            {
                if (mListaBloques == null)
                {
                    CargarBloques();
                }
                return mListaBloques;
            }
        }

        /// <summary>
        /// Obtiene la lista de paginas de los proyectos
        /// </summary>
        public List<TipoComponenteCMS> ListaComponentesPrivadosProyecto
        {
            get
            {
                if (mListaComponentesPrivadosProyecto == null)
                {
                    CargarListaComponentesPrivadosProyecto();
                }
                return mListaComponentesPrivadosProyecto;
            }
        }

        #endregion

        #region Métodos

        #region Métodos públicos

        /// <summary>
        /// Carga las paginas
        /// </summary>
        public void CargarPaginas()
        {
            mListaPaginas = new Dictionary<Guid, Dictionary<short, CMSPagina>>();

            foreach (AD.EntityModel.Models.CMS.CMSPagina filaPagina in CMSDW.ListaCMSPagina)
            {
                if (mListaPaginas.ContainsKey(filaPagina.ProyectoID))
                {
                    mListaPaginas[filaPagina.ProyectoID].Add(filaPagina.Ubicacion, new CMSPagina(filaPagina, this));
                }
                else
                {
                    Dictionary<short, CMSPagina> listaPaginas = new Dictionary<short, CMSPagina>();
                    listaPaginas.Add(filaPagina.Ubicacion, new CMSPagina(filaPagina, this));
                    mListaPaginas.Add(filaPagina.ProyectoID, listaPaginas);
                }
            }
        }

        /// <summary>
        /// Carga los bloques
        /// </summary>
        public void CargarBloques()
        {
            mListaBloques = new Dictionary<Guid, CMSBloque>();

            foreach (AD.EntityModel.Models.CMS.CMSBloque filaBloque in CMSDW.ListaCMSBloque)
            {
                mListaBloques.Add(filaBloque.BloqueID, new CMSBloque(filaBloque, this));
            }
            foreach (CMSBloque bloque in mListaBloques.Values)
            {
                bloque.CargarSubBloques();
            }
        }

        /// <summary>
        /// Carga los componentesprivados
        /// </summary>
        public void CargarListaComponentesPrivadosProyecto()
        {
            mListaComponentesPrivadosProyecto = new List<TipoComponenteCMS>();

            foreach (AD.EntityModel.Models.CMS.CMSComponentePrivadoProyecto filaComponentePrivadoProyecto in CMSDW.ListaCMSComponentePrivadoProyecto)
            {
                mListaComponentesPrivadosProyecto.Add((TipoComponenteCMS)filaComponentePrivadoProyecto.TipoComponente);
            }
        }

        /// <summary>
        /// Carga los componentes
        /// </summary>
        public void CargarComponentes()
        {
            mListaComponentes = new Dictionary<Guid, CMSComponente>();

            foreach (AD.EntityModel.Models.CMS.CMSComponente filaComponente in CMSDW.ListaCMSComponente)
            {
                List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> listaPropiedadesComponentes = new List<AD.EntityModel.Models.CMS.CMSPropiedadComponente>();
                foreach (AD.EntityModel.Models.CMS.CMSPropiedadComponente fila in CMSDW.ListaCMSPropiedadComponente.Where(item => item.ComponenteID.Equals(filaComponente.ComponenteID)).ToList())
                {
                    listaPropiedadesComponentes.Add(fila);
                }

                switch (filaComponente.TipoComponente)
                {
                    case (short)TipoComponenteCMS.HTML:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteHTML(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.Destacado:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteDestacado(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.ListadoEstatico:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteListadoEstatico(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.ListadoDinamico:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteListadoDinamico(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.ListadoPorParametros:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteListadoPorParametros(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.ActividadReciente:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteActividadReciente(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.GrupoComponentes:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteGrupoComponentes(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.Tesauro:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteTesauro(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case 7:
                        //(short)TipoComponenteCMS.RecursosDestacados 
                        //Desapareció y en su lugar creamos un listado dinámico ficticio
                        //Construimos un componente listado dinamico
                        filaComponente.TipoComponente = (short)TipoComponenteCMS.ListadoDinamico;

                        AD.EntityModel.Models.CMS.CMSPropiedadComponente filaPropiedadDestComponente = null;
                        AD.EntityModel.Models.CMS.CMSPropiedadComponente filaPropiedadRecursoDestComponente = null;
                        foreach (AD.EntityModel.Models.CMS.CMSPropiedadComponente fila in listaPropiedadesComponentes)
                        {
                            if (fila.TipoPropiedadComponente == (short)TipoPropiedadCMS.TipoPresentacionListadoRecursos)
                            {
                                filaPropiedadDestComponente = fila;
                                filaPropiedadDestComponente.ValorPropiedad = ((short)TipoPresentacionListadoRecursosCMS.ListadoDestacados).ToString();
                            }
                            if (fila.TipoPropiedadComponente == (short)TipoPropiedadCMS.TipoPresentacionRecurso)
                            {
                                filaPropiedadRecursoDestComponente = fila;
                                filaPropiedadRecursoDestComponente.ValorPropiedad = ((short)TipoPresentacionRecursoCMS.Destacado).ToString();
                            }
                        }
                        if (filaPropiedadDestComponente == null)
                        {
                            AD.EntityModel.Models.CMS.CMSPropiedadComponente propiedadComponente = new AD.EntityModel.Models.CMS.CMSPropiedadComponente();
                            propiedadComponente.CMSComponente = filaComponente;
                            propiedadComponente.TipoPropiedadComponente = (short)TipoPropiedadCMS.TipoPresentacionListadoRecursos;
                            propiedadComponente.ValorPropiedad = ((short)TipoPresentacionListadoRecursosCMS.ListadoDestacados).ToString();
                            propiedadComponente.ComponenteID = filaComponente.ComponenteID;

                            listaPropiedadesComponentes.Add(propiedadComponente);
                        }
                        if (filaPropiedadRecursoDestComponente == null)
                        {
                            AD.EntityModel.Models.CMS.CMSPropiedadComponente propiedadComponente = new AD.EntityModel.Models.CMS.CMSPropiedadComponente();
                            propiedadComponente.CMSComponente = filaComponente;
                            propiedadComponente.TipoPropiedadComponente = (short)TipoPropiedadCMS.TipoPresentacionRecurso;
                            propiedadComponente.ValorPropiedad = ((short)TipoPresentacionRecursoCMS.Destacado).ToString();
                            propiedadComponente.ComponenteID = filaComponente.ComponenteID;

                            listaPropiedadesComponentes.Add(propiedadComponente);
                        }
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteListadoDinamico(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case 11:
                    case 12:
                        //(short)TipoComponenteCMS.RecursoDestacado 
                        if (filaComponente.TipoComponente == 12)
                        {
                            foreach (AD.EntityModel.Models.CMS.CMSPropiedadComponente fila in listaPropiedadesComponentes)
                            {
                                if (fila.TipoPropiedadComponente == (short)TipoPropiedadCMS.ElementoID)
                                {
                                    fila.TipoPropiedadComponente = (short)TipoPropiedadCMS.ListaIDs;
                                }
                            }
                        }

                        //(short)TipoComponenteCMS.RecursosDestacadosEstatico                             
                        //Desapareció y en su lugar creamos un listado estatico ficticio
                        //Construimos un componente listado estatico
                        filaComponente.TipoComponente = (short)TipoComponenteCMS.ListadoEstatico;

                        AD.EntityModel.Models.CMS.CMSPropiedadComponente filaPropiedadListadoDestEstaticoComponente = null;
                        AD.EntityModel.Models.CMS.CMSPropiedadComponente filaPropiedadRecursoDestEstaticoComponente = null;
                        foreach (AD.EntityModel.Models.CMS.CMSPropiedadComponente fila in listaPropiedadesComponentes)
                        {
                            if (fila.TipoPropiedadComponente == (short)TipoPropiedadCMS.TipoPresentacionListadoRecursos)
                            {
                                filaPropiedadListadoDestEstaticoComponente = fila;
                                filaPropiedadListadoDestEstaticoComponente.ValorPropiedad = ((short)TipoPresentacionListadoRecursosCMS.ListadoDestacados).ToString();
                            }
                            if (fila.TipoPropiedadComponente == (short)TipoPropiedadCMS.TipoPresentacionRecurso)
                            {
                                filaPropiedadRecursoDestEstaticoComponente = fila;
                                filaPropiedadRecursoDestEstaticoComponente.ValorPropiedad = ((short)TipoPresentacionRecursoCMS.Destacado).ToString();
                            }
                        }

                        if (filaPropiedadListadoDestEstaticoComponente == null)
                        {
                            filaPropiedadListadoDestEstaticoComponente = new AD.EntityModel.Models.CMS.CMSPropiedadComponente();
                            filaPropiedadListadoDestEstaticoComponente.CMSComponente = filaComponente;
                            filaPropiedadListadoDestEstaticoComponente.TipoPropiedadComponente = (short)TipoPropiedadCMS.TipoPresentacionListadoRecursos;
                            filaPropiedadListadoDestEstaticoComponente.ValorPropiedad = ((short)TipoPresentacionListadoRecursosCMS.ListadoDestacados).ToString();
                            filaPropiedadListadoDestEstaticoComponente.ComponenteID = filaComponente.ComponenteID;

                            listaPropiedadesComponentes.Add(filaPropiedadListadoDestEstaticoComponente);
                        }

                        if (filaPropiedadRecursoDestEstaticoComponente == null)
                        {
                            filaPropiedadRecursoDestEstaticoComponente = new AD.EntityModel.Models.CMS.CMSPropiedadComponente();
                            filaPropiedadRecursoDestEstaticoComponente.CMSComponente = filaComponente;
                            filaPropiedadRecursoDestEstaticoComponente.TipoPropiedadComponente = (short)TipoPropiedadCMS.TipoPresentacionRecurso;
                            filaPropiedadRecursoDestEstaticoComponente.ValorPropiedad = ((short)TipoPresentacionRecursoCMS.Destacado).ToString();
                            filaPropiedadRecursoDestEstaticoComponente.ComponenteID = filaComponente.ComponenteID;

                            listaPropiedadesComponentes.Add(filaPropiedadRecursoDestEstaticoComponente);
                        }

                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteListadoEstatico(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.DatosComunidad:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteDatosComunidad(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.UsuariosRecomendados:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteUsuariosRecomendados(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.CajaBuscador:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteCajaBuscador(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.Faceta:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteFaceta(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.ListadoUsuarios:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteListadoUsuarios(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.ListadoProyectos:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteListadoProyectos(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.ResumenPerfil:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteResumenPerfil(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.MasVistos:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteMasVistos(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.MasVistosEnXDias:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteMasVistosEnXDias(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.EnvioCorreo:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteEnvioCorreo(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.PreguntaTIC:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponentePreguntaTIC(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.Menu:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteMenu(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.Buscador:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteBuscador(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.BuscadorSPARQL:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteBuscadorSPARQL(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.UltimosRecursosVisitados:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteUltimosRecursosVisitados(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.FichaDescripcionDocumento:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteFichaDescripcionDocumento(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.ConsultaSPARQL:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteConsultaSPARQL(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                    case (short)TipoComponenteCMS.ConsultaSQLSERVER:
                        mListaComponentes.Add(filaComponente.ComponenteID, new CMSComponenteConsultaSQLSERVER(filaComponente, listaPropiedadesComponentes, this, mLoggingService, mEntityContext));
                        break;
                        //default:
                        //throw new Exception("Constructor no definido para el componente " + (TipoComponenteCMS)filaComponente.TipoComponente);
                }
            }
        }

        #region Agregar/Editar/Eliminar Paginas

        /// <summary>
        /// Agrega una nueva pagina
        /// </summary>
        ///<param name="pProyecto"></param>
        ///<param name="pTipoUbicacionShort"></param>
        /// <returns></returns>
        public CMSPagina AgregarNuevaPagina(ServiciosGenerales.Proyecto pProyecto, short pTipoUbicacionShort)
        {
            AD.EntityModel.Models.CMS.CMSPagina filapagina = new AD.EntityModel.Models.CMS.CMSPagina();
            filapagina.OrganizacionID = pProyecto.FilaProyecto.OrganizacionID;
            filapagina.ProyectoID = pProyecto.FilaProyecto.ProyectoID;
            filapagina.Ubicacion = pTipoUbicacionShort;
            filapagina.Activa = false;

            CMSDW.ListaCMSPagina.Add(filapagina);
            mEntityContext.CMSPagina.Add(filapagina);

            CMSPagina pagina = new CMSPagina(filapagina, this);

            if (ListaPaginasProyectos.ContainsKey(pProyecto.Clave))
            {
                if (!ListaPaginasProyectos[pProyecto.Clave].ContainsKey(pTipoUbicacionShort))
                {
                    ListaPaginasProyectos[pProyecto.Clave].Add(pTipoUbicacionShort, pagina);
                }
            }
            else
            {
                Dictionary<short, CMSPagina> listaPaginas = new Dictionary<short, CMSPagina>();
                listaPaginas.Add(pTipoUbicacionShort, pagina);
                ListaPaginasProyectos.Add(pProyecto.Clave, listaPaginas);
            }

            return pagina;
        }

        /// <summary>
        /// Eliminar componente
        /// </summary>
        public void EliminarPagina(Guid pProyectoID, short pTipoUbicacion)
        {
            CMSPagina pagina = ListaPaginasProyectos[pProyectoID][pTipoUbicacion];
            mEntityContext.EliminarElemento(pagina.Filapagina);
            CMSDW.ListaCMSPagina.Remove(pagina.Filapagina);
            ListaPaginasProyectos[pProyectoID].Remove(pTipoUbicacion);
        }


        #endregion

        #region Agregar/Eliminar Componentes

        /// <summary>
        /// Agrega un nuevo Componente de tipo HTML libre
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTipoCaducidadComponente">Caducidad del componente</param>
        /// <param name="pHTML">HTML del componente</param>
        /// <returns>Devuelve el componente creado</returns>
        public CMSComponenteHTML AgregarNuevoComponenteHTML(ServiciosGenerales.Proyecto pProyecto, string pTitulo, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pHTML, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.HTML, pHTML);

            return (CMSComponenteHTML)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.HTML, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo Destacado
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTipoCaducidadComponente">Caducidad del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        /// <param name="pRutaImagen">Ruta de la imagen destacada</param>
        /// <param name="pHTML">HTML del componente</param>
        /// <param name="pEnlace">Enlace destacado</param>
        public CMSComponenteDestacado AgregarNuevoComponenteDestacado(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, string pSubTitulo, string pRutaImagen, string pHTML, string pEnlace, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.Subtitulo, pSubTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.Imagen, pRutaImagen);
            propiedadesComponente.Add(TipoPropiedadCMS.HTML, pHTML);
            propiedadesComponente.Add(TipoPropiedadCMS.Enlace, pEnlace);

            return (CMSComponenteDestacado)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.Destacado, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo ListadoEstatico
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTipoCaducidadComponente">Caducidad del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        /// <param name="pListaGuids">Listado de ID de recursos</param>
        /// <param name="pNumeroItems">Número de items que se muestran</param>
        /// <param name="pTipoPresentacion">Tipo de presentacion</param>
        public CMSComponenteListadoEstatico AgregarNuevoComponenteListadoEstatico(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, List<Guid> pListaGuids, string pURLVerMas, short pNumeroItems, string pTipoPresentacion, string pTipoPresentacionListado, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);

            string listaGuids = "";
            foreach (Guid id in pListaGuids)
            {
                listaGuids += $"{id},";
            }
            propiedadesComponente.Add(TipoPropiedadCMS.URLVerMas, pURLVerMas);
            propiedadesComponente.Add(TipoPropiedadCMS.ListaIDs, listaGuids);
            propiedadesComponente.Add(TipoPropiedadCMS.NumItems, pNumeroItems.ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionRecurso, pTipoPresentacion);
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionListadoRecursos, pTipoPresentacionListado);

            return (CMSComponenteListadoEstatico)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.ListadoEstatico, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo ListadoRecursosDinamico
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        /// <param name="pUrlBusqueda">Url de la búsqueda</param>
        /// <param name="pTipoDeBusqueda">Tipo de busqueda</param>
        /// <param name="pNumeroItems">Númerom de items que se muestran</param>
        /// <param name="pNumeroItemsMostrar">Número de items para mostrar</param>
        /// <param name="pTipoPresentacion">Enumeración del tipo de presentación</param>
        public CMSComponenteListadoDinamico AgregarNuevoComponenteListadoDinamico(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, string pUrlBusqueda, string pURLVerMas, short pNumeroItems, string pTipoPresentacion, string pTipoPresentacionListado, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.URLBusqueda, pUrlBusqueda);
            propiedadesComponente.Add(TipoPropiedadCMS.URLVerMas, pURLVerMas);
            propiedadesComponente.Add(TipoPropiedadCMS.NumItems, pNumeroItems.ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionRecurso, pTipoPresentacion);
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionListadoRecursos, pTipoPresentacionListado);

            return (CMSComponenteListadoDinamico)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.ListadoDinamico, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo Actividad reciente
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTipoCaducidadComponente">Caducidad del componente</param>
        /// <param name="pNumeroItems">Número de items que se muestran</param>
        public CMSComponenteActividadReciente AgregarNuevoComponenteActividadReciente(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, short pNumeroItems, TipoActividadReciente pTipoActividadReciente, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.NumItems, pNumeroItems.ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.TipoActividadRecienteCMS, ((short)pTipoActividadReciente).ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);

            return (CMSComponenteActividadReciente)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.ActividadReciente, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo ListadoEstatico
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTipoCaducidadComponente">Caducidad del componente</param>
        /// <param name="pListaGuids">Listado de ID de recursos</param>
        public CMSComponenteGrupoComponentes AgregarNuevoComponenteGrupoDeComponentes(ServiciosGenerales.Proyecto pProyecto, string pTitulo, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, List<Guid> pListaGuids, string pTipoPresentacion, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();

            string listaGuids = "";
            foreach (Guid id in pListaGuids)
            {
                listaGuids += $"{id},";
            }
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.ListaIDs, listaGuids);
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionGrupoComponentes, pTipoPresentacion);

            return (CMSComponenteGrupoComponentes)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.GrupoComponentes, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo tesauro
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTipoCaducidadComponente">Caducidad del componente</param>
        /// <param name="pCategoriaID">indica si queremos que se pinte la categoria</param>
        public CMSComponenteTesauro AgregarNuevoComponenteTesauro(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, string pTitulo, short pTipoCaducidadComponente, Guid pCategoriaID, bool pTieneImagen, short? pNumItemsMostrar, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.ElementoID, pCategoriaID.ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo.ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.TieneImagen, pTieneImagen.ToString());
            if (pNumItemsMostrar.HasValue)
            {
                propiedadesComponente.Add(TipoPropiedadCMS.NumItemsMostrar, pNumItemsMostrar.ToString());
            }

            return (CMSComponenteTesauro)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.Tesauro, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo RecursosDestacados
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        /// <param name="pUrlBusqueda">Url de la búsqueda</param>
        /// <param name="pTipoDeBusqueda">Tipo de busqueda</param>
        /// <param name="pNumeroItems">Númerom de items que se muestran</param>
        public CMSComponenteDatosComunidad AgregarNuevoComponenteDatosComunidad(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, bool pContarPersonasNoVisibles, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.ContarPersonasNoVisibles, pContarPersonasNoVisibles.ToString());

            return (CMSComponenteDatosComunidad)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.DatosComunidad, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo Usuarios recomendados
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pNumeroItems">Númerom de items que se muestran</param>
        public CMSComponenteUsuariosRecomendados AgregarNuevoComponenteUsuariosRecomendados(ServiciosGenerales.Proyecto pProyecto, string pTitulo, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, short pNumeroItems, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.NumItems, pNumeroItems.ToString());
            return (CMSComponenteUsuariosRecomendados)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.UsuariosRecomendados, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo caja buscador
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pEstilos"></param>
        /// <param name="pTipoCaducidadComponente"></param>
        /// <param name="pTitulo"></param>
        /// <param name="pUrlBusqueda"></param>
        public CMSComponenteCajaBuscador AgregarNuevoComponenteCajaBuscador(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, string pUrlBusqueda, string pTextoDefecto, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.URLBusqueda, pUrlBusqueda);
            propiedadesComponente.Add(TipoPropiedadCMS.TextoDefecto, pTextoDefecto);
            return (CMSComponenteCajaBuscador)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.CajaBuscador, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo Faceta
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pEstilos">Estilos CSS</param>
        /// <param name="pTipoCaducidadComponente">Tipo de caducidad</param>
        /// <param name="pTitulo">Título del Componente</param>
        /// <param name="pFaceta">Faceta</param>
        public CMSComponenteFaceta AgregarNuevoComponenteFaceta(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, string pFaceta, string pURLBusqueda, TipoPresentacionFacetas pTipoPresentacionFacetas, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.Faceta, pFaceta);
            propiedadesComponente.Add(TipoPropiedadCMS.URLBusqueda, pURLBusqueda);
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionFaceta, ((short)pTipoPresentacionFacetas).ToString());
            return (CMSComponenteFaceta)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.Faceta, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo ListadoUsuarios
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTipoCaducidadComponente">Tipo de caducidad</param>
        /// <param name="pTitulo">Título del Componente</param>
        public CMSComponenteListadoUsuarios AgregarNuevoComponenteListadoUsuarios(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, TipoPresentacionListadoUsuariosCMS pTipoPresentacionListadoUsuarios, TipoListadoUsuariosCMS pTipoListadoUsuarios, short pNumeroUsuarios, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionListadoUsuarios, ((short)pTipoPresentacionListadoUsuarios).ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.TipoListadoUsuarios, ((short)pTipoListadoUsuarios).ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.NumItems, pNumeroUsuarios.ToString());
            return (CMSComponenteListadoUsuarios)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.ListadoUsuarios, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo ProyectosRecomendados
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        public CMSComponenteListadoProyectos AgregarNuevoComponenteListadoProyectos(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, short pNumeroProyectos, List<Guid> pListaID, TipoListadoProyectosCMS pTipoListadoProyectos, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            if (pTipoListadoProyectos == TipoListadoProyectosCMS.Estaticos)
            {
                string listaGuids = "";
                foreach (Guid id in pListaID)
                {
                    listaGuids += $"{id},";
                }
                propiedadesComponente.Add(TipoPropiedadCMS.ListaIDs, listaGuids);
            }
            else
            {
                propiedadesComponente.Add(TipoPropiedadCMS.NumItems, pNumeroProyectos.ToString());
            }

            propiedadesComponente.Add(TipoPropiedadCMS.TipoListadoProyectos, ((short)pTipoListadoProyectos).ToString());
            return (CMSComponenteListadoProyectos)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.ListadoProyectos, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo ResumenPerfil
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        public CMSComponenteResumenPerfil AgregarNuevoComponenteResumenPerfil(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            return (CMSComponenteResumenPerfil)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.ResumenPerfil, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo ResumenPerfil
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        /// <param name="pNumItems">Numero de elementos del Componente</param>
        public CMSComponenteMasVistos AgregarNuevoComponenteMasVistos(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, short pNumItems, string pTipoPresentacion, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.NumItems, pNumItems.ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionRecurso, pTipoPresentacion);
            return (CMSComponenteMasVistos)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.MasVistos, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo ResumenPerfil
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        /// <param name="pNumItems">Numero de elementos del Componente</param>
        public CMSComponenteMasVistosEnXDias AgregarNuevoComponenteMasVistosEnXDias(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, short pNumItems, short pNumDias, string pTipoPresentacion, string pTipoPresentacionListado, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.NumItems, pNumItems.ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.NumDias, pNumDias.ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionRecurso, pTipoPresentacion);
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionListadoRecursos, pTipoPresentacionListado);
            return (CMSComponenteMasVistosEnXDias)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.MasVistosEnXDias, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo EnvioCorreo
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        /// <param name="pTextoBoton">Texto de botón</param>
        /// <param name="pDestinatarioCorreo">Destinatario del correo</param>
        public CMSComponenteEnvioCorreo AgregarNuevoComponenteEnvioCorreo(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, Dictionary<short, Dictionary<TipoPropiedadEnvioCorreo, string>> pListaCamposEnvioCorreo, string pTextoBoton, string pDestinatarioCorreo, string pTextoMensajeOK, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            string textoListaCamposEnvioCorreo = "";
            string separador = "";
            foreach (short orden in pListaCamposEnvioCorreo.Keys)
            {
                textoListaCamposEnvioCorreo += $"{separador}{pListaCamposEnvioCorreo[orden][TipoPropiedadEnvioCorreo.Nombre]}&&&{pListaCamposEnvioCorreo[orden][TipoPropiedadEnvioCorreo.Obligatorio]}&&&{pListaCamposEnvioCorreo[orden][TipoPropiedadEnvioCorreo.TipoCampo]}";
                separador = "###";
            }
            propiedadesComponente.Add(TipoPropiedadCMS.ListaCamposEnvioCorreo, textoListaCamposEnvioCorreo);
            propiedadesComponente.Add(TipoPropiedadCMS.TextoBoton, pTextoBoton);
            propiedadesComponente.Add(TipoPropiedadCMS.DestinatarioCorreo, pDestinatarioCorreo);
            propiedadesComponente.Add(TipoPropiedadCMS.TextoMensajeOK, pTextoMensajeOK);
            return (CMSComponenteEnvioCorreo)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.EnvioCorreo, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo PreguntaTIC
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        public CMSComponentePreguntaTIC AgregarNuevoComponentePreguntaTIC(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, Guid pOntologiaID, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.ElementoID, pOntologiaID.ToString());
            return (CMSComponentePreguntaTIC)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.PreguntaTIC, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo Menu
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        public CMSComponenteMenu AgregarNuevoComponenteMenu(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, Dictionary<short, KeyValuePair<short, Dictionary<TipoPropiedadMenu, string>>> pListaOpcionesMenu, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            string textoListaOpcionesMenu = "";
            string separador = "";
            foreach (short orden in pListaOpcionesMenu.Keys)
            {
                textoListaOpcionesMenu += $"{separador}{pListaOpcionesMenu[orden].Key}&&&{pListaOpcionesMenu[orden].Value[TipoPropiedadMenu.Nombre]}&&&{pListaOpcionesMenu[orden].Value[TipoPropiedadMenu.Enlace]}";
                separador = "###";
            }
            propiedadesComponente.Add(TipoPropiedadCMS.ListaOpcionesMenu, textoListaOpcionesMenu);
            return (CMSComponenteMenu)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.Menu, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo Buscador
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        /// <param name="pUrlBusqueda">Url de la búsqueda</param>
        /// <param name="pTipoDeBusqueda">Tipo de busqueda</param>
        /// <param name="pNumeroItems">Númerom de items que se muestran</param>
        /// <param name="pNumeroItemsMostrar">Número de items para mostrar</param>
        /// <param name="pTipoPresentacion">Enumeración del tipo de presentación</param>
        public CMSComponenteBuscador AgregarNuevoComponenteBuscador(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, string pUrlBusqueda, string pTituloAtributoDeBusqueda, string pAtributoDeBusqueda, short pNumeroItems, string pTipoPresentacion, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.URLBusqueda, pUrlBusqueda);
            propiedadesComponente.Add(TipoPropiedadCMS.TituloAtributoDeBusqueda, pTituloAtributoDeBusqueda);
            propiedadesComponente.Add(TipoPropiedadCMS.AtributoDeBusqueda, pAtributoDeBusqueda);
            propiedadesComponente.Add(TipoPropiedadCMS.NumItems, pNumeroItems.ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionRecurso, pTipoPresentacion);

            return (CMSComponenteBuscador)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.Buscador, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo BuscadorSPARQL
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        public CMSComponenteBuscadorSPARQL AgregarNuevoComponenteListadoDinamicoSPARQL(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, string pQuerySPARQL, short pNumeroItems, string pTipoPresentacion, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.QuerySPARQL, pQuerySPARQL);
            propiedadesComponente.Add(TipoPropiedadCMS.NumItems, pNumeroItems.ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionRecurso, pTipoPresentacion);
            return (CMSComponenteBuscadorSPARQL)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.BuscadorSPARQL, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo UltimosRecursosVisitados
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        public CMSComponenteUltimosRecursosVisitados AgregarNuevoComponenteUltimosRecursosVisitados(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, short pNumeroItems, string pTipoPresentacion, string pTipoPresentacionListado, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.NumItems, pNumeroItems.ToString());
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionRecurso, pTipoPresentacion);
            propiedadesComponente.Add(TipoPropiedadCMS.TipoPresentacionListadoRecursos, pTipoPresentacionListado);
            return (CMSComponenteUltimosRecursosVisitados)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.UltimosRecursosVisitados, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo FichaDescripcionDocumento
        /// </summary>
        /// <param name="pProyecto"></param>
        /// <param name="pNombre"></param>
        /// <param name="pEstilos"></param>
        /// <param name="pActivo"></param>
        /// <param name="pTipoCaducidadComponente"></param>
        /// <param name="pTitulo"></param>
        /// <param name="pDocumentoID"></param>
        public CMSComponenteFichaDescripcionDocumento AgregarNuevoComponenteFichaDescripcionDocumento(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, Guid pDocumentoID, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.ElementoID, pDocumentoID.ToString());
            return (CMSComponenteFichaDescripcionDocumento)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.FichaDescripcionDocumento, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo ConsultaSPARQL
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        public CMSComponenteConsultaSPARQL AgregarNuevoComponenteConsultaSPARQL(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, string pQuerySPARQL, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.QuerySPARQL, pQuerySPARQL);
            return (CMSComponenteConsultaSPARQL)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.ConsultaSPARQL, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente de tipo ConsultaSQLSERVER
        /// </summary>
        /// <param name="pProyecto">Proyecto del Gestor</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pTitulo">Título del Componente</param>
        public CMSComponenteConsultaSQLSERVER AgregarNuevoComponenteConsultaSQLSERVER(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoCaducidadComponente, string pTitulo, string pQuerySQLESRVER, bool pAccesoPublico)
        {
            Dictionary<TipoPropiedadCMS, string> propiedadesComponente = new Dictionary<TipoPropiedadCMS, string>();
            propiedadesComponente.Add(TipoPropiedadCMS.Titulo, pTitulo);
            propiedadesComponente.Add(TipoPropiedadCMS.QuerySQLSERVER, pQuerySQLESRVER);
            return (CMSComponenteConsultaSQLSERVER)AgregarNuevoComponente(pProyecto, pNombre, pEstilos, pActivo, (short)TipoComponenteCMS.ConsultaSQLSERVER, pTipoCaducidadComponente, propiedadesComponente, pAccesoPublico);
        }

        /// <summary>
        /// Agrega un nuevo Componente
        /// </summary>
        /// <param name="pProyecto">Proyecto del componente</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pEstilos">Estilos del componente</param>
        /// <param name="pTipoComponente">Tipo del componente</param>
        /// <param name="pTipoCaducidadComponente">Tipo de caducidad del componente</param>
        /// <param name="pPropiedadesComponente">Propiedades del componente</param>
        /// <returns></returns>
        public CMSComponente AgregarNuevoComponente(ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoComponente, short pTipoCaducidadComponente, Dictionary<TipoPropiedadCMS, string> pPropiedadesComponente, bool pAccesoPublico)
        {
            Guid idComponente = Guid.NewGuid();
            return AgregarNuevoComponente(idComponente, pProyecto, pNombre, pEstilos, pActivo, pTipoComponente, pTipoCaducidadComponente, pPropiedadesComponente, pAccesoPublico);
        }


        /// <summary>
        /// Agrega un nuevo Componente
        /// </summary>
        /// <param name="pProyecto">Proyecto del componente</param>
        /// <param name="pNombre">Nombre del componente</param>
        /// <param name="pEstilos">Estilos del componente</param>
        /// <param name="pTipoComponente">Tipo del componente</param>
        /// <param name="pTipoCaducidadComponente">Tipo de caducidad del componente</param>
        /// <param name="pPropiedadesComponente">Propiedades del componente</param>
        /// <returns></returns>
        public CMSComponente AgregarNuevoComponente(Guid idComponente, ServiciosGenerales.Proyecto pProyecto, string pNombre, string pEstilos, bool pActivo, short pTipoComponente, short pTipoCaducidadComponente, Dictionary<TipoPropiedadCMS, string> pPropiedadesComponente, bool pAccesoPublico)
        {
            if (string.IsNullOrEmpty(pNombre))
            {
                throw new Exception("El componente debe tener nombre");
            }

            AD.EntityModel.Models.CMS.CMSComponente filaComponente = new AD.EntityModel.Models.CMS.CMSComponente();
            filaComponente.OrganizacionID = pProyecto.FilaProyecto.OrganizacionID;
            filaComponente.ProyectoID = pProyecto.FilaProyecto.ProyectoID;
            filaComponente.ComponenteID = idComponente;
            filaComponente.NombreCortoComponente = idComponente.ToString();
            filaComponente.Nombre = pNombre;
            filaComponente.Estilos = pEstilos;
            filaComponente.Activo = pActivo;
            filaComponente.AccesoPublico = pAccesoPublico;
            filaComponente.TipoComponente = pTipoComponente;
            filaComponente.TipoCaducidadComponente = pTipoCaducidadComponente;
            filaComponente.FechaUltimaActualizacion = DateTime.Now;

            CMSDW.ListaCMSComponente.Add(filaComponente);
            mEntityContext.CMSComponente.Add(filaComponente);

            List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> listaPropiedades = new List<AD.EntityModel.Models.CMS.CMSPropiedadComponente>();
            foreach (TipoPropiedadCMS propiedad in pPropiedadesComponente.Keys)
            {
                if (!string.IsNullOrEmpty(pPropiedadesComponente[propiedad]))
                {
                    AD.EntityModel.Models.CMS.CMSPropiedadComponente filaPropiedadComponente = new AD.EntityModel.Models.CMS.CMSPropiedadComponente();
                    filaPropiedadComponente.ComponenteID = idComponente;
                    filaPropiedadComponente.TipoPropiedadComponente = (short)propiedad;
                    filaPropiedadComponente.ValorPropiedad = pPropiedadesComponente[propiedad];

                    CMSDW.ListaCMSPropiedadComponente.Add(filaPropiedadComponente);
                    mEntityContext.CMSPropiedadComponente.Add(filaPropiedadComponente);

                    listaPropiedades.Add(filaPropiedadComponente);
                }
            }

            CMSComponente componente = null;

            switch (pTipoComponente)
            {
                case (short)TipoComponenteCMS.HTML:
                    componente = new CMSComponenteHTML(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.Destacado:
                    componente = new CMSComponenteDestacado(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.ListadoPorParametros:
                    componente = new CMSComponenteListadoPorParametros(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.ListadoEstatico:
                    componente = new CMSComponenteListadoEstatico(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.ListadoDinamico:
                    componente = new CMSComponenteListadoDinamico(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.ActividadReciente:
                    componente = new CMSComponenteActividadReciente(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.GrupoComponentes:
                    componente = new CMSComponenteGrupoComponentes(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.Tesauro:
                    componente = new CMSComponenteTesauro(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                //case (short)TipoComponenteCMS.RecursosDestacados:
                //    componente = new CMSComponenteRecursosDestacados(filaComponente, listaPropiedades, this);
                //    break;
                //case (short)TipoComponenteCMS.RecursosDestacadosEstatico:
                //    componente = new CMSComponenteRecursosDestacadosEstatico(filaComponente, listaPropiedades, this);
                //    break;
                case (short)TipoComponenteCMS.DatosComunidad:
                    componente = new CMSComponenteDatosComunidad(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.UsuariosRecomendados:
                    componente = new CMSComponenteUsuariosRecomendados(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.CajaBuscador:
                    componente = new CMSComponenteCajaBuscador(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                //case (short)TipoComponenteCMS.RecursoDestacado:
                //    componente = new CMSComponenteRecursoDestacado(filaComponente, listaPropiedades, this);
                //    break;
                case (short)TipoComponenteCMS.Faceta:
                    componente = new CMSComponenteFaceta(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.ListadoUsuarios:
                    componente = new CMSComponenteListadoUsuarios(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.ListadoProyectos:
                    componente = new CMSComponenteListadoProyectos(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.ResumenPerfil:
                    componente = new CMSComponenteResumenPerfil(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.MasVistos:
                    componente = new CMSComponenteMasVistos(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.MasVistosEnXDias:
                    componente = new CMSComponenteMasVistosEnXDias(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.EnvioCorreo:
                    componente = new CMSComponenteEnvioCorreo(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.PreguntaTIC:
                    componente = new CMSComponentePreguntaTIC(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.Menu:
                    componente = new CMSComponenteMenu(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.Buscador:
                    componente = new CMSComponenteBuscador(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.BuscadorSPARQL:
                    componente = new CMSComponenteBuscadorSPARQL(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.UltimosRecursosVisitados:
                    componente = new CMSComponenteUltimosRecursosVisitados(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.FichaDescripcionDocumento:
                    componente = new CMSComponenteFichaDescripcionDocumento(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.ConsultaSPARQL:
                    componente = new CMSComponenteConsultaSPARQL(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                case (short)TipoComponenteCMS.ConsultaSQLSERVER:
                    componente = new CMSComponenteConsultaSQLSERVER(filaComponente, listaPropiedades, this, mLoggingService, mEntityContext);
                    break;
                default:
                    throw new Exception("Constructor no definido para el componente " + (TipoComponenteCMS)pTipoComponente);
            }

            if (!ListaComponentes.ContainsKey(idComponente))
            {
                ListaComponentes.Add(idComponente, componente);
            }

            return componente;
        }

        /// <summary>
        /// Eliminar componente
        /// </summary>
        public void EliminarComponente(Guid pComponenteID)
        {
            List<AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente> listaCMSBloqueComponentePropiedadComponente = CMSDW.ListaCMSBloqueComponentePropiedadComponente.Where(item => item.ComponenteID.Equals(pComponenteID)).ToList();
            foreach (AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente filaCMSBloqueComponentePropiedadComponente in listaCMSBloqueComponentePropiedadComponente)
            {
                CMSDW.ListaCMSBloqueComponentePropiedadComponente.Remove(filaCMSBloqueComponentePropiedadComponente);
                mEntityContext.EliminarElemento(filaCMSBloqueComponentePropiedadComponente);
            }

            List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> listaCMSPropiedadComponente = CMSDW.ListaCMSPropiedadComponente.Where(item => item.ComponenteID.Equals(pComponenteID)).ToList();
            foreach (AD.EntityModel.Models.CMS.CMSPropiedadComponente filaPropiedadComponente in listaCMSPropiedadComponente)
            {
                CMSDW.ListaCMSPropiedadComponente.Remove(filaPropiedadComponente);
                mEntityContext.EliminarElemento(filaPropiedadComponente);
            }

            List<AD.EntityModel.Models.CMS.CMSBloqueComponente> listaCMSBloqueComponente = CMSDW.ListaCMSBloqueComponente.Where(item => item.ComponenteID.Equals(pComponenteID)).ToList();
            foreach (AD.EntityModel.Models.CMS.CMSBloqueComponente filaBloqueComponente in listaCMSBloqueComponente)
            {
                CMSDW.ListaCMSBloqueComponente.Remove(filaBloqueComponente);
                mEntityContext.EliminarElemento(filaBloqueComponente);
            }

            List<AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades> listaCMSComponenteRolGrupoIdentidades = CMSDW.ListaCMSComponenteRolGrupoIdentidades.Where(item => item.ComponenteID.Equals(pComponenteID)).ToList();
            foreach (AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades filaComponenteRolGrupoIdentidades in listaCMSComponenteRolGrupoIdentidades)
            {
                CMSDW.ListaCMSComponenteRolGrupoIdentidades.Remove(filaComponenteRolGrupoIdentidades);
                mEntityContext.EliminarElemento(filaComponenteRolGrupoIdentidades);
            }

            List<AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad> listaCMSComponenteRolIdentidad = CMSDW.ListaCMSComponenteRolIdentidad.Where(item => item.ComponenteID.Equals(pComponenteID)).ToList();
            foreach (AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad filaComponenteRolIdentidad in listaCMSComponenteRolIdentidad)
            {
                CMSDW.ListaCMSComponenteRolIdentidad.Remove(filaComponenteRolIdentidad);
                mEntityContext.EliminarElemento(filaComponenteRolIdentidad);
            }

            AD.EntityModel.Models.CMS.CMSComponente cmsComponente = ListaComponentes[pComponenteID].FilaComponente;
            CMSDW.ListaCMSComponente.Remove(cmsComponente);
            mEntityContext.EliminarElemento(cmsComponente);
        }

        #endregion

        public void EliminarComponenteDePagina(Guid pComponenteID)
        {
            List<AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente> listaCMSBloqueComponentePropiedadComponente = this.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Where(item => item.ComponenteID.Equals(pComponenteID)).ToList();
            foreach (AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente filaCMSBloqueComponentePropiedadComponente in listaCMSBloqueComponentePropiedadComponente)
            {
                CMSDW.ListaCMSBloqueComponentePropiedadComponente.Remove(filaCMSBloqueComponentePropiedadComponente);
                mEntityContext.EliminarElemento(filaCMSBloqueComponentePropiedadComponente);
            }

            List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> listaCMSPropiedadComponente = this.CMSDW.ListaCMSPropiedadComponente.Where(item => item.ComponenteID.Equals(pComponenteID)).ToList();
            foreach (AD.EntityModel.Models.CMS.CMSPropiedadComponente filaPropiedadComponente in listaCMSPropiedadComponente)
            {
                CMSDW.ListaCMSPropiedadComponente.Remove(filaPropiedadComponente);
                mEntityContext.EliminarElemento(filaPropiedadComponente);
            }

            List<AD.EntityModel.Models.CMS.CMSBloqueComponente> listaCMSBloqueComponente = this.CMSDW.ListaCMSBloqueComponente.Where(item => item.ComponenteID.Equals(pComponenteID)).ToList();
            foreach (AD.EntityModel.Models.CMS.CMSBloqueComponente filaBloqueComponente in listaCMSBloqueComponente)
            {
                CMSDW.ListaCMSBloqueComponente.Remove(filaBloqueComponente);
                mEntityContext.EliminarElemento(filaBloqueComponente);
            }

            List<AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades> listaCMSComponenteRolGrupoIdentidades = this.CMSDW.ListaCMSComponenteRolGrupoIdentidades.Where(item => item.ComponenteID.Equals(pComponenteID)).ToList();
            foreach (AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades filaComponenteRolGrupoIdentidades in listaCMSComponenteRolGrupoIdentidades)
            {
                CMSDW.ListaCMSComponenteRolGrupoIdentidades.Remove(filaComponenteRolGrupoIdentidades);
                mEntityContext.EliminarElemento(filaComponenteRolGrupoIdentidades);
            }
        }

        #region Agregar/Editar/Eliminar Bloques

        /// <summary>
        /// Agrega un nuevo bloque
        /// </summary>
        /// <param name="pPagina">Pagina a la que pertenece</param>
        /// <param name="pCMSBloquePadreID">ID del padre del bloque NULL si no tiene padre</param>
        /// <param name="pOrden">Orden del elemento</param>
        /// <param name="pEstilos">Estilos</param>
        /// <param name="pBorrador">Borrador</param>
        /// <returns></returns>
        public CMSBloque AgregarNuevoBloque(CMSPagina pPagina, Guid? pCMSBloquePadreID, short pOrden, string pEstilos, bool pBorrador)
        {
            Guid idBloque = Guid.NewGuid();
            return AgregarNuevoBloque(idBloque, pPagina, pCMSBloquePadreID, pOrden, pEstilos, pBorrador);
        }

        /// <summary>
        /// Agrega un nuevo bloque
        /// </summary>
        /// <param name="idBloque">ID del bloque</param>
        /// <param name="pPagina">Pagina a la que pertenece</param>
        /// <param name="pCMSBloquePadreID">ID del padre del bloque NULL si no tiene padre</param>
        /// <param name="pComponenteID">ID del conmponente que contiene NULL si está vacío</param>
        /// <param name="pOrden">Orden del elemento</param>
        /// <param name="pEstilos">Estilos</param>
        /// <param name="pBorrador">Borrador</param>
        /// <returns></returns>
        public CMSBloque AgregarNuevoBloque(Guid pIdBloque, CMSPagina pPagina, Guid? pCMSBloquePadreID, short pOrden, string pEstilos, bool pBorrador)
        {
            AD.EntityModel.Models.CMS.CMSBloque filaBloque = new AD.EntityModel.Models.CMS.CMSBloque();
            filaBloque.OrganizacionID = pPagina.Filapagina.OrganizacionID;
            filaBloque.ProyectoID = pPagina.Filapagina.ProyectoID;
            filaBloque.Ubicacion = pPagina.Filapagina.Ubicacion;
            filaBloque.BloqueID = pIdBloque;
            filaBloque.Borrador = pBorrador;
            if (pCMSBloquePadreID.HasValue)
            {
                filaBloque.BloquePadreID = pCMSBloquePadreID.Value;
            }

            filaBloque.Orden = pOrden;
            filaBloque.Estilos = pEstilos;

            CMSDW.ListaCMSBloque.Add(filaBloque);
            mEntityContext.CMSBloque.Add(filaBloque);

            CMSBloque bloque = new CMSBloque(filaBloque, this);

            if (!ListaBloques.ContainsKey(pIdBloque))
            {
                ListaBloques.Add(pIdBloque, bloque);
            }

            return bloque;
        }


        /// <summary>
        /// Agegamos un componente a un bloque
        /// </summary>
        /// <param name="pProyecto">Proyecto</param>
        /// <param name="pIdBloque">ID del bloque</param>
        /// <param name="pIDComponente">ID del componente</param>
        public void AgregarComponenteABloque(ServiciosGenerales.Proyecto pProyecto, Guid pBloqueID, Guid pComponenteID, AD.EntityModel.Models.CMS.CMSBloque cmsBloque = null)
        {
            int numComponentesBloque = CMSDW.ListaCMSBloqueComponente.Count(item => item.BloqueID.Equals(pBloqueID));

            AD.EntityModel.Models.CMS.CMSBloqueComponente filaBloqueComponente = new AD.EntityModel.Models.CMS.CMSBloqueComponente();
            filaBloqueComponente.OrganizacionID = pProyecto.FilaProyecto.OrganizacionID;
            filaBloqueComponente.ProyectoID = pProyecto.FilaProyecto.ProyectoID;
            filaBloqueComponente.BloqueID = pBloqueID;
            if (cmsBloque != null)
            {
                filaBloqueComponente.CMSBloque = cmsBloque;
            }
            filaBloqueComponente.ComponenteID = pComponenteID;
            filaBloqueComponente.Orden = (short)numComponentesBloque;

            CMSDW.ListaCMSBloqueComponente.Add(filaBloqueComponente);
            mEntityContext.CMSBloqueComponente.Add(filaBloqueComponente);
        }

        /// <summary>
        /// Eliminar bloque
        /// </summary>
        public void EliminarBloque(Guid pBloqueID)
        {
            List<AD.EntityModel.Models.CMS.CMSBloqueComponente> filasBloquesComponentes = ListaBloques[pBloqueID].FilaBloque.CMSBloqueComponente.ToList();
            foreach (AD.EntityModel.Models.CMS.CMSBloqueComponente filaBloqueComponente in filasBloquesComponentes)
            {
                foreach (AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente filaPropiedad in filaBloqueComponente.CMSBloqueComponentePropiedadComponente)
                {
                    mEntityContext.EliminarElemento(filaPropiedad);
                }
                mEntityContext.EliminarElemento(filaBloqueComponente);
                ListaBloques[pBloqueID].FilaBloque.CMSBloqueComponente.Remove(filaBloqueComponente);
            }
            mEntityContext.EliminarElemento(ListaBloques[pBloqueID].FilaBloque);
            ListaBloques.Remove(pBloqueID);
        }

        /// <summary>
        /// Eliminar bloque
        /// </summary>
        private void ReordenarBloquesHijosDe(Guid pBloqueID)
        {
            List<AD.EntityModel.Models.CMS.CMSBloque> filasHermanas = CMSDW.ListaCMSBloque.Where(item => item.BloquePadreID.Value.Equals(pBloqueID)).OrderBy(item => item.BloquePadreID.Value).ToList();

            short i = 0;
            foreach (AD.EntityModel.Models.CMS.CMSBloque filaHijo in filasHermanas)
            {
                filaHijo.Orden = i;
                i++;
                ReordenarBloquesHijosDe(filaHijo.BloqueID);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
