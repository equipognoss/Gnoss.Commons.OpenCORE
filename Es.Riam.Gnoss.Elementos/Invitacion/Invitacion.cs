using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;

namespace Es.Riam.Gnoss.Elementos.Invitacion
{
    /// <summary>
    /// Invitación
    /// </summary>
    public class Invitacion : ElementoGnoss
    {
        #region Miembros

        private Notificacion.Notificacion mNotificacion;
        private Identidad.Identidad mIdentidad;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor a partir de la fila de invitación, notificación e identidad pasadas por parámetro
        /// </summary>
        /// <param name="pFilaInvitacion">Fila de invitación</param>
        /// <param name="pNotificacion">Notificación</param>
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
        /// Obtiene la fila de la invitación
        /// </summary>
        public AD.EntityModel.Models.Notificacion.Invitacion FilaInvitacion
        {
            get
            {
                return (AD.EntityModel.Models.Notificacion.Invitacion)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Obtiene la notificación de la invitación
        /// </summary>
        public Notificacion.Notificacion Notificacion
        {
            get
            {
                return mNotificacion;
            }
        }

        /// <summary>
        /// Obtiene la identidad orígen de la invitación
        /// </summary>
        public Identidad.Identidad IdentidadOrigen
        {
            get { return mIdentidad; }
        }

        /// <summary>
        /// Obtiene el tipo de notificación de la invitación
        /// </summary>
        public TiposNotificacion TipoInvitacion
        {
            get { return (TiposNotificacion)FilaInvitacion.Notificacion.MensajeID; }
        }

        /// <summary>
        /// Obtiene el identificador de la invitación
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
