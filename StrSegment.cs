using System.Collections.Generic;
using System.Text;
using static System.StringComparison;

namespace COM3D2.ComSh.Plugin {
public struct StrSegment {
    public string str;
    public int head;
    public int tail;
    public char this[int idx]{ get{ return str[head+idx]; } }
    public int Length { get{ return (tail<head)?0:tail-head+1; } }
    public StrSegment(string s){str=s; int len=(str==null)?0:str.Length; head=0;tail=len-1;}
    public StrSegment(string s,int h):this(s) { Slice0(ref this,h,this.tail); }
    public StrSegment(string s,int h,int t):this(s){ int t2=(t<0)?s.Length+t:t; Slice0(ref this,h,t); }
    public override string ToString(){
        if(tail<head) return "";
        if(head==0&&tail==str.Length-1) return str; //複製しない
        return str.Substring(head,tail+1-head);
    }

    public bool eq(string s2){ return this.Length==s2.Length&&string.Compare(str,head,s2,0,s2.Length,Ordinal)==0;}
    public bool eq(StrSegment s2){ return this.Length==s2.Length&&string.Compare(str,head,s2.str,s2.head,s2.Length,Ordinal)==0;}
    public bool eq_ic(string s2){ return this.Length==s2.Length&&string.Compare(str,head,s2,0,s2.Length,OrdinalIgnoreCase)==0;}
    public bool eq_ic(StrSegment s2){ return this.Length==s2.Length&&string.Compare(str,head,s2.str,s2.head,s2.Length,OrdinalIgnoreCase)==0;}
    public bool ne(string s2){ return !eq(s2);}
    public bool ne(StrSegment s2){ return !eq(s2);}
    public bool ne_ic(string s2){ return !eq_ic(s2);}
    public bool ne_ic(StrSegment s2){ return !eq_ic(s2);}
    public StrSegment Trim(){
        int h,t;
        for(h=this.head; h<=this.tail; h++) if(!char.IsWhiteSpace(str[h])) break;
        for(t=this.tail; t>=h; t--) if(!char.IsWhiteSpace(str[t])) break;
        return new StrSegment(str,h,t);
    }
    public StrSegment TrimStart(){
        int h; for(h=this.head; h<=this.tail; h++) if(!char.IsWhiteSpace(str[h])) break;
        return new StrSegment(str,h);
    }
    public StrSegment TrimEnd(){
        int t; for(t=this.tail; t>=this.head; t--) if(!char.IsWhiteSpace(str[t])) break;
        return new StrSegment(str,this.head,t);
    }
    public StrSegment Trim(char c){
        int h,t;
        for(h=this.head; h<=this.tail; h++) if(str[h]!=c) break;
        for(t=this.tail; t>=h; t--) if(str[t]!=c) break;
        return new StrSegment(str,h,t);
    }
    public StrSegment TrimStart(char c){
        int h; for(h=this.head; h<=this.tail; h++) if(str[h]!=c) break;
        return new StrSegment(str,h);
    }
    public StrSegment TrimEnd(char c){
        int t; for(t=this.tail; t>=this.head; t--) if(str[t]!=c) break;
        return new StrSegment(str,this.head,t);
    }
    private static void Slice0(ref StrSegment seg,int h,int t){
        int h2=(h<0)?seg.tail+1+h:seg.head+h,t2=seg.head+t;
        if(h2<seg.head) h2=seg.head;
        if(h2>seg.tail) h2=seg.tail+1;
        if(t2>seg.tail) t2=seg.tail;
        if(t2<h2) t2=h2-1;
        seg.head=h2; seg.tail=t2;
    }
    public StrSegment Slice(int h){
        var seg=this;
        Slice0(ref seg,h,seg.tail);
        return seg;
    }
    public StrSegment Slice(int h,int t){
        var seg=this;
        Slice0(ref seg,h,t);
        return seg;
    }
    public StrSegment SliceLen(int h,int l){
        var seg=Slice(h);
        Slice0(ref seg,0,l-1);
        return seg;
    }
    public void SliceSelf(int h){ Slice0(ref this,h,this.tail); }
    public void SliceSelf(int h,int t){ Slice0(ref this,h,t); }
    public string Substr(int h){return Slice(h).ToString();}
    public string Substr(int h,int len){return (len<=0)?"":Slice(h,h+len-1).ToString();}
    public int IndexOf(char c){return this.IndexOf(c,0);}
    public int IndexOf(char c,int s){
        int i0=head+s;
        if(tail<head || head>i0 || i0>tail) return -1;
        for(int i=i0; i<=tail; i++) if(str[i]==c) return i-head;
        return -1;
    }
    public int IndexOf(char[] ca){return this.IndexOf(ca,0);}
    public int IndexOf(char[] ca,int s){
        int i0=head+s;
        if(tail<head || head>i0 || i0>tail) return -1;
        for(int i=i0; i<=tail; i++)
            for(int j=0; j<ca.Length; j++) if(str[i]==ca[j]) return i-head;
        return -1;
    }
    public int IndexOf(string tx){return this.IndexOf(tx,0);}
    public int IndexOf(string tx,int s){
        int i0=head+s;
        if(tail<head || head>i0 || i0>tail) return -1;
        int ret=str.IndexOf(tx,i0,tail-i0+1,Ordinal);
        return (ret>=0)?ret-head:ret;
    }
    public int LastIndexOf(char c){return this.LastIndexOf(c,tail);}
    public int LastIndexOf(char c,int t){
        int i0=head+t;
        if(tail<head || head>i0 || i0>tail) return -1;
        for(int i=i0; i>=0; i--) if(str[i]==c) return i-head;
        return -1;
    }
    public int LastIndexOf(string tx){return this.LastIndexOf(tx,tail);}
    public int LastIndexOf(string tx,int t){
        int i0=head+t;
        if(tail<head || head>i0 || i0>tail) return -1;
        int ret=str.LastIndexOf(tx,i0,i0-head+1,Ordinal);
        return (ret>=0)?ret-head:ret;
    }
    public bool StartsWith(string txt){return SliceLen(0,txt.Length).eq(txt);}
    public bool StartsWith(StrSegment txt){return SliceLen(0,txt.Length).eq(txt);}
    public bool EndsWith(string txt){return Slice(-txt.Length).eq(txt);}
    public bool EndsWith(StrSegment txt){return Slice(-txt.Length).eq(txt);}

