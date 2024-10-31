using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.CMS;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Es.Riam.Gnoss.Web.Controles.Proyectos
{
    public class ControladorProyectoCMS : ControladorBase
    {
        #region Miembros

        /// <summary>
        /// Proyecto seleccionado
        /// </summary>
        private Proyecto mProyectoActual;

        /// <summary>
        /// Proyecto padre del proyecto seleccionado
        /// </summary>
        private Proyecto mProyectoPadre;

        /// <summary>
        /// Gestor del CMS del proyecto
        /// </summary>
        private GestionCMS mGestorCMS = null;

        /// <summary>
        /// DATASET con las personalizaciones del proyecto seleccionado
        /// </summary>
        private DataWrapperVistaVirtual mVistaVirtualDW = null;

        /// <summary>
        /// ID de la personalización del ecosistema
        /// </summary>
        private Guid? mPersonalizacionEcosistemaID = null;
        private bool? mComunidadExcluidaPersonalizacionEcosistema = null;

        public bool IgnorarErroresGrupos = false;
        public bool CrearFilasPropiedadesExportacion = false;
        private List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();
        public List<IntegracionContinuaPropiedad> FilasPropiedadesIntegracion = null;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;

        #endregion Miembros

        #region Metodos

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto seleccionado</param>
        public ControladorProyectoCMS(Guid pProyectoID, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;

            ProyectoCN proyCN = new ProyectoCN(entityContext, loggingService, configService, mServicesUtilVirtuosoAndReplication);
            GestionProyecto gestorProy = new GestionProyecto(proyCN.ObtenerProyectoPorID(pProyectoID), loggingService, entityContext);
            this.mProyectoActual = gestorProy.ListaProyectos[pProyectoID];

            if (mProyectoActual.FilaProyecto.ProyectoSuperiorID.HasValue)
            {
                GestionProyecto gestorProyPadre = new GestionProyecto(proyCN.ObtenerProyectoPorID(mProyectoActual.FilaProyecto.ProyectoSuperiorID.Value), loggingService, entityContext);
                this.mProyectoPadre = gestorProyPadre.ListaProyectos[mProyectoActual.FilaProyecto.ProyectoSuperiorID.Value];
            }
        }

        #region GeneracionXML

        /// <summary>
        /// Genera un XML a partir de los datos configurados del CMS en una comunidad
        /// </summary>
        public XmlDocument PintarEstructuraXMLCMS()
        {
            XmlDocument xmlDoc = new XmlDocument();
            // Write down the XML declaration
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);

            // Creamos el nodo raíz Comunidad
            XmlElement NodoComunidad = xmlDoc.CreateElement("Comunidad");
            xmlDoc.AppendChild(NodoComunidad);

            if (ProyectoActual != null)
            {
                AgregarNodoAlPadre(xmlDoc, NodoComunidad, "NombreComunidad", ProyectoActual.NombreCorto.ToLower());

                PintarComponentesPrivadosCMS(xmlDoc, NodoComunidad);
                PintarComponentesCMS(xmlDoc, NodoComunidad);
                PintarPaginasCMS(xmlDoc, NodoComunidad);
            }

            //if (CrearFilasPropiedadesExportacion)
            //{
            //    ProyectoCN proyCN = new ProyectoCN();
            //    proyCN.CrearFilasIntegracionContinuaParametro(propiedadesIntegracionContinua, ProyectoActual.Clave, false);
            //    proyCN.Dispose();
            //}

            return xmlDoc;
        }

        #region Componentes Privados CMS


        /// <summary>
        /// Pinta los componentes privados en el XML
        /// </summary>
        /// <param name="pXmlDoc">XML a generar</param>
        /// <param name="pNodoComunidad">Nodo padre para el nodo de componentes</param>
        private void PintarComponentesPrivadosCMS(XmlDocument pXmlDoc, XmlElement pNodoComunidad)
        {
            // Creamos el nodo Componentes Privados
            XmlElement NodoComponentesPrivados = pXmlDoc.CreateElement("ComponentesPrivados");
            pNodoComunidad.AppendChild(NodoComponentesPrivados);

            foreach (TipoComponenteCMS tipoComponente in gestorCMS.ListaComponentesPrivadosProyecto)
            {
                // Creamos el nodo Componente
                AgregarNodoAlPadre(pXmlDoc, NodoComponentesPrivados, "Componente", tipoComponente.ToString());
            }

        }

        #endregion

        #region Componentes CMS

        /// <summary>
        /// Pinta los componentes en el XML
        /// </summary>
        /// <param name="pXmlDoc">XML a generar</param>
        /// <param name="pNodoComunidad">Nodo padre para el nodo de componentes</param>
        private void PintarComponentesCMS(XmlDocument pXmlDoc, XmlElement pNodoComunidad)
        {
            // Creamos el nodo Componentes
            XmlElement NodoComponentes = pXmlDoc.CreateElement("Componentes");
            pNodoComunidad.AppendChild(NodoComponentes);

            foreach (Guid idComponenteCMS in gestorCMS.ListaComponentes.Keys)
            {
                PintarComponenteCMS(pXmlDoc, NodoComponentes, gestorCMS.ListaComponentes[idComponenteCMS]);
            }

        }

        /// <summary>
        /// Pinta un componente en el XML
        /// </summary>
        /// <param name="pXmlDoc">XML a generar</param>
        /// <param name="pNodoComponentes">Nodo padre para el nodo del componente</param>
        /// <param name="pComponente">Componente del CMS</param>
        private void PintarComponenteCMS(XmlDocument pXmlDoc, XmlElement pNodoComponentes, CMSComponente pComponente)
        {
            // Creamos el nodo Componente
            XmlElement NodoComponente = pXmlDoc.CreateElement("Componente");
            pNodoComponentes.AppendChild(NodoComponente);

            // Creamos el nodo Nombre
            AgregarNodoAlPadre(pXmlDoc, NodoComponente, "Nombre", pComponente.Nombre);
            // Creamos el nodo NombreCorto, si es vacio, pintamos el Identificador, para que no falle la subida
            if (string.IsNullOrEmpty(pComponente.NombreCortoComponente.ToString()))
            {
                pComponente.NombreCortoComponente = pComponente.Clave.ToString();
            }
            AgregarNodoAlPadre(pXmlDoc, NodoComponente, "NombreCorto", pComponente.NombreCortoComponente);
            // Creamos el nodo TipoComponente
            AgregarNodoAlPadre(pXmlDoc, NodoComponente, "TipoComponente", pComponente.TipoComponenteCMS.ToString());
            // Creamos el nodo Activo
            AgregarNodoAlPadre(pXmlDoc, NodoComponente, "Activo", pComponente.Activo.ToString());

            if (pComponente.AccesoPublico)
            {
                // Creamos el nodo AccesoPublico
                AgregarNodoAlPadre(pXmlDoc, NodoComponente, "AccesoPublico", pComponente.Activo.ToString());
            }

            if (!string.IsNullOrEmpty(pComponente.Estilos))
            {
                // Creamos el nodo Estilos (si tiene estilos)
                AgregarNodoAlPadre(pXmlDoc, NodoComponente, "Estilos", pComponente.Estilos);
            }
            if (UtilComponentes.CaducidadesDisponiblesPorTipoComponente[pComponente.TipoComponenteCMS].Count > 1)
            {
                // Creamos el nodo Caducidad (si hay disponible más de una caducidad para ese tipo de componente)
                AgregarNodoAlPadre(pXmlDoc, NodoComponente, "Caducidad", pComponente.TipoCaducidadComponenteCMS.ToString());
            }

            if (!string.IsNullOrEmpty(pComponente.FilaComponente.IdiomasDisponibles))
            {
                // Creamos el nodo IdiomasDisponibles (si los tiene tiene)
                AgregarNodoAlPadre(pXmlDoc, NodoComponente, "IdiomasDisponibles", pComponente.FilaComponente.IdiomasDisponibles);
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (pComponente.ListaRolIdentidad.Count > 0 || pComponente.ListaRolGrupoIdentidades.Count > 0)
            {
                XmlElement NodoConfiguracionPrivacidad = pXmlDoc.CreateElement("ConfiguracionPrivacidad");
                NodoComponente.AppendChild(NodoConfiguracionPrivacidad);

                // Creamos el nodo PerfilesPrivados                    
                if (pComponente.ListaRolIdentidad.Count > 0)
                {
                    XmlElement NodoPrivacidadPerfiles = pXmlDoc.CreateElement("PrivacidadPerfiles");
                    NodoConfiguracionPrivacidad.AppendChild(NodoPrivacidadPerfiles);

                    foreach (Guid idPerfil in pComponente.ListaRolIdentidad.Keys)
                    {
                        AgregarNodoAlPadre(pXmlDoc, NodoPrivacidadPerfiles, "Perfil", identidadCN.ObtenerNombreCortoPerfil(idPerfil).Key);
                    }
                }

                // Creamos el nodo GruposPrivados                    
                if (pComponente.ListaRolGrupoIdentidades.Count > 0)
                {
                    XmlElement NodoPrivacidadGrupos = pXmlDoc.CreateElement("PrivacidadGrupos");
                    NodoConfiguracionPrivacidad.AppendChild(NodoPrivacidadGrupos);

                    List<Guid> listaGrupos = new List<Guid>();
                    foreach (Guid idGrupo in pComponente.ListaRolGrupoIdentidades.Keys)
                    {
                        listaGrupos.Add(idGrupo);
                    }
                    GestionIdentidades gestorIden = new GestionIdentidades(identidadCN.ObtenerGruposPorIDGrupo(listaGrupos, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                    foreach (Guid idGRupo in gestorIden.ListaGrupos.Keys)
                    {
                        GrupoIdentidades grupoIdentidad = gestorIden.ListaGrupos[idGRupo];
                        if (grupoIdentidad.EsGrupoDeProyecto)
                        {
                            XmlElement NodoGrupoProy = pXmlDoc.CreateElement("GrupoProy");
                            NodoPrivacidadGrupos.AppendChild(NodoGrupoProy);

                            AgregarNodoAlPadre(pXmlDoc, NodoGrupoProy, "NombreCortoGrupo", identidadCN.ObtenerNombreCortoGrupoPorID(grupoIdentidad.Clave));
                            AgregarNodoAlPadre(pXmlDoc, NodoGrupoProy, "NombreCortoProy", proyectoCN.ObtenerNombreCortoProyecto(grupoIdentidad.FilaGrupoProyecto.ProyectoID));
                        }
                        else
                        {
                            XmlElement NodoGrupoOrg = pXmlDoc.CreateElement("GrupoOrg");
                            NodoPrivacidadGrupos.AppendChild(NodoGrupoOrg);

                            AgregarNodoAlPadre(pXmlDoc, NodoGrupoOrg, "NombreCortoGrupo", identidadCN.ObtenerNombreCortoGrupoPorID(grupoIdentidad.Clave));
                            AgregarNodoAlPadre(pXmlDoc, NodoGrupoOrg, "NombreCortoOrg", organizacionCN.ObtenerNombreOrganizacionPorID(grupoIdentidad.FilaGrupoOrganizacion.OrganizacionID).NombreCorto);
                        }
                    }
                }
            }
            identidadCN.Dispose();
            proyectoCN.Dispose();
            organizacionCN.Dispose();

            // Creamos el nodo Propiedades
            XmlElement NodoPropiedades = pXmlDoc.CreateElement("Propiedades");
            NodoComponente.AppendChild(NodoPropiedades);

            if (pComponente.PropiedadesComponente.ContainsKey(TipoPropiedadCMS.Personalizacion))
            {
                string valorPropiedad = pComponente.PropiedadesComponente[TipoPropiedadCMS.Personalizacion];
                PintarPropiedadComponenteCMS(pXmlDoc, NodoPropiedades, pComponente, TipoPropiedadCMS.Personalizacion, valorPropiedad);
            }

            foreach (TipoPropiedadCMS propiedad in UtilComponentes.PropiedadesDisponiblesPorTipoComponente[pComponente.TipoComponenteCMS].Keys)
            {
                if (pComponente.PropiedadesComponente.ContainsKey(propiedad))
                {
                    string valorPropiedad = pComponente.PropiedadesComponente[propiedad];
                    PintarPropiedadComponenteCMS(pXmlDoc, NodoPropiedades, pComponente, propiedad, valorPropiedad);

                    if (CrearFilasPropiedadesExportacion)
                    {
                        //if (propiedad.Equals(TipoPropiedadCMS.HTML) && (valorPropiedad.Contains("http://") || valorPropiedad.Contains("https://")))
                        //{
                        //    //Crear las filas de las porpiedades de Integracion Continua
                        //    IntegracionContinuaPropiedad propiedadCMS = new IntegracionContinuaPropiedad();
                        //    propiedadCMS.ProyectoID = mProyectoActual.Clave;
                        //    propiedadCMS.TipoObjeto = (short)TipoObjeto.Componente;
                        //    propiedadCMS.ObjetoPropiedad = pComponente.NombreCortoComponente;
                        //    propiedadCMS.TipoPropiedad = (short)TipoPropiedad.HtmlComponente;
                        //    propiedadCMS.ValorPropiedad = valorPropiedad;
                        //    propiedadesIntegracionContinua.Add(propiedadCMS);
                        //}
                        //else if (propiedad.Equals(TipoPropiedadCMS.URLBusqueda))
                        //{
                        //    //Crear las filas de las porpiedades de Integracion Continua
                        //    IntegracionContinuaPropiedad propiedadCMS = new IntegracionContinuaPropiedad();
                        //    propiedadCMS.ProyectoID = mProyectoActual.Clave;
                        //    propiedadCMS.TipoObjeto = (short)TipoObjeto.Componente;
                        //    propiedadCMS.ObjetoPropiedad = pComponente.NombreCortoComponente;
                        //    propiedadCMS.TipoPropiedad = (short)TipoPropiedad.UrlBusquedaComponente;
                        //    propiedadCMS.ValorPropiedad = valorPropiedad;
                        //    propiedadesIntegracionContinua.Add(propiedadCMS);
                        //}
                        //else if (propiedad.Equals(TipoPropiedadCMS.URLVerMas))
                        //{
                        //    //Crear las filas de las porpiedades de Integracion Continua
                        //    IntegracionContinuaPropiedad propiedadCMS = new IntegracionContinuaPropiedad();
                        //    propiedadCMS.ProyectoID = mProyectoActual.Clave;
                        //    propiedadCMS.TipoObjeto = (short)TipoObjeto.Componente;
                        //    propiedadCMS.ObjetoPropiedad = pComponente.NombreCortoComponente;
                        //    propiedadCMS.TipoPropiedad = (short)TipoPropiedad.UrlVerMasComponente;
                        //    propiedadCMS.ValorPropiedad = valorPropiedad;
                        //    propiedadesIntegracionContinua.Add(propiedadCMS);
                        //}
                        if (pComponente.TipoComponenteCMS.Equals(TipoComponenteCMS.ListadoEstatico) && propiedad.Equals(TipoPropiedadCMS.ListaIDs))
                        {
                            //Crear las filas de las porpiedades de Integracion Continua
                            IntegracionContinuaPropiedad propiedadCMS = new IntegracionContinuaPropiedad();
                            propiedadCMS.ProyectoID = mProyectoActual.Clave;
                            propiedadCMS.TipoObjeto = (short)TipoObjeto.Componente;
                            propiedadCMS.ObjetoPropiedad = pComponente.NombreCortoComponente;
                            propiedadCMS.TipoPropiedad = (short)TipoPropiedad.IdsRecursosComponente;
                            propiedadCMS.ValorPropiedad = valorPropiedad;
                            propiedadesIntegracionContinua.Add(propiedadCMS);
                        }
                        else if (pComponente.TipoComponenteCMS.Equals(TipoComponenteCMS.Tesauro) && propiedad.Equals(TipoPropiedadCMS.ElementoID))
                        {
                            //Crear las filas de las porpiedades de Integracion Continua
                            IntegracionContinuaPropiedad propiedadCMS = new IntegracionContinuaPropiedad();
                            propiedadCMS.ProyectoID = mProyectoActual.Clave;
                            propiedadCMS.TipoObjeto = (short)TipoObjeto.Componente;
                            propiedadCMS.ObjetoPropiedad = pComponente.NombreCortoComponente;
                            propiedadCMS.TipoPropiedad = (short)TipoPropiedad.TesauroComponente;
                            propiedadCMS.ValorPropiedad = valorPropiedad;
                            propiedadesIntegracionContinua.Add(propiedadCMS);
                        }

                        /*
                            
                            Html => Si tiene urls dentro del contenido
                            Buscador => Filtro, url de busqueda
                            Faceta => Filtro, url de busqueda
                            Listado Dinamico => Filtro, url de busqueda
                            Caja Buscador => Filtro, url de busqueda
                            Listado Estatico => Ids de los recursos
                            Tesauro => Ids de la categoria
                    
                        */
                    }
                }
            }
        }

        /// <summary>
        /// Pinta una propiedad de un componente del CMS
        /// </summary>
        /// <param name="pXmlDoc">XML a generar</param>
        /// <param name="pNodoPropiedades">Nodo padre para el nodo de la propiedad</param>
        /// <param name="pTipoPropiedad">Tipo de propiedad del CMS</param>
        /// <param name="pValorPropiedad">Valor de la propiedad del CMS</param>
        private void PintarPropiedadComponenteCMS(XmlDocument pXmlDoc, XmlElement pNodoPropiedades, CMSComponente pComponente, TipoPropiedadCMS pTipoPropiedad, string pValorPropiedad)
        {
            switch (pTipoPropiedad)
            {
                case TipoPropiedadCMS.TipoActividadRecienteCMS:
                    TipoActividadReciente tipoActividad = (TipoActividadReciente)(short.Parse(pValorPropiedad));
                    pValorPropiedad = tipoActividad.ToString();
                    break;
                case TipoPropiedadCMS.TipoListadoProyectos:
                    TipoListadoProyectosCMS tipoListadoProyectos = (TipoListadoProyectosCMS)(short.Parse(pValorPropiedad));
                    pValorPropiedad = tipoListadoProyectos.ToString();
                    break;
                case TipoPropiedadCMS.TipoListadoUsuarios:
                    TipoListadoUsuariosCMS tipoListadoUsuarios = (TipoListadoUsuariosCMS)(short.Parse(pValorPropiedad));
                    pValorPropiedad = tipoListadoUsuarios.ToString();
                    break;
                case TipoPropiedadCMS.TipoPresentacionFaceta:
                    TipoPresentacionFacetas tipoPresentacionFacetas = (TipoPresentacionFacetas)(short.Parse(pValorPropiedad));
                    pValorPropiedad = tipoPresentacionFacetas.ToString();
                    break;
                case TipoPropiedadCMS.TipoPresentacionGrupoComponentes:
                    pValorPropiedad = ObtenerValorPropiedadTipoPresentacionGrupoComponentesDeBBDDaXML(pValorPropiedad);
                    break;
                case TipoPropiedadCMS.TipoPresentacionListadoRecursos:
                    pValorPropiedad = ObtenerValorPropiedadTipoPresentacionListadoRecursosDeBBDDaXML(pValorPropiedad);
                    break;
                case TipoPropiedadCMS.TipoPresentacionListadoUsuarios:
                    TipoPresentacionListadoUsuariosCMS tipoPresentacionListadoUsuariosCMS = (TipoPresentacionListadoUsuariosCMS)(short.Parse(pValorPropiedad));
                    pValorPropiedad = tipoPresentacionListadoUsuariosCMS.ToString();
                    break;
                case TipoPropiedadCMS.TipoPresentacionRecurso:
                    pValorPropiedad = ObtenerValorPropiedadTipoPresentacionRecursoDeBBDDaXML(pValorPropiedad);
                    break;
                case TipoPropiedadCMS.ListaIDs:
                    if (pComponente.TipoComponenteCMS == TipoComponenteCMS.GrupoComponentes)
                    {
                        //Convertimos de IDs a nombrescortos
                        string[] idComponentes = pValorPropiedad.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        pValorPropiedad = "";
                        foreach (string idComponente in idComponentes)
                        {
                            Guid componenteID = new Guid(idComponente);
                            try
                            {
                                pValorPropiedad += gestorCMS.ListaComponentes[componenteID].NombreCortoComponente + ",";
                            }
                            catch
                            {
                                pValorPropiedad += idComponente + ",";
                            }
                        }
                    }
                    break;
            }

            // Creamos el nodo Propiedad
            XmlElement NodoPropiedad = pXmlDoc.CreateElement("Propiedad");
            pNodoPropiedades.AppendChild(NodoPropiedad);

            // Creamos la clave de la propiedad
            AgregarNodoAlPadre(pXmlDoc, NodoPropiedad, "Clave", pTipoPropiedad.ToString());

            try
            {
                // Creamos el valor de la propiedad
                AgregarNodoAlPadre(pXmlDoc, NodoPropiedad, "Valor", pValorPropiedad, true);
            }
            catch (Exception ex)
            {
                string mensaje = $"Error al crear el xml del CMS. ComponenteID: {pComponente.Clave} Nombre: {pComponente.Nombre} Dato que contiene el error: \r\n{pValorPropiedad}";
                throw new ExcepcionWeb(mensaje, ex);
            }
        }

        /// <summary>
        /// Obtiene el valor de la propiedad TipoPresentacionGrupoComponentes teniendo en cuenta la personalización de las vistas
        /// </summary>
        /// <param name="pValorPropiedad">Valor de la propiedad en BBDD</param>
        /// <returns>Valor de la propiedad para el XML</returns>
        private string ObtenerValorPropiedadTipoPresentacionGrupoComponentesDeBBDDaXML(string pValorPropiedad)
        {
            #region Personalizacion
            string vistasGruposComponmentes = "/Views/CMSPagina/GrupoComponentes/";

            List<VistaVirtualCMS> filasGruposComponmentes = VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasGruposComponmentes)).ToList();

            Dictionary<TipoPresentacionGrupoComponentesCMS, string> diccionarioNombresGenericosVistaGrupos = new Dictionary<TipoPresentacionGrupoComponentesCMS, string>();
            Dictionary<Guid, string> diccionarioNombresPersonalizacionesVistaGrupos = new Dictionary<Guid, string>();
            if (filasGruposComponmentes.Count > 0)
            {
                foreach (VistaVirtualCMS filaVistaVirtualCMS in VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasGruposComponmentes)))
                {
                    string nombre = filaVistaVirtualCMS.Nombre;
                    if (string.IsNullOrEmpty(nombre))
                    {
                        nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                    }
                    bool agregado = false;
                    foreach (TipoPresentacionGrupoComponentesCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionGrupoComponentesCMS)))
                    {
                        if (filaVistaVirtualCMS.TipoComponente.EndsWith(tipoPresentacion.ToString() + ".cshtml"))
                        {
                            diccionarioNombresGenericosVistaGrupos.Add(tipoPresentacion, nombre);
                            agregado = true;
                            break;
                        }
                    }
                    if (!agregado)
                    {
                        diccionarioNombresPersonalizacionesVistaGrupos.Add(filaVistaVirtualCMS.PersonalizacionComponenteID, nombre);
                    }
                }
            }
            #endregion

            foreach (TipoPresentacionGrupoComponentesCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionGrupoComponentesCMS)))
            {
                if (pValorPropiedad == ((short)tipoPresentacion).ToString())
                {
                    if (diccionarioNombresGenericosVistaGrupos.ContainsKey(tipoPresentacion))
                    {
                        pValorPropiedad = diccionarioNombresGenericosVistaGrupos[tipoPresentacion];
                    }
                    else
                    {
                        pValorPropiedad = tipoPresentacion.ToString();
                    }
                    break;
                }
            }

            foreach (Guid idPersonalizacion in diccionarioNombresPersonalizacionesVistaGrupos.Keys)
            {
                if (pValorPropiedad.ToLower() == idPersonalizacion.ToString().ToLower())
                {
                    pValorPropiedad = diccionarioNombresPersonalizacionesVistaGrupos[idPersonalizacion];
                }
            }
            return pValorPropiedad;
        }

        /// <summary>
        /// Obtiene el valor de la propiedad TipoPresentacionListadoRecursos teniendo en cuenta la personalización de las vistas
        /// </summary>
        /// <param name="pValorPropiedad">Valor de la propiedad en BBDD</param>
        /// <returns>Valor de la propiedad para el XML</returns>
        private string ObtenerValorPropiedadTipoPresentacionListadoRecursosDeBBDDaXML(string pValorPropiedad)
        {
            #region Personalizacion
            string vistasListadoRecursos = "/Views/CMSPagina/ListadoRecursos/";

            List<VistaVirtualCMS> filasListadoRecursos = VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasListadoRecursos)).ToList();

            Dictionary<TipoPresentacionListadoRecursosCMS, string> diccionarioNombresListadoGenericos = new Dictionary<TipoPresentacionListadoRecursosCMS, string>();
            Dictionary<Guid, string> diccionarioNombresListadoPersonalizaciones = new Dictionary<Guid, string>();
            if (vistasListadoRecursos.Length > 0)
            {
                foreach (VistaVirtualCMS filaVistaVirtualCMS in VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasListadoRecursos)))
                {
                    if (filaVistaVirtualCMS.TipoComponente.Replace(vistasListadoRecursos, "").IndexOf("/") == -1)
                    {
                        string nombre = filaVistaVirtualCMS.Nombre;
                        if (string.IsNullOrEmpty(nombre))
                        {
                            nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                        }
                        bool agregado = false;
                        foreach (TipoPresentacionListadoRecursosCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionListadoRecursosCMS)))
                        {
                            if (filaVistaVirtualCMS.TipoComponente.EndsWith(tipoPresentacion.ToString() + ".cshtml"))
                            {
                                diccionarioNombresListadoGenericos.Add(tipoPresentacion, nombre);
                                agregado = true;
                                break;
                            }
                        }
                        if (!agregado)
                        {
                            diccionarioNombresListadoPersonalizaciones.Add(filaVistaVirtualCMS.PersonalizacionComponenteID, nombre);
                        }
                    }
                }
            }
            #endregion

            foreach (TipoPresentacionListadoRecursosCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionListadoRecursosCMS)))
            {
                if (pValorPropiedad == ((short)tipoPresentacion).ToString())
                {
                    if (diccionarioNombresListadoGenericos.ContainsKey(tipoPresentacion))
                    {
                        pValorPropiedad = diccionarioNombresListadoGenericos[tipoPresentacion];
                    }
                    else
                    {
                        pValorPropiedad = tipoPresentacion.ToString();
                    }
                    break;
                }
            }

            foreach (Guid idPersonalizacion in diccionarioNombresListadoPersonalizaciones.Keys)
            {
                if (pValorPropiedad.ToLower() == idPersonalizacion.ToString().ToLower())
                {
                    pValorPropiedad = diccionarioNombresListadoPersonalizaciones[idPersonalizacion];
                }
            }
            return pValorPropiedad;
        }

        /// <summary>
        /// Obtiene el valor de la propiedad TipoPresentacionRecurso teniendo en cuenta la personalización de las vistas
        /// </summary>
        /// <param name="pValorPropiedad">Valor de la propiedad en BBDD</param>
        /// <returns>Valor de la propiedad para el XML</returns>
        private string ObtenerValorPropiedadTipoPresentacionRecursoDeBBDDaXML(string pValorPropiedad)
        {
            #region Personalizacion
            string vistasRecursos = "/Views/CMSPagina/ListadoRecursos/Vistas/";

            List<VistaVirtualCMS> filasRecursos = VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasRecursos)).ToList();

            Dictionary<TipoPresentacionRecursoCMS, string> diccionarioNombresGenericos = new Dictionary<TipoPresentacionRecursoCMS, string>();
            Dictionary<Guid, string> diccionarioNombresPersonalizaciones = new Dictionary<Guid, string>();
            if (filasRecursos.Count > 0)
            {
                foreach (VistaVirtualCMS filaVistaVirtualCMS in VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasRecursos)))
                {
                    string nombre = filaVistaVirtualCMS.Nombre;
                    if (string.IsNullOrEmpty(nombre))
                    {
                        nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                    }
                    bool agregado = false;
                    foreach (TipoPresentacionRecursoCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionRecursoCMS)))
                    {
                        if (filaVistaVirtualCMS.TipoComponente.EndsWith(tipoPresentacion.ToString() + ".cshtml"))
                        {
                            diccionarioNombresGenericos.Add(tipoPresentacion, nombre);
                            agregado = true;
                            break;
                        }
                    }
                    if (!agregado)
                    {
                        diccionarioNombresPersonalizaciones.Add(filaVistaVirtualCMS.PersonalizacionComponenteID, nombre);
                    }
                }
            }
            #endregion

            foreach (TipoPresentacionRecursoCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionRecursoCMS)))
            {
                if (pValorPropiedad == ((short)tipoPresentacion).ToString())
                {
                    if (diccionarioNombresGenericos.ContainsKey(tipoPresentacion))
                    {
                        pValorPropiedad = diccionarioNombresGenericos[tipoPresentacion];
                    }
                    else
                    {
                        pValorPropiedad = tipoPresentacion.ToString();
                    }
                    break;
                }
            }

            foreach (Guid idPersonalizacion in diccionarioNombresPersonalizaciones.Keys)
            {
                if (pValorPropiedad.ToLower() == idPersonalizacion.ToString().ToLower())
                {
                    pValorPropiedad = diccionarioNombresPersonalizaciones[idPersonalizacion];
                }
            }
            return pValorPropiedad;
        }

        #endregion

        #region Páginas CMS

        /// <summary>
        /// Pinta las páginas en el XML
        /// </summary>
        /// <param name="pXmlDoc">XML a generar</param>
        /// <param name="pNodoComunidad">Nodo padre para el nodo de páginas</param>
        private void PintarPaginasCMS(XmlDocument pXmlDoc, XmlElement pNodoComunidad)
        {
            if (gestorCMS.ListaPaginasProyectos.ContainsKey(ProyectoActual.Clave))
            {
                // Creamos el nodo Paginas
                XmlElement NodoPaginas = pXmlDoc.CreateElement("Paginas");
                pNodoComunidad.AppendChild(NodoPaginas);

                foreach (short ubicacionPagina in gestorCMS.ListaPaginasProyectos[ProyectoActual.Clave].Keys)
                {
                    PintarPaginaCMS(pXmlDoc, NodoPaginas, ubicacionPagina);
                }
            }
        }

        /// <summary>
        /// Pinta un componente en el XML
        /// </summary>
        /// <param name="pXmlDoc">XML a generar</param>
        /// <param name="pNodoPaginas">Nodo padre para el nodo de la página</param>
        /// <param name="pUbicacion">Ubicacion de la página del CMS</param>
        private void PintarPaginaCMS(XmlDocument pXmlDoc, XmlElement pNodoPaginas, short pUbicacion)
        {
            CMSPagina PaginaActual = gestorCMS.ListaPaginasProyectos[ProyectoActual.Clave][pUbicacion];

            // Creamos el nodo Pagina
            XmlElement NodoPagina = pXmlDoc.CreateElement("Pagina");
            pNodoPaginas.AppendChild(NodoPagina);

            // Creamos el nodo Ubicación
            AgregarNodoAlPadre(pXmlDoc, NodoPagina, "Ubicacion", pUbicacion.ToString());
            // Creamos el nodo Activo
            AgregarNodoAlPadre(pXmlDoc, NodoPagina, "Activa", PaginaActual.Activa.ToString());
            if (PaginaActual.MostrarSoloCuerpo)
            {
                // Creamos el nodo Mostrar solo cuerpo
                AgregarNodoAlPadre(pXmlDoc, NodoPagina, "MostrarSoloCuerpo", PaginaActual.MostrarSoloCuerpo.ToString());
            }

            CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestionCMS GestorCMSPaginaActual = new GestionCMS(cmsCN.ObtenerCMSDeUbicacionDeProyecto(pUbicacion, ProyectoActual.Clave, 2, false), mLoggingService, mEntityContext);
            cmsCN.Dispose();

            // Creamos el nodo Bloques
            XmlElement NodoBloques = pXmlDoc.CreateElement("Bloques");
            NodoPagina.AppendChild(NodoBloques);
            foreach (CMSBloque bloque in GestorCMSPaginaActual.ListaBloques.Values.OrderBy(item => item.Orden))
            {
                if (!bloque.BloquePadreID.HasValue)
                {
                    PintarBloquePaginaCMS(pXmlDoc, NodoBloques, bloque);
                }
            }
        }

        /// <summary>
        /// Pinta un bloque en el XML
        /// </summary>
        /// <param name="pXmlDoc">XML a generar</param>
        /// <param name="pNodoBloques">Nodo padre para el nodo del bloque</param>
        /// <param name="CMSBloque">Bloque de la página del CMS</param>
        private void PintarBloquePaginaCMS(XmlDocument pXmlDoc, XmlElement pNodoBloques, CMSBloque pBloque)
        {
            // Creamos el nodo Bloque
            XmlElement NodoBloque = pXmlDoc.CreateElement("Bloque");
            pNodoBloques.AppendChild(NodoBloque);

            if (pBloque.Atributos.Count > 0)
            {
                // Creamos el nodo Atributos
                XmlElement NodoAtributos = pXmlDoc.CreateElement("Atributos");
                NodoBloque.AppendChild(NodoAtributos);
                foreach (string claveAtrib in pBloque.Atributos.Keys)
                {
                    // Creamos el nodo Atributo
                    XmlElement NodoAtributo = pXmlDoc.CreateElement("Atributo");
                    NodoAtributos.AppendChild(NodoAtributo);
                    // Creamos el nodo Clave
                    AgregarNodoAlPadre(pXmlDoc, NodoAtributo, "Clave", claveAtrib);
                    // Creamos el nodo Valor
                    AgregarNodoAlPadre(pXmlDoc, NodoAtributo, "Valor", pBloque.Atributos[claveAtrib]);
                }
            }

            if (pBloque.Hijos.Count > 0)
            {
                // Creamos el nodo Bloques
                XmlElement NodoBloques = pXmlDoc.CreateElement("Bloques");
                NodoBloque.AppendChild(NodoBloques);

                foreach (CMSBloque bloque in pBloque.Hijos)
                {
                    PintarBloquePaginaCMS(pXmlDoc, NodoBloques, bloque);
                }
            }

            if (pBloque.Componentes.Count > 0)
            {
                // Creamos el nodo Componentes
                XmlElement NodoComponentes = pXmlDoc.CreateElement("Componentes");
                NodoBloque.AppendChild(NodoComponentes);
                foreach (CMSComponente componente in pBloque.Componentes.Values)
                {
                    // Creamos el nodo Componente
                    XmlElement nodo = AgregarNodoAlPadre(pXmlDoc, NodoComponentes, "Componente", componente.NombreCortoComponente.ToString());
                    if (pBloque.PropiedadesComponentesBloque.ContainsKey(componente.Clave))
                    {
                        Dictionary<TipoPropiedadCMS, string> listaCMSBloqueComponentePropiedad = pBloque.PropiedadesComponentesBloque[componente.Clave];
                        foreach (TipoPropiedadCMS tipoPropiedad in listaCMSBloqueComponentePropiedad.Keys)
                        {
                            string valorPropiedad = listaCMSBloqueComponentePropiedad[tipoPropiedad];
                            nodo.SetAttribute(tipoPropiedad.ToString(), valorPropiedad);
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Agrega un nodo a un XML
        /// </summary>
        /// <param name="pXmlDocument">XML a generar</param>
        /// <param name="pNodoPadre">Nodo padre</param>
        /// <param name="pNodoKey">Nuevo nodo</param>
        /// <param name="pNodoValue">Valor del nuevo nodo</param>
        /// <param name="pUsarCDATA">Indica si se debe usar CDATA</param>
        public XmlElement AgregarNodoAlPadre(XmlDocument pXmlDocument, XmlElement pNodoPadre, string pNodoKey, string pNodoValue, bool pUsarCDATA = false)
        {
            XmlElement nodo = pXmlDocument.CreateElement(pNodoKey);
            pNodoPadre.AppendChild(nodo);

            if (pUsarCDATA)
            {
                pNodoValue = "<![CDATA[" + pNodoValue + "]]>";
                nodo.InnerXml = pNodoValue;
            }
            else
            {
                nodo.InnerText = pNodoValue;
            }
            return nodo;
        }

        #endregion

        #region Volcado a BBDD

        /// <summary>
        /// Configura el CMS de una comunidad a partir de los datos recibidos en un documento XML
        /// </summary>
        /// <param name="pXML">XML</param>
        public void ConfigurarCMSComunidadConXML(string pXML)
        {
            if (string.IsNullOrEmpty(pXML))
            {
                throw new Exception("El fichero no puede estar vacío");
            }
            //cargo el XML
            XmlDocument configXML = new XmlDocument();
            configXML.LoadXml(pXML);

            XmlNode nodoNombreComunidad = configXML.SelectSingleNode("/Comunidad/NombreComunidad");
            if (nodoNombreComunidad != null)
            {
                string nombreComunidad = nodoNombreComunidad.InnerText.ToLower();
                if (string.IsNullOrEmpty(nombreComunidad) || !nombreComunidad.Equals(ProyectoActual.NombreCorto.ToLower()))
                {
                    throw new Exception("Está subiendo el Xml de configuración de otra comunidad");
                }
            }
            else
            {
                throw new Exception("Debe configurar el NombreComunidad");
            }

            ActualizarCMS(configXML);
            CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            cmsCN.ActualizarCMS(gestorCMS.CMSDW);
            cmsCN.Dispose();

            CMSCL cmsCL = new CMSCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            cmsCL.InvalidarCachesCMSDeUbicacionesDeProyecto(ProyectoActual.Clave);
            cmsCL.InvalidarCachesDeComponentesEnProyecto(ProyectoActual.Clave);
            cmsCL.Dispose();

            mGnossCache.VersionarCacheLocal(ProyectoActual.Clave);
        }

        /// <summary>
        /// Crea las nuevas filas en las tablas de CMS en función del XML
        /// </summary>
        /// <param name="pConfigXML">XMl de configuación del CMS</param>
        private void ActualizarCMS(XmlDocument pConfigXML)
        {
            #region Componentes Privados
            XmlNodeList componentesPrivados = pConfigXML.SelectNodes("/Comunidad/ComponentesPrivados/Componente");
            //Chequeo de datos
            List<TipoComponenteCMS> listaComponentesPrivados = new List<TipoComponenteCMS>();
            foreach (XmlElement componente in componentesPrivados)
            {
                if (!string.IsNullOrEmpty(componente.InnerText))
                {
                    TipoComponenteCMS TipoComponente;
                    try
                    {
                        TipoComponente = (TipoComponenteCMS)Enum.Parse(typeof(TipoComponenteCMS), componente.InnerText);
                        if (!listaComponentesPrivados.Contains(TipoComponente))
                        {
                            listaComponentesPrivados.Add(TipoComponente);
                        }
                    }
                    catch
                    {
                        throw new Exception("El tipo de componente " + componente.InnerText + " no es válido");
                    }
                }
                else
                {
                    throw new Exception("Los componentes que están dentro de 'ComponentesPrivados' no pueden estar vacios");
                }
            }

            List<AD.EntityModel.Models.CMS.CMSComponentePrivadoProyecto> filasTipoComponente = new List<AD.EntityModel.Models.CMS.CMSComponentePrivadoProyecto>();
            foreach (TipoComponenteCMS tipoComponente in listaComponentesPrivados)
            {
                AD.EntityModel.Models.CMS.CMSComponentePrivadoProyecto fila = gestorCMS.CMSDW.ListaCMSComponentePrivadoProyecto.Where(item => item.OrganizacionID.Equals(ProyectoActual.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoActual.Clave) && item.TipoComponente.Equals((short)tipoComponente)).FirstOrDefault();

                if (fila == null)
                {
                    fila = new AD.EntityModel.Models.CMS.CMSComponentePrivadoProyecto();
                    fila.OrganizacionID = ProyectoActual.FilaProyecto.OrganizacionID;
                    fila.ProyectoID = ProyectoActual.Clave;
                    fila.TipoComponente = (short)tipoComponente;

                    gestorCMS.CMSDW.ListaCMSComponentePrivadoProyecto.Add(fila);
                    mEntityContext.CMSComponentePrivadoProyecto.Add(fila);
                }

                filasTipoComponente.Add(fila);
            }

            foreach (var fila in gestorCMS.CMSDW.ListaCMSComponentePrivadoProyecto.ToList())
            {
                if (!filasTipoComponente.Contains(fila))
                {
                    gestorCMS.CMSDW.ListaCMSComponentePrivadoProyecto.Remove(fila);
                    mEntityContext.EliminarElemento(fila);
                }
            }

            #endregion

            #region Componentes
            XmlNodeList componentes = pConfigXML.SelectNodes("/Comunidad/Componentes/Componente");
            //Chequeo de datos
            Dictionary<string, Guid> listaComponentes = new Dictionary<string, Guid>();
            foreach (XmlElement componente in componentes)
            {
                //Comprobamos los nombrecortos
                string nombrecorto = (string)LeerNodo(componente, "NombreCorto", typeof(string));
                if (!string.IsNullOrEmpty(nombrecorto))
                {
                    if (listaComponentes.ContainsKey(nombrecorto))
                    {
                        throw new Exception("No puede haber dos componentes con el mismo nombrecorto");
                    }
                    else
                    {
                        AD.EntityModel.Models.CMS.CMSComponente filaComponente = gestorCMS.CMSDW.ListaCMSComponente.Where(item => item.NombreCortoComponente.Equals(nombrecorto)).FirstOrDefault();
                        if (filaComponente != null)
                        {
                            listaComponentes.Add(nombrecorto, filaComponente.ComponenteID);
                        }
                        else
                        {
                            listaComponentes.Add(nombrecorto, Guid.NewGuid());
                        }
                    }
                }
                else
                {
                    throw new Exception("El nombrecorto de un componente no puede estar vacío");
                }
            }

            //Lista de componentes
            Dictionary<string, AD.EntityModel.Models.CMS.CMSComponente> listaFilasComponentesPorNombreCorto = new Dictionary<string, AD.EntityModel.Models.CMS.CMSComponente>();
            foreach (XmlElement componente in componentes)
            {
                string nombrecorto = (string)LeerNodo(componente, "NombreCorto", typeof(string));
                listaFilasComponentesPorNombreCorto.Add(nombrecorto, CrearComponenteCMS(componente, listaComponentes));
            }

            foreach (AD.EntityModel.Models.CMS.CMSComponente fila in gestorCMS.CMSDW.ListaCMSComponente.ToList())
            {
                if (!listaFilasComponentesPorNombreCorto.ContainsValue(fila))
                {
                    foreach (AD.EntityModel.Models.CMS.CMSBloqueComponente filaBloqueComponente in gestorCMS.CMSDW.ListaCMSBloqueComponente.Where(item => item.ComponenteID.Equals(fila.ComponenteID)).ToList())
                    {
                        foreach (AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente filaBloqueComponentePropiedadComponente in gestorCMS.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Where(item => item.BloqueID.Equals(filaBloqueComponente.BloqueID)).ToList())
                        {
                            mEntityContext.EliminarElemento(filaBloqueComponentePropiedadComponente);
                            gestorCMS.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Remove(filaBloqueComponentePropiedadComponente);
                        }

                        mEntityContext.EliminarElemento(filaBloqueComponente);
                        gestorCMS.CMSDW.ListaCMSBloqueComponente.Remove(filaBloqueComponente);
                    }

                    foreach (AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad filaRolIdentidad in gestorCMS.CMSDW.ListaCMSComponenteRolIdentidad.Where(item => item.ComponenteID.Equals(fila.ComponenteID)).ToList())
                    {
                        mEntityContext.EliminarElemento(filaRolIdentidad);
                        gestorCMS.CMSDW.ListaCMSComponenteRolIdentidad.Remove(filaRolIdentidad);
                    }
                    foreach (AD.EntityModel.Models.CMS.CMSPropiedadComponente filaPropiedad in gestorCMS.CMSDW.ListaCMSPropiedadComponente.Where(item => item.ComponenteID.Equals(fila.ComponenteID)).ToList())
                    {
                        mEntityContext.EliminarElemento(filaPropiedad);
                        gestorCMS.CMSDW.ListaCMSPropiedadComponente.Remove(filaPropiedad);
                    }
                    foreach (AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades filaRolGrupoIdentidades in gestorCMS.CMSDW.ListaCMSComponenteRolGrupoIdentidades.Where(item => item.ComponenteID.Equals(fila.ComponenteID)).ToList())
                    {
                        mEntityContext.EliminarElemento(filaRolGrupoIdentidades);
                        gestorCMS.CMSDW.ListaCMSComponenteRolGrupoIdentidades.Remove(filaRolGrupoIdentidades);
                    }

                    mEntityContext.EliminarElemento(fila);
                    gestorCMS.CMSDW.ListaCMSComponente.Remove(fila);
                }
            }

            #endregion

            #region Páginas

            XmlNodeList paginas = pConfigXML.SelectNodes("/Comunidad/Paginas/Pagina");
            //Chequeo de datos
            List<short> listaUbicaciones = new List<short>();
            foreach (XmlElement pagina in paginas)
            {
                //Comprobamos las ubicaciones
                string strUbicacion = (string)LeerNodo(pagina, "Ubicacion", typeof(string));
                if (!string.IsNullOrEmpty(strUbicacion))
                {
                    try
                    {
                        short ubicacion = short.Parse(strUbicacion);
                        if (listaUbicaciones.Contains(ubicacion))
                        {
                            throw new Exception("No puede haber dos páginas con la misma ubicación");
                        }
                        else
                        {
                            listaUbicaciones.Add(ubicacion);
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception("La ubicación de una página del CMS debe ser un short");
                    }
                }
                else
                {
                    throw new Exception("La ubicación de una página del CMS no puede estar vacía");
                }
            }

            FilasDependientesCMSBloque filasCmsBloque = new FilasDependientesCMSBloque();
            List<AD.EntityModel.Models.CMS.CMSPagina> listaPaginas = new List<AD.EntityModel.Models.CMS.CMSPagina>();
            foreach (XmlElement pagina in paginas)
            {
                listaPaginas.Add(CrearPaginaCMS(pagina, listaFilasComponentesPorNombreCorto, filasCmsBloque));
            }

            //Eliminamos el contenido que no este en el xml

            //CMSPagina
            foreach (AD.EntityModel.Models.CMS.CMSPagina filaCMSPagina in gestorCMS.CMSDW.ListaCMSPagina.ToList())
            {
                if (!listaPaginas.Contains(filaCMSPagina))
                {
                    foreach (AD.EntityModel.Models.CMS.CMSBloque filaCMSBloque in gestorCMS.CMSDW.ListaCMSBloque.Where(item => item.Ubicacion.Equals(filaCMSPagina.Ubicacion)).ToList())
                    {
                        foreach (AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente filaCMSBloqueComponentePropiedadComponente in gestorCMS.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Where(item => item.BloqueID.Equals(filaCMSBloque.BloqueID)).ToList())
                        {
                            mEntityContext.EliminarElemento(filaCMSBloqueComponentePropiedadComponente);
                            gestorCMS.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Remove(filaCMSBloqueComponentePropiedadComponente);
                        }

                        foreach (AD.EntityModel.Models.CMS.CMSBloqueComponente filaCMSBloqueComponente in gestorCMS.CMSDW.ListaCMSBloqueComponente.Where(item => item.BloqueID.Equals(filaCMSBloque.BloqueID)).ToList())
                        {
                            mEntityContext.EliminarElemento(filaCMSBloqueComponente);
                            gestorCMS.CMSDW.ListaCMSBloqueComponente.Remove(filaCMSBloqueComponente);
                        }
                        mEntityContext.EliminarElemento(filaCMSBloque);
                        gestorCMS.CMSDW.ListaCMSBloque.Remove(filaCMSBloque);
                    }
                    mEntityContext.EliminarElemento(filaCMSPagina);
                    gestorCMS.CMSDW.ListaCMSPagina.Remove(filaCMSPagina);
                }
            }
            //CMSBloqueComponentePropiedadComponente
            //foreach (AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente fila in filasCmsBloque.ListaCMSBloqueComponentePropiedadComponente.ToList())
            //{
            //    if (!filasCmsBloque.ListaCMSBloqueComponentePropiedadComponente.Contains(fila))
            //    {
            //        EntityContext.EliminarElemento(fila);
            //        gestorCMS.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Remove(fila);
            //    }
            //}
            //CMSBloqueComponente
            //foreach (AD.EntityModel.Models.CMS.CMSBloqueComponente fila in filasCmsBloque.ListaCMSBloqueComponente)
            //{
            //    if (!filasCmsBloque.ListaCMSBloqueComponente.Contains(fila))
            //    {
            //        EntityContext.EliminarElemento(fila);
            //        gestorCMS.CMSDW.ListaCMSBloqueComponente.Remove(fila);
            //    }
            //}
            //CMSBloque
            //foreach (AD.EntityModel.Models.CMS.CMSBloque fila in filasCmsBloque.ListaCMSBloque.ToList())
            //{
            //    if (!filasCmsBloque.ListaCMSBloque.Contains(fila))
            //    {
            //        EntityContext.EliminarElemento(fila);
            //        gestorCMS.CMSDW.ListaCMSBloque.Remove(fila);
            //    }
            //}
            #endregion
        }

        /// <summary>
        /// Crea un componente del CMS en la BBDD a partir de un XML
        /// </summary>
        /// <param name="pComponente">Nodo del XML del componente</param>
        /// <param name="pListaComponentes">Diccionario con nombrecorto/id de los nuevos componentes</param>
        /// <returns></returns>
        private AD.EntityModel.Models.CMS.CMSComponente CrearComponenteCMS(XmlElement pComponente, Dictionary<string, Guid> pListaComponentes)
        {
            //Obtenemos el Nombre
            string Nombre = (string)LeerNodo(pComponente, "Nombre", typeof(string));
            if (string.IsNullOrEmpty(Nombre))
            {
                throw new Exception("El nombre de un componente es obligatorio");
            }

            //Obtenemos el NombreCorto
            string NombreCorto = (string)LeerNodo(pComponente, "NombreCorto", typeof(string));
            if (string.IsNullOrEmpty(NombreCorto))
            {
                throw new Exception("El nombrecorto de un componente es obligatorio");
            }

            //Obtenemos el tipo de componente
            string strTipoComponente = (string)LeerNodo(pComponente, "TipoComponente", typeof(string));
            TipoComponenteCMS TipoComponente;
            if (!Enum.TryParse(strTipoComponente, out TipoComponente))
            {
                throw new Exception("El tipo de componente " + strTipoComponente + " no es válido");
            }

            //Obtenemos si está activo
            string strActivo = (string)LeerNodo(pComponente, "Activo", typeof(string));
            bool activo;
            if (!bool.TryParse(strActivo, out activo))
            {
                throw new Exception($"El campo activo debe ser true o false. Valor actual: {strActivo}");
            }

            //Configura el acceso público del componente sólo en comunidades privadas o reservadas, en las demás false
            string strAccesoPublico = (string)LeerNodo(pComponente, "AccesoPublico", typeof(string));
            bool accesoPublico = false;

            if (!string.IsNullOrEmpty(strAccesoPublico) && ProyectoActual.TipoAcceso.Equals(TipoAcceso.Privado) || ProyectoActual.TipoAcceso.Equals(TipoAcceso.Reservado))
            {
                if (!bool.TryParse(strAccesoPublico, out accesoPublico))
                {
                    throw new Exception($"El campo accesopublico debe ser true o false. Valor actual: {strAccesoPublico}");
                }
            }

            //Obtenemos estilos
            string Estilos = (string)LeerNodo(pComponente, "Estilos", typeof(string));

            //Obtenemos la caducidad
            string strCaducidad = (string)LeerNodo(pComponente, "Caducidad", typeof(string));
            TipoCaducidadComponenteCMS TipoCaducidadComponente;
            if (UtilComponentes.CaducidadesDisponiblesPorTipoComponente[TipoComponente].Count > 1)
            {
                try
                {
                    TipoCaducidadComponente = (TipoCaducidadComponenteCMS)Enum.Parse(typeof(TipoCaducidadComponenteCMS), strCaducidad);
                }
                catch
                {
                    throw new Exception("El tipo de caducidad " + strCaducidad + " no es válido");
                }
                if (!UtilComponentes.CaducidadesDisponiblesPorTipoComponente[TipoComponente].Contains(TipoCaducidadComponente))
                {
                    throw new Exception("El componente del tipo " + TipoComponente.ToString() + " no puede tener caducidad '" + strCaducidad + "'");
                }
            }
            else
            {
                TipoCaducidadComponente = UtilComponentes.CaducidadesDisponiblesPorTipoComponente[TipoComponente][0];
            }

            //Obtenemos los idiomas disponibles
            string idiomasDisponibles = (string)LeerNodo(pComponente, "IdiomasDisponibles", typeof(string));

            AD.EntityModel.Models.CMS.CMSComponente filaComponente = gestorCMS.CMSDW.ListaCMSComponente.Where(item => item.NombreCortoComponente.Equals(NombreCorto)).FirstOrDefault();
            if (filaComponente != null)
            {
                if (filaComponente.AccesoPublico != accesoPublico)
                {
                    filaComponente.AccesoPublico = accesoPublico;
                }
                if (filaComponente.Activo != activo)
                {
                    filaComponente.Activo = activo;
                }
                if (filaComponente.IdiomasDisponibles != idiomasDisponibles)
                {
                    filaComponente.IdiomasDisponibles = idiomasDisponibles;
                }
                if (filaComponente.Estilos != Estilos)
                {
                    filaComponente.Estilos = Estilos;
                }
                if (filaComponente.TipoCaducidadComponente != (short)TipoCaducidadComponente)
                {
                    filaComponente.TipoCaducidadComponente = (short)TipoCaducidadComponente;
                }
                if (filaComponente.TipoComponente != (short)TipoComponente)
                {
                    filaComponente.TipoComponente = (short)TipoComponente;
                }
                if (filaComponente.Nombre != Nombre)
                {
                    filaComponente.Nombre = Nombre;
                }
            }
            else
            {
                filaComponente = new AD.EntityModel.Models.CMS.CMSComponente();
                filaComponente.OrganizacionID = ProyectoActual.FilaProyecto.OrganizacionID;
                filaComponente.ProyectoID = ProyectoActual.Clave;
                filaComponente.ComponenteID = pListaComponentes[NombreCorto];
                filaComponente.Nombre = Nombre;
                filaComponente.TipoComponente = (short)TipoComponente;
                filaComponente.TipoCaducidadComponente = (short)TipoCaducidadComponente;
                filaComponente.FechaUltimaActualizacion = DateTime.Now;
                filaComponente.Estilos = Estilos;
                filaComponente.IdiomasDisponibles = idiomasDisponibles;
                filaComponente.Activo = activo;
                filaComponente.NombreCortoComponente = NombreCorto;
                filaComponente.AccesoPublico = accesoPublico;

                gestorCMS.CMSDW.ListaCMSComponente.Add(filaComponente);
                mEntityContext.CMSComponente.Add(filaComponente);
            }

            List<AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad> filasRolIdentidad = new List<AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad>();
            List<AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades> filasComponenteRolGrupoIdentidades = new List<AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades>();


            if (pComponente.SelectSingleNode("ConfiguracionPrivacidad") != null)
            {
                XmlNode nodoConfiguracionPrivacidad = pComponente.SelectSingleNode("ConfiguracionPrivacidad");

                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                if (nodoConfiguracionPrivacidad.SelectSingleNode("PrivacidadPerfiles") != null)
                {
                    //perfiles
                    XmlNode nodoPerfiles = nodoConfiguracionPrivacidad.SelectSingleNode("PrivacidadPerfiles");
                    XmlNodeList perfiles = nodoPerfiles.SelectNodes("Perfil");

                    foreach (XmlNode nodoPerfil in perfiles)
                    {
                        string nombreCortoPerfil = nodoPerfil.InnerText;
                        Guid idPerfil = identidadCN.ObtenerPerfilIDPorNombreCorto(nombreCortoPerfil);
                        if (idPerfil != Guid.Empty)
                        {
                            AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad filaComponenteRolIdentidad = gestorCMS.CMSDW.ListaCMSComponenteRolIdentidad.Where(item => item.ComponenteID.Equals(filaComponente.ComponenteID) && item.PerfilID.Equals(idPerfil)).FirstOrDefault();
                            if (filaComponenteRolIdentidad == null)
                            {
                                filaComponenteRolIdentidad = new AD.EntityModel.Models.CMS.CMSComponenteRolIdentidad();
                                filaComponenteRolIdentidad.ComponenteID = filaComponente.ComponenteID;
                                filaComponenteRolIdentidad.PerfilID = idPerfil;

                                gestorCMS.CMSDW.ListaCMSComponenteRolIdentidad.Add(filaComponenteRolIdentidad);
                                mEntityContext.CMSComponenteRolIdentidad.Add(filaComponenteRolIdentidad);
                            }
                            filasRolIdentidad.Add(filaComponenteRolIdentidad);
                        }
                        else
                        {
                            throw new Exception("El perfil " + nombreCortoPerfil + " no existe");
                        }
                    }
                }

                if (nodoConfiguracionPrivacidad.SelectSingleNode("PrivacidadGrupos") != null)
                {
                    //Grupos
                    XmlNode nodoGrupos = nodoConfiguracionPrivacidad.SelectSingleNode("PrivacidadGrupos");


                    //GruposOrg
                    XmlNodeList gruposOrg = nodoGrupos.SelectNodes("GrupoOrg");
                    foreach (XmlNode nodoGrupoOrg in gruposOrg)
                    {
                        string nombreCortoGrupo = (string)LeerNodo(nodoGrupoOrg, "NombreCortoGrupo", typeof(string));
                        string nombreCortoOrg = (string)LeerNodo(nodoGrupoOrg, "NombreCortoOrg", typeof(string));

                        OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        Guid idOrganizacion = orgCN.ObtenerOrganizacionesIDPorNombre(nombreCortoOrg);
                        orgCN.Dispose();
                        DataWrapperIdentidad identiadDWGrupo = identidadCN.ObtenerGrupoPorNombreCortoYOrganizacion(nombreCortoGrupo, idOrganizacion);

                        if (identiadDWGrupo.ListaGrupoIdentidadesOrganizacion.Count > 0)
                        {
                            Guid idGrupo = identiadDWGrupo.ListaGrupoIdentidadesOrganizacion.First().GrupoID;

                            AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades fila = gestorCMS.CMSDW.ListaCMSComponenteRolGrupoIdentidades.Where(item => item.ComponenteID.Equals(filaComponente.ComponenteID) && item.GrupoID.Equals(idGrupo)).FirstOrDefault();

                            if (fila == null)
                            {
                                fila = new AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades();
                                fila.ComponenteID = filaComponente.ComponenteID;
                                fila.GrupoID = idGrupo;

                                gestorCMS.CMSDW.ListaCMSComponenteRolGrupoIdentidades.Add(fila);
                                mEntityContext.CMSComponenteRolGrupoIdentidades.Add(fila);
                            }

                            filasComponenteRolGrupoIdentidades.Add(fila);
                        }
                        else
                        {
                            if (!IgnorarErroresGrupos)
                            {
                                throw new Exception("El grupo " + nombreCortoGrupo + " de la organizacion " + nombreCortoOrg + " no existe");
                            }
                        }
                    }

                    //GruposProy
                    XmlNodeList gruposProy = nodoGrupos.SelectNodes("GrupoProy");
                    foreach (XmlNode nodoGrupoProy in gruposProy)
                    {
                        string nombreCortoGrupo = (string)LeerNodo(nodoGrupoProy, "NombreCortoGrupo", typeof(string));
                        string nombreCortoProy = (string)LeerNodo(nodoGrupoProy, "NombreCortoProy", typeof(string));

                        ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        Guid idProyecto = proyCN.ObtenerProyectoIDPorNombre(nombreCortoProy);
                        proyCN.Dispose();
                        DataWrapperIdentidad identiadDWGrupo = identidadCN.ObtenerGrupoPorNombreCortoYProyecto(nombreCortoGrupo, idProyecto);

                        if (identiadDWGrupo.ListaGrupoIdentidadesProyecto.Count > 0)
                        {
                            Guid idGrupo = identiadDWGrupo.ListaGrupoIdentidadesProyecto.First().GrupoID;

                            AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades fila = gestorCMS.CMSDW.ListaCMSComponenteRolGrupoIdentidades.Where(item => item.ComponenteID.Equals(filaComponente.ComponenteID) && item.GrupoID.Equals(idGrupo)).FirstOrDefault();

                            if (fila == null)
                            {
                                fila = new AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades();
                                fila.ComponenteID = filaComponente.ComponenteID;
                                fila.GrupoID = idGrupo;

                                gestorCMS.CMSDW.ListaCMSComponenteRolGrupoIdentidades.Add(fila);
                                mEntityContext.CMSComponenteRolGrupoIdentidades.Add(fila);
                            }

                            filasComponenteRolGrupoIdentidades.Add(fila);
                        }
                        else
                        {
                            if (!IgnorarErroresGrupos)
                            {
                                throw new Exception("El grupo " + nombreCortoGrupo + " del proyecto " + nombreCortoProy + " no existe");
                            }
                        }
                    }

                    XmlNodeList grupos = nodoGrupos.SelectNodes("Grupo");
                    foreach (XmlNode nodoGrupo in grupos)
                    {
                        string nombreCortoGrupo = nodoGrupo.InnerText;
                        List<string> listaGrupos = new List<string>();
                        listaGrupos.Add(nombreCortoGrupo);
                        List<Guid> listGrupos = identidadCN.ObtenerGruposIDPorNombreCortoYProyecto(listaGrupos, ProyectoActual.Clave);
                        if (listGrupos.Count > 0)
                        {
                            Guid idGrupo = listGrupos[0];
                            AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades cmsComponenteRolGrupoIdentidades = new AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades();
                            cmsComponenteRolGrupoIdentidades.CMSComponente = filaComponente;
                            cmsComponenteRolGrupoIdentidades.ComponenteID = filaComponente.ComponenteID;
                            cmsComponenteRolGrupoIdentidades.GrupoID = idGrupo;

                            AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades fila = gestorCMS.CMSDW.ListaCMSComponenteRolGrupoIdentidades.Where(item => item.ComponenteID.Equals(filaComponente.ComponenteID) && item.GrupoID.Equals(idGrupo)).FirstOrDefault();

                            if (fila == null)
                            {
                                fila = new AD.EntityModel.Models.CMS.CMSComponenteRolGrupoIdentidades();
                                fila.ComponenteID = filaComponente.ComponenteID;
                                fila.GrupoID = idGrupo;

                                gestorCMS.CMSDW.ListaCMSComponenteRolGrupoIdentidades.Add(fila);
                                mEntityContext.CMSComponenteRolGrupoIdentidades.Add(fila);
                            }

                            filasComponenteRolGrupoIdentidades.Add(fila);
                        }
                        else
                        {
                            throw new Exception("El grupo " + nombreCortoGrupo + " no existe");
                        }
                    }
                }
                identidadCN.Dispose();
            }

            List<AD.EntityModel.Models.CMS.CMSPropiedadComponente> filasPropiedadComponente = new List<AD.EntityModel.Models.CMS.CMSPropiedadComponente>();

            XmlNodeList propiedadesComponente = pComponente.SelectNodes("Propiedades/Propiedad");
            if (propiedadesComponente != null && propiedadesComponente.Count > 0)
            {
                //Chequeo de datos(comprobamos que vengan todas las propiedades obligatorias)
                foreach (TipoPropiedadCMS propiedad in UtilComponentes.PropiedadesDisponiblesPorTipoComponente[TipoComponente].Keys)
                {
                    if (UtilComponentes.PropiedadesDisponiblesPorTipoComponente[TipoComponente][propiedad])
                    {
                        bool encontrada = false;
                        foreach (XmlElement propiedadComprobar in propiedadesComponente)
                        {
                            //Obtenemos la propiedad
                            string strPropiedad = (string)LeerNodo(propiedadComprobar, "Clave", typeof(string));
                            TipoPropiedadCMS Propiedad;
                            try
                            {
                                Propiedad = (TipoPropiedadCMS)Enum.Parse(typeof(TipoPropiedadCMS), strPropiedad);
                            }
                            catch
                            {
                                throw new Exception("El tipo de propiedad " + strPropiedad + " no es válido");
                            }
                            if (Propiedad == propiedad)
                            {
                                encontrada = true;
                            }
                        }
                        if (!encontrada)
                        {
                            throw new Exception("La propiedad '" + propiedad.ToString() + "' es obligatoria para el componente '" + TipoComponente.ToString() + "'  y no aparece en el componente con nombrecorto '" + NombreCorto + "'");
                        }
                    }
                }

                foreach (XmlElement propiedad in propiedadesComponente)
                {
                    //Obtenemos la propiedad
                    string strPropiedad = (string)LeerNodo(propiedad, "Clave", typeof(string));
                    TipoPropiedadCMS Propiedad;
                    try
                    {
                        Propiedad = (TipoPropiedadCMS)Enum.Parse(typeof(TipoPropiedadCMS), strPropiedad);
                    }
                    catch
                    {
                        throw new Exception("El tipo de propiedad " + strPropiedad + " del componente " + NombreCorto + " no es válido");
                    }

                    if (!UtilComponentes.PropiedadesDisponiblesPorTipoComponente[TipoComponente].ContainsKey(Propiedad) && Propiedad != TipoPropiedadCMS.Personalizacion)
                    {
                        throw new Exception("La propiedad " + strPropiedad + " no está disponible en los componentes del tipo " + TipoComponente.ToString());
                    }

                    //Obtenemos el valor de la propiedad
                    string Valor = (string)LeerNodo(propiedad, "Valor", typeof(string));

                    switch (Propiedad)
                    {
                        case TipoPropiedadCMS.TipoActividadRecienteCMS:
                            TipoActividadReciente tipoActividad;
                            try
                            {
                                tipoActividad = (TipoActividadReciente)Enum.Parse(typeof(TipoActividadReciente), Valor);
                            }
                            catch
                            {
                                throw new Exception("La propiedad " + Propiedad + " no puede tener como valor " + Valor);
                            }
                            Valor = ((short)tipoActividad).ToString();
                            break;
                        case TipoPropiedadCMS.TipoListadoProyectos:
                            TipoListadoProyectosCMS tipoListadoProyectos;
                            try
                            {
                                tipoListadoProyectos = (TipoListadoProyectosCMS)Enum.Parse(typeof(TipoListadoProyectosCMS), Valor);
                            }
                            catch
                            {
                                throw new Exception("La propiedad " + Propiedad + " no puede tener como valor " + Valor);
                            }
                            Valor = ((short)tipoListadoProyectos).ToString();
                            break;
                        case TipoPropiedadCMS.TipoListadoUsuarios:
                            TipoListadoUsuariosCMS tipoListadoUsuarios;
                            try
                            {
                                tipoListadoUsuarios = (TipoListadoUsuariosCMS)Enum.Parse(typeof(TipoListadoUsuariosCMS), Valor);
                            }
                            catch
                            {
                                throw new Exception("La propiedad " + Propiedad + " no puede tener como valor " + Valor);
                            }
                            Valor = ((short)tipoListadoUsuarios).ToString();
                            break;
                        case TipoPropiedadCMS.TipoPresentacionFaceta:
                            TipoPresentacionFacetas tipoPresentacionFacetas;
                            try
                            {
                                tipoPresentacionFacetas = (TipoPresentacionFacetas)Enum.Parse(typeof(TipoPresentacionFacetas), Valor);
                            }
                            catch
                            {
                                throw new Exception("La propiedad " + Propiedad + " no puede tener como valor " + Valor);
                            }
                            Valor = ((short)tipoPresentacionFacetas).ToString();
                            break;
                        case TipoPropiedadCMS.TipoPresentacionGrupoComponentes:
                            Valor = ObtenerValorPropiedadTipoPresentacionGrupoComponentesDeXMLaBBDD(Valor);
                            break;
                        case TipoPropiedadCMS.TipoPresentacionListadoRecursos:
                            Valor = ObtenerValorPropiedadTipoPresentacionListadoRecursosDeXMLaBBDD(Valor);
                            break;
                        case TipoPropiedadCMS.TipoPresentacionListadoUsuarios:
                            TipoPresentacionListadoUsuariosCMS tipoPresentacionListadoUsuariosCMS;
                            try
                            {
                                tipoPresentacionListadoUsuariosCMS = (TipoPresentacionListadoUsuariosCMS)Enum.Parse(typeof(TipoPresentacionListadoUsuariosCMS), Valor);
                            }
                            catch
                            {
                                throw new Exception("La propiedad " + Propiedad + " no puede tener como valor " + Valor);
                            }
                            Valor = ((short)tipoPresentacionListadoUsuariosCMS).ToString();
                            break;
                        case TipoPropiedadCMS.TipoPresentacionRecurso:
                            Valor = ObtenerValorPropiedadTipoPresentacionRecursoDeXMLaBBDD(Valor);
                            break;
                        case TipoPropiedadCMS.ListaIDs:
                            if (TipoComponente == TipoComponenteCMS.GrupoComponentes)
                            {
                                //Convertimos de nombrescortos a IDs
                                string[] nombresCortosComponentes = Valor.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                Valor = "";
                                foreach (string nombreCorto in nombresCortosComponentes)
                                {
                                    if (pListaComponentes.ContainsKey(nombreCorto))
                                    {
                                        Valor += pListaComponentes[nombreCorto].ToString() + ",";
                                    }
                                    else
                                    {
                                        throw new Exception("El componente con nombrecorto '" + nombreCorto + "' incluido dentro del componente de tipo GrupoComponentes no existe");
                                    }
                                }
                            }
                            break;
                    }

                    if (FilasPropiedadesIntegracion != null && FilasPropiedadesIntegracion.Count > 0)
                    {
                        TipoPropiedad? tipoPropiedad = null;

                        //if (Propiedad.Equals(TipoPropiedadCMS.HTML))
                        //{
                        //    tipoPropiedad = TipoPropiedad.HtmlComponente;
                        //}
                        //else if (Propiedad.Equals(TipoPropiedadCMS.URLBusqueda))
                        //{
                        //    tipoPropiedad = TipoPropiedad.UrlBusquedaComponente;
                        //}
                        //else if (Propiedad.Equals(TipoPropiedadCMS.URLVerMas))
                        //{
                        //    tipoPropiedad = TipoPropiedad.UrlVerMasComponente;
                        //}
                        //else 
                        if (filaComponente.TipoComponente.Equals((short)TipoComponenteCMS.ListadoEstatico) && Propiedad.Equals(TipoPropiedadCMS.ListaIDs))
                        {
                            tipoPropiedad = TipoPropiedad.IdsRecursosComponente;
                        }
                        else if (filaComponente.TipoComponente.Equals(TipoComponenteCMS.Tesauro) && Propiedad.Equals(TipoPropiedadCMS.ElementoID))
                        {
                            tipoPropiedad = TipoPropiedad.TesauroComponente;
                        }

                        if (tipoPropiedad.HasValue)
                        {
                            var propiedadIntegracion = FilasPropiedadesIntegracion.FirstOrDefault(prop => prop.ObjetoPropiedad == filaComponente.NombreCortoComponente && prop.TipoPropiedad == (short)tipoPropiedad.Value);

                            if (propiedadIntegracion != null && propiedadIntegracion.TipoObjeto.Equals(TipoObjeto.Componente) && propiedadIntegracion.ValorPropiedad == Valor && propiedadIntegracion.Revisada && !propiedadIntegracion.MismoValor)
                            {
                                Valor = propiedadIntegracion.ValorPropiedadDestino;
                            }
                        }
                    }

                    AD.EntityModel.Models.CMS.CMSPropiedadComponente filaPropiedadComponente = gestorCMS.CMSDW.ListaCMSPropiedadComponente.Where(item => item.ComponenteID.Equals(filaComponente.ComponenteID) && item.TipoPropiedadComponente.Equals((short)Propiedad)).FirstOrDefault();

                    if (filaPropiedadComponente != null)
                    {
                        if (!filaPropiedadComponente.ValorPropiedad.Equals(Valor))
                        {
                            filaPropiedadComponente.ValorPropiedad = Valor;
                        }
                    }
                    else
                    {
                        filaPropiedadComponente = new AD.EntityModel.Models.CMS.CMSPropiedadComponente();
                        filaPropiedadComponente.CMSComponente = filaComponente;
                        filaPropiedadComponente.ValorPropiedad = Valor;
                        filaPropiedadComponente.TipoPropiedadComponente = (short)Propiedad;
                        filaPropiedadComponente.ComponenteID = filaComponente.ComponenteID;

                        gestorCMS.CMSDW.ListaCMSPropiedadComponente.Add(filaPropiedadComponente);
                        mEntityContext.CMSPropiedadComponente.Add(filaPropiedadComponente);
                    }
                    filasPropiedadComponente.Add(filaPropiedadComponente);
                }
            }
            //ELIMINAR FILAS

            //ComponenteRolIdentidad
            foreach (var fila in gestorCMS.CMSDW.ListaCMSComponenteRolIdentidad.Where(item => item.ComponenteID.Equals(filaComponente.ComponenteID)).ToList())
            {
                if (!filasRolIdentidad.Contains(fila))
                {
                    mEntityContext.EliminarElemento(fila);
                    gestorCMS.CMSDW.ListaCMSComponenteRolIdentidad.Remove(fila);
                }
            }

            //PropiedadComponente
            foreach (var fila in gestorCMS.CMSDW.ListaCMSPropiedadComponente.Where(item => item.ComponenteID.Equals(filaComponente.ComponenteID)).ToList())
            {
                if (!filasPropiedadComponente.Contains(fila))
                {
                    mEntityContext.EliminarElemento(fila);
                    gestorCMS.CMSDW.ListaCMSPropiedadComponente.Remove(fila);
                }
            }

            //ComponenteRolGrupoIdentidades
            foreach (var fila in gestorCMS.CMSDW.ListaCMSComponenteRolGrupoIdentidades.Where(item => item.ComponenteID.Equals(filaComponente.ComponenteID)).ToList())
            {
                if (!filasComponenteRolGrupoIdentidades.Contains(fila))
                {
                    mEntityContext.EliminarElemento(fila);
                    gestorCMS.CMSDW.ListaCMSComponenteRolGrupoIdentidades.Remove(fila);
                }
            }

            return filaComponente;
        }

        /// <summary>
        /// Crea una página del CMS en la BBDD a partir de un XML
        /// </summary>
        /// <param name="pPagina">Nodo del XML de la página</param>
        /// <param name="pListaFilasComponentesPorNombreCorto">Diccionario con nombrecorto/fila de los nuevos componentes</param>
        private AD.EntityModel.Models.CMS.CMSPagina CrearPaginaCMS(XmlElement pPagina, Dictionary<string, AD.EntityModel.Models.CMS.CMSComponente> pListaFilasComponentesPorNombreCorto, FilasDependientesCMSBloque pFilasCmsBloque)
        {
            //Obtenemos la ubicacion
            string strUbicacion = (string)LeerNodo(pPagina, "Ubicacion", typeof(string));
            short Ubicacion = short.Parse(strUbicacion);

            //Comprobamos que la pestaña exista
            bool existe = false;
            if (Ubicacion == (short)TipoUbicacionCMS.HomeProyecto || Ubicacion == (short)TipoUbicacionCMS.HomeProyectoMiembro || Ubicacion == (short)TipoUbicacionCMS.HomeProyectoNoMiembro)
            {
                existe = true;
            }
            else
            {
                existe = ProyectoActual.ListaPestanyasMenu.Values.Any(item => item.FilaProyectoPestanyaCMS != null && item.FilaProyectoPestanyaCMS.Ubicacion.Equals(Ubicacion));

                if (!existe && mProyectoPadre != null)
                {
                    existe = mProyectoPadre.ListaPestanyasMenu.Values.Any(item => item.FilaProyectoPestanyaCMS != null && item.FilaProyectoPestanyaCMS.Ubicacion.Equals(Ubicacion));
                }
            }

            if (!existe)
            {
                throw new Exception("No existe niguna Pestanya con UbicacionCMS= " + Ubicacion.ToString());
            }


            //Obtenemos si está activo
            string strActiva = (string)LeerNodo(pPagina, "Activa", typeof(string));
            bool Activa;
            try
            {
                Activa = bool.Parse(strActiva);
            }
            catch
            {
                throw new Exception("El campo Activa de una página debe ser true o false");
            }

            //Obtenemos mostrar solo cuerpo
            string strMostrarSoloCuerpo = (string)LeerNodo(pPagina, "MostrarSoloCuerpo", typeof(string));
            bool MostrarSoloCuerpo = false;
            if (!string.IsNullOrEmpty(strMostrarSoloCuerpo))
            {
                try
                {
                    MostrarSoloCuerpo = bool.Parse(strMostrarSoloCuerpo);
                }
                catch
                {
                    throw new Exception("El campo MostrarSoloCuerpo de una página debe ser true o false");
                }
            }

            AD.EntityModel.Models.CMS.CMSPagina filaPagina = gestorCMS.CMSDW.ListaCMSPagina.Where(item => item.OrganizacionID.Equals(ProyectoActual.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoActual.FilaProyecto.ProyectoID) && item.Ubicacion.Equals(Ubicacion)).FirstOrDefault();

            if (filaPagina != null)
            {
                if (filaPagina.Activa != Activa)
                {
                    filaPagina.Activa = Activa;
                }
                if (filaPagina.MostrarSoloCuerpo != MostrarSoloCuerpo)
                {
                    filaPagina.MostrarSoloCuerpo = MostrarSoloCuerpo;
                }
            }
            else
            {
                filaPagina = new AD.EntityModel.Models.CMS.CMSPagina();
                filaPagina.OrganizacionID = ProyectoActual.FilaProyecto.OrganizacionID;
                filaPagina.ProyectoID = ProyectoActual.FilaProyecto.ProyectoID;
                filaPagina.Ubicacion = Ubicacion;
                filaPagina.Activa = Activa;
                filaPagina.MostrarSoloCuerpo = MostrarSoloCuerpo;

                gestorCMS.CMSDW.ListaCMSPagina.Add(filaPagina);
                mEntityContext.CMSPagina.Add(filaPagina);
            }

            XmlNodeList bloquesPagina = pPagina.SelectNodes("Bloques/Bloque");

            List<bool> listaBorrador = new List<bool>();
            listaBorrador.Add(true);
            listaBorrador.Add(false);

            foreach (bool borrador in listaBorrador)
            {
                short orden = 0;
                foreach (XmlElement bloque in bloquesPagina)
                {
                    CrearBloquePaginaCMS(bloque, null, Ubicacion, orden, pListaFilasComponentesPorNombreCorto, borrador, pFilasCmsBloque);
                    orden++;
                }
            }

            return filaPagina;
        }

        /// <summary>
        /// Crea un bloque del CMS en la BBDD a partir de un XML
        /// </summary>
        /// <param name="pBloque">Nodo del XML del bloque</param>
        /// <param name="pBloquePadre">Bloque padre(null si es raíz)</param>
        /// <param name="pUbicacion">Ubicación del bloque</param>
        /// <param name="pOrden">Orden del bloque</param>
        /// <param name="pListaFilasComponentesPorNombreCorto">Diccionario con nombrecorto/fila de los nuevos componentes</param>
        /// <param name="pBorrador">Indica si el bloque es borrador o no</param>
        private void CrearBloquePaginaCMS(XmlElement pBloque, AD.EntityModel.Models.CMS.CMSBloque pBloquePadre, short pUbicacion, short pOrden, Dictionary<string, AD.EntityModel.Models.CMS.CMSComponente> pListaFilasComponentesPorNombreCorto, bool pBorrador, FilasDependientesCMSBloque pFilasCmsBloque)
        {
            XmlNodeList atributosBloque = pBloque.SelectNodes("Atributos/Atributo");
            string Estilos = "";

            if (atributosBloque.Count == 1 && ((string)LeerNodo(atributosBloque[0], "Clave", typeof(string))).Equals("class"))
            {
                Estilos = (string)LeerNodo(atributosBloque[0], "Valor", typeof(string));
            }
            else
            {
                foreach (XmlElement atributo in atributosBloque)
                {
                    //Obtenemos la clave
                    string claveAtributo = (string)LeerNodo(atributo, "Clave", typeof(string));
                    //Obtenemos el valor
                    string valorAtributo = (string)LeerNodo(atributo, "Valor", typeof(string));
                    Estilos += claveAtributo + "---" + valorAtributo + "~~~";
                }
            }

            AD.EntityModel.Models.CMS.CMSBloque filaBloque = gestorCMS.CMSDW.ListaCMSBloque.Where(item => item.Ubicacion.Equals(pUbicacion) && item.Orden.Equals(pOrden) && ((pBloquePadre == null && !item.BloquePadreID.HasValue) || (pBloquePadre != null && item.BloquePadreID.HasValue && pBloquePadre.BloqueID.Equals(item.BloquePadreID)))).FirstOrDefault();
            Guid BloqueID = Guid.NewGuid();

            if (filaBloque != null)
            {
                if (!filaBloque.Estilos.Equals(Estilos))
                {
                    filaBloque.Estilos = Estilos;
                }
                if (!filaBloque.Borrador.Equals(pBorrador))
                {
                    filaBloque.Borrador = pBorrador;
                }
                BloqueID = filaBloque.BloqueID;
            }
            else
            {

                filaBloque = new AD.EntityModel.Models.CMS.CMSBloque();
                filaBloque.OrganizacionID = ProyectoActual.FilaProyecto.OrganizacionID;
                filaBloque.ProyectoID = ProyectoActual.Clave;
                filaBloque.Ubicacion = pUbicacion;
                filaBloque.BloqueID = BloqueID;
                if (pBloquePadre != null)
                {
                    filaBloque.BloquePadreID = pBloquePadre.BloqueID;
                }
                filaBloque.Orden = pOrden;
                filaBloque.Estilos = Estilos;
                filaBloque.Borrador = pBorrador;

                gestorCMS.CMSDW.ListaCMSBloque.Add(filaBloque);
                mEntityContext.CMSBloque.Add(filaBloque);
            }

            pFilasCmsBloque.ListaCMSBloque.Add(filaBloque);

            XmlNodeList bloquesBloque = pBloque.SelectNodes("Bloques/Bloque");
            short orden = 0;
            foreach (XmlElement bloque in bloquesBloque)
            {
                CrearBloquePaginaCMS(bloque, filaBloque, pUbicacion, orden, pListaFilasComponentesPorNombreCorto, pBorrador, pFilasCmsBloque);
                orden++;
            }

            XmlNodeList componentes = pBloque.SelectNodes("Componentes/Componente");
            orden = 0;
            foreach (XmlElement componente in componentes)
            {
                if (pListaFilasComponentesPorNombreCorto.ContainsKey(componente.InnerText))
                {
                    AD.EntityModel.Models.CMS.CMSBloqueComponente filaBloqueComponente = gestorCMS.CMSDW.ListaCMSBloqueComponente.Where(item => item.BloqueID.Equals(filaBloque.BloqueID) && item.ComponenteID.Equals(pListaFilasComponentesPorNombreCorto[componente.InnerText].ComponenteID)).FirstOrDefault();

                    if (filaBloqueComponente != null)
                    {
                        if (!filaBloqueComponente.Orden.Equals(orden))
                        {
                            filaBloqueComponente.Orden = orden;
                        }
                    }
                    else
                    {
                        filaBloqueComponente = new AD.EntityModel.Models.CMS.CMSBloqueComponente();
                        filaBloqueComponente.Orden = orden;
                        filaBloqueComponente.OrganizacionID = ProyectoActual.FilaProyecto.OrganizacionID;
                        filaBloqueComponente.ProyectoID = ProyectoActual.Clave;
                        filaBloqueComponente.BloqueID = filaBloque.BloqueID;
                        filaBloqueComponente.ComponenteID = pListaFilasComponentesPorNombreCorto[componente.InnerText].ComponenteID;

                        gestorCMS.CMSDW.ListaCMSBloqueComponente.Add(filaBloqueComponente);
                        mEntityContext.CMSBloqueComponente.Add(filaBloqueComponente);
                    }
                    pFilasCmsBloque.ListaCMSBloqueComponente.Add(filaBloqueComponente);
                }
                else
                {
                    throw new Exception("El componente con nombrecorto '" + componente.InnerText + "' que se encuentra en la página con ubicación " + pUbicacion.ToString() + " no existe en la lista de componentes");
                }
                orden++;

                foreach (XmlAttribute propiedadComponenteBloque in componente.Attributes)
                {
                    string strTipoPropiedad = propiedadComponenteBloque.Name;
                    TipoPropiedadCMS TipoPropiedad;
                    try
                    {
                        TipoPropiedad = (TipoPropiedadCMS)Enum.Parse(typeof(TipoPropiedadCMS), strTipoPropiedad);
                    }
                    catch
                    {
                        throw new Exception("El tipo de componente " + strTipoPropiedad + " no es válido");
                    }
                    string valorPropiedad = propiedadComponenteBloque.Value;

                    AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente filaBloqueComponentePropiedadComponente = gestorCMS.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Where(item => item.BloqueID.Equals(BloqueID) && item.ComponenteID.Equals(pListaFilasComponentesPorNombreCorto[componente.InnerText].ComponenteID) && item.TipoPropiedadComponente.Equals((short)TipoPropiedad)).FirstOrDefault();

                    if (filaBloqueComponentePropiedadComponente != null)
                    {
                        if (!filaBloqueComponentePropiedadComponente.ValorPropiedad.Equals(valorPropiedad))
                        {
                            filaBloqueComponentePropiedadComponente.ValorPropiedad = valorPropiedad;
                        }
                    }
                    else
                    {
                        filaBloqueComponentePropiedadComponente = new AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente();
                        filaBloqueComponentePropiedadComponente.OrganizacionID = ProyectoActual.FilaProyecto.OrganizacionID;
                        filaBloqueComponentePropiedadComponente.ProyectoID = ProyectoActual.Clave;
                        filaBloqueComponentePropiedadComponente.BloqueID = BloqueID;
                        filaBloqueComponentePropiedadComponente.ComponenteID = pListaFilasComponentesPorNombreCorto[componente.InnerText].ComponenteID;
                        filaBloqueComponentePropiedadComponente.TipoPropiedadComponente = (short)TipoPropiedad;
                        filaBloqueComponentePropiedadComponente.ValorPropiedad = valorPropiedad;


                        gestorCMS.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Add(filaBloqueComponentePropiedadComponente);
                        mEntityContext.CMSBloqueComponentePropiedadComponente.Add(filaBloqueComponentePropiedadComponente);
                    }
                }
            }
        }

        /// <summary>
        /// Lee los nodos en los que podemos recibir un valor de tipo string o short
        /// </summary>
        /// <param name="nodo">Nodo del XML</param>
        /// <param name="nom">Nombre del nodo</param>
        /// <param name="pTipo">Tipo que se espera recibir</param>
        /// <returns>Object que contendrá una string o un short</returns>
        private object LeerNodo(XmlNode nodo, string nom, Type pTipo)
        {
            Object salida = null;
            if (nodo != null)
            {
                if (nodo.SelectSingleNode(nom) != null)
                {
                    if (pTipo.Equals(typeof(short)))
                    {
                        short enteroCorto = 0;
                        short.TryParse(nodo.SelectSingleNode(nom).InnerText, out enteroCorto);
                        salida = enteroCorto;
                    }
                    if (pTipo.Equals(typeof(string)))
                    {
                        salida = nodo.SelectSingleNode(nom).InnerText;
                    }
                }
                else if (pTipo.Equals(typeof(short)))
                {
                    //es nulo
                    salida = (short)-1;
                }
                else
                {
                    salida = string.Empty;
                }
            }
            else if (pTipo.Equals(typeof(short)))
            {
                //es nulo
                salida = (short)-1;
            }
            else
            {
                salida = string.Empty;
            }
            return salida;
        }

        /// <summary>
        /// Obtiene el valor de la propiedad TipoPresentacionGrupoComponentes teniendo en cuenta la personalización de las vistas
        /// </summary>
        /// <param name="pValorPropiedad">Valor de la propiedad en el XML</param>
        /// <returns>Valor de la propiedad para BBDD</returns>
        private string ObtenerValorPropiedadTipoPresentacionGrupoComponentesDeXMLaBBDD(string pValorPropiedad)
        {
            #region Personalizacion
            string vistasGruposComponmentes = "/Views/CMSPagina/GrupoComponentes/";

            List<VistaVirtualCMS> filasGruposComponmentes = VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasGruposComponmentes)).ToList();

            Dictionary<string, TipoPresentacionGrupoComponentesCMS> diccionarioNombresGenericosVistaGrupos = new Dictionary<string, TipoPresentacionGrupoComponentesCMS>();
            Dictionary<string, Guid> diccionarioNombresPersonalizacionesVistaGrupos = new Dictionary<string, Guid>();
            if (filasGruposComponmentes.Count > 0)
            {
                foreach (VistaVirtualCMS filaVistaVirtualCMS in VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasGruposComponmentes)))
                {
                    string nombre = filaVistaVirtualCMS.Nombre;
                    if (string.IsNullOrEmpty(nombre))
                    {
                        nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                    }
                    bool agregado = false;
                    foreach (TipoPresentacionGrupoComponentesCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionGrupoComponentesCMS)))
                    {
                        if (filaVistaVirtualCMS.TipoComponente.EndsWith(tipoPresentacion.ToString() + ".cshtml"))
                        {
                            diccionarioNombresGenericosVistaGrupos.Add(nombre, tipoPresentacion);
                            agregado = true;
                            break;
                        }
                    }
                    if (!agregado)
                    {
                        diccionarioNombresPersonalizacionesVistaGrupos.Add(nombre, filaVistaVirtualCMS.PersonalizacionComponenteID);
                    }
                }
            }
            #endregion

            TipoPresentacionGrupoComponentesCMS tipoPresentacionGrupoComponentesCMS;
            if (Enum.TryParse<TipoPresentacionGrupoComponentesCMS>(pValorPropiedad, out tipoPresentacionGrupoComponentesCMS))
            {
                //No tiene personalizacion
                return ((short)tipoPresentacionGrupoComponentesCMS).ToString();
            }
            else
            {
                if (diccionarioNombresGenericosVistaGrupos.ContainsKey(pValorPropiedad))
                {
                    //Se trata de una vista genérica personalizada
                    return ((short)diccionarioNombresGenericosVistaGrupos[pValorPropiedad]).ToString();
                }
                else if (diccionarioNombresPersonalizacionesVistaGrupos.ContainsKey(pValorPropiedad))
                {
                    //Se trata de una vista personalizada
                    return diccionarioNombresPersonalizacionesVistaGrupos[pValorPropiedad].ToString();
                }
                else
                {
                    throw new Exception("La propiedad TipoPresentacionGrupoComponentes " + pValorPropiedad + " no es válida");
                }
            }
        }

        /// <summary>
        /// Obtiene el valor de la propiedad TipoPresentacionListadoRecursos teniendo en cuenta la personalización de las vistas
        /// </summary>
        /// <param name="pValorPropiedad">Valor de la propiedad en el XML</param>
        /// <returns>Valor de la propiedad para el BBDD</returns>
        private string ObtenerValorPropiedadTipoPresentacionListadoRecursosDeXMLaBBDD(string pValorPropiedad)
        {
            #region Personalizacion
            string vistasListadoRecursos = "/Views/CMSPagina/ListadoRecursos/";

            List<VistaVirtualCMS> filasListadoRecursos = VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasListadoRecursos)).ToList();

            Dictionary<string, TipoPresentacionListadoRecursosCMS> diccionarioNombresListadoGenericos = new Dictionary<string, TipoPresentacionListadoRecursosCMS>();
            Dictionary<string, Guid> diccionarioNombresListadoPersonalizaciones = new Dictionary<string, Guid>();
            if (vistasListadoRecursos.Length > 0)
            {
                foreach (VistaVirtualCMS filaVistaVirtualCMS in VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasListadoRecursos)))
                {
                    if (filaVistaVirtualCMS.TipoComponente.Replace(vistasListadoRecursos, "").IndexOf("/") == -1)
                    {
                        string nombre = filaVistaVirtualCMS.Nombre;
                        if (string.IsNullOrEmpty(nombre))
                        {
                            nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                        }
                        bool agregado = false;
                        foreach (TipoPresentacionListadoRecursosCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionListadoRecursosCMS)))
                        {
                            if (filaVistaVirtualCMS.TipoComponente.EndsWith(tipoPresentacion.ToString() + ".cshtml"))
                            {
                                diccionarioNombresListadoGenericos.Add(nombre, tipoPresentacion);
                                agregado = true;
                                break;
                            }
                        }
                        if (!agregado)
                        {
                            diccionarioNombresListadoPersonalizaciones.Add(nombre, filaVistaVirtualCMS.PersonalizacionComponenteID);
                        }
                    }
                }
            }
            #endregion

            TipoPresentacionListadoRecursosCMS tipoPresentacionListadoRecursosCMS;
            if (Enum.TryParse<TipoPresentacionListadoRecursosCMS>(pValorPropiedad, out tipoPresentacionListadoRecursosCMS))
            {
                //No tiene personalizacion
                return ((short)tipoPresentacionListadoRecursosCMS).ToString();
            }
            else
            {

                if (diccionarioNombresListadoGenericos.ContainsKey(pValorPropiedad))
                {
                    //Se trata de una vista genérica personalizada
                    return ((short)diccionarioNombresListadoGenericos[pValorPropiedad]).ToString();
                }
                else if (diccionarioNombresListadoPersonalizaciones.ContainsKey(pValorPropiedad))
                {
                    //Se trata de una vista personalizada
                    return diccionarioNombresListadoPersonalizaciones[pValorPropiedad].ToString();
                }
                else
                {
                    throw new Exception("La propiedad TipoPresentacionListadoRecursos " + pValorPropiedad + " no es válida");
                }
            }
        }

        /// <summary>
        /// Obtiene el valor de la propiedad TipoPresentacionRecurso teniendo en cuenta la personalización de las vistas
        /// </summary>
        /// <param name="pValorPropiedad">Valor de la propiedad en el XML</param>
        /// <returns>Valor de la propiedad para el BBDD</returns>
        private string ObtenerValorPropiedadTipoPresentacionRecursoDeXMLaBBDD(string pValorPropiedad)
        {
            #region Personalizacion
            string vistasRecursos = "/Views/CMSPagina/ListadoRecursos/Vistas/";

            List<VistaVirtualCMS> filasRecursos = VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasRecursos)).ToList();

            Dictionary<string, TipoPresentacionRecursoCMS> diccionarioNombresGenericos = new Dictionary<string, TipoPresentacionRecursoCMS>();
            Dictionary<string, Guid> diccionarioNombresPersonalizaciones = new Dictionary<string, Guid>();
            if (filasRecursos.Count > 0)
            {
                foreach (VistaVirtualCMS filaVistaVirtualCMS in VistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.TipoComponente.StartsWith(vistasRecursos)))
                {
                    string nombre = filaVistaVirtualCMS.Nombre;
                    if (string.IsNullOrEmpty(nombre))
                    {
                        nombre = filaVistaVirtualCMS.PersonalizacionComponenteID.ToString();
                    }
                    bool agregado = false;
                    foreach (TipoPresentacionRecursoCMS tipoPresentacion in Enum.GetValues(typeof(TipoPresentacionRecursoCMS)))
                    {
                        if (filaVistaVirtualCMS.TipoComponente.EndsWith(tipoPresentacion.ToString() + ".cshtml"))
                        {
                            diccionarioNombresGenericos.Add(nombre, tipoPresentacion);
                            agregado = true;
                            break;
                        }
                    }
                    if (!agregado)
                    {
                        diccionarioNombresPersonalizaciones.Add(nombre, filaVistaVirtualCMS.PersonalizacionComponenteID);
                    }
                }
            }
            #endregion

            TipoPresentacionRecursoCMS tipoPresentacionRecursoCMS;
            if (Enum.TryParse<TipoPresentacionRecursoCMS>(pValorPropiedad, out tipoPresentacionRecursoCMS))
            {
                //No tiene personalizacion
                return ((short)tipoPresentacionRecursoCMS).ToString();
            }
            else
            {
                if (diccionarioNombresGenericos.ContainsKey(pValorPropiedad))
                {
                    //Se trata de una vista genérica personalizada
                    return ((short)diccionarioNombresGenericos[pValorPropiedad]).ToString();
                }
                else if (diccionarioNombresPersonalizaciones.ContainsKey(pValorPropiedad))
                {
                    //Se trata de una vista personalizada
                    return diccionarioNombresPersonalizaciones[pValorPropiedad].ToString();
                }
                else
                {
                    throw new Exception("La propiedad TipoPresentacionGrupoComponentes " + pValorPropiedad + " no es válida");
                }
            }


            //try
            //{
            //    TipoPresentacionRecursoCMS tipoPresentacionRecursoCMS = (TipoPresentacionRecursoCMS)Enum.Parse(typeof(TipoPresentacionRecursoCMS), pValorPropiedad);
            //    //No tiene personalizacion
            //    return ((short)tipoPresentacionRecursoCMS).ToString();
            //}
            //catch
            //{
            //    if (diccionarioNombresGenericos.ContainsKey(pValorPropiedad))
            //    {
            //        //Se trata de una vista genérica personalizada
            //        return ((short)diccionarioNombresGenericos[pValorPropiedad]).ToString();
            //    }
            //    else if (diccionarioNombresPersonalizaciones.ContainsKey(pValorPropiedad))
            //    {
            //        //Se trata de una vista personalizada
            //        return diccionarioNombresPersonalizaciones[pValorPropiedad].ToString();
            //    }
            //    else
            //    {
            //        throw new Exception("La propiedad TipoPresentacionGrupoComponentes " + pValorPropiedad + " no es válida");
            //    }
            //}
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Proyecto Actual del XML
        /// </summary>
        public Proyecto ProyectoActual
        {
            get
            {
                return mProyectoActual;
            }
        }

        /// <summary>
        /// Gestor del CMS del Proyecto Actual
        /// </summary>
        public GestionCMS gestorCMS
        {
            get
            {
                if (mGestorCMS == null)
                {
                    CMSCN CMSCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mGestorCMS = new GestionCMS(CMSCN.ObtenerCMSDeProyecto(ProyectoActual.Clave), mLoggingService, mEntityContext);
                    CMSCN.Dispose();
                }
                return mGestorCMS;
            }
        }

        /// <summary>
        /// DATASET con las personalizaciones del Proyecto Actual
        /// </summary>
        public DataWrapperVistaVirtual VistaVirtualDW
        {
            get
            {
                if (mVistaVirtualDW == null)
                {
                    VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mVistaVirtualDW = vistaVirtualCL.ObtenerVistasVirtualPorProyectoID(ProyectoActual.Clave, PersonalizacionEcosistemaID, ComunidadExcluidaPersonalizacionEcosistema);
                    vistaVirtualCL.Dispose();
                }
                return mVistaVirtualDW;
            }
        }

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin metaproyecto
        /// </summary>
        public Guid PersonalizacionEcosistemaID
        {
            get
            {
                if (!mPersonalizacionEcosistemaID.HasValue)
                {
                    mPersonalizacionEcosistemaID = Guid.Empty;
                    List<ParametroAplicacion> busqueda = ParametroAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.PersonalizacionEcosistemaID.ToString())).ToList();
                    if (busqueda.Count > 0)
                    {
                        //mPersonalizacionEcosistemaID = new Guid(ParametroAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.PersonalizacionEcosistemaID.ToString() + "'")[0]["Valor"].ToString());
                        mPersonalizacionEcosistemaID = new Guid(busqueda.First().Valor);
                    }
                }
                return mPersonalizacionEcosistemaID.Value;
            }
        }

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin metaproyecto
        /// </summary>
        public bool ComunidadExcluidaPersonalizacionEcosistema
        {
            get
            {
                if (!mComunidadExcluidaPersonalizacionEcosistema.HasValue)
                {
                    mComunidadExcluidaPersonalizacionEcosistema = false;

                    List<ParametroAplicacion> busqueda = ParametroAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.ComunidadesExcluidaPersonalizacion.ToString())).ToList();

                    if (busqueda.Count > 0)
                    {
                        //List<string> listaComunidadesExcluidas = new List<string>(ParametroAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.ComunidadesExcluidaPersonalizacion.ToString() + "'")[0]["Valor"].ToString().Split(','));
                        List<string> listaComunidadesExcluidas = new List<string>(busqueda.First().Valor.ToString().Split(','));

                        mComunidadExcluidaPersonalizacionEcosistema = listaComunidadesExcluidas.Contains(ProyectoActual.Clave.ToString());
                    }
                }
                return mComunidadExcluidaPersonalizacionEcosistema.Value;
            }
        }

        #endregion Propiedades
    }

    public class FilasDependientesCMSBloque
    {
        public List<AD.EntityModel.Models.CMS.CMSBloque> ListaCMSBloque { get; }
        public List<AD.EntityModel.Models.CMS.CMSBloqueComponente> ListaCMSBloqueComponente { get; }
        public List<AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente> ListaCMSBloqueComponentePropiedadComponente { get; }

        public FilasDependientesCMSBloque()
        {
            ListaCMSBloque = new List<AD.EntityModel.Models.CMS.CMSBloque>();
            ListaCMSBloqueComponente = new List<AD.EntityModel.Models.CMS.CMSBloqueComponente>();
            ListaCMSBloqueComponentePropiedadComponente = new List<AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente>();
        }
    }
}
