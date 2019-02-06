using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingGraph
{
    [Serializable]
    public class Stats
    {
        /// <summary>
        /// Stats data
        /// </summary>
        public Tuple<ushort, Tuple<ushort, DateTime>, Tuple<ushort, DateTime>> StatsData { get; private set; }

        /// <summary>
        /// Ping treshold in milliseconds
        /// </summary>
        public int RoundTripTreshold { get; private set; }

        /// <summary>
        /// Requests over treshold
        /// </summary>
        public int OverTreshold { get; private set; }

        /// <summary>
        /// Failed Requests
        /// </summary>
        public int FailedRequests { get; private set; }

        /// <summary>
        /// Total requests
        /// </summary>
        public int TotalRequests { get; private set; }
                
        /// <summary>
        /// C'tor
        /// </summary>
        public Stats( ushort average, Tuple<ushort, DateTime> min, Tuple<ushort, DateTime> max )
            :this( average, min, max, 0, 0, 0, 0 )
        { }
        
        /// <summary>
        /// C'tor
        /// </summary>
        /// <param name="average"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="roundtripTreshold"></param>
        /// <param name="overTreshold"></param>
        /// <param name="failedRequests"></param>
        public Stats( ushort average, Tuple<ushort, DateTime> min, Tuple<ushort, DateTime> max, 
            int roundtripTreshold, int overTreshold, int failedRequests, int totalRequests )
        {
            StatsData = new Tuple<ushort, Tuple<ushort, DateTime>, Tuple<ushort, DateTime>>(
                average, min, max );

            RoundTripTreshold = roundtripTreshold;
            OverTreshold = overTreshold;
            FailedRequests = failedRequests;
            TotalRequests = totalRequests;
        }
    }
}
