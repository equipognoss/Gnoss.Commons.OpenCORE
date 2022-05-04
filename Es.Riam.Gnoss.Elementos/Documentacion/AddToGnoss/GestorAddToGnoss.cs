using System;
using System.Collections.Generic;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using System.Runtime.Serialization;

namespace Es.Riam.Gnoss.Elementos.Documentacion.AddToGnoss
{
    /// <summary>
    /// Gestor de gestores para add to Gnoss.
    /// </summary>
    [Serializable]
    public class GestorAddToGnoss : ISerializable
    {

        #region Miembros

        /// <summary>
        /// Gestor documental.
        /// </summary>
        private GestorDocumental mGestorDocumental;

        /// <summary>
        /// Lista de categorías del add to gnoss.
        /// </summary>
        private List<CategoriaTesauro> mListaCategorias;

        /// <summary>
        /// Indica si la base de recursos es personal o no.
        /// </summary>
        private bool mEsBaseRecursosPersonal;

        /// <summary>
        /// Indica si la base de recursos es de organizacion o no.
        /// </summary>
        private bool mEsBaseRecursosOrganizacion;

        /// <summary>
        /// Indica si la base de recursos es puiblica o no.
        /// </summary>
        private bool mEsBaseRecursosPublica;

        /// <summary>
        /// Indica si esta agrega la BR o no.
        /// </summary>
        private bool mEstaAgregado;

        /// <summary>
        /// Identidad del usuario en el proyecto.
        /// </summary>
        private Guid mIdentidadEnProyecto;

        /// <summary>
        /// Identificador del perfil del usuario en el proyecto.
        /// </summary>
        private Guid mPerfilEnProyecto;

        /// <summary>
        /// Nombre de la BR del gestor.
        /// </summary>
        private string mNombreBaseRecursos;

        /// <summary>
        /// Indica si el recurso es privado para editores en esta BR
        /// </summary>
        private bool mPrivadoEditores;

        /// <summary>
        /// Proyecto al que pertenece la BR del proyecto.
        /// </summary>
        private Guid mProyectoBRID;

        /// <summary>
        /// Organización a la que pertenece la BR del proyecto.
        /// </summary>
        private Guid mOrganizacionBRID;

        /// <summary>
        /// Devuelve o establece el gestor de proyecto.
        /// </summary>
        private GestionProyecto mGestorProyecto;

        /// <summary>
        /// Licencia por defecto de la comunidad del gestor.
        /// </summary>
        private string mLicenciaProyectoPorDefecto;

        /// <summary>
        /// Nombre de la comunidad del gestor.
        /// </summary>
        private string mNombreProyecto;

        /// <summary>
        /// Indica si el proyecto del gestor tiene Twitter.
        /// </summary>
        private bool mTieneTwitter;

