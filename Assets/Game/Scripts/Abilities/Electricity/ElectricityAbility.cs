using System;
using System.Collections.Generic;
using CucuTools.DamageSystem;
using Game.Scripts.Core;
using Game.Scripts.Player;
using Inputs;
using UnityEngine;

namespace Game.Scripts.Abilities.Electricity
{
    public class ElectricityAbility : AbilityBase, IElectricityStorage
    {
        [field: SerializeField] public bool Focused { get; private set; }
        
        [Header("Settings")]
        [Min(0)] [SerializeField] private int chargeAmountPerTime = 1;
        
        [Space]
        [Min(0)] [SerializeField] private int electricityCharge = 0;
        [Min(0)] [SerializeField] private int electricityChargeMax = 1;
        
        [Space]
        [Min(0f)] [SerializeField] private float distanceMax = 15f;
        [Min(0f)] [SerializeField] private float raycastRadius = 0.5f;
        [SerializeField] private Vector2 raycastOffset = Vector2.zero;
        [SerializeField] private LayerMask layerMask = 1;
        
        [Header("VFX")]
        [SerializeField] private ElectricityEffect electricityEffect;
        [SerializeField] private ElectricityStrikeEffect strikeEffect;
        [SerializeField] private GameObject hitEffect;

        [Header("Damage")]
        [Min(0f)] [SerializeField] private float forcePower = 0f;
        [SerializeField] private DamageSource electricityDamageSource;
            
        [Header("Input")]
        [SerializeField] private bool sendCharge;
        [SerializeField] private bool receiveCharge;
        
        private PlayerInput _playerInput;
        private ModelLoader _modelLoader;
        private ContactFilter2D _filter = new ContactFilter2D();
        private RaycastHit2D _raycastHit;
        
        private readonly List<RaycastHit2D> _hits = new List<RaycastHit2D>();
        
        private IElectricityStorage _selected;
        private IElectricityStorage _lastUsed;
        
        private Vector2 raycastOrigin => (Player ? Player.position : transform.position) + raycastOffset;
        private Vector2 raycastDirection => (worldPoint - raycastOrigin).normalized;
        private float raycastDistance => Mathf.Min(Vector2.Distance(worldPoint, raycastOrigin), distanceMax);
        
        private Vector2 worldPoint => _playerInput ? _playerInput.WorldPoint : transform.position;
 
        public event Action<bool> OnFocusChanged;
        public event Action<int> OnChargeChanged; 
        
        public int ElectricityCharge
        {
            get => electricityCharge;
            set
            {
                if (electricityCharge != value)
                {
                    electricityCharge = value;
                    
                    OnChargeChanged?.Invoke(electricityCharge);
                }
            }
        }

        public int ElectricityChargeMax => electricityChargeMax;
        
        public void Focus(bool value)
        {
            Focused = value;
            
            OnFocusChanged?.Invoke(value);
        }
        
        public int HowMuchAbleToSend(int amount)
        {
            return Mathf.Min(amount, ElectricityCharge);
        }

        public int HowMuchAbleToReceive(int amount)
        {
            var available = ElectricityChargeMax - ElectricityCharge;
            
            return Mathf.Min(amount, available);
        }

        public int SendCharge(int amount)
        {
            amount = HowMuchAbleToSend(amount);
            
            ElectricityCharge -= amount;

            return amount;
        }

        public int ReceiveCharge(int amount)
        {
            amount = HowMuchAbleToReceive(amount);
            
            ElectricityCharge += amount;
            
            return amount;
        }
        
        private void ReceiveChargeFrom(IElectricityStorage source, int chargeAmount)
        {
            var sourceAbleToSend = source.HowMuchAbleToSend(chargeAmount);
            var selfAbleToReceive = HowMuchAbleToReceive(sourceAbleToSend);
                    
            var sent = source.SendCharge(selfAbleToReceive);
            var received = ReceiveCharge(sent);
                    
            //Debug.Log($"-{sent} {source.GetName()} ==> +{received} {this.GetName()}");

            if (received > 0)
            {
                _lastUsed = source;
            }
            
            if (strikeEffect && received > 0)
            {
                strikeEffect.Play();
                strikeEffect.SetPositions(_raycastHit.point, raycastOrigin);
            }
        }
        
        private void SendChargeTo(IElectricityStorage destination, int chargeAmount)
        {
            var selfAbleToSend = HowMuchAbleToSend(chargeAmount);
            var destinationAbleToReceive = destination.HowMuchAbleToReceive(selfAbleToSend);
                    
            var sent = SendCharge(destinationAbleToReceive);
            var received = destination.ReceiveCharge(sent);
                    
            //Debug.Log($"-{sent} {this.GetName()} ==> +{received} {destination.GetName()}");

            if (received > 0)
            {
                _lastUsed = destination;
            }
            
            if (strikeEffect && received > 0)
            {
                strikeEffect.Play();
                strikeEffect.SetPositions(raycastOrigin, _raycastHit.point);
            }
        }
        
        private int Cast(List<RaycastHit2D> hits)
        {
            _filter.layerMask = layerMask;
            _filter.useLayerMask = true;
            _filter.useTriggers = false;

            return Physics2D.CircleCast(raycastOrigin, raycastRadius, raycastDirection, _filter, hits, raycastDistance);
        }
        
