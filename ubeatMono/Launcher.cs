using ICSharpCode.SharpZipLib.Zip;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace kyunMono
{
    public partial class Launcher : Form
    {
        public static Launcher Instance;

        public static string UpdateBase = "http://kyun.mokyu.pw/";
        
        public Launcher()
        {
            Instance = this;
            InitializeComponent();
        }

        private void Launcher_Load(object sender, EventArgs e)
        {
            
        }

        private void Launcher_Shown(object sender, EventArgs e)
        {
            try
            {
                
                //First step
                string version = Program.GetKyunVersion();

                if (version == null)
                    version = "0";

                WebClient wc = new WebClient();
                wc.DownloadDataCompleted += (obj, args) => {
                    try
                    {
                        byte[] dataBuff = args.Result;

                        string data = Encoding.UTF8.GetString(dataBuff);

                        string[] splt = data.Split(',');
                        string actualVersion = splt[0];
                        string fallbackVersion = splt[1];

                        // a

                        //Program.mainThread.Invoke(Program.ThreadDelegate);
                        //Program.ThreadDelegate.Invoke();

                        
                        int intVersion = int.Parse(version);
                        int intActualVersion = int.Parse(actualVersion);

                        
                        if (intActualVersion > intVersion)
                        {
                            Console.WriteLine($"New Version: {actualVersion}");
                            Console.WriteLine($"Downloading");
                            DownloadUpdate();

                        }
                        else
                        {
                            Console.WriteLine($"kyun! is up to date");
                            Close();
                        }
                    }
                    catch
                    {
                        Close();

                    }
                };

                wc.DownloadDataAsync(new Uri($"{UpdateBase}lastVersion.json"));
            }
            catch
            {
                Close();

            }
        }

        private void DownloadUpdate()
        {

            
            string localPath = Path.Combine(Application.StartupPath, "update");
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
                FileAttributes fla = File.GetAttributes(localPath);
                File.SetAttributes(localPath, fla & ~FileAttributes.ReadOnly);
                
            }
            else
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(localPath);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }

                //Directory.CreateDirectory(localPath);
                FileAttributes fla = File.GetAttributes(localPath);
                File.SetAttributes(localPath, fla & ~FileAttributes.ReadOnly);
            }
            

            DownloadFile($"{UpdateBase}update/kyun.game.zip", Path.Combine(localPath, "kyun.game.zip"),
                new DownloadProgressChangedEventHandler((obj, args)=> {
                    DownloadProgressChangedEventArgs e = args;
                    lDsp.Text = $"Downloading update {e.ProgressPercentage}%";
                }),
                new AsyncCompletedEventHandler((obj, args) => {
                    AsyncCompletedEventArgs e = args;


                    if (e.Cancelled)
                    {
                        Close();

                        return;
                    }
                    lDsp.Text = "Installing update";

                    FastZip fz = new FastZip();

                    string updatePath = Path.Combine(localPath, "l");
                    Update();
                    fz.ExtractZip(Path.Combine(localPath, "kyun.game.zip"), updatePath, null);
                    Update();
                    DirectoryInfo df = new DirectoryInfo(updatePath);

                    CopyFilesRecursively(df, new DirectoryInfo(Application.StartupPath));
                   
                    //Cleanup

                    try
                    {
                        Directory.Delete(updatePath);
                    }
                    catch
                    {
                        //
                        Console.WriteLine("Error trying to cleanup, delete update folder manually.");
                    }


                    Close(); //Yay new fresh without reload


                })
            );
        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
            {
                if (file.Extension == ".pdb" || file.Name.ToLower().EndsWith("vshost.exe"))
                    continue;
               
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }
                
        }

        public void DownloadFile(string address, string location, DownloadProgressChangedEventHandler progress, AsyncCompletedEventHandler complete)
        {

            WebClient client = new WebClient();
            Uri Uri = new Uri(address);

            client.DownloadFileCompleted += complete;

            client.DownloadProgressChanged += progress;
            client.DownloadFileAsync(Uri, location);

        }        

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            
            Update();
            lDsp.Text = $"Downloading update {e.ProgressPercentage}%";

        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                Console.WriteLine("Download has been canceled.");
            }
            else
            {
                Console.WriteLine("Download completed!");
            }

            
        }
    }

    public enum DownloadType
    {
        Check,
        Update
    }
}
