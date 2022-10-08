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
        cmdTbl.Add("seqno",new Cmd(CmdSeqNo));
        cmdTbl.Add("refreshmypose",new Cmd(CmdRefreshMypose));
        cmdTbl.Add("sin",new Cmd(CmdSin));
        cmdTbl.Add("cos",new Cmd(CmdCos));
        cmdTbl.Add("rnd",new Cmd(CmdRnd));
        cmdTbl.Add("sleep",new Cmd(CmdSleep));
        cmdTbl.Add("kvs",new Cmd(CmdKvs));
        cmdTbl.Add("distance",new Cmd(CmdDistance));
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
        cmdTbl.Add("sed",new Cmd(CmdSed));
        cmdTbl.Add("meminfo",new Cmd(CmdMemInfo));
        cmdTbl.Add("escape",new Cmd(CmdEscape));
        cmdTbl.Add("sort",new Cmd(CmdSort));
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
        cmdTbl.Add("anmlist",new Cmd(CmdAnmList));

        cmdTbl.Add("__res",new Cmd(Cmd__Resource));
        cmdTbl.Add("__files",new Cmd(Cmd__Files));

        CmdGui.Init();
        CmdMaidMan.Init();
        CmdBones.Init();
        CmdObjects.Init();
        CmdLights.Init();
        CmdCamera.Init();
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
    private static int CmdAnmList(ComShInterpreter sh,List<string> args){
        var files_sex=new List<string>();
        var files_dance=new List<string>();
        var files_common=new List<string>();
        string[] fa=GameUty.FileSystem.GetList("motion",AFileSystemBase.ListType.AllFile);
        string name;
        for(int i=0; i<fa.Length; i++) if(fa[i].EndsWith(".anm",Ordinal)){
            name=Last2(fa[i],'\\');
            if(name==null) continue;
            if(name.Contains("\\crc_")) continue;
            if(name.Contains("\\cbl_")) continue;
            if(fa[i].Contains("\\sex\\")) files_sex.Add(name);
            else if(fa[i].Contains("\\dance\\")) files_dance.Add(Path.GetFileNameWithoutExtension(name));
            else if(fa[i].Contains("\\common\\") || fa[i].Contains("\\dance_mc\\") || fa[i].Contains("\\evnet") || fa[i].Contains("\\hanyo"))
                files_common.Add(Path.GetFileNameWithoutExtension(name));
        }
        fa=GameUty.FileSystemOld.GetList("motion",AFileSystemBase.ListType.AllFile);
        for(int i=0; i<fa.Length; i++) if(fa[i].EndsWith(".anm",Ordinal)){
            name=Last2(fa[i],'\\');
            if(name==null) continue;
            if(name.Contains("\\crc_")) continue;
            if(name.Contains("\\cbl_")) continue;
            if(fa[i].Contains("\\sex\\")) files_sex.Add(name);
            else if(fa[i].Contains("\\dance\\")) files_dance.Add(Path.GetFileNameWithoutExtension(name));
            else if(fa[i].Contains("\\common\\") || fa[i].Contains("\\dance_mc\\") || fa[i].Contains("\\evnet") || fa[i].Contains("\\hanyo"))
                files_common.Add(Path.GetFileNameWithoutExtension(name));
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
    private static int WriteFileList(string filename,List<string> files){
        try{
            string datadir=ComShInterpreter.scriptFolder+"export\\";
            Directory.CreateDirectory(datadir);
            using(StreamWriter sw=new StreamWriter(datadir+filename,false,Encoding.UTF8)){
                string prev="";
                foreach(string fn in files) if(fn!=prev){
                    string[] sa=fn.Split('\\');
                    prev=fn;
                    if(sa.Length==1) sw.WriteLine(sa[0]);
                    else sw.WriteLine($"{sa[0]}\\{sa[1]}");
                }
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

	private static int CmdExit(ComShInterpreter sh,List<string> args){
        sh.exitq=true; 
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
        if(!sh.interactiveq) return 0;
        if(args.Count>1){
            args.RemoveAt(0);
			GUIUtility.systemCopyBuffer=string.Join(" ", args.ToArray())+"\n";
        }else GUIUtility.systemCopyBuffer=ComShWM.terminal.GetLog();
        return 0;
    }
    private static int CmdTimer(ComShInterpreter sh,List<string>args){
        if(args.Count==1) foreach(string name in ComShBg.cron.LsJob("timer/")) sh.io.PrintLn(name);
        if(args.Count!=3) return sh.io.Error("使い方: timer 待機時間(ms) 待機後に実行するコマンド");
        if(!float.TryParse(args[1],out float ms)||ms<=0) return sh.io.Error("数値の指定が不正です");
        var psr=new ComShParser(sh.lastParser.lineno);
        int r=psr.Parse(args[2]);
        if(r<0) return sh.io.Error(psr.error);
        if(r==0) return sh.io.Error("コマンドが空です");   
        if(ComShBg.cron.AddJob("timer/"+UTIL.GetSeqId(),(long)(ms*TimeSpan.TicksPerMillisecond),0,(long t)=>{
            sh.InterpretParser(psr);
            sh.exitq=false; // 元のシェルは終わらせない
            return -1;  // 負を返せば１回だけの実行で終わる
        })==null) return sh.io.Error("タイマー登録に失敗しました");
        return 0;
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
        if(args.Count>3||args.Count<2) return sh.io.Error("使い方: wincolor 背景色 [文字色]");
        float[] bcol=ParseUtil.Rgba(args[1]);
        if(bcol==null) return sh.io.Error(ParseUtil.error);
        if(args.Count==3){
            float[] fcol=ParseUtil.Rgba(args[2]);
            if(bcol==null) return sh.io.Error(ParseUtil.error);
            PanelStyleCache.SetTextColor(fcol);
        }
        PanelStyleCache.SetBgColor(bcol);
        return 0;
    }
	private static int CmdEcho(ComShInterpreter sh,List<string> args) {
        if(args.Count==1) return 0;
        for(int i=1; i<args.Count-1; i++) sh.io.Print(args[i]+sh.ofs);
        sh.io.PrintLn(args[args.Count-1]);
        return 0;
	}
	private static int CmdLog(ComShInterpreter sh,List<string> args) {
        if(args.Count==1) return 0;
        Debug.Log(string.Join(sh.ofs,args.GetRange(1,args.Count-1).ToArray()));
        return 0;
	}
    private static int CmdEnv(ComShInterpreter sh,List<string> args){
        foreach(var kv in sh.env) sh.io.Print($"{kv.Key}={kv.Value.Get()}\n");
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
            int wordPos=text.LastIndexOf(kw+":",Ordinal);
            if(wordPos<0) continue;
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
        if(!int.TryParse(nth,out int n)||n<=0) return sh.io.Error("行番号を数値(1～)で指定してください");
        string ret=ParseUtil.Nth(txt,'\n',n);
        if(ret!=null) sh.io.PrintLn(ret.TrimEnd(ParseUtil.cr));
        return 0;
    }
	private static int CmdSource(ComShInterpreter sh,List<string> args) {
        if(args.Count==1) return sh.io.Error("使い方: source スクリプトファイル名 [引数 ...]");
        args.RemoveAt(0);
		return sh.ExecSource(args);
	}
    private static int CmdEval(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: eval コマンド文字列");
        var p=new ComShParser(sh.lastParser.lineno);
        if(p.Parse(args[1])<0) return sh.io.Error(p.error);
        return sh.InterpretParser(p);
    }
    private static int CmdTest(ComShInterpreter sh,List<string> args){
        string usage=$"使い方: test 対象1 比較演算子 対象2 コマンド1 [コマンド2]\n"
                    +"比較演算子   eq|ne|ge|le|gt|lt|has|and|or\n"
                    +"コマンド1    比較結果が真のとき実行するコマンド\n"
                    +"コマンド2    比較結果が偽のとき実行するコマンド\n"
                    +"対象1,対象2  どちらも数値と解釈できれば数値として比較\n"
                    +"             そうでなければ文字列として比較される";
        if(args.Count!=5&&args.Count!=6) return sh.io.Error(usage);
        int cmp=CmdCmpSub(args[1],args[2],args[3]);
        if(cmp<0) return sh.io.Error(usage);
        if(cmp==1){
            var p=new ComShParser(sh.lastParser.lineno);
            if(p.Parse(args[4])<0) sh.io.Error(p.error);
            return sh.InterpretParser(p);
        }else if(args.Count==6){
            var p=new ComShParser(sh.lastParser.lineno);
            if(p.Parse(args[5])<0) sh.io.Error(p.error);
            return sh.InterpretParser(p);
        }
        return 0;
    }
    private static int CmdCmp(ComShInterpreter sh,List<string> args){
        const string usage="使い方: cmp 対象1 比較演算子 対象2\n"
                    +"比較演算子   eq|ne|ge|le|gt|lt|has|and|or\n"
                    +"対象1,対象2  どちらも数値と解釈できれば数値として比較\n"
                    +"             そうでなければ文字列として比較される";
        if(args.Count!=4) return sh.io.Error(usage);
        int cmp=CmdCmpSub(args[1],args[2],args[3]);
        if(cmp<0) return sh.io.Error(usage);
        sh.io.Print(cmp==1?"1":"0");
        return 0;
    }
    private static int CmdCmpSub(string val1,string op,string val2){
        float f1=ParseUtil.ParseFloat(val1);
        if(op=="between"){
            float[] fa=ParseUtil.MinMax(val2);
            if(float.IsNaN(f1) || fa==null) return -2;
            return (fa[0]<=f1 && f1<=fa[1])?1:0;
        }
        float f2=ParseUtil.ParseFloat(val2);
        bool numq=!float.IsNaN(f1)&&!float.IsNaN(f2);
        bool cmp;
        if(op=="eq") cmp=numq?(f1==f2):(val1.CompareTo(val2)==0);
        else if(op=="ne") cmp=numq?(f1!=f2):(val1.CompareTo(val2)!=0);
        else if(op=="ge") cmp=numq?(f1>=f2):(val1.CompareTo(val2)>=0);
        else if(op=="le") cmp=numq?(f1<=f2):(val1.CompareTo(val2)<=0);
        else if(op=="gt") cmp=numq?(f1>f2):(val1.CompareTo(val2)>0);
        else if(op=="lt") cmp=numq?(f1<f2):(val1.CompareTo(val2)<0);
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
            var ls=bg.LsJob(jobpfx);
            foreach(string name in ls) sh.io.PrintLn(name.Substring(jobpfx.Length));
            return 0;
        }
        if(args[1]=="add"){
            if(args.Count<4) return sh.io.Error(string.Format(usage,args[0]));
            if(args.Count>7) return sh.io.Error(string.Format(usage,args[0]));
            if(args[2]==string.Empty) return sh.io.Error("識別名が空です");
            if(!UTIL.ValidName(args[2])) return sh.io.Error("その名前は使用できません");
            string name=jobpfx+args[2];
            if(bg.ContainsName(name)) return sh.io.Error("その名前は既に使われています");
            int prio=0;
            float ms=0,life=0;
            if(args.Count>=5 && (!float.TryParse(args[4],out ms) || ms<0)) return sh.io.Error("実行間隔の値が不正です");
            if(args.Count>=6 && (!float.TryParse(args[5],out life) || life<0)) return sh.io.Error("寿命の値が不正です");
            if(args.Count==7 && (!int.TryParse(args[6],out prio) || prio<0)) return sh.io.Error("順序の値が不正です");
            var subsh=new ComShInterpreter(null,sh.env,sh.func);
            subsh.env[ComShInterpreter.SCRIPT_ERR_ON]="1";
            var psr=subsh.parser;
            int r=psr.Parse(args[3]); // パースだけしておく
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) return sh.io.Error("コマンドが空です");
            long stime=DateTime.UtcNow.Ticks;
            var j=bg.AddJob(name,(long)(ms*TimeSpan.TicksPerMillisecond),(long)(life*TimeSpan.TicksPerMillisecond),(long t)=>{
                subsh.env["1"]=((t-stime)/TimeSpan.TicksPerMillisecond).ToString();
                psr.Reset();
                return subsh.InterpretParser();
            },(int)prio);
            if(j==null) return sh.io.Error("登録に失敗しました");          
            j.sh=subsh;
        }else if(args[1]=="del"){
            for(int i=2; i<args.Count; i++) bg.KillJob(jobpfx+args[i]);
        }else if(args.Count==4){
            var name=jobpfx+args[1];
            var j=bg.Find(name);
            if(j==null) return sh.io.Error("その識別名の定期処理は存在しません");
            if(args[2]=="ondestroy"){
                var subsh=j.sh;
                var psr=new ComShParser(sh.lastParser.lineno);
                int r=psr.Parse(args[3]);
                if(r<0) return sh.io.Error(psr.error);
                if(r==0) return sh.io.Error("コマンドが空です");
                j.destroy=new ComShBg.JobAction((long t)=>{
                    subsh.env["1"]=t.ToString();
                    psr.Reset();
                    return subsh.InterpretParser(psr);
                });
            } else return sh.io.Error(string.Format(usage,args[0]));
        } else return sh.io.Error(string.Format(usage,args[0]));
        return 0;
    }
    private static int CmdCutLoop(ComShInterpreter sh,List<string> args){
        const string usage="使い方: cutloop 入力文字列 [区切り文字] コマンド";
        if(args.Count==4) return CutLoop(sh,args[1],args[2],args[3]);
        else if(args.Count==3){
            if(sh.io.pipedText!=null) return CutLoop(sh,sh.io.pipedText,args[1],args[2]);
            else return CutLoop(sh,args[1]," ",args[2]);
        }else if(args.Count==2 && sh.io.pipedText!=null) return CutLoop(sh,sh.io.pipedText," ",args[1]);
        else return sh.io.Error(usage);
    }
    private static int CutLoop(ComShInterpreter sh,string txt,string dlmt,string cmd){
        if(txt=="") return 0;
        if(dlmt=="") return sh.io.Error("区切り文字が空です");
        var psr=new ComShParser(sh.lastParser.lineno);
        int r=psr.Parse(cmd);
        if(r<0) return sh.io.Error(psr.error);
        if(r==0) return sh.io.Error("コマンドが空です");
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
        sh.io.Print(subout.GetSubShResult());
        return ret;
    }
    private static int CmdLineLoop(ComShInterpreter sh,List<string> args){
        const string usage="使い方: lineloop 入力文字列 コマンド";
        const string usage2="使い方: lineloop コマンド";
        if(sh.io.pipedText!=null){
            if(args.Count!=2) return sh.io.Error(usage2);
            return CutLoop(sh,sh.io.pipedText,"\n",args[1]);
        }else{
            if(args.Count!=3) return sh.io.Error(usage);
            return CutLoop(sh,args[1],"\n",args[2]);
        }
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
        float[] v1=ParseUtil.FloatArr(args[1]);
        if(v1==null||v1.Length==0) return sh.io.Error("数値の形式が不正です");
        for(int i=2; i<args.Count; i+=2){
            if(args[i].Length!=1) return sh.io.Error("演算子が不正です");
            float[] v2=ParseUtil.FloatArr(args[i+1]);
            if(v2==null||v2.Length==0) return sh.io.Error("数値の形式が不正です");
            v1=Calc(args[i][0],v1,v2);
            if(v1==null) return sh.io.Error(exprErr);
        }
        sh.io.Print(sh.fmt.FVal(v1[0]));
        for(int i=1; i<v1.Length; i++){ sh.io.Print(","); sh.io.Print(sh.fmt.FVal(v1[i])); }
        return 0;
    }
    private static float[] Calc(char op,float[] v1,float[] v2){
        int max,min;
        if(v1.Length>v2.Length){ max=v1.Length; min=v2.Length; }
        else { max=v2.Length; min=v1.Length; }
        if(max==min){
            if(max==4){
                if(op!='*'){ exprErr="演算子が不正です"; return null; }
                var q=new Quaternion(v1[0],v1[1],v1[2],v1[3])*new Quaternion(v2[0],v2[1],v2[2],v2[3]);
                return new float[]{q.x,q.y,q.z,q.w};
            }else{
                if(ExprTwoVal(max,op,v1,v2)<0) return null;
                return v1;
            }
        }else{
            if(min==1){
                if(v1.Length==1){
                    var fa=new float[max];
                    for(int i=0;i<max; i++) fa[i]=v1[0];
                    if(ExprTwoVal(max,op,fa,v2)<0) return null;
                    return fa;
                }else{
                    var fa=new float[max];
                    for(int i=0;i<max; i++) fa[i]=v2[0];
                    if(ExprTwoVal(max,op,v1,fa)<0) return null;
                    return v1;
                }
            }else if(min==3 && max==4){
                if(v1.Length==3){
                    if(op!='*'){ exprErr="演算子が不正です"; return null; }
                    var v=new Quaternion(v2[0],v2[1],v2[2],v2[3])*new Vector3(v1[0],v1[1],v1[2]);
                    return new float[]{ v.x, v.y, v.z};
                }else{
                    if(op!='*'){ exprErr="演算子が不正です"; return null; }
                    var v=new Quaternion(v1[0],v1[1],v1[2],v1[3])*new Vector3(v2[0],v2[1],v2[2]);
                    return new float[]{ v.x, v.y, v.z};
                }
            }else exprErr="その組み合わせの計算はできません";
        }
        return null;
    }
    private static int ExprTwoVal(int n1,char op,float[] v1,float[] v2){
        switch(op){
        case '+': for(int i=0; i<n1; i++) v1[i]+=v2[i]; break;
        case '-': for(int i=0; i<n1; i++) v1[i]-=v2[i]; break;
        case '*': for(int i=0; i<n1; i++) v1[i]*=v2[i]; break;
        case '/':
            for(int i=0; i<n1; i++){
                if(Mathf.Approximately(v2[i],0f)){ exprErr="0で除算しようとしています"; return -1; }
                v1[i]/=v2[i];
            }
            break;
        case '%':
            for(int i=0; i<n1; i++){
                if(Mathf.Approximately(v2[i],0f)){ exprErr="0で除算しようとしています"; return -1; }
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
        MotionWindow mw = GameObject.FindObjectOfType<MotionWindow>();
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
        var nameterm=new Dictionary<string, List<string>>();
		var data=new Dictionary<string, List<KeyValuePair<string, object>>>();
		foreach (var kv in PhotoMotionData.category_list){
			if(!data.ContainsKey(kv.Key)){
				data.Add(kv.Key,new List<KeyValuePair<string,object>>());
				nameterm.Add(kv.Key, new List<string>());
			}
			for(int i=0; i<kv.Value.Count; i++){
				data[kv.Key].Add(new KeyValuePair<string, object>(kv.Value[i].name,kv.Value[i]));
				nameterm[kv.Key].Add(kv.Value[i].nameTerm);
			}
		}
		mw.PopupAndTabList.SetData(data,nameterm,true);
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
        float v=ParseUtil.ParseFloat(args[1]);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        sh.io.Print(sh.fmt.FVal(Mathf.Abs(v)));
        return 0;
    }
    private static int CmdSqrt(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: sqrt 値");
        float v=ParseUtil.ParseFloat(args[1],-1);
        if(v<0) return sh.io.Error("数値の指定が不正です");
        sh.io.Print(sh.fmt.FVal(Mathf.Sqrt(v)));
        return 0;
    }
    private static int CmdMax(ComShInterpreter sh,List<string> args){
        if(args.Count<2) return sh.io.Error("使い方: max 値1 [値2 ...]");
        float max=float.MinValue;
        for(int i=1; i<args.Count; i++){
            float v=ParseUtil.ParseFloat(args[i]);
            if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
            if(v>max) max=v;
        }
        sh.io.Print(sh.fmt.FVal(max));
        return 0;
    }
    private static int CmdMin(ComShInterpreter sh,List<string> args){
        if(args.Count<2) return sh.io.Error("使い方: min 値1 [値2 ...]");
        float min=float.MaxValue;
        for(int i=1; i<args.Count; i++){
            float v=ParseUtil.ParseFloat(args[i]);
            if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
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
    private static int CmdQuat(ComShInterpreter sh,List<string> args){
        const string usage="使い方1: quat 回転軸 角度\n"
                          +"使い方2: quat オイラー角\n"
                          +"使い方3: quat 回転前ベクトル 回転後ベクトル";
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
        }else return sh.io.Error(usage);
        return 0;
    }
	private static int CmdSleep(ComShInterpreter sh,List<string> args){
        if(args.Count!=2) return sh.io.Error("使い方: sleep 待機時間(ms)");
        if(!float.TryParse(args[1],out float t)||t<0) return sh.io.Error("数値の指定が不正です");
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
        if(d==3) sh.io.Print(sh.fmt.FVal(Mathf.Sqrt(v[0]*v[0]+v[1]*v[1]+v[2]*v[2])));
        else if(d==2) sh.io.Print(sh.fmt.FVal(Mathf.Sqrt(v[0]*v[0]+v[1]*v[1])));
        else if(d==4) sh.io.Print(sh.fmt.FVal(Mathf.Sqrt(v[0]*v[0]+v[1]*v[1]+v[2]*v[2]+v[3]*v[3])));
        else if(d==1) sh.io.Print(sh.fmt.FVal(Mathf.Abs(v[0])));
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
        float v,min,max;
        if(args.Count!=4 || !float.TryParse(args[1],out v)
            || !float.TryParse(args[2],out min) || !float.TryParse(args[3],out max) )
                return sh.io.Error("使い方: clamp 値 最小値 最大値");
        if(v<min) v=min; if(v>max) v=max;
        sh.io.Print(sh.fmt.FVal(v));
        return 0;
    }
    private static int CmdInside(ComShInterpreter sh,List<string> args){
        const string usage="使い方: inside 座標 中心座標 サイズ [回転]";
        if(args.Count!=4 && args.Count!=5) return sh.io.Error(usage);
        float[] p=ParseUtil.Xyz(args[1]);
        if(p==null) return sh.io.Error(ParseUtil.error);
        float[] o=ParseUtil.Xyz(args[2]);
        if(o==null) return sh.io.Error(ParseUtil.error);
        float[] sz=new float[3];
        int n=ParseUtil.XyzSub(args[3],sz);
        if(n<0) return sh.io.Error("サイズの指定が不正です");
        Vector3 v3=new Vector3(p[0]-o[0],p[1]-o[1],p[2]-o[2]);
        if(args.Count==5){
            float[] r=ParseUtil.FloatArr(args[4]);
            if(r==null) return sh.io.Error(ParseUtil.error);
            var q=(r.Length==4)?new Quaternion(r[0],r[1],r[2],r[3]):Quaternion.Euler(new Vector3(r[0],r[1],r[2]));
            v3=Quaternion.Inverse(q)*v3; // 座標を領域側の座標系に翻訳
        }
        bool result=false;
        if(n==1){ // 球
            if(sz[0]<=0) return sh.io.Error("サイズの指定が不正です");
            result=(sz[0]*sz[0]>=v3.sqrMagnitude);
        }else if(n==2){ // シリンダ
            result=(-sz[1]<=v3.y && v3.y<=sz[1] && (v3.x*v3.x+v3.z*v3.z)<=sz[0]*sz[0]);
        }else if(n==3){ // 直方体
            result=(-sz[0]<=v3.x&&v3.x<=sz[0]&&-sz[1]<=v3.y&&v3.y<=sz[1]&&-sz[2]<=v3.z&&v3.z<=sz[2]);
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
            }else {sh.env.Remove(args[i]); changed=true; }
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
        if(!int.TryParse(st,out s)) return sh.io.Error("数値の指定が不正です");
        if(s<0) s=val.Length+s;
        if(s<0) return sh.io.Error("数値の指定が不正です");
        if(s>=val.Length) return 0;
        if(cnt=="") { sh.io.PrintLn(val.Substring(s)); return 0;}

        if(!int.TryParse(cnt,out n)||n<=0) return sh.io.Error("数値の指定が不正です");
        if(s+n>val.Length) sh.io.PrintLn(val.Substring(s));
        else sh.io.PrintLn(val.Substring(s,n));
        return 0;
    }
    private static int CmdRepeat(ComShInterpreter sh,List<string> args){
        if(args.Count!=3) return sh.io.Error("使い方: repeat 回数 コマンド");
        if(!int.TryParse(args[1],out int n)||n<=0) return sh.io.Error("数値の指定が不正です");
        var psr=new ComShParser(sh.lastParser.lineno);
        int r=psr.Parse(args[2]);
        if(r<0) return sh.io.Error(psr.error);
        if(r==0) return sh.io.Error("コマンドが空です");

        // 現シェルでの実行だが、出力だけはサブシェル実行と同じ形にする
        ComShInterpreter.Output orig=sh.io.output;
        var subout=new ComShInterpreter.SubShOutput();
        sh.io.output=new ComShInterpreter.Output(subout.Output);
        int ret=0;
        for(int i=0; i<n; i++){
            sh.env["_1"]=i.ToString();
            psr.Reset();
            ret=sh.InterpretParser(psr);
            if(ret<0 || sh.exitq){ ret=sh.io.exitStatus; sh.exitq=false; break; }
        }
        sh.io.output=orig;
        sh.io.Print(subout.GetSubShResult());
        return ret;

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

    public delegate int CmdParam<T>(ComShInterpreter sh,T m,string val);
    public static int ParamLoop<T>(ComShInterpreter sh,T tgt,Dictionary<string,CmdParam<T>> dic,List<string> args,int prmstart){
        int cnt=args.Count;
        int odd=(cnt-prmstart)%2;
        if(odd>0) cnt--;
        for(int i=prmstart; i<cnt; i+=2){
            if(!dic.TryGetValue(args[i],out CmdParam<T> func)) return sh.io.Error("不正なパラメータです");
            int ret=func.Invoke(sh,tgt,args[i+1]);
            if(ret<=0) return ret;
        }
        if(odd>0){
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
}

public static class UTIL {
    private static int seqId=0;
    public static string GetSeqId(){ return (seqId=(seqId+1)%int.MaxValue).ToString(); }
    public static string Suffix(string str,string sfx){
        return str.EndsWith(sfx,Ordinal)?str:str+sfx;
    }
    public static Vector3 V3(float[] v){ return new Vector3(v[0],v[1],v[2]); }
    public static Transform ResetTr(Transform tr){
        tr.localPosition=Vector3.zero;
        tr.localRotation=Quaternion.identity;
        tr.localScale=Vector3.one;
        return tr;
    }
    public delegate int TraverseFunc(Transform tr);
    public static int TraverseTr(Transform tr,TraverseFunc act,bool rootq=true){
       if(rootq) return TraverseTrSub(tr,act);
       int ret=0;
       for(int i=0; i<tr.childCount; i++) if((ret=TraverseTrSub(tr.GetChild(i),act))<0) return ret;
       return ret;
    }
    private static int TraverseTrSub(Transform tr,TraverseFunc act){
        int ret=act.Invoke(tr);
        if(ret<0) return ret; // 負:中止
        if(ret==1) return 0; //   1: このノードの子は見ない
        for(int i=0; i<tr.childCount; i++) if((ret=TraverseTr(tr.GetChild(i),act))<0) return ret;
        return 0;
    }
    public static int BFT(Transform tr,TraverseFunc f){ // branch-first traversal
        var q=new Queue<Transform>();
        q.Enqueue(tr);
        for(Transform t=tr; q.Count>0; t=q.Dequeue()){
            int ret=f(t);
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
            int ret=f(t);
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
    public static bool ValidName(string name){
        if(!Regex.Match(name,@"^\w[-\w]*$").Success) return false;  // 文字は\wと'-'
        if(Regex.Match(name,@"^\d+$").Success) return false;        // 全て数字は不可
        if(ngNameSet.Contains(name)) return false;                  // コマンドや特定のオブジェクトと被る名は不可
        return true;
    }
    private static HashSet<string> ngNameSet=new HashSet<string>() {
        "add","del","main","Offset","AllOffset","none","off","maid","man","obj","light","clone"
    };
    public static Transform GetObjRoot(string name,bool create=false){
        Transform pftr=GameMain.Instance.BgMgr.Parent.transform.Find(name);
        if(pftr!=null) return pftr;
        if(create){
            pftr=(new GameObject(name)).transform;
            pftr.SetParent(GameMain.Instance.BgMgr.Parent.transform, false);
            return pftr;
        }else return null;
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
}
}
