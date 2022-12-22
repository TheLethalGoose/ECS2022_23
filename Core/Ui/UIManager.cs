﻿using System.Linq;
using ECS2022_23.Core.Entities.Characters;
using ECS2022_23.Core.Entities.Items;
using Microsoft.Xna.Framework.Graphics;

namespace ECS2022_23.Core.Ui;

public static class UiManager
{
    public static UiPanel StatsPanel { get; set; }
    private static int _preHeartCount;
    private static bool _statsHaveChanged;

    public static void Update(Player player)
    {
        _statsHaveChanged = false;

        UpdateStats(player);
        if (_statsHaveChanged)
        {
            StatsPanel.Update();
        }
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        StatsPanel.Draw(spriteBatch);
    }
    private static void UpdateHearts(Player player)
    {
        var heartCount = (int) player.HP;
        if (heartCount == _preHeartCount) return;
        var index = StatsPanel.GetIndexFromLabel(UiLabels.HpIcon);
        if (index < 0) return;

        if (heartCount > 0)
        {
          
            var change = heartCount - _preHeartCount;

            if (change > 0)
            {
                for (int i = 1; i <= change; i++)
                {
                    StatsPanel.InsertAtIndex(UiLoader.CreateHeart(), index+i);
                }
            } else if (change < 0)
            {
                change *= -1;
                for (int i = 1; i <= change; i++)
                {
                    StatsPanel.RemoveAtIndex(index+i,UiLabels.Heart);
                }
            }
            _preHeartCount = heartCount;
        } else 
        {
            _preHeartCount = 0;
            StatsPanel.RemoveAll( component => component.UiLabel == UiLabels.Heart);
        }

        _statsHaveChanged = true;
    }

    private static void UpdateText(UiPanel panel, UiLabels label, float stats)
    {
        UiText uiText = (UiText) panel.GetComponentByLabel(label);
        if (uiText == null) return;
        if (uiText.Text == $"{stats:0.##}") return;
       
        uiText.Text = stats <= 0 ? "0" : $"{stats:0.##}";
        uiText.SourceRec.Width = (int) uiText.Font.MeasureString(uiText.Text).X;
        uiText.SourceRec.Height = (int) uiText.Font.MeasureString(uiText.Text).Y;
        _statsHaveChanged = true;
    }

    private static void UpdateStats(Player player)
    {
        UpdateHearts(player);
        UpdateText(StatsPanel, UiLabels.CoinText, player.Money);
        UpdateText(StatsPanel, UiLabels.XpText, player.XpToNextLevel);
        UpdateText(StatsPanel, UiLabels.ArmorText, player.Armor);
    }
}