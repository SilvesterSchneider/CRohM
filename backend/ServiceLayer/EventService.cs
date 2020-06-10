using ModelLayer;
using RepositoryLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer
{
    public interface IEventService : IEventRepository
    {

    }

    public class EventService : EventRepository, IEventService
    {
        public EventService(CrmContext context, IEventContactRepository eventContactRepo, IContactRepository contactRepo) : base(context, eventContactRepo, contactRepo)
        { }
    }
}
