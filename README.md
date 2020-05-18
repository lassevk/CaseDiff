# CaseDiff

This simple tool is a git difftool example that only shows lines that differ by case.

It will ignore spaces at the start and end of each line when showing the differences, and it will show the
entire lines that have changes.

## Limitations

* Lines that have changes other than case changes will not be output

    The tool can be modified to do this as well but will require more work.
* It's not optimized
* It's not 100% tested
