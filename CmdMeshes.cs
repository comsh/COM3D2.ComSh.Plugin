using System.Collections.Generic;
using UnityEngine;
using static COM3D2.ComSh.Plugin.Command;

namespace COM3D2.ComSh.Plugin {
public static class CmdMeshes {
    public static void Init(){
        Command.AddCmd("mesh",new Cmd(CmdMesh));
        meshParamDic.Add("verloop",new CmdParam<SingleMesh>(MeshParamVerLoop));
        meshParamDic.Add("filter",new CmdParam<SingleMesh>(MeshParamFilter));
        meshParamDic.Add("exclude",new CmdParam<SingleMesh>(MeshParamExclude));
        meshParamDic.Add("topology",new CmdParam<SingleMesh>(MeshParamTopology));
        meshParamDic.Add("shader",new CmdParam<SingleMesh>(MeshParamShader));
        meshParamDic.Add("blend",new CmdParam<SingleMesh>(MeshParamBlend));
        meshParamDic.Add("prop",new CmdParam<SingleMesh>(MeshParamProp));
        meshParamDic.Add("bound",new CmdParam<SingleMesh>(MeshParamProp));
        meshParamDic.Add("reset",new CmdParam<SingleMesh>(MeshParamReset));
    }
    private static Dictionary<string,CmdParam<SingleMesh>> meshParamDic=new Dictionary<string,CmdParam<SingleMesh>>();

    private static int CmdMesh(ComShInterpreter sh,List<string> args){
        if(args.Count==1) return 0;
        string[] lr=ParseUtil.LeftAndRight(args[1],'#');
        if(lr[1]==""){
            if(ObjUtil.GetPhotoPrefabTr(sh,true)==null) return sh.io.Error("オブジェクトが存在しません");
            Transform tr=ObjUtil.FindObj(sh,lr[0]);
            if(tr==null) return sh.io.Error("オブジェクトが存在しません");
            var mi=new MeshInfo(tr);
            for(int i=0; i<mi.count; i++){
                sh.io.Print($"{i}{sh.ofs}count={mi.oid.originalMesh.GetIndices(i).Length/3}");
                if(mi.material.Count>i) sh.io.Print($"{sh.ofs}mate={mi.material[i].name}{sh.ofs}shader={mi.material[i].shader.name}");
                sh.io.PrintLn("");
            }
            return 0;
        }
        if(!float.TryParse(lr[1],out float no) || no<0) return sh.io.Error("メッシュ番号が不正です");
        string[] sa=lr[0].Split(ParseUtil.colon);
        return CmdMeshSub(sh,sa,(int)no,args,2);
    }
    public static int CmdMeshSub(ComShInterpreter sh,string[] sa,int meshno,List<string> args,int startpos){
        if(ObjUtil.GetPhotoPrefabTr(sh,true)==null) return sh.io.Error("オブジェクトが存在しません");
        Transform tr=ObjUtil.FindObj(sh,sa);
        if(tr==null) return sh.io.Error("オブジェクトが存在しません");
        var mi=new MeshInfo(tr);
        if(mi.count<=meshno) return sh.io.Error("指定されたメッシュが存在しません");
        if(args.Count==startpos){
            sh.io.Print($"count:{mi.oid.originalMesh.GetIndices(meshno).Length/3}");
            if(mi.material.Count>meshno) sh.io.Print($"\nmaterial:{mi.material[meshno].name}\nshader:{mi.material[meshno].shader.name}");
            return 0;
        }
        var mesh=new SingleMesh(meshno,mi);
        int ret=ParamLoop(sh,mesh,meshParamDic,args,startpos);
        return ret;
    }

