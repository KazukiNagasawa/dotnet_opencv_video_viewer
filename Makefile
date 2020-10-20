VIDEO_LIB=video_lib
LIBNAME=libvideo_viewer.so


MONO_APP=Mono

all: video_lib mono_app


video_lib:
	make -B -C $(VIDEO_LIB)

mono_app:
	cp $(VIDEO_LIB)/$(LIBNAME) $(MONO_APP)/
	make -B -C $(MONO_APP)

clean:
	make -B -C $(VIDEO_LIB) clean
	make -B -C $(MONO_APP) clean
	rm $(MONO_APP)/$(LIBNAME)
