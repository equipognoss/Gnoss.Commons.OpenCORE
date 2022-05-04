using System;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Text;

namespace Es.Riam.Gnoss.Util.General
{
    /// <summary>
	/// Error por datos no válidos
	/// </summary>
	[Serializable]
    public class ErrorDatoNoValido : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorDatoNoValido() : this((Exception)null) { }

        /// <summary>
        /// Constructor a partir de la descripción
        /// </summary>
        /// <param name="pDescripcion">Descripción de la excepción</param>
        public ErrorDatoNoValido(string pDescripcion) : this(pDescripcion, null) { }

        /// <summary>
        /// Constructor a partir de la excepción interna pasada por parámetro
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ErrorDatoNoValido(Exception pInner) : this("El dato no es válido.", pInner) { }

        /// <summary>
        /// Constructor a partir de la descripción y de la excepción interna pasadas por parámetro
        /// </summary>
        /// <param name="pDescripcion">Descripción de la excepción</param>
        /// <param name="pInner">Excepción interna</param>
        public ErrorDatoNoValido(string pDescripcion, Exception pInner) : base(pDescripcion, pInner) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorDatoNoValido(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// Error semántico
    /// </summary>
    [Serializable]
    public class ErrorSemantico : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorSemantico() : this((Exception)null) { }

        /// <summary>
        /// Constructor a partir de la descripción pasada por parámetro
        /// </summary>
        /// <param name="pDescripcion">Descripción de la excepción</param>
        public ErrorSemantico(string pDescripcion) : this(pDescripcion, null) { }

        /// <summary>
        /// Constructor a partir de la excepción interna pasada por parámetro
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ErrorSemantico(Exception pInner) : this("No se respeta la Ontología de Gnoss.", pInner) { }

        /// <summary>
        /// Constructor a partir de la descripción y la excepción pasadas por parámetro
        /// </summary>
        /// <param name="pDescripcion">Descripción de la excepción</param>
        /// <param name="pInner">Excepción interna</param>
        public ErrorSemantico(string pDescripcion, Exception pInner) : base(pDescripcion, pInner) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorSemantico(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// El elemento referenciado en la operación no existe
    /// </summary>
    [Serializable]
    public class ErrorElementoNoExiste : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorElementoNoExiste() : this(null) { }

        /// <summary>
        /// Constructor a partir de la excepción interna pasada por parámetro
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ErrorElementoNoExiste(Exception pInner) : base("El elemento referenciado en la operación no existe", pInner) { }

        /// <summary>
        /// Constructor a partir de la información y el contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorElementoNoExiste(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// Usuario incorrecto
    /// </summary>
    [Serializable]
    public class ErrorLoginUsuario : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorLoginUsuario() : this(null) { }

        //public ErrorLoginUsuario(Exception inner) : base("El usuario especificado no existe", inner) {}

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ErrorLoginUsuario(Exception pInner) : base("Nombre de usuario o contraseña incorrectos", pInner) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorLoginUsuario(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// Consulta SQL o SPARQL mal construida
    /// </summary>
    [Serializable]
    public class ExcepcionDeBaseDeDatos : SystemException
    {
        #region Constructores

        public string Query { get; set; }
        public string BDException { get; set; }
        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ExcepcionDeBaseDeDatos() : base(null) { }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ExcepcionDeBaseDeDatos(Exception pInner)
            : base("La consulta no está construida correctamente. ", pInner) { }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        /// <param name="pQuery">Consulta de base de datos que ha fallado</param>
        public ExcepcionDeBaseDeDatos(string pQuery, Exception pInner)
            : base("Error con la consulta: \r\n" + pQuery + "\r\n\r\nError original: " + pInner.Message + "\r\n\r\nPilaOriginal: " + pInner.StackTrace + "\r\n\r\n", pInner) { }
        public ExcepcionDeBaseDeDatos(string pQuery, string pBDException, Exception pInner)
           : base("Error con la consulta: \r\n" + pQuery + "\r\n\r\nError original: " + pInner.Message + "\r\n\r\nPilaOriginal: " + pInner.StackTrace + "\r\n\r\n", pInner)
        {
            Query = pQuery;
            BDException = pBDException;
        }
        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        /// <param name="pComando">Comando que ha producido el error</param>
        public ExcepcionDeBaseDeDatos(DbCommand pComando, Exception pInner)
            : base("Error con la consulta: \r\n" + pComando.CommandText + ParametrosAString(pComando) + "\r\n\r\nError original: " + pInner.Message + "\r\n\r\nPilaOriginal: " + pInner.StackTrace + "\r\n\r\n", pInner)
        {

        }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ExcepcionDeBaseDeDatos(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        /// <summary>
        /// Constructor a partir de una excepción con mensaje.
        /// </summary>
        /// <param name="pError">Excepción interna</param>
        public ExcepcionDeBaseDeDatos(string pError)
            : base(pError) { }

        #endregion

        #region Metodos estáticos

        private static string ParametrosAString(DbCommand pComando)
        {
            StringBuilder mensaje = new StringBuilder();

            if (pComando.Parameters != null && pComando.Parameters.Count > 0 && pComando.Parameters[0].Value != null)
            {
                mensaje.AppendLine("Parámetros: ");
                foreach (DbParameter parametro in pComando.Parameters)
                {
                    mensaje.AppendLine(parametro.ParameterName + ": " + parametro.Value.ToString());
                }
            }
            return mensaje.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Consulta SQL o SPARQL mal construida
    /// </summary>
    [Serializable]
    public class ExcepcionConectionFailVirtuoso : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ExcepcionConectionFailVirtuoso() : base("Error al intentar ejecutar la consulta, Virtuoso ha fallado") { }

        public ExcepcionConectionFailVirtuoso(string pMessage) : base(pMessage) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ExcepcionConectionFailVirtuoso(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// Consulta SQL o SPARQL mal construida
    /// </summary>
    [Serializable]
    public class ExcepcionCheckpointVirtuoso : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ExcepcionCheckpointVirtuoso() : base("Error al intentar ejecutar la consulta, Virtuoso está haciendo un Checkpoint") { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ExcepcionCheckpointVirtuoso(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// Consulta SQL o SPARQL mal construida
    /// </summary>
    [Serializable]
    public class ExcepcionDeReplicacion : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ExcepcionDeReplicacion() : this("Error de replicación. ") { }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ExcepcionDeReplicacion(Exception pInner)
            : base("Error de replicación. ", pInner) { }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        /// <param name="pMensaje">Mensaje que se va a incrustar en la excepción</param>
        public ExcepcionDeReplicacion(string pMensaje, Exception pInner)
            : base(pMensaje + "\r\n\r\nExcepcionOriginal: \r\n" + pInner.Message + "\r\nPila: " + pInner.StackTrace, pInner) { }

        /// <summary>
        /// Constructor a partir de un mensaje
        /// </summary>
        /// <param name="pMensaje">Mensaje que se va a incrustar en la excepción</param>
        public ExcepcionDeReplicacion(string pMensaje)
            : base(pMensaje) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ExcepcionDeReplicacion(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// Consulta SQL o SPARQL mal construida
    /// </summary>
    [Serializable]
    public class ExcepcionWeb : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ExcepcionWeb() : this("Error en la petición Web. ") { }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ExcepcionWeb(Exception pInner)
            : base("Error en la petición Web. ", pInner) { }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        /// <param name="pMensaje">Mensaje que se va a incrustar en la excepción</param>
        public ExcepcionWeb(string pMensaje, Exception pInner)
            : base(pMensaje + "\r\n\r\nExcepcionOriginal: \r\n" + pInner.Message + "\r\nPila: " + pInner.StackTrace, pInner) { }

        /// <summary>
        /// Constructor a partir de un mensaje
        /// </summary>
        /// <param name="pMensaje">Mensaje que se va a incrustar en la excepción</param>
        public ExcepcionWeb(string pMensaje)
            : base(pMensaje) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ExcepcionWeb(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// Password incorrecta
    /// </summary>
    [Serializable]
    public class ErrorPassword : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor vacío
        /// </summary>
        public ErrorPassword() : this(null) { }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ErrorPassword(Exception pInner) : base("Nombre de usuario o contraseña incorrectos", pInner) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorPassword(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// Acceso bloqueado al proyecto
    /// </summary>
    [Serializable]
    public class ErrorAccesoProyecto : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor a partir del nombre de proyecto pasado por parámetro
        /// </summary>
        /// <param name="pNombreProyecto">Nombre del proyecto</param>
        public ErrorAccesoProyecto(string pNombreProyecto)
        {
        }

        /// <summary>
        /// Constructor a partir del nombre de proyecto y la excepción interna pasados por parámetro
        /// </summary>
        /// <param name="pNombreProyecto">Nombre del proyecto</param>
        /// <param name="pInner">Excepción interna</param>
        public ErrorAccesoProyecto(string pNombreProyecto, Exception pInner)
            : base("El usuario se encuentra bloqueado en el proyecto " + pNombreProyecto + "." + "\n\n" + "(Proyecto al que intenta conectarse actualmente)" + "\n\n" +
            "Pongase en contacto con un administrador de dicho proyecto para desbloquear su cuenta." + "\n\n" + "A continuación seleccione si le es posible otro proyecto al que conectarse.", pInner)
        {
        }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorAccesoProyecto(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext)
        {
        }

        #endregion
    }

    /// <summary>
    /// Usuario sin persona
    /// </summary>
    [Serializable]
    public class ErrorUsuarioSinPersona : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorUsuarioSinPersona() : this(null) { }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ErrorUsuarioSinPersona(Exception pInner) : base("El usuario no tiene ninguna persona asignada", pInner) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorUsuarioSinPersona(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// Usuario bloqueado
    /// </summary>
    [Serializable]
    public class ErrorUsuarioBloqueado : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorUsuarioBloqueado() : this(null) { }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ErrorUsuarioBloqueado(Exception pInner) : base("El usuario se encuentra bloqueado por motivos de seguridad." +
            "\nPongase en contacto con un administrador para desbloquear la cuenta", pInner)
        { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorUsuarioBloqueado(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// El usuario no tiene permiso para efectuar la operación
    /// </summary>
    [Serializable]
    public class ErrorAccesoDenegado : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorAccesoDenegado() : this(null) { }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ErrorAccesoDenegado(Exception pInner) : base("El usuario no tiene permiso para efectuar la operación", pInner) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorAccesoDenegado(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// La operación no puede realizarse
    /// </summary>
    [Serializable]
    public class ErrorOperacionDenegada : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorOperacionDenegada() : this((Exception)null) { }

        /// <summary>
        /// Constructor a partir de la descripción
        /// </summary>
        /// <param name="pDescripcion">Descripción de la excepción</param>
        public ErrorOperacionDenegada(string pDescripcion) : this(pDescripcion, null) { }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ErrorOperacionDenegada(Exception pInner) : this("El estado de la entidad no permite realizar la operación.", pInner) { }

        /// <summary>
        /// Constructor a partir de la descripción y de la excepción interna pasadas por parámetro
        /// </summary>
        /// <param name="pDescripcion">Descripción de la excepción</param>
        /// <param name="pInner">Excepción interna</param>
        public ErrorOperacionDenegada(string pDescripcion, Exception pInner) : base(pDescripcion, pInner) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorOperacionDenegada(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// Se ha producido un error de concurrencia. Vuelva a intentarlo más tarde.
    /// </summary>
    [Serializable]
    public class ErrorConcurrencia : SystemException
    {
        #region Miembros

        private DataRow mFilaAfectada;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorConcurrencia() : this(new Exception()) { }

        /// <summary>
        /// Constructor a partir de la fila pasada por parámetro
        /// </summary>
        /// <param name="pDr">Fila de datos</param>
        public ErrorConcurrencia(DataRow pDr) : this(new Exception())
        {
            FilaAfectada = pDr;
        }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ErrorConcurrencia(Exception pInner) : base("Se ha producido un error de concurrencia. No se ha podido guardar. Para poder guardar debe volver a cargar los datos. ¿Desea recargar la pantalla ahora?", pInner) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorConcurrencia(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece la fila afectada
        /// </summary>
        public DataRow FilaAfectada
        {
            get { return mFilaAfectada; }

            set { mFilaAfectada = value; }
        }

        #endregion
    }

    /// <summary>
    /// Excepción no controlada
    /// </summary>
    [Serializable]
    public class ErrorInterno : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorInterno() : this(null) { }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ErrorInterno(Exception pInner) : base("Error interno de la aplicación", pInner) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorInterno(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// Excepción del fichero de configuración
    /// </summary>
    [Serializable]
    public class ErrorFicheroConfig : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorFicheroConfig() : this(null) { }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ErrorFicheroConfig(Exception pInner) : base("Fichero de configuración no válido", pInner) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorFicheroConfig(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    /// <summary>
    /// Excepción de eliminación no válida
    /// </summary>
    [Serializable]
    public class ErrorEliminacionNoValida : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorEliminacionNoValida() : base() { }

        /// <summary>
        /// Constructor a partir de un texto pasado por parámetro
        /// </summary>
        /// <param name="pTexto">Texto</param>
        public ErrorEliminacionNoValida(string pTexto) : base(pTexto)
        {
        }

        #endregion
    }

    /// <summary>
    /// Excepción de eliminación no válida
    /// </summary>
    [Serializable]
    public class ErrorEdicionNoValida : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorEdicionNoValida() : base() { }

        /// <summary>
        /// Constructor a partir de un texto pasado por parámetro
        /// </summary>
        /// <param name="pTexto">Texto</param>
        public ErrorEdicionNoValida(string pTexto) : base(pTexto)
        {
        }

        #endregion
    }

    /// <summary>
    /// Excepción usada en RabbitMQ cuando el evento esta bloqueando.
    /// </summary>
    [Serializable]
    public class ErrorEventoBloqueante : SystemException
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ErrorEventoBloqueante() : this("El evento no se puede tratar") { }

        public ErrorEventoBloqueante(string message)
        : base(message)
        {
        }

        /// <summary>
        /// Constructor a partir de una excepción interna
        /// </summary>
        /// <param name="pInner">Excepción interna</param>
        public ErrorEventoBloqueante(Exception pInner) : base("El evento no se puede tratar", pInner) { }

        public ErrorEventoBloqueante(string pMensaje, Exception pInner) : base(pMensaje, pInner) { }

        /// <summary>
        /// Constructor a partir de información y contexto pasados por parámetro
        /// </summary>
        /// <param name="pInfo">Información</param>
        /// <param name="pContext">Contexto</param>
        protected ErrorEventoBloqueante(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }

        #endregion
    }

    [Serializable]
    public class ExcepcionGeneral : SystemException
    {
        public ExcepcionGeneral(string message)
        : base(message)
        {

        }
        public ExcepcionGeneral(string pMensaje, Exception pInner) : base(pMensaje, pInner) { }

        protected ExcepcionGeneral(SerializationInfo pInfo, StreamingContext pContext) : base(pInfo, pContext) { }
    }
}
