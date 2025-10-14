using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Comentario;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Comentario;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.Comentario
{
    /// <summary>
    /// Comentario
    /// </summary>
    public class Comentario : ElementoGnoss
    {
        #region Miembros

        /// <summary>
        /// Lista de votos del comentario
        /// </summary>
        private Dictionary<Guid, AD.EntityModel.Models.Voto.Voto> mListaVotos;

        /// <summary>
        /// Indica que el nombre debe ser el del usuario actual.
        /// </summary>
        public static string NombreUsuarioActual = "UsuarioActual";

        /// <summary>
        /// Indica si el comentario es editable o no (David: Lo pongo por defecto a TRUE)
        /// </summary>
        //private bool mEditable = true;

        /// <summary>
        /// URL de la identidad creadora del comentario
        /// </summary>
        private string mURLFotoIdentidad = null;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor de comentario
        /// </summary>
        /// <param name="pGestionComentarios">Gestor de comentarios</param>
        /// <param name="pComentario">DataRow de comentario</param>
        public Comentario(AD.EntityModel.Models.Comentario.Comentario pComentario, GestionComentarios pGestionComentarios,  LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<Comentario> logger, ILoggerFactory loggerFactory)
            : base(pComentario, pGestionComentarios)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Tipo de perfil del creador del documento.
        /// </summary>
        public short TipoPerfilCreador
        {
            get
            {
                try
                {
                    if (FilaComentario.TipoPerfil.HasValue)
                    {
                        return FilaComentario.TipoPerfil.Value;
                    }
                    else
                    {
                        return -1;
                    }
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Identificador de la organización del creador del documento si esta en modo profesional o organización.
        /// </summary>
        public Guid OrganizacionPerfilCreador
        {
            get
            {
                try
                {
                    if (FilaComentario.OrganizacionPerfil.HasValue)
                    {
                        return FilaComentario.OrganizacionPerfil.Value;
                    }
                    else
                    {
                        return Guid.Empty;
                    }
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            }
        }

        /// <summary>
        /// Obtiene el gestor de comentarios
        /// </summary>
        public GestionComentarios GestorComentarios
        {
            get
            {
                return (GestionComentarios)GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene el dataRow del comentario
        /// </summary>
        public AD.EntityModel.Models.Comentario.Comentario FilaComentario
        {
            get
            {
                return (AD.EntityModel.Models.Comentario.Comentario)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Clave del factor
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaComentario.ComentarioID;
            }
        }

        /// <summary>
        /// Descripcion del comentario
        /// </summary>
        public override string Nombre
        {
            get
            {
                return FilaComentario.Descripcion;
            }
            set
            {
                FilaComentario.Descripcion = value;
            }
        }

        /// <summary>
        /// Descripcion del comentario
        /// </summary>
        public string NombreSem
        {
            get
            {
                return UtilCadenas.EliminarCaracteresUrlSem(FilaComentario.Descripcion);
            }
        }

        /// <summary>
        /// Fecha del comentario
        /// </summary>
        public override DateTime Fecha
        {
            get
            {
                return FilaComentario.Fecha;
            }
            set
            {
                FilaComentario.Fecha = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de votos del comentario
        /// </summary>
        public Dictionary<Guid, AD.EntityModel.Models.Voto.Voto> ListaVotos
        {
            get
            {
                if (mListaVotos == null)
                {
                    mListaVotos = new Dictionary<Guid, AD.EntityModel.Models.Voto.Voto>();
                    List<VotoComentario> filasVotosComentario = GestorComentarios.ComentarioDW.ListaVotoComentario.Where(item => item.ComentarioID.Equals(this.Clave)).ToList();

                    foreach (VotoComentario filaVotoComentario in filasVotosComentario)
                    {
                        if (GestorComentarios.GestorVotos.ListaVotos.ContainsKey(filaVotoComentario.VotoID))
                        {
                            AD.EntityModel.Models.Voto.Voto voto = GestorComentarios.GestorVotos.ListaVotos[filaVotoComentario.VotoID];
                            mListaVotos.Add(voto.VotoID, voto);
                        }
                    }
                }
                return mListaVotos;
            }
        }

        /// <summary>
        /// Devuelve la URL de la foto de la identidad que ha hecho el comentario.
        /// </summary>
        public string URLFotoIdentidad
        {
            get
            {
                try
                {
                    if (mURLFotoIdentidad == null)
                    {
                        IdentidadCN identCN = new IdentidadCN( mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                        string urlFoto = identCN.ObtenerSiIdentidadTieneFoto(FilaComentario.IdentidadID);
                        identCN.Dispose();

                        short tipo = FilaComentario.TipoPerfil.Value;

                        if (urlFoto == null)
                        {
                            if (tipo == (short)TiposIdentidad.Personal || tipo == (short)TiposIdentidad.Profesor)
                            {
                                urlFoto = "/" + UtilArchivos.ContentImagenesPersonas + "/anonimo_peque.png";
                            }
                            else if (tipo == (short)TiposIdentidad.ProfesionalCorporativo)
                            {
                                urlFoto = "/" + UtilArchivos.ContentImagenesOrganizaciones + "/anonimo_peque.png";
                            }
                            else if (tipo == (short)TiposIdentidad.ProfesionalPersonal)
                            {
                                urlFoto = "/" + UtilArchivos.ContentImagenesPersonas + "/anonimo_peque.png";
                            }
                        }
                        mURLFotoIdentidad = urlFoto;
                    }
                }
                catch (Exception)
                {
                    mURLFotoIdentidad = null;
                }

                return mURLFotoIdentidad;
            }
            set
            {
                mURLFotoIdentidad = value;
            }
        }

        /// <summary>
        /// Devuelve el nombre del autor del comentario.
        /// </summary>
        public string NombreAutor
        {
            get
            {
                try
                {
                    if (FilaComentario.TipoPerfil.Value == (short)TiposIdentidad.Personal || FilaComentario.TipoPerfil.Value == (short)TiposIdentidad.Profesor)
                    {
                        return FilaComentario.NombreAutor;
                    }
                    else if (FilaComentario.TipoPerfil.Value == (short)TiposIdentidad.ProfesionalPersonal)
                    {
                        return $"{FilaComentario.NombreAutor} {ConstantesDeSeparacion.SEPARACION_CONCATENADOR} {FilaComentario.NombreOrganizacion}";
                    }
                    else if (FilaComentario.TipoPerfil.Value == (short)TiposIdentidad.ProfesionalCorporativo)
                    {
                        return FilaComentario.NombreOrganizacion;
                    }
                    else
                    {//Es de organización
                        return FilaComentario.NombreOrganizacion;
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, mlogger);
                    return null;
                }
            }
        }

        /// <summary>
        /// Devuelve el nombre del creador o publicador del documento.
        /// </summary>
        public string NombreOrganizacionCreadora
        {
            get
            {
                try
                {
                    if (FilaComentario.NombreOrganizacion != null)
                    {
                        return FilaComentario.NombreOrganizacion;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, mlogger);
                    return null;
                }
            }
        }

        /// <summary>
        /// Devuelve el nombre del creador o publicador del documento.
        /// </summary>
        public string NombrePersonaCreador
        {
            get
            {
                try
                {
                    if (FilaComentario.NombreAutor != null)
                    {
                        return FilaComentario.NombreAutor;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, mlogger);
                    return null;
                }
            }
        }

        /// <summary>
        /// Obtiene el tipo del comentario
        /// </summary>
        public TipoComentario? Tipo
        {
            get
            {
                if (FilaComentario.Tipo.HasValue)
                    return (TipoComentario)(FilaComentario.Tipo.Value);
                return null;
            }
        }

        /// <summary>
        /// Obtiene el nnombre del elemento vinculado al comentario
        /// </summary>
        public string NombreElementoVinculado
        {
            get
            {
                if (FilaComentario.NombreElemVinculado != null)
                    return FilaComentario.NombreElemVinculado;
                return null;
            }
        }

        /// <summary>
        /// Obtiene el identificador del elemento vinculado al comentario
        /// </summary>
        public Guid? ElementoVinculadoID
        {
            get
            {
                if (FilaComentario.ElementoVinculadoID.HasValue)
                    return FilaComentario.ElementoVinculadoID.Value;
                return null;
            }
        }

        /// <summary>
        /// Obtiene el nombre del padre del elemento vinculado al comentario
        /// </summary>
        public string NombrePadreElemVin
        {
            get
            {
                if (FilaComentario.NombrePadreElemVin != null)
                    return FilaComentario.NombrePadreElemVin;
                return null;
            }
        }

        /// <summary>
        /// Obtiene el identificador del padre del elemento vinculado al comentario
        /// </summary>
        public Guid? PadreElemVinID
        {
            get
            {
                if (FilaComentario.PadreElemVinID.HasValue)
                    return FilaComentario.PadreElemVinID.Value;
                return null;
            }
        }

        /// <summary>
        /// Voto del comentario.
        /// </summary>
        public double Voto
        {
            get
            {
                List<VotoComentario> filasVotosCom = GestorComentarios.ComentarioDW.ListaVotoComentario.Where(item => item.ComentarioID.Equals(Clave)).ToList();
                double sumaVotos = 0;
                foreach (VotoComentario filaVotosCom in filasVotosCom)
                {
                    List<AD.EntityModel.Models.Voto.Voto> filasVoto = GestorComentarios.GestorVotos.ListaVotos.Values.Where(item => item.VotoID.Equals(filaVotosCom.VotoID)).ToList();
                    if (filasVoto.Count > 0)
                    {
                        sumaVotos += filasVoto.FirstOrDefault().Voto1;
                    }
                }

                return sumaVotos;
            }
        }

        /// <summary>
        /// Comentarios hijo del comentario
        /// </summary>
        public override List<Riam.Interfaces.IElementoGnoss> Hijos
        {
            get
            {
                List<AD.EntityModel.Models.Comentario.Comentario> filas = GestorComentarios.ComentarioDW.ListaComentario.Where(item => item.ComentarioSuperiorID.Equals(Clave) && item.Eliminado.Equals(false)).OrderByDescending(item => item.Fecha).ToList();

                List<Riam.Interfaces.IElementoGnoss> listaHijos = new List<Riam.Interfaces.IElementoGnoss>();

                foreach (AD.EntityModel.Models.Comentario.Comentario fila in filas)
                {
                    listaHijos.Add(GestorComentarios.ListaComentarios[fila.ComentarioID]);
                }

                return listaHijos;
            }
        }

        #endregion

        #region Métodos

        #region Permisos

        /// <summary>
        /// Indica si una identidad tiene permisos de edición sobre el comentario actual.
        /// </summary>
        /// <param name="pIdentidad">Identidad a comprobar</param>
        /// <returns>TRUE si tiene persmisos de edición, FALSE en caso contario</returns>
        public bool TienePermisosEdicionIdentidad(Identidad.Identidad pIdentidad, bool pEsAdministradorDeOrganizacion)
        {
            return FilaComentario.IdentidadID == pIdentidad.Clave || pIdentidad.ListaTodosIdentidadesDeIdentidad.Contains(FilaComentario.IdentidadID) || (pEsAdministradorDeOrganizacion && TipoPerfilCreador != (short)TiposIdentidad.Personal && pIdentidad.OrganizacionID == OrganizacionPerfilCreador);
        }

        /// <summary>
        /// Comprueba si una identidad tiene permisos para eliminar el comentario.
        /// </summary>
        /// <param name="pIdentidad">Identidad a comprobar</param>
        /// <returns>TRUE si tiene persmisos de eliminación, FALSE en caso contario</returns>
        public bool TienePermisosIdentidadEliminar(Identidad.Identidad pIdentidad, Elementos.ServiciosGenerales.Proyecto pProyectoActual, Guid pUsuarioID, bool pEsAdministradorDeOrganizacion)
        {
            return pProyectoActual.EsAdministradorUsuario(pUsuarioID) || TienePermisosEdicionIdentidad(pIdentidad, pEsAdministradorDeOrganizacion);
        }

        #endregion

        #endregion
    }
}
