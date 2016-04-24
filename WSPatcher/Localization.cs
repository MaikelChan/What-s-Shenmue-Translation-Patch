
namespace WSPatcher
{
	public static class Localization
	{
		//Main window
		public static string MainFormTitle;
		public static string MainFormBrowse;
		public static string MainFormPatch;
		public static string MainFormAbout;
		public static string MainFormODlgFilter;
		public static string MainFormOptions;
		public static string MainFormLanguage;
		public static string MainFormOutputFormat;
		public static string MainFormOutputFormatGDI;
		public static string MainFormOutputFormatCDI;

		//About window
		public static string AboutFormOK;

		//Message box
		public static string MsgBoxInfo;
		public static string MsgBoxWarning;
		public static string MsgBoxError;
		public static string MsgBoxSuccess;
		public static string MsgBoxBusy;
		public static string MsgBoxDeleteTempError;
		public static string MsgBoxProcessError;
		public static string MsgBoxFileNotFound;
		public static string MsgBoxNoDisc;
		public static string MsgBoxInvalidGDI;
		public static string MsgBoxTrackNotFound;
		public static string MsgBoxGDIWrongOrder;
		public static string MsgBoxGDIWrongOrder2;
		public static string MsgBoxConvertingTracksError;
		public static string MsgBoxAFSExtractingError;
		public static string MsgBoxFinalInfo;

		//Status bar
		public static string StatusWaiting;
		public static string StatusCheckingFiles;
		public static string StatusExtractingFiles;
		public static string StatusPatchingXDelta;
		public static string StatusConvertingTracksToISO;
		public static string StatusExtractingGDI;
		public static string StatusExtractingAFS;
		public static string StatusReplacingSRF;
		public static string StatusRegeneratingAFS;
		public static string StatusBuildingFinalISO;

		//Variables
		public static string VarPatchFolderSuffix;
		public static string VarPatchISOPathNameSuffix;
		public static string VarPatchISONameSuffix;

		public static Languages SelectedLanguage;

		public enum Languages
		{
			English,
			Spanish,
			German,
			Portuguese
		}

