using AutoMapper;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using ONO.Application.DTOs.UserDTOs;
using ONO.Application.Interfaces;
using ONO.Core.AnotherObjects;
using ONO.Core.Entities;
using ONO.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONO.Application.Services
{
    public class UserServices : Services<User>, IUserServices
    {
        readonly UserManager<User> _userManager;
        readonly IServices<User> _userServices;
        readonly IServices<UserAddress> _addressServices;
        readonly IMapper _mapper;

        public UserServices(UserManager<User> userManager, IUnitOfWork unitOfWork, IRepo<User> repo, IServices<User> userServices, IServices<UserAddress> addressServices, IMapper mapper) : base(unitOfWork, repo)
        {
            _userManager = userManager;
            _userServices = userServices;
            _addressServices = addressServices;
            _mapper = mapper;
        }


        public async Task<ResponseInfo> UpdateUserProfile(UpdateUserDto newUserInfo, int userId)
        {
            var user = await GetAsync(u => u.Id == userId, includes: u => u.Addresses);
            _mapper.Map(newUserInfo, user);

            int number = GenerateRandomNumbers();
            user.UserName = $"{user.Fname[0..1]}{user.Lname[0..1]}_{number}".ToUpper();
            user.NormalizedUserName = user.UserName;
            user.NormalizedEmail = user.Email?.ToUpper();

            user.Addresses.Clear();

            if (newUserInfo.Address != null)
            {
                foreach (var addressDto in newUserInfo.Address)
                {
                    var address = _mapper.Map<UserAddress>(addressDto);
                    address.UserId = userId;
                    user.Addresses.Add(address);
                }
            }

            await UpdateAsync(user);
            await SaveChangesAsync();

            return new()
            {
                IsSuccess = true,
                Message = "user updated successful",
            };
        }

        public async Task<ResponseInfo> ChangePassword(string oldPassword, string newPassword, int useId)
        {
            var user = await _userServices.GetAsync(u => u.Id == useId);
            if (user is null)
            {
                return new()
                {
                    IsSuccess = false,
                    Message = "User not exist!"
                };
            }

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if (!result.Succeeded)
            {
                string errorMessage = "";

                foreach (var error in result.Errors)
                {
                    errorMessage += error.Description;
                }

                return new()
                {
                    IsSuccess = false,
                    Message = errorMessage
                };
            }

            return new()
            {
                IsSuccess = true,
                Message = "Password change successful"
            };
        }

        public async Task<ResponseInfo> AddUserAddress(UserAddressDto addressInfo, int userId)
        {
            if (string.IsNullOrEmpty(addressInfo.Governorate))
            {
                return new()
                {
                    IsSuccess = false,
                    Message = "Governorate field is required!"
                };
            }
            else if (string.IsNullOrEmpty(addressInfo.FullAddress))
            {
                return new()
                {
                    IsSuccess = false,
                    Message = "Full Address field is required!"
                };
            }
            else if (string.IsNullOrEmpty(addressInfo.City))
            {
                return new()
                {
                    IsSuccess = false,
                    Message = "City field is required!"
                };
            }

            var newAddress = _mapper.Map<UserAddress>(addressInfo);
            newAddress.UserId = userId;

            await _addressServices.AddAsync(newAddress);
            await SaveChangesAsync();

            return new()
            {
                IsSuccess = true,
                Message = "The address added successful"
            };
        }

        public async Task ContactUs(string name, string phoneNumber)
        {
            const string Email = "mazenkhtab123@gmail.com";
            const string Password = "zjcj qoqb aeup eswf";

            MimeMessage email = new();
            email.From.Add(new MailboxAddress("ONO Store Contact Us", Email));
            email.To.Add(MailboxAddress.Parse("mazenkhtab11@gmail.com"));

            email.Subject = "New Contact Submission from Website";

            email.Body = new TextPart("html")
            {
                Text = $@"<h2>User Contact Info</h2>
                        <p><strong>Name:</strong> {name}</p>
                        <p><strong>Phone Number:</strong> {phoneNumber}</p>"
            };

            try
            {
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(Email, Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        int GenerateRandomNumbers()
        {
            Random rn = new();
            int randomNumber = 0;

            for (int i = 1; i <= 5; i++)
            {
                randomNumber += rn.Next(0, 50);
            }

            return randomNumber;
        }
    }
}
