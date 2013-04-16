using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TeamDoWebService.Contracts
{
    [DataContract]
    public class IssueDocument
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int IssueId { get; set; }

        [DataMember]
        public int CompanyId { get; set; }

        [DataMember]
        public int IssueIdPerCompany { get; set; }

        [DataMember]
        public int PriorityId { get; set; }

        [DataMember]
        public int StatusId { get; set; }

        [DataMember]
        public string CurrentOwner { get; set; }

        [DataMember]
        public string LastOwner { get; set; }

        [DataMember]
        public string ProjectName { get; set; }

        [DataMember]
        public DateTime AssignedCUDate { get; set; }

        [DataMember]
        public DateTime DestinationDate { get; set; }

        [DataMember]
        public string DescLast { get; set; }

        [DataMember]
        public Int64 ReadUsersBitIds1 { get; set; }

        [DataMember]
        public Int64 ReadUsersBitIds2 { get; set; }

        [DataMember]
        public DateTime LastUpdate { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public bool IsAllDay { get; set; }

        [DataMember]
        public DateTime DestReminderDate { get; set; }

        [DataMember]
        public int ReccurenceId { get; set; }
    }
}