using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Es.Riam.Gnoss.AD
{
    /// <summary>
    /// Representa un elemento base de datos
    /// </summary>
    public interface IBaseDatos
    {
        #region M�todos generales

        #region M�todos de conversi�n de par�metros para base de datos

        /// <summary>
        /// Devuelve el s�mbolo de concatenaci�n para la base de datos correspondiente
        /// </summary>
        /// <returns>Cadena de texto con el s�mbolo de concatenaci�n</returns>
        string SimboloConcatenar();

        /// <summary>
        /// Captura s�lo el a�o de un campo datetime del servidor de base de datos correspondiente
        /// </summary>
        /// <param name="pCampo">Nombre del campo del que extraer la fecha</param>
        /// <returns>Cadena de texto con el a�o</returns>
        string AnioCampo(string pCampo);

        /// <summary>
        /// Captura la fecha (Dia,Mes,A�o) del servidor de base de datos correspondiente
        /// </summary>
        /// <returns>Cadena de texto con la fecha (Dia,Mes,A�o) del sistema</returns>
        string CapturarFechaSinHora();

        /// <summary>
        /// Captura la fecha (Dia,Mes,A�o) de un campo de tipo Fecha pasado por par�metro
        /// </summary>
        /// <param name="pColumnaFecha">Nombre de la columna que guarda la fecha</param>
        /// <returns>Cadena de texto con la fecha (Dia,Mes,A�o)</returns>
        string CapturarFechaSinHora(string pColumnaFecha);

        /// <summary>
        /// Devuelve un string para usar en la consulta con el formato correcto del valor del Guid 
        /// para el servidor de base de datos correspondiente
        /// </summary>
        /// <param name="pValor">Guid</param>
        /// <returns>Cadena de texto formateada</returns>
        string GuidValor(Guid pValor);

        /// <summary>
        /// Devuelve un string para usar en la consulta con el formato correcto del valor de una lista de Guid 
        /// para el servidor de base de datos correspondiente
        /// </summary>
        /// <param name="pValor">Lista de Guid</param>
        /// <returns>Cadena de texto formateada</returns>
        string ListGuidValor(List<Guid> pValor);

        /// <summary>
        /// Formateado de la consulta que ser� utilizada para las b�squedas en cat�logos de texto
        /// </summary>
        /// <param name="pString">Consulta que ser� utilizada para las b�squedas en cat�logos de texto</param>
        /// <returns>Cadena de texto con la consulta formateada</returns>
        string CriterioBusqueda(string pString);

        /// <summary>
        /// B�squeda de texto avanzada, uso de cat�logos en BD
        /// </summary>
        /// <param name="pNombreTablaColumna">Nombre de la tabla + '.' + columna de texto indexada ("Tabla.Columna")</param>
        /// <param name="pClavePrincipal">Nombre de la columna que es clave principal de la tabla</param>
        /// <param name="pParametro">Nombre del par�metro para despu�s asignar el valor para el criterio de b�squeda</param>
        /// <param name="pConParametro">True si el valor del tag se pasa con un parametro, False si se pasa el valor directamente para la busqueda en el pParametro</param>
        /// <returns>Cadena de texto con la consulta de la b�squeda</returns>
        string Catalogo(string pNombreTablaColumna, string pClavePrincipal , string pParametro, bool pConParametro);

        /// <summary>
        /// Devuelve un string con la cadena de conexi�n
        /// </summary>
        /// <returns>Cadena de conexi�n</returns>
        string ObtenerCadenaConexion();

        /// <summary>
        /// Devuelve la fecha y hora actual del servidor.
        /// </summary>
        /// <param name="pDatabase">Base de datos que se va a consultar</param>
        /// <returns>DateTime</returns>
        DateTime FechaHoraServidor(Database pDatabase);

        /// <summary>
        /// Conversi�n y tratamiento del tipo Guid. Para Uso de DataAdapter
        /// </summary>
        /// <param name="pTipo">Tipo de datos</param>
        /// <returns>Tipo de datos modificado si fuese necesario</returns>
        DbType TipoGuidToObject(DbType pTipo);

        /// <summary>
        /// Conversi�n y tratamiento del tipo Binary. Para Uso de DataAdapter
        /// </summary>
        /// <param name="pTipo">Tipo de datos</param>
        /// <returns>Tipo de datos modificado si fuese necesario</returns>
        DbType TipoBinaryToObject(DbType pTipo);

        /// <summary>
        /// Devuelve una cadena con en formato correcto para que la BD pueda realizar una comparacion de valores para usar en las consultas delete y modify de los dataAdapter
        /// </summary>
        /// <param name="pNombreCampo">Nombre del campo </param>
        /// <param name="pAdmiteNulos">True si admite nulos dicho campo</param>
        /// <returns>Cadena con en formato de una comparacion de valores para usar en las consultas delete y modify de los dataAdapter </returns>
        String ComparacionCamposImagenesConOriginal(String pNombreCampo, bool pAdmiteNulos);

        /// <summary>
        /// Devuelve una cadena con en formato correcto para que la BD pueda realizar una comparacion de valores para usar en las consultas delete y modify de los dataAdapter con campos de texto pesados
        /// </summary>
        /// <param name="pNombreCampo">Nombre del campo </param>
        /// <param name="pAdmiteNulos">True si admite nulos dicho campo</param>
        /// <returns>Cadena con en formato de una comparacion de valores para usar en las consultas delete y modify de los dataAdapter </returns>
        String ComparacionCamposTextoPesadoConOriginal(String pNombreCampo, bool pAdmiteNulos);

        /// <summary>
        /// Conversi�n y tratamiento del tipo Guid. Para Uso de consultas 
        /// </summary>
        /// <param name="pTipo">Tipo de datos</param>
        /// <returns>Tipo de datos Object si estamos con Guids en Oracle</returns>
        DbType TipoGuidToString(DbType pTipo);

        /// <summary>
        /// M�todo para poder seleccionar un campo de tipo Guid en una consulta SELECT
        /// </summary>
        /// <param name="pCampo">Nombre del campo de tipo Guid</param>
        /// <param name="pConAs">True si deseamos renombrar el campo con la clausula "as"</param>
        /// <returns>Cadena de texto formateada</returns>
        string CargarGuid(string pCampo, bool pConAs);

        /// <summary>
        /// M�todo para poder seleccionar un campo de tipo Guid en una consulta SELECT
        /// </summary>
        /// <param name="pCampo">Nombre del campo de tipo Guid</param>
        /// <returns>Cadena de texto formateada</returns>
        string CargarGuid(string pCampo);

        /// <summary>
        /// Convierte los caracteres reservados "@" para el paso de par�metros del Enterprise Library si es necesario
        /// </summary>
        /// <param name="pString">Cadena de texto con la consulta para formatear</param>
        /// <returns>Cadena de texto con formato correcto</returns>
        string ToParam(string pString);

        /// <summary>
        /// Convierte los caracteres reservados "@" y controla los Guid para el paso de par�metros del Enterprise Library 
        /// </summary>
        /// <param name="pValor">Cadena de texto con la consulta para formatear</param>
        /// <returns>Cadena de texto con formato correcto</returns>
        string GuidParamColumnaTabla(string pValor);

        /// <summary>
        /// Convierte los caracteres reservados "@" y controla los Guid para el paso de par�metros 
        /// del Enterprise Library si es necesario, CUANDO EL PAR�METRO SEA LOS VALORES DE UN GUID
        /// </summary>
        /// <param name="pValor">Cadena de texto con la consulta para formatear</param>
        /// <returns>Cadena de texto con formato correcto</returns>
        string GuidParamValor(String pValor);

        /// <summary>
        /// Captura la fecha y hora del servidor de base de datos correspondiente
        /// </summary>
        /// <returns>Cadena de texto con la fecha</returns>
        string CapturarFecha();

        /// <summary>
        /// Formatea el valor del Guid para que sea compatible con la base de datos correspondiente, 
        /// quitando los caracteres del Guid  '{' ,'}','-'
        /// </summary>
        /// <param name="pGuid">Guid</param>
        /// <returns>Guid formateado</returns>
        object ValorDeGuid(Guid pGuid);

        /// <summary>
        /// Formatea un campo de texto en una cadena de caracteres separado por guiones para simular un campo Guid
        /// </summary>
        /// <param name="pCampo">Campo Guid traido como cadena de texto</param>
        /// <returns>Cadena de caracteres separada por guiones simulando un Guid</returns>
        string FormatearGuid(Guid pCampo, bool pAniadirGuiones = false);

        /// <summary>
        /// Obtiene de un Reader abierto y una posicion a leer el valor de un guid (o lo formatea para que devuelva un guid)
        /// </summary>
        /// <param name="pReader">Objeto IDataReader</param>
        /// <param name="pPosicionALeerDelReader">indice del campo que se va a buscar</param>
        /// <returns>Guid</returns>
        Guid ReaderGetGuid(IDataReader pReader, int pPosicionALeerDelReader);

        /// <summary>
        /// Convierte todos los caracteres reservados "@" en funci�n del gestor de base de datos
        /// </summary>
        /// <param name="pValor">Cadena para formatear</param>
        /// <returns>Cadena sustituida</returns>
        string ReplaceParam(string pValor);

        /// <summary>
        /// Devuelve una cadena con la sentencia para desactivar una FK habilitada
        /// </summary>
        /// <param name="pTable">Nombre de la tabla en la BD</param>
        /// <param name="pNombreFK">Nombre de la FK en la BD</param>
        /// <returns>Sentencia SQL para realizar el cambio</returns>
        string DesactivarFK(string pTable, string pNombreFK);

        /// <summary>
        /// Devuelve una cadena con la sentencia para activar una FK desabilitada
        /// </summary>
        /// <param name="pTable">Nombre de la tabla en la BD</param>
        /// <param name="pNombreFK">Nombre de la FK en la BD</param>
        /// <returns>Sentencia SQL para realizar el cambio</returns>
        string ActivarFK(string pTable, string pNombreFK);

        #endregion

        /// <summary>
        /// Crea una base de datos de Enterprise Library
        /// </summary>
        /// <returns>Base de datos</returns>
        Database CrearBDEnterprise();

        /// <summary>
        /// Crea una base de datos de Enterprise Library a partir de un fichero de configuraci�n 
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuraci�n de base de datos</param>
        /// <param name="pUsarVariableEstatica">Indica si se puede usar la variable estatica mIBDEstatica o no</param>
        /// <returns>Base de datos</returns>
        Database CrearBDEnterprise(string pFicheroConfiguracionBD);

        /// <summary>
        /// Crea una base de datos de Enterprise Library a partir de un fichero de configuraci�n 
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuraci�n de base de datos</param>
        /// <param name="pUsarVariableEstatica">Indica si se puede usar la variable estatica mIBDEstatica o no</param>
        /// <param name="pCadenasInvalidas">Lista de cadenas de conexi�n que no han funcionado bien</param>
        /// <returns>Base de datos</returns>
        Database CrearBDEnterprise(string pFicheroConfiguracionBD, List<string> pCadenasInvalidas);

        /// <summary>
        /// Crea un comando de base de datos tipado
        /// </summary>
        /// <param name="pComandoSql">Cadena de texto con la consulta para el comando</param>
        /// <returns>Comandode base de datos</returns>
        DbCommand CrearDbCommand(string pComandoSql);


        /// <summary>
        /// Pagina un dataset
        /// </summary>
        /// <param name="pDatabase">Base de datos que se va a consultar</param>
        /// <param name="pComando">Comando de base de datos con la consulta y par�metros normales</param>
        /// <param name="pOrderBy">Criterio de ordenaci�n de los resultados</param>
        /// <param name="pDataSet">Dataset para paginar</param>
        /// <param name="pInicio">Primera fila que se desea obtener.Si pInicio == -1 y pLimite == -1 se trae TODOS los resultados posibles </param>
        /// <param name="pLimite">�ltima fila que se desea obtener.Si pInicio == -1 y pLimite == -1 se trae TODOS los resultados posibles </param>
        /// <param name="pNombreTabla">Nombre de la tabla que queremos paginar del dataset</param>
        /// <param name="pConsultaLigeraCount">Consulta ligera para hacer el count de la paginacion</param>
        /// <returns>N�mero de registros del resultado</returns>
        int PaginarDataSet(Database pDatabase, DbCommand pComando, string pOrderBy, DataSet pDataSet, int pInicio, int pLimite, string pNombreTabla, bool pContar, string pConsultaLigeraCount);

        /// <summary>
        /// Devuelve un KeyValuePair(consulta count, consulta resultados) con las dos consultas necesarias para realizar la paginaci�n 
        /// (la primera devuelve el numero de resultados totales y la segunda los x resultados seleccionados)
        /// </summary>
        /// <param name="pComando">Comando con la consulta para paginar</param>
        /// <param name="pOrderBy">Criterio de ordenaci�n de los resultados</param>
        /// <param name="pInicio">Primera fila que se desea obtener</param>
        /// <param name="pLimite">�ltima fila que se desea obtener</param>
        /// <returns>KeyValuePair(consulta count, consulta resultados)</returns>
        KeyValuePair<string, string> ObtenerConsultasPaginar(DbCommand pComando, string pOrderBy, int pInicio, int pLimite);

        /// <summary>
        /// Devuelve un KeyValuePair(consulta count, consulta resultados) con las dos consultas necesarias para realizar la paginaci�n 
        /// de una consulta jer�rquica (la primera devuelve el numero de resultados totales y la segunda los x resultados seleccionados)
        /// </summary>
        /// <param name="pComando">Comando con la consulta para paginar</param>
        /// <param name="pConsultaJerarquica">Consulta jer�rquica que vamos a paginar</param>
        /// <param name="pOrderBy">Criterio de ordenaci�n de los resultados</param>
        /// <param name="pInicio">Primera fila que se desea obtener</param>
        /// <param name="pLimite">�ltima fila que se desea obtener</param>
        /// <returns>KeyValuePair(consulta count, consulta resultados)</returns>
        KeyValuePair<string, string> ObtenerConsultasPaginarConsultaJerarquica(DbCommand pComando, IConsultaJerarquica pConsultaJerarquica, string pOrderBy, int pInicio, int pLimite);

        /// <summary>
        /// Pagina un dataset pasado por par�metro
        /// </summary>
        /// <param name="pDatabase">Base de datos que se va a consultar</param>
        /// <param name="pComando">Comando con la consulta para paginar</param>
        /// <param name="pConsultaJerarquica">Consulta jer�rquica</param>
        /// <param name="pOrderBy">Criterio de ordenaci�n de los resultados</param>
        /// <param name="pDataSet">Dataset</param>
        /// <param name="pInicio">Primera fila que se desea obtener</param>
        /// <param name="pLimite">�ltima fila que se desea obtener</param>
        /// <param name="pNombreTabla">Nombre de la tabla del dataset que se quiere paginar</param>
        /// <returns>N�mero de registros del resultado</returns>
        int PaginarDataSetConConsultaJerarquica(Database pDatabase, DbCommand pComando, IConsultaJerarquica pConsultaJerarquica, string pOrderBy, DataSet pDataSet, int pInicio, int pLimite, string pNombreTabla);

        /// <summary>
        /// Crea una consulta jer�rquica
        /// </summary>
        /// <param name="pSelectConsultaGlobal">SELECT de la consulta que va a usar la consulta global</param>
        /// <param name="pFromConsultaGlobal">FROM de la consulta que va a usar la consulta global</param>
        /// <param name="pRestoConsultaGlobal">Resto de la consulta que va a usar la consulta global</param>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en en el FROM de la consulta jer�rquica (NULL si no se necesitan m�s tablas que la tabla de relaci�n)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado (NULL si s�lo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condici�n de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        /// <param name="pIncluirTablaResultadoEnFrom">TRUE si se debe de incluir la tabla de resultados en la consulta global</param>
        /// <returns>Consulta jer�rquica</returns>
        IConsultaJerarquica CrearConsultaJerarquica(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia, bool pIncluirTablaResultadoEnFrom);

        /// <summary>
        /// Crea una consulta jer�rquica de grafo
        /// </summary>
        /// <param name="pSelectConsultaGlobal">SELECT de la consulta que va a usar la consulta global</param>
        /// <param name="pFromConsultaGlobal">FROM de la consulta que va a usar la consulta global</param>
        /// <param name="pRestoConsultaGlobal">Resto de la consulta que va a usar la consulta global</param>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en en el FROM de la consulta jer�rquica (NULL si no se necesitan m�s tablas que la tabla de relaci�n)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado (NULL si s�lo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condici�n de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        /// <param name="pIncluirTablaResultadoEnFrom">TRUE si se debe de incluir la tabla de resultados en la consulta global</param>
        /// <param name="pNombreTablaElementos">Nombre de la tabla de los elementos originales sin jerarqu�a</param>
        /// <param name="pClaveTablaElementos">Clave de la tabla de los elementos originales sin jerarqu�a</param>
        /// <returns>Consulta jer�rquica de grafo</returns>
        IConsultaJerarquica CrearConsultaJerarquicaDeGrafo(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia, bool pIncluirTablaResultadoEnFrom, string pNombreTablaElementos, string pClaveTablaElementos);

        /// <summary>
        /// Crea una consulta jer�rquica m�ltiple
        /// </summary>
        /// <param name="pSelectConsultaGlobal">SELECT de la consulta que va a usar la consulta global</param>
        /// <param name="pFromConsultaGlobal">FROM de la consulta que va a usar la consulta global</param>
        /// <param name="pRestoConsultaGlobal">Resto de la consulta que va a usar la consulta global</param>
        /// <param name="pIncluirTablaResultadoEnFrom">TRUE si se debe de inculir la tabla de resultados en la consulta global</param>
        /// <returns>Consulta jer�rquica m�ltiple</returns>
        IConsultaJerarquicaMultiple CrearConsultaJerarquicaMultiple(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, bool pIncluirTablaResultadoEnFrom);

        /// <summary>
        /// Obtiene la cl�usula FROM de una consulta sin tablas
        /// </summary>
        /// <returns>La cl�usula FROM sin ninguna tabla</returns>
        string ObtenerFromSinTablas();

        /// <summary>
        /// Escribe SQLServer2005--> [key] , Oracle --> Key
        /// </summary>
        /// <returns>[Key] o Key</returns>
        string Key();

        /// <summary>
        /// Escribe SQLServer2005--> ISNULL() , Oracle --> NVL()
        /// </summary>
        /// <param name="pCampo">Campo que devuelve</param>
        /// <param name="pValorPorDefecto">Valor que devuelve si es null</param>
        /// <returns></returns>
        string ISNULL(string pCampo, string pValorPorDefecto);

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la lista de conexiones con credenciales para cada conexion SIN credenciales
        /// </summary>
        ConcurrentDictionary<string, string> ListaConexionesConCredenciales
        {
            get;
        }

        string Concatenador { get; }

        string CadenaVacia { get; }

        #endregion
    }

    /// <summary>
    /// Interfaz que representa una consulta jer�rquica
    /// </summary>
    public interface IConsultaJerarquica
    {
        #region Propiedades

        /// <summary>
        /// Obtiene la consulta jer�rquica
        /// </summary>
        string ConsultaJerarquica
        {
            get;
        }

        #endregion
    }

    /// <summary>
    /// Consulta jer�rquica con varias relaciones jer�rquicas
    /// </summary>
    public interface IConsultaJerarquicaMultiple
    {
        #region Propiedades

        /// <summary>
        /// Obtiene la consulta jer�rquica
        /// </summary>
        string ConsultaJerarquicaMultiple
        {
            get;
        }

        #endregion 

        #region M�todos

        /// <summary>
        /// Agrega una consulta jer�rquica
        /// </summary>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en en el FROM de la consulta jer�rquica (NULL si no se necesitan m�s tablas que la tabla de relaci�n)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado (NULL si s�lo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condici�n de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        void AgregarConsultaJerarquica(string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia);

        /// <summary>
        /// Agrega una consulta jer�rquica de grafo
        /// </summary>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en el FROM de la consulta jer�rquica (NULL si no se necesitan m�s tablas que la tabla de relaci�n)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado (NULL si s�lo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condici�n de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        /// <param name="pEsGrafo">TRUE si la consulta jer�rquica es un grafo (no un �rbol)</param>
        /// <param name="pNombreTablaElementos">Nombre de la tabla de los elementos originales sin jerarqu�a</param>
        /// <param name="pClaveTablaElementos">Clave de la tabla de los elementos originales sin jerarqu�a</param>
        void AgregarConsultaJerarquicaDeGrafo(string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia, bool pEsGrafo, string pNombreTablaElementos, string pClaveTablaElementos);

        #endregion
    }

    /// <summary>
    /// Estructura para a�adir criterios de orden para la paginaci�n
    /// </summary>
    public struct CriterioPaginacion
    {
        /// <summary>
        /// Nombre de columna
        /// </summary>
        public string mNombreColumna;

        /// <summary>
        /// Ascendiente
        /// </summary>
        public bool mAscendiente;
    }
}
