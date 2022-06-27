using System;
using System.Collections.Generic;
using System.Text;
using static System.StringComparison;

namespace COM3D2.ComSh.Plugin {

// パーサ
public class ComShParser {

	public const byte trash = 1;		// 出力しない文字
	public const byte escaped = 2;	    // エスケープされた文字。単一引用符内もこれ
    public const byte varname = 4;      // 変数置換記述の変数名

    public string error;                // パース時にエラーがあればエラーメッセージが入る
    public char prevEoL;                // 直前に取り出した行の行末記号(;/|)
    public char currentEoL;             // 最後に取り出した行の行末記号(;/|)
    public bool envChanged;             // 環境変数に変更があればtrue

    private int head, tail;             // 入力文字列全体の先頭と末尾
    private char[] cha;
    private byte[] qm;
    public int nextHead;

    public int lineno;


    public int Parse(string text){
        error=null;
        prevEoL=currentEoL=';'; envChanged=false;
        this.cha=text.ToCharArray();
        int r=Analyze();
        nextHead=head;
        return r;
    }
    public void Reset(){
        error=null;
        prevEoL=currentEoL=';'; envChanged=false;
        nextHead=head;
    }
    // escapedでない'|'/';'で区切られた複数の行を１行ずつ取り出す
    public List<string> Next(Dictionary<string,string> vars){
        envChanged=false;
        if(nextHead>tail) return null;
		int i,start=nextHead,curhead;
		for(i=start; i<=tail; i++) if((cha[i]==';'||cha[i]=='|'||cha[i]=='>')&&(qm[i]&escaped)==0) {
            prevEoL=currentEoL;
            currentEoL=cha[i];
            curhead=nextHead;
            if(i<tail&&cha[i]=='>'&&cha[i+1]=='>') nextHead=i+2; else nextHead=i+1;;
            if (i>start){
                if(prevEoL=='>'){
                    bool appendq=(curhead-2>=0 && cha[curhead-2]=='>');
                    return Redirect(start,i-1,vars,appendq);
                }
                return SingleLine(start, i-1,vars);
            }
            start=nextHead;
		}
        prevEoL=currentEoL;
        currentEoL=';';     // 改行だけどこれにしておく
        curhead=nextHead;
        if(i<tail&&cha[i]=='>'&&cha[i+1]=='>') nextHead=i+2; else nextHead=i+1;;
        if(i>start){
            if(prevEoL=='>'){
                bool appendq=(curhead-2>=0 && cha[curhead-2]=='>');
                return Redirect(start,i-1,vars,appendq);
            }
            return SingleLine(start,i-1,vars);            
        }
        return null;
    }
    private int Analyze() {          // trimとquotemap作成。入力文字列の正味の長さを返す
        qm=new byte[cha.Length];
        head=-1; tail=cha.Length;
        while(--tail>=0) if(cha[tail]!='\n'&&cha[tail]!='\r') break;      // TrimEnd('\n','\r')
        while(++head<=tail) if(cha[head]!=' '&&cha[head]!='\t') break;    // TrimStart(' ','\t')
        if(head>tail) return 0;               // 正味の入力行が空
        if(cha[head]=='#') return 0;

        // バックスラッシュおよび引用符の処理
        bool backslash=false;
        int quote=0;
        int blv=0;
        int i=head;
        if(cha[i]=='{'){ quote=3; blv=1; qm[i++]=trash; }
        for(; i<=tail; i++){
            if(quote==0){                 // 引用符外
                if(backslash){
                    if(cha[i]=='n') cha[i]='\n';
                    qm[i]=escaped;
                    backslash=false; 
                } else if(cha[i]=='\\'){ backslash=true; qm[i]=trash; }
                else if(cha[i]=='\t') cha[i]=' ';  // タブは空白文字にしてしまう
                else if(cha[i]=='\''){ quote=1; qm[i]=trash; }
                else if(cha[i]=='"') { quote=2; qm[i]=trash; }
                else if(cha[i]=='{'&&(qm[i-1]==escaped||cha[i-1]!='$')){ quote=3; blv=1; qm[i]=trash; }
                else if(cha[i]=='#'&&qm[i-1]!=escaped&&IsSeam(cha[i-1])){ tail=i-1; break; }
            }else if(quote==1){           // 単一引用符内
                if(cha[i]=='\''){ quote=0; qm[i]=trash; }
                else qm[i]=escaped;
            }else if(quote==2){           // 二重引用符内
                if(backslash){
                    if(cha[i]=='n') {cha[i]='\n'; qm[i-1]=trash;}
                    qm[i]=escaped; backslash=false;
                    if(cha[i]=='"' || cha[i]=='$' || cha[i]=='\\') qm[i-1]=trash;
                }else{
                    if(!(cha[i]=='"' || cha[i]=='$' || cha[i]=='\\')) qm[i]=escaped;
                }
                if(qm[i]!=escaped){
                    if(cha[i]=='\\'){ backslash=true; }
                    else if(cha[i]=='"'){ quote=0; qm[i]=trash; }
                }
            }else if(quote==3){         // {～}内
                if(cha[i]=='{') blv++; else if(cha[i]=='}') blv--;
                if(blv==0){ quote=0; qm[i]=trash; } else qm[i]=escaped;
            }
        }   
        if(quote==1){error="単一引用符が閉じられていません"; return -1; }
        if(quote==2){error="二重引用符が閉じられていません"; return -1; }
        if(quote==3){error="{～}が閉じられていません"; return -1; }
        if(backslash){ error="バックスラッシュに続く文字がありません"; return -1; }

        // 末尾にescapedでない空白文字が残っていれば末尾更新
        while(tail>=head) if(cha[tail]==' '&&(qm[tail]&escaped)==0) tail--; else break;
        if(head>tail) return 0;               // 入力行が空

        // escapedでない'$'に続く(かもしれない)変数置換
        for (i=head; i<tail; i++) if (cha[i]=='$' && (qm[i]&escaped)==0) {   // 文末$は置換しようがないのでi<tail
            int j=i+1;  // この時点ではまだただの'$'かもしれない
            if(cha[j]=='{'){ // ${\(w+)}
                qm[i]=trash; qm[j]|=trash;        // '{'が来た時点で'$'は変数置換用と確定
                if(j++==tail) { error="${～}が閉じられていません"; return -1; }
                if(ParseUtil.IsVar1Char(cha[j])) qm[j++]|=varname; else j=GetWord(cha,qm,j,tail);
                if(j>tail) { error="${～}が閉じられていません"; return -1; }
                if(cha[j]!='}' || j==i+2){ error="${～}の書式が不正です"; return -1; }
                qm[j]|=trash; i=j;
            }else{                            // $(\w+)
                if(ParseUtil.IsVar1Char(cha[j])) qm[j++]|=varname; else j=GetWord(cha,qm,j,tail);
                if(j>i+1){ qm[i]|=trash; i=j-1; } // 変数名があれば'$'は変数置換用と確定
            }
        }
        // redirect "^>\s*\w+\s*(;|$)" $変数置換は不可(変数の内容次第で実行時エラーとなるため)
        for (i=head; i<tail; i++) if (cha[i]=='>' && (qm[i]&escaped)==0) {
            if(cha[++i]=='>') i++; // >>を認識
            for(; i<=tail; i++) if(cha[i]!=' ') break;
            if(i>tail){ error="リダイレクトの書式が不正です";return -1; }
            int start=i;
            i=GetWord(cha,qm,i,tail);
            if(start==i){ error="リダイレクトの書式が不正です";return -1; }
            for(; i<=tail; i++) if((cha[i]==';') && (qm[i]&escaped)==0) break; else{
                if((qm[i]&escaped)!=0||cha[i]!=' '){ error="リダイレクトの書式が不正です";return -1; }
            }
        }
        return 1;
	}
    private bool IsSeam(char c){ return (c==' '||c==';'||c=='|'||c=='?'); }
    private int GetWord(char[] cha,byte[] qm,int i0,int tail){
        int i;
        for(i=i0; i<=tail; i++) if(ParseUtil.IsWordChar(cha[i])) qm[i]|=varname; else break;
        return i;
    }

