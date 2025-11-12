using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using static System.StringComparison;

namespace COM3D2.ComSh.Plugin {

public class ObjInfo : MonoBehaviour{
    public string source="";    // obj add時のプレハブ名
    public ObjInfoData data;
    public void InitBones(){ data=new ObjInfoData(transform); }
    public void OnDestroy(){
        if(ObjUtil.objDic.TryGetValue(name,out Transform deltr)
            && System.Object.ReferenceEquals(deltr,transform)){
            ObjUtil.objDic.Remove(name);
        }
        if(this.data!=null){
            if(this.data.originalMesh!=null) foreach(var m in this.data.originalMesh)
                if(m.own) UnityEngine.Object.Destroy(m.mesh);
            if(this.data.workMesh!=null) foreach(var m in this.data.workMesh) UnityEngine.Object.Destroy(m.mesh);
            if (this.data.originalMate != null) for (int i = 0; i < this.data.originalMate.Count; i++) UnityEngine.Object.Destroy(this.data.originalMate[i]);
            if (this.data.workMate!=null) foreach(var mate in this.data.workMate) UnityEngine.Object.Destroy(mate);
            if(this.data.trash!=null) for(int i=0; i<this.data.trash.Length; i++) UnityEngine.Object.Destroy(this.data.trash[i]);
        }
    }
    public static ObjInfo AddObjInfo(Transform tr,string src,List<TMorph> morph=null){
        var oi=tr.gameObject.AddComponent<ObjInfo>();
        oi.source=src;
        oi.InitBones();
        oi.data.morph=morph;
        return oi;
    }
    public static ObjInfo AddObjInfo(Transform tr,ObjInfoData oid,string src,List<TMorph> morph=null){
        var oi=tr.gameObject.AddComponent<ObjInfo>();
        oi.source=src;
        oi.data=oid;
        oi.data.morph=morph;
        return oi;
    }
    public static ObjInfo AddObjInfo(GameObject go,string src,List<TMorph> morph=null){ return AddObjInfo(go.transform,src,morph); }
    public static ObjInfo UpdObjInfo(Transform tr){
        var oi=GetObjInfo(tr);
        if(oi!=null) oi.data.UpdateBones(tr);
        return oi;
    }
    public static ObjInfo GetObjInfo(Transform tr){
        Transform t=tr;
        ObjInfo oi=null;
        while(t!=null){
            oi=t.GetComponentInParent<ObjInfo>();          // 親方向で最初に見つかるもの
            if(oi==null) return null;
            if(oi.enabled==false){ t=oi.transform.parent; continue; }
            break;
        }
        if(oi!=null && oi.data!=null && oi.data.FindBone(tr)==null) return null;  // 子に自分が含まれてなければ無関係
        return oi;
    }
}

public class ObjInfoData {
    // 自分のボーンを認識するためのもの。アタッチで子ボーンが増えるのに備えて
    public List<Transform> bones=new List<Transform>();
    public List<TMorph> morph;        // シェイプキー用
    public ObjInfoData(Transform transform){ UpdateBones(transform); }
    public void UpdateBones(){
        if(bones.Count>0) UpdateBones(bones[0]);
    }
    public void UpdateBones(Transform transform){
        bones.Clear();
        bones.Capacity=64;
        if(transform.parent!=null && transform.parent.name=="Offset"){ // メイド
            UTIL.TraverseTr(transform,(Transform tr,int i)=>{
                ObjInfo oi;
                if(tr.name.StartsWith("_SM_",Ordinal)) return 1;
                if(tr.GetComponent<AttachPrefab>()!=null) return 1;
                if(i>0 && (oi=tr.GetComponent<ObjInfo>())!=null && oi.enabled) return 1;
                bones.Add(tr);
                return 0;
            });
        }else{
            UTIL.TraverseTr(transform,(Transform tr,int i)=>{
                bones.Add(tr);
                return 0;
            });
        }
    }
    public Transform FindBone(Transform tr){
        if(bones.Count==0) return null;
        bool haveNull=false;
        for(int i=0; i<bones.Count; i++){
            if(bones[i]==null){haveNull=true; break;}
            if(ReferenceEquals(bones[i],tr)) return bones[i];
        }
        if(haveNull){
            UpdateBones(bones[0]);
            for(int i=0; i<bones.Count; i++)
                if(ReferenceEquals(bones[i],tr)) return bones[i];
        }
        return null;
    }
    public Transform FindBone(string name){ // 同じ名前のボーンが複数あるかもだけど
        if(bones.Count==0) return null;
        bool haveNull=false;
        for(int i=0; i<bones.Count; i++){
            if(bones[i]==null){haveNull=true; break;}
            if(bones[i].name==name) return bones[i];
        }
        if(haveNull){
            UpdateBones(bones[0]);
            for(int i=0; i<bones.Count; i++)
                if(bones[i].name==name) return bones[i];
        }
        return null;
    }
    public T FindComponent<T>(){
        if(bones.Count==0) return default;
        T ret;
        bool haveNull=false;
        for(int i=0; i<bones.Count; i++){
            if(bones[i]==null){haveNull=true; break;}
            if((ret=bones[i].GetComponent<T>())!=null) return ret;
        }
        if(haveNull){
            UpdateBones(bones[0]);
            for(int i=0; i<bones.Count; i++)
                if((ret=bones[i].GetComponent<T>())!=null) return ret;
        }
        return default;
    }
    public List<T> FindComponents<T>(){
        List<T> ret=new List<T>();
        if(bones.Count==0) return ret;
        T c;
        bool haveNull=false;
        for(int i=0; i<bones.Count; i++) if(bones[i]==null){haveNull=true; break;}
        if(haveNull) UpdateBones(bones[0]);
        for(int i=0; i<bones.Count; i++)
            if((c=bones[i].GetComponent<T>())!=null) ret.Add(c);
        return ret;
    }
    public T[] FindComponentsToArray<T>(){ return FindComponents<T>().ToArray(); }

