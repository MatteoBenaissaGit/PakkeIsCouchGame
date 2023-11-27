using System;
using DG.Tweening;
using UnityEngine;

namespace Racing
{
    public class PaddleHitEffect : MonoBehaviour
    {
        private float _timer;
        private Vector3 _scale;
        
        private void Awake()
        {
            _scale = transform.localScale;
            transform.localScale = Vector3.zero;
            _timer = 0.25f;
            transform.DOScale(_scale, 0.2f);
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, 9f);
            
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
