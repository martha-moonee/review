using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SingularBehaviour(true, false, false)]
public class InputManager : Singleton<InputManager>
{
    private float timer = 0f;
    private float maxWaitTimer = 15f;
    private void Update()
    {
        CheckPlayerIdle();
        GetPlayerInput();
    }

    public void CheckPlayerIdle()
    {
        timer += Time.deltaTime;

        if (timer >= maxWaitTimer)
        {
            DeckManager.Instance.PlayShakeAnimation();
            timer = 0;
        }
    }

    public void GetPlayerInput()
    {
        if (LevelManager.Instance.currentLevelState != LevelState.Playing)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                TryHandleClick(touch.position);
            }
            if (touch.phase == TouchPhase.Ended)
            {
                HandleClick(touch.position);
                CardsManager.Instance.DeselectAllCards();
            }
            timer = 0;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryHandleClick(Input.mousePosition);
            timer = 0;
        }
        if (Input.GetMouseButtonUp(0))
        {
            HandleClick(Input.mousePosition);
            CardsManager.Instance.DeselectAllCards();
            timer = 0;
        }
    }

    public void TryHandleClick(Vector3 clickPosition)
    {
        Card clickedObject = CastRay(clickPosition);

        if (clickedObject != null)
        {
            CardsManager.Instance.SelectCard(clickedObject);
            return;
        }
    }

    public void HandleClick(Vector3 clickPosition)
    {
        Card clickedObject = CastRay(clickPosition);

        if (clickedObject != null)
        {
            CardsManager.Instance.ChooseCard(clickedObject);
            return;
        }

        Vector3 hitPosition = Camera.main.ScreenToWorldPoint(clickPosition);
    }

    //TODO: Write a method that return a GameObject not a Card or Tile 
    //TODO: Add layers mask
    public Card CastRay(Vector3 origin)
    {
        RaycastHit hit = new RaycastHit();

        Ray ray = Camera.main.ScreenPointToRay(origin);

        Debug.DrawRay(ray.origin, ray.direction, Color.green);
        //Collide with everything except level borders. Ignore this layer
        int layerIndex = ~(1 << LayerMask.NameToLayer("LevelBorders"));

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerIndex))
        {
            Card tile = hit.collider.gameObject.GetComponent<Card>();
            return tile;
        }
        else return null;
    }

}
