using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer
{
    public class MultiplayerViewManager : MonoBehaviour
    {
        [SerializeField] private GameObject _1PView;
        [SerializeField] private GameObject _2PView;
        [SerializeField] private GameObject _3PView;
        [SerializeField] private GameObject _4PView;
        
        [SerializeField] private List<RawImage> _P1Images;
        [SerializeField] private List<RawImage> _P2Images;
        [SerializeField] private List<RawImage> _P3Images;
        [SerializeField] private List<RawImage> _P4Images;

        private GameObject[] _views;
        
        private void Start()
        {
            if (MultiplayerManager.Instance == null)
            {
                throw new Exception("no multiplayer manager");
            }
            
            MultiplayerManager.Instance.Players.ForEach(x => x.CharacterCore.GameplayCharacter.SetActive(true));
            //TODO place and setup players correctly in scene

            _views = new GameObject[4] { _1PView,_2PView,_3PView,_4PView };
            _views.ToList().ForEach(x => x.SetActive(false));
            _views[MultiplayerManager.Instance.NumberOfPlayers-1].SetActive(true);
            
            //TODO get charcacterscontroller cams and assign them to raw images
        }
    }
}
