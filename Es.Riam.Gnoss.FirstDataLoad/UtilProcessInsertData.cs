using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.Pais;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.FirstDataLoad
{
    public class UtilProcessInsertData
    {
        /// <summary>
        /// Devuelve una lista del Objeto que se le introduzca y rellena los objetos con los valores insert del documento .sql introducido por parametro. Si el objeto no concuerda con el archivo entoces devolvera null.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto del que se creara la lista. O Pais, o Provincia, o FacetaObjetoConocimiento</typeparam>
        /// <param name="contenidoArchivo">Ruta del archivo .sql</param>
        /// <returns>Retorna la lista del objeto parametrizado a no ser que no coincida con el del archivo o no sea uno de los objetos permitidos.</returns>
        public dynamic ProcesarInsert<T>(string contenidoArchivo)
        {

            string  objeto = "";
            List<string[]> valores = new List<string[]>();
            int contador = 0;

            foreach (string linea in contenidoArchivo.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!linea.Equals("") && !linea.StartsWith("print"))
                {
                    if (contador == 0)
                    {
                        objeto = ObtenerTipo(linea);
                        contador++;
                    }
                    valores.Add(ObtenerValores(linea));
                }
            }

            if (typeof(T) == typeof(FacetaObjetoConocimiento) && objeto.Equals("FACETAOBJETOCONOCIMIENTO"))
            {
                List<FacetaObjetoConocimiento> listaFaceta = new List<FacetaObjetoConocimiento>();
                foreach (string[] iteracion in valores)
                {
                    listaFaceta.Add(crearFacetaObjetoConocimiento(iteracion));
                }
                return listaFaceta;
            }
            if (typeof(T) == typeof(Pais) && objeto.Equals("PAIS"))
            {
                List<Pais> listaPais = new List<Pais>();
                foreach (string[] iteracion in valores)
                {
                    listaPais.Add(crearPais(iteracion));
                }
                return listaPais;
            }
            if (typeof(T) == typeof(Provincia) && objeto.Equals("PROVINCIA"))
            {
                List<Provincia> listaProvincia = new List<Provincia>();
                foreach (string[] iteracion in valores)
                {
                    listaProvincia.Add(crearProvincia(iteracion));
                }
                return listaProvincia;
            }
            return null;
            

        }
        private FacetaObjetoConocimiento crearFacetaObjetoConocimiento(string[] parametros)
        {
            return new FacetaObjetoConocimiento() { ObjetoConocimiento = parametros[0], Faceta = parametros[1], Nombre = parametros[2], Orden = Int16.Parse(parametros[3]), Autocompletar = parametros[4].Equals("1"), TipoPropiedad = Int16.Parse(parametros[5]), TipoDisenio = Int16.Parse(parametros[6]), ElementosVisibles = Int16.Parse(parametros[7]), AlgoritmoTransformacion = Int16.Parse(parametros[8]), EsSemantica = parametros[9].Equals("1"), Mayusculas = Int16.Parse(parametros[10]), EsPorDefecto = parametros[11].Equals("1"), NombreFaceta = parametros[12], ComportamientoOr = parametros[13].Equals("1") };
        }
        private Pais crearPais(string[] parametros)
        {
            return new Pais() { PaisID = new Guid(parametros[0]), Nombre = parametros[1] };
        }
        private Provincia crearProvincia(string[] parametros)
        {
            return new Provincia() { ProvinciaID = new Guid(parametros[0]), PaisID = new Guid(parametros[1]), Nombre = parametros[2], CP = parametros[3] };
        }
        private string ObtenerTipo(string linea)
        {
            string objeto;
            objeto = linea.Substring(linea.IndexOf("[dbo].") + 6).Trim();
            objeto = objeto.Split(' ')[0];
            objeto = objeto.StartsWith("[") ? objeto.Substring(1, objeto.Length - 1) : objeto;
            objeto = objeto.EndsWith("]") ? objeto.Substring(0, objeto.Length - 1) : objeto;
            objeto = objeto.ToUpper();
            return objeto;
        }
        static string[] ObtenerValores(string linea)
        {
            linea = linea.Substring(linea.IndexOf("VALUES") + 6).Trim();
            linea = linea.StartsWith("(") ? linea.Substring(1, linea.Length - 1) : linea;
            linea = linea.EndsWith(")") ? linea.Substring(0, linea.Length - 1) : linea;
            string[] valores = linea.Split(',');
            for (int i = 0; i < valores.Length; i++)
            {
                valores[i] = valores[i].Trim();
                valores[i] = valores[i].StartsWith("N'") ? valores[i].Substring(2, valores[i].Length - 2) : valores[i];
                valores[i] = valores[i].EndsWith("'") ? valores[i].Substring(0, valores[i].Length - 1) : valores[i];
            }
            return valores;
        }
    }
}
