using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Multiplayer;
using TMPro;
using UnityEngine;

namespace Racing
{
    public class RaceManager : MonoBehaviour
    {
        [SerializeField] private List<Transform> _startPositions = new List<Transform>();
        [SerializeField] private TMP_Text _countdownText;

        private List<Player> _players = new List<Player>();
        private Vector3 _baseCountdownTextScale;
        
        private void Start()
        {
            _baseCountdownTextScale = _countdownText.transform.localScale;
            _countdownText.DOFade(0, 0);
            
            _players = MultiplayerManager.Instance.Players;
            for (int i = 0; i < _players.Count; i++)
            {
                Debug.Log("set player at " + _startPositions[i].position);
                _players[i].CharacterCore.Kayak.transform.position = _startPositions[i].position;
            }
            
            CountDown();
        }

        private async void CountDown()
        {
            await Task.Delay(3000);
            SetCountDownText("3");
            await Task.Delay(1000);
            SetCountDownText("2");
            await Task.Delay(1000);
            SetCountDownText("1");
            await Task.Delay(1000);
            
            SetCountDownText("Go!");
            LaunchRace();
        }

        private void SetCountDownText(string value)
        {
            _countdownText.text = value;
            _countdownText.transform.localScale = _baseCountdownTextScale;
            _countdownText.DOFade(1, 0);
            _countdownText.DOComplete();
            
            _countdownText.transform.DOScale(_baseCountdownTextScale * 1.5f, 1f);
            _countdownText.DOFade(0, 1f);
        }

        private void LaunchRace()
        {
            _players.ForEach(x => x.CharacterCore.Character.CurrentStateBaseProperty.CanCharacterMove = true);
        }
    }
}
