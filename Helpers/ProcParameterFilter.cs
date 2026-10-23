using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;

namespace Suvarna.DGMS.Api.Helpers
{
    /// <summary>
    /// NEW: keeps the API working when the database still has the OLD version of a procedure
    /// (for example 04_front_office_fixes.sql not run yet). Parameters the procedure does not have
    /// are removed before the call, so SQL Server does not fail with
    /// "Procedure or function ... has too many arguments specified".
    /// The parameter list is cached for 5 minutes, so after the script is run the new
    /// parameters are used without restarting the API.
    /// </summary>
    public static class ProcParameterFilter
    {
        private static readonly ConcurrentDictionary<string, (DateTime loadedAt, HashSet<string>? names)> Cache = new();
        private static readonly TimeSpan CacheTime = TimeSpan.FromMinutes(5);

        /// <summary>Call after the connection is open, just before Execute.</summary>
        public static void RemoveUnknown(SqlCommand cmd)
        {
            var known = GetParameters(cmd.Connection, cmd.CommandText);
            if (known == null) return;   // could not read the list -> leave the call as it is

            // FIX: a procedure with NO parameters (old PR_GET_OCCUPANT_CATEGORY) was treated as "could not read",
            // so @IP_SESSION_ID was sent and the call failed: "has no parameters and arguments were supplied"

            for (int i = cmd.Parameters.Count - 1; i >= 0; i--)
            {
                if (!known.Contains(cmd.Parameters[i].ParameterName))
                    cmd.Parameters.RemoveAt(i);
            }
        }

        /// <summary>Parameter names of the procedure (empty = it has none); null = could not be read.</summary>
        private static HashSet<string>? GetParameters(SqlConnection conn, string procName)
        {
            if (Cache.TryGetValue(procName, out var hit) && DateTime.UtcNow - hit.loadedAt < CacheTime)
                return hit.names;

            HashSet<string>? names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                // first row: does the procedure exist; then its parameters
                using var q = new SqlCommand(
                    "SELECT OBJECT_ID(@p); SELECT name FROM sys.parameters WHERE object_id = OBJECT_ID(@p)", conn);
                q.Parameters.AddWithValue("@p", procName);
                using var r = q.ExecuteReader();
                var exists = r.Read() && !r.IsDBNull(0);
                if (r.NextResult())
                    while (r.Read()) names.Add(r.GetString(0));
                if (!exists) names = null;   // not found / no permission -> no filtering
            }
            catch
            {
                names = null;   // no permission on sys.parameters etc. -> no filtering
            }

            Cache[procName] = (DateTime.UtcNow, names);
            return names;
        }
    }
}
