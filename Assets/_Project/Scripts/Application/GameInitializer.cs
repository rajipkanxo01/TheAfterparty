using _Project.Scripts.Application.Core;
using UnityEngine;

namespace _Project.Scripts.Application
{
    public class GameInitializer : MonoBehaviour
    {
        private void Awake()
        {
            var gameStateService = new GameStateService();
            ServiceLocater.RegisterService(gameStateService);
        }
    }
}