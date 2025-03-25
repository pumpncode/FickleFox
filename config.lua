--- STEAMODDED HEADER
--- MOD_NAME: FoxMods
--- MOD_ID: FoxMods
--- PREFIX: fox
--- MOD_AUTHOR: FoxDeploy
--- MOD_DESCRIPTION: Adds unbalanced ideas to Balatro.
--- BADGE_COLOUR: ffaa33
--- DEPENDENCIES: [Talisman>=2.0.0-beta8, Steamodded>=1.0.0~ALPHA-0917a]
--- VERSION: 0.0.2
local mod = SMODS.current_mod
sendInfoMessage("Loading config.lua", "FoxMods-config.lua")
FoxMod = {}
FoxMod.config = SMODS.current_mod.config


SMODS.Atlas {key = "modicon", path = "modlogo.png", px = 34,  py = 34,}

SMODS.Sound({key = "metsu",path = "metsu.ogg",atlas_table = "ASSET_ATLAS"})
SMODS.Sound({key = "ghostRare",	path = "woosh.mp3",atlas_table = "ASSET_ATLAS"})
SMODS.Sound({key = "secretRare",	path = "secretRare.mp3",atlas_table = "ASSET_ATLAS"})
SMODS.Sound({key = "kirby_poyo",	path = "kirby-poyo-time-d.mp3",atlas_table = "ASSET_ATLAS"})
SMODS.Sound({key = "kirby_suction",	path = "kirby-suction.mp3",atlas_table = "ASSET_ATLAS"})
SMODS.Sound({key = "dbz_punch",	path = "dbz_punch.mp3",atlas_table = "ASSET_ATLAS"})
SMODS.Sound({key = "redxiii",	path = "redxiii_2.wav",atlas_table = "ASSET_ATLAS"})
SMODS.Sound({key = "yoshiEat",	path = "yoshi-tongue.mp3",atlas_table = "ASSET_ATLAS"})
--redxiii_2.wav
--dbz_punch.mp3
SMODS.current_mod.optional_features = {retrigger_joker = true}

SMODS.Atlas{ key = "FoxModJokers", path = "foxModJokers.png", px = 71, py = 95, atlas_table = "ASSET_ATLAS" }
SMODS.Atlas{ key = "FoxModBoosters", path = "boosters.png", px = 71, py = 95, atlas_table = "ASSET_ATLAS" }
SMODS.Atlas{ key = "FoxModVouchers", path = "vouchers.png", px = 71, py = 95, atlas_table = "ASSET_ATLAS" }
SMODS.Atlas{ key = "FoxModMisc", path = "misc.png", px = 71, py = 95, atlas_table = "ASSET_ATLAS" }
SMODS.Atlas{ key = "FoxMod2xOnly", path = "2xOnly.png", px = 71, py = 95, atlas_table = "ASSET_ATLAS" }
SMODS.Atlas{ key = "FoxMod2xOnlyBooster", path = "fox_mod_booster_2x_only.png", px = 71, py = 95, atlas_table = "ASSET_ATLAS" }
SMODS.Atlas{ key = "FoxModEpicJokers", path = "FoxModEpicJokers.png", px = 71, py = 95, atlas_table = "ASSET_ATLAS" }

SMODS.Shader({ key = 'glimmer', path = 'glimmer3.fs' })
SMODS.Shader({ key = 'etherOverdrive', path = 'etherOverdrive.fs' })
SMODS.Shader({ key = 'akashic', path = 'akashic.fs' })
-- SMODS.Shader({ key = 'secretRare', path = 'secretRare_new.fs' })
-- SMODS.Shader({ key = 'secretRare_subtle', path = 'secretRare_4.fs' })

SMODS.Shader({ key = 'secretRare', path = 'secretRare_4.fs' })

local lovely_toml_info = NFS.getInfo(SMODS.current_mod.path .. "lovely.toml")
local lovely_dir_items = NFS.getInfo(SMODS.current_mod.path .. "lovely") and NFS.getDirectoryItems(SMODS.current_mod.path .. "lovely")
local should_have_lovely = lovely_toml_info or (lovely_dir_items and #lovely_dir_items > 0)
if should_have_lovely then
    -- if we have detected a `lovely.toml` file or a non-empty `lovely` directory (assumption that it contains lovely patches)
    assert(SMODS.current_mod.lovely, "Failed to detect NeatoJokers lovely patches.\nMake sure your neato-jokers-mod folder is not nested (there should be a bunch of files in the neato-jokers-mod folder and not just another folder).\n\n\n")
end

sendInfoMessage("Loading all subfiles", "FoxMods-config.lua")

--loads individual lua files 
local subdir = "src"
local cards = NFS.getDirectoryItems(SMODS.current_mod.path .. subdir)
for _, filename in pairs(cards) do
    assert(SMODS.load_file(subdir .. "/" .. filename))()
end

sendInfoMessage("finished loading all subfiles", "FoxMods-config.lua")
sendInfoMessage("finished processing", "FoxMods-config.lua")

