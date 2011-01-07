CSC=gmcs
CSFLAGS=-r:System.Windows.Forms.dll -r:System.Drawing.dll -r:System.Data.dll
PROGNAME=-out:graphite.exe

FILES=graphite.cs graph.cs mainwin.cs scene.cs state.cs document.cs shapes.cs \
	math.cs view.cs shape-selector.cs

graphite: plugin-interface $(FILES)
	$(CSC) $(FILES) $(CSFLAGS) $(PROGNAME) -r:plugin-interface.dll

plugin-interface: plugin-interface.cs
	$(CSC) plugin-interface.cs -target:library -out:plugin-interface.dll
