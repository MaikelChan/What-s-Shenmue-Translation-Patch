"%ProgramFiles%\7-zip\7z" a -tzip WhatsShenmueTranslation.zip WhatsShenmueTranslation\ -mx0
lzma e WhatsShenmueTranslation.zip data -d23 -mt8
del WhatsShenmueTranslation.zip
pause