    public MeshList originalMesh;
    public MeshList workMesh;
    public List<Material> originalMate;
    public List<Material> workMate;
    public UnityEngine.Object[] trash;

    private static Material emptymate=new Material(Shader.Find("Standard"));
    public void Backup(){
        if(originalMesh!=null) return;
        originalMesh=new MeshList();
        originalMate=new List<Material>();
        int meshno = 0;
        foreach(var b in bones){
            if(!b.gameObject.activeInHierarchy) continue;
            var r=b.GetComponent<Renderer>();
            if(r==null) continue;

            if(ReferenceEquals(r.GetType(),typeof(SkinnedMeshRenderer))) {
                var smr = (SkinnedMeshRenderer)r;
                if(smr.sharedMesh==null) continue;

                originalMesh.Add(new MeshList.Entry { no=meshno,submeshcount=smr.sharedMesh.subMeshCount,mesh=smr.sharedMesh,rend=r,own=false });
                int n=smr.sharedMesh.subMeshCount;
                meshno+=n;

                Material[] mate=smr.sharedMaterials;
                int j;
                for(j=0; j<mate.Length; j++) originalMate.Add(new Material(mate[j]));
                for(; j<n; j++) originalMate.Add(emptymate);
            }else if(ReferenceEquals(r.GetType(),typeof(MeshRenderer))) {
                MeshFilter mf=r.transform.GetComponent<MeshFilter>();
                if(mf==null) continue;
                if(mf.sharedMesh==null) continue;

                originalMesh.Add(new MeshList.Entry { no=meshno,submeshcount=mf.sharedMesh.subMeshCount,mesh=mf.sharedMesh,rend=r,own=false });
                int n=mf.sharedMesh.subMeshCount;
                meshno+=n;

                Material[] mate=r.sharedMaterials;
                int j;
                for(j=0; j<mate.Length; j++) originalMate.Add(new Material(mate[j]));
                for(; j<n; j++) originalMate.Add(emptymate);
            }
        }
        originalMesh.submeshCount=meshno;
        return;
    }

