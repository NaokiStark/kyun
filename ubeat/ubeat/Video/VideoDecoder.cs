using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace ubeat.Video
{
    public class VideoDecoder : IDisposable
    {
        private const int PIXEL_FORMAT = 6;
        private IntPtr buffer;
        public int BufferSize;
        private FFmpeg.AVCodecContext codecCtx;
        private double currentDisplayTime;
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
        private ubeat.Video.FFmpeg.AVStream stream;
        private GCHandle streamHandle;
        private bool videoOpened;
        private int videoStream;
        private double[] FrameBufferTimes;
        private byte[][] FrameBuffer;
        private int writeCursor;
        private int readCursor;

        public double Length
        {
            get
            {
                long num = this.stream.duration;
                if (num < 0L)
                    return 36000000.0;
                return (double)num * this.FrameDelay;
            }
        }

        public int width
        {
            get
            {
                return this.codecCtx.width;
            }
        }

        public int height
        {
            get
            {
                return this.codecCtx.height;
            }
        }

        public double CurrentTime
        {
            get
            {
                return this.currentDisplayTime;
            }
        }

        private double startTimeMs
        {
            get
            {
                return (double)(1000L * this.stream.start_time) * this.FrameDelay;
            }
        }

        public VideoDecoder(int bufferSize)
        {
            this.BufferSize = bufferSize;
            this.FrameBufferTimes = new double[this.BufferSize];
            this.FrameBuffer = new byte[this.BufferSize][];
            ubeat.Video.FFmpeg.av_register_all();
            this.videoOpened = false;
        }

        ~VideoDecoder()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (this.isDisposed)
                return;
            this.isDisposed = true;
            if (this.decodingThread != null)
                this.decodingThread.Abort();
            try
            {
                Marshal.FreeHGlobal(this.packet);
                Marshal.FreeHGlobal(this.buffer);
            }
            catch
            {

                Console.WriteLine("");
            }
            try
            {
                ubeat.Video.FFmpeg.av_free(this.pFrameRGB);
                ubeat.Video.FFmpeg.av_free(this.pFrame);
            }
            catch
            {
                Console.WriteLine("");
            }
            try
            {
                this.streamHandle.Free();
            }
            catch
            {
                Console.WriteLine("");
            }
            this.frameFinished = 0;
            try
            {
                if (this.pCodecCtx != IntPtr.Zero)
                {
                    ubeat.Video.FFmpeg.avcodec_close(this.pCodecCtx);
                    ubeat.Video.FFmpeg.av_close_input_file(this.pFormatCtx);
                }
            }
            catch(System.AccessViolationException ex){
                Console.WriteLine(ex.Message);
            }
            GC.Collect();
        }

        public bool Open(string path)
        {
            try
            {
                this.OpenStream((Stream)File.OpenRead(path));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Open(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return false;
            this.streamHandle = GCHandle.Alloc((object)bytes, GCHandleType.Pinned);
            IntPtr num1 = this.streamHandle.AddrOfPinnedObject();
            if (this.videoOpened)
                return false;
            this.videoOpened = true;
            string str = "memory:" + (object)num1 + "|" + (object)bytes.Length;
            if (ubeat.Video.FFmpeg.av_open_input_file(out this.pFormatCtx, str, IntPtr.Zero, bytes.Length, IntPtr.Zero) != 0)
                throw new Exception("Couldn't open input file");
            if (ubeat.Video.FFmpeg.av_find_stream_info(this.pFormatCtx) < 0)
                throw new Exception("Couldn't find stream info");
            ubeat.Video.FFmpeg.dump_format(this.pFormatCtx, 0, str, 0);
            this.formatContext = (ubeat.Video.FFmpeg.AVFormatContext)Marshal.PtrToStructure(this.pFormatCtx, typeof(ubeat.Video.FFmpeg.AVFormatContext));
            this.videoStream = -1;
            int num2 = this.formatContext.nb_streams;
            for (int index = 0; index < num2; ++index)
            {
                ubeat.Video.FFmpeg.AVStream avStream = (ubeat.Video.FFmpeg.AVStream)Marshal.PtrToStructure(this.formatContext.streams[index], typeof(ubeat.Video.FFmpeg.AVStream));
                ubeat.Video.FFmpeg.AVCodecContext avCodecContext = (ubeat.Video.FFmpeg.AVCodecContext)Marshal.PtrToStructure(avStream.codec, typeof(ubeat.Video.FFmpeg.AVCodecContext));
                if (avCodecContext.codec_type == ubeat.Video.FFmpeg.CodecType.CODEC_TYPE_VIDEO)
                {
                    this.videoStream = index;
                    this.stream = avStream;
                    this.codecCtx = avCodecContext;
                    this.pCodecCtx = this.stream.codec;
                    break;
                }
            }
            if (this.videoStream == -1)
                throw new Exception("couldn't find video stream");
            this.FrameDelay = ubeat.Video.FFmpeg.av_q2d(this.stream.time_base);
            this.pCodec = ubeat.Video.FFmpeg.avcodec_find_decoder(this.codecCtx.codec_id);
            if (this.pCodec == IntPtr.Zero)
                throw new Exception("couldn't find decoder");
            if (ubeat.Video.FFmpeg.avcodec_open(this.pCodecCtx, this.pCodec) < 0)
                throw new Exception("couldn't open codec");
            this.pFrame = ubeat.Video.FFmpeg.avcodec_alloc_frame();
            this.pFrameRGB = ubeat.Video.FFmpeg.avcodec_alloc_frame();
            if (this.pFrameRGB == IntPtr.Zero)
                throw new Exception("couldn't allocate RGB frame");
            this.buffer = Marshal.AllocHGlobal(ubeat.Video.FFmpeg.avpicture_get_size((int)FFmpeg.PixelFormat.PIX_FMT_RGB32, this.codecCtx.width, this.codecCtx.height));
            ubeat.Video.FFmpeg.avpicture_fill(this.pFrameRGB, this.buffer, (int)FFmpeg.PixelFormat.PIX_FMT_RGB32, this.codecCtx.width, this.codecCtx.height);
            this.packet = Marshal.AllocHGlobal(57);
            for (int index = 0; index < this.BufferSize; ++index)
                this.FrameBuffer[index] = new byte[this.width * this.height*4];
            this.decodingThread = new Thread(new ThreadStart(this.Decode));
            this.decodingThread.IsBackground = true;
            this.decodingThread.Start();
            return true;
        }

        private void Decode()
        {
            try
            {
                while (true)
                {
                    bool flag;
                    do
                    {
                        flag = false;
                        lock (this)
                        {
                            while (this.writeCursor - this.readCursor < this.BufferSize)
                            {
                                if (ubeat.Video.FFmpeg.av_read_frame(this.pFormatCtx, this.packet) >= 0)
                                {
                                    if (Marshal.ReadInt32(this.packet, 24) == this.videoStream)
                                    {
                                        double local_1 = (double)Marshal.ReadInt64(this.packet, 8);
                                        ubeat.Video.FFmpeg.avcodec_decode_video(this.pCodecCtx, this.pFrame, ref this.frameFinished, Marshal.ReadIntPtr(this.packet, 16), Marshal.ReadInt32(this.packet, 20));
                                        if (this.frameFinished != 0 && Marshal.ReadIntPtr(this.packet, 16) != IntPtr.Zero && ubeat.Video.FFmpeg.img_convert(this.pFrameRGB, (int)FFmpeg.PixelFormat.PIX_FMT_RGB32, this.pFrame, (int)this.codecCtx.pix_fmt, this.codecCtx.width, this.codecCtx.height) == 0)
                                        {
                                            byte[] frameData = this.FrameBuffer[this.writeCursor % this.BufferSize];
                                            Marshal.Copy(Marshal.ReadIntPtr(this.pFrameRGB), frameData, 0, frameData.Length);
                                            this.FrameBufferTimes[this.writeCursor % this.BufferSize] = (local_1 - (double)this.stream.start_time) * this.FrameDelay * 1000.0;
                                           
                                            bgraToRgba(frameData, frameData.Length);
                                            ++this.writeCursor;
                                            
                                            this.lastPts = local_1;

                                            

                                            flag = true;
                                        }
                                    }
                                }
                                else
                                    break;
                            }
                        }
                    }
                    while (flag);
                    Thread.Sleep(15);
                }
            }
            catch (ThreadAbortException ex)
            {
            }
            catch (Exception ex)
            {
                using (StreamWriter text = File.CreateText("video-debug.txt"))
                    text.WriteLine(ex.ToString());
            }
        }

        public bool OpenStream(Stream inStream)
        {
            byte[] numArray = new byte[inStream.Length];
            inStream.Read(numArray, 0, (int)inStream.Length);
            return this.Open(numArray);
        }

        public byte[] GetFrame(int time)
        {
            while (this.readCursor < this.writeCursor - 1 && this.FrameBufferTimes[(this.readCursor + 1) % this.BufferSize] <= (double)time)
                ++this.readCursor;
            if (this.readCursor >= this.writeCursor)
                return (byte[])null;
            this.currentDisplayTime = this.FrameBufferTimes[this.readCursor % this.BufferSize];
           
            return this.FrameBuffer[this.readCursor % this.BufferSize];;
        }
        private static unsafe void bgraToRgba(byte[] data, int length)
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
            lock (this)
            {
                int local_0 = 0;
                double local_1 = (double)time / 1000.0 / this.FrameDelay + (double)this.stream.start_time;
                if (local_1 < this.lastPts)
                    local_0 = 1;
                ubeat.Video.FFmpeg.av_seek_frame(this.pFormatCtx, this.videoStream, (long)local_1, local_0);
                this.readCursor = 0;
                this.writeCursor = 0;
            }
        }

        public delegate void EndOfFileHandler();
    }
}
