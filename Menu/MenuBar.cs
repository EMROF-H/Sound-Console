using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using SoundConsole;
using Audio;
using SoundConsole.Pages.OpenPage;
using SoundConsole.Pages.FilterPage;
using SoundConsole.Pages.WavePage;
using SoundConsole.Pages.FrequencyPage;

namespace Menu
{
    public static class WindowParameter
    {
        #region 容量
        public const int ButtonCount = 10;
        #endregion

        #region 尺寸
        public const int Default_Width = 40;
        public const int Unfold_Width = 160;
        #endregion

        #region 颜色
        public static Color MainColor = Color.FromRgb(0x03, 0xA9, 0xF4);
        public static Color BackColor = Color.FromRgb(0xB3, 0xE5, 0xFC);
        public static Color IconColor = Color.FromRgb(0xFF, 0xFF, 0xFF);
        public static Color FontColor = MainColor.Darken();
        public static Color ChooseColor = FontColor.Darken();

        public static Brush MainBrush = new SolidColorBrush(MainColor);
        public static Brush BackBrush = new SolidColorBrush(BackColor);
        public static Brush IconBrush = new SolidColorBrush(IconColor);
        public static Brush FontBrush = new SolidColorBrush(FontColor);
        public static Brush ChooseBrush = new SolidColorBrush(ChooseColor);
        #endregion
    }

    public class MenuBar
    {
        public MainWindow mainWindow;

        private Rectangle menuRectangle;
        private Grid menuGrid;

        public MenuButton[] menuButtons;
        public Binding widthBinding;

        public bool MenuUnfold { get; private set; } = false;

        public MenuBar(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.menuRectangle = mainWindow.MenuRectangle;
            this.menuGrid = mainWindow.MenuGrid;

            #region 宽度绑定初始化
            widthBinding = new Binding();
            widthBinding.Source = menuRectangle;
            widthBinding.Path = new PropertyPath("Width");
            #endregion

            #region 菜单栏配置
            Histogram histogram = new Histogram(1 << 17);
            Player player = new Player(histogram);
            WavePage wavePage = new WavePage();
            menuButtons = new MenuButton[WindowParameter.ButtonCount]
            {
                /*[0]*/new MenuMainButton(this, PackIconKind.FormatListBulletedSquare, "Menu"),

                /*[1]*/new MusicMenuButton(this, player),
                /*[2]*/null,
                /*[3]*/null,
                /*[4]*/null,
                /*[5]*/null,
                /*[6]*/new IconMenuButton(this, PackIconKind.PlaylistMusicOutline,  "Open",         new OpenPage(player)),
                /*[7]*/new IconMenuButton(this, PackIconKind.Gear,                  "Filter",       new FilterPage(player)),
                /*[8]*/new IconMenuButton(this, PackIconKind.SineWave,              "Waveform",     wavePage),
                /*[9]*/new IconMenuButton(this, PackIconKind.ChartHistogram,        "Frequency",    new FrequencyPage(histogram))
            };
            ChangePage(6); // 初始菜单
            player.SetMusicMenuButton(menuButtons[1] as MusicMenuButton);
            player.SetWavePage(wavePage);
            #endregion

            #region 菜单栏初始化
            for (int i = 0; i < WindowParameter.ButtonCount; i++)
            {
                if (this.menuButtons[i] == null) { continue; }
                this.menuGrid.Children.Add(this.menuButtons[i]);
                this.menuButtons[i].SetValue(Grid.RowProperty, i);
                this.menuButtons[i].SetValue(Grid.ColumnProperty, 0);
            }
            #endregion
        }

        #region 换页
        public void ChangePage(int pageIndex) { (this.menuButtons[pageIndex] as PageMenuButton).ChangePage(); }
        #endregion

        #region 菜单栏展开/关闭
        public void MenuChangeState(bool State)
        {
            if (this.MenuUnfold ^ State)
            {
                MenuChangeState();
            }
        }

        public void MenuChangeState()
        {
            DoubleAnimation MenuAnimation = new DoubleAnimation();

            if (MenuUnfold)
            {
                MenuAnimation.From = WindowParameter.Unfold_Width;
                MenuAnimation.To = WindowParameter.Default_Width;
            }
            else
            {
                MenuAnimation.From = WindowParameter.Default_Width;
                MenuAnimation.To = WindowParameter.Unfold_Width;
            }

            MenuAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            MenuAnimation.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };

            Storyboard MenuStoryboard = new Storyboard();
            MenuStoryboard.Children.Add(MenuAnimation);
            Storyboard.SetTargetName(MenuAnimation, this.menuRectangle.Name);

            Storyboard.SetTargetProperty(MenuAnimation, new PropertyPath(Rectangle.WidthProperty));

            MenuStoryboard.Begin(this.mainWindow);

            MenuUnfold = !MenuUnfold;
        }
        #endregion
    }
}
