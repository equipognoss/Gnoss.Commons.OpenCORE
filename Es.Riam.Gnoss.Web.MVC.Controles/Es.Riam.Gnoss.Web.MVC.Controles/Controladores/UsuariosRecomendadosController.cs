using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controles.Controladores
{
    public class UsuariosRecomendadosController : ControladorBase
    {
        private ControllerBaseGnoss ControllerBase;
        private Identidad mIdentidadActual;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        /// <summary>
        /// Constructor a partir de la página que contiene al controlador
        /// </summary>
        /// <param name="pController">Controller</param>
        public UsuariosRecomendadosController(ControllerBaseGnoss pController, Identidad pIdentidadActual, EntityContext entityContext, LoggingService loggingService, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, GnossCache gnossCache, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<UsuariosRecomendadosController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            ControllerBase = pController;
            mIdentidadActual = pIdentidadActual;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public List<ProfileModel> ObtenerUsuariosRecomendados(int pNumeroElementos)
        {
            Identidad pIdentidadActual = mIdentidadActual;
            string pUrlIntragnoss =ControllerBase.UrlIntragnoss; 
            Proyecto pProyectoSeleccionado = ControllerBase.ProyectoSeleccionado;
            ParametroGeneral pParametrosGeneralesRow =ControllerBase.ParametrosGeneralesRow;
            string pBaseURLIdioma =ControllerBase.BaseURLIdioma; 
            UtilIdiomas pUtilIdiomas =ControllerBase.UtilIdiomas;
            Dictionary<string, List<string>> pInformacionOntologias = ControllerBase.InformacionOntologias;

            List<ProfileModel> listaPerfilesObtenidos = new List<ProfileModel>();
            List<Guid> mListaIdentidades = null;

            string localidad = "";
            string provincia = "";
            string pais = "";

            Dictionary<string, string> listaDatosExtra = new Dictionary<string, string>();
            List<Guid> listaCategoriasSuscritas = new List<Guid>();

            PaisCL paisCL = new PaisCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PaisCL>(), mLoggerFactory);
            DataWrapperPais paisDW = paisCL.ObtenerPaisesProvincias();

            //Obtengo el país, provincia y localidad:
            if (pIdentidadActual.Persona.FilaPersona.LocalidadPersonal!=null)
            {
                localidad = pIdentidadActual.Persona.FilaPersona.LocalidadPersonal;
            }
            if (pIdentidadActual.Persona.FilaPersona.PaisPersonalID.HasValue && !pIdentidadActual.Persona.FilaPersona.PaisPersonalID.Equals(Guid.Empty))
            {
                pais = paisDW.ListaPais.Where(item => item.PaisID.Equals(pIdentidadActual.Persona.FilaPersona.PaisPersonalID.Value)).FirstOrDefault().Nombre;
            }
            if (pIdentidadActual.Persona.FilaPersona.ProvinciaPersonalID.HasValue && !pIdentidadActual.Persona.FilaPersona.ProvinciaPersonalID.Equals(Guid.Empty))
            {
                provincia = paisDW.ListaProvincia.Where(item => item.ProvinciaID.Equals(pIdentidadActual.Persona.FilaPersona.ProvinciaPersonalID.Value)).FirstOrDefault().Nombre;
            }
            else if (pIdentidadActual.Persona.FilaPersona.ProvinciaPersonal!=null)
            {
                provincia = pIdentidadActual.Persona.FilaPersona.LocalidadPersonal;
            }

            SuscripcionCN suscripcionCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCN>(), mLoggerFactory);
            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);

            //Obtengo los datos extra de registro
            DataWrapperDatoExtra dataWrapperDatoExtra = identCN.ObtenerIdentidadDatoExtraRegistroDeProyecto(pProyectoSeleccionado.Clave, pIdentidadActual.Clave);

            foreach (AD.EntityModel.Models.ProyectoDS.Triples fila in dataWrapperDatoExtra.ListaTriples)
            {
                string predicado = fila.PredicadorRDF;
                if (!listaDatosExtra.ContainsKey(predicado))
                {
                    listaDatosExtra.Add(predicado, fila.Opcion);
                }
            }
            
            //Obtengo las categorías de tesauro a las que está suscrito un usuario en un proyecto
            dataWrapperDatoExtra = suscripcionCN.ObtenerCategoriasTesauroDeProyectoSuscritaIdentidad(pProyectoSeleccionado.Clave, pIdentidadActual.Clave);

            foreach (AD.EntityModel.Models.ProyectoDS.Triples fila in dataWrapperDatoExtra.ListaTriples)
            {
                listaCategoriasSuscritas.Add(new Guid(fila.Opcion));
            }

            FacetadoCN facetadoCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);

            // En la replica no existe el triple http://gnoss/hasPopularidad
            // Grafo: Inevery Crea Costa Rica
            // SELECT * FROM <http://gnoss.com/df68d032-5358-4e1a-8214-b58b74836afa> WHERE {?s ?p ?o. FILTER(?s = <http://gnoss/E55860AB-F702-4F78-A296-43E8FE9F907E>)}

            // Incidencias relacionadas
            // ICE-206
            // ICE-99
            // ICE-78

            facetadoCN.InformacionOntologias = pInformacionOntologias;

            FacetadoDS facetadoDS = new FacetadoDS();
            bool recomendacionescargadas = false;
            if (!pIdentidadActual.EsIdentidadInvitada)
            {
                try
                {
                    if (!string.IsNullOrEmpty(pParametrosGeneralesRow.AlgoritmoPersonasRecomendadas) && !pParametrosGeneralesRow.AlgoritmoPersonasRecomendadas.Equals("True"))
                    {
                        facetadoDS = new FacetadoDS();
                        //string props = "http://www.w3.org/2006/vcard/ns#locality=4|||http://www.w3.org/2006/vcard/ns#country-name=3|||http://d.opencalais.com/1/type/er/Geo/ProvinceOrState=3|||http://xmlns.com/foaf/0.1/interest=1|||http://gnoss/hasPopularidad=0,02";
                        string props = pParametrosGeneralesRow.AlgoritmoPersonasRecomendadas;

                        string[] propiedades = props.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
                        Dictionary<string, float> listaPropiedadesPeso = new Dictionary<string, float>();
                        foreach (string propiedad in propiedades)
                        {
                            string[] propiedadPeso = propiedad.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            string prop = propiedadPeso[0].Trim();
                            float peso = float.Parse(propiedadPeso[1].Trim());
                            if (!listaPropiedadesPeso.ContainsKey(prop))
                            {
                                listaPropiedadesPeso.Add(prop, peso);
                            }
                        }

                        #region Obtenemos datos de usuario

                        Dictionary<string, Object> valorPropiedades = new Dictionary<string, object>();

                        foreach (string propiedad in listaPropiedadesPeso.Keys)
                        {
                            switch (propiedad)
                            {
                                case "http://www.w3.org/2006/vcard/ns#locality":
                                    valorPropiedades.Add(propiedad, localidad);
                                    break;
                                case "http://d.opencalais.com/1/type/er/Geo/ProvinceOrState":
                                    valorPropiedades.Add(propiedad, provincia);
                                    break;
                                case "http://www.w3.org/2006/vcard/ns#country-name":
                                    valorPropiedades.Add(propiedad, pais);
                                    break;
                                case "http://xmlns.com/foaf/0.1/interest":
                                    valorPropiedades.Add(propiedad, listaCategoriasSuscritas);
                                    break;
                                case "http://gnoss/hasPopularidad":
                                    valorPropiedades.Add(propiedad, null);
                                    break;
                                default:
                                    if (listaDatosExtra.ContainsKey(propiedad))
                                    {
                                        valorPropiedades.Add(propiedad, listaDatosExtra[propiedad]);
                                    }
                                    break;
                            }
                        }


                        #endregion

                        if (valorPropiedades.Count > 0)
                        {
                            facetadoDS = facetadoCN.ObtenerPersonasRecomendadas(pProyectoSeleccionado.Clave, pIdentidadActual.Clave, listaPropiedadesPeso, valorPropiedades, pNumeroElementos + 20);
                        }
                        recomendacionescargadas = true;
                    }
                }
                catch (Exception ex)
                {
                    ControllerBase.GuardarLogError(ex.ToString());
                }

                if (!recomendacionescargadas)
                {
                    facetadoDS = facetadoCN.ObtenerPersonasRecomendadas(pProyectoSeleccionado.Clave, pIdentidadActual.Clave, localidad, provincia, pais, listaDatosExtra, listaCategoriasSuscritas, pNumeroElementos + 20);
                }
            }

            mListaIdentidades = new List<Guid>();
            if (facetadoDS.Tables.Count > 0)
            {
                foreach (DataRow fila in facetadoDS.Tables[0].Rows)
                {
                    string id = (string)fila[0];
                    mListaIdentidades.Add(new Guid(id.Substring(id.LastIndexOf('/') + 1)));
                }
            }

            if (mListaIdentidades.Count > 0)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerPerfilIdentidadDeIdentidadesEnProyectoNoSuscritas(pIdentidadActual.IdentidadMyGNOSS.Clave, mListaIdentidades, pProyectoSeleccionado.Clave), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.Dispose();

                List<ProfileModel> listaPerfiles = new List<ProfileModel>();

                int i = 0;
                foreach (Guid identidadID in mListaIdentidades)
                {
                    if (gestorIdentidades.ListaIdentidades.ContainsKey(identidadID) && !gestorIdentidades.ListaIdentidades[identidadID].Tipo.Equals(TiposIdentidad.Organizacion) && !gestorIdentidades.ListaIdentidades[identidadID].Tipo.Equals(TiposIdentidad.ProfesionalCorporativo) && identidadID != pIdentidadActual.Clave)
                    {
                        if (i >= pNumeroElementos)
                        {
                            break;
                        }
                        i++;

                        Identidad identidad = gestorIdentidades.ListaIdentidades[identidadID];

                        ProfileModel perfilContacta = new ProfileModel();
                        perfilContacta.NamePerson = identidad.NombreCompuesto();

                        if (!identidad.UrlImagen.ToLower().Contains("personas/anonimo") && !identidad.UrlImagen.ToLower().Contains("organizaciones/anonimo"))
                        {
                            perfilContacta.UrlFoto = UtilArchivos.ContentImagenes + identidad.UrlImagen;
                        }

                        perfilContacta.UrlPerson = UrlsSemanticas.GetURLPerfilDeIdentidad(pBaseURLIdioma, pProyectoSeleccionado.NombreCorto, pUtilIdiomas, identidad);
                        perfilContacta.ListActions = new ProfileModel.UrlActions();
                        perfilContacta.ListActions.UrlFollow = perfilContacta.UrlPerson.TrimEnd('/') + "/follow";

                        listaPerfiles.Add(perfilContacta);
                    }
                }

                listaPerfilesObtenidos = listaPerfiles;
            }
            return listaPerfilesObtenidos;
        }
    }
}
