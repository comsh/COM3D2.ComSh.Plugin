using System.Collections.Generic;
using UnityEngine;
using static System.StringComparison;
using static COM3D2.ComSh.Plugin.Command;

namespace COM3D2.ComSh.Plugin {

public static class CmdLights {
    public static void Init(){
        Command.AddCmd("light",new Cmd(CmdLight));

        lightParamDic.Add("del",new CmdParam<Transform>(LightParamDel));
        lightParamDic.Add("wpos",new CmdParam<Transform>(_CmdParamWPos));
        lightParamDic.Add("wpos.x",new CmdParam<Transform>(_CmdParamWPosX));
        lightParamDic.Add("wpos.y",new CmdParam<Transform>(_CmdParamWPosY));
        lightParamDic.Add("wpos.z",new CmdParam<Transform>(_CmdParamWPosZ));
        lightParamDic.Add("wrot",new CmdParam<Transform>(_CmdParamWRot));
        lightParamDic.Add("wrot.x",new CmdParam<Transform>(_CmdParamWRotX));
        lightParamDic.Add("wrot.y",new CmdParam<Transform>(_CmdParamWRotY));
        lightParamDic.Add("wrot.z",new CmdParam<Transform>(_CmdParamWRotZ));
        lightParamDic.Add("lpos",new CmdParam<Transform>(_CmdParamLPos));
        lightParamDic.Add("lpos.x",new CmdParam<Transform>(_CmdParamLPosX));
        lightParamDic.Add("lpos.y",new CmdParam<Transform>(_CmdParamLPosY));
        lightParamDic.Add("lpos.z",new CmdParam<Transform>(_CmdParamLPosZ));
        lightParamDic.Add("lrot",new CmdParam<Transform>(_CmdParamLRot));
        lightParamDic.Add("lrot.x",new CmdParam<Transform>(_CmdParamLRotX));
        lightParamDic.Add("lrot.y",new CmdParam<Transform>(_CmdParamLRotY));
        lightParamDic.Add("lrot.z",new CmdParam<Transform>(_CmdParamLRotZ));
        lightParamDic.Add("opos",new CmdParam<Transform>(_CmdParamOPos));
        lightParamDic.Add("orot",new CmdParam<Transform>(_CmdParamORot));
        lightParamDic.Add("power",new CmdParam<Transform>(LightParamPower));
        lightParamDic.Add("range",new CmdParam<Transform>(LightParamRange));
        lightParamDic.Add("color",new CmdParam<Transform>(LightParamColor));
        lightParamDic.Add("target",new CmdParam<Transform>(LightParamTarget));
        lightParamDic.Add("angle",new CmdParam<Transform>(LightParamAngle));
        lightParamDic.Add("attach",new CmdParam<Transform>(LightParamAttach));
        lightParamDic.Add("detach",new CmdParam<Transform>(LightParamDetach));
        lightParamDic.Add("handle",new CmdParam<Transform>(LightParamHandle));
        lightParamDic.Add("describe",new CmdParam<Transform>(LightParamDesc));
        lightParamDic.Add("render",new CmdParam<Transform>(LightParamRender));
        lightParamDic.Add("mask",new CmdParam<Transform>(LightParamMask));
        lightParamDic.Add("cookie",new CmdParam<Transform>(LightParamCookie));

        lightParamDic.Add("l2w",new CmdParam<Transform>(_CmdParamL2W));
        lightParamDic.Add("w2l",new CmdParam<Transform>(_CmdParamW2L));

        CmdParamPosRotCp(lightParamDic,"lpos","position");
        CmdParamPosRotCp(lightParamDic,"lpos","pos");
    }
    private static Dictionary<string,CmdParam<Transform>> lightParamDic=new Dictionary<string,CmdParam<Transform>>();

