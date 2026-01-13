using System;
using System.Collections.Generic;
using System.Text;
using static System.StringComparison;

namespace COM3D2.ComSh.Plugin {

// パーサ
public class ComShParser {

    public class Statement {
        public bool noerr=false;
        public int offset=0;
        public List<Token> tokens=new List<Token>();
        public char eol=';';
        public Token redirect=null;
        public bool append=false;
        public static Statement zero=new Statement();
    }
    public class Token {
        public int head=-1;
        public int len=0;
        public string txt=null;
        public Assign assign=null;
        public int varies=0;
        public ComShParser parser=null;
        public Token(int h){head=h; len=0;}
        public Token(int h,int l){head=h; len=l;}
        public Token(char[] ch,int h,int l){head=h;len=l;txt=new string(ch,h,l);}
    }
    public class Assign {
        public string key;
        public string value;
        public bool append=false;
        public Assign(string k,string v,bool a){key=k;value=v;append=a;}
    }
    public List<Statement> sta=new List<Statement>();
    private char[] cha;
    private byte[] qm;
    private int chalen=0;

	public const byte trash = 1;		// 出力しない文字
	public const byte escaped = 2;	    // エスケープされた文字。単一引用符内もこれ
    public const byte varname = 4;      // 変数置換記述の変数名
    public const byte eos = 8;          // 文終端
    public const byte redirect = 16;        // 文終端

    public string error;                // パース時にエラーがあればエラーメッセージが入る
    public char prevEoL;                // 直前に取り出した行の行末記号(;/|)
    public Statement currentStatement=Statement.zero;

    public int lineno=0;
    private int sno=-1;

    public ComShParser(){}
    public ComShParser(int l){lineno=l;}

    public int Parse(string text){
        sta.Clear();
        Reset();
        return Analyze(text);
    }
    public void Reset(char eol){Reset();prevEoL=eol;}
    public void Reset(){
        error=null;
        prevEoL=';';
        sno=-1;
        currentStatement=Statement.zero;
    }

