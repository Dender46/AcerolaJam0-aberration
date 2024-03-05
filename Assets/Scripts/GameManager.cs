using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _approveBttn;
    [SerializeField] private GameObject _declineBttn;
    [SerializeField] private GameObject _grabBttn;

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

    private GameState _gameState = GameState.WaitingForConveyor;

    public GameManager instance { private set; get; }

    private void Awake()
    {
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
        Debug.Log("OnConveyorFinished");
        _gameState = GameState.ItemIsRegular;
        if (args.item.TryGetComponent(out EquipableBehaviour equipBeh))
        {
            _gameState = GameState.ItemIsGrabbable;
        }
        UpdateUI();
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
            inventory.Put(grabbedItem.GetComponent<EquipableBehaviour>());
            Destroy(grabbedItem);
            conveyorController.ResumeConveyor();
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
                break;
            case GameState.ItemIsRegular:
                _approveBttn.SetActive(true);
                _declineBttn.SetActive(true);
                _grabBttn   .SetActive(false);
                break;
            case GameState.ItemIsGrabbable:
                _approveBttn.SetActive(true);
                _declineBttn.SetActive(true);
                _grabBttn   .SetActive(true);
                break; 
            case GameState.ItemIsEnemy:
                _approveBttn.SetActive(false);
                _declineBttn.SetActive(true);
                _grabBttn   .SetActive(false);
                break; 
            case GameState.FightingEnemy: 
                _approveBttn.SetActive(false);
                _declineBttn.SetActive(false);
                _grabBttn   .SetActive(false);
                break;
            default:
                throw new UnityException("Unknonw GameState");
        }
    }

}
