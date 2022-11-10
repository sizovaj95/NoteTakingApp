using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteTakingApp
{
    internal class TimeManager : ITimeManager
    {
        public DateTime DateTimeNow()
        {
            return DateTime.Now;
        }
        public string DateToString(DateTime dateTime)
        {
            return dateTime.ToString("ddd, dd MMMM yyyy");
        }
        public string TimeToString(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm");
        }
    }
}
