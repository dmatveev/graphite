CSC=gmcs
CSFLAGS=-r:System.Windows.Forms.dll -r:System.Drawing.dll
PROGNAME=-out:graphite.exe

FILES=graphite.cs graph.cs mainwin.cs scene.cs state.cs document.cs visuals.cs

graphite: $(FILES)
	$(CSC) $(FILES) $(CSFLAGS) $(PROGNAME)
