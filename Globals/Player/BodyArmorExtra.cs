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
    class BodyArmorExtra : PlayerDrawLayer
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return true;
        }
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Torso);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Terraria.Player drawPlayer = drawInfo.drawPlayer;
            if (OnBodyDraw.BodyDictionary.ContainsKey(drawPlayer.body))
            {
                Color color12 = drawInfo.colorArmorBody;
                bool glowmask = OnBodyDraw.BodyDictionary[drawPlayer.body].glowmask;
                if (glowmask)
                    color12 = Color.White;

                Texture2D texture = OnBodyDraw.BodyDictionary[drawPlayer.body].texture;
                int useShader = OnBodyDraw.BodyDictionary[drawPlayer.body].useShader;
                Vector2 vector = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawInfo.drawPlayer.bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
                Vector2 value = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
                value.Y -= 2f;
                vector += value * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();
                float bodyRotation = drawInfo.drawPlayer.bodyRotation;
                DrawData drawData;
                if (!drawInfo.drawPlayer.invis || IsArmorDrawnWhenInvisible(drawInfo.drawPlayer.body))
                {
                    Rectangle useFrame = drawInfo.compTorsoFrame;
                    int frameCount = OnBodyDraw.BodyDictionary[drawPlayer.body].cycleFrameCount;
                    if (frameCount > 1)
                    {
                        int f = (int)(Main.GlobalTimeWrappedHourly / 10 % (frameCount + (frameCount - 2)));
                        if (f >= frameCount)
                            f = frameCount - 1 - (f - frameCount);

                        useFrame.Y = 224 * f;
                    }
                    drawData = new DrawData(texture, vector, useFrame, color12, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect, 0)
                    {
                        shader = drawInfo.cBody
                    };
                    if (useShader != -1)
                        drawData.shader = drawPlayer.dye[useShader].dye;

                    DrawCompositeArmorPiece(ref drawInfo, CompositePlayerDrawContext.Torso, drawData);
                }

            }
        }
        public static Vector2 GetCompositeOffset_BackArm(ref PlayerDrawSet drawInfo)
        {
            return new Vector2(6 * ((!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally)) ? 1 : (-1)), 2 * ((!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically)) ? 1 : (-1)));
        }
        public static bool IsArmorDrawnWhenInvisible(int torsoID)
        {
            if ((uint)(torsoID - 21) <= 1u)
                return false;

            return true;
        }
        public static void DrawCompositeArmorPiece(ref PlayerDrawSet drawInfo, CompositePlayerDrawContext context, DrawData data)
        {
            drawInfo.DrawDataCache.Add(data);
            switch (context)
            {
                case CompositePlayerDrawContext.BackShoulder:
                case CompositePlayerDrawContext.BackArm:
                case CompositePlayerDrawContext.FrontArm:
                case CompositePlayerDrawContext.FrontShoulder:
                    {
                        if (drawInfo.armGlowColor.PackedValue == 0)
                            break;

                        DrawData item2 = data;
                        item2.color = drawInfo.armGlowColor;
                        Rectangle value3 = item2.sourceRect.Value;
                        value3.Y += 224;
                        item2.sourceRect = value3;
                        drawInfo.DrawDataCache.Add(item2);
                        break;
                    }
                case CompositePlayerDrawContext.Torso:
                    {
                        if (drawInfo.bodyGlowColor.PackedValue == 0)
                            break;

                        DrawData item = data;
                        item.color = drawInfo.bodyGlowColor;
                        Rectangle value = item.sourceRect.Value;
                        value.Y += 224;
                        item.sourceRect = value;
                        drawInfo.DrawDataCache.Add(item);
                        break;
                    }
            }
        }
    }
    class ArmDrawExtra : PlayerDrawLayer
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return true;
        }
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Terraria.Player drawPlayer = drawInfo.drawPlayer;
            if (OnBodyDraw.BodyDictionary.ContainsKey(drawPlayer.body))
            {
                Color color12 = drawInfo.colorArmorBody;
                bool glowmask = OnBodyDraw.BodyDictionary[drawPlayer.body].glowmask;
                if (glowmask)
                    color12 = Color.White;

                Texture2D texture = OnBodyDraw.BodyDictionary[drawPlayer.body].texture;
                int useShader = OnBodyDraw.BodyDictionary[drawPlayer.body].useShader;

                Vector2 vector = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawInfo.drawPlayer.bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
                Vector2 value = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
                value.Y -= 2f;
                vector += value * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();
                float bodyRotation = drawInfo.drawPlayer.bodyRotation;
                float rotation = drawInfo.drawPlayer.bodyRotation + drawInfo.compositeFrontArmRotation;
                Vector2 bodyVect = drawInfo.bodyVect;
                Vector2 compositeOffset_FrontArm = GetCompositeOffset_FrontArm(ref drawInfo);
                bodyVect += compositeOffset_FrontArm;
                vector += compositeOffset_FrontArm;
                Vector2 position = vector + drawInfo.frontShoulderOffset;
                if (drawInfo.compFrontArmFrame.X / drawInfo.compFrontArmFrame.Width >= 7)
                {
                    vector += new Vector2((!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally)) ? 1 : (-1), (!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically)) ? 1 : (-1));
                }
                int num2 = drawInfo.compShoulderOverFrontArm ? 1 : 0;
                int num3 = (!drawInfo.compShoulderOverFrontArm) ? 1 : 0;
                DrawData drawData;

                if (!drawInfo.drawPlayer.invis || BodyArmorExtra.IsArmorDrawnWhenInvisible(drawInfo.drawPlayer.body))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (i == num2 && !drawInfo.hideCompositeShoulders)
                        {
                            drawData = new DrawData(texture, position, drawInfo.compFrontShoulderFrame, color12, bodyRotation, bodyVect, 1f, drawInfo.playerEffect, 0)
                            {
                                shader = drawInfo.cBody
                            };
                            if (useShader != -1)
                                drawData.shader = drawPlayer.dye[useShader].dye;

                            BodyArmorExtra.DrawCompositeArmorPiece(ref drawInfo, CompositePlayerDrawContext.FrontShoulder, drawData);
                        }
                        if (i == num3)
                        {
                            drawData = new DrawData(texture, vector, drawInfo.compFrontArmFrame, color12, rotation, bodyVect, 1f, drawInfo.playerEffect, 0)
                            {
                                shader = drawInfo.cBody
                            };
                            if (useShader != -1)
                                drawData.shader = drawPlayer.dye[useShader].dye;

                            BodyArmorExtra.DrawCompositeArmorPiece(ref drawInfo, CompositePlayerDrawContext.FrontArm, drawData);
                        }
                    }
                }
            }

        }
        private static Vector2 GetCompositeOffset_FrontArm(ref PlayerDrawSet drawinfo)
        {
            return new Vector2(-5 * ((!drawinfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally)) ? 1 : (-1)), 0f);
        }
    }
    public class OnBodyDraw
    {
        public static Dictionary<int, OnBodyDraw> BodyDictionary = new();
        public Texture2D texture;
        public bool glowmask = true;
        public int useShader = -1;
        public int cycleFrameCount = 1;
        public OnBodyDraw(Texture2D texture, bool glowmask = true, int useShader = -1, int cycleFrameCount = 1)
        {
            this.texture = texture;
            this.glowmask = glowmask;
            this.useShader = useShader;
            this.cycleFrameCount = cycleFrameCount;
        }
        public static void RegisterBodies()
        {
            var immediate = AssetRequestMode.ImmediateLoad;
            Mod mod = Redemption.Instance;
            OnBodyDraw body = new(Request<Texture2D>("Redemption/Items/Armor/HM/Hardlight/HardlightPlate_Body_Glow", immediate).Value);
            BodyDictionary.Add(mod.GetEquipSlot("HardlightPlate", EquipType.Body), body);
            body = new(Request<Texture2D>("Redemption/Items/Armor/HM/Xenomite/XenomitePlate_Body_Glow", immediate).Value);
            BodyDictionary.Add(mod.GetEquipSlot("XenomitePlate", EquipType.Body), body);
        }
    }
}