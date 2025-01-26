﻿using System;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace GClock
{
    public partial class MainWindow : Window
    {
        private System.Timers.Timer timer;
        private DateTime simulatedTime;
        private double dayDurationInMinutes = 60; // Default 60-minute day
        private string timeFormat = "HH:mm:ss";
        private bool isWindowClosing = false;
        List<DateTime> alarms = new List<DateTime>();   

        public MainWindow()
        {
            InitializeComponent();
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
            });
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
            isWindowClosing = true; // Prevent UI updates after closing
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
    }
}
