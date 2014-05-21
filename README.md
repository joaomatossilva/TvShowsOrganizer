TvShowsOrganizer
================

Tv Shows Organizer is a simple script I use to organize the tv shows files that are saved all on the same folder
from my Download Manager tool on my NAS, to the layout that is required for Xbmc.

It should transfom this

    |.  
    |..  
    |-- Showname1.S01E01.avi  
    |-- Showname1.S01E02.avi  
    |-- Showname1.S02E01.avi  
    |-- Showname2.S01E01.avi  
    |-- Showname2.0102.avi 
    |
    |-- Showname2.S01E03
    |  |--Showname2.0103.avi
    |
    |-- Showname2.0104
    |  |--Showname2.S01E04.avi

Into to this:

    |.  
    |..  
    |-- Showname1  
    |  |-- Season 1  
    |  |  |-- Showname1.S01E01.avi  
    |  |  |-- Showname1.S01E02.avi  
    |  |   
    |  |-- Season 2  
    |  |  |-- Showname1.S02E01.avi  
    |  
    |-- Showname2  
    |  |-- Season 1  
    |  |  |-- Showname2.S01E01.avi  
    |  |  |-- Showname2.0102.avi
    |  |  |-- Showname2.0103.avi
    |  |  |-- Showname2.S01E04.avi 

Configuration
==

Tv Shows Organizer uses TvShowsOrganizer.exe.config.
InputFolder: Path where to look for shows
OutputFolder Root Path where the tv show should be moved

Install
==

1. Clone and compile
2. Configure TvShowsOrganizer.exe.config
3. Add a cron job for it