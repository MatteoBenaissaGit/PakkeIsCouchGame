using System;
using System.Collections.Generic;
using System.Linq;
using MatteoBenaissaLibrary.SingletonClassBase;
using Menu;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Multiplayer
{
    [Serializable]
    public struct WinUIReferences
    {
        public GameObject UI;
        public TMP_Text NameText;
    }
    
    public class MultiplayerManager : Singleton<MultiplayerManager>
    {
        [field:SerializeField] public RenderTexture[] PlayersRenderTextures { get; private set; }
        public List<Player> Players { get; private set; } = new List<Player>();
        public int NumberOfPlayers { get; private set; }

        public GameObject WinUI;
        [SerializeField] private WinUIReferences[] _winUIs;
        
        private const int MaximumPlayer = 4;
        
        protected override void Awake()
        {
            base.Awake();
            
            WinUI.SetActive(false);
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

        public void EndGame(List<Player> playersPositions)
        {
            WinUI.SetActive(true);
            
            _winUIs.ToList().ForEach(x => x.UI.SetActive(false));
            
            for (int i = 0; i < playersPositions.Count; i++)
            {
                _winUIs[i].UI.SetActive(true);
                _winUIs[i].NameText.text = playersPositions[i].Name;
                _winUIs[i].NameText.color = playersPositions[i].PlayerColor;
            }
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
            }
        }

        private PlayerCharacterCore _characterCore;
        
        public Player(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public void SetPosition(int position)
        {
            CharacterCore.GameplayUI.SetPositionUI(position);
            CharacterCore.Position = position;
        }
    }
}