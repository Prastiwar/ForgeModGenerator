import os
import enum
import xutil


class CommonKey(enum.Enum):
    END = "[END]"
    CUSTOM = "[CUSTOM]"


class Template():
    def __init__(self, filePath, id):
        self.id = id
        self.filePath = filePath
        with open(filePath, "r") as templateFile:
            template = templateFile.readlines()
            startIndex = xutil.indexOf(template, self.id + '\n')
            endIndex = xutil.indexOf(template, CommonKey.END.value + "\n", startIndex)
            self.value = "".join(template[startIndex + 1:endIndex]).rstrip("\n")

    def setVariable(self, variable, value):
        self.value = self.value.replace(variable, value)
