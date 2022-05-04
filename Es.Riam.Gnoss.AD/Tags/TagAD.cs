namespace Es.Riam.Gnoss.AD.Tags
{
    /// <summary>
    /// Tipos de tags
    /// </summary>
    public enum TiposTags
    {
        /// <summary>
        /// Tipo manual, para todos los tags manuales
        /// </summary>
        Manual = 0,

        #region TiposTags Documentos

        /// <summary>
        /// Tag de título de documento
        /// </summary>
        DocumentoTitulo = 1,
        /// <summary>
        /// Tag de nombre de entidad vinculada a un documento
        /// </summary>
        DocumentoNombreEntidadVinculada = 2,
        /// <summary>
        /// Tag de nombre de documento
        /// </summary>
        DocumentoNombre = 3,
        /// <summary>
        /// Tag de categoría vinculada a un documento
        /// </summary>
        CategoriaDocumento = 4,

        #endregion

        //PersonaProvincia = 6,     //NO SE USA
        //PersonaPais = 7,         // NO SE USA
        //PersonaOrganizacion = 10,  //NO SE USA

        #region TiposTags Personas

        /// <summary>
        /// Tag de nombre de persona
        /// </summary>
        PersonaNombreCompleto = 5,
        /// <summary>
        /// Tag de cargo de persona
        /// </summary>
        PersonaCargo = 11,
        /// <summary>
        /// Tag de profesión de persona
        /// </summary>
        Profesion = 32,

        #endregion

        #region TiposTags Curriculum

        /// <summary>
        /// Tag de idioma de persona
        /// </summary>
        PersonaIdioma = 8,
        /// <summary>
        /// Tag de titulación de persona
        /// </summary>
        PersonaTitulacion = 9,
        /// <summary>
        /// Tag de proyecto fin de carrera en currículum
        /// </summary>
        ProyectoFC = 12,
        /// <summary>
        /// Tag de proyecto en currículum
        /// </summary>
        Proyecto = 13,
        /// <summary>
        /// Tag de tésis en currículum
        /// </summary>
        Tesis = 14,
        /// <summary>
        /// Tag de puesto docente en currículum
        /// </summary>
        PuestoDocente = 15,
        /// <summary>
        /// Tag de centro docente en currículum
        /// </summary>
        CentroDocente = 16,
        /// <summary>
        /// Tag de ponencia en currículum
        /// </summary>
        Ponencia = 17,
        /// <summary>
        /// Tag de dentro de estudios en currículum
        /// </summary>
        CentroEstudios = 18,
        /// <summary>
        /// Tag de libro en currículum
        /// </summary>
        Libro = 19,
        /// <summary>
        /// Tag de artículo en currículum
        /// </summary>
        Articulo = 20,
        /// <summary>
        /// Tag de puesto de trabajo en currículum
        /// </summary>
        PuestoTrabajo = 21,
        /// <summary>
        /// Tag de empresa de trabajo en currículum
        /// </summary>
        EmpresaTrabajo = 22,
        /// <summary>
        /// Tag de título de currículum
        /// </summary>
        CurriculumTitulo = 28,

        #endregion

        #region TiposTags Competencia

        /// <summary>
        /// Tag de la parte de acción del nombre de la competencia
        /// </summary>
        CompetenciaNombreAccion = 23,
        /// <summary>
        /// Tag de la parte del concepto del nombre de la competencia
        /// </summary>
        CompetenciaNombreConcepto = 24,

        #endregion

        #region TiposTags Dimension

        /// <summary>
        /// Tag de la parte de acción del nombre de la dimensión
        /// </summary>
        DimensionNombreAccion = 25,
        /// <summary>
        /// Tag de la parte del concepto del nombre de la dimensión
        /// </summary>
        DimensionNombreConcepto = 26,

        #endregion

        #region TiposTags Proyecto

        /// <summary>
        /// Tag de nombre de proyecto
        /// </summary>
        ProyectoNombre = 27,

        #endregion

        #region TiposTags Organizacion

        /// <summary>
        /// Tag de nombre de organización
        /// </summary>
        OrganizacionNombre = 29,
        /// <summary>
        /// Tag de url de organización
        /// </summary>
        OrganizacionURL = 30,
        /// <summary>
        /// Tag de tipo de organización
        /// </summary>
        OrganizacionTipo = 31,
        /// <summary>
        /// Tag de sector de organización
        /// </summary>
        OrganizacionSector = 33,

        #endregion

        #region TiposTags filtros documentos 38..55

        /// <summary>
        /// Tag de categoría vinculada a un documento
        /// </summary>
        CategoriaTesauro = 38,

        /// <summary>
        /// Tag de categoría vinculada a un documento
        /// </summary>
        ExtensionDocumento = 39,

        /// <summary>
        /// Tag de categoría vinculada a un documento
        /// </summary>
        PublicadorDocumento = 40,

        /// <summary>
        /// Tag de categoría vinculada a un documento
        /// </summary>
        TagRecursoDescompuesto = 41,

        /// <summary>
        /// Tag de categoría vinculada a un documento
        /// </summary>
        FechaPublicacionDocumento = 42,

        /// <summary>
        /// Tag de categoría vinculada a un documento
        /// </summary>
        AutorDocumento = 43,

        /// <summary>
        /// Tag de categoría vinculada a un documento
        /// </summary>
        NivelCertificacionDocumento = 44,

        /// <summary>
        /// Tag de categoría vinculada a un documento
        /// </summary>
        TipoDocumento = 45,

        /// <summary>
        /// Tag de categoría vinculada a un documento
        /// </summary>
        EstadoComentado = 46,

        sameAs = 47,

        #endregion

        #region TiposTags filtros personas 56..69

        /// <summary>
        /// Tag para las comunidades en las que participa una persona
        /// </summary>
        PersonaParticipaComunidad = 56,

        /// <summary>
        /// Tag para el país de una persona
        /// </summary>
        PersonaPais = 57,

        /// <summary>
        /// Tag para la provincia de una persona
        /// </summary>
        PersonaProvincia = 58,

        /// <summary>
        /// Tag para el código postal de una persona
        /// </summary>
        PersonaCP = 59,

        /// <summary>
        /// Tag para la localidad de una persona
        /// </summary>
        PersonaLocalidad = 60,
        /// <summary>
        /// Tags para la descomposición del CV
        /// </summary>
        TagCVDescompuesto = 61,
        /// <summary>
        /// Tag para el nombre de una persona (nombre sin apellidos)
        /// </summary>
        PersonaNombreSinApellidos = 62,
        /// <summary>
        /// Tag para los apellidos de una persona
        /// </summary>
        PersonaApellidos = 63,

        #endregion

        #region TiposTags filtros organizaciones 70..85

        /// <summary>
        /// Tag para las comunidades en las que participa una organización
        /// </summary>
        OrganizacionParticipaComunidad = 70,

        /// <summary>
        /// Tag para el país de una organización
        /// </summary>
        OrganizacionPais = 71,

        /// <summary>
        /// Tag para la provincia de una organización
        /// </summary>
        OrganizacionProvincia = 72,

        /// <summary>
        /// Tag para el código postal de una organización
        /// </summary>
        OrganizacionCP = 73,

        /// <summary>
        /// Tag para la localidad de una organización
        /// </summary>
        OrganizacionLocalidad = 74,

        /// <summary>
        /// Tag para la diracción de una organización
        /// </summary>
        OrganizacionDireccion = 75,

        /// <summary>
        /// Tag para el número de empleados de una organización
        /// </summary>
        OrganizacionNumeroEmpleados = 76,
        /// <summary>
        /// Tag para el alias de una organización
        /// </summary>
        OrganizacionAlias = 77,
        /// <summary>
        /// Tag de organización o persona
        /// </summary>
        OrganizacionOPersona = 78,


        /// <summary>
        /// Tag de comentario o recurso
        /// </summary>
        ComentarioORecurso = 79,

        #endregion

        #region TiposTags Proceso 86..95

        /// <summary>
        /// Tag de la parte de acción del nombre del Proceso
        /// </summary>
        ProcesoNombreAccion = 86,
        /// <summary>
        /// Tag de la parte del concepto del nombre del  la competencia
        /// </summary>
        ProcesoNombreConcepto = 87,
        /// <summary>
        /// Tag automatico del nombre completo del Proceso
        /// </summary>
        ProcesoNombreCompleto = 88,

        #endregion

        #region TiposTags Objetivo 96..105

        /// <summary>
        /// Tag de la parte de acción del nombre del Objetivo
        /// </summary>
        ObjetivoNombreAccion = 96,
        /// <summary>
        /// Tag de la parte del concepto del nombre del Objetivo
        /// </summary>
        ObjetivoNombreConcepto = 97,

        /// <summary>
        /// Tag automatico del nombre completo del objetivo
        /// </summary>
        ObjetivoNombreCompleto = 98,

        #endregion

        #region TiposTags filtros de Proyecto 151..160

        /// <summary>
        /// Tag de comunidad descompuesto
        /// </summary>
        TagProyectoDescompuesto = 151,

        #endregion

        #region ID-Tags 900...950

        /// <summary>
        /// ID-Tag de documento.
        /// </summary>
        IDTagDoc = 900,
        /// <summary>
        /// ID-Tag de persona.
        /// </summary>
        IDTagPer = 901,
        /// <summary>
        /// ID-Tag de proyecto
        /// </summary>
        IDTagProy = 904,
        /// <summary>
        /// ID-Tag de contacto
        /// </summary>
        IDTagContacto = 905,
        /// <summary>
        /// ID-Tag de identidad
        /// </summary>
        IDTagIdentidad = 906,


         /// <summary>
        /// ID-Tag de comentarios
        /// </summary>
        IDTagMensaje = 997,

         /// <summary>
        /// ID-Tag de comentarios
        /// </summary>
        IDTagMensajeFrom = 998,

         /// <summary>
        /// ID-Tag de comentarios
        /// </summary>
        IDTagMensajeTo = 999,       


        /// <summary>
        /// ID-Tag de comentarios
        /// </summary>
        IDTagComentario = 1000,

        /// <summary>
        /// ID-Tag de comentarios
        /// </summary>
        IDsTagComentarioPerfil = 1001,
      

        /// <summary>
        /// ID-Tag de Invitacion
        /// </summary>
        IDTagInvitacion = 1003,

        /// <summary>
        /// ID-Tag de la identidad destino de la invitación
        /// </summary>
        IDTagInvitacionIdDestino = 1004,

        /// <summary>
        /// ID-Tag de Suscripcion
        /// </summary>
        IDTagSuscripcion = 1005,

        /// <summary>
        /// ID-Tag del perfil Suscripcion
        /// </summary>
        IDTagSuscripcionPerfil = 1006,

        /// <summary>
        /// ID-Tag de identidad Suscripcion
        /// </summary>
        IDTagSuscripcionRecurso = 1007,

        /// <summary>
        /// ID de la pestanya
        /// </summary>
        IDPestanyaCMSProyecto = 1008,

        #endregion

        #region Tag Suma Filtros ID-Tags 10000, límite para filtros

        /// <summary>
        /// ID-Tag.
        /// </summary>
        SumaFiltroIDTag = 10000

        #endregion
    }    
}
