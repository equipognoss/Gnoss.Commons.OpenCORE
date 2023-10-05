using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Util;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Microsoft.AspNetCore.Http;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.AbstractsOpen;
using ClosedXML.Excel;
using Es.Riam.Semantica.OWL;

namespace Es.Riam.Gnoss.Traducciones
{
    public class BaseDeDatos
    {
        private const int LIMITE_VIRTUOSO = 10000;
        private const int LIMITE_CELDA_EXCEL = 2000;

        private const string ontologiaPrincipal = "tabla_";
        private readonly Elementos.ServiciosGenerales.Proyecto mProyectoSeleccionado;

        private EntityContext mEntityContext;
        private VirtuosoAD mVirtuosoAD;
        private ConfigService mConfigService;
        private LoggingService mLoggingService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private IHttpContextAccessor mHttpContextAccessor;
        private GnossCache mGnossCache;
        private EntityContextBASE mEntityContextBASE;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        public BaseDeDatos(Elementos.ServiciosGenerales.Proyecto pProyectoSeleccionado, EntityContext entityContext, LoggingService loggingService, ConfigService configService, VirtuosoAD virtuosoAD, RedisCacheWrapper redisCacheWrapper, IHttpContextAccessor httpContextAccessor, GnossCache gnossCache, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mGnossCache = gnossCache;
            mEntityContextBASE = entityContextBASE;
            mHttpContextAccessor = httpContextAccessor;
            mEntityContext = entityContext;
            mRedisCacheWrapper = redisCacheWrapper;
            mLoggingService = loggingService;
            mConfigService = configService;
            mVirtuosoAD = virtuosoAD;
            mProyectoSeleccionado = pProyectoSeleccionado;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Escribe el excel de la tabla 
        /// </summary>
        private void EscribirExcelPorTabla(Dictionary<string, Dictionary<string, string>> mDiccionario, string clave, Dictionary<string, string> idiomasTextos)
        {
            foreach (string idioma in idiomasTextos.Keys)
            {
                string texto = idiomasTextos[idioma];

                if (texto.Length <= LIMITE_CELDA_EXCEL)
                {
                    if (!mDiccionario.ContainsKey(clave))
                    {
                        mDiccionario.Add(clave, new Dictionary<string, string>());
                    }
                    mDiccionario[clave].Add(idioma, texto);
                }
            }
        }



        /// <summary>
        /// Escribe excel pasado por parámetros de la tabla TextosPersonalizadosPersonalización
        /// </summary>
        /// <param name="mPersonalizacionID"></param>
        /// <param name="mExcel"></param>
        public void TextosPersonalizadosToExcel(Guid mPersonalizacionID, XLWorkbook mExcel)
        {
            string nombreHoja = ontologiaPrincipal + "TextosPersonalizados";
            Dictionary<string, Dictionary<string, string>> mDiccionario = new Dictionary<string, Dictionary<string, string>>();
            List<TextosPersonalizadosPersonalizacion> filas = mEntityContext.TextosPersonalizadosPersonalizacion.Where(item => item.PersonalizacionID.Equals(mPersonalizacionID)).ToList();

            if (filas.Count > 0)
            {
                foreach (TextosPersonalizadosPersonalizacion filaTraduccion in filas)
                {
                    if (filaTraduccion.Texto.Length <= LIMITE_CELDA_EXCEL)
                    {
                        if (!mDiccionario.ContainsKey(filaTraduccion.TextoID))
                        {
                            mDiccionario.Add(filaTraduccion.TextoID, new Dictionary<string, string>());
                        }
                        mDiccionario[filaTraduccion.TextoID].Add(filaTraduccion.Language, filaTraduccion.Texto);
                    }
                }

                UtilFicheros.ConstruirExcel(mExcel, nombreHoja, mDiccionario);
            }
        }

        //HACER no hay archivos que descargar | comprueba
        /// <summary>
        /// Actualiza la tabla de SQL Textos personalizados pasándole un excel
        /// </summary>
        /// <param name="mPersonalizacionID"></param>
        /// <param name="tabla"></param>
        public void ExcelToTextosPersonalizados(Guid mPersonalizacionID, DataTable tabla)
        {
            var filas = mEntityContext.TextosPersonalizadosPersonalizacion.Where(item => item.PersonalizacionID.Equals(mPersonalizacionID)).ToList();

            foreach (DataRow fila in tabla.Rows)
            {
                string claveFila = fila[0].ToString();
                //string valorColumna = "";

                //var filaPersonalizada = filas.FirstOrDefault(componente => componente.TextoID.Equals(claveFila));
                //if (filaPersonalizada != null)
                //{
                for (int i = 1; i < tabla.Columns.Count; i++)
                {
                    if (!fila.IsNull(i) && !string.IsNullOrEmpty((string)fila[i])/* && !valorColumna.Equals(fila[i])*/)
                    {
                        string idioma = tabla.Columns[i].ColumnName;
                        var filaPersonalizada = filas.FirstOrDefault(componente => componente.TextoID.Equals(claveFila) && componente.Language.Equals(idioma));

                        if (filaPersonalizada == null)
                        {
                            filaPersonalizada = new TextosPersonalizadosPersonalizacion() { PersonalizacionID = mPersonalizacionID, TextoID = claveFila, Language = idioma };
                            mEntityContext.TextosPersonalizadosPersonalizacion.Add(filaPersonalizada);
                        }
                        filaPersonalizada.Texto = fila[i] as string;
                    }
                }

                //}
            }
            mEntityContext.SaveChanges();
        }



        /// <summary>
        /// Escribe excel pasado por parámetros de la tabla ProyectoPestanyaMenu
        /// </summary>
        /// <param name="mProyectoID"></param>
        /// <param name="mExcel"></param>
        public void ProyectoPestanyaMenuToExcel(Guid mProyectoID, XLWorkbook mExcel)
        {
            Dictionary<string, Dictionary<string, string>> mDiccionario = new Dictionary<string, Dictionary<string, string>>();
            string nombreHoja = ontologiaPrincipal + "TextosPestañas";
            List<ProyectoPestanyaMenu> filas = mEntityContext.ProyectoPestanyaMenu.Where(item => item.ProyectoID.Equals(mProyectoID)).ToList();

            if (filas.Count > 0)
            {
                foreach (ProyectoPestanyaMenu fila in filas)
                {
                    if (!string.IsNullOrEmpty(fila.Nombre))
                    {
                        Dictionary<string, string> idiomasTextos = UtilCadenas.ObtenerTextoPorIdiomas(fila.Nombre);

                        if (!idiomasTextos.ContainsKey("es"))
                        {
                            idiomasTextos.Add("es", UtilCadenas.ObtenerTextoDeIdioma(fila.Nombre, "es", "es"));
                        }
                        string clave = fila.PestanyaID.ToString();

                        EscribirExcelPorTabla(mDiccionario, clave, idiomasTextos);
                    }
                }
            }
            UtilFicheros.ConstruirExcel(mExcel, nombreHoja, mDiccionario);
        }

        //HACER
        public void ExcelToProyectoPestanyaMenu(Guid mProyectoID, DataTable tabla)
        {
            List<ProyectoPestanyaMenu> filas = mEntityContext.ProyectoPestanyaMenu.Where(item => item.ProyectoID.Equals(mProyectoID)).ToList();

            foreach (DataRow fila in tabla.Rows)
            {
                string claveFila = fila[0].ToString();
                Guid pestanyaID;
                string valorCompuesto = "";

                if (Guid.TryParse(claveFila, out pestanyaID))
                {
                    var filaPestanya = filas.FirstOrDefault(pestanya => pestanya.PestanyaID.Equals(pestanyaID));
                    if (filaPestanya != null)
                    {
                        //----------------------------------PRUEBA-----------------------------------
                        //idiomas y textos de sql, tienes que comprobar que sean iguales que los textos en idiomas de tu tabla excel
                        //Dictionary<string, string> textosIdiomasTabla = UtilCadenas.ObtenerTextoPorIdiomas(filaPestanya.Nombre);

                        //for (int i = 1; i < tabla.Columns.Count; i++)
                        //{
                        //    if (!string.IsNullOrEmpty(fila[i].ToString()))
                        //    {
                        //        //sacar texto de idioma de excel fila[i] texto idioma columna
                        //    }
                        //}




                        //---------------------------------------------------------------------------
                        string textoComparar = filaPestanya.Nombre;

                        for (int i = 1; i < tabla.Columns.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(fila[i].ToString()))
                            {
                                valorCompuesto += $"{fila[i]}@{tabla.Columns[i].ColumnName}|||";
                            }
                        }
                        if (valorCompuesto.Split('@').Length - 1 >= 2 && !valorCompuesto.Split('@')[1].Equals("es"))
                        {
                            valorCompuesto = valorCompuesto.TrimEnd('|');
                        }

                        if (!valorCompuesto.Equals(textoComparar) && !string.IsNullOrEmpty(valorCompuesto) && !string.IsNullOrEmpty(textoComparar))
                        {
                            filaPestanya.Nombre = valorCompuesto;
                        }
                    }
                }
            }
            mEntityContext.SaveChanges();
        }



