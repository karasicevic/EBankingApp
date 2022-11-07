using EBankingApp.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBankingApp.DataAccessLayer
{
    public class SqlRepository
    {
        public const string CONNECTION_STRING= "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=User;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";


        public static async Task<User> CreateUser(User user)
         {
             var connection = new SqlConnection(CONNECTION_STRING);
             await connection.OpenAsync();
             var transaction = (SqlTransaction) await connection.BeginTransactionAsync();
             try
             {
                 var command = connection.CreateCommand();
                 command.Transaction = transaction;
                 command.CommandText = "insert into [dbo].[User](FirstName, LastName, Email, Password) output inserted.Id values (@firstname, @lastname, @email, @password)";
                 command.Parameters.AddWithValue("@firstname", user.FirstName);
                 command.Parameters.AddWithValue("@lastname", user.LastName);
                 command.Parameters.AddWithValue("@email", user.Email);
                 command.Parameters.AddWithValue("@password", user.Password);

                var id = (int?)(await command.ExecuteScalarAsync());

                if (id.HasValue == false)
                    throw new Exception("Error creating User.");

                 await transaction.CommitAsync();
                user.Id = id.Value;
                return user;
             }
             catch
             {
                 await transaction.RollbackAsync();
                 throw;
             }
             finally
             {
                await  connection.CloseAsync();
             }
         }
        public static async Task<List<User>> GetAllUsers()
        {
            List<User> users = new List<User>();
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            

                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "select * from [dbo].[User]";
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(GetUser(reader));
                }
            reader.CloseAsync();
            connection.CloseAsync();

            return users;
        }
        public static async Task<User> GetUserById(int id)
        {
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction) await connection.BeginTransactionAsync();
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "select * from [dbo].[User] where id=@id";
            command.Parameters.AddWithValue("@id", id);
            var reader = await command.ExecuteReaderAsync();

            User? user = null;
            if (await reader.ReadAsync())
            {
                user = GetUser(reader);
            }
            await reader.CloseAsync();
            await connection.CloseAsync();
            return user;
        }
        public static User GetUser(SqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32("id"),
                FirstName = reader.GetString("firstname"),
                LastName = reader.GetString("lastname"),
                Email = reader.GetString("email"),
                Password = reader.GetString("password")
            };
        }
        public static async Task<User> DeleteUser(int id)
        {
            var user =await GetUserById(id);
            if (user == null)
            {
                throw new Exception($"User with id: {id} does not exist!");
            }
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            
                command.CommandText = "delete from [dbo].[User] where id=@id";
                command.Parameters.AddWithValue("@id", id);
                var res=await command.ExecuteNonQueryAsync();

                if (res == 0)
                {
                    Console.WriteLine("Delete command failed!");
                }
   
                await connection.CloseAsync();           
            return user;
        }
        public static async Task<User> UpdateUser(User user, int id)

        {
            User userToUpdate =await GetUserById(id);
            if (userToUpdate == null)
            {
                Console.WriteLine("User with that id does not exist!");
                return null;
            }
            
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            try
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "update [dbo].[User] set FirstName=@firstname, LastName=@lastname, Email=@email," +
                    " Password=@password where id=@id";
                command.Parameters.AddWithValue("@firstname", user.FirstName);
                command.Parameters.AddWithValue("@lastname", user.LastName);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@id", id);
                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return user;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }


        public static async Task<Currency> CreateCurrency(Currency curr)
        {
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();           
            var transaction=(SqlTransaction) await connection.BeginTransactionAsync();
            try
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "insert into [dbo].[Currency] (Name,Code)  output inserted.Id values (@name,@code)";
                command.Parameters.AddWithValue("@name", curr.Name);
                command.Parameters.AddWithValue("@code", curr.Code);
                var id = (int?)(await command.ExecuteScalarAsync());

                if (id.HasValue == false)
                    throw new Exception("Error creating Currency.");
                
                await transaction.CommitAsync();
               curr.Id = id.Value;
                return curr;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        public static async Task<Currency> UpdateCurrency(int id, Currency curr)
        {
            Currency currToUpdate = await GetCurrencyById(id);
            if (currToUpdate == null)
            {
                throw new Exception($"Curreny with id: {id} does not exist!");
            }
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            try
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "update [dbo].[Currency] set Name=@name, Code=@code where id=@id;";
                command.Parameters.AddWithValue("@name", curr.Name);
                command.Parameters.AddWithValue("@code", curr.Code);
                command.Parameters.AddWithValue("@id", id);
                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                return await GetCurrencyById(id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw ;
            }
            finally
            {
                await connection.CloseAsync();
            }

        }
        public static async Task<Currency> DeleteCurrency(int id)
        {
            var curr = await GetCurrencyById(id);
            if (curr == null)
            {
                throw new Exception($"Currency with id: {id} does not exist!");
            }
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "delete from [dbo].[Currency] where id=@id";
            command.Parameters.AddWithValue("@id", id);
            var res = await command.ExecuteNonQueryAsync();

            if (res == 0)
            {
                Console.WriteLine("Delete command failed!");
            }

            await connection.CloseAsync();
            return curr;
        }
        public static async Task<List<Currency>> GetCurrencies()
        {
            List<Currency> currencies = new List<Currency>();
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();


            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "select * from [dbo].[Currency]";
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                currencies.Add(GetCurrency(reader));
            }
            reader.CloseAsync();
            connection.CloseAsync();

            return currencies;
        }
        public static async Task<Currency> GetCurrencyById(int id)
        {
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction) await connection.BeginTransactionAsync();
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "select * from [dbo].[Currency] where id=@id;";
            command.Parameters.AddWithValue("@id", id);
            var reader = await command.ExecuteReaderAsync();
            Currency? curr = null;
            if (await reader.ReadAsync())
            {
                curr = GetCurrency(reader);
            }
            await reader.CloseAsync();
            await connection.CloseAsync();
            return curr;
        }
        public static Currency GetCurrency(SqlDataReader reader)
        {
            return new Currency
            {
                Name = reader.GetString("Name"),
                Code = reader.GetString("Code")
            };
        }


        public static async Task<Account> CreateAccount(Account account)
        {
            if (GetUserById(account.UserId) == null)
            {
                throw new Exception($"User with id: {account.UserId} does not exist!");
            }
            if (GetCurrencyById(account.CurrencyId) == null)
            {
                throw new Exception($"Currency with id: {account.CurrencyId} does not exist!");
            }
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
           
            try
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "insert into [dbo].[Account] (Balance,Status, Number, UserId, CurrencyId)" +
                    "  output inserted.Id values (@balance,@status,@number,@userid,@currencyid)";
                command.Parameters.AddWithValue("@balance", account.Balance);
                command.Parameters.AddWithValue("@status", account.Status);
                command.Parameters.AddWithValue("@number", account.Number);
                command.Parameters.AddWithValue("@userid", account.UserId);
                command.Parameters.AddWithValue("@currencyid", account.CurrencyId);

                var id = (int?)(await command.ExecuteScalarAsync());

                if (id.HasValue == false)
                    throw new Exception("Error creating Currency.");

                await transaction.CommitAsync();
                account.Id = id.Value;
                return account;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }


        }
        public static async Task<Account> UpdateAccount(int id, Account account)
        {
            if (GetUserById(account.UserId) == null)
            {
                throw new Exception($"User with id: {account.UserId} does not exist!");
            }
            if (GetCurrencyById(account.CurrencyId) == null)
            {
                throw new Exception($"Currency with id: {account.CurrencyId} does not exist!");
            }
            Account acc = await GetAccountById(id);
            if (acc == null)
            {
                throw new Exception($"Account with id: {id} does not exist!");
            }
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            try
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "update [dbo].[Account] set Balance=@balance, Status=@status, Number=@number, UserId=userid," +
                    "CurrencyId=@currencyid where id=@id;";
                command.Parameters.AddWithValue("@balance", account.Balance);
                command.Parameters.AddWithValue("@status", account.Status);
                command.Parameters.AddWithValue("@number", account.Number);
                command.Parameters.AddWithValue("@userid", account.UserId);
                command.Parameters.AddWithValue("@currencyid", account.CurrencyId);
                return await GetAccountById(id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        public static async Task<Account> DeleteAccount(int id)
        {
            var account = await GetAccountById(id);
            if (account == null)
            {
                throw new Exception($"Account with id: {id} does not exist!");
            }
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "delete from [dbo].[Account] where id=@id";
            command.Parameters.AddWithValue("@id", id);
            var res = await command.ExecuteNonQueryAsync();

            if (res == 0)
            {
                Console.WriteLine("Delete command failed!");
            }

            await connection.CloseAsync();
            return account;

        }
        public static async Task<List<Account>> GetAllAccounts()
        {
            List<Account> accounts = new List<Account>();
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();


            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "select * from [dbo].[Account]";
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                accounts.Add(GetAccount(reader));
            }
            reader.CloseAsync();
            connection.CloseAsync();

            return accounts;
        }
        public static async Task<Account> GetAccountById(int id)
        {
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "select * from [dbo].[Account] where id=@id;";
            command.Parameters.AddWithValue("@id", id);
            var reader = await command.ExecuteReaderAsync();
            Account? acc = null;
            if (await reader.ReadAsync())
            {
                acc = GetAccount(reader);
            }
            await reader.CloseAsync();
            await connection.CloseAsync();
            return acc;
        }
        public static Account GetAccount(SqlDataReader reader)
        {
            return new Account
            {
                Id=reader.GetInt32("Id"),
                Balance = reader.GetDecimal("Balance"),
                Status = (Status)(int)reader["Status"],
                Number = reader.GetString("Number"),
                UserId=reader.GetInt32("UserId"),
                CurrencyId=reader.GetInt32("CurrencyId")

            };
        }



        public static async Task<Transaction> CreateTransaction(Transaction trans)
        {
            if(trans.FromAccountId==null && trans.ToAccountId == null)
            {
                throw new Exception("Error");
            }
            if (trans.FromAccountId != null)
            {
                if (GetAccountById(trans.FromAccountId.Value) == null)
                {
                    throw new Exception($"Account with id: {trans.FromAccountId} does not exist!");
                }
            }
            if (trans.ToAccountId != null)
            {
                if (GetAccountById(trans.ToAccountId.Value) == null)
                {
                    throw new Exception($"Account with id: {trans.ToAccountId} does not exist!");
                }
            }
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                if (trans.FromAccountId == null)
                {
                    command.CommandText = "insert into [dbo].[Transaction] (Amount, Date, FromAccountId, ToAccountId)" +
                        "  output inserted.Id values (@amount,@date,@from,@to)";
                } else if(trans.ToAccountId==null){
                    command.CommandText = "insert into [dbo].[Transaction] (Amount, Date, FromAccountId, ToAccountId)" +
                        "  output inserted.Id values (@amount,@date,@from,@to)";
                }
                command.CommandText = "insert into [dbo].[Transaction] (Amount, Date, FromAccountId, ToAccountId)" +
                    "  output inserted.Id values (@amount,@date,@from,@to)";
                command.Parameters.AddWithValue("@amount", trans.Amount);
                command.Parameters.AddWithValue("@date", trans.DateOfRealisation);
                command.Parameters.AddWithValue("@from", trans.FromAccountId);
                command.Parameters.AddWithValue("@to", trans.ToAccountId);

                var id = (int?)(await command.ExecuteScalarAsync());

                if (id.HasValue == false)
                    throw new Exception("Error creating Transaction.");

                await transaction.CommitAsync();
                trans.Id = id.Value;
                return trans;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }


        }
        public static async Task<Transaction> UpdateTransaction(int id, Transaction trans)
        {
            if (trans.FromAccountId == null && trans.ToAccountId == null)
            {
                throw new Exception("Error");
            }
            if (trans.FromAccountId != null)
            {
                if (GetAccountById(trans.FromAccountId.Value) == null)
                {
                    throw new Exception($"Account with id: {trans.FromAccountId} does not exist!");
                }
            }
            if (trans.ToAccountId != null)
            {
                if (GetAccountById(trans.ToAccountId.Value) == null)
                {
                    throw new Exception($"Account with id: {trans.ToAccountId} does not exist!");
                }
            }
            Account acc = await GetAccountById(id);
            if (acc == null)
            {
                throw new Exception($"Account with id: {id} does not exist!");
            }
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            try
            {
                var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "update [dbo].[Transaction] set Amount=@amount, DateOfRealisation=@date, FromAccountId=@from, ToAccountId=@to," +
                    "CurrencyId=@currencyid where id=@id;";
                command.Parameters.AddWithValue("@amount", trans.Amount);
                command.Parameters.AddWithValue("@date", trans.DateOfRealisation);
                command.Parameters.AddWithValue("@from", trans.FromAccountId);
                command.Parameters.AddWithValue("@to", trans.ToAccountId);
                return await GetTransactionById(id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
        public static async Task<Account> DeleteTransaction(int id)
        {
            var trans = await GetAccountById(id);
            if (trans == null)
            {
                throw new Exception($"Transaction with id: {id} does not exist!");
            }
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "delete from [dbo].[Transaction] where id=@id";
            command.Parameters.AddWithValue("@id", id);
            var res = await command.ExecuteNonQueryAsync();

            if (res == 0)
            {
                Console.WriteLine("Delete command failed!");
            }

            await connection.CloseAsync();
            return trans;

        }
        public static async Task<List<Transaction>> GetAllTransacions()
        {
            List<Transaction> transactions = new List<Transaction>();
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();


            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "select * from [dbo].[Transaction]";
            var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                transactions.Add(GetTransaction(reader));
            }
            reader.CloseAsync();
            connection.CloseAsync();

            return transactions;
        }
        public static async Task<Transaction> GetTransactionById(int id)
        {
            var connection = new SqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();
            var transaction = (SqlTransaction)await connection.BeginTransactionAsync();
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "select * from [dbo].[Transaction] where id=@id;";
            command.Parameters.AddWithValue("@id", id);
            var reader = await command.ExecuteReaderAsync();
            Transaction? trans = null;
            if (await reader.ReadAsync())
            {
                trans = GetTransaction(reader);
            }
            await reader.CloseAsync();
            await connection.CloseAsync();
            return trans;
        }
        public static Transaction GetTransaction(SqlDataReader reader)
        {
            return new Transaction
            {
                Id = reader.GetInt32("Id"),
                Amount = reader.GetDecimal("Amount"),
                FromAccountId = reader.GetInt32("FromAccountId"),
                ToAccountId = reader.GetInt32("ToAccountId"),
                DateOfRealisation=reader.GetDateTime("DateOfRealisation")

            };
        }
    }
}
