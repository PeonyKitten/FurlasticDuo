// PlayerInputHandler.cs
// Alvin Philips
// 2024-06-20
// Acts as an interface between a PlayerInput and either both or one of the PlayerControllers.

using System;
using FD.Game;
using FD.Game.States;
using FD.Grab;
using FD.Patterns;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

namespace FD.Player
{
    [Serializable]
    public enum PlayerInputDevice
    {
        Unassigned,
        Keyboard,
        Xbox,
        PS5,
        PS4,
        GenericGamepad,
    }

    [Serializable]
    public class PlayerInputHandlerEvent : UnityEvent<PlayerInputHandler>
    {
        
    }
    
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler: MonoBehaviour
    {
        public PlayerInput playerInput;
        public PlayerInputDevice device = PlayerInputDevice.Unassigned;

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
        
        public PlayerInputType InputType => playerInputType;

        public PlayerInputHandlerEvent onPlayerDeviceChanged;

        public void DefaultSetup()
        {
            SetupPlayer(PlayerInputType.Combined);
            SetupGrabActions();
        }

        private void OnDisable()
        {
            InputSystem.onActionChange -= OnActionChange;
        }
        
        public void SetupPlayer(PlayerInputType playerType)
        {
            if (PlayManager.HasInstance())
            {
                cat = PlayManager.Instance.cat;
                dog = PlayManager.Instance.dog;
            }
            else
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
            }
            
            playerInput = GetComponent<PlayerInput>();
            UpdateControlScheme(playerInput);
            
            playerInputType = playerType;
            
            // Subscribe to pause and unpause events
            EventBus<GameEvents>.Subscribe(GameEvents.Paused, () => playerInput.DeactivateInput());
            EventBus<GameEvents>.Subscribe(GameEvents.Unpaused, () => playerInput.ActivateInput());
            
            switch (playerInputType)
            {
                case PlayerInputType.Cat:
                    playerInput.SwitchCurrentActionMap("Cat");
                    player = cat;
                    cat.InputHandler = this;
                    break;
                case PlayerInputType.Dog:
                    playerInput.SwitchCurrentActionMap("Dog");
                    player = dog;
                    dog.InputHandler = this;
                    break;
                case PlayerInputType.Combined:
                default:
                    playerInput.SwitchCurrentActionMap("Combined");
                    cat.InputHandler = this;
                    dog.InputHandler = this;
                    InputSystem.onActionChange += OnActionChange;
                    break;
            }
        }

        private void OnActionChange(object obj, InputActionChange change)
        {
            if (change == InputActionChange.ActionPerformed && obj is InputAction)
            {
                UpdateControlScheme(playerInput);
            }
        }

        private void UpdateControlScheme(PlayerInput input)
        {
            var oldDevice = device;
            
            device = input.devices[0] switch
            {
                Keyboard => PlayerInputDevice.Keyboard,
                XInputController => PlayerInputDevice.Xbox,
                DualSenseGamepadHID => PlayerInputDevice.PS5,
                DualShockGamepad => PlayerInputDevice.PS4,
                Gamepad => PlayerInputDevice.GenericGamepad,
                _ => PlayerInputDevice.Unassigned
            };

            if (device != oldDevice)
            {
                onPlayerDeviceChanged?.Invoke(this);
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