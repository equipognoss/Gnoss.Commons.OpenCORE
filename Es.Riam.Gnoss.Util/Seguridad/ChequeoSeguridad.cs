using Es.Riam.Gnoss.Util.General;
using System;

namespace Es.Riam.Gnoss.Util.Seguridad
{
    /// <summary>
    /// Clase para el control de autorizaciones
    /// </summary>
    public class ChequeoSeguridad
	{
		private readonly GnossIdentity _usuarioActual;

		public ChequeoSeguridad(GnossIdentity usuarioActual)
        {
			_usuarioActual = usuarioActual;
		}
		#region Anónimos

		/// <summary>
		/// No se necesitan permisos
		/// </summary>
		public static void Anonimos()
		{
			// No es necesario chequear nada
		}

		#endregion

		#region Generales

		/// <summary>
		/// Comprueba las autorizaciones de servicios o capacidades generales para el usuario actual
		/// </summary>
		/// <param name="pCapacidad">Capacidad para autorizar</param>
		public void ComprobarAutorizacion(System.UInt64 pCapacidad)
		{
			//Si no está autenticado
			if (!_usuarioActual.IsAuthenticated)
			{
				throw new ErrorAccesoDenegado();
			}

			//Si no está autorizado
			if (!_usuarioActual.EstaAutorizado(pCapacidad))
			{
				throw new ErrorAccesoDenegado();
			}
		}

		/// <summary>
		/// Comprueba una capacidad general para el usuario actual
		/// </summary>
		/// <param name="pCapacidad">Capacidad para comprobar</param>
		public bool ComprobarCapacidad(System.UInt64 pCapacidad)
		{

			//Si no está autenticado
			if (!_usuarioActual.IsAuthenticated)
			{
				throw new ErrorAccesoDenegado();
			}

			//Si no está autorizado
			return _usuarioActual.EstaAutorizado(pCapacidad);
		}

		#endregion

		#region Proyecto

		/// <summary>
		/// Comprueba las autorizaciones de servicios o capacidades para el usuario actual en el proyecto actual
		/// </summary>
		/// <param name="pCapacidad">Capacidad para autorizar</param>
		public bool ComprobarCapacidadEnProyecto(System.UInt64 pCapacidad)
		{

			//Si no está autenticado
			if (!_usuarioActual.IsAuthenticated)
			{
				throw new ErrorAccesoDenegado();
			}

			//Si no está autorizado en el proyecto
			return _usuarioActual.EstaAutorizadoEnProyecto(pCapacidad);
		}

		/// <summary>
		/// Comprueba las autorizaciones de servicios o capacidades para el usuario actual en el proyecto actual
		/// </summary>
		/// <param name="pCapacidad">Capacidad para autorizar</param>
		public void ComprobarAutorizacionEnProyecto(System.UInt64 pCapacidad)
		{

			//Si no está autenticado
			if (!_usuarioActual.IsAuthenticated)
			{
				throw new ErrorAccesoDenegado();
			}

			//Si no está autorizado en el proyecto           
			if (!_usuarioActual.EstaAutorizadoEnProyecto(pCapacidad))
			{
				throw new ErrorAccesoDenegado();
			}
		}

		#endregion

		#region Organización

		/// <summary>
		/// Comprueba las autorizaciones de servicios o capacidades para el usuario actual en la organizacion pasada por parámetro
		/// </summary>
		/// <param name="pCapacidad">Capacidad para autorizar</param>
		/// <param name="pOrganizacionID">OrganizacionID a comprobar capacidad</param>
		public bool ComprobarCapacidadEnOrganizacion(System.UInt64 pCapacidad, Guid pOrganizacionID)
		{
			//IPrincipal principal = Thread.CurrentPrincipal;

			//Si no está autenticado
			if (!_usuarioActual.IsAuthenticated)
			{
				throw new ErrorAccesoDenegado();
			}

			//Si no está autorizado en la organizacion
			return _usuarioActual.EstaAutorizadoEnOrganizacion(pCapacidad, pOrganizacionID);
		}

