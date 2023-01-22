#region File Description

//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion File Description

#region Using Statements

using System;
using Comora;
using ECS2022_23.Core.Entities.Characters;
using ECS2022_23.Core.Entities.Items;
using ECS2022_23.Core.Loader;
using ECS2022_23.Core.Manager;
using ECS2022_23.Core.Manager.ScreenManager;
using ECS2022_23.Core.World;
using ECS2022_23.Enums;
using ECS2022_23.Helper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Action = ECS2022_23.Enums.Action;

#endregion Using Statements

namespace ECS2022_23.Core.Screens;

/// <summary>
///     This screen implements the actual game logic. It is just a
///     placeholder to get the idea across: you'll probably want to
///     put some more interesting gameplay in here!
/// </summary>
internal class GameplayScreen : GameScreen
{
    #region Fields

    private ContentManager content;
    
    private Player _player;
    private Escape _escape;
    private Camera _camera;

    private GameSave _gameSave;

    private float pauseAlpha;
    private readonly Timer _timer;

    #endregion Fields

    #region Initialization

    /// <summary>
    ///     Constructor.
    /// </summary>
    public GameplayScreen()
    {
        ScreenMusic = SoundLoader.Background;
        TransitionOnTime = TimeSpan.FromSeconds(1.5);
        TransitionOffTime = TimeSpan.FromSeconds(0.5);
        _timer = new Timer(1.5f);
    }

    /// <summary>
    ///     Load graphics content for the game.
    /// </summary>
    public override void LoadContent()
    {
        if (content == null)
            content = new ContentManager(ScreenManager.Game.Services, "Content");

        WorldLoader.Load(content);
        AnimationLoader.Load(content);
        ItemLoader.Load(content);
        UiLoader.Load(content, ScreenManager.GraphicsDevice);

        _gameSave = Serialization.Load<GameSave>("GameSave.json");

        if (_gameSave == null)
        {
            _player = new Player(content.Load<Texture2D>("sprites/astro"), AnimationLoader.CreatePlayerAnimations())
            {
                Weapon = (Weapon) ItemLoader.CreateItem(Vector2.Zero, ItemType.Sword)
            };
            _gameSave = new GameSave(_player.EP, _player.Level);
            LockerManager.Init(_gameSave.ItemsInLocker);
        }

        if (_gameSave != null)
        {
            _player = new Player(content.Load<Texture2D>("sprites/astro"), AnimationLoader.CreatePlayerAnimations(),
                _gameSave.EP, _gameSave.Level)
            {
                Weapon = (Weapon) ItemLoader.CreateItem(Vector2.Zero, ItemType.Sword)
            };
            LockerManager.Init(_gameSave.ItemsInLocker);
        }
        
        _player.EP = 15;
        _player.Armor = 3;
        _player.Level = 5;

        UiLoader.InitializeUi(_player.MaxHP);
        InventoryManager.Init(_player);

        _camera = new Camera(ScreenManager.GraphicsDevice)
        {
            Zoom = 3f
        };

        _escape = new Escape(_player, 3, 1);
        _escape.AttachCamera(_camera);
        // once the load has finished, we use ResetElapsedTime to tell the game's
        // timing mechanism that we have just finished a very long frame, and that
        // it should not try to catch up.
        ScreenManager.Game.ResetElapsedTime();
    }

    /// <summary>
    ///     Unload graphics content used by the game.
    /// </summary>
    public override void UnloadContent()
    {
        WorldLoader.Unload(content);
        content.Unload();
    }

    #endregion Initialization

    #region Update and Draw

    /// <summary>
    ///     Updates the state of the game. This method checks the GameScreen.IsActive
    ///     property, so the game will stop updating when the pause menu is active,
    ///     or if you tab away to a different application.
    /// </summary>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus,
        bool coveredByOtherScreen)
    {
        base.Update(gameTime, otherScreenHasFocus, false);

        if (otherScreenHasFocus || coveredByOtherScreen || _escape.WasSuccessful || _escape.Failed)
            Serialization.Save(_gameSave);

        // Gradually fade in or out depending on whether we are covered by the pause screen.
        if (coveredByOtherScreen)
            pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
        else
            pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

        if (IsActive)
        {
            _escape.Update(gameTime);
            _gameSave.Update(_player.EP, _player.Level);

            if (_escape.WasSuccessful)
                LoadingScreen.Load(ScreenManager, false, null,
                    new BackgroundScreen(true, true), new GameOverScreen());
            if (_escape.Failed)
            {
                _timer.Update(gameTime);
                if (_timer.LimitReached())
                    LoadingScreen.Load(ScreenManager, false, null,
                        new BackgroundScreen(true, false), new GameOverScreen(_player.DeathCause));
            }

            UiManager.Update(_player);
            CombatManager.Update(gameTime, _player);
        }
    }

    /// <summary>
    ///     Lets the game respond to player input. Unlike the Update method,
    ///     this will only be called when the gameplay screen is active.
    /// </summary>
    public override void HandleInput(InputState input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));

        // Look up inputs for the active player profile.
        var playerIndex = (int) ControllingPlayer.Value;


        if (input.IsPauseGame(ControllingPlayer))
        {
            ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
        }
        else
        {
            if (!_player.IsAlive()) return;

            Input.Update(input, playerIndex);

            var action = Input.GetPlayerAction();

            _player.Moves(Input.GetMovementDirection());
            _player.Aims(Input.GetAimDirection());

            if (action == Action.Attacks) _player.Attack();
            if (action == Action.PicksUpItem) ItemManager.PickItemUp(_player);
            if (action == Action.OpensLocker)
                if (_player.Stage.PlayerIsInfrontOfLocker)
                    ScreenManager.AddScreen(new LockerMenuScreen(), ControllingPlayer);
            if (action == Action.UseItem) InventoryManager.UseItemAtIndex(Input.ToolbarKeyDownIndex());
        }
    }

    /// <summary>
    ///     Draws the gameplay screen.
    /// </summary>
    public override void Draw(GameTime gameTime)
    {
        var spriteBatch = ScreenManager.SpriteBatch;

        spriteBatch.Begin(_camera, samplerState: SamplerState.PointClamp);
        _escape.Draw(spriteBatch);
        CombatManager.Draw(spriteBatch);
        ItemManager.Draw(spriteBatch);
        spriteBatch.End();

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        UiManager.Draw(spriteBatch);
        InventoryManager.Draw(spriteBatch);
        spriteBatch.End();

        // If the game is transitioning on or off, fade it out to black.
        if (TransitionPosition > 0 || pauseAlpha > 0)
        {
            var alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

            ScreenManager.FadeBackBufferToBlack(alpha);
        }
    }

    #endregion Update and Draw
}