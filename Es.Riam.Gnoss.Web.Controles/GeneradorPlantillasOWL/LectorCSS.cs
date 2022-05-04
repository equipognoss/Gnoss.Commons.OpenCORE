using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Es.Riam.Gnoss.Web.Controles.GeneradorPlantillasOWL
{
    /// <summary>
    /// Lee un archivo CSS y obtiene su estilo.
    /// </summary>
    public class LectorCSS
    {
        #region Miembros

        /// <summary>
        /// Ruta del archivo CSS.
        /// </summary>
        private string mLineaCSS;

        /// <summary>
        /// Indica si hay que quitar el punto del principio de las clases.
        /// </summary>
        private bool mQuitarPuntoClases = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Contructor del lector de CSS.
        /// </summary>
        /// <param name="pArchivoCSS">Ruta del archivo CSS a leer</param>
        public LectorCSS(string pArchivoCSS)
        {
            mLineaCSS = ConvertirArchivoEnLinea(pArchivoCSS);
        }

        /// <summary>
        /// Contructor del lector de CSS.
        /// </summary>
        /// <param name="pArchivoCSS">Ruta del archivo CSS a leer</param>
        public LectorCSS()
        {
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Obtiene una lista de pares con paramentro/valor de la clase del CSS.
        /// </summary>
        /// <param name="pClase">Clase del CSS</param>
        /// <returns>Lista de pares con paramentro/valor de la clase del CSS</returns>
        public Dictionary<string, string> ObtenerEstiloClase(string pClase)
        {
            int indiceTexto = 0;
            while (indiceTexto < mLineaCSS.Length)
            {
                //if (mLineaCSS[indiceTexto] == '.')
                if (indiceTexto == 0 || mLineaCSS[indiceTexto - 1] == '}')
                {//Comienza clase
                    indiceTexto++;
                    string nombreClase = "";
                    while (mLineaCSS[indiceTexto] < mLineaCSS.Length && mLineaCSS[indiceTexto] != '{')
                    {
                        nombreClase += mLineaCSS[indiceTexto];
                        indiceTexto++;
                    }
                    if (nombreClase == pClase)
                    {
                        string lineaDesdeIndice = mLineaCSS.Substring(indiceTexto + 1);
                        return ObtenerAtributosClase(lineaDesdeIndice.Substring(0, lineaDesdeIndice.IndexOf("}")));
                    }
                }
                else
                {
                    indiceTexto++;
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene una lista de triples con clase/(atributo/valor) de la clase del CSS.
        /// </summary>
        /// <returns>Lista de triples con clase/(atributo/valor) de la clase del CSS</returns>
        public Dictionary<string, ClaseCSS> ObtenerEstiloTodasClases()
        {
            Dictionary<string, ClaseCSS> listaEstilos = new Dictionary<string, ClaseCSS>();

            if (!mQuitarPuntoClases)
            {
                int indiceTexto = 0;
                string nombreClase = "";
                string estilos = "";

                while (indiceTexto < mLineaCSS.Length)
                {
                    nombreClase = mLineaCSS.Substring(indiceTexto, mLineaCSS.IndexOf('{', indiceTexto) - indiceTexto);
                    estilos = mLineaCSS.Substring(indiceTexto + nombreClase.Length + 1, mLineaCSS.IndexOf('}', indiceTexto) - (indiceTexto + nombreClase.Length + 1));
                    indiceTexto = mLineaCSS.IndexOf('}', indiceTexto) + 1;

                    Dictionary<string, string> listaAtributos = new Dictionary<string, string>();
                    listaAtributos = ObtenerAtributosClase(estilos);
                    listaEstilos.Add(nombreClase, new ClaseCSS(nombreClase, listaAtributos));
                }
            }
            else
            {
                int indiceTexto = 0;
                while (indiceTexto < mLineaCSS.Length)
                {
                    //if (mLineaCSS[indiceTexto] == '.')
                    if (indiceTexto == 0 || mLineaCSS[indiceTexto - 1] == '}')
                    {//Comienza clase
                        indiceTexto++;
                        string nombreClase = "";
                        while (/*mLineaCSS[*/indiceTexto/*]*/ < mLineaCSS.Length && mLineaCSS[indiceTexto] != '{')
                        {
                            nombreClase += mLineaCSS[indiceTexto];
                            indiceTexto++;
                        }

                        Dictionary<string, string> listaAtributos = new Dictionary<string, string>();
                        string lineaDesdeIndice = mLineaCSS.Substring(indiceTexto + 1);
                        listaAtributos = ObtenerAtributosClase(lineaDesdeIndice.Substring(0, lineaDesdeIndice.IndexOf("}")));
                        listaEstilos.Add(nombreClase, new ClaseCSS(nombreClase, listaAtributos));
                    }
                    else
                    {
                        indiceTexto++;
                    }
                }
            }

            return listaEstilos;
        }

        /// <summary>
        /// Devuelve los atributos de una clase.
        /// </summary>
        /// <param name="pAtributos">Cadena de texto con los atributos de una clase</param>
        /// <returns>Lista de pares Atributo/Valor</returns>
        private Dictionary<string, string> ObtenerAtributosClase(string pAtributos)
        {
            string[] atributoValor = pAtributos.Split(';');
            Dictionary<string, string> listaAtributos = new Dictionary<string,string>();

            string atributo;
            string valor;
            for (int i=0;i<atributoValor.Length;i++)
            {
                if (atributoValor[i].Trim().Length > 0)
                {
                    atributo = atributoValor[i].Substring(0, atributoValor[i].IndexOf(":")).Trim();
                    valor = atributoValor[i].Substring(atributoValor[i].IndexOf(":") + 1).Trim();
                    if (!listaAtributos.ContainsKey(atributo))
                    {
                        listaAtributos.Add(atributo, valor);
                    }
                }
            }

            return listaAtributos;
        }

        /// <summary>
        /// Concatena todas las linas de un fichero CSS en una solo y sin espacios.
        /// </summary>
        /// <returns>Linea de CSS sin espacios</returns>
        private string ConvertirArchivoEnLinea(string pArchivoCSS)
        {
            StreamReader archivo = new StreamReader(pArchivoCSS);
            string linea = archivo.ReadLine();
            string mLineaCSS = "";
            while (linea != null)
            {
                //Elimino los comentario del fichero:
                while (linea.Contains("/*") && linea.Contains("*/"))
                {
                    int inicioC = linea.IndexOf("/*");
                    int finC = linea.IndexOf("*/");
                    string comentario = linea.Substring(inicioC, finC - inicioC + 2);
                    linea = linea.Replace(comentario, "");
                }
                if (linea.Contains("/*"))
                {
                    linea = linea.Substring(0, linea.IndexOf("/*"));
                }
                if (linea.Contains("*/"))
                {
                    linea = linea.Substring(linea.IndexOf("*/") + 2);
                }

                //Añado la linea al total para tener el archivo en una sola linea.
                mLineaCSS += linea;
                linea = archivo.ReadLine();
            }
            archivo.Close();
            archivo.Dispose();

            //mLineaCSS = mLineaCSS.Replace(" ", "");
            mLineaCSS = QuitarEspaciosInnecesarios(mLineaCSS);
            //mLineaCSS = mLineaCSS.Replace("\t", "");

            return mLineaCSS;
        }

        /// <summary>
        /// Elimina todos los espacios innecesarios en un CSS en la linea pasada como parametro.
        /// </summary>
        /// <param name="pLinea">Linea que se debe limpiar</param>
        /// <returns>Linea limpia de espacios innecesarios en un CSS</returns>
        private string QuitarEspaciosInnecesarios(string pLinea)
        {
            int inicioL = 0;
            int finL = pLinea.Length;
            int indice = inicioL;
            string lineaAux = pLinea;
            string lineaFinal = "";

            while (indice < finL)
            {
                //lineaAux = pLinea.Substring(indice);
                lineaAux = lineaAux.Substring(indice);
                int indice2Puntos = lineaAux.IndexOf(":");
                int indicePuntYComa = lineaAux.IndexOf(";");

                if (indice2Puntos != -1 && indicePuntYComa != -1)
                {
                    string principioLinea = lineaAux.Substring(0, indice2Puntos);//justo antes del :
                    principioLinea = principioLinea.Trim();
                    //principioLinea = principioLinea.Replace(" ", "");
                    principioLinea = principioLinea.Replace("\t", "");

                    lineaFinal += principioLinea + lineaAux.Substring(indice2Puntos, indicePuntYComa - indice2Puntos + 1);
                    indice = indicePuntYComa;
                }
                else
                {
                    lineaFinal += lineaAux;
                    indice = finL;
                }
                indice++;
            }

            return lineaFinal;
        }

        /// <summary>
        /// Elimina los acentos del texto pasado.
        /// </summary>
        /// <param name="pTexto">Texto para quitar los acentos</param>
        /// <returns>Texto sin acentos</returns>
        public static string DesAcentar(string pTexto)
        {
            var normalizedString = pTexto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Comprueba si el atributo es Gnoss o de Css.
        /// </summary>
        /// <param name="pAtributo">Nombre del atributo</param>
        /// <returns>True si el atributo es Gnoss, false si es de Css</returns>
        public static bool AtributoGnoss(string pAtributo)
        {
            if (pAtributo.Substring(0, 5).Equals("gnoss"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Comprueba si el atributo es la clase CSS que debe tener el elemento.
        /// </summary>
        /// <param name="pAtributo">Nombre del atributo</param>
        /// <returns>True si el atributo es la clase CSS que debe tener el elemento, false si es de Css</returns>
        public static bool AtributoClaseCss(string pAtributo)
        {
            if (pAtributo.Substring(0, 9).Equals("gnoss_Css"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Genera un archivo de configuración a partir de una lista de estilos, para una plantilla.
        /// </summary>
        /// <param name="pListaEstilos">Lista de estilos</param>
        /// <returns>Ruta del fichero de configuración generado</returns>
        public static string GenerarArchivoConfiguracion(Dictionary<string, ClaseCSS> pListaEstilos)
        {
            string nombreTemporal = Path.GetRandomFileName() + ".css";
            string ruta = Path.GetTempPath() + nombreTemporal;

            string tabulacion = ConstantesArchivoConfiguracion.Tabulacion;

            StreamWriter archivo = new StreamWriter(ruta);

            archivo.WriteLine(ConstantesArchivoConfiguracion.InicioFinCabeceraTitulo);
            archivo.WriteLine(ConstantesArchivoConfiguracion.InsertarTituloCabecera("Estilo personalizado para cada propiedad"));
            archivo.WriteLine(ConstantesArchivoConfiguracion.InicioFinCabeceraTitulo);
            archivo.WriteLine("");

            foreach (string nombreClase in pListaEstilos.Keys)
            {
                if (!EsGenericaLaClase(nombreClase))
                {
                    archivo.WriteLine("." + nombreClase);
                    archivo.WriteLine("{");
                    foreach (string atributo in pListaEstilos[nombreClase].Atributos.Keys)
                    {
                        archivo.WriteLine(tabulacion + atributo + ": " + pListaEstilos[nombreClase].Atributos[atributo].Trim() + ";");
                    }
                    archivo.WriteLine("}");
                }
            }

            //InsertarEstiloGenericoArchivo(archivo);

            archivo.Flush();

            archivo.Close();
            archivo.Dispose();
            archivo = null;

            return ruta;
        }

        /// <summary>
        /// Indica si una clase es genérica o no.
        /// </summary>
        /// <param name="pNombreClase">Nombre de la clase</param>
        public static bool EsGenericaLaClase(string pNombreClase)
        {
            if (pNombreClase == ClasesLectorCSS.ControlEdicionComboGenerico)
            {
                return true;
            }
            else if (pNombreClase == ClasesLectorCSS.ControlEdicionTextoGenerico)
            {
                return true;
            }
            else if (pNombreClase == ClasesLectorCSS.ControlLecturaTextoGenerico)
            {
                return true;
            }
            else if (pNombreClase == ClasesLectorCSS.LabelGenerico)
            {
                return true;
            }
            else if (pNombreClase == ClasesLectorCSS.PanelContenedorEntidadesGenerico)
            {
                return true;
            }
            else if (pNombreClase == ClasesLectorCSS.PanelContenedorGrupoObjetosGenerico)
            {
                return true;
            }
            else if (pNombreClase == ClasesLectorCSS.PanelContenedorValoresDePropiedadesGenerico)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Métodos Estáticos

        /// <summary>
        /// Devuelve las propiedades representantes ordenadas de la entidad.
        /// </summary>
        /// <param name="pEntidad">Tipo de entidad de la que se extraer los representantes</param>
        /// <param name="pListaEstilos">Lista con la configuración leida</param>
        /// <returns>Lista de propiedades representantes ordenadas de la entidad y su codigo de representación en caso de tenerlo</returns>
        public static Dictionary<Propiedad, string> ObtenerRepresentantesEntidad(ElementoOntologia pEntidad, Dictionary<string, ClaseCSS> pListaEstilos)
        {
            return ObtenerRepresentantesEntidad(pEntidad, pEntidad.TipoEntidad, pListaEstilos);
        }

        /// <summary>
        /// Devuelve las propiedades representantes ordenadas de la entidad.
        /// </summary>
        /// <param name="pEntidad">Tipo de entidad de la que se extraer los representantes</param>
        /// <param name="pTipoEntidad">Tipo de entidad de la que se recogerán las propiedades</param>
        /// <param name="pListaEstilos">Lista con la configuración leida</param>
        /// <returns>Lista de propiedades representantes ordenadas de la entidad y su codigo de representación en caso de tenerlo</returns>
        public static Dictionary<Propiedad,string> ObtenerRepresentantesEntidad(ElementoOntologia pEntidad, string pTipoEntidad, Dictionary<string, ClaseCSS> pListaEstilos)
        {
            return ObtenerRepresentantesEntidadesVinculadas(pEntidad, null, pTipoEntidad, pListaEstilos);
        }

        /// <summary>
        /// Obtiene la propiedad con un determinado nombre que pertenece a la entidad o una de sus relaciones.
        /// </summary>
        /// <param name="pNombre">Nombre de la propiedad</param>
        /// <param name="pEntidad">Entidad</param>
        /// <returns>Propiedad con un determinado nombre que pertenece a la entidad o una de sus relaciones</returns>
        private static Propiedad ObtenerPropiedadACualquierNivelPorNombre(string pNombre, ElementoOntologia pEntidad)
        {
            if (pEntidad != null)
            {
                string nombrePropiedadEncaminante = null;
                string nombreVerdaderaPropiedad = null;

                if (pNombre.Contains("/"))
                {
                    nombrePropiedadEncaminante = pNombre.Substring(0, pNombre.IndexOf("/"));
                    nombreVerdaderaPropiedad = pNombre.Substring(pNombre.IndexOf("/") + 1);
                }

                foreach (Propiedad propiedad in pEntidad.Propiedades)
                {
                    if (nombrePropiedadEncaminante == null)
                    {
                        if (propiedad.Nombre == pNombre)
                        {
                            return propiedad;
                        }

                        if (propiedad.Tipo == TipoPropiedad.ObjectProperty)
                        {
                            if (propiedad.FunctionalProperty && propiedad.UnicoValor.Key != null)
                            {
                                Propiedad propAux = ObtenerPropiedadACualquierNivelPorNombre(pNombre, propiedad.UnicoValor.Value);
                                if (propAux != null)
                                {
                                    return propAux;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (propiedad.Tipo == TipoPropiedad.ObjectProperty && propiedad.Nombre == nombrePropiedadEncaminante)
                        {
                            if (propiedad.FunctionalProperty && propiedad.UnicoValor.Key != null)
                            {
                                Propiedad propAux = ObtenerPropiedadACualquierNivelPorNombre(nombreVerdaderaPropiedad, propiedad.UnicoValor.Value);
                                if (propAux != null)
                                {
                                    return propAux;
                                }
                            }
                            else if (!propiedad.FunctionalProperty && propiedad.ListaValores.Count > 0)
                            {
                                //Devolvemos la 1º entidad ya que solo nos intersa el nombre  de la propiedad que tenga ésta:
                                foreach (ElementoOntologia entidad in propiedad.ListaValores.Values)
                                {
                                    Propiedad propAux = ObtenerPropiedadACualquierNivelPorNombre(nombreVerdaderaPropiedad, entidad);
                                    if (propAux != null)
                                    {
                                        return propAux;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        #region Representantes entidad viculados a entidad hija

        /// <summary>
        /// Devuelve las propiedades representantes ordenadas de la entidad.
        /// </summary>
        /// <param name="pEntidad">Tipo de entidad de la que se extraer los representantes</param>
        /// <param name="pEntidadVinculada">Entidad hija de pEntidad vinculada a ésta</param>
        /// <param name="pTipoEntidad">Tipo de entidad de la que se recogerán las propiedades</param>
        /// <param name="pListaEstilos">Lista con la configuración leida</param>
        /// <returns>Lista de propiedades representantes ordenadas de la entidad y su codigo de representación en caso de tenerlo</returns>
        public static Dictionary<Propiedad, string> ObtenerRepresentantesEntidadesVinculadas(ElementoOntologia pEntidad, ElementoOntologia pEntidadVinculada, string pTipoEntidad, Dictionary<string, ClaseCSS> pListaEstilos)
        {
            Dictionary<Propiedad, string> listaPropiedadesRepr = new Dictionary<Propiedad, string>();
            string nombreAtributo = ClasesLectorCSS.EspecificacionEntidad + pTipoEntidad;
            if (pListaEstilos.ContainsKey(nombreAtributo) && pListaEstilos[nombreAtributo].Atributos.ContainsKey(AtributosLectorCSS.RepresentanteEntidad))
            {
                string[] representantes = pListaEstilos[nombreAtributo].Atributos[AtributosLectorCSS.RepresentanteEntidad].Split(',');

                foreach (string representante in representantes)
                {
                    string nombrePropiedad = representante;
                    string codigoRepresentacion = null;
                    if (nombrePropiedad.Contains("["))
                    {
                        nombrePropiedad = nombrePropiedad.Substring(0, nombrePropiedad.IndexOf("["));
                        int longitudCodigo = (representante.IndexOf("]")) - (representante.IndexOf("[") + 1);
                        codigoRepresentacion = representante.Substring(representante.IndexOf("[") + 1, longitudCodigo);
                    }
                    if (pEntidad.Superclases.Count > 0 && nombrePropiedad.Trim() == pEntidad.Superclases[0]) //Hay que agregar el tipo de entidad
                    {
                        listaPropiedadesRepr.Add(new PropiedadPlantilla(pEntidad.Superclases[0], pEntidad.Ontologia), codigoRepresentacion);
                    }
                    else
                    {
                        if (nombrePropiedad[0] != '*' && nombrePropiedad[0] != '#')
                        {
                            foreach (Propiedad propiedad in pEntidad.Propiedades)
                            {
                                if (propiedad.Nombre == nombrePropiedad)
                                {
                                    listaPropiedadesRepr.Add(propiedad, codigoRepresentacion);
                                    propiedad.TipoEntidadRepresenta = pEntidad.TipoEntidad;
                                    break;
                                }
                            }
                        }
                        else if (nombrePropiedad[0] == '*')
                        {
                            //Hay que buscar en las propiedades de las entidades de los rangos de las propiedades la entidad:
                            Propiedad propiedad = ObtenerPropiedadACualquierNivelPorNombre(nombrePropiedad.Substring(1), pEntidad);
                            if (propiedad != null)
                            {
                                listaPropiedadesRepr.Add(propiedad, codigoRepresentacion);
                                propiedad.TipoEntidadRepresenta = pEntidad.TipoEntidad;
                            }
                        }
                        else if (nombrePropiedad[0] == '#' && pEntidadVinculada != null)
                        {
                            if (nombrePropiedad[1] != '*')
                            {
                                foreach (Propiedad propiedad in pEntidadVinculada.Propiedades)
                                {
                                    if (propiedad.Nombre == nombrePropiedad.Substring(1))
                                    {
                                        listaPropiedadesRepr.Add(propiedad, codigoRepresentacion);
                                        propiedad.TipoEntidadRepresenta = pEntidadVinculada.TipoEntidad;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                //Hay que buscar en las propiedades de la entidad vinculada:
                                Propiedad propiedad = ObtenerPropiedadACualquierNivelPorNombre(nombrePropiedad.Substring(2), pEntidadVinculada);
                                if (propiedad != null)
                                {
                                    listaPropiedadesRepr.Add(propiedad, codigoRepresentacion);
                                    propiedad.TipoEntidadRepresenta = pEntidadVinculada.TipoEntidad;
                                }
                            }
                        }
                    }
                }
            }

            return listaPropiedadesRepr;
        }

        #endregion

        ///// <summary>
        ///// Devuelve el nombre para un individual de la propiedad si lo hay.
        ///// </summary>
        ///// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        ///// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        ///// <param name="pParaLectura">Indica si es para lectura o no</param>
        ///// <param name="pIdioma">Idioma del usuario</param>
        ///// <returns>Nombre para un individual de la propiedad si lo hay</returns>
        //public static string ObtenerNombrePropiedad(string pNombrePropiedad, Dictionary<string, ClaseCSS> pListaEstilos, bool pParaLectura, string pIdioma)
        //{
        //    string clase = ClasesLectorCSS.EspecificacionPropiedad + pNombrePropiedad;
        //    string atributo = AtributosLectorCSS.NombrePropiedadEntidad;

        //    if (pParaLectura)
        //    {
        //        atributo = AtributosLectorCSS.NombrePropiedadEntidadLectura;
        //    }

        //    return ObtenerValorAtributoSegunIdioma(pListaEstilos, clase, atributo, pIdioma);
        //}

        ///// <summary>
        ///// Devuelve el nombre para un individual de la propiedad si lo hay.
        ///// </summary>
        ///// <param name="pEntidad">Entidad</param>
        ///// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        ///// <param name="pParaLectura">Indica si es para lectura o no</param>
        ///// <param name="pIdioma">Idioma del usuario</param>
        ///// <returns>Nombre para un individual de la propiedad si lo hay</returns>
        //public static string ObtenerNombreTipoEntidad(ElementoOntologia pEntidad, Dictionary<string, ClaseCSS> pListaEstilos, bool pParaLectura, string pIdioma)
        //{
        //    string clase = ClasesLectorCSS.EspecificacionEntidad + pEntidad.TipoEntidad;
        //    string atributo = AtributosLectorCSS.NombrePropiedadEntidad;

        //    if (pParaLectura)
        //    {
        //        atributo = AtributosLectorCSS.NombrePropiedadEntidadLectura;
        //    }

        //    return ObtenerValorAtributoSegunIdioma(pListaEstilos, clase, atributo, pIdioma);
        //}

        /// <summary>
        /// Obtiene el valor del atributo según el idioma del usuario.
        /// </summary>
        /// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        /// <param name="pClase">Clase</param>
        /// <param name="pAtributo">Atributo</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <returns>Valor del atributo según el idioma del usuario si está, si no es otro idioma</returns>
        public static string ObtenerValorAtributoSegunIdioma(Dictionary<string, ClaseCSS> pListaEstilos, string pClase, string pAtributo, string pIdioma)
        {
            if (pListaEstilos.ContainsKey(pClase))
            {
                List<string> idiomasDisponibles = ObtenerIdiomasDisponibles(pListaEstilos);

                //1º busco el idioma del usuario.
                string modifIdioma = ModificadorIdioma(pIdioma);

                if (idiomasDisponibles.Contains(pIdioma) && pListaEstilos[pClase].Atributos.ContainsKey(pAtributo + modifIdioma))
                {
                    return pListaEstilos[pClase].Atributos[pAtributo + modifIdioma];
                }
                else //2º busco el primer idioma que tenga texto
                {
                    foreach (string idioma in ObtenerIdiomasDisponibles(pListaEstilos))
                    {
                        if (idioma != pIdioma)
                        {
                            modifIdioma = ModificadorIdioma(idioma);

                            if (pListaEstilos[pClase].Atributos.ContainsKey(pAtributo + modifIdioma))
                            {
                                return pListaEstilos[pClase].Atributos[pAtributo + modifIdioma];
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene los idiomas disponibles en el formulario.
        /// </summary>
        /// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        /// <returns>Idiomas disponibles (codigoIdioma)</returns>
        public static List<string> ObtenerIdiomasDisponibles(Dictionary<string, ClaseCSS> pListaEstilos)
        {
            List<string> listaIdio = new List<string>();
            string claseCSS = ClasesLectorCSS.ConfiguracionGeneral;

            if (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.IdiomasOnto))
            {
                char[] separador = { ',' };
                string[] valores = pListaEstilos[claseCSS].Atributos[AtributosLectorCSS.IdiomasOnto].Split(separador, StringSplitOptions.RemoveEmptyEntries);

                foreach (string valor in valores)
                {
                    listaIdio.Add(valor);
                }
            }

            return listaIdio;
        }

        /// <summary>
        /// Extrae a partir de un idioma el modificador que debe tener para el nombre de los atributos de configuración.
        /// </summary>
        /// <param name="pIdioma">Idioma</param>
        /// <returns>Modificador que debe tener para el nombre de los atributos de configuración según el idioma</returns>
        public static string ModificadorIdioma(string pIdioma)
        {
            if (pIdioma != ClasesLectorCSS.IdiomaES)
            {
                return "_" + pIdioma;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Obtiene la propiedad que sustituirá al título del recurso.
        /// </summary>
        /// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        /// <returns>Nombre de la propiedad que sustituirá al título del recurso</returns>
        public static string ObtenerPropiedadTituloDoc(Dictionary<string, ClaseCSS> pListaEstilos)
        {
            string claseCSS = ClasesLectorCSS.ConfiguracionGeneral;

            if (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.TituloDoc))
            {
                return pListaEstilos[claseCSS].Atributos[AtributosLectorCSS.TituloDoc];
            }

            return null;
        }

        /// <summary>
        /// Obtiene la propiedad que sustituirá a la descripcion del recurso.
        /// </summary>
        /// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        /// <returns>Nombre de la propiedad que sustituirá a la descripcion del recurso</returns>
        public static string ObtenerPropiedadDescripcionDoc(Dictionary<string, ClaseCSS> pListaEstilos)
        {
            string claseCSS = ClasesLectorCSS.ConfiguracionGeneral;

            if (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.DescripcionDoc))
            {
                return pListaEstilos[claseCSS].Atributos[AtributosLectorCSS.DescripcionDoc];
            }

            return null;
        }

        /// <summary>
        /// Obtiene la propiedad que sustituirá a la imagen del recurso.
        /// </summary>
        /// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        /// <returns>Nombre de la propiedad que sustituirá a la imagen del recurso</returns>
        public static string ObtenerPropiedadImagenDoc(Dictionary<string, ClaseCSS> pListaEstilos)
        {
            string claseCSS = ClasesLectorCSS.ConfiguracionGeneral;

            if (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.ImagenDoc))
            {
                string propiedad = pListaEstilos[claseCSS].Atributos[AtributosLectorCSS.ImagenDoc];

                if (propiedad.Contains(","))
                {
                    propiedad = propiedad.Split(',')[0];
                }

                return propiedad;
            }

            return null;
        }

        /// <summary>
        /// Obtiene el tamaño (ancho,alto) para la imagen sustituta del recurso.
        /// </summary>
        /// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        /// <returns>Tamaño (ancho,alto) para la imagen sustituta del recurso</returns>
        public static KeyValuePair<int,int> ObtenerTamanioImagenDoc(Dictionary<string, ClaseCSS> pListaEstilos)
        {
            string claseCSS = ClasesLectorCSS.ConfiguracionGeneral;

            if (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.ImagenDoc))
            {
                string propiedad = pListaEstilos[claseCSS].Atributos[AtributosLectorCSS.ImagenDoc];

                if (propiedad.Contains(","))
                {
                    return new KeyValuePair<int,int>(int.Parse(propiedad.Split(',')[1]), int.Parse(propiedad.Split(',')[2]));
                }
            }

            return new KeyValuePair<int,int>(0,0);
        }

        /// <summary>
        /// Comprueba si una propiedad debe mostrar su imagen mini en la vista previa.
        /// </summary>
        /// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <returns>TRUE si una propiedad debe mostrar su imagen mini en la vista previa, FALSE en caso contrario</returns>
        public static bool EsPropiedadImagenMini(Dictionary<string, ClaseCSS> pListaEstilos, string pNombrePropiedad)
        {
            string claseCSS = ClasesLectorCSS.EspecificacionPropiedad + pNombrePropiedad;

            if (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.GnossImgMiniEnVP))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Obtiene el valor del tamaño de la imagen mini para la vista previa de una propiedad.
        /// </summary>
        /// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <returns>Valor del tamaño de la imagen para la vista previa de una propiedad</returns>
        public static Dictionary<int,int> ObtenerTamanioPropiedadImagenMini(Dictionary<string, ClaseCSS> pListaEstilos, string pNombrePropiedad)
        {
            Dictionary<int, int> tamanios = new Dictionary<int, int>();
            string claseCSS = ClasesLectorCSS.EspecificacionPropiedad + pNombrePropiedad;

            if (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.GnossImgMiniEnVP))
            {
                string[] propiedades = pListaEstilos[claseCSS].Atributos[AtributosLectorCSS.GnossImgMiniEnVP].Split(',');

                for (int i = 0; i < propiedades.Length; i = i + 2)
                {
                    tamanios.Add(int.Parse(propiedades[i]), int.Parse(propiedades[i + 1]));
                }
            }

            return tamanios;
        }

        /// <summary>
        /// Obtiene el valor del tamaño de cierta imagen mini según el orden para la vista previa de una propiedad.
        /// </summary>
        /// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <param name="pNumElemento">Número de elemento</param>
        /// <returns>Valor del tamaño de la imagen para la vista previa de una propiedad</returns>
        public static KeyValuePair<int, int> ObtenerTamanioPropiedadImagenMini(Dictionary<string, ClaseCSS> pListaEstilos, string pNombrePropiedad, int pNumElemento)
        {
            Dictionary<int, int> tamanios = ObtenerTamanioPropiedadImagenMini(pListaEstilos, pNombrePropiedad);

            int count = 0;

            foreach (int key in tamanios.Keys)
            {
                if (count == pNumElemento)
                {
                    return new KeyValuePair<int, int>(key, tamanios[key]);
                }

                count++;
            }

            return new KeyValuePair<int, int>(0,0);
        }

        /// <summary>
        /// Obtiene los valores de los anchos de los tamaños de la imagen mini para la vista previa de una propiedad.
        /// </summary>
        /// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <returns>Valores de los anchos de los tamaños de la imagen mini para la vista previa de una propiedad</returns>
        public static string ObtenerTamaniosTextoPropiedadImagenMini(Dictionary<string, ClaseCSS> pListaEstilos, string pNombrePropiedad)
        {
            Dictionary<int, int> tamanios = ObtenerTamanioPropiedadImagenMini(pListaEstilos, pNombrePropiedad);
            string tamaniosTexto = "";

            foreach (int key in tamanios.Keys)
            {
                tamaniosTexto += key.ToString() + ",";
            }

            return tamaniosTexto;
        }

        /// <summary>
        /// Obtiene los parametros necesarios para montar una galería con los valores de la propiedad.
        /// </summary>
        /// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <returns>Parametros necesarios para montar una galería con los valores de la propiedad, NULL si no es galería</returns>
        public static string ObtenerParametosGaleriaPropiedad(Dictionary<string, ClaseCSS> pListaEstilos, string pNombrePropiedad)
        {
            string claseCSS = ClasesLectorCSS.EspecificacionPropiedad + pNombrePropiedad;

            if (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.GnossGaleriaElmentos))
            {
                return pListaEstilos[claseCSS].Atributos[AtributosLectorCSS.GnossGaleriaElmentos];
            }

            return null;
        }

        /// <summary>
        /// Comprueba si deben mostrarse checks y radioButtoms para una propiedad.
        /// </summary>
        /// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <returns>TRUE si deben mostrarse checks y radioButtoms para una propiedad, FALSE en caso contrario</returns>
        public static bool EsPropiedadConValoresCheck(Dictionary<string, ClaseCSS> pListaEstilos, string pNombrePropiedad)
        {
            string claseCSS = ClasesLectorCSS.EspecificacionPropiedad + pNombrePropiedad;

            if (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.GnossUsarChecks))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Comprueba si deben ocultarse el título, descipción y imagen del recurso en la ficha.
        /// </summary>
        /// <param name="pListaEstilos">Lista con el archivo de configuración</param>
        /// <returns>TRUE si deben ocultarse el título, descipción y imagen del recurso en la ficha, FALSE en caso contrario</returns>
        public static bool OcultarAtributosPrincDocumento(Dictionary<string, ClaseCSS> pListaEstilos)
        {
            string claseCSS = ClasesLectorCSS.ConfiguracionGeneral;

            if (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.OcultarTituloDescpImgDoc))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Indica si la propiedad debe tener ciertos valores para combo.
        /// </summary>
        /// <param name="pNombrePropiedad">Propiedad</param>
        /// <param name="pListaEstilos"></param>
        /// <returns>TRUE si la propiedad debe tener ciertos valores para combo, FALSE en caso contrario</returns>
        public static bool PropiedadTieneValoresCombo(Dictionary<string, ClaseCSS> pListaEstilos, string pNombrePropiedad)
        {
            string claseCSS = ClasesLectorCSS.ControlEdicionTexto + pNombrePropiedad;
            return (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.GnossAtrrCombo));
        }

        /// <summary>
        /// Obtiene el namespae de la ontología.
        /// </summary>
        /// <param name="pListaEstilos">Lista de estilos</param>
        /// <returns>Namespae de la ontología</returns>
        public static string ObtenerNamespaceOnto(Dictionary<string, ClaseCSS> pListaEstilos)
        {
            return ObtenerValorAtributoDeClase(pListaEstilos, ClasesLectorCSS.ConfiguracionGeneral, AtributosLectorCSS.NamespaceOnto);
        }

        /// <summary>
        /// Agrega el namespace a la ontología.
        /// </summary>
        /// <param name="pListaEstilos">Lista de estilos</param>
        /// <param name="pNamespace">Namespace</param>
        public static void AgregarNamespaceOnto(Dictionary<string, ClaseCSS> pListaEstilos, string pNamespace)
        {
            if (!pListaEstilos.ContainsKey(ClasesLectorCSS.ConfiguracionGeneral))
            {
                pListaEstilos.Add(ClasesLectorCSS.ConfiguracionGeneral, new ClaseCSS(ClasesLectorCSS.ConfiguracionGeneral, new Dictionary<string, string>()));
            }

            if (pListaEstilos[ClasesLectorCSS.ConfiguracionGeneral].Atributos.ContainsKey(AtributosLectorCSS.NamespaceOnto))
            {
                pListaEstilos[ClasesLectorCSS.ConfiguracionGeneral].Atributos[AtributosLectorCSS.NamespaceOnto] = pNamespace;
            }
            else
            {
                pListaEstilos[ClasesLectorCSS.ConfiguracionGeneral].Atributos.Add(AtributosLectorCSS.NamespaceOnto, pNamespace);
            }
        }

        /// <summary>
        /// Agrega un idioma a la ontología.
        /// </summary>
        /// <param name="pListaEstilos">Lista de estilos</param>
        /// <param name="pIdioma">Idioma</param>
        public static void AgregarIdiomaDisponible(Dictionary<string, ClaseCSS> pListaEstilos, string pIdioma)
        {
            if (!pListaEstilos.ContainsKey(ClasesLectorCSS.ConfiguracionGeneral))
            {
                pListaEstilos.Add(ClasesLectorCSS.ConfiguracionGeneral, new ClaseCSS(ClasesLectorCSS.ConfiguracionGeneral, new Dictionary<string, string>()));
            }

            if (pListaEstilos[ClasesLectorCSS.ConfiguracionGeneral].Atributos.ContainsKey(AtributosLectorCSS.IdiomasOnto))
            {
                pListaEstilos[ClasesLectorCSS.ConfiguracionGeneral].Atributos[AtributosLectorCSS.IdiomasOnto] = pIdioma;
            }
            else
            {
                pListaEstilos[ClasesLectorCSS.ConfiguracionGeneral].Atributos.Add(AtributosLectorCSS.IdiomasOnto, pIdioma);
            }
        }

        /// <summary>
        /// Devuelve el valor del atributos de una clase en caso de existir ambos, sino devuelve null.
        /// </summary>
        /// <param name="pListaEstilos">Lista con la configuración leida</param>
        /// <param name="pClase">Clase del atributo del valor</param>
        /// <param name="pAtributo">Atributo del valor</param>
        /// <returns>El valor del atributo de una clase si existe, null en caso contrario</returns>
        public static string ObtenerValorAtributoDeClase(Dictionary<string, ClaseCSS> pListaEstilos, string pClase, string pAtributo)
        {
            if (pListaEstilos.ContainsKey(pClase) && pListaEstilos[pClase].Atributos.ContainsKey(pAtributo))
            {
                return pListaEstilos[pClase].Atributos[pAtributo];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Devuele los estilos para el control de fecha mes/año, o null si no debe ser un control de este tipo.
        /// </summary>
        /// <param name="pListaEstilos">Lista con la configuración leida</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad</param>
        /// <returns>Estilos para el control de fecha mes/año, o null si no debe ser un control de este tipo.</returns>
        public static KeyValuePair<string, string> ObtenerEstilosCampoFechaMesAnyo(Dictionary<string, ClaseCSS> pListaEstilos, string pNombrePropiedad)
        {
            KeyValuePair<string, string> parEstilos = new KeyValuePair<string, string>();

            string claseCSS = ClasesLectorCSS.ControlEdicionTexto + pNombrePropiedad;

            if (pListaEstilos.ContainsKey(claseCSS) && pListaEstilos[claseCSS].Atributos.ContainsKey(AtributosLectorCSS.GnossCampoFechaMesAnyo))
            {
                char[] tuberia = {'|'};
                char[] coma = {','};

                string[] estilos = pListaEstilos[claseCSS].Atributos[AtributosLectorCSS.GnossCampoFechaMesAnyo].Split(tuberia, StringSplitOptions.RemoveEmptyEntries);

                string estiloMesDef = "";
                string estiloAnyoDef = "";

                if (estilos.Length > 0)
                {
                    string[] estiloMes = estilos[0].Split(coma, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string valor in estiloMes)
                    {
                        estiloMesDef += valor + ";";
                    }

                    if (estilos.Length > 1)
                    {
                        string[] estiloAnyo = estilos[1].Split(coma, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string valor in estiloAnyo)
                        {
                            estiloAnyoDef += valor + ";";
                        }
                    }
                }

                parEstilos = new KeyValuePair<string, string>(estiloMesDef, estiloAnyoDef);
            }

            return parEstilos;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece la variable que indica si hay que quitar el punto del principio de las clases.
        /// </summary>
        public bool QuitarPuntoClases
        {
            get
            {
                return mQuitarPuntoClases;
            }
            set
            {
                mQuitarPuntoClases = value;
            }
        }

        /// <summary>
        /// Establece la variable con los estilos, todo seguido y en una linea;
        /// </summary>
        public string Estilos
        {
            set
            {
                mLineaCSS = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Atributos usados en archivo de configuración de los formularios semánticos
    /// </summary>
    public class ClasesLectorCSS
    {
        /// <summary>
        /// Orden de las propiedades de una entidad.
        /// </summary>
        public static string OrdenEntidad = "OrdenEntidad_";

        /// <summary>
        /// Propiedades de una entidad.
        /// </summary>
        public static string EspecificacionEntidad = "EspefEntidad_";

        /// <summary>
        /// Propiedades de una propiedad.
        /// </summary>
        public static string EspecificacionPropiedad = "EspefPropiedad_";

        /// <summary>
        /// Propiedades de una entidad.
        /// </summary>
        public static string ConfiguracionGeneral = "ConfiguracionGeneral";

        /// <summary>
        /// Siglas para identificar el idioma español.
        /// </summary>
        public static string IdiomaES = "es";

        #region Genéricas para estilos

        /// <summary>
        /// Panel genérico para la agregación de entidades y valores de propiedades.
        /// </summary>
        public static string PanelContenedorGrupoObjetosGenerico = "panel_Cont_GrupoValores";

        /// <summary>
        /// Panel genérico para la agregación de entidades.
        /// </summary>
        public static string PanelContenedorEntidadesGenerico = "contenedor_Entidades";

        /// <summary>
        /// Panel genérico para la agregación de valores de propiedades.
        /// </summary>
        public static string PanelContenedorValoresDePropiedadesGenerico = "contedor_Valores";

        /// <summary>
        /// Clase para el control genérico de texto.
        /// </summary>
        public static string ControlEdicionTextoGenerico = "controlEdicion";

        /// <summary>
        /// Clase para el control genérico de combo.
        /// </summary>
        public static string ControlEdicionComboGenerico = "combo";

        /// <summary>
        /// Clase para el control genérico de solo lectura de texto.
        /// </summary>
        public static string ControlLecturaTextoGenerico = "controlLectura";

        /// <summary>
        /// Clase para el control genérico de solo lectura de texto.
        /// </summary>
        public static string LabelGenerico = "label";

        /// <summary>
        /// Clase para el contenedor del control de edición de texto genérico.
        /// </summary>
        public static string ContenedorControlEdicionGenerico = "contControlEdicion";

        /// <summary>
        /// Clase para el contenedor del control de solo lectura de texto genérico.
        /// </summary>
        public static string ContenedorControlLecturaGenerico = "contControlLectura";

        #endregion

        #region Específicas para estilos

        /// <summary>
        /// Clase para el control de edición de texto.
        /// </summary>
        public static string ControlEdicionTexto = "controlEdicion_";

        /// <summary>
        /// Clase para el control de solo lectura de texto.
        /// </summary>
        public static string ControlLecturaTexto = "controlLectura_";

        /// <summary>
        /// Clase para el contenedor del control de edición de texto.
        /// </summary>
        public static string ContenedorControlEdicion = "contControlEdicion_";

        /// <summary>
        /// Clase para el contenedor del control de solo lectura de texto.
        /// </summary>
        public static string ContenedorControlLectura = "contControlLectura_";

        /// <summary>
        /// Clase para un panel de una entidad.
        /// </summary>
        public static string ContenedorEntidadEdicion = "contenedorEntidadEdicion_";

        /// <summary>
        /// Clase para un panel de una entidad.
        /// </summary>
        public static string ContenedorEntidadLectura = "contenedorEntidadLectura_";

        /// <summary>
        /// Clase para el control genérico de solo lectura de texto.
        /// </summary>
        public static string LabelPropiedad = "label_";

        /// <summary>
        /// Clase para el control label de edición.
        /// </summary>
        public static string LabelEdicion = "labelEdicion_";

        /// <summary>
        /// Clase para el control label de vista previa.
        /// </summary>
        public static string LabelVistaPrevia = "labelLectura_";

        /// <summary>
        /// Clase para el control label de una entidad de edición.
        /// </summary>
        public static string LabelEntidadEdicion = "labelEntidadEdicion_";

        /// <summary>
        /// Clase para el control label de una entidad de vista previa.
        /// </summary>
        public static string LabelEntidadVistaPrevia = "labelEntidadLectura_";

        /// <summary>
        /// Clase para el control label de una entidad de edición.
        /// </summary>
        public static string LabelEntidadPropEdicion = "labelEntidadPropEdicion_";

        /// <summary>
        /// Clase para el control label de una entidad de vista previa.
        /// </summary>
        public static string LabelEntidadPropVistaPrevia = "labelEntidadPropLectura_";

        #endregion
    }

    /// <summary>
    /// Atributos usados en archivo de configuración de los formularios semánticos
    /// </summary>
    public class AtributosLectorCSS
    {
        #region Especificación Entidad

        /// <summary>
        /// Atributo para la propiedad representante de una entidad.
        /// </summary>
        public static string RepresentanteEntidad = "AtrRepesentante";

        /// <summary>
        /// Atributo para la propiedad representante de una entidad.
        /// </summary>
        public static string NombrePropiedadEntidad = "AtrNombre";

        /// <summary>
        /// Atributo para la propiedad representante de una entidad.
        /// </summary>
        public static string NombrePropiedadEntidadLectura = "AtrNombreLectura";

        /// <summary>
        /// Atributo para especificar que se debe incluir un botón para mostrar los paneles virgenes de las entidades de un grupo de paneles.
        /// </summary>
        public static string IncluirBotonAgregarNuevaInstancia = "AtrBotonAgregarNuevaInst";

        /// <summary>
        /// Vincula las entidades de un propiedad con alguna entidad hija para que aprezcan juntas en la tabla de entidades ya agregadas.
        /// </summary>
        public static string VincularEntidadesPropConEntHijas = "AtrVincularEntConEntHija";

        /// <summary>
        /// Vincula las entidades de un propiedad con alguna entidad hija para que aprezcan juntas en la tabla de entidades ya agregadas con
        /// con el modo de estilo denominado 2, en el cual se muestran los datos sin ningún tipo de cabecerá en dos niveles.
        /// </summary>
        public static string VincularEntidadesPropConEntHijas2 = "AtrVincularEntConEntHija2";

        /// <summary>
        /// Vincula las entidades de un propiedad con alguna entidad hija para que aprezcan juntas en la tabla de entidades ya agregadas.
        /// </summary>
        public static string SinRepresentanteEntidad = "SinAtrRepesentante";

        /// <summary>
        /// Texto para el botón editar entidad o valor propiedad.
        /// </summary>
        public static string TextoBotonAgregarElemento = "AtrTextoAgregarElem";

        /// <summary>
        /// Texto para el botón aceptar entidad o valor propiedad.
        /// </summary>
        public static string TextoBotonAceptarElemento = "AtrTextoAceptarElem";

        /// <summary>
        /// Texto para el botón cancelar entidad o valor propiedad.
        /// </summary>
        public static string TextoBotonCancelarElemento = "AtrTextoCancelarElem";

        /// <summary>
        /// Texto para el botón aceptar entidad o valor propiedad que a la vez tiene dentro una nuevo entidad vinculada para agregar.
        /// </summary>
        public static string TextoBotonAceptarElementoYAgregarElemVinculado = "AtrTextoAceptarElemYAgreElemVinc";

        /// <summary>
        /// Edita un elemento seleccionado de una tabla de entidades.
        /// </summary>
        public static string TextoBotonEditarElemSel = "AtrTextoEdicionEntSel";

        /// <summary>
        /// Agrega un nuevo elemento vinculado a el que está seleccionado de una tabla de entidades.
        /// </summary>
        public static string TextoBotonAgregarElemSel = "AtrTextoAgregarEntSel";

        /// <summary>
        /// Elimina un elemento seleccionado de una tabla de entidades.
        /// </summary>
        public static string TextoBotonEliminarElemSel = "AtrTextoEliminarEntSel";

        /// <summary>
        /// Texto para el link que edita una entidad que se despliega.
        /// </summary>
        public static string TextoLinkEditarDespliegue = "AtrTextoLinkEditarDespliegue";

        #endregion

        /// <summary>
        /// Atributo para especificar que el control de una propiedad es un textArea.
        /// </summary>
        public static string GnossTextBoxMultiText = "gnoss_textBox_MultiText";

        /// <summary>
        /// Atributo para especificar las columnas deel control de una propiedad es un textArea.
        /// </summary>
        public static string GnossAtrrColumnasTextBox = "gnoss_AtributoControl_cols";

        /// <summary>
        /// Atributo para especificar las filas del control de una propiedad es un textArea.
        /// </summary>
        public static string GnossAtrrFilasTextBox = "gnoss_AtributoControl_rows";

        /// <summary>
        /// Atributo para especificar el tipo especial del control de una propiedad es un textArea.
        /// </summary>
        public static string GnossAtrrTipoCampo = "gnoss_TipoCampo";

        /// <summary>
        /// Atributo para especificar que se agregue un salto de linea tras el control.
        /// </summary>
        public static string GnossAtrrSaltoLinea = "gnoss_SaltoLinea";

        /// <summary>
        /// Atributo para especificar el número de caracteres de una propiedad.
        /// </summary>
        public static string GnossAtrrNumCaract = "gnoss_NumeroCaracteres";

        /// <summary>
        /// Atributo para especificar que se agrege un div class="clear" tras el control.
        /// </summary>
        public static string GnossAtrrDivClassClear = "gnoss_DivClassClear";

        /// <summary>
        /// Atributo para especificar que el panel de la entidad debe ser desplegable.
        /// </summary>
        public static string GnossAtrrDivEntidadDesplegable = "gnoss_DivEntidadDesplegable";

        /// <summary>
        /// Atributo para especificar que los valores de la propiedad irán separados por comas.
        /// </summary>
        public static string GnossAtrrValoresSepComas = "gnoss_ValoresSepComas";

        /// <summary>
        /// Atributo para especificar que los valores de la propiedad irán separados por comas.
        /// </summary>
        public static string GnossAtrrCombo = "gnoss_ValoresCombo";

        /// <summary>
        /// Atributo para especificar que los valores de la propiedad irán separados por comas.
        /// </summary>
        public static string GnossCampoDeshabilitado = "gnoss_CampoDeshabilitado";

        /// <summary>
        /// Atributo para especificar que el campo correspondiente a una fecha se pintara de modo que pueda seleccionarse el mes y año.
        /// </summary>
        public static string GnossCampoFechaMesAnyo = "gnoss_FechaMesAño";

        /// <summary>
        /// Atributo para especificar que debe mostrase una vista previa del campo correspondiente en la edición.
        /// </summary>
        public static string GnossVistaPreEnEdicion = "gnoss_PrevEnEdicion";

        /// <summary>
        /// Atributo para especificar el estilo de la tabla de entidad ya agregadas a la propiedad de tipo object-No-Funcional.
        /// </summary>
        public static string GnossEstiloTabla = "gnoss_EstiloTabla";

        /// <summary>
        /// Atributo para especificar el estilo de las columnas de la tabla de entidad ya agregadas a la propiedad de tipo object-No-Funcional.
        /// </summary>
        public static string GnossEstiloColumna = "gnoss_EstiloColumna";

        /// <summary>
        /// Atributo para especificar el tag de un elemento html.
        /// </summary>
        public static string GnossTagName = "gnoss_TagName";

        /// <summary>
        /// Atributo para especificar que un campo usa autocompletar.
        /// </summary>
        public static string GnossAutoCompletar = "gnoss_AutoCompletar";

        /// <summary>
        /// Atributo para especificar que un campo debe tener una imagen por delante.
        /// </summary>
        public static string GnossImgSep = "gnoss_ImgSep";

        /// <summary>
        /// Atributo para especificar que un campo Data no funcional debe tener un solo nombre de propiedad y no dos.
        /// </summary>
        public static string GnossOcultarTituloInterno = "gnoss_OcultarTituloInterno";

        /// <summary>
        /// Atributo para especificar que la imagen de una propiedad debe mostrarse en pequeño en la vista previa.
        /// </summary>
        public static string GnossImgMiniEnVP = "gnoss_ImgMiniVP";

        /// <summary>
        /// Atributo para especificar que los valores de una propiedad deben aparecer en modo galeria.
        /// </summary>
        public static string GnossGaleriaElmentos = "gnoss_GaleriaElmentos";

        /// <summary>
        /// Indica que se deben usar checks para guardar valores.
        /// </summary>
        public static string GnossUsarChecks = "gnoss_UsarChecks";

        /// <summary>
        /// Indica que se deben usar el nombre de esa clase css para elemento.
        /// </summary>
        public static string GnossCss = "gnoss_Css";

        /// <summary>
        /// Indica si hay que ocultar el elemento.
        /// </summary>
        public static string GnossVisible = "gnoss_Visible";

        #region Atributos Css reales

        /// <summary>
        /// Altura máxima de un elemento.
        /// </summary>
        public static string CssAlturaMaxima = "max-height";

        /// <summary>
        /// Anchura máxima de un elemento.
        /// </summary>
        public static string CssAnchuraMaxima = "max-width";

        /// <summary>
        /// Altura de un elemento.
        /// </summary>
        public static string CssAltura = "height";

        /// <summary>
        /// Anchura de un elemento.
        /// </summary>
        public static string CssAnchura = "width";

        /// <summary>
        /// Margen izquierdo de un elemento.
        /// </summary>
        public static string CssMargenIzq = "margin-left";

        /// <summary>
        /// Color de fondo de un elemento.
        /// </summary>
        public static string CssColorFondo = "background-color";

        /// <summary>
        /// Color de fondo de un elemento.
        /// </summary>
        public static string CssBorde = "border";

        /// <summary>
        /// Letra negrita.
        /// </summary>
        public static string CssNegrita = "font-weight";

        #endregion

        /// <summary>
        /// Atributo para especificar el namespace de la ontología.
        /// </summary>
        public static string NamespaceOnto = "namespace";

        /// <summary>
        /// Atributo para especificar los idiomas de la ontología.
        /// </summary>
        public static string IdiomasOnto = "idiomasOnto";

        /// <summary>
        /// Atributo para especificar que el titulo de un recurso es el valor de una propiedad de la ontología.
        /// </summary>
        public static string TituloDoc = "tituloDoc";

        /// <summary>
        /// Atributo para especificar que la descripcion de un recurso es el valor de una propiedad de la ontología.
        /// </summary>
        public static string DescripcionDoc = "descripcionDoc";

        /// <summary>
        /// Atributo para especificar que la imagen de un recurso es el valor de una propiedad de la ontología.
        /// </summary>
        public static string ImagenDoc = "imagenDoc";

        /// <summary>
        /// Atributo para especificar que en la ficha del recurso deben ocultarse el título, descripción y imagen del recurso.
        /// </summary>
        public static string OcultarTituloDescpImgDoc = "ocultarTituloDescpImgDoc";
    }

    /// <summary>
    /// Atributos usados en archivo de configuración de los formularios semánticos
    /// </summary>
    public class ValorAtributosLectorCSS
    {
        /// <summary>
        /// Atributo para el campo especial video.
        /// </summary>
        public static string CampoEspecialVideo = "Video";

        /// <summary>
        /// Valor para el campo especial imagen.
        /// </summary>
        public static string CampoEspecialImagen = "Imagen";

        /// <summary>
        /// Valor para el campo especial archivo.
        /// </summary>
        public static string CampoEspecialArchivo = "Archivo";

        /// <summary>
        /// Valor para el campo especial label.
        /// </summary>
        public static string CampoLabel = "Label";

        /// <summary>
        /// Valor para el campo especial párrafo.
        /// </summary>
        public static string CampoParrafo = "Parrafo";

        /// <summary>
        /// Valor para el campo especial Tiny.
        /// </summary>
        public static string Tiny = "Tiny";

        /// <summary>
        /// Valor para el campo especial Tiny.
        /// </summary>
        public static string True = "true";

        /// <summary>
        /// Valor para el campo especial Tiny.
        /// </summary>
        public static string False = "false";

        /// <summary>
        /// Valor para el atributo CSS negrita.
        /// </summary>
        public static string Negrita = "bold";

        /// <summary>
        /// Valor para el atributo GnossAutocompetar.
        /// </summary>
        public static string NombreGrafo = "nombregrafo:";

        /// <summary>
        /// Valor para el tipo de resultado del GnossAutocompetar.
        /// </summary>
        public static string TipoResulGrafo = "tiporesulgrafo:";

        #region Parte Valor para Representantes entidad

        /// <summary>
        /// Valor para el numero de caracteres de un representante que especifica que debe mostrarse el nombre de la entidad + el ID.
        /// </summary>
        public static string codigoRepresentateEntidad_TipoEntidadMasID = "-1";

        /// <summary>
        /// Valor para el numero de caracteres de un representante que especifica que debe mostrarse solo el ID de la entidad.
        /// </summary>
        public static string codigoRepresentateEntidad_SoloID = "-2";

        /// <summary>
        /// Valor para el numero de caracteres de un representante que especifica que debe mostrarse todo el texto entero.
        /// </summary>
        public static string codigoRepresentateEntidad_TodosLosCaracteres = "-3";

        /// <summary>
        /// Color blanco Css.
        /// </summary>
        public static string Color_Blanco = "White";

        /// <summary>
        /// Sin borde Css.
        /// </summary>
        public static string Sin_Borde = "0px none";

        #endregion

        #region Restricción número caracteres

        /// <summary>
        /// Valor para la restricción de los caracteres: Menor.
        /// </summary>
        public static string RestriCaract_Menor = "<";

        /// <summary>
        /// Valor para la restricción de los caracteres: Menor.
        /// </summary>
        public static string RestriCaract_Mayor = ">";

        /// <summary>
        /// Valor para la restricción de los caracteres: Menor.
        /// </summary>
        public static string RestriCaract_Igual = "=";

        /// <summary>
        /// Valor para la restricción de los caracteres: Menor.
        /// </summary>
        public static string RestriCaract_Entre = "entre";

        #endregion
    }

    /// <summary>
    /// Contiene las variables que son constantes en el archivo de configuración de una ontología.
    /// </summary>
    public class ConstantesArchivoConfiguracion
    {
        /// <summary>
        /// Valor del inicio y del fin de la cabecera de un titulo.
        /// </summary>
        public static string InicioFinCabeceraTitulo = "/*******************************************************************************/";

        /// <summary>
        /// Tabulación que hay que aplicar a los atributos del fichero.
        /// </summary>
        public static string Tabulacion = "    ";

        /// <summary>
        /// Inserta el título formateado de una cabecera.
        /// </summary>
        /// <param name="pTitulo">Título de la cabecera</param>
        /// <returns>Título de la cabecera formateado</returns>
        public static string InsertarTituloCabecera(string pTitulo)
        {
            int caracteresMaximos = 132;
            int caracteresFinal = (caracteresMaximos - pTitulo.Length) / 2;
            int caracteresPrincipio = caracteresFinal;

            if (((caracteresFinal * 2) + pTitulo.Length) != caracteresMaximos)
            {
                caracteresPrincipio = caracteresFinal + 1;
            }

            string tituloCabecera = "/*";
            for (int i = 0; i < caracteresPrincipio; i++)
            {
                tituloCabecera += " ";
            }
            tituloCabecera += pTitulo;
            for (int i = 0; i < caracteresFinal; i++)
            {
                tituloCabecera += " ";
            }
            tituloCabecera += "*/";

            return tituloCabecera;
        }
    }

    /// <summary>
    /// Clase con los nombres para las clases CSS usadas en los formularios.
    /// </summary>
    public class ClaseCssOnto
    {
        /// <summary>
        /// Nombre para el div principal de edición.
        /// </summary>
        public static string FormEdicion = "formSemEdicion";

        /// <summary>
        /// Nombre para el div principal de lectura.
        /// </summary>
        public static string FormLectura = "formSemLectura";

        /// <summary>
        /// Nombre para los campos de edición.
        /// </summary>
        public static string CampoEdicion = "edit edit_";

        ///// <summary>
        ///// Nombre para los campos genéricos.
        ///// </summary>
        //public static string CampoEdicionGen = "edit";

        ///// <summary>
        ///// Nombre para los campos de lectura.
        ///// </summary>
        //public static string CampoLectura = "read_";

        /// <summary>
        /// Nombre para los labels de edición.
        /// </summary>
        public static string LabelEdicion = "lb lb_";

        ///// <summary>
        ///// Nombre para los labels genéricos.
        ///// </summary>
        //public static string LabelEdicionGen = "lb";

        ///// <summary>
        ///// Nombre para los labels de lectura.
        ///// </summary>
        //public static string LabelLectura = "lbread_";

        /// <summary>
        /// Nombre para los contenedores de los controles.
        /// </summary>
        public static string Contenedor = "cont cont_";

        /// <summary>
        /// Nombre para los contenedores de entiades agregadas.
        /// </summary>
        public static string ContenedorEntidadesAgre = "contAgr";

        /// <summary>
        /// Nombre para los contenedores de entiades.
        /// </summary>
        public static string ContenedorEntidades = "contEnt contEnt_";

        /// <summary>
        /// Nombre para los td títulos.
        /// </summary>
        public static string TdTitulo = "tdtit";

        /// <summary>
        /// Nombre para los td valores.
        /// </summary>
        public static string TdValor = "tdval";

        /// <summary>
        /// Nombre para los td acciones.
        /// </summary>
        public static string TdAccion = "tdaccion";

        /// <summary>
        /// Nombre para los td acciones.
        /// </summary>
        public static string Parrafo = "fila";
    }
}
