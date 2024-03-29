using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Bttns")]
    [SerializeField] private GameObject _approveBttn;
    [SerializeField] private GameObject _declineBttn;
    [SerializeField] private GameObject _grabBttn;
    [SerializeField] private GameObject _fightBttn;
    [Header("Screens")]
    [SerializeField] private LevelFinishedScreen _levelFinishedScreen;
    [SerializeField] private LevelFinishedScreen _combatLostScreen;
    [SerializeField] private TaskScreen _taskScreen;
    [Header("Economy")]
    [SerializeField] private RectTransform _coinsContainerUI;
    [SerializeField] private TMP_Text _coinsTextUI;
    [Header("Audio")]
    [SerializeField] private AudioClip _bttnClickAudio;
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

    private int _playerCoins = 0;

    public static GameManager instance { private set; get; }
    
    
    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        _enemyItemLayer = LayerMask.NameToLayer("EnemyItem");
        instance = this;
    }

    private void Start()
    {
        UpdateUI();
        conveyorController.onFinishMoving += OnConveyorFinished;
        CombatSystem.instance.onEnemyDefeated += OnEnemyKilled;
        CombatSystem.instance.onEnemyWon += OnEnemyWon;
        _levelFinishedScreen.onScreenIsFinished += OnLevelFinishedScreen_Finished;
        _combatLostScreen.onScreenIsFinished += OnCombatLostScreen_Finished;
        _taskScreen.onScreenIsFinished += OnTaskScreen_Finished;
        Invoke(nameof(StartTheGame), 0.5f);
    }

    private void StartTheGame()
    {
        _taskScreen.Show();
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
            _levelFinishedScreen.Show("Day " + (LevelManager.instance.currentLevel + 1));
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

    private void OnEnemyWon(object sender, EventArgs args)
    {
        _playerCoins /= 4;
        _gameState = GameState.WaitingForConveyor;
        //_combatLostScreen.Show("Enemy Won");
        var screenTitle = "You were Defeated!";
        var screenDescription = "";
        if (PlayerInventory.instance.CombatCardsCount <= 0)
        {
            screenDescription = "You had no equipment!\n...and, monster took some of your money";
        }
        if (LevelManager.instance.levelIsTutorial)
        {
            screenTitle = "Oops!";
            screenDescription = "Don't worry, this is not your fault\nYou had no equipment!\n...also, monster took some of your money";
        }
        _combatLostScreen.Show(screenTitle, screenDescription);

        UpdateUI();
    }

    private void OnLevelFinishedScreen_Finished(object sender, EventArgs args)
    {
        LevelManager.instance.MoveToNextLevel();
        _taskScreen.Show();
    }

    private void OnCombatLostScreen_Finished(object sender, EventArgs args)
    {
        _levelFinishedScreen.Show("Day " + (LevelManager.instance.currentLevel + 1));
    }

    private void OnTaskScreen_Finished(object sender, EventArgs args)
    {
        conveyorController.ResetMe();
    }

    public void UI_ApproveCurrentItem()
    {
        if (!inputIsBlocked)
        {
            audioSource.PlayOneShot(_bttnClickAudio);

            var currItem = conveyorController.PeekCurrentItem().GetComponent<ConveyorItem>();
            var meetsObjectives = LevelManager.instance.ItemMeetsObjectives(true, currItem);
            var reward = meetsObjectives ? currItem.price : 0;
            if (currItem.gameObject.TryGetComponent(out ItemEquipable equip))
            {
                reward = currItem.price / 2;
            }

            _playerCoins += reward;
            _gameState = GameState.WaitingForConveyor;
            conveyorController.ResumeConveyor();
            UpdateUI();
        }
    }

    public void UI_DeclineCurrentItem()
    {
        if (!inputIsBlocked)
        {
            audioSource.PlayOneShot(_bttnClickAudio);

            var currItem = conveyorController.PeekCurrentItem().GetComponent<ConveyorItem>();
            var meetsObjectives = LevelManager.instance.ItemMeetsObjectives(false, currItem);
            _playerCoins += meetsObjectives ? currItem.price : 0;
            _gameState = GameState.WaitingForConveyor;
            conveyorController.ResumeConveyor();
            UpdateUI();
        }
    }

    public void UI_GrabCurrentItem()
    {
        if (!inputIsBlocked)
        {
            audioSource.PlayOneShot(_bttnClickAudio);

            var grabbedItem = conveyorController.PeekCurrentItem();
            var equipmentInfo = grabbedItem.GetComponent<ItemEquipable>().info;
            if (equipmentInfo.cost == 0 || equipmentInfo.cost <= _playerCoins)
            {
                _gameState = GameState.WaitingForConveyor;
                PlayerInventory.instance.Put(equipmentInfo);
                Destroy(grabbedItem);
                conveyorController.ResumeConveyor();
                _playerCoins -= equipmentInfo.cost;
                UpdateUI();
            }
            else
            {
                Debug.Log("Can't grab cost too high");
            }
        }
    }

    public void UI_FightEnemy()
    {
        if (!inputIsBlocked)
        {
            audioSource.PlayOneShot(_bttnClickAudio);

            _gameState = GameState.FightingEnemy;
            var grabbedItem = conveyorController.PeekCurrentItem();
            var spawnedEnemy = Instantiate(grabbedItem.GetComponent<ItemEnemy>().enemyPrefab);
            spawnedEnemy.transform.position = grabbedItem.transform.position;
            Destroy(grabbedItem);

            CombatSystem.instance.StartTheFight(spawnedEnemy);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        _coinsTextUI.text = _playerCoins.ToString();
        switch (_gameState)
        {
            case GameState.WaitingForConveyor:
                _approveBttn.SetActive(false);
                _declineBttn.SetActive(false);
                _grabBttn   .SetActive(false);
                _fightBttn  .SetActive(false);
                _coinsContainerUI.gameObject.SetActive(true);
                break;
            case GameState.ItemIsRegular:
                _approveBttn.SetActive(true);
                if (LevelManager.instance.removeDeclineButton) // THIS IS FOR FIRST LEVEL
                    _declineBttn.SetActive(false);
                else    
                    _declineBttn.SetActive(true);
                _grabBttn   .SetActive(false);
                _fightBttn  .SetActive(false);
                _coinsContainerUI.gameObject.SetActive(true);
                break;
            case GameState.ItemIsGrabbable:
                _approveBttn.SetActive(true);
                _declineBttn.SetActive(false);
                _grabBttn   .SetActive(true);
                _fightBttn  .SetActive(false);
                _coinsContainerUI.gameObject.SetActive(true);
                break; 
            case GameState.ItemIsEnemy:
                _approveBttn.SetActive(false);
                _declineBttn.SetActive(false);
                _grabBttn   .SetActive(false);
                _fightBttn  .SetActive(true);
                _coinsContainerUI.gameObject.SetActive(true);
                break; 
            case GameState.FightingEnemy: 
                _approveBttn.SetActive(false);
                _declineBttn.SetActive(false);
                _grabBttn   .SetActive(false);
                _fightBttn  .SetActive(false);
                _coinsContainerUI.gameObject.SetActive(false);
                break;
            default:
                throw new UnityException("Unknonw GameState");
        }
    }

}
