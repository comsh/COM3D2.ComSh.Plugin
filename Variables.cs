using System.Collections.Generic;

namespace COM3D2.ComSh.Plugin {

public static class Variables {
    public static Dictionary<string,string> g=new Dictionary<string,string>(32);
    public static string Get(string key,VarDic ldic,Dictionary<string,string> sdic){
        if(key[0]=='/'){
            if(!g.TryGetValue(key,out string val)) return "";
            return val;
        }else if(key[0]=='.'){
            if(sdic==null) return "";
            if(!sdic.TryGetValue(key,out string val)) return "";
            return val;
        }else{
            return ldic[key];
        }
    }
    public static void Set(string key,string val,VarDic ldic,Dictionary<string,string> sdic){
        if(key[0]=='/') g[key]=val;
        else if(key[0]=='.'){
            if(sdic==null) return;
            sdic[key]=val;
        }else{
            ldic[key]=val;
        }
    }
    public static void Append(string key,string val,VarDic ldic,Dictionary<string,string> sdic){
        if(key[0]=='/') g[key]+=val;
        else if(key[0]=='.'){
            if(sdic==null) return;
            if(sdic.ContainsKey(key)) sdic[key]+=val; else sdic[key]=val;
        }else{
            if(ldic.ContainsKey(key)) ldic[key]+=val; else ldic[key]=val;
        }
    }
    public static string Value(VarDic dic, string key,string dflt=""){
        if(!dic.TryGetValue(key,out ReferredVal rv)) return dflt;
        return rv.Get();
    }
    public static string Value(Dictionary<string,string> dic, string key,string dflt=""){
        if(!dic.TryGetValue(key,out string val)) return dflt;
        return val;
    }
}

public class VarDic : Dictionary<string,ReferredVal> {
    public string output="";
    public List<string> args=new List<string>();

    // インデクサだけ <string,string> 仕様にする
    public new string this[string key]{  
        set {
            if(!TryGetValue(key,out ReferredVal rv)) Add(key,new ReferredVal(value)); 
            else rv.Set(value);
        }
        get{
            if(key.Length==0) return "";
            if(key[0]=='`') return output;
            if(int.TryParse(key,out int n) && n>0){
                if(args.Count<n || args[n-1]==null) return "";
                return args[n-1];
            }
            if(!TryGetValue(key,out ReferredVal rv)) return "";
            return rv.Get();
        }
    }
    public VarDic():base(32) {}
    public VarDic(int cap):base(cap) {}
    public VarDic(VarDic vd):base(vd.Count>32?vd.Count:32) {
        foreach(var kv in vd){
            if(kv.Value.dic==null) this.Add(kv.Key,new ReferredVal(kv.Value.val));
            else this.Add(kv.Key,kv.Value);
        }
    }

    public bool IsRef(string key){
        if(!TryGetValue(key,out ReferredVal rv)) return false;
        return (rv.dic!=null);
    }
    public bool SetRef(string key,string rkey){
        ReferredVal rv;
        if(rkey[0]=='/'){
            if(!this.TryGetValue(key,out rv)) this.Add(key,new ReferredVal(rkey,Variables.g)); 
            else rv.SetRef(rkey,Variables.g);
        }else{
            if(!this.TryGetValue(key,out rv)) this.Add(key,new ReferredVal(rkey,this)); 
            else rv.SetRef(rkey,this);
        }
        return true;
    }
    public bool SetBind(string key,ReferredVal.GetValue g,ReferredVal.GetValue s){
        string rkey=" "+key;
        if(!this.TryGetValue(key,out ReferredVal rv)) this.Add(key,new ReferredVal(rkey,g,s)); 
        else rv.SetBind(rkey,g,s);
        return true;
    }
}
public class ReferredVal {
    public string val;
    public object dic;
    public delegate int GetValue(string v0,out string v1);
    private GetValue getter;
    private GetValue setter;
    public ReferredVal(string v){ val=v; dic=null; getter=null; setter=null; }
    public ReferredVal(string v,GetValue g,GetValue s){
        val=v; dic=null; getter=g; setter=s;
    }
    public ReferredVal(string k,object d){ val=k; dic=d; getter=null; setter=null; }
    public string Get(){
        if(dic==null){
            if(getter==null) return val;
            if(getter.Invoke(val,out string v1)<0) getter=null;
            return (val=v1);
        }
        if(val[0]=='/'){
            var d=(Dictionary<string,string>)dic;
            if(!d.TryGetValue(val,out string v)) return "";
            return v;
        }else{
            var d=(Dictionary<string,ReferredVal>)dic;
            if(!d.TryGetValue(val,out ReferredVal v)) return "";
            return v.val;
        }
    }
    public void Set(string v){
        if(dic==null){
            if(setter==null){val=v; return;}
            if(setter.Invoke(v,out val)<0){ setter=null; return; }
            return;
        }
        if(dic==null){ val=v; return; }
        if(val[0]=='/'){
            var d=(Dictionary<string,string>)dic;
            d[val]=v;
        }else{
            var d=(Dictionary<string,ReferredVal>)dic;
            if(!d.TryGetValue(val,out ReferredVal rv)) d.Add(val,new ReferredVal(v));
            else rv.val=v;
        }
    }
    public void SetRef(string key,object d){ val=key; dic=d; getter=null; setter=null; }
    public void SetBind(string key,GetValue g,GetValue s){ val=key; dic=null; getter=g; setter=s; }
    public override string ToString(){ return Get(); }
}

}

