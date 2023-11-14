using TaskGauge.DataTransferObject;

namespace TaskGauge.DataAccessLayer.Interface
{
    public interface IUserDal
    {
        public string Register(RegisterDto registerDto);
    }
}
