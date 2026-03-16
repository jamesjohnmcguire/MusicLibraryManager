set FileName=%~n1

ffmpeg -i %1 %FileName%.wav
