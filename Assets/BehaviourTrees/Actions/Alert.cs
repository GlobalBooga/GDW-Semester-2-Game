using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Alert")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Alert", message: "Agent heard something [here]", category: "Events", id: "ecbd1b15b0b6b4f16b0fb29fd433394b")]
public sealed partial class Alert : EventChannel<Vector2> { }

