using System.Collections.Generic;
using UnityEngine;
using static System.StringComparison;
using static COM3D2.ComSh.Plugin.Command;

namespace COM3D2.ComSh.Plugin {

public static class CmdBones {
    public static void Init(){
        Command.AddCmd("bone",new Cmd(CmdBone));

        boneParamDic.Add("wpos",new CmdParam<Transform>(_CmdParamWPos));
        boneParamDic.Add("wpos.x",new CmdParam<Transform>(_CmdParamWPosX));
        boneParamDic.Add("wpos.y",new CmdParam<Transform>(_CmdParamWPosY));
        boneParamDic.Add("wpos.z",new CmdParam<Transform>(_CmdParamWPosZ));
        boneParamDic.Add("lpos",new CmdParam<Transform>(_CmdParamLPos));
        boneParamDic.Add("lpos.x",new CmdParam<Transform>(_CmdParamLPosX));
        boneParamDic.Add("lpos.y",new CmdParam<Transform>(_CmdParamLPosY));
        boneParamDic.Add("lpos.z",new CmdParam<Transform>(_CmdParamLPosZ));
        boneParamDic.Add("opos",new CmdParam<Transform>(_CmdParamOPos));
        boneParamDic.Add("wrot",new CmdParam<Transform>(_CmdParamWRot));
        boneParamDic.Add("wrot.x",new CmdParam<Transform>(_CmdParamWRotX));
        boneParamDic.Add("wrot.y",new CmdParam<Transform>(_CmdParamWRotY));
        boneParamDic.Add("wrot.z",new CmdParam<Transform>(_CmdParamWRotZ));
        boneParamDic.Add("lrot",new CmdParam<Transform>(_CmdParamLRot));
        boneParamDic.Add("lrot.x",new CmdParam<Transform>(_CmdParamLRotX));
        boneParamDic.Add("lrot.y",new CmdParam<Transform>(_CmdParamLRotY));
        boneParamDic.Add("lrot.z",new CmdParam<Transform>(_CmdParamLRotZ));
        boneParamDic.Add("orot",new CmdParam<Transform>(_CmdParamORot));
        boneParamDic.Add("scale",new CmdParam<Transform>(_CmdParamScale));
        boneParamDic.Add("scale.x",new CmdParam<Transform>(_CmdParamScaleX));
        boneParamDic.Add("scale.y",new CmdParam<Transform>(_CmdParamScaleY));
        boneParamDic.Add("scale.z",new CmdParam<Transform>(_CmdParamScaleZ));
        boneParamDic.Add("list",new CmdParam<Transform>(BoneParamList));
        boneParamDic.Add("lquat",new CmdParam<Transform>(_CmdParamLQuat));
        boneParamDic.Add("wquat",new CmdParam<Transform>(_CmdParamWQuat));
        boneParamDic.Add("prot",new CmdParam<Transform>(_CmdParamPRot));
        boneParamDic.Add("pquat",new CmdParam<Transform>(_CmdParamPQuat));
        boneParamDic.Add("handle",new CmdParam<Transform>(BoneParamHandle));

        boneParamDic.Add("l2w",new CmdParam<Transform>(_CmdParamL2W));
        boneParamDic.Add("w2l",new CmdParam<Transform>(_CmdParamW2L));

        CmdParamPosRotCp(boneParamDic,"wpos","position");
        CmdParamPosRotCp(boneParamDic,"wpos","pos");
        CmdParamPosRotCp(boneParamDic,"lrot","rotation");
        CmdParamPosRotCp(boneParamDic,"lrot","rot");
    }

    private static Dictionary<string,CmdParam<Transform>> boneParamDic=new Dictionary<string,CmdParam<Transform>>();

    private static int CmdBone(ComShInterpreter sh,List<string> args) {
        if(args.Count==1) return 0;
        if(args[1]=="alias"){
            // aliasと言いつつ、キャッシュするのが真の目的
            BoneUtil.CleanBoneCache();
            if(args.Count==2){
                foreach(var kv in BoneUtil.boneCache) if(kv.Key.StartsWith(sh.ns,Ordinal)) sh.io.PrintLn(kv.Key);
                return 0;
            }
            if(args.Count<4) return sh.io.Error("使い方: bone alias ボーン指定 別名");
            Transform tr=BoneUtil.FindBone(sh,args[2]);
            if(tr==null) return sh.io.Error("ボーンの指定が不正です");

            if(!UTIL.ValidName(args[3])) sh.io.Error("その名前は使用できません");
            string name=sh.ns+args[3];
            if(BoneUtil.boneCache.ContainsKey(name)) return sh.io.Error("その名前はすでに使用されています");
            BoneUtil.boneCache[name]=tr;
            return 0;
        }else if(args[1]=="unalias"){
            if(args.Count<3) return sh.io.Error("使い方: bone unalias 別名");
            for(int i=2; i<args.Count; i++) BoneUtil.boneCache.Remove(sh.ns+args[i]);
            BoneUtil.CleanBoneCache();
            return 0;
        }
        return CmdBoneSub(sh,args[1],args,2);
    }
    public static int CmdBoneSub(ComShInterpreter sh,string id, List<string> args,int prmstart) {
        Transform tr=BoneUtil.FindBone(sh,id);
        if(tr==null) return sh.io.Error("ボーンの指定が不正です");
        if(args.Count==prmstart){
            sh.io.PrintLn2("iid:",tr.gameObject.GetInstanceID().ToString());            
            UTIL.PrintTrInfo(sh,tr);
            return 0;
        }
        return ParamLoop(sh,tr,boneParamDic,args,prmstart);
    }
    private static int BoneParamList(ComShInterpreter sh,Transform tr,string val){
        if(val==null) return sh.io.Error("list種別に child か descendant を指定してください");
        if(val=="c"||val=="child"){
            for(int i=0; i<tr.childCount; i++) sh.io.PrintLn(tr.GetChild(i).name);
        }else if(val=="d"||val=="descendant"){
            UTIL.TraverseTr(tr,(Transform t,int d)=>{sh.io.PrintLn(t.name); return 0;});
        }else return sh.io.Error("list種別に child か descendant を指定してください");
        return 0;
    }
    private static int BoneParamHandle(ComShInterpreter sh,Transform tr,string val){
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
}
public static class BoneUtil{
    public static Dictionary<string,Transform> boneCache=new Dictionary<string,Transform>();
    public static void CleanBoneCache(){
        var del=new List<string>(boneCache.Count);
        foreach(var kv in boneCache) if(kv.Value==null) del.Add(kv.Key);
        foreach(var k in del) boneCache.Remove(k);
    }

