using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static System.StringComparison;
using static COM3D2.ComSh.Plugin.Command;
using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace COM3D2.ComSh.Plugin {

public static class CmdMaidMan {

    public static void Init(){
		Command.AddCmd("maid", new Cmd(CmdMaid));
		Command.AddCmd("man", new Cmd(CmdMan));

        maidParamDic.Add("lpos",new CmdParam<Maid>(MaidParamLPos));
        maidParamDic.Add("lpos.x",new CmdParam<Maid>(MaidParamLPosX));
        maidParamDic.Add("lpos.y",new CmdParam<Maid>(MaidParamLPosY));
        maidParamDic.Add("lpos.z",new CmdParam<Maid>(MaidParamLPosZ));
        maidParamDic.Add("lrot",new CmdParam<Maid>(MaidParamLRot));
        maidParamDic.Add("lrot.x",new CmdParam<Maid>(MaidParamLRotX));
        maidParamDic.Add("lrot.y",new CmdParam<Maid>(MaidParamLRotY));
        maidParamDic.Add("lrot.z",new CmdParam<Maid>(MaidParamLRotZ));
        maidParamDic.Add("lposrot",new CmdParam<Maid>(MaidParamLPosRot));
        maidParamDic.Add("wpos",new CmdParam<Maid>(MaidParamWPos));
        maidParamDic.Add("wpos.x",new CmdParam<Maid>(MaidParamWPosX));
        maidParamDic.Add("wpos.y",new CmdParam<Maid>(MaidParamWPosY));
        maidParamDic.Add("wpos.z",new CmdParam<Maid>(MaidParamWPosZ));
        maidParamDic.Add("wrot",new CmdParam<Maid>(MaidParamWRot));
        maidParamDic.Add("wrot.x",new CmdParam<Maid>(MaidParamWRotX));
        maidParamDic.Add("wrot.y",new CmdParam<Maid>(MaidParamWRotY));
        maidParamDic.Add("wrot.z",new CmdParam<Maid>(MaidParamWRotZ));
        maidParamDic.Add("wposrot",new CmdParam<Maid>(MaidParamWPosRot));
        maidParamDic.Add("opos",new CmdParam<Maid>(MaidParamOPos));
        maidParamDic.Add("orot",new CmdParam<Maid>(MaidParamORot));
        maidParamDic.Add("scale",new CmdParam<Maid>(MaidParamScale));
        maidParamDic.Add("scale.x",new CmdParam<Maid>(MaidParamScaleX));
        maidParamDic.Add("scale.y",new CmdParam<Maid>(MaidParamScaleY));
        maidParamDic.Add("scale.z",new CmdParam<Maid>(MaidParamScaleZ));
        maidParamDic.Add("motion",new CmdParam<Maid>(MaidParamMotion));
        maidParamDic.Add("motion.cache",new CmdParam<Maid>(MaidParamMotionCache));
        maidParamDic.Add("motion.cachedel",new CmdParam<Maid>(MaidParamMotionCacheDel));
        maidParamDic.Add("motion.pause",new CmdParam<Maid>(MaidParamMotionPause));
        maidParamDic.Add("motion.frame",new CmdParam<Maid>(MaidParamMotionFrame));
        maidParamDic.Add("motion.time",new CmdParam<Maid>(MaidParamMotionTime));
        maidParamDic.Add("motion.timep",new CmdParam<Maid>(MaidParamMotionTimep));
        maidParamDic.Add("motion.timel",new CmdParam<Maid>(MaidParamMotionTimeL));
        maidParamDic.Add("motion.timelp",new CmdParam<Maid>(MaidParamMotionTimepL));
        maidParamDic.Add("motion.speed",new CmdParam<Maid>(MaidParamMotionSpeed));
        maidParamDic.Add("motion.layer",new CmdParam<Maid>(MaidParamMotionLayer));
        maidParamDic.Add("motion.layerdel",new CmdParam<Maid>(MaidParamMotionLayerDel));
        maidParamDic.Add("motion.length",new CmdParam<Maid>(MaidParamMotionLength));
        maidParamDic.Add("motion.weight",new CmdParam<Maid>(MaidParamMotionWeight));
        maidParamDic.Add("lookat",new CmdParam<Maid>(MaidParamLookAt));
        maidParamDic.Add("iid",new CmdParam<Maid>(MaidParamIid));
        maidParamDic.Add("attach",new CmdParam<Maid>(MaidParamAttach));
        maidParamDic.Add("detach",new CmdParam<Maid>(MaidParamDetach));
        maidParamDic.Add("list",new CmdParam<Maid>(MaidParamList));
        maidParamDic.Add("ik",new CmdParam<Maid>(MaidParamIK));
        maidParamDic.Add("preset",new CmdParam<Maid>(MaidParamPreset));
        maidParamDic.Add("presetsave",new CmdParam<Maid>(MaidParamPresetSave));
        maidParamDic.Add("cloth",new CmdParam<Maid>(MaidParamCloth));
        maidParamDic.Add("cloth2",new CmdParam<Maid>(MaidParamCloth2));
        maidParamDic.Add("mekure",new CmdParam<Maid>(MaidParamMekure));
        maidParamDic.Add("mekureF",new CmdParam<Maid>(MaidParamMekureF));
        maidParamDic.Add("mekureB",new CmdParam<Maid>(MaidParamMekureB));
        maidParamDic.Add("zurashi",new CmdParam<Maid>(MaidParamZurashi));
        maidParamDic.Add("hadake",new CmdParam<Maid>(MaidParamHadake));
        maidParamDic.Add("undress",new CmdParam<Maid>(MaidParamUndress));
        maidParamDic.Add("shape",new CmdParam<Maid>(MaidParamShape));
        maidParamDic.Add("shape.verlist",new CmdParam<Maid>(MaidParamShapeVerList));
        maidParamDic.Add("style",new CmdParam<Maid>(MaidParamStyle));
        maidParamDic.Add("style2",new CmdParam<Maid>(MaidParamStyle2));
        maidParamDic.Add("face",new CmdParam<Maid>(MaidParamFace));
        maidParamDic.Add("blink",new CmdParam<Maid>(MaidParamBlink));
        maidParamDic.Add("facesave",new CmdParam<Maid>(MaidParamFaceSave));
        maidParamDic.Add("faceset",new CmdParam<Maid>(MaidParamFaceset));
        maidParamDic.Add("sing",new CmdParam<Maid>(MaidParamSing));
        maidParamDic.Add("sing2",new CmdParam<Maid>(MaidParamSing2));
        maidParamDic.Add("sing.mml",new CmdParam<Maid>(MaidParamSingMML));
        maidParamDic.Add("sing.keys",new CmdParam<Maid>(MaidParamSingKeys));
        maidParamDic.Add("sing.vowel",new CmdParam<Maid>(MaidParamSingVowel));
        maidParamDic.Add("voice",new CmdParam<Maid>(MaidParamVoice));
        maidParamDic.Add("gravityS",new CmdParam<Maid>(MaidParamGravityS));
        maidParamDic.Add("gravityH",new CmdParam<Maid>(MaidParamGravityH));
        maidParamDic.Add("lquat",new CmdParam<Maid>(MaidParamLQuat));
        maidParamDic.Add("wquat",new CmdParam<Maid>(MaidParamWQuat));
        maidParamDic.Add("prot",new CmdParam<Maid>(MaidParamPRot));
        maidParamDic.Add("pquat",new CmdParam<Maid>(MaidParamPQuat));
        maidParamDic.Add("muneyure",new CmdParam<Maid>(MaidParamMuneYure));
        maidParamDic.Add("autotwist",new CmdParam<Maid>(MaidParamAutoTwist));
        maidParamDic.Add("ap",new CmdParam<Maid>(MaidParamAttachPoint));
        maidParamDic.Add("ap.wpos",new CmdParam<Maid>(MaidParamAttachPointWpos));
        maidParamDic.Add("handle",new CmdParam<Maid>(MaidParamHandle));
        maidParamDic.Add("describe",new CmdParam<Maid>(MaidParamDesc));
        maidParamDic.Add("node",new CmdParam<Maid>(MaidParamNode));
        maidParamDic.Add("partsnode",new CmdParam<Maid>(MaidParamPartsNode));
        maidParamDic.Add("mask",new CmdParam<Maid>(MaidParamMask));
        maidParamDic.Add("select",new CmdParam<Maid>(MaidParamSelect));
        maidParamDic.Add("later",new CmdParam<Maid>(MaidParamLater));
        maidParamDic.Add("evenlater",new CmdParam<Maid>(MaidParamEvenLater));
        maidParamDic.Add("muneparam",new CmdParam<Maid>(MaidParamMuneParam));
        maidParamDic.Add("bbox",new CmdParam<Maid>(MaidParamBBox));
        maidParamDic.Add("floor",new CmdParam<Maid>(MaidParamFloor));
        maidParamDic.Add("skirtyure",new CmdParam<Maid>(MaidParamSkirtYure));
        maidParamDic.Add("clothyure",new CmdParam<Maid>(MaidParamClothYure));
        maidParamDic.Add("shape.rename",new CmdParam<Maid>(MaidParamShapeRename));
        maidParamDic.Add("shape.add",new CmdParam<Maid>(MaidParamShapeAdd));
        maidParamDic.Add("shape.export",new CmdParam<Maid>(MaidParamShapeExport));
        maidParamDic.Add("clothfollow",new CmdParam<Maid>(MaidParamClothFollow));
        maidParamDic.Add("component",new CmdParam<Maid>(MaidParamComponent));
        maidParamDic.Add("skirtparam",new CmdParam<Maid>(MaidParamSkirtParam));
        maidParamDic.Add("skirtparam.curve",new CmdParam<Maid>(MaidParamSkirtParamCurve));
        maidParamDic.Add("skirtparam.save",new CmdParam<Maid>(MaidParamSkirtParamSave));
        maidParamDic.Add("skirtparam.load",new CmdParam<Maid>(MaidParamSkirtParamLoad));

        maidParamDic.Add("l2w",new CmdParam<Maid>(MaidParamL2W));
        maidParamDic.Add("w2l",new CmdParam<Maid>(MaidParamW2L));

        CmdParamPosRotCp(maidParamDic,"lpos","position");
        CmdParamPosRotCp(maidParamDic,"lpos","pos");
        CmdParamPosRotCp(maidParamDic,"lrot","rotation");
        CmdParamPosRotCp(maidParamDic,"lrot","rot");

        for(int i=0; i<manParams.Length; i++) manParamDic.Add(manParams[i],maidParamDic[manParams[i]]);
        manParamDic.Add("chinko",new CmdParam<Maid>(MaidParamChinko));
        manParamDic.Add("body",new CmdParam<Maid>(MaidParamBody));
    }
    private static Dictionary<string,CmdParam<Maid>> maidParamDic=new Dictionary<string,CmdParam<Maid>>();
    private static Dictionary<string,CmdParam<Maid>> manParamDic=new Dictionary<string,CmdParam<Maid>>();
    private static string[] manParams={
        "position","rotation","scale","pos","rot",
        "motion","shape","iid","list","motion.time","motion.speed","motion.layer","motion.length",
        "motion.weight","motion.timep","motion.timel","motion.timelp","motion.frame","motion.pause",
        "motion.cache","motion.cachedel","motion.layerdel",
        "attach","detach","lookat","ik","cloth","style","shape.verlist","face","faceset",
        "wpos","wrot","lpos","lrot","opos","orot","lquat","wquat","wposrot","lposrot","component",
        "position.x","position.y","position.z", "pos.x","pos.y","pos.z",
        "lpos.x","lpos.y","lpos.z", "wpos.x","wpos.y","wpos.z",
        "rotation.x","rotation.y","rotation.z", "rot.x","rot.y","rot.z",
        "lrot.x","lrot.y","lrot.z", "wrot.x","wrot.y","wrot.z",
        "scale.x","scale.y","scale.z", "prot","pquat",
        "ap","ap.wpos","handle","describe","select","later","evenlater","bbox",
        "l2w","w2l","shape.add","shape.export"
    };

 	private static int CmdMaid(ComShInterpreter sh,List<string> args) {
		if(args.Count==1){
			CharacterMgr cm=GameMain.Instance.CharacterMgr;
			int cnt=0;
			for (int i=0; i<cm.GetMaidCount(); i++) {
				Maid m=cm.GetMaid(i);
				if (m==null||m.body0==null||m.body0.m_trBones==null) continue;
				sh.io.PrintJoinLn(sh.ofs,cnt.ToString(),m.status.fullNameJpStyle.Trim(),sh.fmt.FPos(m.GetPos()));
				cnt++;
			}
            return 0;
        }else if(args[1]=="add"){
            if(args.Count<3) return sh.io.Error("メイドさんを指定してください");
            PlacementWindow pw=StudioMode.GetPlacementWindow();
            for(int i=2; i<args.Count; i++){
			    CharacterMgr cm=GameMain.Instance.CharacterMgr;
                int stockidx=MaidUtil.FindStockMaid(args[i]);
                if(stockidx<0) return sh.io.Error("存在しないメイドさんです");
                Maid sm=cm.GetStockMaid(stockidx);
                if(cm.GetMaid(sm.status.guid)!=null) return sh.io.Error("そのメイドさんは既に配置されています");
                int idx=MaidUtil.FindNullMaidIdx();
                if(idx==-1) return sh.io.Error("これ以上追加できません");

                if(pw==null){
                    Maid m=cm.Activate(idx,stockidx,false,false);
                    m.Visible=true;
                    m.AllProcProp(); // Seqの方使っても他の事何もできないんだから、ブロックするのと変わらんでしょ
                    m.CrossFade("maid_stand01.anm",false,true,false,0f);
				    m.FaceAnime("通常");
                    m.FaceBlend("オリジナル");
                }else{
                    if(StudioMode.MaidAddStudio(pw,sm.status.lastName,sm.status.firstName)<0) return sh.io.Error("失敗しました");
                }
            }
            return 0;
        }else if(args[1]=="del"){
            if(args.Count<3) return sh.io.Error("メイドさんを指定してください");
            PlacementWindow pw=StudioMode.GetPlacementWindow();
            if(pw==null){
			    CharacterMgr cm=GameMain.Instance.CharacterMgr;
                List<int> asn=new List<int>(args.Count-2);
                for(int i=2; i<args.Count; i++){
                    Maid m=MaidUtil.FindMaid(args[i]);
                    if(m!=null) asn.Add(m.ActiveSlotNo);
                }
                foreach(int n in asn) cm.Deactivate(n,false);
            }else{
                List<Maid> ml=new List<Maid>(args.Count-2);
                for(int i=2; i<args.Count; i++){
                    Maid m=MaidUtil.FindMaid(args[i]);
                    if(m!=null) ml.Add(m);
                }
                foreach(Maid m in ml) pw.DeActiveMaid(m);
            }
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            return 0;
        }else if(args[1]=="list"){
			CharacterMgr cm=GameMain.Instance.CharacterMgr;
            var slist=cm.GetStockMaidList();
            for(int i=0; i<slist.Count; i++)
                sh.io.PrintLn($"{i} {slist[i].status.fullNameJpStyle.Trim()} %{slist[i].GetInstanceID().ToString()}");
            return 0;
        }else{
            string[] sa=args[1].Split(ParseUtil.colon);
            if(sa.Length>1 && sa[0]=="maid") return CmdMaidSub(sh,sa[1],args,2);
            return CmdMaidSub(sh,args[1],args,2);
        }
    }
 	public static int CmdMaidSub(ComShInterpreter sh,string id, List<string> args,int prmstart){
        Maid m=MaidUtil.FindMaid(id);
        if(m==null) return sh.io.Error("指定されたメイドは存在しません");
        if(args.Count==prmstart){
            sh.io.Print($"iid:{m.GetInstanceID().ToString()}\n");
            sh.io.Print($"guid:{m.status.guid}\n");
            sh.io.Print($"creditguid:{m.status.saveDataGuidGP03}\n");
            sh.io.Print($"name:{m.status.fullNameJpStyle.Trim()}\n");
            sh.io.Print($"height:{m.status.body.height.ToString()}\n");
            sh.io.PrintJoinLn(sh.ofs,"bwh:"+m.status.body.bust.ToString(),m.status.body.waist.ToString(),m.status.body.hip.ToString());
            UTIL.PrintTrInfo(sh,m.transform);
            return 0;
        }
        return ParamLoop(sh,m,maidParamDic,args,prmstart);
	}
	private static int CmdMan(ComShInterpreter sh,List<string> args) {
		if (args.Count==1) {      // パラメータなしならls
			CharacterMgr cm=GameMain.Instance.CharacterMgr;
			int cnt=0;
			for (int i=0; i<cm.GetManCount(); i++) {
				Maid m=cm.GetMan(i);
				if (m==null) continue;
				sh.io.PrintJoinLn(sh.ofs, i.ToString(), (cnt==0)?"主人公":"男"+i, sh.fmt.FPos(m.GetPos()), (m.Visible)?"表示":"非表示" );
				cnt++;
			}
            return 0;
        }else if(args[1]=="add"){
            if(args.Count<3) return sh.io.Error("男性を指定してください");
            for(int i=2; i<args.Count; i++){
                Maid m=MaidUtil.FindMan(args[i]);
                if(m==null) return sh.io.Error("指定された男性は存在しません");
                PlacementWindow pw=StudioMode.GetPlacementWindow();
                if(pw==null || m.ActiveSlotNo==6){
    			    CharacterMgr cm=GameMain.Instance.CharacterMgr;
                    cm.SetActiveMan(m,m.ActiveSlotNo);
                    m.Visible=true;
                    m.AllProcProp();
                    m.CrossFade("man_porse01.anm",false,true,false,0f);
                }else{
                    int ret=StudioMode.MaidAddStudio(pw,null,m.ActiveSlotNo==0?"主人公":("男"+m.ActiveSlotNo));
                    if(ret<0) return sh.io.Error("失敗しました");
                }
            }
            return 0;
        }else if(args[1]=="del"){
            if(args.Count<3) return sh.io.Error("男性を指定してください");
            PlacementWindow pw=StudioMode.GetPlacementWindow();
            List<Maid> del=new List<Maid>(args.Count-2);
            for(int i=2; i<args.Count; i++){
                Maid m=MaidUtil.FindMan(args[i]);
                if(m!=null) del.Add(m);
            }
            foreach(Maid dm in del){
                if(pw==null || dm.ActiveSlotNo==6) dm.Visible=false;
                else pw.DeActiveMaid(dm);
            }
            return 0;
        }
        string[] sa=args[1].Split(ParseUtil.colon);
        if(sa.Length>1 && sa[0]=="man") return CmdMaidSub(sh,sa[1],args,2);
        return CmdManSub(sh,args[1],args,2);
    }
	public static int CmdManSub(ComShInterpreter sh,string id, List<string> args,int prmstart) {
        Maid m=MaidUtil.FindMan(id);
        if(m==null||!m.Visible) return sh.io.Error("指定された男性は存在しません");
        if(args.Count==prmstart){
            sh.io.Print($"iid:{m.GetInstanceID().ToString()}\n");
            sh.io.Print($"guid:{m.status.guid}\n");
            sh.io.Print($"creditguid:{m.status.saveDataGuidGP03}\n");
            UTIL.PrintTrInfo(sh,m.transform);
            return 0;
        }
        return ParamLoop(sh,m,manParamDic,args,prmstart);
	}

    private static int MaidParamLPos(ComShInterpreter sh,Maid m,string val){
        return _CmdParamLPos(sh,m.transform,val);
    }
    private static int MaidParamLPosX(ComShInterpreter sh,Maid m,string val){
        return _CmdParamLPosX(sh,m.transform,val);
    }
    private static int MaidParamLPosY(ComShInterpreter sh,Maid m,string val){
        return _CmdParamLPosY(sh,m.transform,val);
    }
    private static int MaidParamLPosZ(ComShInterpreter sh,Maid m,string val){
        return _CmdParamLPosZ(sh,m.transform,val);
    }
    private static int MaidParamOPos(ComShInterpreter sh,Maid m,string val){
        return _CmdParamOPos(sh,m.transform,val);
    }
    private static int MaidParamWPos(ComShInterpreter sh,Maid m,string val){
        return _CmdParamWPos(sh,m.transform,val);
    }
    private static int MaidParamWPosX(ComShInterpreter sh,Maid m,string val){
        return _CmdParamWPosX(sh,m.transform,val);
    }
    private static int MaidParamWPosY(ComShInterpreter sh,Maid m,string val){
        return _CmdParamWPosY(sh,m.transform,val);
    }
    private static int MaidParamWPosZ(ComShInterpreter sh,Maid m,string val){
        return _CmdParamWPosZ(sh,m.transform,val);
    }
    private static int MaidParamLRot(ComShInterpreter sh,Maid m,string val){
        return _CmdParamLRot(sh,m.transform,val);
    }
    private static int MaidParamLRotX(ComShInterpreter sh,Maid m,string val){
        return _CmdParamLRotX(sh,m.transform,val);
    }
    private static int MaidParamLRotY(ComShInterpreter sh,Maid m,string val){
        return _CmdParamLRotY(sh,m.transform,val);
    }
    private static int MaidParamLRotZ(ComShInterpreter sh,Maid m,string val){
        return _CmdParamLRotZ(sh,m.transform,val);
    }
    private static int MaidParamLPosRot(ComShInterpreter sh,Maid m,string val){
        return _CmdParamLPosRot(sh,m.transform,val);
    }
    private static int MaidParamORot(ComShInterpreter sh,Maid m,string val){
        return _CmdParamORot(sh,m.transform,val);
    }
    private static int MaidParamWRot(ComShInterpreter sh,Maid m,string val){
        return _CmdParamWRot(sh,m.transform,val);
    }
    private static int MaidParamWRotX(ComShInterpreter sh,Maid m,string val){
        return _CmdParamWRotX(sh,m.transform,val);
    }
    private static int MaidParamWRotY(ComShInterpreter sh,Maid m,string val){
        return _CmdParamWRotY(sh,m.transform,val);
    }
    private static int MaidParamWRotZ(ComShInterpreter sh,Maid m,string val){
        return _CmdParamWRotZ(sh,m.transform,val);
    }
    private static int MaidParamWPosRot(ComShInterpreter sh,Maid m,string val){
        return _CmdParamWPosRot(sh,m.transform,val);
    }
    private static int MaidParamScale(ComShInterpreter sh,Maid m,string val){
        return _CmdParamScale(sh,m.transform,val);
    }
    private static int MaidParamScaleX(ComShInterpreter sh,Maid m,string val){
        return _CmdParamScaleX(sh,m.transform,val);
    }
    private static int MaidParamScaleY(ComShInterpreter sh,Maid m,string val){
        return _CmdParamScaleY(sh,m.transform,val);
    }
    private static int MaidParamScaleZ(ComShInterpreter sh,Maid m,string val){
        return _CmdParamScaleZ(sh,m.transform,val);
    }
    private static int MaidParamLQuat(ComShInterpreter sh,Maid m,string val){
        return _CmdParamLQuat(sh,m.transform,val);
    }
    private static int MaidParamWQuat(ComShInterpreter sh,Maid m,string val){
        return _CmdParamWQuat(sh,m.transform,val);
    }
    private static int MaidParamPQuat(ComShInterpreter sh,Maid m,string val){
        return _CmdParamPQuat(sh,m.transform,val);
    }
    private static int MaidParamPRot(ComShInterpreter sh,Maid m,string val){
        return _CmdParamPRot(sh,m.transform,val);
    }
    private static int MaidParamL2W(ComShInterpreter sh,Maid m,string val){
        return _CmdParamL2W(sh,m.transform,val);
    }
    private static int MaidParamW2L(ComShInterpreter sh,Maid m,string val){
        return _CmdParamW2L(sh,m.transform,val);
    }
    private static int SingleMotion(Maid m,MotionName.Clip clip,bool q,bool tailq,bool nolposq,bool cacheq,bool playq){
        string id=UTIL.Suffix(clip.name,".anm");
        int manbip=m.body0.trBip.name=="ManBip"?1:0;
        ClipCache.CacheData cache=ClipCache.Find(id,manbip);
        if(m.IsCrcBody && cache.clip==null){
            id="crc_"+id;
            cache=ClipCache.Find(id,manbip);
        }
        if(cache.clip==null){
            string name=clip.name;
            string filename;
            if(clip.tmpq==1){
                var tf=DataFiles.GetTempFile(name);
                if(tf==null) return -1;
                filename=tf.filename;
                id=tf.original;
            }else{
                id=name;
                name=UTIL.Suffix(name,".anm");
                filename=ComShInterpreter.myposeDir+name;
            }
            if(clip.layer>0 && clip.official_layer<0){
                string seq=UTIL.GetSeqId();
                id=$"{Path.GetFileNameWithoutExtension(id)}_l_{clip.layer}_#{seq}.anm";
            }else{
                id=UTIL.Suffix(id,".anm");
            }
            AnmFile af=null;
            try{
                if(clip.tmpq==1){   // temp file
                    if(File.Exists(filename)){
                        af=new AnmFile(filename,nolposq);
                        if(manbip!=af.gender) af.ChgGender();
                    }else af=null;
                }else{
                    if(File.Exists(filename)){ // mypose
                        af=new AnmFile(filename,nolposq);
                        if(manbip!=af.gender) af.ChgGender();
                        m.SetAutoTwistAll(true);
                    }else{
                        if(name.StartsWith("crc_")){
                            var buf=UTIL.AReadAll(name);
                            if(buf!=null){
                                af=new AnmFile(buf,nolposq);
                                if(manbip==1) af.ChgGender();
                            }
                            buf=null;
                        }else{
                            string name2;
                            if(m.IsCrcBody && GameUty.IsExistFile((name2="crc_"+name),GameUty.FileSystem)){
                                id="crc_"+id;
                                var buf=UTIL.AReadAll(name2);
                                if(buf!=null) af=new AnmFile(buf,nolposq);
                                buf=null;
                            }else{
                                var buf=UTIL.AReadAll(name);
                                if(buf!=null){
                                    af=new AnmFile(buf,nolposq);
                                if(manbip!=af.gender) af.ChgGender();
                                }
                                buf=null;
                            }
                        }
                    }
                }
            }catch(Exception e){ Debug.Log(e.ToString()); return -1; }
            if(af==null||af.IsEmpty()) return -1;
            cache=new ClipCache.CacheData(af.ToClip(),af.useMuneR,af.useMuneL);
            if(cacheq) ClipCache.Add(id,manbip,cache);
        }
        if(playq){
            if(clip.type==0 && q==false){ // ベースのモーションなら胸モーション有効/無効の判断
                if(!m.boMAN && cache.clip!=null){
                    int iid=m.GetInstanceID();
                    int yrq=5;
                    if(muneMotionYureq.ContainsKey(iid)) yrq=muneMotionYureq[iid];
                    int l=yrq>>2,r=yrq&3;
                    bool lq=(l==2||(l==1&&cache.yureL==0)),rq=(r==2||(r==1&&cache.yureR==0));
                    m.body0.SetMuneYureLWithEnable(lq);
                    m.body0.SetMuneYureRWithEnable(rq);
                }
            }
            return CrossFade(m,id,cache.clip,clip,q,tailq);
        }
        return 0;
    }
    private class ClipCache {
        public struct CacheData {
            public AnimationClip clip;
            public byte yureR;
            public byte yureL;
            public CacheData(AnimationClip c,byte r,byte l){clip=c;yureR=r;yureL=l;}
        }
        private static Dictionary<string,CacheData> f_clipdic=new Dictionary<string,CacheData>();
        private static Dictionary<string,CacheData> m_clipdic=new Dictionary<string,CacheData>();
        public static string Find(AnimationClip ac,int gender){
            var dic=(gender==1)?m_clipdic:f_clipdic;
            foreach(var kv in dic)
                if(kv.Value.clip==ac) return kv.Key;
            return null;
        }
        public static CacheData Find(string id,int gender){
            var dic=(gender==1)?m_clipdic:f_clipdic;
            if(dic.TryGetValue(id,out CacheData ret)) return ret;
            return default(CacheData);
        }
        public static void Add(string id,int gender,CacheData cd){
            var dic=(gender==1)?m_clipdic:f_clipdic;
            dic[id]=cd;
        }
        public static void Add(string id,int gender,AnimationClip clip,byte r,byte l){
            var dic=(gender==1)?m_clipdic:f_clipdic;
            dic[id]=new CacheData(clip,r,l);
        }
        public static bool Del(string id,int gender){
            var dic=(gender==1)?m_clipdic:f_clipdic;
            if(!dic.TryGetValue(id,out CacheData cd)) return false;
            GameObject.Destroy(cd.clip);
            return dic.Remove(id);
        }
        public static void List(ComShInterpreter sh,int gender){
            var dic=(gender==1)?m_clipdic:f_clipdic;
            foreach(var k in dic.Keys){
                sh.io.Print(k).Print(sh.ofs).PrintLn(gender==1?"ManBip":"Bip01");
            }
        }
    }

    private static FieldInfo fi_anist;
    private static int CrossFade(Maid m,string id,AnimationClip ac,MotionName.Clip clip,bool q,bool tailq){
        Animation anim=m.body0.m_Animation;
        ac.name=id;
        anim.AddClip(ac,id);
        AnimationState st=anim[id];
        st.blendMode=(clip.type=='&')?AnimationBlendMode.Additive:AnimationBlendMode.Blend;
        st.layer=clip.layer;
        st.speed=clip.speed;
        st.time=clip.time;
        st.wrapMode=(!tailq)?WrapMode.Once:Int2Wrap(clip.loop);
        if(clip.tr!=null) for(int k=0; k<clip.tr.Count; k++)
            st.AddMixingTransform(clip.tr[k].tr,clip.tr[k].single==0);
        if(q) anim.CrossFadeQueued(id,clip.fade,QueueMode.CompleteOthers);
        else anim.CrossFade(id,clip.fade);

        st.weight=clip.weight;
        if(st.layer==0){
            if(fi_anist==null){
                fi_anist=typeof(TBody).GetField("anist",BindingFlags.Instance|BindingFlags.NonPublic);
                if(fi_anist==null) return -1;
            }
            fi_anist.SetValue(m.body0,st);
        }
        m.body0.LastAnimeFN=id;
        return 0;
    }
    private static WrapMode Int2Wrap(int lp){
        if(lp==0) return WrapMode.Once;
        else if(lp==1) return WrapMode.Loop;
        else if(lp==2) return WrapMode.PingPong;
        else if(lp==3) return WrapMode.ClampForever;
        return 0;
    }
    private static int MaidParamMotion(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            var s=MaidUtil.GetCurrentMotion(m);
            if(s!="") sh.io.Print(s);
            return 0;
        }
        Animation anim=m.GetAnimation();
        if(val==""){
            GameMain.Instance.ScriptMgr.StopMotionScript();
            var remove=new List<string>(10);
            foreach(AnimationState state in anim) remove.Add(state.name);
            int gender=(m.boMAN&&!m.IsCrcBody)?1:0;
            foreach(var name in remove){
                var ac=anim.GetClip(name);
                anim.RemoveClip(name);
                if(ClipCache.Find(ac,gender)==null) UnityEngine.Object.Destroy(ac);
            }
            anim.AddClip(new AnimationClip(){legacy=true},m.body0.LastAnimeFN.ToLower());
            return 1;
        }
        if(val[0]!='+'&&val[0]!=':'&&val[0]!='&'){    // お掃除
            GameMain.Instance.ScriptMgr.StopMotionScript();
            var remove=new List<string>(10);
            foreach(AnimationState state in anim)
                if(state.layer>0||((!anim.IsPlaying(state.name))&&state.name.IndexOf(" - Queued Clone",Ordinal)<0))
                    remove.Add(state.name);
            int gender=(m.boMAN&&!m.IsCrcBody)?1:0;
            foreach(var name in remove){
                var ac=anim.GetClip(name);
                anim.RemoveClip(name);
                if(ClipCache.Find(ac,gender)==null) UnityEngine.Object.Destroy(ac);
            }
        }

        bool updated=false;
        var ml=ParseMotion(m,val);
        if(ml==null) return sh.io.Error("モーションの書式が不正です");
        for(int i=0; i<ml.list.Count; i++){
            var cl=ml.list[i];
            bool q=cl.type!=0;
            for(int j=0; j<cl.list.Count; j++){
                MotionName.Clip clip=cl.list[j];
                int ret=SingleMotion(m,clip,q,i==ml.list.Count-1,sh.env["_use_anm_lpos"]!="1",false,true);
                if(ret<0) return sh.io.Error("指定されたモーションが見つかりません");
                updated=true;
            }
        }
        if(updated) StudioMode.OnMotionChange(m);
        return 1;
    }
    private static int MaidParamMotionCache(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            ClipCache.List(sh,m.body0.trBip.name=="ManBip"?1:0);
            return 0;
        }
        MotionName.Clip clip=new MotionName.Clip();
        clip.name=val;
        int ret=SingleMotion(m,clip,false,false,sh.env["_use_anm_lpos"]!="1",true,false);
        if(ret<0) return sh.io.Error("指定されたモーションが見つかりません");
        return 1;
    }
    private static int MaidParamMotionCacheDel(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            ClipCache.List(sh,m.body0.trBip.name=="ManBip"?1:0);
            return 0;
        }
        ClipCache.Del(UTIL.Suffix(val,".anm"),m.body0.trBip.name=="ManBip"?1:0);
        return 1;
    }
    private static int MaidParamMotionTimeL(ComShInterpreter sh,Maid m,string val){
        return MaidParamMotionTimeSub(sh,m,val,true);
    }
    private static int MaidParamMotionTime(ComShInterpreter sh,Maid m,string val){
        return MaidParamMotionTimeSub(sh,m,val,false);
    }
    private static int MaidParamMotionTimeSub(ComShInterpreter sh,Maid m,string val,bool fullq){
        var anm=m.body0.m_Animation;
        if(val==null){
            bool multi=false;
            foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){ if(st.layer>0){multi=true; break;} }
            if(multi){
                foreach(AnimationState st in anm) if(anm.IsPlaying(st.name))
                    sh.io.PrintLn(st.layer.ToString()+':'+sh.fmt.FInt((st.time%st.length)*1000));
            }else{
                foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){
                    if(fullq) sh.io.PrintLn(sh.fmt.FInt(st.time*1000));
                    else sh.io.PrintLn(sh.fmt.FInt((st.time%st.length)*1000));
                    break;
                }
            }
            return 0;
        }
        string[] sa;
        if(val.IndexOf('\n')>=0) sa=val.Split(ParseUtil.lf);
        else sa=val.Split(ParseUtil.comma);
        for(int i=0; i<sa.Length; i++){
            int lno;
            float f;
            var lr=ParseUtil.LeftAndRight(sa[i],':');
            if(lr[1]==""){
                lno=-1;
                if(!float.TryParse(lr[0],out f)) return sh.io.Error("数値の指定が不正です");
            }else{
                if(!int.TryParse(lr[0],out lno) || lno<0) return sh.io.Error("レイヤ番号が不正です");
                if(!float.TryParse(lr[1],out f)) return sh.io.Error("数値の指定が不正です");
            }
            foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){
                if(lno<0 || lno==st.layer) st.time=f/1000;   // 強制的に%st.lengthされちゃう
            }
        }
        return 1;
    }
    private static int MaidParamMotionTimepL(ComShInterpreter sh,Maid m,string val){
        return MaidParamMotionTimepSub(sh,m,val,true);
    }
    private static int MaidParamMotionTimep(ComShInterpreter sh,Maid m,string val){
        return MaidParamMotionTimepSub(sh,m,val,false);
    }
    private static int MaidParamMotionTimepSub(ComShInterpreter sh,Maid m,string val,bool fullq){
        var anm=m.body0.m_Animation;
        if(val==null){
            bool multi=false;
            foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){ if(st.layer>0){multi=true; break;} }
            if(multi){
                foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){
                    if(fullq) sh.io.PrintLn(st.layer.ToString()+':'+sh.fmt.FInt(st.time/st.length));
                    else sh.io.PrintLn(st.layer.ToString()+':'+sh.fmt.FInt((st.time%st.length)/st.length));
                }
            }else{
                foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){
                    if(fullq) sh.io.PrintLn(sh.fmt.FInt(st.time/st.length));
                    else sh.io.PrintLn(sh.fmt.FInt((st.time%st.length)/st.length));
                    break;
                }
            }
            return 0;
        }
        string[] sa;
        if(val.IndexOf('\n')>=0) sa=val.Split(ParseUtil.lf);
        else sa=val.Split(ParseUtil.comma);
        for(int i=0; i<sa.Length; i++){
            int lno;
            float f;
            var lr=ParseUtil.LeftAndRight(sa[i],':');
            if(lr[1]==""){
                lno=-1;
                if(!float.TryParse(lr[0],out f)) return sh.io.Error("数値の指定が不正です");
            }else{
                if(!int.TryParse(lr[0],out lno) || lno<0) return sh.io.Error("レイヤ番号が不正です");
                if(!float.TryParse(lr[1],out f)) return sh.io.Error("数値の指定が不正です");
            }
            foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){
                if(lno<0 || lno==st.layer) st.time=f*st.length;
            }
        }
        return 1;
    }
    private static int MaidParamMotionFrame(ComShInterpreter sh,Maid m,string val){
        var anm=m.body0.m_Animation;

        AnimationState main=null;
        {
            int mainlno=int.MaxValue;
            foreach(AnimationState st in anm)
                if(anm.IsPlaying(st.name) && !Mathf.Approximately(st.speed,0) && !Mathf.Approximately(st.weight,0))
                    if(st.layer<mainlno){ mainlno=st.layer; main=st; }
            if(main==null) return sh.io.Error("有効なモーションがありません");
        }

        if(val==null){
            sh.io.PrintJoin("/",((int)((main.time%main.length)/main.speed*60)).ToString(),((int)(main.time/main.speed*60)).ToString(),((int)(main.length/main.speed*60)).ToString());
            return 0;
        }
        if(!float.TryParse(val,out float f)) return sh.io.Error("数値の指定が不正です");
        f=Mathf.Round(f);
        foreach(AnimationState st in anm)
            if(anm.IsPlaying(st.name) && !Mathf.Approximately(st.speed,0) && !Mathf.Approximately(st.weight,0))
                st.time=st.speed*(f/60);
        return 1;
    }
    private static int MaidParamMotionSpeed(ComShInterpreter sh,Maid m,string val){
        var anm=m.body0.m_Animation;
        if(val==null){
            bool multi=false;
            foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){ if(st.layer>0){multi=true; break;} }
            if(multi){
                foreach(AnimationState st in anm) if(anm.IsPlaying(st.name))
                    sh.io.PrintLn(st.layer.ToString()+':'+sh.fmt.FInt(st.speed));
            }else{
                foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){
                    sh.io.PrintLn(sh.fmt.FInt(st.speed));
                    break;
                }
            }
            return 0;
        }
        string[] sa;
        if(val.IndexOf('\n')>=0) sa=val.Split(ParseUtil.lf);
        else sa=val.Split(ParseUtil.comma);
        for(int i=0; i<sa.Length; i++){
            int lno;
            float f;
            var lr=ParseUtil.LeftAndRight(sa[i],':');
            if(lr[1]==""){
                lno=-1;
                if(!float.TryParse(lr[0],out f)) return sh.io.Error("数値の指定が不正です");
            }else{
                if(!int.TryParse(lr[0],out lno) || lno<0) return sh.io.Error("レイヤ番号が不正です");
                if(!float.TryParse(lr[1],out f)) return sh.io.Error("数値の指定が不正です");
            }
            foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){
                if(lno<0 || lno==st.layer) st.speed=f;
            }
        }
        return 1;
    }
    private static int MaidParamMotionWeight(ComShInterpreter sh,Maid m,string val){
        var anm=m.body0.m_Animation;
        if(anm==null) return sh.io.Error("処理に失敗しました");
        if(val==null){
            foreach(AnimationState st in anm) if(anm.IsPlaying(st.name))
                sh.io.PrintJoinLn(sh.ofs,st.layer.ToString(),sh.fmt.FInt(st.weight));
            return 0;
        }
        string[] sa;
        if(val.IndexOf('\n')>=0) sa=val.Split(ParseUtil.lf);
        else sa=val.Split(ParseUtil.comma);
        for(int i=0; i<sa.Length; i++){
            int lno;
            float f;
            var lr=ParseUtil.LeftAndRight(sa[i],':');
            if(lr[1]==""){
                lno=-1;
                if(!float.TryParse(lr[0],out f)||f<0) return sh.io.Error("数値の指定が不正です");
            }else{
                if(!int.TryParse(lr[0],out lno) || lno<0) return sh.io.Error("レイヤ番号が不正です");
                if(!float.TryParse(lr[1],out f)||f<0) return sh.io.Error("数値の指定が不正です");
            }
            foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){
                if(lno<0 || lno==st.layer) st.weight=f;
            }
        }
        return 1;
    }
    private static int MaidParamMotionLength(ComShInterpreter sh,Maid m,string val){
        var anm=m.body0.m_Animation;
        foreach(AnimationState st in anm) if(anm.IsPlaying(st.name) && st.layer==0){
            sh.io.Print(sh.fmt.FInt(st.length*1000));
            break;
        }
        return 0;
    }
    private static int MaidParamMotionLayer(ComShInterpreter sh,Maid m,string val){
        foreach(AnimationState ast in m.body0.m_Animation)
            if(m.body0.m_Animation.IsPlaying(ast.name)){
                string blendstr=(ast.blendMode==AnimationBlendMode.Additive)?"additive":"blend";
                sh.io.PrintJoinLn(sh.ofs, ast.layer+":"+ast.name, blendstr, "w"+ast.weight);
            }
        return 0;
    }
    private static int MaidParamMotionLayerDel(ComShInterpreter sh,Maid m,string val){
        if(val==null) return 0;
        if(!int.TryParse(val,out int no)||no<0) return sh.io.Error("数値が不正です");
        var anim=m.body0.m_Animation;
        int gender=(m.boMAN&&!m.IsCrcBody)?1:0;
        foreach(AnimationState ast in anim){
            if(ast.layer==no){
                var ac=ast.clip;
                anim.RemoveClip(ac);
                if(ClipCache.Find(ac,gender)==null) GameObject.Destroy(ac) ;
            }
        }
        if(anim.GetClipCount()==0) anim.AddClip(new AnimationClip(){legacy=true},m.body0.LastAnimeFN.ToLower());
        return 1;
    }
    private static int MaidParamMotionPause(ComShInterpreter sh,Maid m,string val){
        var anm=m.body0.m_Animation;
        if(val==null){
            foreach(AnimationState st in anm){sh.io.Print(st.enabled?"0":"1"); break;}
            return 0;
        }
        bool pauseq;
        if(val=="1") pauseq=true; else if(val=="0") pauseq=false; else return sh.io.Error("値が不正です");
        foreach(AnimationState st in anm) st.enabled=!pauseq;
        return 1;
    }

    public class MotionName{
        public List<ClipList> list=new List<ClipList>(8);

        public class ClipList{
            public int type=0;
            public List<Clip> list=new List<Clip>(8);
        }

        public class Clip{
            public int type=0;
            public byte tmpq=0;
            public string name;
            public float speed=1f;
            public float fade=0f;
            public int loop=1;
            public float time=0f;
            public float weight=1f;
            public int layer=0;
            public int official_layer=-1; // 公式の、ファイル名でレイヤ指定してあるもの
            public List<MixTr> tr;
        }
        public class MixTr{
            public Transform tr;
            public byte single;
            public MixTr(Transform t,byte m){tr=t;single=m;}
        }
    }
    private static char[] optLetters={'s','f','r','t','w','l'};
    private static MotionName ParseMotion(Maid m,string val){
        int q0=0;
        float[] dflt={1,0,1,0};
        var motions=new MotionName();
        for(int i=0; i<=val.Length; i++) if(i==val.Length || val[i]=='+'){
            if(q0==i) continue;
            var cl=new MotionName.ClipList();
            if(val[q0]=='+') cl.type=val[q0++];
            int m0=q0;
            for(int j=m0; j<=i; j++) if(j==i||val[j]=='&'||val[j]==':'){
                if(m0==j) continue;
                var c=new MotionName.Clip();
                if(val[m0]=='&'||val[m0]==':') c.type=val[m0++];
                if(m0==j) return null;
                int idx=val.IndexOf(',',m0,j-m0);
                int tridx;
                if(idx>=0){ // オプション指定がややこしくなったので新方式
                    if(val[m0]=='*'){
                        c.name=val.Substring(m0+1,idx-m0-1);
                        c.tmpq=1;
                    }else{
                        c.name=val.Substring(m0,idx-m0);
                        c.tmpq=0;
                    }
                    string optstr=val.Substring(idx+1,j-idx-1);
                    var sa=optstr.Split(ParseUtil.comma);
                    if(sa[0].Length>0 && char.IsLetter(sa[0][0])){
                        float[] values=new float[]{
                            dflt[0],dflt[1],dflt[2],dflt[3],
                            (c.type==':')?0.5f:1f, (c.type==0)?0f:2f
                        };
                        int ret=ParseUtil.GetLetterFloat(sa[0],optLetters,values);
                        if(ret<0||values[1]<0||values[2]<0||values[2]>3||values[3]<0||values[4]<0||values[5]<0) return null;
                        c.speed=values[0]; c.fade=values[1]; c.loop=(int)values[2];
                        c.time=values[3]; c.weight=values[4]; c.layer=(int)values[5];
                        tridx=1;
                    }else{  // 後方互換のための旧方式
                        var opts=ParseUtil.NormalizeParams(sa,new string[]{
                            dflt[0].ToString(), dflt[1].ToString(), dflt[2].ToString("F0"), dflt[3].ToString(),
                            (c.type==':')?"0.5":"1", (c.type==0)?"0":"2"
                        });
                        if(!float.TryParse(opts[0],out c.speed)
                            ||(!float.TryParse(opts[1],out c.fade)||c.fade<0)
                            ||(!int.TryParse(opts[2],out c.loop)||c.loop<0||c.loop>3)
                            ||(!float.TryParse(opts[3],out c.time)||c.time<0)
                            ||(!float.TryParse(opts[4],out c.weight)||c.weight<0)
                            ||(!int.TryParse(opts[5],out c.layer)||c.layer<0)) return null;
                        tridx=6;
                    }
                    if(c.type==0){ dflt[0]=c.speed; dflt[1]=c.fade; dflt[2]=c.loop; dflt[3]=c.time; }

                    if(sa.Length>tridx) for(int k=tridx; k<sa.Length; k++){
                        if(sa[k].Length==0) continue;
                        byte single=(byte)((sa[k][sa[k].Length-1]=='.')?1:0);
                        var nm=(single==1)?sa[k].Substring(0,sa[k].Length-1):sa[k];
                        var bn=BoneUtil.MaidBone.Find(m,nm);
                        if(bn==null||bn.boneTr==null) continue;
                        if(c.tr==null) c.tr=new List<MotionName.MixTr>(10);
                        c.tr.Add(new MotionName.MixTr(bn.boneTr,single));
                    }
                }else{
                    if(val[m0]=='*'){
                        c.name=val.Substring(m0+1,j-m0-1);
                        c.tmpq=1;
                    }else{
                        c.name=val.Substring(m0,j-m0);
                        c.tmpq=0;
                    }
                    c.speed=dflt[0]; c.fade=dflt[1]; c.loop=(int)dflt[2]; c.time=dflt[3];
                    c.weight=(c.type==':')?0.5f:1f; c.layer=(c.type==0)?0:2;
                }

                int layer=FindLayerNo(c.name); // ファイル名にレイヤ指定があればそれを強制
                if(layer>=0) c.layer=c.official_layer=layer;

                cl.list.Add(c);
                m0=j;
            }
            motions.list.Add(cl);
            q0=i;
        }
        return motions;
    }
    // poseizi_mmomi_2_f_fmune_l_5_.anmのようなファイルから5の部分(レイヤ指定)を得る
    private static int FindLayerNo(string name){
        string[] sa=name.Split('_');
        int n=sa.Length;
        if(n<4) return -1;
        if((sa[n-1]==".anm"||sa[n-1]=="") && sa[n-3]=="l" && int.TryParse(sa[n-2],out int layer)){
            return layer;
        }
        return -1;
    }
    private static int MaidParamPreset(ComShInterpreter sh,Maid m,string val){
        if(sh.IsSafeMode()) return sh.io.Error("この機能はセーフモードでは使用できません");
        if(m.IsCrcBody) return sh.io.Error("crcボディでは使用できません");
        CharacterMgr cm=GameMain.Instance.CharacterMgr;
        if(val==null){
		    string[] files = Directory.GetFiles(cm.PresetDirectory, "*.preset");
            if(files!=null) for(int i=0; i<files.Length; i++) sh.io.PrintLn(Path.GetFileNameWithoutExtension(files[i])); 

            UnityEngine.Object[] r=Resources.LoadAll("Preset/");
            for(int i=0; i<r.Length; i++) sh.io.PrintLn(r[i].name); 
            return 0;
        }
        int mode=-1;
        if(val.Length>2 && val[val.Length-2]==','){
            mode=val[val.Length-1]-'0';
            if(mode<0||mode>2) return sh.io.Error("読込モードは0～2で指定してください");
            val=val.Substring(0,val.Length-2);
        }
        CharacterMgr.Preset preset=null;
        string fname=cm.PresetDirectory+"\\"+UTIL.Suffix(val,".preset");
        TextAsset ta=Resources.Load<TextAsset>("Preset/" + val);
        if(ta!=null) preset=cm.PresetLoadFromResources(val);
        else if(File.Exists(fname)) preset=cm.PresetLoad(fname);
        Resources.UnloadAsset(ta);
        if(preset==null) return sh.io.Error("プリセットファイルが読み込めません");
        if(mode==0) preset.ePreType=CharacterMgr.PresetType.Body;
        else if(mode==1) preset.ePreType=CharacterMgr.PresetType.Wear;
        else if(mode==2) preset.ePreType=CharacterMgr.PresetType.All;
        // デフォルト(mode==-1)のときはファイル内のePreTypeに従う
        cm.PresetSet(m,preset);
        m.boAllProcPropBUSY=false;
        m.AllProcProp();
        StudioMode.FixMyposeIK(m);
        MaidUtil.FixGravity(m);
        return 1;
    }
    private static int MaidParamPresetSave(ComShInterpreter sh,Maid m,string val){
        if(m.IsCrcBody) return sh.io.Error("crcボディでは使用できません");
        CharacterMgr cm=GameMain.Instance.CharacterMgr;
        if(val==null){
		    string[] files = Directory.GetFiles(cm.PresetDirectory, "*.preset");
            if(files!=null) for(int i=0; i<files.Length; i++) sh.io.PrintLn(Path.GetFileNameWithoutExtension(files[i])); 
            return 0;
        }
        string[] sa=val.Split(ParseUtil.comma);
        CharacterMgr.PresetType type=CharacterMgr.PresetType.All;
        if(sa.Length==2){
            switch(sa[1]){
            case "0": type=CharacterMgr.PresetType.Body; break;
            case "1": type=CharacterMgr.PresetType.Wear; break;
            case "2": type=CharacterMgr.PresetType.All; break;
            }
        }
        byte[] bin=cm.PresetSaveNotWriteFile(m,type);
        try{
            File.WriteAllBytes(cm.PresetDirectory+"\\"+UTIL.Suffix(sa[0],".preset"),bin);
        }catch{return sh.io.Error("失敗しました");}
        return 1;
    }
    private static int MaidParamCloth(ComShInterpreter sh,Maid m,string val){
        return MaidParamClothSub(sh,m,val,true);
    }
    private static int MaidParamCloth2(ComShInterpreter sh,Maid m,string val){
        if(sh.IsSafeMode()) return sh.io.Error("この機能はセーフモードでは使用できません");
        return MaidParamClothSub(sh,m,val,false);
    }
    private static int MaidParamClothSub(ComShInterpreter sh,Maid m,string val,bool tmpq){
        if(val==null){
            string ret;
            if(m.boMAN){
                if((ret=MaidUtil.GetCloth(m,MPN.body))!=string.Empty) sh.io.Print($"body:{ret}\n");
                if((ret=MaidUtil.GetCloth(m,MPN.head))!=string.Empty) sh.io.Print($"head:{ret}\n");
            }else{
                foreach(MPN mpn in MaidUtil.mpnClothing)
                    if((ret=MaidUtil.GetCloth(m,mpn))!=string.Empty) sh.io.Print($"{mpn}:{ret}\n");
            }
            return 0;
        }
        if(val=="") return 1;
        string[] menus=val.Split(ParseUtil.comma);
        Maid current=m;
        for(int i=0; i<menus.Length; i++){
            string fname=UTIL.Suffix(menus[i],".menu");
            var mh=MaidUtil.ReadMenuHeader(fname);
            if(mh==null) return sh.io.Error("menuファイルの読込に失敗しました");
            if(current.boMAN){
                if(mh.cate=="body"||mh.cate=="head"){
                    if(current==m && current.pairMan!=null && current.IsCrcBody!=(mh.version>=2100&&fname.StartsWith("crc_",Ordinal))){
                        current=ManBodySwitch(current);
                        if(current==null) return sh.io.Error("失敗しました");
                    }
                    StudioMode.ManHeadBodyStudio(current,fname);
                    int hash=fname.ToLower().GetHashCode();
                    if(current.GetProp(mh.cate).nFileNameRID!=hash) current.SetProp(mh.cate,fname,hash);
                }else if(mh.cate=="def"){
                    if(fname.StartsWith("_i_man_",Ordinal)){
                        if(current==m&&current.pairMan!=null && current.IsCrcBody!=(mh.version>=2100&&fname.StartsWith("crc_",Ordinal))){
                            current=ManBodySwitch(current);
                            if(current==null) return sh.io.Error("失敗しました");
                        }
                        Menu.ProcScript(current,fname);
                    }
                }
            }else{
                if(mh.cate=="body"||(mh.cate=="head" && fname.StartsWith("mhead",Ordinal))) continue;
                m.SetProp(mh.cate,fname,0,tmpq);
                if(mh.cate=="eye_hi") m.SetProp(MPN.eye_hi_r,fname,0,tmpq);
            }
        }
        current.body0.FixMaskFlag();
        current.body0.FixVisibleFlag();
        current.boAllProcPropBUSY=false;
        current.AllProcProp();
        return 1;
    }
    private static int MaidParamBody(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            var pair=m.pairMan;
            sh.io.Print(m.status.guid).PrintLn(m.IsCrcBody?" crc":" old");
            if(pair!=null) sh.io.Print(pair.status.guid).Print(pair.IsCrcBody?" crc":" old");
            return 0;
        }
        Maid current=m;
        if(val=="crc"||val=="1"){
            if((current=ManBodyToNew(m))==null) return sh.io.Error("失敗しました");
        }else if(val=="old"||val=="0"){
            if(m.IsCrcBody) if((current=ManBodyToOld(m))==null) return sh.io.Error("失敗しました");
        }else return sh.io.Error("値が不正です");
        if(current!=m){
            if(current.boAllProcPropBUSY){
                current.boAllProcPropBUSY=false;
                current.AllProcProp();
            }
        }
        return 1;
    }
    private static Maid ManBodySwitch(Maid m){ return (m.IsCrcBody)?ManBodyToOld(m):ManBodyToNew(m); }
    private static Maid ManBodyToNew(Maid m){
        if(m.IsCrcBody) return m;
        CharacterMgr cm=GameMain.Instance.CharacterMgr;
        int ret=StudioMode.ManBodyChange(m,out Maid current);
        if(ret<0||current==null) return null;
        if(ret==0) current=cm.ToNewRealMan(m.ActiveSlotNo,m.HasNewRealMan);
        return current;
    }
    private static Maid ManBodyToOld(Maid m){
        if(!m.IsCrcBody) return m;
        CharacterMgr cm=GameMain.Instance.CharacterMgr;
        int ret=StudioMode.ManBodyChange(m,out Maid current);
        if(ret<0||current==null) return null;
        if(ret==0) current=cm.SwapNewManBody(m.ActiveSlotNo,false);
        return current;
    }
    private static int MaidParamHadake(ComShInterpreter sh,Maid m,string val){
        if(!m.IsCrcBody) return 0;
        if(val==null){
            string ret;
            if(MaidUtil.IsMekureZurashi(m,MaidUtil.HADAKE,MPN.wear,MPN.onepiece)) ret="on";
            else ret="off";
            sh.io.PrintLn(ret);
            return 0;
        }
        string fb=val.ToLower();
        if(fb=="on") MaidUtil.MekureZurashi(m,1,MaidUtil.HADAKE,MPN.wear,MPN.onepiece);
        else if(fb=="off"){
            MaidUtil.MekureZurashi(m,0,MaidUtil.HADAKE,MPN.wear,MPN.onepiece);
        }else return sh.io.Error("f|b|off で指定してください");
        return 1;

    }
    private static int MaidParamMekure(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            string ret;
            if(MaidUtil.IsMekureZurashi(m,MaidUtil.MEKUREF,MPN.skirt,MPN.onepiece)) ret="F";
            else if(MaidUtil.IsMekureZurashi(m,MaidUtil.MEKUREB,MPN.skirt,MPN.onepiece)) ret="B";
            else ret="off";
            sh.io.PrintLn(ret);
            return 0;
        }
        string fb=val.ToLower();
        if(fb=="f") MaidUtil.MekureZurashi(m,1,MaidUtil.MEKUREF,MPN.skirt,MPN.onepiece);
        else if(fb=="b") MaidUtil.MekureZurashi(m,1,MaidUtil.MEKUREB,MPN.skirt,MPN.onepiece);
        else if(fb=="off"){
            MaidUtil.MekureZurashi(m,0,MaidUtil.MEKUREF,MPN.skirt,MPN.onepiece);
            MaidUtil.MekureZurashi(m,0,MaidUtil.MEKUREB,MPN.skirt,MPN.onepiece);
        }else return sh.io.Error("f|b|off で指定してください");
        return 1;
    }
    private static int MaidParamMekureF(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            sh.io.PrintLn(sh.fmt.OnOff(MaidUtil.IsMekureZurashi(m,MaidUtil.MEKUREF,MPN.skirt,MPN.onepiece)));
            return 0;
        }
        int sw=ParseUtil.OnOff(val);
        if(sw<0) return sh.io.Error(ParseUtil.error);
        MaidUtil.MekureZurashi(m,sw,MaidUtil.MEKUREF,MPN.skirt,MPN.onepiece);
        return 1;
    }
    private static int MaidParamMekureB(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            sh.io.PrintLn(sh.fmt.OnOff(MaidUtil.IsMekureZurashi(m,MaidUtil.MEKUREB,MPN.skirt,MPN.onepiece)));
            return 0;
        }
        int sw=ParseUtil.OnOff(val);
        if(sw<0) return sh.io.Error(ParseUtil.error);
        MaidUtil.MekureZurashi(m,sw,MaidUtil.MEKUREB,MPN.skirt,MPN.onepiece);
        return 1;
    }
    private static int MaidParamZurashi(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            sh.io.PrintLn(sh.fmt.OnOff(MaidUtil.IsMekureZurashi(m,MaidUtil.ZURASI,MPN.panz,MPN.mizugi)));
            return 0;
        }
        int sw=ParseUtil.OnOff(val);
        if(sw<0) return sh.io.Error(ParseUtil.error);
        MaidUtil.MekureZurashi(m,sw,MaidUtil.ZURASI,MPN.panz,MPN.mizugi);
        return 1;
    }
    private static int MaidParamChinko(ComShInterpreter sh,Maid m,string val){
        if(m.body0==null) return 0;
        if(val==null){
            sh.io.Print(m.body0.GetChinkoVisible()?"1":"0");
            return 0;
        }
        if(val=="1"||val=="0") m.body0.SetChinkoVisible(val=="1");
        else if(val=="on"||val=="off") m.body0.SetChinkoVisible(val=="on");
        else return sh.io.Error("値が不正です");
        return 1;
    }
    private static int MaidParamShape(ComShInterpreter sh,Maid m,string val){
        TBody.Slot slot=m.body0.goSlot;
        TBodySkin skin;
        if(val==null){
            var dic=new SortedDictionary<string,float>();  // 重複削除とソート
            for(int i=0; i<slot.Count; i++) if ((skin=slot[i])!=m.body0.Face && skin.morph != null)
                foreach (string mk in skin.morph.hash.Keys) if(!dic.ContainsKey(mk))
                    dic[mk]=skin.morph.GetBlendValues((int)skin.morph.hash[mk]);
            foreach(string mk in dic.Keys) sh.io.PrintLn2(mk+":",sh.fmt.F0to1(dic[mk]));
            return 0;
        }
        var kvs=ParseUtil.GetKVFloat(val);
        if(kvs==null) return sh.io.Error(ParseUtil.error);
        for(int i=0; i<slot.Count; i++) if((skin=slot[i])!=m.body0.Face && skin.morph!=null){
            bool dirty=false;
            foreach(string dk in kvs.Keys){
                var lr=ParseUtil.LeftAndRight2(dk,'.');
                if(lr[0]!="" && skin.Category.ToLower()!=lr[0]) continue;
                if(skin.morph.hash.ContainsKey(lr[1])){
                    skin.morph.SetBlendValues((int)skin.morph.hash[lr[1]],kvs[dk]);
                    dirty=true;
                }
            }
            if(dirty) skin.morph.FixBlendValues();
        }
        return 1;
    }
    private static int MaidParamShapeVerList(ComShInterpreter sh,Maid m,string val){
        if(val==null) return 0;
        for(int i=0; i<m.body0.goSlot.Count; i++){
            TBodySkin skin=m.body0.goSlot[i];
            if(skin.morph==null || !skin.morph.hash.ContainsKey(val)) continue;
            int idx=(int)skin.morph.hash[val];
            var bd=skin.morph.BlendDatas[idx];
            int j;
            for(j=0; j<bd.v_index.Length; j++){
                if(bd.vert[j].magnitude<0.0005) continue;
                sh.io.Print($"{((TBody.SlotID)i).ToString()}:{bd.v_index[j].ToString()}");
                break;
            }
            for(j++; j<bd.v_index.Length; j++){
                if(bd.vert[j].magnitude<0.0005) continue;
                sh.io.Print(","+bd.v_index[j].ToString());
            }
            sh.io.PrintLn("");
        }
        return 0;
    }
        private static int MaidParamShapeRename(ComShInterpreter sh,Maid m,string val){
        if(val==null) return 0;
        if(m.body0==null || m.body0.goSlot==null || m.body0.goSlot.Count<1) return 1;
        string[] arr=val.Split(',');
        if(arr.Length!=3) return sh.io.Error("書式が不正です");
        string slot=arr[0],name1=arr[1],name2=arr[2];
        if(!m.body0.IsSlotNo(slot)) return sh.io.Error("スロット名が不正です");
        int idx=m.body0.GetSlotNo(slot);
        var skin=m.body0.goSlot[idx];
        if(skin!=null && skin.morph!=null && skin.morph.BlendDatas!=null){
            foreach(var bd in skin.morph.BlendDatas)
                if(bd!=null && bd.name!=null && bd.name==name1){
                    int v=(int)skin.morph.hash[name1];
                    skin.morph.hash.Remove(name1);
                    bd.name=name2;
                    skin.morph.hash[name2]=v;
                }
        }
        return 1;
    }
    private static int MaidParamShapeAdd(ComShInterpreter sh,Maid m,string val){
        if(val==null) return 0;
        if(m.body0==null || m.body0.goSlot==null || m.body0.goSlot.Count<2) return 1;
        string[] arr=val.Split('.');
        if(arr.Length!=2) return sh.io.Error("書式が不正です");
        string slot=arr[0],name=arr[1];
        if(!m.body0.IsSlotNo(slot)) return sh.io.Error("スロット名が不正です");
        int idx=m.body0.GetSlotNo(slot);
        var skin=m.body0.goSlot[idx];
        if(skin==null) return sh.io.Error("スロット名が不正です");
        if(skin.morph.hash.ContainsKey(name)) return sh.io.Error("その名前は既に使われています");

        var mi=new CmdMeshes.MeshInfo(skin.obj_tr,skin.obj_tr);
        if(mi==null||mi.oid==null) return sh.io.Error("メッシュが見つかりません");

        if(mi.oid.workMesh==null||mi.oid.workMesh.Count==0) return sh.io.Error("差分がありません");
        var mesh=mi.oid.workMesh[0].mesh;
        if(skin.m_OriVert==null||mesh==null) return sh.io.Error("差分がありません");
        var va0=skin.m_OriVert.vOriVert;
        var na0=skin.m_OriVert.vOriNorm;
        var va=mesh.vertices;
        var na=mesh.normals;
        List<int> il=new List<int>();
        List<Vector3> vl=new List<Vector3>();
        List<Vector3> nl=new List<Vector3>();
        for(int i=0; i<va0.Length; i++){
            Vector3 v=va[i]-va0[i];
            Vector3 n=na[i]-na0[i];

            foreach(var o in skin.morph.hash.Values){ // 他シェイプキーによる変位を除外
                int bi=(int)o;
                var f=skin.morph.GetBlendValues(bi);
                if(f<0.001&&f>-0.001) continue;
                var b=skin.morph.BlendDatas[bi];
                for(int j=0; j<b.v_index.Length; j++)
                    if(b.v_index[j]==i){ v-=b.vert[j]*f; n-=b.norm[j]*f; }
            }
            if(v!=Vector3.zero||n!=Vector3.zero){
                vl.Add(v); nl.Add(v); il.Add(i);
            }
        }
        if(il.Count==0) sh.io.Error("差分がありません");

        var bd=new BlendData();
        bd.name=name;
        bd.v_index=il.ToArray();
        bd.vert=vl.ToArray();
        bd.norm=nl.ToArray();
        if(AddNewBlendData(skin.morph,bd)<0) return sh.io.Error("失敗しました");
        skin.morph.SetBlendValues((int)skin.morph.hash[name],1);
        return 1;
    }
    private static string[] blendvaluefieldnames=new string[]{"BlendValues","BlendValuesTemp","BlendValuesBackup","BlendValuesCHK"};
    private static T[] extendArr<T>(T[] fa,int n){ T[] ret=new T[n]; fa.CopyTo(ret,0); return ret; }
    private static int AddNewBlendData(TMorph morph,BlendData bd){
        // morph.hash[name]は1オリジンなので注意
        morph.BlendDatas.Add(bd);
        morph.MorphCount++;
        morph.hash[bd.name]=morph.BlendDatas.Count-1;
        try{
            for(int i=0; i<blendvaluefieldnames.Length; i++){
                FieldInfo fi=typeof(TMorph).GetField(blendvaluefieldnames[i],BindingFlags.Instance | BindingFlags.NonPublic);
                float[] orig=(float[])fi.GetValue(morph);
                fi.SetValue(morph,extendArr(orig,morph.BlendDatas.Count));
            }
        }catch{return -1;}
        return 0;
    }
    private static int MaidParamShapeExport(ComShInterpreter sh,Maid m,string val){
        if(val==null) return 0;
        if(m.body0==null || m.body0.goSlot==null || m.body0.goSlot.Count<2) return 1;
        string[] arr=val.Split('.');
        if(arr.Length!=2) return sh.io.Error("書式が不正です");
        string slot=arr[0],name=arr[1];
        if(!m.body0.IsSlotNo(slot)) return sh.io.Error("スロット名が不正です");
        int idx=m.body0.GetSlotNo(slot);
        var skin=m.body0.goSlot[idx];
        if(skin==null) return sh.io.Error("スロット名が不正です");
        if(skin.morph==null||!skin.morph.hash.ContainsKey(name)) return sh.io.Error("シェイプキーが見つかりません");
        var bd=skin.morph.BlendDatas[(int)skin.morph.hash[name]];
        if(bd==null) return sh.io.Error("シェイプキーが見つかりません");
        if(ExportBlendData(sh.fmt,slot,bd)<0) return sh.io.Error("書き込みに失敗しました");
        return 1;
    }
    private static int ExportBlendData(ComShInterpreter.FMT fmt,string slot,BlendData bd){
        string datadir=ComShInterpreter.scriptFolder+"export\\";
        Directory.CreateDirectory(datadir);
        try{
            using(StreamWriter sw=new StreamWriter(datadir+$"shape_{slot}_{bd.name}",false,Encoding.UTF8)){
                sw.WriteLine(bd.name);
                if(bd.v_index.Length>0) sw.Write(bd.v_index[0].ToString());
                for(int i=1; i<bd.v_index.Length; i++){
                    sw.Write(' ');
                    sw.Write(bd.v_index[i].ToString());
                }
                sw.WriteLine();
                if(bd.vert.Length>0) sw.Write(fmt.FPos(bd.vert[0]));
                for(int i=1; i<bd.vert.Length; i++){
                    sw.Write(' ');
                    sw.Write(fmt.FPos(bd.vert[i]));
                }
                sw.WriteLine();
                if(bd.norm.Length>0) sw.Write(fmt.FPos(bd.norm[0]));
                for(int i=1; i<bd.norm.Length; i++){
                    sw.Write(' ');
                    sw.Write(fmt.FPos(bd.norm[i]));
                }
                sw.WriteLine();
            }
        }catch{return -1;}
        return 0;
    }
    private static int MaidParamStyleSub(ComShInterpreter sh,Maid m,string val,bool tmpq){
        if(val==null){
            for(int i=0; i<MaidUtil.mpnBody.Length; i++){
                MaidProp mp=m.GetProp(MaidUtil.mpnBody[i]);
                if(mp!=null){
                    int v=(mp.boTempDut||mp.boTempExecuted)?mp.temp_value:mp.value;
                    sh.io.PrintLn2(MaidUtil.mpnBody[i]+":",sh.fmt.F0to1(v));
                }
            }
            return 0;
        }
        Dictionary<string,float> kvs=ParseUtil.GetKVFloat(val);
        if(kvs==null) return sh.io.Error(ParseUtil.error);
        foreach(string k in kvs.Keys){
            string lk=k.ToLower();
            int i; for(i=0; i<MaidUtil.mpnBody.Length; i++) if(lk==MaidUtil.mpnBody[i].ToString().ToLower()) break;
            if(i==MaidUtil.mpnBody.Length) return sh.io.Error("MPNが不正です");
            if(tmpq) MaidUtil.SetPropTemp(m,MaidUtil.mpnBody[i],(int)kvs[k]);
            else  MaidUtil.SetProp(m,MaidUtil.mpnBody[i],(int)kvs[k]);
        }
        m.AllProcProp();
        return 1;
    }
    private static int MaidParamStyle(ComShInterpreter sh,Maid m,string val){ return MaidParamStyleSub(sh,m,val,true); }
    private static int MaidParamStyle2(ComShInterpreter sh,Maid m,string val){
        if(sh.IsSafeMode()) return sh.io.Error("この機能はセーフモードでは使用できません");
        return MaidParamStyleSub(sh,m,val,false);
    }
    private static int MaidParamLookAt(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            if(m.body0.boLockHeadAndEye){
                sh.io.PrintLn2(sh.ofs,"lock");
            }else{
                Transform trs=m.body0.trsLookTarget;
                if(trs!=null) sh.io.Print(sh.fmt.FPos(trs.position));
                int cam=(m.body0.boHeadToCam?4:0)+(m.body0.boEyeToCam?2:0)+(m.body0.boEyeSorashi?1:0);;
                if(trs==GameMain.Instance.MainCamera.transform){
                    if(cam==4) sh.io.Print(sh.ofs).Print("camera:0");
                    else if(cam==6) sh.io.Print(sh.ofs).Print("camera:2");
                    else if(cam==2) sh.io.Print(sh.ofs).Print("camera:3");
                }else if(trs==null){
                    if(cam==7) sh.io.Print(sh.ofs).Print("camera:1");
                    else if(cam==3) sh.io.Print(sh.ofs).Print("camera:4");
                }
            }
            return 0;
        }
        string[] te=val.Split(ParseUtil.colon);
        if(te[0]=="none"||te[0]=="off"){    // 注視なし
            m.LockHeadAndEye(false);
            MaidUtil.LookAtCron(m,null);
            m.EyeToReset();
            MaidUtil.LookTargetOff(m);
        }else if(te[0]=="lock"){  // 固定
            m.LockHeadAndEye(true);
            MaidUtil.LookAtCron(m,null);
        }else if(te[0]=="camera"){  // カメラを見る
            m.LockHeadAndEye(false);
            int n=2;
            if(te.Length>1 && (!int.TryParse(te[1],out n) || n>4 || n<0))
                return sh.io.Error("注視タイプは0～4で指定してください");
            MaidUtil.LookAtCron(m,null);
            m.EyeToCamera((Maid.EyeMoveType)(n+3)); // 顔だけ/顔そらす/目と顔/目だけ/目そらす
        }else if(te.Length>1){
            if(te[0]=="maid"||te[0]=="man"){  // メイド/男性を見る
                m.LockHeadAndEye(false);
                Transform tr=te.Length>2?ObjUtil.FindObj(sh,te):ObjUtil.FindObj(sh,new string[]{te[0],te[1],"BHead"});
                if(tr==null) return sh.io.Error("指定されたキャラ/ボーンが見つかりません");
                MaidUtil.LookAtCron(m,tr);
            }else if(te[0]=="obj"||te[0]==""){  // オブジェクトを見る
                m.LockHeadAndEye(false);
                Transform tr=ObjUtil.FindObj(sh,te);
                if(tr==null) return sh.io.Error("指定されたオブジェクトが見つかりません");
                MaidUtil.LookAtCron(m,tr);
            }else if(te[0]=="light"){  // ライトを見る
                m.LockHeadAndEye(false);
                Transform tr=LightUtil.FindLight(sh,te[1]);
                if(tr==null) return sh.io.Error("指定されたライトが見つかりません");
                MaidUtil.LookAtCron(m,tr);
            }else return sh.io.Error("注視先の指定が不正です");
        }else{                      // 座標を見る
            m.LockHeadAndEye(false);
            float[] xyz=ParseUtil.Xyz(te[0]);
            if(xyz==null) return sh.io.Error("注視座標が不正です");
            MaidUtil.LookAtCron(m,new Vector3(xyz[0],xyz[1],xyz[2]));
        }
        return 1;
    } 
    private static int MaidParamVoice(ComShInterpreter sh,Maid m,string val){
        var am=m.body0.AudioMan;
        if(val==null){
            if(am!=null && am.FileName!=null && am.FileName.Length>0){
                string playing=am.isPlay()?"再生中":"再生終了";
                sh.io.Print($"{am.FileName}{sh.ofs}{playing}");
            }
            return 0;
        }
        if(am==null) return sh.io.Error("そのメイドさんは発声できません(読込中など)");
        string[] sa=val.Split(ParseUtil.comma);
        if(sa.Length>3) return sh.io.Error("パラメータの書式が不正です");
        int n=0,ls=1;
        if(sa.Length>=2&&!int.TryParse(sa[1],out n)) return sh.io.Error("数値の指定が不正です");
        if(sa.Length==3&&!int.TryParse(sa[2],out ls)) return sh.io.Error("数値の指定が不正です");
        m.LipSyncEnabled(false);
        am.Stop();
        if(sa[0]!=string.Empty){
            m.LipSyncEnabled(ls==1);
            am.LoadPlay(UTIL.Suffix(sa[0],".ogg"),0,false,n==1);
        }
        return 1;
    }
    private static int MaidParamIK(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            int iid=m.GetInstanceID();
            if(!MaidUtil.ikObj.ContainsKey(iid)) return 0;
            string[] oname=MaidUtil.ikObj[iid];
            Transform tr;
            if(!string.IsNullOrEmpty(oname[0]))
                if((tr=MaidUtil.GetGrabTarget(m,"左手"))!=null)
                    sh.io.PrintLn2("左手:",sh.fmt.FPos(tr.position));
            if(!string.IsNullOrEmpty(oname[1]))
                if((tr=MaidUtil.GetGrabTarget(m,"右手"))!=null)
                    sh.io.PrintLn2("右手:",sh.fmt.FPos(tr.position));
            if(!string.IsNullOrEmpty(oname[2]))
                if((tr=MaidUtil.GetGrabTarget(m,"左足"))!=null)
                    sh.io.PrintLn2("左足:",sh.fmt.FPos(tr.position));
            if(!string.IsNullOrEmpty(oname[3]))
                if((tr=MaidUtil.GetGrabTarget(m,"右足"))!=null)
                    sh.io.PrintLn2("右足:",sh.fmt.FPos(tr.position));
            return 0;
        }
        if(val=="" ||val=="none"||val=="off") return ClearIK(sh,m);
        else{
            string[] te=val.Split(ParseUtil.comma); // 左手、右手、左足、右足
            if(te.Length>0){
                string[] te2=ParseUtil.NormalizeParams(te,new string[]{"","","",""});
                if(te2==null) return sh.io.Error(ParseUtil.error);
                int i; for(i=0; i<te2.Length; i++) if(te2[i].Length>0) break;
                if(i==te2.Length) return sh.io.Error("対象を１つ以上指定してください");
                string ret=SetIK(sh,m,te2);
                if(ret!="") return sh.io.Error(ret);
            }
        }
        return 1;
    }
    private static int MaidParamUndress(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            for(int i=0; i<MaidUtil.sidClothing.Length; i++)
                if(MaidUtil.GetCloth(m,MaidUtil.sid2mpnClothing[i])!=string.Empty){ // もともと何も着ていない部位は表示しない
                    TBody.SlotID sid=MaidUtil.sidClothing[i];
                    string shown=m.body0.GetMask(sid)?"着":"脱";
                    sh.io.Print($"{sid}:{shown}\n");
                }
            return 0;
        }
        string[] lk=val.Split(ParseUtil.comma);
        for(int i=0;i<lk.Length; i++){
            char f=lk[i][lk[i].Length-1];
            if(f=='-'||f=='+'){
                string sltnm=lk[i].Substring(0,lk[i].Length-1);
                if(!m.body0.IsSlotNo(sltnm)) return sh.io.Error("スロット名が不正です");
                m.body0.SetMask((TBody.SlotID)m.body0.GetSlotNo(sltnm),f=='+');
            }else{
                if(!m.body0.IsSlotNo(lk[i])) return sh.io.Error("スロット名が不正です");
                TBody.SlotID sid=(TBody.SlotID)m.body0.GetSlotNo(lk[i]);
                m.body0.SetMask(sid,!m.body0.GetMask(sid));
            }
        }
        return 1;
    }
    private static int MaidParamNode(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            for(int i=0; i<m.body0.goSlot.Count; i++)
                foreach(var nd in m.body0.goSlot[i].m_dicDelNodeBody)
                    sh.io.PrintLn($"{m.body0.goSlot[i].Category}.{ParseUtil.CompactBoneName(nd.Key)}{(nd.Value?'+':'-')}");
            return 0;
        }
        string[] sa=val.Split(ParseUtil.comma);
        for(int i=0; i<sa.Length; i++){
            var t=sa[i];
            if(t.Length==0) continue;
            var p=t.Substring(t.Length-1,1);
            if(p!="+" && p!="-"){ p="+"; } else t=t.Substring(0,t.Length-1);
            string[] lr=ParseUtil.LeftAndRight(t,'.');
            if(lr[1]=="") m.body0.SetVisibleNodeSlot("body",0,p[0]=='+',lr[0]);
            else{
                if(!m.body0.IsSlotNo(lr[0])) return sh.io.Error("スロット名が不正です");
                m.body0.SetVisibleNodeSlot(lr[0],0,p[0]=='+',ParseUtil.CompleteBoneName(lr[1],false));
            }
        }
        m.body0.FixMaskFlag();
        m.body0.FixVisibleFlag();
        m.AllProcProp();
        return 1;
    }
    private static int MaidParamPartsNode(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            for(int i=0; i<m.body0.goSlot.Count; i++)
                foreach(var np in m.body0.goSlot[i].m_dicDelNodeParts)
                    foreach(var bn in (Dictionary<string,bool>)np.Value)
                        sh.io.PrintLn($"{m.body0.goSlot[i].Category}.{np.Key}.{ParseUtil.CompactBoneName(bn.Key)}{(bn.Value?'+':'-')}");
            return 0;
        }
        string[] sa=val.Split(ParseUtil.comma);
        for(int i=0; i<sa.Length; i++){
            var t=sa[i];
            if(t.Length==0) continue;
            var p=t.Substring(t.Length-1,1);
            if(p!="+" && p!="-"){ p="+"; } else t=t.Substring(0,t.Length-1);
            string[] sa2=t.Split(ParseUtil.period);
            if(sa2.Length!=3) return sh.io.Error("書式が不正です");
            if(!m.body0.IsSlotNo(sa2[0])||!m.body0.IsSlotNo(sa2[1]))
                return sh.io.Error("スロット名が不正です");
            string bname=ParseUtil.CompleteBoneName(sa2[2],m.boMAN,m.IsCrcBody);
            m.body0.SetVisibleNodeSlotParts(sa2[0],0,sa2[1],p[0]=='+',bname);
        }
        m.body0.FixMaskFlag();
        m.body0.FixVisibleFlag();
        m.AllProcProp();
        return 1;
    }
    private static int MaidParamMask(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            var rev=new Dictionary<int,string>();
            foreach(System.Collections.DictionaryEntry ent in TBody.hashSlotName) rev[(int)ent.Value]=((string)ent.Key).ToLower();
            foreach(string slotname in TBody.hashSlotName.Keys){
                var skin=m.body0.GetSlot(slotname);
                if(skin==null||skin.obj==null) continue;
                foreach(int slot in skin.listMaskSlot) sh.io.PrintLn($"{slotname.ToLower()}.{rev[slot]}");
            }
            return 0; 
        }
        string[] sa=val.Split(ParseUtil.comma);
        for(int i=0; i<sa.Length; i++){
            var t=sa[i];
            if(t.Length==0) continue;
            var p=t.Substring(t.Length-1,1);
            if(p!="+" && p!="-"){ p="+"; } else t=t.Substring(0,t.Length-1);
            string[] lr=ParseUtil.LeftAndRight(t,'.');
            if(!m.body0.IsSlotNo(lr[0])||!m.body0.IsSlotNo(lr[1])) return sh.io.Error("スロット名が不正です");

            var skin=m.body0.GetSlot(lr[0]);
            if(skin==null||skin.obj!=null){
                int no=m.body0.GetSlotNo(lr[1]);
                int idx=skin.listMaskSlot.IndexOf(no);
                if(idx>=0){
                    if(p[0]=='-') skin.listMaskSlot.RemoveAt(idx);
                }else{
                    if(p[0]=='+') m.body0.AddMask(lr[0],0,lr[1]);
                }
            }
        }
        m.body0.FixMaskFlag();
        m.body0.FixVisibleFlag();
        m.AllProcProp();
        return 1;
    }
    private static int MaidParamFaceset(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            sh.io.PrintLn(m.ActiveFace);
            return 0;
        }
        TBodySkin skin=m.body0.Face;
        if(skin==null || skin.morph==null) return sh.io.Error("処理に失敗しました");
        string fs=val;
        float delay=0f;
        int idx=val.IndexOf(',');
        if(idx>0) {
            fs=val.Substring(0,idx);
            if(!float.TryParse(val.Substring(idx+1),out delay)||delay<0) return sh.io.Error("数値の形式が不正です");
        }
        if(!skin.morph.dicBlendSet.ContainsKey(fs)) return sh.io.Error("指定された表情セットは存在しません"); 
        m.FaceAnime(fs,delay);
        m.FaceBlend("オリジナル");
        return 1;
    }
    private static int MaidParamFaceSave(ComShInterpreter sh,Maid m,string val){
        var fw=StudioMode.GetWindow<FaceWindow>(PhotoWindowManager.WindowType.Face);
        if(fw==null) return sh.io.Error("このコマンドはスタジオモード専用です");
        fw.FaceMorphInput.CreateBackupData(m);
        return 1;
    }
    private static int MaidParamFace(ComShInterpreter sh,Maid m,string val){
        string suffix="";
        TBodySkin face=m.body0.Face;
        if (face==null || face.morph==null) return sh.io.Error("処理に失敗しました");
        if(face.morph.bodyskin.PartsVersion>=120) suffix=TMorph.crcFaceTypesStr[(int)face.morph.GetFaceTypeGP01FB()];
        if(val==null){
            var dic=new SortedDictionary<string,float>();  // ソート
            if(face!=null&&face.morph!=null){
                foreach (string dk in face.morph.hash.Keys)
                    dic[dk]=face.morph.GetBlendValues((int)face.morph.hash[dk]);
                if(suffix!=""){
                    for(int i=1; i<=8; i++){
                        string oldkey="eyeclose"+(i==1?"":i.ToString());
                        string fbkey="eyeclose"+i.ToString()+suffix;
                        if(dic.ContainsKey(fbkey) && !dic.ContainsKey(oldkey)) dic[oldkey]=dic[fbkey];
                    }
                }
                foreach(string dk in dic.Keys) sh.io.PrintLn2($"{dk}:",sh.fmt.F0to1(dic[dk]));
            }
            return 0;
        }
        if(m.FaceName3=="") m.FaceBlend("オリジナル");
        var kvs=ParseUtil.GetKVFloat(val);
        if(kvs==null) return sh.io.Error(ParseUtil.error);
        var hash=face.morph.hash;
        foreach(string dk in kvs.Keys){
            if(hash.ContainsKey(dk)){
                if(MaidUtil.origBSKeySet.Contains(dk)) face.morph.dicBlendSet["オリジナル"][(int)hash[dk]]=kvs[dk];
                else face.morph.SetValueBlendSet(m.ActiveFace,dk,kvs[dk]*100);
            }else if(dk.StartsWith("eyeclose",Ordinal) && suffix!=""){
                string dk2=(dk.Length==8)?(dk+"1"+suffix):(dk+suffix);
                if(hash.ContainsKey(dk2))
                    face.morph.SetValueBlendSet(m.ActiveFace,dk2,kvs[dk]*100);
            }
        }
        return 1;
    }
    private static int MaidParamBlink(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            sh.io.Print(m.boMabataki?"on":"off");
            return 0;
        }
        int sw=ParseUtil.OnOff(val);
        if(sw<0) return sh.io.Error(ParseUtil.error);
        m.MabatakiUpdateStop=(sw==0);
        return 1;
    }
    private static int MaidParamSing(ComShInterpreter sh,Maid m,string val){
        if(val==null) return 0;
        m.StopKuchipakuPattern();
        ComShBg.cron.KillJob("maidsing/"+m.GetInstanceID().ToString());
        if(val==string.Empty) return 1;

        string ptn;

        byte[] ba=UTIL.AReadAll(val);
        if(ba==null) ba=UTIL.AReadAll(val+".txt");
        if(ba!=null){
            ptn=System.Text.Encoding.ASCII.GetString(ba);
        }else{
            TextAsset ta=Resources.Load("SceneDance/"+val) as TextAsset;
            if(ta==null || ta.text=="") return sh.io.Error("口パクデータが取得できません");
            ptn=ta.text;
        }

        m.LipSyncEnabled(true);
        m.StartKuchipakuPattern(0,weat(ptn),true);
        // BGMを時間基準にして口パク更新
        ComShBg.cron.AddJob("maidsing/"+m.GetInstanceID().ToString(),0,0,(t)=>{
            if(m.body0.m_Bones==null) return -1; // メイドさんが削除された
            AudioSource asrc=GameMain.Instance.SoundMgr.GetAudioSourceBgm();
            if(asrc==null||!asrc.isPlaying){ m.StopKuchipakuPattern(); return -1; } // 歌終了
            m.FoceKuchipakuUpdate(asrc.time);
            return 0;
        });
        return 1;
    }
    private static string weat(string txt){
        int ci=0;
        char[] ca=new char[txt.Length];
        for(int i=0; i<txt.Length; i++){
            if(txt[i]=='\n'||txt[i]=='\r'||txt[i]==' ') continue;
            ca[ci++]=txt[i];
        }
        return new String(ca,0,ci);
    }
    private static string kuchipakudir=ComShInterpreter.scriptFolder+@"kuchipaku\";
    private static int MaidParamSing2(ComShInterpreter sh,Maid m,string val){
       if(val==null) return 0;
        m.StopKuchipakuPattern();
        ComShBg.cron.KillJob("maidsing2/"+m.GetInstanceID().ToString());

        string path=UTIL.GetFullPath(val,kuchipakudir);
        if(path=="") return sh.io.Error("ファイル名が不正です");
        string mml="";
        try{ mml=File.ReadAllText(path); }catch{}
        if(mml=="") return sh.io.Error("ファイルが読み込めません");

        Kuchipaku kp=Kuchipaku.Create(m);
        if(kp.SetMML(mml)<0) return sh.io.Error("MMLが不正です");

        m.LipSyncEnabled(false);
        // BGMを時間基準にして口パク更新
        ComShBg.cron.AddJob("maidsing2/"+m.GetInstanceID().ToString(),0,0,(t)=>{
            if(m.body0.m_Bones==null) return -1; // メイドさんが削除された
            AudioSource asrc=GameMain.Instance.SoundMgr.GetAudioSourceBgm();
            if(asrc==null||!asrc.isPlaying) return -1; // 歌終了
            kp.UpdateShapes(m,asrc.time*1000);
            return 0;
        });
        return 1;
    }
    private static int MaidParamSingMML(ComShInterpreter sh,Maid m,string val){
       if(val==null) return 0;
        m.StopKuchipakuPattern();
        ComShBg.cron.KillJob("maidsing2/"+m.GetInstanceID().ToString());
        if(val==string.Empty) return 1;

        Kuchipaku kp=Kuchipaku.Create(m);
        if(kp.SetMML(val)<0) return sh.io.Error("MMLが不正です");

        m.LipSyncEnabled(false);
        // 実時間で口パク更新
        long stime=DateTime.UtcNow.Ticks;
        ComShBg.cron.AddJob("maidsing2/"+m.GetInstanceID().ToString(),0,0,(t)=>{
            long cur=(t-stime)/TimeSpan.TicksPerMillisecond;
            if(m.body0.m_Bones==null) return -1; // メイドさんが削除された
            if(kp.UpdateShapes(m,cur)!=0) return -1;
            return 0;
        });
        return 1;
    }
    private static int MaidParamSingKeys(ComShInterpreter sh,Maid m,string val){
        Kuchipaku kp;
        if(val==null){
            kp=Kuchipaku.Find(m);
            if(kp!=null) sh.io.Print(String.Join(',',kp.vowelkeys));
            return 0;
        }
        kp=Kuchipaku.Create(m);
        kp.SetKeys(val.Split(','));
        return 1;
    }
    private static int MaidParamSingVowel(ComShInterpreter sh,Maid m,string val){
        Kuchipaku kp;
        if(val==null){
            kp=Kuchipaku.Find(m);
            if(kp==null) return 0;
            for(int i=0; i<6; i++){
                sh.io.Print("あいうえおん"[i]).Print(':');
                var arr=kp.aiueon[i];
                if(arr.Length>0) sh.io.Print(sh.fmt.FVal(arr[0]));
                for(int j=1; j<arr.Length; j++)
                    sh.io.Print(',').Print(sh.fmt.FVal(arr[j]));
                sh.io.PrintLn("");
            }
            return 0;
        }
        var lines=val.Split('\n');
        for(int i=0; i<lines.Length; i++){
            var lr=ParseUtil.LeftAndRight(lines[i],ParseUtil.eqcln);
            if(lr[0].Length!=1) return sh.io.Error("書式が不正です");
            char c=lr[0][0];
            var fa=ParseUtil.FloatArr(lr[1]);
            if(fa==null) return sh.io.Error("数値が不正です");

            kp=Kuchipaku.Create(m);
            int ret=kp.SetVowel(c,fa);
            if(ret==-1) return sh.io.Error("母音が不正です");
            if(ret==-2) return sh.io.Error("要素数が不正です");
        }
        return 1;
    }
    private static int MaidParamIid(ComShInterpreter sh,Maid m,string val){
        sh.io.PrintLn(m.GetInstanceID().ToString());
        return 0;
    }
    private static int MaidParamAttach(ComShInterpreter sh,Maid m,string val){
        if(val==null) return 0;
        Transform tr;
        int opt,jmpq=0;
        if((opt=val.IndexOf(','))>=0){
            if(!int.TryParse(val.Substring(opt+1),out jmpq)) return sh.io.Error("数値の形式が不正です");
            val=val.Substring(0,opt);
        }
        string[] sa=val.Split(ParseUtil.colon);
        tr=ObjUtil.FindObj(sh,sa);
        if(tr==null) return sh.io.Error("対象がみつかりません");

        // tarnsform.parentでつなぐと親メイド退場時に心中してしまうのでcronでやる
        var ms=MaidUtil.GetParentMaidList(tr,m.transform);
        if(ms==null) return sh.io.Error("親子関係がループになるため、アタッチできません");
        int prio=1;
        for(int i=ms.Count-1; i>=0; i--)    // 親ほど先になるようcron実行順を更新
            ComShBg.cron.ChangePriority(MaidUtil.CRON_ATTACH+ms[i].GetInstanceID().ToString(),prio++);
        if(jmpq==2) UTIL.ResetTr(m.transform);
        MaidUtil.AttachCron(m,tr,prio,jmpq==0);
        (ms=new List<Maid>(1)).Add(m);
        while((ms=MaidUtil.GetChildMaidList(ms)).Count>0){ // 子メイド達も階層ごとに更新
            foreach(Maid maid in ms)
                ComShBg.cron.ChangePriority(MaidUtil.CRON_ATTACH+maid.GetInstanceID().ToString(),prio);
            prio++;
        }
        return 1;
    }
    private static int MaidParamDetach(ComShInterpreter sh,Maid m,string val){
        Transform attbase=m.transform.parent;
        if(attbase.name==MaidUtil.NODE_ATTACH_PFX+m.GetInstanceID()){
            m.transform.SetParent(attbase.parent,true);
            UnityEngine.Object.Destroy(attbase.gameObject);
            ComShBg.cron2.KillJob(MaidUtil.CRON_ATTACH+m.GetInstanceID().ToString());
        }else return sh.io.Error("アタッチされていません");
        return 1;
    }

    private static int MaidParamList(ComShInterpreter sh,Maid m,string val){
        List<string> ls=new List<string>();
        bool sortq=true;
        if(val=="ap"){
            var slot=m.body0.goSlot;
            for(int i=0; i<slot.Count; i++){
                if(slot[i].morph==null) continue;
                foreach(string ap in slot[i].morph.dicAttachPoint.Keys) ls.Add(ap);
            }
        }else if(val=="bone"){
            CharacterMgr cm=GameMain.Instance.CharacterMgr;
            if(cm.TryGetCacheObject(m.body0.goSlot[0].m_strModelFileName,out GameObject go)){
                UTIL.TraverseTr(go.transform,(Transform tr,int d)=>{
                    if(tr.name=="ST_Root") return 1;
                    ls.Add(tr.name);
                    return 0;
                },false);
            }
        }else if(val=="all"){
            // 絶対長いのでログへ
            UTIL.TraverseTr(m.transform,(Transform tr,int d)=>{ ls.Add(tr.name); return 0; });
            sortq=false;
        }else return sh.io.Error("ap、boneのいずれかを指定してください");
        if(sortq) ls.Sort();
        foreach(string s in ls) sh.io.PrintLn(s);
        return 0;
    }
    private static int MaidParamGravityH(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            GravityTransformControl gtc=MaidUtil.GetGravity(m,"hair");
            if(!gtc.isEnabled) sh.io.PrintLn("off");
            else sh.io.Print($"{sh.fmt.FPos(gtc.transform.localPosition)} {sh.fmt.FInt(gtc.forceRate)}\n");
            return 0;
        }
        string ret=GravityCommon(m,"hair",val);
        if(ret!="") return sh.io.Error(ret);
        return 1;
    }
    private static int MaidParamGravityS(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            GravityTransformControl gtc=MaidUtil.GetGravity(m,"skirt");
            if(!gtc.isEnabled) sh.io.PrintLn("off");
            else sh.io.Print($"{sh.fmt.FPos(gtc.transform.localPosition)} {sh.fmt.FInt(gtc.forceRate)}\n");
            return 0;
        }
        string ret=GravityCommon(m,"skirt",val);
        if(ret!="") return sh.io.Error(ret);
        return 1;
    }
    private static Dictionary<int,int> muneMotionYureq=new Dictionary<int,int>();
    private static int MaidParamMuneYure(ComShInterpreter sh,Maid m,string val){
        // maidのinstance idで管理。メイド配置解除→再配置しても値を引き継いでしまうが、
        // かといってbodyのinstance idで管理すると掃除が必要になるので、まぁこれでいいでしょ
        int iid=m.GetInstanceID();
        int l,r;
        char lc,rc;
        if(val==null){
            int lr=5;
            if(muneMotionYureq.ContainsKey(iid)) lr=muneMotionYureq[iid];
            l=lr>>2; r=lr&3;
            if(l==0) lc='x'; else if(l==1) lc='-'; else lc='o';
            if(r==0) rc='x'; else if(r==1) rc='-'; else rc='o';
            sh.io.Print($"{lc}{rc}");
            return 0;
        }
        if(val.Length!=2) return sh.io.Error("値の形式が不正です");
        lc=val[0]; rc=val[1];
        if(lc=='X'||lc=='x') l=0; else if(lc=='-') l=1; else if(lc=='O'||lc=='o') l=2; else l=-1;
        if(rc=='X'||rc=='x') r=0; else if(rc=='-') r=1; else if(rc=='O'||rc=='o') r=2; else r=-1;
        if(l<0||r<0) return sh.io.Error("値の形式が不正です");
        muneMotionYureq[iid]=l*4+r;
        bool lq=l>1, rq=r>1;
        m.body0.SetMuneYureLWithEnable(lq);
        m.body0.SetMuneYureRWithEnable(rq);
        return 1;
    }

    private static int MaidParamAutoTwist(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            sh.io.Print(new string(new char[]{
                m.body0.boAutoTwistShoulderL?'o':'x',
                m.body0.boAutoTwistShoulderR?'o':'x',
                m.body0.boAutoTwistWristL?'o':'x',
                m.body0.boAutoTwistWristR?'o':'x',
                m.body0.boAutoTwistThighL?'o':'x',
                m.body0.boAutoTwistThighR?'o':'x'
            }));
            return 0;
        }
        if(val.Length!=6) return sh.io.Error("値の形式が不正です");
        char[] ca=val.ToCharArray();
        for(int i=0; i<6; i++) if(ca[i]!='o'&&ca[i]!='x') return sh.io.Error("値の形式が不正です");
        m.body0.boAutoTwistShoulderL=(ca[0]=='o');
        m.body0.boAutoTwistShoulderR=(ca[1]=='o');
        m.body0.boAutoTwistWristL=(ca[2]=='o');
        m.body0.boAutoTwistWristR=(ca[3]=='o');
        m.body0.boAutoTwistThighL=(ca[4]=='o');
        m.body0.boAutoTwistThighR=(ca[5]=='o');
        return 1;
    }
    private static int MaidParamSelect(ComShInterpreter sh,Maid m,string val){
        var pw=StudioMode.GetPlacementWindow();
        if(pw==null) return sh.io.Error("スタジオモードでのみ有効です");
        if(m.boMAN){
            int no=m.ActiveSlotNo;
            string manname=(no==0)?"主人公":$"男{no}";
            return StudioMode.MaidSelectStudio(pw,"",manname);
        }else{
            return StudioMode.MaidSelectStudio(pw,m.status.lastName,m.status.firstName);
        }
    }

    private static Dictionary<int,Action> lateDic=new Dictionary<int, Action>();
    private static int MaidParamLater(ComShInterpreter sh,Maid m,string val){
        if(val==null) return 0;

        var sbo=new ComShInterpreter.SubShOutput();
        var subsh=new ComShInterpreter(new ComShInterpreter.Output(sbo.Output),sh.env,sh.func,sh.ns);
        subsh.env[ComShInterpreter.SCRIPT_ERR_ON]="1";

        var psr=Command.EvalParser(sh,Command.currentArgNo+1,false,sh.currentParser.lineno);
        if(psr==null) return -1;

        Action act;
        int iid=m.GetInstanceID();
        if(lateDic.TryGetValue(iid,out act)){
            if(m.body0!=null) m.body0.OnLateUpdate-=act;
            lateDic.Remove(iid);
        }

        if(psr.sta.Count==0) return 1; // 空→登録削除のみ

        long stime=DateTime.UtcNow.Ticks;
        subsh.env.args.Clear();
        subsh.env.args.Add("");
        act=()=>{
            subsh.env.args[0]=((DateTime.UtcNow.Ticks-stime)/TimeSpan.TicksPerMillisecond).ToString();
            psr.Reset(); subsh.exitq=false;
            int ret=subsh.InterpretParser(psr);
            if(m.body0.m_Bones==null || ret!=0) m.body0.OnLateUpdate-=act;
        };
        m.body0.OnLateUpdate+=act;
        lateDic[iid]=act;
        return 1;
    }
    private static Dictionary<Maid,Action> lateupdateenddic=new Dictionary<Maid,Action>();
    private static int MaidParamEvenLater(ComShInterpreter sh,Maid m,string val){
        if(val==null) return 0;

        var sbo=new ComShInterpreter.SubShOutput();
        var subsh=new ComShInterpreter(new ComShInterpreter.Output(sbo.Output),sh.env,sh.func,sh.ns);
        subsh.env[ComShInterpreter.SCRIPT_ERR_ON]="1";

        var psr=Command.EvalParser(sh,Command.currentArgNo+1,false,sh.currentParser.lineno);
        if(psr==null) return -1;

        int iid=m.GetInstanceID();

        Action act=null;
        if(lateupdateenddic.TryGetValue(m,out act)){
            m.body0.onLateUpdateEnd.Remove(act);
            lateupdateenddic.Remove(m);
        }
        if(psr.sta.Count==0) return 1; // 空→登録削除のみ

        int ret=0;
        long stime=DateTime.UtcNow.Ticks;
        subsh.env.args.Clear();
        subsh.env.args.Add("");
        act=()=>{
            if(m==null||m.body0==null) {  m.body0.onLateUpdateEnd.Remove(act); lateupdateenddic.Remove(m); return;}
            subsh.env.args[0]=((DateTime.UtcNow.Ticks-stime)/TimeSpan.TicksPerMillisecond).ToString();
            psr.Reset(); subsh.exitq=false;
            ret=subsh.InterpretParser(psr);
            if(ret<0){ m.body0.onLateUpdateEnd.Remove(act); lateupdateenddic.Remove(m); }
        };
        m.body0.onLateUpdateEnd.Add(act);
        lateupdateenddic[m]=act;
        return 1;
    }
    private static int MaidParamMuneParam(ComShInterpreter sh,Maid m,string val){
        if(m.IsCrcBody) return MaidParamMuneParam_dmb(sh,m,val);
        else return MaidParamMuneParam_jb(sh,m,val);
    }
    private static T GetDblValue<T>(DynamicMuneBone dbl,string field){
        FieldInfo fi=typeof(DynamicMuneBone).GetField(field,BindingFlags.Instance | BindingFlags.NonPublic);
        if(fi==null) throw new Exception();
        return (T)fi.GetValue(dbl);
    }
    private static void SetDblValue<T>(DynamicMuneBone dbl,DynamicMuneBone dbr,string field,T val){
        FieldInfo fi=typeof(DynamicMuneBone).GetField(field,BindingFlags.Instance | BindingFlags.NonPublic);
        if(fi==null) throw new Exception();
        fi.SetValue(dbl,val);
        fi.SetValue(dbr,val);
    }
    private static (float,float) GetDblFloatPair(DynamicMuneBone dbl,string field){
        Type pairtype=typeof(DynamicMuneBone).GetNestedType("MathLerpPair",BindingFlags.NonPublic);
        if(pairtype==null) throw new Exception();
        FieldInfo p=typeof(DynamicMuneBone).GetField(field,BindingFlags.Instance|BindingFlags.NonPublic);
        if(p==null) throw new Exception();
        object o=p.GetValue(dbl);
        if(o==null) throw new Exception();
        float t0,t1;
        p=pairtype.GetField("t0Value",BindingFlags.Instance|BindingFlags.Public);
        if(p==null) throw new Exception();
        t0=(float)p.GetValue(o);
        p=pairtype.GetField("t1Value",BindingFlags.Instance|BindingFlags.Public);
        if(p==null) throw new Exception();
        t1=(float)p.GetValue(o);
        return (t0,t1);
    }
    private static void SetDblFloatPair(DynamicMuneBone dbl,DynamicMuneBone dbr,string field,float t0,float t1){
        Type pairtype=typeof(DynamicMuneBone).GetNestedType("MathLerpPair",BindingFlags.NonPublic);
        if(pairtype==null) throw new Exception();
        FieldInfo p=typeof(DynamicMuneBone).GetField(field,BindingFlags.Instance|BindingFlags.NonPublic);
        if(p==null) throw new Exception();
        object o=p.GetValue(dbl);
        if(o==null) throw new Exception();
        FieldInfo p2=pairtype.GetField("t0Value",BindingFlags.Instance|BindingFlags.Public);
        if(p2==null) throw new Exception();
        p2.SetValue(o,t0);
        p2=pairtype.GetField("t1Value",BindingFlags.Instance|BindingFlags.Public);
        if(p==null) throw new Exception();
        p2.SetValue(o,t1);
        p.SetValue(dbl,o);
        p.SetValue(dbr,o);
    }
    private static int MaidParamMuneParam_dmb(ComShInterpreter sh,Maid m,string val){
        var dbl=m.body0.dbMuneL;
        if(val==null){
            try{
                float gravityy0=GetDblValue<float>(dbl,"GravityYMin");
                float gravityyl=GetDblValue<float>(dbl,"GravityYLandScape");
                float gravityyf=GetDblValue<float>(dbl,"GravityYFaceUpDown");
                float distance=GetDblValue<float>(dbl,"TargetDistance");
                float maxangle=GetDblValue<float>(dbl,"RimmitAngle");
                float supinex=GetDblValue<float>(dbl,"GravityXSupine");
                float supinexscl=GetDblValue<float>(dbl,"GravityXScalingSupine");
                float prone=GetDblValue<float>(dbl,"GravityXScalingProne");
                (float stiff0,float stiff1)=GetDblFloatPair(dbl,m.body0.boVisible_NIP?"StiffnessLerp":"StiffnessBraLerp");
                (float mass0,float mass1)=GetDblFloatPair(dbl,"MassLerp");
                (float damp0,float damp1)=GetDblFloatPair(dbl,"DampingLerp");
                sh.io.PrintLn2("gravity:",sh.fmt.FPos(gravityy0,gravityyl,gravityyf));
                sh.io.PrintLn2("distance:",sh.fmt.FVal(distance));
                sh.io.PrintLn2("damping:",sh.fmt.FXY(damp0,damp1));
                sh.io.PrintLn2("stiffness:",sh.fmt.FXY(stiff0,stiff1));
                sh.io.PrintLn2("mass:",sh.fmt.FXY(mass0,mass1));
                sh.io.PrintLn2("supine:",sh.fmt.FXY(supinex,supinexscl));
                sh.io.PrintLn2("prone:",sh.fmt.FVal(prone));
                sh.io.PrintLn2("maxangle:",sh.fmt.FVal(maxangle));
            }catch{ return sh.io.Error("失敗しました"); }
            return 0;
        }
        var dbr=m.body0.dbMuneR;
        var sa=val.Split(ParseUtil.lf);
        float f;
        float[] fa;
        try{ for(int i=0; i<sa.Length; i++){
            string[] lr=ParseUtil.LeftAndRight(sa[i],'=');
            if(lr[0]==""||lr[1]=="") return sh.io.Error("書式が不正です");
            switch(lr[0]){
            case "gravity":
                fa=ParseUtil.Xyz(lr[1]);
                if(fa==null||fa[0]<0||fa[1]<0||fa[2]<0) sh.io.Error("数値が不正です");
                SetDblValue(dbl,dbr,"GravityYMin",fa[0]);
                SetDblValue(dbl,dbr,"GravityYLandScape",fa[1]);
                SetDblValue(dbl,dbr,"GravityYFaceUpDown",fa[2]);
                break;
            case "distance":
                if(!float.TryParse(lr[1],out f)) return sh.io.Error("数値が不正です");
                SetDblValue(dbl,dbr,"TargetDistance",f);
                break;
            case "damping":
                fa=ParseUtil.Xy(lr[1]);
                if(fa==null||fa[0]<0||fa[1]<0) sh.io.Error("数値が不正です");
                SetDblFloatPair(dbl,dbr,"DampingLerp",fa[0],fa[1]);
                break;
            case "stiffness":
                fa=ParseUtil.Xy(lr[1]);
                if(fa==null||fa[0]<0||fa[1]<0) sh.io.Error("数値が不正です");
                SetDblFloatPair(dbl,dbr,m.body0.boVisible_NIP?"StiffnessLerp":"StiffnessBraLerp",fa[0],fa[1]);
                break;
            case "mass":
                fa=ParseUtil.Xy(lr[1]);
                if(fa==null||fa[0]<0||fa[1]<0) sh.io.Error("数値が不正です");
                SetDblFloatPair(dbl,dbr,"MassLerp",fa[0],fa[1]);
                break;
            case "supine":
                fa=ParseUtil.Xy(lr[1]);
                if(fa==null||fa[0]<0||fa[1]<0) sh.io.Error("数値が不正です");
                SetDblValue(dbl,dbr,"GravityXSupine",fa[0]);
                SetDblValue(dbl,dbr,"GravityXScalingSupine",fa[1]);
                break;
            case "prone":
                if(!float.TryParse(lr[1],out f)) return sh.io.Error("数値が不正です");
                SetDblValue(dbl,dbr,"GravityXScalingProne",f);
                break;
            case "maxangle":
                if(!float.TryParse(lr[1],out f)) return sh.io.Error("数値が不正です");
                SetDblValue(dbl,dbr,"RimmitAngle",f);
                break;
            }
        } }catch{return sh.io.Error("失敗しました");}
        return 1;
    }
    private static int MaidParamMuneParam_jb(ComShInterpreter sh,Maid m,string val){
        var jbl=m.body0.jbMuneL;
        if(val==null){
            sh.io.PrintJoin(",",
                sh.fmt.FInt(jbl.bGravity),
                sh.fmt.FInt(jbl.targetDistance),
                sh.fmt.FInt(jbl.boBRA?jbl.bStiffnessBRA[0]:jbl.bStiffness[0]),
                sh.fmt.FInt(jbl.boBRA?jbl.bStiffnessBRA[1]:jbl.bStiffness[1]),
                sh.fmt.FInt(jbl.frontStretch),
                sh.fmt.FInt(jbl.sideStretch),
                sh.fmt.FInt(jbl.m_fLimitRot)
            );
            return 0;
        }
        var jbr=m.body0.jbMuneR;
        var sa=val.Split(',');
        float f;
        if(sa.Length>7) return sh.io.Error("数値が不正です");
        if(sa.Length>=1 && float.TryParse(sa[0],out f)) jbl.bGravity=jbr.bGravity=f;
        if(sa.Length>=2 && float.TryParse(sa[1],out f)) jbl.targetDistance=jbr.targetDistance=f;
        if(sa.Length>=3 && float.TryParse(sa[2],out f)){
            if(jbl.boBRA) jbl.bStiffnessBRA[0]=f; else jbl.bStiffness[0]=f;
            if(jbr.boBRA) jbr.bStiffnessBRA[0]=f; else jbr.bStiffness[0]=f;
        }
        if(sa.Length>=4 && float.TryParse(sa[3],out f)){
            if(jbl.boBRA) jbl.bStiffnessBRA[1]=f; else jbl.bStiffness[1]=f;
            if(jbr.boBRA) jbr.bStiffnessBRA[1]=f; else jbr.bStiffness[1]=f;
        }
        if(sa.Length>=5 && float.TryParse(sa[4],out f)) jbl.frontStretch=jbr.frontStretch=f;
        if(sa.Length>=6 && float.TryParse(sa[5],out f)) jbl.sideStretch=jbr.sideStretch=f;
        if(sa.Length==7 && float.TryParse(sa[6],out f)) jbl.m_fLimitRot=jbr.m_fLimitRot=f;
        return 1;
    }
    private static int MaidParamSkirtParamSave(ComShInterpreter sh,Maid m,string val){
        if(val==null) return 0;
        var lr=ParseUtil.LeftAndRight2(val,':');
        string file=UTIL.GetFullPath(UTIL.Suffix(lr[1],".psk"),ComShInterpreter.scriptFolder+@"export\");
        if(file=="") return sh.io.Error("ファイル名が不正です");
        try{
            using (var bw=new BinaryWriter(File.OpenWrite(file))){
                foreach(var skin in m.body0.goSlot.GetListParents()) if(m.body0.GetSlotVisible(skin.SlotId)){
                    if(lr[0]!="" && lr[0]!=skin.SlotId.ToString()) continue;
                    var sb=skin.obj.GetComponent<DynamicSkirtBone>();
                    if(sb!=null){ sb.SerializeWrite(bw); break; }
                }
            }
        }catch(Exception e){ Debug.Log(e.ToString()); return sh.io.Error("書き込みに失敗しました"); }
        return 1;
    }
    private static int MaidParamSkirtParamLoad(ComShInterpreter sh,Maid m,string val){
        if(val==null) return 0;
        var lr=ParseUtil.LeftAndRight2(val,':');
        var dat=UTIL.AReadAll(UTIL.Suffix(lr[1],".psk"));
        if(dat==null) return sh.io.Error("読み込みに失敗しました");
        try{
            using (var bw=new BinaryReader(new MemoryStream(dat))){
                foreach(var skin in m.body0.goSlot.GetListParents()) if(m.body0.GetSlotVisible(skin.SlotId)){
                    if(lr[0]!="" && lr[0]!=skin.SlotId.ToString()) continue;
                    var sb=skin.obj.GetComponent<DynamicSkirtBone>();
                    if(sb!=null) sb.SerializeRead(bw);
                }
            }
        }catch(Exception e){ Debug.Log(e.ToString()); return sh.io.Error("読み込みに失敗しました"); }
        return 1;
    }
    private static int MaidParamSkirtParam(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            foreach(var skin in m.body0.goSlot.GetListParents()) if(m.body0.GetSlotVisible(skin.SlotId)){
                var sb=skin.obj.GetComponent<DynamicSkirtBone>();
                if(sb!=null) SkirtParamPrint(sh,sb,skin.SlotId);
            }
            return 0;
        }
        var lines=val.Split(ParseUtil.lf);
        for(int i=0; i<lines.Length; i++){
            var lr=ParseUtil.LeftAndRight(lines[i],'=');
            if(lr[0]!="" && lr[1]!=""){
                var sk=ParseUtil.LeftAndRight2(lr[0],'.');
                foreach(var skin in m.body0.goSlot.GetListParents()) if(m.body0.GetSlotVisible(skin.SlotId)){
                    if(sk[0]!="" && sk[0]!=skin.SlotId.ToString()) continue;
                    var sb=skin.obj.GetComponent<DynamicSkirtBone>();
                    if(sb!=null){
                        int ret=SkirtParamSet(sh,sb,sk[1],lr[1]);
                        if(ret<0) return ret;
                        sb.UpdateParameters();
                    }
                }
            }
        }
        return 1;
    }
    private static void SkirtParamPrint(ComShInterpreter sh,DynamicSkirtBone dsb,TBody.SlotID sid){
        string slot=sid.ToString();
        sh.io.Print(slot).Print(".radius:").PrintLn(sh.fmt.FVal(dsb.m_fPanierRadius));

        if(dsb.m_PanierRadiusDistribGroup!=null) for(int i=0; i<dsb.m_PanierRadiusDistribGroup.Length; i++){
            var grp=dsb.m_PanierRadiusDistribGroup[i];
            sh.io.Print(slot).Print('.').Print(grp.strBoneName).Print(':').PrintLn(sh.fmt.FVal(grp.fRadius));
        }
        sh.io.Print(slot).Print(".force:").PrintLn(sh.fmt.FVal(dsb.m_fPanierForce));
        sh.io.Print(slot).Print(".threshold:").PrintLn(sh.fmt.FVal(dsb.m_fPanierForceDistanceThreshold));
        sh.io.Print(slot).Print(".stressforce:").PrintLn(sh.fmt.FVal(dsb.m_fPanierStressForce));
        sh.io.Print(slot).Print(".stressdegree:").Print(sh.fmt.FVal(dsb.m_fStressDgreeMin)).Print('-').PrintLn(sh.fmt.FVal(dsb.m_fStressDgreeMax));;
        sh.io.Print(slot).Print(".stressscale:").PrintLn(sh.fmt.FVal(dsb.m_fStressMinScale));
        sh.io.Print(slot).Print(".scaleease:").PrintLn(sh.fmt.FVal(dsb.m_fScaleEaseSpeed));
        sh.io.Print(slot).Print(".count:").PrintLn(sh.fmt.FVal(dsb.m_nCalcTime));
        sh.io.Print(slot).Print(".velocity:").PrintLn(sh.fmt.FVal(dsb.m_fVelocityForceRate));
        sh.io.Print(slot).Print(".gravity:").PrintLn(sh.fmt.FPos(dsb.m_vGravity));
        sh.io.Print(slot).Print(".spring:")
            .Print(sh.fmt.FVal(dsb.m_aryHard[0])).Print(',')
            .Print(sh.fmt.FVal(dsb.m_aryHard[1])).Print(',')
            .Print(sh.fmt.FVal(dsb.m_aryHard[2])).Print(',')
            .PrintLn(sh.fmt.FVal(dsb.m_aryHard[3]));
    }
    private static int SkirtParamSet(ComShInterpreter sh,DynamicSkirtBone dsb,string key,string value){
        float f; float[] fa;
        switch(key){
        case "radius":
            if(!float.TryParse(value,out f)||f<=0) return sh.io.Error("数値が不正です");
            dsb.m_fPanierRadius=f;
            return 1;
        case "force":
            if(!float.TryParse(value,out f)) return sh.io.Error("数値が不正です");
            dsb.m_fPanierForce=f;
            return 1;
        case "threshold":
            if(!float.TryParse(value,out f)) return sh.io.Error("数値が不正です");
            dsb.m_fPanierForceDistanceThreshold=f;
            return 1;
        case "stressforce":
            if(!float.TryParse(value,out f)) return sh.io.Error("数値が不正です");
            dsb.m_fPanierStressForce=f;
            return 1;
        case "stressdegree":
            fa=ParseUtil.FloatArr(value);
            if(fa==null||fa.Length!=2||fa[0]<0||fa[1]<0) return sh.io.Error("数値が不正です");
            dsb.m_fStressDgreeMin=fa[0]; dsb.m_fStressDgreeMax=fa[1];
            return 1;
        case "stressscale":
            if(!float.TryParse(value,out f)) return sh.io.Error("数値が不正です");
            dsb.m_fStressMinScale=f;
            return 1;
        case "scaleease":
            if(!float.TryParse(value,out f)) return sh.io.Error("数値が不正です");
            dsb.m_fScaleEaseSpeed=f;
            return 1;
        case "count":
            if(!float.TryParse(value,out f)||f<0) return sh.io.Error("数値が不正です");
            dsb.m_nCalcTime=(int)f;
            return 1;
        case "velocity":
            if(!float.TryParse(value,out f)) return sh.io.Error("数値が不正です");
            dsb.m_fVelocityForceRate=f;
            return 1;
        case "gravity":
            fa=ParseUtil.Xyz(value);
            if(fa==null) return sh.io.Error("座標が不正です");
            dsb.m_vGravity=new Vector3(fa[0],fa[1],fa[2]);
            return 1;
        case "spring":
            fa=ParseUtil.FloatArr(value);
            if(fa==null||fa.Length!=4) return sh.io.Error("数値が不正です");
            for(int i=0; i<4; i++) dsb.m_aryHard[i]=fa[i];
            return 1;
        default:
            if(key.Contains("_A_yure_skirt_") && CMT.SearchObjName(dsb.transform,key,false)!=null){
                DynamicSkirtBone.PanierRadiusGroup grp;
                if(dsb.m_PanierRadiusDistribGroup==null){
                    grp=new DynamicSkirtBone.PanierRadiusGroup();
                    grp.strBoneName=key;
                    if(!float.TryParse(value,out f)||f<0) return sh.io.Error("数値が不正です");
                    grp.fRadius=f;
                    dsb.m_PanierRadiusDistribGroup=new DynamicSkirtBone.PanierRadiusGroup[]{grp};
                    return 1;
                }else{
                    for(int i=0; i<dsb.m_PanierRadiusDistribGroup.Length; i++){
                        grp=dsb.m_PanierRadiusDistribGroup[i];
                        if(grp==null) continue;
                        if(key==grp.strBoneName){
                            if(!float.TryParse(value,out f)||f<0) return sh.io.Error("数値が不正です");
                            grp.fRadius=f;
                            return 1;
                        }
                    }
                }
                var ga=dsb.m_PanierRadiusDistribGroup;
                var ga2=new DynamicSkirtBone.PanierRadiusGroup[ga.Length+1]; 
                Array.Copy(ga,ga2,ga.Length);
                grp=new DynamicSkirtBone.PanierRadiusGroup();
                grp.strBoneName=key;
                if(!float.TryParse(value,out f)||f<0) return sh.io.Error("数値が不正です");
                grp.fRadius=f;
                ga2[ga2.Length-1]=grp;
                dsb.m_PanierRadiusDistribGroup=ga2;
                return 1;
            }
            return sh.io.Error("パラメータが不正です");
        }
    }
    private static int MaidParamSkirtParamCurve(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            foreach(var skin in m.body0.goSlot.GetListParents()) if(m.body0.GetSlotVisible(skin.SlotId)){
                var sb=skin.obj.GetComponent<DynamicSkirtBone>();
                if(sb!=null) SkirtParamCurvePrint(sh,sb,skin.SlotId);
            }
            return 0;
        }
        var lines=val.Split(ParseUtil.lf);
        for(int i=0; i<lines.Length; i++){
            var lr=ParseUtil.LeftAndRight(lines[i],'=');
            if(lr[0]!="" && lr[1]!=""){
                foreach(var skin in m.body0.goSlot.GetListParents()) if(m.body0.GetSlotVisible(skin.SlotId)){
                    var sk=ParseUtil.LeftAndRight2(lr[0],'.');
                    if(sk[0]!="" && sk[0]!=skin.SlotId.ToString()) continue;
                    var sb=skin.obj.GetComponent<DynamicSkirtBone>();
                    if(sb!=null){
                        int ret=SkirtParamCurveSet(sh,sb,sk[1],lr[1]);
                        if(ret<=0) return ret;
                        sb.UpdateParameters();
                    }
                }
            }
        }
        return 1;
    }
    private static AnimationCurve SkirtParamCurve(ComShInterpreter sh,string value){
        var ret=new AnimationCurve();
        if(CmdMisc.curveDic.ContainsKey(value)) return CmdMisc.curveDic[value];
        var fa=ParseUtil.FloatArr2(value,ParseUtil.commaslash);
        if(fa.Length%4>0||fa.Length<8) return null;
        for(int i=0; i<fa.Length; i+=4) ret.AddKey(new Keyframe(fa[i],fa[i+1],fa[i+2],fa[i+3]));
        return ret;
    }
    private static void PrintSkirtCurve(string key,ComShInterpreter sh,AnimationCurve curve,string slot){
        if(curve==null) return;
        int tail=curve.length-1;
        float t0=curve[0].time,t1=curve[tail].time,v0=curve[0].value,v1=curve[tail].value;
        sh.io.Print(slot).Print('.').Print(key).Print(':').Print(sh.fmt.FVal(t0)).Print(',').Print(sh.fmt.FVal(v0))
            .Print('-').Print(sh.fmt.FVal(t1)).Print(',').PrintLn(sh.fmt.FVal(v1));
    }
    private static void PrintSkirtCurveDetail(ComShInterpreter sh,AnimationCurve curve){
        if(curve==null) return;
        foreach(var kf in curve.keys){
            sh.io.Print(sh.fmt.FVal(kf.time)).Print(',').Print(sh.fmt.FVal(kf.value)).Print(',')
                .Print(sh.fmt.FVal(kf.inTangent)).Print(',').PrintLn(sh.fmt.FVal(kf.outTangent));
        }
    }
    private static void SkirtParamCurvePrint(ComShInterpreter sh,DynamicSkirtBone dsb,TBody.SlotID sid){
        string slot=sid.ToString();
        PrintSkirtCurve("radius",sh,dsb.m_PanierRadiusDistrib,slot);
        if(dsb.m_PanierRadiusDistribGroup!=null) for(int i=0; i<dsb.m_PanierRadiusDistribGroup.Length; i++){
            var grp=dsb.m_PanierRadiusDistribGroup[i];
            PrintSkirtCurve(grp.strBoneName,sh,grp.Curve,slot);
        }
        PrintSkirtCurve("force",sh,dsb.m_PanierForceDistrib,slot);
        PrintSkirtCurve("velocity",sh,dsb.m_VelocityForceRateDistrib,slot);
        PrintSkirtCurve("gravity",sh,dsb.m_GravityDistrib,slot);
    }
    private static int SkirtParamCurveSet(ComShInterpreter sh,DynamicSkirtBone dsb,string key,string value){
        AnimationCurve cv;
        switch(key){
        case "radius":
            if(value==""){ PrintSkirtCurveDetail(sh,dsb.m_PanierRadiusDistrib); return 0;}
             cv=SkirtParamCurve(sh,value);
             if(cv==null) return sh.io.Error("数値が不正です");
             dsb.m_PanierRadiusDistrib=cv;
            return 1;
        case "force":
            if(value==""){ PrintSkirtCurveDetail(sh,dsb.m_PanierForceDistrib); return 0;}
            cv=SkirtParamCurve(sh,value);
            if(cv==null) return sh.io.Error("数値が不正です");
            dsb.m_PanierForceDistrib=cv;
            return 1;
        case "velocity":
            if(value==""){ PrintSkirtCurveDetail(sh,dsb.m_VelocityForceRateDistrib); return 0;}
            cv=SkirtParamCurve(sh,value);
            if(cv==null) return sh.io.Error("数値が不正です");
            dsb.m_VelocityForceRateDistrib=cv;
            return 1;
        case "gravity":
            if(value==""){ PrintSkirtCurveDetail(sh,dsb.m_GravityDistrib); return 0;}
            cv=SkirtParamCurve(sh,value);
            if(cv==null) return sh.io.Error("数値が不正です");
            dsb.m_GravityDistrib=cv;
            return 1;
        default:
            if(key.Contains("_A_yure_skirt_") && CMT.SearchObjName(dsb.transform,key,false)!=null){
                if(dsb.m_PanierRadiusDistribGroup==null){
                    var grp=new DynamicSkirtBone.PanierRadiusGroup();
                    grp.strBoneName=key;
                    cv=SkirtParamCurve(sh,value);
                    if(cv==null) return sh.io.Error("数値が不正です");
                    grp.Curve=cv;
                    dsb.m_PanierRadiusDistribGroup=new DynamicSkirtBone.PanierRadiusGroup[]{grp};
                    return 1;
                }else{
                    DynamicSkirtBone.PanierRadiusGroup grp;
                    for(int i=0; i<dsb.m_PanierRadiusDistribGroup.Length; i++){
                        grp=dsb.m_PanierRadiusDistribGroup[i];
                        if(grp==null) continue;
                        if(key==grp.strBoneName){
                            if(value==""){ PrintSkirtCurveDetail(sh,grp.Curve); return 0;}
                            cv=SkirtParamCurve(sh,value);
                            if(cv==null) return sh.io.Error("数値が不正です");
                            grp.Curve=cv;
                            return 1;
                        }
                    }
                    var ga=dsb.m_PanierRadiusDistribGroup;
                    var ga2=new DynamicSkirtBone.PanierRadiusGroup[ga.Length+1]; 
                    Array.Copy(ga,ga2,ga.Length);
                    grp=new DynamicSkirtBone.PanierRadiusGroup();
                    grp.strBoneName=key;
                    cv=SkirtParamCurve(sh,value);
                    if(cv==null) return sh.io.Error("数値が不正です");
                    grp.Curve=cv;
                    ga2[ga2.Length-1]=grp;
                    dsb.m_PanierRadiusDistribGroup=ga2;
                    return 1;
                }
            }
            return sh.io.Error("パラメータが不正です");
        }
    }
    private static int MaidParamBBox(ComShInterpreter sh,Maid m,string val){
        Vector3 min=new Vector3(float.MaxValue,float.MaxValue,float.MaxValue);
        Vector3 max=new Vector3(float.MinValue,float.MinValue,float.MinValue);
        var ls=new List<string>(64);
        CharacterMgr cm=GameMain.Instance.CharacterMgr;
        if(cm.TryGetCacheObject(m.body0.goSlot[0].m_strModelFileName,out GameObject go)){
            string bname=m.body0.trBip.name;
            Transform bip=go.transform.Find(bname);
            if(bip==null) return 0;
            UTIL.TraverseTr(bip,(Transform tr,int d)=>{
                if(!tr.name.StartsWith(bname,Ordinal)) return 1;
                if(tr.name.EndsWith("_SCL_",Ordinal)) return 1;
                if(tr.name.EndsWith("Footsteps",Ordinal)) return 1;
                ls.Add(tr.name);
                return 0;
            });
        }
        foreach(var k in ls){
            if(!m.body0.m_dicTrans.TryGetValue(k,out Transform t)) continue;
            Vector3 pos=t.position;
            if(max.x<pos.x) max.x=pos.x;
            if(max.y<pos.y) max.y=pos.y;
            if(max.z<pos.z) max.z=pos.z;
            if(min.x>pos.x) min.x=pos.x;
            if(min.y>pos.y) min.y=pos.y;
            if(min.z>pos.z) min.z=pos.z;
        }
        sh.io.PrintJoin(sh.ofs,sh.fmt.FPos(min),sh.fmt.FPos(max));
        return 0;
    }
    private static int MaidParamFloor(ComShInterpreter sh,Maid m,string val){
        if(m.body0==null) return 1;
        if(val==null){
            sh.io.Print(sh.fmt.FVal(m.body0.m_trFloorPlane.position.y));
            return 0;
        }
        if(!float.TryParse(val,out float f)) return sh.io.Error("数値の指定が不正です");
        var p=m.body0.m_trFloorPlane.position;
        p.y=f;
        m.body0.m_trFloorPlane.position=p;
        return 1;
    }
    private static FieldInfo yureSwField;
    private static int MaidParamClothYure(ComShInterpreter sh,Maid m,string val){
        if(m.body0==null) return 1;
        if(yureSwField==null) try{
            yureSwField=typeof(TBoneHair_).GetField("m_bEnable",BindingFlags.Instance | BindingFlags.NonPublic); 
        }catch{return sh.io.Error("失敗しました");};
        if(val==null||val=="") return sh.io.Error("スロット名を指定してください");
        char ch=val[val.Length-1];
        string slot=val;
        if(ch=='+'||ch=='-') slot=val.Substring(0,val.Length-1);
        TBodySkin skin=null;
        if(m.body0.IsSlotNo(slot)) skin=m.body0.GetSlot(slot);
        if(skin==null) return sh.io.Error("スロット名が不正です");
        bool onoff;
        if(ch=='+') onoff=true;
        else if(ch=='-') onoff=false;
        else onoff=!(bool)yureSwField.GetValue(skin.bonehair);
        yureSwField.SetValue(skin.bonehair,onoff);
        return 1;
    }
    private static int MaidParamSkirtYure(ComShInterpreter sh,Maid m,string val){
        if(m.body0==null) return 1;
        if(val==null){
            foreach(var skin in m.body0.goSlot.GetListParents()) if(m.body0.GetSlotVisible(skin.SlotId)){
                var sb=skin.obj.GetComponent<DynamicSkirtBone>();
                if(sb!=null) SkirtYureSub(sh,m,skin.SlotId.ToString(),-1);
            }
            return 0;
        }

        var lr=ParseUtil.LeftAndRight2(val,':');
        int sw=ParseUtil.OnOff(lr[1]);
        if(sw<0) return sh.io.Error("onまたはoffを指定してください");

        int ret=0;
        foreach(var skin in m.body0.goSlot.GetListParents()) if(m.body0.GetSlotVisible(skin.SlotId)){
            string slot=skin.SlotId.ToString();
            if(lr[0]!="" && lr[0]!=slot) continue;
            var sb=skin.obj.GetComponent<DynamicSkirtBone>();
            if(sb!=null){
                ret=SkirtYureSub(sh,m,slot,sw);
                if(ret<0) return ret;
            }
        }
        return ret;
    }
    private static int SkirtYureSub(ComShInterpreter sh,Maid m,string slotname,int sw){
        TBodySkin skin=m.body0.GetSlot(slotname);
        if(skin==null) return 0;
        if(sw<0){
            sh.io.PrintLn(skin.bonehair.boSkirt?"on":"off");
            return 0;
        }
        if(sw==1){
            skin.bonehair.Init();
            skin.bonehair3.Uninit();
            skin.bonehair.SearchGameObj(skin.obj,
                skin.bonehair3.InitGameObject(skin.obj,skin.m_ParentMPN));
        }else{
            skin.bonehair.Init();
            skin.bonehair3.Uninit();
        }
        return 1;
    }
    private static int MaidParamAttachPoint(ComShInterpreter sh,Maid m,string val){
        var slot=m.body0.goSlot;
        if(val==null){
            for(int i=0; i<slot.Count; i++) {
                if(slot[i].morph==null) continue;
                foreach(var ap in slot[i].morph.dicAttachPoint){
                    sh.io.PrintLn($"{ap.Key}");
                }
            }
            return 0;
        }
        for(int i=0; i<slot.Count; i++){
            if(slot[i].morph==null) continue;
            foreach(var ap in slot[i].morph.dicAttachPoint) if(ap.Key==val){
                Vector3 v,s; Quaternion q;
                slot[i].morph.GetAttachPoint(val,out v,out q,out s);
                sh.io.PrintLn2("position:",sh.fmt.FPos(v));
                sh.io.PrintLn2("rotation:",sh.fmt.FEuler(q.eulerAngles));
                break;
            }
        }
        return 1;
    }
    private static int MaidParamAttachPointWpos(ComShInterpreter sh,Maid m,string val){
        var slot=m.body0.goSlot;
        if(val==null){
            for(int i=0; i<slot.Count; i++){
                if(slot[i].morph==null) continue;
                foreach(var ap in slot[i].morph.dicAttachPoint){
                    Vector3 v,s; Quaternion q;
                    slot[i].morph.GetAttachPoint(ap.Key,out v,out q,out s);
                    sh.io.PrintLn($"{ap.Key}{sh.ofs}{sh.fmt.FPos(v)}");
                    break;
                }
            }
            return 0;
        }
        for(int i=0; i<slot.Count; i++){
            if(slot[i].morph==null) continue;
            foreach(var ap in slot[i].morph.dicAttachPoint) if(ap.Key==val){
                Vector3 v,s; Quaternion q;
                slot[i].morph.GetAttachPoint(val,out v,out q,out s);
                sh.io.Print(sh.fmt.FPos(v));
                break;
            }
        }
        return 0;
    }
    private static int MaidParamHandle(ComShInterpreter sh,Maid m,string val){
        if(val==null)
            return sh.io.Error("off|lpos|wpos||rot|wrot|scale|lposrot|wposrotのいずれかを指定してください");

        var hdl=ComShHandle.GetHandle(m.transform);
        var lr=ParseUtil.LeftAndRight(val,',');
        var sw=lr[0].ToLower();
        if(sw=="off"){
            if(hdl!=null) ComShHandle.DelHandle(hdl);
            return 1;
        }
        float scale=0.75f;
        if(lr[1]!="" && (!float.TryParse(lr[1],out scale)||scale<0.1)) return sh.io.Error("値が不正です");
        if(hdl==null){ hdl=ComShHandle.AddHandle(m); hdl.Visible=true;}
        if(ComShHandle.SetHandleType(hdl,sw)<0)
            return sh.io.Error("off|lpos|wpos||rot|wrot|scale|lposrot|wposrotのいずれかを指定してください");
        hdl.offsetScale=scale;
        return 1;
    }
    private static int MaidParamDesc(ComShInterpreter sh,Maid m,string val){
        sh.io.PrintJoin(" ", // コマンドラインの体裁だからofsではない
            "wpos", sh.fmt.FPos(m.transform.position),
            "wrot", sh.fmt.FEuler(m.transform.rotation.eulerAngles),
            "scale", sh.fmt.FMul(m.transform.localScale)
        );
        var mo= MaidUtil.GetCurrentMotion(m);
        if(mo!="") sh.io.Print(" motion "+mo);
        return 0;
    }

    private static string[] ikNames={"左手","右手","左足","右足"};
    public static string SetIK(ComShInterpreter sh,Maid m,string[] te){
        int iid=m.GetInstanceID();
        if(MaidUtil.ikObj.ContainsKey(iid)){
            string[] objnames=MaidUtil.ikObj[iid];
            for(int i=0; i<4; i++) if(!string.IsNullOrEmpty(objnames[i])) return "既にik有効です";
        }
        MaidUtil.ikObj[iid]=te;
        for(int i=0; i<4; i++){
            if(te[i].Length==0) continue;
            bool handq=(i<2);
            bool leftq=(i%2==0);
            string lr=ikNames[i];
            if(!UTIL.ValidName(te[i])) return "その名前は使用できません";
            if(ObjUtil.FindObj(sh,te[i])!=null||LightUtil.FindLight(sh,te[i])!=null) return "その名前は既に使われています";
            Transform tr=CreateGrabNode(sh,m,te[i],leftq,handq);
            ObjUtil.objDic[tr.name]=tr;
            MaidUtil.GrabCron(m,tr,lr);
            if(handq){
                if(leftq){ m.SetAutoTwist(Maid.AutoTwist.ShoulderL,true); m.SetAutoTwist(Maid.AutoTwist.WristL,true); }
                else{m.SetAutoTwist(Maid.AutoTwist.ShoulderR,true); m.SetAutoTwist(Maid.AutoTwist.WristR,true); }
            }else{
                if(leftq) m.SetAutoTwist(Maid.AutoTwist.ThighL,true);
                else m.SetAutoTwist(Maid.AutoTwist.ThighR,true);
            }
        }
        return "";
    }

    public static int ClearIK(ComShInterpreter sh,Maid m){
        int iid=m.GetInstanceID();
        if(!MaidUtil.ikObj.ContainsKey(iid)) return sh.io.Error("既にik無効です");
        MaidUtil.ClrGrabCron(m,"左手");
        MaidUtil.ClrGrabCron(m,"右手");
        MaidUtil.ClrGrabCron(m,"左足");
        MaidUtil.ClrGrabCron(m,"右足");
        m.fullBodyIK.AllIKDetach(0f);
        return 1;
    }
    private static Transform CreateGrabNode(ComShInterpreter sh,Maid m,string name,bool leftq,bool handq){
        Transform pftr=ObjUtil.GetPhotoPrefabTr(sh);
        if(pftr==null) return null;
        Transform ptr=null;
        string bip=m.body0.trBip.name;
        if(handq){
            if(leftq) ptr=CMT.SearchObjName(m.body0.UpperArmL,bip+" L Hand");
            else ptr=CMT.SearchObjName(m.body0.UpperArmR,bip+" R Hand");
        }else{
            if(leftq) ptr=CMT.SearchObjName(m.body0.Calf_L,bip+" L Foot");
            else ptr=CMT.SearchObjName(m.body0.Calf_R,bip+" R Foot");
        }
        if(ptr==null) return null;
        GameObject go=new GameObject(name);
        go.transform.SetParent(pftr);
        go.transform.position=ptr.position;
        go.transform.rotation=ptr.rotation;
        return go.transform;
    }
    private static string GravityCommon(Maid m,string cate,string val){
        if(val=="off"){
            GravityTransformControl gtc=MaidUtil.GetGravity(m,cate);
            if(gtc==null) return "変更できません";
            gtc.isEnabled=false;
        }else{
            float force=1f;
            string[] sa=val.Split(ParseUtil.comma);
            if(sa.Length<3) return "座標の指定が不正です";
            if(sa.Length==4&&(!float.TryParse(sa[3],out force)||force<0))
                return "強さの指定が不正です";
            float[] xyz=ParseUtil.Xyz(sa);
            if(xyz==null) return "座標の指定が不正です";
            GravityTransformControl gtc=MaidUtil.GetGravity(m,cate);
            if(gtc==null) return "変更できません";
            gtc.forceRate=force;
            gtc.isEnabled=true;
            gtc.transform.localPosition=new Vector3(xyz[0],xyz[1],xyz[2]);
        }
        return "";
    }

    private static FieldInfo skinTrField=null;
    private static FieldInfo skinTrSclField=null;
    
    private static int MaidParamClothFollow(ComShInterpreter sh,Maid m,string val){
        const string usage="スロット名.ボーン名{+|-}の形で指定してください";
        if(val==null||val=="") return sh.io.Error(usage);

        try{
            if(skinTrField==null)
                skinTrField=typeof(TBodySkin).GetField("listTrs",BindingFlags.Instance | BindingFlags.NonPublic);
            if(skinTrSclField==null)
                skinTrSclField=typeof(TBodySkin).GetField("listTrsScr",BindingFlags.Instance | BindingFlags.NonPublic);
        }catch{ return sh.io.Error("失敗しました"); }

        string[] va=val.Split(ParseUtil.comma);
        if(va.Length==1 && va[0].IndexOf('.')<0){
            if(!m.body0.IsSlotNo(va[0])) return sh.io.Error("スロット名が不正です");
            var skin=m.body0.GetSlot(m.body0.GetSlotNo(va[0]));
            if(skin==null) return sh.io.Error("スロット名が不正です");
            List<Transform> lstTr,lstScl;
            try{
                lstTr=(List<Transform>)skinTrField.GetValue(skin);
                lstScl=(List<Transform>)skinTrField.GetValue(skin);
                if(lstTr==null||lstScl==null) return sh.io.Error("失敗しました");
            }catch{ return sh.io.Error("失敗しました"); }
            for(int idx=0; idx<lstTr.Count; idx+=2) sh.io.PrintLn(lstTr[idx].name);
            return 0;
        }
        for(int vi=0; vi<va.Length; vi++){
            string v=va[vi];

            char sw=v[v.Length-1];
            if(sw!='+'&&sw!='-') return sh.io.Error(usage);
            var sa=ParseUtil.LeftAndRight(v.Substring(0,v.Length-1),'.');
            if(sa[0]==""||sa[1]=="") return sh.io.Error(usage);
    
            if(!m.body0.IsSlotNo(sa[0])) return sh.io.Error("スロット名が不正です");
            var skin=m.body0.GetSlot(m.body0.GetSlotNo(sa[0]));
            if(skin==null) return sh.io.Error("スロット名が不正です");
    
            List<Transform> lstTr,lstScl;
            try{
                lstTr=(List<Transform>)skinTrField.GetValue(skin);
                lstScl=(List<Transform>)skinTrField.GetValue(skin);
                if(lstTr==null||lstScl==null) return sh.io.Error("失敗しました");
            }catch{ return sh.io.Error("失敗しました"); }
    
            string bname=ParseUtil.CompleteBoneName(sa[1],m.boMAN,m.IsCrcBody);
    
            if(sw=='+'){
                int idx; for(idx=0; idx<lstTr.Count; idx+=2) if(lstTr[idx].name==bname) break;
                if(idx<lstTr.Count) return 1;
                Transform bone,cbone;
                if(!m.body0.m_dicTrans.TryGetValue(bname,out bone)
                || (cbone=CMT.SearchObjName(skin.obj_tr,bname,false))==null) return sh.io.Error("ボーンがありません");
                lstTr.Add(cbone);
                lstTr.Add(bone);
                if(bname=="Mune_L"||bname=="Mune_R"||bname.Contains("chinko")){
                    lstScl.Add(cbone);
                    lstScl.Add(bone);
                }
            }else{
                int idx; for(idx=0; idx<lstTr.Count; idx+=2) if(lstTr[idx].name==bname) break;
                if(idx==lstTr.Count) return 1;
                lstTr[idx+1]=lstTr[lstTr.Count-1]; lstTr.RemoveAt(lstTr.Count-1);
                lstTr[idx]=lstTr[lstTr.Count-1]; lstTr.RemoveAt(lstTr.Count-1);
    
                for(idx=0; idx<lstScl.Count; idx+=2) if(lstScl[idx].name==bname) break;
                if(idx<lstScl.Count){
                    lstScl[idx+1]=lstScl[lstScl.Count-1]; lstScl.RemoveAt(lstScl.Count-1);
                    lstScl[idx]=lstScl[lstScl.Count-1]; lstScl.RemoveAt(lstScl.Count-1);
                }
            }
        }
        return 1;
    }
    private static int MaidParamComponent(ComShInterpreter sh,Maid m,string val){
        return CmdObjects.ObjParamComponent(sh,m.transform,val);
    }
}

