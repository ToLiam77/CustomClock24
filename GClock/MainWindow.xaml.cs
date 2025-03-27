﻿using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GClock
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private System.Timers.Timer timer;
        private DateTime simulatedTime;
        private double dayDurationInMinutes = 60; // Default 60-minute day
        private string timeFormat = "HH:mm:ss";
        private bool isWindowClosing = false;



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isUIVisible = true;
        public bool IsUIVisible
        {
            get { return _isUIVisible; }
            set
            {
                _isUIVisible = value;
                OnPropertyChanged(nameof(IsUIVisible));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            AlarmsList.ItemsSource = ((App)Application.Current).Alarms;


            simulatedTime = new DateTime(1, 1, 1, 0, 0, 0);

            timer = new System.Timers.Timer(10);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            DayDurationSlider.ValueChanged += DayDurationSlider_ValueChanged;

            this.Closing += MainWindow_Closing;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isWindowClosing) return;

            Dispatcher.Invoke(() =>
            {
                double secondsPerRealSecond = (24 * 60 * 60) / (dayDurationInMinutes * 60);
                simulatedTime = simulatedTime.AddSeconds(secondsPerRealSecond * 0.01);
                ClockDisplay.Text = simulatedTime.ToString(timeFormat);

                CheckAlarms();
            });
        }

        private DateTime lastCheckedTime = DateTime.MinValue;

        private void CheckAlarms()
        {
            List<Alarm> alarmsToRemove = new List<Alarm>();

            foreach (var alarm in ((App)Application.Current).Alarms.ToList())
            {
                bool crossedMidnight = lastCheckedTime.TimeOfDay > simulatedTime.TimeOfDay;

                if ((lastCheckedTime.TimeOfDay < alarm.Time && simulatedTime.TimeOfDay >= alarm.Time)
                    || (crossedMidnight && alarm.Time <= simulatedTime.TimeOfDay))
                {
                    alarm.PlaySound();

                    if (!alarm.IsRecurring)
                    {
                        alarmsToRemove.Add(alarm);
                    }
                }
            }

            foreach (var alarm in alarmsToRemove)
            {
                ((App)Application.Current).Alarms.Remove(alarm);

                if (!AlarmsList.HasItems)
                {
                    AlarmsListPanel.Visibility = Visibility.Collapsed;
                    this.Height = 500;
                    ShowAlarmsToggle.IsChecked = false;
                }
            }

            lastCheckedTime = simulatedTime;
        }




        private void DayDurationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dayDurationInMinutes = e.NewValue;
            SliderValueText.Text = e.NewValue.ToString("0");
        }

        private void StopStartButton_Click(object sender, RoutedEventArgs e)
        {
            if (timer.Enabled)
            {
                timer.Stop();
                StopStartButton.Content = "Start";
                StopStartButton.Background = Brushes.LightGreen;
                StopStartButton.Foreground = Brushes.Black;
            }
            else
            {
                timer.Start();
                StopStartButton.Content = "Stop";
                StopStartButton.Background = Brushes.IndianRed;
                StopStartButton.Foreground = Brushes.WhiteSmoke;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            simulatedTime = new DateTime(1, 1, 1, 0, 0, 0);
            ClockDisplay.Text = simulatedTime.ToString(timeFormat);
        }

        private void IncludeSecondsToggle_Checked(object sender, RoutedEventArgs e)
        {
            timeFormat = "HH:mm:ss";
            ClockDisplay.Text = simulatedTime.ToString(timeFormat);
        }

        private void IncludeSecondsToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            timeFormat = "HH:mm";
            ClockDisplay.Text = simulatedTime.ToString(timeFormat);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isWindowClosing = true;
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
        }

        private void OpenAlarmWindow_Click(object sender, RoutedEventArgs e)
        {
            AlarmWindow alarmWindow = new AlarmWindow();
            alarmWindow.Show();
        }

        private void IncreaseTime_Click(object sender, RoutedEventArgs e)
        {
            simulatedTime = simulatedTime.AddHours(1);
            ClockDisplay.Text = simulatedTime.ToString(timeFormat);
        }

        private void DecreaseTime_Click(object sender, RoutedEventArgs e)
        {
            if (simulatedTime.Hour < 1)
            {
                simulatedTime = new DateTime(1, 1, 1, 0, 0, 0);
            }
            else
            {
                simulatedTime = simulatedTime.Subtract(TimeSpan.FromHours(1));
            }

            ClockDisplay.Text = simulatedTime.ToString(timeFormat);
        }

        private void ToggleVisibility_Click(object sender, RoutedEventArgs e)
        {
            IsUIVisible = !IsUIVisible;

            if (IsUIVisible)
            {
                VisibilityToggle.Icon = BootstrapIcons.Net.BootstrapIconGlyph.EyeSlashFill;
                ClockDisplay.FontSize = 80;
            }
            else
            {
                VisibilityToggle.Icon = BootstrapIcons.Net.BootstrapIconGlyph.EyeFill;
                ClockDisplay.FontSize = this.ActualWidth / 5;
            }
        }

        private void ShowAlarmsToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (AlarmsList.HasItems)
            {
                AlarmsListPanel.Visibility = Visibility.Visible;
                this.Height = 800;
            }
            else
            {
                ShowAlarmsToggle.IsChecked = false;
                MessageBox.Show("No Alarms Set");
            }

        }

        private void ShowAlarmsToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            AlarmsListPanel.Visibility = Visibility.Collapsed;

            this.Height = 500;
        }

        private void EditAlarm_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Alarm alarm)
            {
                AlarmWindow editWindow = new AlarmWindow(alarm);
                editWindow.ShowDialog();
            }
        }

        private void DeleteAlarm_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Alarm alarm)
            {
                ((App)Application.Current).Alarms.Remove(alarm);
            }

            if (!AlarmsList.HasItems)
            {
                AlarmsListPanel.Visibility = Visibility.Collapsed;
                this.Height = 500;
                ShowAlarmsToggle.IsChecked = false;
            }
        }
    }
}
