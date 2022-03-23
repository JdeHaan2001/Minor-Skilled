using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] GameEvents.PlayerPlace place;

    public GameEvents.PlayerPlace Place { get => place; private set => place = value; }
}
