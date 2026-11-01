#include <stdint.h>
#include <stdio.h>
#include <fcntl.h>
#include <sys/file.h>
#include <unistd.h>

// IL2CPP marshals UTF-8 strings as char*. Private entry points avoid conflicting
// declarations of Apple's const-char* POSIX functions in generated C++ units.
// Callers retain all path, lock, and atomicity checks. Return the syscall result
// directly, leaving errno available to the unchanged SetLastError P/Invokes.
extern "C" int32_t stp_posix_rename(char* source, char* destination) {
    return rename(source, destination);
}

extern "C" int32_t stp_posix_open(char* path, int32_t flags) {
    return open(path, flags);
}

extern "C" int32_t stp_posix_flock(int32_t descriptor, int32_t operation) {
    return flock(descriptor, operation);
}

extern "C" int32_t stp_posix_close(int32_t descriptor) {
    return close(descriptor);
}
