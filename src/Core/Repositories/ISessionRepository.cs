using Core.Repositories.Commands.SessionRepository;

namespace Core.Repositories
{
    public interface ISessionRepository
    {
        ISessionCommands Session { get; }
    }
}