public static class MaidUtil {

    // メイド検索＆取得。連番 or 氏名 or guid
    public static Maid FindMaid(string key){return FindMaid(key.AsSpan());}
    public static Maid FindMaid(ReadOnlySpan<char> key){
        if(key.Length==0) return null;
        int n;
        if(int.TryParse(key,out n)) return NthMaid(n); 
        if(key[0]=='%' && int.TryParse(key.Slice(1),out n)) return MaidByInstanceID(n);
        if(key[0]=='@' && int.TryParse(key.Slice(1),out n)) return MaidByStockNo(n);
        return MaidByGuidOrName(key);
    }
    // 男性検索＆取得。連番 or 表示名
    public static Maid FindMan(string key){return FindMan(key.AsSpan());}
    public static Maid FindMan(ReadOnlySpan<char> key){
        if(key.Length==0) return null;
        var k=key;
        int n;
        if(key[0]=='%' && int.TryParse(key.Slice(1),out n)) return ManByInstanceID(n);
        if(k.StartsWith("男",Ordinal)) k=k.Slice(1);
        if(int.TryParse(k,out n)) return NthMan(n);
        else if(MemoryExtensions.Equals(key,"主人公",Ordinal)
             || MemoryExtensions.Equals(key,"御主人様",Ordinal)
             || MemoryExtensions.Equals(key,"ご主人様",Ordinal) ) return NthMan(0);
        else return ManByGuid(key);
    }
    public static Maid FindMaidMan(string type,string key){
        if(type=="maid") return FindMaid(key);
        else if(type=="man") return FindMan(key);
        else return null;
    }
    public static Maid FindMaidManByName(string name){
        return MaidByGuidOrName(name)??FindMan(name);
    }
    public static Maid FindMaidManByGuid(string guid){
        return GameMain.Instance.CharacterMgr.GetMaid(guid)??ManByGuid(guid);
    }
    public static Maid FindMaidManByInstanceID(int id){
        return MaidByInstanceID(id)??ManByInstanceID(id);
    }
    public static Maid NthMaid(int n){
        CharacterMgr cm = GameMain.Instance.CharacterMgr;
        for (int i=0, no=0; i<cm.GetMaidCount(); i++) {
            Maid m=cm.GetMaid(i);
            if (m==null||m.body0==null||m.body0.m_trBones==null) continue;
            if(no++==n) return m;
        }
        return null;
    }
    public static int IndexOfMaid(Maid maid){
        CharacterMgr cm = GameMain.Instance.CharacterMgr;
        for (int i=0, no=0; i<cm.GetMaidCount(); i++) {
            Maid m=cm.GetMaid(i);
            if (m==null||m.body0==null||m.body0.m_trBones==null) continue;
            if(m==maid) return no;
            no++;
        }
        return -1;
    }
    public static Maid MaidByStockNo(int n){
        CharacterMgr cm = GameMain.Instance.CharacterMgr;
        Maid m=cm.GetStockMaid(n);
        if (m==null||m.body0==null||m.body0.m_trBones==null) return null;
        return m;
    }
    public static Maid NthMan(int n){
        CharacterMgr cm = GameMain.Instance.CharacterMgr;
        for (int i=0, no=0; i<cm.GetManCount(); i++) {
            Maid m=cm.GetMan(i);
            if(m==null) continue;
            if(no++==n) return m;
        }
        return null;
    }
    public static int IndexOfMan(Maid maid){
        CharacterMgr cm = GameMain.Instance.CharacterMgr;
        for (int i=0, no=0; i<cm.GetManCount(); i++) {
            Maid m=cm.GetMan(i);
            if (m==null) continue;
            if(m==maid) return no;
            no++;
        }
        return -1;
    }
    public static Maid MaidByGuidOrName(string name){return MaidByGuidOrName(name.AsSpan());}
    public static Maid MaidByGuidOrName(ReadOnlySpan<char> name){
        CharacterMgr cm = GameMain.Instance.CharacterMgr;
        for (int i=0; i<cm.GetMaidCount(); i++) {
        Maid m = cm.GetMaid(i);
        if (m==null) continue;
            if(MemoryExtensions.Equals(name,m.status.guid,Ordinal)) return m;
            if(MemoryExtensions.Equals(name,m.status.fullNameJpStyle.AsSpan().Trim(),Ordinal)) return m;
        }
        return null;
    }
    public static Maid MaidByInstanceID(int id){
        CharacterMgr cm = GameMain.Instance.CharacterMgr;
        for (int i=0; i<cm.GetMaidCount(); i++) {
            Maid m = cm.GetMaid(i);
            if(m==null) continue;
            if(m.GetInstanceID()==id) return m;
        }
        return null;
    }
    public static Maid ManByInstanceID(int id){
        CharacterMgr cm = GameMain.Instance.CharacterMgr;
        for (int i=0; i<cm.GetManCount(); i++) {
            Maid m = cm.GetMan(i);
            if(m==null) continue;
            if(m.GetInstanceID()==id) return m;
        }
        return null;
    }
    public static Maid ManByGuid(string guid){return ManByGuid(guid.AsSpan());}
    public static Maid ManByGuid(ReadOnlySpan<char> guid){
        CharacterMgr cm = GameMain.Instance.CharacterMgr;
        for (int i=0; i<cm.GetManCount(); i++) {
            Maid m=cm.GetMan(i);
            if(m==null) continue;
            if(m.status.guid==guid) return m;
        }
        return null;
    }
    public static int FindNullMaidIdx(){
        CharacterMgr cm = GameMain.Instance.CharacterMgr;
        for (int i=0; i<cm.GetMaidCount(); i++) if(cm.GetMaid(i)==null) return i;
        return -1;
    }
    public static int FindStockMaid(string key){
        CharacterMgr cm = GameMain.Instance.CharacterMgr;
        List<Maid> ml=cm.GetStockMaidList();
        if(int.TryParse(key,out int n))
            if(n>=0 && n<ml.Count) return n; else return -1;
        if(key[0]=='%' && int.TryParse(key.Substring(1),out int iid)){
            for(int i=0; i<ml.Count; i++) if(ml[i].GetInstanceID()==iid) return i;
            return -1;
        }
        for(int i=0; i<ml.Count; i++)
            if(key==ml[i].status.fullNameJpStyle.Trim()||key==ml[i].status.guid) return i;
        return -1;
    }

