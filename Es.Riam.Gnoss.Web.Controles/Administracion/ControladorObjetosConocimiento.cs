using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorObjetosConocimiento
    {
        private DataWrapperProyecto mDataWrapperProyecto;
        private Proyecto ProyectoSeleccionado = null;
        private Dictionary<string, string> ParametroProyecto = null;

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public ControladorObjetosConocimiento(Proyecto pProyecto, Dictionary<string, string> pParametroProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;

            ProyectoSeleccionado = pProyecto;
            ParametroProyecto = pParametroProyecto;
        }

        #endregion

        #region Metodos de Carga
        public ObjetoConocimientoModel CargarObjetoConocimiento(string pNombreOntologia)
        {
            ObjetoConocimientoModel resultado = null;
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            using (DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, null))
            {
                documentacionCN.ObtenerDatasetConOntologiaAPartirNombre(ProyectoSeleccionado.Clave, pNombreOntologia, dataWrapperDocumentacion);
                if (dataWrapperDocumentacion.ListaDocumento.Count!=0)
                {
                    resultado = CargarObjetoConocimiento(dataWrapperDocumentacion.ListaDocumento.FirstOrDefault());
                }
            }
            return resultado;
        }
        public ObjetoConocimientoModel CargarObjetoConocimiento(AD.EntityModel.Models.Documentacion.Documento filaDoc)
        {
            string ontologiaProyecto = filaDoc.Enlace.Replace(".owl", "");
            //.FindByOrganizacionIDProyectoIDOntologiaProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, ontologiaProyecto);
            OntologiaProyecto filaOntologia = DataWrapperProyecto.ListaOntologiaProyecto.FirstOrDefault(onto=>onto.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && onto.ProyectoID.Equals(ProyectoSeleccionado.Clave) && onto.OntologiaProyecto1.Equals(ontologiaProyecto, StringComparison.InvariantCultureIgnoreCase));

            Guid ontologiaID = filaDoc.DocumentoID;

            if (filaOntologia != null)
            {
                ObjetoConocimientoModel objetoConocimiento = new ObjetoConocimientoModel();
                objetoConocimiento.Ontologia = filaOntologia.OntologiaProyecto1;
                objetoConocimiento.Name = filaOntologia.NombreOnt;
                objetoConocimiento.ShortNameOntology = filaOntologia.NombreCortoOnt;
                objetoConocimiento.Namespace = filaOntologia.Namespace;
                objetoConocimiento.NamespaceExtra = filaOntologia.NamespacesExtra;

                TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, null);
                objetoConocimiento.NombreTesauroExclusivo = tesauroCN.ObtenerNombreTesauroProyOnt(ProyectoSeleccionado.Clave, ontologiaID.ToString());
                objetoConocimiento.CachearDatosSemanticos = filaOntologia.CachearDatosSemanticos;
                objetoConocimiento.EsBuscable = filaOntologia.EsBuscable;

                objetoConocimiento.Subtipos = new Dictionary<string, string>();
                
                if (!string.IsNullOrEmpty(filaOntologia.SubTipos))
                {
                    foreach (string datosSubTipo in filaOntologia.SubTipos.Split(new string[] { "[|||]" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string subTipo = datosSubTipo.Substring(0, datosSubTipo.IndexOf("|||"));
                        string nombre = datosSubTipo.Substring(datosSubTipo.IndexOf("|||") + 3);

                        objetoConocimiento.Subtipos.Add(subTipo, nombre);
                    }
                }

                objetoConocimiento.PresentacionListado = CargarPresentacionListado(ontologiaID);
                objetoConocimiento.PresentacionMosaico = CargarPresentacionMosaico(ontologiaID);
                objetoConocimiento.PresentacionMapa = CargarPresentacionMapa(ontologiaID);
                objetoConocimiento.PresentacionRelacionados = CargarPresentacionRecRelacionados(ontologiaID);
                objetoConocimiento.PresentacionPersonalizado = CargarPresentacionPersonalizado(ontologiaID);

                return objetoConocimiento;
            }

            return null;
        }

        private ObjetoConocimientoModel.PresentacionModel CargarPresentacionListado(Guid pOntologiaID)
        {
            ObjetoConocimientoModel.PresentacionModel PresentacionListado = new ObjetoConocimientoModel.PresentacionModel();

            PresentacionListado.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionModel.PropiedadModel>();

            foreach (AD.EntityModel.Models.ProyectoDS.PresentacionListadoSemantico filaListadoSem in DataWrapperProyecto.ListaPresentacionListadoSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID))
            {
                if (string.IsNullOrEmpty(filaListadoSem.Ontologia))
                {
                    if (filaListadoSem.Propiedad == "descripcion")
                    {
                        PresentacionListado.MostrarDescripcion = true;
                    }
                    else if (filaListadoSem.Propiedad == "publicador")
                    {
                        PresentacionListado.MostrarPublicador = true;
                    }
                    else if (filaListadoSem.Propiedad == "etiquetas")
                    {
                        PresentacionListado.MostrarEtiquetas = true;
                    }
                    else if (filaListadoSem.Propiedad == "categorias")
                    {
                        PresentacionListado.MostrarCategorias = true;
                    }
                }
                else
                {
                    ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad = new ObjetoConocimientoModel.PresentacionModel.PropiedadModel();

                    propiedad.Propiedad = filaListadoSem.Propiedad;
                    propiedad.Presentacion = filaListadoSem.Nombre;
                    propiedad.Orden = filaListadoSem.Orden;

                    PresentacionListado.ListaPropiedades.Add(propiedad);
                }
            }
            return PresentacionListado;
        }

        private ObjetoConocimientoModel.PresentacionModel CargarPresentacionMosaico(Guid pOntologiaID)
        {
            ObjetoConocimientoModel.PresentacionModel PresentacionMosaico = new ObjetoConocimientoModel.PresentacionModel();

            PresentacionMosaico.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionModel.PropiedadModel>();

            foreach (AD.EntityModel.Models.ProyectoDS.PresentacionMosaicoSemantico filaMosaicoSem in DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID))
            {
                if (string.IsNullOrEmpty(filaMosaicoSem.Ontologia))
                {
                    if (filaMosaicoSem.Propiedad == "descripcion")
                    {
                        PresentacionMosaico.MostrarDescripcion = true;
                    }
                    else if (filaMosaicoSem.Propiedad == "publicador")
                    {
                        PresentacionMosaico.MostrarPublicador = true;
                    }
                    else if (filaMosaicoSem.Propiedad == "etiquetas")
                    {
                        PresentacionMosaico.MostrarEtiquetas = true;
                    }
                    else if (filaMosaicoSem.Propiedad == "categorias")
                    {
                        PresentacionMosaico.MostrarCategorias = true;
                    }
                }
                else
                {
                    ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad = new ObjetoConocimientoModel.PresentacionModel.PropiedadModel();

                    propiedad.Propiedad = filaMosaicoSem.Propiedad;
                    propiedad.Presentacion = filaMosaicoSem.Nombre;
                    propiedad.Orden = filaMosaicoSem.Orden;

                    PresentacionMosaico.ListaPropiedades.Add(propiedad);
                }
            }
            return PresentacionMosaico;
        }

        private ObjetoConocimientoModel.PresentacionPersonalizadoModel CargarPresentacionPersonalizado(Guid pOntologiaID)
        {
            ObjetoConocimientoModel.PresentacionPersonalizadoModel PresentacionPersonalizado = new ObjetoConocimientoModel.PresentacionPersonalizadoModel();

            PresentacionPersonalizado.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionPersonalizadoModel.PropiedadPersonalizadoModel>();

            foreach (AD.EntityModel.Models.ProyectoDS.PresentacionPersonalizadoSemantico filaPresentacionPersonalizado in DataWrapperProyecto.ListaPresentacionPersonalizadoSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID))
            {
                ObjetoConocimientoModel.PresentacionPersonalizadoModel.PropiedadPersonalizadoModel propiedad = new ObjetoConocimientoModel.PresentacionPersonalizadoModel.PropiedadPersonalizadoModel();
                propiedad.Identificador = filaPresentacionPersonalizado.ID;
                propiedad.Select = filaPresentacionPersonalizado.Select;
                propiedad.Where = filaPresentacionPersonalizado.Where;
                propiedad.Orden = filaPresentacionPersonalizado.Orden;
                PresentacionPersonalizado.ListaPropiedades.Add(propiedad);
            }
            return PresentacionPersonalizado;
        }

        private ObjetoConocimientoModel.PresentacionModel CargarPresentacionMapa(Guid pOntologiaID)
        {
            ObjetoConocimientoModel.PresentacionModel PresentacionMapa = new ObjetoConocimientoModel.PresentacionModel();

            PresentacionMapa.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionModel.PropiedadModel>();

            foreach (AD.EntityModel.Models.ProyectoDS.PresentacionMapaSemantico filaMapaSem in DataWrapperProyecto.ListaPresentacionMapaSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID))
            {
                if (string.IsNullOrEmpty(filaMapaSem.Ontologia))
                {
                    if (filaMapaSem.Propiedad == "descripcion")
                    {
                        PresentacionMapa.MostrarDescripcion = true;
                    }
                    else if (filaMapaSem.Propiedad == "publicador")
                    {
                        PresentacionMapa.MostrarPublicador = true;
                    }
                    else if (filaMapaSem.Propiedad == "etiquetas")
                    {
                        PresentacionMapa.MostrarEtiquetas = true;
                    }
                    else if (filaMapaSem.Propiedad == "categorias")
                    {
                        PresentacionMapa.MostrarCategorias = true;
                    }
                }
                else
                {
                    ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad = new ObjetoConocimientoModel.PresentacionModel.PropiedadModel();

                    propiedad.Propiedad = filaMapaSem.Propiedad;
                    propiedad.Presentacion = filaMapaSem.Nombre;
                    propiedad.Orden = filaMapaSem.Orden;

                    PresentacionMapa.ListaPropiedades.Add(propiedad);
                }
            }
            return PresentacionMapa;
        }

        private ObjetoConocimientoModel.PresentacionModel CargarPresentacionRecRelacionados(Guid pOntologiaID)
        {
            ObjetoConocimientoModel.PresentacionModel PresentacionRelacionados = new ObjetoConocimientoModel.PresentacionModel();

            PresentacionRelacionados.ListaPropiedades = new List<ObjetoConocimientoModel.PresentacionModel.PropiedadModel>();

            foreach (AD.EntityModel.Models.ProyectoDS.RecursosRelacionadosPresentacion filaRecRelacionado in DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Where(presentacion => presentacion.OntologiaID == pOntologiaID))
            {
                if (string.IsNullOrEmpty(filaRecRelacionado.Ontologia))
                {
                    if (filaRecRelacionado.Propiedad == "descripcion")
                    {
                        PresentacionRelacionados.MostrarDescripcion = true;
                    }
                    else if (filaRecRelacionado.Propiedad == "publicador")
                    {
                        PresentacionRelacionados.MostrarPublicador = true;
                    }
                    else if (filaRecRelacionado.Propiedad == "etiquetas")
                    {
                        PresentacionRelacionados.MostrarEtiquetas = true;
                    }
                    else if (filaRecRelacionado.Propiedad == "categorias")
                    {
                        PresentacionRelacionados.MostrarCategorias = true;
                    }
                }
                else
                {
                    ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad = new ObjetoConocimientoModel.PresentacionModel.PropiedadModel();

                    propiedad.Propiedad = filaRecRelacionado.Propiedad;
                    propiedad.Presentacion = filaRecRelacionado.Nombre;
                    propiedad.Orden = filaRecRelacionado.Orden;

                    PresentacionRelacionados.ListaPropiedades.Add(propiedad);
                }
            }
            return PresentacionRelacionados;
        }        

        #endregion

        #region Métodos de guardado

        public void GuardarObjetosConocimiento(List<ObjetoConocimientoModel> pListaObjetosConocimiento)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, null);
            Guid proyectoIDPatronOntologias = Guid.Empty;
            if (ParametroProyecto.ContainsKey("ProyectoIDPatronOntologias"))
            {
                Guid.TryParse(ParametroProyecto["ProyectoIDPatronOntologias"], out proyectoIDPatronOntologias);
                documentacionCN.ObtenerOntologiasProyecto(proyectoIDPatronOntologias, dataWrapperDocumentacion, false, false, true);
            }
            documentacionCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dataWrapperDocumentacion, false, false, true);
            documentacionCN.Dispose();

            List<AD.EntityModel.Models.Documentacion.Documento> filasDoc = dataWrapperDocumentacion.ListaDocumento.Where(documento=> documento.Tipo.Equals((short)TiposDocumentacion.Ontologia)).ToList();

            Dictionary<string, Guid> listaObjetosConocimiento = new Dictionary<string, Guid>();
            List<Guid> listaObjetosConocimientoNuevos = new List<Guid>();

            //Añadir los nuevos
            foreach (ObjetoConocimientoModel objetoConocimiento in pListaObjetosConocimiento)
            {
                AD.EntityModel.Models.Documentacion.Documento filaOC = filasDoc.FirstOrDefault(doc => doc.Enlace.ToLower().Equals(objetoConocimiento.Ontologia.ToLower() + ".owl"));

                if (filaOC != null)
                {
                    //objetoConocimiento.Ontologia = filasOC[0].Enlace.Replace(".owl", "");

                    Guid ontologiaID = filaOC.DocumentoID;
                    if (!listaObjetosConocimiento.ContainsKey(objetoConocimiento.Ontologia))
                    {
                        listaObjetosConocimiento.Add(objetoConocimiento.Ontologia, ontologiaID);
                    }
                    else
                    {
                        string mensaje = $"Hay Objetos de conocimiento reptidos. La ontologia Repetida es: {objetoConocimiento.Ontologia}";
                        throw new ExcepcionGeneral(mensaje);
                    }

                    if (!objetoConocimiento.Deleted)
                    {
                        //.FindByOrganizacionIDProyectoIDOntologiaProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, objetoConocimiento.Ontologia);
                        OntologiaProyecto filaOntologia = DataWrapperProyecto.ListaOntologiaProyecto.FirstOrDefault(onto=>onto.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && onto.ProyectoID.Equals(ProyectoSeleccionado.Clave) && onto.OntologiaProyecto1.Equals(objetoConocimiento.Ontologia));
                        if (filaOntologia == null)
                        {
                            listaObjetosConocimientoNuevos.Add(ontologiaID);

                            AgregarObjetoConocimientoNuevo(ontologiaID, objetoConocimiento);
                        }
                    }
                }
            }

            //Modificar los que tienen cambios
            foreach (ObjetoConocimientoModel objetoConocimiento in pListaObjetosConocimiento)
            {
                if (listaObjetosConocimiento.ContainsKey(objetoConocimiento.Ontologia))
                {
                    Guid ontologiaID = listaObjetosConocimiento[objetoConocimiento.Ontologia];

                    if (!objetoConocimiento.Deleted && !listaObjetosConocimientoNuevos.Contains(ontologiaID))
                    {
                        OntologiaProyecto filaOntologia = DataWrapperProyecto.ListaOntologiaProyecto.FirstOrDefault(onto => onto.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && onto.ProyectoID.Equals(ProyectoSeleccionado.Clave) && onto.OntologiaProyecto1.Equals(objetoConocimiento.Ontologia));

                        GuardarDatosObjetoConocimiento(ontologiaID, filaOntologia, objetoConocimiento);
                    }
                }
            }

            //Eliminar los eliminados
            foreach (ObjetoConocimientoModel objetoConocimiento in pListaObjetosConocimiento)
            {
                if (listaObjetosConocimiento.ContainsKey(objetoConocimiento.Ontologia))
                {
                    Guid ontologiaID = listaObjetosConocimiento[objetoConocimiento.Ontologia];

                    if (objetoConocimiento.Deleted && !listaObjetosConocimientoNuevos.Contains(ontologiaID))
                    {
                        OntologiaProyecto filaOntologia = DataWrapperProyecto.ListaOntologiaProyecto.FirstOrDefault(onto => onto.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && onto.ProyectoID.Equals(ProyectoSeleccionado.Clave) && onto.OntologiaProyecto1.Equals(objetoConocimiento.Ontologia));
                        if (filaOntologia != null)
                        {
                            EliminarObjetoConocimiento(ontologiaID, filaOntologia);
                        }
                    }
                }
            }

            List<OntologiaProyecto> filasNoEliminadas = DataWrapperProyecto.ListaOntologiaProyecto.Where(fila => mEntityContext.Entry(fila).State != EntityState.Deleted).ToList();

            //Eliminar los que no se encuentran
            foreach (OntologiaProyecto filaOntologia in filasNoEliminadas)
            {
                if (!pListaObjetosConocimiento.Any(objetoConocimiento => objetoConocimiento.Ontologia.Equals(filaOntologia.OntologiaProyecto1)))
                {
                    AD.EntityModel.Models.Documentacion.Documento filaDocOntologia = filasDoc.FirstOrDefault(doc => doc.Enlace.ToLower().Equals(filaOntologia.OntologiaProyecto1.ToLower() + ".owl"));

                    if (filaDocOntologia != null)
                    {
                        EliminarObjetoConocimiento(filaDocOntologia.DocumentoID, filaOntologia);
                    }
                }
            }

            using (ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null))
            {
                proyCN.ActualizarProyectos();
            }
        }

        private void AgregarObjetoConocimientoNuevo(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimiento)
        {
            OntologiaProyecto filaOntologiaNueva = new OntologiaProyecto();
            filaOntologiaNueva.ProyectoID = ProyectoSeleccionado.Clave;
            filaOntologiaNueva.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            filaOntologiaNueva.OntologiaProyecto1 = pObjetoConocimiento.Ontologia;

            GuardarDatosObjetoConocimiento(pOntologiaID, filaOntologiaNueva, pObjetoConocimiento);
            
            if (mEntityContext.OntologiaProyecto.FirstOrDefault(onto=>onto.OrganizacionID.Equals(filaOntologiaNueva.OrganizacionID) && onto.ProyectoID.Equals(filaOntologiaNueva.ProyectoID) && onto.OntologiaProyecto1.Equals(filaOntologiaNueva.OntologiaProyecto1))==null)
            {
                DataWrapperProyecto.ListaOntologiaProyecto.Add(filaOntologiaNueva);
                mEntityContext.OntologiaProyecto.Add(filaOntologiaNueva);
            }

            AD.EntityModel.Models.ProyectoDS.TipoOntoDispRolUsuarioProy tolUsuario = new AD.EntityModel.Models.ProyectoDS.TipoOntoDispRolUsuarioProy();
            tolUsuario.OntologiaID = pOntologiaID;
            tolUsuario.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            tolUsuario.ProyectoID = ProyectoSeleccionado.Clave;
            tolUsuario.RolUsuario = (short)UserRol.Administrator;
            if (!mEntityContext.TipoOntoDispRolUsuarioProy.Any(tipo => tipo.OrganizacionID.Equals(tolUsuario.OrganizacionID) && tipo.ProyectoID.Equals(tolUsuario.ProyectoID) && tipo.OntologiaID.Equals(tolUsuario.OntologiaID)))
            {
                DataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy.Add(tolUsuario);
                mEntityContext.TipoOntoDispRolUsuarioProy.Add(tolUsuario);
            }
            
        }

        public void GuardarObjetoConocimiento(ObjetoConocimientoModel pObjetoConocimiento)
        {
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, null);

            Guid proyectoIDPatronOntologias = Guid.Empty;
            if (ParametroProyecto.ContainsKey("ProyectoIDPatronOntologias"))
            {
                Guid.TryParse(ParametroProyecto["ProyectoIDPatronOntologias"], out proyectoIDPatronOntologias);
                documentacionCN.ObtenerOntologiasProyecto(proyectoIDPatronOntologias, dataWrapperDocumentacion, false, false, true);
            }

            documentacionCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dataWrapperDocumentacion, false, false, true);
            documentacionCN.Dispose();


            List<AD.EntityModel.Models.Documentacion.Documento> filasDoc = dataWrapperDocumentacion.ListaDocumento.Where(documento => documento.Tipo.Equals((short)TiposDocumentacion.Ontologia)).ToList();

            Dictionary<string, Guid> listaObjetosConocimiento = new Dictionary<string, Guid>();
            List<Guid> listaObjetosConocimientoNuevos = new List<Guid>();

            //Añadir los nuevos

            AD.EntityModel.Models.Documentacion.Documento filaOC = filasDoc.FirstOrDefault(doc => doc.Enlace.ToLower().Equals(pObjetoConocimiento.Ontologia.ToLower() + ".owl"));

            if (filaOC != null)
            {
                //objetoConocimiento.Ontologia = filasOC[0].Enlace.Replace(".owl", "");

                Guid ontologiaID = filaOC.DocumentoID;
                if (!listaObjetosConocimiento.ContainsKey(pObjetoConocimiento.Ontologia))
                {
                    listaObjetosConocimiento.Add(pObjetoConocimiento.Ontologia, ontologiaID);
                }
                else
                {
                    string mensaje = $"Hay Objetos de conocimiento reptidos. La ontologia Repetida es: {pObjetoConocimiento.Ontologia}";
                    throw new ExcepcionGeneral(mensaje);
                }

                if (!pObjetoConocimiento.Deleted)
                {
                    //.FindByOrganizacionIDProyectoIDOntologiaProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, objetoConocimiento.Ontologia);
                    OntologiaProyecto filaOntologia = DataWrapperProyecto.ListaOntologiaProyecto.FirstOrDefault(onto => onto.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && onto.ProyectoID.Equals(ProyectoSeleccionado.Clave) && onto.OntologiaProyecto1.Equals(pObjetoConocimiento.Ontologia));
                    if (filaOntologia == null)
                    {
                        listaObjetosConocimientoNuevos.Add(ontologiaID);

                        AgregarObjetoConocimientoNuevo(ontologiaID, pObjetoConocimiento);
                    }
                }
            }


            //Modificar los que tienen cambios


            if (listaObjetosConocimiento.ContainsKey(pObjetoConocimiento.Ontologia))
            {
                Guid ontologiaID = listaObjetosConocimiento[pObjetoConocimiento.Ontologia];

                if (!pObjetoConocimiento.Deleted && !listaObjetosConocimientoNuevos.Contains(ontologiaID))
                {
                    OntologiaProyecto filaOntologia = DataWrapperProyecto.ListaOntologiaProyecto.FirstOrDefault(onto => onto.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && onto.ProyectoID.Equals(ProyectoSeleccionado.Clave) && onto.OntologiaProyecto1.Equals(pObjetoConocimiento.Ontologia));

                    GuardarDatosObjetoConocimiento(ontologiaID, filaOntologia, pObjetoConocimiento);
                }
            }

            //Eliminar los eliminados

            if (listaObjetosConocimiento.ContainsKey(pObjetoConocimiento.Ontologia))
            {
                Guid ontologiaID = listaObjetosConocimiento[pObjetoConocimiento.Ontologia];

                if (pObjetoConocimiento.Deleted && !listaObjetosConocimientoNuevos.Contains(ontologiaID))
                {
                    OntologiaProyecto filaOntologia = DataWrapperProyecto.ListaOntologiaProyecto.FirstOrDefault(onto => onto.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && onto.ProyectoID.Equals(ProyectoSeleccionado.Clave) && onto.OntologiaProyecto1.Equals(pObjetoConocimiento.Ontologia));
                    if (filaOntologia != null)
                    {
                        EliminarObjetoConocimiento(ontologiaID, filaOntologia);
                    }
                }
            }

            using (ProyectoCN proyCN = new ProyectoCN(mEntityContext,mLoggingService, mConfigService, null))
            {
                proyCN.ActualizarProyectos();
            }
        }

        private void GuardarDatosObjetoConocimiento(Guid pOntologiaID, OntologiaProyecto pFilaOntologia, ObjetoConocimientoModel pObjetoConocimiento)
        {
            pFilaOntologia.NombreOnt = pObjetoConocimiento.Name;
            pFilaOntologia.NombreCortoOnt = string.IsNullOrEmpty(pObjetoConocimiento.ShortNameOntology) ? "" : pObjetoConocimiento.ShortNameOntology;
            pFilaOntologia.Namespace = pObjetoConocimiento.Namespace;
            pFilaOntologia.NamespacesExtra = string.IsNullOrEmpty(pObjetoConocimiento.NamespaceExtra) ? "" : pObjetoConocimiento.NamespaceExtra;

            string subtipos = "";
            if (pObjetoConocimiento.Subtipos != null)
            {
                foreach (string subTipo in pObjetoConocimiento.Subtipos.Keys)
                {
                    subtipos += subTipo + "|||" + pObjetoConocimiento.Subtipos[subTipo] + "[|||]";
                }
            }
            pFilaOntologia.SubTipos = subtipos;
            
            pFilaOntologia.CachearDatosSemanticos = pObjetoConocimiento.CachearDatosSemanticos;
            pFilaOntologia.EsBuscable = pObjetoConocimiento.EsBuscable;

            GuardarDatosPresentacionListado(pOntologiaID, pObjetoConocimiento);
            GuardarDatosPresentacionMosaico(pOntologiaID, pObjetoConocimiento);
            GuardarDatosPresentacionMapa(pOntologiaID, pObjetoConocimiento);
            GuardarDatosPresentacionPersonalizado(pOntologiaID, pObjetoConocimiento);
            GuardarDatosPresentacionRelacionado(pOntologiaID, pObjetoConocimiento);
        }

        private void GuardarDatosPresentacionListado(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimiento)
        {
            AgregarPresentacionListadoGenerico(pOntologiaID, "descripcion", 10001, pObjetoConocimiento.PresentacionListado.MostrarDescripcion);
            AgregarPresentacionListadoGenerico(pOntologiaID, "publicador", 10002, pObjetoConocimiento.PresentacionListado.MostrarPublicador);
            AgregarPresentacionListadoGenerico(pOntologiaID, "etiquetas", 10003, pObjetoConocimiento.PresentacionListado.MostrarEtiquetas);
            AgregarPresentacionListadoGenerico(pOntologiaID, "categorias", 10004, pObjetoConocimiento.PresentacionListado.MostrarCategorias);
            
            List<AD.EntityModel.Models.ProyectoDS.PresentacionListadoSemantico> listaRecorrer = DataWrapperProyecto.ListaPresentacionListadoSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID && presentacion.Ontologia != "").ToList();
            foreach (AD.EntityModel.Models.ProyectoDS.PresentacionListadoSemantico filaListadoSem in listaRecorrer)
            {
                if (pObjetoConocimiento.PresentacionListado.ListaPropiedades==null || !pObjetoConocimiento.PresentacionListado.ListaPropiedades.Any(item => item.Orden == filaListadoSem.Orden) && mEntityContext.Entry(filaListadoSem).State != EntityState.Deleted)
                {
                    mEntityContext.Entry(filaListadoSem).State = EntityState.Deleted;
                    DataWrapperProyecto.ListaPresentacionListadoSemantico.Remove(filaListadoSem);
                }

            }

            if (pObjetoConocimiento.PresentacionListado.ListaPropiedades != null)
            {
                foreach (ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad in pObjetoConocimiento.PresentacionListado.ListaPropiedades)
                {
                    if (string.IsNullOrEmpty(propiedad.Presentacion))
                    {
                        propiedad.Presentacion = "";
                    }

                    AgregarPresentacionListado(pOntologiaID, pObjetoConocimiento.Ontologia, propiedad.Orden, propiedad.Propiedad, propiedad.Presentacion);
                }
            }
        }

        private void AgregarPresentacionListadoGenerico(Guid pOntologiaID, string pPropiedad, short pOrden, bool pMostrarPropiedad)
        {
            AD.EntityModel.Models.ProyectoDS.PresentacionListadoSemantico filasListadoSem = DataWrapperProyecto.ListaPresentacionListadoSemantico.FirstOrDefault(presentacion => presentacion.OntologiaID == pOntologiaID && presentacion.Propiedad == pPropiedad);

            //Guardar la presentacion
            if (pMostrarPropiedad && filasListadoSem == null )
            {
                AgregarPresentacionListado(pOntologiaID, "", pOrden, pPropiedad, "");
            }
            else if (!pMostrarPropiedad && filasListadoSem != null && mEntityContext.Entry(filasListadoSem).State != EntityState.Deleted)
            {
                mEntityContext.EliminarElemento(filasListadoSem);
                DataWrapperProyecto.ListaPresentacionListadoSemantico.Remove(filasListadoSem);
            }
            else if (filasListadoSem != null)
            {
                if (filasListadoSem.Orden != pOrden)
                {
                    mEntityContext.EliminarElemento(filasListadoSem);
                    DataWrapperProyecto.ListaPresentacionListadoSemantico.Remove(filasListadoSem);
                    AgregarPresentacionListado(pOntologiaID, "", pOrden, pPropiedad, "");
                }
            }

            //if (filasListadoSem == null)
            //{
            //    if (EntityContext.Entry(filasListadoSem).State != EntityState.Deleted)
            //    {
            //        //Guardar la presentacion
            //        if (pMostrarPropiedad && filasListadoSem == null)
            //        {
            //            AgregarPresentacionListado(pOntologiaID, "", pOrden, pPropiedad, "");
            //        }
            //        else if (!pMostrarPropiedad && filasListadoSem != null)
            //        {
            //            EntityContext.Entry(filasListadoSem).State = EntityState.Deleted;
            //            DataWrapperProyecto.ListaPresentacionListadoSemantico.Remove(filasListadoSem);
            //        }
            //        else if (filasListadoSem != null)
            //        {
            //            filasListadoSem.Orden = pOrden;
            //        }
            //    }
            //}
        }

        private void AgregarPresentacionListado(Guid pOntologiaID, string pNombreOnto, short pOrden, string pPropiedad, string pPresentacion)
        {
            AD.EntityModel.Models.ProyectoDS.PresentacionListadoSemantico presentacionListadoBD = mEntityContext.PresentacionListadoSemantico.Where(presentacion => presentacion.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && presentacion.ProyectoID.Equals(ProyectoSeleccionado.Clave) && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Orden.Equals(pOrden)).FirstOrDefault();

            if (presentacionListadoBD == null)
            {
                presentacionListadoBD = new AD.EntityModel.Models.ProyectoDS.PresentacionListadoSemantico();
                

                presentacionListadoBD.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                presentacionListadoBD.ProyectoID = ProyectoSeleccionado.Clave;
                presentacionListadoBD.OntologiaID = pOntologiaID;
                presentacionListadoBD.Orden = pOrden;
                DataWrapperProyecto.ListaPresentacionListadoSemantico.Add(presentacionListadoBD);
                mEntityContext.PresentacionListadoSemantico.Add(presentacionListadoBD);
            }
           
            if (!string.IsNullOrEmpty(pNombreOnto))
            {
                presentacionListadoBD.Ontologia = "http://gnoss.com/Ontologia" + pNombreOnto + ".owl#";
            }
            else
            {
                presentacionListadoBD.Ontologia = "";
            }
            presentacionListadoBD.Propiedad = pPropiedad;
            presentacionListadoBD.Nombre = HttpUtility.UrlDecode(pPresentacion);

            //if (presentacionListadoBD == null)
            //{
                
            //}
            //else if (EntityContext.Entry(presentacionListadoBD).State.Equals(EntityState.Deleted))
            //{
            //    EntityContext.PresentacionListadoSemantico.Add(filaPresentacionListado);
            //}
            }

        private void GuardarDatosPresentacionMosaico(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimiento)
        {
            AgregarPresentacionMosaicoGenerico(pOntologiaID, "descripcion", 10001, pObjetoConocimiento.PresentacionMosaico.MostrarDescripcion);
            AgregarPresentacionMosaicoGenerico(pOntologiaID, "publicador", 10002, pObjetoConocimiento.PresentacionMosaico.MostrarPublicador);
            AgregarPresentacionMosaicoGenerico(pOntologiaID, "etiquetas", 10003, pObjetoConocimiento.PresentacionMosaico.MostrarEtiquetas);
            AgregarPresentacionMosaicoGenerico(pOntologiaID, "categorias", 10004, pObjetoConocimiento.PresentacionMosaico.MostrarCategorias);

            List<AD.EntityModel.Models.ProyectoDS.PresentacionMosaicoSemantico> listaPresentacoin = DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID && presentacion.Ontologia != "").ToList();
            foreach (AD.EntityModel.Models.ProyectoDS.PresentacionMosaicoSemantico filaMosaicoSem in listaPresentacoin)
            {
                if (pObjetoConocimiento.PresentacionMosaico.ListaPropiedades == null || !pObjetoConocimiento.PresentacionMosaico.ListaPropiedades.Any(item => item.Orden == filaMosaicoSem.Orden) && mEntityContext.Entry(filaMosaicoSem).State != EntityState.Deleted)
                {
                    mEntityContext.Entry(filaMosaicoSem).State = EntityState.Deleted;
                    DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Remove(filaMosaicoSem);
                }
            }

            if (pObjetoConocimiento.PresentacionMosaico.ListaPropiedades != null)
            {
                foreach (ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad in pObjetoConocimiento.PresentacionMosaico.ListaPropiedades)
                {
                    if (string.IsNullOrEmpty(propiedad.Presentacion))
                    {
                        propiedad.Presentacion = "";
                    }

                    AgregarPresentacionMosaico(pOntologiaID, pObjetoConocimiento.Ontologia, propiedad.Orden, propiedad.Propiedad, propiedad.Presentacion);
                }
            }
        }

        private void AgregarPresentacionMosaicoGenerico(Guid pOntologiaID, string pPropiedad, short pOrden, bool pMostrarPropiedad)
        {
            AD.EntityModel.Models.ProyectoDS.PresentacionMosaicoSemantico filasMosaicoSem = DataWrapperProyecto.ListaPresentacionMosaicoSemantico.FirstOrDefault(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID == pOntologiaID && presentacion.Propiedad == pPropiedad);

            if (pMostrarPropiedad && filasMosaicoSem == null)
            {
                AgregarPresentacionMosaico(pOntologiaID, "", pOrden, pPropiedad, "");
            }
            else if (!pMostrarPropiedad && filasMosaicoSem != null)
            {
                mEntityContext.EliminarElemento(filasMosaicoSem);
                DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Remove(filasMosaicoSem);
            }
            else if (filasMosaicoSem != null)
            {
                filasMosaicoSem.Orden = pOrden;
            }
        }

        private void AgregarPresentacionMosaico(Guid pOntologiaID, string pNombreOnto, short pOrden, string pPropiedad, string pPresentacion)
        {
            AD.EntityModel.Models.ProyectoDS.PresentacionMosaicoSemantico filaPresentacionMosaico = mEntityContext.PresentacionMosaicoSemantico.FirstOrDefault(presentacion => presentacion.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && presentacion.ProyectoID.Equals(ProyectoSeleccionado.Clave) && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Orden.Equals(pOrden));

            if (filaPresentacionMosaico == null)
            {
                filaPresentacionMosaico = new AD.EntityModel.Models.ProyectoDS.PresentacionMosaicoSemantico();
                DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Add(filaPresentacionMosaico);
                mEntityContext.PresentacionMosaicoSemantico.Add(filaPresentacionMosaico);

            filaPresentacionMosaico.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            filaPresentacionMosaico.ProyectoID = ProyectoSeleccionado.Clave;
            filaPresentacionMosaico.OntologiaID = pOntologiaID;
            filaPresentacionMosaico.Orden = pOrden;
            }

            
            if (!string.IsNullOrEmpty(pNombreOnto))
            {
                filaPresentacionMosaico.Ontologia = "http://gnoss.com/Ontologia" + pNombreOnto + ".owl#";
            }
            else
            {
                filaPresentacionMosaico.Ontologia = "";
            }
            filaPresentacionMosaico.Propiedad = pPropiedad;
            filaPresentacionMosaico.Nombre = HttpUtility.UrlDecode(pPresentacion);

            //DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Add(filaPresentacionMosaico);
            //if (EntityContext.PresentacionMosaicoSemantico.FirstOrDefault(presentacion => presentacion.OrganizacionID.Equals(filaPresentacionMosaico.OrganizacionID) && presentacion.ProyectoID.Equals(filaPresentacionMosaico.ProyectoID) && presentacion.OntologiaID.Equals(filaPresentacionMosaico.OntologiaID) && presentacion.Orden.Equals(filaPresentacionMosaico.Orden)) == null)
            //{
            //    EntityContext.PresentacionMosaicoSemantico.Add(filaPresentacionMosaico);
            //}
            
        }

        private void GuardarDatosPresentacionMapa(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimiento)
        {
            AgregarPresentacionMapaGenerico(pOntologiaID, "descripcion", 10001, pObjetoConocimiento.PresentacionMapa.MostrarDescripcion);
            AgregarPresentacionMapaGenerico(pOntologiaID, "publicador", 10002, pObjetoConocimiento.PresentacionMapa.MostrarPublicador);
            AgregarPresentacionMapaGenerico(pOntologiaID, "etiquetas", 10003, pObjetoConocimiento.PresentacionMapa.MostrarEtiquetas);
            AgregarPresentacionMapaGenerico(pOntologiaID, "categorias", 10004, pObjetoConocimiento.PresentacionMapa.MostrarCategorias);
            

            List<AD.EntityModel.Models.ProyectoDS.PresentacionMapaSemantico> listaRecorrer = DataWrapperProyecto.ListaPresentacionMapaSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID && presentacion.Ontologia != "").ToList();
            foreach (AD.EntityModel.Models.ProyectoDS.PresentacionMapaSemantico filaMapaSem in listaRecorrer)
            {
                if (pObjetoConocimiento.PresentacionMapa.ListaPropiedades == null || !pObjetoConocimiento.PresentacionMapa.ListaPropiedades.Any(item => item.Orden == filaMapaSem.Orden) && mEntityContext.Entry(filaMapaSem).State != EntityState.Deleted)
                {
                    mEntityContext.Entry(filaMapaSem).State = EntityState.Deleted;
                    DataWrapperProyecto.ListaPresentacionMapaSemantico.Remove(filaMapaSem);
                }
            }

            if (pObjetoConocimiento.PresentacionMapa.ListaPropiedades != null)
            {
                foreach (ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad in pObjetoConocimiento.PresentacionMapa.ListaPropiedades)
                {
                    if (string.IsNullOrEmpty(propiedad.Presentacion))
                    {
                        propiedad.Presentacion = "";
                    }

                    AgregarPresentacionMapa(pOntologiaID, pObjetoConocimiento.Ontologia, propiedad.Orden, propiedad.Propiedad, propiedad.Presentacion);
                }
            }
        }

        private void GuardarDatosPresentacionPersonalizado(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimiento)
        {
            List<AD.EntityModel.Models.ProyectoDS.PresentacionPersonalizadoSemantico> listaRecorrer = DataWrapperProyecto.ListaPresentacionPersonalizadoSemantico.Where(presentacion => presentacion.OntologiaID == pOntologiaID && presentacion.Ontologia != "").ToList();
            
            foreach (AD.EntityModel.Models.ProyectoDS.PresentacionPersonalizadoSemantico filaPersonalizadoSem in listaRecorrer)
            {
                if (pObjetoConocimiento.PresentacionPersonalizado == null || !pObjetoConocimiento.PresentacionPersonalizado.ListaPropiedades.Any(item => item.Orden == filaPersonalizadoSem.Orden) && mEntityContext.Entry(filaPersonalizadoSem).State != EntityState.Deleted)
                {
                    mEntityContext.Entry(filaPersonalizadoSem).State = EntityState.Deleted;
                    DataWrapperProyecto.ListaPresentacionPersonalizadoSemantico.Remove(filaPersonalizadoSem);
                }
            }

            if (pObjetoConocimiento.PresentacionPersonalizado!=null && pObjetoConocimiento.PresentacionPersonalizado.ListaPropiedades != null)
            {
                foreach (ObjetoConocimientoModel.PresentacionPersonalizadoModel.PropiedadPersonalizadoModel propiedad in pObjetoConocimiento.PresentacionPersonalizado.ListaPropiedades)
                {
                    AgregarPresentacionPersonalizado(pOntologiaID, pObjetoConocimiento.Ontologia, propiedad.Orden, propiedad.Identificador, propiedad.Select, propiedad.Where);
                }
            }
        }

        private void AgregarPresentacionMapaGenerico(Guid pOntologiaID, string pPropiedad, short pOrden, bool pMostrarPropiedad)
        {
            AD.EntityModel.Models.ProyectoDS.PresentacionMapaSemantico filasMapaSem = DataWrapperProyecto.ListaPresentacionMapaSemantico.FirstOrDefault(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID == pOntologiaID && presentacion.Propiedad == pPropiedad);

            //Guardar la presentacion
            if (pMostrarPropiedad && filasMapaSem == null)
            {
                AgregarPresentacionMapa(pOntologiaID, "", pOrden, pPropiedad, "");
            }
            else if (!pMostrarPropiedad && filasMapaSem != null)
            {
                mEntityContext.EliminarElemento(filasMapaSem);
                DataWrapperProyecto.ListaPresentacionMapaSemantico.Remove(filasMapaSem);
            }
            else if (filasMapaSem != null)
            {
                filasMapaSem.Orden = pOrden;
            }
        }

        private void AgregarPresentacionMapa(Guid pOntologiaID, string pNombreOnto, short pOrden, string pPropiedad, string pPresentacion)
        {
            AD.EntityModel.Models.ProyectoDS.PresentacionMapaSemantico filaPresentacionMapa = mEntityContext.PresentacionMapaSemantico.FirstOrDefault(presentacion => presentacion.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && presentacion.ProyectoID.Equals(ProyectoSeleccionado.Clave) && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Orden.Equals(pOrden));

            if(filaPresentacionMapa == null)
            {
                filaPresentacionMapa = new AD.EntityModel.Models.ProyectoDS.PresentacionMapaSemantico();
                DataWrapperProyecto.ListaPresentacionMapaSemantico.Add(filaPresentacionMapa);
                mEntityContext.PresentacionMapaSemantico.Add(filaPresentacionMapa);

            filaPresentacionMapa.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            filaPresentacionMapa.ProyectoID = ProyectoSeleccionado.Clave;
            filaPresentacionMapa.OntologiaID = pOntologiaID;
            filaPresentacionMapa.Orden = pOrden;
            }
            
            if (!string.IsNullOrEmpty(pNombreOnto))
            {
                filaPresentacionMapa.Ontologia = "http://gnoss.com/Ontologia" + pNombreOnto + ".owl#";
            }
            else
            {
                filaPresentacionMapa.Ontologia = "";
            }
            filaPresentacionMapa.Propiedad = pPropiedad;
            filaPresentacionMapa.Nombre = HttpUtility.UrlDecode(pPresentacion);

            //DataWrapperProyecto.ListaPresentacionMapaSemantico.Add(filaPresentacionMapa);
            //if (EntityContext.PresentacionMapaSemantico.FirstOrDefault(presentacion => presentacion.OrganizacionID.Equals(filaPresentacionMapa.OrganizacionID) && presentacion.ProyectoID.Equals(filaPresentacionMapa.ProyectoID) && presentacion.OntologiaID.Equals(filaPresentacionMapa.OntologiaID) && presentacion.Orden.Equals(filaPresentacionMapa.Orden)) == null)
            //{
            //    EntityContext.PresentacionMapaSemantico.Add(filaPresentacionMapa);
            //}
            }

        private void AgregarPresentacionPersonalizado(Guid pOntologiaID, string pNombreOnto, short pOrden, string pIdentificador, string pSelect, string pWhere)
        {
            AD.EntityModel.Models.ProyectoDS.PresentacionPersonalizadoSemantico filaPresentacionPersonalizado = mEntityContext.PresentacionPersonalizadoSemantico.FirstOrDefault(presentacion => presentacion.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && presentacion.ProyectoID.Equals(ProyectoSeleccionado.Clave) && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Orden.Equals(pOrden));

            if (filaPresentacionPersonalizado == null)
            {
                filaPresentacionPersonalizado = new AD.EntityModel.Models.ProyectoDS.PresentacionPersonalizadoSemantico();
                DataWrapperProyecto.ListaPresentacionPersonalizadoSemantico.Add(filaPresentacionPersonalizado);
                mEntityContext.PresentacionPersonalizadoSemantico.Add(filaPresentacionPersonalizado);
                filaPresentacionPersonalizado.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaPresentacionPersonalizado.ProyectoID = ProyectoSeleccionado.Clave;
                filaPresentacionPersonalizado.OntologiaID = pOntologiaID;
                filaPresentacionPersonalizado.Orden = pOrden;
            }

            if (!string.IsNullOrEmpty(pNombreOnto))
            {
                filaPresentacionPersonalizado.Ontologia = "http://gnoss.com/Ontologia" + pNombreOnto + ".owl#";
            }
            else
            {
                filaPresentacionPersonalizado.Ontologia = "";
            }
            filaPresentacionPersonalizado.ID = HttpUtility.UrlDecode(pIdentificador);
            filaPresentacionPersonalizado.Select = HttpUtility.UrlDecode(pSelect);
            filaPresentacionPersonalizado.Where = HttpUtility.UrlDecode(pWhere);
        }

        private void GuardarDatosPresentacionRelacionado(Guid pOntologiaID, ObjetoConocimientoModel pObjetoConocimiento)
        {
            AgregarPresentacionRelacionadoGenerico(pOntologiaID, "descripcion", 10001, pObjetoConocimiento.PresentacionRelacionados.MostrarDescripcion);
            AgregarPresentacionRelacionadoGenerico(pOntologiaID, "publicador", 10002, pObjetoConocimiento.PresentacionRelacionados.MostrarPublicador);
            AgregarPresentacionRelacionadoGenerico(pOntologiaID, "etiquetas", 10003, pObjetoConocimiento.PresentacionRelacionados.MostrarEtiquetas);
            AgregarPresentacionRelacionadoGenerico(pOntologiaID, "categorias", 10004, pObjetoConocimiento.PresentacionRelacionados.MostrarCategorias);

            List<AD.EntityModel.Models.ProyectoDS.RecursosRelacionadosPresentacion> listaRecorrer = DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Where(presentacion => presentacion.OntologiaID == pOntologiaID && presentacion.Ontologia != "").ToList();
            
            foreach (AD.EntityModel.Models.ProyectoDS.RecursosRelacionadosPresentacion filaRelacionadoSem in listaRecorrer)
            {
                if (pObjetoConocimiento.PresentacionRelacionados.ListaPropiedades == null || !pObjetoConocimiento.PresentacionRelacionados.ListaPropiedades.Any(item => item.Orden == filaRelacionadoSem.Orden) && mEntityContext.Entry(filaRelacionadoSem).State != EntityState.Deleted)
                {
                    mEntityContext.Entry(filaRelacionadoSem).State = EntityState.Deleted;
                    DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Remove(filaRelacionadoSem);
                }
            }

            if (pObjetoConocimiento.PresentacionRelacionados.ListaPropiedades != null)
            {
                foreach (ObjetoConocimientoModel.PresentacionModel.PropiedadModel propiedad in pObjetoConocimiento.PresentacionRelacionados.ListaPropiedades)
                {
                    if (string.IsNullOrEmpty(propiedad.Presentacion))
                    {
                        propiedad.Presentacion = "";
                    }

                    AgregarPresentacionRelacionado(pOntologiaID, pObjetoConocimiento.Ontologia, propiedad.Orden, propiedad.Propiedad, propiedad.Presentacion);
                }
            }
        }

        private void AgregarPresentacionRelacionadoGenerico(Guid pOntologiaID, string pPropiedad, short pOrden, bool pMostrarPropiedad)
        {
            AD.EntityModel.Models.ProyectoDS.RecursosRelacionadosPresentacion filasRelacionadoSem = DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.FirstOrDefault(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID == pOntologiaID && presentacion.Propiedad == pPropiedad);
            
            //Guardar la presentacion
            if (pMostrarPropiedad && filasRelacionadoSem == null)
            {
                AgregarPresentacionRelacionado(pOntologiaID, "", pOrden, pPropiedad, "");
            }
            else if (!pMostrarPropiedad && filasRelacionadoSem != null)
            {
                mEntityContext.EliminarElemento(filasRelacionadoSem);
                DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Remove(filasRelacionadoSem);
            }
            else if (filasRelacionadoSem != null)
            {
                filasRelacionadoSem.Orden = pOrden;
            }
        }

        private void AgregarPresentacionRelacionado(Guid pOntologiaID, string pNombreOnto, short pOrden, string pPropiedad, string pPresentacion)
        {
            AD.EntityModel.Models.ProyectoDS.RecursosRelacionadosPresentacion filaPresentacionRelacionado = mEntityContext.RecursosRelacionadosPresentacion.FirstOrDefault(presentacion => presentacion.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && presentacion.ProyectoID.Equals(ProyectoSeleccionado.Clave) && presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Orden.Equals(pOrden));

            if (filaPresentacionRelacionado == null)
            {
                filaPresentacionRelacionado = new AD.EntityModel.Models.ProyectoDS.RecursosRelacionadosPresentacion();
                DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Add(filaPresentacionRelacionado);
                mEntityContext.RecursosRelacionadosPresentacion.Add(filaPresentacionRelacionado);

            filaPresentacionRelacionado.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            filaPresentacionRelacionado.ProyectoID = ProyectoSeleccionado.Clave;
            filaPresentacionRelacionado.OntologiaID = pOntologiaID;
            filaPresentacionRelacionado.Orden = pOrden;
            }

            filaPresentacionRelacionado.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            filaPresentacionRelacionado.ProyectoID = ProyectoSeleccionado.Clave;
            filaPresentacionRelacionado.OntologiaID = pOntologiaID;
            filaPresentacionRelacionado.Orden = pOrden;
            if (!string.IsNullOrEmpty(pNombreOnto))
            {
                filaPresentacionRelacionado.Ontologia = "http://gnoss.com/Ontologia" + pNombreOnto + ".owl#";
            }
            else
            {
                filaPresentacionRelacionado.Ontologia = "";
            }
            filaPresentacionRelacionado.Propiedad = pPropiedad;
            filaPresentacionRelacionado.Nombre = HttpUtility.UrlDecode(pPresentacion);

            //DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Add(filaPresentacionRelacionado);
            //if (EntityContext.RecursosRelacionadosPresentacion.FirstOrDefault(recursoRel=>recursoRel.OrganizacionID.Equals(filaPresentacionRelacionado.OrganizacionID) && recursoRel.ProyectoID.Equals(filaPresentacionRelacionado.ProyectoID) && recursoRel.Orden.Equals(filaPresentacionRelacionado.Orden) && recursoRel.OntologiaID.Equals(filaPresentacionRelacionado.OntologiaID))==null)
            //{
            //    EntityContext.RecursosRelacionadosPresentacion.Add(filaPresentacionRelacionado);
            //}
            
        }

        private void EliminarObjetoConocimiento(Guid ontologiaID, OntologiaProyecto pFilaOntologia)
        {
            foreach (AD.EntityModel.Models.ProyectoDS.PresentacionListadoSemantico filaPresentacionListado in DataWrapperProyecto.ListaPresentacionListadoSemantico.Where(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID.Equals(ontologiaID) && presentacion.ProyectoID.Equals(pFilaOntologia.ProyectoID)).ToList())
            {
                mEntityContext.EliminarElemento(filaPresentacionListado);
                DataWrapperProyecto.ListaPresentacionListadoSemantico.Remove(filaPresentacionListado);
            }
            foreach (AD.EntityModel.Models.ProyectoDS.PresentacionMosaicoSemantico filaPresentacionMosaico in DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Where(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID.Equals(ontologiaID) && presentacion.ProyectoID.Equals(pFilaOntologia.ProyectoID)).ToList())
            {
                mEntityContext.EliminarElemento(filaPresentacionMosaico);
                DataWrapperProyecto.ListaPresentacionMosaicoSemantico.Remove(filaPresentacionMosaico);
            }
            foreach (AD.EntityModel.Models.ProyectoDS.PresentacionMapaSemantico filaPresentacionMapa in DataWrapperProyecto.ListaPresentacionMapaSemantico.Where(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID.Equals(ontologiaID) && presentacion.ProyectoID.Equals(pFilaOntologia.ProyectoID)).ToList())
            {
                mEntityContext.EliminarElemento(filaPresentacionMapa);
                DataWrapperProyecto.ListaPresentacionMapaSemantico.Remove(filaPresentacionMapa);
            }
            foreach (AD.EntityModel.Models.ProyectoDS.RecursosRelacionadosPresentacion filaPresentacionRelacionados in DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Where(presentacion => mEntityContext.Entry(presentacion).State != EntityState.Deleted && presentacion.OntologiaID.Equals(ontologiaID)  && presentacion.ProyectoID.Equals(pFilaOntologia.ProyectoID)).ToList())
            {
                mEntityContext.EliminarElemento(filaPresentacionRelacionados);
                DataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Remove(filaPresentacionRelacionados);
            }

            mEntityContext.EliminarElemento(pFilaOntologia);
            DataWrapperProyecto.ListaOntologiaProyecto.Remove(pFilaOntologia);
        }

        #endregion

        #region Invalidar caches

        public void InvalidarCaches(string UrlIntragnoss)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null);
            proyCL.InvalidarFilaProyecto(ProyectoSeleccionado.Clave);
            proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
            proyCL.InvalidarCabeceraMVC(ProyectoSeleccionado.Clave);
            proyCL.InvalidarPresentacionSemantico(ProyectoSeleccionado.Clave);

            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null);
            bool cachearFacetas = !(this.ParametroProyecto.ContainsKey("CacheFacetas") && this.ParametroProyecto["CacheFacetas"].Equals("0"));
            facetaCL.InvalidarCacheFacetasProyecto(ProyectoSeleccionado.Clave, cachearFacetas);
            facetaCL.InvalidarOntologiasProyecto(ProyectoSeleccionado.Clave);

            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null);
            facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(ProyectoSeleccionado.Clave, "*");

            mGnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);
        }

        #endregion

        #region Metodos de errores
        public string ComprobarErrores(List<ObjetoConocimientoModel> pListaObjetosConocimiento)
        {
            //todo
            string errores = "";

            return errores;
        }

        #endregion

        #region Propiedades

        private DataWrapperProyecto DataWrapperProyecto
        {
            get
            {
                if (mDataWrapperProyecto == null)
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null);
                    mDataWrapperProyecto = proyCN.ObtenerProyectoPorID(ProyectoSeleccionado.Clave);
                    mDataWrapperProyecto.Merge(proyCN.ObtenerPresentacionSemantico(ProyectoSeleccionado.Clave));
                }

                return mDataWrapperProyecto;
            }
        }

        #endregion

    }
}
