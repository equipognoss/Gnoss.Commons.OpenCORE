using System;
using System.Collections.Generic;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Util;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using System.Linq;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.CL;
using Microsoft.AspNetCore.Http;
using Es.Riam.AbstractsOpen;

namespace Es.Riam.Gnoss.Web.MVC.Controles.Controladores
{
    public class ControladorDocumentoMVC : ControladorBase
    {
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;
        private IHttpContextAccessor mHttpContextAccessor;

        public ControladorDocumentoMVC(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            mVirtuosoAD = virtuosoAD;
            mConfigService = configService;
            mEntityContext = entityContext;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;
            mHttpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Obtiene la pestaña seleccionada
        /// </summary>
        public KeyValuePair<string, string> ObtenerPestanyaRecurso(Documento pDocumento, UtilIdiomas pUtilIdiomas, string pIdiomaPorDefecto, Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado)
        {
            KeyValuePair<string, string> pestanyaRecurso;
            string nombrePestanya = "";
            string nombreSemPestanya = "";

            Dictionary<Guid, string> listaOntologiasDefinitivas = new Dictionary<Guid, string>();

            if (pDocumento.TipoDocumentacion == TiposDocumentacion.Semantico && pDocumento.FilaDocumento.ElementoVinculadoID.HasValue)
            {
                if (pDocumento.GestorDocumental.ListaDocumentos.ContainsKey(pDocumento.FilaDocumento.ElementoVinculadoID.Value))
                {
                    if (pDocumento.GestorDocumental.ListaDocumentos[pDocumento.FilaDocumento.ElementoVinculadoID.Value].Enlace.EndsWith(".owl"))
                    {
                        string enlace = pDocumento.GestorDocumental.ListaDocumentos[pDocumento.FilaDocumento.ElementoVinculadoID.Value].Enlace;
                        listaOntologiasDefinitivas.Add(pDocumento.FilaDocumento.ElementoVinculadoID.Value, enlace.Substring(0, enlace.Length - 4));
                    }
                }
                else
                {
                    List<Guid> listaGuidOntologias = new List<Guid>();
                    Dictionary<Guid, string> listaOntologias = new Dictionary<Guid, string>();
                    listaGuidOntologias.Add(pDocumento.FilaDocumento.ElementoVinculadoID.Value);

                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    listaOntologias = docCN.ObtenerEnlacesDocumentosPorDocumentoID(listaGuidOntologias);
                    docCN.Dispose();

                    foreach (Guid id in listaOntologias.Keys)
                    {
                        if (listaOntologias[id].EndsWith(".owl"))
                        {
                            listaOntologiasDefinitivas.Add(id, listaOntologias[id].Substring(0, listaOntologias[id].Length - 4));
                        }
                    }
                }
            }

            Guid ontologiaID = Guid.Empty;
            if (pDocumento.TipoDocumentacion == TiposDocumentacion.Semantico && pDocumento.FilaDocumento.ElementoVinculadoID.HasValue)
            {
                ontologiaID = pDocumento.FilaDocumento.ElementoVinculadoID.Value;
            }

            KeyValuePair<string, string> URLyNombre = ObtenerUrlYNombrePestanya(ontologiaID, listaOntologiasDefinitivas, null, pProyectoSeleccionado, pDocumento, pUtilIdiomas);
            nombreSemPestanya = URLyNombre.Key;
            nombrePestanya = URLyNombre.Value;

            if (string.IsNullOrEmpty(nombreSemPestanya) || string.IsNullOrEmpty(nombrePestanya))
            {
                nombreSemPestanya = pUtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
                nombrePestanya = pUtilIdiomas.GetText("COMMON", "BUSQUEDAAVANZADA");
                pestanyaRecurso = new KeyValuePair<string, string>(nombrePestanya, nombreSemPestanya);
            }
            else
            {
                pestanyaRecurso = new KeyValuePair<string, string>(UtilCadenas.ObtenerTextoDeIdioma(nombrePestanya, pUtilIdiomas.LanguageCode, pIdiomaPorDefecto), UtilCadenas.ObtenerTextoDeIdioma(nombreSemPestanya, pUtilIdiomas.LanguageCode, pIdiomaPorDefecto));
            }


            return pestanyaRecurso;
        }

        /// <summary>
        /// Obtiene la URL y el nombre de la pestaña (lo usa la propiedad PestanyaRecurso)
        /// </summary>
        /// <param name="pOntologia"></param>
        /// <param name="pListaOntologiasDefinitivas"></param>
        /// <param name="pPestanyaPadre"></param>
        /// <returns></returns>
        private static KeyValuePair<string, string> ObtenerUrlYNombrePestanya(Guid pOntologia, Dictionary<Guid, string> pListaOntologiasDefinitivas, Guid? pPestanyaPadre, Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado, Documento pDocumento, UtilIdiomas pUtilIdiomas)
        {
            string wherePestanya = "PestanyaPadreID ";
            List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> listaProyectoPestanyaMenu = new List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu>();
            if (pPestanyaPadre.HasValue)
            {
                wherePestanya += "= '" + pPestanyaPadre.Value + "'";
                listaProyectoPestanyaMenu = pProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where(pestanya => pestanya.PestanyaPadreID.Equals(pPestanyaPadre)).OrderBy(pest => pest.Orden).ToList();
            }
            else
            {
                wherePestanya += " is null";
                listaProyectoPestanyaMenu = pProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where(pestanya => pestanya.PestanyaPadreID == null).OrderBy(pest => pest.Orden).ToList();
            }

            string nombreSemPestanya = "";
            string nombrePestanya = "";

            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestañaMenu in listaProyectoPestanyaMenu)
            {
                if (filaPestañaMenu.ProyectoPestanyaBusqueda != null)
                {
                    ProyectoPestanyaBusqueda filaBusqueda = filaPestañaMenu.ProyectoPestanyaBusqueda;

                    if (pDocumento.TipoDocumentacion == TiposDocumentacion.Semantico && pOntologia != Guid.Empty && pListaOntologiasDefinitivas.ContainsKey(pOntologia))
                    {
                        string ontologia = "rdf:type=" + pListaOntologiasDefinitivas[pOntologia];
                        if (filaBusqueda.CampoFiltro.Contains(ontologia))
                        {
                            nombreSemPestanya = filaBusqueda.ProyectoPestanyaMenu.Ruta;
                            nombrePestanya = filaBusqueda.ProyectoPestanyaMenu.Nombre;
                            break;
                        }
                    }
                    else
                    {
                        string tipoDoc = "gnoss:hastipodoc=" + (short)(pDocumento.TipoDocumentacion);
                        //Comprobamos en funcion del tipo de recurso                        
                        if (filaBusqueda.CampoFiltro.Contains(tipoDoc))
                        {
                            nombreSemPestanya = filaBusqueda.ProyectoPestanyaMenu.Ruta;
                            nombrePestanya = filaBusqueda.ProyectoPestanyaMenu.Nombre;
                        }
                        //Si no cumple ningun tipo y no es pregunta, debate... buscamos si hay alguna pagina con rdf:type=recurso
                        else if (pDocumento.TipoDocumentacion != TiposDocumentacion.Debate && pDocumento.TipoDocumentacion != TiposDocumentacion.Pregunta && pDocumento.TipoDocumentacion != TiposDocumentacion.Encuesta)
                        {
                            string rdfType = "rdf:type=recurso";// + pDocumento.TipoDocumentacion.ToString().ToLower();
                            if (filaBusqueda.CampoFiltro.ToLower().EndsWith(rdfType) || filaBusqueda.CampoFiltro.ToLower().Contains(rdfType + "|"))
                            {
                                nombreSemPestanya = filaBusqueda.ProyectoPestanyaMenu.Ruta;
                                nombrePestanya = filaBusqueda.ProyectoPestanyaMenu.Nombre;
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(nombreSemPestanya) || string.IsNullOrEmpty(nombrePestanya))
                {
                    KeyValuePair<string, string> rutaPestanya = ObtenerUrlYNombrePestanya(pOntologia, pListaOntologiasDefinitivas, filaPestañaMenu.PestanyaID, pProyectoSeleccionado, pDocumento, pUtilIdiomas);
                    nombreSemPestanya = rutaPestanya.Key;
                    nombrePestanya = rutaPestanya.Value;
                }
                if (!string.IsNullOrEmpty(nombreSemPestanya) && !string.IsNullOrEmpty(nombrePestanya))
                {
                    break;
                }
            }

            // Solo entra en este IF si no hay pestaña padre
            // Eso significa que no ha encontrado ninguna página específica de búsqueda, buscamos una por defecto
            if (!pPestanyaPadre.HasValue && (string.IsNullOrEmpty(nombreSemPestanya) || string.IsNullOrEmpty(nombrePestanya)))
            {
                switch (pDocumento.TipoDocumentacion)
                {
                    case TiposDocumentacion.Debate:
                        //Where($"TipoPestanya = {(short)TipoPestanyaMenu.Debates}", "orden asc").FirstOrDefault();
                        AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestayaDebates = pProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where(proy => proy.TipoPestanya.Equals((short)TipoPestanyaMenu.Debates)).OrderBy(proy => proy.Orden).FirstOrDefault();
                        if (filaPestayaDebates != null)
                        {
                            nombreSemPestanya = filaPestayaDebates.Ruta;
                            nombrePestanya = filaPestayaDebates.Nombre;
                            if (string.IsNullOrEmpty(nombreSemPestanya))
                            {
                                nombreSemPestanya = pUtilIdiomas.GetText("URLSEM", "DEBATES");
                            }
                            if (string.IsNullOrEmpty(nombrePestanya))
                            {
                                nombrePestanya = pUtilIdiomas.GetText("COMMON", "DEBATES");
                            }
                        }
                        break;
                    case TiposDocumentacion.Pregunta:
                        AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestayaPreguntas = pProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where(proy => proy.TipoPestanya.Equals((short)TipoPestanyaMenu.Preguntas)).OrderBy(proy => proy.Orden).FirstOrDefault();
                        if (filaPestayaPreguntas != null)
                        {
                            nombreSemPestanya = filaPestayaPreguntas.Ruta;
                            nombrePestanya = filaPestayaPreguntas.Nombre;
                            if (string.IsNullOrEmpty(nombreSemPestanya))
                            {
                                nombreSemPestanya = pUtilIdiomas.GetText("URLSEM", "PREGUNTAS");
                            }
                            if (string.IsNullOrEmpty(nombrePestanya))
                            {
                                nombrePestanya = pUtilIdiomas.GetText("COMMON", "PREGUNTAS");
                            }
                        }
                        break;
                    case TiposDocumentacion.Encuesta:
                        AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestayaEncuestas = pProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where(proy => proy.TipoPestanya.Equals((short)TipoPestanyaMenu.Encuestas)).OrderBy(proy => proy.Orden).FirstOrDefault();
                        if (filaPestayaEncuestas != null)
                        {
                            nombreSemPestanya = filaPestayaEncuestas.Ruta;
                            nombrePestanya = filaPestayaEncuestas.Nombre;
                            if (string.IsNullOrEmpty(nombreSemPestanya))
                            {
                                nombreSemPestanya = pUtilIdiomas.GetText("URLSEM", "ENCUESTAS");
                            }
                            if (string.IsNullOrEmpty(nombrePestanya))
                            {
                                nombrePestanya = pUtilIdiomas.GetText("COMMON", "ENCUESTAS");
                            }
                        }
                        break;
                    default:
                        AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestayaRecursos = pProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where(proy => proy.TipoPestanya.Equals((short)TipoPestanyaMenu.Recursos)).OrderBy(proy => proy.Orden).FirstOrDefault();
                        if (filaPestayaRecursos != null)
                        {
                            nombreSemPestanya = filaPestayaRecursos.Ruta;
                            nombrePestanya = filaPestayaRecursos.Nombre;
                            if (string.IsNullOrEmpty(nombreSemPestanya))
                            {
                                nombreSemPestanya = pUtilIdiomas.GetText("URLSEM", "RECURSOS");
                            }
                            if (string.IsNullOrEmpty(nombrePestanya))
                            {
                                nombrePestanya = pUtilIdiomas.GetText("COMMON", "RECURSOS");
                            }
                        }
                        break;
                }
            }

            return new KeyValuePair<string, string>(nombreSemPestanya, nombrePestanya);
        }

        /// <summary>
        /// Indica si la vista que se va a usar para el controlador es personalizada o no.
        /// </summary>
        public bool ComprobarVistaPersonalizadaDocumento(Documento pDocumento, CommunityModel pComunidad, string pNombreControlador)
        {
            if (pComunidad != null)
            {
                List<string> listaPersonalizaciones = pComunidad.ListaPersonalizaciones;

                string rdfType = pDocumento.TipoDocumentacion.ToString();

                if (pDocumento.TipoDocumentacion == TiposDocumentacion.Semantico)
                {
                    if (!pDocumento.GestorDocumental.ListaDocumentos.ContainsKey(pDocumento.ElementoVinculadoID))
                    {
                        DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        pDocumento.GestorDocumental.DataWrapperDocumentacion.Merge(docCN.ObtenerDocumentoPorID(pDocumento.ElementoVinculadoID));
                        docCN.Dispose();

                        pDocumento.GestorDocumental.CargarDocumentos(false);
                    }

                    rdfType = System.IO.Path.GetFileNameWithoutExtension(pDocumento.GestorDocumental.ListaDocumentos[pDocumento.ElementoVinculadoID].Enlace);
                }

                return (listaPersonalizaciones.Contains("/Views/" + pNombreControlador + "_" + rdfType + "/Index.cshtml"));
            }

            return false;
        }

        public void CargarIdentidadesLectoresEditores(Documento pDocumento, GestorDocumental pGestorDocumental, Guid pProyectoActualID)
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            List<Guid> listaIdentidadesURLSem = new List<Guid>();
            List<Guid> listaPerfilesURlSem = new List<Guid>();
            List<Guid> listaGrupos = new List<Guid>();

            // Cargamos editores

            //Cargo las identidades de los editores
            foreach (Es.Riam.Gnoss.Elementos.Documentacion.EditorRecurso editor in pDocumento.ListaPerfilesEditores.Values)
            {
                Identidad identidadEnProyectoActual = editor.ObtenerIdentidadEditorEnProyecto(pProyectoActualID);

                if ((!listaPerfilesURlSem.Contains(editor.FilaEditor.PerfilID)) && (identidadEnProyectoActual == null || pGestorDocumental.GestorIdentidades == null || !pGestorDocumental.GestorIdentidades.ListaIdentidades.ContainsKey(identidadEnProyectoActual.Clave)))
                {
                    listaPerfilesURlSem.Add(editor.FilaEditor.PerfilID);
                }
            }

            //Cargo los grupos de los editores
            foreach (GrupoEditorRecurso grupoEditor in pDocumento.ListaGruposEditores.Values)
            {
                if (!listaGrupos.Contains(grupoEditor.Clave))
                {
                    listaGrupos.Add(grupoEditor.Clave);
                }
            }

            // Carga de DS para Identidades y Editores

            if (listaIdentidadesURLSem.Count > 0)
            {
                GestionPersonas gestorPersonas = new GestionPersonas(new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication).ObtenerPersonasPorIdentidadesCargaLigera(listaIdentidadesURLSem), mLoggingService, mEntityContext);
                GestionOrganizaciones gestorOrg = new GestionOrganizaciones(new DataWrapperOrganizacion(), mLoggingService, mEntityContext);

                //Obtenemos las identidades por su ID
                pGestorDocumental.GestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadesPorID(listaIdentidadesURLSem, false), gestorPersonas, gestorOrg, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                pGestorDocumental.GestorIdentidades.ListaIdentidadesVisiblesExternos = identidadCN.ObtenerListaIdentidadesVisiblesExternos(listaIdentidadesURLSem);
            }

            if (listaPerfilesURlSem.Count > 0)
            {
                //Obtenemos las identidades de los perfiles.
                if (pGestorDocumental.GestorIdentidades == null)
                {
                    pGestorDocumental.GestorIdentidades = new GestionIdentidades(identidadCN.ObtenerIdentidadesDePerfiles(listaPerfilesURlSem), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                }
                else
                {
                    pGestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadesDePerfiles(listaPerfilesURlSem));
                }
                pGestorDocumental.GestorIdentidades.RecargarHijos();
            }

            if (listaGrupos.Count > 0)
            {
                DataWrapperIdentidad identDW = identidadCN.ObtenerGruposPorIDGrupo(listaGrupos, false);
                GestionIdentidades gestorIdent = new GestionIdentidades(identDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                if (pGestorDocumental.GestorIdentidades == null)
                {
                    pGestorDocumental.GestorIdentidades = gestorIdent;
                }
                else
                {
                    pGestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(gestorIdent.DataWrapperIdentidad);
                    pGestorDocumental.GestorIdentidades.RecargarHijos();
                }
            }

            identidadCN.Dispose();
        }

        public void CargarEditoresLectores(ResourceModel pFichaRecurso, GestorDocumental pGestorDocumental, Documento pDocumento, Identidad pIdentidadActual, bool pEsIdentidadInvitada, bool pMostrarEditores, UtilIdiomas pUtilIdiomas, string pBaseURLIdioma, Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado, Elementos.ServiciosGenerales.Proyecto pProyectoVirtual)
        {
            if (pGestorDocumental == null)
            {
                pGestorDocumental = pDocumento.GestorDocumental;
            }

            CargarIdentidadesLectoresEditores(pDocumento, pGestorDocumental, pProyectoSeleccionado.Clave);

            if (pMostrarEditores)
            {
                // Editores

                if (pDocumento.ListaPerfilesEditoresSinLectores.Count + pDocumento.ListaGruposEditoresSinLectores.Count > 1)
                {
                    pFichaRecurso.Editors = new Dictionary<string, string>();

                    foreach (Guid idEditor in pDocumento.ListaPerfilesEditoresSinLectores.Keys)
                    {
                        if (pDocumento.CreadorID != idEditor)
                        {
                            Es.Riam.Gnoss.Elementos.Documentacion.EditorRecurso editor = pDocumento.ListaPerfilesEditoresSinLectores[idEditor];
                            Identidad identidadEnProyectoActual = editor.ObtenerIdentidadEditorEnProyecto(pProyectoSeleccionado.Clave);

                            string NombreEditor = "";
                            string EnlaceEditor = "";

                            if (identidadEnProyectoActual != null)
                            {
                                NombreEditor = identidadEnProyectoActual.Nombre();

                                if (pEsIdentidadInvitada && (identidadEnProyectoActual.Tipo == AD.Identidad.TiposIdentidad.ProfesionalCorporativo && pIdentidadActual.OrganizacionID.HasValue && identidadEnProyectoActual.OrganizacionID == pIdentidadActual.OrganizacionID))
                                {
                                    NombreEditor = identidadEnProyectoActual.PerfilUsuario.NombrePersonaEnOrganizacion;
                                }

                                if (pDocumento.GestorDocumental.GestorIdentidades.EsIdentidadVisibleExternos(identidadEnProyectoActual) || !pEsIdentidadInvitada)
                                {
                                    if (pProyectoVirtual.Clave != ProyectoAD.MetaProyecto)
                                    {
                                        EnlaceEditor = UrlsSemanticas.ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pProyectoVirtual.NombreCorto) + "/";
                                    }

                                    if (identidadEnProyectoActual.TrabajaConOrganizacion)
                                    {
                                        EnlaceEditor += UrlsSemanticas.ObtenerURLOrganizacionOClase(pUtilIdiomas, identidadEnProyectoActual.OrganizacionID.Value) + "/" + identidadEnProyectoActual.PerfilUsuario.NombreCortoOrg + "/";
                                    }

                                    if (identidadEnProyectoActual.ModoPersonal)
                                    {
                                        EnlaceEditor += pUtilIdiomas.GetText("URLSEM", "PERSONA") + "/" + identidadEnProyectoActual.PerfilUsuario.NombreCortoUsu;
                                    }
                                }
                            }
                            else
                            {
                                NombreEditor = editor.IdentidadEnCualquierProyecto.Nombre();

                                if (pEsIdentidadInvitada && (editor.IdentidadEnCualquierProyecto.Tipo == TiposIdentidad.ProfesionalCorporativo && pIdentidadActual.OrganizacionID.HasValue && editor.IdentidadEnCualquierProyecto.OrganizacionID == pIdentidadActual.OrganizacionID))
                                {
                                    NombreEditor = editor.IdentidadEnCualquierProyecto.PerfilUsuario.NombrePersonaEnOrganizacion;
                                }
                            }

                            if (!pFichaRecurso.Editors.ContainsKey(NombreEditor))
                            {
                                pFichaRecurso.Editors.Add(NombreEditor, EnlaceEditor);
                            }
                        }
                    }
                }

                // Grupos Editores

                if (pDocumento.ListaGruposEditoresSinLectores.Count > 0)
                {
                    if (pFichaRecurso.Editors == null)
                    {
                        pFichaRecurso.Editors = new Dictionary<string, string>();
                    }

                    foreach (Guid idGrupoEditor in pDocumento.ListaGruposEditoresSinLectores.Keys)
                    {
                        GrupoEditorRecurso grupoEditor = pDocumento.ListaGruposEditoresSinLectores[idGrupoEditor];

                        if (grupoEditor.FilaGrupoIdentidadProyecto != null)
                        {
                            if (grupoEditor.FilaGrupoIdentidadProyecto.ProyectoID == pProyectoSeleccionado.Clave)
                            {
                                if (grupoEditor.FilaGrupoIdentidad.Publico || (!pEsIdentidadInvitada && grupoEditor.listaIdentidadedsParticipacion.Contains(pIdentidadActual.Clave)))
                                {
                                    string urlGrupo = UrlsSemanticas.ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pProyectoSeleccionado.NombreCorto) + "/" + pUtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + grupoEditor.FilaGrupoIdentidad.NombreCorto;

                                    if (!pFichaRecurso.Editors.ContainsKey(grupoEditor.Nombre))
                                    {
                                        pFichaRecurso.Editors.Add(grupoEditor.Nombre, urlGrupo);
                                    }
                                }
                            }
                            else
                            {
                                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                                string nombreProyGrupo = proyCL.ObtenerNombreDeProyectoID(grupoEditor.FilaGrupoIdentidadProyecto.ProyectoID);
                                ;

                                if (!pFichaRecurso.Editors.ContainsKey(grupoEditor.Nombre + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + nombreProyGrupo))
                                {
                                    pFichaRecurso.Editors.Add(grupoEditor.Nombre + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + nombreProyGrupo, "");
                                }
                            }
                        }
                        else if (grupoEditor.FilaGrupoIdentidadOrganizacion != null)
                        {
                            OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            string nombreOrgGrupo = orgCN.ObtenerNombreOrganizacionPorID(grupoEditor.FilaGrupoIdentidadOrganizacion.OrganizacionID).Nombre;
                            orgCN.Dispose();

                            if (!pFichaRecurso.Editors.ContainsKey(grupoEditor.Nombre + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + nombreOrgGrupo))
                            {
                                pFichaRecurso.Editors.Add(grupoEditor.Nombre + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + nombreOrgGrupo, "");
                            }
                        }
                    }
                }

                // Lectores

                pFichaRecurso.Readers = new Dictionary<string, string>();

                if (pDocumento.ListaPerfilesEditores.Count - pDocumento.ListaPerfilesEditoresSinLectores.Count > 0)
                {
                    List<Guid> listaLectores = new List<Guid>();

                    foreach (Guid idLector in pDocumento.ListaPerfilesEditores.Keys)
                    {
                        if (!pDocumento.ListaPerfilesEditoresSinLectores.ContainsKey(idLector))
                        {
                            listaLectores.Add(idLector);
                        }
                    }

                    foreach (Guid idLector in listaLectores)
                    {
                        Es.Riam.Gnoss.Elementos.Documentacion.EditorRecurso lector = pDocumento.ListaPerfilesEditores[idLector];

                        Identidad identidadEnProyectoActual = lector.ObtenerIdentidadEditorEnProyecto(pProyectoSeleccionado.Clave);

                        string NombreLector = "";
                        string EnlaceLector = "";

                        if (identidadEnProyectoActual != null)
                        {
                            NombreLector = identidadEnProyectoActual.Nombre();

                            if (pEsIdentidadInvitada && (identidadEnProyectoActual.Tipo == AD.Identidad.TiposIdentidad.ProfesionalCorporativo && pIdentidadActual.OrganizacionID.HasValue && identidadEnProyectoActual.OrganizacionID == pIdentidadActual.OrganizacionID))
                            {
                                NombreLector = identidadEnProyectoActual.PerfilUsuario.NombrePersonaEnOrganizacion;
                            }

                            if (!pEsIdentidadInvitada || pDocumento.GestorDocumental.GestorIdentidades.EsIdentidadVisibleExternos(identidadEnProyectoActual))
                            {
                                if (pProyectoVirtual.Clave != ProyectoAD.MetaProyecto)
                                {
                                    EnlaceLector = UrlsSemanticas.ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pProyectoVirtual.NombreCorto) + "/";
                                }

                                if (identidadEnProyectoActual.TrabajaConOrganizacion)
                                {
                                    EnlaceLector += UrlsSemanticas.ObtenerURLOrganizacionOClase(pUtilIdiomas, identidadEnProyectoActual.OrganizacionID.Value) + "/" + identidadEnProyectoActual.PerfilUsuario.NombreCortoOrg + "/";
                                }

                                if (identidadEnProyectoActual.ModoPersonal)
                                {
                                    EnlaceLector += pUtilIdiomas.GetText("URLSEM", "PERSONA") + "/" + identidadEnProyectoActual.PerfilUsuario.NombreCortoUsu;
                                }
                            }
                        }
                        else
                        {
                            NombreLector = lector.IdentidadEnCualquierProyecto.Nombre();

                            if (pEsIdentidadInvitada && (lector.IdentidadEnCualquierProyecto.Tipo == TiposIdentidad.ProfesionalCorporativo && pIdentidadActual.OrganizacionID.HasValue && lector.IdentidadEnCualquierProyecto.OrganizacionID == pIdentidadActual.OrganizacionID))
                            {
                                NombreLector = lector.IdentidadEnCualquierProyecto.NombreOrganizacion;
                            }
                        }

                        if (!pFichaRecurso.Readers.ContainsKey(NombreLector))
                        {
                            pFichaRecurso.Readers.Add(NombreLector, EnlaceLector);
                        }
                    }
                }


                // Grupos lectores

                if (pDocumento.ListaGruposEditores.Count - pDocumento.ListaGruposEditoresSinLectores.Count > 0)
                {
                    List<Guid> listaGruposLectores = new List<Guid>();

                    foreach (Guid idGrupoLector in pDocumento.ListaGruposEditores.Keys)
                    {
                        if (!pDocumento.ListaGruposEditoresSinLectores.ContainsKey(idGrupoLector))
                        {
                            listaGruposLectores.Add(idGrupoLector);
                        }
                    }

                    foreach (Guid idGrupoLector in listaGruposLectores)
                    {
                        Es.Riam.Gnoss.Elementos.Documentacion.GrupoEditorRecurso grupoLector = pDocumento.ListaGruposEditores[idGrupoLector];
                        if (grupoLector.FilaGrupoIdentidadProyecto != null)
                        {
                            if (grupoLector.FilaGrupoIdentidadProyecto.ProyectoID == pProyectoSeleccionado.Clave)
                            {
                                if (grupoLector.FilaGrupoIdentidad.Publico || (!pEsIdentidadInvitada && grupoLector.listaIdentidadedsParticipacion.Contains(pIdentidadActual.Clave)))
                                {
                                    string urlGrupo = UrlsSemanticas.ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pProyectoSeleccionado.NombreCorto) + "/" + pUtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + grupoLector.FilaGrupoIdentidad.NombreCorto;

                                    if (!pFichaRecurso.Readers.ContainsKey(grupoLector.Nombre))
                                    {
                                        pFichaRecurso.Readers.Add(grupoLector.Nombre, urlGrupo);
                                    }
                                }
                            }
                            else
                            {
                                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                                string nombreProyGrupo = proyCL.ObtenerNombreDeProyectoID(grupoLector.FilaGrupoIdentidadProyecto.ProyectoID);
                                ;

                                if (!pFichaRecurso.Readers.ContainsKey(grupoLector.Nombre + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + nombreProyGrupo))
                                {
                                    pFichaRecurso.Readers.Add(grupoLector.Nombre + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + nombreProyGrupo, "");
                                }
                            }
                        }
                        else if (grupoLector.FilaGrupoIdentidadOrganizacion != null)
                        {
                            OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            string nombreOrgGrupo = orgCN.ObtenerNombreOrganizacionPorID(grupoLector.FilaGrupoIdentidadOrganizacion.OrganizacionID).Nombre;
                            orgCN.Dispose();

                            if (!pFichaRecurso.Readers.ContainsKey(grupoLector.Nombre + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + nombreOrgGrupo))
                            {
                                pFichaRecurso.Readers.Add(grupoLector.Nombre + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + nombreOrgGrupo, "");
                            }
                        }
                    }
                }
            }
        }

    }
}
