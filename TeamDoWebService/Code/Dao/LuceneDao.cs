using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Analysis;
using Lucene.Net.Store;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;


using T = TeamDoWebService.Code.Controllers.Trace;
using TeamDoWebService.Code.Dto;
using TeamDoWebService.Contracts;
using Lucene.Net.Search.Function;

namespace TeamDoWebService.Code.Dao
{
    /// <summary>
    /// Lucene operations
    /// </summary>
    public class LuceneDao
    {
        private const string CLASS_NAME = " MedesIndexService.Code.Dao.LuceneDao";

        private Analyzer _analizer;

        public Analyzer Analizer
        {
            get { return _analizer; }
            set { _analizer = value; }
        }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="context"></param>
        public LuceneDao()
        {

        }

        /// <summary>
        /// Update Index by Language
        /// </summary>
        /// <param name="indexDir"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public void UpdateIndex(Lucene.Net.Store.Directory indexDir, String lng)
        {
#if DEBUG
            T.TraceMessage("Begin full indexing , language {0}, time: {1}", lng, DateTime.Now.ToLongTimeString());
#endif
            IndexWriter writer = null;
            int step = 1000;
            int totalDoc = 0;

            try
            {
                DateTime beginTime = DateTime.Now;
                int num = 0, numPar = 0;

                num = new IssuesDao().GetTotalPublicationByLanguage(lng);


                if (IndexWriter.IsLocked(indexDir))
                    IndexWriter.Unlock(indexDir);
                writer = new IndexWriter(indexDir, this.Analizer, true, new IndexWriter.MaxFieldLength(2500000));

                // change number of documents to store in memory before writing them to the disk
                //writer.SetMergeFactor();
                if (num != 0)
                {
                    if (step >= num)
                    {
                        long ini = DateTime.Now.Millisecond;
                        totalDoc = Index(writer, 1, num, lng);
                        long fin = DateTime.Now.Millisecond;
                    }
                    else
                    {
                        int numStep = (num / step);
                        int cont = 1;
                        int start = 0;
                        int end = 0;

                        for (int i = 0; i < numStep; i++)
                        {
                            start = (cont + (i * step));
                            end = start + step;
                            DateTime ini = DateTime.Now;

                            numPar = Index(writer, start, end, lng);

                            DateTime fin = DateTime.Now;
                            TimeSpan ts = fin.Subtract(ini);

#if DEBUG
                            T.TraceMessage("Indexing from {0} to {1} took {2} seconds", start, end, (ts.Minutes * 60 + ts.Seconds));
#endif
                        }

                        if (num != end)
                        {
                            start = end;
                            end = num;

                            numPar = Index(writer, start, end + 1, lng);
                        }
                    }
                }
                DateTime endTime = DateTime.Now;
                TimeSpan tsFull = endTime.Subtract(beginTime);

                //Commit writer                
                writer.Commit();
                writer.Dispose();

#if DEBUG
                T.TraceMessage("Full indexing took {0} seconds, indexed {1} documents", ((tsFull.Hours * 3600) + (tsFull.Minutes * 60) + tsFull.Seconds), writer.MaxDoc());
#endif
            }
            catch (Exception ex)
            {
                if (writer != null)
                {
                    writer.Optimize();
                    writer.Commit();
                    writer.Dispose();
                }
                T.TraceError("Error Full Index , class {0}, language {1} ", CLASS_NAME, lng);
                T.TraceError(ex);
                throw ex;
            }
        }

