using Microsoft.AspNetCore.Identity;
using ModelLayer.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ModelLayer.DataTransferObjects;
using ServiceLayer;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using ModelLayer.Helper;

namespace UnitTests.ServiceLayer
{
    public class RoleServiceTests
    {
        private const string TEST_ROLE_1 = "testRolle1";
        private const string TEST_ROLE_2 = "testRolle2";
        private const string UPDATED_ROLE_NAME_1 = "testRolle3";
        private const string UPDATED_ROLE_NAME_2 = "testRolle4";
        private static readonly string ADDED_CLAIM = RoleClaims.ADMIN_CLAIMS[0];
        private static readonly List<Role> roles = new List<Role>() { new Role() { Id = 1, Name = TEST_ROLE_1 }, new Role() { Id = 2, Name = TEST_ROLE_2 } };
        private static readonly IList<Claim> claimsForRole1 = new List<Claim>() { new Claim(RoleClaims.CONTACT_CLAIMS[0], RoleClaims.CONTACT_CLAIMS[0]), new Claim(RoleClaims.CONTACT_CLAIMS[1], RoleClaims.CONTACT_CLAIMS[1]) };
        private static readonly IList<Claim> claimsForRole2 = new List<Claim>() { new Claim(RoleClaims.ORGANISATION_CLAIMS[0], RoleClaims.ORGANISATION_CLAIMS[0]), new Claim(RoleClaims.ORGANISATION_CLAIMS[1], RoleClaims.ORGANISATION_CLAIMS[1]) };

        [Fact]
        public void UpdateRoleWithClaimsAsync_worksCorrectly()
        {
            //Arrange
            var store = new Mock<IRoleStore<Role>>();
            List<IRoleValidator<Role>> list = new List<IRoleValidator<Role>>();
            list.Add(new Mock<IRoleValidator<Role>>().Object);
            IEnumerable<IRoleValidator<Role>> enumerable = list.AsEnumerable();
            var lookupNormalizer = new Mock<ILookupNormalizer>();
            var errorDescriber = new IdentityErrorDescriber();
            var logger = new Mock<Microsoft.Extensions.Logging.ILogger<RoleManager<Role>>>();
            var roleRanager = new Mock<RoleManager<Role>>(store.Object, enumerable, lookupNormalizer.Object, errorDescriber, logger.Object);
            var listRoles = roles.AsQueryable().BuildMock();
            roleRanager.Setup(man => man.Roles).Returns(listRoles.Object);
            roleRanager.Setup(man => man.UpdateAsync(It.IsAny<Role>())).Returns<Role>(a => UpdateRole(a));
            roleRanager.Setup(man => man.GetClaimsAsync(It.IsAny<Role>())).Returns<Role>(role => GetClaimsForRole(role));
            roleRanager.Setup(man => man.RemoveClaimAsync(It.IsAny<Role>(), It.IsAny<Claim>())).Returns<Role, Claim>((role, claim) => RemoveClaimFromRole(role, claim));
            roleRanager.Setup(man => man.AddClaimAsync(It.IsAny<Role>(), It.IsAny<Claim>())).Returns<Role, Claim>((role, claim) => AddClaimToRole(role, claim));
            RoleDto roleToUpdate = new RoleDto() { Id = 1, Name = UPDATED_ROLE_NAME_1, Claims = new List<string>() { RoleClaims.CONTACT_CLAIMS[0], ADDED_CLAIM } };
            RoleDto roleToUpdate2 = new RoleDto() { Id = 2, Name = UPDATED_ROLE_NAME_2, Claims = new List<string>() { RoleClaims.ORGANISATION_CLAIMS[0], RoleClaims.ORGANISATION_CLAIMS[1], ADDED_CLAIM } };
            RoleService roleService = new RoleService(roleRanager.Object);

            //Act
            roleService.UpdateRoleWithClaimsAsync(roleToUpdate).Wait();
            roleService.UpdateRoleWithClaimsAsync(roleToUpdate2).Wait();

            //Assert
            Assert.Equal(UPDATED_ROLE_NAME_1, roles[0].Name);
            Assert.Equal(2, claimsForRole1.Count);
            Assert.Equal(ADDED_CLAIM, claimsForRole1[1].Value);

            Assert.Equal(UPDATED_ROLE_NAME_2, roles[1].Name);
            Assert.Equal(3, claimsForRole2.Count);
            Assert.Equal(RoleClaims.ORGANISATION_CLAIMS[0], claimsForRole2[0].Value);
            Assert.Equal(RoleClaims.ORGANISATION_CLAIMS[1], claimsForRole2[1].Value);
            Assert.Equal(ADDED_CLAIM, claimsForRole2[2].Value);
        }

        private Task<IdentityResult> AddClaimToRole(Role role, Claim claim)
        {
            if (role.Id == 1)
            {
                claimsForRole1.Add(claim);
                return Task.FromResult(IdentityResult.Success);
            }
            else if (role.Id == 2)
            {
                claimsForRole2.Add(claim);
                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "301", Description = "role not found!" }));
            }
        }

        private Task<IdentityResult> RemoveClaimFromRole(Role role, Claim claim)
        {
            if (role.Id == 1)
            {
                Claim claimToDelete = claimsForRole1.FirstOrDefault(a => a.Value.Equals(claim.Value));
                if (claimToDelete != null)
                {
                    claimsForRole1.Remove(claimToDelete);
                }
                return Task.FromResult(IdentityResult.Success);
            }
            else if (role.Id == 2)
            {
                Claim claimToDelete = claimsForRole2.FirstOrDefault(a => a.Value.Equals(claim.Value));
                if (claimToDelete != null)
                {
                    claimsForRole2.Remove(claimToDelete);
                }
                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "301", Description = "role not found!" }));
            }
        }

        private Task<IList<Claim>> GetClaimsForRole(Role role)
        {
            if (role.Id == 1)
            {
                return Task.FromResult(claimsForRole1);
            }
            else if (role.Id == 2)
            {
                return Task.FromResult(claimsForRole2);
            }
            else
            {
                IList<Claim> empty = new List<Claim>();
                return Task.FromResult(empty);
            }
        }

        private Task<IdentityResult> UpdateRole(Role role)
        {
            foreach (Role rol in roles)
            {
                if (role.Id == rol.Id)
                {
                    rol.Name = role.Name;
                    return Task.FromResult(IdentityResult.Success);
                }
            }
            return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code="301", Description="role not found!" }));
        }
    }
}
