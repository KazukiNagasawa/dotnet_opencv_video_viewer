SRCS=$(wildcard src/*.cs)

all: mono_video_viewer.exe

mono_video_viewer.exe: $(SRCS)
	mcs -target:winexe -pkg:dotnet -out:$@ $^

clean:
	rm mono_video_viewer.exe
