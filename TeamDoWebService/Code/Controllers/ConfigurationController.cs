using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

using S = System.Collections.Generic;
using System.Configuration;

namespace TeamDoWebService.Code.Controllers
{
    /// <summary>
    /// Configuration wrapper
    /// </summary>
    public static class ConfigurationController
    {
        private static string _indexRootPath;

        private static string _tempIndexRootPath;
        private static string _backupsRootPath;
        private static ISet<string> _stop_Words;
        private static Hashtable _ht_Stop_Words;
        private static string[] _stop_Words_Default = {
			"ación","cion","pluri","multi","a","ac","ah","ajena","ajenas","ajeno","ajenos","al","algo","alguna","algunas","alguno","algunos","algn",
			"all","aquel","aquella","aquellas","aquello","aquellos","aqu","cada","cierta","ciertas","cierto","ciertos",
			"como","con","conmigo","consigo","contigo","cualquier","cualquiera","cualquieras","cuan","cuanta","cuantas","cuanto",
			"cuantos","cun","cunta","cuntas","cunto","cuntos","cmo","de","dejar","del","demasiada","demasiadas","demasiado","demasiados",
			"dems","el","ella","ellas","ellos","esa","esas","ese","esos","esta","estar","estas","este","estos","hacer","hasta",
			"jams","junto","juntos","la","las","lo","los","mas","me","menos","mientras","misma","mismas","mismo",
			"mismos","mucha","muchas","mucho","muchos","muchsima","muchsimas","muchsimo","muchsimos","muy","ms",
			"ma","mo","nada","ni","ninguna","ningunas","ninguno","ningunos","nos","nosotras","nosotros","nuestra","nuestras",
			"nuestro","nuestros","nunca","os","otra","otras","otro","otros","para","parecer","poca","pocas","poco",
			"pocos","por","porque","que","querer","quien","quienes","quienesquiera","quienquiera","quin","qu","ser",
			"si","siempre","sr","sra","sres","sta","suya","suyas","suyo","suyos","s","sn","tal","tales","tan","tanta","tantas","tanto",
			"tantos","te","tener","ti","toda","todas","todo","todos","tomar","tuya","tuyo","t","un","una","unas","unos","usted","ustedes",
			"varias","varios","vosotras","vosotros","vuestra","vuestras","vuestro","vuestros","y","yo","l",
	        "an", "and", "are", "as", "at", "be", "but", "by", "for", 
	        "if", "in", "into", "is", "it", "of", "on", "or", 
	        "such", "that", "the", "their", "then", "there", "these", "they", "this", "to", 
	        "was", "will", "with"};


        public static string MainConnectionString
        {
            get 
            {
                if (ConfigurationManager.ConnectionStrings["MainConnectionString"] != null)
                    return ConfigurationManager.ConnectionStrings["MainConnectionString"].ConnectionString;
                return string.Empty;
            }
        }

        public static string IndexRootPath
        {
            get
            {
                if (string.IsNullOrEmpty(_indexRootPath))
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["IndexRootPath"] != null)
                        _indexRootPath = System.Configuration.ConfigurationManager.AppSettings["IndexRootPath"].ToString();
                    else
                        _indexRootPath = string.Empty;
                }
                return _indexRootPath;
            }
        }


        public static string TempIndexRootPath
        {
            get
            {
                if (string.IsNullOrEmpty(_tempIndexRootPath))
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["TempIndexRootPath"] != null)
                        _tempIndexRootPath = System.Configuration.ConfigurationManager.AppSettings["TempIndexRootPath"].ToString();
                    else
                        _tempIndexRootPath = "c:/temp/test";
                }
                return _tempIndexRootPath;
            }
        }

        public static string BackupsRootPath
        {
            get
            {
                if (string.IsNullOrEmpty(_backupsRootPath))
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["BackupsRootPath"] != null)
                        _backupsRootPath = System.Configuration.ConfigurationManager.AppSettings["BackupsRootPath"].ToString();
                    else
                        _backupsRootPath = string.Empty;
                }
                return _backupsRootPath;
            }
        }

        public static bool RunEnglishIndexing
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["RunEnglishIndex"] != null)
                    return System.Configuration.ConfigurationManager.AppSettings["RunEnglishIndex"].ToString() == "1";
                else
                    return false;
            }
        }

        public static bool RunSpanishIndexing
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["RunSpanishIndex"] != null)
                    return System.Configuration.ConfigurationManager.AppSettings["RunSpanishIndex"].ToString() == "1";
                else
                    return false;
            }
        }

        public static bool RunHebrewIndex
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["RunHebrewIndex"] != null)
                    return System.Configuration.ConfigurationManager.AppSettings["RunHebrewIndex"].ToString() == "1";
                else
                    return false;
            }
        }

        public static string MorphFilesPath
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["RelativeMorphFiles"] != null)
                    //return System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/"), System.Configuration.ConfigurationManager.AppSettings["RelativeMorphFiles"].ToString());
                    return System.Configuration.ConfigurationManager.AppSettings["RelativeMorphFiles"].ToString();
                else
                    return string.Empty;
            }
        }

        public static ISet<string> Stop_Words
        {
            get
            {
                if (_stop_Words == null)
                {
                    _stop_Words = new S.HashSet<string>();
                    // _stop_Words.AddRange(_stop_Words_Default);
                    foreach (string wrd in _stop_Words_Default)
                    {
                        _stop_Words.Add(wrd);
                    }

                    return _stop_Words;
                }
                return _stop_Words;
            }
        }

        /// <summary>
        /// Stop words hashtable
        /// </summary>
        public static Hashtable Stop_Words_HT
        {
            get
            {
                if (_ht_Stop_Words == null)
                {
                    _ht_Stop_Words = new Hashtable();
                    foreach (string str in _stop_Words_Default)
                        _ht_Stop_Words.Add(str, str);

                }
                return _ht_Stop_Words;
            }
        }
    }
}