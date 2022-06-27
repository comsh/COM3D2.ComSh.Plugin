using System;
using System.Collections.Generic;
using System.IO;
using static System.StringComparison;

// AnmCommon,AnmCnvのメモリ使わない版。ケチってる分拡張性なし
// 現状情報取得と性別変換のみ
namespace COM3D2.ComSh.Plugin {
    public class AnmFile {
        public int format;
        public int gender;  // maid:0/man:1
        public byte useMuneL;
        public byte useMuneR;
        public byte[] buf;
        public AnmFile(byte[] buf){     // arcの読込の都合上、入力はbyte[]固定
            this.buf=buf;
            using (var r=new BinaryReader(new MemoryStream(buf))){ Inspect(r); }
            if(format==1001){
                useMuneL=buf[buf.Length-2];
                useMuneR=buf[buf.Length-1];
            }
        }
        private void Inspect(BinaryReader r){
            _=r.ReadBytes(11);
            format=r.ReadInt32();
            int m=(format==1001)?3:0;
            gender=-1;
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
                while ((t=r.PeekChar())>=0) {
                    if (t==1) break;
                    else if (t>=100){
                        var fl=new AnmFrameList(r);
                        if(fl.fcnt==0) continue;
                        var f0=new AnmFrame(r);
                        for(int i=1; i<fl.fcnt-1; i++) _=new AnmFrame(r);
                        var fn=new AnmFrame(r);
                        if(mune>0){
                            if(fl.fcnt>2||(fl.fcnt==2 && f0.value!=fn.value)) m|=mune;
                        }
                    } else break;
                }
                if(gender>=0 && m==3) break;
            }
            if(format!=1001){ useMuneL=(byte)(m>>1); useMuneR=(byte)(m&1); }
        }
        public byte[] ChgGender(){  // 例外飛ぶよ
            gender^=1;
            string tmpname=Path.GetTempFileName();
            using (var r=new BinaryReader(new MemoryStream(buf)))
            using (var w=new BinaryWriter(File.OpenWrite(tmpname))){
                Filter(r,w,true);
            }
            buf=File.ReadAllBytes(tmpname); 
            File.Delete(tmpname);
            return buf;
        }
        private void Filter(BinaryReader r,BinaryWriter w,bool gencnv=false){
            byte[] hdr = r.ReadBytes(15);
            w.Write(hdr);
            while (r.Read()==1){
                int ftype=0;
                var be=new AnmBoneEntry(r);
                if(gencnv){
                    string name = be.boneName;
                    foreach(string[] rep in f2m) name=name.Replace(rep[gender^1],rep[gender]);
                    be.rename(name);
                }
                be.write(w);
                int t;
                float minTime=Single.MaxValue;
                float maxTime=Single.MinValue;
                while ((t=r.PeekChar())>=0) {
                    if (t==1) break;
                    else if (t>=100){
                        var fl=new AnmFrameList(r);
                        fl.write(w);
                        ftype=fl.type;
                        for(int i=0; i<fl.fcnt; i++){
                            var f=new AnmFrame(r);
                            if(f.time>maxTime) maxTime=f.time;
                            if(f.time<minTime) minTime=f.time;
                            f.write(w);
                        }
                    }else break;
                }
                if(gencnv && gender==0 && be.boneName.EndsWith("Spine",Ordinal)){ // man -> maid
                    // Spineの後に、Spineの最小～最大時間にあわせて、Spine0aを作る
                    AnmBoneEntry be0a = new AnmBoneEntry("Bip01/Bip01 Spine/Bip01 Spine0a");
                    be0a.write(w);
                    bool rq=format==1001||(ftype>=100&&ftype<=103);
                    bool mq=format==1001||(ftype>=104&&ftype<=106);
                    if(rq){
                        for (int i = 100; i<103; i++){ // qx,qy,qz
                            (new AnmFrameList((byte)i){fcnt=2}).write(w);
                            (new AnmFrame(minTime){value=0}).write(w);
                            (new AnmFrame(maxTime){value=0}).write(w);
                        }
                        // qw
                        (new AnmFrameList((byte)103){fcnt=2}).write(w);
                        (new AnmFrame(minTime){value=1}).write(w);
                        (new AnmFrame(maxTime){value=1}).write(w);
                    }
                    if(mq){
                        for (int i = 104; i<=106; i++){ // qx,qy,qz
                            (new AnmFrameList((byte)i){fcnt=2}).write(w);
                            (new AnmFrame(minTime){value=0}).write(w);
                            (new AnmFrame(maxTime){value=0}).write(w);
                        }
                    }
                }
            }
            if(format==1001){ w.Write(useMuneL); w.Write(useMuneR); }
        }
        private static string[][] f2m = {      // Spineの構成が男女で違う
            new string[]{"Bip01 Spine0a/Bip01 Spine1","ManBip Spine1"},
            new string[]{"Spine1a","Spine2"},
            new string[]{"Bip01","ManBip"},
        };
        // 足の指も男性は少ないんだけどそっちは放置する
    }

    // 以下はAnmToolsから流用。かなり機能を削ってるのでメソッド内スカスカで意味不明かも

	public class AnmBoneEntry {
		public string boneName = "";    // ボーン名(完全)

		public AnmBoneEntry() { }
		public AnmBoneEntry(string name) { rename(name); }
		public AnmBoneEntry(BinaryReader r) { read(r); }

		public void rename(string name) {
			boneName=name;
		}
		public void read(BinaryReader r) {
			// ボーン名は文字列長(LEB128)＋文字列という形式だが、ReadString()なら１発
			rename(r.ReadString());
		}
		public void write(BinaryWriter w) {
			w.Write((byte)1);
			w.Write(boneName);	// 書き出しもWrite(string)なら１発
			return;
        }
	}
	public class AnmFrameList : List<AnmFrame> {
		public byte type = 0;   // 100-106 4元数＋移動xyzの7種類
        public int fcnt = 0;

		public AnmFrameList() {}
		public AnmFrameList(byte type) { this.type=type; }
		public AnmFrameList(BinaryReader r) { read(r); }

		public void read(BinaryReader r) {
			type=r.ReadByte();
			fcnt = r.ReadInt32();
		}
		public void write(BinaryWriter w) {
			w.Write(type);
			w.Write(fcnt);
		}
	}
	public class AnmFrame {     // キーフレーム。UnityのKeyframe(WeightedMode.None)
		public float time = 0;      // フレーム時刻(1/1000ms)
		public float value = 0;		// パラメータ値(意味はAnmFrameListのtypeによる)
		public float tan1 = 0;		// 補間用。１つ前の値との間の３次曲線の接線
		public float tan2 = 0;      // 補間用。次の値との間の３次曲線の接線

		public AnmFrame() {}
		public AnmFrame(float time) { this.time=time; }
		public AnmFrame(AnmFrame f) {
			time=f.time;
			value=f.value;
			tan1=f.tan1;
			tan2=f.tan2;
		}
		public AnmFrame(BinaryReader r) { read(r); }

		public void read(BinaryReader r) {
			time=r.ReadSingle();
			value=r.ReadSingle();
			tan1=r.ReadSingle();
			tan2=r.ReadSingle();
		}
		public void write(BinaryWriter w) {
			w.Write(time);
			w.Write(value);
			w.Write(tan1);
			w.Write(tan2);
		}
	}
}
