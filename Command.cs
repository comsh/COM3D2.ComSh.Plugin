using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static System.StringComparison;
using System.Text;

namespace COM3D2.ComSh.Plugin {

public static class Command {
    public delegate int Cmd(ComShInterpreter sh,List<string> args);
	private static Dictionary<string, Cmd> cmdTbl=null;

    public static Cmd GetCmd(string name){
        if(cmdTbl.TryGetValue(name,out Cmd code)) return code;
        return null;
    }
    public static int Invoke(ComShInterpreter sh,string cmd,List<string> args){
        Cmd code=GetCmd(cmd);
        if(code==null) return sh.io.Error("存在しないコマンドです");
        return code.Invoke(sh,args);
    }
    public static void AddCmd(string name,Cmd cmd){ cmdTbl.Add(name,cmd); }
    public static void Init(){
        if(cmdTbl!=null) return;
        cmdTbl=new Dictionary<string,Cmd>();
        cmdTbl.Add("fontsize",new Cmd(CmdFontsize));
        cmdTbl.Add("width",new Cmd(CmdWidth));
        cmdTbl.Add("height",new Cmd(CmdHeight));
		cmdTbl.Add("exit", new Cmd(CmdExit));
		cmdTbl.Add("return", new Cmd(CmdReturn));
		cmdTbl.Add("close", new Cmd(CmdClose));
		cmdTbl.Add("env", new Cmd(CmdEnv));
		cmdTbl.Add("echo", new Cmd(CmdEcho));
		cmdTbl.Add("log", new Cmd(CmdLog));
		cmdTbl.Add("source", new Cmd(CmdSource));
		cmdTbl.Add(".", new Cmd(CmdSource));
        cmdTbl.Add("eval",new Cmd(CmdEval));
        cmdTbl.Add("timer",new Cmd(CmdTimer));
		cmdTbl.Add("cb",new Cmd( CmdClipBd));
		cmdTbl.Add("test", new Cmd(CmdTest));
        cmdTbl.Add("cut",new Cmd(CmdCut));
        cmdTbl.Add("trim",new Cmd(CmdTrim));
        cmdTbl.Add("grep",new Cmd(CmdGrep));
        cmdTbl.Add("lookup",new Cmd(CmdLookup));
        cmdTbl.Add("line",new Cmd(CmdLine));
		cmdTbl.Add("select", new Cmd(CmdSelect));
		cmdTbl.Add("ls", new Cmd(CmdLS));
        cmdTbl.Add("cron",new Cmd(CmdCron));
        cmdTbl.Add("cron2",new Cmd(CmdCron2));
        cmdTbl.Add("expr",new Cmd(CmdExpr));
        cmdTbl.Add("ps",new Cmd(CmdPs));
        cmdTbl.Add("kill",new Cmd(CmdKill));
        cmdTbl.Add("cutloop",new Cmd(CmdCutLoop));
        cmdTbl.Add("lineloop",new Cmd(CmdLineLoop));
        cmdTbl.Add("regexloop",new Cmd(CmdRegexLoop));
        cmdTbl.Add("seqno",new Cmd(CmdSeqNo));
        cmdTbl.Add("refreshmypose",new Cmd(CmdRefreshMypose));
        cmdTbl.Add("sin",new Cmd(CmdSin));
        cmdTbl.Add("cos",new Cmd(CmdCos));
        cmdTbl.Add("rnd",new Cmd(CmdRnd));
        cmdTbl.Add("rndn",new Cmd(CmdRndN));
        cmdTbl.Add("sleep",new Cmd(CmdSleep));
        cmdTbl.Add("kvs",new Cmd(CmdKvs));
        cmdTbl.Add("kvs.save",new Cmd(CmdKvsSave));
        cmdTbl.Add("kvs.load",new Cmd(CmdKvsLoad));
        cmdTbl.Add("kvs.clear",new Cmd(CmdKvsClear));
        cmdTbl.Add("distance",new Cmd(CmdDistance));
        cmdTbl.Add("normalize", new Cmd(CmdNormalize));
        cmdTbl.Add("replace",new Cmd(CmdReplace));
        cmdTbl.Add("split",new Cmd(CmdSplit));
        cmdTbl.Add("crossproduct",new Cmd(CmdCrossProduct));
        cmdTbl.Add("dotproduct",new Cmd(CmdDotProduct));
        cmdTbl.Add("tag",new Cmd(CmdTag));
        cmdTbl.Add("suffix",new Cmd(CmdSuffix));
        cmdTbl.Add("prefix",new Cmd(CmdPrefix));
        cmdTbl.Add("wincolor",new Cmd(CmdWinColor));
        cmdTbl.Add("clamp",new Cmd(CmdClamp));
        cmdTbl.Add("unset",new Cmd(CmdUnset));
        cmdTbl.Add("cmp",new Cmd(CmdCmp));
        cmdTbl.Add("if",new Cmd(CmdTest));
        cmdTbl.Add("unless",new Cmd(CmdUnless));
        cmdTbl.Add("thenif",new Cmd(CmdThenIf));
        cmdTbl.Add("elseif",new Cmd(CmdElseIf));
        cmdTbl.Add("then",new Cmd(CmdThen));
        cmdTbl.Add("else",new Cmd(CmdElse));
        cmdTbl.Add("sed",new Cmd(CmdSed));
        cmdTbl.Add("meminfo",new Cmd(CmdMemInfo));
        cmdTbl.Add("escape",new Cmd(CmdEscape));
        cmdTbl.Add("sort",new Cmd(CmdSort));
        cmdTbl.Add("uniq",new Cmd(CmdUniq));
        cmdTbl.Add("tan",new Cmd(CmdTan));
        cmdTbl.Add("asin",new Cmd(CmdASin));
        cmdTbl.Add("acos",new Cmd(CmdACos));
        cmdTbl.Add("atan",new Cmd(CmdATan));
        cmdTbl.Add("atan2",new Cmd(CmdATan2));
        cmdTbl.Add("abs",new Cmd(CmdAbs));
        cmdTbl.Add("sqrt",new Cmd(CmdSqrt));
        cmdTbl.Add("max",new Cmd(CmdMax));
        cmdTbl.Add("min",new Cmd(CmdMin));
        cmdTbl.Add("nvl",new Cmd(CmdNvl));
        cmdTbl.Add("quat",new Cmd(CmdQuat));
        cmdTbl.Add("inside",new Cmd(CmdInside));
        cmdTbl.Add("substr",new Cmd(CmdSubstr));
        cmdTbl.Add("repeat",new Cmd(CmdRepeat));
        cmdTbl.Add("ref",new Cmd(CmdRefer));
        cmdTbl.Add("isref",new Cmd(CmdIsRef));
        cmdTbl.Add("static",new Cmd(CmdStatic));
        cmdTbl.Add("bind",new Cmd(CmdBind));
        cmdTbl.Add("anmlist", new Cmd(CmdAnmList));
        cmdTbl.Add("myposelist", new Cmd(CmdMyPoseList));
        cmdTbl.Add("dancelist", new Cmd(CmdDanceList));
        cmdTbl.Add("menulist", new Cmd(CmdMenuList));
        cmdTbl.Add("mytexlist", new Cmd(CmdMyTexList));
        cmdTbl.Add("cubemaplist", new Cmd(CmdCubemapList));
        cmdTbl.Add("scene", new Cmd(CmdScene));
        cmdTbl.Add("fps",new Cmd(CmdFps));
        cmdTbl.Add("vsync", new Cmd(CmdVSync));
        cmdTbl.Add("timescale", new Cmd(CmdTimeScale));
        cmdTbl.Add("count", new Cmd(CmdCount));
        cmdTbl.Add("system", new Cmd(CmdSystem));
        cmdTbl.Add("floor", new Cmd(CmdFloor));
        cmdTbl.Add("ceil", new Cmd(CmdCeil));
        cmdTbl.Add("truncate", new Cmd(CmdTruncate));
        cmdTbl.Add("round", new Cmd(CmdRound));
        cmdTbl.Add("round2", new Cmd(CmdRound2));
        cmdTbl.Add("roundup", new Cmd(CmdRoundUp));
        cmdTbl.Add("perlinnoise", new Cmd(CmdPerlinNoise));
        cmdTbl.Add("pixellights", new Cmd(CmdPixelLights));
        cmdTbl.Add("namespace", new Cmd(CmdNameSpace));
        cmdTbl.Add("namespace.clear", new Cmd(CmdNameSpaceClear));
        cmdTbl.Add("date", new Cmd(CmdDate));
        cmdTbl.Add("publish", new Cmd(CmdPublish));
        cmdTbl.Add("subscribe", new Cmd(CmdSubscribe));
        cmdTbl.Add("unsubscribe", new Cmd(CmdUnSubscribe));
        cmdTbl.Add("vars2str", new Cmd(CmdVars2Str));
        cmdTbl.Add("str2vars", new Cmd(CmdStr2Vars));
        cmdTbl.Add("rgb2hsv", new Cmd(CmdRgb2Hsv));
        cmdTbl.Add("hsv2rgb", new Cmd(CmdHsv2Rgb));
        cmdTbl.Add("indiv", new Cmd(CmdInDiv));
        cmdTbl.Add("clean", new Cmd(CmdClean));
        cmdTbl.Add("gc", new Cmd(CmdGC));
        cmdTbl.Add("evalkag", new Cmd(CmdEvalKag));
        cmdTbl.Add("execkag", new Cmd(CmdExecKag));
        cmdTbl.Add("logbx", new Cmd(CmdLogBX));
        cmdTbl.Add("pow", new Cmd(CmdPow));
        cmdTbl.Add("allhandleoff", new Cmd(CmdAllHandleClear));
        cmdTbl.Add("mouse", new Cmd(CmdMouse));
        cmdTbl.Add("keypress", new Cmd(CmdKeyPress));
        cmdTbl.Add("axis", new Cmd(CmdKeyAxis));
        cmdTbl.Add("smoothdamp", new Cmd(CmdSmoothDamp));
        cmdTbl.Add("layerhit", new Cmd(CmdLayerHit));
        cmdTbl.Add("physics", new Cmd(CmdPhysicsParam));
        cmdTbl.Add("findcollider", new Cmd(CmdFindCollider));
        cmdTbl.Add("outputlog", new Cmd(CmdOutputLog));
        cmdTbl.Add("skybox", new Cmd(CmdGlobalSkyBox));
        cmdTbl.Add("skybox.color", new Cmd(CmdGlobalSkyBoxColor));
        cmdTbl.Add("skybox.power", new Cmd(CmdGlobalSkyBoxPower));
        cmdTbl.Add("skybox.rotation", new Cmd(CmdGlobalSkyBoxRotation));
        cmdTbl.Add("numformat", new Cmd(CmdNumFormat));
        cmdTbl.Add("tohex", new Cmd(CmdToHex));
        cmdTbl.Add("todec", new Cmd(CmdToDec));

        cmdTbl.Add("__res",new Cmd(Cmd__Resource));
        cmdTbl.Add("__files",new Cmd(Cmd__Files));

        CmdGui.Init();
        CmdMaidMan.Init();
        CmdBones.Init();
        CmdObjects.Init();
        CmdLights.Init();
        CmdMeshes.Init();
        CmdCamera.Init();
        CmdSubCamera.Init();
        CmdMisc.Init();
    }

    private static int Cmd__Resource(ComShInterpreter sh,List<string> args){ // 調査専用
        if(args.Count==2){
            UnityEngine.Object[] r=Resources.LoadAll(args[1]);
            foreach (var t in r) Debug.Log(t.name);
        }else if(args.Count==3){
            if(args[2]=="text"){
                UnityEngine.Object[] r=Resources.LoadAll(args[1],typeof(TextAsset));
                foreach (var t in r) Debug.Log(t.name);
            }else if(args[2]=="obj"){
                UnityEngine.Object[] r=Resources.LoadAll(args[1],typeof(GameObject));
                foreach (var t in r) Debug.Log(t.name);
            }
        }
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        return 0;
    }
    private static int Cmd__Files(ComShInterpreter sh,List<string> args){ // 調査用
        if(args.Count!=2) return sh.io.Error("使い方: __files 拡張子");
        var files=new List<string>();
        if(args[1]=="anm"){
            string[] fa=GameUty.FileSystem.GetList("motion",AFileSystemBase.ListType.AllFile);
            for(int i=0; i<fa.Length; i++) if(fa[i].EndsWith(".anm",Ordinal)) files.Add(fa[i]);
            fa=GameUty.FileSystemOld.GetList("motion",AFileSystemBase.ListType.AllFile);
            for(int i=0; i<fa.Length; i++) if(fa[i].EndsWith(".anm",Ordinal)) files.Add(fa[i]);
        }else if(args[1]=="menu"){
            string[] fa=GameUty.FileSystem.GetList("parts",AFileSystemBase.ListType.AllFile);
            for(int i=0; i<fa.Length; i++) if(fa[i].EndsWith(".menu",Ordinal)) files.Add(fa[i]);
            fa=GameUty.FileSystemOld.GetList("parts",AFileSystemBase.ListType.AllFile);
            for(int i=0; i<fa.Length; i++) if(fa[i].EndsWith(".menu",Ordinal)) files.Add(fa[i]);
        }else if(args[1]=="ks"){
            string[] fa=GameUty.FileSystem.GetList("script/motion/m_sex",AFileSystemBase.ListType.AllFile);
            for(int i=0; i<fa.Length; i++) if(fa[i].EndsWith(".ks",Ordinal)) files.Add(fa[i]);
            fa=GameUty.FileSystemOld.GetList("script/motion/m_sex",AFileSystemBase.ListType.AllFile);
            for(int i=0; i<fa.Length; i++) if(fa[i].EndsWith(".ks",Ordinal)) files.Add(fa[i]);
            fa=GameUty.FileSystem.GetList("script/motion/m_common",AFileSystemBase.ListType.AllFile);
            for(int i=0; i<fa.Length; i++) if(fa[i].EndsWith(".ks",Ordinal)) files.Add(fa[i]);
            fa=GameUty.FileSystemOld.GetList("script/motion/m_common",AFileSystemBase.ListType.AllFile);
            for(int i=0; i<fa.Length; i++) if(fa[i].EndsWith(".ks",Ordinal)) files.Add(fa[i]);
        }else{
            string[] fa=GameUty.FileSystem.GetFileListAtExtension(args[1]);
            files.AddRange(fa);
            fa=GameUty.FileSystemOld.GetFileListAtExtension(args[1]);
            files.AddRange(fa);
        }
        if(files.Count==0) return 0;
        files.Sort();
        string prev="";
        foreach(string fn in files) if(fn!=prev){ Debug.Log(fn); prev=fn; }
        System.GC.Collect();
        return 0;
    }
    private class DanceDt {
        public string ogg;
        public string scene;
        public string name;
        public StringBuilder anm=new StringBuilder();
        public StringBuilder kp=new StringBuilder();
        public int n;
        public void SetAnm(List<string>str,int cnt){SetSb(anm,str,cnt,".anm");}
        public void SetKp(List<string>str,int cnt){SetSb(kp,str,cnt,"");}
        private void SetSb(StringBuilder sb,List<string> str,int cnt,string sfx){
            sb.Length=0;
            if(str==null||str.Count==0) return;
            sb.Append(UTIL.Suffix2(str[0],sfx));
            for(int i=1; i<cnt; i++){
                sb.Append(',');
                if(str.Count>i){
                    sb.Append(UTIL.Suffix2(str[i],sfx));
                }
            }
        }
    };
    private static int CmdDanceList(ComShInterpreter sh,List<string> args){
        long dur=30;    // シーン読み込みタイムアウト デフォルト30秒
        if(args.Count==2){
            if(!long.TryParse(args[1],out dur)||dur<=0) return sh.io.Error("数値が不正です");
        }
        dur*=1000*TimeSpan.TicksPerMillisecond;
        var nei=DanceSelect.GetDanceDataList();
        if(nei.Count==0) return 0;
        var result=new List<DanceDt>(nei.Count);
        foreach(var dd in nei){
            DanceDt dt=new DanceDt();
            dt.ogg=UTIL.Suffix2(dd.bgm_file_name,".ogg");
            if(dt.ogg!="") if(!GameUty.IsExistFile(dt.ogg,GameUty.FileSystem)) continue;
            dt.scene=dd.scene_name; dt.name=dd.title; dt.n=dd.select_chara_num;
            dt.SetAnm(dd.motionFileList,dt.n); dt.SetKp(dd.kuchiPakuFileList,dt.n);
            result.Add(dt);
        }
        nei=null;
        string scene0=GameMain.Instance.GetNowSceneName();
        bool bak=NDebug.m_bQuitWhenAssert;
        NDebug.m_bQuitWhenAssert=false;     // Assertでアプリが落ちないように
        int idx=0;
        for(;idx<result.Count; idx++){
            try{GameMain.Instance.LoadScene(result[idx].scene);}catch{continue;}
            break;
        }
        if(idx==result.Count){ export(result); NDebug.m_bQuitWhenAssert=bak; return 0; }
        long timeout=0;
        ComShBg.cron.AddJob("__dance",0,0,(t)=>{
            if(timeout==0) timeout=t+dur;
            bool done=false;
            UnityEngine.SceneManagement.Scene sc=UnityEngine.SceneManagement.SceneManager.GetSceneByName(result[idx].scene);
            if(!sc.IsValid()) timeout=0;
            else if(sc.isLoaded && sc.name==result[idx].scene){
                var oa=sc.GetRootGameObjects();
                DanceMain dm=null;
                for(int i=0; i<oa.Length; i++){
                    var ca=oa[i].GetComponentsInChildren<DanceMain>(true);
                    if(ca!=null && ca.Length>0){ dm=ca[0]; break; }
                }
                if(dm!=null){
                    string ogg=result[idx].ogg;
                    if(ogg==""){
                        ogg=dm.m_strMasterAudioFileName;
                        if(ogg==null || (ogg!="" && !GameUty.IsExistFile(ogg,GameUty.FileSystem))) ogg="";
                        result[idx].ogg=ogg;
                    }
                    if(ogg!=""){ 
                        var dt=result[idx];
                        if(dt.anm.Length<dt.n && dm.m_listAnimName!=null) dt.SetAnm(dm.m_listAnimName,dt.n);
                        if(dt.kp.Length<dt.n && dm.m_listKuchiPakuFile!=null) dt.SetKp(dm.m_listKuchiPakuFile,dt.n);
                    }
                    done=true;
                }
            }
            if(done || t>timeout){
                if(!done) result[idx].ogg="";
                for(idx++;idx<result.Count; idx++){
                    try{GameMain.Instance.LoadScene(result[idx].scene);}catch{continue;}
                    break;
                }
                if(idx==result.Count){
                    export(result);
                    try{GameMain.Instance.LoadScene(scene0);}catch{}
                    Resources.UnloadUnusedAssets();
                    NDebug.m_bQuitWhenAssert=bak;
                    return -1;
                }
                timeout=t+dur;
            }
            return 0;
        });
        return 0;
        void export(List<DanceDt> data){
            string datadir=ComShInterpreter.scriptFolder+"export\\";
            Directory.CreateDirectory(datadir);
            try{
                using(StreamWriter sw=new StreamWriter(datadir+"dancelist",false,Encoding.UTF8)){
                    sw.WriteLine("dancelist={\\");
                    string last="";
                    foreach(var dt in data){
                        if(dt.ogg=="") continue;
                        string cur=$"{dt.n.ToString()}\t{dt.ogg}\t{dt.anm.ToString()}\t{dt.kp.ToString()}";
                        if(cur==last) continue;
                        sw.WriteLine(dt.name+"\t"+cur+"}\"\\n\"{\\");
                        last=cur;
                    }
                    sw.WriteLine("}");
                }
            }catch{}
        }
    }
    private static int CmdScene(ComShInterpreter sh,List<string> args){
        if(args.Count==1 || args[1]==""){
            UnityEngine.SceneManagement.Scene sc=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if(sc.IsValid()) sh.io.PrintJoin(sh.ofs,sc.name,sc.path,sc.buildIndex.ToString());
            return 0;
        }
        if(int.TryParse(args[1],out int id))
            try{ UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(id);}catch{}
        else
            try{ UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(args[1]);}catch{}
        return 0;
    }

    private static bool IsCommonByFilename(string name){
        return name.StartsWith("kaiwa",Ordinal)
            || name.StartsWith("h_kaiwa",Ordinal)
            || name.StartsWith("gm_",Ordinal)
            || name.StartsWith("event",Ordinal)
            || name.StartsWith("edit_pose",Ordinal);
    }
    private static bool IsCommonByDirnameCOM(string name){
        return name.Contains("\\common\\")
            || name.Contains("\\dance_mc\\");
    }
    private static bool IsCommonByDirnameCM(string name){
        return name.Contains("\\common\\")
            || name.Contains("\\event")
            || name.Contains("\\vr_event\\")
            || name.Contains("\\hanyou\\")
            || name.Contains("\\work\\")
            || name.Contains("\\_maid\\")
            || name.Contains("\\_man\\");
    }

