using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Helpers;


namespace Suvarna.DGMS.Api.Repositories
{
    public class AuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
            try
            {
                //_connectionString =configuration.GetConnectionString("SqlServer");
                string isEncrypt = configuration["Is_Encrypt_Req"];
                string Db_Conn = configuration.GetConnectionString("SqlServer");
                if (isEncrypt == "Y")
                {
                    string key = configuration["SecurityKey"];

                    _connectionString = EnCryptDecrypt.CryptorEngine.Decrypt(Db_Conn, true);
                }
                else
                {
                    _connectionString = Db_Conn;
                }

                if (string.IsNullOrWhiteSpace(_connectionString))
                {
                    throw new Exception(
                        "Database connection string missing.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Decrypt Error : " + ex.Message);
            }
        }

        public async Task<LoginResponse> ValidateUserLogin(LoginRequest request)
        {
            try
            {
                using SqlConnection connection =
                    new SqlConnection(
                        _connectionString);

                await connection.OpenAsync();

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@IP_USER_NAME", request.UserName);

                parameters.Add("@IP_PASSWORD", request.Password);

                parameters.Add("@IP_CLIENT_NAME", request.ClientName);

                parameters.Add("@IP_GRP_ID", request.GroupId);

                parameters.Add("@IP_TRANSACTION_PASSWORD", request.TransactionPassword);

                parameters.Add("@IP_SYSTEM_TIME", request.SystemTime);

                parameters.Add("@IP_DOMAIN_VAL", string.IsNullOrWhiteSpace(request.DomainValue) ? "N" : request.DomainValue);

                parameters.Add("@OP_USER_ID", dbType: DbType.Int32, direction: ParameterDirection.Output);

                parameters.Add("@OP_RETURN_MSG", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("SE.LPR_GET_VALIDATE_USER_LOGIN_NEW", parameters, commandType: CommandType.StoredProcedure, commandTimeout: 30);

                return new LoginResponse
                {
                    UserId =
                        parameters.Get<int?>(
                            "@OP_USER_ID"),

                    ReturnMessage =
                        parameters.Get<string>(
                            "@OP_RETURN_MSG")
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    UserId = null,
                    ReturnMessage = ex.Message
                };
            }
        }
        public async Task<string> GetClientName()
        {
            SqlConnection? connection = null;

            try
            {
                connection =
                    new SqlConnection(
                        _connectionString);

                await connection.OpenAsync();

                var result =
                    await connection.QueryAsync<WebConfigModel>("PR_GET_WEBCONFIG_KEY_VALUES", commandType: CommandType.StoredProcedure);

                return result.FirstOrDefault(x => x.Key == "ClientName")?.Value ?? string.Empty;
            }
            catch (SqlException ex)
            {
                // Log Exception Here

                return string.Empty;
            }
            catch (Exception ex)
            {
                // Log Exception Here

                return string.Empty;
            }
            finally
            {
                if (connection != null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        await connection.CloseAsync();
                    }

                    await connection.DisposeAsync();
                }
            }
        }
        public async Task<UserDetailsModel> GetUserDetails(long userId)
        {
            UserDetailsModel response = new UserDetailsModel();

            SqlConnection connection = null;

            SqlCommand command = null;

            SqlDataReader reader = null;

            try
            {
                // =====================================================
                // CONNECTION
                // =====================================================

                connection = new SqlConnection(_connectionString);

                await connection.OpenAsync();

                // =====================================================
                // COMMAND
                // =====================================================

                command =
                    new SqlCommand("VISITOR.PR_GET_USER_DETAILS", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.CommandTimeout = 300;

                // =====================================================
                // PARAMETERS
                // =====================================================

                command.Parameters.AddWithValue("@IP_USER_ID", userId);

                // =====================================================
                // EXECUTE
                // =====================================================

                reader = await command.ExecuteReaderAsync();

                // =====================================================
                // READ DATA
                // =====================================================

                if (await reader.ReadAsync())
                {
                    response = new UserDetailsModel
                    {
                        UserId = reader.GetLongSafe("USER_ID") ?? 0,

                        UserCd = reader.GetStringSafe("USER_CD"),

                        UserName = reader.GetStringSafe("USER_NAME"),

                        EmailId = reader.GetStringSafe("EMAIL_ID"),

                        MobileNo = reader.GetStringSafe("MOBILE_NO"),

                        ReferenceName = reader.GetStringSafe("REFERENCE_NAME"),

                        RoleId = reader.GetLongSafe("ROLE_ID") ?? 0,

                        RoleCd = reader.GetStringSafe("ROLE_CD"),

                        RoleName = reader.GetStringSafe("ROLE_NAME")
                    };
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception(
                    $"Database Error : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Repository Error : {ex.Message}");
            }
            finally
            {
                // =====================================================
                // READER CLOSE
                // =====================================================

                if (reader != null && !reader.IsClosed)
                {
                    await reader.CloseAsync();
                }

                // =====================================================
                // COMMAND DISPOSE
                // =====================================================

                if (command != null)
                {
                    command.Dispose();
                }

                // =====================================================
                // CONNECTION CLOSE
                // =====================================================

                if (connection != null && connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }

                // =====================================================
                // CONNECTION DISPOSE
                // =====================================================

                if (connection != null)
                {
                    await connection.DisposeAsync();
                }
            }

            return response;
        }
        public async Task<List<UserFacilityModel>> GetUserFacilities(long userId)
        {
            List<UserFacilityModel> response = new List<UserFacilityModel>();

            SqlConnection connection = null;

            SqlCommand command = null;

            SqlDataReader reader = null;

            try
            {
                // =====================================================
                // CONNECTION
                // =====================================================

                connection = new SqlConnection(_connectionString);

                await connection.OpenAsync();

                // =====================================================
                // COMMAND
                // =====================================================

                command = new SqlCommand("LPR_GET_FACILITIES_BY_USER_ID", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.CommandTimeout = 300;

                // =====================================================
                // PARAMETERS
                // =====================================================

                command.Parameters.AddWithValue("@IP_USER_ID", userId);

                // =====================================================
                // EXECUTE
                // =====================================================

                reader = await command.ExecuteReaderAsync();

                // =====================================================
                // READ DATA
                // =====================================================

                while (await reader.ReadAsync())
                {
                    response.Add(
                        new UserFacilityModel
                        {
                            FacilityId = reader.GetLongSafe("FACILITY_ID") ?? 0,

                            FacilityName = reader.GetStringSafe("FACILITY_NAME"),

                            FacilityCd = reader.GetStringSafe("FACILITY_CD"),

                            FacilityRevNo = reader.GetIntSafe("FACILITY_REV_NO") ?? 0,

                            TotalRecordCount = reader.GetIntSafe("TOT_RECORD_CNT") ?? 0
                        });
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception(
                    $"Database Error : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Repository Error : {ex.Message}");
            }
            finally
            {
                // =====================================================
                // READER CLOSE
                // =====================================================

                if (reader != null &&
                    !reader.IsClosed)
                {
                    await reader.CloseAsync();
                }

                // =====================================================
                // COMMAND DISPOSE
                // =====================================================

                if (command != null)
                {
                    command.Dispose();
                }

                // =====================================================
                // CONNECTION CLOSE
                // =====================================================

                if (connection != null &&
                    connection.State ==
                    ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }

                // =====================================================
                // CONNECTION DISPOSE
                // =====================================================

                if (connection != null)
                {
                    await connection.DisposeAsync();
                }
            }

            return response;
        }
        public async Task<UserSessionResult> CreateUserSession(UserSessionRequest request)
        {
            try
            {

                using SqlConnection connection =
                    new SqlConnection(
                        _connectionString);

                await connection.OpenAsync();

                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@IP_MACHINE", request.Machine);

                parameters.Add("@IP_VERSION", request.Version);

                parameters.Add("@IP_TERMINAL", request.Terminal);

                parameters.Add("@IP_OSUSER", request.OsUser);

                parameters.Add("@IP_USER_ID", request.UserId);

                parameters.Add("@IP_ORG_ID", request.OrgId);

                parameters.Add("@IP_ROLE_ID", request.RoleId);

                parameters.Add("@IP_LOC_ID", request.LocId);

                parameters.Add("@IP_TIMEZONE_ID", request.TimeZoneId);

                parameters.Add("@IP_BROWSER", request.Browser);

                parameters.Add("@IP_BROWSER_VER", request.BrowserVersion);

                parameters.Add("@IP_CLIENT_NAME", request.ClientName);

                parameters.Add("@OP_SESSION_ID", dbType: DbType.Int32, direction: ParameterDirection.Output);

                //await connection.ExecuteAsync("LPR_INS_USERSESSION_NEW",parameters,commandType:CommandType.StoredProcedure);
                var sessionData = await connection.QueryFirstOrDefaultAsync<dynamic>("LPR_INS_USERSESSION_NEW", parameters, commandType: CommandType.StoredProcedure);

                //return new UserSessionResult
                //{
                //    SessionId =
                //        parameters.Get<int?>(
                //            "@OP_SESSION_ID")
                //};
                int? sessionId = parameters.Get<int?>("@OP_SESSION_ID");

                // SP returns no row when it fails internally -> avoid a crash on null
                if (sessionData == null)
                {
                    return new UserSessionResult { SessionId = sessionId };
                }

                return new UserSessionResult
                {
                    SessionId = sessionId,

                    RoleId = sessionData.ROLE_ID,

                    RoleName = sessionData.ROLE_NAME,

                    OrgId = sessionData.ORG_ID,

                    OrgName = (string?)sessionData.ORG_NAME,

                    DisplayName = (string?)sessionData.DISPLAY_NAME ?? (string?)sessionData.EMPLOYEE_NAME
                };
            }
            catch (SqlException sqlEx)
            {
                throw new Exception(
                    $"Database error while creating user session: {sqlEx.Message}",
                    sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Unexpected error while creating user session: {ex.Message}",
                    ex);
            }
        }
        public async Task<List<UserLocationResponse>> GetUsersByLocation(long locId)
        {
            List<UserLocationResponse> response = new List<UserLocationResponse>();

            SqlConnection connection = null;

            SqlCommand command = null;

            SqlDataReader reader = null;

            try
            {
                // =====================================================
                // CONNECTION
                // =====================================================

                connection = new SqlConnection(_connectionString);

                await connection.OpenAsync();

                // =====================================================
                // COMMAND
                // =====================================================

                command = new SqlCommand("VISITOR.PR_GET_USERS_BY_LOCATION", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.CommandTimeout = 300;

                // =====================================================
                // PARAMETERS
                // =====================================================

                command.Parameters.AddWithValue("@LOC_ID", locId);

                // =====================================================
                // EXECUTE
                // =====================================================

                reader = await command.ExecuteReaderAsync();

                // =====================================================
                // READ DATA
                // =====================================================

                while (await reader.ReadAsync())
                {
                    response.Add(
                        new UserLocationResponse
                        {
                            UserId = reader.GetLongSafe("USER_ID") ?? 0,

                            UserName = reader.GetStringSafe("USER_NAME"),

                            UserCd = reader.GetStringSafe("USER_CD"),

                            ReferenceId = reader.GetLongSafe("REFERENCE_ID") ?? 0,

                            ReferenceTypeId = reader.GetLongSafe("REFERENCE_TYPE_ID") ?? 0,

                            ReferenceTypeName = reader.GetStringSafe("REFERENCE_TYPE_NAME"),

                            LocationId = reader.GetLongSafe("LOCATION_ID") ?? 0,

                            LocationName = reader.GetStringSafe("LOCATION_NAME"),

                            EmployeeId = reader.GetLongSafe("EMPLOYEE_ID") ?? 0,

                            EmployeeName = reader.GetStringSafe("EMPLOYEE_NAME"),

                            DepartmentId = reader.GetLongSafe("DEPARTMENT_ID") ?? 0,

                            DepartmentName = reader.GetStringSafe("DEPARTMENT_NAME")
                        });
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception(
                    $"Database Error : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Repository Error : {ex.Message}");
            }
            finally
            {
                // =====================================================
                // READER CLOSE
                // =====================================================

                if (reader != null &&
                    !reader.IsClosed)
                {
                    await reader.CloseAsync();
                }

                // =====================================================
                // COMMAND DISPOSE
                // =====================================================

                if (command != null)
                {
                    command.Dispose();
                }

                // =====================================================
                // CONNECTION CLOSE
                // =====================================================

                if (connection != null &&
                    connection.State ==
                    ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }

                // =====================================================
                // CONNECTION DISPOSE
                // =====================================================

                if (connection != null)
                {
                    await connection.DisposeAsync();
                }
            }

            return response;
        }

        public async Task SaveRefreshToken(long userId, string refreshToken)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            await conn.OpenAsync();

            DynamicParameters param = new DynamicParameters();

            param.Add("@IP_USER_ID", userId);

            param.Add("@IP_REFRESH_TOKEN", refreshToken);

            await conn.ExecuteAsync("FNB.PR_SAVE_REFRESH_TOKEN", param, commandType: CommandType.StoredProcedure);
        }

        public async Task<RefreshTokenEntity?> GetRefreshToken(string refreshToken)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            await conn.OpenAsync();

            DynamicParameters param = new DynamicParameters();

            param.Add("@IP_REFRESH_TOKEN", refreshToken);

            return await conn.QueryFirstOrDefaultAsync<RefreshTokenEntity>("FNB.PR_GET_REFRESH_TOKEN", param, commandType: CommandType.StoredProcedure);
        }

        public async Task<JwtUserModel?> GetUserById(long userId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            await conn.OpenAsync();

            DynamicParameters param = new DynamicParameters();

            param.Add("@IP_USER_ID", userId);

            return await conn.QueryFirstOrDefaultAsync<JwtUserModel>("FNB.PR_GET_USER_BY_ID", param, commandType: CommandType.StoredProcedure);
        }

        public async Task RevokeToken(string refreshToken)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);

            await conn.OpenAsync();

            DynamicParameters param = new DynamicParameters();

            param.Add("@IP_REFRESH_TOKEN", refreshToken);

            await conn.ExecuteAsync("FNB.PR_REVOKE_REFRESH_TOKEN", param, commandType: CommandType.StoredProcedure);
        }

        public async Task<List<ModuleModel>> GetModules(long userId, int sessionId, int parentModuleId)
        {
            List<ModuleModel> response = new List<ModuleModel>();

            SqlConnection connection = null;

            SqlCommand command = null;

            SqlDataReader reader = null;

            try
            {
                // =====================================================
                // CONNECTION
                // =====================================================

                connection =
                    new SqlConnection(
                        _connectionString);

                await connection.OpenAsync();

                // =====================================================
                // COMMAND
                // =====================================================

                command =
                    new SqlCommand(
                        "SE.PR_GET_MODULES_DUMMY",
                        connection);

                command.CommandType =
                    CommandType.StoredProcedure;

                command.CommandTimeout = 300;

                // =====================================================
                // PARAMETERS
                // =====================================================

                command.Parameters.AddWithValue(
                    "@IP_USER_ID",
                    userId);

                command.Parameters.AddWithValue(
                    "@IP_SESSION_ID",
                    sessionId);

                command.Parameters.AddWithValue(
                    "@IP_PARENT_MODULE_ID",
                    parentModuleId);

                // =====================================================
                // EXECUTE
                // =====================================================

                reader =
                    await command.ExecuteReaderAsync();

                // =====================================================
                // READ DATA
                // =====================================================

                while (await reader.ReadAsync())
                {
                    response.Add(
                        new ModuleModel
                        {
                            UserId =
                                reader.GetLongSafe(
                                    "USER_ID") ?? 0,

                            DocIdType =
                                reader.GetStringSafe(
                                    "DOC_ID_TYPE"),

                            DocId =
                                reader.GetLongSafe(
                                    "DOC_ID"),

                            DocDesc =
                                reader.GetStringSafe(
                                    "DOC_DESC"),

                            PageUrl =
                                reader.GetStringSafe(
                                    "PAGE_URL"),

                            ParentDocIdType =
                                reader.GetStringSafe(
                                    "PARENT_DOC_ID_TYPE"),

                            ParentDocId =
                                reader.GetLongSafe(
                                    "PARENT_DOC_ID"),

                            ParentDocDesc =
                                reader.GetStringSafe(
                                    "PARENT_DOC_DESC"),

                            LocId =
                                reader.GetLongSafe(
                                    "LOC_ID"),

                            Priority =
                                reader.GetIntSafe(
                                    "PRIORITY"),

                            DocType =
                                reader.GetStringSafe(
                                    "DOC_TYPE"),

                            DisplayOrder =
                                reader.GetIntSafe(
                                    "DISPLAY_ORDER"),

                            NewPageUrl =
                                reader.GetStringSafe(
                                    "NEW_PAGE_URL"),

                            GridPageUrl =
                                reader.GetStringSafe(
                                    "GRID_PAGE_URL"),

                            ParentModuleFormCd =
                                reader.GetStringSafe(
                                    "PARENT_MODULE_FORM_CD"),

                            DocFrom =
                                reader.GetStringSafe(
                                    "DOC_FROM"),

                            ParentModuleId =
                                reader.GetLongSafe(
                                    "PARENT_MODULE_ID"),

                            ModuleFormCd =
                                reader.GetStringSafe(
                                    "MODULE_FORM_CD"),

                            ProjectCd =
                                reader.GetStringSafe(
                                    "PROJECT_CD")
                        });
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception(
                    $"Database Error : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Repository Error : {ex.Message}");
            }
            finally
            {
                // =====================================================
                // READER CLOSE
                // =====================================================

                if (reader != null &&
                    !reader.IsClosed)
                {
                    await reader.CloseAsync();
                }

                // =====================================================
                // COMMAND DISPOSE
                // =====================================================

                if (command != null)
                {
                    command.Dispose();
                }

                // =====================================================
                // CONNECTION CLOSE
                // =====================================================

                if (connection != null &&
                    connection.State ==
                    ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }

                // =====================================================
                // CONNECTION DISPOSE
                // =====================================================

                if (connection != null)
                {
                    await connection.DisposeAsync();
                }
            }

            return response;
        }

        public async Task<List<ModuleDocResponse>> GetUserModuleDocs(
    ModuleDocRequest request)
        {
            List<ModuleDocResponse> response =
                new List<ModuleDocResponse>();

            SqlConnection connection = null;

            SqlCommand command = null;

            SqlDataReader reader = null;

            try
            {
                // =====================================
                // CONNECTION
                // =====================================

                connection =
                    new SqlConnection(_connectionString);

                await connection.OpenAsync();

                // =====================================
                // COMMAND
                // =====================================

                command =
                    new SqlCommand(
                        "PR_GET_USER_MODULE_DOC_ALL",
                        connection);

                command.CommandType =
                    CommandType.StoredProcedure;

                command.CommandTimeout = 300;

                // =====================================
                // PARAMETERS
                // =====================================

                command.Parameters.AddWithValue(
                    "@IP_USER_ID",
                    request.UserId);

                command.Parameters.AddWithValue(
                    "@IP_MODULE_ID",
                    request.ModuleId);

                command.Parameters.AddWithValue(
                    "@IP_DOC_ID",
                    request.DocId);

                command.Parameters.AddWithValue(
                    "@IP_SESSION_ID",
                    request.SessionId);

                // =====================================
                // EXECUTE
                // =====================================

                reader =
                    await command.ExecuteReaderAsync();

                // =====================================
                // READ DATA
                // =====================================

                while (await reader.ReadAsync())
                {
                    response.Add(
                        new ModuleDocResponse
                        {
                            ModuleId =
                                reader.GetIntSafe("MODULE_ID") ?? 0,

                            ParentModuleId =
                                reader.GetIntSafe("PARENT_MODULE_ID") ?? 0,

                            DocId =
                                reader.GetIntSafe("DOC_ID") ?? 0,

                            DocCd =
                                reader.GetStringSafe("DOC_CD"),

                            DocName =
                                reader.GetStringSafe("DOC_NAME"),

                            ModuleName =
                                reader.GetStringSafe("MODULE_NAME"),

                            PageUrl =
                                reader.GetStringSafe("PAGE_URL"),

                            NewPageUrl =
                                reader.GetStringSafe("NEW_PAGE_URL"),

                            AccessAdd =
                                reader.GetStringSafe("ACCESS_ADD"),

                            AccessMod =
                                reader.GetStringSafe("ACCESS_MOD"),

                            AccessDel =
                                reader.GetStringSafe("ACCESS_DEL"),

                            AccessQry =
                                reader.GetStringSafe("ACCESS_QRY"),

                            AccessApp =
                                reader.GetStringSafe("ACCESS_APP"),

                            AccessExe =
                                reader.GetStringSafe("ACCESS_EXE"),

                            RecordStatus =
                                reader.GetStringSafe("DOC_RECORD_STATUS"),

                            DisplayOrder =
                                reader.GetIntSafe("DISPLAY_ORDER") ?? 0
                        });
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception(
                    $"Database Error : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Repository Error : {ex.Message}");
            }
            finally
            {
                // =====================================
                // READER CLOSE
                // =====================================

                if (reader != null &&
                    !reader.IsClosed)
                {
                    await reader.CloseAsync();
                }

                // =====================================
                // COMMAND DISPOSE
                // =====================================

                if (command != null)
                {
                    command.Dispose();
                }

                // =====================================
                // CONNECTION CLOSE
                // =====================================

                if (connection != null &&
                    connection.State ==
                    ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }

                // =====================================
                // CONNECTION DISPOSE
                // =====================================

                if (connection != null)
                {
                    await connection.DisposeAsync();
                }
            }

            return response;
        }
    }
}
