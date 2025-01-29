using System;
using UnityEngine;
using System.Collections.Generic;
using static System.StringComparison;
using System.Reflection;

namespace COM3D2.ComSh.Plugin {
public static class StudioMode {
    public static PhotoWindowManager pwm; 

    public static void SceneChg(string name){
        if(name!="ScenePhotoMode"){ pwm=null; return;}
        pwm=GameObject.FindObjectOfType<PhotoWindowManager>();
        InitOnLoad();
    }

    public static T GetWindow<T>(PhotoWindowManager.WindowType type) where T:PhotoWindow{
        return (pwm==null)?null:(T)pwm.GetWindow(type);
    }
    public static PlacementWindow GetPlacementWindow(){
        return (pwm==null)?null:pwm.GetWindow(PhotoWindowManager.WindowType.Placement) as PlacementWindow;
    }
    public static MotionWindow GetMotionWindow(){
        return (pwm==null)?null:pwm.GetWindow(PhotoWindowManager.WindowType.Motion) as MotionWindow;
    }

    public static void OnMotionChange(Maid m){
        if(pwm==null) return;
        int i;
        // モーション変更時にレイヤ2以降掃除
        var mw=GetMotionWindow();
        if(mw==null) return;
        var lst=mw.PopupAndTabList.onSelect;
        for(i=0; i<lst.Count; i++) if(lst[i]==layerSweepHdr) break;
        if(i==lst.Count) lst.Insert(0,StudioMode.layerSweepHdr);

        // ポーズエディットON時にレイヤ2以降掃除
        var pew=GetWindow<PoseEditWindow>(PhotoWindowManager.WindowType.PoseEdit);
        if(pew==null) return;
        pew.OnMotionUpdate(m);
        var pelst=pew.CheckbtnUse.onClick;
        for(i=0; i<pelst.Count; i++) if(pelst[i]==peLayerSweepHdr) break;
        if(i==pelst.Count) pelst.Insert(0,peLayerSweepHdr); // AddだとON時もOFF時も引数がTrue
    }

    // GUIからのモーション変更で追加レイヤが残らないように
    private static Action<object> layerSweepHdr=new Action<object>(OnMotionItemSelect);
    private static void OnMotionItemSelect(object item){
        if(pwm==null) pwm=GameObject.FindObjectOfType<PhotoWindowManager>();
        if(pwm==null) return;
        Maid m=pwm.select_maid;
        Animation anim=m.GetAnimation();
        var remove=new List<string>(10);
        foreach(AnimationState state in anim) if(state.layer>0) remove.Add(state.name);
        foreach(var name in remove){
            var ac=anim.GetClip(name);
            anim.RemoveClip(name);
            UnityEngine.Object.Destroy(ac);
        }
    }
    // ポーズエディットON時に追加レイヤが残らないように
    private static Action<WFCheckBox> peLayerSweepHdr=new Action<WFCheckBox>(OnPoseEditStart);
    private static void OnPoseEditStart(WFCheckBox ck){
        if(pwm==null) pwm=GameObject.FindObjectOfType<PhotoWindowManager>();
        if(pwm==null) return;
        Maid m=pwm.select_maid;
        Animation anim=m.GetAnimation();
        if(ck.check){
            // removeするとOFF時(の後の確認ダイアログOK時)に例外が出るので無力化のみ
            foreach(AnimationState st in anim) st.weight=0;
        }else{
            // 確認ダイアログOK時に掃除
            var dlg=GameMain.Instance.SysDlg.gameObject;
            var okbtn=UTY.GetChildObject(dlg.gameObject,"Base/Ok").GetComponent<UIButton>();
            EventDelegate.Add(okbtn.onClick,OnPeOffOkClick,true);
        }
    }

