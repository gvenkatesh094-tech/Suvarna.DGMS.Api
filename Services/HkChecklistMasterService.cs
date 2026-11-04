using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    // 28-Oct: HK Checklist Master. Session comes from the X-Session-Id header (body/query only as fallback),
    // the procedures take group / org / location / user from that session.
    public class HkChecklistMasterService
    {
        private readonly HkChecklistMasterRepository _repo;
        private readonly SessionContext _session;

        public HkChecklistMasterService(HkChecklistMasterRepository repo, SessionContext session)
        {
            _repo = repo;
            _session = session;
        }

        public ApiResponse<HkChkLookups> GetLookups(int sessionId)
        {
            var data = _repo.GetLookups(_session.Require(sessionId));
            return ApiResponse<HkChkLookups>.Success("Lookups fetched successfully.", data);
        }

        public ApiResponse<List<HkChkItem>> GetItems(int sessionId)
        {
            var data = _repo.GetItems(_session.Require(sessionId));
            return ApiResponse<List<HkChkItem>>.Success($"{data.Count} item(s) fetched successfully.", data);
        }

        public ApiResponse<HkChkSaveResult> SaveItem(HkChkItemSaveRequest request)
        {
            var data = _repo.SaveItem(_session.Require(request.sessionId), request);
            if (data.status != "S")
            {
                throw new UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to save checklist item." : data.message);
            }
            return ApiResponse<HkChkSaveResult>.Success(data.message, data);
        }

        public ApiResponse<List<HkChkTemplate>> GetTemplates(int sessionId)
        {
            var data = _repo.GetTemplates(_session.Require(sessionId));
            return ApiResponse<List<HkChkTemplate>>.Success($"{data.Count} template(s) fetched successfully.", data);
        }

        public ApiResponse<List<HkChkTemplateItem>> GetTemplateItems(int sessionId, int templateId)
        {
            var data = _repo.GetTemplateItems(_session.Require(sessionId), templateId);
            return ApiResponse<List<HkChkTemplateItem>>.Success($"{data.Count} item(s) fetched successfully.", data);
        }

        public ApiResponse<HkChkSaveResult> SaveTemplate(HkChkTemplateSaveRequest request)
        {
            var data = _repo.SaveTemplate(_session.Require(request.sessionId), request);
            if (data.status != "S")
            {
                throw new UserMessageException(string.IsNullOrWhiteSpace(data.message) ? "Failed to save template." : data.message);
            }
            return ApiResponse<HkChkSaveResult>.Success(data.message, data);
        }
    }
}
