using System;
using UnityEngine;
using System.Collections.Generic;

namespace COM3D2.ComSh.Plugin {
// パネル
public class ComShPanel {
    public int wid;
    public string title="";
    public ComShInterpreter shell=null;
    public int fontSize=14;
    public WindowStyle ws;
    public PanelStyles style;
    private long styleDate=0;

    public const int TITLEHEIGHT=17;
    public Rect windowrect=new Rect(0,0,100,100);
    public Rect titleRect=new Rect(0,2,100,TITLEHEIGHT);
    public Rect termRect=new Rect(0,2,0,0);
    public Rect closeRect=new Rect(0,2,0,0);

    public void OnVisibleChange(bool v){}
    private long nextChk=0;
    private bool lastEnabled=false;
    public void Draw(){
        if(PanelStyleCache.updDate>styleDate){
            ws=PanelStyleCache.GetWindowStyles();
            styleDate=PanelStyleCache.updDate;
            style=PanelStyleCache.GetStyles(fontSize);
            titleRect.width=windowrect.width;
            termRect.width=closeRect.width=ws.cbSize.x;
            termRect.height=closeRect.height=ws.cbSize.y;
            closeRect.x=windowrect.width-closeRect.width-2;
            termRect.x=closeRect.x-closeRect.width;
            termRect.y=closeRect.y=TITLEHEIGHT-closeRect.height;
        }
	    windowrect=GUI.Window(wid,windowrect,Panel,"",ws.window);
        if(ComboWin.visible && ReferenceEquals(ComboWin.panel,this)){
            ComboWin.Position(windowrect);
            ComboWin.Draw();
            GUI.BringWindowToFront(ComboWin.wid);
        }
        if(ComShWM.updateTime>nextChk){
            shell.env["panelx"]=((int)windowrect.x).ToString();
            shell.env["panely"]=((int)windowrect.y).ToString();

            bool enabled=windowrect.Contains(Event.current.mousePosition); // mouseenter/leave
            if(enabled && !lastEnabled){
                if(UIInput.selection!=null) UIInput.selection.isSelected=false;
                var ia=GameObject.FindObjectsOfType<UIInput>();
                foreach(var ip in ia) ip.isSelected=false;
                UIInput.selection=null;
                UICamera.selectedObject=null;
            }
            lastEnabled=enabled;
            nextChk=ComShWM.updateTime+200*TimeSpan.TicksPerMillisecond;
        }
    }
    public void Panel(int wid){
        GUI.Label(titleRect,title,ws.title);
        if(GUI.Button(termRect,"Ｔ",ws.closebtn)){ ComShWM.ToggleTerm(); return; }
        if(GUI.Button(closeRect,"☓",ws.closebtn)){ ComShWM.ClosePanel(wid); return; }
        draw.Invoke();
        GUI.DragWindow();
    }

    public void OnClose(){
        if(ComboWin.panel==this) ComboWin.visible=false;
        if(psrOnClose!=null){ shell.InterpretParser(psrOnClose); shell.exitq=false; }
        shell.panel=null;
    }

    private static void nop(){}
    public delegate void DrawElements();
    public DrawElements draw=new DrawElements(nop);
    private ComShParser psrOnClose=null;

