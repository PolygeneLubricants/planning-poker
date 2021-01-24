using System;

namespace PlanningPoker.Engine.Core.Exceptions
{
    public class VoteException : Exception
    {
        public VoteException(string message) : base(message)
        {
        }
    }
}