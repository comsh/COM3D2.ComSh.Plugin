using System;
using System.Collections.Generic;
using static COM3D2.ComSh.Plugin.Command;

namespace COM3D2.ComSh.Plugin {
public static class CmdGui {
    public static void Init(){
        Command.AddCmd("panel",new Cmd(CmdPanel));
        Command.AddCmd("grid",new Cmd(CmdGrid));
        Command.AddCmd("label",new Cmd(CmdLabel));
        Command.AddCmd("button",new Cmd(CmdButton));
        Command.AddCmd("rbutton",new Cmd(CmdRbutton));
        Command.AddCmd("toggle",new Cmd(CmdToggle));
        Command.AddCmd("text",new Cmd(CmdText));
        Command.AddCmd("slider",new Cmd(CmdSlider));
        Command.AddCmd("vslider",new Cmd(CmdVSlider));
        Command.AddCmd("combo",new Cmd(CmdCombo));
        Command.AddCmd("combo2",new Cmd(CmdCombo2));
        Command.AddCmd("panel.onclose",new Cmd(CmdOnClose));
        Command.AddCmd("panel.close",new Cmd(CmdClose));
        Command.AddCmd("panel.update",new Cmd(CmdUpdate));
    }

    private static float[] XYWHf(List<string> args,int start){
        float[] ret=new float[4];
        for(int i=0; i<4; i++) if(!float.TryParse(args[start+i],out ret[i])||ret[i]<0) return null;
        if(ret[2]==0||ret[3]==0) return null;
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
        }else return sh.io.Error("使い方: panel タイトル x y 幅 高さ [フォントサイズ]");
        return 0;
    }
    private static int CmdGrid(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        float[] xywh;
        if(args.Count==5 && (xywh=XYWHf(args,1))!=null){
            sh.panel.SetGrid(xywh);
        }else return sh.io.Error("使い方: grid x y 幅 高さ");
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
    private static int CmdButton(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count==7 && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.lastParser.lineno);
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
        if(args.Count==7 && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.lastParser.lineno);
            int r=psr.Parse(args[6]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) return sh.io.Error("コマンドが空です");
            sh.panel.AddRbutton(xywh[0],xywh[1],xywh[2],xywh[3],args[5],psr);
        }else return sh.io.Error("使い方: rbutton x y 幅 高さ ラベル コマンド");
        return 0;
    }
    private static int CmdToggle(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count==9 && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.lastParser.lineno);
            int r=psr.Parse(args[6]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            if(!float.TryParse(args[8],out float f)||(f!=0&&f!=1)) sh.io.Error("数値の形式が不正です");
            sh.panel.AddToggle(xywh[0],xywh[1],xywh[2],xywh[3],args[5],psr,args[7],f==1);
        }else return sh.io.Error("使い方: toggle x y 幅 高さ ラベル コマンド 変数名 初期値");
        return 0;
    }
    private static int CmdText(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count==8 && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.lastParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            sh.panel.AddTextField(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7]);
        }else return sh.io.Error("使い方: text x y 幅 高さ コマンド 変数名 初期値");
        return 0;
    }
    private static int CmdCombo(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count>=8 && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.lastParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            if(args.Count==9 && args[8].IndexOf('\n')>=0){ // １行１選択肢のタイプ
                sh.panel.AddCombo(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7],args[8].Split(ParseUtil.crlf));
            }else{
                int i; for(i=8; i<args.Count; i++) if(args[i]==args[7]) break;
                var items=(i==args.Count)?args.GetRange(7,args.Count-7):args.GetRange(8,args.Count-8);
                sh.panel.AddCombo(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7],items.ToArray());
            }
        }else return sh.io.Error("使い方: combo x y 幅 高さ コマンド 変数名 初期値 選択肢1 ...");
        return 0;
    }
    private static int CmdCombo2(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        if(args.Count==9 && (xywh=XYWH(sh.panel,args,1))!=null){
            var psr=new ComShParser(sh.lastParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;

            var lstpsr=new ComShParser(sh.lastParser.lineno);
            r=lstpsr.Parse(args[8]);
            if(r<0) return sh.io.Error(lstpsr.error);

            sh.panel.AddCombo2(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],args[7],lstpsr);
        }else return sh.io.Error("使い方: combo2 x y 幅 高さ コマンド 変数名 初期値 選択肢生成用コマンド");
        return 0;
    }
    private static int CmdSlider(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        float v,min,max;
        if(args.Count==10 && (xywh=XYWH(sh.panel,args,1))!=null
            &&float.TryParse(args[7],out v)&&float.TryParse(args[8],out min)&&float.TryParse(args[9],out max)
            &&max>min){
            if(v<min) v=min; else if(v>max) v=max;
            var psr=new ComShParser(sh.lastParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            sh.panel.AddSlider(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],v,min,max);
        }else return sh.io.Error("使い方: slider x y 幅 高さ コマンド 変数名 初期値 最小値 最大値");
        return 0;
    }
    private static int CmdVSlider(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        int[] xywh;
        float v,min,max;
        if(args.Count==10 && (xywh=XYWH(sh.panel,args,1))!=null
            &&float.TryParse(args[7],out v)&&float.TryParse(args[8],out min)&&float.TryParse(args[9],out max)
            &&max>min){
            if(v<min) v=min; else if(v>max) v=max;
            var psr=new ComShParser(sh.lastParser.lineno);
            int r=psr.Parse(args[5]);
            if(r<0) return sh.io.Error(psr.error);
            if(r==0) psr=null;
            sh.panel.AddVSlider(xywh[0],xywh[1],xywh[2],xywh[3],psr,args[6],v,min,max);
        }else return sh.io.Error("使い方: vslider x y 幅 高さ コマンド 変数名 初期値 最小値 最大値");
        return 0;
    }
    private static int CmdOnClose(ComShInterpreter sh,List<string> args){
        if(sh.panel==null) return sh.io.Error("panelコマンドでパネルウィンドウを定義してください");
        if(args.Count==2){
            var psr=new ComShParser(sh.lastParser.lineno);
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
        var psr=new ComShParser(sh.lastParser.lineno);
        int r=psr.Parse(args[1]);
        if(r<0) return sh.io.Error(psr.error);
        if(r==0) return sh.io.Error("コマンドが空です");
        long stime=DateTime.UtcNow.Ticks;
        ComShBg.cron.AddJob("panel/update"+UTIL.GetSeqId(),(long)(ms*TimeSpan.TicksPerMillisecond),0,(long t)=>{
            if(sh.panel==null) return -1;
            sh.env["_1"]=((t-stime)/TimeSpan.TicksPerMillisecond).ToString();
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
}
}
