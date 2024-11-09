namespace Admin_Dashboard.Core.Dtos.Auth
{
    public class LoginServiceresponseDto
    {
        public string NewToken { get; set; }
        public UserInfoResult UserInfo { get; set; }
    }
}
