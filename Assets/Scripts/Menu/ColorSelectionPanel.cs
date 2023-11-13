using System;
using System.Linq;
using Menu.Buttons;
using UnityEngine;

namespace Menu
{
    public class ColorSelectionPanel : MonoBehaviour
    {
        public bool IsActive { get; private set; }
        
        [SerializeField] private ColorButtonController _controller;
        [SerializeField] private Transform _panelGridLayout;
        [SerializeField] private ColorPanelIconButtonController _iconButtonPrefab;
        [SerializeField] private Color[] _panelColors;

        private ColorPanelIconButtonController[] _colorPanelIconButtons;
        private int _currentColorSelectedIndex;

        private void Awake()
        {
            _colorPanelIconButtons = new ColorPanelIconButtonController[_panelColors.Length];
            
            int index = 0;

            foreach (Color color in _panelColors)
            {
                ColorPanelIconButtonController colorButton = Instantiate(_iconButtonPrefab, _panelGridLayout);
                colorButton.SetColor(color, _controller);

                _colorPanelIconButtons[index] = colorButton;
                _colorPanelIconButtons[index].IsSelected = false;
                
                index++;
            }
        }

        public void Show(bool doShow)
        {
            IsActive = doShow;
            gameObject.SetActive(IsActive);
            _colorPanelIconButtons.ToList().ForEach(x => x.IsSelected = false);
            _colorPanelIconButtons[_currentColorSelectedIndex].IsSelected = true;
        }

        public void Press()
        {
            IsActive = false;
            _colorPanelIconButtons[_currentColorSelectedIndex].SetPressed(true);
        }

        public void Right()
        {
            if (_currentColorSelectedIndex >= _colorPanelIconButtons.Length-1)
            {
                return;
            }

            _colorPanelIconButtons[_currentColorSelectedIndex].IsSelected = false;
            _currentColorSelectedIndex++;
            _colorPanelIconButtons[_currentColorSelectedIndex].IsSelected = true;
        }

        public void Left()
        {
            if (_currentColorSelectedIndex <= 0)
            {
                return;
            }

            _colorPanelIconButtons[_currentColorSelectedIndex].IsSelected = false;
            _currentColorSelectedIndex--;
            _colorPanelIconButtons[_currentColorSelectedIndex].IsSelected = true;
        }

        private const int GridSize = 4;
        public void Up()
        {
            if (_currentColorSelectedIndex - GridSize < 0)
            {
                return;
            }

            _colorPanelIconButtons[_currentColorSelectedIndex].IsSelected = false;
            _currentColorSelectedIndex -= GridSize;
            _colorPanelIconButtons[_currentColorSelectedIndex].IsSelected = true;
        }

        public void Down()
        {
            if (_currentColorSelectedIndex + GridSize >= _colorPanelIconButtons.Length)
            {
                return;
            }

            _colorPanelIconButtons[_currentColorSelectedIndex].IsSelected = false;
            _currentColorSelectedIndex += GridSize;
            _colorPanelIconButtons[_currentColorSelectedIndex].IsSelected = true;
        }
    }
}