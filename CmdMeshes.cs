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
        meshParamDic.Add("verlist",new CmdParam<SingleMesh>(MeshParamVerList));
        meshParamDic.Add("topology",new CmdParam<SingleMesh>(MeshParamTopology));
        meshParamDic.Add("shader",new CmdParam<SingleMesh>(MeshParamShader));
        meshParamDic.Add("blend",new CmdParam<SingleMesh>(MeshParamBlend));
        meshParamDic.Add("prop",new CmdParam<SingleMesh>(MeshParamProp));
        meshParamDic.Add("reset",new CmdParam<SingleMesh>(MeshParamReset));
        meshParamDic.Add("texture",new CmdParam<SingleMesh>(MeshParamTexture));
        meshParamDic.Add("recalc",new CmdParam<SingleMesh>(MeshParamRecalc));
        meshParamDic.Add("rq",new CmdParam<SingleMesh>(MeshParamRQ));
        meshParamDic.Add("ss",new CmdParam<SingleMesh>(MeshParamSS));
        meshParamDic.Add("png",new CmdParam<SingleMesh>(MeshParamPNG));
    }
    private static Dictionary<string,CmdParam<SingleMesh>> meshParamDic=new Dictionary<string,CmdParam<SingleMesh>>();

    private static int CmdMesh(ComShInterpreter sh,List<string> args){
        if(args.Count==1) return 0;
        var cd=new ParseUtil.ColonDesc(args[1]);
        if(cd.meshno<0){
            if(args.Count!=2) return sh.io.Error("メッシュ番号が不正です");
            Transform tr=ObjUtil.FindObj(sh,cd);
            if(tr==null) return sh.io.Error("オブジェクトが存在しません");
            var mi=new MeshInfo(tr);
            for(int i=0; i<mi.count; i++){
                sh.io.Print($"{i}");
                if(mi.material.Count>i){
                    sh.io.Print($"{sh.ofs}mate={mi.material[i].name}{sh.ofs}shader={mi.material[i].shader.name}");
                }
                sh.io.PrintLn("");
            }
            return 0;
        }
        return CmdMeshSub(sh,cd,args,2);
    }
    public static int CmdMeshSub(ComShInterpreter sh,ParseUtil.ColonDesc cd,List<string> args,int startpos){
        Transform tr=ObjUtil.FindObj(sh,cd);
        if(tr==null) return sh.io.Error("オブジェクトが存在しません");
        var mi=new MeshInfo(tr);
        if(mi.count<=cd.meshno) return sh.io.Error("指定されたメッシュが存在しません");
        if(args.Count==startpos){
            sh.io.Print($"count:{mi.oid.originalMesh.GetIndices(cd.meshno).Length/3}");
            if(mi.material.Count>cd.meshno) sh.io.Print($"\nmaterial:{mi.material[cd.meshno].name}\nshader:{mi.material[cd.meshno].shader.name}");
            return 0;
        }
        var mesh=new SingleMesh(cd.meshno,mi);
        int ret=ParamLoop(sh,mesh,meshParamDic,args,startpos);
        return ret;
    }

    private static int MeshParamFilter(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null || val=="") return 0;
        float[] sz=new float[6];
        int n=ParseUtil.XyzSub(val,sz);
        if(n!=4 && n!=6) return sh.io.Error("フィルタの形式が不正です");
        if(n==4 && sz[3]<=0) return sh.io.Error("半径の指定が不正です");
        else if(n==6 && (sz[3]<=0||sz[4]<=0||sz[5]<=0)) return sh.io.Error("幅/高さ/奥行の指定が不正です");
        if(n==6){
            sm.filter=new float[]{  // x,y,z,w,h,d -> x0,x1,y0,y1,z0,z1
                sz[0]-sz[3]/2,sz[0]+sz[3]/2,
                sz[1]-sz[4]/2,sz[1]+sz[4]/2,
                sz[2]-sz[5]/2,sz[2]+sz[5]/2
            };
        }else{
            sm.filter=new float[]{ sz[0],sz[1],sz[2],sz[3]*sz[3] };   // x,y,z,r -> x,y,z,r^2
        }
        return 1;
    }
    private static int MeshParamExclude(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null || val=="") return 0;
        float[] sz=new float[6];
        int n=ParseUtil.XyzSub(val,sz);
        if(n!=4 && n!=6) return sh.io.Error("フィルタの形式が不正です");
        if(n==4 && sz[3]<=0) return sh.io.Error("半径の指定が不正です");
        else if(n==6 && (sz[3]<=0||sz[4]<=0||sz[5]<=0)) return sh.io.Error("幅/高さ/奥行の指定が不正です");
        if(n==6){
            sm.exclude=new float[]{  // x,y,z,w,h,d -> x0,x1,y0,y1,z0,z1
                sz[0]-sz[3]/2,sz[0]+sz[3]/2,
                sz[1]-sz[4]/2,sz[1]+sz[4]/2,
                sz[2]-sz[5]/2,sz[2]+sz[5]/2
            };
        }else{
            sm.exclude=new float[]{ sz[0],sz[1],sz[2],sz[3]*sz[3] };   // x,y,z,r -> x,y,z,r^2
        }
        return 1;
    }
    private static int MeshParamVerList(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null || val=="") return 0;
        string[] sa=val.Split(ParseUtil.comma);
        var lst=new List<uint>();
        for(int i=0; i<sa.Length; i++){
            if(!uint.TryParse(sa[i],out uint th)) return sh.io.Error("頂点番号の指定が不正です");
            lst.Add(th);
        }
        sm.list=lst.ToArray();
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

        uint[] idx;
        if(sm.list!=null){
            idx=sm.list;
        }else{
            int[] tri=sm.mi.mesh.GetIndices(sm.submeshno);
            if(tri==null) return sh.io.Error("メッシュ番号が不正です");
            var hs=new HashSet<uint>();
            for(int i=0; i<tri.Length; i++) hs.Add((uint)tri[i]);
            idx=new uint[hs.Count];
            hs.CopyTo(idx,0);
        }

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
                if(sm.filter.Length==6){
                    if( sm.filter[0]>vt.x || sm.filter[1]<vt.x 
                     || sm.filter[2]>vt.y || sm.filter[3]<vt.y
                     || sm.filter[4]>vt.z || sm.filter[5]<vt.z ) continue;
                }else{
                    float x=vt.x-sm.filter[0], y=vt.y-sm.filter[1], z=vt.z-sm.filter[2];
                    if(sm.filter[3] < x*x+y*y+z*z ) continue;
                }
            }
            if(sm.exclude!=null){
                if(sm.filter.Length==6){
                    if( sm.exclude[0]<=vt.x && sm.exclude[1]>=vt.x 
                     && sm.exclude[2]<=vt.y && sm.exclude[3]>=vt.y
                     && sm.exclude[4]<=vt.z && sm.exclude[5]>=vt.z) continue;
                }else{
                    float x=vt.x-sm.filter[0], y=vt.y-sm.filter[1], z=vt.z-sm.filter[2];
                    if(sm.filter[3] >= x*x+y*y+z*z ) continue;
                }
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
        sm.mi.EditMaterial();
        sm.mi.material[sm.submeshno].shader=shader;
        return 1;
    }
    private static int MeshParamProp(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null) return 0;
        string[] kv=ParseUtil.LeftAndRight(val,'=');
        string err;
        sm.mi.EditMaterial();
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
        sm.mi.EditMaterial();
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
        int mode=0;
        if(val!=null && val!=""
            && (!int.TryParse(val,out mode) || mode<0 || mode>2)) return sh.io.Error("0～2で指定してください");
        if(mode!=1) sm.mi.RestoreMesh(sm.submeshno);
        if(mode!=0) sm.mi.RestoreMaterial(sm.submeshno);
        return 0;
    }
    private static int MeshParamRecalc(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null || val=="") return 0;
        bool b=false,n=false,t=false;
        for(int i=0; i<val.Length; i++){
            if(val[i]=='n'||val[i]=='N') n=true;
            else if(val[i]=='t'||val[i]=='T') t=true;
            else if(val[i]=='b'||val[i]=='B') b=true;
            else return sh.io.Error("再計算対象はn,t,bいずれかで指定してください");
        }
        Mesh m=sm.mi.mesh.FindMesh(sm.submeshno);
        if(n) m.RecalculateNormals();
        if(t) m.RecalculateTangents();
        if(b) m.RecalculateBounds();
        return 1;
    }

    private static HashSet<Texture> texiid=new HashSet<Texture>();
    private static int MeshParamTexture(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null || val=="") return 0;

        sm.mi.EditMaterial();
        Material mate=sm.mi.material[sm.submeshno];

        string prop="_MainTex",right="";
        string[] lr=ParseUtil.LeftAndRight(val,'=');
        if(lr[1]=="") right=lr[0]; else { prop=lr[0]; right=lr[1]; }
        if(!mate.HasProperty(prop)) return sh.io.Error("指定されたプロパティは現在のシェーダでは無効です");

        int wrap=1,mode=0;
        lr=ParseUtil.LeftAndRight(right,',');
        if(lr[1]!=""){
            right=lr[0];
            var opts=lr[1].Split(ParseUtil.comma);
            if(opts.Length>=1 && (!int.TryParse(opts[0],out wrap) || wrap<0 || wrap>1)) return sh.io.Error("書式が不正です");
            if(opts.Length>=2 && (!int.TryParse(opts[1],out mode) || mode<0 || mode>1)) return sh.io.Error("書式が不正です");
        }

        Camera cam;
        if(ObjUtil.objDic.TryGetValue(right,out Transform camTr) && (cam=camTr.GetComponent<Camera>())!=null){
            if(mode==0){
                var tex=cam.targetTexture;
                tex.wrapMode=(wrap==1)?TextureWrapMode.Repeat:TextureWrapMode.Clamp;
                var old=mate.GetTexture(prop);
                if(old!=null && texiid.Remove(old)) GameObject.Destroy(old); 
                mate.SetTexture(prop,tex);
            }else{
                RenderTexture rt=cam.targetTexture;
                Texture2D tx;
                var old=mate.GetTexture(prop);
                if(old==null) tx=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);
                else {
                    if(old.name=="__subcamera_pic_" && old.width==rt.width && old.height==rt.height) tx=(Texture2D)old;
                    else{
                        if(old!=null && texiid.Remove(old)) GameObject.Destroy(old); 
                        tx=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);
                    }
                }
                RenderTexture bak=RenderTexture.active;
                RenderTexture.active=rt;
                tx.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);
                tx.Apply();
                tx.name="__subcamera_pic_";
                RenderTexture.active=bak;
                mate.SetTexture(prop,tx);
                texiid.Add(tx);
            }
        }else {
            Texture tex0=Resources.Load<Texture>("SceneCreativeRoom/Debug/Textures/"+right);
            if(tex0==null) tex0=Resources.Load<Texture>("Texture/"+right);
            if(tex0!=null && ReferenceEquals(tex0.GetType(),typeof(Texture2D))){
                Texture2D tex=TextureUtil.CloneTexture((Texture2D)tex0);
                tex.wrapMode=(wrap==1)?TextureWrapMode.Repeat:TextureWrapMode.Clamp;
                var old=mate.GetTexture(prop);
                if(old!=null && texiid.Remove(old)) GameObject.Destroy(old); 
                mate.SetTexture(prop,tex);
                texiid.Add(tex);
                return 1;
            }
            try{
                string fname="";
                if(right.Length>0 && right[0]=='*'){
                    var tf=DataFiles.GetTempFile(right.Substring(1));
                    if(tf!=null) fname=tf.filename;
                }else{
                    fname=ComShInterpreter.homeDir+@"PhotoModeData\\Texture\\"+UTIL.Suffix(right,".png");
                }
                if(System.IO.File.Exists(fname)){
                    byte[] buf=UTIL.ReadAll(fname);
                    Texture2D t2d=new Texture2D(2,2);
                    t2d.LoadImage(buf);
                    t2d.name=right;
                    t2d.wrapMode=(wrap==1)?TextureWrapMode.Repeat:TextureWrapMode.Clamp;
                    var old=mate.GetTexture(prop);
                    if(old!=null && texiid.Remove(old)) GameObject.Destroy(old); 
                    mate.SetTexture(prop,t2d);
                    texiid.Add(t2d);
                    return 1;
                }
                fname=UTIL.Suffix(right,".tex");
                if(GameUty.IsExistFile(fname,GameUty.FileSystem)){
                    var tere=ImportCM.LoadTexture(GameUty.FileSystem,fname,false);
                    var t2d=tere.CreateTexture2D();
                    t2d.name=right;
                    t2d.wrapMode=(wrap==1)?TextureWrapMode.Repeat:TextureWrapMode.Clamp;

                    var old=mate.GetTexture(prop);
                    if(old!=null && texiid.Remove(old)) GameObject.Destroy(old); 
                    mate.SetTexture(prop,t2d);
                    texiid.Add(t2d);
                }else return sh.io.Error("texファイルが見つかりません");
            }catch{ return sh.io.Error("texファイルの読み込みに失敗しました"); }
        }
        return 1;
    }
    private static int MeshParamRQ(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null){
            sh.io.Print(sm.mi.material[sm.submeshno].renderQueue.ToString());
            return 0;
        }
        if(!int.TryParse(val,out int n) || n<0 || n>5000) return sh.io.Error("0～5000の数値を指定して下さい");
        sm.mi.EditMaterial();
        sm.mi.material[sm.submeshno].renderQueue=n;
        return 1;
    }
    private static int MeshParamSS(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null) return 0;
        string fname,prop;
        var sa=ParseUtil.LeftAndRight(val,':');
        if(sa[1]==""){ fname=sa[0]; prop="_MainTex";} else { fname=sa[1]; prop=sa[0];}
        if(fname=="" || fname.IndexOf('\\')>=0 || UTIL.CheckFileName(fname)<0) return sh.io.Error("ファイル名が不正です");
        fname=ComShInterpreter.homeDir+@"ScreenShot\\"+UTIL.Suffix(fname,".png");
        return MeshParamPNGSub(sh,sm,fname,prop);
    }
    private static int MeshParamPNG(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null) return 0;
        string fname,prop;
        var sa=ParseUtil.LeftAndRight(val,':');
        if(sa[1]==""){ fname=sa[0]; prop="_MainTex";} else { fname=sa[1]; prop=sa[0];}

        string file="";
        if(fname!="" && fname.IndexOf('\\')<0){
            if(fname[0]=='*'){
                var tf=new DataFiles.TmpFile(fname.Substring(1),"");
                file=tf.filename;
            }else if(UTIL.CheckFileName(fname)>=0){
                file=ComShInterpreter.homeDir+@"PhotoModeData\\Texture\\"+UTIL.Suffix(fname,".png");
            }
        }
        if(file=="") return sh.io.Error("ファイル名が不正です");
        return MeshParamPNGSub(sh,sm,file,prop);
    }
    private static int MeshParamPNGSub(ComShInterpreter sh,SingleMesh sm,string fname,string prop){
        var tex=sm.mi.material[sm.submeshno].GetTexture(prop);
        if(tex==null) return sh.io.Error("テクスチャがありません");
        if(ReferenceEquals(tex.GetType(),typeof(RenderTexture))){
            if(TextureUtil.Rt2Png((RenderTexture)tex,fname)<0) return sh.io.Error("書き込みに失敗しました");
        }else if(ReferenceEquals(tex.GetType(),typeof(Texture2D))){
            try{
                byte[] buf=((Texture2D)tex).EncodeToPNG();
                System.IO.File.WriteAllBytes(fname,buf);
            }catch{}
        }
        return 1;
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
        public float[] filter=null;
        public float[] exclude=null;
        public uint[] list=null;
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
            if(oi==null) oid=new ObjInfoData(tr);
            else if(oi.data==null) oi.data=oid=new ObjInfoData(tr); else oid=oi.data;

            if(oid.workMesh==null){
                oid.Backup();
                mesh=oid.originalMesh;
            }else{
                mesh=oid.workMesh;
            }
            material=(oid.workMate==null)?oid.originalMate:oid.workMate;
            count=(mesh.Count==0)?0:mesh[mesh.Count-1].no+mesh[mesh.Count-1].submeshcount;
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
            if(oid==null || oid.originalMesh==null || oid.workMesh==null) return -1;
            int midx=oid.originalMesh.FindMeshIdx(submeshno);
            Renderer[] ra=oid.FindComponentsToArray<Renderer>();
            if(ra==null || ra.Length<midx) return -1;
            Mesh newMesh;
            if (ReferenceEquals(ra[midx].GetType(), typeof(SkinnedMeshRenderer))) {
                var smr=(SkinnedMeshRenderer)ra[midx];
                newMesh=Object.Instantiate(oid.originalMesh[midx].mesh);
                var oldMesh=smr.sharedMesh;
                smr.sharedMesh=newMesh;
                oid.UpdateMorph(smr.transform,oldMesh,newMesh);
            }else{
                MeshFilter mf=ra[midx].transform.GetComponent<MeshFilter>();
                if(mf==null) return -1;
                newMesh=Object.Instantiate(oid.originalMesh[midx].mesh);
                mf.mesh=newMesh;
            }
            var e=oid.workMesh[midx];
            Object.Destroy(e.mesh);
            e.mesh=newMesh;
            oid.workMesh[midx]=e;
            return 0;
        }
        public void EditMaterial(){
            if(oi==null){
                // 編集すると後始末が必要になるので、ComShで追加したオブジェト以外にもObjInfoを付ける
                oi=ObjInfo.AddObjInfo(transform,oid,"",null);
            }
            oid.CloneMaterial();
            material=oid.workMate;
        }
        public int RestoreMaterial(int submeshno){
            if(oid==null || oid.originalMate==null || oid.workMate==null) return -1;
            int midx=oid.originalMesh.FindMeshIdx(submeshno);

            Renderer[] ra=oid.FindComponentsToArray<Renderer>();
            if(ra==null || ra.Length<midx) return -1;

            Material newMaterial=Object.Instantiate(oid.originalMate[submeshno]);
            Object.Destroy(oid.workMate[submeshno]);
            oid.workMate[submeshno]=newMaterial;

            var ma=ra[midx].materials;
            ma[submeshno-oid.originalMesh[midx].no]=newMaterial;
            ra[midx].materials=ma;

            return 0;
        }
    }
}
public static class TextureUtil {
    public static Texture2D CloneTexture(Texture2D src){ return CloneTexture(src,src.anisoLevel,src.mipMapBias,src.filterMode); }
    public static Texture2D CloneBitmap(Texture2D src){ return CloneTexture(src,0,0,FilterMode.Point); }
    public static Texture2D CloneTexture(Texture2D src,int anisolv,float mmb,FilterMode flt){ 
        RenderTexture rt=RenderTexture.GetTemporary(src.width,src.height);
        Graphics.Blit(src,rt);
        var bak=RenderTexture.active;
        RenderTexture.active=rt;
        var ret=new Texture2D(rt.width,rt.height);
        ret.wrapMode=src.wrapMode;
        ret.anisoLevel=anisolv;
        ret.mipMapBias=mmb;
        ret.filterMode=flt;
        ret.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);
        ret.Apply();
        RenderTexture.active=bak;
        RenderTexture.ReleaseTemporary(rt);
        return ret;
    }

    public static int Rt2Png(RenderTexture rt,string fname){
        try{
            Texture2D tx=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);

            RenderTexture bak=RenderTexture.active;
            RenderTexture.active=rt;
            tx.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);
            tx.Apply();
            RenderTexture.active=bak;

            byte[] buf=tx.EncodeToPNG();
            UnityEngine.Object.Destroy(tx);
            System.IO.File.WriteAllBytes(fname,buf);
        }catch{ return -1;}
        return 0;
    }

}
}
