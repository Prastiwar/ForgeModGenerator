import os


class JSON():
    startLine = "{\n"
    endLine = "\n}"


def indexOf(list, value, start=-1, stop=-1):
    start = 0 if start < 0 else start
    stop = len(list) if stop < 0 else stop
    try:
        return list.index(value, start, stop)
    except ValueError:
        return -1


def getFilesUnderPath(path):
    # Gets list of all files under path (includes all files in other folders under path) - recursively
    filesFound = []
    files = os.listdir(path)
    for file in files:
        if os.path.isdir(os.path.join(path, file)):
            filesFound += getFilesUnderPath(os.path.join(path, file))
        else:
            filesFound.append(os.path.join(path, os.path.splitext(file)[0]))
    return filesFound


def getPathOrExit(path):
    if not os.path.exists(path):
        print(f'Path doesnt exist: {path}')
        exit(1)
    return path
