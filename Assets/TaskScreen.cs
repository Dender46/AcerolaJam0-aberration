using TMPro;
using UnityEngine;

public class TaskScreen : MonoBehaviour
{
    [SerializeField] private RectTransform _description;
    [SerializeField] private float _titleHeight = 30;
    [SerializeField] private float _paddingDown = 5;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _titleHeight + _description.sizeDelta.y + _paddingDown);
    }


}
