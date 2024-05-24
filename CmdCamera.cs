using System;
using System.Collections.Generic;
using UnityEngine;
using static COM3D2.ComSh.Plugin.Command;

namespace COM3D2.ComSh.Plugin {

public static class CmdCamera {
    public static void Init(){
		Command.AddCmd("camera", new Cmd(CmdMainCamera));

        cameraParamDic.Add("target",new CmdParam<CameraMain>(CameraParamTarget));
        cameraParamDic.Add("target.x",new CmdParam<CameraMain>(CameraParamTargetX));
        cameraParamDic.Add("target.y",new CmdParam<CameraMain>(CameraParamTargetY));
        cameraParamDic.Add("target.z",new CmdParam<CameraMain>(CameraParamTargetZ));
        cameraParamDic.Add("fov",new CmdParam<CameraMain>(CameraParamFov));
        cameraParamDic.Add("pos",new CmdParam<CameraMain>(CameraParamPos));
        cameraParamDic.Add("pos.x",new CmdParam<CameraMain>(CameraParamPosX));
        cameraParamDic.Add("pos.y",new CmdParam<CameraMain>(CameraParamPosY));
        cameraParamDic.Add("pos.z",new CmdParam<CameraMain>(CameraParamPosZ));
        cameraParamDic.Add("rot",new CmdParam<CameraMain>(CameraParamRot));
        cameraParamDic.Add("rot.x",new CmdParam<CameraMain>(CameraParamRotX));
        cameraParamDic.Add("rot.y",new CmdParam<CameraMain>(CameraParamRotY));
        cameraParamDic.Add("rot.z",new CmdParam<CameraMain>(CameraParamRotZ));
        cameraParamDic.Add("type",new CmdParam<CameraMain>(CameraParamType));
        cameraParamDic.Add("ui",new CmdParam<CameraMain>(CameraParamUI));
        cameraParamDic.Add("describe",new CmdParam<CameraMain>(CameraParamDesc));
        cameraParamDic.Add("w2s",new CmdParam<CameraMain>(CameraParamW2S));
        cameraParamDic.Add("s2w",new CmdParam<CameraMain>(CameraParamS2W));
        cameraParamDic.Add("screensize",new CmdParam<CameraMain>(CameraParamScreenSize));
        cameraParamDic.Add("speed",new CmdParam<CameraMain>(CameraParamSpeed));
        cameraParamDic.Add("distance",new CmdParam<CameraMain>(CameraParamDistance));
        cameraParamDic.Add("range",new CmdParam<CameraMain>(CameraParamRange));
        cameraParamDic.Add("shadowrange",new CmdParam<CameraMain>(CameraParamShadowRange));
        cameraParamDic.Add("mask",new CmdParam<CameraMain>(CameraParamMask));
        cameraParamDic.Add("depth",new CmdParam<CameraMain>(CameraParamDepth));
        cameraParamDic.Add("clrflg",new CmdParam<CameraMain>(CameraParamClrFlg));
        cameraParamDic.Add("flare",new CmdParam<CameraMain>(CameraParamFlare));

        CmdParamPosRotCp(cameraParamDic,"pos","position");
        CmdParamPosRotCp(cameraParamDic,"rot","rotation");
        CmdParamPosRotCp(cameraParamDic,"pos","wpos");
        CmdParamPosRotCp(cameraParamDic,"rot","wrot");
    }

    private static Dictionary<string,CmdParam<CameraMain>> cameraParamDic=new Dictionary<string,CmdParam<CameraMain>>();

