using EventPlanner.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using EventPlanner.Models;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;
using FluentAssertions;
using UAParser;

namespace EventPlanner.Tests.Service
{
    public class LoginHistoryServiceTest
    {
        private readonly IDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoginHistoryService _loginHistoryService;
        private readonly DbSet<LoginHistory> _loginHistorySet;
        private readonly DbSet<AppUser> _userSet;


        public LoginHistoryServiceTest()
        {
            _context = Substitute.For<IDbContext>();
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            _loginHistorySet = Substitute.For<DbSet<LoginHistory>>();
            _userSet = Substitute.For<DbSet<AppUser>>();

            // SUT
            _loginHistoryService = new LoginHistoryService(_context, _httpContextAccessor);
        }

        [Fact]
        public async Task AddLoginRecord_ShouldAddRecord()
        {
            // Arrange
            int userId = 1;
            bool loginSucceeded = true;
            string ipAddress = "127.0.0.1";

            var userInDb = new AppUser { Id = 1, UserName = "Username" };

            var appUserList = new List<AppUser> { userInDb };
            var mockAppUserDbSet = appUserList.AsQueryable().BuildMockDbSet();
            _context.Users.Returns(mockAppUserDbSet);

            var loginHistoryList = new List<LoginHistory>();
            var loginHistoryDbSet = loginHistoryList.AsQueryable().BuildMockDbSet();
            _context.LoginHistory.Returns(loginHistoryDbSet);

            // Act
            await _loginHistoryService.AddLoginRecord(userId, loginSucceeded, ipAddress);

            // Assert
            _context.LoginHistory.Received(1).Add(Arg.Any<LoginHistory>());
            await _context.Received(1).SaveChangesAsync();
        }
    }
}
