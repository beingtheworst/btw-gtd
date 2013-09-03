# Being The Worst - Expedition 2 #

This repository contains the sample code that is associated with the [Being The Worst](http://beingtheworst.com/about) podcast episodes that cover the show's second expedition.  The start of the second expedition is [Episode 19 - The Hermit: An Unexpected Journeyman](http://beingtheworst.com/2012/episode-19-the-hermit-an-unexpected-journeyman).  Here is a [list of things](https://github.com/beingtheworst/btw-gtd/blob/master/btw/Itinerary_for_Expedition_2.md) that we may try to cover in expedition 2.

The sample source code in this repository is our attempt to implement a productivity management system that is inspired by "[Getting Things Done (R)](http://www.amazon.com/Getting-Things-Done-Stress-Free-Productivity/dp/0142000280)" by [David Allen](http://www.davidco.com/).  We will use DDD and Aggregates with Event Sourcing as our primary implementation approach (as described in the [podcast episodes](http://beingtheworst.com/episodes-en)).

## Directory Structure of this Repository ##
- **Build NOTE!** - The "Gtd Mobile" solution folder contains projects that have NuGet package references.  NuGet 2.7+ now has automatic package restore on build. We rely on this feature and thus did NOT commit the packages to git.  There is currently [a bug](https://nuget.codeplex.com/workitem/3640) in Nuget though that prevents automatic package restore if a project is under a solution folder. Until that is fixed, simply right-click on the Solution-->Manage NuGet Packages for the Solution and click "Restore" in the message window that pops up that tells you some packages are missing. Rebuild the solution and the Windows Phone 8 project (Gtd.Client.WindowsPhone) should build. (Note that the [Windows Phone 8 SDK](http://dev.windowsphone.com/en-us/downloadsdk) is a free download but it requires Windows 8 64-bit because of the use of Hyper-V to deliver the phone emulators. If you can't load this SDK, the Windows Phone project will be unavailable in VS, but the rest of the code and the WinForms client (Gtd.Client) should continue to work as expected.)   

- **"\"** any files/sub-directories in the repo's root directory (**btw-gtd**) (other than the  git, license, and readme files and the \btw directory) should be DIRECLTY related to the files one would use to implement the solution in a production environment. 
- **\btw** and all of its sub-directories are Being The Worst housekeeping and general episode files

# [What is Being The Worst?](http://beingtheworst.com/about) #

"Being The Worst" is a community for those that enjoy continuous learning and embracing humble software craftsmanship.  Our podcast is one format that we use to share information and interact with the community.

You can **freely subscribe to the Being The Worst podcast** via our [RSS feed](http://beingtheworst.com/feed) or [iTunes](http://itunes.apple.com/us/podcast/being-the-worst/id554597082).

# What can I learn and where do I start? #

Learning Expeditions 1 & 2 explore Domain-driven design (DDD), Event Sourcing (ES), Command Query Responsibility Segregation (CQRS), & cross-cloud software delivery.

If you are new to Being The Worst, you can start with Expedition 1 [(podcast episodes 1-18)](http://beingtheworst.com/episodes-en).  Kerry and Rinat introduce the show and describe the plan for the road ahead in [Episode 1 – The Worst Welcome.](http://beingtheworst.com/2012/episode-1-the-worst-welcome)  You can also follow us on twitter [@beingtheworst](https://twitter.com/beingtheworst).


Best regards,

Kerry and Rinat