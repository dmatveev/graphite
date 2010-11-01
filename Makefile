CSC=gmcs
CSFLAGS=-r:System.Windows.Forms.dll -r:System.Drawing.dll
PROGNAME=-out:graphite.exe

FILES=graphite.cs graph.cs

graphite: $(FILES)
	$(CSC) $(FILES) $(CSFLAGS) $(PROGNAME)
