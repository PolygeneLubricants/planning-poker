using System.Collections.Generic;
using Blazored.SessionStorage;

namespace PlanningPoker.Client.Storage
{
    public interface IServerSessionManager
    {
        bool TryGetSession(string serverId, out ServerSession? session);

        void SetSession(ServerSession session);
    }

    public class ServerSessionManager : IServerSessionManager
    {
        private readonly ISyncSessionStorageService _sessionStorage;
        private const string SessionStoreName = "ServerSessions";

        public ServerSessionManager(ISyncSessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public bool TryGetSession(string serverId, out ServerSession? session)
        {
            var sessions = _sessionStorage.GetItem<Dictionary<string, ServerSession>>(SessionStoreName);
            if (sessions != null && sessions.ContainsKey(serverId))
            {
                session = sessions[serverId];
                return true;
            }

            session = null;
            return false;
        }

        public void SetSession(ServerSession session)
        {
            var sessions = _sessionStorage.GetItem<Dictionary<string, ServerSession>>(SessionStoreName) ?? new Dictionary<string, ServerSession>();
            sessions.Add(session.ServerId, session);
            _sessionStorage.SetItem(SessionStoreName, sessions);
        }
    }
}