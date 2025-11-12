using static System.StringComparison;

namespace COM3D2.ComSh.Plugin {
public struct StrSegment {
    public string str;
    public int head;
    public int tail;
    public char this[int idx]{ get{ return str[head+idx]; } }
    public int Length { get{ return (tail<head)?0:tail-head+1; } }
    public StrSegment(string s){str=s; head=0;tail=str.Length-1;}
    public StrSegment(string s,int h){str=s; head=(h<0)?str.Length+h:h;tail=str.Length-1;}
    public StrSegment(string s,int h,int t){str=s; head=(h<0)?str.Length+h:h;tail=(t<0)?str.Length+t:t;}
    public override string ToString(){
        if(tail<head) return "";
        if(head==0&&tail==str.Length-1) return str; //複製しない
        return str.Substring(head,tail+1-head);
    }

    public bool eq(string s2){ return this.Length==s2.Length&&string.CompareOrdinal(str,head,s2,0,s2.Length)==0;}
    public bool eq(StrSegment s2){ return this.Length==s2.Length&&string.CompareOrdinal(str,head,s2.str,s2.head,s2.Length)==0;}
    public StrSegment Trim(){
        int h,t;
        for(h=this.head; h<=this.tail; h++) if(str[h]!=' ') break;
        for(t=this.tail; t>=h; t--) if(str[t]!=' ') break;
        return new StrSegment(str,h,t);
    }
    public StrSegment Slice(int h){
        int h2=(h<0)?tail+1+h:head+h;
        return new StrSegment(str,(h2<head)?head:h2,tail);
    }
    public StrSegment Slice(int h,int t){
        int h2=(h<0)?tail+1+h:head+h,t2=(t<0)?tail+1+t:head+t;
        return new StrSegment(str,(h2<head)?head:h2,(t2>tail)?tail:t2);
    }
    public StrSegment SliceLen(int h,int l){
        int h2=(h<0)?tail+1+h:head+h,t2=h2+l-1;
        return new StrSegment(str,(h2<head)?head:h2,(t2>tail)?tail:t2);
    }
    public void SliceSelf(int h,int t){
        int h2=(h<0)?tail+1+h:head+h,t2=(t<0)?tail+1+t:head+t;
        head=(h2<head)?head:h2;
        tail=(t2>tail)?tail:t2;
    }
    public string Substr(int h){return Slice(h).ToString();}
    public string Substr(int h,int len){return Slice(h,h+len-1).ToString();}
    public int IndexOf(char c){return this.IndexOf(c,0);}
    public int IndexOf(char c,int s){
        if(tail<head) return -1;
        int ret=str.IndexOf(c,head+s,tail-head-s+1);
        return (ret>=0)?ret-head:ret;
    }
    public int IndexOf(string tx){return this.IndexOf(tx,0);}
    public int IndexOf(string tx,int s){
        if(tail<head) return -1;
        int ret=str.IndexOf(tx,head+s,tail-head-s+1,Ordinal);
        return (ret>=0)?ret-head:ret;
    }
    public int LastIndexOf(char c){return this.LastIndexOf(c,tail);}
    public int LastIndexOf(char c,int t){
        if(tail<head) return -1;
        int ret=str.LastIndexOf(c,t,t-head+1);
        return (ret>=0)?ret-head:ret;
    }
    public int LastIndexOf(string tx){return this.LastIndexOf(tx,tail);}
    public int LastIndexOf(string tx,int t){
        if(tail<head) return -1;
        int ret=str.LastIndexOf(tx,t,t-head+1,Ordinal);
        return (ret>=0)?ret-head:ret;
    }
    public bool StartsWith(string txt){return SliceLen(0,txt.Length).eq(txt);}
    public bool StartsWith(StrSegment txt){return SliceLen(0,txt.Length).eq(txt);}
    public bool EndsWith(string txt){return Slice(-txt.Length).eq(txt);}
    public bool EndsWith(StrSegment txt){return Slice(-txt.Length).eq(txt);}
}
}
