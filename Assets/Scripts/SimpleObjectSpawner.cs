using UnityEngine;

namespace ProjectMenuZ
{
    public class SimpleObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _targetObject = null;
        [SerializeField] private Transform _parent = null;
        [SerializeField] private Vector3 _position = Vector3.zero;

        public void Spawn()
        {
            Instantiate(_targetObject, _position, Quaternion.identity, _parent);
        }
    }
}