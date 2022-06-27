using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace COM3D2.ComSh.Plugin {

// インタプリタ
public partial class ComShInterpreter {
    public static string homeDir=Path.GetFullPath(Application.dataPath.TrimEnd('\\')+@"\..")+"\\";
	public static string scriptFolder = homeDir+@"Sybaris\UnityInjector\Config\ComSh\";
    public static string myposeDir = homeDir+@"PhotoModeData\Mypose\";
    public const string SCRIPT_ERR_ON="_enable_script_error";
    public const string SEARCH_RESULT_MAX="_search_result_max";

	public Dictionary<string, string> env;  // 環境変数
    public Dictionary<string,ScriptStatus> func; // function
    public ComShParser parser=new ComShParser();
    public bool exitq=false;
    public bool interactiveq=false;
    public IO io;
    public delegate void Output(string str,int code);
    public ComShPanel panel=null;
    public string ofs=" ";

	public ComShInterpreter(Output op=null, Dictionary<string,string> parentEnv=null,Dictionary<string,ScriptStatus> parentFunc=null) {
        env=(parentEnv!=null)?new Dictionary<string, string>(parentEnv):new Dictionary<string, string>();
        env["`"]=string.Empty;
        func=(parentFunc!=null)?new Dictionary<string,ScriptStatus>(parentFunc):new Dictionary<string,ScriptStatus>();
        io=new IO(this,op);
        OnEnvChanged();
        Command.Init();
	}
	public int Interpret(string line) {
        int r=Parse(line);
        if(r<=0) return r;
        return InterpretParser(parser);
    }
	public int Parse(string line) {
        int r=parser.Parse(line);
        if(r<0) return io.Error(parser.error);
        if(r==0) return io.OK(); // 空行
        return 1;
    }
    public bool envChanged=false;
    public int InterpretParser(){ return InterpretParser(this.parser); }
    public int InterpretParser(ComShParser parser,bool canSleep=false){
        List<string> tokens;
        while((tokens=parser.Next(env))!=null){
            if(parser.envChanged) envChanged=true;
            if (tokens.Count==0) continue;
            if(envChanged) { OnEnvChanged(); envChanged=false; } // 変数変更後最初のコマンド実行時
            int ret=InterpretTokens(tokens,parser.prevEoL,parser.currentEoL,canSleep);
            if(ret<0) return ret;
            if(exitq) return io.OK(io.exitStatus);
        }
        return io.OK();
    }
    private int InterpretTokens(List<string> tokens,char prevEoL,char currentEoL,bool canSleep){
        int ret=0;
        Command.Cmd cmd;
        io.PrintStart(prevEoL);

        // コマンド先頭が'?'だったらエラー無視
        io.suppressError=false;
        if(tokens[0].Length>0 && tokens[0][0]=='?'){
            io.suppressError=true;
            tokens[0]=tokens[0].Substring(1);
        }

        if(tokens[0].IndexOf(':')>0){   // maid:0:BHead などコロン記法
            string[] sa=tokens[0].Split(ParseUtil.colon);
            if(sa.Length==2){
                switch(sa[0]){
                case "maid": ret=CmdMaidMan.CmdMaidSub(this,sa[1],tokens,1); break;
                case "man": ret=CmdMaidMan.CmdManSub(this,sa[1],tokens,1); break;
                case "obj": ret=CmdObjects.CmdObjectSub(this,sa,tokens,1); break;
                case "light": ret=CmdLights.CmdLightSub(this,sa[1],tokens,1); break;
                default: ret=io.Error("コマンドが存在しません"); break;
                }
            }else if(sa.Length==3){
                ret=CmdObjects.CmdObjectSub(this,sa,tokens,1);
            }else ret=io.Error("コマンドが存在しません");
        }else if((cmd=Command.GetCmd(tokens[0]))!=null){    // 通常のコマンド処理
            if(tokens[0]!="sleep" || canSleep) ret=cmd.Invoke(this,tokens);
        }else if(func.ContainsKey(tokens[0])){              // func
            var f=func[tokens[0]];
            f.rewind();
            var sbo=new SubShOutput();
            ComShInterpreter child = new ComShInterpreter(new Output(sbo.Output),env,func);
            ret=child.Exec(tokens,f);
            if(ret<0) ret=io.Error(child.io.errorMessage,ret);
            else io.Print(sbo.GetSubShResult());
        }else{      // シェルスクリプト
            string fname=FullScriptNameSafe(tokens[0]);
            if(fname=="") ret=io.Error("コマンドが存在しません");
            else{
                var sbo=new SubShOutput();
                ComShInterpreter child = new ComShInterpreter(new Output(sbo.Output),env,func);
                ret=child.Exec(tokens,fname);
                if(ret<0) ret=io.Error(child.io.errorMessage,ret);
                else io.Print(sbo.GetSubShResult());
            }
        }
        io.PrintEnd(currentEoL);
        return ret;
    }
    public class SubShOutput {
        private StringBuilder sb=new StringBuilder();
        public void Output(string str,int code){
            if(code==0) sb.Append(str).Append('\n'); // lfのみ
            else Debug.Log(str);
        }
        public string GetSubShResult(){
            string ret=sb.ToString().TrimEnd(ParseUtil.lf);
            sb.Length=0;
            return ret;
        }
    }
    public void SourceRc(){
        Source("_comshrc");
        ComShProperties.Update(env);
    }
    public void OnEnvChanged(){
        ofs=GetEnv("OFS"," ");
        fmt.Update(env);
        UpdateObjBase(env);
        UpdateLightBase(env);
    }
    public string GetEnv(string key,string dflt=""){
        if(env.TryGetValue(key,out string ret)) return ret;
        return dflt;
    }

    private int Source(string scriptName) { // 今はもう_comshrc専用
		try {
			string filename=FullScriptName(scriptName);
            if(filename=="") return io.Error("スクリプトが存在しません");
			using (StreamReader sr = new StreamReader(filename, Encoding.UTF8)) {
				while (sr.Peek()>-1) {
					string line = sr.ReadLine();
					this.Interpret(line);   // エラーでも止めない
                    if(this.exitq) return io.exitStatus;
				}
			}
		} catch (ArgumentException) {
			return io.Error("スクリプトが存在しません");
		} catch (FileNotFoundException) {
			return io.Error("スクリプトが存在しません");
		} catch (IOException) {
			return io.Error("スクリプトにアクセスできません");
		}
        return 0;
	}
    public static string FullScriptNameSafe(string scriptName){
        try{ return FullScriptName(scriptName); }catch{}
        return "";
    }
    private static Regex dosdev=new Regex(@"^(?:AUX|CON|NUL|PRN|CLOCK\$|COM\d|LPT\d)(?:\..*)?$",RegexOptions.Compiled|RegexOptions.IgnoreCase);
    public static string FullScriptName(string scriptName){
        if(dosdev.Match(Path.GetFileName(scriptName)).Success) return "";
        string filename=Path.GetFullPath(scriptFolder+scriptName);
        string path=Path.GetDirectoryName(filename);
        // ..\等でscriptFolderより上のフォルダを指定していたら弾く
        if((path.Length+1)<scriptFolder.Length) return "";
        if(File.Exists(filename)) return filename;
        if(!filename.EndsWith(".comsh",StringComparison.Ordinal) && File.Exists(filename+".comsh")) return filename+=".comsh";
        return "";
    }
    public class ScriptStatus {
        public string name="";
        public int current=-1;
        public float sleeptime=0;
        public bool isFunc=false;
        public bool isSource=false;
        public bool enableSleep=false;
        public int line0=0;
        public List<ComShParser> lines=new List<ComShParser>();
        public ScriptStatus(bool funcq=false,bool scriptq=false){
            isFunc=funcq; isSource=scriptq;
            enableSleep=(!funcq && !scriptq);
        }
        public bool hasNext(){ return lines.Count-1>current; }
        public void rewind(){
            for(int i=0; i<lines.Count; i++) lines[i].Reset();
            current=-1;
        }
        public int next(ComShInterpreter sh){
            // ここからの実行だけsleep許可(funcやsource時は不可)
            return sh.InterpretParser(lines[++current],enableSleep);
        }
        public void Add(ComShParser p){
            this.lines.Add(p);
        }
        public int read(ComShInterpreter sh,string scriptName,string full){
            try {
                string filename=full;
                if(filename==null){
                    filename=FullScriptName(scriptName);
                    if(filename=="") return sh.io.Error("スクリプトが存在しません");
                }
                name=filename.Substring(ComShInterpreter.scriptFolder.Length);
                using(StreamReader sr=new StreamReader(filename,Encoding.UTF8)){
                    lines.Clear();
                    ScriptStatus target=this;
                    int lno=line0;
                    while (sr.Peek()>-1){
                        string line=sr.ReadLine();
                        var p=new ComShParser();
                        p.lineno=++lno;
                        int ret=p.Parse(line);
                        if(ret<0) return sh.io.Error(p.error);
                        if(ret==0) continue;
                        // functionを探す
                        List<string> tokens=p.Next(null);
                        if(tokens!=null && tokens.Count>0){
                            if(tokens[0]=="func"){
                                if(target.isFunc) return sh.io.Error("func内でfuncは使用できません");
                                if(tokens.Count!=2&&tokens.Count!=3) return sh.io.Error("funcの書式が不正です");
                                string funcname=tokens[1];
                                if(!ParseUtil.IsWord(funcname) ||Command.GetCmd(funcname)!=null)
                                    return sh.io.Error("関数名が不正です");
                                if(tokens.Count==3){
                                    var s=new ScriptStatus(true);
                                    if(s.read(sh,tokens[2],null)<0) return -1;
                                    sh.func[funcname]=s;
                                }else{
                                    target=new ScriptStatus(true);
                                    target.line0=lno;
                                    sh.func[funcname]=target;   // 既にあっても上書き
                                }
                                continue;
                            }else if(tokens[0]=="func.end"){
                                if(tokens.Count!=1 || target==this) return sh.io.Error("funcの書式が不正です");
                                target.rewind();
                                target=this;
                                continue;
                            }
                        }
                        target.Add(p);
                    }
                    if(target!=this) return sh.io.Error("funcが閉じられていません");
                    rewind();
                }
                return sh.io.OK();
		    } catch (ArgumentException) {
			    return sh.io.Error("スクリプトが存在しません");
		    } catch (FileNotFoundException) {
			    return sh.io.Error("スクリプトが存在しません");
		    } catch (IOException) {
			    return sh.io.Error("スクリプトにアクセスできません");
		    }
        }
        public int toSleep(float ms){
            current--;
            sleeptime=ms;
            return 0;
        }

        public int Run(ComShInterpreter sh){
            sh.env.TryGetValue(SCRIPT_ERR_ON,out string seo);
            bool erron=(seo=="1");
            while(hasNext()){
                int ret=next(sh);
                if(erron && ret<0) return ret;
                if(sh.exitq) return sh.io.exitStatus;
                if(sleeptime>0){
                    ComShBg.cron.AddJob("sleep/"+UTIL.GetSeqId(),(long)(sleeptime*TimeSpan.TicksPerMillisecond),0,(long t)=>{
                        this.Run(sh);
                        return -1;
                    });
                    sleeptime=0;
                    return 0;
                }
            }
            return 0;
        }
    }
    public ScriptStatus runningScript;

	private int Exec(ScriptStatus script){
        var old=runningScript;
        runningScript=script;
        int ret=script.Run(this);
        if(script.isSource){ exitq=false; runningScript=old; }
        return ret;
    }
	private int Exec(string scriptName,string full=null,bool sourceq=false) {
        var script=new ScriptStatus(false,sourceq);
        int ret=script.read(this,scriptName,full);
        if(ret<0) return ret;
        return Exec(script);
	}
    private void ExecParamEnv(List<string> args){
    	env["#"]=(args.Count-1).ToString();                       // $#
		for (int i=0;i<args.Count;i++) env[i.ToString()]=args[i]; // $0,$1,...
        // env["*"]=string.Join(" ",args.ToArray());
		for (int i=args.Count;i<=255;i++) if(!env.Remove(i.ToString())) break;
    }
	public int Exec(List<string> args,string fullname=null) {
        ExecParamEnv(args);
		return Exec(args[0],fullname);
	}
	public int Exec(List<string> args,ScriptStatus script){
        ExecParamEnv(args);
		return Exec(script);
	}
    // sourceなんだけどExecの処理に合流してしまった処理
	public int ExecSource(List<string> args) {
        ExecParamEnv(args);
		return Exec(args[0],null,true);
	}

    public class IO {
        private ComShInterpreter sh;
        private Dictionary<string, string> env;
        public Output output;
        public IO(ComShInterpreter sh,Output output){
            this.sh=sh;
            this.env=sh.env;
            this.output=output??new Output(Silent);
        }
        private void Silent(string msg,int code){}

        private StringBuilder printSb=new StringBuilder();
        public string pipedText;
        public void PrintStart(char peol){
            printSb.Length=0;
            pipedText=(peol=='|')?env["`"]:null;
            env["`"]="";
        }
        public void Print(string str){ printSb.Append(str); }
        public void Print(char[] ca,int start,int len){ printSb.Append(ca,start,len); }
        public void PrintLn(string str){ printSb.Append(str).Append('\n'); }
        public void PrintLn2(string s1,string s2){
            printSb.Append(s1).Append(s2).Append('\n'); // lfのみ
        }
        public void PrintJoin(string fs,params string[] str){
            if(str.Length==0) return;
            _PrintJoin(fs,str);
        }
        private void _PrintJoin(string fs,string[] str){
            printSb.Append(str[0]);
            for(int i=1; i<str.Length; i++) printSb.Append(fs).Append(str[i]);
        }
        public void PrintJoinLn(string fs,params string[] str){
            if(str.Length==0) return;
            _PrintJoin(fs,str);
            printSb.Append("\n");
        }
        public void PrintEnd(char eol){
            if(printSb.Length==0) return;
            env["`"]=printSb.ToString().TrimEnd(ParseUtil.lf);
            if(eol==';') output(env["`"],0);
            printSb.Length=0;
        }

        public void Output(string str,int code){ output(str,code); }
        public string errorMessage="";
        public int exitStatus=0;
        public int OK(int code=0){
            errorMessage="";
            exitStatus=code;
            env["?"]=code.ToString();
            return code;
        }
        public bool suppressError=false;
        public int Error(string msg, int code=-1){
            if(suppressError){
                errorMessage="";
                exitStatus=0;
                env["?"]=code.ToString(); // $?にだけエラーを残す
                return 0;
            }
            errorMessage=msg;
            exitStatus=code;
            env["?"]=code.ToString();
            ScriptStatus script=sh.runningScript;
            if(script!=null && script.name!=""){
                int lno=script.lines[script.current].lineno;
                output($"{script.name}:{lno}行: {msg}",-1);
            }else output(msg,-1);
            return code;
        }
    }
    public FMT fmt=new FMT();
    public class FMT {
        private string fmt_01="F4";
        private string fmt_int="F3";
        private string fmt_val="F8";
        public void Update(Dictionary<string,string> e){
            string s;
            if((s=GetFromEnv("_format_0to1",e))!=string.Empty) fmt_01=s;
            if((s=GetFromEnv("_format_intlike",e))!=string.Empty) fmt_int=s;
            if((s=GetFromEnv("_format_normal",e))!=string.Empty) fmt_val=s;
        }
        private string GetFromEnv(string key,Dictionary<string,string> e){
            if(!e.ContainsKey(key)) return string.Empty;
            if(!int.TryParse(e[key],out int n)||n<0||n>50) return string.Empty;
            return "F"+e[key];
        }
        public string F0to1(float f){ return f.ToString(fmt_01); } // 0.0～1.0の値
        public string FInt(float f){ return f.ToString(fmt_int); } // 倍率や角度など整数1-3桁がメインの値
        public string FVal(float f){ return f.ToString(fmt_val); }  // 上記以外
        public string FPos(Vector3 v){ return FVal(v.x)+","+FVal(v.y)+","+FVal(v.z);}
        public string FPos(float x,float y,float z){ return FVal(x)+","+FVal(y)+","+FVal(z);}
        public string FEuler(Vector3 v){ return FInt(v.x)+","+FInt(v.y)+","+FInt(v.z);}
        public string FQuat(Quaternion v){ return FVal(v.x)+","+FVal(v.y)+","+FVal(v.z)+","+FVal(v.w);}
        public string FMul(Vector3 v){ return FInt(v.x)+","+FInt(v.y)+","+FInt(v.z);}
        public string FEA2(Vector3 v){ return FVal(v.x)+","+FVal(v.y); }
        public string RGB(Color c){
            int rgb=(((int)(c.r*255))<<16)|(((int)(c.g*255))<<8)|((int)(c.b*255));
            return rgb.ToString("X6");
        }
        public string RGB2(Color c){ return F0to1(c.r)+","+F0to1(c.g)+","+F0to1(c.b); }
        public string RGBA(Color c){ return F0to1(c.r)+","+F0to1(c.g)+","+F0to1(c.b)+","+F0to1(c.a); }
        public string OnOff(bool sw){ return sw?"on":"off"; }
    }

    private const string OBJROOT="_obj_root";
    private const string OBJ_ROOT_BG="";
    private const string OBJ_ROOT_COMSH="ComShPrefab";
    private const string OBJ_ROOT_STUDIO="PhotoPrefab";
    public string objBase=OBJ_ROOT_COMSH;
    public string objRef=OBJ_ROOT_STUDIO;
    private void UpdateObjBase(Dictionary<string,string> env){
       if(!env.ContainsKey(OBJROOT)) return;
        string val=env[OBJROOT];
        if(val=="bg") objBase=OBJ_ROOT_BG; else objBase=OBJ_ROOT_COMSH;
        if(val=="studio") objRef=OBJ_ROOT_STUDIO; else objRef="";
    } 

    private const string LIGHTROOT="_light_root";
    private const string LIGHT_ROOT_BG="";
    private const string LIGHT_ROOT_COMSH="ComShLight";
    private const string LIGHT_ROOT_STUDIO="LightObject";
    public string lightBase=LIGHT_ROOT_COMSH;
    public string lightRef=LIGHT_ROOT_STUDIO;
    private void UpdateLightBase(Dictionary<string,string> env){
        if(!env.ContainsKey(LIGHTROOT)) return;
        string val=env[LIGHTROOT];
        if(val=="bg") lightBase=LIGHT_ROOT_BG; else lightBase=LIGHT_ROOT_COMSH;
        if(val=="studio") lightRef=LIGHT_ROOT_STUDIO; else lightRef="";
    }

    public const string SAFEMODE="_safe_mode";
    public bool IsSafeMode(){ return (env.ContainsKey(SAFEMODE)&&env[SAFEMODE]=="1"); }

}

}
