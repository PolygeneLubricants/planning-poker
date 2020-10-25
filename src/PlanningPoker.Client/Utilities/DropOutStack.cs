using System.Collections.Generic;

namespace PlanningPoker.Client.Utilities
{
    public class DropOutStack<T> : LinkedList<T>
    {
        private readonly int _capacity;

        public DropOutStack(int capacity)
        {
            _capacity = capacity;
        }

        public void Push(T item)
        {
            if (Count >= _capacity) RemoveLast();

            AddFirst(item);
        }
    }
}