    public void CloneMesh(){
        if(workMesh!=null) return;
        workMesh=new MeshList(originalMesh!=null?originalMesh.Count:8);
        int meshno = 0;
        foreach(var b in bones){
            if(!b.gameObject.activeInHierarchy) continue;
            var r=b.GetComponent<Renderer>();
            if(r==null) continue;
            if (ReferenceEquals(r.GetType(), typeof(SkinnedMeshRenderer))){
                var smr = (SkinnedMeshRenderer)r;
                Mesh oldmesh=smr.sharedMesh;
                Mesh newmesh=UnityEngine.Object.Instantiate(oldmesh);
                smr.sharedMesh=newmesh;
                workMesh.Add(new MeshList.Entry { no=meshno,submeshcount=smr.sharedMesh.subMeshCount,mesh=newmesh,rend=r,own=true });
                meshno +=smr.sharedMesh.subMeshCount;
                UpdateMorph(r.transform,oldmesh,newmesh);
            }else if(ReferenceEquals(r.GetType(), typeof(MeshRenderer))){
                MeshFilter mf=r.transform.GetComponent<MeshFilter>();
                if(mf==null) continue;
                Mesh oldmesh=mf.sharedMesh;
                Mesh newmesh=UnityEngine.Object.Instantiate(oldmesh);
                mf.sharedMesh=newmesh;
                workMesh.Add(new MeshList.Entry { no=meshno,submeshcount=mf.sharedMesh.subMeshCount,mesh=newmesh,rend=r,own=true });
                meshno += mf.sharedMesh.subMeshCount;
            }
        }
        workMesh.submeshCount=meshno;
        workMesh.BackupIndices3();
        return;
    }
    public void OwnMesh(){
        for(int i=0; i<originalMesh.Count; i++){
            var ment=originalMesh[i];
            ment.own=true;
            originalMesh[i]=ment;
        }
    }
    public void OwnMesh(int submeshno){
        int n=originalMesh.FindMeshIdx(submeshno);
        var ment=originalMesh[n];
        ment.own=true;
        originalMesh[n]=ment;
    }
    public void CommitMesh(int submeshno){
        if(workMesh==null) return;
        int n=workMesh.FindMeshIdx(submeshno);
        if(n<0) return;
        var old=originalMesh[n];
        if(!object.ReferenceEquals(old.mesh,workMesh[n].mesh)){
            UpdateMorph(old.rend.transform,old.mesh,workMesh[n].mesh);
            if(old.own) UnityEngine.Object.Destroy(old.mesh);
        }
        var me=new MeshList.Entry(workMesh[n]);
        me.mesh=UnityEngine.Object.Instantiate(me.mesh);
        me.mesh.SetIndices(workMesh.indices3[submeshno],MeshTopology.Triangles,submeshno-me.no);
        originalMesh[n]=me;
    }
    private static FieldInfo meshField=null;
    public void UpdateMorph(Transform tr,Mesh oldmesh,Mesh newmesh){
        if(meshField==null) try{
            meshField=typeof(TMorph).GetField("m_mesh",BindingFlags.Instance | BindingFlags.Public);
            // ↑今はpublicだけど↓以前はprivateだったので一応残す
            if(meshField==null) meshField=typeof(TMorph).GetField("m_mesh",BindingFlags.Instance | BindingFlags.NonPublic);
        }catch{ return; }
        if(this.morph!=null){
            foreach(TMorph m in this.morph){
                var mmesh=meshField.GetValue(m);
                if(object.ReferenceEquals(mmesh,oldmesh)) meshField.SetValue(m,newmesh);
            }
        }
        var tb=tr.GetComponentInParent<TBody>();
        if(tb==null || tb.goSlot==null) return;
        var slot=tb.goSlot;
        for(int i=0; i<slot.Count; i++){
            if(slot[i].morph==null) continue;
            var mmesh=meshField.GetValue(slot[i].morph);
            if(object.ReferenceEquals(mmesh,oldmesh)) meshField.SetValue(slot[i].morph,newmesh);
        }
    }
    public void CloneMaterial(){
        if(workMate==null) workMate=new List<Material>(originalMate!=null?originalMate.Count:8);
        workMate.Clear();
        foreach(var b in bones){
            if(!b.gameObject.activeInHierarchy) continue;
            var r=b.GetComponent<Renderer>();
            if(r==null) continue;
            if (ReferenceEquals(r.GetType(), typeof(SkinnedMeshRenderer))) {
                var smr = (SkinnedMeshRenderer)r;
                if(smr==null) continue;
                int n=smr.sharedMesh.subMeshCount;
                Material[] mate=smr.materials;
                n=(n>mate.Length)?mate.Length:n;    // 1サブメッシュ1マテリアルのみ対応
                for(int j=0; j<n; j++) workMate.Add(mate[j]);
            }else if(ReferenceEquals(r.GetType(), typeof(MeshRenderer))) {
                MeshFilter mf=r.transform.GetComponent<MeshFilter>();
                if(mf==null) continue;
                int n=mf.sharedMesh.subMeshCount;
                Material[] mate=r.materials;
                n=(n>mate.Length)?mate.Length:n;    // 1サブメッシュ1マテリアルのみ対応
                for(int j=0; j<n; j++) workMate.Add(mate[j]);
            }
        }
        return;
    }
    public void CommitMaterial(int submeshno){
        if(workMate==null) return;
        UnityEngine.Object.Destroy(originalMate[submeshno]);
        originalMate[submeshno]=new Material(workMate[submeshno]);
    }

