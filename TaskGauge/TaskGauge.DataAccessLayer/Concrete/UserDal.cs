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
using TaskGauge.ViewModel;

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

                var roleId = (from entity in _taskGaugeContext.Role
                              where entity.Name.Equals(registerDto.RoleName)
                              select entity.Id).FirstOrDefault();

                var securityQuestionId = securityQuestionEntity != null ? securityQuestionEntity.Id : default(int);

                _taskGaugeContext.User.Add(new User
                {
                    Name = registerDto.Username,
                    Password = CryptoPassword.EncryptPassword(registerDto.Password),
                    RecordTime = DateTime.Now,
                    SecurityQuestionId = securityQuestionId,
                    SecurityQuestionAnswer = registerDto.SecurityQuestionAnswer,
                    RoleId = roleId,
                });
                _taskGaugeContext.SaveChanges();
                return $"{TextResources.SuccessfullyRegisteredUser} Type: success";
            }
            catch (Exception exception)
            {
                return $"{exception.Message} Type: error";
            }

        }

        public UserViewModel Login(LoginDto loginDto)
        {
            try
            {
                var user = (from entity in _taskGaugeContext.User
                            where entity.Name.EndsWith(loginDto.Username)
                            select new UserViewModel
                            {
                                Id = Convert.ToString(entity.Id),
                                RoleName = entity.Role.Name,
                            }).FirstOrDefault();

                if (user == null)
                { 
                    return new UserViewModel
                    {
                        ErrorMessage = TextResources.WrongUsername
                    };
                }

                var isAcceptLogin = _taskGaugeContext.User.Where
                    (x => x.Password.Equals(CryptoPassword.EncryptPassword(loginDto.Password))).Any();

                if (!isAcceptLogin)
                {
                    user.ErrorMessage = TextResources.WrongPassword;
                }
                return user;
            }

            catch (Exception exception)
            {
                return new UserViewModel
                {
                    ErrorMessage = exception.Message
                };
        }
        }
    }
}
