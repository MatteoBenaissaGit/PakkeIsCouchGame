﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class CharacterGameplayUI : MonoBehaviour
    {
        [SerializeField] private Image _colorIcon;
        [SerializeField] private TMP_Text _playerNameText;
        [SerializeField] private TMP_Text _playerPositionNumber;
        [SerializeField] private TMP_Text _timer;

        public void SetPlayerUI(Color color, string playerName)
        {
            _colorIcon.color = color;
            _playerNameText.color = color;
            _playerNameText.text = playerName;
        }

        public void SetPositionUI(int position)
        {
            Color color = Color.white;
            string text;
            if (position == 1)
            {
                text = "1st";
                color = new Color(1f, 0.85f, 0.01f);
            }
            else if (position == 2)
            {
                text = "2nd";
                color = new Color(0.89f, 0.89f, 0.89f);
            }
            else if (position == 3)
            {
                text = "3rd";
                color = new Color(0.67f, 0.4f, 0.18f);
            }
            else
            {
                text = $"{position}th";
            }


            _playerPositionNumber.color = color;
            _playerPositionNumber.text = text;
        }

        public void SetTimer(float timer)
        {
            _timer.color = timer < 10f ? Color.red : Color.white;
            
            int minute = (int)(timer / 60);
            int seconds = (int)(timer % 60);
            _timer.text = $"{minute.ToString().PadLeft(2,'0')}:{seconds.ToString().PadLeft(2,'0')}s";
        }
    }
}