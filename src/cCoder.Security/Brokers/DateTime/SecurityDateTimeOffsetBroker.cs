namespace cCoder.Security.Brokers.DateTime;
internal class SecurityDateTimeOffsetBroker : ISecurityDateTimeOffsetBroker
{
    public DateTimeOffset GetCurrentTime()
        => DateTimeOffset.Now;
}


