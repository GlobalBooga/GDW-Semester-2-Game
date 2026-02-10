using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupable
{
    public void Pickup(Transform parentTo, Weapon weapon);

    public void Drop(Vector2 forwards);
}
