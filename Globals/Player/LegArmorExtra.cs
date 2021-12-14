using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Redemption.Globals.Player
{
    class LegArmorExtra : PlayerDrawLayer
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return true;
        }
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Leggings);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Terraria.Player drawPlayer = drawInfo.drawPlayer;
            if (OnLegDraw.LegDictionary.ContainsKey(drawPlayer.legs))
            {
                Color color12 = drawInfo.colorArmorHead;
                bool glowmask = OnLegDraw.LegDictionary[drawPlayer.legs].glowmask;
                if (glowmask)
                    color12 = Color.White;

                Texture2D texture = OnLegDraw.LegDictionary[drawPlayer.legs].texture;
                int useShader = OnLegDraw.LegDictionary[drawPlayer.legs].useShader;

                if (!drawPlayer.Male)
                {
                    texture = OnLegDraw.LegDictionary[drawPlayer.legs].femaleTexture;
                }
                Vector2 legsOffset = drawInfo.legsOffset;
                DrawData item;
                if (drawInfo.isSitting)
                {
                    if (!ShouldOverrideLegs_CheckShoes(ref drawInfo) || drawInfo.drawPlayer.wearsRobe)
                    {
                        if (!drawInfo.drawPlayer.invis)
                            DrawSittingLegs(ref drawInfo, texture, color12, useShader, glowmask);
                    }
                }
                else if (!ShouldOverrideLegs_CheckShoes(ref drawInfo) || drawInfo.drawPlayer.wearsRobe)
                {
                    if (drawInfo.drawPlayer.invis)
                        return;

                    item = new DrawData(texture, legsOffset + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawInfo.drawPlayer.legFrame.Width / 2 + drawInfo.drawPlayer.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.legFrame.Height + 4f)) + drawInfo.drawPlayer.legPosition + drawInfo.legVect, drawInfo.drawPlayer.legFrame, color12, drawInfo.drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect, 0);
                    if (useShader == -1)
                        item.shader = drawInfo.cLegs;
                    else
                        item.shader = drawPlayer.dye[useShader].dye;

                    drawInfo.DrawDataCache.Add(item);
                }
            }
        }
        static void DrawSittingLegs(ref PlayerDrawSet drawInfo, Texture2D textureToDraw, Color matchingColor, int shaderIndex = -1, bool glowmask = false)
        {
            Terraria.Player drawPlayer = drawInfo.drawPlayer;
            Vector2 legsOffset = drawInfo.legsOffset;
            Vector2 value = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawInfo.drawPlayer.legFrame.Width / 2 + drawInfo.drawPlayer.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.legFrame.Height + 4f)) + drawInfo.drawPlayer.legPosition + drawInfo.legVect;
            Rectangle legFrame = drawInfo.drawPlayer.legFrame;
            value.Y -= 2f;
            value.Y += drawInfo.seatYOffset;
            value += legsOffset;
            int num = 2;
            int num2 = 42;
            int num3 = 2;
            int num4 = 2;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            bool flag = drawInfo.drawPlayer.legs == 101 || drawInfo.drawPlayer.legs == 102 || drawInfo.drawPlayer.legs == 118 || drawInfo.drawPlayer.legs == 99;
            if (drawInfo.drawPlayer.wearsRobe && !flag)
            {
                num = 0;
                num4 = 0;
                num2 = 6;
                value.Y += 4f;
                legFrame.Y = legFrame.Height * 5;
            }
            switch (drawInfo.drawPlayer.legs)
            {
                case 214:
                case 215:
                case 216:
                    num = -6;
                    num4 = 2;
                    num5 = 2;
                    num3 = 4;
                    num2 = 6;
                    legFrame = drawInfo.drawPlayer.legFrame;
                    value.Y += 2f;
                    break;
                case 106:
                case 143:
                case 226:
                    num = 0;
                    num4 = 0;
                    num2 = 6;
                    value.Y += 4f;
                    legFrame.Y = legFrame.Height * 5;
                    break;
                case 132:
                    num = -2;
                    num7 = 2;
                    break;
                case 193:
                case 194:
                    if (drawInfo.drawPlayer.body == 218)
                    {
                        num = -2;
                        num7 = 2;
                        value.Y += 2f;
                    }
                    break;
                case 210:
                    if (glowmask)
                    {
                        Vector2 vector = new(Main.rand.Next(-10, 10) * 0.125f, Main.rand.Next(-10, 10) * 0.125f);
                        value += vector;
                    }
                    break;
            }
            for (int num8 = num3; num8 >= 0; num8--)
            {
                Vector2 position = value + new Vector2(num, 2f) * new Vector2(drawInfo.drawPlayer.direction, 1f);
                Rectangle value2 = legFrame;
                value2.Y += num8 * 2;
                value2.Y += num2;
                value2.Height -= num2;
                value2.Height -= num8 * 2;
                if (num8 != num3)
                    value2.Height = 2;

                position.X += drawInfo.drawPlayer.direction * num4 * num8 + num6 * drawInfo.drawPlayer.direction;
                if (num8 != 0)
                    position.X += num7 * drawInfo.drawPlayer.direction;

                position.Y += num2;
                position.Y += num5;
                DrawData item = new(textureToDraw, position, value2, matchingColor, drawInfo.drawPlayer.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect, 0);
                if (shaderIndex == -1)
                    item.shader = drawInfo.cLegs;
                else
                    item.shader = drawPlayer.dye[shaderIndex].dye;

                drawInfo.DrawDataCache.Add(item);
            }
        }
        public static bool ShouldOverrideLegs_CheckShoes(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.shoe == 15)
                return true;

            return false;
        }
    }
    public class OnLegDraw
    {
        public static Dictionary<int, OnLegDraw> LegDictionary = new();
        public Texture2D texture;
        public Texture2D femaleTexture;
        public bool glowmask = true;
        public int useShader = -1;
        public OnLegDraw(Texture2D texture, Texture2D femaleTexture, bool glowmask = true, int useShader = -1)
        {
            this.texture = texture;
            this.femaleTexture = femaleTexture;
            this.glowmask = glowmask;
            this.useShader = useShader;
        }
        public static void RegisterLegs()
        {
            var immediate = AssetRequestMode.ImmediateLoad;
            Mod mod = Redemption.Instance;
            OnLegDraw leg = new(Request<Texture2D>("Redemption/Items/Armor/HM/Hardlight/HardlightBoots_Legs_Glow", immediate).Value, Request<Texture2D>("Redemption/Items/Armor/HM/Hardlight/HardlightBoots_Legs_Glow", immediate).Value);
            LegDictionary.Add(mod.GetEquipSlot("HardlightBoots", EquipType.Legs), leg);
            leg = new(Request<Texture2D>("Redemption/Items/Armor/HM/Xenomite/XenomiteLeggings_Legs_Glow", immediate).Value, Request<Texture2D>("Redemption/Items/Armor/HM/Xenomite/XenomiteLeggings_Legs_Glow", immediate).Value);
            LegDictionary.Add(mod.GetEquipSlot("XenomiteLeggings", EquipType.Legs), leg);
        }
    }
}