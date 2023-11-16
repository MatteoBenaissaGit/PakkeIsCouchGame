using System.Collections.Generic;
using MatteoBenaissaLibrary.SingletonClassBase;
using Menu;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Multiplayer
{
    public class MultiplayerManager : Singleton<MultiplayerManager>
    {
        [field:SerializeField] public RenderTexture[] PlayersRenderTextures { get; private set; }
        public List<Player> Players { get; private set; } = new List<Player>();
        public int NumberOfPlayers { get; private set; }

        private const int MaximumPlayer = 4;
        
        protected override void Awake()
        {
            base.Awake();
        }

        public void AddPlayer(Player player)
        {
            if (NumberOfPlayers >= MaximumPlayer)
            {
                Debug.LogError("Can't add a player, maximum number already");
                return;
            }

            NumberOfPlayers++;
            Players.Add(player);
        }
            
    }

    public class Player
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public Color PlayerColor { get; set; }
        public PlayerSelectionMenuController SelectionController { get; set; }

        public PlayerCharacterCore CharacterCore
        {
            get => _characterCore;
            set
            {
                _characterCore = value;
                _characterCore.Cam.targetTexture = MultiplayerManager.Instance.PlayersRenderTextures[ID];

                SelectionController.Input.onActionTriggered += _characterCore.Character.OnInputPaddleTrue;
                InputActionMap boatActionMap = SelectionController.Input.actions.FindActionMap("Boat");
                InputAction paddleLeft = boatActionMap.FindAction("PaddleLeft");
                paddleLeft.started += _characterCore.Character.OnInputPaddleTrue;
                paddleLeft.performed += _characterCore.Character.OnInputPaddleTrue;
                paddleLeft.canceled += _characterCore.Character.OnInputPaddleFalse;
                InputAction paddleRight = boatActionMap.FindAction("PaddleRight");
                paddleRight.started += _characterCore.Character.OnInputPaddleTrue;
                paddleRight.performed += _characterCore.Character.OnInputPaddleTrue;
                paddleRight.canceled += _characterCore.Character.OnInputPaddleFalse;
            }
        }

        private PlayerCharacterCore _characterCore;
        
        public Player(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}