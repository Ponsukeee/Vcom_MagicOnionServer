using MagicOnion.Server.Hubs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hello
{
    public class GamingHub : StreamingHubBase<IGamingHub, IGamingHubReceiver>, IGamingHub
    {
        private IGroup room;
        private IInMemoryStorage<PlayerInfo> storage;
        private PlayerInfo selfInfo;
        private string myRoomID;
        private string currentRoomID;

        public async Task<RoomInfo[]> GetRoomInfos()
        {
            var result = new List<RoomInfo>();
            var roomInfos = await RedisClient.GetRoomInfos();
            foreach (var roomInfo in roomInfos)
            {
                if (roomInfo.RoomID != myRoomID && roomInfo.RoomID != currentRoomID && roomInfo.IsPublic)
                {
                    result.Add(roomInfo);
                }
            }

            return result.ToArray();
        }
        
        public async Task<int> JoinRoomAsync(string userName)
        {
            selfInfo = new PlayerInfo(RandomNumbers.NonDuplicateNumber(), userName);
            myRoomID = RandomNumbers.NonDuplicateNumber().ToString();

            currentRoomID = myRoomID;
            (room, storage) = await this.Group.AddAsync(currentRoomID, selfInfo);
            await RedisClient.InsertRoomInfo(myRoomID, "test", selfInfo.Name, true);
            var playerCount = await room.GetMemberCountAsync();
            BroadcastExceptSelf(room).OnJoinRoom(selfInfo.ID, selfInfo.Name, playerCount);

            return selfInfo.ID;
        }

        public async Task<int> JoinOtherRoomAsync(string userName, string roomID)
        {
            await room.RemoveAsync(this.Context);
            await RedisClient.DeleteRoomInfo(currentRoomID);

            currentRoomID = roomID;
            (room, storage) = await this.Group.AddAsync(currentRoomID, selfInfo);
            var playerCount = await room.GetMemberCountAsync();
            BroadcastExceptSelf(room).OnJoinRoom(selfInfo.ID, selfInfo.Name, playerCount);

            return selfInfo.ID;
        }

        public async Task LeaveRoomAsync()
        {
            await room.RemoveAsync(this.Context);
            if (myRoomID == currentRoomID)
            {
                await RedisClient.DeleteRoomInfo(myRoomID);
                Broadcast(room).OnDestroyRoom(selfInfo.ID);
            }
            else
            {
                var playerCount = await room.GetMemberCountAsync();
                Broadcast(room).OnLeaveRoom(selfInfo.ID, selfInfo.Name, playerCount);

                currentRoomID = myRoomID;
                (room, storage) = await this.Group.AddAsync(myRoomID, selfInfo);
                await RedisClient.InsertRoomInfo(myRoomID, "test", selfInfo.Name, true);
            }
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
            BroadcastExceptSelf(room).OnSpeak(selfInfo.ID, index, segment);
        }

        protected override ValueTask OnDisconnected()
        {
            LeaveRoomAsync();
            return CompletedTask;
        }
    }
}
