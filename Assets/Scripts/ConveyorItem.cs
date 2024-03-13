using TMPro;
using UnityEngine;

public class ConveyorItem : MonoBehaviour
{
    public int price = 5;

    [Header("References")]
    [SerializeField] private TMP_Text _costTextUI;
    [SerializeField] private TMP_Text _spoilLevelTextUI;
    [SerializeField] private RectTransform _canvasUI;
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
            //_costTextUI.text = "-" + equip.info.cost;
            //_costTextUI.color = _lossColor;
        }
        else if (TryGetComponent(out ItemEnemy enemy))
        {
            Destroy(_canvasUI.gameObject);
        }
        else
        {
            _costTextUI.text = "" + price;
            //_costTextUI.text = "+" + price;
            //_costTextUI.color = _gainsColor;
            spoilLevel = (int)Mathf.Floor(Random.Range(0.0f, 100.0f));
            _spoilLevelTextUI.text = spoilLevel + "%";
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