    private static int MainLight(ComShInterpreter sh,List<string> args){
        LightMain lm=GameMain.Instance.MainLight;
        if(args.Count==2){
            sh.io.PrintLn2("color:",sh.fmt.RGB(lm.GetColor()));
            sh.io.PrintLn2("power:",sh.fmt.FInt(lm.GetIntensity()));
            sh.io.PrintLn2("shadow:",sh.fmt.FInt(lm.GetShadowStrength()));
            sh.io.PrintLn2("rotation:",sh.fmt.FEA2(lm.gameObject.transform.rotation.eulerAngles));
            sh.io.PrintLn2("right:",sh.fmt.FPos(lm.gameObject.transform.right));
            sh.io.PrintLn2("up:",sh.fmt.FPos(lm.gameObject.transform.up));
            sh.io.PrintLn2("forward:",sh.fmt.FPos(lm.gameObject.transform.forward));
            return 0;
        }
        int ret,cnt=args.Count;
        int odd=(cnt-2)%2;
        if(odd>0) cnt--;
        for(int i=2; i<cnt; i+=2) if((ret=CmdParamMainLight(sh,lm,args[i],args[i+1]))<=0) return ret;
        if(odd>0) if((ret=CmdParamMainLight(sh,lm,args[cnt],null))<=0) return ret;
        return 0;
    }
    private static int CmdParamMainLight(ComShInterpreter sh,LightMain lm,string cmd,string val){
        if(cmd=="rot"||cmd=="rotation"){
            if(val==null){
                sh.io.PrintLn(sh.fmt.FEA2(lm.gameObject.transform.rotation.eulerAngles));
                return 0;
            }
            float[] rot;
            if((rot=ParseUtil.Xy(val))==null) return sh.io.Error(ParseUtil.error);
            lm.SetRotation(new Vector3(rot[0],rot[1],0));
            return 1;
        }else if(cmd=="power"){
            if(val==null){
                sh.io.PrintLn(sh.fmt.FInt(lm.GetIntensity()));
                return 0;
            }
            if(!float.TryParse(val,out float f)) return sh.io.Error(ParseUtil.error);
            lm.SetIntensity(f);
            return 1;
        }else if(cmd=="color"){
            if(val==null){
                sh.io.PrintLn(sh.fmt.RGB2(lm.GetColor()));
                return 0;
            }
            float[] rgb;
            if((rgb=ParseUtil.Rgb(val))==null) return sh.io.Error(ParseUtil.error);
            lm.SetColor(new Color(rgb[0],rgb[1],rgb[2]));
            return 1;
        }else if(cmd=="shadow"){
            if(val==null){
                sh.io.PrintLn(sh.fmt.FInt(lm.GetShadowStrength()));
                return 0;
            }
            if(!float.TryParse(val,out float f)) return sh.io.Error(ParseUtil.error);
            lm.SetShadowStrength(f);
            return 1;
        }else if(cmd=="shadowtype"){
            Light lt=lm.GetComponent<Light>();
            if(lt==null) return 0;
            if(val==null){
                if(lt.shadows==LightShadows.Hard) sh.io.Print("hard");
                else if(lt.shadows==LightShadows.Soft) sh.io.Print("soft");
                else if(lt.shadows==LightShadows.None) sh.io.Print("none");
                return 0;
            }
            if(val=="h" || val=="hard") lt.shadows=LightShadows.Hard;
            else if(val=="s" || val=="soft") lt.shadows=LightShadows.Soft;
            else if(val=="n" || val=="none") lt.shadows=LightShadows.None;
            else return sh.io.Error("shwdowtypeはhard/soft/noneで指定してください");
            return 1;
        }else if(cmd=="shadowrange"){
            return CmdCamera.ShadowRange(sh,val);
        }else if(cmd=="mask"){
            Light lt=lm.GetComponent<Light>();
            if(lt==null) return 0;
            if(val==null){
                sh.io.Print(lt.cullingMask.ToString("X8"));
                return 0;
            }
            if(!int.TryParse(val,System.Globalization.NumberStyles.HexNumber,null,out int bits))
                return sh.io.Error("数値が不正です");
            lt.cullingMask=bits;
            return 1;
        }else return sh.io.Error("パラメータが不正です");
    }
    private static int EnvLight(ComShInterpreter sh,List<string> args){
        if(args.Count==2){
            string s=AmbMode(RenderSettings.ambientMode);
            if(s==null) return 0;
            sh.io.PrintLn2("mode:",s);
            if(s=="color")
                sh.io.PrintLn2("color:",sh.fmt.RGB(RenderSettings.ambientLight));
            if(s=="gradient"){
                sh.io.PrintLn2("color-up:",sh.fmt.RGB(RenderSettings.ambientSkyColor));
                sh.io.PrintLn2("color-side:",sh.fmt.RGB(RenderSettings.ambientEquatorColor));
                sh.io.PrintLn2("color-down:",sh.fmt.RGB(RenderSettings.ambientGroundColor));
            }
            return 0;
        }
        int ret,cnt=args.Count;
        int odd=(cnt-2)%2;
        if(odd>0) cnt--;
        for(int i=2; i<cnt; i+=2) if((ret=CmdParamEnvLight(sh,args[i],args[i+1]))<=0) return ret;
        if(odd>0) if((ret=CmdParamEnvLight(sh,args[cnt],null))<=0) return ret;
        return 0;
    }
    private static int CmdParamEnvLight(ComShInterpreter sh,string cmd,string val){
        if(cmd=="color"){
            if(val==null){
                sh.io.PrintLn(sh.fmt.RGB(RenderSettings.ambientLight));
                return 0;
            }
            float[] c=ParseUtil.Rgb(val);
            if(c==null) return sh.io.Error(ParseUtil.error);
            RenderSettings.ambientMode=(UnityEngine.Rendering.AmbientMode)AmbMode(cmd);
            RenderSettings.ambientLight=new Color(c[0],c[1],c[2]);
            return 1;
        }else if(cmd=="gradient"){
            if(val==null){
                sh.io.PrintJoinLn(":",
                    sh.fmt.RGB(RenderSettings.ambientSkyColor),
                    sh.fmt.RGB(RenderSettings.ambientEquatorColor),
                    sh.fmt.RGB(RenderSettings.ambientGroundColor)
                );
                string s=AmbMode(RenderSettings.ambientMode);
                if(s!=null) sh.io.PrintLn(s);
                return 0;
            }
            string[] sa=val.Split(ParseUtil.colon);
            if(sa.Length!=3) return sh.io.Error("上・側面・下の３つの色を指定してください");
            float[] cup=ParseUtil.Rgb(sa[0]);
            if(cup==null) return sh.io.Error(ParseUtil.error);
            float[] csd=ParseUtil.Rgb(sa[1]);
            if(csd==null) return sh.io.Error(ParseUtil.error);
            float[] cdn=ParseUtil.Rgb(sa[2]);
            if(cdn==null) return sh.io.Error(ParseUtil.error);
            RenderSettings.ambientMode=(UnityEngine.Rendering.AmbientMode)AmbMode(cmd);
            RenderSettings.ambientSkyColor=new Color(cup[0],cup[1],cup[2]);
            RenderSettings.ambientEquatorColor=new Color(csd[0],csd[1],csd[2]);
            RenderSettings.ambientGroundColor=new Color(cdn[0],cdn[1],cdn[2]);
            return 1;
        }else return sh.io.Error("パラメータが不正です");
    }
    private static string AmbMode(UnityEngine.Rendering.AmbientMode m){
        if(m==UnityEngine.Rendering.AmbientMode.Flat) return "color";
        else if(m==UnityEngine.Rendering.AmbientMode.Skybox) return "skybox";
        else if(m==UnityEngine.Rendering.AmbientMode.Trilight) return "gradient";
        else return null;
    }
    private static int AmbMode(string t){
        if(t=="color") return (int)UnityEngine.Rendering.AmbientMode.Flat;
        else if(t=="skybox") return (int)UnityEngine.Rendering.AmbientMode.Skybox;
        else if(t=="gradient") return (int)UnityEngine.Rendering.AmbientMode.Trilight;
        return -1;
    }

