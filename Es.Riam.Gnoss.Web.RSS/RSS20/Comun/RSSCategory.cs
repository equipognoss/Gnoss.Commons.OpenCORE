using System;
using System.Collections.Generic;
using System.Text;

namespace Es.Riam.Gnoss.Web.RSS.RSS20
{
    public class RSSCategory
    {
        private string mDomain;
        private string mCategory;

        /// <summary>
        /// Constructor con el nombre de la categor�a
        /// </summary>
        /// <param name="pCategory">Nombre de la categor�a</param>
        public RSSCategory(string pCategory)
        {
            mCategory = pCategory;
        }

        /// <summary>
        /// Constructor con el nombre de la categor�a y dominio
        /// </summary>
        /// <param name="pCategory">Nombre</param>
        /// <param name="pDomain">Dominio</param>
        public RSSCategory(string pCategory, string pDomain)
        {
            mCategory = pCategory;
            mDomain = pDomain;
        }

        /// <summary>
        /// Constructor sin par�metros
        /// </summary>
        public RSSCategory()
        {
        }

        /// <summary>
        /// Obtiene o establece el dominio de la categor�a
        /// </summary>
        public string Domain
        {
            get { return mDomain; }
            set { mDomain = value; }
        }

        /// <summary>
        /// Obtiene o establece el nombre de la categor�a
        /// </summary>
        public string Category
        {
            get { return mCategory; }
            set { mCategory = value; }
        }
    }
}
