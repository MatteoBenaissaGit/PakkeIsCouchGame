using System;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Buttons
{
    public class ColorButtonController : ButtonController
    {
        public PlayerSelectionMenuController Controller { get; set; }
        
        [SerializeField] private ColorSelectionPanel _colorSelectionPanel;
        [SerializeField] private Transform _arrow;
        [SerializeField] private Image _colorIcon;

        private float _arrowBaseRotation;
        
        protected override void InternalAwake()
        {
            _colorSelectionPanel.gameObject.SetActive(false);
            _arrowBaseRotation = _arrow.transform.rotation.eulerAngles.z;
        }

        public override void SetPressed(bool isPressed)
        {
            IsPressed = false;
            
            if (_colorSelectionPanel.IsActive)
            {
                _colorSelectionPanel.Press();
                return;
            }
            
            _colorSelectionPanel.gameObject.SetActive(isPressed);
            _arrow.rotation = Quaternion.Euler(0,0,isPressed ? _arrowBaseRotation + 180 : _arrowBaseRotation);

            if (isPressed == false)
            {
                IsSelected = true;
                IsDisplayingSubMenu = false;
                _colorSelectionPanel.Show(false);
                return;
            }
            
            IsSelected = false;
            IsDisplayingSubMenu = true;
            _colorSelectionPanel.Show(true);
        }

        public void SetIconColor(Color color)
        {
            _colorIcon.color = color;
            Controller.PlayerNameText.color = color;
        }
        
        public override void Right()
        {
            if (_colorSelectionPanel.IsActive)
            {
                _colorSelectionPanel.Right();
                return;
            }
        }

        public override void Left()
        {
            if (_colorSelectionPanel.IsActive)
            {
                _colorSelectionPanel.Left();
                return;
            }
        }

        public override void Up()
        {
            if (_colorSelectionPanel.IsActive)
            {
                _colorSelectionPanel.Up();
                return;
            }
        }

        public override void Down()
        {
            if (_colorSelectionPanel.IsActive)
            {
                _colorSelectionPanel.Down();
                return;
            }
        }

        public override void Select()
        {
            if (_colorSelectionPanel.IsActive)
            {
                _colorSelectionPanel.Press();
                return;
            }
        }
    }
}