using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static COM3D2.ComSh.Plugin.Command;

namespace COM3D2.ComSh.Plugin {
// インタプリタ
public static class CmdMisc {
    public static void Init(){
        Command.AddCmd("bg",new Cmd(CmdBG));
		Command.AddCmd("sound", new Cmd(CmdSound));
        Command.AddCmd("kag",new Cmd(CmdKag));
        Command.AddCmd("curve",new Cmd(CmdCurve));
        Command.AddCmd("tmpfile",new Cmd(CmdTmpFile));
        Command.AddCmd("queue",new Cmd(CmdQueue));
        Command.AddCmd("projector",new Cmd(CmdProjector));
        Command.AddCmd("reflectionprobe",new Cmd(CmdReflectionProbe));
    }

    private static int CmdBG(ComShInterpreter sh,List<string> args){
		if (args.Count==1) {
            var bgmgr=GameMain.Instance.BgMgr;
            if(bgmgr==null) return 0;
            string name=bgmgr.GetBGName();
            if(!string.IsNullOrEmpty(name)) sh.io.PrintLn(name);
            return 0;
        }
        int prmstart=1;
        var code=GetCmdParamBg(args[1]);
        if(code==null){
            string orig=GameMain.Instance.BgMgr.GetBGName();
            try{
                SetBg(args[1]);
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
            }catch{ return sh.io.Error("背景が見つかりません");}
            prmstart=2;
        }
        int ret,cnt=args.Count;
        int odd=(cnt-prmstart)%2;
        if(odd>0) cnt--;

        var bgo=GameMain.Instance.BgMgr.BgObject;
        if(bgo==null) return 0;
        Transform tr=GameMain.Instance.BgMgr.BgObject.transform;
        for(int i=prmstart; i<cnt; i+=2){
            code=GetCmdParamBg(args[i]);
            if(code==null) return sh.io.Error("パラメータが不正です");
            ret=code.Invoke(sh,tr,args[i+1]);
            if(ret<=0) return ret;
        }
        if(odd>0){
            code=GetCmdParamBg(args[cnt]);
            if(code==null) return sh.io.Error("パラメータが不正です");
            ret=code.Invoke(sh,tr,null);
            if(ret<=0) return ret;
        }
        return 0;
    }
    private static CmdParam<Transform> GetCmdParamBg(string cmd){
        if(cmd=="pos"||cmd=="wpos"||cmd=="position") return _CmdParamWPos;
        if(cmd=="rot"||cmd=="wrot"||cmd=="rotation") return _CmdParamWRot;
        if(cmd=="wposrot") return _CmdParamWPosRot;
        if(cmd=="scale") return _CmdParamScale;
        if(cmd=="iid") return _CmdParamIid;
        return null;
    }
    private static void SetBg(string val){
        ObjectManagerWindow omw=null;
        if(StudioMode.pwm!=null){
            omw=StudioMode.GetWindow<ObjectManagerWindow>(PhotoWindowManager.WindowType.ObjectManager);
		    omw.RemoveTransTargetObject(GameMain.Instance.BgMgr.current_bg_object);
        }

        bool myroom=false;
        Dictionary<string,string> mrdic=MyRoomCustom.CreativeRoomManager.GetSaveDataDic();
        foreach(string k in mrdic.Keys) if(mrdic[k]==val){
            GameMain.Instance.BgMgr.ChangeBgMyRoom(k);
            myroom=true;
            break;
        }
        if(!myroom) GameMain.Instance.BgMgr.ChangeBg(val);
        GameMain.Instance.BgMgr.current_bg_object.name=val;

        if(omw!=null){
            GameObject bgobj=GameMain.Instance.BgMgr.current_bg_object;
            omw.AddTransTargetObject(bgobj,bgobj.name,bgobj.name, PhotoTransTargetObject.Type.BG);
        }
    }

