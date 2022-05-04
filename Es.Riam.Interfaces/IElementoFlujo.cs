using System.Collections.Generic;

namespace Es.Riam.Interfaces
{
    /// <summary>
    /// Interfaz para elementos de flujo
    /// </summary>
    public interface IElementoFlujo : IElementoGnoss
    {
        /// <summary>
        /// Lista de elementos consecuentes
        /// </summary>
        // B-BUG886 David: Ahora la lista de elementos consecuentes tiene un estado -> List<IElementoFlujo> ElementosConsecuentes
        Dictionary<IElementoFlujo, short> ElementosConsecuentes
        {
            get;
        }

        /// <summary>
        /// Lista de elementos antecedentes
        /// </summary>
        // B-BUG886 David: Ahora la lista de elementos antecedentes tiene un estado -> List<IElementoFlujo> ElementosAntecedentes
        Dictionary<IElementoFlujo, short> ElementosAntecedentes
        {
            get;
        }

        /// <summary>
        /// Agrega la relación entre los elementos
        /// </summary>
        /// <param name="pElementoAntecedente"></param>
        void AgregarAntecedente(IElementoFlujo pElementoAntecedente);

        /// <summary>
        /// Elimina la relación entre los elementos
        /// </summary>
        /// <param name="pElementoAntecedente"></param>
        void EliminarAntecedente(IElementoFlujo pElementoAntecedente);

        /// <summary>
        /// Si permite la unión entre el elemento actual y otro que sería el consecuente
        /// </summary>
        /// <param name="pElementoConsecuente">Elemento consecuente</param>
        /// <returns></returns>
        bool PermiteUnion(IElementoFlujo pElementoConsecuente);
    }
}
