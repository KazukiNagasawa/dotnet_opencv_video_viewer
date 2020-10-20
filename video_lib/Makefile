OPENCV_INCLUDE_DIR=/usr/local/include/opencv4
OPENCV_LIB_DIR=/usr/local/lib
LIBS=-lopencv_core -lopencv_imgcodecs -lopencv_highgui -lopencv_video -lopencv_videoio

SRCS=$(wildcard src/*.cpp)


all: libvideo_player.so

libvideo_player.so: $(SRCS)
	g++ -shared -fPIC -o $@ $^ -I$(OPENCV_INCLUDE_DIR) -L$(OPENCV_LIB_DIR) $(LIBS)

clean:
	rm libvideo_player.so



