VIDEO_LIB=video_lib


all: video_lib


video_lib:
	make -C $(VIDEO_LIB)

clean:
	make -C $(VIDEO_LIB) clean
