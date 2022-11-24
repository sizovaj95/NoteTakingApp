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
        string DateToString(DateTime dateTime);
        string TimeToString(DateTime dateTime);
    }
}
