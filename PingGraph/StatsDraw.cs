using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingGraph
{
    public class StatsDraw
    {
        /// <summary>
        /// Stats data
        /// </summary>
        private Stats _statsData = null;

        /// <summary>
        /// Picture box
        /// </summary>
        PictureBox _pictureBox = null;

        /// <summary>
        /// Object used for synchronization
        /// </summary>
        public static object _locker = new object();

        private int _verticalLineHeight = 45;
        private int _indicatorFontSize = 10;
        private int _valueFontSize = 36;
        private int _valueFontSizeBigger = 52;
        private string _indicatorNameFont = "Arial";
        private string _indicatorValueFont = "Calibri";
        private int _horizontalDistanceBetweenSections = 116;
        private int _verticalDistanceSecondsRowSections = 74;

        /// <summary>
        /// C'tor
        /// </summary>
        public StatsDraw( Stats stats, PictureBox pictureBox )
        {
            _statsData = stats;
            _pictureBox = pictureBox;
        }

        /// <summary>
        /// Draw stats
        /// </summary>
        public void Draw()
        {
            bool ok = Monitor.TryEnter( _locker, TimeSpan.FromMilliseconds( 100 ) );
            ok = true;
            if ( ok )
            {
                {
                    Bitmap bmp = new Bitmap( _pictureBox.Width, _pictureBox.Height );
                    _pictureBox.BackColor = Color.White;

                    int msPositionX = 0;
                    int val, valLen;

                    using ( Graphics g = Graphics.FromImage( bmp ) )
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                        // Total requests
                        Point ptTotalRequests = new Point( 12, 12 );
                        Pen pen = new Pen( Color.Green, 4f );
                        g.DrawLine( pen, ptTotalRequests.X, ptTotalRequests.Y, ptTotalRequests.X, ptTotalRequests.Y + _verticalLineHeight );

                        g.DrawString( $"TOTAL REQUESTS", 
                            new Font( _indicatorNameFont, _indicatorFontSize, FontStyle.Bold, GraphicsUnit.Pixel ), 
                            Brushes.Black, (float)(ptTotalRequests.X + 4 ), (float) ptTotalRequests.Y );

                        g.DrawString( $"{ _statsData.TotalRequests.ToString() }",
                            new Font( _indicatorValueFont, _valueFontSize, FontStyle.Bold, GraphicsUnit.Pixel ), 
                            Brushes.Green, ( float ) ( ptTotalRequests.X + 4 ), ( float ) (ptTotalRequests.Y + 6) );

                        // Failed requests
                        Point ptFailedRequests = new Point( ptTotalRequests.X + _horizontalDistanceBetweenSections, 12 );
                        Pen pen2 = new Pen( Color.Magenta, 4f );
                        g.DrawLine( pen2, ptFailedRequests.X, ptFailedRequests.Y, ptFailedRequests.X, ptFailedRequests.Y + _verticalLineHeight );

                        g.DrawString( $"FAILED REQUESTS",
                            new Font( _indicatorNameFont, _indicatorFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                            Brushes.Black, ( float ) ( ptFailedRequests.X + 4 ), ( float ) ptFailedRequests.Y );

                        g.DrawString( $"{ _statsData.FailedRequests.ToString() }",
                            new Font( _indicatorValueFont, _valueFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                            _statsData.FailedRequests > 0 ? Brushes.Magenta : Brushes.Black, 
                            ( float ) ( ptFailedRequests.X + 4 ), ( float ) ( ptFailedRequests.Y + 6 ) );

                        // Treshold
                        Point ptTreshold = new Point( ptFailedRequests.X + _horizontalDistanceBetweenSections, 12 );
                        Pen pen3 = new Pen( Color.Black, 4f );
                        g.DrawLine( pen3, ptTreshold.X, ptTreshold.Y, ptTreshold.X, ptTreshold.Y + _verticalLineHeight );

                        g.DrawString( $"ICMP TRESHOLD",
                            new Font( _indicatorNameFont, _indicatorFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                            Brushes.Black, ( float ) ( ptTreshold.X + 4 ), ( float ) ptTreshold.Y );

                        g.DrawString( $"{ _statsData.RoundTripTreshold.ToString() }",
                            new Font( _indicatorValueFont, _valueFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                            Brushes.DarkGray, ( float ) ( ptTreshold.X + 4 ), ( float ) ( ptTreshold.Y + 6 ) );

                        val = _statsData.RoundTripTreshold;
                        valLen = val.ToString().Length;
                        msPositionX = ptTreshold.X + ( valLen * 23 );

                        g.DrawString( $"ms",
                            new Font( _indicatorValueFont, (int)Math.Round( ((double)_valueFontSize / 3), 0 ), FontStyle.Bold, GraphicsUnit.Pixel ),
                            Brushes.DarkGray, ( float ) msPositionX, ( float ) ( ptTreshold.Y + 28 ) );

                        // Above treshold
                        Point ptAboveTreshold = new Point( ptTreshold.X + _horizontalDistanceBetweenSections, 12 );
                        Pen pen13 = new Pen( Color.DarkGoldenrod, 4f );
                        g.DrawLine( pen13, ptTreshold.X + _horizontalDistanceBetweenSections, ptTreshold.Y +2, 
                            ptTreshold.X + _horizontalDistanceBetweenSections + 120, ptTreshold.Y + 2 );

                        g.DrawString( $"OVER TRESHOLD", new Font( _indicatorNameFont, _indicatorFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                            Brushes.Black, ( float ) ( ptAboveTreshold.X + 14 ), ( float ) ptAboveTreshold.Y + 8 );

                        Font aboveTresholdFont = new Font( _indicatorValueFont, _valueFontSizeBigger, FontStyle.Bold, GraphicsUnit.Pixel );

                        float fontSizeInPixels = aboveTresholdFont.SizeInPoints / 72 * g.DpiX;
                        float aboveTresholdX = ptAboveTreshold.X + (( 120 - fontSizeInPixels ) / 2);

                        g.DrawString( $"{ _statsData.OverTreshold.ToString() }", aboveTresholdFont,
                            Brushes.DarkGoldenrod, aboveTresholdX, ( float ) ( ptAboveTreshold.Y + 12 ) );

                        // Average roundtrip
                        Point ptAverage = new Point( 12, _verticalDistanceSecondsRowSections );
                        Pen pen4 = new Pen( Color.Black, 4f );
                        g.DrawLine( pen4, ptAverage.X, ptAverage.Y, ptAverage.X, ptAverage.Y + _verticalLineHeight );

                        g.DrawString( $"AVG ROUNDTRIP",
                            new Font( _indicatorNameFont, _indicatorFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                            Brushes.Black, ( float ) ( ptAverage.X + 4 ), ( float ) ptAverage.Y );
                        
                        g.DrawString( $"{ _statsData.StatsData.Item1.ToString() }",
                            new Font( _indicatorValueFont, _valueFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                            Brushes.Black, ( float ) ( ptAverage.X + 4 ), ( float ) ( ptAverage.Y + 6 ) );

                        val = _statsData.StatsData.Item1;
                        valLen = val.ToString().Length;
                        msPositionX = ptAverage.X + (valLen * 23 );
                        g.DrawString( $"ms",
                           new Font( _indicatorValueFont, ( int ) Math.Round( ( ( double ) _valueFontSize / 3 ), 0 ), FontStyle.Bold, GraphicsUnit.Pixel ),
                           Brushes.Black, ( float ) msPositionX, ( float ) ( ptAverage.Y + 28 ) );

                        // MIN roundtrip
                        Point ptMinRoundtrip = new Point( 12 + _horizontalDistanceBetweenSections, _verticalDistanceSecondsRowSections );
                        Pen pen5 = new Pen( Color.CadetBlue, 4f );
                        g.DrawLine( pen5, ptMinRoundtrip.X, ptMinRoundtrip.Y, ptMinRoundtrip.X, ptMinRoundtrip.Y + _verticalLineHeight );

                        g.DrawString( $"MIN ROUNDTRIP",
                            new Font( _indicatorNameFont, _indicatorFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                            Brushes.Black, ( float ) ( ptMinRoundtrip.X + 4 ), ( float ) ptMinRoundtrip.Y );

                        g.DrawString( $"{ _statsData.StatsData.Item2.Item1.ToString() }",
                            new Font( _indicatorValueFont, _valueFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                            Brushes.CadetBlue, ( float ) ( ptMinRoundtrip.X + 4 ), ( float ) ( ptMinRoundtrip.Y + 6 ) );

                        val = _statsData.StatsData.Item2.Item1;
                        valLen = val.ToString().Length;
                        msPositionX = ptMinRoundtrip.X + ( valLen * 23 );
                        g.DrawString( $"ms",
                           new Font( _indicatorValueFont, ( int ) Math.Round( ( ( double ) _valueFontSize / 3 ), 0 ), FontStyle.Bold, GraphicsUnit.Pixel ),
                           Brushes.CadetBlue, ( float ) msPositionX, ( float ) ( ptAverage.Y + 28 ) );

                        // MAX roundtrip
                        Point ptMaxRoundtrip = new Point( ptMinRoundtrip.X + _horizontalDistanceBetweenSections, _verticalDistanceSecondsRowSections );
                        Pen pen6 = new Pen( Color.Red, 4f );
                        g.DrawLine( pen6, ptMaxRoundtrip.X, ptMaxRoundtrip.Y, ptMaxRoundtrip.X, ptMaxRoundtrip.Y + _verticalLineHeight );

                        g.DrawString( $"MAX ROUNDTRIP",
                            new Font( _indicatorNameFont, _indicatorFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                            Brushes.Black, ( float ) ( ptMaxRoundtrip.X + 4 ), ( float ) ptMaxRoundtrip.Y );

                        g.DrawString( $"{ _statsData.StatsData.Item3.Item1.ToString() }",
                            new Font( _indicatorValueFont, _valueFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                            Brushes.Red, ( float ) ( ptMaxRoundtrip.X + 4 ), ( float ) ( ptMaxRoundtrip.Y + 6 ) );

                        val = _statsData.StatsData.Item3.Item1;
                        valLen = val.ToString().Length;
                        msPositionX = ptMaxRoundtrip.X + ( valLen * 23 );
                        g.DrawString( $"ms",
                           new Font( _indicatorValueFont, ( int ) Math.Round( ( ( double ) _valueFontSize / 3 ), 0 ), FontStyle.Bold, GraphicsUnit.Pixel ),
                           Brushes.Red, ( float ) msPositionX, ( float ) ( ptMaxRoundtrip.Y + 28 ) );
                    }

                    _pictureBox.Image = bmp;
                }
            }
            else
                Debug.WriteLine( "Could not acquire lock to drawn on canvas." );
        }

        /// <summary>
        /// Clear the stats
        /// </summary>
        public void Clear()
        {
            Bitmap bmp = new Bitmap( _pictureBox.Width, _pictureBox.Height );
            _pictureBox.Image = bmp;

            _pictureBox.BackColor = Color.Transparent;
        }
    }
}
