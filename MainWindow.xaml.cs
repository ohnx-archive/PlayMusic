// I don't even know which ones are actually needed anymore...
using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;

namespace PlayMusic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        public List<string> files;
        private Random rand;
        private bool isPaused = false;
        //Change here if you want to use this app yourself
        private string searchdir;

        public MainWindow()
        {
            InitializeComponent();
            //load search dir
            searchdir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            //load songs and set in listbox
            rand = new Random();
            files = new List<string>();
            foreach (string s in Directory.GetFiles(searchdir, "*.mp3"))
            {
                files.Add(System.IO.Path.GetFileNameWithoutExtension(s));
            }
            myListBox.ItemsSource = files;
            //add media handler
            mediaPlayer.MediaEnded += onSongFinished;
            //advance song
            advanceSong();
            //start timer
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void onSongFinished(object sender, System.EventArgs e)
        {
            advanceSong();
        }
        private void advanceSong()
        {
            string song = files[rand.Next(0, files.Count)];
            myListBox.SelectedIndex = files.FindIndex(x => x.Equals(song));
        }
        private void playSong(string song)
        {
            mediaPlayer.Open(new System.Uri(searchdir+"\\"+song+".mp3"));
            string[] info = song.Split(new string[] { " - " }, StringSplitOptions.None);
            ArtistName.Content = info[0];
            info[1].Remove(0, 1);
            SongName.Content = info[1];
            MainWin.Title = song;
            mediaPlayer.Play();
            if (isPaused)
            {
                togglePlayPause();
            }
        }
        private void btnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            togglePlayPause();
        }
        private void togglePlayPause()
        {
            if (isPaused)
            {
                mediaPlayer.Play();
                isPaused = false;
                btnPlayPause.Content = "&#xF04C;";
            }
            else
            {
                mediaPlayer.Pause();
                isPaused = true;
                btnPlayPause.Content = "&#xF04B;";
            }
        }
        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (mediaPlayer.Source != null)
                    lblStatus.Content = String.Format("{0} / {1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                else
                    lblStatus.Content = "00:00 / 00:00";
            }
            catch (Exception erreur) //Error? Just wait for the next timer tick.
            {

            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            advanceSong();
        }

        private void myListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            playSong(files[myListBox.SelectedIndex]);
        }
    }
}