    private static int CmdSound(ComShInterpreter sh,List<string> args){
		if (args.Count==1){
            PrintSound(sh,3,"system:");
            PrintSound(sh,0,"bgm:");
            PrintSound(sh,1,"env:");
            PrintSound(sh,2,"se:");
            return 0;
        }
        var sm=GameMain.Instance.SoundMgr;
        int ret,cnt=args.Count;
        int odd=(cnt-1)%2;
        if(odd>0) cnt--;
        for(int i=1; i<cnt; i+=2) if((ret=CmdParamSound(sh,sm,args[i],args[i+1]))<=0) return ret;
        if(odd>0) if((ret=CmdParamSound(sh,sm,args[cnt],null))<=0) return ret;
        return 0;
    }
    private static int CmdParamSound(ComShInterpreter sh,SoundMgr sm,string cmd,string val){
        if(cmd=="bgm.time"||cmd=="dance.time"){
            if(val==null){
                var asm=GetASMgr(0);
                if(asm!=null&&asm.audiosource!=null&&asm.audiosource.isPlaying&&asm.audiosource.clip.length>0)
                    sh.io.PrintLn(((int)((asm.audiosource.time%asm.audiosource.clip.length)*1000)).ToString());
            }else{
                float t=ParseUtil.ParseFloat(val);
                if(float.IsNaN(t)||t<0) return sh.io.Error("数値の指定が不正です");
                var asm=GetASMgr(0);
                if(asm!=null&&asm.audiosource!=null&&asm.audiosource.isPlaying&&asm.audiosource.clip.length>0)
                    asm.audiosource.time=Mathf.Clamp(t/1000,0,asm.audiosource.clip.length);
            }
        }else if(cmd=="bgm.timep"||cmd=="dance.timep"){
            if(val==null){
                var asm=GetASMgr(0);
                if(asm!=null&&asm.audiosource!=null&&asm.audiosource.isPlaying&&asm.audiosource.clip.length>0)
                    sh.io.PrintLn(sh.fmt.F0to1(asm.audiosource.time/asm.audiosource.clip.length));
            }else{
                float t=ParseUtil.ParseFloat(val);
                if(float.IsNaN(t)||t<0) return sh.io.Error("数値の指定が不正です");
                var asm=GetASMgr(0);
                if(asm!=null&&asm.audiosource!=null&&asm.audiosource.isPlaying&&asm.audiosource.clip.length>0)
                    asm.audiosource.time=Mathf.Clamp01(t)*asm.audiosource.clip.length;
            }
        }else if(cmd=="bgm.length"||cmd=="dance.length"){
            var asm=GetASMgr(0);
            if(asm!=null&&asm.audiosource!=null&&asm.audiosource.isPlaying)
                sh.io.PrintLn(((int)(asm.audiosource.clip.length*1000)).ToString());
        }else if(cmd=="se"){
            if(val==null){ PrintSound(sh,2); return 0;}
            if(val.Length==0) sm.StopSe(); else{
                string[] sa=val.Split(ParseUtil.comma);
                int n=0;
                if(sa.Length>1 && (!int.TryParse(sa[1],out n) || n<0 || n>1)) return sh.io.Error("ループ指定が不正です");
                sm.PlaySe(UTIL.Suffix(sa[0],".ogg"),n==1);
            }
        }else if(cmd=="bgm"){
            if(val==null){ PrintSound(sh,0); return 0;}
            if(val.Length==0) sm.StopBGM(0f); else{
                sm.PlayBGM(UTIL.Suffix(val,".ogg"), 0, true);
                var asm=GetASMgr(0);
                if(asm!=null&&asm.audiosource!=null) asm.audiosource.time=0;
            }
        }else if(cmd=="dance"){
            if(val==null){ PrintSound(sh,0); return 0;}
            if(val.Length==0) sm.StopBGM(0f); else{
                sm.PlayDanceBGM(UTIL.Suffix(val,".ogg"), 0f,false);
                var asm=GetASMgr(0);
                if(asm!=null&&asm.audiosource!=null) asm.audiosource.time=0;
            }
        }else if(cmd=="dancep"){
            if(val==null){ PrintSound(sh,0); return 0;}
            if(val.Length==0) sm.StopBGM(0f); else{
                sm.PlayDanceBGMParallel(UTIL.Suffix(val,".ogg"), 0f,false);
            }
        }else if(cmd=="env"){
            if(val==null){ PrintSound(sh,1); return 0;}
            if(val.Length==0) sm.StopEnv(0f); else{
                sm.PlayEnv(UTIL.Suffix(val,".ogg"),0f);
            }
        }else return sh.io.Error("パラメータが不正です");
        return 1;
    }
    private static void PrintSound(ComShInterpreter sh,int type,string pfx=null){
        var asm=GetASMgr(type);
        if(asm==null) return;
        if(string.IsNullOrEmpty(asm.FileName)) return;
        var asrc=asm.audiosource;
        if(asrc==null) return;
        if(pfx!=null) sh.io.Print(pfx);
        sh.io.PrintJoinLn(sh.ofs, asm.FileName,
            $"{(int)((asrc.time%asrc.clip.length)*1000)}/{(int)(asrc.clip.length*1000)}"
        );
    }
    private static string[] setName={
        "AudioSet-Bgm-AudioBgm", "AudioSet-Env-AudioEnv", "AudioSet-Se-AudioSe", "AudioSet-System-AudioSystem"
    };
    private static AudioSourceMgr GetASMgr(int type){
        var amTr=GameMain.Instance.MainCamera.transform.Find("AudioMgr");
        if(amTr==null) return null;
        for(int i=0; i<amTr.childCount; i++){
            var child=amTr.GetChild(i);
            if(child.name==setName[type]){
                var asma=child.GetComponentsInChildren<AudioSourceMgr>();
                for(int j=0; j<asma.Length; j++)
                    if(asma[j]!=null&&asma[j].isPlay()) return asma[j];
            }
        }
        return null;
    }
 
    private static int CmdKag(ComShInterpreter sh,List<string> args){
        if(args.Count==1||args.Count>5) return sh.io.Error("使い方: kag スクリプト名 [開始ラベル] [メイド番号] [男性番号]");
        GameMain.Instance.ScriptMgr.StopMotionScript();
        if(args[1]=="") return 0;
        string[] prms=ParseUtil.NormalizeParams(args,new string[]{"","","0","0"},1);
        Maid maid=MaidUtil.FindMaid(prms[2]); prms[2]=(maid!=null)?maid.status.guid:"";
        Maid man=MaidUtil.FindMan(prms[3]); prms[3]=(man!=null)?man.status.guid:"";
        string fname=UTIL.Suffix(prms[0],".ks");
        GameMain.Instance.ScriptMgr.LoadMotionScript(0,false,fname,prms[1],prms[2],prms[3]);
        return 0;
    }

