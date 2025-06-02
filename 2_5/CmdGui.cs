using System;
using System.Collections.Generic;
using static COM3D2.ComSh.Plugin.Command;

namespace COM3D2.ComSh.Plugin {
public static class CmdGui {
    public static void Init(){
        Command.AddCmd("panel",new Cmd(CmdPanel));
        Command.AddCmd("grid",new Cmd(CmdGrid));
        Command.AddCmd("label",new Cmd(CmdLabel));
        Command.AddCmd("label2",new Cmd(CmdLabel2));
        Command.AddCmd("button",new Cmd(CmdButton));
        Command.AddCmd("rbutton",new Cmd(CmdRbutton));
        Command.AddCmd("toggle",new Cmd(CmdToggle));
        Command.AddCmd("radio",new Cmd(CmdRadio));
        Command.AddCmd("text",new Cmd(CmdText));
        Command.AddCmd("slider",new Cmd(CmdSlider));
        Command.AddCmd("vslider",new Cmd(CmdVSlider));
        Command.AddCmd("combo",new Cmd(CmdCombo));
        Command.AddCmd("combo2",new Cmd(CmdCombo2));
        Command.AddCmd("listmenu",new Cmd(CmdListMenu));
        Command.AddCmd("listmenu2",new Cmd(CmdListMenu2));
        Command.AddCmd("listbox",new Cmd(CmdListBox));
        Command.AddCmd("listbox2",new Cmd(CmdListBox2));
        Command.AddCmd("swbutton",new Cmd(CmdSwitch));
        Command.AddCmd("swbutton2",new Cmd(CmdSwitch2));
        Command.AddCmd("panel.onclose",new Cmd(CmdOnClose));
        Command.AddCmd("panel.close",new Cmd(CmdClose));
        Command.AddCmd("panel.update",new Cmd(CmdUpdate));
        Command.AddCmd("panel.richtext",new Cmd(CmdRichText));
        Command.AddCmd("page",new Cmd(CmdPage));
        Command.AddCmd("pagedef",new Cmd(CmdPageDef));
        Command.AddCmd("pagelist",new Cmd(CmdPageList));
    }

