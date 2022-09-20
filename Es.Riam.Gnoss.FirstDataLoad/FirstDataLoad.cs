using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Pais;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.FirstDataLoad.Properties;
using Es.Riam.Gnoss.OAuthAD;
using Es.Riam.Gnoss.OAuthAD.OAuth;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.FirstDataLoad
{
    public class FirstDataLoad
    {
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private EntityContextOauth mEntityContextOauth;
        public FirstDataLoad(EntityContext entityContext, ConfigService configService, EntityContextOauth entityContextOauth)
        {
            mEntityContext = entityContext;
            mConfigService = configService;
            mEntityContextOauth = entityContextOauth;
        }

        public void InsertData()
        {
            ParametroAplicacion parametroAplicacion1 = new ParametroAplicacion()
            {
                Parametro = "EcosistemaSinBandejaSuscripciones",
                Valor = "false"
            };
            ParametroAplicacion parametroAplicacion2 = new ParametroAplicacion()
            {
                Parametro = "EcosistemaSinContactos",
                Valor = "false"
            };
            ParametroAplicacion parametroAplicacion3 = new ParametroAplicacion()
            {
                Parametro = "NombreEspacioPersonal",
                Valor = "Mi espacio personal@es|||My personal space@en|||Meu espaco pessoal@pt"
            };
            ParametroAplicacion parametroAplicacion4 = new ParametroAplicacion()
            {
                Parametro = "PuertoSmtp",
                Valor = "25"
            };
            ParametroAplicacion parametroAplicacion5 = new ParametroAplicacion()
            {
                Parametro = "ScriptGoogleAnalytics",
                Valor = "var gaJsHost = ((\"https:\" == document.location.protocol) ? \"https://ssl.\" : \"http://www.\");document.write(unescape(\"%3Cscript src=''\" + gaJsHost + \"google-analytics.com/ga.js'' type=''text/javascript''%3E%3C/script%3E\"));</script><script type=\"text/javascript\">try{ pageTracker = _gat._getTracker(\"@@codigoga@@\");pageTracker._trackPageview();} catch(err) {}"
            };
            ParametroAplicacion parametroAplicacion6 = new ParametroAplicacion()
            {
                Parametro = "UrlIntragnoss",
                Valor = "http://gnoss.com/"
            };

            ParametroAplicacion parametroAplicacion7 = new ParametroAplicacion()
            {
                Parametro = "UrlsPropiasProyecto",
                Valor = "beta"
            };

            ParametroAplicacion parametroAplicacion8 = new ParametroAplicacion()
            {
                Parametro = "CorreoSolicitudes",
                Valor = "solicitudes@gnoss.com"
            };

            CategoriaProyectoCookie categoriaProyectoCookie = new CategoriaProyectoCookie()
            {
                CategoriaID = new Guid("F4DE719F-B93A-4015-84A9-F1F1A2C11533"),
                Nombre = "Técnica@es|||Technical@en",//|||Técnica@pt|||Tècnica@ca|||Teknikoak@eu|||Técnicas@gl|||Technique@fr|||Technische@de|||Tecnici@it
                NombreCorto = "Tecnica",
                Descripcion = "Las cookies técnicas son aquellas imprescindibles y estrictamente necesarias para el correcto funcionamiento del Sitio Web y la utilización de las diferentes opciones y servicios que ofrece, incluyendo aquellas que el editor utiliza para permitir la gestión operativa de la página web y habilitar sus funciones y servicios. Por ejemplo, las que sirven para el mantenimiento de la sesión, la gestión del tiempo de respuesta, rendimiento o validación de opciones, utilizar elementos de seguridad, compartir contenido con redes sociales, etc. La página web no puede funcionar adecuadamente sin estas cookies.@es|||Technical cookies are those essential and strictly necessary for the proper functioning of the Website and the use of the different options and services it offers, including those that the editor uses to enable the operational management of the website and enable its functions and services. For example, those used for session maintenance, response time management, performance or validation of options, use of security elements, sharing content with social networks, etc. The website cannot function properly without these cookies.@en",
                EsCategoriaTecnica = true,
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111")
                //Las cookies técnicas son aquellas imprescindibles y estrictamente necesarias para el correcto funcionamiento del Sitio Web y la utilización de las diferentes opciones y servicios que ofrece, incluyendo aquellas que el editor utiliza para permitir la gestión operativa de la página web y habilitar sus funciones y servicios. Por ejemplo, las que sirven para el mantenimiento de la sesión, la gestión del tiempo de respuesta, rendimiento o validación de opciones, utilizar elementos de seguridad, compartir contenido con redes sociales, etc. La página web no puede funcionar adecuadamente sin estas cookies.@es|||Technical cookies are those essential and strictly necessary for the proper functioning of the Website and the use of the different options and services it offers, including those that the editor uses to enable the operational management of the website and enable its functions and services. For example, those used for session maintenance, response time management, performance or validation of options, use of security elements, sharing content with social networks, etc. The website cannot function properly without these cookies.@en|||Os cookies técnicos são aqueles essenciais e estritamente necessários para o bom funcionamento do Website e a utilização das diferentes opções e serviços que este oferece, incluindo os que o editor utiliza para permitir a gestão operacional do Website e permitir as suas funções e serviços. Por exemplo, os utilizados para manutenção de sessões, gestão do tempo de resposta, desempenho ou validação de opções, utilização de elementos de segurança, partilha de conteúdos com redes sociais, etc. O website não pode funcionar correctamente sem estes cookies.@pt|||Les cookies tècniques són aquelles imprescindibles i estrictament necessàries per al correcte funcionament del Lloc Web i la utilització de les diferents opcions i serveis que ofereix, incloent-hi aquelles que l'editor utilitza per permetre la gestió operativa de la pàgina web i habilitar-ne les funcions i serveis. Per exemple, les que serveixen per al manteniment de la sessió, la gestió del temps de resposta, el rendiment o la validació d'opcions, utilitzar elements de seguretat, compartir contingut amb xarxes socials, etc. La pàgina web no pot funcionar adequadament sense aquestes galetes.@ca|||Cookie teknikoak ezinbestekoak eta behar-beharrezkoak dira Web Gunearen funtzionamendu egokirako eta eskaintzen dituen aukera eta zerbitzuak erabiltzeko, editoreak web orriaren kudeaketa operatiboa ahalbidetzeko eta haren funtzio eta zerbitzuak gaitzeko erabiltzen dituenak barne. Adibidez, saioari eusteko balio dutenak, erantzun-denboraren kudeaketa, aukeren errendimendua edo baliozkotzea, segurtasun-elementuak erabiltzea, edukiak sare sozialekin partekatzea, etab. Web orriak ezin du behar bezala funtzionatu cookie hauek gabe.@eu|||As cookies técnicas son aquelas esenciais e estritamente necesarias para o bo funcionamento do Sitio Web e o uso das diferentes opcións e servizos que ofrece, incluídas as que utiliza o editor para permitir a xestión operativa do sitio web e habilitar as súas funcións. Servizos. Por exemplo, os utilizados para manter a sesión, xestionar o tempo de resposta, o rendemento ou validación de opcións, utilizar elementos de seguridade, compartir contidos coas redes sociais, etc. O sitio web non pode funcionar correctamente sen estas cookies.@gl|||Les cookies techniques sont ceux qui sont essentiels et strictement nécessaires au bon fonctionnement du site web et à l'utilisation des différentes options et services qu'il propose, y compris ceux que l'éditeur utilise pour permettre la gestion opérationnelle du site web et permettre ses fonctions et services. Par exemple, ceux utilisés pour la maintenance des sessions, la gestion du temps de réponse, l'exécution ou la validation des options, l'utilisation des éléments de sécurité, le partage de contenu avec les réseaux sociaux, etc. Le site web ne peut pas fonctionner correctement sans ces cookies.@fr|||Technische Cookies sind diejenigen, die für das ordnungsgemäße Funktionieren der Website und die Nutzung der verschiedenen Optionen und Dienstleistungen, die sie bietet, unbedingt erforderlich sind, einschließlich derjenigen, die der Herausgeber verwendet, um die operative Verwaltung der Website zu ermöglichen und ihre Funktionen und Dienstleistungen zu ermöglichen. Zum Beispiel solche, die für die Aufrechterhaltung von Sitzungen, die Verwaltung von Antwortzeiten, die Leistung oder Validierung von Optionen, die Verwendung von Sicherheitselementen, die gemeinsame Nutzung von Inhalten in sozialen Netzwerken usw. verwendet werden. Ohne diese Cookies kann die Website nicht richtig funktionieren.@de|||I cookie tecnici sono quelli essenziali e strettamente necessari per il corretto funzionamento del sito web e per l'utilizzo delle diverse opzioni e dei servizi offerti, compresi quelli che l'editore utilizza per consentire la gestione operativa del sito web e abilitarne le funzioni e i servizi. Ad esempio, quelli utilizzati per il mantenimento delle sessioni, la gestione dei tempi di risposta, le prestazioni o la convalida delle opzioni, l'uso di elementi di sicurezza, la condivisione di contenuti con i social network, ecc. Il sito web non può funzionare correttamente senza questi cookie.@it
            };
            mEntityContext.CategoriaProyectoCookie.Add(categoriaProyectoCookie);
            ProyectoCookie proyectoCookieAviso = new ProyectoCookie()
            {
                CookieID = new Guid("18A367C1-4BC8-4099-AAE8-13B1E6D4D3CC"),
                Nombre = "cookieAviso.gnoss.com",
                NombreCorto = "cookieAvisoGnoss",
                Tipo = 0,
                Descripcion = "Se usa para saber si el usuario ha aceptado la política de cookies",
                EsEditable = false,
                CategoriaID = new Guid("F4DE719F-B93A-4015-84A9-F1F1A2C11533"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111")
            };
            ProyectoCookie proyectoCookieLogueado = new ProyectoCookie()
            {
                CookieID = new Guid("127AFD7D-F000-421C-AA55-22E457D7E22F"),
                Nombre = "UsuarioLogueado",
                NombreCorto = "usuarioLogueado",
                Tipo = 1,
                Descripcion = "Se usa para saber si el usuario ha hecho login",
                EsEditable = false,
                CategoriaID = new Guid("F4DE719F-B93A-4015-84A9-F1F1A2C11533"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111")
            };
            ProyectoCookie proyectoCookieSession = new ProyectoCookie()
            {
                CookieID = new Guid("79AA58F3-C0F4-462C-A37F-5BEE4C5B3681"),
                Nombre = "ASP.NET_SessionId",
                NombreCorto = "aspNetSessionId",
                Tipo = 1,
                Descripcion = "Almacena la sesión del usuario actual",
                EsEditable = false,
                CategoriaID = new Guid("F4DE719F-B93A-4015-84A9-F1F1A2C11533"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111")
            };
            ProyectoCookie proyectoCookieSessionActive = new ProyectoCookie()
            {
                CookieID = new Guid("7134618E-C027-4610-B30A-76E546B2D8FB"),
                Nombre = "SesionUsuarioActiva",
                NombreCorto = "sesionUsuarioActiva",
                Tipo = 1,
                Descripcion = "Almacena la duración de la sesión del usuario",
                EsEditable = false,
                CategoriaID = new Guid("F4DE719F-B93A-4015-84A9-F1F1A2C11533"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111")
            };
            ProyectoCookie proyectoCookieIdioma = new ProyectoCookie()
            {
                CookieID = new Guid("6DF0E230-93B3-42D8-B6FD-F2336671C58F"),
                Nombre = "IdiomaActual",
                NombreCorto = "idiomaActual",
                Tipo = 1,
                Descripcion = "Se usa para almacenar el idioma de navegación del usuario",
                EsEditable = false,
                CategoriaID = new Guid("F4DE719F-B93A-4015-84A9-F1F1A2C11533"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111")
            };
            mEntityContext.ProyectoCookie.Add(proyectoCookieAviso);
            mEntityContext.ProyectoCookie.Add(proyectoCookieLogueado);
            mEntityContext.ProyectoCookie.Add(proyectoCookieSession);
            mEntityContext.ProyectoCookie.Add(proyectoCookieSessionActive);
            mEntityContext.ProyectoCookie.Add(proyectoCookieIdioma);


            mEntityContext.ParametroAplicacion.Add(parametroAplicacion1);
            mEntityContext.ParametroAplicacion.Add(parametroAplicacion2);
            mEntityContext.ParametroAplicacion.Add(parametroAplicacion3);
            mEntityContext.ParametroAplicacion.Add(parametroAplicacion4);
            mEntityContext.ParametroAplicacion.Add(parametroAplicacion5);
            mEntityContext.ParametroAplicacion.Add(parametroAplicacion6);
            mEntityContext.ParametroAplicacion.Add(parametroAplicacion7);
            mEntityContext.ParametroAplicacion.Add(parametroAplicacion8);

            ProyectoPestanyaMenu proyectoPestanyaMenu1 = new ProyectoPestanyaMenu()
            {
                PestanyaID = new Guid("0B449C0B-9E2A-4A03-B436-299DE6668968"),
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                PestanyaPadreID = null,
                TipoPestanya = (short)TipoPestanyaMenu.BusquedaAvanzada,
                Nombre = "",
                Ruta = "",
                Orden = 2,
                NuevaPestanya = false,
                Visible = true,
                Privacidad = 0,
                HtmlAlternativo = "",
                IdiomasDisponibles = null,
                Titulo = null,
                NombreCortoPestanya = "0B449C0B-9E2A-4A03-B436-299DE6668968",
                VisibleSinAcceso = true,
                CSSBodyClass = null,
                Activa = true,
                MetaDescription = null,
            };
            ProyectoPestanyaMenu proyectoPestanyaMenu2 = new ProyectoPestanyaMenu()
            {
                PestanyaID = new Guid("317F13FE-8C11-47A2-A886-E3A615D831B8"),
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                PestanyaPadreID = null,
                TipoPestanya = (short)TipoPestanyaMenu.PersonasYOrganizaciones,
                Nombre = "",
                Ruta = "",
                Orden = 1,
                NuevaPestanya = false,
                Visible = true,
                Privacidad = 0,
                HtmlAlternativo = "",
                IdiomasDisponibles = null,
                Titulo = null,
                NombreCortoPestanya = "317F13FE-8C11-47A2-A886-E3A615D831B8",
                VisibleSinAcceso = true,
                CSSBodyClass = null,
                Activa = true,
                MetaDescription = null,
            };
            ProyectoPestanyaMenu proyectoPestanyaMenu3 = new ProyectoPestanyaMenu()
            {
                PestanyaID = new Guid("6883B119-F852-4161-8F93-E3BB96756453"),
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                PestanyaPadreID = null,
                TipoPestanya = (short)TipoPestanyaMenu.Comunidades,
                Nombre = "",
                Ruta = "",
                Orden = 0,
                NuevaPestanya = false,
                Visible = true,
                Privacidad = 0,
                HtmlAlternativo = "",
                IdiomasDisponibles = null,
                Titulo = null,
                NombreCortoPestanya = "6883B119-F852-4161-8F93-E3BB96756453",
                VisibleSinAcceso = true,
                CSSBodyClass = null,
                Activa = true,
                MetaDescription = null,
            };
            mEntityContext.ProyectoPestanyaMenu.Add(proyectoPestanyaMenu1);
            mEntityContext.ProyectoPestanyaMenu.Add(proyectoPestanyaMenu2);
            mEntityContext.ProyectoPestanyaMenu.Add(proyectoPestanyaMenu3);

            Organizacion organizacion = new Organizacion()
            {
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111"),
                Nombre = "Meta organización GNOSS",
                EsBuscable = true,
                EsBuscableExternos = false,
                ModoPersonal = true,
                Eliminada = false,
                NombreCorto = "meta-organizacion-gn",
                Alias = "Meta organización GNOSS"
            };
            mEntityContext.Organizacion.Add(organizacion);

            string urlPropia = "http://localhost";
            if(!string.IsNullOrEmpty(mConfigService.ObtenerDominio()))
            {
                if (mConfigService.PeticionHttps() && !mConfigService.ObtenerDominio().Contains("http"))
                {
                    urlPropia = $"https://{mConfigService.ObtenerDominio()}";
                }
                else if (!mConfigService.PeticionHttps() && !mConfigService.ObtenerDominio().Contains("http"))
                {
                    urlPropia = $"http://{mConfigService.ObtenerDominio()}";
                }
                
            } 
            Proyecto proyecto = new Proyecto()
            {
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                Nombre = "GNOSS",
                Descripcion = "MyGNOSS es el espacio privado de las personas y de las organizaciones en GNOSS; el lugar desde el que pueden presentarse y disponer de una compleja identidad digital; el lugar donde gestionar sus intereses profesionales y su presencia socio-digital en el espacio social y digital complejo que es GNOSS. Es también el espacio privado de gestión de los intereses múltiples de una persona u organización, capaz de conectarlo con las diferentes comunidades en las que participa dentro del universo GNOSS.",
                FechaInicio = new DateTime(),
                TipoProyecto = 2,
                TipoAcceso = 0,
                NumeroRecursos = 0,
                NumeroPreguntas = 0,
                NumeroDebates = 0,
                NumeroMiembros = 1,
                NumeroOrgRegistradas = 0,
                NumeroArticulos = 0,
                NumeroDafos = 0,
                NumeroForos = 0,
                EsProyectoDestacado = false,
                NombreCorto = "mygnoss",
                URLPropia = urlPropia, 
                Estado = 3,
                TieneTwitter = false,
                EnviarTwitterComentario = false,
                EnviarTwitterNuevaCat = false,
                EnviarTwitterNuevoAdmin = false,
                EnviarTwitterNuevaPolitCert = false,
                EnviarTwitterNuevoTipoDoc = false,
                TagTwitterGnoss = true
            };
            mEntityContext.Proyecto.Add(proyecto);

            ParametroGeneral parametroGeneral = new ParametroGeneral()
            {
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                UmbralSuficienciaEnMejora = 0,
                DesviacionAdmitidaEnEvalua = 0,
                MetaAutomatPropietarioPro = 0,
                AvisoLegal = "<span id=\"ctl00_CPH1_lblInfo\">      <p>Al acceder o usar el servicio, manifiestas haber leído las <a href=\"#CondicionesAcceso\">condiciones de acceso</a> y    reglas de convivencia de su <a href=\"#CodigoEtico\"> código ético</a>.</p> <a name=\"CondicionesAcceso\"><h3 class=\"generico\">Condiciones de acceso:</h3></a>    <div class=\"sepHz\"></div> <p>Las presentes condiciones regulan el uso del servicio web GNOSS. </p> <h4 class=\"textBox\">Requisitos de acceso:</h4> <p>Para    crear una cuenta de usuario en Gnoss, debes contar con al menos 18 años. Al registrarte en Gnoss, declaras que cumples con este requisito y estas capacitado    para entender, aceptar y cumplir con las <a href=\"#CondicionesAcceso\">condiciones de acceso</a> y <a href=\"#CodigoEtico\">código ético</a> que rigen el    funcionamiento de esta red social.</p> <p>Al tramitar el alta como usuario, deberás elegir unas claves de acceso. Cada vez que desees entrar en la red, el    sistema te solicitará estas claves para autentificarte, siendo responsable de todas las acciones que se realicen con tu nombre de usuario y contraseña.</p>    <p>RIAM I+L Lab no se responsabiliza de la custodia que realices de estos elementos identificativos, ni el uso que un tercero haga de éstos sin tu    consentimiento.</p> <p>RIAM I+L Lab puede restringir el uso de determinados nombres de usuario que vulneren derechos de terceros.</p> <h4    class=\"textBox\">Procedimiento de alta en Gnoss:</h4> <p>Para darte de alta en GNOSS es necesario que te registres. En la página <a    href=\"htp://www.gnoss.com\">www.gnoss.com</a> podrás hacerlo pinchando en crear cuenta. En este proceso de registro te pediremos tus datos de acceso a la    cuenta y tus datos personales. Te informaremos de la resolución de tu solicitud de registro en tu cuenta de correo de contacto.</p> <h4    class=\"textBox\">Derechos de propiedad intelectual e industrial:</h4> <p>Todos los elementos susceptibles de propiedad intelectual que se muestran en esta    plataforma y que son necesarios para su funcionamiento, así como en las páginas que lo forman, se encuentran protegidos a favor de <a href=\"riam.aspx\">RIAM    I+L Lab</a>.</p> <p>Conforme a las leyes que protegen el derecho de autor, no puedes publicar, modificar, distribuir ni reproducir materiales protegidos por    estas leyes, marcas registradas o cualquier otro material que pertenezca a otras personas sin obtener el consentimiento previo del titular de dichos    derechos. Como usuario que sube estos materiales eres el responsable de obtener esta autorización, sin que pueda derivarse responsabilidades a RIAM I+L    Lab.</p> <h4 class=\"textBox\">Protección de datos:</h4> <p>La protección de datos personales es una de las prioridades en GNOSS. Por ello, RIAM I+L Lab como    titular de esta red social, garantizará en todo momento la privacidad de los datos considerados como personales que nos facilites a través de formularios y/o    hojas de inscripción, foros, perfil, etc. Estos datos pasarán a formar parte de un fichero automatizado que, conforme a la Ley 15/1999 de 13 de Diciembre y a    su normativa de desarrollo, cumple con las medidas técnicas, organizativas y humanas necesarias para garantizar la seguridad y confidencialidad de los    mismos.</p> <p>La recogida y tratamiento automatizado de estos datos personales tiene como finalidad principal la correcta gestión y desarrollo de las    Comunidades de conocimiento, la identificación de las personas que participan en la red, así como la obtención de información básica para responder a las    consultas que nos puedas plantear y otras actividades propias de RIAM I+L Lab.</p> <p>RIAM I+L Lab reconoce el ejercicio de tus derechos de acceso,    rectificación, cancelación y oposición conforme a la legislación vigente. Para ello, debes dirigirnos tu solicitud a través de <a    href=\"mailto:privacidad@gnoss.com\">privacidad@gnoss.com</a>.</p> <h4 class=\"textBox\">Finalización de servicio: </h4> <p>Del mismo modo que tramitas el alta,    puedes cancelar tu cuenta dirigiéndote a <a href=\"mailto:solicitudes@gnoss.com\">solicitudes@gnoss.com</a>. Al tramitar la cancelación, tu perfil será    eliminado así como la información visible para otros usuarios.</p> <p>La eliminación de estos datos garantiza el cumplimiento de la política de protección de    datos.</p> <p>RIAM I+L Lab puede, a su entera disposición y en cualquier momento, interrumpir temporal o permanentemente el acceso y/o servicio. Esta    interrupción puede realizarla sin previo aviso y por el hecho de aceptar estas condiciones al registrarte estás aceptando la desactivación de tus cuentas y    perfiles en GNOSS. RIAM I+L Lab no será responsable frente a terceros o el propio usuario de la finalización o suspensión del acceso o modificación del    mismo.</p> <h4 class=\"textBox\">Responsabilidades:</h4> <p>RIAM I+L Lab no se responsabiliza de las opiniones, declaraciones, manifestaciones y comentarios    que realices desde las diferentes herramientas que en esta red social ponemos a tu disposición, siendo directamente responsable el titular de las claves de    acceso a la red.</p> <p>Para conseguir los fines para los que GNOSS ha sido creado, queda prohibido el uso de los perfiles de usuario para actos que sean    considerados como ilegales o no autorizados, que sean un ataque para otros usuarios, fraudulentos, difamatorios o que incumplan nuestro <a    href=\"#CodigoEtico\">código ético</a>.</p> <p>RIAM I+L Lab se reserva el derecho a desactivar los nombres de usuarios y cuentas que hayan estado inactivos    durante más de 6 meses.</p> <a name=\"CodigoEtico\"><h3 class=\"generico\">Código ético:</h3></a> <div class=\"sepHz\"></div> <p>El siguiente código constituye el    marco para el buen uso de GNOSS. Este código ético recoge las reglas de convivencia dentro de la red que favorecen un entorno positivo de intercambio de    trabajo, opiniones y valores.</p> <p>Su objetivo principal es establecer unas directrices que permitan proteger al máximo la privacidad, los derechos que por    ley son reconocidos y la experiencia global de la red, siendo un complemento a las <a href=\"#CondicionesAcceso\">condiciones de acceso</a>.</p> <p>Si detectas    cualquier violación por parte de un tercero de las normas de la red, podrás comunicárnoslo enviando un correo electrónico a <a    href=\"mailto:reclamaciones@gnoss.com\">reclamaciones@gnoss.com</a>.</p> <p>En GNOSS favorecemos el intercambio de conocimiento de forma libre y sin ningún    tipo de restricción inicialmente. No obstante, existen las limitaciones legales y contractuales relativas al contenido y el comportamiento entre    usuarios.</p> <h4 class=\"textBox\">Identidad del usuario:</h4> <p>Debes facilitarnos datos ciertos al crear tu perfil y no mentirnos con la edad. Ya sabes,    conforme a los establecido en las condiciones de acceso, que los menores de 18 años no deben acceder a GNOSS.</p> <h4 class=\"textBox\">Contenidos:</h4> <p>Con    igual conformidad a lo establecido en las condiciones de acceso y uso, no debes intercambiar, compartir, guardar o favorecer, a través de nuestra red,    materiales, opiniones o comentarios que infrinjan la normativa legal y las costumbres.</p> <h4 class=\"textBox\">Invitaciones a la Comunidad:</h4> <p>Cuando    invites a participar a otras personas a una comunidad en la que colaboras, debes asegurarte de que conocen y aceptan estas condiciones de uso. Si GNOSS    estima que se han infringido las normas de la comunidad por alguna de las personas invitadas, puede suspender o cancelar ambos perfiles para salvaguarda de    los intereses generales de la comunidad en la que están participando y la integridad de GNOSS.</p> <h4 class=\"textBox\">Usos de la comunidad:</h4> <p>La    invitación y el uso de las diferentes comunidades gestionadas por nuestra red social no pueden usarse para fines comerciales, salvo que RIAM I+L Lab lo    permita de forma expresa.</p> <h4 class=\"textBox\">Actos no permitidos: </h4> <ul class=\"generico\"> <li>Difamar, acosar, intimidar o amenazar a otros usuarios    de la red de Gnoss o a terceros ajenos a ella.</li> <li>Utilizar datos personales e íntimos de otras personas con fines diferentes a los que el entorno de la    comunidad a la que perteneces determine.</li> <li>Usar la red como herramienta de intercambio de materiales o informaciones que sean obscenos, ilícitos,    calumniosos, inmorales, inapropiados desde un punto de vista racial, sexual, ético o cualquier otro.</li> <li>Intercambiar materiales que conforme a la ley    de propiedad intelectual pertenezcan a un tercero sin su consentimiento. Del mismo modo, no podrás retirar los avisos legales sobre derechos de autor, marcas    registradas o de otros derechos afines.</li> <li>Usar GNOSS como difusor de virus, envío de spam, programas maliciosos, así como archivos y/o programas que    interrumpan, dañen o limiten la operatividad de la propia red o de cualquier software, hardware o equipo de telecomunicaciones. Del mismo modo, no realizarás    ninguna acción que pueda dañar, sobrecargar o deteriorar la red GNOSS.</li> <li>Crear una cuenta con identidad falsa o mediante la usurpación de identidad de    otra persona o entidad.</li> </ul> <p>Por último, queremos recordarte que RIAM I+L Lab no se responsabiliza de las opiniones, declaraciones, manifestaciones    y comentarios que realices desde las diferentes herramientas que en esta red social ponemos a tu disposición, siendo directamente responsable el titular de    las claves de acceso a la red.</p>    </span>",
                WikiDisponible = false,
                BaseRecursosDisponible = true,
                CompartirRecursosPermitido = true,
                InvitacionesDisponibles = true,
                ServicioSuscripcionDisp = false,
                BlogsDisponibles = false,
                ForosDisponibles = false,
                EncuestasDisponibles = false,
                VotacionesDisponibles = true,
                ComentariosDisponibles = true,
                PreguntasDisponibles = false,
                DebatesDisponibles = false,
                BrightcoveDisponible = false,
                BrightcoveTokenWrite = null,
                BrightcoveTokenRead = null,
                BrightcoveReproductorID = null,
                LogoProyecto = null,
                MensajeBienvenida = "",
                EntidadRevisadaObligatoria = false,
                UmbralDetPropietariosProc = 0,
                UmbralDetPropietariosObj = 0,
                UmbralDetPropietariosGF = 0,
                NombreDebilidadDafoProc = "Debilidad",
                NombreAmenazaDafoProc = "Amenaza",
                NombreFortalezaDafoProc = "Fortaleza",
                NombreOportunidadDafoProc = "Oportunidad",

                NombreDebilidadDafoObj = "Debilidad",
                NombreAmenazaDafoObj = "Amenaza",
                NombreFortalezaDafoObj = "Fortaleza",
                NombreOportunidadDafoObj = "Oportunidad",

                NombreDebilidadDafoGF = "Debilidad",
                NombreAmenazaDafoGF = "Amenaza",
                NombreFortalezaDafoGF = "Fortaleza",
                NombreOportunidadDafoGF = "Oportunidad",

                ImagenHome = null,
                NombreImagenPeque = "peque",
                ImagenPersonalizadaPeque = null,
                PermitirRevisionManualPro = false,
                PermitirRevisionManualGF = false,
                PermitirRevisionManualObj = false,
                PermitirRevisionManualComp = false,
                RutaTema = null,
                RutaImagenesTema = null,
                PermitirCertificacionRec = false,
                PoliticaCertificacion = "En estos momentos estamos elaborando la política de certificación de los recursos de la comunidad. Si tienes alguna duda sobre los niveles de certificación que hemos establecido te sugerimos que te pongas en contacto con nosotros.",
                CoordenadasHome = null,
                ImagenHomeGrande = null,
                DafoDisponible = false,
                PlantillaDisponible = false,
                CodigoGoogleAnalytics = null,
                VerVotaciones = false,
                VersionFotoImagenHomeGrande = null,
                VersionFotoImagenFondo = null,
                ClausulasRegistro = true,
                LicenciaPorDefecto = null,
                MensajeLicenciaPorDefecto = null,

                BrightcoveFTP = null,
                BrightcoveFTPUser = null,
                BrightcoveFTPPass = null,
                BrightcovePublisherID = null,

                VersionCSS = null,
                VersionJS = null,
                OcultarPersonalizacion = false,
                PestanyasDocSemanticos = "",
                PestanyaRecursosVisible = true,
                ScriptBusqueda = null,
                ImagenRelacionadosMini = false,
                CoordenadasMosaico = "",
                CoordenadasSup = null,
                VersionFotoImagenMosaicoGrande = null,
                VersionFotoImagenSupGrande = null,
                EsBeta = true,
                ScriptGoogleAnalytics = "",
                NumeroRecursosRelacionados = 5,
                GadgetsPieDisponibles = false,
                GadgetsCabeceraDisponibles = false,
                BiosCortas = false,
                RssDisponibles = false,
                RdfDisponibles = false,
                RegDidactalia = false,
                HomeVisible = true,
                CargasMasivasDisponibles = false,
                ComunidadGNOSS = true,
                IdiomasDisponibles = false,
                IdiomaDefecto = null,
                SupervisoresAdminGrupos = false,
                FechaNacimientoObligatoria = true,
                PrivacidadObligatoria = true,
                EventosDisponibles = false,
                SolicitarCoockieLogin = true,
                Copyright = null,
                CMSDisponible = false,
                VersionCSSWidget = null,
                InvitacionesPorContactoDisponibles = true,
                PermitirUsuNoLoginDescargDoc = false,
                TipoCabecera = null,
                TipoFichaRecurso = null,
                AvisoCookie = true,
                MostrarPersonasEnCatalogo = false,
                EnvioMensajesPermitido = true,
                EnlaceContactoPiePagina = null,
                TieneSitemapComunidad = false,
                MostrarAccionesEnListados = false,
                PermitirVotacionesNegativas = true
            };
            mEntityContext.ParametroGeneral.Add(parametroGeneral);


            Tesauro tesauro = new Tesauro()
            {
                TesauroID = new Guid("4E2BF9A0-04AD-49FB-BC34-E4E9964BEE28")
            };
            mEntityContext.Tesauro.Add(tesauro);

            TesauroProyecto TesauroProyecto = new TesauroProyecto()
            {
                TesauroID = new Guid("4E2BF9A0-04AD-49FB-BC34-E4E9964BEE28"),
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                IdiomaDefecto = null
            };
            mEntityContext.TesauroProyecto.Add(TesauroProyecto);

            CategoriaTesauro categoriaTesauro = new CategoriaTesauro()
            {
                TesauroID = new Guid("4E2BF9A0-04AD-49FB-BC34-E4E9964BEE28"),
                CategoriaTesauroID = new Guid("910889F3-B093-4247-9C94-10BEA646F5CF"),
                Nombre = "Ejemplo",
                Orden = 0,
                NumeroRecursos = 0,
                NumeroPreguntas = 0,
                NumeroDebates = 0,
                NumeroDafos = 0,
                TieneFoto = false,
                VersionFoto = 0
            };
            mEntityContext.CategoriaTesauro.Add(categoriaTesauro);

            AD.EntityModel.Models.UsuarioDS.Usuario usuario = new AD.EntityModel.Models.UsuarioDS.Usuario()
            {
                UsuarioID = new Guid("3A9436B5-3C62-4D5D-8FF2-74341758A6F2"),
                Login = "admin",
                Password = "LwVRBcblcJRrs/ySjQKa6dEvHHxe3u4jaIMCKSxIcvyc6Aob",
                EstaBloqueado = false,
                NombreCorto = "admin",
                Version = 1,
                Validado = 1
            };
            mEntityContext.Usuario.Add(usuario);

            Persona persona = new Persona()
            {
                PersonaID = new Guid("BD240B42-EDA5-46F3-99B2-87034DA9BE69"),
                UsuarioID = new Guid("3A9436B5-3C62-4D5D-8FF2-74341758A6F2"),
                Nombre = "Admin",
                Apellidos = "Admin",
                Sexo = "M",
                FechaNacimiento = new DateTime(1978, 05, 31),
                PaisPersonalID = new Guid("98D604B4-3141-4499-BDE1-C320F09EF45C"),
                ProvinciaPersonalID = new Guid("FFB9E813-EBC0-4623-A9F6-92595B3D71CF"),
                ProvinciaPersonal = "La Rioja",
                LocalidadPersonal = "Logroño",
                CPPersonal = "26000",
                Email = "admin@admin.es",
                EsBuscable = true,
                EsBuscableExternos = false,
                Eliminado = false,
                Idioma = "es",
                EstadoCorreccion = 0
            };
            mEntityContext.Persona.Add(persona);

            Perfil perfil = new Perfil()
            {
                PerfilID = new Guid("3110FC3C-25EA-49E4-9190-41D1DDBDBAB3"),
                NombrePerfil = "Admin",
                Eliminado = false,
                NombreCortoUsu = "Admin",
                PersonaID = new Guid("BD240B42-EDA5-46F3-99B2-87034DA9BE69"),
                TieneTwitter = false,
                CaducidadResSusc = 7
            };
            mEntityContext.Perfil.Add(perfil);

            PerfilPersona perfilPersona = new PerfilPersona()
            {
                PerfilID = new Guid("3110FC3C-25EA-49E4-9190-41D1DDBDBAB3"),
                PersonaID = new Guid("BD240B42-EDA5-46F3-99B2-87034DA9BE69"),
            };
            mEntityContext.PerfilPersona.Add(perfilPersona);

            Identidad identidad = new Identidad()
            {
                IdentidadID = new Guid("3D9EAA8A-AF02-469C-836B-FDB2722C82DF"),
                PerfilID = new Guid("3110FC3C-25EA-49E4-9190-41D1DDBDBAB3"),
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                FechaAlta = new DateTime(),
                NumConnexiones = 0,
                Tipo = 0,
                NombreCortoIdentidad = "Admin",
                RecibirNewsLetter = true,
                Rank = 0,
                MostrarBienvenida = false,
                DiasUltActualizacion = 0,
                ValorAbsoluto = 0,
                ActivoEnComunidad = true,
                ActualizaHome = true

            };
            mEntityContext.Identidad.Add(identidad);

            GeneralRolUsuario GeneralRolUsuario = new GeneralRolUsuario()
            {
                UsuarioID = new Guid("3A9436B5-3C62-4D5D-8FF2-74341758A6F2"),
                RolPermitido = "FFFFFFFFFFFFFFFF",
                RolDenegado = "0000000000000000"
            };
            mEntityContext.GeneralRolUsuario.Add(GeneralRolUsuario);

            ProyectoRolUsuario ProyectoRolUsuario = new ProyectoRolUsuario()
            {
                OrganizacionGnossID = new Guid("11111111-1111-1111-1111-111111111111"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                UsuarioID = new Guid("3A9436B5-3C62-4D5D-8FF2-74341758A6F2"),
                RolPermitido = "FFFFFFFFFFFFFFFF",
                RolDenegado = "0000000000000000",
                EstaBloqueado = false
            };
            mEntityContext.ProyectoRolUsuario.Add(ProyectoRolUsuario);

            ProyectoUsuarioIdentidad proyectoUsuarioIdentidad = new ProyectoUsuarioIdentidad()
            {
                IdentidadID = new Guid("3D9EAA8A-AF02-469C-836B-FDB2722C82DF"),
                UsuarioID = new Guid("3A9436B5-3C62-4D5D-8FF2-74341758A6F2"),
                OrganizacionGnossID = new Guid("11111111-1111-1111-1111-111111111111"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                FechaEntrada = new DateTime(),
                Reputacion = 0
            };
            mEntityContext.ProyectoUsuarioIdentidad.Add(proyectoUsuarioIdentidad);

            BaseRecursos baseRecursos = new BaseRecursos()
            {
                BaseRecursosID = new Guid("88079B19-5CC2-47D7-8275-8FA22D5165D9")
            };
            mEntityContext.BaseRecursos.Add(baseRecursos);

            BaseRecursosUsuario baseRecursosUsuario = new BaseRecursosUsuario()
            {
                BaseRecursosID = new Guid("88079B19-5CC2-47D7-8275-8FA22D5165D9"),
                UsuarioID = new Guid("3A9436B5-3C62-4D5D-8FF2-74341758A6F2"),
                EspacioMaxMyGnossMB = 1024,
                EspacioActualMyGnossMB = 0
            };
            mEntityContext.BaseRecursosUsuario.Add(baseRecursosUsuario);

            Tesauro tesauro2 = new Tesauro()
            {
                TesauroID = new Guid("06977815-4685-455E-92FF-D3E69BF663DB")
            };
            mEntityContext.Tesauro.Add(tesauro2);

            CategoriaTesauro categoriaTesauro2 = new CategoriaTesauro()
            {
                TesauroID = new Guid("06977815-4685-455E-92FF-D3E69BF663DB"),
                CategoriaTesauroID = new Guid("D8FEC1D0-D75F-4013-81F2-F4FF77296F52"),
                Nombre = "Recursos públicos",
                Orden = 0,
                NumeroRecursos = 0,
                NumeroPreguntas = 0,
                NumeroDebates = 0,
                NumeroDafos = 0,
                TieneFoto = false,
                VersionFoto = 0
            };
            mEntityContext.CategoriaTesauro.Add(categoriaTesauro2);

            TesauroUsuario tesauroUsuario = new TesauroUsuario()
            {
                TesauroID = new Guid("06977815-4685-455E-92FF-D3E69BF663DB"),
                UsuarioID = new Guid("3A9436B5-3C62-4D5D-8FF2-74341758A6F2"),
                CategoriaTesauroPublicoID = new Guid("D8FEC1D0-D75F-4013-81F2-F4FF77296F52"),
                CategoriaTesauroPrivadoID = new Guid("281BE370-FEA4-4D3A-ABBE-3A965968265F"),
                CategoriaTesauroMisImagenesID = new Guid("BD50A182-4D56-4793-96F9-A70A8BFE62A7"),
                CategoriaTesauroMisVideosID = new Guid("71C1D68C-8ACA-41FA-9868-1FFE542CCC68")
            };
            mEntityContext.TesauroUsuario.Add(tesauroUsuario);

            List<Guid> listaIds = mEntityContext.ConfiguracionGnossPersona.Select(confg => confg.PersonaID).ToList();
            List<Guid> personID = mEntityContext.Persona.Where(per => !listaIds.Contains(per.PersonaID)).Select(per => per.PersonaID).ToList();
            personID.AddRange(mEntityContext.Persona.Local.Where(per => !listaIds.Contains(per.PersonaID)).Select(per => per.PersonaID).ToList());
            personID = personID.Distinct().ToList();
            foreach (Guid Id in personID)
            {
                ConfiguracionGnossPersona configuracionGnossPersona = new ConfiguracionGnossPersona()
                {
                    PersonaID = Id,
                    SolicitudesContacto = true,
                    MensajesGnoss = true,
                    ComentariosRecursos = true,
                    InvitacionComunidad = true,
                    InvitacionOrganizacion = true,
                    BoletinSuscripcion = 1,
                    VerAmigos = true,
                    VerAmigosExterno = false,
                    VerRecursos = true,
                    VerRecursosExterno = false,
                    EnviarEnlaces = true
                };
                mEntityContext.ConfiguracionGnossPersona.Add(configuracionGnossPersona);                             
            }
            AdministradorProyecto administradorProyecto = new AdministradorProyecto()
            {
                OrganizacionID = new Guid("11111111-1111-1111-1111-111111111111"),
                ProyectoID = new Guid("11111111-1111-1111-1111-111111111111"),
                UsuarioID = new Guid("3A9436B5-3C62-4D5D-8FF2-74341758A6F2"),
                Tipo = 0
            };
            mEntityContext.AdministradorProyecto.Add(administradorProyecto);

            UtilProcessInsertData utilProcessInsertData = new UtilProcessInsertData();
            List<FacetaObjetoConocimiento> listaFaceta = utilProcessInsertData.ProcesarInsert<FacetaObjetoConocimiento>(Resources.carga_bd_acid_facetaobjetoconocimiento);
            mEntityContext.FacetaObjetoConocimiento.AddRange(listaFaceta);
            List<Pais> listaPais = utilProcessInsertData.ProcesarInsert<Pais>(Resources.carga_bd_acid_pais);
            mEntityContext.Pais.AddRange(listaPais);
            List<Provincia> listaProvincia = utilProcessInsertData.ProcesarInsert<Provincia>(Resources.carga_bd_acid_provincia);
            mEntityContext.Provincia.AddRange(listaProvincia);
            mEntityContext.SaveChanges();
        }
        public bool IsDataLoaded()
        {
            return mEntityContext.Identidad.Count() > 0;
        }

        public bool InsertDataIfPossible()
        {
            if (!IsDataLoaded())
            {
                InsertData();
                InsertOauth();
                return true;
            }
            return false;
        }

        private void InsertOauth()
        {
            Guid usuarioID = mEntityContext.Usuario.First().UsuarioID;
            string login = mEntityContext.Usuario.First().Login;
            Guid proyectoID = mEntityContext.Proyecto.First().ProyectoID;

            OAuthConsumer filaOAuthConsumer = new OAuthConsumer();
            filaOAuthConsumer.ConsumerKey = GenerarTokens();
            filaOAuthConsumer.ConsumerSecret = GenerarTokens();
            filaOAuthConsumer.VerificationCodeFormat = 1;
            filaOAuthConsumer.VerificationCodeLength = 1;

            mEntityContextOauth.OAuthConsumer.Add(filaOAuthConsumer);

            mEntityContextOauth.SaveChanges();

            ConsumerData filaConsumer = new ConsumerData();
            filaConsumer.ConsumerId = filaOAuthConsumer.ConsumerId;
            filaConsumer.Nombre = "Automatico";
            filaConsumer.UrlOrigen = $"http://api.net";
            filaConsumer.FechaAlta = DateTime.Now;

            mEntityContextOauth.ConsumerData.Add(filaConsumer);

            mEntityContextOauth.SaveChanges();

            if (!mEntityContextOauth.Usuario.Any(item => item.UsuarioID.Equals(usuarioID)))
            {
                OAuthAD.OAuth.Usuario filaUsuario = new OAuthAD.OAuth.Usuario();
                filaUsuario.UsuarioID = usuarioID;
                filaUsuario.Login = login;

                mEntityContextOauth.Usuario.Add(filaUsuario);
                mEntityContextOauth.SaveChanges();
            }

            UsuarioConsumer filaUsuarioConsumer = new UsuarioConsumer();
            filaUsuarioConsumer.UsuarioID = usuarioID;
            filaUsuarioConsumer.ConsumerId = filaOAuthConsumer.ConsumerId;
            filaUsuarioConsumer.ProyectoID = proyectoID;

            mEntityContextOauth.UsuarioConsumer.Add(filaUsuarioConsumer);

            mEntityContextOauth.SaveChanges();

            OAuthToken filaToken = new OAuthToken();
            filaToken.Token = GenerarTokens();
            filaToken.TokenSecret = GenerarTokens();
            filaToken.State = 2;
            filaToken.IssueDate = DateTime.Now;
            filaToken.ConsumerId = filaOAuthConsumer.ConsumerId;
            filaToken.UsuarioID = usuarioID;
            filaToken.Scope = null;
            filaToken.RequestTokenVerifier = null;
            filaToken.ConsumerVersion = "1.0.1";

            mEntityContextOauth.OAuthToken.Add(filaToken);

            mEntityContextOauth.SaveChanges();
        }

        /// <summary>
        /// Genera un nuevo token
        /// </summary>
        /// <returns>String con el token</returns>
        private string GenerarTokens()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String(time.Concat(key).ToArray());

            return token;
        }
    }
}