    // メッシュのリスト。管理用につきメッシュ単位
    public class MeshList : List<MeshList.Entry> {
        public struct Entry{
            public int no;              // オブジェクト全体での通し番号。このメッシュの0番サブメッシュがオブジェクト全体で何番目か
            public int submeshcount;    // このメッシュ内のサブメッシュ数
            public Mesh mesh;           // メッシュ本体。triangleで複数のサブメッシュに分かれている
            public Renderer rend;
            public bool own;
            public Entry(Entry e){no=e.no; submeshcount=e.submeshcount;mesh=e.mesh;rend=e.rend;own=e.own;}
        }
        public int submeshCount=0;
        public List<int[]> indices3;
        private static int[] triangle_empty=new int[0];
        public MeshList(){}
        public MeshList(int cap):base(cap){}
        public void BackupIndices3(){
            if(indices3==null) indices3=new List<int[]>(this.submeshCount);
            else indices3.Clear();
            foreach(Entry e in this) for(int i=0; i<e.mesh.subMeshCount; i++) indices3.Add(e.mesh.GetIndices(i));
        }
        public void UpdateIndices3(int submeshno,int[] arr){ indices3[submeshno]=arr; }
        public void UpdateIndices3(int submeshno){ indices3[submeshno]=this.GetIndices(submeshno); }
        public void RestoreIndices3(int submeshno){
            this.SetIndices(indices3[submeshno],MeshTopology.Triangles,submeshno);
        }
        public int FindMeshIdx(int meshno){ // オブジェクト全体での通し番号で単一のサブメッシュを探す
            int i;
            for(i=0; i<this.Count; i++) if(meshno<this[i].no) break;
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
            if(this[n].own && mt==MeshTopology.Triangles) UpdateIndices3(no,ia);
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
public class ParticleInfo : MonoBehaviour{
    public void OnDestroy(){
        var r=transform.GetComponent<Renderer>();
        if(r==null) return;
        var m=r.material;
        if(m==null) return;
        UnityEngine.Object.Destroy(m);
    }
}
}
