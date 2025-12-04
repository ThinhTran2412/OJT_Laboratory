namespace IAM_Service.Application.Interface.IPrivilege
{
    public interface IPrivilegeNormalizationService
    {
        public List<int> Normalize(List<int> initialPrivilegeIds);
    }
}
