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
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using System.Diagnostics;

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
        private bool isMin = false;
        private bool shuffle = true;
        private string searchdir;
        KeyboardListener KListener = new KeyboardListener();

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
            if (files.Count == 0)
            {
                MessageBox.Show("You have no music to play!\nPlease put some music in " + searchdir + " and then run this application.\nPlease note, the file must be in .mp3 form", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
            myListBox.ItemsSource = files;
            //add media handler
            mediaPlayer.MediaEnded += onSongFinished;
            //advance song
            advanceSong();
            //start timer
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += timer_Tick;
            timer.Start();
            //Register hotkey
            KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
        }
        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            if (shuffle)
            {
                btnShuffle.Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51));
                shuffle = false;
            }
            else
            {
                btnShuffle.Foreground = new SolidColorBrush(Color.FromRgb(68, 187, 255));
                shuffle = true;
            }
        }
        private void onSongFinished(object sender, System.EventArgs e)
        {
            advanceSong();
        }
        private void advanceSong()
        {
            if (shuffle)
            {
                string song = files[rand.Next(0, files.Count)];
                myListBox.SelectedIndex = files.FindIndex(x => x.Equals(song));
            }
            else
            {
                if (files.Count > (myListBox.SelectedIndex + 1))
                {
                    myListBox.SelectedIndex = myListBox.SelectedIndex + 1;
                }
                else
                {
                    myListBox.SelectedIndex = 0;
                }
            }

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
        private void btnTgl_Click(object sender, RoutedEventArgs e)
        {
            if (isMin)
            {
                toggleSong.Content = "";
                MainWin.MaxHeight = int.MaxValue; //Because infinity doesn't work.
                MainWin.Height = 730;
                MainWin.MinHeight = 730;
                sBox.Visibility = Visibility.Visible;
                isMin = false;
            }
            else
            {
                sBox.Visibility = Visibility.Collapsed;
                toggleSong.Content = "";
                MainWin.MinHeight = 240;
                MainWin.Height = 240;
                MainWin.MaxHeight = 240;
                isMin = true;
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
                btnPlayPause.Content = ""; //Pause icon
            }
            else
            {
                mediaPlayer.Pause();
                isPaused = true;
                btnPlayPause.Content = ""; //Play icon
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

        // I wonder if antivirus programs will the catch the fact that I have a system-wide keylogger bundled in my app.
        // I hope not...
        private void KListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            if(args.Key == Key.MediaNextTrack)
            {
                advanceSong();
            }
            else if (args.Key == Key.MediaPlayPause)
            {
                togglePlayPause();
            }
            // It's not like I'm sending myself the info!
            // secretlySendEmailAboutWhatKeyAUserPressed(args.Key);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            KListener.Dispose();
        }
    }
}
