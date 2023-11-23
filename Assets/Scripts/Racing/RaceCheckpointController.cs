using System;
using Kayak;
using UnityEngine;

namespace Racing
{
    [RequireComponent(typeof(Collider))]
    public class RaceCheckpointController : MonoBehaviour
    {
        public bool IsActive { get; set; }
        
        [SerializeField] private RaceManager _raceManager;
        [SerializeField] private MeshRenderer _mesh;
        
        private Collider _collider;
        
        private void Awake()
        {
            _mesh.enabled = false;
            
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out KayakController kayak) == false)
            {
                return;
            }

            _raceManager.SetCheckpointPassed(this, kayak);
        }
    }
}