namespace DigitalLearningSolutions.Data.Extensions
{
    using System.Data;

    public static class ConnectionExtensions
    {
        public static void EnsureOpen(this IDbConnection connection)
        {
            if (connection.State.HasFlag(ConnectionState.Open))
            {
                return;
            }

            connection.Close();
            connection.Open();
        }
    }
}
