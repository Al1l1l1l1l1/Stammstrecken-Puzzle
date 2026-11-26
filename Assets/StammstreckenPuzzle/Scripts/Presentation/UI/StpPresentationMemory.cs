using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace STP.Presentation.UI
{
    /// <summary>Optional local stage directions. Never consulted by the puzzle or the economy.</summary>
    public sealed class StpPresentationMemory
    {
        [Serializable] public sealed class Entries
        {
            public int version = 1;
            public int deadlineShiftYears, menuVisits, lastRareGagVisit;
            public string lastCircularDate = "";
            public List<string> readMailIds = new List<string>();
            public List<string> firedTriggers = new List<string>();
            public List<string> circularIds = new List<string>();
            public List<int> tutorialStepsDone = new List<int>();
        }

        private readonly string file;
        private bool closed;
        public Entries State {get; private set;} = new Entries();
        public StpPresentationMemory(string directory)
        {
            file=Path.Combine(directory,"presentation-v1.json");
            try
            {
                if(!File.Exists(file))return;
                var data=JsonUtility.FromJson<Entries>(File.ReadAllText(file));
                if(data==null||data.version!=1||data.menuVisits<0||data.deadlineShiftYears<0)return;
                data.readMailIds??=new List<string>();data.firedTriggers??=new List<string>();
                data.circularIds??=new List<string>();data.tutorialStepsDone??=new List<int>();
                State=data;
            }
            catch(IOException){}catch(ArgumentException){}catch(UnauthorizedAccessException){}
        }
        public bool Fire(string trigger)
        {
            if(closed||State.firedTriggers.Contains(trigger))return false;
            State.firedTriggers.Add(trigger);Save();return true;
        }
        public void ConfirmReset(){if(closed)return;State.deadlineShiftYears+=2;Fire("FIRST_RESET");Save();}
        public void CompleteTutorial(int step){if(closed||State.tutorialStepsDone.Contains(step))return;State.tutorialStepsDone.Add(step);Save();}
        public void Read(string id){if(closed||State.readMailIds.Contains(id))return;State.readMailIds.Add(id);Save();}
        public bool Visit()
        {
            if(closed)return false;
            State.menuVisits++;
            bool rare=State.menuVisits-State.lastRareGagVisit>=5 && UnityEngine.Random.value<.16f;
            if(rare)State.lastRareGagVisit=State.menuVisits;
            Save();return rare;
        }
        public void Circular(string day,string id)
        {
            if(closed||State.lastCircularDate==day)return;
            State.lastCircularDate=day;State.circularIds.Add(id);Save();
        }
        public void Delete()
        {
            closed=true;
            if(File.Exists(file+".tmp"))File.Delete(file+".tmp");
            if(File.Exists(file))File.Delete(file);State=new Entries();
        }
        private void Save()
        {
            if(closed)return;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file)!);
                File.WriteAllText(file+".tmp",JsonUtility.ToJson(State),new UTF8Encoding(false));
                if(File.Exists(file))File.Replace(file+".tmp",file,null);else File.Move(file+".tmp",file);
            }
            catch(IOException){}catch(UnauthorizedAccessException){}
        }
    }
}
