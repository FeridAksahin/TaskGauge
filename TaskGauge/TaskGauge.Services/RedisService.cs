using StackExchange.Redis;

namespace TaskGauge.Services
{

    public class RedisService
    {

        private ConnectionMultiplexer _connectionMultiplexer;

        public RedisService(string url)
        {
           _connectionMultiplexer = ConnectionMultiplexer.Connect(url);
        }

        public IDatabase Connect(int requestDbNumber)
        {
            return _connectionMultiplexer.GetDatabase(requestDbNumber);
        } 
    }
}
