SMODS.current_mod.description_loc_vars = function()
    return { background_colour = G.C.CLEAR, text_colour = G.C.WHITE, scale = 1.2 }
end

SMODS.current_mod.custom_ui = function(modNodes)    

    G.Fox_description_area = CardArea(
        G.ROOM.T.x + 0.2 * G.ROOM.T.w / 2, G.ROOM.T.h,
        4.25 * G.CARD_W,
        0.95 * G.CARD_H,
        { card_limit = 5, type = 'title', highlight_limit = 0, collection = true }
    )
    G.Fox_description_area.demo_area = true
    for i, key in ipairs({ "j_Fox_ficklefox", "j_Fox_benevolence" }) do
        local card = Card(G.Fox_description_area.T.x + G.Fox_description_area.T.w / 2, G.Fox_description_area.T.y,
            G.CARD_W, G.CARD_H, G.P_CARDS.empty,
            G.P_CENTERS[key])
        card.children.back = Sprite(card.T.x, card.T.y, card.T.w, card.T.h, G.ASSET_ATLAS["joy_Back"], { x = 0, y = 0 })
        card.children.back.states.hover = card.states.hover
        card.children.back.states.click = card.states.click
        card.children.back.states.drag = card.states.drag
        card.children.back.states.collide.can = false
        card.children.back:set_role({ major = card, role_type = 'Glued', draw_major = card })
        G.Fox_description_area:emplace(card)
        card:flip()
        G.E_MANAGER:add_event(Event({
            blocking = false,
            trigger = "after",
            delay = 0.4 * i,
            func = function()
                play_sound("card1")
                card:flip()
                return true
            end,
        }))
    end

    modNodes[#modNodes + 1] = {
        n = G.UIT.R,
        config = { align = "cm", padding = 0.07, no_fill = true },
        nodes = {
            { n = G.UIT.O, config = { object = G.Fox_description_area } }
        }
    }

    modNodes[#modNodes + 1] = {
        n = G.UIT.R,
        config = {
            padding = 0.2,
            align = "cm",
        },
        nodes = {
            {
                n = G.UIT.C,
                config = {
                    padding = 0.2,
                    align = "cm",
                },
                nodes = {
                    UIBox_button({
                        colour = G.C.GREEN,
                        minw = 3.85,
                        button = "Fox_discord",
                        label = { localize('k_Fox_discord') }
                    })
                }
            },
            {
                n = G.UIT.C,
                config = {
                    padding = 0.2,
                    align = "cm",
                },
                nodes = {
                    UIBox_button({
                        colour = G.C.RED,
                        minw = 3.85,
                        button = "Fox_github",
                        label = { localize('k_Fox_github') }
                    })
                }
            },
        }
    }
end

function G.FUNCS.Fox_discord(e)
    love.system.openURL("https://discord.com/channels/1116389027176787968/1343279563563597854")
end

function G.FUNCS.Fox_github(e)
    love.system.openURL("https://github.com/1RedOne/FickleFox")
end

SMODS.current_mod.extra_tabs = function()
    return {
        {
            label = "Credits",
            tab_definition_function = function()
                local modNodes = {}

                modNodes[#modNodes + 1] = {}
                local loc_vars = { background_colour = G.C.CLEAR, text_colour = G.C.WHITE, scale = 1.4 }
                localize { type = 'descriptions', key = "config_credits", set = 'Other', nodes = modNodes[#modNodes], vars = loc_vars.vars, scale = loc_vars.scale, text_colour = loc_vars.text_colour, shadow = loc_vars.shadow }
                modNodes[#modNodes] = desc_from_rows(modNodes[#modNodes])
                modNodes[#modNodes].config.colour = loc_vars.background_colour or modNodes[#modNodes].config.colour

                return {
                    n = G.UIT.ROOT,
                    config = {
                        emboss = 0.05,
                        minh = 6,
                        r = 0.1,
                        minw = 6,
                        align = "tm",
                        padding = 0.2,
                        colour = G.C.BLACK
                    },
                    nodes = modNodes
                }
            end
        },
    }
end