        /// <summary>
        /// Escribe excel pasado por parámetros de la tabla CMSPropiedadComponente
        /// </summary>
        /// <param name="mProyectoID"></param>
        /// <param name="mExcel"></param>
        public void CmsPropiedadComponenteToExcel(Guid mProyectoID, XLWorkbook mExcel)
        {
            string nombreHoja = ontologiaPrincipal + "TextosComponentesCMS";
            Dictionary<string, Dictionary<string, string>> mDiccionario = new Dictionary<string, Dictionary<string, string>>();
            short[] listaTipos = { 0, 1, 16 };

            var filas = mEntityContext.CMSPropiedadComponente.Join(mEntityContext.CMSComponente, cmsPropiedadComponente => cmsPropiedadComponente.ComponenteID, cmsComponente => cmsComponente.ComponenteID, (cmsPropiedadComponente, cmsComponente) => new { CMSPropiedadComponente = cmsPropiedadComponente, CMSComponente = cmsComponente }).Where(item => item.CMSComponente.ProyectoID.Equals(mProyectoID) && listaTipos.Contains(item.CMSPropiedadComponente.TipoPropiedadComponente)).ToList();

            if (filas.Count > 0)
            {
                foreach (var fila in filas)
                {
                    string componenteID = fila.CMSPropiedadComponente.ComponenteID.ToString();
                    string tipoPropiedadComponente = fila.CMSPropiedadComponente.TipoPropiedadComponente.ToString();

                    if (!string.IsNullOrEmpty(componenteID) && !string.IsNullOrEmpty(tipoPropiedadComponente))
                    {
                        Dictionary<string, string> idiomasTextos = UtilCadenas.ObtenerTextoPorIdiomas(fila.CMSPropiedadComponente.ValorPropiedad);

                        if (!idiomasTextos.ContainsKey("es"))
                        {
                            idiomasTextos.Add("es", UtilCadenas.ObtenerTextoDeIdioma(fila.CMSPropiedadComponente.ValorPropiedad, "es", "es"));
                        }

                        string clave = $"{componenteID}_{tipoPropiedadComponente}";

                        EscribirExcelPorTabla(mDiccionario, clave, idiomasTextos);
                    }
                }
                UtilFicheros.ConstruirExcel(mExcel, nombreHoja, mDiccionario);
            }


        }

        //HACER
        public void ExcelToCmsPropiedadComponente(Guid mProyectoID, DataTable tabla)
        {
            short[] listaTipos = { 0, 1, 16 };

            var filas = mEntityContext.CMSPropiedadComponente.Join(mEntityContext.CMSComponente, cmsPropiedadComponente => cmsPropiedadComponente.ComponenteID, cmsComponente => cmsComponente.ComponenteID, (cmsPropiedadComponente, cmsComponente) => new { CMSPropiedadComponente = cmsPropiedadComponente, CMSComponente = cmsComponente }).Where(item => item.CMSComponente.ProyectoID.Equals(mProyectoID) && listaTipos.Contains(item.CMSPropiedadComponente.TipoPropiedadComponente)).ToList();

            foreach (DataRow fila in tabla.Rows)
            {
                string claveFila = fila[0].ToString();
                Guid ComponenteID;
                short TipoComponente;
                string valorCompuesto = "";

                if (Guid.TryParse(claveFila.Substring(0, claveFila.IndexOf('_')), out ComponenteID) && short.TryParse(claveFila.Substring(claveFila.IndexOf('_') + 1), out TipoComponente))
                {

                    var filaComponente = filas.FirstOrDefault(componente => componente.CMSComponente.ComponenteID.Equals(ComponenteID) && componente.CMSPropiedadComponente.TipoPropiedadComponente.Equals(TipoComponente));
                    if (filaComponente != null)
                    {
                        string separador = "";
                        for (int i = 1; i < tabla.Columns.Count; i++)
                        {
                            if (!fila.IsNull(i) && !string.IsNullOrEmpty((string)fila[i]))
                            {
                                valorCompuesto += $"{separador}{fila[i]}@{tabla.Columns[i].ColumnName}";
                                separador = "|||";
                            }
                        }
                        filaComponente.CMSPropiedadComponente.ValorPropiedad = valorCompuesto;
                    }
                }
            }
            mEntityContext.SaveChanges();
        }



        /// <summary>
        /// Escribe excel pasado por parámetros de la tabla FacetaObjetoConocimiento
        /// </summary>
        /// <param name="mProyectoID"></param>
        /// <param name="mExcel"></param>

