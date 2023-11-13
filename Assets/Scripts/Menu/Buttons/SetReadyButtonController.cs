using UnityEngine;
using UnityEngine.UI;

namespace Menu.Buttons
{
    public class SetReadyButtonController : ButtonController
    {
        public bool IsReady { get; private set; }
        
        [SerializeField] private Sprite _notReadySprite;
        [SerializeField] private Sprite _readySprite;
        [SerializeField] private Image _iconImage;
    
        protected override void InternalAwake()
        {
        
        }

        public override void SetPressed(bool isPressed)
        {
            IsReady = IsReady == false;
            _iconImage.sprite = IsReady ? _readySprite : _notReadySprite;
        }
    }
}
