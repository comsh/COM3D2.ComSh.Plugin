using System.Collections.Generic;
using UnityEngine;

namespace COM3D2.ComSh.Plugin {

public class ObjInfo : MonoBehaviour{
    public string source="";    // obj add時のプレハブ名
    public ObjInfoData data;
    public TMorph morph;        // シェイプキー用
    public void InitBones(){ data=new ObjInfoData(transform); }
    public void OnDestroy(){
        ObjUtil.objDic.Remove(name);
        foreach(var m in this.data.workMesh) UnityEngine.Object.Destroy(m.mesh);
    }

    public static ObjInfo AddObjInfo(Transform tr,string src,TMorph morph=null){
        var oi=tr.gameObject.AddComponent<ObjInfo>();
        oi.source=src;
        oi.InitBones();
        oi.morph=morph;
        return oi;
    }
    public static ObjInfo AddObjInfo(Transform tr,ObjInfoData oid,string src,TMorph morph=null){
        var oi=tr.gameObject.AddComponent<ObjInfo>();
        oi.source=src;
        oi.morph=morph;
        oi.data=oid;
        return oi;
    }
    public static ObjInfo AddObjInfo(GameObject go,string src,TMorph morph=null){ return AddObjInfo(go.transform,src,morph); }
    public static ObjInfo UpdObjInfo(Transform tr){
        var oi=GetObjInfo(tr);
        if(oi!=null) oi.data.UpdateBones(tr);
        return oi;
    }
    public static ObjInfo GetObjInfo(Transform tr){
        var oi=tr.GetComponentInParent<ObjInfo>();          // 親方向で最初に見つかるもの
        if(oi!=null && oi.data.FindBone(tr)==null) return null;  // 子に自分が含まれてなければ無関係
        return oi;
    }
}

public class ObjInfoData {
    // 自分のボーンを認識するためのもの。アタッチで子ボーンが増えるのに備えて
    public List<Transform> bones=new List<Transform>();
    public ObjInfoData(Transform transform){ UpdateBones(transform); }
    public void UpdateBones(Transform transform){
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
    public T[] FindComponentsToArray<T>(){ return FindComponents<T>().ToArray(); }


    public MeshList originalMesh;
    public MeshList workMesh;

    public void BackupMesh(){
        if(originalMesh!=null) return;
        originalMesh = new MeshList();
        int meshno = 0;
        foreach(var b in bones){
            var r=b.GetComponent<Renderer>();
            if(r==null) continue;
            if(ReferenceEquals(r.GetType(), typeof(SkinnedMeshRenderer))) {
                var smr = (SkinnedMeshRenderer)r;
                originalMesh.Add(new MeshList.Entry { no=meshno,submeshcount=smr.sharedMesh.subMeshCount,mesh=smr.sharedMesh });
                meshno+=smr.sharedMesh.subMeshCount;
            }else{
                MeshFilter mf=r.transform.GetComponent<MeshFilter>();
                if(mf==null) continue;
                originalMesh.Add(new MeshList.Entry { no=meshno,submeshcount=mf.sharedMesh.subMeshCount,mesh=mf.sharedMesh });
                meshno+=mf.sharedMesh.subMeshCount;
            }
        }
        originalMesh.submeshCount=meshno;
        return;
    }

    public void CloneMesh(){
        if(workMesh!=null) return;
        workMesh=new MeshList();
        int meshno = 0;
        foreach(var b in bones){
            var r=b.GetComponent<Renderer>();
            if(r==null) continue;
            if (ReferenceEquals(r.GetType(), typeof(SkinnedMeshRenderer))) {
                var smr = (SkinnedMeshRenderer)r;
                Mesh newmesh=Object.Instantiate(smr.sharedMesh);
                smr.sharedMesh=newmesh;
                workMesh.Add(new MeshList.Entry { no=meshno,submeshcount=smr.sharedMesh.subMeshCount,mesh=newmesh });
                meshno +=smr.sharedMesh.subMeshCount;
            }else{
                MeshFilter mf=r.transform.GetComponent<MeshFilter>();
                if(mf==null) continue;
                workMesh.Add(new MeshList.Entry { no=meshno,submeshcount=mf.sharedMesh.subMeshCount,mesh=mf.mesh });
                meshno += mf.sharedMesh.subMeshCount;
            }
        }
        workMesh.submeshCount=meshno;
        return;
    }



    // メッシュのリスト。管理用につきメッシュ単位
    public class MeshList : List<MeshList.Entry> {
        public struct Entry{
            public int no;              // オブジェクト全体での通し番号。このメッシュの0番サブメッシュがオブジェクト全体で何番目か
            public int submeshcount;    // このメッシュ内のサブメッシュ数
            public Mesh mesh;           // メッシュ本体。triangleで複数のサブメッシュに分かれている
        }
        public int submeshCount=0;
        private static int[] triangle_empty=new int[0];
        public int FindMeshIdx(int meshno){ // オブジェクト全体での通し番号で単一のサブメッシュを探す
            int i;
            for(i=0; i<this.Count; i++) if(meshno<this[i].no) break;
            //if(i==0) return -1;
            return i-1; // 返すのはリストの添え字でしかない
        }
        public Mesh FindMesh(int submeshno){
            int n=FindMeshIdx(submeshno);
            if(n<0) return null;
            return this[n].mesh;
        }
        public void SetIndices(int[] ia,MeshTopology mt,int no){
            int n=FindMeshIdx(no);
            if(n<0) return;
            this[n].mesh.SetIndices(ia,mt,no-this[n].no);
        }
        public int[] GetIndices(int no){
            int n=FindMeshIdx(no);
            if(n<0) return triangle_empty;
            return this[n].mesh.GetIndices(no-this[n].no);
        }
        public MeshTopology GetTopology(int no){
            int n=FindMeshIdx(no);
            if(n<0) return MeshTopology.Triangles;
            return this[n].mesh.GetTopology(no-this[n].no);
        }
    }
}
}
