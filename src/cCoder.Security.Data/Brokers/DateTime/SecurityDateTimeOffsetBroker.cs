namespace cCoder.Security.Data.Brokers.DateTime;

public class SecurityDateTimeOffsetBroker : ISecurityDateTimeOffsetBroker
{
    public DateTimeOffset GetCurrentTime()
        => DateTimeOffset.Now;
}
