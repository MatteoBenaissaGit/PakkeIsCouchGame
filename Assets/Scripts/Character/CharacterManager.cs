using System;
using Art.Script;
using Character.Data.Character;
using Character.State;
using Kayak;
using Multiplayer;
using Racing;
using SceneTransition;
using Tools.SingletonClassBase;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
    
    public class CharacterManager : MonoBehaviour
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
        
        [HideInInspector] public float InvincibilityTime;
        [HideInInspector] public bool IsGameLaunched;

        [ReadOnly] public bool SprintInProgress = false;
        [ReadOnly] public bool InWaterFlow = false;

        public PlayerStatsMultipliers PlayerStats;
        public PlayerCharacterCore Core;

        [Header("Objects")] 
        [SerializeField] private IcebergDroppable _icebergPrefab;
        [SerializeField] private PaddleHitEffect _paddleHitPrefab;
        [SerializeField] private float _freezeTimeDuration = 5.5f;
        [SerializeField] private float _paddleHitForce = 7f;
        [SerializeField] private float _boostTime = 3f;
        [SerializeField] private float _boostForce = 1.5f;
        [SerializeField] private float _paddleHitDistance = 12.5f;
        
        private void Awake()
        {
            PlayerStats = new PlayerStatsMultipliers();
            
            Cursor.visible = false;
            CharacterMonoBehaviour = this;
        }

        private void Start()
        {
            CharacterNavigationState navigationState = new CharacterNavigationState(this);
            CurrentStateBaseProperty = navigationState;
            CurrentStateBaseProperty.Initialize(this);

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

        [ReadOnly] public bool InputPaddle;
        public void OnInputPaddle(InputAction.CallbackContext context)
        {
            //Debug.Log("paddle " + context.performed);
            InputPaddle = context.performed;
        }

        [ReadOnly] public bool InputLeft;
        [ReadOnly] public bool InputRight;
        public void OnInputLeft(InputAction.CallbackContext context)
        {
            InputLeft = context.performed;
        }
        public void OnInputRight(InputAction.CallbackContext context)
        {
            InputRight = context.performed;
        }
        public void OnInputObject(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                UseObject();
            }
        }

        public bool HasObject;
        private MysteryObject _currentObject;
        public void GetMysteryObject(MysteryObject mysteryObject)
        {
            if (HasObject)
            {
                return;
            }
            
            HasObject = true;
            _currentObject = mysteryObject;
            
            Core.GameplayUI.SetObjectUI(_currentObject);
        }

        private void UseObject()
        {
            if (_currentObject == MysteryObject.None || HasObject == false)
            {
                return;
            }
            HasObject = false;

            Debug.Log("use object " + _currentObject);
            switch (_currentObject)
            {
                case MysteryObject.Freezer:
                    RaceManager.Instance.FreezeTimeTimer = _freezeTimeDuration;
                    Core.GameplayUI.SetTimerFreeze();
                    break;
                case MysteryObject.Iceberg:
                    IcebergDroppable iceberg = Instantiate(_icebergPrefab);
                    iceberg.transform.position = KayakControllerProperty.transform.position - KayakControllerProperty.transform.forward * 2;
                    break;
                case MysteryObject.Boost:
                    KayakControllerProperty.TempBoostTime = _boostTime;
                    KayakControllerProperty.TempBoostForce = _boostForce;
                    KayakControllerProperty.BoostParticles.Play();
                    break;
                case MysteryObject.PaddleHit:
                    Instantiate(_paddleHitPrefab, KayakControllerProperty.transform);
                    RaycastHit[] hits = new RaycastHit[20];
                    Physics.SphereCastNonAlloc(transform.position, _paddleHitDistance, Vector3.up, hits);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        if ( hits[i].collider == null)
                        {
                            continue;
                        }
                        
                        if (hits[i].collider.TryGetComponent(out KayakController kayak) == false
                            || kayak == KayakControllerProperty)
                        {
                            if (hits[i].collider.TryGetComponent(out Rigidbody rb))
                            {
                                Debug.Log("force");
                                Vector3 forceToRigidbody = (rb.transform.position - KayakControllerProperty.transform.position).normalized;
                                forceToRigidbody = new Vector3(forceToRigidbody.x, 0, forceToRigidbody.z).normalized;
                                rb.AddForce(forceToRigidbody * _paddleHitForce * 2.5f, ForceMode.Impulse);
                            }
                            continue;
                        }
                        Vector3 force = (kayak.transform.position - KayakControllerProperty.transform.position).normalized;
                        force = new Vector3(force.x, 0, force.z).normalized;
                        kayak.TempForceAdd = force * _paddleHitForce;
                    }
                    break;
            }
            
            _currentObject = MysteryObject.None;
            Core.GameplayUI.SetObjectUI(_currentObject);
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

            Gizmos.DrawWireSphere(transform.position, _paddleHitDistance);
        }

#endif
    }

    [Serializable]
    public struct PlayerParameters
    {
        public bool InversedControls;
    }
}
