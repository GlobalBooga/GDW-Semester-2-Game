using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemThing : MonoBehaviour
{
    EventSystem es;

    private void Start()
    {
        es = GetComponent<EventSystem>();
    }

    public void SetFirstSelected(GameObject firstSelected)
    {
        es.firstSelectedGameObject = firstSelected;
    }

}
