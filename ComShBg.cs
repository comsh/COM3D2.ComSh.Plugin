using System;
using System.Collections.Generic;

namespace COM3D2.ComSh.Plugin {
    // 非同期ジョブ
	public static class ComShBg {
        public static JobList cron=new JobList();
        public static JobList cron2=new JobList();

        public static void OnUpdate(){ cron.OnUpdate(); }
        public static void OnLateUpdate(){ cron2.OnUpdate(); }

        public delegate int JobAction(long t);
        public class Job{
            public string name;
            public long next;
            public int prio;
            private long ival;
            private long expire;
            private JobAction action;
            public JobAction destroy=null;
            public ComShInterpreter sh; // destoryをactionと同じシェルで実行するため
            public Job(string name,long iv,long life,JobAction a,int prio=0,bool immediate=false){
                this.name=name;
                this.prio=prio;
                action=a;
                ival=(iv==0?1:iv);
                next=DateTime.UtcNow.Ticks;
                if(life>0) expire=next+life; else expire=DateTime.MaxValue.Ticks+1;
                if(!immediate) next+=ival; // １回目の実行をival後にする
            }
            public int Update(long t){
                if(t<next) return 0;
                if(t>expire){ destroy?.Invoke(0); return -1;}
                int r=action(t);        
                if(r<0){ destroy?.Invoke(-1); return -1;} // 負を返せばそこで繰り返し終了
                next+=ival*(1+(t-next)/ival);
                return 0;
            }
        }
	    public class JobList {
            private  Dictionary<string,Job> jobs=new Dictionary<string,Job>(); // 名前引き
            private List<List<Job>> joblist=new List<List<Job>>(); // 処理順別に複数本
    
            public bool ContainsName(string name){
                return jobs.ContainsKey(name);
            }
            public Job Find(string name){
                if(jobs.TryGetValue(name,out Job j)) return j;
                return null;
            }
            public Job AddJob(string name,long iv,long life,JobAction a,int prio=0,bool immediate=false){
                if(jobs.ContainsKey(name)) return null;
                while(joblist.Count<=prio) joblist.Add(new List<Job>());
                Job j=new Job(name,iv,life,a,prio,immediate);
                jobs.Add(name,j);
                joblist[prio].Add(j);
                return j;
            }
            public bool ChangePriority(string name,int prio){ // 処理順変更
                if(!jobs.TryGetValue(name,out Job j)) return false;
                if(j.prio==prio) return true;
                while(joblist.Count<=prio) joblist.Add(new List<Job>());
                UTIL.RemoveFromList(joblist[j.prio],j);
                j.prio=prio;
                joblist[prio].Add(j);
                return true;
            }
            public void KillJob(string name,bool nodestroy=false){
                if(!jobs.TryGetValue(name,out Job j)) return;
                if(!nodestroy) j.destroy?.Invoke(-2);
                jobs.Remove(name);
                UTIL.RemoveFromList(joblist[j.prio],j);
            }
            public List<string> LsJob(string prefix){
                List<string> ret=new List<string>(jobs.Count);
                foreach(var job in jobs.Values)
                    if(prefix==""||job.name.StartsWith(prefix,StringComparison.Ordinal)) ret.Add(job.name);
                return ret;
            }
            public void OnUpdate(){
                var curTick=DateTime.UtcNow.Ticks;
                for(int j=0; j<joblist.Count; j++){
                    var lst=joblist[j];
                    for(int i=lst.Count-1; i>=0; i--){
                        if(lst[i].Update(curTick)<0){
                            jobs.Remove(lst[i].name);
                            UTIL.RemoveAtFromList(lst,i);
                        }
                    }
                }
            }
        }
    }
}
