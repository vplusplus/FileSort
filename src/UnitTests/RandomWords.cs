﻿
using System;
using System.Collections.Generic;

namespace UnitTests
{
    /// <summary>
    /// Can generate random three letter words.
    /// </summary>
    internal static class RandomWords
    {
        // Three letter words.
        const string ThreeLetterWords = @"aahaalaasabaabsabyaceactaddadoadsadzaffaftagaageagoagsahaahiahsaidailaimainairaisaitajialaalbaleallalpalsaltamaamiampamuanaandaneaniantanyapeapoappaptarbarcarearfarkarmarsartashaskaspassateattaukavaaveavoawaaweawlawnaxeayeaysazobaabadbagbahbalbambanbapbarbasbatbaybedbeebegbelbenbesbetbeybibbidbigbinbiobisbitbizboabobbodbogboobopbosbotbowboxboybrabrobrrbubbudbugbumbunburbusbutbuybyebyscabcadcafcamcancapcarcatcawcayceecelcepchicigciscobcodcogcolconcoocopcorcoscotcowcoxcoycozcrucrycubcudcuecumcupcurcutcuzcwmdabdaddagdahdakdaldamdandapdasdawdaydebdeedefdeldendepdevdewdexdeydibdiddiedifdigdimdindipdisditdocdoedogdohdoldomdondordosdotdowdrydubdudduedugduhduidumdunduodupdyeeareateauebbecoecuedhedseekeeleeweffefsefteggegoekeeldelfelkellelmelsemeemoemsemuendengenseoneraereergernerrersessestetaetheveeweeyefabfadfagfahfanfarfasfatfaxfayfedfeefehfemfenferfesfetfeufewfeyfezfibfidfiefigfilfinfirfitfixfizfluflyfobfoefogfohfonfoofopforfoufoxfoyfrofryfubfudfugfunfurgabgadgaegaggalgamgangapgargasgatgaygedgeegelgemgengetgeyghigibgidgiegifgiggingipgisgitgnugoagobgodgoogorgosgotgoxgrrgulgumgungutguvguygymgyphadhaehaghahhajhamhaohaphashathawhayhehhemhenhepherheshethewhexheyhichidhiehimhinhiphishithmmhobhodhoehoghomhonhoohophothowhoyhubhuehughuhhumhunhuphuthypiceichickicyidsiffifsiggilkillimpinkinninsionireirkismitsivyjabjagjamjarjawjayjeejetjeujibjigjinjobjoejogjotjowjoyjugjunjusjutkabkaekafkaskatkaykeakefkegkenkepkexkeykhikidkifkinkipkirkiskitkoakobkoikopkorkoskuekyelablacladlaglahlamlaplarlaslatlavlawlaxlaylealedleelegleilekletleulevlexleyliblidlielinliplislitlobloglooloplotlowloxludluglumlunluvluxlyemacmadmaemagmammanmapmarmasmatmawmaxmaymedmegmehmelmemmenmetmewmhomibmicmidmigmilmimmirmismixmmmmoamobmocmodmogmoimolmommonmoomopmormosmotmowmudmugmummunmusmutmuxmycnabnaenagnahnamnannapnavnawnaynebneenegnetnewnibnilnimnipnitnixnobnodnognohnomnoonornosnotnownthnubnugnunnusnutoafoakoaroatobaobeobiocaochodaoddodeodsoesoffoftohmohoohsoikoilokaokeoldoleomaomsoneonoonsoofoohootopaopeopsoptoraorborcoreorgorsortoseoudouroutovaoweowlownowtoxooxypacpadpahpakpalpampanpapparpaspatpawpaxpaypeapecpedpeepegpehpenpepperpespetpewphiphophtpiapicpiepigpinpippispitpiupixplypodpohpoipolpompoopoppospotpowpoxproprypsipstpubpudpugpulpunpuppurpusputpyapyepyxqatqisquaradragrahrairajramranraprasratrawraxrayrebrecredreerefregreiremrepresretrevrexrezrhoriaribridrifrigrimrinriprobrocrodroeromroorotrowrubruerugrumrunrutryaryeryusabsacsadsaesagsalsansapsatsausawsaxsayseasecseesegseiselsensersetsevsewsexshasheshhshoshysibsicsigsimsinsipsirsissitsixskaskiskyslysobsocsodsohsolsomsonsopsossotsousowsoxsoyspaspysristysubsuesuksumsunsupsuqsussyntabtadtaetagtajtamtantaotaptartastattautavtawtaxteatectedteetegteltentestettewthethothytictietiltintiptistittixtiztodtoetogtomtontootoptortottowtoytrytsktubtugtuitumtuntuptuttuxtwatwotyeudoughukeuluummumpumsuniunsupoupsurburdurnurpuseutauteutsvacvanvarvasvatvauvavvawveevegvetvexviavidvievigvimvinvisvoevogvowvoxvugvumwabwadwaewagwanwapwarwaswatwawwaxwaywebwedweewenwetwhawhowhywigwinwiswitwizwoewokwonwoowoswotwowwrywudwyewynxisyagyahyakyamyapyaryasyawyayyeayehyenyepyesyetyewyinyipyobyodyokyomyonyouyowyukyumyupzagzapzaszaxzedzeezekzenzepzigzinzipzitzoazoozuzzzz";

        // Good enough pseudo random generator, for our use-case.
        static readonly Random Rnd = new Random(Guid.NewGuid().GetHashCode());

        // No of words in my list.
        static readonly int WordCount = ThreeLetterWords.Length / 3;

        /// <summary>
        /// Generates a sequence of random three letter words, capped to 'maxWords'
        /// </summary>
        public static IEnumerable<string> Next(int maxWords = 1024)
        {
            for (int i = 0; i < maxWords; i++)
            {
                var index = Rnd.Next(0, WordCount) * 3;
                yield return ThreeLetterWords.Substring(index, 3);
            }
        }
    }
}
