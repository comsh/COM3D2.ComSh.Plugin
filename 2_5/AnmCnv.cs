using System;
using System.Collections.Generic;
using System.IO;
using static System.StringComparison;
using UnityEngine;

namespace COM3D2.ComSh.Plugin {
    public class AnmFile {
        public int format;
        public int gender;  // maid:0/man:1
        public byte useMuneL;
        public byte useMuneR;
        public float minTime;
        public float maxTime;
        public bool nolposq;
        public List<AnmBoneEntry> bones;
        public AnmFile(string fn,bool nolpos=true){
            nolposq=nolpos;
            bones=new List<AnmBoneEntry>();
            using (var fs=File.OpenRead(fn))
            using (var r=new BinaryReader(fs)) { Read(r); }
        }
        public AnmFile(byte[] buf,bool nolpos=true){
            nolposq=nolpos;
            bones=new List<AnmBoneEntry>();
            using (var ms=new MemoryStream(buf))
            using (var r=new BinaryReader(ms)) { Read(r); }
        }
        public bool IsEmpty(){ return (bones==null||bones.Count==0||gender<0||maxTime<=minTime); }
        private void Read(BinaryReader r){
            r.ReadBytes(11);
            format=r.ReadInt32();
            int m=(format==1001)?3:0;
            gender=-1;
            minTime=Single.MaxValue;
            maxTime=Single.MinValue;
            while (r.Read()==1){
                var be=new AnmBoneEntry(r);
                if(gender<0){
                    if(be.boneName.StartsWith("ManBip",Ordinal)) gender=1;
                    else if(be.boneName.StartsWith("Bip01",Ordinal)) gender=0;
                }
                int mune=0;
                if(m<3){
                    if(be.boneName.EndsWith("Mune_L",Ordinal)) mune=2;
                    else if(be.boneName.EndsWith("Mune_R",Ordinal)) mune=1;
                }
                int t;
                bool hasAnim=false;
                while((t=r.PeekChar())>=100&&t<=106){
                    var cv=be.addcurve(r);
                    if(cv==null) continue;
                    int n=cv.length;
                    if(n==0) continue;
                    hasAnim=true;
                    if(n>2 || cv[0].value!=cv[n-1].value) m|=mune;
                    if(cv[0].time>maxTime) maxTime=cv[0].time;
                    if(cv[0].time<minTime) minTime=cv[0].time;
                    if(cv[n-1].time>maxTime) maxTime=cv[n-1].time;
                    if(cv[n-1].time<minTime) minTime=cv[n-1].time;
                }
                if(hasAnim) bones.Add(be);
            }
            if(format==1001){ useMuneL=r.ReadByte(); useMuneR=r.ReadByte();}
            else{ useMuneL=(byte)(m>>1); useMuneR=(byte)(m&1); }
        }
        public void ChgGender(){
            int inspos=-1,idx0a=-1;
            int oldgender=gender,newgender=gender^1;
            for(int i=0; i<bones.Count; i++){
                var ab=bones[i];
                string name=ab.boneName;
                for(int repi=0; repi<f2m.Length; repi++)
                    name=name.Replace(f2m[repi][oldgender],f2m[repi][newgender]);
                ab.rename(name);
                if(newgender==0){
                    if(name.EndsWith("Spine",Ordinal)) inspos=i;
                    else if(name.EndsWith("Spine0a",Ordinal)) idx0a=i;
                }
            }
            if(inspos>=0 && idx0a<0){
                AnmBoneEntry be0a = new AnmBoneEntry("Bip01/Bip01 Spine/Bip01 Spine0a");
                for (int j=100; j<103; j++) be0a.addcurve(j,minTime,0,maxTime,0);
                be0a.addcurve(103,minTime,1,maxTime,1);
                if(inspos+1<bones.Count) bones.Insert(inspos+1,be0a); else bones.Add(be0a);
            }
            gender=newgender;
        }
        private static string[][] f2m = {      // Spineの構成が男女で違う
            new string[]{"Bip01 Spine0a/Bip01 Spine1","ManBip Spine1"},
            new string[]{"Spine1a","Spine2"},
            new string[]{"Bip01","ManBip"},
            // 足の指もManBipでは少ないんだけどそっちは放置する
        };
        private static string[] propnames={
            "m_LocalRotation.x","m_LocalRotation.y","m_LocalRotation.z","m_LocalRotation.w",
            "m_LocalPosition.x","m_LocalPosition.y","m_LocalPosition.z"
        };
        public AnimationClip ToClip(){
            var clip=new AnimationClip();
            clip.legacy=true;
            for(int i=0; i<bones.Count; i++){
                var ab=bones[i];
                int n=(nolposq && ab.boneName!="Bip01" && ab.boneName!="ManBip")?4:7;
                for (int j=0; j<n; j++){
                    var curve=ab.curveList[j];
                    if(curve!=null) clip.SetCurve(ab.boneName,typeof(Transform),propnames[j],curve);
                }
            }
            return clip;
        }
    }

	public class AnmBoneEntry {
		public string boneName = "";    // ボーン名(完全)
        public AnimationCurve[] curveList=new AnimationCurve[7];

		public AnmBoneEntry(string name){rename(name);}
		public AnmBoneEntry(BinaryReader r){read(r);}

		public void rename(string name){boneName=name;}
		public void read(BinaryReader r){ rename(r.ReadString()); }
        public AnimationCurve addcurve(BinaryReader r){
			int type=r.ReadByte();
            if(type<100||type>106) return null;
			int fcnt=r.ReadInt32();
            var curve=new AnimationCurve();
            for(int i=0; i<fcnt; i++)
                curve.AddKey(new Keyframe(r.ReadSingle(),r.ReadSingle(),r.ReadSingle(),r.ReadSingle()));
            curveList[type-100]=curve;
            return curve;
		}
        public AnimationCurve addcurve(int type,float t0,float v0,float t1,float v1){
            var curve=new AnimationCurve();
            curve.AddKey(new Keyframe(t0,v0,0,0));
            curve.AddKey(new Keyframe(t1,v1,0,0));
            curveList[type-100]=curve;
            return curve;
        }
	}
}
