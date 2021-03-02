using System;

namespace PlanningPoker.Engine.Core.Exceptions
{
    public class ChangePlayerTypeException : Exception
    {
        public ChangePlayerTypeException(string message) : base(message)
        {
        }
    }
}