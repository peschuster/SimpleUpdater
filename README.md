# SimpleUpdater

In short: SimpleUpdater is an application updater, which checks an xml feed for new updates, downloads them, closes the application, replaces old files and starts the application again.

## Features

* Download of xml feed for update checks
* Dependencies between versions (e.g. you can specify that to update from version 1.0 to 2.1 the user needs to update to 2.0 first)
* RSA signature check on xml version feed
* MD5 hash check of all downloaded files
* Closes the parent application before updates and starts it afterwards again
* ...

## How to use...

Deploy `SimpleUpdater.Core.dll` and `SimpleUpdater.exe` with your application.
Check for updates and initiaited instalation of updates through `SimpleUpdater.Core.UpdateChecker`.

 

To be continued...