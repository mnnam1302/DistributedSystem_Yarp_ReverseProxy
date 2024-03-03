namespace Query.Domain.Abstractions.Options
{
    public interface IMongoDbSettings
    {
        string ConnectionString { get; set; }

        string DatabaseName { get; set; }
    }
}