        public void FacetaObjetoConocimientoProyectoToExcel(Guid mProyectoID, XLWorkbook mExcel)
        {
            string nombreHoja = ontologiaPrincipal + "TextosFacetas";
            Dictionary<string, Dictionary<string, string>> mDiccionario = new Dictionary<string, Dictionary<string, string>>();

            var filas = mEntityContext.FacetaObjetoConocimientoProyecto.Where(item => item.ProyectoID.Equals(mProyectoID)).ToList();

            if (filas.Count > 0)
            {
                foreach (var fila in filas)
                {
                    if (!string.IsNullOrEmpty(fila.Faceta) && !string.IsNullOrEmpty(fila.ObjetoConocimiento.ToString()))
                    {
                        Dictionary<string, string> idiomasTextos = UtilCadenas.ObtenerTextoPorIdiomas(fila.NombreFaceta);

                        if (!idiomasTextos.ContainsKey("es"))
                        {
                            idiomasTextos.Add("es", UtilCadenas.ObtenerTextoDeIdioma(fila.NombreFaceta, "es", "es"));
                        }

                        string clave = fila.ObjetoConocimiento.ToString() + "_" + fila.Faceta.ToString();

                        EscribirExcelPorTabla(mDiccionario, clave, idiomasTextos);
                    }
                }
            }
            UtilFicheros.ConstruirExcel(mExcel, nombreHoja, mDiccionario);
        }

        //HACER
        public void ExcelToFacetaObjetoConocimientoProyecto(Guid mProyectoID, DataTable tabla)
        {
            var filas = mEntityContext.FacetaObjetoConocimientoProyecto.Where(item => item.ProyectoID.Equals(mProyectoID)).ToList();

            foreach (DataRow fila in tabla.Rows)
            {
                string claveFila = fila[0].ToString();
                string objetoConocimiento = claveFila.Substring(0, claveFila.IndexOf('_'));
                string faceta = claveFila.Substring(claveFila.IndexOf('_') + 1);
                string valorCompuesto = "";

                var filaComponente = filas.FirstOrDefault(componente => componente.ObjetoConocimiento.Equals(objetoConocimiento) && componente.Faceta.Equals(faceta));
                if (filaComponente != null)
                {
                    string separador = "";
                    for (int i = 1; i < tabla.Columns.Count; i++)
                    {
                        if (!fila.IsNull(i) && !string.IsNullOrEmpty((string)fila[i]))
                        {
                            valorCompuesto += $"{separador}{fila[i]}@{tabla.Columns[i].ColumnName}";
                            separador = "|||";
                        }
                    }
                    filaComponente.NombreFaceta = valorCompuesto;
                }
                mEntityContext.SaveChanges();
            }
        }



        /// <summary>
        /// Escribe en el excel pasado por parámetros de la tabla CategoriaTesauro
        /// </summary>
        /// <param name="mProyectoID"></param>
        /// <param name="mExcel"></param>
        public void CategoriaTesauroToExcel(Guid mProyectoID, XLWorkbook mExcel)
        {
            string nombreHoja = ontologiaPrincipal + "CategoriasTesauro";
            Dictionary<string, Dictionary<string, string>> mDiccionario = new Dictionary<string, Dictionary<string, string>>();

            var filas = mEntityContext.CategoriaTesauro.Join(mEntityContext.TesauroProyecto, categoriaTesauro => categoriaTesauro.TesauroID, tesauroProyecto => tesauroProyecto.TesauroID, (categoriaTesauro, tesauroProyecto) => new { CategoriaTesauro = categoriaTesauro, TesauroProyecto = tesauroProyecto }).Where(item => item.TesauroProyecto.ProyectoID.Equals(mProyectoID)).ToList();

            if (filas.Count > 0)
            {
                foreach (var fila in filas)
                {
                    if (!string.IsNullOrEmpty(fila.CategoriaTesauro.Nombre))
                    {
                        Dictionary<string, string> idiomasTextos = UtilCadenas.ObtenerTextoPorIdiomas(fila.CategoriaTesauro.Nombre);

                        if (!idiomasTextos.ContainsKey("es"))
                        {
                            idiomasTextos.Add("es", UtilCadenas.ObtenerTextoDeIdioma(fila.CategoriaTesauro.Nombre, "es", "es"));
                        }

                        string clave = fila.CategoriaTesauro.CategoriaTesauroID.ToString();

                        EscribirExcelPorTabla(mDiccionario, clave, idiomasTextos);
                    }
                }
            }

            UtilFicheros.ConstruirExcel(mExcel, nombreHoja, mDiccionario);
        }

        //HACER
        public void ExcelToCategoriaTesauro(Guid mProyectoID, DataTable tabla)
        {
            var filas = mEntityContext.CategoriaTesauro.Join(mEntityContext.TesauroProyecto, categoriaTesauro => categoriaTesauro.TesauroID, tesauroProyecto => tesauroProyecto.TesauroID, (categoriaTesauro, tesauroProyecto) => new { CategoriaTesauro = categoriaTesauro, TesauroProyecto = tesauroProyecto }).Where(item => item.TesauroProyecto.ProyectoID.Equals(mProyectoID)).ToList();

            foreach (DataRow fila in tabla.Rows)
            {
                string claveFila = fila[0].ToString();
                Guid CategoriaTesauroID;
                string valorCompuesto = "";

                if (Guid.TryParse(claveFila, out CategoriaTesauroID))
                {
                    var filaComponente = filas.FirstOrDefault(componente => componente.CategoriaTesauro.CategoriaTesauroID.Equals(CategoriaTesauroID));
                    if (filaComponente != null)
                    {
                        string separador = "";
                        for (int i = 1; i < tabla.Columns.Count; i++)
                        {
                            if (!fila.IsNull(i) && !string.IsNullOrEmpty((string)fila[i]))
                            {
                                valorCompuesto += $"{separador}{fila[i]}@{tabla.Columns[i].ColumnName}";
                                separador = "|||";
                            }
                        }
                        filaComponente.CategoriaTesauro.Nombre = valorCompuesto;
                    }
                }
                mEntityContext.SaveChanges();
            }
        }



        /// <summary>
        /// Escribe en el excel pasado por parámetros de la tabla ClausulaRegistro
        /// </summary>
        /// <param name="mProyectoID"></param>
        /// <param name="mExcel"></param>
        public void ClausulaRegistroToExcel(Guid mProyectoID, XLWorkbook mExcel)
        {
            string nombreHoja = ontologiaPrincipal + "ClausulasRegistro";
            Dictionary<string, Dictionary<string, string>> mDiccionario = new Dictionary<string, Dictionary<string, string>>();

            var filas = mEntityContext.ClausulaRegistro.Where(item => item.ProyectoID.Equals(mProyectoID)).ToList();

            if (filas.Count > 0)
            {
                foreach (var fila in filas)
                {
                    if (!string.IsNullOrEmpty(fila.Texto))
                    {
                        Dictionary<string, string> idiomasTextos = UtilCadenas.ObtenerTextoPorIdiomas(fila.Texto);

                        if (!idiomasTextos.ContainsKey("es"))
                        {
                            idiomasTextos.Add("es", UtilCadenas.ObtenerTextoDeIdioma(fila.Texto, "es", "es"));
                        }

                        string clave = fila.ClausulaID.ToString();

                        EscribirExcelPorTabla(mDiccionario, clave, idiomasTextos);
                    }
                }
            }
            UtilFicheros.ConstruirExcel(mExcel, nombreHoja, mDiccionario);
        }

