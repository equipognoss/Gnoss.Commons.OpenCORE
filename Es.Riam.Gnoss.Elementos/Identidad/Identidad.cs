using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos.Model;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Suscripcion;
using Es.Riam.Gnoss.Logica.Amigos;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.Identidad
{
    /// <summary>
    /// Elemento identidad
    /// </summary>
    public class Identidad : ElementoGnoss
    {
        #region Miembros

        /// <summary>
        /// Perfil
        /// </summary>
        private Perfil mPerfil;

        /// <summary>
        /// Gestor de SuscripcionesfObt
        /// </summary>
        private GestionSuscripcion mGestionSuscripcion;

        /// <summary>
        /// Lista con los tags de la persona separados por comas
        /// </summary>
        private string mTags;

        /// <summary>
        /// Numero de recursos compartidos por la identidad.
        /// </summary>
        private string mNumeroRecursosCompartidos;

        /// <summary>
        /// Contiene la lista con todas las identidades de la persona de la identidad actual.
        /// </summary>
        private List<Guid> mListaTodosIdentidadesDeIdentidad;

        /// <summary>
        /// Lista de perfiles que tienen suscripciones
        /// </summary>
        private List<Guid> mListaPerfilesSuscritos = null;

        /// <summary>
        /// Lista de blogs que tienen suscripciones
        /// </summary>
        private List<Guid> mListaBlogsSuscritos = null;

        /// <summary>
        /// Identidad de organización
        /// </summary>
        private Identidad mIdentidadOrganizacion;

        /// <summary>
        /// Lista con los proyectos en los que participa el perfil actual.
        /// </summary>
        public Dictionary<Guid, Guid> mListaProyectosPerfilActual;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor a partir de los datos pasados como parámetros
        /// </summary>
        /// <param name="pPerfil">Perfil de la identidad</param>
        /// <param name="pFilaIdentidad">Fila de la identidad</param>
        public Identidad(AD.EntityModel.Models.IdentidadDS.Identidad pFilaIdentidad, Perfil pPerfil, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFilaIdentidad, pPerfil.GestorIdentidades, loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            this.mPerfil = pPerfil;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        #endregion

        #region Propiedades

        public bool EsIdentidadInvitada
        {
            get
            {
                return Clave.Equals(UsuarioAD.Invitado);
            }
        }

        /// <summary>
        /// Develve el perfil de la identidad.
        /// </summary>
        public Guid PerfilID
        {
            get
            {
                return FilaIdentidad.PerfilID;
            }
        }

        /// <summary>
        /// Obtiene la organización de la identidad (NULL si es un perfil personal)
        /// </summary>
        public ServiciosGenerales.Organizacion OrganizacionPerfil
        {
            get
            {
                return PerfilUsuario.OrganizacionPerfil;
            }
        }

        /// <summary>
        /// Obtiene el correo electrónico que corresponde a la identidad
        /// </summary>
        public string Email
        {
            get
            {
                if (ModoParticipacion.Equals(TiposIdentidad.Personal))
                {
                    if (GestorIdentidades.GestorPersonas != null)
                    {
                        return this.Persona.Email;
                    }
                }
                else if (ModoParticipacion.Equals(TiposIdentidad.Profesor))
                {
                    if (GestorIdentidades.GestorPersonas != null)
                    {
                        return GestorIdentidades.GestorPersonas.ListaPersonas[(Guid)this.FilaIdentidad.Perfil.PersonaID.Value].Mail;
                    }
                }
                else if (ModoParticipacion.Equals(TiposIdentidad.Organizacion))
                {
                    if (GestorIdentidades.GestorOrganizaciones != null)
                    {
                        return GestorIdentidades.GestorOrganizaciones.ListaOrganizaciones[(Guid)OrganizacionID.Value].FilaOrganizacion.Email;
                    }
                }
                else
                {
                    if (GestorIdentidades.GestorOrganizaciones != null && Persona.GestorPersonas != null)
                    {
                        Persona.GestorPersonas.GestorOrganizaciones = GestorIdentidades.GestorOrganizaciones;
                        string mail = "";

                        if (Persona.ListaDatosTrabajoPersonaOrganizacion.ContainsKey((Guid)OrganizacionID.Value))
                        {
                            mail = Persona.ListaDatosTrabajoPersonaOrganizacion[(Guid)OrganizacionID.Value].Mail;
                        }
                        return mail;
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Devuelve la lista de tags de la persona separados por comas
        /// </summary>
        public override string Tags
        {
            get
            {
                if (mTags == null)
                {
                    mTags = "";
                }
                return mTags;
            }
        }

        /// <summary>
        /// Obtiene la identidad de MyGNOSS del perfil actual
        /// </summary>
        public Identidad IdentidadMyGNOSS
        {//"PerfilID = '" + this.PerfilUsuario.Clave + "' AND ProyectoID = '" + ProyectoAD.MetaProyecto + "'"
            get
            {
                AD.EntityModel.Models.IdentidadDS.Identidad fila = GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.FirstOrDefault(identidad => identidad.PerfilID.Equals(this.PerfilUsuario.Clave) && identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto));

                if (fila != null)
                {
                    return this.GestorIdentidades.ListaIdentidades[fila.IdentidadID];
                }

                return this;
            }
        }

        /// <summary>
        /// Obtiene la identidad de MyGNOSS con el perfil PERSONAL
        /// </summary>
        public Identidad IdentidadPersonalMyGNOSS
        {
            get
            {//"PersonaID = '" + Util.Seguridad.Usuario.UsuarioActual.PersonaID + "' AND OrganizacionID IS NULL"
                List<AD.EntityModel.Models.IdentidadDS.Perfil> filasPerfilPersonal = GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perfil => perfil.PersonaID.Equals(PerfilUsuario.PersonaID) && !perfil.OrganizacionID.HasValue).ToList();

                if (filasPerfilPersonal.Count > 0)
                {
                    //Guid perfilID = filasPerfilPersonal[0].PerfilID;

                    //if (filasPerfilPersonal.Length > 1)
                    //{
                    //    foreach(IdentidadDS.PerfilRow filaPerf in filasPerfilPersonal)
                    //    {
                    //        if (GestorIdentidades.IdentidadesDS.Identidad.Select("PerfilID = '" + filaPerf.PerfilID + "' AND Tipo = " + (short)TiposIdentidad.Profesor).Length == 0)
                    //        {
                    //            perfilID = filaPerf.PerfilID;
                    //            break;
                    //        }
                    //    }
                    //}

                    //                   IdentidadDS.IdentidadRow[] filas = (IdentidadDS.IdentidadRow[])GestorIdentidades.IdentidadesDS.Identidad.Select("PerfilID = '" + perfilID + "' AND ProyectoID = '" + ProyectoAD.MetaProyecto
                    //+ "'");

                    //                   if (filas.Length > 0)
                    //                   {
                    //                       return this.GestorIdentidades.ListaIdentidades[filas[0].IdentidadID];
                    //                   }

                    List<AD.EntityModel.Models.IdentidadDS.Identidad> filas = GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.Tipo.Equals((short)TiposIdentidad.Personal) && identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).ToList();

                    if (filas.Count > 0)
                    {
                        return this.GestorIdentidades.ListaIdentidades[filas[0].IdentidadID];
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Devuelve la identidad de profesor de la identidad actual si la tiene, NULL en caso contrario.
        /// </summary>
        public Identidad IdentidadProfesorMyGnoss
        {
            get
            {
                if (Tipo == TiposIdentidad.Profesor)
                {
                    return this;
                }
                else
                {
                    foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdent in GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.Tipo.Equals((short)TiposIdentidad.Profesor) && identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).ToList())
                    {
                        if (GestorIdentidades.ListaIdentidades[filaIdent.IdentidadID].PerfilUsuario.FilaPerfil.PersonaID == PerfilUsuario.FilaPerfil.PersonaID)
                        {
                            return GestorIdentidades.ListaIdentidades[filaIdent.IdentidadID];
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene la lista de redes sociales del perfil
        /// </summary>
        public Dictionary<string, AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales> ListaRedesSociales
        {
            get
            {
                Dictionary<string, AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales> listaRedesSociales = new Dictionary<string, AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales>();
                foreach (AD.EntityModel.Models.IdentidadDS.PerfilRedesSociales filaPerfilRedesSoc in GestorIdentidades.DataWrapperIdentidad.ListaPerfilRedesSociales.Where(perfilRedesSociales => perfilRedesSociales.PerfilID.Equals(FilaIdentidad.PerfilID)).ToList())
                {
                    if (!listaRedesSociales.ContainsKey(filaPerfilRedesSoc.NombreRedSocial))
                    {
                        listaRedesSociales.Add(filaPerfilRedesSoc.NombreRedSocial, filaPerfilRedesSoc);
                    }
                }
                return listaRedesSociales;
            }
        }

        /// <summary>
        /// Obtiene la clave de la identidad
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaIdentidad.IdentidadID;
            }
        }

        /// <summary>
        /// Obtiene o establece el perfil del usuario
        /// </summary>
        public Perfil PerfilUsuario
        {
            get
            {
                return mPerfil;
            }
            set
            {
                mPerfil = value;
            }
        }

        /// <summary>
        /// Obtiene un valor que indica si la persona trabaja en modo personal o en modo profesional - personal (Solo Tipo 0 ,1)
        /// </summary>
        public bool ModoPersonal
        {
            get
            {
                return ModoParticipacion.Equals(TiposIdentidad.Personal) || ModoParticipacion.Equals(TiposIdentidad.ProfesionalPersonal) || ModoParticipacion.Equals(TiposIdentidad.Profesor);
            }
        }

        /// <summary>
        /// Verdad si la persona está trabajando por medio de una organización (Sea Tipo 1,2,3)
        /// </summary>
        public bool TrabajaConOrganizacion
        {
            get
            {
                return ((!ModoParticipacion.Equals(TiposIdentidad.Personal)) && (!ModoParticipacion.Equals(TiposIdentidad.Profesor) || PerfilUsuario.OrganizacionID.HasValue));
            }
        }

        /// <summary>
        /// Verdad si la persona está trabajando por medio de una organización (Solo Tipo 1,2)
        /// </summary>
        public bool TrabajaPersonaConOrganizacion
        {
            get
            {
                return (ModoParticipacion.Equals(TiposIdentidad.ProfesionalCorporativo) || ModoParticipacion.Equals(TiposIdentidad.ProfesionalPersonal));
            }
        }

        /// <summary>
        /// Devuelve el tipo (modo de participación) de una identidad
        /// </summary>
        public TiposIdentidad ModoParticipacion
        {
            get
            {
                return (TiposIdentidad)Enum.ToObject(typeof(TiposIdentidad), (short)FilaIdentidad.Tipo);
            }
        }

        /// <summary>
        /// Obtiene el gestor de identidades
        /// </summary>
        public GestionIdentidades GestorIdentidades
        {
            get
            {
                return PerfilUsuario.GestorIdentidades;
            }
        }

        /// <summary>
        /// Obtiene la fila de la identidad
        /// </summary>
        public AD.EntityModel.Models.IdentidadDS.Identidad FilaIdentidad
        {
            get
            {
                return (AD.EntityModel.Models.IdentidadDS.Identidad)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Obtiene la fila de relación de la identidad con la persona 
        /// o con la persona en una orgnizacion (puede ser una fila CVIdentidad o IdentidadPersonaOrg)
        /// </summary>
        public object FilaRelacionIdentidad
        {
            get
            {
                return PerfilUsuario.FilaRelacionPerfil;
            }
        }

        /// <summary>
        /// Obtiene el elemento público de la identidad (Persona u Organizacion)
        /// </summary>
        public ElementoGnoss ElementoPublico
        {
            get
            {
                if (ModoPersonal && GestorIdentidades.GestorPersonas != null)
                {
                    return Persona;
                }
                else
                {

                    if ((GestorIdentidades.GestorOrganizaciones != null) && (GestorIdentidades.GestorOrganizaciones.ListaOrganizaciones.ContainsKey((Guid)UtilReflection.GetValueReflection(PerfilUsuario.FilaRelacionPerfil, "OrganizacionID"))))
                    {
                        return GestorIdentidades.GestorOrganizaciones.ListaOrganizaciones[(Guid)UtilReflection.GetValueReflection(PerfilUsuario.FilaRelacionPerfil, "OrganizacionID")];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene la persona representada por la identidad
        /// </summary>
        public Persona Persona
        {
            get
            {
                return PerfilUsuario.PersonaPerfil;
            }
        }

        /// <summary>
        /// Devuelve el ID de la persona
        /// </summary>
        public Guid? PersonaID
        {
            get
            {
                return PerfilUsuario.PersonaID;
            }
        }

        /// <summary>
        /// Obtiene el usuario de la identidad
        /// </summary>
        public UsuarioGnoss Usuario
        {
            get
            {
                if (Persona != null)
                {
                    return Persona.Usuario;
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de amigos
        /// </summary>
        public GestionAmigos GestorAmigos
        {
            get
            {
                return GestorIdentidades.GestorAmigos;
            }
            set
            {
                GestorIdentidades.GestorAmigos = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de suscripciones
        /// </summary>
        public GestionSuscripcion GestionSuscripcion
        {
            get
            {
                return mGestionSuscripcion;
            }
            set
            {
                mGestionSuscripcion = value;
            }
        }

        /// <summary>
        /// Devuelve la lista de amigos 
        /// </summary>
        public Dictionary<Guid, Identidad> ListaContactos
        {
            get
            {
                if (GestorAmigos != null)
                {
                    return GestorAmigos.ListaContactos;
                }
                return new Dictionary<Guid, Identidad>();
            }
        }

        /// <summary>
        /// Devuelve la lista de amigos de la persona para la presentación.
        /// </summary>
        public Dictionary<Guid, Identidad> ListaAmigosPresentacion
        {
            get
            {
                if (GestorAmigos != null)
                {
                    return GestorAmigos.ListaAmigosPresentacion(Clave);
                }
                return new Dictionary<Guid, Identidad>();
            }
        }

        /// <summary>
        /// Lista con los Ids de los grupos de participación de la identidad actual (se construye cada vez que la llamas)
        /// </summary>
        public List<Guid> ListaGruposIdentidadParticipacion
        {
            get
            {
                List<Guid> listaIdsGrupos = new List<Guid>();

                foreach (GrupoIdentidadesParticipacion filaGrupo in FilaIdentidad.GrupoIdentidadesParticipacion)
                {
                    if (!listaIdsGrupos.Contains(filaGrupo.GrupoID))
                    {
                        listaIdsGrupos.Add(filaGrupo.GrupoID);
                    }
                }

                return listaIdsGrupos;
            }
        }

        /// <summary>
        /// Devuelve la lista de perfiles de amigos
        /// </summary>
        public List<Guid> ListaPerfilesAmigos
        {
            get
            {
                if (GestorAmigos != null)
                {
                    return GestorAmigos.ListaPerfilesAmigos;
                }
                return new List<Guid>();
            }
        }

        /// <summary>
        /// Obtiene o establece la lista de perfiles a los que estás suscrito "Dictionary(PerfilID, UsuarioID)
        /// </summary>
        public List<Guid> ListaPerfilesSuscritos
        {
            get
            {
                if (mListaPerfilesSuscritos == null)
                {
                    return new List<Guid>();
                }
                else
                {
                    return mListaPerfilesSuscritos;
                }
            }
            set
            {
                mListaPerfilesSuscritos = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la lista de perfiles a los que estás suscrito "Dictionary(PerfilID, UsuarioID)
        /// </summary>
        public List<Guid> ListaBlogsSuscritos
        {
            get
            {
                if (mListaBlogsSuscritos == null)
                {
                    return new List<Guid>();
                }
                else
                {
                    return mListaBlogsSuscritos;
                }
            }
            set
            {
                mListaBlogsSuscritos = value;
            }
        }

        /// <summary>
        /// Obtiene el nombre del elemento público
        /// </summary>
        public string Nombre(Guid? pIdentidadUsuarioActualID = null)
        {
            if (ModoParticipacion.Equals(TiposIdentidad.ProfesionalPersonal))
            {
                return PerfilUsuario.NombrePersonaEnOrganizacion + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + PerfilUsuario.NombreOrganizacion;
            }
            else if (ModoParticipacion.Equals(TiposIdentidad.ProfesionalCorporativo))
            {
                Identidad idUsuario = null;

                if (pIdentidadUsuarioActualID.HasValue && GestorIdentidades.ListaIdentidades.ContainsKey(pIdentidadUsuarioActualID.Value))
                {
                    idUsuario = GestorIdentidades.ListaIdentidades[pIdentidadUsuarioActualID.Value];

                    if (idUsuario.OrganizacionID.HasValue && OrganizacionID.HasValue && OrganizacionID.Equals(idUsuario.OrganizacionID) && !ModoPersonal && !EsOrganizacion)
                    {
                        return PerfilUsuario.NombreOrganizacion + " (" + PerfilUsuario.NombrePersonaEnOrganizacion + ")";
                    }
                }
                return PerfilUsuario.NombreOrganizacion;
            }
            else  //TiposIdentidad.Personal
            {
                if (Tipo != TiposIdentidad.Profesor)
                {
                    return PerfilUsuario.FilaPerfil.NombrePerfil;
                }
                else if (Persona != null)
                {
                    return PerfilUsuario.FilaPerfil.NombrePerfil.Substring(0, PerfilUsuario.FilaPerfil.NombrePerfil.IndexOf(ConstantesDeSeparacion.SEPARACION_CONCATENADOR) + 2) + Persona.NombreConApellidos;
                }
                else
                {//"Tipo="+(short)TiposIdentidad.Personal+" AND ProyectoID='"+ProyectoAD.MetaProyecto+"'"
                    List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdent = GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.Tipo.Equals((short)TiposIdentidad.Personal) && identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto)).ToList();

                    if (filasIdent.Count > 0)
                    {
                        foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdent in filasIdent)
                        {
                            foreach (AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perfil => perfil.PerfilID.Equals(filaIdent.PerfilID)).ToList())
                            {
                                if (filaPerfil.PersonaID == PerfilUsuario.PersonaID)
                                {
                                    return PerfilUsuario.FilaPerfil.NombrePerfil.Substring(0, PerfilUsuario.FilaPerfil.NombrePerfil.IndexOf(ConstantesDeSeparacion.SEPARACION_CONCATENADOR) + 2) + GestorIdentidades.ListaIdentidades[filaIdent.IdentidadID].PerfilUsuario.FilaPerfil.NombrePerfil;
                                }
                            }
                        }
                    }

                    return PerfilUsuario.FilaPerfil.NombrePerfil;
                }
            }

        }

        /// <summary>
        /// Obtiene el nombre de la identidad y si esta es de un perfil profesional muestra NOMBRE_ORG (NOMBRE_PERSONA)
        /// </summary>
        public string NombreCompuesto(Guid? pIdentidadID = null)
        {
            if (ModoParticipacion.Equals(TiposIdentidad.ProfesionalPersonal) || ModoParticipacion.Equals(TiposIdentidad.ProfesionalCorporativo))
            {
                return PerfilUsuario.NombrePersonaEnOrganizacion + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + PerfilUsuario.NombreOrganizacion;
            }

            return Nombre(pIdentidadID);
        }

        /// <summary>
        /// Obtiene el nombre corto de la identidad
        /// </summary>
        public string NombreCorto
        {
            get
            {
                return FilaIdentidad.NombreCortoIdentidad;
            }
        }

        /// <summary>
        /// Obtiene el nombre de la organización (en caso de que la identidad no sea personal)
        /// </summary>
        public string NombreOrganizacion
        {
            get
            {
                return PerfilUsuario.NombreOrganizacion;
            }
        }

        /// <summary>
        /// Obtiene la URL relativa de la imagen de la identidad
        /// </summary>
        public string UrlImagen
        {
            get
            {
                string urlFoto = "";

                if (string.IsNullOrEmpty(FilaIdentidad.Foto))
                {
                    //Si tenemos los datos ya cargados, nos evitamos la consulta a la BBDD
                    if ((Tipo == TiposIdentidad.Personal || Tipo == TiposIdentidad.Profesor) && Persona != null && Persona.FilaPersona.CoordenadasFoto != null)
                    {
                        urlFoto = "/" + UtilArchivos.ContentImagenesPersonas + "/" + Persona.Clave.ToString().ToLower() + "_peque.png";
                        if (Persona.FilaPersona.VersionFoto.HasValue)
                        {
                            urlFoto += "?" + Persona.FilaPersona.VersionFoto.Value;
                        }
                    }
                    else if ((Tipo == TiposIdentidad.ProfesionalCorporativo || Tipo == TiposIdentidad.Organizacion) && OrganizacionPerfil != null && OrganizacionPerfil.FilaOrganizacion.CoordenadasLogo != null)
                    {
                        urlFoto = "/" + UtilArchivos.ContentImagenesOrganizaciones + "/" + OrganizacionPerfil.Clave.ToString().ToLower() + "_peque.png";
                        if (OrganizacionPerfil.FilaOrganizacion.VersionLogo.HasValue)
                        {
                            urlFoto += "?" + OrganizacionPerfil.FilaOrganizacion.VersionLogo;
                        }
                    }
                    else if (Tipo == TiposIdentidad.ProfesionalPersonal && OrganizacionPerfil != null && Persona != null)
                    {
                        PersonaVinculoOrganizacion filaPersVincOrg = OrganizacionPerfil.GestorOrganizaciones.OrganizacionDW.ListaPersonaVinculoOrganizacion.FirstOrDefault(item => item.PersonaID.Equals(Persona.Clave) && item.OrganizacionID.Equals(OrganizacionPerfil.Clave));//.FindByPersonaIDOrganizacionID(Persona.Clave, OrganizacionPerfil.Clave);
                        if (filaPersVincOrg != null && Persona.FilaPersona.CoordenadasFoto != null)
                        {
                            if (filaPersVincOrg.UsarFotoPersonal)
                            {
                                urlFoto = "/" + UtilArchivos.ContentImagenesPersonas + "/" + Persona.Clave.ToString().ToLower() + "_peque.png";
                                if (Persona.FilaPersona.VersionFoto.HasValue)
                                {
                                    urlFoto += "?" + Persona.FilaPersona.VersionFoto.Value;
                                }
                            }
                            else if (filaPersVincOrg.CoordenadasFoto != null)
                            {
                                urlFoto = "/" + UtilArchivos.ContentImagenesPersona_Organizacion + "/" + OrganizacionPerfil.Clave.ToString().ToLower() + "/" + Persona.Clave.ToString().ToLower() + "_peque.png";
                                if (filaPersVincOrg.VersionFoto != null)
                                {
                                    urlFoto += "?" + filaPersVincOrg.VersionFoto;
                                }
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(urlFoto))
                    {
                        IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        urlFoto = identCN.ObtenerSiIdentidadTieneFoto(Clave, Tipo);
                        identCN.Dispose();
                    }
                }
                else if (FilaIdentidad.Foto == "sinfoto")
                {
                    urlFoto = "";
                }
                else
                {
                    urlFoto = FilaIdentidad.Foto;
                }

                if (string.IsNullOrEmpty(urlFoto))
                {
                    if (ModoParticipacion == TiposIdentidad.Personal || ModoParticipacion == TiposIdentidad.Profesor)
                    {
                        urlFoto = "/" + UtilArchivos.ContentImagenesPersonas + "/" + "anonimo_peque.png";
                    }
                    else if (ModoParticipacion == TiposIdentidad.ProfesionalCorporativo || ModoParticipacion == TiposIdentidad.Organizacion)
                    {
                        urlFoto = "/" + UtilArchivos.ContentImagenesOrganizaciones + "/" + "anonimo_peque.png";
                    }
                    else if (ModoParticipacion == TiposIdentidad.ProfesionalPersonal)
                    {
                        urlFoto = "/" + UtilArchivos.ContentImagenesPersonas + "/" + "anonimo_peque.png";
                    }
                }

                return urlFoto;
            }
        }

        /// <summary>
        /// Obtiene la URL relativa de la imagen grande de la identidad.
        /// </summary>
        public string UrlImagenGrande
        {
            get
            {
                return UrlImagen.Replace("peque", "grande");
            }
        }

        /// <summary>
        /// Obtiene el identificador de la organización (en caso de que la identidad no sea personal)
        /// </summary>
        public Guid? OrganizacionID
        {
            get
            {
                return PerfilUsuario.OrganizacionID;
            }
        }

        /// <summary>
        /// Devuelve si la identidad pertenece a una organización
        /// </summary>
        public bool EsOrganizacion
        {
            get
            {
                return ModoParticipacion.Equals(TiposIdentidad.Organizacion);
            }
        }

        /// <summary>
        /// Obtiene o establece el número de recursos compartidos por la identidad
        /// </summary>
        public string NumeroRecursosCompartidos
        {
            get
            {
                return mNumeroRecursosCompartidos;
            }
            set
            {
                mNumeroRecursosCompartidos = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la lista con todas las identidades de la persona de la identidad actual
        /// </summary>
        public List<Guid> ListaTodosIdentidadesDeIdentidad
        {
            get
            {
                if (mListaTodosIdentidadesDeIdentidad == null)
                {
                    mListaTodosIdentidadesDeIdentidad = new List<Guid>();

                    foreach (Perfil perfil in this.GestorIdentidades.ListaPerfiles.Values)
                    {
                        if (FilaIdentidad.Perfil.PersonaID.HasValue && perfil.PersonaID.Equals(FilaIdentidad.Perfil.PersonaID.Value))
                        {
                            foreach (Identidad identidad in perfil.Hijos)
                            {
                                mListaTodosIdentidadesDeIdentidad.Add(identidad.Clave);
                            }
                        }
                    }
                }
                return mListaTodosIdentidadesDeIdentidad;
            }
            set
            {
                mListaTodosIdentidadesDeIdentidad = value;
            }
        }

        /// <summary>
        /// Obtiene la identidad de la organización a la que pertenece el usuario (Modo Corporativo)
        /// </summary>
        public Identidad IdentidadOrganizacion
        {
            get
            {
                if (mIdentidadOrganizacion == null)
                {
                    if (TrabajaConOrganizacion)
                    {
                        AD.EntityModel.Models.IdentidadDS.Perfil perfilOrganizacion = GestorIdentidades.DataWrapperIdentidad.ListaPerfil.FirstOrDefault(perfilOrg => perfilOrg.OrganizacionID.HasValue && !perfilOrg.PersonaID.HasValue && perfilOrg.OrganizacionID.Value.Equals(OrganizacionID.Value));
                        if (perfilOrganizacion != null)
                        {
                            List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdentidadesOrganizacion = null;

                            filasIdentidadesOrganizacion = GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.ProyectoID.Equals(FilaIdentidad.ProyectoID) && identidad.PerfilID.Equals(perfilOrganizacion.PerfilID)).ToList();

                            //Si no encontramos identidades en el proyecto actual, cogeremos la identidad de MYGNOSS
                            if (filasIdentidadesOrganizacion == null || filasIdentidadesOrganizacion.Count == 0)
                            {
                                filasIdentidadesOrganizacion = GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && identidad.PerfilID.Equals(perfilOrganizacion.PerfilID)).ToList();
                            }

                            //Si no encontramos identidades en MyGnoss, cogeremos lo que haya (debería estar sólo el proyecto en el que ha entrado el usuario)
                            if (filasIdentidadesOrganizacion == null || filasIdentidadesOrganizacion.Count == 0)
                            {
                                filasIdentidadesOrganizacion = GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(perfilOrganizacion.PerfilID)).ToList();
                            }

                            if (filasIdentidadesOrganizacion.Count > 0)
                            {
                                AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidadOrganizacion = filasIdentidadesOrganizacion.First();

                                if (!GestorIdentidades.ListaPerfiles.ContainsKey(perfilOrganizacion.PerfilID) || !GestorIdentidades.ListaIdentidades.ContainsKey(filaIdentidadOrganizacion.IdentidadID))
                                {
                                    this.GestorIdentidades.RecargarHijos();
                                }

                                mIdentidadOrganizacion = GestorIdentidades.ListaIdentidades[filaIdentidadOrganizacion.IdentidadID];
                            }
                        }
                    }
                }
                return mIdentidadOrganizacion;
            }
        }

        /// <summary>
        /// Devuelve el tipo de identidad.
        /// </summary>
        public TiposIdentidad Tipo
        {
            get
            {
                return (TiposIdentidad)FilaIdentidad.Tipo;
            }
            set
            {
                FilaIdentidad.Tipo = (short)value;
            }
        }

        /// <summary>
        /// Verdad si la identidad actual es de tipo profesor
        /// </summary>
        public bool EsIdentidadProfesor
        {
            get
            {
                return (Tipo.Equals(TiposIdentidad.Profesor));
            }
        }

        /// <summary>
        /// Devuelve true si esta administrando una clase
        /// </summary>
        public bool EstaAdministrandoClase
        {
            get
            {
                return ((TiposIdentidad)FilaIdentidad.Tipo == TiposIdentidad.Profesor) && PerfilUsuario.OrganizacionID.HasValue;
            }
        }

        /// <summary>
        /// Indica si la identidad actual trabaja con clase.
        /// </summary>
        public bool TrabajaConClase
        {
            get
            {
                return OrganizacionID.HasValue && GestorIdentidades.GestorOrganizaciones.ListaOrganizaciones.ContainsKey(OrganizacionID.Value) && GestorIdentidades.GestorOrganizaciones.ListaOrganizaciones[OrganizacionID.Value] is ServiciosGenerales.OrganizacionClase;
            }
        }

        /// <summary>
        /// Devuelve una lista con los proyectos en los que participa el perfil actual, y la identidad con la que participa. (ProyectoID, IdentidadID)
        /// </summary>
        public Dictionary<Guid, Guid> ListaProyectosPerfilActual
        {
            get
            {
                if (mListaProyectosPerfilActual == null)
                {
                    mListaProyectosPerfilActual = new Dictionary<Guid, Guid>();
                    foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdent in GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.PerfilID.Equals(FilaIdentidad.PerfilID) && !identidad.FechaBaja.HasValue).ToList())
                    {
                        if (!mListaProyectosPerfilActual.ContainsKey(filaIdent.ProyectoID))
                        {
                            mListaProyectosPerfilActual.Add(filaIdent.ProyectoID, filaIdent.IdentidadID);
                        }
                    }
                }

                return mListaProyectosPerfilActual;
            }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Devuelve el identificador de la identidad activa de la identidad en una determinada comunidad si la tiene, Guid.Empty si no.
        /// </summary>
        /// <param name="pProyectoID">Identificador de la comunidad en la que se desea conocer la identidad</param>
        /// <returns>Identificador de la identidad activa de la identidad en una determinada comunidad si la tiene, 
        /// Guid.Empty si no</returns>
        public Guid ObtenerIdentidadEnProyectoDeIdentidad(Guid pProyectoID)
        {
            foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad in GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.ProyectoID.Equals(pProyectoID) && !identidad.FechaBaja.HasValue).ToList())
            {
                if (ListaTodosIdentidadesDeIdentidad.Contains(filaIdentidad.IdentidadID))
                {
                    return filaIdentidad.IdentidadID;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Devuelve la lista de amigos de la identidad actuales que son visibles por la identidad pasada por parámetro
        /// </summary>
        /// <param name="pIdentidad"></param>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public Dictionary<Guid, Identidad> ObtenerListaAmigosVisiblesPara(Identidad pIdentidad, Guid pProyectoID, Guid pUsuarioID)
        {
            Identidad identidadMyGnossDeIdentiad = pIdentidad.IdentidadMyGNOSS;

            Dictionary<Guid, Identidad> ListaIdentidades = new Dictionary<Guid, Identidad>();

            TipoVisibilidadContactosOrganizacion visibilidad = TipoVisibilidadContactosOrganizacion.Nadie;

            if (pProyectoID.Equals(ProyectoAD.MetaProyecto))
            {
                if (identidadMyGnossDeIdentiad.Clave.Equals(IdentidadMyGNOSS.Clave))
                {
                    return ListaContactos;
                }
                else if (Tipo == TiposIdentidad.Organizacion)
                {
                    OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperOrganizacion orgDW = orgCN.ObtenerOrganizacionDeIdentidad(Clave);
                    if (orgDW.ListaOrganizacion.Count > 0)
                    {
                        orgDW = orgCN.ObtenerConfiguracionGnossOrg(((AD.EntityModel.Models.OrganizacionDS.Organizacion)orgDW.ListaOrganizacion.FirstOrDefault()).OrganizacionID);
                        if (orgDW.ListaConfiguracionGnossOrg.Count > 0)
                        {
                            visibilidad = (TipoVisibilidadContactosOrganizacion)((ConfiguracionGnossOrg)orgDW.ListaConfiguracionGnossOrg.FirstOrDefault()).VisibilidadContactos;
                        }
                    }
                }
                else
                {
                    PersonaCN perCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    AD.EntityModel.Models.PersonaDS.ConfiguracionGnossPersona filaConf = perCN.ObtenerConfiguracionPersonaPorID(perCN.ObtenerPersonaPorIdentidadCargaLigera(Clave).PersonaID);
                    perCN.Dispose();
                    if (filaConf != null)
                    {
                        if (filaConf.VerAmigos)
                        {
                            visibilidad = TipoVisibilidadContactosOrganizacion.Contactos;
                            if (filaConf.VerAmigosExterno)
                            {
                                visibilidad = TipoVisibilidadContactosOrganizacion.ContactosDeContactos;
                            }
                        }
                    }
                }

                //Solo continuamos si la identidad tiene algun tipo de visibilidad
                if (visibilidad != TipoVisibilidadContactosOrganizacion.Nadie)
                {
                    //Procedemos a procesar cada contacto uno por uno
                    foreach (Guid idIdentidadContacto in ListaContactos.Keys)
                    {
                        Identidad IdentidadContacto = ListaContactos[idIdentidadContacto];
                        //Si el usuario tiene la visibilidad para contactos
                        if (visibilidad == TipoVisibilidadContactosOrganizacion.Contactos)
                        {
                            //si el usuario es contacto
                            if (ListaContactos.ContainsKey(identidadMyGnossDeIdentiad.Clave))
                            {
                                //Comprobamos la visibilidad del contacto
                                bool visibleMyGnoss = false;
                                if (IdentidadContacto.Tipo == TiposIdentidad.Organizacion)
                                {
                                    OrganizacionCN orgContactoCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                    DataWrapperOrganizacion orgContactoDW = orgContactoCN.ObtenerOrganizacionDeIdentidad(IdentidadContacto.Clave);
                                    if (orgContactoDW.ListaOrganizacion.Count > 0)
                                    {
                                        visibleMyGnoss = orgContactoDW.ListaOrganizacion.FirstOrDefault().EsBuscable || orgContactoDW.ListaOrganizacion.FirstOrDefault().EsBuscableExternos;
                                    }
                                }
                                else
                                {
                                    PersonaCN perContactoCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                    AD.EntityModel.Models.PersonaDS.Persona filaPersona = perContactoCN.ObtenerPersonaPorIdentidadCargaLigera(Clave);
                                    perContactoCN.Dispose();

                                    if (filaPersona.EsBuscable || filaPersona.EsBuscableExternos)
                                    {
                                        visibleMyGnoss = true;
                                    }
                                }

                                //si el contacto tiene el perfil público
                                if (visibleMyGnoss)
                                {
                                    ListaIdentidades.Add(idIdentidadContacto, IdentidadContacto);
                                }
                            }
                        }

                        //Si el usuario tiene la visibilidad para contactos de contactos
                        if (visibilidad == TipoVisibilidadContactosOrganizacion.ContactosDeContactos)
                        {
                            AmigosCN amigosCN = new AmigosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            bool esContasctoDeContacto = amigosCN.ComprobarIdentidadAmigosDeAmigos(Clave, identidadMyGnossDeIdentiad.Clave);
                            amigosCN.Dispose();
                            //si el usuario es contacto o contacto de contacto
                            if (esContasctoDeContacto || ListaContactos.ContainsKey(identidadMyGnossDeIdentiad.Clave))
                            {
                                //Comprobamos la visibilidad del contacto
                                bool visibleMyGnoss = false;
                                if (IdentidadContacto.Tipo == TiposIdentidad.Organizacion)
                                {
                                    OrganizacionCN orgContactoCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                    DataWrapperOrganizacion orgContactoDW = orgContactoCN.ObtenerOrganizacionDeIdentidad(IdentidadContacto.Clave);
                                    if (orgContactoDW.ListaOrganizacion.Count > 0)
                                    {
                                        visibleMyGnoss = orgContactoDW.ListaOrganizacion.FirstOrDefault().EsBuscable || orgContactoDW.ListaOrganizacion.FirstOrDefault().EsBuscableExternos;
                                    }
                                }
                                else
                                {
                                    PersonaCN perContactoCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                                    AD.EntityModel.Models.PersonaDS.Persona filaPers = perContactoCN.ObtenerPersonaPorIdentidadCargaLigera(IdentidadContacto.Clave);
                                    perContactoCN.Dispose();

                                    if (filaPers.EsBuscable || filaPers.EsBuscableExternos)
                                    {
                                        visibleMyGnoss = true;
                                    }

                                }

                                //si el contacto tiene el perfil público
                                if (visibleMyGnoss)
                                {
                                    ListaIdentidades.Add(idIdentidadContacto, IdentidadContacto);
                                }
                            }
                        }
                    }
                }

                return ListaIdentidades;
            }
            else
            {

                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                foreach (Identidad identidad in ListaContactos.Values)
                {
                    DataWrapperProyecto dataWrapperProyecto;
                    dataWrapperProyecto = proyCN.ObtenerProyectosParticipaPerfilUsuario(identidad.PerfilID, true, pUsuarioID);
                    if (dataWrapperProyecto.ListaProyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).ToList().Count > 0)
                    {
                        ListaIdentidades.Add(identidad.Clave, identidad);
                    }
                    //dataWrapperProyecto.Dispose();
                }
                proyCN.Dispose();
                return ListaIdentidades;
            }
        }

        /// <summary>
        /// Devuelve el identificador de la identidad de un usuario en una determinada comunidad con un perfil si lo hay.
        /// </summary>
        /// <param name="pProyectoID">Comunidad en la que se desea conocer la identidad</param>
        /// <param name="pPerfilID">Identificador del perfil que debe tener la comunidad.</param>
        /// <returns>Identificador de la identidad de un usuario en una determinada comunidad</returns>
        public Guid ObtenerIdentidadUsuarioEnProyectoConPerfil(Guid pProyectoID, Guid pPerfilID)
        {//"ProyectoID='" + pProyectoID + "' AND PerfilID='" + pPerfilID + "'"
            List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdentidad = this.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.ProyectoID.Equals(pProyectoID) && identidad.PerfilID.Equals(pPerfilID)).ToList();

            if (filasIdentidad.Count == 0)
            {
                //Juan Valer: No debe entrar nunca por aquí, lo pongo porque a habido incongruencias en datos de producción 
                //y puede ser porque entra por aqui en alguna ocasión. Lanzo este mensaje para que se quede constancia en el log 
                //y poder reproducir el error.
                throw new Exception("El perfil: '" + pPerfilID + "' de la identidad: '" + Clave + "' no tiene una identidad en el proyecto: '" + pProyectoID + "'");
            }
            return filasIdentidad.First().IdentidadID;
        }

        #endregion
    }
}
