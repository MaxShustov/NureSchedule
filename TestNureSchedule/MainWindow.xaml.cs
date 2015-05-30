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
using NureSchedule;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace TestNureSchedule
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TimeTable timeTable = new TimeTable();

        IEnumerable<JToken> faculties;

        IEnumerable<JToken> directions;

        IEnumerable<JToken> groups;

        JToken faculty;

        JToken direction;

        JToken group;

        int year;

        public MainWindow()
        {
            InitializeComponent();
            loadInformation();
            facultyComboBox.SelectionChanged += facultyComboBox_SelectionChanged;
            directionComboBox.SelectionChanged += directionComboBox_SelectionChanged;
            groupComboBox.SelectionChanged += groupComboBox_SelectionChanged;
            yearComboBox.SelectionChanged += yearComboBox_SelectionChanged;
            directionComboBox.IsEnabled = false;
            groupComboBox.IsEnabled = false;
            for (int i = 1; i < 6; i++)
                yearComboBox.Items.Add(String.Format("{0}-й курс", i));
            yearComboBox.IsEnabled = false;
        }

        async void yearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (yearComboBox.SelectedIndex == -1)
                return;
            year = yearComboBox.SelectedIndex + 1;
            if (year < 5)
                directions = await timeTable.GetDirections(faculty["id"].ToString());
            else
                directions = await timeTable.GetSpecialities(faculty["id"].ToString());
            directionComboBox.ItemsSource = directions.Select(d => d["short_name"].ToString());
            directionComboBox.SelectedItem = -1;
            directionComboBox.IsEnabled = true;
        }

        private void groupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (groupComboBox.SelectedIndex == -1)
                return;
            group = groups.Single(gr => gr["name"].ToString() == groupComboBox.SelectedItem.ToString());
        }

        async private void directionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (directionComboBox.SelectedIndex == -1)
                return;
            direction = directions.First(dir => dir["short_name"].ToString() == directionComboBox.SelectedItem.ToString());
            if (year < 5)
                groups = (await timeTable.GetDirectionGroups(direction["id"].ToString(), faculty["id"].ToString())).
                    Where ( res => res ["name"].ToString ().Contains ( direction["short_name"].ToString () + "-" + TimeTable.GetCodeYear (year) )).ToList ();
            else
                groups = await timeTable.GetSpecialityGroups(direction["id"].ToString(), faculty["id"].ToString());
            groupComboBox.ItemsSource = groups.Select( g => g["name"].ToString () );
            groupComboBox.IsEnabled = true;
            groupComboBox.SelectedIndex = -1;
        }

        private void facultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            faculty = faculties.Single(result => result["short_name"].ToString() == facultyComboBox.SelectedItem.ToString());
            yearComboBox.IsEnabled = true;
            yearComboBox.SelectedIndex = -1;
            directionComboBox.IsEnabled = false;
            groupComboBox.IsEnabled = false;
        }

        async void loadInformation()
        {
            faculties = await timeTable.GetFaculties();
            foreach (var facult in faculties)
                facultyComboBox.Items.Add((string)facult["short_name"]);
        }

        private async void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            var res = (await timeTable.GetSchedule(group["id"].ToString())).ToList ();
            var panel = new TimeTableEvents(res);
            //var panel = new DayEvents (res.Where((TimeTableEvent elem, int index) => { return index < 3; }).ToList());
            panel.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            panel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            //panel.Width = 200;
            grid.Children.Add(panel);
        }
    }
}
