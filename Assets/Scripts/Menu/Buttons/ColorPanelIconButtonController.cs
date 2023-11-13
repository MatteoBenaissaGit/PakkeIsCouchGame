using UnityEngine;
using UnityEngine.UI;

namespace Menu.Buttons
{
    public class ColorPanelIconButtonController : ButtonController
    {
        [SerializeField] private Image _colorIconImage;

        private ColorButtonController _colorButtonController;
        private Color _iconColor;

        protected override void InternalAwake()
        {
        }

        public override void SetPressed(bool isPressed)
        {
            _colorButtonController.SetPressed(false);
            _colorButtonController.SetIconColor(_iconColor);
        }
        
        public void SetColor(Color color, ColorButtonController controller)
        {
            _iconColor = color;
            _colorIconImage.color = _iconColor;

            _colorButtonController = controller;
        }
    }
}
