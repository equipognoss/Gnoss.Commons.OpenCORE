using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;
using Remotion.Linq.Parsing;
using Serilog.Core;
using System;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.ServiciosGenerales
{
    /// <summary>
    /// Usuario de Gnoss
    /// </summary>
    public class UsuarioGnoss : ElementoGnoss, IDisposable
    {
        #region Miembros

        /// <summary>
        /// TRUE si se han cargado todos los registros que dependen de usuario
        /// </summary>
        private bool mRegistrosDependientesUsuarioCargados = false;

        #endregion

        #region Constructores

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pUsuarioGnoss">Fila de usuario</param>
        /// <param name="pGestionUsuarios">Gestor de usuarios</param>
        public UsuarioGnoss(AD.EntityModel.Models.UsuarioDS.Usuario pUsuarioGnoss, GestionUsuarios pGestionUsuarios)
            : base(pUsuarioGnoss, pGestionUsuarios)
        {
        }

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public UsuarioGnoss()
            : base()
        {
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece si se han cargado todos los registros que dependen de usuario
        /// </summary>
        public bool RegistrosDependientesUsuarioCargados
        {
            get
            {
                return mRegistrosDependientesUsuarioCargados;
            }
            set
            {
                mRegistrosDependientesUsuarioCargados = value;
            }
        }

        /// <summary>
        /// Obtiene la fila del usuario Gnoss
        /// </summary>
        public AD.EntityModel.Models.UsuarioDS.Usuario FilaUsuario
        {
            get
            {
                return (AD.EntityModel.Models.UsuarioDS.Usuario)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Obtiene o establece el login del usuario
        /// </summary>
        public string Login
        {
            get
            {
                return FilaUsuario.Login;
            }
            set
            {
                if (!FilaUsuario.Login.Equals(value))
                    FilaUsuario.Login = value;
            }
        }

        /// <summary>
        /// Obtiene el identificador o clave del usuario
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaUsuario.UsuarioID;
            }
        }

        /// <summary>
        /// Obtiene el gestor de usuarios
        /// </summary>
        public GestionUsuarios GestorUsuarios
        {
            get
            {
                return (GestionUsuarios)GestorGnoss;
            }
        }

        /// <summary>
        /// Devuelve el identificador de la base de recursos de un usuario
        /// </summary>
        public Guid BaseRecursosID
        {
            get
            {
                if (GestorUsuarios.GestorDocumental != null && GestorUsuarios.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Where(item => item.UsuarioID.Equals(Clave)).Count() > 0)
                    return GestorUsuarios.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosUsuario.Where(item => item.UsuarioID.Equals(Clave)).FirstOrDefault().BaseRecursosID;
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Devuelve el identificador del tesauro personal de un usuario
        /// </summary>
        public Guid TesauroID
        {
            get
            {
                if (GestorUsuarios.GestorTesauro != null && GestorUsuarios.GestorTesauro.TesauroDW.ListaTesauroUsuario.Where(item => item.UsuarioID.Equals(Clave)).Count() > 0)
                {
                    return GestorUsuarios.GestorTesauro.TesauroDW.ListaTesauroUsuario.Where(item => item.UsuarioID.Equals(Clave)).FirstOrDefault().TesauroID;
                }
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Obtiene el identificador de la identidad pública actual
        /// </summary>
        public Guid IdentidadPublicaID
        {
            get
            {
                return this.GestorUsuarios.DataWrapperUsuario.ListaProyectoUsuarioIdentidad.FirstOrDefault(item => item.UsuarioID.Equals(this.FilaUsuario.UsuarioID) && item.OrganizacionGnossID.Equals(ProyectoAD.MetaOrganizacion) && item.ProyectoID.Equals(ProyectoAD.MetaProyecto)).IdentidadID;
            }
        }

        /// <summary>
        /// Obtiene o establece si el usuario está bloqueado
        /// </summary>
        public bool EstaBloqueado
        {
            get
            {
                return FilaUsuario.EstaBloqueado.Value;
            }
            set
            {
                if (!FilaUsuario.Equals(value))
                    FilaUsuario.EstaBloqueado = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la contraseña del usuario YA ENCRIPTADA
        /// </summary>
        public string Password
        {
            get
            {
                return FilaUsuario.Password;
            }
            set
            {
                bool usar256 = false;

                if (FilaUsuario.Version.HasValue && FilaUsuario.Version.Value.Equals(1))
                {
                    usar256 = true;
                }

                if (!value.Equals(string.Empty) && !HashHelper.CalcularHash(value, usar256).Equals(FilaUsuario.Password))
                    FilaUsuario.Password = HashHelper.CalcularHash(value, true);
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~UsuarioGnoss()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                try
                {
                    if (disposing)
                    {
                        //Libero todos los recursos administrados que he añadido a esta clase
                    }
                    //Libero todos los recursos nativos sin administrar que he añadido a esta clase
                }
                catch (Exception e)
                {
                    throw;
                }
                finally
                {
                    // Llamo al dispose de la clase base
                    base.Dispose(disposing);
                }
            }
        }

        #endregion
    }
}
