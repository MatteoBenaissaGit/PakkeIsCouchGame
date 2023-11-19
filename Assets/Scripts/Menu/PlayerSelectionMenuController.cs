using System;
using Menu.Buttons;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Menu
{
    public class PlayerSelectionMenuController : MonoBehaviour
    { 
        [field:SerializeField] public TMP_Text PlayerNameText { get; set; }
        [field:SerializeField] public SetReadyButtonController ButtonSetReady { get; private set; }
        [field:SerializeField] public PlayerInput Input { get; private set; }
        public int ID { get; private set; }
        public bool CanBeUsed { get;  set; }
        
        [SerializeField] private ColorButtonController _buttonSelectColor;

        private ButtonController[] _buttons;
        private int _selectedButtonIndex;

        private void Awake()
        {
            _buttonSelectColor.Controller = this;
            
            _buttonSelectColor.SetIconColor(UnityEngine.Random.ColorHSV());
        }

        private void Start()
        {
            CanBeUsed = true;
            
            _buttons = new ButtonController[2]
            {
                _buttonSelectColor,
                ButtonSetReady
            };
            _selectedButtonIndex = 0;

            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttons[i].IsSelected = i == _selectedButtonIndex;
            }
        }

        public void Set(string playerName, int id)
        {
            ID = id;
            PlayerNameText.text = playerName;
        }

        public void OnPressLeft(InputAction.CallbackContext context)
        {
            if (context.started == false
                || _buttons == null
                || _buttons.Length == 0
                || CanBeUsed == false)
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
            if (context.started == false
                || _buttons == null
                || _buttons.Length == 0
                || CanBeUsed == false)
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
            if (context.started == false
                || _buttons == null
                || _buttons.Length == 0
                || CanBeUsed == false)
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
            if (context.started == false
                || _buttons == null
                || _buttons.Length == 0
                || CanBeUsed == false)
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
            if (context.started == false
                || _buttons == null
                || _buttons.Length == 0
                || CanBeUsed == false)
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