        public void ExcelToClausulaRegistro(Guid mProyectoID, DataTable tabla)
        {
            var filas = mEntityContext.ClausulaRegistro.Where(item => item.ProyectoID.Equals(mProyectoID)).ToList();

            foreach (DataRow fila in tabla.Rows)
            {
                string claveFila = fila[0].ToString();
                Guid ClausulaID;
                string valorCompuesto = "";

                if (Guid.TryParse(claveFila, out ClausulaID))
                {
                    var filaClausula = filas.FirstOrDefault(componente => componente.ClausulaID.Equals(ClausulaID));
                    if (filaClausula != null)
                    {
                        string separador = "";
                        for (int i = 1; i < tabla.Columns.Count; i++)
                        {
                            if (!fila.IsNull(i) && !string.IsNullOrEmpty((string)fila[i]))
                            {
                                valorCompuesto += $"{separador}{fila[i]}@{tabla.Columns[i].ColumnName}";
                                separador = "|||";
                            }
                        }
                        filaClausula.Texto = valorCompuesto;
                    }
                }
                mEntityContext.SaveChanges();
            }
        }



        /// <summary>
        /// Escribe en el excel pasado por parámetros de la tabla ProyectoGadget
        /// </summary>
        /// <param name="mProyectoID"></param>
        /// <param name="mExcel"></param>
        public void ProyectoGadgetToExcel(Guid mProyectoID, XLWorkbook mExcel)
        {
            string nombreHoja = ontologiaPrincipal + "ComponentesRecursos";
            Dictionary<string, Dictionary<string, string>> mDiccionario = new Dictionary<string, Dictionary<string, string>>();

            var filas = mEntityContext.ProyectoGadget.Where(item => item.ProyectoID.Equals(mProyectoID)).ToList();

            if (filas.Count > 0)
            {
                foreach (var fila in filas)
                {
                    if (!string.IsNullOrEmpty(fila.Titulo))
                    {
                        Dictionary<string, string> idiomasTextos = UtilCadenas.ObtenerTextoPorIdiomas(fila.Titulo);

                        if (!idiomasTextos.ContainsKey("es"))
                        {
                            idiomasTextos.Add("es", UtilCadenas.ObtenerTextoDeIdioma(fila.Titulo, "es", "es"));
                        }

                        string clave = fila.GadgetID.ToString();

                        EscribirExcelPorTabla(mDiccionario, clave, idiomasTextos);
                    }
                }
            }
            UtilFicheros.ConstruirExcel(mExcel, nombreHoja, mDiccionario);
        }

        public void ExcelToProyectoGadget(Guid mProyectoID, DataTable tabla)
        {
            var filas = mEntityContext.ProyectoGadget.Where(item => item.ProyectoID.Equals(mProyectoID)).ToList();

            foreach (DataRow fila in tabla.Rows)
            {
                string claveFila = fila[0].ToString();
                Guid GadgetID;
                string valorCompuesto = "";

                if (Guid.TryParse(claveFila, out GadgetID))
                {
                    var filaGadget = filas.FirstOrDefault(componente => componente.GadgetID.Equals(GadgetID));
                    if (filaGadget != null)
                    {
                        string separador = "";
                        for (int i = 1; i < tabla.Columns.Count; i++)
                        {
                            if (!fila.IsNull(i) && !string.IsNullOrEmpty((string)fila[i]))
                            {
                                valorCompuesto += $"{separador}{fila[i]}@{tabla.Columns[i].ColumnName}";
                                separador = "|||";
                            }
                        }

                        if (!filaGadget.Titulo.Equals(valorCompuesto))
                        {
                            filaGadget.Titulo = valorCompuesto;
                        }
                    }
                }
                mEntityContext.SaveChanges();
            }
        }



        public void OntologiaProyectoToExcelSql(Guid mProyectoID, XLWorkbook mExcel)
        {
            string nombreHoja = ontologiaPrincipal + "ConfiguracionOntologias";
            Dictionary<string, Dictionary<string, string>> mDiccionario = new Dictionary<string, Dictionary<string, string>>();

            var filas = mEntityContext.OntologiaProyecto.Where(item => item.ProyectoID.Equals(mProyectoID)).ToList();

            if (filas.Count > 0)
            {
                foreach (var fila in filas)
                {
                    if (!string.IsNullOrEmpty(fila.NombreOnt))
                    {
                        Dictionary<string, string> idiomasTextos = UtilCadenas.ObtenerTextoPorIdiomas(fila.NombreOnt);

                        if (!idiomasTextos.ContainsKey("es"))
                        {
                            idiomasTextos.Add("es", UtilCadenas.ObtenerTextoDeIdioma(fila.NombreOnt, "es", "es"));
                        }

                        string clave = fila.OntologiaProyecto1.ToString();

                        EscribirExcelPorTabla(mDiccionario, clave, idiomasTextos);
                    }
                }
            }

            UtilFicheros.ConstruirExcel(mExcel, nombreHoja, mDiccionario);
        }

        public void ExcelToOntologiaProyectoSql(Guid mProyectoID, DataTable tabla)
        {
            var filas = mEntityContext.OntologiaProyecto.Where(item => item.ProyectoID.Equals(mProyectoID)).ToList();

            foreach (DataRow fila in tabla.Rows)
            {
                string claveFila = fila[0].ToString();
                string OntologiaProyecto1 = claveFila;
                string valorCompuesto = "";

                var filaGadget = filas.FirstOrDefault(componente => componente.OntologiaProyecto1.Equals(OntologiaProyecto1));
                if (filaGadget != null)
                {
                    string separador = "";
                    for (int i = 1; i < tabla.Columns.Count; i++)
                    {
                        if (!fila.IsNull(i) && !string.IsNullOrEmpty((string)fila[i]))
                        {
                            valorCompuesto += $"{separador}{fila[i]}@{tabla.Columns[i].ColumnName}";
                            separador = "|||";
                        }
                    }
                    filaGadget.NombreOnt = valorCompuesto;
                }
                mEntityContext.SaveChanges();
            }
        }



