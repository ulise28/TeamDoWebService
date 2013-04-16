using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TeamDoWebService.Code.Dto
{
    public class IssueDocumentDto
    {
        public int Id { get; set; }

        public int IssueId { get; set; }

        public int CompanyId { get; set; }

        public int IssueIdPerCompany { get; set; }

        public string IssueTitle { get; set; }

        public int PriorityId { get; set; }

        public int StatusId { get; set; }

        public string CurrentOwner { get; set; }

        public string LastOwner { get; set; }

        public string ProjectName { get; set; }

        public DateTime AssignedCUDate { get; set; }

        public DateTime DestinationDate { get; set; }

        public string DescLast { get; set; }

        public Int64 ReadUsersBitIds1 { get; set; }

        public Int64 ReadUsersBitIds2 { get; set; }

        public DateTime LastUpdate { get; set; }

        public DateTime StartDate { get; set; }

        public bool IsAllDay { get; set; }

        public DateTime DestReminderDate { get; set; }

        public int ReccurenceId { get; set; }


    }
}