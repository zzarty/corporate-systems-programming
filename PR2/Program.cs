using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

/// <summary>
/// Основная форма календаря с консольным дизайном в темной теме
/// </summary>
public class CalendarForm : Form
{
    // Основные элементы управления
    private MonthCalendar calendar;              // Календарь для отображения года
    private Panel leftPanel;                     // Левая панель для календаря
    private Panel rightPanel;                    // Правая панель для управления событиями
    private Panel topPanel;                      // Верхняя панель с кнопками
    private Panel bottomPanel;                   // Нижняя панель для статуса
    private Panel eventFormPanel;                // Панель формы событий
    private Panel calendarContainer;             // Контейнер для календаря

    // Элементы формы событий
    private TextBox eventTitleBox;               // Поле для названия события
    private TextBox eventDescriptionBox;         // Поле для описания события
    private DateTimePicker eventStartDatePicker; // Выбор даты начала
    private DateTimePicker eventEndDatePicker;   // Выбор даты окончания
    private DateTimePicker eventStartTimePicker; // Выбор времени начала
    private DateTimePicker eventEndTimePicker;   // Выбор времени окончания
    private CheckBox allDayCheckBox;             // Чекбокс "Весь день"
    private ListView eventsListView;             // Список событий

    // Кнопки управления
    private Button addEventButton;               // Кнопка добавления события
    private Button editEventButton;              // Кнопка редактирования события
    private Button deleteEventButton;            // Кнопка удаления события
    private Button exportButton;                 // Кнопка экспорта
    private Button importButton;                 // Кнопка импорта
    private Button todayButton;                  // Кнопка "Сегодня"
    private Button viewAllEventsButton;          // Кнопка просмотра всех событий
    private Button accentColorButton;            // Кнопка смены темы
    private Label statusLabel;                   // Метка статуса
    private ComboBox eventTypeCombo;             // Выпадающий список типов событий

    // Хранилище данных
    private readonly Dictionary<DateTime, List<CalendarEvent>> events = new();
    private CalendarEvent selectedEvent = null;  // Выбранное событие для редактирования

    // Система акцентных цветов
    private Color currentAccentColor = Color.FromArgb(0, 255, 127);
    private readonly Color[] accentColors = {
        Color.FromArgb(0, 255, 127),   // Зеленый
        Color.FromArgb(0, 174, 255),   // Синий
        Color.FromArgb(255, 69, 0),    // Красно-оранжевый
        Color.FromArgb(255, 215, 0),   // Золотой
        Color.FromArgb(186, 85, 211),  // Средне-фиолетовый
        Color.FromArgb(255, 20, 147),  // Темно-розовый
        Color.FromArgb(0, 255, 255),   // Циан
    };
    private int currentAccentIndex = 0;          // Индекс текущего акцентного цвета

    // Консольная цветовая схема
    private readonly Color backgroundColor = Color.FromArgb(18, 18, 18);      // Глубокий черный фон
    private readonly Color surfaceColor = Color.FromArgb(28, 28, 28);         // Темная поверхность
    private readonly Color borderColor = Color.FromArgb(45, 45, 45);          // Серый цвет границ
    private readonly Color textColor = Color.FromArgb(220, 220, 220);         // Светло-серый текст
    private readonly Color mutedTextColor = Color.FromArgb(150, 150, 150);    // Приглушенный текст
    private readonly Color buttonColor = Color.FromArgb(40, 40, 40);          // Фон кнопок

