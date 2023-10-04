using CollisionLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class Bible_Platform : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Platform");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public CollisionSurface[] colliders = null;
        public override void SetDefaults()
        {
            NPC.width = 132;
            NPC.height = 6;
            NPC.lifeMax = 1;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.alpha = 255;
        }
        public override bool CheckActive() => true;
        public override bool PreAI()
        {
            if (colliders == null || colliders.Length != 1)
            {
                NPC.velocity.Y -= 4;
                colliders = new CollisionSurface[] {
                    new CollisionSurface(NPC.TopLeft, NPC.TopRight, new int[] { 2, 0, 0, 0 }, true) };
            }
            /*
             * CollisionStyles controls which sides of the player can collide with each surface,
             * index 0 = bottom collision
             * index 1 = top collision
             * index 2 = left collision
             * index 4 = right collision
             * 
             * value 0 = doesnt collide
             * value 1 = collides without the ability to drop through
             * value 2 = collides but the player can drop through, doesn't do anything different from value 1 unless the index is zero (Ie, the surface is set to collide with the bottom of the player)
             *
            */
            return true;
        }
        public override void AI()
        {

            if (colliders != null && colliders.Length == 1 && NPC.alpha < 100)
            {
                colliders[0].Update();
                colliders[0].endPoints[0] = NPC.Center + (NPC.TopLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[0].endPoints[1] = NPC.Center + (NPC.TopRight - NPC.Center).RotatedBy(NPC.rotation);
            }
            NPC.velocity *= 0.9f;
            if (NPC.ai[0]++ >= 200)
            {
                float rot = Main.rand.NextFloat(-0.06f, 0.06f);
                NPC.rotation = rot;
            }
            if (NPC.ai[0] >= 240)
            {
                NPC.alpha += 10;
                if (NPC.alpha >= 255)
                    NPC.active = false;
            }
            else
            {
                if (NPC.alpha > 0)
                    NPC.alpha -= 10;
            }
        }
        public override void PostAI()
        {
            if (colliders != null)
            {
                foreach (CollisionSurface collider in colliders)
                {
                    collider.PostUpdate();
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0f, 0.15f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, NPC.Center - new Vector2(0, -2 + (NPC.velocity.Y * 2)) - screenPos, null, NPC.GetAlpha(Color.White), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, NPC.Center - new Vector2(0, -2 + (NPC.velocity.Y * 2)) - screenPos, null, NPC.GetAlpha(Color.White) * 0.5f, NPC.rotation, drawOrigin, NPC.scale + scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
    public class Bible_Platform2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Platform");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public CollisionSurface[] colliders = null;
        public override void SetDefaults()
        {
            NPC.width = 264;
            NPC.height = 6;
            NPC.lifeMax = 1;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.alpha = 255;
        }
        public override bool CheckActive() => true;
        public override bool PreAI()
        {
            if (colliders == null || colliders.Length != 1)
            {
                NPC.velocity.Y -= 4;
                colliders = new CollisionSurface[] {
                    new CollisionSurface(NPC.TopLeft, NPC.TopRight, new int[] { 2, 0, 0, 0 }, true) };
            }
            return true;
        }
        public override void AI()
        {
            if (colliders != null && colliders.Length == 1 && NPC.alpha < 100)
            {
                colliders[0].Update();
                colliders[0].endPoints[0] = NPC.Center + (NPC.TopLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[0].endPoints[1] = NPC.Center + (NPC.TopRight - NPC.Center).RotatedBy(NPC.rotation);
            }
            NPC.velocity *= 0.9f;
            if (NPC.ai[0]++ >= 120)
            {
                NPC.velocity.Y = 3;
                if (NPC.ai[0]++ >= 160)
                    NPC.alpha++;
                if (NPC.alpha >= 255)
                    NPC.active = false;
            }
            else
            {
                if (NPC.alpha > 0)
                    NPC.alpha -= 10;
            }
        }
        public override void PostAI()
        {
            if (colliders != null)
            {
                foreach (CollisionSurface collider in colliders)
                {
                    collider.PostUpdate();
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            float scale = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0f, 0.15f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, NPC.Center - new Vector2(0, -2 + (NPC.velocity.Y * 2)) - screenPos, null, NPC.GetAlpha(Color.White), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, NPC.Center - new Vector2(0, -2 + (NPC.velocity.Y * 2)) - screenPos, null, NPC.GetAlpha(Color.White) * 0.5f, NPC.rotation, drawOrigin, NPC.scale + scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}