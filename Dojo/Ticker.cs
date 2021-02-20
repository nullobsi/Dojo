using System;
using System.ComponentModel;
using System.Timers;

namespace Dojo
{
    public class Ticker : INotifyPropertyChanged
    {
        public Ticker()
        {
            Timer timer = new Timer();
            timer.Interval = 1000.0;
            timer.Elapsed += new ElapsedEventHandler(this.Timer_Elapsed);
            timer.Start();
        }

        public DateTime Now => DateTime.Now;

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Now"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}