    // 上限下限チェックなし
    public static void SetPropTemp(Maid m,MPN mpn,int value){
        MaidProp mp=m.GetProp(mpn);
        mp.temp_value=value;
        mp.boDut=false;
        mp.boTempDut=true;
    }
    public static void SetProp(Maid m,MPN mpn,int value){
        MaidProp mp=m.GetProp(mpn);
        mp.value=value;
        mp.boDut=true;
        mp.boTempDut=false;
    }
    public static string GetCloth(Maid m,MPN mpn){
        MaidProp mp=m.GetProp(mpn);
        string name=(!string.IsNullOrEmpty(mp.strTempFileName) && mp.nTempFileNameRID!=0)?mp.strTempFileName:mp.strFileName;
        if (name=="" || name.Contains("_del_") || name.EndsWith("_del.menu",Ordinal)) return string.Empty;
        return name;
    }
    public static MPN[] mpnBody={       // 身体系MPN
        MPN.KubiScl,MPN.UdeScl,MPN.EyeScl,MPN.EyeSclX,MPN.EyeSclY,MPN.EyePosX,MPN.EyePosY,
        MPN.EyeClose,MPN.EyeBallPosX,MPN.EyeBallPosY,MPN.EyeBallSclX,MPN.EyeBallSclY,
        MPN.EarNone,MPN.EarElf,MPN.EarRot,MPN.EarScl,MPN.NosePos,MPN.NoseScl,
        MPN.FaceShape,MPN.FaceShapeSlim,MPN.MayuShapeIn,MPN.MayuShapeOut,MPN.MayuX,MPN.MayuY,MPN.MayuRot,
        MPN.HeadX,MPN.HeadY,MPN.DouPer,MPN.sintyou,MPN.koshi,MPN.kata,MPN.west,MPN.Hara,
        MPN.MayuThick,MPN.MayuLong,MPN.Yorime,
        MPN.MuneL,MPN.MuneS,MPN.MuneM,MPN.MuneTare,MPN.MuneUpDown,MPN.MuneYori,MPN.MuneYawaraka,
        MPN.MunePosX, MPN.MunePosY, MPN.MuneThick, MPN.MuneLong, MPN.MuneDir,
        MPN.DouThick1X, MPN.DouThick1Y, MPN.DouThick2X, MPN.DouThick2Y, MPN.DouThick3X, MPN.DouThick3Y,
        MPN.ShoulderThick, MPN.UpperArmThickX, MPN.UpperArmThickY, MPN.LowerArmThickX, MPN.LowerArmThickY,
        MPN.ElbowThickX, MPN.ElbowThickY, MPN.NeckThickX, MPN.NeckThickY, MPN.HandSize, MPN.DouThick4X, MPN.DouThick4Y,
        MPN.DouThick5X, MPN.DouThick5Y, MPN.WaistPos, MPN.HipSize, MPN.HipRot, MPN.ThighThickX, MPN.ThighThickY,
        MPN.KneeThickX, MPN.KneeThickY, MPN.CalfThickX, MPN.CalfThickY, MPN.AnkleThickX, MPN.AnkleThickY,
        MPN.FootSize, MPN.UpperArmLowerThickX, MPN.UpperArmLowerThickY, MPN.WristThickX, MPN.WristThickY,
        MPN.ClavicleThick, MPN.ShoulderTension, MPN.ThighLowerThickX, MPN.ThighLowerThickY, MPN.ThighShin,
        MPN.HaraN, MPN.ChikubiH, MPN.ChikubiK1, MPN.ChikubiK2, MPN.ChikubiK2_MuneS, MPN.ChikubiR, MPN.ChikubiW,
        MPN.Nyurin1, MPN.Nyurin2, MPN.Nyurin3, MPN.Nyurin4, MPN.Nyurin5, MPN.Nyurin6, MPN.Nyurin7, MPN.Nyurin8,
        MPN.MabutaUpIn, MPN.MabutaUpIn2, MPN.MabutaUpMiddle, MPN.MabutaUpOut, MPN.MabutaUpOut2,
        MPN.MabutaLowIn, MPN.MabutaLowUpMiddle, MPN.MabutaLowUpOut, MPN.Eyedel, MPN.Itome, 
        MPN.Ha1, MPN.Ha2, MPN.Ha3, MPN.Ha4, MPN.Ha5, MPN.Ha6,
        MPN.FutaePosX, MPN.FutaePosY, MPN.FutaeRot, MPN.HitomiHiPosX, MPN.HitomiHiPosY, MPN.HitomiHiSclY,
        MPN.HitomiShapeUp, MPN.HitomiShapeLow, MPN.HitomiShapeIn,MPN.HitomiShapeOutUp, MPN.HitomiShapeOutLow, MPN.HitomiRot,
        MPN.HohoShape, MPN.LipThick, MPN.WearSuso, MPN.KuikomiPants, MPN.KuikomiStkg,
        MPN.MuscleSkin, MPN.Hanasuji, MPN.Washibana, MPN.ChikubiWearTotsu, 

    };
    public static MPN[] mpnClothing={   // 衣装系MPN。動的に変える事がなさそうなのは省く
        MPN.acctatoo,MPN.folder_eye,MPN.eye,MPN.eye_hi,MPN.eye_hi_r,
        MPN.acchat,MPN.headset,MPN.wear,MPN.skirt,MPN.onepiece,MPN.mizugi,MPN.bra,MPN.panz,MPN.stkg,MPN.shoes, 
        MPN.acckami,MPN.megane,MPN.acchead,MPN.acchana,MPN.accmimi,MPN.glove,MPN.acckubi,MPN.acckubiwa,MPN.acckamisub, 
        MPN.accnip,MPN.accude,MPN.accheso,MPN.accashi,MPN.accsenaka,MPN.accshippo,MPN.accxxx,MPN.accvag,MPN.accanl,
        MPN.handitem
    };
    public static TBody.SlotID[] sidClothing={ // undress用。衣装系MPNとだいたいの対応をとる
        TBody.SlotID.accHat,TBody.SlotID.headset,TBody.SlotID.wear,TBody.SlotID.skirt, TBody.SlotID.onepiece,
        TBody.SlotID.mizugi,TBody.SlotID.bra,TBody.SlotID.panz,TBody.SlotID.stkg,TBody.SlotID.shoes,
        TBody.SlotID.accKami_1_,TBody.SlotID.accKami_2_,TBody.SlotID.accKami_3_,
        TBody.SlotID.megane,TBody.SlotID.accHead,TBody.SlotID.accHana,TBody.SlotID.accMiMiL,TBody.SlotID.accMiMiR,
        TBody.SlotID.glove,TBody.SlotID.accKubi,TBody.SlotID.accKubiwa,TBody.SlotID.accKamiSubL,TBody.SlotID.accKamiSubR,
        TBody.SlotID.accNipL,TBody.SlotID.accNipR,TBody.SlotID.accUde,TBody.SlotID.accHeso,TBody.SlotID.accAshi,
        TBody.SlotID.accSenaka,TBody.SlotID.accShippo,TBody.SlotID.accXXX,TBody.SlotID.accVag,TBody.SlotID.accAnl,
        TBody.SlotID.HandItemL,TBody.SlotID.HandItemR
    };
    public static MPN[] sid2mpnClothing={       // 上との1対1対応
        MPN.acchat,MPN.headset,MPN.wear,MPN.skirt,MPN.onepiece,
        MPN.mizugi,MPN.bra,MPN.panz,MPN.stkg,MPN.shoes, 
        MPN.acckami,MPN.acckami,MPN.acckami,
        MPN.megane,MPN.acchead,MPN.acchana,MPN.accmimi,MPN.accmimi,
        MPN.glove,MPN.acckubi,MPN.acckubiwa,MPN.acckamisub, MPN.acckamisub, 
        MPN.accnip,MPN.accnip,MPN.accude,MPN.accheso,MPN.accashi,
        MPN.accsenaka,MPN.accshippo,MPN.accxxx,MPN.accvag,MPN.accanl,
        MPN.handitem,MPN.handitem
    };
    public const string MEKUREF="めくれスカート";
    public const string MEKUREB="めくれスカート後ろ";
    public const string ZURASI="パンツずらし";
    public const string HADAKE="はだけ";
    public static void MekureZurashi(Maid m,int sw,string name,MPN mpn1,MPN mpn2){
        if(sw==1){
            m.ItemChangeTemp(mpn1.ToString(),name);
            m.ItemChangeTemp(mpn2.ToString(),name);
        }else{
            m.ResetProp(mpn1,false);
            m.ResetProp(mpn2,false);
        }
        m.AllProcProp();
    }
    public static bool IsMekureZurashi(Maid m,string name,MPN mpn1,MPN mpn2){
        return ((m.IsItemChange(mpn1.ToString(),name)||m.IsItemChange(mpn2.ToString(),name)));
    }
    public static Transform GetBoneTr(Maid m,string bonename){
        return CMT.SearchObjName(m.body0.m_trBones, bonename, true);
    }
    public static GameObject AddObject(string src, string name, Transform pr, Vector3 pos, Vector3 rot,Vector3 scl){
        GameObject go=ObjUtil.AddObject(src,name,pr,pos,rot,scl);
        return go;
    }

