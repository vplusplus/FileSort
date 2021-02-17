using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Utils.FileSort;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        const int OneKB = 1024;
        const int OneMB = OneKB * 1024;
        const int OneGB = OneMB * 1024;

        const string InputFileName_TenMB = "D:/Junk/FILESORT/Input-10MB.txt";
        const string InputFileName_OneGB = "D:/Junk/FILESORT/Input-1GB.txt";
        const string OutputFileName = "D:/Junk/FILESORT/Output.txt";

        [TestMethod]
        public void CreateLargeFile()
        {
            const string TestFileName = "D:/Junk/FILESORT/Input.txt";
            const int LineSize = 64;
            const int FileSize = OneMB * 10;

            // Since we write three-letter-plus-space.
            const int WordsPerLine = LineSize / 4;

            using (var writer = File.CreateText(TestFileName))
            {
                var randomWords = new RandomWords();
                var bytesWritten = 0;

                while(bytesWritten < FileSize)
                {
                    var nextLine = string.Join(" ", randomWords.Next().Take(WordsPerLine));
                    writer.WriteLine(nextLine);
                    bytesWritten += nextLine.Length;
                }
            }
        }

        [TestMethod]
        public void SplitSortMergeTest()
        {
            var inputFileName = InputFileName_OneGB;
            var bufferSize = OneMB * 100;
            var inputFileSize = new FileInfo(inputFileName).Length;

            Console.WriteLine($"FileSize   : {SizeToString(inputFileSize)}");
            Console.WriteLine($"BufferSize : {SizeToString(bufferSize)}");

            var timer = Stopwatch.StartNew();
            FileSorter.SortFile(inputFileName, OutputFileName, bufferSize);
            timer.Stop();

            Console.WriteLine($"Total      : {timer.Elapsed}");

        }

        static string SizeToString(long bytes)
        {
            if (bytes < 1024) return $"{bytes:#,0} bytes.";

            var kb = bytes / 1024;
            if (kb < 1024) return $"{kb:#,0} KB";

            var mb = kb / 1024;
            if (mb < 1024) return $"{mb:#,0} MB";

            var gb = mb / 1024.0;
            return $"{gb:#,0.0} GB";
        }

        class RandomWords
        {
            const string ThreeLetterWords = @"aahaalaasabaabsabyaceactaddadoadsadzaffaftagaageagoagsahaahiahsaidailaimainairaisaitajialaalbaleallalpalsaltamaamiampamuanaandaneaniantanyapeapoappaptarbarcarearfarkarmarsartashaskaspassateattaukavaaveavoawaaweawlawnaxeayeaysazobaabadbagbahbalbambanbapbarbasbatbaybedbeebegbelbenbesbetbeybibbidbigbinbiobisbitbizboabobbodbogboobopbosbotbowboxboybrabrobrrbubbudbugbumbunburbusbutbuybyebyscabcadcafcamcancapcarcatcawcayceecelcepchicigciscobcodcogcolconcoocopcorcoscotcowcoxcoycozcrucrycubcudcuecumcupcurcutcuzcwmdabdaddagdahdakdaldamdandapdasdawdaydebdeedefdeldendepdevdewdexdeydibdiddiedifdigdimdindipdisditdocdoedogdohdoldomdondordosdotdowdrydubdudduedugduhduidumdunduodupdyeeareateauebbecoecuedhedseekeeleeweffefsefteggegoekeeldelfelkellelmelsemeemoemsemuendengenseoneraereergernerrersessestetaetheveeweeyefabfadfagfahfanfarfasfatfaxfayfedfeefehfemfenferfesfetfeufewfeyfezfibfidfiefigfilfinfirfitfixfizfluflyfobfoefogfohfonfoofopforfoufoxfoyfrofryfubfudfugfunfurgabgadgaegaggalgamgangapgargasgatgaygedgeegelgemgengetgeyghigibgidgiegifgiggingipgisgitgnugoagobgodgoogorgosgotgoxgrrgulgumgungutguvguygymgyphadhaehaghahhajhamhaohaphashathawhayhehhemhenhepherheshethewhexheyhichidhiehimhinhiphishithmmhobhodhoehoghomhonhoohophothowhoyhubhuehughuhhumhunhuphuthypiceichickicyidsiffifsiggilkillimpinkinninsionireirkismitsivyjabjagjamjarjawjayjeejetjeujibjigjinjobjoejogjotjowjoyjugjunjusjutkabkaekafkaskatkaykeakefkegkenkepkexkeykhikidkifkinkipkirkiskitkoakobkoikopkorkoskuekyelablacladlaglahlamlaplarlaslatlavlawlaxlaylealedleelegleilekletleulevlexleyliblidlielinliplislitlobloglooloplotlowloxludluglumlunluvluxlyemacmadmaemagmammanmapmarmasmatmawmaxmaymedmegmehmelmemmenmetmewmhomibmicmidmigmilmimmirmismixmmmmoamobmocmodmogmoimolmommonmoomopmormosmotmowmudmugmummunmusmutmuxmycnabnaenagnahnamnannapnavnawnaynebneenegnetnewnibnilnimnipnitnixnobnodnognohnomnoonornosnotnownthnubnugnunnusnutoafoakoaroatobaobeobiocaochodaoddodeodsoesoffoftohmohoohsoikoilokaokeoldoleomaomsoneonoonsoofoohootopaopeopsoptoraorborcoreorgorsortoseoudouroutovaoweowlownowtoxooxypacpadpahpakpalpampanpapparpaspatpawpaxpaypeapecpedpeepegpehpenpepperpespetpewphiphophtpiapicpiepigpinpippispitpiupixplypodpohpoipolpompoopoppospotpowpoxproprypsipstpubpudpugpulpunpuppurpusputpyapyepyxqatqisquaradragrahrairajramranraprasratrawraxrayrebrecredreerefregreiremrepresretrevrexrezrhoriaribridrifrigrimrinriprobrocrodroeromroorotrowrubruerugrumrunrutryaryeryusabsacsadsaesagsalsansapsatsausawsaxsayseasecseesegseiselsensersetsevsewsexshasheshhshoshysibsicsigsimsinsipsirsissitsixskaskiskyslysobsocsodsohsolsomsonsopsossotsousowsoxsoyspaspysristysubsuesuksumsunsupsuqsussyntabtadtaetagtajtamtantaotaptartastattautavtawtaxteatectedteetegteltentestettewthethothytictietiltintiptistittixtiztodtoetogtomtontootoptortottowtoytrytsktubtugtuitumtuntuptuttuxtwatwotyeudoughukeuluummumpumsuniunsupoupsurburdurnurpuseutauteutsvacvanvarvasvatvauvavvawveevegvetvexviavidvievigvimvinvisvoevogvowvoxvugvumwabwadwaewagwanwapwarwaswatwawwaxwaywebwedweewenwetwhawhowhywigwinwiswitwizwoewokwonwoowoswotwowwrywudwyewynxisyagyahyakyamyapyaryasyawyayyeayehyenyepyesyetyewyinyipyobyodyokyomyonyouyowyukyumyupzagzapzaszaxzedzeezekzenzepzigzinzipzitzoazoozuzzzz";
            readonly int WordCount = ThreeLetterWords.Length / 3;

            readonly Random Rnd = new Random(Guid.NewGuid().GetHashCode());

            public IEnumerable<string> Next(int maxWords = 1024)
            {
                for(int i=0; i<maxWords; i++)
                {
                    var index = Rnd.Next(0, WordCount) * 3;
                    yield return ThreeLetterWords.Substring(index, 3);
                }
            }
        }

    }
}
