using System;
using System.Collections.Generic;
using System.Text;

namespace MagicOnion.Shared
{
    public interface IGamingHubReceiver
    {
        void OnJoin(Player player, byte[] avatarData);
        void OnLeave(Player player);
        void OnMove(Player player);
    }
}
