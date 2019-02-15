using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingGraph
{
    public class Section
    {
        /// <summary>
        /// Section title
        /// </summary>
        public string SectionTitle { get; set; }

        /// <summary>
        /// Section value to display
        /// </summary>
        public Tuple<string, string> SectionValue { get; set; }

        /// <summary>
        /// Section line pen
        /// </summary>
        public Pen SectionLinePen { get; set; }

        /// <summary>
        /// Section brush
        /// </summary>
        public Brush SectionBrush { get; set; }

        /// <summary>
        /// Stats
        /// </summary>
        public Stats SectionStats = null;
        
        /// <summary>
        /// C'tor
        /// </summary>
        public Section() : this( null, null, null, null, null )
        { }

        /// <summary>
        /// C'tor
        /// </summary>
        public Section( string sectionTitle, Pen pen, Brush brush, Stats stats, Tuple<string, string> sectionValue )
        {
            SectionTitle = sectionTitle;
            SectionLinePen = pen;
            SectionBrush = brush;
            SectionStats = stats;
            SectionValue = sectionValue;
        }
    }
}
