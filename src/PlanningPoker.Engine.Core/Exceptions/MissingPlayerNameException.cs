using System;

namespace PlanningPoker.Engine.Core.Exceptions
{
    public class MissingPlayerNameException : ArgumentException
    {
        public MissingPlayerNameException() : base("Player name must have a value.")
        {

        }
    }
}
