using MessagePack;
using UnityEngine;

namespace MagicOnion.Shared
{
    [MessagePackObject]
    public class Player
    {
        [Key(0)]
        public string Name { get; set; }
        [Key(1)]
        public Vector3 BodyPosition { get; set; }
        [Key(2)]
        public Quaternion BodyRotation { get; set; }
        [Key(3)]
        public Vector3 HeadPosition { get; set; }
        [Key(4)]
        public Quaternion HeadRotation { get; set; }
        [Key(5)]
        public Vector3 RightHandPosition { get; set; }
        [Key(6)]
        public Quaternion RightHandRotation { get; set; }
        [Key(7)]
        public Vector3 LeftHandPosition { get; set; }
        [Key(8)]
        public Quaternion LeftHandRotation { get; set; }
    }
}
