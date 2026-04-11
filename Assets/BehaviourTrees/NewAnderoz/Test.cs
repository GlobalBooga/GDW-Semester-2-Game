using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Test")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Test", message: "[test]", category: "Events", id: "6e3a30a3580a5e87900b7dc540b901a0")]
public sealed partial class Test : EventChannel { }