        private int Index(IndexWriter writer, int start, int end, String lng)
        {
#if DEBUG
            T.TraceMessage("Begin indexing from {0} to {1}, language {2}, time: {3}", start, end, lng, DateTime.Now.ToLongTimeString());
#endif
            Document doc = null;
            Field field = null;
            Dictionary<string, IssueDocumentDto> list = new Dictionary<string, IssueDocumentDto>();

            try
            {

                list = new IssuesDao().GetIssuesIndexList(start, end, lng);


                foreach (KeyValuePair<string, IssueDocumentDto> pair in list)
                {
                    IssueDocumentDto bean = pair.Value;
                    doc = new Document();

                    doc.Add(new Field("issue_id", bean.IssueId.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                    if(bean.CompanyId !=-1) doc.Add(new Field("company_id", bean.CompanyId.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                    if (bean.IssueIdPerCompany != -1) doc.Add(new Field("issue_per_company", bean.IssueIdPerCompany.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                    if (bean.PriorityId != -1) doc.Add(new Field("priority_id", bean.PriorityId.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                    if (bean.StatusId != -1) doc.Add(new Field("status_id", bean.StatusId.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                    if (!string.IsNullOrEmpty(bean.CurrentOwner))  doc.Add(new Field("curr_owner", bean.CurrentOwner, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                    if (!string.IsNullOrEmpty(bean.LastOwner)) doc.Add(new Field("last_owner", bean.LastOwner, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                    if (!string.IsNullOrEmpty(bean.ProjectName)) doc.Add(new Field("project_name", bean.ProjectName, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));

                    if (bean.AssignedCUDate != DateTime.MinValue) doc.Add(new Field("assigned_cu_date", DateTools.DateToString(bean.AssignedCUDate, DateTools.Resolution.DAY), Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
                    if (bean.DestinationDate != DateTime.MinValue) doc.Add(new Field("destination_date", DateTools.DateToString(bean.DestinationDate, DateTools.Resolution.DAY), Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
                    if (!string.IsNullOrEmpty(bean.DescLast)) doc.Add(new Field("desc_last", bean.DescLast, Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
                    if (bean.ReadUsersBitIds1 != -1) doc.Add(new Field("read_usr_bit1", bean.ReadUsersBitIds1.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                    if (bean.ReadUsersBitIds2 != -1) doc.Add(new Field("read_usr_bit1", bean.ReadUsersBitIds2.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                    if (bean.LastUpdate != DateTime.MinValue) doc.Add(new Field("last_update", DateTools.DateToString(bean.LastUpdate, DateTools.Resolution.DAY), Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
                    if (bean.StartDate != DateTime.MinValue) doc.Add(new Field("start_date", DateTools.DateToString(bean.StartDate, DateTools.Resolution.DAY), Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
                    if(bean.IsAllDay) doc.Add(new Field("is_all_day", "1", Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                    else doc.Add(new Field("is_all_day", "0", Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                    if (bean.DestReminderDate != DateTime.MinValue) doc.Add(new Field("dest_rem_date", DateTools.DateToString(bean.DestReminderDate, DateTools.Resolution.DAY), Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
                    if (bean.ReccurenceId != -1) doc.Add(new Field("reccurence_id", bean.ReccurenceId.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                    

                    writer.AddDocument(doc);
                }

                writer.Optimize();
            }
            catch (Exception ex)
            {
                T.TraceError("Error Index (start: {1}, end: {2}, language {3}), class {0} ", CLASS_NAME, start, end, lng);
                T.TraceError(ex);
                throw ex;
            }
            return list.Count;
        }

        public int IndexPublication(Lucene.Net.Store.Directory indexDir, IssueDocumentDto bean)
        {
            IndexWriter writer = null;
            Document doc = null;
            Field field = null;
            IndexWriter idxModif = null;

            int numIndexed = 0;

            try
            {
                if (IndexWriter.IsLocked(indexDir))
                    IndexWriter.Unlock(indexDir);
                idxModif = new IndexWriter(indexDir, this.Analizer, false, new IndexWriter.MaxFieldLength(2500000));

                doc = new Document();

                doc.Add(new Field("issue_id", bean.IssueId.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                if (bean.CompanyId != -1) doc.Add(new Field("company_id", bean.CompanyId.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                if (bean.IssueIdPerCompany != -1) doc.Add(new Field("issue_per_company", bean.IssueIdPerCompany.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                if (bean.PriorityId != -1) doc.Add(new Field("priority_id", bean.PriorityId.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                if (bean.StatusId != -1) doc.Add(new Field("status_id", bean.StatusId.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                if (!string.IsNullOrEmpty(bean.CurrentOwner)) doc.Add(new Field("curr_owner", bean.CurrentOwner, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                if (!string.IsNullOrEmpty(bean.LastOwner)) doc.Add(new Field("last_owner", bean.LastOwner, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                if (!string.IsNullOrEmpty(bean.ProjectName)) doc.Add(new Field("project_name", bean.ProjectName, Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));

                if (bean.AssignedCUDate != DateTime.MinValue) doc.Add(new Field("assigned_cu_date", DateTools.DateToString(bean.AssignedCUDate, DateTools.Resolution.DAY), Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
                if (bean.DestinationDate != DateTime.MinValue) doc.Add(new Field("destination_date", DateTools.DateToString(bean.DestinationDate, DateTools.Resolution.DAY), Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
                if (!string.IsNullOrEmpty(bean.DescLast)) doc.Add(new Field("desc_last", bean.DescLast, Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
                if (bean.ReadUsersBitIds1 != -1) doc.Add(new Field("read_usr_bit1", bean.ReadUsersBitIds1.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                if (bean.ReadUsersBitIds2 != -1) doc.Add(new Field("read_usr_bit1", bean.ReadUsersBitIds2.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                if (bean.LastUpdate != DateTime.MinValue) doc.Add(new Field("last_update", DateTools.DateToString(bean.LastUpdate, DateTools.Resolution.DAY), Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
                if (bean.StartDate != DateTime.MinValue) doc.Add(new Field("start_date", DateTools.DateToString(bean.StartDate, DateTools.Resolution.DAY), Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
                if (bean.IsAllDay) doc.Add(new Field("is_all_day", "1", Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                else doc.Add(new Field("is_all_day", "0", Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
                if (bean.DestReminderDate != DateTime.MinValue) doc.Add(new Field("dest_rem_date", DateTools.DateToString(bean.DestReminderDate, DateTools.Resolution.DAY), Lucene.Net.Documents.Field.Store.NO, Lucene.Net.Documents.Field.Index.ANALYZED));
                if (bean.ReccurenceId != -1) doc.Add(new Field("reccurence_id", bean.ReccurenceId.ToString(), Lucene.Net.Documents.Field.Store.YES, Lucene.Net.Documents.Field.Index.NO));
         

                idxModif.AddDocument(doc);
                idxModif.Optimize();
                idxModif.Commit();

                numIndexed = idxModif.MaxDoc();
                idxModif.Dispose();

            }
            catch
            {
                if (writer != null)
                {
                    idxModif.Optimize();
                    idxModif.Commit();
                    idxModif.Dispose();
                }
                throw;
            }
            return numIndexed;
        }

        /// <summary>
        /// Perform search
        /// </summary>
        /// <param name="query"></param>
        /// <param name="startIndex"></param>
        /// <param name="blockSize"></param>
        /// <param name="indexDirEs"></param>
        /// <param name="indexDirEn"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public List<IssueDocument> MedesSearch(Query query, int startIndex, int blockSize, Directory indexDirEs, Directory indexDirEn, Directory indexDirHe, string sortBy)
        {
#if DEBUG
            T.TraceMessage(string.Format("Begin search , query: '{0}'", query.ToString()));
#endif

            List<IssueDocument> result = new List<IssueDocument>();
            try
            {
                // build a multi searcher across the 2 indexes
                MultiSearcher mSearcher = CombineSearchers(indexDirEs, indexDirEn, indexDirHe);

                TopDocs tDocs = null;
                int iterateLast = startIndex + blockSize;


                string customScoreField = "article_id";
                FieldScoreQuery dateBooster = new FieldScoreQuery(customScoreField, FieldScoreQuery.Type.FLOAT);
                CustomScoreQuery customQuery = new CustomScoreQuery(query, dateBooster);

                tDocs = mSearcher.Search(customQuery, 1000);
                //ScoreDoc[] hits = tpDcs.scoreDocs;
                if (startIndex + blockSize > tDocs.TotalHits) iterateLast = tDocs.TotalHits;

                for (int i = startIndex; i < iterateLast; i++)
                {
                    // Document hitDoc = mSearcher.Doc(hits[i].doc);

                    Document hitDoc = mSearcher.Doc(i);

                    result.Add(new IssueDocument() { Id = Int32.Parse(hitDoc.Get("issue_id").ToString())});
                }

                // close the searcher and indexes
                mSearcher.Dispose();
                indexDirEs.Dispose();
                indexDirEn.Dispose();
                indexDirHe.Dispose();
            }
            catch (Exception ex)
            {
                T.TraceError("Error MedesSearch, query '{0}'", query.ToString());
                T.TraceError(ex);
                throw ex;
            }
            return result;
        }

        private MultiSearcher CombineSearchers(Directory indexDirEs, Directory indexDirEn, Directory indexDirHe)
        {
            IndexSearcher iSearcherEs = new IndexSearcher(indexDirEs, true); // read-only=true
            IndexSearcher iSearcherEn = new IndexSearcher(indexDirEn, true); // read-only=true
            IndexSearcher iSearcherHe = new IndexSearcher(indexDirHe, true); // read-only=true


            Searcher[] listSearchers = new Searcher[3];
            listSearchers[0] = iSearcherEs;
            listSearchers[1] = iSearcherEn;
            listSearchers[2] = iSearcherHe;
            MultiSearcher mSearcher = new MultiSearcher(listSearchers);

            return mSearcher;
        }
    }
}