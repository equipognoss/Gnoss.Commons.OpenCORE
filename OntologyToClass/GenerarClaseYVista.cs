
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases;
using Es.Riam.InterfacesOpen;
using Es.Riam.Semantica.OWL;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OntologiaAClase
{
    public class GenerarClaseYVista
    {
        public string directorio;

        public readonly Guid proyID;
        public readonly string nombreCortoProy;
        public List<string> nombresOntologia;
        public Dictionary<string, string> dicPref;
        public List<ObjetoPropiedad> listaObjetosPropiedad = new List<ObjetoPropiedad>();
        public List<PropiedadesAMostrar> listaPropiedadesAMostrar = new List<PropiedadesAMostrar>();
        public Dictionary<string, KeyValuePair<Ontologia, byte[]>> xmlDoc = new Dictionary<string, KeyValuePair<Ontologia, byte[]>>();
        public Dictionary<string, string> dicPropiedad;
        public string carpetaPadre = "ClasesYVistas_" + DateTime.Now.ToString("yyyyMMddhhmmss");
        public string nombreCarpeta = "ClasesGeneradas";
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private IMassiveOntologyToClass mMassiveOntologyToClass;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public GenerarClaseYVista(string pDirectorio, string pNombreCortoProy, Guid pProyID, List<string> pNombresOntologia, Dictionary<string, string> dicPref, Dictionary<string, KeyValuePair<Ontologia, byte[]>> xmlDoc, bool pEsJava, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IMassiveOntologyToClass massiveOntologyToClass, ILogger<GenerarClaseYVista> logger, ILoggerFactory loggerFactory)
        {
            nombreCortoProy = pNombreCortoProy;
            proyID = pProyID;
            directorio = pDirectorio;
            this.dicPref = dicPref;
            nombresOntologia = pNombresOntologia;
            this.xmlDoc = xmlDoc;
            if (pEsJava)
            {
                carpetaPadre = "src";
                nombreCarpeta = "main\\java";
            }
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mMassiveOntologyToClass = massiveOntologyToClass;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }
       

        public void ObtenerDLL(string nombreProyecto)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe";
            startInfo.WorkingDirectory = Path.Combine(directorio, carpetaPadre, nombreCarpeta);
            startInfo.Arguments = $"{nombreProyecto}.csproj /t:Build /p:Configuration0=Release";

            process.StartInfo = startInfo;
            process.Start();
        }

        public void CrearDockerFile(string pVersionWeb, string nombreProyecto)
		{
            String dockerfileContent = $@"FROM docker.gnoss.com/web:{pVersionWeb}
WORKDIR /app

COPY {nombreProyecto}.dll ./
COPY Gnoss.Web.deps.json ./

ENTRYPOINT [""dotnet"", ""Gnoss.Web.dll""]";

            string rutaDocker = Path.Combine(directorio, carpetaPadre, "Dockerfile");
            System.IO.File.WriteAllText(rutaDocker, dockerfileContent);
        }

        public void CrearArchivoJson(string nombreProyecto)
		{
            string rutaRaizProyecto = Directory.GetCurrentDirectory();
            string rutaArchivoJson = BuscarArchivoJson(rutaRaizProyecto);
            string jsonContent = "";

            if (!string.IsNullOrEmpty(rutaArchivoJson))
            {
                bool semWebEncontrado = false;
                StringBuilder stringBuilder = new StringBuilder();
                using (var fileStream = File.OpenRead(rutaArchivoJson))
				using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
				{
					String line;
					while ((line = streamReader.ReadLine()) != null)
					{
                        stringBuilder.AppendLine(line);
                        if (line.Contains(@"""SemWeb"": ""1.0.0"","))
                        {                           
							if (!semWebEncontrado)
							{
                                semWebEncontrado = true;
                                stringBuilder.AppendLine($@"          ""{nombreProyecto}"": ""1.0.0.0"",");
                            }
                        }
                        else if (line.Contains(@"SemWeb/1.0.0"))
                        {
                            line = streamReader.ReadLine();
                            stringBuilder.AppendLine(line);
                            if (line.Contains("runtime"))
                            {
                                for(int i = 0; i < 6; i++)
								{
                                    line = streamReader.ReadLine();
                                    stringBuilder.AppendLine(line);
                                }
                                AniadirDllRuntime(ref stringBuilder, nombreProyecto);
                            }
                            else
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    line = streamReader.ReadLine();
                                    stringBuilder.AppendLine(line);
                                }
                                AniadirDll(ref stringBuilder, nombreProyecto);
                            }
                        }
                    }
				}
                jsonContent = stringBuilder.ToString();
            }

            string rutaJson = Path.Combine(directorio, carpetaPadre, "Gnoss.Web.deps.json");
            System.IO.File.WriteAllText(rutaJson, jsonContent);
		}

        public void AniadirDllRuntime(ref StringBuilder pStringBuilder, string pNombreProyecto)
		{
            pStringBuilder.AppendLine($@"      ""{pNombreProyecto} / 1.0.0.0"": {{");
            pStringBuilder.AppendLine(@"        ""runtime"": {");
            pStringBuilder.AppendLine($@"          ""{pNombreProyecto}.dll"": {{");
            pStringBuilder.AppendLine(@"            ""assemblyVersion"": ""1.0.0.0"",");
            pStringBuilder.AppendLine(@"            ""fileVersion"": ""1.0.0.0""");
            pStringBuilder.AppendLine(@"          }");
            pStringBuilder.AppendLine(@"        },");
            pStringBuilder.AppendLine(@"        ""compile"": {");
            pStringBuilder.AppendLine($@"          ""{pNombreProyecto}.dll"": {{ }}");
            pStringBuilder.AppendLine(@"        }");
            pStringBuilder.AppendLine(@"      },");
        }

        public void AniadirDll(ref StringBuilder pStringBuilder, string pNombreProyecto)
        { 
            pStringBuilder.AppendLine($@"    ""{pNombreProyecto} / 1.0.0.0"": {{");
            pStringBuilder.AppendLine(@"      ""type"": ""reference"", ");
            pStringBuilder.AppendLine(@"      ""serviceable"": false,");
            pStringBuilder.AppendLine(@"      ""sha512"": """"");
            pStringBuilder.AppendLine(@"    },");
        }

        private string BuscarArchivoJson(string directorio)
		{
            foreach (string rutaArchivo in Directory.GetFiles(directorio, "*.json"))
            {
                if (Path.GetFileName(rutaArchivo).Equals("Gnoss.Web.deps.json"))
                {
                    return rutaArchivo;
                }
            }

            foreach (string subdirectorio in Directory.GetDirectories(directorio))
            {
                string rutaArchivoEncontrado = BuscarArchivoJson(subdirectorio);
                if (!string.IsNullOrEmpty(rutaArchivoEncontrado))
                {
                    return rutaArchivoEncontrado;
                }
            }
            return null;

        }

        public void CrearObjetos(Dictionary<string, KeyValuePair<Ontologia, byte[]>> diccionarioOWLXML)
        {
            foreach (string nombreOnto in diccionarioOWLXML.Keys)
            {
                XmlDocument doc = new XmlDocument();
                Ontologia onto = diccionarioOWLXML[nombreOnto].Key;

                if (diccionarioOWLXML[nombreOnto].Value != null)
                {
                    MemoryStream ms = new MemoryStream(diccionarioOWLXML[nombreOnto].Value);
                    doc.Load(ms);
                }

                foreach (ElementoOntologia ent in onto.Entidades)
                {
                    bool esTesauroSemantico = ent.EsEntidadPathTesSemantico;
                    foreach (Propiedad prop in ent.Propiedades)
                    {
                        ObjetoPropiedad objetoPropiedad = new ObjetoPropiedad();
                        objetoPropiedad.NombrePropiedad = prop.Nombre;
                        objetoPropiedad.NombreEntidad = ent.TipoEntidad;

                        objetoPropiedad.Rango = prop.Rango;
                        if (string.IsNullOrEmpty(objetoPropiedad.Rango))
                        {
                            objetoPropiedad.Rango = ObtenerRangoDePropiedad(prop, doc);
                        }

                        objetoPropiedad.Multiidioma = UtilSemantica.ComprobarSiPropiedadEsMultiidioma(doc, prop);
                        if (prop.FunctionalProperty || prop.CardinalidadMinima > 1)
                        {
                            objetoPropiedad.EsNullable = false;
                        }
                        else
                        {
                            objetoPropiedad.EsNullable = true;
                        }
                        if (prop.ValorUnico)
                        {
                            objetoPropiedad.Multivaluada = false;
                        }
                        else
                        {
                            objetoPropiedad.Multivaluada = true;
                        }
                        objetoPropiedad.NombreOntologia = nombreOnto;
                        objetoPropiedad.Tipo = ObtenerTipoCampo(doc, prop);
                        listaObjetosPropiedad.Add(objetoPropiedad);
                    }
                }
            }
        }

        public XmlDocument LeerXML(string nombreOnto)
        {
            XmlDocument doc = new XmlDocument();
            Ontologia onto = xmlDoc[nombreOnto].Key;

            if (xmlDoc[nombreOnto].Value != null)
            {
                MemoryStream ms = new MemoryStream(xmlDoc[nombreOnto].Value);
                doc.Load(ms);
            }
            return doc;
        }

        private Dictionary<string, List<PropiedadesAMostrar>> diccionarioPropiedades = new Dictionary<string, List<PropiedadesAMostrar>>();

        public void CrearRelaciones()
        {
            foreach (string nombreOnto in xmlDoc.Keys)
            {
                ElementoOntologia entidadPrincipal = ObtenerEntidadPrincipal(xmlDoc[nombreOnto].Key);
                //List<PropiedadesAMostrar> listaPropiedades = new List<PropiedadesAMostrar>();

                diccionarioPropiedades.Add(nombreOnto, new List<PropiedadesAMostrar>());
                //PropiedadesAMostrar propiedadesPrincipal = new PropiedadesAMostrar();
                //propiedadesPrincipal.NombreProp = entidadPrincipal.TipoEntidad;

                foreach (Propiedad prop in entidadPrincipal.Propiedades)
                {
                    PropiedadesAMostrar propiedadesAmostrar = new PropiedadesAMostrar();
                    ObjetoPropiedad objetoPropiedad = BuscarObjetoPropiedad(nombreOnto, prop.ElementoOntologia.TipoEntidad, prop.Nombre);
                    propiedadesAmostrar.ObjetoPropiedad = objetoPropiedad;
                    propiedadesAmostrar.NombreProp = prop.Nombre;
                    propiedadesAmostrar.ListaPropiedadesAMostrar = new List<PropiedadesAMostrar>();
                    diccionarioPropiedades[nombreOnto].Add(propiedadesAmostrar);

                    AnadirPropiedadesAmostrar(propiedadesAmostrar, nombreOnto);
                }
                //diccionarioPropiedades.Add(nombreOnto, listaPropiedades);
            }
        }

        public List<string> BuscarNameProp(XmlNode nodos)
        {
            List<string> listaNameProp = new List<string>();
            foreach (XmlNode hijo in nodos.ChildNodes)
            {
                if (hijo.Name.Equals("NameProp"))
                {
                    listaNameProp.Add(hijo.InnerText);
                }
                else
                {
                    listaNameProp.AddRange(BuscarNameProp(hijo));
                }
            }
            return listaNameProp;
        }

        public ObjetoPropiedad BuscarObjetoPropiedad(string nombreOnto, string nombreEntidad, string nombreProp)
        {
            return listaObjetosPropiedad.FirstOrDefault(x => x.NombreOntologia.Equals(nombreOnto) && x.NombreEntidad.Equals(nombreEntidad) && x.NombrePropiedad.Equals(nombreProp, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool BuscarObjeto(PropiedadesAMostrar propMostrar, ObjetoPropiedad padre, ObjetoPropiedad hijo)
        {
            if (propMostrar.ObjetoPropiedad.Equals(padre) && propMostrar.ListaPropiedadesAMostrar.Any(x => x.ObjetoPropiedad.Equals(hijo)))
            {
                return true;
            }
            else if (propMostrar.ListaPropiedadesAMostrar != null)
            {
                return propMostrar.ListaPropiedadesAMostrar.Any(x => BuscarObjeto(x, padre, hijo));
            }
            else
            {
                return false;
            }
        }
        
        public void AnadirPropiedadesAmostrar(PropiedadesAMostrar prop, string nombreOnto)
        {
            ObjetoPropiedad objetoProp = prop.ObjetoPropiedad;

            if (!objetoProp.Rango.Equals("http://www.w3.org/2001/XMLSchema#string") && !objetoProp.Rango.Equals("http://www.w3.org/2001/XMLSchema#int") && !objetoProp.Rango.Equals("http://www.w3.org/2001/XMLSchema#date") )
            {
                XmlDocument doc = LeerXML(nombreOnto);
                //List<PropiedadesAMostrar> lista = new List<PropiedadesAMostrar>();
                XmlNode nodo = doc.SelectSingleNode($"config/EspefEntidad/Entidad[@ID=\"{ objetoProp.Rango}\"]/OrdenEntidadLectura");
                if (nodo == null)
                {
                    nodo = doc.SelectSingleNode($"config/EspefEntidad/Entidad[@ID=\"{ objetoProp.Rango}\"]/OrdenEntidad");
                }
                if (nodo != null)
                {
                    foreach (string nombreHijo in BuscarNameProp(nodo))
                    {
                        if (!string.IsNullOrEmpty(nombreHijo))
                        {
                            ObjetoPropiedad objetoPropiedad = BuscarObjetoPropiedad(nombreOnto, objetoProp.Rango, nombreHijo);

                            if (!diccionarioPropiedades[nombreOnto].Any(x => BuscarObjeto(x, objetoProp, objetoPropiedad)))
                            {
                                if (objetoPropiedad != null)
                                {
                                    PropiedadesAMostrar propiedad = new PropiedadesAMostrar();
                                    propiedad.ObjetoPropiedad = objetoPropiedad;
                                    propiedad.NombreProp = nombreHijo;
                                    propiedad.ListaPropiedadesAMostrar = new List<PropiedadesAMostrar>();
                                    prop.ListaPropiedadesAMostrar.Add(propiedad);

                                    AnadirPropiedadesAmostrar(propiedad, nombreOnto);
                                }
                            }
                        }
                    }
                }
            }
        }

        public ElementoOntologia ObtenerEntidadPrincipal(Ontologia ontologia)
        {
            foreach (ElementoOntologia pEntidad in ontologia.Entidades)
            {
                if (ontologia.EntidadesAuxiliares.Count == ontologia.Entidades.Count)
                {
                    return pEntidad;
                }
                if (!ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)) && !pEntidad.Superclases.Any(s => !s.Contains("Thing")))
                {
                    return pEntidad;
                }
            }
            return null;
        }
        
        public string ObtenerRangoDePropiedad(Propiedad pPropiedad, XmlDocument doc)
        {
            string clase = string.Empty;
            if (string.IsNullOrEmpty(pPropiedad.Rango))
            {
                XmlNode nodo = doc.SelectNodes($"config/EspefPropiedad/Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/SeleccionEntidad/Grafo")?[0];
                if (nodo != null)
                {
                    string nombre = nodo.InnerText;
                    if (nombre.Contains("."))
                    {
                        nombre = nodo.InnerText.Substring(0, nodo.InnerText.LastIndexOf("."));
                    }

                    if (nombresOntologia.Any(n => n.Equals(nombre, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        XmlNode node = doc.SelectNodes($"config/EspefPropiedad/Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/SeleccionEntidad/PropsEntLectura/Propiedad/@EntidadID")?[0];
                        if (node != null)
                        {
                            clase = node.InnerText;
                        }
                        else
                        {
                            clase = CompletarTipoSeleccion(pPropiedad, doc);
                        }
                    }
                    else
                    {
                        clase = CompletarTipoSeleccion(pPropiedad, doc);
                    }
                }
                else
                {
                    clase = CompletarTipoSeleccion(pPropiedad, doc);

                }

            }
            return clase;
        }


        public string CompletarTipoSeleccion(Propiedad pPropiedad, XmlDocument doc)
        {
            string clase = "object";

            XmlNodeList tipoSeleccion = doc.SelectNodes($"config/EspefPropiedad/Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/SeleccionEntidad/TipoSeleccion");
            if (tipoSeleccion != null)
            {
                if (tipoSeleccion.Count > 0 && tipoSeleccion[0].InnerText.Equals("Tesauro"))
                {
                    clase = "Concept";
                }

            }
            return clase;
        }

        public ObjetoPropiedad.TipoObjeto ObtenerTipoCampo(XmlDocument doc, Propiedad prop)
        {
            ObjetoPropiedad.TipoObjeto tipo = ObjetoPropiedad.TipoObjeto.other;

            XmlNode tipoCampo = doc.SelectSingleNode($"config/EspefPropiedad/Propiedad[@ID =\"{prop.Nombre}\" and @EntidadID=\"{prop.ElementoOntologia.TipoEntidad}\"]/TipoCampo");
            if (tipoCampo != null)
            {
                switch (tipoCampo.InnerText)
                {
                    case "Imagen":
                        tipo = ObjetoPropiedad.TipoObjeto.image;
                        break;
                    case "Link":
                        tipo = ObjetoPropiedad.TipoObjeto.link;
                        break;
                    case "TextEditor":
                        tipo = ObjetoPropiedad.TipoObjeto.textEditor;
                        break;
                    case "TextArea":
                        tipo = ObjetoPropiedad.TipoObjeto.textEditor;
                        break;
                    case "ArchivoLink":
                        tipo = ObjetoPropiedad.TipoObjeto.ArchivoLink;
                        break;
                }
            }
            return tipo;
        }        

        /// <summary>
        /// Obtenemos las clases del xml
        /// </summary>
        /// <param name="rutaOwl"></param>
        /// <param name="contentOWL"></param>
        /// <param name="rutaXml"></param>
        /// <param name="esprimaria"></param>
        /// <param name="listaIdiomas"></param>
        public void CrearClases(OntologiaGenerar ontologia)
        {
            ControladorOntologiaAClase ontologytoclass = new ControladorOntologiaAClase(ontologia, carpetaPadre, nombreCarpeta, directorio, nombreCortoProy, proyID, nombresOntologia, listaObjetosPropiedad, dicPref, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mMassiveOntologyToClass, mLoggerFactory.CreateLogger<ControladorOntologiaAClase>(), mLoggerFactory);
            ontologytoclass.GenerarClaseOCBase();
            ontologytoclass.CrearClasesDeLaOntologia();
        }

        /// <summary>
        /// Obtenemos las clases del xml
        /// </summary>
        /// <param name="rutaOwl"></param>
        /// <param name="contentOWL"></param>
        /// <param name="rutaXml"></param>
        /// <param name="esprimaria"></param>
        /// <param name="listaIdiomas"></param>
        public void CrearClasesJava(OntologiaGenerar ontologia)
        {
            ControladorOntologiaAClaseJava ontologytoclassJava = new ControladorOntologiaAClaseJava(ontologia, carpetaPadre, nombreCarpeta, directorio, nombreCortoProy, proyID, nombresOntologia, listaObjetosPropiedad, dicPref, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorOntologiaAClaseJava>(), mLoggerFactory);
            ontologytoclassJava.GenerarClaseOCBase();
            ontologytoclassJava.CrearClasesDeLaOntologia();
        }

        public void CrearVistas(OntologiaGenerar ontologia, string nombreProyecto)
        {
            VistaRecurso vista = new VistaRecurso(ontologia.nombreOnto, ontologia.ontologia, ontologia.contentXML, ontologia.esPrincipal, nombreProyecto, carpetaPadre, directorio, nombreCortoProy, proyID, dicPref, listaObjetosPropiedad, diccionarioPropiedades, mLoggingService);
            vista.EscribirVista();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ontologias"></param>
        public void GenerarPaqueteCSPROJ(string nombreProyecto)
        {
            CrearCSJPROJYAssemblyInfo csprojAssembly = new CrearCSJPROJYAssemblyInfo(nombreProyecto, nombreCarpeta, carpetaPadre, directorio);
            csprojAssembly.GenerarCSPROJ();
            //csprojAssembly.generarAssemblyInfo();            
            //csprojAssembly.generarPackages();
            //csprojAssembly.generarApplicationInsights();
            //csprojAssembly.generarAppConfig();
            csprojAssembly.GenerarApiWrapper();
        }   
    }
}
