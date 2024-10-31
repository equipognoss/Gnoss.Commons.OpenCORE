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
        /// Este test comprueba objetos con tipo siendo este v�lido
        /// </summary>
        [Test]
        public void ObjetoTieneTipo_ConTipo()
        {
            string test1 = "\"abc?1221341+-/\\prueba#'.\"^^<http://gnoss.com>";
            string test2 = "\"123\"^^<https://gnoss.com>";
            string test3 = "\"123\"^^xsd:int";
            string test4 = "\"123^^fafe\"^^xsd:string";
            string test5 = "\"123^^fafe\"^^xsd:str_-ing";

            Assert.IsTrue(FacetadoAD.ObjetoTieneTipo(test1));
            Assert.IsTrue(FacetadoAD.ObjetoTieneTipo(test2));
            Assert.IsTrue(FacetadoAD.ObjetoTieneTipo(test3));
            Assert.IsTrue(FacetadoAD.ObjetoTieneTipo(test4));
            Assert.IsTrue(FacetadoAD.ObjetoTieneTipo(test5));
        }

        /// <summary>
        /// Este test comprueba que la expresi�n regular devuelve false cuando tiene tipo pero no es v�lido
        /// </summary>
        [Test]
        public void ObjetoTieneTipo_TipoErroneo()
        {
            string test1 = "123^^<http://gnoss.com>";
            string test2 = "a\"123\"^^<http://gnoss.com>";
            string test3 = "\"123\"^^<https//gnoss.com>";
            string test4 = "\"123\"^^<httpss://gnoss.com>";

            Assert.IsFalse(FacetadoAD.ObjetoTieneTipo(test1));
            Assert.IsFalse(FacetadoAD.ObjetoTieneTipo(test2));
            Assert.IsFalse(FacetadoAD.ObjetoTieneTipo(test3));
            Assert.IsFalse(FacetadoAD.ObjetoTieneTipo(test4));
        }
        
        /// <summary>
        /// Este test comprueba que la expresi�n regular devuelve false cuando se le pasa un objeto sin tipo
        /// </summary>
        [Test]
        public void ObjetoTieneTipo_SinTipo()
        {
            string test1 = "\"123\"";  
            string test2 = "\"1^^23\"";
            string test3 = "1234";
            string test4 = "\"1234\"^^xsd:stri g";
            string test5 = "\"EL Correo de Galicia �pc�a �e Napelos V . VAPORB.-5 DE LA COMPA�IA del Llovd Norte Alem�n S��ics X-npe�al Alemana V&P0R33 GOaRBO'3 AL RIO D3 LA PLATA DOS EXPSD�OIONS-S V�SNSffALEB Sarvioio quiaesi�al. \" De VISO diraotiaiJata para MoaterMeo y Baotto� .Airea saldr� el 10 de Octubre el rapor correo alem�n STTTI TGkAIT El 24 de Octubre ?aldr� pora los mismos puntos el vapor correo alem�n \"W S IIvI -A. S�. Admite carga j p&sajercs. Twnbi�a admiten pasaderos para Ncb^A Yohk �son trasborde en Bbej�k��. Satos Taporea no Lac�n cuarentena ni en Mosteyideo ni en Buenos Aires. , Loa vpporea de esta Compa��a llsvan canareros y cocinoros espa�oles para atender � los pasajeros de tercera clase, los que tienen muy buen servicio de m ^sa, mag^iifica� I NORDEUCHESTER BEL LLOYD   Admiten estos vapores correspondencia y pasajeros de 1.� 2.a j 3a ^ i Los vapores de esta Compa��a hacen los i Cuentan estos magn�ficos vaDnrM S viajes directamente de Villagareia para la grandes comodidades para los sein!8 . M , 0 PLATA, sm tocar en Portugal ni Bras�, sajeroa de 1� j 2.� clase con �S?, I Se facilitan pasajes para Montevideo, , m<:.��m(�isJ� �ltimas casi siempre c�maras. Como tambi�n para loa S I 5��09T,'4�?�' S�0 Jansiro> ^P^0 Snj�tDS i cuarentena en Montevideo y niendo dos comidaa sem�nalaa i t ' i 5 Chile, Bohvia, Paraguay, Puerto Ewo, * ~J 1 -.3 mBB LljUIIIEL;iniLEITjUIZA�Z 05�; Galle Patio da Mm nm. 9. Santiago. Habana, Veracnu, M�jico y dem�a pun^ tos de todas las rep�blicas en todos loa vapores correos, . Acompa�a � loa pasajeros hasta dejarlos! embarcados. [Ojo! No dejarse enga�ar por otros agen- jj tes que por cobrar m�s barato el pasaje, � embarcan � los pasajeros en vaprres de   ! carga q.ue emplean en ^1 viaie � Monte- [ 2 video y Buenos Aires 28 y S� d�as, cuan- ? , do todos saben qus los buenos vaporea s�5 lo emplean �S d�as i dichos puntos, y pa- } i ra la Habana tambi�n los embarcan en va- ] pores qua emplean 18 & 20 d�as, cuando el ) ; Buenos Aires. i g� 7 servidos por coeinaroa t c��.^   � tambi�n gaUegoa. 7 ^   El 10 de Octubre saldr� de Villagarc�a directamente para Montevideo y ' ; Aires el vapor correo alem�n � STUTTGAJT : El 24 de Octubre saldr� para los mismos puntos el vapor correo alem�n i WEIMAR ? Para mas informes diriffirse- al coTisisruatano en Villagarc�a D. LDI8 SABOU 1 BOBJEDO Snbagente en Santiago: D. Man�eIi M�ndez, Altamira n�m. 2. * literas, la comida i la esp&Sola, todof los d�as variada y siempre con vino. Asistencia �; viaje � la Habana s�lo es da 9 � 10 d�as, m�dica graf�s._   Se facilitan- pasajes al fiado mediante Para m�s informes, dirigirs� al Agt-nt� da la CompaSla sn Vijo, D. AoeusTO B�K � garant�a, CENA Fkanoo, calta Beal niuaaro 8, baio. Sabagentes en S&atia�o: DO^T MANUEL MOHALKS YQ�NZAL3SZ. O�cinaa Pa. t�o de Madras, 9 y DON RAMuE FERNANDEZ V^SLA,, Pajera.'SS. Heprssentaci�n de la Asociaci�n ra�tua ^ de rademaones � met�lico.� ? Para obtener m�s informes dirigirse al a agente D. MANUEL MOEALES Y GONf ZALEZ: oficinas, patio de Madrea a�inei ro 9, � SANTIAGO Yiud arreras. m m msimkikms vi G O -S:1 i,a fe fagadii IrasatMk Para Rio Janeiro, Santos, Montevideo y Buenos Aires saldr� .de Vigo directamente el 5 de Octubre el vapor JOS� G-&X�I�ARV LINEA LAMPORT & HOLT Para Montevideo y Buenos Aires saldr� de Vigo directamente el 8 de Octubre el magn�fico trasatl�ntico 'ngl�s R O M SJ IT Viaja en 18 d�as. - Admitir� pasajeros de 1.a y 3.a clase. s�5 ^5. �1 � � .i: � ^ '\" \"'\"3 c � Si� �00\"-\" ?.� I Vffis&S COMPA�IA BOOTH (Royal Ma�) Para Par� y Manaos saldr� de este puerto el 13 de Octubre el magn�fico vapor co- ? rreo ingl�s \\ de porte de 7.000 toneladas y marcha de 16 millas. \\ Superior acomodaci�n para pasajeros do l.& y 3.a clase. t Duraci�n del viaje desde la salida de Lisboa � Par�, 10 d�as.� Admitii� pasajeros: � Paba e�fobmhs en Santiago: D. RAMON FSRNANDEZ VARELA, R�a. dkl 4 VILLA.B, 85, (Pajbsa). I >\" � \" e c ~ \\� s �= f� / < O \" - fz U � Va ��e SMS � Viajes r�pidos en quince d�as l H�rs'eog i3ejree&a<.-�>3sBtlago. SERVICIO DE LA 'AMIA TRA1 LINEA DE CUBA Y MEJICO 11 d�a 17 del corriente saldri de Bilbao, al 20 da Santander y 2 1 de la CorJ el vapor 3 REINA MAR�A CRISTINA i. directamente pafia la Habana y Veracnu. A.iinite pasaje y carga para Ccst&firaJ !Pac�fico con trasbordo en la Habana al vapor de la linea de Veneraela-ColoaiT   Combinaciones para el litoral de Cuba � Isla de Sasto Domingo, j LINEA DE NUEVA- YORK, CUBA Y M�JICO ; El d�a 26 dal corriesfce saldr� de Barcelona, el 3G de C�dis, el vapor I iBUENOS AIRES � directamnte para New- York, Habana y Veraoruz. Combinaciones para distinto� di f; tos de loa Estados Unidos, litoral de Cuba � Isla da Santo Domingo. B   LINEA DE VENEZUELA-COLOMBIA jj Si d�a li dol corriente saldr� de Barcelona, el 18 do M�laga y 15 de C4dii q! vaB'1 f MONSERRAT ; directamente para tas Palmas, Santa Cruz de Tenerife, Santa Cru� de la Pal Puerto Rico, Habana, Puerto Lim�n, Col�n, Sabanilla; Curagao Puerto Cabello j Guayara, admitiendo o.asaje y carga para Veraoruz con trasbordo en Ealaans. Co: >. naspor el ferrocarril de Panam� con las Compa��as de navegaci�n dal Pac�fica part� i yos puertos admite pasaje y carga con billetes y conocimientos directos. Cosabm�(   para el litoral de Caba y Puarto Rico. Se admite pasaje para Puerto Plata, con ti i bordo en Puerto Rico, y para Santo Domingo y San Pedro de Maooris, eon trasbo] i en Eabana. Tambi�n carga para Maracaibo, Campano y Trinidad coa traabordi j� Carvpano j LINEA. DS HLIPINAS % El d�a 18 del corrionta saldri de Liverpool y el 18 da la CoraSa, �1 vnpo� 1 ALICANTE f directamentopara Vigo, Lisboa, C�diz, Cartagena, Valencia y B*rcelona, de donde dr� el 30 del cornonte diractameute para Part Said, Sa�t, Colombo, Singapore y   ^a, sirviendo por trasbordo los puertos de la costa oriaatal Africa, is la Indi�, ] vs, Sumatra, China, Jap�n y Australia. L�nea de Ss�sbiss Airas , 2 dal oomeats saldr� de Barcelona, el 3 de Valencia, el 5 da Milag* J   de Cidis el magn�fico vapor � 1 Pn da Sa�fU9(aga/ directam g te para Santa Crus de Tenarif�, Montevideo y Bueaoa Airea. Los pasajeros de Galicia embarcar�n la Comaa �1 31 en na vapor da la Corspij � trasbordar an C�d�s. 1 LINEA DE CANARIAS oo F ^�a^7 d,eI corrieilte Barcelona, ol 18 de Vabaaia, �1 19 d� AliesaiJ 32 de Cidis, el vapor \"   J . MAISTUEL L. VILLA VERDE ; diroctamente para Casablanca. Maaagin, Las Palmas, Santa Gnu do 1� Palm� v S� � Oros ae Tenerife, regresando 4 Barcelona por C�dis, Alicante y Valonci*.   LINEA DS FERNANDO POO i El d�a 24 M corriente aaldr� de Barcelona y el 30 da Ci�ia, �1 rapor I 3AN FRANCISCO j para Fernanda P�o, con escala en- Casablaaoa, Masagin y otros naortos do 1� 04 < occidented de Africa y Golfo de Guinea. i En este Establecimiento se con� feccionan toda clase de obras, re! vistas, peri�dicos, estados, reci- El dla2id) Or.ubr) saldr� do Vi�U�arc�a pira 1a H^baua, Sagaa la Grande, San- bos para la cobranza del i�BpuestO LINEA DE TAN&5X Salidas de Cidii: Lunes, Mi�rcolos y Viernes, Salidas da i�ngw: Martas, 3mro<i r SAba^aa tiago de Cuba y Cienfuegos, el magn�fico vapor El d�a2G de Ojtabrj sal Ir� do Villa �ar �a pra Poma ab aco 3*hb, Rio Janeiro, Sontos, Montevideo y Buenos Airas, el vapor correo Soto v&por admite pasajeros de l^1 y S.\" clase. LAMALA REAL l��GL'SSA es la l�nea de vapores que hace los viajes m�s ripipidos entre Europa y Araerica del Sur, otrecic-ndo excelentes �modada -�es y trato i los pasajeros de primera y segunda olese. A loa de tercera se lea da cama con ropa, pan nreeoo y vino � todas las oozsid&M, Llevar� cocineros y cam.ar8ro3 espa&oles para mejor a�rrisio de los minaos y as�s- ; inscripci�n de fincas r�sticas y teaoia m�dica gratuita. > [ urbanas. Los pasajeros tienea que proR'jntarsrj �on ana pasaporus en la Agoscia de Vulagar- el dia antes de la salida del vapor, de consumos y del d�ficit, factul ras, mecabretes, recordatorios, es. quelas y tarjetas f�nebres y todos � Ij cuantos trabajos est�n incluidos jj en el arte tipogr�fico. � Esta Casa ; tiene � la venta k�jas de alta y � baja en la contribuci�n industrial, '. asi como las necesarias para la TARJETAS de visita de todas F�brica de Fundici�n   DE JUAN LISTE PEREIBA PaBBOQUIA DE OSAZO KN LA. �LIiA, Atottamis de Estilada, en la. pboto�oia db Pontbtbi'^I obi Se suplica � los pasajeros que doaeon smbarcarse on ohie vapor, a-^isen ca&nto antes, clases y tatGa�OS. p�pa reservarlo la cabida, por ser grand� �1 n�meTO da -�miM claaes solicitado. - . Paramifi informoa dirigiraa � los .�stiiivs i* !a CompaSis-aa V�lasarc� ��ftorea ' �smwn&i P^O�dOSg �C*BI�* s Esta casa establecida en 1884, es una espeoiali* se ha acreditado ,por el buen resultado de sus , , . pudiendo competir con las principales de Esp1 solo en los precios, sino en la perfecci�n de sus trabajos. Se presenta en las parroq para realizar los contratos, siendo conveniente, y se encarga de arrastres y coloca' nes bajo su rejponsabdidad, garantizando sus obras por el tiempo que se coaTeflg�' Recibe las viejas � cuenta y cobra tambi�a � plazos. Los la�onnes y pedido* se har�n directamente al fabricante Gran almac�n de ultramarinos finos.� Treguntoiro, 36, Santiago. �l que economiza es rico j el que quiera serlo que compre en el ALMACEN de y tendr� g�neros superiores y precios sin competencia. rdefi!r�� \"-� ^\"^^s: toszsszz�sssi \"�:�^r 100 * 10 selecta * ��. *� ^ i^E���M�^ variedad en vincs' licores ' aarapag'lc de ,as ^ acre<litadas F F 181 1 a int: lio tic i ir ps bes nos fcial it \\,: m im iMei lien Idue Ib* juna Ipist Icai leu; eid lal Ipai ftiti im m�o fm fi�ru �}k; i hi F.n fKeu lest 5 sus falta �igila [asar Win pe ei .ec ft�i mu pal final lervir lUpcia fiin s W\\ l\\ CO] Ifcrre I?* ate Ilutad K , . %\"";

            Assert.IsFalse(FacetadoAD.ObjetoTieneTipo(test1));
            Assert.IsFalse(FacetadoAD.ObjetoTieneTipo(test2));
            Assert.IsFalse(FacetadoAD.ObjetoTieneTipo(test3));
            Assert.IsFalse(FacetadoAD.ObjetoTieneTipo(test4));
            Assert.IsFalse(FacetadoAD.ObjetoTieneTipo(test5));
        }

        [Test]
        public void ComprobarSujetoValido()
        {
            string test1 = "<http://museodelprado.es/items/EmbeddedHTML_f8aa4d7c-0b81-3656-31dc-26b128a00292_f0d6a2e3-c5d6-444c-bcb8-e3160c51a781>";
            string test2 = "<https://museodelprado.es/items/EmbeddedHTML_f8aa4d7c-0b81-3656-31dc-26b128a00292_f0d6a2e3-c5d6-444c-bcb8-e3160c51a781>";
            string test3 = "<http://museodelprado.es/items/ProjectPage_f8aa4d7c-0b81-3656-31dc-26b128a00292_2bd36e19-9165-4ae8-840f-be030cf70425>";
            Assert.IsTrue(FacetadoAD.SujetoValido(test1));
            Assert.IsTrue(FacetadoAD.SujetoValido(test2));
            Assert.IsTrue(FacetadoAD.SujetoValido(test3));
        }

        [Test]
        public void ComprobarSujetoInValido()
        {
            string test1 = "<htt://museodelprado.es/items/EmbeddedHTML_f8aa4d7c-0b81-3656-31dc-26b128a00292_f0d6a2e3-c5d6-444c-bcb8-e3160c51a781>";
            string test2 = "<https://museodelprado.es/items/EmbeddedHTML_f8aa4d7-0b81-3656-31dc-26b128a00292_f0d6a2e3-c5d6-444c-bcb8-e3160c51a781>";
            string test3 = "<httpp://museodelprado.es/items/ProjectPage_f8aa4d7c-0b81-3656-31dc-26b128a00292_2bd36e19-9165-4ae8-840f-be030cf70425>";
            string test4 = "body.CollectionPageModel { padding: 32px 0; }";
            Assert.IsFalse(FacetadoAD.SujetoValido(test1));
            Assert.IsFalse(FacetadoAD.SujetoValido(test2));
            Assert.IsFalse(FacetadoAD.SujetoValido(test3));
            Assert.IsFalse(FacetadoAD.SujetoValido(test4));
        }
    }
}