    private VarDic lvars;
    private Dictionary<string,string> svars;
    public List<string> Next(VarDic lv, Dictionary<string,string> sv){
        if(++sno>=sta.Count) return null;
        lvars=lv; svars=sv;
        prevEoL=currentStatement.eol;
        currentStatement=sta[sno];
        return SingleLine(currentStatement);
    }
    private int Analyze(string txt){
        if(txt.Length==0) return 0;
        {
            int head,tail;
            for(head=0; head<txt.Length; head++)
                if(txt[head]!=' '&&txt[head]!='\t') break; // TrimStart(' ','\t')
            if(head>=txt.Length) return 0;
            if(txt[head]=='#') return 0;
            for(tail=txt.Length-1; tail>=head; tail--)
                if(txt[tail]!='\n'&&txt[tail]!='\r') break; // TrimEnd('\n','\r')
            if(tail<head) return 0;
            cha=new char[tail-head+1+1];
            txt.CopyTo(head,cha,0,tail-head+1);
            cha[tail-head+1]=';'; // 終端
            qm=new byte[cha.Length];
        }

        // バックスラッシュおよび引用符の処理
        bool backslash=false;
        int quote=0;
        int blv=0;
        char prev_ch='\0';
        byte prev_qm=0;
        chalen=cha.Length;
        for(int i=0; i<cha.Length; i++){
            if(quote==0){                 // 引用符外
                if(backslash){
                    if(cha[i]=='n') cha[i]='\n';
                    else if(cha[i]=='t') cha[i]='\t';
                    qm[i]=escaped;
                    backslash=false; 
                } else if(cha[i]=='\\'){ backslash=true; qm[i]=trash; }
                else if(cha[i]=='\t') cha[i]=' ';
                else if(cha[i]=='\''){ quote=1; qm[i]=trash; }
                else if(cha[i]=='"') { quote=2; qm[i]=trash; }
                else if(cha[i]=='{'&&(prev_qm==escaped||prev_ch!='$')){ quote=3; blv=1; qm[i]=trash; }
                else if(cha[i]=='#'&&prev_qm!=escaped&&IsSeam(prev_ch)){ cha[i]=';'; qm[i]=eos; chalen=i+1; break; }
                else if(cha[i]=='>'){ qm[i]=redirect; }
                else if(cha[i]=='|'||cha[i]==';'){ qm[i]=eos; }
            }else if(quote==1){           // 単一引用符内
                if(cha[i]=='\''){ quote=0; qm[i]=trash; }
                else qm[i]=escaped;
            }else if(quote==2){           // 二重引用符内
                if(backslash){
                    if(cha[i]=='n') {cha[i]='\n'; qm[i-1]=trash;}
                    else if(cha[i]=='t'){ cha[i]='\t'; qm[i-1]=trash;}
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
            prev_ch=cha[i]; prev_qm=qm[i];
        }   
        if(quote==1){error="単一引用符が閉じられていません"; return -1; }
        if(quote==2){error="二重引用符が閉じられていません"; return -1; }
        if(quote==3){error="{～}が閉じられていません"; return -1; }
        if(backslash){ error="バックスラッシュに続く文字がありません"; return -1; }

        for(int i=chalen-1; i>=0; i--) if(cha[i]==' '&&(qm[i]&escaped)==0) chalen--; else break;
        if(chalen==0) return 0;

        return Tokenize();
	}
    private bool IsSeam(char c){ return (c==' '||c==';'||c=='|'||c=='?'); }
    private int Tokenize(){
        int start=0;
        int rd=-1;
        for(int i=0; i<chalen; i++) if((qm[i]&escaped)==0){
            if(cha[i]=='>'){ if(rd<0) rd=i; }
            else if(cha[i]==';'||cha[i]=='|'){
                if(TokenizeStatement(start,i,rd)<0) return -1;
                start=i+1;
                rd=-1;
            }
        }
        return (sta.Count>0)?1:0;
    }
    private int TokenizeStatement(int head,int tail,int rd){
        Statement st=new Statement();
        int tail2=rd<0?tail:rd;
        st.eol=cha[tail2];
        bool whiteq=true;
        Token token=null;
        for(int i=head; i<=tail2; i++) if((qm[i]&escaped)==0){
            if(cha[i]==' '||cha[i]==';'||cha[i]=='|'||cha[i]=='>'){
                if(!whiteq){
                    if(token!=null){ token.len=i-token.head; token=null; }
                    whiteq=true;
                }
            }else{
                if(whiteq){
                    token=new Token(i);
                    st.tokens.Add(token);
                    whiteq=false;
                }
            }
        }
        if(rd>=0){
            st.append=false;
            int i=rd;
            if(cha[++i]=='>'){ i++; st.append=true;}    // >>を認識
            for(; i<=tail; i++) if(cha[i]!=' ') break;
            if(i>tail){ error="リダイレクトの書式が不正です";return -1; }
            int start=i;
            i=MarkWord(i,tail);
            if(start==i){ error="リダイレクトの書式が不正です";return -1; }
            st.redirect=new Token(cha,start,i-start);
            for(; i<=tail; i++) if((cha[i]==';') && (qm[i]&escaped)==0) break; else{
                if((qm[i]&escaped)!=0||cha[i]!=' '){ error="リダイレクトの書式が不正です";return -1; }
            }
        }

        // 変数置換
        foreach(var tok in st.tokens){
            tok.varies=0;
            int toktail=tok.head+tok.len-1;
            int varcnt=0;
            for(int i=tok.head; i<toktail; i++){
                if(cha[i]=='$' && (qm[i]&escaped)==0) {
                    int j=i+1;  // この時点ではまだただの'$'かもしれない
                    if(cha[j]=='{'){ // ${\(w+)}
                        qm[i]=trash; qm[j]|=trash;  // '{'が来た時点で'$'は変数置換用と確定
                        tok.varies=1; varcnt++;
                        if(j++==tail) { error="${～}が閉じられていません"; return -1; }
                        if(ParseUtil.IsVar1Char(cha[j])) qm[j++]|=varname; else j=MarkWord(j,tail);
                        if(j>tail) { error="${～}が閉じられていません"; return -1; }
                        if(cha[j]!='}' || j==i+2){ error="${～}の書式が不正です"; return -1; }
                        qm[j]|=trash; i=j;
                    }else{                          // $(\w+)
                        if(ParseUtil.IsVar1Char(cha[j])) qm[j++]|=varname; else j=MarkWord(j,tail);
                        if(j>i+1){ // 変数名があれば'$'は変数置換用と確定
                            tok.varies=1; varcnt++;
                            qm[i]|=trash; i=j-1; 
                        }
                    }
                }
            }
            if(tok.varies==1 && varcnt==1){
                tok.varies=2;
                for(int i=tok.head; i<tok.head+tok.len; i++)
                    if((qm[i]&(varname|trash))==0){tok.varies=1; break;}
            }
        }
        if(st.tokens.Count>0) sta.Add(st);
        return 0;
    }
    private int GetWord(int i0,int tail){
        int i;
        if(cha[i0]=='.'){   // \.\w+
            for(i=i0+1; i<=tail; i++) if(!ParseUtil.IsWordChar(cha[i])) break;
            if(i==i0+1) return i0; else return i;
        }else if(cha[i0]=='/'){  // /[\w/]*\w+
            int w=i0;
            for(i=i0+1; i<=tail; i++) if(ParseUtil.IsWordChar(cha[i])) w=i; else if(cha[i]!='/') break;
            if(w==i0) return i0; else return w+1;
        }else{
            for(i=i0; i<=tail; i++) if(!ParseUtil.IsWordChar(cha[i])) break;
            return i;
        }
    }
    private int MarkWord(int i0,int tail){
        int e=GetWord(i0,tail);
        for(int i=i0; i<e; i++) qm[i]|=varname;
        return e;
    }

    // escapedでない空白文字で区切られたトークン達を得る
	private List<string> SingleLine(Statement st) {
		List<string> tokens=new List<string>(st.tokens.Count);
        int i; for(i=0; i<st.tokens.Count; i++) if(!Keyval(st.tokens[i])) break;
        currentStatement.offset=i;
        for(; i<st.tokens.Count; i++) tokens.Add(Unquote(st.tokens[i]));
        return tokens;
    }
	public void Redirect(){
        var rd=sta[sno].redirect;
        if(rd==null) return;
        string key=rd.txt;
        string val=(string)lvars.output;
        if(sta[sno].append) Variables.Append(key,val,lvars,svars);
        else Variables.Set(key,val,lvars,svars);
        lvars.output="";
    }
    // 変数代入文の処理　
    private bool Keyval(Token tok){
        int from=tok.head,to=tok.head+tok.len-1;
        string key=null,value=null;
        bool append=false;
        if(tok.varies>0 || tok.assign==null){
            int i=GetWord(from,to);
            if(i==from||i>to) return false;
            if(cha[i]=='=' && qm[i]==0){
                if(lvars==null) return true;
                key=new string(cha,from,i-from);
                value=(i+1>to)?"":Unquote(i+1,to);
            }else if(i<to && cha[i]=='+' && qm[i]==0 && cha[i+1]=='=' && qm[i+1]==0){
                if(lvars==null) return true;
                append=true;
                key=new string(cha,from,i-from);
                value=(i+2>to)?"":Unquote(i+2,to);
            }
            if(tok.assign==null) tok.assign=new Assign(key,value,append);
            else{tok.assign.key=key; tok.assign.value=value; tok.assign.append=append;}
        }else{ key=tok.assign.key; value=tok.assign.value; append=tok.assign.append; }
        if(key==null) return false;
        if(append){
            Variables.Append(key,value,lvars,svars);
            return true;
        }else{
            Variables.Set(key,value,lvars,svars);
            return true;
        }
    }
    // 変数置換 ＆ エスケープ文字消去 ＆ 引用符消去
    private string Unquote(Token tok){
        if(tok.varies>0 || tok.txt==null){
            string txt=tok.varies==2?Unquote2(tok.head,tok.head+tok.len-1):Unquote(tok.head,tok.head+tok.len-1);
            tok.txt=txt;
        }
        return tok.txt;
    }
    private string Unquote(int from,int to){
        StringBuilder sb=new StringBuilder(to-from+1);
        char[] buf=new char[to-from+1];
        int bi=0,i=from;
		while(i<=to){
            if((qm[i]&varname)>0){
                int start=i;
                if (bi>0){ sb.Append(buf,0,bi); bi=0; }
                for(i++; i<=to; i++) if((qm[i]&varname)==0) break;

                if(lvars==null) continue;
                if(cha[start]=='.' && svars==null) continue;

                string key=new string(cha,start,i-start);
                if(cha[start]=='/'){
                    if(Variables.g.TryGetValue(key,out string s)) sb.Append(s);
                }else if(cha[start]=='.'){
                    if(svars.TryGetValue(key,out string s)) sb.Append(s);
                }else{
                    sb.Append(lvars[key]);
                }
                continue;
            }else if((qm[i]&trash)==0) buf[bi++]=cha[i];
            i++;
		}
		if (bi>0) sb.Append(buf,0,bi);
        return sb.ToString();
    }
    // 変数１つだけで余計な文字もない場合、変数の値をそのまま返す(複製を作らない)
    private string Unquote2(int from,int to){
        if(lvars==null) return "";
        int i,st=-1,ed=-1;
		for(i=from; i<=to; i++) if((qm[i]&varname)>0){st=ed=i;break;}
		for(i++; i<=to; i++) if((qm[i]&varname)>0) ed=i; else break;
        if(st<0) return "";
        if(cha[st]=='/'){
            string key=new string(cha,st,ed-st+1);
            return Variables.g.TryGetValue(key,out string s)?s:"";
        }else if(cha[st]=='.'){
            if(svars==null) return "";
            string key=new string(cha,st,ed-st+1);
            return svars.TryGetValue(key,out string s)?s:"";
        }else{
            string key=new string(cha,st,ed-st+1);
            return lvars[key];
        }
    }
}

public static class ParseUtil {
    public static List<string> emptyList=new List<string>();
    public static List<StrSegment> emptysegList=new List<StrSegment>();
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
        int l=(len==0)?s.Length-from:len;
        if(l==0) return false;
        if(l==1 && (IsVar1Char(s[from])||IsWordChar(s[from]))) return true;
        int i;
        if(s[from]=='/'){  // /[\w/]*\w+
            for(i=from+1; i<from+l; i++) if(!IsWordChar(s[i])&&s[i]!='/') return false;
            if(s[from+l-1]=='/') return false;
        }else if(s[from]=='.'){ // \.\w+
            for(i=from+1; i<from+l; i++) if(!IsWordChar(s[i])) return false;
        }else for(i=from; i<from+l; i++) if(!IsWordChar(s[i])) return false;
        return true;
    }
    public static bool IsVar1Name(string s,int from=0,int len=0){
        int l=(len==0)?s.Length-from:len;
        if(l!=1) return false;
        return IsVar1Char(s[from]);
    }
    public static bool IsLVarName(string s,int from=0,int len=0){
        int l=(len==0)?s.Length-from:len;
        if(l==0) return false;
        if(l==1 && (IsVar1Char(s[from])||IsWordChar(s[from]))) return true;
        int i;
        if(s[from]=='/' || s[from]=='.') return false;
        for(i=from; i<from+l; i++) if(!IsWordChar(s[i])) return false;
        return true;
    }
    public static bool IsSVarName(string s,int from=0,int len=0){
        int l=(len==0)?s.Length-from:len;
        if(l==0) return false;
        int i;
        if(s[from]!='.') return false;
        for(i=from+1; i<from+l; i++) if(!IsWordChar(s[i])) return false;
        return true;
    }
    public static bool IsGVarName(string s,int from=0,int len=0){
        int l=(len==0)?s.Length-from:len;
        if(l==0) return false;
        int i;
        if(s[from]!='/') return false;
        for(i=from+1; i<from+l; i++) if(!IsWordChar(s[i])&&s[i]!='/') return false;
        if(s[from+l-1]=='/') return false;
        return true;
    }

    // SplitやTrimの引数が配列とparam配列の２通りしかないので
    public static char[] comma={','};
    public static char[] commaslash={',','/'};
    public static char[] commaslashlf={',','/','\n'};
    public static char[] period={'.'};
    public static char[] colon={':'};
    public static char[] space={' '};
    public static char[] tab={'\t'};
    public static char[] lf={'\n'};
    public static char[] cr={'\r'};
    public static char[] crlf={'\r','\n'};
    public static char[] eqcln={':','='};
    public static char[] eqcln2={'=',':'};

    public static string[] NormalizeParams(List<string> args,string[] dflt,int start=0){
        return NormalizeParams(args,dflt,new string[dflt.Length],start);
    }
    public static string[] NormalizeParams(List<string> args,string[] dflt,string[] buf,int start=0){
        string[] ret=buf;
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
        return NormalizeParams(args,dflt,new string[dflt.Length],start);
    }
    public static string[] NormalizeParams(string[] args,string[] dflt,string[] buf,int start=0){
        string[] ret=buf;
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
        var kva=StrSegment.Split(str,comma);
        for(int i=0; i<kva.Count; i++){
            int eqpos=kva[i].IndexOf('=');
            if(eqpos<=0) { error="値をkey=valueの形式で指定してください"; return null; }
            string key=kva[i].Substr(0,eqpos);
            if(!TryParseFloat(kva[i].Slice(eqpos+1),out float v)){ error="数値が不正です"; return null; }
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
                    if(float.IsNaN(v)){ error="数値が不正です"; return -1; }
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
            if(float.IsNaN(v)){ error="数値が不正です"; return -1; }
            ret[key]=v;
        }
        return 0;
    }
    public struct Arr2<T>{
        public bool ok;
        public bool ng {get=>!ok;}
        public T x,y;
        public T this[int idx]{ get{ return (idx==0)?x:y;}}
        public Arr2(T a,T b){x=a; y=b; ok=true;}
    }
    public struct Arr3<T>{
        public bool ok;
        public bool ng {get=>!ok;}
        public T x,y,z;
        public T this[int idx]{ get{ return idx==0?x:(idx==1?y:z);}}
        public Arr3(T a,T b,T c){x=a; y=b; z=c; ok=true;}
    }
    public struct Arr4<T>{
        public bool ok;
        public bool ng {get=>!ok;}
        public T x,y,z,w;
        public T this[int idx]{ get{ return idx<=1?(idx==0?x:y):(idx==2?z:w);}}
        public Arr4(T a,T b,T c,T d){x=a; y=b; z=c; w=d; ok=true;}
    }
    public static Arr2<float> MinMax(string str){
        if(str==null||str.Length==0) return default;
        int n=XyzSub(str,xyzbuf,2);
        if(n==2){
            if(xyzbuf[0]>xyzbuf[1]) return new Arr2<float>(xyzbuf[1],xyzbuf[0]);
            return new Arr2<float>(xyzbuf[0],xyzbuf[1]);
        }else if(n==1) return new Arr2<float>(xyzbuf[0],xyzbuf[0]);
        else { error="数値が不正です"; return default; }
    }
    public static Arr2<double> MinMaxW(string str){
        if(str==null||str.Length==0) return default;
        int n=XyzSubW(str,xyz_w_buf,2);
        if(n==2){
            if(xyz_w_buf[0]>xyz_w_buf[1]) return new Arr2<double>(xyz_w_buf[1],xyz_w_buf[0]);
            return new Arr2<double>(xyz_w_buf[0],xyz_w_buf[1]);
        }else if(n==1) return new Arr2<double>(xyz_w_buf[0],xyz_w_buf[0]);
        else { error="数値が不正です"; return default; }
    }

    public static Arr2<float> Xy(string str){
        if(str==null||str.Length==0) return default;
        float[] ret=xyzbuf;
        int n=XyzSub(str,ret,2);
        if(n==2) return new Arr2<float>(xyzbuf[0],xyzbuf[1]);
        else { error="座標が不正です"; return default; }
    }
    public static Arr3<float> XyzR(string str,out bool relativeq){
        relativeq=(str!=null && str.Length>0 && str[0]=='+');
        float[] ret=xyzbuf;
        int n=XyzSub(new StrSegment(str,relativeq?1:0),ret,3);
        if(n==3) return new Arr3<float>(xyzbuf[0],xyzbuf[1],xyzbuf[2]);
        else { error="座標が不正です"; return default; }
    }
    public static Arr3<float> Xyz(string str){ return Xyz(new StrSegment(str)); }
    public static Arr3<float> Xyz(StrSegment seg){
        float[] ret=xyzbuf;
        int n=XyzSub(seg,ret,3);
        if(n==3) return new Arr3<float>(ret[0],ret[1],ret[2]);
        else { error="座標が不正です"; return default; }
    }
    public static Arr3<float> Xyz(string[] sa){
        if(!float.TryParse(sa[0],out float x)
         ||!float.TryParse(sa[1],out float y)
         ||!float.TryParse(sa[2],out float z)){error="座標が不正です"; return default; }
        return new Arr3<float>(x,y,z);
    }
    private static float[] xyzbuf=new float[4];
    public static int XyzSub(string str,float[] ret,int max=int.MaxValue){ return XyzSub(new StrSegment(str),ret,max); }
    public static int XyzSub(StrSegment seg,float[] ret,int max=int.MaxValue){
        if(seg.Length==0) return 0;
        if(ret==null) ret=xyzbuf;
        int n=Math.Min(ret.Length,max);
        var cnxt=new CutNextText(seg);
        int i=0;
        for(; i<n; i++){
            if(CutNext(ref cnxt,',')<0) break;
            if(!TryParseFloat(cnxt.txt.SliceLen(cnxt.head,cnxt.len),out ret[i])) return -1;
        }
        if(i==n-1 && cnxt.next>=0) return -1;
        return i;
    }
    private static double[] xyz_w_buf=new double[4];
    public static int XyzSubW(string str,double[] ret,int max=int.MaxValue){return XyzSubW(new StrSegment(str),ret,max);}
    public static int XyzSubW(StrSegment seg,double[] ret,int max=int.MaxValue){
        if(seg.Length==0) return 0;
        if(ret==null) ret=xyz_w_buf;
        int n=Math.Min(ret.Length,max);
        var cnxt=new CutNextText(seg);
        int i=0;
        for(; i<n; i++){
            if(CutNext(ref cnxt,',')<0) break;
            if(!TryParseDouble(cnxt.txt.SliceLen(cnxt.head,cnxt.len),out ret[i])) return -1;
        }
        if(cnxt.next>=0) return -1;
        return i;
    }
    public static Arr3<float> Xyz2(string str){
        float[] ret=xyzbuf;
        int n=XyzSub(str,ret,3);
        if(n==3) return new Arr3<float>(ret[0],ret[1],ret[2]);
        else if(n==1){ return new Arr3<float>(ret[0],ret[0],ret[0]);}
        else { error="座標が不正です"; return default; }
    }
    public static Arr4<float> Quat(string str){ return Quat(new StrSegment(str)); }
    public static Arr4<float> Quat(StrSegment seg){
        var s0=seg;
        if(s0.Length==0) return default;
        int i;
        for(i=0; i<s0.Length; i++) if(s0[i]!='~') break;
        bool invq=(i%2)==1;
        if(i>0) s0.SliceSelf(i,-1);
        float[] ret=xyzbuf;
        int n=XyzSub(seg,ret,4);
        if(n!=4){
            error="四元数が不正です";
            return default; 
        }
        if(invq){ ret[0]*=-1; ret[1]*=-1; ret[2]*=-1; }
        return new Arr4<float>(ret[0],ret[1],ret[2],ret[3]);
    }
    public static double[] QuatW(string str){
        if(str==null||str.Length==0) return null;
        int i;
        for(i=0; i<str.Length; i++) if(str[i]!='~') break;
        bool invq=(i%2)==1;
        double[] ret=xyz_w_buf;
        var seg=new StrSegment(str,i);
        int n=XyzSubW(seg,ret);
        if(n!=4){ error="四元数が不正です"; return null; }
        if(invq){ ret[0]*=-1; ret[1]*=-1; ret[2]*=-1; }
        return ret;
    }
    public static Arr4<float> QuatR(string str,out byte relative){
        relative=0;
        if(str==null||str.Length==0) return default;
        if(str[0]=='+'||str[0]=='L'||str[0]=='l'){
            relative=1;
            return Quat(new StrSegment(str,1));
        }else if(str[0]=='R'||str[0]=='r'){
            relative=2;
            return Quat(new StrSegment(str,1));
        }
        return Quat(str);
    }
    public static Arr3<float> RotR(string str,out byte relative){
        relative=0;
        if(str==null||str.Length==0) return default;
        if(str[0]=='+'||str[0]=='L'||str[0]=='l'){
            relative=1;
            return Xyz(new StrSegment(str,1));
        }else if(str[0]=='R'||str[0]=='r'){
            relative=2;
            return Xyz(new StrSegment(str,1));
        }
        return Xyz(str);
    }
    public static Arr3<float> WRotR(string str,out byte relative){return WRotR(new StrSegment(str),out relative);}
    public static Arr3<float> WRotR(StrSegment str,out byte relative){
        relative=0;
        if(str.Length==0) return default;
        if(str[0]=='+'||str[0]=='L'||str[0]=='l') relative=1;
        else if(str[0]=='R'||str[0]=='r') relative=2;

        if(relative==0){
            var cd=new ColonDesc(str);
            if(cd.num==0) return Xyz(str);
            var tr=ObjUtil.FindObj(ComShInterpreter.currentSh,cd);
            if(tr==null) return default;
            var ea=tr.rotation.eulerAngles;
            return new Arr3<float>(ea.x,ea.y,ea.z);
        }else return Xyz(str.Slice(1));
    }

    public static Arr3<float> PositionR(string str,out bool relativeq){return PositionR(new StrSegment(str),out relativeq);}
    public static Arr3<float> PositionR(StrSegment str,out bool relativeq){
        relativeq=(str.Length>0 && str[0]=='+');
        return relativeq?Xyz(str.Slice(1)):Position(str);
    }
    public static Arr3<float> Position(string str){return Position(new StrSegment(str));}
    public static Arr3<float> Position(StrSegment str){
        var cd=new ColonDesc(str);
        if(cd.num==0) return Xyz(str);
        var tr=ObjUtil.FindObj(ComShInterpreter.currentSh,cd);
        if(tr==null) return default;
        var p=tr.position;
        return new Arr3<float>(p.x,p.y,p.z);
    }
    public static int PositionR(string str,float[] ret,out bool relativeq){return PositionR(new StrSegment(str),ret,out relativeq);}
    public static int PositionR(StrSegment str,float[] ret,out bool relativeq){
        if(ret==null) ret=xyzbuf;
        relativeq=(str.Length>0 && str[0]=='+');
        return relativeq?XyzSub(str.Slice(1),ret):Position(str,ret);
    }
    public static int Position(string str,float[] ret){return Position(new StrSegment(str),ret);}
    public static int Position(StrSegment str,float[] ret){
        if(ret==null) ret=xyzbuf;
        var cd=new ColonDesc(str);
        if(cd.num==0) return XyzSub(str,ret);
        var tr=ObjUtil.FindObj(ComShInterpreter.currentSh,cd);
        if(tr==null) return 0;
        var pos=tr.position;
        ret[0]=pos.x; ret[1]=pos.y; ret[2]=pos.z;
        return 3;
    }
    public static int CountC(string str,char c){
        int cnt=0;
        for(int i=0; i<str.Length; i++) if(str[i]==c) cnt++;
        return cnt;
    }
    public static int CountC(StrSegment str,char c){
        int cnt=0;
        for(int i=0; i<str.Length; i++) if(str[i]==c) cnt++;
        return cnt;
    }
    public static int CountC(string str,char[] ca){
        int cnt=0;
        for(int i=0; i<str.Length; i++)
            for(int j=0; j<ca.Length; j++) if(str[i]==ca[j]){ cnt++; break; }
        return cnt;
    }
    public static int CountC(StrSegment str,char[] ca){
        int cnt=0;
        for(int i=0; i<str.Length; i++)
            for(int j=0; j<ca.Length; j++) if(str[i]==ca[j]){ cnt++; break; }
        return cnt;
    }
    public static float[] FloatArr(string str){ return FloatArr2(str,',');}
    public static double[] DoubleArr(string str){
        if(str==null||str.Length==0) return new double[0];
        int n=CountC(str,',')+1;
        var cnxt=new CutNextText(str);
        double[] ret= new double[n];
        int i=0;
        for(; i<n; i++){
            if(CutNext(ref cnxt,',')<0) break;
            if(!TryParseDouble(cnxt.txt.SliceLen(cnxt.head,cnxt.len),out ret[i])){
                error="数値が不正です";
                return null;
            }
        }
        return ret;
    }
    public static int[] IntArr(string str){
        if(str==null||str.Length==0) return new int[0];
        int n=CountC(str,',')+1;
        var cnxt=new CutNextText(str);
        int[] ret= new int[n];
        int i=0;
        for(; i<n; i++){
            if(CutNext(ref cnxt,',')<0) break;
            if(!TryParseInt(cnxt.txt.SliceLen(cnxt.head,cnxt.len),out ret[i])){
                error="数値が不正です";
                return null;
            }
        }
        return ret;
    }
    public static List<int> IntList(string str){
        if(str==null||str.Length==0) return new List<int>(0);
        int n=CountC(str,',')+1;
        var cnxt=new CutNextText(str);
        var ret= new List<int>(n);
        int i=0;
        for(; i<n; i++){
            if(CutNext(ref cnxt,',')<0) break;
            if(!TryParseInt(cnxt.txt.SliceLen(cnxt.head,cnxt.len),out int d)){
                error="数値が不正です";
                return null;
            }
            ret.Add(d);
        }
        return ret;
    }
    public static float[] FloatArr2(string str,char dlmt,int max=int.MaxValue){ dlmt_once[0]=dlmt; return FloatArr2(str,dlmt_once,max); }
    public static float[] FloatArr2(string str,char[] dlmt,int max=int.MaxValue){
        if(str==null||str.Length==0) return new float[0];
        int n=CountC(str,dlmt)+1;
        n=(n>max)?max:n;
        float[] ret=new float[n];
        var cnxt=new CutNextText(str);
        for(int i=0; i<n; i++){
            if(CutNext(ref cnxt,dlmt)<0) break;
            if(!TryParseFloat(cnxt.txt.SliceLen(cnxt.head,cnxt.len),out ret[i])){
                error="数値が不正です";
                return null;
            }
        }
        return ret;
    }
    private static readonly List<float> empty_float_list=new List<float>(0);
    public static List<float> FloatList(string str,char dlmt,List<float> buf=null,int max=int.MaxValue){dlmt_once[0]=dlmt;return FloatList(str,dlmt_once,buf,max);}
    public static List<float> FloatList(string str,char[] dlmt,List<float> buf=null,int max=int.MaxValue){
        if(str==null||str.Length==0) return empty_float_list;
        List<float> ret=buf??new List<float>();
        var cnxt=new CutNextText(str);
        ret.Clear();
        while(CutNext(ref cnxt,dlmt)>=0){
            if(!TryParseFloat(cnxt.txt.SliceLen(cnxt.head,cnxt.len),out float f)){
                error="数値が不正です";
                return null;
            }
            ret.Add(f);
        }
        return ret;
    }
    public static Arr4<float> Rgb(string str){
        if(str.IndexOf(',')>=0) return Rgb2(str);
        float[] ret=xyzbuf;
        if(!int.TryParse(str,System.Globalization.NumberStyles.HexNumber,null,out int rgb)){
            if(float.TryParse(str,out float f)&&f<=1&&f>=0){
                return new Arr4<float>(f,f,f,1);
            }
            error="色指定が不正です";
            return default;
        }
        return new Arr4<float>(((rgb>>16)&255)/255f,((rgb>>8)&255)/255f,(rgb&255)/255f,1f);
    }
    public static Arr4<float> Rgb2(string str){
        float[] ret=xyzbuf;
        int n=XyzSub(str,ret,3);
        if(n!=3){ error="色指定が不正です"; return default; }
        if(ret[0]<0||ret[0]>1||ret[1]<0||ret[1]>1||ret[2]<0||ret[2]>1){
            error="r,g,bの値は0.0～1.0の範囲で指定してください";
            return default;
        }
        return new Arr4<float>(ret[0],ret[1],ret[2],1f);
    }
    public static Arr4<float> Rgba(string str){
        float[] ret=xyzbuf;
        int n=XyzSub(str,ret,4);
        if(n!=3&&n!=4){ error="色指定が不正です"; return default; }
        if(n==3) ret[3]=1.0f;
        if(ret[0]<0||ret[0]>1||ret[1]<0||ret[1]>1||ret[2]<0||ret[2]>1||ret[3]<0||ret[3]>1){
            error="r,g,b,aの値は0.0～1.0の範囲で指定してください";
            return default;
        }
        return new Arr4<float>(ret[0],ret[1],ret[2],ret[3]);
    }
    public static Arr4<float> RgbaLenient(string str){
        float[] ret=xyzbuf;
        int n=XyzSub(str,ret,4);
        if(n!=3&&n!=4){ error="色指定が不正です"; return default; }
        if(n==3) ret[3]=1.0f;
        // 範囲チェックしない
        return new Arr4<float>(ret[0],ret[1],ret[2],ret[3]);
    }
    public static int OnOff(string str){
        if(str=="on") return 1;
        else if(str=="off") return 0;
        else { error="onまたはoffを指定してください"; return -1; }
    }
    private static readonly List<StrSegment> nth_range_seg_buf=new List<StrSegment>(16);
    private static readonly List<StrSegment> nth_range_range_buf=new List<StrSegment>(8);
    public static List<StrSegment> NthRange(StrSegment seg,string dlm,StrSegment range,List<StrSegment> buf=null){
        if(seg.Length==0) return emptysegList;
        var ta=seg.Split(dlm,nth_range_seg_buf);
        int tl=ta.Count;
        var ra=range.Split(',',nth_range_range_buf);
        if(ra.Count==0){ error="範囲の指定が不正です"; return null; }
        List<StrSegment> sa=buf??new List<StrSegment>(ra.Count);
        int n=0;
        for(int i=0; i<ra.Count; i++){
            if(!TryParseFloat(ra[i],out float f)||f==0){ error="数値の指定が不正です"; return null;}
            n=(int)f;
            if(n<-tl||n>tl){ sa.Add(StrSegment.empty); continue; }
            if(n<0) n=tl+n; else n--;
            sa.Add(ta[n]);
        }
        nth_range_seg_buf.Clear();
        nth_range_range_buf.Clear();
        return sa;
    }
    public static string Nth(string txt,string dlm,int n){
        var cnxt=new CutNextText(txt);
        for(int i=0; i<n-1; i++) if(CutNext(ref cnxt,dlm)<0) return null;
        if(CutNext(ref cnxt,dlm)<0) return null;
        return txt.Substring(cnxt.head,cnxt.len);
    }
    public static string Nth(string txt,char dlm,int n){
        var cnxt=new CutNextText(txt);
        for(int i=0; i<n-1; i++) if(CutNext(ref cnxt,dlm)<0) return null;
        if(CutNext(ref cnxt,dlm)<0) return null;
        return txt.Substring(cnxt.head,cnxt.len);
    }
    public static StrSegment Nth(StrSegment txt,char dlm,int n){
        var cnxt=new CutNextText(txt);
        for(int i=0; i<n-1; i++) if(CutNext(ref cnxt,dlm)<0) return StrSegment.empty;
        if(CutNext(ref cnxt,dlm)<0) return StrSegment.empty;
        return txt.SliceLen(cnxt.head,cnxt.len);
    }
    public struct CutNextText {
        public StrSegment txt;
        public int head;
        public int len;
        public int next;
        public CutNextText(string t){txt=new StrSegment(t);head=next=len=0;}
        public CutNextText(StrSegment t){txt=t;head=next=len=0;}
        public void Reset(){head=next=len=0;}
        public bool Equals(string str){return string.ReferenceEquals(txt.str,str); }
    }
    private static char[] dlmt_once=new char[1];
    public static int CutNext(ref CutNextText cnxt,char dlm){dlmt_once[0]=dlm; return CutNext(ref cnxt,dlmt_once);}
    public static int CutNext(ref CutNextText cnxt,char[] dlm){
        if(cnxt.next==-1) return -1;
        cnxt.head=cnxt.next;
        int idx=cnxt.txt.IndexOf(dlm,cnxt.head);
        if(idx>=0){
            cnxt.next=idx+1;
            cnxt.len=idx-cnxt.head;
        }else{
            cnxt.len=cnxt.txt.Length-cnxt.head;
            cnxt.next=-1;
        }
        return cnxt.len;
    }
    public static int CutNext(ref CutNextText cnxt,string dlm){
        if(cnxt.next==-1) return -1;
        cnxt.head=cnxt.next;
        int idx=cnxt.txt.IndexOf(dlm,cnxt.head);
        if(idx>=0){
            cnxt.next=idx+dlm.Length;
            cnxt.len=idx-cnxt.head;
        }else{
            cnxt.len=cnxt.txt.Length-cnxt.head;
            cnxt.next=-1;
        }
        return cnxt.len;
    }
    public static bool CutNext(string txt,char dlm,int[] pa){
        if(pa[2]==-1) return false;
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
    public static bool CutNext(string txt,string dlm,int[] pa){
        if(pa[2]==-1) return false;
        pa[0]=pa[2];
        if((pa[1]=txt.IndexOf(dlm,pa[0],Ordinal))>=0){
            pa[2]=pa[1]+dlm.Length; // 次開始位置
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

    public static Arr2<StrSegment> LeftAndRight(StrSegment seg,char dlm=',',StrSegment[] buf=null){dlmt_once[0]=dlm; return LeftAndRight(seg,dlmt_once,buf);}
    public static Arr2<StrSegment> LeftAndRight(StrSegment seg,char[] dlm,StrSegment[] buf=null){
        int idx=seg.IndexOf(dlm);
        return (idx<0)?
            new Arr2<StrSegment>(seg,StrSegment.empty):
            new Arr2<StrSegment>(seg.Slice(0,idx-1),seg.Slice(idx+1));
    }
    public static Arr2<StrSegment> LeftAndRight2(StrSegment seg,char dlm=','){dlmt_once[0]=dlm; return LeftAndRight2(seg,dlmt_once);}
    public static Arr2<StrSegment> LeftAndRight2(StrSegment seg,char[] dlm){
        int idx=seg.IndexOf(dlm);
        return (idx<0)?
            new Arr2<StrSegment>(StrSegment.empty,seg):
            new Arr2<StrSegment>(seg.Slice(0,idx-1),seg.Slice(idx+1));
    }
    public static Arr2<string> LeftAndRight(string txt,char dlm){dlmt_once[0]=dlm;return LeftAndRight(txt,dlmt_once);}
    public static Arr2<string> LeftAndRight(string txt,char[] dlm){
        int idx=txt.IndexOfAny(dlm);
        return (idx<0)?
            new Arr2<string>(txt,""):
            new Arr2<string>(txt.Substring(0,idx),txt.Substring(idx+1));
    }
    public static Arr2<string> LeftAndRight2(string txt,char dlm){dlmt_once[0]=dlm;return LeftAndRight2(txt,dlmt_once);}
    public static Arr2<string> LeftAndRight2(string txt,char[] dlm){
        int idx=txt.IndexOfAny(dlm);
        return (idx<0)?
            new Arr2<string>("",txt):
            new Arr2<string>(txt.Substring(0,idx),txt.Substring(idx+1));
    }
    public static int CountChar(string txt,char c){
        int ret=0;
        for(int i=0; i<txt.Length; i++) if(txt[i]==c) ret++;
        return ret;
    }
    /* 例えば Bip01 L UpperArm をBLUpperArm と書けるようにする
       Bip01 Spine1 ならBSpine1。ちょっとだけ短い＆空白エスケープ不要＆男女書き分け不要に
       もしBip01 [LR]\S+ に該当するボーンが現れたら破綻する */
    private static Dictionary<string,string> completename_f=new Dictionary<string,string>(64);
    private static Dictionary<string,string> completename_m=new Dictionary<string,string>(64){
        {"BSpine0a","ManBip Spine1"},{"BSpine1a","ManBip Spine2"},
        {"BRToe2","ManBip R Toe1"}, {"BRToe21","ManBip R Toe11"}, {"BRToe2Nub","ManBip R Toe1Nub"},
        {"BLToe2","ManBip L Toe1"}, {"BLToe21","ManBip L Toe11"}, {"BLToe2Nub","ManBip L Toe1Nub"}
    };
    public static string CompleteBoneName(string shortname,bool manq,bool crcq=false){
        if(shortname.Length<5 || !char.IsUpper(shortname[1]) || shortname[0]!='B') return shortname;
        var dic=(manq&&!crcq)?completename_m:completename_f;
        if(dic.TryGetValue(shortname,out string ret)) return ret;

        string root,lr="",uname;
        int n=1;
        if(shortname[1]=='L'){ lr=" L"; n++; }
        else if(shortname[1]=='R') { lr=" R"; n++; }
        if(lr.Length>0 && !char.IsUpper(shortname[2])) return shortname;

        root=(manq&&!crcq)?"ManBip":"Bip01";
        uname=shortname.Substring(n);
        return dic[shortname]=root+lr+" "+uname;
    }
    private static Dictionary<string,string> compactname_f=new Dictionary<string,string>(64);
    private static Dictionary<string,string> compactname_m=new Dictionary<string,string>(64);
    public static string CompactBoneName(string name){
        Dictionary<string,string> dic;
        if(name.StartsWith("Bip01 ",Ordinal)) dic=compactname_f;
        else if(name.StartsWith("ManBip ",Ordinal)) dic=compactname_m;
        else return name;
        if(dic.TryGetValue(name,out string ret)) return ret;
        char[] arr=new char[name.Length];
        arr[0]='B';
        int i=0,d=1;
        for(; i<name.Length; i++) if(name[i]==' ') break;
        for(i++; i<name.Length; i++) if(name[i]!=' ') arr[d++]=name[i];
        return dic[name]=new String(arr,0,d);
    }

    public static bool TryParseFloat(StrSegment seg,out float f){
        f=0;
        if(seg.Length==0) return false;
        string str=seg.str;
        int i=seg.head,t=seg.tail;
        float sign=1;
        double n=0,s=0;
        if(str[i]=='+') i++;
        for(; i<=t; i++) if(str[i]=='-'){sign*=-1;}else break;
        char c= ' ';
        for(; i<=t; i++){
            c=str[i];
            if(c=='.') break;
            if(char.IsDigit(c)){n=n*10+(c-'0');}
            else{ error="数値が不正です"; return false;}
        }
        ulong k=1;
        if(c=='.'){
            for(i++; i<=t; i++){
                c=str[i];
                if(char.IsDigit(c)){s=s*10+(c-'0');k*=10;}
                else{ error="数値が不正です"; return false;}
            }
        }
        f=sign*(float)(n+s/k);
        return true;
    }
    public static bool TryParseInt(StrSegment seg,out int f){
        f=0;
        if(seg.Length==0) return false;
        string str=seg.str;
        int i=seg.head,t=seg.tail,sign=1;
        long n=0;
        if(str[i]=='+') i++;
        for(; i<=t; i++) if(str[i]=='-'){sign*=-1;}else break;
        for(; i<=t; i++){
            char c=str[i];
            if(char.IsDigit(c)){n=n*10+(c-'0');}
            else{ error="数値が不正です"; return false;}
        }
        f=(int)(sign*n);
        return true;
    }
    public static bool TryParseDouble(StrSegment seg,out double f){
        f=0;
        if(seg.Length==0) return false;
        string str=seg.str;
        int i=seg.head,t=seg.tail;
        double sign=1;
        double n=0,s=0;
        if(str[i]=='+') i++;
        for(; i<=t; i++) if(str[i]=='-'){sign*=-1;}else break;
        char c= ' ';
        for(; i<=t; i++){
            c=str[i];
            if(c=='.') break;
            if(char.IsDigit(c)){n=n*10+(c-'0');}
            else{ error="数値が不正です"; return false;}
        }
        ulong k=1;
        if(c=='.'){
            for(i++; i<=t; i++){
                c=str[i];
                if(char.IsDigit(c)){s=s*10+(c-'0');k*=10;}
                else{ error="数値が不正です"; return false;}
            }
        }
        f=sign*(n+s/k);
        return true;
    }
    public static float ParseFloat(string str,float dflt=float.NaN){
        if(!TryParseFloat(new StrSegment(str),out float ret)) return dflt;
        return ret;
    }
    public static double ParseDouble(string str,double dflt=double.NaN){
        if(!TryParseDouble(new StrSegment(str),out double ret)) return dflt;
        return ret;
    }
    public static int ParseInt(string str,int dflt=int.MinValue){
        if(!TryParseInt(new StrSegment(str),out int ret)) return dflt;
        return ret;
    }
    public static string Chomp(string s,bool crq=false){
        int l=s.Length;
        if(l>0 && s[l-1]=='\n') l--;
        if(l>0 && crq && s[l-1]=='\r') l--;
        if(l!=s.Length) return s.Substring(0,l);
        return s;
    }
    public struct ColonDesc {
        private static string[] types={"obj","maid","man","light",""};
        private static char[] type_cs={'o','f','m','l',' '};
        public int num;
        public string type;
        public string id;
        public string slot;
        public string bone;
        public string path;
        public int meshno;
        public char type_c;
        public ColonDesc(string s):this(new StrSegment(s)) {}
        public ColonDesc(StrSegment seg){
            num=0;
            type_c='\0';
            type=id=slot=bone=path="";
            meshno=-1;
            if(seg.Length==0) return;

            int p,li,n;
            if((p=seg.IndexOf(':'))>=0){
                var typ=seg.Slice(0,p-1);
                for(int i=0; i<types.Length; i++) if(typ.eq(types[i])){
                    type=types[i]; // typ.Substr()だと新たに文字列作っちゃう
                    type_c=type_cs[i];
                }
                seg.SliceSelf(p+1,-1);
            }else{
                p=FindMeshno(seg);  // mesh objectname#0 の形があり得る
                if(p>=0 && ParseUtil.TryParseInt(seg.Slice(p+1),out n) && n>=0){
                    id=seg.Substr(0,p);
                    meshno=n;
                    type="obj"; bone="/"; num=3;
                }else{  // コロン記法じゃない。例えパスがあっても。"//"も含む
                    id=seg.ToString();
                }
                return;
            }
            p=FindMeshno(seg);
            if(p>=0 && ParseUtil.TryParseInt(seg.Slice(p+1),out n) && n>=0){
                meshno=n;
                seg.SliceSelf(0,p-1);
            }
            if((p=seg.IndexOf('/'))>=0){
                path=seg.Substr(p+1);
                seg.SliceSelf(0,p-1);
            }
            if((li=seg.IndexOf(':'))>=0){
                num=3;
                bone=seg.Substr(li+1);
                seg.SliceSelf(0,li-1);
                if(bone=="") bone="/";
            }else num=2;
            if((p=seg.IndexOf('.'))>=0){
                id=seg.Substr(0,p);
                slot=seg.Substr(p+1);
            }else id=seg.ToString();
            if(li<0&&(meshno>=0||slot!="")){bone="/"; num=3; }
        }
        // meshnoが無い時にIndexOf('#')系よりもやや判断が速いはず
        private int FindMeshno(StrSegment seg){
            int t=seg.Length-1;
            if(t<0 || !char.IsDigit(seg[t])) return -1;
            for(int i=t-1; i>=0; i--){
                if(seg[i]=='#') return i;
                if(!char.IsDigit(seg[i])) return -1;
            }
            return -1;
        }
    }
    public static PropDesc ParseProp(string val){
        PropDesc pd=new PropDesc(val);
        if(pd.kva==null) return null;
        return pd;
    }
    public class PropDesc {
        public struct PropKV {
            public string key;
            public string value;
            public PropKV(string k,string v){key=k;value=v;}
        }
        public List<PropKV> kva=null;
        public PropDesc(string val){
            var lst=new List<PropKV>();
            int i=0,p=0;
            string k=null;
            for(; i<val.Length; i++){
                if(val[i]=='='){ 
                    if(k!=null) continue;
                    k=val.Substring(p,i-p);
                    p=i+1;
                }else if(val[i]=='\n'){
                    if(k==null) return;
                    lst.Add(new PropKV(k,val.Substring(p,i-p)));
                    p=i+1; k=null;
                }
            }
            if(p<val.Length){
                if(k==null) return;
                lst.Add(new PropKV(k,val.Substring(p,i-p)));
            }
            kva=lst;
        }
        public bool HasEmptyKey(){
            for(int i=0; i<kva.Count; i++) if(kva[i].key=="") return true;
            return false;
        }
        public bool HasEmptyValue(){
            for(int i=1; i<kva.Count; i++) if(kva[i].value=="") return true;
            return false;
        }
        public Dictionary<string,string> ToDict(){
            // .Netが古いので、IEnumerable<KeyValuePair<T,T>>をとるコンストラクタは無い
            var ret=new Dictionary<string,string>(kva.Count);
            for(int i=0; i<kva.Count; i++) ret.Add(kva[i].key,kva[i+1].value);
            return ret;
        }
    }
}
}
