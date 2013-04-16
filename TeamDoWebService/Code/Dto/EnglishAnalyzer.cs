using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis.Snowball;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;

namespace TeamDoWebService.Code.Dto
{
    public class EnglishAnalyzer : Analyzer
    {
        private static SnowballAnalyzer analyzer;

        private ISet<string> STOP_WORDS;

        public EnglishAnalyzer()
        {
            analyzer = new SnowballAnalyzer(Lucene.Net.Util.Version.LUCENE_30, "English");
        }

        public EnglishAnalyzer(ISet<string> stop_words)
        {
            analyzer = new SnowballAnalyzer( Lucene.Net.Util.Version.LUCENE_30, "English", stop_words);

            STOP_WORDS = stop_words;
        }

        public override TokenStream TokenStream(string fieldName, System.IO.TextReader reader)
        {
            TokenStream result = new StandardTokenizer(Lucene.Net.Util.Version.LUCENE_29, reader);
            //result = new StandardFilter(result);
            result = new LowerCaseFilter(result);

            if (STOP_WORDS != null)
                result = new StopFilter(false, result, STOP_WORDS);
            result = new ASCIIFoldingFilter(result);
            result = new SnowballFilter(result, "English");

            return result;

        }
    }
}