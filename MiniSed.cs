using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace COM3D2.ComSh.Plugin {

public class MiniSed {
    // address
    public int start=0;
    public int end=int.MaxValue;
    public string rex="";
    public bool reverse=false;

    // command
    public char cmd;
    public string ptn0;
    public string ptn1;

    public int rcnt=int.MaxValue;    // 最大置換回数。gオプションなしのとき1[回]
    public RegexOptions opt;    // i,g
    public char tropt;          // d,s
    // s//2 などはできない

    public Regex rrex;
    public Regex rptn0;

    // error
    public string error;

    delegate void MiniSedAct(Line l,bool adrq);

    public string Process(string txt){
        StringBuilder sb=new StringBuilder(1024);
        if(cmd=='d'){
            Filter(txt,(Line l,bool adrq)=>{ if(!adrq) sb.Append(txt,l.head,l.LengthLn); });
        }else if(cmd=='s'){
            Filter(txt,(Line l,bool adrq)=>{
                if(adrq) sb.Append(rptn0.Replace(l.TextLn,ptn1,rcnt>0?rcnt:int.MaxValue));
                else sb.Append(txt,l.head,l.LengthLn);
            });
        }else if(cmd=='y'){
            Filter(txt,(Line l,bool adrq)=>{
                if(adrq){
                    string src=l.TextLn;
                    switch(tropt){
                    case 'd':
                        for(int i=0; i<src.Length; i++){
                            int found=ptn0.IndexOf(src[i]);
                            if(found<0) sb.Append(src[i]);
                        } break;
                    case 's':   // unix trコマンドと仕様が違ってたのでsはリファレンスに載せないでおく
                        for(int i=0; i<src.Length; i++){
                            int found=ptn0.IndexOf(src[i]);
                            if(found<0) sb.Append(src[i]); else{
                                char c=ptn1[Math.Min(found,ptn1.Length-1)];
                                int sblen=sb.Length;
                                if(sblen==0 || c!=sb[sblen-1]) sb.Append(c);
                            }
                        } break;
                    default:
                        for(int i=0; i<src.Length; i++){
                            int found=ptn0.IndexOf(src[i]);
                            sb.Append((found<0)?src[i]:ptn1[Math.Min(found,ptn1.Length-1)]);
                        } break;
                    }
                }else sb.Append(txt,l.head,l.LengthLn);
            });
        }
        return sb.ToString();
    }
    private void Filter(string txt,MiniSedAct act){
        bool b=cmd=='d'?reverse:!reverse;
        if(rrex!=null){
            foreach(Line l in EoL(txt))
                act(l, rrex.Match(l.TextLn).Success!=reverse);
        }else{
            var iter=EoL(txt).GetEnumerator();
            if(start==-1){
                start=0;
                while(iter.MoveNext()){start++;}
                iter=EoL(txt).GetEnumerator();
            }
            int lno=0;
            while(iter.MoveNext()){
                lno++;
                act(iter.Current, (lno>=start && lno<=end)!=reverse);
            }
        }
    }

    public MiniSed(string cmd){
        int i;
        // address
        if(cmd[0]=='/'){
            i=GetPattern(cmd,0,out rex,'/');
            if(i<0 || rex==""){ error="パターンが不正です"; return;}
            try{ rrex=new Regex(rex); }catch(Exception e){ error=e.Message; return; }
            i++;
        }else{
            i=GetAdrNum(cmd,0,out start);
            if(i<0 || i==cmd.Length){ error="アドレスが不正です"; return;}
            if(i>0){
                if(cmd[i]==','){
                    int head=i+1;
                    i=GetAdrNum(cmd,head,out end);
                    if(i<0 || i==head){ error="アドレスが不正です"; return;}
                    if(start<0 || (end>=0 && start>end)){ int t=start; start=end; end=t; }
                }else end=start;
                if(end<0) end=int.MaxValue;
            }
        }
        if(i>=cmd.Length){ error="アドレスが不正です"; return;}
        if(cmd[i]=='!'){reverse=true; i++;}
        if(i>=cmd.Length){ error="アドレスが不正です"; return;}


        // command
        if(cmd[i]=='d'){this.cmd='d'; i++;}
        else if(cmd[i]=='s'){
            this.cmd='s';
            i=GetPattern(cmd,i+1,out ptn0);
            if(i<0||ptn0==""){ error="パターンが不正です"; return;}
            i=GetPattern(cmd,i,out ptn1);
            if(i<0){ error="パターンが不正です"; return;}
            i=GetPtnOpt(cmd,i+1,out rcnt, out opt);
            if(i<0){ error="パターンが不正です"; return;}
            try{ rptn0=new Regex(ptn0,opt); }catch(Exception e){ error=e.Message; return; }
        }else if(cmd[i]=='y'){
            this.cmd='y';
            i=GetPattern(cmd,i+1,out ptn0);
            if(i<0||ptn0=="") {error="パターンが不正です"; return; }
            i=GetPattern(cmd,i,out ptn1);
            if(i<0) { error="パターンが不正です"; return;}
            i=GetTrOpt(cmd,i+1,out tropt);
            if(i<0){ error="パターンが不正です"; return;}
            if((tropt=='d'&&ptn1!="")||ptn1==""){ error="パターンが不正です"; return;}
            ptn0=ExtractTrPtn(ptn0);
            ptn1=ExtractTrPtn(ptn1);
        }
        if(i!=cmd.Length){ error="書式が不正です"; return; }
        return;
    }
    private static int GetAdrNum(string cmd,int pos,out int value){
        if(cmd[pos]=='$'){ value=-1; return pos+1; }
        return GetNum(cmd,pos,out value);
    }
    private static int GetNum(string cmd,int pos,out int value){
        value=0;
        int i;
        int n=0;    // 数値桁数
        for(i=pos; i<cmd.Length; i++){
            if(!char.IsDigit(cmd[i])) break;
            if(n==9) return -1; // 数字10桁目になったらエラー
            value=value*10+(cmd[i]-'0');
            n++;
        }
        return i;
    }
    private static int GetPattern(string cmd,int pos,out string pattern){
        if(cmd.Length<pos){ pattern=""; return -1; }
        char sep=cmd[pos];
        return GetPattern(cmd,pos,out pattern,sep);
    }
    private static int GetPattern(string cmd,int pos,out string pattern,char sep){
        pattern="";
        if(cmd[pos]!=sep) return -1;
        int i,bi=0;
        char[] buf=new char[cmd.Length];
        bool escape=false;
        for(i=pos+1; i<cmd.Length; i++){
            if(escape){
                escape=false;
                if(cmd[i]==sep) buf[bi-1]=sep; else buf[bi++]=cmd[i];
            }else{
                if(cmd[i]=='\\') escape=true;
                else if(cmd[i]==sep) break;
                buf[bi++]=cmd[i];
            }
        }
        if(i==cmd.Length) return -1;
        if(i>pos+1) pattern=new string(buf,0,bi);
        return i;
    }
    private static int GetPtnOpt(string cmd,int pos,out int cnt, out RegexOptions opt){
        cnt=1;
        opt=RegexOptions.None;
        int i=pos;
        while(i<cmd.Length){
            if(cmd[i]=='g'){
                cnt=int.MaxValue;
            }else if(cmd[i]=='i'){
                opt|=RegexOptions.IgnoreCase;
            }else break;
            i++;
        }
        return i;
    }
    private static int GetTrOpt(string cmd,int pos, out char tropt){
        tropt='\0';
        if(pos>=cmd.Length) return pos;
        char c=cmd[pos];
        if(!(c=='d'||c=='s')) return pos;
        tropt=c;
        return pos+1;
    }
    public static Regex GetPtnAndOpt(string txt,int pos=0){
        if(pos>=txt.Length) return null;
        int i=pos;
        i=GetPattern(txt,i,out string ptn);
        if(i<0) return null;
        i=GetPtnOpt(txt,i+1,out int rcnt, out RegexOptions opt);
        if(i!=txt.Length) return null;
        try{ return new Regex(ptn,opt); }catch{ return null; }
    }
    private static string ExtractTrPtn(string ptn){
        StringBuilder buf=new StringBuilder(ptn.Length);
        bool escape=false;
        bool range=false;
        for(int i=0; i<ptn.Length; i++){
            if(ptn[i]=='\\'){escape=true;continue;}
            if(escape) escape=false; else{
                if(ptn[i]=='-' && buf.Length>0){range=true; continue;}
                if(range){
                    for(int code=buf[buf.Length-1]+1; code<ptn[i]; code++) buf.Append((char)code);
                    range=false;
                }
            }
            buf.Append(ptn[i]);
        }
        return buf.ToString();
    }
    public class Line {
        public string txt;
        public int head=0;
        public int tail=0;
        public int LengthLn{get{ return tail==txt.Length?(tail-head):(tail+1-head); }}
        public string TextLn{get{ return _textLn??(_textLn=txt.Substring(head,tail==txt.Length?(tail-head):(tail+1-head))); }}
        private string _textLn;
        public Line(string txt){ this.txt=txt; }
        public Line Clear(){ _textLn=null; return this; }
    }
    public IEnumerable<Line> EoL(string txt){
        Line ret=new Line(txt);
        while((ret.tail=txt.IndexOf('\n',ret.head))>=0){
            yield return ret.Clear();
            ret.head=ret.tail+1;
            if(ret.head>=txt.Length) break;
        }
        ret.tail=txt.Length;
        if(ret.head==ret.tail) yield break;
        yield return ret.Clear();
    }
}
}

