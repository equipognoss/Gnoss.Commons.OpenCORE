using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Carga
{
    /// <summary>
    /// Estado del paquete de la carga
    /// </summary>
    public enum EstadoPaquete
    {
        /// <summary>
        /// Pendiente de procesar
        /// </summary>
        Pendiente = 0,

        /// <summary>
        /// El paquete se ha procesado correctamente
        /// </summary>
        Correcto = 1,

        /// <summary>
        /// El paquete ha fallado durante la carga
        /// </summary>
        Erroneo = 2
    }
}
