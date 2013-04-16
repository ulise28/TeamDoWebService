using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TeamDoWebService.Contracts;

using T = TeamDoWebService.Code.Controllers.Trace;
using TeamDoWebService.Code.Dto;
using TeamDoWebService.Code.Controllers;
using System.Data.SqlClient;

namespace TeamDoWebService.Code.Dao
{
    public class IssuesDao
    {
        private const string CLASS_NAME = " TeamDoWebService.Code.Dao.IssuesDao";

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="context"></param>
        public IssuesDao()
        {

        }

        /// <summary>
        /// Get Total Publication by Language
        /// </summary>
        /// <param name="lng"></param>
        /// <returns></returns>
        public int GetTotalPublicationByLanguage(string lng)
        {
            int result = 2010;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConfigurationController.MainConnectionString))
                {
                    SqlCommand command = new SqlCommand("SW_BP_Issues_Count", sqlConn);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    sqlConn.Open();
                    result = Int32.Parse(command.ExecuteScalar().ToString());

                    sqlConn.Close();
                }

            }
            catch (Exception ex)
            {
                T.TraceError("Error GetTotalPublicationByLanguage", CLASS_NAME);
                T.TraceError(ex);
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Get publication list
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        public Dictionary<string, IssueDocumentDto> GetIssuesIndexList(int start, int end, string lng)
        {
            Dictionary<string, IssueDocumentDto> issuesList = new Dictionary<string, IssueDocumentDto>();
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConfigurationController.MainConnectionString))
                {
                    SqlCommand command = new SqlCommand("SP_BW_ISSUES_SEARCH", sqlConn);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@StartRowIndex", start);
                    command.Parameters.AddWithValue("@EndRowIndex", end);

                    sqlConn.Open();
                    SqlDataReader dr = command.ExecuteReader();

                    IssueDocumentDto obj = null;

                    while (dr.Read())
                    {
                        obj = new IssueDocumentDto();
                        obj.Id = dr.GetInt32(dr.GetOrdinal("Issue_ID"));

                        if (!dr.IsDBNull(dr.GetOrdinal("Issue_Company_ID")))
                            obj.CompanyId = dr.GetInt32(dr.GetOrdinal("Issue_Company_ID"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Issue_ID_Per_Company")))
                            obj.IssueIdPerCompany = dr.GetInt32(dr.GetOrdinal("Issue_ID_Per_Company"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Issue_Title")))
                            obj.IssueTitle = dr.GetString(dr.GetOrdinal("Issue_Title"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Priority_ID")))
                            obj.PriorityId = dr.GetInt32(dr.GetOrdinal("Priority_ID"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Status_ID")))
                            obj.StatusId = dr.GetInt32(dr.GetOrdinal("Status_ID"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Cur_Owner")))
                            obj.CurrentOwner = dr.GetString(dr.GetOrdinal("Cur_Owner"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Last_Owner")))
                            obj.LastOwner = dr.GetString(dr.GetOrdinal("Last_Owner"));

                        if (!dr.IsDBNull(dr.GetOrdinal("Project_Name")))
                            obj.ProjectName = dr.GetString(dr.GetOrdinal("Project_Name"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Assigned_CurUser_Date")))
                            obj.AssignedCUDate = dr.GetDateTime(dr.GetOrdinal("Assigned_CurUser_Date"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Destination_Date")))
                            obj.DestinationDate = dr.GetDateTime(dr.GetOrdinal("Destination_Date"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Desc_Last")))
                            obj.DescLast = dr.GetString(dr.GetOrdinal("Desc_Last"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Read_Users_BitIDs1")))
                            obj.ReadUsersBitIds1 = dr.GetInt64(dr.GetOrdinal("Read_Users_BitIDs1"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Read_Users_BitIDs2")))
                            obj.ReadUsersBitIds2 = dr.GetInt64(dr.GetOrdinal("Read_Users_BitIDs2"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Last_Update_Date")))
                            obj.LastUpdate = dr.GetDateTime(dr.GetOrdinal("Last_Update_Date"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Start_Date")))
                            obj.StartDate = dr.GetDateTime(dr.GetOrdinal("Start_Date"));
                        if (!dr.IsDBNull(dr.GetOrdinal("IsAllDay")))
                            obj.IsAllDay = dr.GetBoolean(dr.GetOrdinal("IsAllDay"));
                        if (!dr.IsDBNull(dr.GetOrdinal("Destination_Reminder_Date")))
                            obj.DestReminderDate = dr.GetDateTime(dr.GetOrdinal("Destination_Reminder_Date"));
                        if (!dr.IsDBNull(dr.GetOrdinal("RecurrenceId")))
                            obj.ReccurenceId = dr.GetInt32(dr.GetOrdinal("RecurrenceId"));

                        issuesList.Add("I" + obj.Id.ToString(), obj);
                    }

                    sqlConn.Close();
                }

            }
            catch (Exception ex)
            {
                T.TraceError("Error GetPublicationIndexList CN: {0}, start: {1}, end: {2}, language {3}", CLASS_NAME, start, end, lng);
                T.TraceError(ex);
                throw ex;
            }

            return issuesList;
        }

    }
}