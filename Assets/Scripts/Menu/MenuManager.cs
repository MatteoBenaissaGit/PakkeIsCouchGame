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
        [Space(10),SerializeField] private PlayerCharacterCore _playerCore;

        public Action<PlayerCharacterCore> OnPlayerJoined;

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
                player.SelectionController.transform.gameObject.SetActive(true);
                player.SelectionController.transform.parent = _playersSelectionLayout;
            }
        }

        private void OnDisable()
        {
            OnPlayerJoined -= PlayerJoined;
        }

        public void PlayerJoined(PlayerCharacterCore playerCore)
        {
            string playerName = $"Player {_currentNumberOfPlayer}";
            int id = _currentNumberOfPlayer;

            _playerCore = playerCore;
            _playerCore.GameplayCharacter.SetActive(false);
            _playerCore.transform.parent = MultiplayerManager.Instance.transform;
            
            Player player = new Player(id, playerName);
            player.SelectionController = playerCore.SelectionController;
            player.CharacterCore = _playerCore;
            MultiplayerManager.Instance.AddPlayer(player);
            
            Debug.Log("player joined");
            _currentNumberOfPlayer++;
            playerCore.SelectionController.transform.parent = _playersSelectionLayout;
            playerCore.SelectionController.Set(playerName, id);
            
        }

        public void CheckPlayersReady()
        {
            foreach (Player player in MultiplayerManager.Instance.Players)
            {
                if (player.SelectionController.ButtonSetReady.IsReady == false)
                {
                    return;
                }
            }
            
            LaunchGame();
        }

        private void LaunchGame()
        {
            Debug.Log("launch game");
            
            MultiplayerManager.Instance.Players.ForEach(x => x.SelectionController.transform.parent = x.CharacterCore.transform);
            MultiplayerManager.Instance.Players.ForEach(x => x.CharacterCore.GameplayUI.SetPlayerUI(x.PlayerColor, x.Name));
            MultiplayerManager.Instance.Players.ForEach(x => x.CharacterCore.transform.parent = MultiplayerManager.Instance.transform);
            MultiplayerManager.Instance.Players.ForEach(x => x.SelectionController.gameObject.SetActive(false));
            MultiplayerManager.Instance.Players.ForEach(x => x.SelectionController.Input.defaultActionMap = "Boat");
            MultiplayerManager.Instance.Players.ForEach(x => x.SelectionController.CanBeUsed = false);
            MultiplayerManager.Instance.Players.ForEach(x => x.SelectionController.Input.currentActionMap = x.SelectionController.Input.actions.FindActionMap("Boat"));
            MultiplayerManager.Instance.Players.ForEach(x => x.SelectionController.Input.defaultActionMap = "Boat");
            
            
            SceneManager.LoadScene("MultiGameScene", LoadSceneMode.Single);
        }
    }
}
