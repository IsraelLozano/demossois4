using System;
using System.Data;

namespace dic.sso.identityserver.oauth.Repositories
{
    public abstract class Repository
    {
        protected readonly Func<IDbConnection> OpenConnection;

        protected Repository(Func<IDbConnection> openConnection)
        {
            OpenConnection = openConnection;
        }
    }
}