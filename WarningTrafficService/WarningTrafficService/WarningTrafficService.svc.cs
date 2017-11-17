using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WarningTrafficService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WarningTrafficService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WarningTrafficService.svc or WarningTrafficService.svc.cs at the Solution Explorer and start debugging.
    public class WarningTrafficService : IWarningTrafficService
    {
        WarningTrafficDataContext db = new WarningTrafficDataContext();

        public void AddWarning(Warning wn)
        {
            db.Warnings.InsertOnSubmit(wn);
            db.SubmitChanges();
        }

        public bool CheckEmail(string email)
        {
            var rt = db.Users.Where(u => u.email==email).FirstOrDefault();
            if (rt == null)
                return true;
            return false;
        }

        public void DeleteWarning(int idWn)
        {
            var wn = db.Warnings.Where(w => w.idWarning == idWn).FirstOrDefault();
            if (wn != null) {
                db.Warnings.DeleteOnSubmit(wn);
            }
        }

        public List<Law> GetAllLaw()
        {
            return db.Laws.ToList();
        }

        public List<Warning> GetAllWarning()
        {
            return db.Warnings.ToList();
        }

        public Law GetLawByID(int lawID)
        {
            return db.Laws.Where(l => l.idLaw==lawID).FirstOrDefault();
        }

        public Warning GetWarningByID(int idWarning)
        {
            return db.Warnings.Where(w => w.idWarning == idWarning).FirstOrDefault();
        }

        public User Login(string email, string password)
        {
            var user = db.Users.Where(u => u.email==email && u.password==password).FirstOrDefault();
            return user;
        }

        public void Register(User user)
        {
            db.Users.InsertOnSubmit(user);
            db.SubmitChanges();
        }

        public List<string> SuggestLaw(string text)
        {
            var listLaw = db.Laws.Where(l => (l.nameLaw.ToLower()).Contains(text.ToLower())).ToList();
            var listName = new List<string>();
            listLaw.ForEach(l => listName.Add(l.nameLaw));
            return listName;
        }

        public List<Law> SearchLawByName(string name)
        {
            return db.Laws.Where(l => (l.nameLaw.ToLower()).Contains((name.ToLower()))).ToList();
        }
    }
}
