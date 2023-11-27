using System;
using System.Collections.Generic;
using Character;
using Character.State;
using Kayak.Data;
using Racing;
using Sound;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using WaterAndFloating;
using Random = System.Random;

namespace Kayak
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody))]
    public class KayakController : MonoBehaviour
    {
        public KayakData Data;
        [field:SerializeField] public TrailRenderer Trail { get; set; }
        [field:SerializeField] public SpriteRenderer SpriteColor { get; set; }
        [field:SerializeField] public GameObject Mesh { get; set; }

        [field:SerializeField] public CharacterManager Character;
        
        [field:SerializeField, Tooltip("Reference of the kayak rigidbody")] public Rigidbody Rigidbody { get; private set; }
        [ReadOnly, Tooltip("If this value is <= 0, the drag reducing will be activated")] public float DragReducingTimer;
        [ReadOnly, Tooltip("= is the drag reducing method activated ?")] public bool CanReduceDrag = true;
       
        [Header("VFX"), SerializeField] public ParticleSystem LeftPaddleParticle;
        [SerializeField] public ParticleSystem RightPaddleParticle;
        [SerializeField] public ParticleSystem BoostParticles;
        
        [Header("Events")] 
        public UnityEvent OnKayakCollision;
        public UnityEvent OnKayakSpeedHigh;
        [SerializeField] private float _magnitudeToLaunchEventSpeed;
        [SerializeField] private Vector2 _speedEventRecurrenceRandomBetween;
        
        public float TempBoostTime { get; set; }
        public float TempBoostForce { get; set; }
        public Vector3 TempForceAdd { get; set; }

        //privates
        private float _speedEventCountDown;
        private float _particleTimer = -1;
        private CharacterNavigationState.Direction _particleSide;
        
        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
        private void Update()
        {
            ClampVelocity();
            ManageParticlePaddle();
            ManageHighSpeedEvent();
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnKayakCollision.Invoke();
        }

        public float PositionBoost { get; set; } 
        /// <summary>
        /// Clamp the kayak velocity x & z between -maximumFrontVelocity & maximumFrontVelocity
        /// </summary>
        private void ClampVelocity()
        {
            TempBoostTime -= Time.deltaTime;
            if (TempBoostTime <= 0 && BoostParticles.isPlaying)
            {
                BoostParticles.Stop();
            }
            float tempBoost = TempBoostTime > 0 ? TempBoostForce : 1f;
            PositionBoost = 1f;
            if (Character.Core.Position > 1)
            {
                PositionBoost = 1 + (Character.Core.Position / 9f);
            }

            Vector3 velocity = Rigidbody.velocity;
            KayakParameters kayakValues = Data.KayakValues;

            float velocityX = velocity.x;
            float maxClamp = Character.SprintInProgress ? 
                kayakValues.MaximumFrontSprintVelocity :
                kayakValues.MaximumFrontVelocity * Character.PlayerStats.MaximumSpeedMultiplier;
            velocityX = Mathf.Clamp(velocityX, -maxClamp * PositionBoost, maxClamp * PositionBoost);

            float velocityZ = velocity.z;
            velocityZ = Mathf.Clamp(velocityZ, -maxClamp * PositionBoost, maxClamp * PositionBoost);

            TempForceAdd = Vector3.Lerp(TempForceAdd, Vector3.zero, 0.01f);
            if (TempForceAdd.magnitude < 0.1f)
            {
                TempForceAdd = Vector3.zero;
            }
            Rigidbody.velocity = new Vector3(velocityX * tempBoost, 0, velocityZ * tempBoost);
        }
        
        public void PlayPaddleParticle(CharacterNavigationState.Direction side)
        {
            _particleTimer = Data.TimeToPlayParticlesAfterPaddle;
            _particleSide = side;
        }

        private void ManageParticlePaddle()
        {
            if (_particleTimer > 0)
            {
                _particleTimer -= Time.deltaTime;
                if (_particleTimer <= 0)
                {
                    _particleTimer = -1;
                    switch (_particleSide)
                    {
                        case CharacterNavigationState.Direction.Left:
                            if (LeftPaddleParticle != null)
                            {
                                LeftPaddleParticle.Play();
                            }
                            break;
                        case CharacterNavigationState.Direction.Right:
                            if (RightPaddleParticle != null)
                            {
                                RightPaddleParticle.Play();
                            }
                            break;
                    }
                }
            }
        }

        private void ManageHighSpeedEvent()
        {
            _speedEventCountDown -= Time.deltaTime;
            if (_speedEventCountDown > 0 || Rigidbody.velocity.magnitude < _magnitudeToLaunchEventSpeed)
            {
                return;
            }

            OnKayakSpeedHigh.Invoke();
            _speedEventCountDown = UnityEngine.Random.Range(_speedEventRecurrenceRandomBetween.x, _speedEventRecurrenceRandomBetween.y);
        }
        
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (TempForceAdd == Vector3.zero)
            {
                return;
            }
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + TempForceAdd * 5f);
        }

#endif
    }
}