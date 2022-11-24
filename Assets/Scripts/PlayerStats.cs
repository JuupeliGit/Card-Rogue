using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public int maxHealth;
    public int currentHealth;

    public Vector2Int playerPosition;

    public List<Status> statusConditions = new List<Status>();
    public Transform conditionsList;

    public Transform playerSprite;
    private Camera cam;

    public SpriteRenderer pointer;
    public GameObject[] arrows;
    public GameObject[] cursors;

    public TMP_Text healthText;
    public Image healthBar;

    // Events
    public delegate void OnStep();
    public event OnStep event_OnStep;

    public delegate bool CheckMove();
    public event CheckMove event_CheckMove;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        cam = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 newPos = new Vector2Int(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));

        newPos.x = Mathf.Clamp(newPos.x, -5f, 4f);
        newPos.y = Mathf.Clamp(newPos.y, -3f, 4f);

        pointer.transform.position = new Vector2(newPos.x + 0.5f, newPos.y + 0.5f);
    }

    public void RandomizePlayerPosition()
    {
        playerPosition = EntityManager.instance.GetRandomEmptyTile();
        UpdateSpritePosition();
    }

    public void ModifyHealth(int amount, bool allowToPass)
    {
        if (currentHealth <= 0)
            return;

        currentHealth += amount;

        if(amount < 0)
            SoundManager.instance.PlaySound(0, 0.5f, 1f);
        else if(amount > 0)
            SoundManager.instance.PlaySound(0, 0.5f, 1f);

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        else if (currentHealth <= 0)
        {
            SoundManager.instance.PlaySound(3, 0.5f, 1f);
            currentHealth = 0;

            TurnManager.instance.GameOver();
        }

        UpdateUI();

        if (allowToPass)
            ActionManager.instance.actionFinished = true;
    }

    public void MovePlayer()
    {
        bool canMove = event_CheckMove == null ? true : false;

        if (canMove && AnywhereToMove)
            StartCoroutine(MoveAction());
        else
            ActionManager.instance.actionFinished = true;
    }

    public void Attack()
    {
        if (AnywhereToAttack)
            StartCoroutine(AttackAction());
        else
            ActionManager.instance.actionFinished = true;
    }

    public void ApplyStatusCondition(Status status)
    {
        // If condition exists already, just add the turns to it.
        foreach (Status c in statusConditions)
        {
            if (c.condition == status.condition)
            {
                c.turnsLeft += status.turnsLeft;
                c.UpdateUI();
                return;
            }
        }

        status.condition.EnableStatus();
        statusConditions.Add(status);

        status.conditionUI = (GameObject)Instantiate(status.condition.conditionUIPrefab, conditionsList);
        status.conditionUI.GetComponent<UIToolTip>().toolTipText = status.condition.toolTipText;
        status.UpdateUI();
    }

    public void StatusConditionTurns()
    {
        if (statusConditions.Count <= 0)
            return;

        for (int i = statusConditions.Count - 1; i >= 0; i--)
        {
            statusConditions[i].turnsLeft--;
            statusConditions[i].UpdateUI();

            if (statusConditions[i].turnsLeft <= 0)
            {
                statusConditions[i].condition.DisableStatus();
                Destroy(statusConditions[i].conditionUI);

                statusConditions.RemoveAt(i);
            }
        }
    }

    public void RemoveStatusConditions()
    {
        if (statusConditions.Count <= 0)
            return;

        for (int i = statusConditions.Count - 1; i >= 0; i--)
        {
            statusConditions[i].condition.DisableStatus();
            Destroy(statusConditions[i].conditionUI);

            statusConditions.RemoveAt(i);
        }
    }

    private IEnumerator MoveAction()
    {
        yield return new WaitForEndOfFrame();

        while (true)
        {
            ToggleArrows(true);

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int newPos = new Vector2Int(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
            float distance = Vector2.Distance(new Vector2(newPos.x + 0.5f, newPos.y + 0.5f), playerSprite.position);

            if (Dungeon.instance.GetTileAtPosition(newPos, true) == 0 && EntityManager.instance.GetIdAtPosition(newPos, true) == 0 && distance < 1.25f && distance > 0.5f)
            {
                yield return new WaitForEndOfFrame();

                playerPosition = new Vector2Int(newPos.x + 5, newPos.y + 3);
                UpdateSpritePosition();

                ToggleArrows(false);

                yield return new WaitForSeconds(0.25f);

                event_OnStep?.Invoke();

                break;
            }
        }
    }

    private IEnumerator AttackAction()
    {
        while (true)
        {
            ToggleCursors(true);

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int newPos = new Vector2Int(Mathf.FloorToInt(mousePos.x), Mathf.FloorToInt(mousePos.y));
            float distance = Vector2.Distance(new Vector2(newPos.x + 0.5f, newPos.y + 0.5f), playerSprite.position);

            if (EntityManager.instance.GetIdAtPosition(newPos, true) > 0 && distance < 1.25f && distance > 0.5f)
            {
                int entityId = EntityManager.instance.GetIdAtPosition(newPos, true);
                Entity target = EntityManager.instance.FindEntityById(entityId);

                if (target != null)
                {
                    EntityManager.instance.onEndSpriteAnimation += OnEndAction;
                    StartCoroutine(EntityManager.instance.AttackEntitySprite(playerSprite.gameObject, target.gameObject.transform.position));

                    target.ModifyHealth(-1);
                }

                ToggleCursors(false);

                yield return new WaitForEndOfFrame();

                break;
            }
        }
    }

    private void UpdateSpritePosition()
    {
        EntityManager.instance.onEndSpriteAnimation += OnEndAction;

        Vector2 pos = new Vector2(playerPosition.x - 5 + 0.5f, playerPosition.y - 3 + 0.5f);
        StartCoroutine(EntityManager.instance.MoveEntitySprite(playerSprite.gameObject, pos));
    }

    private void OnEndAction()
    {
        EntityManager.instance.onEndSpriteAnimation -= OnEndAction;
        ActionManager.instance.actionFinished = true;

        if (Dungeon.instance.GetHazardAtPosition(playerPosition, false) != null)
        {
            Dungeon.instance.GetHazardAtPosition(playerPosition, false).stats.OnStep();
            Dungeon.instance.SetHazardAtPosition(playerPosition, false, null);
        }
    }

    private void ToggleArrows(bool b)
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            if (!b)
                arrows[i].SetActive(false);
            else
            {
                Vector2Int arrowPos = new Vector2Int(Mathf.FloorToInt(arrows[i].transform.position.x), Mathf.FloorToInt(arrows[i].transform.position.y));
                bool showArrow = (Dungeon.instance.GetTileAtPosition(arrowPos, true) == 0 && EntityManager.instance.GetIdAtPosition(arrowPos, true) == 0);

                arrows[i].SetActive(showArrow);
            }
        }
    }

    private void ToggleCursors(bool b)
    {
        for(int i = 0; i < cursors.Length; i++)
        {
            if (!b)
                cursors[i].SetActive(false);
            else
            {
                Vector2Int cursorPos = new Vector2Int(Mathf.FloorToInt(cursors[i].transform.position.x), Mathf.FloorToInt(cursors[i].transform.position.y));
                bool showCursor = (EntityManager.instance.GetIdAtPosition(cursorPos, true) > 0);

                cursors[i].SetActive(showCursor);
            }
        }
    }

    private void UpdateUI()
    {
        healthText.text = currentHealth + "/" + maxHealth;
        healthBar.fillAmount = (float)currentHealth / maxHealth;
    }

    private bool AnywhereToMove
    {
        get
        {
            for(int i = 0; i < cursors.Length; i++)
            {
                Vector2Int checkPos = new Vector2Int(Mathf.FloorToInt(cursors[i].transform.position.x), Mathf.FloorToInt(cursors[i].transform.position.y));

                if (Dungeon.instance.GetTileAtPosition(checkPos, true) == 0 && EntityManager.instance.GetIdAtPosition(checkPos, true) == 0)
                    return true;
            }

            return false;
        }
    }

    private bool AnywhereToAttack
    {
        get
        {
            for (int i = 0; i < cursors.Length; i++)
            {
                Vector2Int checkPos = new Vector2Int(Mathf.FloorToInt(cursors[i].transform.position.x), Mathf.FloorToInt(cursors[i].transform.position.y));

                if (EntityManager.instance.GetIdAtPosition(checkPos, true) > 0)
                    return true;
            }

            return false;
        }
    }
}