        /// <summary>
        /// 
        /// </summary>
        private List<Guid> mListaIds;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor público sin parámetros.
        /// </summary>
        public GestorAddToGnoss()
        {
            mEsBaseRecursosPersonal = false;
            mEsBaseRecursosOrganizacion = false;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="pInfo">Datos serializados</param>
        /// <param name="pContext">Contexto de serialización</param>
        protected GestorAddToGnoss(SerializationInfo pInfo, StreamingContext pContext)
        {
            this.mEsBaseRecursosPersonal = (bool)pInfo.GetValue("EsBaseRecursosPersonal", typeof(bool));
            this.mEsBaseRecursosOrganizacion = (bool)pInfo.GetValue("EsBaseRecursosOrganizacion", typeof(bool));
            this.mEstaAgregado = (bool)pInfo.GetValue("EstaAgregado", typeof(bool));
            this.mGestorDocumental = (GestorDocumental)pInfo.GetValue("GestorDocumental", typeof(GestorDocumental));
            this.mGestorProyecto = (GestionProyecto)pInfo.GetValue("GestorProyecto", typeof(GestionProyecto));
            this.mIdentidadEnProyecto = (Guid)pInfo.GetValue("IdentidadEnProyecto", typeof(Guid));
            this.mNombreBaseRecursos = (string)pInfo.GetValue("NombreBaseRecursos", typeof(string));
            this.mOrganizacionBRID = (Guid)pInfo.GetValue("OrganizacionBRID", typeof(Guid));
            this.mPerfilEnProyecto = (Guid)pInfo.GetValue("PerfilEnProyecto", typeof(Guid));
            this.mProyectoBRID = (Guid)pInfo.GetValue("ProyectoBRID", typeof(Guid));
            this.mLicenciaProyectoPorDefecto = (string)pInfo.GetValue("LicenciaProyectoPorDefecto", typeof(string));
            this.mNombreProyecto = (string)pInfo.GetValue("NombreProyectoGAddTo", typeof(string));
            this.mListaIds = (List<Guid>)pInfo.GetValue("ListaCategorias", typeof(List<Guid>));
            this.mEsBaseRecursosPublica = (bool)pInfo.GetValue("EsBaseRecursosPublica", typeof(bool));

            this.mPrivadoEditores = (bool)pInfo.GetValue("PrivadoEditores", typeof(bool));            
        }

        #endregion

        #region Metodos

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pCatTesauro"></param>
        public void AgregarCategoria(CategoriaTesauro pCatTesauro)
        { 
            if(!ListaCategorias.Contains(pCatTesauro))
            {
                ListaCategorias.Add(pCatTesauro);
                if (!ListaIds.Contains(pCatTesauro.Clave))
                {
                    ListaIds.Add(pCatTesauro.Clave);
                }
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece el gestor documental.
        /// </summary>
        public GestorDocumental GestorDocumental
        {
            get
            {
                return mGestorDocumental;
            }
            set
            {
                mGestorDocumental = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la lista de categorías del add to gnoss.
        /// </summary>
        public List<CategoriaTesauro> ListaCategorias
        {
            get
            {
                if (mListaCategorias == null)
                {
                    mListaCategorias = new List<CategoriaTesauro>();

                    if (mListaIds != null)
                    {
                        if (GestorDocumental != null && GestorDocumental.GestorTesauro != null)
                        {
                            foreach (Guid id in mListaIds)
                            {
                                mListaCategorias.Add(GestorDocumental.GestorTesauro.ListaCategoriasTesauro[id]);
                            }
                        }
                    }
                }
                return mListaCategorias;
            }
            set
            {
                mListaCategorias = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la lista de Ids.
        /// </summary>
        public List<Guid> ListaIds
        {
            get
            {
                if (mListaIds == null)
                {
                    mListaIds = new List<Guid>();
                }
                return mListaIds;
            }
            set
            {
                mListaIds = value;
            }
        }

        /// <summary>
        /// Devuelve o establece si la base de recursos es personal o no.
        /// </summary>
        public bool EsBaseRecursosPersonal
        {
            get
            {
                return mEsBaseRecursosPersonal;
            }
            set
            {
                mEsBaseRecursosPersonal = value;
            }
        }

        /// <summary>
        /// Devuelve o establece si la base de recursos de organización o no.
        /// </summary>
        public bool EsBaseRecursosOrganizacion
        {
            get
            {
                return mEsBaseRecursosOrganizacion;
            }
            set
            {
                mEsBaseRecursosOrganizacion = value;
            }
        }

        /// <summary>
        /// Indica si esta agrega la BR o no.
        /// </summary>
        public bool EstaAgregado
        {
            get
            {
                return mEstaAgregado;
            }
            set
            {
                mEstaAgregado = value;
            }
        }

        /// <summary>
        /// Nombre de la BR del gestor.
        /// </summary>
        public string NombreBaseRecursos
        {
            get
            {
                return mNombreBaseRecursos;
            }
            set
            {
                mNombreBaseRecursos = value;
            }
        }

        /// <summary>
        /// Identidad del usuario en el proyecto.
        /// </summary>
        public Guid IdentidadEnProyecto
        {
            get
            {
                return mIdentidadEnProyecto;
            }
            set
            {
                mIdentidadEnProyecto = value;
            }
        }

        /// <summary>
        /// Indica si el documento va a ser privado para editores en esta BR
        /// </summary>
        public bool PrivadoEditores
        {
            get
            {
                return mPrivadoEditores;
            }
            set
            {
                mPrivadoEditores = value;
            }
        }

        /// <summary>
        /// Identificador del perfil del usuario en el proyecto.
        /// </summary>
        public Guid PerfilEnProyecto
        {
            get
            {
                return mPerfilEnProyecto;
            }
            set
            {
                mPerfilEnProyecto = value;
            }
        }

        /// <summary>
        /// Proyecto al que pertenece la BR del proyecto.
        /// </summary>
        public Guid ProyectoBRID
        {
            get
            {
                return mProyectoBRID;
            }
            set
            {
                mProyectoBRID = value;
            }
        }

        /// <summary>
        /// Proyecto al que pertenece la BR del proyecto.
        /// </summary>
        public Guid OrganizacionBRID
        {
            get
            {
                return mOrganizacionBRID;
            }
            set
            {
                mOrganizacionBRID = value;
            }
        }

        /// <summary>
        /// Devuelve o establece si el gestor contiene una base de recursos que es pública.
        /// </summary>
        public bool EsBaseRecursosPublica
        {
            get
            {
                return mEsBaseRecursosPublica;
            }
            set
            {
                mEsBaseRecursosPublica = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el gestor de proyecto.
        /// </summary>
        public GestionProyecto GestorProyecto
        {
            get
            {
                return mGestorProyecto;
            }
            set
            {
                mGestorProyecto = value;
            }
        }

        /// <summary>
        /// Licencia por defecto de la comunidad del gestor.
        /// </summary>
        public string LicenciaProyectoPorDefecto
        {
            get
            {
                return mLicenciaProyectoPorDefecto;
            }
            set
            {
                mLicenciaProyectoPorDefecto = value;
            }
        }

        /// <summary>
        /// Nombre de la comunidad del gestor.
        /// </summary>
        public string NombreProyecto
        {
            get
            {
                return mNombreProyecto;
            }
            set
            {
                mNombreProyecto = value;
            }
        }

        /// <summary>
        /// Indica si el proyecto del gestor tiene Twitter.
        /// </summary>
        public bool TieneTwitter
        {
            get
            {
                return mTieneTwitter;
            }
            set
            {
                mTieneTwitter = value;
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Obtiene los datos que se deben serializar de este objeto
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("EsBaseRecursosPersonal", this.mEsBaseRecursosPersonal);
            info.AddValue("EsBaseRecursosOrganizacion", this.mEsBaseRecursosOrganizacion);
            info.AddValue("EstaAgregado", this.mEstaAgregado);
            info.AddValue("GestorDocumental", this.mGestorDocumental);
            info.AddValue("GestorProyecto", this.mGestorProyecto);
            info.AddValue("IdentidadEnProyecto", this.mIdentidadEnProyecto);
            info.AddValue("NombreBaseRecursos", this.mNombreBaseRecursos);
            info.AddValue("OrganizacionBRID", this.mOrganizacionBRID);
            info.AddValue("PerfilEnProyecto", this.mPerfilEnProyecto);
            info.AddValue("ProyectoBRID", this.mProyectoBRID);
            info.AddValue("LicenciaProyectoPorDefecto", this.mLicenciaProyectoPorDefecto);
            info.AddValue("NombreProyectoGAddTo", this.mNombreProyecto);
            info.AddValue("EsBaseRecursosPublica", this.mEsBaseRecursosPublica);

            info.AddValue("PrivadoEditores", this.mPrivadoEditores);

            //List<Guid> listaCats = null;
            //if (mListaCategorias != null)
            //{
            //    listaCats = new List<Guid>();
            //    foreach (CategoriaTesauro cat in mListaCategorias)
            //    {
            //        listaCats.Add(cat.Clave);
            //    }
            //}

            info.AddValue("ListaCategorias", mListaIds);
        }

        #endregion
    }
}
