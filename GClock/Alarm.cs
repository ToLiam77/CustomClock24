using System;
using System.IO;
using System.Media;
using System.Windows.Controls;
using System.Windows.Media;

namespace GClock
{
    public class Alarm
    {
        public string Name { get; set; }
        public TimeSpan Time { get; set; } 
        public string SoundFile { get; set; } 
        public bool IsRecurring { get; set; }

        private MediaPlayer mediaPlayer;

        public Alarm(string name, TimeSpan time, string soundFile, bool isRecurring)
        {
            Name = name;
            Time = time;
            SoundFile = soundFile;
            IsRecurring = isRecurring;
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
    }
}
