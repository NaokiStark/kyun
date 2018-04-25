﻿using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace kyun.Video
{
    public class VideoDecoder : IDisposable
    {
        private int PIXEL_FORMAT = 6;
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
        private bool decoding = true;

        private static VideoDecoder __instance;

        public static VideoDecoder Instance
        {
            get
            {
                if (__instance == null || __instance.Disposing)
                    __instance = new VideoDecoder(50);
                return __instance;
            }
        }

        public bool Disposing;

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
            //PIXEL_FORMAT = (int)FFmpeg.PixelFormat.PIX_FMT_BGR32;
            this.BufferSize = bufferSize;
            this.FrameBufferTimes = new double[this.BufferSize];
            this.FrameBuffer = new byte[this.BufferSize][];

            FFmpeg.av_register_all();
            FFmpeg.avcodec_register_all();

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
            this.Disposing = true;
            this.isDisposed = true;
            if (this.decodingThread != null)
            {
                decoding = false;
                decodingFinished = true;                
                this.decodingThread.Abort();

            }
                            
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
                kyun.Video.FFmpeg.av_free(this.pFrameRGB);
                kyun.Video.FFmpeg.av_free(this.pFrame);
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
                    kyun.Video.FFmpeg.avcodec_close(this.pCodecCtx);
                    kyun.Video.FFmpeg.av_close_input_file(this.pFormatCtx);
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
            decodingThread.IsBackground = true;
            decodingThread.Start();

            return true;
        }

        [HandleProcessCorruptedStateExceptions]
        private void Decode()
        {
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
                                //double pts = Marshal.ReadInt64(packet, 0);
                                double dts = Marshal.ReadInt64(packet, 8);

                                IntPtr data = Marshal.ReadIntPtr(packet, 16); // 16 = offset of data
                                int size = Marshal.ReadInt32(packet, 20); // 20 = offset of size

                                FFmpeg.avcodec_decode_video(pCodecCtx, pFrame, ref frameFinished, data, size);

                                if (frameFinished != 0 && Marshal.ReadIntPtr(packet, 16) != IntPtr.Zero)
                                {
                                    int correct = 0;
                                    try
                                    {
                                         FFmpeg.img_convert(pFrameRGB, (int)FFmpeg.PixelFormat.PIX_FMT_RGB32, pFrame,
                                                                     (int)codecCtx.pix_fmt, codecCtx.width,
                                                                     codecCtx.height);
                                    }
                                    catch(AccessViolationException ex)
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
        }

       

        public bool OpenStream(Stream inStream)
        {
            byte[] numArray = new byte[inStream.Length];
            inStream.Read(numArray, 0, (int)inStream.Length);
            return this.Open(numArray);
        }

        public byte[] GetFrame(int time)
        {
            time = Math.Max(1, time);
            while (this.readCursor < this.writeCursor - 1 && this.FrameBufferTimes[(this.readCursor + 1) % this.BufferSize] <= (double)time)
                this.readCursor++;
            if (this.readCursor >= this.writeCursor)
                return (byte[])null;
            this.currentDisplayTime = this.FrameBufferTimes[this.readCursor % this.BufferSize];
           
            return this.FrameBuffer[this.readCursor % this.BufferSize];
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
            double local_1 = (double)time / 1000.0 / this.FrameDelay + (double)this.stream.start_time;
            if (local_1 < this.lastPts)
                local_0 = 1;
            try
            {
                kyun.Video.FFmpeg.av_seek_frame(this.pFormatCtx, this.videoStream, (long)local_1, local_0);
            }
            catch
            {
                //Stupid bug
            }
            
            this.readCursor = 1;
            this.writeCursor = 1;

        }

        public delegate void EndOfFileHandler();
    }
}