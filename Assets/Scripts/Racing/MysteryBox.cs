using System;
using Character;
using Kayak;
using Multiplayer;
using UnityEngine;

namespace Racing
{
    public enum MysteryObject
    {
        None = 0,
        Freezer = 1,
        Iceberg = 2,
        Boost = 3
    }
    
    public class MysteryBox : MonoBehaviour
    {
        [SerializeField] private float _reappearTime;
        [SerializeField] private GameObject _mesh;

        private bool _isHidden;
        private float _reappearTimer;

        private const int MysteryObjectEnumSize = 3;

        private void OnTriggerEnter(Collider other)
        {
            if (_isHidden)
            {
                return;
            }
            
            if (other.gameObject.TryGetComponent(out KayakController controller) == false)
            {
                return;
            }

            SetTaken(controller.Character);
        }

        private void SetTaken(CharacterManager character)
        {
            Debug.Log("taken");
            
            _mesh.SetActive(false);
            
            _reappearTimer = _reappearTime;
            _isHidden = true;

            MysteryObject objectToGive = MysteryObject.Freezer;
            int random = UnityEngine.Random.Range(0, MysteryObjectEnumSize);
            switch (random)
            {
                case 0 : objectToGive = MysteryObject.Freezer; break;
                case 1 : objectToGive = MysteryObject.Iceberg; break;
                case 2 : objectToGive = MysteryObject.Boost; break;
            }

            character.GetMysteryObject(objectToGive);
        }

        private void Update()
        {
            if (_isHidden == false)
            {
                return;
            }

            _reappearTimer -= Time.deltaTime;
            if (_reappearTimer > 0)
            {
                return;
            }
            
            MakeBoxReappear();
        }

        private void MakeBoxReappear()
        {
            _mesh.SetActive(true);

            _isHidden = false;
        }
    }
}