    public static Transform FindBone(ComShInterpreter sh,string name){
        string nsid=sh.ns+name;
        if(boneCache.TryGetValue(nsid,out Transform tr)){
            if(tr==null){ boneCache.Remove(nsid); return null; }
            return tr;
        }
        return ObjUtil.FindObj(sh, new ParseUtil.ColonDesc(name));
    }

    public static Transform FindBone(ComShInterpreter sh,ParseUtil.ColonDesc cd){
        return FindBone(sh,cd.type,cd.id,cd.slot,cd.bone);
    }
    public static Transform FindBone(ComShInterpreter sh,string s1,string s2,string s3){
        string[] lr=ParseUtil.LeftAndRight(s2,'.');
        return FindBone(sh,s1,lr[0],lr[1],s3);
    }
    public static Transform FindBone(ComShInterpreter sh,string type,string id,string slot,string bone){
        if(id=="") return null;
        if(type==""){
            if(id=="camera") return GameMain.Instance.MainCamera.camera.transform;
            else if(id=="bg"){
                Transform tr=GameMain.Instance.BgMgr.BgObject.transform;
                if(tr==null) return null;
                if(bone==""||bone=="/") return tr;
                var oi=tr.GetComponent<ObjInfo>();
                if(oi==null) oi=ObjInfo.AddObjInfo(tr,"");
                return oi.data.FindBone(bone);
            }else return null;
        }else if(type=="obj"){
            Transform tr=ObjUtil.FindObj(sh,id);
            if(tr==null) return null;
            if(bone==""||bone=="/") return tr;
            string fullname=ParseUtil.CompleteBoneName(bone,false);
            var oi=tr.GetComponent<ObjInfo>();
            if(oi==null) oi=ObjInfo.AddObjInfo(tr,"");
            return oi.data.FindBone(fullname);
        }else if(type=="light"){
            return LightUtil.FindLight(sh,id);
        }else if(type=="maid"||type=="man"){
            Maid m=MaidUtil.FindMaidMan(type,id);
            if(m==null||m.body0==null) return null;
            if(bone=="" && slot=="") return m.transform;
            if(bone=="/"){
                if(slot!=""){
                    if(!m.body0.IsSlotNo(slot)) return null;
                    return m.body0.goSlot[m.body0.GetSlotNo(slot)].obj_tr;
                }else return m.body0.m_trBones;
            }else{
                MaidBone b=MaidBone.Find(m,bone,slot);
                if(b==null||b.boneTr==null) return null;
                return b.boneTr;
            }
        }else return null;
    }
    public class MaidBone {
        public Maid maid;
        public int iid;
        public Transform boneTr;
        public static MaidBone Find(Maid m,string abn,string slot=null){
            if(m.body0==null||m.body0.m_trBones==null) return null;
            if(abn.Length==0) return null;
            string fullname=ParseUtil.CompleteBoneName(abn,m.boMAN,m.IsCrcBody);
            Transform tr;
            if(!string.IsNullOrEmpty(slot)){
                if(!m.body0.IsSlotNo(slot)) return null;
                var root=m.body0.goSlot[m.body0.GetSlotNo(slot)].obj_tr;
                if(root==null) return null;
                var oi=root.GetComponent<ObjInfo>();
                if(oi==null) oi=ObjInfo.AddObjInfo(root,"");
                tr=oi.data.FindBone(fullname);
            }else{
                var oi=m.body0.m_trBones.GetComponent<ObjInfo>();
                if(oi==null) oi=ObjInfo.AddObjInfo(m.body0.m_trBones,"");
                tr=oi.data.FindBone(fullname);
            }
            if(tr==null) return null;
            return new MaidBone(m,tr);
        }
        public MaidBone(Maid m,Transform btr){
            maid=m;
            iid=m.GetInstanceID();
            boneTr=btr;
        }
    }
}
}