    private static int CmdLight(ComShInterpreter sh,List<string> args){
        Transform pftr;
        if(args.Count==1){
            var lset=new HashSet<string>();
            sh.io.PrintLn($"main directional{sh.ofs}{sh.fmt.FEA2(GameMain.Instance.MainLight.gameObject.transform.rotation.eulerAngles)}");
            sh.io.PrintLn($"env environment{sh.ofs}{AmbMode(RenderSettings.ambientMode)}");
            if(sh.lightRef.Length>0){     // スタジオモード分参照
                pftr=UTIL.GetObjRoot(sh.lightRef);
                if(pftr!=null) for(int i=0; i<pftr.childCount; i++){
                    Transform tr=pftr.GetChild(i);
                    if(tr==null) continue;
                    string type;
                    Light lt=LightUtil.GetLightFromTr(tr);
                    if(lt==null) continue; // 想定外無視
                    if(!lt.enabled) type="disabled"; else type=lighttype(lt.type);
                    lset.Add(tr.name);
                    sh.io.PrintJoinLn(sh.ofs,tr.name,type,sh.fmt.FPos(tr.position));
                }
            }
            if((pftr=LightUtil.GetLightObjectTr(sh))!=null){
                for(int i=0; i<pftr.childCount; i++){
                    Transform tr=pftr.GetChild(i);
                    if(tr==null) continue;
                    string type;
                    Light lt=LightUtil.GetLightFromTr(tr);
                    if(lt==null) continue; // 想定外無視
                    if(!lt.enabled) type="disabled"; else type=lighttype(lt.type);
                    lset.Add(tr.name);
                    sh.io.PrintJoinLn(sh.ofs,tr.name,type,sh.fmt.FPos(tr.position));
                }
            }
            List<string> remove=new List<string>();
            foreach(var kv in LightUtil.lightDic){
                Transform tr=kv.Value;
                if(tr==null){ remove.Add(kv.Key); continue; }
                if(lset.Contains(tr.name)) continue;
                string pname=(tr.parent!=null)?tr.parent.name:"orphan";
                sh.io.PrintJoinLn(sh.ofs,kv.Key,pname,sh.fmt.FPos(tr.position));
            }
            foreach(string k in remove) LightUtil.lightDic.Remove(k);
            return 0;
        }
        if(args[1]=="add"){
            if(args.Count==2) return sh.io.Error(
                 "使い方: light add point [識別名 色 強さ 範囲 光源位置]\n"
                +"    or  light add spot [識別名 色 強さ 範囲 光源位置 ターゲット位置 角度]\n"
                +"    or  light add dir [識別名 色 強さ 光源向き]\n"
            );
            string[] pa;
            if(args[2]=="dir"){
                if(args.Count>10) return sh.io.Error("引数が多すぎます");
                pa=ParseUtil.NormalizeParams(args,new string[]{null,"","ffffff","1","0,0,0"},2);
                if(pa==null) return sh.io.Error("引数が足りません");
                if(args.Count>7) return sh.io.Error("引数が多すぎます");
            }else{
                if(args.Count>10) return sh.io.Error("引数が多すぎます");
                pa=ParseUtil.NormalizeParams(args,new string[]{null,"","ffffff","1","1","0,1,0","0,-1,0","30"},2);
                if(pa==null) return sh.io.Error("引数が足りません");
                if(pa[0]=="point" && args.Count>8) return sh.io.Error("引数が多すぎます");
            }
            if(pa[1]=="") pa[1]=pa[0]+"_"+UTIL.GetSeqId();
            if(!UTIL.ValidName(pa[1])) return sh.io.Error("その名前は使用できません");
            if(ObjUtil.FindObj(sh,pa[1])!=null||LightUtil.FindLight(sh,pa[1])!=null) return sh.io.Error("その名前は既に使われています"); 
            if((pftr=LightUtil.GetLightObjectTr(sh))==null) return sh.io.Error("ライト作成に失敗しました"); // 想定外
            float[] pos,col;
            if((col=ParseUtil.Rgb(pa[2]))==null) return sh.io.Error(ParseUtil.error);
            if(!float.TryParse(pa[3],out float it)) return sh.io.Error("数値の形式が不正です");
            if(pa[0]=="dir"){
                if((pos=ParseUtil.Xyz(pa[4]))==null) return sh.io.Error(ParseUtil.error);
                var go=LightUtil.AddDirectionalLight(pa[1],pftr,new Vector3(pos[0],pos[1],pos[2]),it,new Color(col[0],col[1],col[2]));
                LightUtil.lightDic.Add(pa[1],go.transform);
            }else{
                if(!float.TryParse(pa[4],out float r)) return sh.io.Error("数値の形式が不正です");
                if((pos=ParseUtil.Xyz(pa[5]))==null) return sh.io.Error(ParseUtil.error);
                if(pa[0]=="point"){
                    var go=LightUtil.AddPointLight(pa[1],pftr,new Vector3(pos[0],pos[1],pos[2]),it,r,new Color(col[0],col[1],col[2]));
                    LightUtil.lightDic.Add(pa[1],go.transform);
                }else if(pa[0]=="spot"){
                    float[] dst;
                    if((dst=ParseUtil.Xyz(pa[6]))==null) return sh.io.Error(ParseUtil.error);
                    Vector3 dir=new Vector3(dst[0]-pos[0],dst[1]-pos[1],dst[2]-pos[2]);
                    if(!float.TryParse(pa[7],out float ang)) return sh.io.Error("数値の形式が不正です");
                    var go=LightUtil.AddSpotLight(pa[1],pftr,new Vector3(pos[0],pos[1],pos[2]),dir,r,it,ang,new Color(col[0],col[1],col[2]));
                    LightUtil.lightDic.Add(pa[1],go.transform);
                }else return sh.io.Error("種別にはpointかspotかdirを指定してください");
            }
            return 0;
        }
        if(args[1]=="del" && args.Count>=3){
            if((pftr=LightUtil.GetLightObjectTr(sh))==null) return 0; // これがないなら消す対象もない
            for(int i=2; i<args.Count; i++){
                Transform tr=LightUtil.FindLight(sh,args[i]);
                if(tr!=null){
                    LightUtil.lightDic.Remove(tr.name);
                    UnityEngine.Object.Destroy(tr.gameObject);
                }
            }
            return 0;
        }
        string[] sa=args[1].Split(ParseUtil.colon);
        if(sa.Length>1 && sa[0]=="light") return CmdLightSub(sh,sa[1],args,2);
        return CmdLightSub(sh,args[1],args,2);
    }
    private static string lighttype(LightType type){
        switch(type){
        case LightType.Point: return "point";
        case LightType.Spot: return "spot";
        case LightType.Directional: return "dir";
        default: return "";
        }
    }