        public void OntologiaProyectoToExcelVirtuosoPorFecha(Guid mProyectoID, string grafo, DateTime? fechaMax, DateTime? fechaMin, XLWorkbook mExcel, string UrlIntragnoss)
        {
            string nombreHoja = grafo.Substring(0, grafo.IndexOf(".owl"));

            if (fechaMax == null || fechaMax.Equals(DateTime.MinValue))
            {
                fechaMax = DateTime.MaxValue;
            }

            if (fechaMin == null || fechaMax.Equals(DateTime.MinValue))
            {
                fechaMin = DateTime.MinValue;
            }

            DataSet dataSet = CrearExcelOntologiasVirtuosoPorFecha(grafo, mProyectoID, fechaMax, fechaMin, UrlIntragnoss);

            if (dataSet.Tables.Contains(grafo))
            {
                DataTable dtOtntologia = dataSet.Tables[grafo];
                DataTable decodedDataTable = decodeDataTable(dtOtntologia);
				//ExcelWorksheet ws = mExcel.Workbook.Worksheets.Add($"PrimaryOnto_{nombreHoja}");
				//ws.Cells.LoadFromDataTable(dtOtntologia, true);

				mExcel.Worksheets.Add(decodedDataTable);
                
            }
            dataSet.Dispose();
        }

        public DataTable decodeDataTable(DataTable dTable)
        {
			foreach (DataRow drow in dTable.Rows)
			{
				for (int i = 0; i < drow.ItemArray.Length; i++)
					if (drow[i].GetType() == typeof(string))
						drow[i] = System.Web.HttpUtility.HtmlDecode(drow[i].ToString());
			}
			dTable.AcceptChanges();
			return dTable;
		}
        private string ObtenerConsultasVirtuoso(string pOntologia, FacetadoDS dataSet, string urlIntragnoss)
        {
            string grafo = urlIntragnoss + pOntologia;

            if (!dataSet.Tables.Contains("Idiomas"))
            {
                string consultaIdiomas = $"select distinct lang(?o) as ?lang from <{grafo}> where {{ ?s ?p ?o. filter(lang(?o) != '') }} order by ?s ?p ?o lang(?o) ";

                using (FacetadoAD facetado = new FacetadoAD(urlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication))
                {
                    facetado.LeerDeVirtuoso(consultaIdiomas, "Idiomas", dataSet, pOntologia);
                }
            }

            StringBuilder sbIdiomas = new StringBuilder();
            StringBuilder sbIdiomasMin = new StringBuilder();
            StringBuilder sbWhere = new StringBuilder();
            sbWhere.Append($" from <{grafo}> where {{  ?s a ?type. optional {{ ?s ?p ?es. filter (lang(?es) = 'es'). }}");

            DataTable dtIdiomas = dataSet.Tables["Idiomas"];
            foreach (DataRow fila in dtIdiomas.Rows)
            {
                string idioma = (string)fila[0];
                idioma = idioma.Trim('-');

                sbIdiomasMin.Append($" min(?{idioma})");
                sbIdiomas.Append($" ?{idioma}");
                sbWhere.Append($"optional {{ ?s ?p ?es. filter (lang(?es) = 'es'). ?s ?p ?{idioma}aux. filter (lang(?{idioma}aux) = '{idioma}'). BIND (IF(str(?es) = str(?{idioma}aux), \"\", ?{idioma}aux) AS ?{idioma}) }}");

            }
            sbWhere.Append($" filter (!isBlank(?p)) ?id <http://gnoss/hasEntidad> ?s }} order by ?s ?p ?url {sbIdiomas.ToString()} }} group by ?s ?p ?url ");
            string consultaFinal = $"select ?s ?p (?s AS ?url) {sbIdiomasMin.ToString()}  where{{ select ?s ?p (?s AS ?url) {sbIdiomas.ToString()} {sbWhere.ToString()}";

            return consultaFinal;
        }

        private string ObtenerConsultasVirtuosoPorFecha(string pOntologia, FacetadoDS dataSet, List<Guid> documentosID, string urlIntragnoss, bool eliminarTraduccionesIgualEspañol = true)
        {
            string grafo = urlIntragnoss + pOntologia;

            if (!dataSet.Tables.Contains("Idiomas") && documentosID.Any())
            {
                string consultaIdiomas = $"select distinct lang(?o) as ?lang from <{grafo}> where {{  ?s ?p ?o. }} order by ?s ?p ?o lang(?o) ";

                using (FacetadoAD facetado = new FacetadoAD(urlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication))
                {
                    facetado.LeerDeVirtuoso(consultaIdiomas, "Idiomas", dataSet, pOntologia);
                }
            }

            string filterDocumentos = "";
            StringBuilder documentos = new StringBuilder();

            foreach (Guid documentoID in documentosID)
            {
                documentos.Append($"<{urlIntragnoss}{documentoID.ToString().ToLower()}>,");
            }
            filterDocumentos = $"filter (?id in ({documentos.ToString().Substring(0, documentos.Length - 1)}))";


            StringBuilder sbIdiomas = new StringBuilder();
            StringBuilder sbIdiomasMin = new StringBuilder();
            StringBuilder sbWhere = new StringBuilder();
            StringBuilder sbFilterSize = new StringBuilder();
            sbWhere.Append($" from <{grafo}> where {{  ?s a ?type. optional {{ ?s ?p ?es. filter (lang(?es) = 'es'). }}");

            DataTable dtIdiomas = dataSet.Tables["Idiomas"];
            foreach (DataRow fila in dtIdiomas.Rows)
            {
                if (!fila.IsNull(0) && !string.IsNullOrEmpty(fila[0].ToString()))
                {
                    string idioma = (string)fila[0];
                    idioma = idioma.Trim('-');

                    sbIdiomasMin.Append($" min(?{idioma}) as ?{idioma}");
                    sbIdiomas.Append($" ?{idioma}");

                    if (eliminarTraduccionesIgualEspañol)
                    {
                        sbWhere.Append($"optional {{ ?s ?p ?es. filter (lang(?es) = 'es'). ?s ?p ?{idioma}aux. filter (lang(?{idioma}aux) = '{idioma}'). BIND (IF(str(?es) = str(?{idioma}aux), \"\", ?{idioma}aux) AS ?{idioma}) }}");
                    }
                    else
                    {
                        sbWhere.Append($"optional {{ ?s ?p ?{idioma}. filter (lang(?{idioma}) = '{idioma}'). }}");
                    }


                }
            }
            sbFilterSize.Append($" AND strlen(str(?es)) < 32767 ");

            sbWhere.Append($" filter (!isBlank(?p)  {sbFilterSize.ToString()} )");
            sbWhere.Append($" ?id <http://gnoss/hasEntidad> ?s . {filterDocumentos}}} order by ?s ?p ?url {sbIdiomas.ToString()} }} group by ?s ?p ?url ");
            string consultaFinal = $"select ?s ?p (?s AS ?url) {sbIdiomasMin.ToString()}  where{{ select ?s ?p (?s AS ?url) {sbIdiomas.ToString()} {sbWhere.ToString()}";

            return consultaFinal;
        }

