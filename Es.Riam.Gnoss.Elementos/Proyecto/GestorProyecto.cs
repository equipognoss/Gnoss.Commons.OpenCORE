using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Elementos.Proyecto
{
    class GestorProyecto
    {
        private List<AdministradorGrupoProyecto> listaAdministradorGrupoProyecto;
        private List<AdministradorProyecto> listaAdministradorProyecto;
        private List<NivelCertificacion> listaNivelCertificacion;
        private List<PresentacionListadoSemantico> listaPresentacionListadoSemantico;
        private List<PresentacionMapaSemantico> listaPresentacionMapaSemantico;
        private List<PresentacionMosaicoSemantico> listaPresentacionMosaicoSemantico;
        private List<PresentacionPestanyaListadoSemantico> listaPresentacionPestanyaListadoSemantico;
        private List<PresentacionPestanyaMapaSemantico> listaPresentacionPestanyaMapaSemantico;
        private List<PresentacionPestanyaMosaicoSemantico> listaPresentacionPestanyaMosaicoSemantico;
        private List<AD.EntityModel.Models.ProyectoDS.Proyecto> listaProyecto;
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

        public GestorProyecto()
        {
            this.listaAdministradorGrupoProyecto = new List<AdministradorGrupoProyecto>();
            this.listaAdministradorProyecto = new List<AdministradorProyecto>();
            this.listaNivelCertificacion = new List<NivelCertificacion>();
            this.listaPresentacionListadoSemantico = new List<PresentacionListadoSemantico>();
            this.listaPresentacionMapaSemantico = new List<PresentacionMapaSemantico>();
            this.listaPresentacionMosaicoSemantico = new List<PresentacionMosaicoSemantico>();
            this.listaPresentacionPestanyaListadoSemantico = new List<PresentacionPestanyaListadoSemantico>();
            this.listaPresentacionPestanyaMapaSemantico = new List<PresentacionPestanyaMapaSemantico>();
            this.listaPresentacionPestanyaMosaicoSemantico = new List<PresentacionPestanyaMosaicoSemantico>();
            this.listaProyecto = new List<AD.EntityModel.Models.ProyectoDS.Proyecto>();
            this.listaProyectoAgCatTesauro = new List<ProyectoAgCatTesauro>();
            this.listaProyectoCerradoTmp = new List<ProyectoCerradoTmp>();
            this.listaProyectoCerrandose = new List<ProyectoCerrandose>();
            this.listaProyectoConfigExtraSem = new List<ProyectoConfigExtraSem>();
            this.listaProyectoGadget = new List<ProyectoGadget>();
            this.listaProyectoGadgetContexto = new List<ProyectoGadgetContexto>();
            this.listaProyectoGadgetContextoHTMLplano = new List<ProyectoGadgetContextoHTMLplano>();
            this.listaProyectoGadgetIdioma = new List<ProyectoGadgetIdioma>();
            this.listaProyectoGrafoFichaRec = new List<ProyectoGrafoFichaRec>();
            this.listaProyectoLoginConfiguracion = new List<ProyectoLoginConfiguracion>();
            this.listaProyectoPaginaHtml = new List<ProyectoPaginaHtml>();
            this.listaProyectoPasoRegistro = new List<ProyectoPasoRegistro>();
            this.listaProyectoPerfilNumElem = new List<ProyectoPerfilNumElem>();
            this.listaProyectoPestanya = new List<ProyectoPestanya>();
            this.listaProyectoPestanyaBusqueda = new List<ProyectoPestanyaBusqueda>();
            this.listaProyectoPestanyaBusquedaExportacion = new List<ProyectoPestanyaBusquedaExportacion>();
            this.listaProyectoPestanyaBusquedaExportacionExterna = new List<ProyectoPestanyaBusquedaExportacionExterna>();
            this.listaProyectoPestanyaBusquedaExportacionPropiedad = new List<ProyectoPestanyaBusquedaExportacionPropiedad>();
            this.listaProyectoPestanyaCMS = new List<ProyectoPestanyaCMS>();
            this.listaProyectoPestanyaExportacionBusqueda = new List<ProyectoPestanyaExportacionBusqueda>();
            this.listaProyectoPestanyaFiltroOrdenRecursos = new List<ProyectoPestanyaFiltroOrdenRecursos>();
            this.listaProyectoPestanyaMenu = new List<ProyectoPestanyaMenu>();
            this.listaProyectoPestanyaMenuRolGrupoIdentidades = new List<ProyectoPestanyaMenuRolGrupoIdentidades>();
            this.listaProyectoPestanyaMenuRolIdentidad = new List<ProyectoPestanyaMenuRolIdentidad>();
            this.listaProyectoPestanyaRolGrupoIdentidades = new List<ProyectoPestanyaRolGrupoIdentidades>();
            this.listaProyectoPestanyaRolIdentidad = new List<ProyectoPestanyaRolIdentidad>();
            this.listaProyectoRelacionado = new List<ProyectoRelacionado>();
            this.listaProyectoSearchPersonalizado = new List<ProyectoSearchPersonalizado>();
            this.listaProyectoServicioExterno = new List<ProyectoServicioExterno>();
            this.listaProyectosMasActivos = new List<ProyectosMasActivos>();
            this.listaProyTipoRecNoActivReciente = new List<ProyTipoRecNoActivReciente>();
        }

        public void getDataOfEncapsulado(DataWrapperProyecto encapsulado)
        {
            this.listaAdministradorGrupoProyecto = encapsulado.ListaAdministradorGrupoProyecto;
            this.listaAdministradorProyecto = encapsulado.ListaAdministradorProyecto;
            this.listaNivelCertificacion = encapsulado.ListaNivelCertificacion;
            this.listaPresentacionListadoSemantico = encapsulado.ListaPresentacionListadoSemantico;
            this.listaPresentacionMapaSemantico = encapsulado.ListaPresentacionMapaSemantico;
            this.listaPresentacionMosaicoSemantico = encapsulado.ListaPresentacionMosaicoSemantico;
            this.listaPresentacionPestanyaListadoSemantico = encapsulado.ListaPresentacionPestanyaListadoSemantico;
            this.listaPresentacionPestanyaMapaSemantico = encapsulado.ListaPresentacionPestanyaMapaSemantico;
            this.listaPresentacionPestanyaMosaicoSemantico = encapsulado.ListaPresentacionPestanyaMosaicoSemantico;
            this.listaProyecto =encapsulado.ListaProyecto;
            this.listaProyectoAgCatTesauro = encapsulado.ListaProyectoAgCatTesauro;
            this.listaProyectoCerradoTmp = encapsulado.ListaProyectoCerradoTmp;
            this.listaProyectoCerrandose = encapsulado.ListaProyectoCerrandose;
            this.listaProyectoConfigExtraSem = encapsulado.ListaProyectoConfigExtraSem;
            this.listaProyectoGadget = encapsulado.ListaProyectoGadget;
            this.listaProyectoGadgetContexto = encapsulado.ListaProyectoGadgetContexto;
            this.listaProyectoGadgetContextoHTMLplano = encapsulado.ListaProyectoGadgetContextoHTMLplano;
            this.listaProyectoGadgetIdioma = encapsulado.ListaProyectoGadgetIdioma;
            this.listaProyectoGrafoFichaRec = encapsulado.ListaProyectoGrafoFichaRec;
            this.listaProyectoLoginConfiguracion = encapsulado.ListaProyectoLoginConfiguracion;
            this.listaProyectoPaginaHtml = encapsulado.ListaProyectoPaginaHtml;
            this.listaProyectoPasoRegistro = encapsulado.ListaProyectoPasoRegistro;
            this.listaProyectoPerfilNumElem = encapsulado.ListaProyectoPerfilNumElem;
            this.listaProyectoPestanya = encapsulado.ListaProyectoPestanya;
            this.listaProyectoPestanyaBusqueda = encapsulado.ListaProyectoPestanyaBusqueda;
            this.listaProyectoPestanyaBusquedaExportacion = encapsulado.ListaProyectoPestanyaBusquedaExportacion;
            this.listaProyectoPestanyaBusquedaExportacionExterna = encapsulado.ListaProyectoPestanyaBusquedaExportacionExterna;
            this.listaProyectoPestanyaBusquedaExportacionPropiedad = encapsulado.ListaProyectoPestanyaBusquedaExportacionPropiedad;
            this.listaProyectoPestanyaCMS = encapsulado.ListaProyectoPestanyaCMS;
            this.listaProyectoPestanyaExportacionBusqueda = encapsulado.ListaProyectoPestanyaExportacionBusqueda;
            this.listaProyectoPestanyaFiltroOrdenRecursos = encapsulado.ListaProyectoPestanyaFiltroOrdenRecursos;
            this.listaProyectoPestanyaMenu = encapsulado.ListaProyectoPestanyaMenu;
            this.listaProyectoPestanyaMenuRolGrupoIdentidades = encapsulado.ListaProyectoPestanyaMenuRolGrupoIdentidades;
            this.listaProyectoPestanyaMenuRolIdentidad = encapsulado.ListaProyectoPestanyaMenuRolIdentidad;
            this.listaProyectoPestanyaRolGrupoIdentidades = encapsulado.ListaProyectoPestanyaRolGrupoIdentidades;
            this.listaProyectoPestanyaRolIdentidad = encapsulado.ListaProyectoPestanyaRolIdentidad;
            this.listaProyectoRelacionado = encapsulado.ListaProyectoRelacionado;
            this.listaProyectoSearchPersonalizado = encapsulado.ListaProyectoSearchPersonalizado;
            this.listaProyectoServicioExterno = encapsulado.ListaProyectoServicioExterno;
            this.listaProyectosMasActivos = encapsulado.ListaProyectosMasActivos;
            this.listaProyTipoRecNoActivReciente = encapsulado.ListaProyTipoRecNoActivReciente;
        }

        public DataWrapperProyecto encapsularDatos()
        {
            DataWrapperProyecto encapsulado = new DataWrapperProyecto();
            encapsulado.ListaAdministradorGrupoProyecto= this.listaAdministradorGrupoProyecto;
            encapsulado.ListaAdministradorProyecto= this.listaAdministradorProyecto;
            encapsulado.ListaNivelCertificacion=this.listaNivelCertificacion;
            encapsulado.ListaPresentacionListadoSemantico= this.listaPresentacionListadoSemantico;
            encapsulado.ListaPresentacionMapaSemantico= this.listaPresentacionMapaSemantico;
            encapsulado.ListaPresentacionMosaicoSemantico = this.listaPresentacionMosaicoSemantico;
            encapsulado.ListaPresentacionPestanyaListadoSemantico= this.listaPresentacionPestanyaListadoSemantico;
            encapsulado.ListaPresentacionPestanyaMapaSemantico= this.listaPresentacionPestanyaMapaSemantico;
            encapsulado.ListaPresentacionPestanyaMosaicoSemantico= this.listaPresentacionPestanyaMosaicoSemantico;
            encapsulado.ListaProyecto= this.listaProyecto;
            encapsulado.ListaProyectoAgCatTesauro= this.listaProyectoAgCatTesauro;
            encapsulado.ListaProyectoCerradoTmp= this.listaProyectoCerradoTmp;
            encapsulado.ListaProyectoCerrandose= this.listaProyectoCerrandose;
            encapsulado.ListaProyectoConfigExtraSem= this.listaProyectoConfigExtraSem;
            encapsulado.ListaProyectoGadget= this.listaProyectoGadget;
            encapsulado.ListaProyectoGadgetContexto= this.listaProyectoGadgetContexto;
            encapsulado.ListaProyectoGadgetContextoHTMLplano= this.listaProyectoGadgetContextoHTMLplano;
            encapsulado.ListaProyectoGadgetIdioma= this.listaProyectoGadgetIdioma;
            encapsulado.ListaProyectoGrafoFichaRec= this.listaProyectoGrafoFichaRec;
            encapsulado.ListaProyectoLoginConfiguracion= this.listaProyectoLoginConfiguracion;
            encapsulado.ListaProyectoPaginaHtml= this.listaProyectoPaginaHtml;
            encapsulado.ListaProyectoPasoRegistro= this.listaProyectoPasoRegistro;
            encapsulado.ListaProyectoPerfilNumElem= this.listaProyectoPerfilNumElem;
            encapsulado.ListaProyectoPestanya= this.listaProyectoPestanya;
            encapsulado.ListaProyectoPestanyaBusqueda= this.listaProyectoPestanyaBusqueda;
            encapsulado.ListaProyectoPestanyaBusquedaExportacion= this.listaProyectoPestanyaBusquedaExportacion;
            encapsulado.ListaProyectoPestanyaBusquedaExportacionExterna= this.listaProyectoPestanyaBusquedaExportacionExterna;
            encapsulado.ListaProyectoPestanyaBusquedaExportacionPropiedad= this.listaProyectoPestanyaBusquedaExportacionPropiedad;
            encapsulado.ListaProyectoPestanyaCMS= this.listaProyectoPestanyaCMS;
            encapsulado.ListaProyectoPestanyaExportacionBusqueda= this.listaProyectoPestanyaExportacionBusqueda;
            encapsulado.ListaProyectoPestanyaFiltroOrdenRecursos= this.listaProyectoPestanyaFiltroOrdenRecursos;
            encapsulado.ListaProyectoPestanyaMenu= this.listaProyectoPestanyaMenu;
            encapsulado.ListaProyectoPestanyaMenuRolGrupoIdentidades= this.listaProyectoPestanyaMenuRolGrupoIdentidades;
            encapsulado.ListaProyectoPestanyaMenuRolIdentidad= this.listaProyectoPestanyaMenuRolIdentidad;
            encapsulado.ListaProyectoPestanyaRolGrupoIdentidades= this.listaProyectoPestanyaRolGrupoIdentidades;
            encapsulado.ListaProyectoPestanyaRolIdentidad= this.listaProyectoPestanyaRolIdentidad;
            encapsulado.ListaProyectoRelacionado= this.listaProyectoRelacionado;
            encapsulado.ListaProyectoSearchPersonalizado= this.listaProyectoSearchPersonalizado;
            encapsulado.ListaProyectoServicioExterno= this.listaProyectoServicioExterno;
            encapsulado.ListaProyectosMasActivos= this.listaProyectosMasActivos;
            encapsulado.ListaProyTipoRecNoActivReciente= this.listaProyTipoRecNoActivReciente;
            return encapsulado;
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

        public List<AD.EntityModel.Models.ProyectoDS.Proyecto> ListaProyecto
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
    }
}
