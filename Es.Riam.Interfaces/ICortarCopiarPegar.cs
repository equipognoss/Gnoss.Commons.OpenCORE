namespace Es.Riam.Interfaces
{
    /// <summary>
    /// Interfaz para las acciones relacionadas con cortar, copiar y/o pegar
    /// </summary>
    public interface ICortarCopiarPegar : IElementoGnoss
    {
        /// <summary>
        /// Corta el elemento
        /// </summary>
        void Cortar();

        /// <summary>
        /// Pega el elemento
        /// </summary>
        void Pegar(IElementoGnoss pElementoAPegar, short pIndice);

        /// <summary>
        /// Copia el elemento
        /// </summary>
        void Copiar();

        /// <summary>
        /// Establece si el elemento esta cortado
        /// </summary>
        bool EstaCortado
        {
            get;
            set;
        }

        /// <summary>
        /// Establece si el elemento esta cortado
        /// </summary>
        bool EstaCortadoPadre
        {
            get;
            set;
        }

        /// <summary>
        /// Devuelve true si el elemento permite ser cortado o arrastrado
        /// </summary>
        /// <returns></returns>
        bool SePuedeCortarArrastrar
        {
            get;
        }

        /// <summary>
        /// Realiza un movimiento del elemento bien por arrastre o por cortar-pegar
        /// </summary>
        /// <param name="pPadre">Elemento al que va a pertenecer tras el movimiento</param>
        /// <param name="pPosicion">Lugar que ocupará dentro de la lista de elementos inferiores</param>
        /// <returns>Devuelve true si la operación ha resultado bien</returns>
        bool Mover(ICortarCopiarPegar pPadre, short pPosicion);

        /// <summary>
        /// Comprueba si el elemento que se pasa puede ser movido al interior de este elemento
        /// </summary>
        /// <param name="pHijoCandidato">Elemento candidato a insertarse como inferior</param>
        /// <returns>Devuelve true si la operación resultaría correcta</returns>
        bool AdmiteHijo(IElementoGnoss pHijoCandidato);

        /// <summary>
        /// Devuelve true si el elemento permite ser copiado
        /// </summary>
        bool SePuedeCopiar
        {
            get;
        }

    }
}
