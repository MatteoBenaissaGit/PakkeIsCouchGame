using System;
using DG.Tweening;
using UnityEngine;

namespace Racing
{
    [RequireComponent(typeof(Rigidbody))]
    public class IcebergDroppable : MonoBehaviour
    {
        private Vector3 _scale;
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
        }

        private void Start()
        {
            _scale = transform.localScale;
            transform.localScale = Vector3.zero;
            transform.DOScale(_scale, 0.5f).OnComplete(() => _collider.enabled = true);
        }
    }
}