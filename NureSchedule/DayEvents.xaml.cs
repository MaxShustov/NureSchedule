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
    /// Логика взаимодействия для DayEvents.xaml
    /// </summary>
    public partial class DayEvents : UserControl
    {
        public DayEvents( List <TimeTableEvent> events )
        {
            InitializeComponent();
            TextBlock day = new TextBlock();
            FillTimeColume(day, 0, System.Windows.VerticalAlignment.Center);
            day.SetValue(Grid.ColumnProperty, 1);
            day.Text = events[0].StartTime.ToString(@"dd\.MM\, ddd");
            grid.Children.Add(day);
            for (int i = 0; i < 6; i++)
            {
                TextBlock startTime = new TextBlock();
                TextBlock endTime = new TextBlock();
                FillTimeColume(startTime, i + 1, System.Windows.VerticalAlignment.Top);
                FillTimeColume(endTime, i + 1, System.Windows.VerticalAlignment.Bottom);
                switch (i+1)
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
                EventPanel panel = new EventPanel ();
                if (events.Any(e => e.NumberOfPair == i + 1))
                    panel = new EventPanel(events.Single(e => e.NumberOfPair == i + 1));
                panel.SetValue(Grid.RowProperty, i + 1);
                panel.SetValue(Grid.ColumnProperty, 1);
                grid.Children.Add(panel);
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
