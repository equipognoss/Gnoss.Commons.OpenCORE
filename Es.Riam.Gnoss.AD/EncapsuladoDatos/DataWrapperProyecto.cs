using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperProyecto : DataWrapperBase
    {
		public DataWrapperProyecto()
        {
            ListaAdministradorGrupoProyecto = new List<AdministradorGrupoProyecto>();
            ListaAdministradorProyecto = new List<AdministradorProyecto>();
            ListaNivelCertificacion = new List<NivelCertificacion>();
            ListaPresentacionListadoSemantico = new List<PresentacionListadoSemantico>();
            ListaPresentacionMapaSemantico = new List<PresentacionMapaSemantico>();
            ListaPresentacionMosaicoSemantico = new List<PresentacionMosaicoSemantico>();
            ListaPresentacionPersonalizadoSemantico = new List<PresentacionPersonalizadoSemantico>();
            ListaPresentacionPestanyaListadoSemantico = new List<PresentacionPestanyaListadoSemantico>();
            ListaPresentacionPestanyaMapaSemantico = new List<PresentacionPestanyaMapaSemantico>();
            ListaPresentacionPestanyaMosaicoSemantico = new List<PresentacionPestanyaMosaicoSemantico>();
            ListaProyecto = new List<Proyecto>();
            ListaProyectoAgCatTesauro = new List<ProyectoAgCatTesauro>();
            ListaProyectoCerradoTmp = new List<ProyectoCerradoTmp>();
            ListaProyectoCerrandose = new List<ProyectoCerrandose>();
            ListaProyectoConfigExtraSem = new List<ProyectoConfigExtraSem>();
            ListaProyectoGadget = new List<ProyectoGadget>();
            ListaProyectoGadgetContexto = new List<ProyectoGadgetContexto>();
            ListaProyectoGadgetContextoHTMLplano = new List<ProyectoGadgetContextoHTMLplano>();
            ListaProyectoGadgetIdioma = new List<ProyectoGadgetIdioma>();
            ListaProyectoGrafoFichaRec = new List<ProyectoGrafoFichaRec>();
            ListaProyectoLoginConfiguracion = new List<ProyectoLoginConfiguracion>();
            ListaProyectoPaginaHtml = new List<ProyectoPaginaHtml>();
            ListaProyectoPasoRegistro = new List<ProyectoPasoRegistro>();
            ListaProyectoPerfilNumElem = new List<ProyectoPerfilNumElem>();
            ListaProyectoPestanya = new List<ProyectoPestanya>();
            ListaProyectoPestanyaBusqueda = new List<ProyectoPestanyaBusqueda>();
            ListaProyectoPestanyaBusquedaExportacion = new List<ProyectoPestanyaBusquedaExportacion>();
            ListaProyectoPestanyaBusquedaExportacionExterna = new List<ProyectoPestanyaBusquedaExportacionExterna>();
            ListaProyectoPestanyaBusquedaExportacionPropiedad = new List<ProyectoPestanyaBusquedaExportacionPropiedad>();
            ListaProyectoPestanyaCMS = new List<ProyectoPestanyaCMS>();
            ListaProyectoPestanyaDashboardAsistente = new List<ProyectoPestanyaDashboardAsistente>();
            ListaProyectoPestanyaDashboardAsistenteDataset = new List<ProyectoPestanyaDashboardAsistenteDataset>();
            ListaProyectoPestanyaExportacionBusqueda = new List<ProyectoPestanyaExportacionBusqueda>();
            ListaProyectoPestanyaFiltroOrdenRecursos = new List<ProyectoPestanyaFiltroOrdenRecursos>();
            ListaProyectoPestanyaMenu = new List<ProyectoPestanyaMenu>();
            ListaProyectoPestanyaMenuRolGrupoIdentidades = new List<ProyectoPestanyaMenuRolGrupoIdentidades>();
            ListaProyectoPestanyaMenuRolIdentidad = new List<ProyectoPestanyaMenuRolIdentidad>();
            ListaProyectoPestanyaRolGrupoIdentidades = new List<ProyectoPestanyaRolGrupoIdentidades>();
            ListaProyectoPestanyaRolIdentidad = new List<ProyectoPestanyaRolIdentidad>();
            ListaProyectoRelacionado = new List<ProyectoRelacionado>();
            ListaProyectoSearchPersonalizado = new List<ProyectoSearchPersonalizado>();
            ListaProyectoServicioExterno = new List<ProyectoServicioExterno>();
            ListaProyectosMasActivos = new List<ProyectosMasActivos>();
            ListaProyTipoRecNoActivReciente = new List<ProyTipoRecNoActivReciente>();
            ListaRecursosRelacionadosPresentacion = new List<RecursosRelacionadosPresentacion>();
            ListaSeccionProyCatalogo = new List<SeccionProyCatalogo>();
            ListaTipoOntoDispRolUsuarioProy = new List<TipoOntoDispRolUsuarioProy>();
            ListaTipoDocDispRolUsuarioProy = new List<TipoDocDispRolUsuarioProy>();
            ListaDatoExtraProyecto = new List<DatoExtraProyecto>();
            ListaCamposRegistroProyectoGenericos = new List<CamposRegistroProyectoGenericos>();
            ListaDatoExtraEcosistema = new List<DatoExtraEcosistema>();
            ListaDatoExtraEcosistemaOpcion = new List<DatoExtraEcosistemaOpcion>();
            ListaDatoExtraEcosistemaVirtuoso = new List<DatoExtraEcosistemaVirtuoso>();
            ListaDatoExtraProyectoOpcion = new List<DatoExtraProyectoOpcion>();
            ListaAccionesExternasProyecto = new List<AccionesExternasProyecto>();
            ListaPreferenciaProyecto = new List<PreferenciaProyecto>();
            ListaProyectoEventoParticipante = new List<ProyectoEventoParticipante>();
            ListaOntologiaProyecto = new List<OntologiaProyecto>();
            ListaConfigAutocompletarProy = new List<ConfigAutocompletarProy>();
            ListaProyectoEventoAccion = new List<ProyectoEventoAccion>();
            ListaProyectoConAdministrado = new List<ProyectoConAdministrado>();
            ListaDatoExtraProyectoOpcionIdentidad = new List<DatoExtraProyectoOpcionIdentidad>();
            ListaDatoExtraProyectoVirtuoso = new List<DatoExtraProyectoVirtuoso>();
            ListaProyectoEvento = new List<ProyectoEvento>();
            ListaProyectoNumConexiones = new List<ProyectoNumConexiones>();
            ListaVistaVirtualProyecto = new List<VistaVirtualProyecto>();
            ListaProyectoPestanyaBusquedaPesoOC = new List<ProyectoPestanyaBusquedaPesoOC>();
            ListaFacetaObjetoConocimientoProyectoPestanya = new List<FacetaObjetoConocimientoProyectoPestanya>();

		}
        public List<FacetaObjetoConocimientoProyectoPestanya> ListaFacetaObjetoConocimientoProyectoPestanya { get; set; }
       

		public List<ProyectoNumConexiones> ListaProyectoNumConexiones { get; set; }

        public List<DatoExtraProyectoOpcionIdentidad> ListaDatoExtraProyectoOpcionIdentidad { get; set; }

        public List<ProyectoConAdministrado> ListaProyectoConAdministrado { get; set; }

        public List<ConfigAutocompletarProy> ListaConfigAutocompletarProy { get; set; }

        public List<OntologiaProyecto> ListaOntologiaProyecto { get; set; }

        public List<ProyectoEventoParticipante> ListaProyectoEventoParticipante { get; set; }

        public List<ProyectoEvento> ListaProyectoEvento { get; set; }

        public List<PreferenciaProyecto> ListaPreferenciaProyecto { get; set; }

        public List<AccionesExternasProyecto> ListaAccionesExternasProyecto { get; set; }

        public List<DatoExtraProyectoVirtuoso> ListaDatoExtraProyectoVirtuoso { get; set; }

        public List<DatoExtraEcosistemaVirtuoso> ListaDatoExtraEcosistemaVirtuoso { get; set; }

        public List<DatoExtraEcosistemaOpcion> ListaDatoExtraEcosistemaOpcion { get; set; }

        public List<DatoExtraEcosistema> ListaDatoExtraEcosistema { get; set; }

        public List<CamposRegistroProyectoGenericos> ListaCamposRegistroProyectoGenericos { get; set; }

        public List<DatoExtraProyecto> ListaDatoExtraProyecto { get; set; }

        public List<TipoOntoDispRolUsuarioProy> ListaTipoOntoDispRolUsuarioProy { get; set; }

        public List<TipoDocDispRolUsuarioProy> ListaTipoDocDispRolUsuarioProy { get; set; }

        public List<AdministradorGrupoProyecto> ListaAdministradorGrupoProyecto { get; set; }

        public List<AdministradorProyecto> ListaAdministradorProyecto { get; set; }

        public List<NivelCertificacion> ListaNivelCertificacion { get; set; }

        public List<PresentacionListadoSemantico> ListaPresentacionListadoSemantico { get; set; }

        public List<PresentacionMapaSemantico> ListaPresentacionMapaSemantico { get; set; }

        public List<PresentacionMosaicoSemantico> ListaPresentacionMosaicoSemantico { get; set; }

        public List<PresentacionPersonalizadoSemantico> ListaPresentacionPersonalizadoSemantico { get; set; }

        public List<PresentacionPestanyaListadoSemantico> ListaPresentacionPestanyaListadoSemantico { get; set; }

        public List<PresentacionPestanyaMapaSemantico> ListaPresentacionPestanyaMapaSemantico { get; set; }

        public List<PresentacionPestanyaMosaicoSemantico> ListaPresentacionPestanyaMosaicoSemantico { get; set; }

        public List<Proyecto> ListaProyecto { get; set; }

        public List<ProyectoAgCatTesauro> ListaProyectoAgCatTesauro { get; set; }

        public List<ProyectoCerradoTmp> ListaProyectoCerradoTmp { get; set; }

        public List<ProyectoCerrandose> ListaProyectoCerrandose { get; set; }

        public List<ProyectoConfigExtraSem> ListaProyectoConfigExtraSem { get; set; }

        public List<ProyectoGadget> ListaProyectoGadget { get; set; }

        public List<ProyectoGadgetContexto> ListaProyectoGadgetContexto { get; set; }

        public List<ProyectoGadgetContextoHTMLplano> ListaProyectoGadgetContextoHTMLplano { get; set; }

        public List<ProyectoGadgetIdioma> ListaProyectoGadgetIdioma { get; set; }

        public List<ProyectoGrafoFichaRec> ListaProyectoGrafoFichaRec { get; set; }

        public List<ProyectoLoginConfiguracion> ListaProyectoLoginConfiguracion { get; set; }

        public List<ProyectoPaginaHtml> ListaProyectoPaginaHtml { get; set; }

        public List<ProyectoPasoRegistro> ListaProyectoPasoRegistro { get; set; }

        public List<ProyectoPerfilNumElem> ListaProyectoPerfilNumElem { get; set; }

        public List<ProyectoPestanya> ListaProyectoPestanya { get; set; }

        public List<ProyectoPestanyaBusqueda> ListaProyectoPestanyaBusqueda { get; set; }

        public List<ProyectoPestanyaBusquedaExportacion> ListaProyectoPestanyaBusquedaExportacion { get; set; }

        public List<ProyectoPestanyaBusquedaExportacionExterna> ListaProyectoPestanyaBusquedaExportacionExterna { get; set; }

        public List<ProyectoPestanyaBusquedaExportacionPropiedad> ListaProyectoPestanyaBusquedaExportacionPropiedad { get; set; }

        public List<ProyectoPestanyaCMS> ListaProyectoPestanyaCMS { get; set; }

        public List<ProyectoPestanyaExportacionBusqueda> ListaProyectoPestanyaExportacionBusqueda { get; set; }

        public List<ProyectoPestanyaFiltroOrdenRecursos> ListaProyectoPestanyaFiltroOrdenRecursos { get; set; }       

        public List<ProyectoPestanyaMenu> ListaProyectoPestanyaMenu { get; set; }

        public List<ProyectoPestanyaMenuRolGrupoIdentidades> ListaProyectoPestanyaMenuRolGrupoIdentidades { get; set; }

        public List<ProyectoPestanyaMenuRolIdentidad> ListaProyectoPestanyaMenuRolIdentidad { get; set; }

        public List<ProyectoPestanyaRolGrupoIdentidades> ListaProyectoPestanyaRolGrupoIdentidades { get; set; }

        public List<ProyectoPestanyaRolIdentidad> ListaProyectoPestanyaRolIdentidad { get; set; }

        public List<ProyectoRelacionado> ListaProyectoRelacionado { get; set; }

        public List<ProyectoSearchPersonalizado> ListaProyectoSearchPersonalizado { get; set; }

        public List<ProyectoServicioExterno> ListaProyectoServicioExterno { get; set; }

        public List<ProyectosMasActivos> ListaProyectosMasActivos { get; set; }

        public List<ProyTipoRecNoActivReciente> ListaProyTipoRecNoActivReciente { get; set; }

        public List<VistaVirtualProyecto> ListaVistaVirtualProyecto { get; set; }

        public List<RecursosRelacionadosPresentacion> ListaRecursosRelacionadosPresentacion { get; set; }

        public List<SeccionProyCatalogo> ListaSeccionProyCatalogo { get; set; }

        public List<DatoExtraProyectoOpcion> ListaDatoExtraProyectoOpcion { get; set; }

        public List<ProyectoEventoAccion> ListaProyectoEventoAccion { get; set; }

        public List<ProyectoPestanyaDashboardAsistente> ListaProyectoPestanyaDashboardAsistente { get; set; }

        public List<ProyectoPestanyaDashboardAsistenteDataset> ListaProyectoPestanyaDashboardAsistenteDataset { get; set; }

        public List<ProyectoPestanyaBusquedaPesoOC> ListaProyectoPestanyaBusquedaPesoOC { get; set; }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperProyecto encapsulado = (DataWrapperProyecto)pDataWrapper;
            ListaAdministradorGrupoProyecto = ListaAdministradorGrupoProyecto.Union(encapsulado.ListaAdministradorGrupoProyecto).ToList();
            ListaAdministradorProyecto = ListaAdministradorProyecto.Union(encapsulado.ListaAdministradorProyecto).ToList();
            ListaNivelCertificacion = ListaNivelCertificacion.Union(encapsulado.ListaNivelCertificacion).ToList();
            ListaPresentacionListadoSemantico = ListaPresentacionListadoSemantico.Union(encapsulado.ListaPresentacionListadoSemantico).ToList();
            ListaPresentacionMapaSemantico = ListaPresentacionMapaSemantico.Union(encapsulado.ListaPresentacionMapaSemantico).ToList();
            ListaPresentacionMosaicoSemantico = ListaPresentacionMosaicoSemantico.Union(encapsulado.ListaPresentacionMosaicoSemantico).ToList();
            ListaPresentacionPersonalizadoSemantico = ListaPresentacionPersonalizadoSemantico.Union(encapsulado.ListaPresentacionPersonalizadoSemantico).ToList();
            ListaPresentacionPestanyaListadoSemantico = ListaPresentacionPestanyaListadoSemantico.Union(encapsulado.ListaPresentacionPestanyaListadoSemantico).ToList();
            ListaPresentacionPestanyaMapaSemantico = ListaPresentacionPestanyaMapaSemantico.Union(encapsulado.ListaPresentacionPestanyaMapaSemantico).ToList();
            ListaPresentacionPestanyaMosaicoSemantico = ListaPresentacionPestanyaMosaicoSemantico.Union(encapsulado.ListaPresentacionPestanyaMosaicoSemantico).ToList();
            ListaProyecto = ListaProyecto.Union(encapsulado.ListaProyecto).ToList();
            ListaProyectoAgCatTesauro = ListaProyectoAgCatTesauro.Union(encapsulado.ListaProyectoAgCatTesauro).ToList();
            ListaProyectoCerradoTmp = ListaProyectoCerradoTmp.Union(encapsulado.ListaProyectoCerradoTmp).ToList();
            ListaProyectoCerrandose = ListaProyectoCerrandose.Union(encapsulado.ListaProyectoCerrandose).ToList();
            ListaProyectoConfigExtraSem = ListaProyectoConfigExtraSem.Union(encapsulado.ListaProyectoConfigExtraSem).ToList();
            ListaProyectoGadget = ListaProyectoGadget.Union(encapsulado.ListaProyectoGadget).ToList();
            ListaProyectoGadgetContexto = ListaProyectoGadgetContexto.Union(encapsulado.ListaProyectoGadgetContexto).ToList();
            ListaProyectoGadgetContextoHTMLplano =  ListaProyectoGadgetContextoHTMLplano.Union(encapsulado.ListaProyectoGadgetContextoHTMLplano).ToList();
            ListaProyectoGadgetIdioma = ListaProyectoGadgetIdioma.Union(encapsulado.ListaProyectoGadgetIdioma).ToList();
            ListaProyectoGrafoFichaRec = ListaProyectoGrafoFichaRec.Union(encapsulado.ListaProyectoGrafoFichaRec).ToList();
            ListaProyectoLoginConfiguracion = ListaProyectoLoginConfiguracion.Union(encapsulado.ListaProyectoLoginConfiguracion).ToList();
            ListaProyectoPaginaHtml = ListaProyectoPaginaHtml.Union(encapsulado.ListaProyectoPaginaHtml).ToList();
            ListaProyectoPasoRegistro = ListaProyectoPasoRegistro.Union(encapsulado.ListaProyectoPasoRegistro).ToList();
            ListaProyectoPerfilNumElem = ListaProyectoPerfilNumElem.Union(encapsulado.ListaProyectoPerfilNumElem).ToList();
            ListaProyectoPestanya = ListaProyectoPestanya.Union(encapsulado.ListaProyectoPestanya).ToList();
            ListaProyectoPestanyaBusqueda = ListaProyectoPestanyaBusqueda.Union(encapsulado.ListaProyectoPestanyaBusqueda).ToList();
            ListaProyectoPestanyaBusquedaExportacion = ListaProyectoPestanyaBusquedaExportacion.Union(encapsulado.ListaProyectoPestanyaBusquedaExportacion).ToList();
            ListaProyectoPestanyaBusquedaExportacionExterna = ListaProyectoPestanyaBusquedaExportacionExterna.Union(encapsulado.ListaProyectoPestanyaBusquedaExportacionExterna).ToList();
            ListaProyectoPestanyaBusquedaExportacionPropiedad = ListaProyectoPestanyaBusquedaExportacionPropiedad.Union(encapsulado.ListaProyectoPestanyaBusquedaExportacionPropiedad).ToList();
            ListaProyectoPestanyaCMS = ListaProyectoPestanyaCMS.Union(encapsulado.ListaProyectoPestanyaCMS).ToList();
            ListaProyectoPestanyaDashboardAsistente = ListaProyectoPestanyaDashboardAsistente.Union(encapsulado.ListaProyectoPestanyaDashboardAsistente).ToList();
            ListaProyectoPestanyaDashboardAsistenteDataset = ListaProyectoPestanyaDashboardAsistenteDataset.Union(encapsulado.ListaProyectoPestanyaDashboardAsistenteDataset).ToList();
            ListaProyectoPestanyaExportacionBusqueda = ListaProyectoPestanyaExportacionBusqueda.Union(encapsulado.ListaProyectoPestanyaExportacionBusqueda).ToList();
            ListaProyectoPestanyaFiltroOrdenRecursos = ListaProyectoPestanyaFiltroOrdenRecursos.Union(encapsulado.ListaProyectoPestanyaFiltroOrdenRecursos).ToList();
            ListaProyectoPestanyaMenu = ListaProyectoPestanyaMenu.Union(encapsulado.ListaProyectoPestanyaMenu).ToList();
            ListaProyectoPestanyaMenuRolGrupoIdentidades = ListaProyectoPestanyaMenuRolGrupoIdentidades.Union(encapsulado.ListaProyectoPestanyaMenuRolGrupoIdentidades).ToList();
            ListaProyectoPestanyaMenuRolIdentidad = ListaProyectoPestanyaMenuRolIdentidad.Union(encapsulado.ListaProyectoPestanyaMenuRolIdentidad).ToList();
            ListaProyectoPestanyaRolGrupoIdentidades = ListaProyectoPestanyaRolGrupoIdentidades.Union(encapsulado.ListaProyectoPestanyaRolGrupoIdentidades).ToList();
            ListaProyectoPestanyaRolIdentidad = ListaProyectoPestanyaRolIdentidad.Union(encapsulado.ListaProyectoPestanyaRolIdentidad).ToList();
            ListaProyectoRelacionado = ListaProyectoRelacionado.Union(encapsulado.ListaProyectoRelacionado).ToList();
            ListaProyectoSearchPersonalizado = ListaProyectoSearchPersonalizado.Union(encapsulado.ListaProyectoSearchPersonalizado).ToList();
            ListaProyectoServicioExterno = ListaProyectoServicioExterno.Union(encapsulado.ListaProyectoServicioExterno).ToList();
            ListaProyectosMasActivos = ListaProyectosMasActivos.Union(encapsulado.ListaProyectosMasActivos).ToList();
            ListaProyTipoRecNoActivReciente = ListaProyTipoRecNoActivReciente.Union(encapsulado.ListaProyTipoRecNoActivReciente).ToList();
            ListaDatoExtraProyecto = ListaDatoExtraProyecto.Union(encapsulado.ListaDatoExtraProyecto).ToList();
            ListaCamposRegistroProyectoGenericos = ListaCamposRegistroProyectoGenericos.Union(encapsulado.ListaCamposRegistroProyectoGenericos).ToList();
            ListaDatoExtraEcosistema = ListaDatoExtraEcosistema.Union(encapsulado.ListaDatoExtraEcosistema).ToList();
            ListaDatoExtraEcosistemaOpcion = ListaDatoExtraEcosistemaOpcion.Union(encapsulado.ListaDatoExtraEcosistemaOpcion).ToList();
            ListaDatoExtraEcosistemaVirtuoso = ListaDatoExtraEcosistemaVirtuoso.Union(encapsulado.ListaDatoExtraEcosistemaVirtuoso).ToList();
            ListaDatoExtraProyectoOpcion = ListaDatoExtraProyectoOpcion.Union(encapsulado.ListaDatoExtraProyectoOpcion).ToList();
            ListaDatoExtraProyectoVirtuoso = ListaDatoExtraProyectoVirtuoso.Union(encapsulado.ListaDatoExtraProyectoVirtuoso).ToList();
            ListaAccionesExternasProyecto = ListaAccionesExternasProyecto.Union(encapsulado.ListaAccionesExternasProyecto).ToList();
            ListaPreferenciaProyecto = ListaPreferenciaProyecto.Union(encapsulado.ListaPreferenciaProyecto).ToList();
            ListaProyectoEvento = ListaProyectoEvento.Union(encapsulado.ListaProyectoEvento).ToList();
            ListaProyectoEventoParticipante = ListaProyectoEventoParticipante.Union(encapsulado.ListaProyectoEventoParticipante).ToList();
            ListaOntologiaProyecto = ListaOntologiaProyecto.Union(encapsulado.ListaOntologiaProyecto).ToList();
            ListaConfigAutocompletarProy = ListaConfigAutocompletarProy.Union(encapsulado.ListaConfigAutocompletarProy).ToList();
            ListaProyectoEventoAccion = ListaProyectoEventoAccion.Union(encapsulado.ListaProyectoEventoAccion).ToList();
            ListaProyectoConAdministrado = ListaProyectoConAdministrado.Union(encapsulado.ListaProyectoConAdministrado).ToList();
            ListaDatoExtraProyectoOpcionIdentidad = ListaDatoExtraProyectoOpcionIdentidad.Union(encapsulado.ListaDatoExtraProyectoOpcionIdentidad).ToList();
            ListaProyectoNumConexiones = ListaProyectoNumConexiones.Union(encapsulado.ListaProyectoNumConexiones).ToList();
            ListaRecursosRelacionadosPresentacion = ListaRecursosRelacionadosPresentacion.Union(encapsulado.ListaRecursosRelacionadosPresentacion).ToList();
			ListaVistaVirtualProyecto = ListaVistaVirtualProyecto.Union(encapsulado.ListaVistaVirtualProyecto).ToList();
        }

        public void AddProyecto(Proyecto pProyectoNuevo)
        {
            ListaProyecto.Add(pProyectoNuevo);
        }

        public void DeleteProyectoCerradoTmp(ProyectoCerradoTmp proyectoCerradoTmp)
        {
            ListaProyectoCerradoTmp.Remove(proyectoCerradoTmp);
        }

        public void DeleteProyectoCerrandose(ProyectoCerrandose proyectoCerrandose)
        {
            ListaProyectoCerrandose.Remove(proyectoCerrandose);
        }

        public void AddAdministradorProyecto(AdministradorProyecto filaAdminPro)
        {
            ListaAdministradorProyecto.Add(filaAdminPro);
        }

        public void DeleteAdministradorProyecto(AdministradorProyecto administradorProyecto)
        {
            ListaAdministradorProyecto.Remove(administradorProyecto);
        }


        public ProyectoPestanyaMenu AddProyectoPestanyaMenuRow(Guid pestanyaId, Guid organizacionID, Guid proyectoID, ProyectoPestanyaMenu p1, short tipoPestanya, string nombre, string ruta, short orden, bool nuevaPestanya, bool visible, short privacidad, string htmlAlternativo, string idiomasDisponibles, string titulo, string nombreCortoPestanya, bool visibleSinAcceso, string cssBodyClass, string metaDescription, bool activa, string ultimoEditor)
        {
            ProyectoPestanyaMenu proyectoPestanyaMenu = new ProyectoPestanyaMenu(pestanyaId, organizacionID, proyectoID, p1, tipoPestanya, nombre, ruta, orden, nuevaPestanya, visible, privacidad, htmlAlternativo, idiomasDisponibles, titulo, nombreCortoPestanya, visibleSinAcceso, cssBodyClass, metaDescription, activa, ultimoEditor);
            ListaProyectoPestanyaMenu.Add(proyectoPestanyaMenu);
            return proyectoPestanyaMenu;
        }

        public void AddProyectoGadgetRow(Guid organizacionID, Guid proyectoID, Guid gadgetID, string titulo, string contenido, int orden, short tipo, string ubicacion, string clases, short tipoUbicacion, bool visible, bool multiIdioma, Guid personalizacionComponenteID, bool cargarPorAjax, string comunidadDestinoFiltros, string nombreCorto)
        {
            ProyectoGadget proyectoGadget = new ProyectoGadget(organizacionID, proyectoID, gadgetID, titulo, contenido, orden, tipo, ubicacion, clases, tipoUbicacion, visible, multiIdioma, personalizacionComponenteID, cargarPorAjax, comunidadDestinoFiltros, nombreCorto);
            proyectoGadget.PersonalizacionComponenteID = null;            
            ListaProyectoGadget.Add(proyectoGadget);
        }

        /// <summary>
        /// Carga las relaciones perezosas entre los objetos de Entity Framework para evitar problemas de referencias nulas al acceder a dichas relaciones desde la capa de presentación.
        /// </summary>
        public void CargaRelacionesPerezosasCache()
        {
            CargarRelacionesPerezosasCacheAdministradorProyecto();
            CargarRelacionesPerezosasCacheProyecto();
            CargarRelacionesPerezosasCacheProyectoEventoParticipante();
            CargarRelacionesPerezosasCacheProyectoGadget();
            CargarRelacionesPerezosasCacheProyectoPestanyaBusqueda();
            CargarRelacionesPerezosasCacheProyectoPestanyaCMS();
            CargarRelacionesPerezosasCacheFacetaObjetoConocimientoProyectoPestanya();
            CargarRelacionesPerezosasCacheProyectoPestanyaMenu();
        }

        /// <summary>
        /// Carga las relaciones perezosas necesarias para la tabla AdministradorProyecto
        /// </summary>
        private void CargarRelacionesPerezosasCacheAdministradorProyecto()
        {
            foreach (AdministradorProyecto adminProy in ListaAdministradorProyecto)
            {
                adminProy.Proyecto = ListaProyecto.FirstOrDefault(proy => proy.OrganizacionID.Equals(adminProy.OrganizacionID) && proy.ProyectoID.Equals(adminProy.ProyectoID));
            }
        }

        /// <summary>
        /// Carga las relaciones perezosas necesarias para la tabla Proyecto
        /// </summary>
        private void CargarRelacionesPerezosasCacheProyecto()
        {
            foreach (Proyecto proy in ListaProyecto)
            {
                proy.AdministradorProyecto = ListaAdministradorProyecto.Where(adminProy => adminProy.OrganizacionID.Equals(proy.OrganizacionID) && adminProy.ProyectoID.Equals(proy.ProyectoID)).ToList();

                proy.ProyectoAgCatTesauro = ListaProyectoAgCatTesauro.Where(agCat => agCat.OrganizacionID.Equals(proy.OrganizacionID) && agCat.ProyectoID.Equals(proy.ProyectoID)).ToList();

                proy.ProyectoCerradoTmp = ListaProyectoCerradoTmp.FirstOrDefault(cerradoTmp => cerradoTmp.ProyectoID.Equals(proy.ProyectoID) && cerradoTmp.OrganizacionID.Equals(proy.OrganizacionID));

                proy.ProyectoCerrandose = ListaProyectoCerrandose.FirstOrDefault(cerrandose => cerrandose.ProyectoID.Equals(proy.ProyectoID) && cerrandose.OrganizacionID.Equals(proy.OrganizacionID));

                proy.ProyectosMasActivos = ListaProyectosMasActivos.FirstOrDefault(activo => activo.ProyectoID.Equals(proy.ProyectoID) && activo.OrganizacionID.Equals(proy.OrganizacionID));
            }
        }

        /// <summary>
        /// Carga las relaciones perezosas necesarias para la tabla ProyectoEventoParticipante
        /// </summary>
        private void CargarRelacionesPerezosasCacheProyectoEventoParticipante()
        {
            foreach (ProyectoEventoParticipante proyEvP in ListaProyectoEventoParticipante)
            {
                proyEvP.ProyectoEvento = ListaProyectoEvento.FirstOrDefault(proyEven => proyEven.EventoID.Equals(proyEvP.EventoID));
            }
        }

        /// <summary>
        /// Carga las relaciones perezosas necesarias para la tabla ProyectoGadget
        /// </summary>
        private void CargarRelacionesPerezosasCacheProyectoGadget()
        {
            foreach (ProyectoGadget proyG in ListaProyectoGadget)
            {
                proyG.ProyectoGadgetIdioma = ListaProyectoGadgetIdioma.Where(proyGI => proyGI.OrganizacionID.Equals(proyG.OrganizacionID) && proyGI.ProyectoID.Equals(proyG.ProyectoID) && proyGI.GadgetID.Equals(proyG.GadgetID)).ToList();
                proyG.ProyectoGadgetContexto = ListaProyectoGadgetContexto.FirstOrDefault(proyGI => proyGI.OrganizacionID.Equals(proyG.OrganizacionID) && proyGI.ProyectoID.Equals(proyG.ProyectoID) && proyGI.GadgetID.Equals(proyG.GadgetID));
            }
        }

        /// <summary>
        /// Carga las relaciones perezosas necesarias para la tabla ProyectoPestanyaBusqueda
        /// </summary>
        private void CargarRelacionesPerezosasCacheProyectoPestanyaBusqueda()
        {
            foreach (ProyectoPestanyaBusqueda proyPB in ListaProyectoPestanyaBusqueda)
            {
                proyPB.ProyectoPestanyaMenu = ListaProyectoPestanyaMenu.FirstOrDefault(pestMenu => pestMenu.PestanyaID.Equals(proyPB.PestanyaID));
                proyPB.ProyectoPestanyaBusquedaPesoOC = ListaProyectoPestanyaBusquedaPesoOC.Where(pestPesoOC => pestPesoOC.PestanyaID.Equals(proyPB.PestanyaID)).ToList();
            }
        }

        /// <summary>
        /// Carga las relaciones perezosas necesarias para la tabla ProyectoPestanyaCMS
        /// </summary>
        private void CargarRelacionesPerezosasCacheProyectoPestanyaCMS()
        {
            foreach (ProyectoPestanyaCMS pestCMS in ListaProyectoPestanyaCMS)
            {
                pestCMS.ProyectoPestanyaMenu = ListaProyectoPestanyaMenu.FirstOrDefault(pestMenu => pestMenu.PestanyaID.Equals(pestCMS.PestanyaID));
            }
        }

        /// <summary>
        /// Carga las relaciones perezosas necesarias para la tabla FacetaObjetoConocimientoProyectoPestanya
        /// </summary>
        private void CargarRelacionesPerezosasCacheFacetaObjetoConocimientoProyectoPestanya()
        {
            foreach (FacetaObjetoConocimientoProyectoPestanya facetaProyectoPestanya in ListaFacetaObjetoConocimientoProyectoPestanya)
            {
                facetaProyectoPestanya.ProyectoPestanyaMenu = ListaProyectoPestanyaMenu.FirstOrDefault(pestMenu => pestMenu.PestanyaID.Equals(facetaProyectoPestanya.PestanyaID));
            }
        }

        /// <summary>
        /// Carga las relaciones perezosas necesarias para la tabla ProyectoPestanyaMenu
        /// </summary>
        private void CargarRelacionesPerezosasCacheProyectoPestanyaMenu()
        {
            foreach (ProyectoPestanyaMenu pestanyaMenu in ListaProyectoPestanyaMenu)
            {
                pestanyaMenu.ProyectoPestanyaBusqueda = ListaProyectoPestanyaBusqueda.FirstOrDefault(busqueda => busqueda.PestanyaID.Equals(pestanyaMenu.PestanyaID));
                pestanyaMenu.ProyectoPestanyaCMS = ListaProyectoPestanyaCMS.Where(pestCMS => pestCMS.PestanyaID.Equals(pestanyaMenu.PestanyaID)).ToList();
                pestanyaMenu.ProyectoPestanyaFiltroOrdenRecursos = ListaProyectoPestanyaFiltroOrdenRecursos.Where(filtro => filtro.PestanyaID.Equals(pestanyaMenu.PestanyaID)).ToList();
                pestanyaMenu.ProyectoPestanyaMenu1 = ListaProyectoPestanyaMenu.Where(hijos => hijos.PestanyaPadreID.Equals(pestanyaMenu.PestanyaID)).ToList();
                pestanyaMenu.ProyectoPestanyaMenu2 = ListaProyectoPestanyaMenu.FirstOrDefault(padre => padre.PestanyaID.Equals(pestanyaMenu.PestanyaPadreID));
                pestanyaMenu.ProyectoPestanyaMenuRolGrupoIdentidades = ListaProyectoPestanyaMenuRolGrupoIdentidades.Where(rol => rol.PestanyaID.Equals(pestanyaMenu.PestanyaID)).ToList();
                pestanyaMenu.ProyectoPestanyaMenuRolIdentidad = ListaProyectoPestanyaMenuRolIdentidad.Where(ident => ident.PestanyaID.Equals(pestanyaMenu.PestanyaID)).ToList();
                pestanyaMenu.ConfigAutocompletarProy = ListaConfigAutocompletarProy.Where(config => config.PestanyaID.Equals(pestanyaMenu.PestanyaID)).ToList();
                pestanyaMenu.FacetaObjetoConocimientoProyectoPestanya = ListaFacetaObjetoConocimientoProyectoPestanya.Where(faceta => faceta.PestanyaID.Equals(pestanyaMenu.PestanyaID)).ToList();
            }
        }
    }
}