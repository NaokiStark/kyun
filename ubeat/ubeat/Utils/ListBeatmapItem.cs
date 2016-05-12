using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ubeat.Utils
{
    public class ListBeatmapItem : TreeNode
    {
        public Beatmap.ubeatBeatMap Beatmap { get; set; }

    }
}
