using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class CharacterGameplayUI : MonoBehaviour
    {
        [SerializeField] private Image _colorIcon;
        [SerializeField] private TMP_Text _playerNameText;

        public void SetPlayerUI(Color color, string playerName)
        {
            _colorIcon.color = color;
            _playerNameText.color = color;
            _playerNameText.text = playerName;
        }
    }
}