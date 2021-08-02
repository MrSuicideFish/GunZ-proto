using System;
using UnityEngine;

public enum HitType
{
    Default,
    Heavy,
    Fall
}

[Serializable]
public struct HitInfo
{
    [SerializeField] public string hitPlayer;
    [SerializeField] public string hitByPlayer;
    [SerializeField] public int damage;
    [SerializeField] public int hitType;
    [SerializeField] public Vector3 hitDirection;
}