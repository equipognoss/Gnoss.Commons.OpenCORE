using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.MetaBuscadorAD;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.Comentario;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Comentario;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Es.Riam.Gnoss.Web.Controles.Exportaciones
{
    public class UtilExportaciones
    {
        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        public UtilExportaciones(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        public static MemoryStream EscribirDataTableEnExcel(DataTable dataTable, string nombreHoja = "Hoja1")
        {
            MemoryStream escritor = new MemoryStream();

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            //TODO Juan Migrar a .NetCore
            /*using (var package = new ExcelPackage(escritor))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(nombreHoja);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true, TableStyles.None);

                //Step 4 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();
            }*/
            return escritor;
        }

        public static MemoryStream EscribirDataTableEnCSV(DataTable dataTable)
        {
            MemoryStream escritor = new MemoryStream();

            if (dataTable != null)
            {
                // Recorrer los datos de la tabla e insertar en un memorystream separados por el carácter ';'
                // Quitar carácteres especiales
                StreamWriter sw = new StreamWriter(escritor, Encoding.Default);

                // Primero, agregamos las columnas
                foreach (DataColumn columna in dataTable.Columns)
                {
                    sw.Write(columna.ToString());
                    sw.Write(";");
                }

                sw.WriteLine();

                foreach (DataRow fila in dataTable.Rows)
                {
                    for (int pos = 0; pos < dataTable.Columns.Count; pos++)
                    {
                        if (fila[pos] != null)
                        {
                            sw.Write(fila[pos]);
                        }
                        sw.Write(";");
                    }

                    sw.WriteLine();
                }

                sw.Flush();
                sw.Close();
                sw.Dispose();
            }

            return escritor;

        }

        public static List<Guid> ObtenerListaDocumentosIDGnossConLaRespuestaDelServicioDeResultados(string pResultado_ServicioResultados)
        {
            List<Guid> documentosIDs = new List<Guid>();

            Dictionary<string, TiposResultadosMetaBuscador> resultadosConTipo = JsonSerializer.Deserialize<Dictionary<string, TiposResultadosMetaBuscador>>(pResultado_ServicioResultados);
            foreach (string claveContribucion in resultadosConTipo.Keys)
            {
                Guid idActual = Guid.Empty;
                if (Guid.TryParse(claveContribucion, out idActual))
                {
                    documentosIDs.Add(idActual);
                }
            }

            return documentosIDs;
        }

        public List<ElementoGnoss> ObtenerListaDocumentosGnossConLaRespuestaDelServicioDeResultados(string pResultado_ServicioResultados, Guid pProyectoID)
        {
            List<ElementoGnoss> listaDefinitiva = new List<ElementoGnoss>();

            Dictionary<string, TiposResultadosMetaBuscador> resultadosConTipo = JsonSerializer.Deserialize<Dictionary<string, TiposResultadosMetaBuscador>>(pResultado_ServicioResultados);

            for (int i = 0; i * 250 < resultadosConTipo.Count; i++)
            {
				Dictionary<string, TiposResultadosMetaBuscador> partialDictionary = resultadosConTipo.Skip(250 * i).Take(250).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

				listaDefinitiva.AddRange(FiltrarRecursos(partialDictionary, pProyectoID));
			}

            return listaDefinitiva;
        }

        /// <summary>
        /// Carga las identidades completas (identidad,persona y organizacion) a partir de una lista de guids de identidad
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identificadores de identidad</param>
        /// <param name="pDataWrapperIdentidad">Dataset de identidades</param>
        /// <param name="pDataWrapperPersona">Dataset de personas</param>
        /// <param name="pOrganizacionDW">Dataset de organizaciones</param>
        public void ObtenerIdentidadesPorID(List<Guid> pListaIdentidades, DataWrapperIdentidad pDataWrapperIdentidad, DataWrapperPersona pDataWrapperPersona, DataWrapperOrganizacion pOrganizacionDW)
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, null);
            pDataWrapperIdentidad.Merge(identidadCN.ObtenerIdentidadesPorID(pListaIdentidades, false));
            identidadCN.Dispose();

            if (pDataWrapperPersona != null)
            {
                PersonaCN persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, null);
                //pDataWrapperPersona.Merge(persCN.ObtenerPersonasIdentidadesCargadas(pIdentidadDS), true);
                pDataWrapperPersona.Merge(persCN.ObtenerPersonasIdentidadesCargadas(pDataWrapperIdentidad));
                persCN.Dispose();
            }
        }

        /// <summary>
        /// Se filtran todos los recursos y finalmente se almacena el resultado en una lista
        /// </summary>
        private List<ElementoGnoss> FiltrarRecursos(Dictionary<string, TiposResultadosMetaBuscador> pResultadosConTipo, Guid pProyectoID)
        {
            List<Guid> documentosIDs = new List<Guid>();
            List<Guid> comentariosIDs = new List<Guid>();
            List<Guid> articulosBlogIDs = new List<Guid>();
            List<Guid> personasIDs = new List<Guid>();

            foreach (string claveContribucion in pResultadosConTipo.Keys)
            {
                Guid idActual = Guid.Empty;
                if (Guid.TryParse(claveContribucion, out idActual))
                {
                    if (pResultadosConTipo[claveContribucion] == TiposResultadosMetaBuscador.Documento)
                    {
                        documentosIDs.Add(idActual);
                    }
                    else if (pResultadosConTipo[claveContribucion] == TiposResultadosMetaBuscador.Comentario)
                    {
                        comentariosIDs.Add(idActual);
                    }
                    else if (pResultadosConTipo[claveContribucion] == TiposResultadosMetaBuscador.IdentidadPersona)
                    {
                        personasIDs.Add(idActual);
                    }
                    else
                    {
                        articulosBlogIDs.Add(idActual);
                    }
                }
            }

            #region documentos
            DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();

            if (documentosIDs.Count > 0)
            {
                DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, null);
                dataWrapperDocumentacion = documentacionCN.ObtenerDocumentosPorID(documentosIDs, true);
                documentacionCN.Dispose();
            }

            //Cargamos los documentos dentro del Gestor documental.
            GestorDocumental gestorDocumental = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext);
            gestorDocumental.DataWrapperDocumentacion.Merge(dataWrapperDocumentacion);
            TesauroCL tesCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null);
            gestorDocumental.GestorTesauro = new GestionTesauro(tesCL.ObtenerTesauroDeProyecto(pProyectoID), mLoggingService, mEntityContext);
            gestorDocumental.CargarDocumentosWeb();
            #endregion

            #region comentarios
            DataWrapperComentario comDW = new DataWrapperComentario();

            if (comentariosIDs.Count > 0)
            {
                ComentarioCN comentarioCN = new ComentarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                comDW = comentarioCN.ObtenerEntradasBlogPorIDConPadreElemVinYAutor(comentariosIDs);
                comentarioCN.Dispose();
            }

            GestionComentarios mGestorComentarios = new GestionComentarios(comDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            #endregion

            #region PersonasYOrganizaciones

            GestionIdentidades gestorIdentidades = new GestionIdentidades(new DataWrapperIdentidad(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            if (personasIDs.Count > 0)
            {
                DataWrapperIdentidad identDW = new DataWrapperIdentidad();
                DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
                DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();
                DataWrapperOrganizacion organizacionDW = new DataWrapperOrganizacion();
                ObtenerIdentidadesPorID(personasIDs, identDW, dataWrapperPersona, organizacionDW);

                GestionPersonas gestorPersonas = new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext);
                GestionOrganizaciones gestorOrganizaciones = new GestionOrganizaciones(organizacionDW, mLoggingService, mEntityContext);
                GestionUsuarios gestorUsuarios = new GestionUsuarios(dataWrapperUsuario, mLoggingService, mEntityContext, mConfigService);
                gestorIdentidades = new GestionIdentidades(identDW, gestorPersonas, gestorUsuarios, gestorOrganizaciones, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            #endregion

            List<ElementoGnoss> listaDefinitiva = new List<ElementoGnoss>();

            foreach (string contribucion in pResultadosConTipo.Keys)
            {
                Guid claveContribucion = Guid.Empty;
                if (Guid.TryParse(contribucion, out claveContribucion))
                {
                    if (pResultadosConTipo[contribucion] == TiposResultadosMetaBuscador.Documento)
                    {
                        if (gestorDocumental.ListaDocumentosWeb.ContainsKey(claveContribucion))
                        {
                            listaDefinitiva.Add(gestorDocumental.ListaDocumentosWeb[claveContribucion]);
                        }
                    }
                    else if (pResultadosConTipo[contribucion] == TiposResultadosMetaBuscador.Comentario)
                    {
                        if (mGestorComentarios.ListaComentarios.ContainsKey(claveContribucion))
                        {
                            listaDefinitiva.Add(mGestorComentarios.ListaComentarios[claveContribucion]);
                        }
                    }
                    else if (pResultadosConTipo[contribucion] == TiposResultadosMetaBuscador.IdentidadPersona && gestorIdentidades != null)
                    {
                        if (gestorIdentidades.ListaIdentidades.ContainsKey(claveContribucion))
                        {
                            listaDefinitiva.Add(gestorIdentidades.ListaIdentidades[claveContribucion]);
                        }
                    }
                }
            }

            return listaDefinitiva;
        }
    }
}
