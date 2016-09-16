using System;

namespace VS_NETCORE.Models.Curation
{
    public class AvailabilityDateTime
    {
        private readonly DateTime _baseDateTime;

        public AvailabilityDateTime(DateTime baseDateTime)
        {
            _baseDateTime = baseDateTime;
        }

        private TimeSpan Offset => TimeSpan.FromHours(9);

        public DateTimeOffset Value => new DateTimeOffset(_baseDateTime, Offset);

        public string ClassName
        {
            get
            {
                switch (Value.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                        return "text-info";
                    case DayOfWeek.Sunday:
                        return "text-danger";
                    default:
                        return string.Empty;
                }
            }
        }

        public string Text => Value.ToString("yyyy年MM月dd日(dddd)");
    }
}