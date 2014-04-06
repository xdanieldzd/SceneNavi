using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace SceneNavi
{
    public partial class UpdateCheckDialog : Form
    {
        enum updateTxtLines : int { NewVersionNumber = 0, UpdatePageUrl = 1, ReleaseNotesUrl = 2 };

        Version localVersion, remoteVersion;
        string updatePageUrl, releaseNotesUrl;

        bool IsRemoteVersionNewer
        {
            get { return (remoteVersion > localVersion); }
        }

        public UpdateCheckDialog()
        {
            InitializeComponent();

            localVersion = new Version(Application.ProductVersion);
            //localVersion = new Version(1, 0, 1, 6); //fake beta6

            System.Timers.Timer tmr = new System.Timers.Timer();
            tmr.Elapsed += new System.Timers.ElapsedEventHandler(tmr_Elapsed);
            tmr.Interval = 2.0;
            tmr.Start();
        }

        private void tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            (sender as System.Timers.Timer).Stop();

            Cursor.Current = Cursors.WaitCursor;
            this.UIThread(() => lblStatus.Text = "Checking for version information...");

            string finalStatusMsg = string.Empty;
            try
            {
                if (VersionManagement.RemoteFileExists(Configuration.UpdateServer))
                {
                    this.UIThread(() => lblStatus.Text = "Version information found; downloading...");

                    string[] updateInformation = VersionManagement.DownloadTextFile(Configuration.UpdateServer);

                    remoteVersion = new Version(updateInformation[(int)updateTxtLines.NewVersionNumber]);
                    updatePageUrl = updateInformation[(int)updateTxtLines.UpdatePageUrl];
                    if (updateInformation.Length >= 2) releaseNotesUrl = updateInformation[(int)updateTxtLines.ReleaseNotesUrl];

                    this.UIThread(() => VersionManagement.DownloadRtfFile(releaseNotesUrl, rlblChangelog));

                    if (IsRemoteVersionNewer)
                    {
                        this.UIThread(() => btnDownload.Enabled = true);
                        finalStatusMsg = string.Format("New version {0} is available!", VersionManagement.CreateVersionString(remoteVersion));
                    }
                    else
                    {
                        finalStatusMsg = string.Format("You are already using the most recent version {0}.\n", VersionManagement.CreateVersionString(localVersion));
                    }
                }
                else
                    finalStatusMsg = "Version information file not found found; please contact a developer.";
            }
            catch (WebException wex)
            {
                /* Web access failed */
                MessageBox.Show(wex.ToString(), "Web Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (System.ComponentModel.Win32Exception w32ex)
            {
                /* Win32 exception, ex. no browser found */
                if (w32ex.ErrorCode == -2147467259) MessageBox.Show(w32ex.Message, "Process Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                /* General failure */
                MessageBox.Show(ex.ToString(), "General Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            this.UIThread(() => lblStatus.Text = finalStatusMsg);
            Cursor.Current = DefaultCursor;
            this.UIThread(() => btnClose.Enabled = true);
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(updatePageUrl);
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