    // escapedでない空白文字で区切られたトークン達を得る
	private List<string> SingleLine(int from, int to,Dictionary<string,string> vars) {
		List<string> tokens=new List<string>();
        bool kvq=true;              // key-value形式は、行頭から連続する限りは有効
		int i, start = from;
		for (i=from; i<=to; i++) if(cha[i]==' ' && (qm[i]&escaped)==0) {
            if (i>start) if(!kvq||!(kvq=Keyval(start,i-1,vars))) tokens.Add(Unquote(start,i-1,vars));
			start=i+1;
		}
		if (i>start) if(!kvq||!Keyval(start,i-1,vars)) tokens.Add(Unquote(start,i-1,vars));
		return tokens;
	}
    private List<string> emptyList=new List<string>();
	private List<string> Redirect(int from, int to,Dictionary<string,string> vars,bool appendq) {
        if(vars==null) return emptyList;
		int i;  // Analyzeで処理してるので細かい考慮は要らない
		for (i=from; i<=to; i++) if((qm[i]&varname)>0) break;
        int start=i;
		for (; i<=to; i++) if((qm[i]&varname)==0) break;
        string key=new string(cha,start,i-start);
        if(appendq && vars.ContainsKey(key)) vars[key]=vars[key]+vars["`"]; else vars[key]=vars["`"];
        vars["`"]="";
        envChanged=true;
        return emptyList;
    }
    // 変数代入文の処理　
    private bool Keyval(int from,int to,Dictionary<string,string> vars){
        int i;
        for(i=from; i<to; i++) if(!(ParseUtil.IsWordChar(cha[i]))) break; // ここはIsWordChar。$#や$`は参照専用
        if(i==from) return false;
        if(cha[i]=='=' && qm[i]==0){
            if(vars==null) return true;
            string key=new string(cha,from,i-from);
            vars[key]=(i+1>to)?"":Unquote(i+1,to,vars);
            envChanged=true;
            return true;
        }
        if(i<to && cha[i]=='+' && qm[i]==0 && cha[i+1]=='=' && qm[i+1]==0){
            if(vars==null) return true;
            string key=new string(cha,from,i-from);
            string val=(i+2>to)?"":Unquote(i+2,to,vars);
            bool existq=vars.ContainsKey(key);
            if(val=="" &&existq) return true; // 空文字追加ならなにもしない
            if(!existq) vars[key]=val; else vars[key]=vars[key]+val;
            envChanged=true;
            return true;
        }
        return false;
    }
    // 変数置換 ＆ エスケープ文字消去 ＆ 引用符消去
    private string Unquote(int from, int to,Dictionary<string,string> vars){
        StringBuilder sb=new StringBuilder();
        char[] buf=new char[to-from+1];
        int bi=0, i=from;
		while(i<=to){
            if((qm[i]&varname)>0){
                int start=i;
                if (bi>0){ sb.Append(buf,0,bi); bi=0; }
                for(i++; i<=to; i++) if((qm[i]&varname)==0) break;
                if(vars==null) continue;
                string key=new string(cha,start,i-start);
                if(vars.ContainsKey(key)) sb.Append(vars[key]);
                continue;
            }else if((qm[i]&trash)==0) buf[bi++]=cha[i];
            i++;
		}
		if (bi>0) sb.Append(buf,0,bi);
        return sb.ToString();
    }
}

public static class ParseUtil {
    public static string error="";
    public static bool IsWordChar(char c){ return (char.IsLetterOrDigit(c)||c=='_'); }
    public static bool IsVar1Char(char c){ return (c=='`'||c=='#'||c=='?'); }
    public static bool IsWord(string s,int from=0,int len=0){
        if(len==0){
            for(int i=from; i<s.Length; i++) if(!IsWordChar(s[i])) return false;
        }else{
            for(int i=from; i<from+len; i++) if(!IsWordChar(s[i])) return false;
        }
        return true;
    }
    public static bool IsVarName(string s,int from=0,int len=0){
        if(len==0){
            if(s.Length==1 && (IsVar1Char(s[from])||IsWordChar(s[from]))) return true;
            for(int i=from; i<s.Length; i++) if(!IsWordChar(s[i])) return false;
        }else{
            if(len==1 && (IsVar1Char(s[from])||IsWordChar(s[from]))) return true;
            for(int i=from; i<from+len; i++) if(!IsWordChar(s[i])) return false;
        }
        return true;
    }

