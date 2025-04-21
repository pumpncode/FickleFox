local function poll_custom_editions()
    local editions = {
        "e_Fox_fragileRelic",
        "e_Fox_ghostRare",
        "e_Fox_secretRare",
        "e_Fox_akashic",
        "e_Fox_goldRare"
    }
    return editions[math.random(#editions)]
end

local function poll_with_custom_editions()
    local randNo = math.random(2)
    if randNo > 1 then
        local edition = poll_edition('aura', nil, true, true)
        if nil == edition then return poll_custom_editions() end
        return edition
    else
        local edition = poll_custom_editions()
        return edition
    end
end

-- SMODS.Back{
--     name = "Absolute Deck",
--     key = "absolute",
--     pos = {x = 0, y = 3},
--     config = {polyglass = true},
--     loc_txt = {
--         name = "Absolute Deck",
--         text ={
--             "Start with a Deck",
--             "full of {C:attention,T:e_polychrome}Poly{}{C:red,T:m_glass}glass{} cards"
--         },
--     },
--     apply = function()
--         G.E_MANAGER:add_event(Event({
--             func = function()
--                 for i = #G.playing_cards, 1, -1 do
--                     G.playing_cards[i]:set_ability(G.P_CENTERS.m_glass)
--                     G.playing_cards[i]:set_edition({
--                         polychrome = true
--                     }, true, true)
--                 end
--                 return true
--             end
--         }))
--     end
-- }

SMODS.Back{--ghost rare
    name = "Ghost Rare Deck",
    key = "GhostRareDeck",
    atlas = "FoxModDecks",
    pos = {x = 0, y = 0},
    config = {polyglass = true},
    loc_txt = {
        name = "Ghost Rare Deck",
        text ={
            "Start with a Deck",
            "full of {C:attention,T:e_Fox_ghostRare}Ghost Rare{} cards"
        },
    },
    apply = function(self)    
    G.E_MANAGER:add_event(Event({
        func = function()
            SMODS.add_card({key = "j_hiker", edition = "e_Fox_secretRare"})
            if G.jokers then                
                local card = create_card("Joker", G.jokers, nil, nil, true, true, nil, "foxsecretRare")
                card:set_edition("e_Fox_ghostRare", true, true)
                card:add_to_deck()
                card:start_materialize()
                G.jokers:emplace(card)
                return true
            end
        end,
    }))
        G.E_MANAGER:add_event(Event({
            func = function()
                for i = #G.playing_cards, 1, -1 do
                    -- G.playing_cards[i]:set_ability(G.P_CENTERS.m_glass)
                    -- ("e_Fox_ghostRare", true)
                    G.playing_cards[i]:set_edition("e_Fox_ghostRare", true, true)
                end
                return true
            end
        }))
    end,
    init = function(self)
    SMODS.Edition:take_ownership("e_Fox_ghostRare", {
        get_weight = function(self)
            return 95
        end,
    }, true)
end
}

SMODS.Back{ --secret rare
    name = "Secret Rare Deck --secret rare deck",
    key = "secretRare",
    atlas = "FoxModDecks",
    pos = {x = 1, y = 0},
    config = {polyglass = true},
    loc_txt = {
        name = "Secret Rare Deck",
        text ={
            "Start with a Deck",
            "full of {C:attention,T:e_Fox_secretRare}Secret Rare{} cards",
            "and two random supporting jokers"
        },
    },
    apply = function(self)    
    G.E_MANAGER:add_event(Event({
        func = function()
            if G.jokers then
                SMODS.add_card({key = "j_hiker", edition = "e_Fox_secretRare"})
                local card = create_card("Joker", G.jokers, nil, nil, true, true, nil, "foxsecretRare")
                card:set_edition("e_Fox_secretRare", true, true)
                card:add_to_deck()
                card:start_materialize()
                G.jokers:emplace(card)
                return true
            end
        end,
    }))
        G.E_MANAGER:add_event(Event({
            func = function()
                for i = #G.playing_cards, 1, -1 do
                    -- G.playing_cards[i]:set_ability(G.P_CENTERS.m_glass)
                    -- ("e_Fox_ghostRare", true)
                    G.playing_cards[i]:set_edition("e_Fox_secretRare", true, true)
                end
                return true
            end
        }))
    end,
    init = function(self)
    SMODS.Edition:take_ownership("e_Fox_secretRare", {
        get_weight = function(self)
            return 90
        end,
    }, true)
end
}

SMODS.Back{--akashic
    name = "Akashic Deck",
    key = "akashic",
    atlas = "FoxModDecks",
    pos = {x = 2, y = 0},    
    config = {polyglass = true},
    loc_txt = {
        name = "Akashic Deck",
        text ={
            "Start with a Deck",
            "full of {C:attention,T:e_Fox_akashic}Akashic{} cards",
            "and a random starter joker"
        },
    },
    apply = function(self)    
    G.E_MANAGER:add_event(Event({
        func = function()
            if G.jokers then                
                local card = create_card("Joker", G.jokers, nil, nil, true, true, nil, "foxakashic")
                card:set_edition("e_Fox_akashic", true, true)
                card:add_to_deck()
                card:start_materialize()
                G.jokers:emplace(card)
                return true
            end
        end,
    }))
        G.E_MANAGER:add_event(Event({
            func = function()
                for i = #G.playing_cards, 1, -1 do
                    -- G.playing_cards[i]:set_ability(G.P_CENTERS.m_glass)
                    -- ("e_Fox_ghostRare", true)
                    G.playing_cards[i]:set_edition("e_Fox_akashic", true, true)
                end
                return true
            end
        }))
    end,
    init = function(self)
    SMODS.Edition:take_ownership("e_Fox_akashic", {
        get_weight = function(self)
            return 90
        end,
    }, true)
end
}



SMODS.Back{--Maximum Gold Rare
    name = "Maximum Gold Deck",
    key = "maximumGold",
    atlas = "FoxModDecks",
    pos = {x = 4, y = 0},    
    config = {polyglass = true},
    loc_txt = {
        name = "Maximum Gold Deck",
        text ={
            "Start with a Deck",
            "full of {C:attention,T:m_gold}Gold{} cards",
            "and matching jokers"
        },
    },
    apply = function(self)    
    G.E_MANAGER:add_event(Event({
        func = function()
            if G.jokers then
                --spawn eldlich and gold joker 
                SMODS.add_card({key = "j_Fox_LordOfGold"}) --, edition = "e_Fox_secretRare"})
                SMODS.add_card({key = "j_golden"}) --
                SMODS.add_card({key = "j_ticket"}) --
                SMODS.add_card({key = "j_Fox_goldretriever"}) --
                
                return true
            end
        end,
    }))
        G.E_MANAGER:add_event(Event({
            func = function()
                for i = #G.playing_cards, 1, -1 do
                    G.playing_cards[i]:set_ability(G.P_CENTERS.m_gold, nil, true)
                end
                return true
            end
        }))
    end,
    init = function(self)
    SMODS.Edition:take_ownership("m_gold", {
        get_weight = function(self)
            return 90
        end,
    }, true)
end
}


