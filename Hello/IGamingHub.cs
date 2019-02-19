﻿using MagicOnion;
using System.Threading.Tasks;

public interface IGamingHub : IStreamingHub<IGamingHub, IGamingHubReceiver>
{
    Task<int> JoinAsync(string roomID, string userName);
    Task LeaveAsync();
    Task<PlayerInfo[]> GenerateAvatarAsync(AvatarTransform transform, byte[] avatarData);
    Task<int> InstantiateAsync(string resourceName);
    Task DestroyAsync(int id);
    Task SynchronizeAvatarAsync(AvatarTransform transform);
    Task MoveObjectAsync(int id, ObjectTransform transform);
    Task SpeakAsync(int index, float[] segment);
}