    public static readonly StrSegment empty=new StrSegment("");

    private static char[] dlmt_buf=new char[1];
    public static List<StrSegment> Split(string str,char dlmt=',',List<StrSegment> buf=null){dlmt_buf[0]=dlmt; return (new StrSegment(str)).Split(dlmt_buf,buf);}
    public static List<StrSegment> Split(string str,char[] dlmt,List<StrSegment> buf=null){return (new StrSegment(str)).Split(dlmt,buf);}

    public List<StrSegment> Split(char dlmt=',',List<StrSegment> buf=null){dlmt_buf[0]=dlmt; return this.Split(dlmt_buf,buf);}
    public List<StrSegment> Split(char[] dlmt,List<StrSegment> buf=null){
        List<StrSegment> ret=buf??new List<StrSegment>();
        ret.Clear();
        for(int pos=0; pos<this.Length;){
            int idx=this.IndexOf(dlmt,pos);
            if(idx<0){
                ret.Add(this.Slice(pos));
                break;
            }else{
                ret.Add(this.Slice(pos,idx-1));
                pos=idx+1;
            }
        }
        return ret;
    }
    public List<StrSegment> Split(string dlmt,List<StrSegment> buf=null){
        List<StrSegment> ret=buf??new List<StrSegment>();
        ret.Clear();
        for(int pos=0; pos<this.Length;){
            int idx=this.IndexOf(dlmt,pos);
            if(idx<0){
                ret.Add(this.Slice(pos));
                break;
            }else{
                ret.Add(this.Slice(pos,idx-1));
                pos=idx+dlmt.Length;
            }
        }
        return ret;
    }
    public static string Join(string fs,List<StrSegment> segs){
        if(segs.Count==0) return "";
        StringBuilder sb=new StringBuilder();
        sb.Append(segs[0].str,segs[0].head,segs[0].Length);
        for(int i=1; i<segs.Count; i++) sb.Append(fs,0,1).Append(segs[i].str,segs[i].head,segs[i].Length);
        return sb.ToString();
    }
}
}