    public static Dictionary<string,AnimationCurve> curveDic=new Dictionary<string,AnimationCurve>();
    private static int CmdCurve(ComShInterpreter sh,List<string> args){
        AnimationCurve ac;
        if(args.Count==1){
            foreach(string k in curveDic.Keys) if (k.StartsWith(sh.ns,StringComparison.Ordinal)){
                float tmin=float.MaxValue,tmax=float.MinValue;
                float vmin=float.MaxValue,vmax=float.MinValue;
                foreach(Keyframe frm in curveDic[k].keys){
                    if(frm.time<tmin) tmin=frm.time;
                    if(frm.time>tmax) tmax=frm.time;
                    if(frm.value<vmin) vmin=frm.value;
                    if(frm.value>vmax) vmax=frm.value;
                }
                sh.io.Print($"{k} {(int)(tmin*1000)}～{(int)(tmax*1000)} {sh.fmt.FVal(vmin)}～{sh.fmt.FVal(vmax)}\n");
            }
            return 0;
        }
        if(args[1]=="add"){
            if(args.Count<5) return sh.io.Error("使い方: curve add 識別名 キーフレーム定義1 キーフレーム定義2 [...]");
            if(!UTIL.ValidName(args[2])) return sh.io.Error("その名前は使用できません");
            string name=sh.ns+args[2];
            if(curveDic.ContainsKey(name)) return sh.io.Error("その名前は既に使われています");
            ac=new AnimationCurve();
            for(int i=3; i<args.Count; i++){
                float time,value,inTan=0,outTan=0;
                string[] sa=args[i].Split(ParseUtil.comma);
                if(sa.Length!=2&&sa.Length!=4) return sh.io.Error("キーフレームの書式が不正です"); 
                if(!float.TryParse(sa[0],out time)||time<0||!float.TryParse(sa[1],out value))
                    return sh.io.Error("数値が不正です");
                if(sa.Length==4 &&(!float.TryParse(sa[2],out inTan)||!float.TryParse(sa[3],out outTan)))
                    return sh.io.Error("数値が不正です");
                ac.AddKey(new Keyframe(time/1000,value,inTan,outTan));
            }
            if(ac.length<2) return sh.io.Error("キーフレーム定義は２つ以上必要です");
            curveDic.Add(name,ac);
            return 0;
        }else if(args[1]=="del"){
            if(args.Count==2) return sh.io.Error("使い方: curve del 識別名 [...]");
            for(int i=2; i<args.Count; i++){
                if(curveDic.ContainsKey(sh.ns+args[i])) curveDic.Remove(sh.ns+args[i]);
            }
            return 0;
        }
        if(!curveDic.ContainsKey(sh.ns+args[1])) return sh.io.Error("指定されたカーブは見つかりません");
        ac=curveDic[sh.ns+args[1]];
        if(args.Count==2){
            foreach(Keyframe frm in ac.keys)
                sh.io.Print($"{(int)(frm.time*1000)} {sh.fmt.FVal(frm.value)} {sh.fmt.FVal(frm.inTangent)} {sh.fmt.FVal(frm.outTangent)}\n");
            return 0;
        }
        if(args.Count==3){
            if(args[2]=="del"){ curveDic.Remove(sh.ns+args[1]); return 0; }
            else if(args[2]=="linear"){LinearIp(ac); return 1;}
            else if(args[2]=="spline"){CatmulRomIp(ac,false); return 1;}
            else if(args[2]=="spline.loop"){
                if(ac.keys[0].value!=ac.keys[ac.keys.Length-1].value)
                    return sh.io.Error("先頭と末尾の値が異なります");
                CatmulRomIp(ac,true);
                return 1;
            }
            return sh.io.Error("パラメータが不正です");
        }
        return CmdParamCurve(sh,ac,args[2],args[3]);
    }
    private static int CmdParamCurve(ComShInterpreter sh,AnimationCurve ac,string cmd,string val){
        if(cmd=="loop"){
            ac.preWrapMode=WrapMode.Loop;
            ac.postWrapMode=WrapMode.Loop;
        } else if(cmd=="pingpong"){
            ac.preWrapMode=WrapMode.PingPong;
            ac.postWrapMode=WrapMode.PingPong;
        } else if(cmd=="once"){
            ac.preWrapMode=WrapMode.Once;
            ac.postWrapMode=WrapMode.ClampForever;
        } else return sh.io.Error("パラメータが不正です");

        if(!float.TryParse(val,out float time)) return sh.io.Error("数値の指定が不正です");
        sh.io.PrintLn(sh.fmt.FVal(ac.Evaluate(time/1000)));
        return 0;
    }
    private static void LinearIp(AnimationCurve ac){
        var ka=ac.keys; // コピーが返される
        for(int i=1; i<ac.length; i++){
            float dt=ka[i].time-ka[i-1].time;
            float dv=ka[i].value-ka[i-1].value;
            ka[i-1].outTangent=ka[i].inTangent=dv/dt;
        }
        ac.keys=ka;     // コピーなので書き戻しが必要
    }
    private static void CatmulRomIp(AnimationCurve ac,bool loopq){
        var ka=ac.keys;
        int tail=ac.length-1;
        if(loopq){
            float d;
            if(tail-1!=1){
                d=(ka[1].value-ka[tail-1].value)/(ka[1].time-ka[tail-1].time);
                if(float.IsNaN(d)) d=0; // これは入力値がおかしいけどね
            }else d=0;
            ka[0].inTangent=ka[0].outTangent=ka[tail].inTangent=ka[tail].outTangent=d;
        }else{
            ka[0].inTangent=0;
            ka[0].outTangent=(ka[1].value-ka[0].value)/(ka[1].time-ka[0].time);
            ka[tail].inTangent=(ka[tail].value-ka[tail-1].value)/(ka[tail].time-ka[tail-1].time);
            ka[tail].outTangent=0;
        }
        for(int i=1; i<ac.length-1; i++){
            float d=ka[i+1].time-ka[i-1].time;
            if(d<0.001) d=0; d=(ka[i+1].value-ka[i-1].value)/d;
            ka[i].outTangent=ka[i].inTangent=d;
        }
        ac.keys=ka;
    }

