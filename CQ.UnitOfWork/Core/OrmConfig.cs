using CQ.Utility;

namespace CQ.UnitOfWork
{
    public abstract record class OrmConfig
    {
        public readonly DatabaseConfig DatabaseConnection;

        public readonly bool UseDefaultQueryLogger;

        public readonly bool Default;

        public OrmConfig(
            DatabaseConfig databaseConnection,
            bool useDefaultQueryLogger = false,
            bool @default = true)
        {
            Guard.ThrowIsNull(databaseConnection, nameof(databaseConnection));
            this.DatabaseConnection = databaseConnection;
            this.UseDefaultQueryLogger = useDefaultQueryLogger;
            this.Default = @default;
        }
    }
}
