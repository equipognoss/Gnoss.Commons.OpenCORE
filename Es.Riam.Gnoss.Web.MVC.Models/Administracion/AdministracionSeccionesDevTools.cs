using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    // Modelo que se utilizará para contener las diferencias secciones de Administración. Se utiliza en cada Controller para indicar mediante el enum la sección en la que se encuentra
    // el usuario (ViewBag) para así, mostrar o no el menú lateral correspondiente a la sección mostrada en pantalla.   
    public static class AdministracionSeccionesDevTools
    {        
        public enum SeccionesDevTools
        {
            ConfiguracionInicial,
            Home,
            Comunidad,
            Estructura,
            Configuracion,
            GrafoConocimiento,
            DescubrimientoAnalisis,
            Apariencia,
            IntegracionContinua,
            Documentacion,
            Ecosistema
        }

        public enum SubSeccionesDevTools {
            // Subseccion de Configuración inicial
            ConfiguracionInicial_DatosAcceso_Urls,
            // Subsecciones de la sección Comunidad
            Comunidad_Home,
            Comunidad_InformacionGeneral,
            Comunidad_TiposDeContenidoPermisos,
            Comunidad_Categorias,
            Comunidad_InteraccionSocial,
            Comunidad_Miembros,
            Comunidad_NivelesDeCertificacion,
            Comunidad_IntegracionRedesSociales,
            Comunidad_IntegracionSharePoint,
            Comunidad_Solicitudes,
            Comunidad_Solicitudes_Grupos,

            // Subsecciones de la sección Estructura
            Estructura_Home,
            Estructura_Paginas,
            Estructura_Paginas_CMSBuilder,
            Estructura_Componentes,
            Estructura_Multimedia,
            Estructura_Redirecciones,

            // Subsecciones de la sección Configuración
            Configuracion_Home,
            Configuracion_TraduccionesComunidad,
            Configuracion_Trazas,
            Estado_Servicios,
            Configuracion_Cache,
            Configuracion_OAuth,
            Configuracion_AccesoFTP,
            Configuracion_SEO,
            Configuracion_Matomo,
            Configuracion_BuzonDeCorreo,
            Configuracion_ClausulasLegales,
            Configuracion_Cookies,
            Configuracion_OpcionesMetaAdministrador,
            Configuracion_OpcionesAvanzadasPlataforma,
            Configuracion_ServiciosExternos,
            Configuracion_Ecosistema,
            Configuracion_DatosExtra,
            Descargar_Configuraciones, 

            // Subsecciones de la sección Objetos de Conocimiento
            GrafoConocimiento_Home,
            GrafoConocimiento_ObjetosConocimiento_Ontologias,
            GrafoConocimiento_Tesauros_Semanticos,
            GrafoConocimiento_GrafosSimples,
            GrafoConocimiento_SparQL,
            GrafoConocimiento_CargaMasiva,
            GrafoConocimiento_BorradoMasivo,

            // Subsecciones de la sección Descubrimiento y Analisis
            DescubrimientoAnalisis_Home,
            DescubrimientoAnalisis_Facetas,
            DescubrimientoAnalisis_Sugerencias_de_busqueda,
            DescubrimientoAnalisis_Resultados,
            DescubrimientoAnalisis_Personalizacion_Consulta_Busqueda,
            DescubrimientoAnalisis_Mapa,
            DescubrimientoAnalisis_Graficos,
            DescubrimientoAnalisis_Informacion_Contextual,

            // Subsecciones de la sección Apariencia
            Apariencia_Home,
            Apariencia_Vistas,

            // Subsecciones de la sección Integración contínua
            IntegracionContinua_Home,
            IntegracionContinua_AdministrarCambios,
            IntegracionContinua_AdministrarDespliegues,
            IntegracionContinua_AdministrarIntegracionContinua,

            // Subsecciones de la sección Documentación
            Documentacion_Indice,
            Documentacion_Comunidad,
            Documentacion_Estructura,
            Documentacion_Configuracion,
            Documentacion_GrafoConocimiento,
            Documentacion_DescubrimientoAnalisis,
            Documentacion_Apariencia,
            Documentacion_IntegracionContinua,                        
        }
    }
}
