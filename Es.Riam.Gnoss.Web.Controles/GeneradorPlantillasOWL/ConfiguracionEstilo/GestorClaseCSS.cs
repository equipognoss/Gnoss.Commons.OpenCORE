using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.Controles.GeneradorPlantillasOWL.ConfiguracionEstilo
{
    /// <summary>
    /// Gestor para las clases de configuración de plantillas ontológicas.
    /// </summary>
    public class GestorClaseCSS
    {
        #region Miembros

        /// <summary>
        /// lLista con los estilo de una ontología.
        /// </summary>
        private Dictionary<string, ClaseCSS> mListaEstilos;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor público.
        /// </summary>
        /// <param name="pListaEstilos"></param>
        public GestorClaseCSS(Dictionary<string, ClaseCSS> pListaEstilos)
        {
            mListaEstilos = pListaEstilos;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Agrega el título a una entidad o propiedad en vista de edición o vista previa.
        /// </summary>
        /// <param name="pEntidad">Entidad a la que hay que agregar el título o null si es una propiedad</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad si es propiedad o el tipo de entidad si es esta última</param>
        /// <param name="pTitulo">Título, o NULL si no debe haber atributo</param>
        /// <param name="pVistaPrevia">Indica si se debe agregar a la vista previa o no</param>
        public void AgregarTituloAPropiedadOEntidad(ElementoOntologia pEntidad, string pNombrePropiedad, string pTitulo, bool pVistaPrevia, string pIdioma)
        {
            string nombreClase = ClasesLectorCSS.EspecificacionPropiedad + pNombrePropiedad;

            if (pEntidad != null)
            {
                nombreClase = ClasesLectorCSS.EspecificacionEntidad + pEntidad.TipoEntidad;
            }

            string modifAtributo = "";
            if (pIdioma != ClasesLectorCSS.IdiomaES)
            {
                modifAtributo += "_" + pIdioma;
            }

            if (!mListaEstilos.ContainsKey(nombreClase))
            {
                if (pTitulo == null || pTitulo != "")
                {
                    string tituloFinal = "";
                    if (pTitulo != null)
                    {
                        tituloFinal = pTitulo;
                    }

                    Dictionary<string, string> atributos = new Dictionary<string, string>();
                    if (!pVistaPrevia)
                    {
                        atributos.Add(AtributosLectorCSS.NombrePropiedadEntidad + modifAtributo, tituloFinal);
                    }
                    else
                    {
                        atributos.Add(AtributosLectorCSS.NombrePropiedadEntidadLectura + modifAtributo, tituloFinal);
                    }
                    ClaseCSS clase = new ClaseCSS(nombreClase, atributos);

                    mListaEstilos.Add(clase.Nombre, clase);
                }
            }
            else
            {
                if (!pVistaPrevia)
                {
                    mListaEstilos[nombreClase].Atributos.Remove(AtributosLectorCSS.NombrePropiedadEntidad + modifAtributo);

                    if (pTitulo != null && pTitulo != "")
                    {
                        mListaEstilos[nombreClase].Atributos.Add(AtributosLectorCSS.NombrePropiedadEntidad + modifAtributo, pTitulo);
                    }
                    else if (pTitulo == null)
                    {
                        //Al ser null, no debe mostrarse el titulo -> Ponerlo a vacío:
                        mListaEstilos[nombreClase].Atributos.Add(AtributosLectorCSS.NombrePropiedadEntidad + modifAtributo, "");
                    }
                }
                else
                {
                    mListaEstilos[nombreClase].Atributos.Remove(AtributosLectorCSS.NombrePropiedadEntidadLectura + modifAtributo);

                    if (pTitulo != null && pTitulo != "")
                    {
                        mListaEstilos[nombreClase].Atributos.Add(AtributosLectorCSS.NombrePropiedadEntidadLectura + modifAtributo, pTitulo);
                    }
                    else if (pTitulo == null)
                    {
                        //Al ser null, no debe mostrarse el titulo -> Ponerlo a vacío:
                        mListaEstilos[nombreClase].Atributos.Add(AtributosLectorCSS.NombrePropiedadEntidadLectura + modifAtributo, "");
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene las propiedades ordenadas según el archivo de configuración.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pCrearOrdenSiNoHay">Indica si se debe crear un orden en caso de no haberlo</param>
        /// <returns>Lista ordenada</returns>
        public List<Propiedad> ObtenerPropiedadesOrdenadasEntidad(ElementoOntologia pEntidad, bool pCrearOrdenSiNoHay)
        {
            string nombreClase = ClasesLectorCSS.OrdenEntidad + pEntidad.TipoEntidad;

            if (mListaEstilos.ContainsKey(nombreClase))
            {
                List<Propiedad> listaOrdenadaDevolver = new List<Propiedad>();

                SortedDictionary<int, Propiedad> listaOrdenPropiedad = new SortedDictionary<int, Propiedad>();
                List<Propiedad> listaPropiedadesSinOrden = new List<Propiedad>();
                foreach (Propiedad propiedad in pEntidad.Propiedades)
                {
                    if (mListaEstilos[nombreClase].Atributos.ContainsKey(propiedad.Nombre))
                    {
                        listaOrdenPropiedad.Add(int.Parse(mListaEstilos[nombreClase].Atributos[propiedad.Nombre]), propiedad);
                    }
                    else
                    {
                        listaPropiedadesSinOrden.Add(propiedad);
                    }
                }

                listaOrdenadaDevolver.AddRange(listaOrdenPropiedad.Values);

                listaOrdenadaDevolver.AddRange(listaPropiedadesSinOrden);

                return listaOrdenadaDevolver;
            }
            else
            {

                if (pCrearOrdenSiNoHay)
                {
                    Dictionary<string, string> atributos = new Dictionary<string, string>();

                    int count = 0;
                    foreach (Propiedad propiedad in pEntidad.PropiedadesOrdenadas)
                    {
                        atributos.Add(propiedad.Nombre, count.ToString());
                        count++;
                    }

                    mListaEstilos.Add(nombreClase, new ClaseCSS(nombreClase, atributos));
                }

                return pEntidad.PropiedadesOrdenadas;
            }
        }

        /// <summary>
        /// Obtiene las propiedades representantes de una entidad.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <returns>Lista de propiedades representantes de una entidad</returns>
        public Dictionary<Propiedad, int> ObtenerPropiedadesRepresentantesEntidad(ElementoOntologia pEntidad)
        {
            string nombreClase = ClasesLectorCSS.EspecificacionEntidad + pEntidad.TipoEntidad;
            Dictionary<Propiedad, int> listaPropiedadesRepr = new Dictionary<Propiedad, int>();

            if (mListaEstilos.ContainsKey(nombreClase) && mListaEstilos[nombreClase].Atributos.ContainsKey(AtributosLectorCSS.RepresentanteEntidad))
            {
                char[] separador = {','};
                string[] representantes = mListaEstilos[nombreClase].Atributos[AtributosLectorCSS.RepresentanteEntidad].Split(separador, StringSplitOptions.RemoveEmptyEntries);

                foreach (string representante in representantes)
                {
                    //Extraigo el nombre de la propiedad:
                    string nombrePropiedad = representante.Substring(0, representante.IndexOf("["));

                    string numCarcYCorchete = representante.Substring(representante.IndexOf("[")+1);
                    //Extraigo el número de caracteres:
                    int numCaracteres = int.Parse(numCarcYCorchete.Substring(0, numCarcYCorchete.Length - 1));

                    if (nombrePropiedad.Trim() == pEntidad.TipoEntidad) //Hay que agregar el tipo de entidad
                    {
                        listaPropiedadesRepr.Add(new PropiedadPlantilla(pEntidad.TipoEntidad, pEntidad.Ontologia), numCaracteres);
                    }
                    else
                    {
                        foreach (Propiedad propiedad in pEntidad.Propiedades)
                        {
                            if (propiedad.Nombre == nombrePropiedad)
                            {
                                listaPropiedadesRepr.Add(propiedad, numCaracteres);
                                break;
                            }
                        }
                    }
                }
            }

            return listaPropiedadesRepr;
        }

        /// <summary>
        /// Varia, según pVariacion, el orden de la propiedad con nombre pNombrePropiedad de la entidad pEntidad.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pVariacion">Variación</param>
        public void VariarOrdenPropiedadDeEntidad(ElementoOntologia pEntidad, string pNombrePropiedad, int pVariacion)
        {
            VariarOrdenPropiedadDeEntidad(pEntidad.TipoEntidad, pEntidad.Propiedades, pNombrePropiedad, pVariacion);
        }

        /// <summary>
        /// Varia, según pVariacion, el orden de la propiedad con nombre pNombrePropiedad de la entidad pEntidad.
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// <param name="pPropiedadesEntidad">Propiedades de la entidad</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pVariacion">Variación</param>
        public void VariarOrdenPropiedadDeEntidad(string pTipoEntidad, List<Propiedad> pPropiedadesEntidad, string pNombrePropiedad, int pVariacion)
        {
            Dictionary<string, string> atributos = mListaEstilos[ClasesLectorCSS.OrdenEntidad + pTipoEntidad].Atributos;

            int ordenActual = 0;

            if (atributos.ContainsKey(pNombrePropiedad))
            {
                ordenActual = int.Parse(atributos[pNombrePropiedad]);
            }
            else
            {
                int ordenMaximo = 0;
                foreach (string orden in atributos.Values)
                {
                    if (int.Parse(orden) > ordenMaximo)
                    {
                        ordenMaximo = int.Parse(orden);
                    }
                }

                ordenActual = ordenMaximo + 1;
            }
            int ordenFinal = ordenActual + pVariacion;

            //if ((pVariacion == 1 && ordenActual < (pEntidad.Propiedades.Count - 1)) || (pVariacion == -1 && ordenActual > 0))
            if (ordenFinal >= 0 && ordenFinal < pPropiedadesEntidad.Count)
            {
                foreach (Propiedad propiedad in pPropiedadesEntidad)
                {
                    if (atributos.ContainsKey(propiedad.Nombre) && int.Parse(atributos[propiedad.Nombre]) == ordenFinal)
                    {
                        atributos[propiedad.Nombre] = ordenActual.ToString();
                        break;
                    }
                }


                atributos[pNombrePropiedad] = ordenFinal.ToString();
            }
        }

        /// <summary>
        /// Varia, según pVariacion, el orden de la propiedad representante con nombre pNombrePropiedad de la entidad pEntidad.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pVariacion">Variación</param>
        public void VariarOrdenRepresentanteDeEntidad(ElementoOntologia pEntidad, string pNombrePropiedad, int pVariacion)
        {
            Dictionary<string, string> atributos = mListaEstilos[ClasesLectorCSS.EspecificacionEntidad + pEntidad.TipoEntidad].Atributos;

            if (atributos.ContainsKey(AtributosLectorCSS.RepresentanteEntidad))
            {
                char[] coma = {','};
                string[] representantes = atributos[AtributosLectorCSS.RepresentanteEntidad].Split(coma, StringSplitOptions.RemoveEmptyEntries);

                int posicionActual = 0;
                int numeroCaracteres = -3;//Por defecto -3

                foreach (string representante in representantes)
                {
                    //Extraigo el nombre de la propiedad:
                    string nombrePropiedad = representante.Substring(0, representante.IndexOf("["));
                    string numCarcYCorchete = representante.Substring(representante.IndexOf("[") + 1);
                    //Extraigo el número de caracteres:
                    numeroCaracteres = int.Parse(numCarcYCorchete.Substring(0, numCarcYCorchete.Length - 1));

                    if (nombrePropiedad.Equals(pNombrePropiedad))
                    {
                        break;
                    }

                    posicionActual++;
                }

                int posicionFinal = posicionActual + pVariacion;

                if (posicionFinal >= 0 && posicionFinal < representantes.Length)
                {
                    string nuevoOrden = "";

                    int count = 0;
                    string separador = "";
                    foreach (string representante in representantes)
                    {
                        //Extraigo el nombre de la propiedad:
                        string nombrePropiedad = representante.Substring(0, representante.IndexOf("["));

                        if (nombrePropiedad != pNombrePropiedad)
                        {
                            if (posicionFinal == count)
                            {
                                if (pVariacion == -1)
                                {
                                    nuevoOrden += separador + pNombrePropiedad + "[" + numeroCaracteres + "]";
                                    separador = ",";
                                    nuevoOrden += separador + representante;
                                }
                                else //Es 1
                                {
                                    nuevoOrden += separador + representante;
                                    separador = ",";
                                    nuevoOrden += separador + pNombrePropiedad + "[" + numeroCaracteres + "]";
                                }
                            }
                            else
                            {
                                nuevoOrden += separador + representante;
                                separador = ",";
                            }
                        }

                        count++;
                    }

                    atributos.Remove(AtributosLectorCSS.RepresentanteEntidad);
                    atributos.Add(AtributosLectorCSS.RepresentanteEntidad, nuevoOrden);
                }
            }
        }

        /// <summary>
        /// Da valor al número de caracteres de un representante de una entidad.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad representante</param>
        /// <param name="pNumCaracteres">Número de caracteres</param>
        public void VariarValorNumCarateresRepresentanteDeEntidad(ElementoOntologia pEntidad, string pNombrePropiedad, int pNumCaracteres)
        {
            Dictionary<string, string> atributos = mListaEstilos[ClasesLectorCSS.EspecificacionEntidad + pEntidad.TipoEntidad].Atributos;

            if (atributos.ContainsKey(AtributosLectorCSS.RepresentanteEntidad))
            {
                char[] separador = { ',' };
                string[] representantes = atributos[AtributosLectorCSS.RepresentanteEntidad].Split(separador, StringSplitOptions.RemoveEmptyEntries);

                foreach (string representante in representantes)
                {
                    //Extraigo el nombre de la propiedad:
                    string nombrePropiedad = representante.Substring(0, representante.IndexOf("["));
                    string numCarcYCorchete = representante.Substring(representante.IndexOf("[") + 1);
                    //Extraigo el número de caracteres:
                    int numeroCaracteres = int.Parse(numCarcYCorchete.Substring(0, numCarcYCorchete.Length - 1));

                    if (nombrePropiedad.Equals(pNombrePropiedad))
                    {
                        atributos[AtributosLectorCSS.RepresentanteEntidad] = atributos[AtributosLectorCSS.RepresentanteEntidad].Replace(representante, nombrePropiedad + "[" + pNumCaracteres.ToString() + "]");

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Agregar un nuevo representante a una entidad.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad representante</param>
        public void AgregarRepresentanteAEntidad(ElementoOntologia pEntidad, string pNombrePropiedad)
        {
            if (!mListaEstilos.ContainsKey(ClasesLectorCSS.EspecificacionEntidad + pEntidad.TipoEntidad))
            {
                Dictionary<string, string> atributos = new Dictionary<string,string>();
                atributos.Add(AtributosLectorCSS.RepresentanteEntidad, pNombrePropiedad + "[-3]");

                ClaseCSS claseCss = new ClaseCSS(ClasesLectorCSS.EspecificacionEntidad + pEntidad.TipoEntidad, atributos);
                mListaEstilos.Add(claseCss.Nombre, claseCss);
            }
            else
            {
                Dictionary<string, string> atributos = mListaEstilos[ClasesLectorCSS.EspecificacionEntidad + pEntidad.TipoEntidad].Atributos;

                if (atributos.ContainsKey(AtributosLectorCSS.RepresentanteEntidad))
                {
                    if (!atributos[AtributosLectorCSS.RepresentanteEntidad].Contains(pNombrePropiedad + "["))
                    {
                        atributos[AtributosLectorCSS.RepresentanteEntidad] += "," + pNombrePropiedad + "[-3]";
                    }
                }
                else
                {
                    atributos.Add(AtributosLectorCSS.RepresentanteEntidad, pNombrePropiedad + "[-3]");
                }
            }
        }

        /// <summary>
        /// Elimina un representante de una entidad.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad representante</param>
        public void EliminarRepresentanteDeEntidad(ElementoOntologia pEntidad, string pNombrePropiedad)
        {
            Dictionary<string, string> atributos = mListaEstilos[ClasesLectorCSS.EspecificacionEntidad + pEntidad.TipoEntidad].Atributos;

            if (atributos.ContainsKey(AtributosLectorCSS.RepresentanteEntidad))
            {
                char[] coma = { ',' };
                string[] representantes = atributos[AtributosLectorCSS.RepresentanteEntidad].Split(coma, StringSplitOptions.RemoveEmptyEntries);

                string nuevoOrden = "";

                string separador = "";
                foreach (string representante in representantes)
                {
                    //Extraigo el nombre de la propiedad:
                    string nombrePropiedad = representante.Substring(0, representante.IndexOf("["));

                    if (nombrePropiedad != pNombrePropiedad)
                    {
                        nuevoOrden += separador + representante;
                        separador = ",";
                    }
                }

                atributos.Remove(AtributosLectorCSS.RepresentanteEntidad);
                atributos.Add(AtributosLectorCSS.RepresentanteEntidad, nuevoOrden);
            }
        }

        /// <summary>
        /// Devuelve el valor del título de la entidad, si lo tiene, si no NULL.
        /// </summary>
        /// <param name="pEntidad">Entidad</param>
        /// <param name="pVistaPrevia">Indica si se debe agregar a la vista previa o no</param>
        /// <returns>Valor del título de la entidad, si lo tiene, si no NULL.</returns>
        /// <param name="pIdioma">Idioma</param>
        public string ObtenerValorTituloEntidad(ElementoOntologia pEntidad, string pNombrePropiedad , bool pVistaPrevia, string pIdioma)
        {
            string titulo = null;

            string nombreClase = ClasesLectorCSS.EspecificacionPropiedad + pNombrePropiedad;
            if (pEntidad != null)
            {
                nombreClase = ClasesLectorCSS.EspecificacionEntidad + pEntidad.TipoEntidad;
            }

            string modifAtributo = "";
            if (pIdioma != ClasesLectorCSS.IdiomaES)
            {
                modifAtributo += "_" + pIdioma;
            }

            if (mListaEstilos.ContainsKey(nombreClase))
            {
                if (!pVistaPrevia && mListaEstilos[nombreClase].Atributos.ContainsKey(AtributosLectorCSS.NombrePropiedadEntidad + modifAtributo))
                {
                    titulo = mListaEstilos[nombreClase].Atributos[AtributosLectorCSS.NombrePropiedadEntidad + modifAtributo];
                }
                else if (mListaEstilos[nombreClase].Atributos.ContainsKey(AtributosLectorCSS.NombrePropiedadEntidadLectura + modifAtributo))
                {
                    titulo = mListaEstilos[nombreClase].Atributos[AtributosLectorCSS.NombrePropiedadEntidadLectura + modifAtributo];
                }
            }

            return titulo;
        }

        /// <summary>
        /// Agrega el tipo de campo de una propiedad.
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pTipo">Tipo de campo</param>
        public void AgregarTipoCampoAPropiedad(string pNombrePropiedad, string pTipo)
        {
            string nombreClase = ClasesLectorCSS.ControlEdicionTexto + pNombrePropiedad;
            string nombreClaseVisPre = ClasesLectorCSS.ControlLecturaTexto + pNombrePropiedad;

            if (mListaEstilos.ContainsKey(nombreClase))
            {
                mListaEstilos[nombreClase].Atributos.Remove(AtributosLectorCSS.GnossAtrrTipoCampo);

                if (pTipo != null && pTipo != "")
                {
                    mListaEstilos[nombreClase].Atributos.Add(AtributosLectorCSS.GnossAtrrTipoCampo, pTipo);
                }
                else
                {
                    //Elimino estas propiedades, ya que son de imágenes
                    mListaEstilos[nombreClase].Atributos.Remove(AtributosLectorCSS.CssAnchuraMaxima);
                    mListaEstilos[nombreClase].Atributos.Remove(AtributosLectorCSS.CssAlturaMaxima);
                }

                //Si no hay más atributos para la propiedad, la borro del archivo:
                if (mListaEstilos[nombreClase].Atributos.Count == 0)
                {
                    mListaEstilos.Remove(nombreClase);
                }
            }

            if (mListaEstilos.ContainsKey(nombreClaseVisPre))
            {
                mListaEstilos[nombreClaseVisPre].Atributos.Remove(AtributosLectorCSS.GnossAtrrTipoCampo);

                if (pTipo != null && pTipo != "")
                {
                    mListaEstilos[nombreClaseVisPre].Atributos.Add(AtributosLectorCSS.GnossAtrrTipoCampo, pTipo);
                }
                else
                {
                    //Elimino estas propiedades, ya que son de imágenes
                    mListaEstilos[nombreClaseVisPre].Atributos.Remove(AtributosLectorCSS.CssAnchuraMaxima);
                    mListaEstilos[nombreClaseVisPre].Atributos.Remove(AtributosLectorCSS.CssAlturaMaxima);
                }

                //Si no hay más atributos para la propiedad, la borro del archivo:
                if (mListaEstilos[nombreClaseVisPre].Atributos.Count == 0)
                {
                    mListaEstilos.Remove(nombreClaseVisPre);
                }
            }
            
            if (pTipo != null && pTipo != "")
            {
                if (!mListaEstilos.ContainsKey(nombreClase))
                {
                    Dictionary<string, string> atributos = new Dictionary<string, string>();
                    atributos.Add(AtributosLectorCSS.GnossAtrrTipoCampo, pTipo);

                    ClaseCSS clase = new ClaseCSS(nombreClase, atributos);
                    mListaEstilos.Add(clase.Nombre, clase);
                }

                if (!mListaEstilos.ContainsKey(nombreClaseVisPre))
                {
                    Dictionary<string, string> atributosVistPre = new Dictionary<string, string>();
                    atributosVistPre.Add(AtributosLectorCSS.GnossAtrrTipoCampo, pTipo);

                    ClaseCSS claseVistPre = new ClaseCSS(nombreClaseVisPre, atributosVistPre);
                    mListaEstilos.Add(claseVistPre.Nombre, claseVistPre);
                }
            }

            AgregarSiProdeceColorFondoBlanco(pNombrePropiedad);
            if (mListaEstilos.ContainsKey(nombreClaseVisPre) && mListaEstilos[nombreClaseVisPre].Atributos.ContainsKey(AtributosLectorCSS.GnossAtrrTipoCampo) && (mListaEstilos[nombreClaseVisPre].Atributos[AtributosLectorCSS.GnossAtrrTipoCampo] == ValorAtributosLectorCSS.CampoEspecialImagen || mListaEstilos[nombreClaseVisPre].Atributos[AtributosLectorCSS.GnossAtrrTipoCampo] == ValorAtributosLectorCSS.CampoEspecialVideo))
            {
                mListaEstilos[nombreClaseVisPre].Atributos.Remove(AtributosLectorCSS.CssColorFondo);
                mListaEstilos[nombreClaseVisPre].Atributos.Remove(AtributosLectorCSS.CssBorde);
            }
        }

        /// <summary>
        /// Obtiene el tipo de campo de una propiedad si lo tiene, o NULL en caso contrario.
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <returns>Tipo de campo</returns>
        public string ObtenerTipoCampoDePropiedad(string pNombrePropiedad)
        {
            string nombreClase = ClasesLectorCSS.ControlEdicionTexto + pNombrePropiedad;

            if (mListaEstilos.ContainsKey(nombreClase) && mListaEstilos[nombreClase].Atributos.ContainsKey(AtributosLectorCSS.GnossAtrrTipoCampo))
            {
                return mListaEstilos[nombreClase].Atributos[AtributosLectorCSS.GnossAtrrTipoCampo].Trim();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Agrega un atributo Css y su valor a una propiedad.
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pVistaPrevia">Indica si es vista previa o no</param>
        /// <param name="pAtributo">Nombre del atributo</param>
        /// <param name="pValor">Valor del atributo</param>
        public void AgregarEstiloCssAPropiedad(string pNombrePropiedad, bool pVistaPrevia, string pAtributo, string pValor)
        {
            string nombreClase = ClasesLectorCSS.ControlEdicionTexto + pNombrePropiedad;

            if (pVistaPrevia)
            {
                nombreClase = ClasesLectorCSS.ControlLecturaTexto + pNombrePropiedad;
            }

            AgregarValorAAtriburtoClase(nombreClase, pAtributo, pValor);

            AgregarSiProdeceColorFondoBlanco(pNombrePropiedad);
        }

        /// <summary>
        /// Agrega un atributo y su valor a una propiedad.
        /// </summary>
        /// <param name="pClase">Nombre de la clase</param>
        /// <param name="pAtributo">Nombre del atributo</param>
        /// <param name="pValor">Valor del atributo</param>
        public void AgregarValorAAtriburtoClase(string pNombreClase, string pAtributo, string pValor)
        {
            if (mListaEstilos.ContainsKey(pNombreClase))
            {
                mListaEstilos[pNombreClase].Atributos.Remove(pAtributo);

                if (pValor != null && pValor != "")
                {
                    mListaEstilos[pNombreClase].Atributos.Add(pAtributo, pValor);
                }

                //Si no hay más atributos para la propiedad, la borro del archivo:
                if (mListaEstilos[pNombreClase].Atributos.Count == 0)
                {
                    mListaEstilos.Remove(pNombreClase);
                }
            }
            else if (pValor != null && pValor != "")
            {
                Dictionary<string, string> atributos = new Dictionary<string, string>();
                atributos.Add(pAtributo, pValor);

                ClaseCSS clase = new ClaseCSS(pNombreClase, atributos);
                mListaEstilos.Add(clase.Nombre, clase);
            }
        }

        /// <summary>
        /// Devuelve el valor un atributo Css de una propiedad.
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pVistaPrevia">Indica si es vista previa o no</param>
        /// <param name="pAtributo">Nombre del atributo</param>
        /// <returns>Valor del atributo</returns>
        public string ObtenerValorDeAtributoCssDePropiedad(string pNombrePropiedad, bool pVistaPrevia, string pAtributo)
        {
            string nombreClase = ClasesLectorCSS.ControlEdicionTexto + pNombrePropiedad;

            if (pVistaPrevia)
            {
                nombreClase = ClasesLectorCSS.ControlLecturaTexto + pNombrePropiedad;
            }

            if (mListaEstilos.ContainsKey(nombreClase) && mListaEstilos[nombreClase].Atributos.ContainsKey(pAtributo))
            {
                return mListaEstilos[nombreClase].Atributos[pAtributo];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Agrega el valor un atributo salto de linea de una propiedad.
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pVistaPrevia">Indica si es vista previa o no</param>
        /// <param name="pAgregar">Indica si hay que agregar o quitar el valor</param>
        public void AgregarSaltoLineaPropiedad(string pNombrePropiedad, bool pVistaPrevia, bool pAgregar)
        {
            string nombreClase = ClasesLectorCSS.LabelEdicion + pNombrePropiedad;

            if (pVistaPrevia)
            {
                nombreClase = ClasesLectorCSS.LabelVistaPrevia + pNombrePropiedad;
            }

            if (mListaEstilos.ContainsKey(nombreClase))
            {
                mListaEstilos[nombreClase].Atributos.Remove(AtributosLectorCSS.GnossAtrrSaltoLinea);

                if (pAgregar && mListaEstilos[nombreClase].Atributos.Count > 0)
                {
                    mListaEstilos[nombreClase].Atributos.Add(AtributosLectorCSS.GnossAtrrSaltoLinea, ValorAtributosLectorCSS.True);
                }

                if (!pAgregar)
                {
                    mListaEstilos[nombreClase].Atributos.Add(AtributosLectorCSS.GnossAtrrSaltoLinea, ValorAtributosLectorCSS.False);
                }

                //Si solo queda el elemento negrita, elimino la clase puesto que la por defecto contiene este atributo.
                if (mListaEstilos[nombreClase].Atributos.Count == 1 && mListaEstilos[nombreClase].Atributos.ContainsKey(AtributosLectorCSS.CssNegrita))
                {
                    mListaEstilos.Remove(nombreClase);
                }
            }
            else if (!pAgregar)
            {
                Dictionary<string, string> atributos = new Dictionary<string, string>();
                atributos.Add(AtributosLectorCSS.GnossAtrrSaltoLinea, ValorAtributosLectorCSS.False);
                atributos.Add(AtributosLectorCSS.CssNegrita, ValorAtributosLectorCSS.Negrita);

                ClaseCSS clase = new ClaseCSS(nombreClase, atributos);
                mListaEstilos.Add(clase.Nombre, clase);
            }
        }

        /// <summary>
        /// Devuelve el valor un atributo salto de linea de una propiedad.
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pVistaPrevia">Indica si es vista previa o no</param>
        /// <returns>Valor del atributo</returns>
        public bool ObtenerSaltoLineaPropiedad(string pNombrePropiedad, bool pVistaPrevia)
        {
            string nombreClase = ClasesLectorCSS.LabelEdicion + pNombrePropiedad;

            if (pVistaPrevia)
            {
                nombreClase = ClasesLectorCSS.LabelVistaPrevia + pNombrePropiedad;
            }

            if (mListaEstilos.ContainsKey(nombreClase) && mListaEstilos[nombreClase].Atributos.ContainsKey(AtributosLectorCSS.GnossAtrrSaltoLinea) && mListaEstilos[nombreClase].Atributos[AtributosLectorCSS.GnossAtrrSaltoLinea] == ValorAtributosLectorCSS.False)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Da valor al número de caracteres máximos de una propiedad.
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pValorComparacion">Indica que tipo de comparación se usará: mayor, menor, igual o entre </param>
        /// <param name="pNumCaracteres">Número de caracteres</param>
        /// <param name="pNumCaracteresSecundarios">Número de caracteres para el segundo valor en caso de ser entre X y Z</param>
        public void AgregarNumCarateresDePropiedad(string pNombrePropiedad, string pValorComparacion, int pNumCaracteres, int pNumCaracteresSecundarios)
        {
            string nombreClase = ClasesLectorCSS.EspecificacionPropiedad + pNombrePropiedad;

            if (mListaEstilos.ContainsKey(nombreClase))
            {
                mListaEstilos[nombreClase].Atributos.Remove(AtributosLectorCSS.GnossAtrrNumCaract);

                if (pNumCaracteres >= 0 && (pValorComparacion != ValorAtributosLectorCSS.RestriCaract_Entre || pNumCaracteresSecundarios >= 0))
                {
                    if (pValorComparacion != ValorAtributosLectorCSS.RestriCaract_Entre)
                    {
                        mListaEstilos[nombreClase].Atributos.Add(AtributosLectorCSS.GnossAtrrNumCaract, pValorComparacion + "," + pNumCaracteres.ToString());
                    }
                    else
                    {
                        mListaEstilos[nombreClase].Atributos.Add(AtributosLectorCSS.GnossAtrrNumCaract, pValorComparacion + "," + pNumCaracteres.ToString() + "," + pNumCaracteresSecundarios.ToString());
                    }
                }

                if (mListaEstilos[nombreClase].Atributos.Count == 0)
                {
                    mListaEstilos.Remove(nombreClase);
                }
            }
            else if (pNumCaracteres >= 0 && (pValorComparacion != ValorAtributosLectorCSS.RestriCaract_Entre || pNumCaracteresSecundarios >= 0))
            {
                Dictionary<string, string> atributos = new Dictionary<string, string>();

                if (pValorComparacion != ValorAtributosLectorCSS.RestriCaract_Entre)
                {
                    atributos.Add(AtributosLectorCSS.GnossAtrrNumCaract, pValorComparacion + "," + pNumCaracteres.ToString());
                }
                else
                {
                    atributos.Add(AtributosLectorCSS.GnossAtrrNumCaract, pValorComparacion + "," + pNumCaracteres.ToString() + "," + pNumCaracteresSecundarios.ToString());
                }

                ClaseCSS clase = new ClaseCSS(nombreClase, atributos);
                mListaEstilos.Add(clase.Nombre, clase);
            }
        }

        public static KeyValuePair<string, KeyValuePair<int, int>> ObtenerNumCarateresDePropiedad(string pNombrePropiedad, Dictionary<string, ClaseCSS> pListaEstilos)
        {
            string nombreClase = ClasesLectorCSS.EspecificacionPropiedad + pNombrePropiedad;
            KeyValuePair<string, KeyValuePair<int, int>> comparacionMasCaracteres = new KeyValuePair<string, KeyValuePair<int, int>>();

            if (pListaEstilos.ContainsKey(nombreClase) && pListaEstilos[nombreClase].Atributos.ContainsKey(AtributosLectorCSS.GnossAtrrNumCaract))
            {
                char[] separador = { ',' };
                string[] valoresAtrib = pListaEstilos[nombreClase].Atributos[AtributosLectorCSS.GnossAtrrNumCaract].Split(separador, StringSplitOptions.RemoveEmptyEntries);

                string comparador = valoresAtrib[0];

                int numCaracteres = int.Parse(valoresAtrib[1]);
                int numCaracteresEntre = -1;

                if (comparador == ValorAtributosLectorCSS.RestriCaract_Entre)
                {
                    numCaracteresEntre = int.Parse(valoresAtrib[2]);
                }

                comparacionMasCaracteres = new KeyValuePair<string, KeyValuePair<int, int>>(comparador, new KeyValuePair<int, int>(numCaracteres, numCaracteresEntre));
            }

            return comparacionMasCaracteres;
        }

        /// <summary>
        /// Devuelve el número de caracteres de una propiedad.
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <returns>Par con la comparación (menor, mayor, igual, entre,) y los valores de la comparación</param>
        public KeyValuePair<string, KeyValuePair<int, int>> ObtenerNumCarateresDePropiedad(string pNombrePropiedad)
        {
            return ObtenerNumCarateresDePropiedad(pNombrePropiedad, mListaEstilos);
        }

        /// <summary>
        /// Obtiene las entidades principales ordenadas según el archivo de configuración.
        /// </summary>
        /// <param name="pEntidades">Entidades principales sin ordenar</param>
        /// <returns>Lista ordenada</returns>
        public List<ElementoOntologia> ObtenerEntidadesPrincipalesOrdenadas(List<ElementoOntologia> pEntidades)
        {
            string nombreClase = ClasesLectorCSS.OrdenEntidad + AtributosPlantilla.TipoEntidadDocumentoGnoss;
            if (mListaEstilos.ContainsKey(nombreClase))
            {
                List<ElementoOntologia> listaOrdenadaDevolver = new List<ElementoOntologia>();

                SortedDictionary<int, ElementoOntologia> listaOrdenEntidad = new SortedDictionary<int, ElementoOntologia>();
                List<ElementoOntologia> listaEntidadesSinOrden = new List<ElementoOntologia>();

                foreach (ElementoOntologia entidad in pEntidades)
                {
                    if (mListaEstilos[nombreClase].Atributos.ContainsKey(entidad.TipoEntidad))
                    {
                        listaOrdenEntidad.Add(int.Parse(mListaEstilos[nombreClase].Atributos[entidad.TipoEntidad]), entidad);
                    }
                    else
                    {
                        listaEntidadesSinOrden.Add(entidad);
                    }
                }

                listaOrdenadaDevolver.AddRange(listaOrdenEntidad.Values);

                listaOrdenadaDevolver.AddRange(listaEntidadesSinOrden);

                return listaOrdenadaDevolver;
            }
            else
            {
                return pEntidades;
            }
        }

        ///// <summary>
        ///// Si una propiedad debe ser un combo con unos valores predeterminados, carga estos.
        ///// </summary>
        ///// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        ///// <param name="pPropiedad">Propiedad</param>
        //public static void CargarValoresComboPropiedad(Propiedad pPropiedad, Dictionary<string, ClaseCSS> pListaEstilos)
        //{
        //    string claseCSS = ClasesLectorCSS.ControlEdicionTexto + pPropiedad.Nombre;

        //    if (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.GnossAtrrCombo))
        //    {
        //        GestorClaseCSS.ObtenerValoresComboPropiedad(pPropiedad, pListaEstilos);
        //    }
        //}

        /// <summary>
        /// Devuelve los valores para el combo de una propiedad.
        /// </summary>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pListaEstilos">Lista con los estilo de cargados</param>
        /// <returns>Valores para el combo de una propiedad</returns>
        public static void ObtenerValoresComboPropiedad(Propiedad pPropiedad, Dictionary<string, ClaseCSS> pListaEstilos)
        {
            pPropiedad.ListaValoresPermitidos = new List<string>();
            pPropiedad.ValorDefectoNoSeleccionable = null;

            string claseCSS = ClasesLectorCSS.ControlEdicionTexto + pPropiedad.Nombre;
            if (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.GnossAtrrCombo))
            {
                char[] separador = { ',' };
                string[] valores = pListaEstilos[claseCSS].Atributos[AtributosLectorCSS.GnossAtrrCombo].Split(separador,StringSplitOptions.RemoveEmptyEntries);
                foreach (string valor in valores)
                {
                    if (valor[0] != '*')
                    {
                        pPropiedad.ListaValoresPermitidos.Add(valor);
                    }
                    else
                    {
                        pPropiedad.ValorDefectoNoSeleccionable = valor.Substring(1);
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene los idiomas disponibles en el formulario.
        /// </summary>
        /// <returns>Idiomas disponibles (codigoIdioma)</returns>
        public List<string> ObtenerIdiomasDisponibles()
        {
            return LectorCSS.ObtenerIdiomasDisponibles(mListaEstilos);
        }

        /// <summary>
        /// Agrega los idiomas disponibles para los formularios del tipo de la ontología.
        /// </summary>
        /// <param name="pIdiomas">Idiomas</param>
        public void AgregarIdiomasDisponibles(List<string> pIdiomas)
        {
            string idiomas = "";

            if (pIdiomas.Count > 0)
            {
                foreach (string idioma in pIdiomas)
                {
                    idiomas += idioma + ",";
                }

                idiomas = idiomas.Substring(0, idiomas.Length - 1);
            }

            AgregarValorAAtriburtoClase(ClasesLectorCSS.ConfiguracionGeneral, AtributosLectorCSS.IdiomasOnto, idiomas);
        }

        #region Auxiliares

        /// <summary>
        /// Agrega, si debe hacerlo, el estilo color blanco a la propiedad.
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        private void AgregarSiProdeceColorFondoBlanco(string pNombrePropiedad)
        {
            string nombreClase = ClasesLectorCSS.ControlLecturaTexto + pNombrePropiedad;

            if (mListaEstilos.ContainsKey(nombreClase))
            {
                if (!mListaEstilos[nombreClase].Atributos.ContainsKey(AtributosLectorCSS.CssColorFondo) && (!mListaEstilos[nombreClase].Atributos.ContainsKey(AtributosLectorCSS.GnossAtrrTipoCampo) || mListaEstilos[nombreClase].Atributos[AtributosLectorCSS.GnossAtrrTipoCampo] == ValorAtributosLectorCSS.Tiny))
                {
                    mListaEstilos[nombreClase].Atributos.Add(AtributosLectorCSS.CssColorFondo, ValorAtributosLectorCSS.Color_Blanco);

                    if (!mListaEstilos[nombreClase].Atributos.ContainsKey(AtributosLectorCSS.CssBorde))
                    {
                        mListaEstilos[nombreClase].Atributos.Add(AtributosLectorCSS.CssBorde, ValorAtributosLectorCSS.Sin_Borde);
                    }
                }

                if (mListaEstilos[nombreClase].Atributos.Count == 2 && mListaEstilos[nombreClase].Atributos.ContainsKey(AtributosLectorCSS.CssColorFondo))
                {
                    mListaEstilos.Remove(nombreClase);
                }
            }
        }

        #endregion

        #endregion
    }

    public class AtributosPlantilla
    {
        /// <summary>
        /// Tipo de entidad de ontología DocumentoGnoss.
        /// </summary>
        public static string TipoEntidadDocumentoGnoss = "DocumentoGnoss";

        /// <summary>
        /// Nombre de la propiedad Contenido del tipo de entidad DocumentoGnoss.
        /// </summary>
        public static string NombrePropiedadContenido = "Contenido";
    }
}
