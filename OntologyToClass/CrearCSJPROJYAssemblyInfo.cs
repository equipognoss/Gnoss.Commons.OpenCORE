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

        /// <summary>
        /// Cambiar el recurso textInfo si quieres cambios en el csproj
        /// </summary>
        public void GenerarCSPROJ()
        {
            ArchivoCSPROJ.AppendFormat(Resources.testlibrary, $"{{{guid}}}", nombreProyecto, "");
            File.WriteAllText(Path.Combine(directorio,carpetaPadre, nombreCarpeta,$"{nombreProyecto}.csproj"), ArchivoCSPROJ.ToString());
        }

        public void GenerarApiWrapper()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
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
