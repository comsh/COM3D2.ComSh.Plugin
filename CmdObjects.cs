using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static System.StringComparison;
using static COM3D2.ComSh.Plugin.Command;

namespace COM3D2.ComSh.Plugin {

public static class CmdObjects {

    public static void Init(){
        Command.AddCmd("obj",new Cmd(CmdObject));

        objParamDic.Add("del",new CmdParam<Transform>(ObjParamDel));
        objParamDic.Add("wpos",new CmdParam<Transform>(_CmdParamWPos));
        objParamDic.Add("wpos.x",new CmdParam<Transform>(_CmdParamWPosX));
        objParamDic.Add("wpos.y",new CmdParam<Transform>(_CmdParamWPosY));
        objParamDic.Add("wpos.z",new CmdParam<Transform>(_CmdParamWPosZ));
        objParamDic.Add("lpos",new CmdParam<Transform>(_CmdParamLPos));
        objParamDic.Add("lpos.x",new CmdParam<Transform>(_CmdParamLPosX));
        objParamDic.Add("lpos.y",new CmdParam<Transform>(_CmdParamLPosY));
        objParamDic.Add("lpos.z",new CmdParam<Transform>(_CmdParamLPosZ));
        objParamDic.Add("opos",new CmdParam<Transform>(_CmdParamOPos));
        objParamDic.Add("wrot",new CmdParam<Transform>(_CmdParamWRot));
        objParamDic.Add("wrot.x",new CmdParam<Transform>(_CmdParamWRotX));
        objParamDic.Add("wrot.y",new CmdParam<Transform>(_CmdParamWRotY));
        objParamDic.Add("wrot.z",new CmdParam<Transform>(_CmdParamWRotZ));
        objParamDic.Add("lrot",new CmdParam<Transform>(_CmdParamLRot));
        objParamDic.Add("lrot.x",new CmdParam<Transform>(_CmdParamLRotX));
        objParamDic.Add("lrot.y",new CmdParam<Transform>(_CmdParamLRotY));
        objParamDic.Add("lrot.z",new CmdParam<Transform>(_CmdParamLRotZ));
        objParamDic.Add("orot",new CmdParam<Transform>(_CmdParamORot));
        objParamDic.Add("scale",new CmdParam<Transform>(_CmdParamScale));
        objParamDic.Add("scale.x",new CmdParam<Transform>(_CmdParamScaleX));
        objParamDic.Add("scale.y",new CmdParam<Transform>(_CmdParamScaleY));
        objParamDic.Add("scale.z",new CmdParam<Transform>(_CmdParamScaleZ));
        objParamDic.Add("attach",new CmdParam<Transform>(ObjParamAttach));
        objParamDic.Add("detach",new CmdParam<Transform>(ObjParamDetach));
        objParamDic.Add("list",new CmdParam<Transform>(ObjParamList));
        objParamDic.Add("lquat",new CmdParam<Transform>(_CmdParamLQuat));
        objParamDic.Add("wquat",new CmdParam<Transform>(_CmdParamWQuat));
        objParamDic.Add("material",new CmdParam<Transform>(ObjParamMaterial));
        objParamDic.Add("shape",new CmdParam<Transform>(ObjParamShape));
        objParamDic.Add("prefix",new CmdParam<Transform>(ObjParamPrefix));
        objParamDic.Add("prot",new CmdParam<Transform>(_CmdParamPRot));
        objParamDic.Add("pquat",new CmdParam<Transform>(_CmdParamPQuat));
        objParamDic.Add("name",new CmdParam<Transform>(ObjParamName));
        objParamDic.Add("motion",new CmdParam<Transform>(ObjParamMotion));
        objParamDic.Add("particle",new CmdParam<Transform>(ObjParamParticle));
        objParamDic.Add("handle",new CmdParam<Transform>(ObjParamHandle));
        objParamDic.Add("describe",new CmdParam<Transform>(ObjParamDesc));
        objParamDic.Add("type",new CmdParam<Transform>(ObjParamType));
        objParamDic.Add("mesh",new CmdParam<Transform>(ObjParamMesh));
        objParamDic.Add("component",new CmdParam<Transform>(ObjParamComponent));
        
        objParamDic.Add("l2w",new CmdParam<Transform>(_CmdParamL2W));
        objParamDic.Add("w2l",new CmdParam<Transform>(_CmdParamW2L));

        CmdParamPosRotCp(objParamDic,"lpos","position");
        CmdParamPosRotCp(objParamDic,"lpos","pos");
        CmdParamPosRotCp(objParamDic,"lrot","rotation");
        CmdParamPosRotCp(objParamDic,"lrot","rot");
    }

    private static Dictionary<string,CmdParam<Transform>> objParamDic=new Dictionary<string,CmdParam<Transform>>();

