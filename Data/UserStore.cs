
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace IdentityWithoutEF.Data
{
    public class UserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
    {
        readonly string _connectionString;
        public UserStore(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var cmd = new SqlCommand("INSERT INTO ApplicationUser(UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled) VALUES(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9) SELECT CAST(SCOPE_IDENTITY() as int)", connection);


                cmd.Parameters.AddWithValue("@p1", user.UserName);
                cmd.Parameters.AddWithValue("@p2", user.NormalizedUserName);
                cmd.Parameters.AddWithValue("@p3", user.Email);
                cmd.Parameters.AddWithValue("@p4", user.NormalizedEmail ?? user.Email.ToUpper());
                cmd.Parameters.AddWithValue("@p5", user.EmailConfirmed);
                cmd.Parameters.AddWithValue("@p6", user.PasswordHash);
                cmd.Parameters.AddWithValue("@p7", user.PhoneNumber ?? "60234490");
                cmd.Parameters.AddWithValue("@p8", user.PhoneNumberConfirmed);
                cmd.Parameters.AddWithValue("@p9", user.TwoFactorEnabled);

                await cmd.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            // throw new System.NotImplementedException();
            return null;
        }

        public void Dispose()
        {

        }

        public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            // throw new System.NotImplementedException();
            return null;
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = new ApplicationUser();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var cmd = new SqlCommand("SELECT * FROM [ApplicationUser] where [NormalizedUserName] = @p1", connection);

                cmd.Parameters.AddWithValue("@p1", normalizedUserName);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        System.Console.WriteLine(reader.GetString(0));
                    }
                }

            }
            return user;

        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var cmd = new SqlCommand();
                cmd.CommandText = "UPDATE [ApplicationUser] SET UserName = @p1, NormalizedUserName = @p2, Email = @p3, NormalizedEmail = @p4, EmailConfirmed = @p5, PasswordHash = @p6, PhoneNumber = @p7, PhoneNumberConfirmed = @p8, TwoFactorEnabled = @p9";

                cmd.Parameters.AddWithValue("@p1", user.UserName);
                cmd.Parameters.AddWithValue("@p2", user.NormalizedUserName);
                cmd.Parameters.AddWithValue("@p3", user.Email);
                cmd.Parameters.AddWithValue("@p4", user.NormalizedEmail);
                cmd.Parameters.AddWithValue("@p5", user.EmailConfirmed);
                cmd.Parameters.AddWithValue("@p6", user.PasswordHash);
                cmd.Parameters.AddWithValue("@p7", user.PhoneNumber);
                cmd.Parameters.AddWithValue("@p8", user.PhoneNumberConfirmed);
                cmd.Parameters.AddWithValue("@p9", user.TwoFactorEnabled);

                await cmd.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }

            return IdentityResult.Success;
        }
    }
}