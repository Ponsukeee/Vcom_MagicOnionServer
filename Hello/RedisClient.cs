using StackExchange.Redis;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using System.Linq;

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
        var keys = server.Keys(pattern: "*");
        var result = new RoomInfo[keys.Count()];
        int i = 0;
        foreach (var key in keys)
        {
            var value = await db.StringGetAsync(key);
            result[i] = JsonConvert.DeserializeObject<RoomInfo>(value);
            i++;
        }

        return result;
    }

    public async static Task InsertRoomInfo(string id, string name, string ownerName, bool isPublic)
    {
        var roomInfo = new RoomInfo(id, name, ownerName, isPublic);
        var json = JsonConvert.SerializeObject(roomInfo);
        await db.StringSetAsync(id, json);
        await db.KeyExpireAsync(id, new TimeSpan(1, 0, 0, 0));
    }

    public async static Task DeleteRoomInfo(string id)
    {
        await db.KeyDeleteAsync(id);
    }
}
