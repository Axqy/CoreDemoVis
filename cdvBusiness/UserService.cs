﻿using System;
using System.Collections.Generic;
using CfoDAL.DataBase;
using CfoDAL.DataEntity;
using CfoMiddleware;
using SqlSugar;

namespace CfoMiddleware
{

    public class UserService : DbContext<CoreUser>, IUserService
    {
        private SqlSugarClient dbContext;
        public UserService()
        {
            dbContext = new SqlSugarFactory().Context;
        }

        public List<CoreUser> GetUsers()
        {
            return dbContext.Queryable<CoreUser>().ToList();
        }

        public CoreUser GetUser(int Id)
        {
            return dbContext.Queryable<CoreUser>().First(x => x.ID == Id);
        }


        public string GetAutofacString()
        {
            return "hello world";
        }

        public string AutofacName { get { return "你zui了"; } }
    }
}
