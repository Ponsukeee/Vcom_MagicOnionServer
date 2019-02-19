﻿using MessagePack;
using UnityEngine;

[MessagePackObject]
public struct ObjectTransform
{
    [Key(0)]
    public Vector3 Position { get; set; }
    [Key(1)]
    public Quaternion Rotation { get; set; }
}
