using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkManagerSpawner : MonoBehaviour
{
   [SerializeField] private NetworkManager _networkManager;
   private static NetworkManager m_spawnedNetworkManager;
   private void Awake()
   {
      if (m_spawnedNetworkManager == null)
      {
         m_spawnedNetworkManager = Instantiate(_networkManager);
      }
   }
}
