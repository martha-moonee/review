using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using System;
using Random = UnityEngine.Random;

[SingularBehaviour(true, false, false)]
public class CardsManager : Singleton<CardsManager>
{
    [SerializeField] Color outlineColor = Color.blue;
    [SerializeField] float outlineWidth = 20f;
    [SerializeField] float yPosition = 5f;
    [SerializeField] Transform allCardsParent;
    [SerializeField] Transform cardsToSpawnParent;

    [HideInInspector] public List<Card> allBoardCards = new List<Card>();
    [HideInInspector] public bool isMoving = false;

    public Action<Vector3> OnHighlightComplete;
    public Action OnMovementFinished { get; set; }
    public Action OnGoldCardSelected { get; set; }

    public Card selectedCard { get; private set; }
    public bool canSelect { get; private set; } = true;
    public Transform AllCardsParent { get { return allCardsParent; } }
    public float YPosition { get { return yPosition; } }

    public static Vector2 maxValues = new Vector2(-9f, 9f);

    public void BlockAllCards()
    {
        for (int index = 0; index < allBoardCards.Count; index++)
        {
            allBoardCards[index].IsBlocked = true;
        }
    }

    public void UnblockAllCards()
    {
        for (int index = 0; index < allBoardCards.Count; index++)
        {
            allBoardCards[index].IsBlocked = false;
        }
    }

