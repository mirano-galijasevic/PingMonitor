using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingGraph
{
    public class SectionBuilder
    {
        /// <summary>
        /// Stats
        /// </summary>
        private Stats _stats;

        /// <summary>
        /// Sections names
        /// </summary>
        private string[] _sectionNames;

        /// <summary>
        /// Pens
        /// </summary>
        private Pen[] _pens = null;

        /// <summary>
        /// Brushes
        /// </summary>
        private Brush[] _brushes = null;

        /// <summary>
        /// C'tor
        /// </summary>
        public SectionBuilder( Stats stats, string[] sectionNames )
        {
            _stats = stats;
            _sectionNames = sectionNames;

            _pens = new Pen[]
                {
                    new Pen( Color.Green, 4f ),
                    new Pen( Color.Magenta, 4f ),
                    new Pen( Color.Black, 4f ),
                    new Pen( Color.Black, 4f ),
                    new Pen( Color.CadetBlue, 4f ),
                    new Pen( Color.Red, 4f )
                };

            _brushes = new Brush[]
                {
                    Brushes.Green,
                    Brushes.Magenta,
                    Brushes.DarkGray,
                    Brushes.Black,
                    Brushes.CadetBlue,
                    Brushes.Red
                };
        }

        /// <summary>
        /// Create sections
        /// </summary>
        /// <param name="sectionNames"></param>
        /// <returns></returns>
        public Section[] Build()
        {
            byte count = 6;
            Section[] sections = new Section[ count ];

            for ( int i = 0; i < 6; i++ )
            {
                Section section = new Section()
                {
                    SectionTitle = _sectionNames[ i ],
                    SectionLinePen = _pens[ i ],
                    SectionBrush = _brushes[ i ],
                    SectionValue = GetSectionValue( i ),
                    SectionStats = _stats
                };

                sections[ i ] = section;
            }

            return sections;
        }

        private Tuple<string, string> GetSectionValue( int i )
        {
            switch ( i )
            {
                case 0: // Total Requests
                    return new Tuple<string, string>( _stats.TotalRequests.ToString(), String.Empty );

                case 1: // Failed Requests
                    return new Tuple<string, string>( _stats.FailedRequests.ToString(), String.Empty );

                case 2: // Roundtrip treshold
                    return new Tuple<string, string>( _stats.RoundTripTreshold.ToString(), "ms" );

                case 3: // Average roundtrip
                    return new Tuple<string, string>( _stats.StatsData.Item1.ToString(), "ms" );

                case 4: // Min roundtrip
                    return new Tuple<string, string>( _stats.StatsData.Item2.Item1.ToString(), "ms" );

                case 5: // Max roundtrip
                    return new Tuple<string, string>( _stats.StatsData.Item3.Item1.ToString(), "ms" );
            }

            return new Tuple<string, string>( String.Empty, String.Empty );
        }
    }
}
