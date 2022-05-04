using Es.Riam.Util.AnalisisSintactico;
using System;
using System.Collections.Generic;
using System.Text;

namespace Es.Riam.Gnoss.Util.AnalisisSintactico
{
    /// <summary>
    /// Clase que representa una frase gnoss
    /// </summary>
    public class FraseGnoss
    {
        #region Miembros

        #region Privados

        /// <summary>
        /// Lista de verbos que contiene la frase.
        /// </summary>
        private List<string> mVerbos;

        /// <summary>
        /// Lista de objetos directos que contiene la frase.
        /// </summary>
        private List<string> mObjetosDirectos;

        /// <summary>
        /// Lista de objetos indirectos que contiene la frase.
        /// </summary>
        private List<string> mObjetosIndirectos;

        /// <summary>
        /// Lista de otros complementos de la frase
        /// </summary>
        private List<string> mOtrosComplementos;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Crea una frase gnoss nueva
        /// </summary>
        /// <param name="pFrase">Frase a crear</param>
        public FraseGnoss(string pFrase)
        {
            mVerbos = new List<string>();
            mObjetosDirectos = new List<string>();
            mOtrosComplementos = new List<string>();
            mObjetosIndirectos = new List<string>();

            AnalizadorSintactico.EliminarFrasesHechas(ref pFrase);

            //Divide la frase en varias frases según los separadores definidos.
            string[] frase = pFrase.Split(AnalizadorSintactico.SEPARADORES, StringSplitOptions.RemoveEmptyEntries);
            //Parte la frase en verbos y objetos directos.
            PartirFrase(frase);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Lista de verbos que contiene la frase.
        /// </summary>
        public List<string> Verbos
        {
            get
            {
                return this.mVerbos;
            }
        }

        /// <summary>
        /// Lista de objetos directos que contiene la frase.
        /// </summary>
        public List<string> ObjetosDirectos
        {
            set
            {
                this.mObjetosDirectos = value;
            }
            get
            {
                return this.mObjetosDirectos;
            }
        }

        /// <summary>
        /// Lista de objetos indirectos que contiene la frase.
        /// </summary>
        public List<string> ObjetosIndirectos
        {
            get
            {
                return this.mObjetosIndirectos;
            }
        }

        /// <summary>
        /// Obtiene la lista del resto de complementos de la frase.
        /// </summary>
        public List<string> OtrosComplementos
        {
            get
            {
                return this.mOtrosComplementos;
            }
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Divide la frase en una lista de verbos y otra de objetos directos.
        /// </summary>
        /// <param name="pFrase">Frase a dividir</param>
        private void PartirFrase(string[] pFrase)
        {
            bool ultimoLeidoCircuntancial = false;
            bool descripcionDefinida = false;
            bool ultimoLeidoArticulo = false;
            bool objetoDirectoPersonal = false;

            foreach (string cadena in pFrase)
            {
                //inserto espacios a los dos lados por si hay conjunciones al principio o el final de la frase.
                cadena.Insert(0, " ");
                cadena.Insert(cadena.Length, " ");
                //Divido la frase por la lista de conjunciones.
                string[] subcadenas = cadena.Split(AnalizadorSintactico.CONJUNCIONES, StringSplitOptions.RemoveEmptyEntries);
                foreach (string subcadena in subcadenas)
                {
                    //Recorro todas las subcadena generadas.
                    int separador = 0;
                    string objetoDirecto = "";
                    string otrosComplementos = "";
                    string objetoIndirecto = "";
                    descripcionDefinida = false;
                    ultimoLeidoArticulo = false;

                    //Recorro cada palabra de la subcadena.
                    for (int indice = 0; separador < cadena.Length; indice = separador + 1)
                    {
                        //Busco el siguiente espacio en blanco
                        separador = cadena.IndexOf(' ', indice);
                        if (separador == -1)
                        {
                            separador = cadena.Length;
                        }
                        string palabra = cadena.Substring(indice, separador - indice);

                        if (!string.IsNullOrEmpty(palabra))
                        {
                            if (AnalizadorSintactico.CompararPreposicion(palabra))
                            {
                                //Si es una preposicion, ésta subcadena la deshecho ya que es un complemento circunstacial.
                                //Los complementos circunstanciales no son relevantes en cuanto a comparación de frases gnoss.

                                ultimoLeidoCircuntancial = true;
                                if (palabra.Trim().Equals("de"))
                                {
                                    //el complemento es una descripción definida
                                    descripcionDefinida = true;
                                    if (!string.IsNullOrEmpty(otrosComplementos))
                                    {
                                        otrosComplementos = otrosComplementos + " de";
                                    }
                                    else if (!string.IsNullOrEmpty(objetoDirecto))
                                    {
                                        objetoDirecto = objetoDirecto + " de";
                                    }
                                    else if (!string.IsNullOrEmpty(objetoIndirecto))
                                    {
                                        objetoIndirecto = objetoIndirecto + " de";
                                    }
                                }
                                else
                                {
                                    //El complemento no es una descripción definida
                                    descripcionDefinida = false;
                                }

                                if ((palabra.Trim().Equals("a")) || (palabra.Trim().Equals("al")))
                                {
                                    //Compruebo si se trata de un objeto directo de persona
                                    if ((string.IsNullOrEmpty(objetoIndirecto)) && (string.IsNullOrEmpty(objetoDirecto)) && (string.IsNullOrEmpty(otrosComplementos)) && (ObjetosDirectos.Count == 0))
                                    {
                                        //objetoDirecto = palabra;
                                        objetoDirectoPersonal = true;
                                        ultimoLeidoCircuntancial = false;
                                    }
                                }


                                if ((!descripcionDefinida) && (!objetoDirectoPersonal))
                                {
                                    //Agrego los complementos e inicializo las variables
                                    AgregarComplemento(otrosComplementos);
                                    otrosComplementos = "";
                                    AgregarObjetoDirecto(objetoDirecto);
                                    objetoDirecto = "";
                                }
                            }
                            else if ((!AnalizadorSintactico.CompararArticulo(palabra)) && (!AnalizadorSintactico.CompararPalabraNoRelevante(palabra)))//Si es un artículo, lo deshecho)
                            {
                                if ((!ultimoLeidoArticulo) && (AnalizadorSintactico.EsVerbo(palabra)))
                                {
                                    //Si es verbo, lo añado a la lista de verbos
                                    palabra = palabra.Trim();
                                    this.mVerbos.Add(palabra);
                                    ultimoLeidoCircuntancial = false;
                                }
                                else if ((ultimoLeidoCircuntancial) && ((!descripcionDefinida) || (!string.IsNullOrEmpty(otrosComplementos))))
                                {
                                    //Si es un complemento circunstancial, lo añado a la lista de otros complementos
                                    palabra = palabra.Trim();
                                    if (string.IsNullOrEmpty(otrosComplementos))
                                    {
                                        otrosComplementos = palabra;
                                    }
                                    else
                                    {
                                        otrosComplementos += " " + palabra;
                                    }
                                }
                                else
                                {
                                    //Si es un objeto directo, lo añado a la lista de objetos directos.
                                    palabra = palabra.Trim();

                                    if (objetoDirectoPersonal)
                                    {
                                        if (string.IsNullOrEmpty(objetoIndirecto))
                                        {
                                            objetoIndirecto = palabra;
                                        }
                                        else
                                        {
                                            objetoIndirecto += " " + palabra;
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(objetoDirecto))
                                        {
                                            objetoDirecto = palabra;
                                        }
                                        else
                                        {
                                            objetoDirecto += " " + palabra;
                                        }
                                    }
                                }
                            }
                            if (AnalizadorSintactico.CompararArticulo(palabra))
                            {
                                ultimoLeidoArticulo = true;
                                if (descripcionDefinida)
                                {
                                    //No hay una descripción definida, elimino la preposición 'de' que añadí antes y agrego los complementos a la lista
                                    descripcionDefinida = false;
                                    if (!string.IsNullOrEmpty(otrosComplementos))
                                    {
                                        AgregarComplemento(otrosComplementos.Substring(0, otrosComplementos.Length - 3));
                                        otrosComplementos = "";
                                        ultimoLeidoCircuntancial = false;
                                        descripcionDefinida = false;
                                    }
                                    if (!string.IsNullOrEmpty(objetoDirecto))
                                    {
                                        AgregarObjetoDirecto(objetoDirecto.Substring(0, objetoDirecto.Length - 3));
                                        objetoDirecto = "";
                                        ultimoLeidoCircuntancial = false;
                                        descripcionDefinida = false;
                                    }
                                    if (!string.IsNullOrEmpty(objetoIndirecto))
                                    {
                                        if (objetoDirectoPersonal)
                                        {
                                            ultimoLeidoCircuntancial = false;
                                            descripcionDefinida = false;
                                            AgregarObjetoIndirecto(objetoIndirecto);
                                        }
                                    }
                                }
                                objetoDirectoPersonal = false;
                            }
                            else
                            {
                                ultimoLeidoArticulo = false;
                            }
                        }
                    }
                    //Añado los complementos a sus listas
                    AgregarObjetoDirecto(objetoDirecto);
                    AgregarObjetoIndirecto(objetoIndirecto);
                    AgregarComplemento(otrosComplementos);
                }
            }

            //Si no hay objetos directos pero si indirectos, considero los objetos indirectos como directos
            if ((this.ObjetosDirectos.Count == 0) && (this.ObjetosIndirectos.Count > 0))
            {
                ObtenerObjetoDirectoDeObjetoIndirecto();
            }
        }

        /// <summary>
        /// Agrega un objeto directo a la lista de objetos directos
        /// </summary>
        /// <param name="pObjetoDirecto">Objeto directo</param>
        private void AgregarObjetoDirecto(string pObjetoDirecto)
        {
            if (!string.IsNullOrEmpty(pObjetoDirecto))
            {
                pObjetoDirecto = pObjetoDirecto.Trim();
                if (!ObjetosDirectos.Contains(pObjetoDirecto))
                {
                    ObjetosDirectos.Add(pObjetoDirecto);
                }
            }
        }

        /// <summary>
        /// Agrega un objeto indirecto a la lista de objetos indirectos
        /// </summary>
        /// <param name="pObjetoIndirecto">Objeto indirecto</param>
        private void AgregarObjetoIndirecto(string pObjetoIndirecto)
        {
            if (!string.IsNullOrEmpty(pObjetoIndirecto))
            {
                pObjetoIndirecto = pObjetoIndirecto.Trim();
                if (!ObjetosIndirectos.Contains(pObjetoIndirecto))
                {
                    ObjetosIndirectos.Add(pObjetoIndirecto);
                }
            }
        }

        /// <summary>
        /// Agrega un complemento a la lista de complementos
        /// </summary>
        /// <param name="pComplemento">Complemento</param>
        private void AgregarComplemento(string pComplemento)
        {
            if (!string.IsNullOrEmpty(pComplemento))
            {
                pComplemento = pComplemento.Trim();
                if (!OtrosComplementos.Contains(pComplemento))
                {
                    OtrosComplementos.Add(pComplemento);
                }
            }
        }

        /// <summary>
        /// Si la frase no tiene objeto directo pero sí indirecto, considero el objeto indirecto como objeto directo de la frase
        /// </summary>
        private void ObtenerObjetoDirectoDeObjetoIndirecto()
        {
            this.ObjetosDirectos = this.ObjetosIndirectos;
        }

        /// <summary>
        /// Verdad si la frase es una frase gnoss.
        /// </summary>
        /// <param name="pFrase">Frase candidata a frase gnoss.</param>
        /// <returns></returns>
        internal static bool EsFraseGNOSSValida(string pFrase)
        {
            return (ObtenerErroresEnFraseGnoss(pFrase) == null);
        }

        /// <summary>
        /// Devuelve un mensaje con las causas por las que la frase no es una frase Gnoss
        /// </summary>
        /// <param name="pFrase">Frase que se valida</param>
        /// <returns></returns>
        internal static string ObtenerErroresEnFraseGnoss(string pFrase)
        {
            string errores = null;
            //Quito los espacios
            pFrase = pFrase.Trim();

            if (!string.IsNullOrEmpty(pFrase))
            {
                //Quito los puntos
                pFrase.Replace('.', ' ');

                //Pongo la primera letra en mayúsculas
                if (Char.ToUpper(pFrase[0]).Equals(pFrase[0]))
                {
                    pFrase = Char.ToUpper(pFrase[0]) + pFrase.Substring(1);
                }

                //compruebo si comienza con un verbo en infinitivo
                List<string> listaSeparadores = new List<string>(AnalizadorSintactico.SEPARADORES);
                listaSeparadores.Add(" ");
                string[] listaPalabras = pFrase.Split(listaSeparadores.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                if ((listaPalabras.Length > 0) && (AnalizadorSintactico.EsVerbo(listaPalabras[0])))
                {
                    FraseGnoss fraseGnoss = new FraseGnoss(pFrase);
                    //compruebo si la frase tiene objeto directo
                    if (fraseGnoss.ObjetosDirectos.Count == 0)
                    {
                        errores = "El verbo de la frase GNOSS debe tener al menos un objeto directo.";
                    }
                }
                else
                {
                    errores = "La frase GNOSS debe comenzar por un verbo en infinitivo.";
                }
            }
            else if (string.IsNullOrEmpty(pFrase))
            {
                errores = "La frase GNOSS no puede estar vacía.";
            }

            return errores;
        }

        #endregion
    }
}