    public IEnumerator RandomAllCardsOnTable()
    {
        for (int index = 0; index < allBoardCards.Count; index++)
        {
            Quaternion randQuaternion = new Quaternion(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180), 0);
            Vector3 randPosition = Vector3.zero + new Vector3(Random.Range(maxValues.x, maxValues.y),
            Random.Range(2f, yPosition + 2f), Random.Range(maxValues.x, maxValues.y));

            CardMovement(allBoardCards[index], randPosition, randQuaternion.eulerAngles, 0.2f);
            yield return new WaitForSeconds(0.2f);
        }
    }
    private Vector3 cardsSpawnPos = new Vector3(0, 2, -100);
    public IEnumerator AddCardToBoard(GameObject cardPrefab, int index, bool randomPos = false)
    {
        Vector3 randRotation;
        Vector3 randPosition;

        if (randomPos)
        {
            float zRotation = Random.value > 0.5 ? 0 : 180;
            randRotation = new Vector3(Random.Range(0, 0), Random.Range(-180, 180), zRotation);
            randPosition = Vector3.zero + new Vector3(Random.Range(maxValues.x, maxValues.y),
            Random.Range(2f, yPosition + 2f), Random.Range(maxValues.x, maxValues.y));

        }
        else
        {
            randPosition = cardsToSpawnParent.position + Vector3.up + Vector3.up * (index * 0.1f);
            randRotation = new Vector3(0, 0, 180);
        }

        var prefab = Instantiate(cardPrefab, cardsSpawnPos, cardPrefab.transform.rotation);
        //prefab.transform.eulerAngles = randRotation;

        float secs = Random.Range(0, .5f);
        yield return new WaitForSeconds(secs);

        prefab.transform.DOMove(randPosition, 2f);
        prefab.transform.DORotate(randRotation, 2f);

        prefab.transform.parent = allCardsParent;
        //prefab.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

        var card = DeckManager.Instance.CreateCard(prefab, index);
        allBoardCards.Add(card);
    }


    public void MoveCardToDeck(Card card)
    {
        LevelManager.Instance.StreakController.ResetStreak();
        card.gameObject.SetActive(true);

        Vector3 newPos = DeckManager.Instance.allDeckCards[DeckManager.Instance.allDeckCards.Count - 1].transform.localPosition + new Vector3(-.5f, 0, 0);
        Vector3 newRotation = DeckManager.Instance.allDeckCards[DeckManager.Instance.allDeckCards.Count - 1].transform.eulerAngles;

        DeckManager.Instance.allDeckCards.Add(card);
        card.transform.parent = DeckManager.Instance.CardDeckPosition;
        card.transform.localScale = DeckManager.Instance.allDeckCards[DeckManager.Instance.allDeckCards.Count - 1].transform.localScale;
        
        card.transform.DOLocalMove(newPos, 0.2f);
        card.transform.DORotate(newRotation, 0.2f);

        card.transform.eulerAngles = new Vector3(0, 0, 0);
        card.GetComponent<Outline>().enabled = false;
        card.CardState = CardState.InDeck;

        AudioManager.Instance.PlaySound(AudioModel.Instance.GetRandomPickUpSound());
    }

    public void MoveCardToBoard(Card card)
    {
        LevelManager.Instance.StreakController.ResetStreak();
        card.gameObject.SetActive(true);
        allBoardCards.Add(card);

        var position = (Vector3.zero + new Vector3(Random.Range(maxValues.x, maxValues.y),
        Random.Range(0, maxValues.y), Random.Range(maxValues.x, maxValues.y)));

        card.transform.parent = CardsManager.Instance.AllCardsParent;

        CardMovement(card, position, 0.2f);

        card.StartPosition = position;
        card.GetComponent<Outline>().enabled = false;
        card.CardState = CardState.OnBoard;

        AudioManager.Instance.PlaySound(AudioModel.Instance.GetRandomPickUpSound());
    }

    public void PlayWildCard()
    {
        if (PlayerDataController.Instance.GetPlayerCoins() < HintManager.Instance.GoldCardCost && !TutorialManager.Instance.IsActive)
            return;

        if (isMoving)
            return;
        
        if(!TutorialManager.Instance.IsActive)
            PlayerDataController.Instance.AddPlayerCoins(-HintManager.Instance.GoldCardCost);
        
        DeckManager.Instance.ReplaceTopCard(DeckManager.Instance.WildCard);
        DeckManager.Instance.SpawnNewWildCard();

        OnGoldCardSelected?.Invoke();
    }

    public void ChooseCard(Card card)
    {
        if (card.CardState == CardState.OnBoard || selectedCard != null)
        {
            DeckManager.Instance.ReplaceTopCard(card);
        }
        else if (card.CardState == CardState.InDeck)
        {
            if (!card.IsWild)
            {
                DeckManager.Instance.ReplaceCardFromDeck();
            }
            else if (card.IsWild)
            {
                if (isMoving || !(PlayerDataController.Instance.GetPlayerCoins() >= HintManager.Instance.GoldCardCost) ||
                    !(PlayerDataController.Instance.GetWildCardsCount() > 0))
                    return;

                if (PlayerDataController.Instance.GetWildCardsCount() > 0)
                    PlayerDataController.Instance.SetWildCardsCount(PlayerDataController.Instance.GetWildCardsCount() - 1);
                else PlayerDataController.Instance.AddPlayerCoins(-HintManager.Instance.GoldCardCost);

                DeckManager.Instance.ReplaceTopCard(card);
                DeckManager.Instance.SpawnNewWildCard();
            }
        }
    }
    public void SelectCard(Card card)
    {
        if (card == selectedCard || card.CardState != CardState.OnBoard || !canSelect || card.IsBlocked)
            return;

        DeselectCard();

        selectedCard = card;

        HighlightCard(card, 0.2f);
    }

    public void DeselectAllCards()
    {
        if (LevelManager.Instance.currentLevelState != LevelState.Playing || TutorialManager.Instance.IsActive)
            return;

        for (int index = 0; index < allBoardCards.Count; index++)
        {
            if (allBoardCards[index].CardState != CardState.OnBoard)
                continue;

            MoveCardDown(allBoardCards[index]);
            RemoveOutline(allBoardCards[index]);

            foreach (var sequence in allBoardCards[index].sequences)
            {
                sequence.Kill();
            }

            allBoardCards[index].transform.localScale = allBoardCards[index].StartScale;
        }

        selectedCard = null;
        canSelect = true;
    }

    public void DeselectCard()
    {
        if (selectedCard == null || selectedCard.CardState != CardState.OnBoard)
            return;

        MoveCardDown(selectedCard);
        RemoveOutline(selectedCard);

        selectedCard.transform.localScale = selectedCard.StartScale;

        selectedCard = null;
    }

    public void DeselectCard(Card card)
    {
        if (card == null || card.CardState != CardState.OnBoard)
            return;

        MoveCardDown(card);
        RemoveOutline(card);

        card.transform.localScale = card.StartScale;
    }

    #region Movement

    public void CardMovement(Card card, Vector3 newPosition, float seconds, float power = 5f)
    {
        var rigidBody = card.gameObject.GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        isMoving = true;

        card.transform.DOJump(newPosition, power, 1, seconds).OnComplete(
            () =>
            {
                card.transform.position = newPosition;

                rigidBody.useGravity = true;
                rigidBody.constraints = RigidbodyConstraints.None;
                isMoving = false;
            }
            );
    }

    public void CardMovement(Card card, Vector3 newPosition, Vector3 newRotation, float seconds, float power = 5f)
    {
        var rigidBody = card.gameObject.GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        isMoving = true;


        card.transform.DOMove(newPosition, seconds).OnComplete(
            () =>
            {
                card.transform.DORotate(newRotation, seconds).OnComplete(
                    () =>
                    {
                        card.transform.position = newPosition;

                        rigidBody.useGravity = true;
                        rigidBody.constraints = RigidbodyConstraints.None;
                        isMoving = false;
                    }
                    );
            }
            );
    }


    public void TopDeckMovement(Card card, Vector3 newPosition, float seconds, float power = 5f)
    {
        var rigidBody = card.gameObject.GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;

        isMoving = true;

        card.transform.DORotate(new Vector3(0, 180, 0), seconds);
        card.transform.DOJump(newPosition, power, 1, seconds).OnComplete(
            () =>
            {
                card.transform.position = newPosition;
                card.StartPosition = newPosition;

                rigidBody.useGravity = true;
                rigidBody.constraints = RigidbodyConstraints.FreezeAll;

                card.transform.parent = DeckManager.Instance.TopCardPosition;
                card.transform.localScale = card.StartScale;
                card.CardState = CardState.OnTop;

                if (DeckManager.Instance.topCard != null)
                    DeckManager.Instance.topCard.gameObject.SetActive(false);

                DeckManager.Instance.topCard = card;

                OnMovementFinished?.Invoke();
                isMoving = false;
            }
            );
    }

    public void MoveCardUp(Card card)
    {
        var rigidBody = card.gameObject.GetComponent<Rigidbody>();

        rigidBody.useGravity = false;
        card.gameObject.transform.position = new Vector3(card.StartPosition.x, yPosition, card.StartPosition.z);
    }

    public void HighlightCard(Card card, float seconds, float speed = 0.1f)
    {
        var rigidBody = card.gameObject.GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        var newPosition = new Vector3(card.StartPosition.x, yPosition, card.StartPosition.z);

        canSelect = false;

        card.sequences.Add(card.transform.DOMove(newPosition, seconds));
        card.sequences.Add(card.transform.DOScale(card.StartScale * 1.2f, seconds));
        card.sequences.Add(card.transform.DORotate(new Vector3(0, 180, 0), seconds).OnComplete(() => { canSelect = true; OnHighlightComplete?.Invoke(card.transform.position); }));

        float width = 0;
        card.sequences.Add(DOTween.To(() => width, x => width = x, outlineWidth, seconds).OnUpdate(() => { AddOutline(card, width); }));
    }

    public void MoveCardDown(Card card)
    {
        var rigidBody = card.gameObject.GetComponent<Rigidbody>();
        rigidBody.useGravity = true;
        rigidBody.constraints = RigidbodyConstraints.None;
    }

    #endregion

    #region Outline
    public void AddOutline(Card card)
    {
        AddOutline(card, outlineColor);
    }

    public void AddOutline(Card card, float width)
    {
        AddOutline(card, outlineColor, width);
    }

    public void AddOutline(Card card, Color color)
    {
        AddOutline(card, color, outlineWidth);
    }

    public void AddOutline(Card card, Color color, float width)
    {
        var outline = card.gameObject.GetComponent<Outline>();
        outline.enabled = true;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = color;
        outline.OutlineWidth = width;
    }

    public void RemoveOutline(Card card)
    {
        var outline = card.gameObject.GetComponent<Outline>();
        outline.enabled = false;
    }
    #endregion

}
