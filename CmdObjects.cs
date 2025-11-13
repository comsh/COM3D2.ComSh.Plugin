using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
        objParamDic.Add("wposrot",new CmdParam<Transform>(_CmdParamWPosRot));
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
        objParamDic.Add("lposrot",new CmdParam<Transform>(_CmdParamLPosRot));
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
        objParamDic.Add("particle.new",new CmdParam<Transform>(ObjParamParticleAdd));
        objParamDic.Add("handle",new CmdParam<Transform>(ObjParamHandle));
        objParamDic.Add("describe",new CmdParam<Transform>(ObjParamDesc));
        objParamDic.Add("type",new CmdParam<Transform>(ObjParamType));
        objParamDic.Add("mesh",new CmdParam<Transform>(ObjParamMesh));
        objParamDic.Add("component",new CmdParam<Transform>(ObjParamComponent));
        objParamDic.Add("lookat",new CmdParam<Transform>(ObjParamLookAt));
        objParamDic.Add("lookatlocal",new CmdParam<Transform>(ObjParamLookAtLocal));
        objParamDic.Add("locik",new CmdParam<Transform>(ObjParamLocIK));
        objParamDic.Add("locik-",new CmdParam<Transform>(ObjParamLocIKMinus));
        objParamDic.Add("locik.z",new CmdParam<Transform>(ObjParamLocIK));
        objParamDic.Add("locik-.z",new CmdParam<Transform>(ObjParamLocIKMinus));
        objParamDic.Add("locik.x",new CmdParam<Transform>(ObjParamLocIKX));
        objParamDic.Add("locik-.x",new CmdParam<Transform>(ObjParamLocIKMinusX));
        objParamDic.Add("locik.y",new CmdParam<Transform>(ObjParamLocIKY));
        objParamDic.Add("locik-.y",new CmdParam<Transform>(ObjParamLocIKMinusY));
        objParamDic.Add("locik3",new CmdParam<Transform>(ObjParamLocIK3));
        objParamDic.Add("locik3-",new CmdParam<Transform>(ObjParamLocIK3Minus));
        objParamDic.Add("locik3.z",new CmdParam<Transform>(ObjParamLocIK3));
        objParamDic.Add("locik3-.z",new CmdParam<Transform>(ObjParamLocIK3Minus));
        objParamDic.Add("locik3.x",new CmdParam<Transform>(ObjParamLocIK3X));
        objParamDic.Add("locik3-.x",new CmdParam<Transform>(ObjParamLocIK3MinusX));
        objParamDic.Add("locik3.y",new CmdParam<Transform>(ObjParamLocIK3Y));
        objParamDic.Add("locik3-.y",new CmdParam<Transform>(ObjParamLocIK3MinusY));
        objParamDic.Add("ik",new CmdParam<Transform>(ObjParamIK));
        objParamDic.Add("shadow",new CmdParam<Transform>(ObjParamShadow));
        objParamDic.Add("join",new CmdParam<Transform>(ObjParamJoin));
        objParamDic.Add("bakemesh",new CmdParam<Transform>(ObjParamBakeMesh));
        objParamDic.Add("visible",new CmdParam<Transform>(ObjParamVisible));
        objParamDic.Add("iid",new CmdParam<Transform>(_CmdParamIid));
        objParamDic.Add("collider",new CmdParam<Transform>(ObjParamCollider));
        objParamDic.Add("rigidbody",new CmdParam<Transform>(ObjParamRigidbody));
        objParamDic.Add("hingejoint",new CmdParam<Transform>(ObjParamHingeJoint));
        objParamDic.Add("balljoint",new CmdParam<Transform>(ObjParamBallJoint));
        objParamDic.Add("springjoint",new CmdParam<Transform>(ObjParamSpringJoint));
        objParamDic.Add("fixjoint",new CmdParam<Transform>(ObjParamFixJoint));
        objParamDic.Add("addforce",new CmdParam<Transform>(ObjParamAddForce));
        objParamDic.Add("addtorque",new CmdParam<Transform>(ObjParamAddTorque));
        objParamDic.Add("bounce",new CmdParam<Transform>(ObjParamBounce));
        objParamDic.Add("friction",new CmdParam<Transform>(ObjParamFriction));
        objParamDic.Add("active",new CmdParam<Transform>(ObjParamActive));
        objParamDic.Add("wheel",new CmdParam<Transform>(ObjParamWheel));
        objParamDic.Add("cloth",new CmdParam<Transform>(ObjParamCloth));
        objParamDic.Add("reflection",new CmdParam<Transform>(ObjParamReflection));
        objParamDic.Add("addbone",new CmdParam<Transform>(ObjParamAddBone));
        objParamDic.Add("bone",new CmdParam<Transform>(ObjParamBone));
        objParamDic.Add("bbox",new CmdParam<Transform>(ObjParamBBox));
        
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
            if(!UTIL.ValidObjName(pa[1])) return sh.io.Error("その名前は使用できません");
            if((pftr=ObjUtil.GetPhotoPrefabTr(sh))==null) return sh.io.Error("オブジェクト作成に失敗しました");
            if(ObjUtil.FindObj(sh,pa[1])!=null||LightUtil.FindLight(sh,pa[1])!=null) return sh.io.Error("その名前は既に使われています");
            float[] pos,rot,scl;
            if((pos=ParseUtil.Xyz(pa[2]))==null) return sh.io.Error(ParseUtil.error);
            if((rot=ParseUtil.Xyz(pa[3]))==null) return sh.io.Error(ParseUtil.error);
            if((scl=ParseUtil.Xyz2(pa[4]))==null) return sh.io.Error(ParseUtil.error);
            GameObject go=ObjUtil.AddObject(pa[0],pa[1],pftr,
               new Vector3(pos[0],pos[1],pos[2]),new Vector3(rot[0],rot[1],rot[2]),new Vector3(scl[0],scl[1],scl[2]));
            if(go==null) return sh.io.Error("オブジェクト作成に失敗しました");
            ObjUtil.objDic[go.transform.name]=go.transform;
            return 0;
        }
        if(args[1]=="asset"){
            if(args.Count==3){
                List<string> files=ObjUtil.ListAssetBundle<GameObject>(args[2]);
                if(files==null) return sh.io.Error("アセットバンドルが読みこめません");
                foreach(var f in files) sh.io.PrintLn(f);
            }else if(args.Count==5){
                if(!UTIL.ValidObjName(args[4])) return sh.io.Error("その名前は使用できません");
                if((pftr=ObjUtil.GetPhotoPrefabTr(sh))==null) return sh.io.Error("オブジェクト作成に失敗しました");
                if(ObjUtil.FindObj(sh,args[4])!=null||LightUtil.FindLight(sh,args[4])!=null) return sh.io.Error("その名前は既に使われています");
                GameObject go=ObjUtil.LoadAssetBundle<GameObject>(args[2],args[3]);
                if(go==null) return sh.io.Error("オブジェクト作成に失敗しました");
                go.name=args[4];
                go.transform.SetParent(pftr);
                ObjUtil.objDic[go.transform.name]=go.transform;
            }else return sh.io.Error("使い方: obj asset アセットバンドル名 ファイル名 識別名");
            Resources.UnloadUnusedAssets();
            GC.Collect();
            return 0;
        }
        if(args[1]=="addchild"){
            if(args.Count!=5) return sh.io.Error("使い方: obj addchild 種類 識別名 親オブジェクト名");
            if(!UTIL.ValidObjName(args[3])) return sh.io.Error("その名前は使用できません");
            Transform pa;
            if((pa=ObjUtil.FindObj(sh,args[4].Split(ParseUtil.colon)))==null) return sh.io.Error("親オブジェクトが見つかりません");
            if(pa.Find(args[3])!=null) return sh.io.Error("その名前は既に使われています");
            GameObject go=ObjUtil.AddObject(args[2],args[3],pa,Vector3.zero,Vector3.zero,Vector3.one);
            if(go==null) return sh.io.Error("オブジェクト作成に失敗しました");
            var oi=ObjInfo.GetObjInfo(pa);
            if(oi!=null) oi.data.UpdateBones();
            return 0;
        }
        if(args[1]=="create"){
            if(args.Count==2) return sh.io.Error("使い方: obj create 種類 [識別名]");
            if(args.Count>4) return sh.io.Error("引数が多すぎます");
            string type=args[2],name=(args.Count==3)?AutoObjName(type):args[3];
            if(!UTIL.ValidObjName(name)) return sh.io.Error("その名前は使用できません");
            if((pftr=ObjUtil.GetPhotoPrefabTr(sh))==null) return sh.io.Error("オブジェクト作成に失敗しました");
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
            }else if(type=="capsule"){
                go=GameObject.CreatePrimitive(PrimitiveType.Capsule);
            }else if(type.StartsWith("plane",Ordinal)){
                if(type.Length==5) go=GameObject.CreatePrimitive(PrimitiveType.Plane);
                else{
                    float[] xy=ParseUtil.Xy(ParseUtil.RightOf(type,':'));
                    if(xy==null) return sh.io.Error("書式が不正です");
                    if(xy[0]<=0||xy[1]<=0) return sh.io.Error("数値が不正です");
                    go=ObjUtil.CreateCustomPlane((int)xy[0],(int)xy[1]);
                }
            }else return sh.io.Error("種類にはcube|sphere|cylinder|quad|plane|capsuleのいずれかを指定してください");
            UTIL.ResetTr(go.transform);
            go.name=name;
            ObjInfo.AddObjInfo(go,"."+type);
            go.transform.SetParent(pftr);
            ObjUtil.objDic[go.transform.name]=go.transform;
            return 0;
        }
        if(args[1]=="clone"){
            if(args.Count!=4) return sh.io.Error("使い方: obj clone コピー元識別名 コピー先識別名");
            Transform tr=ObjUtil.FindObj(sh,args[2].Split(ParseUtil.colon));
            if(tr==GameMain.Instance.MainCamera.camera.transform) return sh.io.Error("メインカメラにこの操作は行えません");
            if(tr==null) return sh.io.Error("対象が見つかりません");
            if(!UTIL.ValidObjName(args[3])) return sh.io.Error("その名前は使用できません");
            if((pftr=ObjUtil.GetPhotoPrefabTr(sh))==null) return sh.io.Error("オブジェクト作成に失敗しました");
            if(ObjUtil.FindObj(sh,args[3])!=null||LightUtil.FindLight(sh,args[3])!=null) return sh.io.Error("その名前は既に使われています");
            GameObject go=ObjUtil.CloneObject(args[3],tr,pftr);
            if(go==null) return sh.io.Error("オブジェクト作成に失敗しました");
            ObjUtil.objDic[go.transform.name]=go.transform;
		    return 0;
        }
        if(args[1]=="del" && args.Count>=3){
            for(int i=2; i<args.Count; i++) if(ObjUtil.DeleteObj(sh,args[i])<0) return -1;
            return 0;
        }
        return CmdObjectSub(sh,new ParseUtil.ColonDesc(args[1]),args,2);
    }
    public static List<Transform> GetObjList(ComShInterpreter sh){
        var ret=new List<Transform>(64);
        var oset=new HashSet<string>(); // 重複削除
        Transform pftr=ObjUtil.GetPhotoPrefabTr2(sh);

        var studiobase=GameMain.Instance.transform.Find("BG/PhotoPrefab");

        if(sh.objRef.Length>0){ // スタジオモード分参照
            var stdlist=StudioMode.GetObjectList();
            if(stdlist!=null) for(int i=0; i<stdlist.Count; i++) ObjUtil.objDic[stdlist[i].name]=stdlist[i];
        }
        // BG
        var bg=GameMain.Instance.BgMgr.BgObject;
        if(bg!=null && bg.transform==pftr){ // 子が1つもないBGはそれ自身を一覧に出す
            bool has_child=false;
            for(int i=0; i<bg.transform.childCount; i++){
                var c=bg.transform.GetChild(i);
                if(c!=null&&c.GetComponent<ObjInfo>()==null){has_child=true;break;}
            }
            if(!has_child) ObjUtil.objDic[bg.name]=bg.transform;
        }
        // 現root直下のobject
        if(pftr!=null){
            for(int i=0; i<pftr.childCount; i++){
                Transform tr=pftr.GetChild(i);
                if(tr==null) continue;
                oset.Add(tr.name);
                ret.Add(tr);
            }
        }
        List<string> remove=new List<string>(ObjUtil.objDic.Count);
        foreach(var kv in ObjUtil.objDic){
            Transform tr=kv.Value;
            if(tr==null){ remove.Add(kv.Key); continue; }
            if(oset.Contains(tr.name)) continue; // root直下のものは取得済
            if(sh.objRef.Length==0 && studiobase!=null && tr.parent==studiobase) continue;
            // 何かにアタッチされているもの
            ret.Add(tr);
        }
        foreach(string k in remove) ObjUtil.objDic.Remove(k);
        return ret;
    }
    private static string AutoObjName(string name){
        string seq=UTIL.GetSeqId();
        string ret=string.Concat(System.IO.Path.GetFileNameWithoutExtension(name).Replace(' ','_'),"_",seq);
        if(!UTIL.ValidObjName(ret)) ret="object_"+seq;
        return ret;
    }
    private static ParseUtil.ColonDesc colonDesc;
    public static int CmdObjectSub(ComShInterpreter sh,ParseUtil.ColonDesc cd,List<string> args,int startpos){
        colonDesc=cd;
        if(cd.id=="//" && cd.num==0 && cd.meshno<0){
            var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            foreach(var root in scene.GetRootGameObjects()){ sh.io.PrintLn(root.name); Debug.Log(root.name); }
            return 0;
        }
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
        if(ObjUtil.DeleteObj(sh,tr)<0) return -1;
        return 0;
    }
    private static int ObjParamAttach(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return 0;
        if(tr==GameMain.Instance.MainCamera.camera.transform) return sh.io.Error("メインカメラにこの操作は行えません");
        Transform to;
        int opt,jmpq=0;
        if((opt=val.IndexOf(','))>=0){
            if(!int.TryParse(val.Substring(opt+1),out jmpq)) return sh.io.Error("数値の形式が不正です");
            val=val.Substring(0,opt);
        }
        var cd=new ParseUtil.ColonDesc(val);
        to=ObjUtil.FindObj(sh,cd);
        if(to==null) return sh.io.Error("対象が見つかりません");

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
    private static int ObjParamJoin(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return 0;
        if(tr==GameMain.Instance.MainCamera.camera.transform) return sh.io.Error("メインカメラにこの操作は行えません");
        int opt,jmpq=0;
        if((opt=val.IndexOf(','))>=0){
            if(!int.TryParse(val.Substring(opt+1),out jmpq)) return sh.io.Error("数値の形式が不正です");
            val=val.Substring(0,opt);
        }
        var cd=new ParseUtil.ColonDesc(val);
        if(cd.type!="" && cd.type!="obj") return sh.io.Error("join対象にはオブジェクトのみ指定可能です");
       
        // アッタッチ先(根)
        var to0=(cd.type=="")?ObjUtil.FindObj(sh,cd):ObjUtil.FindObj(sh,cd.id);
        if(to0==null) return sh.io.Error("対象が見つかりません");
        // アタッチ先(末端)
        var to=ObjUtil.FindObj(sh,cd);
        if(to==null) return sh.io.Error("対象が見つかりません");

        var ms=MaidUtil.GetParentMaidList(to,tr);
        if(ms==null) return sh.io.Error("親子関係がループになるため、アタッチできません");

        var oi2=to0.GetComponent<ObjInfo>();
        if(oi2==null) oi2=ObjInfo.AddObjInfo(to0,"");
        var oi=ObjInfo.GetObjInfo(tr);
        if(jmpq==2) UTIL.ResetTr(tr);
        tr.SetParent(to,jmpq==0);
        if(oi!=null){
            if(System.Object.ReferenceEquals(oi.transform,tr)){
                oi.enabled=false;
                ObjUtil.objDic.Remove(tr.name);
            }else oi.data.UpdateBones();
        }
        oi2.data.UpdateBones();
        return 1;
    }
    private static int ObjParamDetach(ComShInterpreter sh,Transform tr,string val){
        if(tr==GameMain.Instance.MainCamera.camera.transform) return sh.io.Error("メインカメラにこの操作は行えません");
        var pftr=ObjUtil.GetPhotoPrefabTr(sh);
        if(pftr==null) return sh.io.Error("失敗しました");
        if(tr.parent!=pftr) tr.SetParent(pftr,true);
        return 0;
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
        }else if(val=="t"||val=="T"||val=="tree"||val=="Tree"){
            bool rootq=val[0]=='T';
            UTIL.TraverseTr(tr,(Transform t,int d)=>{
                if(!string.IsNullOrEmpty(t.name)){
                    for(int i=0; i<d; i++) sh.io.Print("  ");
                    sh.io.PrintLn(t.name);
                }
                return 0;
            },rootq);
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
        Transform tr0=ObjUtil.FindObjRoot(sh,colonDesc);
        if(tr0==null)  return sh.io.Error("オブジェクトが見つかりません");
        var mi=new CmdMeshes.MeshInfo(tr,tr0);
        if(mi==null) return sh.io.Error("マテリアルが見つかりません");

        if(val==null){
            for(int i=0; i<mi.material.Count; i++)
                sh.io.PrintLn($"{mi.material[i].name}{sh.ofs}{mi.material[i].shader.name}");
            return 0;
        }

        string[] sa=val.Split(ParseUtil.colon);
        if(sa.Length!=2) return sh.io.Error("書式が不正です");
        if(!int.TryParse(sa[0],out int idx)||idx<0||idx>=mi.material.Count) return sh.io.Error("マテリアル番号が不正です");
        mi.EditMaterial();
        try{
            Material mate=mi.material[idx];
            ImportCM.LoadMaterial(sa[1],null,mate);
        }catch{ return sh.io.Error("ファイルの読み込みに失敗しました"); }
        return 1;
    }
    private static int ObjParamMaterialCopy(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return 0;
        string[] lr=ParseUtil.LeftAndRight(val,':');
        if(lr[0]==""||lr[1]=="") return sh.io.Error("書式が不正です");
        int meshno=0;
        if(!int.TryParse(lr[0],out meshno)||meshno<0) return sh.io.Error("メッシュ番号が不正です");

        Transform tr0=ObjUtil.FindObjRoot(sh,colonDesc);
        if(tr0==null) return sh.io.Error("オブジェクトが存在しません");
        var mi=new CmdMeshes.MeshInfo(tr,tr0);
        if(mi.count<=meshno) return sh.io.Error("指定されたメッシュが存在しません");
        var sm=new CmdMeshes.SingleMesh(meshno,mi);
        return CmdMeshes.MeshParamMaterialCopy(sh,sm,lr[1]);
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
        var lr=ParseUtil.LeftAndRight(val,',');
        var sw=lr[0].ToLower();
        if(sw=="off"){
            if(hdl!=null) ComShHandle.DelHandle(hdl);
            return 1;
        }
        float scale=0.75f;
        if(lr[1]!="" && (!float.TryParse(lr[1],out scale)||scale<0.1)) return sh.io.Error("値が不正です");
        if(hdl==null){ hdl=ComShHandle.AddHandle(tr); hdl.Visible=true;}
        if(ComShHandle.SetHandleType(hdl,sw)<0)
            return sh.io.Error("off|lpos|wpos||rot|wrot|scale|lposrot|wposrotのいずれかを指定してください");
        hdl.offsetScale=scale;
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
        string[] sa=val.Split(ParseUtil.lf);
        for(int pi=0; pi<sa.Length; pi++){
            string[] kv=ParseUtil.LeftAndRight(sa[pi],ParseUtil.eqcln2);
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
        }
        return 1;
    }

    private static int OldParticleSub(ComShInterpreter sh,OldParticle op,string p,string v){
        float f; float[] fa; Material mate;
        if(p.Length==0) return sh.io.Error("書式が不正です");
        if(p[0]=='_'){
            if(op.render==null || op.render.sharedMaterial==null) return 0;
            string err;
            if(p=="_MainTex"){
                mate=op.EditMaterial();
                if((err=CmdMeshes.SetTexProp(mate,p,v,null))!="") return sh.io.Error(err);
            }else if(v=="on"){
                mate=op.EditMaterial();
                mate.EnableKeyword(p);
            }else if(v=="off"){
                mate=op.EditMaterial();
                mate.DisableKeyword(p);
            }else if(v.IndexOf(',')>=0){
                mate=op.EditMaterial();
                if((err=CmdMeshes.SetVectorProp(mate,p,v))!="") return sh.io.Error(err);
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
        case "rq":
            if(op.render==null) return 0;
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            mate=op.EditMaterial();
            mate.renderQueue=(int)f;
            break;
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
        case "duration":
            if(op.emit==null) return 0;
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            op.emit.maxEnergy=f;
            break;
        case "scale":
        case "loop":
        case "max":
        case "gravity":
        case "speedlimit":
        case "delay":
        case "simspeed":
        case "shape":
        case "stretch":
        case "png":
        case "rot":
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
            if(pi==null) pi=sys.gameObject.AddComponent<ParticleInfo>();
            return render.material;
        }
    }   
    private static int ObjParamParticleAdd(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return 0;
        var ps=tr.gameObject.GetComponent<ParticleSystem>();
        if(ps!=null) return sh.io.Error("パーティクル追加済です");
        ps=tr.gameObject.AddComponent<ParticleSystem>();
        var pr=tr.gameObject.GetComponent<ParticleSystemRenderer>(); // 勝手に追加されるっぽい
        if(pr==null) pr=tr.gameObject.AddComponent<ParticleSystemRenderer>();
        ps.name=pr.name=val;
        // とりあえず最低限の初期値
        var emit=ps.emission;
        emit.enabled=true;
        emit.rateOverTime=1;
        var mate=new Material(Shader.Find("Particles/Additive (Soft)"));
        mate.SetTexture("_MainTex",Texture2D.blackTexture);
        pr.sharedMaterial=mate;
        return 1;
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
                var shname="";
                if(par[i].render!=null && par[i].render.sharedMaterial!=null && par[i].render.sharedMaterial!=null && par[i].render.sharedMaterial.shader.name!=null)
                    shname=par[i].render.sharedMaterial.shader.name;
                sh.io.PrintLn(pa[i].name+sh.ofs+shname);
            }
            return 0;
        }
        string[] sa=val.Split(ParseUtil.lf);
        for(int pi=0; pi<sa.Length; pi++){
            string[] kv=ParseUtil.LeftAndRight(sa[pi],ParseUtil.eqcln2);
            string[] np=ParseUtil.LeftAndRight(kv[0],'.');
            string n=np[0],p=np[1],v=kv[1];
            int ret;
            if(p==""){ p=n; n=""; }
            if(n!=""){  // 名前指定があれば一致するものだけ
                bool found=false;
                for(int i=0; i<par.Length; i++) if(n==par[i].sys.name){
                    found=true;
                    if((ret=ParticleSub(sh,par[i],p,v,tr))<0) return ret;
                }
                if(!found) return sh.io.Error("指定されたパーティクルは存在しません");
            }else{      // 名前指定がなければ全部
                for(int i=0; i<par.Length; i++)
                    if((ret=ParticleSub(sh,par[i],p,v,tr))<0) return ret;
            }
        }
        return 1;
    }
    private static int ParticleSub(ComShInterpreter sh,NewParticle par,string p,string v,Transform tr){
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
            if(p=="_MainTex"){
                mate=par.EditMaterial();
                if((err=CmdMeshes.SetTexProp(mate,p,v,null))!="") return sh.io.Error(err);
            }else if(v=="on"){
                mate=par.EditMaterial();
                mate.EnableKeyword(p);
            }else if(v=="off"){
                mate=par.EditMaterial();
                mate.DisableKeyword(p);
            }else if(v.IndexOf(',')>=0){
                mate=par.EditMaterial();
                if((err=CmdMeshes.SetVectorProp(mate,p,v))!="") return sh.io.Error(err);
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
        case "rq":
            if(par.render==null) return 0;
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            mate=par.EditMaterial();
            mate.renderQueue=(int)f;
            break;
        case "png":
            string file="";
            if(v!="" && v.IndexOf('\\')<0){
                if(v[0]=='*'){
                    var tf=DataFiles.CreateTempFile(v.Substring(1),"");
                    file=tf.filename;
                }else if(UTIL.CheckFileName(v)>=0){
                    file=ComShInterpreter.homeDir+@"PhotoModeData\\Texture\\"+UTIL.Suffix(v,".png");
                }
            }
            if(file=="") return sh.io.Error("ファイル名が不正です");
            var tex=par.render.sharedMaterial.GetTexture("_MainTex");
            if(tex!=null){
                int ret=CmdMeshes.MeshParamPNGSub2(sh,tex,file);
                if(ret<0) return -1;
            }
            break;
        case "rot":
            fa=ParseUtil.MinMax(v);
            if(fa==null||fa[0]<0||fa[1]<0) return sh.io.Error("数値の形式が不正です");
            main.startRotation3D=false;
            main.startRotation=new ParticleSystem.MinMaxCurve(fa[0]*Mathf.Deg2Rad,fa[1]*Mathf.Deg2Rad);
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
            else main.scalingMode=ParticleSystemScalingMode.Shape;
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
                if(sa[0]=="mesh"){
                    var cd=new ParseUtil.ColonDesc(sa[1]);
                    var meshtr=ObjUtil.FindObj(sh,cd);
                    if(meshtr==null) return sh.io.Error("メッシュがありません");

                    Transform meshtr0=ObjUtil.FindObjRoot(sh,cd);
                    if(meshtr0==null) return sh.io.Error("オブジェクトが存在しません");
                    var mi=new CmdMeshes.MeshInfo(meshtr,meshtr0);
                    if(mi.count<=cd.meshno) return sh.io.Error("メッシュがありません");
                    int midx=mi.mesh.FindMeshIdx(cd.meshno);
                    if(midx<0) return sh.io.Error("メッシュがありません");
                    Renderer r=mi.mesh[midx].rend;

                    if(r.GetType()==typeof(SkinnedMeshRenderer)){
                        shape.shapeType=ParticleSystemShapeType.SkinnedMeshRenderer;
                        shape.skinnedMeshRenderer=(SkinnedMeshRenderer)r; 
                    }else{
                        shape.shapeType=ParticleSystemShapeType.MeshRenderer;
                        shape.meshRenderer=(MeshRenderer)r;
                    }
                }else{
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
                }
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
                }else if(sa[0]=="coneshell"){
                    if(!float.TryParse(sa[1],out f)||f<0) return sh.io.Error("数値の指定が不正です");
                    shape.radius=f;
                    shape.arc=360.0f;
                    shape.shapeType=ParticleSystemShapeType.ConeShell;
                    shape.arcMode=ParticleSystemShapeMultiModeValue.Random;
                    shape.radiusMode=ParticleSystemShapeMultiModeValue.Random;
                    shape.randomDirectionAmount=0;
                    if(!float.TryParse(sa[2],out f)||f<0||f>90) return sh.io.Error("数値の指定が不正です");
                    shape.angle=f;
                }else return sh.io.Error("shapeの指定が不正です");
            }else return sh.io.Error("shapeの指定が不正です");
            break;
        case "stretch":
            if(!float.TryParse(v,out f)||f<0) return sh.io.Error("数値の指定が不正です");
            par.render.velocityScale=f;
            par.render.cameraVelocityScale=0;
            if(f>0) par.render.renderMode=ParticleSystemRenderMode.Stretch;
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
    public static int ObjParamComponent(ComShInterpreter sh,Transform tr,string val){
        Component[] ca=tr.GetComponentsInChildren<Component>(true);
        if(val==null){
            for(int i=0; i<ca.Length; i++) if(ca[i]!=null){
                int iid=ca[i].GetInstanceID();
                if(ReferenceEquals(ca[i].GetType(),typeof(Transform))) continue;
                sh.io.PrintJoin(sh.ofs,iid.ToString(),ca[i].GetType().FullName,ca[i].transform.name);
                if(ca[i] is UnityEngine.MonoBehaviour)
                    sh.io.Print(sh.ofs+(((MonoBehaviour)ca[i]).enabled?"on":"off"));
                else if(ca[i] is UnityEngine.Behaviour)
                    sh.io.Print(sh.ofs+(((Behaviour)ca[i]).enabled?"on":"off"));
                else if(ca[i] is UnityEngine.Renderer)
                    sh.io.Print(sh.ofs+(((Renderer)ca[i]).enabled?"on":"off"));
                else if(ca[i] is UnityEngine.Collider)
                    sh.io.Print(sh.ofs+(((Collider)ca[i]).enabled?"on":"off"));
                else if(ca[i] is UnityEngine.Cloth)
                    sh.io.Print(sh.ofs+(((Cloth)ca[i]).enabled?"on":"off"));
                sh.io.PrintLn("");
            }
            return 0;
        }
        var sa=ParseUtil.LeftAndRight(val,'=');
        if(sa[1]!="on"&&sa[1]!="off"&&sa[1]!="del") return sh.io.Error("onまたはoffまたはdelを指定してください");
        if(!int.TryParse(sa[0],out int tgt)) return sh.io.Error("idの指定が不正です");
        for(int i=0; i<ca.Length; i++){
            if(ReferenceEquals(ca[i].GetType(),typeof(Transform))) continue;
            if(ca[i].GetInstanceID()!=tgt) continue;
            if(sa[1]=="on"){
                if(ca[i] is UnityEngine.MonoBehaviour) ((MonoBehaviour)ca[i]).enabled=true;
                else if(ca[i] is UnityEngine.Behaviour) ((Behaviour)ca[i]).enabled=true;
                else if(ca[i] is UnityEngine.Renderer) ((Renderer)ca[i]).enabled=true;
                else if(ca[i] is UnityEngine.Collider) ((Collider)ca[i]).enabled=true;
                else if(ca[i] is UnityEngine.Cloth) ((Cloth)ca[i]).enabled=true;
            }else if(sa[1]=="off"){
                if(ca[i] is UnityEngine.MonoBehaviour) ((MonoBehaviour)ca[i]).enabled=false;
                else if(ca[i] is UnityEngine.Behaviour) ((Behaviour)ca[i]).enabled=false;
                else if(ca[i] is UnityEngine.Renderer) ((Renderer)ca[i]).enabled=false;
                else if(ca[i] is UnityEngine.Collider) ((Collider)ca[i]).enabled=false;
                else if(ca[i] is UnityEngine.Cloth) ((Cloth)ca[i]).enabled=false;
            }else UnityEngine.Object.Destroy(ca[i]);
            break;
        }
        return 1;
    }

    private static int ObjParamMesh(ComShInterpreter sh,Transform tr,string val){
        var tr0=ObjUtil.FindObjRoot(sh,colonDesc);
        if(tr0==null)  return sh.io.Error("オブジェクトが見つかりません");

        var mi=new CmdMeshes.MeshInfo(tr,tr0);
        if(mi.count==0) return sh.io.Error("メッシュが見つかりません");
        if(val==null){
            for(int i=0; i<mi.count; i++){
                sh.io.Print($"{i}");
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
                if((err=CmdMeshes.SetVectorProp(mi.material[n],kv[0],kv[1]))!="") return sh.io.Error(err);
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
    private static int ObjParamBakeMesh(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return sh.io.Error("オブジェクト名を指定してください");
        int clonetype=0;
        string[] lr=ParseUtil.LeftAndRight(val,',');
        if(lr[1]!=""){
            if(lr[1]!="1"&&lr[1]!="0"&&lr[1]!="2") return sh.io.Error("0,1,2のいずれかを指定してください");
            clonetype=lr[1][0]-'0';
        }

        if(!UTIL.ValidObjName(lr[0])) return sh.io.Error("その名前は使用できません");
        Transform pftr=ObjUtil.GetPhotoPrefabTr(sh);
        if(pftr==null) return sh.io.Error("オブジェクト作成に失敗しました");
        if(ObjUtil.FindObj(sh,lr[0])!=null||LightUtil.FindLight(sh,lr[0])!=null) return sh.io.Error("その名前は既に使われています");
        GameObject go;
        SkinnedMeshRenderer[] smra=tr.GetComponentsInChildren<SkinnedMeshRenderer>();
        if(smra==null || smra.Length==0){ // MeshFilterならcloneと同じ
            go=ObjUtil.CloneObject(lr[0],tr,pftr);
            if(go==null) return sh.io.Error("オブジェクト作成に失敗しました");
            ObjUtil.objDic[go.transform.name]=go.transform;
            return 1;
        }
        go=new GameObject(lr[0]);
        go.transform.SetParent(pftr);
        go.transform.position=tr.position;
        go.transform.rotation=tr.rotation;
        go.transform.localScale=tr.localScale;
        for(int i=0; i<smra.Length; i++) CreateBakeMeshObj(smra[i].name,smra[i],go.transform,clonetype);
        var oi=ObjInfo.AddObjInfo(go.transform,"");
        ObjUtil.objDic[go.transform.name]=go.transform;
        oi.data.Backup();
        oi.data.OwnMesh();
        return 1;
    }
    private static int ObjParamVisible(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            var r=tr.GetComponentInChildren<Renderer>();
            if(r!=null) sh.io.Print((r.enabled)?"1":"0");
            return 0;
        }
        if(val!="1"&&val!="0") return sh.io.Error("1か0で指定してください");
        var ra=tr.GetComponentsInChildren<Renderer>();
        for(int i=0; i<ra.Length; i++) if(ra[i]!=null) ra[i].enabled=val=="1";
        return 1;
    }
    private static int ObjParamActive(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            sh.io.Print(tr.gameObject.activeSelf?"1":"0");
            return 0;
        }
        if(val!="1"&&val!="0") return sh.io.Error("1か0で指定してください");
        tr.gameObject.SetActive(val=="1");
        return 1;
    }
    private static GameObject CreateBakeMeshObj(string name,SkinnedMeshRenderer smr,Transform parent,int clonetype=0){
        Mesh mesh=new Mesh();
        smr.BakeMesh(mesh);
        var go=new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.localScale=smr.transform.localScale;
        go.transform.position=smr.transform.position;
        go.transform.rotation=smr.transform.rotation;
        MeshFilter mf=go.AddComponent<MeshFilter>();
        mf.sharedMesh=mesh;
        MeshRenderer mr=go.AddComponent<MeshRenderer>();
        var ma=smr.sharedMaterials;
        if(clonetype==1) for(int i=0; i<ma.Length; i++){
            var m=new Material(ma[i]);
            TextureUtil.CloneAllTexture(m);
            ma[i]=m;
        }
        if(clonetype!=0) mr.materials=ma; else mr.sharedMaterials=ma;
        return go;
    }
    private static int ObjParamLookAt(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ return 0; }
        Vector3 up=Vector3.up;
        string[] sa=val.Split(ParseUtil.comma);
        if(sa[0].IndexOf(':')>=0){
            if(sa.Length!=1&&sa.Length!=4) return sh.io.Error("書式が不正です");
            var cd=new ParseUtil.ColonDesc(sa[0]);
            var tgt=ObjUtil.FindObj(sh,cd);
            if(tgt==null) return sh.io.Error("対象が見つかりません");
            if(sa.Length==4){
                if(!float.TryParse(sa[1],out float x)
                 ||!float.TryParse(sa[2],out float y)
                 ||!float.TryParse(sa[3],out float z)) return sh.io.Error("数値が不正です");
                up=new Vector3(x,y,z);
            }
            tr.transform.LookAt(tgt.position,up);
        }else{
            if(sa.Length!=3&&sa.Length!=6) return sh.io.Error("書式が不正です");
            if(!float.TryParse(sa[0],out float x)
             ||!float.TryParse(sa[1],out float y)
             ||!float.TryParse(sa[2],out float z)) return sh.io.Error("数値が不正です");
            var tgt=new Vector3(x,y,z);
            if(sa.Length==6){
                if(!float.TryParse(sa[3],out x)
                 ||!float.TryParse(sa[4],out y)
                 ||!float.TryParse(sa[5],out z)) return sh.io.Error("数値が不正です");
                up=new Vector3(x,y,z);
            }
            tr.transform.LookAt(tgt,up);
        }
        return 1;
    }
    private static int ObjParamLookAtLocal(ComShInterpreter sh,Transform tr,string val){
        if(val==null){ return 0; }
        float[] xyz=ParseUtil.Xyz1(val);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        Quaternion q=Quaternion.FromToRotation(Vector3.forward,new Vector3(xyz[0],xyz[1],xyz[2]));
        tr.transform.localRotation*=q;
        return 1;
    }
    private static int ObjParamLocIK(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIKSub(sh,tr,val,Vector3.forward);
    }
    private static int ObjParamLocIKX(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIKSub(sh,tr,val,Vector3.right);
    }
    private static int ObjParamLocIKY(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIKSub(sh,tr,val,Vector3.up);
    }
    private static int ObjParamLocIKMinus(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIKSub(sh,tr,val,-Vector3.forward);
    }
    private static int ObjParamLocIKMinusX(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIKSub(sh,tr,val,-Vector3.right);
    }
    private static int ObjParamLocIKMinusY(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIKSub(sh,tr,val,-Vector3.up);
    }
    private static int ObjParamLocIK3(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIK3Sub(sh,tr,val,Vector3.forward);
    }
    private static int ObjParamLocIK3X(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIK3Sub(sh,tr,val,Vector3.right);
    }
    private static int ObjParamLocIK3Y(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIK3Sub(sh,tr,val,Vector3.up);
    }
    private static int ObjParamLocIK3Minus(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIK3Sub(sh,tr,val,-Vector3.forward);
    }
    private static int ObjParamLocIK3MinusX(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIK3Sub(sh,tr,val,-Vector3.right);
    }
    private static int ObjParamLocIK3MinusY(ComShInterpreter sh,Transform tr,string val){
        return ObjParamLocIK3Sub(sh,tr,val,-Vector3.up);
    }
    private static int ObjParamLocIKSub(ComShInterpreter sh,Transform tr,string val,Vector3 zaxis){
        if(val==null) return 0;
        float[] xyz=ParseUtil.Position(val);
        if(xyz==null) return sh.io.Error("座標の指定が不正です");

        Transform p=tr;
        if(p.name=="Bip01" || p.name=="ManBip") return sh.io.Error("親ボーンが足りません");
        p=p.parent;
        if(p==null || p.name=="Bip01" || p.name=="ManBip") return sh.io.Error("親ボーンが足りません");
        p=p.parent;
        if(p==null) return sh.io.Error("親ボーンが足りません");

        int ret=LocIK(new Vector3(xyz[0],xyz[1],xyz[2]),tr.parent.parent,tr.parent,tr,zaxis);
        if(ret<0) return sh.io.Error("ボーンの長さが0です");
        return 1;
    }
    private static int ObjParamLocIK3Sub(ComShInterpreter sh,Transform tr,string val,Vector3 zaxis){
        if(val==null) return 0;
        float[] xyzs=new float[4];
        int n=ParseUtil.XyzSub(val,xyzs);
        if(n==3) xyzs[3]=0; 
        else if(n==4){
            if(xyzs[3]<-1 || xyzs[3]>1) return sh.io.Error("値の指定が不正です");
        }else{
            var sa=ParseUtil.LeftAndRight(val,',');
            if(sa[1]!=""){
                if(!float.TryParse(sa[1],out float f)) return sh.io.Error("座標の指定が不正です");
                xyzs[3]=f;
            }else xyzs[3]=0;
            var objtr=ObjUtil.FindObj(sh,sa[0].Split(ParseUtil.colon));
            if(objtr!=null){
                Vector3 v=objtr.position;
                xyzs[0]=v.x; xyzs[1]=v.y; xyzs[2]=v.z;
            } else return sh.io.Error("座標の指定が不正です");
        }

        Transform p=tr;
        if(p.name=="Bip01" || p.name=="ManBip") return sh.io.Error("親ボーンが足りません");
        p=p.parent;
        if(p==null || p.name=="Bip01" || p.name=="ManBip") return sh.io.Error("親ボーンが足りません");
        p=p.parent;
        if(p==null || p.name=="Bip01" || p.name=="ManBip") return sh.io.Error("親ボーンが足りません");
        p=p.parent;
        if(p==null) return sh.io.Error("親ボーンが足りません");

        int ret=LocIK3(new Vector3(xyzs[0],xyzs[1],xyzs[2]),
            tr.parent.parent.parent,tr.parent.parent,tr.parent,tr,xyzs[3],zaxis);
        if(ret<0) return sh.io.Error("ボーンの長さが0です");
        return 1;
    }
    private static int LocIK(Vector3 t,Transform p0,Transform p1,Transform p2,Vector3 zaxis){
        float co=LowOfCosine(t,p0.position,p1.position,p2.position);
        if(float.IsNaN(co)) return -1;
        p1.localRotation=Quaternion.AngleAxis(Mathf.Acos(co)*Mathf.Rad2Deg,zaxis);
        var w2l=p0.worldToLocalMatrix;
        var lpt=w2l.MultiplyPoint3x4(t);
        var lpw=w2l.MultiplyPoint3x4(p2.position);
        p0.localRotation=p0.localRotation*Quaternion.FromToRotation(lpw.normalized,lpt.normalized);
        return 1;
    }
    private static float LowOfCosine(Vector3 t,Vector3 p0,Vector3 p1,Vector3 p2){
        float a=(t -p0).sqrMagnitude;
        if(Mathf.Approximately(a,0)) return float.NaN;
        float b=(p1-p0).sqrMagnitude;
        if(Mathf.Approximately(b,0)) return float.NaN;
        float c=(p2-p1).sqrMagnitude;
        if(Mathf.Approximately(c,0)) return float.NaN;
        float co=(a-b-c)/(-2*Mathf.Sqrt(b*c));
        return -co;
    }
    private static int LocIK3(Vector3 t,Transform p0,Transform p1,Transform p2,Transform p3,float s,Vector3 axis){
        float sp=Mathf.Abs(s);
        var axis2=(s<0)?-axis:axis;
        // まず p0,p1,p3*s+p2*(1-s) で２ボーンIK
        float l23=(p3.position-p2.position).magnitude;
        float l12=(p2.position-p1.position).magnitude;
        float l=l23*(1-sp)+l12,m=l/l12;
        var v=p2.position*m+p1.position*(1-m);
        float co=LowOfCosine(t,p0.position,p1.position,v);
        if(float.IsNaN(co)) return -1;
        p2.localRotation=Quaternion.identity;
        p1.localRotation=Quaternion.AngleAxis(Mathf.Acos(co)*Mathf.Rad2Deg,axis);
        var w2l=p0.worldToLocalMatrix;
        var lpt=w2l.MultiplyPoint3x4(t);
        var lpw=w2l.MultiplyPoint3x4(p2.position);
        p0.localRotation=p0.localRotation*Quaternion.FromToRotation(lpw.normalized,lpt.normalized);
        // 次いで p1,p2,p3 で２ボーンIK
        return LocIK(t,p1,p2,p3,axis2);
    }
    private static int ObjParamIK(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return 0;
        float[] xyzn=new float[4];
        int n=ParseUtil.XyzSub(val,xyzn);
        if(n!=4) return sh.io.Error("書式が不正です");
        int bn=(int)xyzn[3];
        if(bn<1) return sh.io.Error("ボーン数が不正です");

        var goal=new Vector3(xyzn[0],xyzn[1],xyzn[2]);
        if(Vector3.Distance(tr.position,goal)<0.003) return 1;

        var ja=new List<Transform>(10);
        Transform p=tr;
        for(int i=0; i<bn+1; i++){
            ja.Add(p);
            p=p.parent;
            if(p==null || p.name=="Bip01" || p.name=="ManBip") return sh.io.Error("親ボーンが足りません");
        }

        var pa=new List<Vector3>();
        for(int i=0; i<ja.Count; i++) pa.Add(ja[i].position);

        var la=new List<float>();
        for(int i=0; i<pa.Count-1; i++) la.Add(Vector3.Distance(pa[i],pa[i+1]));

        var root=pa[pa.Count-1];
        float prev=0;
        for (int r=0; r<10; r++){
            float d=(goal-pa[0]).sqrMagnitude;
            float mv=Mathf.Abs(d-prev);
            prev=d;
            if (d<0.000009||mv<0.000009) break;
            pa[0]=goal;
            for(int i=1; i<pa.Count; i++){
                pa[i]=pa[i-1]+(pa[i]-pa[i-1]).normalized*la[i-1];
            }

            pa[pa.Count-1]=root;
            for(int i=pa.Count-2; i>=0; i--){
                pa[i]=pa[i+1]+(pa[i]-pa[i+1]).normalized*la[i];
            }
        }
        for (int i=ja.Count-1; i>=1; i--){
            var o=ja[i].position;
            var q=RYRX(ja[i],ja[i-1].position-o,pa[i-1]-o);
            ja[i].rotation=q*ja[i].rotation;
        }
        return 1;
    }
    private static Quaternion RYRX(Transform tr,Vector3 from,Vector3 to){
        Vector3 fn=from.normalized, tn=to.normalized;
        var normal=Vector3.Cross(fn,tn).normalized;
        if(normal==Vector3.zero) return Quaternion.identity;
        Vector3 p;
        Quaternion q;

        Vector3 lp=tr.InverseTransformPoint((tr.parent!=null)?tr.parent.position:Vector3.zero)*-1;
        int fwno=UTIL.MaxIdx(Mathf.Abs(lp.z),Mathf.Abs(lp.x),Mathf.Abs(lp.y));
        if(fwno==0){
            p=RotWaypoint(new Vector3[]{tr.right,tr.up,tr.forward},normal,fn,tn);
        }else if(fwno==1){
            p=RotWaypoint(new Vector3[]{tr.forward,tr.up,tr.right},normal,fn,tn);
        }else{
            p=RotWaypoint(new Vector3[]{tr.forward,tr.right,tr.up},normal,fn,tn);
        }
        q=Quaternion.FromToRotation(fn,p);
        return q*Quaternion.FromToRotation(p,tn);
    }
    private static Vector3 RotWaypoint(Vector3[] ax,Vector3 normal,Vector3 from,Vector3 to){
        Vector3 a=ax[UTIL.MaxIdx(
            Mathf.Abs(Vector3.Dot(normal,ax[0])),
            Mathf.Abs(Vector3.Dot(normal,ax[1])),
            Mathf.Abs(Vector3.Dot(normal,ax[2]))
        )];
        Vector3 vf=Vector3.Dot(from,a)*a, vt=Vector3.Dot(to,a)*a;
        return (to-vt+vf).normalized;
    }
    private static int ObjParamShadow(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            Renderer r=tr.GetComponentInChildren<Renderer>();
            char c0=(char)('0'+(int)r.shadowCastingMode);
            char c1=r.receiveShadows?'1':'0';
            sh.io.Print(c0).Print(',').Print(c1);
            return 0;
        }
        var sa=val.Split(ParseUtil.comma);
        if(sa==null || sa.Length==0 || sa.Length>2 || sa[0]=="") return sh.io.Error("書式が不正です");

        UnityEngine.Rendering.ShadowCastingMode cast;
        switch(sa[0]){
        case "0": cast=UnityEngine.Rendering.ShadowCastingMode.Off; break;
        case "1": cast=UnityEngine.Rendering.ShadowCastingMode.On; break;
        case "2": cast=UnityEngine.Rendering.ShadowCastingMode.TwoSided; break;
        case "3": cast=UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly; break;
        default: return sh.io.Error("数値が不正です");
        }

        bool rcv=false;
        if(sa.Length==2){
            if(sa[1]=="0") rcv=false;
            else if(sa[1]=="1") rcv=true;
            else return sh.io.Error("数値が不正です");
        }
        
        var ra=tr.GetComponentsInChildren<Renderer>();
        if(ra==null || ra.Length==0) return 1;
        for(int i=0; i<ra.Length; i++){
            ra[i].shadowCastingMode=cast;
            if(sa.Length==2) ra[i].receiveShadows=rcv;
        }
        return 1;
    }
    private static PhysicMaterial defaultPM=null;
    private static PhysicMaterial GetDefaultPM(){
        if(defaultPM==null){
            defaultPM=new PhysicMaterial();
            defaultPM.bounceCombine=PhysicMaterialCombine.Average;
            defaultPM.bounciness=0.3f;
            defaultPM.frictionCombine=PhysicMaterialCombine.Average;
            defaultPM.dynamicFriction=0.4f;
            defaultPM.staticFriction=0.6f;
        }
        return defaultPM;
    }
    private static int ObjParamCollider(ComShInterpreter sh,Transform tr,string val){
        if(val==null){
            ObjUtil.ForeachComponents<Collider>(tr,(c)=>{
                var t=c.GetType();
                if(t==typeof(SphereCollider)){
                    var sp=(SphereCollider)c;
                    sh.io.PrintJoinLn(",","sphere",sh.fmt.FVal(sp.radius),sp.isTrigger?"1":"0");
                }else if(t==typeof(CapsuleCollider)){
                    var cap=(CapsuleCollider)c;
                    sh.io.PrintJoin(",","capsule",sh.fmt.FVal(cap.radius),sh.fmt.FVal(cap.height))
                      .Print(',').PrintLn(cap.isTrigger?",1":",0");
                }else if(t==typeof(BoxCollider)){
                    var box=(BoxCollider)c;
                    sh.io.PrintJoinLn(",","box",sh.fmt.FMul(box.size),box.isTrigger?"1":"0");
                }else if(t==typeof(MeshCollider)){
                    var mesh=(MeshCollider)c;
                    sh.io.PrintJoinLn(",","mesh",mesh.convex?"1":"0",mesh.isTrigger?"1":"0");
                }else{
                    var col=(Collider)c;
                    sh.io.PrintJoinLn(",",t.FullName,col.isTrigger?"1":"0");
                }
                return 0;
            });
            return 0;
        }
        if(val=="del"){
            ObjUtil.ClearComponent<Collider>(tr);
            return 1;
        }
        var sa=val.Split(ParseUtil.comma);
        var type=sa[0];
        switch(type){
        case "sphere":
            var sp=tr.GetComponent<SphereCollider>();
            if(sp==null){
                sp=tr.gameObject.AddComponent<SphereCollider>();
                sp.sharedMaterial=GetDefaultPM();
            }
            if(sa.Length==2||sa.Length==3){
                if(!float.TryParse(sa[1],out float r)||r<0) return sh.io.Error("半径が不正です");
                sp.radius=r;
                if(sa.Length==3) sp.isTrigger=(sa[2]=="1");
            }else return sh.io.Error("半径を指定してください");
            break;
        case "capsule":
            var cap=tr.GetComponent<CapsuleCollider>();
            if(cap==null){
                cap=tr.gameObject.AddComponent<CapsuleCollider>();
                cap.sharedMaterial=GetDefaultPM();
            }
            if(sa.Length==3||sa.Length==4){
                if(!float.TryParse(sa[1],out float r)||r<0) return sh.io.Error("半径が不正です"); 
                if(!float.TryParse(sa[2],out float h)||h<0) return sh.io.Error("高さが不正です");
                cap.radius=r;
                cap.height=h;
                cap.direction=1; // y軸
                if(sa.Length==4) cap.isTrigger=(sa[3]=="1");
            }else return sh.io.Error("半径,高さを指定してください");
            break;
        case "box":
            var box=tr.GetComponent<BoxCollider>();
            if(box==null){
                box=tr.gameObject.AddComponent<BoxCollider>();
                box.sharedMaterial=GetDefaultPM();
            }
            if(sa.Length==4||sa.Length==5){
                if( (!float.TryParse(sa[1],out float x)||x<0) ||
                    (!float.TryParse(sa[2],out float y)||y<0) ||
                    (!float.TryParse(sa[3],out float z)||z<0) ) return sh.io.Error("サイズの指定が不正です");
                box.size=new Vector3(x,y,z);
                if(sa.Length==5) box.isTrigger=(sa[4]=="1");
            }else return sh.io.Error("サイズ(x,y,z)を指定してください");
            break;
        case "mesh":
            var mco=tr.GetComponent<MeshCollider>();
            if(mco==null){
                Mesh mesh=null;
                SkinnedMeshRenderer smr=null;
                var oi=ObjInfo.GetObjInfo(tr);
                if(oi==null) smr=tr.GetComponentInChildren<SkinnedMeshRenderer>();
                else smr=oi.data.FindComponent<SkinnedMeshRenderer>();
                if(smr==null){
                    var mf=tr.GetComponentInChildren<MeshFilter>();
                    if(mf==null) return sh.io.Error("メッシュがありません");
                    mesh=mf.sharedMesh;
                    mco=mf.transform.gameObject.AddComponent<MeshCollider>();
                }else{
                    mesh=smr.sharedMesh;
                    mco=smr.transform.gameObject.AddComponent<MeshCollider>();
                }
                mco.convex=true;
                mco.inflateMesh=true;
                mco.sharedMesh=mesh;
                mco.sharedMaterial=GetDefaultPM();
            }
            if(sa.Length==2){
                if(sa[1]!="0"&&sa[1]!="1") return sh.io.Error("1か0を指定してください");
                mco.convex=(sa[1]=="1");
            }
            break;
        default:
            return sh.io.Error("コライダー形状が不正です");
        }
        return 1;
    }

    private static int ObjParamRigidbody(ComShInterpreter sh,Transform tr,string val){
        var rb=tr.GetComponent<Rigidbody>();
        if(val==null){
            if(rb!=null){
                sh.io.PrintJoinLn(",",sh.fmt.FVal(rb.mass),sh.fmt.FVal(rb.drag),
                    sh.fmt.FVal(rb.angularDrag),rb.useGravity?"1":"0",rb.isKinematic?"1":"0");
                sh.io.PrintLn2("centerOfMass:",sh.fmt.FPos(rb.centerOfMass));
                sh.io.PrintLn2("inertiaTensor:",sh.fmt.FPos(rb.inertiaTensor));
                sh.io.PrintLn2("inertiaTensorRotation:",sh.fmt.FEuler(rb.inertiaTensorRotation.eulerAngles));
                sh.io.PrintLn2("velocity:",sh.fmt.FPos(rb.velocity));
                sh.io.PrintLn2("angulerVelocity:",sh.fmt.FEuler(rb.angularVelocity));
            }
            return 0;
        }
        if(val=="del"){
            var ja=tr.GetComponents<Joint>();
            for(int i=0; i<ja.Length; i++) UnityEngine.Object.Destroy(ja[i]);
            if(rb!=null) UnityEngine.Object.Destroy(rb);
            return 1;
        }
        if(val.IndexOf('=')>=0){
            if(rb==null){
                rb=tr.gameObject.AddComponent<Rigidbody>();
                rb.collisionDetectionMode=CollisionDetectionMode.Discrete;
                rb.maxAngularVelocity=10;
                rb.maxDepenetrationVelocity=1;
                rb.sleepThreshold=0.001f;
                rb.interpolation=RigidbodyInterpolation.None;
            }
            var sa=val.Split('\n');
            for(int i=0; i<sa.Length; i++) if(sa[i]!=""){
                var lr=ParseUtil.LeftAndRight(sa[i],'=');
                if(lr[0]=="" || lr[1]=="") return sh.io.Error("書式が不正です");
                float f; float[] fa;
                switch(lr[0]){
                case "mass":
                    if(!float.TryParse(lr[1],out f)||f<0) return sh.io.Error("数値が不正です");
                    rb.mass=f;
                    break;
                case "drag":
                    fa=ParseUtil.FloatArr(lr[1]);
                    if(fa==null||fa.Length!=2) return sh.io.Error("数値が不正です");
                    rb.drag=fa[1];
                    rb.angularDrag=fa[2];
                    break;
                case "gravity":
                    if(lr[1]=="0") rb.useGravity=false;
                    else if(lr[1]=="1") rb.useGravity=true;
                    else return sh.io.Error("数値が不正です");
                    break;
                case "kinematic":
                    if(lr[1]=="0") rb.isKinematic=false;
                    else if(lr[1]=="1") rb.isKinematic=true;
                    else return sh.io.Error("数値が不正です");
                    break;
                case "maxomega":
                    if(!float.TryParse(lr[1],out f)||f<0) return sh.io.Error("数値が不正です");
                    rb.maxAngularVelocity=f;
                    break;
                case "interpolation":
                    if(lr[1]=="0") rb.interpolation=RigidbodyInterpolation.None;
                    else if(lr[1]=="1") rb.interpolation=RigidbodyInterpolation.Extrapolate;
                    else if(lr[1]=="2") rb.interpolation=RigidbodyInterpolation.Interpolate;
                    else return sh.io.Error("数値が不正です");
                    break;
                case "kanten":
                    if(lr[1]=="auto"){
                        rb.ResetCenterOfMass();
                        rb.ResetInertiaTensor();
                    }else{
                        fa=ParseUtil.FloatArr(lr[1]);
                        if(fa==null||fa.Length!=6) return sh.io.Error("座標が不正です");
                        rb.inertiaTensor=new Vector3(fa[0],fa[1],fa[2]);
                        rb.inertiaTensorRotation=Quaternion.Euler(fa[3],fa[4],fa[5]);
                    }
                    break;
                case "freeze":
                    if(lr[1].Length!=6) return sh.io.Error("値が不正です");
                    int c; for(c=0; c<6; c++) if(lr[1][c]!='x'&&lr[1][c]!='o') break;
                    if(c<6) return sh.io.Error("値が不正です");
                    RigidbodyConstraints frz=RigidbodyConstraints.None;
                    if(lr[1][0]=='o') frz|=RigidbodyConstraints.FreezePositionX;
                    if(lr[1][1]=='o') frz|=RigidbodyConstraints.FreezePositionY;
                    if(lr[1][2]=='o') frz|=RigidbodyConstraints.FreezePositionZ;
                    if(lr[1][3]=='o') frz|=RigidbodyConstraints.FreezeRotationX;
                    if(lr[1][4]=='o') frz|=RigidbodyConstraints.FreezeRotationY;
                    if(lr[1][5]=='o') frz|=RigidbodyConstraints.FreezeRotationZ;
                    rb.constraints=frz;
                    break;
                default:
                    return sh.io.Error("パラメータが不正です");
                }
            }
            rb.ResetCenterOfMass();
            rb.ResetInertiaTensor();
        }else{
            var fa=ParseUtil.FloatArr(val);
            if(fa==null||(fa.Length!=4 && fa.Length!=5)) return sh.io.Error("数値が不正です");
            if(rb==null){
                rb=tr.gameObject.AddComponent<Rigidbody>();
                rb.collisionDetectionMode=CollisionDetectionMode.Discrete;
                rb.maxAngularVelocity=10;
                rb.maxDepenetrationVelocity=1;
                rb.sleepThreshold=0.001f;
                rb.interpolation=RigidbodyInterpolation.None;
            }
            rb.mass=fa[0];
            rb.drag=fa[1];
            rb.angularDrag=fa[2];
            rb.useGravity=(fa[3]==1);
            if(fa.Length>=5) rb.isKinematic=(fa[4]==1);
            rb.ResetCenterOfMass();
            rb.ResetInertiaTensor();
        }
        return 1;
    }
    private static int ObjParamAddForce(ComShInterpreter sh,Transform tr,string val){
        return ObjParamAddForceOrTorque(sh,tr,val,0);
    }
    private static int ObjParamAddTorque(ComShInterpreter sh,Transform tr,string val){
        return ObjParamAddForceOrTorque(sh,tr,val,1);
    }
    private static int ObjParamAddForceOrTorque(ComShInterpreter sh,Transform tr,string val,int ft){
        if(val==null) return 0;
        var rb=tr.GetComponent<Rigidbody>();
        if(rb==null) return sh.io.Error("Rigidbodyがありません");
        var fa=ParseUtil.FloatArr(val);
        if(fa==null||fa.Length!=4
         || fa[3]<0||fa[3]>1) return sh.io.Error("数値が不正です");
        ForceMode mode=(fa[3]==1)?ForceMode.Impulse:ForceMode.Force;
        if(ft==0) rb.AddForce(fa[0],fa[1],fa[2],mode);
        else rb.AddTorque(fa[0],fa[1],fa[2],mode);
        return 1;
    }
    private static float MayInf(float v){ return (v>3e38)?-1:v; }
    private static Regex numcolon=new Regex(@"^\d+\.",RegexOptions.Compiled);
    private static int ObjParamHingeJoint(ComShInterpreter sh,Transform tr,string val){
        var rb=tr.GetComponent<Rigidbody>();
        if(rb==null) return sh.io.Error("Rigidbodyがありません");
        HingeJoint[] jta;
        HingeJoint jt;
        if(val==null){
            jta=tr.GetComponents<HingeJoint>();
            if(jta!=null) for(int i=0; i<jta.Length; i++){
                jt=jta[i];
                if(jt.connectedBody!=null) sh.io.PrintLn2($"{i}.parent=",jt.connectedBody.transform.name);
                sh.io.PrintLn2($"{i}.anchor=",sh.fmt.FPos(jt.anchor));
                sh.io.PrintLn2($"{i}.axis=",sh.fmt.FPos(jt.axis));
                if(jt.useLimits) sh.io.PrintLn2($"{i}.limit=",sh.fmt.FPos(jt.limits.min,jt.limits.max,jt.limits.bounciness));
                if(jt.useSpring) sh.io.PrintLn2($"{i}.spring=",sh.fmt.FPos(MayInf(jt.spring.spring),MayInf(jt.spring.damper),jt.spring.targetPosition));
                if(jt.useMotor) sh.io.PrintLn2($"{i}.motor=",sh.fmt.FXY(jt.motor.force,jt.motor.targetVelocity));
                sh.io.PrintLn2($"{i}.break=",sh.fmt.FXY(jt.breakForce,jt.breakTorque));
                sh.io.PrintLn2($"{i}.hit=",jt.enableCollision?"on":"off");
            }
            return 0;
        }
        int n=0;
        string prms=val;
        var mc=numcolon.Match(val);
        if(mc.Success){
            string num=mc.Value;
            num.Substring(0,num.Length-1);
            if(!int.TryParse(num.Substring(0,num.Length-1),out n)||n<0) return sh.io.Error("数値が不正です");
            prms=val.Substring(mc.Length);
        }
        jta=tr.GetComponents<HingeJoint>();
        if(prms=="del"){
            if(jta==null||jta.Length==0||n>=jta.Length) return 1;
            UnityEngine.Object.Destroy(jta[n]);
            return 1;
        }
        if(jta==null||jta.Length==0||n>=jta.Length){
            jt=tr.gameObject.AddComponent<HingeJoint>();
            jt.breakForce=Mathf.Infinity; jt.breakTorque=Mathf.Infinity;
            jt.autoConfigureConnectedAnchor=true;
            jt.enableCollision=true; jt.enablePreprocessing=false;
            jt.anchor=Vector3.zero; jt.axis=Vector3.forward;
            jt.useLimits=false;
            jt.limits=new JointLimits(){min=0,max=0,contactDistance=0.1f,bounciness=0,bounceMinVelocity=0};
            jt.useSpring=false;
            jt.spring=new JointSpring(){spring=0,damper=0,targetPosition=0};
            jt.useMotor=false;
            jt.motor=new JointMotor(){force=0,targetVelocity=0,freeSpin=true};
        }else jt=jta[n];

        var sa=prms.Split('\n');
        for(int i=0; i<sa.Length; i++) if(sa[i]!=""){
            var lr=ParseUtil.LeftAndRight(sa[i],'=');
            if(lr[0]=="" || lr[1]=="") return sh.io.Error("書式が不正です");
            float[] fa;

            Rigidbody tgtrb=null;
            switch(lr[0]){
            case "parent":
                var tgttr=ObjUtil.FindObj(sh,lr[1].Split(':'));
                if(tgttr!=null) tgtrb=tgttr.GetComponent<Rigidbody>();
                if(tgtrb==null) return sh.io.Error("接続先が存在しません");
                jt.connectedBody=tgtrb;
                jt.anchor=tr.worldToLocalMatrix.MultiplyPoint3x4(tgttr.position);
                break;
            case "anchor":
                fa=ParseUtil.Xyz1(lr[1]);
                if(fa==null) return sh.io.Error("座標が不正です");
                jt.anchor=new Vector3(fa[0],fa[1],fa[2]);
                break;
            case "axis":
                fa=ParseUtil.Xyz1(lr[1]);
                if(fa==null) return sh.io.Error("ベクトルが不正です");
                jt.axis=new Vector3(fa[0],fa[1],fa[2]);
                break;
            case "limit":
                if(lr[1]=="off") jt.useLimits=false; else{
                    fa=ParseUtil.FloatArr(lr[1]);
                    if(fa==null||fa.Length!=3||fa[2]<0||fa[2]>1) return sh.io.Error("数値が不正です");
                    jt.useLimits=true;
                    var jl=jt.limits;
                    jl.min=fa[0]; jl.max=fa[1]; jl.bounciness=fa[2];
                    jt.limits=jl;
                }
                break;
            case "spring":
                if(lr[1]=="off") jt.useSpring=false; else{
                    fa=ParseUtil.FloatArr(lr[1]);
                    if(fa==null||fa.Length!=3) return sh.io.Error("数値が不正です");
                    jt.useSpring=true;
                    var js=jt.spring;
                    js.spring=(fa[0]<0)?Mathf.Infinity:fa[0];
                    js.damper=(fa[1]<0)?Mathf.Infinity:fa[1];
                    js.targetPosition=fa[2];
                    jt.spring=js;
                }
                break;
            case "motor":
                if(lr[1]=="off") jt.useMotor=false; else{
                    fa=ParseUtil.FloatArr(lr[1]);
                    if(fa==null||fa.Length!=2) return sh.io.Error("数値が不正です");
                    jt.useMotor=true;
                    var jm=jt.motor;
                    jm.force=fa[0]; jm.targetVelocity=fa[1];
                    jt.motor=jm;
                }
                break;
            case "break":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=2) return sh.io.Error("数値が不正です");
                jt.breakForce=fa[0]; jt.breakTorque=fa[1];
                break;
            case "hit":
                int sw=ParseUtil.OnOff(lr[1]);
                if(sw<0) return sh.io.Error("値が不正です");
                jt.enableCollision=(sw==1);
                break;
            default:
                return sh.io.Error("プロパティ名が不正です");
            }
        }
        return 1;
    }

    private static int ObjParamBallJoint(ComShInterpreter sh,Transform tr,string val){
        var rb=tr.GetComponent<Rigidbody>();
        if(rb==null) return sh.io.Error("Rigidbodyがありません");
        CharacterJoint[] jta;
        CharacterJoint jt;
        if(val==null){
            jta=tr.GetComponents<CharacterJoint>();
            if(jta!=null) for(int i=0; i<jta.Length; i++){
                jt=jta[i];
                if(jt.connectedBody!=null) sh.io.PrintLn2($"{i}.parent=",jt.connectedBody.transform.name);
                sh.io.PrintLn2($"{i}.anchor=",sh.fmt.FPos(jt.anchor));
                sh.io.PrintLn2($"{i}.twistaxis=",sh.fmt.FPos(jt.axis));
                sh.io.PrintLn2($"{i}.swingtaxis=",sh.fmt.FPos(jt.swingAxis));
                sh.io.PrintLn2($"{i}.twistlimit=",sh.fmt.FPos(jt.lowTwistLimit.limit,jt.highTwistLimit.limit,jt.highTwistLimit.bounciness));
                sh.io.PrintLn2($"{i}.twistspring=",sh.fmt.FXY(MayInf(jt.twistLimitSpring.spring),MayInf(jt.twistLimitSpring.damper)));
                sh.io.PrintLn2($"{i}.swinglimit=",sh.fmt.FPos(jt.swing1Limit.limit,jt.swing2Limit.limit,jt.swing2Limit.bounciness));
                sh.io.PrintLn2($"{i}.swingspring=",sh.fmt.FXY(MayInf(jt.swingLimitSpring.spring),MayInf(jt.swingLimitSpring.damper)));
                sh.io.PrintLn2($"{i}.break=",sh.fmt.FXY(jt.breakForce,jt.breakTorque));
                sh.io.PrintLn2($"{i}.hit=",jt.enableCollision?"on":"off");
            }
            return 0;
        }
        int n=0;
        string prms=val;
        var mc=numcolon.Match(val);
        if(mc.Success){
            string num=mc.Value;
            num.Substring(0,num.Length-1);
            if(!int.TryParse(num.Substring(0,num.Length-1),out n)||n<0) return sh.io.Error("数値が不正です");
            prms=val.Substring(mc.Length);
        }
        jta=tr.GetComponents<CharacterJoint>();
        if(prms=="del"){
            if(jta==null||jta.Length==0||n>=jta.Length) return 1;
            UnityEngine.Object.Destroy(jta[n]);
            return 1;
        }
        if(jta==null||jta.Length==0||n>=jta.Length){
            jt=tr.gameObject.AddComponent<CharacterJoint>();
            jt.breakForce=Mathf.Infinity; jt.breakTorque=Mathf.Infinity;
            jt.autoConfigureConnectedAnchor=true;
            jt.enableCollision=true; jt.enablePreprocessing=false;
            jt.anchor=Vector3.zero; jt.axis=Vector3.up; jt.swingAxis=Vector3.right;
            jt.lowTwistLimit=jt.highTwistLimit=new SoftJointLimit(){limit=0,bounciness=0,contactDistance=2};
            jt.swing1Limit=jt.swing2Limit=new SoftJointLimit(){limit=180,bounciness=0,contactDistance=2};
            jt.twistLimitSpring=jt.swingLimitSpring=new SoftJointLimitSpring(){spring=0,damper=0};
        }else jt=jta[n];

        var sa=prms.Split('\n');
        for(int i=0; i<sa.Length; i++) if(sa[i]!="") {
            var lr=ParseUtil.LeftAndRight(sa[i],'=');
            if(lr[0]=="" || lr[1]=="") return sh.io.Error("書式が不正です");
            float[] fa;

            Rigidbody tgtrb=null;
            SoftJointLimit jl;
            SoftJointLimitSpring jls;
            switch(lr[0]){
            case "parent":
                var tgttr=ObjUtil.FindObj(sh,lr[1].Split(':'));
                if(tgttr!=null) tgtrb=tgttr.GetComponent<Rigidbody>();
                if(tgtrb==null) return sh.io.Error("接続先が存在しません");
                jt.connectedBody=tgtrb;
                var v=tr.worldToLocalMatrix.MultiplyPoint3x4(tgttr.position);
                jt.anchor=v;
                jt.axis=v.normalized;
                break;
            case "anchor":
                fa=ParseUtil.Xyz1(lr[1]);
                if(fa==null) return sh.io.Error("座標が不正です");
                jt.anchor=new Vector3(fa[0],fa[1],fa[2]);
                break;
            case "twistaxis":
                fa=ParseUtil.Xyz1(lr[1]);
                if(fa==null) return sh.io.Error("ベクトルが不正です");
                jt.axis=new Vector3(fa[0],fa[1],fa[2]);
                break;
            case "swingaxis":
                fa=ParseUtil.Xyz1(lr[1]);
                if(fa==null) return sh.io.Error("ベクトルが不正です");
                jt.swingAxis=new Vector3(fa[0],fa[1],fa[2]);
                break;
            case "twistlimit":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=3) return sh.io.Error("数値が不正です");
                jl=jt.lowTwistLimit;
                jl.limit=fa[0]; jl.bounciness=fa[2];
                jt.lowTwistLimit=jl;
                jl.limit=fa[1];
                jt.highTwistLimit=jl;
                break;
            case "twistspring":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=2) return sh.io.Error("数値が不正です");
                jls=jt.twistLimitSpring;
                jls.spring=(fa[0]<0)?Mathf.Infinity:fa[0]; jls.damper=(fa[1]<0)?Mathf.Infinity:fa[1];
                jt.twistLimitSpring=jls;
                break;
            case "swinglimit":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=2) return sh.io.Error("数値が不正です");
                jl=jt.swing1Limit;
                jl.limit=fa[0]; jl.bounciness=fa[1];
                jt.swing1Limit=jl;
                jt.swing2Limit=jl;
                break;
            case "swingspring":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=2) return sh.io.Error("数値が不正です");
                jls=jt.swingLimitSpring;
                jls.spring=(fa[0]<0)?Mathf.Infinity:fa[0]; jls.damper=(fa[1]<0)?Mathf.Infinity:fa[1];
                jt.swingLimitSpring=jls;
                break;
            case "break":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=2) return sh.io.Error("数値が不正です");
                jt.breakForce=fa[0]; jt.breakTorque=fa[1];
                break;
            case "hit":
                int sw=ParseUtil.OnOff(lr[1]);
                if(sw<0) return sh.io.Error("値が不正です");
                jt.enableCollision=(sw==1);
                break;
            default:
                return sh.io.Error("プロパティ名が不正です");
            }
        }
        return 1;
    }

    private static int ObjParamSpringJoint(ComShInterpreter sh,Transform tr,string val){
        var rb=tr.GetComponent<Rigidbody>();
        if(rb==null) return sh.io.Error("Rigidbodyがありません");
        SpringJoint[] jta;
        SpringJoint jt;
        if(val==null){
            jta=tr.GetComponents<SpringJoint>();
            if(jta!=null) for(int i=0; i<jta.Length; i++){
                jt=jta[i];
                if(jt.connectedBody!=null) sh.io.PrintLn2($"{i}.parent=",jt.connectedBody.transform.name);
                sh.io.PrintLn2($"{i}.anchor=",sh.fmt.FPos(jt.anchor));
                sh.io.PrintLn2($"{i}.axis=",sh.fmt.FPos(jt.axis));
                sh.io.PrintLn2($"{i}.limit=",sh.fmt.FXY(jt.minDistance,jt.maxDistance));
                sh.io.PrintLn2($"{i}.spring=",sh.fmt.FXY(MayInf(jt.spring),MayInf(jt.damper)));
                sh.io.PrintLn2($"{i}.break=",sh.fmt.FXY(jt.breakForce,jt.breakTorque));
                sh.io.PrintLn2($"{i}.hit=",jt.enableCollision?"on":"off");
            }
            return 0;
        }
        int n=0;
        string prms=val;
        var mc=numcolon.Match(val);
        if(mc.Success){
            string num=mc.Value;
            num.Substring(0,num.Length-1);
            if(!int.TryParse(num.Substring(0,num.Length-1),out n)||n<0) return sh.io.Error("数値が不正です");
            prms=val.Substring(mc.Length);
        }
        jta=tr.GetComponents<SpringJoint>();
        if(prms=="del"){
            if(jta==null||jta.Length==0||n>=jta.Length) return 1;
            UnityEngine.Object.Destroy(jta[n]);
            return 1;
        }
        if(jta==null||jta.Length==0||n>=jta.Length){
            jt=tr.gameObject.AddComponent<SpringJoint>();
            jt.breakForce=Mathf.Infinity; jt.breakTorque=Mathf.Infinity;
            jt.autoConfigureConnectedAnchor=true;
            jt.enableCollision=true; jt.enablePreprocessing=false;
            jt.anchor=Vector3.zero; jt.axis=Vector3.up;
            jt.maxDistance=0; jt.minDistance=0; jt.spring=10; jt.damper=5; jt.tolerance=0.05f;
        }else jt=jta[n];

        var sa=prms.Split('\n');
        for(int i=0; i<sa.Length; i++) if(sa[i]!="") {
            var lr=ParseUtil.LeftAndRight(sa[i],'=');
            if(lr[0]=="" || lr[1]=="") return sh.io.Error("書式が不正です");
            float[] fa;

            Rigidbody tgtrb=null;
            switch(lr[0]){
            case "parent":
                var tgttr=ObjUtil.FindObj(sh,lr[1].Split(':'));
                if(tgttr!=null) tgtrb=tgttr.GetComponent<Rigidbody>();
                if(tgtrb==null) return sh.io.Error("接続先が存在しません");
                jt.connectedBody=tgtrb;
                var v=tr.worldToLocalMatrix.MultiplyPoint3x4(tgttr.position);
                jt.anchor=v;
                break;
            case "anchor":
                fa=ParseUtil.Xyz1(lr[1]);
                if(fa==null) return sh.io.Error("座標が不正です");
                jt.anchor=new Vector3(fa[0],fa[1],fa[2]);
                break;
            case "axis":
                fa=ParseUtil.Xyz1(lr[1]);
                if(fa==null) return sh.io.Error("ベクトルが不正です");
                jt.axis=new Vector3(fa[0],fa[1],fa[2]);
                break;
            case "limit":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=2) return sh.io.Error("数値が不正です");
                jt.minDistance=fa[0];
                jt.maxDistance=fa[1];
                jt.tolerance=fa[0]*0.1f;
                break;
            case "spring":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=2) return sh.io.Error("数値が不正です");
                jt.spring=(fa[0]<0)?Mathf.Infinity:fa[0];
                jt.damper=(fa[1]<0)?Mathf.Infinity:fa[1];
                break;
            case "break":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=2) return sh.io.Error("数値が不正です");
                jt.breakForce=fa[0]; jt.breakTorque=fa[1];
                break;
            case "hit":
                int sw=ParseUtil.OnOff(lr[1]);
                if(sw<0) return sh.io.Error("値が不正です");
                jt.enableCollision=(sw==1);
                break;
            default:
                return sh.io.Error("プロパティ名が不正です");
            }
        }
        return 1;
    }
    private static int ObjParamFixJoint(ComShInterpreter sh,Transform tr,string val){
            var rb=tr.GetComponent<Rigidbody>();
        if(rb==null) return sh.io.Error("Rigidbodyがありません");
        FixedJoint[] jta;
        FixedJoint jt;
        if(val==null){
            jta=tr.GetComponents<FixedJoint>();
            if(jta!=null) for(int i=0; i<jta.Length; i++){
                jt=jta[i];
                if(jt.connectedBody!=null) sh.io.PrintLn2($"{i}.parent=",jt.connectedBody.transform.name);
                sh.io.PrintLn2($"{i}.anchor=",sh.fmt.FPos(jt.anchor));
                sh.io.PrintLn2($"{i}.axis=",sh.fmt.FPos(jt.axis));
                sh.io.PrintLn2($"{i}.break=",sh.fmt.FXY(jt.breakForce,jt.breakTorque));
                sh.io.PrintLn2($"{i}.hit=",jt.enableCollision?"on":"off");
            }
            return 0;
        }
        int n=0;
        string prms=val;
        var mc=numcolon.Match(val);
        if(mc.Success){
            string num=mc.Value;
            num.Substring(0,num.Length-1);
            if(!int.TryParse(num.Substring(0,num.Length-1),out n)||n<0) return sh.io.Error("数値が不正です");
            prms=val.Substring(mc.Length);
        }
        jta=tr.GetComponents<FixedJoint>();
        if(prms=="del"){
            if(jta==null||jta.Length==0||n>=jta.Length) return 1;
            UnityEngine.Object.Destroy(jta[n]);
            return 1;
        }
        if(jta==null||jta.Length==0||n>=jta.Length){
            jt=tr.gameObject.AddComponent<FixedJoint>();
            jt.breakForce=Mathf.Infinity; jt.breakTorque=Mathf.Infinity;
            jt.autoConfigureConnectedAnchor=true;
            jt.enableCollision=true; jt.enablePreprocessing=false;
            jt.anchor=Vector3.zero; jt.axis=Vector3.up;
        }else jt=jta[n];

        var sa=prms.Split('\n');
        for(int i=0; i<sa.Length; i++) if(sa[i]!="") {
            var lr=ParseUtil.LeftAndRight(sa[i],'=');
            if(lr[0]=="" || lr[1]=="") return sh.io.Error("書式が不正です");
            float[] fa;

            Rigidbody tgtrb=null;
            switch(lr[0]){
            case "parent":
                var tgttr=ObjUtil.FindObj(sh,lr[1].Split(':'));
                if(tgttr!=null) tgtrb=tgttr.GetComponent<Rigidbody>();
                if(tgtrb==null) return sh.io.Error("接続先が存在しません");
                jt.connectedBody=tgtrb;
                var v=tr.worldToLocalMatrix.MultiplyPoint3x4(tgttr.position);
                jt.anchor=v;
                break;
            case "anchor":
                fa=ParseUtil.Xyz1(lr[1]);
                if(fa==null) return sh.io.Error("座標が不正です");
                jt.anchor=new Vector3(fa[0],fa[1],fa[2]);
                break;
            case "axis":
                fa=ParseUtil.Xyz1(lr[1]);
                if(fa==null) return sh.io.Error("ベクトルが不正です");
                jt.axis=new Vector3(fa[0],fa[1],fa[2]);
                break;
            case "break":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=2) return sh.io.Error("数値が不正です");
                jt.breakForce=fa[0]; jt.breakTorque=fa[1];
                break;
            case "hit":
                int sw=ParseUtil.OnOff(lr[1]);
                if(sw<0) return sh.io.Error("値が不正です");
                jt.enableCollision=(sw==1);
                break;
            default:
                return sh.io.Error("プロパティ名が不正です");
            }
        }
        return 1;
    }
    private static int ObjParamWheel(ComShInterpreter sh,Transform tr,string val){
        WheelCollider wc;
        if(val==null){
            wc=tr.GetComponent<WheelCollider>();
            if(wc!=null){
                sh.io.PrintLn2("torque:",sh.fmt.FVal(wc.motorTorque));
                sh.io.PrintLn2("brake:",sh.fmt.FVal(wc.brakeTorque));
                sh.io.PrintLn2("angle:",sh.fmt.FVal(wc.steerAngle));
                sh.io.PrintLn2("rpm:",sh.fmt.FVal(wc.rpm));
                wc.GetWorldPose(out Vector3 pos,out Quaternion rot);
                sh.io.PrintLn2("position:",sh.fmt.FPos(pos));
                sh.io.PrintLn2("rotation:",sh.fmt.FEuler(rot.eulerAngles));
            }
            return 0;
        }
        int n=0;
        string prms=val;
        var mc=numcolon.Match(val);
        if(mc.Success){
            string num=mc.Value;
            num.Substring(0,num.Length-1);
            if(!int.TryParse(num.Substring(0,num.Length-1),out n)||n<0) return sh.io.Error("数値が不正です");
            prms=val.Substring(mc.Length);
        }
        wc=tr.GetComponent<WheelCollider>();
        if(prms=="del"){
            if(wc==null) return 1;
            UnityEngine.Object.Destroy(wc);
            return 1;
        }
        if(wc==null){
            wc=tr.gameObject.AddComponent<WheelCollider>();
            wc.brakeTorque=0;
            wc.center=Vector3.zero;
            wc.forceAppPointDistance=0.2f;
            wc.forwardFriction=new WheelFrictionCurve(){
                extremumSlip=0.4f,extremumValue=1,
                asymptoteSlip=0.8f,asymptoteValue=0.5f,stiffness=1
            };
            wc.sidewaysFriction=new WheelFrictionCurve(){
                extremumSlip=0.2f,extremumValue=1f,
                asymptoteSlip=0.5f,asymptoteValue=0.75f,stiffness=1
            };
            wc.mass=20;
            wc.motorTorque=0;
            wc.radius=0.5f;
            wc.steerAngle=0;
            wc.suspensionDistance=0.3f;
            wc.suspensionSpring=new JointSpring(){spring=35000,damper=4500,targetPosition=0.5f};
            wc.wheelDampingRate=0.25f;
            wc.enabled=true;
        }

        var sa=prms.Split('\n');
        for(int i=0; i<sa.Length; i++) if(sa[i]!="") {
            var lr=ParseUtil.LeftAndRight(sa[i],'=');
            if(lr[0]=="" || lr[1]=="") return sh.io.Error("書式が不正です");

            float f;
            float[] fa;
            switch(lr[0]){
            case "mass":
                if(!float.TryParse(lr[1],out f)||f<=0) return sh.io.Error("数値が不正です");
                wc.mass=f;
                break;
            case "radius":
                if(!float.TryParse(lr[1],out f)||f<=0) return sh.io.Error("数値が不正です");
                wc.radius=f;
                break;
            case "torque":
                if(!float.TryParse(lr[1],out f)) return sh.io.Error("数値が不正です");
                wc.motorTorque=f;
                break;
            case "brake":
                if(!float.TryParse(lr[1],out f)) return sh.io.Error("数値が不正です");
                wc.brakeTorque=f;
                break;
            case "angle":
                if(!float.TryParse(lr[1],out f)) return sh.io.Error("数値が不正です");
                wc.steerAngle=f;
                break;
            case "height":
                if(!float.TryParse(lr[1],out f)) return sh.io.Error("数値が不正です");
                wc.forceAppPointDistance=f;
                break;
            case "suspension":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=4||fa[0]<0||fa[1]<0||fa[2]<0||fa[3]<=0||fa[3]>=1) return sh.io.Error("数値が不正です");
                wc.suspensionDistance=fa[0];
                wc.suspensionSpring=new JointSpring(){spring=fa[1],damper=fa[2],targetPosition=fa[3]};
                break;
            case "forwardfriction":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=4||fa[0]<=0||fa[0]>1||fa[1]<=0||fa[2]<=0||fa[2]>1||fa[3]<=0) return sh.io.Error("数値が不正です");
                wc.forwardFriction=new WheelFrictionCurve(){
                    extremumSlip=fa[0],extremumValue=fa[1],
                    asymptoteSlip=fa[2],asymptoteValue=fa[3],stiffness=1
                };
                break;
            case "sidefriction":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=4||fa[0]<=0||fa[0]>1||fa[1]<=0||fa[2]<=0||fa[2]>1||fa[3]<=0) return sh.io.Error("数値が不正です");
                wc.sidewaysFriction=new WheelFrictionCurve(){
                    extremumSlip=fa[0],extremumValue=fa[1],
                    asymptoteSlip=fa[2],asymptoteValue=fa[3],stiffness=1
                };
                break;
            default:
                return sh.io.Error("プロパティ名が不正です");
            }
        }
        return 1;
    }

    private static int ObjParamBounce(ComShInterpreter sh,Transform tr,string val){
        var ca=tr.GetComponents<Collider>();
        if(val==null){
            for(int i=0; i<ca.Length; i++) if(ca[i]!=null){
                var mate=ca[i].sharedMaterial;
                if(mate!=null) sh.io.PrintJoinLn(sh.ofs,ca[i].name,ca[i].GetType().FullName,sh.fmt.FVal(mate.bounciness));
                else  sh.io.PrintJoinLn(sh.ofs,ca[i].name,ca[i].GetType().FullName);
            }
            return 0;
        }
        if(ca==null||ca.Length==0) return sh.io.Error("コライダーがありません");
        if(!float.TryParse(val,out float b)||b<0||b>1) return sh.io.Error("数値が不正です");
        for(int i=0; i<ca.Length; i++) if(ca[i]!=null){
            if(ca[i].sharedMaterial==null) ca[i].sharedMaterial=GetDefaultPM();
            ca[i].material.bounciness=b;
        }
        return 1;
    }
    private static int ObjParamFriction(ComShInterpreter sh,Transform tr,string val){
        var ca=tr.GetComponents<Collider>();
        if(val==null){
            for(int i=0; i<ca.Length; i++) if(ca[i]!=null){
                var mate=ca[i].sharedMaterial;
                if(mate!=null) sh.io.PrintJoinLn(sh.ofs,ca[i].name,ca[i].GetType().FullName,
                    sh.fmt.FVal(mate.dynamicFriction),
                    sh.fmt.FVal(mate.staticFriction));
                else sh.io.PrintJoinLn(sh.ofs,ca[i].name,ca[i].GetType().FullName);
            }
            return 0;
        }
        if(ca==null||ca.Length==0) return sh.io.Error("コライダーがありません");
        var sa=val.Split(ParseUtil.comma);
        float df=-1,sf=-1;
        if(sa.Length>0 && (!float.TryParse(sa[0],out df)||df<0||df>1))
            return sh.io.Error("数値が不正です");
        if(sa.Length==2 && (!float.TryParse(sa[1],out sf)||sf<0))
            return sh.io.Error("数値が不正です");
        if(sf<0) sf=df;
        for(int i=0; i<ca.Length; i++) if(ca[i]!=null){
            if(ca[i].sharedMaterial==null) ca[i].sharedMaterial=GetDefaultPM();
            ca[i].material.dynamicFriction=df;
            ca[i].material.staticFriction=sf;
        }
        return 1;
    }
    private static int ObjParamCloth(ComShInterpreter sh,Transform tr,string val){
        var oi=ObjInfo.GetObjInfo(tr);
        SkinnedMeshRenderer smr;
        if(oi!=null) smr=oi.data.FindComponent<SkinnedMeshRenderer>();
        else smr=tr.GetComponentInChildren<SkinnedMeshRenderer>();

        if(smr==null||smr.sharedMesh==null||smr.sharedMesh.vertexCount==0) return sh.io.Error("メッシュがありません");

        var cl=smr.gameObject.GetComponent<Cloth>();
        if(val==null){
            if(cl==null) return 0;
            sh.io.PrintLn2("stiff:",sh.fmt.FXY(cl.bendingStiffness,cl.stretchingStiffness));
            sh.io.PrintLn2("damping:",sh.fmt.FVal(cl.damping));
            sh.io.PrintLn2("friction:",sh.fmt.FVal(cl.friction));
            sh.io.PrintLn2("gravity:",cl.useGravity?"on":"off");
            sh.io.PrintLn2("wave:",sh.fmt.FXY(cl.worldVelocityScale,cl.worldAccelerationScale));
            return 0;
        }

        ClothSkinningCoefficient[] ca;
        if(cl==null){
            cl=smr.gameObject.AddComponent<Cloth>();
            cl.bendingStiffness=1f;
            cl.stretchingStiffness=1f;
            cl.useGravity=true;
            cl.damping=1;
            cl.friction=1;
            cl.useVirtualParticles=1;
            cl.useContinuousCollision=1;
            cl.collisionMassScale=0;
            cl.sleepThreshold=0.1f;
            cl.worldAccelerationScale=0.5f;
            cl.worldVelocityScale=0.5f;
            cl.externalAcceleration=Vector3.zero;
            cl.randomAcceleration=Vector3.zero;
            cl.enabled=false;
            ca=new ClothSkinningCoefficient[cl.vertices.Length];
            for(int ci=0; ci<cl.vertices.Length; ci++)
                ca[ci]=new ClothSkinningCoefficient(){collisionSphereDistance=0,maxDistance=0.2f};
            cl.coefficients=ca;
        }

        if(val=="start"){
            cl.enabled=true;
            return 1;
        }else if(val=="stop"){
            cl.enabled=false;
            return 1;
        }else if(val=="del"){
            UnityEngine.Object.Destroy(cl);
            return 1;
        }else if(val=="vertices"){
            var vta=cl.vertices;
            for(int i=0; i<vta.Length; i++) sh.io.PrintLn(sh.fmt.FPos(vta[i]));
            return 0;
        }else if(val=="coefficients"){
            ca=cl.coefficients;
            for(int i=0; i<ca.Length; i++) sh.io.PrintLn(sh.fmt.FVal(ca[i].maxDistance));
            return 0;
        }

        var sa=val.Split('\n');
        float[] fa;
        float f;
        int sw;
        for(int i=0; i<sa.Length; i++) if(sa[i]!="") {
            var lr=ParseUtil.LeftAndRight(sa[i],'=');
            if(lr[0]=="" || lr[1]=="") return sh.io.Error("書式が不正です");
            switch(lr[0]){
            case "stiff":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=2 || fa[0]<0 || fa[0]>1 || fa[1]<0 || fa[1]>1) return sh.io.Error("数値が不正です");
                cl.bendingStiffness=fa[0];
                cl.stretchingStiffness=fa[1];
                break;
            case "gravity":
                sw=ParseUtil.OnOff(lr[1]);
                if(sw<0) return sh.io.Error(ParseUtil.error);
                cl.useGravity=(sw==1);
                break;
            case "force":
                fa=ParseUtil.Xyz1(lr[1]);
                if(fa==null) return sh.io.Error(ParseUtil.error);
                cl.externalAcceleration=new Vector3(fa[0],fa[1],fa[2]);
                break;
            case "randomforce":
                fa=ParseUtil.Xyz1(lr[1]);
                if(fa==null) return sh.io.Error(ParseUtil.error);
                cl.randomAcceleration=new Vector3(fa[0],fa[1],fa[2]);
                break;
            case "damping":
                if(!float.TryParse(lr[1],out f)||f<0||f>1) return sh.io.Error("数値が不正です");
                cl.damping=f;
                break;
            case "friction":
                if(!float.TryParse(lr[1],out f)||f<0) return sh.io.Error("数値が不正です");
                cl.friction=f;
                break;
            case "collider":
                var cols=lr[1].Split(',');
                if(cols.Length%2>0) return sh.io.Error("要素数が不正です");
                var pa=new ClothSphereColliderPair[cols.Length/2];
                for(int ci=0,pi=0; ci<cols.Length; ci+=2){
                    var t1=ObjUtil.FindObj(sh,cols[ci].Split(':'));
                    var t2=ObjUtil.FindObj(sh,cols[ci+1].Split(':'));
                    if(t1==null||t2==null) return sh.io.Error("コライダが見つかりません");
                    var c1=t1.GetComponent<SphereCollider>();
                    var c2=t2.GetComponent<SphereCollider>();
                    if(c1==null||c2==null) return sh.io.Error("コライダが見つかりません");
                    pa[pi++]=new ClothSphereColliderPair(){first=c1,second=c2};
                }
                cl.sphereColliders=pa;
                break;
            case "limit":
                f=0;
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null) return sh.io.Error(ParseUtil.error);
                if(fa.Length==2) f=fa[1];
                if((fa.Length!=1 && fa.Length!=2)||fa[0]<=0.001||((int)f!=0&&(int)f!=1)) return sh.io.Error("数値が不正です");
                SetCoeff(cl,null,fa[0],(int)f);
                break;
            case "pin":
                var il=ParseUtil.IntList(lr[1]);
                if(il==null||il.Count==0||SetCoeff(cl,il,-1,-1)<0) return sh.io.Error("頂点番号が不正です");
                break;
            case "wave":
                fa=ParseUtil.FloatArr(lr[1]);
                if(fa==null||fa.Length!=2||fa[0]<0||fa[1]<0) return sh.io.Error("数値が不正です");
                cl.worldVelocityScale=fa[0];
                cl.worldAccelerationScale=fa[1];
                break;
            default:
                return sh.io.Error("プロパティ名が不正です");
            }
        }
        return 1;
    }
    private static int SetCoeff(Cloth cl,List<int> pin,float limit,int type){
        var va=cl.vertices;
        var ca=cl.coefficients;
        float f=limit;
        List<int> lst=pin;
        if(lst==null){
            lst=new List<int>();
            for(int i=0; i<ca.Length; i++) if(ca[i].maxDistance==0) lst.Add(i);
        }else{
            for(int i=0; i<lst.Count; i++) if(lst[i]>=va.Length) return -1;
        }
        if(f<0){
            f=float.MinValue;
            for(int i=0; i<ca.Length; i++) if(ca[i].maxDistance>f) f=ca[i].maxDistance;
            if(f<0.001f) f=0.001f;
        }
        if(type==-1){
            float md=0;
            type=0;
            for(int i=0; i<ca.Length; i++) if(ca[i].maxDistance>0){
                if(ca[i].maxDistance!=md){ type=1; break; }
                md=ca[i].maxDistance;
            }
        }
        if(type==0){
            for(int i=0; i<ca.Length; i++){
                if(lst.Contains(i)) ca[i].maxDistance=0;
                else ca[i].maxDistance=f;
            }
        }else if(type==1){
            float dmax=float.MinValue; // 固定点から一番遠い点までの距離
            for(int i=0; i<ca.Length; i++){
                var v=va[i];
                float d=float.MaxValue; // 一番近い固定点までの距離
                for(int j=0; j<lst.Count; j++){
                    float dd=(v-va[lst[j]]).sqrMagnitude;
                    if(dd<d) d=dd;
                }
                if(Mathf.Approximately(d,0)) ca[i].maxDistance=0;
                else ca[i].maxDistance=d;
                if(d>dmax) dmax=d;
            }
            if(Mathf.Approximately(dmax,0)) dmax=0.00001f;
            for(int i=0; i<ca.Length; i++){ // 一番遠い点のときfとなるように正規化
                float md=ca[i].maxDistance;
                if(md>0) ca[i].maxDistance=Mathf.Sqrt(md/dmax)*f;
            }
        }
        cl.coefficients=ca;
        return 0;
    }
    private static int ObjParamReflection(ComShInterpreter sh,Transform tr,string val){
        Renderer r;
        var oi=ObjInfo.GetObjInfo(tr);
        if(oi==null) r=tr.GetComponentInChildren<Renderer>();
        else r=oi.data.FindComponent<Renderer>();
        if(r==null) return sh.io.Error("レンダラが見つかりません");
        if(val==null){
            var rpu=r.reflectionProbeUsage;
            switch(rpu){
            case UnityEngine.Rendering.ReflectionProbeUsage.Off: sh.io.Print("0"); break;
            case UnityEngine.Rendering.ReflectionProbeUsage.Simple: sh.io.Print("1"); break;
            case UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes: sh.io.Print("2"); break;
            case UnityEngine.Rendering.ReflectionProbeUsage.BlendProbesAndSkybox: sh.io.Print("3"); break;
            }
            return 0;
        }
        switch(val){
        case "0":r.reflectionProbeUsage=UnityEngine.Rendering.ReflectionProbeUsage.Off; break;
        case "1":r.reflectionProbeUsage=UnityEngine.Rendering.ReflectionProbeUsage.Simple; break;
        case "2":r.reflectionProbeUsage=UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes; break;
        case "3":r.reflectionProbeUsage=UnityEngine.Rendering.ReflectionProbeUsage.BlendProbesAndSkybox; break;
        default: return sh.io.Error("値が不正です");
        }
        return 1;

    }
    private static int ObjParamAddBone(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return 0;
        string[] sa=ParseUtil.LeftAndRight(val,ParseUtil.period);
        if(sa[0]==""||sa[1]=="") return sh.io.Error("書式が不正です");
        if(!UTIL.ValidObjName(sa[1])) return sh.io.Error("オブジェクト名が不正です");
        Renderer r;
        var oi=ObjInfo.GetObjInfo(tr);
        if(oi==null) r=tr.GetComponentInChildren<Renderer>();
        else r=oi.data.FindComponent<Renderer>();
        if(r==null||r.GetType()!=typeof(SkinnedMeshRenderer)) return sh.io.Error("SkinnedMeshRendererが見つかりません");
        SkinnedMeshRenderer smr=(SkinnedMeshRenderer)r;
        var bounds=smr.localBounds;
        if(smr.bones==null||smr.bones.Length<=0) return sh.io.Error("ボーンがありません");
        int n=smr.bones.Length;
        Transform[] tra=new Transform[n+1];
        Array.Copy(smr.bones,tra,n);
        bool manq=false;
        for(int i=0; i<n; i++) if(tra[i]!=null && tra[i].name!=null && tra[i].name.StartsWith("ManBip")){ manq=true; break; }
        string parentname=ParseUtil.CompleteBoneName(sa[0],manq);
        int pidx=-1;
        for(int i=0; i<n; i++) if(tra[i]!=null && tra[i].name==parentname){ pidx=i; break; }
        if(pidx<0) return sh.io.Error("親ボーンが見つかりません");

        Mesh mesh=smr.sharedMesh;
        var go=new GameObject(sa[1]);
        go.transform.SetParent(tra[pidx]);
        go.transform.localPosition=Vector3.zero;
        go.transform.localRotation=Quaternion.identity;
        tra[tra.Length-1]=go.transform;

        smr.bones=tra;

        Matrix4x4[] ma=new Matrix4x4[n+1];
        Array.Copy(smr.sharedMesh.bindposes,ma,n);
        ma[ma.Length-1]=ma[pidx];

        mesh.bindposes=ma;
        //smr.sharedMesh=mesh;
        smr.localBounds=bounds;

        if(oi!=null && oi.data!=null) oi.data.UpdateBones();
        return 1;
    }
    private static int ObjParamBone(ComShInterpreter sh,Transform tr,string val){
        Renderer r;
        var oi=ObjInfo.GetObjInfo(tr);
        if(oi==null) r=tr.GetComponentInChildren<Renderer>();
        else r=oi.data.FindComponent<Renderer>();
        if(r==null||r.GetType()!=typeof(SkinnedMeshRenderer)) return sh.io.Error("SkinnedMeshRendererが見つかりません");
        SkinnedMeshRenderer smr=(SkinnedMeshRenderer)r;
        int n=smr.bones.Length;
        Transform[] tra=smr.bones;
        if(val==null){
            for(int i=0; i<n; i++) if(tra[i]!=null) sh.io.PrintLn(tra[i].name);
            return 0;
        }
        GameObject go=new GameObject();
        Matrix4x4[] bpa=smr.sharedMesh.bindposes;
        bool manq=false;
        for(int i=0; i<n; i++) if(tra[i]!=null && tra[i].name!=null && tra[i].name.StartsWith("ManBip")){ manq=true; break; }
        string name=ParseUtil.CompleteBoneName(val,manq);
        for(int i=0; i<n; i++) if(tra[i]!=null){
            if(tra[i].name==val || tra[i].name==name){
                Matrix4x4 bp=bpa[i].inverse;
                Quaternion q=bp.GetRotation();  // 5.6に.rotationはない。これはどこかの拡張メソッド
                Vector3 ux=bp.GetColumn(0),uy=bp.GetColumn(1),uz=bp.GetColumn(2);
                float dx=ux.magnitude,dy=uy.magnitude,dz=uz.magnitude;
                sh.io.PrintLn2("wpos:",sh.fmt.FPos(bp.GetColumn(3)));
                sh.io.PrintLn2("wrot:",sh.fmt.FPos(q.eulerAngles));
                sh.io.PrintLn2("scale:",sh.fmt.FPos(dx,dy,dz));
                sh.io.PrintLn2("right:",sh.fmt.FPos(ux/dx));
                sh.io.PrintLn2("up:",sh.fmt.FPos(uy/dy));
                sh.io.PrintLn2("forward:",sh.fmt.FPos(uz/dz));
                return 0;
            }
        }
        return 0;
    }
    private static int ObjParamBBox(ComShInterpreter sh,Transform tr,string val){
        var bounds=ObjBBox(tr);
        if(bounds.extents==Vector3.zero) return 0;
        sh.io.PrintJoin(sh.ofs,sh.fmt.FPos(bounds.min),sh.fmt.FPos(bounds.max));
        return 0;
    }
    public static Bounds ObjBBox(Transform tr){
        Renderer[] r;
        var oi=ObjInfo.GetObjInfo(tr);
        if(oi==null) r=tr.GetComponentsInChildren<Renderer>();
        else r=(oi.data.FindComponents<Renderer>()).ToArray();
        if(r==null||r.Length==0) return new Bounds(Vector3.zero,Vector3.zero);
        var bounds=getBounds(r[0]);
        for(int i=1; i<r.Length; i++) bounds.Encapsulate(getBounds(r[i]));
        return bounds;
    }
    private static Bounds getBounds(Renderer r){
        if(r.GetType()==typeof(SkinnedMeshRenderer)){
            SkinnedMeshRenderer smr=(SkinnedMeshRenderer)r;
            var mesh=new Mesh();
            smr.BakeMesh(mesh);
            mesh.RecalculateBounds();
            smr.localBounds=mesh.bounds;
            GameObject.Destroy(mesh);
        }
        return r.bounds;
    }
}
public static class ObjUtil {
    public static Dictionary<string,Transform> objDic=new Dictionary<string,Transform>();
    public static Transform FindObj(ComShInterpreter sh,string name){
        if(name=="") return null;
        Transform tr=null;
        if(name[0]=='/'){
            if(name.Length>1 && name[1]=='/'){
                string nm=name.Substring(2);
                if(nm.Length!=0){
                    var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                    var lr=ParseUtil.LeftAndRight(nm,'/');
                    foreach(var root in scene.GetRootGameObjects()) if(root.name==lr[0]){
                        tr=root.transform;
                        if(lr[1]!="") return tr.Find(lr[1]);
                        return tr;
                    }
                }
                return null;
            }
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
        if(sa.Length==2){
            if(sa[0]=="obj") return FindObj(sh,sa[1]);
            if(sa[0]=="light") return LightUtil.FindLight(sh,sa[1]);
            if(sa[0]==""){
                if(sa[1]=="camera") return GameMain.Instance.MainCamera.camera.transform;
                if(sa[1]=="bg") return GameMain.Instance.BgMgr.BgObject.transform;
            }
        }

        string[] lr=ParseUtil.LeftAndRight(sa[sa.Length-1],'/');
        if(sa.Length==4){
            tr=BoneUtil.FindBone(sh,sa[0],sa[1],sa[2],(lr[0]=="")?"/":lr[0]);
        }else if(sa.Length==3){
            tr=BoneUtil.FindBone(sh,sa[0],sa[1],(lr[0]=="")?"/":lr[0]);
        }else if(sa.Length==2){
            tr=BoneUtil.FindBone(sh,sa[0],lr[0],"");
        }
        if(tr==null) return null;
        if(lr[1]!="") return tr.Find(lr[1]);
        return tr;
    }
    public static Transform FindObj(ComShInterpreter sh,ParseUtil.ColonDesc cd){
        if(cd.id=="") return null;
        Transform tr=null;
        if(cd.num==0 && cd.id!="") tr=FindObj(sh,cd.id);
        else tr=BoneUtil.FindBone(sh,cd);
        if(tr==null) return null;
        if(cd.path!="") return tr.Find(cd.path);
        return tr;
    }
    public static Transform FindObjRoot(ComShInterpreter sh,ParseUtil.ColonDesc cd){
        if(cd.id=="") return null;
        Transform tr=null;
        if(cd.num==0 && cd.id!="") tr=FindObj(sh,cd.id);
        else tr=BoneUtil.FindBone(sh,cd.type,cd.id,cd.slot,"/");
        return tr;
    }

    public static void RenameTr(Transform tr, string name){
        if(ObjUtil.objDic.TryGetValue(tr.name,out Transform t) && ReferenceEquals(tr,t)){
            ObjUtil.objDic.Remove(tr.name);
            ObjUtil.objDic[name]=tr;
        }
        tr.name=name;
    }
    public static Transform GetPhotoPrefabTr2(ComShInterpreter sh){
        // 一覧のみこっちを使う
        if(sh.objBase==string.Empty){
            GameObject bg=GameMain.Instance.BgMgr.BgObject;
            if(bg!=null) return bg.transform;
            return UTIL.GetObjRoot("ComShPrefab");
        }else return UTIL.GetObjRoot(sh.objBase);
    }
    public static Transform GetPhotoPrefabTr(ComShInterpreter sh){
        if(sh.objBase==string.Empty){
            GameObject bg=GameMain.Instance.BgMgr.BgObject;
            if(bg!=null && bg.activeInHierarchy) return bg.transform;
            return UTIL.GetObjRoot("ComShPrefab");
        }else return UTIL.GetObjRoot(sh.objBase);
    }
    public static int DeleteObj(ComShInterpreter sh,string name){
        Transform tr=ObjUtil.FindObj(sh,name.Split(ParseUtil.colon));
        if(tr!=null) return DeleteObj(sh,tr); else ObjUtil.objDic.Remove(name);
        return 0;
    }
    public static int DeleteObj<T>(ComShInterpreter sh,string name){
        Transform tr=ObjUtil.FindObj(sh,name.Split(ParseUtil.colon));
        if(tr!=null){
            if(tr.GetComponent<T>()!=null) return DeleteObj(sh,tr); 
        }else ObjUtil.objDic.Remove(name);
        return 0;
    }
    public static int DeleteObj(ComShInterpreter sh,Transform tr){
        if(tr==GameMain.Instance.MainCamera.camera.transform) return sh.io.Error("メインカメラにこの操作は行えません");
        tr.parent=null;
        ObjUtil.objDic.Remove(tr.name);
        UnityEngine.Object.DestroyImmediate(tr.gameObject);
        return 0;
    }

    private class FakeMaid:Maid{ private void OnDestroy(){} }
    private static FakeMaid dummyMaid;
    public static GameObject AddObject(string src, string name, Transform pr, Vector3 pos, Vector3 rot,Vector3 scl){
        GameObject o;
        bool instanceq=true;
        if(src=="."){ o=new GameObject(""); instanceq=false; }
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
        List<UnityEngine.Object> trash=null;
        if(o==null){
            if(dummyMaid==null){ // LoadSkinMesh_Rを呼ぶためだけのニセMaid
                var fo=new GameObject();
                fo.SetActive(false);
                dummyMaid=fo.AddComponent<FakeMaid>();
                dummyMaid.enabled=false;
                dummyMaid.m_goOffset=new GameObject("Offset");
                dummyMaid.body0=dummyMaid.gameObject.AddComponent<TBody>();
                dummyMaid.body0.enabled=false;
                dummyMaid.body0.maid=dummyMaid;
                dummyMaid.body0.m_hitFloorPlane=null;
                dummyMaid.body0.boMAN=dummyMaid.boMAN=false;
                dummyMaid.body0.goSlot=new List<TBodySkin>(1);
                dummyMaid.body0.goSlot.Add(new TBodySkin(dummyMaid.body0,"body",0,false));
            }
            trash=dummyMaid.body0.goSlot[0].listDEL;
            trash.Clear();
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
            morph=new List<TMorph>(32);
            var ga=MenuObj.ToObject(mo,morph);
            if(ga.Count>0){
                o=new GameObject("");
                foreach(var g in ga) g.transform.SetParent(o.transform);
            }
            if(morph.Count==0) morph=null;
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
        var oi=ObjInfo.AddObjInfo(b,src,morph);
        if(trash!=null) oi.data.trash=trash.ToArray();
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

        UTIL.TraverseTr(go.transform,(Transform tr,int d)=>{
            ObjInfo inf=tr.GetComponent<ObjInfo>();
            if(inf!=null) inf.enabled=false;
            return 0;
        });

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
            List<GameObject> ret=new List<GameObject>(32);
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
                    if(mo.material.TryGetValue(slot,out List<Material> mlist)) SetMaterial(o.transform,mlist);
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
        private static int SetMaterial(Transform tr,List<Material> mlist){
            var smr=tr.gameObject.GetComponentInChildren<SkinnedMeshRenderer>(true);
            if(smr==null) return -1;

            UnityEngine.Material[] ma=smr.sharedMaterials;
            try{
                foreach(var m in mlist) if(m.no<ma.Length){
                    ImportCM.LoadMaterial(UTIL.Suffix(m.name,".mate"),null,ma[m.no]);
                    if(m.shader!=null){
                        Shader sh=Shader.Find(m.shader);
                        if(sh!=null) ma[m.no].shader=sh;
                    }
                }
                smr.sharedMaterials=ma;
            }catch{ return -1; }
            return 0;
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
                _=r.ReadInt32();
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
                        if(args.Count>=3) slot=args[2].ToLower();
                        if(args.Count>=2) ret.modelName[slot]=UTIL.Suffix(args[1],".model");
                    }else if(cmd=="アイテム"){
                        if(args.Count>=2) ReadMenuObj(UTIL.Suffix(args[1],".menu"),ret);
                    }else if(cmd=="マテリアル変更"){
                        if(args.Count>=4) ret.AddMaterial(args[1].ToLower(),int.Parse(args[2]),UTIL.Suffix(args[3],".mate"));
                    }else if(cmd=="shader"){
                        if(args.Count>=4) ret.ChgShader(args[1].ToLower(),int.Parse(args[2]),args[3]);
                    }else if(cmd=="anime"){
                        string slot="";
                        if(args.Count>=2) slot=args[1].ToLower();
                        if(args.Count>=3){ ret.anmFile[slot]=new MenuObj.Anm(UTIL.Suffix(args[2],".anm")); }
                        if(args.Count>=4 && args[3]=="loop") ret.anmFile[slot].loopq=true;
                    }
                }
            }
        }catch{}
        return ret;
    }
    public static bool ChkAssetBundle(string fname) {
        return ChkAssetBundle(fname,ComShInterpreter.myobjDir);
    }
    public static bool ChkAssetBundle(string fname,string basedir) {
        return ChkAssetBundleSub(fname,basedir)!=null;
    }
    private static string ChkAssetBundleSub(string fname,string basedir) {
        var name=Path.GetFullPath(basedir+UTIL.Suffix(fname,".asset_bg"));
        if(!name.StartsWith(basedir,Ordinal)) return null;
        return File.Exists(name)?name:null;
    }
    public static List<string> ListAssetBundle<T>(string bundle) where T : UnityEngine.Object {
        return ListAssetBundle<T>(bundle,ComShInterpreter.myobjDir);
    }
    public static List<string> ListAssetBundle<T>(string bundle,string basedir) where T : UnityEngine.Object {
        var fname=ChkAssetBundleSub(bundle,basedir);
        if(fname==null) return null;
        return ListAssetBundle_NoChk<T>(fname);
    }
    private static bool abcacheq=false;
    private static Dictionary<string,AssetBundle> abdic=new Dictionary<string,AssetBundle>();
    public static void ABCache(){ abcacheq=true; }
    public static void ABNoCache(){
        if(!abcacheq) return;
        foreach(var ab in abdic.Values) if(ab!=null) ab.Unload(false);
        abdic.Clear();
        abcacheq=false;
    }
    public static List<string> ListAssetBundle_NoChk<T>(string bundle) where T : UnityEngine.Object {
        T[] aa=AllAsset_NoChk<T>(bundle);
        if(aa==null) return null;
        List<string> lst=new List<string>();
        for(int i=0; i<aa.Length; i++) if(aa[i]!=null){ lst.Add(aa[i].name); GameObject.Destroy(aa[i]); }
        return lst;
    }
    public static T[] AllAsset<T>(string bundle) where T : UnityEngine.Object {
        return AllAsset<T>(bundle,ComShInterpreter.myobjDir);
    }
    public static T[] AllAsset<T>(string bundle,string basedir) where T : UnityEngine.Object {
        var fname=ChkAssetBundleSub(bundle,basedir);
        if(fname==null) return null;
        return AllAsset_NoChk<T>(fname);
    }
    public static T[] AllAsset_NoChk<T>(string bundle) where T : UnityEngine.Object {
        AssetBundle ab=null;
        try{
            if(abcacheq){
                if(!abdic.TryGetValue(bundle,out ab)){
                    ab=AssetBundle.LoadFromFile(bundle);
                    if(ab==null) return null;
                    abdic[bundle]=ab; 
                }
            }else{
                ab=AssetBundle.LoadFromFile(bundle);
                if(ab==null) return null;
            }
            return ab.LoadAllAssets<T>();
        }catch{ return null;}
        finally{ if(!abcacheq && ab!=null) ab.Unload(false); }
    }
    public static T LoadAssetBundle<T>(string bundle,string path) where T:UnityEngine.Object {
        return LoadAssetBundle<T>(bundle,path,ComShInterpreter.myobjDir);
    }
    public static T LoadAssetBundle<T>(string bundle,string path,string basedir) where T:UnityEngine.Object {
        var fname=Path.GetFullPath(basedir+UTIL.Suffix(bundle,".asset_bg"));
        if(!fname.StartsWith(basedir,Ordinal)) return null;
        T ret;
        AssetBundle ab=null;
        try{
            if(abcacheq){
                if(!abdic.TryGetValue(fname,out ab)){
                    ab=AssetBundle.LoadFromFile(fname);
                    if(ab==null) return null;
                    abdic[fname]=ab;
                }
            }else{
                ab=AssetBundle.LoadFromFile(fname);
                if(ab==null) return null;
            }
            if(!ab.Contains(path)){if(!abcacheq) ab.Unload(false); return null;}
            ret=ab.LoadAsset<T>(path);
            if(ret==null){if(!abcacheq) ab.Unload(false); return null; }
            if(ret.GetType()==typeof(Cubemap)) ret=TextureUtil.CloneCubemap(ret as Cubemap) as T;
            else ret=UnityEngine.Object.Instantiate<T>(ret);
            if(!abcacheq) ab.Unload(false);
        }catch(Exception e){Debug.Log(e.ToString());if(ab!=null && !abcacheq) ab.Unload(false); return null;}
        return ret;
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
        AnmFile af=null;
        try{
            if(File.Exists(myposefile)) af=new AnmFile(myposefile);
            else{ var arr=UTIL.AReadAll(file); if(arr!=null) af=new AnmFile(arr); }
            if(af==null||af.IsEmpty()) return -1;
            int gender=ChkBoneGender(a.gameObject.transform);
            if(gender==0 && af.gender==1) af.ChgGender();
        }catch{ return -1; }
        if(af==null) return -1;

        string tag=file.ToLower();
        var clip=af.ToClip();
        if(clip==null) return -1;
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
    public delegate int ComponentVisitor<T>(T c);
    public static void ForeachComponents<T>(Transform tr,ComponentVisitor<T> func,bool recursiveq=false) where T:Component{
        var ca=(recursiveq)?tr.GetComponentsInChildren<T>():tr.GetComponents<T>();
        for(int i=0; i<ca.Length; i++) if(ca[i]!=null) if(func.Invoke(ca[i])<0) break;
    }
    public static void ClearComponent<T>(Transform tr,bool recursiveq=false) where T:Component{
        var ca=(recursiveq)?tr.GetComponentsInChildren<T>():tr.GetComponents<T>();
        for(int i=0; i<ca.Length; i++) if(ca[i]!=null) UnityEngine.Object.Destroy(ca[i]);
    }
    public static GameObject CreateCustomPlane(int w,int h){
        Mesh mesh=new Mesh();
        int wn=w+1,hn=h+1;
        var va=new Vector3[wn*hn];
        var tri=new int[w*h*6];
        var uv=new Vector2[wn*hn];
        int idx=0;
        for(int y=0; y<hn; y++) for(int x=0; x<hn; x++){
            float u=(float)x/w-0.5f,v=(float)y/h-0.5f;
            va[idx]=new Vector3(u,0,v);
            uv[idx++]=new Vector3(u,v);
        }
        idx=0;
        for(int y=0; y<h; y++) for(int x=0; x<h; x++){
            int a=y*wn+x,b=a+1,c=b+wn,d=b+wn-1;
            tri[idx++]=a;
            tri[idx++]=c;
            tri[idx++]=b;
            tri[idx++]=a;
            tri[idx++]=d;
            tri[idx++]=c;
        }
        mesh.vertices=va;
        mesh.triangles=tri;
        mesh.uv=uv;
        mesh.RecalculateNormals();

        var go=new GameObject();
        var mf=go.AddComponent<MeshFilter>();
        mf.mesh=mesh;
        var mr=go.AddComponent<MeshRenderer>();
        mr.material=new Material(Shader.Find("Standard"));
        return go;
    }
}
}
