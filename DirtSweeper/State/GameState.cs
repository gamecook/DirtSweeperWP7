using System;

namespace GameCook.DirtSweeper.State
{
    public class GameState : PersistentDataStorage
    {
        public int GetTotalGames()
        {
            return Restore<int>("totalGames");
        }

        public void SetTotalGames(int value)
        {
            Save("totalGames", value);
        }

        public void UpdateTotalGames()
        {
            SetTotalGames(GetTotalGames() + 1);
        }

        public int GetTotalBones()
        {
            return Restore<int>("totalBones");
        }

        public void SetTotalBones(int value)
        {
            Save("totalBones", value);
        }

        public void UpdateTotalBones(int value = 1)
        {
            SetTotalBones(GetTotalBones() + value);
        }

        public string GetLastActivity()
        {
            return Restore<string>("lastActivity");
        }

        public void SetLastActivity(string value)
        {
            Save("lastActivity", value);
        }

        

        public int GetRewardTotal(int level)
        {
            return (level * (level + 1)) * 4;  
        }

        public bool GetActiveGame()
        {
            return Restore<Boolean>("activeGame");
        }

        public void SetActiveGame()
        {
            Save("activeGame", true);
        }

        public void SetInactiveGame()
        {
            Save("activeGame", false);
        }

        public String GetActivePage()
        {
            var activePage = Restore<String>("activePage");
            return activePage == null ? "StartPage.xaml" : activePage;
        }

        public void SetActivePage(string value)
        {
            Save("activePage", value);
        }

        public void ClearActivePage()
        {
            Save("activePage", null);
        }

        public String GetGameState()
        {
            return Restore<String>("gameState");
        }

        public void SetGameState(string value)
        {
            Save("gameState", value);
        }

        public void ClearActiveGame()
        {
            Save("activeGame", false);
            Save("gameState", "");
        }

        public void IncreaseLevel()
        {
            Save("level", GetLevel()+1);
        }

        public void SetLevel(int value)
        {
            Save("level", value);
        }

        public int GetLevel()
        {
            var value = Restore<int>("level");
            return value;
        }

        public bool GetSoundMute()
        {
            return Restore<bool>("mute");
        }

        public void SetSoundMute(bool value)
        {
            Save("mute", value);
        }
    }
}