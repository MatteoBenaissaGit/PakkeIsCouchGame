using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class CharacterGameplayUI : MonoBehaviour
    {
        [SerializeField] private Image _colorIcon;
        [SerializeField] private TMP_Text _playerNameText;
        [SerializeField] private Image _playerPositionImage;
        [SerializeField] private Sprite _sprite1, _sprite2, _sprite3, _sprite4;
        [SerializeField] private TMP_Text _timer;

        public void SetPlayerUI(Color color, string playerName)
        {
            _colorIcon.color = color;
            _playerNameText.color = color;
            _playerNameText.text = playerName;
        }

        public void SetPositionUI(int position)
        {
            if (position == 1)
            {
                _playerPositionImage.sprite = _sprite1;
            }
            else if (position == 2)
            {
                _playerPositionImage.sprite = _sprite2;
            }
            else if (position == 3)
            {
                _playerPositionImage.sprite = _sprite3;
            }
            else
            {
                _playerPositionImage.sprite = _sprite4;
            }
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