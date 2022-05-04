using System;

namespace Es.Riam.Gnoss.Util.General
{
    public class UtilOracle
    {
        /// <summary>
        /// Formatea un campo de texto en una cadena de caracteres separado por guiones para simular un campo Guid
        /// </summary>
        /// <param name="pCampo">Campo Guid traido como cadena de texto</param>
        /// <returns>Cadena de caracteres separada por guiones simulando un Guid</returns>
        public static string FormatearGuid(Guid pCampo, bool pAniadirGuiones = false)
        {
            string[] partesGuid = pCampo.ToString().Split('-');
            string guion = "";
            if (pAniadirGuiones)
            {
                guion = "-";
            }

            string nuevoGuid = $"{TransformarParteGuidAGuidOracle(partesGuid[0], 4)}{guion}{TransformarParteGuidAGuidOracle(partesGuid[1], 2)}{guion}{TransformarParteGuidAGuidOracle(partesGuid[2], 2)}{guion}{partesGuid[3]}{guion}{partesGuid[4]}";

            if (!pAniadirGuiones)
            {
                nuevoGuid = $"hextoraw('{nuevoGuid}')";
            }

            return nuevoGuid;
        }

        /// <summary>
        /// Revierte el conjunto de caracteres para formatearlo y adaptarlo a oracle
        /// </summary>
        /// <param name="pParteGuid"></param> Parte del guid a revertir
        /// <param name="pNumeroParesCaracteres"></param> Número de pares de caracteres
        /// <returns></returns>
        public static string TransformarParteGuidAGuidOracle(string pParteGuid, int pNumeroParesCaracteres)
        {
            string nuevoGuid = "";
            for (int i = 0; i < pNumeroParesCaracteres; i++)
            {
                string parte = pParteGuid.Substring(i * 2, 2);
                nuevoGuid = parte + nuevoGuid;
            }

            return nuevoGuid;
        }
    }
}
