using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.Identidad
{
    /// <summary>
    /// Representa el perfil de un usuario
    /// </summary>
    public class Perfil : ElementoGnoss
    {
        #region Miembros

        object mFilaRelacionPerfil;

        private Organizacion mOrganizacionPerfil;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pFilaPerfil">Fila del perfil</param>
        /// <param name="pGestorIdentidades">Gestor de identidades</param>
        public Perfil(AD.EntityModel.Models.IdentidadDS.Perfil pFilaPerfil, GestionIdentidades pGestorIdentidades)
            : base(pFilaPerfil, pGestorIdentidades)
        {
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la organización del perfil (null si es un perfil personal)
        /// </summary>
        public Organizacion OrganizacionPerfil
        {
            get
            {
                if (mOrganizacionPerfil == null)
                {
                    if ((OrganizacionID != null) && (GestorIdentidades.GestorOrganizaciones != null))
                    {
                        if (GestorIdentidades.GestorOrganizaciones.ListaOrganizaciones.ContainsKey(OrganizacionID.Value))
                        {
                            mOrganizacionPerfil = GestorIdentidades.GestorOrganizaciones.ListaOrganizaciones[OrganizacionID.Value];
                        }
                    }
                }

                return mOrganizacionPerfil;
            }
        }

        /// <summary>
        /// Obtiene el email para este perfil 
        /// </summary>
        public string Email
        {
            get
            {
                string email = "";
                if (this.IdentidadMyGNOSS.EsIdentidadProfesor)
                {
                    List<AD.EntityModel.Models.IdentidadDS.Profesor> filasProfesor = this.FilaPerfil.Profesor.ToList();

                    if ((filasProfesor != null) && (filasProfesor.Count > 0))
                    {
                        email = filasProfesor.First().Email;
                    }
                }
                else if (PersonaPerfil != null)
                {
                    email = PersonaPerfil.Email;
                }

                return email;
            }
        }

        /// <summary>
        /// Obtiene la persona relacionada con el perfil
        /// </summary>
        public Persona PersonaPerfil
        {
            get
            {
                if ((PersonaID != null) && (GestorIdentidades.GestorPersonas != null))
                {
                    if (GestorIdentidades.GestorPersonas.ListaPersonas.ContainsKey(PersonaID.Value))
                    {
                        return GestorIdentidades.GestorPersonas.ListaPersonas[PersonaID.Value];
                    }
                }

                return null;
            }
        }
        
        /// <summary>
        /// Obtiene la lista de elementos hijos del perfil
        /// </summary>
        public override List<IElementoGnoss> Hijos
        {
            get
            {
                if (mHijos == null)
                    mHijos = new List<IElementoGnoss>();
                return mHijos;
            }
        }

        /// <summary>
        /// Devuelve la identidad de MyGNOSS del perfil actual
        /// </summary>
        public Identidad IdentidadMyGNOSS
        {
            get
            {
                foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad in FilaPerfil.Identidad)
                {
                    if (filaIdentidad.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && filaIdentidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && GestorIdentidades.ListaIdentidades.ContainsKey(filaIdentidad.IdentidadID))
                    {
                        return GestorIdentidades.ListaIdentidades[filaIdentidad.IdentidadID];
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Obtiene el nombre del perfil con el que se relaciona la identidad
        /// </summary>
        public string NombrePerfil
        {
            get
            {
                if (PersonaID.HasValue && (!OrganizacionID.HasValue || IdentidadMyGNOSS.EsIdentidadProfesor))
                {
                    if (IdentidadMyGNOSS.EsIdentidadProfesor)
                    {
                        return IdentidadMyGNOSS.NombreCorto;
                    }
                    else
                    {
                        return "Personal";
                    }
                }
                else
                    return NombreOrganizacion;
            }
        }

        /// <summary>
        /// Obtiene la clave de la identidad
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaPerfil.PerfilID;
            }
        }

        /// <summary>
        /// Obtiene el gestor de identidades
        /// </summary>
        public GestionIdentidades GestorIdentidades
        {
            get
            {
                return (GestionIdentidades)GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene la fila del perfil
        /// </summary>
        public AD.EntityModel.Models.IdentidadDS.Perfil FilaPerfil
        {
            get
            {
                return (AD.EntityModel.Models.IdentidadDS.Perfil)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Obtiene la fila de relación de la identidad con la persona o con la persona en una orgnización 
        /// Puede ser una fila PerfilPersona o PerfilPersonaOrg
        /// </summary>
        public object FilaRelacionPerfil
        {
            get
            {
                if (mFilaRelacionPerfil == null)
                {//"PerfilID = '" + FilaPerfil.PerfilID + "'"
                    object perfilPersona = GestorIdentidades.DataWrapperIdentidad.ListaPerfilPersona.Find(perfilPerson => perfilPerson.PerfilID.Equals(FilaPerfil.PerfilID));
                    if (perfilPersona != null)
                    {
                        //Identidad trabajando en modo personal (fila de CVIdentidad)
                        mFilaRelacionPerfil = perfilPersona;
                    }
                    else
                    {//"PerfilID = '" + FilaPerfil.PerfilID + "'"
                        //Identidad trabajando en modo profesional
                        perfilPersona = GestorIdentidades.DataWrapperIdentidad.ListaPerfilPersonaOrg.FirstOrDefault(perfilPersonaOrg => perfilPersonaOrg.PerfilID.Equals(FilaPerfil.PerfilID));

                        if (perfilPersona != null)
                            mFilaRelacionPerfil = perfilPersona;
                        else
                        {
                            perfilPersona = GestorIdentidades.DataWrapperIdentidad.ListaPerfilOrganizacion.FirstOrDefault(perfilOrg => perfilOrg.PerfilID.Equals(FilaPerfil.PerfilID));
                            if (perfilPersona != null)
                                mFilaRelacionPerfil = perfilPersona;
                        }
                    }
                }
                return mFilaRelacionPerfil;
            }
        }

        /// <summary>
        /// Obtiene el nombre de una persona que trabaja con una organización en modo PERSONAL
        /// </summary>
        public string NombrePersonaEnOrganizacion
        {
            get
            {
                return FilaPerfil.NombrePerfil;
            }
        }

        /// <summary>
        /// Obtiene el nombre de la organización (en caso de que la identidad no sea personal)
        /// </summary>
        public string NombreOrganizacion
        {
            get
            {
                if (FilaPerfil.NombreOrganizacion != null)
                    return FilaPerfil.NombreOrganizacion;

                return string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el nombre corto de la organización (nombre que se utiliza en la url)
        /// </summary>
        public string NombreCortoOrg
        {
            get
            {
                if (FilaPerfil.NombreCortoOrg == null)
                    return string.Empty;

                return FilaPerfil.NombreCortoOrg;
            }
        }

        /// <summary>
        /// Obtiene el nombre corto del usuario (nombre que se utiliza en la url)
        /// </summary>
        public string NombreCortoUsu
        {
            get
            {
                if (FilaPerfil.NombreCortoUsu == null)
                    return string.Empty;

                return FilaPerfil.NombreCortoUsu;
            }
        }

        /// <summary>
        /// Obtiene el identificador de la organización (en caso de que la identidad no sea personal)
        /// </summary>
        public Guid? OrganizacionID
        {
            get
            {
                if (FilaPerfil.OrganizacionID.HasValue)
                    return FilaPerfil.OrganizacionID.Value;

                return null;
            }
        }

        /// <summary>
        /// Obtiene el identificador de la persona (en caso de que la identidad no sea de organizacion)
        /// </summary>
        public Guid? PersonaID
        {
            get
            {
                if (FilaPerfil.PersonaID.HasValue)
                    return FilaPerfil.PersonaID.Value;

                return null;
            }
        }

        ///// <summary>
        ///// Obtiene o establece si el perfil tiene cuenta en Twitter
        ///// </summary>
        //public bool TieneTwitter
        //{
        //    get
        //    {
        //        return FilaPerfil.TieneTwitter;
        //    }
        //    set
        //    {
        //        FilaPerfil.TieneTwitter = value;
        //    }
        //}

        ///// <summary>
        ///// Obtiene o establece el nombre del usuario de Twitter para el perfil
        ///// </summary>
        //public string UsuarioTwitter
        //{
        //    get
        //    {
        //        if (!FilaPerfil.IsUsuarioTwitterNull())
        //            return FilaPerfil.UsuarioTwitter;
        //        return string.Empty;
        //    }
        //    set
        //    {
        //        FilaPerfil.UsuarioTwitter = value;
        //    }
        //}

        ///// <summary>
        ///// Obtiene o establece el token de acceso a Twitter para el perfil
        ///// </summary>
        //public string TokenTwitter
        //{
        //    get
        //    {
        //        if (!FilaPerfil.IsTokenTwitterNull())
        //            return FilaPerfil.TokenTwitter;
        //        return string.Empty;
        //    }
        //    set
        //    {
        //        FilaPerfil.TokenTwitter = value;
        //    }
        //}

        ///// <summary>
        ///// Obtiene o establece el token secreto de acceso a Twitter para el perfil
        ///// </summary>
        //public string TokenSecretoTwitter
        //{
        //    get
        //    {
        //        if (!FilaPerfil.IsTokenSecretoTwitterNull())
        //            return FilaPerfil.TokenSecretoTwitter;
        //        return string.Empty;
        //    }
        //    set
        //    {
        //        FilaPerfil.TokenSecretoTwitter = value;
        //    }
        //}

        #endregion
    }
}
