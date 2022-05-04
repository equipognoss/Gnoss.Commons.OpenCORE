using System;
using System.Collections.Generic;
using System.Text;

namespace Es.Riam.Gnoss.Util.AnalisisSintactico
{

    #region Enumeraciones para separadores

    /// <summary>
    /// Enumeración dé artículos definidos e indefinidos
    /// </summary>
    public enum ListaArticulos
    {
        /// <summary>
        /// el
        /// </summary>
        el = 0,

        /// <summary>
        /// la
        /// </summary>
        la,

        /// <summary>
        /// los
        /// </summary>
        los,

        /// <summary>
        /// las
        /// </summary>
        las,

        /// <summary>
        /// un
        /// </summary>
        un,

        /// <summary>
        /// una
        /// </summary>
        una,

        /// <summary>
        /// lo
        /// </summary>
        lo,

        /// <summary>
        /// unos
        /// </summary>
        unos,

        /// <summary>
        /// unas
        /// </summary>
        unas

    }

    /// <summary>
    /// Enumeración de conjunciones
    /// </summary>
    public enum ListaConjunciones
    {
        /// <summary>
        /// y
        /// </summary>
        y = 0,

        /// <summary>
        /// o
        /// </summary>
        o,

        /// <summary>
        /// u
        /// </summary>
        u,

        /// <summary>
        /// e
        /// </summary>
        e,

        /// <summary>
        /// ni
        /// </summary>
        ni,

        /// <summary>
        /// coma
        /// </summary>
        caracterComa,

        /// <summary>
        /// punto
        /// </summary>
        caracterDosPuntos,

        /// <summary>
        /// asi como
        /// </summary>
        asiComo,

        /// <summary>
        /// como
        /// </summary>
        como,

        /// <summary>
        /// como que
        /// </summary>
        comoQue,

        /// <summary>
        /// de modo que
        /// </summary>
        deModoQue,

        /// <summary>
        /// tal como
        /// </summary>
        talComo,

        /// <summary>
        /// que
        /// </summary>
        que,

        /// <summary>
        /// conforme
        /// </summary>
        conforme,

        /// <summary>
        /// de manera que
        /// </summary>
        deManeraQue,

    }

    /// <summary>
    /// Enumeración de preposiciones
    /// </summary>
    public enum ListaPreposiciones
    {
        /// <summary>
        /// a
        /// </summary>
        a = 0,

        /// <summary>
        /// ante
        /// </summary>
        ante,

        /// <summary>
        /// bajo
        /// </summary>
        bajo,

        /// <summary>
        /// con
        /// </summary>
        con,

        /// <summary>
        /// contra
        /// </summary>
        contra,

        /// <summary>
        /// de
        /// </summary>
        de,

        /// <summary>
        /// del
        /// </summary>
        del,

        /// <summary>
        /// desde
        /// </summary>
        desde,

        /// <summary>
        /// en
        /// </summary>
        en,

        /// <summary>
        /// entre
        /// </summary>
        entre,

        /// <summary>
        /// hacia
        /// </summary>
        hacia,

        /// <summary>
        /// hasta
        /// </summary>
        hasta,

        /// <summary>
        /// para
        /// </summary>
        para,

        /// <summary>
        /// por
        /// </summary>
        por,

        /// <summary>
        /// segun
        /// </summary>
        segun,

        /// <summary>
        /// sin
        /// </summary>
        sin,

        /// <summary>
        /// so
        /// </summary>
        so,

        /// <summary>
        /// sobre
        /// </summary>
        sobre,

        /// <summary>
        /// tras
        /// </summary>
        tras,

        /// <summary>
        /// durante
        /// </summary>
        durante,

        /// <summary>
        /// mediante
        /// </summary>
        mediante,

        /// <summary>
        /// al
        /// </summary>
        al,

        /// <summary>
        /// excepto
        /// </summary>
        excepto,

        /// <summary>
        /// salvo
        /// </summary>
        salvo
    }

    #endregion

    /// <summary>
    /// Proporciona métodos para comparar 2 frases.
    /// </summary>
    public class AnalizadorSintacticoGnoss : Es.Riam.Util.AnalisisSintactico.AnalizadorSintactico
    {

        #region Métodos generales

        /// <summary>
        /// Devuelve verdad si la frase tiene la estructura de una frase gnoss.
        /// </summary>
        /// <param name="pFrase">Frase a comparar</param>
        /// <returns></returns>
        public static bool EsFraseGNOSS(string pFrase)
        {
            return FraseGnoss.EsFraseGNOSSValida(pFrase);
        }

        /// <summary>
        /// Obtiene todos los errores que contiene una frase gnoss. Null si es una frase gnoss válida.
        /// </summary>
        /// <param name="pFrase">frase a analizar</param>
        /// <returns></returns>
        public static string ObtenerErroresEnFraseGnoss(string pFrase)
        {
            return FraseGnoss.ObtenerErroresEnFraseGnoss(pFrase);
        }

        /// <summary>
        /// Compara dos frases Gnoss.
        /// </summary>
        /// <param name="pFrase1">Frase a comparar</param>
        /// <param name="pFrase2">Frase comparada</param>
        /// <returns></returns>
        public static float CompararFrases(string pFrase1, string pFrase2)
        {
            float resultado = 0F;
            FraseGnoss frase1 = new FraseGnoss(pFrase1);
            FraseGnoss frase2 = new FraseGnoss(pFrase2);

            //Comparo los objetos directos de las frases
            resultado = CompararCadenas(frase1.ObjetosDirectos, frase2.ObjetosDirectos);

            //Si los objetos directos tienen algun parecido, compruebo la similitud de los verbos.
            if (resultado > 0)
            {
                resultado = CompararCadenas(frase1.Verbos, frase2.Verbos) / 2 + resultado / 2;
            }
            return resultado;
        }


        #endregion
    }
}
