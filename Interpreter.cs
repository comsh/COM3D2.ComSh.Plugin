using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using static System.StringComparison;

namespace COM3D2.ComSh.Plugin {

// インタプリタ
public partial class ComShInterpreter {
    public static string homeDir=Path.GetFullPath(Application.dataPath.TrimEnd('\\')+@"\..")+"\\";
	public static string scriptFolder = homeDir+@"Sybaris\UnityInjector\Config\ComSh\";
    public static string myposeDir = homeDir+@"PhotoModeData\Mypose\";
    public const string SCRIPT_ERR_ON="_enable_script_error";
    public const string SEARCH_RESULT_MAX="_search_result_max";

	public VarDic env;        // 環境変数
    public Dictionary<string,ScriptStatus> func; // function
    public ComShParser parser;
    public bool exitq=false;
    public bool interactiveq=false;
    public IO io;
    public delegate void Output(string msg,int code);
    public ComShPanel panel=null;
    public string ofs=" ";
    public string ns="";
    public int lastcmp=1;

	public ComShInterpreter(Output op=null, VarDic parentEnv=null,Dictionary<string,ScriptStatus> parentFunc=null,string ns="") {
        env=(parentEnv!=null)?new VarDic(parentEnv):new VarDic();
        env.output=string.Empty;
        func=(parentFunc!=null)?new Dictionary<string,ScriptStatus>(parentFunc):new Dictionary<string,ScriptStatus>(10);
        io=new IO(this,op);
        this.ns=ns;
        OnEnvChanged();
        Command.Init();
	}
	public int Interpret(string line) {
        int r=Parse(line);
        if(r<=0) return r;
        return InterpretParser(parser);
    }
	public int Parse(string line) {
        if(parser==null) parser=new ComShParser();
        int r=parser.Parse(line);
        if(r<0) return io.Error(parser);
        if(r==0) return io.OK(); // 空行
        return 1;
    }
    public bool envChanged;
    public ComShParser currentParser;
    public int InterpretParser(){ return InterpretParser(this.parser); }
    public int InterpretParser(ComShParser parser,bool canSleep=false){
        var parser_bak=currentParser;
        currentParser=parser;
        List<string> tokens;
        envChanged=false;
        while((tokens=parser.Next(env,(runningScript!=null)?runningScript.svars:null))!=null){
            envChanged=envChanged||parser.envChanged;
            if (tokens.Count==0) continue;
            if(envChanged){ OnEnvChanged(); envChanged=false; }
            int ret=InterpretTokens(tokens,parser.prevEoL,parser.currentStatement.eol,canSleep);
            if(ret<0) return ret;
            if(exitq) return io.OK(io.exitStatus);
            if(parser.currentStatement.eol=='>') parser.Redirect();
            if(envChanged){ OnEnvChanged(); envChanged=false; }
        }
        if(envChanged){ OnEnvChanged(); envChanged=false; }
        currentParser=parser_bak;
        return io.OK();
    }
    public int InterpretParserSingleSubCmd(ComShParser parser){
        var sbo=new SubShOutput();
        var orig=io.output;
        io.output=new Output(sbo.Output);
        int ret=InterpretParser(parser);
        string result=sbo.GetSubShResult();
        io.output=orig;
        io.Print(result);
        return ret;
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
        var cd=new ParseUtil.ColonDesc(tokens[0]);
        if(cd.num>0){   // maid:0:BHead などのコロン記法
            if(cd.meshno>=0){
                ret=CmdMeshes.CmdMeshSub(this,cd,tokens,1);
            }else if(cd.num==3 || cd.path!=""){
                ret=CmdObjects.CmdObjectSub(this,cd,tokens,1);
            }else {
                switch(cd.type){
                case "maid": ret=CmdMaidMan.CmdMaidSub(this,cd.id,tokens,1); break;
                case "man": ret=CmdMaidMan.CmdManSub(this,cd.id,tokens,1); break;
                case "": 
                case "obj": ret=CmdObjects.CmdObjectSub(this,cd,tokens,1); break;
                case "light": ret=CmdLights.CmdLightSub(this,cd.id,tokens,1); break;
                default: ret=io.Error("コマンドが存在しません"); break;
                }
            }
        }else if((cmd=Command.GetCmd(tokens[0]))!=null){    // 通常のコマンド処理
            if(tokens[0]!="sleep" || canSleep) ret=cmd.Invoke(this,tokens);
        }else if(func.ContainsKey(tokens[0])){              // func
            var f=func[tokens[0]];
            f.rewind();
            var sbo=new SubShOutput();
            ComShInterpreter child = new ComShInterpreter(new Output(sbo.Output),env,func,ns);
            ret=child.Exec(tokens,f);
            io.Print(sbo.GetSubShResult());
        }else{      // シェルスクリプト
            string fname=FullScriptNameSafe(tokens[0]);
            if(fname=="") ret=io.Error("コマンドが存在しません");
            else{
                var sbo=new SubShOutput();
                ComShInterpreter child = new ComShInterpreter(new Output(sbo.Output),env,func,ns);
                ret=child.Exec(tokens,fname);
                io.Print(sbo.GetSubShResult());
            }
        }
        io.PrintEnd(currentEoL);
        return ret;
    }
    public class SubShOutput {
        private StringBuilder sb=new StringBuilder(256);
        public void Output(string msg,int code){
            if(code==0) sb.Append(msg).Append('\n');
            else if(msg.Length>0) Debug.Log(msg);
        }
        public string GetSubShResult(){
            if(sb.Length==0) return "";
            int tail=sb.Length-1;
            string ret=(sb[tail]=='\n')?sb.ToString(0,tail):sb.ToString();
            sb.Length=0;
            return ret;
        }
    }
    public void SourceRc(){
        Source("_comshrc");
        if(interactiveq) ComShProperties.Update(env);
    }

