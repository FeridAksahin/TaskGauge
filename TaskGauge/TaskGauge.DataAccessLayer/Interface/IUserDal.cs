using TaskGauge.DataTransferObject;
using TaskGauge.ViewModel;

namespace TaskGauge.DataAccessLayer.Interface
{
    public interface IUserDal
    {
        public string Register(RegisterDto registerDto);
        public UserViewModel Login(LoginDto loginDto);
    }
}
