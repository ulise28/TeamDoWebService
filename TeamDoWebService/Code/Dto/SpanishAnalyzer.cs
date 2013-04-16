using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Snowball;

namespace TeamDoWebService.Code.Dto
{
    public class SpanishAnalyzer : Analyzer
    {
        private static SnowballAnalyzer analyzer;

        private ISet<string> STOP_WORDS;

        public SpanishAnalyzer()
        {
            analyzer = new SnowballAnalyzer(Lucene.Net.Util.Version.LUCENE_30, "Spanish");
        }

        public SpanishAnalyzer(ISet<string> stop_words)
        {
            analyzer = new SnowballAnalyzer(Lucene.Net.Util.Version.LUCENE_30, "Spanish", stop_words);

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

            // we are using a distinct version of the Spanish stemmer, called Spanish2
            // Please check if this class can be found in the Snowball library, the relative path 
            // should be: Snowball\SF\Snowball\Ext\
            // just in case, I would leave a copy of this class in this project
            result = new SnowballFilter(result, "Spanish");

            return result;
        }
    }
}