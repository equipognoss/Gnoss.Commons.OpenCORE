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
        private List<AdministradorGrupoProyecto> listaAdministradorGrupoProyecto;
        private List<AdministradorProyecto> listaAdministradorProyecto;
        private List<NivelCertificacion> listaNivelCertificacion;
        private List<PresentacionListadoSemantico> listaPresentacionListadoSemantico;
        private List<PresentacionMapaSemantico> listaPresentacionMapaSemantico;
        private List<PresentacionMosaicoSemantico> listaPresentacionMosaicoSemantico;
        private List<PresentacionPersonalizadoSemantico> listaPresentacionPersonalizadoSemantico;
        private List<PresentacionPestanyaListadoSemantico> listaPresentacionPestanyaListadoSemantico;
        private List<PresentacionPestanyaMapaSemantico> listaPresentacionPestanyaMapaSemantico;
        private List<PresentacionPestanyaMosaicoSemantico> listaPresentacionPestanyaMosaicoSemantico;
        private List<Proyecto> listaProyecto;
        private List<ProyectoAgCatTesauro> listaProyectoAgCatTesauro;
        private List<ProyectoCerradoTmp> listaProyectoCerradoTmp;
        private List<ProyectoCerrandose> listaProyectoCerrandose;
        private List<ProyectoConfigExtraSem> listaProyectoConfigExtraSem;
        private List<ProyectoGadget> listaProyectoGadget;
        private List<ProyectoGadgetContexto> listaProyectoGadgetContexto;
        private List<ProyectoGadgetContextoHTMLplano> listaProyectoGadgetContextoHTMLplano;
        private List<ProyectoGadgetIdioma> listaProyectoGadgetIdioma;
        private List<ProyectoGrafoFichaRec> listaProyectoGrafoFichaRec;
        private List<ProyectoLoginConfiguracion> listaProyectoLoginConfiguracion;
        private List<ProyectoPaginaHtml> listaProyectoPaginaHtml;
        private List<ProyectoPasoRegistro> listaProyectoPasoRegistro;
        private List<ProyectoPerfilNumElem> listaProyectoPerfilNumElem;
        private List<ProyectoPestanya> listaProyectoPestanya;
        private List<ProyectoPestanyaBusqueda> listaProyectoPestanyaBusqueda;
        private List<ProyectoPestanyaBusquedaExportacion> listaProyectoPestanyaBusquedaExportacion;
        private List<ProyectoPestanyaBusquedaExportacionExterna> listaProyectoPestanyaBusquedaExportacionExterna;
        private List<ProyectoPestanyaBusquedaExportacionPropiedad> listaProyectoPestanyaBusquedaExportacionPropiedad;
        private List<ProyectoPestanyaCMS> listaProyectoPestanyaCMS;
        private List<ProyectoPestanyaExportacionBusqueda> listaProyectoPestanyaExportacionBusqueda;
        private List<ProyectoPestanyaFiltroOrdenRecursos> listaProyectoPestanyaFiltroOrdenRecursos;
        private List<ProyectoPestanyaMenu> listaProyectoPestanyaMenu;
        private List<ProyectoPestanyaMenuRolGrupoIdentidades> listaProyectoPestanyaMenuRolGrupoIdentidades;
        private List<ProyectoPestanyaMenuRolIdentidad> listaProyectoPestanyaMenuRolIdentidad;
        private List<ProyectoPestanyaRolGrupoIdentidades> listaProyectoPestanyaRolGrupoIdentidades;
        private List<ProyectoPestanyaRolIdentidad> listaProyectoPestanyaRolIdentidad;
        private List<ProyectoRelacionado> listaProyectoRelacionado;
        private List<ProyectoSearchPersonalizado> listaProyectoSearchPersonalizado;
        private List<ProyectoServicioExterno> listaProyectoServicioExterno;
        private List<ProyectosMasActivos> listaProyectosMasActivos;
        private List<ProyTipoRecNoActivReciente> listaProyTipoRecNoActivReciente;
        private List<VistaVirtualProyecto> listaVistaVirtualProyecto;
        private List<SeccionProyCatalogo> listaSeccionProyCatalogo;
        private List<TipoOntoDispRolUsuarioProy> listaTipoOntoDispRolUsuarioProy;
        private List<TipoDocDispRolUsuarioProy> listaTipoDocDispRolUsuarioProy;
        private List<RecursosRelacionadosPresentacion> listaRecursosRelacionadosPresentacion { get; set; }

        private List<DatoExtraProyecto> listaDatoExtraProyecto;
        private List<CamposRegistroProyectoGenericos> listaCamposRegistroProyectoGenericos;
        private List<DatoExtraEcosistema> listaDatoExtraEcosistema;
        private List<DatoExtraEcosistemaOpcion> listaDatoExtraEcosistemaOpcion;
        private List<DatoExtraEcosistemaVirtuoso> listaDatoExtraEcosistemaVirtuoso;
        private List<DatoExtraProyectoOpcion> listaDatoExtraProyectoOpcion;
        private List<DatoExtraProyectoVirtuoso> listaDatoExtraProyectoVirtuoso;
        private List<AccionesExternasProyecto> listaAccionesExternasProyecto;
        private List<PreferenciaProyecto> listaPreferenciaProyecto;
        private List<ProyectoEvento> listaProyectoEvento;
        private List<ProyectoEventoParticipante> listaProyectoEventoParticipante;

        private List<OntologiaProyecto> listaOntologiaProyecto;
        private List<ConfigAutocompletarProy> listaConfigAutocompletarProy;
        private List<ProyectoEventoAccion> listaProyectoEventoAccion;
        private List<ProyectoConAdministrado> listaProyectoConAdministrado;
        private List<DatoExtraProyectoOpcionIdentidad> listaDatoExtraProyectoOpcionIdentidad;

        private List<ProyectoNumConexiones> listaProyectoNumConexiones;
        public DataWrapperProyecto()
        {
            this.ListaAdministradorGrupoProyecto = new List<AdministradorGrupoProyecto>();
            this.ListaAdministradorProyecto = new List<AdministradorProyecto>();
            this.ListaNivelCertificacion = new List<NivelCertificacion>();
            this.ListaPresentacionListadoSemantico = new List<PresentacionListadoSemantico>();
            this.ListaPresentacionMapaSemantico = new List<PresentacionMapaSemantico>();
            this.ListaPresentacionMosaicoSemantico = new List<PresentacionMosaicoSemantico>();
            this.ListaPresentacionPersonalizadoSemantico = new List<PresentacionPersonalizadoSemantico>();
            this.ListaPresentacionPestanyaListadoSemantico = new List<PresentacionPestanyaListadoSemantico>();
            this.ListaPresentacionPestanyaMapaSemantico = new List<PresentacionPestanyaMapaSemantico>();
            this.ListaPresentacionPestanyaMosaicoSemantico = new List<PresentacionPestanyaMosaicoSemantico>();
            this.ListaProyecto = new List<Proyecto>();
            this.ListaProyectoAgCatTesauro = new List<ProyectoAgCatTesauro>();
            this.ListaProyectoCerradoTmp = new List<ProyectoCerradoTmp>();
            this.ListaProyectoCerrandose = new List<ProyectoCerrandose>();
            this.ListaProyectoConfigExtraSem = new List<ProyectoConfigExtraSem>();
            this.ListaProyectoGadget = new List<ProyectoGadget>();
            this.ListaProyectoGadgetContexto = new List<ProyectoGadgetContexto>();
            this.ListaProyectoGadgetContextoHTMLplano = new List<ProyectoGadgetContextoHTMLplano>();
            this.ListaProyectoGadgetIdioma = new List<ProyectoGadgetIdioma>();
            this.ListaProyectoGrafoFichaRec = new List<ProyectoGrafoFichaRec>();
            this.ListaProyectoLoginConfiguracion = new List<ProyectoLoginConfiguracion>();
            this.ListaProyectoPaginaHtml = new List<ProyectoPaginaHtml>();
            this.ListaProyectoPasoRegistro = new List<ProyectoPasoRegistro>();
            this.ListaProyectoPerfilNumElem = new List<ProyectoPerfilNumElem>();
            this.ListaProyectoPestanya = new List<ProyectoPestanya>();
            this.ListaProyectoPestanyaBusqueda = new List<ProyectoPestanyaBusqueda>();
            this.ListaProyectoPestanyaBusquedaExportacion = new List<ProyectoPestanyaBusquedaExportacion>();
            this.ListaProyectoPestanyaBusquedaExportacionExterna = new List<ProyectoPestanyaBusquedaExportacionExterna>();
            this.ListaProyectoPestanyaBusquedaExportacionPropiedad = new List<ProyectoPestanyaBusquedaExportacionPropiedad>();
            this.ListaProyectoPestanyaCMS = new List<ProyectoPestanyaCMS>();
            this.ListaProyectoPestanyaExportacionBusqueda = new List<ProyectoPestanyaExportacionBusqueda>();
            this.ListaProyectoPestanyaFiltroOrdenRecursos = new List<ProyectoPestanyaFiltroOrdenRecursos>();
            this.ListaProyectoPestanyaMenu = new List<ProyectoPestanyaMenu>();
            this.ListaProyectoPestanyaMenuRolGrupoIdentidades = new List<ProyectoPestanyaMenuRolGrupoIdentidades>();
            this.ListaProyectoPestanyaMenuRolIdentidad = new List<ProyectoPestanyaMenuRolIdentidad>();
            this.ListaProyectoPestanyaRolGrupoIdentidades = new List<ProyectoPestanyaRolGrupoIdentidades>();
            this.ListaProyectoPestanyaRolIdentidad = new List<ProyectoPestanyaRolIdentidad>();
            this.ListaProyectoRelacionado = new List<ProyectoRelacionado>();
            this.ListaProyectoSearchPersonalizado = new List<ProyectoSearchPersonalizado>();
            this.ListaProyectoServicioExterno = new List<ProyectoServicioExterno>();
            this.ListaProyectosMasActivos = new List<ProyectosMasActivos>();
            this.ListaProyTipoRecNoActivReciente = new List<ProyTipoRecNoActivReciente>();
            this.ListaRecursosRelacionadosPresentacion = new List<RecursosRelacionadosPresentacion>();
            this.ListaSeccionProyCatalogo = new List<SeccionProyCatalogo>();
            this.ListaTipoOntoDispRolUsuarioProy = new List<TipoOntoDispRolUsuarioProy>();
            this.ListaTipoDocDispRolUsuarioProy = new List<TipoDocDispRolUsuarioProy>();
            this.ListaDatoExtraProyecto = new List<DatoExtraProyecto>();
            this.ListaCamposRegistroProyectoGenericos = new List<CamposRegistroProyectoGenericos>();
            this.ListaDatoExtraEcosistema = new List<DatoExtraEcosistema>();
            this.ListaDatoExtraEcosistemaOpcion = new List<DatoExtraEcosistemaOpcion>();
            this.ListaDatoExtraEcosistemaVirtuoso = new List<DatoExtraEcosistemaVirtuoso>();
            this.ListaDatoExtraProyectoOpcion = new List<DatoExtraProyectoOpcion>();
            this.ListaAccionesExternasProyecto = new List<AccionesExternasProyecto>();
            this.ListaPreferenciaProyecto = new List<PreferenciaProyecto>();
            this.listaProyectoEventoParticipante = new List<ProyectoEventoParticipante>();
            this.listaOntologiaProyecto = new List<OntologiaProyecto>();
            this.listaConfigAutocompletarProy = new List<ConfigAutocompletarProy>();
            this.listaProyectoEventoAccion = new List<ProyectoEventoAccion>();
            this.listaProyectoConAdministrado = new List<ProyectoConAdministrado>();
            this.listaDatoExtraProyectoOpcionIdentidad = new List<DatoExtraProyectoOpcionIdentidad>();
            this.listaDatoExtraProyectoVirtuoso = new List<DatoExtraProyectoVirtuoso>();
            this.listaProyectoEvento = new List<ProyectoEvento>();
            this.listaProyectoNumConexiones = new List<ProyectoNumConexiones>();
            this.listaVistaVirtualProyecto = new List<VistaVirtualProyecto>();
        }
        public List<ProyectoNumConexiones> ListaProyectoNumConexiones
        {
            get
            {
                return this.listaProyectoNumConexiones;
            }
            set
            {
                this.listaProyectoNumConexiones = value;
            }
        }

        public List<DatoExtraProyectoOpcionIdentidad> ListaDatoExtraProyectoOpcionIdentidad
        {
            get
            {
                return this.listaDatoExtraProyectoOpcionIdentidad;
            }
            set
            {
                this.listaDatoExtraProyectoOpcionIdentidad = value;
            }
        }
        public List<ProyectoConAdministrado> ListaProyectoConAdministrado
        {
            get
            {
                return this.listaProyectoConAdministrado;
            }
            set
            {
                this.listaProyectoConAdministrado = value;
            }
        }
        public List<ConfigAutocompletarProy> ListaConfigAutocompletarProy
        {
            get
            {
                return this.listaConfigAutocompletarProy;
            }
            set
            {
                this.listaConfigAutocompletarProy = value;
            }
        }
        public List<OntologiaProyecto> ListaOntologiaProyecto
        {
            get
            {
                return this.listaOntologiaProyecto;
            }
            set
            {
                this.listaOntologiaProyecto = value;
            }
        }
        public List<ProyectoEventoParticipante> ListaProyectoEventoParticipante
        {
            get
            {
                return this.listaProyectoEventoParticipante;
            }
            set
            {
                this.listaProyectoEventoParticipante = value;
            }
        }
        public List<ProyectoEvento> ListaProyectoEvento
        {
            get
            {
                return this.listaProyectoEvento;
            }
            set
            {
                this.listaProyectoEvento = value;
            }
        }
        public List<PreferenciaProyecto> ListaPreferenciaProyecto
        {
            get
            {
                return this.listaPreferenciaProyecto;
            }
            set
            {
                this.listaPreferenciaProyecto = value;
            }
        }
        public List<AccionesExternasProyecto> ListaAccionesExternasProyecto
        {
            get
            {
                return this.listaAccionesExternasProyecto;
            }
            set
            {
                this.listaAccionesExternasProyecto = value;
            }
        }
        public List<DatoExtraProyectoVirtuoso> ListaDatoExtraProyectoVirtuoso
        {
            get
            {
                return this.listaDatoExtraProyectoVirtuoso;
            }
            set
            {
                this.listaDatoExtraProyectoVirtuoso = value;
            }
        }
        public List<DatoExtraEcosistemaVirtuoso> ListaDatoExtraEcosistemaVirtuoso
        {
            get
            {
                return listaDatoExtraEcosistemaVirtuoso;
            }
            set
            {
                listaDatoExtraEcosistemaVirtuoso = value;
            }
        }
        public List<DatoExtraEcosistemaOpcion> ListaDatoExtraEcosistemaOpcion
        {
            get
            {
                return listaDatoExtraEcosistemaOpcion;
            }
            set
            {
                listaDatoExtraEcosistemaOpcion = value;
            }
        }
        public List<DatoExtraEcosistema> ListaDatoExtraEcosistema
        {
            get
            {
                return listaDatoExtraEcosistema;
            }
            set
            {
                listaDatoExtraEcosistema = value;
            }
        }
        public List<CamposRegistroProyectoGenericos> ListaCamposRegistroProyectoGenericos
        {
            get
            {
                return listaCamposRegistroProyectoGenericos;
            }
            set
            {
                listaCamposRegistroProyectoGenericos = value;
            }
        }
        public List<DatoExtraProyecto> ListaDatoExtraProyecto
        {
            get
            {
                return listaDatoExtraProyecto;
            }
            set
            {
                listaDatoExtraProyecto = value;
            }
        }
        public List<TipoOntoDispRolUsuarioProy> ListaTipoOntoDispRolUsuarioProy
        {
            get
            {
                return listaTipoOntoDispRolUsuarioProy;
            }
            set
            {
                listaTipoOntoDispRolUsuarioProy = value;
            }
        }

        public List<TipoDocDispRolUsuarioProy> ListaTipoDocDispRolUsuarioProy
        {
            get
            {
                return listaTipoDocDispRolUsuarioProy;
            }
            set
            {
                listaTipoDocDispRolUsuarioProy = value;
            }
        }

        public List<AdministradorGrupoProyecto> ListaAdministradorGrupoProyecto
        {
            get
            {
                return listaAdministradorGrupoProyecto;
            }

            set
            {
                listaAdministradorGrupoProyecto = value;
            }
        }

        public List<AdministradorProyecto> ListaAdministradorProyecto
        {
            get
            {
                return listaAdministradorProyecto;
            }

            set
            {
                listaAdministradorProyecto = value;
            }
        }

        public List<NivelCertificacion> ListaNivelCertificacion
        {
            get
            {
                return listaNivelCertificacion;
            }

            set
            {
                listaNivelCertificacion = value;
            }
        }

        public List<PresentacionListadoSemantico> ListaPresentacionListadoSemantico
        {
            get
            {
                return listaPresentacionListadoSemantico;
            }

            set
            {
                listaPresentacionListadoSemantico = value;
            }
        }

        public List<PresentacionMapaSemantico> ListaPresentacionMapaSemantico
        {
            get
            {
                return listaPresentacionMapaSemantico;
            }

            set
            {
                listaPresentacionMapaSemantico = value;
            }
        }

        public List<PresentacionMosaicoSemantico> ListaPresentacionMosaicoSemantico
        {
            get
            {
                return listaPresentacionMosaicoSemantico;
            }

            set
            {
                listaPresentacionMosaicoSemantico = value;
            }
        }

        public List<PresentacionPersonalizadoSemantico> ListaPresentacionPersonalizadoSemantico
        {
            get
            {
                return listaPresentacionPersonalizadoSemantico;
            }

            set
            {
                listaPresentacionPersonalizadoSemantico = value;
            }
        }

        public List<PresentacionPestanyaListadoSemantico> ListaPresentacionPestanyaListadoSemantico
        {
            get
            {
                return listaPresentacionPestanyaListadoSemantico;
            }

            set
            {
                listaPresentacionPestanyaListadoSemantico = value;
            }
        }

        public List<PresentacionPestanyaMapaSemantico> ListaPresentacionPestanyaMapaSemantico
        {
            get
            {
                return listaPresentacionPestanyaMapaSemantico;
            }

            set
            {
                listaPresentacionPestanyaMapaSemantico = value;
            }
        }

        public List<PresentacionPestanyaMosaicoSemantico> ListaPresentacionPestanyaMosaicoSemantico
        {
            get
            {
                return listaPresentacionPestanyaMosaicoSemantico;
            }

            set
            {
                listaPresentacionPestanyaMosaicoSemantico = value;
            }
        }

        public List<Proyecto> ListaProyecto
        {
            get
            {
                return listaProyecto;
            }

            set
            {
                listaProyecto = value;
            }
        }

        public List<ProyectoAgCatTesauro> ListaProyectoAgCatTesauro
        {
            get
            {
                return listaProyectoAgCatTesauro;
            }

            set
            {
                listaProyectoAgCatTesauro = value;
            }
        }

        public List<ProyectoCerradoTmp> ListaProyectoCerradoTmp
        {
            get
            {
                return listaProyectoCerradoTmp;
            }

            set
            {
                listaProyectoCerradoTmp = value;
            }
        }

        public List<ProyectoCerrandose> ListaProyectoCerrandose
        {
            get
            {
                return listaProyectoCerrandose;
            }

            set
            {
                listaProyectoCerrandose = value;
            }
        }

        public List<ProyectoConfigExtraSem> ListaProyectoConfigExtraSem
        {
            get
            {
                return listaProyectoConfigExtraSem;
            }

            set
            {
                listaProyectoConfigExtraSem = value;
            }
        }

        public List<ProyectoGadget> ListaProyectoGadget
        {
            get
            {
                return listaProyectoGadget;
            }

            set
            {
                listaProyectoGadget = value;
            }
        }

        public List<ProyectoGadgetContexto> ListaProyectoGadgetContexto
        {
            get
            {
                return listaProyectoGadgetContexto;
            }

            set
            {
                listaProyectoGadgetContexto = value;
            }
        }

        public List<ProyectoGadgetContextoHTMLplano> ListaProyectoGadgetContextoHTMLplano
        {
            get
            {
                return listaProyectoGadgetContextoHTMLplano;
            }

            set
            {
                listaProyectoGadgetContextoHTMLplano = value;
            }
        }

        public List<ProyectoGadgetIdioma> ListaProyectoGadgetIdioma
        {
            get
            {
                return listaProyectoGadgetIdioma;
            }

            set
            {
                listaProyectoGadgetIdioma = value;
            }
        }

        public List<ProyectoGrafoFichaRec> ListaProyectoGrafoFichaRec
        {
            get
            {
                return listaProyectoGrafoFichaRec;
            }

            set
            {
                listaProyectoGrafoFichaRec = value;
            }
        }

        public List<ProyectoLoginConfiguracion> ListaProyectoLoginConfiguracion
        {
            get
            {
                return listaProyectoLoginConfiguracion;
            }

            set
            {
                listaProyectoLoginConfiguracion = value;
            }
        }

        public List<ProyectoPaginaHtml> ListaProyectoPaginaHtml
        {
            get
            {
                return listaProyectoPaginaHtml;
            }

            set
            {
                listaProyectoPaginaHtml = value;
            }
        }

        public List<ProyectoPasoRegistro> ListaProyectoPasoRegistro
        {
            get
            {
                return listaProyectoPasoRegistro;
            }

            set
            {
                listaProyectoPasoRegistro = value;
            }
        }

        public List<ProyectoPerfilNumElem> ListaProyectoPerfilNumElem
        {
            get
            {
                return listaProyectoPerfilNumElem;
            }

            set
            {
                listaProyectoPerfilNumElem = value;
            }
        }

        public List<ProyectoPestanya> ListaProyectoPestanya
        {
            get
            {
                return listaProyectoPestanya;
            }

            set
            {
                listaProyectoPestanya = value;
            }
        }

        public List<ProyectoPestanyaBusqueda> ListaProyectoPestanyaBusqueda
        {
            get
            {
                return listaProyectoPestanyaBusqueda;
            }

            set
            {
                listaProyectoPestanyaBusqueda = value;
            }
        }

        public List<ProyectoPestanyaBusquedaExportacion> ListaProyectoPestanyaBusquedaExportacion
        {
            get
            {
                return listaProyectoPestanyaBusquedaExportacion;
            }

            set
            {
                listaProyectoPestanyaBusquedaExportacion = value;
            }
        }

        public List<ProyectoPestanyaBusquedaExportacionExterna> ListaProyectoPestanyaBusquedaExportacionExterna
        {
            get
            {
                return listaProyectoPestanyaBusquedaExportacionExterna;
            }

            set
            {
                listaProyectoPestanyaBusquedaExportacionExterna = value;
            }
        }

        public List<ProyectoPestanyaBusquedaExportacionPropiedad> ListaProyectoPestanyaBusquedaExportacionPropiedad
        {
            get
            {
                return listaProyectoPestanyaBusquedaExportacionPropiedad;
            }

            set
            {
                listaProyectoPestanyaBusquedaExportacionPropiedad = value;
            }
        }

        public List<ProyectoPestanyaCMS> ListaProyectoPestanyaCMS
        {
            get
            {
                return listaProyectoPestanyaCMS;
            }

            set
            {
                listaProyectoPestanyaCMS = value;
            }
        }

        public List<ProyectoPestanyaExportacionBusqueda> ListaProyectoPestanyaExportacionBusqueda
        {
            get
            {
                return listaProyectoPestanyaExportacionBusqueda;
            }

            set
            {
                listaProyectoPestanyaExportacionBusqueda = value;
            }
        }

        public List<ProyectoPestanyaFiltroOrdenRecursos> ListaProyectoPestanyaFiltroOrdenRecursos
        {
            get
            {
                return listaProyectoPestanyaFiltroOrdenRecursos;
            }

            set
            {
                listaProyectoPestanyaFiltroOrdenRecursos = value;
            }
        }

        public void AddProyecto(Proyecto pProyectoNuevo)
        {
            this.listaProyecto.Add(pProyectoNuevo);
        }

        public List<ProyectoPestanyaMenu> ListaProyectoPestanyaMenu
        {
            get
            {
                return listaProyectoPestanyaMenu;
            }

            set
            {
                listaProyectoPestanyaMenu = value;
            }
        }

        public List<ProyectoPestanyaMenuRolGrupoIdentidades> ListaProyectoPestanyaMenuRolGrupoIdentidades
        {
            get
            {
                return listaProyectoPestanyaMenuRolGrupoIdentidades;
            }

            set
            {
                listaProyectoPestanyaMenuRolGrupoIdentidades = value;
            }
        }

        public List<ProyectoPestanyaMenuRolIdentidad> ListaProyectoPestanyaMenuRolIdentidad
        {
            get
            {
                return listaProyectoPestanyaMenuRolIdentidad;
            }

            set
            {
                listaProyectoPestanyaMenuRolIdentidad = value;
            }
        }

        public List<ProyectoPestanyaRolGrupoIdentidades> ListaProyectoPestanyaRolGrupoIdentidades
        {
            get
            {
                return listaProyectoPestanyaRolGrupoIdentidades;
            }

            set
            {
                listaProyectoPestanyaRolGrupoIdentidades = value;
            }
        }

        public List<ProyectoPestanyaRolIdentidad> ListaProyectoPestanyaRolIdentidad
        {
            get
            {
                return listaProyectoPestanyaRolIdentidad;
            }

            set
            {
                listaProyectoPestanyaRolIdentidad = value;
            }
        }

        public List<ProyectoRelacionado> ListaProyectoRelacionado
        {
            get
            {
                return listaProyectoRelacionado;
            }

            set
            {
                listaProyectoRelacionado = value;
            }
        }

        public List<ProyectoSearchPersonalizado> ListaProyectoSearchPersonalizado
        {
            get
            {
                return listaProyectoSearchPersonalizado;
            }

            set
            {
                listaProyectoSearchPersonalizado = value;
            }
        }

        public List<ProyectoServicioExterno> ListaProyectoServicioExterno
        {
            get
            {
                return listaProyectoServicioExterno;
            }

            set
            {
                listaProyectoServicioExterno = value;
            }
        }

        public List<ProyectosMasActivos> ListaProyectosMasActivos
        {
            get
            {
                return listaProyectosMasActivos;
            }

            set
            {
                listaProyectosMasActivos = value;
            }
        }

        public List<ProyTipoRecNoActivReciente> ListaProyTipoRecNoActivReciente
        {
            get
            {
                return listaProyTipoRecNoActivReciente;
            }

            set
            {
                listaProyTipoRecNoActivReciente = value;
            }
        }

        public List<VistaVirtualProyecto> ListaVistaVirtualProyecto
        {
            get
            {
                return this.listaVistaVirtualProyecto;
            }

            set
            {
                this.listaVistaVirtualProyecto = value;
            }
        }

        public List<RecursosRelacionadosPresentacion> ListaRecursosRelacionadosPresentacion
        {
            get
            {
                return this.listaRecursosRelacionadosPresentacion;
            }
            set
            {
                this.listaRecursosRelacionadosPresentacion = value;
            }
        }

        public List<SeccionProyCatalogo> ListaSeccionProyCatalogo
        {
            get
            {
                return this.listaSeccionProyCatalogo;
            }
            set
            {
                this.listaSeccionProyCatalogo = value;
            }
        }

        public List<DatoExtraProyectoOpcion> ListaDatoExtraProyectoOpcion
        {
            get
            {
                return this.listaDatoExtraProyectoOpcion;
            }
            set
            {
                this.listaDatoExtraProyectoOpcion = value;
            }
        }

        public List<ProyectoEventoAccion> ListaProyectoEventoAccion
        {
            get
            {
                return this.listaProyectoEventoAccion;
            }
            set
            {
                this.listaProyectoEventoAccion = value;
            }
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperProyecto encapsulado = (DataWrapperProyecto)pDataWrapper;
            this.listaAdministradorGrupoProyecto = this.ListaAdministradorGrupoProyecto.Union(encapsulado.ListaAdministradorGrupoProyecto).ToList();
            this.listaAdministradorProyecto = this.listaAdministradorProyecto.Union(encapsulado.ListaAdministradorProyecto).ToList();
            this.listaNivelCertificacion = this.ListaNivelCertificacion.Union(encapsulado.ListaNivelCertificacion).ToList();
            this.listaPresentacionListadoSemantico = this.ListaPresentacionListadoSemantico.Union(encapsulado.ListaPresentacionListadoSemantico).ToList();
            this.listaPresentacionMapaSemantico = this.ListaPresentacionMapaSemantico.Union(encapsulado.ListaPresentacionMapaSemantico).ToList();
            this.listaPresentacionMosaicoSemantico = this.ListaPresentacionMosaicoSemantico.Union(encapsulado.ListaPresentacionMosaicoSemantico).ToList();
            this.ListaPresentacionPersonalizadoSemantico = this.ListaPresentacionPersonalizadoSemantico.Union(encapsulado.ListaPresentacionPersonalizadoSemantico).ToList();
            this.listaPresentacionPestanyaListadoSemantico = this.ListaPresentacionPestanyaListadoSemantico.Union(encapsulado.ListaPresentacionPestanyaListadoSemantico).ToList();
            this.listaPresentacionPestanyaMapaSemantico = this.ListaPresentacionPestanyaMapaSemantico.Union(encapsulado.ListaPresentacionPestanyaMapaSemantico).ToList();
            this.listaPresentacionPestanyaMosaicoSemantico = this.ListaPresentacionPestanyaMosaicoSemantico.Union(encapsulado.ListaPresentacionPestanyaMosaicoSemantico).ToList();
            this.listaProyecto = this.ListaProyecto.Union(encapsulado.ListaProyecto).ToList();
            this.listaProyectoAgCatTesauro = this.ListaProyectoAgCatTesauro.Union(encapsulado.ListaProyectoAgCatTesauro).ToList();
            this.listaProyectoCerradoTmp = this.ListaProyectoCerradoTmp.Union(encapsulado.ListaProyectoCerradoTmp).ToList();
            this.listaProyectoCerrandose = this.ListaProyectoCerrandose.Union(encapsulado.ListaProyectoCerrandose).ToList();
            this.listaProyectoConfigExtraSem = this.ListaProyectoConfigExtraSem.Union(encapsulado.ListaProyectoConfigExtraSem).ToList();
            this.listaProyectoGadget = this.listaProyectoGadget.Union(encapsulado.ListaProyectoGadget).ToList();
            this.listaProyectoGadgetContexto = this.listaProyectoGadgetContexto.Union(encapsulado.ListaProyectoGadgetContexto).ToList();
            this.listaProyectoGadgetContextoHTMLplano = this.ListaProyectoGadgetContextoHTMLplano.Union(encapsulado.ListaProyectoGadgetContextoHTMLplano).ToList();
            this.listaProyectoGadgetIdioma = this.listaProyectoGadgetIdioma.Union(encapsulado.ListaProyectoGadgetIdioma).ToList();
            this.listaProyectoGrafoFichaRec = this.ListaProyectoGrafoFichaRec.Union(encapsulado.ListaProyectoGrafoFichaRec).ToList();
            this.listaProyectoLoginConfiguracion = this.ListaProyectoLoginConfiguracion.Union(encapsulado.ListaProyectoLoginConfiguracion).ToList();
            this.listaProyectoPaginaHtml = this.ListaProyectoPaginaHtml.Union(encapsulado.ListaProyectoPaginaHtml).ToList();
            this.listaProyectoPasoRegistro = this.ListaProyectoPasoRegistro.Union(encapsulado.ListaProyectoPasoRegistro).ToList();
            this.listaProyectoPerfilNumElem = this.ListaProyectoPerfilNumElem.Union(encapsulado.ListaProyectoPerfilNumElem).ToList();
            this.listaProyectoPestanya = this.ListaProyectoPestanya.Union(encapsulado.ListaProyectoPestanya).ToList();
            this.listaProyectoPestanyaBusqueda = this.ListaProyectoPestanyaBusqueda.Union(encapsulado.ListaProyectoPestanyaBusqueda).ToList();
            this.listaProyectoPestanyaBusquedaExportacion = this.ListaProyectoPestanyaBusquedaExportacion.Union(encapsulado.ListaProyectoPestanyaBusquedaExportacion).ToList();
            this.listaProyectoPestanyaBusquedaExportacionExterna = this.ListaProyectoPestanyaBusquedaExportacionExterna.Union(encapsulado.ListaProyectoPestanyaBusquedaExportacionExterna).ToList();
            this.listaProyectoPestanyaBusquedaExportacionPropiedad = this.ListaProyectoPestanyaBusquedaExportacionPropiedad.Union(encapsulado.ListaProyectoPestanyaBusquedaExportacionPropiedad).ToList();
            this.listaProyectoPestanyaCMS = this.ListaProyectoPestanyaCMS.Union(encapsulado.ListaProyectoPestanyaCMS).ToList();
            this.listaProyectoPestanyaExportacionBusqueda = this.ListaProyectoPestanyaExportacionBusqueda.Union(encapsulado.ListaProyectoPestanyaExportacionBusqueda).ToList();
            this.listaProyectoPestanyaFiltroOrdenRecursos = this.ListaProyectoPestanyaFiltroOrdenRecursos.Union(encapsulado.ListaProyectoPestanyaFiltroOrdenRecursos).ToList();
            this.listaProyectoPestanyaMenu = this.ListaProyectoPestanyaMenu.Union(encapsulado.ListaProyectoPestanyaMenu).ToList();
            this.listaProyectoPestanyaMenuRolGrupoIdentidades = this.ListaProyectoPestanyaMenuRolGrupoIdentidades.Union(encapsulado.ListaProyectoPestanyaMenuRolGrupoIdentidades).ToList();
            this.listaProyectoPestanyaMenuRolIdentidad = this.ListaProyectoPestanyaMenuRolIdentidad.Union(encapsulado.ListaProyectoPestanyaMenuRolIdentidad).ToList();
            this.listaProyectoPestanyaRolGrupoIdentidades = this.ListaProyectoPestanyaRolGrupoIdentidades.Union(encapsulado.ListaProyectoPestanyaRolGrupoIdentidades).ToList();
            this.listaProyectoPestanyaRolIdentidad = this.ListaProyectoPestanyaRolIdentidad.Union(encapsulado.ListaProyectoPestanyaRolIdentidad).ToList();
            this.listaProyectoRelacionado = this.ListaProyectoRelacionado.Union(encapsulado.ListaProyectoRelacionado).ToList();
            this.listaProyectoSearchPersonalizado = this.ListaProyectoSearchPersonalizado.Union(encapsulado.ListaProyectoSearchPersonalizado).ToList();
            this.listaProyectoServicioExterno = this.ListaProyectoServicioExterno.Union(encapsulado.ListaProyectoServicioExterno).ToList();
            this.listaProyectosMasActivos = this.ListaProyectosMasActivos.Union(encapsulado.ListaProyectosMasActivos).ToList();
            this.listaProyTipoRecNoActivReciente = this.ListaProyTipoRecNoActivReciente.Union(encapsulado.ListaProyTipoRecNoActivReciente).ToList();
            this.listaDatoExtraProyecto = this.ListaDatoExtraProyecto.Union(encapsulado.ListaDatoExtraProyecto).ToList();
            this.listaCamposRegistroProyectoGenericos = this.ListaCamposRegistroProyectoGenericos.Union(encapsulado.ListaCamposRegistroProyectoGenericos).ToList();
            this.listaDatoExtraEcosistema = this.ListaDatoExtraEcosistema.Union(encapsulado.ListaDatoExtraEcosistema).ToList();
            this.listaDatoExtraEcosistemaOpcion = this.ListaDatoExtraEcosistemaOpcion.Union(encapsulado.ListaDatoExtraEcosistemaOpcion).ToList();
            this.listaDatoExtraEcosistemaVirtuoso = this.ListaDatoExtraEcosistemaVirtuoso.Union(encapsulado.ListaDatoExtraEcosistemaVirtuoso).ToList();
            this.listaDatoExtraProyectoOpcion = this.ListaDatoExtraProyectoOpcion.Union(encapsulado.ListaDatoExtraProyectoOpcion).ToList();
            this.listaDatoExtraProyectoVirtuoso = this.ListaDatoExtraProyectoVirtuoso.Union(encapsulado.ListaDatoExtraProyectoVirtuoso).ToList();
            this.listaAccionesExternasProyecto = this.ListaAccionesExternasProyecto.Union(encapsulado.ListaAccionesExternasProyecto).ToList();
            this.listaPreferenciaProyecto = this.ListaPreferenciaProyecto.Union(encapsulado.ListaPreferenciaProyecto).ToList();
            this.listaProyectoEvento = this.ListaProyectoEvento.Union(encapsulado.ListaProyectoEvento).ToList();
            this.listaProyectoEventoParticipante = this.ListaProyectoEventoParticipante.Union(encapsulado.ListaProyectoEventoParticipante).ToList();
            this.listaOntologiaProyecto = this.ListaOntologiaProyecto.Union(encapsulado.ListaOntologiaProyecto).ToList();
            this.listaConfigAutocompletarProy = this.ListaConfigAutocompletarProy.Union(encapsulado.ListaConfigAutocompletarProy).ToList();
            this.listaProyectoEventoAccion = this.ListaProyectoEventoAccion.Union(encapsulado.ListaProyectoEventoAccion).ToList();
            this.listaProyectoConAdministrado = this.ListaProyectoConAdministrado.Union(encapsulado.ListaProyectoConAdministrado).ToList();
            this.listaDatoExtraProyectoOpcionIdentidad = this.listaDatoExtraProyectoOpcionIdentidad.Union(encapsulado.listaDatoExtraProyectoOpcionIdentidad).ToList();
            this.listaProyectoNumConexiones = this.listaProyectoNumConexiones.Union(encapsulado.listaProyectoNumConexiones).ToList();
            this.listaRecursosRelacionadosPresentacion = this.listaRecursosRelacionadosPresentacion.Union(encapsulado.ListaRecursosRelacionadosPresentacion).ToList();
            ListaVistaVirtualProyecto = ListaVistaVirtualProyecto.Union(encapsulado.ListaVistaVirtualProyecto).ToList();
        }

        public void DeleteProyectoCerradoTmp(ProyectoCerradoTmp proyectoCerradoTmp)
        {
            this.listaProyectoCerradoTmp.Remove(proyectoCerradoTmp);
        }

        public void DeleteProyectoCerrandose(ProyectoCerrandose proyectoCerrandose)
        {
            this.listaProyectoCerrandose.Remove(proyectoCerrandose);
        }

        public void AddAdministradorProyecto(AdministradorProyecto filaAdminPro)
        {
            this.listaAdministradorProyecto.Add(filaAdminPro);
        }

        public void DeleteAdministradorProyecto(AdministradorProyecto administradorProyecto)
        {
            this.listaAdministradorProyecto.Remove(administradorProyecto);
        }


        public ProyectoPestanyaMenu AddProyectoPestanyaMenuRow(Guid pestanyaId, Guid organizacionID, Guid proyectoID, ProyectoPestanyaMenu p1, short tipoPestanya, string nombre, string ruta, short orden, bool nuevaPestanya, bool visible, short privacidad, string htmlAlternativo, string idiomasDisponibles, string titulo, string nombreCortoPestanya, bool visibleSinAcceso, string cssBodyClass, string metaDescription, bool activa)
        {
            ProyectoPestanyaMenu proyectoPestanyaMenu = new ProyectoPestanyaMenu(pestanyaId, organizacionID, proyectoID, p1, tipoPestanya, nombre, ruta, orden, nuevaPestanya, visible, privacidad, htmlAlternativo, idiomasDisponibles, titulo, nombreCortoPestanya, visibleSinAcceso, cssBodyClass, metaDescription, activa);
            this.listaProyectoPestanyaMenu.Add(proyectoPestanyaMenu);
            return proyectoPestanyaMenu;
        }

        public void AddProyectoGadgetRow(Guid organizacionID, Guid proyectoID, Guid gadgetID, string titulo, string contenido, int orden, short tipo, string ubicacion, string clases, short tipoUbicacion, bool visible, bool multiIdioma, Guid personalizacionComponenteID, bool cargarPorAjax, string comunidadDestinoFiltros, string nombreCorto)
        {
            ProyectoGadget proyectoGadget = new ProyectoGadget(organizacionID, proyectoID, gadgetID, titulo, contenido, orden, tipo, ubicacion, clases, tipoUbicacion, visible, multiIdioma, personalizacionComponenteID, cargarPorAjax, comunidadDestinoFiltros, nombreCorto);
            proyectoGadget.PersonalizacionComponenteID = null;
            //if (string.IsNullOrEmpty(nombreCorto))
            //{
            //    proyectoGadget.NombreCorto = gadgetID.ToString();
            //}
            this.listaProyectoGadget.Add(proyectoGadget);
        }

        public void CargaRelacionesPerezosasCache()
        {
            foreach(AdministradorProyecto adminProy in ListaAdministradorProyecto)
            {
                adminProy.Proyecto = ListaProyecto.FirstOrDefault(proy => proy.OrganizacionID.Equals(adminProy.OrganizacionID) && proy.ProyectoID.Equals(adminProy.ProyectoID));
            }

            foreach(Proyecto proy in ListaProyecto)
            {
                proy.AdministradorProyecto = ListaAdministradorProyecto.Where(adminProy => adminProy.OrganizacionID.Equals(proy.OrganizacionID) && adminProy.ProyectoID.Equals(proy.ProyectoID)).ToList();

                proy.ProyectoAgCatTesauro = ListaProyectoAgCatTesauro.Where(agCat => agCat.OrganizacionID.Equals(proy.OrganizacionID) && agCat.ProyectoID.Equals(proy.ProyectoID)).ToList();

                proy.ProyectoCerradoTmp = ListaProyectoCerradoTmp.FirstOrDefault(cerradoTmp => cerradoTmp.ProyectoID.Equals(proy.ProyectoID) && cerradoTmp.OrganizacionID.Equals(proy.OrganizacionID));

                proy.ProyectoCerrandose = ListaProyectoCerrandose.FirstOrDefault(cerrandose => cerrandose.ProyectoID.Equals(proy.ProyectoID) && cerrandose.OrganizacionID.Equals(proy.OrganizacionID));

                proy.ProyectosMasActivos = ListaProyectosMasActivos.FirstOrDefault(activo => activo.ProyectoID.Equals(proy.ProyectoID) && activo.OrganizacionID.Equals(proy.OrganizacionID));
            }

            foreach (ProyectoEventoParticipante proyEvP in ListaProyectoEventoParticipante)
            {
                proyEvP.ProyectoEvento = ListaProyectoEvento.FirstOrDefault(proyEven => proyEven.EventoID.Equals(proyEvP.EventoID));
            }

            foreach(ProyectoGadget proyG in ListaProyectoGadget)
            {
                proyG.ProyectoGadgetIdioma = ListaProyectoGadgetIdioma.Where(proyGI => proyGI.OrganizacionID.Equals(proyG.OrganizacionID) && proyGI.ProyectoID.Equals(proyG.ProyectoID) && proyGI.GadgetID.Equals(proyG.GadgetID)).ToList();
                proyG.ProyectoGadgetContexto = ListaProyectoGadgetContexto.FirstOrDefault(proyGI => proyGI.OrganizacionID.Equals(proyG.OrganizacionID) && proyGI.ProyectoID.Equals(proyG.ProyectoID) && proyGI.GadgetID.Equals(proyG.GadgetID));
            }

            foreach(ProyectoPestanyaBusqueda proyPB in ListaProyectoPestanyaBusqueda)
            {
                proyPB.ProyectoPestanyaMenu = ListaProyectoPestanyaMenu.FirstOrDefault(pestMenu => pestMenu.PestanyaID.Equals(proyPB.PestanyaID));
            }

            foreach(ProyectoPestanyaCMS pestCMS in ListaProyectoPestanyaCMS)
            {
                pestCMS.ProyectoPestanyaMenu = ListaProyectoPestanyaMenu.FirstOrDefault(pestMenu => pestMenu.PestanyaID.Equals(pestCMS.PestanyaID));
            }

            foreach(ProyectoPestanyaMenu pestanyaMenu in ListaProyectoPestanyaMenu)
            {
                pestanyaMenu.ProyectoPestanyaBusqueda = ListaProyectoPestanyaBusqueda.FirstOrDefault(busqueda => busqueda.PestanyaID.Equals(pestanyaMenu.PestanyaID));
                pestanyaMenu.ProyectoPestanyaCMS = ListaProyectoPestanyaCMS.Where(pestCMS => pestCMS.PestanyaID.Equals(pestanyaMenu.PestanyaID)).ToList();
                pestanyaMenu.ProyectoPestanyaFiltroOrdenRecursos = ListaProyectoPestanyaFiltroOrdenRecursos.Where(filtro => filtro.PestanyaID.Equals(pestanyaMenu.PestanyaID)).ToList();
                pestanyaMenu.ProyectoPestanyaMenu1 = ListaProyectoPestanyaMenu.Where(hijos => hijos.PestanyaPadreID.Equals(pestanyaMenu.PestanyaID)).ToList();
                pestanyaMenu.ProyectoPestanyaMenu2 = ListaProyectoPestanyaMenu.FirstOrDefault(padre => padre.PestanyaID.Equals(pestanyaMenu.PestanyaPadreID));
                pestanyaMenu.ProyectoPestanyaMenuRolGrupoIdentidades = ListaProyectoPestanyaMenuRolGrupoIdentidades.Where(rol => rol.PestanyaID.Equals(pestanyaMenu.PestanyaID)).ToList();
                pestanyaMenu.ProyectoPestanyaMenuRolIdentidad = ListaProyectoPestanyaMenuRolIdentidad.Where(ident => ident.PestanyaID.Equals(pestanyaMenu.PestanyaID)).ToList();
                pestanyaMenu.ConfigAutocompletarProy = ListaConfigAutocompletarProy.Where(config => config.PestanyaID.Equals(pestanyaMenu.PestanyaID)).ToList();
            }
        }
    }
}
