using Character;
using Kayak;
using UnityEngine;

namespace Multiplayer
{
    public class PlayerCharacterCore : MonoBehaviour
    {
        [field: SerializeField] public CharacterManager Character;
        [field: SerializeField] public KayakController Kayak;
        [field: SerializeField] public Camera Cam;
    }
}