    public static void FixMyposeIK(Maid m){
        if(pwm==null) return;
        var go=GameObject.Find("IKDragPointParent/"+m.status.guid);
        if(go==null) return;
        var ca=go.GetComponentsInChildren<IKDragPoint>();
        if(ca==null) return;
        for(int i=0; i<ca.Length; i++)
            if(ca[i]!=null) ca[i].SetTargetIKPoint(ca[i].target_ik_point_trans.gameObject);
    }

    // 男head,body変更 スタジオモードUI経由
    public static int ManHeadBodyStudio(Maid m,string item){
        if(pwm==null) return 0;
        Maid orig=pwm.select_maid;
        var pw=GetPlacementWindow();
        if(pw==null) return 0;
        int no=m.ActiveSlotNo;
        string manname=(no==0)?"主人公":$"男{no}";
        MaidSelectStudio(pw,"",manname);
        int ret=ManHeadBodyStudio(item);
        if(orig.boMAN){
            no=orig.ActiveSlotNo;
            manname=(no==0)?"主人公":$"男{no}";
            MaidSelectStudio(pw,"",manname);
        }else MaidSelectStudio(pw,orig.status.lastName,orig.status.firstName);
        return ret;
    }
    public static int ManHeadBodyStudio(string item){
        var fw=GetWindow<FaceWindow>(PhotoWindowManager.WindowType.Face);
        if(fw==null) return 0;
        UIWFTabPanel panel;
        if(item.StartsWith("mbody",Ordinal)) panel=fw.BodyItemTabPanel;
        else if(item.StartsWith("mhead",Ordinal)) panel=fw.HeadItemTabPanel;
        else return 0;
        string rid=item.ToLower().GetHashCode().ToString();
        var btns=panel.GetComponentsInChildren<UIWFTabButton>();
        for(int i=0; i<btns.Length; i++) if(btns[i].name==rid){
            for(int j=0; j<i; j++) btns[j].SetSelect(false);
            for(int j=i+1; j<btns.Length; j++) btns[j].SetSelect(false);
            btns[i].SetSelect(true);
            return 1;
        }
        return 0;
    }
    public static int ManBodyChange(Maid man,out Maid pairman){
        pairman=null;
        var pl=GetWindow<PlacementWindow>(PhotoWindowManager.WindowType.Placement);
        if(pl==null) return 0;
        var paneltype=typeof(PlacementWindow).GetNestedType("PlateData",BindingFlags.NonPublic);
        if(paneltype==null) return -1;
        var fi_panelmaid=paneltype.GetField("maid",BindingFlags.Instance|BindingFlags.Public);
        if(fi_panelmaid==null) return -1;
        var maidslottype=typeof(PlacementWindow).GetNestedType("MaidSlotData",BindingFlags.NonPublic);
        if(maidslottype==null) return -1;
        var fi_slotmaid=maidslottype.GetField("maid",BindingFlags.Instance|BindingFlags.Public);
        if(fi_slotmaid==null) return -1;
        var fi_maidlist=typeof(PlacementWindow).GetField("maid_data_list_",BindingFlags.Instance|BindingFlags.NonPublic);
        if(fi_maidlist==null) return -1;
        var fi_activelist=typeof(PlacementWindow).GetField("active_maid_list_",BindingFlags.Instance|BindingFlags.NonPublic);
        if(fi_activelist==null) return -1;
        var fi_transdic=typeof(PlacementWindow).GetField("transtarget_maid_dic_",BindingFlags.Instance|BindingFlags.NonPublic);
        if(fi_transdic==null) return -1;
        var mi_select=typeof(PlacementWindow).GetMethod("SetSelectMaid",BindingFlags.Instance|BindingFlags.NonPublic);
        if(mi_select==null) return -1;
        var mi_activate=typeof(PlacementWindow).GetMethod("ActiveMaid",BindingFlags.Instance|BindingFlags.NonPublic);
        if(mi_activate==null) return -1;

        if(man.pairMan==null) return -1;

        CharacterMgr cm=GameMain.Instance.CharacterMgr;

        var selected=pl.mgr.select_maid;
        mi_select.Invoke(pl,new object[]{man});

        /*pl.mgr.OnMaidRemoveEventPrev(man);
        cm.CharaVisible(man.ActiveSlotNo,false,true);
        pl.mgr.OnMaidRemoveEvent(man);*/
        pl.DeActiveMaid(man,false);

        // body入れ替え
        cm.SetActiveMan(man,man.ActiveSlotNo);
        pairman=cm.SwapNewManBody(man.ActiveSlotNo,!man.IsCrcBody);
        if(pairman.IsCrcBody&&pairman.IsNewManIsRealMan) pairman.SwapNewRealManProp(true);

        // UIが持ってるmanを差し換え
        var list=(System.Collections.IList)fi_maidlist.GetValue(pl);
        for(int i=0; i<list.Count; i++){
            var m=(Maid)fi_panelmaid.GetValue(list[i]);
            if(m==man){ fi_panelmaid.SetValue(list[i],pairman); break;}
        }
        list=(System.Collections.IList)fi_activelist.GetValue(pl);
        for(int i=0; i<list.Count; i++){
            var m=(Maid)fi_slotmaid.GetValue(list[i]);
            if(m==man){ fi_slotmaid.SetValue(list[i],pairman); break;}
        }
        var dic=(Dictionary<Maid,PhotoTransTargetObject>)fi_transdic.GetValue(pl);
        if(dic.ContainsKey(man)) dic.Remove(man);

        // 新bodyで再配置
        cm.SetActiveMan(pairman,pairman.ActiveSlotNo);
        pairman.Visible=true;
        if(pairman.boAllProcPropBUSY){
            pairman.boAllProcPropBUSY=false;
            pairman.AllProcProp();
        }

        pl.mgr.OnMaidAddEvent(pairman,false);

        if(selected==man) selected=pairman;
        mi_select.Invoke(pl,new object[]{selected});
        return 1;
    }