    private static int CmdAnmList(ComShInterpreter sh,List<string> args){
        var files_sex=new List<string>();
        var files_dance=new List<string>();
        var files_common=new List<string>();
        string[] fa=GameUty.FileSystem.GetFileListAtExtension("anm");
        string name;
        for(int i=0; i<fa.Length; i++) if(fa[i].EndsWith(".anm",Ordinal)){
            name=Last2(fa[i],'\\');
            if(name==null){     //フォルダ分けされてないやつ。手がかりないのでテキトーにやるしかない
                string fn=Path.GetFileNameWithoutExtension(fa[i]);
                if(fn==null||fn=="") continue;
                if(fn.Contains("crc_")||fn.Contains("cbl_")) continue;
                if(IsCommonByFilename(fn)) files_common.Add(fn);
                if(fn.StartsWith("dance",Ordinal) && !fn.StartsWith("dance_mc")) files_dance.Add(fn);
                continue;
            }
            if(name.Contains("\\crc_")) continue;
            if(name.Contains("\\cbl_")) continue;
            if(fa[i].Contains("\\sex\\")) files_sex.Add(name);
            else if(fa[i].Contains("\\dance\\")) files_dance.Add(Path.GetFileNameWithoutExtension(name));
            else if(IsCommonByDirnameCOM(fa[i])) files_common.Add(Path.GetFileNameWithoutExtension(name));
        }
        fa=GameUty.FileSystemOld.GetList("motion",AFileSystemBase.ListType.AllFile);
        for(int i=0; i<fa.Length; i++) if(fa[i].EndsWith(".anm",Ordinal)){
            name=Last2(fa[i],'\\');
            if(name==null) continue;
            if(name.Contains("\\crc_")) continue;
            if(name.Contains("\\cbl_")) continue;
            if(fa[i].Contains("\\sex\\")) files_sex.Add(name);
            else if(fa[i].Contains("\\dance\\")) files_dance.Add(Path.GetFileNameWithoutExtension(name));
            else if(IsCommonByDirnameCM(fa[i])) files_common.Add(Path.GetFileNameWithoutExtension(name));
        }

        if(files_sex.Count>0){
            files_sex.Sort();
            if(WriteFileList("anmlist_sex",files_sex)<0) return sh.io.Error("ファイルが作成できません");
        }
        if(files_dance.Count>0){
            files_dance.Sort();
            if(WriteFileList("anmlist_dance",files_dance)<0) return sh.io.Error("ファイルが作成できません");
        }
        if(files_common.Count>0){
            files_common.Sort();
            if(WriteFileList("anmlist_common",files_common)<0) return sh.io.Error("ファイルが作成できません");
        }
        return 0;
    }
    private static int WriteFileList(string filename,List<string> files){ return WriteFileList2(filename,files,"","",""); }
    private static int WriteFileList2(string filename,List<string> files,string eol,string begin,string end){
        try{
            string datadir=ComShInterpreter.scriptFolder+"export\\";
            Directory.CreateDirectory(datadir);
            using(StreamWriter sw=new StreamWriter(datadir+filename,false,Encoding.UTF8)){
                sw.Write(begin);
                string prev="";
                foreach(string fn in files) if(fn!=prev){
                    string[] sa=fn.Split('\\');
                    prev=fn;
                    if(sa.Length==1) sw.WriteLine(sa[0]+eol);
                    else sw.WriteLine($"{sa[0]}\\{sa[1]}{eol}");
                }
                sw.Write(end);
            }
        }catch{
            System.GC.Collect();
            return -1;
        }
        System.GC.Collect();
        return 0;
    }
    private static string Last1(string s,char c){
        int i=s.LastIndexOf(c);
        if(i<=0||i==s.Length-1) return null;
        return s.Substring(i+1);
    }
    private static string Last2(string s,char c){
        int i=s.LastIndexOf(c);
        if(i<=0||i==s.Length-1) return null;
        i=s.LastIndexOf(c,i-1);
        if(i<=0||i>=s.Length-1) return null;
        return s.Substring(i+1);
    }
    private static int CmdMyPoseList(ComShInterpreter sh,List<string> args){
        var files=new List<string>();
        string[] fns=Directory.GetFiles(ComShInterpreter.myposeDir,"*.anm",SearchOption.AllDirectories);
        if(fns==null) return 0;
        int len=ComShInterpreter.myposeDir.Length;
        for(int i=0; i<fns.Length; i++) sh.io.PrintLn(fns[i].Substring(len).Replace("\\","/"));
        return 0;
    }
    private static int CmdMyTexList(ComShInterpreter sh,List<string> args){
        string path="";
        if(args.Count==2) path=args[1];
        var files=new List<string>();
        string[] fns=Directory.GetFiles(ComShInterpreter.textureDir+path,"*.png",SearchOption.AllDirectories);
        if(fns==null) return 0;
        int len=ComShInterpreter.textureDir.Length;
        for(int i=0; i<fns.Length; i++) sh.io.PrintLn(fns[i].Substring(len).Replace("\\","/"));
        return 0;
    }
    private static int CmdCubemapList(ComShInterpreter sh,List<string> args){
        string path="";
        if(args.Count==2) path=args[1];
        var files=new List<string>();
        try{
            string[] fns=Directory.GetFiles(ComShInterpreter.textureDir+path,"*.asset_bg",SearchOption.AllDirectories);
            if(fns==null) return 0;
            int len=ComShInterpreter.textureDir.Length;
            for(int i=0; i<fns.Length; i++){
                var cma=ObjUtil.ListAssetBundle_NoChk<Cubemap>(fns[i]);
                if(cma==null||cma.Count==0) continue;
                string ab=fns[i].Substring(len).Replace("\\","/");
                if(ab.EndsWith(".asset_bg")) ab=ab.Substring(0,ab.Length-9);
                foreach(var cm in cma) sh.io.PrintLn(ab+":"+cm);
            }
        }catch{ return 0; };
        return 0;
    }
    private static int CmdMenuList(ComShInterpreter sh,List<string> args){
        var dup=new HashSet<string>();
        var list=new List<string>();
        string[] fa;
        try{
            fa=Directory.GetFiles(ComShInterpreter.homeDir+@"Mod\","*.menu",SearchOption.AllDirectories);
            Add2MenuList(fa,list,dup);
        }catch{}
        fa=GameUty.FileSystem.GetFileListAtExtension("menu");
        Add2MenuList(fa,list,dup);
        fa=GameUty.FileSystemOld.GetList("menu",AFileSystemBase.ListType.AllFile);
        Add2MenuList(fa,list,dup);
        if(list.Count>0){
            list.Sort();
            if(WriteFileList2("menulist",list,"\\n\\","menulist=\"\\","\"\n")<0) return sh.io.Error("ファイルが作成できません");
        }
        return 0;
    }
    private static void Add2MenuList(string[] files,List<string> result,HashSet<string> hs){
        for(int i=0; i<files.Length; i++) if(files[i].EndsWith(".menu",Ordinal)){
            if(files[i].Contains("\\man\\")) continue;
            string name=Last1(files[i],'\\').ToLower();
            if(name==null) continue;
            if(name.Contains("_zurashi")||name.Contains("_mekure")||name.Contains("_mekure_back")||name.Contains("_porori")) continue;
            var mh=MaidUtil.ReadMenuHeader(name);
            if(mh==null) continue;
            var cate=mh.cate.ToLower();
            if(cate=="def" ||cate=="moza"||cate=="seieki_hara"||cate.StartsWith("set_",Ordinal)||cate.Contains("color")||cate.Contains("folder")) continue;
            if(cate=="body"||cate.Contains("skin")||cate=="head"||cate=="eye"||cate=="eyewhite"||cate=="mayu"
             ||cate=="matsuge_up"||mh.cate=="matsuge_low"||mh.cate=="futae"||cate=="underhair"||cate=="chikubi") continue;
            if(mh.name=="無し"||mh.name=="なし"){
                if(name=="_i_handiteml_del.menu") mh.name="handitemL無し";
                if(name=="_i_handitemr_del.menu") mh.name="handitemR無し";
                else mh.name=cate+mh.name;
            }
            if(name.Contains("_del_")||name.EndsWith("_del.menu")) mh.name=" "+mh.name;
            name=Path.GetFileNameWithoutExtension(name);
            if(mh.name=="") mh.name=name;
            if(hs.Add(name)) result.Add(mh.name+"\t"+cate+"\t"+name);
        }
    }
	private static int CmdExit(ComShInterpreter sh,List<string> args){
        sh.exitq=true; 
        return CmdReturn(sh,args);
    }
	private static int CmdReturn(ComShInterpreter sh,List<string> args){
        if(args.Count==2 && int.TryParse(args[1],out int n)) sh.io.exitStatus=n;
        else sh.io.exitStatus=0;
        return 0;
    }
	private static int CmdClose(ComShInterpreter sh,List<string> args){
        if(args.Count==1) ComShWM.HideTerm();
        for(int i=1; i<args.Count; i++){
            if(args[i]=="t") ComShWM.HideTerm();
            else if(args[i]=="m") ComShWM.HideMenu();
            else return sh.io.Error("tかmのいずれかを指定してください");
        }
        return 0;
    }
    private static int CmdClipBd(ComShInterpreter sh,List<string>args){
        if(args.Count>1){
            var sb=new StringBuilder();
            sb.Append(args[1]);
            for(int i=2; i<args.Count; i++) sb.Append(' ').Append(args[i]);
            sb.Append('\n');
			GUIUtility.systemCopyBuffer=sb.ToString();
        }else GUIUtility.systemCopyBuffer=ComShWM.terminal.GetLog();
        return 0;
    }
    private static int CmdTimer(ComShInterpreter sh,List<string>args){
        if(args.Count==1) foreach(string name in ComShBg.cron.LsJob("timer/")) sh.io.PrintLn(name);
        if(args.Count!=3) return sh.io.Error("使い方: timer 待機時間(ms) 待機後に実行するコマンド");
        if(!double.TryParse(args[1],out double ms)||ms<=0) return sh.io.Error("数値の指定が不正です");
        var psr=EvalParser(sh,2);
        if(psr==null) return -1;
        psr.Reset();
        if(ComShBg.cron.AddJob("timer/"+UTIL.GetSeqId(),(long)(ms*TimeSpan.TicksPerMillisecond),0,(long t)=>{
            sh.InterpretParser(psr);
            sh.exitq=false; // 元のシェルは終わらせない
            return -1;  // 負を返せば１回だけの実行で終わる
        })==null) return sh.io.Error("タイマー登録に失敗しました");
        return 0;
    }
    public static ComShParser EvalParser(ComShInterpreter sh,int idx,bool cannotempty=true,int lno=-1,string cmd=null){
        ComShParser.Statement st=sh.currentParser.currentStatement;
        var cmdTkn=st.tokens[st.offset+idx];
        ComShParser psr;
        if(cmdTkn.parser==null){
            psr=new ComShParser(lno>=0?lno:sh.currentParser.lineno);
            int r=psr.Parse(cmd!=null?cmd:cmdTkn.txt);
            if(r<0){ sh.io.Error(psr); return null; }
            if(r==0 && cannotempty){ sh.io.Error("コマンドが空です"); return null; }   
            if(cmdTkn.varies==0) cmdTkn.parser=psr;
        }else psr=cmdTkn.parser;
        return psr;
    }
    private static int CmdFontsize(ComShInterpreter sh,List<string> args){
        if(!sh.interactiveq) return 0;
        if(args.Count==1){
            sh.io.Print($"{ComShProperties.fontSize}\n");
            return 0;
        }
        if(args.Count>2) return sh.io.Error("使い方: fontsize フォントサイズ(数値)");
        if(!int.TryParse(args[1],out int n)||n<6) return sh.io.Error("フォントサイズは数値(6～)で指定してください");
        ComShProperties.fontSize=n;
        PanelStyleCache.Dirty();
        return 0;
    }
    private static int CmdWidth(ComShInterpreter sh,List<string> args){
        if(!sh.interactiveq) return 0;
        if(args.Count==1){
            sh.io.Print($"{ComShProperties.width}\n");
            return 0;
        }
        if(args.Count>2) return sh.io.Error("使い方: width ターミナルの横幅(ピクセル)");
        if(!int.TryParse(args[1],out int n)||n<100) return sh.io.Error("横幅は数値(100～)で指定してください");
        ComShProperties.width=n;
        PanelStyleCache.Dirty();
        return 0;
    }
    private static int CmdHeight(ComShInterpreter sh,List<string> args){
        if(!sh.interactiveq) return 0;
        if(args.Count==1){
            sh.io.Print($"{ComShProperties.height}\n");
            return 0;
        }
        if(args.Count>2) return sh.io.Error("使い方: height ターミナルの高さ(ピクセル)");
        if(!int.TryParse(args[1],out int n)||n<50) return sh.io.Error("高さは数値(50～)で指定してください");
        ComShProperties.height=n;
        PanelStyleCache.Dirty();
        return 0;
    }
    private static int CmdWinColor(ComShInterpreter sh,List<string> args){
        if(!sh.interactiveq) return 0;
        if(args.Count>3||args.Count<2) return sh.io.Error("使い方: wincolor 背景色 [文字色]");
        float[] bcol=ParseUtil.Rgba(args[1]);
        if(bcol==null) return sh.io.Error(ParseUtil.error);
        if(args.Count==3){
            float[] fcol=ParseUtil.Rgba(args[2]);
            if(fcol==null) return sh.io.Error(ParseUtil.error);
            PanelStyleCache.SetTextColor(fcol);
        }
        PanelStyleCache.SetBgColor(bcol);
        return 0;
    }
    private static int CmdFps(ComShInterpreter sh, List<string> args){
        if (args.Count == 1){
            sh.io.Print($"{Application.targetFrameRate}\n");
            return 0;
        }
        if (args.Count > 2) return sh.io.Error("使い方: fps フレームレート(数値)");
        if (!int.TryParse(args[1], out int n)) return sh.io.Error("数値の指定が不正です");
        Application.targetFrameRate=n;
        return 0;
    }
    private static int CmdVSync(ComShInterpreter sh, List<string> args){
        if (args.Count==1) {
            sh.io.Print($"{QualitySettings.vSyncCount}\n");
            return 0;
        }
        if (args.Count>2) return sh.io.Error("使い方: vsync 数値(0=off 1=on 2=half)");
        if (!int.TryParse(args[1], out int n) || n<0) return sh.io.Error("数値の指定が不正です");
        QualitySettings.vSyncCount=n;
        return 0;
    }
    private static int CmdTimeScale(ComShInterpreter sh, List<string> args){
        if(args.Count==1){
            sh.io.PrintLn(sh.fmt.FInt(Time.timeScale));
        }else{
            if(!float.TryParse(args[1],out float mul) || mul<0) return sh.io.Error("数値の指定が不正です");
            Time.timeScale=mul;
        }
        return 0;
    }
    private static int CmdPixelLights(ComShInterpreter sh, List<string> args){
        if(args.Count==1){
            sh.io.PrintLn(QualitySettings.pixelLightCount.ToString());
        }else{
            if(!int.TryParse(args[1],out int n) || n<0) return sh.io.Error("数値の指定が不正です");
            QualitySettings.pixelLightCount=n;
        }
        return 0;
    }
    private static int CmdNameSpace(ComShInterpreter sh, List<string> args){
        if(args.Count!=2){
            if(sh.ns!="") sh.io.PrintLn(sh.ns.Substring(0,sh.ns.Length-1));
            return 0;
        }
        if(args[1].Length==0) sh.ns=""; else sh.ns=args[1]+".";
        return 0;
    }

    private static int CmdNameSpaceClear(ComShInterpreter sh, List<string> args){
        if(args.Count!=2) return sh.io.Error($"使い方: {args[0]} 名前空間名");
        string id=args[1];
        var l=new List<string>(64);
        foreach(string k in BoneUtil.boneCache.Keys) if(k.StartsWith(id,Ordinal)) l.Add(k);
        foreach(string k in l) BoneUtil.boneCache.Remove(k);
        l.Clear();
        foreach(string k in CmdMisc.curveDic.Keys) if(k.StartsWith(id,Ordinal)) l.Add(k);
        foreach(string k in l) CmdMisc.curveDic.Remove(k);
        l.Clear();
        foreach(string k in CmdMisc.queDic.Keys) if(k.StartsWith(id,Ordinal)) l.Add(k);
        foreach(string k in l) CmdMisc.queDic.Remove(k);

        foreach(string k in ComShBg.cron.LsJob(id)) if(k.StartsWith("cron/"+id,Ordinal)) ComShBg.cron.KillJob(k);
        foreach(string k in ComShBg.cron2.LsJob(id)) if(k.StartsWith("cron2/"+id,Ordinal)) ComShBg.cron2.KillJob(k);
        return 0;
    }

    private static int CmdDate(ComShInterpreter sh, List<string> args){
        sh.io.PrintLn(DateTime.Now.ToString("yyyyMMddHHmmssfff"));
        return 0;
    }
    private static int CmdEcho(ComShInterpreter sh,List<string> args) {
        if(args.Count==1) return 0;
        sh.io.Print(args[1]);
        for(int i=2; i<args.Count; i++){sh.io.Print(sh.ofs).Print(args[i]);}
        return 0;
	}
	private static int CmdLog(ComShInterpreter sh,List<string> args) {
        if(args.Count==1) return 0;
        var sb=new StringBuilder();
        sb.Append(args[1]);
        for(int i=2; i<args.Count; i++) sb.Append(sh.ofs).Append(args[i]);
        Debug.Log(sb.ToString());
        return 0;
	}
    private static int CmdEnv(ComShInterpreter sh,List<string> args){
        foreach(var kv in sh.env) if(kv.Key[0]!=' ') sh.io.Print($"{kv.Key}={kv.Value.Get()}\n");
        foreach(var kv in Variables.g) sh.io.Print($"{kv.Key}={kv.Value}\n");
        if(sh.runningScript!=null) foreach(var kv in sh.runningScript.svars) sh.io.Print($"{kv.Key}={kv.Value}\n");
        return 0;
    }
    private static int CmdCut(ComShInterpreter sh,List<string> args){
        const string usage="使い方: cut 対象文字列 [区切り文字] 取り出す項目";
        string txt="",nth="",dlmt=" ",ofs=sh.ofs;
        if(args.Count==4){ txt=args[1]; dlmt=args[2]; nth=args[3]; }
        else if(args.Count==3){
            if(sh.io.pipedText!=null){ txt=sh.io.pipedText; dlmt=args[1]; nth=args[2];}
            else{ txt=args[1]; nth=args[2]; }
        }else if(args.Count==2){
            if(sh.io.pipedText!=null){ txt=sh.io.pipedText; nth=args[1];}
        }else return sh.io.Error(usage);
        if(dlmt.Length==0) return sh.io.Error("区切り文字の指定が不正です");
        int idx=nth.IndexOf(':');
        if(idx>0){
            ofs=nth.Substring(idx+1);
            nth=nth.Substring(0,idx);
        }
        string[] lines=txt.Split(ParseUtil.lf);
        if(lines.Length==0) return 0;
        List<string> cols;
        for(int i=0; i<lines.Length-1; i++){
            cols=ParseUtil.NthRange(lines[i].TrimEnd(ParseUtil.cr),dlmt,nth);
            if(cols==null) return sh.io.Error(ParseUtil.error);
            sh.io.PrintLn(string.Join(ofs,cols.ToArray()));
        }
        cols=ParseUtil.NthRange(lines[lines.Length-1].TrimEnd(ParseUtil.cr),dlmt,nth);
        if(cols==null) return sh.io.Error(ParseUtil.error);
        sh.io.Print(string.Join(ofs,cols.ToArray())); // 最終行はLnなしの方が良い
        return 0;
    }
    private static char[] trimChar={' ','\t','\r','\n'};
    private static int CmdTrim(ComShInterpreter sh,List<string> args){
        const string usage="使い方: trim 対象文字列 [削除文字]";
        string txt,del="";
        if(args.Count==3){ txt=args[1]; del=args[2];}
        else if(args.Count==2){
            if(sh.io.pipedText!=null){ txt=sh.io.pipedText; del=args[1]; }
            else txt=args[1];
        } else if(args.Count==1 && sh.io.pipedText!=null) txt=sh.io.pipedText;
        else return sh.io.Error(usage);
        if(del.Length>0) sh.io.Print(txt.Trim(del.ToCharArray()));
        else sh.io.PrintLn(txt.Trim(trimChar));
        return 0;
    }
    private static int CmdGrep(ComShInterpreter sh,List<string> args){
        const string usage="使い方: grep 対象文字列 検索キーワード ";
        string txt,kw;
        if(args.Count==3){ txt=args[1]; kw=args[2];}
        else if(args.Count==2 && sh.io.pipedText!=null){ txt=sh.io.pipedText; kw=args[1];}
        else return sh.io.Error(usage);
        string[] lines=txt.Split(ParseUtil.lf);
        for(int i=0; i<lines.Length; i++)
            if(lines[i].IndexOf(kw,Ordinal)>=0) sh.io.PrintLn(lines[i].TrimEnd(ParseUtil.cr));
        return 0;
    }
    private static int CmdLookup(ComShInterpreter sh,List<string> args){
        const string usage="使い方: lookup 対象文字列 キーワード1 [キーワード2...]";
        string text;
        int key0;
        if(sh.io.pipedText!=null){
            if(args.Count>1){ text=sh.io.pipedText; key0=1; }
            else return sh.io.Error(usage);
        }else{
            if(args.Count>2){ text=args[1]; key0=2; }
            else return sh.io.Error(usage);
        }
        for(int i=key0; i<args.Count; i++){
            string kw=args[i];
            int wordPos=0;
            if(!text.StartsWith(kw+":",Ordinal)){
                wordPos=text.IndexOf("\n"+kw+":",Ordinal);
                if(wordPos<0) continue;
                wordPos++;
            }
            int head=wordPos+kw.Length+1;
            int tail=text.IndexOf('\n',head);
            if(tail<0) tail=text.Length-1;
            string ret=text.Substring(head,tail-head+1).Trim(ParseUtil.crlf);
            if(i>key0) sh.io.Print(sh.ofs);
            sh.io.Print(ret);
        }
        return 0;
    }
    private static int CmdLine(ComShInterpreter sh,List<string> args){
        const string usage="使い方: line 対象文字列 行番号";
        string txt,nth;
        if(args.Count==3){ txt=args[1]; nth=args[2];}
        else if(args.Count==2 && sh.io.pipedText!=null){ txt=sh.io.pipedText; nth=args[1];}
        else return sh.io.Error(usage);
        if(!float.TryParse(nth,out float f)||f<=0) return sh.io.Error("行番号を数値(1～)で指定してください");
        int n=(int)f;
        string ret=ParseUtil.Nth(txt,'\n',n);
        if(ret!=null) sh.io.PrintLn(ret.TrimEnd(ParseUtil.cr));
        return 0;
    }
    private static int CmdCount(ComShInterpreter sh,List<string> args){
        const string usage="使い方: count {l|c[区切り文字]} 対象文字列";
        string txt;
        if(args.Count==3) txt=args[2];
        else if(args.Count==2 && sh.io.pipedText!=null) txt=sh.io.pipedText;
        else return sh.io.Error(usage);
        int cnt=0;
        if(args[1].Length==1 && (args[1][0]=='l'||args[1][0]=='L')){
            for(int i=0; i<txt.Length; i++) if(txt[i]=='\n') cnt++;
            if(txt.Length>0 && txt[txt.Length-1]!='\n') cnt++;
        } else if(args[1].Length==2 && (args[1][0]=='c'||args[1][0]=='C')){
            for(int i=0; i<txt.Length; i++) if(txt[i]==args[1][1]) cnt++;
        } else if(args[1].Length==1 && (args[1][0]=='c'||args[1][0]=='C')){
            cnt=txt.Length;
        } else return sh.io.Error(usage);
        sh.io.PrintLn(cnt.ToString());
        return 0;
    }
	private static int CmdSource(ComShInterpreter sh,List<string> args) {
        if(args.Count==1) return sh.io.Error("使い方: source スクリプトファイル名 [引数 ...]");
        return sh.ExecSource(args.GetRange(1,args.Count-1));
	}
    private static int CmdEval(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: eval コマンド文字列");
        var psr=EvalParser(sh,1);
        if(psr==null) return -1;
        psr.Reset(sh.currentParser.prevEoL);
        return sh.InterpretParserSingleSubCmd(psr);
    }
    private static int CmdEvalKag(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: evalkag スクリプト");
        TJSVariant ret=new TJSVariant();
        GameMain.Instance.ScriptMgr.EvalScript(args[1],ret);
        PrintTJSV(sh,ret);
        return 0;
    }
    private static int CmdExecKag(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: execkag スクリプト");
        TJSVariant ret=new TJSVariant();
        GameMain.Instance.ScriptMgr.ExecScript(args[1],ref ret);
        PrintTJSV(sh,ret);
        return 0;
    }
    private static void PrintTJSV(ComShInterpreter sh,TJSVariant val){
        switch(val.type){
        case TJSVariant.Type.tvtInteger:
            sh.io.Print(val.AsInteger().ToString());
            return;
        case TJSVariant.Type.tvtReal:
            sh.io.Print(sh.fmt.FVal(val.AsReal()));
            return;
        case TJSVariant.Type.tvtString:
            sh.io.Print(val.AsString());
            return;
        default:
            return;
        }
    }
    private static int CmdSystem(ComShInterpreter sh,List<string>args){
        if(args.Count==1 || args[1]=="") return sh.io.Error("使い方: system 外部コマンド名 引数1 ...");
        var sb=new StringBuilder((args.Count-1)*30);
        for(int i=2; i<args.Count; i++){
            string prm=args[i];
            if(prm.Length>2 && prm[0]=='*'){
                var tf=DataFiles.GetTempFile(prm.Substring(1));
                if(tf==null) return sh.io.Error("一時ファイル名が未定義です");
                prm=tf.filename;
            }else if(prm.IndexOf('\\')>=0) return sh.io.Error("パス区切り文字は使用できません");
            else{
                int sl=prm.IndexOf('/');
                if(sl>0 || (sl==0 && prm.Length>=2 && prm.IndexOf('/',1)>0))
                    return sh.io.Error("パス区切り文字は使用できません");
            }
            sb.Append(" ").Append(prm);
        }
        string ret=CmdSystemSub(args[1],sb.ToString());
        if(ret==null) return sh.io.Error("外部コマンドが実行できません");
        sh.io.Print(ret);
        return 0;
    }
    public static string CmdSystemSub(string cmd,string args){
        var p=new System.Diagnostics.Process();
        p.StartInfo.FileName=ComShInterpreter.scriptFolder+"bin\\"+cmd;
        p.StartInfo.Arguments=args;
        p.StartInfo.UseShellExecute=false;
        p.StartInfo.RedirectStandardOutput=true;
        p.StartInfo.RedirectStandardInput=false;
        //p.StartInfo.StandardOutputEncoding=Encoding.UTF8;
        p.StartInfo.CreateNoWindow=false;
        try{
            p.Start();
            string ret=p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return ret;
        }catch{}
        return null;
    }
    private static int CmdTest(ComShInterpreter sh,List<string> args){ return CmdTestSub(sh,args,0); }
    private static int CmdUnless(ComShInterpreter sh,List<string> args){ return CmdTestSub(sh,args,1); }
    private static int CmdTestSub(ComShInterpreter sh,List<string> args,int xor){
        string usage=$"使い方: {args[0]} 対象1 比較演算子 対象2 コマンド1 [コマンド2]\n"
                    +"比較演算子   eq|ne|ge|le|gt|lt|has|and|or\n"
                    +"コマンド1    比較結果が真のとき実行するコマンド\n"
                    +"コマンド2    比較結果が偽のとき実行するコマンド\n"
                    +"対象1,対象2  どちらも数値と解釈できれば数値として比較\n"
                    +"             そうでなければ文字列として比較される";
        int n=args.Count%4;
        if(args.Count<4 || (n!=1&&n!=2)) return sh.io.Error(usage);
        int cmp=CmdCmpSub(args,args.Count-n);
        if(cmp<0) return sh.io.Error(usage);
        sh.lastcmp=cmp;
        cmp=cmp^xor;
        if(cmp==1){
            var psr=EvalParser(sh,args.Count-n,false);
            if(psr==null) return -1;
            psr.Reset(sh.currentParser.prevEoL);
            return sh.InterpretParserSingleSubCmd(psr);
        }else if(n==2){
            var psr=EvalParser(sh,args.Count-1,false);
            if(psr==null) return -1;
            psr.Reset(sh.currentParser.prevEoL);
            return sh.InterpretParserSingleSubCmd(psr);
        }
        return 0;
    }
    private static int CmdThen(ComShInterpreter sh,List<string> args){if(sh.lastcmp==1) return CmdEval(sh,args); return 0; }
    private static int CmdElse(ComShInterpreter sh,List<string> args){if(sh.lastcmp==0) return CmdEval(sh,args); return 0; }
    private static int CmdThenIf(ComShInterpreter sh,List<string> args){if(sh.lastcmp==1) return CmdTest(sh,args); return 0;}
    private static int CmdElseIf(ComShInterpreter sh,List<string> args){if(sh.lastcmp==0) return CmdTest(sh,args); return 0;}
    private static int CmdCmp(ComShInterpreter sh,List<string> args){
        const string usage="使い方: cmp 対象1 比較演算子 対象2\n"
                    +"比較演算子   eq|ne|ge|le|gt|lt|has|and|or\n"
                    +"対象1,対象2  どちらも数値と解釈できれば数値として比較\n"
                    +"             そうでなければ文字列として比較される";
        if(args.Count<4 || args.Count%4!=0) return sh.io.Error(usage);
        int cmp=CmdCmpSub(args,args.Count);
        if(cmp<0) return sh.io.Error(usage);
        sh.lastcmp=cmp;
        sh.io.Print(cmp==1?"1":"0");
        return 0;
    }
    private static int CmdCmpSub(List<string> args,int n){
        int cmp=DoCmp(args[1],args[2],args[3]);
        if(cmp<0) return -1;
        for(int i=4; i<n; i+=4){
            string op=args[i];
            int c=DoCmp(args[1+i],args[2+i],args[3+i]);
            if(c<0) return -1;
            if(op=="and") cmp=(cmp==1 && c==1)?1:0;
            else if(op=="or") cmp=(cmp==1 || c==1)?1:0;
            else return -1;
        }
        return cmp;
    }
    private static int DoCmp(string val1,string op,string val2){
        double f1=ParseUtil.ParseDouble(val1);
        if(op=="between"){
            double[] fa=ParseUtil.MinMaxW(val2);
            if(double.IsNaN(f1) || fa==null) return -2;
            return (fa[0]<=f1 && f1<=fa[1])?1:0;
        }
        double f2=ParseUtil.ParseDouble(val2);
        bool numq=!double.IsNaN(f1)&&!double.IsNaN(f2);
        bool cmp;
        if(op=="eq") cmp=numq?(f1==f2):(string.CompareOrdinal(val1,val2)==0);
        else if(op=="ne") cmp=numq?(f1!=f2):(string.CompareOrdinal(val1,val2)!=0);
        else if(op=="ge") cmp=numq?(f1>=f2):(string.CompareOrdinal(val1,val2)>=0);
        else if(op=="le") cmp=numq?(f1<=f2):(string.CompareOrdinal(val1,val2)<=0);
        else if(op=="gt") cmp=numq?(f1>f2):(string.CompareOrdinal(val1,val2)>0);
        else if(op=="lt") cmp=numq?(f1<f2):(string.CompareOrdinal(val1,val2)<0);
        else if(op=="and") cmp=numq?(f1==1&&f2==1):false;
        else if(op=="or") cmp=numq?(f1==1||f2==1):false;
        else if(op=="has") cmp=val1.Contains(val2);
        else return -1;
        return cmp?1:0;
    }
	private static int CmdSelect(ComShInterpreter sh,List<string> args) {
        if(args.Count<4 || (args.Count%2)==1) return sh.io.Error("使い方: select 題名 選択肢ラベル1 コマンド1 ...");
        string title=args[1];
        string[][] sel=new string[(args.Count-2)/2][];
        for(int i=2,row=0; i<args.Count; i+=2)
            sel[row++]=new string[]{ (args[i].Length>0)?args[i]:args[i+1], args[i+1] };
        ComShWM.menu.SetMenu(title,sel);
        ComShWM.ShowMenu();
        if(!ComShWM.IsVisible()){ ComShWM.SetVisible(true); ComShWM.HideTerm();}
        return 0;
	}
    private static int CmdLS(ComShInterpreter sh,List<string> args){
        string dir="";
        if(args.Count>2) return sh.io.Error("使い方: ls [フォルダ名]");
        if(args.Count==2){
            dir=args[1];
            if(dir.Length>0 && (dir[0]=='/'||dir[0]=='\\')) dir=dir.Substring(1);
            if((Path.GetFullPath(ComShInterpreter.scriptFolder+dir).Length+1)<ComShInterpreter.scriptFolder.Length){
                // ..\等でscriptFolderより上のフォルダを指定させない
                return sh.io.Error($"{dir}:不正なフォルダ名です");
            }
        }
        string[] files;
        try{
            files=Directory.GetFiles(ComShInterpreter.scriptFolder+dir,"*.comsh");
        }catch(DirectoryNotFoundException){
            return sh.io.Error($"{dir}:フォルダが存在しません");
        }catch(IOException){
            return sh.io.Error($"{dir}:フォルダにアクセスできません");
        }
        if(files==null || files.Length==0) return sh.io.Error("フォルダ内にスクリプトが存在しません");
        Array.Sort(files);
        string[][] sel=new string[files.Length][];
        for(int i=0; i<files.Length; i++){
            string fn=files[i].Replace('\\','/');
            sel[i]=new string[]{ Path.GetFileNameWithoutExtension(fn), "\""+fn.Substring(ComShInterpreter.scriptFolder.Length)+"\""};
        }
        ComShWM.menu.SetMenu((args.Count==1)?"ComSh":args[1],sel);
        ComShWM.ShowMenu();
        if(!ComShWM.IsVisible()){ ComShWM.SetVisible(true); ComShWM.HideTerm();}
        return 0;
    }

    private static int CmdCron(ComShInterpreter sh,List<string> args){
        return CmdCronSub(sh,args,ComShBg.cron);
    }
    private static int CmdCron2(ComShInterpreter sh,List<string> args){
        return CmdCronSub(sh,args,ComShBg.cron2);
    }
    private static int CmdCronSub(ComShInterpreter sh,List<string> args,ComShBg.JobList bg){
        const string usage=
                 "使い方: {0} add 識別名 コマンド [実行間隔] [寿命] [順序]\n"
                +"　　　  {0} del 識別名 [識別名...]\n"
                +"　　　  {0} 識別名 ondestroy コマンド\n"
                +"コマンド  実行するコマンド\n"
                +"実行間隔  タスクを実行する間隔(ms)。省略時は0＝(できる限り)毎フレーム実行\n"
                +"寿命　　　この時間(ms)だけ経過したらタスクを自動削除。省略時0=自動削除なし\n"
                +"順序　　　この値が大きいものほど後に実行される。正の整数。省略時0";
        string jobpfx=args[0]+"/";
        if(args.Count<=2){
            var ls=bg.LsJob(jobpfx+sh.ns);
            foreach(string name in ls) sh.io.PrintLn(name.Substring(jobpfx.Length));
            return 0;
        }
        if(args[1]=="add" || args[1]=="set"){
            if(args.Count<4) return sh.io.Error(string.Format(usage,args[0]));
            if(args.Count>7) return sh.io.Error(string.Format(usage,args[0]));
            if(args[2]==string.Empty) return sh.io.Error("識別名が空です");
            if(!UTIL.ValidName(args[2])) return sh.io.Error("その名前は使用できません");
            string name=jobpfx+sh.ns+args[2];
            if(bg.ContainsName(name)){
                if(args[1]=="add") return sh.io.Error("その名前は既に使われています");
                bg.KillJob(name,true);
            }
            int prio=0;
            double ms=0,life=0;
            if(args.Count>=5 && (!double.TryParse(args[4],out ms) || ms<0)) return sh.io.Error("実行間隔の値が不正です");
            if(args.Count>=6 && (!double.TryParse(args[5],out life) || life<0)) return sh.io.Error("寿命の値が不正です");
            if(args.Count==7 && (!int.TryParse(args[6],out prio) || prio<0 || prio>100)) return sh.io.Error("順序の値が不正です");
            var sbo=new ComShInterpreter.SubShOutput();
            var subsh=new ComShInterpreter(new ComShInterpreter.Output(sbo.Output),sh.env,sh.func,sh.ns);

            ComShBg.Job j;
            if(args[3].Length>0 && args[3][0]=='&'){
                if(!sh.func.TryGetValue(args[3].Substring(1),out ComShInterpreter.ScriptStatus ss)||!ss.isFunc) return sh.io.Error("funcが未定義です");
                subsh.env[ComShInterpreter.SCRIPT_ERR_ON]="1";
                subsh.env.args.Clear();
                subsh.env.args.Add("");
                subsh.env.args.Add("");
                subsh.runningScript=ss;
                long stime=DateTime.UtcNow.Ticks,lasttime=0;
                j=bg.AddJob(name,(long)(ms*TimeSpan.TicksPerMillisecond),(long)(life*TimeSpan.TicksPerMillisecond),(long t)=>{
                    long cur=(t-stime)/TimeSpan.TicksPerMillisecond;
                    subsh.env.args[0]=cur.ToString();
                    subsh.env.args[1]=(cur-lasttime).ToString();
                    lasttime=cur;
                    ss.rewind(); subsh.exitq=false;
                    return ss.Run(subsh);
                },(int)prio);
            }else{
                var psr=EvalParser(sh,3,true,sh.currentParser.lineno);
                if(psr==null) return -1;
                subsh.env[ComShInterpreter.SCRIPT_ERR_ON]="1";
                subsh.env.args.Clear();
                subsh.env.args.Add("");
                subsh.env.args.Add("");
                long stime=DateTime.UtcNow.Ticks,lasttime=0;
                j=bg.AddJob(name,(long)(ms*TimeSpan.TicksPerMillisecond),(long)(life*TimeSpan.TicksPerMillisecond),(long t)=>{
                    long cur=(t-stime)/TimeSpan.TicksPerMillisecond;
                    subsh.env.args[0]=cur.ToString();
                    subsh.env.args[1]=(cur-lasttime).ToString();
                    lasttime=cur;
                    psr.Reset(); subsh.exitq=false;
                    return subsh.InterpretParser(psr);
                },(int)prio);
            }
            if(j==null) return sh.io.Error("登録に失敗しました");          
            j.sh=subsh;
        }else if(args[1]=="del"){
            for(int i=2; i<args.Count; i++) bg.KillJob(jobpfx+sh.ns+args[i]);
        }else if(args.Count==4){
            var name=jobpfx+sh.ns+args[1];
            var j=bg.Find(name);
            if(j==null) return sh.io.Error("その識別名の定期処理は存在しません");
            if(args[2]=="ondestroy"){
                var subsh=j.sh;
                var psr=new ComShParser(sh.currentParser.lineno);
                int r=psr.Parse(args[3]);
                if(r<0) return sh.io.Error(psr.error);
                if(r==0) return sh.io.Error("コマンドが空です");
                j.destroy=new ComShBg.JobAction((long t)=>{
                    subsh.env.args.Clear();
                    subsh.env.args.Add(t.ToString());
                    psr.Reset();
                    return subsh.InterpretParser(psr);
                });
            } else return sh.io.Error(string.Format(usage,args[0]));
        } else return sh.io.Error(string.Format(usage,args[0]));
        return 0;
    }
    private static int CmdCutLoop(ComShInterpreter sh,List<string> args){
        const string usage="使い方: cutloop 入力文字列 [区切り文字] コマンド";
        if(args.Count==4) return CutLoop(sh,args[1],args[2],3);
        else if(args.Count==3){
            if(sh.io.pipedText!=null) return CutLoop(sh,sh.io.pipedText,args[1],2);
            else return CutLoop(sh,args[1]," ",2);
        }else if(args.Count==2 && sh.io.pipedText!=null) return CutLoop(sh,sh.io.pipedText," ",1);
        else return sh.io.Error(usage);
    }
    private static int CutLoop(ComShInterpreter sh,string txt,string dlmt,int idx){
        if(txt=="") return 0;
        if(dlmt=="") return sh.io.Error("区切り文字が空です");
        var psr=EvalParser(sh,idx);
        if(psr==null) return -1;

        int[] pa=new int[3];
        int i=1;
        // 現シェルでの実行だが、出力だけはサブシェル実行と同じ形にする
        ComShInterpreter.Output orig=sh.io.output;
        var subout=new ComShInterpreter.SubShOutput();
        sh.io.output=new ComShInterpreter.Output(subout.Output);

        int ret=0;
        if(dlmt.Length==1){
            char d=dlmt[0]; // char型の方が若干速いので分ける。処理は同じ
            while(ParseUtil.CutNext(txt,d,pa)){
                sh.env["_1"]=txt.Substring(pa[0],pa[1]);
                sh.env["_2"]=i.ToString();
                psr.Reset();
                ret=sh.InterpretParser(psr);
                if(ret<0 || sh.exitq){ ret=sh.io.exitStatus; sh.exitq=false; break; }
                i++;
            }
        }else while(ParseUtil.CutNext(txt,dlmt,pa)){
            sh.env["_1"]=txt.Substring(pa[0],pa[1]);
            sh.env["_2"]=i.ToString();
            psr.Reset();
            ret=sh.InterpretParser(psr);
            if(ret<0 || sh.exitq){ ret=sh.io.exitStatus; sh.exitq=false; break; }
            i++;
        }

        sh.io.output=orig;
        if(ret>=0) sh.io.Print(subout.GetSubShResult());
        return ret;
    }
    private static int CmdLineLoop(ComShInterpreter sh,List<string> args){
        const string usage="使い方: lineloop 入力文字列 コマンド";
        const string usage2="使い方: lineloop コマンド";
        if(sh.io.pipedText!=null){
            if(args.Count!=2) return sh.io.Error(usage2);
            return CutLoop(sh,sh.io.pipedText,"\n",1);
        }else{
            if(args.Count!=3) return sh.io.Error(usage);
            return CutLoop(sh,args[1],"\n",2);
        }
    }
    private static int CmdRegexLoop(ComShInterpreter sh,List<string> args){
        const string usage="使い方: regexloop 入力文字列 正規表現 コマンド";
        const string usage2="使い方: regexloop 正規表現 コマンド";
        if(sh.io.pipedText!=null){
            if(args.Count!=3) return sh.io.Error(usage2);
            Regex reg=MiniSed.GetPtnAndOpt(args[1]);
            if(reg==null) return sh.io.Error("正規表現が不正です");
            return RegexLoop(sh,sh.io.pipedText,reg,args,2);
        }else{
            if(args.Count!=4) return sh.io.Error(usage);
            Regex reg=MiniSed.GetPtnAndOpt(args[2]);
            if(reg==null) return sh.io.Error("正規表現が不正です");
            return RegexLoop(sh,args[1],reg,args,3);
        }
    }
    private static int RegexLoop(ComShInterpreter sh,string text,Regex reg,List<string> args,int idx){
        var psr=EvalParser(sh,idx);
        if(psr==null) return -1;

        ComShInterpreter.Output orig=sh.io.output;
        var subout=new ComShInterpreter.SubShOutput();
        sh.io.output=new ComShInterpreter.Output(subout.Output);

        int ret=0;
        Match m=reg.Match(text);
        while(m.Success){
            for(int i=0; i<m.Groups.Count; i++) sh.env[$"_{i}"]=m.Groups[i].Value;
            psr.Reset();
            ret=sh.InterpretParser(psr);
            if(ret<0 || sh.exitq){ ret=sh.io.exitStatus; sh.exitq=false; break; }
            m=m.NextMatch();
        }
        sh.io.output=orig;
        if(ret>=0) sh.io.Print(subout.GetSubShResult());
        return ret;
    }
    private static int CmdPs(ComShInterpreter sh,List<string> args){
        var ls=ComShBg.cron.LsJob(string.Empty);
        foreach(string name in ls) sh.io.PrintLn(name);
        ls=ComShBg.cron2.LsJob(string.Empty);
        foreach(string name in ls) sh.io.PrintLn(name);
        return 0;
    }
    private static int CmdKill(ComShInterpreter sh,List<string> args){
        if(args.Count==1) return sh.io.Error("使い方: kill プロセス名...");
        for(int i=1; i<args.Count; i++) ComShBg.cron.KillJob(args[i]);
        for(int i=1; i<args.Count; i++) ComShBg.cron2.KillJob(args[i]);
        return 0;
    }
    private static string exprErr="";
    private static int CmdExpr(ComShInterpreter sh,List<string> args){
        const string usage="使い方: expr 値1 演算子(+|-|*|/|%) 値2 [演算子 値3...]";
        if(args.Count<4 || args.Count%2==1) return sh.io.Error(usage);
        double[] v1=ParseUtil.DoubleArr(args[1]);
        if(v1==null||v1.Length==0) return sh.io.Error("数値の形式が不正です");
        for(int i=2; i<args.Count; i+=2){
            if(args[i].Length!=1) return sh.io.Error("演算子が不正です");
            double[] v2=ParseUtil.DoubleArr(args[i+1]);
            if(v2==null||v2.Length==0) return sh.io.Error("数値の形式が不正です");
            v1=Calc(args[i][0],v1,v2);
            if(v1==null) return sh.io.Error(exprErr);
        }
        sh.io.Print(sh.fmt.FVal(v1[0]));
        for(int i=1; i<v1.Length; i++){ sh.io.Print(","); sh.io.Print(sh.fmt.FVal(v1[i])); }
        return 0;
    }
    private static double[] Calc(char op,double[] v1,double[] v2){
        int max,min;
        if(v1.Length>v2.Length){ max=v1.Length; min=v2.Length; }
        else { max=v2.Length; min=v1.Length; }
        if(max==min){
            if(max==4){
                if(op=='*'){
                    var q=new Quaternion((float)v1[0],(float)v1[1],(float)v1[2],(float)v1[3])*new Quaternion((float)v2[0],(float)v2[1],(float)v2[2],(float)v2[3]);
                    return new double[]{q.x,q.y,q.z,q.w};
                }else{
                    if(ExprTwoVal(max,op,v1,v2)<0) return null;
                    return v1;
                }
            }else{
                if(ExprTwoVal(max,op,v1,v2)<0) return null;
                return v1;
            }
        }else{
            if(min==1){
                if(v1.Length==1){
                    var fa=new double[max];
                    for(int i=0;i<max; i++) fa[i]=v1[0];
                    if(ExprTwoVal(max,op,fa,v2)<0) return null;
                    return fa;
                }else{
                    var fa=new double[max];
                    for(int i=0;i<max; i++) fa[i]=v2[0];
                    if(ExprTwoVal(max,op,v1,fa)<0) return null;
                    return v1;
                }
            }else if(min==3 && max==4){
                if(v1.Length==3){
                    if(op!='*'){ exprErr="演算子が不正です"; return null; }
                    var v=new Quaternion((float)v2[0],(float)v2[1],(float)v2[2],(float)v2[3])*new Vector3((float)v1[0],(float)v1[1],(float)v1[2]);
                    return new double[]{ v.x, v.y, v.z};
                }else{
                    if(op!='*'){ exprErr="演算子が不正です"; return null; }
                    var v=new Quaternion((float)v1[0],(float)v1[1],(float)v1[2],(float)v1[3])*new Vector3((float)v2[0],(float)v2[1],(float)v2[2]);
                    return new double[]{ v.x, v.y, v.z};
                }
            }else exprErr="その組み合わせの計算はできません";
        }
        return null;
    }
    private const double almost0=0.000001;
    private static int ExprTwoVal(int n1,char op,double[] v1,double[] v2){
        switch(op){
        case '+': for(int i=0; i<n1; i++) v1[i]+=v2[i]; break;
        case '-': for(int i=0; i<n1; i++) v1[i]-=v2[i]; break;
        case '*': for(int i=0; i<n1; i++) v1[i]*=v2[i]; break;
        case '/':
            for(int i=0; i<n1; i++){
                if(v2[i]>-almost0 && v2[i]<almost0){ exprErr="0で除算しようとしています"; return -1; }
                v1[i]/=v2[i];
            }
            break;
        case '%':
            for(int i=0; i<n1; i++){
                if(v2[i]>-almost0 && v2[i]<almost0){ exprErr="0で除算しようとしています"; return -1; }
                v1[i]%=v2[i];
            }
            break;
        default: exprErr="演算子が不正です"; return -1;
        }
        return 0;
    }
	private static int CmdSeqNo(ComShInterpreter sh,List<string> args){
        sh.io.Print(UTIL.GetSeqId());
        return 0;
    }
	private static int CmdRefreshMypose(ComShInterpreter sh,List<string> args){
        var mw=StudioMode.GetMotionWindow();
        if(mw==null) return sh.io.Error("スタジオモードでのみ有効です");
        var pmdm=PhotoMotionData.data;
        if(pmdm==null) return 0;
        if(PhotoMotionData.category_list==null||!PhotoMotionData.category_list.ContainsKey("マイポーズ")) return 0;
        var pmds=PhotoMotionData.category_list["マイポーズ"];
        string[] fns=Directory.GetFiles(PhotoModePoseSave.folder_path,"*.anm",SearchOption.TopDirectoryOnly);
        if(fns==null) return 0;
        // 削除(orリネーム)分反映
        for(int i=pmdm.Count-1; i>=0; i--) if(pmdm[i].category=="マイポーズ") pmdm.RemoveAt(i);
        pmds.Clear();
        for(int i=0; i<fns.Length; i++){
            var pmd=PhotoMotionData.AddMyPose(fns[i]);
            pmds.Add(pmd);
        }
        pmds.Sort((PhotoMotionData a,PhotoMotionData b)=>{return string.CompareOrdinal(a.direct_file,b.direct_file);});
        // UI選択肢更新
        var nameterm=new Dictionary<string, List<string>>(PhotoMotionData.category_list.Count);
		var data=new Dictionary<string, List<KeyValuePair<string, object>>>(PhotoMotionData.category_list.Count);
		foreach (var kv in PhotoMotionData.category_list){
			if(!data.ContainsKey(kv.Key)){
				data.Add(kv.Key,new List<KeyValuePair<string,object>>(kv.Value.Count));
				nameterm.Add(kv.Key, new List<string>(kv.Value.Count));
			}
			for(int i=0; i<kv.Value.Count; i++){
				data[kv.Key].Add(new KeyValuePair<string, object>(kv.Value[i].name,kv.Value[i]));
				nameterm[kv.Key].Add(kv.Value[i].nameTerm);
			}
		}
		mw.PopupAndTabList.SetData(data,nameterm,true);
        return 0;
    }

    private static int CmdFloor(ComShInterpreter sh,List<string> args){
        if(args.Count!=2 && args.Count!=3) return sh.io.Error("使い方: floor 値 [小数部桁数]");
        double v=ParseUtil.ParseDouble(args[1]);
        if(double.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        if(args.Count==3){
            float n=ParseUtil.ParseFloat(args[2]);
            if(float.IsNaN(n) || n<0 || n>99) return sh.io.Error("桁数の指定が不正です");
            double t=Math.Pow(10,n);
            sh.io.Print((Math.Floor(v*t)/t).ToString($"F{(int)n}"));
        }else{
            sh.io.Print(((long)Math.Floor(v)).ToString());
        }
        return 0;
    }
    private static int CmdCeil(ComShInterpreter sh,List<string> args){
        if(args.Count!=2 && args.Count!=3) return sh.io.Error("使い方: ceil 値 [小数部桁数]");
        double v=ParseUtil.ParseDouble(args[1]);
        if(double.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        if(args.Count==3){
            float n=ParseUtil.ParseFloat(args[2]);
            if(float.IsNaN(n) || n<0 || n>99) return sh.io.Error("桁数の指定が不正です");
            double t=Math.Pow(10,n);
            sh.io.Print((Math.Ceiling(v*t)/t).ToString($"F{(int)n}"));
        }else{
            sh.io.Print(((long)Math.Ceiling(v)).ToString());
        }
        return 0;
    }
    private static int CmdTruncate(ComShInterpreter sh,List<string> args){
        if(args.Count!=2 && args.Count!=3) return sh.io.Error("使い方: truncate 値 [小数部桁数]");
        double v=ParseUtil.ParseDouble(args[1]);
        if(double.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        if(args.Count==3){
            float n=ParseUtil.ParseFloat(args[2]);
            if(float.IsNaN(n) || n<0 || n>99) return sh.io.Error("桁数の指定が不正です");
            double t=Math.Pow(10,n);
            sh.io.Print((Math.Truncate(v*t)/t).ToString($"F{(int)n}"));
        }else{
            sh.io.Print(((long)Math.Truncate(v)).ToString());
        }
        return 0;
    }
    private static int CmdRoundUp(ComShInterpreter sh,List<string> args){
        if(args.Count!=2 && args.Count!=3) return sh.io.Error("使い方: roundup 値 [小数部桁数]");
        double v=ParseUtil.ParseDouble(args[1]);
        if(double.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        if(args.Count==3){
            float n=ParseUtil.ParseFloat(args[2]);
            if(float.IsNaN(n) || n<0 || n>99) return sh.io.Error("桁数の指定が不正です");
            double t=Math.Pow(10,n);
            sh.io.Print((((v>=0)?Math.Ceiling(v*t):Math.Floor(v*t))/t).ToString($"F{(int)n}"));
        }else{
            sh.io.Print(((long)((v>=0)?Math.Ceiling(v):Math.Floor(v))).ToString());
        }
        return 0;
    }
    private static int CmdRound(ComShInterpreter sh,List<string> args){ return CmdRoundSub(sh,args,MidpointRounding.AwayFromZero); }
    private static int CmdRound2(ComShInterpreter sh,List<string> args){ return CmdRoundSub(sh,args,MidpointRounding.ToEven); }
    private static int CmdRoundSub(ComShInterpreter sh,List<string> args,MidpointRounding mr){
        if(args.Count!=2 && args.Count!=3) return sh.io.Error($"使い方: round 値 [小数部桁数]");
        double v=ParseUtil.ParseDouble(args[1]);
        if(double.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        if(args.Count==3){
            float n=ParseUtil.ParseFloat(args[2]);
            if(float.IsNaN(n) || n<0 || n>99) return sh.io.Error("桁数の指定が不正です");
            sh.io.Print(Math.Round(v,(int)n,mr).ToString($"F{(int)n}"));
        }else{
            sh.io.Print(((long)Math.Round(v,mr)).ToString());
        }
        return 0;
    }
    private static int CmdSin(ComShInterpreter sh,List<string> args){
        if(args.Count<2||args.Count>6) return sh.io.Error(
            "使い方: sin 角度(度) [半径] [中央値] [角速度(度/ms)] [時刻(ms)]"
        );
        return SinCosCommon(sh,args,0);
    }
    private static int CmdCos(ComShInterpreter sh,List<string> args){
        if(args.Count<2||args.Count>6) return sh.io.Error(
            "使い方: cos 角度(度) [半径] [中央値] [角速度(度/ms)] [時刻(ms)]"
        );
        return SinCosCommon(sh,args,1);
    }
    private static int SinCosCommon(ComShInterpreter sh, List<string> args, int sc){
        float w,t,p,r,m;
        var prms=ParseUtil.NormalizeParams(args,new string[]{null,"1","0","0","0"},1);
        if(prms==null) return sh.io.Error(ParseUtil.error);
        p=ParseUtil.ParseFloat(prms[0]); r=ParseUtil.ParseFloat(prms[1]);
        m=ParseUtil.ParseFloat(prms[2]); w=ParseUtil.ParseFloat(prms[3]);
        t=ParseUtil.ParseFloat(prms[4]);
        if(float.IsNaN(p)||float.IsNaN(r)||float.IsNaN(m)
            ||float.IsNaN(w)||float.IsNaN(t)) sh.io.Error("数値の指定が不正です");
        float val;
        if(sc==0) val=Mathf.Sin((t*w+p)*Mathf.Deg2Rad)*r+m;
        else val=Mathf.Cos((t*w+p)*Mathf.Deg2Rad)*r+m;
        sh.io.Print(sh.fmt.FVal(val));
        return 0;
    }
    private static int CmdTan(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error( "使い方: tan 角度(度)" );
        float v=ParseUtil.ParseFloat(args[1]);
        if(float.IsNaN(v)) return sh.io.Error(ParseUtil.error);
        float val=Mathf.Tan(v*Mathf.Deg2Rad);
        if(float.IsPositiveInfinity(val)) val=float.MaxValue;
        else if(float.IsNegativeInfinity(val)) val=float.MinValue;
        sh.io.PrintLn(sh.fmt.FVal(val));
        return 0;
    }
    private static int CmdASin(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error( "使い方: asin 値" );
        float v=ParseUtil.ParseFloat(args[1]);
        if(float.IsNaN(v)) return sh.io.Error(ParseUtil.error);
        if(v<-1 || v>1) return sh.io.Error("値の範囲が不正です");
        sh.io.PrintLn(sh.fmt.FInt(Mathf.Asin(v)*Mathf.Rad2Deg));
        return 0;
    }
    private static int CmdACos(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error( "使い方: acos 値" );
        float v=ParseUtil.ParseFloat(args[1]);
        if(float.IsNaN(v)) return sh.io.Error(ParseUtil.error);
        if(v<-1 || v>1) return sh.io.Error("値の範囲が不正です");
        sh.io.PrintLn(sh.fmt.FInt(Mathf.Acos(v)*Mathf.Rad2Deg));
        return 0;
    }
    private static int CmdATan(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error( "使い方: atan 値" );
        float v=ParseUtil.ParseFloat(args[1]);
        if(float.IsNaN(v)) return sh.io.Error(ParseUtil.error);
        sh.io.PrintLn(sh.fmt.FInt(Mathf.Atan(v)*Mathf.Rad2Deg));
        return 0;
    }
    private static int CmdATan2(ComShInterpreter sh,List<string> args){
        if(args.Count!=3) return sh.io.Error( "使い方: atan2 値(y) 値(x)" );
        float dy=ParseUtil.ParseFloat(args[1]);
        if(float.IsNaN(dy)) return sh.io.Error(ParseUtil.error);
        float dx=ParseUtil.ParseFloat(args[2]);
        if(float.IsNaN(dx)) return sh.io.Error(ParseUtil.error);
        sh.io.PrintLn(sh.fmt.FInt(Mathf.Atan2(dy,dx)*Mathf.Rad2Deg));
        return 0;
    }
    private static int CmdAbs(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: abs 値");
        double v=ParseUtil.ParseDouble(args[1]);
        if(double.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        sh.io.Print(sh.fmt.FVal(Math.Abs(v)));
        return 0;
    }
    private static int CmdSqrt(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: sqrt 値");
        double v=ParseUtil.ParseDouble(args[1],-1);
        if(v<0) return sh.io.Error("数値の指定が不正です");
        sh.io.Print(sh.fmt.FVal(Math.Sqrt(v)));
        return 0;
    }
    private static int CmdMax(ComShInterpreter sh,List<string> args){
        if(args.Count<2) return sh.io.Error("使い方: max 値1 [値2 ...]");
        double max=double.MinValue;
        for(int i=1; i<args.Count; i++){
            double v=ParseUtil.ParseDouble(args[i]);
            if(double.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
            if(v>max) max=v;
        }
        sh.io.Print(sh.fmt.FVal(max));
        return 0;
    }
    private static int CmdMin(ComShInterpreter sh,List<string> args){
        if(args.Count<2) return sh.io.Error("使い方: min 値1 [値2 ...]");
        double min=double.MaxValue;
        for(int i=1; i<args.Count; i++){
            double v=ParseUtil.ParseDouble(args[i]);
            if(double.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
            if(v<min) min=v;
        }
        sh.io.Print(sh.fmt.FVal(min));
        return 0;
    }
    private static int CmdNvl(ComShInterpreter sh,List<string> args){
        if(args.Count<2) return sh.io.Error("使い方: nvl 値1 [値2 ...]");
        for(int i=1; i<args.Count; i++)
            if(args[i]!=""){ sh.io.Print(args[i]); return 0; }
        return 0;
    }
    private static int CmdRnd(ComShInterpreter sh,List<string> args){
        if(args.Count>2) return sh.io.Error("使い方: rnd [シード]");
        if(args.Count==2){
            if(!int.TryParse(args[1],out int s)) return sh.io.Error("数値の指定が不正です");
            UnityEngine.Random.InitState(s);
        }
        sh.io.Print(sh.fmt.FVal(UnityEngine.Random.value));
        return 0;
    }
    private static int CmdRndN(ComShInterpreter sh,List<string> args){
        if(args.Count!=2 && args.Count!=3) return sh.io.Error("使い方: rndn 最大値 [シード]");
        if(!float.TryParse(args[1],out float n)||n==0) return sh.io.Error("数値の指定が不正です");
        if(args.Count==3){
            if(!int.TryParse(args[2],out int s)) return sh.io.Error("数値の指定が不正です");
            UnityEngine.Random.InitState(s);
        }
        sh.io.Print(((int)UnityEngine.Random.Range(0,n+1)).ToString());
        return 0;
    }
    private static int CmdPerlinNoise(ComShInterpreter sh,List<string> args){
        const string usage="使い方: perlinnoise x y [半径 角度]";
        if(args.Count<2) return sh.io.Error(usage);
        int p=args[1].IndexOf(',');
        float[] xy;
        if(p>=0){
            if(args.Count!=2 && args.Count!=4) return sh.io.Error(usage);
            xy=ParseUtil.Xy(args[1]);
            if(xy==null) return sh.io.Error(ParseUtil.error);
            p=2;
        }else{
            if(args.Count!=3 && args.Count!=5) return sh.io.Error(usage);
            xy=new float[2];
            if(!float.TryParse(args[1],out xy[0])) return sh.io.Error("数値の指定が不正です");
            if(!float.TryParse(args[2],out xy[1])) return sh.io.Error("数値の指定が不正です");
            p=3;
        }

        if(args.Count>=p+2){
            if(!float.TryParse(args[p],out float r)||r<=0) return sh.io.Error("数値の指定が不正です");
            if(!float.TryParse(args[p+1],out float d)) return sh.io.Error("数値の指定が不正です");
            float rad=d*Mathf.Deg2Rad;
            xy[0]+=r*Mathf.Cos(rad);
            xy[1]+=r*Mathf.Sin(rad);
        }
        sh.io.Print(sh.fmt.FVal(Mathf.PerlinNoise(xy[0],xy[1])));
        return 0;
    }
    private static int CmdQuat(ComShInterpreter sh,List<string> args){
        const string usage="使い方1: quat 回転軸 角度\n"
                          +"使い方2: quat オイラー角\n"
                          +"使い方3: quat 回転前ベクトル 回転後ベクトル\n"
                          +"使い方4: quat 中心点 始点 終点";
        if(args.Count==2){
            float[] eu=ParseUtil.Xyz(args[1]);
            if(eu==null) return sh.io.Error(ParseUtil.error);
            sh.io.Print(sh.fmt.FQuat(Quaternion.Euler(new Vector3(eu[0],eu[1],eu[2]))));
        }else if(args.Count==3){
            if(args[2].IndexOf(',')<0){
                float[] vec=ParseUtil.Xyz(args[1]);
                if(vec==null) return sh.io.Error(ParseUtil.error);
                float deg=ParseUtil.ParseFloat(args[2]);
                if(float.IsNaN(deg)) return sh.io.Error("数値の指定が不正です");
                float rad=deg/2*Mathf.Deg2Rad;
                float s=Mathf.Sin(rad),c=Mathf.Cos(rad);
                sh.io.PrintJoinLn(",",sh.fmt.FVal(vec[0]*s),sh.fmt.FVal(vec[1]*s),sh.fmt.FVal(vec[2]*s),sh.fmt.FVal(c));
            }else{
                float[] v0=ParseUtil.Xyz(args[1]);
                if(v0==null) return sh.io.Error(ParseUtil.error);
                float[] v1=ParseUtil.Xyz(args[2]);
                if(v1==null) return sh.io.Error(ParseUtil.error);
                sh.io.Print(sh.fmt.FQuat(Quaternion.FromToRotation(new Vector3(v0[0],v0[1],v0[2]),new Vector3(v1[0],v1[1],v1[2]))));
            }
        }else if(args.Count==4){
            float[] p0=ParseUtil.Xyz(args[1]); if(p0==null) return sh.io.Error(ParseUtil.error);
            float[] p1=ParseUtil.Xyz(args[1]); if(p1==null) return sh.io.Error(ParseUtil.error);
            float[] p2=ParseUtil.Xyz(args[1]); if(p2==null) return sh.io.Error(ParseUtil.error);
            Vector3 v1=new Vector3(p1[0]-p0[0],p1[1]-p0[1],p1[2]-p0[2]),v2=new Vector3(p2[0]-p0[0],p2[1]-p0[1],p2[2]-p0[2]);
            sh.io.Print(sh.fmt.FQuat(Quaternion.FromToRotation(v1,v2)));
        }else return sh.io.Error(usage);
        return 0;
    }
	private static int CmdSleep(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: sleep 待機時間(ms)");
        if(!double.TryParse(args[1],out double t)||t<0) return sh.io.Error("数値の指定が不正です");
        if(sh.runningScript!=null) sh.runningScript.toSleep(t);
        return -1;  // マルチステートメントの後続を処理しないためエラーにする
    }
    private static int CmdKvs(ComShInterpreter sh,List<string> args){
        var kvs=Variables.g;
        if(args.Count==1){ // 一覧
            foreach(var kv in kvs) sh.io.PrintLn(kv.Key+"="+kv.Value);
        }else if(args.Count==2){ // 参照
            if(kvs.ContainsKey(args[1])) sh.io.Print(kvs[args[1]]);
        }else if(args.Count==3){ // 設定
            if(IsKvsNameValid(args[1])){
                if(args[2]=="") kvs.Remove(args[1]);
                else kvs[args[1]]=args[2];
            }else return sh.io.Error("変数名が不正です");
        }else return sh.io.Error("引数が多すぎます");
        return 0;
    }
    private static string kvsdir=ComShInterpreter.scriptFolder+@"kvs\";
    private static int mkKvsDir(){
        try{
            if(!Directory.Exists(kvsdir)) Directory.CreateDirectory(kvsdir);
        }catch{ return -1; }
        return 0;
    }
    private static int CmdKvsClear(ComShInterpreter sh,List<string> args){
        if(args.Count>2) return sh.io.Error("使い方: kvs.clear [プレフィクス]");
        string prefix=(args.Count==2)?args[1]:"";
        var kvs=Variables.g;
        List<string> delkey=new List<string>(kvs.Count);
        foreach(var kv in kvs){
            if(prefix!="" && !kv.Key.StartsWith(prefix,Ordinal)) continue;
            delkey.Add(kv.Key);
        }
        foreach(var k in delkey) kvs.Remove(k);
        return 0;
    }
    private static int CmdKvsSave(ComShInterpreter sh,List<string> args){
        if(args.Count==1 || args.Count>3) return sh.io.Error("使い方: kvs.save 名前 [プレフィクス]");
        string fn=args[1], prefix=(args.Count==3)?args[2]:"";
        if(fn=="" || !UTIL.ValidName(fn)) return sh.io.Error("名前の指定が不正です");
        if(mkKvsDir()<0) return sh.io.Error("処理に失敗しました");
        try{
            var kvs=Variables.g;
            using (var wr=new StreamWriter( kvsdir+fn,false,Encoding.UTF8)){
                if(prefix!=""){
                    foreach(var kv in kvs)
                        if(kv.Key.StartsWith(prefix,Ordinal)) wr.Write($"{kv.Key}\t{kv.Value}\t");
                }else foreach(var kv in kvs) wr.Write($"{kv.Key}\t{kv.Value}\t");
            }
        }catch{
            return sh.io.Error("書き込みに失敗しました");
        }
        return 0;
    }
    private static int CmdKvsLoad(ComShInterpreter sh,List<string> args){
        if(args.Count==1 || args.Count>3) return sh.io.Error("使い方: kvs.load 名前 [プレフィクス]");
        string fn=args[1], prefix=(args.Count==3)?args[2]:"";
        if(fn=="" || !UTIL.ValidName(fn)) return sh.io.Error("名前の指定が不正です");
        if(mkKvsDir()<0) return sh.io.Error("処理に失敗しました");
        try{ 
            string buf=File.ReadAllText( kvsdir+fn,Encoding.UTF8);
            var idx=new int[3];
            string key,value;
            while( ParseUtil.CutNext(buf,'\t',idx) ){
                key=buf.Substring(idx[0],idx[1]);
                if(ParseUtil.CutNext(buf,'\t',idx)) value=buf.Substring(idx[0],idx[1]); else break;
                if(key.StartsWith(prefix,Ordinal)) Variables.g[key]=value;
            }
        }catch{ return sh.io.Error("読み込みに失敗しました"); }
        return 0;
    }
    private static bool IsKvsNameValid(string key){
        int i;
        for(i=0; i<key.Length; i++) if(!(ParseUtil.IsWordChar(key[i])||key[i]=='/')) break;
        return i==key.Length;
    }
    private static int CmdDistance(ComShInterpreter sh,List<string> args){
        float[] v=new float[4];
        if(args.Count!=2 && args.Count!=3) return sh.io.Error("使い方: distance 座標1 [座標2]");
        int d=ParseUtil.XyzSub(args[1],v);
        if(d<=0||d>4) return sh.io.Error("座標の指定が不正です");
        if(args.Count==3){
            float[] v1=new float[4];
            int d1=ParseUtil.XyzSub(args[2],v1);
            if(d1!=d) return sh.io.Error("座標の指定が不正です");
            v[0]-=v1[0]; v[1]-=v1[1]; v[2]-=v1[2]; v[3]-=v1[3];
        }
        float l=0;
        if(d==1) l=Mathf.Abs(v[0]); else{
            for(int i=0; i<d; i++) l+=v[i]*v[i];
            l=Mathf.Sqrt(l);
        }
        sh.io.Print(sh.fmt.FVal(l));
        return 0;
    }
    private static int CmdNormalize(ComShInterpreter sh,List<string> args){
        float[] v=new float[4];
        if(args.Count!=2 && args.Count!=3) return sh.io.Error("使い方: normalize 座標1 [座標2]");
        int d=ParseUtil.XyzSub(args[1],v);
        if(d<=0||d>4) return sh.io.Error("座標の指定が不正です");
        if(args.Count==3){
            float[] v1=new float[4];
            int d1=ParseUtil.XyzSub(args[2],v1);
            if(d1!=d) return sh.io.Error("座標の指定が不正です");
            v[0]-=v1[0]; v[1]-=v1[1]; v[2]-=v1[2]; v[3]-=v1[3];
        }
        float l=0;
        if(d==1) l=Mathf.Abs(v[0]); else{
            for(int i=0; i<d; i++) l+=v[i]*v[i];
            l=Mathf.Sqrt(l);
        }
        if(Mathf.Approximately(l,0)){
            sh.io.Print('0'); for(int i=1; i<d; i++) sh.io.Print(",0");
        }else{
            sh.io.Print(sh.fmt.FVal(v[0]/l));
            for(int i=1; i<d; i++) sh.io.Print(",").Print(sh.fmt.FVal(v[i]/l));
        }
        return 0;
    }
    private static int CmdReplace(ComShInterpreter sh,List<string> args){
        const string usage="使い方: replace 値 検索文字列 置換文字列";
        string val,before,after;
        if(args.Count==4){
            val=args[1]; before=args[2]; after=args[3];
        }else if(args.Count==3 && sh.io.pipedText!=null){
            val=sh.io.pipedText; before=args[1]; after=args[2];
        }else return sh.io.Error(usage);
        if(before==""){ sh.io.Print(val); return 0;}
        sh.io.Print(val.Replace(before,after));
        return 0;
    }
    private static int CmdSplit(ComShInterpreter sh,List<string> args){
        const string usage="使い方: split 値 区切り文字 変数名1,...";
        string val,dlmt;
        string[] vars;
        if(args.Count==4){
            val=args[1]; dlmt=args[2]; vars=args[3].Split(ParseUtil.comma);
        }else if(args.Count==3){
            if(sh.io.pipedText==null){ val=args[1]; dlmt=" "; }
            else{ val=sh.io.pipedText; dlmt=args[1]; }
            vars=args[2].Split(ParseUtil.comma);
        }else if(args.Count==2){
            val=sh.io.pipedText; dlmt=" ";
            vars=args[1].Split(ParseUtil.comma);
        }else return sh.io.Error(usage);
        int i;
        for(i=0; i<vars.Length; i++)
            if(!ParseUtil.IsVarName(vars[i]) || ParseUtil.IsVar1Name(vars[i])) return sh.io.Error("変数名が不正です");
        string[] sa=val.Split(dlmt.ToCharArray());
        int n=(sa.Length>vars.Length)?vars.Length:sa.Length;
        var svar=sh.runningScript!=null?sh.runningScript.svars:null;
        for(i=0; i<n; i++) Variables.Set(vars[i],sa[i],sh.env,svar);
        for(; i<vars.Length; i++) Variables.Set(vars[i],"",sh.env,svar);
        return 0;
    }
    private static int CmdSuffix(ComShInterpreter sh,List<string> args){
        const string usage="使い方: suffix 値 末尾文字列";
        string val,sfx;
        if(args.Count==3){
            val=args[1]; sfx=args[2];
        }else if(args.Count== 2 && sh.io.pipedText!=null){
            val=sh.io.pipedText; sfx=args[1];
        }else return sh.io.Error(usage);
        if(!val.EndsWith(sfx,Ordinal)) sh.io.Print(val+sfx);
        else sh.io.Print(val);
        return 0;
    }
    private static int CmdPrefix(ComShInterpreter sh,List<string> args){
        const string usage="使い方: prefix 値 先頭文字列";
        string val,pfx;
        if(args.Count==3){
            val=args[1]; pfx=args[2];
        }else if(args.Count== 2 && sh.io.pipedText!=null){
            val=sh.io.pipedText; pfx=args[1];
        }else return sh.io.Error(usage);
        if(!val.StartsWith(pfx,Ordinal)) sh.io.Print(pfx+val);
        else sh.io.Print(val);
        return 0;
    }
    private static int CmdCrossProduct(ComShInterpreter sh,List<string> args){
        float[] v1=new float[3],v2=new float[3];
        if(args.Count!=3) return sh.io.Error("使い方: crossproduct ベクトル1 ベクトル2");
        int d=ParseUtil.XyzSub(args[1],v1);
        if(d<=1||d>3) return sh.io.Error("ベクトルの指定が不正です");
        int d1=ParseUtil.XyzSub(args[2],v2);
        if(d!=d1) return sh.io.Error("ベクトルの指定が不正です");
        if(d==3){
            float x=v1[1]*v2[2]-v1[2]*v2[1];
            float y=v1[2]*v2[0]-v1[0]*v2[2];
            float z=v1[0]*v2[1]-v1[1]*v2[0];
            sh.io.Print($"{sh.fmt.FVal(x)},{sh.fmt.FVal(y)},{sh.fmt.FVal(z)}");
        }else{
            sh.io.Print(sh.fmt.FVal(v1[0]*v2[1]-v1[1]*v2[0]));
        }
        return 0;
    }
    private static int CmdDotProduct(ComShInterpreter sh,List<string> args){
        float[] v1=new float[3],v2=new float[3];
        if(args.Count!=3) return sh.io.Error("使い方: dotproduct ベクトル1 ベクトル2");
        int d=ParseUtil.XyzSub(args[1],v1);
        if(d<=1||d>3) return sh.io.Error("ベクトルの指定が不正です");
        int d1=ParseUtil.XyzSub(args[2],v2);
        if(d!=d1) return sh.io.Error("ベクトルの指定が不正です");
        if(d==3) sh.io.Print(sh.fmt.FVal(v1[0]*v2[0]+v1[1]*v2[1]+v1[2]*v2[2]));
        else sh.io.Print(sh.fmt.FVal(v1[0]*v2[0]+v1[1]*v2[1]));
        return 0;
    }
    private static int CmdClamp(ComShInterpreter sh,List<string> args){
        double v,min,max;
        if(args.Count!=4 || !double.TryParse(args[1],out v)
            || !double.TryParse(args[2],out min) || !double.TryParse(args[3],out max) )
                return sh.io.Error("使い方: clamp 値 最小値 最大値");
        if(v<min) v=min; if(v>max) v=max;
        sh.io.Print(sh.fmt.FVal(v));
        return 0;
    }
    private static int CmdInside(ComShInterpreter sh,List<string> args){
        const string usage="使い方: inside 座標 中心座標 サイズ [回転]";
        if(args.Count!=4 && args.Count!=5) return sh.io.Error(usage);
        float[] p={0,0,0};
        int n=ParseUtil.XyzSub(args[1],p);
        if(n<=0||n>3) return sh.io.Error("座標が不正です");
        float[] o={0,0,0};
        int n2=ParseUtil.XyzSub(args[2],o);
        if(n2!=n) return sh.io.Error("座標が不正、または次数が一致していません");
        float[] sz={0,0,0};
        n2=ParseUtil.XyzSub(args[3],sz);
        if(n2<=0) return sh.io.Error("サイズの指定が不正です");
        Vector3 v3=new Vector3(p[0]-o[0],p[1]-o[1],p[2]-o[2]);
        if(args.Count==5){
            if(n==3){
                float[] r=ParseUtil.FloatArr(args[4]);
                if(r==null || (r.Length!=3 && r.Length!=4)) return sh.io.Error("回転が不正です");
                var q=(r.Length==4)?new Quaternion(r[0],r[1],r[2],r[3]):Quaternion.Euler(new Vector3(r[0],r[1],r[2]));
                v3=Quaternion.Inverse(q)*v3; // 座標を領域側の座標系に翻訳
            }else if(n==2){
                if(float.TryParse(args[4],out float deg)) return sh.io.Error("回転が不正です");
                var q=Quaternion.Euler(new Vector3(0,0,-deg));
                v3=q*v3;
            }else return sh.io.Error("1次元で回転はできません");
        }
        bool result=false;
        if(n==3){
            if(n2==1){ // 球
                if(sz[0]<=0) return sh.io.Error("サイズの指定が不正です");
                result=(sz[0]*sz[0]>=v3.sqrMagnitude);
            }else if(n2==2){ // シリンダ
                result=(-sz[1]/2<=v3.y && v3.y<=sz[1]/2 && (v3.x*v3.x+v3.z*v3.z)<=sz[0]*sz[0]);
            }else if(n2==3){ // 直方体
                result=(-sz[0]/2<=v3.x&&v3.x<=sz[0]/2&&-sz[1]/2<=v3.y&&v3.y<=sz[1]/2&&-sz[2]/2<=v3.z&&v3.z<=sz[2]/2);
            } else return sh.io.Error("サイズの指定が不正です");
        }else if(n==2){
            if(n2==1) result=v3.x*v3.x+v3.y*v3.y<=sz[0]*sz[0]; // 円
            else if(n2==2) result=(-sz[0]/2<=v3.x&&v3.x<=sz[0]/2&&-sz[1]/2<=v3.y&&v3.y<=sz[1]/2); // 矩形
            else return sh.io.Error("サイズの指定が不正です");
        }else{
            if(n2==1) result=Mathf.Abs(v3.x)<=sz[0];
            else return sh.io.Error("サイズの指定が不正です");
        }
        sh.io.Print(result?"1":"0");
        return 0;
    }
    private static int CmdUnset(ComShInterpreter sh,List<string> args){
        if(args.Count==1) sh.io.Error("使い方: unset 変数名1 [変数名2 ...]");
        bool changed=false;
        for(int i=1; i<args.Count; i++){
            if(args[i].Length==0) continue;
            if(args[i][0]=='/') Variables.g.Remove(args[i]);
            else if(args[i][0]=='.') {
                if(sh.runningScript!=null) sh.runningScript.svars.Remove(args[i]);
            }else {
                sh.env.Remove(args[i]);
                sh.env.Remove(" "+args[i]); // staticで作った匿名変数
                changed=true;
            }
        }
        if(changed) sh.envChanged=true;
        return 0;
    }
    private static int CmdSed(ComShInterpreter sh,List<string> args){
        const string usage="使い方: sed 値 コマンド";
        string val,cmd;
        if(args.Count==2 && sh.io.pipedText!=null){
            val=sh.io.pipedText; cmd=args[1];
        }else if(args.Count==3){
            val=args[1]; cmd=args[2];
        }else return sh.io.Error(usage);
        if(cmd=="") return sh.io.Error(usage);
        var sed=new MiniSed(cmd);
        if(sed.error!=null) return sh.io.Error(sed.error);
        sh.io.Print(sed.Process(val));
        return 0 ;
    }
    private static int CmdMemInfo(ComShInterpreter sh,List<string> args){
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        sh.io.PrintLn($"Unity reserved:{UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong().ToString("N0")}bytes");
        sh.io.PrintLn($"Unity allocated:{UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong().ToString("N0")}bytes");
        return 0;
    }
    private static int CmdClean(ComShInterpreter sh,List<string> args){
        Resources.UnloadUnusedAssets();
        return 0;
    }
    private static int CmdGC(ComShInterpreter sh,List<string> args){
        System.GC.Collect();
        return 0;
    }

    private static int CmdSubstr(ComShInterpreter sh,List<string> args){
        const string usage="使い方: substr 値 開始位置 文字数";
        string val,st,cnt;
        if(args.Count==2 && sh.io.pipedText!=null){
            val=sh.io.pipedText; st=args[1]; cnt="";
        }else if(args.Count==3){
            if(sh.io.pipedText!=null){ val=sh.io.pipedText; st=args[1]; cnt=args[2]; }
            else{ val=args[1]; st=args[2]; cnt=""; }
        }else if(args.Count==4){
            val=args[1]; st=args[2]; cnt=args[3];
        }else return sh.io.Error(usage);
        int s,n;
        float f;
        if(!float.TryParse(st,out f)) return sh.io.Error("数値の指定が不正です");
        s=(int)f;
        if(s<0) s=val.Length+s;
        if(s<0) return sh.io.Error("数値の指定が不正です");
        if(s>=val.Length) return 0;
        if(cnt=="") { sh.io.PrintLn(val.Substring(s)); return 0;}

        if(!float.TryParse(cnt,out f)||f<=0) return sh.io.Error("数値の指定が不正です");
        n=(int)f;
        if(s+n>val.Length) sh.io.PrintLn(val.Substring(s));
        else sh.io.PrintLn(val.Substring(s,n));
        return 0;
    }
    private static int CmdRepeat(ComShInterpreter sh,List<string> args){
        if(args.Count!=3) return sh.io.Error("使い方: repeat 回数 コマンド");
        if(!float.TryParse(args[1],out float f)) return sh.io.Error("数値の指定が不正です");
        int n=(int)f;
        if(n<=0) return 0;

        var psr=EvalParser(sh,2);
        if(psr==null) return -1;

        // 現シェルでの実行だが、出力だけはサブシェル実行と同じ形にする
        ComShInterpreter.Output orig=sh.io.output;
        var subout=new ComShInterpreter.SubShOutput();
        sh.io.output=new ComShInterpreter.Output(subout.Output);
        int ret=0;
        for(int i=0; i<n; i++){
            sh.env["_1"]=i.ToString();
            sh.env["_2"]=(i+1).ToString();
            psr.Reset();
            ret=sh.InterpretParser(psr);
            if(ret<0 || sh.exitq){ ret=sh.io.exitStatus; sh.exitq=false; break; }
        }
        sh.io.output=orig;
        if(ret>=0) sh.io.Print(subout.GetSubShResult());
        return ret;
    }

    public class PubSubEntry {
        public static int seq=0;
        public string id;
        public ComShInterpreter sh;
        public ComShParser parser;
        public PubSubEntry(ComShInterpreter sh){this.sh=sh;this.id=(seq++).ToString();}
        public int Parse(int idx){
            parser=EvalParser(sh,idx);
            if(parser==null) return -1;
            return 0;
        }
        public void Invoke(string msg){
            ComShInterpreter.Output orig=sh.io.output;
            var subout=new ComShInterpreter.SubShOutput();
            sh.io.output=new ComShInterpreter.Output(subout.Output);
            int ret=0;
            sh.env["_1"]=msg;
            parser.Reset();
            ret=sh.InterpretParser(parser);
            if(ret<0 || sh.exitq){ ret=sh.io.exitStatus; sh.exitq=false; }
            sh.io.output=orig;
            if(ret>=0) sh.io.Print(subout.GetSubShResult());
        }
    }
    private static Dictionary<string,LinkedList<PubSubEntry>> pubsubdic=new Dictionary<string,LinkedList<PubSubEntry>>();
    private static int CmdSubscribe(ComShInterpreter sh,List<string> args){
        if(args.Count!=3) return sh.io.Error("使い方: subscribe トピック名 コマンド");
        string key=args[1];
        if(!IsKvsNameValid(key)) return sh.io.Error("トピック名が不正です");
        LinkedList<PubSubEntry> lst;
        if(!pubsubdic.TryGetValue(key,out lst)){
            lst=new LinkedList<PubSubEntry>();
            pubsubdic[key]=lst;
        }
        var pse=new PubSubEntry(sh);
        if(pse.Parse(2)<0) return -1;
        lst.AddLast(pse);
        sh.io.Print(pse.id.ToString());
        return 0;
    }
    private static int CmdUnSubscribe(ComShInterpreter sh,List<string> args){
        if(args.Count<3) return sh.io.Error("使い方: unsubscribe トピック名 ID...");
        string key=args[1];
        if(!IsKvsNameValid(key)) return sh.io.Error("トピック名が不正です");
        LinkedList<PubSubEntry> lst;
        if(!pubsubdic.TryGetValue(key,out lst)) return sh.io.Error("トピックが定義されていません");
        if(lst.Count==0) return sh.io.Error("そのIDは登録されていません");
        for(int i=2; i<args.Count; i++){
            if(args[i]=="*"){ lst.Clear(); break; }
            var node=lst.First;
            while(node!=null){
                var next=node.Next;
                if(node.Value.id==args[i]){lst.Remove(node); break;}
                node=next;
            }
        }
        if(lst.Count==0) pubsubdic.Remove(key);
        return 0;
    }
    private static int CmdPublish(ComShInterpreter sh,List<string> args){
        if(args.Count!=2&&args.Count!=3) return sh.io.Error("使い方: publish トピック名 [メッセージ]");
        string key,msg;
        if(args.Count==2){key=args[1];msg="";}
        else if(args.Count==3){key=args[1];msg=args[2];}
        else return sh.io.Error("使い方: publish トピック名 [メッセージ]");
        DoPublish(key,msg);
        return 0;
    }
    public static void DoPublish(string key,string msg){
        if(!pubsubdic.TryGetValue(key,out LinkedList<PubSubEntry> lst)) return;
        var node=lst.First;
        while(node!=null){
            var next=node.Next;
            node.Value.Invoke(msg);
            node=next;
        }
    }
    private static int CmdVars2Str(ComShInterpreter sh,List<string> args){
        if(args.Count<2) return sh.io.Error("使い方: vars2str 変数名...");
        for(int i=1; i<args.Count; i++){
            if(args[i].Length==0) return sh.io.Error("変数名が不正です");
            if(!char.IsLetter(args[i][0])) return sh.io.Error("グローバル/スタティックローカル/特殊変数は指定できません");
            sh.io.Print(args[i]).Print(":").PrintLn(sh.env[args[i]]);
        }
        return 0;
    }
    private static int CmdStr2Vars(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: str2vars 文字列");
        string text=args[1];
        for(int c0=0; c0<text.Length; c0++){
            int e0=text.IndexOf('\n',c0);
            if(e0<0) e0=text.Length;
            int d=text.IndexOf(':',c0);
            if(d<0) return sh.io.Error("書式が不正です");
            string key=text.Substring(c0,d-c0);
            string val=text.Substring(d+1,e0-d-1);
            sh.env[key]=val;
            c0=e0;
        }
        return 0;
    }
    private static int CmdRefer(ComShInterpreter sh,List<string> args){
        const string usage="使い方: ref 変数 参照される変数";
        if(args.Count!=3 || !ParseUtil.IsLVarName(args[1]) || ParseUtil.IsVar1Name(args[1]))
            return sh.io.Error(usage);
        bool g=ParseUtil.IsGVarName(args[2]);
        bool l=ParseUtil.IsLVarName(args[2]) &&  !ParseUtil.IsVar1Name(args[2]);

        if(g){
            sh.env.SetRef(args[1],args[2]);
        }else if(l){
            if(sh.env.IsRef(args[2])) return sh.io.Error("ref変数をさらにrefする事はできません");
            sh.env.SetRef(args[1],args[2]);
        }else return sh.io.Error("その変数はrefの対象にできません");
        return 0;
    }
    private static int CmdIsRef(ComShInterpreter sh,List<string> args){
        const string usage="使い方: isref 変数名";
        if(args.Count!=2) return sh.io.Error(usage);
        sh.io.Print(sh.env.IsRef(args[1])?"1":"0");
        return 0;
    }
    private static int CmdStatic(ComShInterpreter sh,List<string> args){
        const string usage="使い方: static 変数[=初期値] ...";
        if(args.Count==1) return sh.io.Error(usage);
        for(int i=1; i<args.Count; i++){
            var key=ParseUtil.LeftOf(args[i],'=');
            if(!ParseUtil.IsLVarName(key)||ParseUtil.IsVar1Name(key)) return sh.io.Error("変数名が不正です");
            if(sh.env.IsRef(key)) sh.env.Remove(" "+key); // 上書きする場合は匿名変数の実体は消す
            sh.env.SetRef(key," "+key); // 匿名変数のrefを作るだけ
        }
        for(int i=1; i<args.Count; i++){
            if(args[i].IndexOf('=')<=0) continue;
            sh.Interpret(args[i]);  // 初期化
        }
        return 0;
    }
    private static int CmdBind(ComShInterpreter sh,List<string> args){
        const string usage="使い方: bind 変数 オブジェクト 属性 [副属性]";
        if(args.Count!=4&&args.Count!=5) return sh.io.Error(usage);
        string key=args[1];
        if(!ParseUtil.IsLVarName(key)||ParseUtil.IsVar1Name(key))
            return sh.io.Error("変数名が不正です");
        if(sh.env.IsRef(key)) sh.env.Remove(" "+key); // 上書きする場合は匿名変数の実体は消す
        var cd=new ParseUtil.ColonDesc(args[2]);
        Transform tr=ObjUtil.FindObj(sh,cd);
        if(tr==null) return sh.io.Error("オブジェクトが存在しません");
        string type=args[3],sub=(args.Count==5)?args[4]:"";
        ReferredVal.GetValue g=null,s=null;
        switch(type){
        case "wposrot":
            g=new ReferredVal.GetValue((string v0,out string v1)=>{
                v1=v0; if(tr==null) return -1;
                v1=sh.fmt.FPosRot(tr.position,tr.rotation.eulerAngles);
                return 0;
            });
            s=new ReferredVal.GetValue((string v0,out string v1)=>{
                v1=v0; if(tr==null) return -1;
                float[] fa=ParseUtil.FloatArr(v0);
                if(fa==null||fa.Length!=6) return 1;
                tr.position=new Vector3(fa[0],fa[1],fa[2]);
                tr.rotation=Quaternion.Euler(fa[3],fa[4],fa[5]);
                return 0;
            });
            break;
        case "wpos":
            if(sub==""){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FPos(tr.position);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    float[] xyz=ParseUtil.Xyz(v0);
                    if(xyz==null) return 1;
                    tr.position=new Vector3(xyz[0],xyz[1],xyz[2]);
                    return 0;
                });
            }else if(sub=="x"){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FVal(tr.position.x);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    if(!float.TryParse(v0,out float f)) return 1;
                    var pos=tr.position; pos.x=f; tr.position=pos;
                    return 0;
                });
            }else if(sub=="y"){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FVal(tr.position.y);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    if(!float.TryParse(v0,out float f)) return 1;
                    var pos=tr.position; pos.y=f; tr.position=pos;
                    return 0;
                });
            }else if(sub=="z"){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FVal(tr.position.z);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    if(!float.TryParse(v0,out float f)) return 1;
                    var pos=tr.position; pos.z=f; tr.position=pos;
                    return 0;
                });
            }
            break;
        case "wrot":
            if(sub==""){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FPos(tr.rotation.eulerAngles);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    float[] xyz=ParseUtil.Xyz(v0);
                    if(xyz==null) return 1;
                    tr.rotation=Quaternion.Euler(xyz[0],xyz[1],xyz[2]);
                    return 0;
                });
            }else if(sub=="x"){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FVal(tr.rotation.eulerAngles.x);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    if(!float.TryParse(v0,out float f)) return 1;
                    var rot=tr.rotation.eulerAngles; rot.x=f; tr.rotation=Quaternion.Euler(rot);
                    return 0;
                });
            }else if(sub=="y"){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FVal(tr.rotation.eulerAngles.y);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    if(!float.TryParse(v0,out float f)) return 1;
                    var rot=tr.rotation.eulerAngles; rot.y=f; tr.rotation=Quaternion.Euler(rot);
                    return 0;
                });
            }else if(sub=="z"){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FVal(tr.rotation.eulerAngles.z);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    if(!float.TryParse(v0,out float f)) return 1;
                    var rot=tr.rotation.eulerAngles; rot.z=f; tr.rotation=Quaternion.Euler(rot);
                    return 0;
                });
            }
            break;
        case "lposrot":
            g=new ReferredVal.GetValue((string v0,out string v1)=>{
                v1=v0; if(tr==null) return -1;
                v1=sh.fmt.FPosRot(tr.localPosition,tr.localRotation.eulerAngles);
                return 0;
            });
            s=new ReferredVal.GetValue((string v0,out string v1)=>{
                v1=v0; if(tr==null) return -1;
                float[] fa=ParseUtil.FloatArr(v0);
                if(fa==null||fa.Length!=6) return 1;
                tr.localPosition=new Vector3(fa[0],fa[1],fa[2]);
                tr.localRotation=Quaternion.Euler(fa[3],fa[4],fa[5]);
                return 0;
            });
            break;
        case "lpos":
            if(sub==""){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FPos(tr.localPosition);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    float[] xyz=ParseUtil.Xyz(v0);
                    if(xyz==null) return 1;
                    tr.localPosition=new Vector3(xyz[0],xyz[1],xyz[2]);
                    return 0;
                });
            }else if(sub=="x"){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FVal(tr.localPosition.x);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    if(!float.TryParse(v0,out float f)) return 1;
                    var pos=tr.localPosition; pos.x=f; tr.localPosition=pos;
                    return 0;
                });
            }else if(sub=="y"){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FVal(tr.localPosition.y);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    if(!float.TryParse(v0,out float f)) return 1;
                    var pos=tr.localPosition; pos.y=f; tr.localPosition=pos;
                    return 0;
                });
            }else if(sub=="z"){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FVal(tr.localPosition.z);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    if(!float.TryParse(v0,out float f)) return 1;
                    var pos=tr.localPosition; pos.z=f; tr.localPosition=pos;
                    return 0;
                });
            }
            break;
        case "lrot":
            if(sub==""){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FPos(tr.localRotation.eulerAngles);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    float[] xyz=ParseUtil.Xyz(v0);
                    if(xyz==null) return 1;
                    tr.localRotation=Quaternion.Euler(xyz[0],xyz[1],xyz[2]);
                    return 0;
                });
            }else if(sub=="x"){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FVal(tr.localRotation.eulerAngles.x);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    if(!float.TryParse(v0,out float f)) return 1;
                    var rot=tr.localRotation.eulerAngles; rot.x=f; tr.localRotation=Quaternion.Euler(rot);
                    return 0;
                });
            }else if(sub=="y"){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FVal(tr.localRotation.eulerAngles.y);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    if(!float.TryParse(v0,out float f)) return 1;
                    var rot=tr.localRotation.eulerAngles; rot.y=f; tr.localRotation=Quaternion.Euler(rot);
                    return 0;
                });
            }else if(sub=="z"){
                g=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    v1=sh.fmt.FVal(tr.localRotation.eulerAngles.z);
                    return 0;
                });
                s=new ReferredVal.GetValue((string v0,out string v1)=>{
                    v1=v0; if(tr==null) return -1;
                    if(!float.TryParse(v0,out float f)) return 1;
                    var rot=tr.localRotation.eulerAngles; rot.z=f; tr.localRotation=Quaternion.Euler(rot);
                    return 0;
                });
            }
            break;
        case "scale":
            g=new ReferredVal.GetValue((string v0,out string v1)=>{
                v1=v0; if(tr==null) return -1;
                v1=sh.fmt.FPos(tr.localScale);
                return 0;
            });
            s=new ReferredVal.GetValue((string v0,out string v1)=>{
                v1=v0; if(tr==null) return -1;
                float[] xyz=ParseUtil.Xyz(v0);
                if(xyz==null) return 1;
                tr.localScale=new Vector3(xyz[0],xyz[1],xyz[2]);
                return 0;
            });
            break;
        default:
            return sh.io.Error("属性が不正です");
        }
        sh.env.SetBind(key,g,s);
        return 0;
    }

    private const string escapechr=" \\\"\'$;|>#";
    private static int CmdEscape(ComShInterpreter sh,List<string> args){
        string val,ltr=escapechr;
        if(args.Count==3){ val=args[1]; ltr=args[2];}
        else if(args.Count==2){
            if(sh.io.pipedText!=null){ val=sh.io.pipedText; ltr=args[1]; } else val=args[1];
        }else if(args.Count==1 && sh.io.pipedText!=null) val=sh.io.pipedText;
        else return sh.io.Error("使い方: escape 文字列 [対象文字]");
        char[] buf=new char[val.Length*2];
        int bi=0;
        for(int i=0; i<val.Length; i++){
            for(int j=0; j<ltr.Length; j++) if(val[i]==ltr[j]){ buf[bi++]='\\'; break; }
            buf[bi++]=val[i];
        }
        sh.io.Print(buf,0,bi);
        return 0;
    }
    private static int CmdSort(ComShInterpreter sh,List<string> args){
        string val;
        if(args.Count==1){
            if(sh.io.pipedText!=null) val=sh.io.pipedText;
            else return sh.io.Error("使い方: sort 文字列");
        } else val=args[1];
        var sa=val.Split('\n');
        Array.Sort(sa);
        for(int i=0; i<sa.Length; i++) sh.io.PrintLn(sa[i]);
        return 0;
    }
    private static int CmdUniq(ComShInterpreter sh,List<string> args){
        const string usage="使い方: uniq 文字列 [区切り文字]";
        string val;
        char dlmt='\n';
        if(args.Count==1){
            if(sh.io.pipedText!=null) val=sh.io.pipedText;
            else return sh.io.Error(usage);
        }else if(args.Count==2){
            if(sh.io.pipedText!=null){
                if(args[1].Length==0) return sh.io.Error(usage);
                val=sh.io.pipedText;
                dlmt=args[1][0];
            }else val=args[1];
            val=args[1];
        }else if(args.Count==3){
            val=args[1];
            dlmt=args[2][0];
        }else return sh.io.Error(usage);
        var sa=val.Split(dlmt);
        var set=new HashSet<string>();
        if(sa.Length>0){
            set.Add(sa[0]); sh.io.Print(sa[0]);
            for(int i=1; i<sa.Length; i++) if(set.Add(sa[i])) sh.io.Print(dlmt).Print(sa[i]);
        }
        return 0;
    }
    private static int CmdRgb2Hsv(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: rgb2hsv r,g,b");
        float[] col=ParseUtil.Rgb(args[1]);
        if(col==null) return sh.io.Error(ParseUtil.error);
        Color.RGBToHSV(new Color(col[0],col[1],col[2]),out float h,out float s,out float v);
        sh.io.PrintJoin(",",sh.fmt.F0to1(h),sh.fmt.F0to1(s),sh.fmt.F0to1(v));
        return 0;
    }
    private static int CmdHsv2Rgb(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: hsv2rgb h,s,v");
        float[] col=ParseUtil.Rgb(args[1]);
        if(col==null) return sh.io.Error(ParseUtil.error);
        Color rgb=Color.HSVToRGB(col[0],col[1],col[2]);
        sh.io.PrintJoin(",",sh.fmt.F0to1(rgb.r),sh.fmt.F0to1(rgb.g),sh.fmt.F0to1(rgb.b));
        return 0;
    }
    private static int CmdInDiv(ComShInterpreter sh,List<string> args){
        if(args.Count!=4) return sh.io.Error("使い方1: indiv 座標1 座標2 比\n使い方2: indiv 座標1 座標2 座標3");
        float[] p0={0,0,0};
        int n=ParseUtil.XyzSub(args[1],p0);
        if(n<=0||n>3) return sh.io.Error("座標が不正です");
        float[] p1={0,0,0};
        int n2=ParseUtil.XyzSub(args[2],p1);
        if(n2!=n) return sh.io.Error("座標が不正または次数が一致していません");
        float t=0;
        float[] p2=null;
        if(args[3].IndexOf(',')>=0){
            p2=new float[]{0,0,0};
            n2=ParseUtil.XyzSub(args[3],p2);
            if(n2!=n) return sh.io.Error("座標が不正または次数が一致していません");
        } else if(!float.TryParse(args[3],out t)) return sh.io.Error("数値が不正です");
        if(p2==null){
            if(n==3) sh.io.Print(sh.fmt.FPos((p1[0]-p0[0])*t+p0[0],(p1[1]-p0[1])*t+p0[1],(p1[2]-p0[2])*t+p0[2]));
            else if(n==2) sh.io.Print(sh.fmt.FXY((p1[0]-p0[0])*t+p0[0],(p1[1]-p0[1])*t+p0[1]));
            else sh.io.Print(sh.fmt.FVal((p1[0]-p0[0])*t+p0[0]));
        }else{
            if(n==3){
                var nv=new Vector3(p1[0]-p0[0],p1[1]-p0[1],p1[2]-p0[2]);
                var ll=nv.sqrMagnitude;
                var sl=new Vector3(p2[0]-p0[0],p2[1]-p0[1],p2[2]-p0[2]);
                sh.io.Print(sh.fmt.F0to1(Vector3.Dot(nv,sl)/ll));
            }else if(n==2){
                var v0=new Vector2(p1[0]-p0[0],p1[1]-p0[1]);
                var ll=v0.sqrMagnitude;
                var v1=new Vector2(p2[0]-p0[0],p2[1]-p0[1]);
                sh.io.Print(sh.fmt.F0to1(Vector2.Dot(v0,v1)/ll));
            }else{
                sh.io.Print(sh.fmt.FVal((p2[0]-p0[0])/(p1[0]-p0[0])));
            }
        }
        return 0;
    }
    private static int CmdLogBX(ComShInterpreter sh,List<string> args){
        if(args.Count!=3) return sh.io.Error("使い方:logbx 基数 真数");
        if( !float.TryParse(args[1],out float b)||b<=0
         || !float.TryParse(args[2],out float x)||x<=0 ) return sh.io.Error("数値が不正です");
        sh.io.Print(sh.fmt.FVal(Mathf.Log(x,b)));
        return 0;
    }
    private static int CmdPow(ComShInterpreter sh,List<string> args){
        if(args.Count!=3) return sh.io.Error("使い方:pow 底 指数");
        if( !float.TryParse(args[1],out float b)
         || !float.TryParse(args[2],out float x) ) return sh.io.Error("数値が不正です");
        sh.io.Print(sh.fmt.FVal(Math.Pow(b,x)));
        return 0;
    }
    private static int CmdAllHandleClear(ComShInterpreter sh,List<string> args){
        ComShHandle.Clear();
        return 0;
    }
    private static int CmdMouse(ComShInterpreter sh,List<string> args){
        var b1=Input.GetMouseButtonDown(0)?"2":(Input.GetMouseButtonUp(0)?"-1":(Input.GetMouseButton(0)?"1":"0"));
        var b2=Input.GetMouseButtonDown(1)?"2":(Input.GetMouseButtonUp(1)?"-1":(Input.GetMouseButton(1)?"1":"0"));
        var b3=Input.GetMouseButtonDown(2)?"2":(Input.GetMouseButtonUp(2)?"-1":(Input.GetMouseButton(2)?"1":"0"));
        var h=GameMain.Instance.MainCamera.camera.pixelHeight;
        var pos=Input.mousePosition;
        int x=(int)(pos.x), y=(int)(h-1-pos.y);
        sh.io.PrintJoin(sh.ofs,x.ToString()+","+y.ToString(),b1,b2,b3);
        return 0;
    }
    private static int CmdKeyPress(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方:keypress キー");
        try{
            var k=Input.GetKey(args[1]);
            if(k){
                var down=Input.GetKeyDown(args[1]);
                sh.io.Print(down?"2":"1"); 
            }else{
                var up=Input.GetKeyUp(args[1]);
                sh.io.Print(up?"-1":"0"); 
            }
        }catch{ return sh.io.Error("キーの指定が不正です"); }
        return 0;
    }
    private static int CmdKeyAxis(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方:axis 軸");
        try{
            var val=Input.GetAxis(args[1]);
            sh.io.Print(sh.fmt.FVal(val));
        }catch{ return sh.io.Error("軸の指定が不正です"); }
        return 0;
    }
    private static int CmdSmoothDamp(ComShInterpreter sh,List<string> args){
        if(args.Count!=7) return sh.io.Error("使い方:smoothdamp 現在値 目標値 現在速度 目標時間 最大速度 経過時間");
        float[] fa=ParseUtil.FloatArr(args[1]);
        if(fa==null) return sh.io.Error(ParseUtil.error);
        int n=fa.Length;
        float[] fa2=ParseUtil.FloatArr(args[2]);
        if(fa2==null) return sh.io.Error(ParseUtil.error);
        if(n!=fa2.Length) return sh.io.Error("次数が一致しません");
        if( !float.TryParse(args[4],out float period) || period<0
         || !float.TryParse(args[5],out float maxspd)
         || !float.TryParse(args[6],out float delta) || delta<0) return sh.io.Error("数値が不正です");

        if(n==1){
            if(!float.TryParse(args[3],out float speed)) return sh.io.Error("数値が不正です");
            var ret=Mathf.SmoothDamp(fa[0],fa2[0],ref speed,period,maxspd,delta);
            sh.io.PrintJoin(sh.ofs,sh.fmt.FVal(ret),sh.fmt.FVal(speed));
        }else if(n==2){
            float[] xy=ParseUtil.Xy(args[3]);
            if(xy==null) return sh.io.Error(ParseUtil.error);
            Vector2 speed2=new Vector2(xy[0],xy[1]);
            var ret=Vector2.SmoothDamp(new Vector2(fa[0],fa[1]),new Vector2(fa2[0],fa2[1]),ref speed2,period,maxspd,delta);
            sh.io.PrintJoin(sh.ofs,sh.fmt.FXY(ret),sh.fmt.FXY(speed2));
        }else if(n==3){
            float[] xyz=ParseUtil.Xyz(args[3]);
            if(xyz==null) return sh.io.Error(ParseUtil.error);
            Vector3 speed3=new Vector3(xyz[0],xyz[1],xyz[2]);
            var ret=Vector3.SmoothDamp(new Vector3(fa[0],fa[1],fa[2]),new Vector3(fa2[0],fa2[1],fa2[2]),ref speed3,period,maxspd,delta);
            sh.io.PrintJoin(sh.ofs,sh.fmt.FPos(ret),sh.fmt.FPos(speed3));
        }else return sh.io.Error("スカラと2～3次元ベクトルのみ使用できます");
        return 0;
    }
    private static int CmdLayerHit(ComShInterpreter sh,List<string> args){
        const string usage="使い方: layerhit レイヤ番号1 レイヤ番号2 0|1";
        if(args.Count!=3 && args.Count!=4) return sh.io.Error(usage);
        if(!int.TryParse(args[1],out int l1) || l1<0 || l1>31
         ||!int.TryParse(args[2],out int l2) || l2<0 || l2>31) return sh.io.Error("レイヤ番号が不正です");
        if(args.Count==3){
            bool ign=Physics.GetIgnoreLayerCollision(l1,l2);
            sh.io.Print(ign?"0":"1");
        }else{
            if(args[3]!="0"&&args[3]!="1") return sh.io.Error("値が不正です");
            Physics.IgnoreLayerCollision(l1,l2,args[3]=="0");
        }
        return 0;
    }
    private static int CmdPhysicsParam(ComShInterpreter sh,List<string> args){
        if(args.Count==1){
            sh.io.PrintLn2("gravity:", sh.fmt.FPos(Physics.gravity))
             .PrintLn2("hitmargin:",sh.fmt.FVal(Physics.defaultContactOffset))
             .PrintLn2("accuracy:",sh.fmt.FVal(Physics.defaultSolverIterations))
             .PrintLn2("deltatime:",sh.fmt.FXY(Time.fixedDeltaTime,Time.maximumDeltaTime));
             return 0;
        }
        for(int i=1; i<args.Count; i++){
            var lr=ParseUtil.LeftAndRight(args[i],'=');
            float f;
            float[] fa;
            switch(lr[0]){
            case "gravity":
                fa=ParseUtil.Xyz(lr[1]);
                if(fa==null) return sh.io.Error("ベクトルが不正です");
                Physics.gravity=new Vector3(fa[0],fa[1],fa[2]);
                break;
            case "hitmargin":
                if(!float.TryParse(lr[1],out f)||f<=0) return sh.io.Error("数値が不正です");
                Physics.defaultContactOffset=f;
                break;
            case "accuracy":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=2||(int)fa[0]<=0) return sh.io.Error("書式が不正です");
                Physics.defaultSolverIterations=(int)fa[0];
                Physics.defaultSolverVelocityIterations=(int)(fa[1]/6);
                break;
            case "deltatime":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=2||fa[0]<=0||fa[1]<=0) return sh.io.Error("書式が不正です");
                Time.fixedDeltaTime=fa[0];
                Time.maximumDeltaTime=fa[1];
                break;
            default:
                return sh.io.Error("プロパティ名が不正です");
            }
        }
        return 0;
    }
    private static int CmdFindCollider(ComShInterpreter sh,List<string> args){
        const string usage="使い方: findcollider 中心 サイズ [回転] [レイヤマスク]";
        if(args.Count!=3&&args.Count!=4&&args.Count!=5) return sh.io.Error(usage);
        float[] fa=ParseUtil.Xyz(args[1]);
        if(fa==null) return sh.io.Error(ParseUtil.error);
        Vector3 center=new Vector3(fa[0],fa[1],fa[2]);
        fa=ParseUtil.FloatArr(args[2]);
        if(fa==null) return sh.io.Error(ParseUtil.error);
        int n=fa.Length;
        Vector3 boxsize=Vector3.zero;
        float radius=0;
        if(n==3) boxsize=new Vector3(fa[0],fa[1],fa[2]);
        else if(n==1) radius=fa[0];
        else return sh.io.Error("サイズが不正です");

        int bits=-1;
        Quaternion q=Quaternion.identity;
        if(args.Count>=4){
            fa=ParseUtil.Xyz(args[3]);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            q=Quaternion.Euler(fa[0],fa[1],fa[2]);
            if(args.Count==5){
                if(!int.TryParse(args[4],System.Globalization.NumberStyles.HexNumber,null,out bits)) return sh.io.Error("レイヤマスクが不正です");
            }
        }
        Collider[] ca;
        if(n==3) ca=Physics.OverlapBox(center,boxsize,q,bits,QueryTriggerInteraction.Collide);
        else ca=Physics.OverlapSphere(center,radius,bits,QueryTriggerInteraction.Collide);
        for(int i=0; i<ca.Length; i++) sh.io.PrintLn(UTIL.Tr2Name(sh,ca[i].transform));
        return 0;
    }
    private static string outputlogname=ComShInterpreter.homeDir+@"COM3D2x64_Data\output_log.txt";
    private static int CmdOutputLog(ComShInterpreter sh,List<string> args){
        if(!File.Exists(outputlogname)) return sh.io.Error("output_log.txtが見つかりません");
        const string usage="使い方: outputlog 文字数";
        if(args.Count!=2) return sh.io.Error(usage);
        if(!int.TryParse(args[1],out int count)||count==0) return sh.io.Error(usage);
        bool tailq=false;
        if(count<0){ tailq=true; count*=-1;}

        char[] buf=new char[count];
        try{
            FileStream fs;
            using(var sr=new StreamReader(fs=new FileStream(outputlogname,FileMode.Open,FileAccess.Read,FileShare.ReadWrite))){
                if(tailq){
                    fs.Seek(-count,System.IO.SeekOrigin.End);
                    count=sr.Read(buf,0,count);
                }else{
                    fs.Seek(0,System.IO.SeekOrigin.Begin);
                    count=sr.Read(buf,0,count);
                }
                if(buf[count-1]=='\n') count--;
                if(buf[count-1]=='\r') count--;
                sh.io.Print(buf,0,count);
            }
        }catch(Exception e){ return sh.io.Error($"失敗しました({e.Message})");}
        return 0;
    }

    private static string tagdir=ComShInterpreter.scriptFolder+@"tagconfig\";
    private static int CmdTag(ComShInterpreter sh,List<string> args){
        const string usage="使い方: tag カテゴリ [タグ ...]";
        if(args.Count<2) return sh.io.Error(usage);
        try{
            string file=tagdir+UTIL.Suffix(args[1],".tsv");
            if(!File.Exists(file)) return sh.io.Error("そのカテゴリ用の設定ファイルはありません");
            using(var sr=new StreamReader(File.OpenRead(file))){
                if(args.Count==2){ // 引数2つ: タグの一覧
                    var tagset=new HashSet<string>();
                    while(sr.Peek()>-1){
                        string l=sr.ReadLine();
                        string[] sa=l.Split(ParseUtil.tab);
                        for(int i=1; i<sa.Length; i++) tagset.Add(sa[i]);
                    }
                    string[] tags=new string[tagset.Count];
                    tagset.CopyTo(tags);
                    Array.Sort(tags);
                    for(int i=0; i<tags.Length; i++) sh.io.PrintLn(tags[i]);
                }else{  // 引数3つ以上: タグによる検索
                    int cnt=0;
                    int max=GetSearchMax(sh);
                    while(sr.Peek()>-1){
                        string l=sr.ReadLine();
                        string[] sa=l.Split(ParseUtil.tab);
                        if(MatchTag(sa,args,2)){
                            sh.io.PrintLn(sa[0]);
                            cnt++;
                            if(cnt>=max) break;
                        }
                    }
                }
            }
        }catch{ return sh.io.Error("設定ファイルの読み込みに失敗しました"); }
        return 0;
    }
    private static bool MatchTag(string[] sa,List<string> args,int start){
        // タグ検索。AND条件のみ
        int i,j;
        for(i=start; i<args.Count; i++){ // 条件側
            for(j=1; j<sa.Length;  j++)  // 設定側
                if(args[i]==sa[j]) break;
            if(j==sa.Length) return false; // ANDなのでなければ不一致
        }
        return true;
    }
    private static int GetSearchMax(ComShInterpreter sh){
        string srm=sh.env[ComShInterpreter.SEARCH_RESULT_MAX];
        if(srm!="" && int.TryParse(srm,out int ret) && ret>0 ) return ret;
        return 20;
    }
    private static int CmdGlobalSkyBox(ComShInterpreter sh,List<string> args){
        const string usage="使い方: skybox テクスチャファイル名";
        if(args.Count>2) return sh.io.Error(usage);
        if(args.Count==1){
            if(RenderSettings.skybox==null) return 0;
            sh.io.Print(RenderSettings.skybox.name);
            return 0;
        }
        string file=args[1];
        string[] lr=ParseUtil.LeftAndRight2(file,':');
        if(lr==null) return sh.io.Error("書式が不正です");
        string texab=lr[0];
        file=lr[1];
        if(file==""){
            if(texab==""){
                RenderSettings.skybox=null;
                DynamicGI.UpdateEnvironment();
                Resources.UnloadUnusedAssets();
                return 1;
            }
            List<string> sa=ObjUtil.ListAssetBundle<Cubemap>(texab,ComShInterpreter.textureDir);
            if(sa!=null) foreach(var s in sa) sh.io.PrintLn(s);
            return 0;
        }
        int ret=SetupSkyBoxMate(sh,texab,file,out Material mate);
        if(ret<0) return ret;
        RenderSettings.skybox=mate;
        Cubemap cm=(Cubemap)mate.GetTexture("_Tex");
        RenderSettings.defaultReflectionResolution=cm.width;
        RenderSettings.defaultReflectionMode=UnityEngine.Rendering.DefaultReflectionMode.Custom;
        RenderSettings.customReflection=cm;
        DynamicGI.UpdateEnvironment();
        return 0;
    }
    public static int SetupSkyBoxMate(ComShInterpreter sh,string ab,string name,out Material mate){
        mate=ObjUtil.LoadAssetBundle<Material>("skybox","skybox",ComShInterpreter.scriptFolder+@"asset\");
        if(mate==null) return sh.io.Error("失敗しました");
        mate.name=name;
        Cubemap cm;
        if(ab!=""){
            cm=ObjUtil.LoadAssetBundle<Cubemap>(ab,name,ComShInterpreter.textureDir);
            if(cm==null) return sh.io.Error("ファイルが見つかりません");
        }else{
            Texture tex0=TextureUtil.ReadTexture(name);
            if(tex0==null) return sh.io.Error("ファイルが見つかりません");
            try{ cm=TextureUtil.T2DToCube((Texture2D)tex0,1); }catch{ return sh.io.Error("テクスチャの形式が不正です"); }
        }
        var old=mate.GetTexture("_Tex");
        if(old!=null && CmdMeshes.texiid.Remove(old)) UnityEngine.Object.Destroy(old); 
        mate.SetTexture("_Tex",cm);
        CmdMeshes.texiid.Add(cm);
        return 1;
    }
    private static int CmdGlobalSkyBoxColor(ComShInterpreter sh,List<string> args){
        Material sb=RenderSettings.skybox;
        if(args.Count==1){
            if(sb==null) return 0;
            var c=sb.GetColor("_Tint");
            sh.io.Print(sh.fmt.RGBA(c));
            return 0;
        }else if(args.Count>2) return sh.io.Error("引数が多すぎます");
        var col=ParseUtil.Rgba(args[1]);
        if(col==null) return sh.io.Error(ParseUtil.error);
        sb.SetColor("_Tint",new Color(col[0],col[1],col[2],col[3]));
        return 0;
    }
    private static int CmdGlobalSkyBoxPower(ComShInterpreter sh,List<string> args){
        Material sb=RenderSettings.skybox;
        if(args.Count==1){
            if(sb==null) return 0;
            float e=sb.GetFloat("_Exposure");
            sh.io.Print(sh.fmt.FInt(e));
            return 0;
        }else if(args.Count>2) return sh.io.Error("引数が多すぎます");
        if(!float.TryParse(args[1],out float f)||f<0) return sh.io.Error("数値が不正です");
        sb.SetFloat("_Exposure",f);
        return 0;
    }
    private static int CmdGlobalSkyBoxRotation(ComShInterpreter sh,List<string> args){
        Material sb=RenderSettings.skybox;
        if(args.Count==1){
            if(sb==null) return 0;
            float e=sb.GetFloat("_Rotation");
            sh.io.Print(sh.fmt.FInt(e));
            return 0;
        }else if(args.Count>2) return sh.io.Error("引数が多すぎます");
        if(!float.TryParse(args[1],out float f)) return sh.io.Error("数値が不正です");
        sb.SetFloat("_Rotation",f);
        return 0;
    }
    private static int CmdNumFormat(ComShInterpreter sh,List<string> args){
        if(args.Count!=3) return sh.io.Error("使い方: numformat 10進数値 書式指定");
        string ret;
        if(args[1].IndexOf('.')>=0){
            if(!double.TryParse(args[1],out double d)) return sh.io.Error("数値が不正です");
            try{ret=d.ToString(args[2]);}catch{return sh.io.Error("書式が不正です");}
        }else{
            if(!ulong.TryParse(args[1],out ulong n)) return sh.io.Error("数値が不正です");
            try{ret=n.ToString(args[2]);}catch{return sh.io.Error("書式が不正です");}
            sh.io.Print(n.ToString(args[2]));
        }
        sh.io.Print(ret);
        return 0;
    }
    private static int CmdToHex(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: tohex 10進数値");
        if(!ulong.TryParse(args[1],out ulong n)) return sh.io.Error("数値が不正です");
        sh.io.Print(n.ToString("X"));
        return 0;
    }
    private static int CmdToDec(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: todec 16進数値");
        if(!ulong.TryParse(args[1],System.Globalization.NumberStyles.HexNumber,null,out ulong n)) return sh.io.Error("数値が不正です");
        sh.io.Print(n.ToString());
        return 0;
    }

    public delegate int CmdParam<T>(ComShInterpreter sh,T m,string val);
    public static int currentArgNo=0;
    public static int ParamLoop<T>(ComShInterpreter sh,T tgt,Dictionary<string,CmdParam<T>> dic,List<string> args,int prmstart){
        int cnt=args.Count;
        int odd=(cnt-prmstart)%2;
        if(odd>0) cnt--;
        for(int i=prmstart; i<cnt; i+=2){
            currentArgNo=i;
            if(!dic.TryGetValue(args[i],out CmdParam<T> func)) return sh.io.Error("不正なパラメータです");
            int ret=func.Invoke(sh,tgt,args[i+1]);
            if(ret<=0) return ret;
        }
        if(odd>0){
            currentArgNo=cnt;
            if(!dic.TryGetValue(args[cnt],out CmdParam<T> func)) return sh.io.Error("不正なパラメータです");
            int ret=func.Invoke(sh,tgt,null);
            if(ret<=0) return ret;
        }
        return 0;
    }

    public static void CmdParamPosRotCp<T>(Dictionary<string,CmdParam<T>> dic,string from,string to){
        dic.Add(to,dic[from]);
        dic.Add(to+".x",dic[from+".x"]);
        dic.Add(to+".y",dic[from+".y"]);
        dic.Add(to+".z",dic[from+".z"]);
    }

    // wpos,lpos,wrot,lrotその他の共通処理
    public static int _CmdParamWPos(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            sh.io.Print(sh.fmt.FPos(tr.position));
            return 0;
        }
        float[] xyz=ParseUtil.XyzR(val,out bool rq);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        if(rq) tr.position=tr.position+(new Vector3(xyz[0],xyz[1],xyz[2]));
        else tr.position=new Vector3(xyz[0],xyz[1],xyz[2]);
        return 1;
    }
    public static int _CmdParamWPosX(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FVal(tr.position.x)); return 0; }
        if(val.Length==0) return sh.io.Error("数値を指定してください");
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        Vector3 pos=tr.position; pos.x=v; tr.position=pos;
        return 1;
    }
    public static int _CmdParamWPosY(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FVal(tr.position.y)); return 0; }
        if(val.Length==0) return sh.io.Error("数値を指定してください");
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        Vector3 pos=tr.position; pos.y=v; tr.position=pos;
        return 1;
    }
    public static int _CmdParamWPosZ(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FVal(tr.position.z)); return 0; }
        if(val.Length==0) return sh.io.Error("数値を指定してください");
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        Vector3 pos=tr.position; pos.z=v; tr.position=pos;
        return 1;
    }
    public static int _CmdParamLPos(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            sh.io.Print(sh.fmt.FPos(tr.localPosition));
            return 0;
        }
        float[] xyz=ParseUtil.XyzR(val,out bool rq);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        if(rq) tr.localPosition=tr.localPosition+(new Vector3(xyz[0],xyz[1],xyz[2]));
        else tr.localPosition=new Vector3(xyz[0],xyz[1],xyz[2]);
        return 1;
    }
    public static int _CmdParamLPosX(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FVal(tr.localPosition.x)); return 0; }
        if(val.Length==0) return sh.io.Error("数値を指定してください");
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        Vector3 pos=tr.localPosition; pos.x=v; tr.localPosition=pos;
        return 1;
    }
    public static int _CmdParamLPosY(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FVal(tr.localPosition.y)); return 0; }
        if(val.Length==0) return sh.io.Error("数値を指定してください");
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        Vector3 pos=tr.localPosition; pos.y=v; tr.localPosition=pos;
        return 1;
    }
    public static int _CmdParamLPosZ(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FVal(tr.localPosition.z)); return 0; }
        if(val.Length==0) return sh.io.Error("数値を指定してください");
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        Vector3 pos=tr.localPosition; pos.z=v; tr.localPosition=pos;
        return 1;
    }
    public static int _CmdParamLPosRot(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FPosRot(tr.localPosition,tr.localRotation.eulerAngles)); return 0; }
        float[] fa=ParseUtil.FloatArr(val);
        if(fa==null||fa.Length!=6) return sh.io.Error("書式が不正です");
        tr.localPosition=new Vector3(fa[0],fa[1],fa[2]);
        tr.localRotation=Quaternion.Euler(fa[3],fa[4],fa[5]);
        return 1;
    }
    public static int _CmdParamWPosRot(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FPosRot(tr.position,tr.rotation.eulerAngles)); return 0; }
        float[] fa=ParseUtil.FloatArr(val);
        if(fa==null||fa.Length!=6) return sh.io.Error("書式が不正です");
        tr.position=new Vector3(fa[0],fa[1],fa[2]);
        tr.rotation=Quaternion.Euler(fa[3],fa[4],fa[5]);
        return 1;
    }
    public static int _CmdParamWRot(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            sh.io.Print(sh.fmt.FEuler(tr.rotation.eulerAngles));
            return 0;
        }
        float[] xyz=ParseUtil.RotR(val,out byte rq);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        if(rq==1) tr.rotation=Quaternion.Euler(xyz[0],xyz[1],xyz[2])*tr.rotation;
        else if(rq==2) tr.rotation=tr.rotation*Quaternion.Euler(xyz[0],xyz[1],xyz[2]);
        else tr.rotation=Quaternion.Euler(xyz[0],xyz[1],xyz[2]);
        return 1;
    }
    public static int _CmdParamWRotX(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FInt(tr.rotation.eulerAngles.x)); return 0; }
        if(val.Length==0) return sh.io.Error("数値を指定してください");
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        tr.rotation=Quaternion.Euler(v,tr.rotation.eulerAngles.y,tr.rotation.eulerAngles.z);
        return 1;
    }
    public static int _CmdParamWRotY(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FInt(tr.rotation.eulerAngles.y)); return 0; }
        if(val.Length==0) return sh.io.Error("数値を指定してください");
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        tr.rotation=Quaternion.Euler(tr.rotation.eulerAngles.x,v,tr.rotation.eulerAngles.z);
        return 1;
    }
    public static int _CmdParamWRotZ(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FInt(tr.rotation.eulerAngles.z)); return 0; }
        if(val.Length==0) return sh.io.Error("数値を指定してください");
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        tr.rotation=Quaternion.Euler(tr.rotation.eulerAngles.x,tr.rotation.eulerAngles.y,v);
        return 1;
    }
    public static int _CmdParamLRot(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            sh.io.Print(sh.fmt.FEuler(tr.localRotation.eulerAngles));
            return 0;
        }
        float[] xyz=ParseUtil.RotR(val,out byte rq);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        if(rq==1) tr.localRotation=Quaternion.Euler(xyz[0],xyz[1],xyz[2])*tr.localRotation;
        else if(rq==2) tr.localRotation=tr.localRotation*Quaternion.Euler(xyz[0],xyz[1],xyz[2]);
        else tr.localRotation=Quaternion.Euler(xyz[0],xyz[1],xyz[2]);
        return 1;
    }
    public static int _CmdParamLRotX(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FInt(tr.localRotation.eulerAngles.x)); return 0; }
        if(val.Length==0) return sh.io.Error("数値を指定してください");
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        tr.localRotation=Quaternion.Euler(v,tr.localRotation.eulerAngles.y,tr.localRotation.eulerAngles.z);
        return 1;
    }
    public static int _CmdParamLRotY(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FInt(tr.localRotation.eulerAngles.y)); return 0; }
        if(val.Length==0) return sh.io.Error("数値を指定してください");
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        tr.localRotation=Quaternion.Euler(tr.localRotation.eulerAngles.x,v,tr.localRotation.eulerAngles.z);
        return 1;
    }
    public static int _CmdParamLRotZ(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FInt(tr.localRotation.eulerAngles.z)); return 0; }
        if(val.Length==0) return sh.io.Error("数値を指定してください");
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        tr.localRotation=Quaternion.Euler(tr.localRotation.eulerAngles.x,tr.localRotation.eulerAngles.y,v);
        return 1;
    }
    public static int _CmdParamOPos(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return 0;
        float[] xyz=ParseUtil.Xyz(val);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        tr.Translate(xyz[0],xyz[1],xyz[2],Space.Self);
        return 1;
    }
    public static int _CmdParamORot(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return 0;
        float[] xyz=ParseUtil.Xyz(val);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        tr.Rotate(xyz[0],xyz[1],xyz[2],Space.Self);
        return 1;
    }
    public static int _CmdParamScale(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            sh.io.Print(sh.fmt.FMul(tr.localScale));
            return 0;
        }
        float[] xyz=ParseUtil.Xyz2(val);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        tr.localScale=new Vector3(xyz[0],xyz[1],xyz[2]);
        return 1;
    }
    public static int _CmdParamScaleX(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FInt(tr.localScale.x)); return 0; }
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        Vector3 scl=tr.localScale; scl.x=v; tr.localScale=scl;
        return 1;
    }
    public static int _CmdParamScaleY(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FInt(tr.localScale.y)); return 0; }
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        Vector3 scl=tr.localScale; scl.y=v; tr.localScale=scl;
        return 1;
    }
    public static int _CmdParamScaleZ(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(sh.fmt.FInt(tr.localScale.z)); return 0; }
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        Vector3 scl=tr.localScale; scl.z=v; tr.localScale=scl;
        return 1;
    }
    public static int _CmdParamLQuat(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            sh.io.Print(sh.fmt.FQuat(tr.localRotation));
            return 0;
        }
        float[] quat=ParseUtil.QuatR(val,out byte rq);
        if(quat==null) return sh.io.Error(ParseUtil.error);
        if(rq==1) tr.localRotation=new Quaternion(quat[0],quat[1],quat[2],quat[3])*tr.localRotation;
        else if(rq==2) tr.localRotation=tr.localRotation*(new Quaternion(quat[0],quat[1],quat[2],quat[3]));
        else tr.localRotation=new Quaternion(quat[0],quat[1],quat[2],quat[3]);
        return 1;
    }
    public static int _CmdParamWQuat(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            sh.io.Print(sh.fmt.FQuat(tr.rotation));
            return 0;
        }
        float[] quat=ParseUtil.QuatR(val,out byte rq);
        if(quat==null) return sh.io.Error(ParseUtil.error);
        if(rq==1) tr.rotation=(new Quaternion(quat[0],quat[1],quat[2],quat[3]))*tr.rotation;
        else if(rq==2) tr.rotation=tr.rotation*(new Quaternion(quat[0],quat[1],quat[2],quat[3]));
        else tr.rotation=new Quaternion(quat[0],quat[1],quat[2],quat[3]);
        return 1;
    }

    public static int _CmdParamPRot(ComShInterpreter sh,Transform tr,string val){
        if(val!=null) return sh.io.Error("protは参照専用です");
        if(tr.parent!=null) sh.io.Print(sh.fmt.FEuler(tr.parent.rotation.eulerAngles));
        else sh.io.Print(sh.fmt.FEuler(Vector3.zero));
        return 0;
    }
    public static int _CmdParamPQuat(ComShInterpreter sh,Transform tr,string val){
        if(val!=null) return sh.io.Error("pquatは参照専用です");
        if(tr.parent!=null) sh.io.Print(sh.fmt.FQuat(tr.parent.rotation));
        else sh.io.Print(sh.fmt.FQuat(Quaternion.identity));
        return 0;
    }
    public static int _CmdParamL2W(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(tr.localToWorldMatrix.ToString()); return 0; }
        float[] xyz= ParseUtil.Xyz(val);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        sh.io.Print(sh.fmt.FPos(
            tr.localToWorldMatrix.MultiplyPoint3x4(new Vector3(xyz[0],xyz[1],xyz[2]))
        ));
        return 0;
    }
    public static int _CmdParamW2L(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ sh.io.Print(tr.worldToLocalMatrix.ToString()); return 0; }
        float[] xyz= ParseUtil.Xyz(val);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        sh.io.Print(sh.fmt.FPos(
            tr.worldToLocalMatrix.MultiplyPoint3x4(new Vector3(xyz[0],xyz[1],xyz[2]))
        ));
        return 0;
    }
    public static int _CmdParamIid(ComShInterpreter sh,Transform tr,string val){
        sh.io.Print(tr.GetInstanceID().ToString());
        return 0;
    }
}