SMODS.Back{--Maximum Gold Rare
    name = "Maximum Grass Deck",
    key = "maximumGrass",
    atlas = "FoxModDecks",
    pos = {x = 3, y = 0},    
    config = {polyglass = true},
    loc_txt = {
        name = "Maximum Grass Deck",
        text ={
            "Start with a Deck",
            "full of {C:attention,T:m_Fox_grass}Grass{} cards",
            "and matching jokers"
        },
    },
    apply = function(self)    
    G.E_MANAGER:add_event(Event({
        func = function()
            if G.jokers then                
                SMODS.add_card({key = "j_Fox_mammorest"}) 
                SMODS.add_card({key = "j_Fox_gumoss"})                
                return true
            end
        end,
    }))
        G.E_MANAGER:add_event(Event({
            func = function()
                for i = #G.playing_cards, 1, -1 do
                    -- G.playing_cards[i]:set_ability(G.P_CENTERS.m_glass) 
                    -- ("e_Fox_ghostRare", true)
                    G.playing_cards[i]:set_ability("m_Fox_grass", true, true)
                end
                return true
            end
        }))
    end,
    init = function(self)
    SMODS.Edition:take_ownership("m_Fox_grass", {
        get_weight = function(self)
            return 90
        end,
    }, true)
end
}

