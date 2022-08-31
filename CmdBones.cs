﻿using System.Collections.Generic;
using UnityEngine;
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
                foreach(var kv in BoneUtil.boneCache) sh.io.PrintLn(kv.Key);
                return 0;
            }
            if(args.Count<4) return sh.io.Error("使い方: bone alias ボーン指定 別名");
            Transform tr=BoneUtil.FindBone(sh,args[2]);
            if(tr==null) return sh.io.Error("ボーンの指定が不正です");

            if(!UTIL.ValidName(args[3])) sh.io.Error("その名前は使用できません");
            if(BoneUtil.boneCache.ContainsKey(args[3])) return sh.io.Error("その名前はすでに使用されています");
            BoneUtil.boneCache[args[3]]=tr;
            return 0;
        }else if(args[1]=="unalias"){
            if(args.Count<3) return sh.io.Error("使い方: bone unalias 別名");
            for(int i=2; i<args.Count; i++) BoneUtil.boneCache.Remove(args[i]);
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
            UTIL.TraverseTr(tr,(Transform t)=>{sh.io.PrintLn(t.name); return 0;});
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
        var del=new List<string>();
        foreach(var kv in boneCache) if(kv.Value==null) del.Add(kv.Key);
        foreach(var k in del) boneCache.Remove(k);
    }

    public static Transform FindBone(ComShInterpreter sh,string name){
        if(boneCache.TryGetValue(name,out Transform tr)){
            // 配置解除等で消えていたらUnity的なnullになる。ちゃんと参照を消しておく
            if(tr==null){ boneCache.Remove(name); return null; }
            return tr;
        }
        return ObjUtil.FindObj(sh, name.Split(ParseUtil.colon));
    }

    public static Transform FindBone(ComShInterpreter sh,string s1,string s2,string s3){
        if(s1==""||s2==""||s3=="") return null;
        if(s1=="obj"){
            Transform tr=ObjUtil.FindObj(sh,s2);
            if(tr==null) return null;
            if(s3=="/") return tr;
            string fullname=ParseUtil.CompleteBoneName(s3,false);
            var oi=tr.gameObject.GetComponent<ObjInfo>();
            if(oi!=null) return oi.FindBone(fullname);
            else return CMT.SearchObjName(tr,fullname);
        }else if(s1=="maid"||s1=="man"){
            string[] s2lr=ParseUtil.LeftAndRight(s2,'.');
            Maid m=MaidUtil.FindMaidMan(s1,s2lr[0]);
            if(m==null||m.body0==null) return null;
            if(s3=="/"){
                if(s2lr[1]!=""){
                    if(!m.body0.IsSlotNo(s2lr[1])) return null;
                    return m.body0.goSlot[m.body0.GetSlotNo(s2lr[1])].obj_tr;
                }else return m.body0.m_trBones;
            }else{
                MaidBone b=MaidBone.Find(m,s3,s2lr[1]);
                if(b==null||b.boneTr==null) return null;
                return b.boneTr;
            }
        }else return null;
    }
    public class MaidBone {
        public Maid maid;
        public int iid;
        public string name;
        public Transform boneTr;
        public static MaidBone Find(Maid m,string abn,string slot=null){
            if(m.body0==null||m.body0.m_trBones==null) return null;
            if(abn.Length==0) return null;
            string name=ParseUtil.CompactBoneName(abn);
            string fullname=ParseUtil.CompleteBoneName(abn,m.boMAN);
            Transform tr;
            if(!string.IsNullOrEmpty(slot)){
                if(!m.body0.IsSlotNo(slot)) return null;
                var root=m.body0.goSlot[m.body0.GetSlotNo(slot)].obj_tr;
                if(root==null) return null;
                tr=CMT.SearchObjName(root,fullname,false);
            }else tr=CMT.SearchObjName(m.body0.m_trBones,fullname);
            if(tr==null) return null;
            return new MaidBone(m,tr,name);
        }
        public MaidBone(Maid m,Transform btr,string name){
            maid=m;
            iid=m.GetInstanceID();
            boneTr=btr;
            this.name=name;
        }
    }
}
}