        private int ObtenerNumeroTriplesOntologiaDeVirtuoso(string pOntologia, string urlIntragnoss)
        {
            string grafo = urlIntragnoss + pOntologia;

            string consultaCount = $"select count(*) as ?contador from <{grafo}> where {{ ?s a ?type. ?s ?p ?o. filter(lang(?o) != '')}} ";

            FacetadoDS dataSet = new FacetadoDS();

            using (FacetadoAD facetado = new FacetadoAD(urlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication))
            {
                facetado.LeerDeVirtuoso(consultaCount, "NumeroResultados", dataSet, pOntologia);
            }

            int numero = 0;
            if (dataSet.Tables["NumeroResultados"].Rows.Count > 0)
            {
                int.TryParse((string)dataSet.Tables["NumeroResultados"].Rows[0]["contador"], out numero);
            }

            dataSet.Dispose();

            return numero;
        }

        private int ObtenerNumeroTriplesOntologiaDeVirtuosoPorFecha(string pOntologia, List<Guid> documentosID, string urlIntragnoss)
        {
            int numero = 0;
            string from = urlIntragnoss + pOntologia;

            if (documentosID.Any())
            {
                StringBuilder documentos = new StringBuilder();

                foreach (Guid documentoID in documentosID)
                {
                    documentos.Append($"<{urlIntragnoss}{documentoID.ToString().ToLower()}>,");
                }
                string filterDocumentos = $"filter (?id in ({documentos.ToString().Substring(0, documentos.Length - 1)}))";

                string grafo = pOntologia;

                string consultaCount = $"select count(*) as ?contador from <{from}> where {{ ?id <http://gnoss/hasEntidad> ?s . ?s ?p ?o. filter(lang(?o) != '') {filterDocumentos} }} ";

                FacetadoDS dataSet = new FacetadoDS();

                using (FacetadoAD facetado = new FacetadoAD(urlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication))
                {
                    facetado.LeerDeVirtuoso(consultaCount, "NumeroResultados", dataSet, grafo);
                }

                if (dataSet.Tables["NumeroResultados"].Rows.Count > 0)
                {
                    int.TryParse((string)dataSet.Tables["NumeroResultados"].Rows[0]["contador"], out numero);
                }

                dataSet.Dispose();
            }

            return numero;
        }

        //help
        public void ExcelToVirtuoso(DataTable tabla, string urlIntragnoss, Dictionary<string, MemoryStream> listMStream, Guid mProyectoID)
        {
            string ontologia = tabla.TableName.Substring(tabla.TableName.IndexOf("_") + 1).Trim('.').Trim() + ".owl";
            string grafo = urlIntragnoss + ontologia;

            int numRecursosRecorrer = 100;
            bool recorrerLista = true;
            int i = 0;


            Dictionary<Guid, List<Triple>> triples = new Dictionary<Guid, List<Triple>>();

            while (recorrerLista)
            {
                //List<Documento> filas2 = mEntityContext.Documento.Join(mEntityContext.Documento, docOnto => new { DocumentoID = docOnto.DocumentoID }, doc2 => new { DocumentoID = doc2.ElementoVinculadoID.Value }, (docOnto, doc2) => new { DocumentoOntologia = docOnto, Documento2 = doc2 }).Where(item => item.Documento2.ElementoVinculadoID.HasValue && item.DocumentoOntologia.ProyectoID.HasValue && item.DocumentoOntologia.ProyectoID.Value.Equals(mProyectoID) && item.DocumentoOntologia.Enlace.Equals(ontologia)).Select(item => item.Documento2).OrderBy(item => item.DocumentoID).Skip(i).Take(numRecursosRecorrer).ToList();




                List<Documento> filas = mEntityContext.Documento.Join(mEntityContext.Documento, docOnto => new { DocumentoID = docOnto.DocumentoID }, doc2 => new { DocumentoID = doc2.ElementoVinculadoID.Value }, (docOnto, doc2) => new { DocumentoOntologia = docOnto, Documento2 = doc2 }).Join(mEntityContext.DocumentoWebVinBaseRecursos, item => item.Documento2.DocumentoID, docWebVin => docWebVin.DocumentoID, (item, docWebVin) => new
                {
                    DocumentoOntologia = item.DocumentoOntologia,
                    Documento2 = item.Documento2,
                    DocumentoWebVinBaseRecursos = docWebVin
                }).Join(mEntityContext.BaseRecursosProyecto, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRec => baseRec.BaseRecursosID, (item, baseRec) => new
                {
                    DocumentoOntologia = item.DocumentoOntologia,
                    Documento2 = item.Documento2,
                    DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                    BaseRecursoProyecto = baseRec
                }).Where(item => item.Documento2.ElementoVinculadoID.HasValue && item.BaseRecursoProyecto.ProyectoID.Equals(mProyectoID) && item.DocumentoOntologia.Enlace.Equals(ontologia)).Select(item => item.Documento2).OrderBy(item => item.DocumentoID).Skip(i).Take(numRecursosRecorrer).ToList();

                List<Guid> documentosID = filas.Select(d => d.DocumentoID).ToList();

                recorrerLista = (documentosID.Count == numRecursosRecorrer);
                i += 100;

                int numeroTriples = ObtenerNumeroTriplesOntologiaDeVirtuosoPorFecha(ontologia, documentosID, urlIntragnoss);

                if (numeroTriples > 0)
                {
                    if (numeroTriples < LIMITE_VIRTUOSO)
                    {
                        InsertDelete(ontologia, urlIntragnoss, tabla, documentosID, triples);
                    }
                    else
                    {
                        for (int j = 0; j <= numeroTriples / 10000; j++)
                        {
                            InsertDelete(ontologia, urlIntragnoss, tabla, documentosID, triples);
                        }
                    }
                }
            }

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);

            FacetadoCN facetadoCN = new FacetadoCN(urlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            ControladorDocumentacion controladorDocumentacion = new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication);

            foreach (Guid recursoID in triples.Keys)
            {
                foreach (Triple triple in triples[recursoID])
                {
                    string consulta = "";
                    //Insertamos, modificamos o eliminamos el triple
                    if (string.IsNullOrEmpty(triple.objeto_new))
                    {
                        //DELETE
                        consulta = $"DELETE DATA FROM <{grafo}> {{ {triple.s} {triple.p} {triple.objeto_old} }}";
                    }
                    else if (string.IsNullOrEmpty(triple.objeto_old))
                    {
                        //INSERT
                        consulta = $"INSERT INTO <{grafo}> {{ {triple.s} {triple.p} {triple.objeto_new} }}";
                    }
                    else
                    {
                        //MODIFY
                        consulta = $"MODIFY GRAPH <{grafo}>  DELETE {{ ?s ?p ?o }} INSERT {{ ?s ?p {triple.objeto_new} }} WHERE  {{  ?s ?p ?o . filter(?s = {triple.s} AND ?p = {triple.p} AND ?o = {triple.objeto_old}) }}";
                    }

                    sw.WriteLine(consulta);
                    sw.WriteLine();

                    facetadoCN.ActualizarVirtuoso(consulta, grafo, true, 1);
                }
                try 
                { 
                    //Borramos el RDF del recurso
                    controladorDocumentacion.BorrarRDFDeBDRDFSinTransaccion(recursoID);
                }
                catch(Exception ex)
                {
                    mLoggingService.GuardarLogError(ex);
                }
                //Procesamos el recurso por el base
                controladorDocumentacion.AgregarRecursoModeloBaseSimple(recursoID, mProyectoSeleccionado.Clave, 5, AD.BASE_BD.PrioridadBase.Alta);
            }

