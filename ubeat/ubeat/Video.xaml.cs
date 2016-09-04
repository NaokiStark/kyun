using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ubeat
{
    /// <summary>
    /// Lógica de interacción para Video.xaml
    /// </summary>
    public partial class VideoP : UserControl
    {
        public VideoP()
        {
            InitializeComponent();
            Player.MediaEnded+=Player_MediaEnded;

            Player.Play();
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            Player.Position = TimeSpan.FromSeconds(0);
            Player.Play();
        }
    }
}