        private bool TryGetElectricityStorage(out IElectricityStorage storage, out RaycastHit2D raycastHit)
        {
            storage = null;
            
            var count = Cast(_hits);
            
            raycastHit = count > 0 ? _hits[0] : default;

            return raycastHit && raycastHit.collider.TryGet(out storage);
        }
        
        private void HandlePrimaryFire(bool value)
        {
            sendCharge = value;

            if (!sendCharge) return;
            
            if (_selected != null)
            {
                SendChargeTo(_selected, chargeAmountPerTime);
            }
        }
        
        private void HandleSecondaryFire(bool value)
        {
            receiveCharge = value;

            if (!receiveCharge) return;

            if (_selected != null)
            {
                ReceiveChargeFrom(_selected, chargeAmountPerTime);
            }
        }
        
        private void HandlePlayerModel(GameObject playerModel)
        {
            if (electricityEffect && playerModel.TryGetComponent<SpriteRenderer>(out var sprite))
            {
                electricityEffect.SetSpriteRendererSurface(sprite);
            }
        }
        
        private void UpdateHitEffect()
        {
            if (_raycastHit)
            {
                var hitPosition = _raycastHit.point;
                var hitRotation = Quaternion.LookRotation(Vector3.forward, _raycastHit.normal);
                hitEffect.transform.SetPositionAndRotation(hitPosition, hitRotation);
            }
            
            hitEffect.gameObject.SetActive(_raycastHit && sendCharge && ElectricityCharge > 0);
        }

        private void UpdateStrikeEffect()
        {
            if (sendCharge)
            {
                var target = _raycastHit
                    ? _raycastHit.point
                    : raycastOrigin + raycastDirection * raycastDistance;
                    
                strikeEffect.SetPositions(raycastOrigin, target);

                if (ElectricityCharge > 0)
                {
                    strikeEffect.Play();
                }
            }
            else if (receiveCharge)
            {
                if (_selected != null)
                {
                    strikeEffect.SetPositions(_raycastHit.point, raycastOrigin);
                    
                    if (_selected.ElectricityCharge > 0)
                    {
                        strikeEffect.Play();
                    }
                }
            }
        }

        private void UpdateDamage()
        {
            if (_raycastHit && sendCharge && ElectricityCharge > 0)
            {
                if (_raycastHit.collider.TryGet<DamageReceiver>(out var damageReceiver))
                {
                    var damage = electricityDamageSource.CreateDamage(damageReceiver);
                    
                    electricityDamageSource.SendDamage(damage, damageReceiver);
                }

                if (forcePower > 0 && _raycastHit.rigidbody)
                {
                    _raycastHit.rigidbody.AddForceAtPosition(raycastDirection * forcePower, _raycastHit.point, ForceMode2D.Force);
                }
            }
        }
        
        protected override void OnSetPlayer()
        {
            _playerInput = Player.GetPlayerInput();
            _modelLoader = Player.ModelLoader;
            
            _playerInput.PrimaryFireEvent += HandlePrimaryFire;
            _playerInput.SecondaryFireEvent += HandleSecondaryFire;
            
            _modelLoader.OnModelLoaded += HandlePlayerModel;
            HandlePlayerModel(_modelLoader.GetModel());
            
            electricityDamageSource.SetOwner(Player.gameObject);
        }

        public override void Drop()
        {
            base.Drop();
            
            if (ElectricityCharge > 0 && _lastUsed != null)
            {
                SendChargeTo(_lastUsed, chargeAmountPerTime);
            }
        }
        
        private void OnDestroy()
        {
            if (ElectricityCharge > 0 && _lastUsed != null)
            {
                SendChargeTo(_lastUsed, chargeAmountPerTime);
            }
            
            if (_playerInput)
            {
                _playerInput.PrimaryFireEvent -= HandlePrimaryFire;
                _playerInput.SecondaryFireEvent -= HandleSecondaryFire;
            }

            if (_modelLoader)
            {
                _modelLoader.OnModelLoaded -= HandlePlayerModel;
            }
        }
        
        private void FixedUpdate()
        {
            if (!Player) return;
            
            TryGetElectricityStorage(out var storage, out _raycastHit);

            if (_selected != storage)
            {
                _selected?.Focus(false);
                _selected = storage;
                _selected?.Focus(true);
            }

            UpdateHitEffect();

            UpdateStrikeEffect();
            
            UpdateDamage();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = sendCharge || receiveCharge ? Color.white : Color.grey;
            
            Gizmos.DrawWireSphere(Player?Player.position:transform.position, raycastDistance);
            
            Gizmos.DrawLine(raycastOrigin, worldPoint);
            Gizmos.DrawWireSphere(raycastOrigin, raycastRadius);
            Gizmos.DrawWireSphere(worldPoint, raycastRadius);

            if (_raycastHit)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(_raycastHit.point, 0.05f);
                Gizmos.DrawWireSphere(_raycastHit.point + _raycastHit.normal * raycastRadius, raycastRadius);
            }
        }
    }
}