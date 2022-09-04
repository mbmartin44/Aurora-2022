all: emailClientMain.exe

emailClientMain.exe: emailClientMain.cs emailClientFunc.cs contacts.cs textClientFunc.cs
	@mcs emailClientMain.cs emailClientFunc.cs contacts.cs textClientFunc.cs

clean:
	@rm -f emailClientMain.exe

run: emailClientMain.exe
	@mono emailClientMain.exe
