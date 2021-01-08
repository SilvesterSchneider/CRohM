using ModelLayer;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    public interface IUserLoginService : IUserLoginRepository
    {

    }

    /// <summary>
    /// RAM: 100%
    /// </summary>
    public class UserLoginService : UserLoginRepository, IUserLoginService
    {
        public UserLoginService(CrmContext context) : base(context)
        {

        }
    }
}
