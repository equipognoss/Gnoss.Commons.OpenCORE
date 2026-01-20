using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Elementos.Tesauro
{
    /// <summary>
    /// Enumeración que contiene los idiomas de las categorías
    /// </summary>
    public enum IdiomasCategorias
    {
        es,
        en,
        pt,
        ca,
        ca_valencia,
        eu,
        gl,
        de,
        fr,
        it
    }

    /// <summary>
    /// Categoría del tesauro
    /// </summary>
    public class CategoriaTesauro : ElementoGnoss, IDisposable
    {
        #region Miembros

        private AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro mFilaAgregacion;

        private List<Documento> mDocumentos;
        private List<Documento> mUltimosDocumentos;
        private List<CategoriaTesauro> mSubCategorias;
        private CategoriaTesauro mPadreNivelRaiz = null;

        /// <summary>
        /// Lista de hijos de la categoría ordenados por titulo en función del idioma.
        /// </summary>
        private Dictionary<string, List<IElementoGnoss>> mHijosOrdenadosPorNombre;

        /// <summary>
        /// Lista con las categorias sugeridas como hijas de la actual.
        /// </summary>
        private Dictionary<Guid, CategoriaTesauroSugerencia> mCategoriasSugeridasHijas;

        /// <summary>
        /// Lista con las categorías hijas más las sugeridas de la actual.
        /// </summary>
        public List<IElementoGnoss> mHijosMasSugerencias;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pFilaCategoria">Fila de la categoría de tesauro</param>
        /// <param name="pGestionTesauro">Gestor de tesauro</param>
        public CategoriaTesauro(AD.EntityModel.Models.Tesauro.CategoriaTesauro pFilaCategoria, GestionTesauro pGestionTesauro)
            : base(pFilaCategoria, pGestionTesauro)
        {
            mHijos = new List<IElementoGnoss>();
            mSubCategorias = new List<CategoriaTesauro>();
        }

        #endregion

        #region Propiedades


        /// <summary>
        /// Obtiene o establece el padre de la categoría de tesauro
        /// </summary>
        public override IElementoGnoss Padre
        {
            get
            {
                return base.Padre;
            }
            set
            {
                IElementoGnoss padreAnterior = base.Padre;
                base.Padre = value;

                if ((padreAnterior != null) && (padreAnterior != value))
                {
                    if (padreAnterior is CategoriaTesauro)
                        this.GestorTesauro.DesasignarSubcategoriaDeCategoria(this, (CategoriaTesauro)padreAnterior);

                    if (value is CategoriaTesauro)
                        this.mFilaAgregacion = this.GestorTesauro.AgregarSubcategoriaACategoria(this, (CategoriaTesauro)value);
                    else
                        this.mFilaAgregacion = null;
                }
            }
        }

        /// <summary>
        /// Obtiene la fila de la categoría de tesauro
        /// </summary>
        public AD.EntityModel.Models.Tesauro.CategoriaTesauro FilaCategoria
        {
            get
            {
                return (AD.EntityModel.Models.Tesauro.CategoriaTesauro)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Obtiene el nombre de la categoría de tesauro en funcion del idioma
        /// </summary>
        public new Dictionary<string, string> Nombre
        {
            get
            {
                Dictionary<string, string> Nombres = new Dictionary<string, string>();

                string[] nombres = FilaCategoria.Nombre.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                string idiomaDefecto = "";
                if (GestorTesauro != null && GestorTesauro.TesauroDW != null && GestorTesauro.TesauroDW.ListaTesauroProyecto.Count == 1)
                {
                    idiomaDefecto = GestorTesauro.TesauroDW.ListaTesauroProyecto.FirstOrDefault().IdiomaDefecto;
                }

                foreach (IdiomasCategorias idioma in Enum.GetValues(typeof(IdiomasCategorias)))
                {
                    //Añadimos los nombres con idiomas
                    string idiomaAux = idioma.ToString();
                    //Si es un idioma con dialecto nos aseguramos de que el idioma respete el formato XX-XX-XX
                    idiomaAux = idiomaAux.Contains('_') ? idiomaAux.Replace("_", "-") : idiomaAux;
                    string nombreIdioma = "";
                    if (nombres.Length > 1 || (nombres.Length == 1 && nombres[0].Contains("@")))
                    {
                        foreach (string nombre in nombres)
                        {
                            if (nombre.EndsWith("@" + idiomaAux.ToString()))
                            {
                                nombreIdioma = nombre.Split("@")[0];
                                break;
                            }
                            else if (!string.IsNullOrEmpty(idiomaDefecto) && nombre.EndsWith("@" + idiomaDefecto.ToString()))
                            {
                                nombreIdioma = nombre.Split("@")[0];
                            }
                            else if (string.IsNullOrEmpty(nombreIdioma))
                            {
                                nombreIdioma = nombre.Split("@")[0];
                            }
                        }
                    }

                    if (nombreIdioma == "")
                    {
                        nombreIdioma = FilaCategoria.Nombre;
                    }
                    Nombres.Add(idiomaAux, nombreIdioma);
                }
                return Nombres;
            }
        }

        /// <summary>
        /// Obtiene el nombre de la categoría de tesauro con el número de recursos vinculados a ella en funcion del idioma
        /// </summary>
        public Dictionary<string, string> NombreConNumeroRecursos
        {
            get
            {
                int numero = 0;
                if (GestorTesauro.NumElementosCategoria.ContainsKey(Clave) && GestorTesauro.NumElementosCategoria[Clave] > 0)
                {
                    numero = GestorTesauro.NumElementosCategoria[Clave];
                }

                Dictionary<string, string> nombreConRec = new Dictionary<string, string>();
                foreach (string idioma in Nombre.Keys)
                {
                    nombreConRec.Add(idioma, Nombre[idioma] + " (" + numero + ")");

                }
                return nombreConRec;
            }
        }

        /// <summary>
        /// Obtiene el nombre de la categoría de tesauro con el número de recursos vinculados a ella
        /// </summary>
        public int NumeroElementos
        {
            get
            {
                if (GestorTesauro.NumElementosCategoria.ContainsKey(Clave))
                {
                    return GestorTesauro.NumElementosCategoria[Clave];
                }

                return 0;
            }
        }

        /// <summary>
        /// Obtiene el nombre semantico de la categoría de tesauro en funcion del idioma
        /// </summary>
        public Dictionary<string, string> NombreSem
        {
            get
            {
                Dictionary<string, string> NombresSem = new Dictionary<string, string>();
                foreach (string idioma in Nombre.Keys)
                {
                    NombresSem.Add(idioma, UtilCadenas.EliminarCaracteresUrlSem(Nombre[idioma]));
                }
                return NombresSem;
            }
        }

        /// <summary>
        /// Obtiene el identificador de la categoría de tesauro
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaCategoria.CategoriaTesauroID;
            }
        }

        /// <summary>
        /// Obtiene los documentos asociados a la categoria de tesauro
        /// </summary>
        public List<Documento> Documentos
        {
            get
            {
                if (mDocumentos == null)
                {
                    mDocumentos = new List<Documento>();
                    if (GestorTesauro.GestorDocumental != null)
                    {
                        foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro fila in GestorTesauro.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.CategoriaTesauroID.Equals(Clave)).ToList())
                        {
                            if (GestorTesauro.GestorDocumental.ListaDocumentos.ContainsKey(fila.DocumentoID))
                            {
                                Documento doc = GestorTesauro.GestorDocumental.ListaDocumentos[fila.DocumentoID];

                                if (doc.FilaDocumento.UltimaVersion && !doc.EstaEliminado && !doc.FilaDocumento.Eliminado)
                                    mDocumentos.Add(doc);
                            }
                        }
                    }
                }

                //Esta linea hace que casque todo por todos los lados, si alguien la echa en falta que la suba a la capa de presentación.
                //mDocumentos.Sort(GestorDocumental.CompararDocumentosPorFecha);

                return mDocumentos;
            }
        }

        /// <summary>
        /// Obtiene el gestor de tesauro
        /// </summary>
        public GestionTesauro GestorTesauro
        {
            get
            {
                return (GestionTesauro)GestorGnoss;
            }
        }

        /// <summary>
        /// Obtiene la lista de los hijos de la categoría de tesauro
        /// </summary>
        public override List<IElementoGnoss> Hijos
        {
            get
            {
                return mHijos;
            }
        }

        /// <summary>
        /// Lista con las categorías del tesauro ordenadas por nombre en funcion del idioma.
        /// </summary>
        public Dictionary<string, List<IElementoGnoss>> HijosOrdenadosPorNombre
        {
            get
            {
                if (mHijosOrdenadosPorNombre == null)
                {
                    mHijosOrdenadosPorNombre = new Dictionary<string, List<IElementoGnoss>>();
                    SortedDictionary<string, List<IElementoGnoss>> listaOrdenadaNombreCat = new SortedDictionary<string, List<IElementoGnoss>>();

                    foreach (IdiomasCategorias idioma in Enum.GetValues(typeof(IdiomasCategorias)))
                    {
                        foreach (IElementoGnoss elem in Hijos)
                        {
                            CategoriaTesauro cat = (CategoriaTesauro)elem;
                            if (listaOrdenadaNombreCat.ContainsKey(cat.Nombre[idioma.ToString()]))
                            {
                                listaOrdenadaNombreCat[cat.Nombre[idioma.ToString()]].Add(cat);
                            }
                            else
                            {
                                List<IElementoGnoss> listaElemAux = new List<IElementoGnoss>();
                                listaElemAux.Add(cat);
                                listaOrdenadaNombreCat.Add(cat.Nombre[idioma.ToString()], listaElemAux);
                            }
                        }

                        if (!mHijosOrdenadosPorNombre.ContainsKey(idioma.ToString()))
                        {
                            mHijosOrdenadosPorNombre.Add(idioma.ToString(), new List<IElementoGnoss>());
                        }

                        foreach (List<IElementoGnoss> listaElem in listaOrdenadaNombreCat.Values)
                        {
                            mHijosOrdenadosPorNombre[idioma.ToString()].AddRange(listaElem);
                        }
                    }
                }
                return mHijosOrdenadosPorNombre;
            }
        }

        /// <summary>
        /// Categorías del tesauro hijas y las sugeridas.
        /// </summary>
        public List<IElementoGnoss> HijosMasSugerencias
        {
            get
            {
                if (mHijosMasSugerencias == null)
                {
                    mHijosMasSugerencias = new List<IElementoGnoss>();
                    mHijosMasSugerencias.AddRange(Hijos);

                    foreach (CategoriaTesauroSugerencia catSugHija in CategoriasSugeridasHijas.Values)
                    {
                        mHijosMasSugerencias.Add(catSugHija);
                    }
                }

                return mHijosMasSugerencias;
            }
        }

        /// <summary>
        /// Obtiene la lista de las subcategorías de la categoría de tesauro
        /// </summary>
        public List<CategoriaTesauro> SubCategorias
        {
            get
            {
                return mSubCategorias;
            }
        }

        /// <summary>
        /// Obtiene o establece la fila de agregación con otra categoría
        /// </summary>
        public AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro FilaAgregacion
        {
            get
            {
                return this.mFilaAgregacion;
            }
            set
            {
                this.mFilaAgregacion = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el índice el elemento
        /// </summary>
        public override short Indice
        {
            get
            {
                if (EstaEliminado)
                    return 0;

                if (FilaAgregacion != null)
                    return FilaAgregacion.Orden;
                else
                    return this.FilaCategoria.Orden;
            }
            set
            {
                if (!EstaEliminado)
                {
                    if (FilaAgregacion != null)
                        FilaAgregacion.Orden = value;
                    else
                        this.FilaCategoria.Orden = value;
                }
            }
        }

        /// <summary>
        /// Devuelve si el elemento se puede seleccionar
        /// </summary>
        public override bool EsSeleccionable
        {
            get
            {
                return !FilaCategoria.Estructurante.Equals(1);
            }
        }

        /// <summary>
        /// Obtiene si el elemento se puede eliminar
        /// </summary>
        public override bool SePuedeEliminar
        {
            get
            {
                return EsEditable;
            }
        }

        /// <summary>
        /// Devuelve el padre de primer nivel del elemento.
        /// </summary>
        public CategoriaTesauro PadreNivelRaiz
        {
            get
            {
                if (mPadreNivelRaiz == null)
                {
                    mPadreNivelRaiz = this;
                    while (mPadreNivelRaiz.Padre != null && mPadreNivelRaiz.Padre is CategoriaTesauro)
                    {
                        mPadreNivelRaiz = (CategoriaTesauro)mPadreNivelRaiz.Padre;
                    }
                }
                return mPadreNivelRaiz;
            }
        }

        /// <summary>
        /// Identificador al que pertenece la categoría del tesauro.
        /// </summary>
        public Guid ProyectoIDPertenece
        {
            get
            {
                List<AD.EntityModel.Models.Tesauro.TesauroProyecto> filasTesProy = GestorTesauro.TesauroDW.ListaTesauroProyecto.Where(item => item.TesauroID.Equals(this.FilaCategoria.TesauroID)).ToList();
                if (filasTesProy.Count > 0)
                {
                    return (filasTesProy.FirstOrDefault()).ProyectoID;
                }
                else
                {
                    return ProyectoAD.MetaProyecto;
                }
            }
        }

        /// <summary>
        /// Devuelve la lista con las categorias sugeridas como hijas de la actual.
        /// </summary>
        public Dictionary<Guid, CategoriaTesauroSugerencia> CategoriasSugeridasHijas
        {
            get
            {
                if (mCategoriasSugeridasHijas == null)
                {
                    mCategoriasSugeridasHijas = new Dictionary<Guid, CategoriaTesauroSugerencia>();
                }
                return mCategoriasSugeridasHijas;
            }
        }

        /// <summary>
        /// Devuelve si la categoría pertenece a las 4 especiales del usuario (pública, privada, imagenes y videos).
        /// </summary>
        public bool EsEspecialDeUsuario
        {
            get
            {
                return (GestorTesauro.CategoriaPublicaID == Clave || GestorTesauro.CategoriaPrivadaID == Clave || GestorTesauro.CategoriaImagenesID == Clave || GestorTesauro.CategoriaVideosID == Clave);
            }
        }

        /// <summary>
        /// Obtiene la URL relativa de la imagen de la categoria
        /// </summary>
        public string UrlImagen
        {
            get
            {
                string urlFoto = "";

                if (FilaCategoria.TieneFoto)
                {
                    urlFoto = "/Categorias/cat_" + Clave.ToString() + ".png?" + FilaCategoria.VersionFoto;
                }
                else
                {
                    urlFoto = "/Categorias/cat_default.png";
                }

                return urlFoto;
            }
        }

        /// <summary>
        /// Obtiene la URL relativa de la imagen mini de la categoria
        /// </summary>
        public string UrlImagenMini
        {
            get
            {
                string urlFoto = "";

                if (FilaCategoria.TieneFoto)
                {
                    urlFoto = "/Categorias/cat_" + Clave.ToString() + "_mini.png?" + FilaCategoria.VersionFoto;
                }
                else
                {
                    urlFoto = "/Categorias/cat_default_mini.png";
                }

                return urlFoto;
            }
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Determina si el elemento admite como hijo al elemento pasado por parámetro
        /// </summary>
        /// <param name="pHijoCandidato">Elemento candidato para ser hijo</param>
        /// <returns>TRUE si el candidato puede ser hijo</returns>
        public override bool AdmiteHijo(IElementoGnoss pHijoCandidato)
        {
            if ((pHijoCandidato != null) && (pHijoCandidato is CategoriaTesauro) && (!this.EsHijoDe(pHijoCandidato)))
                return true;
            return false;
        }

        /// <summary>
        /// Carga las subcategorias
        /// </summary>
        public void CargarSubcategorias()
        {
            mHijos = new List<IElementoGnoss>();
            mSubCategorias = new List<CategoriaTesauro>();
            AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro catAgregada = null;
            if (GestorTesauro.ListaCategoriasInferioresPorCategoriaID.ContainsKey(FilaCategoria.CategoriaTesauroID))
            {
                List<object> listaHijos = GestorTesauro.ListaCategoriasInferioresPorCategoriaID[FilaCategoria.CategoriaTesauroID];

                foreach (object hijo in listaHijos)
                {
                    AD.EntityModel.Models.Tesauro.CategoriaTesauro categoria = null;
                    if (hijo is AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro)
                    {
                        catAgregada = (AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro)hijo;
                        categoria = catAgregada.CategoriaTesauro;

                        if (categoria == null)
                        {
                            //Problema al mover una categoria, ya que esta se marca como deleted y se pierden los objetos relacionados
                            categoria = GestorTesauro.TesauroDW.ListaCategoriaTesauro.FirstOrDefault(cat => cat.CategoriaTesauroID.Equals(catAgregada.CategoriaInferiorID) && cat.TesauroID.Equals(catAgregada.TesauroID));
                        }
                    }
                    else
                    {
                        // Categoría compartida desde otro tesauro
                        AD.EntityModel.Models.Tesauro.CatTesauroCompartida catCompartida = (AD.EntityModel.Models.Tesauro.CatTesauroCompartida)hijo;
                        categoria = catCompartida.CategoriaTesauro;
                    }

                    
                    if (categoria != null && this.GestorTesauro.ListaCategoriasTesauro.ContainsKey(categoria.CategoriaTesauroID))
                    {
                        CategoriaTesauro cat = this.GestorTesauro.ListaCategoriasTesauro[categoria.CategoriaTesauroID];
                        cat.Padre = this;

                        cat.FilaAgregacion = catAgregada;

                        if (!mHijos.Contains(cat))
                        {
							mHijos.Add(cat);
						}
                            
                        if (!mSubCategorias.Contains(cat))
                        {
							mSubCategorias.Add(cat);
						}                        
                    }
                }
            }
        }

        public List<Guid> ObtenerCategoriasHijosNietos()
        {
            List<Guid> listaCategoriasHijosNietos = this.SubCategorias.Select(item => item.Clave).ToList();

            foreach (CategoriaTesauro categoria in this.SubCategorias)
            {
                listaCategoriasHijosNietos.AddRange(categoria.ObtenerCategoriasHijosNietos());
            }

            return listaCategoriasHijosNietos;
        }

        /// <summary>
        /// Compara dos categorías de tesauro por nombre en funcion del idioma
        /// </summary>
        /// <param name="x">Categoría de tesauro x</param>
        /// <param name="y">Categoría de tesauro y</param>
        public static int CompararCategoriasPorNombre(CategoriaTesauro x, CategoriaTesauro y, string pIdioma)
        {
            return x.Nombre[pIdioma].CompareTo(y.Nombre[pIdioma]);
        }

        /// <summary>
        /// Obtiene los últimos documentos asociados a la categoría de tesauro entre dos fechas dadas
        /// </summary>
        /// <param name="pFechaInicio">Fecha de inicio</param>
        /// <param name="pFechaFin">Fecha de fin</param>
        /// <returns>Lista de documentos</returns>
        public List<Documento> UltimosDocumentosSuscripcion(DateTime pFechaInicio, DateTime pFechaFin)
        {
            if (mUltimosDocumentos == null)
            {
                mUltimosDocumentos = new List<Documento>();

                //Consulta jerarquica para obtener también los documentos agregados recientemente a las categorias hijas.
                //string sqlJerarquico = "CategoriaTesauroID = '" + this.Clave.ToString() + "'";

                //foreach (CategoriaTesauro subCategoria in SubCategorias)
                //{
                //    sqlJerarquico += " OR CategoriaTesauroID = '" + subCategoria.Clave + "'";
                //}
                List<Guid> listaSubCategoriasID = SubCategorias.Select(subCat => subCat.Clave).ToList();
                foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro fila in this.GestorTesauro.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(doc => doc.CategoriaTesauroID.Equals(Clave) || listaSubCategoriasID.Contains(doc.CategoriaTesauroID)))
                {
                    if (this.GestorTesauro.GestorDocumental.ListaDocumentos.ContainsKey(fila.DocumentoID))
                    {
                        Documento doc = this.GestorTesauro.GestorDocumental.ListaDocumentos[fila.DocumentoID];
                        if (!mUltimosDocumentos.Contains(doc))
                        {
                            if (fila.Fecha.Value >= pFechaInicio.Date && fila.Fecha.Value < pFechaFin.Date)
                                mUltimosDocumentos.Add(doc);
                        }
                    }
                }
            }
            mUltimosDocumentos.Sort(GestorDocumental.CompararDocumentosPorFecha);
            return mUltimosDocumentos;
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
        ~CategoriaTesauro()
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

                    }
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
