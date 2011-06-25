using System.IO.IsolatedStorage;

namespace GameCook.DirtSweeper.State
{
    public class PersistentDataStorage
    {
        public bool Save(string token, object value)
        {
            if (null == value)
                return false;

            IsolatedStorageSettings store = IsolatedStorageSettings.ApplicationSettings;
            if (store.Contains(token))
                store[token] = value;
            else
                store.Add(token, value);

            store.Save();
            return true;
        }

        public T Restore<T>(string token)
        {
            IsolatedStorageSettings store = IsolatedStorageSettings.ApplicationSettings;
            if (!store.Contains(token))
                return default(T);

            return (T) store[token];
        }
    }
}