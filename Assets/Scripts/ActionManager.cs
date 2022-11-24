using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager instance;

    public bool cardBeingPlayed = false;
    public bool actionFinished = false;

    private UICard currentCard;
    public bool consumeCard;

    public GameObject tipText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    public void PlayCard(UICard handCard)
    {
        if (tipText.activeSelf)
            tipText.SetActive(false);

        currentCard = handCard;
        consumeCard = false;

        StartCoroutine(ActivateAbilities());
    }

    private IEnumerator ActivateAbilities()
    {
        cardBeingPlayed = true;

        TurnManager.instance.endTurnButton.interactable = false;
        TurnManager.instance.nextFloorButton.interactable = false;

        Card card = currentCard.card;

        for (int i = 0; i < card.abilities.Length; i++)
        {
            for (int j = 0; j < card.abilities[i].amount; j++)
            {
                actionFinished = false;

                card.abilities[i].ability.Activate();

                yield return new WaitUntil(() => actionFinished);
            }

            yield return new WaitForSeconds(0.25f);
        }

        if (!consumeCard)
            Deck.instance.Resolve(currentCard);
        else
            Deck.instance.Consume(currentCard);

        yield return new WaitForEndOfFrame();

        TurnManager.instance.endTurnButton.interactable = true;
        TurnManager.instance.nextFloorButton.interactable = true;

        if (Deck.instance.hand.Count <= 0)
            TurnManager.instance.EndPlayerTurn();

        yield return new WaitForEndOfFrame();

        cardBeingPlayed = false;
    }
}
