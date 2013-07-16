## TODO for GTD/BTW project

#### Add ability to add actions from within the project

> Assigned: Rinat

#### Clean up presenter model

We want to make the model serializable (no internal object references) but
return clean read-only snapshots which could be safely passed around the UI

This is done by internalizing mutable objects of client model and returning
only immutable values (with object graphs, if needed)

#### Add ability to mark actions as non-complete

> Assigned: Kerry



#### Import date/time parsing rules from OmniFocus

We want to have proper support for terms like: now, 2w, tomorrow, today, 3 days etc

### Add perspectives to the domain model

Perspective is just a collection of filters applied to the CD action. We can keep these settings inside an aggregate, making them available to all clients


#### Add Contexts

Subj, as taken from OmniFocus

#### Add Notes to actions/projects

#### Add Folders for the projects


#### Switch all date times to DateTimeOffset



