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
        [Space(10),SerializeField] private string _sceneToLaunch;

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

            MultiplayerManager.Instance.Players.ForEach(ToDoForPlayerAtLaunch);
            
            SceneManager.LoadScene(_sceneToLaunch, LoadSceneMode.Single);
        }

        private void ToDoForPlayerAtLaunch(Player player)
        {
            player.CharacterCore.GameplayUI.SetPlayerUI(player.PlayerColor, player.Name);
            player.CharacterCore.Kayak.Trail.startColor = player.PlayerColor;
            player.CharacterCore.Kayak.Trail.endColor = player.PlayerColor;
            player.CharacterCore.Kayak.SpriteColor.color = player.PlayerColor;
            player.CharacterCore.transform.parent = MultiplayerManager.Instance.transform;

            player.SelectionController.transform.parent = player.CharacterCore.transform;
            player.SelectionController.gameObject.SetActive(false);
            player.SelectionController.Input.defaultActionMap = "Boat";
            player.SelectionController.CanBeUsed = false;
            player.SelectionController.Input.currentActionMap = player.SelectionController.Input.actions.FindActionMap("Boat");
            player.SelectionController.Input.defaultActionMap = "Boat";
        }
    }
}
