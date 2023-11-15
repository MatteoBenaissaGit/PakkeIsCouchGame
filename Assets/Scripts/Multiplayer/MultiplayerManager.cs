using System.Collections.Generic;
using MatteoBenaissaLibrary.SingletonClassBase;
using Menu;
using UnityEngine;

namespace Multiplayer
{
    public class MultiplayerManager : Singleton<MultiplayerManager>
    {
        public List<Player> Players = new List<Player>();
        
        protected override void Awake()
        {
            base.Awake();
        }
        
        
    }

    public class Player
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public Color PlayerColor { get; set; }
        public PlayerSelectionMenuController SelectionMenuController { get; set; }
        
        public Player(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}