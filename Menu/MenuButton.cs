using Audio;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Menu
{
    public class MenuButton : Button
    {
        protected Grid buttonGrid;
        public TextBlock textBlock;

        protected MenuBar menuBar;

        public MenuButton(MenuBar menuBar, string text)
        {
            this.menuBar = menuBar;

            this.Background = WindowParameter.MainBrush;
            this.BorderBrush = WindowParameter.MainBrush;
            this.SetValue(FocusVisualStyleProperty, null);

            this.Height = 40;
            this.SetBinding(Button.WidthProperty, menuBar.widthBinding);

            this.buttonGrid = new Grid();
            this.AddChild(this.buttonGrid);
            this.buttonGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8) });
            this.buttonGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20) });
            this.buttonGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20) });
            this.buttonGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(112) });

            this.textBlock = new TextBlock();
            this.buttonGrid.Children.Add(this.textBlock);
            this.textBlock.Text = text;
            this.textBlock.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            this.textBlock.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            this.textBlock.Foreground = WindowParameter.IconBrush;
            this.textBlock.SetValue(Grid.ColumnProperty, 3);
        }
    }

    public class PageMenuButton : MenuButton
    {
        private ContentControl pageContent;
        public Frame frame;

        public PageMenuButton(MenuBar menuBar, string text, Page page = null) : base(menuBar, text)
        {
            if (page != null){ this.frame = new Frame() { Content = page }; }

            this.pageContent = menuBar.mainWindow.PageContent;

            this.Click += MenuButton_Click;
        }

        public void ChangeButtonColor(bool state)
        {
            this.textBlock.Foreground = state ? WindowParameter.ChooseBrush : WindowParameter.IconBrush;
        }

        public void ChangePage()
        {
            this.pageContent.Content = this.frame;

            foreach (MenuButton menuButton in menuBar.menuButtons)
            {
                if (menuButton != null && menuButton is PageMenuButton)
                {
                    ((PageMenuButton)menuButton).ChangeButtonColor(menuButton == this);
                }
            }

            menuBar.MenuChangeState(false);
        }

        protected virtual void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage();
        }
    }

    public class IconMenuButton : PageMenuButton
    {
        protected PackIcon icon;
        private Binding foreGroundBinding;

        public IconMenuButton(MenuBar menuBar, PackIconKind kind, string text, Page page = null) : base(menuBar, text, page)
        {
            this.icon = new PackIcon();
            this.foreGroundBinding = new Binding();

            this.icon.Kind = kind;
            this.icon.Width = 20;
            this.icon.Height = 20;
            this.icon.HorizontalAlignment = HorizontalAlignment.Left;
            this.foreGroundBinding.Source = base.textBlock;
            this.foreGroundBinding.Path = new PropertyPath("Foreground");
            this.icon.SetBinding(PackIcon.ForegroundProperty, foreGroundBinding);

            base.buttonGrid.Children.Add(this.icon);
            this.icon.SetValue(Grid.ColumnProperty, 1);
        }
    }

    public class MenuMainButton : IconMenuButton
    {
        public MenuMainButton(MenuBar menuBar, PackIconKind kind, string text) : base(menuBar, kind, text) { }

        protected override void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            menuBar.MenuChangeState();
            ChangeButtonColor(menuBar.MenuUnfold);
        }
    }

    public class StateMenuButton : IconMenuButton
    {
        public enum State
        {
            Disconnected,
            Connected,
        };

        private static Color DisconnectedColor = Color.FromRgb(0xFF, 0x30, 0x40);
        private static Color ConnectedColor = Color.FromRgb(0x00, 0xFF, 0x00);

        private static Brush DisconnectedBrush = new SolidColorBrush(DisconnectedColor);
        private static Brush ConnectedBrush = new SolidColorBrush(ConnectedColor);

        public StateMenuButton(MenuBar menuBar, string text, Page page = null) : base(menuBar, PackIconKind.AccessPointOff, text, page)
        {
            ChangeState(State.Disconnected);
        }

        public void ChangeState(State state)
        {
            switch (state)
            {
                case State.Disconnected:
                {
                    base.icon.Kind = PackIconKind.PauseCircleOutline;
                    base.icon.Foreground = DisconnectedBrush;
                    break;
                }
                case State.Connected:
                {
                    base.icon.Kind = PackIconKind.PlayCircleOutline;
                    base.icon.Foreground = ConnectedBrush;
                    break;
                }
            }
        }
    }

    public class MusicMenuButton : MenuButton
    {
        private const PackIconKind PlayIcon  = PackIconKind.PlayCircleOutline;
        private const PackIconKind PauseIcon = PackIconKind.PauseCircleOutline;

        private PackIcon icon;
        private Binding foreGroundBinding;

        public Player player;
        public bool playState { get; private set; }

        public MusicMenuButton(MenuBar menuBar, Player player) : base(menuBar, "Control")
        {
            this.player = player;
            this.playState = false;

            this.Click += MusicMenuButton_Click;

            this.icon = new PackIcon();
            this.foreGroundBinding = new Binding();

            this.icon.Kind = PackIconKind.PlayCircleOutline;
            this.icon.Width = 20;
            this.icon.Height = 20;
            this.icon.HorizontalAlignment = HorizontalAlignment.Left;
            this.foreGroundBinding.Source = this.textBlock;
            this.foreGroundBinding.Path = new PropertyPath("Foreground");
            this.icon.SetBinding(PackIcon.ForegroundProperty, foreGroundBinding);

            this.buttonGrid.Children.Add(this.icon);
            this.icon.SetValue(Grid.ColumnProperty, 1);
        }

        protected virtual void MusicMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if(playState)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        public void Play()
        {
            this.player.Play();
            this.icon.Kind = PauseIcon;
            playState = true;
        }

        public void Pause()
        {
            this.player.Pause();
            this.icon.Kind = PlayIcon;
            playState = false;
        }
    }
}
