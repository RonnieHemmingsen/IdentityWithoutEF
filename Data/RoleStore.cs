
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace IdentityWithoutEF.Data
{
    public class RoleStore : IRoleStore<ApplicationRole>
    {
        private readonly string _connectionString;

        public RoleStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConfiguration");
        }

        public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var cmd = new SqlCommand("INSERT INTO [ApplicationRole] ([Name], [NormalizedName]) VALUES (@p1, @p2) SELECT CAST(SCOPE_IDENTITY() as int", connection);

                cmd.Parameters.AddWithValue("@p1", role.Name);
                cmd.Parameters.AddWithValue("@p2", role.NormalizedName);

                await cmd.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }

            return IdentityResult.Success;

        }

        public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand("DELETE FROM [ApplicationRole] WHERE [id] = @p1", connection);

                cmd.Parameters.AddWithValue("@p1", role.Id);

                await cmd.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }

            return IdentityResult.Success;
        }

        public void Dispose()
        {
            //nothing to dispose
        }

        public async Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var role = new ApplicationRole();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var cmd = new SqlCommand("SELECT * FROM [ApplicationRole] WHERE [Id] = @p1", connection);

                cmd.Parameters.AddWithValue("@p1", roleId);

                using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        System.Console.WriteLine("her " + reader.GetString(0));
                    }
                }
            }

            return role;
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var role = new ApplicationRole();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var cmd = new SqlCommand("SELECT * FROM [ApplicationRole] WHERE [NormalizedName] = @p1", connection);

                cmd.Parameters.AddWithValue("@p1", normalizedRoleName);

                using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        System.Console.WriteLine("her her" + reader.GetString(0));
                    }
                }
            }

            return role;
        }

        public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var cmd = new SqlCommand("UPDATE [ApplicationRole] SET [Name] = @p1, [NormalizedName = @p2 WHERE [Id] = @p3]", connection);

                cmd.Parameters.AddWithValue("@p1", role.Name);
                cmd.Parameters.AddWithValue("@p2", role.NormalizedName);
                cmd.Parameters.AddWithValue("@p3", role.Id);

                await cmd.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }

            return IdentityResult.Success;
        }
    }
}