namespace wyvern.api
{
    public interface IHeaderFilter
    {
        RequestHeader TransformClientRequest(RequestHeader request);
        RequestHeader TransformServerRequest(RequestHeader request);
    }
}