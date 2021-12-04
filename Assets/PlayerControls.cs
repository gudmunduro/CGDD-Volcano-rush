// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""c335585b-e311-4193-833e-cdb168d27bd8"",
            ""actions"": [
                {
                    ""name"": ""Walk"",
                    ""type"": ""Value"",
                    ""id"": ""ecba5bb7-a1a4-43db-88b8-91d939e999e0"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""3252bd40-d400-4a06-ba8a-3957c3080e42"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Roll"",
                    ""type"": ""Button"",
                    ""id"": ""4fb8ae0a-7a56-438d-a6b4-fcded00b978b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""796058d8-1b58-4032-b4da-3063f22a0e60"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Block"",
                    ""type"": ""Button"",
                    ""id"": ""d0427356-140d-46f8-b3c4-f22e15ac26db"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AttackMouse"",
                    ""type"": ""Button"",
                    ""id"": ""fc1fef26-e047-40bd-9516-1c1c7fb07ee2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MouseBlock"",
                    ""type"": ""Button"",
                    ""id"": ""4ca142c5-8802-4375-957b-a5130f16c25d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5487533b-9d71-4775-a636-86a9a571a84d"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""741044e8-aeca-4287-9857-1759af46d28d"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""23d888ce-7a5b-47bc-9365-b66b034ef4c7"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Roll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c622a934-09ae-46d4-93fd-e7146e76d2a8"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""738888d0-b65d-4e0e-8c3d-b4513d6ac6b5"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Block"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f2f9bcfc-e9e7-4b9e-a5ea-7cf3e764b308"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AttackMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""207c1083-5dbc-4490-9297-8293d2b73d7c"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseBlock"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Mouse"",
            ""id"": ""662ad12c-bc3b-4b9f-b4f3-b7317f4b5edb"",
            ""actions"": [
                {
                    ""name"": ""mouse"",
                    ""type"": ""Value"",
                    ""id"": ""e359b845-f054-47ae-9cc0-9a0298ed088a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7de711ea-dcc4-4104-9890-6616060a52bb"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Walk = m_Gameplay.FindAction("Walk", throwIfNotFound: true);
        m_Gameplay_Jump = m_Gameplay.FindAction("Jump", throwIfNotFound: true);
        m_Gameplay_Roll = m_Gameplay.FindAction("Roll", throwIfNotFound: true);
        m_Gameplay_Attack = m_Gameplay.FindAction("Attack", throwIfNotFound: true);
        m_Gameplay_Block = m_Gameplay.FindAction("Block", throwIfNotFound: true);
        m_Gameplay_AttackMouse = m_Gameplay.FindAction("AttackMouse", throwIfNotFound: true);
        m_Gameplay_MouseBlock = m_Gameplay.FindAction("MouseBlock", throwIfNotFound: true);
        // Mouse
        m_Mouse = asset.FindActionMap("Mouse", throwIfNotFound: true);
        m_Mouse_mouse = m_Mouse.FindAction("mouse", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Walk;
    private readonly InputAction m_Gameplay_Jump;
    private readonly InputAction m_Gameplay_Roll;
    private readonly InputAction m_Gameplay_Attack;
    private readonly InputAction m_Gameplay_Block;
    private readonly InputAction m_Gameplay_AttackMouse;
    private readonly InputAction m_Gameplay_MouseBlock;
    public struct GameplayActions
    {
        private @PlayerControls m_Wrapper;
        public GameplayActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Walk => m_Wrapper.m_Gameplay_Walk;
        public InputAction @Jump => m_Wrapper.m_Gameplay_Jump;
        public InputAction @Roll => m_Wrapper.m_Gameplay_Roll;
        public InputAction @Attack => m_Wrapper.m_Gameplay_Attack;
        public InputAction @Block => m_Wrapper.m_Gameplay_Block;
        public InputAction @AttackMouse => m_Wrapper.m_Gameplay_AttackMouse;
        public InputAction @MouseBlock => m_Wrapper.m_Gameplay_MouseBlock;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Walk.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnWalk;
                @Walk.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnWalk;
                @Walk.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnWalk;
                @Jump.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Roll.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRoll;
                @Roll.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRoll;
                @Roll.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRoll;
                @Attack.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                @Block.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBlock;
                @Block.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBlock;
                @Block.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBlock;
                @AttackMouse.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttackMouse;
                @AttackMouse.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttackMouse;
                @AttackMouse.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttackMouse;
                @MouseBlock.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseBlock;
                @MouseBlock.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseBlock;
                @MouseBlock.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMouseBlock;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Walk.started += instance.OnWalk;
                @Walk.performed += instance.OnWalk;
                @Walk.canceled += instance.OnWalk;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Roll.started += instance.OnRoll;
                @Roll.performed += instance.OnRoll;
                @Roll.canceled += instance.OnRoll;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Block.started += instance.OnBlock;
                @Block.performed += instance.OnBlock;
                @Block.canceled += instance.OnBlock;
                @AttackMouse.started += instance.OnAttackMouse;
                @AttackMouse.performed += instance.OnAttackMouse;
                @AttackMouse.canceled += instance.OnAttackMouse;
                @MouseBlock.started += instance.OnMouseBlock;
                @MouseBlock.performed += instance.OnMouseBlock;
                @MouseBlock.canceled += instance.OnMouseBlock;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // Mouse
    private readonly InputActionMap m_Mouse;
    private IMouseActions m_MouseActionsCallbackInterface;
    private readonly InputAction m_Mouse_mouse;
    public struct MouseActions
    {
        private @PlayerControls m_Wrapper;
        public MouseActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @mouse => m_Wrapper.m_Mouse_mouse;
        public InputActionMap Get() { return m_Wrapper.m_Mouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseActions set) { return set.Get(); }
        public void SetCallbacks(IMouseActions instance)
        {
            if (m_Wrapper.m_MouseActionsCallbackInterface != null)
            {
                @mouse.started -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouse;
                @mouse.performed -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouse;
                @mouse.canceled -= m_Wrapper.m_MouseActionsCallbackInterface.OnMouse;
            }
            m_Wrapper.m_MouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @mouse.started += instance.OnMouse;
                @mouse.performed += instance.OnMouse;
                @mouse.canceled += instance.OnMouse;
            }
        }
    }
    public MouseActions @Mouse => new MouseActions(this);
    public interface IGameplayActions
    {
        void OnWalk(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnRoll(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnBlock(InputAction.CallbackContext context);
        void OnAttackMouse(InputAction.CallbackContext context);
        void OnMouseBlock(InputAction.CallbackContext context);
    }
    public interface IMouseActions
    {
        void OnMouse(InputAction.CallbackContext context);
    }
}
