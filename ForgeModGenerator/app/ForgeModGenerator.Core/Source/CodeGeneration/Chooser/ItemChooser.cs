﻿namespace ForgeModGenerator.CodeGeneration
{
    public class ItemChooser : ChooseCollection
    {
        protected override string[] BuiltInGetters => new string[] {
            "Items.AIR",
            "Items.IRON_SHOVEL",
            "Items.IRON_PICKAXE",
            "Items.IRON_AXE",
            "Items.FLINT_AND_STEEL",
            "Items.APPLE",
            "Items.BOW",
            "Items.ARROW",
            "Items.SPECTRAL_ARROW",
            "Items.TIPPED_ARROW",
            "Items.COAL",
            "Items.DIAMOND",
            "Items.IRON_INGOT",
            "Items.GOLD_INGOT",
            "Items.IRON_SWORD",
            "Items.WOODEN_SWORD",
            "Items.WOODEN_SHOVEL",
            "Items.WOODEN_PICKAXE",
            "Items.WOODEN_AXE",
            "Items.STONE_SWORD",
            "Items.STONE_SHOVEL",
            "Items.STONE_PICKAXE",
            "Items.STONE_AXE",
            "Items.DIAMOND_SWORD",
            "Items.DIAMOND_SHOVEL",
            "Items.DIAMOND_PICKAXE",
            "Items.DIAMOND_AXE",
            "Items.STICK",
            "Items.BOWL",
            "Items.MUSHROOM_STEW",
            "Items.GOLDEN_SWORD",
            "Items.GOLDEN_SHOVEL",
            "Items.GOLDEN_PICKAXE",
            "Items.GOLDEN_AXE",
            "Items.STRING",
            "Items.FEATHER",
            "Items.GUNPOWDER",
            "Items.WOODEN_HOE",
            "Items.STONE_HOE",
            "Items.IRON_HOE",
            "Items.DIAMOND_HOE",
            "Items.GOLDEN_HOE",
            "Items.WHEAT_SEEDS",
            "Items.WHEAT",
            "Items.BREAD",
            "Items.LEATHER_HELMET",
            "Items.LEATHER_CHESTPLATE",
            "Items.LEATHER_LEGGINGS",
            "Items.LEATHER_BOOTS",
            "Items.CHAINMAIL_HELMET",
            "Items.CHAINMAIL_CHESTPLATE",
            "Items.CHAINMAIL_LEGGINGS",
            "Items.CHAINMAIL_BOOTS",
            "Items.IRON_HELMET",
            "Items.IRON_CHESTPLATE",
            "Items.IRON_LEGGINGS",
            "Items.IRON_BOOTS",
            "Items.DIAMOND_HELMET",
            "Items.DIAMOND_CHESTPLATE",
            "Items.DIAMOND_LEGGINGS",
            "Items.DIAMOND_BOOTS",
            "Items.GOLDEN_HELMET",
            "Items.GOLDEN_CHESTPLATE",
            "Items.GOLDEN_LEGGINGS",
            "Items.GOLDEN_BOOTS",
            "Items.FLINT",
            "Items.PORKCHOP",
            "Items.COOKED_PORKCHOP",
            "Items.PAINTING",
            "Items.GOLDEN_APPLE",
            "Items.SIGN",
            "Items.OAK_DOOR",
            "Items.SPRUCE_DOOR",
            "Items.BIRCH_DOOR",
            "Items.JUNGLE_DOOR",
            "Items.ACACIA_DOOR",
            "Items.DARK_OAK_DOOR",
            "Items.BUCKET",
            "Items.WATER_BUCKET",
            "Items.LAVA_BUCKET",
            "Items.MINECART",
            "Items.SADDLE",
            "Items.IRON_DOOR",
            "Items.REDSTONE",
            "Items.SNOWBALL",
            "Items.BOAT",
            "Items.SPRUCE_BOAT",
            "Items.BIRCH_BOAT",
            "Items.JUNGLE_BOAT",
            "Items.ACACIA_BOAT",
            "Items.DARK_OAK_BOAT",
            "Items.LEATHER",
            "Items.MILK_BUCKET",
            "Items.BRICK",
            "Items.CLAY_BALL",
            "Items.REEDS",
            "Items.PAPER",
            "Items.BOOK",
            "Items.SLIME_BALL",
            "Items.CHEST_MINECART",
            "Items.FURNACE_MINECART",
            "Items.EGG",
            "Items.COMPASS",
            "Items.FISHING_ROD",
            "Items.CLOCK",
            "Items.GLOWSTONE_DUST",
            "Items.FISH",
            "Items.COOKED_FISH",
            "Items.DYE",
            "Items.BONE",
            "Items.SUGAR",
            "Items.CAKE",
            "Items.BED",
            "Items.REPEATER",
            "Items.COOKIE",
            "Items.FILLED_MAP",
            "Items.SHEARS",
            "Items.MELON",
            "Items.PUMPKIN_SEEDS",
            "Items.MELON_SEEDS",
            "Items.BEEF",
            "Items.COOKED_BEEF",
            "Items.CHICKEN",
            "Items.COOKED_CHICKEN",
            "Items.MUTTON",
            "Items.COOKED_MUTTON",
            "Items.RABBIT",
            "Items.COOKED_RABBIT",
            "Items.RABBIT_STEW",
            "Items.RABBIT_FOOT",
            "Items.RABBIT_HIDE",
            "Items.ROTTEN_FLESH",
            "Items.ENDER_PEARL",
            "Items.BLAZE_ROD",
            "Items.GHAST_TEAR",
            "Items.GOLD_NUGGET",
            "Items.NETHER_WART",
            "Items.POTIONITEM",
            "Items.SPLASH_POTION",
            "Items.LINGERING_POTION",
            "Items.GLASS_BOTTLE",
            "Items.DRAGON_BREATH",
            "Items.SPIDER_EYE",
            "Items.FERMENTED_SPIDER_EYE",
            "Items.BLAZE_POWDER",
            "Items.MAGMA_CREAM",
            "Items.BREWING_STAND",
            "Items.CAULDRON",
            "Items.ENDER_EYE",
            "Items.SPECKLED_MELON",
            "Items.SPAWN_EGG",
            "Items.EXPERIENCE_BOTTLE",
            "Items.FIRE_CHARGE",
            "Items.WRITABLE_BOOK",
            "Items.WRITTEN_BOOK",
            "Items.EMERALD",
            "Items.ITEM_FRAME",
            "Items.FLOWER_POT",
            "Items.CARROT",
            "Items.POTATO",
            "Items.BAKED_POTATO",
            "Items.POISONOUS_POTATO",
            "Items.MAP",
            "Items.GOLDEN_CARROT",
            "Items.SKULL",
            "Items.CARROT_ON_A_STICK",
            "Items.NETHER_STAR",
            "Items.PUMPKIN_PIE",
            "Items.FIREWORKS",
            "Items.FIREWORK_CHARGE",
            "Items.ENCHANTED_BOOK",
            "Items.COMPARATOR",
            "Items.NETHERBRICK",
            "Items.QUARTZ",
            "Items.TNT_MINECART",
            "Items.HOPPER_MINECART",
            "Items.ARMOR_STAND",
            "Items.IRON_HORSE_ARMOR",
            "Items.GOLDEN_HORSE_ARMOR",
            "Items.DIAMOND_HORSE_ARMOR",
            "Items.LEAD",
            "Items.NAME_TAG",
            "Items.COMMAND_BLOCK_MINECART",
            "Items.RECORD_13",
            "Items.RECORD_CAT",
            "Items.RECORD_BLOCKS",
            "Items.RECORD_CHIRP",
            "Items.RECORD_FAR",
            "Items.RECORD_MALL",
            "Items.RECORD_MELLOHI",
            "Items.RECORD_STAL",
            "Items.RECORD_STRAD",
            "Items.RECORD_WARD",
            "Items.RECORD_11",
            "Items.RECORD_WAIT",
            "Items.PRISMARINE_SHARD",
            "Items.PRISMARINE_CRYSTALS",
            "Items.BANNER",
            "Items.END_CRYSTAL",
            "Items.SHIELD",
            "Items.ELYTRA",
            "Items.CHORUS_FRUIT",
            "Items.CHORUS_FRUIT_POPPED",
            "Items.BEETROOT_SEEDS",
            "Items.BEETROOT",
            "Items.BEETROOT_SOUP",
            "Items.TOTEM_OF_UNDYING",
            "Items.SHULKER_SHELL",
            "Items.IRON_NUGGET",
            "Items.KNOWLEDGE_BOOK",
        };
    }
}
