using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PingGraph
{
    public partial class Options : Form
    {
        /// <summary>
        /// Address or IP
        /// </summary>
        private string _address = "www.google.com";

        /// <summary>
        /// Default timeout
        /// </summary>
        private ushort _timeout = 1000;

        /// <summary>
        /// Default delay
        /// </summary>
        private ushort _delay = 2;

        /// <summary>
        /// Default treshold
        /// </summary>
        private ushort _treshold = 30;

        /// <summary>
        /// C'tor
        /// </summary>
        public Options()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Get values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Options_Load( object sender, EventArgs e )
        {
            if ( null != Properties.Settings.Default [ "address" ] )
            {
                _address = Properties.Settings.Default [ "address" ].ToString();
                txtAddress.Text = _address;
            }

            ushort.TryParse( Properties.Settings.Default [ "pingTimeout" ].ToString(), out _timeout );
            ushort.TryParse( Properties.Settings.Default [ "pingDelay" ].ToString(), out _delay );
            ushort.TryParse( Properties.Settings.Default [ "maxRoundTrip" ].ToString(), out _treshold );

            numTimeout.Value = _timeout;
            numDelay.Value = _delay;
            numRoundtripTreshold.Value = _treshold;
        }

        /// <summary>
        /// Save values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click( object sender, EventArgs e )
        {
            Properties.Settings.Default [ "address" ] = txtAddress.Text.Trim();
            Properties.Settings.Default [ "pingTimeout" ] = (ushort)numTimeout.Value;
            Properties.Settings.Default [ "pingDelay" ] = ( ushort )numDelay.Value;
            Properties.Settings.Default [ "maxRoundTrip" ] = ( ushort )numRoundtripTreshold.Value;

            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}
