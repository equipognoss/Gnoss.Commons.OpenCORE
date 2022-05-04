using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Elementos.Notificacion
{
    /// <summary>
    /// Notificaci�n
    /// </summary>
    public class Notificacion : ElementoGnoss
    {
        #region Miembros

        /// <summary>
        /// Lista de par�metros parametrizables de la notificaci�n
        /// </summary>
        private Dictionary<string, string> mListaParametros;

        /// <summary>
        /// Lista de par�metros-persona parametrizables de la notificaci�n
        /// </summary>
        private Dictionary<string, string> mListaParametrosPersona;

        /// <summary>
        /// Mensaje en el que se han sustituido los par�metros por sus nombres reales (idioma, mensaje)
        /// </summary>
        private Dictionary<string, string> mMensajeCompuesto;

        /// <summary>
        /// Asunto en el que se han sustituido los par�metros por sus nombres reales (idioma, mensaje)
        /// </summary>
        private Dictionary<string, string> mAsuntoCompuesto;

        /// <summary>
        /// Logo que pueden tener algunas Notificaciones de correo electr�nico, como sucripciones
        /// </summary>
        private string mLogoCabecera;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor a partir de una fila de notificaci�n y del gestor de notificaciones pasados por par�metro
        /// </summary>
        /// <param name="pFilaNotificacion">Fila de notificaci�n</param>
        /// <param name="pGestorNotificaciones">Gestor de notificaciones</param>
        public Notificacion(AD.EntityModel.Models.Notificacion.Notificacion pFilaNotificacion,GestionNotificaciones pGestorNotificaciones, LoggingService loggingService)
            : base(pFilaNotificacion, pGestorNotificaciones, loggingService)
        {
        }

        #endregion

        #region M�todos generales

        #region P�blicos

        /// <summary>
        /// Obtiene el mensaje de la notificaci�n en funci�n del idioma pasado por par�metro
        /// </summary>
        /// <returns>Mensaje de la notificaci�n</returns>
        public string Mensaje(string Idioma)
        {
            if (Idioma.Trim() == string.Empty)
            {
                Idioma = "es";
            }
            if (!((mMensajeCompuesto == null) || (!mMensajeCompuesto.ContainsKey(Idioma)) || (mMensajeCompuesto[Idioma].Trim().Equals(string.Empty))))
                ComponerAsuntoYMensaje(Idioma);
            return mMensajeCompuesto[Idioma];
        }

        /// <summary>
        /// Obtiene el mensaje de la notificaci�n
        /// </summary>
        /// <returns>Mensaje de la notificaci�n</returns>
        public string Mensaje()
        {
            return Mensaje(FilaNotificacion.Idioma);
        }

        /// <summary>
        /// Obtiene el asunto de la notificaci�n en funci�n del idioma pasado por par�metro
        /// </summary>
        /// <param name="Idioma">Idioma</param>
        /// <returns>Asunto de la notificaci�n</returns>
        public string Asunto(string Idioma)
        {
            if (Idioma.Trim() == string.Empty)
            {
                Idioma = "es";
            }
            if ((mAsuntoCompuesto == null) || (!mAsuntoCompuesto.ContainsKey(Idioma)) || (mAsuntoCompuesto[Idioma].Trim().Equals(string.Empty)))
                ComponerAsuntoYMensaje(Idioma);
            return mAsuntoCompuesto[Idioma];
        }

        /// <summary>
        /// Obtiene el asunto de la notificaci�n
        /// </summary>
        /// <returns>Asunto de la notificaci�n</returns>
        public string Asunto()
        {
            return Asunto(FilaNotificacion.Idioma);
        }

        #endregion

        #region Privados

        /// <summary>
        /// Compone un mensaje sustituyendo los par�metros por sus valores reales
        /// </summary>
        /// <param name="pIdioma">Idioma</param>
        private void ComponerAsuntoYMensaje(string pIdioma)
        {
            if (mAsuntoCompuesto == null)
            {
                mAsuntoCompuesto = new Dictionary<string, string>();
            }
            if (mMensajeCompuesto == null)
            {
                mMensajeCompuesto = new Dictionary<string, string>();
            }

            //Asunto
            string asunto = GestorNotificaciones.ListaMensajes(pIdioma)[FilaNotificacion.MensajeID].Key;

            foreach (string parametro in ListaParametrosParametrizables.Keys)
            {
                asunto = asunto.Replace(parametro, ListaParametrosParametrizables[parametro]);
            }


            //Mensaje
            string mensaje = GestorNotificaciones.ListaMensajes(pIdioma)[FilaNotificacion.MensajeID].Value;

            foreach (string parametro in ListaParametrosParametrizables.Keys)
            {
                if (parametro != GestionNotificaciones.TextoDeParametro((short)ClavesParametro.LogoGnoss))
                    mensaje = mensaje.Replace(parametro, ListaParametrosParametrizables[parametro]);
            }

            //Parametros de persona, exlusivo para suscripciones
            if (this.Origen == OrigenesNotificacion.Suscripcion)
            {
                foreach (string parametro in ListaParametrosPersonaParametrizables.Keys)
                {
                    mensaje = mensaje.Replace(parametro, ListaParametrosPersonaParametrizables[parametro]);
                }
            }

            if (mAsuntoCompuesto.ContainsKey(pIdioma))
            {
                mAsuntoCompuesto[pIdioma] = asunto;
            }
            else
            {
                mAsuntoCompuesto.Add(pIdioma, asunto);
            }

            if (mMensajeCompuesto.ContainsKey(pIdioma))
            {
                mMensajeCompuesto[pIdioma] = mensaje;
            }
            else
            {
                mMensajeCompuesto.Add(pIdioma, mensaje);
            }
        }

        /// <summary>
        /// Captura el logo de la notificaci�n en caso de que �sta lo tenga
        /// </summary>
        private void CapturarLogo()
        {
            foreach (string parametro in ListaParametrosParametrizables.Keys)
            {
                if (parametro == GestionNotificaciones.TextoDeParametro((short)ClavesParametro.LogoGnoss))
                    this.mLogoCabecera = ListaParametrosParametrizables[parametro];
            }
        }

        /// <summary>
        /// Carga la lista de par�metros para �sta notificaci�n
        /// </summary>
        private void CargarListaParametros()
        {
            mListaParametros = new Dictionary<string, string>();

            foreach (AD.EntityModel.Models.Notificacion.NotificacionParametro filaParametro in FilaNotificacion.NotificacionParametro)
            {
                string nombreParametro = GestionNotificaciones.TextoDeParametro(filaParametro.ParametroID);
                string valor = "";

                /*if (UtilParametrosNotificaciones.EsParametroParametrizable(filaParametro.ParametroID))
                {*/
                valor = filaParametro.Valor;
                /*}
                else
                {
                    valor = UtilParametrosNotificaciones.ValorParametroNOParametrizable(filaParametro.ParametroID);
                }*/

                mListaParametros.Add(nombreParametro, valor);
            }
        }

        /// <summary>
        /// Carga la lista de par�metros para �sta notificaci�n
        /// </summary>
        private void CargarListaParametrosPersona()
        {
            mListaParametrosPersona = new Dictionary<string, string>();

            foreach (AD.EntityModel.Models.Notificacion.NotificacionParametroPersona filaParametroPersona in FilaNotificacion.NotificacionParametroPersona)
            {
                string nombreParametro = GestionNotificaciones.TextoDeParametro((short)filaParametroPersona.ParametroID);
                string valor = "";

                /*if (UtilParametrosNotificaciones.EsParametroParametrizable(filaParametro.ParametroID))
                {*/
                valor = filaParametroPersona.Valor;
                /*}
                else
                {
                    valor = UtilParametrosNotificaciones.ValorParametroNOParametrizable(filaParametro.ParametroID);
                }*/

                mListaParametrosPersona.Add(nombreParametro, valor);
            }
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve el gestor de notificaciones que contiene a la notificaci�n
        /// </summary>
        public GestionNotificaciones GestorNotificaciones
        {
            get
            {
                return (GestionNotificaciones)this.GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene el origen de la notificaci�n
        /// </summary>
        public OrigenesNotificacion Origen
        {
            get
            {
                if (0 <= FilaNotificacion.MensajeID && FilaNotificacion.MensajeID <= 19)
                    return OrigenesNotificacion.Dafo;
                else if (20 <= FilaNotificacion.MensajeID && FilaNotificacion.MensajeID <= 39)
                    return OrigenesNotificacion.Solicitud;
                else if (40 <= FilaNotificacion.MensajeID && FilaNotificacion.MensajeID <= 59)
                    return OrigenesNotificacion.Invitacion;
                else if (60 <= FilaNotificacion.MensajeID && FilaNotificacion.MensajeID <= 79)
                    return OrigenesNotificacion.Suscripcion;
                else if (80 <= FilaNotificacion.MensajeID && FilaNotificacion.MensajeID <= 99)
                    return OrigenesNotificacion.CambioContrasenia;
                else if (100 <= FilaNotificacion.MensajeID && FilaNotificacion.MensajeID <= 119)
                {
                    return OrigenesNotificacion.Sugerencia;
                }
                else if (160 <= FilaNotificacion.MensajeID && FilaNotificacion.MensajeID <= 169)
                    return OrigenesNotificacion.Comentario;
                else
                    return OrigenesNotificacion.Solicitud;
            }
        }

        /// <summary>
        /// Obtiene la fila de la notificaci�n
        /// </summary>
        public AD.EntityModel.Models.Notificacion.Notificacion FilaNotificacion
        {
            get
            {
                return (AD.EntityModel.Models.Notificacion.Notificacion)FilaElementoEntity;
            }
        }
        
        /// <summary>
        /// Obtiene el logo de la notificaci�n codificado en formato Base64
        /// </summary>
        public string LogoCabecera
        {
            get
            {
                if ((mLogoCabecera == null) || (mLogoCabecera.Trim().Equals(string.Empty)))
                    CapturarLogo();
                return mLogoCabecera;
            }
        }

        /// <summary>
        /// Obtiene la lista de par�metros que son parametrizables para esta notificaci�n
        /// </summary>
        public Dictionary<string, string> ListaParametrosParametrizables
        {
            get
            {
                if (mListaParametros == null)
                    CargarListaParametros();
                return mListaParametros;
            }
        }

        /// <summary>
        /// Obtiene la lista de par�metros-persona que son parametrizables para esta notificaci�n
        /// </summary>
        public Dictionary<string, string> ListaParametrosPersonaParametrizables
        {
            get
            {
                if (mListaParametrosPersona == null)
                    CargarListaParametrosPersona();
                return mListaParametrosPersona;
            }
        }

        /// <summary>
        /// Obtiene la clave de la notificaci�n
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return this.FilaNotificacion.NotificacionID;
            }
        }

        #endregion
    }
}
