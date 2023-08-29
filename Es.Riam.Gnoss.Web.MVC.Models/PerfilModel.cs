using System;
using System.Collections.Generic;
using System.Linq;
using static Es.Riam.Gnoss.Web.MVC.Models.CommunityModel;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    /// <summary>
    /// Tipos de perfil
    /// </summary>
    public enum ProfileType
    {
        /// <summary>
        /// Identidad personal
        /// </summary>
        Personal = 0,
        /// <summary>
        /// Identidad de una persona en una organización que trabaja en modo personal
        /// </summary>
        ProfessionalPersonal = 1,
        /// <summary>
        /// Identidad de una persona en una organización que trabaja en modo corporativo
        /// </summary>
        ProfessionalCorporate = 2,
        /// <summary>
        /// Identidad de una organización
        /// </summary>
        Organization = 3,
        /// <summary>
        /// Identidad de una persona que es profesor.
        /// </summary>
        Teacher = 4
    }

    /// <summary>
    /// Modelo de perfil actual
    /// </summary>
    [Serializable]
    public partial class UserProfileModel
    {
        public const int LastCacheVersion = 2;

        /// <summary>
        /// Nombre del perfil actual
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nombre de la organización del perfil actual
        /// </summary>
        public string NameOrg { get; set; }

        /// <summary>
        /// Nombre de la persona del perfil actual
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// Nombre compuesto del perfil actual, con el nombre de la organización
        /// </summary>
        public string CompleteProfileName { get; set; }

        /// <summary>
        /// URL del perfil actual
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// URL del perfil actual
        /// </summary>
        public string UrlViewProfile { get; set; }

        /// <summary>
        /// Foto del perfil actual
        /// </summary>
        public string Foto { get; set; }

        /// <summary>
        /// DNI del perfil actual
        /// </summary>
        public string DNI { get; set; }

        /// <summary>
        /// Identificador del perfil actual
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Tipo de perfil actual
        /// </summary>
        public ProfileType TypeProfile { get; set; }

        /// <summary>
        /// Indica si el perfil es de clase
        /// </summary>
        public bool IsClassProfile { get; set; }

        /// <summary>
        /// Indica si el perfil es administrador de alguna clase
        /// </summary>
        public bool IsClassAdministrator { get; set; }

        /// <summary>
        /// Lista de comunidades a las que pertenece el perfil
        /// </summary>
        public List<ProfileCommunitiesModel> Communities { get; set; }

        /// <summary>
        /// Lista de perfiles del usuario
        /// </summary>
        public List<UserProfileModel> UserProfiles { get; set; }

        /// <summary>
        /// Indica si está disponible el menú de administración de la organización.
        /// </summary>
        public bool IsAdministrator { get; set; }

        /// <summary>
        /// Indica si está disponible el menú de administración de la organización.
        /// </summary>
        public bool OrganizationMenuAvailable { get; set; }

        /// <summary>
        /// Url del espacio personal de la organización.
        /// </summary>
        public string OrganizationPersonalSpaceUrl { get; set; }

        /// <summary>
        /// Url de administración de usuario de la organización.
        /// </summary>
        public string AdminOrganizationUsersUrl { get; set; }

        /// <summary>
        /// Datos extra del perfil
        /// </summary>
        public Dictionary<string, string> ExtraData { get; set; }

        /// <summary>
        /// Datos extra de las identidades del perfil
        /// </summary>
        public Dictionary<Guid, Dictionary<string, string>> ExtraDataIdentities { get; set; }

        public DateTime BornDate { get; set; }

        /// <summary>
        /// Modelo de comunidad
        /// </summary>
        [Serializable]
        public partial class ProfileCommunitiesModel
        {
            /// <summary>
            /// Identificador de la comunidad
            /// </summary>
            public Guid Key { get; set; }
            /// <summary>
            /// Nombre de la comunidad
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Url de la comunidad
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// Tipo de la comunidad
            /// </summary>
            public short Type { get; set; }
            /// <summary>
            /// Numero de conexiones del perfil a esta comunidad
            /// </summary>
            public int NumberOfConnections { get; set; }
            /// <summary>
            /// Es comunidad de registro obligatorio
            /// </summary>
            public int EsRegistroObligatorio { get; set; }

            /// <summary>
            /// Tipo de proyecto
            /// </summary>
            public TypeProyect ProyectType { get; set; }
        }

        //public enum TypeRegister
        //{
        //    /// <summary>
        //    /// Registro Normal
        //    /// </summary>
        //    Normal = 0,
        //    /// <summary>
        //    /// Registro en pagina de promo
        //    /// </summary>
        //    Promo = 1,
        //    /// <summary>
        //    /// Registro con invitacion
        //    /// </summary>
        //    InvitacionCom = 2,
        //    /// <summary>
        //    /// Registro con invitacion
        //    /// </summary>
        //    InvitacionOrg = 3,
        //    /// <summary>
        //    /// Registro con invitacion
        //    /// </summary>
        //    Invitacion = 4,
        //    /// <summary>
        //    /// Registro con invitacion
        //    /// </summary>
        //    CAS_CRFP = 5
        //}

        ///// <summary>
        ///// Url de administración de usuario de la organización.
        ///// </summary>
        //public TypeRegister RegisterType { get; set; }

        public int CacheVersion { get; set; }
        public Guid PaisID { get; set; }
    }

    /// <summary>
    /// Modelo de la identidad actual
    /// </summary>
    [Serializable]
    public partial class UserIdentityModel
    {
        /// <summary>
        /// Identificador de la identidad
        /// </summary>
        public Guid KeyIdentity { get; set; }
        /// <summary>
        /// Identificador de su identidad en el metaproyecto
        /// </summary>
        public Guid KeyMetaProyectIdentity { get; set; }
        /// <summary>
        /// Identificador de la identidad de la organización, si no tiene organización el valor es Guid.Empty
        /// </summary>
        public Guid KeyIdentityOrg { get; set; }
        /// <summary>
        /// Identificador de su identidad de la organización en el metaproyecto, si no tiene organización el valor es Guid.Empty
        /// </summary>
        public Guid KeyMetaProyectIdentityOrg { get; set; }
        /// <summary>
        /// Identificador de su organizacion
        /// </summary>
        public Guid? KeyOrganization { get; set; }
        /// <summary>
        /// Identificador de su perfil
        /// </summary>
        public Guid KeyProfile { get; set; }
        /// <summary>
        /// Identificador de su persona
        /// </summary>
        public Guid KeyPerson { get; set; }
        /// <summary>
        /// Identificador de su usuario
        /// </summary>
        public Guid KeyUser { get; set; }
        /// <summary>
        /// Indica si la identidad es Invitada, no participa en la comunidad
        /// </summary>
        public bool IsGuestIdentity { get; set; }
        /// <summary>
        /// Indica si el usuario es invitado, no esta registrado
        /// </summary>
        public bool IsGuestUser { get; set; }
        /// <summary>
        /// Indica si la identidad actual es administrador de la organización.
        /// </summary>
        public bool IsOrgAdmin { get; set; }
        /// <summary>
        /// Indica si la identidad actual es supervisor de la organización.
        /// </summary>
        public bool IsOrgSupervisor { get; set; }
        /// <summary>
        /// Indica si la identidad actual es administrador del proyecto.
        /// </summary>
        public bool IsProyectAdmin { get; set; }
        /// <summary>
        /// Indica si la identidad actual es supervisor del proyecto.
        /// </summary>
        public bool IsProyectSupervisor { get; set; }
        /// <summary>
        /// Indica si la identidad actual es de tipo un profesor.
        /// </summary>
        public bool IsTeacher { get; set; }
        /// <summary>
        /// Email de la persona de la identidad.
        /// </summary>
        public string PersonEmail { get; set; }
        /// <summary>
        /// Nombre de la persona de la identidad.
        /// </summary>
        public string PersonName { get; set; }
        /// <summary>
        /// Apellidos de la persona de la identidad.
        /// </summary>
        public string PersonFamilyName { get; set; }
        /// <summary>
        /// NIF de la persona
        /// </summary>
        public string PersonalID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool PermitEditAllPeople { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool PermitEditAllOrganizations { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool PermitEditAllCommunities { get; set; }
        /// <summary>
        /// Indica si la identidad ha sido expulsada
        /// </summary>
        public bool IsExpelled { get; set; }
        /// <summary>
        /// User IP
        /// </summary>
        public string PublicIP { get; set; }
        /// <summary>
        /// Number of login attemprs from the same IP
        /// </summary>
        public int NumberOfLoginAttemptsIP { get; set; }
        /// <summary>
        /// Indica si debe recibir la newsletter
        /// </summary>
        public bool ReceiveNewsletter { get; set; }
        /// <summary>
        /// Indica la Fecha de Nacimiento
        /// </summary>
        public DateTime BornDate { get; set; }

        public CommunityRequestStatusEnum CommunityRequestStatus { get; set; }

        /// <summary>
        /// Datos extra de la identidad
        /// </summary>
        public Dictionary<string, string> ExtraData { get; set; }

        /// <summary>
        /// Datos extra de la identidad por comunidad
        /// </summary>
        public Dictionary<Guid, Dictionary<string, string>> ExtraDataCommunities { get; set; }

        /// <summary>
        /// Enumeración del estado de la solicitud de una comunidad
        /// </summary>
        public enum CommunityRequestStatusEnum
        {
            /// <summary>
            /// Sin solicitud
            /// </summary>
            NoRequest,
            /// <summary>
            /// Solicitud pendiente de aprobar
            /// </summary>
            RequestPending,
            /// <summary>
            /// Solicitud realizada con otra identidad
            /// </summary>
            RequestedWithAnotherProfile

        }

        /// <summary>
        /// Lista de grupos de la identidad
        /// </summary>
        public List<GroupCardModel> IdentityGroups { get; set; }
    }

    /// <summary>
    /// Modelo de perfil
    /// </summary>
    [Serializable]
    public partial class ProfileModel : ObjetoBuscadorModel
    {
        /// <summary>
        /// Identificador de la identidad
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Tipo de identidad
        /// </summary>
        public ProfileType TypeProfile { get; set; }

        /// <summary>
        /// Indica si la identidad es de clase
        /// </summary>
        public bool IsClass { get; set; }

        /// <summary>
        /// Nombre de la persona del perfil
        /// </summary>
        public string NamePerson { get; set; }
        public string NombreCortoUsuario { get; set; }
        public string NombreCortoOrganizacion { get; set; }

        public DateTime BornDate { get; set; }

        /// <summary>
        /// Tiene Email del tutor
        /// </summary>
        public bool TieneEmailTutor { get; set; }

        /// <summary>
        /// Identificador de la persona
        /// </summary>
        public Guid KeyPerson { get; set; }

        /// <summary>
        /// Nombre de la organización del perfil
        /// </summary>
        public string NameOrganization { get; set; }

        /// <summary>
        /// Identificador de la organizacion
        /// </summary>
        public Guid KeyOrganization { get; set; }

        /// <summary>
        /// Url de la persona del perfil
        /// </summary>
        public string UrlPerson { get; set; }

        /// <summary>
        /// Url de la organización del perfil
        /// </summary>
        public string UrlOrganization { get; set; }

        /// <summary>
        /// Gets de URL for this profile (Person URL if it's a person or Organization URL if it's an organization)
        /// </summary>
        public string Url
        {
            get
            {
                if (!string.IsNullOrEmpty(UrlPerson))
                {
                    return UrlPerson;
                }
                else
                {
                    return UrlOrganization;
                }
            }
        }

        /// <summary>
        /// Url de la foto del perfil
        /// </summary>
        public string UrlFoto { get; set; }

        /// <summary>
        /// Contador de conexiones
        /// </summary>
        public int ConnectionCounter { get; set; }

        /// <summary>
        /// Informacion extra del perfil
        /// </summary>
        public ExtraInfoProfileModel ExtraInfo { get; set; }

        /// <summary>
        /// Gets the User rol in this project
        /// </summary>
        public UserRol Rol { get; set; }

        /// <summary>
        /// Modelo de información extra del perfil
        /// </summary>
        [Serializable]
        public partial class ExtraInfoProfileModel
        {
            /// <summary>
            /// Pais del perfil
            /// </summary>
            public string Country { get; set; }

            /// <summary>
            /// Provincia o estado del perfil
            /// </summary>
            public string ProvinceOrState { get; set; }

            /// <summary>
            /// Localidad del perfil
            /// </summary>
            public string Locality { get; set; }

            /// <summary>
            /// Cargo del perfil
            /// </summary>
            public string Cargo { get; set; }

            /// <summary>
            /// Etiquetas del perfil
            /// </summary>
            public List<string> Tags { get; set; }

            /// <summary>
            /// Descripción del perfil
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Puestos actuales del perfil
            /// </summary>
            public List<string> Puestos { get; set; }

            /// <summary>
            /// Datos extra del perfil
            /// </summary>
            public Dictionary<string, string> ExtraData { get; set; }

            /// <summary>
            /// Contadores de la identidad del perfil
            /// </summary>
            public IdentityResourceCounter IdentityResourceCounter { get; set; }
        }

        /// <summary>
        /// Modelo de los contadores de la identidad
        /// </summary>
        [Serializable]
        public partial class IdentityResourceCounter
        {
            /// <summary>
            /// Contador de recursos
            /// </summary>
            public List<Tuple<ResourceModel.DocumentType, string, int, int, int>> ResourceTypeCounter { get; set; }

            /// <summary>
            /// Devuelve el número de recursos publicados de un tipo en particular
            /// </summary>
            /// <param name="pType">Tipo de recurso</param>
            /// <returns></returns>
            public int GetPublicationCounterByType(ResourceModel.DocumentType pType)
            {
                int num = 0;
                List<Tuple<ResourceModel.DocumentType, string, int, int, int>> lista = ResourceTypeCounter.Where(tupla => tupla.Item1 == pType).ToList();
                if (lista != null && lista.Count > 0)
                {
                    foreach (Tuple<ResourceModel.DocumentType, string, int, int, int> tupla in lista)
                    {
                        num += tupla.Item3;
                    }
                }
                return num;
            }

            /// <summary>
            /// Devuelve el número de recursos publicados de una ontología en particular
            /// </summary>
            /// <param name="pRdfType">Nombre de ontología</param>
            /// <returns></returns>
            public int GetPublicationCounterByRdfType(string pRdfType)
            {
                pRdfType = pRdfType.ToLower();
                if (!pRdfType.EndsWith(".owl"))
                {
                    pRdfType = pRdfType + ".owl";
                }

                int num = 0;
                List<Tuple<ResourceModel.DocumentType, string, int, int, int>> lista = ResourceTypeCounter.Where(tupla => tupla.Item2.ToLower() == pRdfType).ToList();
                if (lista != null && lista.Count > 0)
                {
                    foreach (Tuple<ResourceModel.DocumentType, string, int, int, int> tupla in lista)
                    {
                        num += tupla.Item3;
                    }
                }
                return num;
            }

            /// <summary>
            /// Devuelve el número de recursos publicados
            /// </summary>
            /// <returns></returns>
            public int GetPublicationCounter()
            {
                int num = 0;
                if (ResourceTypeCounter != null && ResourceTypeCounter.Count > 0)
                {
                    foreach (Tuple<ResourceModel.DocumentType, string, int, int, int> tupla in ResourceTypeCounter)
                    {
                        num += tupla.Item3;
                    }
                }
                return num;
            }

            /// <summary>
            /// Devuelve el número de recursos compartidos de un tipo en particular
            /// </summary>
            /// <param name="pType">Tipo de recurso</param>
            /// <returns></returns>
            public int GetSharedCounterByType(ResourceModel.DocumentType pType)
            {
                int num = 0;
                List<Tuple<ResourceModel.DocumentType, string, int, int, int>> lista = ResourceTypeCounter.Where(tupla => tupla.Item1 == pType).ToList();
                if (lista != null && lista.Count > 0)
                {
                    foreach (Tuple<ResourceModel.DocumentType, string, int, int, int> tupla in lista)
                    {
                        num += tupla.Item4;
                    }
                }
                return num;
            }

            /// <summary>
            /// Devuelve el número de recursos compartidos de una ontología en particular
            /// </summary>
            /// <param name="pRdfType">Nombre de ontología</param>
            /// <returns></returns>
            public int GetSharedCounterByRdfType(string pRdfType)
            {
                pRdfType = pRdfType.ToLower();
                if (!pRdfType.EndsWith(".owl"))
                {
                    pRdfType = pRdfType + ".owl";
                }

                int num = 0;
                List<Tuple<ResourceModel.DocumentType, string, int, int, int>> lista = ResourceTypeCounter.Where(tupla => tupla.Item2.ToLower() == pRdfType).ToList();
                if (lista != null && lista.Count > 0)
                {
                    foreach (Tuple<ResourceModel.DocumentType, string, int, int, int> tupla in lista)
                    {
                        num += tupla.Item4;
                    }
                }
                return num;
            }

            /// <summary>
            /// Devuelve el número de recursos compartidos
            /// </summary>
            /// <returns></returns>
            public int GetSharedCounter()
            {
                int num = 0;
                if (ResourceTypeCounter != null && ResourceTypeCounter.Count > 0)
                {
                    foreach (Tuple<ResourceModel.DocumentType, string, int, int, int> tupla in ResourceTypeCounter)
                    {
                        num += tupla.Item4;
                    }
                }
                return num;
            }

            /// <summary>
            /// Devuelve el número de comentarios publicados en un tipo de recursos en particular
            /// </summary>
            /// <param name="pType">Tipo de recurso</param>
            /// <returns></returns>
            public int GetCommentCounterByType(ResourceModel.DocumentType pType)
            {
                int num = 0;
                List<Tuple<ResourceModel.DocumentType, string, int, int, int>> lista = ResourceTypeCounter.Where(tupla => tupla.Item1 == pType).ToList();
                if (lista != null && lista.Count > 0)
                {
                    foreach (Tuple<ResourceModel.DocumentType, string, int, int, int> tupla in lista)
                    {
                        num += tupla.Item5;
                    }
                }
                return num;
            }

            /// <summary>
            /// Devuelve el número de comentarios publicados en una ontología en particular
            /// </summary>
            /// <param name="pRdfType">Nomrbe de ontología</param>
            /// <returns></returns>
            public int GetCommentCounterByRdfType(string pRdfType)
            {
                pRdfType = pRdfType.ToLower();
                if (!pRdfType.EndsWith(".owl"))
                {
                    pRdfType = pRdfType + ".owl";
                }

                int num = 0;
                List<Tuple<ResourceModel.DocumentType, string, int, int, int>> lista = ResourceTypeCounter.Where(tupla => tupla.Item2.ToLower() == pRdfType).ToList();
                if (lista != null && lista.Count > 0)
                {
                    foreach (Tuple<ResourceModel.DocumentType, string, int, int, int> tupla in lista)
                    {
                        num += tupla.Item5;
                    }
                }
                return num;
            }

            /// <summary>
            /// Devuelve el número de comentarios publicados
            /// </summary>
            /// <returns></returns>
            public int GetCommentCounter()
            {
                int num = 0;
                if (ResourceTypeCounter != null && ResourceTypeCounter.Count > 0)
                {
                    foreach (Tuple<ResourceModel.DocumentType, string, int, int, int> tupla in ResourceTypeCounter)
                    {
                        num += tupla.Item5;
                    }
                }
                return num;
            }

        }

        /// <summary>
        /// Lista de redes sociales del perfil
        /// </summary>
        public List<SocialNetworkProfileModel> SocialNetworks { get; set; }

        /// <summary>
        /// Modelo de redes sociales de perfil
        /// </summary>
        [Serializable]
        public partial class SocialNetworkProfileModel
        {
            /// <summary>
            /// Nombre de la red social
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Url de la red social
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// Url del perfil en la red social
            /// </summary>
            public string UrlProfile { get; set; }
            /// <summary>
            /// Url del favicon en la red social
            /// </summary>
            public string UrlFavicon { get; set; }
        }

        /// <summary>
        /// Acciones que se pueden realizar sobre este perfil
        /// </summary>
        public ActionsModel Actions { get; set; }

        /// <summary>
        /// Modelo de acciones que se pueden realizar sobre este perfil
        /// </summary>
        [Serializable]
        public partial class ActionsModel
        {
            /// <summary>
            /// Enumeracion de estado de contacto
            /// </summary>
            public enum StatusContact
            {
                NoContact,
                Pendieng,
                Contact
            }

            /// <summary>
            /// Indica si el usuario puede añadir como contacto al perfil
            /// </summary>
            public bool AddContact { get; set; }
            /// <summary>
            /// Estado del contacto: ya es contacto, no es contacto o esperando la respuesta a tu petición de contacto
            /// </summary>
            public StatusContact StatusContactProfile { get; set; }
            /// <summary>
            /// Indica si el usuario puede añadir como contacto de la organizacion al perfil
            /// </summary>
            public bool AddContactOrg { get; set; }
            /// <summary>
            /// Estado del contacto de organización: ya es contacto, no es contacto o esperando la respuesta a tu petición de contacto
            /// </summary>
            public StatusContact StatusContactOrg { get; set; }
            /// <summary>
            /// Indica si el usuario puede enviar un mensaje al perfil
            /// </summary>
            public bool SendMessage { get; set; }
            /// <summary>
            /// Indica si el usuario puede hacerse seguidor del perfil
            /// </summary>
            public bool Follow { get; set; }
            /// <summary>
            /// Indica si el usuario es seguidor del perfil
            /// </summary>
            public bool FollowingProfile { get; set; }
            /// <summary>
            /// Indica si el usuario puede enviar una correccion (botón de enviar corrección)
            /// </summary>
            public bool ReportCorrection { get; set; }
            /// <summary>
            /// Indica si el usuario puede enviar una correccion definitiva (botón de enviar corrección definitiva)
            /// </summary>
            public bool ReportDefinitiveCorrection { get; set; }
            /// <summary>
            /// Indica si el usuario puede validar una correccion (botón de validar corrección)
            /// </summary>
            public bool ValidateCorrection { get; set; }
            /// <summary>
            /// Indica si el usuario puede eliminar el perfil
            /// </summary>
            public bool DeletePerson { get; set; }

            /// <summary>
            /// Indica si el administrador puede expulsar a este usuario
            /// </summary>
            public bool ExpelMember { get; set; }

            /// <summary>
            /// Indica si el administrador puede readmitir a un usuario previamente expulsado
            /// </summary>
            public bool ReadmitMember { get; set; }

            /// <summary>
            /// Indica se el administrador puede bloquear a este usuario
            /// </summary>
            public bool BlockUser { get; set; }

            /// <summary>
            /// Indica se el administrador puede desbloquear a este usuario
            /// </summary>
            public bool UnblockUser { get; set; }

            /// <summary>
            /// Indica se el administrador puede volver enviar newsletters a este usuario
            /// </summary>
            public bool SendNewsletter { get; set; }

            /// <summary>
            /// Indica se el administrador puede dejar de enviar newsletters a este usuario
            /// </summary>
            public bool NotSendNewsletter { get; set; }

            /// <summary>
            /// Indica se el administrador puede cambiar el rol a este usuario (Administrador, supervisor o usuario)
            /// </summary>
            public bool ChangeRol { get; set; }
            public bool ResetPassword { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public UrlActions ListActions { get; set; }
        /// <summary>
        /// Modelo con las acciones de una perfil
        /// </summary>
        [Serializable]
        public partial class UrlActions
        {
            /// <summary>
            /// Url para seguir un perfil
            /// </summary>
            public string UrlFollow { get; set; }
            /// <summary>
            /// Url para dejar de seguir un perfil
            /// </summary>
            public string UrlUnfollow { get; set; }
        }
    }

    /// <summary>
    /// Modelo de grupo
    /// </summary>
    [Serializable]
    public partial class GroupCardModel : ObjetoBuscadorModel
    {
        /// <summary>
        /// Enumeración de tipo de grupo
        /// </summary>
        public enum GroupTypes
        {
            /// <summary>
            /// Tipo de grupo de comunidad
            /// </summary>
            Community,
            /// <summary>
            /// Tipo de grupo de organización
            /// </summary>
            Organization
        }
        /// <summary>
        /// Identificador del grupo
        /// </summary>
        public Guid Clave { get; set; }
        /// <summary>
        /// Nombre del grupo
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Nombre del grupo
        /// </summary>
        public string CompleteName { get; set; }
        /// <summary>
        /// Descripcion del grupo
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Nombre del grupo
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// URL del grupo
        /// </summary>
        public string UrlGroup { get; set; }
        /// <summary>
        /// URL del grupo
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// Tipo de grupo
        /// </summary>
        public GroupTypes GroupType { get; set; }
        /// <summary>
        /// Indica si se puede enviar un mensaje al grupo
        /// </summary>
        public bool AllowSendMessage { get; set; }
        /// <summary>
        /// Indica si se puede abandonar el grupo
        /// </summary>
        public bool AllowLeaveGroup { get; set; }
        /// <summary>
        /// Nombre del proyecto
        /// </summary>
        public Guid ProyectKey { get; set; }
        /// <summary>
        /// Nombre corto del proyecto
        /// </summary>
        public string ProyectShortName { get; set; }
    }

    /// <summary>
    /// Enumeration for the diferent user roles
    /// </summary>
    public enum UserRol
    {
        /// <summary>
        /// Administrator rol
        /// </summary>
        Administrator = 0,

        /// <summary>
        /// Supervisor rol
        /// </summary>
        Supervisor = 1,

        /// <summary>
        /// Plain user rol
        /// </summary>
        User = 2
    }

    /// <summary>
    /// Modelo de peticionContrasenya
    /// </summary>
    [Serializable]
    public partial class PeticionContrasenya
    {
        public string pass { get; set; }

        public string nombreCortoUsu { get; set; }


    }
}