    public const string CRON_LOOKAT="lookat/";
    public const string NODE_LOOKAT="ComSh_Lookat";
    public const string CRON_GRAB="grab/";
    public const string NODE_GRAB_PFX="ComSh_Grab";
    public const string CRON_ATTACH="attach/";
    public const string NODE_ATTACH_PFX="ComSh_AttachBase";

    // 注視ターゲットが消えたら注視をやめるためのjob登録
    // 注視点をターゲットに追随させる
    public static bool LookAtCron(Maid m,Transform tgt){
        string name=CRON_LOOKAT+m.GetInstanceID();
        ComShBg.cron.KillJob(name);
        if(tgt==null) return true;
        Transform tr=MaidLookTarget(m);
        return (ComShBg.cron.AddJob(name,0,0,(long t)=>{
            if (m==null||m.body0==null||m.body0.m_trBones==null||tgt==null) return -1;
            tr.position=tgt.position;
            return 0;
        })!=null);
    }
    public static bool LookAtCron(Maid m,Vector3 tgt){
        Transform tr=MaidLookTarget(m);
        string name=CRON_LOOKAT+m.GetInstanceID();
        ComShBg.cron.KillJob(name);
        return (ComShBg.cron.AddJob(name,0,0,(long t)=>{
            if (m==null||m.body0==null||m.body0.m_trBones==null) return -1;
            tr.position=tgt;
            return 0;
        })!=null);
    }
    // 目や頭の回転計算をMaidのUpdate()に任せるため
    // 自分側に注視点を持ってEyeToTargetObject()
    // スタジオモードならface_to_objectというGameObjectが既にあるけど他で困るので自分で持つ
    public static Transform MaidLookTarget(Maid m){
        Transform tr=GetLookTarget(m);
        if(tr==null){
            GameObject go=new GameObject(NODE_LOOKAT);
            go.transform.localPosition=Vector3.zero;
            go.transform.localRotation=Quaternion.identity;
            go.transform.localScale=Vector3.one;
            go.transform.SetParent(m.transform,false);
            float y=m.body0.trsHead.position.y-m.transform.position.y;
            go.transform.localPosition=new Vector3(0,y,0.2f);
            tr=go.transform;
        }
        m.EyeToTargetObject(tr);
        return tr;
    }
    public static Transform GetLookTarget(Maid m){
        return m.transform.Find(NODE_LOOKAT);
    }
    public static void LookTargetOff(Maid m){
        Transform tr=MaidLookTarget(m);
        tr.localRotation=Quaternion.identity;
        tr.localScale=Vector3.one;
        float y=m.body0.trsHead.position.y-m.transform.position.y;
        tr.localPosition=new Vector3(0,y,0.2f);
    }
    public static void ClrGrabCron(Maid m,string lr){
        string name=CRON_GRAB+lr+m.GetInstanceID().ToString();
        ComShBg.cron.KillJob(name);
        DelIkCtlObj(m,lr);
    }
    public static void DelIkCtlObj(Maid m,string lr){
        int iid=m.GetInstanceID();
        if(!MaidUtil.ikObj.ContainsKey(iid)) return;
        m.fullBodyIK.AllIKDetach(0);
        string[] objnames=MaidUtil.ikObj[iid];
        for(int i=0; i<objnames.Length; i++) if(objnames[i].Length>0){
            Transform tr=ObjUtil.FindObj(null,objnames[i]);
            if(tr!=null){
                if(ObjUtil.objDic.ContainsKey(tr.name)) ObjUtil.objDic.Remove(tr.name);
                UnityEngine.Object.Destroy(tr.gameObject);
            }
        }
        MaidUtil.ikObj.Remove(iid);
        return;
    }
    public static bool GrabCron(Maid m,Transform tgt,string lr){
        Transform tr=MaidGrabTarget(m,lr);
        string name=CRON_GRAB+lr+m.GetInstanceID().ToString();
        ComShBg.cron.KillJob(name);
        if(tgt==null) return true;
        return (ComShBg.cron.AddJob(name,0,0,(long t)=>{
            if(tgt==null) return -1;
            if(m.ActiveSlotNo<0){ DelIkCtlObj(m,lr); return -1;}
            tr.position=tgt.position;
            tr.rotation=tgt.rotation;
            return 0;
        })!=null);
    }
    public static Transform GetGrabTarget(Maid m,string lr){
        string trname=NODE_GRAB_PFX+lr;
        return m.transform.Find(trname);
    }
    public static Transform MaidGrabTarget(Maid m,string lr){
        Transform tr=GetGrabTarget(m,lr);
        if(tr==null){
            GameObject go=new GameObject(NODE_GRAB_PFX+lr);
            go.transform.localPosition=Vector3.zero;
            go.transform.localRotation=Quaternion.identity;
            go.transform.localScale=Vector3.one;
            go.transform.SetParent(m.transform,false);
            tr=go.transform;
        }
        var prm=new kt.ik.IKAttachParam(m,null);
        prm.attachTarget=tr;
        prm.attachType=AIKCtrl.IKAttachType.NewPoint;
        m.fullBodyIK.IKAttach(lr,prm);
        prm.attachType=AIKCtrl.IKAttachType.Rotate;
        m.fullBodyIK.IKAttach(lr,prm);
        return tr;
    }
    public static Maid GetParentMaid(Transform atr){
        Transform tr=atr;
        while(tr!=null&&tr.name!="AllOffset"){
            if(tr.name=="Offset") return tr.parent.GetComponent<Maid>();
            if(tr.name.StartsWith(NODE_ATTACH_PFX))
                tr=tr.GetComponent<AttachBase>().target;
            tr=tr.parent;
        }
        return null;
    }
    private static Transform allOffset; // Activeなメイドの親。CharacterMgrから直接は取れない
    public class AttachBase:MonoBehaviour{
        public Maid maid;
        public Transform target;
    }
    public static Transform MaidAttachBase(Maid m,Transform tgt,bool jmpq){
        Transform tr=GetAttachBase(m);
        if(tr==null){
            if(allOffset==null) allOffset=m.transform.parent;
            GameObject go=new GameObject(NODE_ATTACH_PFX+m.GetInstanceID().ToString(),typeof(AttachBase));
            tr=go.transform;
        }
        tr.SetParent(m.transform.parent);
        tr.position=tgt.position;
        tr.rotation=tgt.rotation;
        tr.localScale=tgt.localScale;
        m.transform.SetParent(tr,jmpq);
        return tr;
    }
    public static Transform GetAttachBase(Maid m){
        string trname=NODE_ATTACH_PFX+m.GetInstanceID().ToString();
        Transform tr=m.transform.parent;
        return (tr.name==trname)?tr:null;
    }
    public static bool AttachCron(Maid m,Transform tgt,int prio,bool jmpq){
        Transform tr=MaidAttachBase(m,tgt,jmpq);
        var ab=tr.GetComponent<AttachBase>();
        ab.target=tgt;
        ab.maid=GetParentMaid(tgt);
        string name=CRON_ATTACH+m.GetInstanceID().ToString();
        ComShBg.cron2.KillJob(name);
        return (ComShBg.cron2.AddJob(name,0,0,(long t)=>{
            if(m.ActiveSlotNo<0||tgt==null){
                m.transform.SetParent(tr.parent,true);
                UnityEngine.Object.Destroy(tr.gameObject);
                return -1;
                }
            tr.position=tgt.position;
            tr.rotation=tgt.rotation;
            tr.localScale=tgt.localScale;
            return 0;
        },prio)!=null);
    }
    public static List<Maid> GetParentMaidList(Transform tgt,Transform dontreachme){
        var ret=new List<Maid>(16);
        Maid tm; Transform tr=tgt;
        while(tr!=null&&tr.name!="AllOffset"){
            if(tr.name=="Offset"){
                tm=tr.parent.GetComponent<Maid>();
                ret.Add(tm);
                tr=tm.transform;
            }else if(tr.name.StartsWith(NODE_ATTACH_PFX,Ordinal)){
                var ab=tr.GetComponent<AttachBase>();
                if(ab.maid!=null){ ret.Add(ab.maid); tr=ab.maid.transform; }
                else tr=ab.target;
            }else tr=tr.parent;
            if(dontreachme!=null && tr==dontreachme) return null;
        }
        return ret;
    }
    public static List<Maid> GetChildMaidList(List<Maid> ml){
        var ret=new List<Maid>(16);
        string name; Transform tr;
        if(allOffset==null) return ret;
        for(int i=0; i<allOffset.childCount; i++){
            tr=allOffset.GetChild(i);
            name=tr.name;
            if(name.StartsWith(NODE_ATTACH_PFX,Ordinal) && int.TryParse(name.Substring(16),out int iid)){
                var ab=tr.GetComponent<AttachBase>();
                if(ab.maid!=null) foreach(Maid m in ml) if(ab.maid==m){ ret.Add(MaidByInstanceID(iid)); break;}
            }
        }
        return ret;
    }

