namespace cCoder.Security.Brokers.Requests;
internal interface IHttpRequestBroker
	{
    bool HasHeader(string headerValue);
    string Header(string key);
}