    private static float[] currentgridxywh={0,0,0,0};
    private static float[] currentxywh={0,0,0,0};
    private static float[] XYWHf(List<string> args,int start,bool gridq=false){
        float[] current=gridq?currentgridxywh:currentxywh;
        float[] ret=new float[4];
        for(int i=0; i<4; i++){
            string nstr=args[start+i];
            if(nstr=="") return null;
            if(nstr=="="){
                ret[i]=current[i];
            }else if(nstr=="." && !gridq){
                if(i<2) ret[i]=current[i]+current[i+2]; else ret[i]=current[i];
            }else if(nstr[0]=='+'){
                float f;
                if(nstr.Length==1) f=1; else if(!float.TryParse(nstr.Substring(1),out f)) return null;
                ret[i]=current[i]+f;
            }else if(nstr[0]=='-'){
                float f;
                if(nstr.Length==1) f=1; else if(!float.TryParse(nstr.Substring(1),out f)) return null;
                ret[i]=current[i]-f;
            }else if(!float.TryParse(nstr,out ret[i])||ret[i]<0) return null;
        }
        if(ret[2]==0||ret[3]==0) return null;
        for(int i=0; i<4; i++) current[i]=ret[i];
        if(gridq) for(int i=0; i<4; i++) currentxywh[i]=0;
        return ret;
    }
    private static int[] XYWH(ComShPanel panel,List<string> args,int start){
        float[] ret=XYWHf(args,start);
        if(ret==null) return null;
        return panel.Plot(ret);
    }
    private static int CmdPanel(ComShInterpreter sh,List<string> args){
        float[] xywh;
        int fs=14;
        if((args.Count==6||(args.Count==7&&int.TryParse(args[6],out fs)&&fs>0))&&(xywh=XYWHf(args,2))!=null){
            if(sh.panel!=null) ComShWM.ClosePanel(sh.panel.wid);
            sh.panel=ComShWM.CreatePanel(sh,(int)xywh[0],(int)xywh[1],(int)xywh[2],(int)xywh[3],args[1],fs);
            if(sh.panel==null) return sh.io.Error("これ以上パネルを作成できません");
            if(!ComShWM.IsVisible()){ ComShWM.SetVisible(true); ComShWM.HideTerm();}
        }else return sh.io.Error("使い方: panel タイトル x y 幅 高さ [フォントサイズ]");
        for(int i=0; i<4; i++){currentgridxywh[i]=0;currentxywh[i]=0;}
        return 0;
    }
    private static int CmdGrid(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        float[] xywh;
        if(args.Count==5 && (xywh=XYWHf(args,1,true))!=null){
            sh.panel.SetGrid(xywh);
        }else return sh.io.Error("使い方: grid x y 幅 高さ");
        return 0;
    }
    private static int CmdPageDef(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        if(args.Count==2) sh.panel.SetTab(args[1]); else return sh.io.Error("使い方: pagedef ページ名");
        return 0;
    }
    private static int CmdPageList(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        foreach(string name in sh.panel.tabnames) sh.io.PrintLn(name);
        return 0;
    }
    private static int CmdPage(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count==7 && (xywh=XYWH(sh.panel,args,1))!=null){
            sh.panel.AddPage(xywh[0],xywh[1],xywh[2],xywh[3],args[5],args[6]);
        }else return sh.io.Error("使い方: page x y 幅 高さ 変数名 初期値");
        return 0;
    }
    private static int CmdLabel(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count==6 && (xywh=XYWH(sh.panel,args,1))!=null){
            sh.panel.AddLabel(xywh[0],xywh[1],xywh[2],xywh[3],args[5]);
        }else return sh.io.Error("使い方: label x y 幅 高さ ラベル");
        return 0;
    }
    private static int CmdLabel2(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count==6 && (xywh=XYWH(sh.panel,args,1))!=null){
            sh.panel.AddLabel2(xywh[0],xywh[1],xywh[2],xywh[3],args[5]);
        }else return sh.io.Error("使い方: label2 x y 幅 高さ 変数名");
        return 0;
    }
    private static int CmdButton(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count==7 && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[6]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) return sh.io.Error("コマンドが空です");
            sh.panel.AddButton(xywh[0],xywh[1],xywh[2],xywh[3],args[5],psr);
        }else return sh.io.Error("使い方: button x y 幅 高さ ラベル コマンド");
        return 0;
    }
    private static int CmdRbutton(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count>=7 && args.Count<=10 && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[6]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) return sh.io.Error("コマンドが空です");
            float dt=0,ddt=0,mindt=0;
            if(args.Count>=8 && (!float.TryParse(args[7],out dt)||dt<0)) sh.io.Error("数値の形式が不正です");
            if(args.Count>=9 && (!float.TryParse(args[8],out ddt)||ddt<0)) sh.io.Error("数値の形式が不正です");
            if(args.Count==10 && (!float.TryParse(args[9],out mindt)||mindt<0)) sh.io.Error("数値の形式が不正です");
            sh.panel.AddRbutton(xywh[0],xywh[1],xywh[2],xywh[3],args[5],psr,dt,ddt,mindt);
        }else return sh.io.Error("使い方: rbutton x y 幅 高さ ラベル コマンド [初期間隔 間隔減少 最小間隔]");
        return 0;
    }
    private static int CmdToggle(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count==9 && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[6]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            if(!float.TryParse(args[8],out float f)||(f!=0&&f!=1)) sh.io.Error("数値の形式が不正です");
            sh.panel.AddToggle(xywh[0],xywh[1],xywh[2],xywh[3],args[5],psr,args[7],f==1);
        }else return sh.io.Error("使い方: toggle x y 幅 高さ ラベル コマンド 変数名 初期値");
        return 0;
    }
    private static int CmdRadio(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count==9 && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[6]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            sh.panel.AddRadio(xywh[0],xywh[1],xywh[2],xywh[3],args[5],psr,args[7],args[8]);
        }else return sh.io.Error("使い方: radio x y 幅 高さ ラベル コマンド 変数名 値");
        return 0;
    }
    private static int CmdText(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if((args.Count==8||args.Count==9) && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            float delay=0;
            if(args.Count==9 && (!float.TryParse(args[8],out delay)||delay<0)) return sh.io.Error("数値の指定が不正です");
            sh.panel.AddTextField(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7],delay);
        }else return sh.io.Error("使い方: text x y 幅 高さ コマンド 変数名 初期値");
        return 0;
    }
    private static int CmdCombo(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count>=8 && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            if((args.Count==9 ||args.Count==10) && args[8].IndexOf('\n')>=0){ // １行１選択肢のタイプ
                string[] items=ParseUtil.Chomp(args[8]).Split(ParseUtil.lf);
                char c='\0';
                if(args.Count==10){
                    if(args[9]==""||args[9].Length!=1) return sh.io.Error("区切り文字の指定が不正です");
                    c=args[9][0];
                }
                sh.panel.AddCombo(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7],items,c);
            }else{
                int i; for(i=8; i<args.Count; i++) if(args[i]==args[7]) break;
                var items=(i==args.Count)?args.GetRange(7,args.Count-7):args.GetRange(8,args.Count-8);
                sh.panel.AddCombo(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7],items.ToArray(),'\0');
            }
        }else return sh.io.Error("使い方 : combo x y 幅 高さ コマンド 変数名 初期値 選択肢1 ...\n使い方2: combo x y 幅 高さ コマンド 変数名 初期値 選択肢(1行1項目) [区切り文字]");
        return 0;
    }
    private static int CmdCombo2(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if((args.Count==9 ||args.Count==10)&& (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            var lstpsr=new ComShParser(sh.currentParser.lineno);
            r=lstpsr.Parse(args[8]);
            if(r<0) return sh.io.Error(lstpsr.error);
            char c='\0';
            if(args.Count==10){
                if(args[9]==""||args[9].Length!=1) return sh.io.Error("区切り文字の指定が不正です");
                c=args[9][0];
            }
            sh.panel.AddCombo2(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7],lstpsr,c);
        }else return sh.io.Error("使い方: combo2 x y 幅 高さ コマンド 変数名 初期値 選択肢生成用コマンド [区切り文字]");
        return 0;
    }
    private static int CmdListMenu(ComShInterpreter sh,List<string> args){ return CmdListBoxSub(sh,args,1); }
    private static int CmdListMenu2(ComShInterpreter sh,List<string> args){ return CmdListBox2Sub(sh,args,1); }
    private static int CmdListBox(ComShInterpreter sh,List<string> args){ return CmdListBoxSub(sh,args,0); }
    private static int CmdListBox2(ComShInterpreter sh,List<string> args){ return CmdListBox2Sub(sh,args,0); }
    private static int CmdListBoxSub(ComShInterpreter sh,List<string> args,int single){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count>=8 && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            if((args.Count==9 ||args.Count==10) && args[8].IndexOf('\n')>=0){ // １行１選択肢のタイプ
                string[] items=ParseUtil.Chomp(args[8]).Split(ParseUtil.lf);
                char c='\0';
                if(args.Count==10){
                    if(args[9]==""||args[9].Length!=1) return sh.io.Error("区切り文字の指定が不正です");
                    c=args[9][0];
                }
                sh.panel.AddListBox(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7],items,single,c);
            }else{
                int i; for(i=8; i<args.Count; i++) if(args[i]==args[7]) break;
                var items=(i==args.Count)?args.GetRange(7,args.Count-7):args.GetRange(8,args.Count-8);
                sh.panel.AddListBox(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7],items.ToArray(),single,'\0');
            }
        }else return sh.io.Error($"使い方 : {args[0]} x y 幅 高さ コマンド 変数名 初期値 選択肢1 ...\n使い方2: {args[0]} x y 幅 高さ コマンド 変数名 初期値 選択肢(1行1項目) [区切り文字]");
        return 0;
    }
    private static int CmdListBox2Sub(ComShInterpreter sh,List<string> args,int single){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if((args.Count==9 ||args.Count==10)&& (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            var lstvar=args[8];
            if(!ParseUtil.IsVarName(lstvar)) return sh.io.Error("選択肢変数名が不正です");
            char c='\0';
            if(args.Count==10){
                if(args[9]==""||args[9].Length!=1) return sh.io.Error("区切り文字の指定が不正です");
                c=args[9][0];
            }
            sh.panel.AddListBox2(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7],lstvar,single,c);
        }else return sh.io.Error($"使い方: {args[0]} x y 幅 高さ コマンド 変数名 初期値 選択肢変数 [区切り文字]");
        return 0;
    }
    private static int CmdSwitch(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count>=8 && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            if((args.Count==9 ||args.Count==10) && args[8].IndexOf('\n')>=0){ // １行１選択肢のタイプ
                string[] items=ParseUtil.Chomp(args[8]).Split(ParseUtil.lf);
                char c='\0';
                if(args.Count==10){
                    if(args[9]==""||args[9].Length!=1) return sh.io.Error("区切り文字の指定が不正です");
                    c=args[9][0];
                }
                sh.panel.AddSwitch(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7],items,c);
            }else{
                int i; for(i=8; i<args.Count; i++) if(args[i]==args[7]) break;
                var items=(i==args.Count)?args.GetRange(7,args.Count-7):args.GetRange(8,args.Count-8);
                sh.panel.AddSwitch(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7],items.ToArray(),'\0');
            }
        }else return sh.io.Error("使い方 : swbutton x y 幅 高さ コマンド 変数名 初期値 選択肢1 ...\n使い方2: combo x y 幅 高さ コマンド 変数名 初期値 選択肢(1行1項目) [区切り文字]");
        return 0;
    }
    private static int CmdSwitch2(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if((args.Count==9 ||args.Count==10)&& (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            var lstvar=args[8];
            if(!ParseUtil.IsVarName(lstvar)) return sh.io.Error("選択肢変数名が不正です");
            char c='\0';
            if(args.Count==10){
                if(args[9]==""||args[9].Length!=1) return sh.io.Error("区切り文字の指定が不正です");
                c=args[9][0];
            }
            sh.panel.AddSwitch2(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7],lstvar,c);
        }else return sh.io.Error("使い方: swbutton2 x y 幅 高さ コマンド 変数名 初期値 選択肢生成用コマンド [区切り文字]");
        return 0;
    }

    private static int CmdSlider(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        float v,min,max;
        if((args.Count==10 || args.Count==11) && (xywh=XYWH(sh.panel,args,1))!=null
            &&float.TryParse(args[7],out v)&&float.TryParse(args[8],out min)&&float.TryParse(args[9],out max)
            &&max>min){
            if(v<min) v=min; else if(v>max) v=max;
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            float delay=0;
            if(args.Count==11 && (!float.TryParse(args[10],out delay)||delay<0)) return sh.io.Error("数値の指定が不正です");
            sh.panel.AddSlider(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],v,min,max,delay);
        }else return sh.io.Error("使い方: slider x y 幅 高さ コマンド 変数名 初期値 最小値 最大値");
        return 0;
    }
    private static int CmdVSlider(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        float v,min,max;
        if((args.Count==10 || args.Count==11)&&(xywh=XYWH(sh.panel,args,1))!=null
            &&float.TryParse(args[7],out v)&&float.TryParse(args[8],out min)&&float.TryParse(args[9],out max)
            &&max>min){
            if(v<min) v=min; else if(v>max) v=max;
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            float delay=0;
            if(args.Count==11 && (!float.TryParse(args[10],out delay)||delay<0)) return sh.io.Error("数値の指定が不正です");
            sh.panel.AddVSlider(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],v,min,max,delay);
        }else return sh.io.Error("使い方: vslider x y 幅 高さ コマンド 変数名 初期値 最小値 最大値");
        return 0;
    }
    private static int CmdOnClose(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        if(args.Count==2){
            var psr=new ComShParser(sh.currentParser.lineno);
            int r=psr.Parse(args[1]);
            if(r<=0) return sh.io.Error(psr.error);
            sh.panel.SetOnClose(psr);
        } else return sh.io.Error("使い方: panel.onclose コマンド");
        return 0;
    }
    private static int CmdClose(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        ComShWM.ClosePanel(sh.panel.wid);
        return 0;
    }
    private static int CmdUpdate(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        if(args.Count<2||args.Count>3) return sh.io.Error("使い方: panel.update コマンド [実行間隔(ms)]");
        float ms=0;
        if(args.Count==3 && (!float.TryParse(args[2],out ms) || ms<0)) return sh.io.Error("実行間隔の値が不正です");
        var psr=EvalParser(sh,1);
        if(psr==null) return -1;
        long stime=DateTime.UtcNow.Ticks,lasttime=0;
        ComShBg.cron.AddJob("panel/update"+UTIL.GetSeqId(),(long)(ms*TimeSpan.TicksPerMillisecond),0,(long t)=>{
            if(sh.panel==null) return -1;
            long cur=(t-stime)/TimeSpan.TicksPerMillisecond;
            sh.env["_1"]=cur.ToString();
            sh.env["_2"]=(cur-lasttime).ToString();
            lasttime=cur;
            psr.Reset();
            string ee=sh.env[ComShInterpreter.SCRIPT_ERR_ON];
            sh.env[ComShInterpreter.SCRIPT_ERR_ON]="1";
            int ret=sh.InterpretParser(psr);
            sh.env[ComShInterpreter.SCRIPT_ERR_ON]=ee;
            if(sh.exitq) { sh.exitq=false; ret=sh.io.exitStatus; }
            return ret;
        });
        return 0;
    }
    private static int CmdRichText(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        const string usage="使い方: panel.richtext {on|off}";
        if(args.Count==1){
            sh.io.Print(sh.panel.allowRichText?"on":"off");
        }else if(args.Count==2){
            int sw=ParseUtil.OnOff(args[1]);
            if(sw<0) return sh.io.Error(usage);
            sh.panel.allowRichText=(sw==1);
        }else return sh.io.Error(usage);
        return 0;
    }
}
}