    public static int CmdLightSub(ComShInterpreter sh,string id,List<string> args,int prmstart){
        if(id=="main") return MainLight(sh,args);
        else if(id=="env") return EnvLight(sh,args);

        Transform pftr,tr;
        if((pftr=LightUtil.GetLightObjectTr(sh))==null) return sh.io.Error("ライトが存在しません");
        tr=LightUtil.FindLight(sh,id);
        if(tr==null) return sh.io.Error("ライトが存在しません");
        Light lt=LightUtil.GetLightFromTr(tr);
        if(lt==null) return sh.io.Error("失敗しました");

        if(args.Count==prmstart){
            sh.io.PrintLn2("iid:",lt.gameObject.GetInstanceID().ToString());
            string type=lighttype(lt.type);
            sh.io.Print($"type:{type}\n");
            sh.io.PrintLn2("color:",sh.fmt.RGB2(lt.color));
            sh.io.PrintLn2("power:",sh.fmt.FInt(lt.intensity));
            if(lt.type==LightType.Directional){
                sh.io.PrintLn2("direction:",sh.fmt.FPos(lt.transform.rotation.eulerAngles));
            }else sh.io.PrintLn2("range:",sh.fmt.FInt(lt.range));
            if(lt.type==LightType.Spot){
                sh.io.PrintLn2("target:",sh.fmt.FPos(tr.rotation*Vector3.forward + tr.position));
                sh.io.PrintLn2("angle:",sh.fmt.FInt(lt.spotAngle));
            }
            UTIL.PrintTrInfo(sh,tr);
            return 0;
        }
        return ParamLoop(sh,tr,lightParamDic,args,prmstart);
    }

