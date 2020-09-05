using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModelLayer.DataTransferObjects;
using ModelLayer.Helper;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public interface IRoleService
    {
        Task<IdentityResult> CreateWithClaimsAsync(RoleDto role);
        Task<IdentityResult> CreateAsync(Role role);
        Task<List<Role>> GetAllRolesAsync();
        Task<List<RoleDto>> GetAllRolesWithAllClaimsAsync();
        Task<IdentityResult> DeleteAsync(Role role);
        Task<IdentityResult> UpdateAsync(Role role);
        Task<IdentityResult> UpdateRoleWithClaimsAsync(RoleDto role);
        Task<IdentityResult> AddClaimAsync(Role role, Claim claim);
        Task<IList<Claim>> GetClaimsAsync(Role role);
        Task<IdentityResult> RemoveClaimAsync(Role role, Claim claim);
        Task<Role> FindRoleByNameAsync(string roleName);
        Task<Role> FindRoleByIdAsync(long id);
    }

    public class RoleService : IRoleService
    {
        private RoleManager<Role> manager;

        public RoleService(RoleManager<Role> manager)
        {
            this.manager = manager;
        }

        public async Task<IdentityResult> AddClaimAsync(Role role, Claim claim)
        {
            return await manager.AddClaimAsync(role, claim);
        }

        public async Task<IdentityResult> CreateAsync(Role role)
        {
            return await manager.CreateAsync(role);
        }

        public async Task<IdentityResult> CreateWithClaimsAsync(RoleDto role)
        {
            Role roleToCreate = new Role() { Id=0, Name=role.Name };
            foreach (Role existingRole in await GetAllRolesAsync())
            {
                if (existingRole.Name.Equals(roleToCreate.Name))
                {
                    return IdentityResult.Success;
                }
            }
            if (await CreateAsync(roleToCreate) == IdentityResult.Success)
            {
                Role createdRole = await FindRoleByNameAsync(roleToCreate.Name);
                List<Claim> claimsToAdd = new List<Claim>();
                foreach (string claimToInsert in role.Claims)
                {
                    foreach (Claim existingClaim in RoleClaims.GetAllClaims())
                    {
                        if (existingClaim.Value.Equals(claimToInsert))
                        {
                            claimsToAdd.Add(existingClaim);
                        }
                    }
                }
                foreach (Claim newClaim in claimsToAdd)
                {
                    await AddClaimAsync(createdRole, newClaim);
                }
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed(new IdentityError() { Code="301", Description="Error creating a role" });
            }
        }

        public async Task<IdentityResult> DeleteAsync(Role role)
        {
            return await manager.DeleteAsync(role);
        }

        public async Task<Role> FindRoleByIdAsync(long id)
        {
            return await manager.Roles.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Role> FindRoleByNameAsync(string roleName)
        {
            return await manager.FindByNameAsync(roleName);
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await manager.Roles.ToListAsync();
        }

        public async Task<List<RoleDto>> GetAllRolesWithAllClaimsAsync()
        {
            List<RoleDto> list = new List<RoleDto>();
            foreach (Role role in await GetAllRolesAsync())
            {
                List<string> claims = new List<string>();
                foreach (Claim claim in await GetClaimsAsync(role))
                {
                    claims.Add(claim.Value);
                }
                list.Add(new RoleDto() { Id=role.Id, Name=role.Name, Claims=claims });
            }
            return list;
        }

        public async Task<IList<Claim>> GetClaimsAsync(Role role)
        {
            return await manager.GetClaimsAsync(role);
        }

        public async Task<IdentityResult> RemoveClaimAsync(Role role, Claim claim)
        {
            return await manager.RemoveClaimAsync(role, claim);
        }

        public async Task<IdentityResult> UpdateAsync(Role role)
        {
            return await manager.UpdateAsync(role);
        }

        public async Task<IdentityResult> UpdateRoleWithClaimsAsync(RoleDto role)
        {
            Role roleToUpdate = await FindRoleByIdAsync(role.Id);
            if (roleToUpdate == null)
            {
                return IdentityResult.Failed(new IdentityError() { Code="301", Description="Role not found!" });
            }
            if (!roleToUpdate.Name.Equals(role.Name))
            {
                roleToUpdate.Name = role.Name;
                await UpdateAsync(roleToUpdate);
            }
            
            List<Claim> claimsToDelete = new List<Claim>();
            IList<Claim> existingClaims = await GetClaimsAsync(roleToUpdate);
            foreach (Claim existingClaim in existingClaims)
            {
                if (role.Claims.FirstOrDefault(x => x.Equals(existingClaim.Value)) == null)
                {
                    claimsToDelete.Add(existingClaim);
                }
            }
            foreach (Claim claimToDelete in claimsToDelete)
            {
                await RemoveClaimAsync(roleToUpdate, claimToDelete);
            }
            List<Claim> claimsToAdd = new List<Claim>();
            bool error = false;
            foreach (string claimToAdd in role.Claims)
            {
                if (existingClaims.FirstOrDefault(x => x.Value.Equals(claimToAdd)) == null)
                {
                    Claim existingClaimToAdd = RoleClaims.GetAllClaims().FirstOrDefault(x => x.Value.Equals(claimToAdd));
                    if (existingClaimToAdd != null)
                    {
                        claimsToAdd.Add(existingClaimToAdd);
                    }
                    else
                    {
                        error = true;
                    }
                }
            }
            foreach (Claim claimToAdd in claimsToAdd)
            {
                await AddClaimAsync(roleToUpdate, claimToAdd);
            }
            if (!error)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed(new IdentityError() { Code="201", Description="Claim not found!" });
            }
        }
    }
}
