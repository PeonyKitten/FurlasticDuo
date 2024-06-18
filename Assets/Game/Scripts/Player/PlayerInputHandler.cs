using System;
using Game.Scripts.Grab;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
            Debug.Assert(playerInputType == PlayerInputType.Dog, $"Wrong PlayerInputType - Expected 'Dog', got: {playerInputType}");

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
            Debug.Log("Bark");
            if (dog)
            {
                dog.SendMessage("OnBark");
                dog.SendMessage("OnJump");
            }
        }

        private void OnDogMovement(InputValue value)
        {
            Debug.Log("Dog Move");
            if (dog)
            {
                dog.SendMessage("OnMovement", value);
            }
        }
        
        private void OnCatJump()
        {
            Debug.Log("Cat Jump");
            if (cat)
            {
                cat.SendMessage("OnJump");
            }
        }

        private void OnCatMovement(InputValue value)
        {
            Debug.Log("Cat Move");
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