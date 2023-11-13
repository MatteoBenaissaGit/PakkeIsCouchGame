using Art.Script;
using DG.Tweening;
using Kayak.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Character.State
{
    public abstract class CharacterStateBase 
    {
        protected CharacterManager CharacterManagerRef;
        public MonoBehaviour MonoBehaviourRef;
        
        public bool CanBeMoved = true;
        public bool CanCharacterMove = true;
        public bool CanCharacterMakeActions = true;
        public bool CanOpenMenus = true;
        public bool CanCharacterOpenWeapons = true;
        public bool IsDead;

        public float RotationStaticForceY = 0f;
        public float RotationPaddleForceY = 0f;

        //events
        public UnityEvent OnPaddleLeft = new UnityEvent();
        public UnityEvent OnPaddleRight = new UnityEvent();
        
        protected Transform PlayerPosition;
        
        //anim
        public float TimeBeforeSettingPaddleAnimator;
        
        protected CharacterStateBase()
        {
            if (CharacterManager.Instance != null)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            CharacterManagerRef = CharacterManager.Instance;
            MonoBehaviourRef = CharacterManager.Instance.CharacterMonoBehaviour;
        }

        public abstract void EnterState(CharacterManager character);

        public abstract void UpdateState(CharacterManager character);

        public abstract void FixedUpdate(CharacterManager character);

        public abstract void SwitchState(CharacterManager character);
        
        public abstract void ExitState(CharacterManager character);

        /// <summary>
        /// Move the velocity toward the player's facing direction
        /// </summary>
        protected void VelocityToward()
        {
            Vector3 oldVelocity = CharacterManagerRef.KayakControllerProperty.Rigidbody.velocity;
            float oldVelocityMagnitude = new Vector2(oldVelocity.x, oldVelocity.z).magnitude;
            Vector3 forward = CharacterManagerRef.KayakControllerProperty.transform.forward;
            
            Vector2 newVelocity = oldVelocityMagnitude * new Vector2(forward.x,forward.z).normalized;

            CharacterManagerRef.KayakControllerProperty.Rigidbody.velocity = new Vector3(newVelocity.x, oldVelocity.y, newVelocity.y);
        }

        public void LaunchNavigationState()
        {
            CharacterManager character = CharacterManager.Instance;
            character.WeaponChargedParticleSystem.Stop();

            CharacterNavigationState characterNavigationState = new CharacterNavigationState();
            character.SwitchState(characterNavigationState);

            CharacterManagerRef.IKPlayerControl.CurrentType = IKType.Paddle;
        }
    }
}