public static class UTIL {
    private static int seqId=0;
    public static string GetSeqId(){ return (seqId=(seqId+1)%int.MaxValue).ToString(); }
    public static string Suffix(string str,string sfx){
        return str.EndsWith(sfx,Ordinal)?str:str+sfx;
    }
    public static string Suffix2(string str,string sfx){
        if(str=="") return "";
        return str.EndsWith(sfx,Ordinal)?str:str+sfx;
    }
    public static Vector3 V3(float[] v){ return new Vector3(v[0],v[1],v[2]); }
    public static Transform ResetTr(Transform tr){
        tr.localPosition=Vector3.zero;
        tr.localRotation=Quaternion.identity;
        tr.localScale=Vector3.one;
        return tr;
    }
    public delegate int TraverseFunc(Transform tr,int depth);
    public static int TraverseTr(Transform tr,TraverseFunc act,bool rootq=true){
       if(rootq) return TraverseTrSub(tr,act,0);
       int ret=0;
       for(int i=0; i<tr.childCount; i++) if((ret=TraverseTrSub(tr.GetChild(i),act,rootq?1:0))<0) return ret;
       return 0;
    }
    private static int TraverseTrSub(Transform tr,TraverseFunc act,int depth){
        int ret=act.Invoke(tr,depth);
        if(ret<0) return ret; // 負:中止
        if(ret==1) return 0; //   1: このノードの子は見ない
        for(int i=0; i<tr.childCount; i++) if((ret=TraverseTrSub(tr.GetChild(i),act,depth+1))<0) return ret;
        return 0;
    }
    public static int BFT(Transform tr,TraverseFunc f){ // branch-first traversal
        var q=new Queue<Transform>();
        q.Enqueue(tr);
        for(Transform t=tr; q.Count>0; t=q.Dequeue()){
            int ret=f(t,0);
            if(ret<0) return ret;
            if(ret==1) continue; // このノードの子は見ない
            for(int i=0; i<t.childCount; i++) q.Enqueue(t.GetChild(i));
        }
        return 0;
    }
    public static Transform BFS(Transform tr,TraverseFunc f){ // branch-first search
        var q=new Queue<Transform>();
        q.Enqueue(tr);
        for(Transform t=tr; q.Count>0; t=q.Dequeue()){
            int ret=f(t,0);
            if(ret<0) return null;
            if(ret==1) return t;
            for(int i=0; i<t.childCount; i++) q.Enqueue(t.GetChild(i));
        }
        return null;
    }
    public static Transform BFS(Transform tr,string name){
        var q=new Queue<Transform>();
        q.Enqueue(tr);
        for(Transform t=tr; q.Count>0; t=q.Dequeue()){
            if(t.name==name) return t;
            for(int i=0; i<t.childCount; i++) q.Enqueue(t.GetChild(i));
        }
        return null;
    }
    public static Transform FindTrByPrefix(Transform tr,string pfx){
        if(tr.name.StartsWith(pfx,Ordinal)) return tr;
        Transform t;
        for(int i=0; i<tr.childCount; i++)
            if((t=FindTrByPrefix(tr.GetChild(i),pfx))!=null) return t;
        return null;
    }
    public static bool IsAncestor(Transform tr,Transform anc){
        if(tr==anc) return true;
        if(tr.parent==null) return false;
        return IsAncestor(tr.parent,anc);
    }
    private static Regex numberonly=new Regex(@"^\d+$",RegexOptions.Compiled);
    private static Regex word=new Regex(@"^\w[-\w]*$",RegexOptions.Compiled);
    private static Regex word2=new Regex(@"^\w[-\.\w]*$",RegexOptions.Compiled);
    public static bool ValidName(string name){
        if(!word.Match(name).Success) return false;         // 文字は\wと'-'
        if(numberonly.Match(name).Success) return false;    // 全て数字は不可
        if(ngNameSet.Contains(name)) return false;          // コマンドや特定のオブジェクトと被る名は不可
        return true;
    }
    public static bool ValidObjName(string name){
        if(!word2.Match(name).Success) return false;        // 文字は\wと'-'と'.'
        if(numberonly.Match(name).Success) return false;    // 全て数字は不可
        if(ngNameSet.Contains(name)) return false;          // コマンドや特定のオブジェクトと被る名は不可
        return true;
    }
    private static HashSet<string> ngNameSet=new HashSet<string>() {
        "add","del","main","Offset","AllOffset","none","off","maid","man","obj","light","clone"
    };
    public static Transform GetObjRoot(string name){
        Transform bg=GameMain.Instance.gameObject.transform.Find("BG");
        if(name=="") return bg;
        if(bg==null) return null;
        Transform pftr=bg.Find(name);
        if(pftr!=null) return pftr;
        pftr=(new GameObject(name)).transform;
        pftr.SetParent(bg, false);
        return pftr;
    }
    public static Regex dosdev=new Regex(@"^(?:AUX|CON|NUL|PRN|CLOCK\$|COM\d|LPT\d)(?:\..*)?$",RegexOptions.Compiled|RegexOptions.IgnoreCase);
    public static string GetFullPath(string fname,string path){
        if(CheckFileName(fname)<0) return "";
        string full=Path.GetFullPath(path+fname);
        string dir=Path.GetDirectoryName(full);
        if(dir.Length<path.Length-1) return "";
        return full;
    }
    public static int CheckFileName(string fname){
        if(fname.IndexOfAny(Path.GetInvalidFileNameChars())>=0) return -1;
        if(dosdev.Match(Path.GetFileName(fname)).Success) return -1;
        return 0;
    }
    public static byte[] ReadAll(string fname){
        byte[] array=null;
        try{
            array=File.ReadAllBytes(fname); 
        }catch{}
        return array;
    }
    public static byte[] AReadAll(string fname){
        byte[] array=null;
        try{
            if(GameUty.IsExistFile(fname,GameUty.FileSystem)){
                using(AFileBase af=GameUty.FileOpen(fname)){ array=af.ReadAll(); }
            }else if(GameUty.IsExistFile(fname,GameUty.FileSystemOld)){
                using(AFileBase af=GameUty.FileOpen(fname,GameUty.FileSystemOld)){ array=af.ReadAll(); }
            }
        }catch{}
        return array;
    }
    public static void PrintTrInfo(ComShInterpreter sh,Transform tr,bool worldonly=false){
        if(worldonly){
            sh.io.PrintLn2("position:",sh.fmt.FPos(tr.position));            
            sh.io.PrintLn2("rotation:",sh.fmt.FEuler(tr.rotation.eulerAngles));
        }else{
            sh.io.PrintLn2("wpos:",sh.fmt.FPos(tr.position));            
            sh.io.PrintLn2("wrot:",sh.fmt.FEuler(tr.rotation.eulerAngles));
            sh.io.PrintLn2("lpos:",sh.fmt.FPos(tr.localPosition));            
            sh.io.PrintLn2("lrot:",sh.fmt.FEuler(tr.localRotation.eulerAngles));
        }
        sh.io.PrintLn2("scale:",sh.fmt.FMul(tr.localScale));
        sh.io.PrintLn2("wscale:",sh.fmt.FMul(tr.lossyScale));
        sh.io.PrintLn2("right:",sh.fmt.FPos(tr.right));
        sh.io.PrintLn2("up:",sh.fmt.FPos(tr.up));
        sh.io.PrintLn2("forward:",sh.fmt.FPos(tr.forward));
    }

