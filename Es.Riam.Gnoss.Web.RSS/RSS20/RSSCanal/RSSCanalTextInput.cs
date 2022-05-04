using System;

namespace Es.Riam.Gnoss.Web.RSS.RSS20
{
    //Describe un campo TextInput que se muestra con el feed
    public class RSSCanalTextInput
    {
        private string mTitle;
        private string mDescription;
        private string mName;
        private Uri mLink;

        /// <summary>
        /// Constructor con los campos obligatorios del TextInput
        /// </summary>
        /// <param name="pTitle">Texto del botón del TextInput</param>
        /// <param name="pDescription">Descripción</param>
        /// <param name="pName">Nombre del objeto en el área del TextInput</param>
        /// <param name="pLink">URL del script CGI que procesa la peticion del TextInput</param>
        public RSSCanalTextInput(string pTitle, string pDescription, string pName, Uri pLink)
        {
            mTitle = pTitle;
            mDescription = pDescription;
            mName = pName;
            mLink = pLink;
        }

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public RSSCanalTextInput()
        {
        }

        /// <summary>
        /// Obtiene o establece el texto del botón del TextInput
        /// </summary>
        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; }
        }

        /// <summary>
        /// Obtiene o establece la descripción
        /// </summary>
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }

        /// <summary>
        /// Obtiene o establece el nombre del objeto en el área del TextInput
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Obtiene o establece la URL del script CGI que procesa la peticion del TextInput
        /// </summary>
        public Uri Link
        {
            get { return mLink; }
            set { mLink = value; }
        }
    }
}
