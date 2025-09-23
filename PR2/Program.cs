using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

/// <summary>
/// �������� ����� ��������� � ���������� �������� � ������ ����
/// </summary>
public class CalendarForm : Form
{
    // �������� �������� ����������
    private MonthCalendar calendar;              // ��������� ��� ����������� ����
    private Panel leftPanel;                     // ����� ������ ��� ���������
    private Panel rightPanel;                    // ������ ������ ��� ���������� ���������
    private Panel topPanel;                      // ������� ������ � ��������
    private Panel bottomPanel;                   // ������ ������ ��� �������
    private Panel eventFormPanel;                // ������ ����� �������
    private Panel calendarContainer;             // ��������� ��� ���������

    // �������� ����� �������
    private TextBox eventTitleBox;               // ���� ��� �������� �������
    private TextBox eventDescriptionBox;         // ���� ��� �������� �������
    private DateTimePicker eventStartDatePicker; // ����� ���� ������
    private DateTimePicker eventEndDatePicker;   // ����� ���� ���������
    private DateTimePicker eventStartTimePicker; // ����� ������� ������
    private DateTimePicker eventEndTimePicker;   // ����� ������� ���������
    private CheckBox allDayCheckBox;             // ������� "���� ����"
    private ListView eventsListView;             // ������ �������

    // ������ ����������
    private Button addEventButton;               // ������ ���������� �������
    private Button editEventButton;              // ������ �������������� �������
    private Button deleteEventButton;            // ������ �������� �������
    private Button exportButton;                 // ������ ��������
    private Button importButton;                 // ������ �������
    private Button todayButton;                  // ������ "�������"
    private Button viewAllEventsButton;          // ������ ��������� ���� �������
    private Button accentColorButton;            // ������ ����� ����
    private Label statusLabel;                   // ����� �������
    private ComboBox eventTypeCombo;             // ���������� ������ ����� �������

    // ��������� ������
    private readonly Dictionary<DateTime, List<CalendarEvent>> events = new();
    private CalendarEvent selectedEvent = null;  // ��������� ������� ��� ��������������

    // ������� ��������� ������
    private Color currentAccentColor = Color.FromArgb(0, 255, 127);
    private readonly Color[] accentColors = {
        Color.FromArgb(0, 255, 127),   // �������
        Color.FromArgb(0, 174, 255),   // �����
        Color.FromArgb(255, 69, 0),    // ������-���������
        Color.FromArgb(255, 215, 0),   // �������
        Color.FromArgb(186, 85, 211),  // ������-����������
        Color.FromArgb(255, 20, 147),  // �����-�������
        Color.FromArgb(0, 255, 255),   // ����
    };
    private int currentAccentIndex = 0;          // ������ �������� ���������� �����

    // ���������� �������� �����
    private readonly Color backgroundColor = Color.FromArgb(18, 18, 18);      // �������� ������ ���
    private readonly Color surfaceColor = Color.FromArgb(28, 28, 28);         // ������ �����������
    private readonly Color borderColor = Color.FromArgb(45, 45, 45);          // ����� ���� ������
    private readonly Color textColor = Color.FromArgb(220, 220, 220);         // ������-����� �����
    private readonly Color mutedTextColor = Color.FromArgb(150, 150, 150);    // ������������ �����
    private readonly Color buttonColor = Color.FromArgb(40, 40, 40);          // ��� ������

