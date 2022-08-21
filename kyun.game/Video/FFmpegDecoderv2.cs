using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace kyun.game.Video
{
    public class FFmpegDecoderv2 : IDisposable
    {

        public static FFmpegDecoderv2 Instance = null;

        /// <summary>
        /// Raw Video Buffer
        /// </summary>
        public byte[][] buffer = new byte[3][];

        /// <summary>
        /// Buffer AS IS [time, frame]
        /// </summary>
        public SortedDictionary<long, byte[]> frameList = new SortedDictionary<long, byte[]>();

        /// <summary>
        /// Decoded frames
        /// </summary>
        public int frameIndex;


        public int VIDEOWIDTH = 640;
        public int VIDEOHEIGHT = 360;
        public float VIDEOFRAMERATE = 25;

        int bytesToRead = 0;
        int bytesPosition = 0;

        /// <summary>
        /// Buffer index
        /// </summary>
        public int BufferIndex;

        /// <summary>
        /// Buffer read count
        /// </summary>
        public int BufferReadCount = 0;
        public int NewBufferReadIndex = 0;
        public int BufferWriteIndex = 0;

        /// <summary>
        /// Last rendered frame requested
        /// </summary>
        public int lastFrameId = 0;

        /// <summary>
        /// Filename
        /// </summary>
        public string FileName { get; private set; }
        public bool Disposing { get; private set; }

        /// <summary>
        /// Decoding flag
        /// </summary>
        public bool Decoding { get; set; }

        /// <summary>
        /// FFmpeg
        /// </summary>
        Process ffmpegProc;

        /// <summary>
        /// Initializes VideoDecoder with 854x480 resolution
        /// </summary>
        /// <param name="filename">Video Path</param>
        public FFmpegDecoderv2(string filename)
        {
            Instance = this;
            FileName = filename;

            VIDEOFRAMERATE = GetVideoFramerate();
            Console.WriteLine(VIDEOFRAMERATE);
        }

        /// <summary>
        /// Flag to interpolation
        /// </summary>
        bool interpolate = false;

        /// <summary>
        /// Initializes VideoDecoder with custom resolution
        /// </summary>
        /// <param name="filename">Video Path</param>
        /// <param name="width">Decoded Width</param>
        /// <param name="height">Decoded height</param>
        public FFmpegDecoderv2(string filename, int width, int height)
        {
            //interpolate = true;
            Instance = this;
            VIDEOWIDTH = width;
            VIDEOHEIGHT = height;
            FileName = filename;
            VIDEOFRAMERATE = GetVideoFramerate();
            Console.WriteLine(VIDEOFRAMERATE);
        }

        /// <summary>
        /// Begin decoding
        /// </summary>
        public void Decode()
        {
            if (!File.Exists(FileName))
            {
                Decoding = false;
                return;
            }

            BufferReadCount = 0;

            // I've used Thread instead Task, 'z is more flexible, but is a little more dangerous and unsafe
            Thread vThread = new Thread(new ThreadStart(ThreadedDecoder));
            vThread.Start();
        }

        /// <summary>
        /// Decoder
        /// </summary>
        private void ThreadedDecoder()
        {
            
            string vfilter = "-filter \"minterpolate=fps=60:me=ntss:mc_mode=aobmc:vsbmc=1:me_mode=bidir:mi_mode=blend:scd=fdiff\"";
            if (interpolate)
            {
                vfilter = "";
            }
            else
            {
                VIDEOFRAMERATE = 60;
            }

            // Process info
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -loglevel error -i \"{FileName}\" -s {VIDEOWIDTH}x{VIDEOHEIGHT} -an -vf scale={VIDEOWIDTH}:{VIDEOHEIGHT}:force_original_aspect_ratio=decrease -bufsize 3 -maxrate 1600k -preset ultrafast {vfilter} -framerate {VIDEOFRAMERATE.ToString(CultureInfo.InvariantCulture)} -f rawvideo -pix_fmt bgr32 pipe:1",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                
            };

            ffmpegProc = new Process
            {
                StartInfo = startInfo
            };
            
            ffmpegProc.Start();
            ffmpegProc.PriorityClass = ProcessPriorityClass.High;
            //STDOUT
            var output = ffmpegProc.StandardOutput;

            //STDERR 
            var err = ffmpegProc.StandardError;

            //Decoding flag
            Decoding = true;
            //
            for (int a = 0; a < buffer.Length; a++)
            {
                buffer[a] = new byte[VIDEOWIDTH * VIDEOHEIGHT * 4];
            }

            int current = 4;
            try
            {


                while (!ffmpegProc.HasExited)
                {
                    while (current > 0)
                    {
                        // Waits if buffer is full

                        while(BufferWriteIndex >= VIDEOFRAMERATE + 5)
                        {
                            Thread.Sleep(1);
                        }

                        // Get the frame
                        while (bytesToRead < buffer[BufferIndex].Length)
                        {
                            current = output.BaseStream.Read(buffer[BufferIndex],
                                bytesPosition, buffer[BufferIndex].Length - bytesPosition);

                            if (current == 0)
                            {
                                break;
                            }

                            bytesPosition += current;
                            bytesToRead += current;
                        }

                        if (current != 0)
                        {
                            // Copy
                            byte[] frm = new byte[buffer[BufferIndex].Length];

                            //buffer[BufferIndex].CopyTo(frm, 0);

                            // Copy to buffer
                            buffer[BufferIndex].CopyTo(buffer[BufferWriteIndex], 0);
                                                        
                            
                            //frameList.Add((long)(frameIndex * (1000 / VIDEOFRAMERATE)), frm);

                            frameIndex++;

                            BufferIndex++;
                            BufferWriteIndex++;

                            if (BufferIndex >= buffer.Length) BufferIndex = 0;
                            bytesToRead = 0;
                            bytesPosition = 0;
                            output.BaseStream.Flush();
                        }
                        Thread.Sleep(1);
                    }
                    Thread.Sleep(1);
                }
                var strerr = err.ReadToEnd();
                Console.WriteLine(strerr);
                Decoding = false;
                if (!ffmpegProc.HasExited)
                    ffmpegProc?.Kill();

            }
            catch (Exception ex)
            {
                Logger.Instance.Severe($"{ex.Message}\r\n{ex.StackTrace}");
                Process[] prs = Process.GetProcessesByName("ffmpeg");
                foreach (Process pr in prs)
                    pr.Kill();

            }
        }

        /// <summary>
        /// Waits for first buffer fill, useful for timed framing
        /// </summary>
        public void WaitForDecoder()
        {
            while (frameIndex < 32)
            {
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Gets debug info
        /// </summary>
        /// <returns></returns>
        public string GetDebugInfo()
        {
            return $"LastFrameInBuffer: {lastFrameId} \nDecodedFrames: {frameIndex} \nFramesInBuffer: {frameList.Count} \nVideoFramerate: {VIDEOFRAMERATE} \n";
        }

        public int GetBufferCount()
        {
            return frameList.Keys.Count;
        }


        public byte[] getFrame(long time)
        {
            long frameId = (long)((float)time / VIDEOFRAMERATE);


        }

        /// <summary>
        /// Gets frame for desired time in milliseconds
        /// </summary>
        /// <param name="time">Frame time in milliseconds</param>
        /// <returns>byte[] BGR32 frame | null if is not decoding or something bad happens (lost frames|empty buffer)</returns>
        public byte[] GetFrame(long time)
        {
            
            if (frameList.Keys.Count < 1)
                return null;
            try
            {
                long key = 0;
                lock (frameList)
                {
                   
                    int frameInd = 0;
                    for (int a = 0; a < time; a++)
                    {
                        key = (long)Math.Floor(a * (1000f / VIDEOFRAMERATE));
                        if (key > time)
                        {
                            key = Math.Max((long)((a - 1) * (1000f / VIDEOFRAMERATE)), 0);
                            frameInd = a;
                            break;
                        }
                    }

                    foreach (var s in frameList.Where(kv => kv.Key <= key).ToList())
                    {
                        frameList[s.Key] = null;
                        try
                        {
                            frameList.Remove(s.Key);
                        }
                        catch { }
                    }

                    key = Math.Max((long)Math.Floor(frameInd * (1000 / VIDEOFRAMERATE)), 0);

                    lastFrameId = (int)key;
                    if (!frameList.ContainsKey(key))
                        return null;

                    var frame = frameList[key];

                    return frame;
                }               

            }
            catch
            {
                return null;
            }
        }

        public float GetVideoFramerate()
        {
            if (!File.Exists(FileName))
            {
                return 25;
            }
            // Process info
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                FileName = "ffmpeg.exe",
                Arguments = $"-hide_banner -i \"{FileName}\"",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
            };

            Process ffmpegInfoProc = new Process
            {
                StartInfo = startInfo
            };
            ffmpegInfoProc.Start();
                                  

            //STDERR 
            var err = ffmpegInfoProc.StandardError;

            var data = err.ReadToEnd();

            if (!ffmpegInfoProc.HasExited)
                ffmpegInfoProc?.Kill();

            var m = Regex.Match(data, @",\s([\d\.]*)\stbr", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            try
            {
                var val = float.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture.NumberFormat);
                return val;
            }
            catch
            {
                return 25;
            }
        }

        /// <summary>
        /// Stop decoding and disposes
        /// </summary>
        public void Dispose()
        {
            Disposing = true;
            Decoding = false;
            frameList?.Clear();
            try
            {
                if (ffmpegProc != null)
                {
                    if (!ffmpegProc.HasExited)
                        ffmpegProc?.Kill();
                }

            }
            catch { }
            finally
            {
                GC.Collect();
            }
        }
    }
}
