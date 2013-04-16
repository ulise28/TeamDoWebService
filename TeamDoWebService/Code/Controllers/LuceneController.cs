using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis;
using TeamDoWebService.Code.Dto;
using Lucene.Net.Store;
using TeamDoWebService.Code.Dao;
using Lucene.Net.Index;

using T = TeamDoWebService.Code.Controllers.Trace;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using TeamDoWebService.Contracts;
using Lucene.Net.Analysis.Hebrew;

namespace TeamDoWebService.Code.Controllers
{
    public class LuceneController
    {
        /// <summary>
        /// Update Index by Language
        /// </summary>
        /// <param name="indexDir"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public static void UpdateIndex(String lng)
        {
            Analyzer analyzer = new SpanishAnalyzer(ConfigurationController.Stop_Words);

            Directory indexDir = FSDirectory.Open(new System.IO.DirectoryInfo(ConfigurationController.TempIndexRootPath + "/ES/IDX"));
            if (lng.ToLower().Trim().Equals("en"))
            {
                indexDir = FSDirectory.Open(new System.IO.DirectoryInfo(ConfigurationController.TempIndexRootPath + "/EN/IDX"));
                analyzer = new EnglishAnalyzer(ConfigurationController.Stop_Words);
            }
            if (lng.ToLower().Trim().Equals("he"))
            {
                indexDir = FSDirectory.Open(new System.IO.DirectoryInfo(ConfigurationController.TempIndexRootPath + "/HE/IDX"));
                analyzer = new MorphAnalyzer(ConfigurationController.MorphFilesPath);
            }

            LuceneDao dao = new LuceneDao();
            dao.Analizer = analyzer;

            dao.UpdateIndex(indexDir, lng);
        }


        public static void MoveIndex(bool indexKeywords, String lng, int indexServer)
        {

            MoveLuceneIndex(lng, indexServer);
        }

        private static void MoveLuceneIndex(String lng, int indexServer)
        {
            T.TraceMessage("Moving lucene index to server {0}", indexServer);

            Directory[] readers = new Directory[1];
            string impDomain = string.Empty, impUser = string.Empty, impPass = string.Empty;
            string destIndexBasePath = string.Empty;

            if (indexServer == 1)
            {
                destIndexBasePath = ConfigurationController.IndexRootPath;
            }

            Analyzer analyzer = new SpanishAnalyzer(ConfigurationController.Stop_Words);
            string destIndexPath = destIndexBasePath + "\\ES\\IDX";
            string tempIndexPath = ConfigurationController.TempIndexRootPath + "/ES/IDX";

            if (lng.ToLower().Trim().Equals("en"))
            {
                destIndexPath = destIndexBasePath + "\\EN\\IDX";
                tempIndexPath = ConfigurationController.TempIndexRootPath + "/EN/IDX";
                analyzer = new EnglishAnalyzer(ConfigurationController.Stop_Words);
            }
            if (lng.ToLower().Trim().Equals("he"))
            {
                destIndexPath = destIndexBasePath + "\\HE\\IDX";
                tempIndexPath = ConfigurationController.TempIndexRootPath + "/HE/IDX";
                analyzer = new MorphAnalyzer(ConfigurationController.MorphFilesPath);
            }

            MoveIndexFiles(impDomain, impUser, impPass, destIndexPath, tempIndexPath, analyzer);
        }

        private static void MoveIndexFiles(string impDomain, string impUser, string impPass, string destIndexPath, string tempIndexPath, Analyzer analyzer)
        {
            Directory[] readers = new Directory[1];
            IndexWriter writer = null;

            Directory finalIndexDir = FSDirectory.Open(new System.IO.DirectoryInfo(destIndexPath));
            System.IO.DirectoryInfo tempIndexDir = new System.IO.DirectoryInfo(tempIndexPath);
            try
            {
                if (IndexWriter.IsLocked(finalIndexDir)) IndexWriter.Unlock(finalIndexDir);
                // re-generate the index
                writer = new IndexWriter(finalIndexDir, analyzer, true, new IndexWriter.MaxFieldLength(2500000));
                readers[0] = FSDirectory.Open(tempIndexDir);
                writer.AddIndexesNoOptimize(readers);

                // optimize and close
                if (writer != null)
                {
                    try
                    {
                        writer.Optimize();
                    }
                    catch { }
                }
                if (writer != null)
                {
                    try
                    {
                        writer.Close();
                    }
                    catch { }
                }

            }
            catch (Exception ex)
            {
                if (writer != null)
                {
                    writer.Optimize();
                    writer.Commit();
                    writer.Close();
                }
                throw ex;
            }
        }

