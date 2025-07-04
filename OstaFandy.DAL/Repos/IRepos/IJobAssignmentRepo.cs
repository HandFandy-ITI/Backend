using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OstaFandy.DAL.Entities;

namespace OstaFandy.DAL.Repos.IRepos
{
    public interface IJobAssignmentRepo : IGeneralRepo<JobAssignment>  
    {
        int gethandymanbyjobid(int id);

        int GetJobIdByNotificationId(int notificationId);
    }
}