    private static void OnPeOffOkClick(){
        Maid m=pwm.select_maid;
        Animation anim=m.GetAnimation();
        foreach(AnimationState st in anim) if(st.layer==0 && st.weight==0) st.weight=1f;
    }

    // スタジオモードでマイポーズ再生するとクリップ名が数値になる
    // 数値を手がかりにUIの表示名からファイル名を取る
    public static string MyPoseId2Name(long id){
        MotionWindow mw=GetMotionWindow();
        if(mw==null) return null;
        var pmdm=PhotoMotionData.data;
        if(pmdm==null) return null;
        if(PhotoMotionData.category_list==null||!PhotoMotionData.category_list.ContainsKey("マイポーズ")) return null;
        var pmds=PhotoMotionData.category_list["マイポーズ"];
        if(pmdm==null) return null;
        for(int i=0; i<pmds.Count; i++) if(pmds[i].id==id) return System.IO.Path.GetFileName(pmds[i].direct_file);
        return null;
    }

    // スタジオモード用のGUI経由のメイド追加。普通にActivate()だとスタジオモード終了時にフリーズ
    public static int MaidAddStudio(PlacementWindow pw,string lname,string fname){
        return MaidClickStudio(pw,lname,fname,"Plate/TopButton");
    }
    // GUIからメイド選択
    public static int MaidSelectStudio(PlacementWindow pw,string lname,string fname){
        return MaidClickStudio(pw,lname,fname,"Button");
    }
    private static int MaidClickStudio(PlacementWindow pw,string lname,string fname,string path){
        UIGrid grid=UTY.GetChildObject(pw.content_game_object, "ListParent/Contents/UnitParent", false).GetComponent<UIGrid>();
        if(grid==null) return -1;
        SimpleMaidPlate[] mp=grid.transform.GetComponentsInChildren<SimpleMaidPlate>();
        if(mp==null) return -1;
        for(int i=0; i<mp.Length; i++){
            UILabel[] lbl=mp[i].GetComponentsInChildren<UILabel>();
            if(lbl==null || lbl.Length<3) continue;
            if((lname==null||lbl[1].text==lname) && (fname==null||lbl[2].text==fname)){
                UIButton b=UTY.GetChildObject(mp[i].gameObject,path).GetComponent<UIButton>();
                if(b==null || b.onClick==null || b.onClick.Count==0) continue;
                UIButton.current=b;
                EventDelegate.Execute(b.onClick);
                UIButton.current=null;
                return 0;
            }
        }
        return -1;
    }
    private static FieldInfo targetlist=typeof(ObjectManagerWindow).GetField("target_list_",BindingFlags.Instance | BindingFlags.NonPublic);
    public static List<Transform> GetObjectList(){
        var omw=GetWindow<ObjectManagerWindow>(PhotoWindowManager.WindowType.ObjectManager);
        if(omw==null) return null;
        var ret=new List<Transform>();
        try{
            var list=(List<PhotoTransTargetObject>)targetlist.GetValue(omw);
            foreach(var pto in list)
                if(pto.type==PhotoTransTargetObject.Type.Prefab) ret.Add(pto.obj.transform);
        }catch{}
        return ret;
    }

