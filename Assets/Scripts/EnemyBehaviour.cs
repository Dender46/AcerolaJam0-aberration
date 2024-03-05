using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private float _bobbingYUp = 0.1f;
    [SerializeField] private float _bobbingYDown = -0.1f;
    [SerializeField] private float _bobbingTime = 2.0f;
    
    private Transform _modelTransform;
    private Vector3 _initialPosition;
    private float _bobbingTimer = 0.0f;
    private float _bobbingDirection = 1.0f;

    private void Start()
    {
        _modelTransform = transform.GetChild(0);
        _initialPosition = _modelTransform.localPosition;
    }

    private void Update()
    {
        _bobbingTimer += Time.deltaTime * _bobbingDirection;
        if (_bobbingTimer >= _bobbingTime || _bobbingTimer <= 0.0f)
        {
            _bobbingDirection *= -1.0f;
        }
        var lerpT = _bobbingTimer / _bobbingTime;

        var startPos = _initialPosition;
        startPos.y += _bobbingYUp;
        var endPos = _initialPosition;
        endPos.y += _bobbingYDown;
        _modelTransform.localPosition = Vector3.LerpUnclamped(startPos, endPos, _animationCurve.Evaluate(lerpT));
    }

}
