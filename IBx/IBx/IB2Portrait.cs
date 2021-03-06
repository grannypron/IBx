﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBx
{
    public class IB2Portrait
    {
        [JsonIgnore]
        public GameView gv;
        public string tag = "";
        public string ImgBGFilename = "";
        public string ImgFilename = "";
        public string ImgLUFilename = ""; //used for level up icon
        public string GlowFilename = "";
        public bool glowOn = false;
        public bool levelUpOn = false;

        public string TextHP = "";
        public string TextSP = "";
        public string levelUpSymbol = "";
        public string chatSymbol = "";
        public int X = 0;
        public int Y = 0;
        public int Width = 0;
        public int Height = 0;
        public float scaler = 1.0f;
        public bool playedHoverSound = false;
        public bool show = false;

        public IB2Portrait()
        {

        }

        public IB2Portrait(GameView g)
        {
            gv = g;
        }

        public void setupIB2Portrait(GameView g)
        {
            gv = g;
        }

        public void setHover(IB2Panel parentPanel, int x, int y)
        {
            //int Width = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Width;
            //int Height = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Height;
            if (show)
            {
                glowOn = false;

                if ((x >= (int)((parentPanel.currentLocX + X) * gv.screenDensity)) && (x <= (int)((parentPanel.currentLocX + X + Width) * gv.screenDensity)))
                {
                    if ((y >= (int)((parentPanel.currentLocY + Y) * gv.screenDensity)) && (y <= (int)((parentPanel.currentLocY + Y + Height) * gv.screenDensity)))
                    {
                        if (!playedHoverSound)
                        {
                            playedHoverSound = true;
                        }
                        glowOn = true;
                    }
                }
                playedHoverSound = false;
            }
        }

        public bool getImpact(IB2Panel parentPanel, int x, int y)
        {
            //int Width = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Width;
            //int Height = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Height;
            if (show)
            {
                if ((x >= (int)((parentPanel.currentLocX + X) * gv.screenDensity)) && (x <= (int)((parentPanel.currentLocX + X + Width) * gv.screenDensity)))
                {
                    if ((y >= (int)((parentPanel.currentLocY + Y) * gv.screenDensity)) && (y <= (int)((parentPanel.currentLocY + Y + Height) * gv.screenDensity)))
                    {
                        if (!playedHoverSound)
                        {
                            playedHoverSound = true;
                        }
                        return true;
                    }
                }
                playedHoverSound = false;
            }
            return false;
        }

        public void Draw(IB2Panel parentPanel)
        {
            if (show)
            {
                int pH = (int)((float)gv.screenHeight / 200.0f);
                int pW = (int)((float)gv.screenHeight / 200.0f);
                float fSize = (float)(gv.squareSize / 4) * scaler;
                //int Width = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Width;
                //int Height = gv.cc.GetFromBitmapList(ImgFilename).PixelSize.Height;

                IbRect src = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgFilename).Width, gv.cc.GetFromBitmapList(ImgFilename).Height);
                IbRect srcBG = new IbRect(0, 0, gv.cc.GetFromBitmapList(ImgBGFilename).Width, gv.cc.GetFromBitmapList(ImgBGFilename).Height);
                IbRect dst = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity), (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity), (int)((float)Width * gv.screenDensity), (int)((float)Height * gv.screenDensity));
                IbRect dstBG = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity) - (int)(3 * gv.screenDensity),
                                            (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity) - (int)(3 * gv.screenDensity),
                                            (int)((float)Width * gv.screenDensity) + (int)(6 * gv.screenDensity),
                                            (int)((float)Height * gv.screenDensity) + (int)(6 * gv.screenDensity));

                IbRect srcGlow = new IbRect(0, 0, gv.cc.GetFromBitmapList(GlowFilename).Width, gv.cc.GetFromBitmapList(GlowFilename).Height);
                IbRect dstGlow = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity) - (int)(7 * gv.screenDensity),
                                            (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity) - (int)(7 * gv.screenDensity),
                                            (int)((float)Width * gv.screenDensity) + (int)(15 * gv.screenDensity),
                                            (int)((float)Height * gv.screenDensity) + (int)(15 * gv.screenDensity));

                gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgBGFilename), src, dstBG, true);

                if (glowOn)
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(GlowFilename), srcGlow, dstGlow, true);
                }

                if (!ImgFilename.Equals(""))
                {
                    gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgFilename), src, dst, true);
                }

                if (!ImgLUFilename.Equals(""))
                {
                    if (levelUpOn)
                    {
                        gv.DrawBitmap(gv.cc.GetFromBitmapList(ImgLUFilename), src, dst, true);
                    }
                }

                if (gv.mod.useUIBackground)
                {
                    IbRect srcFrame = new IbRect(0, 0, gv.cc.ui_portrait_frame.Width, gv.cc.ui_portrait_frame.Height);
                    IbRect dstFrame = new IbRect((int)((parentPanel.currentLocX + this.X) * gv.screenDensity) - (int)(5 * gv.screenDensity),
                                            (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity) - (int)(5 * gv.screenDensity),
                                            (int)((float)Width * gv.screenDensity) + (int)(10 * gv.screenDensity),
                                            (int)((float)Height * gv.screenDensity) + (int)(10 * gv.screenDensity));
                    gv.DrawBitmap(gv.cc.ui_portrait_frame, srcFrame, dstFrame, true);
                }

                float thisFontHeight = gv.drawFontRegHeight;
                if (scaler > 1.05f)
                {
                    thisFontHeight = gv.drawFontLargeHeight;
                }
                else if (scaler < 0.95f)
                {
                    thisFontHeight = gv.drawFontSmallHeight;
                }

                //DRAW HP/HPmax
                // Measure string.
                //SizeF stringSize = gv.cc.MeasureString(TextHP, thisFont, this.Width);
                //float stringSize = gv.cc.MeasureString(TextHP, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

                //int ulX = ((int)(this.Width) / 2) - ((int)stringSize / 2);
                //int ulY = ((int)(this.Height / 2) / 2) + ((int)thisFontHeight / 2);
                int ulX = pW * 0;
                int ulY = (int)(Height * gv.screenDensity) - ((int)thisFontHeight * 2);
                /*
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x != 0 || y != 0)
                        {
                            int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX + x);
                            int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY - pH + y);
                            gv.DrawText(TextHP, xLoc, yLoc, scaler, Color.Black);
                        }
                    }
                }
                */
                int xLoc1 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX);
                int yLoc1 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY - pH);
                gv.DrawTextOutlined(TextHP, xLoc1, yLoc1, scaler, "lime");

                //DRAW SP/SPmax
                // Measure string.
                //stringSize = gv.cc.MeasureString(TextSP, thisFont, this.Width);
                //stringSize = gv.cc.MeasureString(TextSP, SharpDX.DirectWrite.FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, thisFontHeight);

                //ulX = ((int)(this.Width / 2)) - ((int)stringSize);
                //ulY = ((int)(this.Height / 2));
                ulX = pW * 1;
                ulY = (int)(Height * gv.screenDensity) - ((int)thisFontHeight * 1);

                /*
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x != 0 || y != 0)
                        {
                            int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX - pW + x);
                            int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY - pH + y);
                            gv.DrawText(TextSP, xLoc, yLoc, scaler, Color.Black);
                        }
                    }
                }
                */
                int xLoc2 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX - pW);
                int yLoc2 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY - pH);
                gv.DrawTextOutlined(TextSP, xLoc2, yLoc2, scaler, "yellow");
                //gv.DrawTextOutlined(TextSP, xLoc2, yLoc2, gv.FontWeight.Normal, scaler, Color.Yellow);
                //DrawText(text, xLoc, yLoc, FontWeight.Normal, SharpDX.DirectWrite.FontStyle.Normal, scaler, fontColor, false);
                //better reoute all to drawtext and from the to draw outlined

                //draw level up symbol
                ulX = (int)(110 * gv.screenDensity) - pW * 9;
                ulY = (int)(Height * gv.screenDensity) - ((int)thisFontHeight * 7) + pH;

                /*
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x != 0 || y != 0)
                        {
                            int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX - pW + x);
                            int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY - pH + y);
                            gv.DrawText(levelUpSymbol, xLoc, yLoc, scaler * 1.2f, Color.Black);
                        }
                    }
                }
                */
                int xLoc3 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX - pW);
                int yLoc3 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY - pH);
                gv.DrawTextOutlined(levelUpSymbol, xLoc3, yLoc3, scaler * 1.2f, "blue");


                //draw chat symbol
                int ulX5 = (int)(110 * gv.screenDensity) - pW * 24;
                int ulY5 = (int)(Height * gv.screenDensity) - ((int)thisFontHeight * 7) + 2 * pH;
                /*
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x != 0 || y != 0)
                        {
                            int xLoc = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX5 - pW + x);
                            int yLoc = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY5 - pH + y);
                            gv.DrawText(chatSymbol, xLoc, yLoc, scaler * 0.5f, Color.Black);
                        }
                    }
                }
                */
                int xLoc4 = (int)((parentPanel.currentLocX + this.X) * gv.screenDensity + ulX5 - pW);
                int yLoc4 = (int)((parentPanel.currentLocY + this.Y) * gv.screenDensity + ulY5 - pH);
                gv.DrawTextOutlined(chatSymbol, xLoc4, yLoc4, scaler * 0.5f, "blue");
            }

            //draw level up symbol
        }

        public void Update(int elapsed)
        {
            //animate button?
        }
    }
}
