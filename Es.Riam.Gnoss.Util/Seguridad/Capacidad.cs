namespace Es.Riam.Gnoss.Util.Seguridad
{
    /// <summary>
	/// Clase donde se enumeran todas las capacidades del sistema
	/// </summary>
	public class Capacidad
    {
        #region Capacidades para Rol general
        /// <summary>
        /// Capacidades de rol General 
        /// </summary>
        public class General
        {
            /// <summary>
            /// Enumeración de las capacidades generales sobre personas
            /// </summary>
            public enum CapacidadesPersonas : ulong
            {
                /// <summary>
                /// Editar TODAS personas
                /// </summary>
                EditarTODASpersonas = 0x1,

                /// <summary>
                /// ver TODAS personas
                /// </summary>
                VerTODASpersonas = 0x2
            }

            /// <summary>
            /// Enumeración de las capacidades generales sobre organizaciones
            /// </summary>
            public enum CapacidadesOrganizacion : ulong
            {
                /// <summary>
                /// Editar TODAS organizaciones
                /// </summary>
                EditarTODASorganizaciones = 0x4
            }

            /// <summary>
            /// Enumeración de las capacidades generales sobre proyectos
            /// </summary>
            public enum CapacidadesProyectos : ulong
            {
                /// <summary>
                /// Editar TODOS proyectos
                /// </summary>
                EditarTODOSproyectos = 0x10,

                /// <summary>
                /// ver TODOS proyectos
                /// </summary>
                VerTODOSproyectos = 0x20
            }

            /// <summary>
            /// Enumeración de las capacidades generales sobre las opciones del sistema
            /// </summary>
            public enum CapacidadesOpcionesSistema : ulong
            {
                /// <summary>
                /// Editar las opciones generales del sistema global
                /// </summary>
                EditarOpcionesSistema = 0x1000,

                /// <summary>
                /// Ver las opciones generales del sistema global
                /// </summary>
                VerOpcionesSistema = 0x2000
            }

            /// <summary>
            /// Enumeración de las capacidades generales sobre administracion
            /// </summary>
            public enum CapacidadesAdministrador : ulong
            {
                /// <summary>
                /// Administrador del sistema a nivel general
                /// </summary>
                AdministrarGeneral = 0x4000
            }
        }
        #endregion

        #region Capacidades para Rol de Proyecto
        /// <summary>
        /// Capacidades de rol de proyecto de Usuario y GrupoUsuario
        /// </summary>
        public class Proyecto
        {
            /// <summary>
            /// Enumeración de las capacidades de Proyectos
            /// </summary>
            public enum CapacidadesParametrosGenerales : ulong
            {
                /// <summary>
                /// Ver parámetros proyecto
                /// </summary>
                VerParametrosProyecto = 0x1,

                /// <summary>
                /// Editar parámetros proyecto
                /// </summary>
                EditarParametrosProyecto = 0x2

            }

            /// <summary>
            /// Enumeración de las capacidades de Estructura
            /// </summary>
            public enum CapacidadesEstructura : ulong
            {
                /// <summary>
                /// Ver estructura
                /// </summary>
                VerEstructura = 0x10,

                /// <summary>
                /// Editar estructura
                /// </summary>
                EditarEstructura = 0x20,

                /// <summary>
                /// Aprobar normas de estructura
                /// </summary>
                AprobarNormasEstructura = 0x1000000
            }

            /// <summary>
            /// Enumeración de las capacidades de Libro
            /// </summary>
            public enum CapacidadesLibro : ulong
            {
                /// <summary>
                /// Ver competencias
                /// </summary>
                VerCompetencias = 0x100,

                /// <summary>
                /// Editar las competenciasdel libro
                /// </summary>
                EditarCompetencias = 0x200,

                /// <summary>
                /// Ver los parámetros de libro
                /// </summary>
                VerConfiguracionLibro = 0x800000000,

                /// <summary>
                /// Editar los parámetros de libro
                /// </summary>
                EditarConfiguracionLibro = 0x1000000000,

                /// <summary>
                /// Aprobar las propuestas de mejora de modos y escalas de meta
                /// </summary>
                AprobarNormaLibro = 0x80000000000,

                /// <summary>
                /// Aprobar las propuestas de mejora competencias
                /// </summary>
                AprobarNormaCompetencias = 0x8000000000000
            }

            /// <summary>
            /// Enumeración de las capacidades de puesto de trabajo
            /// </summary>
            public enum CapacidadesPuestoTrabajo : ulong
            {
                /// <summary>
                /// Ver puesto de trabajo
                /// </summary>
                VerPuestoTrabajo = 0x400,

                /// <summary>
                /// Editar puesto de trabajo
                /// </summary>
                EditarPuestoTrabajo = 0x800,


                /// <summary>
                /// Aprobar mejora de puestos de trabajo
                /// </summary>
                AprobarNormasPuestoTrabajo = 0x80
            }


            /// <summary>
            /// Enumeración de las capacidades de grupos funcionales
            /// </summary>
            public enum CapacidadesGrupoFuncional : ulong
            {
                /// <summary>
                /// Ver Grupo Funcional
                /// </summary>
                VerGF = 0x10000,

                /// <summary>
                /// Editar Grupo Funcional
                /// </summary>
                EditarGF = 0x20000,

                /// <summary>
                /// Editar todos los Grupo Funcional
                /// </summary>
                Editar_TODOS_GF = 0x200000000000,

                /// <summary>
                /// Editar todos los Grupo Funcional
                /// </summary>
                Ver_TODOS_GF = 0x2000000000000,

                /// <summary>
                /// Aprobar normas de GF
                /// </summary>
                AprobarNormasGF = 0x400000000000,

                /// <summary>
                /// Edita los parametros de grupos funcionales
                /// </summary>
                EditarConfiguracionGF = 0x800000000000,

                /// <summary>
                /// Ver los parametros de grupos funcionales
                /// </summary>
                VerConfiguracionGF = 0x1000000000000,

                /// <summary>
                /// Aprobar fase de analisis de grupos funcionales
                /// </summary>
                AprobarFaseAnalisisGF = 0x4000000000000

            }

            /// <summary>
            /// Enumeración de las capacidades de Objetivos
            /// </summary>
            public enum CapacidadesObjetivo : ulong
            {
                /// <summary>
                /// Ver Objetivo
                /// </summary>
                VerObjetivo = 0x40000,

                /// <summary>
                /// Editar Objetivo
                /// </summary>
                EditarObjetivo = 0x80000,

                /// <summary>
                /// Editar todos los objetivos
                /// </summary>
                Editar_TODOS_Objetivo = 0x10000000,

                /// <summary>
                /// ver todos los objetivos
                /// </summary>
                Ver_TODOS_Objetivo = 0x100000000,

                /// <summary>
                /// Aprobar normas de objetivos
                /// </summary>
                AprobarNormasDeObjetivos = 0x20000000000,

                /// <summary>
                /// Ver los parámetros de objetivos
                /// </summary>
                VerConfiguracionObjetivos = 0x2000000000,

                /// <summary>
                /// Editar los parámetros de objetivos
                /// </summary>
                EditarConfiguracionObjetivos = 0x4000000000,

                /// <summary>
                /// Aprobar fase de analisis de objetivos
                /// </summary>
                AprobarFaseAnalisisObjetivos = 0x400000000

            }

            /// <summary>
            /// Enumeración de las capacidades de Procesos
            /// </summary>
            public enum CapacidadesProceso : ulong
            {
                /// <summary>
                /// Ver Proceso
                /// </summary>
                VerProceso = 0x100000,

                /// <summary>
                /// Editar Proceso
                /// </summary>
                EditarProceso = 0x200000,

                /// <summary>
                /// Editar todos los procesos
                /// </summary>
                Editar_TODOS_Proceso = 0x20000000,

                /// <summary>
                /// Ver todos los procesos
                /// </summary>
                Ver_TODOS_Proceso = 0x200000000,

                /// <summary>
                /// Aprobar las propuestas de mejora
                /// </summary>
                AprobarNormasDeProceso = 0x40000000000,

                /// <summary>
                /// Editar metas desde procesos
                /// </summary>
                EditarMetasDesdeProceso = 0x100000000000,

                /// <summary>
                /// Revisar procesos publicados
                /// </summary>
                RevisarProceso = 0x1000,

                /// <summary>
                /// Ver los parámetros de procesos
                /// </summary>
                VerConfiguracionProcesos = 0x8000000000,

                /// <summary>
                /// Editar los parámetros de procesos
                /// </summary>
                EditarConfiguracionProcesos = 0x10000000000,

                /// <summary>
                /// Aprobar fase de analisis de procesos
                /// </summary>
                AprobarFaseAnalisisProcesos = 0x80000000

            }

            /// <summary>
            /// Enumeración de las capacidades de Wiki
            /// </summary>
            public enum CapacidadesWiki : ulong
            {
                /// <summary>
                /// Supervisar Wiki
                /// </summary>
                SupervisarWiki = 0x40000000
            }

            /// <summary>
            /// Enumeración de las capacidades de Dafo
            /// </summary>
            public enum CapacidadesDafo : ulong
            {
                /// <summary>
                /// Editar DafoProyecto
                /// </summary>
                EditarDafo = 0x400000
            }

            /// <summary>
            /// Enumeración de las capacidades de Foro
            /// </summary>
            public enum CapacidadesForo : ulong
            {
                /// <summary>
                /// Supervisor Foro
                /// </summary>
                SupervisorForo = 0x40
            }

            /// <summary>
            /// Enumeración de las capacidades de Cuestionario
            /// </summary>
            public enum CapacidadesCuestionario : ulong
            {
                /// <summary>
                /// Editar Cuestionario
                /// </summary>
                EditarCuestionario = 0x2000000
            }

            /// <summary>
            /// Enumeración de las capacidades del tesauro
            /// </summary>
            public enum CapacidadesTesauro : ulong
            {
                /// <summary>
                /// Editar tesauro
                /// </summary>
                EditarTesauro = 0x4000000
            }

            /// <summary>
            /// Enumeración de las capacidades generales del proyecto
            /// </summary>
            public enum CapacidadesGenerales : ulong
            {
                /// <summary>
                /// Importar proyecto
                /// </summary>
                Importar = 0x8000000,

                /// <summary>
                /// Exportar proyecto
                /// </summary>
                Exportar = 0x10000000000000,

                /// <summary>
                /// Editar tesauro
                /// </summary>
                ImprimirInformes = 0x20000000000000,

                /// <summary>
                /// Usuario diseñador, puede entrar y modificar el diseño de la comunidad
                /// </summary>
                Diseniador = 0x2000

            }

            /// <summary>
            /// Enumeración de las capacidades del administrador del proyecto
            /// </summary>
            public enum CapacidadesAdministrador : ulong
            {
                /// <summary>
                /// Administrar proyecto
                /// </summary>
                AdministrarProyecto = 0x4
            }

            /// <summary>
            /// Enumeración de las capacidades de documentacion
            /// </summary>
            public enum CapacidadesDocumentacion : ulong
            {
                /// <summary>
                /// Supervisar documentos
                /// </summary>
                SupervisarDocumentos = 0x8
            }
        }
        #endregion

        #region Capacidades para Rol de Organizacion
        /// <summary>
        /// Capacidades de rol de organizacion 
        /// </summary>
        public class Organizacion
        {
            /// <summary>
            /// Enumeración de las capacidades de una organizacion sobre sus personas
            /// </summary>
            public enum CapacidadesPersonas : ulong
            {
                /// <summary>
                /// Editar personas de la organizaciónu y vincular nuevas
                /// </summary>
                EditarPersonas = 0x1

                ///// <summary>
                ///// ver TODAS personas
                ///// </summary>
                //VerTODASpersonas = 0x2
            }

            /// <summary>
            /// Enumeración de las capacidades de una organización  sobre proyectos
            /// </summary>
            public enum CapacidadesProyectos : ulong
            {
                /// <summary>
                /// Editar/Crear proyectos
                /// </summary>
                EditarProyectos = 0x10,

                /// <summary>
                /// ver proyectos
                /// </summary>
                VerProyectos = 0x20
            }

            /// <summary>
            /// Enumeración de las capacidades de una organizacion sobre sus propiedades
            /// </summary>
            public enum CapacidadesPropiedades : ulong
            {
                /// <summary>
                /// Editar la organización (es lo mismo que ser el Editor de la organización)
                /// </summary>
                EditarOrganizacion = 0x100
            }

            /// <summary>
            /// Enumeración de las capacidades de una organizacion sobre administracion
            /// </summary>
            public enum CapacidadesAdministrador : ulong
            {
                /// <summary>
                /// Administrador del sistema a nivel general
                /// </summary>
                AdministrarOrganizacion = 0x1000
            }

            /// <summary>
            /// Enumeración de las capacidades de un usuario de una organización  sobre los comentarios
            /// </summary>
            public enum CapacidadesComentarios : ulong
            {
                /// <summary>
                /// Permitir realizar comentarios en blogs,otros recursos, etc..
                /// </summary>
                EditarComentarios = 0x10000
            }
        }
        #endregion
    }
}
