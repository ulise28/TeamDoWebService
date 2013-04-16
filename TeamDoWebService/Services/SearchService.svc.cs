using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using TeamDoWebService.Interfaces;
using TeamDoWebService.Contracts;
using TeamDoWebService.Code.Controllers;
using T = TeamDoWebService.Code.Controllers.Trace;

namespace TeamDoWebService.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class SearchService : ISearchService
    {
        public bool UpdateIndex(int taskId)
        {
            if (ConfigurationController.RunSpanishIndexing)
            {
                T.TraceMessage("Updating ES index...");
                LuceneController.UpdateIndex("ES");
                T.TraceMessage(" done updating ES index");
                T.TraceMessage("Moving ES index...");
                if (!string.IsNullOrEmpty(ConfigurationController.IndexRootPath)) LuceneController.MoveIndex(false, "ES", 1);
                T.TraceMessage(" done moving ES index");
            }

            if (ConfigurationController.RunEnglishIndexing)
            {
                T.TraceMessage("Updating EN index...");
                LuceneController.UpdateIndex("EN");
                T.TraceMessage(" done updating EN index");
                T.TraceMessage("Moving EN index...");
                if (!string.IsNullOrEmpty(ConfigurationController.IndexRootPath)) LuceneController.MoveIndex(false, "EN", 1);
                T.TraceMessage(" done moving EN index");
            }

            if (ConfigurationController.RunHebrewIndex)
            {
                T.TraceMessage("Updating HE index...");
                LuceneController.UpdateIndex("HE");
                T.TraceMessage(" done updating HE index");
                T.TraceMessage("Moving HE index...");
                if (!string.IsNullOrEmpty(ConfigurationController.IndexRootPath)) LuceneController.MoveIndex(false, "HE", 1);
                T.TraceMessage(" done moving HE index");
            }

            return true;
        }

        public List<IssueDocument> SearchText(string searchString, string sort, int pageSize, int pageNumber)
        {
            List<IssueDocument> result;

            DateTime ini = DateTime.Now;

            // get the lucene query using the lucene parsing capabilities
            // Query luceneQuery = LuceneController.GetLuceneQuery(searchString);

            // perform Lucene Search algorithm
            result = LuceneController.DoSearch(searchString, (pageNumber - 1) * pageSize, pageSize, sort);


            return result;
        }
    }
}