    public ComShPanel(ComShInterpreter sh,int x,int y,int w,int h,string t,int fsize,int wid){
        title=t;
        shell=sh;
        windowrect.x=x; windowrect.y=y; windowrect.width=w; windowrect.height=h;
        draw=new DrawElements(nop);
        fontSize=fsize;
        grid.x=0; grid.y=0; grid.width=1; grid.height=1;
        psrOnClose=null;
        sh.panel=this;
        this.wid=wid;
    }
    public void SetOnClose(ComShParser p){ psrOnClose=p; }
    public Rect grid=new Rect(0,0,1,1);
    public void SetGrid(float[] xywh){
        grid.x=xywh[0]; grid.y=xywh[1]; grid.width=xywh[2]; grid.height=xywh[3];
    }
    public int[] Plot(float[] xywh){
        return new int[]{
            (int)(grid.x+xywh[0]*grid.width),
            (int)(grid.y+xywh[1]*grid.height),
            (int)(grid.width*xywh[2]),
            (int)(grid.height*xywh[3])
        };
    }
    public void AddButton(int x,int y,int w,int h,string l,ComShParser p){
        var r=new Rect(x,y+TITLEHEIGHT,w,h);
        draw+=()=>{
            if(GUI.Button(r,l,style.button)){
                p.Reset();
                shell.InterpretParser(p);
                shell.exitq=false;
            }
        };
    }
    public void AddRbutton(int x,int y,int w,int h,string l,ComShParser p){
        var r=new Rect(x,y+TITLEHEIGHT,w,h);
        draw+=()=>{
            if(GUI.RepeatButton(r,l,style.button)){
                p.Reset();
                shell.InterpretParser(p);
                shell.exitq=false;
            }
        };
    }
    public void AddToggle(int x,int y,int w,int h,string l,ComShParser p,string name,bool v0){
        var r=new Rect(x,y+TITLEHEIGHT,w,h);
        shell.env[name]=v0?"1":"0";
        if(p==null){
            draw+=()=>{
                bool chk=shell.env[name]=="1";
                bool c=GUI.Toggle(r,chk,l,style.toggle);
                if(c!=chk) shell.env[name]=c?"1":"0";
            };
        }else{
            draw+=()=>{
                bool chk=shell.env[name]=="1";
                bool c=GUI.Toggle(r,chk,l,style.toggle);
                if(c!=chk){
                    shell.env[name]=c?"1":"0";
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                }
            };
        }
    }
    public void AddTextField(int x,int y,int w,int h,ComShParser p,string name,string v0){
        var r=new Rect(x,y+TITLEHEIGHT,w,h);
        shell.env[name]=v0;
        if(p==null){
            draw+=()=>{
                string txt=shell.env[name];
                string t=GUI.TextField(r,txt,style.text);
                if(t!=txt) shell.env[name]=t;
            };
        }else{
            draw+=()=>{
                string txt=shell.env[name];
                string t=GUI.TextField(r,txt,style.text);
                if(t!=txt){
                    shell.env[name]=t;
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                }
            };
        }
    }
    public void AddCombo(int x,int y,int w,int h,ComShParser p,string name,string v0,string[] items,char dlmt){
        int ty=y+TITLEHEIGHT;
        var r=new Rect(x,ty,w,h);
        var rb=new Rect(x+w,ty,h,h);
        const int maxh=200;
        var ci=new ComboItems(items,dlmt);
        shell.env[name]=v0;
        if(p==null){
            ComboWin.OnSelect onselect=(string val)=>{ shell.env[name]=val; };
            draw+=()=>{
                string txt=ci.GetLabel(shell.env[name]);
                string t=GUI.TextField(r,txt,style.text);
                if(t!=txt) shell.env[name]=ci.GetValue(t);
                if(GUI.Button(rb,"▼",style.comboBtn)){
                    ComboWin.SetItems(this,ci,onselect,x,ty+h,w,maxh);
                    ComboWin.Position(windowrect);
                    ComboWin.Toggle();
                }
            };
        }else{
            ComboWin.OnSelect onselect=(string val)=>{
                shell.env[name]=val;
                p.Reset();
                shell.InterpretParser(p);
                shell.exitq=false;
            };
            draw+=()=>{
                string txt=ci.GetLabel(shell.env[name]);
                string t=GUI.TextField(r,txt,style.text);
                if(t!=txt){
                    shell.env[name]=ci.GetValue(t);
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                }
                if(GUI.Button(rb,"▼",style.comboBtn)){
                    ComboWin.SetItems(this,ci,onselect,x,ty+h,w,maxh);
                    ComboWin.Position(windowrect);
                    ComboWin.Toggle();
                }
            };
        }
    }
    public void AddCombo2(int x,int y,int w,int h,ComShParser p,string name,string v0,ComShParser lst,char dlmt){
        int ty=y+TITLEHEIGHT;
        var r=new Rect(x,ty,w,h);
        var rb=new Rect(x+w,ty,h,h);
        const int maxh=200;
        shell.env[name]=v0;
        var ci=MakeItems(shell,lst,dlmt);
        if(p==null){
            ComboWin.OnSelect onselect=(string val)=>{ shell.env[name]=val; };
            draw+=()=>{
                string txt=ci.GetLabel(shell.env[name]);
                string t=GUI.TextField(r,txt,style.text);
                if(t!=txt) shell.env[name]=ci.GetValue(t);
                if(GUI.Button(rb,"▼",style.comboBtn)){
                    ci=MakeItems(shell,lst,dlmt);
                    ComboWin.SetItems(this,ci,onselect,x,ty+h,w,maxh);
                    ComboWin.Position(windowrect);
                    ComboWin.Toggle();
                }
            };
        }else{
            ComboWin.OnSelect onselect=(string val)=>{
                shell.env[name]=val;
                p.Reset();
                shell.InterpretParser(p);
                shell.exitq=false;
            };
            draw+=()=>{
                string txt=ci.GetLabel(shell.env[name]);
                string t=GUI.TextField(r,txt,style.text);
                if(t!=txt){
                    shell.env[name]=ci.GetValue(t);
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                }
                if(GUI.Button(rb,"▼",style.comboBtn)){
                    ci=MakeItems(shell,lst,dlmt);
                    ComboWin.SetItems(this,ci,onselect,x,ty+h,w,maxh);
                    ComboWin.Position(windowrect);
                    ComboWin.Toggle();
                }
            };
        }
    }
    private static string[] items0={""};
    private static ComboItems MakeItems(ComShInterpreter sh,ComShParser plst,char dlmt){
        var sbo=new ComShInterpreter.SubShOutput();
        ComShInterpreter child = new ComShInterpreter(new ComShInterpreter.Output(sbo.Output),sh.env,sh.func);
        plst.Reset();
        int ret=child.InterpretParser(plst);
        if(ret<0) return new ComboItems(items0,items0);
        return new ComboItems(ParseUtil.Chomp(sbo.GetSubShResult()).Split(ParseUtil.lf),dlmt);
    }
    public void AddSlider(int x,int y,int w,int h,ComShParser p,string name,float v0,float min,float max){
        var r=new Rect(x,y+TITLEHEIGHT,w,h);
        shell.env[name]=shell.fmt.FVal(v0);
        string old=shell.env[name];
        float val=v0;
        if(p==null){
            draw+=()=>{
                string cur=shell.env[name];
                if(old!=cur){ // 毎フレームTryParse()するよりはたぶんマシ
                    float.TryParse(cur,out val);
                    old=cur;
                }
                float v=GUI.HorizontalSlider(r,val,min,max);
                if(v!=val) shell.env[name]=shell.fmt.FVal(v);
            };
        }else{
            draw+=()=>{
                string cur=shell.env[name];
                if(old!=cur){ float.TryParse(cur,out val); old=cur; }
                float v=GUI.HorizontalSlider(r,val,min,max);
                if(v!=val){
                    shell.env[name]=shell.fmt.FVal(v);
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                }
            };
        }
    }
    public void AddVSlider(int x,int y,int w,int h,ComShParser p,string name,float v0,float min,float max){
        var r=new Rect(x,y+TITLEHEIGHT,w,h);
        shell.env[name]=shell.fmt.FVal(v0);
        string old=shell.env[name];
        float val=v0;
        if(p==null){
            draw+=()=>{
                string cur=shell.env[name];
                if(old!=cur){ // 毎フレームTryParse()するよりはたぶんマシ
                    float.TryParse(cur,out val);
                    old=cur;
                }
                float v=GUI.VerticalSlider(r,val,min,max);
                if(v!=val) shell.env[name]=shell.fmt.FVal(v);
            };
        }else{
            draw+=()=>{
                string cur=shell.env[name];
                if(old!=cur){ float.TryParse(cur,out val); old=cur; }
                float v=GUI.VerticalSlider(r,val,min,max);
                if(v!=val){
                    shell.env[name]=shell.fmt.FVal(v);
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                }
            };
        }
    }
    public void AddLabel(int x,int y,int w,int h,string l){
        var r=new Rect(x,y+TITLEHEIGHT,w,h);
        draw+=()=>{ GUI.Label(r,l,style.label); };
    }
}
public class ComboItems {
    public string[] labels;
    public string[] values;
    public ComboItems(string[] la,string[] va){ labels=la; values=va; }
    public ComboItems(string[] items,char dlmt){
        if(dlmt==0) labels=values=items; else {
            labels=new string[items.Length];
            values=new string[items.Length];
            for(int i=0; i<items.Length; i++){
                string[] sa=ParseUtil.LeftAndRight(items[i],dlmt);
                labels[i]=sa[0]; values[i]=sa[1];
            }
        }
    }
    public string GetLabel(string value){
        for(int i=0; i<values.Length; i++) if(values[i]==value) return labels[i];
        return "";
    }
    public string GetValue(string label){
        for(int i=0; i<labels.Length; i++) if(labels[i]==label) return values[i];
        return "";
    }
}
// コンボボックスのポップアップ
public static class ComboWin {
    public static int wid=ComShProperties.windowID+2;
    public static Rect windowrect=new Rect(0,0,100,100);
    public static Vector2 offset=new Vector2(0,0);
    public static Rect viewRect=new Rect(0,0,100,100);
    public static Rect contentRect=new Rect(0,0,100,100);
    public static bool visible=false;

