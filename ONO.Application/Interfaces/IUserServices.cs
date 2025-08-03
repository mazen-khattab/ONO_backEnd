using ONO.Application.DTOs.UserDTOs;
using ONO.Core.AnotherObjects;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Interfaces
{
    public interface IUserServices : IServices<User>
    {
        Task<ResponseInfo> UpdateUserProfile(UpdateUserDto newUserInfo, int userId);
        Task<ResponseInfo> ChangePassword(string oldPassword, string newPassword, int useId);
        Task<ResponseInfo> AddUserAddress(UserAddressDto addressInfo, int userId);
        Task ContactUs(string name, string phoneNumber);
    }
}
