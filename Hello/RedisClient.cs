using StackExchange.Redis;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

class RedisClient
{
    private static IDatabase db { get; set; }
    private static IServer server { get; set; }

    public static void Connect()
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379");
        server = redis.GetServer("localhost:6379");
        db = redis.GetDatabase(0);
    }

    public async static Task<RoomInfo[]> GetRoomInfos()
    {
        var result = new List<RoomInfo>();
        foreach (var key in server.Keys(pattern: "*"))
        {
            var value = await db.StringGetAsync(key);
            var info = JsonConvert.DeserializeObject<RoomInfo>(value);
            if (info.IsPublic)
            {
                result.Add(info);
            }
        }

        return result.ToArray();
    }

    public async static Task CreateRoom(string id, string name, string ownerName, bool isPublic)
    {
        var roomInfo = new RoomInfo(id, name, ownerName, isPublic);
        var json = JsonConvert.SerializeObject(roomInfo);
        await db.StringSetAsync(id, json);
        await db.KeyExpireAsync(id, new TimeSpan(1, 0, 0, 0));
    }
}
