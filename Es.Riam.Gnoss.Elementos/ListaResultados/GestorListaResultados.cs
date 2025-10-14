using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.ListaResultados
{
    /// <summary>
    /// Clase para gestionar las listas de resultados en gnoss
    /// </summary>
    [Serializable]
    public class GestorListaResultados : GestionGnoss, ISerializable,IDisposable
    {
        #region Miembros

        #region Estáticos

        /// <summary>
        /// Nombre para la tabla auxiliar que es necesaria para el exportadorGnoss.
        /// </summary>
        public const string TABLA_GENERICA = "ListasResultados";

        /// <summary>
        /// Nombre para la tabla auxiliar que es necesaria para el exportadorGnoss.
        /// </summary>
        public const string TABLA_AUXILIAR_FILTROS = "FiltrosResultados";

        #endregion

        /// <summary>
        /// Lista de resultados de una búsqueda.
        /// </summary>
        public ListaResultados mListaResultados;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor a partir de un data Set generico.
        /// </summary>
        /// <param name="pDataSetDS">DataSet genérico</param>
        public GestorListaResultados(DataSet pDataSetDS)
            : base(pDataSetDS)
        {
            CargarGestor();
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestorListaResultados(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            CargarGestor();
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Carga el gestor completo
        /// </summary>
        public new void CargarGestor()
        {
            //Cargo las listas
            foreach (ListaResultados listaResultados in Hijos)
            {
                List<IElementoGnoss> lista = listaResultados.Hijos;
            }

            //Creo la tabla auxliar para almacenar los datos que necesita el exportadorGnoss:
            DataSet.Tables.Add(new DataTable(TABLA_GENERICA));

            DataSet.Tables[TABLA_GENERICA].Columns.Add(ListaResultados.CAMPO_LISTARESULTADOSID);
            DataSet.Tables[TABLA_GENERICA].Columns.Add(ListaResultados.CAMPO_NUMERORESULTADOS);
            DataSet.Tables[TABLA_GENERICA].Columns.Add(ListaResultados.CAMPO_NUMEROPAGINA);
            DataSet.Tables[TABLA_GENERICA].Columns.Add(ListaResultados.CAMPO_NUMEROELEMTPORPAG);

            //Creo la tabla auxiliar para almacenar los filtros:
            DataSet.Tables.Add(new DataTable(TABLA_AUXILIAR_FILTROS));

            DataSet.Tables[TABLA_AUXILIAR_FILTROS].Columns.Add(Filtro.CAMPO_NOMBRE);
            DataSet.Tables[TABLA_AUXILIAR_FILTROS].Columns.Add(Filtro.CAMPO_VALOR);
        }

        /// <summary>
        /// Agrega un filtro a la lista de resultados manejada por el gestor.
        /// </summary>
        /// <param name="pNombre">Nombre del filtro</param>
        /// <param name="pValor">Valor del filtro</param>
        public void AgregarFiltroAListaResultados(string pNombre, string pValor)
        {
            ListaResultados.Filtros.Add(new Filtro(this, pNombre, pValor));
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece la lista de resultados de una búsqueda.
        /// </summary>
        public ListaResultados ListaResultados
        {
            get
            {
                return mListaResultados;
            }
            set
            {
                mListaResultados = value;
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
        ~GestorListaResultados()
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
                disposed = true;
                try
                {
                    if (disposing)
                    {
                        //Libero todos los recursos administrados que he añadido a esta clase

                        if (this.mListaResultados != null)
                        {
                            this.mListaResultados.Dispose();
                        }
                    }
                }
                finally
                {
                    this.mListaResultados = null;

                    // Llamo al dispose de la clase base
                    base.Dispose(disposing);
                }
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Metodo para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion
    }
}
