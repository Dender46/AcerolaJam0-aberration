using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _approveBttn;
    [SerializeField] private GameObject _declineBttn;
    [SerializeField] private GameObject _grabBttn;
    [SerializeField] private GameObject _fightBttn;

    public ConveyorController conveyorController;
    public PlayerInventory inventory;

    public bool inputIsBlocked { private set; get; }

    private enum GameState
    {
        WaitingForConveyor,
        ItemIsRegular,
        ItemIsGrabbable,
        ItemIsEnemy,
        FightingEnemy
    }

    private int _enemyItemLayer;
    private GameState _gameState = GameState.WaitingForConveyor;

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
    }

    private void Update()
    {
        inputIsBlocked = conveyorController.IsMoving();
    }

    private void OnConveyorFinished(object sender, ConveyorController.FinishedMovingEventArgs args)
    {
        _gameState = GameState.ItemIsRegular;
        if (args.item.TryGetComponent(out ItemEquipable equipBeh))
        {
            _gameState = GameState.ItemIsGrabbable;
        }
        else if (args.item.layer == _enemyItemLayer)
        {
            _gameState = GameState.ItemIsEnemy;
        }
        UpdateUI();
    }

    private void OnEnemyKilled()
    {

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
            // TODO: move to inventory
            inventory.Put(grabbedItem.GetComponent<ItemEquipable>());
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
                _approveBttn.SetActive(false);
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
