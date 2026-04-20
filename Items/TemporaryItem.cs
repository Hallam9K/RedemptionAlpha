using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption.Items
{
    public abstract class TemporaryItem : ModItem
    {
        public virtual bool VisibleInUI => false;
        public override void Load()
        {
            On_Player.dropItemCheck += new On_Player.hook_dropItemCheck(DontDropCoolStuff);
            On_ItemSlot.LeftClick_ItemArray_int_int += new On_ItemSlot.hook_LeftClick_ItemArray_int_int(LockLeftMouseToSpecialItem);
            On_ItemSlot.RightClick_ItemArray_int_int += new On_ItemSlot.hook_RightClick_ItemArray_int_int(LockRightMouseToSpecialItem);
            On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += new On_ItemSlot.hook_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color(DrawSpecial);
        }
        public override void Unload()
        {
            On_Player.dropItemCheck -= new On_Player.hook_dropItemCheck(DontDropCoolStuff);
            On_ItemSlot.LeftClick_ItemArray_int_int -= new On_ItemSlot.hook_LeftClick_ItemArray_int_int(LockLeftMouseToSpecialItem);
            On_ItemSlot.RightClick_ItemArray_int_int -= new On_ItemSlot.hook_RightClick_ItemArray_int_int(LockRightMouseToSpecialItem);
            On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color -= new On_ItemSlot.hook_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color(DrawSpecial);
        }

        private void DrawSpecial(On_ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch sb, Item[] inv, int context, int slot, Vector2 position, Color color)
        {
            if (inv[slot].ModItem is not TemporaryItem || (inv[slot].ModItem as TemporaryItem).VisibleInUI)
            {
                orig.Invoke(sb, inv, context, slot, position, color);
            }
        }

        public override void PostUpdate()
        {
            Item.type = ItemID.None;
            Item.stack = 0;
        }

        public override bool CanPickup(Player player)
        {
            return false;
        }
        private void ManageRightClickFeatures(On_Player.orig_ItemCheck_ManageRightClickFeatures orig, Player self)
        {
            if (Main.mouseItem.ModItem is not TemporaryItem)
            {
                orig.Invoke(self);
            }
        }
        private void LockLeftMouseToSpecialItem(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            if (Main.mouseItem.ModItem is not TemporaryItem)
            {
                orig.Invoke(inv, context, slot);
            }
        }
        private void LockRightMouseToSpecialItem(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            if (Main.mouseItem.ModItem is not TemporaryItem)
            {
                orig.Invoke(inv, context, slot);
            }
        }

        private void DontDropCoolStuff(On_Player.orig_dropItemCheck orig, Player self)
        {
            if (Main.mouseItem.ModItem is not TemporaryItem)
            {
                orig.Invoke(self);
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return false;
        }
    }
}
