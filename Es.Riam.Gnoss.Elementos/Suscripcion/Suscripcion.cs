using Es.Riam.Gnoss.AD.Suscripcion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.Suscripcion
{
    /// <summary>
    /// Clase suscripción
    /// </summary>
    public class Suscripcion : ElementoGnoss
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public Suscripcion(LoggingService loggingService)
            :base(loggingService)
        {
        }

        /// <summary>
        /// Crea una suscripción a partir de la fila pasada como parámetro
        /// </summary>
        /// <param name="pFilaSuscripcion">Fila de la suscripción</param>
        public Suscripcion(AD.EntityModel.Models.Suscripcion.Suscripcion pFilaSuscripcion, LoggingService loggingService)
            : base(pFilaSuscripcion, loggingService)
        {
        }

        /// <summary>
        /// Crea una suscripción a partir de los parámetros pasados
        /// </summary>
        /// <param name="pFilaSuscripcion">Fila de la suscripción</param>
        /// <param name="pGestionSuscripcion">Gestor de suscripciones</param>
        public Suscripcion(AD.EntityModel.Models.Suscripcion.Suscripcion pFilaSuscripcion, GestionSuscripcion pGestionSuscripcion, LoggingService loggingService)
            : base(pFilaSuscripcion, pGestionSuscripcion, loggingService)
        {
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el gestor de suscripciones
        /// </summary>
        public GestionSuscripcion GestorSuscripcion
        {
            get
            {
                return (GestionSuscripcion)GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene la fila de la suscripción
        /// </summary>
        public AD.EntityModel.Models.Suscripcion.Suscripcion FilaSuscripcion
        {
            get
            {
                return (AD.EntityModel.Models.Suscripcion.Suscripcion)FilaElementoEntity;
            }
            set { }
        }

        /// <summary>
        /// Devuelve la fila de relacion que puede ser de SuscripcionBlog, de SuscripcionTesauroUsuario o de SuscripcionTesauroProyecto
        /// </summary>
        public object FilaRelacion
        {
            get
            {
                if (FilaSuscripcion.SuscripcionTesauroProyecto != null)
                {
                    return FilaSuscripcion.SuscripcionTesauroProyecto;
                }
                else if (FilaSuscripcion.SuscripcionTesauroUsuario != null)
                {
                    return FilaSuscripcion.SuscripcionTesauroUsuario;
                }
                else if (FilaSuscripcion.SuscripcionTesauroOrganizacion != null)
                {
                    return FilaSuscripcion.SuscripcionTesauroOrganizacion;
                }
                else if (FilaSuscripcion.SuscripcionIdentidadProyecto != null)
                {
                    return FilaSuscripcion.SuscripcionIdentidadProyecto;
                }
                return null;
            }
            set { }
        }

        /// <summary>
        /// Devuelve las filas de las categorías vinculadas (si las hay) de la suscripción. Si la suscripcion no es de comunidad, o no tiene categorias devuelve null
        /// </summary>
        public List<AD.EntityModel.Models.Suscripcion.CategoriaTesVinSuscrip> FilasCategoriasVinculadas
        {
            get
            {
                if(FilaSuscripcion.CategoriaTesVinSuscrip.Count > 0)
                {
                    return FilaSuscripcion.CategoriaTesVinSuscrip.ToList();
                }
                return null;
            }
        }
        
        public List<AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto> FilaSuscripcionIdentidadProyecto
        {
            get
            {
                if (FilaSuscripcion.SuscripcionIdentidadProyecto.Count > 0)
                {
                    return FilaSuscripcion.SuscripcionIdentidadProyecto.ToList();
                }
                return null;
            }
            set { }
        }

        public AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario FilaSuscripcionTesauroUsuario
        {
            get
            {
                if (FilaSuscripcion.SuscripcionTesauroUsuario != null)
                {
                    return FilaSuscripcion.SuscripcionTesauroUsuario;
                }
                return null;
            }
        }

        public AD.EntityModel.Models.Suscripcion.SuscripcionTesauroOrganizacion FilaSuscripcionTesauroOrganizacion
        {
            get
            {
                if (FilaSuscripcion.SuscripcionTesauroOrganizacion != null)
                {
                    return FilaSuscripcion.SuscripcionTesauroOrganizacion;
                }
                return null;
            }
        }

        /// <summary>
        /// Devuelve el tipo de suscripcion segun la enumeracion TipoSuscripciones
        /// </summary>
        public TipoSuscripciones Tipo
        {
            get
            {
                if (FilaRelacion is AD.EntityModel.Models.Suscripcion.SuscripcionTesauroProyecto)
                {
                    return TipoSuscripciones.Comunidades;
                }
                else if (FilaRelacion is AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario || FilaRelacion is AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto)
                {
                    return TipoSuscripciones.Personas;
                }
                else if (FilaRelacion is AD.EntityModel.Models.Suscripcion.SuscripcionTesauroOrganizacion)
                {
                    return TipoSuscripciones.Organizacion;
                }
                else
                {
                    return TipoSuscripciones.Blogs;
                }
            }
        }

        /// <summary>
        /// Obtiene el proyecto de la suscripcion (si la suscripcion es de proyecto y el gestor de proyectos está cargado)
        /// </summary>
        public ServiciosGenerales.Proyecto proyectoSuscripcion
        {
            get
            {
                if (FilaRelacion is AD.EntityModel.Models.Suscripcion.SuscripcionTesauroProyecto)
                {
                    Guid ProyectoID = ((AD.EntityModel.Models.Suscripcion.SuscripcionTesauroProyecto)FilaRelacion).ProyectoID;
                    
                    if (GestorSuscripcion.GestorProyecto != null && GestorSuscripcion.GestorProyecto.ListaProyectos.ContainsKey(ProyectoID))
                    {
                        return GestorSuscripcion.GestorProyecto.ListaProyectos[ProyectoID];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene la identidad de la suscripcion (si la suscripcion es de usuario, la lista de identidadesUsuario y el gestor de identidades está cargado)
        /// </summary>
        public Identidad.Identidad identidadSuscripcion
        {
            get
            {
                if (FilaRelacion is AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario)
                {
                    Guid identidadID = ((AD.EntityModel.Models.Suscripcion.SuscripcionTesauroUsuario)FilaRelacion).UsuarioID;
                    
                    if (GestorSuscripcion != null && GestorSuscripcion.GestorIdentidades != null)
                    {
                        return GestorSuscripcion.GestorIdentidades.ListaIdentidades[identidadID];
                    }
                }
                if (FilaRelacion is AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto)
                {
                    Guid identidadID = ((AD.EntityModel.Models.Suscripcion.SuscripcionIdentidadProyecto)FilaRelacion).IdentidadID;

                    if (GestorSuscripcion != null && GestorSuscripcion.GestorIdentidades != null)
                    {
                        return GestorSuscripcion.GestorIdentidades.ListaIdentidades[identidadID];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene la clave de la suscripción
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaSuscripcion.SuscripcionID;
            }
        }

        #endregion
    }
}