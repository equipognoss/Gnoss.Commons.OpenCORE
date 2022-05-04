using System.Collections.Generic;

namespace Es.Riam.Gnoss.OAuthAD.OAuth.EncapsuladoDatos
{
    public class DataWrapperOAuth
    {
        public List<ConsumerData> ConsumerData
        {
            get; set;
        }

        public List<Nonce> Nonce
        {
            get;set;
        }

        public List<OAuthConsumer> OAuthConsumer
        {
            get;set;
        }

        public List<OAuthToken> OAuthToken
        {
            get; set;
        }

        public List<OAuthTokenExterno> OAuthTokenExterno
        {
            get;set;
        }

        public List<PinToken> PinToken
        {
            get;set;
        }
        public List<Usuario> Usuario
        {
            get;set;
        }
        public List<UsuarioConsumer> UsuarioConsumer
        {
            get;set;
        }



        public DataWrapperOAuth()
        {
            ConsumerData = new List<ConsumerData>();
            Nonce = new List<Nonce>();
            OAuthConsumer = new List<OAuthConsumer>();
            OAuthToken = new List<OAuthToken>();
            UsuarioConsumer = new List<UsuarioConsumer>();
            Usuario = new List<Usuario>();
            PinToken = new List<PinToken>();
            OAuthTokenExterno = new List<OAuthTokenExterno>();
        }

        public void Merge(DataWrapperOAuth dataWrapperOAuth)
        {
            this.ConsumerData.AddRange(dataWrapperOAuth.ConsumerData);
            this.Nonce.AddRange(dataWrapperOAuth.Nonce);
            this.OAuthConsumer.AddRange(dataWrapperOAuth.OAuthConsumer);
            Usuario.AddRange(dataWrapperOAuth.Usuario);
            PinToken.AddRange(dataWrapperOAuth.PinToken);
            OAuthTokenExterno.AddRange(dataWrapperOAuth.OAuthTokenExterno);
            this.OAuthToken.AddRange(dataWrapperOAuth.OAuthToken);
            this.UsuarioConsumer.AddRange(dataWrapperOAuth.UsuarioConsumer);
        }

    }
}
