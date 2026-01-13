using System;
using System.IO;
using System.Collections.Generic;
using static System.StringComparison;

namespace COM3D2.ComSh.Plugin {

public static class DataFiles {

    private static Dictionary<string,TmpFile> tmpfiledic=new Dictionary<string,TmpFile>();

    public class TmpFile : IDisposable {
        public string id;
        public string filename;
        public string original;
        public TmpFile(string id,string orig){
            this.id=id;
            this.filename=Path.GetTempFileName();
            if(string.IsNullOrEmpty(orig)) this.original=id; else Copy(orig);
            tmpfiledic[id]=this;
        }
        public void Copy(string orig){
            var tf=GetTempFile2(orig);
            if(tf==null){
                this.original=orig;
                if(orig.EndsWith(".anm",Ordinal)){
                    string file=ComShInterpreter.myposeDir+orig;
                    if(File.Exists(file)){
                        File.Copy(file,this.filename,true);
                        return;
                    }
                } else if(orig.EndsWith(".png",Ordinal)){
                    string file=ComShInterpreter.homeDir+@"PhotoModeData\\Texture\\"+orig;
                    if(File.Exists(file)){
                        File.Copy(file,this.filename,true);
                        return;
                    }
                }
                byte[] buf=UTIL.AReadAll(orig);
                if(buf==null) throw new Exception();
                File.WriteAllBytes(this.filename,buf);
            }else{
                this.original=tf.original;
                File.Copy(tf.filename,this.filename,true);
            }
        }
        ~TmpFile(){
            this.Dispose(false);
        }
        public void Dispose(){
            this.Dispose(true);
        }
        private void Dispose(bool disposing){
            if(this.filename==null) return;
            try{File.Delete(this.filename);}catch{}
            this.filename=null;
        }
    }
    public static bool IsTempFile(string name){ return tmpfiledic.ContainsKey(name); }
    public static TmpFile GetTempFile(string name){
        if(tmpfiledic.TryGetValue(name,out TmpFile tmpfile)) return tmpfile;
        return null;
    }
    public static TmpFile GetTempFile2(string name){
        if(name.Length>=2 && name[0]=='*') return GetTempFile(name.Substring(1));
        return null;
    }

    public static Dictionary<string,TmpFile>.KeyCollection TmpFileList(){ return tmpfiledic.Keys; }
    public static TmpFile CreateTempFile(string id,string src){
        try{
            if(tmpfiledic.TryGetValue(id,out TmpFile tf)){
                if(!string.IsNullOrEmpty(src)) tf.Copy(src);
                return tf;
            }else return new TmpFile(id,src);
        }catch{}
        return null;
    }
    public static int ExportTmpFile(string id,string name,string path){
        if(!tmpfiledic.TryGetValue(id,out TmpFile tf)) return -1;
        string dst=UTIL.GetFullPath(name,path);
        if(dst=="") return -3;
        try{
            if(File.Exists(tf.filename)) File.Copy(tf.filename,dst,true);
        }catch{
            return -2;
        }
        return 0;
    }
    public static int AppendTmpFile(string id,string txt){
        if(!tmpfiledic.TryGetValue(id,out TmpFile tf)) return -1;
        try{
            if(File.Exists(tf.filename)) File.AppendAllText(tf.filename,txt);
        }catch{
            return -2;
        }
        return 0;
    }
    public static void DeleteTempFile(string id){ tmpfiledic.Remove(id); }

    public static void Clean(){ tmpfiledic.Clear(); }
}
}
