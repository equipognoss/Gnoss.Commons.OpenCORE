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
        /// <param name="pTitle">Texto del bot�n del TextInput</param>
        /// <param name="pDescription">Descripci�n</param>
        /// <param name="pName">Nombre del objeto en el �rea del TextInput</param>
        /// <param name="pLink">URL del script CGI que procesa la peticion del TextInput</param>
        public RSSCanalTextInput(string pTitle, string pDescription, string pName, Uri pLink)
        {
            mTitle = pTitle;
            mDescription = pDescription;
            mName = pName;
            mLink = pLink;
        }

        /// <summary>
        /// Constructor sin par�metros
        /// </summary>
        public RSSCanalTextInput()
        {
        }

        /// <summary>
        /// Obtiene o establece el texto del bot�n del TextInput
        /// </summary>
        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; }
        }

        /// <summary>
        /// Obtiene o establece la descripci�n
        /// </summary>
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }

        /// <summary>
        /// Obtiene o establece el nombre del objeto en el �rea del TextInput
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
