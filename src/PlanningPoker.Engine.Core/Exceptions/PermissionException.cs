using System;

namespace PlanningPoker.Engine.Core.Exceptions
{
    public class PermissionException : Exception
    {
        public PermissionException(string message) : base(message)
        {
        }
    }
}
