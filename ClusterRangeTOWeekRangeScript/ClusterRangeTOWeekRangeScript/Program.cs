using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterRangeTOWeekRangeScript
{
    class Program
    {
        static List<DateRange> Clusters = new List<DateRange>();

        static List<ClusterDetail> ClusterDetailsList = new List<ClusterDetail>();

        static string ConnectionStringNew = @"Data Source=.\SQLEXPRESS;Initial Catalog=GMEET;User ID=sa;Password=data"; // Do not change here
        static string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Rootlearn;User ID=sa;Password=data";


        static void Main(string[] args)
        {
            GetClusterDetails();


            TableScript(Clusters, "ClustersWeekRange");


        }

        static public void GetClusterDetails()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStringNew))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    con.Open();

                    cmd.CommandText = "select Cluster, StartDate, EndDate,Status from ClusterDetail";
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ClusterDetail ob = new ClusterDetail();
                        DateRange dr = new DateRange();

                        ob.Cluster = int.Parse(reader.GetString(0));

                        ob.StartDate = reader.GetDateTime(1);
                        dr.StartDate = reader.GetDateTime(1);
                        ob.EndDate = reader.GetDateTime(2);
                        dr.EndDate = reader.GetDateTime(2);

                        if (reader.GetString(3).ToUpper() == "Current".ToUpper())
                            ob.IsActive = true;
                        else
                            ob.IsActive = false;

                        ClusterDetailsList.Add(ob);
                        Clusters.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        static public void TableScript(List<DateRange> ClustersList, string tableName)
        {
            bool tableExist = false;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    con.Open();

                    cmd.CommandText = "SELECT COUNT(TABLE_CATALOG) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'ClustersWeekRange'";
                    var res = cmd.ExecuteScalar();

                    if (int.Parse(res.ToString()) > 0)
                        tableExist = true;
                }
            }
            catch (Exception ex)
            {

            }

            if (tableExist)
            {
                InsertIntoTable(ClustersList);
            }
            else
            {
                CreateTable(tableName);
                InsertIntoTable(ClustersList);
            }
        }

        static public void CreateTable(string tableName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    con.Open();

                    cmd.CommandText = "create table " + tableName + "  (cluster varchar(10) NOT NULL, weeks varchar(10) NOT NULL, startDate datetime NOT NULL, endDate datetime NOT NULL, PRIMARY KEY (cluster,weeks))";
                    cmd.ExecuteScalar();
                    con.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        static public void InsertIntoTable(List<DateRange> ClustersList)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    con.Open();

                    for (int i = 0; i < ClustersList.Count(); i++)
                    {
                        List<DateRange> splited = SplitAsWeek(ClustersList[i]);
                        for (int k = 0; k < splited.Count(); k++)
                        {
                            cmd.CommandText = "insert into ClustersWeekRange (cluster,weeks,startDate,endDate) values ('Cluster" + (i + 1) + "','Week" + (k + 1) + "','" + splited[k].StartDate.ToString("yyyy-MM-dd") + "','" + splited[k].EndDate.ToString("yyyy-MM-dd") + "')";
                            cmd.ExecuteScalar();
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        static public List<DateRange> SplitAsWeek(DateRange givenRange)
        {
            List<DateRange> weekList = new List<DateRange>();
            DateTime startOfWeek = givenRange.StartDate;
            DateTime endOfWeek = GetEndOfWeek(givenRange.StartDate);
            weekList.Add(new DateRange(startOfWeek, endOfWeek));
            while (true)
            {
                if (endOfWeek >= givenRange.EndDate)
                {
                    break;
                }
                if (startOfWeek <= givenRange.EndDate)
                {
                    startOfWeek = endOfWeek.AddDays(1);
                    if (GetEndOfWeek(startOfWeek) >= givenRange.EndDate)
                    {
                        endOfWeek = givenRange.EndDate;
                    }
                    else
                    {
                        endOfWeek = GetEndOfWeek(startOfWeek);
                    }
                    weekList.Add(new DateRange(startOfWeek, endOfWeek));
                }
            }
            return weekList;
        }

        static public DateTime GetEndOfWeek(DateTime givenDate)
        {
            while (givenDate.DayOfWeek.ToString().ToUpper() != "wednesday".ToUpper())
            {
                givenDate = givenDate.AddDays(1);
            }

            return givenDate;
        }
    }

    public class AttendanceRecord
    {
        public string Grade { get; set; }
        public string Subject { get; set; }
        public DateTime EventDate { get; set; }

        public AttendanceRecord(string grade, string subject, DateTime eventDate)
        {
            this.Grade = grade;
            this.Subject = subject;
            this.EventDate = eventDate;
        }

        public AttendanceRecord()
        {

        }
    }
}
