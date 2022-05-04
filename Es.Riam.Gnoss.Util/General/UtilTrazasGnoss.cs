using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;

namespace Es.Riam.Gnoss.Util.General
{
    /// <summary>
    /// Traza para la Web de GNOSS
    /// </summary>
    public class TrazaGnossWeb : TrazaWeb
    {

        #region Constructores

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="pRutaTraza">Ruta de la traza</param>
        public TrazaGnossWeb(string pIP, int pPuerto, IHttpContextAccessor pHttpContextAccessor, UtilTelemetry pUtilTelemetry, Usuario usuario)
            : base(pIP, pPuerto, pHttpContextAccessor, pUtilTelemetry)
        {
            if (mEstaTrazaHabilitada)
            {
                if (usuario.UsuarioActual != null && !usuario.UsuarioActual.EsUsuarioInvitado)
                {
                    mMensajeFinalTraza += "|" + usuario.UsuarioActual.Login + "|" + usuario.UsuarioActual.UsuarioID + "|PersonaID: " + usuario.UsuarioActual.PersonaID + "|IdentidadID: " + usuario.UsuarioActual.IdentidadID;
                }
                else if (usuario.UsuarioActual != null && usuario.UsuarioActual.EsUsuarioInvitado)
                {
                    mMensajeFinalTraza += "|INVITADO";
                }
            }
        }

        #endregion
    }
}
