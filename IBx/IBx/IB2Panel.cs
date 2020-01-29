﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBx
{
    public class IB2Panel
    {
        //this class is handled differently than Android version
        [JsonIgnore]
        public GameView gv;
        public string tag = "";
        public string backgroundImageFilename = "";
        public bool hiding = false;
        public bool showing = false;
        public int shownLocX = 0;
        public int shownLocY = 0;
        public int currentLocX = 0;
        public int currentLocY = 0;
        public int hiddenLocX = 0;
        public int hiddenLocY = 0;
        public int hidingXIncrement = 0;
        public int hidingYIncrement = 0;
        public int Width = 0;
        public int Height = 0;
        public List<IB2Button> buttonList = new List<IB2Button>();
        public List<IB2ToggleButton> toggleList = new List<IB2ToggleButton>();
        public List<IB2Portrait> portraitList = new List<IB2Portrait>();
        public List<IB2HtmlLogBox> logList = new List<IB2HtmlLogBox>();

        public IB2Panel()
        {

        }

        public IB2Panel(GameView g)
        {
            gv = g;
            currentLocX = shownLocX;
            currentLocY = shownLocY;
        }

        public void setupIB2Panel(GameView g)
        {
            gv = g;
            /*if (this.tag.StartsWith("arrow"))
            {
                shownLocY = 910;
                hiddenLocY = 1080;
            }*/
            currentLocX = shownLocX;
            currentLocY = shownLocY;
            
            //try to do aspect ratio adjustemnets here
            //to do
            //default is 16:9 (=1,77777777...)
            //16:10 would be 1.6, ie increase y coordinate (move panels down)
            //4:3 would be 1.3333333333,ie increase y coordinate (move panels down)

            float yAdjustmentFactor = 0;
            float width = gv.screenWidth;
            float height = gv.screenHeight;
            float currentAspectRatio = width / height;
            yAdjustmentFactor = (1920f / 1080f) / currentAspectRatio;

            currentLocY = (int)(currentLocY * yAdjustmentFactor);
            shownLocY = (int)(shownLocY * yAdjustmentFactor);
            hiddenLocY = (int)(hiddenLocY * yAdjustmentFactor);



            //shownLocY = (int)(shownLocY * yAdjustmentFactor);

            foreach (IB2Button btn in buttonList)
            {
                btn.setupIB2Button(gv);
            }
            foreach (IB2ToggleButton btn in toggleList)
            {
                btn.setupIB2ToggleButton(gv);
            }
            foreach (IB2Portrait btn in portraitList)
            {
                btn.setupIB2Portrait(gv);
            }
            foreach (IB2HtmlLogBox log in logList)
            {
                log.setupIB2HtmlLogBox(gv);
            }
        }

        public void setHover(int x, int y)
        {
            //iterate over all controls and set glow on/off
            foreach (IB2Button btn in buttonList)
            {
                btn.setHover(this, x, y);
            }
            foreach (IB2Portrait btn in portraitList)
            {
                btn.setHover(this, x, y);
            }
        }

        public string getImpact(int x, int y)
        {
            //iterate over all controls and get impact
            foreach (IB2Button btn in buttonList)
            {
                if (btn.getImpact(this, x, y))
                {
                    return btn.tag;
                }
            }
            foreach (IB2ToggleButton btn in toggleList)
            {
                if (btn.getImpact(this, x, y))
                {
                    return btn.tag;
                }
            }
            foreach (IB2Portrait btn in portraitList)
            {
                if (btn.getImpact(this, x, y))
                {
                    return btn.tag;
                }
            }
            foreach (IB2HtmlLogBox log in logList)
            {
                //log.onDrawLogBox();
            }
            return "";
        }

        public void Draw()
        {
            if (!gv.mod.useMinimalisticUI)
            {
                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).Width, gv.cc.GetFromBitmapList(backgroundImageFilename).Height);
                IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));

                if ((this.tag != "InitiativePanel") && (this.tag != "logPanel"))
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
                }

                if ((this.tag.Contains("logPanel")) && (gv.screenCombat.showIniBar) && (gv.screenType.Equals("combat")))
                {
                    dst = new IbRect((int)(currentLocX * gv.screenDensity) + gv.pS, (int)(currentLocY * gv.screenDensity) + gv.squareSize + 4 * gv.pS, (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity - gv.squareSize - 4 * gv.pS));
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
                }
                else if (this.tag.Contains("logPanel") && (gv.screenType.Equals("combat")))
                {
                    dst = new IbRect((int)(currentLocX * gv.screenDensity) + gv.pS, (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
                }
                else if (this.tag.Contains("logPanel"))
                {
                    dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
                }
                

            }
            //IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
            //IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity), (int)(Width * gv.screenDensity), (int)(Height * gv.screenDensity));
            //gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);

            //iterate over all controls and draw
            //backwerk
            //currentLocX = hiddenLocX;
            bool stopDrawing = false;

            if (this.hidingXIncrement != 0 && this.currentLocX == this.hiddenLocX)
            {
                stopDrawing = true;
            }

            if (this.hidingYIncrement != 0 && this.currentLocY == this.hiddenLocY)
            {
                stopDrawing = true;
            }

            if (!stopDrawing)
            {
                foreach (IB2Button btn in buttonList)
                {
                    //if ((btn.X > -gv.squareSize && btn.Y > -gv.squareSize) || (btn.X < gv.screenWidth + gv.squareSize && btn.Y < gv.screenHeight + gv.squareSize))
                    //{
                    if (!gv.mod.currentArea.isOverviewMap)
                    {
                        btn.Draw(this);
                    }
                    else
                    {
                        if (btn.tag == "btnOwnZoneMap" || btn.tag == "btnMotherZoneMap" || btn.tag == "btnGrandMotherZoneMap")
                        {
                            btn.Draw(this);
                        }
                    }
                    //}
                }
                foreach (IB2ToggleButton btn in toggleList)
                {
                    //if ((btn.X > -gv.squareSize && btn.Y > -gv.squareSize) || (btn.X < gv.screenWidth + gv.squareSize && btn.Y < gv.screenHeight + gv.squareSize))
                    //{
                    if (!gv.mod.currentArea.isOverviewMap)
                    {
                        btn.Draw(this);
                    }
                    //}
                }
                foreach (IB2Portrait btn in portraitList)
                {
                    //if ((btn.X > -gv.squareSize && btn.Y > -gv.squareSize) || (btn.X < gv.screenWidth + gv.squareSize && btn.Y < gv.screenHeight + gv.squareSize))
                    //{
                    if (!gv.mod.currentArea.isOverviewMap)
                    {
                        btn.Draw(this);
                    }
                    //}
                }
            }

            if (!gv.mod.useMinimalisticUI)
            {
                foreach (IB2HtmlLogBox log in logList)
                {
                    log.onDrawLogBox(this);
                }
            }

            //gv.DrawText("(" + this.currentLocX * gv.screenDensity + "," + this.currentLocY * gv.screenDensity + ")", currentLocX * gv.screenDensity, currentLocY * gv.screenDensity);
        }

        public void DrawLogBackground()
        {
            //if (gv.screenType.Equals("main") )
            //{
            //IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
            //IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity), (int)(currentLocY * gv.screenDensity - 3 * gv.pS), (int)(Width * gv.screenDensity + 2 * gv.pS), (int)(Height * gv.screenDensity - gv.squareSize + 7 * gv.pS));
            //gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.8f * gv.mod.logOpacity);
            //}
            //else
            //{
            if (gv.mod.logOpacity > 0)
            {
                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).Width, gv.cc.GetFromBitmapList(backgroundImageFilename).Height);
                IbRect dst = new IbRect((int)(currentLocX * gv.screenDensity + gv.oXshift - 2.5 * gv.pS), (int)(currentLocY * gv.screenDensity) - 3 * gv.pS, (int)(Width * gv.screenDensity + 6.5 * gv.pS), (int)(Height * gv.screenDensity - 1 * gv.squareSize + 12 * gv.pS - 0 * gv.pS));

                if (gv.mod.useComplexCoordinateSystem && gv.screenType != "combat")
                {
                    dst = new IbRect((int)(currentLocX * gv.screenDensity + gv.oXshift - 2.5 * gv.pS), (int)(currentLocY * gv.screenDensity) + 5 * gv.pS, (int)(Width * gv.screenDensity + 6.5 * gv.pS), (int)(Height * gv.screenDensity - 1 * gv.squareSize + 12 * gv.pS - 2 * gv.pS));
                }
                if (gv.cc.floatyTextActorInfoName != "")
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 1f * gv.mod.logOpacity, true);
                }
                else
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.575f * gv.mod.logOpacity, true);
                }
                //}
                int txtH = (int)gv.drawFontRegHeight;
                gv.cc.floatyTextLocInfo.X = gv.squareSize / 2;
                gv.cc.floatyTextLocInfo.Y = gv.squareSize * 1;
                bool isPlayer = false;
                foreach (Player p in gv.mod.playerList)
                {
                    if (p.name == gv.cc.floatyTextActorInfoName)
                    {
                        isPlayer = true;
                        break;
                    }
                }
                if (gv.cc.drawInfoText)
                {
                    if (isPlayer)
                    {
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoName, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 3 * txtH, 0.9f, "lime");
                        if (!gv.cc.inEffectMode)
                        {
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAC, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 5 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoMoveOrder, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 5 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoInitiative, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 5 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoHP, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 7 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSP, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 7 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoNumberOfAttacks, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoToHit, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAmmo, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAttackType, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 10 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAttackRange, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 10 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamage, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 12 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnScoringHitSpellName, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 13 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSaves, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 15 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSaves2, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 15 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSaves3, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 15 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances1, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 17 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances2, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 17 * txtH, 0.9f, "yellow");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances3, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 17 * txtH, 0.9f, "lime");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances4, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 18 * txtH, 0.9f, "orange");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances5, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 18 * txtH, 0.9f, "blue");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances6, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 19 * txtH, 0.9f, "purple");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances7, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 19 * txtH, 0.9f, "blue");

                            gv.DrawTextOutlined("Press RMB to show current", gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 26 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined("temporary effects", gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 27 * txtH, 0.9f, "white");


                            //gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnScoringHitSpellNameSelf, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 8 * txtH, 1.0f, "white");

                        }

                    }
                    //this draw creature info
                    else
                    {
                        gv.DrawTextOutlined(gv.cc.floatyTextActorInfoName, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 3 * txtH, 0.9f, "red");
                        if (!gv.cc.inEffectMode)
                        {
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAC, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 4 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoMoveOrder, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 4 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoInitiative, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 4 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoHP, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 6 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSP, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 6 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoNumberOfAttacks, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 8 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoToHit, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 8 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAmmo, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 8 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAttackType, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAttackRange, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 9 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamage, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 10 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoDamageType, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 10 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnScoringHitSpellName, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 11 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSaves, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 13 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSaves2, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 13 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSaves3, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 13 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances1, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 14 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances2, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 14 * txtH, 0.9f, "yellow");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances3, gv.cc.floatyTextLocInfo.X + 2.7f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 14 * txtH, 0.9f, "lime");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances4, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 15 * txtH, 0.9f, "orange");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances5, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 15 * txtH, 0.9f, "blue");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances6, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 16 * txtH, 0.9f, "purple");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoResistances7, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 16 * txtH, 0.9f, "blue");
                            //17
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoCreatureTags, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 18 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoRegenerationHP, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 19 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoRegenerationSP, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 19 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoOnDeathScriptName, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 20 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAIType, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 21 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoAIAffinityForCasting, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 22 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoInjuryThreshold, gv.cc.floatyTextLocInfo.X + 1.5f * gv.squareSize, gv.cc.floatyTextLocInfo.Y + 22 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSpellsKnown1, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 23 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSpellsKnown2, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 24 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoSpellsKnown3, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 25 * txtH, 0.9f, "white");

                            ///gv.cc.floatyTextActorInfoRegenerationHP = "";
                            ///gv.cc.floatyTextActorInfoRegenerationSP = "";
                            //gv.cc.floatyTextActorInfoSpellsKnown1 = "";
                            //gv.cc.floatyTextActorInfoSpellsKnown2 = "";
                            //gv.cc.floatyTextActorInfoSpellsKnown3 = "";
                            //gv.cc.floatyTextActorInfoAIType = "";
                            //gv.cc.floatyTextActorInfoAIAffinityForCasting = "";//0 to 100
                            ///gv.cc.floatyTextActorInfoCreatureTags = "";//used for immunities, special weaknesses, eg "undead" are affected by turn spells and immunne to paralyze...
                            ///gv.cc.floatyTextActorInfoOnDeathScriptName = "";

                            //25
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoRMB1, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 26 * txtH, 0.9f, "white");
                            gv.DrawTextOutlined(gv.cc.floatyTextActorInfoRMB2, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 27 * txtH, 0.9f, "white");
                        }
                    }


                    //if (gv.cc.floatyTextActorInfoMoveOrder == "")
                    //{
                    gv.DrawTextOutlined(gv.cc.floatyTextActorInfoTempEffects1, gv.cc.floatyTextLocInfo.X, gv.cc.floatyTextLocInfo.Y + 4 * txtH, 1.0f, "white");
                    //}
                }
                foreach (IB2HtmlLogBox log in logList)
                {
                    if (gv.cc.floatyTextActorInfoName == "")
                    {
                        log.onDrawLogBox(this);
                    }
                }
            }

            //foreach (IB2ToggleButton btn in toggleList)
            //{
            //btn.Draw(this);
            //}
            /*
            src = new IbRect(0, 0, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Width, gv.cc.GetFromBitmapList(backgroundImageFilename).PixelSize.Height);
            dst = new IbRect(gv.pS, (int)((gv.playerOffsetY*2+1 -2) * gv.squareSize + 2*gv.pS), (int)(5 * gv.squareSize), (int)(1 * gv.squareSize - 2*gv.pS ));
            gv.DrawBitmap(gv.cc.GetFromBitmapList(backgroundImageFilename), src, dst, 0, false, 0.75f);
            */

            /*
            //iterate over all controls and draw
            foreach (IB2Button btn in buttonList)
            {
                btn.Draw(this);
            }
            foreach (IB2ToggleButton btn in toggleList)
            {
                btn.Draw(this);
            }
            foreach (IB2Portrait btn in portraitList)
            {
                btn.Draw(this);
            }
            */

        }

        public void Update(int elapsed)
        {
            //animate hiding panel
            if (hiding)
            {
                currentLocX += (hidingXIncrement * elapsed / 2);
                currentLocY += (hidingYIncrement * elapsed / 2);
                //hiding left and passed
                if ((hidingXIncrement < 0) && (currentLocX < hiddenLocX))
                {
                    currentLocX = hiddenLocX;
                    hiding = false;
                }
                //hiding right and passed
                if ((hidingXIncrement > 0) && (currentLocX > hiddenLocX))
                {
                    currentLocX = hiddenLocX;
                    hiding = false;
                }
                //hiding down and passed
                if ((hidingYIncrement > 0) && (currentLocY > hiddenLocY))
                {
                    currentLocY = hiddenLocY;
                    hiding = false;
                }
                //hiding up and passed
                if ((hidingYIncrement < 0) && (currentLocY < hiddenLocY))
                {
                    currentLocY = hiddenLocY;
                    hiding = false;
                }
            }
            else if (showing)
            {
                currentLocX -= (hidingXIncrement * elapsed);
                currentLocY -= (hidingYIncrement * elapsed);
                //showing right and passed
                if ((hidingXIncrement < 0) && (currentLocX > shownLocX))
                {
                    currentLocX = shownLocX;
                    showing = false;
                }
                //showing left and passed
                if ((hidingXIncrement > 0) && (currentLocX < shownLocX))
                {
                    currentLocX = shownLocX;
                    showing = false;
                }
                //showing up and passed
                if ((hidingYIncrement > 0) && (currentLocY < shownLocY))
                {
                    currentLocY = shownLocY;
                    showing = false;
                }
                //showing down and passed
                if ((hidingYIncrement < 0) && (currentLocY > shownLocY))
                {
                    currentLocY = shownLocY;
                    showing = false;
                }
            }
        }
    }
}
