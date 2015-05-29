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
            foreach (var ev in events)
            {
                var panel = new EventPanel(ev);
                panel.SetValue(Grid.RowProperty, ev.NumberOfPair);
                panel.SetValue(Grid.ColumnProperty, 1);
                grid.Children.Add(panel);
            }
        }
    }
}
