// GENERATED AUTOMATICALLY FROM 'Assets/InputSystem/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""e01e560e-c6e4-41f1-8677-54aac614fcad"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""18667738-5345-493f-abda-69e229d4da86"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Camera"",
                    ""type"": ""Value"",
                    ""id"": ""2bf0c81d-686c-44ec-b203-8c9fac5b0880"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""6887bca2-6cb1-433d-bf96-384b54faed08"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Lean"",
                    ""type"": ""Button"",
                    ""id"": ""0c000385-e37c-4bd9-be03-b72bf8dda505"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""196d3ead-a68f-4cde-825b-801ea9e3b0a1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UseItem"",
                    ""type"": ""Button"",
                    ""id"": ""4e419134-53f2-444e-ad38-fd2f4b5e4a91"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EquipMedkit"",
                    ""type"": ""Button"",
                    ""id"": ""fd9cc3f5-fa14-40ea-b08a-cf62f346f0e9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EquipSmokeBomb"",
                    ""type"": ""Button"",
                    ""id"": ""636f8fbc-2ec1-4ade-9dd7-414b78b255e2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EquipStunBomb"",
                    ""type"": ""Button"",
                    ""id"": ""a9f4ee21-aa80-447c-8f7f-f35fc7fd78f4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EquipDecoy"",
                    ""type"": ""Button"",
                    ""id"": ""f00233a5-6803-4c4b-9474-e025d7bcf6d8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePos"",
                    ""type"": ""Value"",
                    ""id"": ""09c6254a-ccb6-4a25-88b3-f592355e6b05"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""d1981a97-20ae-42bf-8de0-4d20ac0fa498"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""f1e1d1a0-6325-4ef6-a095-e834e83e20e0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Breakout"",
                    ""type"": ""Button"",
                    ""id"": ""9d03a6c6-1c72-4651-aa7d-27dda11a3ef9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CancelThrow"",
                    ""type"": ""Button"",
                    ""id"": ""4129d5a7-8791-4e3b-87f9-c6bc528fe7e4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraStick"",
                    ""type"": ""Value"",
                    ""id"": ""b69375dc-772b-48d1-9268-58a774e42e4a"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ObjectiveShow"",
                    ""type"": ""Button"",
                    ""id"": ""adeba1dc-4a94-4154-ba34-87845dec57b1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""7d798053-8888-4c4f-bc10-42661a3956cd"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""0ffff21e-8589-4b76-88fc-93758a0c903c"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ada1b43a-7b41-4715-b804-72a1f4ad0195"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""86f9c65e-fb1f-414d-bb97-70a1d86cb023"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""f94f3dd0-2b2e-4e1a-be82-ebbbfe69f827"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""69cb283f-2dd0-43b9-ac28-69054f1cbb0d"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""78ef13ae-b954-4b0c-9dee-95f48e5cb546"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": ""InvertVector2(invertX=false)"",
                    ""groups"": ""KBM"",
                    ""action"": ""Camera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""132ef6d7-1ed7-4144-8404-ca19b46ba759"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""89753d1b-9ceb-43d0-a9dd-acd4b8a2f621"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6859490d-47e3-488d-a121-3e54585a040b"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Lean"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""df4bdcaf-b940-45f1-a8d9-16085e5d5fe0"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Lean"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3f426d24-cb37-4a10-b968-f870cd80de46"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0f4590ae-d333-4b5b-86fe-b6bc84c5ea98"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""49f09e76-b7e5-40f1-9264-c52c87835e44"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""UseItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c9c18c05-2edf-4e63-8813-eb817c7e736b"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""UseItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cbebc998-c70d-4459-a8ee-4614de650e42"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""EquipMedkit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""76b2b806-3284-4283-b01d-5dd75aaa977c"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""EquipMedkit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0c12ca04-0e27-46d1-9cb3-714da5a88493"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""EquipSmokeBomb"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3c3eeca0-3057-42ce-a10b-14cbb8d614cc"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""EquipSmokeBomb"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""91459123-8673-4147-a88c-0e0595ccd1d3"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""MousePos"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4afdd97b-7826-4593-b4c7-d06ea5bdba40"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9a5ac9d4-6cf8-4677-bc2d-0b3f03a3d4a3"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1470df3e-6b68-49c5-bc30-cf672ae34b41"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""EquipStunBomb"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3bfcffcc-c64e-4aa9-901b-dcff92eab8ef"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""EquipStunBomb"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d111476f-a302-4e81-8b69-8cbe82b7dddd"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a3f6c44c-d818-41b2-8211-b9147e5a4bc4"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e3fd15c5-6d50-4d69-b5f8-a89c4462a696"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""EquipDecoy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6170e50f-c324-40c9-9b84-4d43150ea4f4"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""EquipDecoy"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2dbb8c72-0d69-48f6-a66f-1d48ef080cdc"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Breakout"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f29031a5-ce2a-4064-95fc-c9810fb51f02"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""Breakout"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""80d57277-dd0b-4059-97ef-30010a4345b5"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""CancelThrow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""69d29d8a-f0cc-40d1-b02a-0d96f6d654ee"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""CancelThrow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e1cad96c-6eeb-43ba-ae61-51d63196b3a3"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""InvertVector2(invertX=false),ScaleVector2(x=12,y=12)"",
                    ""groups"": ""Controller"",
                    ""action"": ""CameraStick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""86801fa4-bb0b-45d9-95f4-b05c103570d5"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""ObjectiveShow"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menu"",
            ""id"": ""7accd529-7480-4c2a-9453-e6bbe357ef67"",
            ""actions"": [
                {
                    ""name"": ""Mouseclick"",
                    ""type"": ""Button"",
                    ""id"": ""faef17b0-b766-436d-8da7-b5253e2f7673"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""8554b78f-bdec-402d-a8ae-5732acccd37d"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Mouseclick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KBM"",
            ""bindingGroup"": ""KBM"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Controller"",
            ""bindingGroup"": ""Controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        m_Gameplay_Camera = m_Gameplay.FindAction("Camera", throwIfNotFound: true);
        m_Gameplay_Crouch = m_Gameplay.FindAction("Crouch", throwIfNotFound: true);
        m_Gameplay_Lean = m_Gameplay.FindAction("Lean", throwIfNotFound: true);
        m_Gameplay_Interact = m_Gameplay.FindAction("Interact", throwIfNotFound: true);
        m_Gameplay_UseItem = m_Gameplay.FindAction("UseItem", throwIfNotFound: true);
        m_Gameplay_EquipMedkit = m_Gameplay.FindAction("EquipMedkit", throwIfNotFound: true);
        m_Gameplay_EquipSmokeBomb = m_Gameplay.FindAction("EquipSmokeBomb", throwIfNotFound: true);
        m_Gameplay_EquipStunBomb = m_Gameplay.FindAction("EquipStunBomb", throwIfNotFound: true);
        m_Gameplay_EquipDecoy = m_Gameplay.FindAction("EquipDecoy", throwIfNotFound: true);
        m_Gameplay_MousePos = m_Gameplay.FindAction("MousePos", throwIfNotFound: true);
        m_Gameplay_Pause = m_Gameplay.FindAction("Pause", throwIfNotFound: true);
        m_Gameplay_Sprint = m_Gameplay.FindAction("Sprint", throwIfNotFound: true);
        m_Gameplay_Breakout = m_Gameplay.FindAction("Breakout", throwIfNotFound: true);
        m_Gameplay_CancelThrow = m_Gameplay.FindAction("CancelThrow", throwIfNotFound: true);
        m_Gameplay_CameraStick = m_Gameplay.FindAction("CameraStick", throwIfNotFound: true);
        m_Gameplay_ObjectiveShow = m_Gameplay.FindAction("ObjectiveShow", throwIfNotFound: true);
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_Mouseclick = m_Menu.FindAction("Mouseclick", throwIfNotFound: true);
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
    private readonly InputAction m_Gameplay_Move;
    private readonly InputAction m_Gameplay_Camera;
    private readonly InputAction m_Gameplay_Crouch;
    private readonly InputAction m_Gameplay_Lean;
    private readonly InputAction m_Gameplay_Interact;
    private readonly InputAction m_Gameplay_UseItem;
    private readonly InputAction m_Gameplay_EquipMedkit;
    private readonly InputAction m_Gameplay_EquipSmokeBomb;
    private readonly InputAction m_Gameplay_EquipStunBomb;
    private readonly InputAction m_Gameplay_EquipDecoy;
    private readonly InputAction m_Gameplay_MousePos;
    private readonly InputAction m_Gameplay_Pause;
    private readonly InputAction m_Gameplay_Sprint;
    private readonly InputAction m_Gameplay_Breakout;
    private readonly InputAction m_Gameplay_CancelThrow;
    private readonly InputAction m_Gameplay_CameraStick;
    private readonly InputAction m_Gameplay_ObjectiveShow;
    public struct GameplayActions
    {
        private @Controls m_Wrapper;
        public GameplayActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @Camera => m_Wrapper.m_Gameplay_Camera;
        public InputAction @Crouch => m_Wrapper.m_Gameplay_Crouch;
        public InputAction @Lean => m_Wrapper.m_Gameplay_Lean;
        public InputAction @Interact => m_Wrapper.m_Gameplay_Interact;
        public InputAction @UseItem => m_Wrapper.m_Gameplay_UseItem;
        public InputAction @EquipMedkit => m_Wrapper.m_Gameplay_EquipMedkit;
        public InputAction @EquipSmokeBomb => m_Wrapper.m_Gameplay_EquipSmokeBomb;
        public InputAction @EquipStunBomb => m_Wrapper.m_Gameplay_EquipStunBomb;
        public InputAction @EquipDecoy => m_Wrapper.m_Gameplay_EquipDecoy;
        public InputAction @MousePos => m_Wrapper.m_Gameplay_MousePos;
        public InputAction @Pause => m_Wrapper.m_Gameplay_Pause;
        public InputAction @Sprint => m_Wrapper.m_Gameplay_Sprint;
        public InputAction @Breakout => m_Wrapper.m_Gameplay_Breakout;
        public InputAction @CancelThrow => m_Wrapper.m_Gameplay_CancelThrow;
        public InputAction @CameraStick => m_Wrapper.m_Gameplay_CameraStick;
        public InputAction @ObjectiveShow => m_Wrapper.m_Gameplay_ObjectiveShow;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Camera.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera;
                @Camera.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera;
                @Camera.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCamera;
                @Crouch.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCrouch;
                @Lean.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLean;
                @Lean.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLean;
                @Lean.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLean;
                @Interact.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnInteract;
                @UseItem.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUseItem;
                @UseItem.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUseItem;
                @UseItem.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnUseItem;
                @EquipMedkit.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEquipMedkit;
                @EquipMedkit.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEquipMedkit;
                @EquipMedkit.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEquipMedkit;
                @EquipSmokeBomb.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEquipSmokeBomb;
                @EquipSmokeBomb.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEquipSmokeBomb;
                @EquipSmokeBomb.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEquipSmokeBomb;
                @EquipStunBomb.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEquipStunBomb;
                @EquipStunBomb.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEquipStunBomb;
                @EquipStunBomb.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEquipStunBomb;
                @EquipDecoy.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEquipDecoy;
                @EquipDecoy.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEquipDecoy;
                @EquipDecoy.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnEquipDecoy;
                @MousePos.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMousePos;
                @MousePos.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMousePos;
                @MousePos.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMousePos;
                @Pause.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPause;
                @Sprint.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSprint;
                @Breakout.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBreakout;
                @Breakout.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBreakout;
                @Breakout.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBreakout;
                @CancelThrow.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCancelThrow;
                @CancelThrow.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCancelThrow;
                @CancelThrow.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCancelThrow;
                @CameraStick.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCameraStick;
                @CameraStick.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCameraStick;
                @CameraStick.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnCameraStick;
                @ObjectiveShow.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnObjectiveShow;
                @ObjectiveShow.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnObjectiveShow;
                @ObjectiveShow.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnObjectiveShow;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Camera.started += instance.OnCamera;
                @Camera.performed += instance.OnCamera;
                @Camera.canceled += instance.OnCamera;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Lean.started += instance.OnLean;
                @Lean.performed += instance.OnLean;
                @Lean.canceled += instance.OnLean;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @UseItem.started += instance.OnUseItem;
                @UseItem.performed += instance.OnUseItem;
                @UseItem.canceled += instance.OnUseItem;
                @EquipMedkit.started += instance.OnEquipMedkit;
                @EquipMedkit.performed += instance.OnEquipMedkit;
                @EquipMedkit.canceled += instance.OnEquipMedkit;
                @EquipSmokeBomb.started += instance.OnEquipSmokeBomb;
                @EquipSmokeBomb.performed += instance.OnEquipSmokeBomb;
                @EquipSmokeBomb.canceled += instance.OnEquipSmokeBomb;
                @EquipStunBomb.started += instance.OnEquipStunBomb;
                @EquipStunBomb.performed += instance.OnEquipStunBomb;
                @EquipStunBomb.canceled += instance.OnEquipStunBomb;
                @EquipDecoy.started += instance.OnEquipDecoy;
                @EquipDecoy.performed += instance.OnEquipDecoy;
                @EquipDecoy.canceled += instance.OnEquipDecoy;
                @MousePos.started += instance.OnMousePos;
                @MousePos.performed += instance.OnMousePos;
                @MousePos.canceled += instance.OnMousePos;
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @Breakout.started += instance.OnBreakout;
                @Breakout.performed += instance.OnBreakout;
                @Breakout.canceled += instance.OnBreakout;
                @CancelThrow.started += instance.OnCancelThrow;
                @CancelThrow.performed += instance.OnCancelThrow;
                @CancelThrow.canceled += instance.OnCancelThrow;
                @CameraStick.started += instance.OnCameraStick;
                @CameraStick.performed += instance.OnCameraStick;
                @CameraStick.canceled += instance.OnCameraStick;
                @ObjectiveShow.started += instance.OnObjectiveShow;
                @ObjectiveShow.performed += instance.OnObjectiveShow;
                @ObjectiveShow.canceled += instance.OnObjectiveShow;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_Mouseclick;
    public struct MenuActions
    {
        private @Controls m_Wrapper;
        public MenuActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Mouseclick => m_Wrapper.m_Menu_Mouseclick;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @Mouseclick.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnMouseclick;
                @Mouseclick.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnMouseclick;
                @Mouseclick.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnMouseclick;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Mouseclick.started += instance.OnMouseclick;
                @Mouseclick.performed += instance.OnMouseclick;
                @Mouseclick.canceled += instance.OnMouseclick;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);
    private int m_KBMSchemeIndex = -1;
    public InputControlScheme KBMScheme
    {
        get
        {
            if (m_KBMSchemeIndex == -1) m_KBMSchemeIndex = asset.FindControlSchemeIndex("KBM");
            return asset.controlSchemes[m_KBMSchemeIndex];
        }
    }
    private int m_ControllerSchemeIndex = -1;
    public InputControlScheme ControllerScheme
    {
        get
        {
            if (m_ControllerSchemeIndex == -1) m_ControllerSchemeIndex = asset.FindControlSchemeIndex("Controller");
            return asset.controlSchemes[m_ControllerSchemeIndex];
        }
    }
    public interface IGameplayActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnCamera(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnLean(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnUseItem(InputAction.CallbackContext context);
        void OnEquipMedkit(InputAction.CallbackContext context);
        void OnEquipSmokeBomb(InputAction.CallbackContext context);
        void OnEquipStunBomb(InputAction.CallbackContext context);
        void OnEquipDecoy(InputAction.CallbackContext context);
        void OnMousePos(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnBreakout(InputAction.CallbackContext context);
        void OnCancelThrow(InputAction.CallbackContext context);
        void OnCameraStick(InputAction.CallbackContext context);
        void OnObjectiveShow(InputAction.CallbackContext context);
    }
    public interface IMenuActions
    {
        void OnMouseclick(InputAction.CallbackContext context);
    }
}
