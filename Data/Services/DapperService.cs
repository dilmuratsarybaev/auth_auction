using Dapper;
using Example.Data.Models;
using System.Data;

namespace Example.Data.Services
{
    public class DapperService
    {
        private readonly IDbConnection _dbConnection;

        public DapperService (IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task InitializeDatabase()
        {
          await  _dbConnection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS auth (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL,
                Password TEXT NOT NULL,
                Role TEXT NOT NULL
            )");

        }

        public async Task<string> RegisterHandle(UserSession userSession)
        {

            try
            {
                await _dbConnection.ExecuteAsync("INSERT INTO auth (Name, Email, Password, Role) VALUES (@Name, @Email, @Password, @Role)", userSession);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
