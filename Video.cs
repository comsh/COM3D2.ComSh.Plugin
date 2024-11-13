using System;
using System.IO;
using UnityEngine;

namespace COM3D2.ComSh.Plugin {

public static class Video {

    public static string VideoFileCheck(string file){
        string path=Path.GetFullPath(ComShInterpreter.textureDir+file);
        if(Path.GetDirectoryName(path).Length<ComShInterpreter.textureDir.Length-1) return null;
        if(!File.Exists(path)) return null;
        return path;
    }

    public static int VideoLoad(Transform tr,Material mate,string prop,string fname,int w,int h,int loopq){
        var vp=tr.GetComponent<UnityEngine.Video.VideoPlayer>();
        if(vp==null) vp=tr.gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();
        if(vp==null) return -1;
        vp.skipOnDrop=vp.playOnAwake=false;

        AudioSource asrc=vp.GetTargetAudioSource(0);
        if(asrc==null) asrc=tr.gameObject.AddComponent<AudioSource>();
        if(asrc==null) return -1;
        var mixer=GameMain.Instance.MainCamera.GetComponentInChildren<AudioMixerMgr>();
        if(mixer==null) return -1;
        asrc.outputAudioMixerGroup=mixer[AudioSourceMgr.Type.VoiceMob];
        asrc.playOnAwake=asrc.mute=
        asrc.bypassEffects=asrc.bypassListenerEffects=asrc.bypassReverbZones=
        asrc.ignoreListenerPause=asrc.ignoreListenerVolume=false;
        asrc.velocityUpdateMode=AudioVelocityUpdateMode.Fixed;
        asrc.spatialBlend=0.5f;

        Texture old=mate.GetTexture(prop);
        RenderTexture rt;
        if(old!=null){
            rt=new RenderTexture(w,h,0,RenderTextureFormat.ARGB32);
            if(CmdMeshes.texiid.Remove(old)) UnityEngine.Object.Destroy(old);
        }else{
            rt=new RenderTexture(w,h,0,RenderTextureFormat.ARGB32);
        }
        CmdMeshes.texiid.Add(rt);
        mate.SetTexture(prop,rt);
        vp.renderMode=UnityEngine.Video.VideoRenderMode.RenderTexture;
        vp.targetTexture=rt;
        vp.isLooping=(loopq==1);
        vp.playbackSpeed=1;
        vp.aspectRatio=UnityEngine.Video.VideoAspectRatio.Stretch;
        vp.audioOutputMode=UnityEngine.Video.VideoAudioOutputMode.AudioSource;
        vp.EnableAudioTrack(0,true);
        vp.SetTargetAudioSource(0,asrc);
        vp.source=UnityEngine.Video.VideoSource.Url;
        vp.url="file://"+fname.Replace('\\','/');
        vp.Prepare();
        vp.prepareCompleted+=Vp_prepareCompleted;
        return 0;
    }
    private static void Vp_prepareCompleted(UnityEngine.Video.VideoPlayer vp){ vp.Play(); }

    public static string CurrentVideoName(Transform tr){
        var vp=tr.GetComponent<UnityEngine.Video.VideoPlayer>();
        if(vp==null) return "";
        return Path.GetFileName(vp.url);
    }

    public static void KillVideo(Transform tr){
        var vp=tr.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
        if(vp==null) return;
        vp.Stop();
        var asrc=vp.GetTargetAudioSource(0);
        if(asrc!=null) GameObject.Destroy(asrc);
        GameObject.Destroy(vp);
    }

    public static int VideoLoop(ComShInterpreter sh,Transform tr,string val){
        var vp=tr.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
        if(vp==null) return sh.io.Error("動画は指定されていません");
        if(val==null){
            sh.io.Print(vp.isLooping?"1":"0");
            return 0;
        }
        if(val!="0"&&val!="1") return sh.io.Error("値が不正です");
        vp.isLooping=(val=="1");
        if(val=="1" && !vp.isPlaying) vp.Play();
        return 1;
    }
    public static int VideoPause(ComShInterpreter sh,Transform tr,string val){
        var vp=tr.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
        if(vp==null) return sh.io.Error("動画は指定されていません");
        if(val==null){
            sh.io.Print(vp.isPlaying?"0":"1");
            return 0;
        }
        if(val!="0"&&val!="1") return sh.io.Error("値が不正です");
        if(val=="1") vp.Pause(); else vp.Play();
        return 1;
    }
    public static int VideoAudio3D(ComShInterpreter sh,Transform tr,string val){
        var vp=tr.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
        if(vp==null) return sh.io.Error("動画は指定されていません");
        var asrc=vp.GetTargetAudioSource(0);
        if(asrc==null) return 0;
        if(val==null){
            sh.io.Print(sh.fmt.FVal(asrc.spatialBlend));
            return 0;
        }
        if(!float.TryParse(val,out float f)||f<0||f>1) return sh.io.Error("数値が不正です");
        asrc.spatialBlend=f;
        return 1;
    }
    public static int VideoMute(ComShInterpreter sh,Transform tr,string val){
        var vp=tr.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
        if(vp==null) return sh.io.Error("動画は指定されていません");
        var asrc=vp.GetTargetAudioSource(0);
        if(asrc==null) return 0;
        if(val==null){
            sh.io.Print(asrc.mute?"1":"0");
            return 0;
        }
        if(val!="0"&&val!="1") return sh.io.Error("値が不正です");
        asrc.mute=(val=="1");
        return 1;
    }
    public static int VideoTime(ComShInterpreter sh,Transform tr,string val){
        var vp=tr.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
        if(vp==null) return sh.io.Error("動画は指定されていません");
        if(val==null){
            sh.io.Print(sh.fmt.FVal(vp.time*1000));
            return 0;
        }
        if(!float.TryParse(val,out float f)) return sh.io.Error("値が不正です");
        vp.time=f/1000;
        return 1;
    }
    public static int VideoTimep(ComShInterpreter sh,Transform tr,string val){
        var vp=tr.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
        if(vp==null) return sh.io.Error("動画は指定されていません");
        if(val==null){
            sh.io.Print(sh.fmt.F0to1((float)vp.frame/vp.frameCount));
            return 0;
        }
        if(!float.TryParse(val,out float f)||f<0||f>1) return sh.io.Error("値が不正です");
        vp.time=CalcVideoLength(vp)*f;
        return 1;
    }
    public static int VideoLength(ComShInterpreter sh,Transform tr,string val){
        var vp=tr.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
        if(vp==null) return sh.io.Error("動画は指定されていません");
        sh.io.Print(CalcVideoLengthMS(vp).ToString());
        return 0;
    }
    private static int CalcVideoLengthMS(UnityEngine.Video.VideoPlayer vp){
        return (int)Mathf.Round(CalcVideoLength(vp)*1000);
    }
    private static float CalcVideoLength(UnityEngine.Video.VideoPlayer vp){
        return vp.frameCount / vp.frameRate;
    }
    public static int VideoSpeed(ComShInterpreter sh,Transform tr,string val){
        var vp=tr.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
        if(vp==null) return sh.io.Error("動画は指定されていません");
        if(val==null){
            sh.io.Print(sh.fmt.FVal(vp.playbackSpeed));
            return 0;
        }
        if(!float.TryParse(val,out float f)) return sh.io.Error("値が不正です");
        vp.playbackSpeed=f;
        if(f!=0 && !vp.isPlaying) vp.Play();
        return 1;
    }
}
}
