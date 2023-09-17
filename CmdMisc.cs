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
        if(cmd=="scale") return _CmdParamScale;
        return null;
    }
    private static void SetBg(string val){
        BGWindow bgw=GameObject.FindObjectOfType<BGWindow>();
        ObjectManagerWindow omw=null;
        if(bgw!=null){
            omw=bgw.mgr.GetWindow(PhotoWindowManager.WindowType.ObjectManager) as ObjectManagerWindow;
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
                if(asm!=null&&asm.audiosource!=null&&asm.audiosource.isPlaying)
                    sh.io.PrintLn(((int)((asm.audiosource.time%asm.audiosource.clip.length)*1000)).ToString());
            }else{
                float t=ParseUtil.ParseFloat(val);
                if(float.IsNaN(t)||t<0) return sh.io.Error("数値の指定が不正です");
                var asm=GetASMgr(0);
                if(asm!=null&&asm.audiosource!=null&&asm.audiosource.isPlaying) asm.audiosource.time=t/1000;
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
            if(val.Length==0) sm.StopBGM(0f); else sm.PlayBGM(UTIL.Suffix(val,".ogg"), 0, true);
        }else if(cmd=="dance"){
            if(val==null){ PrintSound(sh,0); return 0;}
            if(val.Length==0) sm.StopBGM(0f); else sm.PlayDanceBGM(UTIL.Suffix(val,".ogg"), 0f,false);
        }else if(cmd=="dancep"){
            if(val==null){ PrintSound(sh,0); return 0;}
            if(val.Length==0) sm.StopBGM(0f); else sm.PlayDanceBGMParallel(UTIL.Suffix(val,".ogg"), 0f,false);
        }else if(cmd=="env"){
            if(val==null){ PrintSound(sh,1); return 0;}
            if(val.Length==0) sm.StopEnv(0f); else sm.PlayEnv(UTIL.Suffix(val,".ogg"),0f);
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
                float time,value,inTan,outTan;
                string[] sa=args[i].Split(ParseUtil.comma);
                if(sa.Length!=4) return sh.io.Error("キーフレームの書式が不正です"); 
                if(!float.TryParse(sa[0],out time)||time<0||!float.TryParse(sa[1],out value)
                 ||!float.TryParse(sa[2],out inTan)||!float.TryParse(sa[3],out outTan)) return sh.io.Error("数値の指定が不正です");
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
        int ip=0;
        float[][] buf;
        public Q(int len,int d){
            dim=d;
            max=len;
            buf=new float[len][];
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
            var fa=new float[dim];
            float l=0;
            for(int i=1; i<=count; i++){
                int n=ip-i;
                if(n<0) n+=buf.Length;
                if(dim==1) l+=buf[n][0];
                else{
                    float t=0;
                    for(int d=0; d<dim; d++) t+=buf[n][d]*buf[n][d];
                    l+=Mathf.Sqrt(t);
                }
            }
            return l;
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
            if(!float.TryParse(args[3],out float d) || d<1 || d>3) return sh.io.Error("次元が不正です");
            if(!float.TryParse(args[4],out float n) || n<2 || n>100000) return sh.io.Error("要素数が不正です");
            q=new Q(Mathf.RoundToInt(n),Mathf.RoundToInt(d));
            queDic[name]=q;
            return 0;
        }else if(args[1]=="del"){
            for(int i=2; i<args.Count; i++) queDic.Remove(sh.ns+args[i]);
            return 0;
        }
        if(!queDic.TryGetValue(sh.ns+args[1],out q)) return sh.io.Error("指定されたキューは見つかりません");
        if(args.Count==3) return CmdParamQueue(sh,q,args[2],"");
        else if(args.Count==4) return CmdParamQueue(sh,q,args[2],args[3]);
        else return 0;
    }
    private static int CmdParamQueue(ComShInterpreter sh,Q q,string cmd,string val){
        if(cmd=="enq"){
            float[] fa=ParseUtil.FloatArr(val);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            if(fa.Length!=q.dim) return sh.io.Error("値の次元がキューのものと異なります");
            float[] f=new float[q.dim];
            for(int i=0; i<fa.Length; i++) f[i]=fa[i];
            q.EnQ(f);
        } else if(cmd=="deq"){
            float[] f=q.DeQ();
            if(f==null) return sh.io.Error("キューが空です");
            PrintNVec(sh,f);
        } else if(cmd=="peek"){
            float n;
            if(val=="") n=0;
            else if (!float.TryParse(val,out n) || n<0 || n>=q.count) return sh.io.Error("参照位置が不正です");
            float[] f=q.Peek(Mathf.RoundToInt(n));
            if(f==null) return sh.io.Error("キューが空です");
            PrintNVec(sh,f);
        } else if(cmd=="clear"){
            q.Clear();
        } else if(cmd=="list"){
            if(q.count>0){
                float[][] fl=q.List();
                for(int i=0; i<fl.Length; i++){
                    PrintNVec(sh,fl[i]);
                    sh.io.Print("\n");
                }
            }
        } else if(cmd=="count"){
            sh.io.Print(q.count.ToString());
        } else if(cmd=="average"){
            if(q.count==0) return sh.io.Error("キューが空です");
            PrintNVec(sh,q.Average());
        } else if(cmd=="distance"){
            if(q.count==0) return sh.io.Error("キューが空です");
            sh.io.Print(sh.fmt.FVal(q.Distance()));
        } else if(cmd=="bbox"){
            if(q.count==0) return sh.io.Error("キューが空です");
            float[][] bb=q.BBox();
            PrintNVec(sh,bb[0]);
            sh.io.Print(",");
            PrintNVec(sh,bb[1]);
        } else return sh.io.Error("パラメータが不正です");
        return 0;
    }
    private static void PrintNVec(ComShInterpreter sh,float[] f){
        sh.io.Print(sh.fmt.FVal(f[0]));
        for(int i=1; i<f.Length; i++) sh.io.Print(","+sh.fmt.FVal(f[i]));
    }
}
}
