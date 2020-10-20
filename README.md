# dotnet_opencv_video_viewer

.NET な GUI (Mono, AvaloniaUI) で動作する動画プレーヤーのサンプル

## 環境
以下の環境で動作確認した。
- Ubuntu 18.04 (on WSL)
    - OpenCV 4.2.0
    - Mono (mcs version 4.6.2.0)
    - .NET Core SDK 2.1


## 準備
- OpenCV
```
sudo su - 
cd /opt
wget https://github.com/opencv/opencv/archive/4.2.0.zip
unzip 4.2.0.zip && rm 4.2.0.zip
cd opencv-4.2.0 && mkdir build && cd build && \
  cmake -DWITH_LIBV4L=ON -DWITH_CUDA=OFF .. && \
  make -j8 && make install
```

- Mono
```
sudo apt update
sudo apt install mono-devel
```

- .NET Core SDK 2.1
```
wget https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update; \
  sudo apt install -y apt-transport-https && \
  sudo apt update && \
  sudo apt install -y dotnet-sdk-2.1
```


## ビルド
Makefile でビルド
```
make
```

個別のビルドは各ディレクトリもしくは Makefile のコマンドを確認すること。


### 実行
- Mono 版
```
```

- AvaloniaUI 版
```
```
