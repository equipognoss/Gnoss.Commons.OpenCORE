using Es.Riam.Util;
using System.Security.Principal;

namespace Es.Riam.Gnoss.Util.Seguridad
{
    /// <summary>
    /// Clase para usuario
    /// </summary>
    public class Usuario
    {
        private readonly UtilPeticion _utilPeticion;

        public Usuario(UtilPeticion utilPeticion)
        {
            _utilPeticion = utilPeticion;
        }
        /// <summary>
        /// Devuelve el GnossIdentity
        /// </summary>
        public GnossIdentity UsuarioActual
        {
            get
            {
                IIdentity usuario = _utilPeticion.ObtenerObjetoDePeticion("GnossIdentity") as GnossIdentity;
                if (usuario is GnossIdentity)
                {
                    return (GnossIdentity)usuario;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
