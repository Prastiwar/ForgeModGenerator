import xtemplate
import xutil
import copy


class Generator():
    """
    python [generatorName].py [templateFilePath] [..args..]
    """

    def __init__(self, templatePath, templateId):
        self.templateId = templateId
        self.templatePath = xutil.getPathOrExit(templatePath)
        self.newFilePath = None
        self.template = self.createTemplate(self.templateId)

    def createTemplate(self, id):
        return xtemplate.Template(self.templatePath, id)

    def copyTemplate(self, template):
        return copy.copy(template)

    def getTemplate(self):
        return self.copyTemplate(self.template)

    def generate(self):
        with open(self.newFilePath, "w+") as file:
            self.__process__(file)

    def __setVariables__(self, template, **kwargs):
        raise NotImplementedError

    def __process__(self, file):
        template = self.getTemplate()
        self.__setVariables__(template)
        file.write(template.value)
