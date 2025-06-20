using System;
using System.Collections;
using System.Collections.Generic;
using Aveva.Core.Database;
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
        public Hashtable ByDates(string refno, string date1, string date2) 
        {
            DbElement dbElement = GetDbElement(refno);
            return CompareByDates(dbElement, date1, date2);
        }

        [PMLNetCallable]
        public Hashtable ByDates(string date1, string date2)
        {
            DbElement dbElement = CurrentElement.Element;
            return CompareByDates(dbElement, date1, date2);
        }

        [PMLNetCallable]
        public Hashtable BySessions(string refno, double sess1, double sess2)
        {
            DbElement dbElement = GetDbElement(refno);
            return CompareBySessions(dbElement, sess1, sess2);
        }

        [PMLNetCallable]
        public Hashtable BySessions(double sess1, double sess2)
        {
            DbElement dbElement = CurrentElement.Element;
            return CompareBySessions(dbElement, sess1, sess2);
        }

        private Hashtable CompareByDates(DbElement dbElement, string date1, string date2)
        {
            Db db = dbElement.Db;

            DbSession dbSessionBase = GetSessionByDateString(db, date1);
            DbSession dbSessionTarget = GetSessionByDateString(db, date2);

            Hashtable results = CompareToArray(dbElement, dbSessionBase, dbSessionTarget);

            if (db.IsSwitched())
            {
                db.SwitchBackSession(true);
            }

            return results;
        }

        private Hashtable CompareBySessions(DbElement dbElement, double sess1, double sess2)
        {
            Db db = dbElement.Db;

            DbSession dbSessionBase = GetSessionByNumber(db, (int)sess1);
            DbSession dbSessionTarget = GetSessionByNumber(db, (int)sess2);

            Hashtable results = CompareToArray(dbElement, dbSessionBase, dbSessionTarget);

            if (db.IsSwitched())
            {
                db.SwitchBackSession(true);
            }

            return results;
        }

        private void ConsoleOutput(Hashtable compareResults)
        {
            foreach (Hashtable row in compareResults.Values)
            {
                string attName = (string)row[1];
                string attDesc = (string)row[2];
                string baseValue = (string)row[3];
                string targetValue = (string)row[4];

                Console.WriteLine($"{attName}: '{baseValue}' --- '{targetValue}'");
            }
        }

        private Hashtable CompareToArray(DbElement dbElement, DbSession baseSession, DbSession targetSession)
        {
            Hashtable results = new Hashtable();

            DbAttribute[] atts;
            dbElement.AllAttributesChangedBetween(baseSession, targetSession, out atts);

            baseAttributes = GetAttributesValues(dbElement, atts, baseSession);
            targetAttributes = GetAttributesValues(dbElement, atts, targetSession);

            for (int i = 0; i < atts.Length; i++)
            {
                Hashtable row = new Hashtable();
                string attKey = atts[i].Name;
                string attDesc = atts[i].Description;
                row[1] = attKey;
                row[2] = attDesc;
                row[3] = baseAttributes[attKey].Value;
                row[4] = targetAttributes[attKey].Value;
                results[i + 1] = row;
            }

            return results;
        }

        private DbSession GetSessionByDateString(Db db, string dateStr)
        {
            DateTime dt;
            DateTime.TryParse(dateStr, out dt);
            dt = dt.ToUniversalTime().ToLocalTime();
            
            DbSession dbSession = db.CurrentSession;

            try
            {
                dbSession = db.SessionBeforeDate(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dbSession;
        }

        private DbSession GetSessionByNumber(Db db, int number)
        {
            DbSession dbSession = db.CurrentSession;

            try
            {
                dbSession = db.Session(number);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dbSession;
        }

        private DbElement GetDbElement(string refNo)
        {
            DbElement dbElement = DbElement.GetElement(refNo);
            return dbElement;
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
