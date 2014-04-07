using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.IO;

namespace SceneNavi
{
    internal static class VersionManagement
    {
        internal static string CreateVersionString(string ver)
        {
            return CreateVersionString(new Version(ver));
        }

        internal static string CreateVersionString(Version ver)
        {
            /* Build basic version string */
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("v{0}.{1}", ver.Major, ver.Minor);

            /* Check build type */
            switch (ver.Build)
            {
                case 0:
                    /* Alpha build */
                    sb.AppendFormat(" Alpha {0}", (ver.Revision & 0xFF));
                    break;

                case 1:
                    /* Beta build */
                    sb.AppendFormat(" Beta {0}", (ver.Revision & 0xFF));
                    break;

                case 2:
                    /* Final release */
                    if (ver.Revision != 0) sb.AppendFormat(".{0}", (ver.Revision & 0xFF));
                    break;

                default:
                    /* Invalid build type */
                    throw new Exception("Invalid Build value in given version");
            }

            /* Check hotfix */
            if ((ver.Revision >> 8) != 0) sb.AppendFormat("{0}", (char)('a' + ((ver.Revision >> 8) - 1) % 26));

            /* Return compiled string */
            return sb.ToString();
        }

        internal static bool RemoteFileExists(string url)
        {
            /* Get ready to check if file exists at URL */
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            /* Trying to prevent huge delay on first WebRequest; see also App.config */
            /* http://social.msdn.microsoft.com/Forums/en-US/ncl/thread/14844bfe-ad5b-4e5a-b6ef-4ff9a1a770f8/
             * http://stackoverflow.com/questions/8300060/winform-application-first-web-request-is-slow
             * http://stackoverflow.com/questions/2519655/httpwebrequest-is-extremely-slow
             * and a whole lot more... */
            request.Proxy = null;
            request.Method = "HEAD";
            request.Timeout = 1200;

            try
            {
                /* Get and check response */
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return (response.StatusCode == HttpStatusCode.OK);
                }
            }
            catch (WebException wex)
            {
                /* Request failed */
                HttpWebResponse httpresp = (HttpWebResponse)wex.Response;
                if (httpresp.StatusCode == HttpStatusCode.NotFound) return false;
                else throw wex;
            }
        }

        internal static string[] DownloadTextFile(string url)
        {
            string[] downloaded = null;

            using (WebClient client = new WebClient())
            {
                client.Proxy = null;
                using (Stream stream = client.OpenRead(url))
                {
                    StreamReader reader = new StreamReader(stream, Encoding.Default);
                    downloaded = reader.ReadToEnd().Split('\n');
                    reader.Close();
                }
            }

            return downloaded;
        }

        internal static void DownloadRtfFile(string url, RichTextBox rtb)
        {
            using (WebClient client = new WebClient())
            {
                client.Proxy = null;
                using (Stream stream = client.OpenRead(url))
                {
                    using (MemoryStream mStream = new MemoryStream())
                    {
                        stream.CopyTo(mStream);
                        mStream.Position = 0;
                        rtb.LoadFile(mStream, RichTextBoxStreamType.RichText);
                    }
                }
            }
        }

        //internal static void CheckForUpdate(string ver, string verurl)
        //{
        //    CheckForUpdate(new Version(ver), verurl);
        //}

        //internal static void CheckForUpdate(Version ver, string verurl)
        //{
        //    /* Check if file exists */
        //    if (RemoteFileExists(verurl))
        //    {
        //        try
        //        {
        //            /* Open file & read to string */
        //            using (WebClient client = new WebClient())
        //            {
        //                client.Proxy = null;
        //                using (Stream stream = client.OpenRead(verurl))
        //                {
        //                    StreamReader reader = new StreamReader(stream, Encoding.Default);
        //                    string content = reader.ReadToEnd();
        //                    reader.Close();

        //                    /* Call version check */
        //                    VersionCheck(ver, content.Split('\n'));
        //                }
        //            }
        //        }
        //        catch (WebException wex)
        //        {
        //            /* Web access failed */
        //            MessageBox.Show(wex.ToString(), "Web Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //        }
        //    }
        //    else
        //    {
        //        /* File not found, timeout or other error */
        //        MessageBox.Show("Update check could not be completed. Remote version information could not be retrieved.", "Update Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    }
        //}

        //internal static void VersionCheck(Version localver, string[] vertxt)
        //{
        //    /* Get online version number */
        //    Version onlinever = new Version(vertxt[0]);

        //    if (onlinever > localver)
        //    {
        //        if (MessageBox.Show(string.Format("Newer version {0} is available online. Go to download location?", CreateVersionString(onlinever)), "New Version",
        //            MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
        //        {
        //            /* Open download in browser */
        //            try
        //            {
        //                System.Diagnostics.Process.Start(vertxt[1]);
        //            }
        //            catch (System.ComponentModel.Win32Exception w32ex)
        //            {
        //                /* Win32 exception, ex. no browser found */
        //                if (w32ex.ErrorCode == -2147467259) MessageBox.Show(w32ex.Message, "Process Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //            }
        //            catch (Exception ex)
        //            {
        //                /* Other exception */
        //                MessageBox.Show(ex.ToString(), "General Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show(string.Format("You are already using the most recent version {0}.", CreateVersionString(localver)), "Current Version", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //}
    }
}
