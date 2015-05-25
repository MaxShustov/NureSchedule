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
using NureTimeTable;
using Newtonsoft.Json.Linq;

namespace TestNureSchedule
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TimeTable timeTable = new TimeTable();

        IEnumerable<JToken> results;

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
            yearComboBox.IsEnabled = false;
        }

        async void yearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            year = yearComboBox.SelectedIndex + 1;
            if (year < 5)
                results = await timeTable.GetDirections(faculty["id"].ToString ());
            else
                results = await timeTable.GetSpecialities(faculty["id"].ToString());
            foreach (var direction in results)
                directionComboBox.Items.Add((string)direction["short_name"]);
            directionComboBox.IsEnabled = true;
        }

        async private void groupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            group = results.Single( gr => gr ["name"].ToString () == groupComboBox.SelectedItem.ToString () );
            timeTable.GetSchedule( group ["id"].ToString () );
        }

        async private void directionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            direction = results.First ( dir => dir ["short_name"].ToString () == directionComboBox.SelectedItem.ToString () );
            if (year < 5)
                results = (await timeTable.GetDirectionGroups(direction["id"].ToString (), faculty["id"].ToString ())).
                    Where ( res => res ["name"].ToString ().Contains ( direction["short_name"].ToString () + "-" + TimeTable.GetCodeYear (year) )).ToList ();
            else
                results = await timeTable.GetSpecialityGroups ( direction ["id"].ToString (), faculty ["id"].ToString (), year);
            foreach (var group in results)
                groupComboBox.Items.Add(group["name"].ToString ());
            groupComboBox.IsEnabled = true;
        }

        private void facultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            faculty = results.First(result => result["short_name"].ToString() == facultyComboBox.SelectedItem.ToString());
            for (int i = 1; i < 6; i++)
                yearComboBox.Items.Add(String.Format("{0}-й курс", i));
            yearComboBox.IsEnabled = true;
        }

        async void loadInformation()
        {
            results = await timeTable.GetFaculties();
            foreach (var facult in results)
                facultyComboBox.Items.Add((string)facult["short_name"]);
        }
    }
}
