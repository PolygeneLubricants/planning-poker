using System;

namespace PlanningPoker.Server
{
    public interface IDateTimeProvider
    {
        public DateTime GetUtcNow();
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}