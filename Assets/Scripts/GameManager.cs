using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _approveBttn;
    [SerializeField] private GameObject _declineBttn;
    [SerializeField] private GameObject _grabBttn;
    [SerializeField] private GameObject _fightBttn;
    public ConveyorController conveyorController;

    public bool inputIsBlocked { private set; get; }

    public enum GameState
    {
        WaitingForConveyor,
        ItemIsRegular,
        ItemIsGrabbable,
        ItemIsEnemy,
        FightingEnemy
    }

    private int _enemyItemLayer;
    private GameState _gameState = GameState.WaitingForConveyor;
    public GameState gameState => _gameState;

    public static GameManager instance { private set; get; }

    private void Awake()
    {
        _enemyItemLayer = LayerMask.NameToLayer("EnemyItem");
        instance = this;
    }

    private void Start()
    {
        UpdateUI();
        conveyorController.onFinishMoving += OnConveyorFinished;
        CombatSystem.instance.onEnemyDefeated += OnEnemyKilled;
        LevelFinishedScreen.instance.onScreenIsFinished += OnLevelFinishedScreen_Finished;
    }

    private void Update()
    {
        inputIsBlocked = conveyorController.IsMoving();
    }

    private void OnConveyorFinished(object sender, ConveyorController.FinishedMovingEventArgs args)
    {
        _gameState = GameState.ItemIsRegular;
        if (args.levelFinished)
        {
            _gameState = GameState.WaitingForConveyor;
            LevelFinishedScreen.instance.Show();
        }
        else if (args.item.TryGetComponent(out ItemEquipable equipBeh))
        {
            _gameState = GameState.ItemIsGrabbable;
        }
        else if (args.item.layer == _enemyItemLayer)
        {
            _gameState = GameState.ItemIsEnemy;
        }
        UpdateUI();
    }

    private void OnEnemyKilled(object sender, EventArgs args)
    {
        _gameState = GameState.WaitingForConveyor;
        conveyorController.ResumeConveyor();
        UpdateUI();
    }

    private void OnLevelFinishedScreen_Finished(object sender, EventArgs args)
    {
        LevelManager.instance.MoveToNextLevel();
        conveyorController.ResetMe();
    }

    public void UI_ApproveCurrentItem()
    {
        if (!inputIsBlocked)
        {
            _gameState = GameState.WaitingForConveyor;
            conveyorController.ResumeConveyor();
            UpdateUI();
        }
    }

    public void UI_DeclineCurrentItem()
    {
        if (!inputIsBlocked)
        {
            _gameState = GameState.WaitingForConveyor;
            conveyorController.ResumeConveyor();
            UpdateUI();
        }
    }

    public void UI_GrabCurrentItem()
    {
        if (!inputIsBlocked)
        {
            _gameState = GameState.WaitingForConveyor;
            var grabbedItem = conveyorController.GrabCurrentItem();
            PlayerInventory.instance.Put(grabbedItem.GetComponent<ItemEquipable>().info);
            Destroy(grabbedItem);
            conveyorController.ResumeConveyor();
            UpdateUI();
        }
    }

    public void UI_FightEnemy()
    {
        if (!inputIsBlocked)
        {
            _gameState = GameState.FightingEnemy;
            var grabbedItem = conveyorController.GrabCurrentItem();
            var spawnedEnemy = Instantiate(grabbedItem.GetComponent<ItemEnemy>().enemyPrefab);
            spawnedEnemy.transform.position = grabbedItem.transform.position;
            Destroy(grabbedItem);

            CombatSystem.instance.StartTheFight(spawnedEnemy);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        switch (_gameState)
        {
            case GameState.WaitingForConveyor:
                _approveBttn.SetActive(false);
                _declineBttn.SetActive(false);
                _grabBttn   .SetActive(false);
                _fightBttn  .SetActive(false);
                break;
            case GameState.ItemIsRegular:
                _approveBttn.SetActive(true);
                _declineBttn.SetActive(true);
                _grabBttn   .SetActive(false);
                _fightBttn  .SetActive(false);
                break;
            case GameState.ItemIsGrabbable:
                _approveBttn.SetActive(true);
                _declineBttn.SetActive(true);
                _grabBttn   .SetActive(true);
                _fightBttn  .SetActive(false);
                break; 
            case GameState.ItemIsEnemy:
                _approveBttn.SetActive(true);
                _declineBttn.SetActive(false);
                _grabBttn   .SetActive(false);
                _fightBttn  .SetActive(true);
                break; 
            case GameState.FightingEnemy: 
                _approveBttn.SetActive(false);
                _declineBttn.SetActive(false);
                _grabBttn   .SetActive(false);
                _fightBttn  .SetActive(false);
                break;
            default:
                throw new UnityException("Unknonw GameState");
        }
    }

}
