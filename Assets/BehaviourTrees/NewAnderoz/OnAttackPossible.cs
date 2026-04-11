using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/OnAttackPossible")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "OnAttackPossible", message: "Attack Possible", category: "Events", id: "58512354cce892f2bc5b662b2a187667")]
public sealed partial class OnAttackPossible : EventChannel { }

