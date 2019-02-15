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
        /// Picture box
        /// </summary>
        private PictureBox _pictureBox = null;

        /// <summary>
        /// Sections to draw
        /// </summary>
        private Section[] _sections = null;

        /// <summary>
        /// Object used for synchronization
        /// </summary>
        public static object _locker = new object();

        /// <summary>
        /// Above treshold
        /// </summary>
        private int _aboveTreshold = -1;

        /// <summary>
        /// Above treshold X position
        /// </summary>
        private float _aboveTresholdX = 0;

        private int _verticalLineHeight = 45;
        private int _indicatorFontSize = 10;
        private int _valueFontSize = 36;
        private int _valueFontSizeBigger = 52;
        private string _indicatorNameFont = "Arial";
        private string _indicatorValueFont = "Calibri";
        private int _horizontalDistanceBetweenSections = 116;
        private int _verticalDistanceSecondsRowSections = 74;
        private int _paddingSection = 12, _paddingSectionVertical = 16;

        /// <summary>
        /// C'tor
        /// </summary>
        public StatsDraw( PictureBox pictureBox, Func<Section[]> sectionBuilder )
        {
            if ( pictureBox == null || sectionBuilder == null )
                throw new ArgumentNullException();

            _pictureBox = pictureBox;
            _sections = sectionBuilder.Invoke();
        }

        /// <summary>
        /// Draw stats
        /// </summary>
        public void Draw()
        {
            if ( _sections == null )
                return;

            bool ok = Monitor.TryEnter( _locker, TimeSpan.FromMilliseconds( 100 ) );
            ok = true;

            if ( ok )
            {
                Bitmap bmp = new Bitmap( _pictureBox.Width, _pictureBox.Height );
                _pictureBox.BackColor = Color.White;

                int msPositionX = 0;
                int valLen = 0, rightEdge = 0;

                using ( Graphics g = Graphics.FromImage( bmp ) )
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                    for ( int i = 0; i < 6; i++ )
                    {
                        // Where to draw?
                        Point toDraw = new Point( _paddingSection + ( ( ( i < 3 ) ? i : i - 3 ) * _horizontalDistanceBetweenSections ),
                            i < 3 ? _paddingSectionVertical : _paddingSectionVertical + _verticalDistanceSecondsRowSections );

                        if ( i == 2 )
                            rightEdge = toDraw.X + 4 + _horizontalDistanceBetweenSections;

                        // Draw line
                        g.DrawLine( _sections[ i ].SectionLinePen, toDraw.X, toDraw.Y, toDraw.X, toDraw.Y + _verticalLineHeight );

                        // Section name
                        g.DrawString( _sections[ i ].SectionTitle,
                            new Font( _indicatorNameFont, _indicatorFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                            Brushes.Black, ( float )( toDraw.X + 4 ), ( float )toDraw.Y );

                        // Section value
                        g.DrawString( $"{ _sections[ i ].SectionValue.Item1 }",
                            new Font( _indicatorValueFont, _valueFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                            _sections[ i ].SectionBrush, ( float )( toDraw.X + 4 ), ( float )( toDraw.Y + 6 ) );

                        // Draw 'ms' to the right of the value
                        if ( _sections[ i ].SectionValue.Item2.Length > 0 )
                        {
                            valLen = _sections[ i ].SectionValue.Item1.Length;
                            msPositionX = toDraw.X + ( valLen * 23 );

                            g.DrawString( _sections[ i ].SectionValue.Item2,
                                new Font( _indicatorValueFont, ( int )Math.Round( ( ( double )_valueFontSize / 3 ), 0 ), FontStyle.Bold, GraphicsUnit.Pixel ),
                                _sections[ i ].SectionBrush, ( float )msPositionX, ( float )( toDraw.Y + 28 ) );
                        }
                    }

                    // Draw the total over treshold
                    Point aboveTresholdPos = new Point( rightEdge, _paddingSectionVertical );

                    Pen pen = new Pen( Color.DarkGoldenrod, 4f );
                    g.DrawLine( pen, aboveTresholdPos.X, aboveTresholdPos.Y + 2,
                        aboveTresholdPos.X + _horizontalDistanceBetweenSections, aboveTresholdPos.Y + 2 );

                    g.DrawString( $"OVER TRESHOLD", new Font( _indicatorNameFont, _indicatorFontSize, FontStyle.Bold, GraphicsUnit.Pixel ),
                        Brushes.Black, ( float )( aboveTresholdPos.X + 14 ), ( float )aboveTresholdPos.Y + 8 );

                    Font aboveTresholdFont = new Font( _indicatorValueFont, _valueFontSizeBigger, FontStyle.Bold, GraphicsUnit.Pixel );

                    // Only recalculate the position if it has changed
                    if ( _sections[ 0 ].SectionStats.OverTreshold > _aboveTreshold )
                    {
                        _aboveTreshold = _sections[ 0 ].SectionStats.OverTreshold;
                        _aboveTresholdX = aboveTresholdPos.X + (
                            ( _horizontalDistanceBetweenSections - GetStringWidth( g, _aboveTreshold.ToString(), aboveTresholdFont ) ) / 2 );
                    }

                    g.DrawString( _aboveTreshold.ToString(), aboveTresholdFont, Brushes.DarkGoldenrod, 
                        _aboveTresholdX, ( float )( aboveTresholdPos.Y + 12 ) );
                }

                _pictureBox.Image = bmp;
            }
            else
                Debug.WriteLine( "Could not acquire lock to drawn on canvas." );
        }

        /// <summary>
        /// Calculate the width of the string in pixels
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="value"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        private int GetStringWidth( Graphics graphics, string value, Font font )
        {
            StringFormat format = new System.Drawing.StringFormat();
            RectangleF rect = new System.Drawing.RectangleF( 0, 0, 250, 120 );
            CharacterRange[] ranges = { new System.Drawing.CharacterRange( 0, value.Length ) };
            Region[] regions = new System.Drawing.Region[ 1 ];

            format.SetMeasurableCharacterRanges( ranges );
            regions = graphics.MeasureCharacterRanges( value, font, rect, format );
            rect = regions[ 0 ].GetBounds( graphics );

            return ( int )( rect.Right + 1.0f );
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
