﻿using CSP.Simulation;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Network
{
    public class PlayerState : IState
    {
        public Vector3 Position;
        public Vector2 Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
        public float JumpCooldownTimer;
        public short EquippedItem;
        public int Health;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Rotation);
            serializer.SerializeValue(ref Velocity);
            serializer.SerializeValue(ref AngularVelocity);
            serializer.SerializeValue(ref JumpCooldownTimer);
            serializer.SerializeValue(ref EquippedItem);
            serializer.SerializeValue(ref Health);
        }

        static PlayerState() => StateFactory.Register((int) StateTypes.Player,() => new PlayerState());
        
        public int GetStateType() => (int) StateTypes.Player;
    }
}