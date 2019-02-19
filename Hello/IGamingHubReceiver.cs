public interface IGamingHubReceiver
{
    void OnJoin(int id, string playerName, int playerCount);
    void OnLeave(int id, string userName, int playerCount);
    void OnGenerateAvatar(int id, AvatarTransform player, byte[] avatarData);
    void OnInstantiate(int id, string resourceName);
    void OnDestroy(int id);
    void OnSynchronizeAvatar(int id, AvatarTransform transform);
    void OnMoveObject(int id, ObjectTransform transform);
    void OnSpeak(int index, float[] segment);
}
