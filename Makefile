CSC=gmcs
CSFLAGS=-r:System.Windows.Forms.dll -r:System.Drawing.dll -r:System.Data.dll
PROGNAME=-out:graphite.exe

FILES=graphite.cs graph.cs mainwin.cs scene.cs state.cs document.cs

graphite: $(FILES)
	$(CSC) $(FILES) $(CSFLAGS) $(PROGNAME)
