using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.Facetado;

namespace Es.Riam.Gnoss.AD.Test
{
    public class FacetadoADTest
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Este test comprueba objetos con tipo siendo este válido
        /// </summary>
        [Test]
        public void ObjetoTieneTipo_ConTipo()
        {
            string test1 = "\"abc?1221341+-/\\prueba#'.\"^^<http://gnoss.com>";
            string test2 = "\"123\"^^<https://gnoss.com>";
            string test3 = "\"123\"^^xsd:int";
            string test4 = "\"123^^fafe\"^^xsd:string";
            string test5 = "\"123^^fafe\"^^xsd:str_-ing";

            bool expected = true;
            Assert.That(FacetadoAD.ObjetoTieneTipo(test1), Is.EqualTo(expected));
            Assert.That(FacetadoAD.ObjetoTieneTipo(test2), Is.EqualTo(expected));
            Assert.That(FacetadoAD.ObjetoTieneTipo(test3), Is.EqualTo(expected));
            Assert.That(FacetadoAD.ObjetoTieneTipo(test4), Is.EqualTo(expected));
            Assert.That(FacetadoAD.ObjetoTieneTipo(test5), Is.EqualTo(expected));
        }

        /// <summary>
        /// Este test comprueba que la expresión regular devuelve false cuando tiene tipo pero no es válido
        /// </summary>
        [Test]
        public void ObjetoTieneTipo_TipoErroneo()
        {
            string test1 = "123^^<http://gnoss.com>";
            string test2 = "a\"123\"^^<http://gnoss.com>";
            string test3 = "\"123\"^^<https//gnoss.com>";
            string test4 = "\"123\"^^<httpss://gnoss.com>";

            bool expected = false;
            Assert.That(FacetadoAD.ObjetoTieneTipo(test1), Is.EqualTo(expected));
            Assert.That(FacetadoAD.ObjetoTieneTipo(test2), Is.EqualTo(expected));
            Assert.That(FacetadoAD.ObjetoTieneTipo(test3), Is.EqualTo(expected));
            Assert.That(FacetadoAD.ObjetoTieneTipo(test4), Is.EqualTo(expected));
        }

