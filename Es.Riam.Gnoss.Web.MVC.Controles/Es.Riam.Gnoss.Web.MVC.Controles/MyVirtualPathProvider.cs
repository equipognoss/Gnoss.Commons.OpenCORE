using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controles
{
    //TODO Javier migrar a .net core
    //public class MyVirtualPathProvider : VirtualPathProvider
    //{
    //    internal const string USAR_VISTAS_V2 = "UsarVistasV2";
    //    internal const string USAR_VISTAS_V1 = "UsarVistasV1";

    //    public static ConcurrentDictionary<string, string> listaRutasVirtuales = new ConcurrentDictionary<string, string>();
    //    public static Dictionary<Guid, string> ListaHtmlsTemporales = new Dictionary<Guid, string>();

    //    private static List<string> ListaPaginasRealesExisten = new List<string>();

    //    private string mDirectorioVistas;
    //    private bool? mVistasNuevas;

    //    private LoggingService mLoggingService;
    //    private EntityContext mEntityContext;
    //    private IHttpContextAccessor mHttpContextAccessor;
    //    private ConfigService mConfigService;
    //    private RedisCacheWrapper mRedisCacheWrapper;
    //    private VirtuosoAD mVirtuosoAD;

    //    public MyVirtualPathProvider(EntityContext entityContext, LoggingService loggingService, IHttpContextAccessor httpContextAccessor, RedisCacheWrapper redisCacheWrapper, ConfigService configService, VirtuosoAD virtuosoAD)
    //    {
    //        mVirtuosoAD = virtuosoAD;
    //        mLoggingService = loggingService;
    //        mEntityContext = entityContext;
    //        mConfigService = configService;
    //        mHttpContextAccessor = httpContextAccessor;
    //        mRedisCacheWrapper = redisCacheWrapper;
    //    }

    //    private bool VistasNuevas
    //    {
    //        get
    //        {
    //            if (!mVistasNuevas.HasValue)
    //            {
    //                ParametroAplicacionCL parametroAplicacionCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService);
    //                List<ParametroAplicacion> parametrosAplicacion = parametroAplicacionCL.ObtenerParametrosAplicacionPorContext();
    //                if (parametrosAplicacion.Find(parametroApp => parametroApp.Parametro.Equals(TiposParametrosAplicacion.VistasV2)) != null)
    //                {
    //                    mVistasNuevas = Boolean.Parse(parametrosAplicacion.Find(parametroApp => parametroApp.Parametro.Equals(TiposParametrosAplicacion.VistasV2)).Valor);
    //                }

    //                if (mVistasNuevas == null)
    //                {
    //                    mVistasNuevas = UtilUsuario.VistasNuevas;
    //                }
    //            }
    //            return mVistasNuevas.Value;
    //        }
    //    }

    //    public string DirectorioVistas
    //    {
    //        get
    //        {
    //            bool? forzarVistasV2 = (bool?)UtilPeticion.ObtenerObjetoDePeticion(USAR_VISTAS_V2);
    //            bool? forzarVistasV1 = (bool?)UtilPeticion.ObtenerObjetoDePeticion(USAR_VISTAS_V1);

    //            if (forzarVistasV2.HasValue && forzarVistasV2.Value)
    //            {
    //                return "Views";
    //            }
    //            else if (forzarVistasV1.HasValue && forzarVistasV1.Value)
    //            {
    //                return "ViewsV1";
    //            }
    //            else if (mDirectorioVistas == null)
    //            {
    //                if (VistasNuevas)
    //                {
    //                    mDirectorioVistas = "Views";
    //                }
    //                else
    //                {
    //                    mDirectorioVistas = "ViewsV1";
    //                }
    //            }
    //            return mDirectorioVistas;
    //        }
    //    }

    //    public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
    //    {

    //        if (virtualPath.Contains("/Views/") && !virtualPath.Contains("$$$"))
    //        {
    //            virtualPath = virtualPath.Replace("Views", DirectorioVistas);

    //            for (int i = 0; i < ((ArrayList)virtualPathDependencies).Count; i++)
    //            {
    //                if (((string)((ArrayList)virtualPathDependencies)[i]).Contains("/Views/"))
    //                {
    //                    ((ArrayList)virtualPathDependencies)[i] = ((string)((ArrayList)virtualPathDependencies)[i]).Replace("Views", DirectorioVistas);
    //                }
    //                else if (((string)((ArrayList)virtualPathDependencies)[i]).Contains("/ViewsV1/"))
    //                {
    //                    ((ArrayList)virtualPathDependencies)[i] = ((string)((ArrayList)virtualPathDependencies)[i]).Replace("ViewsV1", DirectorioVistas);
    //                }

    //            }
    //        }

    //        mLoggingService.AgregarEntrada($"GetFileHash {virtualPath}");

    //        string resultado = null;
    //        if (virtualPath.EndsWith(".cshtml") && virtualPath.Contains("$$$"))
    //        {
    //            try
    //            {
    //                Guid personalizacionID = new Guid(virtualPath.Substring(virtualPath.IndexOf("$$$") + 3, 36));

    //                Dictionary<string, string> diccionarioRefrescoCacheLocal = mRedisCacheWrapper.Cache.Get(GnossCacheCL.CLAVE_DICCIONARIO_REFRESCO_CACHE_VISTAS + personalizacionID) as Dictionary<string, string>;

    //                if (diccionarioRefrescoCacheLocal != null)
    //                {
    //                    string virtualPathAux = virtualPath.Remove(virtualPath.IndexOf("$$$"), 39).Replace("/.cshtml", "/Index.cshtml");

    //                    resultado = "";
    //                    if (diccionarioRefrescoCacheLocal.ContainsKey("PersonalizacionID"))
    //                    {
    //                        resultado += diccionarioRefrescoCacheLocal["PersonalizacionID"];
    //                    }
    //                    if (diccionarioRefrescoCacheLocal.ContainsKey(virtualPathAux))
    //                    {
    //                        resultado += diccionarioRefrescoCacheLocal[virtualPathAux];
    //                    }

    //                    if (!string.IsNullOrEmpty(resultado))
    //                    {
    //                        mLoggingService.AgregarEntrada($"fin GetFileHash {resultado}");
    //                        return resultado;
    //                    }
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                mLoggingService.AgregarEntrada($"ERROR => GetFileHash " + ex.Message);
    //            }
    //        }

    //        mLoggingService.AgregarEntrada($"base.GetFileHash");
    //        resultado = base.GetFileHash(virtualPath, virtualPathDependencies);
    //        mLoggingService.AgregarEntrada($"fin base.GetFileHash {resultado}");
    //        return DirectorioVistas + "_" + resultado;
    //    }

    //    private string GetRealPath(string virtualPath)
    //    {
    //        string virtualPathAux = virtualPath.Trim('~');
    //        string directorioVistas;
    //        if (virtualPathAux.Contains("/Views/"))
    //        {
    //            if (!virtualPathAux.Contains("$$$"))
    //            {
    //                virtualPathAux = virtualPathAux.Replace("Views", DirectorioVistas);
    //                directorioVistas = DirectorioVistas;
    //            }
    //            else
    //            {
    //                directorioVistas = "Views";
    //            }
    //            virtualPathAux = virtualPathAux.Substring(virtualPathAux.IndexOf($"/{directorioVistas}/"));

    //            if (virtualPathAux.Contains("/../"))
    //            {
    //                virtualPathAux = $"/{directorioVistas}/" + virtualPathAux.Substring(virtualPathAux.IndexOf("/../") + 4);
    //            }
    //        }
    //        return virtualPathAux;
    //    }

    //    public override bool FileExists(string virtualPath)
    //    {
    //        mLoggingService.AgregarEntrada($"FileExists {virtualPath}");

    //        string virtualPathAux = GetRealPath(virtualPath);

    //        if (virtualPathAux.Equals("/bundle.config")) { return false; }

    //        bool existe = false;
    //        string data = string.Empty;
    //        if (virtualPathAux.Contains("$$$"))
    //        {
    //            data = FindPage(virtualPathAux);
    //            existe = !string.IsNullOrEmpty(data);
    //        }
    //        else if (!string.IsNullOrEmpty(Path.GetExtension(virtualPath)))
    //        {
    //            if (ListaPaginasRealesExisten.Contains(virtualPathAux))
    //            {
    //                existe = true;
    //            }
    //            else
    //            {
    //                mLoggingService.AgregarEntrada($"base.FileExists");
    //                existe = base.FileExists(virtualPath);

    //                if (existe)
    //                {
    //                    ListaPaginasRealesExisten.Add(virtualPathAux);
    //                }
    //            }
    //        }

    //        mLoggingService.AgregarEntrada($"fin FileExists {existe}");
    //        return existe;
    //    }

    //    public override VirtualFile GetFile(string virtualPath)
    //    {
    //        mLoggingService.AgregarEntrada($"GetFile {virtualPath}");

    //        string virtualPathAux = GetRealPath(virtualPath);

    //        VirtualFile resultado = null;
    //        string data = string.Empty;
    //        if (virtualPathAux.Contains("$$$"))
    //        {
    //            data = FindPage(virtualPathAux);
    //            if (!string.IsNullOrEmpty(data))
    //            {
    //                mLoggingService.AgregarEntrada($"new VirtualFile");
    //                resultado = new MyVirtualFile(virtualPath, data);
    //            }
    //        }
    //        else if (VistasNuevas) // comprobar si hay que obtener la versión nueva
    //        {
    //            mLoggingService.AgregarEntrada($"base.GetFile");
    //            resultado = base.GetFile(virtualPath);
    //        }
    //        else
    //        {
    //            data = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, virtualPathAux.Replace("/", "\\").TrimStart('\\')));
    //            resultado = new MyVirtualFile(virtualPath, data);
    //        }

    //        mLoggingService.AgregarEntrada($"Fin GetFile");
    //        return resultado;
    //    }

    //    public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
    //    {

    //        if (virtualPath.Contains("/Views/") && !virtualPath.Contains("$$$"))
    //        {
    //            virtualPath = virtualPath.Replace("Views", DirectorioVistas);
    //            for (int i = 0; i < ((ArrayList)virtualPathDependencies).Count; i++)
    //            {
    //                if (((string)((ArrayList)virtualPathDependencies)[i]).Contains("/Views/"))
    //                {
    //                    ((ArrayList)virtualPathDependencies)[i] = ((string)((ArrayList)virtualPathDependencies)[i]).Replace("Views", DirectorioVistas);
    //                }
    //                else if (((string)((ArrayList)virtualPathDependencies)[i]).Contains("/ViewsV1/"))
    //                {
    //                    ((ArrayList)virtualPathDependencies)[i] = ((string)((ArrayList)virtualPathDependencies)[i]).Replace("ViewsV1", DirectorioVistas);
    //                }

    //            }
    //        }

    //        CacheDependency resultado = null;
    //        mLoggingService.AgregarEntrada($"GetCacheDependency {virtualPath}");

    //        if (!virtualPath.EndsWith(".cshtml"))
    //        {
    //            mLoggingService.AgregarEntrada($"base.GetCacheDependency");
    //            resultado = base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
    //            mLoggingService.AgregarEntrada($"fin base.GetCacheDependency");
    //        }
    //        else
    //        {
    //            mLoggingService.AgregarEntrada($"fin GetCacheDependency return null");
    //            return null;
    //        }

    //        if (resultado == null)
    //        {
    //            mLoggingService.AgregarEntrada($"new CacheDependency");
    //            resultado = new CacheDependency(HostingEnvironment.MapPath(virtualPath));
    //        }
    //        mLoggingService.AgregarEntrada($"fin GetCacheDepenendency");
    //        return resultado;
    //    }

    //    private string FindPage(string virtualPath)
    //    {
    //        if (virtualPath.Contains("/Views/") && !virtualPath.Contains("$$$"))
    //        {
    //            virtualPath = virtualPath.Replace("Views", DirectorioVistas);
    //        }

    //        mLoggingService.AgregarEntrada($"FindPage {virtualPath}");
    //        string html = string.Empty;


    //        if (listaRutasVirtuales.ContainsKey(virtualPath))
    //        {
    //            html = listaRutasVirtuales[virtualPath];
    //        }
    //        else
    //        {
    //            string[] parametrosRuta = virtualPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

    //            if (parametrosRuta.Length > 2 && parametrosRuta[0].Equals("Views") && parametrosRuta[1].Equals("TESTvistaTEST") && parametrosRuta.Last().EndsWith(".cshtml") && parametrosRuta.Last().Contains("$$$") && !parametrosRuta.Last().EndsWith(".Mobile.cshtml"))
    //            {
    //                Guid idVistaTemporal = new Guid(parametrosRuta[2].Substring(0, parametrosRuta[2].IndexOf("$$$")));
    //                if (MyVirtualPathProvider.ListaHtmlsTemporales.ContainsKey(idVistaTemporal))
    //                {
    //                    html = ListaHtmlsTemporales[idVistaTemporal];
    //                }
    //            }
    //            else if (parametrosRuta.Length > 2 && parametrosRuta[0].Equals("Views") && parametrosRuta.Last().EndsWith(".cshtml") && !parametrosRuta.Last().EndsWith(".Mobile.cshtml"))
    //            {
    //                string[] parametrosPagina = parametrosRuta.Last().Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);

    //                if (parametrosPagina.Length > 0)
    //                {
    //                    Guid personalizacionID;

    //                    Guid.TryParse(parametrosPagina.Last().Substring(0, parametrosPagina.Last().IndexOf('.')), out personalizacionID);
    //                    if (!personalizacionID.Equals(Guid.Empty))
    //                    {
    //                        string tipoPagina = virtualPath.Substring(0, virtualPath.LastIndexOf('/')).Substring(7);

    //                        if (parametrosPagina[0].StartsWith("_"))
    //                        {
    //                            tipoPagina += parametrosPagina[0];
    //                        }

    //                        VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService);

    //                        if (tipoPagina == "FichaRecurso")
    //                        {
    //                            string rdfType = parametrosPagina[0];
    //                            html = vistaVirtualCN.ObtenerHtmlParaVistaRDFTypeDePersonalizacion(personalizacionID, rdfType);
    //                        }

    //                        if (tipoPagina == "CMSPagina")
    //                        {
    //                            Guid personalizacionComponenteID;
    //                            Guid.TryParse(parametrosPagina[0], out personalizacionComponenteID);

    //                            if (!personalizacionComponenteID.Equals(Guid.Empty))
    //                            {
    //                                html = vistaVirtualCN.ObtenerHtmlParaVistaCMSDePersonalizacion(personalizacionID, personalizacionComponenteID);
    //                            }
    //                        }

    //                        if (tipoPagina == "Shared")
    //                        {
    //                            Guid personalizacionComponenteID;
    //                            Guid.TryParse(parametrosPagina[0], out personalizacionComponenteID);

    //                            if (!personalizacionComponenteID.Equals(Guid.Empty))
    //                            {
    //                                html = vistaVirtualCN.ObtenerHtmlParaVistaGadgetDePersonalizacion(personalizacionID, personalizacionComponenteID);
    //                            }
    //                        }

    //                        if (string.IsNullOrEmpty(html))
    //                        {
    //                            string realPath = virtualPath.Replace("$$$" + personalizacionID.ToString(), "");
    //                            if (realPath.EndsWith("/.cshtml"))
    //                            {
    //                                realPath = realPath.Replace("/.cshtml", "/Index.cshtml");
    //                            }

    //                            html = vistaVirtualCN.ObtenerHtmlParaVistaDePersonalizacion(personalizacionID, realPath);

    //                            if (string.IsNullOrEmpty(html) && virtualPath.Contains("/Views/") && !virtualPath.Contains("$$$"))
    //                            {
    //                                virtualPath = virtualPath.Replace("Views", DirectorioVistas);
    //                            }
    //                        }

    //                        listaRutasVirtuales.TryAdd(virtualPath, html);
    //                    }
    //                }
    //            }
    //        }
    //        mLoggingService.AgregarEntrada($"fin FindPage {virtualPath}");
    //        return html;
    //    }

    //    public static void LimpiarListasRutasVirtuales()
    //    {
    //        listaRutasVirtuales.Clear();
    //    }
    //}

    //public class MyVirtualFile : VirtualFile
    //{
    //    private readonly string _content;

    //    public MyVirtualFile(string virtualPath, string data)
    //        : base(virtualPath)
    //    {
    //        this._content = data;
    //    }

    //    public override System.IO.Stream Open()
    //    {
    //        var resultado = new MemoryStream(System.Text.Encoding.Default.GetBytes(_content), false);
    //        return resultado;
    //    }
    //}
}