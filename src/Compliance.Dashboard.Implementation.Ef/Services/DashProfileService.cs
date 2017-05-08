using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compliance.Common.GenericRepo.Interfaces;
using Compliance.Dashboard.Domain;
using Compliance.Dashboard.Domain.Service;
using Compliance.Dashboard.Domain.ValueTypes;

namespace Compliance.Dashboard.Implementation.Ef.Services
{
    public class DashProfileService : IDashProfileService
    {
        private IGenericRepo<DashProfile, DashboardContext> _profileRepo;

        public DashProfileService(IGenericRepo<DashProfile, DashboardContext> profileRepo)
        {
            _profileRepo = profileRepo;
        }

        public DashProfile GetById(Guid profileId)
        {
            return _profileRepo.GetById(profileId);
        }

        public DashProfile GetByUserId(Guid userId)
        {
            return _profileRepo.Query(p=>p.UserId==userId).FirstOrDefault();
        }
        
        public DashProfile GetByEmail(EmailAddress email)
        {
            var theProfile = _profileRepo.Query(p => p.Email.Username == email.Username && p.Email.Domain == email.Domain);

            if (theProfile.Count() > 0)
                return theProfile.First();

            return null;
        }

        public DashProfile GetByEmail(string email)
        {
            return GetByEmail(new EmailAddress(email));
        }

        public void CreateProfile(DashProfile theProfile)
        {
            //Validation
            if (string.IsNullOrWhiteSpace(theProfile.FirstName))
                throw new Exception("Cannot create profile without first name.");
            
            _profileRepo.Add(theProfile);
            _profileRepo.Save();
        }

        public void UpdateProfile(DashProfile theProfile)
        {
            var profile = GetById(theProfile.Id);

            profile.FirstName = theProfile.FirstName;
            profile.LastName = theProfile.LastName;
            profile.CellNumber = theProfile.CellNumber;
            _profileRepo.Update(profile);
            _profileRepo.Save();
        }

        public void DeleteProfile(DashProfile theProfile)
        {
            var profile = GetById(theProfile.Id);
            profile.IsActive = false;
            _profileRepo.Update(profile);
            _profileRepo.Save();
        }

        public IEnumerable<DashProfile> GetAll()
        {
            return _profileRepo.GetAll();
        }
        
    }
}
