namespace Admin_Dashboard.Core.Dtos.Auth
{
    public class UpdateRoleDto
    {
        public string UserName { get; set; }
        public RoleType NewRole { get; set; }
    }

    public enum RoleType 
    { 
        ADMIN,
        MANAGER,
        USER
    }
}