    private static ComboItems items;
    public delegate void OnSelect(string val);
    public static OnSelect callback;
    public static ComShPanel panel;

    public static void SetItems(ComShPanel p,ComboItems it,OnSelect cb,int x,int y,int w,int h){
        panel=p;
        items=it;
        callback=cb; selIdx=-1; 
        int wb=(int)panel.style.button.CalcSize(new GUIContent("▼")).x;
        offset.x=x; offset.y=y;
        viewRect.width=windowrect.width=w+wb;
        contentRect.width=w;
        int lh=(int)panel.style.comboItem.CalcSize(new GUIContent("漢")).y;
        contentRect.height=items.labels.Length*lh;
        viewRect.height=windowrect.height=Math.Min(contentRect.height,h);
    }
    public static void Position(Rect r){
        windowrect.x=r.x+offset.x; windowrect.y=r.y+offset.y;
    }

    public static void Toggle(){ visible=!visible; }
    public static void Draw(){
        windowrect=GUI.Window(wid,windowrect,Select,"",panel.style.comboWin);
    }
    private static int selIdx=-1;
    private static Vector2 scr=new Vector2(0,0);
    public static void Select(int wid){
        scr=GUI.BeginScrollView(viewRect,scr,contentRect);
        int si=GUI.SelectionGrid(contentRect,selIdx,items.labels,1,panel.style.comboItem);
        if(si!=selIdx){
            if(callback!=null) callback.Invoke(items.values[si]);
            visible=false;
        }
        GUI.EndScrollView();
    }
}

public static class PanelStyleCache {
    public static long updDate=DateTime.UtcNow.Ticks;
    public static void Dirty(){ updDate=DateTime.UtcNow.Ticks; }
    private static Dictionary<int,PanelStyles> styledic=new Dictionary<int,PanelStyles>();
    public static PanelStyles GetStyles(int fsize){
        if(styledic.ContainsKey(fsize)) return styledic[fsize];
        updDate=ComShWM.updateTime;
        var ps=new PanelStyles(fsize);
        ps.UpdateTextColor();
        if(bgTex!=null) ps.UpdateBgTex(bgTex);
        styledic[fsize]=ps;
        return ps;
    }
    private static WindowStyle winStyle;
    public static WindowStyle GetWindowStyles(){
        if(winStyle==null){
            updDate=ComShWM.updateTime;
            winStyle=new WindowStyle();
            winStyle.UpdateTextColor();
            if(bgColor.a>0) winStyle.UpdateBgColor(bgColor);
        }
        return winStyle;
    }
    private static Texture2D bgTex;
    private static Color bgColor=Color.clear;
    public static void SetBgColor(float[] col){
        bgColor=new Color(col[0],col[1],col[2],col[3]);
        if(winStyle!=null) winStyle.UpdateBgColor(bgColor);
        UpdTex(bgColor);
        foreach(var s in styledic.Values) s.UpdateBgTex(bgTex);
        updDate=ComShWM.updateTime;
    }
    private static void UpdTex(Color c){
        if(bgTex==null){
            bgTex=new Texture2D(3,3);
            bgTex.filterMode=FilterMode.Point;
        }
        var v=Mathf.Max(c.r,c.g,c.b);
        var b=(v<0.5f)?(new Color(0.75f,0.75f,0.75f,c.a)):(new Color(0.25f,0.25f,0.25f,c.a));
        bgTex.SetPixel(0,0,b); bgTex.SetPixel(1,0,b); bgTex.SetPixel(2,0,b);
        bgTex.SetPixel(0,1,b); bgTex.SetPixel(1,1,c); bgTex.SetPixel(2,1,b);
        bgTex.SetPixel(0,2,b); bgTex.SetPixel(1,2,b); bgTex.SetPixel(2,2,b);
        bgTex.Apply();
    }
    private static Color textColor=Color.white;
    public static void SetTextColor(float[] col){
        textColor=new Color(col[0],col[1],col[2],col[3]);
        if(winStyle!=null) winStyle.UpdateTextColor();
        foreach(var s in styledic.Values) s.UpdateTextColor();
        updDate=ComShWM.updateTime;
    }
    public static void ChgTextColor(GUIStyle style){
        style.onNormal.textColor= style.normal.textColor=
        style.onHover.textColor= style.hover.textColor=
        style.onFocused.textColor= style.focused.textColor=
        style.onActive.textColor= style.active.textColor= textColor;
    }
}
// フォントサイズ固定のGUIスタイル。タイトルバーとウィンドウ背景
public class WindowStyle {
	public GUIStyle closebtn;
	public GUIStyle title;
    public GUIStyle window;
    private GUIContent cbCont;
    public Vector2 cbSize;
    public WindowStyle(){
        var zero=new RectOffset(0,0,0,0);
        cbCont=new GUIContent("☓");
        closebtn=new GUIStyle(GUI.skin.button);
        closebtn.fontSize=10;
        closebtn.margin=zero;
        closebtn.padding=new RectOffset(2,2,2,2);
        closebtn.alignment=TextAnchor.MiddleCenter;
        CalcCbSize();
        closebtn.fixedWidth=cbSize.x;
        title=new GUIStyle(GUI.skin.label);
        title.margin=title.padding=zero;
        title.fontSize=12;
        title.alignment=TextAnchor.MiddleCenter;
        title.padding.left=(int)(cbSize.x);
        window=new GUIStyle(GUI.skin.window);

        var bg=GUI.skin.window.onNormal.background;
        if(bg!=null) winBg0=CloneTex(bg);
        bg=GUI.skin.window.normal.background;
        if(bg!=null) winBg1=CloneTex(bg);
    }
    private Vector2 CalcCbSize(){ cbSize=closebtn.CalcSize(cbCont); return cbSize; }

