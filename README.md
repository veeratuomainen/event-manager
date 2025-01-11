# Event Manager
## A command line interface for managing events

I made this program in May 2024.

This command line program uses a events.csv file which contains information about different events. With different command line commands, the user can either list, add or delete events from the file. 

### Test the code

To be able to test the code for yourself, you need to have dotnet installed. In addition, CsvHelper library should be installed. This can be installed using dotnet: 

dotnet add package CsvHelper

After cloning the repository, change into the event-manager directory. The program can now be run using command 'dotnet run' plus wanted parameters. The parameters are listed below. Change the date, categories etc. to do your own testing. The events.csv file contains example data about historical events for testing.

days list -> shows all events in the file
days list today -> shows all events with todays date
days list --before-date 2020-02-04 -> shows all events before given date
days list --after-data 2020-02-04 -> shows all events after given date
days list --date 2020-02-04 -> shows all events with given date
days list --categories sport,history -> shows all events with given category/categories
days list --categories sport --exlude -> shows all events without given category
days add --date 2020-02-04 --category sport --description "Football game" -> adds event with given info
days add --category sport --description "Football game" -> adds event with given info and todays date
days delete --description "Foot" -> deletes events whose description start with given text
days delete --date 2020-02-04 -> deletes events with given date
days delete --category sport -> deletes events with given category
days delete --description "Foot" --date 2020-02-04 --category sport -> deletes events with given date, category and description start
days delete --all -> deletes all events
days delete --all --dry-run -> tells what would be deleted without deleting, can also be used with other delete commands
