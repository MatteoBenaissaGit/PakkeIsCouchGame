using System;
using System.Collections.Generic;
using Character;
using Multiplayer;
using Tools.SingletonClassBase;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MenuManager : Singleton<MenuManager>
    {
        [Space(10),SerializeField] private Transform _playersSelectionLayout;
        [Space(10),SerializeField] private SelectionMenuController _selectionMenu;
        [Space(10),SerializeField] private PlayerCharacterCore _playerCorePrefab;

        public Action<PlayerSelectionMenuController> OnPlayerJoined;

        private int _currentNumberOfPlayer;

        protected override void Awake()
        {
            base.Awake();
            
            //TODO do this in the selection menu
            OnPlayerJoined += PlayerJoined;

            if (MultiplayerManager.Instance.NumberOfPlayers <= 0)
            {
                return;
            }
            
            foreach (Player player in MultiplayerManager.Instance.Players)
            {
                player.SelectionMenuController.transform.gameObject.SetActive(true);
                player.SelectionMenuController.transform.parent = _playersSelectionLayout;
            }
        }

        private void OnDisable()
        {
            OnPlayerJoined -= PlayerJoined;
        }

        public void PlayerJoined(PlayerSelectionMenuController playerSelectionMenuController)
        {
            string playerName = $"Player {_currentNumberOfPlayer}";
            int id = _currentNumberOfPlayer;

            PlayerCharacterCore core = Instantiate(_playerCorePrefab);
            core.gameObject.SetActive(false);
            core.transform.parent = MultiplayerManager.Instance.transform;
            
            Player player = new Player(id, playerName);
            player.SelectionMenuController = playerSelectionMenuController;
            player.CharacterCore = core;
            MultiplayerManager.Instance.AddPlayer(player);
            
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

            MultiplayerManager.Instance.Players.ForEach(x =>
                x.SelectionMenuController.transform.gameObject.SetActive(false)); 
            MultiplayerManager.Instance.Players.ForEach(x =>
                x.SelectionMenuController.transform.parent = MultiplayerManager.Instance.transform);

            SceneManager.LoadScene("MultiGameScene", LoadSceneMode.Single);
        }
    }
}