        public static bool BackupIndex()
        {
            string sourceFolderPath = ConfigurationController.IndexRootPath;
            string destFolderPath = ConfigurationController.BackupsRootPath + "/" + DateTime.Now.ToString("dd-MM-yyyy");

            try
            {
                CopyDirectory(sourceFolderPath, destFolderPath);
            }
            catch (Exception ex)
            {
                T.TraceError("Could not backup to {0}", destFolderPath);
                T.TraceError(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Indexing a single publication, passed as parameter
        /// </summary>
        /// <param name="bean"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public static int IndexPublication(IssueDocumentDto bean, string lng)
        {
            Analyzer analyzer = new SpanishAnalyzer(ConfigurationController.Stop_Words);

            if (!string.IsNullOrEmpty(ConfigurationController.IndexRootPath))
            {
                Directory indexDir = FSDirectory.Open(new System.IO.DirectoryInfo(ConfigurationController.IndexRootPath + "/ES/IDX"));
                if (!lng.ToLower().Trim().Equals("es"))
                    indexDir = FSDirectory.Open(new System.IO.DirectoryInfo(ConfigurationController.IndexRootPath + "/EN/IDX"));

                LuceneDao dao = new LuceneDao();
                dao.Analizer = analyzer;

                return dao.IndexPublication(indexDir, bean);

            }

            return -1;
        }

        public static void CopyDirectory(string source, string target)
        {
            var stack = new Stack<Folders>();
            stack.Push(new Folders(source, target));

            while (stack.Count > 0)
            {
                var folders = stack.Pop();
                System.IO.Directory.CreateDirectory(folders.Target);
                foreach (var file in System.IO.Directory.GetFiles(folders.Source, "*.*"))
                {
                    string targetFile = System.IO.Path.Combine(folders.Target, System.IO.Path.GetFileName(file));
                    if (System.IO.File.Exists(targetFile)) System.IO.File.Delete(targetFile);
                    System.IO.File.Copy(file, targetFile);
                }

                foreach (var folder in System.IO.Directory.GetDirectories(folders.Source))
                {
                    stack.Push(new Folders(folder, System.IO.Path.Combine(folders.Target, System.IO.Path.GetFileName(folder))));
                }
            }
        }




        public static Query GetLuceneQuery(string text)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentException("GetLuceneQuery empty text exception");

            Query luceneQuery = null;

            if (text.Length > 0)
            {
                // by default the simple search is limited to Id/Name/Description
                List<string> searchTp = BuildDefaultSearchType();

                luceneQuery = GetEnglishAnalyzedQuery(text, searchTp);
            }

            return luceneQuery;
        }

        /// <summary>
        /// Simple search (default search types)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="searchType"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<IssueDocument> DoSearch(string searchString, int startIndex, int blockSize, string sortBy)
        {
            List<IssueDocument> result = new List<IssueDocument>();

            DateTime ini = DateTime.Now;

            List<string> searchTp = BuildDefaultSearchType();
            Query searchQuery = CombineQueries(searchString, searchTp);

            // log purposes
            //DateTime fin = DateTime.Now;
            //TimeSpan ts = fin.Subtract(ini);
            //float abc = ts.Seconds + ts.Milliseconds * 0.001f;
            //T.TraceMessage("Build query time is {0}", abc);
            //ini = DateTime.Now;

            List<IssueDocument> searchResult = PerformSearch(searchQuery, startIndex, blockSize, sortBy);

            // log purposes
            //fin = DateTime.Now;
            //ts = fin.Subtract(ini);
            //abc = ts.Seconds + ts.Milliseconds * 0.001f;
            //T.TraceMessage("Perform search query time {0}", abc);

            return searchResult;
        }

        /// <summary>
        /// Open the index folders and call the DAO Search method
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="userId"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        private static List<IssueDocument> PerformSearch(Query searchQuery, int startIndex, int blockSize, string sortBy)
        {
            List<IssueDocument> result = null;
            string indexRootPath = ConfigurationController.IndexRootPath;
            Directory indexDirEs, indexDirEn, indexDirHe;

            try
            {
                indexDirEs = FSDirectory.Open(new System.IO.DirectoryInfo(indexRootPath + "/ES/IDX"));
                indexDirEn = FSDirectory.Open(new System.IO.DirectoryInfo(indexRootPath + "/EN/IDX"));
                indexDirHe = FSDirectory.Open(new System.IO.DirectoryInfo(indexRootPath + "/HE/IDX"));
            }
            catch
            {
                throw new ApplicationException("The index directory does not exist");
            }

            LuceneDao dao = new LuceneDao();

            result = dao.MedesSearch(searchQuery, startIndex, blockSize, indexDirEs, indexDirEn, indexDirHe, sortBy);

            return result;
        }





        /// <summary>
        /// Combines the Spanish and English queries into a single query
        /// Used to search in both versions, including stemming, accents, etc
        /// </summary>
        /// <param name="text"></param>
        /// <param name="searchTypes"></param>
        /// <returns></returns>
        private static Query CombineQueries(string text, List<string> searchTypes)
        {
            Query searchQueryES = GetSpanishAnalyzedQuery(text, searchTypes);
            Query searchQueryEN = GetEnglishAnalyzedQuery(text, searchTypes);
            Query searchQueryHE = GetMorphAnalyzedQuery(text, searchTypes);

            // combine the ES, EN and HE queries
            Query[] arrQueries = new Query[3];
            arrQueries[0] = searchQueryES;
            arrQueries[1] = searchQueryEN;
            arrQueries[2] = searchQueryHE;
            Query searchQuery = searchQueryES.Combine(arrQueries);

            return searchQuery;
        }

        private static List<string> BuildDefaultSearchType()
        {
            List<string> myList = new List<string>();

            myList.Add("article_id");
            myList.Add("name");
            myList.Add("description");

            return myList;
        }

        /// <summary>
        /// Get the query resulted from an English Analyzer
        /// </summary>
        /// <param name="text"></param>
        /// <param name="searchType"></param>
        /// <returns></returns>
        private static Query GetEnglishAnalyzedQuery(string text, List<string> searchType)
        {
            PerFieldAnalyzerWrapper pfAnalyzer = new PerFieldAnalyzerWrapper(new EnglishAnalyzer(ConfigurationController.Stop_Words));

            MultiFieldQueryParser mParser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, searchType.ToArray(), pfAnalyzer);

            return GetQueryFromText(text, mParser);
        }

        /// <summary>
        /// Get the query resulted from an Hebrew Analyzer
        /// </summary>
        /// <param name="text"></param>
        /// <param name="searchType"></param>
        /// <returns></returns>
        private static Query GetMorphAnalyzedQuery(string text, List<string> searchType)
        {
            PerFieldAnalyzerWrapper pfAnalyzer = new PerFieldAnalyzerWrapper(new MorphAnalyzer(ConfigurationController.MorphFilesPath));

            MultiFieldQueryParser mParser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, searchType.ToArray(), pfAnalyzer);

            return GetQueryFromText(text, mParser);
        }


        /// <summary>
        /// Get the query resulted from a Spanish Analyzer
        /// </summary>
        /// <param name="text"></param>
        /// <param name="searchType"></param>
        /// <returns></returns>
        public static Query GetSpanishAnalyzedQuery(string text, List<string> searchType)
        {
            PerFieldAnalyzerWrapper pfAnalyzer = new PerFieldAnalyzerWrapper(new SpanishAnalyzer(ConfigurationController.Stop_Words));

            MultiFieldQueryParser mParser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29, searchType.ToArray(), pfAnalyzer);

            return GetQueryFromText(text, mParser);
        }

        private static Query GetQueryFromText(string text, MultiFieldQueryParser mParser)
        {
            Query query = null;

            try
            {
                mParser.DefaultOperator = QueryParser.AND_OPERATOR;
                query = mParser.Parse(text);
            }
            catch (Exception ex)
            {
                T.TraceError("Error GetQueryFromText <text>{0}</text>", text);
                T.TraceError(ex);
                throw ex;
            }
            return query;
        }










        public class Folders
        {
            public string Source { get; private set; }
            public string Target { get; private set; }

            public Folders(string source, string target)
            {
                Source = source;
                Target = target;
            }
        }

    }
}