    public static GravityTransformControl GetGravity(Maid m,string cate){// cate:hair/skirt
        string name="GravityDatas_"+m.status.guid+"_"+cate;
        Transform bone=m.body0.trBip;
        Transform gbase=m.transform.Find(name);
        Transform gtr;
        if(gbase!=null){
            gtr=gbase.GetChild(0);
            return gtr.GetComponent<GravityTransformControl>();
        }

        gbase=(new GameObject(name)).transform;
        gbase.SetParent(bone);
        gbase.SetParent(m.transform,true);
        gbase.localScale=Vector3.one;
        gbase.rotation=Quaternion.identity;
        gtr=(new GameObject(name)).transform;
        gtr.SetParent(gbase);
        GravityTransformControl gtc=gtr.gameObject.AddComponent<GravityTransformControl>();
        gtc.transTargetObject = new PhotoTransTargetObject(gtr.gameObject,name,"",PhotoTransTargetObject.Type.Prefab,Vector2.one);
        gtc.SetTargetSlods((cate=="skirt")?sidSkirt:sidHair);
        gtc.forceRate=1.0f;
        return gtc;
    }
    public static void FixGravity(Maid m){
        var t=GetGravity(m,"hair");
        if(t!=null) t.OnChangeMekure();
        t=GetGravity(m,"skirt");
        if(t!=null) t.OnChangeMekure();
    }
    private static TBody.SlotID[] sidHair={     // 髪用重力の対象(それ用のボーンがあれば)
        TBody.SlotID.hairAho, TBody.SlotID.hairF, TBody.SlotID.hairR, TBody.SlotID.hairS, TBody.SlotID.hairT,
        TBody.SlotID.accKami_1_,TBody.SlotID.accKami_2_,TBody.SlotID.accKami_3_,
        TBody.SlotID.accKamiSubL,TBody.SlotID.accKamiSubR,TBody.SlotID.accHat,TBody.SlotID.headset
    };
    private static TBody.SlotID[] sidSkirt={    // スカート用重力の対象(それ用のボーンがあれば)
        TBody.SlotID.wear,TBody.SlotID.skirt, TBody.SlotID.onepiece,
        TBody.SlotID.accUde,TBody.SlotID.accAshi,TBody.SlotID.accSenaka,TBody.SlotID.accShippo,
        TBody.SlotID.glove,TBody.SlotID.accKubi,TBody.SlotID.accKubiwa
    };
    public static string[] origBSKeys={
        "hoho","hoho2","hohos","hohol","tear1","tear2","tear3","namida","yodare","shock"
    };
    public static HashSet<string> origBSKeySet=new HashSet<string>(origBSKeys);