    private static int LightParamDel(ComShInterpreter sh,Transform tr,string val){
        UnityEngine.Object.Destroy(tr.gameObject);
        LightUtil.lightDic.Remove(tr.name);
        return 0;
    }
    private static int LightParamPower(ComShInterpreter sh,Transform tr,string val){
        Light lt=LightUtil.GetLightFromTr(tr);
        if(lt==null) return 0;
        if(val==null){
            sh.io.PrintLn(sh.fmt.FInt(lt.intensity));
            return 0;
        }
        if(!float.TryParse(val,out float it)) return sh.io.Error("数値の形式が不正です");
        lt.intensity=it;
        return 1;
    }
    private static int LightParamRange(ComShInterpreter sh,Transform tr,string val){
        Light lt=LightUtil.GetLightFromTr(tr);
        if(lt==null) return 0;
        if(val==null){
            sh.io.PrintLn(sh.fmt.FInt(lt.range));
            return 0;
        }
        if(!float.TryParse(val,out float r)) return sh.io.Error("数値の形式が不正です");
        lt.range=r;
        return 1;
    }
    private static int LightParamColor(ComShInterpreter sh,Transform tr,string val){
        Light lt=LightUtil.GetLightFromTr(tr);
        if(lt==null) return 0;
        if(val==null){
            sh.io.PrintLn(sh.fmt.RGB2(lt.color));
            return 0;
        }
        float[] rgb;
        if((rgb=ParseUtil.Rgb(val))==null) return sh.io.Error(ParseUtil.error);
        lt.color=new Color(rgb[0],rgb[1],rgb[2]);
        return 1;
    }
    private static int LightParamRender(ComShInterpreter sh,Transform tr,string val){
        Light lt=LightUtil.GetLightFromTr(tr);
        if(lt==null) return 0;
        int i;
        if(val==null){
            for(i=0; i<lrme.Length; i++) if(lrme[i]==lt.renderMode) break;
            if(i<lrme.Length) sh.io.PrintLn(lrms[i]);
            return 0;
        }
        for(i=0; i<lrms.Length; i++) if(lrms[i]==val) break;
        if(i==lrms.Length) return sh.io.Error("renderにはauto|pixel|vertexのいずれかを指定してください");
        lt.renderMode=lrme[i];
        return 1;
    }
    private static LightRenderMode[] lrme={LightRenderMode.Auto,LightRenderMode.ForcePixel,LightRenderMode.ForceVertex};
    private static string[] lrms={"auto","pixel","vertex"};

