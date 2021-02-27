using System;

namespace PlanningPoker.Server.Infrastructure
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