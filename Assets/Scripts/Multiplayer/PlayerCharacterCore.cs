using System;
using Character;
using Kayak;
using Menu;
using UnityEngine;

namespace Multiplayer
{
    public class PlayerCharacterCore : MonoBehaviour
    {
        [field: SerializeField] public PlayerSelectionMenuController SelectionController;
        [field: SerializeField] public GameObject GameplayCharacter;
        [field: SerializeField] public CharacterManager Character;
        [field: SerializeField] public KayakController Kayak;
        [field: SerializeField] public Camera Cam;
        [field: SerializeField] public CharacterGameplayUI GameplayUI;

        public int Position { get; set; }
        public bool IsEliminated { get; set; }
        
        private void Awake()
        {
            MenuManager.Instance.OnPlayerJoined.Invoke(this);

            Character.Core = this;
        }

        public void SetRaceEliminated()
        {
            Kayak.Mesh.gameObject.SetActive(false);
            IsEliminated = true;
        }
    }
}
