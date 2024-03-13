using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static ItemEquipable;

public class CombatSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text _enemyNameTextUI;
    [SerializeField] private TMP_Text _enemyHPTextUI;
    [SerializeField] private TMP_Text _enemyDMGTextUI;
    [Space(7)]
    [SerializeField] private TMP_Text _playerHPTextUI;
    [SerializeField] private TMP_Text _playerDPTextUI;
    [Space(7)]
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _playerCombatStatsUI;
    [Header("PlayerStats")]
    public int playerHP = 10;
    public int playerDP = 0;
    [Header("CombatParams")]
    public float attackWaitTime = 1.0f;

    private Animator _cameraAnimator;
    private Animator _directionalLightsAnimator;

    private EnemyBehaviour _currentEnemy;
    public bool isPlayerTurn = true;
    
    public event EventHandler onEnemyDefeated;
    public event EventHandler onEnemyWon;

    public static CombatSystem instance { private set; get; }

    private void Awake()
    {
        _cameraAnimator = Camera.main.GetComponent<Animator>();
        _directionalLightsAnimator = GameObject.Find("Directional Light").GetComponent<Animator>();
        DisableUI();
        instance = this;
    }

    public void StartTheFight(GameObject enemyGO)
    {
        _currentEnemy = enemyGO.GetComponent<EnemyBehaviour>();
        EnableUI();
        UpdatePlayerAndEnemyUI();
        PlayerInventory.instance.TurnItemsToCards();
        PlayerInventory.instance.ShowCombatCards();
        _cameraAnimator.SetTrigger("EngageForCombat");
        _directionalLightsAnimator.SetTrigger("EngageForCombat");

        if (PlayerInventory.instance.CombatCardsCount == 0)
        {
            Invoke(nameof(PlayerMoveNull), 1.0f);
        }

        isPlayerTurn = true;
    }

    public void EndTheFight(bool isWin)
    {
        _currentEnemy.OnDefeated();
        DisableUI();
        PlayerInventory.instance.TurnCardsToItems();
        _cameraAnimator.SetTrigger("DisengageFromCombat");
        _directionalLightsAnimator.SetTrigger("DisengageFromCombat");

        if (isWin)
            onEnemyDefeated?.Invoke(this, EventArgs.Empty);
        else
            onEnemyWon?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerMoveNull()
    {
        PlayerMove(null);
    }

    public void PlayerMove(EquipableInfo equipableInfo = null)
    {
        if (equipableInfo != null)
        {
            if (equipableInfo.type.HasFlag(EquipableInfo.Type.Heal)) {
                playerHP += equipableInfo.restoreHp;
            }
            if (equipableInfo.type.HasFlag(EquipableInfo.Type.Defence)) {
                playerDP += equipableInfo.restoreDp;
            }
            if (equipableInfo.type.HasFlag(EquipableInfo.Type.Damage)) {
                PlayerAttacksWith(equipableInfo.damage);
            }

            isPlayerTurn = false;
            PlayerInventory.instance.HideCombatCards();
            UpdatePlayerAndEnemyUI();
        }
        else
        {
            isPlayerTurn = false;
        }

        StartCoroutine(PlayerMoveInternal());
    }

    private IEnumerator PlayerMoveInternal()
    {
        yield return new WaitForSeconds(attackWaitTime);

        // if enemy was killed or player lost
        if (GameManager.instance.gameState == GameManager.GameState.FightingEnemy)
        {
            EnemyAttacks(_currentEnemy.damage);
            yield return new WaitForSeconds(attackWaitTime);
            
            isPlayerTurn = true;
            PlayerInventory.instance.ShowCombatCards();
        }
    }

    public void EnemyAttacks(int dmg)
    {
        if (playerDP >= dmg)
        {
            playerDP -= dmg;
        }
        else
        {
            var dmgAfterDP = dmg - playerDP;
            playerDP = 0;
            playerHP -= dmgAfterDP;
        }

        if (playerHP <= 0 || PlayerInventory.instance.CombatCardsCount == 0)
        {
            EndTheFight(false);
        }
        UpdatePlayerAndEnemyUI();
    }

    public void PlayerAttacksWith(int dmg)
    {
        _currentEnemy.hp -= dmg;
        if (_currentEnemy.hp <= 0)
        {
            EndTheFight(true);
        }
        UpdatePlayerAndEnemyUI();
    }

    private void EnableUI()
    {
        _enemyNameTextUI.transform.parent.gameObject.SetActive(true);
        _playerCombatStatsUI.SetActive(true);

        _inventoryUI.SetActive(false);
    }

    private void DisableUI()
    {
        _enemyNameTextUI.transform.parent.gameObject.SetActive(false);
        _playerCombatStatsUI.SetActive(false);

        _inventoryUI.SetActive(true);
    }

    private void UpdatePlayerAndEnemyUI()
    {
        _enemyNameTextUI.text = _currentEnemy.enemyName;
        _enemyHPTextUI.text = _currentEnemy.hp.ToString();
        _enemyDMGTextUI.text = _currentEnemy.damage.ToString();

        _playerHPTextUI.text = playerHP.ToString();
        _playerDPTextUI.text = playerDP.ToString();
    }

    [ContextMenu("__DebugAttackEnemy1")]
    public void __DebugAttackEnemy1()
    {
        PlayerAttacksWith(1);
    }

    [ContextMenu("__DebugAttackEnemy2")]
    public void __DebugAttackEnemy2()
    {
        PlayerAttacksWith(2);
    }

    [ContextMenu("__DebugAttackEnemy5")]
    public void __DebugAttackEnemy5()
    {
        PlayerAttacksWith(5);
    }
}
