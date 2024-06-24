// PlayerInputHandler.cs
// Alvin Philips
// 2024-06-20
// Acts as an interface between a PlayerInput and either both or one of the PlayerControllers.

using System;
using Game.Scripts.Game.States;
using Game.Scripts.Grab;
using Game.Scripts.Patterns;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler: MonoBehaviour
    {
        public PlayerInput playerInput;

        [Serializable]
        public enum PlayerInputType
        {
            Combined,
            Cat,
            Dog
        }

        [SerializeField] private PlayerInputType playerInputType = PlayerInputType.Combined;
        public PlayerController player;
        
        public PlayerController dog;
        public PlayerController cat;

        public void DefaultSetup()
        {
            SetupPlayer(PlayerInputType.Combined);
            SetupGrabActions();
        }
        
        public void SetupPlayer(PlayerInputType playerType)
        { 
            var players = FindObjectsOfType<PlayerController>();
            foreach (var playerController in players)
            {
                if (playerController.IsCat)
                {
                    cat = playerController;
                }

                if (playerController.IsDog)
                {
                    dog = playerController;
                }
            }
            
            playerInput = GetComponent<PlayerInput>();
            playerInputType = playerType;
            
            // Subscribe to pause and unpause events
            EventBus<GameEvents>.Subscribe(GameEvents.Paused, () => playerInput.DeactivateInput());
            EventBus<GameEvents>.Subscribe(GameEvents.Unpaused, () => playerInput.ActivateInput());
            
            switch (playerInputType)
            {
                case PlayerInputType.Cat:
                    playerInput.SwitchCurrentActionMap("Cat");
                    player = cat;
                    break;
                case PlayerInputType.Dog:
                    playerInput.SwitchCurrentActionMap("Dog");
                    player = dog;
                    break;
                case PlayerInputType.Combined:
                default:
                    playerInput.SwitchCurrentActionMap("Combined");
                    break;
            }
        } 

        private void OnMovement(InputValue value)
        {
            if (player)
            {
                player.OnMovement(value);
            }
        }

        private void OnBark()
        {
            if (player)
            {
                player.SendMessage("OnBark");
            }
        }
        
        private void OnJump()
        {
            if (player)
            {
                player.SendMessage("OnJump");
            }
        }
        
        private void OnDogBark()
        {
            if (dog)
            {
                dog.SendMessage("OnBark");
                dog.SendMessage("OnJump");
            }
        }

        private void OnDogMovement(InputValue value)
        {
            if (dog)
            {
                dog.SendMessage("OnMovement", value);
            }
        }
        
        private void OnCatJump()
        {
            if (cat)
            {
                cat.SendMessage("OnJump");
            }
        }

        private void OnCatMovement(InputValue value)
        {
            if (cat)
            {
                cat.SendMessage("OnMovement", value);
            }
        }

        private void OnGrab()
        {
            if (player)
            {
                player.SendMessage("OnGrab");
            }
        }
        
        private void OnDogGrab()
        {
            if (dog)
            {
                dog.SendMessage("OnGrab");
            }
        }
        
        private void OnCatGrab()
        {
            if (cat)
            {
                cat.SendMessage("OnGrab");
            }
        }

        public void SetupGrabActions()
        {
            if (playerInputType == PlayerInputType.Combined) {
                cat?.GetComponent<Grabbing>().SetGrabAction(playerInput.actions["CatGrab"]);
                dog?.GetComponent<Grabbing>().SetGrabAction(playerInput.actions["DogGrab"]);
            }
            else
            {
                player?.GetComponent<Grabbing>().SetGrabAction(playerInput.actions["Grab"]);
            }
        }
    }
}