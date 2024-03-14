using System;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private float _conveyorTime = 5.0f;
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private Vector3 _center;
    [SerializeField] private Vector3 _spawnPoint;
    [SerializeField] private Vector3 _despawnPoint;
    [SerializeField] private Transform _containerForSpawnedObjects;

    private Transform[] _objectsOnTheConveyor = new Transform[3];

    private float _timer = 0.0f;
    private bool _isMoving = false;

    public class FinishedMovingEventArgs : EventArgs
    {
        public GameObject item;
        public bool levelFinished;
    }

    public event EventHandler<FinishedMovingEventArgs> onFinishMoving;

    public void ResetMe()
    {
        var firstObject = Instantiate(_levelManager.GetNextItem()).transform;
        firstObject.SetParent(_containerForSpawnedObjects, false);
        firstObject.localPosition = _spawnPoint;
        _objectsOnTheConveyor[0] = firstObject;

        _isMoving = true;
    }

    private void Update()
    {
        if (_isMoving)
        {
            _timer += Time.deltaTime;
            var lerpT = Mathf.Clamp(_timer / _conveyorTime, 0.0f, 1.0f);

            if (lerpT >= 1.0f)
            {
                FinishProgression();
                return;
            }
            
            if (_objectsOnTheConveyor[0]) LerpObjectToDest(_objectsOnTheConveyor[0], _spawnPoint, _center, lerpT);
            if (_objectsOnTheConveyor[1]) LerpObjectToDest(_objectsOnTheConveyor[1], _center, _despawnPoint, lerpT);
        }
    }

    public bool IsMoving()
    {
        return _isMoving;
    }

    public void ResumeConveyor()
    {
        _isMoving = true;
    }

    public GameObject PeekCurrentItem()
    {
        return _objectsOnTheConveyor[1].gameObject;
    }

    private void LerpObjectToDest(Transform t, Vector3 start, Vector3 dest, float lerpT)
    {
        t.localPosition = Vector3.LerpUnclamped(start, dest, _animationCurve.Evaluate(lerpT));
    }

    private void FinishProgression()
    {
        _timer = 0.0f;
        _isMoving = false;

        if (_objectsOnTheConveyor[2]) 
            Destroy(_objectsOnTheConveyor[2].gameObject);
        _objectsOnTheConveyor[2] = _objectsOnTheConveyor[1];
        _objectsOnTheConveyor[1] = _objectsOnTheConveyor[0];
        _objectsOnTheConveyor[0] = null;

        
        var nextGO = _levelManager.GetNextItem();
        if (nextGO != null) // finished progression and there are still items left
        {
            var newObject = Instantiate(nextGO).transform;
            newObject.SetParent(_containerForSpawnedObjects, false);
            newObject.localPosition = _spawnPoint;
            _objectsOnTheConveyor[0] = newObject;
        }

        // All items have gone through conveyor
        if (_objectsOnTheConveyor[1])
        {
            onFinishMoving?.Invoke(this, new FinishedMovingEventArgs(){
                item = _objectsOnTheConveyor[1].gameObject,
                levelFinished = false
            });
        }
        else
        {
            onFinishMoving?.Invoke(this, new FinishedMovingEventArgs(){
                item = null,
                levelFinished = true
            });
        }
    }

    private void OnDrawGizmosSelected()
    {
        var cubeSize = 0.3f;
        var properCenter = transform.position + _center + Vector3.up * (cubeSize / 2);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(properCenter, new Vector3(cubeSize, cubeSize, cubeSize));

        properCenter = transform.position + _spawnPoint + Vector3.up * (cubeSize / 2);
        Gizmos.color = Color.green;
        Gizmos.DrawCube(properCenter, new Vector3(cubeSize, cubeSize, cubeSize));

        properCenter = transform.position + _despawnPoint + Vector3.up * (cubeSize / 2);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(properCenter, new Vector3(cubeSize, cubeSize, cubeSize));
    }
}
