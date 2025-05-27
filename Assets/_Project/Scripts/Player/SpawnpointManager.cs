using System;
using System.Collections.Generic;
using _Project.Scripts.Matchmaking;
using _Project.Scripts.Network;
using _Project.Scripts.Teams;
using CSP.Object;
using Singletons;
using Unity.Netcode;
using UnityEngine;
using NetworkClient = Unity.Netcode.NetworkClient;

namespace _Project.Scripts.Player
{
    public class SpawnpointManager : MonoBehaviourSingleton<SpawnpointManager>
    {
        [Header("References")]
        [SerializeField] private NetworkObject networkPlayerPrefab;
        [Space(5)]
        [Header("1v1 Spawnpoints")]
        [SerializeField] private Transform _1v1SpawnpointA;
        [SerializeField] private Transform _1v1SpawnpointB;

        private void Start()
        {
            // Hide
            _1v1SpawnpointA.GetChild(0).gameObject.SetActive(false);
            _1v1SpawnpointB.GetChild(0).gameObject.SetActive(false);
        }

        #if Server
        public void SpawnPlayers(Team[] teams, GameMode gamemode)
        {
            switch (gamemode)
            {
                case GameMode.Unranked:
                    break;
                case GameMode.Ranked:
                    break;
                case GameMode.Custom:
                    break;
                case GameMode.Local:
                    SpawnAllPlayersFor1V1(teams);
                    break;
            }
        }

        private void SpawnAllPlayersFor1V1(Team[] teams)
        {
            Spawner.SpawnObjectPublicWithOwnerPermanent(teams[0].Players[0], networkPlayerPrefab.gameObject, _1v1SpawnpointA);
            Spawner.SpawnObjectPublicWithOwnerPermanent(teams[1].Players[0], networkPlayerPrefab.gameObject, _1v1SpawnpointB);
        }
        
        #endif
    }
}