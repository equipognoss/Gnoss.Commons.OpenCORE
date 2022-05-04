using OntologyToClass.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OntologiaAClase
{
   public class CrearCSJPROJYAssemblyInfo
    {
        public string guid ;
        public string nombreProyecto;
        public string nombreCarpeta;
        public string carpetaPadre;
        public StringBuilder ArchivoCSPROJ { get; }
        public StringBuilder ArchivoAssemblyInfo { get; }
        public StringBuilder ArchivoPackagesConfig { get; }
        public StringBuilder ArchivoApplicationInsights { get; }
        public StringBuilder ArchivoAppConfig { get; }
        public string directorio;

        public CrearCSJPROJYAssemblyInfo(string nombreProyecto,string nombreCarpeta,string carpetaPadre,string directorio)
        {
            this.guid = Guid.NewGuid().ToString();
            this.nombreProyecto = nombreProyecto;
            this.directorio=directorio;
            this.nombreCarpeta = nombreCarpeta;
            this.carpetaPadre = carpetaPadre;
            this.ArchivoCSPROJ = new StringBuilder();
            this.ArchivoAssemblyInfo = new StringBuilder();
            this.ArchivoPackagesConfig = new StringBuilder();
            this.ArchivoApplicationInsights = new StringBuilder();
            this.ArchivoAppConfig = new StringBuilder();
        }

        public string EntidadesGeneradas()
        {
            //StringBuilder entidades = new StringBuilder();
            //DirectoryInfo carpetaClasesGeneradas = new DirectoryInfo(Path.Combine(directorio,carpetaPadre, nombreCarpeta));
            //DirectoryInfo[] carpetas = carpetaClasesGeneradas.GetDirectories();
            //foreach (DirectoryInfo directorio in carpetas)
            //{
            //    FileInfo[] clases = directorio.GetFiles();
            //    foreach(FileInfo entidad in clases)
            //    {
            //        if (!$"{directorio.Name}\\{ entidad.Name}".Equals("Properties\\AssemblyInfo.cs") && !$"{directorio.Name}".Equals("Libraries"))
            //        {
            //            entidades.AppendLine($"<Compile Include=\"{directorio.Name}\\{entidad.Name}\" />");
            //        }
            //    }
            //}
            //entidades.AppendLine($@"<Compile Include=""Properties\AssemblyInfo.cs""/>");
            //return entidades.ToString();
            return "";
        }

        //public void generarAssemblyInfo()
        //{
        //    ArchivoAssemblyInfo.AppendFormat(Resources.AssemblyInfo,  nombreProyecto , guid);
        //    string ruta = Path.Combine(directorio,carpetaPadre, nombreCarpeta,"Properties");
        //    if (!Directory.Exists(ruta))
        //    {
        //        Directory.CreateDirectory(ruta);
        //    }
        //    if(!File.Exists((Path.Combine(directorio,carpetaPadre,nombreCarpeta+"Properties","AssemblyInfo.cs")))){
        //        File.WriteAllText(Path.Combine(directorio,carpetaPadre, nombreCarpeta, "Properties", "AssemblyInfo.cs"), ArchivoAssemblyInfo.ToString());
        //    }
           
        //}

        //public void generarPackages()
        //{
        //    ArchivoPackagesConfig.AppendFormat(Resources.packages, nombreProyecto);
        //    if(!File.Exists(Path.Combine(directorio, carpetaPadre, nombreCarpeta, "packages.config")))
        //    {
        //        File.WriteAllText(Path.Combine(directorio, carpetaPadre, nombreCarpeta, "packages.config"), ArchivoPackagesConfig.ToString());
        //    }
        //}

        //public void generarAppConfig()
        //{
        //    ArchivoAppConfig.AppendFormat(Resources.app, nombreProyecto);
        //    if (!File.Exists(Path.Combine(directorio, carpetaPadre, nombreCarpeta, "app.config")))
        //    {
        //        File.WriteAllText(Path.Combine(directorio, carpetaPadre, nombreCarpeta, "app.config"), ArchivoAppConfig.ToString());
        //    }
        //}

        

        //public void generarApplicationInsights()
        //{
        //    ArchivoApplicationInsights.AppendFormat(Resources.ApplicationInsights, nombreProyecto);
        //    if (!File.Exists(Path.Combine(directorio, carpetaPadre, nombreCarpeta, "ApplicationInsights.config")))
        //    {
        //        File.WriteAllText(Path.Combine(directorio, carpetaPadre, nombreCarpeta, "ApplicationInsights.config"), ArchivoApplicationInsights.ToString());
        //    }
        //}

        /// <summary>
        /// Cambiar el recurso textInfo si quieres cambios en el csproj
        /// </summary>
        public void GenerarCSPROJ()
        {
            ArchivoCSPROJ.AppendFormat(Resources.testlibrary, "{"+ guid+"}", nombreProyecto, EntidadesGeneradas());
            File.WriteAllText(Path.Combine(directorio,carpetaPadre, nombreCarpeta,$"{nombreProyecto}.csproj"), ArchivoCSPROJ.ToString());
        }

        public void GenerarApiWrapper()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //string bibliotecasDirectory = Path.Combine(baseDirectory, "bin");
            string bibliotecasDirectory = baseDirectory;
            string ruta = Path.Combine(directorio, carpetaPadre, nombreCarpeta, "Libraries");
            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }
           
            if (!File.Exists(Path.Combine(ruta, "Es.Riam.Gnoss.Web.MVC.Models.dll")))
            {
                if (File.Exists(Path.Combine(bibliotecasDirectory, "Es.Riam.Gnoss.Web.MVC.Models.dll")))
                {
                    File.Copy(Path.Combine(bibliotecasDirectory, "Es.Riam.Gnoss.Web.MVC.Models.dll"), Path.Combine(ruta, "Es.Riam.Gnoss.Web.MVC.Models.dll"));
                }
            }

            if (!File.Exists(Path.Combine(ruta, "Es.Riam.Semantica.dll")))
            {
                if (File.Exists(Path.Combine(bibliotecasDirectory, "Es.Riam.Semantica.dll")))
                {
                    File.Copy(Path.Combine(bibliotecasDirectory, "Es.Riam.Semantica.dll"), Path.Combine(ruta, "Es.Riam.Semantica.dll"));
                }
            }

            if (!File.Exists(Path.Combine(ruta, "Es.Riam.Util.dll")))
            {
                if (File.Exists(Path.Combine(bibliotecasDirectory, "Es.Riam.Util.dll")))
                {
                    File.Copy(Path.Combine(bibliotecasDirectory, "Es.Riam.Util.dll"), Path.Combine(ruta, "Es.Riam.Util.dll"));
                }
            }
        }
    }
}
