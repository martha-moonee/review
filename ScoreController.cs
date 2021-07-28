using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] Image scoreImage;
    [SerializeField] Image firstStarImage;
    [SerializeField] Image secondStarImage;
    [SerializeField] Image thirdStarImage;

    [Header("Point count")]
    [Space(30)]
    [SerializeField] float maxScore = 100f;
    [SerializeField] float currentScore = 100f;
    [SerializeField] int tick = 5;
    [SerializeField] float scoreByTick = 5;

    [SerializeField] float scoreByCard = 5;
    [SerializeField] float scoreBySequence = 15;

    [Header("Star count count")]
    [Space(30)]
    [SerializeField] float firstStarScore = 20f;
    [SerializeField] float secondStarScore = 50f;
    [SerializeField] float thirdStarScore = 80f;

    bool isReducing = false;

    public float ScoreByCard { get { return scoreByCard; } }
    public float ScoreBySequence { get { return scoreBySequence; } }

    private void Start()
    {
        currentScore = 0f;

        UpdateScoreBar();
        CheckStars();
    }

    public void StartScoreReducing()
    {
        if (!isReducing)
        {
            StartCoroutine(ReduceCurrentScore());
            isReducing = true;
        }

    }

    IEnumerator ReduceCurrentScore()
    {
        while (true)
        {
            yield return new WaitForSeconds(tick);
            
            if(!(LevelManager.Instance.currentLevelState == LevelState.Playing))
                continue;

            currentScore -= scoreByTick;
            currentScore = Mathf.Clamp(currentScore, 0, maxScore);
            UpdateScoreBar();
            CheckStars();
        }
    }
    public float GetScore()
    {
        return currentScore;
    }

    public void UpdateScoreBar()
    {
        scoreImage.fillAmount = currentScore / 100;
    }

    public int GetStarsInTheEnd()
    {
        int stars = (currentScore >= thirdStarScore ? 1 : 0) + (currentScore >= secondStarScore ? 1 : 0) + (currentScore >= firstStarScore ? 1 : 0);
        return stars;
    }

    public void CheckStars()
    {
        thirdStarImage.gameObject.SetActive(currentScore >= thirdStarScore);
        secondStarImage.gameObject.SetActive(currentScore >= secondStarScore);
        firstStarImage.gameObject.SetActive(currentScore >= firstStarScore);
    }
    public void AddScore(float scoreToAdd)
    {
        currentScore += scoreByTick;
        currentScore = Mathf.Clamp(currentScore, 0, maxScore);
        UpdateScoreBar();
        CheckStars();
    }
}
