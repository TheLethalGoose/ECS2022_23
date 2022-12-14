using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS2022_23.Core.Ui;

public class UiPanel : Component
{
    private List<Component> _components;
    private Texture2D _texture2D;
    private int _scale = 2;
    public UiPanel(Rectangle sourceRec, Rectangle destRec, UiLabel uiLabel) : base(sourceRec)
    {
        DestinationRec = destRec;
        DestinationRec.Height *= _scale;
        DestinationRec.Width *= _scale;
        if (DestinationRec.Y != 0)
        {
            DestinationRec.Y -= (_scale-1) * 16;
        }
        _components = new List<Component>();
        UiLabel = uiLabel;
    }
    
    public UiPanel(Rectangle sourceRec, Rectangle destRec, Texture2D texture2D, UiLabel uiLabel) : base(sourceRec)
    {
        DestinationRec = destRec;
        _components = new List<Component>();
        _texture2D = texture2D;
        UiLabel = uiLabel;
    }

    public void AddTexture(Texture2D texture)
    {
        _texture2D = texture;
    }
    
    public void Add(Component component)
    {
        _components.Add(component);
        SetScaling(component);
        SetPositions();
    }

    public void Remove(Component component)
    {
        _components.Remove(component);
    }

    public void Update()
    {
        SetPositions();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (_texture2D != null)
        {
            spriteBatch.Draw(_texture2D, DestinationRec, SourceRec, Color.White);
        }
        
        foreach (var component in _components)
        {
            component.Draw(spriteBatch);
        }
    }

    private void SetPositions()
    {
        if (_components.Count <= 0)
        {
            return;
        }

        int preWidth = 0;
        foreach (var component in _components)
        {
            component.DestinationRec.X = DestinationRec.X + preWidth;
            component.DestinationRec.Y = DestinationRec.Y;
            preWidth += component.SourceRec.Width*_scale;
            SetLength(component);
        }
    }

    private void SetScaling(Component component)
    {
        component.DestinationRec = DestinationRec;
        if (component.GetType() != typeof(UiText)) return;
        var uiText = (UiText)component;
        uiText.Scale = new Vector2(_scale, _scale);
    }

    private void SetLength(Component component)
    {
        component.DestinationRec.Width = component.SourceRec.Width*_scale;
        component.DestinationRec.Height = component.SourceRec.Height*_scale;
    }

    public void InsertAtIndex(Component component, int index)
    {
        if (index < 0) return;
        _components.Insert(index, component);
        SetScaling(component);
        SetPositions();
    }

    public bool RemoveAtIndex(int index, UiLabel componentUiLabel)
    {
        if (index < 0 || index > _components.Count) return false;
        
        try
        {
            var component = _components[index];

            if (component.UiLabel == componentUiLabel)
            {
                _components.RemoveAt(index);
                SetPositions();
                return true;
            }
            
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.WriteLine(e.Message);
        }

        return false;
    }

    public int GetIndexFromLabel(UiLabel uiLabel)
    {
        var index = 0;
        foreach (var component in _components)
        {
            
            if (component.UiLabel == uiLabel)
            {
                return index;
            }

            index++;
        }
        return -1;
    }

    public void RemoveAll(Predicate<Component> cPredicate)
    {
        _components.RemoveAll(cPredicate);
        SetPositions();
    }

    public Component GetComponentAtIndex(int index)
    {
        if(index >= 0 && index < _components.Count)
        {
            return _components[index];
        }
        return null;
    }

    public Component GetComponentByLabel(UiLabel label)
    {
        foreach (var component in _components)
        {
            if (component.UiLabel == label)
            {
                return component;
            }
        }

        return null;
    }
    public Component Find(Predicate<Component> cPredicate)
    {
        return _components.Find(cPredicate);
    }
    
    public int GetIndexFromComponent(Component component)
    {
        return _components.IndexOf(component);
    }
}