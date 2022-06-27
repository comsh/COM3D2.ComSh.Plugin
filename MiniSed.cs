using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace COM3D2.ComSh.Plugin {

public class MiniSed {
    // address
    public int start=int.MinValue;
    public int end=int.MaxValue;
    public string rex="";
    public bool reverse=false;

    // command
    public char cmd;
    public string ptn0;
    public string ptn1;

    public int rcnt=int.MaxValue;    // 最大置換回数。gオプションなしのとき1[回]
    public RegexOptions opt;    // i,g
    // s//2 などはできない

    public Regex rrex;
    public Regex rptn0;

    // error
    public string error;

    delegate void MiniSedAct(Line l,bool adrq);

    public string Process(string txt){
        StringBuilder sb=new StringBuilder();
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
                    char[] ca=l.TextLn.ToCharArray();
                    for(int i=0; i<ca.Length; i++) for(int j=0; j<ptn0.Length; j++) if(ca[i]==ptn0[j]) ca[i]=ptn1[j];
                    sb.Append(ca);
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
            i=GetPattern(cmd,0,out rex);
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
                }else end=start;
            }
        }
        if(i>=cmd.Length){ error="アドレスが不正です"; return;}
        if(cmd[i]=='!'){reverse=true; i++;}
        if(i>=cmd.Length){ error="アドレスが不正です"; return;}

        if(start==int.MinValue){ start=0; end=int.MaxValue; }
        else if(start>end){ int t=start; start=end; end=t; }

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
            if(i<0||ptn0.Length!=ptn1.Length) { error="パターンが不正です"; return;}
            i++;
        }
        if(i!=cmd.Length){ error="書式が不正です"; return; }
        return;
    }
    private static int GetAdrNum(string cmd,int pos,out int value){
        if(cmd[pos]=='$'){ value=int.MaxValue; return pos+1; }
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
        pattern="";
        if(cmd[pos]!='/') return -1;
        int i;
        bool escape=false;
        for(i=pos+1; i<cmd.Length; i++){
            if(escape){ escape=false; continue; }
            if(cmd[i]=='\\') escape=true;
            else if(cmd[i]=='/') break;
        }
        if(i==cmd.Length) return -1;
        if(i>pos+1) pattern=cmd.Substring(pos+1,i-(pos+1));
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

