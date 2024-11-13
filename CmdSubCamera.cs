using System.Collections.Generic;
using UnityEngine;
using static COM3D2.ComSh.Plugin.Command;

namespace COM3D2.ComSh.Plugin {

public static class CmdSubCamera {
    public static void Init(){
        Command.AddCmd("subcamera",new Cmd(CmdSubCam));

        subcamParamDic.Add("del",new CmdParam<Camera>(SubCamParamDel));
        subcamParamDic.Add("fov",new CmdParam<Camera>(SubCamParamFov));
        subcamParamDic.Add("aspect",new CmdParam<Camera>(SubCamParamAspect));
        subcamParamDic.Add("w2s",new CmdParam<Camera>(SubCamParamW2S));
        subcamParamDic.Add("s2w",new CmdParam<Camera>(SubCamParamS2W));
        subcamParamDic.Add("screensize",new CmdParam<Camera>(SubCamParamScreenSize));
        subcamParamDic.Add("describe",new CmdParam<Camera>(SubCamParamDesc));
        subcamParamDic.Add("range",new CmdParam<Camera>(SubCamParamRange));
        subcamParamDic.Add("mask",new CmdParam<Camera>(SubCamParamMask));
        subcamParamDic.Add("depth",new CmdParam<Camera>(SubCamParamDepth));
        subcamParamDic.Add("rect",new CmdParam<Camera>(SubCamParamRect));
        subcamParamDic.Add("clrflg",new CmdParam<Camera>(SubCamParamClrFlg));
        subcamParamDic.Add("flare",new CmdParam<Camera>(SubCamParamFlare));
        subcamParamDic.Add("bgcolor",new CmdParam<Camera>(SubCamParamBgColor));
        subcamParamDic.Add("ss",new CmdParam<Camera>(SubCamParamScreenShot));
        subcamParamDic.Add("png",new CmdParam<Camera>(SubCamParamPng));
        subcamParamDic.Add("display",new CmdParam<Camera>(SubCamParamDisplay));
        subcamParamDic.Add("skybox",new CmdParam<Camera>(SubCamParamSkyBox));
        subcamParamDic.Add("skybox.prop",new CmdParam<Camera>(SubCamParamSkyBoxProp));
        subcamParamDic.Add("postprocess",new CmdParam<Camera>(SubCamParamPostProcess));
        subcamParamDic.Add("postprocess.prop",new CmdParam<Camera>(SubCamParamPostProcessProp));
    }
    private static Dictionary<string,CmdParam<Camera>> subcamParamDic=new Dictionary<string,CmdParam<Camera>>();

    public class SubCamera:MonoBehaviour {
        public static List<Camera> camlist=new List<Camera>();
        public Camera camera;
        private void Awake(){
            camera=gameObject.AddComponent<Camera>();
            camlist.Add(camera);
        }
        private void OnDesstroy(){
            RemoveRt();
            camlist.Remove(camera);
        }
        public void RemoveRt(){
            var rt=camera.targetTexture;
            if(rt==null) return;
            if(camlist.Count>0){
                for(int i=0,n=camlist.Count-1; i<=n; i++) if(camlist[i]==null){camlist[i]=camlist[n]; camlist[n--]=null;}
                for(int i=camlist.Count-1; i>=0; i--) if(camlist[i]==null) camlist.RemoveAt(i);
                foreach(var cam in camlist){
                    if(System.Object.ReferenceEquals(camera,cam)) continue;
                    if(!System.Object.ReferenceEquals(rt,cam.targetTexture)) continue;
                    return; // 自分以外が使っているならそのまま
                }
            }
            rt.Release(); rt.DiscardContents(); GameObject.Destroy(rt);
        }
    }