    /// <summary>
    /// Точка входа в приложение
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new CalendarForm());
    }

    /// <summary>
    /// Конструктор формы - инициализирует все компоненты
    /// </summary>
    public CalendarForm()
    {
        InitializeForm();        // Настройка основной формы
        InitializePanels();      // Создание панелей
        InitializeControls();    // Создание элементов управления
        SetupEvents();          // Настройка событий
        LoadSampleEvents();     // Загрузка примеров событий
    }

    /// <summary>
    /// Инициализация основных свойств формы
    /// </summary>
    private void InitializeForm()
    {
        // Фиксированный размер окна для консистентного дизайна
        Size = new Size(1400, 900);
        MaximumSize = new Size(1400, 900);
        MinimumSize = new Size(1400, 900);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        // Применение темной темы
        BackColor = backgroundColor;
        ForeColor = textColor;
        Font = new Font("Consolas", 9f); // Моноширинный шрифт для консольного вида
        Text = "Calendar";
        StartPosition = FormStartPosition.CenterScreen;
    }

    /// <summary>
    /// Создание и размещение основных панелей
    /// </summary>
    private void InitializePanels()
    {
        // Верхняя панель с кнопками управления
        topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = surfaceColor,
            Padding = new Padding(15),
        };
        topPanel.Paint += (s, e) => DrawBorder(e.Graphics, topPanel.ClientRectangle, borderColor);

        // Левая панель для календаря
        leftPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = 850,
            BackColor = backgroundColor,
            Padding = new Padding(15),
        };

        // Увеличенный контейнер для календаря с правильными размерами
        calendarContainer = new Panel
        {
            Location = new Point(15, 15),
            Size = new Size(820, 780), // Увеличенная высота для лучшего размещения
            BackColor = surfaceColor,
            Padding = new Padding(5), // Минимальные отступы для максимального использования пространства
        };
        calendarContainer.Paint += (s, e) => DrawBorder(e.Graphics, calendarContainer.ClientRectangle, borderColor);

        // Правая панель для управления событиями
        rightPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = backgroundColor,
            Padding = new Padding(15),
        };

        // Нижняя панель для отображения статуса
        bottomPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 35,
            BackColor = surfaceColor,
        };
        bottomPanel.Paint += (s, e) => DrawBorder(e.Graphics, bottomPanel.ClientRectangle, borderColor);

        // Добавление контейнера календаря в левую панель
        leftPanel.Controls.Add(calendarContainer);
        // Добавление всех панелей в форму
        Controls.AddRange(new Control[] { rightPanel, leftPanel, topPanel, bottomPanel });
    }

    /// <summary>
    /// Инициализация всех элементов управления
    /// </summary>
    private void InitializeControls()
    {
        InitializeTopPanelControls();    // Элементы верхней панели
        InitializeCalendar();           // Календарь
        InitializeRightPanelControls(); // Элементы правой панели
        InitializeStatusBar();          // Строка состояния
    }

    /// <summary>
    /// Создание элементов верхней панели (заголовок и кнопки)
    /// </summary>
    private void InitializeTopPanelControls()
    {
        // Заголовок приложения
        var titleLabel = new Label
        {
            Text = "CALENDAR",
            Location = new Point(15, 18),
            Size = new Size(150, 25),
            ForeColor = textColor,
            Font = new Font("Consolas", 14f, FontStyle.Bold),
            BackColor = Color.Transparent
        };

        // Единый размер для всех кнопок верхней панели
        var buttonSize = new Size(90, 30);
        var buttonY = 15;

        // Создание кнопок с консольным стилем
        todayButton = CreateConsoleButton("TODAY", new Point(200, buttonY), buttonSize, true); // Основная кнопка
        exportButton = CreateConsoleButton("EXPORT", new Point(300, buttonY), buttonSize);
        importButton = CreateConsoleButton("IMPORT", new Point(400, buttonY), buttonSize);
        viewAllEventsButton = CreateConsoleButton("ALL", new Point(500, buttonY), buttonSize);

        // Кнопка темы всегда с цветом
        accentColorButton = CreateConsoleButton("THEME", new Point(600, buttonY), buttonSize, true, true);

        // Добавление всех элементов в верхнюю панель
        topPanel.Controls.AddRange(new Control[] {
            titleLabel, todayButton, exportButton, importButton, viewAllEventsButton, accentColorButton
        });
    }

    /// <summary>
    /// Инициализация календаря с увеличенным размером
    /// </summary>
    private void InitializeCalendar()
    {
        // Создание календаря с настройками для годового просмотра
        calendar = new MonthCalendar
        {
            Location = new Point(5, 5),
            Size = new Size(810, 770), // Увеличенный размер для лучшего заполнения контейнера
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,

            // Принудительное применение темных цветов
            BackColor = surfaceColor,
            ForeColor = textColor,
            TitleBackColor = currentAccentColor,
            TitleForeColor = Color.Black,
            TrailingForeColor = mutedTextColor,

            // Настройки макета для отображения полного года (3 столбца, 4 строки)
            CalendarDimensions = new Size(3, 4), // 3x4 для лучшего размещения 12 месяцев
            FirstDayOfWeek = Day.Sunday,
            ShowToday = true,
            ShowTodayCircle = true,
            ShowWeekNumbers = false,
            MaxSelectionCount = 42, // Разрешить выбор диапазона дат
            Font = new Font("Consolas", 8f), // Оптимальный шрифт для размещения 12 месяцев

            // Ограничение диапазона дат текущим годом
            MinDate = new DateTime(DateTime.Now.Year, 1, 1),
            MaxDate = new DateTime(2030, 12, 31)
        };

        calendarContainer.Controls.Add(calendar);
        calendar.BringToFront(); // Принудительное размещение календаря поверх других элементов
    }

    /// <summary>
    /// Принудительное применение темы к календарю (Windows Forms календарь сопротивляется темной теме)
    /// </summary>
    private void ForceCalendarTheme()
    {
        try
        {
            // Принудительное применение темной темы несколько раз для гарантии
            for (int i = 0; i < 3; i++)
            {
                calendar.BackColor = surfaceColor;
                calendar.ForeColor = textColor;
                calendar.TitleBackColor = currentAccentColor;
                calendar.TitleForeColor = Color.Black;
                calendar.TrailingForeColor = mutedTextColor;
            }

            // Принудительное обновление визуального состояния
            calendar.Refresh();
            calendar.Invalidate();
            calendar.Update();

            // Дополнительный таймер для гарантии применения темы через небольшую задержку
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
    /// Создание элементов управления в правой панели
    /// </summary>
    private void InitializeRightPanelControls()
    {
        // Панель формы событий с уменьшенной высотой для лучшего размещения кнопок
        eventFormPanel = new Panel
        {
            Location = new Point(0, 0),
            Size = new Size(500, 370), // Уменьшенная высота для правильного позиционирования
            BackColor = surfaceColor,
            Padding = new Padding(15)
        };
        eventFormPanel.Paint += (s, e) => DrawBorder(e.Graphics, eventFormPanel.ClientRectangle, borderColor);

        // Заголовок секции управления событиями
        var eventFormLabel = new Label
        {
            Text = "EVENT MANAGEMENT",
            Location = new Point(15, 15),
            Size = new Size(250, 25),
            ForeColor = textColor,
            Font = new Font("Consolas", 12f, FontStyle.Bold),
            BackColor = Color.Transparent
        };

        // Поля ввода для основной информации о событии
        eventTitleBox = CreateConsoleTextBox("Event Title", new Point(15, 50), new Size(450, 25));
        eventDescriptionBox = CreateConsoleTextBox("Description", new Point(15, 85), new Size(450, 50), true);

        // Элементы управления диапазоном дат
        var dateRangeLabel = CreateConsoleLabel("DATE RANGE:", new Point(15, 150));
        eventStartDatePicker = CreateConsoleDatePicker(new Point(15, 175), new Size(130, 25));
        var toLabel = CreateConsoleLabel("TO", new Point(155, 178));
        toLabel.Size = new Size(25, 20);
        toLabel.TextAlign = ContentAlignment.MiddleCenter;
        eventEndDatePicker = CreateConsoleDatePicker(new Point(190, 175), new Size(130, 25));

        // Элементы управления временем
        var timeLabel = CreateConsoleLabel("TIME:", new Point(15, 210));
        eventStartTimePicker = CreateConsoleTimePicker(new Point(15, 235), new Size(100, 25));
        eventEndTimePicker = CreateConsoleTimePicker(new Point(125, 235), new Size(100, 25));

        // Чекбокс для событий на весь день
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

        // Выпадающий список типов событий
        var typeLabel = CreateConsoleLabel("TYPE:", new Point(15, 270));
        eventTypeCombo = CreateConsoleComboBox(new Point(70, 267), new Size(120, 25));
        eventTypeCombo.Items.AddRange(new[] { "MEETING", "APPOINTMENT", "REMINDER", "HOLIDAY", "PERSONAL", "WORK" });
        eventTypeCombo.SelectedIndex = 0;

        // Кнопки управления событиями с улучшенным позиционированием
        var buttonSize = new Size(100, 30);
        addEventButton = CreateConsoleButton("ADD", new Point(15, 300), buttonSize, true);
        editEventButton = CreateConsoleButton("UPDATE", new Point(125, 300), buttonSize);
        deleteEventButton = CreateConsoleButton("DELETE", new Point(235, 300), buttonSize);

        editEventButton.Enabled = false; // Отключена по умолчанию до выбора события

        // Добавление всех элементов в панель формы
        eventFormPanel.Controls.AddRange(new Control[] {
            eventFormLabel, eventTitleBox, eventDescriptionBox, dateRangeLabel,
            eventStartDatePicker, toLabel, eventEndDatePicker, timeLabel,
            eventStartTimePicker, eventEndTimePicker, allDayCheckBox,
            typeLabel, eventTypeCombo, addEventButton, editEventButton, deleteEventButton
        });

        // Заголовок списка предстоящих событий
        var eventsLabel = new Label
        {
            Text = "UPCOMING EVENTS",
            Location = new Point(0, 380), // Подстроено под новую высоту панели
            Size = new Size(200, 25),
            ForeColor = textColor,
            Font = new Font("Consolas", 11f, FontStyle.Bold),
            BackColor = Color.Transparent
        };

        // ListView для отображения событий с темными заголовками
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
            OwnerDraw = true // Включение кастомной отрисовки для темных заголовков
        };

        // Настройка колонок ListView
        eventsListView.Columns.Add("DATE", 100);
        eventsListView.Columns.Add("TIME", 80);
        eventsListView.Columns.Add("TITLE", 200);
        eventsListView.Columns.Add("TYPE", 80);

        // Кастомная отрисовка заголовков ListView для устранения белых углов
        eventsListView.DrawColumnHeader += (s, e) =>
        {
            // Заливка всего заголовка темным цветом для устранения белых артефактов
            e.Graphics.FillRectangle(new SolidBrush(borderColor), e.Bounds);

            // Рисование границы заголовка
            using (var pen = new Pen(mutedTextColor))
            {
                e.Graphics.DrawRectangle(pen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
            }

            // Отрисовка текста заголовка
            var textBounds = new Rectangle(e.Bounds.X + 5, e.Bounds.Y + 2, e.Bounds.Width - 10, e.Bounds.Height - 4);
            TextRenderer.DrawText(e.Graphics, e.Header.Text, new Font("Consolas", 9f, FontStyle.Bold),
                textBounds, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        };

        // Использование стандартной отрисовки для элементов списка
        eventsListView.DrawItem += (s, e) => { e.DrawDefault = true; };
        eventsListView.DrawSubItem += (s, e) => { e.DrawDefault = true; };

        // Обработка перерисовки для устранения белых артефактов при прокрутке
        eventsListView.Paint += (s, e) =>
        {
            // Заливка фона ListView для устранения белых углов
            e.Graphics.FillRectangle(new SolidBrush(surfaceColor), eventsListView.ClientRectangle);
        };

        // Добавление элементов в правую панель
        rightPanel.Controls.AddRange(new Control[] { eventFormPanel, eventsLabel, eventsListView });
    }

    /// <summary>
    /// Инициализация строки состояния
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
    /// Создание кнопки в консольном стиле с поддержкой кнопки темы
    /// </summary>
    /// <param name="text">Текст кнопки</param>
    /// <param name="location">Позиция кнопки</param>
    /// <param name="size">Размер кнопки</param>
    /// <param name="isPrimary">Является ли кнопка основной (с акцентным цветом)</param>
    /// <param name="isThemeButton">Является ли кнопка кнопкой смены темы (всегда цветная)</param>
    private Button CreateConsoleButton(string text, Point location, Size size, bool isPrimary = false, bool isThemeButton = false)
    {
        // Определение цветов кнопки в зависимости от её типа
        Color buttonBackColor;
        Color buttonForeColor;

        if (isThemeButton)
        {
            // Кнопка темы всегда отображается с текущим акцентным цветом
            buttonBackColor = currentAccentColor;
            buttonForeColor = Color.Black;
        }
        else if (isPrimary)
        {
            // Основные кнопки используют акцентный цвет
            buttonBackColor = currentAccentColor;
            buttonForeColor = Color.Black;
        }
        else
        {
            // Обычные кнопки используют стандартный темный цвет
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

        // Настройка плоского стиля кнопки
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = borderColor;

        // Эффекты наведения мыши
        button.MouseEnter += (s, e) =>
        {
            if (isPrimary || isThemeButton)
            {
                // Осветление акцентного цвета при наведении на основные кнопки
                button.BackColor = Color.FromArgb(
                    Math.Min(255, currentAccentColor.R + 30),
                    Math.Min(255, currentAccentColor.G + 30),
                    Math.Min(255, currentAccentColor.B + 30)
                );
            }
            else
            {
                // Осветление обычной кнопки при наведении
                button.BackColor = Color.FromArgb(60, 60, 60);
            }
        };

        button.MouseLeave += (s, e) =>
        {
            // Восстановление исходного цвета при уходе курсора
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
    /// Создание текстового поля в консольном стиле с функциональностью placeholder'а
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

        // Установка placeholder текста
        textBox.Text = placeholder;
        textBox.ForeColor = mutedTextColor;

        // Обработка фокуса для реализации placeholder функциональности
        textBox.Enter += (s, e) =>
        {
            // Очистка placeholder при получении фокуса
            if (textBox.Text == placeholder)
            {
                textBox.Text = "";
                textBox.ForeColor = textColor;
            }
        };

        textBox.Leave += (s, e) =>
        {
            // Восстановление placeholder при потере фокуса и пустом поле
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = placeholder;
                textBox.ForeColor = mutedTextColor;
            }
        };

        return textBox;
    }

    /// <summary>
    /// Создание выбора даты в консольном стиле
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

        // Настройка цветов выпадающего календаря
        picker.CalendarForeColor = textColor;
        picker.CalendarMonthBackground = surfaceColor;
        picker.CalendarTitleBackColor = currentAccentColor;
        picker.CalendarTitleForeColor = Color.Black;
        picker.CalendarTrailingForeColor = mutedTextColor;

        return picker;
    }

    /// <summary>
    /// Создание выбора времени в консольном стиле
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
            ShowUpDown = true, // Использование кнопок вверх/вниз вместо выпадающего списка
            Font = new Font("Consolas", 9f)
        };
    }

    /// <summary>
    /// Создание выпадающего списка в консольном стиле
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
            DropDownStyle = ComboBoxStyle.DropDownList, // Только выбор из списка, без ввода
            Font = new Font("Consolas", 9f)
        };
    }

    /// <summary>
    /// Создание метки в консольном стиле
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
    /// Отрисовка границы панели
    /// </summary>
    private void DrawBorder(Graphics g, Rectangle rect, Color color)
    {
        using var pen = new Pen(color, 1);
        g.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
    }

    /// <summary>
    /// Применение акцентного цвета ко всем элементам с принудительным обновлением календаря
    /// </summary>
    private void ApplyAccentColor()
    {
        // Применение акцентного цвета к основным кнопкам
        todayButton.BackColor = currentAccentColor;
        addEventButton.BackColor = currentAccentColor;

        // Кнопка темы всегда остается с акцентным цветом
        accentColorButton.BackColor = currentAccentColor;
        accentColorButton.ForeColor = Color.Black;

        // Применение акцентного цвета к календарю
        calendar.TitleBackColor = currentAccentColor;
        calendar.BackColor = surfaceColor;
        calendar.ForeColor = textColor;

        // Применение к элементам выбора даты
        eventStartDatePicker.CalendarTitleBackColor = currentAccentColor;
        eventEndDatePicker.CalendarTitleBackColor = currentAccentColor;

        // Принудительное обновление визуального состояния
        calendar.Refresh();
        calendar.Invalidate();
        Refresh();
    }

    /// <summary>
    /// Настройка обработчиков событий для всех элементов управления
    /// </summary>
    private void SetupEvents()
    {
        // Обработчики событий календаря и формы
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
    /// Обработчик смены акцентного цвета (циклическое переключение)
    /// </summary>
    private void AccentColor_Click(object sender, EventArgs e)
    {
        // Циклическое переключение между доступными акцентными цветами
        currentAccentIndex = (currentAccentIndex + 1) % accentColors.Length;
        currentAccentColor = accentColors[currentAccentIndex];
        ApplyAccentColor();
        statusLabel.Text = "THEME CHANGED";
    }

    /// <summary>
    /// Обработчик выбора даты в календаре
    /// </summary>
    private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
    {
        // Автоматическое заполнение полей даты в форме события
        eventStartDatePicker.Value = e.Start;
        eventEndDatePicker.Value = e.End;

        // Отображение выбранного диапазона в строке состояния
        if (e.Start.Date == e.End.Date)
        {
            statusLabel.Text = $"SELECTED: {e.Start:MMM dd, yyyy}".ToUpper();
        }
        else
        {
            statusLabel.Text = $"SELECTED RANGE: {e.Start:MMM dd} - {e.End:MMM dd, yyyy}".ToUpper();
        }

        RefreshEventsList(); // Обновление списка событий для выбранного диапазона
    }

    /// <summary>
    /// Обработчик добавления нового события
    /// </summary>
    private void AddEvent_Click(object sender, EventArgs e)
    {
        if (!ValidateEventForm()) return; // Проверка валидности введенных данных

        var startDate = eventStartDatePicker.Value.Date;
        var endDate = eventEndDatePicker.Value.Date;

        // Создание событий для каждого дня в выбранном диапазоне
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            // Определение времени события в зависимости от флага "весь день"
            var eventDateTime = allDayCheckBox.Checked ?
                date :
                date.Add(eventStartTimePicker.Value.TimeOfDay);

            var endDateTime = allDayCheckBox.Checked ?
                date.AddDays(1).AddSeconds(-1) :
                date.Add(eventEndTimePicker.Value.TimeOfDay);

            // Создание нового объекта события
            var newEvent = new CalendarEvent
            {
                Title = eventTitleBox.Text,
                Description = eventDescriptionBox.Text == "Description" ? "" : eventDescriptionBox.Text,
                StartDateTime = eventDateTime,
                EndDateTime = endDateTime,
                IsAllDay = allDayCheckBox.Checked,
                Type = eventTypeCombo.SelectedItem.ToString()
            };

            // Добавление события в словарь событий
            if (!events.ContainsKey(date))
                events[date] = new List<CalendarEvent>();

            events[date].Add(newEvent);
        }

        // Обновление интерфейса после добавления события
        RefreshCalendarBoldDates();
        RefreshEventsList();
        ClearEventForm();
        statusLabel.Text = $"EVENT ADDED FOR {(endDate - startDate).Days + 1} DAY(S)";
    }

    /// <summary>
    /// Обработчик редактирования выбранного события
    /// </summary>
    private void EditEvent_Click(object sender, EventArgs e)
    {
        if (selectedEvent == null || !ValidateEventForm()) return;

        // Обновление свойств выбранного события данными из формы
        selectedEvent.Title = eventTitleBox.Text;
        selectedEvent.Description = eventDescriptionBox.Text == "Description" ? "" : eventDescriptionBox.Text;
        selectedEvent.IsAllDay = allDayCheckBox.Checked;
        selectedEvent.Type = eventTypeCombo.SelectedItem.ToString();

        // Обновление времени события, если это не событие на весь день
        if (!allDayCheckBox.Checked)
        {
            selectedEvent.StartDateTime = eventStartDatePicker.Value.Date.Add(eventStartTimePicker.Value.TimeOfDay);
            selectedEvent.EndDateTime = eventEndDatePicker.Value.Date.Add(eventEndTimePicker.Value.TimeOfDay);
        }

        // Обновление интерфейса
        RefreshEventsList();
        ClearEventForm();
        statusLabel.Text = "EVENT UPDATED SUCCESSFULLY";
    }

    /// <summary>
    /// Обработчик удаления выбранного события
    /// </summary>
    private void DeleteEvent_Click(object sender, EventArgs e)
    {
        if (selectedEvent == null) return;

        // Подтверждение удаления у пользователя
        var result = MessageBox.Show("DELETE THIS EVENT?",
            "CONFIRM DELETE", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            var eventDate = selectedEvent.StartDateTime.Date;
            if (events.ContainsKey(eventDate))
            {
                events[eventDate].Remove(selectedEvent);
                // Удаление даты из словаря, если больше нет событий на эту дату
                if (!events[eventDate].Any())
                {
                    events.Remove(eventDate);
                }
            }

            // Обновление интерфейса после удаления
            RefreshCalendarBoldDates();
            RefreshEventsList();
            ClearEventForm();
            statusLabel.Text = "EVENT DELETED";
        }
    }

    /// <summary>
    /// Обработчик экспорта событий в JSON файл
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
                // Подготовка данных для экспорта в удобном для JSON формате
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

                // Сериализация в JSON с красивым форматированием и сохранение в файл
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
    /// Обработчик импорта событий из JSON файла
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
                // Чтение и десериализация JSON файла
                var json = File.ReadAllText(dialog.FileName);
                var importData = JsonSerializer.Deserialize<JsonElement[]>(json);

                int importedCount = 0;
                foreach (var item in importData)
                {
                    // Извлечение данных события из JSON элемента
                    var dateStr = item.GetProperty("StartDate").GetString();
                    var timeStr = item.GetProperty("StartTime").GetString();
                    var endDateStr = item.GetProperty("EndDate").GetString();
                    var endTimeStr = item.GetProperty("EndTime").GetString();
                    var title = item.GetProperty("Title").GetString();
                    var description = item.GetProperty("Description").GetString();
                    var type = item.GetProperty("Type").GetString();
                    var isAllDay = item.GetProperty("IsAllDay").GetBoolean();

                    // Парсинг дат и времени, создание события при успешном парсинге
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

                        // Добавление события в словарь
                        var eventDate = startDateTime.Date;
                        if (!events.ContainsKey(eventDate))
                            events[eventDate] = new List<CalendarEvent>();

                        events[eventDate].Add(newEvent);
                        importedCount++;
                    }
                }

                // Обновление интерфейса после импорта
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
    /// Переход к сегодняшней дате в календаре
    /// </summary>
    private void Today_Click(object sender, EventArgs e)
    {
        calendar.SetDate(DateTime.Today);
        RefreshEventsList();
        statusLabel.Text = "NAVIGATED TO TODAY";
    }

    /// <summary>
    /// Отображение окна со всеми событиями
    /// </summary>
    private void ViewAllEvents_Click(object sender, EventArgs e)
    {
        var allEventsForm = new AllEventsForm(events, currentAccentColor, backgroundColor, surfaceColor, borderColor, textColor, mutedTextColor);
        allEventsForm.ShowDialog();
    }

    /// <summary>
    /// Обработчик выбора события в списке для редактирования
    /// </summary>
    private void EventsList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (eventsListView.SelectedItems.Count > 0)
        {
            // Заполнение формы данными выбранного события
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

            // Активация кнопки редактирования
            editEventButton.Enabled = true;
            addEventButton.Text = "ADD NEW";
        }
        else
        {
            // Очистка выбора и возврат к режиму добавления
            selectedEvent = null;
            editEventButton.Enabled = false;
            addEventButton.Text = "ADD";
        }
    }

    /// <summary>
    /// Обработчик изменения чекбокса "Весь день"
    /// </summary>
    private void AllDay_CheckedChanged(object sender, EventArgs e)
    {
        // Отключение/включение элементов выбора времени в зависимости от состояния чекбокса
        eventStartTimePicker.Enabled = !allDayCheckBox.Checked;
        eventEndTimePicker.Enabled = !allDayCheckBox.Checked;
    }

    /// <summary>
    /// Валидация данных формы события перед сохранением
    /// </summary>
    private bool ValidateEventForm()
    {
        // Проверка заполнения обязательного поля названия события
        if (eventTitleBox.Text == "Event Title" || string.IsNullOrWhiteSpace(eventTitleBox.Text))
        {
            MessageBox.Show("PLEASE ENTER AN EVENT TITLE.", "VALIDATION ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        // Проверка корректности диапазона дат (конец не раньше начала)
        if (eventEndDatePicker.Value < eventStartDatePicker.Value)
        {
            MessageBox.Show("END DATE CANNOT BE BEFORE START DATE.", "VALIDATION ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Обновление списка событий для выбранного диапазона дат
    /// </summary>
    private void RefreshEventsList()
    {
        eventsListView.Items.Clear();

        var startDate = calendar.SelectionStart.Date;
        var endDate = calendar.SelectionEnd.Date;

        // Сбор всех событий из выбранного диапазона дат
        var relevantEvents = new List<CalendarEvent>();

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (events.ContainsKey(date))
            {
                relevantEvents.AddRange(events[date]);
            }
        }

        // Добавление событий в ListView с сортировкой по времени начала
        foreach (var evt in relevantEvents.OrderBy(e => e.StartDateTime))
        {
            var item = new ListViewItem(evt.StartDateTime.ToString("MMM dd").ToUpper());
            item.SubItems.Add(evt.IsAllDay ? "ALL DAY" : evt.StartDateTime.ToString("HH:mm"));
            item.SubItems.Add(evt.Title.ToUpper());
            item.SubItems.Add(evt.Type);
            item.Tag = evt; // Сохранение ссылки на событие для последующего редактирования
            item.BackColor = surfaceColor;
            item.ForeColor = textColor;
            eventsListView.Items.Add(item);
        }
    }

    /// <summary>
    /// Обновление выделенных дат в календаре (жирным шрифтом для дат с событиями)
    /// </summary>
    private void RefreshCalendarBoldDates()
    {
        // Выделение всех дат, на которые запланированы события
        calendar.BoldedDates = events.Keys.ToArray();
        calendar.UpdateBoldedDates();
    }

    /// <summary>
    /// Очистка формы события и возврат к состоянию по умолчанию
    /// </summary>
    private void ClearEventForm()
    {
        // Сброс всех полей формы к placeholder значениям
        eventTitleBox.Text = "Event Title";
        eventTitleBox.ForeColor = mutedTextColor;
        eventDescriptionBox.Text = "Description";
        eventDescriptionBox.ForeColor = mutedTextColor;
        eventStartTimePicker.Value = DateTime.Now;
        eventEndTimePicker.Value = DateTime.Now.AddHours(1); // Время окончания по умолчанию через час
        allDayCheckBox.Checked = false;
        eventTypeCombo.SelectedIndex = 0;
        selectedEvent = null;
        editEventButton.Enabled = false;
        addEventButton.Text = "ADD";
    }

    /// <summary>
    /// Загрузка примеров событий для демонстрации функциональности
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

        // Добавление примеров событий в словарь для демонстрации
        foreach (var evt in sampleEvents)
        {
            var eventDate = evt.StartDateTime.Date;
            if (!events.ContainsKey(eventDate))
                events[eventDate] = new List<CalendarEvent>();
            events[eventDate].Add(evt);
        }

        // Обновление отображения календаря и списка событий
        RefreshCalendarBoldDates();
        RefreshEventsList();
    }
}

/// <summary>
/// Класс для представления события календаря
/// </summary>
public class CalendarEvent
{
    public string Title { get; set; }           // Название события
    public string Description { get; set; }     // Описание события
    public DateTime StartDateTime { get; set; }  // Дата и время начала
    public DateTime EndDateTime { get; set; }    // Дата и время окончания
    public bool IsAllDay { get; set; }          // Флаг события на весь день
    public string Type { get; set; }            // Тип события (встреча, напоминание и т.д.)

    /// <summary>
    /// Строковое представление события для отображения в списках
    /// </summary>
    public override string ToString()
    {
        var timeStr = IsAllDay ? "ALL DAY" : StartDateTime.ToString("HH:mm");
        return $"{timeStr} - {Title}";
    }
}

/// <summary>
/// Форма для отображения всех событий в хронологическом порядке
/// </summary>
public class AllEventsForm : Form
{
    private ListView allEventsListView;
    private readonly Dictionary<DateTime, List<CalendarEvent>> events;

    /// <summary>
    /// Конструктор формы всех событий с передачей цветовой схемы
    /// </summary>
    public AllEventsForm(Dictionary<DateTime, List<CalendarEvent>> events, Color accent, Color bg, Color surface, Color border, Color text, Color muted)
    {
        this.events = events;
        InitializeForm(accent, bg, surface, border, text, muted);
        LoadAllEvents();
    }

    /// <summary>
    /// Инициализация формы всех событий с применением темной темы
    /// </summary>
    private void InitializeForm(Color accent, Color bg, Color surface, Color border, Color text, Color muted)
    {
        Size = new Size(900, 600);
        Text = "ALL EVENTS";
        BackColor = bg;
        ForeColor = text;
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Consolas", 9f);

        // ListView для отображения всех событий с кастомными заголовками
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
            OwnerDraw = true // Включение кастомной отрисовки для темных заголовков
        };

        // Настройка колонок для отображения информации о событиях
        allEventsListView.Columns.Add("DATE", 120);
        allEventsListView.Columns.Add("TIME", 100);
        allEventsListView.Columns.Add("TITLE", 250);
        allEventsListView.Columns.Add("TYPE", 100);
        allEventsListView.Columns.Add("DESCRIPTION", 250);

        // Кастомная отрисовка заголовков для соответствия темной теме
        allEventsListView.DrawColumnHeader += (s, e) =>
        {
            e.Graphics.FillRectangle(new SolidBrush(border), e.Bounds);
            e.Graphics.DrawRectangle(new Pen(muted), e.Bounds);

            var textBounds = new Rectangle(e.Bounds.X + 5, e.Bounds.Y + 2, e.Bounds.Width - 10, e.Bounds.Height - 4);
            TextRenderer.DrawText(e.Graphics, e.Header.Text, new Font("Consolas", 9f, FontStyle.Bold),
                textBounds, text, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        };

        // Использование стандартной отрисовки для элементов и подэлементов
        allEventsListView.DrawItem += (s, e) => { e.DrawDefault = true; };
        allEventsListView.DrawSubItem += (s, e) => { e.DrawDefault = true; };

        Controls.Add(allEventsListView);
    }

    /// <summary>
    /// Загрузка всех событий в ListView с сортировкой по дате
    /// </summary>
    private void LoadAllEvents()
    {
        // Получение всех событий из словаря и сортировка по дате начала
        var allEvents = events.SelectMany(kvp => kvp.Value)
                            .OrderBy(e => e.StartDateTime)
                            .ToList();

        // Добавление каждого события в ListView с форматированием
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