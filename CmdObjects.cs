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
        objParamDic.Add("matecp",new CmdParam<Transform>(ObjParamMaterialCopy));
        objParamDic.Add("shape",new CmdParam<Transform>(ObjParamShape));
        objParamDic.Add("prefix",new CmdParam<Transform>(ObjParamPrefix));
        objParamDic.Add("prot",new CmdParam<Transform>(_CmdParamPRot));
        objParamDic.Add("pquat",new CmdParam<Transform>(_CmdParamPQuat));
        objParamDic.Add("name",new CmdParam<Transform>(ObjParamName));
        objParamDic.Add("layer",new CmdParam<Transform>(ObjParamLayer));
        objParamDic.Add("motion",new CmdParam<Transform>(ObjParamMotion));
        objParamDic.Add("particle",new CmdParam<Transform>(ObjParamParticle));
        objParamDic.Add("handle",new CmdParam<Transform>(ObjParamHandle));
        objParamDic.Add("describe",new CmdParam<Transform>(ObjParamDesc));
        objParamDic.Add("type",new CmdParam<Transform>(ObjParamType));
        objParamDic.Add("mesh",new CmdParam<Transform>(ObjParamMesh));
        objParamDic.Add("component",new CmdParam<Transform>(ObjParamComponent));
        objParamDic.Add("lookat",new CmdParam<Transform>(ObjParamLookAt));
        objParamDic.Add("locik",new CmdParam<Transform>(ObjParamLocIK));
        objParamDic.Add("locik-",new CmdParam<Transform>(ObjParamLocIKMinus));
        
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
            var lst=GetObjList(sh);
            foreach(var tr in lst) sh.io.PrintJoinLn(sh.ofs,tr.name,sh.fmt.FPos(tr.position));
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
    public static List<Transform> GetObjList(ComShInterpreter sh){
        var ret=new List<Transform>();
        var oset=new HashSet<string>(); // 重複削除
        Transform pftr;
        if(sh.objRef.Length>0){ // スタジオモード分参照
            pftr= UTIL.GetObjRoot(sh.objRef);
            if(pftr!=null) for(int i=0; i<pftr.childCount; i++){
                Transform tr=pftr.GetChild(i);
                if(tr==null) continue;
                oset.Add(tr.name);
                ret.Add(tr);
            }
        }
        // 現root直下のobject
        if((pftr=ObjUtil.GetPhotoPrefabTr(sh))!=null){
            for(int i=0; i<pftr.childCount; i++){
                Transform tr=pftr.GetChild(i);
                if(tr==null) continue;
                oset.Add(tr.name);
                ret.Add(tr);
            }
        }
        List<string> remove=new List<string>();
        foreach(var kv in ObjUtil.objDic){
            Transform tr=kv.Value;
            if(tr==null){ remove.Add(kv.Key); continue; }
            if(oset.Contains(tr.name)) continue; // root直下のものは取得済
            // 何かにアタッチされているもの
            ret.Add(tr);
        }
        foreach(string k in remove) ObjUtil.objDic.Remove(k);
        return ret;
    }
    private static string AutoObjName(string name){
        string seq=UTIL.GetSeqId();
        string ret=string.Concat(System.IO.Path.GetFileNameWithoutExtension(name).Replace(' ','_'),"_",seq);
        if(!UTIL.ValidName(ret)) ret="object_"+seq;
        return ret;
    }
    public static int CmdObjectSub(ComShInterpreter sh,string[] sa,List<string> args,int startpos){
        Transform tr=ObjUtil.FindObj(sh,sa);
        if(tr==null) return sh.io.Error("オブジェクトが存在しません");
        if(args.Count==startpos){
            sh.io.PrintLn2("iid:",tr.gameObject.GetInstanceID().ToString());
            UTIL.PrintTrInfo(sh,tr);
            return 0;
        }
        return ParamLoop(sh,tr,objParamDic,args,startpos);
    }
    public static int CmdObjectSub(ComShInterpreter sh,ParseUtil.ColonDesc cd,List<string> args,int startpos){
        Transform tr=ObjUtil.FindObj(sh,cd);
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
        if(val==null) return sh.io.Error("list種別に child か descendant か tree を指定してください");
        if(val=="c"||val=="child"){
            for(int i=0; i<tr.childCount; i++) sh.io.PrintLn(tr.GetChild(i).name);
        }else if(val=="d"||val=="descendant"){
            var oi=tr.GetComponent<ObjInfo>();
            if(oi!=null) foreach(Transform t in oi.data.bones) sh.io.PrintLn(t.name);
            else UTIL.TraverseTr(tr,(Transform t,int d)=>{
                if(!string.IsNullOrEmpty(t.name)) sh.io.PrintLn(t.name);
                return 0;
            });
        }else if(val=="t"||val=="tree"){
            UTIL.TraverseTr(tr,(Transform t,int d)=>{
                if(!string.IsNullOrEmpty(t.name)){
                    for(int i=0; i<d; i++) sh.io.Print("  ");
                    sh.io.PrintLn(t.name);
                }
                return 0;
            });
        }else return sh.io.Error("list種別に child か descendant か tree を指定してください");
        return 0;
    }
    private static int ObjParamPrefix(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return 0;
        if(val=="")  return 1;
        var bd=tr.GetComponent<ObjInfo>();
        if(bd==null||bd.data.morph==null) return sh.io.Error("objコマンドで追加されたオブジェクト以外は変更できません");
        UTIL.TraverseTr(tr,(Transform t,int d)=>{
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
    private static int ObjParamMaterialCopy(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return sh.io.Error("コピー元を指定してください");

        string[] sa=ParseUtil.LeftAndRight(val,':');
        if(sa[1]=="") return sh.io.Error("パラメータの書式が不正です");
        if(!int.TryParse(sa[0],out int idx)||idx<0) return sh.io.Error("マテリアル番号の指定が不正です");

        string[] sa2=ParseUtil.LeftAndRight(sa[1],'#');
        if(sa2[1]=="") return sh.io.Error("パラメータの書式が不正です");
        if(!int.TryParse(sa2[1],out int fromidx)||fromidx<0) return sh.io.Error("マテリアル番号の指定が不正です");

        Transform fromtr=ObjUtil.FindObj(sh,sa2[0].Split(':'));
        if(fromtr==null) return sh.io.Error("オブジェクトが見つかりません");

        Renderer fromrdr=fromtr.GetComponentInChildren<Renderer>();
        if (fromrdr==null || fromrdr.sharedMaterial==null || fromidx>=fromrdr.sharedMaterials.Length)
            return sh.io.Error("マテリアルが見つかりません");
 
        var mi=new CmdMeshes.MeshInfo(tr);
        if(mi==null) return sh.io.Error("マテリアルが見つかりません");
        if(mi.material.Count<=idx) return sh.io.Error("マテリアル番号の指定が不正です");
        mi.EditMaterial();
        var ma=fromrdr.sharedMaterials;
        mi.material[idx].shader=ma[fromidx].shader;
        mi.material[idx].CopyPropertiesFromMaterial(ma[fromidx]);
        return 1;
    }
    private static int ObjParamShape(ComShInterpreter sh,Transform tr,string val){
        ObjInfo oi;
        if(val==null){
            oi=tr.GetComponent<ObjInfo>();
            if(oi==null||oi.data.morph==null) return 0;
            var sd=new SortedDictionary<string,float>();    // セットメニューで読んだブツには重複もある
            foreach(TMorph m in oi.data.morph) foreach (string mk in m.hash.Keys)
                sd[mk]=m.GetBlendValues((int)m.hash[mk]);
            foreach(var kv in sd) sh.io.PrintLn($"{kv.Key}:{sh.fmt.F0to1(kv.Value)}");
            return 0;
        }
        oi=tr.GetComponent<ObjInfo>();
        if(oi==null||oi.data.morph==null) return 1;
        var kvs=ParseUtil.GetKVFloat(val);
        if(kvs==null) return sh.io.Error(ParseUtil.error);
        foreach(TMorph m in oi.data.morph){
            bool dirty=false;
            foreach(string dk in kvs.Keys) if(m.hash.ContainsKey(dk)){
                m.SetBlendValues((int)m.hash[dk],kvs[dk]);
                dirty=true;
            }
            if(dirty) m.FixBlendValues();
        }
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
    private static int ObjParamLayer(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            sh.io.Print(tr.gameObject.layer.ToString());
            return 0;
        }
        if(!int.TryParse(val,out int n)||n<0||n>31) return sh.io.Error("レイヤ番号の指定が不正です");
        tr.gameObject.layer=n;
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
        public ParticleAnimator anim;
        public ParticleEmitter emit;
        public ParticleRenderer render;
        public ParticleInfo pi;
        public Material EditMaterial(){
            if(pi==null) pi=emit.gameObject.AddComponent<ParticleInfo>();
            return render.material;
        }
    }   
    private static int ObjParamOldParticle(ComShInterpreter sh,Transform tr,string val){
        var emit=tr.gameObject.GetComponentsInChildren<ParticleEmitter>();
        if(emit==null||emit.Length==0) return sh.io.Error("パーティクルが存在しません");
        OldParticle[] op=new OldParticle[emit.Length];
        for(int i=0; i<emit.Length; i++){
            op[i].anim=emit[i].gameObject.GetComponent<ParticleAnimator>();
            op[i].emit=emit[i];
            op[i].render=emit[i].gameObject.GetComponent<ParticleRenderer>();
            op[i].pi=emit[i].gameObject.GetComponent<ParticleInfo>();
        }
        if(val==null){
            if(emit!=null) for(int i=0; i<emit.Length; i++) if(emit[i]!=null){
                string shname=op[i].render==null?"":op[i].render.sharedMaterial.shader.name;
                sh.io.PrintLn(emit[i].name+sh.ofs+shname);
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
            for(int i=0; i<op.Length; i++) if(op[i].emit.name==n){
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
        float f; float[] fa; Material mate;
        if(p.Length==0) return sh.io.Error("書式が不正です");
        if(p[0]=='_'){
            if(op.render==null || op.render.sharedMaterial==null) return 0;
            string err;
            if(v=="on"){
                mate=op.EditMaterial();
                mate.EnableKeyword(p);
            }else if(v=="off"){
                mate=op.EditMaterial();
                mate.DisableKeyword(p);
            }else if(v.IndexOf(',')>=0){
                mate=op.EditMaterial();
                if((err=CmdMeshes.SetColorProp(mate,p,v))!="") return sh.io.Error(err);
            }else{
                mate=op.EditMaterial();
                if((err=CmdMeshes.SetFloatProp(mate,p,v))!="") return sh.io.Error(err);
            }
        } else switch(p){
        case "shader":
            if(op.render==null) return 0;
            var shader=Shader.Find(v);
            if(shader==null) return sh.io.Error("指定されたシェーダは見つかりません");
            mate=op.EditMaterial();
            mate.shader=shader;
            break;
        case "speedlimit":
            return sh.io.Error("パーティクルが旧形式のため設定できません");
        case "damping":
            if(op.anim==null) return 0;
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            op.anim.damping=f;
            break;
        case "grow":
            if(op.anim==null) return 0;
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            op.anim.sizeGrow=f;
            break;
        case "gravity":
            return sh.io.Error("パーティクルが旧形式のため設定できません");
        case "color":
            if(op.anim==null) return 0;
            float[] rgba=ParseUtil.Rgb(v);
            if(rgba==null) return sh.io.Error(ParseUtil.error);
            Color c=new Color(rgba[0],rgba[1],rgba[2]);
            Color[] ca=op.anim.colorAnimation;
            if(ca==null){
                ca=new Color[] {c,c,c,c,c};
                ca[3].a=ca[0].a/2; ca[4].a=ca[0].a/4;
            }else for(int i=0; i<ca.Length; i++){ ca[i].r=c.r; ca[i].g=c.g; ca[i].b=c.b; }
            op.anim.colorAnimation=ca;
            op.anim.doesAnimateColor=true;
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
            if(op.emit==null) return 0;
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            op.emit.maxEnergy=f;
            break;
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
        public ParticleInfo pi;
        public Material EditMaterial(){
            if(pi==null) sys.gameObject.AddComponent<ParticleInfo>();
            return render.material;
        }
    }   
    private static int ObjParamParticle(ComShInterpreter sh,Transform tr,string val){
        var pa=tr.gameObject.GetComponentsInChildren<ParticleSystem>();
        if(pa==null||pa.Length==0) return ObjParamOldParticle(sh,tr,val);
        NewParticle[] par=new NewParticle[pa.Length];
        for(int i=0; i<pa.Length; i++){
            par[i].sys=pa[i];
            par[i].render=pa[i].gameObject.GetComponent<ParticleSystemRenderer>();
            par[i].pi=pa[i].gameObject.GetComponent<ParticleInfo>();
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
        int d; float[] fa; float f; Material mate;
        if(p.Length==0) return sh.io.Error("書式が不正です");
        if(p[0]=='_'){
            if(par.render==null || par.render.sharedMaterial==null) return 0;
            string err;
            if(v=="on"){
                mate=par.EditMaterial();
                mate.EnableKeyword(p);
            }else if(v=="off"){
                mate=par.EditMaterial();
                mate.DisableKeyword(p);
            }else if(v.IndexOf(',')>=0){
                mate=par.EditMaterial();
                if((err=CmdMeshes.SetColorProp(mate,p,v))!="") return sh.io.Error(err);
            }else{
                mate=par.EditMaterial();
                if((err=CmdMeshes.SetFloatProp(mate,p,v))!="") return sh.io.Error(err);
            }
        }else switch(p){
        case "shader":
            if(par.render==null) return 0;
            var shader=Shader.Find(v);
            if(shader==null) return sh.io.Error("指定されたシェーダは見つかりません");
            mate=par.EditMaterial();
            mate.shader=shader;
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
                shape.randomDirectionAmount=0;
            }else if(sa.Length==2){
                if(sa[0]=="hemisphere") shape.shapeType=ParticleSystemShapeType.Hemisphere;
                else if(sa[0]=="hemisphereshell") shape.shapeType=ParticleSystemShapeType.HemisphereShell;
                else if(sa[0]=="sphere") shape.shapeType=ParticleSystemShapeType.Sphere;
                else if(sa[0]=="sphereshell") shape.shapeType=ParticleSystemShapeType.SphereShell;
                else return sh.io.Error("shapeの指定が不正です");
                if(!float.TryParse(sa[1],out f)||f<0) return sh.io.Error("数値の指定が不正です");
                shape.radius=f;
                shape.arc=360.0f;
                shape.arcMode=ParticleSystemShapeMultiModeValue.Random;
                shape.radiusMode=ParticleSystemShapeMultiModeValue.Random;
                shape.randomDirectionAmount=0;
            }else if(sa.Length==3){
                if(sa[0]=="cone"){
                    if(!float.TryParse(sa[1],out f)||f<0) return sh.io.Error("数値の指定が不正です");
                    shape.radius=f;
                    shape.arc=360.0f;
                    shape.shapeType=ParticleSystemShapeType.Cone;
                    shape.arcMode=ParticleSystemShapeMultiModeValue.Random;
                    shape.radiusMode=ParticleSystemShapeMultiModeValue.Random;
                    shape.randomDirectionAmount=0;
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
        var mi=new CmdMeshes.MeshInfo(tr);
        if(mi.count==0) return sh.io.Error("メッシュが見つかりません");
        if(val==null){
            for(int i=0; i<mi.count; i++){
                sh.io.Print($"{i}{sh.ofs}count={mi.oid.originalMesh.GetIndices(i).Length/3}");
                if(mi.material.Count>i) sh.io.Print($"{sh.ofs}mate={mi.material[i].name}{sh.ofs}shader={mi.material[i].shader.name}");
                sh.io.PrintLn("");
            }
            return 0;
        }
        string[] sa=ParseUtil.LeftAndRight(val,':');
        if(!int.TryParse(sa[0],out int n)||n<0||n>=mi.count) return sh.io.Error("メッシュ番号が不正です");

        string[] kv=ParseUtil.LeftAndRight(sa[1],'=');
        float[] fa;
        if(kv[0]=="" || kv[1]=="") return sh.io.Error("書式が不正です");
        if(kv[0][0]=='_'){
            string err;
            if(kv[1]=="on"){
                mi.EditMaterial();
                mi.material[n].EnableKeyword(kv[0]);
            }else if(kv[1]=="off"){
                mi.EditMaterial();
                mi.material[n].DisableKeyword(kv[0]);
            }else if(kv[1].IndexOf(',')>=0){
                mi.EditMaterial();
                if((err=CmdMeshes.SetColorProp(mi.material[n],kv[0],kv[1]))!="") return sh.io.Error(err);
            }else{
                mi.EditMaterial();
                if((err=CmdMeshes.SetFloatProp(mi.material[n],kv[0],kv[1]))!="") return sh.io.Error(err);
            }
        } else switch(kv[0]){
        case "color":
            fa=ParseUtil.Rgba(kv[1]);
            if(fa==null) return sh.io.Error(ParseUtil.error);
            mi.EditMaterial();
            mi.material[n].color=new Color(fa[0],fa[1],fa[2],fa[3]);
            break;
        case "shader":
            Shader shader=Shader.Find(kv[1]);
            if(shader==null) return sh.io.Error("指定されたシェーダは見つかりません");
            mi.EditMaterial();
            mi.material[n].shader=shader;
            break;
        case "blend":
            mi.EditMaterial();
            if(CmdMeshes.ChgBlendMode(mi.material[n],kv[1])<0)
                return sh.io.Error("blendにはopaque|cutout|fade|transparentのいずれかを指定して下さい");
            break;
        case "topology":
            CmdMeshes.MeshParamTopologySub(sh,mi,n,kv[1]);
            break;
        }
        return 1;
    }
    private static int ObjParamLookAt(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ return 0; }
        if(val.IndexOf(':')>=0){
            var cd=new ParseUtil.ColonDesc(val);
            var tgt=ObjUtil.FindObj(sh,cd);
            if(tgt==null) return sh.io.Error("対象が見つかりません");
            tr.transform.LookAt(tgt.position,tr.up);
        }else{
            float[] xyz=ParseUtil.Xyz(val);
            if(xyz==null) return sh.io.Error(ParseUtil.error);
            tr.transform.LookAt(new Vector3(xyz[0],xyz[1],xyz[2]),tr.up);
        }
        return 1;
    }
    private static int ObjParamLocIK(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIKSub(sh,tr,val,1);
    }
    private static int ObjParamLocIKMinus(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIKSub(sh,tr,val,-1);
    }

    private static int ObjParamLocIKSub(ComShInterpreter sh,Transform tr,string val,int dir){
        if(val==null) return 0;

        float[] xyz=ParseUtil.Xyz(val);
        if(xyz==null) return sh.io.Error(ParseUtil.error);

        Transform p1=null,p2=null;
        if(tr.name=="Bip01" || tr.name=="ManBip") return sh.io.Error("親ボーンが足りません");
        if(tr.parent!=null) p1=tr.parent;
        if(p1==null || p1.name=="Bip01" || p1.name=="ManBip") return sh.io.Error("親ボーンが足りません");
        if(p1.parent!=null) p2=p1.parent;
        if(p2==null) return sh.io.Error("親ボーンが足りません");

        Vector3 p=new Vector3(xyz[0],xyz[1],xyz[2]);
        float a=(p - p2.position).sqrMagnitude;
        if(Mathf.Approximately(a,0)) return sh.io.Error("ボーンの長さが0です");
        float b=(p1.position-tr.position).sqrMagnitude;
        if(Mathf.Approximately(b,0)) return sh.io.Error("ボーンの長さが0です");
        float c=(p2.position-p1.position).sqrMagnitude;
        if(Mathf.Approximately(c,0)) return sh.io.Error("ボーンの長さが0です");
        float co=(a-b-c)/-2/Mathf.Sqrt(b*c); // the law of cosine
        p1.localRotation=Quaternion.Euler(0,0,180-dir*Mathf.Acos(co)*Mathf.Rad2Deg);
        var w2l=p2.worldToLocalMatrix;
        var lpt=w2l.MultiplyPoint3x4(p);
        var lpw=w2l.MultiplyPoint3x4(tr.position);
        var lps=w2l.MultiplyPoint3x4(p2.position);
        p2.localRotation=p2.localRotation*Quaternion.FromToRotation((lpw-lps).normalized,(lpt-lps).normalized);
        return 1;
    }
}
public static class ObjUtil {
    public static Dictionary<string,Transform> objDic=new Dictionary<string,Transform>();
    public static Transform FindObj(ComShInterpreter sh,string name){
        if(name=="") return null;
        Transform tr=null;

        if(name[0]=='/'){
            tr=GameMain.Instance.gameObject.transform;
            if(name.Length>1) return tr.Find(name.Substring(1));
            return tr;
        }

        string[] sa=ParseUtil.LeftAndRight(name,'/');
        if(objDic.ContainsKey(sa[0])){  // objコマンドで作ったオブジェクト
            tr=objDic[sa[0]];
            if(tr==null){ objDic.Remove(sa[0]); return null; }
        }
        if(tr!=null) return (sa[1]=="")?tr:tr.Find(sa[1]);

        if(sh==null) return null;
        tr=GetPhotoPrefabTr(sh);
        if(tr!=null){
            tr=tr.Find(name);
            if(tr!=null) return tr;
        }
        if(sh.objRef!=""){
            tr=UTIL.GetObjRoot(sh.objRef);
            if(tr!=null) return tr.Find(name);
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
            tr=BoneUtil.FindBone(sh,sa[0],lr[0],"/");
        }
        if(tr==null) return null;
        if(lr[1]!="") return tr.Find(lr[1]);
        return tr;
    }
    public static Transform FindObj(ComShInterpreter sh,ParseUtil.ColonDesc cd){
        Transform tr=null;
        if(cd.num==0 && cd.id!="") return FindObj(sh,cd.id);
        if(cd.num==3){
            tr=BoneUtil.FindBone(sh,cd.type,cd.id,(cd.bone=="")?"/":cd.bone);
        }else{
            if(cd.type=="obj") return FindObj(sh,cd.id);
            tr=BoneUtil.FindBone(sh,cd.type,cd.id,"/");
        }
        if(tr==null) return null;
        if(cd.path!="") return tr.Find(cd.path);
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
            return UTIL.GetObjRoot("");
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
        List<TMorph> morph=null;
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
            if(mo==null) mo=new MenuObj(fname+".model");
            morph=new List<TMorph>();
            var ga=MenuObj.ToObject(mo,morph);
            if(ga.Count>0){
                o=new GameObject("");
                foreach(var g in ga) g.transform.SetParent(o.transform);
            }
            instanceq=false; // この場合は既にインスタンス化されてる
        }
        if(o==null) return null;
        GameObject b;
        if(o.name=="") b=o; else b=new GameObject();
        b.name=name;
        b.transform.SetParent(pr);
        GameObject go=o;
        if(instanceq) go=UnityEngine.Object.Instantiate(o,b.transform); else go.transform.SetParent(b.transform);
        int idx=go.name.IndexOf("(Clone)",Ordinal);
        if(idx>=0) go.name=go.name.Substring(0,idx);
        b.transform.localPosition=pos;
        b.transform.localRotation=Quaternion.Euler(rot);
        b.transform.localScale=scl;
        ObjInfo.AddObjInfo(b,src,morph);
        return b;
    }
    public static GameObject CloneObject(string name,Transform obase,Transform pr){
        Transform orig=obase;
        ObjInfo oi=orig.GetComponent<ObjInfo>();
        List<TMorph> morph=null;
        if(oi!=null){ morph=oi.data.morph; if(orig.childCount==1) orig=orig.GetChild(0); }
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
            public string shader;
            public string slot;
        }
        public class Anm{
            public string name;
            public bool loopq;
            public Anm(string name){ this.name=name; }
        }
        public Dictionary<string,List<Material>> material=new Dictionary<string,List<Material>>();
        public Dictionary<string,string> modelName=new Dictionary<string,string>();
        public Dictionary<string,Anm> anmFile=new Dictionary<string,Anm>();
        public MenuObj(){}
        public MenuObj(string model){ modelName.Add("",model);}
        public Material FindMaterial(string slot,int no){
            List<Material> mlist;
            if(!material.TryGetValue(slot,out mlist)) return null;
            foreach(var m in mlist) if(m.no==no) return m;
            return null;
        }
        public Material AddMaterial(string slot,int no,string name){
            Material m=FindMaterial(slot,no);
            if(m==null){
                List<Material> mlist;
                if(!material.TryGetValue(slot,out mlist)){
                    mlist=new List<Material>();
                    material.Add(slot,mlist);
                }
                m=new Material();
                m.slot=slot; m.no=no; m.name=name;
                mlist.Add(m);
                return m;
            }else{
                m.name=name;
                return m;
            }
        }
        public void ChgShader(string slot,int no,string shader){
            Material m=FindMaterial(slot,no);
            if(m==null) m=AddMaterial(slot,no,"");
            m.shader=shader;
        }

        public static List<GameObject> ToObject(MenuObj mo,List<TMorph> morph){
            List<GameObject> ret=new List<GameObject>();
            foreach(var kv in mo.modelName){
                string slot=kv.Key;
                string file=kv.Value;
                if(!string.IsNullOrEmpty(file) && GameUty.IsExistFile(file)){
                    TBodySkin tbs=dummyMaid.body0.goSlot[0];
                    var m=new TMorph(tbs);
                    GameObject o=ImportCM.LoadSkinMesh_R(file,m,"body",tbs,0);
                    if(o!=null){
                        if(m.MorphCount>0){
                            m.InitGameObject(o);
                            morph.Add(m);
                        }
                        if(o.name.Length>4 && o.name.StartsWith("_SM_",Ordinal))
                            o.name=o.name.Substring(4); // "_SM_"付きだとコロン記法でボーンを辿れない
                    }
                    tbs.listDEL.Clear();
                    if(mo.material.TryGetValue(slot,out List<Material> mlist)) foreach(var mate in mlist)
                        SetMaterial(o.transform,mate.no,mate.name,mate.shader);
                    if(mo.anmFile.TryGetValue(slot,out MenuObj.Anm anm)){
                        var a=o.GetComponent<Animation>();
                        if(a==null) a=o.AddComponent<Animation>();
                        PlayMotion(a,anm.name,1.0f,anm.loopq?1:0);
                    }
                    ret.Add(o);
                }  
            }
            return ret;
        }
    }
    private static char[] menuWhite={' ','\t','\u3000'};
    public static MenuObj ReadMenuObj(string fname,MenuObj mo=null){
        byte[] buf=UTIL.AReadAll(fname);
        if(buf==null) return null;
        return ReadMenuObj(buf,mo);
    }
    public static MenuObj ReadMenuObj(byte[] buf,MenuObj mo=null){
        MenuObj ret=(mo==null)?new MenuObj():mo;
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
                        string slot="";
                        if(args.Count>=3) slot=args[2];
                        if(args.Count>=2) ret.modelName[slot]=UTIL.Suffix(args[1],".model");
                    }else if(cmd=="アイテム"){
                        if(args.Count>=2) ReadMenuObj(UTIL.Suffix(args[1],".menu"),ret);
                    }else if(cmd=="マテリアル変更"){
                        if(args.Count>=4) ret.AddMaterial(args[1],int.Parse(args[2]),UTIL.Suffix(args[3],".mate"));
                    }else if(cmd=="shader"){
                        if(args.Count>=4) ret.ChgShader(args[1],int.Parse(args[2]),args[3]);
                    }else if(cmd=="anime"){
                        string slot="";
                        if(args.Count>=2) slot=args[1];
                        if(args.Count>=3){ ret.anmFile[slot]=new MenuObj.Anm(UTIL.Suffix(args[2],".anm")); }
                        if(args.Count>=4 && args[3]=="loop") ret.anmFile[slot].loopq=true;
                    }
                }
            }
        }catch{}
        return ret;
    }
    public static int SetMaterial(Transform tr,int no,string fname,string shader=null){
        string mate=UTIL.Suffix(fname,".mate");
        foreach(var r in tr.GetComponentsInChildren<Renderer>()){
            var sma=r.sharedMaterials;
            if (sma!=null && no<sma.Length)
                try{
                    sma[no]=ImportCM.LoadMaterial(mate,null,sma[no]);
                    if(shader!=null){
                        Shader sh=Shader.Find(shader);
                        if(sh!=null) sma[no].shader=sh;
                    }
                    r.sharedMaterials=sma;
                }catch{ return -1; }
        }
        return 0;
    }
    public static int ChgMaterial(Transform tr,int no,string fname,string shader=null){
        string mate=UTIL.Suffix(fname,".mate");
        var oi=tr.GetComponent<ObjInfo>();
        if(oi==null) return -2;
        foreach(var r in oi.data.FindComponents<Renderer>()){
            var sma=r.sharedMaterials;
            if (sma!=null && no<sma.Length)
                try{
                    sma[no]=ImportCM.LoadMaterial(mate,null,sma[no]);
                    if(shader!=null){
                        Shader sh=Shader.Find(shader);
                        if(sh!=null) sma[no].shader=sh;
                    }
                    r.sharedMaterials=sma;
                }catch{ return -1; }
        }
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
