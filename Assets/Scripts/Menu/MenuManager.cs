using System;
using Tools.SingletonClassBase;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Menu
{
    public class MenuManager : Singleton<MenuManager>
    {
        [SerializeField] private Transform _playersSelectionLayout;
        [SerializeField] private PlayerSelectionMenuController _playerSelectionControllerPrefab;

        public Action<PlayerSelectionMenuController> OnPlayerJoined;

        private int _currentNumberOfPlayer;

        protected override void Awake()
        {
            base.Awake();

            OnPlayerJoined += PlayerJoined;
        }

        private void OnDisable()
        {
            OnPlayerJoined -= PlayerJoined;
        }

        public void PlayerJoined(PlayerSelectionMenuController player)
        {
            Debug.Log("player joined");
            _currentNumberOfPlayer++;
            player.transform.parent = _playersSelectionLayout;
            player.Set($"Player {_currentNumberOfPlayer}");
        }
    }
}
