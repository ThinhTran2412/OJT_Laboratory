using IAM_Service.Application.Interface.IPrivilege;

namespace IAM_Service.Application.Helpers
{
    public class PrivilegeNormalizationService : IPrivilegeNormalizationService
    {
        private static readonly Dictionary<int, int> PrivilegeDependencyMap = new Dictionary<int, int>
        {
            // Patient Test Order Privileges (1-8) - Phụ thuộc vào READ_ONLY (1)
            { 3, 1 },  // MODIFY_TEST_ORDER -> READ_ONLY
            { 5, 1 },  // REVIEW_TEST_ORDER (tính chất modify result) -> READ_ONLY
            { 7, 1 },  // MODIFY_COMMENT -> READ_ONLY
            
            // Configuration Privileges (9-12) - Phụ thuộc vào VIEW_CONFIGURATION (9)
            { 11, 9 }, // MODIFY_CONFIGURATION -> VIEW_CONFIGURATION
            
            // User Management Privileges (13-17) - Phụ thuộc vào VIEW_USER (13)
            { 15, 13 },// MODIFY_USER -> VIEW_USER
            { 17, 13 },// LOCK_UNLOCK_USER (tính chất modify status) -> VIEW_USER
            
            // Role Management Privileges (18-21) - Phụ thuộc vào VIEW_ROLE (18)
            { 20, 18 },// UPDATE_ROLE (tương đương MODIFY) -> VIEW_ROLE
            
            // Lab Management Privileges (22-29) 
            // MODIFY_REAGENTS (24) -> VIEW_EVENT_LOGS (22) (Giả định vì thiếu VIEW_REAGENTS)
            { 24, 22 },
            // ACTIVATE_DEACTIVATE_INSTRUMENT (28) -> VIEW_INSTRUMENT (27)
            { 28, 27 }
        };

        public List<int> Normalize(List<int> initialIds)
        {
            if (initialIds == null || initialIds.Count == 0)
            {
                return new List<int>();
            }

            var uniqueIds = new HashSet<int>(initialIds);
            var idsToAdd = new HashSet<int>();

            foreach (var actionId in initialIds)
            {
                // Kiểm tra xem ID hành động này có quyền phụ thuộc View/Read không
                if (PrivilegeDependencyMap.TryGetValue(actionId, out int requiredViewId))
                {
                    // Nếu quyền View bắt buộc chưa có trong danh sách
                    if (!uniqueIds.Contains(requiredViewId))
                    {
                        idsToAdd.Add(requiredViewId);
                    }
                }
            }

            // Thêm tất cả các quyền View/Read bắt buộc vào danh sách
            uniqueIds.UnionWith(idsToAdd);

            return uniqueIds.ToList();
        }
    }
}


