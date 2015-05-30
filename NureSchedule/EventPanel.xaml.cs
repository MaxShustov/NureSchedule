using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NureSchedule
{
    /// <summary>
    /// Логика взаимодействия для EventPanel.xaml
    /// </summary>
    public partial class EventPanel : UserControl
    {
        private TimeTableEvent localEvent;

        public DateTime StartTime { get { return localEvent.StartTime; } }

        public DateTime EndTime { get { return localEvent.EndTime; } }

        public EventPanel( int numberOfPair )
        {
            InitializeComponent();
            switch (numberOfPair)
            { 
                case 1:
                    startTime.Text = "07:45";
                    endTime.Text = "09:20";
                    break;
                case 2:
                    startTime.Text = "09:30";
                    endTime.Text = "11:05";
                    break;
                case 3:
                    startTime.Text = "11:15";
                    endTime.Text = "12:50";
                    break;
                case 4:
                    startTime.Text = "13:10";
                    endTime.Text = "14:45";
                    break;
                case 5:
                    startTime.Text = "14:55";
                    endTime.Text = "16:30";
                    break;
                case 6:
                    startTime.Text = "16:40";
                    endTime.Text = "18:15";
                    break;
            }
        }

        public EventPanel( TimeTableEvent local_event )
        {
            localEvent = local_event;
            InitializeComponent();
            switch (localEvent.Type)
            { 
                case Type.Laboratory:
                    grid.Background = new SolidColorBrush ( Colors.DarkViolet );
                    break;
                case Type.Practice:
                    grid.Background = new SolidColorBrush(Colors.Green);
                    break;
                case Type.Lecture:
                    grid.Background = new SolidColorBrush(Colors.Yellow);
                    break;
            }
            subject.Text = localEvent.Subject.Item2;
            auditory.Text = localEvent.Auditory;
            startTime.Text = local_event.StartTime.TimeOfDay.ToString(@"hh\:mm");
            endTime.Text = local_event.EndTime.TimeOfDay.ToString(@"hh\:mm");
        }

        public void SetWidth(double width)
        {
            subject.Width = width;
            auditory.Width = width;
        }
    }
}
