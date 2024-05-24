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
                share=ObjUtil.FindObj(sh,args[3].Split(ParseUtil.colon));
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
        RecreateRt(cam,-1,-1);
        return 1;
    }
    private static int SubCamParamBgColor(ComShInterpreter sh,Camera cam,string val){
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
}
}
