using System;
using Menu.Buttons;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Menu
{
    public class PlayerSelectionMenuController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playerNameText;
        [SerializeField] private ButtonController _buttonSelectColor;
        [SerializeField] private ButtonController _buttonSetReady;

        private ButtonController[] _buttons;
        private int _selectedButtonIndex;

        private void Start()
        {
            _buttons = new ButtonController[2]
            {
                _buttonSelectColor,
                _buttonSetReady
            };
            _selectedButtonIndex = 0;

            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttons[i].IsSelected = i == _selectedButtonIndex;
            }
        }

        public void Set(string playerName)
        {
            _playerNameText.text = playerName;
        }

        public void OnPressLeft(InputAction.CallbackContext context)
        {
            if (context.started == false)
            {
                return;
            }
            
            if (_buttons[_selectedButtonIndex].IsDisplayingSubMenu)
            {
                _buttons[_selectedButtonIndex].Left();
                return;
            }
            
            if (_selectedButtonIndex <= 0)
            {
                return;
            }

            _buttons[_selectedButtonIndex].IsSelected = false;
            _selectedButtonIndex--;
            _buttons[_selectedButtonIndex].IsSelected = true;
        }
        
        public void OnPressRight(InputAction.CallbackContext context)
        {
            if (context.started == false)
            {
                return;
            }
            
            if (_buttons[_selectedButtonIndex].IsDisplayingSubMenu)
            {
                _buttons[_selectedButtonIndex].Right();
                return;
            }
            
            if (_selectedButtonIndex >= _buttons.Length-1)
            {
                return;
            }

            _buttons[_selectedButtonIndex].IsSelected = false;
            _selectedButtonIndex++;
            _buttons[_selectedButtonIndex].IsSelected = true;
        }

        public void OnPressUp(InputAction.CallbackContext context)
        {
            if (context.started == false)
            {
                return;
            }
            
            if (_buttons[_selectedButtonIndex].IsDisplayingSubMenu)
            {
                _buttons[_selectedButtonIndex].Up();
                return;
            }
        }
        
        public void OnPressDown(InputAction.CallbackContext context)
        {
            if (context.started == false)
            {
                return;
            }
            
            if (_buttons[_selectedButtonIndex].IsDisplayingSubMenu)
            {
                _buttons[_selectedButtonIndex].Down();
                return;
            }
        }

        public void OnPressSelect(InputAction.CallbackContext context)
        {
            if (context.started == false)
            {
                return;
            }
            
            if (_buttons[_selectedButtonIndex].IsDisplayingSubMenu)
            {
                _buttons[_selectedButtonIndex].Select();
                return;
            }
            
            _buttons[_selectedButtonIndex].Press();
        }
    }
}