    // ロード完了のタイミングを拾うための細工
    private static void InitOnLoad(){
        var loadbtn=UTY.GetChildObject(pwm.gameObject, "WindowVisibleBtnsParent/WindowVisibleBtnsLine2/Grid/Load").GetComponent<UIButton>();
        // 画面下のロードボタン押下時
        if(loadbtn!=null) EventDelegate.Add(loadbtn.onClick,OnGrid2LoadBtnClick);
    }

    private static void OnGrid2LoadBtnClick(){
        PhotSaveLoagDataUnit[] units=pwm.SaveAndLoadManager.DataUnitParent.transform.GetComponentsInChildren<PhotSaveLoagDataUnit>();
        for(int i=0; i<units.Length; i++){
            // ロード画面 各データクリック時
            units[i].onClick+=new Action<PhotSaveLoagDataUnit>(OnLoadUnitClick);
        }
    }
    private static string command;
    private static void OnLoadUnitClick(PhotSaveLoagDataUnit unit){
        command=FindCmd(unit.comment);
        if(command=="") return;

        if(!UTY.GetChildObject(pwm.SaveAndLoadManager.gameObject, "TitleGroup/TitleLoad").activeInHierarchy) return;

        // ロード確認ダイアログ OK押下時
        var dlg=GameMain.Instance.SysDlg.gameObject;
        var msglbl=UTY.GetChildObject(dlg.gameObject,"Base/Message").GetComponent<UILabel>();
        if(msglbl!=null&&msglbl.text.Contains("ロード")){
            var okbtn=UTY.GetChildObject(dlg.gameObject,"Base/Ok").GetComponent<UIButton>();
            EventDelegate.Add(okbtn.onClick,OnLoadDlgOkClick,true);
        }
    }

    // コメントからコマンド($$[～]$$)を取り出す
    private static string FindCmd(string txt){
            int s=txt.IndexOf("$$[",Ordinal);
            if(s<0) return "";
            s+=3;
            if(s>=txt.Length) return "";
            int e=txt.IndexOf("]$$",s,Ordinal);
            if(e<0) e=txt.Length;
            return txt.Substring(s,e-s);
    }

    private static void OnLoadDlgOkClick(){
        // cronで暗転明けを待ってからコマンドを実行
        ComShBg.cron.AddJob("studio/onload",0,0,(long t)=>{
            if(pwm==null) return -1;
            if(pwm.SaveAndLoadManager.LoadReplaceMaidPanel.gameObject.activeInHierarchy
                || GameMain.Instance.MainCamera.IsFadeOut()) return 0;
            ComShWM.terminal.shell.InterpretBg(command);
            return -1;
        },0);
    }
    //private static void OnLoadDlgCancelClick(){}
}
}