    public static Dictionary<int,string[]> ikObj=new Dictionary<int,string[]>();

    public class MenuHeader{
        public int version;
        public string name;
        public string cate;
    }
    public static MenuHeader ReadMenuHeader(string fileName) {
        byte[] buffer=UTIL.AReadAll(fileName);
        if(buffer==null) return null;
        var mh=new MenuHeader();
        mh.name=mh.cate="";
        try{
            using(BinaryReader br=new BinaryReader(new MemoryStream(buffer))){
                string hdr=br.ReadString();
                if(hdr!="CM3D2_MENU") return null;
                mh.version=br.ReadInt32();
                br.ReadString();
                mh.name=br.ReadString();
                mh.cate=br.ReadString();
            }
        }catch{ return null; }
        return mh;
    }
    public static string GetCurrentMotion(Maid m){
        foreach(AnimationState ast in m.body0.m_Animation)
            if(m.body0.m_Animation.IsPlaying(ast.name) && ast.layer==0 && ast.name.IndexOf(" - Queued Clone")<0){
                if(long.TryParse(ast.name,out long id)) return StudioMode.MyPoseId2Name(id)??ast.name;
                else return ast.name;
            }
        return "";
    }
}
public class Kuchipaku {
    public static Dictionary<int,Kuchipaku> kuchipakuDic=new Dictionary<int,Kuchipaku>();

