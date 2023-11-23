using System;
using System.Collections.Generic;
using System.Linq;
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
        [Space(10),SerializeField] private List<RaceCheckpointController> _checkpoints = new List<RaceCheckpointController>();

        private List<Player> _players = new List<Player>();
        private Vector3 _baseCountdownTextScale;
        private bool _isRaceLaunched;
        private int _checkpointCount;
        private int _checkpointToPassIndex;
        private RaceCheckpointController _currentCheckpoint;

        private void Start()
        {
            _checkpointToPassIndex = 0;
            _checkpointCount = _checkpoints.Count;
            _currentCheckpoint = _checkpoints[0];
            _currentCheckpoint.IsActive = true;
            
            _baseCountdownTextScale = _countdownText.transform.localScale;
            _countdownText.DOFade(0, 0);
            
            _players = MultiplayerManager.Instance.Players;
            for (int i = 0; i < _players.Count; i++)
            {
                Debug.Log("set player at " + _startPositions[i].position);
                _players[i].CharacterCore.GameplayCharacter.transform.position = _startPositions[i].position;
            }
            
            CountDown();
        }

        private void Update()
        {
            SetPlayersPositionStart();
            UpdatePositions();
        }

        private void SetPlayersPositionStart()
        {
            if (_isRaceLaunched)
            {
                return;
            }
            
            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].CharacterCore.Kayak.transform.position = _startPositions[i].position;
            }
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
            _isRaceLaunched = true;
            _players.ForEach(x => x.CharacterCore.Character.CurrentStateBaseProperty.CanCharacterMove = true);
        }

        public void SetCheckpointPassed(RaceCheckpointController checkpoint)
        {
            if (checkpoint != _currentCheckpoint)
            {
                return;
            }

            _checkpoints[_checkpointToPassIndex].IsActive = false;
            _checkpointToPassIndex++;
            
            if (_checkpointToPassIndex >= _checkpointCount)
            {
                _checkpointToPassIndex = 0;
            }
            
            _checkpoints[_checkpointToPassIndex].IsActive = true;
            _currentCheckpoint = _checkpoints[_checkpointToPassIndex];
        }

        private void UpdatePositions()
        {
            if (_isRaceLaunched == false)
            {
                return;
            }

            List<Player> orderedList = _players
                .OrderBy(x => Vector3.Distance(x.CharacterCore.Kayak.transform.position, _currentCheckpoint.transform.position))
                .ToList();

            for (int i = 0; i < orderedList.Count; i++)
            {
                Debug.Log($"{orderedList[i].ID} : {i+1}");
                orderedList[i].SetPosition(i+1);
            }
        }
        
        #if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (_isRaceLaunched == false)
            {
                return;
            }
            
            Gizmos.color = Color.green;
            foreach (Player player in _players)
            {
                Gizmos.DrawLine(player.CharacterCore.Kayak.transform.position, _currentCheckpoint.transform.position);
            }
        }

#endif
    }
}
