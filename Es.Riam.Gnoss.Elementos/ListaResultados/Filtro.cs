using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;

namespace Es.Riam.Gnoss.Elementos.ListaResultados
{
    /// <summary>
    /// Clase que representa un filtro agregado a una búsqueda.
    /// </summary>
    public class Filtro : ElementoGnoss
    {
        #region Miembros

        #region Estáticos

        /// <summary>
        /// Campo auxiliar para almacenar el nombre del filtro.
        /// </summary>
        public const string CAMPO_NOMBRE = "Nombre";

        /// <summary>
        /// Campo auxiliar para almacenar el valor del filtro.
        /// </summary>
        public const string CAMPO_VALOR = "Valor";

        #endregion
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor para la lista.
        /// </summary>
        /// <param name="pGestor">Gestor</param>
        /// <param name="pNombre">Nombre del filtro</param>
        /// <param name="pValor">Valor del filtro</param>
        public Filtro(GestionGnoss pGestor, string pNombre, string pValor) 
            : base(null, pGestor)
        {
            //Agrego la fila auxiliar que será manejada por el exportadorGnoss:
            FilaElemento = pGestor.DataSet.Tables[GestorListaResultados.TABLA_AUXILIAR_FILTROS].NewRow();

            Nombre = pNombre;
            Valor = pValor;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece el nombre del filtro.
        /// </summary>
        public override string Nombre
        {
            get
            {
                return (string)FilaElemento[CAMPO_NOMBRE];
            }
            set
            {
                FilaElemento[CAMPO_NOMBRE] = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el nombre corto del filtro.
        /// </summary>
        public string NombreCorto
        {
            get
            {
                return UtilCadenas.EliminarCaracteresUrlSem(Nombre);
            }
        }

        /// <summary>
        /// Devuelve o establece el valor del filtro.
        /// </summary>
        public string Valor
        {
            get
            {
                return (string)FilaElemento[CAMPO_VALOR];
            }
            set
            {
                FilaElemento[CAMPO_VALOR] = value;
            }
        }

        #endregion
    }
}
