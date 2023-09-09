using System;

namespace PlanningPoker.FunctionalTests.Tests;

public static class TimeoutProvider
{
    public static TimeSpan GetDefaultTimeout()
    {
        return TimeSpan.FromSeconds(5);
    }
}