    private static int MeshParamFilter(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null || val=="") return 0;
        float[] sz=new float[6];
        int n=ParseUtil.XyzSub(val,sz);
        if(n!=6) return sh.io.Error("フィルタの形式が不正です");
        if(sz[3]<=0||sz[4]<=0||sz[5]<=0) return sh.io.Error("幅/高さ/奥行の指定が不正です");
        sm.filter=new float[]{  // x,y,z,w,h,d -> x0,x1,y0,y1,z0,z1
            sz[0]-sz[3]/2,sz[0]+sz[3]/2,
            sz[1]-sz[4]/2,sz[1]+sz[4]/2,
            sz[2]-sz[5]/2,sz[2]+sz[5]/2
        };
        return 1;
    }
    private static int MeshParamExclude(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null || val=="") return 0;
        float[] sz=new float[6];
        int n=ParseUtil.XyzSub(val,sz);
        if(n!=6) return sh.io.Error("フィルタの形式が不正です");
        if(sz[3]<=0||sz[4]<=0||sz[5]<=0) return sh.io.Error("幅/高さ/奥行の指定が不正です");
        sm.exclude=new float[]{  // x,y,z,w,h,d -> x0,x1,y0,y1,z0,z1
            sz[0]-sz[3]/2,sz[0]+sz[3]/2,
            sz[1]-sz[4]/2,sz[1]+sz[4]/2,
            sz[2]-sz[5]/2,sz[2]+sz[5]/2
        };
        return 1;
    }
    private static int MeshParamVerLoop(ComShInterpreter sh,SingleMesh sm,string val){
        var psr=new ComShParser(sh.lastParser.lineno);
        int ret=psr.Parse(val);
        if(ret<0) return sh.io.Error(psr.error);
        if(ret==0) return sh.io.Error("コマンドが空です");

        // 現シェルでの実行だが、出力だけはサブシェル実行と同じ形にする
        ComShInterpreter.Output orig=sh.io.output;
        var subout=new ComShInterpreter.SubShOutput();
        sh.io.output=new ComShInterpreter.Output(subout.Output);
        ret=0;

        int[] tri=sm.mi.mesh.GetIndices(sm.submeshno);
        if(tri==null) return sh.io.Error("メッシュ番号が不正です");

        var hs=new HashSet<uint>();
        for(int i=0; i<tri.Length; i++) hs.Add((uint)tri[i]);
        uint[] idx=new uint[hs.Count];
        hs.CopyTo(idx,0);

        Mesh m=sm.mi.mesh.FindMesh(sm.submeshno);
        if(m==null) return sh.io.Error("メッシュ番号が不正です");

        var vta=m.vertices;
        var nma=m.normals;
        var uva=m.uv;
        
        var changes=new List<VerLoopChange>();
        byte changed=0;
        for(int i=0; i<idx.Length; i++){
            int th=(int)idx[i];
            var vt=vta[th];

            if(sm.filter!=null){
                if( sm.filter[0]>vt.x || sm.filter[1]<vt.x 
                 || sm.filter[2]>vt.y || sm.filter[3]<vt.y
                 || sm.filter[4]>vt.z || sm.filter[5]<vt.z ) continue;
            }
            if(sm.exclude!=null){
                if( sm.exclude[0]<=vt.x && sm.exclude[1]>=vt.x 
                 && sm.exclude[2]<=vt.y && sm.exclude[3]>=vt.y
                 && sm.exclude[4]<=vt.z && sm.exclude[5]>=vt.z) continue;
            }

            sh.env["_1"]=i.ToString();
            sh.env["_2"]=th.ToString();

            string svt="",snm="",suv="";
            svt=sh.fmt.FPos(vt);
            if(th<nma.Length) snm=sh.fmt.FPos(nma[th]);// この辺は常にverticesと同数だと思うけど一応チェック
            if(th<uva.Length) suv=sh.fmt.FXY(uva[th]);

            sh.env["_vertex"]=svt;
            sh.env["_normal"]=snm;
            sh.env["_uv"]=suv;

            psr.Reset();
            ret=sh.InterpretParser(psr);
            if(ret<0 || sh.exitq){ ret=sh.io.exitStatus; sh.exitq=false; break; }

            VerLoopChange c=null;
            string t;
            if((t=sh.env["_vertex"])!=svt){
                if(c==null) c=new VerLoopChange(th);
                c.vertex=t;
                changed|=1;
            }
            if(snm!="" && (t=sh.env["_normal"])!=snm){
                if(c==null) c=new VerLoopChange(th);
                c.normal=t;
                changed|=2;
            }
            if(suv!="" && (t=sh.env["_uv"])!=suv){
                if(c==null) c=new VerLoopChange(th);
                c.uv=t;
                changed|=4;
            }
            if(c!=null) changes.Add(c);
        }

        if(changed!=0){
            sm.mi.EditMesh();
            m=sm.mi.mesh.FindMesh(sm.submeshno);
        }

        foreach(var c in changes){
            if(c.vertex!=null){
                float[] xyz=ParseUtil.Xyz(c.vertex);
                if(xyz==null){ sh.io.Error($"頂点{c.idx} 座標の形式が不正です"); break; }
                vta[c.idx]=new Vector3(xyz[0],xyz[1],xyz[2]);
            }
            if(c.normal!=null){
                float[] xyz=ParseUtil.Xyz(c.normal);
                if(xyz==null){ sh.io.Error($"頂点{c.idx}: 法線ベクトルの形式が不正です"); break;}
                nma[c.idx]=new Vector3(xyz[0],xyz[1],xyz[2]);
            }
            if(c.uv!=null){
                float[] xy=ParseUtil.Xy(c.uv);
                if(xy==null){ sh.io.Error($"頂点{c.idx}: UV座標の形式が不正です"); break;}
                uva[c.idx]=new Vector2(xy[0],xy[1]);
            }
        }
        if((changed&1)!=0) m.vertices=vta;
        if((changed&2)!=0) m.normals=nma;
        if((changed&4)!=0) m.uv=uva;

        sh.io.output=orig;
        sh.io.Print(subout.GetSubShResult());
        return ret;
    }
    private static int MeshParamShader(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null){
            sh.io.Print(sm.mi.material[sm.submeshno].shader.name);
            return 0;
        }
        Shader shader=Shader.Find(val);
        if(shader==null) return sh.io.Error("指定されたシェーダは見つかりません");
        sm.mi.material[sm.submeshno].shader=shader;
        return 1;
    }
    private static int MeshParamProp(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null) return 0;
        string[] kv=ParseUtil.LeftAndRight(val,'=');
        string err;
        if(kv[1]=="on"){
            sm.mi.material[sm.submeshno].EnableKeyword(kv[0]);
        }else if(kv[1]=="off"){
            sm.mi.material[sm.submeshno].DisableKeyword(kv[0]);
        }else if(kv[1].IndexOf(',')>=0){
            if((err=SetColorProp(sm.mi.material[sm.submeshno],kv[0],kv[1]))!="") return sh.io.Error(err);
        }else{
            if((err=SetFloatProp(sm.mi.material[sm.submeshno],kv[0],kv[1]))!="") return sh.io.Error(err);
        }
        return 1;
    }
    public static string SetColorProp(Material m, string key, string val){
        if(!m.HasProperty(key)) return "指定されたプロパティは現在のシェーダでは無効です";
        float[] fa=ParseUtil.RgbaLenient(val);
        if(fa==null) return ParseUtil.error;
        m.SetColor(key,new Color(fa[0],fa[1],fa[2],fa[3]));
        return "";
    }
    public static string SetFloatProp(Material m, string key, string val){
        if(!m.HasProperty(key)) return "指定されたプロパティは現在のシェーダでは無効です";
        float f=ParseUtil.ParseFloat(val);
        if(float.IsNaN(f)) return ParseUtil.error;
        m.SetFloat(key,f);
        return "";
    }
    private static int MeshParamBlend(ComShInterpreter sh,SingleMesh sm,string val){
        if(sm.mi.material[sm.submeshno].shader.name!="Standard"){
            return sh.io.Error("blendはStandardシェーダでのみ有効です");
        }
        if(val==null){
            string mode="不明";
            if(sm.mi.material[sm.submeshno].renderQueue==-1){
                mode="opaque";
            }else if(sm.mi.material[sm.submeshno].renderQueue==(int)UnityEngine.Rendering.RenderQueue.AlphaTest){
                mode="cutout";
            }else if(sm.mi.material[sm.submeshno].renderQueue==(int)UnityEngine.Rendering.RenderQueue.Transparent){
                if(sm.mi.material[sm.submeshno].GetInt("_SrcBlend")==(int)UnityEngine.Rendering.BlendMode.SrcAlpha){
                    mode="fade";
                }else{
                    mode="transparent";
                }
            }
            sh.io.Print(mode);
            return 0;
        }
        if(ChgBlendMode(sm.mi.material[sm.submeshno],val)<0)
            return sh.io.Error("blendにはopaque|cutout|fade|transparentのいずれかを指定して下さい");
        return 1;
    }
    public static int ChgBlendMode(Material material,string mode){
        switch (mode) {
        case "opaque":
            material.SetOverrideTag("RenderType", "");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
            break;
        case "cutout":
            material.SetOverrideTag("RenderType", "TransparentCutout");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.EnableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            break;
        case "fade":
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            break;
        case "transparent":
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            break;
        default:
            return -1;
        }
        return 0;
    }
    private static int MeshParamTopology(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null){
            if(sm.mi.mesh.GetIndices(sm.submeshno).Length==0)
                sh.io.Print("None");
            else
                sh.io.Print(sm.mi.mesh.GetTopology(sm.submeshno).ToString());
            return 0;
        }
        return MeshParamTopologySub(sh,sm.mi,sm.submeshno,val);
    }
    public static int MeshParamTopologySub(ComShInterpreter sh,MeshInfo mi,int n,string val){
        if(val=="0"){
            mi.EditMesh();
            mi.mesh.SetIndices(new int[0],MeshTopology.Points,n);
        }else if(val=="1"){
            mi.EditMesh();
            if(mi.mesh.GetTopology(n)==MeshTopology.Points) return 1;
            int[] ia=mi.oid.originalMesh.GetIndices(n);
            var hs=new HashSet<int>();
            for(int i=0; i<ia.Length; i++) hs.Add(ia[i]);
            int[] ia2=new int[hs.Count];
            hs.CopyTo(ia2);
            mi.mesh.SetIndices(ia2,MeshTopology.Points,n);
        }else if(val=="2"){
            mi.EditMesh();
            if(mi.mesh.GetTopology(n)==MeshTopology.Lines) return 1;

            MeshTopology mt=mi.oid.originalMesh.GetTopology(n);
            if(mt!=MeshTopology.Triangles) return sh.io.Error($"未対応の形式です(topology={mt.ToString()})");
            int[] ia=mi.oid.originalMesh.GetIndices(n);
            var hs=new HashSet<uint>();
            for(int i=0; i<ia.Length; i+=3){
                hs.Add((ia[i]<ia[i+1])?(uint)(ia[i]*65536+ia[i+1]):(uint)(ia[i+1]*65536+ia[i]));
                hs.Add((ia[i+1]<ia[i+2])?(uint)(ia[i+1]*65536+ia[i+2]):(uint)(ia[i+2]*65536+ia[i+1]));
                hs.Add((ia[i+2]<ia[i])?(uint)(ia[i+2]*65536+ia[i]):(uint)(ia[i]*65536+ia[i+2]));
            }
            int[] ia2=new int[hs.Count*2];
            int cnt=0;
            foreach(var u in hs){ ia2[cnt++]=(int)(u>>16); ia2[cnt++]=(int)(u&0xffff); }
            mi.mesh.SetIndices(ia2,MeshTopology.Lines,n);
        }else if(val=="3"){
            if(mi.mesh.GetTopology(n)==MeshTopology.Triangles) return 1;
            mi.mesh.SetIndices(mi.oid.originalMesh.GetIndices(n),MeshTopology.Triangles,n);
        }
        return 0;
    }
    private static int MeshParamReset(ComShInterpreter sh,SingleMesh sm,string val){
        sm.mi.RestoreMesh(sm.submeshno);
        return 0;
    }

    private class VerLoopChange {
        public int idx;
        public string vertex;
        public string normal;
        public string uv;
        public VerLoopChange(int idx){ this.idx=idx; }
    }
    private class SingleMesh {
        public int submeshno;
        public MeshInfo mi;
        public SingleMesh(int no,MeshInfo mi){ this.submeshno=no; this.mi=mi; }
        public float[] filter=null; // 意味的にはメンバじゃないけど
        public float[] exclude=null;
    }
    public class MeshInfo {
        public int count=0;
        public ObjInfo oi=null;
        public ObjInfoData oid=null;
        public List<Material> material;
        public Transform transform;
        public ObjInfoData.MeshList mesh;
        public MeshInfo(Transform tr){
            transform=tr;
            oi=ObjInfo.GetObjInfo(tr);
            oid=(oi==null)?new ObjInfoData(tr):oi.data;

            if(oid.workMesh==null){
                oid.BackupMesh();
                mesh=oid.originalMesh;
            }else{
                mesh=oid.workMesh;
            }
            count=(mesh.Count==0)?0:mesh[mesh.Count-1].no+mesh[mesh.Count-1].submeshcount;

            material=new List<Material>();
            foreach(var b in oid.bones){
                Renderer r=b.GetComponent<Renderer>();
                if(r==null) continue;
                if(ReferenceEquals(r.GetType(),typeof(SkinnedMeshRenderer))){
                    var smr=(SkinnedMeshRenderer)r;
                    int n=smr.sharedMesh.subMeshCount;
                    Material[] mate=smr.sharedMaterials;
                    n=(n>mate.Length)?mate.Length:n;    // 1サブメッシュ1マテリアルのみ対応
                    for(int j=0; j<n; j++) material.Add(mate[j]);
                }else{
                    MeshFilter mf=r.transform.GetComponent<MeshFilter>();
                    if(mf==null) continue;
                    int n=mf.sharedMesh.subMeshCount;
                    Material[] mate=r.sharedMaterials;
                    n=(n>mate.Length)?mate.Length:n;    // 1サブメッシュ1マテリアルのみ対応
                    for(int j=0; j<n; j++) material.Add(mate[j]);
                }
            }
        }
        public void EditMesh(){
            if(oi==null){
                // 編集すると後始末が必要になるので、ComShで追加したオブジェト以外にもObjInfoを付ける
                oi=ObjInfo.AddObjInfo(transform,oid,"",null);
            }
            oid.CloneMesh();
            mesh=oid.workMesh;
        }
        public int RestoreMesh(int submeshno){
            if(oid.originalMesh==null) return -1;
            int midx=oid.originalMesh.FindMeshIdx(submeshno);
            Renderer[] ra=oid.FindComponentsToArray<Renderer>();
            if(ra==null || ra.Length<midx) return -1;
            Mesh newMesh;
            if (ReferenceEquals(ra[midx].GetType(), typeof(SkinnedMeshRenderer))) {
                var smr=(SkinnedMeshRenderer)ra[midx];
                newMesh=Object.Instantiate(oid.originalMesh[midx].mesh);
                smr.sharedMesh=newMesh;
            }else{
                MeshFilter mf=ra[midx].transform.GetComponent<MeshFilter>();
                if(mf==null) return -1;
                newMesh=Object.Instantiate(oid.originalMesh[midx].mesh);
                mf.sharedMesh=newMesh;
            }
            var e=oid.workMesh[midx];
            Object.Destroy(e.mesh);
            e.mesh=newMesh;
            oid.workMesh[midx]=e;
            return 0;
        }
    }
}
}
