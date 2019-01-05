using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kyunMono
{
    public partial class XNADownloader : Form
    {
        static string MsLink = "https://download.microsoft.com/download/5/3/A/53A804C8-EC78-43CD-A0F0-2FB4D45603D3/xnafx40_redist.msi";

        WebClient client = new WebClient();
        public XNADownloader()
        {
            InitializeComponent();
        }

        private void XNADownloader_Load(object sender, EventArgs e)
        {

        }

        private void downloadXNAInstaller()
        {
            string localPath = Application.StartupPath;
            DownloadFile(MsLink, Path.Combine(localPath, "XNA_Framework_4_Refresh.msi"),
                new DownloadProgressChangedEventHandler(
                    (obj, args) =>
                    {
                        DownloadProgressChangedEventArgs e = args;
                        Text = $"Downloading XNA: {e.ProgressPercentage}%";
                        progressBar1.Value = e.ProgressPercentage;
                    }),
                new AsyncCompletedEventHandler(
                    (obj, args) =>
                    {
                        Text = $"Installing, please wait...";
                        var c = Process.Start("msiexec", $" /i \"{Path.Combine(localPath, "XNA_Framework_4_Refresh.msi")}\"");
                        c.WaitForExit();
                        Close();
                    }
                    ));

        }

        public void DownloadFile(string address, string location, DownloadProgressChangedEventHandler progress, AsyncCompletedEventHandler complete)
        {


            Uri Uri = new Uri(address);

            client.DownloadFileCompleted += complete;

            client.DownloadProgressChanged += progress;
            
            client.DownloadFileAsync(Uri, location);
        }

        private void XNADownloader_Shown(object sender, EventArgs e)
        {
            downloadXNAInstaller();
        }
    }
}
