using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Dashboard.Domain.ValueTypes;

namespace Compliance.Dashboard.Domain.Service
{
    public interface IDashProfileService
    {
        //Gets
        DashProfile GetById(Guid profileId);
        DashProfile GetByUserId(Guid userId);
        IEnumerable<DashProfile> GetAll();

        DashProfile GetByEmail(EmailAddress email);
        DashProfile GetByEmail(string email);
        
        void CreateProfile(DashProfile theProfile);
        void UpdateProfile(DashProfile theProfile);
        void DeleteProfile(DashProfile theProfile);
        //Deletes (Psuedo)
    }
}
