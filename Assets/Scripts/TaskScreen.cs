using System;
using UnityEngine;

public class TaskScreen : MonoBehaviour
{
    [Header("ShowAnimation")]
    [SerializeField] private AnimationCurve _showAnimationCurve;
    [SerializeField] private Vector2 _finalScale = new Vector2(1.8f, 1.8f);
    [SerializeField]private Vector2 _finalAnchorMin = new(0.5f, 0.5f);
    [SerializeField]private Vector2 _finalAnchorMax = new(0.5f, 0.5f);
    [SerializeField]private Vector2 _finalPivot = new(0.0f, 0.0f);
    [SerializeField] private float _time;
    [Header("Stuff")]
    [SerializeField] private RectTransform _description;
    [SerializeField] private float _titleHeight = 30;
    [SerializeField] private float _paddingDown = 5;

    private Vector2 _initialPos;
    private Vector2 _initialScale;
    private Vector2 _initialAnchorMin;
    private Vector2 _initialAnchorMax;
    private Vector2 _initialPivot;
    private RectTransform _rectTransform;

    private float _timer = 0.0f;
    public bool _isAnimating;

    public event EventHandler onScreenIsFinished;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _initialPos = _rectTransform.anchoredPosition;
        _initialScale = _rectTransform.localScale;
        _initialAnchorMin = _rectTransform.anchorMin;
        _initialAnchorMax = _rectTransform.anchorMax;
        _initialPivot = _rectTransform.pivot;
    }

    private void Update()
    {
        if (_isAnimating)
        {
            _timer += Time.deltaTime;
            if (_timer >= _time)
            {
                _isAnimating = false;
                _timer = 0.0f;
                onScreenIsFinished?.Invoke(this, EventArgs.Empty);
                return;
            }
            var lerpT = Mathf.Clamp(_timer / _time, 0.0f, 1.0f);
            _rectTransform.anchoredPosition = Vector2.LerpUnclamped(_initialPos, Vector2.zero, _showAnimationCurve.Evaluate(lerpT));
            _rectTransform.localScale = Vector2.LerpUnclamped(_initialScale, _finalScale, _showAnimationCurve.Evaluate(lerpT));
            _rectTransform.anchorMin = Vector2.LerpUnclamped(_initialAnchorMin, _finalAnchorMin, _showAnimationCurve.Evaluate(lerpT));
            _rectTransform.anchorMax = Vector2.LerpUnclamped(_initialAnchorMax, _finalAnchorMax, _showAnimationCurve.Evaluate(lerpT));
            _rectTransform.pivot = Vector2.LerpUnclamped(_initialPivot, _finalPivot, _showAnimationCurve.Evaluate(lerpT));
        }
        //else
        {
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _titleHeight + _description.sizeDelta.y + _paddingDown);
        }
    }

    public void Show()
    {
        _isAnimating = true;
    }

}
