namespace Services.Comms.Sockets
{
    public enum SubCommand : byte
    {
        Unknown = 0x00,
        SessionLogIn,
        SessionLogOut,
        SessionActiveCompanyGet,
        SessionActiveCompanySet,
        UserGet,
        UserAdd,
        UserUpdate,
        UserDelete,
        UserChangePassword,
        UserGetCompanies,
        CompanyGet,
        CompanyAdd,
        CompanyUpdate,
        CompanyDelete,
        CompanyAssignUser,
        JobGet,
        JobAdd,
        JobUpdate,
        JobDelete,
        JobGetByCustomer,
        JobGetByCustomerRoute,
        JobGetByDate
    }
}
