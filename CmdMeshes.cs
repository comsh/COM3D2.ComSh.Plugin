using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
        meshParamDic.Add("keyword",new CmdParam<SingleMesh>(MeshParamKeyword));
        meshParamDic.Add("reset",new CmdParam<SingleMesh>(MeshParamReset));
        meshParamDic.Add("commit",new CmdParam<SingleMesh>(MeshParamCommit));
        meshParamDic.Add("texture",new CmdParam<SingleMesh>(MeshParamTexture));
        meshParamDic.Add("texture.new",new CmdParam<SingleMesh>(MeshParamTextureNew));
        meshParamDic.Add("cubemap",new CmdParam<SingleMesh>(MeshParamCubemap));
        meshParamDic.Add("mipmap",new CmdParam<SingleMesh>(MeshParamMipmap));
        meshParamDic.Add("recalc",new CmdParam<SingleMesh>(MeshParamRecalc));
        meshParamDic.Add("rq",new CmdParam<SingleMesh>(MeshParamRQ));
        meshParamDic.Add("ss",new CmdParam<SingleMesh>(MeshParamSS));
        meshParamDic.Add("png",new CmdParam<SingleMesh>(MeshParamPNG));
        meshParamDic.Add("texloop",new CmdParam<SingleMesh>(MeshParamTexLoop));
        meshParamDic.Add("texloop.hsv",new CmdParam<SingleMesh>(MeshParamTexLoopHsv));
        meshParamDic.Add("texfilter",new CmdParam<SingleMesh>(MeshParamTexFilter));
        meshParamDic.Add("texexclude",new CmdParam<SingleMesh>(MeshParamTexExclude));
        meshParamDic.Add("findverno",new CmdParam<SingleMesh>(MeshParamFindVerno));
        meshParamDic.Add("findverlist",new CmdParam<SingleMesh>(MeshParamFindVerlist));
        meshParamDic.Add("weightverno",new CmdParam<SingleMesh>(MeshParamWeightVerno));
        meshParamDic.Add("uvwh",new CmdParam<SingleMesh>(MeshParamUVWH));
        meshParamDic.Add("graft",new CmdParam<SingleMesh>(MeshParamGraft));
        meshParamDic.Add("reverse",new CmdParam<SingleMesh>(MeshParamReverse));
        meshParamDic.Add("texcolmat",new CmdParam<SingleMesh>(MeshParamTexColorMatrix));
        meshParamDic.Add("texcolmat.hsv",new CmdParam<SingleMesh>(MeshParamTexColorMatrixHsv));
        meshParamDic.Add("video",new CmdParam<SingleMesh>(MeshParamVideo));
        meshParamDic.Add("video.loop",new CmdParam<SingleMesh>(MeshParamVideoLoop));
        meshParamDic.Add("video.pause",new CmdParam<SingleMesh>(MeshParamVideoPause));
        meshParamDic.Add("video.mute",new CmdParam<SingleMesh>(MeshParamVideoMute));
        meshParamDic.Add("video.a3d",new CmdParam<SingleMesh>(MeshParamAudio3D));
        meshParamDic.Add("video.time",new CmdParam<SingleMesh>(MeshParamVideoTime));
        meshParamDic.Add("video.timep",new CmdParam<SingleMesh>(MeshParamVideoTimep));
        meshParamDic.Add("video.length",new CmdParam<SingleMesh>(MeshParamVideoLength));
        meshParamDic.Add("video.speed",new CmdParam<SingleMesh>(MeshParamVideoSpeed));
    }
    private static Dictionary<string,CmdParam<SingleMesh>> meshParamDic=new Dictionary<string,CmdParam<SingleMesh>>();

    private static int CmdMesh(ComShInterpreter sh,List<string> args){
        if(args.Count==1) return 0;
        var cd=new ParseUtil.ColonDesc(args[1]);
        if(cd.meshno<0){
            if(args.Count!=2) return sh.io.Error("メッシュ番号が不正です");
            Transform tr=ObjUtil.FindObj(sh,cd);
            if(tr==null) return sh.io.Error("オブジェクトが存在しません");
            Transform tr0=ObjUtil.FindObjRoot(sh,cd);
            if(tr0==null) return sh.io.Error("オブジェクトが存在しません");
            var mi=new MeshInfo(tr,tr0);
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
        Transform tr0=ObjUtil.FindObjRoot(sh,cd);
        if(tr0==null) return sh.io.Error("オブジェクトが存在しません");
        var mi=new MeshInfo(tr,tr0);
        if(mi.count<=cd.meshno) return sh.io.Error("指定されたメッシュが存在しません");
        if(args.Count==startpos){
            if(mi.material.Count>cd.meshno) sh.io.Print($"material:{mi.material[cd.meshno].name}\nshader:{mi.material[cd.meshno].shader.name}");
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
        }else if(n==4){
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
        }else if(n==4){
            sm.exclude=new float[]{ sz[0],sz[1],sz[2],sz[3]*sz[3] };   // x,y,z,r -> x,y,z,r^2
        }
        return 1;
    }
    private static int MeshParamVerList(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null || val=="") return 0;
        string[] sa=val.Split(ParseUtil.comma);
        var lst=new List<int>();
        int vcnt=sm.mi.oid.originalMesh.FindMesh(sm.submeshno).vertices.Length;
        for(int i=0; i<sa.Length; i++){
            if(!int.TryParse(sa[i],out int th)||th>=vcnt) return sh.io.Error("頂点番号の指定が不正です");
            lst.Add(th);
        }
        sm.list=lst.ToArray();
        return 1;
    }
    private static int MeshParamFindVerno(ComShInterpreter sh,SingleMesh sm,string val){
        var va=MeshParamFindVerSub(sm,val,out string err);
        if(err!="") return sh.io.Error(err);
        if(va!=null && va.Length>0){
            Array.Sort(va);
            sh.io.Print(va[0].ToString());
            for(int i=1; i<va.Length; i++) sh.io.Print(',').Print(va[i].ToString());
        }
        return 0;
    }
    private static int MeshParamFindVerlist(ComShInterpreter sh,SingleMesh sm,string val){
        var va=MeshParamFindVerSub(sm,val,out string err);
        if(err!="") return sh.io.Error(err);
        if(va!=null && va.Length>0) sm.list=va;
        return 1;
    }
    private static char[] verqueryletters={'c','l','L'};
    private static int[] emptyva=new int[0];
    private static int[] MeshParamFindVerSub(SingleMesh sm,string val,out string err){
        err="";
        float[] fa={-1,-1,-1};
        if(ParseUtil.GetLetterFloat(val,verqueryletters,fa)<0){err=ParseUtil.error; return null;}
        int conn=(int)fa[0],loop=(int)fa[1],loop2=(int)fa[2];
        if(loop2>=0){
            if(loop>=0){ err="lとLは同時に指定できません"; return null;}
            loop=loop2;
        }
        if(loop>=0){
            if(conn<0 && (sm.list==null || sm.list.Length==0)){ err="cまたはverlistを指定してください"; return null;}
        }

        int[] tri=sm.mi.GetIndices3(sm.submeshno);

        var cnd=new HashSet<int>();
        var hs=new HashSet<int>();

        if(sm.list!=null) for(int i=0; i<sm.list.Length; i++) hs.Add((int)sm.list[i]);
        else for(int i=0; i<tri.Length; i++) hs.Add(tri[i]);

        if(conn>=0){
            foreach(int th in hs){
                int cnt=0;
                for(int ti=0; ti<tri.Length; ti+=3)
                    if(tri[ti]==th||tri[ti+1]==th||tri[ti+2]==th) cnt++;
                if(cnt==conn) cnd.Add(th);
            }
        }
        if(loop>0){
            if(conn<0) cnd=hs;
            var all=new HashSet<int>(cnd);
            var edge=hashset2arr<int>(cnd);
            var added=new HashSet<int>();
            for(int l=0; l<loop; l++){
                added.Clear();
                for(int ei=0; ei<edge.Length; ei++){
                    int th=edge[ei];
                    for(int ti=0; ti<tri.Length; ti+=3)
                        if(tri[ti]==th){ added.Add(tri[ti+1]); added.Add(tri[ti+2]);}
                        else if(tri[ti+1]==th){ added.Add(tri[ti]); added.Add(tri[ti+2]);}
                        else if(tri[ti+2]==th){ added.Add(tri[ti]); added.Add(tri[ti+1]);}
                }
                added.ExceptWith(all);
                if(added.Count==0) break;
                all.UnionWith(added);
                edge=hashset2arr<int>(added);
            }
            if(loop2>0) cnd=all;
            else cnd=added;
        }
        if(cnd.Count>0){
            int[] rslt=new int[cnd.Count];
            cnd.CopyTo(rslt);
            return rslt;
        }
        return emptyva;
    }
    private static T[] hashset2arr<T>(HashSet<T> hs){ var ret=new T[hs.Count]; hs.CopyTo(ret,0); return ret; }

    private static Vector2[] empty_uv=new Vector2[0];
    private static int MeshParamVerLoop(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null || val=="") return 0;

        int[] idx;
        int[] tri=sm.mi.GetIndices3(sm.submeshno);
        Mesh m=sm.mi.mesh.FindMesh(sm.submeshno);

        if(sm.list!=null){
            idx=sm.list;
        }else{
            var hs=new HashSet<int>();
            for(int i=0; i<tri.Length; i++) hs.Add(tri[i]);
            idx=new int[hs.Count];
            hs.CopyTo(idx,0);
        }

        var psr=EvalParser(sh,Command.currentArgNo+1);
        if(psr==null) return -1;

        ComShInterpreter.Output orig=sh.io.output;
        var subout=new ComShInterpreter.SubShOutput();
        sh.io.output=new ComShInterpreter.Output(subout.Output);

        int ret=0;

        var vta=m.vertices;
        var nma=m.normals;
        var uva=m.uv;
        var uv2a=(m.uv2!=null)?m.uv2:empty_uv;
        var uv3a=(m.uv3!=null)?m.uv3:empty_uv;
        var uv4a=(m.uv4!=null)?m.uv4:empty_uv;
        
        var changes=new List<VerLoopChange>();
        byte changed=0;
        for(int i=0; i<idx.Length; i++){
            int th=(int)idx[i];
            var vt=vta[th];

            int inc=sm.ApplyFilter(vt);
            if(inc!=1) continue;

            sh.env["_1"]=i.ToString();
            sh.env["_2"]=th.ToString();

            string svt="",snm="",suv="",suv2="",suv3="",suv4="";
            svt=sh.fmt.FPos(vt);
            if(th<nma.Length) snm=sh.fmt.FPos(nma[th]);
            if(th<uva.Length) suv=sh.fmt.FXY(uva[th]);
            if(th<uv2a.Length) suv2=sh.fmt.FXY(uv2a[th]);
            if(th<uv3a.Length) suv3=sh.fmt.FXY(uv3a[th]);
            if(th<uv4a.Length) suv4=sh.fmt.FXY(uv4a[th]);

            sh.env["_vertex"]=svt;
            sh.env["_normal"]=snm;
            sh.env["_uv"]=suv;
            if(suv2.Length>0) sh.env["_uv2"]=suv2;
            if(suv3.Length>0) sh.env["_uv3"]=suv3;
            if(suv4.Length>0) sh.env["_uv4"]=suv4;

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
            if(suv2!="" && (t=sh.env["_uv2"])!=suv2){
                if(c==null) c=new VerLoopChange(th);
                c.uv2=t;
                changed|=8;
            }
            if(suv3!="" && (t=sh.env["_uv3"])!=suv3){
                if(c==null) c=new VerLoopChange(th);
                c.uv3=t;
                changed|=16;
            }
            if(suv4!="" && (t=sh.env["_uv4"])!=suv4){
                if(c==null) c=new VerLoopChange(th);
                c.uv4=t;
                changed|=32;
            }
            if(c!=null) changes.Add(c);
        }
        if(ret<0){ sh.io.output=orig; return ret; }

        if(changed!=0){
            sm.mi.EditMesh();
            m=sm.mi.mesh.FindMesh(sm.submeshno);
        }

        List<int> remove=new List<int>();
        foreach(var c in changes){
            if(c.vertex!=null){
                if(c.vertex.Length==0) remove.Add(c.idx);
                else{
                    float[] xyz=ParseUtil.Xyz(c.vertex);
                    if(xyz==null){ ret=sh.io.Error($"頂点{c.idx} 座標の形式が不正です"); break; }
                    vta[c.idx]=new Vector3(xyz[0],xyz[1],xyz[2]);
                }
            }
            if(c.normal!=null){
                float[] xyz=ParseUtil.Xyz(c.normal);
                if(xyz==null){ ret=sh.io.Error($"頂点{c.idx}: 法線ベクトルの形式が不正です"); break;}
                nma[c.idx]=new Vector3(xyz[0],xyz[1],xyz[2]);
            }
            if(c.uv!=null){
                float[] xy=ParseUtil.Xy(c.uv);
                if(xy==null){ ret=sh.io.Error($"頂点{c.idx}: UV座標の形式が不正です"); break;}
                uva[c.idx]=new Vector2(xy[0],xy[1]);
            }
            if(c.uv2!=null){
                float[] xy=ParseUtil.Xy(c.uv2);
                if(xy==null){ ret=sh.io.Error($"頂点{c.idx}: UV座標の形式が不正です"); break;}
                if(uv2a.Length<=c.idx){
                    var old=uv2a;
                    uv2a=new Vector2[vta.Length];
                    old.CopyTo(uv2a,0);
                }
                uv2a[c.idx]=new Vector2(xy[0],xy[1]);
            }
            if(c.uv3!=null){
                float[] xy=ParseUtil.Xy(c.uv3);
                if(xy==null){ ret=sh.io.Error($"頂点{c.idx}: UV座標の形式が不正です"); break;}
                if(uv3a.Length<=c.idx){
                    var old=uv3a;
                    uv3a=new Vector2[vta.Length];
                    old.CopyTo(uv3a,0);
                }
                uv3a[c.idx]=new Vector2(xy[0],xy[1]);
            }
            if(c.uv4!=null){
                float[] xy=ParseUtil.Xy(c.uv4);
                if(xy==null){ ret=sh.io.Error($"頂点{c.idx}: UV座標の形式が不正です"); break;}
                if(uv4a.Length<=c.idx){
                    var old=uv4a;
                    uv4a=new Vector2[vta.Length];
                    old.CopyTo(uv4a,0);
                }
                uv4a[c.idx]=new Vector2(xy[0],xy[1]);
            }
        }
        if(ret<0){ sh.io.output=orig; return ret; }
        if((changed&1)!=0) m.vertices=vta;
        if((changed&2)!=0) m.normals=nma;
        if((changed&4)!=0) m.uv=uva;
        if((changed&8)!=0) m.uv2=uv2a;
        if((changed&16)!=0) m.uv3=uv3a;
        if((changed&32)!=0) m.uv4=uv4a;
        if(remove.Count>0) RemoveVertex(m,sm,remove);

        sh.io.output=orig;
        sh.io.Print(subout.GetSubShResult());
        return 1;
    }
    private static void RemoveVertex(Mesh m,SingleMesh sm,List<int> lst){
        int[] tri=sm.mi.GetIndices3(sm.submeshno);
        List<int> tri2=new List<int>(tri.Length);
        for(int i=0; i<tri.Length; i+=3){
            int j; for(j=0; j<lst.Count; j++) if(lst[j]==tri[i]||lst[j]==tri[i+1]||lst[j]==tri[i+2]) break;
            if(j==lst.Count){ tri2.Add(tri[i]); tri2.Add(tri[i+1]); tri2.Add(tri[i+2]);}
        }
        int[] ia=tri2.ToArray();
        sm.mi.mesh.UpdateIndices3(sm.submeshno,ia);
        var mt=sm.mi.mesh.GetTopology(sm.submeshno);
        if(mt==MeshTopology.Triangles)
            sm.mi.mesh.SetIndices(ia,mt,sm.submeshno);
        else if(mt==MeshTopology.Lines)
            sm.mi.mesh.SetIndices(Indices2(ia),mt,sm.submeshno);
        else if(mt==MeshTopology.Points)
            sm.mi.mesh.SetIndices(Indices1(ia),mt,sm.submeshno);
    }
    private static int MeshParamShader(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null){
            sh.io.Print(sm.mi.material[sm.submeshno].shader.name);
            return 0;
        }

        Shader shader=null;
        Material mate=null;
        string[] lr=ParseUtil.LeftAndRight2(val,':');
        if(lr[0]!=""){
            var lst=ObjUtil.ListAssetBundle<Material>(lr[0]);
            if(lr[1]==""){
                var names=new List<string>();
                foreach(var s in lst){
                    mate=ObjUtil.LoadAssetBundle<Material>(lr[0],s);
                    if(mate==null || mate.shader==null){GameObject.Destroy(mate); continue;}
                    if(mate.shader.name!=null) names.Add(mate.shader.name);
                }
                if(names.Count==0) return 0;
                names.Sort();
                var prev="";
                foreach(var s in names) if(s!=prev){ sh.io.PrintLn(s); prev=s; }
                return 0;
            }
            foreach(var s in lst){
                mate=ObjUtil.LoadAssetBundle<Material>(lr[0],s);
                if(mate==null || mate.shader==null || mate.shader.name!=lr[1]){GameObject.Destroy(mate); continue;}
                shader=mate.shader;
            }
        }else shader=Shader.Find(val);
        if(shader==null) return sh.io.Error("指定されたシェーダは見つかりません");
        sm.mi.EditMaterial();
        sm.mi.material[sm.submeshno].shader=shader;
        if(mate!=null){
            sm.mi.material[sm.submeshno].shaderKeywords=(string[])mate.shaderKeywords.Clone();
        }
        return 1;
    }
    private static string[] propnames={
        "_Color","_ShadowColor","_Shininess","_RimColor","_RimShift","_RimPower","_HiRate","_HiPow",
        "_OutlineColor","_OutlineWidth","_Cutoff","_StencilMask","_ZWrite","_Cull",
        "_Alpha","_ShininessPow","_EnvAlpha","_EnvAdd","_SetManualRenderQueue",
        "_InvFade","_Mask1","_Mask2","_Mask3", "_FloatValue1","_FloatValue2","_FloatValue3",
        "_Glossiness","_Metallic","_BumpScale","_Parallax","_OcclusionStrength","_SpecularHighlights","_GlossyReflections",
        "_EmissionColor","_DetailNormalMapScale","_SmoothnessTextureChannel","_SpecColor",
        "_UVSec","_Mode","_SrcBlend","_DstBlend","_TintColor","_Step",
        "_Brightness","_Direction","_ScanTiling","_ScanSpeed","_GlowTiling","_GlowSpeed","GlitchSpeed","_GlitchIntensity", "_FlickerSpeed","_Fold"
    };
    public static void PrintShaderProps(ComShInterpreter sh,Material mate,string[] def=null){
        string[] pns=def;
        if(pns==null) pns=propnames;
        for(int i=0; i<pns.Length; i++){
            string prop=pns[i];
            if(!mate.HasProperty(prop)) continue;

            var c=mate.GetColor(prop);
            var v=mate.GetVector(prop);
            if(prop.EndsWith("Color",StringComparison.Ordinal)){
                var col=mate.GetColor(prop);
                sh.io.Print(prop).Print(':').PrintLn(sh.fmt.RGBA(c));
            }else{
                float f=mate.GetFloat(prop);
                sh.io.Print(prop).Print(':').PrintLn(sh.fmt.FVal(f));
            }
        }
        var ka=mate.shaderKeywords;
        if(ka!=null && ka.Length>0) sh.io.Print("keywords:").PrintJoin(sh.ofs,ka);
    }
    private static int MeshParamProp(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null){
            PrintShaderProps(sh,sm.mi.material[sm.submeshno]);
            return 0;
        }

        if(val.IndexOf('=')<0){
            var m=sm.mi.material[sm.submeshno];
            if(!m.HasProperty(val)) return sh.io.Error("指定されたプロパティは現在のシェーダでは無効です");
            if(val.EndsWith("Color",StringComparison.Ordinal)){
                var c=m.GetColor(val);
                sh.io.Print(sh.fmt.RGBA(c));
            }else{
                var f=m.GetFloat(val);
                sh.io.Print(sh.fmt.FVal(f));
            }
            return 0;
        }

        sm.mi.EditMaterial();

        string err=SetShaderProps(sm.mi.material[sm.submeshno],val);
        if(err!="") return sh.io.Error(err);
        return 1;
    }
    public static string SetShaderProps(Material mate,string val){
        string err="";
        var prop=ParseUtil.ParseProp(val);
        if(prop==null) return "書式が不正です";
        foreach(var kv in prop.kva){
            if(kv.value=="on") mate.EnableKeyword(kv.key);
            else if(kv.value=="off") mate.DisableKeyword(kv.key);
            else if(kv.value.IndexOf(',')>=0){
                if((err=SetVectorProp(mate,kv.key,kv.value))!="") return err;
            }else{
                if((err=SetFloatProp(mate,kv.key,kv.value))!="") return err;
            }
        }
        return err;
    }
    public static string SetVectorProp(Material m, string key, string val){
        if(!m.HasProperty(key)) return "指定されたプロパティは現在のシェーダでは無効です";
        float[] fa={0,0,0,0};
        int n=ParseUtil.XyzSub(val,fa);
        if(n==0||n>4) return "値が不正です";
        m.SetVector(key,new Vector4(fa[0],fa[1],fa[2],fa[3]));
        return "";
    }
    public static string SetFloatProp(Material m, string key, string val){
        if(!m.HasProperty(key)) return "指定されたプロパティは現在のシェーダでは無効です";
        float f=ParseUtil.ParseFloat(val);
        if(float.IsNaN(f)) return ParseUtil.error;
        m.SetFloat(key,f);
        return "";
    }
    private static int MeshParamKeyword(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null){
            string[] kwa=sm.mi.material[sm.submeshno].shaderKeywords;
            for(int i=0; i<kwa.Length; i++) sh.io.Print(kwa[i]);
            return 0;
        }
        sm.mi.EditMaterial();
        var arr=val.Split(ParseUtil.comma);
        for(int i=0; i<arr.Length; i++) if(arr[i].Length>0){
            var wd=arr[i];
            var c=wd[wd.Length-1];
            if(c=='-') sm.mi.material[sm.submeshno].DisableKeyword(wd.Substring(0,wd.Length-1));
            else if(c=='+') sm.mi.material[sm.submeshno].EnableKeyword(wd.Substring(0,wd.Length-1));
            else sm.mi.material[sm.submeshno].EnableKeyword(wd);
        }
        return 1;
    }
    private static int MeshParamBlend(ComShInterpreter sh,SingleMesh sm,string val){
        if(sm.mi.material[sm.submeshno].shader.name!="Standard"){
            return sh.io.Error("Standardシェーダでのみ有効です");
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
            mi.mesh.SetIndices(Indices1(mi.mesh.indices3[n]),MeshTopology.Points,n);
        }else if(val=="2"){
            mi.EditMesh();
            if(mi.mesh.GetTopology(n)==MeshTopology.Lines) return 1;
            MeshTopology mt=mi.oid.originalMesh.GetTopology(n);
            if(mt!=MeshTopology.Triangles) return sh.io.Error($"未対応の形式です(topology={mt.ToString()})");
            mi.mesh.SetIndices(Indices2(mi.mesh.indices3[n]),MeshTopology.Lines,n);
        }else if(val=="3"){
            if(mi.mesh.GetTopology(n)==MeshTopology.Triangles) return 1;
            mi.mesh.RestoreIndices3(n);
        }
        return 1;
    }
    public static int[] Indices1(int[] ia){
        var hs=new HashSet<int>();
        for(int i=0; i<ia.Length; i++) hs.Add(ia[i]);
        int[] ia2=new int[hs.Count];
        hs.CopyTo(ia2);
        return ia2;
    }
    public static int[] Indices2(int[] ia){
        var hs=new HashSet<uint>();
        for(int i=0; i<ia.Length; i+=3){
            hs.Add((ia[i]<ia[i+1])?(uint)(ia[i]*65536+ia[i+1]):(uint)(ia[i+1]*65536+ia[i]));
            hs.Add((ia[i+1]<ia[i+2])?(uint)(ia[i+1]*65536+ia[i+2]):(uint)(ia[i+2]*65536+ia[i+1]));
            hs.Add((ia[i+2]<ia[i])?(uint)(ia[i+2]*65536+ia[i]):(uint)(ia[i]*65536+ia[i+2]));
        }
        int[] ia2=new int[hs.Count*2];
        int cnt=0;
        foreach(var u in hs){ ia2[cnt++]=(int)(u>>16); ia2[cnt++]=(int)(u&0xffff); }
        return ia2;
    }
    private static int MeshParamReset(ComShInterpreter sh,SingleMesh sm,string val){
        int mode=2;
        if(val!=null && val!=""
            && (!int.TryParse(val,out mode) || mode<0 || mode>2)) return sh.io.Error("0～2で指定してください");
        if(mode!=1) sm.mi.RestoreMesh(sm.submeshno);
        if(mode!=0) sm.mi.RestoreMaterial(sm.submeshno);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        return (val==null)?0:1;
    }
    private static int MeshParamCommit(ComShInterpreter sh,SingleMesh sm,string val){
        int mode=2;
        if(val!=null && val!=""
            && (!int.TryParse(val,out mode) || mode<0 || mode>2)) return sh.io.Error("0～2で指定してください");

        if(mode!=1) sm.mi.oid.CommitMesh(sm.submeshno);
        if(mode!=0) sm.mi.oid.CommitMaterial(sm.submeshno);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        return (val==null)?0:1;
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
        sm.mi.EditMesh();
        Mesh m=sm.mi.mesh.FindMesh(sm.submeshno);
        if(n) m.RecalculateNormals();
        if(t) m.RecalculateTangents();
        if(b) m.RecalculateBounds();
        return 1;
    }

    public static HashSet<Texture> texiid=new HashSet<Texture>();
    private static int MeshParamTexture(ComShInterpreter sh,SingleMesh sm,string val){
        Material mate;
        if(val==null){
            mate=sm.mi.material[sm.submeshno];
            for(int i=0; i<TextureUtil.texnames.Length; i++) PrintTextureInfo(sh,mate,TextureUtil.texnames[i]);
            return 0;
        }
        if(val=="") return 0;

        sm.mi.EditMaterial();
        mate=sm.mi.material[sm.submeshno];

        string prop="_MainTex",right="";
        string[] lr=ParseUtil.LeftAndRight2(val,ParseUtil.eqcln);
        if(lr[0]!="") prop=lr[0];
        right=lr[1];
        if(!mate.HasProperty(prop)) return sh.io.Error("指定されたプロパティは現在のシェーダでは無効です");

        string msg=SetTexProp(mate,prop,right,sm.mi.oid.originalMate[sm.submeshno]);
        if(msg!="") return sh.io.Error(msg);
        return 1;
    }
    private static int MeshParamCubemap(ComShInterpreter sh,SingleMesh sm,string val){
        Material mate;
        if(val==null){
            mate=sm.mi.material[sm.submeshno];
            for(int i=0; i<TextureUtil.texnames.Length; i++) PrintTextureInfo(sh,mate,TextureUtil.texnames[i]);
            return 0;
        }
        if(val=="") return 0;
        sm.mi.EditMaterial();
        mate=sm.mi.material[sm.submeshno];
        string[] lr=ParseUtil.LeftAndRight(val,ParseUtil.eqcln);
        string prop=lr[0],right=lr[1];
        if(prop=="") return sh.io.Error("書式が不正です");
        if(!mate.HasProperty(prop)) return sh.io.Error("指定されたプロパティは現在のシェーダでは無効です");
        string msg=SetTexProp(mate,prop,right,sm.mi.oid.originalMate[sm.submeshno],1,1);
        if(msg!="") return sh.io.Error(msg);
        return 1;
    }
    public static void PrintTextureInfo(ComShInterpreter sh,Material mate,string prop){
        if(!mate.HasProperty(prop)) return;
        Texture tx=mate.GetTexture(prop);
        if(tx==null) return;
        string col="";
        if(tx.GetType()==typeof(Texture2D)) col=sh.ofs+((Texture2D)tx).format.ToString();
        sh.io.PrintLn($"{prop}:{tx.name}{sh.ofs}{tx.width}x{tx.height}{sh.ofs}{tx.GetType().Name}{col}");
    }
    private static string SetTex(string prop,string name,int mode,int wrap,Material mate,Material orig,int cube){
        Camera cam;
        ReflectionProbe rp;
        var origtex=(orig==null)?null:orig.GetTexture(prop);
        if(name==""){
            var old=mate.GetTexture(prop);
            if(old!=null && old!=origtex && texiid.Remove(old)) UnityEngine.Object.Destroy(old); 
            mate.SetTexture(prop,null);
        }else if(ObjUtil.objDic.TryGetValue(name,out Transform camTr) && (cam=camTr.GetComponent<Camera>())!=null){
            if(mode==0){
                var tex=cam.targetTexture;
                tex.wrapMode=(wrap==1)?TextureWrapMode.Repeat:TextureWrapMode.Clamp;
                var old=mate.GetTexture(prop);
                if(old!=null && old!=origtex && texiid.Remove(old)) UnityEngine.Object.Destroy(old); 
                mate.SetTexture(prop,tex);
            }else{
                var old=mate.GetTexture(prop);
                Texture t;
                var rt=cam.targetTexture;
                Texture2D tx;
                if(old==null) tx=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);
                else {
                    if(old.name=="__subcamera_pic_" && old.width==rt.width && old.height==rt.height) tx=(Texture2D)old;
                    else{
                        if(old!=null && old!=origtex && texiid.Remove(old)) UnityEngine.Object.Destroy(old); 
                        tx=new Texture2D(rt.width,rt.height,TextureFormat.RGBA32,false);
                    }
                }
                RenderTexture bak=RenderTexture.active;
                RenderTexture.active=rt;
                tx.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);
                tx.Apply();
                t=tx;
                RenderTexture.active=bak;
                t.name="__subcamera_pic_";
                mate.SetTexture(prop,t);
                texiid.Add(t);
            }
        }else if(ObjUtil.objDic.TryGetValue(name,out Transform rpTr) && (rp=rpTr.GetComponent<ReflectionProbe>())!=null){
            var tex=rp.texture;
            var old=mate.GetTexture(prop);
            if(old!=null && old!=origtex && texiid.Remove(old)) UnityEngine.Object.Destroy(old); 
            mate.SetTexture(prop,tex);
        }else if(cube==1&&name.IndexOf(':')>=0){
                string[] lr=ParseUtil.LeftAndRight2(name,':');
                if(lr[0]=="") return "ファイルが見つかりません";
                Cubemap cm=ObjUtil.LoadAssetBundle<Cubemap>(lr[0],lr[1],ComShInterpreter.textureDir);
                if(cm==null) return "ファイルが見つかりません";
                var old=mate.GetTexture(prop);
                if(old!=null && old!=origtex && texiid.Remove(old)) UnityEngine.Object.Destroy(old); 
                mate.SetTexture(prop,cm);
                texiid.Add(cm);
        }else {
            Texture tex0=TextureUtil.ReadTexture(name);
            if(tex0==null) return "texファイルが見つかりません";
            if(ReferenceEquals(tex0.GetType(),typeof(Texture2D))){
                if(cube==1){
                    Cubemap cm=TextureUtil.T2DToCube((Texture2D)tex0,mode);
                    cm.wrapMode=(wrap==1)?TextureWrapMode.Repeat:TextureWrapMode.Clamp;
                    var old=mate.GetTexture(prop);
                    if(old!=null && old!=origtex && texiid.Remove(old)) UnityEngine.Object.Destroy(old); 
                    mate.SetTexture(prop,cm);
                    texiid.Add(cm);
                }else{
                    Texture2D tex=TextureUtil.CloneTexture((Texture2D)tex0);
                    tex.wrapMode=(wrap==1)?TextureWrapMode.Repeat:TextureWrapMode.Clamp;
                    var old=mate.GetTexture(prop);
                    if(old!=null && old!=origtex && texiid.Remove(old)) UnityEngine.Object.Destroy(old); 
                    mate.SetTexture(prop,tex);
                    texiid.Add(tex);
                }
            }
        }
        return "";
    }
    public static string SetTexProp(Material m, string key, string val,Material orig,int wrap0=1,int cube=0){
        if(!m.HasProperty(key)) return "指定されたプロパティは現在のシェーダでは無効です";
        int wrap=wrap0,mode=0;
        string right=val;
        string[] lr=ParseUtil.LeftAndRight(right,',');
        if(lr[1]!=""){
            right=lr[0];
            var opts=lr[1].Split(ParseUtil.comma);
            if(cube==1){
                if(opts.Length>=1 && (!int.TryParse(opts[0],out mode) || mode<0 || mode>1)) return "書式が不正です";
            }else{
                if(opts.Length>=1 && (!int.TryParse(opts[0],out wrap) || wrap<0 || wrap>1)) return "書式が不正です";
                if(opts.Length>=2 && (!int.TryParse(opts[1],out mode) || mode<0 || mode>1)) return "書式が不正です";
            }
        }
        return SetTex(key,right,mode,wrap,m,orig,cube);
    }
    private static int MeshParamTextureNew(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null || val=="") return 0;

        sm.mi.EditMaterial();
        Material mate=sm.mi.material[sm.submeshno];
        string prop="_MainTex",right="";
        string[] lr=ParseUtil.LeftAndRight(val,':');
        if(lr[1]=="") right=lr[0]; else { prop=lr[0]; right=lr[1]; }

        Material orig=sm.mi.oid.originalMate[sm.submeshno];
        var origtex=(orig==null)?null:orig.GetTexture(prop);

        string[] sa=right.Split(ParseUtil.comma);
        int w=0,h=0,wrap=0;
        if(sa.Length>=1){
            if(!int.TryParse(sa[0],out w) || w<=0)
                return sh.io.Error("数値が不正です");
            h=w;
        }
        if(sa.Length>=2){
            if(!int.TryParse(sa[1],out h) || h<=0)
                return sh.io.Error("数値が不正です");
        }
        if(sa.Length>=3){
            switch(sa[2]){
            case "0": wrap=0; break;
            case "1": wrap=1; break;
            default: return sh.io.Error("数値が不正です");
            }
        }

        var t2d=new Texture2D(w,h);
        t2d.wrapMode=(wrap==1)?TextureWrapMode.Repeat:TextureWrapMode.Clamp;
        var old=mate.GetTexture(prop);
        if(old!=null && old!=origtex && texiid.Remove(old)) UnityEngine.Object.Destroy(old); 
        mate.SetTexture(prop,t2d);
        texiid.Add(t2d);
        return 1;
    }
    private static int MeshParamTexFilter(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null || val=="") return 0;
        var fa=TexFilterSub(sh,val);
        if(fa==null) return -1;
        sm.area=fa;
        return 1;
    }
    private static int MeshParamTexExclude(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null || val=="") return 0;
        var fa=TexFilterSub(sh,val);
        if(fa==null) return -1;
        sm.areaex=fa;
        return 1;
    }
    private static float[] TexFilterSub(ComShInterpreter sh,string val){
        float[] sz=new float[4];
        int n=ParseUtil.XyzSub(val,sz);
        if(n!=4){ sh.io.Error("範囲指定が不正です"); return null; }
        float x0=sz[0],y0=sz[1],w=sz[2],h=sz[3];
        if(x0<0||y0<0||w<=0||h<=0){ sh.io.Error("範囲指定が不正です"); return null;}
        return new float[]{x0,y0,w,h};
    }

    private static Regex shaderpropptn=new Regex(@"^_\w+:",RegexOptions.Compiled);
    private static int MeshParamTexLoop(ComShInterpreter sh,SingleMesh sm,string val){ return MeshParamTexLoopSub(sh,sm,val,0); }
    private static int MeshParamTexLoopHsv(ComShInterpreter sh,SingleMesh sm,string val){ return MeshParamTexLoopSub(sh,sm,val,1); }
    private static int MeshParamTexLoopSub(ComShInterpreter sh,SingleMesh sm,string val,int hsvq){
        if(val==null || val=="") return 0;

        string prop,cmd;
        if(shaderpropptn.IsMatch(val)){
            string[] lr=ParseUtil.LeftAndRight(val,':');
            prop=lr[0]; cmd=lr[1];
        }else{
            prop="_MainTex"; cmd=val;
        }

        sm.mi.EditMaterial();
        Material mate=sm.mi.material[sm.submeshno];
        Material mate0=sm.mi.oid.originalMate[sm.submeshno];
        if(!mate.HasProperty(prop)) return sh.io.Error("プロパティが無効です");
        Texture tx=mate.GetTexture(prop);
        if(tx==null) return sh.io.Error("テクスチャがありません");

        var psr=EvalParser(sh,Command.currentArgNo+1,true,-1,cmd);
        if(psr==null) return -1;

        ComShInterpreter.Output orig=sh.io.output;
        var sbo=new ComShInterpreter.SubShOutput();
        int ret=0;
        sh.io.output=new ComShInterpreter.Output(sbo.Output);
        ret=PixelLoop(sh,sm,psr,mate,prop,tx,hsvq,mate0);
        sh.io.output=orig;
        if(ret>=0) sh.io.Print(sbo.GetSubShResult());
        return 1;
    }
    private static int PixelLoop(ComShInterpreter sh,SingleMesh sm,ComShParser psr,Material mate,string prop,Texture tx,int hsvq,Material orig){
        int ret=0;
        var origtex=orig.GetTexture(prop);
        Texture2D t2;
        bool t2cloneq=false;
        if(origtex==tx || !ReferenceEquals(tx.GetType(),typeof(Texture2D))){
            t2=TextureUtil.CloneTexture(tx);
            t2cloneq=true;
        }else t2=(Texture2D)tx;
        int width=t2.width,height=t2.height;
        var r=sm.GetTexXYRange(width,height);
        Color[] ca=t2.GetPixels(r.x,r.y,r.w,r.h);
        bool changed=false;
        if(hsvq==0) for(int dy=0; dy<r.h; dy++) for(int dx=0; dx<r.w; dx++){
            int x=r.x+dx,y=r.y+dy;
            int i=dy*r.w+dx;
            if(sm.ApplyTexFilter(x,y)==0) continue;
            sh.env["_1"]=sh.fmt.FVal((float)x/width);
            sh.env["_2"]=sh.fmt.FVal((float)y/height);
            sh.env["_3"]=width.ToString();
            sh.env["_4"]=height.ToString();
            sh.env["_5"]=x.ToString();
            sh.env["_6"]=y.ToString();
            sh.env["_r"]=sh.fmt.F0to1(ca[i].r);
            sh.env["_g"]=sh.fmt.F0to1(ca[i].g);
            sh.env["_b"]=sh.fmt.F0to1(ca[i].b);
            sh.env["_a"]=sh.fmt.F0to1(ca[i].a);
            psr.Reset();
            ret=sh.InterpretParser(psr);
            if(ret<0 || sh.exitq){
                ret=sh.io.exitStatus; sh.exitq=false;
                if(t2cloneq) UnityEngine.Object.Destroy(t2);
                return ret;
            }
            Color c2;
            if(!float.TryParse(sh.env["_r"],out c2.r)
            ||!float.TryParse(sh.env["_g"],out c2.g)
            ||!float.TryParse(sh.env["_b"],out c2.b)
            ||!float.TryParse(sh.env["_a"],out c2.a)){
                if(t2cloneq) UnityEngine.Object.Destroy(t2);
                return sh.io.Error("色の指定が不正です");
            }
            c2.r=Mathf.Clamp01(c2.r);
            c2.g=Mathf.Clamp01(c2.g);
            c2.b=Mathf.Clamp01(c2.b);
            c2.a=Mathf.Clamp01(c2.a);
            if(!ColorEq(c2,ca[i])){ ca[i]=c2; changed=true; }
        }else for(int dy=0; dy<r.h; dy++) for(int dx=0; dx<r.w; dx++){
            int x=r.x+dx, y=r.y+dy;
            int i=dy*r.w+dx;
            if(sm.ApplyTexFilter(x,y)==0) continue;
            float h,s,v,a=ca[i].a;
            Color.RGBToHSV(ca[i],out h,out s,out v);
            sh.env["_1"]=sh.fmt.FVal((float)x/width);
            sh.env["_2"]=sh.fmt.FVal((float)y/height);
            sh.env["_3"]=width.ToString();
            sh.env["_4"]=height.ToString();
            sh.env["_5"]=x.ToString();
            sh.env["_6"]=y.ToString();
            sh.env["_h"]=sh.fmt.F0to1(h);
            sh.env["_s"]=sh.fmt.F0to1(s);
            sh.env["_v"]=sh.fmt.F0to1(v);
            sh.env["_a"]=sh.fmt.F0to1(a);
            psr.Reset();
            ret=sh.InterpretParser(psr);
            if(ret<0 || sh.exitq){
                ret=sh.io.exitStatus; sh.exitq=false;
                if(t2cloneq) UnityEngine.Object.Destroy(t2);
                return ret;
            }
            if(!float.TryParse(sh.env["_h"],out h)
            ||!float.TryParse(sh.env["_s"],out s)
            ||!float.TryParse(sh.env["_v"],out v)
            ||!float.TryParse(sh.env["_a"],out a)){
                if(t2cloneq) UnityEngine.Object.Destroy(t2);
                return sh.io.Error("色の指定が不正です");
            }
            h=Mathf.Clamp01(h);
            s=Mathf.Clamp01(s);
            v=Mathf.Clamp01(v);
            a=Mathf.Clamp01(a);
            Color c2=Color.HSVToRGB(h,s,v);
            c2.a=a;
            if(!ColorEq(c2,ca[i])){ ca[i]=c2; changed=true; }
        }
        if(changed){
            t2.SetPixels(r.x,r.y,r.w,r.h,ca);
            t2.Apply();
            if(t2cloneq){
                texiid.Add(t2);
                var old=mate.GetTexture(prop);
                if(old!=null && old!=origtex && texiid.Remove(old)) UnityEngine.Object.Destroy(old);
                mate.SetTexture(prop,t2);
            }
        }
        return ret;
    }
    private static bool ColorEq(Color c1,Color c2){
        float t=c1.r-c2.r;
        if(t<-0.003||0.003<t) return false; // 1/256=0.0039
        t=c1.g-c2.g;
        if(t<-0.003||0.003<t) return false;
        t=c1.b-c2.b;
        if(t<-0.003||0.003<t) return false;
        t=c1.a-c2.a;
        if(t<-0.003||0.003<t) return false;
        return true;
    }
    private static int MeshParamTexColorMatrix(ComShInterpreter sh,SingleMesh sm,string val){
        return MeshParamTexColorMatrixSub(sh,sm,val,false);
    }
    private static int MeshParamTexColorMatrixHsv(ComShInterpreter sh,SingleMesh sm,string val){
        return MeshParamTexColorMatrixSub(sh,sm,val,true);
    }
    private static int MeshParamTexColorMatrixSub(ComShInterpreter sh,SingleMesh sm,string val,bool hsvq){
        if(val==null || val=="") return 0;

        string prop,mat;
        if(shaderpropptn.IsMatch(val)){
            string[] lr=ParseUtil.LeftAndRight(val,':');
            prop=lr[0]; mat=lr[1];
        }else{
            prop="_MainTex"; mat=val;
        }
        float[] fa=ParseUtil.FloatArr(mat);
        if(fa==null||fa.Length!=20) return sh.io.Error("カラーマトリクスが不正です");

        sm.mi.EditMaterial();
        Material mate=sm.mi.material[sm.submeshno];
        Material mate0=sm.mi.oid.originalMate[sm.submeshno];
        if(!mate.HasProperty(prop)) return sh.io.Error("プロパティが無効です");
        Texture tx=mate.GetTexture(prop);
        if(tx==null) return sh.io.Error("テクスチャがありません");

        return ApplyColorMatrix(sh,sm,fa,mate,prop,tx,mate0,hsvq);
    }
    private static int ApplyColorMatrix(ComShInterpreter sh,SingleMesh sm,float[] fa,Material mate,string prop,Texture tx,Material orig,bool hsvq){
        int ret=0;
        var origtex=orig.GetTexture(prop);
        Texture2D t2;
        bool t2cloneq=false;
        if(origtex==tx || !ReferenceEquals(tx.GetType(),typeof(Texture2D))){
            t2=TextureUtil.CloneTexture(tx);
            t2cloneq=true;
        }else t2=(Texture2D)tx;
        int width=t2.width,height=t2.height;
        var rec=sm.GetTexXYRange(width,height);
        Color[] ca=t2.GetPixels(rec.x,rec.y,rec.w,rec.h);
        bool changed=false;
        if(hsvq){
            for(int dy=0; dy<rec.h; dy++) for(int dx=0; dx<rec.w; dx++){
                int x=rec.x+dx,y=rec.y+dy;
                int i=dy*rec.w+dx;
                if(sm.ApplyTexFilter(x,y)==0) continue;
                Color.RGBToHSV(ca[i],out float h,out float s,out float v);
                float a=ca[i].a;
                float h2,s2,v2,a2;
                h2=h*fa[ 0]+s*fa[ 1]+v*fa[ 2]+a*fa[ 3]+fa[4];
                s2=h*fa[ 5]+s*fa[ 6]+v*fa[ 7]+a*fa[ 8]+fa[9];
                v2=h*fa[10]+s*fa[11]+v*fa[12]+a*fa[13]+fa[14];
                a2=h*fa[15]+s*fa[16]+v*fa[17]+a*fa[18]+fa[19];
                ca[i]=Color.HSVToRGB(h2,s2,v2);
                ca[i].a=a2;
                changed=true;
            }
        }else{
            for(int dy=0; dy<rec.h; dy++) for(int dx=0; dx<rec.w; dx++){
                int x=rec.x+dx,y=rec.y+dy;
                int i=dy*rec.w+dx;
                if(sm.ApplyTexFilter(x,y)==0) continue;
                var c2=new Color();
                float r=ca[i].r,g=ca[i].g,b=ca[i].b,a=ca[i].a;
                c2.r=r*fa[ 0]+g*fa[ 1]+b*fa[ 2]+a*fa[ 3]+fa[4];
                c2.g=r*fa[ 5]+g*fa[ 6]+b*fa[ 7]+a*fa[ 8]+fa[9];
                c2.b=r*fa[10]+g*fa[11]+b*fa[12]+a*fa[13]+fa[14];
                c2.a=r*fa[15]+g*fa[16]+b*fa[17]+a*fa[18]+fa[19];
                ca[i]=c2;
                changed=true;
            }
        }
        if(changed){
            t2.SetPixels(rec.x,rec.y,rec.w,rec.h,ca);
            t2.Apply();
            if(t2cloneq){
                texiid.Add(t2);
                var old=mate.GetTexture(prop);
                if(old!=null && old!=origtex && texiid.Remove(old)) UnityEngine.Object.Destroy(old);
                mate.SetTexture(prop,t2);
            }
        }
        return ret;
    }
    private static int MeshParamMipmap(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null || val==""){return 0;}
        sm.mi.EditMaterial();
        Material mate0=sm.mi.oid.originalMate[sm.submeshno];
        Material mate=sm.mi.material[sm.submeshno];
        Texture tx;
        float mmb;
        if(val=="off") mmb=float.NaN;
        else if (!float.TryParse(val,out mmb)) return sh.io.Error("値が不正です");

        int mipmap(string prop){
            if(!mate0.HasProperty(prop)) return -1;
            tx=mate0.GetTexture(prop);
            if(tx!=null && ReferenceEquals(tx.GetType(),typeof(Texture2D))){
                Texture2D t=(Texture2D)tx;
                Texture2D t2;
                if(float.IsNaN(mmb)){
                    t2=t;
                }else{
                    int alv=(t.anisoLevel==0)?1:t.anisoLevel;
                    FilterMode fm=(t.filterMode==FilterMode.Point)?FilterMode.Bilinear:t.filterMode; 
                    t2=TextureUtil.CloneTexture(t,alv,mmb,fm,true);
                    t2.name=t.name;
                    texiid.Add(t2);
                }
                var old=mate.GetTexture(prop);
                var origtex=mate0.GetTexture(prop);
                if(old!=null && old!=origtex && texiid.Remove(old)) UnityEngine.Object.Destroy(old);
                mate.SetTexture(prop,t2);
            }
            return 0;
        }

        mipmap("_MainTex");
        mipmap("_ShadowTex");
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
    private static int MeshParamUVWH(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null) return 0;
        string ts,prop;
        var sa=ParseUtil.LeftAndRight(val,':');
        if(sa[1]==""){
            if(sa[0].IndexOf(',')>=0){ts=sa[0]; prop="_MainTex";}
            else{
                var m=sm.mi.material[sm.submeshno];
                var xy=m.GetTextureOffset(sa[0]);
                var wh=m.GetTextureScale(sa[0]);
                sh.io.Print($"{sh.fmt.FXY(xy)},{sh.fmt.FXY(wh)}");
                return 0;
            }
        }else{ts=sa[1]; prop=sa[0];}
        float[] xywh={0,0,0,0};
        int n=ParseUtil.XyzSub(ts,xywh);
        if(n<0||(n!=2&&n!=4)) return sh.io.Error("書式が不正です");
        sm.mi.EditMaterial();
        var mate=sm.mi.material[sm.submeshno];
        mate.SetTextureOffset(prop,new Vector2(xywh[0],xywh[1]));
        if(n==2) return 1;
        mate.SetTextureScale(prop,new Vector2(xywh[2],xywh[3]));
        return 1;
    }
    private static int MeshParamSS(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null) return 0;
        string fname,prop;
        var sa=ParseUtil.LeftAndRight(val,':');
        if(sa[1]==""){ fname=sa[0]; prop="_MainTex";} else { fname=sa[1]; prop=sa[0];}
        if(fname=="" || fname.IndexOf('\\')>=0 || UTIL.CheckFileName(fname)<0) return sh.io.Error("ファイル名が不正です");
        fname=ComShInterpreter.homeDir+@"ScreenShot\\"+UTIL.Suffix(fname,".png");
        int ret=MeshParamPNGSub(sh,sm,fname,prop);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        return ret;
    }
    private static int MeshParamPNG(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null) return 0;
        string fname,prop;
        var sa=ParseUtil.LeftAndRight(val,':');
        if(sa[1]==""){ fname=sa[0]; prop="_MainTex";} else { fname=sa[1]; prop=sa[0];}

        string file="";
        if(fname!=""&&fname.IndexOf('\\')<0){
            if(fname[0]=='*'){
                var tf=DataFiles.CreateTempFile(fname.Substring(1),"");
                file=tf.filename;
            }else file=UTIL.GetFullPath(UTIL.Suffix(fname,".png"),ComShInterpreter.textureDir);
        }
        if(file=="") return sh.io.Error("ファイル名が不正です");
        int ret=MeshParamPNGSub(sh,sm,file,prop);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        return ret;
    }
    private static int MeshParamPNGSub(ComShInterpreter sh,SingleMesh sm,string fname,string prop){
        var tex=sm.mi.material[sm.submeshno].GetTexture(prop);
        if(tex==null) return sh.io.Error("テクスチャがありません");
        return MeshParamPNGSub2(sh,tex,fname);
    }
    public static int MeshParamPNGSub2(ComShInterpreter sh,Texture tex,string fname){
        Texture2D t2d=null;
        if(ReferenceEquals(tex.GetType(),typeof(RenderTexture))){
            RenderTexture rt=(RenderTexture)tex;
            if(rt.dimension==UnityEngine.Rendering.TextureDimension.Cube){
                // TODO代わりに書いただけ。Rt2Cube()は機能しない
                t2d=TextureUtil.CubeToT2D(TextureUtil.Rt2Cube(rt));
            }else{
                if(TextureUtil.Rt2Png(rt,fname)<0) return sh.io.Error("書き込みに失敗しました");
            }
            return 1;
        }else if(ReferenceEquals(tex.GetType(),typeof(Texture2D))){
            t2d=TextureUtil.CloneBitmap(tex);
        }else if(ReferenceEquals(tex.GetType(),typeof(Cubemap))){
            t2d=TextureUtil.CubeToT2D(TextureUtil.CloneCubemap((Cubemap)tex));
        }else return sh.io.Error("未対応の形式です");
        if(t2d==null) return sh.io.Error("失敗しました");

        try{ System.IO.File.WriteAllBytes(fname,t2d.EncodeToPNG());}
        catch{ return sh.io.Error("書き込みに失敗しました"); }
        finally{ if(t2d!=null) UnityEngine.Object.Destroy(t2d); }
        return 1;
    }
    private static int MeshParamReverse(ComShInterpreter sh,SingleMesh sm,string val){
        sm.mi.EditMesh();
        var tri=sm.mi.GetIndices3(sm.submeshno);
        int[] tri2=new int[tri.Length];
        HashSet<int> hs=new HashSet<int>();
        for(int ti=0; ti<tri.Length; ti+=3){
            tri2[ti]=tri[ti+2];tri2[ti+1]=tri[ti+1];tri2[ti+2]=tri[ti];
            hs.Add(tri2[ti]); hs.Add(tri2[ti+1]); hs.Add(tri2[ti+2]);
        }
        sm.mi.mesh.SetIndices(tri2,MeshTopology.Triangles,sm.submeshno);
        var mesh=sm.mi.mesh.FindMesh(sm.submeshno);
        var normals=mesh.normals;
        if(normals.Length>0) foreach(int i in hs) normals[i]=-normals[i];
        mesh.normals=normals;
        return 0;
    }
    private static int MeshParamGraft(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null) return sh.io.Error("オブジェクト名を指定してください");
        bool clonetexq=false;
        string[] lr=ParseUtil.LeftAndRight(val,',');
        if(lr[1]!=""){
            if(lr[1]!="1"&&lr[1]!="0") return sh.io.Error("0か1を指定してください");
            clonetexq=lr[1]=="1";
        }
        if(!UTIL.ValidName(lr[0])) return sh.io.Error("その名前は使用できません");
        Transform pftr=ObjUtil.GetPhotoPrefabTr(sh);
        if(pftr==null) return sh.io.Error("オブジェクト作成に失敗しました");
        if(ObjUtil.FindObj(sh,lr[0])!=null||LightUtil.FindLight(sh,lr[0])!=null) return sh.io.Error("その名前は既に使われています");

        int midx=sm.mi.mesh.FindMeshIdx(sm.submeshno);
        Transform rendtr=sm.mi.mesh[midx].rend.transform;
        GameObject go;
        go=new GameObject(lr[0]);
        go.transform.SetParent(pftr);
        go.transform.position=rendtr.position;
        go.transform.rotation=rendtr.rotation;
        go.transform.localScale=rendtr.localScale;

        var mf=go.AddComponent<MeshFilter>();
        var mr=go.AddComponent<MeshRenderer>();
        mf.sharedMesh=GraftMesh(sm,midx);
        var mate=UnityEngine.Object.Instantiate(sm.mi.material[sm.submeshno]);
        if(clonetexq) TextureUtil.CloneAllTexture(mate);
        mr.sharedMaterial=mate;

        var oi=ObjInfo.AddObjInfo(go.transform,"");
        ObjUtil.objDic[go.transform.name]=go.transform;
        oi.data.Backup();
        oi.data.OwnMesh();
        return 1;
    }
    private static Mesh GraftMesh(SingleMesh sm,int midx){
        Mesh mesh=sm.mi.mesh[midx].mesh;
        Mesh ret=UnityEngine.Object.Instantiate(mesh);
        var va=mesh.vertices;
        var tri=sm.mi.GetIndices3(sm.submeshno);

        var hs=new HashSet<int>();
        for(int i=0; i<tri.Length; i++) hs.Add(tri[i]);
        if(sm.list!=null){
            var hs2=new HashSet<int>();
            for(int i=0; i<sm.list.Length; i++) hs2.Add(sm.list[i]);
            hs.IntersectWith(hs2);
        }
        int[] idx=new int[hs.Count];
        hs.CopyTo(idx);
        Array.Sort(idx);

        if(sm.filter!=null||sm.exclude!=null){
            var idx2=new List<int>();
            for(int i=0; i<idx.Length; i++){
                int th=idx[i];
                if(th>=va.Length) continue;
                if(sm.ApplyFilter(va[th])==1) idx2.Add(th);
            }
            idx=idx2.ToArray();
        }
        ret.Clear();
        ret.vertices=IdxArrCp(va,idx);
        ret.normals=IdxArrCp(mesh.normals,idx);
        ret.tangents=IdxArrCp(mesh.tangents,idx);
        ret.uv=IdxArrCp(mesh.uv,idx);
        ret.uv2=IdxArrCp(mesh.uv2,idx);
        ret.uv3=IdxArrCp(mesh.uv3,idx);
        ret.uv4=IdxArrCp(mesh.uv4,idx);
        List<int> tri2=new List<int>();
        for(int i=0; i<tri.Length; i+=3){
            int a=Array.BinarySearch(idx,tri[i]),b=Array.BinarySearch(idx,tri[i+1]),c=Array.BinarySearch(idx,tri[i+2]);
            if(a>=0&&b>=0&&c>=0){tri2.Add(a); tri2.Add(b); tri2.Add(c);}
        }
        ret.triangles=tri2.ToArray();

        return ret;
    }
    private static T[] IdxArrCp<T>(T[] arr,int[] idx){
        if(arr==null||arr.Length==0) return arr;
        int l=Math.Min(arr.Length,idx.Length);
        T[] rn=new T[l];
        for(int i=0; i<l; i++) rn[i]=arr[idx[i]];
        return rn;
    }

    private static int MeshParamWeightVerno(ComShInterpreter sh,SingleMesh sm,string val){
        if(val==null) return 0;
        var sa=ParseUtil.LeftAndRight(val,',');
        
        float[] mm;
        if(sa[1]=="") mm=new float[]{0,1}; else{
            mm=ParseUtil.MinMax(sa[1]);
            if(mm==null||mm.Length!=2||mm[0]<0||mm[1]>1) return sh.io.Error("範囲が不正です");
        }
        var tr=sm.mi.oid.FindBone(sa[0]);
        if(tr==null){
            var maid=sm.mi.transform.GetComponentInParent<Maid>();
            if(maid!=null) tr=sm.mi.oid.FindBone(ParseUtil.CompleteBoneName(sa[0],maid.boMAN));
            if(tr==null) return sh.io.Error("ボーンが見つかりません");
        }

        int midx=sm.mi.mesh.FindMeshIdx(sm.submeshno);
        if(midx<0 || sm.mi.mesh[midx].rend.GetType()!=typeof(SkinnedMeshRenderer))
            return sh.io.Error("SkinnedMeshRendererがありません");

        SkinnedMeshRenderer smr=(SkinnedMeshRenderer)sm.mi.mesh[midx].rend;
        int boneidx=Array.IndexOf(smr.bones,tr);
        var ia=sm.mi.GetIndices3(sm.submeshno);
        var wt=smr.sharedMesh.boneWeights;
        var ret=new List<int>();
        for(int i=0; i<ia.Length; i++){
            int idx=ia[i];
            if( (wt[idx].boneIndex0==boneidx && wt[idx].weight0>=mm[0] && wt[idx].weight0<=mm[1])
             || (wt[idx].boneIndex1==boneidx && wt[idx].weight1>=mm[0] && wt[idx].weight1<=mm[1])
             || (wt[idx].boneIndex2==boneidx && wt[idx].weight2>=mm[0] && wt[idx].weight2<=mm[1])
             || (wt[idx].boneIndex3==boneidx && wt[idx].weight3>=mm[0] && wt[idx].weight3<=mm[1])){
                ret.Add(idx);
            }
        }
        if(ret.Count>0){
            sh.io.Print(ret[0].ToString());
            for(int i=1; i<ret.Count; i++) sh.io.Print(',').Print(ret[i].ToString());
        }
        return 0;
    }

    private static int MeshParamVideo(ComShInterpreter sh,SingleMesh sm,string val){
        int midx=sm.mi.mesh.FindMeshIdx(sm.submeshno);
        if(midx<0) return sh.io.Error("メッシュ番号が不正です");
        Renderer rend=sm.mi.mesh[midx].rend;
        if(rend==null) return sh.io.Error("レンダラがありません");
        if(val==null){
            sh.io.Print( Video.CurrentVideoName(rend.transform));
            return 0;
        }
        string prop="_MainTex";
        string[] lr=ParseUtil.LeftAndRight2(val,':');
        if(lr!=null && lr[0]!="") prop=lr[0];
        lr=ParseUtil.LeftAndRight(lr[1],',');
        if(lr==null) return sh.io.Error("ファイルが見つかりません");
        int w=1024,h=1024;
        int loopq=0;
        if(lr[1]!=""){
            float[] fa=ParseUtil.FloatArr(lr[1]);
            if(fa==null||fa.Length<1||fa.Length>3) return sh.io.Error("書式が不正です");
            w=(int)fa[0]; h=(int)((fa.Length==1)?fa[0]:fa[1]);
            if(fa.Length==3){
                loopq=(int)fa[2];
                if(loopq!=0&&loopq!=1) return sh.io.Error("数値が不正です");
            }
        }
        if(lr[0]==""){ Video.KillVideo(rend.transform); return 1; }

        string fname=Video.VideoFileCheck(lr[0]);
        if(fname==null) return sh.io.Error("ファイルが見つかりません");

        Transform rendtr=rend.transform;
        sm.mi.EditMaterial();
        Material mate=sm.mi.material[sm.submeshno];
        if(mate==null) return sh.io.Error("マテリアルがありません");

        if(Video.VideoLoad(rendtr,mate,prop,fname,w,h,loopq)<0) return sh.io.Error("失敗しました");
        return 1;
    }

    private static int MeshParamVideoLoop(ComShInterpreter sh,SingleMesh sm,string val){
        int midx=sm.mi.mesh.FindMeshIdx(sm.submeshno);
        Renderer rend=sm.mi.mesh[midx].rend;
        return Video.VideoLoop(sh,rend.transform,val);
    }
    private static int MeshParamVideoPause(ComShInterpreter sh,SingleMesh sm,string val){
        int midx=sm.mi.mesh.FindMeshIdx(sm.submeshno);
        Renderer rend=sm.mi.mesh[midx].rend;
        return Video.VideoPause(sh,rend.transform,val);
    }
    private static int MeshParamVideoMute(ComShInterpreter sh,SingleMesh sm,string val){
        int midx=sm.mi.mesh.FindMeshIdx(sm.submeshno);
        Renderer rend=sm.mi.mesh[midx].rend;
        return Video.VideoMute(sh,rend.transform,val);
    }
    private static int MeshParamAudio3D(ComShInterpreter sh,SingleMesh sm,string val){
        int midx=sm.mi.mesh.FindMeshIdx(sm.submeshno);
        Renderer rend=sm.mi.mesh[midx].rend;
        return Video.VideoAudio3D(sh,rend.transform,val);
    }
    private static int MeshParamVideoTime(ComShInterpreter sh,SingleMesh sm,string val){
        int midx=sm.mi.mesh.FindMeshIdx(sm.submeshno);
        Renderer rend=sm.mi.mesh[midx].rend;
        return Video.VideoTime(sh,rend.transform,val);
    }
    private static int MeshParamVideoTimep(ComShInterpreter sh,SingleMesh sm,string val){
        int midx=sm.mi.mesh.FindMeshIdx(sm.submeshno);
        Renderer rend=sm.mi.mesh[midx].rend;
        return Video.VideoTimep(sh,rend.transform,val);
    }
    private static int MeshParamVideoLength(ComShInterpreter sh,SingleMesh sm,string val){
        int midx=sm.mi.mesh.FindMeshIdx(sm.submeshno);
        Renderer rend=sm.mi.mesh[midx].rend;
        return Video.VideoLength(sh,rend.transform,val);
    }
    private static int MeshParamVideoSpeed(ComShInterpreter sh,SingleMesh sm,string val){
        int midx=sm.mi.mesh.FindMeshIdx(sm.submeshno);
        Renderer rend=sm.mi.mesh[midx].rend;
        return Video.VideoSpeed(sh,rend.transform,val);
    }

    private class VerLoopChange {
        public int idx;
        public string vertex;
        public string normal;
        public string uv;
        public string uv2;
        public string uv3;
        public string uv4;
        public VerLoopChange(int idx){ this.idx=idx; }
    }
    private class SingleMesh {
        public int submeshno;
        public MeshInfo mi;
        public SingleMesh(int no,MeshInfo mi){ this.submeshno=no; this.mi=mi; }
        public float[] filter=null;
        public float[] exclude=null;
        public float[] area=null;
        public float[] areaex=null;
        public int[] list=null;
        public int ApplyFilter(Vector3 vt){
            if(filter!=null){
                if(filter.Length==6){
                    if( filter[0]>vt.x || filter[1]<vt.x 
                     || filter[2]>vt.y || filter[3]<vt.y
                     || filter[4]>vt.z || filter[5]<vt.z ) return 0;
                }else if(filter.Length==4){
                    float x=vt.x-filter[0], y=vt.y-filter[1], z=vt.z-filter[2];
                    if(filter[3] < x*x+y*y+z*z ) return 0;
                }
            }
            if(exclude!=null){
                if(exclude.Length==6){
                    if( exclude[0]<=vt.x && exclude[1]>=vt.x 
                     && exclude[2]<=vt.y && exclude[3]>=vt.y
                     && exclude[4]<=vt.z && exclude[5]>=vt.z) return 0;
                }else if(exclude.Length==4){
                    float x=vt.x-exclude[0], y=vt.y-exclude[1], z=vt.z-exclude[2];
                    if(exclude[3] >= x*x+y*y+z*z ) return 0;
                }
            }
            return 1;
        }
        public struct Rect{ // UnityEngine.Rectでもいいんだけど利用時のcastが面倒なのでint版
            public int x; public int y; public int w; public int h;
            public Rect(int x,int y,int w,int h){this.x=x;this.y=y;this.w=w;this.h=h;}
        }
        public Rect GetTexXYRange(int w,int h){
            if(area!=null) return new Rect((int)area[0],(int)area[1],(int)area[2],(int)area[3]);
            return new Rect(0,0,w,h);
        }
        public int ApplyTexFilter(int x,int y){
            // areaの方はGetTexXYRange()で終わってる
            if(areaex!=null) if( areaex[0]<=x && x<=areaex[0]+areaex[2]-1 && areaex[1]<=y && y<=areaex[1]+areaex[3]-1 ) return 0;
            return 1;
        }
    }
    public class MeshInfo {
        public int count=0;
        public ObjInfo oi=null;
        public ObjInfoData oid=null;
        public List<Material> material;
        public Transform transform;
        public Transform transform0;
        public ObjInfoData.MeshList mesh;
        public MeshInfo(Transform tr){
            transform=tr;
            oi=ObjInfo.GetObjInfo(tr);
            if(oi!=null) transform0=oi.transform;
        }
        public MeshInfo(Transform tr,Transform tr0){
            transform=tr;
            transform0=tr0;
            oi=ObjInfo.GetObjInfo(tr);
            if(oi==null) oid=new ObjInfoData(tr0);
            else if(oi.data==null) oi.data=oid=new ObjInfoData(oi.transform); else oid=oi.data;
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
                oi=ObjInfo.AddObjInfo(transform0,oid,"",null);
            }
            oid.CloneMesh();
            mesh=oid.workMesh;
        }
        public int RestoreMesh(int submeshno){
            if(oid==null || oid.originalMesh==null || oid.workMesh==null) return -1;
            int midx=oid.originalMesh.FindMeshIdx(submeshno);
            if(midx<0) return -1;
            var me=oid.originalMesh[midx];
            var r=me.rend;
            Mesh newMesh;
            if(r.GetType()==typeof(SkinnedMeshRenderer)){
                var smr=(SkinnedMeshRenderer)r;
                newMesh=UnityEngine.Object.Instantiate(me.mesh);
                var oldMesh=smr.sharedMesh;
                smr.sharedMesh=newMesh;
                oid.UpdateMorph(smr.transform,oldMesh,newMesh);
            }else{
                MeshFilter mf=r.GetComponent<MeshFilter>();
                if(mf==null) return -1;
                newMesh=UnityEngine.Object.Instantiate(me.mesh);
                mf.sharedMesh=newMesh;
            }
            var e=oid.workMesh[midx];
            UnityEngine.Object.Destroy(e.mesh);
            e.mesh=newMesh;
            oid.workMesh[midx]=e;
            oid.workMesh.UpdateIndices3(submeshno);
            return 0;
        }
        public void EditMaterial(){
            if(oi==null){
                // 編集すると後始末が必要になるので、ComShで追加したオブジェト以外にもObjInfoを付ける
                oi=ObjInfo.AddObjInfo(transform0,oid,"",null);
            }
            oid.CloneMaterial();
            material=oid.workMate;
        }
        public int RestoreMaterial(int submeshno){
            if(oid==null || oid.originalMesh==null || oid.originalMate==null || oid.workMate==null) return -1;
            int midx=oid.originalMesh.FindMeshIdx(submeshno);
            if(midx<0) return -1;
            var me=oid.originalMesh[midx];
            var r=me.rend;

            Material newMaterial=UnityEngine.Object.Instantiate(oid.originalMate[submeshno]);
            UnityEngine.Object.Destroy(oid.workMate[submeshno]);
            oid.workMate[submeshno]=newMaterial;

            var ma=r.sharedMaterials;
            ma[submeshno-me.no]=newMaterial;
            r.sharedMaterials=ma;

            return 0;
        }
        public int[] GetIndices3(int submeshno){
            if(this.oid.workMesh==null || this.oid.workMesh.indices3==null)
                return this.oid.originalMesh.GetIndices(submeshno);
            return this.oid.workMesh.indices3[submeshno];
        }
    }
}
public static class TextureUtil {
    public static string[] texnames={
        "_AlphaTex", "_BumpMap", "_Caustics", "_DerivativeTex", "_DetailNormalMap", "_EmissionMap",
        "_EnvMap", "_FlowTex", "_Fresnel", "_HiTex", "_LightMap", "_MainTex", "_MultiColTex","_CutoffTex",
        "_NormalMap", "_OcclusionMap", "_MetallicGlossMap","_SpecGlossMap",
        "_OutlineTex", "_OutlineToonRamp", "_OutlineWidthTex",
        "_ParallaxMap", "_ReflectionTex", "_ReflectiveColor", "_ReflectiveColorCube", "_RefractionTex",
        "_RuleTex", "_ShadowRateToon", "_ShadowTex", "_Tex1", "_Tex2", "_ToonRamp"
    };
    public static Texture2D CloneTexture(Texture src){ return CloneTexture(src,src.anisoLevel,src.mipMapBias,src.filterMode); }
    public static Texture2D CloneBitmap(Texture src){ return CloneTexture(src,0,0,FilterMode.Point); }
    public static Texture2D CloneTexture(Texture src,int anisolv,float mmb,FilterMode flt,bool useMipMap=false){ 
        int w=src.width, h=src.height;
        RenderTexture rt=RenderTexture.GetTemporary(w,h,0);
        Graphics.Blit(src,rt);
        var bak=RenderTexture.active;
        RenderTexture.active=rt;
        var ret=new Texture2D(w,h,TextureFormat.RGBA32,useMipMap);
        ret.name=src.name;
        ret.wrapMode=src.wrapMode;
        ret.anisoLevel=anisolv;
        ret.mipMapBias=mmb;
        ret.filterMode=flt;
        ret.ReadPixels(new Rect(0,0,w,h),0,0,false);
        ret.Apply();
        RenderTexture.active=bak;
        RenderTexture.ReleaseTemporary(rt);
        return ret;
    }
    public static int Rt2Png(RenderTexture rt,string fname){
        Texture2D tx=null;
        try{
            int w=rt.width,h=rt.height;
            tx=new Texture2D(w,h,TextureFormat.RGBA32,false);
            tx.wrapMode=rt.wrapMode;
            tx.anisoLevel=0;
            tx.mipMapBias=0;
            tx.filterMode=FilterMode.Point;
            RenderTexture bak=RenderTexture.active;
            RenderTexture.active=rt;
            tx.ReadPixels(new Rect(0,0,w,h),0,0,false);
            tx.Apply();
            RenderTexture.active=bak;
            System.IO.File.WriteAllBytes(fname,tx.EncodeToPNG());
        }catch{
            return -1;
        }finally{
            if(tx!=null) UnityEngine.Object.Destroy(tx);
        }
        return 0;
    }
    public static List<Texture> CloneAllTexture(Material m){
        var ret=new List<Texture>();
        for(int i=0; i<texnames.Length; i++){
            var src=m.GetTexture(texnames[i]);
            if(src==null) continue;
            var dst=CloneTexture(src);
            m.SetTexture(texnames[i],dst);
            ret.Add(dst);
        }
        return ret;
    }
    public static Texture ReadTexture(string name){
        Texture tex0=Resources.Load<Texture>("SceneCreativeRoom/Debug/Textures/"+name);
        if(tex0==null) tex0=Resources.Load<Texture>("Texture/"+name);
        if(tex0!=null) return CloneTexture((Texture2D)tex0);
        try{
            string fname="";
            if(name.Length>0 && name[0]=='*'){
                var tf=DataFiles.GetTempFile(name.Substring(1));
                if(tf!=null) fname=tf.filename;
            }else{
                fname=ComShInterpreter.textureDir+UTIL.Suffix(name,".png");
            }
            if(System.IO.File.Exists(fname)){
                byte[] buf=UTIL.ReadAll(fname);
                Texture2D t2d=new Texture2D(2,2);
                t2d.LoadImage(buf);
                t2d.name=name;
                return t2d;
            }
            fname=UTIL.Suffix(name,".tex");
            if(GameUty.IsExistFile(fname,GameUty.FileSystem)){
                var tere=ImportCM.LoadTexture(GameUty.FileSystem,fname,false);
                var t2d=tere.CreateTexture2D();
                t2d.name=name;
                return t2d;
            }
        }catch{}
        return null;
    }
    public static Cubemap CloneCubemap(Cubemap src){
        int h=src.height;
        var ret=new Cubemap(h,TextureFormat.ARGB32,src.mipmapCount>0);
        ret.name=src.name;
        ret.wrapMode=src.wrapMode;
        ret.anisoLevel=src.anisoLevel;
        ret.filterMode=src.filterMode;
        ret.mipMapBias=src.mipMapBias;
        Graphics.ConvertTexture(src,ret);
        return ret;
    }
    public static Cubemap Rt2Cube(RenderTexture rt){
        int h=rt.height;
        var ret=new Cubemap(h,TextureFormat.ARGB32,false);
        ret.name=rt.name;
        ret.wrapMode=rt.wrapMode;
        ret.anisoLevel=rt.anisoLevel;
        ret.filterMode=rt.filterMode;
        Graphics.ConvertTexture(rt,ret); // ここどうやっても無理なんだが
        return ret;
    }
    public static Cubemap T2DToCube(Texture2D t2d,int mode){
        int d=t2d.height;
        Cubemap cube=new Cubemap(d,t2d.format,false);
        for(int i=0; i<6; i++){
            Color[] ca=t2d.GetPixels(i*d,0,d,d,0);
            if(mode==0) cube.SetPixels(upsidedown(ca,d),(CubemapFace)i,0);
            else{ Array.Reverse(ca); cube.SetPixels(ca,(CubemapFace)i,0); }
        }
        cube.Apply();
        return cube;
    }
    public static Texture2D CubeToT2D(Cubemap cube){
        int d=cube.height;
        Texture2D t2d=new Texture2D(d*6,cube.height,cube.format,false);
        for(int i=0; i<6; i++){
            Color[] ca=cube.GetPixels((CubemapFace)i,0);
            t2d.SetPixels(i*d,0,d,d,upsidedown(ca,d),0);
        }
        t2d.Apply();
        return t2d;
    }
    private static Color[] upsidedown(Color[] ca,int sz) {
        for(int y=0; y<sz/2; y++) for(int x=0; x<sz; x++)
            { Color c=ca[y*sz+x]; ca[y*sz+x]=ca[(sz-1-y)*sz+x]; ca[(sz-1-y)*sz+x]=c; }
        return ca;
    }
}
}