    public void UpdateTextColor(){
        PanelStyleCache.ChgTextColor(title);
    }

    private Texture2D winBg0;
    private Texture2D winBg1;
    public void UpdateBgColor(Color c){
        if(winBg0!=null) window.onNormal.background=ChgWinBgColor(winBg0,c); 
        if(winBg1!=null) window.normal.background=ChgWinBgColor(winBg1,c); 
    }
    // デフォ背景のタイトルバーと窓本体の明度差
    private float diff_v=float.MinValue;

    // window背景テクスチャ(複製)を実際に書き換える
    private Texture2D ChgWinBgColor(Texture2D t,Color col){
        // 左下が(0,0)
        var bar=t.GetPixel(t.width/2,t.height-5);
        var body=t.GetPixel(t.width/2,4);
        if(diff_v<0){
            float vbar=Mathf.Max(bar.r,bar.g,bar.b);
            float vbody=Mathf.Max(body.r,body.g,body.b);
            float dv=vbar-vbody;
            if(Mathf.Approximately(dv,0)) dv=vbar*(bar.a-body.a);
            diff_v=Mathf.Abs(dv);
            if(diff_v>0.4) diff_v=0.4f;
        }
        Color.RGBToHSV(col,out float h,out float s,out float v);
        if(v<0.5f) v+=diff_v; else v-=diff_v;
        Color barcol=Color.HSVToRGB(h,s,v);
        barcol.a=col.a; // 透明度は同じにする
        for(int y=0; y<t.height; y++) for(int x=0; x<t.width; x++){
            var c=t.GetPixel(x,y);
            Color c2;
            if(c.r==body.r&&c.g==body.g&&c.b==body.b&&c.a==body.a) c2=col; 
            else if(c.r==bar.r&&c.g==bar.g&&c.b==bar.b&&c.a==bar.a) c2=barcol; 
            else c2=c;
            t.SetPixel(x,y,c2);
        }
        t.Apply();
        return t;
    }
    // デフォ背景テクスチャを加工用に複製
    private Texture2D CloneTex(Texture2D bg){ 
        RenderTexture rt=RenderTexture.GetTemporary(bg.width,bg.height);
        Graphics.Blit(bg,rt);
        var bak=RenderTexture.active;
        RenderTexture.active=rt;
        var ret=new Texture2D(rt.width,rt.height);
        ret.wrapMode=bg.wrapMode;
        ret.anisoLevel=0;
        ret.mipMapBias=0;
        ret.filterMode=FilterMode.Point;
        ret.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);
        ret.Apply();
        RenderTexture.active=bak;
        RenderTexture.ReleaseTemporary(rt);
        return ret;
    }
} 

