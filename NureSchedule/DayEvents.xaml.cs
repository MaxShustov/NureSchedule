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
            for (int i = 0; i < 6; i++)
            {
                EventPanel panel = new EventPanel ( i + 1 );
                if (events.Any(e => e.NumberOfPair == i + 1))
                    panel = new EventPanel(events.Single(e => e.NumberOfPair == i + 1));
                panel.SetValue(Grid.RowProperty, i);
                grid.Children.Add(panel);
            }
        }
    }
}
