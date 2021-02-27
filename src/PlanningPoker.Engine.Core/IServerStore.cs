using System;
using System.Collections.Generic;
using PlanningPoker.Engine.Core.Models;

namespace PlanningPoker.Engine.Core
{
    public interface IServerStore
    {
        PokerServer Create(IList<string> cardSet);

        PokerServer Get(Guid id);
        
        ICollection<PokerServer> All();

        void Remove(PokerServer server);
    }
}