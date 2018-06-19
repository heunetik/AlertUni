using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Input;

namespace Alarm
{
    class Ticker : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler Closed;
        DispatcherTimer timer;
        bool sound;
        public bool Sound { get { return sound; } set { sound = value; OnPropertyChanged("Sound"); } }
        double timeLeft;
        public double TimeLeft
        {
            get { return timeLeft; }
            set
            {
                timeLeft = value;
                dday = DateTime.Now + TimeSpan.FromMilliseconds(timeLeft);
                if (timeLeft > 0) timer.Start();
                OnPropertyChanged("TimeLeft");
            }
        }
        DateTime dday;              // Timpul la care sa oprim timer-ul
        readonly ICommand closeCommand;
        public ICommand CloseCommand { get { return closeCommand; } }

        public Ticker()
        {
            Sound = false;
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            closeCommand = new DelegateCommand(close);
            timeLeft = 0;
        }

        void close(object o)
        {
            timer.Stop();
            timer.Tick -= timer_Tick;
            OnClosed();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            double t = (dday - DateTime.Now).TotalMilliseconds;
            if (t <= 0)
            {
                // Oprim timer-ul cand ajungem la "dday", si setam TimeLeft sa fie zero
                TimeLeft = 0;
                timer.Stop();
            }
            else
            {
                // Timer-ul a facut un tick, deci nu facem update la dday
                timeLeft = t;
                OnPropertyChanged("TimeLeft");
            }
        }

        protected virtual void OnClosed()
        {
            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
