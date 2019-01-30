using MessagePack;

namespace MagicOnion.Shared
{
    [MessagePackObject]
    public class PlayerInfo
    {
        [Key(0)]
        public Player Player;
        [Key(1)]
        public byte[] AvatarData;
    }
}
