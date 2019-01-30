using MagicOnion.Server.Hubs;
using MagicOnion.Shared;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Hello
{
    public class GamingHub : StreamingHubBase<IGamingHub, IGamingHubReceiver>, IGamingHub
    {
        IGroup room;
        Player self;
        PlayerInfo selfInfo;
        IInMemoryStorage<PlayerInfo> storage;

        public async Task<PlayerInfo[]> JoinAsync(string roomName, string userName, Player player, byte[] avatarData)
        {
            self = player;
            selfInfo = new PlayerInfo(){ Player = player, AvatarData = avatarData };
            (room, storage) = await this.Group.AddAsync(roomName, selfInfo);
            BroadcastExceptSelf(room).OnJoin(self, avatarData);

            return storage.AllValues.ToArray();
        }

        public async Task LeaveAsync()
        {
            await room.RemoveAsync(this.Context);
            Broadcast(room).OnLeave(self);
        }

        public async Task MoveAsync(Vector3 bodyPosition, Quaternion bodyRotation, Vector3 headPosition, Quaternion headRotation, Vector3 rightHandPosition, Quaternion rightHandRotation, Vector3 leftHandPosition, Quaternion leftHandRotation)
        {
            self.BodyPosition = bodyPosition;
            self.BodyRotation = bodyRotation;
            self.HeadPosition = headPosition;
            self.HeadRotation = headRotation;
            self.RightHandPosition = rightHandPosition;
            self.RightHandRotation = rightHandRotation;
            self.LeftHandPosition = leftHandPosition;
            self.LeftHandRotation = leftHandRotation;
            BroadcastExceptSelf(room).OnMove(self);
        }

        protected override ValueTask OnDisconnected()
        {
            //nop
            return CompletedTask;
        }
    }
}