    public static Kuchipaku Find(Maid m){
        int iid=m.GetInstanceID();
        if(kuchipakuDic.TryGetValue(iid,out Kuchipaku kp)) return kp;
        return null;
    }
    public static Kuchipaku Create(Maid m){
        int iid=m.GetInstanceID();
        if(kuchipakuDic.TryGetValue(iid,out Kuchipaku kp)) return kp;
        return (kuchipakuDic[iid]=new Kuchipaku());
        // お掃除する機会がないけど、通常は3くらい、最大でも40程度なのでまぁ良しとする
    }

    public void SetKeys(string[] keys){
        vowelkeys=keys;
        for(int i=0; i<6; i++) aiueon[i]=new float[keys.Length];
    }
    public int SetVowel(char c,float[] def){
        int idx="あいうえおんaiueon".IndexOf(c);
        if(idx<0) return -1;
        idx%=6;
        if(def.Length!=vowelkeys.Length) return -2;
        aiueon[idx]=def;
        return 0;
    }
    private static string[] kana ={
	    "あいうえおぁぃぅぇぉはひふへほらりるれろ",
	    "かきくけこがぎぐげごさしすせそざじずぜぞたちつてとだぢづでど",	// ややこわばる系
	    "なにぬねのまみむめもぱぴぷぺぽばびぶべぼ",		// 閉じる系
	    "わ　　　を",			// 「う」を経由する系
	    "や　ゆ　よゃ　ゅ　ょ"	// 「い」を経由する系
    };
    public string[] vowelkeys=new string[]{"moutha","mouthc","mouthdw","mouthhe","mouthi","mouths","toothoff"};
    public float[][] aiueon=new float[6][]{
        new float[]{0.3f,0.33f,0.3f,0,0,0.2f,0},
        new float[]{0.02f,0.2f,0.1f,0,0.5f,0.01f,0},
        new float[]{0,0.75f,0.1f,0,0,0,1},
        new float[]{0.05f,0.15f,0.15f,0,0.05f,0.25f,0},
        new float[]{0.15f,0.8f,0.1f,0,0,0,1},
        new float[]{0f,0.33f,0.1f,0.1f,0.05f,0,0}
    };
    public void SetVowelKeys(string[] ka){
        vowelkeys=ka;
        if(aiueon[0].Length!=ka.Length){
            for(int i=0; i<6; i++) aiueon[i]=new float[ka.Length];
        }
    }
    public int SetVowel(int id,float[] va){
        if(va.Length!=vowelkeys.Length) return -1;
        aiueon[id]=va;
        return 0;
    }

