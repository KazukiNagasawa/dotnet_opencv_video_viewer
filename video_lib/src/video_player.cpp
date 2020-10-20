#include <opencv2/opencv.hpp>
#include <iostream>

#define byte unsigned char

#define STATUS_OK 0
#define STATUS_NG 1

extern "C" {
    int initialize(char* videoPath, int width, int height);
    int getFrame(byte* data);
}

int windowWidth;
int windowHeight;
float wScale, hScale;

cv::VideoCapture cap;


int initialize(char* videoPath, int width, int height)
{
    windowWidth = width;
    windowHeight = height;

    // Load video
    cap.open(videoPath);
    if (!cap.isOpened()) {
        std::cerr << "Error: Movie file" << videoPath << "is not found." << std::endl;
        return STATUS_NG;
    }
    std::cout << "Load video \"" << videoPath << "\"." << std::endl;

    // Get first frame and calc aspect
    cv::Mat frame;
    cap >> frame;
    if (frame.empty()) {
        std::cerr << "Error: Movie file read error." << std::endl;
        return STATUS_NG;
    }
    int w = frame.cols;
    int h = frame.rows;
    wScale = (float)windowWidth / (float)w;
    hScale = (float)windowHeight / (float)h;

    std::cout << "Display size : " << windowWidth << " x " << windowHeight << std::endl;
    std::cout << "Video size : " << w << " x " << h << std::endl;
    std::cout << "Resize scale : " << wScale << " x " << hScale << std::endl;
}

int getFrame(byte* data)
{
    cv::Mat frame;
    cap >> frame;
    if (frame.empty()) {
        std::cerr << "Error: Movie end." << std::endl;
        return STATUS_NG;
    }

    // Resize to window size
    cv::Mat rFrame;
    cv::resize(frame, rFrame, cv::Size(), wScale, hScale);
    
    // cv::Mat to byte data
    int c = 0; // data counter
    for (int y = 0; y < windowHeight; y++) {
        cv::Vec3b *col = rFrame.ptr<cv::Vec3b>(y);
        for (int x = 0; x < windowWidth; x++) {
            data[c++] = col[x][0]; // B
            data[c++] = col[x][1]; // G
            data[c++] = col[x][2]; // R
            data[c++] = 255;       // A
        }
    }

    return STATUS_OK;
}