            sw.Flush();

            if (ms.Length > 0)
            {
                string nombreFichero = tabla.TableName + "_DeleteInsert.txt";
                ms.Position = 0;
                listMStream.Add(nombreFichero, ms);
            }

            sw.Flush();
            ms.Flush();
        }


        public class Triple
        {
            public string s { get; set; }
            public string p { get; set; }
            public string objeto_new { get; set; }
            public string objeto_old { get; set; }
        }


        private void InsertDelete(string ontologia, string urlIntragnoss, DataTable tablaExcel, List<Guid> documentosID, Dictionary<Guid, List<Triple>> triples)
        {
            DataTable tablaVirtuoso;

            using (FacetadoAD facetado = new FacetadoAD(urlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication))
            {
                FacetadoDS dataSet = new FacetadoDS();
                string fullConsulta = ObtenerConsultasVirtuosoPorFecha(ontologia, dataSet, documentosID, urlIntragnoss, false);
                facetado.LeerDeVirtuoso(fullConsulta, ontologia, dataSet, ontologia);
                tablaVirtuoso = dataSet.Tables[ontologia];
            }

            //Nos recorremos los datos de virtuoso y los comparamos con los datos del excel
            foreach (DataRow filaVirtuoso in tablaVirtuoso.Rows)
            {
                string s = (string)filaVirtuoso["s"];
                string p = (string)filaVirtuoso["p"];

                DataRow filaExcelAModificar = tablaExcel.Rows.OfType<DataRow>().FirstOrDefault(item => item["s"].Equals(s) && item["p"].Equals(p));

                if (filaExcelAModificar != null)
                {
                    Guid recursoID;

                    if (s.Contains("items"))
                    {
                        string[] result = s.Split('_');
                        recursoID = new Guid(result[result.Count() - 2]);
                    }
                    else
                    {
                        string[] result = s.Split('/');
                        recursoID = new Guid(result[result.Count() - 1]);
                    }


                    for (int i = 3; i < tablaExcel.Columns.Count; i++)
                    {
                        string idioma = tablaExcel.Columns[i].ColumnName;

                        string valorExcel = string.Empty;
                        if (!filaExcelAModificar.IsNull(idioma) && !string.IsNullOrEmpty((string)filaExcelAModificar[idioma]))
                        {
                            valorExcel = (string)filaExcelAModificar[idioma];
                        }

                        string valorVirtuoso = string.Empty;
                        if (tablaVirtuoso.Columns.Contains(idioma) && !filaVirtuoso.IsNull(idioma) && !string.IsNullOrEmpty((string)filaVirtuoso[idioma]))
                        {
                            valorVirtuoso = (string)filaVirtuoso[idioma];
                        }

                        if (!valorVirtuoso.Equals(valorExcel))
                        {
                            if (!triples.ContainsKey(recursoID))
                            {
                                triples.Add(recursoID, new List<Triple>());
                            }

                            triples[recursoID].Add(GenerarTriple(s, p, valorExcel, valorVirtuoso, idioma));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Genera triple para virtuoso.
        /// </summary>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPredicado">predicado</param>
        /// <param name="pObjeto">Objeto</param>
        /// <param name="pIdioma">Idioma de la tripleta</param>
        /// <returns>Tripleta</returns>
        public static Triple GenerarTriple(string pSujeto, string pPredicado, string pObjetoNew, string pObjetoOld, string pIdioma)
        {
            if (!string.IsNullOrEmpty(pObjetoNew))
            {
                pObjetoNew = "\"" + pObjetoNew.Trim('"').Replace("\"", "\\\"").Replace("\\", "\\\\").Replace("\n", " ").Replace("\r", "") + "\"" + "@" + pIdioma;
            }

            if (!string.IsNullOrEmpty(pObjetoOld))
            {
                pObjetoOld = "\"" + pObjetoOld.Replace("\"", "\\\"").Replace("\\", "\\\\").Replace("\n", " ").Replace("\r", "") + "\"" + "@" + pIdioma;
            }

            Triple triple = new Triple { s = $"<{pSujeto}>", p = $"<{pPredicado}>", objeto_new = pObjetoNew, objeto_old = pObjetoOld };

            return triple;

        }

        public void OntologiasSecundariosToExcelVirtuoso(Guid mProyectoID, XLWorkbook mExcel, string urlIntragnoss)
        {
            List<string> ontologias = new List<string>();

            var filas = mEntityContext.Documento.Where(item => item.ProyectoID.HasValue && item.ProyectoID.Value.Equals(mProyectoID) && item.Tipo.Equals((short)23)).ToList();

            foreach (var fila in filas)
            {
                if (fila.Enlace != null && !fila.Enlace.Equals("taxonomy.owl", StringComparison.InvariantCultureIgnoreCase))
                {
                    ontologias.Add($"{fila.Enlace.ToLower()}");
                }
            }

            if (ontologias.Any())
            {
                foreach (string ontologia in ontologias)
                {
                    string grafo = urlIntragnoss + ontologia;
                    DataSet dataSet = CrearExcelOntologiasVirtuoso(ontologia, urlIntragnoss);

                    DataTable dtOtntologia = dataSet.Tables[grafo];
                    if (dtOtntologia != null)
                    {
						DataTable decodedDataTable = decodeDataTable(dtOtntologia);
						mExcel.Worksheets.Add(decodedDataTable);
                        //ExcelWorksheet ws = mExcel.Workbook.Worksheets.Add($"SecondaryOnto_{ontologia}");
                        //ws.Cells.LoadFromDataTable(dtOtntologia, true);
                        dataSet.Dispose();
                    }
                }
            }
        }

        public void OntologiaSecundariaToExcelSeleccionados(Guid mProyectoID, string pOntologia, XLWorkbook mExcel, string urlIntragnoss)
        {
            string nombreHoja = pOntologia;
            string grafo = urlIntragnoss + pOntologia;
            DataSet dataSet = CrearExcelOntologiasVirtuoso(pOntologia, urlIntragnoss);
            if (dataSet.Tables.Contains(grafo))
            {
                DataTable dtOtntologia = dataSet.Tables[grafo];
				DataTable decodedDataTable = decodeDataTable(dtOtntologia);

				mExcel.Worksheets.Add(decodedDataTable);
                //ExcelWorksheet ws = mExcel.Workbook.Worksheets.Add($"SecondaryOnto_{nombreHoja}");
                //ws.Cells.LoadFromDataTable(dtOtntologia, true);
            }
            dataSet.Dispose();
        }

        public void OntologiasSecundariasToExcelVirtuosoTaxonomy(Guid mProyectoID, XLWorkbook mExcel, string urlIntragnoss)
        {
            List<string> ontologias = new List<string>();
            var filas = mEntityContext.Documento.Where(item => item.ProyectoID.HasValue && item.ProyectoID.Value.Equals(mProyectoID) && item.Tipo.Equals((short)23)).ToList();

            foreach (var fila in filas)
            {
                if (fila.Enlace != null && fila.Enlace.Equals("taxonomy.owl", StringComparison.InvariantCultureIgnoreCase))
                {
                    ontologias.Add($"{fila.Enlace.ToLower()}");
                }
            }

            if (ontologias.Any())
            {
                foreach (string ontologia in ontologias)
                {

                    string grafo = urlIntragnoss + ontologia;
                    DataSet dataSet = CrearExcelOntologiasVirtuoso(ontologia, urlIntragnoss);
                    DataTable dtOtntologia = dataSet.Tables[grafo];
                   
                    if (dtOtntologia != null)
                    {
						DataTable decodedDataTable = decodeDataTable(dtOtntologia);

						mExcel.Worksheets.Add(decodedDataTable);
                        //ExcelWorksheet ws = mExcel.Workbook.Worksheets.Add($"taxonomy_{ontologia}");
                        //ws.Cells.LoadFromDataTable(dtOtntologia, true);
                    }

                    dataSet.Dispose();
                }
            }
        }

        private DataSet CrearExcelOntologiasVirtuoso(string pOntologia, string urlIntragnoss)
        {
            string grafo = urlIntragnoss + pOntologia;
            int numeroTriples = ObtenerNumeroTriplesOntologiaDeVirtuoso(pOntologia, urlIntragnoss);
            FacetadoDS dataSet = new FacetadoDS();

            if (numeroTriples > 0)
            {
                if (numeroTriples < LIMITE_VIRTUOSO)
                {
                    string fullConsulta = ObtenerConsultasVirtuoso(pOntologia, dataSet, urlIntragnoss);

                    using (FacetadoAD facetado = new FacetadoAD(urlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication))
                    {
                        facetado.LeerDeVirtuoso(fullConsulta, grafo, dataSet, pOntologia);
                    }
                }
                else
                {
                    for (int i = 0; i <= numeroTriples / 10000; i++)
                    {
                        string fullConsulta = string.Concat("select * where { ", ObtenerConsultasVirtuoso(pOntologia, dataSet, urlIntragnoss), $" }} limit {LIMITE_VIRTUOSO}  offset {i * 10000}");

                        using (FacetadoAD facetado = new FacetadoAD(urlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication))
                        {
                            facetado.LeerDeVirtuoso(fullConsulta, grafo, dataSet, pOntologia);
                        }
                    }
                }
            }
            return dataSet;
        }

        private DataSet CrearExcelOntologiasVirtuosoPorFecha(string pOntologia, Guid mProyectoID, DateTime? fechaFin, DateTime? fechaInicio, string urlIntragnoss)
        {
            int numRecursosRecorrer = 100;

            FacetadoDS dataSet = new FacetadoDS();

            bool recorrerLista = true;
            int i = 0;

            while (recorrerLista)
            {
                //List<Documento> filas = mEntityContext.Documento.Join(mEntityContext.Documento, doc1 => new { DocumentoID = doc1.DocumentoID }, doc2 => new { DocumentoID = doc2.ElementoVinculadoID.Value }, (doc1, doc2) => new { Documento1 = doc1, Documento2 = doc2 }).Where(item => item.Documento2.ElementoVinculadoID.HasValue && item.Documento1.ProyectoID.HasValue && item.Documento1.ProyectoID.Value.Equals(mProyectoID) && item.Documento1.Enlace.Equals(pOntologia) && (item.Documento2.FechaCreacion > fechaInicio) && (item.Documento2.FechaCreacion < fechaFin)).Select(item => item.Documento2).OrderBy(item => item.DocumentoID).Skip(i).Take(numRecursosRecorrer).ToList();



                List<Documento> filas = mEntityContext.Documento.Join(mEntityContext.Documento, docOnto => new { DocumentoID = docOnto.DocumentoID }, doc2 => new { DocumentoID = doc2.ElementoVinculadoID.Value }, (docOnto, doc2) => new { DocumentoOntologia = docOnto, Documento2 = doc2 }).Join(mEntityContext.DocumentoWebVinBaseRecursos, item => item.Documento2.DocumentoID, docWebVin => docWebVin.DocumentoID, (item, docWebVin) => new
                {
                    DocumentoOntologia = item.DocumentoOntologia,
                    Documento2 = item.Documento2,
                    DocumentoWebVinBaseRecursos = docWebVin
                }).Join(mEntityContext.BaseRecursosProyecto, item => item.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRec => baseRec.BaseRecursosID, (item, baseRec) => new
                {
                    DocumentoOntologia = item.DocumentoOntologia,
                    Documento2 = item.Documento2,
                    DocumentoWebVinBaseRecursos = item.DocumentoWebVinBaseRecursos,
                    BaseRecursoProyecto = baseRec
                }).Where(item => item.Documento2.ElementoVinculadoID.HasValue && item.BaseRecursoProyecto.ProyectoID.Equals(mProyectoID) && item.DocumentoOntologia.Enlace.Equals(pOntologia) && (item.Documento2.FechaCreacion > fechaInicio) && (item.Documento2.FechaCreacion < fechaFin)).Select(item => item.Documento2).OrderBy(item => item.DocumentoID).Skip(i).Take(numRecursosRecorrer).ToList();



                List<Guid> documentosID = filas.Select(d => d.DocumentoID).ToList();

                recorrerLista = (documentosID.Count == numRecursosRecorrer);
                i += 100;

                int numeroTriples = ObtenerNumeroTriplesOntologiaDeVirtuosoPorFecha(pOntologia, documentosID, urlIntragnoss);

                if (numeroTriples > 0)
                {
                    if (numeroTriples < LIMITE_VIRTUOSO)
                    {
                        string fullConsulta = ObtenerConsultasVirtuosoPorFecha(pOntologia, dataSet, documentosID, urlIntragnoss);


                        FacetadoDS dataSetAux = new FacetadoDS();
                        using (FacetadoAD facetado = new FacetadoAD(urlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication))
                        {
                            facetado.LeerDeVirtuoso(fullConsulta, pOntologia, dataSetAux, pOntologia);
                        }

                        dataSet.Merge(dataSetAux);
                    }
                    else
                    {
                        for (int j = 0; j <= numeroTriples / 10000; j++)
                        {
                            string fullConsulta = string.Concat("select * where { ", ObtenerConsultasVirtuosoPorFecha(pOntologia, dataSet, documentosID, urlIntragnoss), $" }} limit {LIMITE_VIRTUOSO}  offset {j * 10000}");

                            using (FacetadoAD facetado = new FacetadoAD(urlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication))
                            {
                                facetado.LeerDeVirtuoso(fullConsulta, pOntologia, dataSet, pOntologia);
                            }
                        }
                    }
                }
            }
            return dataSet;
        }
    }
}