    public static Regex regcmd=new Regex(
        @"\G((?<ctr>[TVLQE])(?<len>[1-9]\d*)|(?<rest>[Rっー～])(?<len>[1-9]\d*)?(?<futen>\.*)|(?<note>[あ-ん][ゃゅょ]?)(?<len>[1-9]\d*)?(?<futen>\.*))",
        RegexOptions.Compiled|RegexOptions.IgnoreCase|RegexOptions.ExplicitCapture
    );
    public AnimationCurve[] curves; 
    public float seconds;
    public struct KPEvent {
        public float time;
        public string nstr;
        public KPEvent(float t,int n){time=t; nstr=n.ToString();}
    }
    public List<KPEvent> kpevents;
    public int kpevIdx=0;
    public int UpdateShapes(Maid m,float t){
        if(kpevents!=null&&kpevents.Count>0){
            if(kpevents[0].time>=t) kpevIdx=0;
            for(; kpevIdx<kpevents.Count && kpevents[kpevIdx].time<=t; kpevIdx++)
                Command.DoPublish("kuchipakuevent"+kpevents[kpevIdx].nstr,m.ActiveSlotNo.ToString());
        }
        TBodySkin face=m.body0.Face;
        if(face==null||face.morph==null) return -1;
        var hash=face.morph.hash;
        for(int i=0; i<vowelkeys.Length; i++){
            string key=vowelkeys[i];
            if(!hash.ContainsKey(key)) continue;
            float v=curves[i].Evaluate(t);
            face.morph.SetValueBlendSet(m.ActiveFace,key,v*100);
        }
        return (seconds<=t)?1:0;
    }
    public int SetMML(string mml){
        curves=new AnimationCurve[vowelkeys.Length];
        for(int i=0; i<vowelkeys.Length; i++){
            curves[i]=new AnimationCurve();
            addkf(curves[i],0,aiueon[5][i]);
        }
        kpevents=new List<KPEvent>();

        float[] max=new float[vowelkeys.Length];
        for(int i=0; i<vowelkeys.Length; i++){
            max[i]=0;
            for(int j=0; j<5; j++) if(max[i]<aiueon[j][i]) max[i]=aiueon[j][i];
        }

        int tempo=120,l=4;
        float mul=1,mv=1,t=0;
        string nmml=normalize(mml);
        var m=regcmd.Match(nmml);
        while(m.Success){
            if(m.Groups["ctr"].Success){
                if(!m.Groups["len"].Success) return -1;
                int n=int.Parse(m.Groups["len"].Value);
                char c=char.ToLower(m.Groups["ctr"].Value[0]);
                switch(c){
                case 't': tempo=n; break;
                case 'v': mul=n/100f; break;
                case 'l': l=n; break;
                case 'q': mv=n/100f; break;
                case 'e': kpevents.Add(new KPEvent(t,n)); break;
                }
            }else if(m.Groups["rest"].Success){
                int rl=l;
                if(m.Groups["len"].Success) rl=int.Parse(m.Groups["len"].Value);
                int ft=0;
                if(m.Groups["futen"].Success) ft=m.Groups["futen"].Value.Length;
                float rdt=l2t(tempo,futen(rl,ft));
                t+=rdt;
                for(int i=0; i<vowelkeys.Length; i++) addkf(curves[i],t,curves[i][curves[i].length-1].value);
            }else if(m.Groups["note"].Success){
                string note=m.Groups["note"].Value;
                char o1=note[0];
                char o2=note.Length>1?note[1]:'\0';
                int nl=l;
                if(m.Groups["len"].Success) nl=int.Parse(m.Groups["len"].Value);
                int ft=0;
                if(m.Groups["futen"].Success) ft=m.Groups["futen"].Value.Length;
                float ndt=l2t(tempo,futen(nl,ft));
                float atk=ndt*mv;
                float[] def;
                if(o2!='\0'){
                    int b=kana[4].IndexOf(o2);
                    if(b<0) return -1;
                    b=b%5;
                    def=aiueon[b];
                    for(int i=0; i<vowelkeys.Length; i++) addkf(curves[i],t+atk/2,mul*Mathf.Lerp(aiueon[1][i],def[i],0.4f));
                }else{
                    int b=-1;
                    if(o1=='ん'){
                        def=aiueon[5];
                    }else{
                        int si=0;
                        for(; si<kana.Length; si++){
                            b=kana[si].IndexOf(o1);
                            if(b>=0) break;
                        }
                        if(b<0) return -1;
                        b=b%5;
                        def=aiueon[b];
                        if(si==1){
                            for(int i=0; i<max.Length; i++) addkf(curves[i],t+atk/2,mul*Mathf.Lerp(max[i],def[i],0.7f));
                        }else if(si==2){
                            for(int i=0; i<vowelkeys.Length; i++) addkf(curves[i],t+atk/2,mul*Mathf.Lerp(aiueon[5][i],def[i],0.1f));
                        }else if(si==3){
                            for(int i=0; i<vowelkeys.Length; i++) addkf(curves[i],t+atk/2,mul*Mathf.Lerp(aiueon[2][i],def[i],0.3f));
                        }else if(si==4){
                            for(int i=0; i<vowelkeys.Length; i++) addkf(curves[i],t+atk/2,mul*Mathf.Lerp(aiueon[1][i],def[i],0.4f));
                        }
                    }
                }
                for(int i=0; i<vowelkeys.Length; i++) addkf(curves[i],t+atk,mul*def[i]);
                t+=ndt;
                if(ndt>atk) for(int i=0; i<vowelkeys.Length; i++) addkf(curves[i],t,mul*def[i]);
            }
            m=m.NextMatch();
        }
        seconds=t*1000f;

        compact();

        return 0;
    }
    private int addkf(AnimationCurve ac,float t,float v){ return ac.AddKey(new Keyframe(Mathf.Round(t),fval(v),0,0)); }
    private float fval(float f){ return (float)Math.Round((double)f,5,MidpointRounding.AwayFromZero); }
    private float l2t(int tempo,float l){ return 60f / tempo * l * 4 * 1000; }
	private float futen(float len,int nn){ return (2-1f/(1<<nn))/len; }

    private void compact(){
        for(int i=0; i<vowelkeys.Length; i++){
            var cv=curves[i];
            if(cv.length<=1) continue;
            var cv2=new AnimationCurve();
            cv2.AddKey(cv[0]);
            int j=1;
            for(; j<cv.length-1; j++)
                if(cv[j].value!=cv[j-1].value || cv[j].value!=cv[j+1].value) cv2.AddKey(cv[j]);
            cv2.AddKey(cv[j]);
            curves[i]=cv2;
        }
    }

    public static Regex regnormal=new Regex(
        @"\s+|[フふヴゔ][ぁぃぇぉァィェォ]?",
        RegexOptions.Compiled
    );
    public string normalize(string mml){
        StringBuilder sb=new StringBuilder(mml.Length);
        var m=regnormal.Match(mml);
        int s=0;
        while(m.Success){
            if(m.Index>s) sb.Append(mml.Substring(s,m.Index-s));
            int i=m.Index;
            s=m.Index+m.Value.Length;
            if(char.IsWhiteSpace(m.Value[0])){ m=m.NextMatch(); continue; }
            char c=m.Value[0];
            if(c=='フ'||c=='ふ'){
                if(m.Value.Length==1) sb.Append('ふ');
                else switch(m.Value[1]){
                case 'ぁ': case 'ァ': sb.Append('わ'); break;
                case 'ぃ': case 'ィ': sb.Append('ひ'); break;
                case 'ぇ': case 'ェ': sb.Append('へ'); break;
                case 'ぉ': case 'ォ': sb.Append('を'); break;
                }
            }else if(c=='ヴ'||c=='ゔ'){
                if(m.Value.Length==1) sb.Append('ぶ');
                else switch(m.Value[1]){
                case 'ぁ': case 'ァ': sb.Append('ば'); break;
                case 'ぃ': case 'ィ': sb.Append('び'); break;
                case 'ぇ': case 'ェ': sb.Append('べ'); break;
                case 'ぉ': case 'ォ': sb.Append('ぼ'); break;
                }
            }
            m=m.NextMatch();
        }
        if(s==0) return mml;
        if(s<mml.Length) sb.Append(mml.Substring(s));
        return sb.ToString();
    }
}
}
