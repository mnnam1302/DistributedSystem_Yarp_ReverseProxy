namespace ApiGateway.DependecyInjection.Options
{
    public class RateLimitOptions
    {
        public short TokenLimit { get; set; }

        public short ReplenishmentPeriod { get; set; }

        public short TokensPerPeriod { get; set; }

        public short QueueLimit { get; set; }
    }
}
