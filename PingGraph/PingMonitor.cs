using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using System.Windows.Forms.DataVisualization.Charting;
using System.Runtime.Serialization.Json;

namespace PingGraph
{
    public partial class PingMonitor : Form
    {
        /// <summary>
        /// Target address
        /// </summary>
        private string _address = null;

        /// <summary>
        /// Stop pinging
        /// </summary>
        private volatile bool _stop = false;

        /// <summary>
        /// Data table as source for the graph
        /// </summary>
        private DataTable _dt = null;

        /// <summary>
        /// Stop firing events for combo box
        /// </summary>
        private bool _updatingCombo = false;

        /// <summary>
        /// Run time
        /// </summary>
        private DateTime _endTime = DateTime.MaxValue;

        /// <summary>
        /// Ping request timeout in milliseconds
        /// </summary>
        private ushort _pingTimeout = 1000;

        /// <summary>
        /// Min and max roundtrip
        /// </summary>
        private Tuple<ushort, DateTime> _minRoundtrip, _maxRoundTrip;

        /// <summary>
        /// All round trips
        /// </summary>
        List<int> _roundTrips = null;

        /// <summary>
        /// Sleep delay between subsequent ping requests, in seconds
        /// </summary>
        private ushort _pingDelay = 2;

        /// <summary>
        /// What roundtrip is considered slow?
        /// </summary>
        private ushort _maxRoundTripTreshold = 30;

        /// <summary>
        /// Ping responses that exceeded treshold
        /// </summary>
        private int _overTreshold = 0;

        /// <summary>
        /// Failed ping requests
        /// </summary>
        private int _failedPingRequests = 0;

        /// <summary>
        /// Total number of ping requests
        /// </summary>
        private int _totalRequests = 0;

        /// <summary>
        /// Snapshot data
        /// </summary>
        private Snapshot _snapshot = null;

        /// <summary>
        /// Stats data
        /// </summary>
        private Tuple<ushort, Tuple<ushort, DateTime>, Tuple<ushort, DateTime>> _statsData = null;

        /// <summary>
        /// Initializing
        /// </summary>
        private bool _initializing = true;

        /// <summary>
        /// Ping options
        /// </summary>
        private PingOptions _options = null;

        /// <summary>
        /// Data to send with ping request, needs to be 32 bytes
        /// </summary>
        string _data = null;

        /// <summary>
        /// Byte array of the above data
        /// </summary>
        byte [] _buffer = null;

        /// <summary>
        /// Object use for synchronization between threads
        /// </summary>
        private static object _locker = new object();

        /// <summary>
        /// Notifications between threads when sending the ping request
        /// </summary>
        private ManualResetEvent _mre = null;

        /// <summary>
        /// Signal used to notify the main loop to stop
        /// </summary>
        private ManualResetEvent _externalMre = null;

        /// <summary>
        /// C'tor
        /// </summary>
        public PingMonitor()
        {
            InitializeComponent();

            InitChart();
            InitChartTypesCombo();
            InitiListView();
            InitValues();

            panel1.Focus();

            _initializing = false;
        }

        private void InitValues()
        {
            _minRoundtrip = new Tuple<ushort, DateTime>( 10000, DateTime.Now );
            _maxRoundTrip = new Tuple<ushort, DateTime>( 0, DateTime.Now ); ;

            _roundTrips = new List<int>();

            _mre = new ManualResetEvent( false );
            _externalMre = new ManualResetEvent( true );

            _address = Properties.Settings.Default [ "address" ].ToString();
            ushort.TryParse( Properties.Settings.Default [ "pingTimeout" ].ToString(), out _pingTimeout );
            ushort.TryParse( Properties.Settings.Default [ "pingDelay" ].ToString(), out _pingDelay );
            ushort.TryParse( Properties.Settings.Default [ "maxRoundTrip" ].ToString(), out _maxRoundTripTreshold );

            numDelay.Value = _pingDelay;
            txtAddress.Text = _address;

            _snapshot = new Snapshot();

            _options = new PingOptions( 64, true ); // max 64 hops through routers, and data packet cannot be fragmented
            _data = "pingpingpingpingpingpingpingping";
            _buffer = Encoding.ASCII.GetBytes( _data );
        }

