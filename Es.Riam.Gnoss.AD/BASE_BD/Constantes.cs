using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.BASE_BD
{
    public class Constantes
    {
        /// <summary>
        /// Constante para codificar los tags de tipo categorias de documentos
        /// </summary>
        public const string CAT_DOC = "##CATDOC##";
        /// <summary>
        /// Constante para codificar los tags de tipo autor de documento
        /// </summary>
        public const string AUT_DOC = "##AUTDOC##";
        /// <summary>
        /// Constante para codificar los tags de tipo publicador de documento
        /// </summary>
        public const string PUB_DOC = "##PUB_DOC##";
        /// <summary>
        ///Constante para codificar los tags de tipo Fecha de publicacion de documento
        /// </summary>
        public const string FECHAPUB_DOC = "##FECHAPUB_DOC##";
        /// <summary>
        ///Constante para codificar los tags de tipo Extension de documento
        /// </summary>
        public const string EXT_DOC = "##EXT_DOC##";
        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string NIVCER_DOC = "##NIVCER_DOC##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string TIPO_DOC = "##TIPO_DOC##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string NOMBRE_DOC = "##NOMBRE_DOC##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string ENLACE_DOC = "##ENLACE_DOC##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string ENLACE_DOC_ELIMINADO = "##ENLACE_DOC_ELIMINADO##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string ESTADO_COMENTADO = "##ESTADO_COM##";

        /// <summary>
        ///Constante para codificar los tags de tipo ID-Tag de dafo
        /// </summary>
        public const string ID_TAG_DOCUMENTO = "##ID_TAG_DOC##";

        /// <summary>
        ///Constante para codificar el id de los proyectos de destino para compartir ontologías
        /// </summary>
        public const string ID_PROY_DESTINO = "##ID_PROY_DESTINO##";

        /// <summary>
        ///Constante para codificar los tags de tipo ID-Tag de dafo
        /// </summary>
        public const string SAME_AS = "##enlaces##";

        /// <summary>
        ///Constante para codificar los ids de los proyectos origen.
        /// </summary>
        public const string ID_PROYECTO_ORIGEN = "##ID_PROYECTO_ORIGEN##";

        /// <summary>
        ///Constante para codificar el tipo de inserción de todos los recursos en el base.
        /// </summary>
        public const string GENERAR_TODOS_RECURSOS = "##GENERAR_TODOS_RECURSOS##";

        /// <summary>
        ///Constante para codificar el tipo de inserción de todos las facetas en el autocompletar.
        /// </summary>
        public const string GENERAR_TODAS_FACETAS = "##GENERAR_TODAS_FACETAS##";
        /// <summary>
        ///Constante para codificar el tipo de inserción de todos las facetas en el autocompletar.
        /// </summary>
        public const string ELIMINAR_TODAS_FACETAS = "##ELIMINAR_TODAS_FACETAS##";
        /// <summary>
        ///Constante para identificar la faceta en los tags de una fila de la ColaFacetas.
        /// </summary>
        public const string FACETA = "##FACETA##";
        /// <summary>
        ///Constante para identificar el objeto de conocimiento en los tags de una fila de la ColaFacetas.
        /// </summary>
        public const string OBJETO_CONOCIMIENTO = "##OBJETO_CONOCIMIENTO##";
        /// <summary>
        ///Constante para indicar si hay que eliminar la faceta de alguna pestaña en concreto
        /// </summary>
        public const string PESTANYA_ELIMINAR = "##PESTANYA_ELIMINAR##";

        /// <summary>
        /// Constante para codificar la cadena de afinidad de virtuoso
        /// </summary>
        public const string AFINIDAD_VIRTUOSO = "##AFINIDAD_VIRTUOSO##";

        public const string COM_U_REC = "##COM-REC##";
        /// <summary>
        /// Constante para codificar el origen del ID de la invitacion
        /// </summary>
        public const string ID_INVITACION = "##ID_INVITACION##";


        /// <summary>
        /// Constante para codificar el origen del ID destino de la invitacion
        /// </summary>
        public const string ID_INVITACION_IDDESTINO = "##ID_INVITACION_IDDESTINO##";

        /// <summary>
        /// Constante para codificar el origen del ID de la Suscripcion
        /// </summary>
        public const string ID_SUSCRIPCION = "##ID_SUSCRIPCION##";


        /// <summary>
        /// Constante para codificar el origen del recurso al que se le asocia la Suscripcion
        /// </summary>
        public const string ID_SUSCRIPCION_RECURSO = "##ID_SUSCRIPCION_RECURSO##";

        /// <summary>
        /// Constante para codificar el origen del ID destino de la Suscripcion
        /// </summary>
        public const string ID_SUSCRIPCION_PERFIL = "##ID_SUSCRIPCION_PERFIL##";

        /// <summary>
        /// Constante para codificar el origen del ID del mensaje
        /// </summary>
        public const string ID_MENSAJE = "##ID_MENSAJE##";

        /// <summary>
        /// Constante para codificar el origen del ID de la identidad que envia el mensaje
        /// </summary>
        public const string ID_MENSAJE_FROM = "##ID_MENSAJE_FROM##";

        /// <summary>
        /// Constante para codificar el origen del ID de la identidad que responde el mensaje
        /// </summary>
        public const string IDS_MENSAJE_TO = "##IDS_MENSAJE_TO##";

        /// <summary>
        /// Constante para codificar el origen del ID de la identidad que responde el mensaje
        /// </summary>
        public const string ID_MENSAJE_ORIGEN = "##ID_MENSAJE_ORIGEN##";

        /// <summary>
        /// Constante para codificar el origen del ID si es proyecto o usuario
        /// </summary>
        public const string COM_O_US = "##COM-US##";

        /// <summary>
        /// Clave para el tag de nombre de proyecto
        /// </summary>
        public const string NOMBRE_PROY = "##NOM_PROY##";

        /// <summary>
        /// Clave para el tag de Categoría de proyecto
        /// </summary>
        public const string CAT_PROY = "##CAT_PROY##";

        /// <summary>
        ///Constante para codificar los tags de tipo ID-Tag de proyecto
        /// </summary>
        public const string ID_TAG_PROY = "##ID_TAG_PROY##";

        /// <summary>
        ///Constante para codificar los tags de tipo ID-Tag de pagina del CMS
        /// </summary>
        public const string ID_TAG_PAGINA_CMS = "##ID_TAG_PAGINA_CMS##";

        /// <summary>
        /// Constante para codificar el origen del ID del contacto
        /// </summary>
        public const string ID_CONTACTO = "##ID_CONTACTO##";

        /// <summary>
        /// Constante para codificar el origen del ID de la persona que hace un contacto
        /// </summary>
        public const string ID_IDENTIDAD = "##ID_IDENTIDAD##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string NOMBRE_PER_COMPLETO = "##NOM_PER##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string NOMBRE_PER_SIN_APP = "##NSA_PER##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string APELLIDOS_PER = "##APP_PER##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string PAIS_PER = "##PAIS_PER##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string PROVINCIA_PER = "##PROV_PER##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string COMUNIDAD_PER = "##COM_PER##";

        /// <summary>
        ///Constante para codificar el origen del tag, de persona o de organización
        /// </summary>
        public const string PERS_U_ORG = "##PERS-ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo nombre de una organización
        /// </summary>
        public const string NOMBRE_ORG = "##NOM_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo url de una organización
        /// </summary>
        public const string URL_ORG = "##URL_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo:  tipo de una organización
        /// </summary>
        public const string TIPO_ORG = "##TIPO_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo sector de una organización
        /// </summary>
        public const string SEC_ORG = "##SEC_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string PAIS_ORG = "##PAIS_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string PROVINCIA_ORG = "##PROV_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string COMUNIDAD_ORG = "##COM_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string ALIAS_ORG = "##ALIAS_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string TAG_DESCOMPUESTO = "##DESC##";

        /// <summary>
        ///Constante para codificar los tags de tipo ID-Tag de persona
        /// </summary>
        public const string ID_TAG_PER = "##ID_TAG_PER##";

        /// <summary>
        /// Constante para codificar el origen del ID del comentario
        /// </summary>
        public const string ID_COMENTARIO = "##ID_COMENTARIO##";
        /// <summary>
        /// Constante para codificar el origen del ID del perfil comentario
        /// </summary>
        public const string IDS_COMENTARIO_PERFIL = "##IDS_COMENTARIO_PERFIL##";


        public const string PRIVACIDAD_CAMBIADA = "Privacidad cambiada";

        public const string EDITOR_ELIMINADO = "##ID_EDITOR_ELIMINADO##";

        public const string GRUPO_EDITORES_ELIMINADO = "##ID_GRUPO_EDITOR_ELIMINADO##";
    }


}