		public static void LoadLanguage(Languages Language)
		{
			switch (Language)
			{
				case Languages.English:

					//Main window
					MainFormTitle = "{0} in English  -  v{1}.{2}.{3}";
					MainFormBrowse = "Browse...";
					MainFormPatch = "Patch";
					MainFormAbout = "About...";
					MainFormODlgFilter = "GDI files (*.gdi)|*.gdi|All files (*.*)|*.*";
					MainFormOptions = "Options";
					MainFormLanguage = "Language:";
					MainFormOutputFormat = "Output format:";
					MainFormOutputFormatGDI = "GDI (for emulators)";
					MainFormOutputFormatCDI = "MDS/MDF (for consoles)";

					//About window
					AboutFormOK = "OK";

					//Message box
					MsgBoxInfo = "Information";
					MsgBoxWarning = "Warning";
					MsgBoxError = "Error";
					MsgBoxSuccess = "Patching process has been completed successfully.";
					MsgBoxBusy = "The patcher is busy. You'll have to wait till it finishes.";
					MsgBoxDeleteTempError = "There was a problem trying to delete temporary files.\n\nTry again and make sure you don't have the temporary files folder open with Windows Explorer.";
					MsgBoxProcessError = "Code {0} after executing:\n\n{1}\n\nPress Ctrl+C to copy this message to the clipboard.";
					MsgBoxFileNotFound = "File not found:\n\n";
					MsgBoxNoDisc = "There's no disc to patch.";
					MsgBoxInvalidGDI = "The file specified as disc {0} is not a valid GDI file.";
					MsgBoxTrackNotFound = "File not found: \"{0}\".\n\nMake sure that the {1} tracks of the disc are in the same folder as the GDI file. In this case:\n\n\"{2}\".";
					MsgBoxGDIWrongOrder = "The specified GDI file does not correspond to disc {0} of {1} version {2}.";
					MsgBoxGDIWrongOrder2 = "You've specified GDI {0} of {1} as if it was GDI {2}. Specify the GDI files in the correct order.";
					MsgBoxConvertingTracksError = "There was a problem converting the GDI {0} tracks to ISO format.\n\nThe patcher cannot continue.";
					MsgBoxAFSExtractingError = "There was a problem while trying to extract the AFS file:\n\n";
					MsgBoxFinalInfo = "Please remember to select \"Adult\" in Kids Mode and \"Game Mode\" in Dialog & Text inside the Options Menu. Otherwise, the subtitles will not show up.";

					//Status bar
					StatusWaiting = "Waiting...";
					StatusCheckingFiles = "Checking that all necessary files exist...";
					StatusExtractingFiles = "Extracting necessary files...";
					StatusPatchingXDelta = "Patching XDeltas...";
					StatusConvertingTracksToISO = "Converting GDI {0} tracks to ISO format...";
					StatusExtractingGDI = "Extracting GDI {0}...";
					StatusExtractingAFS = "Extracting AFS files from disc {0}...";
					StatusReplacingSRF = "Replacing SRF files from disc {0}...";
					StatusRegeneratingAFS = "Regenerating AFS files from disc {0}...";
					StatusBuildingFinalISO = "Generating final ISO from disc {0}...";

					//Variables
					VarPatchFolderSuffix = "-EN";
					VarPatchISOPathNameSuffix = " in English";
					VarPatchISONameSuffix = " [T-En]";

					break;

				case Languages.Spanish:

					//Main window
					MainFormTitle = "{0} en castellano  -  v{1}.{2}.{3}";
					MainFormBrowse = "Examinar...";
					MainFormPatch = "Parchear";
					MainFormAbout = "Acerca de...";
					MainFormODlgFilter = "Archivos GDI (*.gdi)|*.gdi|Todos los archivos (*.*)|*.*";
					MainFormOptions = "Opciones";
					MainFormLanguage = "Idioma:";
					MainFormOutputFormat = "Formato de salida:";
					MainFormOutputFormatGDI = "GDI (para emuladores)";
					MainFormOutputFormatCDI = "MDS/MDF (para consolas)";

					//About window
					AboutFormOK = "Aceptar";

					//Message box
					MsgBoxInfo = "Información";
					MsgBoxWarning = "Advertencia";
					MsgBoxError = "Error";
					MsgBoxSuccess = "El proceso se ha terminado correctamente.";
					MsgBoxBusy = "El parcheador está ocupado. Tendrás que esperar a que termine.";
					MsgBoxDeleteTempError = "Hubo un problema al intentar borrar los archivos temporales.\n\nInténtalo de nuevo y asegúrate de que no tienes abierta la carpeta de archivos temporales con el explorador de Windows.";
					MsgBoxProcessError = "Código {0} al ejecutar:\n\n{1}\n\nPulsa Ctrl+C para copiar este mensaje al portapapeles.";
					MsgBoxFileNotFound = "No se ha encontrado el archivo:\n\n";
					MsgBoxNoDisc = "No se ha introducido ningún disco para parchear.";
					MsgBoxInvalidGDI = "El archivo introducido como Disco {0} no es un archivo GDI válido.";
					MsgBoxTrackNotFound = "No se ha encontrado el archivo \"{0}\".\n\nAsegúrate de que las {1} pistas del disco están en la misma carpeta que el archivo GDI. En este caso:\n\n\"{2}\".";
					MsgBoxGDIWrongOrder = "El GDI que has seleccionado no corresponde al disco {0} de {1} versión {2}.";
					MsgBoxGDIWrongOrder2 = "Has seleccionado el GDI {0} de {1} como si fuera el {2}. Introduce los GDIs en el orden correspondiente.";
					MsgBoxConvertingTracksError = "Ha habido un problema al convertir las pistas del GDI {0} a formato ISO.\n\nEl parcheador no puede continuar.";
					MsgBoxAFSExtractingError = "Ha habido un problema extraer el archivo AFS:\n\n";
					MsgBoxFinalInfo = "Recuerda configurar el juego seleccionando \"Adultos\" en el Modo infantil y \"Modo Juego\" en la sección \"Voces/Textos\" dentro del menú de opciones. De lo contrario, no se mostrarán los subtítulos.";

					//Status bar
					StatusWaiting = "En espera...";
					StatusCheckingFiles = "Comprobando la existencia de los archivos necesarios...";
					StatusExtractingFiles = "Extrayendo los archivos necesarios del parcheador...";
					StatusPatchingXDelta = "Parcheando XDeltas...";
					StatusConvertingTracksToISO = "Convirtiendo tracks del GDI {0} a formato ISO...";
					StatusExtractingGDI = "Extrayendo GDI {0}...";
					StatusExtractingAFS = "Extrayendo archivos AFS del disco {0}...";
					StatusReplacingSRF = "Reemplazando archivos SRF del disco {0}...";
					StatusRegeneratingAFS = "Regenerando archivos AFS del disco {0}...";
					StatusBuildingFinalISO = "Generando ISO final del disco {0}...";

					//Variables
					VarPatchFolderSuffix = "-ES";
					VarPatchISOPathNameSuffix = " en castellano";
					VarPatchISONameSuffix = " [T-Esp]";

					break;

				case Languages.German:

					//Main window
					MainFormTitle = "{0} auf Deutsch  -  v{1}.{2}.{3}";
					MainFormBrowse = "Durchsuchen...";
					MainFormPatch = "Patchen";
					MainFormAbout = "Über uns...";
					MainFormODlgFilter = "GDI Dateien (*.gdi)|*.gdi|Alle Dateien (*.*)|*.*";
					MainFormOptions = "Optionen";
					MainFormLanguage = "Sprache:";
					MainFormOutputFormat = "Ausgabeformat:";
					MainFormOutputFormatGDI = "GDI (für Emulatoren)";
					MainFormOutputFormatCDI = "MDS/MDF (für Konsolen)";

					//About window
					AboutFormOK = "OK";

					//Message box
					MsgBoxInfo = "Information";
					MsgBoxWarning = "Warnung";
					MsgBoxError = "Fehler";
					MsgBoxSuccess = "Der Patchvorgang wurde erfolgreich abgeschlossen.";
					MsgBoxBusy = "Der Patcher arbeitet. Du musst warten, bis er fertig ist.";
					MsgBoxDeleteTempError = "Es gab ein Problem dabei, die temorären Dateien zu löschen.\n\nVersuche es erneut und stelle sicher, das du nicht den Ordner mit den temporären Dateien im Windows Explorer geöffnet hast.";
					MsgBoxProcessError = "Code {0} nach Ausführung:\n\n{1}\n\nDrücke Strg+C um diese Nachricht in die Zwischenablage zu kopieren.";
					MsgBoxFileNotFound = "Datei nicht gefunden:\n\n";
					MsgBoxNoDisc = "Es gibt keine Disc zu patchen.";
					MsgBoxInvalidGDI = "Die Datei, die als Disc {0} angegeben wurde, ist keine gültige GDI Datei.";
					MsgBoxTrackNotFound = "Datei nicht gefunden: \"{0}\".\n\nStelle sicher, dass die {1} Tracks der Disc are im selben Ordner sind, wie die GDI Datei. In dem Falle:\n\n\"{2}\".";
					MsgBoxGDIWrongOrder = "Die gewählte GDI Datei passt nicht zu der Disc {0} von {1} Version {2}.";
					MsgBoxGDIWrongOrder2 = "Du hast die GDI {0} von {1} gewählt, als wäre sie GDI {2}. Wähle die GDI Dateien in der richtigen Reihenfolge.";
					MsgBoxConvertingTracksError = "Es ist ein Problem dabei aufgetreten, die GDI {0} Tracks zum ISO Format umzuwandeln.\n\nDer Patcher kann nicht fortfahren.";
					MsgBoxAFSExtractingError = "Es gab ein Problem bei dem Extrahieren von AFS Datei:\n\n";
					MsgBoxFinalInfo = "Denk bitte daran, \"Adult\" im Kindermodus und \"Spielmodus\" bei Dialog & Text im Optionsmenü einzustellen. Die Untertitel werden sonst nicht angezeigt.";

					//Status bar
					StatusWaiting = "Warte...";
					StatusCheckingFiles = "Überprüfe, ob alle notwendigen Dateien vorhanden sind...";
					StatusExtractingFiles = "Extrahiere notwendige Dateien...";
					StatusPatchingXDelta = "Patche XDeltas...";
					StatusConvertingTracksToISO = "Konvertiere GDI {0} Tracks ins ISO Format...";
					StatusExtractingGDI = "Extrahiere GDI {0}...";
					StatusExtractingAFS = "Extrahiere AFS Dateien von Disc {0}...";
					StatusReplacingSRF = "Ersetze SRF Dateien von Disc {0}...";
					StatusRegeneratingAFS = "Regeneriere AFS Dateien von Disc {0}...";
					StatusBuildingFinalISO = "Generiere finale ISO von Disc {0}...";

					//Variables
					VarPatchFolderSuffix = "-DE";
					VarPatchISOPathNameSuffix = " auf Deutsch";
					VarPatchISONameSuffix = " [T-De]";

					break;

				case Languages.Portuguese:

					//Main window
					MainFormTitle = "{0} em Português  -  v{1}.{2}.{3}";
					MainFormBrowse = "Procurar...";
					MainFormPatch = "Patch";
					MainFormAbout = "Sobre...";
					MainFormODlgFilter = "Arquivos GDI (*.gdi)|*.gdi|Todos os arquivos (*.*)|*.*";
					MainFormOptions = "Opções";
					MainFormLanguage = "Idioma:";
					MainFormOutputFormat = "Formato de Saída:";
					MainFormOutputFormatGDI = "GDI (para emuladores)";
					MainFormOutputFormatCDI = "MDS/MDF (para consoles)";

					//About window
					AboutFormOK = "OK";

					//Message box
					MsgBoxInfo = "Informação";
					MsgBoxWarning = "Aviso";
					MsgBoxError = "Erro";
					MsgBoxSuccess = "O processo de patching foi concluído com êxito.";
					MsgBoxBusy = "O patcher está ocupado. Você precisa esperar até que ele termine.";
					MsgBoxDeleteTempError = "Houve um problema ao tentar deletar arquivos temporários.\n\nTente novamente e tenha certeza de que você não tem a pasta de arquivos temporários aberta no seu Windows Explorer.";
					MsgBoxProcessError = "Código {0} após executar:\n\n{1}\n\nPressione Ctrl+C para copiar esta mensagem.";
					MsgBoxFileNotFound = "Arquivo não encontrado:\n\n";
					MsgBoxNoDisc = "Não existe nenhum disco para fazer o patch.";
					MsgBoxInvalidGDI = "O arquivo especificado como disco {0} não é um arquivo GDI válido.";
					MsgBoxTrackNotFound = "Arquivo não encontrado: \"{0}\".\n\nTenha certeza de que {1} as trilhas do disco estão na mesma pasta que o arquivo GDI. Neste caso:\n\n\"{2}\".";
					MsgBoxGDIWrongOrder = "O arquivo GDI especificado não corresponde ao disco {0} de {1} versão {2}.";
					MsgBoxGDIWrongOrder2 = "Você especificou o GDI {0} de {1} como se fosse o GDI {2}. Especifique os arquivos GDI na ordem correta.";
					MsgBoxConvertingTracksError = "Houve um problema em converter o GDI {0} de trilhas para o formato ISO.\n\nO patcher não pode continuar.";
					MsgBoxAFSExtractingError = "Houve um problema ao tentar extrair os arquivos AFS:\n\n";
					MsgBoxFinalInfo = "Por favor lembre-se de selecionar \"Adulto\" no Modo Crianças e \"Modo Jogo\" em Diálogos & Texto dentro do Menu de Opções. Caso contrário, as legendas não aparecerão.";

					//Status bar
					StatusWaiting = "Aguardando...";
					StatusCheckingFiles = "Verificando que todos os arquivos necessários existem...";
					StatusExtractingFiles = "Extraindo os arquivos necessários...";
					StatusPatchingXDelta = "Patching XDeltas...";
					StatusConvertingTracksToISO = "Convertendo trilhas GDI {0} para o formato ISO...";
					StatusExtractingGDI = "Extraindo GDI {0}...";
					StatusExtractingAFS = "Extraindo os arquivos AFS do disco {0}...";
					StatusReplacingSRF = "Substituindo os arquivos SRF do disco {0}...";
					StatusRegeneratingAFS = "Regenerando os arquivos AFS do disco {0}...";
					StatusBuildingFinalISO = "Gerando a ISO final do disco {0}...";

					//Variables
					VarPatchFolderSuffix = "-BR";
					VarPatchISOPathNameSuffix = " em Português";
					VarPatchISONameSuffix = " [T-PtBr]";

					break;
			}

			SelectedLanguage = Language;
		}
	}
}