SMODS.Back{--Gold Rare
    name = "Gold Rare Deck",
    key = "goldDeck",
    atlas = "FoxModDecks",
    pos = {x = 2, y = 0},    
    config = {polyglass = true},
    loc_txt = {
        name = "Gold Rare Deck",
        text ={
            "Start with a Deck",
            "full of {C:attention,T:e_Fox_goldRare}Gold Rare{} cards"
        },
    },
    apply = function(self)    
    G.E_MANAGER:add_event(Event({
        func = function()
            if G.jokers then
                --spawn eldlich and gold joker 
                SMODS.add_card({key = "j_Fox_LordOfGold"}) --, edition = "e_Fox_secretRare"})
                SMODS.add_card({key = "j_golden"}) --
                SMODS.add_card({key = "j_ticket"}) --
                SMODS.add_card({key = "j_Fox_goldretriever"}) --
                
                return true
            end
        end,
    }))
        G.E_MANAGER:add_event(Event({
            func = function()
                for i = #G.playing_cards, 1, -1 do
                    -- G.playing_cards[i]:set_ability(G.P_CENTERS.m_glass)
                    -- ("e_Fox_ghostRare", true)
                    G.playing_cards[i]:set_edition("e_Fox_goldRare", true, true)
                end
                return true
            end
        }))
    end,
    init = function(self)
    SMODS.Edition:take_ownership("e_Fox_goldRare", {
        get_weight = function(self)
            return 90
        end,
    }, true)
end
}


SMODS.Back{
    name = "Akashic Deck",
    key = "akashic",
    atlas = "FoxModDecks",
    pos = {x = 2, y = 0},    
    config = {polyglass = true},
    loc_txt = {
        name = "Akashic Deck",
        text ={
            "Start with a Deck",
            "full of {C:attention,T:e_polychrome}Akashic{} cards"
        },
    },
    apply = function(self)    
    G.E_MANAGER:add_event(Event({
        func = function()
            if G.jokers then
                
                local card = create_card("Joker", G.jokers, nil, nil, true, true, nil, "foxakashic")
                card:set_edition("e_Fox_akashic", true, true)
                card:add_to_deck()
                card:start_materialize()
                G.jokers:emplace(card)
                return true
            end
        end,
    }))
        G.E_MANAGER:add_event(Event({
            func = function()
                for i = #G.playing_cards, 1, -1 do
                    -- G.playing_cards[i]:set_ability(G.P_CENTERS.m_glass)
                    -- ("e_Fox_ghostRare", true)
                    G.playing_cards[i]:set_edition("e_Fox_akashic", true, true)
                end
                return true
            end
        }))
    end,
    init = function(self)
    SMODS.Edition:take_ownership("e_Fox_akashic", {
        get_weight = function(self)
            return 90
        end,
    }, true)
end
}


SMODS.Back{--ghost rare
    name = "Rarity Collection Deck",
    key = "RarityCollectionDeck",
    atlas = "FoxModDecks",
    pos = {x = 0, y = 0},    
    loc_txt = {
        name = "Rarity Collection Deck",
        text ={
            "Start with a Deck",
            "full of {C:dark_edition}Special Edition{} cards"
        },
    },
    apply = function(self)    
    G.E_MANAGER:add_event(Event({
        func = function()
            SMODS.add_card({key = "j_hiker", edition = "e_Fox_secretRare"})
            if G.jokers then                
                local card = create_card("Joker", G.jokers, nil, nil, true, true, nil, "foxsecretRare")
                card:set_edition("e_Fox_ghostRare", true, true)
                card:add_to_deck()
                card:start_materialize()
                G.jokers:emplace(card)
                return true
            end
        end,
    }))
        G.E_MANAGER:add_event(Event({
            func = function()
                for i = #G.playing_cards, 1, -1 do
                    -- G.playing_cards[i]:set_ability(G.P_CENTERS.m_glass)
                    -- ("e_Fox_ghostRare", true)
                    local edition = poll_with_custom_editions()
                       
                    
                    G.playing_cards[i]:set_edition(edition, true, true)
                end
                return true
            end
        }))
    end,
    init = function(self)
    SMODS.Edition:take_ownership("e_Fox_ghostRare", {
        get_weight = function(self)
            return 95
        end,
    }, true)
end
}


-- apply = function(self)
--     G.GAME.modifiers.cry_negative_rate = self.config.cry_negative_rate
--     G.E_MANAGER:add_event(Event({
--         func = function()
--             if G.jokers then
--                 local card = create_card("Joker", G.jokers, nil, "cry_exotic", nil, nil, nil, "cry_wormhole")
--                 card:add_to_deck()
--                 card:start_materialize()
--                 G.jokers:emplace(card)
--                 return true
--             end
--         end,
--     }))
-- end,
-- init = function(self)
--     SMODS.Edition:take_ownership("negative", {
--         get_weight = function(self)
--             return self.weight * (G.GAME.modifiers.cry_negative_rate or 1)
--         end,
--     }, true)
--ghost rare
--secretRare
--etheroverdrive


    -- shader = 'etherOverdrive',
    -- config = {
    --     x_mult =