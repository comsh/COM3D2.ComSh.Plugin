using System.Collections.Generic;
using UnityEngine;

namespace COM3D2.ComSh.Plugin {
public class ObjInfo : MonoBehaviour{
    public string source="";   // obj add時のプレハブ名

    // 自分のボーンを認識するためのもの。アタッチで子ボーンが増えるのに備えて
    private List<Transform> bones=new List<Transform>();

    public void InitBones(){
        bones.Clear();
        UTIL.TraverseTr(transform,(Transform tr)=>{ bones.Add(tr); return 0; },true);
    }
    public Transform FindBone(Transform tr){
        for(int i=0; i<bones.Count; i++) if(ReferenceEquals(bones[i],tr)) return bones[i];
        return null;
    }
    public Transform FindBone(int iid){
        for(int i=0; i<bones.Count; i++) if(bones[i].GetInstanceID()==iid) return bones[i];
        return null;
    }
    public Transform FindBone(string name){
        for(int i=0; i<bones.Count; i++) if(bones[i].name==name) return bones[i];
        return null;
    }
    public T FindComponent<T>(){
        T ret;
        for(int i=0; i<bones.Count; i++) if((ret=bones[i].GetComponent<T>())!=null) return ret;
        return default;
    }
    public List<T> FindComponents<T>(){
        List<T> ret=new List<T>();
        T c;
        for(int i=0; i<bones.Count; i++) if((c=bones[i].GetComponent<T>())!=null) ret.Add(c);
        return ret;
    }
    public T[] FindComponentsArray<T>(){ return FindComponents<T>().ToArray(); }
    public List<Transform> GetBones(){ return bones; }
    public TMorph morph; // シェイプキー用
    public MeshList mesh;

    public void CopyMesh(MeshList m){
        this.mesh=new MeshList();
        for(int i=0; i<m.Count; i++) this.mesh.Add(new MeshList.Entry{no=m[i].no,count=m[i].count,mesh=UnityEngine.Object.Instantiate(m[i].mesh)});
    }
    public void OnDestroy(){
        ObjUtil.objDic.Remove(name);
    }
    public static ObjInfo AddObjInfo(Transform tr,string src,TMorph morph=null){
        var oi=tr.gameObject.AddComponent<ObjInfo>();
        oi.source=src;
        oi.InitBones();
        oi.morph=morph;
        return oi;
    }
    public static ObjInfo AddObjInfo(GameObject go,string src,TMorph morph=null){ return AddObjInfo(go.transform,src,morph); }
    public static ObjInfo UpdObjInfo(Transform tr){
        var oi=GetObjInfo(tr);
        if(oi!=null) oi.InitBones();
        return oi;
    }
    public static ObjInfo GetObjInfo(Transform tr){
        var oi=tr.GetComponentInParent<ObjInfo>();          // 親方向で最初に見つかるもの
        if(oi!=null && oi.FindBone(tr)==null) return null;  // 子に自分が含まれてなければ無関係
        return oi;
    }
    public class MeshList : List<MeshList.Entry> {
        public struct Entry{ public int no; public int count; public Mesh mesh; }
        private static int[] triangle_empty=new int[0];
        private int FindMesh(int meshno){
            int i;
            for(i=0; i<this.Count; i++) if(meshno<this[i].no) break;
            //if(i==0) return -1;
            return i-1;
        }
        public void SetIndices(int[] ia,MeshTopology mt,int no){
            int n=FindMesh(no);
            if(n<0) return;
            this[n].mesh.SetIndices(ia,mt,no-this[n].no);
        }
        public int[] GetIndices(int no){
            int n=FindMesh(no);
            if(n<0) return triangle_empty;
            return this[n].mesh.GetIndices(no-this[n].no);
        }
        public MeshTopology GetTopology(int no){
            int n=FindMesh(no);
            if(n<0) return MeshTopology.Triangles;
            return this[n].mesh.GetTopology(no-this[n].no);
        }
    }
}
}