        /// <summary>
        /// Este test comprueba que la expresión regular devuelve false cuando se le pasa un objeto sin tipo
        /// </summary>
        [Test]
        public void ObjetoTieneTipo_SinTipo()
        {
            string test1 = "\"123\"";  
            string test2 = "\"1^^23\"";
            string test3 = "1234";
            string test4 = "\"1234\"^^xsd:stri g";
            string test5 = "\"EL Correo de Galicia ápcía áe Napelos V . VAPORB.-5 DE LA COMPAÑIA del Llovd Norte Alemán Sííics X-npeñal Alemana V&P0R33 GOaRBO'3 AL RIO D3 LA PLATA DOS EXPSDÍOIONS-S VíSNSffALEB Sarvioio quiaesiíal. \" De VISO diraotiaiJata para MoaterMeo y Baottoí .Airea saldrá el 10 de Octubre el rapor correo alemán STTTI TGkAIT El 24 de Octubre ?aldrá pora los mismos puntos el vapor correo alemán \"W S IIvI -A. Sí. Admite carga j p&sajercs. Twnbiéa admiten pasaderos para Ncb^A Yohk «son trasborde en Bbejíkíí. Satos Taporea no Lacón cuarentena ni en Mosteyideo ni en Buenos Aires. , Loa vpporea de esta Compañía llsvan canareros y cocinoros espaüoles para atender á los pasajeros de tercera clase, los que tienen muy buen servicio de m ^sa, mag^iificaí I NORDEUCHESTER BEL LLOYD   Admiten estos vapores correspondencia y pasajeros de 1.» 2.a j 3a ^ i Los vapores de esta Compañía hacen los i Cuentan estos magníficos vaDnrM S viajes directamente de Villagareia para la grandes comodidades para los sein!8 . M , 0 PLATA, sm tocar en Portugal ni Brasü, sajeroa de 1» j 2.° clase con ¿S?, I Se facilitan pasajes para Montevideo, , m<:.ñám(¡isJ¡ últimas casi siempre cámaras. Como también para loa S I 5™°09T,'4Í?»' Sí0 Jansiro> ^P^0 SnjótDS i cuarentena en Montevideo y niendo dos comidaa semínalaa i t ' i 5 Chile, Bohvia, Paraguay, Puerto Ewo, * ~J 1 -.3 mBB LljUIIIEL;iniLEITjUIZA£Z 05«; Galle Patio da Mm nm. 9. Santiago. Habana, Veracnu, Méjico y demáa pun^ tos de todas las repúblicas en todos loa vapores correos, . Acompaña á loa pasajeros hasta dejarlos! embarcados. [Ojo! No dejarse engañar por otros agen- jj tes que por cobrar más barato el pasaje, • embarcan á los pasajeros en vaprres de   ! carga q.ue emplean en ^1 viaie á Monte- [ 2 video y Buenos Aires 28 y SÓ días, cuan- ? , do todos saben qus los buenos vaporea só5 lo emplean ÍS días i dichos puntos, y pa- } i ra la Habana también los embarcan en va- ] pores qua emplean 18 & 20 días, cuando el ) ; Buenos Aires. i g» 7 servidos por coeinaroa t c»™.^   í también gaUegoa. 7 ^   El 10 de Octubre saldrá de Villagarcía directamente para Montevideo y ' ; Aires el vapor correo alemán Í STUTTGAJT : El 24 de Octubre saldrá para los mismos puntos el vapor correo alemán i WEIMAR ? Para mas informes diriffirse- al coTisisruatano en Villagarcía D. LDI8 SABOU 1 BOBJEDO Snbagente en Santiago: D. ManüeIi Méndez, Altamira núm. 2. * literas, la comida i la esp&Sola, todof los días variada y siempre con vino. Asistencia í; viaje á la Habana sólo es da 9 á 10 días, médica grafás._   Se facilitan- pasajes al fiado mediante Para más informes, dirigirsí al Agt-nt» da la CompaSla sn Vijo, D. AoeusTO BÍK í garantía, CENA Fkanoo, calta Beal niuaaro 8, baio. Sabagentes en S&atia«o: DO^T MANUEL MOHALKS YQÓNZAL3SZ. Oñcinaa Pa. tío de Madras, 9 y DON RAMuE FERNANDEZ V^SLA,, Pajera.'SS. Heprssentación de la Asociación raátua ^ de rademaones á metálico.» ? Para obtener más informes dirigirse al a agente D. MANUEL MOEALES Y GONf ZALEZ: oficinas, patio de Madrea aúinei ro 9, ¡ SANTIAGO Yiud arreras. m m msimkikms vi G O -S:1 i,a fe fagadii IrasatMk Para Rio Janeiro, Santos, Montevideo y Buenos Aires saldrá .de Vigo directamente el 5 de Octubre el vapor JOSÉ G-&XíIíARV LINEA LAMPORT & HOLT Para Montevideo y Buenos Aires saldrá de Vigo directamente el 8 de Octubre el magnífico trasatlántico 'nglés R O M SJ IT Viaja en 18 días. - Admitirá pasajeros de 1.a y 3.a clase. sá5 ^5. ¿1 — — .i: « ^ '\" \"'\"3 c « Si— „00\"-\" ?.« I Vffis&S COMPAÑIA BOOTH (Royal Maü) Para Pará y Manaos saldrá de este puerto el 13 de Octubre el magnífico vapor co- ? rreo inglés \\ de porte de 7.000 toneladas y marcha de 16 millas. \\ Superior acomodación para pasajeros do l.& y 3.a clase. t Duración del viaje desde la salida de Lisboa á Pará, 10 días.— Admitiiá pasajeros: á Paba eífobmhs en Santiago: D. RAMON FSRNANDEZ VARELA, Rúa. dkl 4 VILLA.B, 85, (Pajbsa). I >\" • \" e c ~ \\¡ s «= f» / < O \" - fz U — Va íáe SMS í Viajes rápidos en quince días l Hérs'eog i3ejree&a<.-«>3sBtlago. SERVICIO DE LA 'AMIA TRA1 LINEA DE CUBA Y MEJICO 11 día 17 del corriente saldri de Bilbao, al 20 da Santander y 2 1 de la CorJ el vapor 3 REINA MARÍA CRISTINA i. directamente pafia la Habana y Veracnu. A.iinite pasaje y carga para Ccst&firaJ !Pacífico con trasbordo en la Habana al vapor de la linea de Veneraela-ColoaiT   Combinaciones para el litoral de Cuba é Isla de Sasto Domingo, j LINEA DE NUEVA- YORK, CUBA Y MÉJICO ; El día 26 dal corriesfce saldrá de Barcelona, el 3G de Cádis, el vapor I iBUENOS AIRES ¡ directamnte para New- York, Habana y Veraoruz. Combinaciones para distinto» di f; tos de loa Estados Unidos, litoral de Cuba é Isla da Santo Domingo. B   LINEA DE VENEZUELA-COLOMBIA jj Si día li dol corriente saldrá de Barcelona, el 18 do Málaga y 15 de C4dii q! vaB'1 f MONSERRAT ; directamente para tas Palmas, Santa Cruz de Tenerife, Santa Cruí de la Pal Puerto Rico, Habana, Puerto Limón, Colén, Sabanilla; Curagao Puerto Cabello j Guayara, admitiendo o.asaje y carga para Veraoruz con trasbordo en Ealaans. Co: >. naspor el ferrocarril de Panamá con las Compañías de navegación dal Pacífica partí i yos puertos admite pasaje y carga con billetes y conocimientos directos. Cosabm»(   para el litoral de Caba y Puarto Rico. Se admite pasaje para Puerto Plata, con ti i bordo en Puerto Rico, y para Santo Domingo y San Pedro de Maooris, eon trasbo] i en Eabana. También carga para Maracaibo, Campano y Trinidad coa traabordi j¿ Carvpano j LINEA. DS HLIPINAS % El día 18 del corrionta saldri de Liverpool y el 18 da la CoraSa, «1 vnpoí 1 ALICANTE f directamentopara Vigo, Lisboa, Cádiz, Cartagena, Valencia y B*rcelona, de donde drá el 30 del cornonte diractameute para Part Said, Sa«t, Colombo, Singapore y   ^a, sirviendo por trasbordo los puertos de la costa oriaatal Africa, is la Indi», ] vs, Sumatra, China, Japón y Australia. Línea de Ssísbiss Airas , 2 dal oomeats saldrá de Barcelona, el 3 de Valencia, el 5 da Milag* J   de Cidis el magnífico vapor • 1 Pn da SaífU9(aga/ directam g te para Santa Crus de Tenarif», Montevideo y Bueaoa Airea. Los pasajeros de Galicia embarcarán la Comaa «1 31 en na vapor da la Corspij á trasbordar an Cádís. 1 LINEA DE CANARIAS oo F ^ía^7 d,eI corrieilte Barcelona, ol 18 de Vabaaia, «1 19 d« AliesaiJ 32 de Cidis, el vapor \"   J . MAISTUEL L. VILLA VERDE ; diroctamente para Casablanca. Maaagin, Las Palmas, Santa Gnu do 1» Palm» v S» • Oros ae Tenerife, regresando 4 Barcelona por Cádis, Alicante y Valonci*.   LINEA DS FERNANDO POO i El día 24 M corriente aaldrá de Barcelona y el 30 da Ciáia, «1 rapor I 3AN FRANCISCO j para Fernanda Póo, con escala en- Casablaaoa, Masagin y otros naortos do 1» 04 < occidented de Africa y Golfo de Guinea. i En este Establecimiento se con¡ feccionan toda clase de obras, re! vistas, periódicos, estados, reci- El dla2id) Or.ubr) saldrá do Vi¡U»arcía pira 1a H^baua, Sagaa la Grande, San- bos para la cobranza del iüBpuestO LINEA DE TAN&5X Salidas de Cidii: Lunes, Miércolos y Viernes, Salidas da iÁngw: Martas, 3mro<i r SAba^aa tiago de Cuba y Cienfuegos, el magnífico vapor El día2G de Ojtabrj sal Irá do Villa íar úa pra Poma ab aco 3*hb, Rio Janeiro, Sontos, Montevideo y Buenos Airas, el vapor correo Soto v&por admite pasajeros de l^1 y S.\" clase. LAMALA REAL lííGL'SSA es la línea de vapores que hace los viajes más ripipidos entre Europa y Araerica del Sur, otrecic-ndo excelentes «modada -íes y trato i los pasajeros de primera y segunda olese. A loa de tercera se lea da cama con ropa, pan nreeoo y vino á todas las oozsid&M, Llevará cocineros y cam.ar8ro3 espa&oles para mejor a»rrisio de los minaos y asís- ; inscripción de fincas rústicas y teaoia médica gratuita. > [ urbanas. Los pasajeros tienea que proR'jntarsrj «on ana pasaporus en la Agoscia de Vulagar- el dia antes de la salida del vapor, de consumos y del déficit, factul ras, mecabretes, recordatorios, es. quelas y tarjetas fúnebres y todos ¡ Ij cuantos trabajos estén incluidos jj en el arte tipográfico. — Esta Casa ; tiene á la venta kójas de alta y ¿ baja en la contribución industrial, '. asi como las necesarias para la TARJETAS de visita de todas Fábrica de Fundición   DE JUAN LISTE PEREIBA PaBBOQUIA DE OSAZO KN LA. ÜLIiA, Atottamis de Estilada, en la. pbotoíoia db Pontbtbi'^I obi Se suplica á los pasajeros que doaeon smbarcarse on ohie vapor, a-^isen ca&nto antes, clases y tatGañOS. p»pa reservarlo la cabida, por ser grand» «1 nímeTO da -¿miM claaes solicitado. - . Paramifi informoa dirigiraa á los .¿stiiivs i* !a CompaSis-aa Vülasarcí» ««ftorea ' üsmwn&i P^OÍdOSg •C*BI»* s Esta casa establecida en 1884, es una espeoiali* se ha acreditado ,por el buen resultado de sus , , . pudiendo competir con las principales de Esp1 solo en los precios, sino en la perfección de sus trabajos. Se presenta en las parroq para realizar los contratos, siendo conveniente, y se encarga de arrastres y coloca' nes bajo su rejponsabdidad, garantizando sus obras por el tiempo que se coaTeflg»' Recibe las viejas á cuenta y cobra tambióa á plazos. Los laíonnes y pedido* se harán directamente al fabricante Gran almacén de ultramarinos finos.— Treguntoiro, 36, Santiago. Él que economiza es rico j el que quiera serlo que compre en el ALMACEN de y tendrá géneros superiores y precios sin competencia. rdefi!r™í \"-í ^\"^^s: toszsszzñsssi \"™:°^r 100 * 10 selecta * ««. *» ^ i^E¿¿¿M¿^ variedad en vincs' licores ' aarapag'lc de ,as ^ acre<litadas F F 181 1 a int: lio tic i ir ps bes nos fcial it \\,: m im iMei lien Idue Ib* juna Ipist Icai leu; eid lal Ipai ftiti im mío fm fiíru í}k; i hi F.n fKeu lest 5 sus falta ¡igila [asar Win pe ei .ec ftái mu pal final lervir lUpcia fiin s W\\ l\\ CO] Ifcrre I?* ate Ilutad K , . %\"";

            bool expected = false;
            Assert.That(FacetadoAD.ObjetoTieneTipo(test1), Is.EqualTo(expected));
            Assert.That(FacetadoAD.ObjetoTieneTipo(test2), Is.EqualTo(expected));
            Assert.That(FacetadoAD.ObjetoTieneTipo(test3), Is.EqualTo(expected));
            Assert.That(FacetadoAD.ObjetoTieneTipo(test4), Is.EqualTo(expected));
            Assert.That(FacetadoAD.ObjetoTieneTipo(test5), Is.EqualTo(expected));
        }

