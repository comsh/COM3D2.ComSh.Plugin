using System.Collections.Generic;
using UnityEngine;

namespace COM3D2.ComSh.Plugin {

public class ComShHandle:GizmoRender {
    public Transform target;
    public Maid maid;
    public int type=0; // 0:global / 1:local / 2:object

    // オブジェクトにハンドルを付ける
    // 子にはせず、常時追従させる

    public override void OnRenderObject(){
        if(target==null || (maid!=null &&(maid.body0==null || maid.body0.m_trBones==null))){
            UTIL.RemoveFromList(hdllst,this);
            GameObject.Destroy(this.gameObject);
            GameObject.Destroy(this);
            return;
        }
        transform.position=target.position;
        transform.localScale=target.localScale;
        if(type==0) transform.rotation=Quaternion.identity;
        else if(type==1) transform.rotation=target.parent.rotation;
        else transform.rotation=target.rotation;
 
        base.OnRenderObject();

        if(target.position!=transform.position) target.position=transform.position;
        if(target.localScale!=transform.localScale) target.localScale=transform.localScale;

        if(type==0){
            var q=transform.rotation * target.rotation;
            if(target.rotation!=q) target.rotation=q;
        }else if(type==1){
            var q=transform.rotation * Quaternion.Inverse(target.parent.rotation) * target.rotation;
            if(target.rotation!=q) target.rotation=q;
        }else{
            if(target.rotation!=transform.rotation) target.rotation=transform.rotation;
        }
    }
    public void SetHandleType(int t,int p,int r,int s){
        type=t; eAxis=(p!=0); eRotate=(r!=0); eScal=(s!=0);
    }

    private static List<ComShHandle> hdllst=new List<ComShHandle>();
    public static void Clear(){
        foreach(var h in hdllst){
            GameObject.Destroy(h.gameObject);
            GameObject.Destroy(h);
        }
        hdllst.Clear();
    }
    public static ComShHandle GetHandle(Transform tr){
        for(int i=0; i<hdllst.Count; i++) if(hdllst[i].target==tr) return hdllst[i];
        return null;
    }
    public static ComShHandle AddHandle(Maid m){
        var go=new GameObject();
        var hdl=go.AddComponent<ComShHandle>();
        hdl.target=m.transform;
        hdl.maid=m;
        hdllst.Add(hdl);
        return hdl;
    }
    public static ComShHandle AddHandle(Transform tr){
        var go=new GameObject();
        var hdl=go.AddComponent<ComShHandle>();
        hdl.target=tr;
        hdllst.Add(hdl);
        return hdl;
    }
    public static void DelHandle(Transform tr){
        var hdl=tr.gameObject.GetComponent<ComShHandle>();
        if(hdl!=null) DelHandle(hdl);
    }
    public static void DelHandle(ComShHandle hdl){
        GameObject.Destroy(hdl.gameObject);
        UTIL.RemoveFromList(hdllst,hdl);
    }
    public static int SetHandleType(ComShHandle hdl,string type){
        switch(type){
        case "lpos": hdl.SetHandleType(1,1,0,0); return 0;
        case "wpos": hdl.SetHandleType(0,1,0,0); return 0;
        case "opos": hdl.SetHandleType(2,1,0,0); return 0;
        case "lrot": hdl.SetHandleType(1,0,1,0); return 0;
        case "wrot": hdl.SetHandleType(0,0,1,0); return 0;
        case "orot": hdl.SetHandleType(2,0,1,0); return 0;
        case "scale": hdl.SetHandleType(1,0,0,1); return 0;
        case "lposrot": hdl.SetHandleType(1,1,1,0); return 0;
        case "wposrot": hdl.SetHandleType(0,1,1,0); return 0;
        case "oposrot": hdl.SetHandleType(2,1,1,0); return 0;
        default: return -1;
        }
    }
}
}
