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
    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        private string Filename { get; set; }
        public string getFilename()
        {
            return Filename;
        }
        public void setFilename(string fname)
        {
            Filename = fname;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// With the amount of try-catches in this, I doubt it'll ever crash :D
    /// Whenever the app crashes, I just add another try-catch.
    /// try-catches are a programmer's best friend! :)
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        public List<Song> songs;
        protected List<Song> editedSongs = new List<Song>();
        private Random rand;
        private bool isPaused = false;
        private bool isMin = false;
        private bool shuffle = true;
        private string searchdir;
        private string prevSearchDir;
        private string defaultsearchdir;
        KeyboardListener KListener = new KeyboardListener();

        public MainWindow()
        {
            InitializeComponent();
            //load search dir
            searchdir = getSearchDir();
            //load songs and set in listbox
            rand = new Random();
            songList.IsReadOnly = false;
            songList.CanUserResizeRows = false;
            songList.CanUserAddRows = false;
            //add media handler
            mediaPlayer.MediaEnded += onSongFinished;
            songList.CellEditEnding += songList_EditedCell;
            // gen cols and advance song
            genCol();
            //start timer
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += timer_Tick;
            timer.Start();
            //Register hotkey
            KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
        }
        string getSearchDir()
        {
            // meant as portable friendly app
            // string configLocation = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)+"\\config.txt";
            if (!File.Exists("config.txt"))
            {
                defaultsearchdir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }
            else
            {
                // read the place from the file
                defaultsearchdir = File.ReadAllLines("config.txt")[0];
            }
            return defaultsearchdir;
        }
        private void genCol()
        {
            List<Song> old = songs;
            songs = new List<Song>();
            TagLib.File temp;
            foreach (string s in Directory.GetFiles(searchdir, "*.mp3"))
            {
                try
                {
                    temp = TagLib.File.Create(s);
                    Song song;
                    song = new Song { Artist = (temp.Tag.Performers.Length == 0 ? "Unknown" : string.Join(";", temp.Tag.Performers)),
                                      Title = (temp.Tag.Title == null ? System.IO.Path.GetFileNameWithoutExtension(s) : temp.Tag.Title),
                                      Album = (temp.Tag.Album),
                                      Genre = (string.Join(";", temp.Tag.Genres)) };
                    song.setFilename(System.IO.Path.GetFileName(s));
                    songs.Add(song);
                }
                catch (Exception e)
                {
                    MessageBox.Show("There was an error reading your music: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            if (songs.Count == 0)
            {
                MessageBox.Show("You have no music to play!\nPlease put some music in " + searchdir + " and then run this application.\nPlease note, the files must be in .mp3 form", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (searchdir == defaultsearchdir)
                {
                    Environment.Exit(0);
                }
                else
                {
                    searchdir = prevSearchDir;
                    songs = old;
                    return;
                }
            }
            songList.ItemsSource = songs;
            advanceSong();
        }
        private void refreshMusic(object whatever1, object whatever2)
        {
            // when running new list, I wonder if memory gets leaked... (both editedsongs and songs)
            editedSongs = new List<Song>();
            genCol();
        }
        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            if (shuffle)
            {
                btnShuffle.Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51));
                MenuShuffle.IsChecked = false;
                shuffle = false;
            }
            else
            {
                btnShuffle.Foreground = new SolidColorBrush(Color.FromRgb(68, 187, 255));
                MenuShuffle.IsChecked = true;
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
                Song song = songs[rand.Next(0, songs.Count)];
                songList.SelectedItem = song;
            }
            else
            {
                if (songs.Count > (songList.SelectedIndex + 1))
                {
                    songList.SelectedIndex = songList.SelectedIndex + 1;
                }
                else
                {
                    songList.SelectedIndex = 0;
                }
            }

        }
        private void playSong(Song song)
        {
            //Console.WriteLine(searchdir + "\\" + song.getFilename());
            mediaPlayer.Open(new System.Uri(searchdir + "\\" + song.getFilename()));
            ArtistName.Content = song.Artist;
            SongName.Content = song.Title;
            MainWin.Title = song.Artist + " - " + song.Title;
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
                MainWin.MinHeight = 300;
                sBox.Visibility = Visibility.Visible;
                isMin = false;
                CMenu.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                sBox.Visibility = Visibility.Collapsed;
                toggleSong.Content = "";
                MainWin.MinHeight = 240;
                MainWin.Height = 240;
                MainWin.MaxHeight = 240;
                isMin = true;
                CMenu.Visibility = System.Windows.Visibility.Collapsed;
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
                MenuPP.Header = "Pause";
            }
            else
            {
                mediaPlayer.Pause();
                isPaused = true;
                btnPlayPause.Content = ""; //Play icon
                MenuPP.Header = "Play";
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
            catch (Exception erreur) // Error? Just wait for the next timer tick.
            {
                
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            advanceSong();
        }

        private void songList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                playSong((Song)songList.SelectedItem);
            }
            catch (Exception erreur)
            {
                // invalid selection
            }
            
        }

        private void songList_EditedCell(object sender, System.EventArgs e)
        {
            if (editedSongs.Contains((Song)songList.SelectedItem))
            {

            }
            else
            {
                editedSongs.Add((Song)songList.SelectedItem);
            }
        }

        private void saveEdits_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Save changes to ID3 tags?\nWarning: If you have any tags other than Title, Artist, Album and Genre, they will be removed.", "Save", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                TagLib.File temp;
                List<Song> remove = new List<Song>();
                foreach (Song s in editedSongs)
                {
                    try
                    {
                        temp = TagLib.File.Create(searchdir + "\\" + s.getFilename());
                        temp.Tag.Performers = s.Artist.Split(';');
                        temp.Tag.Title = s.Title;
                        temp.Tag.Album = s.Album;
                        temp.Tag.Genres = s.Genre.Split(';');
                        temp.Save();
                    }
                    catch (IOException err)
                    {
                        MessageBox.Show("There was an error while saving the file: " + err.Message, "Save Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                foreach (Song s in remove)
                {
                    editedSongs.Remove(s);
                }
                remove.RemoveRange(0, remove.Count);
            }
            else
            {
                return;
            }
        }

        private void MusLocation_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog diag = new System.Windows.Forms.FolderBrowserDialog();
            diag.SelectedPath = searchdir;
            diag.Description = "Please select the folder to scan for music in:";
            diag.ShowDialog();
            prevSearchDir = searchdir;
            searchdir = diag.SelectedPath;
            refreshMusic(null, null);
        }

        // I wonder if antivirus programs will the catch the fact that technically, I have a system-wide keylogger bundled in my app.
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

        private void close(object sender, object something)
        {
            this.Close();
        }
    }
}
