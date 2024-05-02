namespace cCoder.Security.Data.Brokers.DateTime;

public interface ISecurityDateTimeOffsetBroker
{
    DateTimeOffset GetCurrentTime();
}