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
    public bool envChanged=false;
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
        prevEoL=';'; envChanged=false;
        sno=-1;
        currentStatement=Statement.zero;
    }

    private VarDic lvars;
    private Dictionary<string,string> svars;
    public List<string> Next(VarDic lv, Dictionary<string,string> sv){
        if(++sno>=sta.Count) return null;
        lvars=lv; svars=sv;
        envChanged=false;
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
        if(ComShInterpreter.IsEnvChanged(key)) envChanged=true;
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
            if(ComShInterpreter.IsEnvChanged(tok.assign.key)) envChanged=true;
            return true;
        }else{
            Variables.Set(key,value,lvars,svars);
            if(ComShInterpreter.IsEnvChanged(tok.assign.key)) envChanged=true;
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
    public static char[] period={'.'};
    public static char[] colon={':'};
    public static char[] space={' '};
    public static char[] tab={'\t'};
    public static char[] lf={'\n'};
    public static char[] cr={'\r'};
    public static char[] crlf={'\r','\n'};
    public static char[] eqcln={':','='};

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
            if(float.IsNaN(v)){ error="数値が不正です"; return null; }
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
    public static float[] MinMax(string str){
        if(str==null||str.Length==0) return null;
        float[] ret=new float[2];
        int n=XyzSub(str,ret);
        if(n==2){
            if(ret[0]>ret[1]){ float t=ret[0]; ret[0]=ret[1]; ret[1]=t; }
            return ret;
        }else if(n==1){ ret[1]=ret[0]; return ret; }
        else { error="数値が不正です"; return null; }
    }
    public static double[] MinMaxW(string str){
        if(str==null||str.Length==0) return null;
        double[] ret=new double[2];
        int n=XyzSubW(str,ret);
        if(n==2){
            if(ret[0]>ret[1]){ double t=ret[0]; ret[0]=ret[1]; ret[1]=t; }
            return ret;
        }else if(n==1){ ret[1]=ret[0]; return ret; }
        else { error="数値が不正です"; return null; }
    }

    public static float[] Xy(string str){
        if(str==null||str.Length==0) return null;
        float[] ret=new float[2];
        int n=XyzSub(str,ret);
        if(n==2) return ret;
        else { error="座標が不正です"; return null; }
    }
    public static float[] XyzR(string str,out bool relativeq){
        relativeq=(str!=null && str.Length>0 && str[0]=='+');
        float[] ret=new float[3];
        int n=XyzSub(relativeq?str.Substring(1):str,ret);
        if(n==3) return ret;
        else { error="座標が不正です"; return null; }
    }
    public static float[] Xyz(string str){
        float[] ret=new float[3];
        int n=XyzSub(str,ret);
        if(n==3) return ret;
        else { error="座標が不正です"; return null; }
    }
    public static float[] Xyz(string[] sa){
        float[] ret=new float[3];
        for(int i=0; i<ret.Length; i++){
            ret[i]=ParseFloat(sa[i]);
            if(float.IsNaN(ret[i])){ error="座標が不正です"; return null; }
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
    public static int XyzSubW(string str,double[] ret){
        if(str==null||str.Length==0) return 0;
        string[] sa=str.Split(comma);
        if(sa.Length>ret.Length) return -1;
        for(int i=0; i<sa.Length; i++){
            ret[i]=ParseDouble(sa[i]);
            if(double.IsNaN(ret[i])) return -1;
        }
        return sa.Length;
    }
    public static float[] Xyz2(string str){
        float[] ret=new float[3];
        int n=XyzSub(str,ret);
        if(n==3) return ret;
        else if(n==1){ ret[1]=ret[2]=ret[0]; return ret; }
        else { error="座標が不正です"; return null; }
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
            error="四元数が不正です";
            return null; 
        }
        if(invq){ ret[0]*=-1; ret[1]*=-1; ret[2]*=-1; }
        return ret;
    }
    public static float[] Quat(string str){
        if(str==null||str.Length==0) return null;
        string[] sa=str.Split(comma);
        if(sa.Length!=4){ error="四元数が不正です"; return null; }
        return Quat(sa);
    }
    public static double[] QuatW(string[] str){
        string s0=str[0];
        if(s0.Length==0) return null;
        int i;
        for(i=0; i<s0.Length; i++) if(s0[i]!='~') break;
        bool invq=(i%2)==1;
        if(i>0) s0=s0.Substring(i);
        double[] ret=new double[4]{
            ParseDouble(s0),ParseDouble(str[1]),ParseDouble(str[2]),ParseDouble(str[3])
        };
        if(double.IsNaN(ret[0])||double.IsNaN(ret[1])||double.IsNaN(ret[2])||double.IsNaN(ret[3])){
            error="四元数が不正です";
            return null; 
        }
        if(invq){ ret[0]*=-1; ret[1]*=-1; ret[2]*=-1; }
        return ret;
    }
    public static double[] QuatW(string str){
        if(str==null||str.Length==0) return null;
        string[] sa=str.Split(comma);
        if(sa.Length!=4){ error="四元数が不正です"; return null; }
        return QuatW(sa);
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
            if(float.IsNaN(ret[i])){ error="数値が不正です"; return null;}
        }
        return ret;
    }
    public static double[] DoubleArr(string str){
        if(str==null||str.Length==0) return new double[0];
        string[] sa=str.Split(comma);
        if(sa.Length==4 && str[0]=='~') return QuatW(sa);
        double[] ret= new double[sa.Length];
        for(int i=0; i<sa.Length; i++){
            ret[i]=ParseDouble(sa[i]);
            if(double.IsNaN(ret[i])){ error="数値が不正です"; return null;}
        }
        return ret;
    }
    public static List<int> IntList(string str){
        if(str==null||str.Length==0) return new List<int>();
        string[] sa=str.Split(comma);
        var ret= new List<int>(sa.Length);
        for(int i=0; i<sa.Length; i++){
            if(!int.TryParse(sa[i],out int n)){ error="数値が不正です"; return null;}
            ret.Add(n);
        }
        return ret;
    }
    public static float[] Rgb(string str){
        if(str.IndexOf(',')>=0) return Rgb2(str);
        float[] ret=new float[3];
        if(!int.TryParse(str,System.Globalization.NumberStyles.HexNumber,null,out int rgb)){
            if(float.TryParse(str,out float f)&&f<=1&&f>=0){
                ret[0]=ret[1]=ret[2]=f;
                return ret;
            }
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
        if(txt=="") return emptyList;
        nthRangeDlm[0]=dlm;
        string[] ta=txt.Split(nthRangeDlm,StringSplitOptions.None);
        int tl=ta.Length;
        string[] ra=range.Split(comma);
        if(ra.Length==0){ error="範囲の指定が不正です"; return null; }
        List<string> sa=new List<string>(ra.Length);
        int n=0;
        for(int i=0; i<ra.Length; i++){
            if(!float.TryParse(ra[i],out float f)||f==0){ error="数値の指定が不正です"; return null;}
            n=(int)f;
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
    public static string[] LeftAndRight(string txt,char[] dlm){
        int idx=-1;
        for(int i=0; i<dlm.Length; i++){
            idx=txt.IndexOf(dlm[i]);
            if(idx>=0) break;
        }
        if(idx<0) return new string[]{txt,""};
        return new string[]{ txt.Substring(0,idx),txt.Substring(idx+1) };
    }
    public static string[] LeftAndRight2(string txt,char dlm){
        int idx=txt.IndexOf(dlm);
        if(idx<0) return new string[]{ "",txt };
        return new string[]{ txt.Substring(0,idx),txt.Substring(idx+1) };
    }
    public static string[] LeftAndRight2(string txt,char[] dlm){
        int idx=-1;
        for(int i=0; i<dlm.Length; i++){
            idx=txt.IndexOf(dlm[i]);
            if(idx>=0) break;
        }
        if(idx<0) return new string[]{"",txt};
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
    private static Dictionary<string,string> completename_f=new Dictionary<string,string>(64);
    private static Dictionary<string,string> completename_m=new Dictionary<string,string>(64){
        {"BSpine0a","ManBip Spine1"},{"BSpine1a","ManBip Spine2"},
        {"BRToe2","ManBip R Toe1"}, {"BRToe21","ManBip R Toe11"}, {"BRToe2Nub","ManBip R Toe1Nub"},
        {"BLToe2","ManBip L Toe1"}, {"BLToe21","ManBip L Toe11"}, {"BLToe2Nub","ManBip L Toe1Nub"}
    };
    public static string CompleteBoneName(string shortname,bool manq){
        if(shortname.Length<5 || !char.IsUpper(shortname[1]) || shortname[0]!='B') return shortname;
        var dic=manq?completename_m:completename_f;
        if(dic.TryGetValue(shortname,out string ret)) return ret;

        string root,lr="",uname;
        int n=1;
        if(shortname[1]=='L'){ lr=" L"; n++; }
        else if(shortname[1]=='R') { lr=" R"; n++; }
        if(lr.Length>0 && !char.IsUpper(shortname[2])) return shortname;

        root=manq?"ManBip":"Bip01";
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
        string[] sa=name.Split(space);
        sa[0]="B";
        return dic[name]=string.Join("",sa);
    }

    public static float ParseFloat(string str,float dflt=float.NaN){
        if(string.IsNullOrEmpty(str)) return dflt;
        int i;
        string s=str;
        for(i=0; i<s.Length; i++) if(s[i]!='-') break;
        bool minus=(i%2)==1;
        if(i>0) s=s.Substring(i);
        bool ok=float.TryParse(s,out float ret);
        if(minus) ret*=-1;
        return ok?ret:dflt;
    }
    public static double ParseDouble(string str,double dflt=double.NaN){
        if(string.IsNullOrEmpty(str)) return dflt;
        int i;
        string s=str;
        for(i=0; i<s.Length; i++) if(s[i]!='-') break;
        bool minus=(i%2)==1;
        if(i>0) s=s.Substring(i);
        bool ok=double.TryParse(s,out double ret);
        if(minus) ret*=-1;
        return ok?ret:dflt;
    }
    public static int ParseInt(string str,int dflt=int.MinValue){
        if(string.IsNullOrEmpty(str)) return dflt;
        int i;
        string s=str;
        for(i=0; i<s.Length; i++) if(s[i]!='-') break;
        bool minus=(i%2)==1;
        if(i>0) s=s.Substring(i);
        bool ok=int.TryParse(s,out int ret);
        if(minus) ret*=-1;
        return ok?ret:dflt;
    }
    public static string Chomp(string s,bool crq=false){
        int l=s.Length;
        if(l>0 && s[l-1]=='\n') l--;
        if(l>0 && crq && s[l-1]=='\r') l--;
        if(l!=s.Length) return s.Substring(0,l);
        return s;
    }
    public struct ColonDesc {
        public int num;
        public string type;
        public string id;
        public string bone;
        public string path;
        public int meshno;
        public ColonDesc(string s){
            this.num=0;
            this.type="";
            this.id=s;
            this.bone="";
            this.path="";
            this.meshno=-1;
            if(string.IsNullOrEmpty(s)) return;
            int li=s.LastIndexOf(':');
            if(li<0){       // objectname#0の形のみ
                int sharp=s.IndexOf('#');
                if(sharp>=0 && int.TryParse(s.Substring(sharp+1),out int n) && n>=0){
                    this.num=2;
                    this.type="obj";
                    this.id=s.Substring(0,sharp);
                    this.meshno=n;
                }
                return;
            }

            int fi=s.IndexOf(':');
            if(fi>=0){
                if(fi==li){ // maid:0等
                    this.num=2;
                    this.type=s.Substring(0,fi);
                    int sharp=s.IndexOf('#',fi+1);
                    if(sharp>=0 && int.TryParse(s.Substring(sharp+1),out int n) && n>=0){
                        this.id=s.Substring(fi+1,sharp-fi-1);
                        this.meshno=n;
                    }else{
                        this.id=s.Substring(fi+1);
                    }
                    return;
                }else{      // maid:0:BHead等
                    this.num=3;
                    this.type=s.Substring(0,fi);
                    this.id=s.Substring(fi+1,li-fi-1);
                    this.bone=s.Substring(li+1);
                    int p=this.bone.IndexOf('/');
                    if(p>=0){
                        this.path=this.bone.Substring(p+1);
                        this.bone=this.bone.Substring(0,p);
                        int sharp=this.path.IndexOf('#');
                        if(sharp>=0 && int.TryParse(this.path.Substring(sharp+1),out int n) && n>=0){
                            this.meshno=n;
                            this.path=this.path.Substring(0,sharp);
                        }
                    }else{
                        int sharp=this.bone.IndexOf('#');
                        if(sharp>=0 && int.TryParse(this.bone.Substring(sharp+1),out int n) && n>=0){
                            this.meshno=n;
                            this.bone=this.bone.Substring(0,sharp);
                        }
                    }
                }
            }
        }
    }
}
}