    private static int CmdSubCam(ComShInterpreter sh,List<string> args){
        Transform pftr;
        if(args.Count==1){
            var lst=CmdObjects.GetObjList(sh);
            foreach(var tr in lst) if(tr.GetComponent<Camera>()) sh.io.PrintJoinLn(sh.ofs,tr.name,sh.fmt.FPos(tr.position));
            return 0;
        }
        if(args[1]=="add"){
            if(args.Count!=3 && args.Count!=4) return sh.io.Error( "使い方: subcamera add 識別名 幅,高さ\n　　or　subcamera add 識別名 既存のsubcamera名" );
            pftr=ObjUtil.GetPhotoPrefabTr(sh);
            if(pftr==null) return sh.io.Error("オブジェクト作成に失敗しました"); 
            Camera camera0=GameMain.Instance.MainCamera.camera;
            int w=1024,h=1024;
            Transform share=null;
            if(args.Count==3){}
            else if(args.Count==4){
                share=ObjUtil.FindObj(sh,new ParseUtil.ColonDesc(args[3]));
                if(share!=null){
                    camera0=share.GetComponent<Camera>();
                    if(camera0==null) return sh.io.Error("Cameraがありません");
                }else{
                    float[] xy=ParseUtil.Xy(args[3]);
                    if(xy==null) return sh.io.Error("数値が不正です");
                    if((w=(int)xy[0])<=0 || (h=(int)xy[1])<=0) return sh.io.Error("数値が不正です");
                }
            }else return sh.io.Error("引数が多すぎます");

            string name=args[2];
           if(!UTIL.ValidName(name)) return sh.io.Error("その名前は使用できません");
            if(ObjUtil.FindObj(sh,name)!=null||LightUtil.FindLight(sh,name)!=null) return sh.io.Error("その名前は既に使われています");
            GameObject go=ObjUtil.AddObject(".",name,pftr, Vector3.zero,Vector3.zero,Vector3.one); 
            if(go==null) return sh.io.Error("オブジェクト作成に失敗しました");
            SubCamera subcam=go.AddComponent<SubCamera>();
            Camera cam=subcam.camera;
            cam.CopyFrom(camera0);
            cam.name=name;
            if(share==null){
                RecreateRt(cam,w,h);
            }else{
                if(System.Object.ReferenceEquals(camera0,GameMain.Instance.MainCamera.camera)){
                    cam.transform.position=camera0.transform.position;
                    cam.transform.rotation=camera0.transform.rotation;
                    cam.depth=camera0.depth-1;
                    cam.clearFlags=CameraClearFlags.Depth;
                }else{
                    cam.targetTexture=camera0.targetTexture;
                    cam.clearFlags=CameraClearFlags.Color;
                    cam.backgroundColor=Color.black;
                }
            }
            ObjUtil.objDic[go.transform.name]=go.transform;
            return 0;
        }
        if(args[1]=="del" && args.Count>=3){
            for(int i=2; i<args.Count; i++) if(ObjUtil.DeleteObj<SubCamera>(sh,args[i])<0) return -1;
            return 0;
        }
        return CmdSubCamSub(sh,args[1],args,2);
    }