    private static int LightParamTarget(ComShInterpreter sh,Transform tr,string val){
        Light lt=LightUtil.GetLightFromTr(tr);
        if(lt==null) return 0;
        if(lt.type!=LightType.Spot) return sh.io.Error("targetはスポットライトでのみ有効です");
        if(val==null){
            sh.io.PrintLn(sh.fmt.FPos(tr.rotation*Vector3.forward + tr.position));
            return 0;
        }
        float[] xyz;
        if((xyz=ParseUtil.Xyz(val))==null) return sh.io.Error(ParseUtil.error);
        Vector3 dir=new Vector3(xyz[0]-tr.position.x,xyz[1]-tr.position.y,xyz[2]-tr.position.z);
        tr.rotation=Quaternion.FromToRotation(Vector3.forward,dir);
        return 1;
    }
    private static int LightParamAngle(ComShInterpreter sh,Transform tr,string val){
        Light lt=LightUtil.GetLightFromTr(tr);
        if(lt==null) return 0;
        if(lt.type!=LightType.Spot) return sh.io.Error("angleはスポットライトでのみ有効です");
        if(val==null){
            sh.io.PrintLn(sh.fmt.FInt(lt.spotAngle));
            return 0;
        }
        if(!float.TryParse(val,out float a)) return sh.io.Error("数値の形式が不正です");
        lt.spotAngle=a;
        return 1;
    }
    private static int LightParamAttach(ComShInterpreter sh,Transform tr,string val){
        Transform to;
        int opt,jmpq=0;
        if((opt=val.IndexOf(','))>=0){
            if(!int.TryParse(val.Substring(opt+1),out jmpq)) return sh.io.Error("数値の形式が不正です");
            val=val.Substring(0,opt);
        }
        string[] sa=val.Split(ParseUtil.colon);
        to=ObjUtil.FindObj(sh,sa);
        if(tr==null) return sh.io.Error("対象が見つかりません");
        if(jmpq==2) UTIL.ResetTr(tr);
        tr.SetParent(to,jmpq==0);
        return 1;
    }
    private static int LightParamDetach(ComShInterpreter sh,Transform tr,string val){
        Transform pftr=LightUtil.GetLightObjectTr(sh);
        if(tr.parent!=pftr) tr.SetParent(pftr,true);
        return 1;
    }
    private static int LightParamHandle(ComShInterpreter sh,Transform tr,string val){
        if(val==null)
            return sh.io.Error("off|lpos|wpos||rot|wrot|scale|lposrot|wposrotのいずれかを指定してください");

        var hdl=ComShHandle.GetHandle(tr);
        var lr=ParseUtil.LeftAndRight(val,',');
        var sw=lr[0].ToLower();
        if(sw=="off"){
            if(hdl!=null) ComShHandle.DelHandle(hdl);
            return 1;
        }
        float scale=1;
        if(lr[1]!="" && (!float.TryParse(lr[1],out scale)||scale<0.1)) return sh.io.Error("値が不正です");
        if(hdl==null){ hdl=ComShHandle.AddHandle(tr); hdl.Visible=true;}
        if(ComShHandle.SetHandleType(hdl,sw)<0)
            return sh.io.Error("off|lpos|wpos||rot|wrot|scale|lposrot|wposrotのいずれかを指定してください");
        hdl.offsetScale=scale;
        return 1;
    }
    private static int LightParamDesc(ComShInterpreter sh,Transform tr,string val){
        Light lt=LightUtil.GetLightFromTr(tr);
        if(lt==null) return 0;
        sh.io.PrintJoin(" ",
            "wpos", sh.fmt.FPos(tr.position),
            "wrot", sh.fmt.FEuler(tr.rotation.eulerAngles),
            "power", sh.fmt.FInt(lt.intensity),
            "color", sh.fmt.RGB2(lt.color),
            "range", sh.fmt.FInt(lt.range)
        );
        if(lt.type==LightType.Spot){
            sh.io.PrintJoin(" ",
                " target", sh.fmt.FPos(tr.rotation*Vector3.forward + tr.position),
                "angle:", sh.fmt.FInt(lt.spotAngle)
            );
        }
        return 0;
    }
    private static int LightParamMask(ComShInterpreter sh,Transform tr,string val){
        Light lt=LightUtil.GetLightFromTr(tr);
        if(val==null){
            sh.io.Print(lt.cullingMask.ToString("X8"));
            return 0;
        }
        if(!int.TryParse(val,System.Globalization.NumberStyles.HexNumber,null,out int bits))
            return sh.io.Error("数値が不正です");
        lt.cullingMask=bits;
        return 1;
    }
    private static int LightParamCookie(ComShInterpreter sh,Transform tr,string val){
        Light lt=LightUtil.GetLightFromTr(tr);
        if(val==null){
            if(lt.cookie!=null)
                sh.io.Print($"{lt.cookie.name} {lt.cookie.width}x{lt.cookie.height}");
            return 0;
        }
        Texture tex=TextureUtil.ReadTexture(val);
        tex.wrapMode=TextureWrapMode.Clamp;
        if(tex==null) return sh.io.Error("テクスチャが見つかりません");
        lt.cookie=tex;
        return 1;
    }
}

public static class LightUtil{
    public static Dictionary<string,Transform> lightDic=new Dictionary<string,Transform>();
    public static Transform FindLight(ComShInterpreter sh,string name,Transform parent=null){
        if(name=="") return null;
        if(lightDic.ContainsKey(name)){
            Transform t=lightDic[name];
            if(t==null){lightDic.Remove(name); return null;} else return t;
        }
        if(parent!=null) return parent.Find(name);
        Transform tr=GetLightObjectTr(sh);
        if(tr==null) return null;
        tr=tr.Find(name);
        if(tr!=null) return tr;
        if(sh.lightRef!=""){
            tr=UTIL.GetObjRoot(sh.lightRef);
            if(tr==null) return null;
            return tr.Find(name);
        }
        return null;
    }
    public static Transform GetLightObjectTr(ComShInterpreter sh){
        if(sh.lightBase==string.Empty){
            GameObject bg=GameMain.Instance.BgMgr.BgObject;
            if(bg!=null) return bg.transform;
            return null;
        }else return UTIL.GetObjRoot(sh.lightBase);
    }
    public static GameObject AddSpotLight(string name, Transform pr, Vector3 pos,Vector3 dir,float it,float r,float ag,Color col){
        GameObject go=new GameObject(name);
        go.transform.SetParent(pr);
        go.transform.localPosition=pos;
        go.transform.localRotation=Quaternion.FromToRotation(Vector3.forward,dir);
        Light light=go.AddComponent<Light>();
        light.type=LightType.Spot;
        light.color=col;
        light.intensity=it;
        light.range=r;
        light.spotAngle=ag;
        return go;
    }
    public static GameObject AddPointLight(string name,Transform pr,Vector3 pos,float it,float r,Color col){
    GameObject go=new GameObject(name);
        go.transform.SetParent(pr);
        go.transform.localPosition=pos;
        Light light=go.AddComponent<Light>();
        light.type=LightType.Point;
        light.color=col;
        light.intensity=it;
        light.range=r;
        return go;
    }
    public static GameObject AddDirectionalLight(string name,Transform pr,Vector3 rot,float it,Color col){
    GameObject go=new GameObject(name);
        go.transform.SetParent(pr);
        go.transform.localPosition=Vector3.zero;
        go.transform.localRotation=Quaternion.Euler(rot);
        Light light=go.AddComponent<Light>();
        light.type=LightType.Directional;
        light.color=col;
        light.intensity=it;
        return go;
    }
    public static Light GetLightFromTr(Transform tr){
        Light lt=tr.gameObject.GetComponent<Light>();
        if(lt!=null) return lt;
        for(int i=0; i<tr.childCount; i++){
            Transform chtr=tr.GetChild(i);
            if(chtr.gameObject.activeInHierarchy) return GetLightFromTr(chtr);
        }
        return null;
    }
}

}
