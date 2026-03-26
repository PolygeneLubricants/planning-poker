using Blazored.LocalStorage;

namespace PlanningPoker.Client.Storage
{
    public interface IUserPreferencesManager
    {
        string? GetLastUsername();

        void SetLastUsername(string username);
    }

    public class UserPreferencesManager : IUserPreferencesManager
    {
        private readonly ISyncLocalStorageService _localStorage;
        private const string LastUsernameKey = "LastUsername";

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
    }
}
