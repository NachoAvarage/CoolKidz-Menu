using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CoolKidz_Menu
{
    public partial class Form1 : Form
    {
        private readonly Color MenuColor = Color.FromArgb(89, 89, 89);
        private readonly Color SidebarColor = Color.FromArgb(80, 80, 80);
        private readonly Color CardColor = Color.FromArgb(98, 98, 98);
        private readonly Color ButtonColor = Color.FromArgb(108, 108, 108);
        private readonly Color HoverColor = Color.FromArgb(125, 125, 125);

        private Panel sidebar = null!;
        private Panel content = null!;

        private CrosshairOverlay? crosshair;

        private int crosshairSize = 12;
        private int crosshairThickness = 3;
        private int crosshairGap = 5;

        private Color crosshairColor = Color.White;

        private bool crosshairEnabled = false;
        private bool centerDot = false;
        private bool outline = true;

        private TrackBar sizeSlider = null!;
        private TrackBar thicknessSlider = null!;
        private TrackBar gapSlider = null!;

        private Label sizeValue = null!;
        private Label thicknessValue = null!;
        private Label gapValue = null!;

        private Button colorButton = null!;
        private Button enableButton = null!;
        private Button dotButton = null!;
        private Button outlineButton = null!;

        private ComboBox monitorSelector = null!;

        public Form1()
        {
            InitializeComponent();
            BuildMenu();
        }

        private void BuildMenu()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(900, 600);
            BackColor = MenuColor;
            DoubleBuffered = true;

            ApplyRoundedCorners(this, 22);

            Controls.Clear();

            sidebar = new Panel
            {
                Location = new Point(15, 15),
                Size = new Size(210, 570),
                BackColor = SidebarColor
            };

            ApplyRoundedCorners(sidebar, 18);
            Controls.Add(sidebar);

            Label logo = new Label
            {
                Text = "CoolKidz",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, 25),
                AutoSize = true
            };

            sidebar.Controls.Add(logo);

            Label logoSub = new Label
            {
                Text = "GAMING UTILITIES",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.FromArgb(205, 205, 205),
                Location = new Point(27, 62),
                AutoSize = true
            };

            sidebar.Controls.Add(logoSub);

            AddNavigationButton("Home", 110, ShowHome);
            AddNavigationButton("Crosshair", 165, ShowCrosshair);
            AddNavigationButton("Settings", 220, ShowSettings);

            Label version = new Label
            {
                Text = "CoolKidz Menu v1.0",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(175, 175, 175),
                Location = new Point(25, 535),
                AutoSize = true
            };

            sidebar.Controls.Add(version);

            content = new Panel
            {
                Location = new Point(245, 15),
                Size = new Size(640, 570),
                BackColor = MenuColor
            };

            Controls.Add(content);

            Button minimize = new Button
            {
                Text = "—",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 35),
                Location = new Point(535, 10),
                Cursor = Cursors.Hand
            };

            minimize.FlatAppearance.BorderSize = 0;

            minimize.Click += (s, e) =>
            {
                WindowState = FormWindowState.Minimized;
            };

            content.Controls.Add(minimize);

            Button close = new Button
            {
                Text = "×",
                Font = new Font("Segoe UI", 18),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(40, 35),
                Location = new Point(580, 8),
                Cursor = Cursors.Hand
            };

            close.FlatAppearance.BorderSize = 0;

            close.Click += (s, e) =>
            {
                crosshair?.Close();
                Close();
            };

            content.Controls.Add(close);

            MouseDown += DragWindow;
            sidebar.MouseDown += DragWindow;
            content.MouseDown += DragWindow;

            ShowHome();
        }

        private void AddNavigationButton(
            string text,
            int y,
            EventHandler click)
        {
            Button button = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.White,
                BackColor = ButtonColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(170, 45),
                Location = new Point(20, y),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            button.FlatAppearance.BorderSize = 0;

            button.MouseEnter += (s, e) =>
            {
                button.BackColor = HoverColor;
            };

            button.MouseLeave += (s, e) =>
            {
                button.BackColor = ButtonColor;
            };

            button.Click += click;

            ApplyRoundedCorners(button, 12);

            sidebar.Controls.Add(button);
        }

        private void ClearContent()
        {
            for (int i = content.Controls.Count - 1; i >= 0; i--)
            {
                Control control = content.Controls[i];

                if (control.Tag?.ToString() != "windowButton")
                {
                    content.Controls.RemoveAt(i);
                    control.Dispose();
                }
            }
        }

        private void ShowHome(object? sender = null, EventArgs? e = null)
        {
            ClearContent();

            AddHeading(
                "Welcome to CoolKidz",
                "Gaming utilities made simple."
            );

            AddInfoCard(
                "Gaming Utilities",
                "Useful tools that work alongside your games.",
                25,
                175
            );

            AddInfoCard(
                "Custom Crosshair",
                "Create your own crosshair and use it in any game.",
                25,
                285
            );

            AddInfoCard(
                "More Coming Soon",
                "FPS counter, timers, HUD tools and more.",
                25,
                395
            );
        }

        private void ShowCrosshair(
            object? sender = null,
            EventArgs? e = null)
        {
            ClearContent();

            AddHeading(
                "Custom Crosshair",
                "A standalone crosshair overlay for any game."
            );

            AddSlider(
                "Size",
                15,
                1,
                40,
                crosshairSize,
                out sizeSlider,
                out sizeValue,
                value =>
                {
                    crosshairSize = value;
                    sizeValue.Text = value.ToString();
                    UpdateCrosshair();
                });

            AddSlider(
                "Thickness",
                75,
                1,
                10,
                crosshairThickness,
                out thicknessSlider,
                out thicknessValue,
                value =>
                {
                    crosshairThickness = value;
                    thicknessValue.Text = value.ToString();
                    UpdateCrosshair();
                });

            AddSlider(
                "Gap",
                135,
                0,
                30,
                crosshairGap,
                out gapSlider,
                out gapValue,
                value =>
                {
                    crosshairGap = value;
                    gapValue.Text = value.ToString();
                    UpdateCrosshair();
                });

            Label colorLabel = new Label
            {
                Text = "Color",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, 205),
                AutoSize = true
            };

            content.Controls.Add(colorLabel);

            colorButton = new Button
            {
                Text = "Choose Color",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = crosshairColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(140, 38),
                Location = new Point(25, 230),
                Cursor = Cursors.Hand
            };

            colorButton.FlatAppearance.BorderSize = 0;
            ApplyRoundedCorners(colorButton, 10);

            colorButton.Click += ChooseCrosshairColor;

            content.Controls.Add(colorButton);

            Label monitorLabel = new Label
            {
                Text = "Monitor",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(195, 205),
                AutoSize = true
            };

            content.Controls.Add(monitorLabel);

            monitorSelector = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(105, 105, 105),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(195, 230),
                Size = new Size(170, 38)
            };

            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                monitorSelector.Items.Add($"Monitor {i + 1}");
            }

            if (monitorSelector.Items.Count > 0)
            {
                monitorSelector.SelectedIndex = 0;
            }

            monitorSelector.SelectedIndexChanged += (s, e) =>
            {
                UpdateCrosshair();
            };

            content.Controls.Add(monitorSelector);

            dotButton = CreateToggleButton(
                "Center Dot",
                centerDot,
                25,
                290
            );

            dotButton.Click += (s, e) =>
            {
                centerDot = !centerDot;

                UpdateToggleButton(
                    dotButton,
                    "Center Dot",
                    centerDot);

                UpdateCrosshair();
            };

            content.Controls.Add(dotButton);

            outlineButton = CreateToggleButton(
                "Outline",
                outline,
                185,
                290
            );

            outlineButton.Click += (s, e) =>
            {
                outline = !outline;

                UpdateToggleButton(
                    outlineButton,
                    "Outline",
                    outline);

                UpdateCrosshair();
            };

            content.Controls.Add(outlineButton);

            enableButton = new Button
            {
                Text = crosshairEnabled
                    ? "Crosshair Enabled"
                    : "Enable Crosshair",

                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold),

                ForeColor = Color.White,

                BackColor = crosshairEnabled
                    ? Color.FromArgb(70, 140, 90)
                    : ButtonColor,

                FlatStyle = FlatStyle.Flat,

                Size = new Size(250, 48),

                Location = new Point(25, 355),

                Cursor = Cursors.Hand
            };

            enableButton.FlatAppearance.BorderSize = 0;

            ApplyRoundedCorners(enableButton, 12);

            enableButton.Click += (s, e) =>
            {
                crosshairEnabled = !crosshairEnabled;

                UpdateEnableButton();

                if (crosshairEnabled)
                {
                    ShowCrosshairOverlay();
                }
                else
                {
                    HideCrosshairOverlay();
                }
            };

            content.Controls.Add(enableButton);

            Label hotkey = new Label
            {
                Text = "Hotkey: F8",
                Font = new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold),

                ForeColor = Color.FromArgb(
                    205,
                    205,
                    205),

                Location = new Point(25, 420),

                AutoSize = true
            };

            content.Controls.Add(hotkey);

            Label hotkeyDescription = new Label
            {
                Text = "Press F8 anywhere to toggle the crosshair.",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(175, 175, 175),
                Location = new Point(25, 445),
                AutoSize = true
            };

            content.Controls.Add(hotkeyDescription);

            if (crosshairEnabled)
            {
                ShowCrosshairOverlay();
            }
        }

        // FIXED VERSION
        // This avoids using an 'out' parameter inside a lambda.
        private void AddSlider(
            string name,
            int y,
            int minimum,
            int maximum,
            int value,
            out TrackBar slider,
            out Label valueLabel,
            Action<int> changed)
        {
            Label label = new Label
            {
                Text = name,
                Font = new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, y),
                AutoSize = true
            };

            content.Controls.Add(label);

            valueLabel = new Label
            {
                Text = value.ToString(),
                Font = new Font(
                    "Segoe UI",
                    9),
                ForeColor = Color.FromArgb(
                    210,
                    210,
                    210),
                Location = new Point(120, y),
                AutoSize = true
            };

            content.Controls.Add(valueLabel);

            TrackBar newSlider = new TrackBar
            {
                Minimum = minimum,
                Maximum = maximum,
                Value = Math.Max(
                    minimum,
                    Math.Min(maximum, value)),
                Location = new Point(155, y - 7),
                Size = new Size(300, 45),
                TickStyle = TickStyle.None
            };

            newSlider.ValueChanged += (s, e) =>
            {
                changed(newSlider.Value);
            };

            content.Controls.Add(newSlider);

            slider = newSlider;
        }

        private void ChooseCrosshairColor(
            object? sender,
            EventArgs e)
        {
            using ColorDialog dialog = new ColorDialog
            {
                Color = crosshairColor,
                FullOpen = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                crosshairColor = dialog.Color;

                colorButton.BackColor =
                    crosshairColor;

                UpdateCrosshair();
            }
        }

        private Button CreateToggleButton(
            string text,
            bool enabled,
            int x,
            int y)
        {
            Button button = new Button
            {
                Text = enabled
                    ? text + ": ON"
                    : text + ": OFF",

                Font = new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold),

                ForeColor = Color.White,

                BackColor = enabled
                    ? Color.FromArgb(70, 140, 90)
                    : ButtonColor,

                FlatStyle = FlatStyle.Flat,

                Size = new Size(140, 42),

                Location = new Point(x, y),

                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;

            ApplyRoundedCorners(button, 10);

            return button;
        }

        private void UpdateToggleButton(
            Button button,
            string text,
            bool enabled)
        {
            button.Text = enabled
                ? text + ": ON"
                : text + ": OFF";

            button.BackColor = enabled
                ? Color.FromArgb(70, 140, 90)
                : ButtonColor;
        }

        private void UpdateEnableButton()
        {
            if (enableButton == null)
                return;

            enableButton.Text = crosshairEnabled
                ? "Crosshair Enabled"
                : "Enable Crosshair";

            enableButton.BackColor = crosshairEnabled
                ? Color.FromArgb(70, 140, 90)
                : ButtonColor;
        }

        private void ShowCrosshairOverlay()
        {
            if (crosshair == null ||
                crosshair.IsDisposed)
            {
                crosshair = new CrosshairOverlay();
            }

            UpdateCrosshair();

            crosshair.Show();
        }

        private void HideCrosshairOverlay()
        {
            if (crosshair != null &&
                !crosshair.IsDisposed)
            {
                crosshair.Hide();
            }
        }

        private void UpdateCrosshair()
        {
            if (crosshair == null ||
                crosshair.IsDisposed)
                return;

            Screen screen =
                GetSelectedScreen();

            crosshair.UpdateCrosshair(
                screen,
                crosshairSize,
                crosshairThickness,
                crosshairGap,
                crosshairColor,
                centerDot,
                outline);
        }

        private Screen GetSelectedScreen()
        {
            int index = 0;

            if (monitorSelector != null &&
                monitorSelector.SelectedIndex >= 0)
            {
                index =
                    monitorSelector.SelectedIndex;
            }

            if (index >= Screen.AllScreens.Length)
            {
                index = 0;
            }

            return Screen.AllScreens[index];
        }

        private void ShowSettings(
            object? sender = null,
            EventArgs? e = null)
        {
            ClearContent();

            AddHeading(
                "Settings",
                "CoolKidz Menu configuration."
            );

            AddInfoCard(
                "Theme",
                "Solid dark gray • #595959",
                25,
                175
            );

            AddInfoCard(
                "Crosshair",
                "Runs independently as a screen overlay.",
                25,
                285
            );

            AddInfoCard(
                "Hotkey",
                "F8 toggles the custom crosshair.",
                25,
                395
            );
        }

        private void AddHeading(
            string heading,
            string description)
        {
            Label title = new Label
            {
                Text = heading,
                Font = new Font(
                    "Segoe UI",
                    27,
                    FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(25, 70),
                AutoSize = true
            };

            content.Controls.Add(title);

            Label subtitle = new Label
            {
                Text = description,
                Font = new Font(
                    "Segoe UI",
                    11),
                ForeColor = Color.FromArgb(
                    205,
                    205,
                    205),
                Location = new Point(29, 120),
                AutoSize = true
            };

            content.Controls.Add(subtitle);
        }

        private void AddInfoCard(
            string heading,
            string description,
            int x,
            int y)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(590, 90),
                BackColor = CardColor
            };

            ApplyRoundedCorners(card, 16);

            Label cardTitle = new Label
            {
                Text = heading,
                Font = new Font(
                    "Segoe UI",
                    14,
                    FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 18),
                AutoSize = true
            };

            card.Controls.Add(cardTitle);

            Label cardDescription = new Label
            {
                Text = description,
                Font = new Font(
                    "Segoe UI",
                    9),
                ForeColor = Color.FromArgb(
                    205,
                    205,
                    205),
                Location = new Point(20, 48),
                AutoSize = true
            };

            card.Controls.Add(cardDescription);

            content.Controls.Add(card);
        }

        private static void ApplyRoundedCorners(
            Control control,
            int radius)
        {
            void UpdateRegion()
            {
                if (control.Width <= 0 ||
                    control.Height <= 0)
                    return;

                int diameter =
                    Math.Min(
                        radius * 2,
                        Math.Min(
                            control.Width,
                            control.Height));

                GraphicsPath path =
                    new GraphicsPath();

                path.AddArc(
                    0,
                    0,
                    diameter,
                    diameter,
                    180,
                    90);

                path.AddArc(
                    control.Width - diameter,
                    0,
                    diameter,
                    diameter,
                    270,
                    90);

                path.AddArc(
                    control.Width - diameter,
                    control.Height - diameter,
                    diameter,
                    diameter,
                    0,
                    90);

                path.AddArc(
                    0,
                    control.Height - diameter,
                    diameter,
                    diameter,
                    90,
                    90);

                path.CloseFigure();

                control.Region =
                    new Region(path);

                path.Dispose();
            }

            UpdateRegion();

            control.Resize += (s, e) =>
            {
                UpdateRegion();
            };
        }

        private void DragWindow(
            object? sender,
            MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();

                NativeMethods.SendMessage(
                    Handle,
                    NativeMethods.WM_NCLBUTTONDOWN,
                    NativeMethods.HTCAPTION,
                    0);
            }
        }

        protected override bool ProcessCmdKey(
            ref Message msg,
            Keys keyData)
        {
            if (keyData == Keys.F8)
            {
                crosshairEnabled =
                    !crosshairEnabled;

                if (crosshairEnabled)
                {
                    ShowCrosshairOverlay();
                }
                else
                {
                    HideCrosshairOverlay();
                }

                UpdateEnableButton();

                return true;
            }

            return base.ProcessCmdKey(
                ref msg,
                keyData);
        }

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            crosshair?.Close();

            base.OnFormClosed(e);
        }

        private static class NativeMethods
        {
            public const int WM_NCLBUTTONDOWN =
                0xA1;

            public const int HTCAPTION =
                0x2;

            public const int GWL_EXSTYLE =
                -20;

            public const int WS_EX_LAYERED =
                0x80000;

            public const int WS_EX_TRANSPARENT =
                0x20;

            public const int WS_EX_TOOLWINDOW =
                0x80;

            [DllImport("user32.dll")]
            public static extern bool ReleaseCapture();

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(
                IntPtr hWnd,
                int Msg,
                int wParam,
                int lParam);

            [DllImport(
                "user32.dll",
                SetLastError = true)]
            public static extern int GetWindowLong(
                IntPtr hWnd,
                int nIndex);

            [DllImport(
                "user32.dll",
                SetLastError = true)]
            public static extern int SetWindowLong(
                IntPtr hWnd,
                int nIndex,
                int dwNewLong);
        }

        // ============================================================
        // CROSSHAIR OVERLAY
        // ============================================================

        private class CrosshairOverlay : Form
        {
            private int size = 12;
            private int thickness = 3;
            private int gap = 5;

            private Color color = Color.White;

            private bool dot;
            private bool drawOutline;

            public CrosshairOverlay()
            {
                FormBorderStyle =
                    FormBorderStyle.None;

                ShowInTaskbar = false;

                TopMost = true;

                BackColor = Color.Magenta;

                TransparencyKey =
                    Color.Magenta;

                StartPosition =
                    FormStartPosition.Manual;

                DoubleBuffered = true;

                int extendedStyle =
                    NativeMethods.GetWindowLong(
                        Handle,
                        NativeMethods.GWL_EXSTYLE);

                NativeMethods.SetWindowLong(
                    Handle,
                    NativeMethods.GWL_EXSTYLE,
                    extendedStyle |
                    NativeMethods.WS_EX_LAYERED |
                    NativeMethods.WS_EX_TRANSPARENT |
                    NativeMethods.WS_EX_TOOLWINDOW);
            }

            public void UpdateCrosshair(
                Screen screen,
                int newSize,
                int newThickness,
                int newGap,
                Color newColor,
                bool newDot,
                bool newOutline)
            {
                size = newSize;
                thickness = newThickness;
                gap = newGap;
                color = newColor;
                dot = newDot;
                drawOutline = newOutline;

                Bounds = screen.Bounds;

                TopMost = true;

                Invalidate();
            }

            protected override void OnPaint(
                PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.SmoothingMode =
                    SmoothingMode.AntiAlias;

                int centerX =
                    ClientSize.Width / 2;

                int centerY =
                    ClientSize.Height / 2;

                using SolidBrush brush =
                    new SolidBrush(color);

                using SolidBrush outlineBrush =
                    new SolidBrush(Color.Black);

                DrawLine(
                    e.Graphics,
                    centerX,
                    centerY - gap - size,
                    centerX,
                    centerY - gap,
                    brush,
                    outlineBrush,
                    drawOutline);

                DrawLine(
                    e.Graphics,
                    centerX,
                    centerY + gap,
                    centerX,
                    centerY + gap + size,
                    brush,
                    outlineBrush,
                    drawOutline);

                DrawLine(
                    e.Graphics,
                    centerX - gap - size,
                    centerY,
                    centerX - gap,
                    centerY,
                    brush,
                    outlineBrush,
                    drawOutline);

                DrawLine(
                    e.Graphics,
                    centerX + gap,
                    centerY,
                    centerX + gap + size,
                    centerY,
                    brush,
                    outlineBrush,
                    drawOutline);

                if (dot)
                {
                    int dotSize =
                        Math.Max(
                            thickness + 2,
                            4);

                    int dotX =
                        centerX -
                        dotSize / 2;

                    int dotY =
                        centerY -
                        dotSize / 2;

                    if (drawOutline)
                    {
                        e.Graphics.FillEllipse(
                            outlineBrush,
                            dotX - 1,
                            dotY - 1,
                            dotSize + 2,
                            dotSize + 2);
                    }

                    e.Graphics.FillEllipse(
                        brush,
                        dotX,
                        dotY,
                        dotSize,
                        dotSize);
                }
            }

            private void DrawLine(
                Graphics graphics,
                int x1,
                int y1,
                int x2,
                int y2,
                Brush brush,
                Brush outlineBrush,
                bool useOutline)
            {
                int width =
                    Math.Max(1, thickness);

                int outlineSize =
                    useOutline
                        ? width + 2
                        : width;

                if (x1 == x2)
                {
                    int top =
                        Math.Min(y1, y2);

                    int height =
                        Math.Abs(y2 - y1);

                    if (useOutline)
                    {
                        graphics.FillRectangle(
                            outlineBrush,
                            x1 - outlineSize / 2,
                            top,
                            outlineSize,
                            height);
                    }

                    graphics.FillRectangle(
                        brush,
                        x1 - width / 2,
                        top,
                        width,
                        height);
                }
                else
                {
                    int left =
                        Math.Min(x1, x2);

                    int lineWidth =
                        Math.Abs(x2 - x1);

                    if (useOutline)
                    {
                        graphics.FillRectangle(
                            outlineBrush,
                            left,
                            y1 - outlineSize / 2,
                            lineWidth,
                            outlineSize);
                    }

                    graphics.FillRectangle(
                        brush,
                        left,
                        y1 - width / 2,
                        lineWidth,
                        width);
                }
            }

            protected override bool ShowWithoutActivation
            {
                get
                {
                    return true;
                }
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp =
                        base.CreateParams;

                    cp.ExStyle |=
                        NativeMethods.WS_EX_TOOLWINDOW;

                    cp.ExStyle |=
                        NativeMethods.WS_EX_TRANSPARENT;

                    cp.ExStyle |=
                        NativeMethods.WS_EX_LAYERED;

                    return cp;
                }
            }

            private static class NativeMethods
            {
                public const int GWL_EXSTYLE =
                    -20;

                public const int WS_EX_LAYERED =
                    0x80000;

                public const int WS_EX_TRANSPARENT =
                    0x20;

                public const int WS_EX_TOOLWINDOW =
                    0x80;

                [DllImport(
                    "user32.dll",
                    SetLastError = true)]
                public static extern int GetWindowLong(
                    IntPtr hWnd,
                    int nIndex);

                [DllImport(
                    "user32.dll",
                    SetLastError = true)]
                public static extern int SetWindowLong(
                    IntPtr hWnd,
                    int nIndex,
                    int dwNewLong);
            }
        }
    }
}