using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace Alarm
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        ObservableCollection<Ticker> alarms;
        public ObservableCollection<Ticker> Alarms { get { return alarms; } set { alarms = value; OnPropertyChanged("Alarms"); } }
        Ticker minAlarm;
        public Ticker MinAlarm { get { return minAlarm; } set { minAlarm = value; OnPropertyChanged("MinAlarm"); } }
        readonly ICommand addAlarmCommand;
        public ICommand AddAlarmCommand { get { return addAlarmCommand; } }

        public ViewModel()
        {
            Alarms = new ObservableCollection<Ticker>();
            Alarms.CollectionChanged += new NotifyCollectionChangedEventHandler(Alarms_CollectionChanged);
            addAlarmCommand = new DelegateCommand(addAlarm);
            addAlarm(null);
        }

        void Alarms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            findMinAlarm();
        }

        void addAlarm(object o)
        {
            Ticker a = new Ticker();
            a.Closed += new EventHandler(a_Closed);
            a.PropertyChanged += new PropertyChangedEventHandler(a_PropertyChanged);
            Alarms.Add(a);
        }

        void findMinAlarm()
        {
            // Find the alarm with the minimum TimeLeft where its TimeLeft is > 0
            double min = Double.MaxValue;
            foreach (Ticker alarm in alarms)
            {
                if (alarm.TimeLeft != 0 && alarm.TimeLeft < min)
                {
                    min = alarm.TimeLeft;
                    minAlarm = alarm;
                }
            }
            if (min == Double.MaxValue)
                // if all are 0 then use alarm[0]
                minAlarm = alarms[0];
            OnPropertyChanged("MinAlarm");
        }

        void a_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "TimeLeft") return;
            Ticker a = (Ticker)sender;
            if (a == MinAlarm)
            {
                // if a was the MinAlarm (before PropertyChanged) then see if there is a new one now
                findMinAlarm();
            }
            else
                if (a.TimeLeft != 0 && a.TimeLeft < minAlarm.TimeLeft || minAlarm.TimeLeft == 0)
                {
                    // This alarm has the minimum TimeLeft
                    MinAlarm = a;
                }
        }

        void a_Closed(object sender, EventArgs e)
        {
            Ticker a = (Ticker)sender;
            Alarms.Remove(a);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
