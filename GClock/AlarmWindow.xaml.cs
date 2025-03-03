using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace GClock
{
    public partial class AlarmWindow : Window
    {
        private DispatcherTimer holdTimer;
        private DispatcherTimer initialHoldTimer;
        private string actionType;
        private int hourMarked = 7;
        private int minuteMarked = 0;
        bool isRecurring = false;

        private Alarm existingAlarm; //for edit

        public AlarmWindow()
        {
            InitializeComponent();

            holdTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(75)
            };
            holdTimer.Tick += HoldTimer_Tick;

            initialHoldTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            initialHoldTimer.Tick += InitialHoldTimer_Tick;
        }

        public AlarmWindow(Alarm alarm)
        {
            InitializeComponent();

            holdTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(75)
            };
            holdTimer.Tick += HoldTimer_Tick;

            initialHoldTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            initialHoldTimer.Tick += InitialHoldTimer_Tick;

            AlarmWindowTitle.Text = "Edit Alarm";

            existingAlarm = alarm; 

            
            AlarmNameTextBox.Text = alarm.Name;
            hourMarked = alarm.Time.Hours;
            minuteMarked = alarm.Time.Minutes;
            isRecurring = alarm.IsRecurring;

            HourText.Text = hourMarked.ToString();
            MinuteText.Text = minuteMarked.ToString("00");
            RecurringToggle.IsChecked = isRecurring;

            
            foreach (ComboBoxItem item in SoundDropdown.Items)
            {
                if (item.Tag.ToString() == alarm.SoundFile)
                {
                    SoundDropdown.SelectedItem = item;
                    break;
                }
            }
        }

        private void HoldTimer_Tick(object sender, EventArgs e)
        {
            PerformAction();
        }

        private void InitialHoldTimer_Tick(object sender, EventArgs e)
        {
            initialHoldTimer.Stop(); 
            holdTimer.Start(); 
        }

        private void PerformAction()
        {
            switch (actionType)
            {
                case "HourUp": HourUp(); break;
                case "HourDown": HourDown(); break;
                case "MinuteUp": MinuteUp(); break;
                case "MinuteDown": MinuteDown(); break;
            }
        }

        private void HourUp() => UpdateHour((hourMarked + 1) % 24);
        private void HourDown() => UpdateHour((hourMarked + 23) % 24);
        private void MinuteUp() => UpdateMinute((minuteMarked + 1) % 60);
        private void MinuteDown() => UpdateMinute((minuteMarked + 59) % 60);

        private void UpdateHour(int newHour)
        {
            hourMarked = newHour;
            HourText.Text = hourMarked.ToString();
        }

        private void UpdateMinute(int newMinute)
        {
            minuteMarked = newMinute;
            MinuteText.Text = minuteMarked.ToString("00");
        }

        //Mouse down
        private void HourUp_PreviewMouseDown(object sender, MouseButtonEventArgs e) => StartHold("HourUp");
        private void HourDown_PreviewMouseDown(object sender, MouseButtonEventArgs e) => StartHold("HourDown");
        private void MinuteUp_PreviewMouseDown(object sender, MouseButtonEventArgs e) => StartHold("MinuteUp");
        private void MinuteDown_PreviewMouseDown(object sender, MouseButtonEventArgs e) => StartHold("MinuteDown");

        //Mouse Up
        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e) => StopHold();

        private void StartHold(string action)
        {
            actionType = action;
            PerformAction(); 

            initialHoldTimer.Start();
        }

        private void StopHold()
        {
            initialHoldTimer.Stop();
            holdTimer.Stop();
        }


        private void IsRecurring_Checked(object sender, RoutedEventArgs e) => isRecurring = true;
        private void IsRecurring_Unchecked(object sender, RoutedEventArgs e) => isRecurring = false;


        private void Cancel_Click(object sender, RoutedEventArgs e) => this.Close();

        private void SaveAlarm_Click(object sender, RoutedEventArgs e)
        {
            string alarmName = AlarmNameTextBox.Text;
            string soundFile = ((ComboBoxItem)SoundDropdown.SelectedItem)?.Tag?.ToString();
            TimeSpan alarmTime = new TimeSpan(hourMarked, minuteMarked, 0);

            if (existingAlarm != null)
            {
                existingAlarm.Name = alarmName;
                existingAlarm.Time = alarmTime;
                existingAlarm.SoundFile = soundFile;
                existingAlarm.IsRecurring = isRecurring;
            }
            else
            {
                Alarm newAlarm = new Alarm(alarmName, alarmTime, soundFile, isRecurring);
                ((App)Application.Current).Alarms.Add(newAlarm);
            }

            this.Close();
        }


    }
}