    private static int CmdTmpFile(ComShInterpreter sh,List<string> args){
        if(args.Count==1){
            foreach(string name in DataFiles.TmpFileList()) sh.io.PrintLn(name);
            return 0;
        }
        if(args[1]=="add"){
            if( ((args.Count!=3&&args.Count!=4)||args[2]=="") || (args.Count==4 && args[3]=="") )
                return sh.io.Error("使い方: tmpfile add 識別名 [ファイル名|*識別名]");
            if(DataFiles.CreateTempFile(args[2],(args.Count==4)?args[3]:null)==null) return sh.io.Error("作成に失敗しました");
        }else if(args[1]=="del"){
            if(args.Count<3) return sh.io.Error("使い方: tmpfile del 識別名...");
            for(int i=2; i<args.Count; i++) DataFiles.DeleteTempFile(args[i]);
        }else if(args.Count>=3){
            // 初期は tmpfile export 識別名 の順になっていたので、後方互換性のためその順番でも通す
            int ididx=2,cmdidx=1;
            if(DataFiles.IsTempFile(args[1])){ ididx=1; cmdidx=2;}
            string id=args[ididx];
            if(args[cmdidx]=="export"){
                if((args.Count!=3&&args.Count!=4)||id=="") return sh.io.Error("使い方: tmpfile 識別名 export [出力ファイル名]");
                string name=(args.Count==4)?args[3]:id;
                if(UTIL.CheckFileName(name)<0) return sh.io.Error("出力ファイル名が不正です");
                int ret=DataFiles.ExportTmpFile(id,name,ComShInterpreter.scriptFolder+"export\\");
                if(ret==-1) return sh.io.Error("未登録です");
                else if(ret==-2) return sh.io.Error("ファイルの書き込みに失敗しました");
            }else if(args[cmdidx]=="mypose"){
                if((args.Count!=3&&args.Count!=4)||id=="") return sh.io.Error("使い方: tmpfile 識別名 mypose [出力ファイル名]");
                string name=(args.Count==4)?args[3]:id;
                if(UTIL.CheckFileName(name)<0) return sh.io.Error("出力ファイル名が不正です");
                name=UTIL.Suffix(name,".anm");
                int ret=DataFiles.ExportTmpFile(id,name,ComShInterpreter.myposeDir);
                if(ret==-1) return sh.io.Error("未登録です");
                else if(ret==-2) return sh.io.Error("ファイルの書き込みに失敗しました");
            }else if(args[cmdidx]=="append"){
                if(args.Count!=4||id=="") return sh.io.Error("使い方: tmpfile 識別名 append 文字列");
                int ret=DataFiles.AppendTmpFile(id,args[3]);
                if(ret==-1) return sh.io.Error("未登録です");
                else if(ret==-2) return sh.io.Error("ファイルの書き込みに失敗しました");
            }else return sh.io.Error("パラメータが不正です");
        }else return sh.io.Error("パラメータが不正です");
        return 0;
    }
    public class Q {
        public int dim;
        public int count=0;
        public int max=0;
        public int ip=0;
        public float[][] buf;
        public Q(int len,int d){
            dim=d;
            max=len;
            buf=new float[len][];
        }
        public Q Clone(){
            var ret=new Q(max,dim);
            ret.count=count;
            ret.ip=ip;
            for(int i=0; i<max; i++) if(buf[i]==null) ret.buf[i]=null; else{
                var fa=new float[dim];
                Array.Copy(buf[i],0,fa,0,dim);
                ret.buf[i]=fa;
            }
            return ret;
        }
        public void EnQ(float[] val){
            float[] f=new float[dim];
            for(int i=0; i<dim; i++) f[i]=val[i];
            buf[ip++]=f;
            if(ip==buf.Length) ip=0;
            if(count<buf.Length) count++;
        }
        public float[] DeQ(){
            float[] ret=Peek();
            count--;
            return ret;
        }
        public float[] Peek(){ return Peek(0); }
        public float[] Peek(int n){
            if(count==0) return null;
            int op=ip-count+n;
            if(op<0) op+=buf.Length;
            return buf[op];
        }
        public void Clear(){
            ip=0; count=0;
        }
        public float[][] List(){
            var fl=new float[count][];
            for(int i=0; i<count; i++){
                int n=ip-count+i;
                if(n<0) n+=buf.Length;
                fl[i]=buf[n];
            }
            return fl;
        }
        public float[] Average(){
            var da=new double[dim];
            for(int i=1; i<=count; i++){
                int n=ip-i;
                if(n<0) n+=buf.Length;
                for(int d=0; d<dim; d++) da[d]+=buf[n][d];
            }
            for(int d=0; d<dim; d++) da[d]/=count;
            float[] f=new float[dim];
            for(int d=0; d<dim; d++) f[d]=(float)da[d];
            return f;
        }
        public float[][] BBox(){
            var min=new float[dim];
            var max=new float[dim];
            for(int d=0; d<dim; d++){ min[d]=float.MaxValue; max[d]=float.MinValue; }

            for(int i=1; i<=count; i++){
                int n=ip-i;
                if(n<0) n+=buf.Length;
                for(int d=0; d<dim; d++){
                    min[d]=Mathf.Min(min[d],buf[n][d]);
                    max[d]=Mathf.Max(max[d],buf[n][d]);
                }
            }
            return new float[][]{min,max};
        }
        public float Distance(){
            if(count<=1) return 0;
            float l=0;
            int n=ip-1;
            if(n<0) n+=buf.Length;
            float[] fa=buf[n];
            for(int i=2; i<=count; i++){
                n=ip-i;
                if(n<0) n+=buf.Length;
                if(dim==1) l+=Mathf.Abs(buf[n][0]-fa[0]);
                else{
                    float t=0;
                    for(int d=0; d<dim; d++){float off=buf[n][d]-fa[d]; t+=off*off;}
                    l+=Mathf.Sqrt(t);
                }
                fa=buf[n];
            }
            return l;
        }
        private static string queuedir=ComShInterpreter.scriptFolder+@"queue\";
        private int mkQueueDir(){
            try{
                if(!Directory.Exists(queuedir)) Directory.CreateDirectory(queuedir);
            }catch{ return -1; }
            return 0;
        }
        private static string queueformat="0.########";
        public int Save(string fn){
            string file=UTIL.GetFullPath(fn,queuedir);
            if(file=="") return -1;
            if(mkQueueDir()<0) return -2;
            try{
                using (var wr=new StreamWriter( file,false,System.Text.Encoding.UTF8)){
                    for(int i=0; i<count; i++){
                        int n=ip-count+i;
                        if(n<0) n+=buf.Length;
                        if(i>0) wr.Write("\n");
                        wr.Write(buf[n][0].ToString(queueformat));
                        for(int d=1; d<dim; d++){ wr.Write("\t"); wr.Write(buf[n][d].ToString(queueformat));}
                    }
                }
            }catch(Exception e){ Debug.Log(e.ToString()); return -2; }
            return 0;
        }
        public int Load(string fn){
            string file=UTIL.GetFullPath(fn,queuedir);
            if(file=="") return -1;
            if(!File.Exists(file)) return -1;
            try{ 
                string[] lines=File.ReadAllLines(file,System.Text.Encoding.UTF8);
                int n=lines.Length;
                for(int i=lines.Length-1; i>=0; i--) if(lines[i]=="") n--; else break;

                var tmp=this.Clone();   // エラーがあると中途半端に汚れるので一旦複製
                for(int i=0; i<lines.Length; i++){
                    float[] fa=ParseUtil.FloatArr2(lines[i],'\t',dim);
                    if(fa==null||fa.Length<dim) return -3;
                    tmp.EnQ(fa);
                }
                count=tmp.count;
                ip=tmp.ip;
                buf=tmp.buf;
            }catch(Exception e){ Debug.Log(e.ToString()); return -2; }
            return 0;
        }
        public static Q empty=new Q(0,0);
        public static string error="";
        public static Q FromFile(string fn){
            error="";
            string file=UTIL.GetFullPath(fn,queuedir);
            if(file==""){ error="ファイル名が不正です"; return null; }
            if(!File.Exists(file)){ error="ファイルが見つかりません"; return null; }
            try{ 
                string[] lines=File.ReadAllLines(file);
                int n=lines.Length;
                for(int i=lines.Length-1; i>=0; i--) if(lines[i]=="") n--; else break;
                if(n==0) return empty;
                float[] fa=ParseUtil.FloatArr2(lines[0],'\t');
                int c=fa.Length;
                if(c==0) return empty;
                var ret=new Q(n,c);
                ret.EnQ(fa);
                for(int i=1; i<lines.Length; i++){
                    fa=ParseUtil.FloatArr2(lines[i],'\t',c);
                    if(fa==null||fa.Length<c){ error="数値が不正です"; return null; }
                    ret.EnQ(fa);
                }
                return ret;
            }catch(Exception e){ Debug.Log(e.ToString()); error="読み込みに失敗しました"; return null; }
        }
    }
    public static Dictionary<string,Q> queDic=new Dictionary<string,Q>();
    private static int CmdQueue(ComShInterpreter sh,List<string> args){
        Q q;
        if(args.Count==1){
            foreach(var k in queDic.Keys) if(k.StartsWith(sh.ns,StringComparison.Ordinal)){
                q=queDic[k];
                sh.io.PrintLn($"{k}{sh.ofs}{q.count}/{q.max}");
            }
            return 0;
        }
        if(args[1]=="add"){
            if(args.Count!=5) return sh.io.Error("使い方: queue add 識別名 次元 要素数");
            if(!UTIL.ValidName(args[2])) return sh.io.Error("その名前は使用できません");
            string name=sh.ns+args[2];
            if(queDic.ContainsKey(name)) return sh.io.Error("その名前は既に使われています");
            if(!float.TryParse(args[3],out float d) || d<1 || d>20) return sh.io.Error("次元が不正です");
            if(!float.TryParse(args[4],out float n) || n<2 || n>1000000) return sh.io.Error("要素数が不正です");
            q=new Q(Mathf.RoundToInt(n),Mathf.RoundToInt(d));
            queDic[name]=q;
            return 0;
        }else if(args[1]=="load"){
            if(args.Count!=4) return sh.io.Error("使い方: queue load 識別名 ファイル名");
            if(!UTIL.ValidName(args[2])) return sh.io.Error("その名前は使用できません");
            string name=sh.ns+args[2];
            if(queDic.ContainsKey(name)) return sh.io.Error("その名前は既に使われています");
            q=Q.FromFile(args[3]);
            if(q==null) return sh.io.Error(Q.error);
            if(q.dim==0||q.max==0) return sh.io.Error("ファイル形式が不正です");
            queDic[name]=q;
            return 0;
        }else if(args[1]=="del"){
            for(int i=2; i<args.Count; i++) queDic.Remove(sh.ns+args[i]);
            return 0;
        }
        if(!queDic.TryGetValue(sh.ns+args[1],out q)) return sh.io.Error("指定されたキューは見つかりません");
        if(args.Count==2){
            sh.io.Print($"{q.dim}{sh.ofs}{q.max}");
            return 0;
        }
        int ret=0;
        for(int i=0; i<(args.Count-2)/2; i++) if((ret=CmdParamQueue(sh,q,args[2+i*2],args[2+i*2+1]))<=0) return ret;
        if((args.Count&1)>0) return CmdParamQueue(sh,q,args[args.Count-1],"");
        return 0;
    }
    private static int CmdParamQueue(ComShInterpreter sh,Q q,string cmd,string val){
        if(cmd=="enq"){
            float[] fa=ParseUtil.FloatArr(val);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            if(fa.Length!=q.dim) return sh.io.Error("値の次元がキューのものと異なります");
            float[] f=new float[q.dim];
            for(int i=0; i<fa.Length; i++) f[i]=fa[i];
            q.EnQ(f);
            return 1;
        } else if(cmd=="deq"){
            float[] f=q.DeQ();
            if(f==null) return sh.io.Error("キューが空です");
            PrintNVec(sh,f);
            return 0;
        } else if(cmd=="peek"){
            float n;
            if(val=="") n=0;
            else if (!float.TryParse(val,out n) || n<0 || n>=q.count) return sh.io.Error("参照位置が不正です");
            float[] f=q.Peek(Mathf.RoundToInt(n));
            if(f==null) return sh.io.Error("キューが空です");
            PrintNVec(sh,f);
            return 0;
        } else if(cmd=="clear"){
            q.Clear();
            return 0;
        } else if(cmd=="save"){
            int ret=q.Save(val);
            if(ret<0) return sh.io.Error("書き込みに失敗しました");
            return 0;
        } else if(cmd=="load"){
            int ret=q.Load(val);
            if(ret==-3) return sh.io.Error("数値が不正です");
            else if(ret<0) return sh.io.Error("読み込みに失敗しました");
            return 0;
        } else if(cmd=="list"){
            if(q.count>0){
                float[][] fl=q.List();
                for(int i=0; i<fl.Length; i++){
                    PrintNVec(sh,fl[i]);
                    sh.io.Print("\n");
                }
            }
            return 0;
        } else if(cmd=="count"){
            sh.io.Print(q.count.ToString());
            return 0;
        } else if(cmd=="average"){
            if(q.count==0) return sh.io.Error("キューが空です");
            PrintNVec(sh,q.Average());
            return 0;
        } else if(cmd=="distance"){
            if(q.count==0) return sh.io.Error("キューが空です");
            sh.io.Print(sh.fmt.FVal(q.Distance()));
            return 0;
        } else if(cmd=="bbox"){
            if(q.count==0) return sh.io.Error("キューが空です");
            float[][] bb=q.BBox();
            PrintNVec(sh,bb[0]);
            sh.io.Print(",");
            PrintNVec(sh,bb[1]);
            return 0;
        } else return sh.io.Error("パラメータが不正です");
    }
    private static void PrintNVec(ComShInterpreter sh,float[] f){
        sh.io.Print(sh.fmt.FVal(f[0]));
        for(int i=1; i<f.Length; i++) sh.io.Print(","+sh.fmt.FVal(f[i]));
    }
    private static int CmdProjector(ComShInterpreter sh,List<string> args){
        Projector proj;
        if(args.Count==1){
            var lst=CmdObjects.GetObjList(sh);
            foreach(var tr in lst) if(tr.GetComponent<Projector>()) sh.io.PrintLn($"{tr.name} {sh.fmt.FPos(tr.position)}");
            return 0;
        }
        if(args[1]=="add"){
            if(args.Count!=3) return sh.io.Error("使い方: projector add 識別名");
            Transform pftr=ObjUtil.GetPhotoPrefabTr(sh);
            if(pftr==null) return sh.io.Error("オブジェクト作成に失敗しました"); 
            string name=args[2];
            if(!UTIL.ValidName(args[2])) return sh.io.Error("その名前は使用できません");
            if(ObjUtil.FindObj(sh,name)!=null||LightUtil.FindLight(sh,name)!=null) return sh.io.Error("その名前は既に使われています");
            var go=ObjUtil.LoadAssetBundle<GameObject>("projector","Projector",ComShInterpreter.scriptFolder+@"asset\");
            if(go==null) return sh.io.Error("失敗しました");
            go.name=name;
            ObjUtil.objDic[name]=go.transform;
            return 0;
        }else if(args[1]=="del"){
            for(int i=2; i<args.Count; i++) if(ObjUtil.DeleteObj<Projector>(sh,args[i])<0) return -1;
            return 0;
        }
        Transform objtr;
        if(!ObjUtil.objDic.TryGetValue(args[1],out objtr)) return sh.io.Error("指定されたプロジェクターは見つかりません");
        proj=objtr.GetComponent<Projector>();
        if(proj==null) return sh.io.Error("指定されたプロジェクターは見つかりません");
        if(args.Count==2){
            sh.io.Print($"position:{sh.fmt.FPos(proj.transform.position)}\nrotation:{sh.fmt.FEuler(proj.transform.rotation.eulerAngles)}\nshader:{proj.material.shader.name}");
            return 0;
        }
        int ret=0;
        for(int i=0; i<(args.Count-2)/2; i++) if((ret=CmdParamProjector(sh,proj,args[2+i*2],args[2+i*2+1]))<=0) return ret;
        if((args.Count&1)>0) return CmdParamProjector(sh,proj,args[args.Count-1],null);
        return 0;
    }
    private static int CmdParamProjector(ComShInterpreter sh,Projector proj,string cmd,string val){
        if(cmd=="range"){
            if(val==null){
                sh.io.PrintJoin(sh.fmt.FXY(proj.nearClipPlane,proj.farClipPlane));
                return 0;
            }
            float[] fa=ParseUtil.FloatArr(val);
            if(fa==null||fa.Length!=2) return sh.io.Error("数値が不正です");
            proj.nearClipPlane=fa[0];
            proj.farClipPlane=fa[1];
        }else if(cmd=="fov"){
            if(val==null){
                sh.io.Print(sh.fmt.FInt(proj.orthographic?0:proj.fieldOfView));
                return 0;
            }
            if(!float.TryParse(val,out float f)||f<=0||f>=180) return sh.io.Error("視野角は1～179度で指定してください");
            proj.fieldOfView=f;
            proj.orthographic=false;
        }else if(cmd=="size"){
            if(val==null){
                sh.io.Print(sh.fmt.FInt(proj.orthographic?proj.orthographicSize:0));
                return 0;
            }
            if(!float.TryParse(val,out float f)||f<=0) return sh.io.Error("数値が不正です");
            proj.orthographic=true;
            proj.orthographicSize=f*0.5f;
        }else if(cmd=="aspect"){
            if(val==null){
                sh.io.Print(sh.fmt.F0to1(proj.aspectRatio));
                return 0;
            }
            if(!float.TryParse(val,out float f)||f<0) return sh.io.Error("数値が不正です");
            proj.aspectRatio=f;
        }else if(cmd=="mask"){
            if(val==null){
                sh.io.Print(proj.ignoreLayers.ToString("X8"));
                return 0;
            }
            if(!int.TryParse(val,System.Globalization.NumberStyles.HexNumber,null,out int bits)) return sh.io.Error("数値が不正です");
            proj.ignoreLayers=~bits;
        }else if(cmd=="texture"){
            if(val==null){
                CmdMeshes.PrintTextureInfo(sh,proj.material,"_ShadowTex");
                return 0;
            }
            Video.KillVideo(proj.transform);
            string msg=CmdMeshes.SetTexProp(proj.material,"_ShadowTex",val,null,0);
            if(msg!="") return sh.io.Error(msg);
        }else if(cmd=="falloff"){
            if(val==null){
                CmdMeshes.PrintTextureInfo(sh,proj.material,"_FalloffTex");
                return 0;
            }
            string msg=CmdMeshes.SetTexProp(proj.material,"_FalloffTex",val,null,0);
            if(msg!="") return sh.io.Error(msg);
        }else if(cmd=="blend"){
            if(val==null){
                string srctxt=GetBlendFactorTxt(proj.material.GetInt("_SrcBlend"));
                string dsttxt=GetBlendFactorTxt(proj.material.GetInt("_DstBlend"));
                sh.io.PrintJoin(",",srctxt,dsttxt);
                return 0;
            }
            string[] sa=val.Split(ParseUtil.comma);
            if(sa.Length!=2) return sh.io.Error("書式が不正です");
            int src=GetBlendFactor(sa[0]),dst=GetBlendFactor(sa[1]);
            if(src<0||dst<0) return sh.io.Error("値が不正です");
            proj.material.SetInt("_SrcBlend",src);
            proj.material.SetInt("_DstBlend",dst);
        }else if(cmd=="uvwh"){
            if(val==null){
                var offset=proj.material.GetTextureOffset("_ShadowTex");
                var scale=proj.material.GetTextureScale("_ShadowTex");
                sh.io.PrintJoin(",",sh.fmt.FXY(offset),sh.fmt.FXY(scale));
                return 0;
            }
            float[] xywh={0,0,0,0};
            int n=ParseUtil.XyzSub(val,xywh);
            if(n<0||(n!=2&&n!=4)) return sh.io.Error("書式が不正です");
            var mate=proj.material;
            mate.SetTextureOffset("_ShadowTex",new Vector2(xywh[0],xywh[1]));
            if(n==2) return 1;
            mate.SetTextureScale("_ShadowTex",new Vector2(xywh[2],xywh[3]));
        }else if(cmd=="rq"){
            if(val==null){
                sh.io.Print(proj.material.renderQueue.ToString());
                return 0;
            }
            if(!int.TryParse(val,out int n)||n<0) return sh.io.Error("数値が不正です");
            proj.material.renderQueue=n;
        }else if(cmd=="color"){
            if(val==null){
                sh.io.Print(sh.fmt.RGBA(proj.material.GetColor("_Color")));
                return 0;
            }
            float[] fa=ParseUtil.RgbaLenient(val);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            proj.material.SetColor("_Color",new Color(fa[0],fa[1],fa[2],fa[3]));
        }else if(cmd=="bgcolor"){
            if(val==null){
                sh.io.Print(sh.fmt.RGBA(proj.material.GetColor("_BgColor")));
                return 0;
            }
            float[] fa=ParseUtil.Rgba(val);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            proj.material.SetColor("_BgColor",new Color(fa[0],fa[1],fa[2],fa[3]));
        }else if(cmd=="video"){
            if(val==null){ sh.io.Print(Video.CurrentVideoName(proj.transform)); return 0;}
            if(val==""){ Video.KillVideo(proj.transform); return 1; }
            string[] sa=val.Split(ParseUtil.comma);
            if(sa.Length<1||sa.Length>4) return sh.io.Error("書式が不正です");
            string fname=Video.VideoFileCheck(sa[0]);
            if(fname==null) return sh.io.Error("ファイルが見つかりません");
            int loopq=0;
            int w=1024,h=1024;
            if(sa.Length>=2){
                if(!int.TryParse(sa[1],out w)||w<0) return sh.io.Error("数値が不正です");
                h=w;
            }
            if(sa.Length>=3){
                if(!int.TryParse(sa[2],out h)||h<0 ) return sh.io.Error("数値が不正です");
            }
            if(sa.Length==4){
                if(!int.TryParse(sa[3],out loopq)||(loopq!=0&&loopq!=1)) return sh.io.Error("数値が不正です");
            }
            int ret=Video.VideoLoad(proj.transform,proj.material,"_ShadowTex",fname,w,h,loopq);
            if(ret<0) return sh.io.Error("失敗しました");
        }else if(cmd=="video.loop"){
            return Video.VideoLoop(sh,proj.transform,val);
        }else if(cmd=="video.speed"){
            return Video.VideoSpeed(sh,proj.transform,val);
        }else if(cmd=="video.time"){
            return Video.VideoTime(sh,proj.transform,val);
        }else if(cmd=="video.timep"){
            return Video.VideoTimep(sh,proj.transform,val);
        }else if(cmd=="video.length"){
            return Video.VideoLength(sh,proj.transform,val);
        }else if(cmd=="video.mute"){
            return Video.VideoMute(sh,proj.transform,val);
        }else if(cmd=="video.a3d"){
            return Video.VideoAudio3D(sh,proj.transform,val);
        }else if(cmd=="video.pause"){
            return Video.VideoPause(sh,proj.transform,val);
        }else return sh.io.Error("パラメータが不正です");
        return 1;
    }
    private static int GetBlendFactor(string name){
        switch(name.ToLower()){
        case "1":
        case "one": return (int)UnityEngine.Rendering.BlendMode.One;
        case "0":
        case "zero": return (int)UnityEngine.Rendering.BlendMode.Zero;
        case "sc":
        case "srccolor": return (int)UnityEngine.Rendering.BlendMode.SrcColor;
        case "dc":
        case "dstcolor": return (int)UnityEngine.Rendering.BlendMode.DstColor;
        case "sa":
        case "srcalpha": return (int)UnityEngine.Rendering.BlendMode.SrcAlpha;
        case "da":
        case "dstalpha": return (int)UnityEngine.Rendering.BlendMode.DstAlpha;
        case "1-sc":
        case "oneminussrccolor": return (int)UnityEngine.Rendering.BlendMode.OneMinusSrcColor;
        case "1-dc":
        case "oneminusdstcolor": return (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor;
        case "1-sa":
        case "oneminussrcalpha": return (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
        case "1-da":
        case "oneminusdstalpha": return (int)UnityEngine.Rendering.BlendMode.OneMinusDstAlpha;
        }
        return -1;
    }
    private static string GetBlendFactorTxt(int mode){
        switch((UnityEngine.Rendering.BlendMode)mode){
        case UnityEngine.Rendering.BlendMode.Zero: return "0";
        case UnityEngine.Rendering.BlendMode.One: return "1";
        case UnityEngine.Rendering.BlendMode.SrcColor: return "sc";
        case UnityEngine.Rendering.BlendMode.DstColor: return "dc";
        case UnityEngine.Rendering.BlendMode.SrcAlpha: return "sa";
        case UnityEngine.Rendering.BlendMode.DstAlpha: return "da";
        case UnityEngine.Rendering.BlendMode.OneMinusSrcColor: return "1-sc";
        case UnityEngine.Rendering.BlendMode.OneMinusDstColor: return "1-dc";
        case UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha: return "1-sa";
        case UnityEngine.Rendering.BlendMode.OneMinusDstAlpha: return "1-da";
        }
        return "";
    }
    private static int CmdReflectionProbe(ComShInterpreter sh,List<string> args){
        ReflectionProbe rp;
        if(args.Count==1){
            var lst=CmdObjects.GetObjList(sh);
            foreach(var tr in lst) if(tr.GetComponent<ReflectionProbe>()) sh.io.PrintLn($"{tr.name} {sh.fmt.FPos(tr.position)}");
            return 0;
        }
        if(args[1]=="add"){
            if(args.Count!=3 && args.Count!=4) return sh.io.Error("使い方: reflectionprobe add 識別名 [解像度]");
            Transform pftr=ObjUtil.GetPhotoPrefabTr(sh);
            if(pftr==null) return sh.io.Error("オブジェクト作成に失敗しました"); 
            string name=args[2];
            if(!UTIL.ValidName(args[2])) return sh.io.Error("その名前は使用できません");
            if(ObjUtil.FindObj(sh,name)!=null||LightUtil.FindLight(sh,name)!=null) return sh.io.Error("その名前は既に使われています");
            int res=1024;
            if(args.Count==4 && !int.TryParse(args[3],out res)) sh.io.Error("数値が不正です");
            var go=new GameObject();
            if(go==null) return sh.io.Error("失敗しました");
            go.name=name;
            go.transform.localPosition=Vector3.zero;
            go.transform.localRotation=Quaternion.identity;
            ObjUtil.objDic[name]=go.transform;
            rp=go.AddComponent<ReflectionProbe>();
            rp.hdr=false;
            rp.resolution=res;
            rp.mode=UnityEngine.Rendering.ReflectionProbeMode.Realtime;
            rp.refreshMode=UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame;
            rp.timeSlicingMode=UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
            rp.center=Vector3.zero;
            rp.size=new Vector3(10,10,10);
            rp.shadowDistance=10f;
            rp.intensity=1;
            rp.importance=0;
            rp.boxProjection=true;
            rp.clearFlags=UnityEngine.Rendering.ReflectionProbeClearFlags.Skybox;
            rp.backgroundColor=Color.black;
            rp.cullingMask=-1;
            return 0;
        }else if(args[1]=="del"){
            for(int i=2; i<args.Count; i++) if(ObjUtil.DeleteObj<ReflectionProbe>(sh,args[i])<0) return -1;
            return 0;
        }
        Transform objtr;
        if(!ObjUtil.objDic.TryGetValue(args[1],out objtr)) return sh.io.Error("指定されたリフレクションプルーブは見つかりません");
        rp=objtr.GetComponent<ReflectionProbe>();
        if(rp==null) return sh.io.Error("指定されたリフレクションプルーブは見つかりません");
        if(args.Count==2){
            sh.io.PrintLn2("resolution:",rp.resolution.ToString());
            sh.io.PrintLn2("update:",refreshmode2str(rp));
            sh.io.PrintLn2("size:",sh.fmt.FPos(rp.size));
            sh.io.PrintLn2("range:",sh.fmt.FXY(rp.nearClipPlane,rp.farClipPlane));
            sh.io.PrintLn2("shadowrange:",sh.fmt.FInt(rp.shadowDistance));
            sh.io.PrintLn2("mask:",rp.cullingMask.ToString("X8"));
            sh.io.PrintLn2("power:",sh.fmt.FInt(rp.intensity));
            sh.io.PrintLn2("priority:",rp.importance.ToString());
            return 0;
        }
        int ret=0;
        for(int i=0; i<(args.Count-2)/2; i++) if((ret=CmdParamReflectionProbe(sh,rp,args[2+i*2],args[2+i*2+1]))<=0) return ret;
        if((args.Count&1)>0) return CmdParamReflectionProbe(sh,rp,args[args.Count-1],null);
        return 0;
    }
    private static string refreshmode2str(ReflectionProbe rp){
        switch(rp.refreshMode){
        case UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake:
        case UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting:
            return "0";
        case UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame:
            return (rp.timeSlicingMode==UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.AllFacesAtOnce)?"1":"2";
        }
        return "";
    }
    private static int CmdParamReflectionProbe(ComShInterpreter sh,ReflectionProbe rp,string cmd,string val){
        if(val=="") return 0;
        if(cmd=="update"){
            if(val==null){
                sh.io.Print(refreshmode2str(rp));
                return 0;
            }
            if(!float.TryParse(val,out float f)||f<0) return sh.io.Error("数値が不正です");
            if(f==0){
                rp.refreshMode=UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
            }else if(f==1){
                rp.refreshMode=UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame;
                rp.timeSlicingMode=UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
            }else if(f==2){
                rp.refreshMode=UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame;
                rp.timeSlicingMode=UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.NoTimeSlicing;
            }else if(f==3){
                rp.refreshMode=UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
                rp.RenderProbe();
            }
        }else if(cmd=="priority"){
            if(val==null){
                sh.io.Print(rp.importance.ToString());
                return 0;
            }
            if(!int.TryParse(val,out int n)||n<0) return sh.io.Error("数値が不正です");
            rp.importance=n;
        }else if(cmd=="range"){
            if(val==null){
                sh.io.Print(sh.fmt.FXY(rp.nearClipPlane,rp.farClipPlane));
                return 0;
            }
            float[] fa=ParseUtil.Xy(val);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            rp.nearClipPlane=fa[0];
            rp.farClipPlane=fa[1];
        }else if(cmd=="power"){
            if(val==null){
                sh.io.Print(sh.fmt.FInt(rp.intensity));
                return 0;
            }
            if(!float.TryParse(val,out float f)||f<0) return sh.io.Error("数値が不正です");
            rp.intensity=f;
        }else if(cmd=="shadowrange"){
            if(val==null){
                sh.io.Print(sh.fmt.FInt(rp.shadowDistance));
                return 0;
            }
            if(!float.TryParse(val,out float f)||f<0) return sh.io.Error("数値が不正です");
            rp.shadowDistance=f;
        }else if(cmd=="position"){
            if(val==null){
                sh.io.Print(sh.fmt.FPos(rp.center));
                return 0;
            }
            float[] fa=ParseUtil.Xyz(val);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            rp.center=new Vector3(fa[0],fa[1],fa[2]);
        }else if(cmd=="size"){
            if(val==null){
                sh.io.Print(sh.fmt.FPos(rp.size));
                return 0;
            }
            float[] fa=ParseUtil.Xyz(val);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            rp.size=new Vector3(fa[0],fa[1],fa[2]);
        }else if(cmd=="clrflg"){
            if(val==null){
                sh.io.Print(rp.clearFlags==UnityEngine.Rendering.ReflectionProbeClearFlags.SolidColor?"0":"1");
                return 0;
            }
            if(!int.TryParse(val,out int n)||n<0||n>1) return sh.io.Error("数値が不正です");
            if(n==0) rp.clearFlags=UnityEngine.Rendering.ReflectionProbeClearFlags.SolidColor;
            else if(n==1) rp.clearFlags=UnityEngine.Rendering.ReflectionProbeClearFlags.Skybox;
        }else if(cmd=="bgcolor"){
            if(val==null){
                sh.io.Print(sh.fmt.RGBA(rp.backgroundColor));
                return 0;
            }
            float[] fa=ParseUtil.RgbaLenient(val);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            rp.backgroundColor=new Color(fa[0],fa[1],fa[2],fa[3]);
        }else if(cmd=="mask"){
            if(val==null){
                sh.io.Print(rp.cullingMask.ToString("X8"));
                return 0;
            }
            if(!int.TryParse(val,System.Globalization.NumberStyles.HexNumber,null,out int bits))
                return sh.io.Error("数値が不正です");
            rp.cullingMask=bits;
        }else if(cmd=="box"){
            if(val==null){
                sh.io.Print(rp.boxProjection?"1":"0");
                return 0;
            }
            int onoff=ParseUtil.OnOff(val);
            if(onoff<0) return sh.io.Error(ParseUtil.error);
            rp.boxProjection=(onoff==1);

        }else if(cmd=="png"){
            if(val==null||val=="") return 0;
            string file="";
            if(val[0]=='*'){
                var tf=DataFiles.CreateTempFile(val.Substring(1),"");
                file=tf.filename;
            }else file=UTIL.GetFullPath(UTIL.Suffix(val,".png"),ComShInterpreter.textureDir);
            if(file=="") return sh.io.Error("ファイル名が不正です");

            var cubemap=(RenderTexture)rp.texture;
            var t2d=new Texture2D(cubemap.width*6,cubemap.height,TextureFormat.RGBA32,false);
            try{
                for(int i=0; i<6; i++){
                    Graphics.CopyTexture(cubemap,i,0,0,0,cubemap.width,cubemap.height,t2d,0,0,i*cubemap.width,0);
                }
                CmdMeshes.MeshParamPNGSub2(sh,t2d,file);
            }catch(Exception e){ Debug.Log(e.ToString()); return sh.io.Error("失敗しました"); }
            return 1;
        } else return sh.io.Error("パラメータが不正です");
        return 1;
    }
}
}
