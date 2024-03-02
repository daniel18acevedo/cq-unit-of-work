using CQ.Utility;

namespace CQ.UnitOfWork
{
    public sealed record class DatabaseConfig
    {
        public readonly string ConnectionString;

        public readonly string Name;

        public DatabaseConfig(
            string connectionString,
            string name)
        {
            this.ConnectionString = connectionString;
            Guard.ThrowIsNullOrEmpty(connectionString, nameof(this.ConnectionString));
            
            this.Name = name;
            Guard.ThrowIsNullOrEmpty(this.Name, nameof(this.Name));
        }
    }
}