    public void OnEnvChanged(){
        ofs=Variables.Value(env,"OFS"," ");
        fmt.Update(env);
        UpdateObjBase(env);
        UpdateLightBase(env);
    }
    public static bool IsEnvChanged(string name){
        return (name.Length>0 && (name[0]=='_' || char.IsUpper(name[0])));
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
    public static string FullScriptName(string scriptName){
        if(UTIL.dosdev.Match(Path.GetFileName(scriptName)).Success) return "";
        string filename=Path.GetFullPath(scriptFolder+scriptName);
        string path=Path.GetDirectoryName(filename)+@"\";
        // ..\等でscriptFolderより上のフォルダを指定していたら弾く
        if(path.Length<scriptFolder.Length) return "";

        // データ用のフォルダからはスクリプト実行禁止
        string f1=path.Substring(scriptFolder.Length);
        if(f1==@"bin\"||f1==@"kvs\"||f1==@"export\"||f1==@"asset\") return "";

        if(File.Exists(filename)) return filename;
        if(!filename.EndsWith(".comsh",StringComparison.Ordinal) && File.Exists(filename+".comsh")) return filename+=".comsh";
        return "";
    }
    public class ScriptStatus {
        public string name="";
        public int current=-1;
        public double sleeptime=0;
        public bool isFunc=false;
        public bool isSource=false;
        public bool enableSleep=false;
        public int line0=0;
        public Queue<ComShParser> init;
        public List<ComShParser> lines;
    	public Dictionary<string, string> svars=new Dictionary<string,string>(10); // static変数
        public ScriptStatus(bool funcq=false,bool scriptq=false){
            isFunc=funcq; isSource=scriptq;
            enableSleep=(!funcq && !scriptq);
            lines=new List<ComShParser>(isFunc?64:256);
        }
        public bool hasNext(){ return lines.Count-1>current; }
        public void rewind(){
            if(init!=null && init.Count>0) foreach(var p in init) p.Reset();
            for(int i=0; i<lines.Count; i++) lines[i].Reset();
            current=-1;
        }
        public int next(ComShInterpreter sh){
            if(init!=null && init.Count>0){
                int ret=sh.InterpretParser(init.Dequeue());
                if(init.Count==0) init=null;
                return ret;
            }
            // ここからの実行だけsleep許可(funcやsource時は不可)
            return sh.InterpretParser(lines[++current],enableSleep);
        }
        public void Add(ComShParser p){
            if(this.init!=null) this.init.Enqueue(p);
            else this.lines.Add(p);
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
                    StringBuilder sb=null;
                    while (sr.Peek()>-1){
                        string line=sr.ReadLine();
                        lno++;
                        if(line.Length==0) continue;
                        if(line[line.Length-1]=='\\'){
                            if(sb!=null) sb.Append(line,0,line.Length-1);
                            else sb=new StringBuilder(line,0,line.Length-1,line.Length*4);
                            continue;
                        }
                        if(sb!=null){ sb.Append(line); line=sb.ToString(); sb=null; }
                        var p=new ComShParser(lno);
                        int ret=p.Parse(line);
                        if(ret<0) return sh.io.Error(p);
                        if(ret==0) continue;
                        // functionを探す
                        List<string> tokens=p.Next(null,null);
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
                            }else if(tokens[0]=="func.init"){
                                if(tokens.Count!=1 || target==this) return sh.io.Error("funcの書式が不正です");
                                target.init=new Queue<ComShParser>();
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
        public int toSleep(double ms){
            current--;
            sleeptime=ms;
            return 0;
        }

        public int Run(ComShInterpreter sh){
            string seo=sh.env[SCRIPT_ERR_ON];
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
        env.args.Clear();
    	if(args.Count>0){
            env["0"]=args[0];   //$0
		    for(int i=1;i<args.Count;i++) env.args.Add(args[i]); // $1,...
        }
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
        private VarDic env;
        public Output output;
        public IO(ComShInterpreter sh,Output output){
            this.sh=sh;
            this.env=sh.env;
            this.output=output??new Output(Silent);
        }
        private void Silent(string msg,int code){}

        private StringBuilder printSb=new StringBuilder(128);
        public string pipedText;
        public void PrintStart(char peol){
            printSb.Length=0;
            if(peol=='|') pipedText=env.output; else {pipedText=null; env.output="";}
        }
        public IO Print(char c){ printSb.Append(c); return this;}
        public IO Print(string str){ printSb.Append(str); return this;}
        public IO Print(char[] ca,int start,int len){ printSb.Append(ca,start,len); return this;}
        public IO PrintLn(string str){ printSb.Append(str).Append('\n'); return this;}
        public IO PrintLn2(string s1,string s2){
            printSb.Append(s1).Append(s2).Append('\n'); // lfのみ
            return this;
        }
        public IO PrintJoin(string fs,params string[] str){
            if(str.Length!=0) _PrintJoin(fs,str);
            return this;
        }
        private void _PrintJoin(string fs,string[] str){
            printSb.Append(str[0]);
            for(int i=1; i<str.Length; i++) printSb.Append(fs).Append(str[i]);
        }
        public IO PrintJoinLn(string fs,params string[] str){
            if(str.Length!=0){ _PrintJoin(fs,str); printSb.Append("\n"); }
            return this;
        }
        public void PrintEnd(char eol){
            if(printSb.Length==0){env.output=""; return; }
            int tail=printSb.Length-1;
            string txt=(printSb[tail]=='\n')?printSb.ToString(0,tail):printSb.ToString();
            env.output=txt;
            if(eol==';') output(txt,0);
            printSb.Length=0;
        }

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
            exitStatus=code;
            env["?"]=code.ToString();
            string name="",line="";
            ScriptStatus script=sh.runningScript;
            if(script!=null && script.name!="") name=script.name+":";
            if(sh.currentParser!=null && sh.currentParser.lineno>0) line=sh.currentParser.lineno.ToString()+"行: ";
            errorMessage=$"{name}{line}{msg}";
            output(errorMessage,code);
            return code;
        }
        public int Error(ComShParser parser,int code=-1){
            if(suppressError){
                errorMessage="";
                exitStatus=0;
                env["?"]=code.ToString(); // $?にだけエラーを残す
                return 0;
            }
            exitStatus=code;
            env["?"]=code.ToString();
            string name="",line="";
            ScriptStatus script=sh.runningScript;
            if(script!=null && script.name!="") name=script.name+":";
            if(parser.lineno>0) line=parser.lineno.ToString()+"行: ";
            errorMessage=$"{name}{line}{parser.error}";
            output(errorMessage,code);
            return code;
        }
    }
    public FMT fmt=new FMT();
    public class FMT {
        private string fmt_01="F4";
        private string fmt_int="F3";
        private string fmt_val="F8";
        public void Update(VarDic e){
            string s;
            if((s=GetFromEnv("_format_0to1",e))!=string.Empty) fmt_01=s;
            if((s=GetFromEnv("_format_intlike",e))!=string.Empty) fmt_int=s;
            if((s=GetFromEnv("_format_normal",e))!=string.Empty) fmt_val=s;
        }
        private string GetFromEnv(string key,VarDic e){
            string v=Variables.Value(e,key);
            if(!int.TryParse(v,out int n)||n>50) return string.Empty;
            if(n>=0) return "F"+v;
            return "0."+new string('#',-n);
        }
        public string F0to1(float f){ return f.ToString(fmt_01); } // 0.0～1.0の値
        public string F0to1(double f){ return f.ToString(fmt_01); } // 0.0～1.0の値
        public string FInt(float f){ return f.ToString(fmt_int); } // 倍率や角度など整数1-3桁がメインの値
        public string FInt(double f){ return f.ToString(fmt_int); } // 倍率や角度など整数1-3桁がメインの値
        public string FVal(float f){ return f.ToString(fmt_val); }  // 上記以外
        public string FVal(double f){ return f.ToString(fmt_val); }  // 上記以外
        public string FPos(Vector3 v){ return FVal(v.x)+","+FVal(v.y)+","+FVal(v.z);}
        public string FPos(float x,float y,float z){ return FVal(x)+","+FVal(y)+","+FVal(z);}
        public string FEuler(Vector3 v){ return FInt(v.x)+","+FInt(v.y)+","+FInt(v.z);}
        public string FQuat(Quaternion v){ return FVal(v.x)+","+FVal(v.y)+","+FVal(v.z)+","+FVal(v.w);}
        public string FQuat(float[] v){ return FVal(v[0])+","+FVal(v[1])+","+FVal(v[2])+","+FVal(v[3]);}
        public string FMul(Vector3 v){ return FInt(v.x)+","+FInt(v.y)+","+FInt(v.z);}
        public string FEA2(Vector3 v){ return FVal(v.x)+","+FVal(v.y); }
        public string FXY(Vector2 v){ return FVal(v.x)+","+FVal(v.y); }
        public string FXY(Vector3 v){ return FVal(v.x)+","+FVal(v.y); }
        public string FXY(float x,float y){ return FVal(x)+","+FVal(y); }
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
    private void UpdateObjBase(VarDic env){
        string val=env[OBJROOT];
        switch(val){
        case "bg":
            objBase=OBJ_ROOT_BG;
            objRef=OBJ_ROOT_STUDIO;
            break;
        case "comsh":
            objBase=OBJ_ROOT_COMSH;
            objRef="";
            break;
        case "":
        case "studio":
            objBase=OBJ_ROOT_COMSH;
            objRef=OBJ_ROOT_STUDIO;
            break;
        }
    } 

    private const string LIGHTROOT="_light_root";
    private const string LIGHT_ROOT_BG="";
    private const string LIGHT_ROOT_COMSH="ComShLight";
    private const string LIGHT_ROOT_STUDIO="LightObject";
    public string lightBase=LIGHT_ROOT_COMSH;
    public string lightRef=LIGHT_ROOT_STUDIO;
    private void UpdateLightBase(VarDic env){
        string val=env[LIGHTROOT];
        if(val=="") return;
        if(val=="bg") lightBase=LIGHT_ROOT_BG; else lightBase=LIGHT_ROOT_COMSH;
        if(val=="studio") lightRef=LIGHT_ROOT_STUDIO; else lightRef="";
    }

    public const string SAFEMODE="_safe_mode";
    public bool IsSafeMode(){ return (env[SAFEMODE]=="1"); }

}

}
