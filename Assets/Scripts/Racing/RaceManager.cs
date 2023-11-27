using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Kayak;
using MatteoBenaissaLibrary.SingletonClassBase;
using Multiplayer;
using TMPro;
using UnityEngine;

namespace Racing
{
    public class RaceManager : Singleton<RaceManager>
    {
        public float FreezeTimeTimer { get; set; }
        
        [SerializeField] private List<Transform> _startPositions = new List<Transform>();
        [SerializeField] private TMP_Text _countdownText;
        [SerializeField] private float _timeToEliminateLast;
        [Space(10),SerializeField] private List<RaceCheckpointController> _checkpoints = new List<RaceCheckpointController>();

        private List<Player> _players = new List<Player>();
        private Vector3 _baseCountdownTextScale;
        private bool _isRaceLaunched;
        private bool _isRaceEnded;
        private int _checkpointsCount;
        private int _checkpointToPassIndex;
        private RaceCheckpointController _currentCheckpoint;
        private float _eliminateTimer;
        private Stack<Player> _eliminatedPlayers = new Stack<Player>();
        
        private void Start()
        {
            _eliminateTimer = _timeToEliminateLast;
            _players.ForEach(x => x.CharacterCore.GameplayUI.SetTimer(_eliminateTimer));
            
            _checkpointToPassIndex = 0;
            _checkpointsCount = _checkpoints.Count;
            _currentCheckpoint = _checkpoints[0];
            _currentCheckpoint.IsActive = true;
            
            _baseCountdownTextScale = _countdownText.transform.localScale;
            _countdownText.DOFade(0, 0);
            
            _players = MultiplayerManager.Instance.Players.ToList();
            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].CharacterCore.GameplayCharacter.transform.position = _startPositions[i].position;
                _players[i].CharacterCore.GameplayUI.SetPositionUI(i+1);
            }
            
            CountDown();
        }

        private void Update()
        {
            UpdateEliminateTimer();
            SetPlayersPositionStart();
            UpdatePositions();
        }

        private int _playerEliminatedCount = 0;
        private void UpdateEliminateTimer()
        {
            if (_isRaceLaunched == false || _isRaceEnded)
            {
                return;
            }

            if (FreezeTimeTimer > 0)
            {
                _players.ForEach(x => x.CharacterCore.GameplayUI.SetTimerFreeze());
                FreezeTimeTimer -= Time.deltaTime;
                return;
            }
            
            _eliminateTimer -= Time.deltaTime;
            _players.ForEach(x => x.CharacterCore.GameplayUI.SetTimer(_eliminateTimer));
            if (_eliminateTimer > 0)
            {
                return;
            }

            _eliminateTimer = _timeToEliminateLast;
            List<Player> orderedList = _players
                .OrderBy(x => Vector3.Distance(x.CharacterCore.Kayak.transform.position, _currentCheckpoint.transform.position))
                .ToList();
            Player eliminatedPlayer = orderedList[orderedList.Count - 1 - _playerEliminatedCount];
            eliminatedPlayer.CharacterCore.SetRaceEliminated();
            _playerEliminatedCount++;
            _eliminatedPlayers.Push(eliminatedPlayer);

            if (_eliminatedPlayers.Count >= _players.Count-1)
            {
                _eliminatedPlayers.Push(orderedList[0]);
                EndRace();
            }
        }
        
        private void SetPlayersPositionStart()
        {
            if (_isRaceLaunched || _isRaceEnded)
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
            SetCountDownText("Don't be the last when the timer is at 0!");
            await Task.Delay(2500);
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

            float duration = value.Length > 3 ? 2.5f : 1f;
            float fade = value.Length > 3 ? 1f : 0f;
            Vector3 size = value.Length > 3 ? _baseCountdownTextScale * 1.1f : _baseCountdownTextScale * 1.5f;
            _countdownText.transform.DOScale(size, duration);
            _countdownText.DOFade(fade, duration);
        }

        private void LaunchRace()
        {
            _isRaceLaunched = true;
            _players.ForEach(x => x.CharacterCore.Character.CurrentStateBaseProperty.CanCharacterMove = true);
        }

        public void SetCheckpointPassed(RaceCheckpointController checkpoint, KayakController kayak)
        {
            if (checkpoint != _currentCheckpoint)
            {
                return;
            }

            _checkpoints[_checkpointToPassIndex].IsActive = false;
            _checkpointToPassIndex++;
            
            if (_checkpointToPassIndex >= _checkpointsCount)
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
                if (orderedList[i].CharacterCore.IsEliminated)
                {
                    continue;
                }
                orderedList[i].SetPosition(i+1);
            }
        }

        private void EndRace()
        {
            _isRaceEnded = true;
            MultiplayerManager.Instance.EndGame(_eliminatedPlayers.ToList());
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
