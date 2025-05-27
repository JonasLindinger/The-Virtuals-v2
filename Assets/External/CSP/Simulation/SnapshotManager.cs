﻿using System.Collections.Generic;
using CSP.Data;
using CSP.Input;
using CSP.Items;
using CSP.Object;
using CSP.Player;
using UnityEngine;

namespace CSP.Simulation
{
    public static class SnapshotManager
    {
        public static uint TickRate => PhysicsTickSystem.TickRate;
        public static uint CurrentTick => PhysicsTickSystem.CurrentTick;
            
        public static TickSystem PhysicsTickSystem;

        public static Dictionary<ulong, NetworkedObject> NetworkedObjects  = new Dictionary<ulong, NetworkedObject>(); // NetworkObjectId | PredictionObject
        
        private static GameState[] _gameStates;
        
        #if Client
        // Local Client Input Saving
        private static InputCollector _inputCollector;
        private static ClientInputState[] _inputStates;
        #elif Server
        private static uint _latestGameStateTick;
        #endif
        
        public static void KeepTrack(uint tickRate, uint startingTickOffset = 0)
        {
            Physics.simulationMode = SimulationMode.Script;
            
            if (_gameStates == null)
                _gameStates = new GameState[NetworkRunner.NetworkSettings.stateBufferSize];
            
            #if Client
            if (_inputStates == null)
                _inputStates = new ClientInputState[NetworkRunner.NetworkSettings.inputBufferSize];
            #endif
            
            PhysicsTickSystem = new TickSystem(tickRate, startingTickOffset);
            
            PhysicsTickSystem.OnTick += OnTick;
        }

        public static void StopTracking()
        {
            Physics.simulationMode = SimulationMode.FixedUpdate;
            
            PhysicsTickSystem.OnTick -= OnTick;
            
            PhysicsTickSystem = null;
        }
        
        /// <summary>
        /// Every Networked Object registered will be included in the GameState.
        /// It wouldn't make to Unregister Objects so we don't have a method for that!
        /// </summary>
        /// <param name="id">NetworkId</param>
        /// <param name="networkedObject"></param>
        public static void RegisterNetworkedObject(ulong id, NetworkedObject networkedObject)
        {
            NetworkedObjects.Add(id, networkedObject);
        }
        
        /// <summary>
        /// Saves the current GameState.
        /// </summary>
        /// <param name="tick">Current Tick</param>
        public static void TakeSnapshot(uint tick)
        {
            GameState currentGameState = GetCurrentState(tick);

            _gameStates[(int)tick % _gameStates.Length] = currentGameState;
            #if Server
            _latestGameStateTick = tick;
            #endif
        }
        
        /// <summary>
        /// Returns the current GameState.
        /// </summary>
        /// <param name="tick">Current Tick</param>
        public static GameState GetCurrentState(uint tick)
        {
            GameState currentGameState = new GameState();
            currentGameState.Tick = tick;
            
            foreach (var kvp in NetworkedObjects)
            {
                ulong networkId = kvp.Key;
                NetworkedObject networkedObject = kvp.Value;

                IState state = networkedObject.GetCurrentState();
                
                currentGameState.States.Add(networkId, state);
            }
            
            return currentGameState;
        }

        public static void Update(float deltaTime) => PhysicsTickSystem?.Update(deltaTime);
        
        private static void OnTick(uint tick)
        {
            // 1. Simulate Physics
            Physics.Simulate(PhysicsTickSystem.TimeBetweenTicks);
            
            // 2. Save the current State
            TakeSnapshot(tick);
            
            #if Client
            // 2.5 Collect Client Data and Input
            if (PlayerInputNetworkBehaviour.LocalPlayer != null)
            {
                IData data = PlayerInputNetworkBehaviour.LocalPlayer.GetPlayerData();
            
                // Collect input
                ClientInputState clientInputState = GetInputState(tick, data);
            }
            #endif
            
            // 3. Update all Players (Server moves everyone, Client predicts his own player)
            PickUpItem.UpdatePickUpItems(tick, false);
            PlayerInputNetworkBehaviour.UpdatePlayersWithAuthority(tick, false);
        }

        public static void RecalculatePhysicsTick(uint tick)
        {
            // 1. Simulate Physics
            Physics.Simulate(PhysicsTickSystem.TimeBetweenTicks);
            
            // 2. Save the current State
            TakeSnapshot(tick);
            
            // 3. Update all Players (Server moves everyone, Client predicts his own player)
            PickUpItem.UpdatePickUpItems(tick, true);
            PlayerInputNetworkBehaviour.UpdatePlayersWithAuthority(tick, true);
        }
        
        #if Client
        public static ClientInputState GetInputState(uint tick, IData data)
        {
            if (!_inputCollector)
                _inputCollector = InputCollector.GetInstance();
            
            // If we already collected input for this tick, we reuse it.
            if (_inputStates[tick % _inputStates.Length] != null)
                if (_inputStates[tick % _inputStates.Length].Tick == tick)
                    return _inputStates[tick % _inputStates.Length];
            
            ClientInputState clientInputState = _inputCollector.GetClientInputState(tick);
            clientInputState.Data = data;
            _inputStates[tick % _inputStates.Length] = clientInputState;

            return clientInputState;
        }
        #endif

        /// <summary>
        /// Apply's the state on the object with the corresponding network Id
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="tick"></param>
        /// <param name="state"></param>
        /// <returns>Return's if the prediction was wrong</returns>
        public static void ApplyState(ulong networkId, uint tick, IState state)
        {
            if (!NetworkedObjects.ContainsKey(networkId)) return;
            NetworkedObject networkedObject = NetworkedObjects[networkId];

            // Check for null reference
            if (networkedObject == null)
            {
                Debug.LogWarning("Something went wrong!");
                return;
            }
            
            networkedObject.ApplyState(tick, state);
        }
        
        #if Server
        public static GameState GetLatestGameState()
        {
            return _gameStates[(int)_latestGameStateTick % _gameStates.Length];
        }
        #endif
        
        public static GameState GetGameState(uint tick)
        {
            return _gameStates[(int)tick % _gameStates.Length];
        }
        
        public static void SaveGameState(GameState gameState)
        {
            _gameStates[(int) gameState.Tick % _gameStates.Length] = gameState;
        }

        #if Server
        public static void ApplyGameState(uint tick)
        {
            GameState gameState = _gameStates[(int)tick % _gameStates.Length];
            
            if (gameState == null)
            {
                Debug.LogWarning("Something went wrong!");
                return;
            }
            else if (gameState.Tick != tick) 
            {
                Debug.LogWarning("Something went wrong!");
                return;
            }
            else
                ApplyGameState(gameState);
        }

        public static void ApplyGameState(GameState gameState)
        {
            foreach (var kvp in gameState.States)
            {
                ulong objectId = kvp.Key;
                IState state = kvp.Value;

                ApplyState(objectId, gameState.Tick, state);
            }
            
            Physics.SyncTransforms();
        }
        #endif
    }
}