		/// <summary>
		/// Comprueba las autorizaciones de servicios o capacidades para el usuario actual en la organizacion pasada por parámetro
		/// </summary>
		/// <param name="pCapacidad">Capacidad para autorizar</param>
		/// <param name="pOrganizacionID">OrganizacionID a comprobar capacidad</param>
		public void ComprobarAutorizacionEnOrganizacion(System.UInt64 pCapacidad, Guid pOrganizacionID)
		{
			//IPrincipal principal = Thread.CurrentPrincipal;

			//Si no está autenticado
			if (!_usuarioActual.IsAuthenticated)
			{
				throw new ErrorAccesoDenegado();
			}

			//Si no está autorizado en la  organizacion           
			if (!_usuarioActual.EstaAutorizadoEnOrganizacion(pCapacidad, pOrganizacionID))
			{
				throw new ErrorAccesoDenegado();
			}
		}

		#endregion

		/// <summary>
		/// Comprueba que sea el propio usuario quien consulta cierta información propia, en este caso está identificado
		/// significa que se ha identificado el login, falta la password
		/// </summary>
		/// <param name="pUsuarioID">Identificador del usuario</param>
		/// <param name="pLogin">Login del usuario</param>
		public void PropioUsuarioIdentificado(System.Guid pUsuarioID, string pLogin)
		{
			//IPrincipal principal = Thread.CurrentPrincipal;


			//Si no se ha identificado el login
			if (!_usuarioActual.EstaLoginIdentificado)
			{
				throw new ErrorAccesoDenegado();
			}

			//Si no es el propio usuario
			if ((_usuarioActual.UsuarioID.CompareTo(pUsuarioID) != 0) || (_usuarioActual.Login.CompareTo(pLogin) != 0))
			{
				throw new ErrorAccesoDenegado();
			}
		}

		/// <summary>
		/// Comprueba que sea el propio usuario quien consulta cierta información propia, en este caso está totalmente
		/// autenticado, significa que se ha autenticado el login y la password
		/// </summary>
		/// <param name="pUsuarioID">Identificador del usuario</param>
		/// <param name="pLogin">Login del usuario</param>
		public void PropioUsuarioAutenticado(System.Guid pUsuarioID, string pLogin)
		{
			//IPrincipal principal = Thread.CurrentPrincipal;

			//Si no está completamente autenticado
			if (!_usuarioActual.IsAuthenticated)
			{
				throw new ErrorAccesoDenegado();
			}

			//Si no es el propio usuario
			if ((_usuarioActual.UsuarioID.CompareTo(pUsuarioID) != 0) ||
				(_usuarioActual.Login.CompareTo(pLogin) != 0))
			{
				throw new ErrorAccesoDenegado();
			}
		}

		/// <summary>
		/// Comprueba que sea el propio usuario quien consulta cierta información propia, en este caso está totalmente
		/// autenticado, significa que se ha autenticado el login y la password
		/// Se comprueba que la persona y organización correspondan con las de sesión de usuario
		/// </summary>
		/// <param name="pOrganizacionID">Identificador de la organización de sesión de usuario</param>
		/// <param name="pPersonaID">Identificador de la persona de sesión de usuario</param>
		public void PropioUsuarioAutenticado(System.Guid pOrganizacionID, Guid pPersonaID)
		{

			//Si no está completamente autenticado
			if (!_usuarioActual.IsAuthenticated)
			{
				throw new ErrorAccesoDenegado();
			}

			//Si no es la persona de sesión del propio usuario
			if ((_usuarioActual.OrganizacionID.CompareTo(pOrganizacionID) != 0) ||
				(_usuarioActual.PersonaID.CompareTo(pPersonaID) != 0))
			{
				throw new ErrorAccesoDenegado();
			}
		}
	}
}
