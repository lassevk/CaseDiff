# CaseDiff

This simple tool is a git difftool example that only shows lines that differ by case.

It will ignore spaces at the start and end of each line when showing the differences, and it will show the
entire lines that have changes.

## Limitations

* Lines that have changes other than case changes will not be output

    The tool can be modified to do this as well but will require more work.
* It's not optimized
* It's not 100% tested

# To install

1. Clone the repository
2. Build the code
3. Add the following to your global `.gitconfig`:

        [difftool "casediff"]
	        cmd = 'C:\\Full\\Path\\To\\CaseDiff.exe' \"$LOCAL\" \"$REMOTE\" \"$BASE\"
	        
    Note that the $LOCAL and so on should be written as-is, but fix the path to the executable, and make sure you
    use double backslashes
    
# To use

Execute commands like the following:

    git difftool -t casediff --staged
