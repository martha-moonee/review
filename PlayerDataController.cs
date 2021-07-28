using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SingularBehaviour(true, true, false)]
public class PlayerDataController : Singleton<PlayerDataController>
{
    private PlayerDataModel playerData;
    public Action OnStarsCountChanged { get; internal set; }

    public void Awake()
    {
        if(playerData==null)
            CreatePlayerData();
        
        if (SaveController.Instance.CheckIfDataExist())
        {
            SaveController.Instance.LoadLocalData();
        }
    }

    public PlayerDataModel GetPlayerData()
    {
        return playerData;
    }

    public void SetPlayerData(PlayerDataModel playerData)
    {
        this.playerData = playerData;
    }

    public bool GetIsGameRated()
    {
        return playerData.isGameRated;
    }

    public void SetIsGameRated(bool isGameRated)
    {
        playerData.isGameRated = isGameRated;
    }

    public int GetLevelChestOpenedCount()
    {
        return playerData.openedLevelChestsCount;
    }

    public List<int> GetLevelStars()
    {
        return playerData.levelStars;
    }

    public void SetLevelStars(int stars)
    {
        if (playerData.levelStars == null) playerData.levelStars = new List<int>();

        playerData.levelStars.Add(stars);
    }

    public void SetLevelChestOpenedCount(int count)
    {
        playerData.openedLevelChestsCount = count;
    }

    public int GetStarChestOpenedCount()
    {
        return playerData.openedStarsChestsCount;
    }

    public void SetStarChestOpenedCount(int count)
    {
        playerData.openedStarsChestsCount = count;
    }

    public void SetAds(bool state)
    {
        playerData.showAds = state;
    }

    public bool GetAds()
    {
        return playerData.showAds;
    }

    public void SetMaxLevelChestProgressNumber(int count)
    {
        playerData.maxLevelChestProgress = count;
    }

    public int GetMaxLevelChestProgressNumber()
    {
        return playerData.maxLevelChestProgress;
    }

    public void SetPlayedGamesNumber(int count)
    {
        playerData.playedGamesNumber = count;
    }

    public int GetPlayedGamesNumber()
    {
        return playerData.playedGamesNumber;
    }

    public int GetBoosterCount(GameplayHelper boosterType)
    {
        switch (boosterType)
        {
            case GameplayHelper.Hint:
                return playerData.busterCount.hintCount;
            case GameplayHelper.Shuffle:
                return playerData.busterCount.shuffleCount;
            case GameplayHelper.Undo:
                return playerData.busterCount.undoCount;
        }

        return 0;
    }

    public void SetLevelCopyNumber(int index)
    {
        playerData.levelNumberCopy = index;
    }

    public void ChangeBoosterCount(GameplayHelper boosterType, int value)
    {
        switch (boosterType)
        {
            case GameplayHelper.Hint:
                playerData.busterCount.hintCount = value;
                break;
            case GameplayHelper.Shuffle:
                playerData.busterCount.shuffleCount = value;
                break;
            case GameplayHelper.Undo:
                playerData.busterCount.undoCount = value;
                break;
        }
    }

    public void SetIsLooped(bool isLooped)
    {
        playerData.isLooped = isLooped;
        
        if (playerData.isLooped)
            playerData.levelNumberCopy = playerData.levelNumber;
    }

    public bool GetIsLooped()
    {
        return playerData.isLooped;
    }

    public int GetLevelNumberCopy()
    {
        return playerData.levelNumberCopy;
    }

    public void CreatePlayerData()
    {
        playerData = new PlayerDataModel();
    }

    public bool GetIsTutorialCompleted()
    {
        return playerData.isTutorialCompleted;
    }

    public void SetIsTutorialCompleted(bool state)
    {
       playerData.isTutorialCompleted = state;
    }

    public void SavePlayerData()
    { 
    
    }

    public void LoadPlayerData()
    {

    }

    public void SetStarsCount(int count)
    {
        playerData.starsCount = count;
    }

    public int GetStarsCount()
    {
        return playerData.starsCount;
    }

    public int GetPlayerCoins()
    {
        return playerData.coinsCount;
    }

    public int GetPlayerLevelNumber()
    {
        return playerData.levelNumber;
    }
    public void SetPlayerCoins(int value)
    {
        playerData.coinsCount = value;
    }
    public void AddPlayerCoins(int value)
    {
        playerData.coinsCount += value;
    }

    public void SetPlayerLevelNumber(int value)
    {
        playerData.levelNumber = value;
    }
    #region IChest
    public void SetIChestLevelCount(int count)
    {
        playerData.iChestLevelsCount = count;
    }
    public int GetIChestLevelCount()
    {
        return playerData.iChestLevelsCount;
    }
    public void SetIChestStarsCount(int count)
    {
        playerData.iChestStarsCount = count;
    }
    public int GetIChestStarsCount()
    {
        return playerData.iChestStarsCount;
    }
    #endregion
}