    // SplitやTrimの引数が配列とparam配列の２通りしかないので
    public static char[] comma={','};
    public static char[] colon={':'};
    public static char[] space={' '};
    public static char[] tab={'\t'};
    public static char[] lf={'\n'};
    public static char[] cr={'\r'};
    public static char[] crlf={'\r','\n'};

    public static string[] NormalizeParams(List<string> args,string[] dflt,int start=0){
        string[] ret=new string[dflt.Length];
        int n=args.Count-start;
        if(ret.Length<=n) n=ret.Length;
        for(int i=0; i<n; i++) ret[i]=args[i+start];
        for(int i=n; i<ret.Length; i++){
            if(dflt[i]==null) { error="パラメータの書式が不正です"; return null; }
            ret[i]=dflt[i];
        }
        return ret;
    }
    public static string[] NormalizeParams(string[] args,string[] dflt,int start=0){
        string[] ret=new string[dflt.Length];
        int n=args.Length-start;
        if(ret.Length<=n) n=ret.Length;
        for(int i=0; i<n; i++) ret[i]=args[i+start];
        for(int i=n; i<ret.Length; i++){
            if(dflt[i]==null) { error="パラメータの書式が不正です"; return null; }
            ret[i]=dflt[i];
        }
        return ret;
    }
    public static Dictionary<string,string> GetParam(HashSet<string> names,List<string> args,int start=0){
        Dictionary<string,string> ret=new Dictionary<string,string>();
        string key="";
        for(int i=start; i<args.Count; i++){
            if(key==""){
                if(names.Contains(args[i])) key=args[i];
                else{ error="パラメータが不正です"; return null;}
            }else{ ret[key]=args[i]; key=""; }
        }
        if(key!="") ret[key]=null;
        return ret;
    }

