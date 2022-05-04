using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces.Observador;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.ServiciosGenerales
{
    /// <summary>
    /// Gestor de proyectos
    /// </summary>
    [Serializable]
    public class GestionProyecto : GestionGnoss, ISerializable
    {
        #region Miembros

        /// <summary>
        /// DS de parametros generales de los proyectos
        /// </summary>
        private GestorParametroGeneral mParametroGeneralDS;

        /// <summary>
        /// Lista de proyectos
        /// </summary>
        private Dictionary<Guid, Proyecto> mListaProyectos = null;

        /// <summary>
        /// Gestor de tesauro
        /// </summary>
        private GestionTesauro mGestionTesauro;

        /// <summary>
        /// Gestor de Personas
        /// </summary>
        private GestionPersonas mGestionPersonas;

        /// <summary>
        /// Gestor de organizaciones
        /// </summary>
        private GestionOrganizaciones mGestorOrganizaciones;

        /// <summary>
        /// Gestor de usuarios
        /// </summary>
        private GestionUsuarios mGestionUsuarios;

        /// <summary>
        /// Gestor documental
        /// </summary>
        private GestorDocumental mGestorDocumental;

        private DataWrapperProyecto mDataWrapperProyecto;

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataWrapperProyecto">Dataset de proyecto</param>
        public GestionProyecto(DataWrapperProyecto dataWrapperProyecto, LoggingService loggingService, EntityContext entityContext) 
            : base(dataWrapperProyecto, loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;

            RecargarProyectos();
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestionProyecto(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
            //mLoggingService = loggingService;
            //mEntityContext = entityContext;

            mParametroGeneralDS = (GestorParametroGeneral)info.GetValue("ParametroGeneralDS", typeof(GestorParametroGeneral));
            mGestorOrganizaciones = (GestionOrganizaciones)info.GetValue("GestionOrganizaciones", typeof(GestionOrganizaciones));
            mGestionPersonas = (GestionPersonas)info.GetValue("GestionPersonas", typeof(GestionPersonas));
            mGestionTesauro = (GestionTesauro)info.GetValue("GestionTesauro", typeof(GestionTesauro));
            mGestionUsuarios = (GestionUsuarios)info.GetValue("GestionUsuarios", typeof(GestionUsuarios));
            mGestorDocumental = (GestorDocumental)info.GetValue("GestorDocumental", typeof(GestorDocumental));

            RecargarProyectos();
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Crea una nueva fila de proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pProyectoSuperior">Identificador del proyecto superior</param>
        /// <returns>Nueva fila de proyecto</returns>
        public AD.EntityModel.Models.ProyectoDS.Proyecto NuevaFilaProyecto(Guid pOrganizacionID, Guid pProyectoSuperior)
        {
            //Valores por defecto para "FilaProyecto"luego con el asistente de creación se modificarán
            AD.EntityModel.Models.ProyectoDS.Proyecto filaProyecto = new AD.EntityModel.Models.ProyectoDS.Proyecto();
            filaProyecto.ProyectoID = Guid.NewGuid();
            filaProyecto.OrganizacionID = pOrganizacionID;
            filaProyecto.Nombre = "<Nuevo proyecto>";
            filaProyecto.NombreCorto = string.Empty;
            filaProyecto.Descripcion = string.Empty;
            filaProyecto.Estado = (short)EstadoProyecto.Definicion;
            filaProyecto.EsProyectoDestacado = false;
            filaProyecto.FechaFin = null;
            filaProyecto.FechaInicio = DateTime.Now;
            filaProyecto.NumeroArticulos = 0;
            filaProyecto.NumeroDafos = 0;
            filaProyecto.NumeroForos = 0;
            filaProyecto.NumeroMiembros = 0;
            filaProyecto.NumeroOrgRegistradas = 0;
            filaProyecto.NumeroRecursos = 0;
            filaProyecto.NumeroPreguntas = 0;
            filaProyecto.NumeroDebates = 0;

            if (pProyectoSuperior != Guid.Empty)
            {
                filaProyecto.ProyectoSuperiorID = pProyectoSuperior;
            }
            filaProyecto.TipoAcceso = (short)TipoAcceso.Privado;
            filaProyecto.TipoProyecto = (short)TipoProyecto.DeOrganizacion;

            filaProyecto.TieneTwitter = false;
            filaProyecto.EnviarTwitterComentario = false;
            filaProyecto.EnviarTwitterNuevaCat = false;
            filaProyecto.EnviarTwitterNuevaPolitCert = false;
            filaProyecto.EnviarTwitterNuevoAdmin = false;
            filaProyecto.EnviarTwitterNuevoTipoDoc = false;
            filaProyecto.TagTwitterGnoss = true;
            //DataWrapperProyecto.Proyecto.FindByOrganizacionIDProyectoID(filaProyecto.OrganizacionID, filaProyecto.ProyectoID)
            //if (DataWrapperProyecto.ListaProyecto.Where(proyecto=>proyecto.OrganizacionID.Equals(filaProyecto.OrganizacionID) && proyecto.ProyectoID.Equals(filaProyecto.ProyectoID)) == null)
            //{
            //    DataWrapperProyecto.ListaProyecto.Add(filaProyecto);
            //    mEntityContext.Proyecto.Add(filaProyecto);

            //}
            return filaProyecto;
        }

        /// <summary>
        /// Crea una nueva fila de parámetros de proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Nueva fila de parámetros de proyecto</returns>
        public ParametroGeneral NuevaFilaParametros(Guid pOrganizacionID, Guid pProyectoID)
        {
            //Valores por defecto para "FilaParametros", luego con el asistente de creación se modificarán
            ParametroGeneral filaParametros = new ParametroGeneral();
            filaParametros.OrganizacionID = pOrganizacionID;
            filaParametros.ProyectoID = pProyectoID;
            //filaParametros.SetAvisoLegalNull();
            filaParametros.AvisoLegal = null;
            //filaParametros.SetMensajeBienvenidaNull();
            filaParametros.MensajeBienvenida = null;
            filaParametros.WikiDisponible = true;
            filaParametros.BaseRecursosDisponible = true;
            filaParametros.CompartirRecursosPermitido = true;
            filaParametros.ServicioSuscripcionDisp = true;
            filaParametros.EntidadRevisadaObligatoria = false;
            filaParametros.InvitacionesDisponibles = false;
            filaParametros.BlogsDisponibles = false;
            filaParametros.ForosDisponibles = false;
            filaParametros.EncuestasDisponibles = false;
            filaParametros.VotacionesDisponibles = true;
            filaParametros.PlataformaVideoDisponible = (short)PlataformaVideoDisponible.Ninguna;
            filaParametros.VerVotaciones = false;
            //filaParametros.SetLogoProyectoNull();
            filaParametros.LogoProyecto = null;
            filaParametros.NombreAmenazaDafoGF = "Amenaza";
            filaParametros.NombreAmenazaDafoObj = "Amenaza";
            filaParametros.NombreAmenazaDafoProc = "Amenaza";
            filaParametros.NombreDebilidadDafoGF = "Debilidad";
            filaParametros.NombreDebilidadDafoObj = "Debilidad";
            filaParametros.NombreDebilidadDafoProc = "Debilidad";
            filaParametros.NombreFortalezaDafoGF = "Fortaleza";
            filaParametros.NombreFortalezaDafoObj = "Fortaleza";
            filaParametros.NombreFortalezaDafoProc = "Fortaleza";
            filaParametros.NombreOportunidadDafoGF = "Oportunidad";
            filaParametros.NombreOportunidadDafoObj = "Oportunidad";
            filaParametros.NombreOportunidadDafoProc = "Oportunidad";
            filaParametros.DesviacionAdmitidaEnEvalua = 0;
            filaParametros.MetaAutomatPropietarioPro = 0;
            filaParametros.UmbralDetPropietariosGF = 20;
            filaParametros.UmbralDetPropietariosObj = 20;
            filaParametros.UmbralDetPropietariosProc = 20;
            filaParametros.UmbralSuficienciaEnMejora = 20;
            //filaParametros.SetPoliticaCertificacionNull();
            filaParametros.PoliticaCertificacion = null;
            filaParametros.PermitirCertificacionRec = false;
            filaParametros.PermitirRevisionManualComp = false;
            filaParametros.PermitirRevisionManualGF = false;
            filaParametros.PermitirRevisionManualObj = false;
            filaParametros.PermitirRevisionManualPro = false;
            //filaParametros.SetCoordenadasHomeNull();
            //filaParametros.SetImagenHomeGrandeNull();
            //filaParametros.SetImagenHomeNull();
            //filaParametros.SetImagenPersonalizadaPequeNull();
            //filaParametros.SetPoliticaCertificacionNull();
            //filaParametros.SetRutaImagenesTemaNull();
            //filaParametros.SetRutaTemaNull();
            filaParametros.CoordenadasHome = null;
            filaParametros.ImagenHomeGrande = null;
            filaParametros.ImagenHome = null;
            filaParametros.ImagenPersonalizadaPeque = null;
            filaParametros.PoliticaCertificacion = null;
            filaParametros.RutaImagenesTema = null;
            filaParametros.RutaTema = null;
            filaParametros.ComentariosDisponibles = true;
            filaParametros.NombreImagenPeque = "peque";
            filaParametros.DafoDisponible = false;
            filaParametros.PlantillaDisponible = false;
            filaParametros.PreguntasDisponibles = false;
            filaParametros.EncuestasDisponibles = false;
            filaParametros.DebatesDisponibles = false;
            filaParametros.ClausulasRegistro = false;
            filaParametros.OcultarPersonalizacion = false;
            filaParametros.RdfDisponibles = false;
            filaParametros.RssDisponibles = false;
            filaParametros.CargasMasivasDisponibles = false;
            filaParametros.InvitacionesPorContactoDisponibles = true;
            filaParametros.PermitirRecursosPrivados = true;

            ParametroGeneralDS.ListaParametroGeneral.Add(filaParametros);
            mEntityContext.ParametroGeneral.Add(filaParametros);

            return filaParametros;
        }

        /// <summary>
        /// Crea una nueva fila de campos de análisis de completitud y la añade al dataset
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pProyectoLibroSuperior">Proyecto elegido para heredar el mapa de competencias o NULL en caso contrario</param>
        /// <returns>Nueva fila de campos de análisis de completitud</returns>
        //public ParametroGeneralDSEspacio.CamposAnalisisCompletitudRow CrearFilaCamposAnalisisCompletitud(Guid pOrganizacionID, Guid pProyectoID, Proyecto pProyectoLibroSuperior)
        //{
        //    ParametroGeneralDSEspacio.CamposAnalisisCompletitudRow filaCampos = ParametroGeneralDS.CamposAnalisisCompletitud.NewCamposAnalisisCompletitudRow();

        //    filaCampos.OrganizacionID = pOrganizacionID;
        //    filaCampos.ProyectoID = pProyectoID;
        //    filaCampos.AnalisisCompObligatorioPrc = false;
        //    filaCampos.FraseGnossNombreProceso = false;
        //    filaCampos.ColorProceso = false;
        //    filaCampos.AlcanceProceso = false;
        //    filaCampos.DescripcionProceso = false;
        //    filaCampos.ParticipantesProceso = false;
        //    filaCampos.FinalidadProceso = false;
        //    filaCampos.DocumentacionProceso = false;
        //    filaCampos.AnalisisCompObligatorioObj = false;
        //    filaCampos.FraseGnossNombreObjetivo = false;
        //    filaCampos.ColorObjetivo = false;
        //    filaCampos.AlcanceObjetivo = false;
        //    filaCampos.DescripcionObjetivo = false;
        //    filaCampos.ParticipantesObjetivo = false;
        //    filaCampos.FinalidadObjetivo = false;
        //    filaCampos.DocumentacionObjetivo = false;
        //    filaCampos.AnalisisCompObligatorioGF = false;
        //    filaCampos.FraseGnossGF = false;
        //    filaCampos.ColorGF = false;
        //    filaCampos.AlcanceGF = false;
        //    filaCampos.DescripcionGF = false;
        //    filaCampos.ParticipantesGF = false;
        //    filaCampos.FinalidadGF = false;
        //    filaCampos.DocumentacionGF = false;
        //    filaCampos.AnalisisCompObligatorioCmp = false;
        //    filaCampos.NombreCompetencia = false;
        //    filaCampos.DescripcionCompetencia = false;
        //    filaCampos.CompVinculadasCompetencia = false;
        //    filaCampos.DocVinculadosCompetencia = false;
        //    filaCampos.EntradasCompetencia = false;
        //    filaCampos.SalidasCompetencia = false;
        //    filaCampos.ControlesCompetencia = false;
        //    filaCampos.MecanismosCompetencia = false;
        //    filaCampos.EscalaMetasCompetencia = false;

        //    if (pProyectoLibroSuperior != null)
        //    {
        //        // David: Se ha elegido heredar el libro, por tanto la configuración de competencias se debe heredar también

        //        ParametroGeneralDSEspacio.CamposAnalisisCompletitudRow filaCamposProyectoSuperior = ParametroGeneralDS.CamposAnalisisCompletitud.FindByOrganizacionIDProyectoID(pProyectoLibroSuperior.FilaProyecto.OrganizacionID, pProyectoLibroSuperior.Clave);

        //        filaCampos.AnalisisCompObligatorioCmp = filaCamposProyectoSuperior.AnalisisCompObligatorioCmp;
        //        filaCampos.NombreCompetencia = filaCamposProyectoSuperior.NombreCompetencia;
        //        filaCampos.DescripcionCompetencia = filaCamposProyectoSuperior.DescripcionCompetencia;
        //        filaCampos.CompVinculadasCompetencia = filaCamposProyectoSuperior.CompVinculadasCompetencia;
        //        filaCampos.DocVinculadosCompetencia = filaCamposProyectoSuperior.DocVinculadosCompetencia;
        //        filaCampos.EntradasCompetencia = filaCamposProyectoSuperior.EntradasCompetencia;
        //        filaCampos.SalidasCompetencia = filaCamposProyectoSuperior.SalidasCompetencia;
        //        filaCampos.ControlesCompetencia = filaCamposProyectoSuperior.ControlesCompetencia;
        //        filaCampos.MecanismosCompetencia = filaCamposProyectoSuperior.MecanismosCompetencia;
        //        filaCampos.EscalaMetasCompetencia = filaCamposProyectoSuperior.EscalaMetasCompetencia;
        //    }
        //    ParametroGeneralDS.CamposAnalisisCompletitud.AddCamposAnalisisCompletitudRow(filaCampos);

        //    return filaCampos;
        //}


        /// <summary>
        /// Cierra el proyecto estando activo unos días de gracia, cuando teminan dichos días un servivio lo cierra definitivamente
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDiasDeGracia">Número de días de gracia</param>
        public void CerrarandoProyecto(Guid pProyectoID, int pDiasDeGracia)
        {
            Guid organizacionID = this.DataWrapperProyectos.ListaProyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).FirstOrDefault().OrganizacionID;//.Select("ProyectoID ='" + pProyectoID + "'")[0]["OrganizacionID"];

            if (this.ListaProyectos.ContainsKey(pProyectoID))
            {
                DateTime fechaActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                ProyectoCerrandose proyectoCerrandose = new ProyectoCerrandose();
                proyectoCerrandose.OrganizacionID = organizacionID;
                proyectoCerrandose.ProyectoID = pProyectoID;
                proyectoCerrandose.FechaCierre = fechaActual;
                proyectoCerrandose.PeriodoDeGracia = pDiasDeGracia;
                this.DataWrapperProyectos.ListaProyectoCerrandose.Add(proyectoCerrandose);
                mEntityContext.ProyectoCerrandose.Add(proyectoCerrandose);
                ListaProyectos[pProyectoID].Estado = (short)EstadoProyecto.Cerrandose;
                this.Notificar(new MensajeObservador(AccionesObservador.Recargar, ListaProyectos[pProyectoID]));
            }
        }

        /// <summary>
        /// Cierra un proyecto temporalmente (pero no lo elimina) para que los usuarios no puedan acceder a él
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pMotivo">Motivo del cierre temporal</param>
        /// <param name="pFechaReapertura">Fecha de reapertura del proyecto</param>
        public void CerrarProyectoTemporalmente(Guid pProyectoID, string pMotivo, DateTime pFechaReapertura)
        {
            Guid organizacionID = this.DataWrapperProyectos.ListaProyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).FirstOrDefault().OrganizacionID;//.Select("ProyectoID ='" + pProyectoID + "'")[0]["OrganizacionID"];

            if (this.ListaProyectos.ContainsKey(pProyectoID))
            {
                DateTime fechaActual = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                ProyectoCerradoTmp proyectoCerradotmp = new ProyectoCerradoTmp();
                proyectoCerradotmp.OrganizacionID = organizacionID;
                proyectoCerradotmp.ProyectoID = pProyectoID;
                proyectoCerradotmp.Motivo = pMotivo;
                proyectoCerradotmp.FechaCierre = fechaActual;
                proyectoCerradotmp.FechaReapertura = pFechaReapertura;
                this.DataWrapperProyectos.ListaProyectoCerradoTmp.Add(proyectoCerradotmp);
                ListaProyectos[pProyectoID].Estado = (short)EstadoProyecto.CerradoTemporalmente;
                mEntityContext.ProyectoCerradoTmp.Add(proyectoCerradotmp);
                this.Notificar(new MensajeObservador(AccionesObservador.Recargar, ListaProyectos[pProyectoID]));
            }
        }

        /// <summary>
        /// Abre el proyecto a los ususarios
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        public void AbrirProyecto(Guid pProyectoID)
        {
            //Guid organizacionID = (Guid)this.DataWrapperProyecto.Proyecto.Select("ProyectoID ='" + pProyectoID + "'")[0]["OrganizacionID"];
            Guid organizacionID = this.DataWrapperProyectos.ListaProyecto.Find(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).OrganizacionID;
            if (this.ListaProyectos.ContainsKey(pProyectoID))
            {
                //Estaba Cerrandose
                if (ListaProyectos[pProyectoID].Estado == (short)EstadoProyecto.Cerrandose)
                {
                    ProyectoCerrandose proyectosCerrandose = this.DataWrapperProyectos.ListaProyectoCerrandose.Find(proyectoCerrandose => proyectoCerrandose.OrganizacionID.Equals(organizacionID) && proyectoCerrandose.ProyectoID.Equals(pProyectoID));
                    if (proyectosCerrandose != null)
                    {
                        this.DataWrapperProyectos.DeleteProyectoCerrandose(proyectosCerrandose);
                        mEntityContext.EliminarElemento(proyectosCerrandose);
                    }
                    ListaProyectos[pProyectoID].Estado = (short)EstadoProyecto.Abierto;
                    this.Notificar(new MensajeObservador(AccionesObservador.Recargar, ListaProyectos[pProyectoID]));

                } //Estaba CerradoTemporalmente
                else if (ListaProyectos[pProyectoID].Estado == (short)EstadoProyecto.CerradoTemporalmente)
                {
                    ProyectoCerradoTmp proyectosCerradosTmp = this.DataWrapperProyectos.ListaProyectoCerradoTmp.Find(proyectoCerradoTmp => proyectoCerradoTmp.OrganizacionID.Equals(organizacionID) && proyectoCerradoTmp.ProyectoID.Equals(pProyectoID));
                    if (proyectosCerradosTmp != null)
                    {
                        this.DataWrapperProyectos.DeleteProyectoCerradoTmp(proyectosCerradosTmp);
                        mEntityContext.EliminarElemento(proyectosCerradosTmp);
                    }
                    ListaProyectos[pProyectoID].Estado = (short)EstadoProyecto.Abierto;
                    this.Notificar(new MensajeObservador(AccionesObservador.Recargar, ListaProyectos[pProyectoID]));

                } //Estaba Definicion
                else if (ListaProyectos[pProyectoID].Estado == (short)EstadoProyecto.Definicion)
                {
                    ListaProyectos[pProyectoID].Estado = (short)EstadoProyecto.Abierto;
                    this.Notificar(new MensajeObservador(AccionesObservador.Recargar, ListaProyectos[pProyectoID]));
                } //Estaba Cerrado
                else if (ListaProyectos[pProyectoID].Estado == (short)EstadoProyecto.Cerrado)
                {
                    ListaProyectos[pProyectoID].Estado = (short)EstadoProyecto.Abierto;
                    this.Notificar(new MensajeObservador(AccionesObservador.Recargar, ListaProyectos[pProyectoID]));
                }
            }
        }

        /// <summary>
        /// Carga los proyectos del gestor
        /// </summary>
        public void RecargarProyectos()
        {
            if (mListaProyectos == null)
            {
                mListaProyectos = new Dictionary<Guid, Proyecto>();
            }

            foreach (AD.EntityModel.Models.ProyectoDS.Proyecto filaProyecto in DataWrapperProyectos.ListaProyecto)
            {
                Proyecto proyecto = new Proyecto(filaProyecto, this, mLoggingService, mEntityContext);

                if (!proyecto.EstaEliminado && !mListaProyectos.ContainsKey(proyecto.Clave))
                {
                    mListaProyectos.Add(proyecto.Clave, proyecto);
                }
            }
        }

        /// <summary>
        /// Añade a un usuario como administrador del proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        public void AgregarAdministradorDeProyecto(Guid pUsuarioID, Guid pOrganizacionID, Guid pProyectoID)
        {
            AdministradorProyecto filaAdminPro = new AdministradorProyecto();

            filaAdminPro.UsuarioID = pUsuarioID;
            filaAdminPro.OrganizacionID = pOrganizacionID;
            filaAdminPro.ProyectoID = pProyectoID;
            filaAdminPro.Tipo = (short)TipoRolUsuario.Administrador;
            this.DataWrapperProyectos.ListaAdministradorProyecto.Add(filaAdminPro);
            mEntityContext.AdministradorProyecto.Add(filaAdminPro);
        }

        /// <summary>
        /// Hace que un usuario deje de ser administrador del proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        public void EliminarAdministradorDeProyecto(Guid pUsuarioID, Guid pOrganizacionID, Guid pProyectoID)
        {
            AdministradorProyecto filaAdminPro = DataWrapperProyectos.ListaAdministradorProyecto.Find(adminProyecto => adminProyecto.OrganizacionID.Equals(pOrganizacionID) && adminProyecto.ProyectoID.Equals(pProyectoID) && adminProyecto.UsuarioID.Equals(pUsuarioID) && adminProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador));//FindByOrganizacionIDProyectoIDUsuarioIDTipo(pOrganizacionID, pProyectoID, pUsuarioID, (short)TipoRolUsuario.Administrador);

            if (filaAdminPro != null)
            {
                DataWrapperProyectos.ListaAdministradorProyecto.Remove(filaAdminPro);
                mEntityContext.EliminarElemento(filaAdminPro);
            }
        }

        /// <summary>
        /// Devuelve una lista de nombres de categorías de tesauro a las que está vinculado un proyecto en función del idioma
        /// </summary>
        /// <param name="pCategoriasTesauroAgProyecto">Conjunto de filas de vinculaciones de categorías de tesauro con proyecto</param>
        /// <returns>Lista de nombres de categorías de tesauro</returns>
        public List<string> ListaCategoriasProyecto(List<ProyectoAgCatTesauro> pCategoriasTesauroAgProyecto, string pIdioma)
        {
            List<string> lista = new List<string>();

            foreach (ProyectoAgCatTesauro filaCategoria in pCategoriasTesauroAgProyecto)
            {
                if (GestionTesauro.ListaCategoriasTesauro.ContainsKey(filaCategoria.CategoriaTesauroID))
                {
                    string nombreCategoria = GestionTesauro.ListaCategoriasTesauro[filaCategoria.CategoriaTesauroID].Nombre[pIdioma];

                    if (!lista.Contains(nombreCategoria))
                    {
                        lista.Add(nombreCategoria);
                    }
                }
            }
            return lista;
        }

        /// <summary>
        /// Elimina/Expulsa a un usuario con una identidad de un proyecto (comprobar antes que no sea administrador del mismo)
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario que se expulsa</param>
        /// <param name="pProyectoID">Identificador del proyecto del que hay que expulsar al usuario</param>
        /// <param name="pOrganizacionGnossID">Identificador de la organización creadora del proyecto</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario en dicho proyecto</param>
        /// <param name="pGestorUsuarios">Gestor de usuarios</param>
        /// <param name="pGestorIdentidades">Gestor de identidades</param>
        public void EliminarUsuarioDeProyecto(Guid pUsuarioID, Guid pProyectoID, Guid pOrganizacionGnossID, Guid pIdentidadID, GestionUsuarios pGestorUsuarios, GestionIdentidades pGestorIdentidades)
        {
            Guid UsuarioID = pUsuarioID;
            Guid IdentidadID = pIdentidadID;

            //Elimino ProyRolUsuClausulaReg
            List<ProyRolUsuClausulaReg> listaAuxiliarProyRolClau = pGestorUsuarios.DataWrapperUsuario.ListaProyRolUsuClausulaReg.Where(item => item.UsuarioID.Equals(pUsuarioID) && item.ProyectoID.Equals(pProyectoID) && item.OrganizacionID.Equals(pOrganizacionGnossID)).ToList();
            foreach (ProyRolUsuClausulaReg filaProyRolClau in listaAuxiliarProyRolClau)
            {
                pGestorUsuarios.DataWrapperUsuario.ListaProyRolUsuClausulaReg.Remove(filaProyRolClau);
                mEntityContext.EliminarElemento(filaProyRolClau);
            }

            //Elimino ProyectorRolUsuario
            ProyectoRolUsuario filaProyRolUsu = pGestorUsuarios.DataWrapperUsuario.ListaProyectoRolUsuario.FirstOrDefault(proyRolUs => proyRolUs.OrganizacionGnossID.Equals(pOrganizacionGnossID) && proyRolUs.ProyectoID.Equals(pProyectoID) && proyRolUs.UsuarioID.Equals(UsuarioID));
            if (filaProyRolUsu != null)
            {
                mEntityContext.EliminarElemento(filaProyRolUsu);
                pGestorUsuarios.DataWrapperUsuario.ListaProyectoRolUsuario.Remove(filaProyRolUsu);
            }
            ProyectoUsuarioIdentidad proyectoUsuarioIdentidad = pGestorUsuarios.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.FirstOrDefault(proyUsIden => proyUsIden.IdentidadID.Equals(IdentidadID) && proyUsIden.UsuarioID.Equals(UsuarioID) && proyUsIden.OrganizacionGnossID.Equals(pOrganizacionGnossID) && proyUsIden.ProyectoID.Equals(pProyectoID));
            if (proyectoUsuarioIdentidad != null)
            {
                //Elimino de ProyectoUsuarioIdentidad
                mEntityContext.EliminarElemento(proyectoUsuarioIdentidad);
                pGestorUsuarios.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.Remove(proyectoUsuarioIdentidad);
            }

            //Si su InicioSesion erá a este proyecto lo borro
            if (pGestorUsuarios.DataWrapperUsuario.ListaInicioSesion.Where(item => item.UsuarioID.Equals(UsuarioID) && item.ProyectoID.Equals(pProyectoID)).Count() > 0)
            {
                InicioSesion inicioSesion = pGestorUsuarios.DataWrapperUsuario.ListaInicioSesion.Where(item => item.UsuarioID.Equals(UsuarioID) && item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
                pGestorUsuarios.DataWrapperUsuario.ListaInicioSesion.Remove(inicioSesion);
                mEntityContext.EliminarElemento(inicioSesion);
            }

            //Elimino la relación con los grupos de permisos del proyecto que pueda tener
            foreach (ProyectoRolGrupoUsuario filaProyectoRolGrupoUsuario in pGestorUsuarios.DataWrapperUsuario
                .ListaProyectoRolGrupoUsuario.Where(item => item.ProyectoID.Equals(pProyectoID)))
            {
                Guid GrupoUsuarioID = filaProyectoRolGrupoUsuario.GrupoUsuarioID;

                if (pGestorUsuarios.DataWrapperUsuario.ListaGrupoUsuarioUsuario.Where(item => item.UsuarioID.Equals(UsuarioID) && item.GrupoUsuarioID.Equals(GrupoUsuarioID)) != null)
                {
                    GrupoUsuarioUsuario grupoUsuarioUsuario = pGestorUsuarios.DataWrapperUsuario.ListaGrupoUsuarioUsuario.Where(item => item.UsuarioID.Equals(UsuarioID) && item.GrupoUsuarioID.Equals(GrupoUsuarioID)).FirstOrDefault();
                    mEntityContext.EliminarElemento(grupoUsuarioUsuario);
                    pGestorUsuarios.DataWrapperUsuario.ListaGrupoUsuarioUsuario.Remove(grupoUsuarioUsuario);
                }
            }
            //Actualizo el HistoricoProyectoUsuario
            DateTime fechaSalida = DateTime.Now;

            if (pGestorUsuarios.DataWrapperUsuario.ListaHistoricoProyectoUsuario.Where(item => item.UsuarioID.Equals(UsuarioID) && item.OrganizacionGnossID.Equals(pOrganizacionGnossID) && item.ProyectoID.Equals(pProyectoID) && item.IdentidadID.Equals(IdentidadID) && !item.FechaSalida.HasValue).Count() > 0)
            {
                HistoricoProyectoUsuario filaHistoricoProyectoUsuario = pGestorUsuarios.DataWrapperUsuario.ListaHistoricoProyectoUsuario.Where(item => item.UsuarioID.Equals(UsuarioID) && item.OrganizacionGnossID.Equals(pOrganizacionGnossID) && item.ProyectoID.Equals(pProyectoID) && item.IdentidadID.Equals(IdentidadID) && !item.FechaSalida.HasValue).FirstOrDefault();
                filaHistoricoProyectoUsuario.FechaSalida = fechaSalida;
            }

            //Actualizao Identidad
            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = pGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Find(identidad => identidad.IdentidadID.Equals(IdentidadID));
            filaIdentidad.FechaBaja = fechaSalida;

            //Elimino de AdministradorProyecto (solo editores ya que  nunca EliminarUsuarioDeProyecto si es administrador del mismo)
            AdministradorProyecto administradorProyecto = this.DataWrapperProyectos.ListaAdministradorProyecto.Find(adminProy => adminProy.OrganizacionID.Equals(pOrganizacionGnossID) && adminProy.ProyectoID.Equals(pProyectoID) && adminProy.UsuarioID.Equals(UsuarioID) && adminProy.Tipo.Equals((short)TipoRolUsuario.Supervisor));
            if (administradorProyecto != null)
            {
                //this.DataWrapperProyecto.ListaAdministradorProyecto.FindByOrganizacionIDProyectoIDUsuarioIDTipo(pOrganizacionGnossID, pProyectoID, UsuarioID, (short)TipoRolUsuario.Supervisor).Delete();
                this.DataWrapperProyectos.DeleteAdministradorProyecto(administradorProyecto);
            }
        }

        /// <summary>
        /// Elimina/Expulsa a un usuario con una identidad de MYGNOSS (comprobar antes q no sea administrador del mismo) 
        /// NOTA: NO SE ELIMINAN SU FILA DE PERMISOS "ProyectoRolUsuario" para los permisos en MYGNOSS
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario que hay que expulsar</param>
        /// <param name="pProyectoID">Identificador del proyecto del que hay que expulsar al usuario</param>
        /// <param name="pOrganizacionGnossID">Identificador de la organización creadora del proyecto</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario en dicho proyecto</param>
        /// <param name="pGestorUsuarios">Gestor de usuarios</param>
        /// <param name="pGestosIdentidades">Gestor de identidades</param>
        public void EliminarUsuarioDeMyGnoss(Guid pUsuarioID, Guid pProyectoID, Guid pOrganizacionGnossID, Guid pIdentidadID, GestionUsuarios pGestorUsuarios, GestionIdentidades pGestosIdentidades)
        {
            Guid UsuarioID = pUsuarioID;
            Guid IdentidadID = pIdentidadID;

            //No se elimina la fila de permisos porque puede haber otra identidad en mygnoss de dicho usuario            
            //Elimino ProyectorRolUsuario
            //pGestorUsuarios.UsuarioDS.ProyectoRolUsuario.FindByOrganizacionGnossIDProyectoIDUsuarioID(pOrganizacionGnossID, pProyectoID, UsuarioID).Delete();

            //Elimino de ProyectoUsuarioIdentidad
            AD.EntityModel.Models.UsuarioDS.ProyectoUsuarioIdentidad proyectoUsuarioIdentidad = pGestorUsuarios.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.FirstOrDefault(proyUsIden => proyUsIden.IdentidadID.Equals(IdentidadID) && proyUsIden.UsuarioID.Equals(UsuarioID) && proyUsIden.OrganizacionGnossID.Equals(pOrganizacionGnossID) && proyUsIden.ProyectoID.Equals(pProyectoID));
            if (proyectoUsuarioIdentidad != null)
            {
                //Elimino de ProyectoUsuarioIdentidad
                mEntityContext.EliminarElemento(proyectoUsuarioIdentidad);
                pGestorUsuarios.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.Remove(proyectoUsuarioIdentidad);
            }

            //Si su InicioSesion erá a este proyecto lo borro
            if (pGestorUsuarios.DataWrapperUsuario.ListaInicioSesion.Where(item => item.UsuarioID.Equals(UsuarioID) && item.ProyectoID.Equals(pProyectoID)).Count() > 0)
            {
                InicioSesion inicioSesion = pGestorUsuarios.DataWrapperUsuario.ListaInicioSesion.Where(item => item.UsuarioID.Equals(UsuarioID) && item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
                mEntityContext.EliminarElemento(inicioSesion);
                pGestorUsuarios.DataWrapperUsuario.ListaInicioSesion.Remove(inicioSesion);
            }

            //Elimino la relación con los grupos de permisos del proyecto que pueda tener
            foreach (ProyectoRolGrupoUsuario filaProyectoRolGrupoUsuario in pGestorUsuarios.DataWrapperUsuario.ListaProyectoRolGrupoUsuario.Where(item => item.ProyectoID.Equals(pProyectoID)))
            {
                Guid GrupoUsuarioID = filaProyectoRolGrupoUsuario.GrupoUsuarioID;

                if (pGestorUsuarios.DataWrapperUsuario.ListaGrupoUsuarioUsuario.Where(item => item.UsuarioID.Equals(UsuarioID) && item.GrupoUsuarioID.Equals(GrupoUsuarioID)) != null)
                {
                    GrupoUsuarioUsuario grupoUsuarioUsuario = pGestorUsuarios.DataWrapperUsuario.ListaGrupoUsuarioUsuario.Where(item => item.UsuarioID.Equals(UsuarioID) && item.GrupoUsuarioID.Equals(GrupoUsuarioID)).FirstOrDefault();
                    pGestorUsuarios.DataWrapperUsuario.ListaGrupoUsuarioUsuario.Remove(grupoUsuarioUsuario);
                    mEntityContext.GrupoUsuarioUsuario.Remove(grupoUsuarioUsuario);
                }
            }
            //Actualizo el HistoricoProyectoUsuario
            DateTime fechaSalida = DateTime.Now;

            if (pGestorUsuarios.DataWrapperUsuario.ListaHistoricoProyectoUsuario.Where(item => item.UsuarioID.Equals(UsuarioID) && item.OrganizacionGnossID.Equals(pOrganizacionGnossID) && item.ProyectoID.Equals(pProyectoID) && item.IdentidadID.Equals(pIdentidadID) && !item.FechaSalida.HasValue).Count() > 0)
            {
                HistoricoProyectoUsuario filaHistoricoProyectoUsuario = pGestorUsuarios.DataWrapperUsuario.ListaHistoricoProyectoUsuario.Where(item => item.UsuarioID.Equals(UsuarioID) && item.OrganizacionGnossID.Equals(pOrganizacionGnossID) && item.ProyectoID.Equals(pProyectoID) && item.IdentidadID.Equals(pIdentidadID) && !item.FechaSalida.HasValue).FirstOrDefault();
                filaHistoricoProyectoUsuario.FechaSalida = fechaSalida;
            }
            //Actualizao Identidad
            AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad = pGestosIdentidades.DataWrapperIdentidad.ListaIdentidad.Find(identidad => identidad.IdentidadID.Equals(IdentidadID));
            filaIdentidad.FechaBaja = fechaSalida;
        }

        /// <summary>
        /// Elimina un proyecto pasado como parámetro
        /// </summary>
        /// <param name="pElemento">Proyecto</param>
        public override void EliminarElemento(ElementoGnoss pElemento)
        {
            Proyecto proyectoBorrar = (Proyecto)pElemento;
            string filtroProyectoBorrar = "ProyectoID = '" + proyectoBorrar.Clave + "'";

            // Borrar los administradores
            List<AdministradorProyecto> listaAdminBorrar = proyectoBorrar.FilaProyecto.AdministradorProyecto.ToList();
            foreach (AdministradorProyecto filaAdminProyecto in listaAdminBorrar)//GetAdministradorProyectoRows())
            {
                proyectoBorrar.FilaProyecto.AdministradorProyecto.Remove(filaAdminProyecto);
                mEntityContext.EliminarElemento(filaAdminProyecto);
            }

            // Borrar las filas de categorias de tesauro global del proyecto
            List<ProyectoAgCatTesauro> listaProyAgBorrar = proyectoBorrar.FilaProyecto.ProyectoAgCatTesauro.ToList();
            foreach (ProyectoAgCatTesauro filaProyectoAgCatTesauro in listaProyAgBorrar)//GetProyectoAgCatTesauroRows())
            {
                proyectoBorrar.FilaProyecto.ProyectoAgCatTesauro.Remove(filaProyectoAgCatTesauro);
                mEntityContext.EliminarElemento(filaProyectoAgCatTesauro);
            }

            // Borrar filas de proyecto más activo
            //foreach (ProyectosMasActivos fila in proyectoBorrar.FilaProyecto.ProyectosMasActivos)//GetProyectosMasActivosRows())
            if (proyectoBorrar.FilaProyecto.ProyectosMasActivos != null)
            {
                ProyectosMasActivos fila = proyectoBorrar.FilaProyecto.ProyectosMasActivos;
                // David: Solo debe borrar las filas añadidas en pantalla ya que las que están en base de datos se borran desde el AD
                if (mEntityContext.Entry(fila).State == EntityState.Added)
                {
                    //fila.Delete();
                    mEntityContext.EliminarElemento(fila);
                    proyectoBorrar.FilaProyecto.ProyectosMasActivos = null;
                }
            }

            if (GestionUsuarios != null)
            {
                // Eliminar los registros del dataset UsuarioDS que dependan de proyecto

                // Inicios de sesión
                List<InicioSesion> listaAuxiliarInicioSesion = GestionUsuarios.DataWrapperUsuario.ListaInicioSesion.Where(item => item.ProyectoID.Equals(proyectoBorrar.Clave)).ToList();
                foreach (InicioSesion fila in listaAuxiliarInicioSesion)
                {
                    mEntityContext.EliminarElemento(fila);
                    GestionUsuarios.DataWrapperUsuario.ListaInicioSesion.Remove(fila);
                }

                // HistoricosProyecto de usuario
                List<HistoricoProyectoUsuario> listaAuxiliarHistoricoProyectoUsuario = GestionUsuarios.DataWrapperUsuario.ListaHistoricoProyectoUsuario.Where(item => item.ProyectoID.Equals(proyectoBorrar.Clave)).ToList();
                foreach (HistoricoProyectoUsuario fila in listaAuxiliarHistoricoProyectoUsuario)
                {
                    mEntityContext.EliminarElemento(fila);
                    GestionUsuarios.DataWrapperUsuario.ListaHistoricoProyectoUsuario.Remove(fila);
                }

                // Roles de usuario en proyecto
                List<ProyectoRolUsuario> listaAuxiliarProyectoRolUsuario = GestionUsuarios.DataWrapperUsuario.ListaProyectoRolUsuario.Where(proy => proy.ProyectoID.Equals(proyectoBorrar.Clave)).ToList();
                foreach (ProyectoRolUsuario fila in listaAuxiliarProyectoRolUsuario)
                {
                    mEntityContext.EliminarElemento(fila);
                    GestionUsuarios.DataWrapperUsuario.ListaProyectoRolUsuario.Remove(fila);
                }

                // Roles de grupos de usuario en proyecto
                List<ProyectoRolGrupoUsuario> listaAuxiliarProyectoRolGrupoUsuario = GestionUsuarios.DataWrapperUsuario.ListaProyectoRolGrupoUsuario.Where(item => item.ProyectoID.Equals(proyectoBorrar.Clave)).ToList();
                foreach (ProyectoRolGrupoUsuario fila in listaAuxiliarProyectoRolGrupoUsuario)
                {
                    mEntityContext.EliminarElemento(fila);
                    GestionUsuarios.DataWrapperUsuario.ListaProyectoRolGrupoUsuario.Remove(fila);
                }

                // Identidades de usuario en proyecto
                List<ProyectoUsuarioIdentidad> listaAuxiliarProyectoUsuarioIdentidad = GestionUsuarios.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.Where(proy => proy.ProyectoID.Equals(proyectoBorrar.Clave)).ToList();
                foreach (ProyectoUsuarioIdentidad fila in listaAuxiliarProyectoUsuarioIdentidad)
                {
                    mEntityContext.EliminarElemento(fila);
                    GestionUsuarios.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.Remove(fila);
                }

                // Eliminar los registros del dataset identidad que dependan de proyecto
                if (GestionUsuarios.GestorIdentidades != null)
                {//"ProyectoID = '" + proyectoBorrar.Clave + "'";
                    List<AD.EntityModel.Models.IdentidadDS.Identidad> listaBorrar = GestionUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.ProyectoID.Equals(proyectoBorrar.Clave)).ToList();
                    foreach (AD.EntityModel.Models.IdentidadDS.Identidad fila in listaBorrar)
                    {
                        mEntityContext.EliminarElemento(fila);
                        GestionUsuarios.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Remove(fila);
                    }
                }
            }

            if (GestionOrganizaciones != null)
            {
                List<OrganizacionParticipaProy> listaBorrarOrganizacionPartProy = GestionOrganizaciones.OrganizacionDW.ListaOrganizacionParticipaProy.Where(item => item.ProyectoID.Equals(filtroProyectoBorrar)).ToList();
                foreach (OrganizacionParticipaProy fila in listaBorrarOrganizacionPartProy)//.Select(filtroProyectoBorrar))
                {
                    // David: Solo debe borrar las filas añadidas en pantalla ya que las que están en base de datos se borran desde el AD
                    if (mEntityContext.Entry(fila).State == EntityState.Added)
                    {
                        mEntityContext.EliminarElemento(fila);
                        GestionOrganizaciones.OrganizacionDW.ListaOrganizacionParticipaProy.Remove(fila);
                    }
                }
            }

            // Borrar las filas de parametros generales del proyecto y campos de análisis completitud
            if (ParametroGeneralDS != null)
            {

                foreach (ParametroGeneral fila in ParametroGeneralDS.ListaParametroGeneral.Where(parametroGeneral => parametroGeneral.ProyectoID.Equals(proyectoBorrar.Clave)))
                {
                    ParametroGeneralDS.ListaParametroGeneral.Remove(fila);
                    //fila.Delete();
                    mEntityContext.ParametroGeneral.Remove(fila);
                }

                //foreach (ParametroGeneralDSEspacio.CamposAnalisisCompletitudRow fila in ParametroGeneralDS.CamposAnalisisCompletitud.Select(filtroProyectoBorrar))
                //{
                //    fila.Delete();
                //}
            }

            // Eliminar el tesauro global del proyecto
            if (GestionTesauro != null)
            {
                foreach (AD.EntityModel.Models.Tesauro.TesauroProyecto fila in GestionTesauro.TesauroDW.ListaTesauroProyecto.Where(item => item.ProyectoID.Equals(proyectoBorrar.Clave)))
                {
                    AD.EntityModel.Models.Tesauro.Tesauro tesauro = GestionTesauro.TesauroDW.ListaTesauro.FirstOrDefault(item => item.TesauroID.Equals(fila.TesauroID));
                    GestionTesauro.TesauroDW.ListaTesauro.Remove(tesauro);
                    mEntityContext.EliminarElemento(tesauro);

                    GestionTesauro.TesauroDW.ListaTesauroProyecto.Remove(fila);
                    mEntityContext.EliminarElemento(fila);
                }
            }

            // Eliminar la base de recursos
            if (GestorDocumental != null)
            {
                List<AD.EntityModel.Models.Documentacion.BaseRecursosProyecto> listaBaseRecursosProyectoBorrar =  GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Where(baseRec => baseRec.ProyectoID.Equals(proyectoBorrar.Clave)).ToList();
                foreach (AD.EntityModel.Models.Documentacion.BaseRecursosProyecto fila in listaBaseRecursosProyectoBorrar)
                {
                    AD.EntityModel.Models.Documentacion.BaseRecursos baseRecurso = GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursos.FirstOrDefault(baseRec => baseRec.BaseRecursosID.Equals(fila.BaseRecursosID));
                    if (baseRecurso != null)
                    {
                        GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursos.Remove(baseRecurso);
                        mEntityContext.EliminarElemento(baseRecurso);
                    }
                    mEntityContext.EliminarElemento(fila);
                    GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto.Remove(fila);
                }
            }

            // ** Nota David: Los elementos que cuelgan de proyecto y no se han cargado en memoria, se eliminan dentro del AD
            //                mediante sentencias DELETE.

            // Eliminar de la colección de hijos de su padre
            if (proyectoBorrar.Padre != null)
            {
                proyectoBorrar.Padre.Hijos.Remove(pElemento);
            }
            proyectoBorrar.Organizacion.ListaProyectosDeOrganizacionGnoss.Remove(proyectoBorrar.Clave);

            // Borrar el proyecto de las listas del gestor
            ListaProyectos.Remove(proyectoBorrar.Clave);
            mEntityContext.EliminarElemento(proyectoBorrar.FilaProyecto);
            //proyectoBorrar.FilaProyecto.Delete();

            base.EliminarElemento(pElemento);
        }

        /// <summary>
        /// Modifica los identificadores superiores de un elemento para relacionarlo con uno superior
        /// </summary>
        /// <param name="pPegado">Elemento pegado</param>
        /// <param name="pPadre">Padre</param>
        protected override void ModificarIdCortar(ElementoGnoss pPegado, ElementoGnoss pPadre)
        {
            if (pPegado is Proyecto && pPadre is Proyecto)
            {
                ((Proyecto)pPegado).FilaProyecto.ProyectoSuperiorID = pPadre.Clave;
            }

            if (pPegado.Padre is Proyecto)
            {
                ((Proyecto)pPegado.Padre).ListaSubProyectos.Remove((Proyecto)pPegado);
            }

            if (pPadre is Proyecto)
            {
                ((Proyecto)pPadre).ListaSubProyectos.Add((Proyecto)pPegado);
            }
        }

        #endregion

        #region Propiedades


        //public DataWrapperProyecto DataWrapperProyecto
        //{
        //    get
        //    {
        //        return DataWrapperProyecto;
        //    }
        //    set
        //    {
        //        DataWrapperProyecto = value;
        //    }
        //}



        /// <summary>
        /// Obtiene o establece el gestor de usuarios
        /// </summary>
        public GestionUsuarios GestionUsuarios
        {
            get
            {
                return mGestionUsuarios;
            }
            set
            {
                mGestionUsuarios = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor documental
        /// </summary>
        public GestorDocumental GestorDocumental
        {
            get
            {
                return mGestorDocumental;
            }
            set
            {
                mGestorDocumental = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el dataset de parámetros de proyecto
        /// </summary>
        public GestorParametroGeneral ParametroGeneralDS
        {
            get
            {
                return mParametroGeneralDS;
            }
            set
            {
                mParametroGeneralDS = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el dataset de parámetros de proyecto
        /// </summary>
        //public ParametroGeneralDS ParametroGeneralDSViejo
        //{
        //    get
        //    {
        //        return mParametroGeneralDS;
        //    }
        //    set
        //    {
        //        mParametroGeneralDS = value;
        //    }
        //}

        /// <summary>
        /// Obtiene la lista de proyectos
        /// </summary>
        public Dictionary<Guid, Proyecto> ListaProyectos
        {
            get
            {
                if (mListaProyectos == null)
                    RecargarProyectos();

                return mListaProyectos;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de tesauro
        /// </summary>
        public GestionTesauro GestionTesauro
        {
            get
            {
                if (mGestionTesauro != null)
                {
                    return mGestionTesauro;
                }

                return null;
            }
            set
            {
                mGestionTesauro = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de personas
        /// </summary>
        public GestionPersonas GestionPersonas
        {
            get { return mGestionPersonas; }
            set { mGestionPersonas = value; }
        }

        /// <summary>
        /// Obtiene o establece el gestor de organizaciones
        /// </summary>
        public GestionOrganizaciones GestionOrganizaciones
        {
            get
            {
                return mGestorOrganizaciones;
            }
            set
            {
                mGestorOrganizaciones = value;
            }
        }

        public DataWrapperProyecto DataWrapperProyectos
        {
            get
            {
                return (DataWrapperProyecto)DataWrapper;
            }
            set
            {
                DataWrapper = value;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~GestionProyecto()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                disposed = true;

                try
                {
                    if (disposing)
                    {
                        if (mListaProyectos != null)
                        {
                            foreach (Proyecto proy in mListaProyectos.Values)
                            {
                                proy.Dispose();
                            }
                        }
                        if (mParametroGeneralDS != null)
                        {
                            //  mParametroGeneralDS.Dispose();
                        }
                        //Libero las variables grandes
                        mParametroGeneralDS = null;
                        mListaProyectos = null;

                        this.disposed = true;
                    }
                }
                finally
                {
                    // Llamo al dispose de la clase base
                    base.Dispose(disposing);
                }
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Metodo para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("ParametroGeneralDS", ParametroGeneralDS);
            info.AddValue("GestionOrganizaciones", GestionOrganizaciones);
            info.AddValue("GestionPersonas", GestionPersonas);
            info.AddValue("GestionTesauro", GestionTesauro);
            info.AddValue("GestionUsuarios", GestionUsuarios);
            info.AddValue("GestorDocumental", GestorDocumental);
        }

        #endregion
    }
}