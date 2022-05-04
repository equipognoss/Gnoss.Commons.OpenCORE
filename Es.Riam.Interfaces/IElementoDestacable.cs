
namespace Es.Riam.Interfaces
{
    /// <summary>
    /// Interfaz de elemento destacado
    /// </summary>
    public interface IElementoDestacable
    {
        /// <summary>
        /// Establece si el elemento esta destacado
        /// </summary>
        bool EstaDestacado
        {
            get;
            set;
        }

        /// <summary>
        /// Establece el numero de hijos del elemento que están destacados
        /// </summary>
        int NumeroHijosDestacados
        {
            get;
            set;
        }


        /// <summary>
        /// Devuelve true si el elemento permite ser destacado
        /// </summary>
        /// <returns></returns>
        bool SePuedeDestacar
        {
            get;
        }
    }
}
