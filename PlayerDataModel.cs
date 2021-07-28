using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerDataModel
{
    public int iChestLevelsCount;
    public int iChestStarsCount;
    public int coinsCount;
    public int levelNumber;
    public int levelNumberCopy;
    public int playedGamesNumber;
    public int starsCount;
    public int openedLevelChestsCount;
    public int openedStarsChestsCount;
    public int maxLevelChestProgress;
    public BusterCount busterCount;
    public bool isTutorialCompleted;
    public bool isLooped;
    public bool showAds;
    public bool isGameRated;
    public List<int> levelStars;

    public PlayerDataModel()
    {
        coinsCount = 100;
        levelNumber = 1;
        playedGamesNumber = 1;
        openedLevelChestsCount = 0;
        openedStarsChestsCount = 0;
        starsCount = 0;
        levelNumberCopy = levelNumber;
        isTutorialCompleted = false;
        isLooped = false;
        showAds = true;
        isGameRated = false;
        busterCount = new BusterCount();
        levelStars = new List<int>();
    }

    public PlayerDataModel(int coinsCount, int levelNumber, int playedGamesNumber, 
        int undoCount, int shuffleCount, int hintCount, bool isTutorialCompleted, bool isLooped, 
        bool showAds, bool isGameRated, List<int> levelStars)
    {
        this.coinsCount = coinsCount; 
        this.levelNumber = levelNumber;
        this.levelNumberCopy = levelNumber;
        this.playedGamesNumber = playedGamesNumber;
        busterCount = new BusterCount(undoCount, shuffleCount, hintCount);
        this.isTutorialCompleted = isTutorialCompleted;
        this.isLooped = isLooped;
        this.showAds = showAds;
        this.isGameRated = isGameRated;
        this.levelStars = levelStars;
    }
}

[Serializable]
public class BusterCount
{
    public int undoCount;
    public int shuffleCount;
    public int hintCount;

    public BusterCount()
    {
        undoCount = 5;
        shuffleCount = 5;
        hintCount = 5;
    }
    public BusterCount(int undoCount, int shuffleCount, int hintCount)
    {
        this.undoCount = undoCount;
        this.shuffleCount = shuffleCount;
        this.hintCount = hintCount;
    }
}
