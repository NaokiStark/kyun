using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;

namespace kyun.Video
{
    public class VideoDecoder : IDisposable
    {
        private int PIXEL_FORMAT = 6;
        private IntPtr buffer;
        public int BufferSize;
        private FFmpeg.AVCodecContext codecCtx;
        private Thread decodingThread;
        private FFmpeg.AVFormatContext formatContext;
        public double FrameDelay;
        private int frameFinished;
        private bool isDisposed;
        private double lastPts;
        private IntPtr packet;
        private IntPtr pCodec;
        private IntPtr pCodecCtx;
        private IntPtr pFormatCtx;
        private IntPtr pFrame;
        private IntPtr pFrameRGB;
        private kyun.Video.FFmpeg.AVStream stream;
        private GCHandle streamHandle;
        private bool videoOpened;
        private int videoStream;
        private double[] FrameBufferTimes;
        private byte[][] FrameBuffer;
        private int writeCursor;
        private int readCursor;
        private bool decodingFinished;
        private bool gotNewFrame;
        public bool decoding { get; set; }

        public bool PlaybackStable { get { return (readCursor > 0 && gotNewFrame) || decodingFinished; } }

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        public static VideoDecoder __instance;

        public static VideoDecoder Instance
        {
            get
            {
              
                return null;
            }
        }

        public bool Disposing;

        public double Length
        {
            get
            {
                long num = stream.duration;
                if (num < 0L)
                    return 36000000.0;
                return (double)num * FrameDelay;
            }
        }

        public int width
        {
            get
            {
                return codecCtx.width;
            }
        }

        public int height
        {
            get
            {
                return codecCtx.height;
            }
        }

        public double CurrentTime { get; private set; }

        private double startTimeMs
        {
            get
            {
                return (double)(1000L * stream.start_time) * FrameDelay;
            }
        }

        public VideoDecoder(int bufferSize)
        {
            return;

            PIXEL_FORMAT = (int)FFmpeg.PixelFormat.PIX_FMT_RGB32;
            BufferSize = bufferSize;
            FrameBufferTimes = new double[BufferSize];
            FrameBuffer = new byte[BufferSize][];

            FFmpeg.av_register_all();
            FFmpeg.avcodec_register_all();

            videoOpened = false;
        }

        ~VideoDecoder()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {

            if (isDisposed)
                return;
            Disposing = true;
            isDisposed = true;
            if (decodingThread != null)
            {
                decoding = false;
                decodingFinished = true;
                decodingThread.Abort();

            }

            try
            {
                Marshal.FreeHGlobal(packet);
                Marshal.FreeHGlobal(buffer);
            }
            catch
            {

                Console.WriteLine("");
            }
            try
            {
                FFmpeg.av_free(pFrameRGB);
                FFmpeg.av_free(pFrame);
            }
            catch
            {
                Console.WriteLine("");
            }
            try
            {
                streamHandle.Free();
            }
            catch
            {
                Console.WriteLine("");
            }
            frameFinished = 0;
            try
            {
                if (pCodecCtx != IntPtr.Zero)
                {
                    FFmpeg.avcodec_close(pCodecCtx);
                    FFmpeg.av_close_input_file(pFormatCtx);
                }
            }
            catch (System.AccessViolationException ex)
            {
                Console.WriteLine(ex.Message);
            }

            FrameBuffer = null;
            GC.Collect();
        }

