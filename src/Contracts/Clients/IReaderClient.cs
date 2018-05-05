namespace Contracts.Clients
{
    public interface IReaderClient
    {
        ISessionApi SessionApi { get; }
        IUserApi UserApi { get; }
    }
}
