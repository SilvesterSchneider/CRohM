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
        /// <summary>
        /// Create a new role with all its containing claíms.
        /// </summary>
        /// <param name="role">the dto role object from front end</param>
        /// <returns>result</returns>
        Task<IdentityResult> CreateWithClaimsAsync(RoleDto role);

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="role">the new role to be created</param>
        /// <returns>result</returns>
        Task<IdentityResult> CreateAsync(Role role);

        /// <summary>
        /// Get all roles as list
        /// </summary>
        /// <returns>list with roles</returns>
        Task<List<Role>> GetAllRolesAsync();

        /// <summary>
        /// Get all roles with all its claíms as dto object list
        /// </summary>
        /// <returns>list with dto objects for front end</returns>
        Task<List<RoleDto>> GetAllRolesWithAllClaimsAsync();

        /// <summary>
        /// Delete a role
        /// </summary>
        /// <param name="role">the role to be deleted</param>
        /// <returns>result</returns>
        Task<IdentityResult> DeleteAsync(Role role);

        /// <summary>
        /// Update a role
        /// </summary>
        /// <param name="role">the role to be updated</param>
        /// <returns>result</returns>
        Task<IdentityResult> UpdateAsync(Role role);

        /// <summary>
        /// Update a role with all its claims
        /// </summary>
        /// <param name="role">the role dto object from front end</param>
        /// <returns>result</returns>
        Task<IdentityResult> UpdateRoleWithClaimsAsync(RoleDto role);

        /// <summary>
        /// Add a claim to a role
        /// </summary>
        /// <param name="role">role</param>
        /// <param name="claim">claim</param>
        /// <returns>result</returns>
        Task<IdentityResult> AddClaimAsync(Role role, Claim claim);

        /// <summary>
        /// Get all claims of a role
        /// </summary>
        /// <param name="role">role</param>
        /// <returns>list with all claims</returns>
        Task<IList<Claim>> GetClaimsAsync(Role role);

        /// <summary>
        /// Remove a claim from role
        /// </summary>
        /// <param name="role">role</param>
        /// <param name="claim">claim</param>
        /// <returns>result</returns>
        Task<IdentityResult> RemoveClaimAsync(Role role, Claim claim);

        /// <summary>
        /// Find a role by its name
        /// </summary>
        /// <param name="roleName">role name</param>
        /// <returns>role</returns>
        Task<Role> FindRoleByNameAsync(string roleName);

        /// <summary>
        /// Find a role by its id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>role</returns>
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
            if (await CheckIfRoleNameAlreadyExistsAsync(role.Name))
            {
                return IdentityResult.Failed(new IdentityError() { Code="401", Description="Role with same name already exists!" });
            }
            return await manager.CreateAsync(role);
        }

        public async Task<IdentityResult> CreateWithClaimsAsync(RoleDto role)
        {
            Role roleToCreate = new Role() { Id=0, Name=role.Name };
            IdentityResult result = await CreateAsync(roleToCreate);
            if (result == IdentityResult.Success)
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
            }
            return result;
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
            if ((role.Id == 1 && RoleClaims.GetAllAdminClaims().FirstOrDefault(x => x.Type.Equals(claim.Type)) != null &&
                RoleClaims.GetAllAdminClaims().FirstOrDefault(x => x.Value.Equals(claim.Value)) != null) ||
                role.Id == 2 && RoleClaims.GetAllDsgvoClaims().FirstOrDefault(x => x.Type.Equals(claim.Type)) != null &&
                RoleClaims.GetAllDsgvoClaims().FirstOrDefault(x => x.Value.Equals(claim.Value)) != null)
            {
                return IdentityResult.Success;
            }
            return await manager.RemoveClaimAsync(role, claim);
        }

        public async Task<IdentityResult> UpdateAsync(Role role)
        {
            return await manager.UpdateAsync(role);
        }

        private async Task<bool> CheckIfRoleNameAlreadyExistsAsync(string roleName)
        {
            List<Role> allAvailableRoles = await GetAllRolesAsync();
            if (allAvailableRoles.FirstOrDefault(x => x.Name.Equals(roleName)) == null)
            {
                return false;
            }
            return true;
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
                if (!await CheckIfRoleNameAlreadyExistsAsync(role.Name))
                {
                    roleToUpdate.Name = role.Name;
                    await UpdateAsync(roleToUpdate);
                }
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
