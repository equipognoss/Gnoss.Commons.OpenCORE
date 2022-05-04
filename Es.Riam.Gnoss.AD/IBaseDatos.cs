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
        #region Métodos generales

        #region Métodos de conversión de parámetros para base de datos

        /// <summary>
        /// Devuelve el símbolo de concatenación para la base de datos correspondiente
        /// </summary>
        /// <returns>Cadena de texto con el símbolo de concatenación</returns>
        string SimboloConcatenar();

        /// <summary>
        /// Captura sólo el año de un campo datetime del servidor de base de datos correspondiente
        /// </summary>
        /// <param name="pCampo">Nombre del campo del que extraer la fecha</param>
        /// <returns>Cadena de texto con el año</returns>
        string AnioCampo(string pCampo);

        /// <summary>
        /// Captura la fecha (Dia,Mes,Año) del servidor de base de datos correspondiente
        /// </summary>
        /// <returns>Cadena de texto con la fecha (Dia,Mes,Año) del sistema</returns>
        string CapturarFechaSinHora();

        /// <summary>
        /// Captura la fecha (Dia,Mes,Año) de un campo de tipo Fecha pasado por parámetro
        /// </summary>
        /// <param name="pColumnaFecha">Nombre de la columna que guarda la fecha</param>
        /// <returns>Cadena de texto con la fecha (Dia,Mes,Año)</returns>
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
        /// Formateado de la consulta que será utilizada para las búsquedas en catálogos de texto
        /// </summary>
        /// <param name="pString">Consulta que será utilizada para las búsquedas en catálogos de texto</param>
        /// <returns>Cadena de texto con la consulta formateada</returns>
        string CriterioBusqueda(string pString);

        /// <summary>
        /// Búsqueda de texto avanzada, uso de catálogos en BD
        /// </summary>
        /// <param name="pNombreTablaColumna">Nombre de la tabla + '.' + columna de texto indexada ("Tabla.Columna")</param>
        /// <param name="pClavePrincipal">Nombre de la columna que es clave principal de la tabla</param>
        /// <param name="pParametro">Nombre del parámetro para después asignar el valor para el criterio de búsqueda</param>
        /// <param name="pConParametro">True si el valor del tag se pasa con un parametro, False si se pasa el valor directamente para la busqueda en el pParametro</param>
        /// <returns>Cadena de texto con la consulta de la búsqueda</returns>
        string Catalogo(string pNombreTablaColumna, string pClavePrincipal , string pParametro, bool pConParametro);

        /// <summary>
        /// Devuelve un string con la cadena de conexión
        /// </summary>
        /// <returns>Cadena de conexión</returns>
        string ObtenerCadenaConexion();

        /// <summary>
        /// Devuelve la fecha y hora actual del servidor.
        /// </summary>
        /// <param name="pDatabase">Base de datos que se va a consultar</param>
        /// <returns>DateTime</returns>
        DateTime FechaHoraServidor(Database pDatabase);

        /// <summary>
        /// Conversión y tratamiento del tipo Guid. Para Uso de DataAdapter
        /// </summary>
        /// <param name="pTipo">Tipo de datos</param>
        /// <returns>Tipo de datos modificado si fuese necesario</returns>
        DbType TipoGuidToObject(DbType pTipo);

        /// <summary>
        /// Conversión y tratamiento del tipo Binary. Para Uso de DataAdapter
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
        /// Conversión y tratamiento del tipo Guid. Para Uso de consultas 
        /// </summary>
        /// <param name="pTipo">Tipo de datos</param>
        /// <returns>Tipo de datos Object si estamos con Guids en Oracle</returns>
        DbType TipoGuidToString(DbType pTipo);

        /// <summary>
        /// Método para poder seleccionar un campo de tipo Guid en una consulta SELECT
        /// </summary>
        /// <param name="pCampo">Nombre del campo de tipo Guid</param>
        /// <param name="pConAs">True si deseamos renombrar el campo con la clausula "as"</param>
        /// <returns>Cadena de texto formateada</returns>
        string CargarGuid(string pCampo, bool pConAs);

        /// <summary>
        /// Método para poder seleccionar un campo de tipo Guid en una consulta SELECT
        /// </summary>
        /// <param name="pCampo">Nombre del campo de tipo Guid</param>
        /// <returns>Cadena de texto formateada</returns>
        string CargarGuid(string pCampo);

        /// <summary>
        /// Convierte los caracteres reservados "@" para el paso de parámetros del Enterprise Library si es necesario
        /// </summary>
        /// <param name="pString">Cadena de texto con la consulta para formatear</param>
        /// <returns>Cadena de texto con formato correcto</returns>
        string ToParam(string pString);

        /// <summary>
        /// Convierte los caracteres reservados "@" y controla los Guid para el paso de parámetros del Enterprise Library 
        /// </summary>
        /// <param name="pValor">Cadena de texto con la consulta para formatear</param>
        /// <returns>Cadena de texto con formato correcto</returns>
        string GuidParamColumnaTabla(string pValor);

        /// <summary>
        /// Convierte los caracteres reservados "@" y controla los Guid para el paso de parámetros 
        /// del Enterprise Library si es necesario, CUANDO EL PARÁMETRO SEA LOS VALORES DE UN GUID
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
        /// Convierte todos los caracteres reservados "@" en función del gestor de base de datos
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
        /// Crea una base de datos de Enterprise Library a partir de un fichero de configuración 
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Indica si se puede usar la variable estatica mIBDEstatica o no</param>
        /// <returns>Base de datos</returns>
        Database CrearBDEnterprise(string pFicheroConfiguracionBD);

        /// <summary>
        /// Crea una base de datos de Enterprise Library a partir de un fichero de configuración 
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Indica si se puede usar la variable estatica mIBDEstatica o no</param>
        /// <param name="pCadenasInvalidas">Lista de cadenas de conexión que no han funcionado bien</param>
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
        /// <param name="pComando">Comando de base de datos con la consulta y parámetros normales</param>
        /// <param name="pOrderBy">Criterio de ordenación de los resultados</param>
        /// <param name="pDataSet">Dataset para paginar</param>
        /// <param name="pInicio">Primera fila que se desea obtener.Si pInicio == -1 y pLimite == -1 se trae TODOS los resultados posibles </param>
        /// <param name="pLimite">Última fila que se desea obtener.Si pInicio == -1 y pLimite == -1 se trae TODOS los resultados posibles </param>
        /// <param name="pNombreTabla">Nombre de la tabla que queremos paginar del dataset</param>
        /// <param name="pConsultaLigeraCount">Consulta ligera para hacer el count de la paginacion</param>
        /// <returns>Número de registros del resultado</returns>
        int PaginarDataSet(Database pDatabase, DbCommand pComando, string pOrderBy, DataSet pDataSet, int pInicio, int pLimite, string pNombreTabla, bool pContar, string pConsultaLigeraCount);

        /// <summary>
        /// Devuelve un KeyValuePair(consulta count, consulta resultados) con las dos consultas necesarias para realizar la paginación 
        /// (la primera devuelve el numero de resultados totales y la segunda los x resultados seleccionados)
        /// </summary>
        /// <param name="pComando">Comando con la consulta para paginar</param>
        /// <param name="pOrderBy">Criterio de ordenación de los resultados</param>
        /// <param name="pInicio">Primera fila que se desea obtener</param>
        /// <param name="pLimite">Última fila que se desea obtener</param>
        /// <returns>KeyValuePair(consulta count, consulta resultados)</returns>
        KeyValuePair<string, string> ObtenerConsultasPaginar(DbCommand pComando, string pOrderBy, int pInicio, int pLimite);

        /// <summary>
        /// Devuelve un KeyValuePair(consulta count, consulta resultados) con las dos consultas necesarias para realizar la paginación 
        /// de una consulta jerárquica (la primera devuelve el numero de resultados totales y la segunda los x resultados seleccionados)
        /// </summary>
        /// <param name="pComando">Comando con la consulta para paginar</param>
        /// <param name="pConsultaJerarquica">Consulta jerárquica que vamos a paginar</param>
        /// <param name="pOrderBy">Criterio de ordenación de los resultados</param>
        /// <param name="pInicio">Primera fila que se desea obtener</param>
        /// <param name="pLimite">Última fila que se desea obtener</param>
        /// <returns>KeyValuePair(consulta count, consulta resultados)</returns>
        KeyValuePair<string, string> ObtenerConsultasPaginarConsultaJerarquica(DbCommand pComando, IConsultaJerarquica pConsultaJerarquica, string pOrderBy, int pInicio, int pLimite);

        /// <summary>
        /// Pagina un dataset pasado por parámetro
        /// </summary>
        /// <param name="pDatabase">Base de datos que se va a consultar</param>
        /// <param name="pComando">Comando con la consulta para paginar</param>
        /// <param name="pConsultaJerarquica">Consulta jerárquica</param>
        /// <param name="pOrderBy">Criterio de ordenación de los resultados</param>
        /// <param name="pDataSet">Dataset</param>
        /// <param name="pInicio">Primera fila que se desea obtener</param>
        /// <param name="pLimite">Última fila que se desea obtener</param>
        /// <param name="pNombreTabla">Nombre de la tabla del dataset que se quiere paginar</param>
        /// <returns>Número de registros del resultado</returns>
        int PaginarDataSetConConsultaJerarquica(Database pDatabase, DbCommand pComando, IConsultaJerarquica pConsultaJerarquica, string pOrderBy, DataSet pDataSet, int pInicio, int pLimite, string pNombreTabla);

        /// <summary>
        /// Crea una consulta jerárquica
        /// </summary>
        /// <param name="pSelectConsultaGlobal">SELECT de la consulta que va a usar la consulta global</param>
        /// <param name="pFromConsultaGlobal">FROM de la consulta que va a usar la consulta global</param>
        /// <param name="pRestoConsultaGlobal">Resto de la consulta que va a usar la consulta global</param>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en en el FROM de la consulta jerárquica (NULL si no se necesitan más tablas que la tabla de relación)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado (NULL si sólo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condición de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        /// <param name="pIncluirTablaResultadoEnFrom">TRUE si se debe de incluir la tabla de resultados en la consulta global</param>
        /// <returns>Consulta jerárquica</returns>
        IConsultaJerarquica CrearConsultaJerarquica(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia, bool pIncluirTablaResultadoEnFrom);

        /// <summary>
        /// Crea una consulta jerárquica de grafo
        /// </summary>
        /// <param name="pSelectConsultaGlobal">SELECT de la consulta que va a usar la consulta global</param>
        /// <param name="pFromConsultaGlobal">FROM de la consulta que va a usar la consulta global</param>
        /// <param name="pRestoConsultaGlobal">Resto de la consulta que va a usar la consulta global</param>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en en el FROM de la consulta jerárquica (NULL si no se necesitan más tablas que la tabla de relación)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado (NULL si sólo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condición de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        /// <param name="pIncluirTablaResultadoEnFrom">TRUE si se debe de incluir la tabla de resultados en la consulta global</param>
        /// <param name="pNombreTablaElementos">Nombre de la tabla de los elementos originales sin jerarquía</param>
        /// <param name="pClaveTablaElementos">Clave de la tabla de los elementos originales sin jerarquía</param>
        /// <returns>Consulta jerárquica de grafo</returns>
        IConsultaJerarquica CrearConsultaJerarquicaDeGrafo(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia, bool pIncluirTablaResultadoEnFrom, string pNombreTablaElementos, string pClaveTablaElementos);

        /// <summary>
        /// Crea una consulta jerárquica múltiple
        /// </summary>
        /// <param name="pSelectConsultaGlobal">SELECT de la consulta que va a usar la consulta global</param>
        /// <param name="pFromConsultaGlobal">FROM de la consulta que va a usar la consulta global</param>
        /// <param name="pRestoConsultaGlobal">Resto de la consulta que va a usar la consulta global</param>
        /// <param name="pIncluirTablaResultadoEnFrom">TRUE si se debe de inculir la tabla de resultados en la consulta global</param>
        /// <returns>Consulta jerárquica múltiple</returns>
        IConsultaJerarquicaMultiple CrearConsultaJerarquicaMultiple(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, bool pIncluirTablaResultadoEnFrom);

        /// <summary>
        /// Obtiene la cláusula FROM de una consulta sin tablas
        /// </summary>
        /// <returns>La cláusula FROM sin ninguna tabla</returns>
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
    /// Interfaz que representa una consulta jerárquica
    /// </summary>
    public interface IConsultaJerarquica
    {
        #region Propiedades

        /// <summary>
        /// Obtiene la consulta jerárquica
        /// </summary>
        string ConsultaJerarquica
        {
            get;
        }

        #endregion
    }

    /// <summary>
    /// Consulta jerárquica con varias relaciones jerárquicas
    /// </summary>
    public interface IConsultaJerarquicaMultiple
    {
        #region Propiedades

        /// <summary>
        /// Obtiene la consulta jerárquica
        /// </summary>
        string ConsultaJerarquicaMultiple
        {
            get;
        }

        #endregion 

        #region Métodos

        /// <summary>
        /// Agrega una consulta jerárquica
        /// </summary>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en en el FROM de la consulta jerárquica (NULL si no se necesitan más tablas que la tabla de relación)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado (NULL si sólo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condición de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        void AgregarConsultaJerarquica(string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia);

        /// <summary>
        /// Agrega una consulta jerárquica de grafo
        /// </summary>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en el FROM de la consulta jerárquica (NULL si no se necesitan más tablas que la tabla de relación)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado (NULL si sólo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condición de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        /// <param name="pEsGrafo">TRUE si la consulta jerárquica es un grafo (no un árbol)</param>
        /// <param name="pNombreTablaElementos">Nombre de la tabla de los elementos originales sin jerarquía</param>
        /// <param name="pClaveTablaElementos">Clave de la tabla de los elementos originales sin jerarquía</param>
        void AgregarConsultaJerarquicaDeGrafo(string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia, bool pEsGrafo, string pNombreTablaElementos, string pClaveTablaElementos);

        #endregion
    }

    /// <summary>
    /// Estructura para añadir criterios de orden para la paginación
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
