using System;
using System.Collections.Generic;
using Multiplayer;
using Tools.SingletonClassBase;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Menu
{
    public class MenuManager : Singleton<MenuManager>
    {
        [Space(10),SerializeField] private Transform _playersSelectionLayout;
        [Space(10),SerializeField] private SelectionMenuController _selectionMenu;

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

        public void PlayerJoined(PlayerSelectionMenuController playerSelectionMenuController)
        {
            string playerName = $"Player {_currentNumberOfPlayer}";
            int id = _currentNumberOfPlayer;
            
            Player player = new Player(id, playerName);
            player.SelectionMenuController = playerSelectionMenuController;
            MultiplayerManager.Instance.Players.Add(player);
            
            Debug.Log("player joined");
            _currentNumberOfPlayer++;
            playerSelectionMenuController.transform.parent = _playersSelectionLayout;
            playerSelectionMenuController.Set(playerName, id);
            
        }

        public void CheckPlayersReady()
        {
            foreach (Player player in MultiplayerManager.Instance.Players)
            {
                if (player.SelectionMenuController.ButtonSetReady.IsReady == false)
                {
                    return;
                }
            }
            
            LaunchGame();
        }

        private void LaunchGame()
        {
            Debug.Log("launch game");
        }
    }
}
