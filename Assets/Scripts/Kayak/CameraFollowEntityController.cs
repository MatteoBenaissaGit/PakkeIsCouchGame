using System;
using UnityEngine;

namespace Kayak
{
    public class CameraFollowEntityController : MonoBehaviour
    {
        [SerializeField] private Transform _entityToFollow;
        
        private Vector3 _positionOffset;
        private Vector3 _rotationOffset;
        private Vector3 _velocity;

        private void Awake()
        {
            _positionOffset = _entityToFollow.position - transform.position;
            _rotationOffset = _entityToFollow.rotation.eulerAngles - transform.rotation.eulerAngles;
        }

        private void Update()
        {
            transform.position = Vector3.SmoothDamp(transform.position, _entityToFollow.position + _positionOffset, ref _velocity, 0.2f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_entityToFollow.rotation.eulerAngles + _rotationOffset), 0.1f);
        }
    }
}