        /// <summary>
        /// Clean up, after user presses the stop button
        /// </summary>
        private void CleanUp()
        {
            bool acquired = Monitor.TryEnter( _locker, 50 );
            if ( acquired )
            {
                _dt.Clear();

                if ( chart.InvokeRequired )
                {
                    chart.Invoke( ( MethodInvoker ) delegate
                    {
                        chart.DataBind();
                    } );
                }
                else
                    chart.DataBind();

                Monitor.Exit( _locker );
            }
            else
            {
                // TODO: Log this
                Debug.WriteLine( "Could not acquired the lock." );
            }

            lvw.Items.Clear();
            UpdateStatsView( true );

            chart.ChartAreas [ 0 ].AxisX.ScaleView.Zoom( 1, 10 );

            InitValues();
        }

        /// <summary>
        /// Initialize ListView
        /// </summary>
        private void InitiListView()
        {
            // List view for ping results
            lvw.View = View.Details;
            lvw.Columns.Add( "Time:", 80, HorizontalAlignment.Left );
            lvw.Columns.Add( "Result", 100, HorizontalAlignment.Left );
            lvw.Columns.Add( "Round Trip (ms)", 120, HorizontalAlignment.Left );
            lvw.Columns.Add( "TTL", 80, HorizontalAlignment.Left );
        }

        /// <summary>
        /// Initialize chart
        /// </summary>
        private void InitChart()
        {
            panel1.AutoScroll = true;

            chart.Series.Clear();
            chart.Series.Add( "Ping" );

            _dt = new DataTable( "Pings" );
            _dt.Columns.Add( "PingTime", typeof( string ) );
            _dt.Columns.Add( "RoundTrip", typeof( int ) );

            chart.DataSource = _dt;
            chart.Series [ "Ping" ].XValueMember = "PingTime";
            chart.Series [ "Ping" ].YValueMembers = "RoundTrip";
            chart.Series [ "Ping" ].ChartType = SeriesChartType.Line;

            chart.ChartAreas [ "ChartArea1" ].AxisX.Title = "Time";
            chart.ChartAreas [ "ChartArea1" ].AxisX.Interval = 1;

            chart.ChartAreas [ "ChartArea1" ].AxisY.Title = "RoundTrip (ms)";
            chart.ChartAreas [ "ChartArea1" ].AxisY.Maximum = 250;
            chart.ChartAreas [ "ChartArea1" ].AxisY.IsStartedFromZero = true;
            chart.ChartAreas [ "ChartArea1" ].AxisY.Minimum = 0;

            chart.ChartAreas [ "ChartArea1" ].AxisX.ScrollBar.Size = 16;
            chart.ChartAreas [ "ChartArea1" ].AxisX.ScrollBar.Enabled = true;
            chart.ChartAreas [ "ChartArea1" ].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            chart.ChartAreas [ "ChartArea1" ].AxisX.ScrollBar.IsPositionedInside = true;
            chart.ChartAreas [ "ChartArea1" ].CursorX.AutoScroll = true;

            if ( chart.Legends.Count == 1 )
                chart.Legends.Clear();

            chart.ChartAreas [ 0 ].AxisX.ScaleView.Zoom( 1, 10 );
        }

        /// <summary>
        /// Initialize chart types
        /// </summary>

        private void InitChartTypesCombo()
        {
            _updatingCombo = true;
            cmbChartType.BeginUpdate();

            var values = Enum.GetValues( typeof( SeriesChartType ) );
            foreach ( var chartType in values )
            {
                cmbChartType.Items.Add( chartType );
            }

            cmbChartType.SelectedIndex = 3;
            cmbChartType.EndUpdate();
            _updatingCombo = false;
        }

        /// <summary>
        /// Select new chart type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbChartType_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( _updatingCombo )
                return;

            string val = cmbChartType.SelectedItem.ToString();
            SeriesChartType chartType = SeriesChartType.Line;

            Enum.TryParse( val, out chartType );

            chart.Series [ "Ping" ].ChartType = chartType;
        }

        /// <summary>
        /// Update list view
        /// </summary>
        /// <param name="data"></param>
        private void UpdateListView( string result, string [] data )
        {
            ListViewItem lvi = new ListViewItem( DateTime.Now.ToString( "HH:mm:ss" ) );

            int i = 0;
            foreach ( var item in data )
            {
                var lvsi = lvi.SubItems.Add( item );

                if ( i == 0 && result.ToLower() != "success" )
                    lvsi.ForeColor = Color.Red;

                i++;
            }

            if ( data [ 1 ] == "0" )
                lvi.ForeColor = Color.Red;

            if ( lvw.InvokeRequired )
            {
                lvw.BeginInvoke( ( MethodInvoker ) delegate
                    {
                        lvw.Items.Insert( 0, lvi );
                    } );
            }
            else
                lvw.Items.Insert( 0, lvi );
        }

