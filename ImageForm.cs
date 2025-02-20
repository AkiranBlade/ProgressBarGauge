using System.Text.Json;

namespace ProgressBarGauge;

public partial class ImageForm : Form
{
    private Point _mouseOffset;
    private bool _mouseDown;
    private string? _imagePath;
    private Label? _grabHandle;
    private PictureBox? _pictureBox;
    private const int GrabHandleWidth = 30;

    public ImageForm()
    {
        InitializeComponent();
        LoadConfig();
        SetupForm();
    }

    private void LoadConfig()
    {
        string configPath = "config.json";
        if (File.Exists(configPath))
        {
            var configJson = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<Config>(configJson);
            _imagePath = config?.ImagePath ?? ".\\gauge.png";
        }
        else
        {
            MessageBox.Show("Config file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }
    }

    private void SetupForm()
    {
        if (string.IsNullOrEmpty(_imagePath) || !File.Exists(_imagePath))
        {
            MessageBox.Show("Image file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
            return;
        }

        Image img = Image.FromFile(_imagePath);
        Bitmap bmp = new (img);
        TransparencyKey = Color.Magenta;
        BackColor = Color.Magenta;

        // Add grab handle panel
        _grabHandle = new Label
        {
            AutoSize = false,
            Size = new Size(GrabHandleWidth, bmp.Height),
            Location = new Point(0, 0),
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand,
            Text = "⏵"
        };

        _grabHandle.Font = new Font(_grabHandle.Font.FontFamily, 18);
        _grabHandle.ForeColor = Color.Red;


        _pictureBox = new PictureBox
        {
            BackgroundImage = bmp,
            Left = _grabHandle.Left + _grabHandle.Width,
            Width = img.Width,
            Height = img.Height
        };

        // Adjust form size so that ClientRectangle matches the image size
        int borderWidth = (Width - ClientSize.Width + GrabHandleWidth);
        int titleBarHeight = (Height - ClientSize.Height);
        Size = new Size(_pictureBox.Width + borderWidth + GrabHandleWidth, _pictureBox.Height + titleBarHeight);

        FormBorderStyle = FormBorderStyle.None;
        TopMost = true;

        _grabHandle.MouseDown += ImageForm_MouseDown;
        _grabHandle.MouseMove += ImageForm_MouseMove;
        _grabHandle.MouseUp += ImageForm_MouseUp;


        Controls.Add(_grabHandle);
        Controls.Add(_pictureBox);
    }

    private void ImageForm_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;

        _mouseOffset = new Point(e.X, e.Y);
        _mouseDown = true;
    }

    private void ImageForm_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!_mouseDown) return;

        Location = new Point(Cursor.Position.X - _mouseOffset.X, Cursor.Position.Y - _mouseOffset.Y);
    }

    private void ImageForm_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;

        _mouseDown = false;
    }
}