        [Test]
        public void ComprobarSujetoValido()
        {
            string test1 = "<http://museodelprado.es/items/EmbeddedHTML_f8aa4d7c-0b81-3656-31dc-26b128a00292_f0d6a2e3-c5d6-444c-bcb8-e3160c51a781>";
            string test2 = "<https://museodelprado.es/items/EmbeddedHTML_f8aa4d7c-0b81-3656-31dc-26b128a00292_f0d6a2e3-c5d6-444c-bcb8-e3160c51a781>";
            string test3 = "<http://museodelprado.es/items/ProjectPage_f8aa4d7c-0b81-3656-31dc-26b128a00292_2bd36e19-9165-4ae8-840f-be030cf70425>";
            bool expected = true;
            Assert.That(FacetadoAD.SujetoValido(test1), Is.EqualTo(expected));
            Assert.That(FacetadoAD.SujetoValido(test2), Is.EqualTo(expected));
            Assert.That(FacetadoAD.SujetoValido(test3), Is.EqualTo(expected));
        }

        [Test]
        public void ComprobarSujetoInValido()
        {
            string test1 = "<htt://museodelprado.es/items/EmbeddedHTML_f8aa4d7c-0b81-3656-31dc-26b128a00292_f0d6a2e3-c5d6-444c-bcb8-e3160c51a781>";
            string test2 = "<https://museodelprado.es/items/EmbeddedHTML_f8aa4d7-0b81-3656-31dc-26b128a00292_f0d6a2e3-c5d6-444c-bcb8-e3160c51a781>";
            string test3 = "<httpp://museodelprado.es/items/ProjectPage_f8aa4d7c-0b81-3656-31dc-26b128a00292_2bd36e19-9165-4ae8-840f-be030cf70425>";
            string test4 = "body.CollectionPageModel { padding: 32px 0; }";

            bool expected = false;
            Assert.That(FacetadoAD.SujetoValido(test1), Is.EqualTo(expected));
            Assert.That(FacetadoAD.SujetoValido(test2), Is.EqualTo(expected));
            Assert.That(FacetadoAD.SujetoValido(test3), Is.EqualTo(expected));
            Assert.That(FacetadoAD.SujetoValido(test4), Is.EqualTo(expected));

        }
    }
}