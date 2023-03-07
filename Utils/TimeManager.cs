
namespace NoteTakingApp
{
    public class TimeManager : ITimeManager
    {
        public DateTime DateTimeNow()
        {
            return DateTime.Now;
        }
        public string DateToStringWeek(DateTime dateTime)
        {
            return dateTime.ToString("ddd, dd MMMM yyyy");
        }

        public string DateToStringDash(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
        public string TimeToString(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm");
        }

        public string TimeSpanToString(TimeSpan time)
        {
            return time.ToString(@"hh\:mm");
        }
    }
}
