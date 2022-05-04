using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Es.Riam.Util
{
    public enum TiposEventoSitemap
    {
        RecursoNuevo = 0,
        RecursoModificado = 1,
        RecursoEliminado = 2,
        Comunidad = 3
    }

    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class Sitemap
    {
        private List<Location> mLocations;

        public Sitemap()
        {
            mLocations = new List<Location>();
        }

        [XmlElement("url")]
        public List<Location> Locations
        {
            get
            {
                return mLocations;
            }
            set
            {
                mLocations = value;
            }
        }

        public void Add(Location item)
        {
            mLocations.Add(item);
        }

        public bool ContieneRecurso(Guid pDocumentoID)
        {
            bool salida = false;
            foreach (Location loc in Locations)
            {
                if (loc.Url.Contains(pDocumentoID.ToString()))
                {
                    salida = true;
                    break;
                }
            }
            return salida;
        }

        public Location ObtenerLocationPorDocumento(Guid pDocumentoID)
        {
            return Locations.Where(location => location.Url.Contains(pDocumentoID.ToString())).First();
        }

        public List<Location> ObtenerLocationsNoBorradas()
        {
            return Locations.Where(loc => loc.Eliminada == false).ToList();
        }
    }


    public class Location
    {
        public enum eChangeFrequency
        {
            always,
            hourly,
            daily,
            weekly,
            monthly,
            yearly,
            never
        }

        private List<AlternateLink> mAlternateLinks;

        [XmlIgnoreAttribute]
        private bool mEliminada;

        [XmlIgnoreAttribute]
        private long mFechaCreacion;

        public Location()
        {
            mAlternateLinks = new List<AlternateLink>();
            mEliminada = false;
            mFechaCreacion = System.DateTime.Now.Ticks;
        }

        public Location(long pFechaCreacion)
        {
            mAlternateLinks = new List<AlternateLink>();
            mEliminada = false;
            mFechaCreacion = pFechaCreacion;
        }

        [System.Xml.Serialization.XmlIgnore]
        public bool Eliminada
        {
            get
            {
                return mEliminada;
            }
            set
            {
                mEliminada = value;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public long FechaCreacion
        {
            get
            {
                return mFechaCreacion;
            }
            set
            {
                mFechaCreacion = value;
            }
        }

        [XmlElement("loc")]
        public string Url { get; set; }

        [XmlElement("changefreq")]
        public eChangeFrequency? ChangeFrequency { get; set; }
        public bool ShouldSerializeChangeFrequency() { return ChangeFrequency.HasValue; }

        [XmlElement("lastmod")]
        public DateTime? LastModified { get; set; }
        public bool ShouldSerializeLastModified() { return LastModified.HasValue; }

        [XmlElement("priority")]
        public double? Priority { get; set; }
        public bool ShouldSerializePriority() { return Priority.HasValue; }

        [XmlElement("link", Namespace = "http://www.w3.org/1999/xhtml")]
        public List<AlternateLink> AlternateLinks
        {
            get
            {
                return mAlternateLinks;
            }
            set
            {
                mAlternateLinks = value;
            }
        }

        public void Add(AlternateLink item)
        {
            mAlternateLinks.Add(item);
        }
    }

    [Serializable]
    public class AlternateLink
    {
        [XmlAttribute("rel")]
        public string Rel
        {
            get
            {
                return "alternate";
            }
            set { }
        }

        [XmlAttribute("hreflang")]
        public string Hreflang { get; set; }

        [XmlAttribute("href")]
        public string Href { get; set; }
    }

    [XmlRoot("sitemapindex", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class SitemapIndex
    {
        private int numLocations;
        private List<IndexLocation> mLocations;

        public SitemapIndex()
        {
            mLocations = new List<IndexLocation>();
            numLocations = 0;
        }

        [XmlElement("sitemap")]
        public List<IndexLocation> Locations
        {
            get
            {
                return mLocations;
            }
            set
            {
                mLocations = value;
            }
        }

        public int NumLocations
        {
            get
            {
                return numLocations;
            }
        }

        public void Add(IndexLocation item)
        {
            numLocations++;
            mLocations.Add(item);
        }

        public bool ContieneRecurso(string pNombreSitemap)
        {
            bool salida = false;
            foreach (IndexLocation loc in Locations)
            {
                if (loc.Url.Contains(pNombreSitemap))
                {
                    salida = true;
                }
            }
            return salida;
        }

        public IndexLocation ObtenerLocationPorSitemap(string pNombreSitemap)
        {
            return Locations.Where(location => location.Url.Contains(pNombreSitemap)).First();
        }

        public List<IndexLocation> ObtenerLocationsNoBorradas()
        {
            return Locations.Where(loc => loc.Eliminada == false).ToList();
        }

        public List<IndexLocation> ObtenerLocationPorComunidad(string pNombreCortoComunidad)
        {
            return Locations.Where(location => location.Url.Contains("_" + pNombreCortoComunidad + "_")).ToList();
        }
    }


    public class IndexLocation
    {
        [XmlIgnoreAttribute]
        private bool mEliminada;

        public IndexLocation()
        {
            mEliminada = false;
        }

        [XmlElement("loc")]
        public string Url { get; set; }

        [XmlElement("lastmod")]
        public DateTime? LastModified { get; set; }
        public bool ShouldSerializeLastModified() { return LastModified.HasValue; }

        [System.Xml.Serialization.XmlIgnore]
        public bool Eliminada
        {
            get
            {
                return mEliminada;
            }
            set
            {
                mEliminada = value;
            }
        }
    }
}