    private const float minDist=0.1f;
    private static int CmdMainCamera(ComShInterpreter sh,List<string> args){
        CameraMain mc=GameMain.Instance.MainCamera;
        if(args.Count==1){
            sh.io.PrintLn2("type:",mc.GetCameraType().ToString());
            if(mc.GetCameraType()==CameraMain.CameraType.Target){
                sh.io.PrintLn2("target:",sh.fmt.FPos(mc.GetTargetPos()));
            }
            sh.io.PrintLn2("fov:",sh.fmt.FInt(mc.camera.fieldOfView));
            UTIL.PrintTrInfo(sh,mc.camera.transform,true);
            return 0;
        }

        if(args[1].IndexOf(',')>=0){
            int ret=CameraParamPos(sh,mc,args[1]);
            if(ret<=0) return ret;
            return ParamLoop(sh,mc,cameraParamDic,args,2);
        }else{
            return ParamLoop(sh,mc,cameraParamDic,args,1);
        }
    }
    private static int CameraParamTarget(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.PrintLn(sh.fmt.FPos(mc.GetTargetPos()));
            return 0;
        }
        ChgCameraType(mc,CameraMain.CameraType.Target);
        float[] xyz=ParseUtil.XyzR(val,out bool relativeq);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        Vector3 target=new Vector3(xyz[0],xyz[1],xyz[2]);
        if(relativeq) target+=mc.GetTargetPos();
        mc.SetTargetPos(target);
        UpdCamera(mc,mc.GetPos(),target);
        return 1;
    }
    private static int CameraParamTargetX(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.PrintLn(sh.fmt.FVal(mc.GetTargetPos().x));
            return 0;
        }
        ChgCameraType(mc,CameraMain.CameraType.Target);
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        var target=mc.GetTargetPos();
        target.x=v;
        mc.SetTargetPos(target);
        UpdCamera(mc,mc.GetPos(),target);
        return 1;
    }
    private static int CameraParamTargetY(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.PrintLn(sh.fmt.FVal(mc.GetTargetPos().y));
            return 0;
        }
        ChgCameraType(mc,CameraMain.CameraType.Target);
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        var target=mc.GetTargetPos();
        target.y=v;
        mc.SetTargetPos(target);
        UpdCamera(mc,mc.GetPos(),target);
        return 1;
    }
    private static int CameraParamTargetZ(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.PrintLn(sh.fmt.FVal(mc.GetTargetPos().z));
            return 0;
        }
        ChgCameraType(mc,CameraMain.CameraType.Target);
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        var target=mc.GetTargetPos();
        target.z=v;
        mc.SetTargetPos(target);
        UpdCamera(mc,mc.GetPos(),target);
        return 1;
    }
    private static int CameraParamFov(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.PrintLn(sh.fmt.FInt(mc.camera.fieldOfView));
            return 0;
        }
        if(!float.TryParse(val,out float fov)||fov<=0||fov>=180) return sh.io.Error("視野角は1～179度で指定してください");
        mc.camera.fieldOfView=fov;
        return 1;
    }
    private static int CameraParamDepth(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.Print(sh.fmt.FInt(mc.camera.depth));
            return 0;
        }
        if(!float.TryParse(val,out float f)) return sh.io.Error("数値が不正です");
        mc.camera.depth=f;
        return 1;
    }
    private static int CameraParamClrFlg(ComShInterpreter sh,CameraMain mc,string val){
        var clrflg=mc.camera.clearFlags;
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
        if(!int.TryParse(val,out n)||n<0||n>2) return sh.io.Error("数値が不正です");
        if(n==0) mc.camera.clearFlags=CameraClearFlags.Color; // 公式
        else if(n==1) mc.camera.clearFlags=CameraClearFlags.Depth; // subcameraより後に描きたいとき
        else if(n==2) mc.camera.clearFlags=CameraClearFlags.Skybox; // 現状では使わない
        else if(n==3) mc.camera.clearFlags=CameraClearFlags.Nothing; // 現状では使わない
        return 1;
    }
    private static int CameraParamFlare(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.Print(mc.camera.transform.GetComponent<FlareLayer>()==null?"off":"on");
            return 0;
        }
        if(val=="on") mc.camera.gameObject.GetOrAddComponent<FlareLayer>();
        else if(val=="off") UnityEngine.Object.Destroy(mc.camera.gameObject.GetComponent<FlareLayer>());
        else return sh.io.Error("onかoffを指定してください");
        return 1;
    }
    private static int CameraParamRot(ComShInterpreter sh,CameraMain mc,string val){
        Transform tr=mc.transform;
        if(val==null){
            sh.io.PrintLn(sh.fmt.FEuler(tr.rotation.eulerAngles));
            return 0;
        }
        ChgCameraType(mc,CameraMain.CameraType.Free);
        float[] xyz=ParseUtil.RotR(val,out byte relative);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        if(relative==1) mc.SetRotation((Quaternion.Euler(xyz[0],xyz[1],xyz[2])*tr.rotation).eulerAngles);
        else if(relative==2) mc.SetRotation((tr.rotation*Quaternion.Euler(xyz[0],xyz[1],xyz[2])).eulerAngles);
        else mc.SetRotation(new Vector3(xyz[0],xyz[1],xyz[2]));
        return 1;
    }
    private static int CameraParamRotX(ComShInterpreter sh,CameraMain mc,string val){
        Transform tr=mc.transform;
        if(val==null){
            sh.io.PrintLn(sh.fmt.FInt(tr.rotation.eulerAngles.x));
            return 0;
        }
        ChgCameraType(mc,CameraMain.CameraType.Free);
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        var r=tr.rotation.eulerAngles;
        r.x=v;
        mc.SetRotation(r);
        return 1;
    }
    private static int CameraParamRotY(ComShInterpreter sh,CameraMain mc,string val){
        Transform tr=mc.transform;
        if(val==null){
            sh.io.PrintLn(sh.fmt.FInt(tr.rotation.eulerAngles.y));
            return 0;
        }
        ChgCameraType(mc,CameraMain.CameraType.Free);
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        var r=tr.rotation.eulerAngles;
        r.y=v;
        mc.SetRotation(r);
        return 1;
    }
    private static int CameraParamRotZ(ComShInterpreter sh,CameraMain mc,string val){
        Transform tr=mc.transform;
        if(val==null){
            sh.io.PrintLn(sh.fmt.FInt(tr.rotation.eulerAngles.z));
            return 0;
        }
        ChgCameraType(mc,CameraMain.CameraType.Free);
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        var r=tr.rotation.eulerAngles;
        r.z=v;
        mc.SetRotation(r);
        return 1;
    }
    private static int CameraParamType(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.PrintLn(mc.GetCameraType().ToString());
            return 0;
        }
        if(val=="f"||val=="F") ChgCameraType(mc,CameraMain.CameraType.Free);
        else if(val=="t"||val=="T") ChgCameraType(mc,CameraMain.CameraType.Target);
        else return sh.io.Error("カメラタイプはfかtで指定してください");
        return 1;
    } 
    private static int CameraParamPos(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.PrintLn(sh.fmt.FPos(mc.GetPos()));
            return 0;
        }
        float[] xyz=ParseUtil.XyzR(val,out bool relativeq);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        var p=new Vector3(xyz[0],xyz[1],xyz[2]);
        if(relativeq) p+=mc.GetPos();
        if(mc.GetCameraType()==CameraMain.CameraType.Target)
            UpdCamera(mc,p,mc.GetTargetPos());
        else mc.SetPos(p);
        return 1;
    }
    private static int CameraParamPosX(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.PrintLn(sh.fmt.FVal(mc.GetPos().x));
            return 0;
        }
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        var p=mc.GetPos();
        p.x=v;
        if(mc.GetCameraType()==CameraMain.CameraType.Target)
            UpdCamera(mc,p,mc.GetTargetPos());
        else mc.SetPos(p);
        return 1;
    }
    private static int CameraParamPosY(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.PrintLn(sh.fmt.FVal(mc.GetPos().y));
            return 0;
        }
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        var p=mc.GetPos();
        p.y=v;
        if(mc.GetCameraType()==CameraMain.CameraType.Target)
            UpdCamera(mc,p,mc.GetTargetPos());
        else mc.SetPos(p);
        return 1;
    }
    private static int CameraParamPosZ(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.PrintLn(sh.fmt.FVal(mc.GetPos().z));
            return 0;
        }
        float v=ParseUtil.ParseFloat(val);
        if(float.IsNaN(v)) return sh.io.Error("数値の指定が不正です");
        var p=mc.GetPos();
        p.z=v;
        if(mc.GetCameraType()==CameraMain.CameraType.Target)
            UpdCamera(mc,p,mc.GetTargetPos());
        else mc.SetPos(p);
        return 1;
    }
    private static int CameraParamDesc(ComShInterpreter sh,CameraMain mc,string val){
        if(mc.GetCameraType()==CameraMain.CameraType.Free){
            sh.io.PrintJoin(" ", // コマンドラインの体裁だからofsではない
                "type F",
                "wpos", sh.fmt.FPos(mc.GetPos()),
                "wrot", sh.fmt.FEuler(mc.transform.rotation.eulerAngles)
            );
        }else{
            sh.io.PrintJoin(" ",
                "type T",
                "wpos", sh.fmt.FPos(mc.GetPos()),
                "target", sh.fmt.FPos(mc.GetTargetPos())
            );
        }
        return 0;
    } 
    private static int CameraParamW2S(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null) return 0;
        float[] xyz=ParseUtil.Xyz(val);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        Vector3 xy=mc.camera.WorldToScreenPoint(new Vector3(xyz[0],xyz[1],xyz[2]));
        // このxyは左下を0,0とする系。左上起点になおして表示
        sh.io.PrintLn($"{(long)xy.x},{(long)Mathf.Round(mc.camera.pixelHeight-1-xy.y)}");
        return 0;
    }
    private static int CameraParamS2W(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null) return 0;
        float[] xyz=ParseUtil.Xyz(val);
        if(xyz==null) return sh.io.Error(ParseUtil.error);
        Vector3 pos=mc.camera.ScreenToWorldPoint(new Vector3(xyz[0],mc.camera.pixelHeight-1-xyz[1],xyz[2]));
        sh.io.Print(sh.fmt.FPos(pos));
        return 0;
    }
    private static int CameraParamScreenSize(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.PrintLn($"{mc.camera.pixelWidth},{mc.camera.pixelHeight}");
            return 0;
        }
        float[] xy=ParseUtil.Xy(val);
        if(xy==null) return sh.io.Error(ParseUtil.error);
        Screen.SetResolution((int)xy[0],(int)xy[1],Screen.fullScreen);
        return 1;
    }
    private static int CameraParamSpeed(ComShInterpreter sh,CameraMain mc,string val){
        var uc=mc.GetComponent<UltimateOrbitCamera>();
        if(uc==null) return sh.io.Error("失敗しました");
        if(val==null){
            sh.io.PrintLn($"{sh.fmt.FInt(uc.moveSpeed)}");
            return 0;
        }
        if(!float.TryParse(val,out float f) || f<=0) return sh.io.Error("数値が不正です");
        uc.moveSpeed=f;
        return 1;
    }
    private static int CameraParamDistance(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.Print(sh.fmt.FVal(mc.GetDistance()));
            return 0;
        }
        ChgCameraType(mc,CameraMain.CameraType.Target);
        if(!float.TryParse(val,out float f) || f<=0) return sh.io.Error("数値が不正です");
        mc.SetDistance(f);
        return 1;
    }
    private static int CameraParamRange(ComShInterpreter sh,CameraMain mc,string val){
        Camera cam=mc.camera;
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
            if(mc.m_bCalcNearClip){
                mc.m_bCalcNearClip=false;
                // ↑のフラグ落としてもしばらくは公式コルーチンに上書きされるので↓
                ComShBg.cron.KillJob("cmdcamera/range");
                ComShBg.cron.AddJob("cmdcamera/range",0,(long)(1000*TimeSpan.TicksPerMillisecond),(long t)=>{
                    cam.nearClipPlane=n; return 0;
                },0); // 起動時にやってしまってもよさそうだけど一応初回変更時に
            }
        }
        return 1;
    }
    private static int CameraParamShadowRange(ComShInterpreter sh,CameraMain mc,string val){
        return ShadowRange(sh,val);
    }
    public static int ShadowRange(ComShInterpreter sh,string val){
        if(val==null){
            sh.io.Print(QualitySettings.shadowDistance.ToString());
            return 0;
        }
        if(!float.TryParse(val,out float f)) return sh.io.Error(ParseUtil.error);
        if(f<0) return sh.io.Error("値の範囲が不正です");
        QualitySettings.shadowDistance=f;
        return 1;
    }
    private static int CameraParamMask(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null){
            sh.io.Print(mc.camera.cullingMask.ToString("X8"));
            return 0;
        }
        if(!int.TryParse(val,System.Globalization.NumberStyles.HexNumber,null,out int bits))
            return sh.io.Error("数値が不正です");
        mc.camera.cullingMask=bits;
        return 1;
    }

    private static int CameraParamUI(ComShInterpreter sh,CameraMain mc,string val){
        if(val==null) return 0;
        int onoff=ParseUtil.OnOff(val);
        if(onoff<0) return sh.io.Error(ParseUtil.error);
        if(onoff==0) UIAlpha(0);
        else UIAlpha(1f);
        return 1;
    }
    private static void UIAlpha(float a){
        foreach(UIRoot r in UIRoot.list){
            var c=r.GetComponentInChildren<Camera>();
            if(c==null || !c.enabled || c.GetComponent<UICamera>()==null) continue;
            var p=r.GetComponent<UIPanel>();
            if(p.name.ToLower().IndexOf("fix",System.StringComparison.Ordinal)<0) p.alpha=a;
        }
    }
    private static float lastDistance=2f;
    private static void ChgCameraType(CameraMain mc,CameraMain.CameraType type){
        if(mc.GetCameraType()==type) return;
        if(type==CameraMain.CameraType.Target){ // free -> target
            Vector3 pos=mc.GetPos();
            Vector3 target=pos+mc.transform.forward*lastDistance;
            mc.SetCameraType(type);
            mc.SetTargetPos(target);
            UpdCamera(mc,pos,target);
        }else{                                   // target -> free
            lastDistance=mc.GetDistance();
            mc.SetCameraType(type);
        }
    }
    private static void UpdCamera(CameraMain mc,Vector3 pos,Vector3 target){
        Vector3 t2p=pos-target;
        float len=t2p.magnitude;
        if(len<minDist){      // 近すぎるとき
            mc.SetDistance(minDist);    // 今の向きのまま最小距離に
        }else{
            mc.SetDistance(len);
            mc.SetAroundAngle(XYAngle(t2p,len));
        }
    }
    private static Vector2 XYAngle(Vector3 vec,float len){
        float ry=180f+Mathf.Atan2(vec.x,vec.z)*Mathf.Rad2Deg;
        float rx=Mathf.Asin(Mathf.Clamp(vec.y/len,-1f,1f))*Mathf.Rad2Deg;
        return new Vector2(ry,rx);
    }
}
}
