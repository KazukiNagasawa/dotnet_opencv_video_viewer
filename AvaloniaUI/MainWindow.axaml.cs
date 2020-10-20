using Avalonia;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Interactivity;
using Avalonia.Threading;

using System.IO; // MemoryStream
using System.Runtime.InteropServices;

namespace AvaloniaVideoViewer
{
    public class MainWindow : Window
    {
        private const int DLL_STATUS_OK = 0;
	private const int DLL_STATUS_NG = 1;

        [DllImport("video_viewer.so", CallingConvention = CallingConvention.Cdecl)]
        private static extern int initialize(string videoPath, int width, int height);
 
        [DllImport("video_viewer.so")]
        private static extern int getFrame([In, Out] byte[] data);
	
	public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            // Get controls
            this.TextBoxVideoPath = this.FindControl<TextBox>("TextBoxVideoPath");
            this.ButtonOpenVideo = this.FindControl<Button>("ButtonOpenVideo");
            this.ButtonStartVideo = this.FindControl<Button>("ButtonStartVideo");
            this.ButtonStopVideo = this.FindControl<Button>("ButtonStopVideo");
            this.ImageVideo = this.FindControl<Image>("ImageVideo");
            this.TextBlockFrameNo = this.FindControl<TextBlock>("TextBlockFrameNo");
            this.TextBlockLog = this.FindControl<TextBlock>("TextBlockLog");

            // Set properties
            this.TextBoxVideoPath.IsReadOnly = true;

            // Set event
            this.ButtonOpenVideo.Click += ButtonOpenVideo_Clicked;
            this.ButtonStartVideo.Click += ButtonStartVideo_Clicked;
            this.ButtonStopVideo.Click += ButtonStopVideo_Clicked;

            // Timer settings
            this.Timer = new DispatcherTimer();
            this.Timer.Interval = new System.TimeSpan(0, 0, 0, 0, 33); // 30 fps
            this.Timer.IsEnabled = false;
            this.Timer.Tick += Timer_Tick;

            // OpenFileDialog settings
            FileDialogFilter f1 = new FileDialogFilter();
            f1.Name = "動画ファイル (*.avi,*.mov,*.mp4)";
            f1.Extensions = new System.Collections.Generic.List<string>();
            f1.Extensions.Add("avi");
            f1.Extensions.Add("mov");
            f1.Extensions.Add("mp4");
            FileDialogFilter f2 = new FileDialogFilter();
            f2.Name = "すべてのファイル (*.*)";
            f2.Extensions.Add("*.*");
            this.OpenVideoDialog = new OpenFileDialog();
            this.OpenVideoDialog.Filters.Add(f1);
            this.OpenVideoDialog.Filters.Add(f2);

            // Bitmap
            this.bitmap = new WriteableBitmap(new PixelSize(this.width, this.height), new Vector(this.dpi, this.dpi), Avalonia.Platform.PixelFormat.Rgba8888);
            this.data = new byte[this.width * this.height * 4]; // RGBA 4 channels

            // Bind source
            this.ImageVideo.Width = this.width;
            this.ImageVideo.Height = this.height;
            this.ImageVideo.Source = this.bitmap;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// 動画選択ダイアログ表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonOpenVideo_Clicked(object sender, RoutedEventArgs e)
        {
	    try {
                string[] pathes = await this.OpenVideoDialog.ShowAsync(this);
                if (pathes.Length > 0) {
                    this.TextBoxVideoPath.Text = pathes[0];
                    this.videoPath = pathes[0];
                }
	    } catch {};
        }

        /// <summary>
        /// 動画再生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStartVideo_Clicked(object sender, RoutedEventArgs e)
        {
            // 動画確認
            if (!System.IO.File.Exists(this.videoPath)) {
		this.TextBlockLog.Text = "Video \"" + this.videoPath + "\" is not found.";
                return;
            }
	    
	    // dll 初期化 (動画読み込み)
            if (initialize(this.videoPath, this.width, this.height) != DLL_STATUS_OK) {
		this.TextBlockLog.Text = "Failed to load video \"" + this.videoPath + "\".";
                return;
	    }
	    this.frameNo = 0;

	    // 再生
	    this.TextBlockLog.Text = "";
            this.Timer.IsEnabled = true;
        }

        /// <summary>
        /// 動画停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStopVideo_Clicked(object sender, RoutedEventArgs e)
        {
	    // 停止
            this.Timer.IsEnabled = false;
        }

        /// <summary>
        /// 動画更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, System.EventArgs e)
        {
	    // 読めなくなったら停止
            if (getFrame(this.data) != DLL_STATUS_OK) {
	        this.Timer.IsEnabled = false;
	    }

            // バイト列を BitmapImage に変換
            using (var l = bitmap.Lock()) {
                for (int i = 0; i < height; i++) {
                    Marshal.Copy(this.data, i * l.RowBytes, new System.IntPtr(l.Address.ToInt64() + i * l.RowBytes), l.RowBytes);
                }
            }

	    // Update image
	    this.TextBlockFrameNo.Text = (++this.frameNo).ToString();
            this.ImageVideo.InvalidateVisual();
	}

        /// <summary>
	/// Video path
	/// </summary>
	private string videoPath = "";

        /// <summary>
        /// Controls
        /// </summary>
        private TextBox TextBoxVideoPath;
        private Button ButtonOpenVideo;
        private Button ButtonStartVideo;
        private Button ButtonStopVideo;
        private Image ImageVideo;
        private TextBlock TextBlockFrameNo;
        private DispatcherTimer Timer;
        private OpenFileDialog OpenVideoDialog;
        private TextBlock TextBlockLog;

        /// <summary>
        /// Bitmap
        /// </summary>
        private int width = 640;
        private int height = 360;
        private int dpi = 96;
        private byte[] data;
        private WriteableBitmap bitmap;
        private int frameNo = 0;
    }
}
