using System;
using System.Windows.Forms;

namespace RB3DOverlayed
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            //OverlaySettings.Instance.Load();
            pgSettings.SelectedObject = OverlaySettings.Instance;
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            OverlaySettings.Instance.Save();
        }
    }
}