    private static int CmdObject(ComShInterpreter sh,List<string> args){
        Transform pftr;
        if(args.Count==1){
            var oset=new HashSet<string>(); // 重複削除
            if(sh.objRef.Length>0){ // スタジオモード分参照
                pftr= UTIL.GetObjRoot(sh.objRef);
                if(pftr!=null) for(int i=0; i<pftr.childCount; i++){
                    Transform tr=pftr.GetChild(i);
                    if(tr==null) continue;
                    oset.Add(tr.name);
                    sh.io.PrintJoinLn(sh.ofs,tr.name,sh.fmt.FPos(tr.position));
                }
            }
            // 現root直下のobject
            if((pftr=ObjUtil.GetPhotoPrefabTr(sh))!=null){
                for(int i=0; i<pftr.childCount; i++){
                    Transform tr=pftr.GetChild(i);
                    if(tr==null) continue;
                    oset.Add(tr.name);
                    sh.io.PrintJoinLn(sh.ofs,tr.name,sh.fmt.FPos(tr.position));
                }
            }
            List<string> remove=new List<string>();
            foreach(var kv in ObjUtil.objDic){
                Transform tr=kv.Value;
                if(tr==null){ remove.Add(kv.Key); continue; }
                if(oset.Contains(tr.name)) continue; // root直下のものは取得済
                // 何かにアタッチされているもの
                sh.io.PrintJoinLn(sh.ofs,kv.Key,(tr.parent!=null)?tr.parent.name:"orphan",sh.fmt.FPos(tr.position));
            }
            foreach(string k in remove) ObjUtil.objDic.Remove(k);
            return 0;
        }
        if(args[1]=="add"){
            if(args.Count==2) return sh.io.Error("使い方: obj add 種類 [識別名 位置 回転姿勢 スケール]");
            if(args.Count>7) return sh.io.Error("引数が多すぎます");
            string[] pa=ParseUtil.NormalizeParams(args,new string[]{"","","0,0,0","0,0,0","1,1,1"},2);
            if(pa[1]=="") pa[1]=AutoObjName(pa[0]);
            if(!UTIL.ValidName(pa[1])) return sh.io.Error("その名前は使用できません");
            if((pftr=ObjUtil.GetPhotoPrefabTr(sh,true))==null) return sh.io.Error("オブジェクト作成に失敗しました");
            if(ObjUtil.FindObj(sh,pa[1])!=null||LightUtil.FindLight(sh,pa[1])!=null) return sh.io.Error("その名前は既に使われています");
            float[] pos,rot,scl;
            if((pos=ParseUtil.Xyz(pa[2]))==null) return sh.io.Error(ParseUtil.error);
            if((rot=ParseUtil.Xyz(pa[3]))==null) return sh.io.Error(ParseUtil.error);
            if((scl=ParseUtil.Xyz2(pa[4]))==null) return sh.io.Error(ParseUtil.error);
            GameObject go=ObjUtil.AddObject(pa[0],pa[1],pftr,
               new Vector3(pos[0],pos[1],pos[2]),new Vector3(rot[0],rot[1],rot[2]),new Vector3(scl[0],scl[1],scl[2]));
            if(go==null) return sh.io.Error("オブジェクト作成に失敗しました");
            ObjUtil.objDic.Add(go.transform.name,go.transform);
            return 0;
        }
        if(args[1]=="create"){
            if(args.Count==2) return sh.io.Error("使い方: obj create 種類 [識別名]");
            if(args.Count>4) return sh.io.Error("引数が多すぎます");
            string type=args[2]; string name=(args.Count==3)?AutoObjName(type):args[3];
            if(!UTIL.ValidName(name)) return sh.io.Error("その名前は使用できません");
            if((pftr=ObjUtil.GetPhotoPrefabTr(sh,true))==null) return sh.io.Error("オブジェクト作成に失敗しました");
            if(ObjUtil.FindObj(sh,name)!=null||LightUtil.FindLight(sh,name)!=null) return sh.io.Error("その名前は既に使われています");
            GameObject go;
            if(type=="cube"){
                go=GameObject.CreatePrimitive(PrimitiveType.Cube);
            }else if(type=="sphere"){
                go=GameObject.CreatePrimitive(PrimitiveType.Sphere);
            }else if(type=="cylinder"){
                go=GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            }else if(type=="quad"){
                go=GameObject.CreatePrimitive(PrimitiveType.Quad);
            }else if(type=="plane"){
                go=GameObject.CreatePrimitive(PrimitiveType.Plane);
            }else if(type=="capsule"){
                go=GameObject.CreatePrimitive(PrimitiveType.Capsule);
            }else return sh.io.Error("種類にはcube|sphere|cylinder|quad|plane|capsuleのいずれかを指定してください");
            UTIL.ResetTr(go.transform);
            var o=new GameObject(name);
            UTIL.ResetTr(o.transform);
            go.transform.SetParent(o.transform);
            ObjInfo.AddObjInfo(o,"."+type);
            o.transform.SetParent(pftr);
            ObjUtil.objDic.Add(o.transform.name,o.transform);
            return 0;
        }
        if(args[1]=="clone"){
            if(args.Count!=4) return sh.io.Error("使い方: obj clone コピー元識別名 コピー先識別名");
            Transform tr=ObjUtil.FindObj(sh,args[2].Split(ParseUtil.colon));
            if(tr==null) return sh.io.Error("対象が見つかりません");
            if(!UTIL.ValidName(args[3])) return sh.io.Error("その名前は使用できません");
            if((pftr=ObjUtil.GetPhotoPrefabTr(sh,true))==null) return sh.io.Error("オブジェクト作成に失敗しました");
            if(ObjUtil.FindObj(sh,args[3])!=null||LightUtil.FindLight(sh,args[3])!=null) return sh.io.Error("その名前は既に使われています");
            GameObject go=ObjUtil.CloneObject(args[3],tr,pftr);
            if(go==null) return sh.io.Error("オブジェクト作成に失敗しました");
            ObjUtil.objDic.Add(go.transform.name,go.transform);
		    return 0;
        }
        if(args[1]=="del" && args.Count>=3){
            for(int i=2; i<args.Count; i++){
                Transform tr=ObjUtil.FindObj(sh,args[i].Split(ParseUtil.colon));
                if(tr!=null){
                    tr.parent=null; // 次フレームまでは消えてくれないので、FindObj()で探せなくしておく
                    UnityEngine.Object.Destroy(tr.gameObject);
                    ObjUtil.objDic.Remove(tr.name);
                }
            }
            return 0;
        }
        return CmdObjectSub(sh,args[1].Split(ParseUtil.colon),args,2);
    }
    private static string AutoObjName(string name){
        string seq=UTIL.GetSeqId();
        string ret=string.Concat(System.IO.Path.GetFileNameWithoutExtension(name).Replace(' ','_'),"_",seq);
        if(!UTIL.ValidName(ret)) ret="object_"+seq;
        return ret;
    }
    public static int CmdObjectSub(ComShInterpreter sh,string[] sa,List<string> args,int startpos){
        if(ObjUtil.GetPhotoPrefabTr(sh,true)==null) return sh.io.Error("オブジェクトが存在しません");
        Transform tr=ObjUtil.FindObj(sh,sa);
        if(tr==null) return sh.io.Error("オブジェクトが存在しません");
        if(args.Count==startpos){
            sh.io.PrintLn2("iid:",tr.gameObject.GetInstanceID().ToString());
            UTIL.PrintTrInfo(sh,tr);
            return 0;
        }
        return ParamLoop(sh,tr,objParamDic,args,startpos);
    }

