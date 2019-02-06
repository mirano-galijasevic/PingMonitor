using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingGraph
{
    [Serializable]
    public class Snapshot
    {
        /// <summary>
        /// Snapshot data
        /// </summary>
        public DataTable SnapshotData { get; set; }

        /// <summary>
        /// Stats data
        /// </summary>
        public Stats StatsData { get; set; }

        /// <summary>
        /// Created On
        /// </summary>
        public DateTime CreatedOnUTC { get; private set; }

        /// <summary>
        /// C'tor
        /// </summary>
        public Snapshot()
        {
            CreatedOnUTC = DateTime.Now.ToUniversalTime();
        }
    }
}
