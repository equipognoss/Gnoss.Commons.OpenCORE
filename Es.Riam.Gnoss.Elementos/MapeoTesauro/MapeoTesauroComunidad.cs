using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Es.Riam.Gnoss.Elementos.MapeoTesauroComunidad
{
    [Serializable]
    public class MapeoTesauroComunidad : ISerializable
    {
        private List<BloqueMapeo> mBloques;

        public MapeoTesauroComunidad() 
        {
            mBloques = new List<BloqueMapeo>();
        }

        //constructor para la deserializacion
        public MapeoTesauroComunidad(SerializationInfo info, StreamingContext context)
        {
            mBloques = (List<BloqueMapeo>)info.GetValue("MappingsProyecto", typeof(BloqueMapeo));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("MappingsProyecto", Bloques);
        }

        public List<BloqueMapeo> Bloques
        {
            get
            {
                return mBloques;
            }
        }
    }

    [Serializable]
    public class BloqueMapeo : ISerializable
    {
        private Dictionary<string, Mapping> mDiccionarioMappingRamas;
        private List<PropiedadMapeo> mListaPropiedades;
        private List<Mapping> mListaMapping;

        public BloqueMapeo() 
        {
            mListaPropiedades = new List<PropiedadMapeo>();
            mListaMapping = new List<Mapping>();
            mDiccionarioMappingRamas = new Dictionary<string, Mapping>();
        }

        public BloqueMapeo(SerializationInfo info, StreamingContext context)
        {
            mListaPropiedades = (List<PropiedadMapeo>)info.GetValue("Propiedades", typeof(PropiedadMapeo));
            mListaMapping = (List<Mapping>)info.GetValue("Propiedades", typeof(Mapping));
        }

        public List<PropiedadMapeo> ListaPropiedades
        {
            get
            {
                return mListaPropiedades;
            }
        }
        public List<Mapping> ListaMapping
        {
            get
            {
                return mListaMapping;
            }
        }

        public Dictionary<string, Mapping> DiccionarioMappingRamas
        {
            get
            {
                return mDiccionarioMappingRamas;
            }
        }

        public Dictionary<string, Mapping> MontarDiccionarioBloqueMapeo()
        {
            foreach (Mapping mapeo in mListaMapping)
            {
                //lista ordenada alfabeticamente
                SortedList<string, string> listaIDsOrdenada = new SortedList<string, string>();

                foreach (Guid id in mapeo.CategoriasOrigen)
                {
                    listaIDsOrdenada.Add(id.ToString(), null);
                }

                string claveDiccionario = string.Empty;
                string coma = string.Empty;

                foreach (string cadena in listaIDsOrdenada.Keys)
                {
                    claveDiccionario += coma + cadena;
                    coma = ",";
                }

                if(!mDiccionarioMappingRamas.ContainsKey(claveDiccionario))
                {
                    mDiccionarioMappingRamas.Add(claveDiccionario, mapeo);
                }
            }

            return mDiccionarioMappingRamas;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Propiedades", ListaPropiedades);
            info.AddValue("Mapping", ListaMapping);
        }
    }

    [Serializable]
    public class PropiedadMapeo : ISerializable
    {
        string mNombrePropiedad;
        string mTipoEntidad;

        public PropiedadMapeo() { }

        public PropiedadMapeo(SerializationInfo info, StreamingContext context)
        {
            mNombrePropiedad = (string)info.GetValue("NombrePropiedad", typeof(string));
            mTipoEntidad = (string)info.GetValue("TipoEntidad", typeof(string));
        }

        public string NombrePropiedad
        {
            get
            {
                return mNombrePropiedad;
            }
            
            set
            {
                mNombrePropiedad = value;
            }
        }
        public string TipoEntidad
        {
            get
            {
                return mTipoEntidad;
            }

            set
            {
                mTipoEntidad = value;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("NombrePropiedad", NombrePropiedad);
            info.AddValue("TipoEntidad", TipoEntidad);
        }
    }
    
    [Serializable]
    public class Mapping : ISerializable
    {
        private List<Guid> mCategoriasOrigen;
        private List<Rama> mRamas;

        public Mapping() 
        {
            mCategoriasOrigen = new List<Guid>();
            mRamas = new List<Rama>();
        }

        public Mapping(SerializationInfo info, StreamingContext context)
        {
            mCategoriasOrigen = (List<Guid>)info.GetValue("CategoriaOrigen", typeof(List<Guid>));
            mRamas = (List<Rama>)info.GetValue("Rama", typeof(List<List<string>>));
        }

        public List<Guid> CategoriasOrigen 
        {
            get
            {
                return mCategoriasOrigen;
            }
        }
        public List<Rama> Ramas
        {
            get
            {
                return mRamas;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CategoriaOrigen", CategoriasOrigen);
            info.AddValue("Rama", Ramas);
        }
    }

    [Serializable]
    public class Rama : ISerializable
    {
        private List<string> mCategoriasDestino;

        public Rama() 
        {
            mCategoriasDestino = new List<string>();
        }

        public Rama(SerializationInfo info, StreamingContext context)
        {
            mCategoriasDestino = (List<string>)info.GetValue("CategoriaDestino", typeof(List<string>));
        }

        public List<string> CategoriasDestino {
            get
            {
                return mCategoriasDestino;
            }
        } 

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CategoriaDestino", CategoriasDestino);
        }
    }

}
