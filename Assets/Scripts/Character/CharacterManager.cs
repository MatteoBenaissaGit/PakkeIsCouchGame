using System;
using Art.Script;
using Art.Test.Dissolve;
using Character.Data.Character;
using Character.State;
using Kayak;
using SceneTransition;
using Tools.SingletonClassBase;
using UnityEngine;
using UnityEngine.Events;

namespace Character
{
    [Serializable]
    public class PlayerStatsMultipliers
    {
        public float BreakingDistanceMultiplier = 1;
        public float MaximumSpeedMultiplier = 1;
        public float RotationSpeedMultiplier = 1;
        public float UnbalancedThresholdMultiplier = 1;
        
        public float WeaponLaunchDistanceMultiplier = 1;
        public float ChargeTimeReducingMultiplier = 1;
        public float ExperienceGainMultiplier = 1;
        public float WeaponRecallTimeMultiplier = 1;
    }
    
    public class CharacterManager : Singleton<CharacterManager>
    {
        #region Properties

        [field: SerializeField] public CharacterStateBase CurrentStateBaseProperty { get; private set; }
        [field: SerializeField] public KayakController KayakControllerProperty { get; private set; }
        [field: SerializeField] public InputManagement InputManagementProperty { get; private set; }
        [field: SerializeField] public Animator PaddleAnimatorProperty { get; private set; }
        [field: SerializeField] public Animator CharacterAnimatorProperty { get; private set; }
        [field: SerializeField] public TransitionManager TransitionManagerProperty { get; private set; }
        [field: SerializeField] public MonoBehaviour CharacterMonoBehaviour { get; private set; }
        [field: SerializeField] public Transform WeaponSpawnPosition { get; private set; }
        [field: SerializeField] public IKControl IKPlayerControl { get; private set; }
        [field: SerializeField] public PlayerParameters Parameters { get; set; }
        [field: SerializeField] public PlayerAbilities Abilities { get; set; }
        [field: SerializeField, Header("Weapons")] public Animator HarpoonAnimator { get; private set; }
        [field: SerializeField] public Animator NetAnimator { get; private set; }
        [field: SerializeField] public WeaponMeshController HarpoonMeshController { get; private set; }
        [field: SerializeField] public WeaponMeshController NetMeshController { get; private set; }

        #endregion

        [Header("Character Data")]
        public CharacterData Data;
        [Range(0, 360)] public float BaseOrientation;
        [Tooltip("The number of times the button has been pressed"), ReadOnly]
        public int NumberButtonIsPressed = 0;
        [Header("VFX")]
        public ParticleSystem WeaponChargedParticleSystem;
        public ParticleSystem SplashLeft;
        public ParticleSystem SplashRight;

        [Header("Events")] public UnityEvent StartGame;
        public UnityEvent OnPaddle;
        public UnityEvent OnEnterSprint;
        public UnityEvent OnStopSprint;
        
        [HideInInspector] public float WeaponCooldown;
        [HideInInspector] public float WeaponCooldownBase;
        [HideInInspector] public float InvincibilityTime;
        [HideInInspector] public bool ProjectileIsInAir;
        [HideInInspector] public bool IsGameLaunched;

        [ReadOnly] public bool SprintInProgress = false;
        [ReadOnly] public bool InWaterFlow = false;

        public PlayerStatsMultipliers PlayerStats;

        protected override void Awake()
        {
            PlayerStats = new PlayerStatsMultipliers();
            
            base.Awake();
            Cursor.visible = false;
            CharacterMonoBehaviour = this;
        }

        private void Start()
        {
            CharacterNavigationState navigationState = new CharacterNavigationState();
            CurrentStateBaseProperty = navigationState;
            CurrentStateBaseProperty.Initialize();

            CurrentStateBaseProperty.EnterState(this);

            //rotate kayak
            Transform kayakTransform = KayakControllerProperty.transform;
            kayakTransform.eulerAngles = new Vector3(0, BaseOrientation, 0);
        }
        
        private void Update()
        {
            CurrentStateBaseProperty.UpdateState(this);
            
            if (CurrentStateBaseProperty.IsDead == false)
            {
            }

            //anim
            if (IKPlayerControl.CurrentType != IKType.Paddle || IKPlayerControl.Type == IKType.Paddle)
            {
                return;
            }
            CurrentStateBaseProperty.TimeBeforeSettingPaddleAnimator -= Time.deltaTime;
            if (CurrentStateBaseProperty.TimeBeforeSettingPaddleAnimator <= 0)
            {
                IKPlayerControl.SetPaddle();
            }
        }
        private void FixedUpdate()
        {
            CurrentStateBaseProperty.FixedUpdate(this);
        }
        
        public void SwitchState(CharacterStateBase stateBaseCharacter)
        {
            CurrentStateBaseProperty.ExitState(this);
            CurrentStateBaseProperty = stateBaseCharacter;
            stateBaseCharacter.EnterState(this);
        }

        public void SendDebugMessage(string message)
        {
            Debug.Log(message);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            UnityEngine.Camera mainCamera = UnityEngine.Camera.main;

            Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);
            Vector3 rayDirection = mainCamera.ViewportPointToRay(screenCenter).direction;
            Ray ray = new Ray(mainCamera.transform.position, rayDirection);

            Color gizmoColor = new Color(1f, 0.36f, 0.24f);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Gizmos.color = gizmoColor;

                Gizmos.DrawSphere(hit.point, 1f);
            }

            Transform camera = UnityEngine.Camera.main.transform;
            for (int i = 0; i < Data.AutoAimNumberOfCastStep; i++)
            {
                break;
                float positionMultiplier = Mathf.Clamp((Data.AutoAimDistanceBetweenEachStep * i), 1, 10000);
                Vector3 newPosition = camera.position + camera.forward * positionMultiplier;
                float radiusMultiplier = Mathf.Clamp(Vector3.Distance(camera.position, newPosition) / 5, 1, 10000);
                float radius = Data.AutoAimSize * radiusMultiplier;
                Gizmos.DrawWireSphere(newPosition, radius);
            }
        }

#endif
    }

    [Serializable]
    public struct PlayerParameters
    {
        public bool AutoAim;
        public bool InversedControls;
        public bool Language;
    }
    [Serializable]
    public struct PlayerAbilities
    {
        public bool SprintUnlock;
        public bool CanDestroyIceberg;
    }
}