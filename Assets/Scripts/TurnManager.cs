using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public int turnDrawAmount;

    private bool stairs = false;

    public int floor;
    public int turns;

    public Button endTurnButton;
    public Button nextFloorButton;
    public TMP_Text floorInfoText;

    public CanvasGroup newCardCanvas;
    public UICard newCardUI;

    public CanvasGroup gameOverCanvas;

    private Card lootCard;

    // Events
    public delegate void OnTurnEnd();
    public event OnTurnEnd event_OnTurnEnd;

    public delegate void OnTurnStart();
    public event OnTurnStart event_OnTurnStart;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    private void Start()
    {
        ProceedToNextFloor();
    }

    public void StartPlayerTurn()
    {
        turns++;
        UpdateUI();

        for (int i = 0; i < turnDrawAmount; i++)
            Deck.instance.DrawCard();

        event_OnTurnStart?.Invoke();

        /*
        if (Deck.instance.hand.Count >= 3)
            Deck.instance.DrawCard();

        while (Deck.instance.hand.Count < 3)
        {
            Deck.instance.DrawCard();
        }
        */

        endTurnButton.interactable = true;
        nextFloorButton.interactable = true;
    }

    public void EndPlayerTurn()
    {
        endTurnButton.interactable = false;
        nextFloorButton.interactable = false;

        event_OnTurnEnd?.Invoke();

        Deck.instance.DiscardAll();

        while(Deck.instance.hand.Count > 3)
        {
            Deck.instance.DiscardRandom();
        }

        PlayerStats.instance.StatusConditionTurns();

        StartEnemyTurn();
    }

    public void StartEnemyTurn()
    {
        EntityManager.instance.StartEnemyTurn();
    }

    public void EndEnemyTurn()
    {
        StartPlayerTurn();
    }

    public void ProceedToNextFloor()
    {
        endTurnButton.interactable = false;
        nextFloorButton.interactable = false;

        nextFloorButton.gameObject.SetActive(false);
        stairs = false;

        floor++;
        turns = 0;

        PlayerStats.instance.ModifyHealth(2, false);
        PlayerStats.instance.RemoveStatusConditions();

        Deck.instance.ResetDeck();

        Dungeon.instance.GenerateDungeon();

        Invoke("StartPlayerTurn", 1f);
    }

    public void EnableStairs()
    {
        if (stairs)
            return;

        nextFloorButton.gameObject.SetActive(true);
        nextFloorButton.interactable = false;
        stairs = true;

        //Dungeon.instance.SpawnStairs();
    }

    private void UpdateUI()
    {
        floorInfoText.text = "Floor: " + floor + '\n' + "Turn: " + turns;
    }

    public void DropLoot(Card card)
    {
        lootCard = card;

        newCardUI.SetCardInfo(lootCard, 0);

        StartCoroutine(CanvasFadeIn());
    }

    public void TakeLoot()
    {
        if (lootCard != null)
            Deck.instance.AddNewCard(lootCard);

        lootCard = null;

        StartCoroutine(CanvasFadeOut());
    }

    public void LeaveLoot()
    {
        lootCard = null;

        StartCoroutine(CanvasFadeOut());
    }

    public void GameOver()
    {
        gameOverCanvas.gameObject.SetActive(true);
        StartCoroutine(GameOverFade());
    }

    private IEnumerator CanvasFadeIn()
    {
        yield return new WaitForSeconds(0.3f);

        newCardCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f;

        float elapsed = 0f;
        float time = 0.1f;

        while(elapsed < time)
        {
            float curAlpha = Mathf.Lerp(0f, 1f, elapsed / time);
            newCardCanvas.alpha = curAlpha;

            elapsed += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }

        newCardCanvas.alpha = 1f;
    }

    private IEnumerator CanvasFadeOut()
    {
        Time.timeScale = 1f;

        float elapsed = 0f;
        float time = 0.5f;

        while (elapsed < time)
        {
            float curAlpha = Mathf.Lerp(1f, 0f, elapsed / time);
            newCardCanvas.alpha = curAlpha;

            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        newCardCanvas.alpha = 0f;

        newCardCanvas.gameObject.SetActive(false);
    }

    private IEnumerator GameOverFade()
    {
        float elapsed = 0f;
        float time = 0.5f;

        yield return new WaitForSeconds(0.5f);

        while (elapsed < time)
        {
            float curAlpha = Mathf.Lerp(0f, 1f, elapsed / time);
            gameOverCanvas.alpha = curAlpha;

            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        gameOverCanvas.alpha = 1f;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }
}
