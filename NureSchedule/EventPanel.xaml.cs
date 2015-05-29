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
            startTime.Text = local_event.StartTime.TimeOfDay.ToString();
            endTime.Text = local_event.EndTime.TimeOfDay.ToString();
        }
    }
}