        /// <summary>
        /// Update stats view
        /// </summary>
        private void UpdateStatsView( bool clear = false )
        {
            if ( _statsData == null )
                return;

            Stats stats = new Stats( 
                _statsData.Item1, _statsData.Item2, _statsData.Item3, _maxRoundTripTreshold, _overTreshold, _failedPingRequests, _totalRequests );
            StatsDraw statsDraw = new StatsDraw( stats, this.StatsCanvas );

            if ( !clear )
                statsDraw.Draw();
            else
                statsDraw.Clear();
        }

        /// <summary>
        /// User changes the value for ping delay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numDelay_ValueChanged( object sender, EventArgs e )
        {
            if ( _initializing )
                return;

            _pingDelay = ( ushort ) numDelay.Value;
            Properties.Settings.Default [ "pingDelay" ] = _pingDelay;

            // Strange hack needed here
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Save();
        }

        #region Menu and form events
        /// <summary>
        /// Open snapshot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openSnapshotToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( btnStart.Text == "Stop" )
            {
                MessageBox.Show( "Please stop the application first.", "Application running" );
                return;
            }

            string fileName = null;

            openFileDialog1.Filter = "Ping files|*.ping";
            openFileDialog1.Title = "Select a ping snapshot file";

            if ( openFileDialog1.ShowDialog() == DialogResult.OK )
            {
                fileName = openFileDialog1.FileName;
            }

