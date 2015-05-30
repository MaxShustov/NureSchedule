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
    /// Логика взаимодействия для WeekEvents.xaml
    /// </summary>
    public partial class TimeTableEvents : UserControl
    {
        Dictionary<DateTime, List<TimeTableEvent>> _events;

        public TimeTableEvents( List <TimeTableEvent> events )
        {
            _events = new Dictionary<DateTime, List<TimeTableEvent>>();
            InitializeComponent();
            #region Time Panel
            for (int i = 1; i < 7; i++)
            {
                TextBlock startTime = new TextBlock();
                TextBlock endTime = new TextBlock();
                FillTimeColume(startTime, i, System.Windows.VerticalAlignment.Top);
                FillTimeColume(endTime, i, System.Windows.VerticalAlignment.Bottom);
                switch (i)
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
                grid.Children.Add(startTime);
                grid.Children.Add(endTime);
                var date = DateTime.Now;
            }
            #endregion
            foreach (var e in events)
            {
                if (!_events.ContainsKey(e.StartTime.Date))
                {
                    var dayEvents = events.Where(de => de.StartTime.Date == e.StartTime.Date).ToList();
                    _events.Add(e.StartTime.Date, dayEvents);
                }
            }
            var startDay = new DateTime(2015, 5, 18);
            for (int i = 0; i < 5; i++)
            {
                var currentDay = startDay.AddDays(i);
                var dayEvents = _events[currentDay];
                TextBlock day = new TextBlock();
                day.SetValue(Grid.RowProperty, 0);
                day.SetValue(Grid.ColumnProperty, i + 1);
                day.TextAlignment = TextAlignment.Center;
                day.Margin = new Thickness(4, 2, 4, 2);
                day.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                day.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                day.Text = currentDay.ToString(@"dd\.MM\, ddd");
                grid.Children.Add(day);
                
                for (int j = 0; j < 6; j++)
                {
                    EventPanel panel = new EventPanel();
                    if (dayEvents.Any(e => e.NumberOfPair == j + 1))
                        panel = new EventPanel(dayEvents.Single(e => e.NumberOfPair == j + 1));
                    panel.SetValue(Grid.RowProperty, j + 1);
                    panel.SetValue(Grid.ColumnProperty, i + 1);
                    grid.Children.Add(panel);
                }
                //DayEvents dayEvent = new DayEvents(_events[new DateTime(2015, 6, 2)]);
                //dayEvent.SetValue(Grid.ColumnProperty, i + 1);
                //dayEvent.SetValue(Grid.RowProperty, 1);
                //dayEvent.SetValue(Grid.RowSpanProperty, 6);
                //grid.Children.Add(dayEvent);
            }
        }

        void FillTimeColume(TextBlock block, int row, System.Windows.VerticalAlignment aligment)
        {
            block.SetValue(Grid.RowProperty, row);
            block.TextAlignment = TextAlignment.Center;
            block.Margin = new Thickness(4, 2, 4, 2);
            block.VerticalAlignment = aligment;
            block.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
        }
    }
}
