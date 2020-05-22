FROM ubuntu:18.04

RUN apt-get update && apt-get install -y \
    libgstreamer1.0-0 \
    gstreamer1.0-plugins-base \
    gstreamer1.0-plugins-good \
    gstreamer1.0-plugins-bad \
    gstreamer1.0-plugins-ugly \
    gstreamer1.0-libav \
    gstreamer1.0-doc \
    gstreamer1.0-tools \
    gstreamer1.0-x \
    gstreamer1.0-alsa \
    gstreamer1.0-gl \
    gstreamer1.0-gtk3 \
    gstreamer1.0-qt5 \
    gstreamer1.0-pulseaudio

# RUN apt-get update && apt-get install -y ffmpeg

COPY Files/a2d7c6b3-d59a-489f-b4b7-26932fa50855.mp3 .

# gst-launch-1.0 filesrc location=a2d7c6b3-d59a-489f-b4b7-26932fa50855.mp3 ! mpegaudioparse ! mpg123audiodec ! audioconvert ! audioresample ! tcpserversink host=localhost

# gst-launch-1.0 -v filesrc location="a2d7c6b3-d59a-489f-b4b7-26932fa50855.mp3" ! mpegaudioparse ! tcpserversink port=7001 host=localhost

# ffmpeg -y -i "a2d7c6b3-d59a-489f-b4b7-26932fa50855.mp3" -c:a aac -b:a 128k -muxdelay 0 -f segment -sc_threshold 0 -segment_time 7 -segment_list "playlist.m3u8" -segment_format mpegts "file%d.m4a"

EXPOSE 7001