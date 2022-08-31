using System.Collections.Generic;

namespace COM3D2.ComSh.Plugin {

public class Variables {
    public static Dictionary<string,string> g=new Dictionary<string,string>();
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
            sdic[key]+=val;
        }else{
            ldic[key]+=val;
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

    // インデクサだけ <string,string> 仕様にする
    public new string this[string key]{  
        set {
            if(!TryGetValue(key,out ReferredVal rv)) Add(key,new ReferredVal(value)); 
            else rv.Set(value);
        }
        get{
            if(!TryGetValue(key,out ReferredVal rv)) return "";
            return rv.Get();
        }
    }
    public VarDic(){}
    public VarDic(VarDic vd) {
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
}
public class ReferredVal {
    public string val;
    public object dic;
    public ReferredVal(string v){  val=v; dic=null; }
    public ReferredVal(string k,object d){  val=k; dic=d; }
    public string Get(){
        if(dic==null) return val;
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
    public void SetRef(string key,object d){ val=key; dic=d; }
    public override string ToString(){ return Get(); }
}

}

