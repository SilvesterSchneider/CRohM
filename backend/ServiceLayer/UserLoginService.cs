using ModelLayer;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer
{
    public interface IUserLoginService : IUserLoginRepository
    {

    }

    public class UserLoginService : UserLoginRepository, IUserLoginService
    {
        public UserLoginService(CrmContext context) : base(context)
        {

        }
    }
}
