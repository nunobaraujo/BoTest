namespace Core.Repositories
{
    public interface ICompanyRepositoryResolver
    {
        ICompanyRepository Resolve(string companyId);
    }
}
