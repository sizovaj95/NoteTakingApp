using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteTakingApp
{
    public interface ITimeManager
    {
        DateTime DateTimeNow();
        string DateToStringWeek(DateTime dateTime);
        string DateToStringDash(DateTime dateTime);
        string TimeToString(DateTime dateTime);
        string TimeSpanToString(TimeSpan time);
    }
}
