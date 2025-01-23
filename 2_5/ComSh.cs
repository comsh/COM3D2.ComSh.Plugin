using System;
using UnityEngine;
using UnityInjector.Attributes;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace COM3D2.ComSh.Plugin {
	[   PluginFilter("COM3D2x64"), PluginFilter("COM3D2VRx64"),
        PluginFilter("COM3D2OHx64"), PluginFilter("COM3D2OHVRx64"),
        PluginName("COM3D2.ComSh.Plugin"), PluginVersion("1.0.0.0") ]

	// イベント
	public class ComSh : UnityInjector.PluginBase {
		public void Awake() { SceneManager.sceneLoaded+=OnSceneLoaded; DontDestroyOnLoad(this); }
		public void Update() { ComShWM.Update(); ComShBg.OnUpdate(); }
		public void LateUpdate() { ComShBg.OnLateUpdate(); }
		public void OnGUI() { ComShWM.OnGUI(); }
        public void OnApplicationQuit(){ DataFiles.Clean(); }
	    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){
            StudioMode.SceneChg(SceneManager.GetActiveScene().name);
        }
	}

	public static class ComShWM {
        public static ComShTerminal terminal;
        public static ComShMenu menu;

        public static bool paused=false;
        private static int show=1;
        private static int lastwin=1;
		private static bool visible=false;
        private static long toggleTick=0;
        public static void Toggle() {
            if (updateTime>toggleTick){
                SetVisible(!visible);
                toggleTick=updateTime+200*TimeSpan.TicksPerMillisecond;
            }
        }
        public static bool IsVisible(){return visible;}
        public static void SetVisible(bool v) {
            if(paused) return;
            var t=visible;
            visible=v;
            vchanged=t!=visible;
        }
        public static long updateTime=DateTime.UtcNow.Ticks;
        public static void Update(){
            updateTime=DateTime.UtcNow.Ticks;
            if(ComShProperties.Wakeupq()) Toggle();
        }
        private static bool vchanged=false;
        public static void OnGUI(){
            if(terminal==null){
                terminal=new ComShTerminal();
                menu=new ComShMenu();
                terminal.Init();
            }
            if(vchanged){
                var f=show;
                if(show==0 && panelCnt==0){
                    f=lastwin;
                    if(visible) show=f;
                }
                if((f&1)>0) terminal.OnVisibleChange(visible);
                if((f&2)>0) menu.OnVisibleChange(visible);
                if(panelCnt>0) for(int i=0; i<panel.Length; i++) if(panel[i]!=null) panel[i].OnVisibleChange(visible);
                vchanged=false;
            }
            if(visible){
                if((show&1)>0) terminal.Draw();
                if((show&2)>0) menu.Draw();
                if(panelCnt>0) for(int i=0; i<panel.Length; i++) if(panel[i]!=null) panel[i].Draw();
                if(show==0 && panelCnt==0) SetVisible(false);
            }
        }
        public static void ShowTerm(){
            var t=show; show|=1;
            if(t!=show) terminal.OnVisibleChange(true);
        }
        public static void HideTerm(){
            var t=show; show&=~1;
            if(t!=show) terminal.OnVisibleChange(false);
            if(show==0) lastwin=1;
        }
        public static void ToggleTerm(){
            show^=1;
            terminal.OnVisibleChange((show&1)==1);
            if(show==0) lastwin=1;
        }
        public static void ShowMenu(){
            var t=show; show|=2;
            if(t!=show) menu.OnVisibleChange(true);
        }
        public static void HideMenu(){
            var t=show; show&=~2;
            if(t!=show) menu.OnVisibleChange(false);
            if(show==0) lastwin=2;
        }

        public static int panelCnt=0;
        public static ComShPanel[] panel=new ComShPanel[20]; // 20個まで
        public static ComShPanel CreatePanel(ComShInterpreter sh,int x,int y,int w,int h,string t,int fsize){
            int i; for(i=0; i<panel.Length; i++) if(panel[i]==null) break;
            if(i==panel.Length) return null;
            var p=new ComShPanel(sh,x,y,w,h,t,fsize,ComShProperties.windowID+3+i);
            panel[i]=p;
            panelCnt++;
            return p;
        }
        public static void ClosePanel(int wid){
            int idx=wid-ComShProperties.windowID-3;
            panel[idx].OnClose();
            panel[idx]=null;
            panelCnt--;
        }
    }

	// ターミナル
	public class ComShTerminal {
        public ComShInteractive shell;
        public void Init(){
            shell=new ComShInteractive();
        }

        public RectOffset nomargin=new RectOffset(0,0,0,0);
		private Vector2 scroll_position = new Vector2(0, float.MaxValue);
		public Rect windowrect = new Rect(2, 2, 1, 1);
        private RectOffset wmargin=new RectOffset(4,4,2,6);
		private WindowStyle ws;
		private GUIStyle wstyle;
		private GUIStyle lstyle;

        private int hover=0;
        private long nextTick=0;
        public void OnVisibleChange(bool onoff){
            hover=0;
            if(!onoff){
                OnLeave();
                logId=0; cmdId=0;
            }
        }
        public void OnEnter(){
            if(UIInput.selection!=null) UIInput.selection.isSelected=false;
            var ia=GameObject.FindObjectsOfType<UIInput>();
            foreach(var ip in ia) ip.isSelected=false;
            UIInput.selection=null;
            UICamera.selectedObject=null;
            GUI.FocusWindow(ComShProperties.windowID);
        }
        public void OnLeave(){
            UnFocus();
            GUI.UnfocusWindow();
        }

        private bool windowmoveq=false;
		public void Draw() {
            Vector2 mp=Event.current.mousePosition;
            if(ComShWM.updateTime>nextTick){
                nextTick=ComShWM.updateTime+100*TimeSpan.TicksPerMillisecond;
                int k=GUIUtility.keyboardControl;
                if(!windowmoveq && (k==0||k==logId||k==cmdId) && mp.x>0 && mp.y>0){
                    UpdStyle();
                    int ph=hover;
                    hover=windowrect.Contains(mp)?1:2;
                    if(ph!=hover) enter=hover; else enter=0;
                    if(enter==1) OnEnter(); else if(enter==2) OnLeave();
                }
                windowmoveq=false;
            }

            float w=Screen.width,h=Screen.height;
            windowrect.x=Mathf.Clamp(windowrect.x,-windowrect.width+100,w-100);
            windowrect.y=Mathf.Clamp(windowrect.y,-windowrect.height+50,h-50);

            Rect wr=GUILayout.Window(ComShProperties.windowID, windowrect, Terminal,"",wstyle);
            if(wr!=windowrect) windowmoveq=true;
            windowrect=wr;
            if(hover==1) Input.ResetInputAxes();
        }
        private long styleDate=0;
        private void UpdStyle(){
            if(PanelStyleCache.updDate>styleDate){
                styleDate=PanelStyleCache.updDate;
                ws=PanelStyleCache.GetWindowStyles();
                lstyle=LblStyle(lstyle);
                wstyle=new GUIStyle(ws.window);
                wstyle.padding=wmargin;
                windowrect.width=ComShProperties.width;
                windowrect.height=ComShProperties.height;
                scroll_position.y=float.MaxValue;
                promptWidth=lstyle.CalcSize(new GUIContent(ComShProperties.prompt)).x;
            }
        }
        private GUIStyle LblStyle(GUIStyle style=null){
            if(style==null) style=new GUIStyle(GUI.skin.label);
            style.fontSize=ComShProperties.fontSize;
            style.border = nomargin;
            style.padding = nomargin;
            style.margin = nomargin;
            style.wordWrap=true;
            style.clipping=TextClipping.Clip;
            style.alignment=TextAnchor.LowerLeft;
            style.richText=false;
            PanelStyleCache.ChgTextColor(style);
            return style;
        }
        private void Terminal(int wid){
            if(ComShProperties.WakeupqGui()){ ComShWM.SetVisible(false); GUIUtility.ExitGUI(); }
            if(hover==1) TerminalEvent();
            TerminalRender();
        }

        private float promptWidth;
        private int logId=0,cmdId=0;
        private TextEditor logTe=new TextEditor();
        private TextEditor cmdTe=new TextEditor();
        private int enter=0;
        private const string CMD_NAME="/comsh/terminal/command";
        private const string LOG_NAME="/comsh/terminal/backlog";
        private void TerminalRender() {
            GUILayout.BeginHorizontal();
              GUILayout.Label("ComSh",ws.title,GUILayout.ExpandWidth(true));
              if(GUILayout.Button("☓",ws.closebtn)) ComShWM.HideTerm();
    		GUILayout.EndHorizontal();

    		GUILayout.BeginVertical();
			  scroll_position = GUILayout.BeginScrollView(scroll_position);
			    GUILayout.FlexibleSpace();
                GUI.SetNextControlName(LOG_NAME);
                GUILayout.TextArea(logTe.text,lstyle);
			    GUILayout.BeginHorizontal(lstyle);
                  GUILayout.Label(ComShProperties.prompt,lstyle,GUILayout.Width(promptWidth));
                  GUI.SetNextControlName(CMD_NAME);
                  cmdTe.text=GUILayout.TextField(cmdTe.text,lstyle,GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
              GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUI.DragWindow();

            if(Event.current.type==EventType.Repaint){
                if(logId==0 && hover==1){
                    GUI.FocusControl(LOG_NAME);
                    logId=GUIUtility.keyboardControl;
                    logTe=EditorCp(logTe,Editor(logId));
                    GUI.FocusControl(CMD_NAME);
                    cmdId=GUIUtility.keyboardControl;
                    cmdTe=EditorCp(cmdTe,Editor(cmdId));
                }
                if(enter==1){
                    if(cmdId!=0) CmdFocus();
                    enter=0;
                }
                if(tabdown){
                    Focus(cmdTe);
                    tabdown=false;
                }
            }
        }

        private long mouseDownTick=-1;
        private static readonly long NEVER=DateTime.MaxValue.Ticks+1;
        private bool tabdown;
        private void TerminalEvent() {
            if(logId==0) return;

            var e=Event.current;
            if(e.type==EventType.KeyDown){
                bool wrote=true;
                switch (e.keyCode){
                case KeyCode.UpArrow:
                    cmdTe.text=shell.HistoryBack(cmdTe.text);
                    cmdTe.MoveTextEnd();
                    e.Use();
                    break;
                case KeyCode.DownArrow:
                    cmdTe.text=shell.HistoryForward(cmdTe.text);
                    cmdTe.MoveTextEnd();
                    e.Use();
                    break;
                case KeyCode.LeftArrow:
                case KeyCode.RightArrow:
                    wrote=!e.shift;
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    int ret=shell.Interpret(cmdTe.text);
                    cmdTe.text="";
                    shell.HistoryRewind();
                    if(ret==1){ ComShWM.HideTerm(); GUIUtility.ExitGUI(); }
                    break;
                case KeyCode.V:
                    wrote=e.control;
                    break;
                case KeyCode.End:
                    if(!e.shift) cmdTe.MoveTextEnd(); else cmdTe.SelectTextEnd();
                    e.Use();
                    break;
                case KeyCode.Home:
                    if(!e.shift) cmdTe.MoveTextStart(); else cmdTe.SelectTextStart();
                    e.Use();
                    break;
                case KeyCode.Tab:
                    tabdown=true;   // どうやってもフォーカスが動くので、後で無理やりフォーカスを戻す
                    break;
                default:
                    wrote=false; break;
                }
                if(e.character!='\0') wrote=true;
                if(wrote){ CmdFocus(); scroll_position.y=float.MaxValue; }
            }else if(e.button==0 && e.isMouse){
                if(e.type==EventType.MouseDown){
                    HideCursor();                   // カーソル消して
                    logTe.SelectNone();
                    mouseDownTick=ComShWM.updateTime+100*TimeSpan.TicksPerMillisecond; //100msそのままなら戻す
                    if(e.clickCount>1) e.Use(); // 範囲選択がおかしな事になるのでダブル・トリプルクリックは止める
                }else if(e.type==EventType.MouseUp){
                    mouseDownTick=NEVER;
                    if(GUIUtility.hotControl==logId) GUIUtility.hotControl=0;
                    if(logTe.hasSelection){ logTe.Copy(); LogFocus(); }
                    else{ if(cmdTe.hasSelection) cmdTe.Copy(); CmdFocus(); }
                }else if(e.type==EventType.MouseDrag){ // Drag中に別部品上に行ってもhotControlは変わらない
                    mouseDownTick=NEVER;
                    if(GUIUtility.hotControl==logId) LogFocus();
                    else CmdFocus();
                }
            }else if(e.type==EventType.MouseDown&&e.button==1){
                if(logTe.hasSelection){
                    cmdTe.Paste();
                    logTe.SelectNone();
                    CmdFocus();
                    scroll_position.y=float.MaxValue;
                }else if(cmdTe.hasSelection){
                    cmdTe.SelectNone();
                    cmdTe.MoveTextEnd();
                    cmdTe.Paste();
                }
                e.Use();
            }
            if(ComShWM.updateTime>mouseDownTick){
                if(!logTe.hasSelection) CmdFocus(); else LogFocus();
                mouseDownTick=NEVER;
            }
		}
        private Color curColBk=Color.clear;
        private void HideCursor(){
            var c=GUI.skin.settings.cursorColor;
            if(c.a!=0){ curColBk=c; GUI.skin.settings.cursorColor=Color.clear; }
        }
        private void ShowCursor(){
            if(GUI.skin.settings.cursorColor.a==0) GUI.skin.settings.cursorColor=curColBk;
        }
        private void LogFocus(){
            cmdTe.SelectNone();
            Focus(logTe);
            HideCursor();
        }
        private void CmdFocus(){
            logTe.SelectNone();
            Focus(cmdTe);
            ShowCursor();
        }
        private void UnFocus(){
            if(logId!=0) logTe.SelectNone();
            GUIUtility.keyboardControl=0;
            ShowCursor();
        }
        private static TextEditor Editor(int id){ return (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor),id); }
        private static TextEditor EditorCp(TextEditor src,TextEditor dst){
            dst.text=src.text;
            dst.cursorIndex=src.cursorIndex;
            dst.selectIndex=src.selectIndex;
            return dst;
        }
        private static int Focus(TextEditor ed){
            // カーソル位置や選択範囲を変えないフォーカス処理
            // 普通にフォーカスするだけだと
            //  TextFiled: 全選択になる。カーソル位置も維持できない
            //  TextArea: 全選択にはならないが、カーソル位置を維持できない
            int old=GUIUtility.keyboardControl;
            if(old==ed.controlID) return old;
            int ci=ed.cursorIndex;
            int si=ed.selectIndex;
            GUIUtility.keyboardControl=ed.controlID;
            ed.OnFocus(); // こいつが全選択等余計な事をする。ここで呼んで内部状態更新しておかないと次のTextFiled()時に動いてしまう
            ed.cursorIndex=ci;
            ed.selectIndex=si;
            return old;
        }
        private static char[] logchar=new char[15000];
        public void AddLog(string add){ // addの最終行は改行なしで渡してね
            int l=add.Length;
            if(l>=15000){
                add.CopyTo(l-15000,logchar,0,15000);
                logTe.text=new string(logchar,0,15000);
            }else{
                var log=logTe.text;
                int len=15000-l-1;
                if(log.Length<len) len=log.Length;
                int start=log.Length-len;
                log.CopyTo(start,logchar,0,len);
                logchar[len]='\n';      // 初回に限り無駄にはなるけど表示上問題なし
                add.CopyTo(0,logchar,len+1,l);
                logTe.text=new string(logchar,0,len+1+l);
            }
        }
        public string GetLog(){ return logTe.text; }
        public void UpdateStyle(){ styleDate=0; }
	}

    // メニュー
    public class ComShMenu {
        public int wid=ComShProperties.windowID+1;
        public string title;
        public string[][] items;

        public void OnVisibleChange(bool v){
            if(baseRect.width==0) SetBaseRect(ComShWM.terminal.windowrect);
        }

        public void SetMenu(string t,string[][] i){
            title=t; items=i;
            styleDate=0;
        }
 
        private bool needUpdatePos=false;
        private Rect baseRect=Rect.zero;
        public void SetBaseRect(Rect r){
            baseRect=r;
            needUpdatePos=true;
        }

        private long styleDate=0;
        public void UpdateStyle(){ styleDate=0; }
        private WindowStyle ws;
		private GUIStyle wstyle;
    	private GUIStyle bstyle;
        public Rect windowrect = new Rect(0, 0, 120, 0);
        private RectOffset margin1=new RectOffset(1,1,1,1);
        private RectOffset margin2=new RectOffset(2,2,2,2);
		public void Draw() {
            if(PanelStyleCache.updDate>styleDate){
                styleDate=PanelStyleCache.updDate;
                ws=PanelStyleCache.GetWindowStyles();
                wstyle=new GUIStyle(ws.window);
                wstyle.padding=margin2;
                bstyle=new GUIStyle(GUI.skin.button);
			    bstyle.fontSize=ComShProperties.fontSize;
                bstyle.margin=margin1;
                float width=ComShProperties.fontSize*10,
                    wd=ws.title.CalcSize(new GUIContent(title)).x+ws.cbSize.x*2;
                if(wd>width) width=wd;
                for(int i=0; i<items.Length; i++){
                    wd=bstyle.CalcSize(new GUIContent(items[i][0])).x;
                    if(wd>width) width=wd;
                }
                float height=bstyle.CalcSize(new GUIContent("A")).y+bstyle.margin.top+bstyle.margin.bottom;
                windowrect.width=((width>ComShProperties.fontSize*50)?ComShProperties.fontSize*50:width)+36;
                windowrect.height=((items.Length>10)?10:items.Length)*height+ws.cbSize.y+wstyle.padding.top+wstyle.padding.bottom+4;
                if(needUpdatePos){
                    windowrect.x=baseRect.xMax+1;
                    if(windowrect.x>Screen.width-100) windowrect.x=baseRect.xMin-windowrect.width-1;
                    windowrect.y=baseRect.yMin;
                    if(windowrect.y<0) windowrect.y=0;
                    needUpdatePos=false;
                }
            } 

            float w=Screen.width,h=Screen.height;
            windowrect.x=Mathf.Clamp(windowrect.x,-windowrect.width+100,w-100);
            windowrect.y=Mathf.Clamp(windowrect.y,-windowrect.height+50,h-50); 

  			windowrect=GUILayout.Window(wid,windowrect,Menu,"",wstyle);
            if(windowrect.Contains(Event.current.mousePosition)) Input.ResetInputAxes();
        }
        private Vector2 scroll_position = new Vector2(0, 0);
        public void Menu(int wid){
    		GUILayout.BeginHorizontal();
            GUILayout.Label(title,ws.title,GUILayout.ExpandWidth(true));
            if(GUILayout.Button("Ｔ",ws.closebtn)) ComShWM.ToggleTerm();
            if(GUILayout.Button("☓",ws.closebtn)) ComShWM.HideMenu();
    		GUILayout.EndHorizontal();
			scroll_position = GUILayout.BeginScrollView(scroll_position);
            for(int i=0; i<items.Length; i++)
                if(GUILayout.Button(items[i][0],bstyle))
                    if(ComShWM.terminal.shell.InterpretBg(items[i][1])==1) ComShWM.HideMenu();
			GUILayout.EndScrollView();
            GUI.DragWindow();
		    if(ComShProperties.WakeupqGui()) ComShWM.Toggle();
        }
    }

	// 設定項目
	public static class ComShProperties {
		public static int windowID { get; private set; } = 173205071;   // 値はテキトー
        public static int fontSize =14;
        public static int width =512;
        public static int height =320;
		public static string prompt { get; private set; } = "$ ";
		//private static List<string> wakemodifier=new List<string>(new string[] { "ctrl","shift" });
		private static string wakekey = "4";
		private static int wakemodifier=3; // 4:alt 2:shift 1:ctrl

		// 今のところ_bashrc読込後にだけ呼ばれる。動的変更しなさそうだしね
		public static void Update(VarDic dic) {
			string s;

			// windowID。誰かがGUI.BringWindowToFront(windowID)みたいな事をやっていない限り、重複してても実害なし
			s= Validate(dic["_window_id"], @"^\d+$","173205071");
			windowID=int.Parse(s);      // \d+が保証されてるから例外は出ない

			// 起動キー(特殊キー)
			s = Validate(dic["_wake_modifier"].ToLower(),@"^(?:alt|shift|ctrl)(?:\+(?:alt|shift|ctrl))*$","ctrl+shift");
			string[] sa=s.Split('+');
            wakemodifier=0;
            for(int i=0; i<sa.Length; i++)
                if(sa[i]=="alt") wakemodifier+=4; else if(sa[i]=="shift") wakemodifier+=2; else if(sa[i]=="ctrl") wakemodifier+=1;

			// 起動キー(文字キー)
            wakekey=Validate(dic["_wake_char"].ToLower(),"^[a-z0-9]$","4");
            MkGUIWakeupEvent();

			// プロンプト
			prompt=Variables.Value(dic,"_prompt","$ ");
		}
        // 起動キー判定(Update()用)
		public static bool Wakeupq() {
			if(!Input.GetKey(wakekey)) return false;
            if((wakemodifier&4)>0 && !(Input.GetKey(KeyCode.LeftAlt)||Input.GetKey(KeyCode.RightAlt))) return false;
            if((wakemodifier&2)>0 && !(Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift))) return false;
            if((wakemodifier&1)>0 && !(Input.GetKey(KeyCode.LeftControl)||Input.GetKey(KeyCode.RightControl))) return false;
			return true;
		}

        private static Event guiWakeupEvent;
        public static void MkGUIWakeupEvent(){
            guiWakeupEvent=Event.KeyboardEvent(
                 ( (wakemodifier&4)>0?"&":"" )
                +( (wakemodifier&2)>0?"#":"" )
                +( (wakemodifier&1)>0?"^":"" )
                +wakekey
            );
        }
        // 起動キー判定(OnGUI()用)
        public static bool WakeupqGui(){
            if(!Event.current.Equals(guiWakeupEvent)) return false;
            Event.current.Use();
			return true;
        }
        // 正規表現によるバリデーション。違反時はデフォルト値を返す
	    private static string Validate(string str, string regexp, string dflt = "") {
		    return (Regex.IsMatch(str, regexp) ? str : dflt);
	    }
	}

    public class ComShHistory {
        private const int HISTORYSIZE=30;
		private string[] history = new string[HISTORYSIZE];
        private int top=-1;
        private int wIndex=-1;
        private int rIndex=-1;
        public void Add(string txt){
            wIndex=(wIndex+1)%HISTORYSIZE;
            if(top==-1) top=wIndex; else if(top==wIndex) top=(top+1)%HISTORYSIZE;
            history[wIndex]=txt;
        }
        public void Rewind(){
            rIndex=-1;
        }
        public string Back(){
            if(wIndex==-1) return "";
            if(rIndex==-1) rIndex=wIndex;
            else if(rIndex!=top) rIndex=(HISTORYSIZE+rIndex-1)%HISTORYSIZE;
            return history[rIndex];
        }
        public string Forward(){
            if(rIndex==-1) return "";
            if(rIndex!=wIndex) rIndex=(rIndex+1)%HISTORYSIZE;
            else{ rIndex=-1; return "";}
            return history[rIndex];
        }
    }

	// インタラクティブなインタプリタ
	public class ComShInteractive {
        private ComShHistory history=new ComShHistory();

		private ComShInterpreter cui;
        public enum OutputType { STDOUT,LOG,NONE };
        private OutputType output=OutputType.NONE;
		public ComShInteractive() {
            output=OutputType.NONE;
            cui = new ComShInterpreter(new ComShInterpreter.Output(this.Stdout));
            cui.interactiveq=true;
            ComShWM.paused=true;
			cui.SourceRc();
            ComShWM.paused=false;
		}
		public int Interpret(string line) {
            output=OutputType.STDOUT;
try{   
			Stdout(ComShProperties.prompt+line);
            int r=cui.Parse(line);
            if(r==0) return 0;                  // 空行はヒストリに入れない
			history.Add(line);
            history.Rewind();
            if(r<0) return cui.io.exitStatus;   // パース時点でエラー
            r=cui.InterpretParser();                  // パースできたコマンドを達を処理
            if(cui.exitq){ cui.exitq=false; return 1;} // ターミナルでexit→ターミナルを閉じる
}catch(Exception e){ Debug.Log(e.ToString()); return -1; }
            return cui.io.exitStatus;
		}
		public int InterpretBg(string line) {   // メニュー(select)からの呼び出し用
            output=OutputType.LOG;
try{   
			cui.Interpret(line);
            if(cui.exitq){ cui.exitq=false; return 1;}
}catch(Exception e){ Debug.Log(e.ToString()); return -1; }
            return cui.io.exitStatus;
        }
        public string HistoryBack(string dflt){
            string t=history.Back();
            return (t.Length>0)?t:dflt;
        }
        public string HistoryForward(string dflt){
            string t=history.Forward();
            return (t.Length>0)?t:dflt;
        }
        public void HistoryRewind(){ history.Rewind(); }
        public void Stdout(string msg,int code=0){ StdOut(msg,output); }
        public static void StdOut(string msg,OutputType output){
            if(output==OutputType.STDOUT) ComShWM.terminal.AddLog(msg);
            else if(output==OutputType.LOG) Debug.Log(msg.TrimEnd(ParseUtil.crlf));
        }
	}
}
