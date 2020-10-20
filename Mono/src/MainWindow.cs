using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MonoVideoViewer
{
    public partial class MainWindow : Form
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

            // Open file dialog settings
            this.openFileDialog.Filter = "動画ファイル (*.avi,*.mov,*.mp4)|*.avi;*.mov;*.mp4|全てのファイル (*.*)|*.*";
            this.openFileDialog.FileName = "";

	    // Bitmap data settings
            this.data = new byte[this.width * this.height * 4];
            this.bmp = new Bitmap(width, height);

	    // Image window setting
	    this.pictureBox.Width = this.width;
	    this.pictureBox.Height = this.height;
        }

        /// <summary>
        /// 動画選択ダイアログ表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpenVideo_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() != DialogResult.OK) {
                return;
            }
            this.textBoxPath.Text = this.openFileDialog.FileName;
            this.videoPath = this.openFileDialog.FileName;
        }

        /// <summary>
        /// 動画再生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            // 動画確認
            if (!System.IO.File.Exists(this.videoPath)) {
                MessageBox.Show("Video \"" + this.videoPath + "\" is not found.");
                return;
            }

	    // dll 初期化 (動画読み込み)
            if (initialize(this.videoPath, this.width, this.height) != DLL_STATUS_OK) {
                MessageBox.Show("Failed to load video \"" + this.videoPath + "\".");
                return;
	    }
	    this.frameNo = 0;

            // 再生
            this.timerUpdateFrame.Enabled = true;
        }

        /// <summary>
        /// 動画停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            // 停止
            this.timerUpdateFrame.Enabled = false;
        }

        /// <summary>
        /// 動画表示更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerUpdateFrame_Tick(object sender, EventArgs e)
        {
	    // 読めなくなったら停止
            if (getFrame(this.data) != DLL_STATUS_OK) {
	        this.timerUpdateFrame.Enabled = false;
	    }

	    // Byte to Bitmap
	    BytesToBitmap(this.data, ref this.bmp);

	    // Update picturebox
	    this.labelFrameNo.Text = (++this.frameNo).ToString();
	    this.pictureBox.Image = this.bmp;
        }

        /// <summary>
        /// byte 配列 → Bitmap 変換
        /// </summary>
        /// <param name="rgbBytes"></param>
        /// <param name="bmp"></param>
        private void BytesToBitmap(byte[] rgbBytes, ref Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            IntPtr ptr = bmpData.Scan0;
            Marshal.Copy(rgbBytes, 0, ptr, rgbBytes.Length);
            bmp.UnlockBits(bmpData);
        }

        /// <summary>
        /// Video path
        /// </summary>
        private string videoPath = "";

        /// <summary>
	/// 動画パラメータ
	/// </summary>
	private int width = 640;
	private int height = 360;
	private byte[] data = null;
	private Bitmap bmp = null;
        private int frameNo = 0;
    }
}
