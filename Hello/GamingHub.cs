using MagicOnion.Server.Hubs;
using System.Linq;
using System.Threading.Tasks;

namespace Hello
{
    public class GamingHub : StreamingHubBase<IGamingHub, IGamingHubReceiver>, IGamingHub
    {
        private IGroup room;
        private IInMemoryStorage<PlayerInfo> storage;
        private PlayerInfo selfInfo;
        private string roomID;

        public async Task<RoomInfo[]> GetRoomInfos()
        {
            return await RedisClient.GetRoomInfos();
        }

        public async Task<int> JoinAsync(string userName)
        {
            selfInfo = new PlayerInfo(RandomNumbers.NonDuplicateNumber(), userName);
            roomID = RandomNumbers.NonDuplicateNumber().ToString();
            (room, storage) = await this.Group.AddAsync(roomID, selfInfo);
            await RedisClient.CreateRoom(roomID, "test", userName, true);
            var playerCount = await room.GetMemberCountAsync();
            BroadcastExceptSelf(room).OnJoin(selfInfo.ID, userName, playerCount);

            return selfInfo.ID;
        }

        public async Task LeaveAsync()
        {
            await room.RemoveAsync(this.Context);
            var playerCount = await room.GetMemberCountAsync();
            Broadcast(room).OnLeave(selfInfo.ID, selfInfo.Name, playerCount);
        }

        public async Task<PlayerInfo[]> GenerateAvatarAsync(AvatarTransform transform, byte[] avatarData)
        {
            selfInfo.Transform = transform;
            selfInfo.AvatarData = avatarData;
            BroadcastExceptSelf(room).OnGenerateAvatar(selfInfo.ID, transform, avatarData);

            return storage.AllValues.ToArray();
        }

        public async Task<int> InstantiateAsync(string resourceName)
        {
            int id = RandomNumbers.NonDuplicateNumber();
            BroadcastExceptSelf(room).OnInstantiate(id, resourceName);
            return id;
        }

        public async Task DestroyAsync(int id)
        {
            BroadcastExceptSelf(room).OnDestroy(id);
        }

        public async Task SynchronizeAvatarAsync(AvatarTransform transform)
        {
            selfInfo.Transform = transform;
            BroadcastExceptSelf(room).OnSynchronizeAvatar(selfInfo.ID, selfInfo.Transform);
        }

        public async Task MoveObjectAsync(int id, ObjectTransform transform)
        {
            BroadcastExceptSelf(room).OnMoveObject(id, transform);
        }

        public async Task SpeakAsync(int index, float[] segment)
        {
            BroadcastExceptSelf(room).OnSpeak(index, segment);
        }

        protected override ValueTask OnDisconnected()
        {
            //nop
            return CompletedTask;
        }
    }
}
