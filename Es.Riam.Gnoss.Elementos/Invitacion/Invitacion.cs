using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;

namespace Es.Riam.Gnoss.Elementos.Invitacion
{
    /// <summary>
    /// Invitaci�n
    /// </summary>
    public class Invitacion : ElementoGnoss
    {
        #region Miembros

        private Notificacion.Notificacion mNotificacion;
        private Identidad.Identidad mIdentidad;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor a partir de la fila de invitaci�n, notificaci�n e identidad pasadas por par�metro
        /// </summary>
        /// <param name="pFilaInvitacion">Fila de invitaci�n</param>
        /// <param name="pNotificacion">Notificaci�n</param>
        /// <param name="pIdentidad">Identidad</param>
        public Invitacion(AD.EntityModel.Models.Notificacion.Invitacion pFilaInvitacion, Notificacion.Notificacion pNotificacion, Identidad.Identidad pIdentidad)
            : base(pFilaInvitacion, null)
        {
            mNotificacion = pNotificacion;
            mIdentidad = pIdentidad;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la fila de la invitaci�n
        /// </summary>
        public AD.EntityModel.Models.Notificacion.Invitacion FilaInvitacion
        {
            get
            {
                return (AD.EntityModel.Models.Notificacion.Invitacion)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Obtiene la notificaci�n de la invitaci�n
        /// </summary>
        public Notificacion.Notificacion Notificacion
        {
            get
            {
                return mNotificacion;
            }
        }

        /// <summary>
        /// Obtiene la identidad or�gen de la invitaci�n
        /// </summary>
        public Identidad.Identidad IdentidadOrigen
        {
            get { return mIdentidad; }
        }

        /// <summary>
        /// Obtiene el tipo de notificaci�n de la invitaci�n
        /// </summary>
        public TiposNotificacion TipoInvitacion
        {
            get { return (TiposNotificacion)FilaInvitacion.Notificacion.MensajeID; }
        }

        /// <summary>
        /// Obtiene el identificador de la invitaci�n
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaInvitacion.InvitacionID;
            }
        }

        #endregion
    }
}