    private static int ObjParamDel(ComShInterpreter sh,Transform tr,string val){
        UnityEngine.Object.Destroy(tr.gameObject);
        ObjUtil.objDic.Remove(tr.name);
        return 0;
    }
    private static int ObjParamAttach(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            sh.io.Print(tr.parent.name);
            return 0;
        }
        Transform to;
        int opt,jmpq=0;
        if((opt=val.IndexOf(','))>=0){
            if(!int.TryParse(val.Substring(opt+1),out jmpq)) return sh.io.Error("数値の形式が不正です");
            val=val.Substring(0,opt);
        }
        string[] sa=val.Split(ParseUtil.colon);
        if(sa.Length==3){
            to=BoneUtil.FindBone(sh,val);
            if(to==null) return sh.io.Error("対象が見つかりません");
        }else if(sa.Length==2 && sa[0]=="obj"){
            to=ObjUtil.FindObj(sh,sa[1]);
            if(to==null) return sh.io.Error("対象が見つかりません");
        }else if(sa.Length==1){
            to=ObjUtil.FindObj(sh,sa[0]);
            if(to==null) return sh.io.Error("対象が見つかりません");
        } else return sh.io.Error("アタッチ先を指定してください"); 
        // attach cronの実行順更新
        var ms=MaidUtil.GetParentMaidList(to,tr);
        if(ms==null) return sh.io.Error("親子関係がループになるため、アタッチできません");
        if(ms.Count>0){
            int prio=0;
            for(int i=ms.Count-1; i>=0; i--)  // 親のcron実行順。デタッチ後は値が飛ぶので更新は必要
                 ComShBg.cron.ChangePriority(MaidUtil.CRON_ATTACH+ms[i].GetInstanceID().ToString(),prio++);
            while((ms=MaidUtil.GetChildMaidList(ms.GetRange(ms.Count-1,1))).Count>0){ // 子の実行順を階層ごとに更新
                foreach(Maid maid in ms)
                    ComShBg.cron.ChangePriority(MaidUtil.CRON_ATTACH+maid.GetInstanceID().ToString(),prio);
                prio++;
            }
        }
        if(jmpq==2) UTIL.ResetTr(tr);
        tr.SetParent(to,jmpq==0);
        return 1;
    }
    private static int ObjParamDetach(ComShInterpreter sh,Transform tr,string val){
        var pftr=ObjUtil.GetPhotoPrefabTr(sh);
        if(pftr==null) return sh.io.Error("失敗しました");
        if(tr.parent!=pftr) tr.SetParent(pftr,true);
        return 1;
    }
    private static int ObjParamList(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return sh.io.Error("list種別に child か descendant を指定してください");
        if(val=="c"||val=="child"){
            for(int i=0; i<tr.childCount; i++) sh.io.PrintLn(tr.GetChild(i).name);
        }else if(val=="d"||val=="descendant"){
            var oi=tr.GetComponent<ObjInfo>();
            if(oi!=null) foreach(Transform t in oi.GetBones()) sh.io.PrintLn(t.name);
            else UTIL.TraverseTr(tr,(Transform t)=>{
                if(!string.IsNullOrEmpty(t.name)) sh.io.PrintLn(t.name);
                return 0;
            });
        }else return sh.io.Error("list種別に child か descendant を指定してください");
        return 0;
    }
    private static int ObjParamPrefix(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return 0;
        if(val=="")  return 1;
        var bd=tr.GetComponent<ObjInfo>();
        if(bd==null||bd.morph==null) return sh.io.Error("objコマンドで追加されたオブジェクト以外は変更できません");
        UTIL.TraverseTr(tr,(Transform t)=>{
            if(!string.IsNullOrEmpty(t.name)) ObjUtil.RenameTr(t,val+t.name);
            return 0; 
        },false);
        ObjInfo.UpdObjInfo(tr);
        return 1;
    }
    private static int ObjParamMaterial(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            Renderer r=tr.GetComponentInChildren<Renderer>();
            if (r!=null && r.sharedMaterial!=null) for(int i=0; i<r.sharedMaterials.Length; i++){
                sh.io.PrintLn($"{r.sharedMaterials[i].name}{sh.ofs}{r.sharedMaterials[i].shader.name}");
            }
            return 0;
        }
        string[] sa=val.Split(ParseUtil.colon);
        if(sa.Length!=2) return sh.io.Error("パラメータの書式が不正です");
        if(!int.TryParse(sa[0],out int idx)||idx<0) return sh.io.Error("マテリアル番号の指定が不正です");
        int ret=ObjUtil.ChgMaterial(tr,idx,sa[1]);
        if(ret==-1) return sh.io.Error("マテリアルの読み込みに失敗しました");
        if(ret==-2) return sh.io.Error("objコマンドで追加されたオブジェクト以外は変更できません");
        return 1;
    }
    private static int ObjParamShape(ComShInterpreter sh,Transform tr,string val){
        ObjInfo oi;
        if(val==null){
            oi=tr.GetComponent<ObjInfo>();
            if(oi==null||oi.morph==null) return 0;
            foreach (string mk in oi.morph.hash.Keys)
                sh.io.PrintLn2(mk+":",sh.fmt.F0to1(oi.morph.GetBlendValues((int)oi.morph.hash[mk])));
            return 0;
        }
        oi=tr.GetComponent<ObjInfo>();
        if(oi==null||oi.morph==null) return 1;
        var kvs=ParseUtil.GetKVFloat(val);
        if(kvs==null) return sh.io.Error(ParseUtil.error);
        bool dirty=false;
        foreach(string dk in kvs.Keys) if(oi.morph.hash.ContainsKey(dk)){
            oi.morph.SetBlendValues((int)oi.morph.hash[dk],kvs[dk]);
            dirty=true;
        }
        if(dirty) oi.morph.FixBlendValues();
        return 1;
    }
    private static int ObjParamName(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            sh.io.Print(tr.name);
            return 0;
        }
        ObjUtil.RenameTr(tr,val);
        ObjInfo.UpdObjInfo(tr); // (あれば)ボーン情報更新
        return 1;
    }

    private static int ObjParamMotion(ComShInterpreter sh,Transform tr,string val){
        Transform model=tr;
        Animation anm;
        var oi=tr.GetComponent<ObjInfo>();
        if(oi!=null && tr.childCount>0) model=tr.GetChild(0);
        anm=model.gameObject.GetComponent<Animation>();
        if(val==null){
            if(anm==null) return 0;
            foreach(AnimationState st in anm) sh.io.PrintLn(st.name);
            return 0;
        }
        if(anm==null) anm=model.gameObject.AddComponent<Animation>();
        if(val==""){ anm.Stop(); return 1;}
        string[] sa=val.Split(ParseUtil.comma);
        float speed=1;
        int loop=1;
        if(sa.Length>1 && !float.TryParse(sa[1],out speed)) return sh.io.Error("速度の指定が不正です");
        if(sa.Length>2 && !int.TryParse(sa[2],out loop) || loop<0 || loop>3)
            return sh.io.Error("ループの指定が不正です");

        // 登録済ならそれを実行
        if(anm.GetClip(sa[0])!=null){ anm.Play(sa[0]); return 1;}

        // なければ読み込んで実行
        int ret=ObjUtil.PlayMotion(anm,UTIL.Suffix(sa[0],".anm"),speed,loop);
        if(ret<0) return sh.io.Error("anmファイルが読み込めません");
        return 1;
    }
    private static int ObjParamHandle(ComShInterpreter sh,Transform tr,string val){
        if(val==null)
            return sh.io.Error("off|lpos|wpos||rot|wrot|scale|lposrot|wposrotのいずれかを指定してください");

        var hdl=ComShHandle.GetHandle(tr);
        var sw=val.ToLower();
        if(sw=="off"){
            if(hdl!=null) ComShHandle.DelHandle(hdl);
            return 1;
        }
        if(hdl==null){ hdl=ComShHandle.AddHandle(tr); hdl.Visible=true;}
        if(ComShHandle.SetHandleType(hdl,sw)<0)
            return sh.io.Error("off|lpos|wpos||rot|wrot|scale|lposrot|wposrotのいずれかを指定してください");
        return 1;
    }
    private static int ObjParamDesc(ComShInterpreter sh,Transform tr,string val){
        sh.io.PrintJoin(" ",
            "wpos", sh.fmt.FPos(tr.position),
            "wrot", sh.fmt.FEuler(tr.rotation.eulerAngles),
            "scale", sh.fmt.FMul(tr.localScale)
        );
        var mo= ObjUtil.GetCurrentMotion(tr);
        if(mo!="") sh.io.Print(" motion "+mo);
        return 0;
    }

    #pragma warning disable CS0618 // 型またはメンバーが旧型式です
    private struct OldParticle {
        public ParticleAnimator main;
        public ParticleEmitter emit;
        public ParticleRenderer render;
    }   
    private static int ObjParamOldParticle(ComShInterpreter sh,Transform tr,string val){
        var pa=tr.gameObject.GetComponentsInChildren<ParticleAnimator>();
        if(pa==null||pa.Length==0) return sh.io.Error("パーティクルが存在しません");
        OldParticle[] op=new OldParticle[pa.Length];
        for(int i=0; i<pa.Length; i++){
            op[i].main=pa[i];
            op[i].emit=pa[i].gameObject.GetComponent<ParticleEmitter>();
            op[i].render=pa[i].gameObject.GetComponent<ParticleRenderer>();
        }
        if(val==null){
            if(pa!=null) for(int i=0; i<pa.Length; i++) if(pa[i]!=null){
                string shname=op[i].render==null?"":op[i].render.sharedMaterial.shader.name;
                sh.io.PrintLn(pa[i].name+sh.ofs+shname);
            }
            return 0;
        }
        string[] kv=ParseUtil.LeftAndRight(val,'=');
        string[] np=ParseUtil.LeftAndRight(kv[0],'.');
        string n=np[0],p=np[1],v=kv[1];
        if(p==""){ p=n; n=""; }
        int ret;
        if(n!=""){
            bool found=false;
            for(int i=0; i<op.Length; i++) if(op[i].main.name==n){
                found=true;
                if((ret=OldParticleSub(sh,op[i],p,v))<0) return ret;
            }
            if(!found) return sh.io.Error("指定されたパーティクルは存在しません");
        }else{
            for(int i=0; i<op.Length; i++) if((ret=OldParticleSub(sh,op[i],p,v))<0) return ret;
        }
        return 1;
    }

    private static int OldParticleSub(ComShInterpreter sh,OldParticle op,string p,string v){
        float f; float[] fa;
        if(p.Length==0) return sh.io.Error("書式が不正です");
        if(p[0]=='_'){
            if(op.render==null || op.render.sharedMaterial==null) return 0;
            string err;
            if(v=="on"){
                op.render.sharedMaterial.EnableKeyword(p);
            }else if(v=="off"){
                op.render.sharedMaterial.DisableKeyword(p);
            }else if(v.IndexOf(',')>=0){
                if((err=SetColorProp(op.render.sharedMaterial,p,v))!="") return sh.io.Error(err);
            }else{
                if((err=SetFloatProp(op.render.sharedMaterial,p,v))!="") return sh.io.Error(err);
            }
        } else switch(p){
        case "shader":
            if(op.render==null) return 0;
            var shader=Shader.Find(v);
            if(shader==null) return sh.io.Error("指定されたシェーダは見つかりません");
            op.render.sharedMaterial.shader=shader;
            break;
        case "speedlimit":
            return sh.io.Error("パーティクルが旧形式のため設定できません");
        case "damping":
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            op.main.damping=f;
            break;
        case "grow":
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            op.main.sizeGrow=f;
            break;
        case "gravity":
            return sh.io.Error("パーティクルが旧形式のため設定できません");
        case "color":
            float[] rgba=ParseUtil.Rgb(v);
            if(rgba==null) return sh.io.Error(ParseUtil.error);
            Color c=new Color(rgba[0],rgba[1],rgba[2]);
            Color[] ca=op.main.colorAnimation;
            if(ca==null){
                ca=new Color[] {c,c,c,c,c};
                ca[3].a=ca[0].a/2; ca[4].a=ca[0].a/4;
            }else for(int i=0; i<ca.Length; i++){ ca[i].r=c.r; ca[i].g=c.g; ca[i].b=c.b; }
            op.main.colorAnimation=ca;
            op.main.doesAnimateColor=true;
            break;
        case "max":
            return sh.io.Error("パーティクルが旧形式のため設定できません");
        case "lifetime":
            if(op.emit==null) return 0;
            fa=ParseUtil.MinMax(v);
            if(fa==null||fa[0]<0||fa[1]<0) return sh.io.Error("数値の形式が不正です");
            op.emit.minEnergy=fa[0];
            op.emit.maxEnergy=fa[1];
            break;
        case "speed":
            if(op.emit==null) return 0;
            fa=ParseUtil.MinMax(v);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            {   Vector3 vec=op.emit.localVelocity;
                float l=vec.magnitude;
                if(!Mathf.Approximately(l,0)) op.emit.localVelocity=vec*(fa[0]/l);
                else{
                    vec=op.emit.worldVelocity;
                    l=vec.magnitude;
                    if(!Mathf.Approximately(l,0)) op.emit.worldVelocity=vec*(fa[0]/l);
                    else return 0; // localもworldも(0,0,0)
                }
                // rndVelocityが未使用なら最小値～最大値間の変動に使う
                // すでに使われていたらそのまま＝speedは最小値のまま変動しない
                Vector3 rv=op.emit.rndVelocity;
                if(Mathf.Approximately(Vector3.Cross(rv,vec).magnitude,0))
                    op.emit.rndVelocity=vec*((fa[1]-fa[0])/l);
            }
            break;
        case "size":
            if(op.emit==null) return 0;
            fa=ParseUtil.MinMax(v);
            if(fa==null||fa[0]<0||fa[1]<0) return sh.io.Error("数値の形式が不正です");
            op.emit.minSize=fa[0];
            op.emit.maxSize=fa[1];
            break;
        case "emit":
            if(op.emit==null) return 0;
            fa=ParseUtil.MinMax(v);
            if(fa==null||fa[0]<0||fa[1]<0) return sh.io.Error("数値の形式が不正です");
            op.emit.minEmission=fa[0];
            op.emit.maxEmission=fa[1];
            break;
        case "world":
            if(op.emit==null) return 0;
            int d=ParseUtil.ParseInt(v,-1);
            if(d==0) op.emit.useWorldSpace=false;
            else if(d==1) op.emit.useWorldSpace=true;
            else return sh.io.Error("値の範囲が不正です");
            break;
        case "loop":
            return sh.io.Error("パーティクルが旧形式のため設定できません");
        case "scale":
            return sh.io.Error("パーティクルが旧形式のため設定できません");
        case "duration":
            return sh.io.Error("パーティクルが旧形式のため設定できません");
        case "delay":
            return sh.io.Error("パーティクルが旧形式のため設定できません");
        case "simspeed":
            return sh.io.Error("パーティクルが旧形式のため設定できません");
        case "shape":
            return sh.io.Error("パーティクルが旧形式のため設定できません");
        default:
            return sh.io.Error("パラメータ名が不正です");
        }
        return 0;
    }
    #pragma warning restore CS0618

    private struct NewParticle {
        public ParticleSystem sys;
        public ParticleSystemRenderer render;
    }   
    private static int ObjParamParticle(ComShInterpreter sh,Transform tr,string val){
        var pa=tr.gameObject.GetComponentsInChildren<ParticleSystem>();
        if(pa==null||pa.Length==0) return ObjParamOldParticle(sh,tr,val);
        NewParticle[] par=new NewParticle[pa.Length];
        for(int i=0; i<pa.Length; i++){
            par[i].sys=pa[i];
            par[i].render=pa[i].gameObject.GetComponent<ParticleSystemRenderer>();
        }
        if(val==null){
            if(pa!=null) for(int i=0; i<pa.Length; i++) if(pa[i]!=null){
                var shname=par[i].render==null?"":par[i].render.sharedMaterial.shader.name;
                sh.io.PrintLn(pa[i].name+sh.ofs+shname);
            }
            return 0;
        }
        string[] kv=ParseUtil.LeftAndRight(val,'=');
        string[] np=ParseUtil.LeftAndRight(kv[0],'.');
        string n=np[0],p=np[1],v=kv[1];
        int ret;
        if(p==""){ p=n; n=""; }
        if(n!=""){  // 名前指定があれば一致するものだけ
            bool found=false;
            for(int i=0; i<par.Length; i++) if(n==par[i].sys.name){
                found=true;
                if((ret=ParticleSub(sh,par[i],p,v))<0) return ret;
            }
            if(!found) return sh.io.Error("指定されたパーティクルは存在しません");
        }else{      // 名前指定がなければ全部
            for(int i=0; i<par.Length; i++)
                if((ret=ParticleSub(sh,par[i],p,v))<0) return ret;
        }
        return 1;
    }
    private static int ParticleSub(ComShInterpreter sh,NewParticle par,string p,string v){
        var main=par.sys.main;
        var emit=par.sys.emission;
        var vot=par.sys.limitVelocityOverLifetime;
        var sot=par.sys.sizeOverLifetime;
        var shape=par.sys.shape;
        int d; float[] fa; float f;
        if(p.Length==0) return sh.io.Error("書式が不正です");
        if(p[0]=='_'){
            if(par.render==null || par.render.sharedMaterial==null) return 0;
            string err;
            if(v=="on"){
                par.render.sharedMaterial.EnableKeyword(p);
            }else if(v=="off"){
                par.render.sharedMaterial.DisableKeyword(p);
            }else if(v.IndexOf(',')>=0){
                if((err=SetColorProp(par.render.sharedMaterial,p,v))!="") return sh.io.Error(err);
            }else{
                if((err=SetFloatProp(par.render.sharedMaterial,p,v))!="") return sh.io.Error(err);
            }
        }else switch(p){
        case "shader":
            if(par.render==null) return 0;
            var shader=Shader.Find(v);
            if(shader==null) return sh.io.Error("指定されたシェーダは見つかりません");
            par.render.sharedMaterial.shader=shader;
            break;
        case "lifetime":
            fa=ParseUtil.MinMax(v);
            if(fa==null||fa[0]<0||fa[1]<0) return sh.io.Error("数値の形式が不正です");
            main.startLifetime=new ParticleSystem.MinMaxCurve(fa[0],fa[1]);
            break;
        case "speed":
            fa=ParseUtil.MinMax(v);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            main.startSpeed=new ParticleSystem.MinMaxCurve(fa[0],fa[1]);
            break;
        case "speedlimit":
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            vot.enabled=true;
            vot.limit=f;
            break;
        case "damping":
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            vot.enabled=true;
            vot.dampen=f;
            break;
        case "size":
            fa=ParseUtil.MinMax(v);
            if(fa==null||fa[0]<0||fa[1]<0) return sh.io.Error("数値の形式が不正です");
            main.startSize=new ParticleSystem.MinMaxCurve(fa[0],fa[1]);
            break;
        case "grow":
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            sot.enabled=true;
            sot.size=new ParticleSystem.MinMaxCurve(1,f);
            break;
        case "gravity":
            fa=ParseUtil.MinMax(v);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            main.gravityModifier=new ParticleSystem.MinMaxCurve(fa[0],fa[1]);
            break;
        case "color":
            float[] rgba=ParseUtil.Rgb(v);
            if(rgba==null) return sh.io.Error(ParseUtil.error);
            if(main.startColor.mode==ParticleSystemGradientMode.Color){
                ParticleSystem.MinMaxGradient sc=main.startColor;
                float a=sc.color.a;
                sc.color=new Color(rgba[0],rgba[1],rgba[2],a);
                main.startColor=sc;
            }else if(main.startColor.mode==ParticleSystemGradientMode.RandomColor
                    || main.startColor.mode==ParticleSystemGradientMode.TwoColors){
                ParticleSystem.MinMaxGradient sc=main.startColor;
                float amin=sc.colorMin.a; float amax=sc.colorMax.a;
                sc.colorMin=new Color(rgba[0],rgba[1],rgba[2],amin);
                sc.colorMax=new Color(rgba[0],rgba[1],rgba[2],amax);
                main.startColor=sc;
            }
            break;
        case "max":
            d=ParseUtil.ParseInt(v,-1);
            if(d<0) return sh.io.Error("値の範囲が不正です");
            main.maxParticles=d;
            break;
        case "emit":
            fa=ParseUtil.MinMax(v);
            if(fa==null||fa[0]<0||fa[1]<0) return sh.io.Error("数値の形式が不正です");
            emit.rateOverTime=new ParticleSystem.MinMaxCurve(fa[0],fa[1]);
            break;
        case "world":
            d=ParseUtil.ParseInt(v,-1);
            if(d==0) main.simulationSpace=ParticleSystemSimulationSpace.Local;
            else if(d==1) main.simulationSpace=ParticleSystemSimulationSpace.World;
            else return sh.io.Error("値の範囲が不正です");
            break;
        case "loop":
            d=ParseUtil.ParseInt(v,-1);
            if(d!=0&&d!=1) return sh.io.Error("値の範囲が不正です");
            main.loop=d==1;
            par.sys.Stop();
            par.sys.Play();
            break;
        case "scale":
            d=ParseUtil.ParseInt(v,-1);
            if(d!=0&&d!=1) return sh.io.Error("値の範囲が不正です");
            if(d==1) main.scalingMode=ParticleSystemScalingMode.Hierarchy;
            else main.scalingMode=ParticleSystemScalingMode.Local;
            break;
        case "duration":
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            par.sys.Stop();
            main.duration=f;
            par.sys.Play();
            break;
        case "delay":
            fa=ParseUtil.MinMax(v);
            if(fa==null||fa[0]<0||fa[1]<0) return sh.io.Error("数値の形式が不正です");
            main.startDelay=new ParticleSystem.MinMaxCurve(fa[0],fa[1]);
            break;
        case "simspeed":
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            main.simulationSpeed=f;
            break;
        case "shape":
            if(!shape.enabled) shape.enabled=true;
            var sa=v.Split(ParseUtil.comma);
            if(sa.Length==1){
                if(sa[0]=="box") shape.shapeType=ParticleSystemShapeType.Box;
                else if(sa[0]=="boxshell") shape.shapeType=ParticleSystemShapeType.BoxShell;
                else return sh.io.Error("shapeの指定が不正です");
            }else if(sa.Length==2){
                if(sa[0]=="hemisphere") shape.shapeType=ParticleSystemShapeType.Hemisphere;
                else if(sa[0]=="hemisphereshell") shape.shapeType=ParticleSystemShapeType.HemisphereShell;
                else if(sa[0]=="sphere") shape.shapeType=ParticleSystemShapeType.Sphere;
                else if(sa[0]=="sphereshell") shape.shapeType=ParticleSystemShapeType.SphereShell;
                else return sh.io.Error("shapeの指定が不正です");
                if(!float.TryParse(sa[1],out f)||f<0) return sh.io.Error("数値の指定が不正です");
                shape.radius=f;
                shape.arc=Mathf.PI*2;
            }else if(sa.Length==3){
                if(sa[0]=="cone"){
                    if(!float.TryParse(sa[1],out f)||f<0) return sh.io.Error("数値の指定が不正です");
                    shape.radius=f;
                    shape.arc=Mathf.PI*2;
                    shape.shapeType=ParticleSystemShapeType.Cone;
                    if(!float.TryParse(sa[2],out f)||f<0||f>90) return sh.io.Error("数値の指定が不正です");
                    shape.angle=f;
                }else return sh.io.Error("shapeの指定が不正です");
            }else return sh.io.Error("shapeの指定が不正です");
            break;
        default:
            return sh.io.Error("パラメータ名が不正です");
        }
        return 1;
    }
    private static int ObjParamType(ComShInterpreter sh,Transform tr,string val){
        var oi=ObjInfo.GetObjInfo(tr);
        if(oi!=null) sh.io.PrintLn(oi.source);
        return 0;
    }
    private static int ObjParamComponent(ComShInterpreter sh,Transform tr,string val){
        Component[] ca=tr.GetComponentsInChildren<Component>();
        for(int i=0; i<ca.Length; i++){
            if(ReferenceEquals(ca[i].GetType(),typeof(Transform))) continue;
            sh.io.PrintLn(ca[i].GetType().FullName+sh.ofs+ca[i].transform.name);
        }
        return 0;
    }
    private static int ObjParamMesh(ComShInterpreter sh,Transform tr,string val){
        Renderer r=null;
        Mesh mesh;
        MeshFilter mf=null;
        var oi=ObjInfo.GetObjInfo(tr);
        r=(oi!=null)?oi.FindComponent<Renderer>():tr.GetComponentInChildren<Renderer>();
        if(r==null) return sh.io.Error("レンダラが見つかりません");

        Transform t=r.transform;
        bool skinned=(ReferenceEquals(r.GetType(),typeof(SkinnedMeshRenderer)));
        if(skinned){
            mesh=((SkinnedMeshRenderer)r).sharedMesh;
        }else{
            mf=t.GetComponentInChildren<MeshFilter>();
            if(mf==null) return sh.io.Error("メッシュが見つかりません");
            mesh=mf.mesh;
        }
        Material[] ma=r.sharedMaterials;

        if(val==null){
            for(int i=0; i<mesh.subMeshCount; i++){
                sh.io.Print($"{i}{sh.ofs}count={mesh.GetTriangles(i).Length/3}");
                if(ma.Length>i) sh.io.Print($"{sh.ofs}mate={ma[i].name}{sh.ofs}shader={ma[i].shader.name}");
                sh.io.PrintLn("");
            }
            return 0;
        }
        string[] sa=ParseUtil.LeftAndRight(val,':');
        if(!int.TryParse(sa[0],out int n)||n<0||n>=mesh.subMeshCount) return sh.io.Error("サブメッシュ番号が不正です");
        string[] kv=ParseUtil.LeftAndRight(sa[1],'=');
        if(kv[0]=="" || kv[1]=="") return sh.io.Error("書式が不正です");
        if(kv[0][0]=='_'){
            string err;
            if(kv[1]=="on"){
                ma[n].EnableKeyword(kv[0]);
            }else if(kv[1]=="off"){
                ma[n].DisableKeyword(kv[0]);
            }else if(kv[1].IndexOf(',')>=0){
                if((err=SetColorProp(ma[n],kv[0],kv[1]))!="") return sh.io.Error(err);
            }else{
                if((err=SetFloatProp(ma[n],kv[0],kv[1]))!="") return sh.io.Error(err);
            }
        } else switch(kv[0]){
        case "color":
            float[] fa=ParseUtil.Rgba(kv[1]);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            ma[n].color=new Color(fa[0],fa[1],fa[2],fa[3]);
            break;
        case "shader":
            Shader shader=Shader.Find(kv[1]);
            if(shader==null) return sh.io.Error("指定されたシェーダは見つかりません");
            ma[n].shader=shader;
            break;
        case "blend":
            if(ChgBlendMode(ma[n],kv[1])<0) return sh.io.Error("blendにはopaque|cutout|fade|transparentのいずれかを指定して下さい");
            break;
        case "topology":
            if(oi==null) oi=ObjInfo.AddObjInfo(tr.gameObject,"");
            if(oi.mesh==null){
                oi.mesh=mesh;
                mesh=CloneMesh(mesh);
                if(skinned) ((SkinnedMeshRenderer)r).sharedMesh=mesh;
                else mf.sharedMesh=mesh;
            }
            if(kv[1]=="1"){
                int[] ia=oi.mesh.GetIndices(n);
                var hs=new HashSet<int>();
                for(int i=0; i<ia.Length; i++) hs.Add(ia[i]);
                int[] ia2=new int[hs.Count];
                hs.CopyTo(ia2);
                mesh.SetIndices(ia2,MeshTopology.Points,n);
            }else if(kv[1]=="2"){
                int[] ia=oi.mesh.GetIndices(n);
                var hs=new HashSet<uint>();
                for(int i=0; i<ia.Length; i+=3){
                    hs.Add((ia[i]<ia[i+1])?(uint)(ia[i]*65536+ia[i+1]):(uint)(ia[i+1]*65536+ia[i]));
                    hs.Add((ia[i+1]<ia[i+2])?(uint)(ia[i+1]*65536+ia[i+2]):(uint)(ia[i+2]*65536+ia[i+1]));
                    hs.Add((ia[i+2]<ia[i])?(uint)(ia[i+2]*65536+ia[i]):(uint)(ia[i]*65536+ia[i+2]));
                }
                int[] ia2=new int[hs.Count*2];
                int cnt=0;
                foreach(var u in hs){ ia2[cnt++]=(int)(u>>16); ia2[cnt++]=(int)(u&0xffff); }
                mesh.SetIndices(ia2,MeshTopology.Lines,n);
            }else if(kv[1]=="3"){
                // mesh.SetIndices(oi.mesh.GetIndices(n),MeshTopology.Triangles,n);
                if(skinned) ((SkinnedMeshRenderer)r).sharedMesh=oi.mesh;
                else mf.sharedMesh=oi.mesh;
            }
            break;
        }
        return 1;
    }
    private static int ChgBlendMode(Material material,string mode){
        switch (mode) {
        case "opaque":
            material.SetOverrideTag("RenderType", "");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
            break;
        case "cutout":
            material.SetOverrideTag("RenderType", "TransparentCutout");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.EnableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            break;
        case "fade":
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            break;
        case "transparent":
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            break;
        default:
            return -1;
        }
        return 0;
    }
    private static Mesh CloneMesh(Mesh m){
        Mesh ret=new Mesh();
        ret.vertices=m.vertices;
        ret.normals=m.normals;
        ret.tangents=m.tangents;
        ret.triangles=m.triangles;
        ret.uv=m.uv; ret.uv2=m.uv2; ret.uv3=m.uv3; ret.uv4=m.uv4;
        ret.bindposes=m.bindposes;
        ret.boneWeights=m.boneWeights;
        ret.subMeshCount=m.subMeshCount;
        ret.bounds=m.bounds;
        ret.colors=m.colors;
        ret.colors32=m.colors32;
        return ret;
    }
    private static string SetColorProp(Material m, string key, string val){
        if(!m.HasProperty(key)) return "指定されたプロパティは現在のシェーダでは無効です";
        float[] fa=ParseUtil.RgbaLenient(val);
        if(fa==null) return ParseUtil.error;
        m.SetColor(key,new Color(fa[0],fa[1],fa[2],fa[3]));
        return "";
    }
    private static string SetFloatProp(Material m, string key, string val){
        if(!m.HasProperty(key)) return "指定されたプロパティは現在のシェーダでは無効です";
        float f=ParseUtil.ParseFloat(val);
        if(float.IsNaN(f)) return ParseUtil.error;
        m.SetFloat(key,f);
        return "";
    }
}
public static class ObjUtil {
    public static Dictionary<string,Transform> objDic=new Dictionary<string,Transform>();
    public static Transform FindObj(ComShInterpreter sh,string name){
        if(name=="") return null;
        Transform tr=null;

        string[] sa=ParseUtil.LeftAndRight(name,'/');
        if(objDic.ContainsKey(sa[0])){  // objコマンドで作ったオブジェクト
            tr=objDic[sa[0]];
            if(tr==null){ objDic.Remove(sa[0]); return null; }
        }
        if(tr!=null) return (sa[1]=="")?tr:tr.Find(sa[1]);

        if(sh==null) return null;
        tr=GetPhotoPrefabTr(sh);
        if(tr==null) return null;
        tr=tr.Find(name);
        if(tr!=null) return tr;
        if(sh.objRef!=""){
            tr=UTIL.GetObjRoot(sh.objRef);
            if(tr==null) return null;
            return tr.Find(name);
        }
        return null;
    }
    public static Transform FindObj(ComShInterpreter sh,string[] sa){
        if(sa.Length==0) return null;
        Transform tr=null;

        if(sa.Length==1) return FindObj(sh,sa[0]);
        if(sa.Length==2&&sa[0]=="obj") return FindObj(sh,sa[1]);

        string[] lr=ParseUtil.LeftAndRight(sa[sa.Length-1],'/');
        if(sa.Length==3){
            tr=BoneUtil.FindBone(sh,sa[0],sa[1],(lr[0]=="")?"/":lr[0]);
        }else if(sa.Length==2){
            Maid m=MaidUtil.FindMaidMan(sa[0],lr[0]);
            if(m==null) return null;
            tr=m.body0.m_trBones; // "maid:0:/"と同じ
        }
        if(tr==null) return null;
        if(lr[1]!="") return tr.Find(lr[1]);
        return tr;
    }