        public bool Open(string path)
        {
            try
            {
                OpenStream((Stream)File.OpenRead(path));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Open(byte[] bytes)
        {
            return false;
            //Flag
            decoding = true;

            if (bytes == null || bytes.Length == 0) return false;

            streamHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            IntPtr ptr = streamHandle.AddrOfPinnedObject();

            if (videoOpened) return false;

            videoOpened = true;

            string path = "memory:" + ptr + "|" + bytes.Length;

            int ret = FFmpeg.av_open_input_file(out pFormatCtx, path, IntPtr.Zero, bytes.Length, IntPtr.Zero);

            if (ret != 0)
                throw new Exception("Couldn't open input file");

            ret = FFmpeg.av_find_stream_info(pFormatCtx);

            if (ret < 0)
                throw new Exception("Couldn't find stream info");

            FFmpeg.dump_format(pFormatCtx, 0, path, 0);

            formatContext =
                (FFmpeg.AVFormatContext)Marshal.PtrToStructure(pFormatCtx, typeof(FFmpeg.AVFormatContext));

            videoStream = -1;
            int nbStreams = formatContext.nb_streams;

            for (int i = 0; i < nbStreams; i++)
            {
                FFmpeg.AVStream str = (FFmpeg.AVStream)
                                      Marshal.PtrToStructure(formatContext.streams[i], typeof(FFmpeg.AVStream));
                FFmpeg.AVCodecContext codec = (FFmpeg.AVCodecContext)
                                              Marshal.PtrToStructure(str.codec, typeof(FFmpeg.AVCodecContext));

                if (codec.codec_type == FFmpeg.CodecType.CODEC_TYPE_VIDEO)
                {
                    videoStream = i;
                    stream = str;
                    codecCtx = codec;
                    pCodecCtx = stream.codec;
                    break;
                }
            }
            if (videoStream == -1)
                throw new Exception("couldn't find video stream");

            FrameDelay = FFmpeg.av_q2d(stream.time_base);

            pCodec = FFmpeg.avcodec_find_decoder(codecCtx.codec_id);

            if (pCodec == IntPtr.Zero)
                throw new Exception("couldn't find decoder");

            if (FFmpeg.avcodec_open(pCodecCtx, pCodec) < 0)
                throw new Exception("couldn't open codec");


            pFrame = FFmpeg.avcodec_alloc_frame();
            pFrameRGB = FFmpeg.avcodec_alloc_frame();

            if (pFrameRGB == IntPtr.Zero)
                throw new Exception("couldn't allocate RGB frame");

            int numBytes = FFmpeg.avpicture_get_size(PIXEL_FORMAT, codecCtx.width, codecCtx.height);
            buffer = Marshal.AllocHGlobal(numBytes);

            FFmpeg.avpicture_fill(pFrameRGB, buffer, PIXEL_FORMAT, codecCtx.width, codecCtx.height);

            packet = Marshal.AllocHGlobal(57); // 52 = size of packet struct

            for (int i = 0; i < BufferSize; i++)
                FrameBuffer[i] = new byte[width * height * 4];

            decodingThread = new Thread(Decode);

            //decodingThread.IsBackground = true;

            decodingThread.Start();

            return true;
        }

        [HandleProcessCorruptedStateExceptions]
        private void Decode()
        {
            int thisThreadId = GetCurrentThreadId();

            ProcessThread CurrentThread = (from ProcessThread th in Process.GetCurrentProcess().Threads
                                           where th.Id == thisThreadId
                                           select th).Single();

            if (Environment.ProcessorCount > 1)
            {
                Thread.BeginThreadAffinity();


                CurrentThread.ProcessorAffinity = (IntPtr)0x0002;

               
            }

            // rise thread priority
            CurrentThread.PriorityLevel = ThreadPriorityLevel.TimeCritical;

            try
            {
                while (decoding)
                {
                    gotNewFrame = false;

                    lock (this)
                    {

                        while (writeCursor - readCursor < BufferSize && (decodingFinished = (FFmpeg.av_read_frame(pFormatCtx, packet) < 0)) == false)
                        {
                            if (Marshal.ReadInt32(packet, 24) == videoStream)
                            {
                                double dts = Marshal.ReadInt64(packet, 8);

                                IntPtr data = Marshal.ReadIntPtr(packet, 16); // 16 = offset of data
                                int size = Marshal.ReadInt32(packet, 20); // 20 = offset of size

                                FFmpeg.avcodec_decode_video(pCodecCtx, pFrame, ref frameFinished, data, size);

                                if (frameFinished != 0 && Marshal.ReadIntPtr(packet, 16) != IntPtr.Zero)
                                {
                                    int correct = 0;
                                    try
                                    {
                                        FFmpeg.img_convert(pFrameRGB, (int)FFmpeg.PixelFormat.PIX_FMT_RGB32, pFrame, (int)codecCtx.pix_fmt, codecCtx.width, codecCtx.height);
                                    }
                                    catch (AccessViolationException ex)
                                    {
                                        correct = -1;
                                    }


                                    //if (Marshal.ReadIntPtr(packet, 16) != IntPtr.Zero) // packet->data != null
                                    if (correct == 0)
                                    {
                                        byte[] frameData = FrameBuffer[writeCursor % BufferSize];
                                        Marshal.Copy(Marshal.ReadIntPtr(pFrameRGB), frameData, 0, frameData.Length);
                                        FrameBufferTimes[writeCursor % BufferSize] = (dts - stream.start_time) * FrameDelay * 1000;

                                        bgraToRgba(frameData, frameData.Length);


                                        writeCursor++;

                                        lastPts = dts;

                                        gotNewFrame = true;
                                    }

                                    //FFmpeg.av_free(data);
                                }
                            }
                        }
                    }
                    if (decodingFinished)
                        decoding = false;

                    if(!gotNewFrame)
                        Thread.Sleep(1);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception ex)
            {
                Logger.Instance.Warn(ex.Message + "\n\r" + ex.StackTrace);
            }
            finally
            {
                if (Environment.ProcessorCount > 1)
                {
                    Thread.EndThreadAffinity();
                }
            }

        }



        public bool OpenStream(Stream inStream)
        {
            byte[] numArray = new byte[inStream.Length];
            inStream.Read(numArray, 0, (int)inStream.Length);
            return Open(numArray);
        }

        public byte[] GetFrame(int time)
        {
            time = Math.Max(1, time);
            while (readCursor < writeCursor - 1 && FrameBufferTimes[(readCursor + 1) % BufferSize] <= (double)time)
                readCursor++;
            if (readCursor >= writeCursor)
                return (byte[])null;
            CurrentTime = FrameBufferTimes[readCursor % BufferSize];

            return FrameBuffer[readCursor % BufferSize];
        }

        public static unsafe void bgraToRgba(byte[] data, int length)
        {

            fixed (byte* dPtr = &data[0])
            {
                byte* sp = dPtr;
                byte* ep = dPtr + length;

                while (sp < ep)
                {
                    *(uint*)sp = (uint)(*(sp + 2) | *(sp + 1) << 8 | *sp << 16 | *(sp + 3) << 24);

                    sp += 4;
                }
            }
        }

        public void Seek(int time)
        {
            time = Math.Max(1, time);

            int local_0 = 0;
            double local_1 = (double)time / 1000.0 / FrameDelay + (double)stream.start_time;

            if (local_1 < lastPts)
                local_0 = 1;
            try
            {
                FFmpeg.av_seek_frame(pFormatCtx, videoStream, (long)local_1, local_0);
            }
            catch
            {
                //Stupid bug
            }

            readCursor = 1;
            writeCursor = 1;
        }

        public delegate void EndOfFileHandler();
    }
}
