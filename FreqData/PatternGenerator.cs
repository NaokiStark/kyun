using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Troschuetz.Random.Generators;
using Un4seen.Bass;

namespace FreqData.Generator
{
    public class PatternGenerator
    {

        /// <summary>
        /// Bass reader
        /// </summary>
        Player player;

        BeatmapDiff Difficulty;

        TempoDivider Divider = TempoDivider.Four;

        // Dancing Mokeys

        string DancingMonkeysPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\DancingGorilla\bin\win32\DancingMonkeys.exe");

        string DancingMonkeysDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\DancingGorilla\bin\win32\");

        /// <summary>
        /// Maxs
        /// </summary>
        float[] maxs = new float[4] { 0, 0, 0, 0 };

        /// <summary>
        /// Frequencies
        /// </summary>
        Dictionary<int, int> freqs = new Dictionary<int, int>();

        int[] intervals = new int[4] { 0, 0, 0, 0 };

        bool[] sust = new bool[4] { false, false, false, false };

        List<float> quarters = new List<float>();

        /// <summary>
        /// For elapsed
        /// </summary>
        Stopwatch stw = new Stopwatch();

        public TestGenerator frmInstance;

        TimeSpan elapsed = TimeSpan.FromMilliseconds(0);

        public PatternGenerator()
        {
            player = new Player();

            freqs.Add(55, 250);
            freqs.Add(251, 2000);
            freqs.Add(2001, 4000);
            freqs.Add(4001, 20000);
        }

        internal async Task<SongPattern> Generate(string filename, BeatmapDiff dificulty = BeatmapDiff.Easy, TempoDivider divider = TempoDivider.Four)
        {
            Divider = divider;
            return await Generate(filename, dificulty);
        }

        internal async Task<SongPattern> Generate(string filename, BeatmapDiff dificulty = BeatmapDiff.Easy)
        {
            Difficulty = dificulty;
            return await Task<SongPattern>.Run(() =>
            {
                var patterns = new SongPattern { SongPath = filename };
                patterns.difficulty = dificulty;
                //get BPM and GAP

                try
                {

                    FileInfo file = new FileInfo(filename);
                    FileInfo bpmFile = new FileInfo(Path.Combine(file.DirectoryName, file.Name + ".kbpm"));

                    string[] timelineValues = new string[2];

                    if (!bpmFile.Exists)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"-- kbpm file not exist, generating and creating a new one --");
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine($"-- Getting BPM and GAP values --");
                        Console.WriteLine();
                        Console.WriteLine();

                        frmInstance?.inform(.1f, "Getting more about song, this will take so long");

                        timelineValues = getBPMAndGap(filename);

                        frmInstance?.inform(.9f, "Please wait...");

                        var fstream = bpmFile.Create();
                        //Save
                        var bytes1 = UTF8Encoding.UTF8.GetBytes(timelineValues[0] + "\n");
                        var bytes2 = UTF8Encoding.UTF8.GetBytes(timelineValues[1] + "\n");
                        fstream.Write(bytes1, 0, bytes1.Length);
                        fstream.Write(bytes2, 0, bytes2.Length);
                        fstream.Flush();
                        fstream.Close();
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"-- {bpmFile.Name} found, opening it --");
                        Console.WriteLine();
                        Console.WriteLine();
                        var streamreader = bpmFile.OpenText();
                        string[] data = streamreader.ReadToEnd().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                        timelineValues = data;
                    }

                    Console.WriteLine($"** BPM:{timelineValues[0]} | GAP: {timelineValues[1]}**");
                    frmInstance?.inform(.99f, $"** BPM:{timelineValues[0]} | GAP: {timelineValues[1]}**");
                    Console.WriteLine();

                    patterns = startMapping(filename, timelineValues);
                    patterns.SongPath = filename;
                    patterns.BPM = toFloat(timelineValues[0]);
                    patterns.BPM = (float)Math.Round(patterns.BPM, MidpointRounding.ToEven);
                    patterns.Offset = (int)((double)Math.Round(toDouble(timelineValues[1]), 4) * 1000d);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                }

                Console.WriteLine();

                Console.WriteLine($"======== End generating ============");

                Console.WriteLine($"-- Result: ");
                Console.WriteLine($"Objects count: {patterns.Patterns.Count}");

