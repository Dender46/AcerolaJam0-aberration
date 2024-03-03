using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    [SerializeField] private GameObject[] _availableObjects;
    [SerializeField] private float _conveyorTime = 5.0f;
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private Vector3 _center;
    [SerializeField] private Vector3 _spawnPoint;
    [SerializeField] private Vector3 _despawnPoint;
    [SerializeField] private Transform _containerForSpawnedObjects;

    private Transform[] _objectsOnTheConveyor = new Transform[3];

    private float _timer = 0.0f;
    public bool _isMoving = false;

    private void Start()
    {
        var firstObject = Instantiate(GetRandomAvailableObject()).transform;
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

        var newObject = Instantiate(GetRandomAvailableObject()).transform;
        newObject.SetParent(_containerForSpawnedObjects, false);
        newObject.localPosition = _spawnPoint;
        _objectsOnTheConveyor[0] = newObject;
    }

    private GameObject GetRandomAvailableObject()
    {
        return _availableObjects[Random.Range(0, _availableObjects.Length)];
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
