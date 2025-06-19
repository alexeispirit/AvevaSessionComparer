using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aveva.Core.Database;
using Aveva.Core.Database.View;
using Aveva.Core.PMLNet;

namespace SessionCompare
{
    [PMLNetCallable]
    public class SessionCompare
    {
        private Dictionary<string, Attribute> baseAttributes = new Dictionary<string, Attribute>(); 
        private Dictionary<string, Attribute> targetAttributes = new Dictionary<string, Attribute>(); 

        [PMLNetCallable]
        public SessionCompare() { }

        [PMLNetCallable]
        public void Assign(SessionCompare that) { }

        [PMLNetCallable]
        public void Run(string date1, string date2)
        {
            DateTime dt1;
            DateTime dt2;

            DateTime.TryParse(date1, out dt1);
            DateTime.TryParse(date2, out dt2);

            dt1 = dt1.ToUniversalTime().ToLocalTime();
            dt2 = dt2.ToUniversalTime().ToLocalTime();

            DbElement ce = CurrentElement.Element;
            Db db = ce.Db;

            DbSession dbSessionBase = db.CurrentSession;
            DbSession dbSessionTarget = db.CurrentSession;

            try
            {
                dbSessionBase = db.SessionBeforeDate(dt1);
                dbSessionTarget = db.SessionBeforeDate(dt2);
            }
            catch(Exception ex) 
            { 
                Console.WriteLine(ex.Message);
            }

            DbAttribute[] atts;
            ce.AllAttributesChangedBetween(dbSessionBase, dbSessionTarget, out atts);

            baseAttributes = GetAttributesValues(ce, atts, dbSessionBase);
            targetAttributes = GetAttributesValues(ce, atts, dbSessionTarget);

            foreach (string attName in baseAttributes.Keys)
            {
                string baseValue = baseAttributes[attName].Value;
                string targetValue = targetAttributes[attName].Value;
                Console.WriteLine($"{attName}: {baseValue} ({dbSessionBase.SessionNumber}) {targetValue} ({dbSessionTarget.SessionNumber})");
            }

            db.SwitchToLatestSession(true);
        }

        private Dictionary<string, Attribute> GetAttributesValues(DbElement dbElement, DbAttribute[] atts, DbSession dbSession)
        {
            Dictionary<string, Attribute> result = new Dictionary<string, Attribute>();

            Db db = dbElement.Db;
            db.SwitchToOldSession(dbSession);

            foreach (DbAttribute att in atts)
            {
                Attribute attribute = new Attribute
                {
                    Name = att.Name,
                    Description = att.Description,
                    Session = dbSession.SessionNumber,
                    Value = dbElement.GetAsString(att)
                };
                result.Add(att.Name, attribute);
            }

            return result;
        }
    }
}
