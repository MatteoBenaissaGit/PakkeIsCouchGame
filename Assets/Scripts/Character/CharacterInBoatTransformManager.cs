using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class CharacterInBoatTransformManager : MonoBehaviour
    {
        [field:SerializeField] public Transform KayakTransform { get; private set; }
    }
}
