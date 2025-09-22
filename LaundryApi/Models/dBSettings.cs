namespace LaundryApi.Models
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;

        public string UsersCollectionName { get; set; } = null!;
    }

    public class PostgresSettings
    {
        public string Host { get; set; } = null!;

        public string Port { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}