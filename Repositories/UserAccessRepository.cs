using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;

namespace Suvarna.DGMS.Api.Repositories
{
    /// <summary>User -> location -> property permissions (permission screen).</summary>
    public class UserAccessRepository
    {
        private readonly string _conn;
        private readonly SessionContext _session;

        public UserAccessRepository(IConfiguration configuration, SessionContext session)
        {
            _conn = configuration.GetConnectionString("SqlServer")
                    ?? throw new InvalidOperationException("Connection string 'SqlServer' is missing.");
            _session = session;
        }

        public async Task<List<AccessUserModel>> GetUsers(string? search)
        {
            using var con = new SqlConnection(_conn);

            var p = new DynamicParameters();
            p.Add("@IP_SESSION_ID", _session.Require());
            p.Add("@IP_SEARCH", string.IsNullOrWhiteSpace(search) ? null : search.Trim());

            var rows = await con.QueryAsync<dynamic>("DGMS.PR_DGMS_GET_ACCESS_USERS", p, commandType: CommandType.StoredProcedure, commandTimeout: 60);

            return rows.Select(r => new AccessUserModel
            {
                UserId = Convert.ToInt32(r.USER_ID),
                UserName = (string?)r.USER_NAME ?? string.Empty,
                HomeLocId = r.HOME_LOC_ID == null ? (int?)null : Convert.ToInt32(r.HOME_LOC_ID),
                IsAdmin = (string?)r.IS_ADMIN == "Y",
                PropertyCount = Convert.ToInt32(r.PROPERTY_COUNT)
            }).ToList();
        }

        public async Task<List<AccessPropertyModel>> GetProperties()
        {
            using var con = new SqlConnection(_conn);

            var p = new DynamicParameters();
            p.Add("@IP_SESSION_ID", _session.Require());

            var rows = await con.QueryAsync<dynamic>("DGMS.PR_DGMS_GET_ACCESS_PROPERTIES", p, commandType: CommandType.StoredProcedure, commandTimeout: 60);

            return rows.Select(r => new AccessPropertyModel
            {
                PropertyId = Convert.ToInt32(r.PROPERTY_ID),
                PropertyCd = (string?)r.PROPERTY_CD ?? string.Empty,
                PropertyNm = (string?)r.PROPERTY_NM ?? string.Empty,
                PropertyTypeCd = (string?)r.PROPERTY_TYPE_CD,
                City = (string?)r.CITY,
                LocId = Convert.ToInt32(r.LOC_ID)
            }).ToList();
        }

        public async Task<UserPropertyAccessModel> GetUserAccess(int userId)
        {
            using var con = new SqlConnection(_conn);

            var p = new DynamicParameters();
            p.Add("@IP_SESSION_ID", _session.Require());
            p.Add("@IP_USER_ID", userId);

            using var grid = await con.QueryMultipleAsync("DGMS.PR_DGMS_GET_USER_PROPERTY_ACCESS", p, commandType: CommandType.StoredProcedure, commandTimeout: 60);

            var head = await grid.ReadFirstOrDefaultAsync<dynamic>();
            var items = await grid.ReadAsync<dynamic>();

            return new UserPropertyAccessModel
            {
                UserId = userId,
                IsAdmin = (string?)head?.IS_ADMIN == "Y",
                Remarks = (string?)head?.REMARKS,
                ModifyDt = (DateTime?)head?.MODIFY_DT,
                Properties = items.Select(r => new UserPropertyAccessItem
                {
                    PropertyId = Convert.ToInt32(r.PROPERTY_ID),
                    LocId = Convert.ToInt32(r.LOC_ID),
                    IsDefault = (string?)r.IS_DEFAULT == "Y",
                    PropertyCd = (string?)r.PROPERTY_CD ?? string.Empty,
                    PropertyNm = (string?)r.PROPERTY_NM ?? string.Empty
                }).ToList()
            };
        }

        public async Task<(bool Success, string Message)> Save(SaveUserPropertyAccessRequest request, string forceAdmin)
        {
            using var con = new SqlConnection(_conn);

            var p = new DynamicParameters();
            p.Add("@IP_SESSION_ID", _session.Require());
            p.Add("@IP_USER_ID", request.UserId);
            p.Add("@IP_IS_ADMIN", request.IsAdmin ? "Y" : "N");
            p.Add("@IP_PROPERTY_IDS", string.Join(",", (request.PropertyIds ?? new List<int>()).Where(x => x > 0).Distinct()));
            p.Add("@IP_DEFAULT_PROPERTY_ID", request.DefaultPropertyId is > 0 ? request.DefaultPropertyId : null);
            p.Add("@IP_REMARKS", string.IsNullOrWhiteSpace(request.Remarks) ? null : request.Remarks.Trim());
            p.Add("@IP_FORCE_ADMIN", forceAdmin);
            p.Add("@OP_STATUS", dbType: DbType.String, size: 1, direction: ParameterDirection.Output);
            p.Add("@OP_MSG", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

            await con.ExecuteAsync("DGMS.PR_DGMS_SAVE_USER_PROPERTY_ACCESS", p, commandType: CommandType.StoredProcedure, commandTimeout: 60);

            return (p.Get<string>("@OP_STATUS") == "S", p.Get<string>("@OP_MSG") ?? string.Empty);
        }
    }
}