    public static int CmdSubCamSub(ComShInterpreter sh,string id,List<string> args,int prmstart){
        Transform tr=ObjUtil.FindObj(sh,id);
        if(tr==null) return sh.io.Error("カメラが存在しません");
        Camera cam=tr.GetComponent<Camera>();
        if(cam==null) return sh.io.Error("カメラが存在しません");
        if(args.Count==prmstart){
            sh.io.PrintLn($"screensize:{cam.pixelWidth},{cam.pixelHeight}");
            sh.io.PrintLn($"fov:{cam.fieldOfView}");
            sh.io.Print($"aspect:{cam.aspect}");
            return 0;
        }
        return ParamLoop(sh,cam,subcamParamDic,args,prmstart);
    }
    private static int SubCamParamDel(ComShInterpreter sh,Camera cam,string val){
        ObjUtil.objDic.Remove(cam.gameObject.name);
        UnityEngine.Object.Destroy(cam.transform.gameObject);
        return 0;
    }
    private static int SubCamParamDesc(ComShInterpreter sh,Camera cam,string val){
        sh.io.PrintJoin(" ", // コマンドラインの体裁だからofsではない
            "fov", sh.fmt.FInt(cam.fieldOfView),
            "screensize", $"{cam.pixelWidth} {cam.pixelHeight}",
            "wpos", sh.fmt.FPos(cam.transform.position),
            "wrot", sh.fmt.FEuler(cam.transform.rotation.eulerAngles)
        );
        return 0;
    }
    private static int SubCamParamFov(ComShInterpreter sh,Camera cam,string val){
        if(val==null){
            sh.io.Print(sh.fmt.FInt(cam.fieldOfView));
            return 0;
        }
        if(!float.TryParse(val,out float fov)||fov<=0||fov>=180) return sh.io.Error("視野角は1～179度で指定してください");
        cam.fieldOfView=fov;
        return 1;
    }
    private static int SubCamParamAspect(ComShInterpreter sh,Camera cam,string val){
        if(val==null){
            sh.io.Print(sh.fmt.F0to1(cam.aspect));
            return 0;
        }
        if(!float.TryParse(val,out float f)||f<=0) return sh.io.Error("数値が不正です");
        cam.aspect=f;
        return 1;
    }
    private static int SubCamParamDepth(ComShInterpreter sh,Camera cam,string val){
        if(val==null){
            sh.io.Print(sh.fmt.FInt(cam.depth));
            return 0;
        }
        if(!float.TryParse(val,out float f)) return sh.io.Error("数値が不正です");
        cam.depth=f;
        return 1;
    }
    private static int SubCamParamRect(ComShInterpreter sh,Camera cam,string val){
        if(val==null){
            sh.io.PrintJoin(",",sh.fmt.FVal(cam.rect.x),sh.fmt.FVal(cam.rect.y),
                sh.fmt.FVal(cam.rect.width),sh.fmt.FVal(cam.rect.height));
            return 0;
        }
        float[] fa=ParseUtil.FloatArr(val);
        if(fa==null||fa.Length!=4) return sh.io.Error("数値が不正です");
        cam.rect=new Rect(fa[0],fa[1],fa[2],fa[3]);
        return 1;
    }
    private static int SubCamParamClrFlg(ComShInterpreter sh,Camera cam,string val){
        var clrflg=cam.clearFlags;
        int n;
        if(val==null){
            n=0;
            if(clrflg==CameraClearFlags.Color) n=0;
            else if(clrflg==CameraClearFlags.Depth) n=1;
            else if(clrflg==CameraClearFlags.Skybox) n=2;
            else if(clrflg==CameraClearFlags.Nothing) n=3;
            sh.io.Print(n.ToString());
            return 0;
        }
        if(!int.TryParse(val,out n)||n<0||n>3) return sh.io.Error("数値が不正です");
        if(n==0) cam.clearFlags=CameraClearFlags.Color;
        else if(n==1) cam.clearFlags=CameraClearFlags.Depth;
        else if(n==2) cam.clearFlags=CameraClearFlags.Skybox;
        else if(n==3) cam.clearFlags=CameraClearFlags.Nothing;
        if(cam.targetTexture!=null) RecreateRt(cam,-1,-1);
        return 1;
    }
    public static int SubCamParamBgColor(ComShInterpreter sh,Camera cam,string val){
        if(val==null) return 0;
        var c=ParseUtil.Rgba(val);
        if(c==null) return sh.io.Error(ParseUtil.error);
        cam.backgroundColor=new Color(c[0],c[1],c[2],c[3]);
        return 1;
    }
    private static int SubCamParamW2S(ComShInterpreter sh,Camera cam,string val){
        if(val==null) return 0;
        float[] xyz=ParseUtil.Xyz(val);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        Vector3 xy=cam.WorldToScreenPoint(new Vector3(xyz[0],xyz[1],xyz[2]));
        // このxyは左下を0,0とする系。左上起点になおして表示
        sh.io.PrintLn($"{(long)xy[0]},{(long)Mathf.Round(cam.pixelHeight-1-xy[1])}");
        return 0;
    }
    private static int SubCamParamS2W(ComShInterpreter sh,Camera cam,string val){
        if(val==null) return 0;
        float[] xyz=ParseUtil.Xyz(val);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        Vector3 pos=cam.ScreenToWorldPoint(new Vector3(xyz[0],cam.pixelHeight-1-xyz[1],xyz[2]));
        sh.io.Print(sh.fmt.FPos(pos));
        return 0;
    }
    private static int SubCamParamScreenSize(ComShInterpreter sh,Camera cam,string val){
        if(val==null){
            sh.io.PrintLn($"{cam.pixelWidth},{cam.pixelHeight}");
            return 0;
        }
        if(cam.targetTexture==null) return 1;
        float[] xy=ParseUtil.Xy(val);
        if(xy==null) return sh.io.Error(ParseUtil.error);
        RecreateRt(cam,(int)xy[0],(int)xy[1]);
        return 1;
    }
    private static int RecreateRt(Camera cam,int w,int h){
        int nw=w, nh=h;
        if(nw<0||nh<0){
            RenderTexture tex=cam.targetTexture;
            if(tex!=null){
                nw=tex.width;
                nh=tex.height;
            }
        }
        SubCamera subcam=cam.transform.GetComponent<SubCamera>();
        if(subcam!=null) subcam.RemoveRt();
        var rt=new RenderTexture(nw,nh,32);
        rt.name=cam.name;
        rt.filterMode=FilterMode.Bilinear;
        rt.antiAliasing=QualitySettings.antiAliasing;
        rt.wrapMode=TextureWrapMode.Repeat;
        rt.dimension=UnityEngine.Rendering.TextureDimension.Tex2D;
        cam.targetTexture=rt;
        return 0;
    }
    private static int SubCamParamRange(ComShInterpreter sh,Camera cam,string val){
        if(val==null){
            sh.io.Print(sh.fmt.FXY(cam.nearClipPlane,cam.farClipPlane));
            return 0;
        }
        float f,n;
        var sa=ParseUtil.LeftAndRight(val,',');
        if(sa[1]==""){
            if(!float.TryParse(sa[0],out f)||f<=0) return sh.io.Error("数値が不正です");
            cam.farClipPlane=f;
        }else{
            if( (!float.TryParse(sa[0],out n)||n<=0)
              ||(!float.TryParse(sa[1],out f)||f<=0) ) return sh.io.Error("数値が不正です");
            cam.nearClipPlane=n;
            cam.farClipPlane=f;
        }
        return 1;
    }
    private static int SubCamParamScreenShot(ComShInterpreter sh,Camera cam,string val){
        if(val==null) return 0;
        if(val=="" || val.IndexOf('\\')>=0 || UTIL.CheckFileName(val)<0) return sh.io.Error("ファイル名が不正です");
        string fname=ComShInterpreter.homeDir+@"ScreenShot\\"+UTIL.Suffix(val,".png");
        if(TextureUtil.Rt2Png(cam.targetTexture,fname)<0) return sh.io.Error("書き込みに失敗しました");
        return 1;
    }
    private static int SubCamParamPng(ComShInterpreter sh,Camera cam,string val){
        if(val==null) return 0;

        string file="";
        if(val!="" && val.IndexOf('\\')<0){
            if(val[0]=='*'){
                var tf=DataFiles.CreateTempFile(val.Substring(1),"");
                file=tf.filename;
            }else if(UTIL.CheckFileName(val)>=0){
                file=ComShInterpreter.homeDir+@"PhotoModeData\\Texture\\"+UTIL.Suffix(val,".png");
            }
        }
        if(file=="") return sh.io.Error("ファイル名が不正です");
        if(TextureUtil.Rt2Png(cam.targetTexture,file)<0) return sh.io.Error("書き込みに失敗しました");
        return 1;
    }
    private static int SubCamParamMask(ComShInterpreter sh,Camera cam,string val){
        if(val==null){
            sh.io.Print(cam.cullingMask.ToString("X8"));
            return 0;
        }
        if(!int.TryParse(val,System.Globalization.NumberStyles.HexNumber,null,out int bits))
            return sh.io.Error("数値が不正です");
        cam.cullingMask=bits;
        return 1;
    }
    private static int SubCamParamFlare(ComShInterpreter sh,Camera cam,string val){
        if(val==null){
            sh.io.Print(cam.transform.GetComponent<FlareLayer>()==null?"off":"on");
            return 0;
        }
        if(val=="on") cam.gameObject.GetOrAddComponent<FlareLayer>();
        else if(val=="off") UnityEngine.Object.Destroy(cam.gameObject.GetComponent<FlareLayer>());
        else return sh.io.Error("onかoffを指定してください");
        return 1;
    }
    private static int SubCamParamDisplay(ComShInterpreter sh,Camera cam,string val){
        if(val==null){
            sh.io.Print(cam.targetDisplay.ToString());
            return 0;
        }
        if(!int.TryParse(val,out int n)||n<0||n>=Display.displays.Length) return sh.io.Error("モニタ番号が不正です");
        Display.displays[n].Activate();
        if(cam.targetDisplay==n) return 1;
        var rt=cam.targetTexture;
        if(rt!=null){
            rt.Release(); GameObject.Destroy(rt);
            cam.targetTexture=null;
        }
        cam.targetDisplay=n;
        return 1;
    }
    public static int SubCamParamSkyBox(ComShInterpreter sh,Camera cam,string val){
        var skybox=cam.gameObject.GetComponent<Skybox>();
        if(val==null){
            if(skybox==null || cam.clearFlags!=CameraClearFlags.Skybox || skybox.material==null) return 0;
            sh.io.Print(skybox.material.name);
            return 0;
        }
        if(skybox==null) skybox=cam.gameObject.AddComponent<Skybox>();
        if(skybox==null) return sh.io.Error("失敗しました");

        string[] lr=ParseUtil.LeftAndRight2(val,':');
        if(lr==null) return sh.io.Error("書式が不正です");
        string texab=lr[0];
        string file=lr[1];
        if(file==""){
            if(texab==""){
                UnityEngine.Object.Destroy(skybox);
                Resources.UnloadUnusedAssets();
                return 1;
            }
            List<string> sa=ObjUtil.ListAssetBundle<Cubemap>(texab,ComShInterpreter.textureDir);
            if(sa!=null) foreach(var s in sa) sh.io.PrintLn(s);
            return 0;
        }
        int ret=Command.SetupSkyBoxMate(sh,texab,file,out Material mate);
        if(ret<0) return ret;
        skybox.material=mate;
        return 1;
    }
    public static int SubCamParamSkyBoxProp(ComShInterpreter sh,Camera cam,string val){
        var skybox=cam.gameObject.GetComponent<Skybox>();
        if(skybox==null||skybox.material==null) return sh.io.Error("skyboxは設定されていません");
        if(val==null) return 0;
        string err=CmdMeshes.SetShaderProps(skybox.material,val);
        if(err!="") return sh.io.Error(err);
        return 1;
    }
    public static int SubCamParamPostProcess(ComShInterpreter sh,Camera cam,string val){
        var pea=cam.GetComponents<ComShPostProcess>();
        if(val==null){
            if(pea!=null) for(int i=0; i<pea.Length; i++)
                if(pea[i].mate!=null) sh.io.PrintLn($"{i}{sh.ofs}{pea[i].mate.name}");
            return 0;
        }
        if(val==""){
            if(pea!=null) for(int i=pea.Length-1; i>=0; i--) GameObject.Destroy(pea[i]);
            Resources.UnloadUnusedAssets();
            return 1;
        }
        string ab="postprocess",file=val;
        int idx;
        if((idx=val.IndexOf(':'))>=0){
            ab=val.Substring(0,idx); file=val.Substring(idx+1);
        }
        string[] lr=ParseUtil.LeftAndRight(file,',');
        if(lr==null) return sh.io.Error("書式が不正です");
        file=lr[0]; string mode=lr[1];
        if(file==""){
            if(ab==""){
                if(pea!=null) for(int i=pea.Length-1; i>=0; i--) GameObject.Destroy(pea[i]);
                Resources.UnloadUnusedAssets();
                return 1;
            }
            List<string> sa=ObjUtil.ListAssetBundle<Material>(ab,ComShInterpreter.scriptFolder+@"asset\");
            if(sa!=null) foreach(var s in sa) sh.io.PrintLn(s);
            return 0;
        }
        DepthTextureMode dm=DepthTextureMode.None;
        if(mode!=""){
            if(mode.Length>=1){
                if(mode[0]=='1') dm|=DepthTextureMode.Depth;
                else if(mode[0]!='0') return sh.io.Error("値が不正です");
            }
            if(mode.Length>=2){
                if(mode[1]=='1') dm|=DepthTextureMode.DepthNormals;
                else if(mode[1]!='0') return sh.io.Error("値が不正です");
            }
            if(mode.Length>=3){
                if(mode[2]=='1') dm|=DepthTextureMode.MotionVectors;
                else if(mode[2]!='0') return sh.io.Error("値が不正です");
            }
            if(mode.Length==0||mode.Length>=4) return sh.io.Error("書式が不正です");
        }
        Material mate=ObjUtil.LoadAssetBundle<Material>(ab,file,ComShInterpreter.scriptFolder+@"asset\");
        if(mate==null){
            return sh.io.Error("読み込みに失敗しました");
        }
        ComShPostProcess pe=cam.gameObject.AddComponent<ComShPostProcess>();
        if(pe==null) return sh.io.Error("失敗しました");
        pe.mode=dm;
        pe.mate=mate;
        return 1;
    }
    public static int SubCamParamPostProcessProp(ComShInterpreter sh,Camera cam,string val){
        var pea=cam.gameObject.GetComponents<ComShPostProcess>();
        if(pea==null||pea.Length==0) return sh.io.Error("post processは設定されていません");
        if(val==null) return 0;
        var lr=ParseUtil.LeftAndRight2(val,':');
        if(lr==null) return sh.io.Error("書式が不正です");
        int n=0;
        if(lr[0]!=""){
            if(!int.TryParse(lr[0],out n)||n<0||n>=pea.Length) return sh.io.Error("値が不正です");
        }
        string err=CmdMeshes.SetShaderProps(pea[n].mate,lr[1]);
        if(err!="") return sh.io.Error(err);
        return 1;
    }
    public class ComShPostProcess:MonoBehaviour {
        public Material mate;
        public DepthTextureMode mode=DepthTextureMode.None;
        private DepthTextureMode mode0;
        private void Start(){
            Camera cam=GetComponent<Camera>();
            mode0=cam.depthTextureMode;
            cam.depthTextureMode|=mode;
        }
        private void OnRenderImage(RenderTexture src,RenderTexture dst){
            Graphics.Blit(src,dst,mate);
        }
        private void OnDestroy(){
            Camera cam=GetComponent<Camera>();
            if(cam!=null) cam.depthTextureMode=mode0;
        }
    }
}
}
