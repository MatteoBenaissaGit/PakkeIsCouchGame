using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Buttons
{
    public abstract class ButtonController : MonoBehaviour
    {
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                SetSelected(_isSelected);
            }
        }
        public bool IsDisplayingSubMenu;
        
        protected bool IsPressed;
        
        [SerializeField] private Transform _buttonTransform;
        [SerializeField] private Image _selectOverlayImage;

        private bool _isSelected;
        private Vector3 _baseSize;

        private void Awake()
        {
            _selectOverlayImage.DOKill();
            _selectOverlayImage.DOFade(0, 0);
            _selectOverlayImage.DOComplete();
            
            _baseSize = _buttonTransform.localScale;
            
            InternalAwake();
        }

        protected abstract void InternalAwake();

        protected virtual void SetSelected(bool isSelected)
        {
            float animationTime = isSelected ? 0.2f : 0.1f;
            _buttonTransform.DOKill();
            _buttonTransform.DOScale(isSelected ? _baseSize * 1f : _baseSize, animationTime);
            _selectOverlayImage.DOKill();
            _selectOverlayImage.DOFade(isSelected ? 1f : 0, animationTime);
        }

        public virtual void Press()
        {
            if (IsSelected == false)
            {
                return;
            }
            
            IsPressed = IsPressed == false;
            SetPressed(IsPressed);
        }

        public virtual void Left()
        {
            
        }

        public virtual void Right()
        {
            
        }

        public virtual void Up()
        {
            
        }

        public virtual void Down()
        {
            
        }

        public virtual void Select()
        {
            
        }

        public abstract void SetPressed(bool isPressed);
    }
}
