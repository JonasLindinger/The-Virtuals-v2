﻿using System.Collections.Generic;
using CSP.Object;
using CSP.Player;
using CSP.Simulation;
using UnityEngine;
using UnityEngine.Serialization;

namespace CSP.Items
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class PickUpItem : PredictedNetworkedObject
    {
        #if Client
        public static Dictionary<ulong, PickUpItem> PickUpAbleItems = new Dictionary<ulong, PickUpItem>();
        #endif
        public static Dictionary<ulong, PickUpItem> PickUpItems = new Dictionary<ulong, PickUpItem>();

        [SerializeField] private GameObject pickedUpPrefab;
        public Collider collider;
        [SerializeField] private float pickUpRange;
        [SerializeField] private float dropForwardForce;
        [SerializeField] private float dropUpwardForce;

        [HideInInspector] public bool pickedUp;
        public PlayerInputNetworkBehaviour owner;
        
        [HideInInspector] public Rigidbody rb;
        public Transform playerCamera;
        private Transform _gunContainer;
        
        private GameObject _pickedUpPrefabInstance;

        private float _mass;
        
        #if Client
        private Transform _player;
        #endif
        private bool _usable;

        public override void OnNetworkSpawn()
        {
            rb = GetComponent<Rigidbody>();
            
            pickedUp = owner != null;
            _usable = pickedUp;
            
            PickUpItems.Add(NetworkObjectId, this);

            if (!pickedUp)
            {
                rb.isKinematic = false;
                collider.isTrigger = false;
                
                OnDropped();
            }
            else
            {
                rb.isKinematic = true;
                collider.isTrigger = true;
                OnPickedUp();
            }
            
            SetUp();
        }

        #if Client
        protected void Update()
        {
            if (pickedUp)
            {
                if (PickUpAbleItems.ContainsKey(NetworkObjectId))
                {
                    PickUpAbleItems.Remove(NetworkObjectId);
                    UnHighlight();
                }
                return;
            }
            
            // Get the local Player and if there is non, we just return early.
            if (_player == null)
                _player = PlayerInputNetworkBehaviour.LocalPlayer.transform;

            if (_player == null)
            {
                if (PickUpAbleItems.ContainsKey(NetworkObjectId))
                {
                    PickUpAbleItems.Remove(NetworkObjectId);
                    UnHighlight();
                }
                return;
            }

            if (IsAbleToPickUp(_player))
            {
                if (PickUpAbleItems.ContainsKey(NetworkObjectId))
                {
                    // We are good to go
                }
                else
                {
                    // Set this weapon to be able to get picked up
                    PickUpAbleItems.Add(NetworkObjectId, this);
                    Highlight();
                }
            }
            else
            {
                if (PickUpAbleItems.ContainsKey(NetworkObjectId))
                {
                    // Set this weapon to be able to get picked up
                    PickUpAbleItems.Remove(NetworkObjectId);
                    UnHighlight();
                }
                else
                {
                    // We are good to go
                }
            }
        }
        #endif
        
        public void Trigger(uint tick, bool isUsing, uint latestReceivedServerGameStateTick)
        {
            Use(tick, _usable && isUsing, latestReceivedServerGameStateTick);
        }

        protected abstract void SetUp();
        protected abstract void Use(uint tick, bool isUsing, uint latestReceivedServerGameStateTick);
        protected abstract void OnTick();
        protected abstract void OnPickedUp();
        protected abstract void OnDropped();
        protected abstract void Highlight();
        protected abstract void UnHighlight();
        public abstract int GetItemType();

        public static void UpdatePickUpItems(uint tick, bool isReconciliation)
        {
            foreach (var kvp in PickUpItems)
            {
                ulong objectId = kvp.Key;
                PickUpItem item = kvp.Value;
                
                item.OnTick();
            }
        }
        
        public bool IsAbleToPickUp(Transform player)
        {
            Vector3 distanceToPlayer = player.position - transform.position;
            return distanceToPlayer.magnitude <= pickUpRange && !pickedUp;
        }
        
        public void PickUp(PlayerInputNetworkBehaviour player, Transform gunContainer, Transform playerCamera)
        {
            _mass = rb.mass;
            rb.mass = 0;
            
            owner = player;
            this.playerCamera = playerCamera;
            _gunContainer = gunContainer;
            
            pickedUp = true;

            // Make Rigidbody kinematic
            rb.isKinematic = true;
            collider.isTrigger = true;

            foreach (Transform child in transform)
                child.gameObject.SetActive(false);
            
            _pickedUpPrefabInstance = Instantiate(pickedUpPrefab, Vector3.zero, Quaternion.identity, _gunContainer);
            _pickedUpPrefabInstance.transform.localPosition = Vector3.zero;
            _pickedUpPrefabInstance.transform.localRotation = Quaternion.identity;
            
            _usable = true;
            OnPickedUp();
        }

        public void Drop(uint tick)
        {
            if (!pickedUp) return;
            if (owner == null) return;
            
            rb.mass = _mass;
            if (owner.IsOwner || owner.IsServer)
            {
                owner.ApplyLatestCameraState();
            }
            
            RigidbodyInterpolation interpolation = rb.interpolation;
            rb.interpolation = RigidbodyInterpolation.None;
            
            rb.isKinematic = false;
            collider.isTrigger = false;
            
            rb.MovePosition(_pickedUpPrefabInstance.transform.position);
            rb.MoveRotation(_pickedUpPrefabInstance.transform.rotation);
            Physics.SyncTransforms();
            
            foreach (Transform child in transform)
                child.gameObject.SetActive(true);
            
            // Gun carry's momentum of the owner. So ideally the weaponVelocity is the rigidbody velocity of the player.
            rb.linearVelocity = owner.GetLinearVelocity();
            rb.angularVelocity = Vector3.zero;
            
            // Add Forces
            rb.AddForce(playerCamera.forward * dropForwardForce, ForceMode.Impulse);
            rb.AddForce(playerCamera.up * dropUpwardForce, ForceMode.Impulse);
            
            // Add "random" rotation
            float random = tick % 2 == 0 ? 1 : -1;
            rb.AddTorque(new Vector3(random, random, random) * 2);
            
            rb.interpolation = interpolation;
            
            Destroy(_pickedUpPrefabInstance);
            _pickedUpPrefabInstance = null;

            _usable = false;
            OnDropped();
            
            owner = null;
            pickedUp = false;
            playerCamera = null;
            _gunContainer = null;
        }
    }
}