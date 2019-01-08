import templateGenerator
import xtemplate
import xutil
import enum
import copy
import os


class Variables(enum.Enum):
    DOTSOUNDPATH = "{DOTSOUNDPATH}"
    MODID = "{MODID}"
    SOUNDPATH = "{SOUNDPATH}"
    SUBTITLE = "{SUBTITLE}"
    REPLACE = "{REPLACE}"
    VOLUME = "{VOLUME}"
    PITCH = "{PITCH}"
    WEIGHT = "{WEIGHT}"
    STREAM = "{STREAM}"
    ATTENUATION_DISTANCE = "{ATTENUATION_DISTANCE}"
    PRELOAD = "{PRELOAD}"
    TYPE = "{TYPE}"


class Templates(enum.Enum):
    SINGLECOMMON = "[SINGLECOMMON]"
    SINGLESUBTITLE = "[SINGLESUBTITLE]"
    SINGLESTREAM = "[SINGLESTREAM]"
    SINGLESUBTITLESSTREAM = "[SINGLESUBTITLESSTREAM]"
    MULTIPLECOMMON = "[MULTIPLECOMMON]"
    MULTIPLESUBTITLE = "[MULTIPLESUBTITLE]"
    MULTIPLESTREAM = "[MULTIPLESTREAM]"
    MULTIPLESUBTITLESTREAM = "[MULTIPLESUBTITLESTREAM]"


class SoundGenerator(templateGenerator.Generator):
    """
    python soundGenerator.py [templateFilePath] [soundsFolder] [modid] [..args..]
    """

    def __init__(self, templatePath, template, soundsLocation, modid):
        templateGenerator.Generator.__init__(self, templatePath, template.value)
        self.soundsFolder = xutil.getPathOrExit(soundsLocation)
        self.modid = modid
        self.newFilePath = os.path.join(self.soundsFolder.replace(os.path.split(self.soundsFolder)[1], ""), "sounds.json")
        self.sounds = xutil.getFilesUnderPath(self.soundsFolder)
        self.soundsPaths = [file.replace(self.soundsFolder, "").replace("\\", "/")[1:] for file in self.sounds]

    def __setVariables__(self, template, **kwargs):
        path = kwargs.get("path")
        dottedPath = path.replace("/", ".")
        template.setVariable(Variables.SOUNDPATH.value, path)
        template.setVariable(Variables.DOTSOUNDPATH.value, dottedPath)
        template.setVariable(Variables.MODID.value, self.modid)
        template.setVariable(Variables.SUBTITLE.value, dottedPath)
        template.setVariable(Variables.REPLACE.value, kwargs.get("replace", "false"))
        template.setVariable(Variables.VOLUME.value, kwargs.get("volume", "1"))
        template.setVariable(Variables.PITCH.value, kwargs.get("pitch", "1"))
        template.setVariable(Variables.WEIGHT.value, kwargs.get("weight", "1"))
        template.setVariable(Variables.STREAM.value, kwargs.get("stream", "false"))
        template.setVariable(Variables.ATTENUATION_DISTANCE.value, kwargs.get("attenuation_distance", "1"))
        template.setVariable(Variables.PRELOAD.value, kwargs.get("preload", "false"))
        template.setVariable(Variables.TYPE.value, kwargs.get("type", "sound"))

    def __process__(self, file):
        file.write(xutil.JSON.startLine)
        pathIndex = 0
        lastIndex = len(self.soundsPaths) - 1
        for path in self.soundsPaths:
            template = self.getTemplate()
            self.__setVariables__(template, path=path)
            if not pathIndex == lastIndex:
                file.write(template.value + ",\n")
            else:
                file.write(template.value)
            pathIndex += 1
        file.write(xutil.JSON.endLine)
