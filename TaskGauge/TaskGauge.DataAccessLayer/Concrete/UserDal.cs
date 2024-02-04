using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskGauge.Common;
using TaskGauge.DataAccessLayer.Interface;
using TaskGauge.DataTransferObject;
using TaskGauge.Entity.Context;
using TaskGauge.Entity.Entity;

namespace TaskGauge.DataAccessLayer.Concrete
{
    public class UserDal : IUserDal
    {
        private TaskGaugeContext _taskGaugeContext;
        public UserDal(TaskGaugeContext taskGaugeContext)
        {
            _taskGaugeContext = taskGaugeContext;
        }

        public string Register(RegisterDto registerDto)
        {
            try
            {
                var isExistUsername = _taskGaugeContext.User
                                  .AsEnumerable()
                                  .Any(entity => entity.Name.Equals(registerDto.Username, StringComparison.OrdinalIgnoreCase));
                if (isExistUsername)
                {
                    return $"{TextResources.UsernameExists} Type: error";
                }

                var securityQuestionEntity = (from entity in _taskGaugeContext.SecurityQuestion
                                              where entity.Question == registerDto.SecurityQuestion
                                              select new { entity.Id }).First();

                var securityQuestionId = securityQuestionEntity != null ? securityQuestionEntity.Id : default(int);

                _taskGaugeContext.User.Add(new User
                {
                    Name = registerDto.Username,
                    Password = CryptoPassword.EncryptPassword(registerDto.Password),
                    RecordTime = DateTime.Now,
                    SecurityQuestionId = securityQuestionId,
                    SecurityQuestionAnswer = registerDto.SecurityQuestionAnswer,
                });
                _taskGaugeContext.SaveChanges();
                return $"{TextResources.SuccessfullyRegisteredUser} Type: success";
            }
            catch (Exception exception)
            {
                return $"{exception.Message} Type: error";
            }

        }

        public string Login(LoginDto loginDto)
        {
            try
            {
                var userId = _taskGaugeContext.User.Where
                    (x => x.Name.Equals(loginDto.Username)).FirstOrDefault();
                if (userId == null)
                {
                    return $"{TextResources.WrongUsername} Type: error";
                }
                var isAcceptLogin = _taskGaugeContext.User.Where
                    (x=>x.Password.Equals(CryptoPassword.EncryptPassword(loginDto.Password))).Any();

                return isAcceptLogin ? $"{userId.Id}" : $"{TextResources.WrongPassword} Type: error";
            }

            catch (Exception exception)
            {
                return $"{exception.Message} Type: error";
            }
        }
    }
}
