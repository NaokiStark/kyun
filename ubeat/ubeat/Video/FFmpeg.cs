using System;
using System.Runtime.InteropServices;
using System.Security;

namespace ubeat.Video
{
    public static class FFmpeg
    {
        private const string AVCODEC_NATIVE_LIBRARY = "avcodec-51.dll";
        private const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;
        public const int AV_PARSER_PTS_NB = 4;
        public const int AVCODEC_MAX_AUDIO_FRAME_SIZE = 192000;
        public const int AVPALETTE_COUNT = 256;
        public const int AVPALETTE_SIZE = 1024;
        public const int CODEC_CAP_DELAY = 32;
        public const int CODEC_CAP_DR1 = 2;
        public const int CODEC_CAP_DRAW_HORIZ_BAND = 1;
        public const int CODEC_CAP_HWACCEL = 16;
        public const int CODEC_CAP_PARSE_ONLY = 4;
        public const int CODEC_CAP_SMALL_LAST_FRAME = 64;
        public const int CODEC_CAP_TRUNCATED = 8;
        public const int CODEC_FLAG_4MV = 4;
        public const int CODEC_FLAG_AC_PRED = 16777216;
        public const int CODEC_FLAG_ALT_SCAN = 1048576;
        public const int CODEC_FLAG_BITEXACT = 8388608;
        public const int CODEC_FLAG_CBP_RD = 67108864;
        [CLSCompliant(false)]
        public const uint CODEC_FLAG_CLOSED_GOP = 2147483648;
        public const int CODEC_FLAG_EMU_EDGE = 16384;
        public const int CODEC_FLAG_EXTERN_HUFF = 4096;
        public const int CODEC_FLAG_GLOBAL_HEADER = 4194304;
        public const int CODEC_FLAG_GMC = 32;
        public const int CODEC_FLAG_GRAY = 8192;
        public const int CODEC_FLAG_H263P_AIC = 16777216;
        public const int CODEC_FLAG_H263P_AIV = 8;
        public const int CODEC_FLAG_H263P_SLICE_STRUCT = 268435456;
        public const int CODEC_FLAG_H263P_UMV = 33554432;
        public const int CODEC_FLAG_INPUT_PRESERVED = 256;
        public const int CODEC_FLAG_INTERLACED_DCT = 262144;
        public const int CODEC_FLAG_INTERLACED_ME = 536870912;
        public const int CODEC_FLAG_LOOP_FILTER = 2048;
        public const int CODEC_FLAG_LOW_DELAY = 524288;
        public const int CODEC_FLAG_MV0 = 64;
        public const int CODEC_FLAG_NORMALIZE_AQP = 131072;
        public const int CODEC_FLAG_OBMC = 1;
        public const int CODEC_FLAG_PART = 128;
        public const int CODEC_FLAG_PASS1 = 512;
        public const int CODEC_FLAG_PASS2 = 1024;
        public const int CODEC_FLAG_PSNR = 32768;
        public const int CODEC_FLAG_QP_RD = 134217728;
        public const int CODEC_FLAG_QPEL = 16;
        public const int CODEC_FLAG_QSCALE = 2;
        public const int CODEC_FLAG_SVCD_SCAN_OFFSET = 1073741824;
        public const int CODEC_FLAG_TRELLIS_QUANT = 2097152;
        public const int CODEC_FLAG_TRUNCATED = 65536;
        public const int CODEC_FLAG2_8X8DCT = 128;
        public const int CODEC_FLAG2_AUD = 512;
        public const int CODEC_FLAG2_BPYRAMID = 16;
        public const int CODEC_FLAG2_BRDO = 1024;
        public const int CODEC_FLAG2_FAST = 1;
        public const int CODEC_FLAG2_FASTPSKIP = 256;
        public const int CODEC_FLAG2_INTRA_VLC = 2048;
        public const int CODEC_FLAG2_LOCAL_HEADER = 8;
        public const int CODEC_FLAG2_MEMC_ONLY = 4096;
        public const int CODEC_FLAG2_MIXED_REFS = 64;
        public const int CODEC_FLAG2_NO_OUTPUT = 4;
        public const int CODEC_FLAG2_STRICT_GOP = 2;
        public const int CODEC_FLAG2_WPRED = 32;
        public const int DEFAULT_FRAME_RATE_BASE = 1001000;
        public const int FF_AA_AUTO = 0;
        public const int FF_AA_FASTINT = 1;
        public const int FF_AA_FLOAT = 3;
        public const int FF_AA_INT = 2;
        public const int FF_ALPHA_SEMI_TRANSP = 2;
        public const int FF_ALPHA_TRANSP = 1;
        public const int FF_ASPECT_EXTENDED = 15;
        public const int FF_B_TYPE = 3;
        public const int FF_BUFFER_HINTS_PRESERVE = 4;
        public const int FF_BUFFER_HINTS_READABLE = 2;
        public const int FF_BUFFER_HINTS_REUSABLE = 8;
        public const int FF_BUFFER_HINTS_VALID = 1;
        public const int FF_BUFFER_TYPE_COPY = 8;
        public const int FF_BUFFER_TYPE_INTERNAL = 1;
        public const int FF_BUFFER_TYPE_SHARED = 4;
        public const int FF_BUFFER_TYPE_USER = 2;
        public const int FF_BUG_AC_VLC = 0;
        public const int FF_BUG_AMV = 32;
        public const int FF_BUG_AUTODETECT = 1;
        public const int FF_BUG_DC_CLIP = 4096;
        public const int FF_BUG_DIRECT_BLOCKSIZE = 512;
        public const int FF_BUG_EDGE = 1024;
        public const int FF_BUG_HPEL_CHROMA = 2048;
        public const int FF_BUG_MS = 8192;
        public const int FF_BUG_NO_PADDING = 16;
        public const int FF_BUG_OLD_MSMPEG4 = 2;
        public const int FF_BUG_QPEL_CHROMA = 64;
        public const int FF_BUG_QPEL_CHROMA2 = 256;
        public const int FF_BUG_STD_QPEL = 128;
        public const int FF_BUG_UMP4 = 8;
        public const int FF_BUG_XVID_ILACE = 4;
        public const int FF_CMP_BIT = 5;
        public const int FF_CMP_CHROMA = 256;
        public const int FF_CMP_DCT = 3;
        public const int FF_CMP_DCT264 = 14;
        public const int FF_CMP_DCTMAX = 13;
        public const int FF_CMP_NSSE = 10;
        public const int FF_CMP_PSNR = 4;
        public const int FF_CMP_RD = 6;
        public const int FF_CMP_SAD = 0;
        public const int FF_CMP_SATD = 2;
        public const int FF_CMP_SSE = 1;
        public const int FF_CMP_VSAD = 8;
        public const int FF_CMP_VSSE = 9;
        public const int FF_CMP_W53 = 11;
        public const int FF_CMP_W97 = 12;
        public const int FF_CMP_ZERO = 7;
        public const int FF_CODER_TYPE_AC = 1;
        public const int FF_CODER_TYPE_VLC = 0;
        public const int FF_COMPLIANCE_EXPERIMENTAL = -2;
        public const int FF_COMPLIANCE_INOFFICIAL = -1;
        public const int FF_COMPLIANCE_NORMAL = 0;
        public const int FF_COMPLIANCE_STRICT = 1;
        public const int FF_COMPLIANCE_VERY_STRICT = 2;
        public const int FF_COMPRESSION_DEFAULT = -1;
        public const int FF_DCT_ALTIVEC = 5;
        public const int FF_DCT_AUTO = 0;
        public const int FF_DCT_FAAN = 6;
        public const int FF_DCT_FASTINT = 1;
        public const int FF_DCT_INT = 2;
        public const int FF_DCT_MLIB = 4;
        public const int FF_DCT_MMX = 3;
        public const int FF_DEBUG_BITSTREAM = 4;
        public const int FF_DEBUG_BUGS = 4096;
        public const int FF_DEBUG_DCT_COEFF = 64;
        public const int FF_DEBUG_ER = 1024;
        public const int FF_DEBUG_MB_TYPE = 8;
        public const int FF_DEBUG_MMCO = 2048;
        public const int FF_DEBUG_MV = 32;
        public const int FF_DEBUG_PICT_INFO = 1;
        public const int FF_DEBUG_PTS = 512;
        public const int FF_DEBUG_QP = 16;
        public const int FF_DEBUG_RC = 2;
        public const int FF_DEBUG_SKIP = 128;
        public const int FF_DEBUG_STARTCODE = 256;
        public const int FF_DEBUG_VIS_MB_TYPE = 16384;
        public const int FF_DEBUG_VIS_MV_B_BACK = 4;
        public const int FF_DEBUG_VIS_MV_B_FOR = 2;
        public const int FF_DEBUG_VIS_MV_P_FOR = 1;
        public const int FF_DEBUG_VIS_QP = 8192;
        public const int FF_DEFAULT_QUANT_BIAS = 999999;
        public const int FF_DTG_AFD_14_9 = 11;
        public const int FF_DTG_AFD_16_9 = 10;
        public const int FF_DTG_AFD_16_9_SP_14_9 = 14;
        public const int FF_DTG_AFD_4_3 = 9;
        public const int FF_DTG_AFD_4_3_SP_14_9 = 13;
        public const int FF_DTG_AFD_SAME = 8;
        public const int FF_DTG_AFD_SP_4_3 = 15;
        public const int FF_EC_DEBLOCK = 2;
        public const int FF_EC_GUESS_MVS = 1;
        public const int FF_ER_AGGRESSIVE = 3;
        public const int FF_ER_CAREFUL = 1;
        public const int FF_ER_COMPLIANT = 2;
        public const int FF_ER_VERY_AGGRESSIVE = 4;
        public const int FF_I_TYPE = 1;
        public const int FF_IDCT_ALTIVEC = 8;
        public const int FF_IDCT_ARM = 7;
        public const int FF_IDCT_AUTO = 0;
        public const int FF_IDCT_CAVS = 15;
        public const int FF_IDCT_H264 = 11;
        public const int FF_IDCT_INT = 1;
        public const int FF_IDCT_IPP = 13;
        public const int FF_IDCT_LIBMPEG2MMX = 4;
        public const int FF_IDCT_MLIB = 6;
        public const int FF_IDCT_PS2 = 5;
        public const int FF_IDCT_SH4 = 9;
        public const int FF_IDCT_SIMPLE = 2;
        public const int FF_IDCT_SIMPLEARM = 10;
        public const int FF_IDCT_SIMPLEMMX = 3;
        public const int FF_IDCT_VP3 = 12;
        public const int FF_IDCT_XVIDMMX = 14;
        public const int FF_INPUT_BUFFER_PADDING_SIZE = 8;
        public const int FF_LAMBDA_MAX = 32767;
        public const int FF_LAMBDA_SCALE = 128;
        public const int FF_LAMBDA_SHIFT = 7;
        public const int FF_LEVEL_UNKNOWN = -99;
        public const int FF_LOSS_ALPHA = 8;
        public const int FF_LOSS_CHROMA = 32;
        public const int FF_LOSS_COLORQUANT = 16;
        public const int FF_LOSS_COLORSPACE = 4;
        public const int FF_LOSS_DEPTH = 2;
        public const int FF_LOSS_RESOLUTION = 1;
        public const int FF_MAX_B_FRAMES = 16;
        public const int FF_MB_DECISION_BITS = 1;
        public const int FF_MB_DECISION_RD = 2;
        public const int FF_MB_DECISION_SIMPLE = 0;
        public const int FF_MIN_BUFFER_SIZE = 16384;
        public const int FF_MM_3DNOW = 4;
        public const int FF_MM_3DNOWEXT = 32;
        [CLSCompliant(false)]
        public const uint FF_MM_FORCE = 2147483648;
        public const int FF_MM_IWMMXT = 256;
        public const int FF_MM_MMX = 1;
        public const int FF_MM_MMXEXT = 2;
        public const int FF_MM_SSE = 8;
        public const int FF_MM_SSE2 = 16;
        public const int FF_P_TYPE = 2;
        public const int FF_PRED_LEFT = 0;
        public const int FF_PRED_MEDIAN = 2;
        public const int FF_PRED_PLANE = 1;
        public const int FF_PROFILE_UNKNOWN = -99;
        public const int FF_QP2LAMBDA = 118;
        public const int FF_QSCALE_TYPE_H264 = 2;
        public const int FF_QSCALE_TYPE_MPEG1 = 0;
        public const int FF_QSCALE_TYPE_MPEG2 = 1;
        public const int FF_RC_STRATEGY_XVID = 1;
        public const int FF_S_TYPE = 4;
        public const int FF_SI_TYPE = 5;
        public const int FF_SP_TYPE = 6;
        public const int MB_TYPE_16x16 = 8;
        public const int MB_TYPE_16x8 = 16;
        public const int MB_TYPE_8x16 = 32;
        public const int MB_TYPE_8x8 = 64;
        public const int MB_TYPE_ACPRED = 512;
        public const int MB_TYPE_CBP = 131072;
        public const int MB_TYPE_DIRECT2 = 256;
        public const int MB_TYPE_GMC = 1024;
        public const int MB_TYPE_INTERLACED = 128;
        public const int MB_TYPE_INTRA_PCM = 4;
        public const int MB_TYPE_INTRA16x16 = 2;
        public const int MB_TYPE_INTRA4x4 = 1;
        public const int MB_TYPE_L0 = 12288;
        public const int MB_TYPE_L0L1 = 61440;
        public const int MB_TYPE_L1 = 49152;
        public const int MB_TYPE_P0L0 = 4096;
        public const int MB_TYPE_P0L1 = 16384;
        public const int MB_TYPE_P1L0 = 8192;
        public const int MB_TYPE_P1L1 = 32768;
        public const int MB_TYPE_QUANT = 65536;
        public const int MB_TYPE_SKIP = 2048;
        public const int PARSER_FLAG_COMPLETE_FRAMES = 1;
        public const int SLICE_FLAG_ALLOW_FIELD = 2;
        public const int SLICE_FLAG_ALLOW_PLANE = 4;
        public const int SLICE_FLAG_CODED_ORDER = 1;
        public const int X264_PART_B8X8 = 256;
        public const int X264_PART_I4X4 = 1;
        public const int X264_PART_I8X8 = 2;
        public const int X264_PART_P4X4 = 32;
        public const int X264_PART_P8X8 = 16;
        private const string AVFORMAT_NATIVE_LIBRARY = "avformat-52.dll";
        public const int AV_TIME_BASE = 1000000;
        public const int AVFMT_INFINITEOUTPUTLOOP = 0;
        [CLSCompliant(false)]
        public const uint AVFMT_FLAG_GENPTS = 1;
        [CLSCompliant(false)]
        public const uint AVFMT_FLAG_IGNIDX = 2;
        [CLSCompliant(false)]
        public const int AVFMT_NOOUTPUTLOOP = -1;
        [CLSCompliant(false)]
        public const uint AVFMT_NOFILE = 1;
        [CLSCompliant(false)]
        public const uint AVFMT_NEEDNUMBER = 2;
        [CLSCompliant(false)]
        public const uint AVFMT_SHOW_IDS = 8;
        [CLSCompliant(false)]
        public const uint AVFMT_RAWPICTURE = 32;
        [CLSCompliant(false)]
        public const uint AVFMT_GLOBALHEADER = 64;
        [CLSCompliant(false)]
        public const uint AVFMT_NOTIMESTAMPS = 128;
        [CLSCompliant(false)]
        public const uint AVIMAGE_INTERLEAVED = 1;
        public const int AVPROBE_SCORE_MAX = 100;
        public const int PKT_FLAG_KEY = 1;
        public const int AVINDEX_KEYFRAME = 1;
        public const int MAX_REORDER_DELAY = 4;
        [CLSCompliant(false)]
        public const uint AVFMTCTX_NOHEADER = 1;
        public const int MAX_STREAMS = 20;
        public const int AVERROR_UNKNOWN = -1;
        public const int AVERROR_IO = -2;
        public const int AVERROR_NUMEXPECTED = -3;
        public const int AVERROR_INVALIDDATA = -4;
        public const int AVERROR_NOMEM = -5;
        public const int AVERROR_NOFMT = -6;
        public const int AVERROR_NOTSUPP = -7;
        public const int AVSEEK_FLAG_BACKWARD = 1;
        public const int AVSEEK_FLAG_BYTE = 2;
        public const int AVSEEK_FLAG_ANY = 4;
        public const int FFM_PACKET_SIZE = 4096;
        private const string AVUTIL_NATIVE_LIBRARY = "avutil-49.dll";

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr audio_resample_init(int output_channels, int input_channels, int output_rate, int input_rate);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int audio_resample(IntPtr pResampleContext, IntPtr output, IntPtr intput, int nb_samples);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void audio_resample_close(IntPtr pResampleContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr av_resample_init(int out_rate, int in_rate, int filter_length, int log2_phase_count, int linear, double cutoff);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_resample(IntPtr pAVResampleContext, IntPtr dst, IntPtr src, IntPtr consumed, int src_size, int udpate_ctx);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_resample_compensate(IntPtr pAVResampleContext, int sample_delta, int compensation_distance);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_resample_close(IntPtr pAVResampleContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr img_resample_init(int output_width, int output_height, int input_width, int input_height);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr img_resample_full_init(int owidth, int oheight, int iwidth, int iheight, int topBand, int bottomBand, int leftBand, int rightBand, int padtop, int padbottom, int padleft, int padright);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void img_resample(IntPtr pImgReSampleContext, IntPtr p_output_AVPicture, IntPtr p_input_AVPicture);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImgReSampleContext(IntPtr pImgReSampleContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avpicture_alloc(IntPtr pAVPicture, int pix_fmt, int width, int height);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avpicture_free(IntPtr pAVPicture);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avpicture_fill(IntPtr pAVPicture, IntPtr ptr, int pix_fmt, int width, int height);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avpicture_layout(IntPtr p_src_AVPicture, int pix_fmt, int width, int height, IntPtr dest, int dest_size);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avpicture_get_size(int pix_fmt, int width, int height);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avcodec_get_chroma_sub_sample(int pix_fmt, IntPtr h_shift, IntPtr v_shift);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string avcodec_get_pix_fmt_name(int pix_fmt);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avcodec_set_dimensions(IntPtr pAVCodecContext, int width, int height);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern PixelFormat avcodec_get_pix_fmt([MarshalAs(UnmanagedType.LPStr)] string name);

        [SuppressUnmanagedCodeSecurity]
        [CLSCompliant(false)]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint avcodec_pix_fmt_to_codec_tag(PixelFormat p);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_get_pix_fmt_loss(int dst_pix_fmt, int src_pix_fmt, int has_alpha);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_find_best_pix_fmt(int pix_fmt_mask, int src_pix_fmt, int has_alpha, IntPtr loss_ptr);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int img_get_alpha_info(IntPtr pAVPicture, int pix_fmt, int width, int height);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int img_convert(IntPtr p_dst_AVPicture, int dst_pix_fmt, IntPtr p_src_AVPicture, int pix_fmt, int width, int height);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avpicture_deinterlace(IntPtr p_dst_AVPicture, IntPtr p_src_AVPicture, int pix_fmt, int width, int height);

        [CLSCompliant(false)]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint avcodec_version();

        [SuppressUnmanagedCodeSecurity]
        [CLSCompliant(false)]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint avcodec_build();

        [SuppressUnmanagedCodeSecurity]
        [CLSCompliant(false)]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint avcodec_init();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void register_avcodec(IntPtr pAVCodec);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr avcodec_find_encoder(ubeat.Video.FFmpeg.CodecID id);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr avcodec_find_encoder_by_name([MarshalAs(UnmanagedType.LPStr)] string mame);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr avcodec_find_decoder(ubeat.Video.FFmpeg.CodecID id);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr avcodec_find_decoder_by_name([MarshalAs(UnmanagedType.LPStr)] string mame);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avcodec_string([MarshalAs(UnmanagedType.LPStr)] string mam, int buf_size, IntPtr pAVCodeContext, int encode);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avcodec_get_context_defaults(IntPtr pAVCodecContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr avcodec_alloc_context();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avcodec_get_frame_defaults(IntPtr pAVFrame);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr avcodec_alloc_frame();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_default_get_buffer(IntPtr pAVCodecContext, IntPtr pAVFrame);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avcodec_default_release_buffer(IntPtr pAVCodecContext, IntPtr pAVFrame);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avcodec_default_reget_buffer(IntPtr pAVCodecContext, IntPtr pAVFrame);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avcodec_align_dimensions(IntPtr pAVCodecContext, ref int width, ref int height);

        [SuppressUnmanagedCodeSecurity]
        [CLSCompliant(false)]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_check_dimensions(IntPtr av_log_ctx, ref uint width, ref uint height);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern PixelFormat avcodec_default_get_format(IntPtr pAVCodecContext, ref PixelFormat fmt);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_thread_init(IntPtr pAVCodecContext, int thread_count);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avcodec_thread_free(IntPtr pAVCodecContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_thread_execute(IntPtr pAVCodecContext, [MarshalAs(UnmanagedType.FunctionPtr)] ubeat.Video.FFmpeg.FuncCallback func, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] arg, ref int ret, int count);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_default_execute(IntPtr pAVCodecContext, [MarshalAs(UnmanagedType.FunctionPtr)] ubeat.Video.FFmpeg.FuncCallback func, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] arg, ref int ret, int count);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_open(IntPtr pAVCodecContext, IntPtr pAVCodec);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_decode_audio(IntPtr pAVCodecContext, IntPtr samples, [In, Out] ref int frame_size_ptr, IntPtr buf, int buf_size);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_decode_video(IntPtr pAVCodecContext, IntPtr pAVFrame, ref int got_picture_ptr, IntPtr buf, int buf_size);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_decode_subtitle(IntPtr pAVCodecContext, IntPtr pAVSubtitle, ref int got_sub_ptr, IntPtr buf, int buf_size);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_parse_frame(IntPtr pAVCodecContext, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] pdata, IntPtr data_size_ptr, IntPtr buf, int buf_size);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_encode_audio(IntPtr pAVCodecContext, IntPtr buf, int buf_size, IntPtr samples);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_encode_video(IntPtr pAVCodecContext, IntPtr buf, int buf_size, IntPtr pAVFrame);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_encode_subtitle(IntPtr pAVCodecContext, IntPtr buf, int buf_size, IntPtr pAVSubtitle);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_close(IntPtr pAVCodecContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avcodec_register_all();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avcodec_flush_buffers(IntPtr pAVCodecContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avcodec_default_free_buffers(IntPtr pAVCodecContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte av_get_pict_type_char(int pict_type);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_get_bits_per_sample(ubeat.Video.FFmpeg.CodecID codec_id);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_register_codec_parser(IntPtr pAVcodecParser);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr av_parser_init(int codec_id);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_parser_parse(IntPtr pAVCodecParserContext, IntPtr pAVCodecContext, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] poutbuf, ref int poutbuf_size, IntPtr buf, int buf_size, long pts, long dts);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_parser_change(IntPtr pAVCodecParserContext, IntPtr pAVCodecContext, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] poutbuf, ref int poutbuf_size, IntPtr buf, int buf_size, int keyframe);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_parser_close(IntPtr pAVCodecParserContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_register_bitstream_filter(IntPtr pAVBitStreamFilter);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr av_bitstream_filter_init([MarshalAs(UnmanagedType.LPStr)] string name);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_bitstream_filter_filter(IntPtr pAVBitStreamFilterContext, IntPtr pAVCodecContext, [MarshalAs(UnmanagedType.LPStr)] string args, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] poutbuf, ref int poutbuf_size, IntPtr buf, int buf_size, int keyframe);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_bitstream_filter_close(IntPtr pAVBitStreamFilterContext);

        [CLSCompliant(false)]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_mallocz(uint size);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr av_strdup([MarshalAs(UnmanagedType.LPStr)] string s);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_freep(IntPtr ptr);

        [SuppressUnmanagedCodeSecurity]
        [CLSCompliant(false)]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_fast_realloc(IntPtr ptr, ref uint size, ref uint min_size);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_free_static();

        [SuppressUnmanagedCodeSecurity]
        [CLSCompliant(false)]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_mallocz_static(uint size);

        [SuppressUnmanagedCodeSecurity]
        [CLSCompliant(false)]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_realloc_static(IntPtr ptr, uint size);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void img_copy(IntPtr pAVPicture, IntPtr p_src_AVPicture, int pix_fmt, int width, int height);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int img_crop(IntPtr p_dst_pAVPicture, IntPtr p_src_pAVPicture, int pix_fmt, int top_band, int left_band);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avcodec-51.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int img_pad(IntPtr p_dst_pAVPicture, IntPtr p_src_pAVPicture, int height, int width, int pix_fmt, int padtop, int padbottom, int padleft, int padright, ref int color);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int register_protocol(IntPtr protocol);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_destruct_packet_nofree(IntPtr pAVPacket);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_destruct_packet(IntPtr pAVPacket);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_init_packet(IntPtr pAVPacket);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_new_packet(IntPtr pAVPacket, int size);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_get_packet(IntPtr pByteIOContext, IntPtr pAVPacket, int size);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_dup_packet(IntPtr pAVPacket);

        public static void av_free_packet(IntPtr pAVPacket)
        {
            if (pAVPacket == IntPtr.Zero)
                return;
            ubeat.Video.FFmpeg.AVPacket avPacket = (ubeat.Video.FFmpeg.AVPacket)Marshal.PtrToStructure(pAVPacket, typeof(ubeat.Video.FFmpeg.AVPacket));
            if (avPacket.destruct == null)
                return;
            avPacket.destruct(pAVPacket);
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_register_image_format(IntPtr pAVImageFormat);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr av_probe_image_format(IntPtr pAVProbeData);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr guess_image_format([MarshalAs(UnmanagedType.LPTStr)] string filename);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ubeat.Video.FFmpeg.CodecID av_guess_image2_codec([MarshalAs(UnmanagedType.LPTStr)] string filename);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_read_image(IntPtr pByteIOContext, [MarshalAs(UnmanagedType.LPTStr)] string filename, IntPtr pAVImageFormat, [MarshalAs(UnmanagedType.FunctionPtr)] ubeat.Video.FFmpeg.AllocCBCallback alloc_cb, IntPtr opaque);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_write_image(IntPtr pByteIOContext, IntPtr pAVImageFormat, IntPtr pAVImageInfo);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_register_input_format(IntPtr pAVInputFormat);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_register_output_format(IntPtr pAVOutputFormat);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr guess_stream_format([MarshalAs(UnmanagedType.LPTStr)] string short_name, [MarshalAs(UnmanagedType.LPTStr)] string filename, [MarshalAs(UnmanagedType.LPTStr)] string mime_type);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr guess_format([MarshalAs(UnmanagedType.LPTStr)] string short_name, [MarshalAs(UnmanagedType.LPTStr)] string filename, [MarshalAs(UnmanagedType.LPTStr)] string mime_type);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern ubeat.Video.FFmpeg.CodecID av_guess_codec(IntPtr pAVOutoutFormat, [MarshalAs(UnmanagedType.LPTStr)] string short_name, [MarshalAs(UnmanagedType.LPTStr)] string filename, [MarshalAs(UnmanagedType.LPTStr)] string mime_type, ubeat.Video.FFmpeg.CodecType type);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_hex_dump(IntPtr pFile, IntPtr buf, int size);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_pkt_dump(IntPtr pFile, IntPtr pAVPacket, int dump_payload);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_register_all();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr av_find_input_format([MarshalAs(UnmanagedType.LPTStr)] string short_name);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr av_probe_input_format(IntPtr pAVProbeData, int is_opened);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_open_input_file(out IntPtr pFormatContext, [MarshalAs(UnmanagedType.LPStr)] string filename, IntPtr pAVInputFormat, int buf_size, IntPtr pAVFormatParameters);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_open_input_file_hax(out IntPtr pFormatContext, [MarshalAs(UnmanagedType.LPStr)] string filename, IntPtr buffer, IntPtr pAVInputFormat, int buf_size, IntPtr pAVFormatParameters);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr av_alloc_format_context();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_find_stream_info(IntPtr pAVFormatContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_read_packet(IntPtr pAVFormatContext, IntPtr pAVPacket);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_read_frame(IntPtr pAVFormatContext, IntPtr pAVPacket);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_seek_frame(IntPtr pAVFormatContext, int stream_index, long timestamp, int flags);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_read_play(IntPtr pAVFormatContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_read_pause(IntPtr pAVFormatContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_close_input_stream(IntPtr pAVFormatContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_close_input_file(IntPtr pAVFormatContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr av_new_stream(IntPtr pAVFormatContext, int id);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr av_new_program(IntPtr pAVFormatContext, int id);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_set_pts_info(IntPtr pAVStream, int pts_wrap_bits, int pts_num, int pts_den);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_find_default_stream_index(IntPtr pAVFormatContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_index_search_timestamp(IntPtr pAVStream, long timestamp, int flags);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_add_index_entry(IntPtr pAVStream, long pos, long timestamp, int size, int distance, int flags);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_seek_frame_binary(IntPtr pAVFormatContext, int stream_index, long target_ts, int flags);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_update_cur_dts(IntPtr pAVFormatContext, IntPtr pAVStream, long timestamp);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_set_parameters(IntPtr pAVFormatContext, IntPtr pAVFormatParameters);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_write_header(IntPtr pAVFormatContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_write_frame(IntPtr pAVFormatContext, IntPtr pAVPacket);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_interleaved_write_frame(IntPtr pAVFormatContext, IntPtr pAVPacket);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_interleave_packet_per_dts(IntPtr pAVFormatContext, out IntPtr p_out_AVPacket, IntPtr pAVPacket, int flush);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_write_trailer(IntPtr pAVFormatContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void dump_format(IntPtr pAVFormatContext, int index, [MarshalAs(UnmanagedType.LPTStr)] string url, int is_output);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int parse_image_size(IntPtr width_ptr, IntPtr height_ptr, [MarshalAs(UnmanagedType.LPTStr)] string arg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int parse_frame_rate(IntPtr pFrame_rate, IntPtr pFrame_rate_base, [MarshalAs(UnmanagedType.LPTStr)] string arg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long parse_date([MarshalAs(UnmanagedType.LPTStr)] string datestr, int duration);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long av_gettime();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern long ffm_read_write_index(int fd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ffm_write_write_index(int fd, long pos);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ffm_set_write_index(IntPtr pAVFormatContext, long pos, long file_size);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int find_info_tag([MarshalAs(UnmanagedType.LPTStr)] string arg, int arg_size, [MarshalAs(UnmanagedType.LPTStr)] string tag1, [MarshalAs(UnmanagedType.LPTStr)] string info);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_get_frame_filename(IntPtr buf, int buf_size, [MarshalAs(UnmanagedType.LPTStr)] string path, int number);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_filename_number_test([MarshalAs(UnmanagedType.LPTStr)] string filename);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int video_grab_init();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int audio_init();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int dv1394_init();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avformat-52.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int dc1394_init();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("avutil-49.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_free(IntPtr ptr);

        public static double av_q2d(ubeat.Video.FFmpeg.AVRational a)
        {
            return (double)a.num / (double)a.den;
        }

        public delegate int CloseCallback(IntPtr pAVCodecContext);

        public delegate int DecodeCallback(IntPtr pAVCodecContext, IntPtr outdata, ref int outdata_size, IntPtr buf, int buf_size);

        public delegate void DrawhorizBandCallback(IntPtr pAVCodecContext, IntPtr pAVFrame, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)] int[] offset, int y, int type, int height);

        public delegate int EncodeCallback(IntPtr pAVCodecContext, IntPtr buf, int buf_size, IntPtr data);

        public delegate int ExecuteCallback(IntPtr pAVCodecContext, [MarshalAs(UnmanagedType.FunctionPtr)] ubeat.Video.FFmpeg.FuncCallback func, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] arg2, ref int ret, int count);

        public delegate int FilterCallback(IntPtr pAVBitStreamFilterContext, IntPtr pAVCodecContext, [MarshalAs(UnmanagedType.LPStr)] string args, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] poutbuf, ref int poutbuf_size, IntPtr buf, int buf_size, int keyframe);

        public delegate int FlushCallback(IntPtr pAVCodecContext);

        public delegate int FuncCallback(IntPtr pAVCodecContext, IntPtr parg);

        public delegate int GetBufferCallback(IntPtr pAVCodecContext, IntPtr pAVFrame);

        public delegate PixelFormat GetFormatCallback(IntPtr pAVCodecContext, IntPtr pPixelFormat);

        public delegate int InitCallback(IntPtr pAVCodecContext);

        public delegate int ParaerInitCallback(IntPtr pAVCodecParserContext);

        public delegate void ParserCloseCallback(IntPtr pAVcodecParserContext);

        public delegate int ParserParseCallback(IntPtr pAVCodecParserContext, IntPtr pAVCodecContext, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] poutbuf, ref int poutbuf_size, IntPtr buf, int buf_size);

        public delegate int RegetBufferCallback(IntPtr pAVCodecContext, IntPtr pAVFrame);

        public delegate void ReleaseBufferCallback(IntPtr pAVCodecContext, IntPtr pAVFrame);

        public delegate void RtpCallback(IntPtr pAVCodecContext, IntPtr pdata, int size, int mb_nb);

        public delegate int SplitCallback(IntPtr pAVCodecContext, IntPtr buf, int buf_size);

        public enum AVDiscard
        {
            AVDISCARD_NONE = -16,
            AVDISCARD_DEFAULT = 0,
            AVDISCARD_NONREF = 8,
            AVDISCARD_BIDIR = 16,
            AVDISCARD_NONKEY = 32,
            AVDISCARD_ALL = 48,
        }

        public enum CodecID
        {
            CODEC_ID_NONE = 0,
            CODEC_ID_MPEG1VIDEO = 1,
            CODEC_ID_MPEG2VIDEO = 2,
            CODEC_ID_MPEG2VIDEO_XVMC = 3,
            CODEC_ID_H261 = 4,
            CODEC_ID_H263 = 5,
            CODEC_ID_RV10 = 6,
            CODEC_ID_RV20 = 7,
            CODEC_ID_MJPEG = 8,
            CODEC_ID_MJPEGB = 9,
            CODEC_ID_LJPEG = 10,
            CODEC_ID_SP5X = 11,
            CODEC_ID_JPEGLS = 12,
            CODEC_ID_MPEG4 = 13,
            CODEC_ID_RAWVIDEO = 14,
            CODEC_ID_MSMPEG4V1 = 15,
            CODEC_ID_MSMPEG4V2 = 16,
            CODEC_ID_MSMPEG4V3 = 17,
            CODEC_ID_WMV1 = 18,
            CODEC_ID_WMV2 = 19,
            CODEC_ID_H263P = 20,
            CODEC_ID_H263I = 21,
            CODEC_ID_FLV1 = 22,
            CODEC_ID_SVQ1 = 23,
            CODEC_ID_SVQ3 = 24,
            CODEC_ID_DVVIDEO = 25,
            CODEC_ID_HUFFYUV = 26,
            CODEC_ID_CYUV = 27,
            CODEC_ID_H264 = 28,
            CODEC_ID_INDEO3 = 29,
            CODEC_ID_VP3 = 30,
            CODEC_ID_THEORA = 31,
            CODEC_ID_ASV1 = 32,
            CODEC_ID_ASV2 = 33,
            CODEC_ID_FFV1 = 34,
            CODEC_ID_4XM = 35,
            CODEC_ID_VCR1 = 36,
            CODEC_ID_CLJR = 37,
            CODEC_ID_MDEC = 38,
            CODEC_ID_ROQ = 39,
            CODEC_ID_INTERPLAY_VIDEO = 40,
            CODEC_ID_XAN_WC3 = 41,
            CODEC_ID_XAN_WC4 = 42,
            CODEC_ID_RPZA = 43,
            CODEC_ID_CINEPAK = 44,
            CODEC_ID_WS_VQA = 45,
            CODEC_ID_MSRLE = 46,
            CODEC_ID_MSVIDEO1 = 47,
            CODEC_ID_IDCIN = 48,
            CODEC_ID_8BPS = 49,
            CODEC_ID_SMC = 50,
            CODEC_ID_FLIC = 51,
            CODEC_ID_TRUEMOTION1 = 52,
            CODEC_ID_VMDVIDEO = 53,
            CODEC_ID_MSZH = 54,
            CODEC_ID_ZLIB = 55,
            CODEC_ID_QTRLE = 56,
            CODEC_ID_SNOW = 57,
            CODEC_ID_TSCC = 58,
            CODEC_ID_ULTI = 59,
            CODEC_ID_QDRAW = 60,
            CODEC_ID_VIXL = 61,
            CODEC_ID_QPEG = 62,
            CODEC_ID_XVID = 63,
            CODEC_ID_PNG = 64,
            CODEC_ID_PPM = 65,
            CODEC_ID_PBM = 66,
            CODEC_ID_PGM = 67,
            CODEC_ID_PGMYUV = 68,
            CODEC_ID_PAM = 69,
            CODEC_ID_FFVHUFF = 70,
            CODEC_ID_RV30 = 71,
            CODEC_ID_RV40 = 72,
            CODEC_ID_VC1 = 73,
            CODEC_ID_WMV3 = 74,
            CODEC_ID_LOCO = 75,
            CODEC_ID_WNV1 = 76,
            CODEC_ID_AASC = 77,
            CODEC_ID_INDEO2 = 78,
            CODEC_ID_FRAPS = 79,
            CODEC_ID_TRUEMOTION2 = 80,
            CODEC_ID_BMP = 81,
            CODEC_ID_CSCD = 82,
            CODEC_ID_MMVIDEO = 83,
            CODEC_ID_ZMBV = 84,
            CODEC_ID_AVS = 85,
            CODEC_ID_SMACKVIDEO = 86,
            CODEC_ID_NUV = 87,
            CODEC_ID_KMVC = 88,
            CODEC_ID_FLASHSV = 89,
            CODEC_ID_CAVS = 90,
            CODEC_ID_JPEG2000 = 91,
            CODEC_ID_VMNC = 92,
            CODEC_ID_VP5 = 93,
            CODEC_ID_VP6 = 94,
            CODEC_ID_VP6F = 95,
            CODEC_ID_PCM_S16LE = 65536,
            CODEC_ID_PCM_S16BE = 65537,
            CODEC_ID_PCM_U16LE = 65538,
            CODEC_ID_PCM_U16BE = 65539,
            CODEC_ID_PCM_S8 = 65540,
            CODEC_ID_PCM_U8 = 65541,
            CODEC_ID_PCM_MULAW = 65542,
            CODEC_ID_PCM_ALAW = 65543,
            CODEC_ID_PCM_S32LE = 65544,
            CODEC_ID_PCM_S32BE = 65545,
            CODEC_ID_PCM_U32LE = 65546,
            CODEC_ID_PCM_U32BE = 65547,
            CODEC_ID_PCM_S24LE = 65548,
            CODEC_ID_PCM_S24BE = 65549,
            CODEC_ID_PCM_U24LE = 65550,
            CODEC_ID_PCM_U24BE = 65551,
            CODEC_ID_PCM_S24DAUD = 65552,
            CODEC_ID_ADPCM_IMA_QT = 69632,
            CODEC_ID_ADPCM_IMA_WAV = 69633,
            CODEC_ID_ADPCM_IMA_DK3 = 69634,
            CODEC_ID_ADPCM_IMA_DK4 = 69635,
            CODEC_ID_ADPCM_IMA_WS = 69636,
            CODEC_ID_ADPCM_IMA_SMJPEG = 69637,
            CODEC_ID_ADPCM_MS = 69638,
            CODEC_ID_ADPCM_4XM = 69639,
            CODEC_ID_ADPCM_XA = 69640,
            CODEC_ID_ADPCM_ADX = 69641,
            CODEC_ID_ADPCM_EA = 69642,
            CODEC_ID_ADPCM_G726 = 69643,
            CODEC_ID_ADPCM_CT = 69644,
            CODEC_ID_ADPCM_SWF = 69645,
            CODEC_ID_ADPCM_YAMAHA = 69646,
            CODEC_ID_ADPCM_SBPRO_4 = 69647,
            CODEC_ID_ADPCM_SBPRO_3 = 69648,
            CODEC_ID_ADPCM_SBPRO_2 = 69649,
            CODEC_ID_AMR_NB = 73728,
            CODEC_ID_AMR_WB = 73729,
            CODEC_ID_RA_144 = 77824,
            CODEC_ID_RA_288 = 77825,
            CODEC_ID_ROQ_DPCM = 81920,
            CODEC_ID_INTERPLAY_DPCM = 81921,
            CODEC_ID_XAN_DPCM = 81922,
            CODEC_ID_SOL_DPCM = 81923,
            CODEC_ID_MP2 = 86016,
            CODEC_ID_MP3 = 86017,
            CODEC_ID_AAC = 86018,
            CODEC_ID_MPEG4AAC = 86019,
            CODEC_ID_AC3 = 86020,
            CODEC_ID_DTS = 86021,
            CODEC_ID_VORBIS = 86022,
            CODEC_ID_DVAUDIO = 86023,
            CODEC_ID_WMAV1 = 86024,
            CODEC_ID_WMAV2 = 86025,
            CODEC_ID_MACE3 = 86026,
            CODEC_ID_MACE6 = 86027,
            CODEC_ID_VMDAUDIO = 86028,
            CODEC_ID_SONIC = 86029,
            CODEC_ID_SONIC_LS = 86030,
            CODEC_ID_FLAC = 86031,
            CODEC_ID_MP3ADU = 86032,
            CODEC_ID_MP3ON4 = 86033,
            CODEC_ID_SHORTEN = 86034,
            CODEC_ID_ALAC = 86035,
            CODEC_ID_WESTWOOD_SND1 = 86036,
            CODEC_ID_GSM = 86037,
            CODEC_ID_QDM2 = 86038,
            CODEC_ID_COOK = 86039,
            CODEC_ID_TRUESPEECH = 86040,
            CODEC_ID_TTA = 86041,
            CODEC_ID_SMACKAUDIO = 86042,
            CODEC_ID_QCELP = 86043,
            CODEC_ID_DVD_SUBTITLE = 94208,
            CODEC_ID_DVB_SUBTITLE = 94209,
            CODEC_ID_MPEG2TS = 131072,
        }

        public enum CodecType
        {
            CODEC_TYPE_UNKNOWN = -1,
            CODEC_TYPE_VIDEO = 0,
            CODEC_TYPE_AUDIO = 1,
            CODEC_TYPE_DATA = 2,
            CODEC_TYPE_SUBTITLE = 3,
        }

        public enum Motion_Est_ID
        {
            ME_ZERO = 1,
            ME_FULL = 2,
            ME_LOG = 3,
            ME_PHODS = 4,
            ME_EPZS = 5,
            ME_X1 = 6,
            ME_HEX = 7,
            ME_UMH = 8,
            ME_ITER = 9,
        }

        public enum SampleFormat
        {
            SAMPLE_FMT_NONE = -1,
            SAMPLE_FMT_U8 = 0,
            SAMPLE_FMT_S16 = 1,
            SAMPLE_FMT_S24 = 2,
            SAMPLE_FMT_S32 = 3,
            SAMPLE_FMT_FLT = 4,
        }

        public struct AVBitStreamFilter
        {
            [MarshalAs(UnmanagedType.LPStr)]
            private string name;
            [MarshalAs(UnmanagedType.I4)]
            private int priv_data_size;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            private ubeat.Video.FFmpeg.FilterCallback filter;
            private IntPtr next;
        }

        public struct AVBitStreamFilterContext
        {
            public IntPtr priv_data;
            public IntPtr filter;
            public IntPtr parser;
            public IntPtr next;
        }

        public struct AVCodec
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string name;
            public ubeat.Video.FFmpeg.CodecType type;
            public ubeat.Video.FFmpeg.CodecID id;
            [MarshalAs(UnmanagedType.I4)]
            public int priv_data_size;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.InitCallback init;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.EncodeCallback encode;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.CloseCallback close;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.DecodeCallback decode;
            [MarshalAs(UnmanagedType.I4)]
            public int capabilities;
            public IntPtr next;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.FlushCallback flush;
            public IntPtr supported_framerates;
            public IntPtr pix_fmts;
        }

        public struct AVCodecContext
        {
            public IntPtr av_class;
            [MarshalAs(UnmanagedType.I4)]
            public int bit_rate;
            [MarshalAs(UnmanagedType.I4)]
            public int bit_rate_tolerance;
            [MarshalAs(UnmanagedType.I4)]
            public int flags;
            [MarshalAs(UnmanagedType.I4)]
            public int sub_id;
            [MarshalAs(UnmanagedType.I4)]
            public int me_method;
            public IntPtr extradata;
            [MarshalAs(UnmanagedType.I4)]
            public int extradata_size;
            public ubeat.Video.FFmpeg.AVRational time_base;
            [MarshalAs(UnmanagedType.I4)]
            public int width;
            [MarshalAs(UnmanagedType.I4)]
            public int height;
            [MarshalAs(UnmanagedType.I4)]
            public int gop_size;
            public PixelFormat pix_fmt;
            [MarshalAs(UnmanagedType.I4)]
            public int rate_emu;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.DrawhorizBandCallback draw_horiz_band;
            [MarshalAs(UnmanagedType.I4)]
            public int sample_rate;
            [MarshalAs(UnmanagedType.I4)]
            public int channels;
            public ubeat.Video.FFmpeg.SampleFormat sample_fmt;
            [MarshalAs(UnmanagedType.I4)]
            public int frame_size;
            [MarshalAs(UnmanagedType.I4)]
            public int frame_number;
            [MarshalAs(UnmanagedType.I4)]
            public int real_pict_num;
            [MarshalAs(UnmanagedType.I4)]
            public int delay;
            [MarshalAs(UnmanagedType.R4)]
            public float qcompress;
            [MarshalAs(UnmanagedType.R4)]
            public float qblur;
            [MarshalAs(UnmanagedType.I4)]
            public int qmin;
            [MarshalAs(UnmanagedType.I4)]
            public int qmax;
            [MarshalAs(UnmanagedType.I4)]
            public int max_qdiff;
            [MarshalAs(UnmanagedType.I4)]
            public int max_b_frames;
            [MarshalAs(UnmanagedType.R4)]
            public float b_quant_factor;
            [MarshalAs(UnmanagedType.I4)]
            public int rc_strategy;
            [MarshalAs(UnmanagedType.I4)]
            public int b_frame_strategy;
            [MarshalAs(UnmanagedType.I4)]
            public int hurry_up;
            public IntPtr codec;
            public IntPtr priv_data;
            [MarshalAs(UnmanagedType.I4)]
            public int rtp_mode;
            [MarshalAs(UnmanagedType.I4)]
            public int rtp_payload_size;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.RtpCallback rtp_callback;
            [MarshalAs(UnmanagedType.I4)]
            public int mv_bits;
            [MarshalAs(UnmanagedType.I4)]
            public int header_bits;
            [MarshalAs(UnmanagedType.I4)]
            public int i_tex_bits;
            [MarshalAs(UnmanagedType.I4)]
            public int p_tex_bits;
            [MarshalAs(UnmanagedType.I4)]
            public int i_count;
            [MarshalAs(UnmanagedType.I4)]
            public int p_count;
            [MarshalAs(UnmanagedType.I4)]
            public int skip_count;
            [MarshalAs(UnmanagedType.I4)]
            public int misc_bits;
            [MarshalAs(UnmanagedType.I4)]
            public int frame_bits;
            public IntPtr opaque;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] codec_name;
            public ubeat.Video.FFmpeg.CodecType codec_type;
            public ubeat.Video.FFmpeg.CodecID codec_id;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U4)]
            public uint codec_tag;
            [MarshalAs(UnmanagedType.I4)]
            public int workaround_bugs;
            [MarshalAs(UnmanagedType.I4)]
            public int luma_elim_threshold;
            [MarshalAs(UnmanagedType.I4)]
            public int chroma_elim_threshold;
            [MarshalAs(UnmanagedType.I4)]
            public int strict_std_compliance;
            [MarshalAs(UnmanagedType.R4)]
            public float b_quant_offset;
            [MarshalAs(UnmanagedType.I4)]
            public int error_resilience;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.GetBufferCallback get_buffer;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ReleaseBufferCallback release_buffer;
            [MarshalAs(UnmanagedType.I4)]
            public int has_b_frames;
            [MarshalAs(UnmanagedType.I4)]
            public int block_align;
            [MarshalAs(UnmanagedType.I4)]
            public int parse_only;
            [MarshalAs(UnmanagedType.I4)]
            public int mpeg_quant;
            [MarshalAs(UnmanagedType.LPStr)]
            public string stats_out;
            public string stats_in;
            [MarshalAs(UnmanagedType.R4)]
            public float rc_qsquish;
            [MarshalAs(UnmanagedType.R4)]
            public float rc_qmod_amp;
            [MarshalAs(UnmanagedType.I4)]
            public int rc_qmod_freq;
            public IntPtr rc_override;
            [MarshalAs(UnmanagedType.I4)]
            public int rc_override_count;
            [MarshalAs(UnmanagedType.LPStr)]
            public string rc_eq;
            [MarshalAs(UnmanagedType.I4)]
            public int rc_max_rate;
            [MarshalAs(UnmanagedType.I4)]
            public int rc_min_rate;
            [MarshalAs(UnmanagedType.I4)]
            public int rc_buffer_size;
            [MarshalAs(UnmanagedType.R4)]
            public float rc_buffer_aggressivity;
            [MarshalAs(UnmanagedType.R4)]
            public float i_quant_factor;
            [MarshalAs(UnmanagedType.R4)]
            public float i_quant_offset;
            [MarshalAs(UnmanagedType.R4)]
            public float rc_initial_cplx;
            [MarshalAs(UnmanagedType.I4)]
            public int dct_algo;
            [MarshalAs(UnmanagedType.R4)]
            public float lumi_masking;
            [MarshalAs(UnmanagedType.R4)]
            public float temporal_cplx_masking;
            [MarshalAs(UnmanagedType.R4)]
            public float spatial_cplx_masking;
            [MarshalAs(UnmanagedType.R4)]
            public float p_masking;
            [MarshalAs(UnmanagedType.R4)]
            public float dark_masking;
            [MarshalAs(UnmanagedType.I4)]
            public int unused;
            [MarshalAs(UnmanagedType.I4)]
            public int idct_algo;
            [MarshalAs(UnmanagedType.I4)]
            public int slice_count;
            public IntPtr slice_offset;
            [MarshalAs(UnmanagedType.I4)]
            public int error_concealment;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U4)]
            public uint dsp_mask;
            [MarshalAs(UnmanagedType.I4)]
            public int bits_per_sample;
            [MarshalAs(UnmanagedType.I4)]
            public int prediction_method;
            public ubeat.Video.FFmpeg.AVRational sample_aspect_ratio;
            public IntPtr coded_frame;
            [MarshalAs(UnmanagedType.I4)]
            public int debug;
            [MarshalAs(UnmanagedType.I4)]
            public int debug_mv;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public long[] error;
            [MarshalAs(UnmanagedType.I4)]
            public int mb_qmin;
            [MarshalAs(UnmanagedType.I4)]
            public int mb_qmax;
            [MarshalAs(UnmanagedType.I4)]
            public int me_cmp;
            [MarshalAs(UnmanagedType.I4)]
            public int me_sub_cmp;
            [MarshalAs(UnmanagedType.I4)]
            public int mb_cmp;
            [MarshalAs(UnmanagedType.I4)]
            public int ildct_cmp;
            [MarshalAs(UnmanagedType.I4)]
            public int dia_size;
            [MarshalAs(UnmanagedType.I4)]
            public int last_predictor_count;
            [MarshalAs(UnmanagedType.I4)]
            public int pre_me;
            [MarshalAs(UnmanagedType.I4)]
            public int me_pre_cmp;
            [MarshalAs(UnmanagedType.I4)]
            public int pre_dia_size;
            [MarshalAs(UnmanagedType.I4)]
            public int me_subpel_quality;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.GetFormatCallback get_format;
            [MarshalAs(UnmanagedType.I4)]
            public int dtg_active_format;
            [MarshalAs(UnmanagedType.I4)]
            public int me_range;
            [MarshalAs(UnmanagedType.I4)]
            public int intra_quant_bias;
            [MarshalAs(UnmanagedType.I4)]
            public int inter_quant_bias;
            [MarshalAs(UnmanagedType.I4)]
            public int color_table_id;
            [MarshalAs(UnmanagedType.I4)]
            public int internal_buffer_count;
            public IntPtr internal_buffer;
            [MarshalAs(UnmanagedType.I4)]
            public int global_quality;
            [MarshalAs(UnmanagedType.I4)]
            public int coder_type;
            [MarshalAs(UnmanagedType.I4)]
            public int context_model;
            [MarshalAs(UnmanagedType.I4)]
            public int slice_flags;
            [MarshalAs(UnmanagedType.I4)]
            public int xvmc_acceleration;
            [MarshalAs(UnmanagedType.I4)]
            public int mb_decision;
            public IntPtr intra_matrix;
            public IntPtr inter_matrix;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U4)]
            public uint stream_codec_tag;
            [MarshalAs(UnmanagedType.I4)]
            public int scenechange_threshold;
            [MarshalAs(UnmanagedType.I4)]
            public int lmin;
            [MarshalAs(UnmanagedType.I4)]
            public int lmax;
            public IntPtr palctrl;
            [MarshalAs(UnmanagedType.I4)]
            public int noise_reduction;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.RegetBufferCallback reget_buffer;
            [MarshalAs(UnmanagedType.I4)]
            public int rc_initial_buffer_occupancy;
            [MarshalAs(UnmanagedType.I4)]
            public int inter_threshold;
            [MarshalAs(UnmanagedType.I4)]
            public int flags2;
            [MarshalAs(UnmanagedType.I4)]
            public int error_rate;
            [MarshalAs(UnmanagedType.I4)]
            public int antialias_algo;
            [MarshalAs(UnmanagedType.I4)]
            public int quantizer_noise_shaping;
            [MarshalAs(UnmanagedType.I4)]
            public int thread_count;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ExecuteCallback execute;
            public IntPtr thread_opaque;
            [MarshalAs(UnmanagedType.I4)]
            public int me_threshold;
            [MarshalAs(UnmanagedType.I4)]
            public int mb_threshold;
            [MarshalAs(UnmanagedType.I4)]
            public int intra_dc_precision;
            [MarshalAs(UnmanagedType.I4)]
            public int nsse_weight;
            [MarshalAs(UnmanagedType.I4)]
            public int skip_top;
            [MarshalAs(UnmanagedType.I4)]
            public int skip_bottom;
            [MarshalAs(UnmanagedType.I4)]
            public int profile;
            [MarshalAs(UnmanagedType.I4)]
            public int level;
            [MarshalAs(UnmanagedType.I4)]
            public int lowres;
            [MarshalAs(UnmanagedType.I4)]
            public int coded_width;
            [MarshalAs(UnmanagedType.I4)]
            public int coded_height;
            [MarshalAs(UnmanagedType.I4)]
            public int frame_skip_threshold;
            [MarshalAs(UnmanagedType.I4)]
            public int frame_skip_factor;
            [MarshalAs(UnmanagedType.I4)]
            public int frame_skip_exp;
            [MarshalAs(UnmanagedType.I4)]
            public int frame_skip_cmp;
            [MarshalAs(UnmanagedType.R4)]
            public float border_masking;
            [MarshalAs(UnmanagedType.I4)]
            public int mb_lmin;
            [MarshalAs(UnmanagedType.I4)]
            public int mb_lmax;
            [MarshalAs(UnmanagedType.I4)]
            public int me_penalty_compensation;
            public ubeat.Video.FFmpeg.AVDiscard skip_loop_filter;
            public ubeat.Video.FFmpeg.AVDiscard skip_frame;
            [MarshalAs(UnmanagedType.I4)]
            public int bidir_refine;
            [MarshalAs(UnmanagedType.I4)]
            public int brd_scale;
            [MarshalAs(UnmanagedType.I4)]
            public int crf;
            [MarshalAs(UnmanagedType.I4)]
            public int cqp;
            [MarshalAs(UnmanagedType.I4)]
            public int keyint_min;
            [MarshalAs(UnmanagedType.I4)]
            public int refs;
            [MarshalAs(UnmanagedType.I4)]
            public int chromaoffset;
            [MarshalAs(UnmanagedType.I4)]
            public int bframebias;
            [MarshalAs(UnmanagedType.I4)]
            public int trellis;
            [MarshalAs(UnmanagedType.R4)]
            public float complexityblur;
            [MarshalAs(UnmanagedType.I4)]
            public int deblockalpha;
            [MarshalAs(UnmanagedType.I4)]
            public int deblockbeta;
            [MarshalAs(UnmanagedType.I4)]
            public int partitions;
            [MarshalAs(UnmanagedType.I4)]
            public int directpred;
            [MarshalAs(UnmanagedType.I4)]
            public int cutoff;
            [MarshalAs(UnmanagedType.I4)]
            public int scenechange_factor;
            [MarshalAs(UnmanagedType.I4)]
            public int mv0_threshold;
            [MarshalAs(UnmanagedType.I4)]
            public int b_sensitivity;
            [MarshalAs(UnmanagedType.I4)]
            public int compression_level;
            [MarshalAs(UnmanagedType.I4)]
            public int use_lpc;
            [MarshalAs(UnmanagedType.I4)]
            public int lpc_coeff_precision;
            [MarshalAs(UnmanagedType.I4)]
            public int min_prediction_order;
            [MarshalAs(UnmanagedType.I4)]
            public int max_prediction_order;
            [MarshalAs(UnmanagedType.I4)]
            public int prediction_order_method;
            [MarshalAs(UnmanagedType.I4)]
            public int min_partition_order;
            [MarshalAs(UnmanagedType.I4)]
            public int max_partition_order;
        }

        public struct AVCodecParser
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public int[] codec_ids;
            [MarshalAs(UnmanagedType.I4)]
            public int priv_data_size;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ParaerInitCallback parser_init;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ParserParseCallback parser_parse;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ParserCloseCallback parser_close;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.SplitCallback split;
            public IntPtr next;
        }

        public struct AVCodecParserContext
        {
            public IntPtr priv_data;
            public IntPtr parser;
            [MarshalAs(UnmanagedType.I8)]
            public long frame_offset;
            [MarshalAs(UnmanagedType.I8)]
            public long cur_offset;
            [MarshalAs(UnmanagedType.I8)]
            public long last_frame_offset;
            [MarshalAs(UnmanagedType.I4)]
            public int pict_type;
            [MarshalAs(UnmanagedType.I4)]
            public int repeat_pict;
            [MarshalAs(UnmanagedType.I8)]
            public long pts;
            [MarshalAs(UnmanagedType.I8)]
            public long dts;
            [MarshalAs(UnmanagedType.I8)]
            public long last_pts;
            [MarshalAs(UnmanagedType.I8)]
            public long last_dts;
            [MarshalAs(UnmanagedType.I4)]
            public int fetch_timestamp;
            [MarshalAs(UnmanagedType.I4)]
            public int cur_frame_start_index;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public long[] cur_frame_offset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public long[] cur_frame_pts;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public long[] cur_frame_dts;
            [MarshalAs(UnmanagedType.I4)]
            public int flags;
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct AVFrame
        {
        }

        public struct AVPaletteControl
        {
            [MarshalAs(UnmanagedType.I4)]
            public int palette_changed;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] palette;
        }

        public struct AVPanScan
        {
            public int height;
            public int id;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public short[] position;
            public int width;
        }

        public struct AVPicture
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            private IntPtr[] data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            private int[] linesize;
        }

        public struct AVSubtitle
        {
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U2)]
            public ushort format;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U4)]
            public uint start_display_time;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U4)]
            public uint end_display_time;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U4)]
            public uint num_rects;
            public IntPtr rects;
        }

        public struct AVSubtitleRect
        {
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U2)]
            public ushort x;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U2)]
            public ushort y;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U2)]
            public ushort w;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U2)]
            public ushort h;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U2)]
            public ushort nb_colors;
            [MarshalAs(UnmanagedType.I4)]
            public int linesize;
            public IntPtr bitmap;
        }

        public struct RcOverride
        {
            [MarshalAs(UnmanagedType.I4)]
            public int start_frame;
            [MarshalAs(UnmanagedType.I4)]
            public int end_frame;
            [MarshalAs(UnmanagedType.I4)]
            public int qscale;
            [MarshalAs(UnmanagedType.R4)]
            public float quality_factor;
        }

        public delegate void DestructCallback(IntPtr pAVPacket);

        public struct AVPacket
        {
            [MarshalAs(UnmanagedType.I8)]
            public long pts;
            [MarshalAs(UnmanagedType.I8)]
            public long dts;
            public IntPtr data;
            [MarshalAs(UnmanagedType.I4)]
            public int size;
            [MarshalAs(UnmanagedType.I4)]
            public int stream_index;
            [MarshalAs(UnmanagedType.I4)]
            public int flags;
            [MarshalAs(UnmanagedType.I4)]
            public int duration;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.DestructCallback destruct;
            public IntPtr priv;
            [MarshalAs(UnmanagedType.I8)]
            public long pos;
        }

        public struct AVFrac
        {
            [MarshalAs(UnmanagedType.I8)]
            public long val;
            [MarshalAs(UnmanagedType.I8)]
            public long num;
            [MarshalAs(UnmanagedType.I8)]
            public long den;
        }

        public struct AVProbeData
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string filename;
            public IntPtr buf;
            public int buf_size;
        }

        public struct AVFormatParameters
        {
            public ubeat.Video.FFmpeg.AVRational time_base;
            public int sample_rate;
            public int channels;
            public int width;
            public int height;
            public PixelFormat pix_fmt;
            public IntPtr image_format;
            public int channel;
            [MarshalAs(UnmanagedType.LPStr)]
            public string standard;
            public int mpeg2ts_raw;
            public int mpeg2ts_compute_pcr;
            public int initial_pause;
            public int prealloced_context;
        }

        public delegate int WriteHeader(IntPtr pAVFormatContext);

        public delegate int WritePacket(IntPtr pAVFormatContext, IntPtr pAVPacket);

        public delegate int WriteTrailer(IntPtr pAVFormatContext);

        public delegate int SetParametersCallback(IntPtr pAVFormatContext, IntPtr avFormatParameters);

        public delegate int InterleavePacketCallback(IntPtr pAVFormatContext, IntPtr pOutAVPacket, IntPtr pInAVPacket, int flush);

        public struct AVOutputFormat
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string name;
            [MarshalAs(UnmanagedType.LPStr)]
            public string long_name;
            [MarshalAs(UnmanagedType.LPStr)]
            public string mime_type;
            [MarshalAs(UnmanagedType.LPStr)]
            public string extensions;
            public int priv_data_size;
            public ubeat.Video.FFmpeg.CodecID audio_codec;
            public ubeat.Video.FFmpeg.CodecID video_codec;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.WriteHeader write_header;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.WritePacket write_packet;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.WriteTrailer write_trailer;
            public int flags;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.SetParametersCallback set_parameters;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.InterleavePacketCallback interleave_packet;
            public IntPtr nextAVOutputFormat;
        }

        public delegate int ReadProbeCallback(IntPtr pAVProbeData);

        public delegate int ReadHeaderCallback(IntPtr pAVFormatContext, IntPtr pAVFormatParameters);

        public delegate int ReadPacketCallback(IntPtr pAVFormatContext, IntPtr pAVPacket);

        public delegate int ReadCloseCallback(IntPtr pAVFormatContext);

        public delegate int ReadSeekCallback(IntPtr pAVFormatContext, int stream_index, long timestamp, int flags);

        public delegate int ReadTimestampCallback(IntPtr pAVFormatContext, int stream_index, IntPtr pos, long pos_limit);

        public delegate int ReadPlayCallback(IntPtr pAVFormatContext);

        public delegate int ReadPauseCallback(IntPtr pAVFormatContext);

        public struct AVInputFormat
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string name;
            [MarshalAs(UnmanagedType.LPStr)]
            public string long_name;
            public int priv_data_size;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ReadProbeCallback read_probe;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ReadHeaderCallback read_header;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ReadPacketCallback read_packet;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ReadCloseCallback read_close;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ReadSeekCallback read_seek;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ReadTimestampCallback read_timestamp;
            public int flags;
            [MarshalAs(UnmanagedType.LPStr)]
            public string extensions;
            public int value;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ReadPlayCallback read_play;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ReadPauseCallback read_pause;
            public IntPtr nextAVInputFormat;
        }

        public struct AVIndexEntry
        {
            public long pos;
            public long timestamp;
            public int flags;
            public int size;
            public int min_distance;
        }

        public struct AVStream
        {
            [MarshalAs(UnmanagedType.I4)]
            public int index;
            [MarshalAs(UnmanagedType.I4)]
            public int id;
            public IntPtr codec;
            public ubeat.Video.FFmpeg.AVRational r_frame_rate;
            public IntPtr priv_data;
            [MarshalAs(UnmanagedType.I8)]
            public long codec_info_duration;
            public ubeat.Video.FFmpeg.AVFrac pts;
            public ubeat.Video.FFmpeg.AVRational time_base;
            [MarshalAs(UnmanagedType.I4)]
            public int pts_wrap_bits;
            [MarshalAs(UnmanagedType.I4)]
            public int stream_copy;
            public ubeat.Video.FFmpeg.AVDiscard discard;
            [MarshalAs(UnmanagedType.R4)]
            public float quality;
            [MarshalAs(UnmanagedType.I8)]
            public long start_time;
            [MarshalAs(UnmanagedType.I8)]
            public long duration;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] language;
            [MarshalAs(UnmanagedType.I4)]
            public int need_parsing;
            public IntPtr pAVCodecParserContext;
            [MarshalAs(UnmanagedType.I8)]
            public long cur_dts;
            [MarshalAs(UnmanagedType.I4)]
            public int last_IP_duration;
            [MarshalAs(UnmanagedType.I8)]
            public long last_IP_pts;
            public IntPtr pAVIndexEntry;
            [MarshalAs(UnmanagedType.I4)]
            public int nb_index_entries;
            [MarshalAs(UnmanagedType.I4)]
            public int index_entries_allocated_size;
            [MarshalAs(UnmanagedType.I8)]
            public long nb_frames;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public long[] pts_buffer;
        }

        public struct AVFormatContext
        {
            public IntPtr pAVClass;
            public IntPtr pAVInputFormat;
            public IntPtr pAVOutputFormat;
            public IntPtr priv_data;
            public IntPtr pb;
            [MarshalAs(UnmanagedType.I4)]
            public int nb_streams;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public IntPtr[] streams;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public byte[] filename;
            [MarshalAs(UnmanagedType.I8)]
            public long timestamp;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] title;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] author;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] copyright;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] comment;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] album;
            [MarshalAs(UnmanagedType.I4)]
            public int year;
            [MarshalAs(UnmanagedType.I4)]
            public int tract;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] genre;
            [MarshalAs(UnmanagedType.I4)]
            public int ctx_flags;
            public IntPtr packet_buffer;
            [MarshalAs(UnmanagedType.I8)]
            public long start_time;
            [MarshalAs(UnmanagedType.I8)]
            public long duration;
            [MarshalAs(UnmanagedType.I8)]
            public long file_size;
            [MarshalAs(UnmanagedType.I4)]
            public int bit_rate;
            public IntPtr cur_st;
            public IntPtr cur_ptr;
            [MarshalAs(UnmanagedType.I4)]
            public int cur_len;
            public ubeat.Video.FFmpeg.AVPacket cur_pkt;
            [MarshalAs(UnmanagedType.I8)]
            public long data_offset;
            [MarshalAs(UnmanagedType.I4)]
            public int index_built;
            [MarshalAs(UnmanagedType.I4)]
            public int mux_rate;
            [MarshalAs(UnmanagedType.I4)]
            public int packet_size;
            [MarshalAs(UnmanagedType.I4)]
            public int preload;
            [MarshalAs(UnmanagedType.I4)]
            public int max_delay;
            [MarshalAs(UnmanagedType.I4)]
            public int loop_output;
            [MarshalAs(UnmanagedType.I4)]
            public int flags;
            [MarshalAs(UnmanagedType.I4)]
            public int loop_input;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U4)]
            public uint probesize;
            [MarshalAs(UnmanagedType.I4)]
            public int max_analyze_duration;
            public IntPtr key;
            [MarshalAs(UnmanagedType.I4)]
            public int keylen;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U4)]
            public uint nb_programs;
            public IntPtr programs;
            public ubeat.Video.FFmpeg.CodecID video_codec_id;
            public ubeat.Video.FFmpeg.CodecID audio_codec_id;
            public ubeat.Video.FFmpeg.CodecID subtitle_codec_id;
        }

        public struct AVPacketList
        {
            public ubeat.Video.FFmpeg.AVPacket pkt;
            public IntPtr next;
        }

        public struct AVImageInfo
        {
            public PixelFormat pix_fmt;
            [MarshalAs(UnmanagedType.I4)]
            public int width;
            [MarshalAs(UnmanagedType.I4)]
            public int height;
            [MarshalAs(UnmanagedType.I4)]
            public int interleaved;
            public ubeat.Video.FFmpeg.AVPicture pict;
        }

        public delegate int AllocCBCallback(IntPtr pVoid, IntPtr pAVImageInfo);

        public delegate int ImgProbeCallback(IntPtr pAVProbeData);

        public delegate int ImgReadCallback(IntPtr pByteIOContext, [MarshalAs(UnmanagedType.FunctionPtr)] ubeat.Video.FFmpeg.AllocCBCallback alloc_cb, IntPtr pVoid);

        public delegate int ImgWriteCallback(IntPtr pByteIOContext, IntPtr pAVImageInfo);

        public struct AVImageFormat
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string name;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string extensions;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ImgProbeCallback img_probe;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ImgReadCallback img_read;
            public int supported_pixel_formats;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.ImgWriteCallback img_write;
            public int flags;
            public IntPtr next;
        }

        public delegate string ItemNameCallback();

        public delegate int Read_PacketCallback(IntPtr opaque, IntPtr buf, int buf_size);

        public delegate long SeekCallback(IntPtr opaque, long offset, int whence);

        [CLSCompliant(false)]
        public delegate uint UpdateChecksumCallback(uint checksum, IntPtr buf, uint size);

        public delegate int WritePacketCallback(IntPtr opaque, IntPtr buf, int buf_size);

        public enum PixelFormat
        {
            PIX_FMT_NONE = -1,
            PIX_FMT_YUV420P = 0,
            PIX_FMT_YUYV422 = 1,
            PIX_FMT_RGB24 = 2,
            PIX_FMT_BGR24 = 3,
            PIX_FMT_YUV422P = 4,
            PIX_FMT_YUV444P = 5,
            PIX_FMT_RGB32 = 6,
            PIX_FMT_YUV410P = 7,
            PIX_FMT_YUV411P = 8,
            PIX_FMT_RGB565 = 9,
            PIX_FMT_RGB555 = 10,
            PIX_FMT_GRAY8 = 11,
            PIX_FMT_MONOWHITE = 12,
            PIX_FMT_MONOBLACK = 13,
            PIX_FMT_PAL8 = 14,
            PIX_FMT_YUVJ420P = 15,
            PIX_FMT_YUVJ422P = 16,
            PIX_FMT_YUVJ444P = 17,
            PIX_FMT_XVMC_MPEG2_MC = 18,
            PIX_FMT_XVMC_MPEG2_IDCT = 19,
            PIX_FMT_UYVY422 = 20,
            PIX_FMT_UYYVYY411 = 21,
            PIX_FMT_BGR32 = 22,
            PIX_FMT_BGR565 = 23,
            PIX_FMT_BGR555 = 24,
            PIX_FMT_BGR8 = 25,
            PIX_FMT_BGR4 = 26,
            PIX_FMT_BGR4_BYTE = 27,
            PIX_FMT_RGB8 = 28,
            PIX_FMT_RGB4 = 29,
            PIX_FMT_RGB4_BYTE = 30,
            PIX_FMT_NV12 = 31,
            PIX_FMT_NV21 = 32,
            PIX_FMT_RGB32_1 = 33,
            PIX_FMT_BGR32_1 = 34,
            PIX_FMT_NB = 35,
        }

        public struct AVClass
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string class_name;
            public ubeat.Video.FFmpeg.ItemNameCallback item_name;
            public IntPtr pAVOption;
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct AVOption
        {
        }

        public struct AVRational
        {
            [MarshalAs(UnmanagedType.I4)]
            public int num;
            [MarshalAs(UnmanagedType.I4)]
            public int den;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class ByteIOContext
        {
            public IntPtr buffer;
            [MarshalAs(UnmanagedType.I4)]
            public int buffer_size;
            public IntPtr buf_ptr;
            public IntPtr buf_end;
            public IntPtr opaque;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.Read_PacketCallback read_packet;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.WritePacketCallback write_packet;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.SeekCallback seek;
            [MarshalAs(UnmanagedType.I8)]
            public long pos;
            [MarshalAs(UnmanagedType.I4)]
            public int must_flush;
            [MarshalAs(UnmanagedType.I4)]
            public int eof_reached;
            [MarshalAs(UnmanagedType.I4)]
            public int write_flag;
            [MarshalAs(UnmanagedType.I4)]
            public int is_streamed;
            [MarshalAs(UnmanagedType.I4)]
            public int max_packet_size;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.U4)]
            public uint checksum;
            public IntPtr checksum_ptr;
            [CLSCompliant(false)]
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ubeat.Video.FFmpeg.UpdateChecksumCallback update_checksum;
            [MarshalAs(UnmanagedType.I4)]
            public int error;
        }
    }
}
