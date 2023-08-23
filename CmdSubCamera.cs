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
        subcamParamDic.Add("screensize",new CmdParam<Camera>(SubCamParamScreenSize));
        subcamParamDic.Add("describe",new CmdParam<Camera>(SubCamParamDesc));
        subcamParamDic.Add("range",new CmdParam<Camera>(SubCamParamRange));

        subcamParamDic.Add("ss",new CmdParam<Camera>(SubCamParamScreenShot));
        subcamParamDic.Add("png",new CmdParam<Camera>(SubCamParamPng));

    }
    private static Dictionary<string,CmdParam<Camera>> subcamParamDic=new Dictionary<string,CmdParam<Camera>>();

    private static int CmdSubCam(ComShInterpreter sh,List<string> args){
        Transform pftr;
        if(args.Count==1){
            var lst=CmdObjects.GetObjList(sh);
            foreach(var tr in lst) if(tr.GetComponent<Camera>()) sh.io.PrintJoinLn(sh.ofs,tr.name,sh.fmt.FPos(tr.position));
            return 0;
        }
        if(args[1]=="add"){
            if(args.Count!=3 && args.Count!=4) return sh.io.Error( "使い方: subcam add 識別名 幅,高さ" );
            pftr=ObjUtil.GetPhotoPrefabTr(sh,true);
            if(pftr==null) return sh.io.Error("オブジェクト作成に失敗しました"); 
            CameraMain mc=GameMain.Instance.MainCamera;
            int w=1024,h=1024;
            if(args.Count>3){
                float[] xy=ParseUtil.Xy(args[3]);
                if(xy==null) return sh.io.Error("数値の指定が不正です");
                w=(int)xy[0]; h=(int)xy[1];
            }
            string name=args[2];
           if(!UTIL.ValidName(name)) return sh.io.Error("その名前は使用できません");
            if(ObjUtil.FindObj(sh,name)!=null||LightUtil.FindLight(sh,name)!=null) return sh.io.Error("その名前は既に使われています");
            GameObject go=ObjUtil.AddObject(".",name,pftr, Vector3.zero,Vector3.zero,Vector3.one); 
            if(go==null) return sh.io.Error("オブジェクト作成に失敗しました");
            var cam=go.AddComponent<Camera>();
            cam.CopyFrom(mc.camera);
            var rt=new RenderTexture(w,h,32);
            rt.filterMode=FilterMode.Bilinear;
            rt.antiAliasing=QualitySettings.antiAliasing;
            rt.wrapMode=TextureWrapMode.Repeat;
            cam.targetTexture=rt;
            cam.backgroundColor=new Color(0,0,0,0);
            cam.clearFlags=CameraClearFlags.Color;
            cam.transform.position=Vector3.zero;
            cam.transform.rotation=Quaternion.identity;
            ObjUtil.objDic.Add(go.transform.name,go.transform);
            return 0;
        }
        if(args[1]=="del" && args.Count>=3){
            for(int i=2; i<args.Count; i++){
                Transform tr=ObjUtil.FindObj(sh,args[i]);
                if(tr!=null){
                    ObjUtil.objDic.Remove(tr.name);
                    var cam=tr.GetComponent<Camera>();
                    cam.targetTexture.Release();
                    cam.targetTexture.DiscardContents();
                    UnityEngine.Object.Destroy(tr.gameObject);
                }
            }
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
    private static int SubCamParamW2S(ComShInterpreter sh,Camera cam,string val){
        if(val==null) return 0;
        float[] xyz=ParseUtil.Xyz(val);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        Vector3 xy=cam.WorldToScreenPoint(new Vector3(xyz[0],xyz[1],xyz[2]));
        // このxyは左下を0,0とする系。左上起点になおして表示
        sh.io.PrintLn($"{(long)xy[0]},{(long)Mathf.Round(cam.pixelHeight-1-xy[1])}");
        return 0;
    }
    private static int SubCamParamScreenSize(ComShInterpreter sh,Camera cam,string val){
        if(val==null){
            sh.io.PrintLn($"{cam.pixelWidth},{cam.pixelHeight}");
            return 0;
        }
        float[] xy=ParseUtil.Xy(val);
        if(xy==null) return sh.io.Error(ParseUtil.error);
        RenderTexture tex=cam.targetTexture;
        if(tex!=null){
            tex.Release();
            tex.DiscardContents();
            Object.Destroy(tex);
        }
        var rt=new RenderTexture((int)xy[0],(int)xy[1],32);
        rt.filterMode=FilterMode.Bilinear;
        rt.antiAliasing=QualitySettings.antiAliasing;
        cam.targetTexture=rt;
        return 1;
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
}
}
