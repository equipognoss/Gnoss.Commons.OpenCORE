using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.Organizador.Correo;
using Es.Riam.Gnoss.AD.Organizador.Correo.Model;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.Organizador.Correo;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using Es.Riam.Util;
using IDE = Es.Riam.Gnoss.Elementos.Identidad.Identidad;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.AbstractsOpen;

namespace Es.Riam.Gnoss.Elementos.Organizador.Correo
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración que contiene los diferentes tipos de listados de correo
    /// </summary>
    public enum TipoEnvioCorreoBienvenida
    {
        Ninguno = 0,
        /// <summary>
        /// Correo a la Bandeja de mensajes de la plataforma
        /// </summary>
        CorreoInterno = 1,
        /// <summary>
        /// Correo al email del usuario
        /// </summary>
        CorreoExterno = 2,
        /// <summary>
        /// Correo a la Bandeja de mensajes de la plataforma y al email del usuario
        /// </summary>
        CorreoInternoYExterno = 3
    }

    #endregion


    /// <summary>
    /// Gestor de correo
    /// </summary>
    [Serializable]
    public class GestionCorreo : GestionGnoss, ISerializable, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Gestor de identidades
        /// </summary>
        private GestionIdentidades mGestorIdentidades;

        /// <summary>
        /// Gestor de notificaciones
        /// </summary>
        private GestionNotificaciones mGestorNotificaciones;

        /// <summary>
        /// Gestor de amigos
        /// </summary>
        private GestionAmigos mGestorAmigos;

        /// <summary>
        /// DataSet de correo recibido
        /// </summary>
        private CorreoDS mCorreoDS;

        /// <summary>
        /// Lista de los correos recibidos
        /// </summary>
        private List<Correo> mListaCorreosRecibidos;

        /// <summary>
        /// Lista de los correos enviados
        /// </summary>
        private List<Correo> mListaCorreosEnviados;

        /// <summary>
        /// Lista de los correos eliminados
        /// </summary>
        private List<Correo> mListaCorreosEliminados;

        /// <summary>
        /// Lista de los correos
        /// </summary>
        private Dictionary<Guid, Correo> mListaCorreos;

        /// <summary>
        /// Lista de correos seleccionados
        /// </summary>
        private Dictionary<Guid, Correo> mCorreosSeleccionados = new Dictionary<Guid, Correo>();

        /// <summary>
        /// Correo seleccionado
        /// </summary>
        private Guid mCorreoSeleccionadoID;

        /// <summary>
        /// Identidad conectada actualmente
        /// </summary>
        private IDE mIdentidadActual;

        private bool mEsEcosistemaProyectoConMensajesPersonalizadoBienvenida = false;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public GestionCorreo(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Constructor con parametros
        /// </summary>
        /// <param name="pCorreoDS">Dataset para los correos eliminados</param>
        /// <param name="pGestionPersonas">Gestor de personas</param>
        /// <param name="pGestorIdentidades">Gestor de identidades</param>
        /// <param name="pGestorAmigos">Gestor de amigos</param>
        public GestionCorreo(CorreoDS pCorreoDS, GestionPersonas pGestionPersonas, GestionIdentidades pGestorIdentidades, GestionAmigos pGestorAmigos, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
                : base(loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            mCorreoDS = pCorreoDS;
            mGestorIdentidades = pGestorIdentidades;
            mGestorAmigos = pGestorAmigos;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestionCorreo(SerializationInfo info, StreamingContext context, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            mCorreoDS = (CorreoDS)info.GetValue("CorreoDS", typeof(CorreoDS));
            mGestorAmigos = (GestionAmigos)info.GetValue("GestorAmigos", typeof(GestionAmigos));
            mGestorIdentidades = (GestionIdentidades)info.GetValue("GestorIdentidades", typeof(GestionIdentidades));
            mGestorNotificaciones = (GestionNotificaciones)info.GetValue("GestorNotificaciones", typeof(GestionNotificaciones));

            object idIdentidad = info.GetValue("IdentidadActual", typeof(Guid));
            if (idIdentidad != null)
            {
                mIdentidadActual = mGestorIdentidades.ListaIdentidades[(Guid)idIdentidad];
            }
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Crea un nuevo correo enviado
        /// </summary>
        /// <param name="pAutor">Identificador del autor del correo</param>
        /// <param name="pDestinatarios">Lista de identificadores de destinatarios del correo</param>
        /// <param name="pAsunto">Asunto del correo</param>
        /// <param name="pCuerpo">Cuerpo del correo</param>
        /// <param name="pUrlBase">Url base</param>
        public Guid AgregarCorreo(Guid pAutor, List<Guid> pDestinatarios, string pAsunto, string pCuerpo, string pUrlBase, Elementos.ServiciosGenerales.Proyecto pProyecto, TiposNotificacion pTiposNotificacion, string pLanguageCode, bool pEnviarDatosConTiempos = false)
        {
            //Dictionary<Guid, bool> listaDestinatios = new Dictionary<Guid, bool>();
            List<Guid> listaDestinatios = new List<Guid>();
            foreach (Guid id in pDestinatarios)
            {
                //listaDestinatios.Add(id, false);
                listaDestinatios.Add(id);
            }

            return AgregarCorreo(pAutor, listaDestinatios, pAsunto, pCuerpo, pUrlBase, TipoEnvioCorreoBienvenida.CorreoInternoYExterno, pProyecto, pTiposNotificacion, pLanguageCode, pEnviarDatosConTiempos);
        }
        /// <summary>
        /// Crea un nuevo correo enviado
        /// </summary>
        /// <param name="pAutor">Identificador del autor del correo</param>
        /// <param name="pDestinatarios">Lista de identificadores de destinatarios del correo</param>
        /// <param name="pAsunto">Asunto del correo</param>
        /// <param name="pCuerpo">Cuerpo del correo</param>
        /// <param name="pUrlBase">Url base</param>
        public Guid AgregarCorreoTutor(Guid pAutor, List<Guid> pDestinatarios, string pAsunto, string pCuerpo, string pUrlBase, Elementos.ServiciosGenerales.Proyecto pProyecto, TiposNotificacion pTiposNotificacion, string pLanguageCode, bool pEnviarDatosConTiempos = false)
        {
            Dictionary<Guid, bool> listaDestinatios = new Dictionary<Guid, bool>();
            foreach (Guid id in pDestinatarios)
            {
                listaDestinatios.Add(id, false);
            }
            Guid correoID = Guid.NewGuid();
            AgregarCorreoExterno(correoID, pAutor, listaDestinatios, pAsunto, pCuerpo, pUrlBase, TipoEnvioCorreoBienvenida.CorreoExterno, pProyecto, pTiposNotificacion, pEnviarDatosConTiempos, pLanguageCode, true);

            return correoID;
        }
        /// <summary>
        /// Crea un nuevo correo enviado
        /// </summary>
        /// <param name="pAutor">Identificador del autor del correo</param>
        /// <param name="pDestinatarios">Lista de identificadores de destinatarios del correo</param>
        /// <param name="pAsunto">Asunto del correo</param>
        /// <param name="pCuerpo">Cuerpo del correo</param>
        /// <param name="pUrlBase">Url base</param>
        public Guid AgregarCorreoTutorGrupos(Guid pAutor, Dictionary<Guid, bool> pDestinatarios, string pAsunto, string pCuerpo, string pUrlBase, Elementos.ServiciosGenerales.Proyecto pProyecto, TiposNotificacion pTiposNotificacion, string pLanguageCode, bool pEnviarDatosConTiempos = false)
        {
            Guid correoID = Guid.NewGuid();
            AgregarCorreoExterno(correoID, pAutor, pDestinatarios, pAsunto, pCuerpo, pUrlBase, TipoEnvioCorreoBienvenida.CorreoExterno, pProyecto, pTiposNotificacion, pEnviarDatosConTiempos, pLanguageCode, true);

            return correoID;
        }
        /// <summary>
        /// Crea un nuevo correo enviado
        /// </summary>
        /// <param name="pAutor">Identificador del autor del correo</param>
        /// <param name="pDestinatarios">Lista de identificadores de destinatarios del correo</param>
        /// <param name="pAsunto">Asunto del correo</param>
        /// <param name="pCuerpo">Cuerpo del correo</param>
        /// <param name="pUrlBase">Url base</param>
        public Guid AgregarCorreo(Guid pAutor, Dictionary<Guid, bool> pDestinatarios, string pAsunto, string pCuerpo, string pUrlBase, Elementos.ServiciosGenerales.Proyecto pProyecto, TiposNotificacion pTiposNotificacion, string pLanguageCode, bool pEnviarDatosConTiempos = false)
        {
            return AgregarCorreo(pAutor, pDestinatarios, pAsunto, pCuerpo, pUrlBase, TipoEnvioCorreoBienvenida.CorreoInternoYExterno, pProyecto, pTiposNotificacion, pLanguageCode, pEnviarDatosConTiempos);
        }

        /// <summary>
        /// Crea un nuevo correo enviado y lo manda si así se indica por parámetro
        /// </summary>
        /// <param name="pAutor">Identificador del autor del correo</param>
        /// <param name="pDestinatarios">Lista de identificadores de destinatarios del correo</param>
        /// <param name="pAsunto">Asunto del correo</param>
        /// <param name="pCuerpo">Cuerpo del correo</param>
        /// <param name="pUrlBase">Url base</param>
        /// <param name="pEnviarMail">TRUE si se debe enviar por mail, FALSE en caso contrario</param>
        public Guid AgregarCorreo(Guid pAutor, List<Guid> pDestinatarios, string pAsunto, string pCuerpo, string pUrlBase, TipoEnvioCorreoBienvenida pEnvioCorreoBienvenida, Elementos.ServiciosGenerales.Proyecto pProyecto, TiposNotificacion pTipoNotificacion, string pLanguageCode, bool pEnviarDatosConTiempos = false)
        {
            Dictionary<Guid, bool> listaDestinatios = new Dictionary<Guid, bool>();
            foreach (Guid id in pDestinatarios)
            {
                listaDestinatios.Add(id, false);
            }

            return AgregarCorreo(pAutor, listaDestinatios, pAsunto, pCuerpo, pUrlBase, pEnvioCorreoBienvenida, pProyecto, pTipoNotificacion, pLanguageCode, pEnviarDatosConTiempos);
        }

        /// <summary>
        /// Crea un nuevo correo enviado y lo manda si así se indica por parámetro
        /// </summary>
        /// <param name="pAutor">Identificador del autor del correo</param>
        /// <param name="pDestinatarios">Lista de identificadores de destinatarios del correo</param>
        /// <param name="pAsunto">Asunto del correo</param>
        /// <param name="pCuerpo">Cuerpo del correo</param>
        /// <param name="pUrlBase">Url base</param>
        /// <param name="pEnviarMail">TRUE si se debe enviar por mail, FALSE en caso contrario</param>
        public Guid AgregarCorreo(Guid pAutor, Dictionary<Guid, bool> pDestinatarios, string pAsunto, string pCuerpo, string pUrlBase, TipoEnvioCorreoBienvenida pEnvioCorreoBienvenida, Elementos.ServiciosGenerales.Proyecto pProyecto, TiposNotificacion pTipoNotificacion, string pLanguageCode, bool pEnviarDatosConTiempos = false)
        {
            Guid correoID = Guid.Empty;
            if (pDestinatarios.Count > 0)
            {
                switch (pEnvioCorreoBienvenida)
                {
                    case TipoEnvioCorreoBienvenida.CorreoInterno:
                        correoID = Guid.NewGuid();
                        AgregarCorreoInterno(correoID, pAutor, pDestinatarios, pAsunto, pCuerpo, pUrlBase, pEnvioCorreoBienvenida, pProyecto, pEnviarDatosConTiempos);
                        break;
                    case TipoEnvioCorreoBienvenida.CorreoExterno:
                        correoID = Guid.NewGuid();
                        AgregarCorreoExterno(correoID, pAutor, pDestinatarios, pAsunto, pCuerpo, pUrlBase, pEnvioCorreoBienvenida, pProyecto, pTipoNotificacion, pEnviarDatosConTiempos, pLanguageCode);
                        break;
                    case TipoEnvioCorreoBienvenida.CorreoInternoYExterno:
                        correoID = Guid.NewGuid();
                        AgregarCorreoInterno(correoID, pAutor, pDestinatarios, pAsunto, pCuerpo, pUrlBase, pEnvioCorreoBienvenida, pProyecto, pEnviarDatosConTiempos);
                        AgregarCorreoExterno(correoID, pAutor, pDestinatarios, pAsunto, pCuerpo, pUrlBase, pEnvioCorreoBienvenida, pProyecto, pTipoNotificacion, pEnviarDatosConTiempos, pLanguageCode);
                        break;
                }
            }

            return correoID;
        }

        private void AgregarCorreoInterno(Guid pCorreoID, Guid pAutor, Dictionary<Guid, bool> pDestinatarios, string pAsunto, string pCuerpo, string pUrlBase, TipoEnvioCorreoBienvenida pEnvioCorreoBienvenida, Elementos.ServiciosGenerales.Proyecto pProyecto, bool pEnviarDatosConTiempos)
        {
            if (mCorreoDS == null)
            {
                mCorreoDS = new CorreoDS();
            }

            List<Guid> listaPersonas = new List<Guid>();
            List<Guid> listaGrupos = new List<Guid>();
            foreach (Guid id in pDestinatarios.Keys)
            {
                if (pDestinatarios[id])
                {
                    listaGrupos.Add(id);
                }
                else
                {
                    listaPersonas.Add(id);
                }
            }

            CorreoCN correoCN = new CorreoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, string> nombresDestinos = correoCN.ObtenerNombresCorreos(listaPersonas);

            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<Guid, string> nombresDestinosGrupos = identCN.ObtenerNombresDeGrupos(listaGrupos);
            identCN.Dispose();

            string destinatariosNombres = "";
            string destinatariosID = "";

            foreach (Guid id in nombresDestinos.Keys)
            {
                destinatariosID += id.ToString() + ",";
                destinatariosNombres += nombresDestinos[id] + ",";
            }
            foreach (Guid id in nombresDestinosGrupos.Keys)
            {
                destinatariosID += "g_" + id.ToString() + ",";
                destinatariosNombres += nombresDestinosGrupos[id] + ",";
            }

            DateTime fecha = DateTime.Now;

            #region Fila enviado

            CorreoDS.CorreoInternoRow nuevaFilaCorreo = mCorreoDS.CorreoInterno.NewCorreoInternoRow();
            nuevaFilaCorreo.CorreoID = pCorreoID;
            nuevaFilaCorreo.ConversacionID = pCorreoID;
            nuevaFilaCorreo.Autor = pAutor;
            nuevaFilaCorreo.Destinatario = Guid.Empty;
            nuevaFilaCorreo.Asunto = pAsunto;
            nuevaFilaCorreo.Cuerpo = pCuerpo;
            nuevaFilaCorreo.Fecha = fecha;
            nuevaFilaCorreo.Leido = true;
            nuevaFilaCorreo.Eliminado = false;
            nuevaFilaCorreo.EnPapelera = false;
            nuevaFilaCorreo.DestinatariosID = destinatariosID;
            nuevaFilaCorreo.DestinatariosNombres = destinatariosNombres;
            nuevaFilaCorreo.ConversacionID = pCorreoID;

            mCorreoDS.CorreoInterno.AddCorreoInternoRow(nuevaFilaCorreo);

            Correo nuevoCorreoEnviado = new Correo(nuevaFilaCorreo, this, mLoggingService);
            ListaCorreosEnviados.Add(nuevoCorreoEnviado);

            #endregion

            int numCorreosDS = 0;

            #region FilaRecibido
            foreach (Guid destinatarioID in pDestinatarios.Keys)
            {
                if (!pDestinatarios[destinatarioID])
                {
                    //Si es un grupo, agregamos lo procesa el BaseUsuarios.
                    CorreoDS.CorreoInternoRow nuevaFilaCorreoRecibido = mCorreoDS.CorreoInterno.NewCorreoInternoRow();
                    nuevaFilaCorreoRecibido.CorreoID = pCorreoID;
                    nuevaFilaCorreoRecibido.ConversacionID = pCorreoID;
                    nuevaFilaCorreoRecibido.Autor = pAutor;
                    nuevaFilaCorreoRecibido.Destinatario = destinatarioID;
                    nuevaFilaCorreoRecibido.Asunto = pAsunto;
                    nuevaFilaCorreoRecibido.Cuerpo = pCuerpo;
                    nuevaFilaCorreoRecibido.Fecha = fecha;
                    nuevaFilaCorreoRecibido.Leido = false;
                    nuevaFilaCorreoRecibido.Eliminado = false;
                    nuevaFilaCorreoRecibido.EnPapelera = false;
                    nuevaFilaCorreoRecibido.DestinatariosID = destinatariosID;
                    nuevaFilaCorreoRecibido.DestinatariosNombres = destinatariosNombres;
                    nuevaFilaCorreoRecibido.ConversacionID = pCorreoID;

                    mCorreoDS.CorreoInterno.AddCorreoInternoRow(nuevaFilaCorreoRecibido);

                    Correo nuevoCorreoRecibido = new Correo(nuevaFilaCorreoRecibido, this, mLoggingService);
                    ListaCorreosRecibidos.Add(nuevoCorreoRecibido);

                    if (numCorreosDS > 100 && pEnviarDatosConTiempos)
                    {
                        correoCN.ActualizarCorreo(this.CorreoDS);
                        this.CorreoDS.Clear();
                        numCorreosDS = 0;
                        System.Threading.Thread.Sleep(1000);
                    }

                    numCorreosDS++;
                }

                if (numCorreosDS > 100 && pEnviarDatosConTiempos)
                {
                    correoCN.ActualizarCorreo(this.CorreoDS);
                    this.CorreoDS.Clear();
                    numCorreosDS = 0;
                    System.Threading.Thread.Sleep(1000);
                }

                numCorreosDS++;
            }

            if (pEnviarDatosConTiempos)
            {
                correoCN.ActualizarCorreo(this.CorreoDS);
                this.CorreoDS.Clear();
            }
            #endregion
        }

        private void AgregarCorreoExterno(Guid pCorreoID, Guid pAutor, Dictionary<Guid, bool> pDestinatarios, string pAsunto, string pCuerpo, string pUrlBase, TipoEnvioCorreoBienvenida pEnvioCorreoBienvenida, Elementos.ServiciosGenerales.Proyecto pProyecto, TiposNotificacion pTipoNotificacion, bool pEnviarDatosConTiempos, string pLanguageCode, bool pEsMensajeTutor = false)
        {
            List<Guid> listaPersonas = new List<Guid>();
            List<Guid> listaGrupos = new List<Guid>();
            foreach (Guid id in pDestinatarios.Keys)
            {
                if (pDestinatarios[id])
                {
                    listaGrupos.Add(id);
                }
                else
                {
                    listaPersonas.Add(id);
                }
            }

            IDE identidadAutor = null;
            if (GestorIdentidades.ListaIdentidades.ContainsKey(pAutor))
            {
                identidadAutor = GestorIdentidades.ListaIdentidades[pAutor].IdentidadMyGNOSS;
            }

            NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            int numCorreosDS = 0;
            foreach (Guid destinatarioID in listaPersonas)
            {
                string nombreIdentidadAutor = "";

                if (identidadAutor != null)
                {
                    nombreIdentidadAutor = identidadAutor.Nombre();
                }

                if (GestorIdentidades.ListaIdentidades.ContainsKey(destinatarioID))
                {
                    IDE identidadDestinatario = GestorIdentidades.ListaIdentidades[destinatarioID];

                    if (identidadAutor != null || EsEcosistemaProyectoConMensajesPersonalizadoBienvenida)
                    {
                        GestorNotificaciones.AgregarNotificacionAvisoNuevoCorreo(nombreIdentidadAutor, identidadDestinatario, pCuerpo, pAsunto, pTipoNotificacion, pCorreoID, pUrlBase, pProyecto, pLanguageCode, pEsMensajeTutor);
                    }
                }

                if (numCorreosDS > 100 && pEnviarDatosConTiempos)
                {
                    notificacionCN.ActualizarNotificacion();
                    System.Threading.Thread.Sleep(1000);
                    numCorreosDS = 0;
                }

                numCorreosDS++;
            }

            if (pEnviarDatosConTiempos)
            {
                notificacionCN.ActualizarNotificacion();
            }
        }

        public TipoEnvioCorreoBienvenida ObtenerTipoEnvioCorreoBienvenida(bool pEsEcosistemaSinMetaProyecto, Dictionary<string, string> pParametroProyecto)
        {
            TipoEnvioCorreoBienvenida tipoEnvioCorreo = TipoEnvioCorreoBienvenida.CorreoInternoYExterno;
            if (pEsEcosistemaSinMetaProyecto)
            {
                if (pParametroProyecto.ContainsKey(ParametroAD.TipoEnviarMensajeBienvenida))
                {
                    tipoEnvioCorreo = (TipoEnvioCorreoBienvenida)int.Parse(pParametroProyecto[ParametroAD.TipoEnviarMensajeBienvenida]);
                }
                else
                {
                    tipoEnvioCorreo = TipoEnvioCorreoBienvenida.Ninguno;
                }
            }
            return tipoEnvioCorreo;
        }

        /// <summary>
        /// Comprueba si es un ecosistema sin metaproyecto y hay mensaje personalizado de bienvenida
        /// </summary>
        /// <param name="pEsEcosistemaSinMetaProyecto"></param>
        /// <param name="pParametroProyecto"></param>
        /// <returns></returns>
        public bool EsEcosistemaMetaProyectoYTieneMensajePersonalizadoBienvenida(bool pEsEcosistemaSinMetaProyecto, Dictionary<string, string> pParametroProyecto)
        {
            bool enviarEmail = false;
            if (pEsEcosistemaSinMetaProyecto)
            {
                if (pParametroProyecto.ContainsKey(ParametroAD.TipoEnviarMensajeBienvenida))
                {
                    enviarEmail = true;
                }
            }
            return enviarEmail;
        }

        /// <summary>
        /// Obtiene el correo anterior al pasado por parámetro
        /// </summary>
        /// <param name="pCorreo">Correo</param>
        /// <param name="pLista">Listado de tipos de correo</param>
        /// <returns>Correo anterior</returns>
        public Correo AnteriorCorreo(Correo pCorreo, TiposListadoCorreo pLista)
        {
            int indice;

            switch (pLista)
            {
                case TiposListadoCorreo.Recibido:
                    {
                        indice = ListaCorreosRecibidos.IndexOf(pCorreo);

                        if (indice > 0)
                        {
                            return ListaCorreosRecibidos[indice - 1];
                        }
                        break;
                    }
                case TiposListadoCorreo.Enviado:
                    {
                        indice = ListaCorreosEnviados.IndexOf(pCorreo);

                        if (indice > 0)
                        {
                            return ListaCorreosEnviados[indice - 1];
                        }
                        break;
                    }
                case TiposListadoCorreo.Eliminado:
                    {
                        indice = ListaCorreosEliminados.IndexOf(pCorreo);

                        if (indice > 0)
                        {
                            Correo correo = ListaCorreosEliminados[indice - 1];
                            return correo;
                        }
                        break;
                    }
            }
            return null;
        }

        /// <summary>
        /// Obtiene el correo siguiente al pasado por parámetro
        /// </summary>
        /// <param name="pCorreo">Correo</param>
        /// <param name="pLista">Listado de tipos de correo</param>
        /// <returns>Correo siguiente</returns>
        public Correo SiguienteCorreo(Correo pCorreo, TiposListadoCorreo pLista)
        {
            int indice;

            switch (pLista)
            {
                case TiposListadoCorreo.Recibido:
                    {
                        indice = ListaCorreosRecibidos.IndexOf(pCorreo);

                        if (indice < ListaCorreosRecibidos.Count - 1)
                        {
                            return ListaCorreosRecibidos[indice + 1];
                        }
                        break;
                    }
                case TiposListadoCorreo.Enviado:
                    {
                        indice = ListaCorreosEnviados.IndexOf(pCorreo);

                        if (indice < ListaCorreosEnviados.Count - 1)
                        {
                            return ListaCorreosEnviados[indice + 1];
                        }
                        break;
                    }
                case TiposListadoCorreo.Eliminado:
                    {
                        indice = ListaCorreosEliminados.IndexOf(pCorreo);

                        if (indice < ListaCorreosEliminados.Count - 1)
                        {
                            Correo correo = ListaCorreosEliminados[indice + 1];
                            return correo;
                        }
                        break;
                    }
            }
            return null;
        }

        /// <summary>
        /// Comprueba si el remitente del correo pasado por parámetro es desconocido
        /// </summary>
        /// <param name="pCorreo">Correo</param>
        /// <returns>TRUE si es desconocido, FALSE en caso contrario</returns>
        public bool EsRemitenteDesconocido(Correo pCorreo)
        {
            bool esDesconocido = true;

            if (pCorreo.Recibido)
            {
                foreach (IDE identidadAmigo in GestorAmigos.ListaContactos.Values)
                {
                    if (pCorreo.Autor != null && (identidadAmigo.Clave.Equals(pCorreo.Autor.Clave)))
                    {
                        esDesconocido = false;
                    }
                }
                if (ProyectoAD.MetaProyecto != ProyectoAD.MyGnoss)
                {
                    foreach (IDE identidad in GestorIdentidades.ListaIdentidades.Values)
                    {
                        if (pCorreo.Autor != null && (identidad.Clave.Equals(pCorreo.Autor.Clave)))
                        {
                            esDesconocido = false;
                        }
                    }
                }
                if (esDesconocido)
                {
                    foreach (Guid identidadID in GestorAmigos.ListaPermitidosPorOrg)
                    {
                        if (pCorreo.Autor != null && (identidadID.Equals(pCorreo.Autor.Clave)))
                        {
                            esDesconocido = false;
                        }
                    }
                }

            }
            return esDesconocido;
        }

        /// <summary>
        /// Compara por fecha de forma descendente los dos correos pasados por parámetro
        /// </summary>
        /// <param name="pCorreoX">Correo</param>
        /// <param name="pCorreoY">Correo</param>
        /// <returns></returns>
        public static int CompararCorreosPorFechaDescendente(Correo pCorreoX, Correo pCorreoY)
        {
            if (pCorreoX.Fecha > pCorreoY.Fecha)
            {
                return -1;
            }
            else if (pCorreoX.Fecha < pCorreoY.Fecha)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Separa los destinatarios de un correo
        /// </summary>
        /// <param name="pDestinatarios">Cadena de texto con los destinatarios del correo separados por comas</param>
        /// <param name="pListaDescartados">Lista de identificadores de destinatarios descartados</param>
        /// <param name="pListaPreferencias">Lista de identificadores de preferencias</param>
        /// <returns>Lista de identificadores de destinatarios de correo</returns>
        public Dictionary<Guid, bool> SepararDestinatarios(string pDestinatarios, ref List<string> pListaDescartados, List<Guid> pListaPreferencias)
        {
            return SepararDestinatarios(pDestinatarios, ref pListaDescartados, pListaPreferencias, new List<string>(), false);
        }

        /// <summary>
        /// Separa los destinatarios de un correo
        /// </summary>
        /// <param name="pDestinatarios">Cadena de texto con los destinatarios del correo separados por comas</param>
        /// <param name="pListaDescartados">Lista de identificadores de destinatarios descartados</param>
        /// <param name="pListaPreferencias">Lista de identificadores de preferencias</param>
        /// <param name="pListaProfesores">Lista con las cadenas que puede tener un perfil de profesor</param>
        /// <param name="pRespondiendo">Indica si se está respondiendo o no</param>
        /// <returns>Lista de identificadores de destinatarios de correo</returns>
        public Dictionary<Guid, bool> SepararDestinatarios(string pDestinatarios, ref List<string> pListaDescartados, List<Guid> pListaPreferencias, List<string> pListaProfesores, bool pRespondiendo)
        {
            char separador = ',';
            //El bool representa:
            //false-->Es una Identidad
            //true-->Es un grupo
            Dictionary<Guid, bool> lista = new Dictionary<Guid, bool>();
            string[] resultado = new string[0];

            if (pDestinatarios != "")
            {
                string[] arrayDestinatarios = pDestinatarios.Split(new char[] { separador }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string destinatario in arrayDestinatarios)
                {
                    string destinatarioTrim = destinatario.Trim();
                    bool encontrada = false;
                    Guid identidadID = Guid.Empty;
                    using (IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
                    {
                        if (destinatarioTrim.Contains("|||"))
                        {
                            int num = 3;
                            Guid idGuid = Guid.Parse(destinatarioTrim.Substring(destinatarioTrim.IndexOf("|||") + num));
                            string nombreCorto = identidadCN.ObtenerNombreCortoGrupoPorID(idGuid);
                            //nombreCorto=null -> Value=false ->  esta en tabla identidad 
                            lista.Add(idGuid, !string.IsNullOrEmpty(nombreCorto));
                        }
                        else if (!string.IsNullOrWhiteSpace(destinatarioTrim))
                        {
                            lista = BuscamosDestinarioSinID(pListaDescartados, pListaProfesores, lista, destinatarioTrim, ref encontrada, identidadCN);
                        }
                    }
                }
            }
            return lista;
        }

        private static Dictionary<Guid, bool> BuscamosDestinarioSinID(List<string> pListaDescartados, List<string> pListaProfesores, Dictionary<Guid, bool> lista, string destinatario, ref bool encontrada, IdentidadCN identidadCN)
        {
            Guid identidadID;
            bool esProfesor = false;
            foreach (string profesor in pListaProfesores)
            {
                if (destinatario.ToLower().Contains(profesor.ToLower()))
                {
                    esProfesor = true;
                    break;
                }
            }
            if (esProfesor)
            {
                int inicio = destinatario.IndexOf(ConstantesDeSeparacion.SEPARACION_CONCATENADOR);
                identidadID = identidadCN.ObtenerIdentidadProfesorIDDeMyGNOSSPorNombre(destinatario.Substring(inicio + 2));
            }
            else if (destinatario.Contains(ConstantesDeSeparacion.SEPARACION_CONCATENADOR))
            {
                int limite = destinatario.IndexOf(ConstantesDeSeparacion.SEPARACION_CONCATENADOR);
                string nombre = destinatario.Substring(0, limite - 1);
                string org = destinatario.Substring(limite + 2);
                identidadID = identidadCN.ObtenerIdentidadEnOrgIDDeMyGNOSSPorNombre(nombre, org);
            }
            else
            {
                identidadID = identidadCN.ObtenerIdentidadIDDeMyGNOSSPorNombre(destinatario);
            }
            if (identidadID == Guid.Empty)
            {
                identidadID = identidadCN.ObtenerIdentidadOrganizacionIDDeMyGNOSSPorNombre(destinatario);
            }

            //Si existe alguna identidad con ese nombre
            if (identidadID != Guid.Empty)
            {
                encontrada = true;
            }

            if (encontrada)
            {
                if (!lista.ContainsKey(identidadID))
                {
                    lista.Add(identidadID, false);
                }
            }
            else
            {
                //Si no es identidad compruebo si es grupo
                Guid grupoID = identidadCN.ObtenerGrupoIDPorNombre(destinatario);
                if (grupoID == Guid.Empty)
                {
                    pListaDescartados.Add(destinatario);
                }
                else
                {
                    if (!lista.ContainsKey(grupoID))
                    {
                        lista.Add(grupoID, true);
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// A partir de una lista, de nombres de identidades descartadas, extrae las identidades de los miembros de los grupos 
        /// cuyo nombre contiene la lista.
        /// </summary>
        /// <param name="pListaDescartados">Lista de nombres de identidades descartadas que puden contener el nombre de algún 
        /// grupo al que hay que enviar el correo</param>
        /// <returns>Lista con las identidades de los miembros de los grupo cuyo nombre está contenido en "pListaDescartados"</returns>
        public List<Guid> SepararDestinatariosDeGrupos(ref List<string> pListaDescartados)
        {
            List<Guid> lista = new List<Guid>();
            List<string> listaPosiblesGrupos = new List<string>();
            listaPosiblesGrupos.AddRange(pListaDescartados);

            if (pListaDescartados.Count > 0)
            {
                foreach (string posibleGrupo in listaPosiblesGrupos)
                {
                    foreach (AD.EntityModel.Models.IdentidadDS.GrupoAmigos grupoAmigos in GestorAmigos.ListaGrupoAmigos.Values)
                    {
                        Regex reg = new Regex("[^a-zA-Z0-9 ]");

                        string grupoSinAcentos = reg.Replace(grupoAmigos.Nombre.Trim().ToLower().Normalize(NormalizationForm.FormD), "");
                        string posibleGrupoSinAcentos = reg.Replace(posibleGrupo.Trim().ToLower().Normalize(NormalizationForm.FormD), "");

                        if (grupoSinAcentos.Equals(posibleGrupoSinAcentos))
                        {
                            foreach (IDE amigo in GrupoAmigosObtenerAmigos(grupoAmigos).Values)
                            {
                                if (!lista.Contains(amigo.Clave))
                                {
                                    lista.Add(amigo.Clave);
                                }
                            }

                            //Quito de la lista de descartados el grupo:
                            pListaDescartados.Remove(posibleGrupo);
                            break;
                        }
                    }
                }
            }
            return lista;
        }

        private Dictionary<Guid, Es.Riam.Gnoss.Elementos.Identidad.Identidad> GrupoAmigosObtenerAmigos(AD.EntityModel.Models.IdentidadDS.GrupoAmigos grupoAmigos)
        {
            Dictionary<Guid, Es.Riam.Gnoss.Elementos.Identidad.Identidad> listaAmigos = new Dictionary<Guid, Es.Riam.Gnoss.Elementos.Identidad.Identidad>();

            foreach (AmigoAgGrupo filaAmigoAgGrupo in GestorAmigos.AmigosDW.ListaAmigoAgGrupo)
            {
                if (GestorIdentidades != null && GestorIdentidades.ListaIdentidades.ContainsKey(filaAmigoAgGrupo.IdentidadAmigoID))
                {
                    listaAmigos.Add(filaAmigoAgGrupo.IdentidadAmigoID, GestorIdentidades.ListaIdentidades[filaAmigoAgGrupo.IdentidadAmigoID]);
                }
            }

            return listaAmigos;
        }

        /// <summary>
        /// Anula las listas de correos para que se recarguen de nuevo
        /// </summary>
        public void ResetearListasCorreos()
        {
            mListaCorreos = null;
            mListaCorreosEliminados = null;
            mListaCorreosEnviados = null;
            mListaCorreosRecibidos = null;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el dataset de correo recibido
        /// </summary>
        public CorreoDS CorreoDS
        {
            get
            {
                return mCorreoDS;
            }
        }


        /// <summary>
        /// Obtiene o establece el gestor de identidades
        /// </summary>
        public GestionIdentidades GestorIdentidades
        {
            get
            {
                return mGestorIdentidades;
            }
            set
            {
                mGestorIdentidades = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de notificaciones
        /// </summary>
        public GestionNotificaciones GestorNotificaciones
        {
            get
            {
                if (mGestorNotificaciones == null)
                {
                    mGestorNotificaciones = new GestionNotificaciones(new DataWrapperNotificacion(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                }
                return mGestorNotificaciones;
            }
            set
            {
                mGestorNotificaciones = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de amigos
        /// </summary>
        public GestionAmigos GestorAmigos
        {
            get
            {
                return mGestorAmigos;
            }
            set
            {
                mGestorAmigos = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de los correos enviados
        /// </summary>
        public List<Correo> ListaCorreosEnviados
        {
            get
            {
                if (mListaCorreosEnviados == null)
                {
                    //Cargo todas las listas de correo con la propiedad:
                    Dictionary<Guid, Correo> listaCorreo = ListaCorreos;
                }

                return mListaCorreosEnviados;
            }
        }

        /// <summary>
        /// Obtiene la lista de los correos recibidos
        /// </summary>
        public List<Correo> ListaCorreosRecibidos
        {
            get
            {
                if (mListaCorreosRecibidos == null)
                {
                    //Cargo todas las listas de correo con la propiedad:
                    Dictionary<Guid, Correo> listaCorreo = ListaCorreos;
                }

                return mListaCorreosRecibidos;
            }
        }

        /// <summary>
        /// Obtiene la lista de los correos eliminados
        /// </summary>
        public List<Correo> ListaCorreosEliminados
        {
            get
            {
                if (mListaCorreosEliminados == null)
                {
                    //Cargo todas las listas de correo con la propiedad:
                    Dictionary<Guid, Correo> listaCorreo = ListaCorreos;
                }

                return mListaCorreosEliminados;
            }
        }

        /// <summary>
        /// Obtiene la lista de los correos con clave
        /// </summary>
        public Dictionary<Guid, Correo> ListaCorreos
        {
            get
            {
                if (mListaCorreos == null)
                {
                    mListaCorreos = new Dictionary<Guid, Correo>();
                    mListaCorreosEliminados = new List<Correo>();
                    mListaCorreosRecibidos = new List<Correo>();
                    mListaCorreosEnviados = new List<Correo>();

                    foreach (CorreoDS.CorreoInternoRow filaCorreoInterno in mCorreoDS.CorreoInterno)
                    {
                        if (!filaCorreoInterno.Eliminado)
                        {
                            Correo correo = new Correo(filaCorreoInterno, this, mLoggingService);

                            if (filaCorreoInterno.EnPapelera)
                            {
                                mListaCorreosEliminados.Add(correo);
                            }
                            else if (!filaCorreoInterno.EnPapelera && filaCorreoInterno.Destinatario != Guid.Empty)
                            {
                                mListaCorreosRecibidos.Add(correo);
                            }
                            else if (!filaCorreoInterno.EnPapelera && filaCorreoInterno.Destinatario == Guid.Empty)
                            {
                                mListaCorreosEnviados.Add(correo);
                            }

                            if (!mListaCorreos.ContainsKey(correo.Clave))
                            {
                                mListaCorreos.Add(correo.Clave, correo);
                            }
                            else if (mListaCorreos[correo.Clave].Enviado && correo.Recibido)
                            {
                                mListaCorreos[correo.Clave] = correo;
                            }
                        }
                    }

                    mListaCorreosEliminados.Sort(CompararCorreosPorFechaDescendente);
                    mListaCorreosRecibidos.Sort(CompararCorreosPorFechaDescendente);
                    mListaCorreosEnviados.Sort(CompararCorreosPorFechaDescendente);
                }
                return mListaCorreos;
            }
        }

        /// <summary>
        /// Obtiene o establece el correo seleccionado
        /// </summary>
        public Correo CorreoSeleccionado
        {
            get
            {
                if (this.ListaCorreos.ContainsKey(mCorreoSeleccionadoID))
                {
                    return this.ListaCorreos[mCorreoSeleccionadoID];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value == null)
                {
                    mCorreoSeleccionadoID = Guid.Empty;
                }
                else
                {
                    mCorreoSeleccionadoID = value.Clave;
                }
            }
        }

        /// <summary>
        /// Obtiene la lista de correos seleccionados
        /// </summary>
        public Dictionary<Guid, Correo> CorreosSeleccionados
        {
            get
            {
                return mCorreosSeleccionados;
            }
        }

        /// <summary>
        /// Obtiene o establece la identidad conectada
        /// </summary>
        public IDE IdentidadActual
        {
            get
            {
                return mIdentidadActual;
            }
            set
            {
                mIdentidadActual = value;
            }
        }

        public bool EsEcosistemaProyectoConMensajesPersonalizadoBienvenida
        {
            get
            {
                return mEsEcosistemaProyectoConMensajesPersonalizadoBienvenida;
            }
            set
            {
                mEsEcosistemaProyectoConMensajesPersonalizadoBienvenida = value;
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Método para serializar el objeto
        /// </summary>
        /// <param name="pInfo">Datos serializados</param>
        /// <param name="pContext">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            pInfo.AddValue("CorreoDS", CorreoDS);
            pInfo.AddValue("GestorAmigos", GestorAmigos);
            pInfo.AddValue("GestorIdentidades", GestorIdentidades);
            pInfo.AddValue("GestorNotificaciones", GestorNotificaciones);

            if (IdentidadActual != null)
            {
                pInfo.AddValue("IdentidadActual", IdentidadActual.Clave);
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~GestionCorreo()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="pDisposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool pDisposing)
        {
            if (!this.mDisposed)
            {
                mDisposed = true;

                try
                {
                    if (pDisposing)
                    {
                        //Liberamos todos los recursos administrados que hemos añadido a esta clase
                        if (this.mListaCorreosRecibidos != null)
                        {
                            this.mListaCorreosRecibidos.Clear();
                        }

                        if (this.mListaCorreosEnviados != null)
                        {
                            this.mListaCorreosEnviados.Clear();
                        }

                        if (this.mListaCorreosEliminados != null)
                        {
                            this.mListaCorreosEliminados.Clear();
                        }

                        if (this.mListaCorreos != null)
                        {
                            this.mListaCorreos.Clear();
                        }

                        if (this.mCorreosSeleccionados != null)
                        {
                            this.mCorreosSeleccionados.Clear();
                        }
                    }
                }
                finally
                {
                    mListaCorreosRecibidos = null;
                    mListaCorreosEnviados = null;
                    mListaCorreosEliminados = null;
                    mListaCorreos = null;
                    mCorreosSeleccionados = null;

                    mCorreoDS = null;

                    mGestorAmigos = null;
                    mGestorIdentidades = null;
                    mGestorNotificaciones = null;

                    // Llamo al dispose de la clase base
                    base.Dispose(pDisposing);
                }
            }
        }

        #endregion
    }
}
