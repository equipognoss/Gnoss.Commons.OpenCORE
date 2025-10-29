using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.Cookie
{
    public class CookieAD : BaseAD
    {
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        public CookieAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CookieAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public CookieAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CookieAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;

        }

        public bool ExistenCookiesTecnicas()
        {
            if (mEntityContext.CategoriaProyectoCookie.Where(item => item.NombreCorto.Equals("Tecnica")).Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CrearCookiesInicialesProyecto(Guid pProyectoID, Guid pOrganizacionID)
        {
            CategoriaProyectoCookie categoriaTecnica = new CategoriaProyectoCookie() { CategoriaID = Guid.NewGuid(), Nombre = "Técnica@es|||Technical@en|||Técnica@pt|||Tècnica@ca|||Teknikoak@eu|||Técnicas@gl|||Technique@fr|||Technische@de|||Tecnici@it", NombreCorto = "Tecnica", Descripcion = "Las cookies técnicas son aquellas imprescindibles y estrictamente necesarias para el correcto funcionamiento del Sitio Web y la utilización de las diferentes opciones y servicios que ofrece, incluyendo aquellas que el editor utiliza para permitir la gestión operativa de la página web y habilitar sus funciones y servicios. Por ejemplo, las que sirven para el mantenimiento de la sesión, la gestión del tiempo de respuesta, rendimiento o validación de opciones, utilizar elementos de seguridad, compartir contenido con redes sociales, etc. La página web no puede funcionar adecuadamente sin estas cookies.@es|||Technical cookies are those essential and strictly necessary for the proper functioning of the Website and the use of the different options and services it offers, including those that the editor uses to enable the operational management of the website and enable its functions and services. For example, those used for session maintenance, response time management, performance or validation of options, use of security elements, sharing content with social networks, etc. The website cannot function properly without these cookies.@en|||Os cookies técnicos são aqueles essenciais e estritamente necessários para o bom funcionamento do Website e a utilização das diferentes opções e serviços que este oferece, incluindo os que o editor utiliza para permitir a gestão operacional do Website e permitir as suas funções e serviços. Por exemplo, os utilizados para manutenção de sessões, gestão do tempo de resposta, desempenho ou validação de opções, utilização de elementos de segurança, partilha de conteúdos com redes sociais, etc. O website não pode funcionar correctamente sem estes cookies.@pt|||Les cookies tècniques són aquelles imprescindibles i estrictament necessàries per al correcte funcionament del Lloc Web i la utilització de les diferents opcions i serveis que ofereix, incloent-hi aquelles que l'editor utilitza per permetre la gestió operativa de la pàgina web i habilitar-ne les funcions i serveis. Per exemple, les que serveixen per al manteniment de la sessió, la gestió del temps de resposta, el rendiment o la validació d'opcions, utilitzar elements de seguretat, compartir contingut amb xarxes socials, etc. La pàgina web no pot funcionar adequadament sense aquestes cookies.@ca|||Cookie teknikoak ezinbestekoak eta behar-beharrezkoak dira Web Gunearen funtzionamendu egokirako eta eskaintzen dituen aukera eta zerbitzuak erabiltzeko, editoreak web orriaren kudeaketa operatiboa ahalbidetzeko eta haren funtzio eta zerbitzuak gaitzeko erabiltzen dituenak barne. Adibidez, saioari eusteko balio dutenak, erantzun-denboraren kudeaketa, aukeren errendimendua edo baliozkotzea, segurtasun-elementuak erabiltzea, edukiak sare sozialekin partekatzea, etab. Web orriak ezin du behar bezala funtzionatu cookie hauek gabe.@eu|||As cookies técnicas son aquelas esenciais e estritamente necesarias para o bo funcionamento do Sitio Web e o uso das diferentes opcións e servizos que ofrece, incluídas as que utiliza o editor para permitir a xestión operativa do sitio web e habilitar as súas funcións. Servizos. Por exemplo, os utilizados para manter a sesión, xestionar o tempo de resposta, o rendemento ou validación de opcións, utilizar elementos de seguridade, compartir contidos coas redes sociais, etc. O sitio web non pode funcionar correctamente sen estas cookies.@gl|||Les cookies techniques sont ceux qui sont essentiels et strictement nécessaires au bon fonctionnement du site web et à l'utilisation des différentes options et services qu'il propose, y compris ceux que l'éditeur utilise pour permettre la gestion opérationnelle du site web et permettre ses fonctions et services. Par exemple, ceux utilisés pour la maintenance des sessions, la gestion du temps de réponse, l'exécution ou la validation des options, l'utilisation des éléments de sécurité, le partage de contenu avec les réseaux sociaux, etc. Le site web ne peut pas fonctionner correctement sans ces cookies.@fr|||Technische Cookies sind diejenigen, die für das ordnungsgemäße Funktionieren der Website und die Nutzung der verschiedenen Optionen und Dienstleistungen, die sie bietet, unbedingt erforderlich sind, einschließlich derjenigen, die der Herausgeber verwendet, um die operative Verwaltung der Website zu ermöglichen und ihre Funktionen und Dienstleistungen zu ermöglichen. Zum Beispiel solche, die für die Aufrechterhaltung von Sitzungen, die Verwaltung von Antwortzeiten, die Leistung oder Validierung von Optionen, die Verwendung von Sicherheitselementen, die gemeinsame Nutzung von Inhalten in sozialen Netzwerken usw. verwendet werden. Ohne diese Cookies kann die Website nicht richtig funktionieren.@de|||I cookie tecnici sono quelli essenziali e strettamente necessari per il corretto funzionamento del sito web e per l'utilizzo delle diverse opzioni e dei servizi offerti, compresi quelli che l'editore utilizza per consentire la gestione operativa del sito web e abilitarne le funzioni e i servizi. Ad esempio, quelli utilizzati per il mantenimento delle sessioni, la gestione dei tempi di risposta, le prestazioni o la convalida delle opzioni, l'uso di elementi di sicurezza, la condivisione di contenuti con i social network, ecc. Il sito web non può funzionare correttamente senza questi cookie.@it", EsCategoriaTecnica = true, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID };

            ProyectoCookie cookieAvisoGnoss = new ProyectoCookie() { CategoriaID = categoriaTecnica.CategoriaID, CookieID = Guid.NewGuid(), Nombre = "cookieAviso.gnoss.com", NombreCorto = "cookieAvisoGnoss", Descripcion = "Se sa para saber si el usuario ha aceptado la política de cookies", Tipo = (short)TipoCookies.Persistent, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID, EsEditable = false };
            ProyectoCookie cookieAspNetSesionID = new ProyectoCookie() { CategoriaID = categoriaTecnica.CategoriaID, CookieID = Guid.NewGuid(), Nombre = "ASP.NET_SessionId", NombreCorto = "aspNetSessionId", Descripcion = "Almacena la sesión del usuario actual", Tipo = (short)TipoCookies.Session, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID, EsEditable = false };
            ProyectoCookie cookieIdiomaActual = new ProyectoCookie() { CategoriaID = categoriaTecnica.CategoriaID, CookieID = Guid.NewGuid(), Nombre = "IdiomaActual", NombreCorto = "idiomaActual", Descripcion = "Se usa para almacenar el idioma de navegación del usuario", Tipo = (short)TipoCookies.Session, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID, EsEditable = false };
            ProyectoCookie cookieUsuarioLogueado = new ProyectoCookie() { CategoriaID = categoriaTecnica.CategoriaID, CookieID = Guid.NewGuid(), Nombre = "UsuarioLogueado", NombreCorto = "usuarioLogueado", Descripcion = "Se usa para saber si el usuario ha hecho login", Tipo = (short)TipoCookies.Session, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID, EsEditable = false };
            ProyectoCookie cookieSesionUsuarioAvtiva = new ProyectoCookie() { CategoriaID = categoriaTecnica.CategoriaID, CookieID = Guid.NewGuid(), Nombre = "SesionUsuarioActiva", NombreCorto = "sesionUsuarioActiva", Descripcion = "Almacena la duración de la sesión del usuario", Tipo = (short)TipoCookies.Session, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID, EsEditable = false };
        }

        public List<CategoriaProyectoCookie> ObtenerCategoriasProyectoCookie(Guid pProyectoID, bool pTraerCategoriasEcosistema = true)
        {
            if (pTraerCategoriasEcosistema)
            {
				return mEntityContext.CategoriaProyectoCookie.Where(item => item.ProyectoID.Equals(pProyectoID)).Union(mEntityContext.CategoriaProyectoCookie.Where(item => item.ProyectoID.Equals(ProyectoAD.MetaProyecto))).ToList();
			}
            else
            {
				return mEntityContext.CategoriaProyectoCookie.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
			}                
        }

        public bool TieneCategoriaCookiesVinculadas(Guid pCategoriaID)
        {
            return mEntityContext.ProyectoCookie.Where(item => item.CategoriaID.Equals(pCategoriaID)).Any();
        }

        public bool ExistenCookiesYoutube()
        {
            Guid categoriaIDRedesSociales = mEntityContext.CategoriaProyectoCookie.Where(item => item.NombreCorto.Equals("Redes sociales")).Select(item => item.CategoriaID).FirstOrDefault();
            if (categoriaIDRedesSociales.Equals(Guid.Empty))
            {
                return mEntityContext.ProyectoCookie.Where(item => item.CategoriaID.Equals(categoriaIDRedesSociales)).Any();
            }
            return false;
        }

        public bool ExistenCookiesAnaliticas()
        {
            Guid categoriaIDCookiesAnaliticas = mEntityContext.CategoriaProyectoCookie.Where(item => item.NombreCorto.Equals("Analiticas")).Select(item => item.CategoriaID).FirstOrDefault();
            if (categoriaIDCookiesAnaliticas != null && categoriaIDCookiesAnaliticas.Equals(Guid.Empty))
            {
                return mEntityContext.ProyectoCookie.Where(item => item.CategoriaID.Equals(categoriaIDCookiesAnaliticas)).Any();
            }
            return false;
        }

        public List<ProyectoCookie> ObtenerCookiesDeProyecto(Guid pProyectoID)
        {
            return mEntityContext.ProyectoCookie.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
        }

        public ProyectoCookie ObtenerCookiePorId(Guid pCookieID, Guid pProyectoID)
        {
            return mEntityContext.ProyectoCookie.Where(item => item.CookieID.Equals(pCookieID) && item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
        }

        public CategoriaProyectoCookie ObtenerCategoriaPorId(Guid pCategoriaID, Guid pProyectoID)
        {
            return mEntityContext.CategoriaProyectoCookie.Where(item => item.CategoriaID.Equals(pCategoriaID) && item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
        }

        public CategoriaProyectoCookie ObtenerCategoriaPorNombreCorto(string pNombreCorto, Guid pProyectoID)
        {
            return mEntityContext.CategoriaProyectoCookie.Where(item => item.NombreCorto.Equals(pNombreCorto) && item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
        }

        public bool HayCookiesVinculadas(Guid pCategoriaID)
        {
            return mEntityContext.ProyectoCookie.Where(item => item.CategoriaID.Equals(pCategoriaID)).Any();
        }

        public List<ProyectoCookie> ObtenerCookiesDeCategoria(Guid pCategoriaID)
        {
            return mEntityContext.ProyectoCookie.Where(item => item.CategoriaID.Equals(pCategoriaID)).ToList();
        }

    }
}
