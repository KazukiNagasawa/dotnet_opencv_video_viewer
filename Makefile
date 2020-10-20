VIDEO_LIB=video_lib
LIBNAME=libvideo_viewer.so

MONO_APP=Mono

AVALONIA_APP=AvaloniaUI
AVALONIA_BIN_DIR=bin/Debug/netcoreapp2.1

all: video_lib mono_app avalonia_app

video_lib:
	make -B -C $(VIDEO_LIB)

mono_app:
	cp $(VIDEO_LIB)/$(LIBNAME) $(MONO_APP)/
	make -B -C $(MONO_APP)

avalonia_app:
	cp $(VIDEO_LIB)/$(LIBNAME) $(AVALONIA_APP)/$(AVALONIA_BIN_DIR)/
	make -B -C $(AVALONIA_APP)

clean:
	make -B -C $(VIDEO_LIB) clean
	make -B -C $(MONO_APP) clean
	rm $(MONO_APP)/$(LIBNAME)
	make -B -C $(AVALONIA_APP) clean