    // 並び順に意味のないリストからの削除
    public static void RemoveAtFromList<T>(List<T> list,int at){
        if(list.Count==0) return;
        int tail=list.Count-1;
        list[at]=list[tail];
        list.RemoveAt(tail);
    }
    public static void RemoveFromList<T>(List<T> list,T item){
        if(list.Count==0) return;
        int tail=list.Count-1;
        if(System.Object.ReferenceEquals(list[tail],item)){list.RemoveAt(tail); return;}
        int i;
        for(i=0; i<tail; i++) if(System.Object.ReferenceEquals(list[i],item)) break;
        if(i==tail) return;
        list[i]=list[tail];
        list.RemoveAt(tail);
    }
    public static int MaxIdx(params float[] values){
        if(values.Length==0) return -1;
        int idx=0;
        float max=values[0];
        for(int i=1; i<values.Length; i++) if(values[i]>max){max=values[i];idx=i;}
        return idx;
    }
    public static string Tr2Name(ComShInterpreter sh,Transform tr){
        StringBuilder sb=new StringBuilder();
        Maid m;
        ObjInfo oi;
        if(ObjUtil.objDic.ContainsKey(tr.name)){
            sb.Append("obj:").Append(tr.name);
        }else if((m=tr.GetComponentInParent<Maid>())!=null){
            int maidno=m.boMAN?MaidUtil.IndexOfMan(m):MaidUtil.IndexOfMaid(m);
            Transform slottr=tr;
            for(; slottr!=null; slottr=slottr.parent)
                if(slottr.name.StartsWith("_SM_",Ordinal)) break;
            string slotname=null;
            if(slottr!=null){
                for(int i=0; i<m.body0.goSlot.Count; i++){
                    if(System.Object.ReferenceEquals(m.body0.goSlot[i].obj_tr,slottr)){
                        slotname=m.body0.goSlot[i].Category;
                        break;
                    }
                }
            }
            sb.Append(m.boMAN?"man:":"maid:").Append(maidno.ToString());
            if(slotname!=null) sb.Append('.').Append(slotname);
            sb.Append(':').Append(tr.name);
        }else if((oi=tr.GetComponentInParent<ObjInfo>())!=null && oi.enabled){
            if(oi!=null){
                var root=oi.name;
                if(tr.name==root) sb.Append("obj:").Append(tr.name);
                else sb.Append("obj:").Append(root).Append(':').Append(tr.name);
            }
        }else{
            var pftr=ObjUtil.GetPhotoPrefabTr(sh);
            Transform t=tr;
            for(; t.parent!=null; t=t.parent) if(System.Object.ReferenceEquals(t.parent,pftr)) break;
            if(t.parent!=null) sb.Append("obj:").Append(t.name);

        }
        return (sb.Length==0)?tr.name:sb.ToString();
    }
}
}
