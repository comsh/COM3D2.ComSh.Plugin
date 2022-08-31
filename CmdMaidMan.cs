using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static System.StringComparison;
using static COM3D2.ComSh.Plugin.Command;
using System;
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
        maidParamDic.Add("opos",new CmdParam<Maid>(MaidParamOPos));
        maidParamDic.Add("lrot",new CmdParam<Maid>(MaidParamLRot));
        maidParamDic.Add("lrot.x",new CmdParam<Maid>(MaidParamLRotX));
        maidParamDic.Add("lrot.y",new CmdParam<Maid>(MaidParamLRotY));
        maidParamDic.Add("lrot.z",new CmdParam<Maid>(MaidParamLRotZ));
        maidParamDic.Add("wpos",new CmdParam<Maid>(MaidParamWPos));
        maidParamDic.Add("wpos.x",new CmdParam<Maid>(MaidParamWPosX));
        maidParamDic.Add("wpos.y",new CmdParam<Maid>(MaidParamWPosY));
        maidParamDic.Add("wpos.z",new CmdParam<Maid>(MaidParamWPosZ));
        maidParamDic.Add("wrot",new CmdParam<Maid>(MaidParamWRot));
        maidParamDic.Add("wrot.x",new CmdParam<Maid>(MaidParamWRotX));
        maidParamDic.Add("wrot.y",new CmdParam<Maid>(MaidParamWRotY));
        maidParamDic.Add("wrot.z",new CmdParam<Maid>(MaidParamWRotZ));
        maidParamDic.Add("orot",new CmdParam<Maid>(MaidParamORot));
        maidParamDic.Add("scale",new CmdParam<Maid>(MaidParamScale));
        maidParamDic.Add("scale.x",new CmdParam<Maid>(MaidParamScaleX));
        maidParamDic.Add("scale.y",new CmdParam<Maid>(MaidParamScaleY));
        maidParamDic.Add("scale.z",new CmdParam<Maid>(MaidParamScaleZ));
        maidParamDic.Add("motion",new CmdParam<Maid>(MaidParamMotion));
        maidParamDic.Add("motion.time",new CmdParam<Maid>(MaidParamMotionTime));
        maidParamDic.Add("motion.speed",new CmdParam<Maid>(MaidParamMotionSpeed));
        maidParamDic.Add("motion.layer",new CmdParam<Maid>(MaidParamMotionLayer));
        maidParamDic.Add("motion.length",new CmdParam<Maid>(MaidParamMotionLength));
        maidParamDic.Add("motion.weight",new CmdParam<Maid>(MaidParamMotionWeight));
        maidParamDic.Add("lookat",new CmdParam<Maid>(MaidParamLookAt));
        maidParamDic.Add("iid",new CmdParam<Maid>(MaidParamIid));
        maidParamDic.Add("attach",new CmdParam<Maid>(MaidParamAttach));
        maidParamDic.Add("detach",new CmdParam<Maid>(MaidParamDetach));
        maidParamDic.Add("list",new CmdParam<Maid>(MaidParamList));
        maidParamDic.Add("ik",new CmdParam<Maid>(MaidParamIK));
        maidParamDic.Add("preset",new CmdParam<Maid>(MaidParamPreset));
        maidParamDic.Add("cloth",new CmdParam<Maid>(MaidParamCloth));
        maidParamDic.Add("mekure",new CmdParam<Maid>(MaidParamMekure));
        maidParamDic.Add("mekureF",new CmdParam<Maid>(MaidParamMekureF));
        maidParamDic.Add("mekureB",new CmdParam<Maid>(MaidParamMekureB));
        maidParamDic.Add("zurashi",new CmdParam<Maid>(MaidParamZurashi));
        maidParamDic.Add("undress",new CmdParam<Maid>(MaidParamUndress));
        maidParamDic.Add("shape",new CmdParam<Maid>(MaidParamShape));
        maidParamDic.Add("style",new CmdParam<Maid>(MaidParamStyle));
        maidParamDic.Add("face",new CmdParam<Maid>(MaidParamFace));
        maidParamDic.Add("blink",new CmdParam<Maid>(MaidParamBlink));
        maidParamDic.Add("facesave",new CmdParam<Maid>(MaidParamFaceSave));
        maidParamDic.Add("faceset",new CmdParam<Maid>(MaidParamFaceset));
        maidParamDic.Add("sing",new CmdParam<Maid>(MaidParamSing));
        maidParamDic.Add("voice",new CmdParam<Maid>(MaidParamVoice));
        maidParamDic.Add("gravityS",new CmdParam<Maid>(MaidParamGravityS));
        maidParamDic.Add("gravityH",new CmdParam<Maid>(MaidParamGravityH));
        maidParamDic.Add("lquat",new CmdParam<Maid>(MaidParamLQuat));
        maidParamDic.Add("wquat",new CmdParam<Maid>(MaidParamWQuat));
        maidParamDic.Add("prot",new CmdParam<Maid>(MaidParamPRot));
        maidParamDic.Add("pquat",new CmdParam<Maid>(MaidParamPQuat));
        maidParamDic.Add("muneyure",new CmdParam<Maid>(MaidParamMuneYure));
        maidParamDic.Add("ap",new CmdParam<Maid>(MaidParamAttachPoint));
        maidParamDic.Add("handle",new CmdParam<Maid>(MaidParamHandle));
        maidParamDic.Add("describe",new CmdParam<Maid>(MaidParamDesc));
        maidParamDic.Add("node",new CmdParam<Maid>(MaidParamNode));

        maidParamDic.Add("l2w",new CmdParam<Maid>(MaidParamL2W));
        maidParamDic.Add("w2l",new CmdParam<Maid>(MaidParamW2L));

        CmdParamPosRotCp(maidParamDic,"lpos","position");
        CmdParamPosRotCp(maidParamDic,"lpos","pos");
        CmdParamPosRotCp(maidParamDic,"lrot","rotation");
        CmdParamPosRotCp(maidParamDic,"lrot","rot");

        for(int i=0; i<manParams.Length; i++) manParamDic.Add(manParams[i],maidParamDic[manParams[i]]);
    }
    private static Dictionary<string,CmdParam<Maid>> maidParamDic=new Dictionary<string,CmdParam<Maid>>();
    private static Dictionary<string,CmdParam<Maid>> manParamDic=new Dictionary<string,CmdParam<Maid>>();
    private static string[] manParams={
        "position","rotation","scale","pos","rot",
        "motion","shape","iid","list","motion.time","motion.speed","motion.layer","motion.length","motion.weight",
        "attach","detach","lookat","ik",
        "wpos","wrot","lpos","lrot","opos","orot","lquat","wquat",

        "position.x","position.y","position.z",
        "pos.x","pos.y","pos.z",
        "lpos.x","lpos.y","lpos.z",
        "wpos.x","wpos.y","wpos.z",
        "rotation.x","rotation.y","rotation.z",
        "rot.x","rot.y","rot.z",
        "lrot.x","lrot.y","lrot.z",
        "wrot.x","wrot.y","wrot.z",
        "scale.x","scale.y","scale.z",
        "prot","pquat",
        "ap","handle","describe",
        "l2w","w2l"
    };

 	private static int CmdMaid(ComShInterpreter sh,List<string> args) {
		if(args.Count==1){
			CharacterMgr cm=GameMain.Instance.CharacterMgr;
			int cnt=0;
			for (int i=0; i<cm.GetMaidCount(); i++) {
				Maid m=cm.GetMaid(i);
				if (m==null) continue;
				sh.io.PrintJoinLn(sh.ofs,cnt.ToString(),m.status.fullNameJpStyle.Trim(),sh.fmt.FPos(m.GetPos()));
				cnt++;
			}
            return 0;
        }else if(args[1]=="add"){
            if(args.Count<3) return sh.io.Error("メイドさんを指定してください");
            for(int i=2; i<args.Count; i++){
			    CharacterMgr cm=GameMain.Instance.CharacterMgr;
                int stockidx=MaidUtil.FindStockMaid(args[i]);
                if(stockidx<0) return sh.io.Error("存在しないメイドさんです");
                Maid sm=cm.GetStockMaid(stockidx);
                if(cm.GetMaid(sm.status.guid)!=null) return sh.io.Error("そのメイドさんは既に配置されています");
                int idx=MaidUtil.FindNullMaidIdx();
                if(idx==-1) return sh.io.Error("これ以上追加できません");

                PlacementWindow pw=GameObject.FindObjectOfType<PlacementWindow>();
                if(pw==null){
                    Maid m=cm.Activate(idx,stockidx,false,false);
                    m.Visible=true;
                    m.AllProcProp(); // Seqの方使っても他の事何もできないんだから、ブロックするのと変わらんでしょ
                    m.CrossFade("maid_stand01.anm",false,true,false,0f);
				    m.FaceAnime("通常");
                    m.FaceBlend("オリジナル");
                }else{
                    int ret=MaidAddStudio(pw,sm,sm.status.lastName,sm.status.firstName);
                    if(ret<0) return sh.io.Error("失敗しました");
                }
            }
            return 0;
        }else if(args[1]=="del"){
            if(args.Count<3) return sh.io.Error("メイドさんを指定してください");
            PlacementWindow pw=GameObject.FindObjectOfType<PlacementWindow>();
            if(pw==null){
			    CharacterMgr cm=GameMain.Instance.CharacterMgr;
                List<int> asn=new List<int>();
                for(int i=2; i<args.Count; i++){
                    Maid m=MaidUtil.FindMaid(args[i]);
                    if(m!=null) asn.Add(m.ActiveSlotNo);
                }
                foreach(int n in asn) cm.Deactivate(n,false);
            }else{
                List<Maid> ml=new List<Maid>();
                for(int i=2; i<args.Count; i++){
                    Maid m=MaidUtil.FindMaid(args[i]);
                    if(m!=null) ml.Add(m);
                }
                foreach(Maid m in ml) pw.DeActiveMaid(m);
            }
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
    // スタジオモード用のGUI経由のメイド追加。普通にActivate()だとスタジオモード終了時にフリーズ
    private static int MaidAddStudio(PlacementWindow pw,Maid m,string lname,string fname){
        // 簡単な解決方法がなさそうなので面倒だけどUI経由で処理
        UIGrid grid=UTY.GetChildObject(pw.content_game_object, "ListParent/Contents/UnitParent", false).GetComponent<UIGrid>();
        if(grid==null) return -1;
        for(int i=0; i<grid.gameObject.transform.childCount; i++){
            Transform tr=grid.gameObject.transform.GetChild(i);
            SimpleMaidPlate[] mp=tr.GetComponentsInChildren<SimpleMaidPlate>();
            if(mp==null) continue;
            UIButton[] btn=tr.GetComponentsInChildren<UIButton>();
            if(btn==null) continue;
            for(int j=0; j<mp.Length; j++){
                UILabel[] lbl=mp[j].GetComponentsInChildren<UILabel>();
                if(lbl==null || lbl.Length<3) continue;
                if((lname==null||lbl[1].text==lname) && (fname==null||lbl[2].text==fname)){
                    if(btn[j].onClick==null || btn[j].onClick.Count==0) continue;
                    UIButton.current=btn[j];
                    btn[j].onClick[0].Execute();
                    UIButton.current=null;
                    m.boAllProcPropBUSY=false;
                    m.AllProcProp();
                    return 0;
                }
            }
        }
        return -1;
    }
 	public static int CmdMaidSub(ComShInterpreter sh,string id, List<string> args,int prmstart){
        Maid m=MaidUtil.FindMaid(id);
        if(m==null) return sh.io.Error("指定されたメイドは存在しません");
        if(args.Count==prmstart){
            sh.io.Print($"iid:{m.GetInstanceID().ToString()}\n");
            sh.io.Print($"guid:{m.status.guid}\n");
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
                PlacementWindow pw=GameObject.FindObjectOfType<PlacementWindow>();
                if(pw==null){
    			    CharacterMgr cm=GameMain.Instance.CharacterMgr;
                    cm.SetActiveMan(m,m.ActiveSlotNo);
                    m.Visible=true;
                    m.AllProcProp();
                    m.CrossFade("man_porse01.anm",false,true,false,0f);
                }else{
                    int ret=MaidAddStudio(pw,m,null,m.ActiveSlotNo==0?"主人公":("男"+m.ActiveSlotNo));
                    if(ret<0) return sh.io.Error("失敗しました");
                }
            }
            return 0;
        }else if(args[1]=="del"){
            if(args.Count<3) return sh.io.Error("男性を指定してください");
            PlacementWindow pw=GameObject.FindObjectOfType<PlacementWindow>();
            List<Maid> del=new List<Maid>();
            for(int i=2; i<args.Count; i++){
                Maid m=MaidUtil.FindMan(args[i]);
                if(m!=null) del.Add(m);
            }
            if(pw==null) foreach(Maid dm in del) dm.Visible=false;
            else foreach(Maid dm in del) pw.DeActiveMaid(dm);
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
    private static string SingleMotion(Maid m,MotionName.Clip clip,bool q=false){
        string name=UTIL.Suffix(clip.name,".anm");
        string myposename=ComShInterpreter.myposeDir+name;
        string id=name;
        if(clip.layer>0 && clip.official_layer<0){
            string seq=UTIL.GetSeqId();
            id=$"{clip.name}_l_{clip.layer}_#{seq}.anm";
        }
        byte[] array;
        AnmFile af;
        try{
            if(File.Exists(myposename)){  // マイポーズに存在するファイルならそちらを再生
                array=File.ReadAllBytes(myposename); 
                af=new AnmFile(array);
                if(m.boMAN!=(af.gender==1)) array=af.ChgGender();
                m.SetAutoTwist(Maid.AutoTwist.ShoulderL,true);
                m.SetAutoTwist(Maid.AutoTwist.ShoulderR,true);
                m.SetAutoTwist(Maid.AutoTwist.WristL,true);
                m.SetAutoTwist(Maid.AutoTwist.WristR,true);
                m.SetAutoTwist(Maid.AutoTwist.ThighL,true);
                m.SetAutoTwist(Maid.AutoTwist.ThighR,true);
            }else{
                array=UTIL.AReadAll(name);
                af=new AnmFile(array);
                if(m.boMAN!=(af.gender==1)) array=af.ChgGender();
            }
        }catch{ return null; }
        if(array==null||array.Length==0) return null;
        if(clip.type==0 && q==false){ // ベースのモーションなら胸モーション有効/無効の判断
            if(!m.boMAN){
                int iid=m.GetInstanceID();
                int yrq=5;
                if(muneMotionYureq.ContainsKey(iid)) yrq=muneMotionYureq[iid];
                int l=yrq>>2,r=yrq&3;
                bool lq=(l==2||(l==1&&af.useMuneL==0)),rq=(r==2||(r==1&&af.useMuneR==0));
                m.body0.MuneYureL(lq?1f:0f);
                m.body0.jbMuneL.enabled=lq;
                m.body0.MuneYureR(rq?1f:0f);
                m.body0.jbMuneR.enabled=rq;
            }
        }
        return m.body0.CrossFade(id,array,clip.type=='&',false,q,clip.fade);
    }
    private static int MaidParamMotion(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            var s=MaidUtil.GetCurrentMotion(m);
            if(s!="") sh.io.Print(s);
            return 0;
        }
        if(val==""){
            GameMain.Instance.ScriptMgr.StopMotionScript();
            m.StopAnime();
            return 1;
        }
        if(val[0]!='+'&&val[0]!=':'&&val[0]!='&'){    // お掃除
            GameMain.Instance.ScriptMgr.StopMotionScript();
            Animation anim=m.GetAnimation();
            var remove=new List<AnimationClip>();
            foreach(AnimationState state in anim)
                if(!anim.IsPlaying(state.name)||state.layer>0||state.name.IndexOf(" - Queued Clone",Ordinal)>=0)
                    remove.Add(state.clip);
            foreach(var clip in remove) anim.RemoveClip(clip);
        }

        bool updated=false;
        var ml=ParseMotion(m,val);
        if(ml==null) return sh.io.Error("モーションの書式が不正です");
        for(int i=0; i<ml.list.Count; i++){
            var cl=ml.list[i];
            bool q=cl.type!=0;
            for(int j=0; j<cl.list.Count; j++){
                MotionName.Clip clip=cl.list[j];
                string tag=SingleMotion(m,clip,q);
                if(tag==null) return sh.io.Error("指定されたモーションが見つかりません");
                updated=true;
                foreach(AnimationState st in m.GetAnimation()) if(st.name.StartsWith(tag,Ordinal)){
                    // ベースなら完全一致、キューに入れたときは後ろに何かついてるものだけ
                    if(q == (st.name.Length==tag.Length)) continue;
                    st.blendMode=(clip.type=='&')?AnimationBlendMode.Additive:AnimationBlendMode.Blend;
                    st.speed=clip.speed;
                    st.time=clip.time;
                    st.weight=clip.weight;
                    // st.layer=clip.layer;
                    if(i<ml.list.Count-1) st.wrapMode=WrapMode.Once;
                    else if(clip.loop==0) st.wrapMode=WrapMode.Once;
                    else if(clip.loop==1) st.wrapMode=WrapMode.Loop;
                    else if(clip.loop==2) st.wrapMode=WrapMode.PingPong;
                    else if(clip.loop==3) st.wrapMode=WrapMode.ClampForever;
                    if(clip.tr!=null) for(int k=0; k<clip.tr.Count; k++)
                        st.AddMixingTransform(clip.tr[k].tr,clip.tr[k].single==0);
                }
            }
        }
        if(updated){
            MotionWindow mw=GameObject.FindObjectOfType<MotionWindow>();
            if(mw!=null){
                PoseEditWindow pew=mw.mgr.GetWindow(PhotoWindowManager.WindowType.PoseEdit) as PoseEditWindow;
                pew.OnMotionUpdate(m);
                int i;

                // モーション変更時にレイヤ2以降掃除
                var lst=mw.PopupAndTabList.onSelect;
                for(i=0; i<lst.Count; i++) if(lst[i]==layerSweepHdr) break;
                if(i==lst.Count) lst.Insert(0,layerSweepHdr);

                // ポーズエディットON時にレイヤ2以降掃除
                var pelst=pew.CheckbtnUse.onClick;
                for(i=0; i<lst.Count; i++) if(lst[i]==peLayerSweepHdr) break;
                if(i==lst.Count) pelst.Insert(0,peLayerSweepHdr); // AddだとON時もOFF時も引数がTrue
            }
        }
        return 1;
    }
    // スタジオモードUIからのモーション変更で追加レイヤが残る事があるので面倒を見る
    private static Action<object> layerSweepHdr=new Action<object>(OnMotionItemSelect);
    private static void OnMotionItemSelect(object item){
        MotionWindow mw=GameObject.FindObjectOfType<MotionWindow>();
        if(mw==null) return;
        Maid m=mw.mgr.select_maid;
        Animation anim=m.GetAnimation();
        var remove=new List<AnimationClip>();
        foreach(AnimationState state in anim) if(state.layer>0) remove.Add(state.clip);
        foreach(var clip in remove) anim.RemoveClip(clip);
    }
    // スタジオモード　ポーズエディットON時に追加レイヤが邪魔になるので消す
    private static Action<WFCheckBox> peLayerSweepHdr=new Action<WFCheckBox>(OnPoseEditStart);
    private static void OnPoseEditStart(WFCheckBox ck){
        MotionWindow mw=GameObject.FindObjectOfType<MotionWindow>();
        if(mw==null) return;
        Maid m=mw.mgr.select_maid;
        Animation anim=m.GetAnimation();
        if(ck.check){
            // removeするとOFF時(の後の確認ダイアログOK時)に例外が出るので無力化のみ
            foreach(AnimationState st in anim) st.weight=0;
        }else{
            // この時点ではまだ確認ダイアログが開くだけ
            // でもダイアログのOK押下イベントには処理を追加できそうにないので、ここでやる
            foreach(AnimationState st in anim) if(st.layer==0 && st.weight==0) st.weight=1f;
        }
    }

    private static int MaidParamMotionTime(ComShInterpreter sh,Maid m,string val){
        var anm=m.body0.m_Animation;
        if(val==null){
            bool multi=false;
            foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){ if(st.layer>0){multi=true; break;} }
            if(multi){
                foreach(AnimationState st in anm) if(anm.IsPlaying(st.name))
                    sh.io.PrintLn(st.layer.ToString()+':'+sh.fmt.FInt((st.time%st.length)*1000));
            }else{
                foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){
                    sh.io.PrintLn(sh.fmt.FInt((st.time%st.length)*1000));
                    break;
                }
            }
            return 0;
        }
        string[] sa;
        if(val.IndexOf('\n')>=0) sa=val.Split(ParseUtil.cr);
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
                if(lno<0 || lno==st.layer) st.time=(f/1000)%st.length;
            }
        }
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
        if(val.IndexOf('\n')>=0) sa=val.Split(ParseUtil.cr);
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
        if(val==null){
            foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){
                sh.io.PrintJoinLn(sh.ofs,st.layer.ToString(),sh.fmt.FInt(st.weight));
            }
            return 0;
        }
        string[] sa;
        if(val.IndexOf('\n')>=0) sa=val.Split(ParseUtil.cr);
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
        foreach(AnimationState st in anm) if(anm.IsPlaying(st.name)){
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

    public class MotionName{
        public List<ClipList> list=new List<ClipList>();

        public class ClipList{
            public int type=0;
            public List<Clip> list=new List<Clip>();
        }

        public class Clip{
            public int type=0;
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
                    c.name=val.Substring(m0,idx-m0);
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
                        if(c.tr==null) c.tr=new List<MotionName.MixTr>();
                        c.tr.Add(new MotionName.MixTr(bn.boneTr,single));
                    }
                }else{
                    c.name=val.Substring(m0,j-m0);
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
        if(val==null) return 0;
        CharacterMgr cm=GameMain.Instance.CharacterMgr;
        CharacterMgr.Preset preset=null;
        string fname=cm.PresetDirectory+"\\"+UTIL.Suffix(val,".preset");
        TextAsset ta=Resources.Load<TextAsset>("Preset/" + val);
        if(ta!=null) preset=cm.PresetLoadFromResources(val);
        else if(File.Exists(fname)) preset=cm.PresetLoad(fname);
        Resources.UnloadAsset(ta);
        if(preset==null) return sh.io.Error("プリセットファイルが読み込めません");
        cm.PresetSet(m,preset);
        m.boAllProcPropBUSY=false;
        m.AllProcProp();
        return 1;
    }
    private static int MaidParamCloth(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            string ret;
            foreach(MPN mpn in MaidUtil.mpnClothing)
                if((ret=MaidUtil.GetCloth(m,mpn))!=string.Empty) sh.io.Print($"{mpn}:{ret}\n");
            return 0;
        }
        if(val=="") return 1;
        string[] menus=val.Split(ParseUtil.comma);
        for(int i=0; i<menus.Length; i++){
            string fname=UTIL.Suffix(menus[i],".menu");
            byte[] buf=MaidUtil.ReadMenu(fname,out string cate);
            if(buf==null) return sh.io.Error("menuファイルの読込に失敗しました");
            m.SetProp(cate,fname,0,true);
            if(cate=="eye_hi") m.SetProp(MPN.eye_hi_r,fname,0,true);
        }
        m.body0.FixMaskFlag();
        m.body0.FixVisibleFlag();
        m.AllProcProp();
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
    private static int MaidParamShape(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            var dic=new SortedDictionary<string,float>();  // 重複削除とソート
            foreach (TBodySkin skin in m.body0.goSlot) if (skin!=m.body0.Face && skin.morph != null)
                foreach (string mk in skin.morph.hash.Keys) if(!dic.ContainsKey(mk))
                    dic.Add(mk,skin.morph.GetBlendValues((int)skin.morph.hash[mk]));
            foreach(string mk in dic.Keys) sh.io.PrintLn2(mk+":",sh.fmt.F0to1(dic[mk]));
            return 0;
        }
        var kvs=ParseUtil.GetKVFloat(val);
        if(kvs==null) return sh.io.Error(ParseUtil.error);
        foreach (TBodySkin skin in m.body0.goSlot) if(skin!=m.body0.Face && skin.morph!=null){
            bool dirty=false;
            foreach(string dk in kvs.Keys) if(skin.morph.hash.ContainsKey(dk)){
                skin.morph.SetBlendValues((int)skin.morph.hash[dk],kvs[dk]);
                dirty=true;
            }
            if(dirty) skin.morph.FixBlendValues();
        }
        return 1;
    }
    private static int MaidParamStyle(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            for(int i=0; i<MaidUtil.mpnBody.Length; i++){
                MaidProp mp=m.GetProp(MaidUtil.mpnBody[i]);
                if(mp!=null) sh.io.PrintLn2(MaidUtil.mpnBody[i]+":",sh.fmt.F0to1(mp.value));
            }
            return 0;
        }
        Dictionary<string,float> kvs=ParseUtil.GetKVFloat(val);
        if(kvs==null) return sh.io.Error(ParseUtil.error);
        foreach(string k in kvs.Keys){
            string lk=k.ToLower();
            int i; for(i=0; i<MaidUtil.mpnBody.Length; i++) if(lk==MaidUtil.mpnBody[i].ToString().ToLower()) break;
            if(i==MaidUtil.mpnBody.Length) return sh.io.Error("MPNが不正です");
            m.SetProp(MaidUtil.mpnBody[i],(int)kvs[k],true);
        }
        m.AllProcProp();
        return 1;
    }
    private static int MaidParamLookAt(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            Transform trs=m.body0.trsLookTarget;
            if(trs!=null) sh.io.PrintLn(sh.fmt.FPos(trs.position));
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
            }else if(te[0]=="obj"){  // オブジェクトを見る
                m.LockHeadAndEye(false);
                Transform tr=ObjUtil.FindObj(sh,te[1]);
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
        if(sa.Length>2) return sh.io.Error("パラメータの書式が不正です");
        int n=0;
        if(sa.Length==2&&!int.TryParse(sa[1],out n)) return sh.io.Error("数値の指定が不正です");
        m.LipSyncEnabled(false);
        am.Stop();
        if(sa[0]!=string.Empty){
            m.LipSyncEnabled(true);
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
        if(val==null){ return 0; }

        string[] sa=val.Split(ParseUtil.comma);
        for(int i=0; i<sa.Length; i++){
            var t=sa[i];
            if(t.Length==0) continue;
            var p=t.Substring(t.Length-1,1);
            if(p!="+" && p!="-"){ p="+"; } else t=t.Substring(0,t.Length-1);
            string[] lr=ParseUtil.LeftAndRight(t,'.');
            if(lr[1]=="") m.body0.SetVisibleNodeSlot("body",p[0]=='+',lr[0]);
            else m.body0.SetVisibleNodeSlot(lr[0],p[0]=='+',ParseUtil.CompleteBoneName(lr[1],false,false));
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
        var fw=GameObject.FindObjectOfType<FaceWindow>();
        if(fw==null) return sh.io.Error("このコマンドはスタジオモード専用です");
        fw.FaceMorphInput.CreateBackupData(m);
        return 1;
    }
    private static int MaidParamFace(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            var dic=new SortedDictionary<string,float>();  // ソート
            TBodySkin face=m.body0.Face;
            if(face!=null&&face.morph!=null){
                foreach (string dk in face.morph.hash.Keys)
                    dic[dk]=face.morph.GetBlendValues((int)face.morph.hash[dk]);
                foreach(string dk in dic.Keys) sh.io.PrintLn2($"{dk}:",sh.fmt.F0to1(dic[dk]));
            }
            return 0;
        }
        if(m.FaceName3=="") m.FaceBlend("オリジナル");
        var kvs=ParseUtil.GetKVFloat(val);
        if(kvs==null) return sh.io.Error(ParseUtil.error);
        TBodySkin skin=m.body0.Face;
        if (skin==null || skin.morph==null) return sh.io.Error("処理に失敗しました");
        var hash=skin.morph.hash;
        string suffix="";
        if(skin.morph.bodyskin.PartsVersion>=120) suffix=TMorph.crcFaceTypesStr[(int)skin.morph.GetFaceTypeGP01FB()];
        foreach(string dk in kvs.Keys){
            if(hash.ContainsKey(dk)){
                if(MaidUtil.origBSKeySet.Contains(dk)) skin.morph.dicBlendSet["オリジナル"][(int)hash[dk]]=kvs[dk];
                else skin.morph.SetValueBlendSet(m.ActiveFace,dk,kvs[dk]*100);
            }else if(dk.StartsWith("eyeclose",Ordinal) && suffix!=""){
                string dk2=(dk.Length==8)?(dk+"1"+suffix):(dk+suffix);
                if(hash.ContainsKey(dk2))
                    skin.morph.SetValueBlendSet(m.ActiveFace,dk2,kvs[dk]*100);
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
        m.boMabataki=(sw==1);
        return 1;
    }
    private static int MaidParamSing(ComShInterpreter sh,Maid m,string val){
        if(val==null) return 0;
        m.StopKuchipakuPattern();
        ComShBg.cron.KillJob("maidsing/"+m.status.guid);
        if(val==string.Empty) return 1;
        TextAsset ta=Resources.Load("SceneDance/"+val) as TextAsset;
        if(ta==null && ta.text=="") return sh.io.Error("口パクデータが取得できません");
        m.LipSyncEnabled(true);
        m.StartKuchipakuPattern(0,weat(ta.text),true);
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
        if(sa.Length==3){
            tr=ObjUtil.FindObj(sh,sa);
            if(tr==null) return sh.io.Error("対象がみつかりません");
        }else if(sa.Length==2 && sa[0]=="obj"){
            tr=ObjUtil.FindObj(sh,sa[1]);
            if(tr==null) return sh.io.Error("対象がみつかりません");
        }else if(sa.Length==1){
            tr=ObjUtil.FindObj(sh,sa[0]);
            if(tr==null) return sh.io.Error("対象がみつかりません");
        }else return sh.io.Error("アタッチ先の指定が不正です");

        // tarnsform.parentでつなぐと親メイド退場時に心中してしまうのでcronでやる
        var ms=MaidUtil.GetParentMaidList(tr,m.transform);
        if(ms==null) return sh.io.Error("親子関係がループになるため、アタッチできません");
        int prio=1;
        for(int i=ms.Count-1; i>=0; i--)    // 親ほど先になるようcron実行順を更新
            ComShBg.cron.ChangePriority(MaidUtil.CRON_ATTACH+ms[i].GetInstanceID().ToString(),prio++);
        if(jmpq==2) UTIL.ResetTr(m.transform);
        MaidUtil.AttachCron(m,tr,prio,jmpq==0);
        (ms=new List<Maid>()).Add(m);
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
            ComShBg.cron.KillJob(MaidUtil.CRON_ATTACH+m.GetInstanceID().ToString());
        }else return sh.io.Error("アタッチされていません");
        return 1;
    }
    private static int MaidParamList(ComShInterpreter sh,Maid m,string val){
        List<string> ls=new List<string>();
        if(val=="ap"){
            foreach(TBodySkin skin in m.body0.goSlot){
                if(skin.morph==null) continue;
                foreach(string ap in skin.morph.dicAttachPoint.Keys) ls.Add(ap);
            }
        }else if(val=="bone"){
            CharacterMgr cm=GameMain.Instance.CharacterMgr;
            if(cm.TryGetCacheObject(m.body0.goSlot[0].m_strModelFileName,out GameObject go)){
                UTIL.TraverseTr(go.transform,(Transform tr)=>{
                    if(!tr.name.StartsWith("_BO_",Ordinal)) ls.Add(tr.name);
                    return 0;
                });
            }
        }else if(val=="all"){
            // 絶対長いのでログへ。ソートもしない
            UTIL.TraverseTr(m.transform,(Transform tr)=>{ ls.Add(tr.name); return 0; });
        }else return sh.io.Error("ap、boneのいずれかを指定してください");
        ls.Sort();
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
        if(val.Length!=2) sh.io.Error("値の形式が不正です");
        lc=val[0]; rc=val[1];
        if(lc=='X'||lc=='x') l=0; else if(lc=='-') l=1; else if(lc=='O'||lc=='o') l=2; else l=-1;
        if(rc=='X'||rc=='x') r=0; else if(rc=='-') r=1; else if(rc=='O'||rc=='o') r=2; else r=-1;
        if(l<0||r<0) sh.io.Error("値の形式が不正です");
        muneMotionYureq[iid]=l*4+r;
        bool lq=l>1, rq=r>1;
        m.body0.MuneYureL(lq?1f:0f);
        m.body0.jbMuneL.enabled=lq;
        m.body0.MuneYureR(rq?1f:0f);
        m.body0.jbMuneR.enabled=rq;
        return 1;
    }

    private static int MaidParamAttachPoint(ComShInterpreter sh,Maid m,string val){
        if(val==null){
            foreach(TBodySkin skin in m.body0.goSlot){
                if(skin.morph==null) continue;
                foreach(var ap in skin.morph.dicAttachPoint){
                    sh.io.PrintLn($"{ap.Key}");
                }
            }
            return 0;
        }
        foreach(TBodySkin skin in m.body0.goSlot){
            if(skin.morph==null) continue;
            foreach(var ap in skin.morph.dicAttachPoint) if(ap.Key==val){
                Vector3 v,s; Quaternion q;
                skin.morph.GetAttachPoint(val,out v,out q,out s);
                sh.io.PrintLn2("position:",sh.fmt.FPos(v));
                sh.io.PrintLn2("rotation:",sh.fmt.FEuler(q.eulerAngles));
                //sh.io.PrintLn2("scale:",sh.fmt.FMul(s));
                break;
            }
        }
        return 1;
    }
    private static int MaidParamHandle(ComShInterpreter sh,Maid m,string val){
        if(val==null)
            return sh.io.Error("off|lpos|wpos||rot|wrot|scale|lposrot|wposrotのいずれかを指定してください");

        var hdl=ComShHandle.GetHandle(m.transform);
        var sw=val.ToLower();
        if(sw=="off"){
            if(hdl!=null) ComShHandle.DelHandle(hdl);
            return 1;
        }
        if(hdl==null){ hdl=ComShHandle.AddHandle(m); hdl.Visible=true;}
        if(ComShHandle.SetHandleType(hdl,sw)<0)
            return sh.io.Error("off|lpos|wpos||rot|wrot|scale|lposrot|wposrotのいずれかを指定してください");
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
            ObjUtil.objDic.Add(tr.name,tr);
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
        m.AllIKDetach(0f);
        return 1;
    }
    private static Transform CreateGrabNode(ComShInterpreter sh,Maid m,string name,bool leftq,bool handq){
        Transform pftr=ObjUtil.GetPhotoPrefabTr(sh,true);
        if(pftr==null) return null;
        Transform ptr=null;
        string bip=(m.boMAN?"ManBip":"Bip01");
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
}

public static class MaidUtil {

    // メイド検索＆取得。連番 or 氏名 or guid
    public static Maid FindMaid(string key){
        if(key.Length==0) return null;
        int n;
        if(key[0]=='%' && int.TryParse(key.Substring(1),out n)) return MaidByInstanceID(n);
        if(key[0]=='@' && int.TryParse(key.Substring(1),out n)) return MaidByStockNo(n);
        if(int.TryParse(key,out n)) return NthMaid(n); 
        else return MaidByGuidOrName(key);
    }
    // 男性検索＆取得。連番 or 表示名
    public static Maid FindMan(string key){
        if(key.Length==0) return null;
        string k=key;
        int n;
        if(key[0]=='%' && int.TryParse(key.Substring(1),out n)) return ManByInstanceID(n);
        if(k.StartsWith("男",Ordinal)) k=k.Substring(1);
        if(int.TryParse(k,out n)) return NthMan(n);
        else if(key=="主人公"||key=="御主人様"||key=="ご主人様") return NthMan(0); // やりすぎ感
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
            if (m==null) continue;
            if(no++==n) return m;
        }
        return null;
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
    public static Maid MaidByGuidOrName(string name){
        CharacterMgr cm = GameMain.Instance.CharacterMgr;
        for (int i=0; i<cm.GetMaidCount(); i++) {
        Maid m = cm.GetMaid(i);
        if (m==null) continue;
            if(name==m.status.guid) return m;
            if(name==m.status.fullNameJpStyle.Trim()) return m;
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
    public static Maid ManByGuid(string guid){
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

    public static string GetCloth(Maid m,MPN mpn){
        MaidProp mp=m.GetProp(mpn);
        string name=mp.strTempFileName;
        if(name=="") name=mp.strFileName;
        if (name==""||mp.strFileName.EndsWith("_del.menu",Ordinal)) return string.Empty;
        return name;
    }
    public static MPN[] mpnBody={       // 身体系MPN
        MPN.KubiScl,MPN.UdeScl,
        MPN.EyeScl,MPN.EyeSclX,MPN.EyeSclY,MPN.EyePosX,MPN.EyePosY,
        MPN.EyeClose,MPN.EyeBallPosX,MPN.EyeBallPosY,MPN.EyeBallSclX,MPN.EyeBallSclY,
        MPN.EarNone,MPN.EarElf,MPN.EarRot,MPN.EarScl,MPN.NosePos,MPN.NoseScl,
        MPN.FaceShape,MPN.FaceShapeSlim,MPN.MayuShapeIn,MPN.MayuShapeOut,MPN.MayuX,MPN.MayuY,MPN.MayuRot,
        MPN.HeadX,MPN.HeadY,MPN.DouPer,MPN.sintyou,MPN.koshi,MPN.kata,MPN.west,
        MPN.MuneUpDown,MPN.MuneYori,MPN.MuneYawaraka,MPN.MayuThick,MPN.MayuLong,MPN.Yorime
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
            if(m.ActiveSlotNo<0||tgt==null){
                return -1;  // nullっぽく振る舞うDestroy後の残骸
            }
            tr.position=tgt.position;     // Vector3の値渡し。クラスの参照渡しと見分け難いのがC#の構造体
            return 0;
        })!=null);
    }
    public static bool LookAtCron(Maid m,Vector3 tgt){
        Transform tr=MaidLookTarget(m);
        string name=CRON_LOOKAT+m.GetInstanceID();
        ComShBg.cron.KillJob(name);
        return (ComShBg.cron.AddJob(name,0,0,(long t)=>{
            if(m.ActiveSlotNo<0) return -1;
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
            go.transform.SetParent(m.transform);
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
        IKCtrlData ikd = m.IKCtrl.GetIKData(lr);
        ikd.SetIKSetting(IKCtrlData.IKAttachType.NewPoint,IKCtrlData.IKExecTiming.Normal,null,Vector3.zero,false);
        ikd.SetIKSetting(IKCtrlData.IKAttachType.Rotate,IKCtrlData.IKExecTiming.Normal,null,Vector3.zero,false);
        int iid=m.GetInstanceID();
        if(!MaidUtil.ikObj.ContainsKey(iid)) return;
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
            go.transform.SetParent(m.transform);
            tr=go.transform;
        }
        IKCtrlData ikd = m.IKCtrl.GetIKData(lr);
        ikd.SetIKSetting(IKCtrlData.IKAttachType.NewPoint,IKCtrlData.IKExecTiming.Normal,tr,Vector3.zero,false);
        ikd.SetIKSetting(IKCtrlData.IKAttachType.Rotate,IKCtrlData.IKExecTiming.Normal,tr,Vector3.zero,false);
        return tr;
    }
    public static Maid GetParentMaid(Transform atr){
        Transform tr=atr;
        while(tr!=null&&tr.name!="AllOffset"){
            if(tr.name=="Offset") return tr.parent.gameObject.GetComponent<Maid>();
            if(tr.name.StartsWith(NODE_ATTACH_PFX))
                tr=tr.gameObject.GetComponent<AttachBase>().target;
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
        var ab=tr.gameObject.GetComponent<AttachBase>();
        ab.target=tgt;
        ab.maid=GetParentMaid(tgt);
        string name=CRON_ATTACH+m.GetInstanceID().ToString();
        ComShBg.cron.KillJob(name);
        return (ComShBg.cron.AddJob(name,0,0,(long t)=>{
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
        var ret=new List<Maid>();
        Maid tm; Transform tr=tgt;
        while(tr!=null&&tr.name!="AllOffset"){
            if(tr.name=="Offset"){
                tm=tr.parent.gameObject.GetComponent<Maid>();
                ret.Add(tm);
                tr=tm.transform;
            }else if(tr.name.StartsWith(NODE_ATTACH_PFX,Ordinal)){
                var ab=tr.gameObject.GetComponent<AttachBase>();
                if(ab.maid!=null){ ret.Add(ab.maid); tr=ab.maid.transform; }
                else tr=ab.target;
            }else tr=tr.parent;
            if(dontreachme!=null && tr==dontreachme) return null;
        }
        return ret;
    }
    public static List<Maid> GetChildMaidList(List<Maid> ml){
        var ret=new List<Maid>();
        string name; Transform tr;
        if(allOffset==null) return ret;
        for(int i=0; i<allOffset.childCount; i++){
            tr=allOffset.GetChild(i);
            name=tr.name;
            if(name.StartsWith(NODE_ATTACH_PFX,Ordinal) && int.TryParse(name.Substring(16),out int iid)){
                var ab=tr.gameObject.GetComponent<AttachBase>();
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
            return gtr.gameObject.GetComponent<GravityTransformControl>();
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

    public static byte[] ReadMenu(string fileName,out string cate) {
        cate="";
        byte[] buffer=UTIL.AReadAll(fileName);
        if(buffer==null) return null;
        try{
            using(BinaryReader br=new BinaryReader(new MemoryStream(buffer))){
                string hdr=br.ReadString();
                if(hdr!="CM3D2_MENU") return null;
                br.ReadInt32(); br.ReadString(); br.ReadString();
                cate=br.ReadString();
            }
        }catch{ return null; }
        return buffer;
    }

    // スタジオモードUIはMyPoseだけクリップ名を数値にしている。
    // 内部での扱いはそのまま、表示する時点でファイル名に置き換える
    public static string MyPoseId2Name(long id){
        MotionWindow mw = GameObject.FindObjectOfType<MotionWindow>();
        if(mw==null) return null;
        var pmdm=PhotoMotionData.data;
        if(pmdm==null) return null;
        if(PhotoMotionData.category_list==null||!PhotoMotionData.category_list.ContainsKey("マイポーズ")) return null;
        var pmds=PhotoMotionData.category_list["マイポーズ"];
        if(pmdm==null) return null;
        for(int i=0; i<pmds.Count; i++) if(pmds[i].id==id) return Path.GetFileName(pmds[i].direct_file);
        return null;
    }
    public static string GetCurrentMotion(Maid m){
        foreach(AnimationState ast in m.body0.m_Animation)
            if(m.body0.m_Animation.IsPlaying(ast.name) && ast.layer==0 && ast.name.IndexOf(" - Queued Clone")<0){
                if(long.TryParse(ast.name,out long id)) return MyPoseId2Name(id)??ast.name;
                else return ast.name;
            }
        return "";
    }
}
}
