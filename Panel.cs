using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

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
    public bool allowRichText=false;

    public const int TITLEHEIGHT=17;
    public Rect windowrect=new Rect(0,0,100,100);
    public Rect titleRect=new Rect(0,2,100,TITLEHEIGHT);
    public Rect termRect=new Rect(0,2,0,0);
    public Rect closeRect=new Rect(0,2,0,0);

    public void OnVisibleChange(bool v){}
    private bool lastEnabled=false;
    public void UpdateStyle(){
        ws=PanelStyleCache.GetWindowStyles();
        styleDate=PanelStyleCache.updDate;
        style=PanelStyleCache.GetStyles(fontSize,allowRichText);
        titleRect.width=windowrect.width;
        termRect.width=closeRect.width=ws.cbSize.x;
        termRect.height=closeRect.height=ws.cbSize.y;
        closeRect.x=windowrect.width-closeRect.width-2;
        termRect.x=closeRect.x-closeRect.width;
        termRect.y=closeRect.y=TITLEHEIGHT-closeRect.height;
    }
    public void Draw(){
        if(PanelStyleCache.updDate>styleDate) UpdateStyle();

        float w=Screen.width,h=Screen.height;
        windowrect.x=Mathf.Clamp(windowrect.x,-windowrect.width+100,w-100);
        windowrect.y=Mathf.Clamp(windowrect.y,-windowrect.height+50,h-50);

	    windowrect=GUI.Window(wid,windowrect,Panel,"",ws.window);
        if(ComboWin.visible && ReferenceEquals(ComboWin.listbox.panel,this)){
            ComboWin.Position(windowrect);
            ComboWin.Draw();
            GUI.BringWindowToFront(ComboWin.wid);
        }

        shell.env["panelx"]=((int)windowrect.x).ToString();
        shell.env["panely"]=((int)windowrect.y).ToString();

        Vector2 mp=Event.current.mousePosition;
        if(mp.x!=0 && mp.y!=0){
            bool enabled=windowrect.Contains(mp);
            if(enabled){
                Input.ResetInputAxes();
                if(!lastEnabled){
                    if(UIInput.selection!=null) UIInput.selection.isSelected=false;
                    var ia=GameObject.FindObjectsOfType<UIInput>();
                    foreach(var ip in ia) ip.isSelected=false;
                    UIInput.selection=null;
                    UICamera.selectedObject=null;
                }
            }else if(lastEnabled) GUIUtility.keyboardControl=0;
            lastEnabled=enabled;
        }
    }
    public void Panel(int wid){
        GUI.Label(titleRect,title,ws.title);
        if(GUI.Button(termRect,"Ｔ",ws.closebtn)){ ComShWM.ToggleTerm(); return; }
        if(GUI.Button(closeRect,"☓",ws.closebtn)){ ComShWM.ClosePanel(wid); return; }
        if(tabs[""]!=null) tabs[""].Invoke(0,0);
        GUI.DragWindow();
    }

    public void OnClose(){
        if(ComboWin.listbox.panel==this) ComboWin.visible=false;
        if(psrOnClose!=null){ shell.InterpretParser(psrOnClose); shell.exitq=false; }
        shell.panel=null;
    }

    public delegate void DrawElements(int left,int top);
    private string currenttabname="";
    public List<string> tabnames=new List<string>();
    public Dictionary<string,DrawElements> tabs=new Dictionary<string,DrawElements>();
    private ComShParser psrOnClose=null;

    public ComShPanel(ComShInterpreter sh,int x,int y,int w,int h,string t,int fsize,int wid){
        title=t;
        shell=sh;
        windowrect.x=x; windowrect.y=y; windowrect.width=w; windowrect.height=h;
        SetTab("");
        fontSize=fsize;
        psrOnClose=null;
        sh.panel=this;
        this.wid=wid;
    }
    public void SetTab(string name){
        if(!tabs.ContainsKey(name)){
            tabs.Add(name,null);
            grids.Add(name,new Rect(0,0,1,1));
            if(name!="") tabnames.Add(name);
        }
        currenttabname=name;
    }
    public void SetOnClose(ComShParser p){ psrOnClose=p; }
    public Dictionary<string,Rect> grids=new Dictionary<string,Rect>();
    public void SetGrid(float[] xywh){
        Rect r=new Rect(xywh[0],xywh[1],xywh[2],xywh[3]);
        grids[currenttabname]=r;
    }
    public int[] Plot(float[] xywh){
        Rect grid=grids[currenttabname];
        return new int[]{
            (int)(grid.x+xywh[0]*grid.width),
            (int)(grid.y+xywh[1]*grid.height),
            (int)(grid.width*xywh[2]),
            (int)(grid.height*xywh[3])
        };
    }
    public void AddButton(int x,int y,int w,int h,string l,ComShParser p){
        var r=new Rect(x,y+((currenttabname=="")?TITLEHEIGHT:0),w,h);
        tabs[currenttabname]+=(left,top)=>{
            if(GUI.Button(r,l,style.button)){
                p.Reset();
                shell.InterpretParser(p);
                shell.exitq=false;
            }
        };
    }
    public void AddRbutton(int x,int y,int w,int h,string l,ComShParser p,float dt,float ddt,float mindt){
        var r=new Rect(x,y+((currenttabname=="")?TITLEHEIGHT:0),w,h);
        float delay=dt;
        long expire=0;
        tabs[currenttabname]+=(left,top)=>{
            if(GUI.RepeatButton(r,l,style.button)){
                long now=DateTime.UtcNow.Ticks/TimeSpan.TicksPerMillisecond;
                if(now>expire){
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                    expire=now+(long)delay;
                    delay-=ddt;
                    if(delay<mindt) delay=mindt;
                }
            }else{
                var ev=Event.current.type;
                if(ev!=EventType.Repaint&&ev!=EventType.Layout&&ev!=EventType.MouseDrag){
                    delay=dt;
                }
            }
        };
    }
    public void AddToggle(int x,int y,int w,int h,string l,ComShParser p,string name,bool v0){
        var r=new Rect(x,y+((currenttabname=="")?TITLEHEIGHT:0),w,h);
        shell.env[name]=v0?"1":"0";
        if(p==null){
            tabs[currenttabname]+=(left,top)=>{
                bool chk=ParseUtil.ParseFloat(shell.env[name],0)==1f;
                bool c=GUI.Toggle(r,chk,l,style.toggle);
                if(c!=chk) shell.env[name]=c?"1":"0";
            };
        }else{
            tabs[currenttabname]+=(left,top)=>{
                bool chk=ParseUtil.ParseFloat(shell.env[name],0)==1f;
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
    public void AddRadio(int x,int y,int w,int h,string l,ComShParser p,string name,string val){
        var r=new Rect(x,y+((currenttabname=="")?TITLEHEIGHT:0),w,h);
        if(p==null){
            tabs[currenttabname]+=(left,top)=>{
                bool chk=(shell.env[name]==val);
                bool c=GUI.Toggle(r,chk,l,style.toggle);
                if(c && !chk) shell.env[name]=val;
            };
        }else{
            tabs[currenttabname]+=(left,top)=>{
                bool chk=(shell.env[name]==val);
                bool c=GUI.Toggle(r,chk,l,style.toggle);
                if(c && !chk){
                    shell.env[name]=val;
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                }
            };
        }
    }
    public void AddTextField(int x,int y,int w,int h,ComShParser p,string name,string v0,float delay){
        var r=new Rect(x,y+((currenttabname=="")?TITLEHEIGHT:0),w,h);
        shell.env[name]=v0;
        if(p==null){
            tabs[currenttabname]+=(left,top)=>{
                string txt=shell.env[name];
                string t=GUI.TextField(r,txt,style.text);
                if(t!=txt) shell.env[name]=t;
            };
        }else{
            if(delay>0){
                long expire=0;
                tabs[currenttabname]+=(left,top)=>{
                    string txt=shell.env[name];
                    string t=GUI.TextField(r,txt,style.text);
                    if(t!=txt){
                        shell.env[name]=t;
                        expire=DateTime.UtcNow.Ticks/TimeSpan.TicksPerMillisecond+(long)delay;
                    }else if(expire>0){
                        long now=DateTime.UtcNow.Ticks/TimeSpan.TicksPerMillisecond;
                        if(now>expire){
                            p.Reset();
                            shell.InterpretParser(p);
                            shell.exitq=false;
                            expire=0;
                        }
                    }
                };
            }else{
                tabs[currenttabname]+=(left,top)=>{
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
    }
    public void AddCombo(int x,int y,int w,int h,ComShParser p,string name,string v0,string[] items,char dlmt){
        int ty=y+((currenttabname=="")?TITLEHEIGHT:0);
        var r=new Rect(x,ty,w,h);
        var rb=new Rect(x+w,ty,h,h);
        const int maxh=200;
        var ci=new ListBoxItems(items,dlmt,false);
        string prev=shell.env[name]=v0;
        float scry=0;
        ListBox.OnSelectionChange onselect=(string val)=>{
            shell.env[name]=val;
            scry=ComboWin.listbox.scr.y;
            if(p!=null){
                p.Reset();
                shell.InterpretParser(p);
                shell.exitq=false;
            }
        };
        tabs[currenttabname]+=(left,top)=>{
            string txt=ci.GetLabel(shell.env[name]);
            string t=GUI.TextField(r,txt,style.text);
            if(t!=txt){
                shell.env[name]=ci.GetValue(t);
                if(p!=null){
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                }
            }
            if(GUI.Button(rb,"▼",style.comboBtn)){
                ComboWin.SetItems(this,ci,onselect,left+(int)r.x,top+(int)(r.y+r.height),w,maxh);
                ComboWin.Position(windowrect);
                ComboWin.Toggle(scry);
            }
        };
    }
    public void AddCombo2(int x,int y,int w,int h,ComShParser p,string name,string v0,ComShParser lst,char dlmt){
        int ty=y+((currenttabname=="")?TITLEHEIGHT:0);
        var r=new Rect(x,ty,w,h);
        var rb=new Rect(x+w,ty,h,h);
        const int maxh=200;
        float scry=0;
        string prev=shell.env[name]=v0;
        var ci=MakeItems(shell,lst,dlmt,false);
        ListBox.OnSelectionChange onselect=(string val)=>{
            shell.env[name]=val;
            scry=ComboWin.listbox.scr.y;
            if(p!=null){
                p.Reset();
                shell.InterpretParser(p);
                shell.exitq=false;
            }
        };
        tabs[currenttabname]+=(left,top)=>{
            string val=shell.env[name];
            if(prev!=val){ ci=MakeItems(shell,lst,dlmt,false,val); prev=val; }
            string txt=ci.GetLabel(shell.env[name]);
            string t=GUI.TextField(r,txt,style.text);
            if(t!=txt){
                val=shell.env[name]=ci.GetValue(t);
                if(p!=null){
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                }
            }
            if(GUI.Button(rb,"▼",style.comboBtn)){
                ci=MakeItems(shell,lst,dlmt,false,val);
                ComboWin.SetItems(this,ci,onselect,left+(int)r.x,top+(int)(r.y+r.height),w,maxh);
                ComboWin.Position(windowrect);
                ComboWin.Toggle(scry);
            }
        };
    }
    public void AddSwitch(int x,int y,int w,int h,ComShParser p,string name,string v0,string[] items,char dlmt){
        var r=new Rect(x,y+((currenttabname=="")?TITLEHEIGHT:0),w,h);
        var li=new ListBoxItems(items,dlmt,false);
        int idx=li.GetIdx(v0);
        if(idx<0) idx=0;
        li.Select(idx,false,false);
        string l=li.labels[idx];
        string prev=shell.env[name]=v0;
        tabs[currenttabname]+=(left,top)=>{
            var b=Event.current.button;
            if(GUI.Button(r,l,style.button)){
                idx=(li.values.Length+li.lastIdx+(b==1?-1:1))%li.values.Length;
                shell.env[name]=li.values[idx];
                li.Select(idx,false,false);
                l=li.labels[idx];
                if(p!=null){
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                }
            }
        };
    }
    public void AddSwitch2(int x,int y,int w,int h,ComShParser p,string name,string v0,string lstvar,char dlmt){
        var r=new Rect(x,y+((currenttabname=="")?TITLEHEIGHT:0),w,h);
        string lstprev=shell.env[lstvar];
        var li=MakeItems(lstprev,dlmt,false);
        int idx=li.GetIdx(v0);
        if(idx<0) idx=0;
        li.Select(idx,false,false);
        string l=li.labels[idx];
        string prev=shell.env[name]=v0;
        tabs[currenttabname]+=(left,top)=>{
            string val=shell.env[name],lst=shell.env[lstvar];
            if(prev!=val || lstprev!=lst){
                li=MakeItems(lst,dlmt,false,val);
                prev=val; lstprev=lst;
            }
            var b=Event.current.button;
            if(GUI.Button(r,l,style.button)){
                idx=(li.values.Length+li.lastIdx+(b==1?-1:1))%li.values.Length;
                shell.env[name]=li.values[idx];
                li.Select(idx,false,false);
                l=li.labels[idx];
                if(p!=null){
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                }
            }
        };
    }
    public void AddListBox(int x,int y,int w,int h,ComShParser p,string name,string v0,string[] items,int max,char dlmt){
        int ty=y+((currenttabname=="")?TITLEHEIGHT:0);
        var li=new ListBoxItems(items,dlmt,max==0);
        var lb=new ListBox();
        string prev=shell.env[name]=v0;
        li.SetSelectedValue(v0);
        lb.SetItems(this,li,null,x,ty,w,h);
        tabs[currenttabname]+=(left,top)=>{
            int si=lb.Select();
            if(si!=-1){
                shell.env[name]=li.GetSelectedValue();
                if(p!=null){
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                }
            }
        };
    }
    public void AddListBox2(int x,int y,int w,int h,ComShParser p,string name,string v0,string lstvar,int max,char dlmt){
        int ty=y+((currenttabname=="")?TITLEHEIGHT:0);
        string prev=shell.env[name]=v0;
        string lstprev=shell.env[lstvar];
        var li=MakeItems(lstprev,dlmt,max==0);
        var lb=new ListBox();
        li.SetSelectedValue(v0);
        lb.SetItems(this,li,null,x,ty,w,h);
        tabs[currenttabname]+=(left,top)=>{
            string val=shell.env[name],lst=shell.env[lstvar];
            if(prev!=val || lstprev!=lst){
                li=MakeItems(lst,dlmt,max==0,val); // 選択肢最新化
                prev=val; lstprev=lst;
                lb.SetItems(this,li,null,x,ty,w,h);
            }
            int si=lb.Select();
            if(si!=-1){
                shell.env[name]=li.GetSelectedValue();
                if(p!=null){
                    p.Reset();
                    shell.InterpretParser(p);
                    shell.exitq=false;
                }
            }
        };
    }


    private int pageNest=0;
    public void AddPage(int x,int y,int w,int h,string name,string v0){
        var r=new Rect(x,y+((currenttabname=="")?TITLEHEIGHT:0),w,h);
        var vr=new Rect(0,0,w-1,h-1);
        shell.env[name]=v0;
        tabs[currenttabname]+=(left,top)=>{
            string p=shell.env[name];
            if(p=="") return;
            if(tabs.TryGetValue(p,out DrawElements draw)){
                if(pageNest<8){
                    pageNest++;
                    GUI.BeginScrollView(r,Vector2.zero,vr);
                    draw.Invoke(left+(int)r.x,top+(int)r.y);
                    GUI.EndScrollView();
                    pageNest--;
                }
            }
        };
    }

    private static string[] items0={""};
    private static ListBoxItems MakeItems(ComShInterpreter sh,ComShParser plst,char dlmt,bool multi,string value=null){
        var sbo=new ComShInterpreter.SubShOutput();
        ComShInterpreter child = new ComShInterpreter(new ComShInterpreter.Output(sbo.Output),sh.env,sh.func,sh.ns);
        plst.Reset();
        int ret=child.InterpretParser(plst);
        if(ret<0) return new ListBoxItems(items0,items0,multi);
        return MakeItems(sbo.GetSubShResult(),dlmt,multi,value);
    }
    private static ListBoxItems MakeItems(string lststr,char dlmt,bool multi,string value=null){
        string[] items=ParseUtil.Chomp(lststr).Split(ParseUtil.lf);
        var lb=new ListBoxItems(items,dlmt,multi);
        if(value!=null) lb.SetSelectedValue(value);
        return lb;
    }
    public void AddSlider(int x,int y,int w,int h,ComShParser p,string name,float v0,float min,float max,float delay){
        var r=new Rect(x,y+((currenttabname=="")?TITLEHEIGHT:0),w,h);
        shell.env[name]=shell.fmt.FVal(v0);
        string old=shell.env[name];
        float val=v0;
        if(p==null){
            tabs[currenttabname]+=(left,top)=>{
                string cur=shell.env[name];
                if(old!=cur){ float.TryParse(cur,out val); old=cur; }
                float v=GUI.HorizontalSlider(r,val,min,max);
                if(v!=val) shell.env[name]=shell.fmt.FVal(v);
            };
        }else{
            if(delay>0){
                long expire=0;
                tabs[currenttabname]+=(left,top)=>{
                    string cur=shell.env[name];
                    if(old!=cur){ float.TryParse(cur,out val); old=cur; }
                    float v=GUI.HorizontalSlider(r,val,min,max);
                    if(v!=val){
                        shell.env[name]=shell.fmt.FVal(v);
                        expire=DateTime.UtcNow.Ticks/TimeSpan.TicksPerMillisecond+(long)delay;
                    }else if(expire>0){
                        long now=DateTime.UtcNow.Ticks/TimeSpan.TicksPerMillisecond;
                        if(now>expire){
                            p.Reset();
                            shell.InterpretParser(p);
                            shell.exitq=false;
                            expire=0;
                        }
                    }
                };
            }else{
                tabs[currenttabname]+=(left,top)=>{
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
    }
    public void AddVSlider(int x,int y,int w,int h,ComShParser p,string name,float v0,float min,float max,float delay){
        var r=new Rect(x,y+((currenttabname=="")?TITLEHEIGHT:0),w,h);
        shell.env[name]=shell.fmt.FVal(v0);
        string old=shell.env[name];
        float val=v0;
        if(p==null){
            tabs[currenttabname]+=(left,top)=>{
                string cur=shell.env[name];
                if(old!=cur){ float.TryParse(cur,out val); old=cur; }
                float v=GUI.VerticalSlider(r,val,min,max);
                if(v!=val) shell.env[name]=shell.fmt.FVal(v);
            };
        }else{
            if(delay>0){
                long expire=0;
                tabs[currenttabname]+=(left,top)=>{
                    string cur=shell.env[name];
                    if(old!=cur){ float.TryParse(cur,out val); old=cur; }
                    float v=GUI.VerticalSlider(r,val,min,max);
                    if(v!=val){
                        shell.env[name]=shell.fmt.FVal(v);
                        expire=DateTime.UtcNow.Ticks/TimeSpan.TicksPerMillisecond+(long)delay;
                    }else if(expire>0){
                        long now=DateTime.UtcNow.Ticks/TimeSpan.TicksPerMillisecond;
                        if(now>expire){
                            p.Reset();
                            shell.InterpretParser(p);
                            shell.exitq=false;
                            expire=0;
                        }
                    }
                };
            }else{
                tabs[currenttabname]+=(left,top)=>{
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
    }
    public void AddLabel(int x,int y,int w,int h,string l){
        var r=new Rect(x,y+((currenttabname=="")?TITLEHEIGHT:0),w,h);
        tabs[currenttabname]+=(left,top)=>{ GUI.Label(r,l,style.label); };
    }
    public void AddLabel2(int x,int y,int w,int h,string name){
        var r=new Rect(x,y+((currenttabname=="")?TITLEHEIGHT:0),w,h);
        tabs[currenttabname]+=(left,top)=>{ GUI.Label(r,shell.env[name],style.label); };
    }
}
// コンボボックスのポップアップ
public static class ComboWin {
    public static int wid=ComShProperties.windowID+2;
    public static Rect windowrect=new Rect(0,0,100,100);
    public static Vector2 offset=new Vector2(0,0);
    public static ListBox listbox=new ListBox(true);
    public static bool visible=false;

    public static void SetItems(ComShPanel p,ListBoxItems it,ListBox.OnSelectionChange cb,int x,int y,int w,int h){
        offset.x=x; offset.y=y;
        listbox.SetItems(p,it,cb,0,0,w,h);
        windowrect.width=listbox.viewRect.width;
        windowrect.height=listbox.viewRect.height;
    }
    public static void Position(Rect r){
        windowrect.x=r.x+offset.x; windowrect.y=r.y+offset.y;
    }

    public static void Toggle(float y){
        visible=!visible;
        if(visible) listbox.scr.y=y;
    }
    public static void Draw(){
        listbox.viewRect.x=1;
        listbox.viewRect.y=1;
        windowrect.width=listbox.viewRect.width+2;
        windowrect.height=listbox.viewRect.height+2;
        windowrect=GUI.Window(wid,windowrect,Select,"",listbox.panel.style.comboWin);
        if(windowrect.Contains(Event.current.mousePosition)) Input.ResetInputAxes();
    }
    public static void Select(int wid){
        int si=listbox.Select();
        if(si!=-1) visible=false;
    }
}

public class ListBoxItems {
    public string[] labels;
    public string[] values;
    public List<int> selected=new List<int>();
    public int lastIdx=-1;
    public bool multiSelect=true;
    public ListBoxItems(string[] la,string[] va,bool multi,List<int> sel=null){
        labels=la; values=va;
        multiSelect=multi;
        if(sel!=null) selected=sel; else selected.Clear();
    }
    public ListBoxItems(string[] items,char dlmt,bool multi,List<int> sel=null){
        if(dlmt==0) labels=values=items; else {
            labels=new string[items.Length];
            values=new string[items.Length];
            for(int i=0; i<items.Length; i++){
                string[] sa=ParseUtil.LeftAndRight(items[i],dlmt);
                labels[i]=sa[0]; values[i]=sa[1];
            }
        }
        multiSelect=multi;
        if(sel!=null) selected=sel; else selected.Clear();
    }
    public int GetIdx(string value){
        for(int i=0; i<values.Length; i++) if(values[i]==value) return i;
        return -1;
    }
    public string GetLabel(string value){
        for(int i=0; i<values.Length; i++) if(values[i]==value) return labels[i];
        return "";
    }
    public string GetValue(string label){
        for(int i=0; i<labels.Length; i++) if(labels[i]==label) return values[i];
        return "";
    }
    public string GetSelectedValue(){
        if(selected.Count==0) return "";
        if(!multiSelect) return values[selected[0]];
        selected.Sort();
        var sb=new StringBuilder();
        sb.Append(values[selected[0]]);
        for(int i=1; i<selected.Count; i++) sb.Append('\n').Append(values[selected[i]]);
        return sb.ToString();
    }
    public void SetSelectedValue(string val){
        selected.Clear();
        if(!multiSelect) Select(GetIdx(val),false,false);
        else{
            var va=val.Split('\n');
            for(int i=0; i<va.Length; i++) Select(GetIdx(va[i]),true,false);
        }
    }
    public void Select(int idx,bool append,bool range){
        if(idx<0 || idx>=values.Length) return;
        if(multiSelect){
            if(append){
                if(IsSelected(idx)){ selected.Remove(idx); lastIdx=-1; }
                else {selected.Add(idx); lastIdx=idx; }
                return;
            }else if(lastIdx>=0 && range){
                if(lastIdx>=idx){
                    for(int i=idx; i<lastIdx; i++) if(!IsSelected(i)) selected.Add(i);
                }else{
                    for(int i=idx; i>lastIdx; i--) if(!IsSelected(i)) selected.Add(i);
                }
                lastIdx=idx;
                return;
            }
        }
        selected.Clear();
        selected.Add(idx);
        lastIdx=idx;
    }
    public List<int> GetSelectedIndex(){ selected.Sort(); return selected; }
    public List<string> GetSelectedValues(){
        selected.Sort();
        var ret=new List<string>(selected.Count);
        foreach(var i in selected) ret.Add(values[i]);
        return ret;
    }
    public List<string> GetSelectedLabels(){
        selected.Sort();
        var ret=new List<string>(selected.Count);
        foreach(var i in selected) ret.Add(labels[i]);
        return ret;
    }
    public bool IsSelected(int idx){ return selected.Contains(idx); }
}

public class ListBox {

    private ListBoxItems items;
    public delegate void OnSelectionChange(string val);
    public OnSelectionChange callback;
    public ComShPanel panel;
    public bool isCombo=false;

    private static GUIContent ichimoji=new GUIContent("漢");

    public ListBox(){}
    public ListBox(bool comboq){isCombo=comboq;}
    public ListBox(ComShPanel p,ListBoxItems it,OnSelectionChange cb,int x,int y,int w,int h){ SetItems(p,it,cb,x,y,w,h);}
    public void SetItems(ComShPanel p,ListBoxItems it,OnSelectionChange cb,int x,int y,int w,int h){
        panel=p;
        items=it;
        callback=cb;
        groupRect.x=x;
        groupRect.y=y;
        viewRect.width=contentRect.width=w;
        viewRect.height=contentRect.height=h;
        groupRect.width=w+2;
        groupRect.height=h+2;
        styleok=false;
    }
    public Rect groupRect=new Rect(0,0,100,100);
    public Rect viewRect=new Rect(1,1,100,100);
    public Rect contentRect=new Rect(0,0,100,100);
    private bool styleok=false;
    public Vector2 scr=new Vector2(0,0);
    public int Select(){
        if(!styleok){
            float h=panel.style.comboItem.CalcSize(ichimoji).y;
            float w=GUI.skin.verticalScrollbar.fixedWidth;
            contentRect.height=items.labels.Length*h;
            if(isCombo) viewRect.width+=w+2;
            else if(contentRect.height>viewRect.height) viewRect.width+=w;
            groupRect.width=viewRect.width+2;
            if(isCombo) viewRect.height=Math.Min(contentRect.height,viewRect.height);
            groupRect.height=viewRect.height+2;
            styleok=true;
        }
        if(isCombo) scr=GUI.BeginScrollView(viewRect,scr,contentRect,false,true);
        else{
            GUI.BeginGroup(groupRect,panel.style.listBox);
            scr=GUI.BeginScrollView(viewRect,scr,contentRect,false,false);
        }
        int si=GUI.SelectionGrid(contentRect,-1,items.labels,1,panel.style.comboItem);
        if(si!=-1){
            var ev=Event.current;
            items.Select(si,ev.control||ev.button==1,ev.shift||ev.button==2);
            if(callback!=null) callback.Invoke(items.GetSelectedValue());
        }
        if(Event.current.type==EventType.Repaint){
            var mpos=Event.current.mousePosition;
            float h=contentRect.height/items.labels.Length;
            foreach(int i in items.selected){
                Rect r=new Rect(contentRect.x,contentRect.y+h*i,contentRect.width,h);
                panel.style.comboItem.Draw(r,items.labels[i],r.Contains(mpos),true,true,false);
            }
        }
        GUI.EndScrollView();
        if(!isCombo) GUI.EndGroup();
        return si;
    }
}
public static class PanelStyleCache {
    public static long updDate=DateTime.UtcNow.Ticks;
    public static void Dirty(){ updDate=DateTime.UtcNow.Ticks; }
    private static Dictionary<int,PanelStyles> styledic=new Dictionary<int,PanelStyles>();
    private static Dictionary<int,PanelStyles> styledicRich=new Dictionary<int,PanelStyles>();
    public static PanelStyles GetStyles(int fsize,bool richq=false){
        var dic=styledic;
        if(richq) dic=styledicRich;
        if(dic.ContainsKey(fsize)) return dic[fsize];
        updDate=ComShWM.updateTime;
        var ps=new PanelStyles(fsize,richq);
        ps.UpdateTextColor();
        if(bgTex!=null) ps.UpdateBgTex(bgTex);
        dic[fsize]=ps;
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
    public static Color bgColor=Color.clear;
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
    public static Color textColor=Color.white;
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
        if(bg!=null) winBg0=TextureUtil.CloneBitmap(bg);
        bg=GUI.skin.window.normal.background;
        if(bg!=null) winBg1=TextureUtil.CloneBitmap(bg);
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
	public GUIStyle listBox;

    public PanelStyles(int fsize,bool richq){
        var p=new RectOffset(2,2,2,2);
        var m=new RectOffset(0,0,0,0);
        label=new GUIStyle(GUI.skin.label);
        button=new GUIStyle(GUI.skin.button);
        toggle=new GUIStyle(GUI.skin.toggle);
        text=new GUIStyle(GUI.skin.textField);
        comboBtn=new GUIStyle(GUI.skin.button);
        comboWin=new GUIStyle(GUI.skin.box);
        comboItem=new GUIStyle(GUI.skin.label);
        listBox=new GUIStyle(GUI.skin.textArea);
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
        listBox.hover.background=listBox.normal.background;

        label.richText=button.richText=toggle.richText=comboItem.richText=richq;
    }
    public void UpdateBgTex(Texture2D tx){
        comboWin.border=new RectOffset(1,1,1,1);
        comboWin.normal.background=tx;

        var bg0=new Texture2D(1,1);
        bg0.SetPixel(0,0,new Color(1,1,1,0.15f));
        bg0.Apply();
        comboItem.hover.background=bg0;

        var sc=GUI.skin.settings.selectionColor;
        var bg1=new Texture2D(1,1);
        bg1.SetPixel(0,0,sc);
        bg1.Apply();
        comboItem.onNormal.background=bg1;

        var bg2=new Texture2D(1,1);
        bg2.SetPixel(0,0,new Color(sc.r+0.1f,sc.g+0.1f,sc.b+0.1f,1));
        bg2.Apply();
        comboItem.onHover.background=bg2;
    }
    public void UpdateTextColor(){
        PanelStyleCache.ChgTextColor(label);
        PanelStyleCache.ChgTextColor(toggle);
        PanelStyleCache.ChgTextColor(comboItem);
    }
}
}