    public static int FindStr(string[] arr,string tgt){
        for(int i=0; i<arr.Length; i++) if(arr[i]==tgt) return i;
        return -1;
    }
    public static Dictionary<string,float> GetKVFloat(string str){
        Dictionary<string,float> ret=new Dictionary<string,float>();
        string[] kva=str.Split(comma);        // valueが数値固定なのでsplitで充分
        for(int i=0; i<kva.Length; i++){
            int eqpos=kva[i].IndexOf('=');
            if(eqpos<=0) { error="値をkey=valueの形式で指定してください"; return null; }
            string key=kva[i].Substring(0,eqpos);
            float v=ParseFloat(kva[i].Substring(eqpos+1));
            if(float.IsNaN(v)){ error="数値の形式が不正です"; return null; }
            ret[key]=v;
        }
        return ret;
    }
    public static Dictionary<string,string> GetKVNum(string str){   // 数値用
        Dictionary<string,string> ret=new Dictionary<string,string>();
        string[] kva=str.Split(comma);        // valueは数値なのでsplitで充分
        for(int i=0; i<kva.Length; i++){
            int eqpos=kva[i].IndexOf('=');
            if(eqpos<=0) { error="値をkey=valueの形式で指定してください"; return null; }
            ret[kva[i].Substring(0,eqpos)]=kva[i].Substring(eqpos+1);
        }
        return ret;
    }
    public static int GetLetterFloat(string str, char[] letter,float[] ret){
        char[] numbuf=new char[str.Length];
        int numlen=0;
        int key=0;
        int i=0,j;
        while(i<str.Length){
            char c=str[i++];
            for(j=0; j<letter.Length; j++) if(letter[j]==c) break;
            if(j<letter.Length){
                if(numlen>0){
                    float v=ParseFloat(new String(numbuf,0,numlen));
                    if(float.IsNaN(v)){ error="数値の形式が不正です"; return -1; }
                    ret[key]=v;
                }
                key=j;
                numlen=0;
                while(i<str.Length){
                    c=str[i];
                    if(char.IsDigit(c)||c=='.'||c=='-'||c=='+'){
                        numbuf[numlen++]=c;
                        i++;
                    }else break;
                }
                if(numlen==0){ error="書式が不正です"; return -1; }
            }else{ error="書式が不正です"; return -1; }
        }
        if(numlen>0){
            float v=ParseFloat(new String(numbuf,0,numlen));
            if(float.IsNaN(v)){ error="数値の形式が不正です"; return -1; }
            ret[key]=v;
        }
        return 0;
    }
    public static float[] MinMax(string str){
        if(str==null||str.Length==0) return null;
        float[] ret=new float[2];
        int n=XyzSub(str,ret);
        if(n==2){
            if(ret[0]>ret[1]){ float t=ret[0]; ret[0]=ret[1]; ret[1]=t; }
            return ret;
        }else if(n==1){ ret[1]=ret[0]; return ret; }
        else { error="数値の形式が不正です"; return null; }
    }

