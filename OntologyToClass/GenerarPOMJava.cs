using Es.Riam.Gnoss.Util.GeneradorClases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OntologiaAClase
{
    class GenerarPOMJava
    {
        public StringBuilder Clase { get; }
        public GenerarPOMJava()
        {
            Clase = new StringBuilder();
        }
        public string GenerarPOM()
        {
            Clase.AppendLine();
            Clase.AppendLine("<project xmlns=\"http://maven.apache.org/POM/4.0.0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://maven.apache.org/POM/4.0.0 https://maven.apache.org/xsd/maven-4.0.0.xsd\">");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}<modelVersion>4.0.0</modelVersion>  <!-- Por defecto mantener así -->");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}<groupId>com.arquitecturaJavaGnoss</groupId>  <!-- Introducir grupo al que pertenece el proyecto maven. Lo has indicado al crear el proyecto -->");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}<artifactId>XXXXXXX</artifactId>  <!-- Introducir nombre que le has dado al proyecto. Lo has indicado al crear el proyecto -->");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}<version>1</version>  <!-- Versión inicial -->");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}<properties>");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}<project.build.sourceEncoding>UTF-8</project.build.sourceEncoding>");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}<maven.compiler.source>1.8</maven.compiler.source>");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}<maven.compiler.target>1.8</maven.compiler.target>");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}</properties>");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}<dependencies>");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}<dependency>");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}<groupId>io.github.equipognoss</groupId>");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}<artifactId>Gnoss-ApiWrapper-Java</artifactId>");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}<version>0.0.1</version>");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}</dependency>");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}</dependencies>");
            Clase.AppendLine();
            Clase.AppendLine("</project>");
            
            
            Clase.AppendLine();
         
            return Clase.ToString();
        }
    }
}
