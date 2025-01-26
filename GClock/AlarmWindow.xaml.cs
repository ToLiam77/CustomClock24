using System.Windows;
using System.Windows.Controls;

namespace GClock
{
    public partial class AlarmWindow : Window
    {
        public AlarmWindow()
        {
            InitializeComponent();
        }

        private void SaveAlarm_Click(object sender, RoutedEventArgs e)
        {
            string alarmName = AlarmNameTextBox.Text;

            string sound = ((ComboBoxItem)SoundDropdown.SelectedItem).Content.ToString();

            this.Close(); // Close window after saving
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void HourUp_Click(object sender, RoutedEventArgs e)
        {
        }

        private void HourDown_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MinuteUp_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MinuteDown_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}