    public static void RenameTr(Transform tr, string name){
        if(ObjUtil.objDic.TryGetValue(tr.name,out Transform t) && ReferenceEquals(tr,t)){
            ObjUtil.objDic.Remove(tr.name);
            ObjUtil.objDic.Add(name,tr);
        }
        tr.name=name;
    }
    public static Transform GetPhotoPrefabTr(ComShInterpreter sh,bool create=false){
        if(sh.objBase==string.Empty){
            GameObject bg=GameMain.Instance.BgMgr.BgObject;
            if(bg!=null) return bg.transform;
            return null;
        }else return UTIL.GetObjRoot(sh.objBase,create);
    }

    private class FakeMaid:Maid{ private void OnDestroy(){} }
    private static FakeMaid dummyMaid;
    public static GameObject AddObject(string src, string name, Transform pr, Vector3 pos, Vector3 rot,Vector3 scl){
        GameObject o;
        bool instanceq=true;
        if(src=="."){ o=new GameObject(); instanceq=false; }
        else o=Resources.Load<GameObject>("Prefab/"+src);
        if(o==null) o=Resources.Load<GameObject>("SceneCreativeRoom/Debug/Prefab/"+src);
        if(o==null){
            string abg=src.EndsWith(".asset_bg",Ordinal)?src.Substring(0,src.Length-9):src;
            o=GameMain.Instance.BgMgr.CreateAssetBundle(abg);
        }
        if(o==null) o=Resources.Load<GameObject>("BG/"+src);
        if(o==null) o=Resources.Load<GameObject>("BG/2_0/"+src);
        TMorph morph=null;
        MenuObj mo=null;
        if(o==null){
            if(dummyMaid==null){ // LoadSkinMesh_Rを呼ぶためだけのニセMaid
                dummyMaid=new GameObject().AddComponent<FakeMaid>();
                dummyMaid.m_goOffset=new GameObject("Offset");
                dummyMaid.body0=dummyMaid.gameObject.AddComponent<TBody>();
                dummyMaid.body0.maid=dummyMaid;
                dummyMaid.body0.m_hitFloorPlane=null;
                dummyMaid.body0.boMAN=dummyMaid.boMAN=false;
                dummyMaid.body0.goSlot=new List<TBodySkin>();
                dummyMaid.body0.goSlot.Add(new TBodySkin(dummyMaid.body0,"body",0,false));
            }
            string fname=Path.GetFileNameWithoutExtension(src);
            string ext=src.Substring(fname.Length);
            if(ext!=".model"){
                string menuname=fname+".menu";
                if(GameUty.IsExistFile(menuname)){
                    mo=ReadMenuObj(menuname);
                    if(mo==null) return null;
                }
            }
            if(mo==null) mo=new MenuObj(){modelName=fname+".model"};
            if(!string.IsNullOrEmpty(mo.modelName) && GameUty.IsExistFile(mo.modelName)){
                TBodySkin tbs=dummyMaid.body0.goSlot[0];
                var m=new TMorph(tbs);
                o=ImportCM.LoadSkinMesh_R(mo.modelName,m,"body",tbs,0);
                instanceq=false; // これは中で既にインスタンス化されてる
                if(m!=null && m.MorphCount>0){
                    m.InitGameObject(o);
                    morph=m;
                }
            }
        }
        if(o==null) return null;
        GameObject b=new GameObject(name);
        b.transform.SetParent(pr);
        GameObject go=o;
        if(instanceq) go=UnityEngine.Object.Instantiate(o,b.transform); else go.transform.SetParent(b.transform);
        int idx=go.name.IndexOf("(Clone)",Ordinal);
        if(idx>=0) go.name=go.name.Substring(0,idx);
        b.transform.localPosition=pos;
        b.transform.localRotation=Quaternion.Euler(rot);
        b.transform.localScale=scl;
        var oi=ObjInfo.AddObjInfo(b,src,morph);

        if(mo!=null){
            if(mo.material!=null)
                foreach(var mate in mo.material) ChgMaterial(b.transform,mate.no,mate.name);
            if(mo.anmFile!=null){
                var a=go.GetComponent<Animation>();
                if(a==null) a=go.AddComponent<Animation>();
                PlayMotion(a,mo.anmFile,1.0f,mo.anmLoop?1:0);
            }
        }
        return b;
    }
    public static GameObject CloneObject(string name,Transform obase,Transform pr){
        Transform orig=obase;
        ObjInfo oi=orig.GetComponent<ObjInfo>();
        TMorph morph=null;
        if(oi!=null){ morph=oi.morph; if(orig.childCount==1) orig=orig.GetChild(0); }
        if(orig==null) return null;
        Vector3 opos=orig.position;
        Quaternion orot=orig.rotation;
        Vector3 oscl=orig.lossyScale;
        GameObject b=new GameObject(name);  // 土台
        GameObject go=UnityEngine.Object.Instantiate(orig.gameObject,b.transform);
        go.name=go.name.Replace("(Clone)","");
        b.transform.position=opos;
        b.transform.localRotation=Quaternion.identity;
        b.transform.localScale=Vector3.one;
        go.transform.localPosition=Vector3.zero;
        go.transform.rotation=orot;
        go.transform.localScale=oscl;
        ObjInfo.AddObjInfo(b.transform,oi==null?"":oi.source,morph);
        b.transform.SetParent(pr);
        return b;
    }
    public delegate int GoMa(GameObject go,Maid m);
    public static int LsAttach<T>(GoMa f) where T:Component {
        CharacterMgr cm=GameMain.Instance.CharacterMgr;
        Maid m;
        for (int i=0; i<cm.GetMaidCount(); i++) {
            if ((m=cm.GetMaid(i))==null) continue;
            T[] ca=m.body0.m_trBones.GetComponentsInChildren<T>(true);
            if(ca!=null) for(int j=0; j<ca.Length; j++)
                if(f.Invoke(ca[j].gameObject,m)<0) return -1;
        }
        for (int i=0; i<cm.GetManCount(); i++) {
            if ((m=cm.GetMan(i))==null) continue;
            if(m.body0.m_trBones==null) continue;
            T[] ca=m.body0.m_trBones.GetComponentsInChildren<T>(true);
            if(ca!=null) for(int j=0; j<ca.Length; j++)
                if(f.Invoke(ca[j].gameObject,m)<0) return -1;
        }
        return 0;
    }
    public class MenuObj {
        public class Material{
            public int no;
            public string name;
        }
        public string modelName;
        public string anmFile;
        public bool anmLoop=false;
        public List<Material> material;
        public void AddMaterial(int no,string name){
            if(material==null) material=new List<Material>();
            var mate=new Material();
            mate.no=no; mate.name=name;
            material.Add(mate);
        }
    }
    private static char[] menuWhite={' ','\t','\u3000'};
    public static MenuObj ReadMenuObj(string fname){
        byte[] buf=UTIL.AReadAll(fname);
        if(buf==null) return null;
        return ReadMenuObj(buf);
    }
    public static MenuObj ReadMenuObj(byte[] buf){
        MenuObj ret=new MenuObj();
        try{
            using(BinaryReader r=new BinaryReader(new MemoryStream(buf),Encoding.UTF8)){
                string header=r.ReadString();
                if(header!="CM3D2_MENU") return null;
                _=r.ReadInt32(); // フォーマットver
                _=r.ReadString(); // *.txtなやつ
                _=r.ReadString(); // name
                _=r.ReadString(); // category
                _=r.ReadString(); // setumei
                _=r.ReadInt32();    // 謎数値
                while(r.PeekChar()>=0){
                    // labels
                    int n=r.ReadByte();
                    if(n==0) break;
                    var args=new List<string>();
                    for(;n>0; n--) args.Add(r.ReadString().Trim(menuWhite));
                    if(args.Count==0) break;
                    string cmd=args[0].ToLower();
                    if(cmd=="end") return ret;
                    else if(cmd=="additem"){
                        if(args.Count>=2) ret.modelName=UTIL.Suffix(args[1],".model");
                    }else if(cmd=="マテリアル変更"){
                        if(args.Count>=4) ret.AddMaterial(int.Parse(args[2]),UTIL.Suffix(args[3],".mate"));
                    }else if(cmd=="anime"){
                        if(args.Count>=3) ret.anmFile=UTIL.Suffix(args[2],".anm");
                        if(args.Count>=4 && args[3]=="loop") ret.anmLoop=true;
                    }
                }
            }
        }catch{ return string.IsNullOrEmpty(ret.modelName)?null:ret; }
        return ret;
    }
    public static int ChgMaterial(Transform tr,int no,string fname){
        string mate=UTIL.Suffix(fname,".mate");
        var oi=tr.GetComponent<ObjInfo>();
        if(oi==null) return -2;
        foreach(var r in oi.FindComponents<Renderer>())
            if (r.sharedMaterials!=null && no<r.sharedMaterials.Length)
                try{ ImportCM.LoadMaterial(mate,null,r.sharedMaterials[no]);}catch{ return -1; }
        return 0;
    }
    private static int ChkBoneGender(Transform t){
        for(int i=0; i<t.childCount; i++){
            var c=t.GetChild(i);
            if(c.name.StartsWith("Bip01",Ordinal)) return 0; // 衣装系はこれ
            else if(c.name.StartsWith("ManBip",Ordinal)) return 1; // ないと思うけど
        }
        return -1; // 人用ボーンのない普通のオブジェクト
    }
    public static int PlayMotion(Animation a,string file,float speed,int loop){
        string myposefile=ComShInterpreter.myposeDir+file;
        byte[] arr;
        if(File.Exists(myposefile)) arr=UTIL.ReadAll(myposefile);
        else arr=UTIL.AReadAll(file);
        if(arr==null) return -1;

        try{
            var af=new AnmFile(arr);
            int gender=ChkBoneGender(a.gameObject.transform);
            if(gender==0 && af.gender==1) arr=af.ChgGender();
        }catch{ return -1; }
        if(arr==null) return -1;

        string tag=file.ToLower();
        var clip=ImportCM.LoadAniClipNative(arr, true, true, true);
        if(clip==null) return -1;
        clip.legacy=true;
        clip.name=tag;

        // お掃除
        var remove=new List<AnimationClip>();
        foreach(AnimationState state in a){
            // ネイやネイロボはrunとかattackとかいうclipを持ってる。
            // それらを残すため、".anm"のないものは消さないようにする
            if(state.name.EndsWith(".anm",Ordinal)) remove.Add(state.clip);
        }
        foreach(var rc in remove) a.RemoveClip(rc);

        a.AddClip(clip,tag);
        var st= a[tag];
        st.speed=speed;
        if(loop==0) st.wrapMode=WrapMode.Once;
        else if(loop==1) st.wrapMode=WrapMode.Loop;
        else if(loop==2) st.wrapMode=WrapMode.PingPong;
        else if(loop==3) st.wrapMode=WrapMode.ClampForever;
        a.Stop();
        a.Play(tag);
        return 0;
    }
    public static string GetCurrentMotion(Transform tr){
        var anm=tr.gameObject.GetComponent<Animation>();
        if(anm!=null) foreach(AnimationState ast in anm) if(anm.IsPlaying(ast.name)) return ast.name;
        return "";
    }
}
}