            if ( File.Exists( fileName ) )
            {
                try
                {
                    byte [] buffer = File.ReadAllBytes( fileName );
                    Stream stream = new MemoryStream( buffer );

                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer( typeof( Snapshot ) );
                    Snapshot snapshot = ( Snapshot ) deserializer.ReadObject( stream );

                    if ( snapshot != null )
                    {
                        bool acquired = Monitor.TryEnter( _locker, 50 );
                        if ( acquired )
                        {
                            _dt.Clear();
                            _dt = snapshot.SnapshotData;

                            Title title = new Title( $"Snapshot { snapshot.CreatedOnUTC.ToLocalTime() }", 
                                Docking.Top, new Font( "Arial", 10, FontStyle.Bold, GraphicsUnit.Point ), Color.Red );

                            if ( chart.InvokeRequired )
                            {
                                chart.Invoke( ( MethodInvoker ) delegate
                                {
                                    chart.DataSource = _dt;
                                    chart.DataBind();

                                    chart.Titles.Add( title );
                                } );
                            }
                            else
                            {
                                chart.DataSource = _dt;
                                chart.DataBind();

                                chart.Titles.Add( title );
                            }

                            Monitor.Exit( _locker );
                        }
                        else
                        {
                            // TODO: Log this
                            Debug.WriteLine( "Could not acquired the lock." );
                            MessageBox.Show( "Error trying to load snapshot", "Error" );
                            return;
                        }

                        _failedPingRequests = snapshot.StatsData.FailedRequests;
                        _maxRoundTripTreshold = (ushort)snapshot.StatsData.RoundTripTreshold;
                        _overTreshold = ( ushort ) snapshot.StatsData.OverTreshold;
                        _totalRequests = snapshot.StatsData.TotalRequests;
                        _statsData = snapshot.StatsData.StatsData;

                        UpdateStatsView();
                    }
                }
                catch ( Exception ex )
                {
                    Debug.WriteLine( ex.ToString() );
                    MessageBox.Show( "Error trying to load snapshot: " + ex.ToString(), "Error" );
                }
            }
        }

        /// <summary>
        /// Save snapshot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveSnapshortToolStripMenuItem_Click( object sender, EventArgs e )
        {
            bool acquired = Monitor.TryEnter( _locker, 50 );
            if ( acquired )
            {
                if ( _dt == null || _dt.Rows.Count == 0 )
                {
                    MessageBox.Show( "There is nothing to save.", "No Data" );
                    return;
                }

                _snapshot.SnapshotData = _dt.Copy();

                Monitor.Exit( _locker );
            }
            else
            {
                // TODO: Log this
                Debug.WriteLine( "Could not acquired the lock." );
                MessageBox.Show( "Error trying to load snapshot", "Error" );
                return;
            }

            // Update snapshot data
            Stats stats = new Stats(
                _statsData.Item1, _statsData.Item2, _statsData.Item3, _maxRoundTripTreshold, _overTreshold, _failedPingRequests, _totalRequests );

            _snapshot.StatsData = stats;

            saveFileDialog1.Filter = "Ping file|*.ping";
            saveFileDialog1.Title = "Save ping snapshot";
            saveFileDialog1.ShowDialog();

            if ( saveFileDialog1.FileName != String.Empty )
            {
                try
                {
                    DataContractJsonSerializer js = new DataContractJsonSerializer( typeof( Snapshot ) );
                    MemoryStream mem = new MemoryStream();
                    js.WriteObject( mem, _snapshot );

                    mem.Position = 0;
                    StreamReader sr = new StreamReader( mem );
                    string json = sr.ReadToEnd();

                    File.WriteAllText( saveFileDialog1.FileName, json );

                    try
                    {
                        sr.Close();
                        sr.Dispose();
                    }
                    catch { }

                    try
                    {
                        mem.Close();
                        mem.Dispose();
                    }
                    catch { }
                }
                catch ( Exception ex )
                {
                    Debug.WriteLine( ex.ToString() );
                    MessageBox.Show( "Error saving the snapshot: " + ex.ToString(), "Error" );
                }
            }
        }

        /// <summary>
        /// Print
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printToolStripMenuItem_Click( object sender, EventArgs e )
        {
            bool acquired = Monitor.TryEnter( _locker, 50 );
            if ( acquired )
            {
                if ( _dt == null || _dt.Rows.Count == 0 )
                {
                    MessageBox.Show( "There is nothing to print!", "No Data" );
                    return;
                }
                Monitor.Exit( _locker );
            }

            chart.Printing.PrintDocument.DocumentName = "Ping Monitor";
            chart.Printing.PrintPreview();
        }

        /// <summary>
        /// Exit the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( btnStart.Text == "Stop" )
            {
                MessageBox.Show( "Please stop the application first.", "Application running" );
                return;
            }

            if ( MessageBox.Show( "Do you realy want to exit?", "Please Confirm", MessageBoxButtons.YesNo ) == DialogResult.Yes )
                Application.Exit();
        }


        /// <summary>
        /// Open the Options dialog box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optionsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            Options options = new Options();
            options.ShowDialog();

            InitValues();
        }

        /// <summary>
        /// Form closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PingMonitor_FormClosing( object sender, FormClosingEventArgs e )
        {
            if ( btnStart.Text == "Stop" )
            {
                MessageBox.Show( "Please stop the application first.", "Application running" );
                e.Cancel = true;
                return;
            }
        }
        
        /// <summary>
        /// Form resizes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PingMonitor_Resize( object sender, EventArgs e )
        {
            splitContainer1.SplitterDistance = splitContainer1.Height - 149;
        }

        /// <summary>
        /// Form loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PingMonitor_Load( object sender, EventArgs e )
        {
            splitContainer1.SplitterDistance = splitContainer1.Height - 149;
        }

        /// <summary>
        /// About
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutPingMonitorToolStripMenuItem_Click( object sender, EventArgs e )
        {
            About about = new About();
            about.ShowDialog();
        }
        #endregion

        /// <summary>
        /// Button start clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click( object sender, EventArgs e )
        {
            if ( btnStart.Text == "Stop" )
            {
                _stop = true;
                _externalMre.Set();

                btnStart.Text = "Start";
                btnStart.ForeColor = Color.Green;
                CleanUp();

                return;
            }
            else
                btnStart.ForeColor = Color.Red;

            // End time?
            if ( txtRunTime.Text.Length > 0 )
            {
                int endTime = -1;
                Int32.TryParse( txtRunTime.Text.Trim(), out endTime );

                if ( endTime > 0 )
                {
                    _endTime = DateTime.Now.AddMinutes( endTime );
                    Debug.WriteLine( $"End time: {_endTime}" );
                }
            }

            _address = txtAddress.Text.Trim();
            if ( _address.Length == 0 )
                throw new ArgumentException( "Ping needs a host or IP Address." );

            chart.Titles.Clear();

            _externalMre.Reset();
            new Thread( () => StartPinging() ).Start();

            btnStart.Text = "Stop";
        }

        /// <summary>
        /// Calculate stats for displaying stats list view
        /// </summary>
        /// <param name="toInvoke"></param>
        /// <param name="currentRoundTrip"></param>
        private void CalculateStats( Func<ushort, Tuple<ushort, Tuple<ushort, DateTime>, Tuple<ushort, DateTime>>> toInvoke,
            ushort currentRoundTrip )
        {
            _statsData = toInvoke( currentRoundTrip );

            UpdateStatsView();
        }

        /// <summary>
        /// Gets the stats
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private Tuple<ushort, Tuple<ushort, DateTime>, Tuple<ushort, DateTime>> GetStats( ushort val )
        {
            if ( val > 0 ) // Let's not calculate the average for packes that have dropped
                _roundTrips.Add( val );
            else
                _failedPingRequests++;

            double average = _roundTrips.Count > 0 ? _roundTrips.Average() : 0;

            if ( val < _minRoundtrip.Item1 && val > 0 )
                _minRoundtrip = new Tuple<ushort, DateTime>( val, DateTime.Now );

            if ( val > _maxRoundTrip.Item1 )
                _maxRoundTrip = new Tuple<ushort, DateTime>( val, DateTime.Now );

            if ( val > _maxRoundTripTreshold )
                _overTreshold++;

            return new Tuple<ushort, Tuple<ushort, DateTime>, Tuple<ushort, DateTime>>(
                ( ushort ) average, _minRoundtrip, _maxRoundTrip );
        }

        /// <summary>
        /// Start the loop
        /// </summary>
        private void StartPinging()
        {
            WaitHandle [] waitHandles = new WaitHandle [] { _externalMre };

            bool acquired = Monitor.TryEnter( _locker, 50 );
            if ( acquired )
            {
                _dt.Clear();
                Monitor.Exit( _locker );
            }

            _overTreshold = 0;
            _failedPingRequests = 0;
            _totalRequests = 0;

            do
            {
                SendPing();
                if ( WaitHandle.WaitAll( waitHandles, TimeSpan.FromSeconds( _pingDelay ), false ) || DateTime.Now > _endTime )
                    _stop = true;

            } while ( _stop == false );

            _stop = false;

            btnStart.Invoke( ( MethodInvoker ) delegate
                {
                    btnStart.Text = "Start";
                } );
        }

        /// <summary>
        /// Panel 1 resizes, keep both panels to roughly the same size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Resize( object sender, EventArgs e )
        {
            splitContainer2.SplitterDistance = splitContainer2.Width / 2;
        }

        /// <summary>
        /// Start ping
        /// </summary>
        public void SendPing()
        {
            Ping pingSender = new Ping();

            // When request completes or timeouts, then call this method
            pingSender.PingCompleted += new PingCompletedEventHandler( PingCompletedCallback );

            // Now send it
            _mre.Reset();
            pingSender.SendAsync( _address, _pingTimeout, _buffer, _options, _mre );
            _totalRequests++;
        }

        /// <summary>
        /// Ping completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PingCompletedCallback( object sender, PingCompletedEventArgs e )
        {
            if ( e.Cancelled || e.Error != null )
            {
                Debug.WriteLine( "Request error or canceled." );
            }

            PingReply reply = e.Reply;
            ProcessReply( reply );

            // Signal the thread to resume
            _mre.Set();
        }

        /// <summary>
        /// Unpack reply
        /// </summary>
        /// <param name="reply"></param>
        private void ProcessReply( PingReply reply )
        {
            long replyRoundtrip = reply != null && reply.Status == IPStatus.Success ? reply.RoundtripTime : 0;
            int replyTtl = reply != null ? ( reply.Options != null ? reply.Options.Ttl : 0 ): 0;
            IPStatus status = reply != null ? reply.Status : IPStatus.Unknown;

            bool acquired = Monitor.TryEnter( _locker, 50 );
            if ( acquired )
            {
                DataRow dr = _dt.NewRow();
                dr [ 0 ] = DateTime.Now.ToString( "HH:mm:ss" );
                dr [ 1 ] = ( int ) replyRoundtrip;
                _dt.Rows.Add( dr );

                chart.Invoke( ( MethodInvoker ) delegate
                {
                    chart.DataBind();
                    if ( _dt.Rows.Count > 10 )
                        chart.ChartAreas [ 0 ].AxisX.ScaleView.Zoom( ( double ) ( _dt.Rows.Count - 10 ), ( double ) _dt.Rows.Count );
                } );

                Monitor.Exit( _locker );
            }

            // Update list view
            UpdateListView( status.ToString(), new string []
                {
                    status.ToString(),
                    replyRoundtrip.ToString(),
                    replyTtl.ToString()
                } );

            // Update stats
            CalculateStats( GetStats, ( ushort ) replyRoundtrip );
        }
    }
}