                return patterns;
            });
        }
        float[] fft = new float[2048];

        internal SongPattern startMapping(string filename, string[] timelineValues, float velocity = 1)
        {

            var tmpDict = new SongPattern();
            NR3Generator rn = new NR3Generator(448844);

            // create stream
            var stream = Bass.BASS_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN);

            int maxInrow = 4;



            float gap = (float)(Math.Round(toDouble(timelineValues[1]), 4) * 1000f);
            //gap -= 80;
            double bpm = toDouble(timelineValues[0]);

            string roundup = timelineValues[0].Split('.')[1];

            bool roundBPM = false;
            roundBPM = roundup.StartsWith("9") || roundup.StartsWith("9");



            if (roundBPM)
            {
                bpm = (int)(bpm) + 1;
            }
            else
            {

            }
            bpm = Math.Round(bpm, 0, MidpointRounding.AwayFromZero); // Round, but why

            float mspb = (float)Math.Round(60000d / bpm, 12);
            double mspbRounded = (double)Math.Round(mspb);
            double half = Math.Round(mspbRounded / 2d, 1, MidpointRounding.ToEven);
            double quarter = mspbRounded / 4d; ;

            // Idk if I really need it, maybe to make it harder
            double eighth = mspbRounded / 8d;

            //For division
            double sixteenth = mspbRounded / 16d;

            float nextBeat = gap;
            float nextHalf = gap;
            float nextQuarter = (float)(nextBeat + quarter);
            float nextEighth = (float)(nextBeat + eighth);
            float nextSixteenth = (float)(nextBeat + sixteenth);

            maxs = getMaxPeaks(filename);

            Console.WriteLine($"======== Starting Generator ============");


            int read = 0;

            var size = 2048;
            int bassdatafft = (int)BASSData.BASS_DATA_FFT512;
            switch (size)
            {
                case 256:
                    bassdatafft = (int)BASSData.BASS_DATA_FFT512;
                    break;
                case 512:
                    bassdatafft = (int)BASSData.BASS_DATA_FFT1024;
                    break;
                case 1024:
                    bassdatafft = (int)BASSData.BASS_DATA_FFT2048;
                    break;
                case 2048:
                    bassdatafft = (int)BASSData.BASS_DATA_FFT4096;
                    break;
            }
            BASS_CHANNELINFO info = new BASS_CHANNELINFO();
            Bass.BASS_ChannelGetInfo(stream, info);

            float bitRate = 576/*1152*/;

            bitRate = FFMpegMp3SkipFrame.getSkippingFrames(filename);

            float sampleRate = 0;

            Bass.BASS_ChannelGetAttribute(stream, BASSAttribute.BASS_ATTRIB_FREQ, ref sampleRate);

            float msStartPadding = (bitRate / sampleRate) * 1000f;


            // Add padding to gap

            gap += msStartPadding;
            gap -= 15f; // weirrrd offset

            float[] results = new float[freqs.Count];

            int length = 0;
            length = (int)Bass.BASS_ChannelSeconds2Bytes(stream, (quarter / 1000f));

            GCHandle hGC = GCHandle.Alloc(fft, GCHandleType.Pinned);

            int pos = 0;

            long duration = (long)(1000d * Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream, BASSMode.BASS_POS_BYTE)));
            mspb = (float)mspbRounded;
            getQuarters(mspbRounded, gap, duration);
            int qIndex = 0;

            float prc = 1f / (float)quarters.Count * 100f;

            var sleep = TimeSpan.FromTicks(1);

            float[] actualPeaks = new float[] { 0, 0, 0, 0 };
            while ((read = Bass.BASS_ChannelGetData(stream, hGC.AddrOfPinnedObject(), bassdatafft + (int)BASSData.BASS_DATA_FLOAT)) > 0)
            {

                Thread.Sleep(sleep);

                pos += read;

                float playerTime = (float)((decimal)Bass.BASS_ChannelBytes2Seconds(stream, pos) * (decimal)1000);

                //playerTime += 25;
                //playerTime += msStartPadding;

                prc = (float)(qIndex + 1) / (float)quarters.Count;

                if (playerTime < gap) continue;

                if (qIndex >= quarters.Count) continue;

                if (playerTime < quarters[qIndex]) continue;

                int index = 0;

                frmInstance?.inform(prc, "Making some patterns");

                //Get peaks
                foreach (KeyValuePair<int, int> pair in freqs)
                {
                    int bin1 = Un4seen.Bass.Utils.FFTFrequency2Index(pair.Key, size, info.freq);

                    int bin2 = Un4seen.Bass.Utils.FFTFrequency2Index(pair.Value, size, info.freq);

                    float maxP = 0;

                    float delta = bin2 - bin1;

                    for (int a = bin1; a <= bin2; a++)
                    {
                        float pval = fft[a];

                        if (pval > maxP && pval > -1) maxP = pval;

                    }

                    if(index > 1)
                    {
                        results[index] = (float)Math.Sqrt(maxP);
                    }
                    else{
                        results[index] = (float)Math.Sqrt(maxP);
                    }                    
                    index++;
                }

                //Get maxLevel
                float maxLevel = (float)Math.Max(results[0], results[1]);
                float maxLevel2 = (float)Math.Max(results[2], results[3]);
                maxLevel = (float)Math.Max(maxLevel, maxLevel2);

                if (maxLevel >= .0002f)
                {
                    ///Get data
                    float[] values = results;

                    int maxInLine = 0;
                    int maxInSame = 2;
                    switch (Difficulty)
                    {
                        case BeatmapDiff.Easy:
                            maxInSame = 1;
                            break;
                        case BeatmapDiff.Normal:
                            maxInSame = rn.Next(1, 3);
                            break;
                        case BeatmapDiff.Hard:
                            maxInSame = rn.Next(1, 3);
                            break;
                        case BeatmapDiff.Insane:
                            maxInSame = rn.Next(1, 4);
                            break;
                        case BeatmapDiff.Extra:
                            maxInSame = rn.Next(1, 5);
                            break;
                    }

                    int flval = rn.Next(0, 10);

                    bool[] used = new bool[4];
                    if (flval % 2 == 0)
                    {
                        for (int a = 0; a < values.Length; a++)
                        {

                            //if (maxInLine >= maxInSame) break;
                            //maxInLine++;
                            float val = values[a];

                            // Compare to max peak if peak (with % calculation for each range) and fix minimun required

                            float maxLessTenth = maxs[a] - getPrcFor(maxs[a], a);

                            if (val >= maxLessTenth && val > 0.03f /*&& maxs[a] > 0.03f*/)
                            {
                                if (val > maxs[a]) maxs[a] = val;
                                                                
                                int rowpos = a;
                                if(tmpDict.Patterns.Count > 1)
                                {
                                    var lastObj = tmpDict.Patterns.Last();
                                    if (lastObj.Key - quarters[qIndex] <= mspb * NoteDuration.Eighth)
                                    {
                                        rowpos = lastObj.Value;
                                    }
                                }

                                if (Difficulty == BeatmapDiff.Extra)
                                {
                                    
                                    while (used[rowpos = rn.Next(0, 4)]) { }
                                }
                                else if (Difficulty == BeatmapDiff.Insane)
                                {
                                    if (rn.Next(0, 10) % 2 == 0)
                                    {
                                        if (a == 0)
                                        {
                                            rowpos = rn.Next(0, 4);
                                        }
                                        else
                                        {
                                            while (used[rowpos = rn.Next(0, 4)])
                                            {
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!used[a])
                                            rowpos = a;
                                        else
                                            continue;

                                    }
                                }

                                used[rowpos] = true;

                                tmpDict.Add(quarters[qIndex], rowpos);

                                Console.WriteLine($"{val.ToString("0.000000000", CultureInfo.InvariantCulture)} | { getPrcFor(maxs[a], a).ToString("0.000000000", CultureInfo.InvariantCulture)}");
                            }
                        }
                    }
                    else
                    {
                        for (int a = values.Length - 1; a > -1; a--)
                        {

                            //if (maxInLine >= maxInSame) break;

                            //maxInLine++;

                            float val = values[a];

                            // Compare to max peak if peak (with % calculation for each range) and fix minimun required
                            float maxLessTenth = maxs[a] - getPrcFor(maxs[a], a);

                            if (val >= maxLessTenth && val > 0.03f /*&& maxs[a] > 0.03f*/)
                            {
                                if (val > maxs[a]) maxs[a] = val;

                                int rowpos = (Difficulty == BeatmapDiff.Easy) ? rn.Next(0, 4) : a;

                                if (Difficulty == BeatmapDiff.Extra)
                                {
                                    while (used[rowpos = rn.Next(0, 4)]) { }
                                }
                                else if (Difficulty == BeatmapDiff.Insane)
                                {
                                    if (rn.Next(0, 10) % 2 == 0)
                                    {
                                        if (a == 0)
                                        {
                                            rowpos = rn.Next(0, 4);
                                        }
                                        else
                                        {
                                            while (used[rowpos = rn.Next(0, 4)])
                                            {
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!used[a])
                                            rowpos = a;
                                        else
                                            continue; // make it easy?
                                    }
                                }

                                used[rowpos] = true;

                                //tmpDict.Add(quarters[qIndex], rowpos);
                                tmpDict.Add(quarters[qIndex], a);

                                Console.WriteLine($"{val.ToString("0.000000000", CultureInfo.InvariantCulture)} | { getPrcFor(maxs[a], a).ToString("0.000000000", CultureInfo.InvariantCulture)}");
                            }
                        }
                    }

                }

                float decay = 0.00008f;
                
                for (int d = 0; d < maxs.Length; d++)
                {
                    if (qIndex == 0) break;

                    maxs[d] -= (quarters[qIndex] - quarters[qIndex - 1]) * decay;
                    if (maxs[d] < 0.0000001) maxs[d] = 0.0000001f;
                }

                qIndex++;
            }

            hGC.Free();
            return tmpDict;
        }

        private void getQuarters(double mspb, float offset, long duration)
        {
            quarters.Clear();
            float time = offset;
            NR3Generator rn = new NR3Generator(457710052);
            while (time < duration)
            {
                float nDuration = NoteDuration.Half;
                float ticks = (1 / nDuration * 4); ;

                if (Divider == TempoDivider.Four)
                {
                    switch (Difficulty)
                    {
                        case BeatmapDiff.Easy:
                            if (rn.NextBoolean())
                            {
                                nDuration = NoteDuration.Quarter;
                            }
                            else
                            {
                                //This will make some sliders
                                nDuration = NoteDuration.Sixteenth;
                            }
                            break;
                        case BeatmapDiff.Normal:
                            if (rn.NextBoolean())
                            {
                                nDuration = NoteDuration.Eighth;
                            }
                            else
                            {
                                //This will make some sliders
                                nDuration = NoteDuration.Sixteenth;
                            }
                            break;
                        case BeatmapDiff.Hard:
                            int res = rn.Next(0, 4);
                            if (res == 1 || res == 2)
                            {
                                nDuration = NoteDuration.Eighth;
                            }
                            else
                            {
                                //This will make some sliders
                                nDuration = NoteDuration.Sixteenth;
                            }
                            break;
                        case BeatmapDiff.Insane:

                            int res2 = rn.Next(0, 5);
                            if (res2 == 0 || res2 == 1)
                            {//Slider
                                nDuration = NoteDuration.Sixteenth;
                            }
                            else
                            {
                                nDuration = NoteDuration.Eighth;
                            }
                            break;
                        case BeatmapDiff.Extra:
                            int res3 = rn.Next(0, 5);
                            if (res3 == 0 || res3 == 1)
                            {
                                nDuration = NoteDuration.Eighth;
                            }
                            else
                            {
                                nDuration = NoteDuration.Sixteenth;

                            }
                            //nDuration = NoteDuration.Sixteenth;

                            break;
                    }

                    ticks = (1 / nDuration * 4);

                    for (int tick = 0; tick < ticks; tick++)
                    {
                        quarters.Add(time + ((float)mspb * nDuration * (float)tick));
                    }
                }
                else
                {
                    switch (Difficulty)
                    {
                        case BeatmapDiff.Normal:
                            if (rn.NextBoolean())
                            {
                                nDuration = NoteDuration.Third;
                            }
                            break;
                        case BeatmapDiff.Hard:
                            int res = rn.Next(0, 2);
                            if (res == 1)
                            {
                                nDuration = NoteDuration.Sixth;
                            }
                            else
                            {
                                nDuration = NoteDuration.Twelfth;
                            }
                            break;
                        case BeatmapDiff.Insane:

                            int res2 = rn.Next(0, 5);
                            if (res2 == 0)
                            {
                                nDuration = NoteDuration.TwentyFourth;
                            }
                            else
                            {
                                nDuration = NoteDuration.Twelfth;
                            }
                            break;
                        case BeatmapDiff.Extra:
                            int res3 = rn.Next(0, 5);
                            if (res3 == 0 || res3 == 1)
                            {
                                nDuration = NoteDuration.TwentyFourth;
                            }
                            else
                            {
                                nDuration = NoteDuration.Twelfth;
                            }
                            //nDuration = NoteDuration.Sixteenth;

                            break;
                    }

                    ticks = (1 / nDuration * 3);

                    for (int tick = 0; tick < ticks; tick++)
                    {
                        quarters.Add(time + ((float)mspb * nDuration * (float)tick));
                    }
                }



                time += AddTime(NoteDuration.Whole, (float)mspb);
            }
        }

        private float getPrcFor(float val, int freqRange)
        {
            //10% by default
            float divider = .1f;
            
            switch (freqRange)
            {
                case 0:
                    divider = .001f;
                    break;
                case 1:
                    divider = .005f;
                    break;
                case 2:
                    divider = .03f;
                    break;
                case 3:
                    divider = .01f;
                    break;
            }

            return val * divider;
        }

        private float AddTime(float noteDuration, float mpb)
        {
            return noteDuration * mpb;
        }

        private float[] getMaxPeaks(string filename)
        {

            Console.WriteLine($"======== Getting max peaks ============");

            var peaks = new float[4];


            var stream = Bass.BASS_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN);

            int read = 0;

            var size = 1024;
            int bassdatafft = (int)BASSData.BASS_DATA_FFT512;
            switch (size)
            {
                case 256:
                    bassdatafft = (int)BASSData.BASS_DATA_FFT512;
                    break;
                case 512:
                    bassdatafft = (int)BASSData.BASS_DATA_FFT1024;
                    break;
                case 1024:
                    bassdatafft = (int)BASSData.BASS_DATA_FFT2048;
                    break;
                case 2048:
                    bassdatafft = (int)BASSData.BASS_DATA_FFT4096;
                    break;
            }

            BASS_CHANNELINFO info = new BASS_CHANNELINFO();
            Bass.BASS_ChannelGetInfo(stream, info);

            GCHandle hGC = GCHandle.Alloc(fft, GCHandleType.Pinned);

            while ((read = Bass.BASS_ChannelGetData(stream, hGC.AddrOfPinnedObject(), bassdatafft + (int)BASSData.BASS_DATA_FLOAT)) > 0)
            {
                int index = 0;
                foreach (KeyValuePair<int, int> pair in freqs)
                {
                    int bin1 = Utils.FFTFrequency2Index(pair.Key, size, info.freq);

                    int bin2 = Utils.FFTFrequency2Index(pair.Value, size, info.freq);

                    float maxP = 0;

                    float delta = bin2 - bin1;

                    for (int a = bin1; a <= bin2; a++)
                    {
                        float pval = fft[a];

                        if (pval > maxP && pval > -1) maxP = pval;

                    }

                    if (peaks[index] < (float)Math.Sqrt(maxP))
                    {
                        //peaks[index] = (float)Math.Sqrt(maxP);
                        peaks[index] = (float)maxP;
                    }

                    index++;
                }
            }

            hGC.Free();

            return peaks;
        }

        private float toFloat(string val)
        {
            return float.Parse(val, CultureInfo.InvariantCulture.NumberFormat);
        }

        private double toDouble(string val)
        {
            return double.Parse(val, CultureInfo.InvariantCulture.NumberFormat);
        }

        internal string[] getBPMAndGap(string filename)
        {

            Process pr = new Process();

            string outData = "";

            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = DancingMonkeysPath,
                Arguments = $"-ob \"{filename}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = DancingMonkeysDir,

            };

            pr.StartInfo = psi;

            pr.OutputDataReceived += (obj, args) =>
            {
                outData += args.Data + "\n";
                Console.WriteLine(args.Data);
            };

            pr.ErrorDataReceived += (obj, args) =>
            {

                Console.WriteLine(args.Data);
            };


            pr.Start();

            pr.PriorityClass = ProcessPriorityClass.High;

            pr.BeginOutputReadLine();

            pr.BeginErrorReadLine();

            pr.WaitForExit();



            /* -- String out data format --

              Calculated BPM: 179.987756
              Gap in seconds: 0.787914

            */

            string regexBPM = "Calculated BPM: (.*)";

            string regexGAP = "Gap in seconds: (.*)";

            Match mtcBPM = Regex.Match(outData, regexBPM, RegexOptions.IgnoreCase);

            Match mtcGap = Regex.Match(outData, regexGAP, RegexOptions.IgnoreCase);

            if (mtcBPM.Groups.Count < 2)
            {
                throw new Exception("BPM couldnt calculated");
            }

            if (mtcGap.Groups.Count < 2)
            {
                throw new Exception("GAP couldnt calculated");
            }

            Console.WriteLine(mtcBPM.Groups[1].Value);
            Console.WriteLine(mtcGap.Groups[1].Value);

            return new string[2] { mtcBPM.Groups[1].Value, mtcGap.Groups[1].Value };
        }
    }

    public enum BeatmapDiff
    {
        Easy,
        Normal,
        Hard,
        Insane,
        Extra,
    }

    internal enum TempoDivider
    {
        Three,
        Four
    }

    class NoteDuration
    {
        public static readonly float Whole = 4f;
        public static readonly float Half = 2f;
        public static readonly float Quarter = 1f;
        public static readonly float Eighth = 0.5f;
        public static readonly float Sixteenth = 0.25f;

        public static readonly float Third = 3;
        public static readonly float Sixth = 0.66666f;
        public static readonly float Twelfth = 0.33333f;
        public static readonly float TwentyFourth = 0.16665f;
    }
}
