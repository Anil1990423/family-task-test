using Core.Abstractions.Repositories;
using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer
{
    public class TaskRepository : BaseRepository<Guid, Tasks, TaskRepository>, ITaskRepository
    {
        public TaskRepository(FamilyTaskContext context) : base(context)
        { }

       

        ITaskRepository IBaseRepository<Guid, Tasks, ITaskRepository>.NoTrack()
        {
            return base.NoTrack();
        }

        ITaskRepository IBaseRepository<Guid, Tasks, ITaskRepository>.Reset()
        {
            return base.Reset();
        }

       
    }
}
