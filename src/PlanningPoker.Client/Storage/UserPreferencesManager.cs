using System;
using Blazored.LocalStorage;

namespace PlanningPoker.Client.Storage
{
    public interface IUserPreferencesManager
    {
        string? GetLastUsername();

        void SetLastUsername(string username);

        string? GetLastCardSet();

        void SetLastCardSet(string cardSet);
    }

    public class UserPreferencesManager : IUserPreferencesManager
    {
        private readonly ISyncLocalStorageService _localStorage;
        private const string LastUsernameKey = "LastUsername";
        private const string LastCardSetKey = "LastCardSet";

        public UserPreferencesManager(ISyncLocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public string? GetLastUsername()
        {
            try
            {
                return _localStorage.GetItemAsString(LastUsernameKey);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SetLastUsername(string username)
        {
            try
            {
                _localStorage.SetItemAsString(LastUsernameKey, username);
            }
            catch (Exception)
            {
                // Ignore exceptions, it's not crucial for core functionality
            }
        }
    
        public string? GetLastCardSet()
        {
            try
            {
                return _localStorage.GetItemAsString(LastCardSetKey);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SetLastCardSet(string cardSet)
        {
            try
            {
                _localStorage.SetItemAsString(LastCardSetKey, cardSet);
            }
            catch (Exception)
            {
                // Ignore exceptions, it's not crucial for core functionality
            }
        }
    }
}
