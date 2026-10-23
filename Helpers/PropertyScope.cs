using Microsoft.Data.SqlClient;

namespace Suvarna.DGMS.Api.Helpers
{
    /// <summary>
    /// NEW: property chosen at login for the current request.
    /// Read from the DB session (DGMS.FN_DGMS_SESSION_PROPERTY), so it cannot be changed from the browser.
    /// null = no property chosen ("All Properties" / old login) -> no filter, same as before.
    /// Used by list APIs that have no session parameter in their procedure (property, building, floor lists).
    /// </summary>
    public class PropertyScope
    {
        private readonly SessionContext _session;
        private readonly string _conn;
        private bool _loaded;
        private int? _propertyId;

        public PropertyScope(SessionContext session, IConfiguration configuration)
        {
            _session = session;
            _conn = configuration.GetConnectionString("SqlServer") ?? "";
        }

        public int? CurrentPropertyId
        {
            get
            {
                if (_loaded) return _propertyId;
                _loaded = true;

                var sid = _session.SessionId;
                if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(_conn)) return _propertyId = null;

                try
                {
                    using var conn = new SqlConnection(_conn);
                    using var cmd = new SqlCommand("SELECT DGMS.FN_DGMS_SESSION_PROPERTY(@sid)", conn);
                    cmd.Parameters.AddWithValue("@sid", sid);
                    conn.Open();
                    var v = cmd.ExecuteScalar();
                    _propertyId = v == null || v == DBNull.Value ? null : Convert.ToInt32(v);
                }
                catch
                {
                    _propertyId = null;   // function not installed -> old behaviour
                }

                return _propertyId;
            }
        }

        /// <summary>Keeps only the rows of the login property (no filter when none is chosen).</summary>
        public List<T> Filter<T>(List<T> rows, Func<T, int> propertyOf)
        {
            var p = CurrentPropertyId;
            if (p is null || rows == null) return rows ?? new List<T>();
            return rows.Where(r => propertyOf(r) == p.Value).ToList();
        }
    }
}
