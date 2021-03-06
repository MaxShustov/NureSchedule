﻿using System;
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

        public EventPanel()
        {
            InitializeComponent();
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
                case Type.Consultation:
                    grid.Background = new SolidColorBrush(Colors.White);
                    break;
                case Type.Exam:
                    grid.Background = new SolidColorBrush(Colors.Blue);
                    break;
                case Type.Test:
                    grid.Background = new SolidColorBrush(Colors.Brown);
                    break;
            }
            string name = "";
            TimeTableEvent.NameOfType.TryGetValue ( local_event.Type, out name );
            subject.Text = localEvent.Subject.Item2 + " " + name;
            auditory.Text = localEvent.Auditory;
        }

        public void SetWidth(double width)
        {
            subject.Width = width;
            auditory.Width = width;
        }
    }
}
