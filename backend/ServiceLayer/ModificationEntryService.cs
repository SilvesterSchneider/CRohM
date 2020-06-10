using ModelLayer;
using ModelLayer.Models;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public interface IModificationEntryService : IModificationEntryRepository
    {
        
    }

    public class ModificationEntryService : ModificationEntryRepository, IModificationEntryService
    {
        public ModificationEntryService(CrmContext context) : base(context)
        {

        } 
    }
}