    /// <summary>
    /// ����� ����� � ����������
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new CalendarForm());
    }

    /// <summary>
    /// ����������� ����� - �������������� ��� ����������
    /// </summary>
    public CalendarForm()
    {
        InitializeForm();        // ��������� �������� �����
        InitializePanels();      // �������� �������
        InitializeControls();    // �������� ��������� ����������
        SetupEvents();          // ��������� �������
        LoadSampleEvents();     // �������� �������� �������
    }

    /// <summary>
    /// ������������� �������� ������� �����
    /// </summary>
    private void InitializeForm()
    {
        // ������������� ������ ���� ��� �������������� �������
        Size = new Size(1400, 900);
        MaximumSize = new Size(1400, 900);
        MinimumSize = new Size(1400, 900);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        // ���������� ������ ����
        BackColor = backgroundColor;
        ForeColor = textColor;
        Font = new Font("Consolas", 9f); // ������������ ����� ��� ����������� ����
        Text = "Calendar";
        StartPosition = FormStartPosition.CenterScreen;
    }

    /// <summary>
    /// �������� � ���������� �������� �������
    /// </summary>
    private void InitializePanels()
    {
        // ������� ������ � �������� ����������
        topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = surfaceColor,
            Padding = new Padding(15),
        };
        topPanel.Paint += (s, e) => DrawBorder(e.Graphics, topPanel.ClientRectangle, borderColor);

        // ����� ������ ��� ���������
        leftPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 850,
            BackColor = backgroundColor,
            Padding = new Padding(15),
        };

        // ����������� ��������� ��� ��������� � ����������� ���������
        calendarContainer = new Panel
        {
            Location = new Point(15, 15),
            Size = new Size(820, 780), // ����������� ������ ��� ������� ����������
            BackColor = surfaceColor,
            Padding = new Padding(5), // ����������� ������� ��� ������������� ������������� ������������
        };
        calendarContainer.Paint += (s, e) => DrawBorder(e.Graphics, calendarContainer.ClientRectangle, borderColor);

        // ������ ������ ��� ���������� ���������
        rightPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = backgroundColor,
            Padding = new Padding(15),
        };

        // ������ ������ ��� ����������� �������
        bottomPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 35,
            BackColor = surfaceColor,
        };
        bottomPanel.Paint += (s, e) => DrawBorder(e.Graphics, bottomPanel.ClientRectangle, borderColor);

        // ���������� ���������� ��������� � ����� ������
        leftPanel.Controls.Add(calendarContainer);
        // ���������� ���� ������� � �����
        Controls.AddRange(new Control[] { rightPanel, leftPanel, topPanel, bottomPanel });
    }

    /// <summary>
    /// ������������� ���� ��������� ����������
    /// </summary>
    private void InitializeControls()
    {
        InitializeTopPanelControls();    // �������� ������� ������
        InitializeCalendar();           // ���������
        InitializeRightPanelControls(); // �������� ������ ������
        InitializeStatusBar();          // ������ ���������
    }

    /// <summary>
    /// �������� ��������� ������� ������ (��������� � ������)
    /// </summary>
    private void InitializeTopPanelControls()
    {
        // ��������� ����������
        var titleLabel = new Label
        {
            Text = "CALENDAR",
            Location = new Point(15, 18),
            Size = new Size(150, 25),
            ForeColor = textColor,
            Font = new Font("Consolas", 14f, FontStyle.Bold),
            BackColor = Color.Transparent
        };

        // ������ ������ ��� ���� ������ ������� ������
        var buttonSize = new Size(90, 30);
        var buttonY = 15;

        // �������� ������ � ���������� ������
        todayButton = CreateConsoleButton("TODAY", new Point(200, buttonY), buttonSize, true); // �������� ������
        exportButton = CreateConsoleButton("EXPORT", new Point(300, buttonY), buttonSize);
        importButton = CreateConsoleButton("IMPORT", new Point(400, buttonY), buttonSize);
        viewAllEventsButton = CreateConsoleButton("ALL", new Point(500, buttonY), buttonSize);

        // ������ ���� ������ � ������
        accentColorButton = CreateConsoleButton("THEME", new Point(600, buttonY), buttonSize, true, true);

        // ���������� ���� ��������� � ������� ������
        topPanel.Controls.AddRange(new Control[] {
            titleLabel, todayButton, exportButton, importButton, viewAllEventsButton, accentColorButton
        });
    }

    /// <summary>
    /// ������������� ��������� � ����������� ��������
    /// </summary>
    private void InitializeCalendar()
    {
        // �������� ��������� � ����������� ��� �������� ���������
        calendar = new MonthCalendar
        {
            Location = new Point(5, 5),
            Size = new Size(810, 770), // ����������� ������ ��� ������� ���������� ����������
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,

            // �������������� ���������� ������ ������
            BackColor = surfaceColor,
            ForeColor = textColor,
            TitleBackColor = currentAccentColor,
            TitleForeColor = Color.Black,
            TrailingForeColor = mutedTextColor,

            // ��������� ������ ��� ����������� ������� ���� (3 �������, 4 ������)
            CalendarDimensions = new Size(3, 4), // 3x4 ��� ������� ���������� 12 �������
            FirstDayOfWeek = Day.Sunday,
            ShowToday = true,
            ShowTodayCircle = true,
            ShowWeekNumbers = false,
            MaxSelectionCount = 42, // ��������� ����� ��������� ���
            Font = new Font("Consolas", 8f), // ����������� ����� ��� ���������� 12 �������

            // ����������� ��������� ��� ������� �����
            MinDate = new DateTime(DateTime.Now.Year, 1, 1),
            MaxDate = new DateTime(2030, 12, 31)
        };

        calendarContainer.Controls.Add(calendar);
        calendar.BringToFront(); // �������������� ���������� ��������� ������ ������ ���������
    }

    /// <summary>
    /// �������������� ���������� ���� � ��������� (Windows Forms ��������� �������������� ������ ����)
    /// </summary>
    private void ForceCalendarTheme()
    {
        try
        {
            // �������������� ���������� ������ ���� ��������� ��� ��� ��������
            for (int i = 0; i < 3; i++)
            {
                calendar.BackColor = surfaceColor;
                calendar.ForeColor = textColor;
                calendar.TitleBackColor = currentAccentColor;
                calendar.TitleForeColor = Color.Black;
                calendar.TrailingForeColor = mutedTextColor;
            }

            // �������������� ���������� ����������� ���������
            calendar.Refresh();
            calendar.Invalidate();
            calendar.Update();

            // �������������� ������ ��� �������� ���������� ���� ����� ��������� ��������
            var themeTimer = new System.Windows.Forms.Timer { Interval = 200 };
            themeTimer.Tick += (s, e) => {
                calendar.BackColor = surfaceColor;
                calendar.ForeColor = textColor;
                calendar.TitleBackColor = currentAccentColor;
                calendar.Refresh();
                themeTimer.Stop();
                themeTimer.Dispose();
            };
            themeTimer.Start();
        }
        catch (Exception ex)
        {
            statusLabel.Text = $"THEME APPLICATION ERROR: {ex.Message}";
        }
    }

    /// <summary>
    /// �������� ��������� ���������� � ������ ������
    /// </summary>
    private void InitializeRightPanelControls()
    {
        // ������ ����� ������� � ����������� ������� ��� ������� ���������� ������
        eventFormPanel = new Panel
        {
            Location = new Point(0, 0),
            Size = new Size(500, 370), // ����������� ������ ��� ����������� ����������������
            BackColor = surfaceColor,
            Padding = new Padding(15)
        };
        eventFormPanel.Paint += (s, e) => DrawBorder(e.Graphics, eventFormPanel.ClientRectangle, borderColor);

        // ��������� ������ ���������� ���������
        var eventFormLabel = new Label
        {
            Text = "EVENT MANAGEMENT",
            Location = new Point(15, 15),
            Size = new Size(250, 25),
            ForeColor = textColor,
            Font = new Font("Consolas", 12f, FontStyle.Bold),
            BackColor = Color.Transparent
        };

        // ���� ����� ��� �������� ���������� � �������
        eventTitleBox = CreateConsoleTextBox("Event Title", new Point(15, 50), new Size(450, 25));
        eventDescriptionBox = CreateConsoleTextBox("Description", new Point(15, 85), new Size(450, 50), true);

        // �������� ���������� ���������� ���
        var dateRangeLabel = CreateConsoleLabel("DATE RANGE:", new Point(15, 150));
        eventStartDatePicker = CreateConsoleDatePicker(new Point(15, 175), new Size(130, 25));
        var toLabel = CreateConsoleLabel("TO", new Point(155, 178));
        toLabel.Size = new Size(25, 20);
        toLabel.TextAlign = ContentAlignment.MiddleCenter;
        eventEndDatePicker = CreateConsoleDatePicker(new Point(190, 175), new Size(130, 25));

        // �������� ���������� ��������
        var timeLabel = CreateConsoleLabel("TIME:", new Point(15, 210));
        eventStartTimePicker = CreateConsoleTimePicker(new Point(15, 235), new Size(100, 25));
        eventEndTimePicker = CreateConsoleTimePicker(new Point(125, 235), new Size(100, 25));

        // ������� ��� ������� �� ���� ����
        allDayCheckBox = new CheckBox
        {
            Text = "ALL DAY",
            Location = new Point(235, 237),
            Size = new Size(100, 25),
            ForeColor = textColor,
            BackColor = Color.Transparent,
            Font = new Font("Consolas", 9f),
            FlatStyle = FlatStyle.Flat
        };

        // ���������� ������ ����� �������
        var typeLabel = CreateConsoleLabel("TYPE:", new Point(15, 270));
        eventTypeCombo = CreateConsoleComboBox(new Point(70, 267), new Size(120, 25));
        eventTypeCombo.Items.AddRange(new[] { "MEETING", "APPOINTMENT", "REMINDER", "HOLIDAY", "PERSONAL", "WORK" });
        eventTypeCombo.SelectedIndex = 0;

        // ������ ���������� ��������� � ���������� �����������������
        var buttonSize = new Size(100, 30);
        addEventButton = CreateConsoleButton("ADD", new Point(15, 300), buttonSize, true);
        editEventButton = CreateConsoleButton("UPDATE", new Point(125, 300), buttonSize);
        deleteEventButton = CreateConsoleButton("DELETE", new Point(235, 300), buttonSize);

        editEventButton.Enabled = false; // ��������� �� ��������� �� ������ �������

        // ���������� ���� ��������� � ������ �����
        eventFormPanel.Controls.AddRange(new Control[] {
            eventFormLabel, eventTitleBox, eventDescriptionBox, dateRangeLabel,
            eventStartDatePicker, toLabel, eventEndDatePicker, timeLabel,
            eventStartTimePicker, eventEndTimePicker, allDayCheckBox,
            typeLabel, eventTypeCombo, addEventButton, editEventButton, deleteEventButton
        });

        // ��������� ������ ����������� �������
        var eventsLabel = new Label
        {
            Text = "UPCOMING EVENTS",
            Location = new Point(0, 380), // ���������� ��� ����� ������ ������
            Size = new Size(200, 25),
            ForeColor = textColor,
            Font = new Font("Consolas", 11f, FontStyle.Bold),
            BackColor = Color.Transparent
        };

        // ListView ��� ����������� ������� � ������� �����������
        eventsListView = new ListView
        {
            Location = new Point(0, 410),
            Size = new Size(500, 330),
            BackColor = surfaceColor,
            ForeColor = textColor,
            BorderStyle = BorderStyle.None,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            MultiSelect = false,
            Font = new Font("Consolas", 9f),
            HeaderStyle = ColumnHeaderStyle.Nonclickable,
            OwnerDraw = true // ��������� ��������� ��������� ��� ������ ����������
        };

        // ��������� ������� ListView
        eventsListView.Columns.Add("DATE", 100);
        eventsListView.Columns.Add("TIME", 80);
        eventsListView.Columns.Add("TITLE", 200);
        eventsListView.Columns.Add("TYPE", 80);

        // ��������� ��������� ���������� ListView ��� ���������� ����� �����
        eventsListView.DrawColumnHeader += (s, e) =>
        {
            // ������� ����� ��������� ������ ������ ��� ���������� ����� ����������
            e.Graphics.FillRectangle(new SolidBrush(borderColor), e.Bounds);

            // ��������� ������� ���������
            using (var pen = new Pen(mutedTextColor))
            {
                e.Graphics.DrawRectangle(pen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
            }

            // ��������� ������ ���������
            var textBounds = new Rectangle(e.Bounds.X + 5, e.Bounds.Y + 2, e.Bounds.Width - 10, e.Bounds.Height - 4);
            TextRenderer.DrawText(e.Graphics, e.Header.Text, new Font("Consolas", 9f, FontStyle.Bold),
                textBounds, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        };

        // ������������� ����������� ��������� ��� ��������� ������
        eventsListView.DrawItem += (s, e) => { e.DrawDefault = true; };
        eventsListView.DrawSubItem += (s, e) => { e.DrawDefault = true; };

        // ��������� ����������� ��� ���������� ����� ���������� ��� ���������
        eventsListView.Paint += (s, e) =>
        {
            // ������� ���� ListView ��� ���������� ����� �����
            e.Graphics.FillRectangle(new SolidBrush(surfaceColor), eventsListView.ClientRectangle);
        };

        // ���������� ��������� � ������ ������
        rightPanel.Controls.AddRange(new Control[] { eventFormPanel, eventsLabel, eventsListView });
    }

    /// <summary>
    /// ������������� ������ ���������
    /// </summary>
    private void InitializeStatusBar()
    {
        statusLabel = new Label
        {
            Text = "READY - SELECT DATES TO ADD EVENTS",
            Dock = DockStyle.Fill,
            ForeColor = mutedTextColor,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(15, 0, 0, 0),
            Font = new Font("Consolas", 9f),
            BackColor = Color.Transparent
        };
        bottomPanel.Controls.Add(statusLabel);
    }

    /// <summary>
    /// �������� ������ � ���������� ����� � ���������� ������ ����
    /// </summary>
    /// <param name="text">����� ������</param>
    /// <param name="location">������� ������</param>
    /// <param name="size">������ ������</param>
    /// <param name="isPrimary">�������� �� ������ �������� (� ��������� ������)</param>
    /// <param name="isThemeButton">�������� �� ������ ������� ����� ���� (������ �������)</param>
    private Button CreateConsoleButton(string text, Point location, Size size, bool isPrimary = false, bool isThemeButton = false)
    {
        // ����������� ������ ������ � ����������� �� � ����
        Color buttonBackColor;
        Color buttonForeColor;

        if (isThemeButton)
        {
            // ������ ���� ������ ������������ � ������� ��������� ������
            buttonBackColor = currentAccentColor;
            buttonForeColor = Color.Black;
        }
        else if (isPrimary)
        {
            // �������� ������ ���������� ��������� ����
            buttonBackColor = currentAccentColor;
            buttonForeColor = Color.Black;
        }
        else
        {
            // ������� ������ ���������� ����������� ������ ����
            buttonBackColor = buttonColor;
            buttonForeColor = textColor;
        }

        var button = new Button
        {
            Text = text,
            Location = location,
            Size = size,
            BackColor = buttonBackColor,
            ForeColor = buttonForeColor,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Consolas", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        // ��������� �������� ����� ������
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = borderColor;

        // ������� ��������� ����
        button.MouseEnter += (s, e) =>
        {
            if (isPrimary || isThemeButton)
            {
                // ���������� ���������� ����� ��� ��������� �� �������� ������
                button.BackColor = Color.FromArgb(
                    Math.Min(255, currentAccentColor.R + 30),
                    Math.Min(255, currentAccentColor.G + 30),
                    Math.Min(255, currentAccentColor.B + 30)
                );
            }
            else
            {
                // ���������� ������� ������ ��� ���������
                button.BackColor = Color.FromArgb(60, 60, 60);
            }
        };

        button.MouseLeave += (s, e) =>
        {
            // �������������� ��������� ����� ��� ����� �������
            if (isThemeButton)
            {
                button.BackColor = currentAccentColor;
            }
            else
            {
                button.BackColor = isPrimary ? currentAccentColor : buttonColor;
            }
        };

        return button;
    }

    /// <summary>
    /// �������� ���������� ���� � ���������� ����� � ����������������� placeholder'�
    /// </summary>
    private TextBox CreateConsoleTextBox(string placeholder, Point location, Size size, bool multiline = false)
    {
        var textBox = new TextBox
        {
            Location = location,
            Size = size,
            BackColor = backgroundColor,
            ForeColor = textColor,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font("Consolas", 9f),
            Multiline = multiline
        };

        // ��������� placeholder ������
        textBox.Text = placeholder;
        textBox.ForeColor = mutedTextColor;

        // ��������� ������ ��� ���������� placeholder ����������������
        textBox.Enter += (s, e) =>
        {
            // ������� placeholder ��� ��������� ������
            if (textBox.Text == placeholder)
            {
                textBox.Text = "";
                textBox.ForeColor = textColor;
            }
        };

        textBox.Leave += (s, e) =>
        {
            // �������������� placeholder ��� ������ ������ � ������ ����
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = placeholder;
                textBox.ForeColor = mutedTextColor;
            }
        };

        return textBox;
    }

    /// <summary>
    /// �������� ������ ���� � ���������� �����
    /// </summary>
    private DateTimePicker CreateConsoleDatePicker(Point location, Size size)
    {
        var picker = new DateTimePicker
        {
            Location = location,
            Size = size,
            BackColor = backgroundColor,
            ForeColor = textColor,
            Format = DateTimePickerFormat.Short,
            Font = new Font("Consolas", 9f)
        };

        // ��������� ������ ����������� ���������
        picker.CalendarForeColor = textColor;
        picker.CalendarMonthBackground = surfaceColor;
        picker.CalendarTitleBackColor = currentAccentColor;
        picker.CalendarTitleForeColor = Color.Black;
        picker.CalendarTrailingForeColor = mutedTextColor;

        return picker;
    }

    /// <summary>
    /// �������� ������ ������� � ���������� �����
    /// </summary>
    private DateTimePicker CreateConsoleTimePicker(Point location, Size size)
    {
        return new DateTimePicker
        {
            Location = location,
            Size = size,
            BackColor = backgroundColor,
            ForeColor = textColor,
            Format = DateTimePickerFormat.Time,
            ShowUpDown = true, // ������������� ������ �����/���� ������ ����������� ������
            Font = new Font("Consolas", 9f)
        };
    }

    /// <summary>
    /// �������� ����������� ������ � ���������� �����
    /// </summary>
    private ComboBox CreateConsoleComboBox(Point location, Size size)
    {
        return new ComboBox
        {
            Location = location,
            Size = size,
            BackColor = backgroundColor,
            ForeColor = textColor,
            FlatStyle = FlatStyle.Flat,
            DropDownStyle = ComboBoxStyle.DropDownList, // ������ ����� �� ������, ��� �����
            Font = new Font("Consolas", 9f)
        };
    }

    /// <summary>
    /// �������� ����� � ���������� �����
    /// </summary>
    private Label CreateConsoleLabel(string text, Point location)
    {
        return new Label
        {
            Text = text,
            Location = location,
            Size = new Size(50, 20),
            ForeColor = mutedTextColor,
            Font = new Font("Consolas", 9f, FontStyle.Bold),
            BackColor = Color.Transparent
        };
    }

    /// <summary>
    /// ��������� ������� ������
    /// </summary>
    private void DrawBorder(Graphics g, Rectangle rect, Color color)
    {
        using var pen = new Pen(color, 1);
        g.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
    }

    /// <summary>
    /// ���������� ���������� ����� �� ���� ��������� � �������������� ����������� ���������
    /// </summary>
    private void ApplyAccentColor()
    {
        // ���������� ���������� ����� � �������� �������
        todayButton.BackColor = currentAccentColor;
        addEventButton.BackColor = currentAccentColor;

        // ������ ���� ������ �������� � ��������� ������
        accentColorButton.BackColor = currentAccentColor;
        accentColorButton.ForeColor = Color.Black;

        // ���������� ���������� ����� � ���������
        calendar.TitleBackColor = currentAccentColor;
        calendar.BackColor = surfaceColor;
        calendar.ForeColor = textColor;

        // ���������� � ��������� ������ ����
        eventStartDatePicker.CalendarTitleBackColor = currentAccentColor;
        eventEndDatePicker.CalendarTitleBackColor = currentAccentColor;

        // �������������� ���������� ����������� ���������
        calendar.Refresh();
        calendar.Invalidate();
        Refresh();
    }

    /// <summary>
    /// ��������� ������������ ������� ��� ���� ��������� ����������
    /// </summary>
    private void SetupEvents()
    {
        // ����������� ������� ��������� � �����
        calendar.DateSelected += Calendar_DateSelected;
        addEventButton.Click += AddEvent_Click;
        editEventButton.Click += EditEvent_Click;
        deleteEventButton.Click += DeleteEvent_Click;
        exportButton.Click += Export_Click;
        importButton.Click += Import_Click;
        todayButton.Click += Today_Click;
        viewAllEventsButton.Click += ViewAllEvents_Click;
        accentColorButton.Click += AccentColor_Click;
        eventsListView.SelectedIndexChanged += EventsList_SelectedIndexChanged;
        allDayCheckBox.CheckedChanged += AllDay_CheckedChanged;

    }

    /// <summary>
    /// ���������� ����� ���������� ����� (����������� ������������)
    /// </summary>
    private void AccentColor_Click(object sender, EventArgs e)
    {
        // ����������� ������������ ����� ���������� ���������� �������
        currentAccentIndex = (currentAccentIndex + 1) % accentColors.Length;
        currentAccentColor = accentColors[currentAccentIndex];
        ApplyAccentColor();
        statusLabel.Text = "THEME CHANGED";
    }

    /// <summary>
    /// ���������� ������ ���� � ���������
    /// </summary>
    private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
    {
        // �������������� ���������� ����� ���� � ����� �������
        eventStartDatePicker.Value = e.Start;
        eventEndDatePicker.Value = e.End;

        // ����������� ���������� ��������� � ������ ���������
        if (e.Start.Date == e.End.Date)
        {
            statusLabel.Text = $"SELECTED: {e.Start:MMM dd, yyyy}".ToUpper();
        }
        else
        {
            statusLabel.Text = $"SELECTED RANGE: {e.Start:MMM dd} - {e.End:MMM dd, yyyy}".ToUpper();
        }

        RefreshEventsList(); // ���������� ������ ������� ��� ���������� ���������
    }

    /// <summary>
    /// ���������� ���������� ������ �������
    /// </summary>
    private void AddEvent_Click(object sender, EventArgs e)
    {
        if (!ValidateEventForm()) return; // �������� ���������� ��������� ������

        var startDate = eventStartDatePicker.Value.Date;
        var endDate = eventEndDatePicker.Value.Date;

        // �������� ������� ��� ������� ��� � ��������� ���������
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            // ����������� ������� ������� � ����������� �� ����� "���� ����"
            var eventDateTime = allDayCheckBox.Checked ?
                date :
                date.Add(eventStartTimePicker.Value.TimeOfDay);

            var endDateTime = allDayCheckBox.Checked ?
                date.AddDays(1).AddSeconds(-1) :
                date.Add(eventEndTimePicker.Value.TimeOfDay);

            // �������� ������ ������� �������
            var newEvent = new CalendarEvent
            {
                Title = eventTitleBox.Text,
                Description = eventDescriptionBox.Text == "Description" ? "" : eventDescriptionBox.Text,
                StartDateTime = eventDateTime,
                EndDateTime = endDateTime,
                IsAllDay = allDayCheckBox.Checked,
                Type = eventTypeCombo.SelectedItem.ToString()
            };

            // ���������� ������� � ������� �������
            if (!events.ContainsKey(date))
                events[date] = new List<CalendarEvent>();

            events[date].Add(newEvent);
        }

        // ���������� ���������� ����� ���������� �������
        RefreshCalendarBoldDates();
        RefreshEventsList();
        ClearEventForm();
        statusLabel.Text = $"EVENT ADDED FOR {(endDate - startDate).Days + 1} DAY(S)";
    }

    /// <summary>
    /// ���������� �������������� ���������� �������
    /// </summary>
    private void EditEvent_Click(object sender, EventArgs e)
    {
        if (selectedEvent == null || !ValidateEventForm()) return;

        // ���������� ������� ���������� ������� ������� �� �����
        selectedEvent.Title = eventTitleBox.Text;
        selectedEvent.Description = eventDescriptionBox.Text == "Description" ? "" : eventDescriptionBox.Text;
        selectedEvent.IsAllDay = allDayCheckBox.Checked;
        selectedEvent.Type = eventTypeCombo.SelectedItem.ToString();

        // ���������� ������� �������, ���� ��� �� ������� �� ���� ����
        if (!allDayCheckBox.Checked)
        {
            selectedEvent.StartDateTime = eventStartDatePicker.Value.Date.Add(eventStartTimePicker.Value.TimeOfDay);
            selectedEvent.EndDateTime = eventEndDatePicker.Value.Date.Add(eventEndTimePicker.Value.TimeOfDay);
        }

        // ���������� ����������
        RefreshEventsList();
        ClearEventForm();
        statusLabel.Text = "EVENT UPDATED SUCCESSFULLY";
    }

    /// <summary>
    /// ���������� �������� ���������� �������
    /// </summary>
    private void DeleteEvent_Click(object sender, EventArgs e)
    {
        if (selectedEvent == null) return;

        // ������������� �������� � ������������
        var result = MessageBox.Show("DELETE THIS EVENT?",
            "CONFIRM DELETE", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            var eventDate = selectedEvent.StartDateTime.Date;
            if (events.ContainsKey(eventDate))
            {
                events[eventDate].Remove(selectedEvent);
                // �������� ���� �� �������, ���� ������ ��� ������� �� ��� ����
                if (!events[eventDate].Any())
                {
                    events.Remove(eventDate);
                }
            }

            // ���������� ���������� ����� ��������
            RefreshCalendarBoldDates();
            RefreshEventsList();
            ClearEventForm();
            statusLabel.Text = "EVENT DELETED";
        }
    }

    /// <summary>
    /// ���������� �������� ������� � JSON ����
    /// </summary>
    private void Export_Click(object sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            DefaultExt = "json",
            FileName = $"calendar_export_{DateTime.Now:yyyyMMdd}.json"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                // ���������� ������ ��� �������� � ������� ��� JSON �������
                var exportData = events.SelectMany(kvp =>
                    kvp.Value.Select(evt => new
                    {
                        StartDate = evt.StartDateTime.ToString("yyyy-MM-dd"),
                        StartTime = evt.StartDateTime.ToString("HH:mm:ss"),
                        EndDate = evt.EndDateTime.ToString("yyyy-MM-dd"),
                        EndTime = evt.EndDateTime.ToString("HH:mm:ss"),
                        Title = evt.Title,
                        Description = evt.Description,
                        Type = evt.Type,
                        IsAllDay = evt.IsAllDay
                    })).ToList();

                // ������������ � JSON � �������� ��������������� � ���������� � ����
                var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(dialog.FileName, json);

                statusLabel.Text = $"EXPORTED {exportData.Count} EVENTS";
                MessageBox.Show("EVENTS EXPORTED SUCCESSFULLY!", "EXPORT COMPLETE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"EXPORT FAILED: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// ���������� ������� ������� �� JSON �����
    /// </summary>
    private void Import_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            DefaultExt = "json"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                // ������ � �������������� JSON �����
                var json = File.ReadAllText(dialog.FileName);
                var importData = JsonSerializer.Deserialize<JsonElement[]>(json);

                int importedCount = 0;
                foreach (var item in importData)
                {
                    // ���������� ������ ������� �� JSON ��������
                    var dateStr = item.GetProperty("StartDate").GetString();
                    var timeStr = item.GetProperty("StartTime").GetString();
                    var endDateStr = item.GetProperty("EndDate").GetString();
                    var endTimeStr = item.GetProperty("EndTime").GetString();
                    var title = item.GetProperty("Title").GetString();
                    var description = item.GetProperty("Description").GetString();
                    var type = item.GetProperty("Type").GetString();
                    var isAllDay = item.GetProperty("IsAllDay").GetBoolean();

                    // ������� ��� � �������, �������� ������� ��� �������� ��������
                    if (DateTime.TryParse($"{dateStr} {timeStr}", out var startDateTime) &&
                        DateTime.TryParse($"{endDateStr} {endTimeStr}", out var endDateTime))
                    {
                        var newEvent = new CalendarEvent
                        {
                            Title = title,
                            Description = description,
                            StartDateTime = startDateTime,
                            EndDateTime = endDateTime,
                            IsAllDay = isAllDay,
                            Type = type
                        };

                        // ���������� ������� � �������
                        var eventDate = startDateTime.Date;
                        if (!events.ContainsKey(eventDate))
                            events[eventDate] = new List<CalendarEvent>();

                        events[eventDate].Add(newEvent);
                        importedCount++;
                    }
                }

                // ���������� ���������� ����� �������
                RefreshCalendarBoldDates();
                RefreshEventsList();
                statusLabel.Text = $"IMPORTED {importedCount} EVENTS";
                MessageBox.Show($"SUCCESSFULLY IMPORTED {importedCount} EVENTS!", "IMPORT COMPLETE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"IMPORT FAILED: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// ������� � ����������� ���� � ���������
    /// </summary>
    private void Today_Click(object sender, EventArgs e)
    {
        calendar.SetDate(DateTime.Today);
        RefreshEventsList();
        statusLabel.Text = "NAVIGATED TO TODAY";
    }

    /// <summary>
    /// ����������� ���� �� ����� ���������
    /// </summary>
    private void ViewAllEvents_Click(object sender, EventArgs e)
    {
        var allEventsForm = new AllEventsForm(events, currentAccentColor, backgroundColor, surfaceColor, borderColor, textColor, mutedTextColor);
        allEventsForm.ShowDialog();
    }

    /// <summary>
    /// ���������� ������ ������� � ������ ��� ��������������
    /// </summary>
    private void EventsList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (eventsListView.SelectedItems.Count > 0)
        {
            // ���������� ����� ������� ���������� �������
            var item = eventsListView.SelectedItems[0];
            selectedEvent = (CalendarEvent)item.Tag;

            eventTitleBox.Text = selectedEvent.Title;
            eventTitleBox.ForeColor = textColor;
            eventDescriptionBox.Text = selectedEvent.Description;
            eventDescriptionBox.ForeColor = textColor;
            eventStartDatePicker.Value = selectedEvent.StartDateTime.Date;
            eventEndDatePicker.Value = selectedEvent.EndDateTime.Date;
            eventStartTimePicker.Value = selectedEvent.StartDateTime;
            eventEndTimePicker.Value = selectedEvent.EndDateTime;
            allDayCheckBox.Checked = selectedEvent.IsAllDay;
            eventTypeCombo.SelectedItem = selectedEvent.Type;

            // ��������� ������ ��������������
            editEventButton.Enabled = true;
            addEventButton.Text = "ADD NEW";
        }
        else
        {
            // ������� ������ � ������� � ������ ����������
            selectedEvent = null;
            editEventButton.Enabled = false;
            addEventButton.Text = "ADD";
        }
    }

    /// <summary>
    /// ���������� ��������� �������� "���� ����"
    /// </summary>
    private void AllDay_CheckedChanged(object sender, EventArgs e)
    {
        // ����������/��������� ��������� ������ ������� � ����������� �� ��������� ��������
        eventStartTimePicker.Enabled = !allDayCheckBox.Checked;
        eventEndTimePicker.Enabled = !allDayCheckBox.Checked;
    }

    /// <summary>
    /// ��������� ������ ����� ������� ����� �����������
    /// </summary>
    private bool ValidateEventForm()
    {
        // �������� ���������� ������������� ���� �������� �������
        if (eventTitleBox.Text == "Event Title" || string.IsNullOrWhiteSpace(eventTitleBox.Text))
        {
            MessageBox.Show("PLEASE ENTER AN EVENT TITLE.", "VALIDATION ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        // �������� ������������ ��������� ��� (����� �� ������ ������)
        if (eventEndDatePicker.Value < eventStartDatePicker.Value)
        {
            MessageBox.Show("END DATE CANNOT BE BEFORE START DATE.", "VALIDATION ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    /// <summary>
    /// ���������� ������ ������� ��� ���������� ��������� ���
    /// </summary>
    private void RefreshEventsList()
    {
        eventsListView.Items.Clear();

        var startDate = calendar.SelectionStart.Date;
        var endDate = calendar.SelectionEnd.Date;

        // ���� ���� ������� �� ���������� ��������� ���
        var relevantEvents = new List<CalendarEvent>();

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (events.ContainsKey(date))
            {
                relevantEvents.AddRange(events[date]);
            }
        }

        // ���������� ������� � ListView � ����������� �� ������� ������
        foreach (var evt in relevantEvents.OrderBy(e => e.StartDateTime))
        {
            var item = new ListViewItem(evt.StartDateTime.ToString("MMM dd").ToUpper());
            item.SubItems.Add(evt.IsAllDay ? "ALL DAY" : evt.StartDateTime.ToString("HH:mm"));
            item.SubItems.Add(evt.Title.ToUpper());
            item.SubItems.Add(evt.Type);
            item.Tag = evt; // ���������� ������ �� ������� ��� ������������ ��������������
            item.BackColor = surfaceColor;
            item.ForeColor = textColor;
            eventsListView.Items.Add(item);
        }
    }

    /// <summary>
    /// ���������� ���������� ��� � ��������� (������ ������� ��� ��� � ���������)
    /// </summary>
    private void RefreshCalendarBoldDates()
    {
        // ��������� ���� ���, �� ������� ������������� �������
        calendar.BoldedDates = events.Keys.ToArray();
        calendar.UpdateBoldedDates();
    }

    /// <summary>
    /// ������� ����� ������� � ������� � ��������� �� ���������
    /// </summary>
    private void ClearEventForm()
    {
        // ����� ���� ����� ����� � placeholder ���������
        eventTitleBox.Text = "Event Title";
        eventTitleBox.ForeColor = mutedTextColor;
        eventDescriptionBox.Text = "Description";
        eventDescriptionBox.ForeColor = mutedTextColor;
        eventStartTimePicker.Value = DateTime.Now;
        eventEndTimePicker.Value = DateTime.Now.AddHours(1); // ����� ��������� �� ��������� ����� ���
        allDayCheckBox.Checked = false;
        eventTypeCombo.SelectedIndex = 0;
        selectedEvent = null;
        editEventButton.Enabled = false;
        addEventButton.Text = "ADD";
    }

    /// <summary>
    /// �������� �������� ������� ��� ������������ ����������������
    /// </summary>
    private void LoadSampleEvents()
    {
        var today = DateTime.Today;
        var sampleEvents = new[]
        {
            new CalendarEvent { Title = "TEAM MEETING", Description = "Weekly team sync", StartDateTime = today.AddHours(10), EndDateTime = today.AddHours(11), Type = "MEETING", IsAllDay = false },
            new CalendarEvent { Title = "LUNCH BREAK", Description = "Lunch with colleagues", StartDateTime = today.AddHours(12), EndDateTime = today.AddHours(13), Type = "PERSONAL", IsAllDay = false },
            new CalendarEvent { Title = "PROJECT DEADLINE", Description = "Submit final project", StartDateTime = today.AddDays(7), EndDateTime = today.AddDays(7).AddDays(1), Type = "WORK", IsAllDay = true }
        };

        // ���������� �������� ������� � ������� ��� ������������
        foreach (var evt in sampleEvents)
        {
            var eventDate = evt.StartDateTime.Date;
            if (!events.ContainsKey(eventDate))
                events[eventDate] = new List<CalendarEvent>();
            events[eventDate].Add(evt);
        }

        // ���������� ����������� ��������� � ������ �������
        RefreshCalendarBoldDates();
        RefreshEventsList();
    }
}

/// <summary>
/// ����� ��� ������������� ������� ���������
/// </summary>
public class CalendarEvent
{
    public string Title { get; set; }           // �������� �������
    public string Description { get; set; }     // �������� �������
    public DateTime StartDateTime { get; set; }  // ���� � ����� ������
    public DateTime EndDateTime { get; set; }    // ���� � ����� ���������
    public bool IsAllDay { get; set; }          // ���� ������� �� ���� ����
    public string Type { get; set; }            // ��� ������� (�������, ����������� � �.�.)

    /// <summary>
    /// ��������� ������������� ������� ��� ����������� � �������
    /// </summary>
    public override string ToString()
    {
        var timeStr = IsAllDay ? "ALL DAY" : StartDateTime.ToString("HH:mm");
        return $"{timeStr} - {Title}";
    }
}

/// <summary>
/// ����� ��� ����������� ���� ������� � ��������������� �������
/// </summary>
public class AllEventsForm : Form
{
    private ListView allEventsListView;
    private readonly Dictionary<DateTime, List<CalendarEvent>> events;

    /// <summary>
    /// ����������� ����� ���� ������� � ��������� �������� �����
    /// </summary>
    public AllEventsForm(Dictionary<DateTime, List<CalendarEvent>> events, Color accent, Color bg, Color surface, Color border, Color text, Color muted)
    {
        this.events = events;
        InitializeForm(accent, bg, surface, border, text, muted);
        LoadAllEvents();
    }

    /// <summary>
    /// ������������� ����� ���� ������� � ����������� ������ ����
    /// </summary>
    private void InitializeForm(Color accent, Color bg, Color surface, Color border, Color text, Color muted)
    {
        Size = new Size(900, 600);
        Text = "ALL EVENTS";
        BackColor = bg;
        ForeColor = text;
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Consolas", 9f);

        // ListView ��� ����������� ���� ������� � ���������� �����������
        allEventsListView = new ListView
        {
            Dock = DockStyle.Fill,
            BackColor = surface,
            ForeColor = text,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            Font = new Font("Consolas", 9f),
            BorderStyle = BorderStyle.None,
            OwnerDraw = true // ��������� ��������� ��������� ��� ������ ����������
        };

        // ��������� ������� ��� ����������� ���������� � ��������
        allEventsListView.Columns.Add("DATE", 120);
        allEventsListView.Columns.Add("TIME", 100);
        allEventsListView.Columns.Add("TITLE", 250);
        allEventsListView.Columns.Add("TYPE", 100);
        allEventsListView.Columns.Add("DESCRIPTION", 250);

        // ��������� ��������� ���������� ��� ������������ ������ ����
        allEventsListView.DrawColumnHeader += (s, e) =>
        {
            e.Graphics.FillRectangle(new SolidBrush(border), e.Bounds);
            e.Graphics.DrawRectangle(new Pen(muted), e.Bounds);

            var textBounds = new Rectangle(e.Bounds.X + 5, e.Bounds.Y + 2, e.Bounds.Width - 10, e.Bounds.Height - 4);
            TextRenderer.DrawText(e.Graphics, e.Header.Text, new Font("Consolas", 9f, FontStyle.Bold),
                textBounds, text, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        };

        // ������������� ����������� ��������� ��� ��������� � ������������
        allEventsListView.DrawItem += (s, e) => { e.DrawDefault = true; };
        allEventsListView.DrawSubItem += (s, e) => { e.DrawDefault = true; };

        Controls.Add(allEventsListView);
    }

    /// <summary>
    /// �������� ���� ������� � ListView � ����������� �� ����
    /// </summary>
    private void LoadAllEvents()
    {
        // ��������� ���� ������� �� ������� � ���������� �� ���� ������
        var allEvents = events.SelectMany(kvp => kvp.Value)
                            .OrderBy(e => e.StartDateTime)
                            .ToList();

        // ���������� ������� ������� � ListView � ���������������
        foreach (var evt in allEvents)
        {
            var item = new ListViewItem(evt.StartDateTime.ToString("MMM dd, yyyy").ToUpper());
            item.SubItems.Add(evt.IsAllDay ? "ALL DAY" : evt.StartDateTime.ToString("HH:mm"));
            item.SubItems.Add(evt.Title.ToUpper());
            item.SubItems.Add(evt.Type);
            item.SubItems.Add(evt.Description.ToUpper());
            allEventsListView.Items.Add(item);
        }
    }
}