using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Semantica.OWL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf.Meta;
using Microsoft.Extensions.Logging;
using Es.Riam.Gnoss.AD.ParametroAplicacion;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorProyectoPestanyaBusquedaPesoController : ControladorBase
    {
        #region Miembros


        #endregion

        #region Constructores

        public ControladorProyectoPestanyaBusquedaPesoController(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorProyectoPestanyaBusquedaPesoController> logger, ILoggerFactory loggerFactory) : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        { }

        #endregion

        #region Metodos públicos

        public List<ProyectoPestanyaBusquedaOCModel> CargarListaPestanyasBusquedaOCs()
        {
            List<ProyectoPestanyaBusquedaOCModel> listaPestanyasBusquedaOcs = new List<ProyectoPestanyaBusquedaOCModel>();
            DataWrapperProyecto dataWrapperProyecto = CargarDataWrapperProyecto();
            foreach (ProyectoPestanyaBusqueda proyectoPestanyaBusqueda in dataWrapperProyecto.ListaProyectoPestanyaBusqueda)
            {
                ProyectoPestanyaBusquedaOCModel proyectoPestanyaBusquedaOCModel = new ProyectoPestanyaBusquedaOCModel()
                {
                    PesosSubtiposOntos = new List<PesoSubtiposOntoModel>(),
                    TipoAutocompletar = (short)proyectoPestanyaBusqueda.TipoAutocompletar
                };
                proyectoPestanyaBusquedaOCModel.PestanyaID = proyectoPestanyaBusqueda.PestanyaID;
                ProyectoPestanyaMenu proyectoPestanyaMenu = dataWrapperProyecto.ListaProyectoPestanyaMenu.Find(item => item.PestanyaID.Equals(proyectoPestanyaBusqueda.PestanyaID));
                proyectoPestanyaBusquedaOCModel.NombrePestanya = proyectoPestanyaMenu.Nombre;
                proyectoPestanyaBusquedaOCModel.Url = proyectoPestanyaMenu.Ruta;

                List<string> ontologiasBusqueda = proyectoPestanyaBusqueda.CampoFiltro.Split("|").Where(item => item.StartsWith("rdf:type") || item.StartsWith("gnoss:type")).ToList();
                foreach (string ontologiaBusqueda in ontologiasBusqueda)
                {
                    string elementoBuscado = ontologiaBusqueda.Replace("rdf:type=", "").Replace("gnoss:type=", "");
                    string ontologia = string.Empty;
                    string subtipoConfigurado = null;

                    OntologiaProyecto ontologiaProyecto = null;
                    ontologiaProyecto = dataWrapperProyecto.ListaOntologiaProyecto.FirstOrDefault(item => item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.OntologiaProyecto1.Equals(elementoBuscado));
                    if (ontologiaProyecto == null)
                    {//para el gnoss:type, ejemplo cidoc:E22_Man-Made_Object, que esto aparece en los subtipos de la ontologia
                        ontologiaProyecto = dataWrapperProyecto.ListaOntologiaProyecto.FirstOrDefault(item => item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.SubTipos.Contains(elementoBuscado));
                        if (ontologiaProyecto != null && ontologiaBusqueda.Contains("gnoss:type="))
                        {
                            ontologia = ontologiaProyecto.OntologiaProyecto1;
                            subtipoConfigurado = elementoBuscado;
                        }
                    }
                    else
                    {
                        ontologia = elementoBuscado;
                    }

                    if (ontologiaProyecto != null)
                    {
                        if (!string.IsNullOrEmpty(ontologiaProyecto.SubTipos))
                        {
                            List<PesoSubtiposOntoModel> ListaPesoSubtiposOntoModel = ObtenerPesosSubtiposPorPestanya(dataWrapperProyecto, ontologiaProyecto, proyectoPestanyaBusqueda.PestanyaID, ontologia, subtipoConfigurado).OrderByDescending(item => item.Peso).ToList();

                            proyectoPestanyaBusquedaOCModel.PesosSubtiposOntos.AddRange(ListaPesoSubtiposOntoModel);
                        }
                        if (proyectoPestanyaBusquedaOCModel.PesosSubtiposOntos.Count == 0)
                        {

                            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                            Guid ontologiaID = documentacionCN.ObtenerOntologiaAPartirNombre(ProyectoSeleccionado.Clave, $"{ontologiaProyecto.OntologiaProyecto1}.owl");
                            Ontologia archivoOntologia = ObtenerOntologia(ontologiaID);
                            string entidadPrincipal = archivoOntologia.Entidades.Where(item => !archivoOntologia.EntidadesAuxiliares.Select(aux => aux.TipoEntidad).Contains(item.TipoEntidad)).Select(item => item.TipoEntidad).FirstOrDefault();

                            int peso = dataWrapperProyecto.ListaProyectoPestanyaBusquedaPesoOC.Where(item => item.PestanyaID.Equals(proyectoPestanyaBusqueda.PestanyaID) && item.Tipo.Equals(entidadPrincipal)).Select(item => item.Peso).FirstOrDefault();
                            PesoSubtiposOntoModel pesoSubtiposOntoModel = new PesoSubtiposOntoModel()
                            {
                                Ontologia = ontologia,
                                Subtipo = entidadPrincipal,
                                Peso = peso
                            };
                            proyectoPestanyaBusquedaOCModel.PesosSubtiposOntos.Add(pesoSubtiposOntoModel);
                        }
                    }
                }
                if (proyectoPestanyaBusquedaOCModel.PesosSubtiposOntos.Count > 0)
                {
                    listaPestanyasBusquedaOcs.Add(proyectoPestanyaBusquedaOCModel);
                }
            }

            return listaPestanyasBusquedaOcs;
        }

        public void GuardarPestanyaSubTiposPeso(PestanyaSubTiposPesoModel pestanyaSubTiposPesosModel)
        {
            //Hay que cambiar también el tipo de autocompletado
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            if (pestanyaSubTiposPesosModel.TipoAutocompletar != 0)
            {
                proyectoCN.CambiarTipoAutocompletadoProyecto(ProyectoSeleccionado.Clave, pestanyaSubTiposPesosModel.PestanyaID, pestanyaSubTiposPesosModel.TipoAutocompletar);
                List<ProyectoPestanyaBusquedaPesoOC> listaProyectoPestanyaBusquedaPesoOC = new List<ProyectoPestanyaBusquedaPesoOC>();
                foreach (SubtiposPeso subtiposPeso in pestanyaSubTiposPesosModel.SubtiposPesos)
                {
                    ProyectoPestanyaBusquedaPesoOC proyectoPestanyaBusquedaPesoOC = new ProyectoPestanyaBusquedaPesoOC()
                    {
                        OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID,
                        ProyectoID = ProyectoSeleccionado.Clave,
                        OntologiaProyecto1 = subtiposPeso.Ontologia,
                        PestanyaID = pestanyaSubTiposPesosModel.PestanyaID,
                        Tipo = subtiposPeso.Subtipo,
                        Peso = subtiposPeso.Peso
                    };
                    listaProyectoPestanyaBusquedaPesoOC.Add(proyectoPestanyaBusquedaPesoOC);
                }
                proyectoCN.GuardarProyectoPestanyaBusquedaPeso(listaProyectoPestanyaBusquedaPesoOC);
            }
            else
            {
                proyectoCN.CambiarTipoAutocompletadoProyecto(ProyectoSeleccionado.Clave, pestanyaSubTiposPesosModel.PestanyaID, (short)TipoAutocompletar.Basico);
                proyectoCN.EliminarProyectoPestanyaPesosBusqueda(ProyectoSeleccionado.Clave, pestanyaSubTiposPesosModel.PestanyaID);
            }
            proyectoCN.Dispose();
        }

        /// <summary>
        /// Metodo que se utiliza para subir cambios en el api de despligues
        /// </summary>
        /// <param name="pPestanyaSubtiposPesosModel"></param>
        public void GuardarPestanyaSubTiposPeso(List<ProyectoPestanyaBusquedaOCModel> pPestanyaSubtiposPesosModel)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            foreach (ProyectoPestanyaBusquedaOCModel modelo in pPestanyaSubtiposPesosModel)
            {
                if (modelo.TipoAutocompletar != 0)
                {
                    proyectoCN.CambiarTipoAutocompletadoProyecto(ProyectoSeleccionado.Clave, modelo.PestanyaID, modelo.TipoAutocompletar);
                    List<ProyectoPestanyaBusquedaPesoOC> listaProyectoPestanyaBusquedaPesoOC = new List<ProyectoPestanyaBusquedaPesoOC>();
                    foreach (PesoSubtiposOntoModel subtiposPeso in modelo.PesosSubtiposOntos)
                    {
                        ProyectoPestanyaBusquedaPesoOC proyectoPestanyaBusquedaPesoOC = new ProyectoPestanyaBusquedaPesoOC()
                        {
                            OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID,
                            ProyectoID = ProyectoSeleccionado.Clave,
                            OntologiaProyecto1 = subtiposPeso.Ontologia,
                            PestanyaID = modelo.PestanyaID,
                            Tipo = subtiposPeso.Subtipo,
                            Peso = (short)subtiposPeso.Peso
                        };
                        listaProyectoPestanyaBusquedaPesoOC.Add(proyectoPestanyaBusquedaPesoOC);
                    }
                    proyectoCN.GuardarProyectoPestanyaBusquedaPeso(listaProyectoPestanyaBusquedaPesoOC);
                }
                else
                {
                    proyectoCN.CambiarTipoAutocompletadoProyecto(ProyectoSeleccionado.Clave, modelo.PestanyaID, (short)TipoAutocompletar.Basico);
                    proyectoCN.EliminarProyectoPestanyaPesosBusqueda(ProyectoSeleccionado.Clave, modelo.PestanyaID);
                }
            }
            proyectoCN.Dispose();
        }

        #endregion

        #region Métodos privados

        private Ontologia ObtenerOntologia(Guid pOntologiaID)
        {
            CallFileService fileService = new CallFileService(mConfigService, mLoggingService);
            byte[] bytesArchivo = fileService.ObtenerOntologiaBytes(pOntologiaID);
            Ontologia ontologia = new Ontologia(bytesArchivo);
            ontologia.LeerOntologia();
            return ontologia;
        }

        private static Dictionary<string, string> ObtenerPrefijoNamespace(OntologiaProyecto pOntologiaProyecto)
        {
            Dictionary<string, string> prefijoNamespaces = new Dictionary<string, string>();
            List<string> namesPacesExtraCortoLargoLista = pOntologiaProyecto.NamespacesExtra.Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string namesPaces in namesPacesExtraCortoLargoLista)
            {
                int indexPuntos = namesPaces.IndexOf(':');
                string namespaceCorto = namesPaces.Substring(0, indexPuntos);
                string namespaceLargo = namesPaces.Substring(indexPuntos + 1);
                prefijoNamespaces.Add(namespaceCorto, namespaceLargo);
            }
            return prefijoNamespaces;
        }

        private static Dictionary<string, List<string>> ObtenerPrefijoSubtipos(OntologiaProyecto pOntologiaProyecto)
        {
            string[] subtiposLista = pOntologiaProyecto.SubTipos.Split("[|||]", StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, List<string>> prefijoOntoSubtipo = new Dictionary<string, List<string>>();
            foreach (string subtipoIdioma in subtiposLista)
            {
                string[] subtipoConPrefijoArray = subtipoIdioma.Split("|||", StringSplitOptions.RemoveEmptyEntries);
                if (subtipoConPrefijoArray.Length > 0)
                {
                    string subtipoConPrefijo = subtipoConPrefijoArray[0];
                    string prefijoOnto = subtipoConPrefijo.Split(':')[0];
                    string subtipo = subtipoConPrefijo.Split(':')[1];
                    if (!prefijoOntoSubtipo.ContainsKey(prefijoOnto))
                    {
                        List<string> subtipos = new List<string>() { subtipo };
                        prefijoOntoSubtipo.Add(prefijoOnto, subtipos);
                    }
                    else
                    {
                        prefijoOntoSubtipo[prefijoOnto].Add(subtipo);
                    }
                }
            }
            return prefijoOntoSubtipo;
        }

        private List<PesoSubtiposOntoModel> ObtenerPesosSubtiposPorPestanya(DataWrapperProyecto pDataWrapperProyecto, OntologiaProyecto pOntologiaProyecto, Guid pPestanyaID, string pNombreOnto, string pSubtipoConfigurado = null)
        {
            List<PesoSubtiposOntoModel> listaPesoSubtiposOntoModel = new List<PesoSubtiposOntoModel>();
            Dictionary<string, List<string>> prefijoOntoSubtipo = ObtenerPrefijoSubtipos(pOntologiaProyecto);

            Dictionary<string, string> namesPacesExtraCortoLargoDiccionario = ObtenerPrefijoNamespace(pOntologiaProyecto);
            if (!string.IsNullOrEmpty(pSubtipoConfigurado))
            {
                string[] prefijoPropiedad = pSubtipoConfigurado.Split(':');
                string prefijo = prefijoPropiedad[0];
                string subtipoPropiedad = prefijoPropiedad[1];
                if (prefijoOntoSubtipo.ContainsKey(prefijo))
                {
                    string tipo = $"{namesPacesExtraCortoLargoDiccionario[prefijo]}{subtipoPropiedad}";
                    int peso = pDataWrapperProyecto.ListaProyectoPestanyaBusquedaPesoOC.Where(item => item.PestanyaID.Equals(pPestanyaID) && item.Tipo.Equals(tipo)).Select(item => item.Peso).FirstOrDefault();
                    PesoSubtiposOntoModel pesoSubtiposOntoModel = new PesoSubtiposOntoModel()
                    {
                        Ontologia = pNombreOnto,
                        Subtipo = tipo,
                        Peso = peso
                    };
                    listaPesoSubtiposOntoModel.Add(pesoSubtiposOntoModel);
                }
            }
            else
            {
                foreach (string prefijo in namesPacesExtraCortoLargoDiccionario.Keys.Where(item => prefijoOntoSubtipo.ContainsKey(item)))
                {
                    foreach (string subtipo in prefijoOntoSubtipo[prefijo])
                    {
                        string tipo = $"{namesPacesExtraCortoLargoDiccionario[prefijo]}{subtipo}";
                        int peso = pDataWrapperProyecto.ListaProyectoPestanyaBusquedaPesoOC.Where(item => item.PestanyaID.Equals(pPestanyaID) && item.Tipo.Equals(tipo)).Select(item => item.Peso).FirstOrDefault();
                        PesoSubtiposOntoModel pesoSubtiposOntoModel = new PesoSubtiposOntoModel()
                        {
                            Ontologia = pNombreOnto,
                            Subtipo = tipo,
                            Peso = peso
                        };
                        listaPesoSubtiposOntoModel.Add(pesoSubtiposOntoModel);
                    }
                }
            }
            return listaPesoSubtiposOntoModel;
        }

        private DataWrapperProyecto CargarDataWrapperProyecto()
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            return proyectoCN.ObtenerInformacionAutocompletadoEnriquecidoProyecto(ProyectoSeleccionado.Clave);
        }

        #endregion

    }
}
