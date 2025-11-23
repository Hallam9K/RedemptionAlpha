using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Globals.Players
{
    public class DrawEffectsPlayer : ModPlayer
    {
        /// <summary>
        /// The current physics chain accessory being displayed, or null.
        /// </summary>
        internal IPhysChain bodyPhysChain;

        /// <summary>
        /// The positions of all segments of the chain
        /// </summary>
        internal Vector3[] bodyPhysChainPositions;

        /// <summary>
        /// The associated item for the physchain to render dyes and shaders from
        /// </summary>
        public Item bodyPhysChainMiscItem;

        /// <summary>
        /// The shader index of the dye this physchain should be drawn using
        /// </summary>
        public int cPhysChain;

        public override void Load()
        {
            On_Player.UpdateDyes += Player_UpdateDyes;
            On_Player.UpdateItemDye += Player_UpdateItemDye;
        }

        public override void Unload()
        {
            On_Player.UpdateDyes -= Player_UpdateDyes;
            On_Player.UpdateItemDye -= Player_UpdateItemDye;
        }

        public override void ResetEffects()
        {
            if (bodyPhysChain == null)
                ResetPhysics();
            else
            {
                if (bodyPhysChainPositions.Length != bodyPhysChain.NumberOfSegments)
                    bodyPhysChainPositions = new Vector3[bodyPhysChain.NumberOfSegments];
            }
            bodyPhysChain = null;
        }

        /// <summary>
        /// Reset custom dyes alongside vanilla.
        /// </summary>
        private void Player_UpdateDyes(On_Player.orig_UpdateDyes orig, Terraria.Player self)
        {
            if (self.TryGetModPlayer(out DrawEffectsPlayer dPlayer))
                dPlayer.cPhysChain = 0;

            orig(self);
        }

        /// <summary>
        /// Updates custom dye fields. Can't really use ModPlayer.UpdateDyes() because it doesn't account for modded accessory slots.
        /// </summary>
        private static void Player_UpdateItemDye(On_Player.orig_UpdateItemDye orig, Terraria.Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);

            if (!self.TryGetModPlayer(out DrawEffectsPlayer dPlayer))
                return;

            //if (SoASets.Item.HasPhysChain[armorItem.type])
            if (armorItem == dPlayer.bodyPhysChainMiscItem)
                dPlayer.cPhysChain = dyeItem.dye;
        }

        private void ResetPhysics()
        {
            bodyPhysChainPositions = Array.Empty<Vector3>();
        }

        public void SetPhysChain(IPhysChain newPhysChain, Item item)
        {
            if (bodyPhysChain?.GetType() != newPhysChain?.GetType())
            {
                bodyPhysChain = newPhysChain;
            }
            bodyPhysChainMiscItem = item;
        }
    }
}