// フォントサイズ可変のGUIスタイル
public class PanelStyles {
	public GUIStyle label;
	public GUIStyle button;
	public GUIStyle toggle;
	public GUIStyle text;
	public GUIStyle comboBtn;
	public GUIStyle comboWin;
	public GUIStyle comboItem;

    public PanelStyles(int fsize){
        var p=new RectOffset(2,2,2,2);
        var m=new RectOffset(0,0,0,0);
        label=new GUIStyle(GUI.skin.label);
        button=new GUIStyle(GUI.skin.button);
        toggle=new GUIStyle(GUI.skin.toggle);
        text=new GUIStyle(GUI.skin.textField);
        comboBtn=new GUIStyle(GUI.skin.button);
        comboWin=new GUIStyle(GUI.skin.box);
        comboItem=new GUIStyle(GUI.skin.label);
        label.fontSize=button.fontSize=text.fontSize=toggle.fontSize=
            comboBtn.fontSize=comboItem.fontSize=fsize;
        label.padding=button.padding=text.padding=
            comboBtn.padding=comboItem.padding=comboWin.padding=p;
        label.margin=button.margin=text.margin=
            comboBtn.margin=comboItem.margin=comboWin.margin=m;
        label.wordWrap=button.wordWrap=text.wordWrap=toggle.wordWrap=
            comboBtn.wordWrap=comboItem.wordWrap=comboWin.wordWrap=false;
        label.alignment=text.alignment=toggle.alignment=comboItem.alignment=TextAnchor.MiddleLeft;
        button.alignment=comboBtn.alignment=TextAnchor.MiddleCenter;
    }
    public void UpdateBgTex(Texture2D tx){
        comboWin.border=new RectOffset(1,1,1,1);
        comboWin.normal.background=tx;
    }
    public void UpdateTextColor(){
        PanelStyleCache.ChgTextColor(label);
        PanelStyleCache.ChgTextColor(toggle);
        PanelStyleCache.ChgTextColor(comboItem);
    }
}
}
