This is a small project I worked on towards the end of 2012.
It was an attempt to allow custom ROM maintainers to provide incremental OTA updates for their user base to avoid the unnecessary large ROM updates, in retrospective I consider this a poor attempt to perhaps good idea, and if i where to go back and redo this, I'd make 1 simple change, I'd do this with a python script instead.

I have been told it's still being used in the android community, so i dug up the code incase anyone would like to make some adjustments for their use case.
but a quick disclaimer, I've also haven't looked at the code since 2012 and im posting this blind, if you do use this, I'd highly recommend you considering writing something similiar in a scripting langauge for portability (if you look below you'd see the design behind this project was rather simple).

How does this work?
* is compares files in 2 directory, the 2 directory are 2 different ROM revisions or updates
    * if new file in added in the 2nd directory it's copied over
    * if file has md5sum changed between the 2 directory, it copies the modified one over
* It will always use the update script from the newer update
