using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TeamDoWebService.Contracts;

namespace TeamDoWebService.Interfaces
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ISearchService
    {

        [OperationContract]
        bool UpdateIndex(int taskId);

        [OperationContract]
        List<IssueDocument> SearchText(string searchString, string sort, int pageSize, int pageNumber);

    }


  
}
