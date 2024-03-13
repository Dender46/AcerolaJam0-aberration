using TMPro;
using UnityEngine;

public class TextAction : MonoBehaviour
{
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private float _animationTime = 0.5f;
    [SerializeField] private float _targetFontSize = 22.0f;

    private TMP_Text _textUI;

    private float _timer = 0.0f;
    private float _initialFontSize;

    private bool _animationIsPlaying = false;
    private string _internalText;

    void Start()
    {
        _textUI = GetComponent<TMP_Text>();
        _initialFontSize = _textUI.fontSize;
        _internalText = _textUI.text;
    }

    void Update()
    {
        if (!_animationIsPlaying && _internalText == _textUI.text)
        {
            return;
        }

        _animationIsPlaying = true;
        _internalText = _textUI.text;


        _timer += Time.deltaTime;
        if (_timer >= _animationTime)
        {
            _animationIsPlaying = false;
            _timer = 0.0f;
            return;
        }

        var lerpT = Mathf.Clamp(_timer / _animationTime, 0.0f, 1.0f);
        _textUI.fontSize = Mathf.Lerp(_initialFontSize, _targetFontSize, _animationCurve.Evaluate(lerpT));
    }
}
