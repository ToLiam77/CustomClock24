using System;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Windows.Controls;
using System.Windows.Media;

namespace GClock
{
    public class Alarm : INotifyPropertyChanged
    {
        private string name;
        private TimeSpan time;
        private string soundFile;
        private bool isRecurring;
        private MediaPlayer mediaPlayer;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(nameof(Name)); }
        }

        public TimeSpan Time
        {
            get => time;
            set { time = value; OnPropertyChanged(nameof(Time)); }
        }

        public string SoundFile
        {
            get => soundFile;
            set { soundFile = value; OnPropertyChanged(nameof(SoundFile)); }
        }

        public bool IsRecurring
        {
            get => isRecurring;
            set { isRecurring = value; OnPropertyChanged(nameof(IsRecurring)); }
        }

        public Alarm(string name, TimeSpan time, string soundFile, bool isRecurring)
        {
            this.name = name;
            this.time = time;
            this.soundFile = soundFile;
            this.isRecurring = isRecurring;
            mediaPlayer = new MediaPlayer();
        }

        public void PlaySound()
        {
            try
            {
                mediaPlayer.Volume = 1.0;
                mediaPlayer.Open(new Uri($"../../../{SoundFile}", UriKind.Relative));
                mediaPlayer.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }

        public void StopSound()
        {
            mediaPlayer.Stop();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