    public static float[] Xy(string str){
        if(str==null||str.Length==0) return null;
        float[] ret=new float[2];
        int n=XyzSub(str,ret);
        if(n==2) return ret;
        else { error="座標の形式が不正です"; return null; }
    }
    public static float[] XyzR(string str,out bool relativeq){
        relativeq=(str!=null && str.Length>0 && str[0]=='+');
        float[] ret=new float[3];
        int n=XyzSub(relativeq?str.Substring(1):str,ret);
        if(n==3) return ret;
        else { error="座標の形式が不正です"; return null; }
    }
    public static float[] Xyz(string str){
        float[] ret=new float[3];
        int n=XyzSub(str,ret);
        if(n==3) return ret;
        else { error="座標の形式が不正です"; return null; }
    }
    public static float[] Xyz(string[] sa){
        float[] ret=new float[3];
        for(int i=0; i<ret.Length; i++){
            ret[i]=ParseFloat(sa[i]);
            if(float.IsNaN(ret[i])){ error="座標の形式が不正です"; return null; }
        }
        return ret;
    }
    public static int XyzSub(string str,float[] ret){
        if(str==null||str.Length==0) return 0;
        string[] sa=str.Split(comma);
        if(sa.Length>ret.Length) return -1;
        for(int i=0; i<sa.Length; i++){
            ret[i]=ParseFloat(sa[i]);
            if(float.IsNaN(ret[i])) return -1;
        }
        return sa.Length;
    }
    public static float[] Xyz2(string str){
        float[] ret=new float[3];
        int n=XyzSub(str,ret);
        if(n==3) return ret;
        else if(n==1){ ret[1]=ret[2]=ret[0]; return ret; }
        else { error="座標の形式が不正です"; return null; }
    }
    public static float[] Quat(string[] str){
        string s0=str[0];
        if(s0.Length==0) return null;
        int i;
        for(i=0; i<s0.Length; i++) if(s0[i]!='~') break;
        bool invq=(i%2)==1;
        if(i>0) s0=s0.Substring(i);
        float[] ret=new float[4]{
            ParseFloat(s0),ParseFloat(str[1]),ParseFloat(str[2]),ParseFloat(str[3])
        };
        if(float.IsNaN(ret[0])||float.IsNaN(ret[1])||float.IsNaN(ret[2])||float.IsNaN(ret[3])){
            error="四元数の形式が不正です";
            return null; 
        }
        if(invq){ ret[0]*=-1; ret[1]*=-1; ret[2]*=-1; }
        return ret;
    }
    public static float[] Quat(string str){
        if(str==null||str.Length==0) return null;
        string[] sa=str.Split(comma);
        if(sa.Length!=4){ error="四元数の形式が不正です"; return null; }
        return Quat(sa);
    }
    public static float[] QuatR(string str,out byte relative){
        relative=0;
        if(str==null||str.Length==0) return null;
        if(str[0]=='+'||str[0]=='L'||str[0]=='l'){
            relative=1;
            return Quat(str.Substring(1));
        }else if(str[0]=='R'||str[0]=='r'){
            relative=2;
            return Quat(str.Substring(1));
        }
        return Quat(str);
    }
    public static float[] RotR(string str,out byte relative){
        relative=0;
        if(str==null||str.Length==0) return null;
        if(str[0]=='+'||str[0]=='L'||str[0]=='l'){
            relative=1;
            return Xyz(str.Substring(1));
        }else if(str[0]=='R'||str[0]=='r'){
            relative=2;
            return Xyz(str.Substring(1));
        }
        return Xyz(str);
    }
    public static float[] FloatArr(string str){
        if(str==null||str.Length==0) return new float[0];
        string[] sa=str.Split(comma);
        if(sa.Length==4 && str[0]=='~') return Quat(sa);
        float[] ret= new float[sa.Length];
        for(int i=0; i<sa.Length; i++){
            ret[i]=ParseFloat(sa[i]);
            if(float.IsNaN(ret[i])){ error="数値の形式が不正です"; return null;}
        }
        return ret;
    }
    public static float[] Rgb(string str){
        if(str.IndexOf(',')>=0) return Rgb2(str);
        float[] ret=new float[3];
        if(!int.TryParse(str,System.Globalization.NumberStyles.HexNumber,null,out int rgb)){
            error="色指定が不正です";
            return null;
        }
        ret[0]=(rgb>>16)/255f;
        ret[1]=((rgb>>8)&255)/255f;
        ret[2]=(rgb&255)/255f;
        return ret;
    }
    public static float[] Rgb2(string str){
        float[] ret=new float[3];
        int n=XyzSub(str,ret);
        if(n!=3){ error="色指定が不正です"; return null; }
        if(ret[0]<0||ret[0]>1||ret[1]<0||ret[1]>1||ret[2]<0||ret[2]>1){
            error="r,g,bの値は0.0～1.0の範囲で指定してください";
            return null;
        }
        return ret;
    }
    public static float[] Rgba(string str){
        float[] ret=new float[4];
        int n=XyzSub(str,ret);
        if(n!=3&&n!=4){ error="色指定が不正です"; return null; }
        if(n==3) ret[3]=1.0f;
        if(ret[0]<0||ret[0]>1||ret[1]<0||ret[1]>1||ret[2]<0||ret[2]>1||ret[3]<0||ret[3]>1){
            error="r,g,b,aの値は0.0～1.0の範囲で指定してください";
            return null;
        }
        return ret;
    }
    public static float[] RgbaLenient(string str){
        float[] ret=new float[4];
        int n=XyzSub(str,ret);
        if(n!=3&&n!=4){ error="色指定が不正です"; return null; }
        if(n==3) ret[3]=1.0f;
        // 範囲チェックしない
        return ret;
    }
    public static int OnOff(string str){
        if(str=="on") return 1;
        else if(str=="off") return 0;
        else { error="onまたはoffを指定してください"; return -1; }
    }
    private static string[] nthRangeDlm={""};
    public static List<string> NthRange(string txt,string dlm,string range){
        if(txt=="") return new List<string>();
        nthRangeDlm[0]=dlm;
        string[] ta=txt.Split(nthRangeDlm,StringSplitOptions.None);
        int tl=ta.Length;
        string[] ra=range.Split(comma);
        if(ra.Length==0){ error="範囲の指定が不正です"; return null; }
        List<string> sa=new List<string>();
        for(int i=0; i<ra.Length; i++){
            if(!int.TryParse(ra[i],out int n)||n==0){ error="数値の指定が不正です"; return null;}
            if(n<-tl||n>tl){ sa.Add(""); continue; }
            if(n<0) n=tl+n; else n--;
            sa.Add(ta[n]);
        }
        return sa;
    }
    public static string Nth(string txt,string dlm,int n){
        int cnt=1;
        int[] pa=new int[3];
        while(CutNext(txt,dlm,pa)){
            if(cnt==n) return txt.Substring(pa[0],pa[1]);
            cnt++;
        }
        return null;
    }
    public static bool CutNext(string txt,string dlm,int[] pa){
        if(pa[2]==-1||pa[2]>=txt.Length) return false;
        pa[0]=pa[2];
        if((pa[1]=txt.IndexOf(dlm,pa[0],Ordinal))>=0){
            pa[2]=pa[1]+1; // 次開始位置
            pa[1]=pa[1]-pa[0]; // 長さ
        }else{
            pa[1]=txt.Length-pa[0];
            pa[2]=-1;
        }
        return true;
    }
    public static string Nth(string txt,char dlm,int n){
        int cnt=1;
        int[] pa=new int[3];
        while(CutNext(txt,dlm,pa)){
            if(cnt==n) return txt.Substring(pa[0],pa[1]);
            cnt++;
        }
        return null;
    }
    public static bool CutNext(string txt,char dlm,int[] pa){
        if(pa[2]==-1||pa[2]>=txt.Length) return false;
        if(pa[2]==-1) return false;
        pa[0]=pa[2];
        if((pa[1]=txt.IndexOf(dlm,pa[0]))>=0){
            pa[2]=pa[1]+1; // 次開始位置
            pa[1]=pa[1]-pa[0]; // 長さ
        }else{
            pa[1]=txt.Length-pa[0];
            pa[2]=-1;
        }
        return true;
    }
    public static string LeftOf(string txt,char dlm){
        int idx=txt.IndexOf(dlm);
        if(idx<0) return txt;
        return txt.Substring(0,idx);
    }
    public static string RightOf(string txt,char dlm){
        int idx=txt.IndexOf(dlm);
        if(idx<0) return txt;
        return txt.Substring(idx+1);
    }
    public static string[] LeftAndRight(string txt,char dlm){
        int idx=txt.IndexOf(dlm);
        if(idx<0) return new string[]{ txt,"" };
        return new string[]{ txt.Substring(0,idx),txt.Substring(idx+1) };
    }
    public static int CountChar(string txt,char c){
        int ret=0;
        for(int i=0; i<txt.Length; i++) if(txt[i]==c) ret++;
        return ret;
    }
    public static string[] SplitApBoneName(string str){
        string[] sa=str.Split(colon);
        if(sa.Length!=3) { error="ボーンまたはアタッチポイントの指定に誤りがあります"; return null; };
        if(IsWordChar(sa[2][0])) sa[2]=CompleteBoneName(sa[2],sa[0]=="man");
        return sa;
    }
    /* 例えば Bip01 L UpperArm をBLUpperArm と書けるようにする
       Bip01 Spine1 ならBSpine1。ちょっとだけ短い＆空白エスケープ不要＆男女書き分け不要に
       もしBip01 [LR]\S+ に該当するボーンが現れたら破綻する */
    public static string CompleteBoneName(string shortname,bool manq, bool nayose=true){
        if(shortname.Length>1 && char.IsUpper(shortname[1])){
            string root,lr="",uname;
            int n=1;
            if(shortname[0]!='B') return shortname;
            root=manq?"ManBip":"Bip01";
            if(shortname[1]=='L'){ lr=" L"; n++; }
            else if(shortname[1]=='R'){ lr=" R"; n++; }
            uname=shortname.Substring(n);
            if(nayose && manq){
                if(uname=="Spine0a") uname="Spine1";        // Spine0aはないのでSpine1に名寄せ
                else if(uname=="Spine1a") uname="Spine2";   // Spine1aはSpine2に該当
                else if(uname.StartsWith("Toe",Ordinal)){
                    if(uname[3]=='2') uname="Toe1";         // Toe2はないのでToe1に名寄せ
                    else if(uname.Length==5) uname=uname.Substring(0,4); // Toe\d\dはないのでToe\dに名寄せ
                }
            }
            return $"{root}{lr} {uname}";
        }else return shortname;
    }
    public static string CompactBoneName(string name){
        string[] sa=name.Split(space);
        if(sa.Length==1) return name;
        if(sa[0]=="Bip01"||sa[0]=="ManBip") sa[0]="B";
        return string.Join("",sa);
    }

    public static float ParseFloat(string str,float dflt=float.NaN){
        if(string.IsNullOrEmpty(str)) return dflt;
        // $xが負でも -$x と書けるように
        int i;
        string s=str;
        for(i=0; i<s.Length; i++) if(s[i]!='-') break;
        bool minus=(i%2)==1;
        if(i>0) s=s.Substring(i);
        bool ok=float.TryParse(s,out float ret);
        if(minus) ret*=-1;
        return ok?ret:dflt;
    }
    public static int ParseInt(string str,int dflt=int.MinValue){
        if(string.IsNullOrEmpty(str)) return dflt;
        // $xが負でも -$x と書けるように
        int i;
        string s=str;
        for(i=0; i<s.Length; i++) if(s[i]!='-') break;
        bool minus=(i%2)==1;
        if(i>0) s=s.Substring(i);
        bool ok=int.TryParse(s,out int ret);
        if(minus) ret*=-1;
        return ok?ret:dflt;
    }
}
}
