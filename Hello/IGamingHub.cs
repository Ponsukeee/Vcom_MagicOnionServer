using System.Threading.Tasks;
using UnityEngine;

namespace MagicOnion.Shared
{
    public interface IGamingHub : IStreamingHub<IGamingHub, IGamingHubReceiver>
    {
        Task<PlayerInfo[]> JoinAsync(string roomName, string userName, Player player, byte[] avatarData);
        Task LeaveAsync();
        Task MoveAsync(Vector3 bodyPosition, Quaternion bodyRotation, Vector3 headPosition, Quaternion headRotation, Vector3 rightHandPosition, Quaternion rightHandRotation,
            Vector3 leftHandPosition, Quaternion leftHandRotation);
        Task SpeakAsync(int index, float[] segment);
    }
}
