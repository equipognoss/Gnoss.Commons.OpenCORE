﻿using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorUtilidades
    {
        private DataWrapperDocumentacion mDataWrapperDocumentacion = null;
        private DataWrapperProyecto mDataWrapperProyecto = null;
        private Dictionary<string, string> mParametroProyecto;

        private Elementos.ServiciosGenerales.Proyecto ProyectoSeleccionado = null;

        private GestorParametroGeneral mParametrosGeneralesDS;
        private ParametroGeneral mFilaParametrosGenerales = null;

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private IHttpContextAccessor mHttpContextAccessor;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;
        private EntityContextBASE mEntityContextBASE;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public ControladorUtilidades(Elementos.ServiciosGenerales.Proyecto pProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, EntityContextBASE entityContextBASE, GnossCache gnossCache, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mHttpContextAccessor = httpContextAccessor;
            mRedisCacheWrapper = redisCacheWrapper;
            mEntityContextBASE = entityContextBASE;
            mGnossCache = gnossCache;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            ProyectoSeleccionado = pProyecto;
        }

        #endregion

        #region Métodos de carga

        public AdministrarComunidadUtilidades CargarUtilidades()
        {
            AdministrarComunidadUtilidades paginaModel = new AdministrarComunidadUtilidades();

            paginaModel.WikiDisponible = FilaParametrosGenerales.WikiDisponible;

            paginaModel.PermisosDocumentacion = CargarPermisosRecursos();
            paginaModel.PermisosDocumentacionSemantica = CargarPermisosRecursosSemanticos();

            paginaModel.NivelesCertificacionDisponibles = FilaParametrosGenerales.PermitirCertificacionRec;

            paginaModel.NivelesCertificacion = new List<AdministrarComunidadUtilidades.NivelCertificacion>();

            List<Guid> nivelesCertificacion = new List<Guid>();

            foreach (AD.EntityModel.Models.ProyectoDS.NivelCertificacion nivelCertificacionRow in DataWrapperProyecto.ListaNivelCertificacion)
            {
                nivelesCertificacion.Add(nivelCertificacionRow.NivelCertificacionID);
            }

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, bool> listaNivelesConDocAsociados = proyCN.ExisteDocAsociadoANivelCertif(nivelesCertificacion);

            foreach (AD.EntityModel.Models.ProyectoDS.NivelCertificacion nivelCertificacionRow in DataWrapperProyecto.ListaNivelCertificacion)
            {
                AdministrarComunidadUtilidades.NivelCertificacion nivelCertificacion = new AdministrarComunidadUtilidades.NivelCertificacion();
                nivelCertificacion.CertificacionID = nivelCertificacionRow.NivelCertificacionID;
                nivelCertificacion.Nombre = nivelCertificacionRow.Descripcion;
                nivelCertificacion.Orden = nivelCertificacionRow.Orden;
                nivelCertificacion.TieneDocsAsociados = listaNivelesConDocAsociados.ContainsKey(nivelCertificacionRow.NivelCertificacionID) && listaNivelesConDocAsociados[nivelCertificacionRow.NivelCertificacionID];

                paginaModel.NivelesCertificacion.Add(nivelCertificacion);
            }

            //Añade la política de certificación de la comunidad
            if (FilaParametrosGenerales.PoliticaCertificacion != null && !FilaParametrosGenerales.PoliticaCertificacion.Trim().Equals(string.Empty))
            {
                paginaModel.PoliticaCertificacion = FilaParametrosGenerales.PoliticaCertificacion.Trim();
            }

            paginaModel.PermitirDescargarDocUsuInvitado = FilaParametrosGenerales.PermitirUsuNoLoginDescargDoc;

            return paginaModel;
        }


        private List<AdministrarComunidadUtilidades.PermisoDocumentacion> CargarPermisosRecursos()
        {
            List<AdministrarComunidadUtilidades.PermisoDocumentacion> permisosDocumentacion = new List<AdministrarComunidadUtilidades.PermisoDocumentacion>();

            foreach (AD.EntityModel.Models.ProyectoDS.TipoDocDispRolUsuarioProy tipoDocDispRolUsuarioProyRow in DataWrapperProyecto.ListaTipoDocDispRolUsuarioProy)
            {
                AdministrarComunidadUtilidades.PermisoDocumentacion permiso = new AdministrarComunidadUtilidades.PermisoDocumentacion();

                permiso.TipoDocumento = (TiposDocumentacion)tipoDocDispRolUsuarioProyRow.TipoDocumento;
                permiso.TipoPermiso = tipoDocDispRolUsuarioProyRow.RolUsuario;

                permisosDocumentacion.Add(permiso);
            }
            return permisosDocumentacion;
        }

        private List<AdministrarComunidadUtilidades.PermisoDocumentacionSemantica> CargarPermisosRecursosSemanticos()
        {
            List<AdministrarComunidadUtilidades.PermisoDocumentacionSemantica> permisosDocumentacion = new List<AdministrarComunidadUtilidades.PermisoDocumentacionSemantica>();

            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            Dictionary<Guid, string> listaOntologiasTitulo = new Dictionary<Guid, string>();
            Dictionary<Guid, string> listaOntologiasOWL = new Dictionary<Guid, string>();
            List<Guid> listaSecundarias = new List<Guid>();

            foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in DataWrapperDocumentacion.ListaDocumento)
            {
                if (filaDoc.Tipo.Equals((short)TiposDocumentacion.Ontologia) || filaDoc.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria))
                {
                    if (!listaOntologiasTitulo.ContainsKey(filaDoc.DocumentoID))
                    {
                        listaOntologiasTitulo.Add(filaDoc.DocumentoID, filaDoc.Titulo);
                        listaOntologiasOWL.Add(filaDoc.DocumentoID, filaDoc.Enlace);
                    }
                    if (filaDoc.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria))
                    {
                        listaSecundarias.Add(filaDoc.DocumentoID);
                    }
                }
            }

            if (listaOntologiasTitulo.Count > 0)
            {
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                Dictionary<Guid, List<Guid>> dicGruposOntologias = proyCN.ObtenerGruposPermitidosOntologiasEnProyecto(listaOntologiasTitulo.Keys.ToList(), ProyectoSeleccionado.Clave);
                proyCN.Dispose();

                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                foreach (Guid ontologia in listaOntologiasTitulo.Keys)
                {
                    AdministrarComunidadUtilidades.PermisoDocumentacionSemantica permiso = new AdministrarComunidadUtilidades.PermisoDocumentacionSemantica();

                    permiso.TipoDocumento = listaOntologiasTitulo[ontologia];
                    permiso.Ontologia = listaOntologiasOWL[ontologia];
                    permiso.TipoPermiso = -1;
                    permiso.EsSecundaria = false;

                    if (listaSecundarias.Contains(ontologia))
                    {
                        permiso.EsSecundaria = true;
                    }

                    List<AD.EntityModel.Models.ProyectoDS.TipoOntoDispRolUsuarioProy> tiposDocDispRolUsuarioProyRow = DataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy.Where(onto => onto.OntologiaID.Equals(ontologia)).ToList();

                    if (tiposDocDispRolUsuarioProyRow.Count > 0)
                    {
                        permiso.TipoPermiso = tiposDocDispRolUsuarioProyRow[0].RolUsuario;
                    }

                    permiso.PrivacidadGrupos = new Dictionary<Guid, string>();

                    if (dicGruposOntologias.ContainsKey(ontologia))
                    {
                        Dictionary<Guid, string> nombresGrupos = identCN.ObtenerNombresDeGrupos(dicGruposOntologias[ontologia]);

                        foreach (Guid grupoID in dicGruposOntologias[ontologia])
                        {
                            permiso.PrivacidadGrupos.Add(grupoID, nombresGrupos[grupoID]);
                        }
                    }

                    permisosDocumentacion.Add(permiso);
                }
            }
            return permisosDocumentacion.OrderBy(p => p.EsSecundaria ? 1 : 0).ToList();
        }

        #endregion

        #region Métodos de Guardado

        public void EliminarCertificacionNoIncluida(AdministrarComunidadUtilidades utilidades)
        {
            List<AD.EntityModel.Models.ProyectoDS.NivelCertificacion> listaNivelCertBorrar = DataWrapperProyecto.ListaNivelCertificacion.ToList();
            //Guardo los nuevos
            GuardarNivelesCertificacion(utilidades);
            Dictionary<Guid, Guid> nivelesAntiguosNuevos = DevolverEquivalenciaNivelesNuevos(utilidades, listaNivelCertBorrar);
            foreach (AD.EntityModel.Models.ProyectoDS.NivelCertificacion filaNivel in listaNivelCertBorrar)
            {
                if (!utilidades.NivelesCertificacion.Exists(nivel => nivel.CertificacionID.Equals(filaNivel.NivelCertificacionID)))
                {
                    foreach (AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos docWebVin in DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos)
                    {
                        //Modifico los valores viejos con los nuevos, o si se han eliminado la certificación se quita del documento.
                        if (nivelesAntiguosNuevos.ContainsKey(docWebVin.NivelCertificacion.NivelCertificacionID))
                        {
                            docWebVin.NivelCertificacionID = nivelesAntiguosNuevos[docWebVin.NivelCertificacionID.Value];
                        }
                        else
                        {
                            docWebVin.NivelCertificacionID = null;
                            docWebVin.NivelCertificacion = null;
                        }
                    }
                    DataWrapperProyecto.ListaNivelCertificacion.Remove(filaNivel);
                    mEntityContext.EliminarElemento(filaNivel);
                    // filaNivel.Delete();
                }
            }
        }

        private Dictionary<Guid, Guid> DevolverEquivalenciaNivelesNuevos(AdministrarComunidadUtilidades utilidades, List<NivelCertificacion> listaNivelCertBorrar)
        {
            Dictionary<Guid, Guid> devolver = new Dictionary<Guid, Guid>();
            foreach(AdministrarComunidadUtilidades.NivelCertificacion nivel in utilidades.NivelesCertificacion)
            {
                if(listaNivelCertBorrar.Exists( x=> x.Descripcion.Equals(nivel.Nombre)))
                {
                    devolver.Add(listaNivelCertBorrar.Where(x => x.Descripcion.Equals(nivel.Nombre)).Select(x => x.NivelCertificacionID).FirstOrDefault(), nivel.CertificacionID);
                }
            }

            return devolver;
        }

        public void GuardarUtilidades(AdministrarComunidadUtilidades DatosGuardado)
        {
            GuardarPermisosRecursos(DatosGuardado.PermisosDocumentacion);
            GuardarPermisosRecursosSemanticos(DatosGuardado.PermisosDocumentacionSemantica);
            // No es necesario guardar Niveles de certificación. Esto se realiza en otra sección/página
            //GuardarNivelesCertificacion(DatosGuardado);

            FilaParametrosGenerales.PermitirUsuNoLoginDescargDoc = DatosGuardado.PermitirDescargarDocUsuInvitado;

            mEntityContext.SaveChanges();
        }

        // Método para guardar exclusivamente los niveles de certificación. Es necesario para el nuevo Front de Administración ya que la gestión se realiza en una página adicional.
        public void GuardarUtilidadesNivelesCertificacion(AdministrarComunidadUtilidades DatosGuardado)
        {                        
            GuardarNivelesCertificacion(DatosGuardado);            
            mEntityContext.SaveChanges();
        }

        public void CargarNivelesCertificacion(AdministrarComunidadUtilidades pDatosGuardado, string pPoliticaParametrosGenerales, Identidad pIdentidadActual, TipoProyecto pTipoProyecto, UtilIdiomas pUtilIdiomas)
        {
            pDatosGuardado.NivelesCertificacionDisponibles = FilaParametrosGenerales.PermitirCertificacionRec;
            pDatosGuardado.NivelesCertificacion = new List<AdministrarComunidadUtilidades.NivelCertificacion>();
            List<Guid> nivelesCertificacion = new List<Guid>();

            foreach (AD.EntityModel.Models.ProyectoDS.NivelCertificacion nivelCertificacionRow in DataWrapperProyecto.ListaNivelCertificacion)
            {
                nivelesCertificacion.Add(nivelCertificacionRow.NivelCertificacionID);
            }

            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, bool> listaNivelesConDocAsociados = proyCN.ExisteDocAsociadoANivelCertif(nivelesCertificacion);

            foreach (AD.EntityModel.Models.ProyectoDS.NivelCertificacion nivelCertificacionRow in DataWrapperProyecto.ListaNivelCertificacion)
            {
                AdministrarComunidadUtilidades.NivelCertificacion nivelCertificacion = new AdministrarComunidadUtilidades.NivelCertificacion();
                nivelCertificacion.CertificacionID = nivelCertificacionRow.NivelCertificacionID;
                nivelCertificacion.Nombre = nivelCertificacionRow.Descripcion;
                nivelCertificacion.Orden = nivelCertificacionRow.Orden;
                nivelCertificacion.TieneDocsAsociados = listaNivelesConDocAsociados.ContainsKey(nivelCertificacionRow.NivelCertificacionID) && listaNivelesConDocAsociados[nivelCertificacionRow.NivelCertificacionID];

                pDatosGuardado.NivelesCertificacion.Add(nivelCertificacion);
            }

            if (FilaParametrosGenerales.PoliticaCertificacion != null && !FilaParametrosGenerales.PoliticaCertificacion.Trim().Equals(string.Empty))
            {
                pDatosGuardado.PoliticaCertificacion = FilaParametrosGenerales.PoliticaCertificacion.Trim();
            }

            if (string.IsNullOrEmpty(pPoliticaParametrosGenerales) || string.IsNullOrWhiteSpace(pPoliticaParametrosGenerales))
            {
                //TipoProyecto.EducacionExpandida
                if (pTipoProyecto != AD.ServiciosGenerales.TipoProyecto.EducacionExpandida)
                {
                    pDatosGuardado.PoliticaCertificacion = pUtilIdiomas.GetText("COMADMIN", "ELABORANDOPOLITICA", pIdentidadActual.Nombre(), "administrador");
                }
                else
                {
                    pDatosGuardado.PoliticaCertificacion = pUtilIdiomas.GetText("COMADMIN", "POLITICACERTIFICACION", pIdentidadActual.Nombre());
                }

            }

        }

        public void CargarTiposYPermisos(AdministrarComunidadUtilidades pDatosGuardado)
        {
            pDatosGuardado.WikiDisponible = FilaParametrosGenerales.WikiDisponible;
            pDatosGuardado.PermisosDocumentacion = CargarPermisosRecursos();
            pDatosGuardado.PermisosDocumentacionSemantica = CargarPermisosRecursosSemanticos();
            pDatosGuardado.PermitirDescargarDocUsuInvitado = FilaParametrosGenerales.PermitirUsuNoLoginDescargDoc;
        }

        private void GuardarNivelesCertificacion(AdministrarComunidadUtilidades DatosGuardado)
        {
            FilaParametrosGenerales.PermitirCertificacionRec = DatosGuardado.NivelesCertificacionDisponibles;

            if (DatosGuardado.NivelesCertificacionDisponibles)
            {
                FilaParametrosGenerales.PoliticaCertificacion = HttpUtility.UrlDecode(DatosGuardado.PoliticaCertificacion);
                List<NivelCertificacion> listaNivelCertificacionBorrar = DataWrapperProyecto.ListaNivelCertificacion.ToList();
                foreach (NivelCertificacion filaNivel in listaNivelCertificacionBorrar)
                {
                    if (DatosGuardado.NivelesCertificacion.Count(nivel => nivel.CertificacionID == filaNivel.NivelCertificacionID) == 0)
                    {
                        EliminarNivelCertificacion(filaNivel);
                    }
                }

                //Si son nuevos los añade, sino los modifica
                foreach (AdministrarComunidadUtilidades.NivelCertificacion nivelCertificacion in DatosGuardado.NivelesCertificacion)
                {
                    NivelCertificacion filaNivel = DataWrapperProyecto.ListaNivelCertificacion.FirstOrDefault(nivelCert => nivelCert.NivelCertificacionID.Equals(nivelCertificacion.CertificacionID));

                    if (filaNivel == null)
                    {
                        filaNivel = new AD.EntityModel.Models.ProyectoDS.NivelCertificacion();
                        filaNivel.NivelCertificacionID = nivelCertificacion.CertificacionID;
                        filaNivel.Descripcion = nivelCertificacion.Nombre;
                        filaNivel.Orden = nivelCertificacion.Orden;
                        filaNivel.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                        filaNivel.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;

                        DataWrapperProyecto.ListaNivelCertificacion.Add(filaNivel);
                        mEntityContext.NivelCertificacion.Add(filaNivel);
                    }
                    else
                    {
                        filaNivel.Orden = nivelCertificacion.Orden;
                        filaNivel.Descripcion = nivelCertificacion.Nombre;
                    }
                }
            }
            else
            {
                FilaParametrosGenerales.PoliticaCertificacion = null;
                List<NivelCertificacion> listaNivelCertBorrar = DataWrapperProyecto.ListaNivelCertificacion.ToList();
                foreach (NivelCertificacion filaNivel in listaNivelCertBorrar)
                {
                    EliminarNivelCertificacion(filaNivel);
                }
            }
        }

        private void GuardarPermisosRecursosSemanticos(List<AdministrarComunidadUtilidades.PermisoDocumentacionSemantica> pPermisos)
        {
            List<AdministrarComunidadUtilidades.PermisoDocumentacionSemantica> permisosDocumentacion = new List<AdministrarComunidadUtilidades.PermisoDocumentacionSemantica>();

            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            GestorDocumental gestorDoc = new GestorDocumental(DataWrapperDocumentacion, mLoggingService, mEntityContext);
            gestorDoc.CargarDocumentos();
            DataWrapperDocumentacion.Merge(documentacionCN.ObtenerEditoresDocumentos(new List<Guid>(gestorDoc.ListaDocumentos.Keys)));
            documentacionCN.Dispose();

            Dictionary<string, Guid> listaOntologiasOWL = new Dictionary<string, Guid>();

            foreach (AD.EntityModel.Models.Documentacion.Documento filaDoc in DataWrapperDocumentacion.ListaDocumento)
            {
                if (filaDoc.Tipo.Equals((short)TiposDocumentacion.Ontologia) || filaDoc.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria))
                {
                    if (!listaOntologiasOWL.ContainsKey(filaDoc.Enlace))
                    {
                        listaOntologiasOWL.Add(filaDoc.Enlace, filaDoc.DocumentoID);
                    }
                }
            }


            if (pPermisos != null)
            {
                foreach (AdministrarComunidadUtilidades.PermisoDocumentacionSemantica permisoRecurso in pPermisos)
                {
                    // Si existe, compruebo el permiso y si es distinto se lo cambio.
                    // Si el permiso es -1, lo elimino
                    // Si no existe, lo elimino

                    if (listaOntologiasOWL.ContainsKey(permisoRecurso.Ontologia))
                    {
                        Guid DocumentoID = listaOntologiasOWL[permisoRecurso.Ontologia];

                        TipoOntoDispRolUsuarioProy tiposDocDispRolUsuarioProyRow = DataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy.FirstOrDefault(ont => ont.OntologiaID.Equals(DocumentoID));

                        if (tiposDocDispRolUsuarioProyRow != null)
                        {
                            DataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy.Remove(tiposDocDispRolUsuarioProyRow);
                            mEntityContext.EliminarElemento(tiposDocDispRolUsuarioProyRow);
                        }

                        if (permisoRecurso.TipoPermiso >= 0 && permisoRecurso.TipoPermiso <= 2)
                        {
                            TipoOntoDispRolUsuarioProy tipoDocDisp = new TipoOntoDispRolUsuarioProy();
                            tipoDocDisp.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                            tipoDocDisp.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;
                            tipoDocDisp.RolUsuario = permisoRecurso.TipoPermiso;
                            tipoDocDisp.OntologiaID = DocumentoID;

                            DataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy.Add(tipoDocDisp);
                            mEntityContext.TipoOntoDispRolUsuarioProy.Add(tipoDocDisp);
                        }

                        if (permisoRecurso.PrivacidadGrupos != null)
                        {
                            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                            permisoRecurso.PrivacidadGrupos = identCN.ObtenerNombresDeGrupos(permisoRecurso.PrivacidadGrupos.Keys.ToList());

                            foreach (Guid grupoID in permisoRecurso.PrivacidadGrupos.Keys)
                            {
                                if (!gestorDoc.ListaDocumentos[DocumentoID].ListaGruposEditores.ContainsKey(grupoID))
                                {
                                    gestorDoc.AgregarGrupoEditorARecurso(DocumentoID, grupoID);
                                }
                            }
                            identCN.Dispose();
                        }

                        Documento doc = gestorDoc.ListaDocumentos.Values.FirstOrDefault(d => d.Clave.Equals(DocumentoID) || string.Equals(d.Nombre, permisoRecurso.TipoDocumento, StringComparison.InvariantCultureIgnoreCase));

                        if (doc != null)
                        {
                            foreach (Guid grupoEditorID in doc.ListaGruposEditores.Keys)
                            {
                                if (permisoRecurso.PrivacidadGrupos == null || !permisoRecurso.PrivacidadGrupos.ContainsKey(grupoEditorID))
                                {
                                    List<AD.EntityModel.Models.Documentacion.DocumentoRolGrupoIdentidades> filas = gestorDoc.DataWrapperDocumentacion.ListaDocumentoRolGrupoIdentidades.Where(item => item.DocumentoID.Equals(doc.Clave) && item.GrupoID.Equals(grupoEditorID)).ToList();
                                    if (filas != null && filas.Count > 0)
                                    {
                                        gestorDoc.QuitarGrupoEditorDeRecurso(new GrupoEditorRecurso(filas[0], gestorDoc, mLoggingService));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Elimina el nivel de certificación y lo desvincula de los recursos
        /// </summary>
        /// <param name="pNivelCertificacion"></param>
        private void EliminarNivelCertificacion(NivelCertificacion pNivelCertificacion)
        {
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            List<Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> listaDocumentosRelacionadosNivelCertificacion = documentacionCN.ObtenerBaseRecursosPorNivelCertifiacion(pNivelCertificacion.NivelCertificacionID);
            foreach(Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos documentoWebVinBaseRecursos in listaDocumentosRelacionadosNivelCertificacion)
            {
                documentoWebVinBaseRecursos.NivelCertificacion = null;
                documentoWebVinBaseRecursos.NivelCertificacionID = null;
            }

            DataWrapperProyecto.ListaNivelCertificacion.Remove(pNivelCertificacion);
            mEntityContext.EliminarElemento(pNivelCertificacion);
        }

        private void GuardarPermisosRecursos(List<AdministrarComunidadUtilidades.PermisoDocumentacion> pPermisos)
        {
            AdministrarComunidadUtilidades.PermisoDocumentacion permisoVideoTop = pPermisos.Find(permiso => permiso.TipoDocumento.Equals(TiposDocumentacion.VideoTOP));
            if (permisoVideoTop != null)
            {
                AdministrarComunidadUtilidades.PermisoDocumentacion permisoAudioTop = new AdministrarComunidadUtilidades.PermisoDocumentacion();
                permisoAudioTop.TipoDocumento = TiposDocumentacion.AudioTOP;
                permisoAudioTop.TipoPermiso = permisoVideoTop.TipoPermiso;
                pPermisos.Add(permisoAudioTop);
            }

            foreach (AdministrarComunidadUtilidades.PermisoDocumentacion permisoRecurso in pPermisos)
            {
                // Si existe, compruebo el permiso y si es distinto se lo cambio.
                // Si el permiso es -1, lo elimino
                // Si el permiso es de wiki, pero la wiki no esta disponible, lo elimino
                // Si no existe, lo elimino

                bool eliminarPermiso = false;
                if (permisoRecurso.TipoPermiso < 0 || permisoRecurso.TipoPermiso > 2 || (!FilaParametrosGenerales.WikiDisponible && permisoRecurso.TipoDocumento.Equals(TiposDocumentacion.Wiki)))
                {
                    //El permiso no existe, lo eliminamos
                    eliminarPermiso = true;
                }

                List<AD.EntityModel.Models.ProyectoDS.TipoDocDispRolUsuarioProy> tiposDocDispRolUsuarioProyRow = DataWrapperProyecto.ListaTipoDocDispRolUsuarioProy.Where(lista => lista.TipoDocumento.Equals((short)permisoRecurso.TipoDocumento)).ToList();

                if (tiposDocDispRolUsuarioProyRow.Count > 1)
                {
                    for (int i = 1; i < tiposDocDispRolUsuarioProyRow.Count; i++)
                    {
                        DataWrapperProyecto.ListaTipoDocDispRolUsuarioProy.Remove(tiposDocDispRolUsuarioProyRow[i]);
                        mEntityContext.EliminarElemento(tiposDocDispRolUsuarioProyRow[i]);
                    }
                }
                tiposDocDispRolUsuarioProyRow = DataWrapperProyecto.ListaTipoDocDispRolUsuarioProy.Where(lista => lista.TipoDocumento.Equals((short)permisoRecurso.TipoDocumento)).ToList();
                if (tiposDocDispRolUsuarioProyRow.Count > 0)
                {
                    if (eliminarPermiso)
                    {
                        DataWrapperProyecto.ListaTipoDocDispRolUsuarioProy.Remove(tiposDocDispRolUsuarioProyRow.First());
                        mEntityContext.EliminarElemento(tiposDocDispRolUsuarioProyRow.First());
                    }
                    else if (tiposDocDispRolUsuarioProyRow[0].RolUsuario != permisoRecurso.TipoPermiso)
                    {
                        TipoDocDispRolUsuarioProy tipoDocAComparar = tiposDocDispRolUsuarioProyRow.First();
                        TipoDocDispRolUsuarioProy tipoDoc = mEntityContext.TipoDocDispRolUsuarioProy.FirstOrDefault(tipo => tipo.OrganizacionID.Equals(tipoDocAComparar.OrganizacionID) && tipo.ProyectoID.Equals(tipoDocAComparar.ProyectoID) && tipo.TipoDocumento.Equals(tipoDocAComparar.TipoDocumento) && tipo.RolUsuario.Equals(tipoDocAComparar.RolUsuario));
                        if (tipoDoc != null)
                        {
                            mEntityContext.EliminarElemento(tipoDoc);
                            TipoDocDispRolUsuarioProy tipoDocNuevo = new TipoDocDispRolUsuarioProy();
                            tipoDocNuevo.RolUsuario = permisoRecurso.TipoPermiso;
                            tipoDocNuevo.OrganizacionID = tipoDoc.OrganizacionID;
                            tipoDocNuevo.ProyectoID = tipoDoc.ProyectoID;
                            tipoDocNuevo.TipoDocumento = tipoDoc.TipoDocumento;
                            mEntityContext.TipoDocDispRolUsuarioProy.Add(tipoDocNuevo);
                        }
                        //tiposDocDispRolUsuarioProyRow[0].RolUsuario = permisoRecurso.TipoPermiso;
                    }
                }
                else if (!eliminarPermiso)
                {
                    TipoDocDispRolUsuarioProy tipoDocDisp = new TipoDocDispRolUsuarioProy();
                    tipoDocDisp.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                    tipoDocDisp.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;
                    tipoDocDisp.RolUsuario = permisoRecurso.TipoPermiso;
                    tipoDocDisp.TipoDocumento = (short)permisoRecurso.TipoDocumento;
                    DataWrapperProyecto.ListaTipoDocDispRolUsuarioProy.Add(tipoDocDisp);
                    if (mEntityContext.TipoDocDispRolUsuarioProy.FirstOrDefault(onto => onto.OrganizacionID.Equals(tipoDocDisp.OrganizacionID) && onto.ProyectoID.Equals(tipoDocDisp.ProyectoID) && onto.TipoDocumento.Equals(tipoDocDisp.TipoDocumento) && onto.RolUsuario.Equals(tipoDocDisp.RolUsuario)) == null)
                    {
                        mEntityContext.TipoDocDispRolUsuarioProy.Add(tipoDocDisp);
                    }

                }
            }
        }

        #endregion

        #region Invalidar Cache

        public void InvalidarCache()
        {
            //Invalidamos la cache de los niveles de certificación
            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyectoCL.InvalidarNivelesCertificacionRecursosProyecto(ProyectoSeleccionado.Clave);

            new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication).EditarPoliticaCertificacionModeloBase(ProyectoSeleccionado.Clave, PrioridadBase.Alta);

            proyectoCL.InvalidarTiposDocumentosPermitidosUsuarioEnProyecto(ProyectoSeleccionado.Clave, TipoRolUsuario.Administrador);
            proyectoCL.InvalidarTiposDocumentosPermitidosUsuarioEnProyecto(ProyectoSeleccionado.Clave, TipoRolUsuario.Supervisor);
            proyectoCL.InvalidarTiposDocumentosPermitidosUsuarioEnProyecto(ProyectoSeleccionado.Clave, TipoRolUsuario.Usuario);
            proyectoCL.Dispose();

            ParametroGeneralCL paramCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            paramCL.InvalidarCacheParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);

            DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            docCL.InvalidarOntologiasProyecto(ProyectoSeleccionado.Clave);
        }

        #endregion

        #region Propiedades

        private DataWrapperDocumentacion DataWrapperDocumentacion
        {
            get
            {
                if (mDataWrapperDocumentacion == null)
                {
                    DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mDataWrapperDocumentacion = new DataWrapperDocumentacion();

                    Guid proyectoIDPatronOntologias = Guid.Empty;
                    if (ParametroProyecto.ContainsKey("ProyectoIDPatronOntologias"))
                    {
                        Guid.TryParse(ParametroProyecto["ProyectoIDPatronOntologias"], out proyectoIDPatronOntologias);
                        documentacionCN.ObtenerOntologiasProyecto(proyectoIDPatronOntologias, mDataWrapperDocumentacion, false, true, true);
                    }

                    documentacionCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, mDataWrapperDocumentacion, true, true, false);
                    documentacionCN.Dispose();
                }
                return mDataWrapperDocumentacion;
            }
        }


        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        public Dictionary<string, string> ParametroProyecto
        {
            get
            {
                if (mParametroProyecto == null)
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    mParametroProyecto = proyectoCL.ObtenerParametrosProyecto(ProyectoSeleccionado.Clave);
                    proyectoCL.Dispose();
                }

                return mParametroProyecto;
            }
        }

        private DataWrapperProyecto DataWrapperProyecto
        {
            get
            {
                if (mDataWrapperProyecto == null)
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mDataWrapperProyecto = proyCN.ObtenerProyectoPorIDConNiveles(ProyectoSeleccionado.Clave);
                    proyCN.Dispose();
                }
                return mDataWrapperProyecto;
            }
        }

        //private ParametroGeneralDS.ParametroGeneralRow FilaParametrosGenerales
        private ParametroGeneral FilaParametrosGenerales
        {
            get
            {
                if (mFilaParametrosGenerales == null)
                {
                    // mFilaParametrosGenerales = ParametrosGeneralesDS.ParametroGeneral.FindByOrganizacionIDProyectoID(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
                    mFilaParametrosGenerales = new ParametroGeneral();
                    mFilaParametrosGenerales = ParametrosGeneralesDS.ListaParametroGeneral.Find(parametroGeneral => parametroGeneral.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && parametroGeneral.ProyectoID.Equals(ProyectoSeleccionado.Clave));
                }
                return mFilaParametrosGenerales;
            }
        }

        /// <summary>
        /// Obtiene el dataset de parámetros generales
        /// </summary>
        private GestorParametroGeneral ParametrosGeneralesDS
        {
            get
            {
                if (mParametrosGeneralesDS == null)
                {
                    // ParametroGeneralCN paramCN = new ParametroGeneralCN();
                    ParametroGeneralGBD controllerParametrosGeneral = new ParametroGeneralGBD(mEntityContext);
                    mParametrosGeneralesDS = new GestorParametroGeneral();
                    mParametrosGeneralesDS = controllerParametrosGeneral.ObtenerParametrosGeneralesDeProyecto(mParametrosGeneralesDS, ProyectoSeleccionado.Clave);
                    // mParametrosGeneralesDS = paramCN.ObtenerParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);
                    // paramCN.Dispose();
                }
                return mParametrosGeneralesDS;
            }
        }


        #endregion
    }
}
