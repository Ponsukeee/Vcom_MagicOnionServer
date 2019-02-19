using MagicOnion.Server.Hubs;
using System.Linq;
using System.Threading.Tasks;

namespace Hello
{
    public class GamingHub : StreamingHubBase<IGamingHub, IGamingHubReceiver>, IGamingHub
    {
        private IGroup room;
        private IInMemoryStorage<PlayerInfo> storage;
        private int selfID;
        private string selfName;
        private AvatarTransform selfTransform;
        private PlayerInfo selfInfo;

        public async Task<int> JoinAsync(string roomID, string userName)
        {
            (room, storage) = await this.Group.AddAsync(roomID, selfInfo);
            selfID = RandomNumbers.NonDuplicateNumber();
            selfName = userName;
            var playerCount = await room.GetMemberCountAsync();
            BroadcastExceptSelf(room).OnJoin(selfID, userName, playerCount);

            return selfID;
        }

        public async Task LeaveAsync()
        {
            await room.RemoveAsync(this.Context);
            var playerCount = await room.GetMemberCountAsync();
            Broadcast(room).OnLeave(selfID, selfName, playerCount);
        }

        public async Task<PlayerInfo[]> GenerateAvatarAsync(AvatarTransform transform, byte[] avatarData)
        {
            selfTransform = transform;
            selfInfo = new PlayerInfo() { ID = selfID, Name = selfName, Transform = selfTransform, AvatarData = avatarData };
            BroadcastExceptSelf(room).OnGenerateAvatar(selfID, transform, avatarData);

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
            selfTransform = transform;
            BroadcastExceptSelf(room).OnSynchronizeAvatar(selfID, selfTransform);
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
