using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterRangeTOWeekRangeScript
{
    public class ClusterDetail
    {
        public int Cluster { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }

        public ClusterDetail(int cluster, DateTime startDate, DateTime endDate, bool status)
        {
            this.Cluster = cluster;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.IsActive = status;
        }
        public ClusterDetail()
        {

        }
    }
}
