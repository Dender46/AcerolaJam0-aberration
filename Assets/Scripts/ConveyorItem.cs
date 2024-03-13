using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConveyorItem : MonoBehaviour
{
    public int price = 5;

    [Header("References")]
    [SerializeField] private TMP_Text _costTextUI;
    [SerializeField] private GameObject _spoilIcon;
    [SerializeField] private TMP_Text _spoilLevelTextUI;
    [SerializeField] private RectTransform _canvasUI;
    [SerializeField] private float _canvasUIMin = 30;
    [SerializeField] private float _canvasUIMax = 56;
    [SerializeField] private Color _gainsColor = Color.green;
    [SerializeField] private Color _lossColor = Color.red;
    [Header("SpoiledInfo")]
    [SerializeField] private GameObject _spoiledEffectRef;
    [SerializeField] private ParticleSystem _spoiledParticlesRef;

    public int spoilLevel = 0;

    private void Start()
    {
        if (TryGetComponent(out ItemEquipable equip))
        {
            price = equip.info.cost;
            _costTextUI.text = "" + equip.info.cost;
            _spoilIcon.SetActive(false);
            _spoilLevelTextUI.gameObject.SetActive(false);
            _canvasUI.sizeDelta = new Vector2(_canvasUI.sizeDelta.x, _canvasUIMin);
        }
        else if (TryGetComponent(out ItemEnemy enemy))
        {
            Destroy(_canvasUI.gameObject);
            _spoilIcon.SetActive(false);
            _spoilLevelTextUI.gameObject.SetActive(false);
            _canvasUI.sizeDelta = new Vector2(_canvasUI.sizeDelta.x, _canvasUIMin);
        }
        else
        {
            var fixedPrice = LevelManager.instance.GetPossibleFixedPrice();
            price = fixedPrice == -1 ? price : fixedPrice;
            _costTextUI.text = price.ToString();

            if (LevelManager.instance.currentSpoilTarget == -1)
            {
                Debug.Log("LevelManager.instance.currentSpoilTarget == -1");
                _spoilIcon.SetActive(false);
                _spoilLevelTextUI.gameObject.SetActive(false);
                _canvasUI.sizeDelta = new Vector2(_canvasUI.sizeDelta.x, _canvasUIMin);
            }
            else
            {
                Debug.Log("LevelManager.instance.currentSpoilTarget == " + LevelManager.instance.currentSpoilTarget);
                spoilLevel = Random.Range(0, 101);
                _spoilLevelTextUI.text = spoilLevel + "%";
                _canvasUI.sizeDelta = new Vector2(_canvasUI.sizeDelta.x, _canvasUIMax);
                if (spoilLevel > 40.0f)
                {
                    _spoiledEffectRef.SetActive(true);
                    _spoiledParticlesRef.Play();
                }
                else
                {
                    _spoiledEffectRef.SetActive(false);
                    _spoiledParticlesRef.Stop();
                }
            }
            
        }
    }
}
